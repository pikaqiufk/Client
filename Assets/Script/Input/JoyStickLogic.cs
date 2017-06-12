using System;
#region using

using UnityEngine;
using EventSystem;

#endregion

public class JoyStickLogic : MonoBehaviour
{
    public UISprite JoyStickSprite; // 摇杆sprite
    public float MaxRadius = 40.0f; // 摇杆最大拖动半径

    private Vector3 mTouchPos = new Vector3(0, 0, 0); // 手指位置 demo里是鼠标
    private Vector3 mJoyStickOrigin = new Vector3(0, 0, 0); // 摇杆原位置 设置后不再更改    
    //private ProcessInput mProcessInput;                       // 玩家输入
    private Vector3 mMouseBuffer = new Vector3(0, 0, 0); // 鼠标位置缓存

    public delegate void DelegateMoveDirection(Vector2 dir);

    public DelegateMoveDirection OnMoveDirection;

#if (UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
    private float _differenceX = 1f;
    private float _differenceY = 1f;
    private float _difTag = 0.77f;
#endif
    private int mFingerId = -1;

    public int FingerId
    {
        get { return mFingerId; }
    }

    private Transform mTransform;

    private static JoyStickLogic _instance;

    public static JoyStickLogic Instance()
    {
        if (_instance == null)
        {
            _instance = new JoyStickLogic();
        }
        return _instance;
    }

    private UICamera mUICamera;

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTransform = gameObject.transform;
        _instance = this;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif


