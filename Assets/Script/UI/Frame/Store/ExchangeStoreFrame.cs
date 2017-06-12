using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ExchangeStoreFrame : MonoBehaviour
	{
	    public BindDataRoot BindData;
	    private bool deleteBinding = true;
	
	    public void OnClickBuyInfoAdd()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(14));
	    }
	
	    public void OnClickBuyInfoBuy()
	    {
	        var e = new StoreOperaEvent(12);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBuyInfoClose()
	    {
	        var e = new StoreOperaEvent(11);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBuyInfoDel()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(15));
	    }
	
	    public void OnClickBuyInfoMax()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(13));
	    }
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.CustomShopFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.CustomShopFrame)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            deleteBinding = false;
	        }
	        else
	        {
	            if (deleteBinding == false)
	            {
	                ClearListener();
	            }
	            deleteBinding = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (deleteBinding == false)
	        {
	            ClearListener();
	        }
	        deleteBinding = true;
	
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
	        if (deleteBinding)
	        {
	            ClearListener();
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
	        if (deleteBinding)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	            var control = UIManager.Instance.GetController(UIConfig.CustomShopFrame);
	            BindData.SetBindDataSource(control.GetDataModel(""));
	
	            var playerData = PlayerDataManager.Instance.PlayerDataModel;
	            BindData.SetBindDataSource(playerData);
	        }
	        deleteBinding = true;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void ClearListener()
	    {
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	        BindData.RemoveBinding();
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