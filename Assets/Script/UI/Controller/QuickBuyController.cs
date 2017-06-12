#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class QuickBuyController : IControllerBase
{
    public QuickBuyController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(QuickBuyOperaEvent.EVENT_TYPE, OnQuickBuyOpera);
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
    }

    public QuickBuyDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new QuickBuyDataModel();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
        
    }

    public void OnShow()
    {
    }

    public void RefreshData(UIInitArguments ievent)
    {
        var e = ievent as QuickBuyArguments;
        if (e == null)
        {
            return;
        }

        if (e.Items.Count == 1)
        {
            var enumerator1 = e.Items.GetEnumerator();
            if (enumerator1.MoveNext())
            {
                var itemId = enumerator1.Current.Key;
                var item = Table.GetItemBase(itemId);
                if (item == null)
                {
                    return;
                }

                var tbStore = Table.GetStore(item.StoreID);
                if (tbStore == null)
                {
                    return;
                }

                if (GameUtils.IsQuickBuyGift(tbStore.ItemId))
                { // 礼包
                    RefreshGiftBuy(itemId, enumerator1.Current.Value);
                }
                else
                { // 一个物品
                    RefreshOneBuy(itemId, enumerator1.Current.Value);
                }
            }
        }
        else if (e.Items.Count > 1)
        { // 多个
            RefreshMultyBuy(e.Items);
        }
    }

    private int GetIconByGoodsType(int type)
    {
        switch (type)
        {
            case 0:
                return 60000;
            case 1:
                return 600001;
            case 2:
                return 600003;
            case 3:
                return 600002;
        }
        return -1;
    }

    private void RefreshOneBuy(int itemId, int itemCount)
    {
        DataModel.Type = 0;

        var item = Table.GetItemBase(itemId);
        if (item == null || item.StoreID == -1)
        {
            return;
        }

        var tbStore = Table.GetStore(item.StoreID);
        if (tbStore == null)
        {
            return;
        }

        DataModel.OneBuy.StoreId = item.StoreID;
        var maxCount = PlayerDataManager.Instance.GetMaxBuyCount(itemId);
        if (maxCount == -1)
        {
            maxCount = 99;
        }

        DataModel.OneBuy.MaxBuyCount = maxCount;

        DataModel.OneBuy.Item.ItemId = item.Id;
        DataModel.OneBuy.Item.HaveCount = tbStore.ItemCount;
        DataModel.OneBuy.Item.RtIconId = GetIconByGoodsType(tbStore.GoodsType);           

        DataModel.Currency = tbStore.NeedType;
        OneBuySetCount(itemCount);
    }

    private void RefreshMultyBuy(Dictionary<int, int> items)
    {
        DataModel.Type = 1;

        DataModel.OriginalPrice = 0;
        DataModel.DiscountPrice = 0;
        DataModel.MultyBuy.ItemList.Clear();
        var enumerator = items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var itemData = new ItemBuyDataModel();
            itemData.ItemId = enumerator.Current.Key;
            itemData.Count = enumerator.Current.Value;
            var item = Table.GetItemBase(itemData.ItemId);
            if (item == null || item.StoreID == -1)
            {
                continue;
            }

            var tbStore = Table.GetStore(item.StoreID);
            if (tbStore != null)
            {
                itemData.HaveCount = tbStore.ItemCount;
                itemData.RtIconId = GetIconByGoodsType(tbStore.GoodsType);
                DataModel.OriginalPrice += itemData.Count * tbStore.Old;
                DataModel.DiscountPrice += itemData.Count * tbStore.NeedValue;
                DataModel.Currency = tbStore.NeedType;
            }
            DataModel.MultyBuy.ItemList.Add(itemData);
        }

        UpdateCost();
    }

    private void RefreshGiftBuy(int itemId, int itemCount)
    {
        DataModel.Type = 2;

        var item = Table.GetItemBase(itemId);
        if (item == null || item.StoreID == -1)
        {
            return;
        }

        var tbStore = Table.GetStore(item.StoreID);
        if (tbStore == null)
        {
            return;
        }

        DataModel.GiftBuy.NeedItemId = itemId;

        DataModel.GiftBuy.Item.ItemId = tbStore.ItemId;
        DataModel.GiftBuy.Item.Count = itemCount;
        DataModel.GiftBuy.Item.HaveCount = tbStore.ItemCount;
        DataModel.GiftBuy.Item.RtIconId = GetIconByGoodsType(tbStore.GoodsType);           

        DataModel.OriginalPrice = itemCount * tbStore.Old;
        DataModel.DiscountPrice = itemCount * tbStore.NeedValue;

        DataModel.Currency = tbStore.NeedType;

        UpdateCost();
    }

    private void OneBuySetCount(int count)
    {
        if (count < 1)
            count = 1;

        if (count > DataModel.OneBuy.MaxBuyCount)
            count = DataModel.OneBuy.MaxBuyCount;

        var tbStore = Table.GetStore(DataModel.OneBuy.StoreId);
        if (tbStore != null)
        {
            DataModel.OneBuy.BuyCount = count;
            DataModel.OneBuy.Item.Count = count;
            DataModel.OriginalPrice = count * tbStore.Old;
            DataModel.DiscountPrice = count * tbStore.NeedValue;
        }

        UpdateCost();
    }

    private void UpdateCost()
    {
        var have = PlayerDataManager.Instance.GetRes(DataModel.Currency);
        if (have >= DataModel.OriginalPrice)
        {
            DataModel.OriginalColor = MColor.white;
        }
        else
        {
            DataModel.OriginalColor = MColor.red;
        }

        if (have >= DataModel.DiscountPrice)
        {
            DataModel.DiscountlColor = MColor.white;
        }
        else
        {
            DataModel.DiscountlColor = MColor.red;
        }

        if (DataModel.OriginalPrice != 0)
        {
            var dis = Math.Floor(10 * (double)DataModel.DiscountPrice / DataModel.OriginalPrice);
            DataModel.Discount = string.Format(GameUtils.GetDictionaryText(100001165), (int)dis);
        }
    }

    public void OnQuickBuyOpera(IEvent ievent)
    {
        var e = ievent as QuickBuyOperaEvent;
        if (e == null)
        {
            return;
        }

        switch (e.Type)
        {
            case 0:
            { // 减少数量
                OneBuySetCount(DataModel.OneBuy.BuyCount - 1);
            }
                break;

            case 3:
            {
                OneBuySetCount(DataModel.OneBuy.BuyCount + 1);
            }
                break;

            case 11:
            {
                BuyStoreItem(DataModel.OneBuy.StoreId, DataModel.OneBuy.BuyCount);
            }
                break;
            case 12:
            {
                var have = PlayerDataManager.Instance.GetRes(DataModel.Currency);
                if (have < DataModel.DiscountPrice)
                {
                    var tbItemCost = Table.GetItemBase(DataModel.Currency);
                    var str = GameUtils.GetDictionaryText(701);
                    str = string.Format(str, tbItemCost.Name);
                    GameUtils.ShowHintTip(str);
                    EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame,
                            new RechargeFrameArguments { Tab = 0 }));
                    EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.QuickBuyUi));
                    return;
                }

                var enumerator = DataModel.MultyBuy.ItemList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var buyItem = enumerator.Current;
                    if (buyItem != null)
                    {
                        var tbItem = Table.GetItemBase(buyItem.ItemId);
                        if (tbItem == null)
                        {
                            continue;
                        }
                        BuyStoreItem(tbItem.StoreID, buyItem.Count);
                    }
                }
            }
                break;
            case 13:
                {
                    var item = Table.GetItemBase(DataModel.GiftBuy.NeedItemId);
                    if (item == null || item.StoreID == -1)
                    {
                        return;
                    }

                    BuyStoreItem(item.StoreID, DataModel.GiftBuy.Item.Count);
                }
                break;
            case 21:
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame,
                        new RechargeFrameArguments { Tab = 2 }));
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.QuickBuyUi));
            }
                break;
        }
    }


    public void BuyStoreItem(int storeId, int count)
    {
        var tbStore = Table.GetStore(storeId);
        if (tbStore == null)
        {
            return;
        }
        var roleType = PlayerDataManager.Instance.GetRoleId();
        if (BitFlag.GetLow(tbStore.SeeCharacterID, roleType) == false)
        {
            return;
        }

        if (tbStore.DisplayCondition != -1)
        {
            var retCond = PlayerDataManager.Instance.CheckCondition(tbStore.DisplayCondition);
            if (retCond != 0)
            {
                GameUtils.ShowHintTip(retCond);
                return;
            }
        }
        var cost = tbStore.NeedValue * count;
        if (PlayerDataManager.Instance.GetRes(tbStore.NeedType) < cost)
        {
            var tbItemCost = Table.GetItemBase(tbStore.NeedType);
            //{0}不足！
            var str = GameUtils.GetDictionaryText(701);
            str = string.Format(str, tbItemCost.Name);
            GameUtils.ShowHintTip(str);
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame,
                    new RechargeFrameArguments { Tab = 0 }));
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.QuickBuyUi));
            return;
        }
        if (tbStore.NeedItem == -1)
        {
            NetManager.Instance.StartCoroutine(StoreBuyCoroutine(storeId, count));
        }
    }

    public IEnumerator StoreBuyCoroutine(int index, int count = 1)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreBuy(index, count, (int)NpcService.NsShop);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (DataModel.Type != 2)
                    EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.QuickBuyUi));

                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    var tbStore = Table.GetStore(index);
                    //购买成功
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(431));

                    if (tbStore == null)
                    {
                        yield break;
                    }
                    var flagId = tbStore.BugSign;
                    if (flagId != -1)
                    {
                        var flag = PlayerDataManager.Instance.GetFlag(flagId);
                        if (flag == false)
                        {
                            PlayerDataManager.Instance.SetFlag(flagId, true);
                        }
                    }

                    PlatformHelper.UMEvent("BuyItem", tbStore.Name, count);
                }
                else if (msg.ErrorCode == (int)ErrorCodes.Error_ItemNoInBag_All)
                {
                    var e = new ShowUIHintBoard(430);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("QuickBuy StoreBuy....StoreId= {0}...ErrorCode...{1}", index, msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("QuickBuy StoreBuy............State..." + msg.State);
            }
        }
    }

    private void OnResourceChange(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }

        var e = ievent as Resource_Change_Event;
        if (e != null && (int)e.Type == DataModel.Currency)
        {
            UpdateCost();
        }
    }

    public FrameState State { get; set; }
}
