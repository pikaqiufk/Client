#region using

using System;
using DataTable;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;

#endregion

public class DamageBoardLogic : MonoBehaviour
{
    public UISprite BackGround;
    public DamageBoardAction BoardAction = new DamageBoardAction();
    public UILabel Label;
    public Transform LabelTransform;
    private float lastRealTime;
    private bool mFlag;
    public int StayTime;
    private bool useRealTime;
    private bool isDirectivityPos;
    public enum BoardShowType
    {
        Fight = 0,
        UI = 1
    }

    public enum DamageBoardActionStage
    {
        Invaild = -1,
        Stage1 = 0,
        Stage2,
        Stage3
    }

    public CombatTextRecord Record { get; set; }

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif
        useRealTime = false;
        LabelTransform = Label.transform;
        lastRealTime = Time.realtimeSinceStartup;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void CheckStage(float dt)
    {
        if (BoardAction == null)
        {
            return;
        }
        if (Record == null)
        {
            return;
        }
        BoardAction.Duration += dt;
        if (BoardAction.Duration >= BoardAction.MaxTime)
        {
            switch (BoardAction.ActionStage)
            {
                case DamageBoardActionStage.Stage1:
                {
                    BoardAction.ActionStage = DamageBoardActionStage.Stage2;
                    BoardAction.Duration = 0.0f;
                    BoardAction.ScaleChg = 0.0f;
                    BoardAction.AlphaChg = 0.0f;
                    BoardAction.Speed = Vector2.zero;
                    BoardAction.Accelerate = Vector2.zero;
                    BoardAction.MaxTime = StayTime/1000f;
                }
                    break;
                case DamageBoardActionStage.Stage2:
                {
                    BoardAction.ActionStage = DamageBoardActionStage.Stage3;
                    BoardAction.Duration = 0.0f;
                    BoardAction.ScaleChg = Record.EndFontSize2 / 1000.0f;
                    BoardAction.AlphaChg = -255.0f / (Record.MoveTime2 / 1000.0f);
                    BoardAction.Speed = new Vector2(Record.SpeedX2, Record.SpeedY2);
                    BoardAction.Accelerate = new Vector2(Record.AddSpeedX2, Record.SpeedY2);
                    BoardAction.MaxTime = Record.MoveTime2 / 1000.0f;
                }
                    break;
                case DamageBoardActionStage.Stage3:
                {
                    if (BoardAction.ShowType == BoardShowType.Fight)
                    {
                        DamageBoardManager.Instace.ActiveCount--;
                    }
                    ComplexObjectPool.Release(gameObject, false, false);
                    BoardAction.ActionStage = DamageBoardActionStage.Invaild;
                }
                    break;
                case DamageBoardActionStage.Invaild:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void LateUpdate()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (mFlag && BackGround.enabled)
        {
            mFlag = false;
            BackGround.height = Record.FontSize1 + 20;
            BackGround.width = Label.width + 20;
        }

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
        //StartAction(Vector2.zero);
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

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

	public void StartAction(Vector3 postion,
		int tableId,
		string value,
		BoardShowType showType = BoardShowType.Fight,
		bool ignorePostion = false,
		float delay = 0,
        bool directivityPos = false)
	{
		if (delay <= 0)
		{
			Action(postion, tableId, value, showType, ignorePostion,directivityPos);
		}
		else
		{
			StartCoroutine(DelayAction(postion, tableId, value, showType, ignorePostion, delay, directivityPos));
		}
	}

    public IEnumerator DelayAction(Vector3 postion,
                            int tableId,
                            string value,
                            BoardShowType showType = BoardShowType.Fight,
                            bool ignorePostion = false,
                            float delay = 0,
                            bool directivityPos = false)
	{
		Label.text = "";
		yield return new WaitForSeconds(delay);
		Action(postion, tableId, value, showType, ignorePostion, directivityPos);
	}

    public void Action(Vector3 position,
                            int tableId,
                            string value,
                            BoardShowType showType = BoardShowType.Fight,
                            bool ignorePostion = false,
                            bool directivityPos = false)
    {
        mFlag = true;
        Record = Table.GetCombatText(tableId);
        StayTime = Record.StopTime1;

        if (Record.IsShowShade == 1)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (BackGround.isActiveAndEnabled)
                {
                    BackGround.enabled = false;
                }
            }
            else
            {
                if (!BackGround.isActiveAndEnabled)
                {
                    BackGround.transform.gameObject.SetActive(true);
                    BackGround.enabled = true;
                }
            }
        }
        else
        {
            if (BackGround.isActiveAndEnabled)
            {
                BackGround.enabled = false;
            }
        }

        var font = Label.bitmapFont;
        if (!font.ToString().Contains(Record.FontPath))
        {
            var fontRes = ResourceManager.PrepareResourceSync<UIFont>("UI/Font/" + Record.FontPath + ".prefab");
            Label.bitmapFont = fontRes;
        }


        if (Record.FontType == 0)
        {
//Dynamic
            Label.applyGradient = true;
            Label.color = Color.white;
            if (Record.Color1 != -1)
            {
                Label.gradientTop = GameUtils.GetTableColor(Record.Color1);
            }
            else
            {
                Label.gradientTop = Color.white;
            }
            if (Record.Color2 != -1)
            {
                Label.gradientBottom = GameUtils.GetTableColor(Record.Color2);
            }
            else
            {
                Label.gradientBottom = Color.white;
            }
        }
        else
        {
//
            Label.applyGradient = false;
            Label.supportEncoding = true;
            Label.symbolStyle = NGUIText.SymbolStyle.Colored;
            if (Record.Color1 != -1)
            {
                Label.color = GameUtils.GetTableColor(Record.Color1);
            }
            else
            {
                Label.color = Color.white;
            }
        }

        if (Record.Shadow != -1)
        {
            Label.effectStyle = UILabel.Effect.Shadow;
            Label.effectColor = GameUtils.GetTableColor(Record.Shadow);
        }
        else
        {
            Label.effectStyle = UILabel.Effect.None;
        }

        if (!string.IsNullOrEmpty(Record.FontAdd))
        {
            value = Record.FontAdd + value;
        }
        Label.text = value;

        BoardAction.WorldPos = position;
        BoardAction.ShowType = showType;
        var offsetX = Random.Range(Record.StartX, Record.StartY);
        var offsetY = Random.Range(Record.MinY, Record.MaxY);

        if (showType == BoardShowType.Fight)
        {
            if (DamageBoardManager.Instace == null || DamageBoardManager.Instace.UiCamera == null)
            {
                BoardAction.ActionStage = DamageBoardActionStage.Stage3;
                if (DamageBoardManager.Instace == null)
                {
                    Logger.Error("DamageBoardManager.Instace == null");
                }

                if (DamageBoardManager.Instace != null && DamageBoardManager.Instace.UiCamera == null)
                {
                    Logger.Error("DamageBoardManager.Instace.UiCamera == null");
                }
                return;
            }


            if (ignorePostion)
            {
                LabelTransform.localPosition = new Vector3(offsetX, offsetY, 0);
            }
            else
            {
                var pt = GameLogic.Instance.MainCamera.WorldToScreenPoint(BoardAction.WorldPos);
                var ff = DamageBoardManager.Instace.UiCamera.camera.ScreenToWorldPoint(pt);
                ff.z = 0;
                LabelTransform.position = ff;
                BoardAction.ChangePos = new Vector3(offsetX, offsetY, 0);
                var locPos = LabelTransform.localPosition;
                var posX = locPos.x + offsetX;
                var posY = locPos.y + offsetY;
                LabelTransform.localPosition = new Vector3(posX, posY, 0);
            }
        }
        else
        {
            LabelTransform.localPosition = new Vector3(offsetX, offsetY, 0);
        }

        LabelTransform.localScale = Vector3.one;
        Label.gameObject.SetActive(true);
        Label.alpha = 1.0f;
        Label.fontSize = Record.FontSize1;

        BoardAction.ActionStage = DamageBoardActionStage.Stage1;
        BoardAction.Duration = 0.0f;
        BoardAction.ScaleChg = Record.EndFontSize1/1000.0f;
        BoardAction.AlphaChg = 0.0f;

        isDirectivityPos = directivityPos;
        if (isDirectivityPos)
        {
//             var pos = Vector3.zero;
//             var pos2 = Vector3.zero;
//             if (null != ObjManager.Instance.MyPlayer)
//             {
//                 pos = ObjManager.Instance.MyPlayer.Position;
//                 pos2 =ObjManager.Instance.MyPlayer.transform.TransformPoint(
//                         ObjManager.Instance.MyPlayer.GetComponent<CapsuleCollider>().center);
//             }
// 
//             var centerPos = GameLogic.Instance.MainCamera.WorldToScreenPoint(pos).xy();
//            var centerPos2 = GameLogic.Instance.MainCamera.WorldToScreenPoint(pos2).xy();

            var targetPos = GameLogic.Instance.MainCamera.WorldToScreenPoint(position).xy();
            var vec2Normal = (targetPos - DamageBoardManager.MyPlayerCenterPos).normalized;
            BoardAction.Speed = vec2Normal * Record.SpeedX1;
            BoardAction.Accelerate = vec2Normal * Record.AddSpeedX1;
        }
        else
        {
            BoardAction.Speed = new Vector2(Record.SpeedX1, Record.SpeedY1);
            BoardAction.Accelerate = new Vector2(Record.AddSpeedX1, Record.SpeedY1); 
        }

        BoardAction.MaxTime = Record.MoveTime1/1000.0f;
        BoardAction.ignorePostion = ignorePostion;
    }


    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif
        float dt;
        if (useRealTime)
        {
            dt = Time.realtimeSinceStartup - lastRealTime;
            lastRealTime = Time.realtimeSinceStartup;
        }
        else
        {
            dt = Time.deltaTime;
        }


        UpdateStage(dt);
        CheckStage(dt);
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void UpdateStage(float dt)
    {
        BoardAction.Update(dt, ref Label);
    }

    public class DamageBoardAction
    {
        public Vector2 Accelerate { get; set; }
        public DamageBoardActionStage ActionStage { get; set; }
        public float AlphaChg { get; set; }
        public Vector3 ChangePos { get; set; }
        public float Duration { get; set; }
        public bool ignorePostion { get; set; }
        public float MaxTime { get; set; }
        public float ScaleChg { get; set; }
        public BoardShowType ShowType { get; set; }
        public Vector2 Speed { get; set; }
        public Vector3 WorldPos { get; set; }

        public void Update(float dt, ref UILabel lable)
        {
#if !UNITY_EDITOR
try
{
#endif
            var lableTransform = lable.transform;
            if (ShowType == BoardShowType.Fight)
            {
                if (DamageBoardManager.Instace == null || DamageBoardManager.Instace.UiCamera == null)
                {
                    ActionStage = DamageBoardActionStage.Stage3;
                    if (DamageBoardManager.Instace == null)
                    {
                        Logger.Error("DamageBoardManager.Instace == null");
                    }

                    if (DamageBoardManager.Instace != null && DamageBoardManager.Instace.UiCamera == null)
                    {
                        Logger.Error("DamageBoardManager.Instace.UiCamera == null");
                    }
                    return;
                }

                if (ignorePostion)
                {
                    Speed = new Vector2(Speed.x + Accelerate.x*dt, Speed.y + Accelerate.y*dt);
                    var pos = lableTransform.localPosition;
                    pos = new Vector3(pos.x + Speed.x*dt, pos.y + Speed.y*dt, 0);
                    lableTransform.localPosition = pos;
                }
                else
                {
                    var pt = GameLogic.Instance.MainCamera.WorldToScreenPoint(WorldPos);
                    var ff = DamageBoardManager.Instace.UiCamera.camera.ScreenToWorldPoint(pt);
                    ff.z = 0;
                    lableTransform.position = ff;
                    Speed = new Vector2(Speed.x + Accelerate.x*dt, Speed.y + Accelerate.y*dt);
                    ChangePos = new Vector3(ChangePos.x + Speed.x*dt, ChangePos.y + Speed.y*dt);
                    var pos = lableTransform.localPosition;
                    pos = new Vector3(pos.x + ChangePos.x, pos.y + ChangePos.y, 0);
                    lableTransform.localPosition = pos;
                }
            }
            else
            {
                Speed = new Vector2(Speed.x + Accelerate.x*dt, Speed.y + Accelerate.y*dt);
                var pos = lableTransform.localPosition;
                pos = new Vector3(pos.x + Speed.x*dt, pos.y + Speed.y*dt, 0);
                lableTransform.localPosition = pos;
            }
            var scale = lableTransform.localScale;
            lableTransform.localScale = new Vector3(scale.x + ScaleChg*dt, scale.y + ScaleChg*dt, 0);
            lable.alpha += AlphaChg*dt/255.0f;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }
    }

	
}