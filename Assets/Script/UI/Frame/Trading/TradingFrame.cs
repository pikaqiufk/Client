using GameUI;
using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TradingFrame : MonoBehaviour
	{
	    public BagFrame BackPackForTrading;
	    public BindDataRoot Binding;
	    public UIToggle ExangeToggle;
	    public GameObject FlyObj;
	    public GameObject HomeObj;
	    private GameObject mFlyPrefab;
	    private bool mRemoveBind = true;
	    public UIToggle OtherPageToggle;
	
	    public void BtnBackToOtherPlayerList()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(12));
	    }
	
	    public void BtnPageDown()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPageUp()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BuyItemAuction()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(10);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BuyItemAuctionCancel()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(13);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void EquipTabPage1()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(11);
	        e.Value = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void EquipTabPage2()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(11);
	        e.Value = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnBtnBackToCityFrame()
	    {
	        var e = new Close_UI_Event(UIConfig.TradingUI);
	        EventDispatcher.Instance.DispatchEvent(e);
            //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.CityUI));
	    }
	
	    public void OnBtnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.TradingUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOptionSelectDia()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(8);
	        e.Value = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOptionSelectOther16()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(8);
	        e.Value = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //public void CloseRecord()
	    //{
	    //    UIEvent_OnTradingEquipOperation e = new UIEvent_OnTradingEquipOperation(2);
	    //    e.Value = 0;
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	
	    public void OnClickPriceOpt()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellOptionDia()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(9);
	        e.Value = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellOptionOther16()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(9);
	        e.Value = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTimeOpt()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.TradingUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            mRemoveBind = false;
	        }
	        else
	        {
	            if (mRemoveBind == false)
	            {
	                RemoveBindEvent();
	            }
	            mRemoveBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mRemoveBind == false)
	        {
	            RemoveBindEvent();
	        }
	        mRemoveBind = true;
	
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
	        if (mRemoveBind)
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
	        if (mRemoveBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	            EventDispatcher.Instance.AddEventListener(UIEvent_TradingFlyAnim.EVENT_TYPE, OnTradingFlyAnim);
	
	
	            var controllerBase = UIManager.Instance.GetController(UIConfig.TradingUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel("TradingDataModel"));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	            BackPackForTrading.AddBindEvent();
	        }
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnInputNumber()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(16));
	    }
	
	    //???
	    public void OnItemAuction()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(12);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnToggleExangeClick()
	    {
	        if (null != ExangeToggle)
	        {
	            if (ExangeToggle.value)
	            {
	                EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(15));
	            }
	        }
	    }
	
	    public void OnToggleValueChange()
	    {
	        if (null != OtherPageToggle)
	        {
	            if (OtherPageToggle.value)
	            {
	                EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(15));
	            }
	        }
	    }
	
	    private void OnTradingFlyAnim(IEvent ievent)
	    {
	        var e = ievent as UIEvent_TradingFlyAnim;
	        var obj = Instantiate(mFlyPrefab) as GameObject;
	        PlayerDataManager.Instance.PlayFlyItem(obj, FlyObj.transform, HomeObj.transform, 12, e.Exp);
	    }
	
	    public void refreshOther()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(10));
	    }
	
	    private void RemoveBindEvent()
	    {
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_TradingFlyAnim.EVENT_TYPE, OnTradingFlyAnim);
	
	
	        Binding.RemoveBinding();
	        BackPackForTrading.RemoveBindEvent();
	    }
	
	    public void ShowRecord()
	    {
	        var e = new UIEvent_OnTradingEquipOperation(2);
	        // e.Value = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        mFlyPrefab = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void StoreOperationAdd()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(8));
	    }
	
	    public void TradingInfoAdd()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(2));
	    }
	
	    public void TradingInfoAddPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(4));
	    }
	
	    public void TradingInfoAddRelease()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(5));
	    }
	
	    public void TradingInfoSub()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(3));
	    }
	
	    public void TradingInfoSubPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(6));
	    }
	
	    public void TradingInfoSubRelease()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(7));
	    }
	
	    public void TradingSellInfoClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(1));
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