#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class StoreController : IControllerBase
{
    private static readonly BagItemDataModel emptyBagItemData = new BagItemDataModel();

    public StoreController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(StoreCellClick.EVENT_TYPE, OnStoreCellClick);
        EventDispatcher.Instance.AddEventListener(StoreOperaEvent.EVENT_TYPE, OnStoreOpera);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnRefrehEquipBagItemStatus);
        EventDispatcher.Instance.AddEventListener(StoreCacheTriggerEvent.EVENT_TYPE, StoreCacheTrigger);
        EventDispatcher.Instance.AddEventListener(UpdateFuBenStoreStore_Event.EVENT_TYPE, UpdateFuBenStoreLimitItems);
        EventDispatcher.Instance.AddEventListener(LoadSceneOverEvent.EVENT_TYPE, ClearFuBenItems);
    }

    public IControllerBase BackPack;
    public StoreDataModel DataModel;
    public Coroutine mPressTriger;

    public Dictionary<int, List<StoreCellData>> mStoreCache = new Dictionary<int, List<StoreCellData>>();
        // <server表param[0] ,数据> 

    public object mStoreCacheObject;
    private readonly Dictionary<int, int> mVipModifyCache = new Dictionary<int, int>();
    private int mServiceType = -1;

    public IEnumerator ApplyStoresCoroutine(int type, int serviceType = -1)
    {
        using (new BlockingLayerHelper(0))
        {
            if (mStoreCache.ContainsKey(type))
            {
                SetsStoreCellDatas(type);
                yield break;
            }
            var msg = NetManager.Instance.ApplyStores(type, serviceType); //106
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var itemList = new List<StoreCellData>();
                    {
                        var __list1 = msg.Response.items;
                        var __listCount1 = __list1.Count;
                        for (var __i1 = 0; __i1 < __listCount1; ++__i1)
                        {
                            var item = __list1[__i1].itemid;
                            {
                                var cell = new StoreCellData
                                {
                                    StoreIndex = item
                                };
                                var tbStore = Table.GetStore(cell.StoreIndex);
                                var limit = tbStore.DayCount;
                                if (limit == -1)
                                {
                                    limit = tbStore.WeekCount;
                                }
                                if (limit == -1)
                                {
                                    limit = tbStore.MonthCount;
                                }
                                cell.ExData = limit;
                                if (limit == -1)
                                {
                                    cell.Limit = -1;
                                }
                                else
                                {
                                    cell.Limit = PlayerDataManager.Instance.GetExData(limit) +
                                                 VipModifyCount(cell.StoreIndex);
                                }

                                // 只要发限购数量了 就覆盖掉扩展计数的数量  不走扩展计数了
                                if (__list1[__i1].itemcount >= 0)
                                {
                                    cell.Limit = __list1[__i1].itemcount;
                                    cell.ExData = -1;
                                }

                                cell.ItemId = tbStore.ItemId;
                                itemList.Add(cell);
                            }
                        }
                    }
                    mStoreCache.Add(type, itemList);
                    SetsStoreCellDatas(type);
                }
                else
                {
                    Logger.Error("ApplyStores............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ApplyStores............State..." + msg.State);
            }
        }
    }

    public IEnumerator ButtonOnPressCoroutine(bool isAdd)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (CheckPressCount(isAdd) == false)
            {
                NetManager.Instance.StopCoroutine(mPressTriger);
                mPressTriger = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    public void BuyEquipItem()
    {
        NetManager.Instance.StartCoroutine(StoreBuyEquipCoroutine(DataModel.SelectId, DataModel.ReplaceEquip.BagId,
            DataModel.ReplaceEquip.Index));
    }

    private bool CheckPressCount(bool isAdd)
    {
        if (isAdd)
        {
            if (DataModel.SelectCount < DataModel.MaxCount)
            {
                DataModel.SelectCount++;
                return true;
            }
        }
        else
        {
            if (DataModel.SelectCount > 1)
            {
                DataModel.SelectCount--;
                return true;
            }
        }
        return false;
    }

    public StoreCellData GetCellData(int tableIndex)
    {
        {
            // foreach(var cellData in DataModel.ItemList)
            if (!mStoreCache.ContainsKey(DataModel.StoreType))
            {
                return null;
            }
            var item = mStoreCache[DataModel.StoreType];
            var __enumerator2 = (item).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var cellData = __enumerator2.Current;
                {
                    if (cellData.StoreIndex == tableIndex)
                    {
                        return cellData;
                    }
                }
            }
        }
        return null;
    }

    public void IsCanUse(StoreRecord tbStore, StoreCellData item)
    {
        if (item.Limit == 0)
        {
            item.CanUse = false;
            return;
        }
        if (tbStore.ItemId < 0)
        {
            item.CanUse = false;
            return;
        }
        var tbItemBase = Table.GetItemBase(tbStore.ItemId);
        item.CanUse = PlayerDataManager.Instance.ItemOrEquipCanUse(tbItemBase);
    }

    public void OnClickBuyInfoAdd()
    {
        if (DataModel.SelectCount < DataModel.MaxCount)
        {
            DataModel.SelectCount++;
        }
    }

    public void OnClickBuyInfoBuy()
    {
        var index = DataModel.SelectId;
        var count = DataModel.SelectCount;
        var tbStore = Table.GetStore(index);
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
        var cost = tbStore.NeedValue*count;
        if (PlayerDataManager.Instance.GetRes(tbStore.NeedType) < cost)
        {
            var tbItemCost = Table.GetItemBase(tbStore.NeedType);
            //{0}不足！
            var str = GameUtils.GetDictionaryText(701);
            str = string.Format(str, tbItemCost.Name);
            GameUtils.ShowHintTip(str);
            PlayerDataManager.Instance.ShowItemInfoGet(tbStore.NeedType);

            if ((int)eResourcesType.GoldRes == tbStore.NeedType)
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));
            }
            return;
        }
        if (tbStore.NeedItem != -1)
        {
            if (DataModel.ReplaceEquip.ItemId == -1)
            {
                var tbItemCost = Table.GetItemBase(tbStore.NeedItem);
                //{0}不足！
                var str = GameUtils.GetDictionaryText(701);
                str = string.Format(str, tbItemCost.Name);
                GameUtils.ShowHintTip(str);
                return;
            }

            var find = false;
            PlayerDataManager.Instance.ForeachEquip(equip =>
            {
                if (equip.ItemId != tbStore.NeedItem)
                {
                    return;
                }
                if (equip.Index != DataModel.ReplaceEquip.Index)
                {
                    return;
                }
                find = true;
            });
            if (find == false)
            {
                return;
            }
            var equipOld = Table.GetEquipBase(DataModel.ReplaceEquip.ItemId);
            var equipNew = Table.GetEquipBase(tbStore.ItemId);
            var itemNew = Table.GetItemBase(tbStore.ItemId);
            if (equipOld == null || equipNew == null || itemNew == null)
            {
                return;
            }
            if (equipOld.Part != equipNew.Part)
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    210115,
                    "",
                    () => { BuyEquipItem(); });
                return;
            }
            var result = PlayerDataManager.Instance.CheckItemEquip(itemNew, equipNew);
            if (result != eEquipLimit.OK)
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    210116,
                    "",
                    () => { BuyEquipItem(); });
                return;
            }
            BuyEquipItem();
        }
        else
        {
            NetManager.Instance.StartCoroutine(StoreBuyCoroutine(DataModel.SelectId, DataModel.SelectCount));
        }
    }

    public void OnClickBuyInfoClose()
    {
        DataModel.SelectId = -1;
//         DataModel.RoleId = -1;
//         DataModel.MaxCount = -1;
//         DataModel.SelectCount = -1;
    }

    public void OnClickBuyInfoDel()
    {
        if (DataModel.SelectCount > 1)
        {
            DataModel.SelectCount--;
        }
    }

    public void OnClickBuyInfoMax()
    {
        DataModel.SelectCount = DataModel.MaxCount;
    }

    private void OnClickPressCount(bool isAdd, bool isPress)
    {
        if (isPress)
        {
            if (mPressTriger != null)
            {
                NetManager.Instance.StopCoroutine(mPressTriger);
            }
            mPressTriger = NetManager.Instance.StartCoroutine(ButtonOnPressCoroutine(isAdd));
        }
        else
        {
            if (mPressTriger != null)
            {
                NetManager.Instance.StopCoroutine(mPressTriger);
                mPressTriger = null;
            }
        }
    }

    public void OnClickReplace()
    {
        if (DataModel.ReplaceList.Count < 2)
        {
            return;
        }
        DataModel.ReplaceIndex++;
        DataModel.ReplaceIndex = DataModel.ReplaceIndex%DataModel.ReplaceList.Count;
        DataModel.ReplaceEquip = DataModel.ReplaceList[DataModel.ReplaceIndex];

        var bagType = DataModel.ReplaceEquip.BagId;
        var bagIndex = DataModel.ReplaceEquip.Index;
        switch ((eBagType) bagType)
        {
            case eBagType.Equip07:
            {
                if (bagIndex == 0)
                {
                    DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271002); //"(左手)";
                }
                else
                {
                    DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271003); //"(右手)";
                }
            }
                break;
            case eBagType.Equip11:
            {
                DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271004); //"(主手)";
            }
                break;
            case eBagType.Equip12:
            {
                DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271005); //"(副手)";
            }
                break;
        }
    }

    private void OnClickShowSelectIcon()
    {
        var storeId = DataModel.SelectId;
        var tbStore = Table.GetStore(storeId);
        if (tbStore == null)
        {
            return;
        }
        var itemId = tbStore.ItemId;
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null)
        {
            return;
        }
        if (tbStore.NeedItem == -1)
        {
            GameUtils.ShowItemIdTip(itemId);
            return;
        }

        var bagItemData = new BagItemDataModel();
        bagItemData.ItemId = itemId;

        if (DataModel.ReplaceEquip != null)
        {
            if (DataModel.ReplaceEquip.ItemId != -1)
            {
                bagItemData.Exdata.InstallData(DataModel.ReplaceEquip.Exdata);
            }
        }


        if (bagItemData.Exdata.Count > 0)
        {
            GameUtils.ShowItemDataTip(bagItemData);
        }
        else
        {
            GameUtils.ShowItemIdTip(itemId, 1);
        }
    }

    public void OnRefrehEquipBagItemStatus(IEvent ievent)
    {
        var e = ievent as UIEvent_BagChange;
        if (e.HasType(eBagType.Equip))
        {
            if (State == FrameState.Open)
            {
                PlayerDataManager.Instance.RefreshEquipBagStatus();
            }
        }
    }

    public void OnStoreCellClick(IEvent ievent)
    {
        var e = ievent as StoreCellClick;
        var cellData = e.CellData;
        var tbStore = Table.GetStore(cellData.StoreIndex);
        if (tbStore == null)
        {
            return;
        }
        var tbItem = Table.GetItemBase(tbStore.ItemId);
        if (tbItem == null)
        {
            return;
        }


        //钻石商店会卖资源，所以特殊处理
        if (DataModel.StoreType != 15 && DataModel.StoreType != 16)
        {
            // 对于普通商店来说 ExData==-1 那么一定 limit == -1   所以替换掉没影响
            // 对于神秘商店来说一定是ExData==-1  如果limit == -1 就是不限购显示最大叠加数 如果limit ！= -1 超过最大叠加数 显示最大叠加数 不超过显示limit 
            // if (cellData.ExData == -1)
            if (cellData.Limit == -1)
            {
                DataModel.MaxCount = tbItem.MaxCount;
            }
            else
            {
                DataModel.MaxCount = cellData.Limit > tbItem.MaxCount ? tbItem.MaxCount : cellData.Limit;
            }
        }
        else
        {
            if (cellData.ExData == -1)
            {
                DataModel.MaxCount = 99;
            }
            else
            {
                DataModel.MaxCount = cellData.Limit > 99 ? 99 : cellData.Limit;
            }
        }


        if (DataModel.MaxCount == 0)
        {
            //已达到限购数量
            var e1 = new ShowUIHintBoard(270118);
            EventDispatcher.Instance.DispatchEvent(e1);
            return;
        }
        DataModel.SelectId = cellData.StoreIndex;
        DataModel.SelectCount = 1;
        if (DataModel.StoreType >= 100)
        {
//装备兑换
            if (tbStore.NeedItem == -1)
            {
                DataModel.ReplaceList.Clear();
                DataModel.ReplaceEquip = emptyBagItemData;
            }
            else
            {
                var tbEquip = Table.GetEquipBase(tbStore.NeedItem);
                if (tbEquip == null)
                {
                    return;
                }

                var list = new List<BagItemDataModel>();
                PlayerDataManager.Instance.ForeachEquip(equip =>
                {
                    if (equip.ItemId == tbStore.NeedItem)
                    {
                        list.Add(equip);
                    }
                });
                DataModel.ReplaceFalg = "";
                if (list.Count > 0)
                {
                    DataModel.ReplaceList = new ObservableCollection<BagItemDataModel>(list);
                    DataModel.ReplaceEquip = DataModel.ReplaceList[0];
                    if (list.Count == 2)
                    {
                        DataModel.HasReplace = true;
                        var bagType = DataModel.ReplaceEquip.BagId;
                        var bagIndex = DataModel.ReplaceEquip.Index;
                        switch ((eBagType) bagType)
                        {
                            case eBagType.Equip07:
                            {
                                if (bagIndex == 0)
                                {
                                    DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271002); //"(左手)";
                                }
                                else
                                {
                                    DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271003); //"(右手)";  
                                }
                            }
                                break;
                            case eBagType.Equip11:
                            {
                                DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271004); //"(主手)";
                            }
                                break;
                            case eBagType.Equip12:
                            {
                                DataModel.ReplaceFalg = GameUtils.GetDictionaryText(271005); //"(副手)";
                            }
                                break;
                        }
                    }
                    else
                    {
                        DataModel.HasReplace = false;
                    }
                }
                else
                {
                    DataModel.HasReplace = false;
                    DataModel.ReplaceList.Clear();
                    DataModel.ReplaceEquip = emptyBagItemData;
                }
                DataModel.ReplaceIndex = 0;
            }

            if (tbItem.Type == 2100)
            {
                DataModel.RoleId = tbItem.Exdata[1];
            }
            else
            {
                DataModel.RoleId = -1;
            }
        }
        else
        {
//普通商店
            //if (tbItem.MaxCount == 1)
            //{
            //    OnClickBuyInfoBuy();
            //}
            //else
            //{
            if (tbItem.Type == 2100)
            {
                DataModel.RoleId = tbItem.Exdata[1];
            }
            else
            {
                DataModel.RoleId = -1;
            }
            //}
        }
    }

    public void OnStoreOpera(IEvent ievent)
    {
        var e = ievent as StoreOperaEvent;
        switch (e.Type)
        {
            case 9:
            {
                OnClickShowSelectIcon();
            }
                break;
            case 10:
            {
                OnClickReplace();
            }
                break;
            case 11:
            {
                OnClickBuyInfoClose();
            }
                break;
            case 12:
            {
                OnClickBuyInfoBuy();
            }
                break;
            case 13:
            {
                OnClickBuyInfoMax();
            }
                break;
            case 14:
            {
                OnClickBuyInfoAdd();
            }
                break;
            case 15:
            {
                OnClickBuyInfoDel();
            }
                break;
            case 16:
            {
                OnClickPressCount(true, true);
            }
                break;
            case 17:
            {
                OnClickPressCount(false, true);
            }
                break;
            case 18:
            {
                OnClickPressCount(true, false);
            }
                break;
            case 19:
            {
                OnClickPressCount(false, false);
            }
                break;
        }
    }

    public void SetsStoreCellDatas(int type)
    {
        var list = mStoreCache[type];
        var cellList = new List<StoreCellData>();
        foreach (var cellData in list)
        {
            var tbStore = Table.GetStore(cellData.StoreIndex);

            //钻石商店要刷新vip影响的数量
            if (DataModel.StoreType == 15 || DataModel.StoreType == 16)
            {
                var limit = tbStore.DayCount;
                if (limit == -1)
                {
                    limit = tbStore.WeekCount;
                }
                if (limit == -1)
                {
                    limit = tbStore.MonthCount;
                }
                cellData.ExData = limit;
                if (limit == -1)
                {
                    cellData.Limit = -1;
                }
                else
                {
                    cellData.Limit = PlayerDataManager.Instance.GetExData(limit) + VipModifyCount(cellData.StoreIndex);
                }
            }

            if (PlayerDataManager.Instance.CheckCondition(tbStore.DisplayCondition) != 0)
            {
                continue;
            }
            IsCanUse(tbStore, cellData);
            cellList.Add(cellData);
        }

        DataModel.ItemList = new ObservableCollection<StoreCellData>(cellList);
    }

    public IEnumerator StoreBuyCoroutine(int index, int count = 1)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreBuy(index, count, mServiceType);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                OnClickBuyInfoClose();
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var tbStore = Table.GetStore(index);
                    //购买成功
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(431));
                    var cellData = GetCellData(index);
                    if (cellData.ExData != -1)
                    {
                        cellData.Limit -= count;
                        if (cellData.Limit <= 0)
                        {
                            cellData.CanUse = false;
                        }

                        //钻石商城和绑钻商城通用相同扩展数据，同步刷新限购次数。
                        if (DataModel.StoreType == 15 || DataModel.StoreType == 16)
                        {
                            var type = DataModel.StoreType == 15 ? 16 : 15;
                            if (mStoreCache.ContainsKey(type))
                            {
                                foreach (var item in mStoreCache[type])
                                {
                                    if (item.ItemId == tbStore.ItemId)
                                    {
                                        item.Limit = cellData.Limit;
                                        item.CanUse = cellData.CanUse;
                                        break;
                                    }
                                }
                            }
                        }
                    }

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
                            UpdateStoreList();
                        }
                    }

                    if (DataModel.StoreType == 15)
                    {
                        PlatformHelper.UMEvent("DiamondShopExpend", tbStore.Name.ToString(), tbStore.NeedValue * count);
                    }

                    PlatformHelper.UMEvent("BuyItem", tbStore.Name.ToString(), count);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                {
                    var e = new ShowUIHintBoard(430);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("StoreBuy....StoreId= {0}...ErrorCode...{1}", index, msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("StoreBuy............State..." + msg.State);
            }
        }
    }

    public IEnumerator StoreBuyEquipCoroutine(int index, int bagId, int bagIndex)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreBuyEquip(index, bagId, bagIndex, mServiceType);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                OnClickBuyInfoClose();
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //购买成功
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(431));
                    var cellData = GetCellData(index);
                    if (cellData.ExData != -1)
                    {
                        cellData.Limit -= 1;
                        if (cellData.Limit <= 0)
                        {
                            cellData.CanUse = false;
                        }
                    }
                    var tbStore = Table.GetStore(index);
                    if (tbStore == null)
                    {
                        yield break;
                    }
                    if (msg.Response == 0)
                    {
                        DataModel.ReplaceEquip.ItemId = tbStore.ItemId;
                        //检查属性变化
                        PlayerDataManager.Instance.RefreshEquipBagStatus(DataModel.ReplaceEquip);
                    }
                    var flagId = tbStore.BugSign;
                    if (flagId != -1)
                    {
                        var flag = PlayerDataManager.Instance.GetFlag(flagId);
                        if (flag == false)
                        {
                            PlayerDataManager.Instance.SetFlag(flagId, true);
                            UpdateStoreList();
                        }
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                {
                    var e = new ShowUIHintBoard(430);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("StoreBuy............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("StoreBuy............State..." + msg.State);
            }
        }
    }

    //商店trigger
    public void StoreCacheTrigger(IEvent ievent)
    {
        var mTime = Game.Instance.ServerTime.Date.AddDays(1);
        if (mStoreCacheObject != null)
        {
            TimeManager.Instance.DeleteTrigger(mStoreCacheObject);
            mStoreCacheObject = null;
        }
        mStoreCacheObject = TimeManager.Instance.CreateTrigger(mTime, () => { mStoreCache.Clear(); },
            (int) TimeSpan.FromDays(1).TotalMilliseconds);
    }

    private void UpdateStoreList()
    {
        SetsStoreCellDatas(DataModel.StoreType);
    }

    private int VipModifyCount(int StoreId)
    {
        var ret = 0;
        if (DataModel.StoreType != 15 && DataModel.StoreType != 16)
        {
            return ret;
        }

        if (mVipModifyCache.ContainsKey(StoreId))
        {
            ret = mVipModifyCache[StoreId];
        }

        return ret;
    }

    public void CleanUp()
    {
        DataModel = new StoreDataModel();
        BackPack = UIManager.Instance.GetController(UIConfig.BackPackUI);
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
    }

    public void OnShow()
    {
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Character});
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as StoreArguments;
        if (args == null)
        {
            return;
        }
        mServiceType = -1;
        BackPack.RefreshData(null);
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Character});
        DataModel.SelectId = -1;
        DataModel.SelectCount = 1;
        DataModel.MaxCount = -1;
        var type = args.Tab;
        DataModel.StoreType = type;

        var tbStoreType = Table.GetStoreType(type);
        if (tbStoreType != null)
        {
            DataModel.ResType = tbStoreType.ResType;
            DataModel.ResNum = 0;
            if (tbStoreType.ResType != -1)
            {
                if (tbStoreType.ResType < (int)eResourcesType.CountRes && tbStoreType.ResType > (int)eResourcesType.InvalidRes)
                {
                    var resNum = PlayerDataManager.Instance.GetRes(tbStoreType.ResType);
                    if (resNum != -1)
                    {
                        DataModel.ResNum = resNum;
                    }
                } 
            }
        }

        if (DataModel.StoreType > 100)
        {
            DataModel.ReplaceEquip = emptyBagItemData;
        }
        DataModel.ReplaceList.Clear();
        DataModel.ShowType = args.Tab;

        if (type == 15 || type == 16)
        {
            mVipModifyCache.Clear();
            var table = Table.GetVIP(PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel));
            for (var i = 0; i < table.BuyItemId.Length; i++)
            {
                var id = table.BuyItemId[i];
                if (id != -1)
                {
                    mVipModifyCache.Add(id, table.BuyItemCount[i]);
                }
            }
        }
        if (args.Args != null && args.Args.Count > 0)
        {
            mServiceType = args.Args[0];
            NetManager.Instance.StartCoroutine(ApplyStoresCoroutine(type, args.Args[0]));
        }
        else
        {
            NetManager.Instance.StartCoroutine(ApplyStoresCoroutine(type));
        }
