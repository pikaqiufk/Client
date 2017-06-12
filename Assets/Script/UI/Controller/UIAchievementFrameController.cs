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

public class UIAchievementFrameController : IControllerBase
{
    //总成就积分扩展数据索引id
    public const int TOTAL_SCORE_DATA_IDX = 50;

    public UIAchievementFrameController()
    {
        CleanUp();


        EventDispatcher.Instance.AddEventListener(Enter_Scene_Event.EVENT_TYPE, evn => { Init(); });

        EventDispatcher.Instance.AddEventListener(Event_ShowAchievementPage.EVENT_TYPE, OnShowAchievementPageEvent);


        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpdateEvent);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpDataEvent);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUpEvent);

        EventDispatcher.Instance.AddEventListener(UI_EventApplyChengJiuItem.EVENT_TYPE, ClaimReward);
    }

    //缓存按类型划分成就
    private readonly Dictionary<int, List<AchievementRecord>> mAnalyzeTable =
        new Dictionary<int, List<AchievementRecord>>();

    private int mCurrentPage = -1;
    //缓存扩展计数影响哪些成就
    private readonly Dictionary<int, List<AchievementRecord>> mExtDataDic =
        new Dictionary<int, List<AchievementRecord>>();

    //缓存标记位影响哪些成就
    private readonly Dictionary<int, List<AchievementRecord>> mFlagDataDic =
        new Dictionary<int, List<AchievementRecord>>();

    private bool mInit;
    //缓存等级影响哪些成就
    private readonly Dictionary<int, List<AchievementRecord>> mLevelDic = new Dictionary<int, List<AchievementRecord>>();
    public FrameState mState;
    private int mTotalAchievement;
    public AchievementDataModel DataModel { get; set; }
    //扩展数据
    public List<int> ExtData
    {
        get { return PlayerDataManager.Instance.ExtData; }
    }

    //标记位
    public BitFlag FlagData
    {
        get { return PlayerDataManager.Instance.FlagData; }
    }

    //填充一个成就数据到数据源
    public void AssignAchievement(AchievementRecord table, AchievementItemDataModel achievement)
    {
        achievement.Id = table.Id;
        achievement.Title = table.Name;
        if (-1 != table.Exdata)
        {
            var progress = Mathf.Min(GetAchievementProgress(achievement.Id), table.ExdataCount);
            achievement.Progress = progress*1.0f/table.ExdataCount;
            achievement.ProgressLabel = string.Format("{0}/{1}", GameUtils.GetBigValueStr(progress),
                GameUtils.GetBigValueStr(table.ExdataCount));
            achievement.ShowPorgress = true;
        }
        else
        {
            achievement.ShowPorgress = false;
        }

        var state = GetAchievementState(achievement.Id);
        achievement.State = (int) state;
        /*
		if (eRewardState.HasGot == state)
		{
			achievement.State = GameUtils.GetDictionaryText(1035);
			achievement.CanGetReward = false;
		}
		else if (eRewardState.CanGet == state)
		{
			achievement.State = GameUtils.GetDictionaryText(1036);
			achievement.CanGetReward = true;
		}
		else if (eRewardState.CannotGet == state)
		{
			achievement.State = GameUtils.GetDictionaryText(1037);
			achievement.CanGetReward = false;
		}
		*/
        var tableItemIdLength0 = table.ItemId.Length;
        for (var i = 0; i < tableItemIdLength0; i++)
        {
            var itemId = table.ItemId[i];
            achievement.Rewards[i].ItemId = itemId;
            achievement.Rewards[i].Count = table.ItemCount[i];
        }
    }

    //
    public void ClaimAchievementReward(int id)
    {
        if (eRewardState.CanGet != GetAchievementState(id))
        {
            return;
        }
        GameLogic.Instance.StartCoroutine(ClaimAchievementRewardCoroutine(id));
    }

    private IEnumerator ClaimAchievementRewardCoroutine(int id)
    {
        using (new BlockingLayerHelper(0))
        {
            Logger.Debug(".............ClaimAchievementRewardCoroutine..................begin");
            var msg = NetManager.Instance.RewardAchievement(id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State != MessageState.Reply)
            {
                Logger.Debug("[ClaimAchievementRewardCoroutine] msg.State != MessageState.Reply");
                yield break;
            }

            if (msg.ErrorCode != (int) ErrorCodes.OK)
            {
                Logger.Debug("[ClaimAchievementRewardCoroutine] ErrorCodes=[{0}]", msg.ErrorCode);

                if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(302));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
                yield break;
            }
            //如果成就领取成功，就把标记位设置一下，防止连续领取
            var table = Table.GetAchievement(id);
            if (null != table)
            {
                FlagData.SetFlag(table.RewardFlagId);
                OnFlagUpdate(table.RewardFlagId);
            }

            const int flagId = 492;
            //成就奖励领取成功清除一个标记
            if (!PlayerDataManager.Instance.GetFlag(flagId))
            {
                var list = new Int32Array();
                list.Items.Add(flagId);
                PlayerDataManager.Instance.SetFlagNet(list);
            }
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(452));

            Logger.Debug(".............ClaimAchievementRewardCoroutine..................end");
        }
    }

    //点击获得成就奖励
    private void ClaimReward(IEvent ievent)
    {
        var e = ievent as UI_EventApplyChengJiuItem;
        if (null != e)
        {
            ClaimAchievementReward(e.Id);
        }
    }

    //获得成就进度
    public int GetAchievementProgress(int id)
    {
        var table = Table.GetAchievement(id);
        if (-1 != table.Exdata)
        {
            return GetExtData(table.Exdata);
        }
        return 0;
    }

    //获得成就状态
    public eRewardState GetAchievementState(int id)
    {
        var table = Table.GetAchievement(id);
        if (0 != FlagData.GetFlag(table.RewardFlagId))
        {
            return eRewardState.HasGot;
        }
        if (0 != FlagData.GetFlag(table.FinishFlagId))
        {
            return eRewardState.CanGet;
        }
        return eRewardState.CannotGet;
    }

    //获得扩展数据值
    public int GetExtData(int idx)
    {
        if (idx >= 0 && idx < ExtData.Count)
        {
            return ExtData[idx];
        }
        return 0;
    }

    //获得总成就积分
    public int GetTotalScore()
    {
        return GetExtData(TOTAL_SCORE_DATA_IDX);
    }

    //初始化
    public void Init()
    {
        if (mInit)
        {
            return;
        }

        DataModel.TotalScore = GetTotalScore();
        Table.ForeachAchievement(table =>
        {
            //-1类型的是大类，不是成就数据
            if (-1 == table.Type)
            {
                return true;
            }
            if (table.Type < 0 || table.Type >= DataModel.Summary.Count)
            {
                Logger.Error("table.Type[{0}] out of range [{1}]", table.Type, table.Name);
                return true;
            }
            var summary = DataModel.Summary[table.Type];

            if (eRewardState.CannotGet != GetAchievementState(table.Id))
            {
                summary.CompletedNum++;
            }
            if (eRewardState.CanGet == GetAchievementState(table.Id))
            {
                //计算小红点个数
                var idx = table.Type + 1;
                if (idx >= 0 && idx < DataModel.Catalog.Count)
                {
                    DataModel.Catalog[idx].Count++;
                }
            }
            return true;
        });
        {
            // foreach(var summary in DataModel.Summary)
            var __enumerator2 = (DataModel.Summary).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var summary = __enumerator2.Current;
                {
                    summary.Progress = 0 == summary.TotalNum ? 0 : summary.CompletedNum*1.0f/summary.TotalNum;
                    summary.ProgressLabel = GameUtils.GetBigValueStr(summary.CompletedNum) + "/" +
                                            GameUtils.GetBigValueStr(summary.TotalNum);
                    var percent = (int) Math.Ceiling(summary.Progress*100);
                    summary.ProgressString = percent + "%";
                }
            }
        }

        RefreshNotice();
        RefeshTotal();
        DataModel.Catalog[0].Checked = true;

        mInit = true;
    }

    //初始化表格数据
    public void InitByTable()
    {
        //先加入总览按钮
        {
            var newCatalog = new AchievementSummaryBtnDataModel();
            newCatalog.Title = GameUtils.GetDictionaryText(300834);
            newCatalog.TypeId = -1;
            DataModel.Catalog.Add(newCatalog);
        }

        //遍历表格分类
        Table.ForeachAchievement(table =>
        {
            //-1类型的是大类，不是成就数据
            if (-1 == table.Type)
            {
                //左侧分类按钮列表
                var newCatalog = new AchievementSummaryBtnDataModel();
                newCatalog.Title = table.Name;
                newCatalog.TypeId = table.Id;
                DataModel.Catalog.Add(newCatalog);

                //成就大类列表
                var newSummary = new AchievementSummaryItemDataModel();
                newSummary.Title = table.Name;
                newSummary.TypeId = table.Id;
                DataModel.Summary.Add(newSummary);

                return true;
            }

            //该成就的分类
            var summary = DataModel.Summary[table.Type];
            summary.TotalNum++;


            {
//分析表格，根据类型缓存表格数据
                List<AchievementRecord> list = null;
                if (!mAnalyzeTable.TryGetValue(table.Type, out list))
                {
                    list = new List<AchievementRecord>();
                    mAnalyzeTable.Add(table.Type, list);
                }
                list.Add(table);
            }


            {
//缓存扩展数据所影响的成就
                if (-1 != table.Exdata)
                {
                    var list = new List<AchievementRecord>();
                    if (!mExtDataDic.TryGetValue(table.Exdata, out list))
                    {
                        list = new List<AchievementRecord>();
                        mExtDataDic.Add(table.Exdata, list);
                    }

                    list.Add(table);
                }
            }

            {
//缓存标记位影响哪些成就
                List<AchievementRecord> list = null;
                var flagId = table.RewardFlagId;
                if (-1 != flagId)
                {
                    if (!mFlagDataDic.TryGetValue(flagId, out list))
                    {
                        list = new List<AchievementRecord>();
                        mFlagDataDic.Add(flagId, list);
                    }
                    list.Add(table);
                }

                flagId = table.FinishFlagId;
                if (-1 != flagId)
                {
                    if (!mFlagDataDic.TryGetValue(flagId, out list))
                    {
                        list = new List<AchievementRecord>();
                        mFlagDataDic.Add(flagId, list);
                    }
                    list.Add(table);
                }

                flagId = table.ClientDisplay;
                if (-1 != flagId)
                {
                    if (!mFlagDataDic.TryGetValue(flagId, out list))
                    {
                        list = new List<AchievementRecord>();
                        mFlagDataDic.Add(flagId, list);
                    }
                    list.Add(table);
                }
            }

            {
//缓存等级影响哪些成就
                List<AchievementRecord> list = null;
                var level = table.ViewLevel;
                if (-1 != level)
                {
                    if (!mLevelDic.TryGetValue(level, out list))
                    {
                        list = new List<AchievementRecord>();
                        mLevelDic.Add(level, list);
                    }
                    list.Add(table);
                }
            }

            mTotalAchievement++;

            return true;
        });
    }

    //该成就是否完成了
    public bool IsAchievementAccomplished(int id)
    {
        return GetAchievementState(id) != eRewardState.CannotGet;
    }

    //当扩展数据更新时
    private void OnExDataUpDataEvent(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        var idx = e.Key;

        if (idx == TOTAL_SCORE_DATA_IDX)
        {
//是成就总积分改变了
            DataModel.TotalScore = GetTotalScore();
            return;
        }

        List<AchievementRecord> list = null;
        if (!mExtDataDic.TryGetValue(idx, out list))
        {
            return;
        }
        {
            var __list5 = list;
            var __listCount5 = __list5.Count;
            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
            {
                var table = __list5[__i5];
                {
                    {
                        // foreach(var achievement in DataModel.CurrentAchievementItemList)
                        var __enumerator14 = (DataModel.CurrentAchievementItemList).GetEnumerator();
                        while (__enumerator14.MoveNext())
                        {
                            var achievement = __enumerator14.Current;
                            {
                                if (achievement.Id == table.Id)
                                {
                                    if (-1 != table.Exdata && idx == table.Exdata)
                                    {
                                        var progress = Mathf.Min(GetAchievementProgress(achievement.Id),
                                            table.ExdataCount);
                                        achievement.Progress = progress*1.0f/table.ExdataCount;
                                        achievement.ProgressLabel = string.Format("{0}/{1}",
                                            GameUtils.GetBigValueStr(progress),
                                            GameUtils.GetBigValueStr(table.ExdataCount));
                                        achievement.ShowPorgress = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //当标记位改变时
    public void OnFlagUpdate(int idx)
    {
        List<AchievementRecord> list = null;
        if (!mFlagDataDic.TryGetValue(idx, out list))
        {
            return;
        }

        //我的等级
        var MyLevel = PlayerDataManager.Instance.GetLevel();

        var updateTypeList = new List<int>();
        {
            var __list3 = list;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var table = __list3[__i3];
                {
                    if (table.Type < 0 || table.Type >= DataModel.Summary.Count)
                    {
                        continue;
                    }

                    var state = GetAchievementState(table.Id);

                    //这个成就的可见标记位改变了，并且当前显示的就这个成就列表
                    if (-1 != table.ClientDisplay && table.ClientDisplay == idx && mCurrentPage == table.Type)
                    {
                        var needAdd = false;
                        if (eRewardState.CannotGet != state)
                        {
//完成就直接显示
                            needAdd = true;
                        }
                        else if (0 != FlagData.GetFlag(table.ClientDisplay))
                        {
                            if (MyLevel >= table.ViewLevel)
                            {
//达到了显示条件才显示
                                needAdd = true;
                            }
                        }

                        //这个成就变可见了
                        if (needAdd)
                        {
                            var find = false;
                            {
                                // foreach(var data in DataModel.CurrentAchievementItemList)
                                var __enumerator12 = (DataModel.CurrentAchievementItemList).GetEnumerator();
                                while (__enumerator12.MoveNext())
                                {
                                    var data = __enumerator12.Current;
                                    {
                                        if (data.Id == table.Id)
                                        {
                                            find = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!find)
                            {
//没找到就要加入到当前列表
                                var achievement = new AchievementItemDataModel();
                                DataModel.CurrentAchievementItemList.Add(achievement);
                                AssignAchievement(table, achievement);
                            }
                        }
                    }

                    if (state == eRewardState.CannotGet)
                    {
                        continue;
                    }

                    //这个类型需要重新计算
                    if (!updateTypeList.Contains(table.Type))
                    {
                        updateTypeList.Add(table.Type);
                    }
                    {
                        // foreach(var achievement in DataModel.CurrentAchievementItemList)
                        var __enumerator13 = (DataModel.CurrentAchievementItemList).GetEnumerator();
                        while (__enumerator13.MoveNext())
                        {
                            var achievement = __enumerator13.Current;
                            {
                                if (achievement.Id == table.Id)
                                {
                                    achievement.State = (int) state;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        //刷新改变的类型
        if (updateTypeList.Count > 0)
        {
            var tempList = DataModel.CurrentAchievementItemList;
            var changed = false;
            for (var i = 1; i < tempList.Count; i++)
            {
                var temp = tempList[i];
                if (eRewardState.CanGet == (eRewardState) temp.State)
                {
                    tempList.RemoveAt(i);
                    tempList.Insert(0, temp);
                    changed = true;
                }
            }
            if (changed)
            {
                DataModel.CurrentAchievementItemList = tempList;
            }

            {
                var __list4 = updateTypeList;
                var __listCount4 = __list4.Count;
                for (var __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var type = __list4[__i4];
                    {
                        RefreshByType(type);
                    }
                }
            }
            RefreshNotice();
        }


        RefeshTotal();
    }

    //标记位更新
    private void OnFlagUpdateEvent(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        var idx = e.Index;
        OnFlagUpdate(idx);
    }

    //等级提升时
    private void OnLevelUpEvent(IEvent ievent)
    {
        var e = ievent as Event_LevelUp;
        var MyLevel = PlayerDataManager.Instance.GetLevel();

        List<AchievementRecord> list = null;
        if (!mLevelDic.TryGetValue(MyLevel, out list))
        {
            return;
        }
        {
            var __list6 = list;
            var __listCount6 = __list6.Count;
            for (var __i6 = 0; __i6 < __listCount6; ++__i6)
            {
                var table = __list6[__i6];
                {
                    if (table.Type != mCurrentPage)
                    {
                        continue;
                    }

                    var achievement = new AchievementItemDataModel();
                    DataModel.CurrentAchievementItemList.Add(achievement);
                    AssignAchievement(table, achievement);
                }
            }
        }
    }

    //显示成就子页
    private void OnShowAchievementPageEvent(IEvent ievent)
    {
        {
            // foreach(var item in DataModel.Catalog)
            var __enumerator7 = (DataModel.Catalog).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var item = __enumerator7.Current;
                {
                    item.Checked = false;
                }
            }
        }

        var evn = ievent as Event_ShowAchievementPage;
        if (-1 == evn.Id)
        {
            DataModel.SubPage = false;
            DataModel.Catalog[0].Checked = true;
        }
        else
        {
            DataModel.SubPage = true;
            ShowAchievementPage(evn.Id, evn.Percent);
        }
    }

    //更新总阶分
    private void RefeshTotal()
    {
        var count = 0;
        {
            // foreach(var item in DataModel.Summary)
            var __enumerator9 = (DataModel.Summary).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var item = __enumerator9.Current;
                {
                    count += item.CompletedNum;
                }
            }
        }

        DataModel.Percent = count*1.0f/mTotalAchievement;
    }

    //刷类型
    public void RefreshByType(int type)
    {
        List<AchievementRecord> list = null;
        if (!mAnalyzeTable.TryGetValue(type, out list))
        {
            return;
        }

        if (type < 0 || type >= DataModel.Summary.Count)
        {
            return;
        }

        var summary = DataModel.Summary[type];

        var completedNum = 0;
        var canGetNum = 0;
        {
            var __list11 = mAnalyzeTable[type];
            var __listCount11 = __list11.Count;
            for (var __i11 = 0; __i11 < __listCount11; ++__i11)
            {
                var table = __list11[__i11];
                {
                    var state = GetAchievementState(table.Id);
                    if (eRewardState.CannotGet == state)
                    {
                    }
                    else
                    {
                        completedNum++;
                        if (eRewardState.CanGet == state)
                        {
                            canGetNum++;
                        }
                    }
                }
            }
        }
        //小红点
        DataModel.Catalog[type + 1].Count = canGetNum;

        //成就总览
        summary.CompletedNum = completedNum;
        summary.Progress = 0 == list.Count ? 0 : summary.CompletedNum*1.0f/list.Count;
        //summary.ProgressLabel = GameUtils.GetBigValueStr(summary.CompletedNum) + "/" + GameUtils.GetBigValueStr(list.Count);
        summary.ProgressLabel = summary.CompletedNum + "/" + list.Count;
        var percent = (int) Math.Ceiling(summary.Progress*100);
        summary.ProgressString = percent + "%";
    }

    private void RefreshNotice()
    {
        var has = false;

        //主界面上的
        //PlayerDataManager.Instance.NoticeData.HasAchievement = false;
        {
            // foreach(var catlog in DataModel.Catalog)
            var __enumerator10 = (DataModel.Catalog).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var catlog = __enumerator10.Current;
                {
                    if (catlog.Count > 0)
                    {
                        has = true;
                        break;
                    }
                }
            }
        }

        PlayerDataManager.Instance.NoticeData.HasAchievement = has;
    }

    //显示某个成就子页
    public void ShowAchievementPage(int id, float percent = 0.0f)
    {
        if (id != mCurrentPage)
        {
            mCurrentPage = id;

            UpdateAchievement();
        }

        DataModel.Catalog[mCurrentPage + 1].Checked = true;
    }

    //刷新当前页
    private void UpdateAchievement()
    {
        var MyLevel = PlayerDataManager.Instance.GetLevel();

        DataModel.CurrentAchievementItemList.Clear();

        var tempList = new List<AchievementItemDataModel>();

        List<AchievementRecord> list = null;
        if (!mAnalyzeTable.TryGetValue(mCurrentPage, out list))
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
                    //并且也没完成
                    if (!IsAchievementAccomplished(table.Id))
                    {
                        //没达到可视等级隐藏
                        if (table.ViewLevel > 0 && MyLevel < table.ViewLevel)
                        {
                            continue;
                        }

                        //扩展数据可见性判断
                        if (-1 != table.ClientDisplay)
                        {
                            if (0 == FlagData.GetFlag(table.ClientDisplay))
                            {
                                continue;
                            }
                        }
                    }

                    var achievement = new AchievementItemDataModel();
                    AssignAchievement(table, achievement);
                    var state = (eRewardState) achievement.State;
                    if (eRewardState.CanGet == state)
                    {
                        tempList.Insert(0, achievement);
                    }
                    else
                    {
                        tempList.Add(achievement);
                    }
                }
            }
        }

        DataModel.CurrentAchievementItemList = new ObservableCollection<AchievementItemDataModel>(tempList);
    }

    public void Close()
    {
        DataModel.SubPage = false;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        mCurrentPage = -1;
        {
            // foreach(var item in DataModel.Catalog)
            var __enumerator1 = (DataModel.Catalog).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var item = __enumerator1.Current;
                {
                    item.Checked = false;
                }
            }
        }
        DataModel.Catalog[0].Checked = true;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
        DataModel = new AchievementDataModel();
        InitByTable();
        mInit = false;
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
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}