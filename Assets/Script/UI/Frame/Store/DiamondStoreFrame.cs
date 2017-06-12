using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class DiamondStoreFrame : MonoBehaviour
	{
	    public BindDataRoot BindData;
	    private bool deleteBind = true;
	    public int StoreType = 15;
	
	    public void OnClickBuyInfoAdd()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(14));
	    }
	
	    public void OnClickBuyInfoAddPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(16));
	    }
	
	    public void OnClickBuyInfoAddUnPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(18));
	    }
	
	    public void OnClickBuyInfoBuy()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(12));
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
	
	    public void OnClickBuyInfoDelPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(17));
	    }
	
	    public void OnClickBuyInfoDelUnPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(19));
	    }
	
	    public void OnClickBuyInfoMax()
	    {
	        EventDispatcher.Instance.DispatchEvent(new StoreOperaEvent(13));
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.StoreUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            deleteBind = false;
	        }
	        else
	        {
	            if (deleteBind == false)
	            {
	                ClearListener();
	            }
	            deleteBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (deleteBind == false)
	        {
	            ClearListener();
	        }
	        deleteBind = true;
	
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
	        if (deleteBind)
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
	        if (deleteBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	            var control = UIManager.Instance.GetController(UIConfig.StoreUI);
	            BindData.SetBindDataSource(control.GetDataModel(""));
	        }
	        deleteBind = true;
	
	        //刷新钻石商店数据
	        UIManager.Instance.GetController(UIConfig.StoreUI).RefreshData(new StoreArguments
	        {
	            Tab = StoreType
	        });
	
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
	}
}