// 	    if (2==type)
// 	    {
// 			if (PlayerDataManager.Instance.GetFlag(522))
// 		    {
// 				var list = new Int32Array();
// 				list.Items.Add(523);
// 
// 				var list1 = new Int32Array();
// 				list1.Items.Add(522);
// 				PlayerDataManager.Instance.SetFlagNet(list, list1);
// 		    }
// 			
// 	    }
    }

    void UpdateFuBenStoreLimitItems(IEvent ievent)
    {
        var e = ievent as UpdateFuBenStoreStore_Event;
        if ( e == null ) return;

        var storeType = e.mStoreType;
        if (!mStoreCache.ContainsKey(storeType)) return;

        if (e.Items == null) return;

        foreach (var item in e.Items.items)
        {
            var cellData = GetCellData(item.itemid);
            if (cellData != null)
            {
                cellData.Limit = item.itemcount;
            }
            SetsStoreCellDatas(storeType);
        }
    }

    void ClearFuBenItems(IEvent ievent)
    {
        var e = ievent as LoadSceneOverEvent;
        if (e == null) return;

        if (mServiceType != -1)
        {
            Table.ForeachService(tableService =>
            {
                if (tableService.Type == mServiceType)
                {
                    if (tableService.Param.Count() > 0 && mStoreCache.ContainsKey(tableService.Param[0]))
                    {
                        mStoreCache[tableService.Param[0]].Clear();
                        mStoreCache.Remove(tableService.Param[0]);
                    }
                }
                
                return true;
            });
            mServiceType = -1;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}