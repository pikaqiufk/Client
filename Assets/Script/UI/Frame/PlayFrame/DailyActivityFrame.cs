using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class DailyActivityFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private bool isDelBind = true;
	
	    public void OnCloseClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.PlayFrame));
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (!isDelBind)
	        {
	            DeleteBindEvent();
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
	
	        if (isDelBind)
	        {
	            DeleteBindEvent();
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
	
	        if (isDelBind)
	        {
	            var controllerBase = UIManager.Instance.GetController(UIConfig.PlayFrame);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            var dataModel = controllerBase.GetDataModel("");
	            Binding.SetBindDataSource(dataModel);
	        }
	        isDelBind = true;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnReward0Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameRewardClick(0));
	    }
	
	    public void OnReward1Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameRewardClick(1));
	    }
	
	    public void OnReward2Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameRewardClick(2));
	    }
	
	    public void OnReward3Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameRewardClick(3));
	    }
	
	    public void OnReward4Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameRewardClick(4));
	    }
	
	    public void OnTab0Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameTabSelectEvent(0));
	    }
	
	    public void OnTab1Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameTabSelectEvent(1));
	    }
	
	    public void OnTab2Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameTabSelectEvent(2));
	    }
	
	    public void OnTab3Click()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_PlayFrameTabSelectEvent(3));
	    }
	
	    private void DeleteBindEvent()
	    {
	        Binding.RemoveBinding();
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
	}
}