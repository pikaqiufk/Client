#region using

using System;
using DataTable;
using EventSystem;
using Shared;
using UnityEngine;


#endregion

namespace GameUI
{
	public class PayGuide : MonoBehaviour
	{
	    public UILabel LabelDown;
	    private object LabelEnableTrigger;
	    public UILabel LabelUp;
	    public UILabel Name;
	    public Transform ParentFrame;
	    public Transform RootFrame;
	    public GameObject TipObj;
	    public TweenPosition Tween;
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, TriggerEvent);
	        EventDispatcher.Instance.RemoveEventListener(FirstRechargeTextSet_Event.EVENT_TYPE, SetNameText);
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void SetNameText(IEvent ievent)
	    {
	        var e = ievent as FirstRechargeTextSet_Event;
	
	        if (e != null && Name != null)
	        {
	            Name.text = e.Str;
	        }
	    }
	
	    private void SetText()
	    {
	        var payCountTotal = PlayerDataManager.Instance.GetExData(eExdataDefine.e69);
	        if (payCountTotal < 1)
	        {
	            Name.text = GameUtils.GetDictionaryText(100000587);
	            LabelUp.text = GameUtils.GetDictionaryText(300952);
	            LabelDown.text = GameUtils.GetDictionaryText(300953);
	        }
	        else
	        {
	            Name.text = GameUtils.GetDictionaryText(100001000);
	            LabelUp.text = GameUtils.GetDictionaryText(300950);
	            LabelDown.text = GameUtils.GetDictionaryText(300951);
	        }
	    }
	
	    [ContextMenu("Test")]
	    private void ShowBtn(Action callback = null)
	    {
	        if (PlayerDataManager.Instance.ReviewState == 1)
	        {
	            return;
	        }
	
	        gameObject.SetActive(true);
	        //var wor ParentFrame.TransformPoint(RootFrame.localPosition)
	        var locPos = ParentFrame.localPosition;
	        Tween.from = new Vector3(-locPos.x, -locPos.y, 0);
	        Tween.enabled = true;
	        Tween.onFinished.Clear();
	        TipObj.SetActive(true);
	        SetText();
	        if (LabelEnableTrigger != null)
	        {
	            TimeManager.Instance.DeleteTrigger(LabelEnableTrigger);
	            LabelEnableTrigger = null;
	        }
	        LabelEnableTrigger = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddSeconds(4.5f), () =>
	        {
	            TimeManager.Instance.DeleteTrigger(LabelEnableTrigger);
	            LabelEnableTrigger = null;
	            TipObj.SetActive(false);
	        });
	
	        if (null != callback)
	        {
	            Tween.SetOnFinished(new EventDelegate(() => { callback(); }));
	        }
	        Tween.to = Vector3.zero;
	        Tween.ResetToBeginning();
	        Tween.PlayForward();
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        //是否勾选EnableNewFunctionTip都要监听。方便调试。
	        EventDispatcher.Instance.AddEventListener(FirstRechargeTextSet_Event.EVENT_TYPE, SetNameText);
	        SetText();
	        if (true != GameSetting.Instance.EnableNewFunctionTip)
	        {
	            return;
	        }
	
	        if (null == GameLogic.Instance)
	        {
	            return;
	        }
	
	        GuidanceRecord table = null;
	        Table.ForeachGuidance(temp =>
	        {
	            if (0 == temp.Name.CompareTo(gameObject.name))
	            {
	                table = temp;
	                return false;
	            }
	            return true;
	        });
	
	        if (null == table)
	        {
	            return;
	        }
	        var instance = PlayerDataManager.Instance;
			var level = instance.GetRes((int)eResourcesType.LevelRes);
	        if (-1 == table.FlagPrepose || instance.GetFlag(table.FlagPrepose) && 
				instance.ReviewState != 1 &&
				level>=table.Level)
	        {
	//判断标记位
	            gameObject.SetActive(true);
	            TipObj.SetActive(false);
	            return;
	        }
	
	        var next = gameObject.GetComponent<OnClickNextGuide>();
	        if (null == next)
	        {
	            next = gameObject.AddComponent<OnClickNextGuide>();
	        }
	        next.GuideStepList.Clear();
	        next.GuideStepList.Add(table.Flag);
	
	        gameObject.SetActive(false);
	        EventDispatcher.Instance.AddEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, TriggerEvent);
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void TriggerEvent(IEvent ievent)
	    {
	        var e = ievent as UIEvent_PlayMainUIBtnAnimEvent;
	        if (null == e)
	        {
	            return;
	        }
	
	        if (0 != e.BtnName.CompareTo(gameObject.name))
	        {
	            return;
	        }
	
	        ShowBtn(e.CallBack);
	    }
	}
}