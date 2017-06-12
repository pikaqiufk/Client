#region using

using System;
using System.Collections;
using ClientDataModel;
using DataTable;
using EventSystem;
using Shared;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BagFrame : MonoBehaviour
	{
	    public static int[] Guide201 = {20092, 20191, 20290};
	    public static int[] Guide71 = {20091, 20191, 20291};
	    public BindDataRoot BagInfoBind;
	    public bool IsTradingBackPack = false;
	    public bool mIsBinding;
	    private BAG_TAB_PAGE bagPage = BAG_TAB_PAGE.PageBase;
	    //private int mUnlockTimer;
	    private DateTime unlockTimer;
	    private Coroutine unlockTimerCoroutine;
	    public UIScrollViewSimple PackScrollView;
	    public UIButton RecycleBtn;
	    public SelectToggleControl SelectLogic;
	    public UILabel UnLockCost;
	    public UILabel UnLockCount;
	    public GameObject UnlockFrame;
	    public UILabel UnLockOkLabel;
	    public UILabel UnLockTime;
	
	    public enum BAG_TAB_PAGE
	    {
	        PageEquip = 0,
	        PageBase = 1,
	        PageBook = 2,
	        PageFarm = 21
	    }
	
	    public void AddBindEvent()
	    {
	        ResetRecuperationAnimation();
	        BagInfoBind.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        if (null != UnlockFrame)
	        {
	            UnlockFrame.SetActive(false);
	        }
	        EventDispatcher.Instance.AddEventListener(PackUnlockUIEvent.EVENT_TYPE, OnBagUnlock);
	        EventDispatcher.Instance.AddEventListener(PackTradingSellPage.EVENT_TYPE, SetSellTradingPage);
	
	
	        var con = UIManager.Instance.GetController(UIConfig.BackPackUI);
	        BagInfoBind.SetBindDataSource(con.GetDataModel(""));
	        if (SelectLogic != null)
	        {
	            bagPage = (BAG_TAB_PAGE) SelectLogic.Select;
	        }
	        if (IsTradingBackPack)
	        {
	            ShowTradingPage(BAG_TAB_PAGE.PageBase);
	        }
	        else
	        {
	            ShowPage(bagPage);
	        }
	        mIsBinding = true;
	    }
	
	    public int GetPackTabPage()
	    {
	        return (int) bagPage;
	    }
	
	    public void ItemClick(int bagType, int id)
	    {
	        if (bagType == (int) BAG_TAB_PAGE.PageEquip)
	        {
	        }
	    }
	
	    public void OnClickAddCapacity()
	    {
	        var e = new PackCapacityEventUi((int) bagPage);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickArrange()
	    {
	        var e = new PackArrangeEventUi((int) bagPage);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClose()
	    {
	        var e1 = new Close_UI_Event(UIConfig.CharacterUI);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    public void OnClickCloseUnlock()
	    {
	        if (UnlockFrame != null)
	        {
	            UnlockFrame.SetActive(false);
	        }
	        if (unlockTimerCoroutine != null)
	        {
	            NetManager.Instance.StopCoroutine(unlockTimerCoroutine);
	            unlockTimerCoroutine = null;
	        }
	        var e = new PackUnlockOperate(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycle()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RecycleUI));
	    }
	
	    public void OnCLickUnlockConfirm()
	    {
	        if (UnlockFrame != null)
	        {
	            UnlockFrame.SetActive(false);
	        }
	        if (unlockTimerCoroutine != null)
	        {
	            NetManager.Instance.StopCoroutine(unlockTimerCoroutine);
	            unlockTimerCoroutine = null;
	        }
	        var e = new PackUnlockOperate(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        if (mIsBinding)
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
	
	    private void OnDisable()
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
	
	    private void OnEnable()
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
	
	    private IEnumerator OnBagOpen()
	    {
	        yield return new WaitForSeconds(0.17f);
	
	        if (!GuideManager.Instance.IsGuiding())
	        {
	            yield break;
	        }
	
	        var itemId = -1;
	
	        var type = PlayerDataManager.Instance.GetRoleId();
	        if (type < 0 || type >= Guide71.Length)
	        {
	            Logger.Debug("OnOpenBag  type<0 || type>=Guide71.Length");
	            yield break;
	        }
	
	        var CurrentGuidingStepId = GuideManager.Instance.GetCurrentGuidingStepId();
	        if (71 == CurrentGuidingStepId)
	        {
	            itemId = Guide71[type];
	        }
	        else if (CurrentGuidingStepId == 201)
	        {
	            itemId = Guide201[type];
	        }
	
	        if (-1 == itemId)
	        {
	            yield break;
	        }
	
	
	        ScrollCellToTop(itemId);
	    }
	
	    private IEnumerator OnEquipOpen()
	    {
	        yield return new WaitForSeconds(0.1f);
	        if (11 == GuideManager.Instance.GetCurrentGuidingStepId())
	        {
	            var equipId = -1;
	            var type = PlayerDataManager.Instance.GetRoleId();
	            if (0 == type)
	            {
	                equipId = 213101;
	            }
	            else if (1 == type)
	            {
	                equipId = 313101;
	            }
	            else if (2 == type)
	            {
	                equipId = 413101;
	            }
	            if (-1 == equipId)
	            {
	                yield break;
	            }
	
	            var idx = -1;
	            var equipBag = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);
	            if (null != equipBag)
	            {
	                for (var i = 0; i < equipBag.Items.Count; i++)
	                {
	                    var equip = equipBag.Items[i];
	                    if (null == equip || equipId != equip.ItemId)
	                    {
	                        continue;
	                    }
	
	                    idx = i;
	                    break;
	                }
	            }
	
	            if (-1 == idx)
	            {
	                yield break;
	            }
	            PackScrollView.SetLookIndex(idx/5, true);
	        }
	    }
	
	    private void OnBagUnlock(IEvent ievent)
	    {
	        var e = ievent as PackUnlockUIEvent;
	        if (e == null)
	        {
	            Logger.Error("PackUnlockUIEvent == null ");
	            return;
	        }
	        if (UnLockCount == null)
	        {
	            Logger.Error("UnLockCount Obj == null ");
	            return;
	        }
	        UnLockCount.text = e.Count.ToString();
	        UnLockCost.text = e.Cost.ToString();
	        unlockTimer = e.Time;
	        if (unlockTimerCoroutine != null)
	        {
	            NetManager.Instance.StopCoroutine(unlockTimerCoroutine);
	        }
	        if (e.IsRefresh)
	        {
	            unlockTimerCoroutine = NetManager.Instance.StartCoroutine(UnlockTimer());
	        }
	        else
	        {
	            UnLockTime.text = GameUtils.GetTimeDiffString(unlockTimer);
	        }
	        if (e.Cost > 0)
	        {
	            UnLockOkLabel.text = GameUtils.GetDictionaryText(270261);
	        }
	        else
	        {
	            UnLockOkLabel.text = GameUtils.GetDictionaryText(270260);
	        }
	        UnlockFrame.SetActive(true);
	    }
	
	    public void RemoveBindEvent()
	    {
	        BagInfoBind.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(PackUnlockUIEvent.EVENT_TYPE, OnBagUnlock);
	        EventDispatcher.Instance.RemoveEventListener(PackTradingSellPage.EVENT_TYPE, SetSellTradingPage);
	
	        if (unlockTimerCoroutine != null)
	        {
	            NetManager.Instance.StopCoroutine(unlockTimerCoroutine);
	            unlockTimerCoroutine = null;
	        }
	        mIsBinding = false;
	    }
	
	    private void ResetRecuperationAnimation()
	    {
	        if (RecycleBtn)
	        {
	            var sprite = RecycleBtn.GetComponent<UISprite>();
	            if (sprite)
	            {
	                sprite.color = Color.white;
	            }
	            var tweens = GetComponents<TweenColor>();
	            {
	                var __array1 = tweens;
	                var __arrayLength1 = __array1.Length;
	                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
	                {
	                    var tween = __array1[__i1];
	                    {
	                        tween.ResetToBeginning();
	                    }
	                }
	            }
	        }
	    }
	
	    private void ScrollCellToTop(int itemId)
	    {
	        var idx = -1;
	        var bag = PlayerDataManager.Instance.GetBag((int) eBagType.BaseItem);
	        if (null != bag)
	        {
	            for (var i = 0; i < bag.Items.Count; i++)
	            {
	                var item = bag.Items[i];
	                if (null == item || itemId != item.ItemId)
	                {
	                    continue;
	                }
	
	                idx = i;
	                break;
	            }
	        }
	
	        if (-1 == idx)
	        {
	            return;
	        }
	        PackScrollView.SetLookIndex(idx/5, true);
	    }
	
	    public void ShowPage(BAG_TAB_PAGE page = BAG_TAB_PAGE.PageBase)
	    {
	        var nPage = (int) page;
	        bagPage = page;
	        var e = new ShowPackPageEvent {PackPage = nPage};
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        var data = PlayerDataManager.Instance.PlayerDataModel.Bags.Bags[nPage];
	        BagInfoBind.SetBindDataSource(data);
	    }
	
	    public void ShowPageBase()
	    {
	        ShowPage(BAG_TAB_PAGE.PageBase);
	
	        StartCoroutine(OnBagOpen());
	    }
	
	    public void ShowPageEquip()
	    {
	        ShowPage(BAG_TAB_PAGE.PageEquip);
	        StartCoroutine(OnEquipOpen());
	    }
	
	    public void ShowPageOther()
	    {
	        ShowPage(BAG_TAB_PAGE.PageBook);
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
	
	    private IEnumerator UnlockTimer()
	    {
	        while (unlockTimer >= Game.Instance.ServerTime)
	        {
	            UnLockTime.text = GameUtils.GetTimeDiffString(unlockTimer);
	            yield return new WaitForSeconds(0.3f);
	        }
	        UnLockOkLabel.text = GameUtils.GetDictionaryText(270260);
	    }
	
	    #region 交易行
	
	    public void ShowPageBaseForTrading()
	    {
	        ShowTradingPage(BAG_TAB_PAGE.PageBase);
	    }
	
	    public void ShowPageEquipForTrading()
	    {
	        ShowTradingPage(BAG_TAB_PAGE.PageEquip);
	    }
	
	    public void ShowPageOtherForTrading()
	    {
	        ShowTradingPage(BAG_TAB_PAGE.PageBook);
	    }
	
	    public void ShowPageOtherForFarm()
	    {
	        ShowTradingPage(BAG_TAB_PAGE.PageFarm);
	    }
	
	    private int TradingSellPage;
	
	    private void SetSellTradingPage(IEvent iEvent)
	    {
	        var e = iEvent as PackTradingSellPage;
	        TradingSellPage = e.Index;
	        ShowTradingPage((BAG_TAB_PAGE) e.BagPage, false);
	    }
	
	    private void ShowTradingPage(BAG_TAB_PAGE page, bool repeat = true)
	    {
	        var nPage = (int) page;
	        bagPage = page;
	        var e = new ShowPackPageEvent {PackPage = nPage};
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        var data = PlayerDataManager.Instance.PlayerDataModel.Bags.Bags[nPage];
	        var bag = new BagBaseDataModel();
	        bag.BagId = data.BagId;
	        bag.Capacity = data.Capacity;
	        bag.MaxCapacity = data.MaxCapacity;
	        bag.Size = data.Size;
	        bag.UnlockTime = data.UnlockTime;
	        var count = data.Items.Count;
	
	        BagItemDataModel selectItem = null;
	        var equipPage = -1;
	        for (var i = 0; i < count; i++)
	        {
	            var item = data.Items[i];
	            if (data.Items[i].ItemId != -1)
	            {
	                var tb = Table.GetItemBase(item.ItemId);
	                if (tb == null)
	                {
	                    continue;
	                }
	                //TradingSellPage == 0 && 
	                if (BitFlag.GetLow(tb.CanTrade, 0))
	                {
	                    if (selectItem == null)
	                    {
	                        selectItem = item;
	                        equipPage = 0;
	                    }
	                    bag.Items.Add(item);
	                }
	                //TradingSellPage == 1 &&
	                if (BitFlag.GetLow(tb.CanTrade, 1))
	                {
	                    if (bagPage == BAG_TAB_PAGE.PageEquip && item.Exdata.Binding == 1)
	                    {
	                        continue;
	                    }
	                    if (selectItem == null)
	                    {
	                        selectItem = item;
	                        equipPage = 1;
	                    }
	                    bag.Items.Add(item);
	                }
	            }
	        }
	
	        BagInfoBind.SetBindDataSource(bag);
	
	        if (selectItem == null)
	        {
	            selectItem = new BagItemDataModel();
	        }
	
	        if (repeat && equipPage != -1)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingEquipTabPage(equipPage));
	        }
	
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingBagItemClick(selectItem));
	    }
	
	    #endregion
	}
}