#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientDataModel.Annotations;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;
using System.Text;

#endregion

public class ActivityController : IControllerBase
{
    #region 数据

    private int mNextFubenIdx = -1;
    private int mainUIActivityClickFuBenId = 0;

    //用于和界面进行绑定的数据
    public ActivityDataModel DataModule;

    //State
    public FrameState State { get; set; }

    //显示boss或者怪物的形象
    public ObjFakeCharacter PetCharacter;

    //tab页id
    public int mActivityId = -1;

    public int ActivityId
    {
        get { return mActivityId; }
        set
        {
            mActivityId = value;
            var playerData = PlayerDataManager.Instance;
            DateTime dt =  Game.Instance.ServerTime.Date.AddDays(1);
            var sp = (dt - Game.Instance.ServerTime);
            switch (mActivityId)
            {
                case -1:
                    DataModule.PageId = 0;
                    return;
                case 0: //恶魔
                {
                    DataModule.LeiJiExp = playerData.GetExData(eExdataDefine.e592).ToString();
                    DataModule.LeftTime = GameUtils.GetTimeDiffString(sp);
                    DataModule.NeedDiamond = playerData.GetExData(eExdataDefine.e594).ToString();
                    DataModule.PageId = 1;
                }
                    break;
                case 1: //血色
                {
                    DataModule.LeiJiExp = playerData.GetExData(eExdataDefine.e591).ToString();
                    DataModule.LeftTime = GameUtils.GetTimeDiffString(sp);
                    DataModule.NeedDiamond = playerData.GetExData(eExdataDefine.e593).ToString();
                    DataModule.PageId = 1;
                }
                    break;
                case 2: //世界boss
                    DataModule.PageId = 2;
                    break;
                case 3: //地图统领
                case 4: //黄金部队
                    DataModule.PageId = 3;
		            break;
				case 13: //蛮荒孤岛
					DataModule.PageId = 32;
                    break;
                case 14: //灵兽岛
                    DataModule.PageId = 64;
                    break;
            }

            if (mActivityId < DungeonActivityCount)
            {
//前几个是副本型活动
                DataModule.FubenIdx = mActivityId;

                CalculateFubenIdAndStartTime(mActivityId);
                OnUpdateTimer(new UpdateActivityTimerEvent(UpdateActivityTimerType.Single));

                var fuben = DataModule.CurFuben;
                fuben.Fuben.ShowOrderBtn = fuben.TimerState != 1;
            }
            else if (mActivityId < DungeonActivityCount + BossActivityCount)
            {
//后两个的是地图上的怪
                DataModule.BossIdx = mActivityId - DungeonActivityCount;
                ChangeBossDataId(DataModule.CurBossUI.BtnList.CurBtn);
            }
            else
            {
//滚动活动
                DataModule.ScrollActId = mActivityId - DungeonActivityCount - BossActivityCount;

                var fuben = DataModule.CurScrollAct;
                fuben.Fuben.ShowOrderBtn = fuben.TimerState != 1;

                var curAct = DataModule.CurScrollAct;
                var tableId = curAct.DyActData.TableId;
                var tbDyAct = Table.GetDynamicActivity(tableId);
                DataModule.PageId = tbDyAct.SufaceTab;
            }
        }
    }

    //副本型活动的个数
    public const int DungeonActivityCount = 3;
    //Boss型活动的个数
    public const int BossActivityCount = 2;
    //滚动活动的个数
    public int ScrollActivityCount;

    //恶魔，血色，世界boss 的副本id（最小id，最大id，引导关id）
    public static readonly int[][] SceneIds =
    {
        new[] {5001, 5007, 5000},
        new[] {4001, 4007, 4000},
        new[] {6000, 6000, 6000}
    };

    //各个类型副本的 TodayCount Exdata index
    public List<int> TodayCountExDataIndex = new List<int>();

    //
    public List<DynamicActivityRecord> DynamicActRecords;

