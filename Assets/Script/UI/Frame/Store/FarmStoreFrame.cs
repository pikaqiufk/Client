using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class FarmStoreFrame : MonoBehaviour
	{
	    public BindDataRoot BindData;
	
	    public void OnClickArrange()
	    {
	        var e = new PackArrangeEventUi((int) eBagType.FarmDepot);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
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
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.StoreFarm);
	        EventDispatcher.Instance.DispatchEvent(e);
	        var e1 = new Show_UI_Event(UIConfig.FarmUI);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        BindData.RemoveBinding();
	
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
	
	        var data = PlayerDataManager.Instance.GetBag((int) eBagType.FarmDepot);
	        BindData.SetBindDataSource(data);
	        BindData.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        var controller = UIManager.Instance.GetController(UIConfig.StoreUI);
	        BindData.SetBindDataSource(controller.GetDataModel(""));
	
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
	
	    private void Update()
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