/********************************************************************************* 

                         Scorpion



  *FileName:WishingController				

  *Version:1.0

  *Date:2017-06-06

  *Description:

**********************************************************************************/
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
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class AspirationFrameCtrler : IControllerBase
{
   
    #region 静态变量
    public const int INTERVEL_TIME = 5;
    #endregion

    #region 成员变量



    private int m_canDrawTypeCount = 8;
    private int m_reduceCount = 0;
    private int m_openLevel = 1;

    private Coroutine m_buttonPress; //临时物品鼠标按下
    private Coroutine m_buyButtonPress; //团购购买鼠标按下
    //    public Coroutine refItemTimeCor3;    //历史团购物品时间显示线程

    private Dictionary<int, int> m_dicNum_Level;
    private int m_freeType; //抽奖类型
    private bool m_isOneDraw;
    private List<int> m_lockList;
    private List<DateTime> m_RefleshTime = new List<DateTime>(); //刷新时间
    private bool m_pressLongTime; //背包物品是否为长按
    private Coroutine m_refItemTimeCor; //我的团购物品时间显示线程
    private Coroutine m_refItemTimeCor2; //当前团购物品时间显示线程
    private ClientConfigRecord m_tbGetTime = Table.GetClientConfig(413); //开奖状态时间
    private Coroutine m_timeCoroutine;
    private int m_treeId;

    private WishingDrawDataModel Draw { get; set; }
    private WishingTreeDataModel Tree { get; set; }
    private WishingDataModel WishData { get; set; }
    //public BuildingData WishBuilding;
    #endregion

    #region 构造函数
    public AspirationFrameCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingInfoItem.EVENT_TYPE, OnBtnShowMsgEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingInfoWillItemBtn.EVENT_TYPE, OnInfoTrueItemBtnEvent);
        EventDispatcher.Instance.AddEventListener(ExData64InitEvent.EVENT_TYPE, OnExDataInit64Event);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingBtnWishingBag.EVENT_TYPE, OnBtnAspritionBag);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingBtnTreeList.EVENT_TYPE, OnBtnTreeListEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingBtnBuyAddOrReduce.EVENT_TYPE, OnBtnBuyAddOrReduceEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingGoodsItemBuy.EVENT_TYPE, OnBuyItemEvent);
        //EventDispatcher.Instance.AddEventListener(UIEvent_WishingGetDrawResult.EVENT_TYPE, GetDrawResult);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingItemOperation.EVENT_TYPE, OnBagTargetOperaEvent);
        // EventDispatcher.Instance.AddEventListener(UIEvent_WishingTenDrawStopEvent.EVENT_TYPE, TenDrawStopEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishingOperation.EVENT_TYPE, OnAspriationOperationEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_WishShowDrawGetEvent.EVENT_TYPE, ShowPaintGet);
    }
    #endregion

    #region 固有函数
    public void CleanUp()
    {
        WishData = new WishingDataModel();
        Draw = new WishingDrawDataModel();
        Tree = new WishingTreeDataModel();
        m_dicNum_Level = new Dictionary<int, int>();
        m_lockList = new List<int>();
        var _tbClientConfig = Table.GetClientConfig(213);
        var _tbItemBase = Table.GetItemBase(int.Parse(_tbClientConfig.Value));
        Draw.OneIconItem.ItemId = _tbItemBase.Id;

        _tbClientConfig = Table.GetClientConfig(214);
        Draw.OneMoney = int.Parse(_tbClientConfig.Value);

        _tbClientConfig = Table.GetClientConfig(215);
        _tbItemBase = Table.GetItemBase(int.Parse(_tbClientConfig.Value));
        Draw.TenIconItem.ItemId = _tbItemBase.Id;

        _tbClientConfig = Table.GetClientConfig(216);
        Draw.TenMoney = int.Parse(_tbClientConfig.Value);

        var _count = 0;
        for (var i = 223; i <= 230; i++)
        {
            _tbClientConfig = Table.GetClientConfig(i);
            var _itemid = int.Parse(_tbClientConfig.Value);
            Draw.WillGetItem[_count].ItemId = _itemid;
            var _tbItembase = Table.GetItemBase(_itemid);
            Draw.WillGetItem[_count].IconId = _tbItembase.Icon;
            _count++;
        }

        Table.ForeachBuilding(tbBuilding =>
        {
            if (tbBuilding.Type == (int)BuildingType.WishingPool)
            {
                var _tableService = Table.GetBuildingService(tbBuilding.ServiceId);
                if (null == _tableService)
                {
                    return true;
                }
                var _max = _tableService.Param[1];
                if (m_dicNum_Level.ContainsKey(_max))
                {
                    m_dicNum_Level[_max] = Math.Min(m_dicNum_Level[_max], tbBuilding.Level);
                }
                else
                {
                    m_dicNum_Level.Add(_max, tbBuilding.Level);
                }
            }

            return true;
        });

        for (var i = 0; i < Draw.WillGetItem.Count; i++)
        {
            var _level = -1;
            {
                // foreach(var VARIABLE in dicNum_Level)
                var __enumerator2 = (m_dicNum_Level).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var _VARIABLE = __enumerator2.Current;
                    {
                        if (i < _VARIABLE.Key)
                        {
                            if (-1 == _level)
                            {
                                _level = _VARIABLE.Value;
                            }
                            else
                            {
                                _level = Math.Min(_level, _VARIABLE.Value);
                            }
                        }
                    }
                }
            }
            Draw.WillGetItem[i].Name = string.Format(GameUtils.GetDictionaryText(300403), _level);
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        var _args = data as WishingArguments;
        if (_args != null)
        {
            WishData.ShowWitchUI = 0;
            if (_args.Tab == 1) //xu yuan shu
            {
                WishData.ShowWitchUI = 2;
                Tree.ShowTreesUI = 1;
            }
            else if (_args.Tab == 2) //xu yuan chi
            {
                WishData.ShowWitchUI = 1;
            }
            //WishBuilding = args.BuildingData;
        }
        InitUI();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        switch (name)
        {
            case "WishData":
                {
                    return WishData;
                }
                break;
            case "Draw":
                {
                    return Draw;
                }
                break;
            case "Tree":
                {
                    return Tree;
                }
                break;
        }
        return null;
    }

    public void Close()
    {
        if (m_refItemTimeCor != null)
        {
            NetManager.Instance.StopCoroutine(m_refItemTimeCor);
            m_refItemTimeCor = null;
        }
        if (m_refItemTimeCor2 != null)
        {
            NetManager.Instance.StopCoroutine(m_refItemTimeCor2);
            m_refItemTimeCor2 = null;
        }
        //if (refItemTimeCor3 != null)
        //{
        //    NetManager.Instance.StopCoroutine(refItemTimeCor3);
        //    refItemTimeCor3 = null;
        //}
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "InitWishingBag")
        {
            InitAspriationBag(param[0] as BagBaseData);
        }
        else if (name == "UpdateWishingBag")
        {
            UpdateAspriationBag((int)param[0], param[1] as ItemsChangeData);
        }
        else if (name.Equals("GetNextFreeTime"))
        {
            return PlayerDataManager.Instance.GetExData64(1);
        }

        return null;
    }

    public void OnShow()
    {
    }
    #endregion
    #region 普通函数
    //判断显示当前或者历史愿望
    private void BtnShowPage(int index)
    {
        Tree.TabPage = index;
        switch (index)
        {
            case 0:
                {
                    Tree.ShowMyGoodsUI = 0;
                    Tree.ShowTreesUI = 1;
                    //CanRefleshMsg(RefleshType.ShopGoods, TreeId);
                }
                break;
            case 1:
                {
                    Tree.ShowMyGoodsUI = 1;
                    Tree.ShowTreesUI = 0;
                    CanUpdateMsg(RefleshType.MyNowGoods, 0);
                }
                break;
            case 2:
                {
                    Tree.ShowMyGoodsUI = 1;
                    Tree.ShowTreesUI = 0;
                    CanUpdateMsg(RefleshType.MyHistoryGoods, 0);
                }
                break;
        }
    }



    #region 主界面

    //根据时间戳控制数据是否重新请求
    private void CanUpdateMsg(RefleshType type, int index)
    {
        switch (type)
        {
            case RefleshType.ShopGoods:
                {
                    if (index >= 0 && index < Tree.TreeIcon.Count)
                    {
                        GetGroupStoreItems(index);
                    }
                }
                break;
            case RefleshType.MyNowGoods:
                {
                    if (m_RefleshTime[1] <= Game.Instance.ServerTime)
                    {
                        GetMyNowThings();
                    }
                }
                break;
            case RefleshType.MyHistoryGoods:
                {
                    if (m_RefleshTime[2] <= Game.Instance.ServerTime)
                    {
                        GetMyHistoryThings();
                    }
                }
                break;
            case RefleshType.TimerOverShopGoods:
                {
                    if (index >= 0 && index < Tree.TreeIcon.Count)
                    {
                        if (m_RefleshTime[3] <= Game.Instance.ServerTime)
                        {
                            GetGroupStoreItems(index);
                        }
                    }
                }
                break;
        }
    }

    //页面初始化
    private void InitUI()
    {
        m_RefleshTime.Clear();
        for (var i = 0; i < (int)RefleshType.REFLESH_COUNT; i++)
        {
            m_RefleshTime.Add(Game.Instance.ServerTime);
        }
        WishData.ShowWishingBag = 0;
        WishData.ShowLog = 0;
        Tree.ShowBuyUI = 0;
        //Tree.ShowTreesUI = 0;
        Tree.ShowMyGoodsUI = 0;
        Tree.TabPage = 0;

        var _time = PlayerDataManager.Instance.GetExData64(1);
        UpdateFreeTime(_time);
        //var tbExdata = Table.GetExdata(251);
        //Draw.FreeTotalCount = tbExdata.InitValue;

        var _vipLevel = PlayerDataManager.Instance.GetRes((int)eResourcesType.VipLevel);
        Table.ForeachVIP((tb) =>
        {
            if (_vipLevel < tb.Id)
            {
                return false;
            }
            m_reduceCount = Math.Max(tb.WishPoolFilterNum, m_reduceCount);
            return true;
        });

        m_lockList.Clear();
        for (var i = 0; i < Draw.WillGetItem.Count; i++)
        {
            var _item = Draw.WillGetItem[i];
            if (i < m_canDrawTypeCount)
            {
                _item.Lock = 0;
                _item.IsShowName = 0;
            }
            else
            {
                _item.Lock = 1;
                _item.IsShowName = 1;
                m_lockList.Add(i);
            }
        }
        Draw.ReduceCount = m_reduceCount;
        Draw.IsShowReduceStr = (m_reduceCount > 0);
        //if (tbBuildServer.Param[2] > 0)
        //{
        //    Draw.ShowGetRidOff = 1;
        //}
        //else
        //{
        //    Draw.ShowGetRidOff = 0;
        //}

        var level = PlayerDataManager.Instance.GetRes((int)eResourcesType.LevelRes);

        //许愿树商品开启
        for (var i = 0; i < Tree.TreeIcon.Count; i++)
        {
            var _limitLevel = Table.GetClientConfig(420 + i).ToInt();
            if (level >= _limitLevel)
            {
                Tree.TreeIcon[i].Lock = 0;
            }
            else
            {
                Tree.TreeIcon[i].Lock = 1;
                Tree.TreeIcon[i].Name = String.Format(GameUtils.GetDictionaryText(300410), _limitLevel);
            }
        }

    }

    //返回按钮
    private void BackBtn()
    {
        if (WishData.ShowWitchUI == 0)
        {
            var e = new Close_UI_Event(UIConfig.WishingUI);
            EventDispatcher.Instance.DispatchEvent(e);
            //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.CityUI));
        }
        else
        {
            WishData.ShowWishingBag = 0;
            if (Tree.ShowMyGoodsUI == 1)
            {
                Tree.ShowMyGoodsUI = 0;
                Tree.ShowTreesUI = 1;
                return;
            }

            WishData.ShowWitchUI = 0;
        }
    }

    //返回家园界面
    //     public void BtnBackCity()
    //     {
    //         var e = new Close_UI_Event(UIConfig.WishingUI);
    //         EventDispatcher.Instance.DispatchEvent(e);
    //         EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.CityUI));
    //     }

    //许愿树界面
    private void BtnAspriationTree()
    {
        //我的愿望包裹未初始化
        Tree.ShowMyGoodsUI = 0;
        WishData.ShowWitchUI = 2;
        Tree.ShowTreesUI = 1;
        //InitAllGroupShopItems(false);
    }

    //许愿池界面
    private void BtnAspriationWishing()
    {
        WishData.ShowWitchUI = 1;
        Draw.UIGetOneShow = 0;
        Draw.UIPetShow = 0;
        Draw.UIGetShow = 0;
    }




    #endregion

    //     public void BtnChatMessage(IEvent ievent)
    //     {
    //         Event_ReceiveChatMessage e =ievent as Event_ReceiveChatMessage;
    //         if (e.ChatType == eChatChannel.WishingDraw || e.ChatType == eChatChannel.WishingGroup)
    //         {
    //             ChatMessageDataModel i = new ChatMessageDataModel();
    //             i.CharId = e.CharacterId;
    //             i.Name = e.CharacterName;
    //             i.Type = (int)e.ChatType;
    //             i.Content = e.Content;
    //             WishData.LogList.Add(i);
    // //             if (WishData.LogList.Count > 100)
    // //             {
    // //                 WishData.LogList.Remove(WishData.LogList[0]);
    // //             }
    //             if (WishData.LogList.Count > tbpanelCount.Value.ToInt() + 20)
    //             {
    //                 List<ChatMessageDataModel> loglist = new List<ChatMessageDataModel>();
    //                 loglist.AddRange(WishData.LogList);
    //                 loglist.RemoveRange(0, 20);
    //                 WishData.LogList = new ObservableCollection<ChatMessageDataModel>(loglist);
    //             }
    //             
    //             WishData.LogOne = i;
    //         }
    //     }

    #region 许愿池





    //显示物品信息
    private void ShowMsg(BagItemDataModel item)
    {
        if (item.ItemId != -1)
        {
            GameUtils.ShowItemDataTip(item, eEquipBtnShow.None);
        }
    }

    //------------------------按钮------------------------------
    //抽奖结果界面隐藏
    private void BtnOK()
    {
        Draw.UIGetShow = 0;

        var _eResult = new UIEvent_WishPlayFlyAnim();
        _eResult.AnimIndex = 1;
        _eResult.ItemIds = new List<int>();
        if (m_isOneDraw)
        {
            _eResult.ItemIds.Add(Draw.OneGet.ItemId);
        }
        else
        {
            for (var i = 0; i < Draw.TenGetItem.Count; i++)
            {
                _eResult.ItemIds.Add(Draw.TenGetItem[i].ItemId);
            }
        }
        EventDispatcher.Instance.DispatchEvent(_eResult);
    }

    //单抽显示
    private void ButtonOnePaint()
    {
        //Draw.FreeNowCount = PlayerDataManager.Instance.GetExData(251);
        //判断免费抽奖时间是否到了
        //if (Draw.FreeNowCount > 0 && Draw.UIFreeDraw == 1)
        if (PlayerDataManager.Instance.NoticeData.WishDrawFree)
        {
            NetManager.Instance.StartCoroutine(PurchaseOne(100));
        }
        else
        {
            var _Moneytype = int.Parse(Table.GetClientConfig(213).Value);
            //判断金币是否够了
            if (_Moneytype == 2)
            {
                if (Draw.OneMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                    PlayerDataManager.Instance.ShowItemInfoGet((int)eResourcesType.GoldRes);
                    return;
                }
            }
            //判断钻石是否够了
            else if (_Moneytype == 3)
            {
                if (Draw.OneMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                    PlayerDataManager.Instance.ShowItemInfoGet((int)eResourcesType.DiamondRes);
                    return;
                }
            }

            NetManager.Instance.StartCoroutine(PurchaseOne(101));
        }
    }

    private IEnumerator PurchaseOne(int free)
    {
        using (new BlockingLayerHelper(0))
        {
            var _array = new Int32Array();
            if (free == 100)
            {
                _array.Items.Add(100);
            }
            else
            {
                _array.Items.Add(101);
            }
            var _value = 0;
            var _WishDataDrawWillGetItemCount2 = Draw.WillGetItem.Count;
            for (var i = _WishDataDrawWillGetItemCount2 - 1; i >= 0; i--)
            {
                if (Draw.WillGetItem[i].Lock == 1)
                {
                    _value = BitFlag.IntSetFlag(_value, i);
                }
            }
            _array.Items.Add(_value);
            m_freeType = free;
            var _msg = NetManager.Instance.DrawWishItem(_array);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    if (m_freeType == 100)
                    {
                        PlatformHelper.Event("city", "WishingPoolOne", 0);
                    }
                    else
                    {
                        PlatformHelper.Event("city", "WishingPoolOne", 1);
                    }
                    Draw.UIGetShow = 0;
                    var _result = _msg.Response;
                    GetPaintResult(_result.Items, _result.Data64[0]);
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //10抽显示
    private void BtnTenPaint()
    {
        var _Moneytype = int.Parse(Table.GetClientConfig(215).Value);
        if (_Moneytype == 2)
        {
            //判断金币是否够了
            if (Draw.TenMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                PlayerDataManager.Instance.ShowItemInfoGet((int)eResourcesType.GoldRes);
                return;
            }
        }
        //判断钻石是否够了
        else if (_Moneytype == 3)
        {
            if (Draw.TenMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                PlayerDataManager.Instance.ShowItemInfoGet((int)eResourcesType.DiamondRes);
                return;
            }
        }
        NetManager.Instance.StartCoroutine(PurchaseTen());
    }

    private IEnumerator PurchaseTen()
    {
        using (new BlockingLayerHelper(0))
        {
            var _array = new Int32Array();
            _array.Items.Add(102);
            var _value = 0;
            var _WishDataDrawWillGetItemCount2 = Draw.WillGetItem.Count;
            for (var i = _WishDataDrawWillGetItemCount2 - 1; i >= 0; i--)
            {
                if (Draw.WillGetItem[i].Lock == 1)
                {
                    _value = BitFlag.IntSetFlag(_value, i);
                }
            }
            _array.Items.Add(_value);
            var _msg = NetManager.Instance.DrawWishItem(_array);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    PlatformHelper.Event("city", "WishingPoolTen");
                    Draw.UIGetShow = 0;
                    var _result = _msg.Response;
                    GetPaintResult(_result.Items, _result.Data64[0]);
                }
                else
                {
                    if (_msg.ErrorCode == (int)ErrorCodes.Unknow)
                    {
                        var _e = new ShowUIHintBoard(200000001);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                    else if (_msg.ErrorCode == (int)ErrorCodes.Error_ItemNoInBag_All)
                    {
                        var _e = new ShowUIHintBoard(300408);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                    else if (_msg.ErrorCode == (int)ErrorCodes.Error_SeedTimeNotOver)
                    {
                        var _e = new ShowUIHintBoard(220900);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                    else if (_msg.ErrorCode == (int)ErrorCodes.ItemNotEnough)
                    {
                        var _e = new ShowUIHintBoard(200000005);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    private void ShowPaintGet(IEvent ievent)
    {
        Draw.UIGetShow = 1;
        if (m_isOneDraw)
        {
            Draw.UIGetOneShow = 1;
        }
        else
        {
            Draw.UIGetOneShow = 10;
        }
    }

    //SC 抽奖结果处理
    private void GetPaintResult(List<ItemBaseData> draw, long time)
    {
        if (draw.Count == 0)
        {
        }
        else if (draw.Count == 1)
        {
            Draw.OneGet.ItemId = draw[0].ItemId;
            Draw.OneGet.Count = draw[0].Count;
            Draw.OneGet.Index = draw[0].Index;
            Draw.OneGet.Exdata.InstallData(draw[0].Exdata);
            if (m_freeType == 100)
            {
                //刷新建筑页面
                //  				if (WishBuilding != null)
                //  				{
                PlayerDataManager.Instance.SetExData64(1, time);
                //  					var ee = new UIEvent_CityWishReflesh();
                //  					ee.MyTime = time;
                //  					ee.BuildData = WishBuilding;
                //  					EventDispatcher.Instance.DispatchEvent(ee);
                //}
                UpdateFreeTime(time);

                //刷新推送
                EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(8, 0));
            }
            m_isOneDraw = true;
            var _eResult = new UIEvent_WishPlayFlyAnim();
            _eResult.AnimIndex = 0;
            _eResult.DrawType = 1;
            _eResult.Exp = int.Parse(Table.GetClientConfig(312).Value);
            _eResult.List = new List<int>();
            _eResult.ItemIds = new List<int>();
            for (var i = 0; i < Draw.WillGetItem.Count; i++)
            {
                if (!m_lockList.Contains(i))
                {
                    _eResult.List.Add(i);
                    _eResult.ItemIds.Add(Draw.WillGetItem[i].ItemId);
                }
            }
            EventDispatcher.Instance.DispatchEvent(_eResult);
        }
        else if (draw.Count <= 10 && draw.Count > 1)
        {
            var _drawCount3 = draw.Count;
            for (var i = 0; i < Draw.TenGetItem.Count; i++)
            {
                Draw.TenGetItem[i].ItemId = -1;
                Draw.TenGetItem[i].Count = 0;
            }

            for (var i = 0; i < Draw.TenNameList.Count; i++)
            {
                Draw.TenNameList[i] = "";
            }

            for (var i = 0; i < _drawCount3; i++)
            {
                Draw.TenGetItem[i].ItemId = draw[i].ItemId;
                Draw.TenGetItem[i].Count = draw[i].Count;
                Draw.TenGetItem[i].Index = draw[i].Index;
                Draw.TenGetItem[i].Exdata.InstallData(draw[i].Exdata);
                var _tbItem = Table.GetItemBase(draw[i].ItemId);
                Draw.TenNameList[i] = _tbItem.Name;
            }
            // Draw.UIGetShow = 1;
            m_isOneDraw = false;
            var _eResult = new UIEvent_WishPlayFlyAnim();
            _eResult.AnimIndex = 0;
            _eResult.DrawType = 10;
            _eResult.Exp = int.Parse(Table.GetClientConfig(312).Value) * 10;
            _eResult.List = new List<int>();
            _eResult.ItemIds = new List<int>();
            for (var i = 0; i < Draw.WillGetItem.Count; i++)
            {
                if (!m_lockList.Contains(i))
                {
                    _eResult.List.Add(i);
                    _eResult.ItemIds.Add(Draw.WillGetItem[i].ItemId);
                }
            }
            EventDispatcher.Instance.DispatchEvent(_eResult);
        }
    }

    //关闭宠物中奖界面
    private void BtnPetOk()
    {
        Draw.UIPetShow = 0;
    }


    private int PressIndex = -1;

  
    //临时包裹index
    private IEnumerator BtnTargetClick(int index)
    {
        m_pressLongTime = false;
        var _pressCd = 0.8f;
        while (true)
        {
            yield return new WaitForSeconds(_pressCd);
            break;
        }
        m_pressLongTime = true;
    }

    //拾取所有背包物品
    private void BtnPickAllTarget()
    {
        NetManager.Instance.StartCoroutine(AspriationPoolDepotTakeOut(-1));
    }

    private IEnumerator AspriationPoolDepotTakeOut(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.WishingPoolDepotTakeOut(index);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    Logger.Info("----------------------WishingPoolDepotTakeOut -----ok");
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //背包整理
    private void BtnWishingArrange()
    {
        NetManager.Instance.StartCoroutine(BagArrange((int)eBagType.WishingPool));
    }

    private IEnumerator BagArrange(int bagid)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.SortBag(bagid);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    var _bag = _msg.Response;
                    InitAspriationBag(_bag);
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    #endregion

    #region 许愿树

 
    //许愿树index商品信息请求
    private void GetGroupStoreItems(int GoodsItemsId)
    {
        NetManager.Instance.StartCoroutine(GroupStoreItemsCoroutine(GoodsItemsId));
    }

    private IEnumerator GroupStoreItemsCoroutine(int GoodsItemsId)
    {
        using (new BlockingLayerHelper(0))
        {
            var _arraylist = new Int32Array();
            _arraylist.Items.Add(GoodsItemsId);
            var _msg = NetManager.Instance.ApplyGroupShopItems(_arraylist);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    if (_msg.Response.Lists.Count >= 1)
                    {
                        SetShopTargets(_msg.Response.Lists[0], 0);
                    }
                    //我的愿望刷新时间
                    if (m_refItemTimeCor != null)
                    {
                        NetManager.Instance.StopCoroutine(m_refItemTimeCor);
                    }
                    Tree.TabPage = 0;
                    m_refItemTimeCor = NetManager.Instance.StartCoroutine(SetTeamShopTime());
                    m_RefleshTime[0] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                    m_RefleshTime[3] = m_RefleshTime[0];
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //type 0 加入到商品列表中，1加入到我的愿望列表中 
    private void SetShopTargets(GroupShopItemList items, int type)
    {
        //if (type == 0)
        //{
        //    Tree.Goods.Clear();
        //}
        //else if (type == 1)
        //{
        //    Tree.MygoodsNow.Clear();
        //}
        var _goodsList = new List<WishingGoodsDataModel>();
        var _MygoodsNowList = new List<WishingGoodsDataModel>();
        for (var i = 0; i < items.Items.Count; i++)
        {
            var _response = items.Items[i];
            var _vartime = Extension.FromServerBinary(_response.OverTime);
            var _item = new WishingGoodsDataModel();
            _item.Guid = _response.Guid;
            _item.GroupShopId = _response.GroupShopId;
            _item.overTime = Extension.FromServerBinary(_response.OverTime);
            _item.DateTime = GameUtils.GetTimeDiffString(_vartime);
            _item.bagItem.ItemId = _response.ItemData.ItemId;
            _item.bagItem.Index = _response.ItemData.Index;
            _item.bagItem.Exdata.InstallData(_response.ItemData.Exdata);
            _item.bagItem.Count = _response.ItemData.Count;
            PlayerDataManager.Instance.RefreshEquipBagStatus(_item.bagItem);
            _item.MinCount = _response.CharacterCount;
            _item.LuckNumber = _response.SelfCount;
            var _tbGroupShop = Table.GetGroupShop(_item.GroupShopId);
            _item.MaxCount = _tbGroupShop.LuckNumber;
            _item.CanBuyCount = _tbGroupShop.BuyLimit;
            //{0}服务器的{1}以{2}个幸运签实现愿望！
            if (_response.LuckyCount > 0)
            {
                var _formart = GameUtils.GetDictionaryText(300407);
                _item.GotInfo = string.Format(_formart, _response.LuckyServer, _response.LuckyPlayer,
                    _response.LuckyCount);
            }
            else
            {
                _item.GotInfo = GameUtils.GetDictionaryText(100001189);
            }


            if ((int)eGroupShopItemState.OnSell == _response.State)
            {
                _item.IsShowCanBuy = 0; //item.LuckNumber>=item.CanBuyCount ? 1:0;
            }
            else if ((int)eGroupShopItemState.WaitResult == _response.State)
            {
                if (_vartime > Game.Instance.ServerTime)
                {
                    _item.IsShowCanBuy = 1;
                    //"即将揭晓"
                    _item.DateTime = GameUtils.GetDictionaryText(300405);
                }
                else
                {
                    //"已揭晓"
                    _item.IsShowCanBuy = 2;
                    _item.DateTime = GameUtils.GetDictionaryText(300406);
                }
            }


            if (_item.MaxCount > 0)
            {
                _item.Percent = (float)_item.MinCount / _item.MaxCount;
            }
            if (type == 0)
            {
                if ((int)eGroupShopItemState.WaitResult == _response.State)
                {
                    _goodsList.Insert(0, _item);
                    //Tree.Goods.Insert(0,item);
                }
                else
                {
                    _goodsList.Add(_item);
                    // Tree.Goods.Add(item);
                }
            }
            else if (type == 1)
            {
                if (_response.CharacterCount > 0)
                {
                    if ((int)eGroupShopItemState.WaitResult == _response.State)
                    {
                        _goodsList.Insert(0, _item);
                        //Tree.Goods.Insert(0, item);
                        var ii = new WishingGoodsDataModel(_item);
                        ii.bagItem.Clone(_item.bagItem);
                        _MygoodsNowList.Insert(0, _item);
                        //Tree.MygoodsNow.Insert(0, item);
                    }
                    else
                    {
                        var ii = new WishingGoodsDataModel(_item);
                        ii.bagItem.Clone(_item.bagItem);
                        _MygoodsNowList.Add(_item);
                        // Tree.MygoodsNow.Add(item);
                    }
                }
            }
        }
        if (type == 0)
        {
            Tree.Goods = new ObservableCollection<WishingGoodsDataModel>(_goodsList);
        }
        else if (type == 1)
        {
            Tree.MygoodsNow = new ObservableCollection<WishingGoodsDataModel>(_MygoodsNowList);
        }
    }

    ////显示关闭我的愿望界面
    //public void BtnMyWish()
    //{
    //    Tree.ShowMyGoodsUI = Tree.ShowMyGoodsUI == 0 ? 1 : 0;
    //    Tree.ShowTreesUI = Tree.ShowMyGoodsUI == 1 ? 0 : 1;
    //    ShowGoods(0);
    //}
    //关闭购买界面
    private void BtnShutBuy()
    {
        Tree.ShowBuyUI = 0;
    }



    //购买按下时间长短
    private IEnumerator ButtonResOnClick(int type)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (!EstimateCanBuyCount(type))
            {
                NetManager.Instance.StopCoroutine(m_buyButtonPress);
                m_buyButtonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd * 0.8f;
            }
        }
        yield break;
    }

    //type = 0 ,减少，type==1  增加。
    private bool EstimateCanBuyCount(int type)
    {
        var _leftCount = 0;
        if (type == 0)
        {
            if (Tree.BuyCount > 1)
            {
                Tree.BuyCount--;
                Tree.TotalConsume = Tree.OnePrice * Tree.BuyCount;
                return true;
            }
            return false;
        }
        if (type == 1)
        {
            var _count = PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes) / Tree.OnePrice;
            if (Tree.BuyCount + Tree.BuyGoods.LuckNumber >= Tree.CanBuyCount)
            {
                String str = GameUtils.GetDictionaryText(100001186);
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100001186));
                return false;
            }
            if (Tree.BuyCount < Tree.LeftCount && Tree.BuyCount < _count)
            {
                Tree.BuyCount++;
                Tree.TotalConsume = Tree.OnePrice * Tree.BuyCount;
                return true;
            }
            return false;
        }

        return false;
    }

    //购买商品
    private void BtnPurchaseOk()
    {
        if (Tree.BuyCount == 0)
        {
            return;
        }
        NetManager.Instance.StartCoroutine(BuyTeamShopItem(Tree.BuyGoods));
    }

    private IEnumerator BuyTeamShopItem(WishingGoodsDataModel item) //long guid, int gropId, int count)
    {
        using (new BlockingLayerHelper(0))
        {

            if (item.CanBuyCount <= item.LuckNumber)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100001187));
                yield break;
            }
            var _msg = NetManager.Instance.BuyGroupShopItem(item.Guid, item.GroupShopId, Tree.BuyCount);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    Logger.Info("Buy OK");
                    SetMyNowThingCount(item, _msg.Response);
                    Tree.ShowBuyUI = 0;
                    var _ee = new UIEvent_WishPlayFlyAnim();
                    _ee.AnimIndex = 2;
                    _ee.DrawType = 0;
                    _ee.Exp = int.Parse(Table.GetClientConfig(313).Value) * Tree.BuyCount;
                    EventDispatcher.Instance.DispatchEvent(_ee);
                }
                else
                {
                    if (_msg.ErrorCode == (int)ErrorCodes.Error_GroupShopCountNotEnough)
                    {
                        var _e = new ShowUIHintBoard(220971);
                        EventDispatcher.Instance.DispatchEvent(_e);
                        //SetMyNowGoodCount(item, msg.Response);
                    }
                    else
                    {
                        //UIManager.Instance.ShowNetError(_msg.ErrorCode);
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                        PlayerDataManager.Instance.ShowItemInfoGet((int)eResourcesType.DiamondRes);
                    }
                    if (Tree.TabPage == 0)
                    {
                        //Tree.ShowBuyUI = 0;
                        GetGroupStoreItems(m_treeId);
                    }
                    if (Tree.TabPage == 1)
                    {
                        //Tree.ShowBuyUI = 0;
                        GetMyNowThings();
                    }
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //冲刷新
    private void SetMyNowThingCount(WishingGoodsDataModel item, int count)
    {
        item.LuckNumber = count;
        item.MinCount = count;
        if (item.MaxCount > 0)
        {
            item.Percent = (float)item.MinCount / item.MaxCount;
        }
        if (item.MinCount == item.MaxCount)
        {
            item.overTime = Game.Instance.ServerTime.AddMinutes(m_tbGetTime.Value.ToInt());
            //"即将揭晓"
            item.DateTime = GameUtils.GetDictionaryText(300405);
            item.IsShowCanBuy = 1;
        }
        var exist = false;
        for (var i = 0; i < Tree.MygoodsNow.Count; i++)
        {
            var _ii = Tree.MygoodsNow[i];
            if (_ii.Guid == item.Guid)
            {
                exist = true;
                break;
            }
        }
        if (!exist)
        {
            var _ii = new WishingGoodsDataModel(item);
            _ii.bagItem.Clone(item.bagItem);
            Tree.MygoodsNow.Insert(0, _ii);
        }
        if (item.MinCount == item.MaxCount)
        {
            if (Tree.TabPage == 0)
            {
                GetGroupStoreItems(m_treeId);
            }
            if (Tree.TabPage == 1)
            {
                GetMyNowThings();
            }
        }
    }

    //显示隐藏背包页面
    private void BtnReceiveBag()
    {
        WishData.ShowWishingBag = WishData.ShowWishingBag == 1 ? 0 : 1;
    }

    //显示许愿树log
    private void BtnDisplayLog()
    {
        WishData.ShowLog = 1;
    }

    //关闭许愿树log
    private void BtnShutLog()
    {
        WishData.ShowLog = 0;
    }




    //设置购买页面
    private void SetBuyTarget(WishingGoodsDataModel item)
    {
        Tree.BuyGoods = item;
        var _tbGroupShop = Table.GetGroupShop(Tree.BuyGoods.GroupShopId);
        Tree.OnePrice = _tbGroupShop.SaleCount;
        Tree.MoneyType = _tbGroupShop.SaleType;
        Tree.LeftCount = Tree.BuyGoods.MaxCount - Tree.BuyGoods.MinCount;
        Tree.BuyCount = 1;
        Tree.TotalConsume = Tree.BuyCount * Tree.OnePrice;
    }

    //关闭购买界面
    private void BtnShutMyBuy()
    {
        Tree.ShowBuyUI = 0;
    }

    //获取现在我的愿望
    private void GetMyNowThings()
    {
        NetManager.Instance.StartCoroutine(MyNowThingsCor());
    }

    private IEnumerator MyNowThingsCor()
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.GetBuyedGroupShopItems(0);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    SetShopTargets(_msg.Response.Lists[0], 1);
                    //当前愿望刷新时间
                    if (m_refItemTimeCor2 != null)
                    {
                        NetManager.Instance.StopCoroutine(m_refItemTimeCor2);
                    }
                    m_refItemTimeCor2 = NetManager.Instance.StartCoroutine(SetMyPersentGoodsTime());
                    m_RefleshTime[1] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //获取历史商品列表
    private void GetMyHistoryThings()
    {
        NetManager.Instance.StartCoroutine(GetTeamShopHistory());
    }

    private IEnumerator GetTeamShopHistory()
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.GetGroupShopHistory(0);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    Tree.MyGoodsHistory.Clear();
                    var _varlist = new List<WishingGoodsDataModel>();
                    for (int i = 0, imax = _msg.Response.Lists[0].Items.Count; i < imax; i++)
                    {
                        var _response = _msg.Response.Lists[0].Items[i];
                        var _item = new WishingGoodsDataModel();
                        _item.Guid = _response.Guid;
                        _item.GroupShopId = _response.GroupShopId;
                        _item.overTime = Extension.FromServerBinary(_response.OverTime);
                        _item.IsShowCanBuy = 2;
                        _item.DateTime = GameUtils.GetDictionaryText(300406);
                        _item.bagItem.ItemId = _response.ItemData.ItemId;
                        _item.bagItem.Index = _response.ItemData.Index;
                        _item.bagItem.Exdata.InstallData(_response.ItemData.Exdata);
                        _item.bagItem.Count = _response.ItemData.Count;
                        PlayerDataManager.Instance.RefreshEquipBagStatus(_item.bagItem);
                        _item.LuckNumber = _response.SelfCount;
                        var _tbGroupShop = Table.GetGroupShop(_item.GroupShopId);
                        _item.MaxCount = _tbGroupShop.LuckNumber;
                        _item.MinCount = _response.CharacterCount;
                        //{0}服务器的{1}以{2}个幸运签实现愿望！

                        if (_response.LuckyCount > 0)
                        {
                            var _formart = GameUtils.GetDictionaryText(300407);
                            _item.GotInfo = string.Format(_formart, _response.LuckyServer, _response.LuckyPlayer,
                                _response.LuckyCount);
                        }
                        else
                        {
                            _item.GotInfo = GameUtils.GetDictionaryText(100001189);
                        }

                        if (_item.MaxCount > 0)
                        {
                            _item.Percent = (float)_item.MinCount / _item.MaxCount;
                        }
                        if (string.Equals(_response.LuckyPlayer, PlayerDataManager.Instance.GetName()))
                        {
                            _item.IsMyGet = 1;
                        }
                        else
                        {
                            _item.IsMyGet = 0;
                        }
                        _varlist.Add(_item);
                    }
                    Tree.MyGoodsHistory = new ObservableCollection<WishingGoodsDataModel>(_varlist);

                    ////历史愿望刷新时间
                    //if (refItemTimeCor3 == null)
                    //{
                    //    refItemTimeCor3 = NetManager.Instance.StartCoroutine(SetMyHistoryGoodsTime());
                    //}
                    m_RefleshTime[2] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                }
            }
            else
            {
                var _e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    //我的当前愿望刷新时间
    private IEnumerator SetMyPersentGoodsTime()
    {
        while (Tree.MygoodsNow.Count > 0)
        {
            for (var i = 0; i < Tree.MygoodsNow.Count; i++)
            {
                var _item = Tree.MygoodsNow[i];
                if (_item.IsShowCanBuy == 1)
                {
                    if (_item.overTime > Game.Instance.ServerTime)
                    {
                        _item.WillGetTime = GameUtils.GetTimeDiffString(_item.overTime);
                    }
                    else
                    {
                        //已揭晓
                        _item.DateTime = GameUtils.GetDictionaryText(300406);
                        _item.IsShowCanBuy = 2;
                    }
                }
                else if (_item.IsShowCanBuy == 0)
                {
                    if (_item.overTime > Game.Instance.ServerTime)
                    {
                        _item.DateTime = GameUtils.GetTimeDiffString(_item.overTime);
                    }
                    else
                    {
                        CanUpdateMsg(RefleshType.MyNowGoods, m_treeId);
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    //商品列表时间刷新
    private IEnumerator SetTeamShopTime()
    {
        while (Tree.Goods.Count > 0)
        {
            for (var i = 0; i < Tree.Goods.Count; i++)
            {
                var _item = Tree.Goods[i];
                if (_item.IsShowCanBuy == 1)
                {
                    if (_item.overTime > Game.Instance.ServerTime)
                    {
                        _item.WillGetTime = GameUtils.GetTimeDiffString(_item.overTime);
                    }
                    else
                    {
                        //已揭晓
                        _item.DateTime = GameUtils.GetDictionaryText(300406);
                        _item.IsShowCanBuy = 2;
                    }
                }
                else if (_item.IsShowCanBuy == 0)
                {
                    if (_item.overTime > Game.Instance.ServerTime)
                    {
                        _item.DateTime = GameUtils.GetTimeDiffString(_item.overTime);
                    }
                    else
                    {
                        CanUpdateMsg(RefleshType.TimerOverShopGoods, m_treeId);
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion

    #region

    //public void TenDrawStopEvent(IEvent ievent)
    //{
    //    UIEvent_WishingTenDrawStopEvent e = ievent as UIEvent_WishingTenDrawStopEvent;
    //    UIEvent_WishingTenDrawStop ee = new UIEvent_WishingTenDrawStop();
    //    int index = e.Index;
    //    int itemId = -1;
    //    ee.Index = index;

    //    if (index >= 0 && index < 10)
    //    {
    //        itemId = Draw.TenGetItem[index].ItemId;
    //        if (itemId >= 60000 && itemId < 70000)
    //        {
    //            ee.IsPet = true;
    //            ee.ItemId = itemId;
    //            Draw.PetID = itemId;
    //            Draw.UIPetShow = 1;

    //        }
    //        else
    //        {
    //            ee.IsPet = false;
    //        }
    //    }
    //    EventDispatcher.Instance.DispatchEvent(ee);
    //}

    //public void ShowOneDrawPet(int itemId)
    //{
    //    if (itemId >= 60000 && itemId < 70000)
    //    {
    //        Draw.PetID = itemId;
    //        Draw.UIPetShow = 1;
    //        UIEvent_WishingOneDrawPetShow e = new UIEvent_WishingOneDrawPetShow();
    //        e.ItemId = itemId;
    //        EventDispatcher.Instance.DispatchEvent(e);
    //    }
    //    Draw.UIGetShow = 1;
    //    Draw.UIGetOneShow = 1;
    //}

    #endregion

    #region 接口



    private void UpdateFreeTime(long time)
    {
        // 未开启
        if (!PlayerDataManager.Instance.GetFlag(152))
        {
            PlayerDataManager.Instance.NoticeData.WishDrawFree = false;
            return;
        }

        var _vartime = Extension.FromServerBinary(time);
        if (_vartime < Game.Instance.ServerTime)
        {
            PlayerDataManager.Instance.NoticeData.WishDrawFree = true;
        }
        else
        {
            PlayerDataManager.Instance.NoticeData.WishDrawFree = false;
            Draw.FreeTime = Extension.FromServerBinary(time);
            if (m_timeCoroutine != null)
            {
                NetManager.Instance.StopCoroutine(m_timeCoroutine);
            }
            m_timeCoroutine = NetManager.Instance.StartCoroutine(TimeCoroute(_vartime));
        }
    }

    private IEnumerator TimeCoroute(DateTime time)
    {
        var _second = (int)(time - Game.Instance.ServerTime).TotalSeconds;
        yield return new WaitForSeconds(_second + 1);
        PlayerDataManager.Instance.NoticeData.WishDrawFree = true;
    }

    public FrameState State { get; set; }



    //初始化背包
    private void InitAspriationBag(BagBaseData bagBase)
    {
        var _bagID = bagBase.BagId;
        if (_bagID == (int)eBagType.WishingPool)
        {
            WishData.DrawBag.Clear();
            var _tbBaseBase = Table.GetBagBase(_bagID);
            var _tbBaseBaseMaxCapacity4 = _tbBaseBase.MaxCapacity;
            WishData.MaxCapacity = _tbBaseBaseMaxCapacity4;
            for (var i = 0; i < _tbBaseBaseMaxCapacity4; i++)
            {
                var _item = new BagItemDataModel();
                _item.Index = i;
                WishData.DrawBag.Add(_item);
            }
            var _bagBaseItemsCount5 = bagBase.Items.Count;
            for (var i = 0; i < _bagBaseItemsCount5; i++)
            {
                var _draw = WishData.DrawBag[i];
                _draw.ItemId = bagBase.Items[i].ItemId;
                _draw.IsChoose = false;
                _draw.Count = bagBase.Items[i].Count;
                _draw.Exdata.InstallData(bagBase.Items[i].Exdata);
            }
            WishData.BagCount = bagBase.Items.Count;
        }
    }

    //更新背包
    private void UpdateAspriationBag(int bagid, ItemsChangeData bag)
    {
        if (bagid == (int)eBagType.WishingPool)
        {
            {
                // foreach(var item in bag.ItemsChange)
                var __enumerator3 = (bag.ItemsChange).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var _item = __enumerator3.Current;
                    {
                        var _key = _item.Key;
                        if (WishData.DrawBag.Count > _item.Value.Index)
                        {
                            var _ii = WishData.DrawBag[_item.Value.Index];
                            if (_ii.ItemId == -1)
                            {
                                WishData.BagCount++;
                            }
                            if (_item.Value.ItemId == -1)
                            {
                                WishData.BagCount--;
                            }
                        }
                        WishData.DrawBag[_key].ItemId = _item.Value.ItemId;
                        WishData.DrawBag[_key].Count = _item.Value.Count;
                        WishData.DrawBag[_key].Exdata.InstallData(_item.Value.Exdata);
                    }
                }
            }
        }
    }

    #endregion
    #endregion

    #region 事件
    //鼠标按下包裹物品
    private void OnBagTargetOperaEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingItemOperation;
        if (_e.Operation == 0) // press 
        {
            PressIndex = _e.Index;
            m_buttonPress = NetManager.Instance.StartCoroutine(BtnTargetClick(_e.Index));
        }
        else if (_e.Operation == 1) // Release
        {
            if (m_buttonPress != null)
            {
                NetManager.Instance.StopCoroutine(m_buttonPress);
                m_buttonPress = null;
                if (PressIndex != _e.Index)
                {
                    PressIndex = -1;
                    return;
                }
                if (!m_pressLongTime) //短按效果
                {
                    NetManager.Instance.StartCoroutine(AspriationPoolDepotTakeOut(_e.Index));
                }
                else
                {
                    GameUtils.ShowItemDataTip(WishData.DrawBag[_e.Index], eEquipBtnShow.Share);
                }
            }
            PressIndex = -1;
        }
    }

    //许愿树点击
    private void OnBtnTreeListEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingBtnTreeList;
        if (Tree.TreeIcon[_e.Index].Lock == 0)
        {
            CanUpdateMsg(RefleshType.ShopGoods, _e.Index);
            Tree.ShowMyGoodsUI = 1;
            Tree.ShowTreesUI = 0;
            m_treeId = _e.Index;
        }
    }

    //type = 0 单个抽奖显示
    private void OnBtnShowMsgEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingInfoItem;
        if (_e.Type == 0)
        {
            ShowMsg(Draw.OneGet);
        }
        else
        {
            ShowMsg(_e.item);
        }
    }
    private void OnExDataInit64Event(IEvent ievent)
    {
        var _time = PlayerDataManager.Instance.GetExData64(1);
        UpdateFreeTime(_time);
        var _e = new UIEvent_CityWishReflesh();
        _e.MyTime = _time;
        EventDispatcher.Instance.DispatchEvent(_e);
    }
    //显示购买页面
    private void OnBuyItemEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingGoodsItemBuy;
        SetBuyTarget(_e.item);
        Tree.ShowBuyUI = 1;
        Tree.CanBuyCount = _e.item.CanBuyCount;
    }
    //+-按钮控制
    private void OnBtnBuyAddOrReduceEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingBtnBuyAddOrReduce;
        switch (_e.Type)
        {
            case 0: // -press 
                {
                    m_buyButtonPress = NetManager.Instance.StartCoroutine(ButtonResOnClick(0));
                }
                break;
            case 1: //  +press
                {
                    m_buyButtonPress = NetManager.Instance.StartCoroutine(ButtonResOnClick(1));
                }
                break;
            case 2: // -release
                {
                    if (m_buyButtonPress != null)
                    {
                        NetManager.Instance.StopCoroutine(m_buyButtonPress);
                        m_buyButtonPress = null;
                    }
                }
                break;
            case 3:
                // +release                                                                                                                                                                                                  
                {
                    if (m_buyButtonPress != null)
                    {
                        NetManager.Instance.StopCoroutine(m_buyButtonPress);
                        m_buyButtonPress = null;
                    }
                }
                break;
            case 4: // -click
                {
                    if (Tree.BuyCount > 1)
                    {
                        Tree.BuyCount--;
                    }
                }
                break;
            case 5: // +click
                {
                    if (Tree.BuyCount >= Tree.CanBuyCount - Tree.BuyGoods.LuckNumber)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100001186));
                        return;
                    }
                    var _count = PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes) / Tree.OnePrice;
                    if (Tree.BuyCount + 1 <= _count)
                    {
                        Tree.BuyCount++;
                    }
                }
                break;
            case 6: //max 
                {
                    if (Tree.OnePrice != 0)
                    {
                        var _count = PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes) / Tree.OnePrice;
                        var _max = Tree.CanBuyCount - Tree.BuyGoods.LuckNumber;
                        Tree.BuyCount = _max;// count > max ? max  : count;                      
                    }
                }
                break;
        }
        Tree.TotalConsume = Tree.OnePrice * Tree.BuyCount;
        Tree.PriceColor = Tree.OnePrice * Tree.BuyCount > PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes) ? MColor.red : MColor.green;
    }
    //许愿树抽奖排除按钮点击
    private void OnInfoTrueItemBtnEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingInfoWillItemBtn;
        var _reduceCount = m_reduceCount;
        if (_reduceCount > 0)
        {
            if (Draw.ReduceCount > 0)
            {
                if (Draw.WillGetItem[_e.Index].Lock == 0)
                {
                    Draw.WillGetItem[_e.Index].Lock = 1;
                    m_lockList.Add(_e.Index);
                    Draw.ReduceCount--;
                }
                else
                {
                    Draw.WillGetItem[_e.Index].Lock = 0;
                    var _i = m_lockList.IndexOf(_e.Index);
                    m_lockList.RemoveAt(_i);
                    Draw.ReduceCount++;
                }
            }
            else
            {
                if (Draw.WillGetItem[_e.Index].Lock == 1)
                {
                    Draw.WillGetItem[_e.Index].Lock = 0;
                    var _i = m_lockList.IndexOf(_e.Index);
                    m_lockList.RemoveAt(_i);
                    Draw.ReduceCount++;
                }
                else
                {
                    Draw.WillGetItem[_e.Index].Lock = 1;
                    m_lockList.Add(_e.Index);
                    Draw.WillGetItem[m_lockList[0]].Lock = 0;
                    m_lockList.RemoveAt(0);
                }
            }
        }
    }
    //背包返回点击事件
    private void OnBtnAspritionBag(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingBtnWishingBag;
        if (_e.Isback == 0)
        {
            WishData.ShowWishingBag = 1;
        }
        else if (_e.Isback == 1)
        {
            WishData.ShowWishingBag = 0;
        }
    }
    //操作按钮的响应函数
    private void OnAspriationOperationEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_WishingOperation;
        switch (_e.Type)
        {
            case 0:
                BtnOK(); //抽奖结果界面隐藏
                break;
            case 1:
                ButtonOnePaint(); //单抽显示
                break;
            case 2:
                BtnTenPaint(); //10抽显示
                break;
            case 3:
                BackBtn(); //返回按钮
                break;
                //             case 4:
                //                 BtnBackCity(); //返回家园界面
                break;
            case 5:
                BtnAspriationTree(); //许愿树界面
                break;
            case 6:
                BtnAspriationWishing(); //许愿池界面
                break;
            case 7:
                BtnPetOk(); //关闭宠物中奖界面
                break;
            //case 8:
            //    BtnMyWish();  //显示关闭我的愿望界面
            //    break;
            case 9:
                BtnShutBuy(); //关闭购买界面
                break;
            case 10:
                BtnPurchaseOk(); //购买商品
                break;
            case 11:
                BtnShutMyBuy(); //关闭购买界面
                break;
            case 14:
                BtnPickAllTarget(); //拾取所有背包物品
                break;
            case 15:
                BtnWishingArrange(); //整理背包
                break;
            case 16:
                BtnReceiveBag(); //显示隐藏背包页面
                break;
            case 17:
                BtnDisplayLog(); //显示许愿树log
                break;
            case 18:
                BtnShutLog(); //关闭许愿树log
                break;
            case 19:
            case 20:
            case 21:
                BtnShowPage(_e.Type - 19); //关闭许愿树log
                break;
        }
    }
    #endregion





    private enum RefleshType
    {
        ShopGoods = 0, //许愿树商品 
        MyNowGoods = 1, //我的商品
        MyHistoryGoods = 2, // 历史商品
        TimerOverShopGoods = 3, //时间到了刷新许愿树商品，防止错误时候重复请求。
        REFLESH_COUNT = 4 //刷新商品个数
    }



}