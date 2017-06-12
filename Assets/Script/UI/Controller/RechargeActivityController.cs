#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class RechargeActivityController : IControllerBase
{
    public RechargeActivityController()
    {
        platfrom = "android";
#if UNITY_ANDROID
        platfrom = "android";
#elif UNITY_IOS
        platfrom = "ios";
#endif
        EventDispatcher.Instance.AddEventListener(RechageActivityRewardOperation.EVENT_TYPE, RewardOperation);
        EventDispatcher.Instance.AddEventListener(RechageActivityMenuItemClick.EVENT_TYPE, MenuClick);
        EventDispatcher.Instance.AddEventListener(RechageActivityOperation.EVENT_TYPE, OnOperation);
        EventDispatcher.Instance.AddEventListener(RechageActivityInvestmentOperation.EVENT_TYPE, InvestmentOperation);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, InitExData);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, UpdateExdata);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, UpdateFlag);
        EventDispatcher.Instance.AddEventListener(RechageActivityInitTables.EVENT_TYPE, InitTables);
        CleanUp();
    }

    private List<int> tableExdata = new List<int>();
    private List<int> tableFlagTrue = new List<int>();
    private List<int> tableFlagFalse = new List<int>();

    private readonly string platfrom;
    private int _menuSelectIndex;

    private readonly Dictionary<int, RechargeActivityMenuItemDataModel> _mExtraIdToMenuItem =
        new Dictionary<int, RechargeActivityMenuItemDataModel>();

    private readonly Dictionary<int, List<int>> _mInvestmentDic = new Dictionary<int, List<int>>();
    private readonly Dictionary<int, List<int>> _mReChargeDic = new Dictionary<int, List<int>>();
    private RechargeActivityMenuItemDataModel _mSelectedMenuItem;
    public RechargeActivityDataModel DataModel;
    private string MainLabelStr = string.Empty;
    private DateTime NearTime;
    private readonly int RandomSecondesToApplyTables = 100;
    private object RefleshTrigger;
    private bool TableUpdated;
    private object Trigger;

    private IEnumerator ApplyRechargeTablesCoroutine()
    {
        var msg = NetManager.Instance.ApplyRechargeTables(0);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply && msg.ErrorCode == (int) ErrorCodes.OK)
        {
            Game.Instance.RechargeActivityData = msg.Response;
            _RechargeActiveTable = msg.Response.RechargeActiveTable;
            _RechargeActiveNoticeTable = msg.Response.RechargeActiveNoticeTable;
            _RechargeActiveCumulativeRewardTable = msg.Response.RechargeActiveCumulativeRewardTable;
            _RechargeActiveInvestmentRewardTable = msg.Response.RechargeActiveInvestmentRewardTable;
            _RechargeActiveCumulativeTable = msg.Response.RechargeActiveCumulativeTable;
            _RechargeActiveInvestmentTable = msg.Response.RechargeActiveInvestmentTable;
            InitData();
            RefleshMenuData();
        }
    }

    private void BuyInvestment()
    {
        var tbActivity = GetRechargeActive(_mSelectedMenuItem.ActivityId);
        var tbRecharge = GetRechargeActiveCumulative(tbActivity.SonType);
        var conditionDic = PlayerDataManager.Instance.CheckCondition(tbRecharge.ConditionId);
        if (conditionDic != 0)
        {
            var e = new ShowUIHintBoard(conditionDic);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        var id = DataModel.Investment.Id;
        //道具不足
//         if (PlayerDataManager.Instance.GetRes(tbRecharge.NeedItemId) < tbRecharge.NeedItemCount)
//         {
//             var str = string.Format(GameUtils.GetDictionaryText(270011), Table.GetItemBase(tbRecharge.NeedItemId).Name);
//             var ee = new ShowUIHintBoard(str);
//             EventDispatcher.Instance.DispatchEvent(ee);
//             return;
//         }
        NetManager.Instance.StartCoroutine(BuyInvestment(id));
    }

    public IEnumerator BuyInvestment(int id)
    {
        using (new BlockingLayerHelper(0))
        {
            var tbRecharge = GetRechargeActiveCumulative(id);
            if (tbRecharge == null || tbRecharge.ChargeID == null)
            {
                yield break;
            }

            string[] str = tbRecharge.ChargeID.Split(',');
            if (str.Length < 2)
            {
                yield break;
            }

            var tableid = 1;
            if (platfrom == "android")
            {
                var aa = str[1];
                tableid = int.Parse(aa);
            }
            else if (platfrom == "ios")
            {
                var aa = str[0];
                tableid = int.Parse(aa);
            }
            else
            {
                yield break; 
            }
            
            var ee = new OnTouZiBtnClick_Event(tableid);
            EventDispatcher.Instance.DispatchEvent(ee);
//             var msg = NetManager.Instance.Investment(id);
//             yield return msg.SendAndWaitUntilDone();
//             if (msg.State == MessageState.Reply)
//             {
//                 if (msg.ErrorCode == (int) ErrorCodes.OK)
//                 {
//                     DataModel.Investment.CanBuy = 0;
//                     var tbRecharge = GetRechargeActiveCumulative(id);
//                     var extraId = tbRecharge.ExtraId;
//                     var instance = PlayerDataManager.Instance;
//                     if (tbRecharge.ResetCount != -1)
//                     {
//                         var tbExdata = Table.GetExdata(extraId);
//                         if (tbExdata != null)
//                         {
//                             var randomValue = MyRandom.Random(tbExdata.RefreshValue[0], tbExdata.RefreshValue[1]);
//                             instance.SetExData(extraId, randomValue);
//                         }
//                     }
//                     instance.SetFlag(tbRecharge.FlagTrueId);
//                     var flagList = tbRecharge.FlagFalseId;
//                     for (var i = 0; i < flagList.Count; i++)
//                     {
//                         instance.SetFlag(flagList[i], false);
//                     }
//                     MenuItemClick(_menuSelectIndex);
//                     RefleshNoticeByExdata(extraId);
//                     var e = new ShowUIHintBoard(431);
//                     EventDispatcher.Instance.DispatchEvent(e);
//                 }
//                 else
//                 {
//                     UIManager.Instance.ShowNetError(msg.ErrorCode);
//                 }
//             }
        }
    }

    public void GainReward(int type, int id, int index)
    {
        NetManager.Instance.StartCoroutine(GainRewardCoroutine(type, id, index));
    }

    public IEnumerator GainRewardCoroutine(int type, int id, int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.GainReward(type, id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var mType = (eReChargeRewardType) type;
                    var instance = PlayerDataManager.Instance;
                    var flagId = 0;
                    var exdataId = 0;
                    switch (mType)
                    {
                        case eReChargeRewardType.Recharge:
                        {
                            var item = DataModel.Recharge.MainLists[index];
                            var tbRecharge = GetRechargeActiveInvestmentReward(id);
                            flagId = tbRecharge.Flag;
                            instance.SetFlag(flagId, true);
                            exdataId = GetRechargeActiveInvestment(tbRecharge.Type).ExtraId;
                            item.GetState = 2;
                        }
                            break;
                        case eReChargeRewardType.Investment:
                        {
                            var item = DataModel.Investment.MainLists[index];
                            var tbRecharge = GetRechargeActiveCumulativeReward(id);
                            flagId = tbRecharge.Flag;
                            instance.SetFlag(flagId, true);
                            exdataId = GetRechargeActiveCumulative(tbRecharge.Type).ExtraId;
                            item.GetState = 2;
                        }
                            break;
                    }
                    RefleshNoticeByExdata(exdataId);
                    RefleshMenuData();
                    MenuItemClick(_menuSelectIndex);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void InitAnalyse()
    {
        _mExtraIdToMenuItem.Clear();
        var menuList = DataModel.MenuLists;
        var instance = PlayerDataManager.Instance;
        foreach (var item in menuList)
        {
            var tbRecharge = GetRechargeActive(item.ActivityId);
            if (tbRecharge == null)
            {
                continue;
            }
            var type = tbRecharge.Type;
            switch (type)
            {
                case (int) eReChargeRewardType.Investment:
                {
                    var tbCumulative = GetRechargeActiveCumulative(tbRecharge.SonType);
                    var rewardList = _mInvestmentDic[tbCumulative.Id];
                    var count = rewardList.Count;
                    var flag = false;
                    for (var i = 0; i < count; i++)
                    {
                        var tbReward = GetRechargeActiveCumulativeReward(rewardList[i]);
                        if (instance.GetFlag(tbReward.Flag))
                        {
                            continue;
                        }
                        if (instance.CheckCondition(tbReward.ConditionId) == 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                    item.NoticeFlag = flag;
                    if (tbCumulative.ExtraId >= 0 && tbCumulative.ActivityId >= 0)
                    {
                        _mExtraIdToMenuItem[tbCumulative.ExtraId] = item;
                    }
                }
                    break;
                case (int) eReChargeRewardType.Recharge:
                {
                    var tbInvestment = GetRechargeActiveInvestment(tbRecharge.SonType);
                    var rewardList = _mReChargeDic[tbInvestment.Id];
                    var count = rewardList.Count;
                    var flag = false;
                    for (var i = 0; i < count; i++)
                    {
                        var tbReward = GetRechargeActiveInvestmentReward(rewardList[i]);
                        if (instance.GetFlag(tbReward.Flag))
                        {
                            continue;
                        }
                        if (instance.CheckCondition(tbReward.ConditionId) == 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                    item.NoticeFlag = flag;
                    if (tbInvestment.ExtraId >= 0 && tbInvestment.ActivityId >= 0)
                    {
                        _mExtraIdToMenuItem[tbInvestment.ExtraId] = item;
                    }
                }
                    break;
                case (int) eReChargeRewardType.FirstRecharge:
                {
                    item.NoticeFlag = instance.GetExData(eExdataDefine.e69) < 1;
                }
                    break;
            }
        }
        RefleshNoticeFlag();
    }

    private void InitData()
    {
        _mReChargeDic.Clear();
        _mInvestmentDic.Clear();
        ForeachRechargeActiveInvestmentReward(table =>
        {
            if (!_mReChargeDic.ContainsKey(table.Type))
            {
                _mReChargeDic[table.Type] = new List<int>();
            }
            _mReChargeDic[table.Type].Add(table.Id);
            return true;
        });
        ForeachRechargeActiveCumulativeReward(table =>
        {
            if (!_mInvestmentDic.ContainsKey(table.Type))
            {
                _mInvestmentDic[table.Type] = new List<int>();
            }
            _mInvestmentDic[table.Type].Add(table.Id);
            return true;
        });

        tableExdata.Clear();
        tableFlagTrue.Clear();
        tableFlagFalse.Clear();

        if (_RechargeActiveCumulativeTable != null && _RechargeActiveCumulativeTable.Records != null)
        {
            foreach (var tempData in _RechargeActiveCumulativeTable.Records.Values)
            {
                tableExdata.Add(tempData.ExtraId);
                tableFlagTrue.Add(tempData.FlagTrueId);
                tableFlagFalse.AddRange(tempData.FlagFalseId);
            }
        }
    }

    private void InitExData(IEvent ievent)
    {
        InitTables();
    }

    private void InitTables()
    {
        NetManager.Instance.StartCoroutine(ApplyRechargeTablesCoroutine());
        var mTime = Game.Instance.ServerTime.Date.AddDays(1);
        if (Trigger != null)
        {
            TimeManager.Instance.DeleteTrigger(Trigger);
            Trigger = null;
        }
        Trigger = TimeManager.Instance.CreateTrigger(mTime, () =>
        {
            NetManager.Instance.StartCoroutine(ApplyRechargeTablesCoroutine());
            TableUpdated = false;
        }, (int) TimeSpan.FromDays(1).TotalMilliseconds);
    }

    private void InitTables(IEvent ievent)
    {
        var count = MyRandom.Random(RandomSecondesToApplyTables);
        NetManager.Instance.StartCoroutine(WaitToSecondsToApplyTables(count));
    }

    private void InvestmentOperation(IEvent ievent)
    {
        var e = ievent as RechageActivityInvestmentOperation;
        var item = DataModel.Investment.MainLists[e.Index];
        if (item.GetState != 1)
        {
            return;
        }
        GainReward((int) eReChargeRewardType.Investment, item.Id, e.Index);
    }

    private void MenuClick(IEvent ievent)
    {
        var e = ievent as RechageActivityMenuItemClick;
        MenuItemClick(e.Index);
    }

    private void MenuItemClick(int index)
    {
        if (DataModel.MenuLists == null)
        {
            return;
        }

        if (index >= DataModel.MenuLists.Count)
        {
            return;
        }

        _menuSelectIndex = index;
        if (_mSelectedMenuItem != null)
        {
            _mSelectedMenuItem.Selected = 0;
        }
        _mSelectedMenuItem = DataModel.MenuLists[index];
        _mSelectedMenuItem.Selected = 1;
        var tbActivity = GetRechargeActive(_mSelectedMenuItem.ActivityId);
        var type = (eReChargeRewardType) tbActivity.Type;
        switch (type)
        {
            case eReChargeRewardType.Notice:
            {
                RefleshNoticePage(tbActivity);
            }
                break;
            case eReChargeRewardType.Recharge:
            {
                RefleshRechargePage(tbActivity);
            }
                break;
            case eReChargeRewardType.Investment:
            {
                RefleshInvestmentPage(tbActivity);
            }
                break;
            case eReChargeRewardType.FirstRecharge:
            {
                RefleshFirstPage(tbActivity);
            }
                break;
            case eReChargeRewardType.DaoHang:
            {
            }
                break;
        }
        DataModel.SelectType = tbActivity.Type;
    }

    private void OnOperation(IEvent ievent)
    {
        var e = ievent as RechageActivityOperation;
        switch (e.Type)
        {
            case 0:
            {
                var tbNotice = GetRechargeActiveNotice(DataModel.Notice.Id);
                GameUtils.GotoUiTab(tbNotice.GotoUiId, tbNotice.GotoUiTab);
            }
                break;
            case 1:
            {
                BuyInvestment();
            }
                break;
            case 2:
            {
                var tbNotice = GetRechargeActiveNotice(DataModel.FirstRecharge.Id);
                GameUtils.GotoUiTab(tbNotice.GotoUiId, tbNotice.GotoUiTab);
            }
                break;
        }
    }

    private void RefleshFirstPage(RechargeActiveEntry tbActivity)
    {
        var notice = DataModel.FirstRecharge;
        var sonType = tbActivity.SonType;
        //
        if (sonType >= 1 && sonType <= 3)
        {
            var roleId = ObjManager.Instance.MyPlayer.RoleId;
            sonType = roleId + 1;
        }
        notice.Id = sonType;
        var tbNotice = GetRechargeActiveNotice(sonType);
        notice.IsShowBtn = (tbNotice.IsBtnShow == 1);
        notice.TitleStr = tbActivity.LabelText;
        tbNotice.Desc = tbNotice.Desc.Replace(@"\", "");
        tbNotice.Desc = tbNotice.Desc.Replace("n", "\n");

        notice.MainStr = tbNotice.Desc;
        notice.Desc = tbNotice.Desc;
        notice.BtnText = tbNotice.BtnText;

        for (var i = 0; i < tbNotice.ItemId.Count; i++)
        {
            notice.ItemId[i] = tbNotice.ItemId[i];
            notice.ItemCount[i] = tbNotice.ItemCount[i];
        }
        if (PlayerDataManager.Instance.CheckCondition(tbNotice.ConditionId) == 0)
        {
            notice.GetState = 1;
        }
        else
        {
            notice.GetState = 0;
        }
    }

    private void RefleshInvestmentPage(RechargeActiveEntry tbActivity)
    {
        var investment = DataModel.Investment;
        var rechargeId = tbActivity.SonType;
        if (!_mInvestmentDic.ContainsKey(rechargeId))
        {
            return;
        }
        //"不限时"
        var varStr = GameUtils.GetDictionaryText(270285);
        //yyyy年MM月dd日hh:mm:ss
        var varStr2 = GameUtils.GetDictionaryText(270286);
        var tbRecharge = GetRechargeActiveCumulative(rechargeId);
        var values = _mInvestmentDic[rechargeId];
        var count = values.Count;
        var instance = PlayerDataManager.Instance;
        var totalDiaCount = 0;
        var list = new List<RechargeActivityInvestmentItemDataModel>();
        for (var i = 0; i < count; i++)
        {
            var item = new RechargeActivityInvestmentItemDataModel();
            var tbReward = GetRechargeActiveCumulativeReward(values[i]);
            item.Id = tbReward.Id;
            totalDiaCount += tbReward.ItemCount;
            if (instance.GetFlag(tbReward.Flag))
            {
                item.GetState = 2;
            }
            else
            {
                if (instance.CheckCondition(tbReward.ConditionId) == 0)
                {
                    item.GetState = 1;
                }
                else
                {
                    item.GetState = 0;
                }
            }
            item.Desc1 = tbReward.Desc1;
            item.Desc2 = tbReward.Desc2;
            item.ItemId = tbReward.ItemId;
            item.ItemCount = tbReward.ItemCount;
            list.Add(item);
        }
        var startTimeStr = string.Empty;
        var endTimeStr = string.Empty;
        if (tbActivity.OpenRule == (int) eRechargeActivityOpenRule.Last)
        {
            investment.DuringTime = varStr;
        }
        else if (tbActivity.OpenRule == (int) eRechargeActivityOpenRule.NewServerAuto)
        {
            startTimeStr = instance.OpenTime.AddHours(tbActivity.StartTime.ToInt()).ToString(varStr2);
            endTimeStr = instance.OpenTime.AddHours(tbActivity.EndTime.ToInt()).ToString(varStr2);
            investment.DuringTime = startTimeStr + "-" + endTimeStr;
        }
        else if (tbActivity.OpenRule == (int) eRechargeActivityOpenRule.LimitTime)
        {
            startTimeStr = Convert.ToDateTime(tbActivity.StartTime).ToString(varStr2);
            endTimeStr = Convert.ToDateTime(tbActivity.EndTime).ToString(varStr2);
            investment.DuringTime = startTimeStr + "-" + endTimeStr;
        }

        var strs = tbRecharge.BuyConditionText.Split('|');

        //DataModel.Recharge.TotalDiamond = instance.GetExData(tbRecharge.ExtraId);
        investment.Id = tbRecharge.Id;
        investment.Multiple = strs.Length > 1 ? int.Parse(strs[1]) : 0;
        var itemName = Table.GetItemBase(tbRecharge.NeedItemId).Name;
        investment.GetStr = "";//totalDiaCount + "倍" + itemName;
        investment.NeedStr = tbRecharge.NeedItemCount + itemName;
        investment.MainLists = new ObservableCollection<RechargeActivityInvestmentItemDataModel>(list);
        investment.CanBuy = instance.GetFlag(tbRecharge.FlagTrueId) ? 0 : 1;
        investment.TypeStr = tbRecharge.TypeStr;
        investment.BuyConditionText = strs[0];
        investment.BgIconId = tbRecharge.BgIconId;
    }

    private void RefleshMenuData()
    {
        var list = new List<RechargeActivityMenuItemDataModel>();
        var instance = PlayerDataManager.Instance;
        //"不限时"
        var varStr = GameUtils.GetDictionaryText(270285);
        var first = true;
        NearTime = Game.Instance.ServerTime;
        ForeachRechargeActive(table =>
        {
            if (!table.ServerIds.Contains(-1) && !table.ServerIds.Contains(PlayerDataManager.Instance.ServerId))
            {
                return true;
            }

            var serverTime = Game.Instance.ServerTime;
            if (table.OpenRule == (int) eRechargeActivityOpenRule.Last)
            {
                var item = new RechargeActivityMenuItemDataModel();
                item.ActivityId = table.Id;
                item.OverTime = Game.Instance.ServerTime.AddSeconds(-2);
                item.TimeLimitStr = varStr;
                var tb = GetRechargeActive(table.Id);
                item.Icon = tb.Icon;
                item.LabelText = tb.LabelText;
                list.Add(item);
            }
            else if (table.OpenRule == (int) eRechargeActivityOpenRule.NewServerAuto)
            {
                var tb = GetRechargeActive(table.Id);
                var overTime =
                    instance.OpenTime.AddHours(tb.EndTime.ToInt());
                var startTime = instance.OpenTime.AddHours(tb.StartTime.ToInt());

                if (table.Type == 2) //是投资活动 而且买了 就延长7天
                {
                    var sonId = table.SonType;
                    var tbTouZi = GetRechargeActiveCumulative(sonId);
                    if (tbTouZi != null)
                    {
                        var flag = tbTouZi.FlagTrueId;
                        if (PlayerDataManager.Instance.GetFlag(flag))
                        {
                            overTime = overTime.AddDays(7);
                        }
                    }
                }

                if (overTime < Game.Instance.ServerTime)
                {
                    return true;
                }
                if (startTime > Game.Instance.ServerTime)
                {
                    if (first)
                    {
                        NearTime = startTime;
                    }
                    else
                    {
                        if (startTime < NearTime)
                        {
                            NearTime = startTime;
                        }
                    }
                    first = false;
                }
                else
                {
                    var item = new RechargeActivityMenuItemDataModel();
                    item.ActivityId = table.Id;
                    item.OverTime = overTime;
                    item.TimeLimitStr = string.Empty;
                    item.Icon = tb.Icon;
                    item.LabelText = tb.LabelText;
                    list.Add(item);
                    if (first)
                    {
                        NearTime = overTime;
                    }
                    else
                    {
                        if (overTime < NearTime)
                        {
                            NearTime = overTime;
                        }
                    }
                    first = false;
                }
            }
            else if (table.OpenRule == (int) eRechargeActivityOpenRule.LimitTime)
            {
                var startTime = Convert.ToDateTime(table.StartTime);
                var overTime = Convert.ToDateTime(table.EndTime);

                if (table.Type == 2) //是投资活动 而且买了 就延长7天
                {
                    var sonId = table.SonType;
                    var tbTouZi = GetRechargeActiveCumulative(sonId);
                    if (tbTouZi != null)
                    {
                        var flag = tbTouZi.FlagTrueId;
                        if (PlayerDataManager.Instance.GetFlag(flag))
                        {
                            overTime = overTime.AddDays(7);
                        }
                    }
                }

                if (serverTime > startTime && serverTime < overTime)
                {
                    var item = new RechargeActivityMenuItemDataModel();
                    item.ActivityId = table.Id;
                    item.OverTime = overTime;
                    item.TimeLimitStr = string.Empty;
                    var tb = GetRechargeActive(table.Id);
                    item.Icon = tb.Icon;
                    item.LabelText = tb.LabelText;
                    list.Add(item);
                    if (first)
                    {
                        NearTime = overTime;
                    }
                    else
                    {
                        if (overTime < NearTime)
                        {
                            NearTime = overTime;
                        }
                    }
                    first = false;
                }
            }
            return true;
        });
        if (!first) //活动结束刷新menu
        {
            if (RefleshTrigger != null)
            {
                TimeManager.Instance.DeleteTrigger(RefleshTrigger);
                RefleshTrigger = null;
            }
            RefleshTrigger = TimeManager.Instance.CreateTrigger(NearTime, () =>
            {
                RefleshMenuData();
                MenuItemClick(_menuSelectIndex);
            });
        }

        DataModel.MenuLists = new ObservableCollection<RechargeActivityMenuItemDataModel>(list);
        if (DataModel.MenuLists.Count > 0)
        {
            DataModel.IsShowTouZiBtn = 1;
        }
        else
        {
            DataModel.IsShowTouZiBtn = 0;
        }
        InitAnalyse();
    }

    private void RefleshNoticeByExdata(int exdataId)
    {
        if (!_mExtraIdToMenuItem.ContainsKey(exdataId))
        {
            return;
        }
        var instance = PlayerDataManager.Instance;
        var item = _mExtraIdToMenuItem[exdataId];
        var tbRecharge = GetRechargeActive(item.ActivityId);
        var type = tbRecharge.Type;
        switch (type)
        {
            case (int) eReChargeRewardType.Investment:
            {
                var tbCumulative = GetRechargeActiveCumulative(tbRecharge.SonType);
                var rewardList = _mInvestmentDic[tbCumulative.Id];
                var count = rewardList.Count;
                var flag = false;
                for (var i = 0; i < count; i++)
                {
                    var tbReward = GetRechargeActiveCumulativeReward(rewardList[i]);
                    if (instance.GetFlag(tbReward.Flag))
                    {
                        continue;
                    }
                    if (instance.CheckCondition(tbReward.ConditionId) == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                item.NoticeFlag = flag;
            }
                break;
            case (int) eReChargeRewardType.Recharge:
            {
                var tbInvestment = GetRechargeActiveInvestment(tbRecharge.SonType);
                var rewardList = _mReChargeDic[tbInvestment.Id];
                var count = rewardList.Count;
                var flag = false;
                for (var i = 0; i < count; i++)
                {
                    var tbReward = GetRechargeActiveInvestmentReward(rewardList[i]);
                    if (instance.GetFlag(tbReward.Flag))
                    {
                        continue;
                    }
                    if (instance.CheckCondition(tbReward.ConditionId) == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                item.NoticeFlag = flag;
            }
                break;
        }
        RefleshNoticeFlag();
    }

    private void RefleshNoticeFlag()
    {
        var oldFlag = PlayerDataManager.Instance.NoticeData.RechageActivity;
        var menuList = DataModel.MenuLists;
        var flag = false;
        foreach (var item in menuList)
        {
            if (item.NoticeFlag)
            {
                flag = true;
                break;
            }
        }
        //var payCountTotal = PlayerDataManager.Instance.GetExData(eExdataDefine.e69);
        //flag = flag || payCountTotal < 1;
        PlayerDataManager.Instance.NoticeData.RechageActivity = flag;
    }

    private void RefleshNoticePage(RechargeActiveEntry tbActivity)
    {
        var notice = DataModel.Notice;
        var sonType = tbActivity.SonType;
        var tbNotice = GetRechargeActiveNotice(sonType);
        notice.IsShowBtn = (tbNotice.IsBtnShow == 1);
        notice.TitleStr = tbActivity.LabelText;
        tbNotice.Desc = tbNotice.Desc.Replace(@"\", "");
        tbNotice.Desc = tbNotice.Desc.Replace("n", "\n");
        notice.MainStr = tbNotice.Desc;
        notice.Desc = tbNotice.Desc;
        notice.BtnText = tbNotice.BtnText;

        for (var i = 0; i < tbNotice.ItemId.Count; i++)
        {
            notice.ItemId[i] = tbNotice.ItemId[i];
            notice.ItemCount[i] = tbNotice.ItemCount[i];
        }

        if (PlayerDataManager.Instance.CheckCondition(tbNotice.ConditionId) == 0)
        {
            notice.GetState = 1;
        }
        else
        {
            notice.GetState = 0;
        }
    }

    private void RefleshRechargePage(RechargeActiveEntry tbActivity)
    {
        var recharge = DataModel.Recharge;
        var rechargeId = tbActivity.SonType;
        if (!_mReChargeDic.ContainsKey(rechargeId))
        {
            return;
        }
        //"不限时"
        var varStr = GameUtils.GetDictionaryText(270285);
        //yyyy年MM月dd日hh:mm:ss
        var varStr2 = GameUtils.GetDictionaryText(270286);
        var tbRecharge = GetRechargeActiveInvestment(rechargeId);
        var values = _mReChargeDic[rechargeId];
        var count = values.Count;
        var btnText = tbRecharge.BtnText;
        var instance = PlayerDataManager.Instance;
        var list = new List<RechargeActivityRewardItemDataModel>();
        var day = 0;
        for (var i = 0; i < count; i++)
        {
            var item = new RechargeActivityRewardItemDataModel();
            var tbReward = GetRechargeActiveInvestmentReward(values[i]);
            item.Id = tbReward.Id;
            if (instance.GetFlag(tbReward.Flag))
            {
                item.GetState = 2;
            }
            else
            {
                if (instance.CheckCondition(tbReward.ConditionId) == 0)
                {
                    item.GetState = 1;
                }
                else
                {
                    item.GetState = 0;
                }
            }
            var index = tbRecharge.ConditionText.IndexOf("{#day}");
            if (index == -1)
            {
                item.ConditionText = string.Format(tbRecharge.ConditionText, tbReward.DiaNeedCount);
            }
            else
            {
                day++;
                item.ConditionText = tbRecharge.ConditionText.Replace("{#day}", day.ToString());
            }

            item.BtnText = btnText;
            for (var ii = 0; ii < tbReward.ItemId.Count; ii++)
            {
                item.ItemId[ii] = tbReward.ItemId[ii];
                item.ItemCount[ii] = tbReward.ItemCount[ii];
            }
            list.Add(item);
        }
        var startTimeStr = string.Empty;
        var endTimeStr = string.Empty;

        if (tbActivity.OpenRule == (int) eRechargeActivityOpenRule.Last)
        {
            recharge.DuringTime = varStr;
        }
        else if (tbActivity.OpenRule == (int) eRechargeActivityOpenRule.NewServerAuto)
        {
            startTimeStr = instance.OpenTime.AddHours(tbActivity.StartTime.ToInt()).ToString(varStr2);
            endTimeStr = instance.OpenTime.AddHours(tbActivity.EndTime.ToInt()).ToString(varStr2);
            recharge.DuringTime = startTimeStr + "-" + endTimeStr;
        }
        else if (tbActivity.OpenRule == (int) eRechargeActivityOpenRule.LimitTime)
        {
            startTimeStr = Convert.ToDateTime(tbActivity.StartTime).ToString(varStr2);
            endTimeStr = Convert.ToDateTime(tbActivity.EndTime).ToString(varStr2);
            recharge.DuringTime = startTimeStr + "-" + endTimeStr;
        }


        recharge.Id = tbRecharge.Id;
        recharge.TotalDiamond = instance.GetExData(tbRecharge.ExtraId);
        recharge.TotalDiamondStr = string.Format(tbRecharge.Tips, recharge.TotalDiamond);
        recharge.Type = tbRecharge.Type;
        recharge.Tips = tbRecharge.Tips;
        recharge.ConditionText = tbRecharge.ConditionText;
        recharge.BtnText = tbRecharge.BtnText;
        recharge.BgIconId = tbRecharge.BgIconId;
        recharge.MainLists = new ObservableCollection<RechargeActivityRewardItemDataModel>(list);
    }

    private void RewardOperation(IEvent ievent)
    {
        var e = ievent as RechageActivityRewardOperation;
        var item = DataModel.Recharge.MainLists[e.Index];
        if (item.GetState != 1)
        {
            return;
        }
        GainReward((int) eReChargeRewardType.Recharge, item.Id, e.Index);
    }

    private void UpdateFlag(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        if (e == null)
        {
            return;
        }
        ChargeSuccess(e.Index);

        if (tableFlagTrue.Contains(e.Index) || tableFlagFalse.Contains(e.Index))
        {
            RefleshMenuData();
        }
    }

    private void UpdateExdata(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        if (e.Key == (int) eExdataDefine.e69)
        {
            var str = string.Empty;
            if (e.Value < 1)
            {
                //首充
                str = GameUtils.GetDictionaryText(100000587);
            }
            else
            {
                str = GameUtils.GetDictionaryText(100001000);
            }
            if (str != MainLabelStr)
            {
                EventDispatcher.Instance.DispatchEvent(new FirstRechargeTextSet_Event(str));
            }
            MainLabelStr = str;
            return;
        }
        RefleshNoticeByExdata(e.Key);
        if (tableExdata.Contains(e.Key))
        {
            RefleshMenuData();
            MenuItemClick(_menuSelectIndex);
        }
       
    }

    private void ChargeSuccess(int index)
    {
        if (_RechargeActiveCumulativeTable == null || _RechargeActiveCumulativeTable.Records == null)
        {
            return;
        }
        var has = false;
        foreach (var tempData in _RechargeActiveCumulativeTable.Records.Values)
        {
            if (tempData.FlagTrueId == index)
            {
                has = true;
            }
        }
        if (has)
        {
            MenuItemClick(_menuSelectIndex);
            var e = new ShowUIHintBoard(431);
            EventDispatcher.Instance.DispatchEvent(e); 
        }
        
    }

    private IEnumerator WaitToSecondsToApplyTables(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        InitTables();
    }

    public void CleanUp()
    {
        DataModel = new RechargeActivityDataModel();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void RefreshData(UIInitArguments data)
    {
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnShow()
    {
        RefleshMenuData();
        if (DataModel.MenuLists.Count > 0)
        {
            MenuItemClick(_menuSelectIndex);
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }

    #region 数据转换

    private RechargeActiveTable _RechargeActiveTable;
    private RechargeActiveNoticeTable _RechargeActiveNoticeTable;
    private RechargeActiveCumulativeRewardTable _RechargeActiveCumulativeRewardTable;
    private RechargeActiveInvestmentRewardTable _RechargeActiveInvestmentRewardTable;
    private RechargeActiveCumulativeTable _RechargeActiveCumulativeTable;
    private RechargeActiveInvestmentTable _RechargeActiveInvestmentTable;

    public void ForeachRechargeActive(Func<RechargeActiveEntry, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach RechargeActive act is null");
            return;
        }

        if (_RechargeActiveTable == null)
        {
            return;
        }

        if (_RechargeActiveTable.Records == null)
        {
            return;
        }

        foreach (var tempRecord in _RechargeActiveTable.Records)
        {
            try
            {
                if (!act(tempRecord.Value))
                {
                    break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public RechargeActiveEntry GetRechargeActive(int nId)
    {
        RechargeActiveEntry tbRechargeActive;
        if (!_RechargeActiveTable.Records.TryGetValue(nId, out tbRechargeActive))
        {
            Logger.Info("RechargeActive[{0}] not find by Table", nId);
            return null;
        }
        return tbRechargeActive;
    }

    public void ForeachRechargeActiveNotice(Func<RechargeActiveNoticeEntry, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach RechargeActiveNotice act is null");
            return;
        }
        foreach (var tempRecord in _RechargeActiveNoticeTable.Records)
        {
            try
            {
                if (!act(tempRecord.Value))
                {
                    break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public RechargeActiveNoticeEntry GetRechargeActiveNotice(int nId)
    {
        RechargeActiveNoticeEntry tbRechargeActiveNotice;
        if (!_RechargeActiveNoticeTable.Records.TryGetValue(nId, out tbRechargeActiveNotice))
        {
            Logger.Info("RechargeActiveNotice[{0}] not find by Table", nId);
            return null;
        }
        return tbRechargeActiveNotice;
    }

    public void ForeachRechargeActiveInvestment(Func<RechargeActiveInvestmentEntry, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach RechargeActiveInvestment act is null");
            return;
        }
        foreach (var tempRecord in _RechargeActiveInvestmentTable.Records)
        {
            try
            {
                if (!act(tempRecord.Value))
                {
                    break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public RechargeActiveInvestmentEntry GetRechargeActiveInvestment(int nId)
    {
        RechargeActiveInvestmentEntry tbRechargeActiveInvestment;
        if (!_RechargeActiveInvestmentTable.Records.TryGetValue(nId, out tbRechargeActiveInvestment))
        {
            Logger.Info("RechargeActiveInvestment[{0}] not find by Table", nId);
            return null;
        }
        return tbRechargeActiveInvestment;
    }

    public void ForeachRechargeActiveInvestmentReward(Func<RechargeActiveInvestmentRewardEntry, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach RechargeActiveInvestmentReward act is null");
            return;
        }
        foreach (var tempRecord in _RechargeActiveInvestmentRewardTable.Records)
        {
            try
            {
                if (!act(tempRecord.Value))
                {
                    break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public RechargeActiveInvestmentRewardEntry GetRechargeActiveInvestmentReward(int nId)
    {
        RechargeActiveInvestmentRewardEntry tbRechargeActiveInvestmentReward;
        if (!_RechargeActiveInvestmentRewardTable.Records.TryGetValue(nId, out tbRechargeActiveInvestmentReward))
        {
            Logger.Info("RechargeActiveInvestmentReward[{0}] not find by Table", nId);
            return null;
        }
        return tbRechargeActiveInvestmentReward;
    }

    public void ForeachRechargeActiveCumulative(Func<RechargeActiveCumulativeEntry, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach RechargeActiveCumulative act is null");
            return;
        }
        foreach (var tempRecord in _RechargeActiveCumulativeTable.Records)
        {
            try
            {
                if (!act(tempRecord.Value))
                {
                    break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public RechargeActiveCumulativeEntry GetRechargeActiveCumulative(int nId)
    {
        RechargeActiveCumulativeEntry tbRechargeActiveCumulative;
        if (!_RechargeActiveCumulativeTable.Records.TryGetValue(nId, out tbRechargeActiveCumulative))
        {
            Logger.Info("RechargeActiveCumulative[{0}] not find by Table", nId);
            return null;
        }
        return tbRechargeActiveCumulative;
    }

    public void ForeachRechargeActiveCumulativeReward(Func<RechargeActiveCumulativeRewardEntry, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach RechargeActiveCumulativeReward act is null");
            return;
        }
        foreach (var tempRecord in _RechargeActiveCumulativeRewardTable.Records)
        {
            try
            {
                if (!act(tempRecord.Value))
                {
                    break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public RechargeActiveCumulativeRewardEntry GetRechargeActiveCumulativeReward(int nId)
    {
        RechargeActiveCumulativeRewardEntry tbRechargeActiveCumulativeReward;
        if (!_RechargeActiveCumulativeRewardTable.Records.TryGetValue(nId, out tbRechargeActiveCumulativeReward))
        {
            Logger.Info("RechargeActiveCumulativeReward[{0}] not find by Table", nId);
            return null;
        }
        return tbRechargeActiveCumulativeReward;
    }

    #endregion
}