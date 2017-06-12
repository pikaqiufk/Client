using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ZhanXinFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public List<UIButton> BuyButtons;
	    public List<UIEventTrigger> DiamondButtons;
	    public UIGridSimple MainScroll;
	    private GEM_TAB_PAGE packPage = GEM_TAB_PAGE.PageGemAll;
	    public BindDataRoot PlayerBinding;
	    public SelectToggleControl SelectLogic;
	    public UIGridSimple SimpleScroll;
	
	    public enum GEM_TAB_PAGE
	    {
	        PageGemAll = 0,
	        PageDiamond = 1, //宝石
	        PageCrystal = 2, //水晶
	        PageAgate = 3 //玛瑙
	    }
	
	    public void BtnBack()
	    {
	        var e = new UIEvent_AstrologyBack();
	        e.Index = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BuyListButton(int index)
	    {
	        var e = new UIEvent_AstrologyBtnBuyList();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void DiamondsButton(int index)
	    {
	        var e = new UIEvent_AstrologyBtnDiamonds();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPutOff()
	    {
	        var e = new UIEvent_AstrologyBtnPutOff();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPutOn()
	    {
	        var e = new UIEvent_AstrologyBtnPutOn();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnUpBack()
	    {
	        var e = new UIEvent_AstrologyOperation(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnUpOk()
	    {
	        var e = new UIEvent_AstrologyOperation(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnUpShow()
	    {
	        var e = new UIEvent_AstrologyOperation(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void Close()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.AstrologyUI));
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_AstrologySetGridLookIndex.EVENT_TYPE, SetGridLookIndex);
	
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
	        PlayerBinding.RemoveBinding();
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.AstrologyUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel("MainData"));
	        Binding.SetBindDataSource(controllerBase.GetDataModel("DetailData"));
	        PlayerBinding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        if (SelectLogic != null)
	        {
	            packPage = (GEM_TAB_PAGE) SelectLogic.Select;
	        }
	
	        ShowPage(packPage);
	
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
	
	        for (var i = 0; i < BuyButtons.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { BuyListButton(j); });
	            BuyButtons[i].onClick.Add(deleget);
	        }
	
	        for (var i = 0; i < DiamondButtons.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { DiamondsButton(j); });
	            DiamondButtons[i].onClick.Add(deleget);
	        }
	        EventDispatcher.Instance.AddEventListener(UIEvent_AstrologySetGridLookIndex.EVENT_TYPE, SetGridLookIndex);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    #region  星座背包
	
	    public void ShowGemAll()
	    {
	        ShowPage(GEM_TAB_PAGE.PageGemAll);
	    }
	
	    public void ShowGemDiamond()
	    {
	        ShowPage(GEM_TAB_PAGE.PageDiamond);
	    }
	
	    public void ShowGemCrystal()
	    {
	        ShowPage(GEM_TAB_PAGE.PageCrystal);
	    }
	
	    public void ShowGemAgate()
	    {
	        ShowPage(GEM_TAB_PAGE.PageAgate);
	    }
	
	
	    public void ShowPage(GEM_TAB_PAGE page = GEM_TAB_PAGE.PageGemAll)
	    {
	        var nPage = (int) page;
	        packPage = page;
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_AstrologyBagTabClick(nPage));
	    }
	
	    public void ArrangeClick()
	    {
	        var e = new UIEvent_AstrologyArrangeClick((int) packPage);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void SetGridLookIndex(IEvent ievent)
	    {
	        var e = ievent as UIEvent_AstrologySetGridLookIndex;
	        if (e.Type == 0)
	        {
	            MainScroll.SetLookIndex(e.Index, true);
	        }
	        else if (e.Type == 1)
	        {
	            SimpleScroll.SetLookIndex(e.Index, true);
	        }
	    }
	
	    #endregion
	}
}