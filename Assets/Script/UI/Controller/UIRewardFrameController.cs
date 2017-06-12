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

#endregion

public class UIRewardFrameController : IControllerBase
{
    public UIRewardFrameController()
    {
        InitTableAfter_AnalyseNotice();
        CacheTable();

        CleanUp();

        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, evn => { Init(); });

        EventDispatcher.Instance.AddEventListener(UIEvent_CliamReward.EVENT_TYPE, OnCliamReward);


        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpdateEvent);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExtDataUpDataEvent);

        EventDispatcher.Instance.AddEventListener(Event_UpdateOnLineReward.EVENT_TYPE, UpdateOnLineRewardEvent);

        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, UpdateLevelReward);

        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, AnalyseNoticeByFlag);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, AnalyseNoticeByExdata);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, UpdateLevelNotice);
        EventDispatcher.Instance.AddEventListener(LevelUpInitEvent.EVENT_TYPE, UpdateLevelNotice);

        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, AnalyseNoticeInit);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, AnalyseNoticeInit);

        EventDispatcher.Instance.AddEventListener(UIEvent_GetOnLineSeconds.EVENT_TYPE, OnGetOnLineSeconds);
        EventDispatcher.Instance.AddEventListener(UIEvent_ActivityCompensateItem.EVENT_TYPE, OnClickCompensateItem);
        EventDispatcher.Instance.AddEventListener(UIEvent_UseGiftCodeEvent.EVENT_TYPE, OnUseGiftCode);
    }

    //缓存标记位影响哪些成就
    private readonly Dictionary<int, List<GiftRecord>> mFlagDataDict = new Dictionary<int, List<GiftRecord>>();
    public bool mHasBeenInited;
    ////缓存扩展计数影响哪些成就
    //Dictionary<int, List<GiftRecord>> mExtDataDict = new Dictionary<int, List<GiftRecord>>();

    //缓存各个类型奖励表格数据
    public List<GiftRecord>[] mTableCache = new List<GiftRecord>[(int) eRewardType.DailyActivityReward + 1];
    public RewardDataModel RewardData { get; set; }

    public void AddFlagDict(int idx, GiftRecord table)
    {
        List<GiftRecord> list = null;
        if (!mFlagDataDict.TryGetValue(idx, out list))
        {
            list = new List<GiftRecord>();
            mFlagDataDict.Add(idx, list);
        }
        list.Add(table);
    }

    public void AnalyseNoticeInit(IEvent ievent)
    {
        AnalyseNotice();
    }

    //缓存表格数据，只调一次
    public void CacheTable()
    {
        for (var i = 0; i < mTableCache.Length; i++)
        {
            mTableCache[i] = new List<GiftRecord>();
        }

        Table.ForeachGift(table =>
        {
            var type = (eRewardType) table.Type;

            if (eRewardType.OnlineReward == type)
            {
                mTableCache[table.Type].Add(table);
                AddFlagDict(table.Flag, table);
            }
            else if (eRewardType.LevelReward == type)
            {
                mTableCache[table.Type].Add(table);
                AddFlagDict(table.Flag, table);
            }
            else if (eRewardType.ContinuesLoginReward == type)
            {
                mTableCache[table.Type].Add(table);
            }
            else if (eRewardType.MonthCheckinReward == type)
            {
                mTableCache[table.Type].Add(table);
            }
            //else if (eRewardType.DailyActivity == type)
            //{
            //    mTableCache[table.Type].Add(table);
            //    AddExtDataDict(table.Exdata, table);
            //}
            else if (eRewardType.DailyActivityReward == type)
            {
                mTableCache[table.Type].Add(table);
                AddFlagDict(table.Flag, table);
            }

            return true;
        });
    }

    //public void AddExtDataDict(int idx, GiftRecord table)
    //{
    //    List<GiftRecord> list = null;
    //    if (!mExtDataDict.TryGetValue(idx, out list))
    //    {
    //        list = new List<GiftRecord>();
    //        mExtDataDict.Add(idx, list);
    //    }
    //    list.Add(table);
    //}

    private void Init()
    {
        if (mHasBeenInited)
        {
            return;
        }
        mHasBeenInited = true;

        UpdateOnLineReward();
        UpdateLevelReward();
        UpdateContinuesLoginReward();
        UpdateMonthCheckinReward();
        UpdateActivity();
        UpdateActivityReward();
    }

    //用表格数据初始化数据源
    public void InitByTable()
    {
        //在线奖励
        RewardData.OnLineReward.Rewards.Clear();
        {
            var __list10 = mTableCache[(int) eRewardType.OnlineReward];
            var __listCount10 = __list10.Count;
            for (var __i10 = 0; __i10 < __listCount10; ++__i10)
            {
                var table = __list10[__i10];
                {
                    RewardData.OnLineReward.Rewards.Add(MakeOnLineReward(table));
                }
            }
        }
        //等级奖励
        RewardData.LevelReward.Rewards.Clear();
        {
            var __list11 = mTableCache[(int) eRewardType.LevelReward];
            var __listCount11 = __list11.Count;
            for (var __i11 = 0; __i11 < __listCount11; ++__i11)
            {
                var table = __list11[__i11];
                {
                    RewardData.LevelReward.Rewards.Add(MakeLevelReward(table));
                }
            }
        }
        //连续登录奖励
        {
            var rewardList = RewardData.ContinuesLoginReward.Rewards;
            var tableList = mTableCache[(int) eRewardType.ContinuesLoginReward];

            var c = rewardList.Count;
            for (var i = 0; i < c; i++)
            {
                var reward = rewardList[i];
                reward.ItemId = -1;
                reward.Count = 0;
            }

            c = tableList.Count;
            for (var i = 0; i < c; i++)
            {
                var table = tableList[i];
                var days = table.Param[ContinuesLoginRewardParamterIndx.Days];
                var idx = days - 1;

                if (idx >= 0 && idx < rewardList.Count)
                {
                    var reward = rewardList[idx];
                    reward.ItemId = table.Param[ContinuesLoginRewardParamterIndx.ItemId];
                    reward.Count = table.Param[ContinuesLoginRewardParamterIndx.ItemCount];
                }
            }
        }

        //每月累计签到奖励
        RewardData.MonthCheckinReward.Rewards.Clear();
        {
            {
                var __list12 = mTableCache[(int) eRewardType.MonthCheckinReward];
                var __listCount12 = __list12.Count;
                for (var __i12 = 0; __i12 < __listCount12; ++__i12)
                {
                    var table = __list12[__i12];
                    {
                        MakeMonthCheckinReward(table);
                    }
                }
            }
        }

        ////每日活跃任务
        //RewardData.ActivityReward.Activity.Clear();
        //{
        //    var __list13 = mTableCache[(int) eRewardType.DailyActivity];
        //    var __listCount13 = __list13.Count;
        //    for (int __i13 = 0; __i13 < __listCount13; ++__i13)
        //    {
        //        var table = __list13[__i13];
        //        {
        //            RewardData.ActivityReward.Activity.Add(MakeActivity(table));
        //        }
        //    }
        //}

        //每日活跃奖励
        RewardData.ActivityReward.ActivityReward.Clear();
        {
            var __list14 = mTableCache[(int) eRewardType.DailyActivityReward];
            var __listCount14 = __list14.Count;
            for (var __i14 = 0; __i14 < __listCount14; ++__i14)
            {
                var table = __list14[__i14];
                {
                    RewardData.ActivityReward.ActivityReward.Add(MakeActivityReward(table));
                }
            }
        }

        CompensateName.Clear();
        Table.ForeachCompensation(table =>
        {
            if (CompensateName.ContainsKey(table.Type))
            {
                return true;
            }
            CompensateName.Add(table.Type, table.Name);
            return true;
        }
            );
        ExpProp = int.Parse(Table.GetClientConfig(584).Value);
        GoodProp = int.Parse(Table.GetClientConfig(585).Value);
        DiamondParm = int.Parse(Table.GetClientConfig(586).Value);
        ResParm = int.Parse(Table.GetClientConfig(587).Value);
    }

    public void UpdateLevelNotice(IEvent ievent)
    {
        AnalyseNoticeByFlag(305);
    }

    public void CleanUp()
    {
        RewardData = new RewardDataModel();
        InitByTable();
        mHasBeenInited = false;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "InitCompensate")
        {
            var items = param[0] as Dictionary<int, Compensation>;
            if (items != null)
            {
                InitCompensate(items);
            }
        }
        return null;
    }

    public void OnShow()
    {
    }

    public void Close()
    {
        //	RewardData.FirstPage = 0;
        RewardData.Tab = 0;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var tab = data as UIRewardFrameArguments;
        if (null != tab)
        {
            RewardData.Tab = tab.Tab;
        }
        else
        {
            var noticeData = PlayerDataManager.Instance.NoticeData;
            if (noticeData.ActivityTimeLength > 0)
            {
                RewardData.Tab = 0;
            }
            else if (noticeData.ActivityLevel > 0)
            {
                RewardData.Tab = 1;
            }
            else if (noticeData.ActivityLoginSeries > 0)
            {
                RewardData.Tab = 2;
            }
            else if (noticeData.ActivityLoginAddup > 0)
            {
                RewardData.Tab = 3;
            }
            else if (noticeData.ActivityCompensateActive > 0)
            {
                RewardData.Tab = 4;
            }
            else
            {
                RewardData.Tab = 0;
            }
        }
        RewardData.Compensate.ShowWitchConfirm = 0;
        EventDispatcher.Instance.DispatchEvent(new UI_Event_OffLineExp(3)); //开界面刷新一次离线经验
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return RewardData;
    }

    public FrameState State { get; set; }

    #region 在线奖励

    public OnLineRewardItemDataModel MakeOnLineReward(GiftRecord table)
    {
        var min = table.Param[OnLineRewardParamterIndx.Minutes]/60;
        var str = min + GameUtils.GetDictionaryText(1041);
        var dataModel = new OnLineRewardItemDataModel
        {
            Id = table.Id,
            Minutes = str,
            Seconds = table.Param[OnLineRewardParamterIndx.Minutes],
            Item = new ItemIconDataModel
            {
                ItemId = table.Param[OnLineRewardParamterIndx.ItemId],
                Count = table.Param[OnLineRewardParamterIndx.ItemCount]
            },
            CanGetReward = false,
            HasGotReward = 0
        };
        return dataModel;
    }

    public void UpdateOnLineRewardEvent(IEvent ievent)
    {
        UpdateOnLineReward();
    }

    public void UpdateOnLineReward()
    {
        var onLineSeconds = GetOnLineSeconds();
        var time = TimeSpan.FromSeconds(onLineSeconds);

        var canGetCount = 0;
        var hasGot = 0;
        {
            // foreach(var item in RewardData.OnLineReward.Rewards)
            var find = false;
            var __enumerator2 = (RewardData.OnLineReward.Rewards).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var item = __enumerator2.Current;
                {
                    var state = GetOnLineRewardState(item.Id);
                    if (state == eRewardState.HasGot)
                    {
                        item.HasGotReward = 1;
                        item.CanGetReward = false;
                        hasGot++;
                        item.TimeDesc = "";
                    }
                    else if (state == eRewardState.CanGet)
                    {
                        item.HasGotReward = 0;
                        item.CanGetReward = true;
                        item.TimeDesc = "";
                        canGetCount++;
                    }
                    else
                    {
                        item.HasGotReward = 0;
                        item.CanGetReward = false;
                        if (time.TotalSeconds < item.Seconds)
                        {
                            if (!find)
                            {
                                var diff = (int) (item.Seconds - time.TotalSeconds);
                                item.TimeDesc = string.Format("{0:D2}:{1:D2}", diff/60, diff%60);
                                find = true;
                            }
                            else
                            {
                                item.TimeDesc = "";
                            }
                        }
                        else
                        {
                            item.TimeDesc = "";
                        }
                    }
                }
            }
        }
        RewardData.OnLineReward.OnLineTip = string.Format(GameUtils.GetDictionaryText(515), time.Hours, time.Minutes,
            time.Seconds, RewardData.OnLineReward.Rewards.Count - hasGot);

        var noticeData = PlayerDataManager.Instance.NoticeData;
        if (null != noticeData)
        {
            noticeData.ActivityTimeLength = canGetCount;
        }
    }

    #endregion

    #region 等级奖励

    public LevelRewardItemDataModel MakeLevelReward(GiftRecord table)
    {
        var dataModel = new LevelRewardItemDataModel
        {
            Id = table.Id,
            Level = table.Param[LevelRewardParamterIndx.Level].ToString()
        };
        var intLevelRewardParamterIndxItemId_Max0 = LevelRewardParamterIndx.ItemId_Max;
        for (var i = LevelRewardParamterIndx.ItemId_1; i <= intLevelRewardParamterIndxItemId_Max0; i++)
        {
            var itemId = table.Param[i];
            if (-1 == itemId)
            {
                break;
            }

            var item = new ItemIconDataModel
            {
                ItemId = itemId,
                Count = 1
            };
            dataModel.Rewards.Add(item);
        }

        return dataModel;
    }

    public void UpdateLevelReward(IEvent ievent)
    {
        UpdateLevelReward();
    }

    public void UpdateLevelReward()
    {
        {
            // foreach(var item in RewardData.LevelReward.Rewards)
            var __enumerator3 = (RewardData.LevelReward.Rewards).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var item = __enumerator3.Current;
                {
                    var state = GetLevelRewardState(item.Id);
                    if (state == eRewardState.HasGot)
                    {
                        item.HasGotReward = 1;
                        item.CanGetReward = false;
                    }
                    else
                    {
                        item.HasGotReward = 0;
                        item.CanGetReward = eRewardState.CanGet == state;
                    }
                }
            }
        }
    }

    #endregion

    #region 连续登录奖励

    public void MakeContinuesLoginReward(GiftRecord table)
    {
        var rewards = RewardData.ContinuesLoginReward.Rewards;
        var days = table.Param[ContinuesLoginRewardParamterIndx.Days];
        // 		int needAdd = days - rewards.Count;
        // 		if (needAdd > 0)
        // 		{
        // 			for (int i = 0; i < needAdd; i++)
        // 			{
        // 				rewards.Add(new ItemIconDataModel());
        // 			}
        // 		}
        var idx = days - 1;
        rewards[idx].ItemId = table.Param[ContinuesLoginRewardParamterIndx.ItemId];
        rewards[idx].Count = 1;
    }

    public void UpdateContinuesLoginReward()
    {
        if (HasGotTodayLoginReward())
        {
            RewardData.ContinuesLoginReward.HasGot = 1;
            RewardData.ContinuesLoginReward.CanGet = false;
        }
        else
        {
            RewardData.ContinuesLoginReward.HasGot = 0;
            RewardData.ContinuesLoginReward.CanGet = true;
        }
        var str = string.Format(GameUtils.GetDictionaryText(516), GetContinuesLoginDayNumber());
        RewardData.ContinuesLoginReward.Tip = str;

        var str1 = string.Format(GameUtils.GetDictionaryText(517), GetContinuesLoginDayNumber());
        RewardData.ContinuesLoginReward.Tip1 = str1;
        RewardData.ContinuesLoginReward.Days = GetContinuesLoginDayNumber();
    }

    #endregion

    #region 每月签到

    public void MakeMonthCheckinReward(GiftRecord table)
    {
        var rewards = RewardData.MonthCheckinReward.Rewards;

        var month = table.Param[MonthCheckinRewardParamterIndx.Month];
        if (999 != month && Game.Instance.ServerTime.Month != month)
        {
            return;
        }

        var days = table.Param[MonthCheckinRewardParamterIndx.Day];
        var itemId = table.Param[MonthCheckinRewardParamterIndx.ItemId];
        var itemCount = table.Param[MonthCheckinRewardParamterIndx.ItemCount];
        var cost = table.Param[MonthCheckinRewardParamterIndx.CostDiamond];

        var needAdd = days - rewards.Count;
        if (needAdd > 0)
        {
            for (var i = 0; i < needAdd; i++)
            {
                rewards.Add(new MonthCheckinRewardItemDataModel());
            }
        }
        var idx = days - 1;
        rewards[idx].Id = table.Id;
        rewards[idx].ItemId = itemId;
        rewards[idx].Count = itemCount;
        rewards[idx].Index = idx + 1;
    }

    public void UpdateMonthCheckinReward(IEvent ievent)
    {
        UpdateMonthCheckinReward();
    }

    public void UpdateMonthCheckinReward()
    {
        var monthCheckinReward = RewardData.MonthCheckinReward;
        var rewards = monthCheckinReward.Rewards;

        var date = Game.Instance.ServerTime;

        //本月已签次数
        var checkinTimes = GetMonthCheckinTimes();

        //是否全签
        var allChecked = checkinTimes >= rewards.Count;

        //本月可补签次数
        var remainCheckTimes = 0;
        if (true != allChecked)
        {
            var total = Math.Min(rewards.Count, date.Day);
            remainCheckTimes = total - checkinTimes;
            if (!HasTodayCheckined())
            {
                remainCheckTimes -= 1;
            }
        }

        monthCheckinReward.CurrentMonth = string.Format(GameUtils.GetDictionaryText(518), date.Month);
        monthCheckinReward.CurrentMonthCheckinTimes = string.Format(GameUtils.GetDictionaryText(519), checkinTimes);
        monthCheckinReward.CurrentMonthReCheckinTimes = string.Format(GameUtils.GetDictionaryText(520), remainCheckTimes);
        monthCheckinReward.CanCheckin = false;
        monthCheckinReward.CanReCheckin = false;

        if (allChecked)
        {
//全签
            {
                // foreach(var item in monthCheckinReward.Rewards)
                var __enumerator4 = (monthCheckinReward.Rewards).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var item = __enumerator4.Current;
                    {
                        item.Selected = false;
                        item.HasGotReward = true;
                    }
                }
            }
        }
        else
        {
            var selectIdx = checkinTimes;
            if (HasTodayCheckined())
            {
                selectIdx = remainCheckTimes > 0 ? checkinTimes : checkinTimes - 1;
            }

            var idx = 0;
            {
                // foreach(var item in monthCheckinReward.Rewards)
                var __enumerator5 = (monthCheckinReward.Rewards).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var item = __enumerator5.Current;
                    {
                        item.Selected = idx == selectIdx;
                        item.HasGotReward = idx < checkinTimes;
                        idx++;
                    }
                }
            }

            if (!HasTodayCheckined())
            {
                monthCheckinReward.CanCheckin = true;
            }
            else if (remainCheckTimes > 0)
            {
                monthCheckinReward.CanReCheckin = true;
            }
        }
    }

    #endregion

    #region 积分奖励

    public ActivityItemDataModel MakeActivity(GiftRecord table)
    {
        var type = (eRewardType) table.Type;

        var id = table.Id;
        var score = table.Param[ActivityParamterIndx.Score].ToString();
        var desc = GameUtils.GetDictionaryText(table.Param[ActivityParamterIndx.DescId]);

        var dataModel = new ActivityItemDataModel
        {
            Id = id,
            Desc = desc,
            Score = score
        };
        return dataModel;
    }

    //每日活跃奖励
    public ActivityRewardItemDataModel MakeActivityReward(GiftRecord table)
    {
        var id = table.Id;
        var itemId = table.Param[ActivityRewardParamterIndx.ItemId];
        var count = table.Param[ActivityRewardParamterIndx.Count];
        var needScore = table.Param[ActivityRewardParamterIndx.NeedScore];

        var dataModel = new ActivityRewardItemDataModel
        {
            Id = id,
            NeedScore = string.Format(GameUtils.GetDictionaryText(528), needScore),
            Item = new ItemIconDataModel
            {
                ItemId = itemId,
                Count = count
            }
        };

        return dataModel;
    }


    public void UpdateActivity()
    {
        //var activity = RewardData.ActivityReward.Activity;
        var activity = new List<ActivityItemDataModel>(RewardData.ActivityReward.Activity);

        var __list6 = activity;
        var __listCount6 = __list6.Count;

        var sort = false;
        for (var __i6 = 0; __i6 < __listCount6; ++__i6)
        {
            var item = __list6[__i6];
            {
                var table = Table.GetGift(item.Id);
                var needTimes = table.Param[ActivityParamterIndx.NeedTimes];
                var progress = GetActivityProgress(item.Id);

                var backup = item.Done;

                if (progress < needTimes)
                {
                    item.Done = false;
                }
                else
                {
                    progress = needTimes;
                    item.Done = true;
                }

                if (!sort)
                {
                    if (backup != item.Done)
                    {
                        sort = true;
                    }
                }

                if (item.Done)
                {
                    item.Doing = false;
                }
                else
                {
                    var uiId = table.Param[ActivityParamterIndx.UIId];
                    if (-1 != uiId)
                    {
                        item.Doing = true;
                    }
                    else
                    {
                        item.Doing = false;
                    }
                }
                item.Progress = string.Format(item.Done ? "[00FF00]{0}/{1}[-]" : "[FFDB93]{0}/{1}[-]", progress,
                    needTimes);
            }
        }


        if (sort)
        {
            activity.Sort(SortActivity);
            RewardData.ActivityReward.Activity = new ObservableCollection<ActivityItemDataModel>(activity);
        }
    }

    public static int SortActivity(ActivityItemDataModel a, ActivityItemDataModel b)
    {
        var va = (a.Done ? 1000000 : 0) + a.Id;

        var vb = (b.Done ? 1000000 : 0) + b.Id;

        if (va > vb)
        {
            return 1;
        }
        if (vb > va)
        {
            return -1;
        }
        return 0;
    }

    public void UpdateActivityReward()
    {
        var score = GetActivityScore();
        var reward = RewardData.ActivityReward.ActivityReward;
        {
            // foreach(var item in reward)
            var __enumerator7 = (reward).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var item = __enumerator7.Current;
                {
                    item.HasGot = eRewardState.HasGot == GetActivityRewardState(item.Id) ? 1 : 0;
                    item.CanGet = eRewardState.CanGet == GetActivityRewardState(item.Id);
                }
            }
        }


        RewardData.ActivityReward.ScoreLabel = score.ToString();
    }

    #endregion

    #region 补偿奖励

    private readonly Dictionary<int, string> CompensateName = new Dictionary<int, string>(); //int:type string:name
    private int GoodProp;
    private int ExpProp;
    private int DiamondParm;
    private int ResParm;

    private int GoldOrDia; //选择为金币或者钻石
    private int CompensateOkType; //选择 0 一键金币  1 一键钻石  2 所选择的item


    private void InitCompensate(Dictionary<int, Compensation> compensationList)
    {
        // foreach(var item in compensationList.Compensations)
        RewardData.Compensate.ItemList.Clear();
        var list = new List<ActivityCompensateItemDataModel>();
        var __enumerator2 = (compensationList).GetEnumerator();
        while (__enumerator2.MoveNext())
        {
            var item = __enumerator2.Current;
            {
                var itemData = new ActivityCompensateItemDataModel();
                itemData.Id = item.Key;
                itemData.NeedGood = 0;
                itemData.NeedDia = 0;
                var __enumerator3 = (item.Value.Data).GetEnumerator();
                if (item.Value.Data.Count == 0)
                {
                    continue;
                }
                var itemList = new List<ItemIconDataModel>();
                while (__enumerator3.MoveNext())
                {
                    var item2 = __enumerator3.Current;
                    {
                        var dd = new ItemIconDataModel();
                        dd.ItemId = item2.Key;
                        dd.Count = item2.Value;
                        itemList.Add(dd);
                        if (item2.Key == 1)
                        {
                            itemData.NeedGood += (item2.Value + ExpProp - 1)/ExpProp;
                        }
                        else if (item2.Key == 2)
                        {
                            itemData.NeedGood += (item2.Value + GoodProp - 1)/GoodProp;
                        }
                        else
                        {
                            var tbItemBase = Table.GetItemBase(item2.Key);
                            if (tbItemBase != null)
                            {
                                itemData.NeedGood += item2.Value*tbItemBase.ItemValue;
                            }
                        }
                    }
                }
                itemData.LeftCount = item.Value.Count;
                itemData.NeedDia = (itemData.NeedGood + DiamondParm - 1)/DiamondParm;

                itemData.GetList = new ObservableCollection<ItemIconDataModel>(itemList);
                if (CompensateName.ContainsKey(itemData.Id))
                {
                    itemData.Name = CompensateName[itemData.Id];
                }

                list.Add(itemData);
            }
        }
        if (list.Count == 0)
        {
            RewardData.Compensate.IsEmpty = 1;
        }
        else
        {
            RewardData.Compensate.IsEmpty = 0;
        }
        PlayerDataManager.Instance.NoticeData.ActivityCompensateActive = list.Count;
        RewardData.Compensate.ItemList = new ObservableCollection<ActivityCompensateItemDataModel>(list);
    }

    public int CompensateSelectIndex;

    private void OnClickCompensateItem(IEvent ievent)
    {
        var e = ievent as UIEvent_ActivityCompensateItem;
        CompensateSelectIndex = e.Idx;
        CompensateOkType = 2;
        float prop = 1;
        if (e.Type == 2)
        {
            prop = (float) ResParm/10000;
            GoldOrDia = 0;
            RewardData.Compensate.ShowWitchConfirm = 1;
        }
        else if (e.Type == 3)
        {
            prop = 1;
            GoldOrDia = 1;
            RewardData.Compensate.ShowWitchConfirm = 2;
        }
        RewardData.Compensate.SelectedItem = CopyActivityCompensateItem(RewardData.Compensate.ItemList[e.Idx], prop);
    }

    private void OnUseGiftCode(IEvent ievent)
    {
        var e = ievent as UIEvent_UseGiftCodeEvent;
        UseGiftCode(e.Code);
    }

    private void UseGiftCode(string code)
    {
        NetManager.Instance.StartCoroutine(UseGiftCodeCoroutine(code));
    }

    private IEnumerator UseGiftCodeCoroutine(string code)
    {
        var msg = NetManager.Instance.UseGiftCode(code);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State != MessageState.Reply)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200005000));
            yield break;
        }
        if (msg.ErrorCode != 0)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(271013));
        }
    }

    private void TotalCompensateItem(int type)
    {
        var newIitem = new ActivityCompensateItemDataModel();
        newIitem.NeedGood = 0;
        newIitem.NeedDia = 0;
        var resDic = new Dictionary<int, int>();
        float prop = 1;

        for (var i = 0; i < RewardData.Compensate.ItemList.Count; i++)
        {
            var item = RewardData.Compensate.ItemList[i];
            newIitem.NeedDia += item.NeedDia;
            newIitem.NeedGood += item.NeedGood;
            for (var j = 0; j < item.GetList.Count; j++)
            {
                var tt = item.GetList[j];
                if (resDic.ContainsKey(tt.ItemId))
                {
                    resDic[tt.ItemId] += tt.Count;
                }
                else
                {
                    resDic.Add(tt.ItemId, tt.Count);
                }
            }
        }
        if (type == 2)
        {
            prop = (float) ResParm/10000;
        }
        else if (type == 3)
        {
            prop = 1;
        }
        var list = new List<ItemIconDataModel>();
        var __enumerator2 = (resDic).GetEnumerator();
        while (__enumerator2.MoveNext())
        {
            var item = __enumerator2.Current;
            {
                var ii = new ItemIconDataModel();
                ii.ItemId = item.Key;
                ii.Count = (int) Math.Ceiling(item.Value*prop);
                list.Add(ii);
            }
        }
        newIitem.GetList = new ObservableCollection<ItemIconDataModel>(list);
        RewardData.Compensate.SelectedItem = newIitem;
    }


    public ActivityCompensateItemDataModel CopyActivityCompensateItem(ActivityCompensateItemDataModel item, float prop)
    {
        var newIitem = new ActivityCompensateItemDataModel();
        newIitem.Id = item.Id;
        newIitem.LeftCount = item.LeftCount;
        newIitem.Name = item.Name;
        newIitem.NeedDia = item.NeedDia;
        newIitem.NeedGood = item.NeedGood;
        for (var i = 0; i < item.GetList.Count; i++)
        {
            var ii = new ItemIconDataModel();
            ii.ItemId = item.GetList[i].ItemId;
            ii.Count = (int) Math.Ceiling(item.GetList[i].Count*prop);
            newIitem.GetList.Add(ii);
        }
        return newIitem;
    }


    private void CompensateOperation(int index)
    {
        switch (index)
        {
            case 0: //一键金币
                if (RewardData.Compensate.ItemList.Count == 0)
                {
                    return;
                }
                TotalCompensateItem(2);
                RewardData.Compensate.ShowWitchConfirm = 1;
                CompensateOkType = 0;
                break;
            case 1: //一键钻石
                if (RewardData.Compensate.ItemList.Count == 0)
                {
                    return;
                }
                TotalCompensateItem(3);
                RewardData.Compensate.ShowWitchConfirm = 2;
                CompensateOkType = 1;
                break;
            case 2: //确定补偿
                var indexType = 0;
                if (CompensateOkType == 0)
                {
                    indexType = -1;
                    GoldOrDia = 0;
                }
                else if (CompensateOkType == 1)
                {
                    indexType = -1;
                    GoldOrDia = 1;
                }
                else if (CompensateOkType == 2)
                {
                    indexType = RewardData.Compensate.SelectedItem.Id;
                }
                ReceiveCompensation(index, indexType, GoldOrDia);

                break;
            case 3: //取消
                RewardData.Compensate.ShowWitchConfirm = 0;
                break;
            default:
                return;
        }
    }

    private void ReceiveCompensation(int index, int indexType, int GoldOrMoney)
    {
        if (GoldOrMoney == 0)
        {
            if (RewardData.Compensate.SelectedItem.NeedGood >
                PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes))
            {
                var e = new ShowUIHintBoard(210100);
                EventDispatcher.Instance.DispatchEvent(e);
                PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.GoldRes);
                return;
            }
        }
        else if (GoldOrMoney == 1)
        {
            if (RewardData.Compensate.SelectedItem.NeedDia >
                PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes))
            {
                var e = new ShowUIHintBoard(210102);
                EventDispatcher.Instance.DispatchEvent(e);
                PlayerDataManager.Instance.ShowItemInfoGet((int) eResourcesType.DiamondRes);
                return;
            }
        }
        NetManager.Instance.StartCoroutine(ReceiveCompensationCoroutine(index, indexType, GoldOrMoney));
    }

    public IEnumerator ReceiveCompensationCoroutine(int index, int indexType, int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ReceiveCompensation(indexType, type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var Compensate = RewardData.Compensate.ItemList;
                    RewardData.Compensate.ShowWitchConfirm = 0;
                    if (CompensateOkType == 0)
                    {
                        PlayerDataManager.Instance.NoticeData.ActivityCompensateActive = 0;
                        RewardData.Compensate.IsEmpty = 1;
                        Compensate.Clear();
                    }
                    else if (CompensateOkType == 1)
                    {
                        PlayerDataManager.Instance.NoticeData.ActivityCompensateActive = 0;
                        RewardData.Compensate.IsEmpty = 1;
                        Compensate.Clear();
                    }
                    else if (CompensateOkType == 2)
                    {
                        Compensate.RemoveAt(CompensateSelectIndex);
                        PlayerDataManager.Instance.NoticeData.ActivityCompensateActive = Compensate.Count;
                        if (Compensate.Count == 0)
                        {
                            RewardData.Compensate.IsEmpty = 1;
                        }
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    #endregion

    #region 小红点

    public void AnalyseNoticeByFlag(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        AnalyseNoticeByFlag(e.Index);
    }

    public void AnalyseNoticeByExdata(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        AnalyseNoticeByExdata(e.Key);
    }

//     public void AnalyseNoticeByFlag(int index)
//     {
//         AnalyseNoticeByFlag(index);
//     }
// 
//     public void AnalyseNoticeByExdata(int index)
//     {
//         AnalyseNoticeByExdata(index);
//     }

    #endregion

    #region 事件

    //当标记位变化时
    public void OnFlagUpdateEvent(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        var idx = e.Index;

        if (CONTINUES_LOGIN_FLAG_IDX == idx)
        {
            UpdateContinuesLoginReward();
            return;
        }
        if (TODAY_CHECKIN_FLAG_IDX == idx)
        {
            UpdateMonthCheckinReward();
            return;
        }

        List<GiftRecord> list = null;
        if (!mFlagDataDict.TryGetValue(idx, out list))
        {
            return;
        }
        {
            var __list8 = list;
            var __listCount8 = __list8.Count;
            for (var __i8 = 0; __i8 < __listCount8; ++__i8)
            {
                var table = __list8[__i8];
                {
                    var type = (eRewardType) table.Type;
                    if (eRewardType.OnlineReward == type)
                    {
                        UpdateOnLineReward();
                    }
                    else if (eRewardType.LevelReward == type)
                    {
                        UpdateLevelReward();
                    }
                    else if (eRewardType.ContinuesLoginReward == type)
                    {
                        UpdateContinuesLoginReward();
                    }
                    else if (eRewardType.DailyActivityReward == type)
                    {
                        UpdateActivityReward();
                    }
                }
            }
        }
    }


    //当扩展数据变化时
    public void OnExtDataUpDataEvent(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        var idx = e.Key;
        if (TODAY_ONLINE_TIME_DATA_IDX == idx)
        {
            UpdateOnLineReward();
        }
        else if (CONTINUES_LOGIN_DATA_IDX == idx)
        {
            UpdateContinuesLoginReward();
        }
        else if (MONTH_CHECKIN_DAYS_DATA_IDX == idx)
        {
            UpdateMonthCheckinReward();
        }
        else if (ACTIVITY_SCORE_DATA_IDX == idx)
        {
            UpdateActivityReward();
        }

        //List<GiftRecord> list = null;
        //if (!mExtDataDict.TryGetValue(idx, out list))
        //    return;
        //{
        //    var __list9 = list;
        //    var __listCount9 = __list9.Count;
        //    for (int __i9 = 0; __i9 < __listCount9; ++__i9)
        //    {
        //        var table = __list9[__i9];
        //        {
        //            var type = (eRewardType)table.Type;
        //            if (eRewardType.DailyActivity == type)
        //            {
        //                UpdateActivity();
        //            }
        //        }
        //    }
        //}
    }

    private void OnCliamReward(IEvent ievent)
    {
        var e = ievent as UIEvent_CliamReward;
        if (null == e)
        {
            return;
        }

        var type = e.RewardType;
        var idx = e.Idx;

        if (type == UIEvent_CliamReward.Type.OnLine)
        {
            ClaimOnLineReward(idx);
        }
        else if (type == UIEvent_CliamReward.Type.Level)
        {
            CliamLevelReward(idx);
        }
        else if (type == UIEvent_CliamReward.Type.CheckinToday)
        {
            CheckinToday();
        }
        else if (type == UIEvent_CliamReward.Type.ReCheckinToday)
        {
            ReCheckinToday();
        }
        else if (type == UIEvent_CliamReward.Type.ClaimContinuesLoginReward)
        {
            ClaimContinuesLoginReward();
        }
        else if (type == UIEvent_CliamReward.Type.Activity)
        {
            CliamActivityReward(idx);
        }
        else if (type == UIEvent_CliamReward.Type.Compensate)
        {
            CompensateOperation(idx);
        }
    }

    #endregion

    #region Reward State

    public const int CONTINUES_LOGIN_FLAG_IDX = 313;
    public const int CONTINUES_LOGIN_DATA_IDX = 17;
    public const int CONTINUES_RECHECKIN_DATA_IDX = 18;

    public const int TODAY_CHECKIN_FLAG_IDX = 466;
    public const int MONTH_CHECKIN_DAYS_DATA_IDX = 16;
    public const int TODAY_ONLINE_TIME_DATA_IDX = 31;

    public const int ACTIVITY_SCORE_DATA_IDX = 15;


    public BitFlag FlagData
    {
        get { return PlayerDataManager.Instance.FlagData; }
    }


    public List<int> ExtData
    {
        get { return PlayerDataManager.Instance.ExtData; }
    }

    public int GetExtData(int idx)
    {
        if (idx >= 0 && idx < ExtData.Count)
        {
            return ExtData[idx];
        }
        return 0;
    }

    public int GetOnLineSeconds()
    {
        return GetExtData(TODAY_ONLINE_TIME_DATA_IDX) + Game.Instance.OnLineSeconds;
    }

    private void OnGetOnLineSeconds(IEvent ievent)
    {
        var e = ievent as UIEvent_GetOnLineSeconds;
        if (null != e)
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateOnLineSeconds(GetOnLineSeconds()));
        }
    }

    public eRewardState GetOnLineRewardState(int id)
    {
        var table = Table.GetGift(id);
        if (0 != FlagData.GetFlag(table.Flag))
        {
            return eRewardState.HasGot;
        }

        var seconds = table.Param[OnLineRewardParamterIndx.Minutes];
        var todayOnlineTime = GetOnLineSeconds();

        if (todayOnlineTime >= seconds)
        {
            return eRewardState.CanGet;
        }
        return eRewardState.CannotGet;
    }

    public eRewardState GetLevelRewardState(int id)
    {
        var table = Table.GetGift(id);
        if (0 != FlagData.GetFlag(table.Flag))
        {
            return eRewardState.HasGot;
        }
        var level = PlayerDataManager.Instance.GetLevel();
        var needLevel = table.Param[LevelRewardParamterIndx.Level];
        if (level >= needLevel)
        {
            return eRewardState.CanGet;
        }
        return eRewardState.CannotGet;
    }

    public bool HasGotTodayLoginReward()
    {
        return 0 != FlagData.GetFlag(CONTINUES_LOGIN_FLAG_IDX);
    }

    public int GetContinuesLoginDayNumber()
    {
        return GetExtData(CONTINUES_LOGIN_DATA_IDX);
    }

    public int GetMonthCheckinTimes()
    {
        return GetExtData(MONTH_CHECKIN_DAYS_DATA_IDX);
    }

    public int GetMonthReCheckinTimes()
    {
        return GetExtData(CONTINUES_RECHECKIN_DATA_IDX);
    }

    public bool HasTodayCheckined()
    {
        return 0 != FlagData.GetFlag(TODAY_CHECKIN_FLAG_IDX);
    }

    public eRewardState GetMonthCheckinRewardState(int id)
    {
        var table = Table.GetGift(id);
        if (0 != FlagData.GetFlag(table.Flag))
        {
            return eRewardState.HasGot;
        }
        var now = Game.Instance.ServerTime;
        var day = table.Param[MonthCheckinRewardParamterIndx.Day];
        if (now.Day > day)
        {
            return eRewardState.CannotGet;
        }

        return eRewardState.CanGet;
    }

    public int GetActivityProgress(int id)
    {
        var table = Table.GetGift(id);
        var idx = table.Exdata;
        if (idx >= 0 && idx < ExtData.Count)
        {
            return GetExtData(idx);
        }
        return 0;
    }

    public int GetActivityScore()
    {
        return GetExtData(ACTIVITY_SCORE_DATA_IDX);
    }

    public eRewardState GetActivityRewardState(int id)
    {
        var table = Table.GetGift(id);
        if (0 != FlagData.GetFlag(table.Flag))
        {
            return eRewardState.HasGot;
        }

        var score = GetActivityScore();
        var needScore = table.Param[ActivityRewardParamterIndx.NeedScore];
        if (score > 0 && score >= needScore)
        {
            return eRewardState.CanGet;
        }
        return eRewardState.CannotGet;
    }

    public void SendClaimRewardRequest(int type, int id)
    {
        GameLogic.Instance.StartCoroutine(ClaimRewardCoroutine(type, id));
    }

    private IEnumerator ClaimRewardCoroutine(int type, int id)
    {
        using (new BlockingLayerHelper(0))
        {
            Logger.Debug(".............ClaimRewardCoroutine..................begin");
            var msg = NetManager.Instance.ActivationReward(type, id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State != MessageState.Reply)
            {
                Logger.Debug("[ClaimRewardCoroutine] msg.State != MessageState.Reply");
                yield break;
            }

            if (msg.ErrorCode != (int) ErrorCodes.OK)
            {
                Logger.Debug("[ClaimRewardCoroutine] ErrorCodes=[{0}]", msg.ErrorCode);
                if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(302));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000006));
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_ErrorTip((ErrorCodes) msg.ErrorCode));
                }
                yield break;
            }
        }
        //AnalyseNotice();

        //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(452));

        Logger.Debug(".............ClaimRewardCoroutine..................end");
    }

    public void ClaimOnLineReward(int id)
    {
        var state = GetOnLineRewardState(id);
        if (eRewardState.CanGet != state)
        {
            return;
        }

        SendClaimRewardRequest((int) eActivationRewardType.TableGift, id);
    }

    public void CliamLevelReward(int id)
    {
        var state = GetLevelRewardState(id);
        if (eRewardState.CanGet != state)
        {
            return;
        }

        SendClaimRewardRequest((int) eActivationRewardType.TableGift, id);
    }

    public void ClaimContinuesLoginReward()
    {
        if (HasGotTodayLoginReward())
        {
            Logger.Debug("HasGotTodayLoginReward");
            return;
        }

        var days = GetContinuesLoginDayNumber();
        var maxDay = 0;
        GiftRecord temp = null;
        Table.ForeachGift(table =>
        {
            if ((eRewardType) table.Type == eRewardType.ContinuesLoginReward)
            {
                var needDays = table.Param[ContinuesLoginRewardParamterIndx.Days];
                if (days >= needDays)
                {
                    if (needDays > maxDay)
                    {
                        maxDay = needDays;
                        temp = table;
                    }
                    return false;
                }
            }
            return true;
        });

        if (null != temp)
        {
            SendClaimRewardRequest((int) eActivationRewardType.TableGift, temp.Id);
        }
    }

    public void CheckinToday()
    {
        if (HasTodayCheckined())
        {
            return;
        }
        var days = GetMonthCheckinTimes();
        var checkinDay = days + 1;
        Table.ForeachGift(table =>
        {
            if ((eRewardType) table.Type == eRewardType.MonthCheckinReward)
            {
                if (checkinDay != table.Param[MonthCheckinRewardParamterIndx.Day])
                {
                    return true;
                }
                var month = table.Param[MonthCheckinRewardParamterIndx.Month];
                if (999 != month && month != Game.Instance.ServerTime.Month)
                {
                    return true;
                }
                SendClaimRewardRequest((int) eActivationRewardType.TableGift, table.Id);
                return false;
            }
            return true;
        });
    }

    public void ReCheckinToday()
    {
        if (!HasTodayCheckined())
        {
            return;
        }

        var days = GetMonthCheckinTimes();
        var checkinDay = days + 1;
        Table.ForeachGift(table =>
        {
            if ((eRewardType) table.Type == eRewardType.MonthCheckinReward)
            {
                if (checkinDay != table.Param[MonthCheckinRewardParamterIndx.Day])
                {
                    return true;
                }
                var month = table.Param[MonthCheckinRewardParamterIndx.Month];
                if (999 != month && month != Game.Instance.ServerTime.Month)
                {
                    return true;
                }
                //var diamond = table.Param[4] + GetMonthReCheckinTimes() * table.Param[5];
                //int diamond = table.Param[4] * SkillExtension.Pow(table.Param[5], GetMonthReCheckinTimes());
                var diamond =
                    (int) (table.Param[4]*SkillExtension.Pow(table.Param[5]/10000.0f, GetMonthReCheckinTimes()));
                diamond = diamond - diamond%5;
                var str = string.Format(GameUtils.GetDictionaryText(530), diamond);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    str,
                    GameUtils.GetDictionaryText(1503),
                    () => { SendClaimRewardRequest((int) eActivationRewardType.TableGift, table.Id); });

                return false;
            }
            return true;
        });
    }

    public void CliamActivityReward(int id)
    {
        var state = GetActivityRewardState(id);
        if (eRewardState.CanGet != state)
        {
            return;
        }

        SendClaimRewardRequest((int) eActivationRewardType.TableGift, id);
    }

    private static readonly Dictionary<int, List<eRewardType>> ExdataGift = new Dictionary<int, List<eRewardType>>();
    private static readonly Dictionary<int, List<eRewardType>> FlagGift = new Dictionary<int, List<eRewardType>>();

    private static readonly Dictionary<eRewardType, List<GiftRecord>> GiftList =
        new Dictionary<eRewardType, List<GiftRecord>>();

    public static void InitTableAfter_AnalyseNotice()
    {
        GiftList.Clear();
        Table.ForeachGift(recoard =>
        {
            var type = (eRewardType) recoard.Type;
            if (eRewardType.OnlineReward == type)
            {
                List<GiftRecord> list;
                if (!GiftList.TryGetValue(type, out list))
                {
                    list = new List<GiftRecord>();
                    GiftList[type] = list;
                }
                list.Add(recoard);
            }
            else if (eRewardType.LevelReward == type)
            {
                List<GiftRecord> list;
                if (!GiftList.TryGetValue(type, out list))
                {
                    list = new List<GiftRecord>();
                    GiftList[type] = list;
                }
                list.Add(recoard);
            }
            else if (eRewardType.ContinuesLoginReward == type)
            {
                List<GiftRecord> list;
                if (!GiftList.TryGetValue(type, out list))
                {
                    list = new List<GiftRecord>();
                    GiftList[type] = list;
                }
                list.Add(recoard);
            }
            else if (eRewardType.MonthCheckinReward == type)
            {
                List<GiftRecord> list;
                if (!GiftList.TryGetValue(type, out list))
                {
                    list = new List<GiftRecord>();
                    GiftList[type] = list;
                }
                list.Add(recoard);
            }
            else if (eRewardType.DailyActivityReward == type)
            {
                List<GiftRecord> list;
                if (!GiftList.TryGetValue(type, out list))
                {
                    list = new List<GiftRecord>();
                    GiftList[type] = list;
                }
                list.Add(recoard);
            }
            if (recoard.Exdata != -1)
            {
                List<eRewardType> list;
                if (ExdataGift.TryGetValue(recoard.Exdata, out list))
                {
                    if (!list.Contains(type))
                    {
                        list.Add(type);
                    }
                }
                else
                {
                    list = new List<eRewardType>();
                    ExdataGift[recoard.Exdata] = list;
                    list.Add(type);
                }
            }
            if (recoard.Flag != -1)
            {
                List<eRewardType> list;
                if (FlagGift.TryGetValue(recoard.Flag, out list))
                {
                    if (!list.Contains(type))
                    {
                        list.Add(type);
                    }
                }
                else
                {
                    list = new List<eRewardType>();
                    FlagGift[recoard.Flag] = list;
                    list.Add(type);
                }
            }
            return true;
        });
    }

    public void AnalyseNotice()
    {
        if (ExtData.Count == 0)
        {
            return;
        }
        int[] count = {0, 0, 0, 0, 0, 0, 0, 0};

        Table.ForeachGift(recoard =>
        {
            var type = (eRewardType) recoard.Type;

            if (eRewardType.OnlineReward == type)
            {
                if (GetOnLineRewardState(recoard.Id) == eRewardState.CanGet)
                {
                    count[recoard.Type]++;
                }
            }
            else if (eRewardType.LevelReward == type)
            {
                if (GetLevelRewardState(recoard.Id) == eRewardState.CanGet)
                {
                    count[recoard.Type]++;
                }
            }
            else if (eRewardType.ContinuesLoginReward == type)
            {
                if (!HasGotTodayLoginReward())
                {
                    count[recoard.Type] = 1;
                }
            }
            else if (eRewardType.MonthCheckinReward == type)
            {
                if (!HasTodayCheckined() && GetMonthCheckinTimes() < 25)
                {
                    count[recoard.Type] = 1;
                }
            }
            else if (eRewardType.DailyActivityReward == type)
            {
                if (GetActivityRewardState(recoard.Id) == eRewardState.CanGet)
                {
                    count[recoard.Type]++;
                }
            }

            return true;
        });

        var noticeData = PlayerDataManager.Instance.NoticeData;
        noticeData.ActivityLevel = count[(int) eRewardType.LevelReward];
        noticeData.ActivityLoginSeries = count[(int) eRewardType.ContinuesLoginReward];
        noticeData.ActivityTimeLength = count[(int) eRewardType.OnlineReward];
        noticeData.ActivityLoginAddup = count[(int) eRewardType.MonthCheckinReward];
        noticeData.ActivityDailyActive = count[(int) eRewardType.DailyActivityReward];
    }

    public void AnalyseNotice_RefreshType(eRewardType type, List<GiftRecord> recordList)
    {
        if (ExtData.Count == 0)
        {
            return;
        }
        var count = 0;
        {
            var __list1 = recordList;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var recoard = __list1[__i1];
                {
                    var TbType = (eRewardType) recoard.Type;
                    if (type != TbType)
                    {
                        continue;
                    }
                    if (eRewardType.OnlineReward == type)
                    {
                        if (GetOnLineRewardState(recoard.Id) == eRewardState.CanGet)
                        {
                            count++;
                        }
                    }
                    else if (eRewardType.LevelReward == type)
                    {
                        if (GetLevelRewardState(recoard.Id) == eRewardState.CanGet)
                        {
                            count++;
                        }
                    }
                    else if (eRewardType.ContinuesLoginReward == type)
                    {
                        if (!HasGotTodayLoginReward())
                        {
                            count = 1;
                        }
                    }
                    else if (eRewardType.MonthCheckinReward == type)
                    {
                        if (!HasTodayCheckined() && GetMonthCheckinTimes() < 25) //今日没签到并且还没签满25次
                        {
                            count = 1;
                        }
                    }
                    else if (eRewardType.DailyActivityReward == type)
                    {
                        if (GetActivityRewardState(recoard.Id) == eRewardState.CanGet)
                        {
                            count++;
                        }
                    }
                }
            }
        }
        var noticeData = PlayerDataManager.Instance.NoticeData;
        var playerData = PlayerDataManager.Instance;
        switch (type)
        {
            case eRewardType.Invalid:
                break;
            case eRewardType.GiftBag:
                break;
            case eRewardType.OnlineReward:
                noticeData.ActivityTimeLength = count;
                break;
            case eRewardType.LevelReward:
                noticeData.ActivityLevel = count;
                break;
            case eRewardType.ContinuesLoginReward:
                noticeData.ActivityLoginSeries = count;
                break;
            case eRewardType.MonthCheckinReward:
                noticeData.ActivityLoginAddup = count;
                break;
            case eRewardType.DailyActivity:
                break;
            case eRewardType.DailyActivityReward:
                noticeData.ActivityDailyActive = count;
                break;
            default:
                Logger.Error("AnalyseNotice_RefreshType {0}", type);
                break;
        }
    }

    public void AnalyseNoticeByFlag(int index)
    {
        List<eRewardType> typeList;
        if (!FlagGift.TryGetValue(index, out typeList))
        {
            return;
        }
        {
            var __list2 = typeList;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var type = __list2[__i2];
                {
                    List<GiftRecord> recordList;
                    if (!GiftList.TryGetValue(type, out recordList))
                    {
                        return;
                    }
                    AnalyseNotice_RefreshType(type, recordList);
                }
            }
        }
    }

    public void AnalyseNoticeByExdata(int index)
    {
        List<eRewardType> typeList;
        if (!ExdataGift.TryGetValue(index, out typeList))
        {
            return;
        }
        {
            var __list3 = typeList;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var type = __list3[__i3];
                {
                    List<GiftRecord> recordList;
                    if (!GiftList.TryGetValue(type, out recordList))
                    {
                        return;
                    }
                    AnalyseNotice_RefreshType(type, recordList);
                }
            }
        }
    }

    #endregion
}