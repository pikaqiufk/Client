#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class EquipUIController : IControllerBase
{
    public static BagItemDataModel mEmptyBagItem = new BagItemDataModel();

    public EquipUIController()
    {
        //mEquipPackController = UIManager.Instance.CreateControllerBase(UIConfig.EquipPack);
        mEquipPackController = UIManager.Instance.GetController(UIConfig.EquipPack);
        mSmithyFrameController = UIManager.Instance.GetController(UIConfig.SmithyUI);
        CleanUp();

        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnRefrehEquipBagItemStatus);
        EventDispatcher.Instance.AddEventListener(EquipOperateClick.EVENT_TYPE, OnClickEquipOperate);
        EventDispatcher.Instance.AddEventListener(VipLevelChangedEvent.EVENT_TYPE, OnVipLevelChanged);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagItemCountChange.EVENT_TYPE, OnBagItemChanged);
        EventDispatcher.Instance.AddEventListener(UIEvent_SpecialItemShowEvent.EVENT_TYPE, OnSpecialItemShow);
        
    }

    //当前选择的物品数据
    public BagItemDataModel mBagItemData;
    //装备筛选界面右侧的相关数据类型
    public IControllerBase mEquipPackController;
    private IControllerBase mSmithyFrameController;
    public int mLastType = -1;
    public EquipUIDataModel DataModel { get; set; }

    public EquipPackDataModel EquipPackDataModel
    {
        get { return mEquipPackController.GetDataModel("") as EquipPackDataModel; }
    }

    //追加网络包逻辑
    public IEnumerator AppendEquipCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var appendData = DataModel.EquipAppendData;
            var itemData = appendData.AppendItem;
            var msg = NetManager.Instance.AppendEquip(itemData.BagId, itemData.Index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var e = new EquipUiNotifyLogic(1);
                    EventDispatcher.Instance.DispatchEvent(e);
                    //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220006));
                    itemData.Exdata[1] = msg.Response;
                    itemData.Exdata[25] += DataModel.EquipAppendData.CostItemCount;
                    RefreshAppend(itemData);
                    if (itemData.Exdata.Binding != 1)
                    {
                        itemData.Exdata.Binding = 1;
                    }
                    RefreshEquipBagStatus(itemData);

                    var tbItemBase = Table.GetItemBase(itemData.ItemId);
                    var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
                    PlatformHelper.UMEvent("EquipJingLian", itemData.BagId.ToString(), msg.Response + "/" + tbEquip.AddAttrMaxValue);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.DiamondNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("AppendEquip..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("AppendEquip..................." + msg.State);
            }
        }
    }

    //检查追加的条件
    public bool CheckInheritAppend()
    {
        var inheritData = DataModel.EquipInheritData;
        var inheritItem = inheritData.InheritItem;
        var inheritedItem = inheritData.InheritedItem;
        var appendValue1 = inheritItem.Exdata[1];
        var appendValue2 = inheritedItem.Exdata[1];
        var tbEquipBase = Table.GetEquipBase(inheritData.InheritedItem.ItemId);
        if (appendValue2 == tbEquipBase.AddAttrMaxValue)
        {
            //被传承装备追加属性已经达到上限，无需传承
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270088));
            return false;
        }
        var tbToEquip = Table.GetItemBase(inheritData.InheritedItem.ItemId);
        var tbFromEquip = Table.GetItemBase(inheritData.InheritItem.ItemId);
        if (tbToEquip.Type == tbFromEquip.Type)
        {
            if (appendValue1 <= appendValue2)
            {
                //被传承装备追加属性已经大于等于传承装备追加属性
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270087));
                return false;
            }
        }
        if (appendValue1 > tbEquipBase.AddAttrMaxValue)
        {
            //被传承装备追加属性上限低于传承装备追加属性，是否继续
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270090, "",
                () => { SmritiEquipConfirm(); });
            return false;
        }
        return true;
    }

    //检查强化的条件
    public bool CheckInheritEnchance()
    {
        var inheritData = DataModel.EquipInheritData;
        var inheritItem = inheritData.InheritItem;
        var inheritedItem = inheritData.InheritedItem;
        var tbItemBase = Table.GetItemBase(inheritedItem.ItemId);
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);

        if (inheritedItem.Exdata[0] == tbEquip.MaxLevel)
        {
            //装备达到最大的强化等级
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270085));
            return false;
        }
        if (inheritedItem.Exdata[0] >= inheritItem.Exdata[0])
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220009));
            return false;
        }
        if (inheritItem.Exdata[0] > tbEquip.MaxLevel)
        {
            //继承装备强化等级已经大于等于传承装备强化等级，无需传承
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270086, "", () => { SmritiEquipConfirm(); });
            return false;
        }
        return true;
    }

    //检查洗练的条件
    public bool CheckInheritExcellent()
    {
        var inheritData = DataModel.EquipInheritData;
        var inheritItem = inheritData.InheritItem;
        var inheritedItem = inheritData.InheritedItem;
        var tbItemBase = Table.GetItemBase(inheritedItem.ItemId);
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);

        var range = tbEquip.ExcellentAttrValue;
        var tbEnchant = Table.GetEquipEnchant(range);
        var maxRate = tbEquip.ExcellentValueMax;

        var isAllSamll = true;
        for (var i = 0; i < 4; i++)
        {
            if (inheritedItem.Exdata[2 + i] < inheritItem.Exdata[2 + i])
            {
                isAllSamll = false;
                break;
            }
        }
        if (isAllSamll)
        {
            //继承装备卓越属性已经大于等于传承装备卓越属性，无需传承
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270091));
            return false;
        }

        var isOneSamll = false;
        for (var i = 0; i < 4; i++)
        {
            if (inheritedItem.Exdata[2 + i] > inheritItem.Exdata[2 + i])
            {
                isOneSamll = true;
                break;
            }
        }
        if (isOneSamll)
        {
            //继承装备有洗炼属性高于传承装备，可能导致战斗力下降，是否继续？
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270092, "",
                () => { SmritiEquipConfirm(); });
            return false;
        }

        var isLimit = false;
        for (var i = 0; i < 4; i++)
        {
            var attrid = tbEquip.ExcellentAttrId[i];
            var index = GameUtils.GetAttrIndex(attrid);
            if (index != -1 && inheritItem.Exdata[2 + i] < tbEnchant.Attr[index]*maxRate/100)
            {
                isLimit = true;
                break;
            }
        }
        if (isLimit)
        {
            //继承装备追加属性上限低于传承装备追加属性，是否继续？
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 220011, "",
                () => { SmritiEquipConfirm(); });
            return false;
        }

        return true;
    }

    //强化网络包处理
    public IEnumerator EnchanceEquipCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var enchangeData = DataModel.EquipEnchanceData;
            var itemData = enchangeData.EnchanceItem;

            var msg = NetManager.Instance.EnchanceEquip(itemData.BagId, itemData.Index,
                Convert.ToInt32(enchangeData.IsSpecialItem), Convert.ToInt32(enchangeData.IsSuccessRate));
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (itemData.Exdata[0] < msg.Response)
                    {
                        //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220002));
                        var e = new EquipUiNotifyLogic(1);
                        EventDispatcher.Instance.DispatchEvent(e);
                        PlatformHelper.UMEvent("EquipEnchance", itemData.BagId.ToString(), (itemData.Exdata[0] + 1) + "|1");
                    }
                    else
                    {
                        //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220003));
                        var e = new EquipUiNotifyLogic(2);
                        EventDispatcher.Instance.DispatchEvent(e);
                        PlatformHelper.UMEvent("EquipEnchance", itemData.BagId.ToString(), (itemData.Exdata[0] - 1) + "|0");
                    }
                    itemData.Exdata[0] = msg.Response;
                    if (itemData.Exdata.Binding != 1)
                    {
                        itemData.Exdata.Binding = 1;
                    }
                    RefreshEnchance(itemData, false);
                    RefreshEquipBagStatus(itemData);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.DiamondNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
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

    //洗练网络包逻辑
    public IEnumerator ExcellentResetEquipCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var itemData = DataModel.EquipExcellentRestData.ExcellentItem;
            var msg = NetManager.Instance.ResetExcellentEquip(itemData.BagId, itemData.Index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220007));
                    for (var i = 0; i < 4; i++)
                    {
                        itemData.Exdata[18 + i] = msg.Response.Items[i];
                    }
                    RefreshExcelletReset(itemData);
                    if (itemData.Exdata.Binding != 1)
                    {
                        itemData.Exdata.Binding = 1;
                    }

                    PlatformHelper.UMEvent("EquipXiLian", itemData.BagId.ToString());
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("ExcellentResetEquip..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("ExcellentResetEquip..................." + msg.State);
            }
        }
    }

    //洗练结果网络包逻辑
    public IEnumerator ExcellentResetOkCoroutine(int ret)
    {
        using (new BlockingLayerHelper(0))
        {
            var itemData = DataModel.EquipExcellentRestData.ExcellentItem;
            var msg = NetManager.Instance.ConfirmResetExcellentEquip(itemData.BagId, itemData.Index, ret);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (ret == 1)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            if (itemData.Exdata[18 + i] != -1)
                            {
                                itemData.Exdata[2 + i] = itemData.Exdata[18 + i];
                                itemData.Exdata[18 + i] = -1;
                            }
                        }
                        var e = new EquipUiNotifyLogic(1);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            itemData.Exdata[18 + i] = -1;
                        }
                    }
                    RefreshExcelletReset(itemData);

                    RefreshEquipBagStatus(itemData);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.DiamondNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("ExcellentResetOK..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("ExcellentResetOK..................." + msg.State);
            }
        }
    }

    public static int GetAdditionalTable1(EquipAdditional1Record tbAdditional, int Value)
    {
        var tbskillup = Table.GetSkillUpgrading(tbAdditional.AddPropArea);
        var level = 0;
        var lValue = tbskillup.GetSkillUpgradingValue(level);
        while (lValue < Value)
        {
            level++;
            var newValue = tbskillup.GetSkillUpgradingValue(level);
            if (newValue == lValue)
            {
                break;
            }
            lValue = newValue;
        }
        return level;
    }

    public bool IsShowAppendNotice(BagItemDataModel bagItem, int changeCount)
    {
        var itemId = bagItem.ItemId;
        if (itemId == -1)
        {
            return false;
        }

        var tbItemBase = Table.GetItemBase(itemId);
        if (tbItemBase == null)
        {
            return false;
        }
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
        if (tbEquip == null)
        {
            return false;
        }

        var appendId = tbEquip.AddIndexID;
        if (appendId == -1)
        {
            return false;
        }
        var tbAppdend = Table.GetEquipAdditional1(appendId);
        if (tbAppdend == null)
        {
            return false;
        }

        var addAttrValue = bagItem.Exdata[1];
        var isMaxValue = (addAttrValue == tbEquip.AddAttrMaxValue);
        if (isMaxValue)
        {
            return false;
        }

        var addLevel = GetAdditionalTable1(tbAppdend, bagItem.Exdata[1]);
        var costItemCount = Table.GetSkillUpgrading(tbAppdend.MaterialCount).GetSkillUpgradingValue(addLevel);
        var costMoney = Table.GetSkillUpgrading(tbAppdend.Money).GetSkillUpgradingValue(addLevel);

        var money = PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes);
        if (money <= costMoney)
        {
            return false;
        }

        var nowCount = PlayerDataManager.Instance.GetItemTotalCount(tbAppdend.MaterialID).Count;
        var beforeCount = nowCount - changeCount;

        if (beforeCount < costItemCount && nowCount >= costItemCount)
        {
            return true;
        }

        return false;
    }

    private void OnBagItemChanged(IEvent ievent)
    {
        var e = ievent as UIEvent_BagItemCountChange;
        var tbAppdend = Table.GetEquipAdditional1(1);
        if (e != null && e.ItemId != tbAppdend.MaterialID)
        {
            return;
        }

        RefreshAppendNotice(e.ChangeCount);
    }

    private void OnSpecialItemShow(IEvent ievent)
    {
        DataModel.EquipEnchanceData.IsSpecialItemShow = !DataModel.EquipEnchanceData.IsSpecialItemShow; 
        EventDispatcher.Instance.DispatchEvent(new EquipUiNotifyRefreshCoumuseList());
    }

    public void OnChangeEquipCell(IEvent ievent)
    {
        if (mLastType == 5)
        {
            return;
        }
        
        var e = ievent as EquipCellSelect;
        RefreshItemData(e.ItemData, e.Index);
    }

    //--------------------------------------------------------------------Append------------------
    public void OnClickEquipAppend()
    {
        PlayerDataManager.Instance.WeakNoticeData.Additional = false;
        var playerData = PlayerDataManager.Instance.PlayerDataModel;
        var appendData = DataModel.EquipAppendData;
        var tbAppdend = Table.GetEquipAdditional1(appendData.AppendId);
        if (tbAppdend == null)
        {
            Logger.Error("OnClickEquipAppend  error :appendData.AppendId = {0}, ", appendData.AppendId);
            return;
        }
        if (appendData.IsMaxValue == 1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220001));
            return;
        }

        if (appendData.CostMoney > playerData.Bags.Resources.Gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
            return;
        }
        //if (appendData.CostItemCount > PlayerDataManager.Instance.GetItemCount(tbAppdend.MaterialID))
        //{
        //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
        //    PlayerDataManager.Instance.ShowItemInfoGet(tbAppdend.MaterialID);
        //    return;
        //}

        if (!GameUtils.CheckEnoughItems(tbAppdend.MaterialID, appendData.CostItemCount))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            return;
        }

        var tbItem = Table.GetItemBase(appendData.AppendItem.ItemId);
        if (tbItem == null)
        {
            return;
        }
        if (tbItem.CanTrade == 1 && appendData.AppendItem.Exdata.Binding != 1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 210117, "",
                () => { NetManager.Instance.StartCoroutine(AppendEquipCoroutine()); });
            return;
        }
        NetManager.Instance.StartCoroutine(AppendEquipCoroutine());
    }

    //--------------------------------------------------------------------Enchance------------------
    public void OnClickEquipEnchance()
    {
        var playerData = PlayerDataManager.Instance.PlayerDataModel;
        var enchangeData = DataModel.EquipEnchanceData;
        var itemData = enchangeData.EnchanceItem;
        if (itemData.ItemId == -1)
        {
            //请选择物品
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270084));
            return;
        }
        var tbItem = Table.GetItemBase(itemData.ItemId);
        if (tbItem == null)
        {
            return;
        }
        if (enchangeData.IsMaxLevel == 1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220000));
            return;
        }

        var enchanceId = DataModel.EquipEnchanceData.EnchanceId;
        if (enchanceId == -1)
        {
            enchanceId = itemData.Exdata[0];
        }
        var tbEnchance = Table.GetEquipBlessing(enchanceId);
        if (tbEnchance == null)
        {
            return;
        }

        var items = new Dictionary<int, int>();
        if (enchangeData.IsSpecialItem)
        {
            items[tbEnchance.WarrantItemId] = tbEnchance.WarrantItemCount;
            //if (tbEnchance.WarrantItemCount > PlayerDataManager.Instance.GetItemCount(tbEnchance.WarrantItemId))
            //{
            //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            //    PlayerDataManager.Instance.ShowItemInfoGet(tbEnchance.WarrantItemId);
            //    return;
            //}
        }

        if (tbEnchance.NeedMoney > playerData.Bags.Resources.Gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
            return;
        }

        for (var i = 0; i < 3; i++)
        {
            if (tbEnchance.NeedItemId[i] != -1)
            {
                items[tbEnchance.NeedItemId[i]] = tbEnchance.NeedItemCount[i];
            }
        }

        if (!GameUtils.CheckEnoughItems(items, true))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            //PlayerDataManager.Instance.ShowItemInfoGet(tbEnchance.NeedItemId[i]);
            return;
        }

        if (tbItem.CanTrade == 1 && itemData.Exdata.Binding != 1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 210117, "",
                () => { NetManager.Instance.StartCoroutine(EnchanceEquipCoroutine()); });
            return;
        }
        NetManager.Instance.StartCoroutine(EnchanceEquipCoroutine());
    }

    //--------------------------------------------------------------------Inherit------------------
    public void OnClickEquipInherit()
    {
        var inheritData = DataModel.EquipInheritData;
        if (inheritData.InheritItem.ItemId == -1
            || inheritData.InheritedItem.ItemId == -1)
        {
            return;
        }

        var tbFromEquip = Table.GetItemBase(inheritData.InheritItem.ItemId);
        var tbToEquip = Table.GetItemBase(inheritData.InheritedItem.ItemId);
        if (tbFromEquip == null || tbToEquip == null)
        {
            return;
        }

        if (!GameUtils.CheckInheritType(tbFromEquip, tbToEquip))
        {
            return;
        }

        if (inheritData.IsDiamond)
        {
            if (inheritData.CostDiamond > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.DiamondRes);
                return;
            }
        }
        if (inheritData.IsGold)
        {
            if (inheritData.CostGold > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
                return;
            }
        }
        if (inheritData.IsEnchance && !CheckInheritEnchance())
        {
            return;
        }
        if (inheritData.IsAdd && !CheckInheritAppend())
        {
            return;
        }
        if (inheritData.IsExcellent && !CheckInheritExcellent())
        {
            return;
        }
        SmritiEquipConfirm();
    }

    public void OnClickEquipInheritedItem()
    {
        RefreshInheritedItem(mEmptyBagItem);
    }

    public void OnClickEquipInheritItem()
    {
        RefreshInheritItem(mEmptyBagItem);
    }

    public void OnClickEquipOperate(IEvent ievent)
    {
        var e = ievent as EquipOperateClick;
        switch (e.OperateType)
        {
            case 0:
            {
                OnClickEquipEnchance();
            }
                break;
            case 1:
            {
                OnClickEquipAppend();
            }
                break;
            case 20:
            {
                OnClickExcellentReset();
            }
                break;
            case 21:
            {
                OnClickExcellentResetAffirm(1);
            }
                break;
            case 22:
            {
                OnClickExcellentResetAffirm(0);
            }
                break;
            case 3:
            {
                OnClickSuperExcellent();
            }
                break;
            case 4:
            {
                OnClickEquipInherit();
            }
                break;
            case 41:
            {
                OnClickEquipInheritItem();
            }
                break;
            case 42:
            {
                OnClickEquipInheritedItem();
            }
                break;
            case 43:
            {
                OnClickTips(true);
            }
                break;
            case 44:
            {
                OnClickTips(false);
            }
                break;
            case 100:
            {
                var count = DataModel.OperateTypes.Count;
                for (var i = 0; i < count; i++)
                {
                    DataModel.OperateTypes[i] = false;
                }
                if (e.Index >= 0 && e.Index < count)
                {
                    DataModel.OperateTypes[e.Index] = true;
                }
                RefreshItemData(mBagItemData, -1, true);
            }
                break;
        }
    }

    //--------------------------------------------------------------------Excellent------------------
    public void OnClickExcellentReset()
    {
        var playerData = PlayerDataManager.Instance.PlayerDataModel;
        var excellentReset = DataModel.EquipExcellentRestData;
        var itemData = excellentReset.ExcellentItem;
        if (itemData.ItemId == -1)
        {
            //请选择物品
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270084));
            return;
        }
        var tbItemBase = Table.GetItemBase(itemData.ItemId);
        if (tbItemBase == null)
        {
            return;
        }
        excellentReset.EquipId = tbItemBase.Exdata[0];
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
        if (tbEquip == null)
        {
            return;
        }
        var tbEquipExcellent = Table.GetEquipExcellent(tbEquip.Ladder);
        if (tbEquipExcellent == null)
        {
            return;
        }
        if (excellentReset.AttrInfos[0].Type == -1)
        {
            GameUtils.ShowHintTip(100000493);
            return;
        }
        if (tbEquipExcellent.GreenMoney > playerData.Bags.Resources.Gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
            return;
        }
        //if (tbEquipExcellent.GreenItemCount > PlayerDataManager.Instance.GetItemCount(tbEquipExcellent.GreenItemId))
        //{
        //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
        //    PlayerDataManager.Instance.ShowItemInfoGet(tbEquipExcellent.GreenItemId);
        //    return;
        //}

        if (!GameUtils.CheckEnoughItems(tbEquipExcellent.GreenItemId, tbEquipExcellent.GreenItemCount))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            return;
        }

        if (tbItemBase.CanTrade == 1 && itemData.Exdata.Binding != 1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 210117, "",
                () => { NetManager.Instance.StartCoroutine(ExcellentResetEquipCoroutine()); });
            return;
        }
        NetManager.Instance.StartCoroutine(ExcellentResetEquipCoroutine());
    }

    //洗练结构的操作，0：取消，1：确定
    public void OnClickExcellentResetAffirm(int ret)
    {
        NetManager.Instance.StartCoroutine(ExcellentResetOkCoroutine(ret));
    }

    //--------------------------------------------------------------------Super------------------
    public void OnClickSuperExcellent()
    {
        var playerData = PlayerDataManager.Instance.PlayerDataModel;
        var excellentData = DataModel.EquipSuperExcellentData;
        var tbEquip = Table.GetEquipBase(excellentData.EquipId);
        if (tbEquip == null)
        {
            return;
        }
        var tbItem = Table.GetItemBase(excellentData.ExcellentItem.ItemId);
        if (tbItem == null)
        {
            return;
        }
        if (excellentData.AttributeInfos[0].Type == -1)
        {
            GameUtils.ShowHintTip(100000495);
            return;
        }

        var ladder = tbEquip.Ladder;
        var tbExcellent = Table.GetEquipExcellent(ladder);
        if (tbExcellent == null)
        {
            return;
        }
        if (excellentData.LockMoney > playerData.Bags.Resources.Gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
            return;
        }
        //if (excellentData.LockItemCount > PlayerDataManager.Instance.GetItemCount(tbExcellent.LockId))
        //{
        //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
        //    PlayerDataManager.Instance.ShowItemInfoGet(tbExcellent.LockId);
        //    return;
        //}
        //if (tbExcellent.ItemCount > PlayerDataManager.Instance.GetItemCount(tbExcellent.ItemId))
        //{
        //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
        //    PlayerDataManager.Instance.ShowItemInfoGet(tbExcellent.ItemId);
        //    return;
        //}
        var items = new Dictionary<int, int>();
        items[tbExcellent.LockId] = excellentData.LockItemCount;
        items[tbExcellent.ItemId] = tbExcellent.ItemCount;
        if (!GameUtils.CheckEnoughItems(items))
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            return;
        }

        var lockList = new List<int>();
        for (var i = 0; i < 6; i++)
        {
            lockList.Add(excellentData.LockList[i] != true ? 0 : 1);
        }
        if (tbItem.CanTrade == 1 && excellentData.ExcellentItem.Exdata.Binding != 1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 210117, "",
                () => { NetManager.Instance.StartCoroutine(SuperExcellentEquipCoroutine(lockList)); });
            return;
        }
        NetManager.Instance.StartCoroutine(SuperExcellentEquipCoroutine(lockList));
    }

    private void OnClickTips(bool isOpen)
    {
        for (var i = 0; i < DataModel.OperateTypes.Count; i++)
        {
            if (DataModel.OperateTypes[i])
            {
                DataModel.Tips[i] = isOpen;
                break;
            }
        }
    }

    //监听是否选择提高成功率
    public void OnEnchanceChange(object sender, PropertyChangedEventArgs e)
    {
        if (DataModel.OperateTypes[0])
        {
            if (e.PropertyName == "IsSuccessRate")
            {
                //通过反向绑定变量IsSuccessRate，如果选择则替换成对应的强化id
                if (DataModel.EquipEnchanceData.IsSuccessRate)
                {
                    var tbEnchance = Table.GetEquipBlessing(DataModel.EquipEnchanceData.NowLevel);
                    DataModel.EquipEnchanceData.EnchanceId = tbEnchance.SpecialId;
                }
                else
                {
                    DataModel.EquipEnchanceData.EnchanceId = DataModel.EquipEnchanceData.NowLevel;
                }
            }
        }
    }

    public void OnEquipCellSwap(IEvent ievent)
    {
        if (mLastType == 5)
        {
            return;
        }
        var inheritData = DataModel.EquipInheritData;
        var inheritItem = inheritData.InheritItem;
        var inheritedItem = inheritData.InheritedItem;
        RefreshInheritItem(inheritedItem);
        RefreshInheritedItem(inheritItem);
    }

    //监听传承的内容
    public void OnInheritChange(object sender, PropertyChangedEventArgs e)
    {
        if (DataModel.OperateTypes[4] &&
            (e.PropertyName == "IsEnchance"
             || e.PropertyName == "IsAdd"
             || e.PropertyName == "IsExcellent"))
        {
            RefreshInheritCost();
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

    //监听属性锁定数量的变化，影响金钱消耗
    public void OnSuperExcellentChange(object sender, PropertyChangedEventArgs e)
    {
        if (DataModel.OperateTypes[3])
        {
            RefreshSuperExcelletCost();
        }
    }

    private void OnVipLevelChanged(IEvent ievent)
    {
        var tbVip = PlayerDataManager.Instance.TbVip;
        DataModel.EquipEnchanceData.EnhanceVipAdd = tbVip.EnhanceRatio;
    }

    //根据传入的物品，生成显示数据
    public void RefreshAppend(object data)
    {
        var itemData = data as BagItemDataModel;
        var appendData = DataModel.EquipAppendData;
        appendData.AppendItem = itemData;
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
        var itemId = itemData.ItemId;
        if (itemId == -1)
        {
            ResetAppend();
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
        appendData.AttributeData.Type = tbEquip.AddAttrId;
        appendData.AttributeData.Value = itemData.Exdata[1];
        appendData.MaxAppendValue = tbEquip.AddAttrMaxValue;
        appendData.EquipId = tbEquip.Id;
        appendData.AppendId = tbEquip.AddIndexID;
        if (appendData.AppendId == -1)
        {
            appendData.IsMaxValue = 1;
            return;
        }
        var tbAppdend = Table.GetEquipAdditional1(appendData.AppendId);
        if (tbAppdend == null)
        {
            return;
        }
        appendData.IsMaxValue = appendData.AttributeData.Value == tbEquip.AddAttrMaxValue ? 1 : 0;
        var addLevel = GetAdditionalTable1(tbAppdend, itemData.Exdata[1]);

        appendData.CostItemCount = Table.GetSkillUpgrading(tbAppdend.MaterialCount).GetSkillUpgradingValue(addLevel);
        appendData.CostMoney = Table.GetSkillUpgrading(tbAppdend.Money).GetSkillUpgradingValue(addLevel);
        if (appendData.IsMaxValue == 1)
        {
            appendData.AttributeData.Change = 0;
            appendData.AttributeData.ChangeEx = 0;
        }
        else
        {
            var minUp = Table.GetSkillUpgrading(tbAppdend.MinSection).GetSkillUpgradingValue(addLevel);
            var maxUp = Table.GetSkillUpgrading(tbAppdend.MaxSection).GetSkillUpgradingValue(addLevel);

            var minValue = minUp + appendData.AttributeData.Value;
            if (minValue > tbEquip.AddAttrMaxValue)
            {
                minValue = tbEquip.AddAttrMaxValue;
                appendData.AttributeData.Change = minValue;
                appendData.AttributeData.ChangeEx = 0;
            }
            else
            {
                appendData.AttributeData.Change = minValue;

                var maxValue = appendData.AttributeData.Value + maxUp;
                if (maxValue > tbEquip.AddAttrMaxValue)
                {
                    maxValue = tbEquip.AddAttrMaxValue;
                }
                appendData.AttributeData.ChangeEx = maxValue;
            }
        }
    }

    public void RefreshAppendNotice(int changeCount)
    {
        var bag = PlayerDataManager.Instance.GetBag((int) eBagType.Equip);
        var count = bag.Items.Count;
        for (var i = 0; i < count; i++)
        {
            var item = bag.Items[i];
            if (IsShowAppendNotice(item, changeCount))
            {
                PlayerDataManager.Instance.WeakNoticeData.NurturanceTotal = true;
                PlayerDataManager.Instance.WeakNoticeData.EquipTotal = true;
                PlayerDataManager.Instance.WeakNoticeData.Additional = true;
                break;
            }
        }
    }

    //通过传进的物品数据，生成显示的数据
    public void RefreshEnchance(object data, bool reset = true)
    {
        var itemData = data as BagItemDataModel;
        var enchangeData = DataModel.EquipEnchanceData;
        enchangeData.EnchanceItem = itemData;
        var playerBags = PlayerDataManager.Instance.PlayerDataModel.Bags;
        if (itemData == null)
        {
            ResetEnchance();
            return;
        }
        var itemId = itemData.ItemId;
        if (itemId == -1)
        {
            ResetEnchance();
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
        enchangeData.EquipId = tbEquip.Id;
        enchangeData.NowLevel = itemData.Exdata[0];
        enchangeData.IsMaxLevel = enchangeData.NowLevel == tbEquip.MaxLevel ? 1 : 0;
        if (reset || enchangeData.IsMaxLevel == 1)
        {
            enchangeData.IsSpecialItem = false;
            
            enchangeData.IsSuccessRate = false;
            DataModel.EquipEnchanceData.IsSpecialItemShow = false;
        }
        var tbEnchance = Table.GetEquipBlessing(enchangeData.NowLevel);
        enchangeData.EnchanceId = enchangeData.IsSuccessRate ? tbEnchance.SpecialId : enchangeData.NowLevel;
        enchangeData.NextLevel = enchangeData.NowLevel + 1;
        for (var i = 0; i < 4; i++)
        {
            var nAttrId = tbEquip.BaseAttr[i];
            enchangeData.Attributes[i].Type = nAttrId;
            if (nAttrId != -1)
            {
                var v = GameUtils.GetBaseAttr(tbEquip, enchangeData.NowLevel, i, nAttrId);
                var nv = 0;
                if (enchangeData.IsMaxLevel != 1)
                {
                    nv = GameUtils.GetBaseAttr(tbEquip, enchangeData.NextLevel, i, nAttrId);
                }
                GameUtils.SetAttribute(enchangeData.Attributes, i, nAttrId, v, nv);
            }
        }
        
    }

    public void RefreshEquipBagStatus(BagItemDataModel bagItemData, BagItemDataModel bagItemData2 = null)
    {
        PlayerDataManager.Instance.GetBagItemFightPoint(bagItemData);
        if (bagItemData2 != null)
        {
            PlayerDataManager.Instance.GetBagItemFightPoint(bagItemData2);
        }
        PlayerDataManager.Instance.RefreshEquipBagStatus();
    }

    //根据传入的物品，生成显示数据
    public void RefreshExcelletReset(object data)
    {
        var itemData = data as BagItemDataModel;
        var excellentResetData = DataModel.EquipExcellentRestData;
        var itemId = itemData.ItemId;
        if (itemId == -1)
        {
            excellentResetData.EquipId = -1;
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        excellentResetData.EquipId = tbItemBase.Exdata[0];
        var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
        if (tbEquip == null)
        {
            return;
        }
        excellentResetData.ExcellentItem = itemData;
        var range = tbEquip.ExcellentAttrValue;
        var tbEnchant = Table.GetEquipEnchant(range);
        if (tbEnchant == null)
        {
            return;
        }
        var minRate = tbEquip.ExcellentValueMin;
        var maxRate = tbEquip.ExcellentValueMax;
        excellentResetData.HasChange = 0;
        var attrCount = 4;
        switch (tbItemBase.Quality)
        {
            case 0: //白
            {
                attrCount = 0;
            }
                break;
            case 1: //绿
            {
                attrCount = 2;
            }
                break;
            case 2: //蓝
            {
                attrCount = 3;
            }
                break;
        }
        for (var i = 0; i < 4; i++)
        {
            var attrid = tbEquip.ExcellentAttrId[i];
            var index = GameUtils.GetAttrIndex(attrid);
            if (index != -1 && i < attrCount)
            {
                excellentResetData.AttrInfos[i].Type = attrid;
                excellentResetData.AttrInfos[i].Value = itemData.Exdata[2 + i];
                excellentResetData.AttrInfos[i].MinValue = tbEnchant.Attr[index]*minRate/100;
                excellentResetData.AttrInfos[i].MaxValue = tbEnchant.Attr[index]*maxRate/100;
                if (itemData.Exdata[18 + i] != -1)
                {
                    excellentResetData.HasChange = 1;
                    excellentResetData.AttrInfos[i].Change = itemData.Exdata[18 + i] - itemData.Exdata[2 + i];
                }
                else
                {
                    excellentResetData.AttrInfos[i].Change = 0;
                }

                if (excellentResetData.AttrInfos[i].Change == 0
                    && excellentResetData.AttrInfos[i].Value == 0
                    || (excellentResetData.AttrInfos[i].Change + excellentResetData.AttrInfos[i].Value == 0))
                {
                    //excellentResetData.AttrInfos[i].Reset();
                    excellentResetData.AttrColors[i] = MColor.grey;
                }
                else
                {
                    excellentResetData.AttrColors[i] = MColor.green;
                }
            }
            else
            {
                excellentResetData.AttrInfos[i].Reset();
                excellentResetData.AttrColors[i] = MColor.grey;
            }
        }
    }

    public void RefreshInherit(object data, int index, bool isFromToggle = false)
    {
        var itemData = data as BagItemDataModel;
        var inheritData = DataModel.EquipInheritData;

        if (index == -1)
        {
            if (inheritData.InheritedItem.ItemId != -1 && inheritData.InheritItem.ItemId != -1)
            {
                RefreshInheritItem(itemData);
            }
            else
            {
                if (inheritData.InheritedItem.ItemId == -1)
                {
                    if (isFromToggle)
                    {
                        RefreshInheritedItem(mEmptyBagItem);
                    }
                    else
                    {
                        RefreshInheritedItem(itemData); 
                    }
                }
                else
                {
                    if (inheritData.InheritItem.ItemId == -1)
                    {
                        if (isFromToggle)
                        {
                            RefreshInheritItem(mEmptyBagItem);
                        }
                        else
                        {
                            RefreshInheritItem(itemData);
                        }
                    }
                }
            }
        }
        else
        {
            if (index == 0)
            {
                RefreshInheritItem(itemData);
            }
            else
            {
                RefreshInheritedItem(itemData);
            }
        }
    }

    public void RefreshShengJie()
    {
        if (mSmithyFrameController != null)
        {
            mSmithyFrameController.RefreshData(new SmithyFrameArguments
            {
                BuildingData = null
            });
        }

        //mEquipPackController.RefreshData(new EquipPackArguments { RefreshForEvoEquip = true });
    }

    //重新计算花费
    public void RefreshInheritCost()
    {
        var inheritData = DataModel.EquipInheritData;
        var itemdData = inheritData.InheritItem;
        if (itemdData.ItemId == -1)
        {
            return;
        }
        if (inheritData.IsEnchance)
        {
            var enchanceLv = itemdData.Exdata[0];
            var tbEnchanve = Table.GetEquipBlessing(enchanceLv);
            inheritData.CostGold = tbEnchanve.SmritiMoney;
            inheritData.CostDiamond = tbEnchanve.SmritiGold;
            return;
        }
        if (inheritData.IsAdd)
        {
            var tbEquip = Table.GetEquipBase(itemdData.ItemId);
            if (tbEquip == null)
            {
                return;
            }
            var tbAdditional = Table.GetEquipAdditional1(tbEquip.AddIndexID);
            if (tbAdditional == null)
            {
                return;
            }
            var AddLevel = GetAdditionalTable1(tbAdditional, itemdData.Exdata[1]);
            //金钱检查
            inheritData.CostGold = Table.GetSkillUpgrading(tbAdditional.SmritiMoney).GetSkillUpgradingValue(AddLevel);
            inheritData.CostDiamond =
                Table.GetSkillUpgrading(tbAdditional.SmritiDiamond).GetSkillUpgradingValue(AddLevel);
            return;
        }
        if (inheritData.IsExcellent)
        {
            var tbItemBase = Table.GetItemBase(itemdData.ItemId);
            var tbEquip = Table.GetEquipBase(tbItemBase.Exdata[0]);
            var tbExcellent = Table.GetEquipExcellent(tbEquip.Ladder);
            inheritData.CostGold = tbExcellent.SmritiMoney;
            inheritData.CostDiamond = tbExcellent.SmritiGold;
        }
    }

    //设置继承目的物品
    public void RefreshInheritedItem(BagItemDataModel bagItem)
    {
        var inheritData = DataModel.EquipInheritData;
        if (bagItem.ItemId != -1 && inheritData.InheritItem == bagItem)
        {
            mEquipPackController.CallFromOtherClass("RefreshForEquipInherit",
                new object[2] {inheritData.InheritItem, inheritData.InheritedItem});
        }
        else
        {
            inheritData.InheritedItem = bagItem;
            //mBagItemData = bagItem;
            SetItemData(bagItem);
            mEquipPackController.CallFromOtherClass("RefreshForEquipInherit",
                new object[2] {inheritData.InheritItem, inheritData.InheritedItem});
        }
    }

    //设置继承来源物品
    public void RefreshInheritItem(BagItemDataModel bagItem)
    {
        var inheritData = DataModel.EquipInheritData;
        if (inheritData.InheritedItem.ItemId != -1)
        {
            SetItemData(inheritData.InheritedItem);
//            mBagItemData = inheritData.InheritedItem;
        }
        if (bagItem.ItemId != -1 && inheritData.InheritItem == bagItem)
        {
            mEquipPackController.CallFromOtherClass("RefreshForEquipInherit",
                new object[2] {inheritData.InheritItem, inheritData.InheritedItem});
            return;
        }
        inheritData.InheritItem = bagItem;
        mEquipPackController.CallFromOtherClass("RefreshForEquipInherit",
            new object[2] {inheritData.InheritItem, inheritData.InheritedItem});
        if (bagItem.ItemId == -1)
        {
            ResetInheritData();
        }
        else
        {
            RefreshInheritCost();
        }
    }

    private void SetItemData(BagItemDataModel data)
    {
        if (null != mBagItemData)
        {
            mBagItemData.IsChoose = false;
        }
        mBagItemData = data;
        if (data != null)
        {
            mBagItemData.IsChoose = true;
        }
    }
    public void RefreshItemData(BagItemDataModel data, int index = -1, bool isFromToggle = false)
    {
        if (data != null)
        {
            SetItemData(data);
//            mBagItemData = data;
        }

        for (var i = 0; i < 6; i++)
        {
            if (DataModel.OperateTypes[i])
            {
                if (i != 4 && mLastType == 4)
                {
                    mEquipPackController.CallFromOtherClass("Refresh", null);
                }
                //if ((i != 4 && mLastType == 4) || mBagItemData == null)
                if (i != 5 && 5 == mLastType)
                {
                    mEquipPackController.CallFromOtherClass("Refresh", null);
                }

                if (mBagItemData == null || mBagItemData.ItemId == -1)
                {
                    if (EquipPackDataModel != null)
                    {
                        if (EquipPackDataModel.EquipItems.Count > 0)
                        {
                            SetItemData(EquipPackDataModel.EquipItems[0].BagItemData);
//                            mBagItemData = EquipPackDataModel.EquipItems[0].BagItemData;
                        }
                        else
                        {
                            if (EquipPackDataModel.PackItems.Count > 0)
                            {
                                SetItemData(EquipPackDataModel.EquipItems[0].BagItemData);
                                //mBagItemData = EquipPackDataModel.PackItems[0].BagItemData;
                            }
                        }
                    }
                    if (mBagItemData == null)
                    {
                        SetItemData(mEmptyBagItem);
//                        mBagItemData = mEmptyBagItem;
                    }
                    mEquipPackController.RefreshData(new EquipPackArguments {DataModel = mBagItemData});
                }
                switch (i)
                {
                    case 0:
                    {
                        RefreshEnchance(mBagItemData);
                    }
                        break;
                    case 1:
                    {
                        RefreshAppend(mBagItemData);
                    }
                        break;
                    case 2:
                    {
                        RefreshExcelletReset(mBagItemData);
                    }
                        break;
                    case 3:
                    {
                        ResetSuperExcellet();
                        RefreshSuperExcellet(mBagItemData);
                    }
                        break;
                    case 4:
                    {
                        if (isFromToggle)
                        {
                            RefreshInherit(mBagItemData, index, isFromToggle);
                        }
                        else
                        {
                            RefreshInherit(mBagItemData, index);
                        }
                    }
                        break;
                    case 5:
                    {
                        RefreshShengJie();
                    }
                    break;
                }
                if (i != 4)
                {
                    if (DataModel.EquipInheritData.InheritItem.ItemId != -1)
                    {
                        DataModel.EquipInheritData.InheritItem = mEmptyBagItem;
                    }
                    if (DataModel.EquipInheritData.InheritedItem.ItemId != -1)
                    {
                        DataModel.EquipInheritData.InheritedItem = mEmptyBagItem;
                    }
                    mEquipPackController.CallFromOtherClass("RefreshSelectFlag", new object[1] {mBagItemData});
                }
                mLastType = i;
            }
        }
    }

    //根据传入的物品，生成显示数据
    public void RefreshSuperExcellet(object data)
    {
        var itemData = data as BagItemDataModel;
        if (itemData == null)
        {
            return;
        }
        var excellentData = DataModel.EquipSuperExcellentData;
        excellentData.ExcellentItem = itemData;
        var itemId = itemData.ItemId;
        if (itemId == -1)
        {
            excellentData.EquipId = -1;
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        if (tbItemBase == null)
        {
            return;
        }
        excellentData.EquipId = tbItemBase.Exdata[0];
        var tbEquip = Table.GetEquipBase(excellentData.EquipId);
        if (tbEquip == null)
        {
            return;
        }
        var range = tbEquip.RandomAttrValue;
        var tbEnchant = Table.GetEquipEnchant(range);
        if (tbEnchant == null)
        {
            return;
        }
        var minRate = tbEquip.RandomValueMin;
        var maxRate = tbEquip.RandomValueMax;
        for (var i = 0; i < 6; i++)
        {
            var attrId = itemData.Exdata[6 + i];
            var attrValue = itemData.Exdata[12 + i];
            if (attrId != -1)
            {
                excellentData.AttributeInfos[i].Type = attrId;
                var index = GameUtils.GetAttrIndex(attrId);
                excellentData.AttributeInfos[i].Value = attrValue*100;
                excellentData.AttributeInfos[i].MinValue = tbEnchant.Attr[index]*minRate/100*100;
                excellentData.AttributeInfos[i].MaxValue = tbEnchant.Attr[index]*maxRate/100*100;
            }
            else
            {
                excellentData.ShowList[i] = false;
                excellentData.AttributeInfos[i].Reset();
            }
        }
        RefreshSuperExcelletCost();
    }

    public void RefreshSuperExcelletCost()
    {
        var excellentData = DataModel.EquipSuperExcellentData;
        var lockNum = 0;
        var attrNum = 0;
        for (var i = 0; i < 6; i++)
        {
            if (excellentData.AttributeInfos[i].Type != -1)
            {
                attrNum++;
            }
            if (excellentData.LockList[i])
            {
                lockNum++;
            }
        }

        if (lockNum == attrNum - 1)
        {
            for (var i = 0; i < 6; i++)
            {
                if (!excellentData.LockList[i])
                {
                    excellentData.ShowList[i] = false;
                }
                else
                {
                    excellentData.ShowList[i] = true;
                }
            }
        }
        else
        {
            for (var i = 0; i < 6; i++)
            {
                if (excellentData.AttributeInfos[i].Type != -1)
                {
                    excellentData.ShowList[i] = true;
                }
                else
                {
                    excellentData.ShowList[i] = false;
                }
            }
        }

        var tbEquip = Table.GetEquipBase(excellentData.EquipId);
        var ladder = tbEquip.Ladder;
        var tbExcellent = Table.GetEquipExcellent(ladder);
        excellentData.LockItemCount = lockNum > 0 ? tbExcellent.LockCount[lockNum - 1] : 0;

        excellentData.LockMoney = tbExcellent.Money[lockNum];
        excellentData.TotalLock = lockNum;
    }

    //重置追加显示
    public void ResetAppend()
    {
        var appendData = DataModel.EquipAppendData;
        appendData.AppendId = -1;
        appendData.AttributeData.Reset();
        appendData.MaxAppendValue = 0;
        appendData.IsMaxValue = 0;
        appendData.CostMoney = 0;
        appendData.CostItemCount = 0;
    }

    //重置界面显示
    public void ResetEnchance()
    {
        var enchangeData = DataModel.EquipEnchanceData;
        enchangeData.EnchanceId = -1;
        enchangeData.NextLevel = 0;
        enchangeData.IsMaxLevel = 0;

        for (var i = 0; i < 4; i++)
        {
            enchangeData.Attributes[i].Reset();
        }
    }

    public void ResetInheritData()
    {
        var inheritData = DataModel.EquipInheritData;
        inheritData.CostGold = 0;
        inheritData.CostDiamond = 0;
    }

    public void ResetSuperExcellet()
    {
        var excellentData = DataModel.EquipSuperExcellentData;
        for (var i = 0; i < 6; i++)
        {
            excellentData.LockList[i] = false;
            excellentData.ShowList[i] = true;
        }
    }

    //继承网络包逻辑
    private void SmritiEquipConfirm()
    {
        var inheritData = DataModel.EquipInheritData;
        var tbItem = Table.GetItemBase(inheritData.InheritedItem.ItemId);
        if (tbItem == null)
        {
            return;
        }
        if (tbItem.CanTrade == 1 && inheritData.InheritedItem.Exdata.Binding != 1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 210117, "",
                () => { NetManager.Instance.StartCoroutine(SmritiEquipCoroutine()); });
            return;
        }
        NetManager.Instance.StartCoroutine(SmritiEquipCoroutine());
    }

    public IEnumerator SmritiEquipCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var inheritData = DataModel.EquipInheritData;
            var smritiType = 0;
            if (inheritData.IsAdd)
            {
                smritiType = 1;
            }
            else if (inheritData.IsExcellent)
            {
                smritiType = 2;
            }
            var costType = 0;
            costType = inheritData.IsGold ? 0 : 1;

            var inheritItem = inheritData.InheritItem;
            var inheritedItem = inheritData.InheritedItem;

            var tbFromEquip = Table.GetEquipBase(inheritItem.ItemId);
            var tbToEquip = Table.GetEquipBase(inheritedItem.ItemId);
            if (tbFromEquip == null || tbToEquip == null)
            {
                yield break;
            }
            var msg = NetManager.Instance.SmritiEquip(smritiType, costType, inheritItem.BagId, inheritItem.Index,
                inheritedItem.BagId, inheritedItem.Index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var e = new EquipUiNotifyLogic(1);
                    EventDispatcher.Instance.DispatchEvent(e);
                    //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220008));
                    if (inheritedItem.Exdata.Binding != 1)
                    {
                        inheritedItem.Exdata.Binding = 1;
                    }
                    if (inheritData.IsEnchance)
                    {
                        if (inheritItem.Exdata[0] > tbToEquip.MaxLevel)
                        {
                            inheritedItem.Exdata[0] = tbToEquip.MaxLevel;
                        }
                        else
                        {
                            inheritedItem.Exdata[0] = inheritItem.Exdata[0];
                        }
                        inheritItem.Exdata[0] = 0;
                    }
                    else if (inheritData.IsAdd)
                    {
                        //if (appendValue1 > tbToEquip.AddAttrMaxValue)
                        //{
                        //    appendValue1 = tbToEquip.AddAttrMaxValue;
                        //}
                        inheritedItem.Exdata[1] = msg.Response;
                        inheritItem.Exdata[1] = 0;
                    }
                    else if (inheritData.IsExcellent)
                    {
                        var range = tbToEquip.ExcellentAttrValue;
                        var tbEnchant = Table.GetEquipEnchant(range);
                        if (tbEnchant == null)
                        {
                            yield break;
                        }
                        var maxRate = tbToEquip.ExcellentValueMax;

                        for (var i = 0; i < 4; i++)
                        {
                            var attrid = tbToEquip.ExcellentAttrId[i];
                            var index = GameUtils.GetAttrIndex(attrid);
                            if (index != -1)
                            {
                                if (attrid != tbFromEquip.ExcellentAttrId[i])
                                {
                                    if (inheritedItem.Exdata[2 + i] < tbEnchant.Attr[index]*maxRate/100)
                                    {
                                        inheritedItem.Exdata[2 + i] = tbEnchant.Attr[index]*maxRate/100;
                                    }
                                    else
                                    {
                                        inheritedItem.Exdata[2 + i] = inheritItem.Exdata[2 + i];
                                    }
                                }
                            }
                            inheritItem.Exdata[2 + i] = 0;
                        }
                    }

                    RefreshEquipBagStatus(inheritItem, inheritedItem);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.DiamondNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_EquipNoAdditionalNoSmrit)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270262));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("SmritiEquip..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("SmritiEquip..................." + msg.State);
            }
        }
    }

    //随灵网络包逻辑
    public IEnumerator SuperExcellentEquipCoroutine(List<int> lockList)
    {
        using (new BlockingLayerHelper(0))
        {
            var excellentData = DataModel.EquipSuperExcellentData;
            var itemData = excellentData.ExcellentItem;
            var array = new Int32Array();
            array.Items.AddRange(lockList);
            var msg = NetManager.Instance.SuperExcellentEquip(itemData.BagId, itemData.Index, array);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                var e = new EquipUiNotifyLogic(1);
                EventDispatcher.Instance.DispatchEvent(e);
                //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(220014));
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    for (var i = 0; i < 6; i++)
                    {
                        if (msg.Response.AttrId.Count > i + 1
                            && msg.Response.AttrValue.Count > i + 1)
                        {
                            itemData.Exdata[6 + i] = msg.Response.AttrId[i];
                            itemData.Exdata[12 + i] = msg.Response.AttrValue[i];
                        }
                    }
                    RefreshSuperExcellet(itemData);

                    RefreshEquipBagStatus(itemData);
                    if (itemData.Exdata.Binding != -1)
                    {
                        itemData.Exdata.Binding = 1;
                    }

                    PlatformHelper.UMEvent("EquipSuiLing", itemData.BagId.ToString());
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("SuperExcellentEquip..................." + msg.ErrorCode);
                }
            }
            else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
            }
            else if (msg.ErrorCode == (int) ErrorCodes.DiamondNotEnough)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
            }
            else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
            }
            else
            {
                Logger.Debug("SuperExcellentEquip..................." + msg.State);
            }
        }
    }

    public void CleanUp()
    {
        if (DataModel != null)
        {
            DataModel.EquipEnchanceData.PropertyChanged -= OnEnchanceChange;
            DataModel.EquipSuperExcellentData.LockList.PropertyChanged -= OnSuperExcellentChange;
            DataModel.EquipInheritData.PropertyChanged -= OnInheritChange;
        }

        DataModel = new EquipUIDataModel();
        DataModel.OperateTypes[0] = true;

        DataModel.EquipEnchanceData.PropertyChanged += OnEnchanceChange;
        DataModel.EquipSuperExcellentData.LockList.PropertyChanged += OnSuperExcellentChange;
        DataModel.EquipInheritData.PropertyChanged += OnInheritChange;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
        EventDispatcher.Instance.AddEventListener(EquipCellSelect.EVENT_TYPE, OnChangeEquipCell);
        EventDispatcher.Instance.AddEventListener(EquipCellSwap.EVENT_TYPE, OnEquipCellSwap);

        if (mEquipPackController != null)
        {
            mEquipPackController.OnShow();
        }

        if (mSmithyFrameController != null)
        {
            mSmithyFrameController.OnShow();
        }
        
    }

    public void Close()
    {
        EventDispatcher.Instance.RemoveEventListener(EquipCellSelect.EVENT_TYPE, OnChangeEquipCell);
        EventDispatcher.Instance.RemoveEventListener(EquipCellSwap.EVENT_TYPE, OnEquipCellSwap);
        //DataModel.ToggleSelect = -1;
        //mLastType = -1;

        if (mSmithyFrameController != null)
        {
            mSmithyFrameController.Close();
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "EquipPack")
        {
            return EquipPackDataModel;
        }
        return DataModel;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as EquipUIArguments;
        //mBagItemData = null;
        SetItemData(null);
        var openId = 0;
        BagItemDataModel itemData = null;
        if (args != null)
        {
            if (args.Tab < 0 || args.Tab >= 6)
            {
                openId = 0;
            }
            else
            {
                openId = args.Tab;
            }
            if (args.Data != null)
            {
                if (args.ResourceType == 0) //背包打开
                {
                    itemData = PlayerDataManager.Instance.GetItem(args.Data.BagId, args.Data.Index);
                }
                else if (args.ResourceType == 1) //身上打开
                {
                    itemData = args.Data;
                }
            }
        }

        for (var i = 0; i < 6; i++)
        {
            DataModel.OperateTypes[i] = false;
        }

        for (var i = 0; i < 6; i++)
        {
            if (i == openId)
            {
                DataModel.OperateTypes[i] = true;
                break;
            }
        }
        for (var i = 0; i < DataModel.Tips.Count; i++)
        {
            DataModel.Tips[i] = false;
        }

        mEquipPackController.CallFromOtherClass("Refresh", null);
        DataModel.ToggleSelect = openId;
        RefreshItemData(itemData);
        mLastType = DataModel.ToggleSelect;
        PlayerDataManager.Instance.RefreshEquipBagStatus();
    }

    public FrameState State { get; set; }
}