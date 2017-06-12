using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class ActivityAwardFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private IControllerBase controller;
	
	    public void OnBtnConfirm()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked_1(0));
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        controller = UIManager.Instance.GetController(UIConfig.ActivityRewardFrame);
	        if (controller == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controller.GetDataModel(""));
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnToggleDouble()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked_1(2));
	    }
	
	    public void OnToggleOneTime()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked_1(1));
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