    //
    public QueueUpDataModel QueueUpData
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.QueueUpData; }
    }

    private readonly bool[] mSkipOnce = {false, false, false};

    private readonly List<ActivityData> FubenActivitys = new List<ActivityData>();

    private Coroutine TimteRefresh;
    #endregion

    #region 初始化以及虚函数

    public ActivityController()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_ActivityTabSelectEvent.EVENT_TYPE, OnTabSelected);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUp);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnInitExData);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(DungeonEnterCountUpdate.EVENT_TYPE, OnDungeonEnterCountUpdate);
        EventDispatcher.Instance.AddEventListener(ActivityFuben_ResetQueue_Event.EVENT_TYPE, OnResetQueue);
        EventDispatcher.Instance.AddEventListener(DungeonTipClickedEvent.EVENT_TYPE, OnDungeonTipClicked);
        EventDispatcher.Instance.AddEventListener(QueneUpdateEvent.EVENT_TYPE, OnQueneUpdated);
        EventDispatcher.Instance.AddEventListener(UpdateActivityTimerEvent.EVENT_TYPE, OnUpdateTimer);
        EventDispatcher.Instance.AddEventListener(LoadSceneOverEvent.EVENT_TYPE, OnLoadSceneOver);
        EventDispatcher.Instance.AddEventListener(UIEvent_ButtonClicked.EVENT_TYPE, OnBtnClicked);
        EventDispatcher.Instance.AddEventListener(ActivityCellClickedEvent.EVENT_TYPE, OnActivityCellClicked);
        EventDispatcher.Instance.AddEventListener(ActivityStateChangedEvent.EVENT_TYPE, OnActivityStateChanged);
        EventDispatcher.Instance.AddEventListener(ActivityClose_Event.EVENT_TYPE, OnTabClose);

        EventDispatcher.Instance.AddEventListener(OnCLickGoToActivityByMainUIEvent.EVENT_TYPE, OpenByClickMainUI);

        EventDispatcher.Instance.AddEventListener(OnClickBuyTiliEvent.EVENT_TYPE, OnClickBuyTili);
        

        CleanUp();
    }

    public void OnClickBuyTili(IEvent ievent)
    {
        var e = ievent as OnClickBuyTiliEvent;
        if (e == null)
        {
            return;
        }

        if (e.mType == 0)
        {
            var needDiamond = Table.GetClientConfig(934).ToInt();
            var tiliCount = Table.GetClientConfig(935).ToInt();
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                       string.Format(GameUtils.GetDictionaryText(210124), needDiamond, tiliCount), "",
                       () => { NetManager.Instance.StartCoroutine(OnClickBuyTili()); }, null, false, true);
        }
        else
        {
            NetManager.Instance.StartCoroutine(OnClickBuyTili());
        }
    }
    public IEnumerator OnClickBuyTili()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.PetIsLandBuyTili(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {

                }
                else if (msg.ErrorCode == (int)ErrorCodes.DiamondNotEnough)
                {
                    var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                    EventDispatcher.Instance.DispatchEvent(e);

                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error(".....PetIsLandBuyTili.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....PetIsLandBuyTili.......{0}.", msg.State);
            }
        }
    }

    public void OpenByClickMainUI(IEvent e)
    {
        var nextId = -1;
        if (DataModule != null && DataModule.Fubens != null && DataModule.Fubens.Count > mainUIActivityClickFuBenId)
        {
            var nearestActivity = DataModule.Fubens[mainUIActivityClickFuBenId];
            if (nearestActivity != null && nearestActivity.Fuben != null)
            {
                var tbFuben = Table.GetFuben(nearestActivity.Fuben.FubenId);
                if (tbFuben != null)
                {
                    if (tbFuben.AssistType == 4) //恶魔广场
                    {
                        nextId = 0;
                    }
                    else if (tbFuben.AssistType == 5) //血色城堡
                    {
                        nextId = 1;
                    }
                    else if (tbFuben.AssistType == 6) //世界BOSS
                    {
                        nextId = 2;
                    }
                }
            }
        }
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ActivityUI, new ActivityArguments
        {
            Tab = nextId
        }));
    }
    public void OnShow()
    {
        EventDispatcher.Instance.AddEventListener(BossCellClickedEvent.EVENT_TYPE, OnBossBtnClicked);

        {
            var fubens = DataModule.Fubens;
            // foreach(var fuben in fubens)
            var __enumerator1 = (fubens).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var fuben = __enumerator1.Current;
                {
                    if (fuben.IsShowCount)
                    {
                        fuben.Fuben.ShowOrderBtn = fuben.TimerState != 1;
                    }
                    else
                    {
                        fuben.Fuben.ShowOrderBtn = false;
                    }
                }
            }
        }

        {
            var fubens = DataModule.ScrollActivity;
            // foreach(var fuben in fubens)
            var __enumerator1 = (fubens).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var fuben = __enumerator1.Current;
                {
                    if (fuben.IsShowCount)
                    {
                        fuben.Fuben.ShowOrderBtn = fuben.TimerState != 1;
                    }
                    else
                    {
                        fuben.Fuben.ShowOrderBtn = false;
                    }
                }
            }
        }
    }

    public void Close()
    {
        EventDispatcher.Instance.RemoveEventListener(BossCellClickedEvent.EVENT_TYPE, OnBossBtnClicked);
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as ActivityArguments;
        if (args != null)
        {
            if (args.Tab >= -1)
            {
                ActivityId = args.Tab;
                DataModule.FirstPageId = DataModule.PageId;
                setExpShow();
            }
            else
            {
                Logger.Error("Tab = {0}", args.Tab);
            }
        }
    }
    public IEnumerator RefreshMainUITime(int dicId, int state, int fubenId, DateTime time)
    {
        mainUIActivityClickFuBenId = fubenId;
        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            var formateStr = GameUtils.GetDictionaryText(dicId);
            var nearestActivity = DataModule.Fubens[fubenId];
            if (nearestActivity == null)
            {
                yield break;
            }

            var nearestTime = time;
            var deltaTime = nearestTime - Game.Instance.ServerTime;
            if (nearestTime <= Game.Instance.ServerTime)
            {
                DataModule.TimeDown = "";
                yield break;
            }

            if (deltaTime.TotalSeconds > 0)
            {
                if (state == 0)
                {
                    DataModule.TimeDowmColor = MColor.red;
                }
                else
                {
                    DataModule.TimeDowmColor = MColor.green;
                }
                var timeStr = GameUtils.GetTimeDiffString(nearestTime);
                var tbFuben = Table.GetFuben(nearestActivity.Fuben.FubenId);
                DataModule.TimeDown = string.Format(formateStr, tbFuben.Name, timeStr);
            }
            else
            {
                DataModule.TimeDown = "";
                yield break;
            }

        }
        yield break;
    }

    private void OnTabClose(IEvent e)
    {
        DataModule.IsShowExp = 0;
    }

    private void setExpShow()
    {
        if (ActivityId == 0 || ActivityId == 1)
        {
            if (int.Parse(DataModule.LeiJiExp) > 0 && (mActivityId == 0 || mActivityId == 1) && DataModule.PageId == 1)
            {
                DataModule.IsShowExp = 1;
            }
            else
            {
                DataModule.IsShowExp = 0;
            }
        }
        else
        {
            DataModule.IsShowExp = 0;
        }
    }
    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModule;
    }

    public void CleanUp()
    {
        DataModule = new ActivityDataModel();

        //初始化“地图统领”和“黄金部队”的按钮
        var idx = new[] {0, 0};
        Table.ForeachWorldBOSS(record =>
        {
            if (record.Type > 1)
            {
                return true;
            }
            if (record.IsDisplayClient == 0)
            {
                return true;
            }

            var type = record.Type;
            var ui = DataModule.BossUI[type];
            var btn = new BtnState();
            btn.Index = idx[type]++;
            btn.TableId = record.Id;
            btn.Selected = btn.Index == 0;
            btn.Enabled = true;
            if (btn.Selected)
            {
                ui.BtnList.CurBtn = btn;
            }
            ui.BtnList.Btns.Add(btn);
            return true;
        });

        //初始化滚动条
        DynamicActRecords = new List<DynamicActivityRecord>();
        Table.ForeachDynamicActivity(record =>
        {
            DynamicActRecords.Add(record);
            return true;
        });
        if (DynamicActRecords.Count > 0)
        {
            DynamicActRecords.Sort((x, y) =>
            {
                if (x.Sort < y.Sort)
                {
                    return -1;
                }
                if (x.Sort > y.Sort)
                {
                    return 1;
                }
                return 0;
            });

            var scrollActs = new List<ActivityData>();
            foreach (var record in DynamicActRecords)
            {
                var activity = new ActivityData();
                activity.WaitTime = string.Empty;
                activity.Fuben = new FubenActivityDataModel();
                activity.Fuben.FubenId = record.FuBenID[0];
                activity.DyActData = new DynamicActivityData();
                activity.DyActData.TableId = record.Id;

                var timeState = -1;
                if (BitFlag.GetLow(record.SurfaceInfo, (int) eDungeonInfoType.PlayCount))
                {
                    activity.IsShowCount = true;
                }
                if (BitFlag.GetLow(record.SurfaceInfo, (int) eDungeonInfoType.RestTime))
                {
                    activity.TimeTitleId = 241002;
                    timeState = 1;
                }
                if (BitFlag.GetLow(record.SurfaceInfo, (int) eDungeonInfoType.QuestionTime))
                {
                    timeState = 1;
                }
                activity.TimerState = timeState;

                scrollActs.Add(activity);
            }
            DataModule.ScrollActivity = new ObservableCollection<ActivityData>(scrollActs);

            FubenActivitys.Clear();
            foreach (var fuben in DataModule.Fubens)
            {
                FubenActivitys.Add(fuben);
            }
            foreach (var fuben in DataModule.ScrollActivity)
            {
                if (fuben.Fuben != null)
                {
                    FubenActivitys.Add(fuben);
                }
            }
        }
        else
        {
            Logger.Error("list.Count == 0");
        }
    }

    private int GetTili()
    {
        var res = PlayerDataManager.Instance.GetExData(eExdataDefine.e630);
        return res;
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name.Equals("IsDevilSquareMaxCount"))
        {
            return DataModule.Fubens[0].EnterCount == DataModule.Fubens[0].MaxCount;
        }
        if (name.Equals("IsBloodCastleMaxCount"))
        {
            return DataModule.Fubens[1].EnterCount == DataModule.Fubens[1].MaxCount;
        }
        return null;
    }

    #endregion

    #region 事件响应

    //升级事件的响应函数
    public void OnLevelUp(IEvent ievent)
    {
        CalculateFubenIdAndStartTimes();
    }

    //ExData初始化事件的响应函数
    public void OnInitExData(IEvent ievent)
    {
        if (TodayCountExDataIndex.Count == 0)
        {
            for (int i = 0, imax = SceneIds.Length; i < imax; ++i)
            {
                var ids = SceneIds[i];
                var sceneId = ids[1];
                var tbScene = Table.GetScene(sceneId);
                if (tbScene == null)
                {
                    continue;
                }
                var tbFuben = Table.GetFuben(tbScene.FubenId);
                if (tbFuben == null)
                {
                    continue;
                }

                TodayCountExDataIndex.Add(tbFuben.TodayCountExdata);
            }

            var scrollActs = DataModule.ScrollActivity;
            for (int i = 0, imax = scrollActs.Count; i < imax; ++i)
            {
                var act = scrollActs[i];
                var tbFuben = Table.GetFuben(act.Fuben.FubenId);
                if (tbFuben == null)
                {
                    TodayCountExDataIndex.Add(-1);
                }
                else
                {
                    TodayCountExDataIndex.Add(tbFuben.TodayCountExdata);
                }

                if (act.DyActData != null)
                {
                    var tbDynamic = Table.GetDynamicActivity(act.DyActData.TableId);
                    if (tbDynamic != null && tbDynamic.SufaceTab == 64)
                    {
                        act.TiliValue = GetTili();
                        act.TiliMaxValue = 100;
                        act.TiliPercent = Math.Min((float)act.TiliValue / 100, 1.0f);
                    } 
                }
            }

            var playerData = PlayerDataManager.Instance;
            for (int i = 0, imax = TodayCountExDataIndex.Count; i < imax; ++i)
            {
                ActivityData activity;
                if (i < DataModule.Fubens.Count)
                {
                    activity = DataModule.Fubens[i];
                }
                else
                {
                    activity = scrollActs[i - DataModule.Fubens.Count];
                }
                var exDataIdx = TodayCountExDataIndex[i];
                if (exDataIdx == -1)
                {
                    activity.EnterCount = -1;
                    activity.IsShowCount = false;
                }
                else
                {
                    activity.EnterCount = playerData.GetExData(exDataIdx);
                    if (activity.Fuben.FubenId == -1)
                    {
                        continue;
                    }
                    var tbFuben = Table.GetFuben(activity.Fuben.FubenId);
                    if (tbFuben == null)
                    {
                        continue;
                    }
                    if (tbFuben.AssistType == (int) eDungeonAssistType.AncientBattlefield)
                    {
                        var restTimeSec = tbFuben.TimeLimitMinutes*60 - activity.EnterCount;
                        if (restTimeSec < 0)
                        {
                            restTimeSec = 0;
                        }
                        activity.WaitTime = GameUtils.GetTimeDiffString((int)restTimeSec, true);
                        activity.TimerState = restTimeSec > 0 ? 1 : 0;
                    }
                }
            }
        }

        var thePlayerData = PlayerDataManager.Instance;
        if (mActivityId == 0) // 恶魔
        {
            DataModule.LeiJiExp = thePlayerData.GetExData(eExdataDefine.e592).ToString();
            DataModule.NeedDiamond = thePlayerData.GetExData(eExdataDefine.e594).ToString();
        }
        else if (mActivityId == 1) // 血色
        {
            DataModule.LeiJiExp = thePlayerData.GetExData(eExdataDefine.e591).ToString();
            DataModule.NeedDiamond = thePlayerData.GetExData(eExdataDefine.e593).ToString();
        }
        setExpShow();
    }

    public void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        if (e.Key == (int) eExdataDefine.e592 || e.Key == (int) eExdataDefine.e594 ||
            e.Key == (int) eExdataDefine.e591 || e.Key == (int) eExdataDefine.e593)
        {
            var playerData = PlayerDataManager.Instance;
            if (mActivityId == 0) // 恶魔
            {
                DataModule.LeiJiExp = playerData.GetExData(eExdataDefine.e592).ToString();
                DataModule.NeedDiamond = playerData.GetExData(eExdataDefine.e594).ToString();
            }
            else if (mActivityId == 1) // 血色
            {
                DataModule.LeiJiExp = playerData.GetExData(eExdataDefine.e591).ToString();
                DataModule.NeedDiamond = playerData.GetExData(eExdataDefine.e593).ToString();
            }
            setExpShow();
        }

        if (e.Key == (int)eExdataDefine.e630 || e.Key == (int)eExdataDefine.e631)
        {
            var scrollActs = DataModule.ScrollActivity;
            for (int i = 0, imax = scrollActs.Count; i < imax; ++i)
            {
                var act = scrollActs[i];
                if (act.DyActData != null)
                {
                    var tbDynamic = Table.GetDynamicActivity(act.DyActData.TableId);
                    if (tbDynamic != null && tbDynamic.SufaceTab == 64)
                    {
                        act.TiliValue = GetTili();
                        act.TiliMaxValue = 100;
                        act.TiliPercent = Math.Min((float)act.TiliValue / 100, 1.0f);
                    }
                }
            }
        }
    }

    //副本次数发生变化的响应函数
    public void OnDungeonEnterCountUpdate(IEvent ievent)
    {
        var e = ievent as DungeonEnterCountUpdate;
        var dungeonId = e.DungeonId;
        var count = e.Count;
        var tbFuben = Table.GetFuben(dungeonId);
        if (tbFuben == null)
        {
            Logger.Log2Bugly("tbFuben = null");
            return;
        }
        var index = TodayCountExDataIndex.FindIndex(i => i == tbFuben.TodayCountExdata);
        if (index < 0)
        {
            return;
        }
        ActivityData activity;
        if (index < DataModule.Fubens.Count)
        {
            activity = DataModule.Fubens[index];
        }
        else
        {
            activity = DataModule.ScrollActivity[index - DataModule.Fubens.Count];
        }
        if (activity.EnterCount == count)
        {
            return;
        }
        activity.EnterCount = count;

        //如果是古战场，则需要做特殊处理
        if (tbFuben.AssistType == (int) eDungeonAssistType.AncientBattlefield)
        {
            var restTimeSec = tbFuben.TimeLimitMinutes*60 - activity.EnterCount;
            if (restTimeSec < 0)
            {
                restTimeSec = 0;
            }

            activity.WaitTime = GameUtils.GetTimeDiffString((int)restTimeSec);
            activity.TimerState = restTimeSec > 0 ? 1 : 0;

            //检查，如果当前正在 古战场副本内，则要刷新副本倒计时
            if (GameLogic.Instance == null)
            {
                return;
            }
            if (GameLogic.Instance.Scene == null)
            {
                return;
            }
            var tbScene1 = GameLogic.Instance.Scene.TableScene;
            if (tbScene1.FubenId == -1)
            {
                return;
            }
            var tbFuben1 = Table.GetFuben(tbScene1.FubenId);
            if (tbFuben1.AssistType == (int) eDungeonAssistType.AncientBattlefield)
            {
//是古战场
                var dueTime = Game.Instance.ServerTime.AddSeconds(restTimeSec);
                EventDispatcher.Instance.DispatchEvent(new NotifyDungeonTime(dueTime.ToBinary()));
            }
        }

        //血色和恶魔次数满了之后刷新推送
        if (activity.EnterCount == activity.MaxCount)
        {
            if (tbFuben.AssistType == (int) eDungeonAssistType.DevilSquare)
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(2, 0));
            }
            else if (tbFuben.AssistType == (int) eDungeonAssistType.BloodCastle)
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(3, 0));
            }
        }
    }

    //界面上各种按钮的响应函数
    private void OnBtnClicked(IEvent ievent)
    {
        var e = ievent as UIEvent_ButtonClicked;
        switch (e.Type)
        {
            case BtnType.Activity_Enter: //进入活动
            {
                var activity = DataModule.CurFuben;
                var fuben = activity.Fuben;
                var checkResult = CheckCondition(activity);
                if (checkResult == CheckConditionResult.None)
                {
                    return;
                }
                if (activity.TimerState != 0)
                {
//可以进副本
                }
                else
                {
//当前不在副本开启时间
                    GameUtils.ShowHintTip(494);
                    return;
                }

                var tbFuben = Table.GetFuben(fuben.FubenId);
                if (checkResult == CheckConditionResult.Team)
                {
                    if (QueueUpData.QueueId == -1)
                    {
                        EnterTeamDungeon(fuben.FubenId);
                    }
                    else if (QueueUpData.QueueId == tbFuben.QueueParam)
                    {
                        MatchingCancel(tbFuben.QueueParam, fuben);
                        EnterTeamDungeon(fuben.FubenId);
                    }
                    else
                    {
                        //正在排别的副本，是否进入
                        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270218, "", () =>
                        {
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                            EnterTeamDungeon(fuben.FubenId);
                        });
                    }
                }
                else if (checkResult == CheckConditionResult.Single)
                {
                    if (QueueUpData.QueueId == -1)
                    {
                        GameUtils.EnterFuben(tbFuben.Id);
                    }
                    else if (QueueUpData.QueueId == tbFuben.QueueParam)
                    {
                        MatchingCancel(tbFuben.QueueParam, fuben);
                        GameUtils.EnterFuben(tbFuben.Id);
                    }
                    else
                    {
                        //正在排别的副本，是否进入
                        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270218, "", () =>
                        {
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                            GameUtils.EnterFuben(tbFuben.Id);
                        });
                    }
                }
            }
                break;

            case BtnType.DynamicActivity_Enter: //进入动态活动副本
            {
                var activity = DataModule.CurScrollAct;
                var fuben = activity.Fuben;
                var checkResult = CheckCondition(activity);
                if (checkResult == CheckConditionResult.None)
                {
                    return;
                }
                if (activity.TimerState != 0)
                {
//可以进副本
                }
                else
                {
//当前不在副本开启时间
                    GameUtils.ShowHintTip(494);
                    return;
                }

                var tbFuben = Table.GetFuben(fuben.FubenId);
                if (tbFuben.AssistType == (int) eDungeonAssistType.AncientBattlefield)
                {
                    var restPlayTime = tbFuben.TimeLimitMinutes*60 - activity.EnterCount;
                    if (restPlayTime <= 0)
                    {
//没有时间了
                        GameUtils.ShowHintTip(457);
                        return;
                    }
                }

                if (activity.DyActData != null)
                {
                    var tbDynamic = Table.GetDynamicActivity(activity.DyActData.TableId);
                    if (tbDynamic != null && tbDynamic.SufaceTab == 64) //灵兽岛判断体力
                    {
                        if (GetTili() <= 0)
                        {
                            var needDiamond = Table.GetClientConfig(934).ToInt();
                            var tiliCount = Table.GetClientConfig(935).ToInt();
                            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                                       string.Format(GameUtils.GetDictionaryText(210124), needDiamond, tiliCount), "",
                                       () => { NetManager.Instance.StartCoroutine(OnClickBuyTili()); }, null, false, true);

                            return;
                        }
                    }
                }

                if (checkResult == CheckConditionResult.Team)
                {
                    if (QueueUpData.QueueId == -1)
                    {
                        EnterTeamDungeon(fuben.FubenId);
                    }
                    else if (QueueUpData.QueueId == tbFuben.QueueParam)
                    {
                        MatchingCancel(tbFuben.QueueParam, fuben);
                        EnterTeamDungeon(fuben.FubenId);
                    }
                    else
                    {
                        //正在排别的副本，是否进入
                        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270218, "", () =>
                        {
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                            EnterTeamDungeon(fuben.FubenId);
                        });
                    }
                }
                else if (checkResult == CheckConditionResult.Single)
                {
                    if (QueueUpData.QueueId == -1)
                    {
                        GameUtils.EnterFuben(tbFuben.Id);
                    }
                    else if (QueueUpData.QueueId == tbFuben.QueueParam)
                    {
                        MatchingCancel(tbFuben.QueueParam, fuben);
                        GameUtils.EnterFuben(tbFuben.Id);
                    }
                    else
                    {
                        //正在排别的副本，是否进入
                        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270218, "", () =>
                        {
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                            GameUtils.EnterFuben(tbFuben.Id);
                        });
                    }
                }
            }
                break;

            case BtnType.Activity_Queue: //预约活动
            case BtnType.DynamicActivity_Queue: //预约活动
            {
                ActivityData activity;
                if (e.Type == BtnType.Activity_Queue)
                {
                    activity = DataModule.CurFuben;
                }
                else
                {
                    activity = DataModule.CurScrollAct;
                }
                var fuben = activity.Fuben;
                if (activity.TimerState == 1)
                {
                    if (fuben.QueueState == 0)
                    {
                        GameUtils.ShowHintTip(496);
                        return;
                    }
                }

                var checkResult = CheckCondition(activity);
                if (checkResult == CheckConditionResult.None)
                {
                    return;
                }

                var tbFuben = Table.GetFuben(fuben.FubenId);

                if (QueueUpData.QueueId == -1)
                {
                    MatchingStart(tbFuben.QueueParam);
                }
                else if (QueueUpData.QueueId == tbFuben.QueueParam)
                {
                    MatchingCancel(tbFuben.QueueParam, fuben);
                }
                else
                {
                    //正在排别的副本，是否取消并预约本活动
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 41004, "", () =>
                    {
                        NextQueueId = tbFuben.QueueParam;
                        EventDispatcher.Instance.AddEventListener(QueueCanceledEvent.EVENT_TYPE, OnOtherQueueCanceled);
                        mCoSafeRemoveListener = RemoveQueueCanceledEventListener(5f);
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                    });
                }
            }
                break;

            case BtnType.Activity_FlytoMonster:
            {
                var tbVip = PlayerDataManager.Instance.TbVip;
                if (tbVip.SceneBossTrans == 0)
                {
                    do
                    {
                        tbVip = Table.GetVIP(tbVip.Id + 1);
                    } while (0 == tbVip.SceneBossTrans);

                    GameUtils.GuideToBuyVip(tbVip.Id);
                    return;
                }
                if (GotoMonster(e.Type) == false)
                    break;

                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ActivityUI));
            }
                break;
            case BtnType.Activity_GotoMonster:
            {
                if (GotoMonster(e.Type) == false)
                    break;

                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ActivityUI));
            }
                break;

            case BtnType.Activity_GetDoubleExp:
            {
                if (int.Parse(DataModule.LeiJiExp) <= 0)
                {
                    GameUtils.ShowNetErrorHint((int)ErrorCodes.Error_ExpNotEnough);
                    return;
                }

                if (PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes) < int.Parse(DataModule.NeedDiamond))
                {
                    GameUtils.ShowNetErrorHint((int)ErrorCodes.DiamondNotEnough);
                    return;
                }

                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(100001121), DataModule.NeedDiamond), "",
                    () => { NetManager.Instance.StartCoroutine(ApplyBuyLeijiExp(mActivityId)); }, null, false, true);
            }
                break;
        }
    }

    public bool GotoMonster(BtnType type)
    {
        var myLevel = PlayerDataManager.Instance.GetLevel();
        var curBtn = DataModule.CurBossUI.BtnList.CurBtn;
        var tbBoss = Table.GetWorldBOSS(curBtn.TableId);
        if (tbBoss == null)
        {
            return false;
        }
        var tbSceneNpc = Table.GetSceneNpc(tbBoss.SceneNpc);
        if (tbSceneNpc == null)
        {
            return false;
        }
        var tbScene = Table.GetScene(tbSceneNpc.SceneID);
        if (tbScene == null)
        {
            return false;
        }
        if (tbScene.IsPublic != 1)
        {
            //场景未开放
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200005011));
            return false;
        }
        if (tbScene.LevelLimit > myLevel)
        {
            //等级不足
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210110));
            return false;
        }

        ObjManager.Instance.MyPlayer.LeaveAutoCombat();

        if (type == BtnType.Activity_FlytoMonster)
        {
            if (tbSceneNpc.PosX >= 0.0 && tbSceneNpc.PosX >= 0.0)
            {
                GameUtils.FlyTo(tbSceneNpc.SceneID, (float)tbSceneNpc.PosX, (float)tbSceneNpc.PosZ);
            }
            else
            {
                GameUtils.FlyTo(tbScene.Id, (float)tbScene.Entry_x, (float)tbScene.Entry_z);
            }
        }
        else
        {
            if (tbSceneNpc.PosX >= 0.0 && tbSceneNpc.PosX >= 0.0)
            {
                var command = GameControl.GoToCommand(tbSceneNpc.SceneID, (float)tbSceneNpc.PosX, (float)tbSceneNpc.PosZ, 1.0f);
                GameControl.Executer.PushCommand(command);
            }
            else
            {
                var command = GameControl.GoToCommand(tbScene.Id, (float)tbScene.Entry_x, (float)tbScene.Entry_z, 1.0f);
                GameControl.Executer.PushCommand(command);
            }
        }

        return true;
    }

    public IEnumerator ApplyBuyLeijiExp(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TakeMultyExpAward(type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {

                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error(".....ApplyBuyLeijiExp.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Warn(".....ApplyBuyLeijiExp.......{0}.", msg.State);
            }
        }
    }

    //滚动活动框的点击事件
    private void OnActivityCellClicked(IEvent ievent)
    {
        var e = ievent as ActivityCellClickedEvent;
        DataModule.ScrollActId = e.Index;

        var curAct = DataModule.CurScrollAct;
        var tableId = curAct.DyActData.TableId;
        var tbDyAct = Table.GetDynamicActivity(tableId);
        CalculateDynamicActivity(curAct, e.Index);

        var conditionId = curAct.ClickConditionId;
        if (conditionId != -1)
        {
            var errDic = PlayerDataManager.Instance.CheckCondition(conditionId);
            if (errDic != 0)
            {
                GameUtils.ShowHintTip(errDic);
                return;
            }
        }

        var uiid = tbDyAct.UIID;
        if (uiid == 60)
        {
            DataModule.PageId = tbDyAct.SufaceTab;
        }
        else if (uiid >= 0)
        {
            GameUtils.GotoUiTab(uiid, tbDyAct.SufaceTab);
        }
        else
        {
            Logger.Error("Table DynamicActivity id = {0}, uiid = {1}", tableId, uiid);
        }
    }

    //界面上各种按钮的响应函数
    private void OnActivityStateChanged(IEvent ievent)
    {
        var activityState = PlayerDataManager.Instance.ActivityState;
        foreach (var i in activityState)
        {
            mSkipOnce[i.Key] = i.Value >= (int) eActivityState.WillEnd;
        }
        CalculateFubenIdAndStartTimes();
    }

    //主界面点击取消排队事件的响应函数
    public void OnResetQueue(IEvent ievent)
    {
        var e = ievent as ActivityFuben_ResetQueue_Event;
        var queueId = e.QueueId;
        if (queueId < 0)
        {
            return;
        }
        var tbQueue = Table.GetQueue(queueId);
        if (tbQueue == null)
        {
            return;
        }
        var fubenId = tbQueue.Param;
        var fubens = DataModule.Fubens;
        {
            // foreach(var fuben in fubens)
            var __enumerator2 = (fubens).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var fuben = __enumerator2.Current;
                {
                    if (fuben.Fuben.FubenId == fubenId)
                    {
                        MatchingCancel(queueId, fuben.Fuben);
                        return;
                    }
                }
            }
        }
    }

    //其它的排队已经取消了，现在可以排新的队了
    public void OnOtherQueueCanceled(IEvent ievent)
    {
        MatchingStart(NextQueueId);
        EventDispatcher.Instance.RemoveEventListener(QueueCanceledEvent.EVENT_TYPE, OnOtherQueueCanceled);
        mCoSafeRemoveListener = null;
    }

    //活动点击的响应
    public void OnTabSelected(IEvent ievent)
    {
        var e = ievent as UIEvent_ActivityTabSelectEvent;
        ActivityId = e.TabIdx;
        setExpShow();
    }

    //界面上的倒计时显示处理函数
    public void OnUpdateTimer(IEvent ievent)
    {
        var e = ievent as UpdateActivityTimerEvent;
        var timeThreshold = Game.Instance.ServerTime.AddMinutes(-1);

        if (e.Type == UpdateActivityTimerType.Single)
        {
            var activity = DataModule.CurFuben;
            if (ActivityId >= DungeonActivityCount || ActivityId < 0)
            {
                return;
            }

            activity.WaitTime = GameUtils.GetTimeDiffString(activity.TargetTime);
        }
        else if (e.Type == UpdateActivityTimerType.MainPage)
        {
            //刷新前三个活动的倒计时，即恶魔，血色，世界boss
            for (int i = 0, imax = DataModule.Fubens.Count; i < imax; i++)
            {
                var activity = DataModule.Fubens[i];
                if (activity.TargetTime < timeThreshold)
                {
                    continue;
                }
                activity.WaitTime = GameUtils.GetTimeDiffString(activity.TargetTime);
            }

            //刷新boss活动倒计时，即地图统领，黄金部队
            for (int i = 0, imax = DataModule.BossUI.Count; i < imax; i++)
            {
                var activity = DataModule.BossUI[i];
                if (activity.TargetTime < timeThreshold)
                {
                    continue;
                }
                activity.WaitTime = GameUtils.GetTimeDiffString(activity.TargetTime);
            }

            if (mNextFubenIdx >= 0)
            {
                // 修改Tip
                var nearestActivity = DataModule.Fubens[mNextFubenIdx];
                var nearestTime = nearestActivity.TargetTime;
                var tbFuben = Table.GetFuben(nearestActivity.Fuben.FubenId);
                var timeStr = GameUtils.GetTimeDiffString(nearestTime);
                switch (nearestActivity.TimerState)
                {
                    case 0:
                    {
                        var formateStr = GameUtils.GetDictionaryText(41008);
                        DataModule.Tip = string.Format(formateStr, tbFuben.Name, timeStr, tbFuben.Name);
                    }
                        break;
                    case 1:
                    {
                        var formateStr = GameUtils.GetDictionaryText(41009);
                        DataModule.Tip = string.Format(formateStr, tbFuben.Name);
                    }
                        break;
                }
            }
            else
            {
                DataModule.Tip = string.Empty;
            }
        }

	    DateTime dt = Game.Instance.ServerTime.Date.AddDays(1);;
        var sp = (dt - Game.Instance.ServerTime);
        DataModule.LeftTime = GameUtils.GetTimeDiffString(sp);
    }

    //怪物列表的点击响应函数（“地图统领”和“黄金部队”的列表）
    public void OnBossBtnClicked(IEvent ievent)
    {
        var e = ievent as BossCellClickedEvent;
        var clickedBtn = e.BtnState;
        var btns = DataModule.CurBossUI.BtnList;
        if (clickedBtn.Selected)
        {
            return;
        }

        clickedBtn.Selected = true;
        btns.CurBtn.Selected = false;
        btns.CurBtn = clickedBtn;

        ChangeBossDataId(clickedBtn);
    }

    //主界面活动提示的响应函数
    public void OnDungeonTipClicked(IEvent ievent)
    {
        var e = ievent as DungeonTipClickedEvent;
        var fubenId = e.FubenId;
        var activity = FubenActivitys.Find(a => a.Fuben.FubenId == fubenId);
        var fuben = activity.Fuben;

        var checkResult = CheckCondition(activity);
        if (checkResult == CheckConditionResult.None)
        {
            return;
        }
        if (checkResult == CheckConditionResult.Single && ActivityId < 2)
        {
//如果是血色或者恶魔，需要先判断一下上次的奖励领没领
        }
        else if (!activity.IsShowCount)
        {
//可以进副本
        }
        else if (activity.TimerState == 1)
        {
//可以进副本
        }
        else
        {
//当前不在副本开启时间
            GameUtils.ShowHintTip(494);
            return;
        }

        var tbFuben = Table.GetFuben(fuben.FubenId);
        if (checkResult == CheckConditionResult.Team)
        {
            //正在排队中，是否进入
            if (QueueUpData.QueueId == -1)
            {
                //弹框，询问是否进入副本
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(41006), tbFuben.Name), "",
                    () => { EnterTeamDungeon(fuben.FubenId); });
            }
            else if (QueueUpData.QueueId == tbFuben.QueueParam)
            {
                //弹框，询问是否进入副本
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(41006), tbFuben.Name), "", () =>
                    {
                        MatchingCancel(tbFuben.QueueParam, fuben);
                        EnterTeamDungeon(fuben.FubenId);
                    });
            }
            else
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270218, "", () =>
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                    EnterTeamDungeon(fuben.FubenId);
                });
            }
        }
        else if (checkResult == CheckConditionResult.Single)
        {
            //正在排队中，是否进入
            if (QueueUpData.QueueId == -1)
            {
                //弹框，询问是否进入副本
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(41006), tbFuben.Name), "",
                    () => { GameUtils.EnterFuben(tbFuben.Id); });
            }
            else if (QueueUpData.QueueId == tbFuben.QueueParam)
            {
                //弹框，询问是否进入副本
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(41006), tbFuben.Name), "", () =>
                    {
                        MatchingCancel(tbFuben.QueueParam, fuben);
                        GameUtils.EnterFuben(tbFuben.Id);
                    });
            }
            else
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 270218, "", () =>
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                    GameUtils.EnterFuben(tbFuben.Id);
                });
            }
        }
    }

    //预约（排队）状态变化的响应函数
    public void OnQueneUpdated(IEvent ievent)
    {
        var data = QueueUpData;
        var queueId = data.QueueId;
        if (queueId == -1)
        {
//cancel
            {
                // foreach(var fuben in fubens)
                var __enumerator3 = (FubenActivitys).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var fuben = __enumerator3.Current.Fuben;
                    {
                        fuben.QueueState = 0;
                    }
                }
            }
        }
        else
        {
            {
                // foreach(var fuben in fubens)
                var __enumerator4 = (FubenActivitys).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var fuben = __enumerator4.Current.Fuben;
                    {
                        var tbFuben = Table.GetFuben(fuben.FubenId);
                        if (tbFuben != null)
                        {
                            if (tbFuben.QueueParam == queueId)
                            {
                                fuben.QueueState = 1;
                            }
                            else
                            {
                                fuben.QueueState = 0;
                            }
                        }
                    }
                }
            }
        }
    }

    //场景加载结束的响应函数，这时需要重新计算下活动时间，以便能在主界面上正确的显示活动tip
    public void OnLoadSceneOver(IEvent ievent)
    {
        var tbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
        if (tbScene.FubenId == -1)
        {
            CalculateFubenIdAndStartTimes();
        }

        if (TimteRefresh == null)
        {
            DataModule.TimeDown = "";
        }
    }

    #endregion

    #region 进入活动

    #region 检查进入活动的条件

    //检查结果
    public enum CheckConditionResult
    {
        None, //不符合条件
        Single, //符合条件，且是单人
        Team //符合条件，且是组队
    }

    //检查物品是否够
    private CheckConditionResult CheckItemEnough(FubenRecord tbFuben)
    {
        for (int i = 0, imax = tbFuben.NeedItemId.Count; i < imax; ++i)
        {
            var id = tbFuben.NeedItemId[i];
            var count = tbFuben.NeedItemCount[i];
            if (id == -1)
            {
                break;
            }
            if (PlayerDataManager.Instance.GetItemCount(id) < count)
            {
//材料不足
                var tbItem = Table.GetItemBase(id);
                var content = string.Format(GameUtils.GetDictionaryText(270246), tbItem.Name);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, content, GameUtils.GetDictionaryText(1503),
                    () => { GameUtils.GotoUiTab(26, tbItem.Exdata[3]); });
                return CheckConditionResult.None;
            }
        }
        return CheckConditionResult.Single;
    }

    //检查进副本的条件（是队伍还是个人，以及是否满足进入条件）
    public CheckConditionResult CheckCondition(ActivityData activity)
    {
        // Team
        var tbFuben = Table.GetFuben(activity.Fuben.FubenId);
        var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
        var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);
        var teamCount = team.Count();
        if (teamCount == 0 || tbFuben.CanGroupEnter != 1)
        {
            return CheckSingleCondition(activity);
        }
        return CheckTeamCondition(activity);
    }

    //检查单人进副本的条件
    public CheckConditionResult CheckSingleCondition(ActivityData activity)
    {
        var fubenId = activity.Fuben.FubenId;
        var tbFuben = Table.GetFuben(fubenId);
        if (tbFuben == null)
        {
            Logger.Error("In CheckSingleCondition(), tbFuben == null, fubenId = {0}", fubenId);
            return CheckConditionResult.None;
        }

        var playerData = PlayerDataManager.Instance;
        var maxCount = tbFuben.TodayCount;
        if (tbFuben.AssistType == 4)
        {
//恶魔
            maxCount += playerData.TbVip.DevilBuyCount;
        }
        else if (tbFuben.AssistType == 5)
        {
//血色
            maxCount += playerData.TbVip.BloodBuyCount;
        }
        if (tbFuben != null && tbFuben.TodayCount != -1 && activity.EnterCount >= maxCount)
        {
//副本次数达到上限
            GameUtils.ShowHintTip(490);
            return CheckConditionResult.None;
        }

        return CheckItemEnough(tbFuben);
    }

    //检查组队进副本的条件
    public CheckConditionResult CheckTeamCondition(ActivityData activity)
    {
        // Team
        var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
        var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);

        if (teamData.TeamList[0].Guid != ObjManager.Instance.MyPlayer.GetObjId())
        {
//我不是队长
            GameUtils.ShowHintTip(440);
            return CheckConditionResult.None;
        }

        var fuben = activity.Fuben;
        var fubenId1 = fuben.FubenId;
        var tbFuben1 = Table.GetFuben(fubenId1);
        var tbScene1 = Table.GetScene(tbFuben1.SceneId);
        var tbScene2 = Table.GetScene(tbFuben1.SceneId + 1);
        var lvMin = tbScene1.LevelLimit;
        var lvMax = Constants.LevelMax;
        if (tbFuben1.OpenTime[0] != -1 && tbScene2 != null && tbScene2.Name == tbScene1.Name)
        {
            lvMax = tbScene2.LevelLimit;
        }

        //检查等级
        var name = string.Empty;
        {
            // foreach(var t in team)
            var __enumerator5 = (team).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var t = __enumerator5.Current;
                {
                    if (t.Level < lvMin || t.Level > lvMax)
                    {
                        name += t.Name + ",";
                    }
                }
            }
        }
        if (name.Length > 0)
        {
            name = name.Substring(0, name.Length - 1);
            EventDispatcher.Instance.DispatchEvent(
                new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(495), name)));
            return CheckConditionResult.None;
        }
        if (CheckSingleCondition(activity) == CheckConditionResult.None)
        {
            return CheckConditionResult.None;
        }
        return CheckConditionResult.Team;
    }

    #endregion

    //组队进入活动副本
    public void EnterTeamDungeon(int fubenId)
    {
        var tbFuben = Table.GetFuben(fubenId);
        if (tbFuben == null)
        {
            return;
        }

        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        if (sceneId == tbFuben.SceneId)
        {
            //已经在此副本当中了
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270081));
            return;
        }

        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            var oldTbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            var newTbScene = Table.GetScene(sceneId);

            if (oldTbScene != null && newTbScene != null)
            {
                if (oldTbScene.FubenId != -1 && newTbScene.FubenId != -1)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210123));
                    return;
                }
            }
        }

        NetManager.Instance.StartCoroutine(EnterTeamDungeonCoroutine(fubenId));
    }

    public IEnumerator EnterTeamDungeonCoroutine(int fubenId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamEnterFuben(fubenId, PlayerDataManager.Instance.ServerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
                else if (DealWithErrorCode(msg.ErrorCode, fubenId, msg.Response.Items))
                {
                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error(".....EnterTeamDungeonCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....EnterTeamDungeonCoroutine.......{0}.", msg.State);
            }
        }
    }

    //预约活动副本
    public void MatchingStart(int queueId)
    {
        NetManager.Instance.StartCoroutine(MatchingStartCoroutine(queueId));
    }

    public IEnumerator MatchingStartCoroutine(int queueId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.MatchingStart(queueId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
                else
                {
                    var tbQueue = Table.GetQueue(queueId);
                    if (tbQueue != null && DealWithErrorCode(msg.ErrorCode, tbQueue.Param, msg.Response.CharacterId))
                    {
                    }
                    else
                    {
                        GameUtils.ShowNetErrorHint(msg.ErrorCode);
                        Logger.Error(".....MatchingStart.......{0}.", msg.ErrorCode);
                    }
                }
            }
            else
            {
                Logger.Warn(".....MatchingStart.......{0}.", msg.State);
            }
        }
    }

    //取消预约活动副本
    public void MatchingCancel(int queueId, FubenActivityDataModel fuben)
    {
        NetManager.Instance.StartCoroutine(MatchingCancelCoroutine(queueId, fuben));
    }

    public IEnumerator MatchingCancelCoroutine(int queueId, FubenActivityDataModel fuben)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.MatchingCancel(queueId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    fuben.QueueState = 0;
                    QueueUpData.QueueId = -1;
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(Game.Instance.ServerTime,
                        -1));
                    EventDispatcher.Instance.DispatchEvent(new QueueCanceledEvent());
                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error(".....MatchingCancel.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....MatchingCancel.......{0}.", msg.State);
            }
        }
    }

    public static Dictionary<int, int> DealErrs = new Dictionary<int, int>
    {
        {(int) ErrorCodes.Error_LevelNoEnough, 300901},
        {(int) ErrorCodes.Error_FubenCountNotEnough, 466},
        {(int) ErrorCodes.ItemNotEnough, 467},
        {(int) ErrorCodes.Error_FubenRewardNotReceived, 497},
        {(int) ErrorCodes.Unline, 498},
        {(int) ErrorCodes.Error_CharacterOutLine, 498},
        {(int) ErrorCodes.Error_AlreadyInThisDungeon, 493},
        {(int) ErrorCodes.Error_CharacterCantQueue, 544}
    };

    //处理网络错误消息
    public bool DealWithErrorCode(int errCode, int fubenId, List<ulong> playerIds)
    {
        if (DealErrs.Keys.Contains(errCode))
        {
            var dicId = DealErrs[errCode];
            if (playerIds.Count <= 0)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicId));
            }
            else
            {
                var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
                var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);
                var players = team.Where(p => playerIds.Contains(p.Guid));
                var names = players.Aggregate(string.Empty, (current, p) => current + (p.Name + ","));
                if (names.Length <= 0)
                {
                    return true;
                }
                //特殊处理！！！
                if (errCode == (int) ErrorCodes.Error_LevelNoEnough)
                {
                    var tbFuben = Table.GetFuben(fubenId);
                    var assistType = (eDungeonAssistType) tbFuben.AssistType;
                    if (assistType == eDungeonAssistType.BloodCastle || assistType == eDungeonAssistType.DevilSquare)
                    {
                        var playerData = PlayerDataManager.Instance;
                        var fubenCount = playerData.GetExData(tbFuben.TotleExdata);
                        if (fubenCount > 0)
                        {
                            dicId = 489;
                        }
                        else
                        {
                            dicId = 491;
                        }
                    }
                }
                names = names.Substring(0, names.Length - 1);
                var content = string.Format(GameUtils.GetDictionaryText(dicId), names);
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(content));
            }
            return true;
        }
        return false;
    }

    #endregion

    #region 其它

    public object mFubenTimer;

    //用来计算所有副本活动的时间
    public void CalculateFubenIdAndStartTimes()
    {
        var now = Game.Instance.ServerTime;
        var nearestTime = now.AddYears(10);
        mNextFubenIdx = -1;

        for (int i = 0, imax = DungeonActivityCount; i < imax; ++i)
        {
            var activity = DataModule.Fubens[i];
            var time = activity.TargetTime;
            if (time <= now || activity.TimerState == 1)
            {
                CalculateFubenIdAndStartTime(i);
            }
            time = activity.TargetTime;

            if (time >= now)
            {
                if (nearestTime > time)
                {
                    nearestTime = time;
                    mNextFubenIdx = i;
                }
            }
        }

        //刷新boss活动倒计时，即地图统领，黄金部队
        for (int i = 0, imax = DataModule.BossUI.Count; i < imax; i++)
        {
            var bossUi = DataModule.BossUI[i];
            CalculateBossActTime(bossUi);
            var time = bossUi.TargetTime;
            if (time >= now)
            {
                if (nearestTime > time)
                {
                    nearestTime = time;
                }
            }
        }

        for (int i = 0, imax = DynamicActRecords.Count; i < imax; ++i)
        {
            var act = DataModule.ScrollActivity[i];
            CalculateDynamicActivity(act, i);
            var time = act.TargetTime;
            if (time >= now)
            {
                if (nearestTime > time)
                {
                    nearestTime = time;
                }
            }
        }

        if (mNextFubenIdx >= 0)
        {
            // 设置下一次的timer
            if (mFubenTimer != null)
            {
                TimeManager.Instance.DeleteTrigger(mFubenTimer);
            }
            mFubenTimer = TimeManager.Instance.CreateTrigger(nearestTime, CalculateFubenIdAndStartTimes);
        }
    }

    //用来计算一个副本活动的时间，并负责通知主界面显示活动提示
    public void CalculateFubenIdAndStartTime(int index)
    {
        if (index >= SceneIds.Length)
        {
            return;
        }
        if (index < 0)
        {
            Logger.Error("--------------Error!!!sceneIdx = {0}!!!----------------", index);
            return;
        }

        var playerData = PlayerDataManager.Instance;
        var myLevel = playerData.GetLevel();
        if (myLevel < 0)
        {
            return;
        }
        var ids = SceneIds[index];
        var tbScene = Table.GetScene(ids[0]);
        if (tbScene == null)
        {
            return;
        }

        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return;
        }

        var totalCount = playerData.GetExData(tbFuben.TotleExdata);

        var fubenId = -1;
        if (index < 2 && totalCount == 0)
        {
//副本完成次数为0次，则取引导关id（血色，恶魔）
            fubenId = ids[2];
        }
        else
        {
            //倒序便利查找适合我等级的副本
            for (int i = ids[1], imax = ids[0]; i >= imax; --i)
            {
                tbScene = Table.GetScene(i);
                tbFuben = Table.GetFuben(tbScene.FubenId);
                var warnDict = PlayerDataManager.Instance.CheckCondition(tbFuben.EnterConditionId);
                if (warnDict != 0)
                {
                    continue;
                }

                fubenId = tbScene.FubenId;
                break;
            }
        }

        var activity = DataModule.Fubens[index];
        var fuben = activity.Fuben;
        fuben.FubenId = fubenId;
        if (fubenId == -1)
        {
            activity.IsGrey = 1;
            return;
        }
        tbFuben = Table.GetFuben(fubenId);
        if (tbFuben == null)
        {
            activity.IsGrey = 1;
            return;
        }

        SetActivityConditionId(activity, tbFuben.EnterConditionId);
        var maxCount = tbFuben.TodayCount;
        if (playerData.TbVip != null)
        {
            if (index == 0)
            {
                //恶魔
                maxCount += playerData.TbVip.DevilBuyCount;
            }
            else if (index == 1)
            {
                //血色
                maxCount += playerData.TbVip.BloodBuyCount;
            }
        }
        activity.MaxCount = maxCount;

        #region 计算奖励

        var items = new Dictionary<int, int>();
        for (int i = 0, imax = tbFuben.DisplayCount.Count; i < imax; ++i)
        {
            var itemId = tbFuben.DisplayReward[i];
            if (itemId == -1)
            {
                break;
            }
            var itemCount = tbFuben.DisplayCount[i];
            items.modifyValue(itemId, itemCount);
        }

        if (tbFuben.IsDynamicExp == 1)
        {
            var tbLevelData = Table.GetLevelData(myLevel);
            if (tbLevelData != null)
            {
                var exp = (int) (1.0*tbFuben.DynamicExpRatio*tbLevelData.DynamicExp/10000);
                items.modifyValue((int) eResourcesType.ExpRes, exp);
            }
        }

        var keys = items.Keys.ToList();
        keys.Sort();
        for (int i = 0, imax = fuben.ItemId.Count, keyLen = keys.Count; i < imax; i++)
        {
            if (i < keyLen)
            {
                var key = keys[i];
                fuben.ItemId[i] = key;
                fuben.ItemCount[i] = items[key];
            }
            else
            {
                fuben.ItemId[i] = -1;
            }
        }

        #endregion

        activity.IsShowCount = tbFuben.TodayCount > 1;
        if (tbFuben.OpenTime[0] == -1)
        {
            activity.TimerState = 1;
            return;
        }

        //计算活动进入时间
        var now = Game.Instance.ServerTime;
        var skipOnce = mSkipOnce[index];
        var bMatchOpenTime = false;
        {
            // foreach(var time in tbFuben.OpenTime)
            var __enumerator6 = (tbFuben.OpenTime).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                if (skipOnce)
                {
                    skipOnce = false;
                    continue;
                }

                var time = __enumerator6.Current;
                {
                    var startTime = new DateTime(now.Year, now.Month, now.Day, time/100, time%100, 0);
                    if (startTime >= now)
                    {
//活动尚未开始进入
                        bMatchOpenTime = true;
                        activity.TargetTime = startTime;
                        activity.TimerState = 0;
                        break;
                    }

                    //判断是否在活动进入时间内
                    var endTime = startTime.AddMinutes(tbFuben.CanEnterTime);
                    if (endTime >= now)
                    {
                        bMatchOpenTime = true;
                        activity.TargetTime = endTime;
                        activity.TimerState = 1;

                        //通知主界面显示，活动副本的提示
                        EventDispatcher.Instance.DispatchEvent(new ShowActivityTipEvent(fubenId, 41000, startTime,
                            endTime));

                        if (index != -1 && DataModule.Fubens.Count > index)
                        {
                            if (TimteRefresh == null)
                            {
                                if (activity.TimerState == 0)
                                {
                                    TimteRefresh = NetManager.Instance.StartCoroutine(RefreshMainUITime(41022, 0, index, activity.TargetTime));
                                }
                                else
                                {
                                    TimteRefresh = NetManager.Instance.StartCoroutine(RefreshMainUITime(41023, 1, index, activity.TargetTime));
                                }
                            }
                            else
                            {
                                NetManager.Instance.StopCoroutine(TimteRefresh);
                                TimteRefresh = null;

                                if (activity.TimerState == 0)
                                {

                                    TimteRefresh = NetManager.Instance.StartCoroutine(RefreshMainUITime(41022, 0, index, activity.TargetTime));
                                }
                                else
                                {
                                    TimteRefresh = NetManager.Instance.StartCoroutine(RefreshMainUITime(41023, 1, index, activity.TargetTime));
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }
        //如果没有匹配上开启时间，则下一次开启时间应该是在第二天
        if (!bMatchOpenTime)
        {
            var time = tbFuben.OpenTime[0];
            var tarTime = new DateTime(now.Year, now.Month, now.Day, time/100, time%100, 0).AddDays(1);
            activity.TargetTime = tarTime;
            activity.TimerState = 0;
        }
        if (index == 2)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tbFuben.OpenTime.Count; i++)
            {
                var time = tbFuben.OpenTime[i];
                var tarTime = new DateTime(now.Year, now.Month, now.Day, time / 100, time % 100, 0).AddDays(1);
                sb.AppendFormat(tarTime.Hour + ":" + tarTime.Minute);
                if (i != tbFuben.OpenTime.Count - 1)
                {
                    sb.AppendFormat("|");
                }
            }
            fuben.StartTime = sb.ToString();
            //var worldBossTime = activity.TargetTime;
            //fuben.StartTime = worldBossTime.Hour + ":" + worldBossTime.Minute;
        }
    }

    private void SetActivityConditionId(ActivityData activity, int id)
    {
        var result = PlayerDataManager.Instance.CheckCondition(id);
        activity.IsGrey = result != 0 ? 1 : 0;
        activity.ClickConditionId = id;
    }

    //用来计算一个BOSS活动的时间
    public void CalculateBossActTime(ActivityData bossUi)
    {
        var now = Game.Instance.ServerTime;
        var destTime = now.AddYears(10);
        foreach (var btn in bossUi.BtnList.Btns)
        {
            var tbWolrdBoss = Table.GetWorldBOSS(btn.TableId);
            var times = tbWolrdBoss.RefleshTime.Split('|');
            if (!times[0].Contains(':'))
            {
                continue;
            }
            var bMatchOpenTime = false;
            foreach (var t in times)
            {
                var tt = t.Split(':');
                var time = new DateTime(now.Year, now.Month, now.Day, tt[0].ToInt(), tt[1].ToInt(), 0);
                if (time >= now && destTime > time)
                {
                    bMatchOpenTime = true;
                    destTime = time;
                    break;
                }
            }
            if (!bMatchOpenTime)
            {
                var tt = times[0].Split(':');
                var time = new DateTime(now.Year, now.Month, now.Day, tt[0].ToInt(), tt[1].ToInt(), 0).AddDays(1);
                if (destTime > time)
                {
                    destTime = time;
                }
            }
        }
        bossUi.WaitTime = GameUtils.GetTimeDiffString(destTime, true);
        bossUi.TargetTime = destTime;
    }

    //用来计算一个动态活动副本的时间
    public void CalculateDynamicActivity(ActivityData activity, int index)
    {
        var record = DynamicActRecords[index];
        var now = Game.Instance.ServerTime;
        var destTime = now;

        var dyActData = activity.DyActData;
        var tbDyAct = Table.GetDynamicActivity(dyActData.TableId);
        var type = (eDynamicActivityType) tbDyAct.Type;
        switch (type)
        {
            case eDynamicActivityType.Dungon:
            {
                var myLevel = PlayerDataManager.Instance.GetLevel();
                if (myLevel < 0)
                {
                    return;
                }
                var tbFuben = Table.GetFuben(record.FuBenID[0]);

                var fubenId = -1;
                //倒序便利查找适合我等级的副本
                for (int i = record.FuBenID.Length - 1, imax = 0; i >= imax; --i)
                {
                    var id = record.FuBenID[i];
                    if (id < 0)
                    {
                        continue;
                    }
                    var tbFuben1 = Table.GetFuben(id);
                    if (tbFuben1 == null)
                    {
                        Logger.Error("Error tbFuben1 == null!");
                        continue;
                    }
                    var warnDict = PlayerDataManager.Instance.CheckCondition(tbFuben1.EnterConditionId);
                    if (warnDict != 0)
                    {
                        continue;
                    }

                    fubenId = id;
                    break;
                }

                if (fubenId == -1)
                {
                    fubenId = tbFuben.Id;
                }

                var fuben = activity.Fuben;
                fuben.FubenId = fubenId;
                tbFuben = Table.GetFuben(fubenId);
                SetActivityConditionId(activity, tbDyAct.OpenCondition);
                if (tbFuben == null)
                {
                    break;
                }

                //计算活动进入时间
                if (tbFuben.OpenTime[0] != -1)
                {
                    var bMatchOpenTime = false;
                    var __enumerator6 = (tbFuben.OpenTime).GetEnumerator();
                    while (__enumerator6.MoveNext())
                    {
                        var time = __enumerator6.Current;
                        {
                            var startTime = new DateTime(now.Year, now.Month, now.Day, time/100, time%100, 0);
                            if (startTime >= now)
                            {
//活动尚未开始进入
                                bMatchOpenTime = true;
                                activity.TargetTime = startTime;
                                fuben.ShowOrderBtn = true;
                                break;
                            }

                            //判断是否在活动进入时间内
                            var endTime = startTime.AddMinutes(tbFuben.OpenLastMinutes);
                            if (endTime >= now)
                            {
                                bMatchOpenTime = true;
                                activity.TargetTime = endTime;
                                fuben.ShowOrderBtn = false;

                                //通知主界面显示，活动副本的提示
                                EventDispatcher.Instance.DispatchEvent(new ShowActivityTipEvent(fubenId, 41000,
                                    startTime, endTime));

                                break;
                            }
                        }
                    }

                    //如果没有匹配上开启时间，则下一次开启时间应该是在第二天
                    if (!bMatchOpenTime)
                    {
                        var time = tbFuben.OpenTime[0];
                        var tarTime = new DateTime(now.Year, now.Month, now.Day, time/100, time%100, 0).AddDays(1);
                        activity.TargetTime = tarTime;
                        fuben.ShowOrderBtn = true;
                    }
                }

                //奖励
                for (int i = 0, imax = tbFuben.DisplayReward.Count; i < imax; ++i)
                {
                    fuben.ItemId[i] = tbFuben.DisplayReward[i];
                    fuben.ItemCount[i] = tbFuben.DisplayCount[i];
                }

                activity.IsShowCount = tbFuben.TodayCount >= 1;
                activity.MaxCount = tbFuben.TodayCount;
            }
                break;
            case eDynamicActivityType.Question:
            {
                var playerData = PlayerDataManager.Instance;
                var nowCount = playerData.GetExData(436);
                var maxCount = Table.GetClientConfig(581).ToInt();

                if (nowCount < maxCount)
                {
                    var startHour = Table.GetClientConfig(206).ToInt();
                    var endHour = Table.GetClientConfig(207).ToInt();
                    var startTime = new DateTime(now.Year, now.Month, now.Day, startHour, 0, 0);
                    var endTime = new DateTime(now.Year, now.Month, now.Day, endHour, 0, 0);
                    var timeTitleId = 0;
                    if (startTime >= now)
                    {
                        destTime = startTime;
                        timeTitleId = 241000;
                        activity.TimerState = 0;
                    }
                    else if (endTime >= now)
                    {
                        destTime = endTime;
                        timeTitleId = 241003;
                        activity.TimerState = 1;
                    }
                    else
                    {
                        destTime = startTime.AddDays(1);
                        timeTitleId = 241000;
                        activity.TimerState = 0;
                    }
                    activity.WaitTime = GameUtils.GetTimeDiffString(destTime);
                    activity.TargetTime = destTime;
                    activity.TimeTitleId = timeTitleId;
                }
                else
                {
                    activity.TimerState = 0;
                    activity.WaitTime = string.Empty;
                    activity.TimeTitleId = 100000308;
                }
                SetActivityConditionId(activity, tbDyAct.OpenCondition);
            }
                break;
            case eDynamicActivityType.Tower:
                {
                    var playerData = PlayerDataManager.Instance;
                    var nowCount = playerData.GetExData(436);
                    var maxCount = Table.GetClientConfig(581).ToInt();

                    if (nowCount < maxCount)
                    {
                        var startHour = Table.GetClientConfig(206).ToInt();
                        var endHour = Table.GetClientConfig(207).ToInt();
                        var startTime = new DateTime(now.Year, now.Month, now.Day, startHour, 0, 0);
                        var endTime = new DateTime(now.Year, now.Month, now.Day, endHour, 0, 0);
                        var timeTitleId = 0;
                        if (startTime >= now)
                        {
                            destTime = startTime;
                            timeTitleId = 241000;
                            activity.TimerState = 0;
                        }
                        else if (endTime >= now)
                        {
                            destTime = endTime;
                            timeTitleId = 241003;
                            activity.TimerState = 1;
                        }
                        else
                        {
                            destTime = startTime.AddDays(1);
                            timeTitleId = 241000;
                            activity.TimerState = 0;
                        }
                        activity.WaitTime = GameUtils.GetTimeDiffString(destTime);
                        activity.TargetTime = destTime;
                        activity.TimeTitleId = timeTitleId;
                    }
                    else
                    {
                        activity.TimerState = 0;
                        activity.WaitTime = string.Empty;
                        activity.TimeTitleId = 100000308;
                    }
                    SetActivityConditionId(activity, tbDyAct.OpenCondition);
                }
                break;
        }
    }

    public Coroutine mCoSafeRemoveListener;
    public int NextQueueId;

    //用来移除对QueueCanceledEvent事件的监听
    public Coroutine RemoveQueueCanceledEventListener(float sec)
    {
        if (mCoSafeRemoveListener != null)
        {
            NetManager.Instance.StopCoroutine(mCoSafeRemoveListener);
            mCoSafeRemoveListener = null;
        }
        return NetManager.Instance.StartCoroutine(RemoveQueueCanceledEventListenerCoroutine(sec));
    }

    public IEnumerator RemoveQueueCanceledEventListenerCoroutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        EventDispatcher.Instance.RemoveEventListener(QueueCanceledEvent.EVENT_TYPE, OnOtherQueueCanceled);
        mCoSafeRemoveListener = null;
    }

    //宠物模型展示
    public void ChangeBossDataId(BtnState btnState)
    {
        var tbWorldBoss = Table.GetWorldBOSS(btnState.TableId);
        var tbSceneNpc = Table.GetSceneNpc(tbWorldBoss.SceneNpc);

        DataModule.BossDataId = tbSceneNpc.DataID;
    }

    #endregion
}