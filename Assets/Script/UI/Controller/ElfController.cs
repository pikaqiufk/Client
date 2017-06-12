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

public class ElfController : IControllerBase
{
    public ElfController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(ElfExdataUpdate.EVENT_TYPE, OnUpdateExData);
        EventDispatcher.Instance.AddEventListener(ElfCellClickEvent.EVENT_TYPE, OnElfCellClick);
        EventDispatcher.Instance.AddEventListener(ElfCell1ClickEvent.EVENT_TYPE, OnElfCell1Click);
        EventDispatcher.Instance.AddEventListener(ElfFlyOverEvent.EVENT_TYPE, OnElfFlyOver);
        EventDispatcher.Instance.AddEventListener(ElfOperateEvent.EVENT_TYPE, OnElfOperate);
        EventDispatcher.Instance.AddEventListener(ElfReplaceEvent.EVENT_TYPE, OnElfReplace);
        EventDispatcher.Instance.AddEventListener(ElfGetOneShowEvent.EVENT_TYPE, OnClickGetShow);
        EventDispatcher.Instance.AddEventListener(ElfShowCloseEvent.EVENT_TYPE, OnClickShowClose);
        EventDispatcher.Instance.AddEventListener(ElfGetDrawResult.EVENT_TYPE, GetDrawResult);
        EventDispatcher.Instance.AddEventListener(ElfOneDrawInfoEvent.EVENT_TYPE, ShowOneDrawInfo);
        EventDispatcher.Instance.AddEventListener(UIEvent_ElfShowDrawGetEvent.EVENT_TYPE, ShowDrawGet);
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChanged);
    }

    private int BagItemCount;
    public ElfDataModel DataModel;
    private readonly ElfItemDataModelComparer ElfItemComparer = new ElfItemDataModelComparer();
    private bool IsOneDraw;
    private readonly int MaxElfFightCount = 3;
    private bool mIsSetOffset;
    private Coroutine mTimeCoroutine;
    public int SelectElfIndex;

    private int CheckElfType(int elfId, int targetIndex)
    {
        var sameIndex = 0;
        var nowFightCount = DataModel.FightElfCount;
        if (targetIndex > nowFightCount)
        {
            return 0;
        }
        var tbElf = Table.GetElf(elfId);
        if (tbElf == null)
        {
            return 0;
        }
        for (var i = 0; i < nowFightCount; i++)
        {
            if (i == targetIndex)
            {
                continue;
            }
            var id = DataModel.Formations[i].ElfData.ItemId;
            if (id == -1)
            {
                continue;
            }
            var tb = Table.GetElf(id);
            if (tb == null)
            {
                continue;
            }
            if (tbElf.ElfType == tb.ElfType)
            {
                return i;
            }
        }
        return -1;
    }

    private bool ElfIdInFormation(int itemId)
    {
        for (var i = 0; i < MaxElfFightCount; i++)
        {
            var item = DataModel.Formations[i].ElfData;
            if (item.ItemId == itemId)
            {
                return true;
            }
        }
        return false;
    }

    private void ElfOperate(int type)
    {
        ElfOperate(DataModel.SelectElf, type);
    }

    private void ElfOperate(ElfItemDataModel elfData, int type, int targetIndex = -1)
    {
        var data = elfData;
        if (data.ItemId == -1)
        {
            return;
        }
        var tbItem = Table.GetItemBase(data.ItemId);
        var tbElf = Table.GetElf(tbItem.Exdata[0]);
        var nowFightCount = DataModel.FightElfCount;
        switch (type)
        {
            case 0:
            {
                if (elfData.Index < 0)
                {
                    return;
                }
                if (elfData.Index >= MaxElfFightCount)
                {
                    //精灵不在战斗
                    var e = new ShowUIHintBoard(240304);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
            }
                break;
            case 1:
            {
                if (nowFightCount == 1 || targetIndex >= nowFightCount)
                {
                    GameUtils.ShowHintTip(240311);
                    return;
                }
                if (targetIndex == -1)
                {
                    for (var i = 1; i < nowFightCount; i++)
                    {
                        if (DataModel.Formations[i].ElfData.ItemId == -1)
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                }
                if (elfData.Index == targetIndex)
                {
                    return;
                }
                if (targetIndex == -1)
                {
                    targetIndex = 1;
                }
                if (targetIndex < 1 || targetIndex > 2)
                {
                    return;
                }
            }
                break;
            case 2:
            {
                if (elfData.Index == 0)
                {
                    //"上阵精灵已在展示中"
                    var e = new ShowUIHintBoard(240305);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                if (elfData.Index >= MaxElfFightCount)
                {
                    if (CheckElfType(elfData.ItemId, 0) != -1)
                    {
                        GameUtils.ShowHintTip(240312);
                        return;
                    }
                }
            }
                break;
            case 4:
            {
                var tbLevel = Table.GetLevelData(DataModel.FormationLevel);
                if (tbLevel.FightingWayExp > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.ElfPiece)
                {
//阵法经验不足
                    GameUtils.ShowHintTip(200002907);
                    return;
                }
            }
                break;
            case 5:
            {
                if (elfData.Exdata.Level == tbElf.MaxLevel)
                {
                    //精灵已达到最大的等级
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(240306));
                    return;
                }

                if (elfData.LvExp > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.ElfPiece)
                {
                    //没有足够精魂可供升级
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(240307));
                    return;
                }
            }
                break;
            case 6:
            {
                if (elfData.Index <= 2)
                {
                    //不能分解出战精灵
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(240308));
                    return;
                }

                var tbLevel = Table.GetLevelData(data.Exdata.Level);
                var count = (int) (tbElf.ResolveCoef[0]/100.0f*tbLevel.ElfResolveValue + tbElf.ResolveCoef[1]);

                if (tbItem.Quality >= 3 || elfData.Exdata.Level > 1)
                {
                    //是否确认分解,获得{0}精魄
                    var str = string.Format(GameUtils.GetDictionaryText(240309), count);
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
                        () => { NetManager.Instance.StartCoroutine(ElfOperateCoroutine(type, elfData, 0)); });
                    return;
                }
            }
                break;
            case 7:
            {
                var items = new Dictionary<int, int>();
                items.modifyValue(DataModel.StarCostResId, DataModel.StarCostResCount);
                for (var i = 0; i < DataModel.StarCostItems.Count; ++i)
                {
                    if (DataModel.StarCostItems[i].ItemId != -1)
                        items.modifyValue(DataModel.StarCostItems[i].ItemId, DataModel.StarCostItems[i].Count);
                }
                if (!GameUtils.CheckEnoughItems(items))
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                    //PlayerDataManager.Instance.ShowItemInfoGet(tbEnchance.NeedItemId[i]);
                    return;
                }
            }
                break;
        }
        NetManager.Instance.StartCoroutine(ElfOperateCoroutine(type, elfData, targetIndex));
    }

    private IEnumerator ElfOperateCoroutine(int type, ElfItemDataModel elfData, int targetIndex)
    {
        if (elfData == null)
        {
            yield break;
        }
        using (new BlockingLayerHelper(0))
        {
            var index = elfData.Index;
            var tbElf = Table.GetElf(elfData.ItemId);
            var msg = NetManager.Instance.ElfOperate(index, type, targetIndex);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    switch (type)
                    {
                        case 0:
                        {
                            var retIndex = GetFreeIndex();
                            EventDispatcher.Instance.DispatchEvent(new ElfFlyEvent(elfData.Index, retIndex, true));
                        }
                            break;
                        case 1:
                        {
                            var elfData1 = DataModel.ElfBag[targetIndex];
                            var fromIdx = elfData.Index;
                            var toIdx = targetIndex;
                            var needCallBack = (elfData1.ItemId != -1 && fromIdx < MaxElfFightCount) ||
                                               fromIdx < MaxElfFightCount;
                            if (!needCallBack)
                            {
                                MoveElfBag(fromIdx, toIdx);
                            }
                            EventDispatcher.Instance.DispatchEvent(new ElfFlyEvent(fromIdx, toIdx, needCallBack));
                        }
                            break;
                        case 2:
                        {
                            var elfData1 = DataModel.ElfBag[targetIndex];
                            var fromIdx = elfData.Index;
                            var toIdx = targetIndex;
                            var needCallBack = (elfData1.ItemId != -1 && fromIdx < MaxElfFightCount) ||
                                               fromIdx < MaxElfFightCount;
                            if (!needCallBack)
                            {
                                MoveElfBag(fromIdx, toIdx);
                            }
                            EventDispatcher.Instance.DispatchEvent(new ElfFlyEvent(fromIdx, toIdx, needCallBack));

                            PlatformHelper.UMEvent("PetFight", tbElf.ElfName, elfData.Exdata.Level.ToString());
                        }
                            break;
                        case 4: //Formation
                        {
                            EventDispatcher.Instance.DispatchEvent(new FormationLevelupEvent());

                            var level = (int) msg.Response;
                            UpdateFormationLevel(level);
                        }
                            break;
                        case 5: //Enchance
                        {
                            EventDispatcher.Instance.DispatchEvent(new ElfLevelupEvent());

                            elfData.Exdata.Level = (int) msg.Response;
                            var tbLevel = Table.GetLevelData(elfData.Exdata.Level);
                            elfData.LvExp = tbLevel.ElfExp*tbElf.ResolveCoef[0]/100;
                            RefreshElfAttribute(DataModel.SelectElf);
                            PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Elf);

                            //遍历所有宠物 确定当前升级是否为等级最高的宠物
                            bool isThisMaxLevel = true;
                            foreach (var item in DataModel.Items)
                            {
                                if (item.Exdata.Level >= elfData.Exdata.Level && item.Index != elfData.Index)
                                {
                                    isThisMaxLevel = false;
                                    break;
                                }
                            }
                            if (isThisMaxLevel)
                            {
                                PlatformHelper.UMEvent("PetLevel", elfData.Exdata.Level.ToString());
                            }
                        }
                            break;
                        case 6: //Resolve
                        {
                            var count = msg.Response;
                            //分解成功，获得{0}精魄
                            var str = string.Format(GameUtils.GetDictionaryText(240310), count);
                            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
                            mIsSetOffset = true;

                            PlatformHelper.UMEvent("PetRecycle", tbElf.ElfName, elfData.Exdata.Level.ToString());
                        }
                            break;
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_ElfBattleMax)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error("----------------msg.ErrorCode---------{0}", msg.ErrorCode);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_ElfNotFind
                         || msg.ErrorCode == (int) ErrorCodes.Error_ElfAlreadyBattle
                         || msg.ErrorCode == (int) ErrorCodes.Error_ElfNotBattle
                         || msg.ErrorCode == (int) ErrorCodes.Error_ElfNotBattleMain
                         || msg.ErrorCode == (int) ErrorCodes.Error_ElfIsBattleMain)
                {
                    //状态错误，重新请求数据
                    var e = new ShowUIHintBoard(270084);
                    EventDispatcher.Instance.DispatchEvent(e);
                    ReApplyBagData();
                    Logger.Error("----------------msg.ErrorCode---------{0}", msg.ErrorCode);
                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error("----------------msg.ErrorCode---------{0}", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("----------------msg.State---------{0}", msg.State);
            }
        }
    }

    private void ElfReplace(int f, int t)
    {
        var from = GetElfItemDataModel(f);
        var to = GetElfItemDataModel(t);
        if (from == null || to == null)
        {
            return;
        }
        if (from.ItemId == -1 && to.ItemId == -1)
        {
            return;
        }
        if (to.Index > 2)
        {
            return;
        }

        if (to.Index == 0)
        {
            ElfOperate(from, 2);
        }
        else if (from.Index == 0)
        {
            if (to.ItemId == -1)
            {
                ElfOperate(from, 1, to.Index);
            }
            else
            {
                ElfOperate(to, 2);
            }
        }
        else
        {
            ElfOperate(from, 1, to.Index);
        }
        // NetManager.Instance.StartCoroutine(ElfReplaceCoroutine( from, to));
    }

    private void GetElfFightCount()
    {
        var oldCount = DataModel.FightElfCount;
        var newCount = 0;
        if (PlayerDataManager.Instance.CheckCondition(GameUtils.ElfSecondCondition) != 0)
        {
            newCount = 1;
        }
        else if (PlayerDataManager.Instance.CheckCondition(GameUtils.ElfThirdCondition) != 0)
        {
            newCount = 2;
        }
        else
        {
            newCount = MaxElfFightCount;
        }

        if (oldCount != newCount)
        {
            DataModel.FightElfCount = newCount;
            var formations = DataModel.Formations;
            formations[1].IsLocked = newCount <= 1;
            formations[2].IsLocked = newCount <= 2;
        }
    }

    private ElfItemDataModel GetElfItemDataModel(int index)
    {
        if (index < 0 || index >= DataModel.ElfBag.Count)
        {
            return null;
        }
        return DataModel.ElfBag[index];
    }

    public int GetFreeIndex()
    {
        var c = DataModel.ElfBag.Count;
        for (var i = MaxElfFightCount; i < c; i++)
        {
            var data = DataModel.ElfBag[i];
            if (data.ItemId == -1)
            {
                return data.Index;
            }
        }
        return -1;
    }

    public void InitElfBag(BagBaseData bagData)
    {
        var tbBagBase = Table.GetBagBase(4);
        if (tbBagBase == null)
        {
            return;
        }
        var listItem = new List<ElfItemDataModel>();
        for (var i = 0; i < tbBagBase.MaxCapacity + 3; i++)
        {
            var itemData = new ElfItemDataModel();
            itemData.Index = i;
            listItem.Add(itemData);
        }
        DataModel.MaxElfCount = tbBagBase.InitCapacity;
        DataModel.ElfBag = new List<ElfItemDataModel>(listItem);
        var list = bagData.Items;
        var listCount = list.Count;
        BagItemCount = listCount;

        PlayerDataManager.Instance.ClearElfTotalCount();
        for (var i = 0; i < listCount; ++i)
        {
            var item = list[i];
            if (item.Index < 0 || item.Index >= DataModel.ElfBag.Count)
            {
                continue;
            }
            var itemData = DataModel.ElfBag[item.Index];
            itemData.ItemId = item.ItemId;
            itemData.Exdata.InstallData(item.Exdata);
            SetElfLevelExp(itemData);
            RefreshElfAttribute(itemData);

            if (item.Index >= MaxElfFightCount)
            {
                PlayerDataManager.Instance.AddElfTotalCount(item.ItemId, 1);
            }
        }
        GetElfFightCount();
        SortAndRefreshElf();
    }

    public void MoveElfBag(int from, int to)
    {
        var bag = DataModel.ElfBag;

        var fromData = bag[from];
        var toData = bag[to];

        if (fromData.Index >= MaxElfFightCount && toData.Index < MaxElfFightCount)
        {
            if (fromData.ItemId != toData.ItemId)
            {
                PlayerDataManager.Instance.AddElfTotalCount(fromData.ItemId, -1);
                PlayerDataManager.Instance.AddElfTotalCount(toData.ItemId, 1);
            }
        }
        else if (fromData.Index < MaxElfFightCount && toData.Index >= MaxElfFightCount)
        {
            if (fromData.ItemId != toData.ItemId)
            {
                PlayerDataManager.Instance.AddElfTotalCount(toData.ItemId, -1);
                PlayerDataManager.Instance.AddElfTotalCount(fromData.ItemId, 1);
            }
        }

        toData.Index = from;
        fromData.Index = to;

        bag[from] = toData;
        bag[to] = fromData;

        SetFormationElf(from, toData);
        SetFormationElf(to, fromData);
        RefreshShowElf(false);
    }

    private void OnElfCell1Click(IEvent ievent)
    {
        if (DataModel.IsAnimating)
        {
            return;
        }

        var e = ievent as ElfCell1ClickEvent;
        var data = e.DataModel;
        var formations = DataModel.Formations;
        var selIdx = 0;
        foreach (var formation in formations)
        {
            if (formation.IsSelect)
            {
                break;
            }
            ++selIdx;
        }
        if (selIdx == formations.Count)
        {
            Logger.Error("In OnElfCell1Click(),no formation elf selected!");
        }
        else if (data.Index == selIdx)
        {
            ElfOperate(data, 0, selIdx);
        }
        else if (selIdx == 0)
        {
            ElfOperate(data, 2, selIdx);
        }
        else
        {
            ElfOperate(data, 1, selIdx);
        }
    }

    private void OnElfCellClick(IEvent ievent)
    {
        var e = ievent as ElfCellClickEvent;
        var data = e.DataModel;
        SelectElfIndex = e.Index;
        SetSelectElf(data);
    }

    private void OnElfFlyOver(IEvent ievent)
    {
        var e = ievent as ElfFlyOverEvent;
        MoveElfBag(e.FromIdx, e.ToIdx);
    }

    private void OnElfOperate(IEvent ievent)
    {
        var e = ievent as ElfOperateEvent;
        switch (e.Type)
        {
            case 0: //disfight
            {
                ElfOperate(0);
            }
                break;
            case 1: //fight
            {
                ElfOperate(1);
            }
                break;
            case 2: //Show
            {
                ElfOperate(2);
            }
                break;
            case 3: //
            {
                //ElfOperate(3);
            }
                break;
            case 10:
            {
                ElfOperate(4);
            }
                break;
            case 11:
            {
                ElfOperate(5);
            }
                break;
            case 12: //回收
            {
                ElfOperate(6);
            }
                break;
            case 13: //升星
            {
                ElfOperate(7);
            }
                break;
            case 20: //formation info
            {
                if (!DataModel.ShowElfList)
                {
                    DataModel.ShowFormationInfo = !DataModel.ShowFormationInfo;
                    EventDispatcher.Instance.DispatchEvent(new ElfPlayAnimationEvent(0, DataModel.ShowFormationInfo,
                        false));
                }
            }
                break;
            case 21: //close formation info
            {
                if (DataModel.ShowFormationInfo)
                {
                    DataModel.ShowFormationInfo = false;
                    EventDispatcher.Instance.DispatchEvent(new ElfPlayAnimationEvent(0, false, false));
                }
            }
                break;
            case 22: //elf list
            {
                if (DataModel.ShowElfList)
                {
                    DataModel.ShowElfList = false;
                    EventDispatcher.Instance.DispatchEvent(new ElfPlayAnimationEvent(1, false, false));
                }
            }
                break;
            case 30: //显示精灵信息0
            case 31: //显示精灵信息1
            case 32: //显示精灵信息2
            {
                var formations = DataModel.Formations;
                var formation = formations[e.Type - 30];
                SetSelectElf(formation.ElfData);
                var selIdx = DataModel.Items.IndexOf(DataModel.SelectElf);
                if (selIdx > 0)
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_ElfSetGridLookIndex(selIdx));
                }
                DataModel.TabIndex = 1;
                DataModel.OnPropertyChanged("TabIndex");
            }
                break;
            case 41: //展示精灵1
            case 42: //展示精灵2
            {
                ElfOperate(DataModel.ElfBag[e.Type - 40], 2, 0);
            }
                break;
            case 50: //cell被点击0
            case 51: //cell被点击1
            case 52: //cell被点击2
            {
                var formations = DataModel.Formations;
                var formation = formations[e.Type - 50];
                if (formation.IsLocked)
                {
                    GameUtils.ShowHintTip(240311);
                    return;
                }
                foreach (var f in formations)
                {
                    if (f != formation)
                    {
                        f.IsSelect = false;
                    }
                }

                var newValue = !formation.IsSelect;
                formation.IsSelect = newValue;
                if (DataModel.ShowElfList != newValue)
                {
                    DataModel.ShowElfList = newValue;
                    EventDispatcher.Instance.DispatchEvent(new ElfPlayAnimationEvent(1, newValue, false));
                }
            }
                break;
            case 59:
            { // 技能
                
            }
                break;
            case 60: //tab 0 被点击
            {
                ResetUI();
            }
                break;
            case 61: //tab 1 被点击
            {
                RefreshShowElf(false, 0);
            }
                break;
            case 62: //tab 2 被点击
                break;
            case 63: //抽奖精灵显示
            case 64:
            case 65:
            case 66:
            case 67:
            case 68:
                ShowElfItemInfo(e.Type - 63);
                break;
        }
    }

    private void OnElfReplace(IEvent ievent)
    {
        var e = ievent as ElfReplaceEvent;
        ElfReplace(e.From, e.To);
    }

    private void OnExDataInit(IEvent ievent)
    {
        var lv = PlayerDataManager.Instance.GetExData(eExdataDefine.e82);
        UpdateFormationLevel(lv);
        var count = PlayerDataManager.Instance.GetExData(eExdataDefine.e411);
        if (count > 0)
        {
            DataModel.IsFreeDraw = 1;
            PlayerDataManager.Instance.NoticeData.ElfDraw = true;
        }
        else
        {
            DataModel.IsFreeDraw = 0;
            PlayerDataManager.Instance.NoticeData.ElfDraw = false;
            RefreshFreeDrawTime();
        }
    }

    private void OnLevelUp(IEvent ievent)
    {
        GetElfFightCount();
    }

    private void OnResourceChanged(IEvent ievent)
    {
        var e = ievent as Resource_Change_Event;
        if (e.Type != eResourcesType.ElfPiece)
        {
            return;
        }

        var oldvalue = e.OldValue;
        var newvalue = e.NewValue;
        var count = DataModel.ElfBag.Count;

        PlayerDataManager.Instance.WeakNoticeData.ElfTotal = false;
        PlayerDataManager.Instance.WeakNoticeData.ElfCanUpgrade = false;

        for (var i = 0; i < count; i++)
        {
            var elf = DataModel.ElfBag[i];
            if (elf == null || elf.ItemId == -1 || elf.LvExp < 1)
            {
                continue;
            }
            var needValue = elf.LvExp;
            if (Table.GetElf(elf.ItemId).MaxLevel == elf.Exdata.Level)
            {
                elf.CanUpgrade = false;
            }
            else
            {
                elf.CanUpgrade = needValue <= newvalue;
            }

//             if (needValue > oldvalue && needValue <= newvalue)
//             {
//                 PlayerDataManager.Instance.WeakNoticeData.ElfTotal = true;
//                 PlayerDataManager.Instance.WeakNoticeData.ElfCanUpgrade = true;
//             }
            if (needValue <= newvalue)
            {
                //PlayerDataManager.Instance.WeakNoticeData.ElfTotal = true;
                //PlayerDataManager.Instance.WeakNoticeData.ElfCanUpgrade = true;
            }
        }
    }

    private void OnUpdateExData(IEvent ievent)
    {
        var e = ievent as ElfExdataUpdate;
        if (e.Type == eExdataDefine.e82)
        {
            UpdateFormationLevel(e.Value);
        }
    }

    public void ReApplyBagData()
    {
        NetManager.Instance.StartCoroutine(ReApplyBagDataCoroutine());
    }

    private IEnumerator ReApplyBagDataCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyBagByType((int) eBagType.Elf);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    InitElfBag(msg.Response);
                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error("ApplyBagByType Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("ApplyBagByType Error!............State..." + msg.State);
            }
        }
    }

    private void RefresElfLists()
    {
        RefreshGroupInfo();
        RefreshElfAttribute(DataModel.SelectElf);
        RefreshFormationAttribute();
    }

    private void RefresFightElf()
    {
        var data = DataModel.Formations[0].ElfData;
        DataModel.FightElf = data;
        if (data.ItemId == -1)
        {
            if (ObjManager.Instance.MyPlayer != null)
            {
                ObjManager.Instance.MyPlayer.RefresElfFollow(-1);
            }
            return;
        }
        var tbElf = Table.GetElf(data.ItemId);
        if (tbElf == null)
        {
            if (ObjManager.Instance.MyPlayer != null)
            {
                ObjManager.Instance.MyPlayer.RefresElfFollow(-1);
            }
        }
        else
        {
            if (ObjManager.Instance.MyPlayer != null)
            {
                ObjManager.Instance.MyPlayer.RefresElfFollow(tbElf.ElfModel);
            }
        }
    }

    public void RefreshElfAttribute(ElfItemDataModel elfData)
    {
        if (elfData.ItemId == -1)
        {
            return;
        }
        var tbItem = Table.GetItemBase(elfData.ItemId);
        var tbElf = Table.GetElf(tbItem.Exdata[0]);
        var level = elfData.Exdata.Level;
        var fightAttr = new Dictionary<int, int>();
        for (var i = 0; i < 6; i++)
        {
            DataModel.BaseAttrAdd[i] = "";
        }
        var maxLevel = elfData.Exdata.Level == tbElf.MaxLevel;

        DataModel.IsNotMaxLevel = !maxLevel;

        if (maxLevel)
        {
            elfData.CanUpgrade = false;
        }

        for (var i = 0; i < 6; i++)
        {
            var id = tbElf.ElfInitProp[i];
            //var value = GameUtils.EquipAttrValueRef(id, tbElf.ElfProp[i]);
            var value = tbElf.ElfProp[i];
            DataModel.BaseAttr[i].Reset();
            if (id != -1)
            {
                //var valuelevel = GameUtils.EquipAttrValueRef(id, tbElf.GrowAddValue[i]);
                var valuelevel = tbElf.GrowAddValue[i];
                value += valuelevel*(level - 1);

                GameUtils.SetAttributeBase(DataModel.BaseAttr, i, id, value);
                //value = GameUtils.EquipAttrValueRef(id, value);
                fightAttr.modifyValue(id, value);
                if (maxLevel == false)
                {
                    var addValue = valuelevel;
                    DataModel.BaseAttrAdd[i] = string.Format("{0}", GameUtils.AttributeValue(id, addValue));
                }
            }
        }
        for (var i = 0; i < 6; i++)
        {
            var id = elfData.Exdata[i + 2];
            var value = elfData.Exdata[i + 8];
            //var realValue = GameUtils.EquipAttrValueRef(id, value);     
            if (id != -1 && value > 0)
            {
                GameUtils.SetAttributeBase(DataModel.InnateAttr, i, id, value);
                //value = GameUtils.EquipAttrValueRef(id, value);
                fightAttr.modifyValue(id, value);
            }
            else
            {
                DataModel.InnateAttr[i].Reset();
            }
        }

        elfData.FightPoint = PlayerDataManager.Instance.GetElfAttrFightPoint(fightAttr, -1, -2);
    }

    //---------------------------------------------------Group---------------------
    private void RefreshFormationAttribute()
    {
        var baseAttr = new Dictionary<int, int>();
        var innateAttr = new Dictionary<int, int>();
        var groupList = new Dictionary<int, int>();
        var tbLevel = Table.GetLevelData(DataModel.FormationLevel);
        var levelRate = tbLevel.FightingWayIncome + 10000.0;

        for (var i = 0; i < MaxElfFightCount; i++)
        {
            var formation = DataModel.Formations[i];
            var elfData = formation.ElfData;
            if (elfData.ItemId == -1)
            {
                continue;
            }
            var rate = 1; //elfData.Index == 0 ? 1.0 : 0.1;
            var level = elfData.Exdata.Level;
            var tbItem = Table.GetItemBase(elfData.ItemId);
            var tbElf = Table.GetElf(tbItem.Exdata[0]);
            for (var j = 0; j < 6; j++)
            {
                var id = tbElf.ElfInitProp[j];
                var value = tbElf.ElfProp[j];
                if (id != -1)
                {
                    if (level > 1)
                    {
                        var upvalue = tbElf.GrowAddValue[j];
                        value += upvalue*(level - 1);
                    }
                    value = (int) (rate*value*levelRate/10000.0);
                    if (value > 0)
                    {
                        baseAttr.modifyValue(id, value);
                    }
                }
            }

            if (elfData.Index == 0)
            {
                for (var j = 0; j < 6; j++)
                {
                    var id = elfData.Exdata[j + 2];
                    var value = elfData.Exdata[j + 8];
                    if (id != -1 && value > 0)
                    {
                        innateAttr.modifyValue(id, value);
                    }
                }
            }

            for (var j = 0; j < 3; j++)
            {
                var groupId = tbElf.BelongGroup[j];
                if (groupId != -1)
                {
                    groupList.modifyValue(groupId, 1);
                }
            }
        }

        var baseCount = baseAttr.Count;
        var index = 0;
        {
            // foreach(var i in baseAttr)
            var __enumerator4 = (baseAttr).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var i = __enumerator4.Current;
                {
                    if (index >= DataModel.GroupBaseAttr.Count)
                    {
                        break;
                    }
                    DataModel.GroupBaseAttr[index].Type = i.Key;
                    DataModel.GroupBaseAttr[index].Value = i.Value;
                    index++;
                }
            }
        }
        for (var i = baseCount; i < DataModel.GroupBaseAttr.Count; i++)
        {
            DataModel.GroupBaseAttr[i].Reset();
        }

        index = 0;
        var innateCount = innateAttr.Count;
        {
            // foreach(var i in innateAttr)
            var __enumerator5 = (innateAttr).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var i = __enumerator5.Current;
                {
                    DataModel.GroupInnateAttr[index].Type = i.Key;
                    DataModel.GroupInnateAttr[index].Value = i.Value;
                    index++;
                }
            }
        }

        for (var i = innateCount; i < 6; i++)
        {
            DataModel.GroupInnateAttr[i].Reset();
        }

        var showList = new List<int>();
        {
            // foreach(var i in groupList)
            var __enumerator6 = (groupList).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var i = __enumerator6.Current;
                {
                    var groupId = i.Key;
                    var tbElfGroup = Table.GetElfGroup(groupId);
                    var flag = true;
                    for (var j = 0; j < 3; j++)
                    {
                        var elfId = tbElfGroup.ElfID[j];
                        if (elfId == -1)
                        {
                            continue;
                        }
                        if (!ElfIdInFormation(elfId))
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        showList.Add(groupId);
                    }
                }
            }
        }

        for (var i = 0; i < showList.Count; i++)
        {
            var tbElfGroup = Table.GetElfGroup(showList[i]);
            SetGroupAttr(DataModel.GroupInfos[i], tbElfGroup, DataModel.SelectElf.Index);
        }

        for (var i = showList.Count; i < DataModel.GroupInfos.Count; i++)
        {
            DataModel.GroupInfos[i].Reset();
        }
    }

    //---------------------------------------------------Main---------------------
    private void RefreshGroupInfo()
    {
        if (DataModel.SelectElf.ItemId == -1)
        {
            return;
        }
        var tbElf = Table.GetElf(DataModel.SelectElf.ItemId);
        for (var i = 0; i < DataModel.SingleGroups.Count; i++)
        {
            var groupId = tbElf.BelongGroup[i];
            var info = DataModel.SingleGroups[i];
            if (groupId != -1)
            {
                var tbElfGroup = Table.GetElfGroup(groupId);
                SetGroupAttr(info, tbElfGroup, DataModel.SelectElf.Index, true);
            }
            else
            {
                info.Reset();
            }
        }
    }

    private void RefreshStarInfo()
    {
        var tbElf = Table.GetElf(DataModel.SelectElf.ItemId);
        if (tbElf == null)
            return;

        DataModel.MaxStar = tbElf.ElfStarUp.Count;
        var curStarLevel = DataModel.SelectElf.Exdata.StarLevel;
        if (curStarLevel >= DataModel.MaxStar)
        {
            return;
        }

        var id = tbElf.ElfStarUp[curStarLevel];
        var tbConsumeArray = Table.GetConsumArray(id);
        if (tbConsumeArray == null)
        {
            DataModel.StarCostResId = -1;
            return;
        }

        var index = 0;
        for (var i = 0; i < tbConsumeArray.ItemId.Length; ++i)
        {
            var itemId = tbConsumeArray.ItemId[i];
            if (itemId == -1)
                break;
            var itemCount = tbConsumeArray.ItemCount[i];
            if (itemId < (int) eResourcesType.CountRes)
            {
                DataModel.StarCostResId = itemId;
                DataModel.StarCostResCount = itemCount;                
            }
            else
            {
                if (index < DataModel.StarCostItems.Count)
                {
                    var data = DataModel.StarCostItems[index];
                    data.ItemId = itemId;
                    data.Count = itemCount;
                    ++index;
                }
            }
        }
        for (var i = index; i < DataModel.StarCostItems.Count; ++i)
        {
            DataModel.StarCostItems[i].ItemId = -1;
        }
    }

    private void RefreshShowElf(bool bRefreshElfList = true, int elfIdx = -1)
    {
        var elfs = DataModel.ElfBag.Where(d => d.ItemId != -1).ToList();

        if (elfs.Count > MaxElfFightCount)
        {
            elfs.Sort(MaxElfFightCount, elfs.Count - MaxElfFightCount, ElfItemComparer);
        }
        DataModel.Items = new ObservableCollection<ElfItemDataModel>(elfs);
        DataModel.NowElfCount = DataModel.Items.Count;
        if (bRefreshElfList)
        {
            elfs.Sort(ElfItemComparer);
            DataModel.ElfList = new ObservableCollection<ElfItemDataModel>(elfs);
        }

        if (DataModel.SelectElf.ItemId == -1 || elfIdx != -1)
        {
            if (elfIdx != -1)
            {
                SelectElfIndex = elfIdx;
            }
            if (DataModel.Items.Count > 0)
            {
                if (SelectElfIndex >= DataModel.Items.Count)
                {
                    SelectElfIndex = DataModel.Items.Count - 1;
                }

                var index = SelectElfIndex;
                if (DataModel.Items.Count <= index)
                {
                    index = DataModel.Items.Count - 1;
                }
                SetSelectElf(DataModel.Items[index]);
            }
        }
        RefresElfLists();
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Elf);

        if (mIsSetOffset)
        {
            mIsSetOffset = false;
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ElfSetGridLookIndex(-1));
        }
        else
        {
            var selIdx = DataModel.Items.IndexOf(DataModel.SelectElf);
            if (selIdx > 0)
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_ElfSetGridLookIndex(selIdx));
            }
        }
    }

    public void ResetInfo()
    {
        for (var i = 0; i < 6; i++)
        {
            DataModel.BaseAttr[i].Reset();
            DataModel.InnateAttr[i].Reset();
            DataModel.GroupInnateAttr[i].Reset();

            DataModel.BaseAttrAdd[i] = "";
        }
        for (var i = 0; i < DataModel.GroupBaseAttr.Count; i++)
        {
            DataModel.GroupBaseAttr[i].Reset();
        }
        for (var i = 0; i < DataModel.SingleGroups.Count; i++)
        {
            DataModel.SingleGroups[i] = new ElfGroupInfoData();
        }
    }

    private void ResetUI()
    {
        DataModel.UIGetShow = 0;
        //DataModel.UIGetOneShow = 0;
        var formations = DataModel.Formations;
        foreach (var f in formations)
        {
            f.IsSelect = false;
        }
        if (DataModel.ShowFormationInfo)
        {
            DataModel.ShowFormationInfo = false;
            EventDispatcher.Instance.DispatchEvent(new ElfPlayAnimationEvent(0, false, true));
        }
        if (DataModel.ShowElfList)
        {
            DataModel.ShowElfList = false;
            EventDispatcher.Instance.DispatchEvent(new ElfPlayAnimationEvent(1, false, true));
        }
        RefreshShowElf();
    }

    private void SetElfLevelExp(ElfItemDataModel itemData)
    {
        if (itemData.ItemId == -1)
        {
            itemData.LvExp = 0;
            return;
        }

        var tbElf = Table.GetElf(itemData.ItemId);
        if (tbElf == null)
        {
            itemData.LvExp = 0;
            return;
        }

        if (tbElf.MaxLevel == itemData.Exdata.Level)
        {
            itemData.LvExp = -1;
            itemData.CanUpgrade = false;
        }
        else
        {
            var tbLevel = Table.GetLevelData(itemData.Exdata.Level);
            itemData.LvExp = tbLevel.ElfExp*tbElf.ResolveCoef[0]/100;
            var res = PlayerDataManager.Instance.GetRes((int) eResourcesType.ElfPiece);
            itemData.CanUpgrade = itemData.LvExp <= res;
        }
    }

    private void SetFormationElf(int elfIdx, ElfItemDataModel elf)
    {
        var formations = DataModel.Formations;
        if (elfIdx >= DataModel.FightElfCount)
        {
            return;
        }

        formations[elfIdx].Install(elf);
        EventDispatcher.Instance.DispatchEvent(new FormationElfModelRefreshEvent(elfIdx));
        if (elfIdx == 0)
        {
            RefresFightElf();
        }
    }

    private void SetGroupAttr(ElfGroupInfoData infoData, ElfGroupRecord tbElfGroup, int elfIdx, bool isCheck = false)
    {
        var isFight = elfIdx < MaxElfFightCount;
        var isActive = isFight;
        infoData.GroupId = tbElfGroup.Id;
        for (var j = 0; j < MaxElfFightCount; j++)
        {
            var itemId = tbElfGroup.ElfID[j];
            infoData.ItemList[j] = itemId;
            if (itemId == -1)
            {
                continue;
            }
            if (isCheck)
            {
                var isFormation = isFight && ElfIdInFormation(itemId);
                if (!isFormation)
                {
                    isActive = false;
                }
                infoData.IsFight[j] = isFormation;
            }
            else
            {
                infoData.IsFight[j] = isFight;
            }
        }
        infoData.AttrColor = isActive ? GameUtils.green : GameUtils.grey;
        var groupAttr = new Dictionary<int, int>();
        var tbElfGroupGroupPorpLength3 = tbElfGroup.GroupPorp.Length;
        for (var j = 0; j < tbElfGroupGroupPorpLength3; j++)
        {
            var attrId = tbElfGroup.GroupPorp[j];
            if (attrId == -1)
            {
                break;
            }
            var attrValue = tbElfGroup.PropValue[j];
            groupAttr.modifyValue(attrId, attrValue);
        }
        var index = 0;
        {
            var enumerator1 = (groupAttr).GetEnumerator();
            while (enumerator1.MoveNext())
            {
                var j = enumerator1.Current;
                {
                    infoData.GroupAttr[index].Type = j.Key;
                    infoData.GroupAttr[index].Value = j.Value;
                    index++;
                }
            }
        }
        var count4 = infoData.GroupAttr.Count;
        for (var j = index; j < count4; j++)
        {
            infoData.GroupAttr[j].Type = -1;
            infoData.GroupAttr[j].Value = 0;
        }
    }

    private void SetSelectElf(ElfItemDataModel data)
    {
        if (DataModel.SelectElf == data)
        {
            return;
        }
        {
            // foreach(var dataModel in DataModel.Items)
            var __enumerator1 = (DataModel.Items).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var dataModel = __enumerator1.Current;
                {
                    if (dataModel == data)
                    {
                        dataModel.IsSelect = true;
                    }
                    else
                    {
                        dataModel.IsSelect = false;
                    }
                }
            }
        }

        DataModel.SelectElf = data;
        if (data.ItemId == -1)
        {
            var e1 = new ElfModelRefreshEvent(-1);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
        else
        {
            var tbElf = Table.GetElf(DataModel.SelectElf.ItemId);
            var e1 = new ElfModelRefreshEvent(tbElf.ElfModel, tbElf.Offset);
            EventDispatcher.Instance.DispatchEvent(e1);
            RefreshGroupInfo();
            RefreshElfAttribute(DataModel.SelectElf);
            RefreshStarInfo();
        }
    }

    private void ShowDrawGet(IEvent ievent)
    {
        if (IsOneDraw)
        {
            DataModel.UIGetOneShow = 1;
        }
        else
        {
            DataModel.UIGetOneShow = 10;
        }
    }

    private void ShowElfItemInfo(int index)
    {
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ElfInfoUI, new ElfInfoArguments
        {
            DataModel = DataModel.ElfShowItems[index]
        }));
    }

    private void ShowOneDrawInfo(IEvent ievent)
    {
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ElfInfoUI,
            new ElfInfoArguments {DataModel = DataModel.OneGetItem}));
    }

    private void SortAndRefreshElf()
    {
        var bag = DataModel.ElfBag;
        for (int i = 0, imax = bag.Count >= MaxElfFightCount ? bag.Count : MaxElfFightCount; i < imax; i++)
        {
            SetFormationElf(i, bag[i]);
        }
        RefreshShowElf();
    }

    public void UpdateElfBag(ItemsChangeData bagData)
    {
        {
            // foreach(var change in bagData.ItemsChange)
            var __enumerator2 = (bagData.ItemsChange).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var change = __enumerator2.Current;
                {
                    var changeItem = change.Value;
                    var index = changeItem.Index;
                    var itemData = DataModel.ElfBag[index];
                    if (DataModel.ElfBag.Count > changeItem.Index)
                    {
                        if (itemData.ItemId == -1)
                        {
                            BagItemCount++;
                            if (index >= MaxElfFightCount && changeItem.ItemId != -1)
                            {
                                PlayerDataManager.Instance.AddElfTotalCount(changeItem.ItemId, 1);
                            }
                        }
                        if (changeItem.ItemId == -1)
                        {
                            BagItemCount--;
                            if (index >= MaxElfFightCount && itemData.ItemId != -1)
                            {
                                PlayerDataManager.Instance.AddElfTotalCount(itemData.ItemId, -1);
                            }
                        }
                    }
                    if (itemData != DataModel.SelectElf)
                    {
                        itemData.IsSelect = false;
                    }

                    //var elfItem = GetElfItem(index);
                    itemData.ItemId = changeItem.ItemId;
                    itemData.Index = changeItem.Index;
                    itemData.Exdata.InstallData(changeItem.Exdata);
                    SetElfLevelExp(itemData);
                    RefreshElfAttribute(itemData);
                }
            }
        }

        SortAndRefreshElf();
    }

    private void UpdateFormationLevel(int level)
    {
        DataModel.FormationLevel = level;
        RefreshFormationAttribute();
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Elf);
    }

    public void CleanUp()
    {
        DataModel = new ElfDataModel();
        DataModel.OneIconID = int.Parse(Table.GetClientConfig(500).Value);
        DataModel.TenIconID = int.Parse(Table.GetClientConfig(502).Value);
        DataModel.OneMoney = int.Parse(Table.GetClientConfig(501).Value);
        DataModel.TenMoney = int.Parse(Table.GetClientConfig(503).Value);
        for (var i = 0; i < DataModel.ShowList.Count; i++)
        {
            var item = new ItemIdDataModel();
            var tbClientConfig = Table.GetClientConfig(510 + i);
            if (tbClientConfig != null)
            {
                item.ItemId = int.Parse(tbClientConfig.Value);
            }
            DataModel.ShowList[i] = item;
        }
        for (var i = 0; i < DataModel.ElfShowItems.Count; i++)
        {
            var item = new ElfItemDataModel();
            var tbClientConfig = Table.GetClientConfig(510 + i);
            if (tbClientConfig != null)
            {
                item.ItemId = int.Parse(tbClientConfig.Value);
            }
            InitElfRandomProp(item);
            DataModel.ElfShowItems[i] = item;
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "UpdateElfBag")
        {
            UpdateElfBag(param[0] as ItemsChangeData);
        }
        else if (name == "InitElfBag")
        {
            InitElfBag(param[0] as BagBaseData);
        }
        else if (name == "GetFightModel")
        {
            var itemId = DataModel.Formations[0].ElfData.ItemId;

            if (itemId == -1)
            {
                return -1;
            }
            var tbElf = Table.GetElf(itemId);
            return tbElf.ElfModel;
        }
        else if (name == "SetGroupAttr")
        {
            SetGroupAttr((ElfGroupInfoData) param[0], (ElfGroupRecord) param[1], (int) param[2], (bool) param[3]);
        }
        else if (name == "GetIsFreeDraw")
        {
            return DataModel.IsFreeDraw;
        }
        return null;
    }

    public void OnShow()
    {
        var tbElf = Table.GetElf(DataModel.SelectElf.ItemId);
        if (tbElf != null)
        {
            var e1 = new ElfModelRefreshEvent(tbElf.ElfModel, tbElf.Offset);
            EventDispatcher.Instance.DispatchEvent(e1);
        }

        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUp);
    }

    public void Close()
    {
        if (mTimeCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mTimeCoroutine);
            mTimeCoroutine = null;
        }

        EventDispatcher.Instance.RemoveEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUp);
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as ElfArguments;
        if (args != null)
        {
            DataModel.TabIndex = args.Tab;
        }
        mIsSetOffset = false;

        GetElfFightCount();
        ResetUI();
        SortAndRefreshElf();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "Resource")
        {
            return PlayerDataManager.Instance.PlayerDataModel.Bags.Resources;
        }
        return DataModel;
    }

    public FrameState State { get; set; }

    #region 抽奖代码

    private void GetDrawResult(IEvent ievent)
    {
        var e = ievent as ElfGetDrawResult;
        DataModel.UIGetOneShow = 0; //0 不显示 ，1 显示单抽，10显示10抽
        var draw = e.DrawItems.Items;
        if (draw.Count == 0)
        {
        }
        else if (draw.Count == 1)
        {
            DataModel.OneGetItem.Index = draw[0].Index;
            DataModel.OneGetItem.ItemId = draw[0].ItemId;
            DataModel.OneGetItem.Exdata.InstallData(draw[0].Exdata);
            RefreshFreeDrawTime();
            //if (FreeType == 100)
            //{
            //    //Draw.FreeNowCount--;
            //    long time = e.DrawTime;
            //   // RefreshFightTime(time);
            //}
            var tbItem = Table.GetItemBase(draw[0].ItemId);
            DataModel.OneDrawName = tbItem.Name;
            DataModel.UIGetShow = 1;
            IsOneDraw = true;
            var ee = new ElfGetDrawResultBack(1);
            EventDispatcher.Instance.DispatchEvent(ee);
            // DataModel.UIGetOneShow = 1;
        }
        else if (draw.Count == 10)
        {
            var drawCount3 = draw.Count;
            for (var i = 0; i < drawCount3; i++)
            {
                DataModel.TenGetItem[i].ItemId = draw[i].ItemId;
                DataModel.TenGetItem[i].Index = draw[i].Index;
                DataModel.TenGetItem[i].Exdata.InstallData(draw[i].Exdata);
                var tbItem = Table.GetItemBase(draw[i].ItemId);
                DataModel.TenNameList[i] = tbItem.Name;
            }
            DataModel.UIGetShow = 1;
            IsOneDraw = false;
            //DataModel.UIGetOneShow = 0;
            var ee = new ElfGetDrawResultBack(10);
            EventDispatcher.Instance.DispatchEvent(ee);
        }
    }

    private void RefreshFreeDrawTime()
    {
        DataModel.IsFreeDraw = 0;
        var mTime = Game.Instance.ServerTime.AddDays(1).Date;

        if (mTimeCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(mTimeCoroutine);
            mTimeCoroutine = null;
        }
        mTimeCoroutine = NetManager.Instance.StartCoroutine(TimerCoroutine(mTime));
        EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(7, 0));
    }

    private IEnumerator TimerCoroutine(DateTime time)
    {
        while (time > Game.Instance.ServerTime)
        {
            yield return new WaitForSeconds(1f);
            if (State == FrameState.Open)
            {
                var timeSpan = time - Game.Instance.ServerTime;
                var str = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                DataModel.DrawTimeString = str + GameUtils.GetDictionaryText(300404);
            }
        }
        yield return new WaitForSeconds(1f);
        DataModel.IsFreeDraw = 1;
    }

    private void OnClickGetShow(IEvent ievent)
    {
        var e = ievent as ElfGetOneShowEvent;
        var Moneytype = 0;
        var Money = 0;
        var drawCount = 0;
        DataModel.UIGetOneShow = 0;
        DataModel.UIGetShow = 0;
        if (e.Type == 1)
        {
            Moneytype = int.Parse(Table.GetClientConfig(500).Value);
            Money = DataModel.OneMoney;
            drawCount = 1;
        }
        else if (e.Type == 10)
        {
            Moneytype = int.Parse(Table.GetClientConfig(502).Value);
            Money = DataModel.TenMoney;
            drawCount = 10;
        }
        var freecount = PlayerDataManager.Instance.GetExData(411);
        //判断金币是否够了
        if (Moneytype == 2)
        {
            if (e.Type == 10 || freecount <= 0)
            {
                if (Money > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                    PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
                    return;
                }
            }
        }
        //判断钻石是否够了
        else if (Moneytype == 3)
        {
            if (e.Type == 10 || freecount <= 0)
            {
                if (Money > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                    PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.DiamondRes);
                    return;
                }
            }
        }
        var tbbag = Table.GetBagBase((int) eBagType.Elf);
        if (tbbag.MaxCapacity < BagItemCount + drawCount)
        {
            var ee = new ShowUIHintBoard(270219);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        NetManager.Instance.StartCoroutine(DrawElf(drawCount));
    }

    private IEnumerator DrawElf(int drawCount)
    {
        using (new BlockingLayerHelper(0))
        {
            var code = 0;
            if (drawCount == 1)
            {
                code = 200;
            }
            else if (drawCount == 10)
            {
                code = 201;
            }
            var msg = NetManager.Instance.DrawLotteryPetEgg(code);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (drawCount == 1)
                    {
                        PlayerDataManager.Instance.NoticeData.ElfDraw = false;
                    }
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Unknow)
                    {
                        var e = new ShowUIHintBoard(200000001);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                    {
                        var e = new ShowUIHintBoard(200002003);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else if (msg.ErrorCode == (int) ErrorCodes.Error_SeedTimeNotOver)
                    {
                        var e = new ShowUIHintBoard(220900);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                    {
                        var e = new ShowUIHintBoard(200000005);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    private void OnClickShowClose(IEvent ievent)
    {
        DataModel.UIGetShow = 0;
    }

    #endregion

    #region 生成随机精灵

    private readonly List<int> IndextoAttrId = new List<int>
    {
        13,
        14,
        9,
        12,
        19,
        20,
        17,
        21,
        22,
        23,
        24,
        26,
        25,
        105,
        110,
        113,
        114,
        119,
        120,
        106,
        111,
        98,
        99
    };

    public void InitElfRandomProp(ElfItemDataModel bagItem)
    {
        var tbElf = Table.GetElf(bagItem.ItemId);
        if (tbElf == null)
        {
            return;
        }
        var list = new List<int>();
        //初始等级
        list.Add(1);
        //是否出战
        list.Add(0);
        //随机附加属性
        for (var i = 0; i != 12; ++i)
        {
            list.Add(-1);
        }
        bagItem.Exdata.InstallData(list);
        var addCount = RandomElfAddCount(tbElf.RandomPropCount);
        InitElfAddAttr(bagItem, tbElf, addCount);
    }

    //随机属性条数随机
    private int RandomElfAddCount(int EquipRelateId)
    {
        if (EquipRelateId == -1)
        {
            return 0;
        }
        var tbRelate = Table.GetEquipRelate(EquipRelateId);
        if (tbRelate == null)
        {
            Logger.Error("EquipRelate Id={0} not find", EquipRelateId);
            return 0;
        }
        var AddCount = 0;
        var nRandom = MyRandom.Random(10000);
        var nTotleRandom = 0;
        for (var i = 0; i != tbRelate.AttrCount.Length; ++i)
        {
            nTotleRandom += tbRelate.AttrCount[i];
            if (nRandom < nTotleRandom)
            {
                if (i == 0)
                {
                    return 0;
                }
                AddCount = i;
                break;
            }
        }
        return AddCount;
    }

    //初始化附加属性
    public void InitElfAddAttr(ElfItemDataModel bagItem, ElfRecord tbElf, int addCount)
    {
        if (addCount <= 0 || addCount > 6)
        {
            return;
        }
        int nRandom, nTotleRandom;
        var TbAttrPro = Table.GetEquipEnchantChance(tbElf.RandomPropPro);
        if (TbAttrPro == null)
        {
            Logger.Error("Equip InitAddAttr Id={0} not find EquipEnchantChance Id={1}", tbElf.Id, tbElf.RandomPropPro);
            return;
        }
        var tempAttrPro = new Dictionary<int, int>();
        var nTotleAttrPro = 0;
        for (var i = 0; i != 23; ++i)
        {
            var nAttrpro = TbAttrPro.Attr[i];
            if (nAttrpro > 0)
            {
                nTotleAttrPro += nAttrpro;
                tempAttrPro[i] = nAttrpro;
            }
        }
        //属性值都在这里
        var tbEnchant = Table.GetEquipEnchant(tbElf.RandomPropValue);
        if (tbEnchant == null)
        {
            Logger.Error("Equip InitAddAttr Id={0} not find tbEquipEnchant Id={1}", tbElf.Id, tbElf.RandomPropValue);
            return;
        }
        //整理概率
        var AttrValue = new Dictionary<int, int>();
        for (var i = 0; i != addCount; ++i)
        {
            nRandom = MyRandom.Random(nTotleAttrPro);
            nTotleRandom = 0;
            foreach (var i1 in tempAttrPro)
            {
                nTotleRandom += i1.Value;
                if (nRandom < nTotleRandom)
                {
                    //AddCount = i1.Key;
                    AttrValue[i1.Key] = tbEnchant.Attr[i1.Key];
                    nTotleAttrPro -= i1.Value;
                    tempAttrPro.Remove(i1.Key);
                    break;
                }
            }
        }
        var NowAttrCount = AttrValue.Count;
        if (NowAttrCount < addCount)
        {
            //Logger.Error("Equip InitAddAttr AddAttr Not Enough AddCount={0},NowAttrCount={1}", addCount, NowAttrCount);
        }

        for (var i = 0; i != NowAttrCount; ++i)
        {
            var nKey = AttrValue.Keys.Min();
            var nAttrId = GetAttrId(nKey);
            if (nAttrId == -1)
            {
                continue;
            }
            var fValue = tbEnchant.Attr[nKey];
            AddAttr(bagItem, i, nAttrId, fValue);
            AttrValue.Remove(nKey);
        }
    }

    //增加附加属性
    private void AddAttr(ElfItemDataModel bagItem, int nIndex, int nAttrId, int nAttrValue)
    {
        bagItem.Exdata[nIndex + 2] = nAttrId;
        bagItem.Exdata[nIndex + 8] = nAttrValue;
    }


    public int GetAttrId(int index)
    {
        if (index > IndextoAttrId.Count || index < 0)
        {
            Logger.Error("GetAttrId index={0}", index);
            return -1;
        }
        return IndextoAttrId[index];
    }

    #endregion
}