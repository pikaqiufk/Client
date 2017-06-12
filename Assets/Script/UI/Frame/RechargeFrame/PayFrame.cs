using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PayFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private bool mHasBindRemoved = true;
	
	    public void BannerBuyButton()
	    {
	        var e = new UIEvent_RechargeFrame_OnClick(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.RechargeFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnEvent_CloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.RechargeFrame)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            mHasBindRemoved = false;
	        }
	        else
	        {
	            if (mHasBindRemoved == false)
	            {
	                RemoveBindEvent();
	            }
	            mHasBindRemoved = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mHasBindRemoved == false)
	        {
	            RemoveBindEvent();
	        }
	        mHasBindRemoved = true;
	
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
	        if (mHasBindRemoved)
	        {
	            RemoveBindEvent();
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
	        if (mHasBindRemoved)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUiBindRemove);
	            var controllerBase = UIManager.Instance.GetController(UIConfig.RechargeFrame);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel("RechargeDataModel"));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void RemoveBindEvent()
	    {
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUiBindRemove);
	        Binding.RemoveBinding();
	    }
	
	    public void VipFunctionDrugStore()
	    {
	        var e = new UIEvent_RechargeFrame_OnClick(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	//         var arg = new StoreArguments { Tab = 0 };
	//         var e = new Show_UI_Event(UIConfig.StoreUI, arg);
	//         EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void VipFunctionQuickRepair()
	    {
	        var e = new UIEvent_RechargeFrame_OnClick(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void VipFunctionStore()
	    {
	        var e = new UIEvent_RechargeFrame_OnClick(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void VipInfoNext()
	    {
	        var e = new UIEvent_RechargeFrame_OnClick(5);
	        e.exData = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void VipInfoPrevious()
	    {
	        var e = new UIEvent_RechargeFrame_OnClick(5);
	        e.exData = -1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}