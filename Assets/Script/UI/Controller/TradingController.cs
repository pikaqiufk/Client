#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using GameUI;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class TradingController : IControllerBase
{
    public TradingController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_TradingFrameButton.EVENT_TYPE, OnBtnEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_TradingBagItemClick.EVENT_TYPE, BagItemClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_OnTradingItemSelled.EVENT_TYPE, OnItemSelled);
        EventDispatcher.Instance.AddEventListener(UIEvent_TradingCoolDownChanged.EVENT_TYPE, OnCoolDownChanged);
        EventDispatcher.Instance.AddEventListener(UIEvent_OnTradingEquipOperation.EVENT_TYPE, OnEquipOperation);
        EventDispatcher.Instance.AddEventListener(UIEvent_TradingEquipTabPage.EVENT_TYPE, RefleshTradingEquipTabPage);
    }

    private bool bVisible;
    public BuildingDataModel mBuildingDataModel;
    public TradingDataModel mDataModel;
    public int mExchangeItemIndex;
    public int mMySellIndex;
    public ulong SellerId;
    public Coroutine ButtonPress { get; set; }
    public float SinglePrice { get; set; }
    public FrameState State { get; set; }

    # region base class

    public void CleanUp()
    {
        if (mDataModel != null)
        {
            mDataModel.SellSelectingItem.PropertyChanged -= OnRateChanged;
            mDataModel.SelectedExchangeItem.PropertyChanged -= OnExchangeRateChanged;
            mDataModel.MyTradingItems.PropertyChanged -= MyStackChange;
        }

        mDataModel = new TradingDataModel();
        mDataModel.SellSelectingItem.PropertyChanged += OnRateChanged;
        mDataModel.SelectedExchangeItem.PropertyChanged += OnExchangeRateChanged;

        mDataModel.ExchangeItems.Clear();
        Table.ForeachTrade(table =>
        {
            var exchangeItem = new ExchangeItemDataModel();
            exchangeItem.ExchangeId = table.Id;
            exchangeItem.BagItem.ItemId = table.ItemID;
            exchangeItem.Price = table.Price;
            exchangeItem.SellCount = 1;
            exchangeItem.SellPrice = table.Price;
            exchangeItem.SellGroupRate = table.Count;
            exchangeItem.SellGroupCountMax = 0;
            exchangeItem.SellGroupCount = 0;
            mDataModel.ExchangeItems.Add(exchangeItem);
            return true;
        });

        //装备交易行功能

        InitMenuList();
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as TradingArguments;

        if (args != null && args.Tab != -1)
        {
            PlayerDataManager.Instance.PlayerDataModel.SkillData.TabSelectIndex = args.Tab;
        }
        else
        {
            PlayerDataManager.Instance.PlayerDataModel.SkillData.TabSelectIndex = 0;
        }

        mDataModel.SellInfoShow = false;
        mDataModel.OtherSellInfoShow = false;
        mDataModel.ExchangeSellInfoShow = false;
        mDataModel.SellSelectingItem.Clone(new TradingItemDataModel());
        BuildingData buildData = null;
        if (args != null && args.BuildingData != null)
        {
            buildData = args.BuildingData;
        }
        else
        {
            if (CityManager.Instance == null || CityManager.Instance.BuildingDataList == null)
            {
                return;
            }
            {
                // foreach(var buildingData in CityManager.Instance.BuildingDataList)
                var __enumerator1 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var buildingData = __enumerator1.Current;
                    {
                        var tbBuild = Table.GetBuilding(buildingData.TypeId);
                        if (tbBuild.Type == 9)
                        {
                            buildData = buildingData;
                            break;
                        }
                    }
                }
            }
        }
        if (buildData == null)
        {
            return;
        }
        var tbBuilding = Table.GetBuilding(buildData.TypeId);
        if (null == tbBuilding)
        {
            return;
        }
        var tbBuildingService = Table.GetBuildingService(tbBuilding.ServiceId);
        var count = tbBuildingService.Param[0];
        var itemCount = mDataModel.MyTradingItems.Count;
        for (var i = itemCount; i < count; i++)
        {
            mDataModel.MyTradingItems.Add(CreateItemFromIdOrDataModel(-1));
        }


        RefreshExchangeItems();
        mDataModel.EquipData.ItemTypeOpt[0] = true;
        OnTradingTabClick(0);
        MenuSelected(0);
        SetSellSelectType(0);
        ApplySellHistory();
    }

    public void RefreshExchangeItems()
    {
        var c = mDataModel.ExchangeItems.Count;
        for (var i = 0; i < c; i++)
        {
            var exchangeItem = mDataModel.ExchangeItems[i];
            var count = PlayerDataManager.Instance.GetItemTotalCount(exchangeItem.BagItem.ItemId).Count;
            exchangeItem.BagItem.Count = count;
            exchangeItem.SellGroupCountMax = count/exchangeItem.SellGroupRate;
            exchangeItem.SellGroupCount = count >= exchangeItem.SellGroupRate ? 1 : 0;
            exchangeItem.SellCount = exchangeItem.SellGroupRate;
            exchangeItem.SellPrice = exchangeItem.SellCount*exchangeItem.Price;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "TradingDataModel")
        {
            return mDataModel;
        }
        return null;
    }

    public void Close()
    {
        bVisible = false;
    }

    private float timeinterval;

    public void Tick()
    {
        if (!bVisible)
        {
            return;
        }

        timeinterval += Time.deltaTime;
        if (timeinterval < 1)
        {
            return;
        }
        timeinterval -= 1;


        //每个物品吆喝时长
        var count = mDataModel.MyTradingItems.Count;
        for (var i = 0; i < count; i++)
        {
            var item = mDataModel.MyTradingItems[i];
            if (item.PeddleDateTime > Game.Instance.ServerTime)
            {
                item.PeddleTime = GameUtils.GetTimeDiffString(item.PeddleDateTime);
            }
        }

        //叫卖cd
        mDataModel.PeddlingTime =
            GameUtils.GetTimeDiffString(DateTime.FromBinary(mDataModel.PeddlingCd) - Game.Instance.ServerTime);
        var freeTime = Extension.FromServerBinary(mDataModel.BroadCastNextFreeTime);
        mDataModel.BroadCastTimeString = GameUtils.GetTimeDiffString(freeTime - Game.Instance.ServerTime);

        if (freeTime < Game.Instance.ServerTime && mDataModel.BaseLayerShow)
        {
            RefreshOtherPlayerWithCD();
            mDataModel.BroadCastNextFreeTime = DateTime.MaxValue.ToBinary();
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public void Init(SelfStoreList list)
    {
        mDataModel.MyTradingItems.Clear();
        var c = list.Items.Count;
        for (var i = 0; i < c; i++)
        {
            var item = CreateItemFromNetData(list.Items[i]);

            mDataModel.MyTradingItems.Add(item);
        }
        RefreshMyStackCount();
        mDataModel.PeddlingCd = Extension.FromServerBinary(list.NextFreeTime).ToBinary();
        RefreshNotice();
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name.Equals("Init"))
        {
            CleanUp();
            Init(param[0] as SelfStoreList);
        }
        return null;
    }

    public void OnShow()
    {
        bVisible = true;
    }

    private void SetSellType(TradingItemDataModel data)
    {
        if (data.TradeType == (int) eSellType.DiamondRes)
        {
            data.SellType = (int) eResourcesType.DiamondRes;
        }
        else
        {
            data.SellType = (int) eResourcesType.Other16;
        }
    }

    public TradingItemDataModel CreateItemFromNetData(SelfStoreOne data)
    {
        var itemBase = data.ItemData;
        if (itemBase == null)
        {
            return new TradingItemDataModel();
        }

        var bagItem = new BagItemDataModel();
        bagItem.ItemId = itemBase.ItemId;
        bagItem.Count = itemBase.Count;
        bagItem.Exdata.InstallData(itemBase.Exdata);
        var tradingItem = CreateItemFromIdOrDataModel(bagItem);
        tradingItem.SellCount = data.ItemData.Count;
        tradingItem.SellPrice = data.NeedCount;
        tradingItem.SellType = data.ItemType;
        tradingItem.TradeType = data.ItemType;
        var overTime = Extension.FromServerBinary(data.BroadcastOverTime);
        if (overTime < Game.Instance.ServerTime)
        {
            tradingItem.PeddleTime = string.Empty;
            tradingItem.IsPeddling = false;
            tradingItem.PeddleDateTime = DateTime.MinValue;
        }
        else
        {
            var diffTime = overTime.Subtract(Game.Instance.ServerTime);
            tradingItem.PeddleTime = GameUtils.GetTimeDiffString(diffTime);
            tradingItem.IsPeddling = true;
            tradingItem.PeddleDateTime = overTime;
        }

        tradingItem.TradingItemId = data.Id;
        tradingItem.State = GetItemState(data.State);
        SetSellType(tradingItem);
        return tradingItem;
    }

    public TradingItemDataModel CreateItemFromNetData(OtherStoreOne data)
    {
        var itemBase = data.ItemData;
        if (itemBase == null)
        {
            return new TradingItemDataModel();
        }

        var bagItem = new BagItemDataModel();
        bagItem.ItemId = itemBase.ItemId;
        bagItem.Count = itemBase.Count;
        bagItem.Exdata.InstallData(itemBase.Exdata);
        var tradingItem = CreateItemFromIdOrDataModel(bagItem);
        tradingItem.SellCount = data.ItemData.Count;
        tradingItem.SellPrice = data.NeedCount;
        tradingItem.TradingItemId = data.Id;
        tradingItem.SellType = data.NeedType;
        tradingItem.TradeType = data.NeedType;
        tradingItem.ManagerId = data.ManagerId;
        tradingItem.State = GetItemState(data.State);
        SetSellType(tradingItem);
        return tradingItem;
    }

    public int GetItemState(int type)
    {
        switch (type)
        {
            case (int) StoreItemType.Buyed:
            {
                return 2;
            }
                break;
            case (int) StoreItemType.Normal:
            {
                return 1;
            }
                break;
            case (int) StoreItemType.Free:
            {
                return 0;
            }
                break;
        }
        return 0;
    }

    public TradingItemDataModel CreateItemFromIdOrDataModel(int id)
    {
        var item = new BagItemDataModel();
        item.ItemId = id;
        item.Count = 0;
        return CreateItemFromIdOrDataModel(item);
    }

    public TradingItemDataModel CreateItemFromIdOrDataModel(BagItemDataModel bagDataModel)
    {
        var dataModel = new TradingItemDataModel();

        if (bagDataModel.ItemId == -1)
        {
            return dataModel;
        }

        dataModel.BagItem.Clone(bagDataModel);
        var tbItem = Table.GetItemBase(bagDataModel.ItemId);
        dataModel.NeedLevel = tbItem.LevelLimit;
        dataModel.SellCount = 1;
        dataModel.MinSinglePrice = tbItem.TradeMin;
        dataModel.MaxSinglePrice = tbItem.TradeMax;
        dataModel.MaxSellCount = tbItem.TradeMaxCount < dataModel.BagItem.Count
            ? tbItem.TradeMaxCount
            : dataModel.BagItem.Count;
        if (dataModel.MaxSellCount == 0)
        {
            dataModel.SliderRate = 0;
        }
        else
        {
            dataModel.SliderCanMove = dataModel.MaxSellCount > 1;
            dataModel.SliderRate = 1/(float) dataModel.MaxSellCount;
        }
        var timeNow = Game.Instance.ServerTime;
        var timeCd = DateTime.FromBinary(mDataModel.PeddlingCd);
        dataModel.IsPeddling = timeNow > timeCd;

        dataModel.PeddleTime = string.Empty;
        dataModel.State = 1;
        dataModel.SellPrice = dataModel.SellCount*dataModel.MinSinglePrice;
        SinglePrice = dataModel.MinSinglePrice;

        return dataModel;
    }

    private void RefleshSellSelectingItem()
    {
        mDataModel.SellSelectingItem.Clone(CreateItemFromIdOrDataModel(mDataModel.SellSelectingItem.BagItem));
        //选择性刷新拍卖行数据
        RefleshSellItemType(mDataModel.SellSelectingItem);
    }

    public OtherPlayerTradingDataModel CreateOtherPlayerFromNetData(StoreBroadcastOne one)
    {
        var player = new OtherPlayerTradingDataModel();
        player.PlayerId = one.SellCharacterId;
        player.PlayerName = one.SellCharacterName;
        player.PeddingItem.SellPrice = one.NeedCount;

        var itemBase = one.ItemData;
        if (itemBase == null)
        {
            return player;
        }
        var bagItem = player.PeddingItem.BagItem;
        bagItem.ItemId = itemBase.ItemId;
        bagItem.Count = itemBase.Count;
        bagItem.Exdata.InstallData(itemBase.Exdata);
        player.PeddingItem.SellCount = bagItem.Count;
        return player;
    }

    #endregion

    #region 界面数据

    public void OnCoolDownChanged(IEvent ievent)
    {
        var e = ievent as UIEvent_TradingCoolDownChanged;
        mDataModel.PeddingCdMax = e.CD;
        mDataModel.PeddlingTime = e.CD.TotalHours.ToString("f0");

        mDataModel.PeddlingLastTimeMax = e.LastTime;
        mDataModel.PeddlingLastTimeString = GameUtils.GetTimeDiffString(mDataModel.PeddlingLastTimeMax);
    }

    public void OnBtnEvent(IEvent ievent)
    {
        var e = ievent as UIEvent_TradingFrameButton;

        switch (e.ButtonIndex)
        {
            //添加按钮
            case 0:
            {
                mMySellIndex = e.Data;
                mDataModel.SellInfoShow = true;

                //引导潜规则
                if (!PlayerDataManager.Instance.GetFlag(534))
                {
                    var list = new Int32Array();
                    list.Items.Add(534);

                    var list1 = new Int32Array();
                    list1.Items.Add(533);
                    PlayerDataManager.Instance.SetFlagNet(list, list1);
                }
            }
                break;
            //关闭卖出界面按钮
            case 1:
            {
                mDataModel.SellInfoShow = false;
            }
                break;
            //add
            case 2:
            {
                OnAddCount();
            }
                break;
            //sub
            case 3:
            {
                OnSubCount();
            }
                break;
            //addPress
            case 4:
            {
                ButtonPress = NetManager.Instance.StartCoroutine(OnAddPress());
            }
                break;
            //addRelease
            case 5:
            {
                if (ButtonPress != null)
                {
                    NetManager.Instance.StopCoroutine(ButtonPress);
                    ButtonPress = null;
                }
            }
                break;
            //subPress
            case 6:
            {
                ButtonPress = NetManager.Instance.StartCoroutine(OnSubPress());
            }
                break;
            //subRelease
            case 7:
            {
                if (null != ButtonPress)
                {
                    NetManager.Instance.StopCoroutine(ButtonPress);
                    ButtonPress = null;
                }
            }
                break;
            //上架新商品
            case 8:
            {
                StoreOperationAdd();
            }
                break;
            //下架商品
            case 9:
            {
                OnBtnCancel(e.Data);
            }
                break;
            //刷新别人的摊位
            case 10:
            {
                RefreshOtherPlayerWithMoney();
            }
                break;
            //获取别人摊位详细信息
            case 11:
            {
                mDataModel.OtherSellInfoShow = true;
                GetOtherPlayerInfo(e.Data);
            }
                break;
            //关闭别人摊位详细信息
            case 12:
            {
                mDataModel.OtherSellInfoShow = false;
            }
                break;
            //购买别人的商品
            case 13:
            {
                BuyOtherPlayerItem(e.Data);
            }
                break;
            //收获卖出物品
            case 14:
            {
                StoreOperationHarvest(e.Data);
            }
                break;
            //去逛街toggle
            case 15:
            {
                RefreshOtherPlayerWithCD();
            }
                break;
            //打开数字键盘
            case 16:
            {
                OpenNumberPad();
            }
                break;
            //兑换物品详情打开
            case 17:
            {
                OpenExchangeInfo(e.Data);
            }
                break;
            //兑换物品
            case 18:
            {
                OperationExchange();
            }
                break;
            //关闭兑换界面
            case 19:
            {
                mDataModel.ExchangeSellInfoShow = false;
            }
                break;
            //兑换界面切换刷新物品数量
            case 20:
            {
                RefreshExchangeItems();
            }
                break;
        }
    }

    public void OpenExchangeInfo(int index)
    {
        var exchange = mDataModel.ExchangeItems[index];
        if (null == exchange)
        {
            return;
        }

        mDataModel.SelectedExchangeItem.Clone(exchange);
        mDataModel.ExchangeSellInfoShow = true;
        mExchangeItemIndex = index;
    }

    public void OpenNumberPad()
    {
        var item = mDataModel.SellSelectingItem;
        var minvalue = item.MinSinglePrice*item.SellCount;
        var maxValue = item.SellCount*item.MaxSinglePrice;

        NumPadLogic.ShowNumberPad(minvalue, maxValue, value =>
        {
            if (value == -1)
            {
                return;
            }

            var selectItem = mDataModel.SellSelectingItem;
            if (value >= selectItem.MinSinglePrice && value <= selectItem.MaxSinglePrice*selectItem.SellCount)
            {
                selectItem.SellPrice = value;
                SinglePrice = selectItem.SellPrice/(float) selectItem.SellCount;
            }
        });
    }

    public void SellPriceChange(int value)
    {
        var item = mDataModel.SellSelectingItem;
        if (value >= item.MinSinglePrice && value <= item.MaxSinglePrice*item.SellCount)
        {
            item.SellPrice = value;
            SinglePrice = item.SellPrice/(float) item.SellCount;
        }
    }

    public void BagItemClick(IEvent ievent)
    {
        var e = ievent as UIEvent_TradingBagItemClick;
        var bagItem = e.BagItem;
        mDataModel.SellSelectingItem.Clone(CreateItemFromIdOrDataModel(bagItem));
        //选择性刷新拍卖行数据
        RefleshSellItemType(mDataModel.SellSelectingItem);
    }


    //刷新显示
    private void RefleshSellItemType(TradingItemDataModel item)
    {
        if (mDataModel.SellSelectingItem == null)
        {
            return;
        }

        if (mDataModel.EquipTabPage != 0 && mDataModel.SellTypeList[0])
        {
            //var item = mDataModel.SellSelectingItem;
            item.SellType = (int) eResourcesType.DiamondRes;
            var sellPrice = int.Parse(Table.GetClientConfig(610).Value);
            item.SellPrice = sellPrice;
            item.PeddleDateTime = Game.Instance.ServerTime.AddHours(Table.GetClientConfig(611).Value.ToInt());
            item.PeddleTime = GameUtils.GetTimeDiffString(item.PeddleDateTime);
            item.MinSinglePrice = sellPrice;
            item.MaxSinglePrice = 999999;
        }
        else
        {
            item.SellType = (int) eResourcesType.Other16;
            var tbItem = Table.GetItemBase(item.BagItem.ItemId);
            if (tbItem == null)
            {
                return;
            }
            if (BitFlag.GetLow(tbItem.CanTrade, 1))
            {
                item.PeddleDateTime = Game.Instance.ServerTime.AddHours(Table.GetClientConfig(611).Value.ToInt());
                item.PeddleTime = GameUtils.GetTimeDiffString(item.PeddleDateTime);
            }
        }
    }

    public void OnItemSelled(IEvent ievent)
    {
        var e = ievent as UIEvent_OnTradingItemSelled;
        _needApplyHistory = true; //打开历史重新请求
        _canApplyHistoryCount = 2;
        if (mDataModel != null)
        {
            var count = mDataModel.MyTradingItems.Count;
            var bFind = false;
            for (var i = 0; i < count; i++)
            {
                var item = mDataModel.MyTradingItems[i];
                if (item.TradingItemId == e.itemId)
                {
                    bFind = true;
                    item.State = GetItemState((int) StoreItemType.Buyed);
                    var name = Table.GetItemBase(item.BagItem.ItemId).Name;
                    EventDispatcher.Instance.DispatchEvent(
                        new ShowUIHintBoard(name + GameUtils.GetDictionaryText(270119)));
                    break;
                }
            }

            if (!bFind)
            {
                Logger.Error("Cant Find Trading Item id ={0}", e.itemId);
            }
            RefreshNotice();
        }
    }

    public void OnRateChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SliderRate")
        {
            var dataModel = mDataModel.SellSelectingItem;
            if (dataModel.BagItem.ItemId == -1)
            {
                dataModel.SellCount = 0;
                dataModel.SellPrice = 0;
                return;
            }
            if (mDataModel.EquipTabPage == 0)
            {
                dataModel.SellCount = (int) (Mathf.Round(dataModel.SliderRate*(dataModel.MaxSellCount - 1)) + 1);
                dataModel.SellPrice = (int) (dataModel.SellCount*SinglePrice);
            }
        }
    }

    public void OnExchangeRateChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "SliderRate")
        {
            var dataModel = mDataModel.SelectedExchangeItem;
            dataModel.SellGroupCount = (int) (Mathf.Round(dataModel.SliderRate*(dataModel.SellGroupCountMax - 1)) + 1);
            dataModel.SellCount = dataModel.SellGroupCount*dataModel.SellGroupRate;
            dataModel.SellPrice = (dataModel.SellCount*dataModel.Price);
        }
    }

    public void MyStackChange(object sender, PropertyChangedEventArgs e)
    {
        RefreshMyStackCount();
    }

    private void RefreshMyStackCount()
    {
        var current = 0;
        var c = mDataModel.MyTradingItems.Count;
        for (var i = 0; i < c; i++)
        {
            var item = mDataModel.MyTradingItems[i];
            if (item.State == 0)
            {
                current++;
            }
        }
        mDataModel.MyStackCount = string.Format("{0}/{1}", c - current, c);
        EventDispatcher.Instance.DispatchEvent(new UIEvent_CityTradingStack(c - current, c));
    }

    public bool OnAddCount()
    {
        var dataModel = mDataModel.SellSelectingItem;
        if (SinglePrice < dataModel.MaxSinglePrice)
        {
            dataModel.SellPrice++;
            SinglePrice = dataModel.SellPrice/(float) dataModel.SellCount;
            return true;
        }
        return false;
    }

    public bool OnSubCount()
    {
        var dataModel = mDataModel.SellSelectingItem;
        if (SinglePrice > dataModel.MinSinglePrice)
        {
            dataModel.SellPrice--;
            SinglePrice = dataModel.SellPrice/(float) dataModel.SellCount;
            return true;
        }
        return false;
    }

    public IEnumerator OnAddPress()
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnAddCount() == false)
            {
                NetManager.Instance.StopCoroutine(ButtonPress);
                ButtonPress = null;
                yield break;
            }
            if (pressCd > 0.0001)
            {
                pressCd = pressCd*0.8f;
            }
        }
    }

    public IEnumerator OnSubPress()
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnSubCount() == false)
            {
                NetManager.Instance.StopCoroutine(ButtonPress);
                ButtonPress = null;
                yield break;
            }
            if (pressCd > 0.0001)
            {
                pressCd = pressCd*0.8f;
            }
        }
    }

    public void RefreshNotice()
    {
        if (null != mDataModel)
        {
            var count = mDataModel.MyTradingItems.Count;
            var noticeState = false;
            for (var i = 0; i < count; i++)
            {
                var item = mDataModel.MyTradingItems[i];
                if (item.State == 2)
                {
                    noticeState = true;
                    break;
                }
            }
            EventDispatcher.Instance.DispatchEvent(new UIEvent_CityRefreshTradingNotice(noticeState));
            PlayerDataManager.Instance.NoticeData.MyTradingItemSelled = noticeState;
        }
    }

    #endregion

    #region 网络数据

    public void OnBtnCancel(int index)
    {
        if (index >= mDataModel.MyTradingItems.Count)
        {
            Logger.Error("StoreOperationCancel index error!! index =" + index);
        }
        else
        {
            NetManager.Instance.StartCoroutine(StoreOperationCancel(index));
        }
    }

    public IEnumerator StoreOperationCancel(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var item = mDataModel.MyTradingItems[index];
            var msg = NetManager.Instance.StoreOperationCancel(item.TradingItemId);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    item.Clone(new TradingItemDataModel());
                    RefreshMyStackCount();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void StoreOperationAdd()
    {
        var tradItem = mDataModel.SellSelectingItem;
        if (tradItem.BagItem.ItemId == -1)
        {
            return;
        }
        var type = 0;
        if (tradItem.IsPeddling)
        {
            var timeNow = Game.Instance.ServerTime;
            var timeCd = DateTime.FromBinary(mDataModel.PeddlingCd);
            if (timeNow > timeCd)
            {
                type = 1;
                NetManager.Instance.StartCoroutine(OnStoreOperationAdd(type));
            }
            else
            {
                var cast = Table.GetClientConfig(302);
                //叫卖冷却剩余{0},是否花费{1}钻石购买冷却!
                var message = string.Format(GameUtils.GetDictionaryText(270120),
                    GameUtils.GetTimeDiffString(timeCd - timeNow), cast.Value);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, message, "",
                    () =>
                    {
                        var diamond = PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes);
                        if (diamond < int.Parse(cast.Value))
                        {
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300401));
                            return;
                        }

                        type = 2;
                        NetManager.Instance.StartCoroutine(OnStoreOperationAdd(type));
                    },
                    () => { });
            }
        }
        else
        {
            NetManager.Instance.StartCoroutine(OnStoreOperationAdd(type));
        }
    }

    public IEnumerator OnStoreOperationAdd(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var tradItem = mDataModel.SellSelectingItem;
            var item = tradItem.BagItem;

            var msg = NetManager.Instance.StoreOperationAdd(type, item.BagId, item.Index, tradItem.SellCount,
                tradItem.SellPrice, mMySellIndex);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlatformHelper.Event("city", "TradingAdd");
                    //上架成功
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270121));
                    mDataModel.SellSelectingItem.TradingItemId = msg.Response;
                    mDataModel.MyTradingItems[mMySellIndex].Clone(mDataModel.SellSelectingItem);
                    mDataModel.SellSelectingItem.Clone(new TradingItemDataModel());
                    mDataModel.SellInfoShow = false;
                    if (type != 0)
                    {
                        var timecd = Game.Instance.ServerTime + mDataModel.PeddingCdMax;
                        mDataModel.PeddlingCd = timecd.ToBinary();

                        mDataModel.MyTradingItems[mMySellIndex].PeddleTime =
                            GameUtils.GetTimeDiffString(mDataModel.PeddlingLastTimeMax);
                        mDataModel.MyTradingItems[mMySellIndex].PeddleDateTime =
                            Game.Instance.ServerTime + mDataModel.PeddlingLastTimeMax;
                    }
                    RefreshMyStackCount();
                    if (type == 1 || type == 2)
                    {
                        //飞经验
                        var exp = int.Parse(Table.GetClientConfig(303).Value);
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFlyAnim(exp));
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void RefreshOtherPlayerWithMoney()
    {
        var cast = Table.GetClientConfig(305);
        //是否花费{0}钻石刷新?
        var message = string.Format(GameUtils.GetDictionaryText(270122), cast.Value);
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, message, "",
            () =>
            {
                var diamond = PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes);
                if (diamond < int.Parse(cast.Value))
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300401));
                    return;
                }
                NetManager.Instance.StartCoroutine(RefreshOtherPlayerMsg(1));
            },
            () => { });
    }

    public void RefreshOtherPlayerWithCD()
    {
        var freeTIme = Extension.FromServerBinary(mDataModel.BroadCastNextFreeTime);
        var nowTime = Game.Instance.ServerTime;

        if (freeTIme < nowTime)
        {
            NetManager.Instance.StartCoroutine(RefreshOtherPlayerMsg(0));
        }
    }

    public IEnumerator RefreshOtherPlayerMsg(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreOperationBroadcast(type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    mDataModel.OtherPlayers.Clear();
                    var list = msg.Response.Items;
                    var otherPlayer = new OtherPlayerTradingDataModel();
                    var count = list.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var onePlayer = CreateOtherPlayerFromNetData(list[i]);
                        mDataModel.OtherPlayers.Add(onePlayer);
                    }
                    mDataModel.BroadCastNextFreeTime = msg.Response.CacheOverTime;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void GetOtherPlayerInfo(int index)
    {
        var player = mDataModel.OtherPlayers[index];
        mDataModel.SelectionOtherPlayer.PlayerId = player.PlayerId;
        mDataModel.SelectionOtherPlayer.PlayerName = player.PlayerName;
        mDataModel.SelectionOtherPlayer.PeddingItem.Clone(player.PeddingItem);

        NetManager.Instance.StartCoroutine(StoreOperationLook(player.PlayerId));
    }

    public IEnumerator StoreOperationLook(ulong playerId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreOperationLook(playerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                mDataModel.OtherPlayerItems.Clear();
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var list = msg.Response;
                    SellerId = list.SellCharacterId;
                    var count = list.Items.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var one = list.Items[i];
                        var item = CreateItemFromNetData(one);
                        mDataModel.OtherPlayerItems.Add(item);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }


    public void BuyOtherPlayerItem(int index)
    {
        var res = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources;
        var item = mDataModel.OtherPlayerItems[index];
        if (item.TradeType == (int) eSellType.DiamondRes || item.TradeType == (int) eSellType.Other16)
        {
            if (item.SellType == (int) eResourcesType.DiamondRes && res.Diamond < item.SellPrice)
            {
                //您的货币不足，购买失败
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(429));
                return;
            }
            if (item.SellType == (int) eResourcesType.Other16 && res.MuCurrency < item.SellPrice)
            {
                //您的货币不足，购买失败
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(429));
                return;
            }
            NetManager.Instance.StartCoroutine(BuyItemAuctionCoroutine(mDataModel.SelectionOtherPlayer.PlayerId,
                item.TradingItemId, item.ManagerId, () =>
                {
                    PlatformHelper.Event("city", "TradingBuy");
                    item.State = GetItemState((int) StoreItemType.Buyed);
                    _needApplyHistory = true;
                    _canApplyHistoryCount = 2;
                }));
        }
        else
        {
            if (res.MuCurrency < item.SellPrice)
            {
                //您的货币不足，购买失败
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(429));
                return;
            }
            NetManager.Instance.StartCoroutine(StoreOperationBuy(index));
        }
    }

    public IEnumerator StoreOperationBuy(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var item = mDataModel.OtherPlayerItems[index];
            var msg = NetManager.Instance.StoreOperationBuy(mDataModel.SelectionOtherPlayer.PlayerId, item.TradingItemId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlatformHelper.Event("city", "TradingBuy");
                    var res = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources;
                    var muGold = res.MuCurrency - item.SellPrice;
                    PlayerDataManager.Instance.SetRes(16, muGold);
                    item.State = GetItemState((int) StoreItemType.Buyed);
                }
                else
                {
                    //您购买的道具已被其它玩家买走
                    UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270123), "",
                        () =>
                        {
                            NetManager.Instance.StartCoroutine(
                                StoreOperationLook(mDataModel.SelectionOtherPlayer.PlayerId));
                        });
                }
            }
        }
    }

    public void StoreOperationHarvest(int index)
    {
        NetManager.Instance.StartCoroutine(StoreOperationHarvestCoroutine(index));
    }

    public IEnumerator StoreOperationHarvestCoroutine(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var item = mDataModel.MyTradingItems[index];
            var msg = NetManager.Instance.StoreOperationHarvest(item.TradingItemId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //var res = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources;
                    //res.MuCurrency = res.MuCurrency + item.SellPrice;
                    var emptyItem = CreateItemFromIdOrDataModel(-1);
                    item.Clone(emptyItem);
                    RefreshNotice();
                    RefreshMyStackCount();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void OperationExchange()
    {
        NetManager.Instance.StartCoroutine(StoreOperationExchange());
    }

    public IEnumerator StoreOperationExchange()
    {
        using (new BlockingLayerHelper(0))
        {
            var item = mDataModel.SelectedExchangeItem;
            var msg = NetManager.Instance.SSStoreOperationExchange(item.ExchangeId, item.SellCount);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var res = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources;
                    var mugold = res.MuCurrency + item.SellPrice;
                    PlayerDataManager.Instance.SetRes(16, mugold);
                    item.BagItem.Count -= item.SellCount;
                    var count = item.BagItem.Count;
                    item.SliderRate = 0;
                    item.SellGroupCountMax = count/item.SellGroupRate;
                    item.SellGroupCount = count >= item.SellGroupRate ? 1 : 0;
                    item.SellCount = item.SellGroupRate;
                    item.SellPrice = (item.SellCount*item.Price);
                    mDataModel.ExchangeSellInfoShow = false;
                    mDataModel.ExchangeItems[mExchangeItemIndex].Clone(item);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    #endregion

    #region //装备交易行功能

    public enum eSortType
    {
        PriceUp = 0,
        PriceDown = 1,
        TimeUp = 2,
        TimeDown = 3,
        None = 4
    }

    public enum eEquipPage
    {
        SubPage = 0,
        DetailPage = 1
    }

    private int _mTotalPage = 1; //总的页数
    private readonly int MaxPageCount = 10; //一页最大显示条数
    private int _mOptSelectIndex; // 单选按钮index 
    private eSortType _mSortType = eSortType.None; //排列方式

    private ExchangeEquipMenuDataModel _mSelectedMenuItem; //选择菜单index
    private ExchangeEquipItemDataModel _mSelectedEquipItem;
    private bool _needApplyHistory = true;
    private int _canApplyHistoryCount = 2;


    private readonly Dictionary<int, ExchangeEquipMenuDataModel> _mMotherList =
        new Dictionary<int, ExchangeEquipMenuDataModel>(); //主menu

    private readonly Dictionary<int, List<ExchangeEquipMenuDataModel>> _mSonList =
        new Dictionary<int, List<ExchangeEquipMenuDataModel>>(); //子menu

    private readonly Dictionary<int, List<ExchangeEquipItemDataModel>> _mEquipLists =
        new Dictionary<int, List<ExchangeEquipItemDataModel>>(); //装备拍卖行总的物品

    private void InitMenuList()
    {
        _mMotherList.Clear();
        _mSonList.Clear();
        var list = new List<ExchangeEquipMenuDataModel>();
        var index1 = 0;
        Table.ForeachAuctionType1(table =>
        {
            var item = new ExchangeEquipMenuDataModel();
            item.Type = 1;
            item.Id = table.Id;
            item.TypeName = table.Type;
            item.Selected = false;
            item.Index = index1;
            index1++;
            //list.Add(item);
            _mMotherList.Add(table.Id, item);
            list.Add(item);
            var sonList = new List<ExchangeEquipMenuDataModel>();
            var count = table.SonList.Count;
            for (var i = 0; i < count; i++)
            {
                var index2 = 0;
                var sonItem = new ExchangeEquipMenuDataModel();
                sonItem.Type = 0;
                var tbAuction2 = Table.GetAuctionType2(table.SonList[i]);
                if (tbAuction2 == null)
                {
                    return true;
                }
                sonItem.Id = tbAuction2.Id;
                sonItem.TypeName = tbAuction2.Name;
                sonItem.Selected = false;
                sonItem.Index = index2;
                index2++;
                sonList.Add(sonItem);
                //
            }
            _mSonList.Add(table.Id, sonList);
            return true;
        });
        mDataModel.EquipData.MenuDatas = new ObservableCollection<ExchangeEquipMenuDataModel>(list);
    }


    public void MenuSelected(int index)
    {
        var menu = mDataModel.EquipData.MenuDatas;

        if (menu[index].Type == 1)
        {
            var list = new List<ExchangeEquipMenuDataModel>();
            if (menu[index].Selected)
            {
                menu[index].Selected = false;
                for (var i = 0; i < _mMotherList.Count; i++)
                {
                    list.Add(_mMotherList[i]);
                }
                mDataModel.EquipData.MenuDatas = new ObservableCollection<ExchangeEquipMenuDataModel>(list);
            }
            else
            {
                if (_mSelectedMenuItem != null)
                {
                    _mSelectedMenuItem.Selected = false;
                }

                var sonIndex = menu[index].Index;
                for (var i = 0; i < _mMotherList.Count; i++)
                {
                    _mMotherList[i].Selected = false;
                    list.Add(_mMotherList[i]);
                    if (i != sonIndex)
                    {
                        continue;
                    }
                    _mMotherList[i].Selected = true;
                    var sonCount = _mSonList[sonIndex].Count;
                    for (var j = 0; j < sonCount; j++)
                    {
                        if (j == 0)
                        {
                            _mSonList[i][j].Selected = true;
                            _mSelectedMenuItem = _mSonList[i][j];
                        }
                        else
                        {
                            _mSonList[i][j].Selected = false;
                        }
                        list.Add(_mSonList[i][j]);
                    }
                }
                mDataModel.EquipData.MenuDatas = new ObservableCollection<ExchangeEquipMenuDataModel>(list);

                var tbAuction = Table.GetAuctionType1(menu[index].Id);
                RefleshSon(tbAuction.SonList[0]);
            }
        }
        else
        {
            if (_mSelectedMenuItem != null)
            {
                _mSelectedMenuItem.Selected = false;
            }
            menu[index].Selected = true;
            _mSelectedMenuItem = menu[index];
            RefleshSon(menu[index].Id);
        }
    }

    public enum eSellType
    {
        DiamondRes = 11,
        Other16 = 10
    }

    private void SetEquipData(List<AuctionItemOne> items)
    {
        _mEquipLists.Clear();
        var list = new List<ExchangeEquipItemDataModel>();
        var list2 = new List<ExchangeEquipItemDataModel>();
        foreach (var item in items)
        {
            var dataOne = new ExchangeEquipItemDataModel();
            dataOne.MangerId = item.ManagerId;
            dataOne.Guid = item.ItemGuid;
            dataOne.Item.ItemId = item.ItemData.ItemId;
            dataOne.Item.Exdata.InstallData(item.ItemData.Exdata);
            dataOne.SellCharacterId = item.SellCharacterId;
            if (item.NeedType == (int) eSellType.DiamondRes)
            {
                dataOne.Type = (int) eResourcesType.DiamondRes;
            }
            else if (item.NeedType == (int) eSellType.Other16)
            {
                dataOne.Type = (int) eResourcesType.Other16;
            }
            dataOne.Price = item.NeedCount;
            dataOne.SellName = item.SellCharacterName;
            dataOne.Time = Extension.FromServerBinary(item.OverTime);
            dataOne.TimeStr = GameUtils.GetTimeDiffString(dataOne.Time);
            if (item.NeedType == (int) eSellType.DiamondRes)
            {
                list.Add(dataOne);
            }
            else if (item.NeedType == (int) eSellType.Other16)
            {
                list2.Add(dataOne);
            }
        }
        _mEquipLists.Add((int) eSellType.DiamondRes, list);
        _mEquipLists.Add((int) eSellType.Other16, list2);
    }

    private void RefleshSon(int auction1Id)
    {
        var classList = new List<ExchangeEquipClassItemDataModel>();
        var tbAution2 = Table.GetAuctionType2(auction1Id);
        var count = tbAution2.SonList.Count;
        for (var i = 0; i < count; i++)
        {
            var tbAution3 = Table.GetAuctionType3(tbAution2.SonList[i]);
            var item = new ExchangeEquipClassItemDataModel();
            item.Id = tbAution3.Id;
            classList.Add(item);
        }
        mDataModel.EquipData.ClassList = new ObservableCollection<ExchangeEquipClassItemDataModel>(classList);

        mDataModel.EquipData.ShowWitchPage = (int) eEquipPage.SubPage;
    }

    private void OnEquipOperation(IEvent ievent)
    {
        var e = ievent as UIEvent_OnTradingEquipOperation;
        switch (e.Type)
        {
            case 0:
            {
                BtnPageUp();
            }
                break;
            case 1:
            {
                BtnPageDown();
            }
                break;
            case 2:
            {
                ApplySellHistory();
            }
                break;
            case 3:
            {
                OnClickPrice();
            }
                break;
            case 4:
            {
                OnClickTime();
            }
                break;
            case 5:
            {
                MenuSelected(e.Value);
            }
                break;
            case 6:
            {
                SonSelect(e.Value);
            }
                break;
            case 7:
            {
                EquipItemSelected(e.Value);
            }
                break;
            case 8:
            {
                SetSelectType(e.Value);
                if (mDataModel.EquipData.ShowWitchPage == 1)
                {
                    _mSortType = eSortType.PriceUp;
                    ResetPage();
                }
            }
                break;
            case 9:
            {
                SetSellSelectType(e.Value);
            }
                break;
            case 10:
            {
                BuyItemAuction();
            }
                break;
            case 11:
            {
                OnTradingTabClick(e.Value);
            }
                break;
            case 12:
            {
                OnItemAuction();
            }
                break;
            case 13:
            {
                BuyItemAuctionCancel();
            }
                break;
        }
    }

    private void BuyItemAuctionCancel()
    {
        mDataModel.EquipData.IsShowBuyPage = false;
    }

    private void EquipItemSelected(int index)
    {
        _mSelectedEquipItem = mDataModel.EquipData.Items[index];
        mDataModel.EquipData.BuyItem = mDataModel.EquipData.BuyItem.Clone(_mSelectedEquipItem);
        mDataModel.EquipData.IsShowBuyPage = true;
    }


    private void SetSellSelectType(int index)
    {
        for (var i = 0; i < mDataModel.SellTypeList.Count; i++)
        {
            mDataModel.SellTypeList[i] = false;
        }
        if (index == 0)
        {
            mDataModel.SellTypeList[0] = true;
        }
        else
        {
            mDataModel.SellTypeList[1] = true;
        }
        RefleshSellSelectingItem();
    }

    private void OnClickTime()
    {
        if (_mSortType == eSortType.TimeUp)
        {
            _mSortType = eSortType.TimeDown;
        }
        else
        {
            _mSortType = eSortType.TimeUp;
        }
        ResetPage();
    }

    private void OnClickPrice()
    {
        if (_mSortType == eSortType.PriceUp)
        {
            _mSortType = eSortType.PriceDown;
        }
        else
        {
            _mSortType = eSortType.PriceUp;
        }
        ResetPage();
    }


    private void ResetPage()
    {
        var mEquipData = mDataModel.EquipData;
        mEquipData.PageIndex = 1;
        RefleshEquipList();
    }

    private void BtnPageDown()
    {
        var mEquipData = mDataModel.EquipData;
        if (mEquipData.PageIndex == 1)
        {
            return;
        }
        mEquipData.PageIndex --;
        RefleshEquipList();
    }

    private void BtnPageUp()
    {
        var mEquipData = mDataModel.EquipData;
        if (mEquipData.PageIndex >= _mTotalPage)
        {
            return;
        }
        mEquipData.PageIndex++;
        RefleshEquipList();
    }

    private void SetPageBtnState()
    {
        var mEquipData = mDataModel.EquipData;
        if (_mTotalPage <= 1 || mEquipData.PageIndex <= 1)
        {
            mEquipData.IsShowPageFront = false;
        }
        else
        {
            mEquipData.IsShowPageFront = true;
        }
        if (mEquipData.PageIndex >= _mTotalPage)
        {
            mEquipData.IsShowPageBack = false;
        }
        else
        {
            mEquipData.IsShowPageBack = true;
        }
    }

    private void RefleshEquipList()
    {
        var equipList = _mEquipLists[_mOptSelectIndex];
        var mEquipData = mDataModel.EquipData;
        if (equipList == null || equipList.Count == 0)
        {
            mEquipData.Items = new ObservableCollection<ExchangeEquipItemDataModel>();
            return;
        }
        mEquipData.PageIndex = Math.Max(mEquipData.PageIndex, 1);
        _mTotalPage = (equipList.Count - 1)/MaxPageCount + 1;

        var startIndex = (mEquipData.PageIndex - 1)*MaxPageCount;
        if (startIndex >= equipList.Count)
        {
            Logger.Error("Trading RefleshEquipList error");
        }
        var items = new List<ExchangeEquipItemDataModel>();
        var addCount = 0;
        for (var i = startIndex; i < equipList.Count && addCount < MaxPageCount; i++)
        {
            items.Add(equipList[i]);
            addCount++;
        }
        SetPageBtnState();
        SetLabelState();
        ResetSort(_mSortType, items);
        mEquipData.Items = new ObservableCollection<ExchangeEquipItemDataModel>(items);
    }


    private void ResetSort(eSortType type, List<ExchangeEquipItemDataModel> Lists)
    {
        switch (type)
        {
            case eSortType.PriceUp:
            {
                Lists.Sort((a, b) =>
                {
                    if (a.Price > b.Price)
                    {
                        return 1;
                    }
                    if (a.Price == b.Price)
                    {
                        if (a.Time < b.Time)
                        {
                            return -1;
                        }
                        return 1;
                    }
                    return -1;
                });
            }
                break;

            case eSortType.PriceDown:
            {
                Lists.Sort((a, b) =>
                {
                    if (a.Price < b.Price)
                    {
                        return 1;
                    }
                    if (a.Price == b.Price)
                    {
                        if (a.Time < b.Time)
                        {
                            return -1;
                        }
                        return 1;
                    }
                    return -1;
                });
            }
                break;
            case eSortType.TimeUp:
            {
                Lists.Sort((a, b) =>
                {
                    if (a.Time < b.Time)
                    {
                        return -1;
                    }
                    if (a.Time == b.Time)
                    {
                        if (a.Price < b.Price)
                        {
                            return -1;
                        }
                        return 1;
                    }
                    return 1;
                });
            }
                break;
            case eSortType.TimeDown:
            {
                Lists.Sort((a, b) =>
                {
                    if (a.Time < b.Time)
                    {
                        return 1;
                    }
                    if (a.Time == b.Time)
                    {
                        if (a.Price < b.Price)
                        {
                            return -1;
                        }
                        return 1;
                    }
                    return -1;
                });
            }
                break;
        }
    }

    private void SetLabelState()
    {
        var equipData = mDataModel.EquipData;
        //equipData.PriceLabel = "价格";
        //equipData.TimeLabel = "时间";
        switch (_mSortType)
        {
            case eSortType.PriceUp:
                equipData.ArrowDirection = 0;
                //equipData.PriceLabel += "↓";
                break;
            case eSortType.PriceDown:
                equipData.ArrowDirection = 1;
                //equipData.PriceLabel += "↑";
                break;
            case eSortType.TimeUp:
                equipData.ArrowDirection = 2;
                //equipData.TimeLabel += "↑";
                break;
            case eSortType.TimeDown:
                equipData.ArrowDirection = 3;
                //equipData.TimeLabel += "↓";
                break;
            case eSortType.None:
                break;
        }
    }

    private void SetSelectType(int index)
    {
        for (var i = 0; i < mDataModel.EquipData.ItemTypeOpt.Count; i++)
        {
            mDataModel.EquipData.ItemTypeOpt[i] = false;
        }
        if (index == 0)
        {
            _mOptSelectIndex = (int) eSellType.DiamondRes;
            mDataModel.EquipData.ItemTypeOpt[0] = true;
        }
        else
        {
            _mOptSelectIndex = (int) eSellType.Other16;
            mDataModel.EquipData.ItemTypeOpt[1] = true;
        }
    }

    private void SetSelectIndex()
    {
        var index = 0;
        var opt = mDataModel.EquipData.ItemTypeOpt;
        for (var i = 0; i < opt.Count; i++)
        {
            if (opt[i])
            {
                index = i;
                break;
            }
        }
        if (index == 0)
        {
            _mOptSelectIndex = (int) eSellType.DiamondRes;
        }
        else
        {
            _mOptSelectIndex = (int) eSellType.Other16;
        }
    }

    private void OnTradingTabClick(int index)
    {
        mDataModel.EquipTabPage = index;
        if (index == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new PackTradingSellPage(index, 1));
        }
        else if (index == 1)
        {
            EventDispatcher.Instance.DispatchEvent(new PackTradingSellPage(index, 0));
        }
    }


    private void RefleshTradingEquipTabPage(IEvent ievent)
    {
        var e = ievent as UIEvent_TradingEquipTabPage;
        mDataModel.EquipTabPage = e.Page;
    }

    private void OnItemAuction()
    {
        NetManager.Instance.StartCoroutine(OnItemAuctionCoroutine(() => { PlatformHelper.Event("city", "TradingAdd"); }));
    }

    private IEnumerator OnItemAuctionCoroutine(Action act)
    {
        using (new BlockingLayerHelper(0))
        {
            var tradItem = mDataModel.SellSelectingItem;
            var item = tradItem.BagItem;
            if (item.ItemId == -1)
            {
                yield break;
            }
            var type = 10;
            if (mDataModel.SellTypeList[0])
            {
                type = (int) eSellType.DiamondRes;
            }
            else if (mDataModel.SellTypeList[1])
            {
                type = (int) eSellType.DiamondRes;
            }
            var msg = NetManager.Instance.OnItemAuction(type, item.BagId, item.Index, tradItem.SellCount,
                tradItem.SellPrice, mMySellIndex);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    mDataModel.SellSelectingItem.TradingItemId = msg.Response;
                    //上架成功
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270121));
                    mDataModel.MyTradingItems[mMySellIndex].Clone(mDataModel.SellSelectingItem);
                    mDataModel.MyTradingItems[mMySellIndex].TradingItemId = msg.Response;
                    mDataModel.SellSelectingItem.Clone(new TradingItemDataModel());
                    mDataModel.SellInfoShow = false;

                    //if (type != 0)
                    //{
                    //    var timecd = Game.Instance.ServerTime + mDataModel.PeddingCdMax;
                    //    mDataModel.PeddlingCd = timecd.ToBinary();

                    //    mDataModel.MyTradingItems[mMySellIndex].PeddleTime =
                    //        GameUtils.GetTimeDiffString(mDataModel.PeddlingLastTimeMax);
                    //    mDataModel.MyTradingItems[mMySellIndex].PeddleDateTime =
                    //        Game.Instance.ServerTime + mDataModel.PeddlingLastTimeMax;
                    //}
                    RefreshMyStackCount();
                    act();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void BuyItemAuction()
    {
        var playerId = PlayerDataManager.Instance.Guid;
        if (playerId == _mSelectedEquipItem.SellCharacterId)
        {
            GameUtils.ShowHintTip(300881);
            return;
        }
        NetManager.Instance.StartCoroutine(BuyItemAuctionCoroutine(_mSelectedEquipItem.SellCharacterId,
            _mSelectedEquipItem.Guid, _mSelectedEquipItem.MangerId, () =>
            {
                foreach (var item in _mEquipLists[_mOptSelectIndex])
                {
                    if (item.Guid == _mSelectedEquipItem.Guid)
                    {
                        _mEquipLists[_mOptSelectIndex].Remove(item);
                        break;
                    }
                }
                mDataModel.EquipData.Items.Remove(_mSelectedEquipItem);
                mDataModel.EquipData.IsShowBuyPage = false;
            }));
    }

    private IEnumerator BuyItemAuctionCoroutine(ulong sellCharacterId,
                                                long guid,
                                                long mangerId,
                                                Action act)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.BuyItemAuction(sellCharacterId, guid, mangerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    act();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void SonSelect(int index)
    {
        var id = mDataModel.EquipData.ClassList[index].Id;
        NetManager.Instance.StartCoroutine(CSSelectItemAuction(id, () =>
        {
            mDataModel.EquipData.ShowWitchPage = (int) eEquipPage.DetailPage;
            SetSelectIndex();
            _mSortType = eSortType.PriceUp;
            ResetPage();
        }));
    }


    private IEnumerator CSSelectItemAuction(int type, Action act)
    {
        using (new BlockingLayerHelper(0))
        {
            var instance = PlayerDataManager.Instance;
            var msg = NetManager.Instance.CSSelectItemAuction(instance.ServerId, type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (msg.Response.Items.Count == 0)
                    {
                        GameUtils.ShowHintTip(300880);
                        yield break;
                    }
                    SetEquipData(msg.Response.Items);

                    act();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void ApplySellHistory()
    {
        if (_needApplyHistory && _canApplyHistoryCount > 0)
        {
            NetManager.Instance.StartCoroutine(ApplySellHistoryCoroutine(() =>
            {
                _canApplyHistoryCount = 0;
                _needApplyHistory = false;
            }));
        }
    }

    private IEnumerator ApplySellHistoryCoroutine(Action act)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplySellHistory(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    SetHistoryData(msg.Response.items);
                    act();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
                _canApplyHistoryCount--;
                ApplySellHistory();
            }
        }
    }

    private void SetHistoryData(List<SellHistoryOne> items)
    {
        if (items == null || items.Count == 0)
        {
            mDataModel.EquipData.HistoryEmptyTip = true;
            return;
        }
        mDataModel.EquipData.HistoryEmptyTip = false;
        mDataModel.HistoryList.Clear();
        var myGuid = PlayerDataManager.Instance.Guid;
        var list = new List<ExchangeEquipHistoryItemDataModel>();
        for (var i = items.Count - 1; i >= 0; i--)
        {
            var value = items[i];
            var item = new ExchangeEquipHistoryItemDataModel();
            item.Item.ItemId = value.ItemData.ItemId;
            item.Item.Exdata.InstallData(value.ItemData.Exdata);
            item.Item.Count = value.ItemData.Count;
            item.Name = value.buyCharacterName;
            //if (value.buyCharacterId == myGuid)
            //{
            item.Type = value.type;
            // }
            //else
            //{
            //    item.Type = 1;
            //}
            item.SaleType = value.resType;
            item.SaleCount = value.resCount;
            item.Time = Extension.FromServerBinary(value.sellTime).ToString("yyyy/MM/dd HH:mm:ss");
            list.Add(item);
        }
        mDataModel.HistoryList = new ObservableCollection<ExchangeEquipHistoryItemDataModel>(list);
    }

    #endregion
}