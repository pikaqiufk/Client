#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class JJCResultFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public TimerLogic RefrehTimer;
	
	    public string TimeFormat(DateTime time)
	    {
	        var ret = "";
	        if (time < Game.Instance.ServerTime)
	        {
	            RefrehTimer.gameObject.SetActive(false);
	        }
	        else
	        {
	            var dif = time - Game.Instance.ServerTime;
	            ret = string.Format("{0}:{1}", dif.Minutes, dif.Seconds);
	        }
	        return ret;
	    }
	
	    public void OnClickExit()
	    {
	        var e = new AreanResultExitEvent();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        RefrehTimer.OnFormatTime -= TimeFormat;
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.AreanaResult);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
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
	
	        RefrehTimer.OnFormatTime += TimeFormat;
	
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