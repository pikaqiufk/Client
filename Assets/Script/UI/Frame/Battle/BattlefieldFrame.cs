#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class BattlefieldFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public TimerLogic ExpectTimerLogic;
	    public TimerLogic StartTimerLogic;
	
	    public string FormatWaitTime(DateTime time)
	    {
	        var str = "";
	        if (time > Game.Instance.ServerTime)
	        {
	            var dateTime = time - Game.Instance.ServerTime;
	            //str = GameUtils.GetTimeDiffString(dateTime);
	            str = string.Format(GameUtils.GetDictionaryText(270052), dateTime.Minutes, dateTime.Seconds);
	        }
	        else
	        {
	            //无法确定时间
	            str = GameUtils.GetDictionaryText(270053);
	        }
	        return str;
	    }
	
	    public string FormatBeginTime(DateTime time)
	    {
	        var dateTime = Game.Instance.ServerTime - time;
	        //return GameUtils.GetTimeDiffString(dateTime);
	        //{0:D2}分{1:D2}秒
	        var str = string.Format(GameUtils.GetDictionaryText(270052), dateTime.Minutes, dateTime.Seconds);
	        return str;
	    }
	
	    public void OnCkickAward()
	    {
	        var e = new BattleOperateEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBack()
	    {
	        var e = new Close_UI_Event(UIConfig.BattleUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.BattleUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickQueue()
	    {
	        var e = new BattleOperateEvent(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (StartTimerLogic)
	        {
	            StartTimerLogic.OnFormatTime -= FormatBeginTime;
	        }
	        if (ExpectTimerLogic)
	        {
	            ExpectTimerLogic.OnFormatTime -= FormatWaitTime;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        Binding.RemoveBinding();
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.BattleUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	
	
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
	
	        if (StartTimerLogic)
	        {
	            StartTimerLogic.OnFormatTime += FormatBeginTime;
	        }
	        if (ExpectTimerLogic)
	        {
	            ExpectTimerLogic.OnFormatTime += FormatWaitTime;
	        }
	
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