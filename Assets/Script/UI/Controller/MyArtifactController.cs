#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion


public class MyArtifactController : IControllerBase
{
    private int lastEquipIndex = -1;
    private int lastPackIndex = -1;
    private static int s_SkillItemType = 26800;
    private bool isAdvanceInit = false;
    private int mEquipCountNow;
    private int mMajorEquipBagId;
    private int mMajorEquipBagIndex;
    private bool isBagDirty = false;

    public readonly List<eEquipType> EquipOrder1 = new List<eEquipType>
    {
        eEquipType.WeaponMain,
        eEquipType.WeaponScend,
        eEquipType.Helmet,
        eEquipType.Chest,
        eEquipType.Hand,
        eEquipType.Leg,
        eEquipType.Foot,
        eEquipType.Necklace,
        eEquipType.RingL,
        eEquipType.RingR
    };

    public readonly List<int> EquipOrder2 = new List<int>
    {
        (int) eBagType.Equip11,
        (int) eBagType.Equip12,
        (int) eBagType.Equip01,
        (int) eBagType.Equip05,
        (int) eBagType.Equip08,
        (int) eBagType.Equip09,
        (int) eBagType.Equip10,
        (int) eBagType.Equip02,
        (int) eBagType.Equip07
    };

    public MyArtifactController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(CityDataInitEvent.EVENT_TYPE, OnCityDataInit); // 初始化数据
        EventDispatcher.Instance.AddEventListener(MyArtifactOpEvent.EVENT_TYPE, OnEvent_Operate);
        EventDispatcher.Instance.AddEventListener(ArtifactRemoveEvoEquipEvent.EVENT_TYPE, OnRemoveEvoEquip);
    }

    private MyArtifactDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new MyArtifactDataModel();
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
        EventDispatcher.Instance.AddEventListener(EquipCellSelect.EVENT_TYPE, OnEquipCellSelect);
        EventDispatcher.Instance.AddEventListener(EquipCellSwap.EVENT_TYPE, OnEquipCellSwap);
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagItemCountChange.EVENT_TYPE, OnBagItemChanged);
        EventDispatcher.Instance.AddEventListener(EquipChangeEndEvent.EVENT_TYPE, OnEquipChange);

        ShowModel();

        if (isBagDirty)
        {
            isBagDirty = false;
            RefreshAllEquip();
            if (lastEquipIndex < 0 || lastEquipIndex >= DataModel.EquipItems.Count)
                SelectEquip(0);
            else
            {
                SelectEquip(lastEquipIndex);
            }
        }
    }

    public void Tick()
    {
    }

    private void OnEquipChange(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }

        var e = ievent as EquipChangeEndEvent;
        if (e == null || e.Item == null)
            return;

        var equip = e.Item;
        var tbEquip = Table.GetEquipBase(equip.ItemId);
        if (tbEquip != null && tbEquip.ShowEquip == 1)
        {
            RefreshAllEquip();
        }
    }

    private void OnBagItemChanged(IEvent ievent)
    {
        var e = ievent as UIEvent_BagItemCountChange;
        if (e == null || e.ItemId == -1)
        {
            return;
        }

        var tbRecord = Table.GetItemBase(e.ItemId);
        if (tbRecord != null && tbRecord.Type == s_SkillItemType)
        {
            RefreshBagItems();
        }

        //var tbEquip = Table.GetEquipBase(e.ItemId);
        //if (tbEquip != null && tbEquip.ShowEquip == 1)
        //{
        //    RefreshAllEquip();
        //}
    }

    private void OnResourceChange(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }

        var e = ievent as Resource_Change_Event;
        if (e == null)
            return;

        if (e.Type == eResourcesType.MagicDust)
        {
            //DataModel.AdvanceDataModel.needMochengCount = eResourcesType.MagicDust;
        }
    }

    private EquipItemDataModel createEquipData(BagItemDataModel bagItem)
    {
        if (bagItem.ItemId != -1)
        {
            var tbRecord = Table.GetEquipBase(bagItem.ItemId);
            if (tbRecord != null && tbRecord.ShowEquip == 1)
            {
                var equipItem = new EquipItemDataModel();
                equipItem.BagItemData = bagItem;
                equipItem.TipButtonShow = (int)eEquipBtnShow.None;
                return equipItem;
            }
        }
        return null;
    }

    private EquipItemDataModel createEquipData2(BagItemDataModel bagData)
    {
        if (bagData.ItemId != -1)
        {
            var tbEquip = Table.GetEquipBase(bagData.ItemId);
            if (tbEquip.ShowEquip == 1)
            {
                var equipItem = new EquipItemDataModel();
                equipItem.BagItemData = bagData;
                equipItem.SelectFlag = false;
                equipItem.TipButtonShow = (int)eEquipBtnShow.None;
                equipItem.CanAdvance = (tbEquip.UpdateEquipID != -1) ? 1 : 0;
                return equipItem;
            }
        }
        return null;
    }


    private void RefreshAllEquip()
    {
        DataModel.EquipItems.Clear();
        DataModel.AdvanceEquipItems.Clear();
        PlayerDataManager.Instance.ForeachEquip(bagItem =>
        {
            var equipItem = createEquipData(bagItem);
            if (equipItem != null)
            {
                DataModel.EquipItems.Add(equipItem);
            }

            var equipData = createEquipData2(bagItem);
            if (equipData != null)
            {
                DataModel.AdvanceEquipItems.Add(equipData);
            }
        });


        var packItems = new List<EquipItemDataModel>();
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
        var __enumerator1 = (playerBags.Bags[(int)eBagType.Equip].Items).GetEnumerator();
        while (__enumerator1.MoveNext())
        {
            var bagData = __enumerator1.Current;

            var equipItem = createEquipData(bagData);
            if (equipItem != null)
            {
                DataModel.EquipItems.Add(equipItem);
            }

            var equipData = createEquipData2(bagData);
            if (equipData != null)
            {
                packItems.Add(equipData);
            }
        }

        packItems.Sort(ComparerItem);
        DataModel.AdvanceBagItems.Clear();
        DataModel.AdvanceBagItems = new ObservableCollection<EquipItemDataModel>(packItems);
    }

    public void RefreshAdvanceBag()
    {

    }

    private void RefreshBagItems()
    {
        DataModel.BagItems.Clear();

        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
        var packItems = DataModel.BagItems;
        var __enumerator2 = (playerBags.Bags[(int)eBagType.BaseItem].Items).GetEnumerator();
        while (__enumerator2.MoveNext())
        {
            var item = __enumerator2.Current;
            if (item != null && item.ItemId != -1)
            {
                var tbRecord = Table.GetItemBase(item.ItemId);
                if (tbRecord != null && tbRecord.Type == s_SkillItemType)
                {
                    var equipItem = new EquipItemDataModel();
                    equipItem.BagItemData = item;
                    equipItem.TipButtonShow = (int)eEquipBtnShow.None;
                    packItems.Add(equipItem);
                }
            }
        }

        DataModel.NotHaveItem = (DataModel.BagItems.Count == 0);
    }

    private void ClearAll()
    {
        lastEquipIndex = -1;
        lastPackIndex = -1;
        DataModel.NeedItem.ItemId = -1;
        DataModel.NeedItem.Count = 0;
    }

    public void RefreshData(UIInitArguments data)
    {
        ClearAll();

        RefreshAllEquip();
        RefreshBagItems();

        DataModel.TogglePack = 0;
        DataModel.Page = 0;

        SelectEquip(0);
    }

    private void ShowModel()
    {
        if (DataModel.CurrentEquip != null)
        {
            var e = new MyArtifactShowEquipEvent(DataModel.CurrentEquip.ItemId);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    private void SelectEquip(int idx)
    {
        if (idx < 0 || idx >= DataModel.EquipItems.Count)
        {
            DataModel.Page = -1;
            return;
        }

        if (DataModel.CurrentEquip == null)
        {
            return;
        }

        if (lastEquipIndex >= 0 && lastEquipIndex < DataModel.EquipItems.Count)
        {
            DataModel.EquipItems[lastEquipIndex].SelectFlag = false;
        }

        lastEquipIndex = idx;
        var selectEquip = DataModel.EquipItems[idx];
        selectEquip.SelectFlag = true;
        DataModel.CurrentEquip = selectEquip.BagItemData;

        ShowModel();

        var bagItem = DataModel.CurrentEquip;
        if ((int)EquipExdataDefine.BuffId < bagItem.Exdata.Count)
        {
            DataModel.SelectBuffId = bagItem.Exdata[(int)EquipExdataDefine.BuffId];
        }
        if ((int)EquipExdataDefine.SkillLevel < bagItem.Exdata.Count)
        {
            DataModel.SelectBuffLevel = bagItem.Exdata[(int)EquipExdataDefine.SkillLevel];
        }

        DataModel.RandBuffId = -1;
        if ((int)EquipExdataDefine.RandBuffId < bagItem.Exdata.Count)
        {
            DataModel.RandBuffId = bagItem.Exdata[(int)EquipExdataDefine.RandBuffId];
        }
    }

    private void SelectItem(int idx)
    {
        if (idx < 0 || idx >= DataModel.BagItems.Count)
        {
            return;
        }

        if (lastPackIndex >= 0 && lastPackIndex < DataModel.BagItems.Count)
        {
            DataModel.BagItems[lastPackIndex].SelectFlag = false;
        }

        lastPackIndex = idx;

        var selectItem = DataModel.BagItems[idx];
        selectItem.SelectFlag = true;
        DataModel.NeedItem.ItemId = selectItem.BagItemData.ItemId;
        DataModel.NeedItem.Count = 1;
        DataModel.Page = 1;
    }

    private void OnEquipCellSelect(IEvent ievent)
    {
        var e = ievent as EquipCellSelect;
        if (e != null)
        {
            if (DataModel.Page == 2)
            { // 进阶
                SelectAdvanceEquip(e.Index, e.ItemData);
            }
            else
            {
                if (DataModel.TogglePack == 0)
                {
                    if (lastEquipIndex != e.Index)
                        SelectEquip(e.Index);
                }
                else
                {
                    SelectItem(e.Index);
                }
            }
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
        EventDispatcher.Instance.RemoveEventListener(EquipCellSelect.EVENT_TYPE, OnEquipCellSelect);
        EventDispatcher.Instance.RemoveEventListener(EquipCellSwap.EVENT_TYPE, OnEquipCellSwap);
        EventDispatcher.Instance.RemoveEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
        EventDispatcher.Instance.RemoveEventListener(UIEvent_BagItemCountChange.EVENT_TYPE, OnBagItemChanged);
        EventDispatcher.Instance.RemoveEventListener(EquipChangeEndEvent.EVENT_TYPE, OnEquipChange);
    }

    private void RequestRandSkill(int bagId, int bagIndex)
    {
        if (DataModel.NeedItem.ItemId == -1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100002120));
            return;
        }

        if (!GameUtils.CheckEnoughItems(DataModel.NeedItem.ItemId, DataModel.NeedItem.Count))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            //PlayerDataManager.Instance.ShowItemInfoGet(tbEnchance.NeedItemId[i]);
            return;
        }

        NetManager.Instance.StartCoroutine(RankEquipSkillCoroutine(bagId, bagIndex, DataModel.NeedItem.ItemId));
    }

    public IEnumerator RankEquipSkillCoroutine(int bagId, int bagIndex, int itemId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.RandEquipSkill(bagId, bagIndex, itemId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    DataModel.RandBuffId = msg.Response;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("msgSendFun..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("msgSendFun..................." + msg.State);
            }
        }
    }

    private void UseEquipSkill(int bagId, int bagIndex, bool isUse)
    {
        BagItemDataModel equipData = null;
        if (bagId == (int)eBagType.Equip)
        {
            equipData = PlayerDataManager.Instance.GetItem(bagId, bagIndex);
        }
        else
        {
            var equipType = PlayerDataManager.Instance.ChangeBagIdToEquipType(bagId, bagIndex);
            if (equipType != -1)
            {
                equipData = PlayerDataManager.Instance.GetEquipData((eEquipType)equipType);
            }
        }

        if (equipData != null)
        {
            if ((int)EquipExdataDefine.RandBuffId < equipData.Exdata.Count)
            {
                var tempBuffId = equipData.Exdata[(int)EquipExdataDefine.RandBuffId];
                if (tempBuffId < 0)
                {
                    return;
                }
                NetManager.Instance.StartCoroutine(UseEquipSkillCoroutine(bagId, bagIndex, isUse));
            }
        }
    }

    public IEnumerator UseEquipSkillCoroutine(int bagId, int bagIndex, bool isUse)
    {
        using (new BlockingLayerHelper(0))
        {
            var type = isUse ? 1 : 0;
            var msg = NetManager.Instance.UseEquipSkill(bagId, bagIndex, type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    DataModel.RandBuffId = -1;
                    DataModel.SelectBuffId = msg.Response;
                    if (isUse)
                    {
                        DataModel.SaveEffect = false;
                        DataModel.SaveEffect = true;
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("msgSendFun..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("msgSendFun..................." + msg.State);
            }
        }
    }

    private void OnEvent_Operate(IEvent ievent)
    {
        var e = ievent as MyArtifactOpEvent;
        if (e == null)
            return;

        switch (e.Idx)
        {
            case 0:
                {
                    DataModel.Page = 0;
                    DataModel.TogglePack = 0;
                }
                break;
            case 1:
                {
                    DataModel.Page = 1;
                    DataModel.TogglePack = 1;
                }
                break;
            case 2:
                {
                    if (DataModel.Page == 0)
                    {
                        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MyArtifactUI));
                    }
                    else
                    {
                        DataModel.Page = 0;
                        DataModel.TogglePack = 0;
                        DataModel.SaveEffect = false;
                        DataModel.RandEffect = false;
                    }

                }
                break;
            case 3:
            {
                DataModel.RandEffect = false;
                DataModel.RandEffect = true;
                var bagItem = DataModel.CurrentEquip;
                RequestRandSkill(bagItem.BagId, bagItem.Index);
            }
                break;
            case 4:
                {
                    var bagItem = DataModel.CurrentEquip;
                    UseEquipSkill(bagItem.BagId, bagItem.Index, true);
                }
                break;
            case 5:
                {
                    var bagItem = DataModel.CurrentEquip;
                    UseEquipSkill(bagItem.BagId, bagItem.Index, false);
                }
                break;
            case 6:
                {
                    if (DataModel.Page == 2)
                        DataModel.AdvanceSelectPack = 1;
                    else
                    {
                        DataModel.TogglePack = 1;
                    }
                }
                break;
            case 7:
                {
                    if (DataModel.Page == 2)
                        DataModel.AdvanceSelectPack = 0;
                    else
                    {
                        DataModel.TogglePack = 0;
                    }
                }
                break;
            case 8:
                {
                    var itemId = int.Parse(Table.GetClientConfig(1208).Value);
                    GameUtils.CheckEnoughItems(itemId, 1);
                }
                break;
            case 9:
                {
                    DataModel.Page = 2;
                    DataModel.TogglePack = 2;
                    ClearEvoData();
                    if (isAdvanceInit == false)
                    {
                        isAdvanceInit = true;
                        PlayerDataManager.Instance.RefreshEquipBagStatus();
                        DataModel.AdvanceSelectPack = 0;
                    }
                }
                break;
            case 10:
                {
                    if (!CheckSmithyEvoItem())
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300600));
                        return;
                    }
                    var castData = DataModel.AdvanceDataModel;
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
        }
    }

    public void AdvanceEquipManual()
    {
        BagItemDataModel majorEquip;
        if (mMajorEquipBagId == (int)eBagType.Equip)
        {
            majorEquip = PlayerDataManager.Instance.GetItem(mMajorEquipBagId, mMajorEquipBagIndex);
        }
        else
        {
            var equipType = PlayerDataManager.Instance.BagIdToEquipType[mMajorEquipBagId];
            majorEquip = PlayerDataManager.Instance.GetEquipData((eEquipType)equipType);
        }
        if (majorEquip != null && majorEquip.ItemId != -1)
        {
            var evoData = DataModel.AdvanceDataModel;
            evoData.EvoedEquip.Clone(evoData.EvolvedItem);
            evoData.IsShowEvoedEquip = true;

            majorEquip.Clone(evoData.EvolvedItem);
        }

        ClearEvoData();
    }

    public BuildingData mBuildingData;
    public int mBuildingLevel;
    public float mSpeedUpPercent;
    public BuildingData BuildingData
    {
        get { return mBuildingData; }
        set
        {
            mBuildingData = value;

            var castData = DataModel.AdvanceDataModel;
            var tbBuilding = Table.GetBuilding(value.TypeId);
            var tbBuildService = Table.GetBuildingService(tbBuilding.ServiceId);

            var pets = mBuildingData.PetList;
            var petCount = pets.Count;
            var baseSpeed = tbBuildService.Param[0] / 100;
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
                var thisPetref = CityPetSkill.GetBSParamByIndex((BuildingType)tbBuilding.Type, tbBuildService, 1,
                    petlist);
                petTotleSpeed += thisPetref / 100;
            }
            mBuildingLevel = tbBuilding.Level;
            mSpeedUpPercent = baseSpeed + petTotleSpeed;
        }
    }

    public void OnCityDataInit(IEvent ievent)
    {
        var buildingdData = CityManager.Instance.GetBuildingDataByType(BuildingType.BlacksmithShop);
        if (buildingdData != null)
        {
            BuildingData = buildingdData;
            //AnalyseNotice();
        }
    }

    public void UseBuildService(Int32Array param, Action onOk = null)
    {
        NetManager.Instance.StartCoroutine(UseBuildServiceCoroutine(param, onOk));
    }

    public IEnumerator UseBuildServiceCoroutine(Int32Array param, Action onOk)
    {
        yield return null;
        using (new BlockingLayerHelper(0))
        {
            var tbBuilding = Table.GetBuilding(BuildingData.TypeId);

            var msg = NetManager.Instance.UseBuildService(BuildingData.AreaId, tbBuilding.ServiceId, param);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    if (param.Items[0] == 3) //装备进阶功能
                    {
                        var list = DataModel.AdvanceDataModel.EvolveItems;
                        for (var i = 0; i < list.Count; i++)
                        {
                            if (list[i].ItemId != -1)
                            {
                                var equipLogic = Table.GetEquipBase(list[i].ItemId).EquipUpdateLogic;
                                var count = Table.GetEquipUpdate(equipLogic).SuccessGetExp;
                                EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyFlyAnim(1, count));
                            }
                        }

                        isBagDirty = true;
                        GameUtils.ShowEquipModel(DataModel.AdvanceDataModel.EvolvedItemId);
                    }
                    if (onOk != null)
                    {
                        onOk();
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    //mRefreshEvoUiAction = null;
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
                //mRefreshEvoUiAction = null;
            }
        }
    }

    private bool CheckSmithyEvoItem()
    {
        var neededItems = DataModel.AdvanceDataModel.NeededItems;
        for (var i = 0; i < neededItems.Count; i++)
        {
            if (neededItems[i].Count > PlayerDataManager.Instance.GetItemCount(neededItems[i].ItemId))
            {
                return false;
            }
        }
        return true;
    }

    private void RefreshEvolvedItem()
    {
        var evoData = DataModel.AdvanceDataModel;
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

    public void RefreshEquipPackForItemId(int itemId)
    {
        var evoData = DataModel.AdvanceDataModel;
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
    }

    private void AdvanceChooseMainEquip(BagItemDataModel itemData)
    {
        ClearEvoData();
        mEquipCountNow = 1;
        var evoData = DataModel.AdvanceDataModel;
        var index = 0;
        var item = evoData.EvolveItems[index];
        var oldItemId = item.ItemId;
        var tbEquip = Table.GetEquipBase(itemData.ItemId);
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
        item.Clone(itemData);

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
            RefreshEquipPackForItemId(itemData.ItemId);
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithySetGridCenter(1));
        }
    }


    private void SelectAdvanceEquip(int index, BagItemDataModel itemData)
    {
        var evoData = DataModel.AdvanceDataModel;
        if (mEquipCountNow >= evoData.NeedEquipCount)
            return;

        EquipItemDataModel equipDataModel;
        if (DataModel.AdvanceSelectPack == 0)
        {
            equipDataModel = DataModel.AdvanceEquipItems[index];
        }
        else
        {
            equipDataModel = DataModel.AdvanceBagItems[index];
        }
        if (equipDataModel.SelectFlag)
        {
            return;
        }
        if (equipDataModel.CanAdvance == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100002130));
            return;
        }

        if (evoData.EvolvedItemId == -1 || DataModel.AdvanceSelectPack == 0)
        { // 主装备
            AdvanceChooseMainEquip(itemData);
            equipDataModel.SelectFlag = true;
        }
        else
        {
            var choseItem = evoData.EvolveItems[0];
            if (choseItem.ItemId == itemData.ItemId)
            { // 同一物品id
                equipDataModel.SelectFlag = true;

                BagItemDataModel item = evoData.EvolveItems[mEquipCountNow];
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
                item.Clone(itemData);

                RefreshEvolvedItem();
                RefreshEquipPackForItemId(itemData.ItemId);
            }
            else
            { // 不同物品
                AdvanceChooseMainEquip(itemData);
                equipDataModel.SelectFlag = true;
            }

        }
        if (mEquipCountNow == 1)
        {
            for (var i = 0; i < 3; i++)
            {
                var evolveItem = evoData.EvolveItems[i];
                evolveItem.ItemId = itemData.ItemId;
            }
        }
    }

    public void ClearEvoData()
    {
        mEquipCountNow = 0;
        var evoData = DataModel.AdvanceDataModel;
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

        var enumrator1 = DataModel.AdvanceBagItems.GetEnumerator();
        while (enumrator1.MoveNext())
        {
            enumrator1.Current.SelectFlag = false;
        }

        var enumrator2 = DataModel.AdvanceEquipItems.GetEnumerator();
        while (enumrator2.MoveNext())
        {
            enumrator2.Current.SelectFlag = false;
        }
    }



    public int ComparerItem(EquipItemDataModel a, EquipItemDataModel b)
    {
        //部位
        var tbEquipA = Table.GetEquipBase(a.BagItemData.ItemId);
        var tbEquipB = Table.GetEquipBase(b.BagItemData.ItemId);
        var bagIdA = GameUtils.GetEquipBagId(tbEquipA);
        var bagIdB = GameUtils.GetEquipBagId(tbEquipB);
        var indexA = EquipOrder2.FindIndex(i => i == bagIdA);
        var indexB = EquipOrder2.FindIndex(i => i == bagIdB);
        if (indexA != indexB)
        {
            return indexA - indexB;
        }

        //阶数
        if (tbEquipA.Ladder != tbEquipB.Ladder)
        {
            return tbEquipB.Ladder - tbEquipA.Ladder;
        }

        //品质
        var tbItemA = Table.GetItemBase(a.BagItemData.ItemId);
        var tbItemB = Table.GetItemBase(b.BagItemData.ItemId);
        if (tbItemA.Quality != tbItemB.Quality)
        {
            return tbItemB.Quality - tbItemA.Quality;
        }

        //强化等级
        var enhanceA = a.BagItemData.Exdata.Enchance;
        var enhanceB = b.BagItemData.Exdata.Enchance;
        if (enhanceA != enhanceB)
        {
            return enhanceB - enhanceA;
        }

        //战力
        if (a.BagItemData.FightValue != b.BagItemData.FightValue)
        {
            return b.BagItemData.FightValue - a.BagItemData.FightValue;
        }

        //item id
        if (a.BagItemData.ItemId != b.BagItemData.ItemId)
        {
            return a.BagItemData.ItemId - b.BagItemData.ItemId;
        }

        //bag index
        return a.BagItemData.Index - b.BagItemData.Index;
    }

    public void OnRemoveEvoEquip(IEvent ievent)
    {
        var e = ievent as ArtifactRemoveEvoEquipEvent;
        var bagItem = e.ItemData;
        var evoData = DataModel.AdvanceDataModel;
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
            ClearEvoData();
        }
        else
        {
            RefreshEquipPackForItemId(itemId);
            RefreshEvolvedItem();

            var enumrator1 = DataModel.AdvanceBagItems.GetEnumerator();
            while (enumrator1.MoveNext())
            {
                var dm = enumrator1.Current;
                if (dm.BagItemData.BagId == bagItem.BagId && dm.BagItemData.Index == bagItem.Index)
                {
                    enumrator1.Current.SelectFlag = false;
                    return;
                }
            }

            var enumrator2 = DataModel.AdvanceEquipItems.GetEnumerator();
            while (enumrator2.MoveNext())
            {
                var dm = enumrator2.Current;
                if (dm.BagItemData.BagId == bagItem.BagId && dm.BagItemData.Index == bagItem.Index)
                {
                    enumrator2.Current.SelectFlag = false;
                    return;
                }
            }
        }
    }

    public void OnEquipCellSwap(IEvent ievent)
    {

    }


    public FrameState State { get; set; }
}