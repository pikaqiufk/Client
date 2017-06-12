#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TaskButtonShow : MonoBehaviour
	{
	    public TweenPosition Tween;
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, TriggerEvent);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnPlayOver()
	    {
	        gameObject.SetActive(false);
	    }
	
	    private void DisplayButton(Action callback = null)
	    {
	        gameObject.SetActive(true);
	
	        Tween.enabled = true;
	        Tween.onFinished.Clear();
	        if (null != callback)
	        {
	            Tween.SetOnFinished(new EventDelegate(() => { callback(); }));
	        }
	        Tween.PlayForward();
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        gameObject.SetActive(false);
	
	        if (true != GameSetting.Instance.EnableNewFunctionTip)
	        {
	            return;
	        }
	
	        if (null == GameLogic.Instance)
	        {
	            return;
	        }
	
	
	        //任务按钮一开始是需要显示的
	
	        /*
			GuidanceRecord table = null;
			Table.ForeachGuidance((temp) =>
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
	
			if (!GameLogic.Instance.GuideTrigger.CheckCondtion(table))
			{
				gameObject.SetActive(false);
			}
			gameObject.SetActive(false);
			 * */
	
	        EventDispatcher.Instance.AddEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, TriggerEvent);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void TriggerEvent(IEvent ievent)
	    {
	        if (true != GameSetting.Instance.EnableNewFunctionTip)
	        {
	            return;
	        }
	
	        if (null == GameLogic.Instance)
	        {
	            return;
	        }
	
	        var e = ievent as UIEvent_PlayMainUIBtnAnimEvent;
	        if (null == e)
	        {
	            return;
	        }
	
	        if (0 != e.BtnName.CompareTo("BtnMission"))
	        {
	            return;
	        }
	
	        DisplayButton(e.CallBack);
	    }
	}
}