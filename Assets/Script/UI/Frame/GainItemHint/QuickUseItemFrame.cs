
#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class QuickUseItemFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private IControllerBase controller;
	
	    public void OnButtonEquip0()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_HintEquipEvent(0));
	    }
	
	    public void OnButtonEquip1()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_HintEquipEvent(1));
	    }
	
	    public void OnButtonUse0()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_HintUseItemEvent(0));
	    }
	
	    public void OnButtonUse1()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_HintUseItemEvent(1));
	    }
	
	    public void OnClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_HintCloseEvent());
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
	        controller = UIManager.Instance.GetController(UIConfig.GainItemHintUI);
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