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

public class SmithyFrameController : IControllerBase
{
    public EquipUIDataModel EquipDataModel;
    //0  添加未开始 1 打造过程中 2 打造完成 3 Lock   4 可添加 
    public SmithyFrameController()
    {
        mEquipPackController = UIManager.Instance.GetController(UIConfig.EquipPack);

        CleanUp();
        EventDispatcher.Instance.AddEventListener(CityDataInitEvent.EVENT_TYPE, OnCityDataInit); // 初始化数据
        EventDispatcher.Instance.AddEventListener(UIEvent_SmithyBtnClickedEvent.EVENT_TYPE, OnForgeBtnClicked);
        EventDispatcher.Instance.AddEventListener(UIEvent_CityUpdateBuilding.EVENT_TYPE, OnUpdateBuilding);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnRefrehEquipBagItemStatus);
        EventDispatcher.Instance.AddEventListener(RemoveEvoEquipEvent.EVENT_TYPE, OnRemoveEvoEquip);
        EventDispatcher.Instance.AddEventListener(SmithyFurnaceCellEvent.EVENT_TYPE, SmithyFurnaceCellClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_SmithTabPageEvent.EVENT_TYPE, TabPageBtn);
        EventDispatcher.Instance.AddEventListener(UIEvent_SmithItemClick.EVENT_TYPE, SmithItemClick);
    }

    public SmithyDataModel DataModel;
    public int mEquipCountNow;
    public Dictionary<int, int> dicNum_Level;
    public BuildingData mBuildingData;
    public IControllerBase mEquipPackController;
    public int mBuildingLevel;
    public Dictionary<int, bool> mLeftSideMenuStates = new Dictionary<int, bool>();
    // 当前的铸造id
    public int mCurTbForgeId = -1;
    // 当前熔炉index
    public int mCurFurnaceIndex;
    // 当前熔炉数
    public int mCurFurnaceCount;
    public Action mRefreshEvoUiAction;
    public float mSpeedUpPercent;

    private bool isShowing;

    public enum DoneBtnState
    {
        BeginForge,
        CompleteImmediately,
        FinishCasting
    }

    public enum ForgeState
    {
        NoBegin = 0,
        During = 1,
        Complete = 2,
        Lock = 3,
        CanAdd = 4
    }

    public EquipPackDataModel mEquipPackDataModel
    {
        get { return mEquipPackController.GetDataModel("") as EquipPackDataModel; }
    }

    public BuildingData BuildingData
    {
        get { return mBuildingData; }
        set
        {
            mBuildingData = value;

            var castData = DataModel.SmithyCastData;
            var tbBuilding = Table.GetBuilding(value.TypeId);
            var tbBuildService = Table.GetBuildingService(tbBuilding.ServiceId);

            var pets = mBuildingData.PetList;
            var petCount = pets.Count;
            var baseSpeed = tbBuildService.Param[0]/100;
            var petlist = new List<int>();
            var petTotleSpeed = 0;
            for (var i = 0; i < petCount; i++)
            {
                petlist.Clear();
                var petId = pets[i];
                if (petId == -1)
                {
                    continue;
                }
                petlist.Clear();
                petlist.Add(petId);
                var thisPetref = CityPetSkill.GetBSParamByIndex((BuildingType) tbBuilding.Type, tbBuildService, 1,
                    petlist);
                petTotleSpeed += thisPetref/100;
            }
            mBuildingLevel = tbBuilding.Level;
            mSpeedUpPercent = baseSpeed + petTotleSpeed;
            castData.SpeedUpPercent = (int)mSpeedUpPercent;

            RefreshUI();
        }
    }

    // 当前熔炉index
    public int CurFurnaceIndex
    {
        get { return mCurFurnaceIndex; }
        set
        {
            if (mCurFurnaceIndex == value)
            {
                return;
            }

            mCurFurnaceIndex = value;

            RefreshUI();
        }
    }

    public FrameState State { get; set; }

    #region 重载

    public void OnShow()
    {
        EventDispatcher.Instance.AddEventListener(EquipCellSelect.EVENT_TYPE, OnEquipCellSelect);
        EventDispatcher.Instance.AddEventListener(EquipCellSwap.EVENT_TYPE, OnEquipCellSwap);
        EventDispatcher.Instance.AddEventListener(SmithyCellClickedEvent.EVENT_TYPE, OnClickCastLeftSideBtn);

        EquipDataModel = UIManager.Instance.GetController(UIConfig.EquipUI).GetDataModel("") as EquipUIDataModel;

        isShowing = true;
        if (EquipDataModel != null && EquipDataModel.OperateTypes[5] && mEquipPackController != null)
        {
            mEquipPackController.RefreshData(new EquipPackArguments { RefreshForEvoEquip = true });        
        }
    }

    public void Close()
    {
        EventDispatcher.Instance.RemoveEventListener(EquipCellSelect.EVENT_TYPE, OnEquipCellSelect);
        EventDispatcher.Instance.RemoveEventListener(EquipCellSwap.EVENT_TYPE, OnEquipCellSwap);
        EventDispatcher.Instance.RemoveEventListener(SmithyCellClickedEvent.EVENT_TYPE, OnClickCastLeftSideBtn);
        ClearEvoData();
        isShowing = false;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
//         var args = data as SmithyFrameArguments;
//         if (args != null && args.BuildingData != null)
//         {
//             BuildingData = args.BuildingData;
//            RefreshCastLeftSideBtns();
//             if (args.Tab >= 0 && args.Tab < 2)
//             {
//                 DataModel.SmithyCastData.TabPage = args.Tab;
//             }
//         }
        ClearEvoData();
        var tbBuilding = Table.GetBuilding(8);
        var tbBuildService = Table.GetBuildingService(tbBuilding.ServiceId);
        var baseSpeed = tbBuildService.Param[0] / 100;

        mBuildingLevel = PlayerDataManager.Instance.GetRes((int)eResourcesType.LevelRes);; //角色等级
        mSpeedUpPercent = baseSpeed;
        var castData = DataModel.SmithyCastData;
        castData.SpeedUpPercent = (int)mSpeedUpPercent;

        RefreshUI();
        RefreshCastLeftSideBtns();

        mEquipPackController.RefreshData(new EquipPackArguments {RefreshForEvoEquip = true});
        mCurTbForgeId = -1;
        DataModel.SmithyCastData.ShowFurnacePage = 0;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "EquipPack")
        {
            return mEquipPackDataModel;
        }
        return DataModel;
    }

    public void CleanUp()
    {
        DataModel = new SmithyDataModel();
        dicNum_Level = new Dictionary<int, int>();
        //
        RefreshLeftMainBtnState(eSmithyCastType.Iron);

        var maxCount = 0;
        Table.ForeachBuilding(tbBuilding =>
        {
            if (tbBuilding.Type == (int) BuildingType.BlacksmithShop)
            {
                var tableService = Table.GetBuildingService(tbBuilding.ServiceId);
                if (null == tableService)
                {
                    return true;
                }
                var max = tableService.Param[2];
                if (max > maxCount)
                {
                    maxCount = max;
                }
                if (dicNum_Level.ContainsKey(max))
                {
                    dicNum_Level[max] = Math.Min(dicNum_Level[max], tbBuilding.Level);
                }
                else
                {
                    dicNum_Level.Add(max, tbBuilding.Level);
                }
            }

            return true;
        });

        var mList = new List<FurnaceProductionDataModel>();
        for (var i = 0; i < maxCount; i++)
        {
            var item = new FurnaceProductionDataModel();
            var level = -1;
            {
                // foreach(var VARIABLE in dicNum_Level)
                var __enumerator2 = (dicNum_Level).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var VARIABLE = __enumerator2.Current;
                    {
                        if (i < VARIABLE.Key)
                        {
                            if (-1 == level)
                            {
                                level = VARIABLE.Value;
                            }
                            else
                            {
                                level = Math.Min(level, VARIABLE.Value);
                            }
                        }
                    }
                }
            }
            item.LockStr = string.Format(GameUtils.GetDictionaryText(270240), level);
            mList.Add(item);
        }
        var products = DataModel.SmithyCastData.FurnaceProducts;
        for (var i = 0; i < products.Count; i++)
        {
            products[i] = mList[i];
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    #endregion

    #region AnalyseNotice

    //初始化建筑信息
    public void OnCityDataInit(IEvent ievent)
    {
        var buildingdData = CityManager.Instance.GetBuildingDataByType(BuildingType.BlacksmithShop);
        if (buildingdData != null)
        {
            BuildingData = buildingdData;
            AnalyseNotice();
        }
    }

    public object NoticeTrigger;

    private void AnalyseNotice()
    {
        if (NoticeTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(NoticeTrigger);
            NoticeTrigger = null;
        }
        var FurnaceProducts = DataModel.SmithyCastData.FurnaceProducts;
        var productCount = FurnaceProducts.Count;
        var minSec = -1;
        var isNotice = false;
        var index = -1;
        for (var i = 0; i < productCount; i++)
        {
            var item = FurnaceProducts[i];
            if (item.State == (int) ForgeState.Complete)
            {
                isNotice = true;
                index = i;
                break;
            }
            if (item.State == (int) ForgeState.During)
            {
                var dif = (int) (item.FinishTimer - Game.Instance.ServerTime).TotalSeconds;
                if (dif <= 0)
                {
                    isNotice = true;
                    index = i;
                    break;
                }

                if (minSec == -1 || minSec > dif)
                {
                    minSec = dif;
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.HomeSmithy = isNotice;
        if (index != -1)
        {
            var tbItemBase = Table.GetItemBase(FurnaceProducts[index].ItemId);
            PlayerDataManager.Instance.NoticeData.HomeSmithyIco = tbItemBase.Icon;
        }
        EventDispatcher.Instance.DispatchEvent(new CityBulidingNoticeRefresh(BuildingData));
        if (isNotice == false)
        {
            if (minSec != -1)
            {
                //等待scends刷新标志
                NoticeTrigger = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddSeconds(minSec + 1),
                    () => { AnalyseNotice(); });
            }
        }
    }

    #endregion

    #region 第一页

    private void RefreshUI()
    {
        var castData = DataModel.SmithyCastData;
        castData.TabPage = 0;
        // 计算下方的显示时间，以及进度条
        var isForging = BuildingData.Exdata[mCurFurnaceIndex] != -1;
        castData.FurnaceProducts[CurFurnaceIndex].IsIdle = !isForging;
        //castData.IsForging = isForging;
        if (isForging)
        {
            // StartTimeBar();
            var curTbForgeId = BuildingData.Exdata[mCurFurnaceIndex];
            SetForgeId(curTbForgeId);
        }
        else
        {
            if (mCurTbForgeId >= 0)
            {
                var tbForged = Table.Getforged(mCurTbForgeId);
                castData.FurnaceProducts[CurFurnaceIndex].ItemId = tbForged.ProductID;
            }
        }
        RefreshCastBtnState();
        RefleshFurnaceProductsState();
    }

    private int ConsumeDiamond()
    {
        // 计算一下进度条百分比
        var furnaceEndTime = BuildingData.Exdata64[mCurFurnaceIndex];
        if (furnaceEndTime != -1)
        {
            var needSec = (int) (Extension.FromServerBinary(furnaceEndTime) - Game.Instance.ServerTime).TotalSeconds;
            var needDiamond = (needSec + 299)/300*Table.GetClientConfig(414).ToInt();
            return needDiamond;
        }
        return 0;
    }

    private void RefreshLeftMainBtnState(eSmithyCastType type)
    {
        var begin = eSmithyCastType.Iron;
        var end = eSmithyCastType.Refine;
        mLeftSideMenuStates.Clear();
        for (var i = begin; i <= end; ++i)
        {
            mLeftSideMenuStates.Add((int) i, i == type);
        }
    }

    private void RefleshFurnaceProductsState()
    {
        var tbBuilding = Table.GetBuilding(BuildingData.TypeId);
        if (tbBuilding != null)
        {
            var FurnaceProducts = DataModel.SmithyCastData.FurnaceProducts;
            var tbService = Table.GetBuildingService(tbBuilding.ServiceId);
            var maxCount = tbService.Param[2];
            for (var i = 0; i < FurnaceProducts.Count; i++)
            {
                if (i >= maxCount)
                {
                    FurnaceProducts[i].State = (int) ForgeState.Lock;
                }
                else
                {
                    if (BuildingData.Exdata[i] < 0)
                    {
                        FurnaceProducts[i].State = (int) ForgeState.CanAdd;
                    }
                    else
                    {
                        var tbForged = Table.Getforged(BuildingData.Exdata[i]);
                        if (tbForged != null)
                        {
                            FurnaceProducts[i].ItemId = tbForged.ProductID;
                        }
                        FurnaceProducts[i].FinishTimer = Extension.FromServerBinary(BuildingData.Exdata64[i]);
                        if (FurnaceProducts[i].FinishTimer <= Game.Instance.ServerTime)
                        {
                            FurnaceProducts[i].State = (int) ForgeState.Complete;
                        }
                        else
                        {
                            FurnaceProducts[i].State = (int) ForgeState.During;
                            if (FinishTimeCor == null)
                            {
                                FinishTimeCor = NetManager.Instance.StartCoroutine(RefleshFinishTime());
                            }
                        }
                    }
                }
            }
        }
    }

    public Coroutine FinishTimeCor;

    public IEnumerator RefleshFinishTime()
    {
        var FurnaceProducts = DataModel.SmithyCastData.FurnaceProducts;
        while (true)
        {
            yield return new WaitForSeconds(0.4f);
            if (State == FrameState.Close)
            {
                NetManager.Instance.StopCoroutine(FinishTimeCor);
                FinishTimeCor = null;
                yield break;
            }
            for (var i = 0; i < FurnaceProducts.Count; i++)
            {
                if (FurnaceProducts[i].State != (int) ForgeState.During)
                {
                    continue;
                }

                if (FurnaceProducts[i].FinishTimer <= Game.Instance.ServerTime)
                {
                    FurnaceProducts[i].State = (int) ForgeState.Complete;
                }
                else
                {
                    FurnaceProducts[i].TimerStr = GameUtils.GetTimeDiffString(FurnaceProducts[i].FinishTimer);
                }
            }
        }
        yield break;
    }

    public void RefreshCastLeftSideBtns()
    {
        var castMenuDatas = new ObservableCollection<CastMenuDataModel>();
        {
            // foreach(var state in mLeftSideMenuStates)
            var __enumerator3 = (mLeftSideMenuStates).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var state = __enumerator3.Current;
                {
                    if ((eSmithyCastType)state.Key != eSmithyCastType.Conversion)
                    {
                        continue;
                    }
                    var menuItem = new CastMenuDataModel();
                    menuItem.Type = 0;
                    menuItem.Index = state.Key;
                    menuItem.Pressed = state.Value;
                    switch ((eSmithyCastType) state.Key)
                    {
                        case eSmithyCastType.Iron:
                            //铸造神铁
                            menuItem.BtnName = GameUtils.GetDictionaryText(270063);
                            break;
                        case eSmithyCastType.Conversion:
                            //转化
                            menuItem.BtnName = GameUtils.GetDictionaryText(270064);
                            break;
                        case eSmithyCastType.Refine:
                            //精炼
                            menuItem.BtnName = GameUtils.GetDictionaryText(300915);
                            break;
                    }
                    castMenuDatas.Add(menuItem);

                    if (state.Value)
                    {
                        Table.Foreachforged(tbRecord =>
                        {
                            if (tbRecord.Type != state.Key)
                            {
                                return true;
                            }

                            if (tbRecord.NeedLevel > mBuildingLevel)
                            {
                                return false;
                            }

                            var menuItem1 = new CastMenuDataModel();
                            menuItem1.Type = 1;
                            menuItem1.Index = tbRecord.Id;
                            menuItem1.BtnName = tbRecord.FormulaName;
                            castMenuDatas.Add(menuItem1);
                            return true;
                        });
                    }
                }
            }
        }

        DataModel.SmithyCastData.CastMenuDatas = castMenuDatas;

        RefreshCastBtnState();
    }

    public void RefreshCastBtnState()
    {
        var castData = DataModel.SmithyCastData;
        // 是否正在铸造
        var isForging = BuildingData.Exdata[mCurFurnaceIndex] != -1;
        {
            // foreach(var castMenuData in castData.CastMenuDatas)
            var __enumerator4 = (castData.CastMenuDatas).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var castMenuData = __enumerator4.Current;
                {
                    if (castMenuData.Type != 0)
                    {
                        castMenuData.Pressed = castMenuData.Index == mCurTbForgeId;
                        castMenuData.IsEnable = !isForging || castMenuData.Index == mCurTbForgeId;
                    }
                }
            }
        }
    }

    //左侧按钮点击了
    public void OnClickCastLeftSideBtn(IEvent ievent)
    {
        if (!EquipDataModel.OperateTypes[5])
        {
            //return;
        }
        var e = ievent as SmithyCellClickedEvent;
        var menuData = e.MenuItemData;
        if (menuData.Type == 0)
        {
            var dic = new Dictionary<int, bool>();
            if (mLeftSideMenuStates[menuData.Index] == false)
            {
                {
                    // foreach(var b in mLeftSideMenuStates)
                    var __enumerator1 = (mLeftSideMenuStates).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var b = __enumerator1.Current;
                        {
                            dic.Add(b.Key, b.Key == menuData.Index);
                        }
                    }
                }
            }
            else
            {
                {
                    // foreach(var b in mLeftSideMenuStates)
                    var __enumerator2 = (mLeftSideMenuStates).GetEnumerator();
                    while (__enumerator2.MoveNext())
                    {
                        var b = __enumerator2.Current;
                        {
                            dic.Add(b.Key, false);
                        }
                    }
                }
            }

            mLeftSideMenuStates = dic;
            RefreshCastLeftSideBtns();

            var castMenuDatas = DataModel.SmithyCastData.CastMenuDatas;
            foreach (var data in castMenuDatas)
            {
                if (data.Type == 0)
                {
                    continue;
                }
                OnLeftSideBtnClicked(data.Index);
                break;
            }
        }
        else if (menuData.Type == 1)
        {
            if (menuData.IsEnable) // && !menuData.Pressed)
            {
                OnLeftSideBtnClicked(menuData.Index);
            }
        }
    }

    public void OnLeftSideBtnClicked(int idx)
    {
        SetForgeId(idx);
        RefreshCastBtnState();
        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithySetGridCenter(0));
    }

    public void SetForgeId(int index)
    {
        if (index < 0)
        {
            return;
        }

        var tbForged = Table.Getforged(index);
        var castData = DataModel.SmithyCastData;

        mCurTbForgeId = index;
        castData.FurnaceProducts[CurFurnaceIndex].ItemId = tbForged.ProductID;
        castData.FurnaceProducts[CurFurnaceIndex].State = (int) ForgeState.NoBegin;

        var neededItems = new List<ItemIdDataModel>();
        for (int i = 0, imax = tbForged.NeedItemID.Length; i < imax; i++)
        {
            if (tbForged.NeedItemID[i] == -1)
            {
                break;
            }
            var itemIdData = new ItemIdDataModel();
            itemIdData.ItemId = tbForged.NeedItemID[i];
            itemIdData.Count = tbForged.NeedItemCount[i];
            neededItems.Add(itemIdData);
        }
        for (var i = 0; i < tbForged.NeedResID.Length; i++)
        {
            if (tbForged.NeedResID[i] == -1)
            {
                continue;
            }
            var itemIdData = new ItemIdDataModel();
            itemIdData.ItemId = tbForged.NeedResID[i];
            itemIdData.Count = tbForged.NeedResCount[i];
            neededItems.Add(itemIdData);
        }

        castData.NeededItems = new ObservableCollection<ItemIdDataModel>(neededItems);

        var str = GameUtils.GetTimeDiffString((int) (tbForged.NeedTime*60/(1f + mSpeedUpPercent/100f)));
        castData.FurnaceProducts[CurFurnaceIndex].NeedTimeNow = str;
        castData.FurnaceProducts[CurFurnaceIndex].NeedTimeOrigin = tbForged.NeedTime;

        if (!mLeftSideMenuStates[tbForged.Type])
        {
            RefreshLeftMainBtnState((eSmithyCastType) tbForged.Type);
            RefreshCastLeftSideBtns();
        }
    }

    ////左侧按钮点击了
    //public void OnFurnaceSelected(IEvent ievent)
    //{
    //    var e = ievent as UIEvent_FurnaceSelectedEvent;
    //    CurFurnaceIndex = e.Index;
    //}

    //其它按钮点击相应
    public void OnForgeBtnClicked(IEvent ievent)
    {
        var e = ievent as UIEvent_SmithyBtnClickedEvent;
        SmithyOperation(e.Type);
    }

    public void SmithyOperation(int type)
    {
        switch (type)
        {
            // 铸造按钮按下
            case 0:
            {
                var castData = DataModel.SmithyCastData.SelectedProduct;
                var param = new Int32Array();
                param.Items.Add(castData.State); // 0:铸造功能
                param.Items.Add(CurFurnaceIndex); // 熔炉id

                var state = castData.State;
                switch (state)
                {
                    case 0:
                    {
                        param.Items.Add(mCurTbForgeId); // 铸造id
                    }
                        break;
                    case 1:
                        //var needZS = DataModel.SmithyCastData.NeedDiamond;
                        var content = GameUtils.GetDictionaryText(240900);
                        var needZS = ConsumeDiamond();
                        content = string.Format(content, needZS);
                        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, content, "",
                            () => { UseBuildService(param); });
                        return;
                    case 2:
                        break;
                }
                UseBuildService(param);
            }
                break;

            // 装备进阶按钮按下
            case 1:
            {
                if (!CheckSmithyEvoItem())
                {
                    var e = new ShowUIHintBoard(300600);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                var castData = DataModel.SmithyEvoData;
                var evoItems = castData.EvolveItems;
                var mainItem = evoItems[0];
                mMajorEquipBagId = mainItem.BagId;
                mMajorEquipBagIndex = mainItem.Index;
                var param = new Int32Array();
                param.Items.Add(3); // 0:铸造功能
                {
                    // foreach(var evoItem in evoItems)
                    var __enumerator5 = (evoItems).GetEnumerator();
                    while (__enumerator5.MoveNext())
                    {
                        var evoItem = __enumerator5.Current;
                        {
                            if (evoItem.ItemId != -1)
                            {
                                param.Items.Add(evoItem.BagId);
                                param.Items.Add(evoItem.Index);
                            }
                        }
                    }
                }
                //mRefreshEvoUiAction = ResetEvoUI;
                //进阶后将获得新装备“[{0}]{1}[-]”\n点击[4BE127]确定[-]按钮继续
                var warnStr = GameUtils.GetDictionaryText(300608);
                var tbItem = Table.GetItemBase(castData.EvolvedItemId);
                if (tbItem == null)
                {
                    return;
                }
                warnStr = string.Format(warnStr, GameUtils.GetTableColorString(tbItem.Color), tbItem.Name);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, warnStr, "",
                    () => { UseBuildService(param, AdvanceEquipManual); });
            }
                break;

            case 2:
            {
                DataModel.SmithyEvoData.IsShowEvoedEquip = false;
            }
                break;
            case 3:
            {
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SmithyUI));
                //var e2 = new Show_UI_Event(UIConfig.CityUI);
                //EventDispatcher.Instance.DispatchEvent(e2);
            }
                break;
        }
    }

    private bool CheckSmithyEvoItem()
    {
        var neededItems = DataModel.SmithyEvoData.NeededItems;
        for (var i = 0; i < neededItems.Count; i++)
        {
            if (neededItems[i].Count > PlayerDataManager.Instance.GetItemCount(neededItems[i].ItemId))
            {
                return false;
            }
        }
        return true;
    }

    public void UseBuildService(Int32Array param, Action onOk = null)
    {
        NetManager.Instance.StartCoroutine(UseBuildServiceCoroutine(param, onOk));
    }

    public IEnumerator UseBuildServiceCoroutine(Int32Array param, Action onOk)
    {
        using (new BlockingLayerHelper(0))
        {
            var tbBuilding = Table.GetBuilding(BuildingData.TypeId);

            var msg = NetManager.Instance.UseBuildService(BuildingData.AreaId, tbBuilding.ServiceId, param);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (param.Items[0] == (int) ForgeState.NoBegin)
                    {
                        DataModel.SmithyCastData.SelectedProduct.State = (int) ForgeState.During;
                        if (FinishTimeCor == null)
                        {
                            FinishTimeCor = NetManager.Instance.StartCoroutine(RefleshFinishTime());
                        }
                        EventDispatcher.Instance.DispatchEvent(new SmithyOnPlayTweenAnim(param.Items[1]));
                        if (NeedItemShowCoroutine != null)
                        {
                            NetManager.Instance.StopCoroutine(NeedShowCoroutine());
                            NeedItemShowCoroutine = null;
                        }
                        NeedItemShowCoroutine = NetManager.Instance.StartCoroutine(NeedShowCoroutine());
                    }
                    else if (param.Items[0] == (int) ForgeState.During)
                    {
                        DataModel.SmithyCastData.SelectedProduct.State = (int) ForgeState.Complete;
                        AnalyseNotice();
                    }
                    else if (param.Items[0] == (int) ForgeState.Complete)
                    {
                        PlatformHelper.Event("city", "FurnaceComplete");
                        DataModel.SmithyCastData.SelectedProduct.State = (int) ForgeState.CanAdd;
                        var tbForged = Table.Getforged(BuildingData.Exdata[mCurFurnaceIndex]);
                        var tbserver = Table.GetBuildingService(tbBuilding.ServiceId);
                        if (tbForged != null)
                        {
                            var count = tbForged.NeedTime*tbserver.Param[4]/10000;
                            //var ee = new UIEvent_SmithyFlyAnim(0, count);
                            //ee.Index = mCurFurnaceIndex;
                            //EventDispatcher.Instance.DispatchEvent(ee);
                        }
                        AnalyseNotice();
                    }
                    else if (param.Items[0] == 3) //装备进阶功能功能
                    {
                        var list = DataModel.SmithyEvoData.EvolveItems;
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (list[i].ItemId != -1)
                            {
                                var equipLogic = Table.GetEquipBase(list[i].ItemId).EquipUpdateLogic;
                                var count = Table.GetEquipUpdate(equipLogic).SuccessGetExp;
                                EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyFlyAnim(1, count));
                            }
                        }
                        mRefreshEvoUiAction = ResetEvoUI;
                    }
                    if (onOk != null)
                    {
                        onOk();
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    mRefreshEvoUiAction = null;
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
                mRefreshEvoUiAction = null;
            }
        }
    }

    //建筑升级
    public void OnUpdateBuilding(IEvent ievent)
    {
        if (BuildingData == null)
        {
            return;
        }
        var e = ievent as UIEvent_CityUpdateBuilding;
        if (BuildingData.AreaId == e.Idx)
        {
            BuildingData = CityManager.Instance.GetBuildingByAreaId(e.Idx);
        }
    }

    private void TabPageBtn(IEvent ievent)
    {
        var e = ievent as UIEvent_SmithTabPageEvent;
        DataModel.SmithyCastData.TabPage = e.Index;
    }

    private void SmithyFurnaceCellClick(IEvent ievent)
    {
        var e = ievent as SmithyFurnaceCellEvent;
        var products = DataModel.SmithyCastData.FurnaceProducts;
        if (e.Type == 0 && mCurFurnaceIndex != e.Index)
        {
            if (DataModel.SmithyCastData.SelectedProduct.State == (int) ForgeState.NoBegin)
            {
                DataModel.SmithyCastData.SelectedProduct.ItemId = -1;
                DataModel.SmithyCastData.SelectedProduct.State = (int) ForgeState.CanAdd;
            }
        }

        mCurFurnaceIndex = e.Index;
        DataModel.SmithyCastData.SelectedProduct = products[mCurFurnaceIndex];

        switch (e.Type)
        {
            case 0: //Add
                DataModel.SmithyCastData.ShowFurnacePage = 1;
                OnLeftSideBtnClicked(0);
                break;
            case 1: //Cancel
                break;
            case 2: //Finish
                SmithyOperation(0);
                break;
            case 3: //SpeedUp
                SmithyOperation(0);
                break;
            case 4:
                GameUtils.ShowItemIdTip(products[e.Index].ItemId, (int) eEquipBtnShow.None);
                break;
            case 6:
                SmithyOperation(0);
                break;
            default:
                break;
        }
    }

    #endregion

    #region 装备进阶

    /// <summary>
    /// </summary>
    /// <param name="ievent"></param>
    private void OnEquipCellSelect(IEvent ievent)
    {
        if (!EquipDataModel.OperateTypes[5])
        {
            return;
        }
        
        var e = ievent as EquipCellSelect;
        var index = e.Index;
        if (index < -1 || index > 2)
        {
            Logger.Error("Illegal index value!index = {0}", index);
            return;
        }

        var evoData = DataModel.SmithyEvoData;
        if ((evoData.EvolvedItemId == -1 && index == -1) || index == 0)
        {
            if (evoData.EvolvedItemId == -1)
            {
                ++mEquipCountNow;
                index = 0;
            }
            var item = evoData.EvolveItems[index];
            var oldItemId = item.ItemId;
            var tbEquip = Table.GetEquipBase(e.ItemData.ItemId);
            if (tbEquip == null)
            {
                return;
            }
            var tbEquipEvo = Table.GetEquipUpdate(tbEquip.EquipUpdateLogic);
            if (tbEquipEvo == null)
            {
                return;
            }
            evoData.EvolvedItemId = tbEquip.UpdateEquipID;
            evoData.NeedEquipCount = tbEquipEvo.NeedEquipCount;
            item.Clone(e.ItemData);

            RefreshEvolvedItem();

            if (oldItemId != item.ItemId)
            {
                var neededItems = new List<ItemIdDataModel>();
                for (int i = 0, imax = tbEquipEvo.NeedItemID.Length; i < imax; i++)
                {
                    if (tbEquipEvo.NeedItemID[i] == -1)
                    {
                        break;
                    }

                    var needItem = new ItemIdDataModel();
                    needItem.ItemId = tbEquipEvo.NeedItemID[i];
                    needItem.Count = tbEquipEvo.NeedItemCount[i];
                    neededItems.Add(needItem);
                }
                for (int i = 0, imax = tbEquipEvo.NeedResID.Length; i < imax; i++)
                {
                    if (tbEquipEvo.NeedResID[i] == -1)
                    {
                        continue;
                    }
                    var needItem = new ItemIdDataModel();
                    needItem.ItemId = tbEquipEvo.NeedResID[i];
                    needItem.Count = tbEquipEvo.NeedResCount[i];
                    neededItems.Add(needItem);
                    if (needItem.ItemId == 10) //魔尘
                    {
                        evoData.needMochengCount = needItem.Count;
                        break;
                    }
                }

                evoData.NeededItems = new ObservableCollection<ItemIdDataModel>(neededItems);
                RefreshEquipPackForItemId(e.ItemData.ItemId);
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithySetGridCenter(1));
            }
        }
        else if (mEquipCountNow < evoData.NeedEquipCount || index > 0)
        {
            BagItemDataModel item = null;
            if (index > 0)
            {
                item = evoData.EvolveItems[index];
            }
            else
            {
                var __enumerator6 = (evoData.EvolveItems).GetEnumerator();
                while (__enumerator6.MoveNext())
                {
                    item = __enumerator6.Current;
                    {
                        if (item.IsGrey)
                        {
                            break;
                        }
                    }
                }
            }
            if (item == null)
            {
                Logger.Error("In OnEquipCellSelect(). item == null!");
                return;
            }

            if (item.IsGrey)
            {
                ++mEquipCountNow;
                evoData.IsEnable = mEquipCountNow == evoData.NeedEquipCount;
            }
            item.Clone(e.ItemData);

            RefreshEvolvedItem();
            RefreshEquipPackForItemId(e.ItemData.ItemId);
        }

        if (mEquipCountNow == 1)
        {
            for (var i = 0; i < 3; i++)
            {
                var evolveItem = evoData.EvolveItems[i];
                evolveItem.ItemId = e.ItemData.ItemId;
            }
        }
    }

    private void OnEquipCellSwap(IEvent ievent)
    {
        if (!EquipDataModel.OperateTypes[5])
        {
            return;
        }
        var e = ievent as EquipCellSwap;
        var fromIdx = e.FromIdx;
        if (fromIdx < 0 || fromIdx > 2)
        {
            Logger.Error("Illegal fromIdx value!fromIdx = {0}", fromIdx);
            return;
        }
        var toIdx = e.ToIdx;
        if (toIdx < 0 || toIdx > 2)
        {
            Logger.Error("Illegal toIdx value!toIdx = {0}", toIdx);
            return;
        }

        var evoData = DataModel.SmithyEvoData;
        var fromItem = evoData.EvolveItems[fromIdx];
        if (fromItem == null)
        {
            return;
        }
        var toItem = evoData.EvolveItems[toIdx];
        var tmpItem = new BagItemDataModel();
        tmpItem.Clone(toItem);

        toItem.ItemId = fromItem.ItemId;
        toItem.Count = fromItem.Count;
        toItem.BagId = fromItem.BagId;
        toItem.Index = fromItem.Index;
        toItem.IsGrey = fromItem.IsGrey;

        fromItem.ItemId = tmpItem.ItemId;
        fromItem.Count = tmpItem.Count;
        fromItem.BagId = tmpItem.BagId;
        fromItem.Index = tmpItem.Index;
        fromItem.IsGrey = tmpItem.IsGrey;
        for (int i = 0, imax = fromItem.Exdata.Count; i < imax; i++)
        {
            toItem.Exdata[i] = fromItem.Exdata[i];
            fromItem.Exdata[i] = tmpItem.Exdata[i];
        }

        if (fromIdx == 0 || toIdx == 0)
        {
            RefreshEvolvedItem();
        }
    }

    public void RefreshEquipPackForItemId(int itemId)
    {
        var evoData = DataModel.SmithyEvoData;
        var evoItems = new List<BagItemDataModel>();
        {
            // foreach(var item in evoData.EvolveItems)
            var __enumerator7 = (evoData.EvolveItems).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var item = __enumerator7.Current;
                {
                    if (!item.IsGrey && item.ItemId != -1)
                    {
                        evoItems.Add(item);
                    }
                }
            }
        }

        //mEquipPackController.CallFromOtherClass("RefreshForItemId", new object[2] {itemId, evoItems});
        if (itemId >= 0)
        {  
            var tbEquip = Table.GetEquipBase(itemId);
            mEquipPackController.CallFromOtherClass("RefreshForSameEquipUpdateId", new object[2] { tbEquip.EquipUpdateLogic, evoItems });
        }
    }

    public void OnRemoveEvoEquip(IEvent ievent)
    {
        var e = ievent as RemoveEvoEquipEvent;
        var bagItem = e.ItemData;
        var evoData = DataModel.SmithyEvoData;
        var itemId = bagItem.ItemId;
        evoData.IsEnable = false;
        foreach (var item in evoData.EvolveItems)
        {
            if (item == bagItem && !item.IsGrey)
            {
                item.Exdata.Enchance = 0;
                item.IsGrey = true;
                --mEquipCountNow;
                evoData.IsEnable = false;
                break;
            }
        }

        var bNeedRefreshEquipPack = evoData.EvolveItems[0] == bagItem || mEquipCountNow == 0;
        if (bNeedRefreshEquipPack)
        {
            ResetEvoUI();
        }
        else
        {
            RefreshEquipPackForItemId(itemId);
            RefreshEvolvedItem();
        }
    }

    private void RefreshEvolvedItem()
    {
        var evoData = DataModel.SmithyEvoData;
        var evolvedItem = evoData.EvolvedItem;
        var mainEquip = evoData.EvolveItems[0];
        if (!mainEquip.IsGrey)
        {
            evolvedItem.Clone(mainEquip);
            evolvedItem.ItemId = evoData.EvolvedItemId;
        }
        var maxEnhance = 0;
        var maxAppend = 0;
        foreach (var item in evoData.EvolveItems)
        {
            if (item.IsGrey)
            {
                continue;
            }
            if (maxEnhance < item.Exdata.Enchance)
            {
                maxEnhance = item.Exdata.Enchance;
            }
            if (maxAppend < item.Exdata.Append)
            {
                maxAppend = item.Exdata.Append;
            }
        }
        if (evolvedItem.Exdata.Enchance != maxEnhance)
        {
            evolvedItem.Exdata.Enchance = maxEnhance;
        }
        if (evolvedItem.Exdata.Append != maxAppend)
        {
            evolvedItem.Exdata.Append = maxAppend;
        }
    }

    public void ResetEvoUI()
    {
        mEquipPackController.CallFromOtherClass("RefreshForEvoEquip", null);
        ClearEvoData();
    }

    public void ClearEvoData()
    {
        mEquipCountNow = 0;
        var evoData = DataModel.SmithyEvoData;
        evoData.NeededItems = new ObservableCollection<ItemIdDataModel>();
        evoData.needMochengCount = 0;
        evoData.EvolvedItemId = -1;
        evoData.EvolvedItem.ItemId = -1;
        evoData.IsEnable = false;
        {
            // foreach(var evolveItem in evoData.EvolveItems)
            var __enumerator8 = (evoData.EvolveItems).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var evolveItem = __enumerator8.Current;
                {
                    evolveItem.ItemId = -1;
                    evolveItem.Exdata.Enchance = 0;
                    evolveItem.IsGrey = true;
                }
            }
        }
    }

    public int mMajorEquipBagId;
    public int mMajorEquipBagIndex;

    public void AdvanceEquipManual()
    {
        BagItemDataModel majorEquip;
        if (mMajorEquipBagId == (int) eBagType.Equip)
        {
            majorEquip = PlayerDataManager.Instance.GetItem(mMajorEquipBagId, mMajorEquipBagIndex);
        }
        else
        {
            var equipType = PlayerDataManager.Instance.BagIdToEquipType[mMajorEquipBagId];
            majorEquip = PlayerDataManager.Instance.GetEquipData((eEquipType) equipType);
        }
        if (majorEquip != null && majorEquip.ItemId != -1)
        {
            var evoData = DataModel.SmithyEvoData;
            evoData.EvoedEquip.Clone(evoData.EvolvedItem);
            evoData.IsShowEvoedEquip = true;

            majorEquip.Clone(evoData.EvolvedItem);
        }
    }

    public void OnRefrehEquipBagItemStatus(IEvent ievent)
    {
        //if (State == FrameState.Open)
        if (isShowing == true)
        {
            PlayerDataManager.Instance.RefreshEquipBagStatus();
            if (mRefreshEvoUiAction != null)
            {
                mRefreshEvoUiAction();
                mRefreshEvoUiAction = null;
            }
        }
    }

    private Coroutine NeedItemShowCoroutine;

    private void SmithItemClick(IEvent ievent)
    {
        var e = ievent as UIEvent_SmithItemClick;
        DataModel.SmithyCastData.ShowFurnacePage = 0;
        var products = DataModel.SmithyCastData.FurnaceProducts;
        if (e.Type == 0)
        {
            for (var i = 0; i < products.Count; i++)
            {
                if (products[i].State == (int) ForgeState.NoBegin)
                {
                    products[i].State = (int) ForgeState.CanAdd;
                }
            }
        }
    }

    private IEnumerator NeedShowCoroutine()
    {
        yield return new WaitForSeconds(0.6f);
        DataModel.SmithyCastData.ShowFurnacePage = 0;
    }

    #endregion
}