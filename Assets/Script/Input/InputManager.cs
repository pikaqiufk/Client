using System;
#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public InputManager()
    {
        Instance = this;
    }

    private bool buttonDown = false;
    private bool mBIsMouseDbClick;
    private int mSceneTouchFingerId = -1;
    private float mSkipTime;
    public DelegateOnMoveDestination OnMoveDestination;
    public DelegateSelectTarget SelectTarget;
    //鼠标按键枚举
    public enum MOUSE_BUTTON
    {
        MOUSE_BUTTON_LEFT,
        MOUSE_BUTTON_RIGHT,
        MOUSE_BUTTON_MIDDLE
    }

    //摇杆状态
    public enum STICK_STATE
    {
        STICK_STATE_FORWARD,
        STICK_STATE_BACKWARD,
        STICK_STATE_LEFT,
        STICK_STATE_RIGHT,
        STICK_STATE_NUM
    }

    public int SceneTouchFingerId
    {
        get { return mSceneTouchFingerId; }
        set { mSceneTouchFingerId = value; }
    }

    public bool SkillButtonPreesed { get; set; }
    public Vector2 SkillButtonPressedPosition { get; set; }

    private void AnalyseMousePos_Click(Vector3 posPressed)
    {
        if (ShouldMove(posPressed))
        {
            return;
        }

        ShouldSelectTarget(posPressed);
    }

    private bool AnalyseMousePos_Stay(Vector3 posPressed)
    {
        if (IsTouchInUI(posPressed))
        {
            return true;
        }

        if (ShouldMove(posPressed))
        {
            return true;
        }

        return false;
    }

    private void AnalyseTouchPos_Click(Vector3 posPressed, Touch touch)
    {
        if (ShouldMove(posPressed))
        {
            return;
        }

        ShouldSelectTarget(posPressed);
    }

    private bool AnalyseTouchPos_Stay(Vector3 posPressed, Touch touch)
    {
        if ((mSceneTouchFingerId == -1) ||
            (mSceneTouchFingerId != -1 && touch.fingerId == mSceneTouchFingerId))
        {
            if (IsTouchInUI(posPressed))
            {
                return true;
            }

            if (ShouldMove(posPressed))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsTouchInUI(Vector3 posPressed)
    {
        if (null != UICamera.mainCamera)
        {
            var nUiRay = UICamera.mainCamera.ScreenPointToRay(posPressed);
            if (Physics.Raycast(nUiRay, 100.0f, LayerMask.GetMask("UI")))
            {
                ReleaseTouch(posPressed);
                return true;
            }
        }
        return false;
    }

    public void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        Instance = null;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void ReleaseTouch(Vector3 posPressed)
    {
        if (mSceneTouchFingerId != -1)
        {
            mSceneTouchFingerId = -1;
        }

        if (null != GameControl.Instance && !ShouldSelectTarget(posPressed))
        {
            GameControl.Instance.OnTouchRelease();
        }
    }

    public void ResetStickState()
    {
    }

    private bool ShouldMove(Vector3 posPressed)
    {
        if (null == GameLogic.Instance)
        {
            return false;
        }

        if (null == ObjManager.Instance.MyPlayer)
        {
            return false;
        }

        if (SkillButtonPreesed)
        {
            return false;
        }

        var ray = GameLogic.Instance.MainCamera.ScreenPointToRay(posPressed);
        RaycastHit hit;

        var layer = ~(1 << LayerMask.NameToLayer("Ignore Raycast"));
        layer &= ~LayerMask.GetMask("MainPlayer");
        if (ObjManager.Instance.MyPlayer.GetCurrentStateName() == OBJ.CHARACTER_STATE.ATTACK ||
            ObjManager.Instance.MyPlayer.GetCurrentStateName() == OBJ.CHARACTER_STATE.RUN)
        {
            layer &= ~(1 << LayerMask.NameToLayer("ObjLogic"));
            layer &= ~(1 << LayerMask.NameToLayer("OtherPlayer"));
        }

        if (Physics.Raycast(ray, out hit, 50, layer) && GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ObjLogic") ||
                hit.collider.gameObject.layer == LayerMask.NameToLayer("OtherPlayer"))
            {
                return false;
            }

            var sceneObj = GameLogic.Instance.Scene.gameObject;
            if (hit.collider.gameObject.transform.IsChildOf(sceneObj.transform))
            {
                if (OnMoveDestination != null)
                {
                    var ok = OnMoveDestination(hit.point);
                    GameLogic.Instance.Scene.ActiveMovingCircle(hit.point, ok ? Color.white : Color.red);
                }

                return true;
            }
        }
        return false;
    }

    private bool ShouldSelectTarget(Vector3 posPressed)
    {
        // 如果是UI，那就算了
        if (null != UICamera.mainCamera)
        {
            var nUiRay = UICamera.mainCamera.ScreenPointToRay(posPressed);
            if (Physics.Raycast(nUiRay, 100.0f, LayerMask.GetMask("UI")))
            {
                return false;
            }
        }

        var pos = posPressed;

        if (GameLogic.Instance == null)
        {
            Logger.Error("GameLogic.Instance is null");
            return false;
        }
        if (GameLogic.Instance.MainCamera == null)
        {
            Logger.Error("GameLogic.Instance.MainCamera is null");
            return false;
        }

        if (GameLogic.Instance.MainCamera.active)
        {
            var l = new List<KeyValuePair<float, ObjBase>>();
            if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
            {
                var sceneObj = GameLogic.Instance.Scene.gameObject;

                GameObject target = null;

                var ray = GameLogic.Instance.MainCamera.ScreenPointToRay(pos);
                var mask = LayerMask.GetMask("ObjLogic", "OtherPlayer");
                var hit = Physics.RaycastAll(ray, 20, mask);
                for (var i = 0; i < hit.Length; ++i)
                {
                    var gameObject = hit[i].collider.gameObject;
                    if (!gameObject.transform.IsChildOf(sceneObj.transform))
                    {
                        ObjBase obj = null;
                        if (null != hit[i].collider.transform.parent)
                        {
                            obj = hit[i].collider.transform.parent.GetComponent<ObjBase>();
                        }

                        if (obj == null)
                        {
                            obj = hit[i].collider.GetComponent<ObjBase>();
                        }

                        if (obj == null)
                        {
                            continue;
                        }

                        l.Add(new KeyValuePair<float, ObjBase>(hit[i].distance, obj));
                        var npc = obj as ObjNPC;
                        if (npc && !ObjManager.Instance.MyPlayer.IsMyEnemy(npc))
                        {
                            target = npc.gameObject;
                            break;
                        }
                    }
                }

                if (l.Count > 0)
                {
                    // 最优先选NPC
                    if (target == null)
                    {
                        l.Sort((a, b) =>
                        {
                            // 优先捡东西
                            bool x = true, y = true;
                            if (a.Value is ObjCharacter)
                            {
                                x = ObjManager.Instance.MyPlayer.IsMyEnemy(a.Value as ObjCharacter);
                            }

                            if (b.Value is ObjCharacter)
                            {
                                y = ObjManager.Instance.MyPlayer.IsMyEnemy(b.Value as ObjCharacter);
                            }

                            // 优先选敌方
                            if (x && !y)
                            {
                                return -1;
                            }
                            if (x && y)
                            {
                                // 如果都是敌方，优先选玩家
                                if (a.Value.GetObjType() == OBJ.TYPE.OTHERPLAYER &&
                                    b.Value.GetObjType() != OBJ.TYPE.OTHERPLAYER)
                                {
                                    return -1;
                                }
                            }

                            return (int) (a.Key - b.Key);
                        });

                        target = l[0].Value.gameObject;
                    }

                    //停止当前的指令
                    SelectTarget(target, -1);
                    mSkipTime = 0.5f;

                    return true;
                }

                return false;
            }
        }

        return false;
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif
        if (mSkipTime > 0)
        {
            mSkipTime -= Time.deltaTime;
            return;
        }

        GameLogic.Instance.MultiTouch--;

        //鼠标按键更新
        //首先处理鼠标按下
        //当鼠标按下未处理的话，才去处理鼠标点击
        if (Input.touchCount <= 0)
        {
            if (null != GameControl.Instance && !GameControl.Instance.JoyStickPressed)
            {
                if (false == UpdateButtonStay(false))
                {
                    UpdateButton(false);
                }
            }

            //更新鼠标双击状态
            UpdateMouseDBClick();
        }
        else if (Input.touchCount == 1)
        {
            var InputtouchCount0 = Input.touchCount;
            for (var i = 0; i < InputtouchCount0; i++)
            {
                // 如果是摇杆的fingerID 即按住摇杆划出了摇杆范围 直接continue
                if (JoyStickLogic.Instance() != null)
                {
                    if (Input.GetTouch(i).fingerId == JoyStickLogic.Instance().FingerId)
                    {
                        continue;
                    }
                }

                if (false == UpdateButtonStay(true, i))
                {
                    UpdateButton(true, i);
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            var zoom = true;
            var InputtouchCount1 = Input.touchCount;
            for (var i = 0; i < InputtouchCount1; i++)
            {
                // 如果是摇杆的fingerID 即按住摇杆划出了摇杆范围 直接continue
                if (JoyStickLogic.Instance() != null)
                {
                    if (Input.GetTouch(i).fingerId == JoyStickLogic.Instance().FingerId)
                    {
                        zoom = false;
                        GameLogic.Instance.MultiTouch = 0;
                        break;
                    }
                }

                // 如果手指碰到UI，也不能缩放
                if (null != UICamera.mainCamera)
                {
                    var nUiRay = UICamera.mainCamera.ScreenPointToRay(Input.GetTouch(i).position);
                    RaycastHit nHit;
                    if (Physics.Raycast(nUiRay, out nHit, LayerMask.GetMask("UI")))
                    {
                        zoom = false;
                        GameLogic.Instance.MultiTouch = 0;
                        break;
                    }
                }
            }

            if (zoom)
            {
                if (GameLogic.Instance.MultiTouch >= 0)
                {
//                     if (GameLogic.Instance.MultiPos[0] > Input.touches[0].position.y &&
//                         GameLogic.Instance.MultiPos[1] > Input.touches[1].position.y)
//                     {
//                         var cam = GameLogic.Instance.MainCamera.GetComponent<CameraController>();
//                         cam.FAngle += 3;
//                         if (cam.FAngle > 60)
//                         {
//                             cam.FAngle = 60;
//                         }
//                         cam.Dirty = true;
// 
//                         GameLogic.Instance.MultiPos[0] = Input.touches[0].position.y;
//                         GameLogic.Instance.MultiPos[1] = Input.touches[1].position.y;
//                     }
//                     else if (GameLogic.Instance.MultiPos[0] < Input.touches[0].position.y &&
//                              GameLogic.Instance.MultiPos[1] < Input.touches[1].position.y)
//                     {
//                         var cam = GameLogic.Instance.MainCamera.GetComponent<CameraController>();
//                         cam.FAngle -= 3;
//                         if (cam.FAngle < 35)
//                         {
//                             cam.FAngle = 35;
//                         }
//                         cam.Dirty = true;
// 
//                         GameLogic.Instance.MultiPos[0] = Input.touches[0].position.y;
//                         GameLogic.Instance.MultiPos[1] = Input.touches[1].position.y;
//                     }
//                     else
                    {
                        var l = (Input.touches[0].position - Input.touches[1].position).magnitude;
                        var cam = GameLogic.Instance.MainCamera.GetComponent<CameraController>();
                        cam.Length += (l - GameLogic.Instance.MultiDistance)*cam.ZoomSpeed;
                        cam.Length = Mathf.Clamp(cam.Length, cam.MinLength, cam.MaxLength);
                        cam.FAngle = Mathf.Lerp(35, 25, (cam.MaxLength - cam.Length)/(cam.MaxLength - cam.MinLength));
                        cam.Dirty = true;
                        GameLogic.Instance.MultiDistance = l;
                    }

                    GameLogic.Instance.MultiTouch++;
                }
                else
                {
                    GameLogic.Instance.MultiDistance = (Input.touches[0].position - Input.touches[1].position).magnitude;

                    GameLogic.Instance.MultiPos[0] = Input.touches[0].position.y;
                    GameLogic.Instance.MultiPos[1] = Input.touches[1].position.y;

                    GameLogic.Instance.MultiTouch = 1;
                }
            }
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    //更新鼠标按钮
    private void UpdateButton(bool isTouch, int nTouchIndex = -1)
    {
        //鼠标左键按下
        if ((!isTouch && Input.GetMouseButtonDown((int) MOUSE_BUTTON.MOUSE_BUTTON_LEFT)) || isTouch)
        {
            Vector3 posPressed;
            if (!isTouch)
            {
                posPressed = Input.mousePosition;
                AnalyseMousePos_Click(posPressed);
            }
            else
            {
                var touch = Input.GetTouch(nTouchIndex);
                posPressed = touch.position;
                AnalyseTouchPos_Click(posPressed, touch);
            }
        }
    }

    //处理鼠标按住
    //返回true表示本次update中不会再去更新鼠标点击
    private bool UpdateButtonStay(bool isTouch, int nTouchIndex = -1)
    {
        //鼠标左键按住
        if ((!isTouch && Input.GetMouseButton((int) MOUSE_BUTTON.MOUSE_BUTTON_LEFT)) ||
            isTouch)
        {
            // 如果点击的是UI 则不处理其他鼠标消息 只对UI作出响应
            Vector3 posPressed;
            if (!isTouch)
            {
                posPressed = Input.mousePosition;
                return AnalyseMousePos_Stay(posPressed);
            }
            var touch = Input.GetTouch(nTouchIndex);
            posPressed = touch.position;

            if (touch.phase == TouchPhase.Ended)
            {
                ShouldSelectTarget(posPressed);
            }

            return AnalyseTouchPos_Stay(posPressed, touch);
        }
        if (Input.GetMouseButtonUp((int) MOUSE_BUTTON.MOUSE_BUTTON_LEFT))
        {
            Vector3 posPressed;
            posPressed = Input.mousePosition;
            ReleaseTouch(posPressed);
        }
        return false;
    }

    private void UpdateMouseDBClick()
    {
        var mouseEvent = Event.current;
        if (null == mouseEvent)
        {
            return;
        }

        if (mouseEvent.isMouse && mouseEvent.type == EventType.MouseDown && mouseEvent.clickCount == 2)
        {
            mBIsMouseDbClick = true;
        }
        else
        {
            mBIsMouseDbClick = false;
        }
    }

    public delegate bool DelegateOnMoveDestination(Vector3 dir);

    public delegate bool DelegateSelectTarget(GameObject gameObject, int skill);
}