        if (null == mUICamera)
        {
            if (null != gameObject.transform.root)
            {
                mUICamera = gameObject.transform.root.GetComponentInChildren<UICamera>();
            }
        }
        if (null != mUICamera)
        {
            mUICamera.allowMultiTouch = true;
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        mJoyStickOrigin.x = mTransform.localPosition.x;
        mJoyStickOrigin.y = mTransform.localPosition.y;
        JoyStickSprite.alpha = 0.5f;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
    if (Input.touchCount > 0)
    {
        UpdateWithTouch();
    }
    else
    {
        // 无触摸点 fingerID又不是-1 应对真机上使用摇杆同时按HOME键退出再进入的情况
        if(mFingerId != -1)
        {
            //LogModule.DebugLog("iPhone HOME come back!");
            mFingerId = -1;
            JoyStickEndMove();
        }
    }
#endif
        if (JoyStickSprite.alpha > 0.8f)
        {
            if (null != OnMoveDirection)
            {
                OnMoveDirection(mTransform.localPosition);
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

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        _instance = null;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnDrag()
    {
#if UNITY_EDITOR
        mTouchPos.x = Input.mousePosition.x;
        mTouchPos.y = Input.mousePosition.y;
        // 两次之间的差值
        var nDeltaX = mTouchPos.x - mMouseBuffer.x;
        var nDeltaY = mTouchPos.y - mMouseBuffer.y;
        var vecDeltaPos = new Vector3(0, 0, 0);
        vecDeltaPos.x = nDeltaX;
        vecDeltaPos.y = nDeltaY;
        JoyStickInMoving(vecDeltaPos);
        // 更新鼠标位置
        mMouseBuffer = Input.mousePosition;
#endif
    }

    private void OnPress(bool bPressed)
    {
#if UNITY_EDITOR
        if (bPressed)
        {
            // 记录鼠标位置
            mMouseBuffer = Input.mousePosition;
            JoyStickStartMove();
        }
        else
        {
            JoyStickEndMove();
        }
#endif

        GameControl.Instance.JoyStickPressed = bPressed;

        if (bPressed)
        {
            EventDispatcher.Instance.DispatchEvent(new MyPlayerMoveBegin());
        }
    }

    private float GetDistance(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));
    }

    private void UpdateWithTouch()
    {
        if (mFingerId == -1)
        {
            var InputtouchCount0 = Input.touchCount;
            for (var i = 0; i < InputtouchCount0; i++)
            {
                var touch = Input.GetTouch(i);
                Vector3 vecTouchPos = touch.position;
                var rayTouch = UICamera.mainCamera.ScreenPointToRay(vecTouchPos);
                RaycastHit hitTouch;
                if (Physics.Raycast(rayTouch, out hitTouch, LayerMask.GetMask("UI")))
                {
                    // 点中摇杆
                    if (hitTouch.collider.gameObject.transform.parent == mTransform.parent)
                    {
                        if (touch.phase == TouchPhase.Began)
                        {
                            // 记录fingerID
                            mFingerId = touch.fingerId;
                            //ProcessInput.Instance().SceneTouchFingerId = -1;
                            InputManager.Instance.SceneTouchFingerId = -1;
                            //m_JoyStickCamera.GetComponent<UICamera>().JoyStickFingerID = m_FingerID;
                            // 摇杆开始移动
                            JoyStickStartMove();
#if (UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
                            _differenceX = touch.position.x;
                            _differenceY = touch.position.y;
#endif
                        }
                    }
                }
            }
        }
        else
        {
            var InputtouchCount1 = Input.touchCount;
            for (var i = 0; i < InputtouchCount1; i++)
            {
                var touch = Input.GetTouch(i);
                if (touch.fingerId == mFingerId && touch.phase == TouchPhase.Moved)
                {
#if (UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
                    JoyStickInMoving(touch.position);
#else
                    JoyStickInMoving(touch.deltaPosition);
#endif
                }
                else if (touch.fingerId == mFingerId && touch.phase == TouchPhase.Ended)
                {
                    mFingerId = -1;
                    JoyStickEndMove();
                }
            }
        }
    }

    private void JoyStickStartMove()
    {
        // 拖动时重设精灵透明度
        JoyStickSprite.alpha = 1.0f;
        // 重置Tween动画
        var nTween = gameObject.GetComponent<TweenPosition>();
        if (null != nTween)
        {
            nTween.from = mTransform.localPosition;
            nTween.ResetToBeginning();
        }
    }

    private void JoyStickInMoving(Vector3 vecDeltaPos)
    {
        // 两次之间的差值
        var nDeltaX = vecDeltaPos.x;
        var nDeltaY = vecDeltaPos.y;

#if (UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR
        nDeltaX -= _differenceX;
        nDeltaY -= _differenceY;

        nDeltaX *= _difTag;
        nDeltaY *= _difTag;

        _differenceX = vecDeltaPos.x;
        _differenceY = vecDeltaPos.y;
#endif
        // 新的XY 先不设置
        var nNewX = mTransform.localPosition.x + nDeltaX;
        var nNewY = mTransform.localPosition.y + nDeltaY;

        // 计算距离
        var nDistance = GetDistance(nNewX, nNewY, mJoyStickOrigin.x, mJoyStickOrigin.y);

        if (nDistance <= MaxRadius)
        {
            // 若拖动位置在最大半径以内 则直接设置newXY
            mTransform.localPosition = new Vector3(nNewX, nNewY, 100);
        }
        else
        {
            // 若拖动位置超出最大半径 则设置连线和圆的交点位置
            var nResult = new Vector3(0, 0, 0);
            // 计算鼠标和摇杆连线的直线方程 与摇杆移动范围为半径的圆的交点坐标
            if (nNewX == mJoyStickOrigin.x)
            {
                // 若直线和x轴垂直
                nResult.x = mJoyStickOrigin.x;
                if (nNewY > mJoyStickOrigin.y)
                {
                    nResult.y = mJoyStickOrigin.y + MaxRadius;
                }
                else
                {
                    nResult.y = mJoyStickOrigin.y - MaxRadius;
                }
            }
            else
            {
                // 直线斜率
                var k = (nNewY - mJoyStickOrigin.y)/(nNewX - mJoyStickOrigin.x);
                // 圆的方程(x-m)^2+(y-n)^2=R^2 此处m为m_nJoyStickOrigin.x, n为m_nJoyStickOrigin.y
                // 联立配成一元二次方程标准型 ax^2+bx+c=0 求出a b c
                var a = Mathf.Pow(k, 2) + 1;
                var b = 2*k*(nNewY - k*nNewX - mJoyStickOrigin.y) - 2*mJoyStickOrigin.x;
                var c = Mathf.Pow(mJoyStickOrigin.x, 2) + Mathf.Pow(nNewY - k*nNewX - mJoyStickOrigin.y, 2) -
                        Mathf.Pow(MaxRadius, 2);
                // 根据求根公式算出两个解
                var x1 = (-b + Mathf.Sqrt(Mathf.Pow(b, 2) - 4*a*c))/(2*a);
                var x2 = (-b - Mathf.Sqrt(Mathf.Pow(b, 2) - 4*a*c))/(2*a);
                // 鼠标在摇杆左右来确定x
                if (nNewX > mJoyStickOrigin.x)
                {
                    nResult.x = x1;
                }
                else
                {
                    nResult.x = x2;
                }
                // 代入直线方程求y
                nResult.y = nNewY - k*nNewX + k*nResult.x;
            }

            mTransform.localPosition = new Vector3(nResult.x, nResult.y, 100);
        }
    }

    private void JoyStickEndMove()
    {
        if (GameControl.Instance == null)
        {
            Logger.Error("JoyStickEndMove  GameControl.Instance == null");
            return;
        }
        GameControl.Instance.JoyStickPressed = false;
        JoyStickSprite.alpha = 0.5f;
        var nTween = gameObject.GetComponent<TweenPosition>();
        if (null != nTween)
        {
            nTween.from = mTransform.localPosition;
            nTween.PlayForward();
        }

        if (OnMoveDirection != null)
        {
            OnMoveDirection(Vector2.zero);
        }
    }
}