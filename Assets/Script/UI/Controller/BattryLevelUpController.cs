/********************************************************************************* 
                         Scorpion

  *FileName:BattryUpgradeFrameCtrler
  *Version:1.0
  *Date:2017-06-08
  *Description:
**********************************************************************************/ 
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
using DataContract;
#endregion

public class BattryUpgradeFrameCtrler : IControllerBase
{

    #region 静态变量

    //恶魔，血色，世界boss 的副本id（最小id，最大id，引导关id）
    public static readonly int[][] s_arraryScenelds =
    {
        new[] {5001, 5007, 5000},
        new[] {4001, 4007, 4000},
        new[] {6000, 6000, 6000},
        new[] {6110, 6110, 6110},
        new[] {6111, 6111, 6111},
        new[] {6112, 6112, 6112}
    };
    //副本型活动的个数
    public const int s_iDungeonAckrtivityCount = 3;
    //Boss型活动的个数
    public const int s_iBossActivityCount = 2;
    private static int s_iItemId = -1;

    #endregion

    #region 成员变量

    private int m_iNextFubenIdx = -1;
    //活动状态ID  
    public int m_iTowersID = 0;
    public int m_iTokrwerIndex = 0;
    public int m_iCruActivityID;    
    //滚动活动的个数
    public int m_iScrollActivityCount;
    //各个类型副本的 TodayCount Exdata index
    public List<int> m_listTodayCountExDataIndex = new List<int>();

    //
    public List<DynamicActivityRecord> m_listDynamicActRecords;
    private readonly bool[] m_bSkipOnce = { false, false, false };
    private readonly List<ActivityData> m_listFubenActivitys = new List<ActivityData>();
    private Coroutine TimteRefresh;
    
    public Coroutine CoSafeRemoveListener;
    public int m_iNextQueueId;
    private Dictionary<int, ulong> m_dicTowerGUIDFac = new Dictionary<int, ulong>();

    #endregion

    #region 构造函数

    public BattryUpgradeFrameCtrler()
    {
        //        EventDispatcher.Instance.AddEventListener(UIEvent_ActivityTabSelectEvent.EVENT_TYPE, OnTabSelected);
        //        EventDispatcher.Instance.AddEventListener(UIEvent_ButtonClicked.EVENT_TYPE, OnBtnClicked);
        //        EventDispatcher.Instance.AddEventListener(PickUpNpc_Event.EVENT_TYPE, ApplyPickUpNpc);
        //        EventDispatcher.Instance.AddEventListener(MieShiSetActivityId_Event.EVENT_TYPE, SetActivityID);
        //        EventDispatcher.Instance.AddEventListener(DungeonTipClickedEvent.EVENT_TYPE, OnDungeonTipClicked);


        EventDispatcher.Instance.AddEventListener(MieShiGetInfo_Event.EVENT_TYPE, OnMieShiDataEvent);

        EventDispatcher.Instance.AddEventListener(BattryLevelUpUpLevelBtn_Event.EVENT_TYPE, OnUpgradeButtonEvent);
        EventDispatcher.Instance.AddEventListener(MieShiOnPaotaiBtn_Event.EVENT_TYPE, OnGetBatteryMsgEvent);
        EventDispatcher.Instance.AddEventListener(MieShiOnGXRankingBtn_Event.EVENT_TYPE, OnGXLevelButtonEvent);
        EventDispatcher.Instance.AddEventListener(GXCortributionRank_Event.EVENT_TYPE, OnRenewalCortributeGradeEvent);
        EventDispatcher.Instance.AddEventListener(FubenGXCortributionRank_Event.EVENT_TYPE, OnRenewalCopyCortributeGradeEvent);

        EventDispatcher.Instance.AddEventListener(ApplyPortraitAward_Event.EVENT_TYPE, OnApplyPortraitRewardEvent);
        EventDispatcher.Instance.AddEventListener(ApplyMishiPortrait_Event.EVENT_TYPE, OnApplyPortraitMsgEvent);
        EventDispatcher.Instance.AddEventListener(MieShiShowFightingResult_Event.EVENT_TYPE, OnShowWarResultEvent);
        EventDispatcher.Instance.AddEventListener(MieShiShowSkillTip_Event.EVENT_TYPE, OnShowSkillnessTipEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnPropChangeEvent);
        // 

        CleanUp();
    }

    #endregion

    #region 属性
    public int ActivityRate { get; set; }
    public MonsterDataModel MonsterMiniature { get; set; }
    //
    public QueueUpDataModel QueueRenewal
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.QueueUpData; }
    }
    
    #endregion

    #region 固有函数

    //State
    public FrameState State { get; set; }
    public INotifyPropertyChanged GetDataModel(string name)
    {
        return MonsterMiniature;
    }
    public void OnShow()
    {
    }
    public void Close()
    {
    }
    public void Tick()
    {
    }
    public void RefreshData(UIInitArguments data)
    {
        var _args = data as ActivityArguments;
        SetPropValue();
    }
    public void CleanUp()
    {

        MonsterMiniature = new MonsterDataModel();
    }
    public void OnChangeScene(int sceneId)
    {
    }
    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    #endregion

    #region 逻辑函数

    private void SetPropValue()
    {
        if (-1 == s_iItemId)
        {
            s_iItemId = Table.GetMieShiPublic(1).ItemId;
        }

        MonsterMiniature.ItemCount = PlayerDataManager.Instance.GetItemCount(s_iItemId);

    }
    private IEnumerator ApplyDownWorldCoroutine()
    {

        var _msg = NetManager.Instance.ApplyMieShiData(PlayerDataManager.Instance.ServerId);
        yield return _msg.SendAndWaitUntilDone();
        if (_msg == null || _msg.Response == null || _msg.Response.Datas == null || _msg.Response.Datas.Count == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new HiedMieShiIcon_Event());
        }
        else if (_msg.State == MessageState.Reply)
        {
            if (_msg.ErrorCode == (int)ErrorCodes.OK)
            {
                // mieShiData = msg.Response;

                if (_msg.Response.Datas.Count != 0)
                {
                    ///保存活动中炮塔guid信息
                    m_dicTowerGUIDFac.Clear();

                    EventDispatcher.Instance.DispatchEvent(new MieShiAddActvityTime_Event(0, new DateTime()));
                    for (int i = 0; i < _msg.Response.Datas.Count; i++)
                    {
                        CommonActivityInfo acd = _msg.Response.Datas[i];

                        for (int j = 0; j < _msg.Response.Datas[i].batterys.Count; j++)
                        {
                            ActivityBatteryOne abo = _msg.Response.Datas[i].batterys[j];
                            m_dicTowerGUIDFac.Add(i * 1000 + abo.batteryId, abo.batteryGuid);
                        }
                        DateTime t = DateTime.FromBinary((long)acd.actiTime);
                        EventDispatcher.Instance.DispatchEvent(new MieShiAddActvityTime_Event(1, t));
                    }
                }

                var _listInfoData = _msg.Response.Datas;

                var _nearlyId = _msg.Response.currentActivityId;
                MonsterMiniature.CurActivityID = _nearlyId;
                m_iCruActivityID = _nearlyId;

                List<ActivityBatteryOne> BatteryOne = _msg.Response.Datas[_nearlyId - 1].batterys;

                /**for (int i = 0; i < MonsterMiniature.MonsterFubens.Count; i++)
                {
                    if (MonsterMiniature.MonsterFubens[i].ActivityId == msg.Response.currentActivityId)
                    {
                        MieShiFightingRecord fiting = Table.GetMieShiFighting(BatteryOne[nearlyId].level);
                        MonsterMiniature.MonsterFubens[i].activity.Fiting = fiting.LevelFighting;
                        break;
                    }
                }**/
                for (int i = 0; i < _listInfoData.Count; i++)
                {
                    var unit = _listInfoData[i];
                    if (unit.activityId != _nearlyId)
                        continue;
                    var _temp = (long)unit.actiTime;
                    var _dateTime = DateTime.FromBinary(_temp);
                    MonsterMiniature.ActivityTime = _dateTime;

                    MonsterMiniature.ActivityState = unit.state;
                    MonsterMiniature.BaoMingState = unit.applyState;
                }

                NetManager.Instance.StartCoroutine(ApplyPersonDatumMsg());

            }
        }

    }


    public IEnumerator ApplyPurchaseAddUpExp(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.TakeMultyExpAward(type);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {

                }
                else
                {
                    GameUtils.ShowNetErrorHint(_msg.ErrorCode);
                    Logger.Error(".....ApplyBuyLeijiExp.......{0}.", _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Warn(".....ApplyBuyLeijiExp.......{0}.", _msg.State);
            }
        }
    }
    public IEnumerator ApplyPersonDatumMsg()
    {
        var _msg = NetManager.Instance.ApplyPortraitData(PlayerDataManager.Instance.ServerId);
        yield return _msg.SendAndWaitUntilDone();
        if (_msg.State == MessageState.Reply)
        {
            if (_msg.ErrorCode == (int)ErrorCodes.OK)
            {
                PlayerDataManager.Instance.BattleMishiMaster = _msg.Response;
                var _e = new BattleMishiRefreshModelMaster(PlayerDataManager.Instance.BattleMishiMaster);
                EventDispatcher.Instance.DispatchEvent(_e);
            }
        }
    }

    public void CastCopyIdAndBegianTimeEX(int index)
    {
        if (index >= s_arraryScenelds.Length)
        {
            return;
        }
        if (index < 0)
        {
            Logger.Error("--------------Error!!!sceneIdx = {0}!!!----------------", index);
            return;
        }

        var _playerData = PlayerDataManager.Instance;
        var _myLevel = _playerData.GetLevel();
        if (_myLevel < 0)
        {
            return;
        }
        var _ids = s_arraryScenelds[3];
        var _tbScene = Table.GetScene(_ids[index]);
        var _tbFuben = Table.GetFuben(_tbScene.FubenId);
        var _totalCount = _playerData.GetExData(_tbFuben.TotleExdata);

        var _fubenId = -1;


        var _activity = MonsterMiniature.MonsterFubens[index];
        var _fuben = _activity.Fuben;
        _fubenId = _fuben.FubenId;
        //   fuben.FubenId = fubenId;
        if (_fubenId == -1)
        {
            _activity.IsGrey = 1;
            return;
        }
        _tbFuben = Table.GetFuben(_fubenId);
        if (_tbFuben == null)
        {
            _activity.IsGrey = 1;
            return;
        }

        SetActionsCriteriaID(_activity, _tbFuben.EnterConditionId);
        ///初始化灭世之战表ID
        for (int i = 0; i < MonsterMiniature.MonsterFubens.Count; i++)
        {
            //MonsterMiniature.MonsterFubens[i].activity.ActivityId = i + 1;
        }

        #region 计算奖励

        var _items = new Dictionary<int, int>();
        for (int i = 0, imax = _tbFuben.RewardCount.Count; i < imax; ++i)
        {
            var _ItemId = _tbFuben.DisplayReward[i];
            if (_ItemId == -1)
            {
                break;
            }
            var _itemCount = _tbFuben.RewardCount[i];
            _items.modifyValue(_ItemId, _itemCount);
        }

        //if (tbFuben.IsDynamicExp == 1)
        //{
        //    var tbLevelData = Table.GetLevelData(myLevel);
        //    if (tbLevelData != null)
        //    {
        //        var exp = (int)(1.0 * tbFuben.DynamicExpRatio * tbLevelData.DynamicExp / 10000);
        //        items.modifyValue((int)eResourcesType.ExpRes, exp);
        //    }
        //}

        var _keys = _items.Keys.ToList();
        //  keys.Sort();
        for (int i = 0, imax = _fuben.ItemId.Count, keyLen = _keys.Count; i < imax; i++)
        {
            if (i < keyLen)
            {
                var _key = _keys[i];
                _fuben.ItemId[i] = _key;
                _fuben.ItemCount[i] = _items[_key];
            }
            else
            {
                _fuben.ItemId[i] = -1;
            }
        }

        #endregion

        _activity.IsShowCount = _tbFuben.TodayCount > 1;
        if (_tbFuben.OpenTime[0] == -1)
        {
            _activity.TimerState = 1;
            return;
        }

        //计算活动进入时间
        var _now = Game.Instance.ServerTime;
        var _skipOnce = m_bSkipOnce[index];
        var _bMatchOpenTime = false;
        {
            // foreach(var time in tbFuben.OpenTime)
            var _enumerator6 = (_tbFuben.OpenTime).GetEnumerator();
            while (_enumerator6.MoveNext())
            {
                if (_skipOnce)
                {
                    _skipOnce = false;
                    continue;
                }

                var time = _enumerator6.Current;
                {
                    var _startTime = new DateTime(_now.Year, _now.Month, _now.Day, time / 100, time % 100, 0);
                    if (_startTime >= _now)
                    {
                        //活动尚未开始进入
                        _bMatchOpenTime = true;
                        _activity.TargetTime = _startTime;
                        _activity.TimerState = 0;
                        break;
                    }

                    //判断是否在活动进入时间内
                    var _endTime = _startTime.AddMinutes(_tbFuben.CanEnterTime);
                    if (_endTime >= _now)
                    {
                        _bMatchOpenTime = true;
                        _activity.TargetTime = _endTime;
                        _activity.TimerState = 1;

                        //通知主界面显示，活动副本的提示
                        EventDispatcher.Instance.DispatchEvent(new ShowActivityTipEvent(_fubenId, 41000, _startTime,
                            _endTime));

                        break;
                    }
                }
            }
            AddSetUpItem();
        }
        //如果没有匹配上开启时间，则下一次开启时间应该是在第二天
        if (!_bMatchOpenTime)
        {
            var _time = _tbFuben.OpenTime[0];
            var _tarTime = new DateTime(_now.Year, _now.Month, _now.Day, _time / 100, _time % 100, 0).AddDays(1);
            _activity.TargetTime = _tarTime;
            _activity.TimerState = 0;
        }
        if (index == 2)
        {
            var _worldBossTime = _activity.TargetTime;
            _fuben.StartTime = _worldBossTime.Hour + ":" + _worldBossTime.Minute;
        }
    }
    private void SetActionsCriteriaID(ActivityData activity, int id)
    {
        var _result = PlayerDataManager.Instance.CheckCondition(id);
        activity.IsGrey = _result != 0 ? 1 : 0;
        activity.ClickConditionId = id;
    }

    //用来计算一个BOSS活动的时间
    public void CastBossActionsTime(ActivityData bossUi)
    {
        var _now = Game.Instance.ServerTime;
        var _destTime = _now.AddYears(10);
        foreach (var _btn in bossUi.BtnList.Btns)
        {
            var _tbWolrdBoss = Table.GetWorldBOSS(_btn.TableId);
            var _times = _tbWolrdBoss.RefleshTime.Split('|');
            if (!_times[0].Contains(':'))
            {
                continue;
            }
            var _bMatchOpenTime = false;
            foreach (var t in _times)
            {
                var _tt = t.Split(':');
                var _time = new DateTime(_now.Year, _now.Month, _now.Day, _tt[0].ToInt(), _tt[1].ToInt(), 0);
                if (_time >= _now && _destTime > _time)
                {
                    _bMatchOpenTime = true;
                    _destTime = _time;
                    break;
                }
            }
            if (!_bMatchOpenTime)
            {
                var _tt = _times[0].Split(':');
                var _time = new DateTime(_now.Year, _now.Month, _now.Day, _tt[0].ToInt(), _tt[1].ToInt(), 0).AddDays(1);
                if (_destTime > _time)
                {
                    _destTime = _time;
                }
            }
        }
        bossUi.WaitTime = GameUtils.GetTimeDiffString(_destTime, true);
        bossUi.TargetTime = _destTime;
    }

    //用来计算一个动态活动副本的时间
    public void CastDynamicalAction(ActivityData activity, int index)
    {
        var _record = m_listDynamicActRecords[index];
        var _now = Game.Instance.ServerTime;
        var _destTime = _now;

        var _dyActData = activity.DyActData;
        var _tbDyAct = Table.GetDynamicActivity(_dyActData.TableId);
        var _type = (eDynamicActivityType)_tbDyAct.Type;
        switch (_type)
        {
            case eDynamicActivityType.Dungon:
                {
                    var _myLevel = PlayerDataManager.Instance.GetLevel();
                    if (_myLevel < 0)
                    {
                        return;
                    }
                    var _tbFuben = Table.GetFuben(_record.FuBenID[0]);

                    var _fubenId = -1;
                    //倒序便利查找适合我等级的副本
                    for (int i = _record.FuBenID.Length - 1, imax = 0; i >= imax; --i)
                    {
                        var _id = _record.FuBenID[i];
                        if (_id < 0)
                        {
                            continue;
                        }
                        var _tbFuben1 = Table.GetFuben(_id);
                        if (_tbFuben1 == null)
                        {
                            Logger.Error("Error tbFuben1 == null!");
                            continue;
                        }
                        var _warnDict = PlayerDataManager.Instance.CheckCondition(_tbFuben1.EnterConditionId);
                        if (_warnDict != 0)
                        {
                            continue;
                        }

                        _fubenId = _id;
                        break;
                    }

                    if (_fubenId == -1)
                    {
                        _fubenId = _tbFuben.Id;
                    }

                    var _fuben = activity.Fuben;
                    _fuben.FubenId = _fubenId;
                    _tbFuben = Table.GetFuben(_fubenId);
                    SetActionsCriteriaID(activity, _tbDyAct.OpenCondition);
                    if (_tbFuben == null)
                    {
                        break;
                    }

                    //计算活动进入时间
                    if (_tbFuben.OpenTime[0] != -1)
                    {
                        var _bMatchOpenTime = false;
                        var _enumerator6 = (_tbFuben.OpenTime).GetEnumerator();
                        while (_enumerator6.MoveNext())
                        {
                            var _time = _enumerator6.Current;
                            {
                                var _startTime = new DateTime(_now.Year, _now.Month, _now.Day, _time / 100, _time % 100, 0);
                                if (_startTime >= _now)
                                {
                                    //活动尚未开始进入
                                    _bMatchOpenTime = true;
                                    activity.TargetTime = _startTime;
                                    _fuben.ShowOrderBtn = true;
                                    break;
                                }

                                //判断是否在活动进入时间内
                                var _endTime = _startTime.AddMinutes(_tbFuben.OpenLastMinutes);
                                if (_endTime >= _now)
                                {
                                    _bMatchOpenTime = true;
                                    activity.TargetTime = _endTime;
                                    _fuben.ShowOrderBtn = false;

                                    //通知主界面显示，活动副本的提示
                                    EventDispatcher.Instance.DispatchEvent(new ShowActivityTipEvent(_fubenId, 41000,
                                        _startTime, _endTime));

                                    break;
                                }
                            }
                        }

                        //如果没有匹配上开启时间，则下一次开启时间应该是在第二天
                        if (!_bMatchOpenTime)
                        {
                            var _time = _tbFuben.OpenTime[0];
                            var _tarTime = new DateTime(_now.Year, _now.Month, _now.Day, _time / 100, _time % 100, 0).AddDays(1);
                            activity.TargetTime = _tarTime;
                            _fuben.ShowOrderBtn = true;
                        }
                    }

                    //奖励
                    for (int i = 0, imax = _tbFuben.DisplayReward.Count; i < imax; ++i)
                    {
                        _fuben.ItemId[i] = _tbFuben.DisplayReward[i];
                        _fuben.ItemCount[i] = _tbFuben.DisplayCount[i];
                    }

                    activity.IsShowCount = _tbFuben.TodayCount >= 1;
                    activity.MaxCount = _tbFuben.TodayCount;
                }
                break;
            case eDynamicActivityType.Question:
                {
                    var _playerData = PlayerDataManager.Instance;
                    var _nowCount = _playerData.GetExData(436);
                    var _maxCount = Table.GetClientConfig(581).ToInt();

                    if (_nowCount < _maxCount)
                    {
                        var _startHour = Table.GetClientConfig(206).ToInt();
                        var _endHour = Table.GetClientConfig(207).ToInt();
                        var _startTime = new DateTime(_now.Year, _now.Month, _now.Day, _startHour, 0, 0);
                        var _endTime = new DateTime(_now.Year, _now.Month, _now.Day, _endHour, 0, 0);
                        var _timeTitleId = 0;
                        if (_startTime >= _now)
                        {
                            _destTime = _startTime;
                            _timeTitleId = 241000;
                            activity.TimerState = 0;
                        }
                        else if (_endTime >= _now)
                        {
                            _destTime = _endTime;
                            _timeTitleId = 241003;
                            activity.TimerState = 1;
                        }
                        else
                        {
                            _destTime = _startTime.AddDays(1);
                            _timeTitleId = 241000;
                            activity.TimerState = 0;
                        }
                        activity.WaitTime = GameUtils.GetTimeDiffString(_destTime);
                        activity.TargetTime = _destTime;
                        activity.TimeTitleId = _timeTitleId;
                    }
                    else
                    {
                        activity.TimerState = 0;
                        activity.WaitTime = string.Empty;
                        activity.TimeTitleId = 100000308;
                    }
                    SetActionsCriteriaID(activity, _tbDyAct.OpenCondition);
                }
                break;
        }
    }
    private IEnumerator ApplyPeoplePrizeInfo(int idNpc)
    {
        var _msg = NetManager.Instance.ApplyPortraitAward(PlayerDataManager.Instance.ServerId);
        yield return _msg.SendAndWaitUntilDone();
        if (_msg.State == MessageState.Reply)
        {
            if (_msg.ErrorCode == (int)ErrorCodes.OK)
            {
            }
        }
    }
    private ulong GainTowerPos(int id)
    {
        ulong _guid = 0;
        m_dicTowerGUIDFac.TryGetValue(MonsterMiniature.MonsterFubenIdx * 1000 + id, out _guid);
        return _guid;
    }
    #region 灭世炮台技能提升
   
    private IEnumerator RecoverSkilledness(int batteryId, int LevelID)
    {
        ulong _guid = GainTowerPos(batteryId);
        var _msg = NetManager.Instance.ApplyPromoteSkill(PlayerDataManager.Instance.ServerId,
       MonsterMiniature.CurActivityID,
       batteryId,
       _guid,
       LevelID);
        yield return _msg.SendAndWaitUntilDone();


        if (_msg.State == MessageState.Reply)
        {

            if (_msg.ErrorCode == (int)ErrorCodes.Error_MieShi_CanNotPromote)
            {
                //不在可提升阶段
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000027));

            }
            else if (_msg.ErrorCode == (int)ErrorCodes.Error_MieShi_BatteryDestory)
            {
                //炮台已摧毁
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000028));
            }
            else if (_msg.ErrorCode == (int)ErrorCodes.Error_MieShi_MaxSkillLvl)
            {
                //达到最高可提升的等级
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000032));
            }
            else if (_msg.ErrorCode == (int)ErrorCodes.ItemNotEnough)
            {
                //道具不足
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000030));
            }
            else if (_msg.ErrorCode == (int)ErrorCodes.DiamondNotEnough)
            {
                //钻石不足
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000031));
            }


            else if (_msg.ErrorCode == (int)ErrorCodes.OK)
            {
                var _battery = _msg.Response.battery;
                var _LV = Table.GetMieShiPublic(1);
                MonsterMiniature.isSurc = true;
                for (int i = 0; i < MonsterMiniature.MonsterTowers.Count; i++)
                {
                    MonsterTowerDataModel mtdm = MonsterMiniature.MonsterTowers[i];
                    if (mtdm.TowerId == _battery.batteryId)
                    {
                        mtdm.Level = _battery.skillLevel;
                        break;
                    }
                }

                MonsterTowerDataModel mt = MonsterMiniature.MonsterTowers[_battery.batteryId - 1];
                var _dateTime = DateTime.FromBinary((long)_battery.skillLvlEndTime);
                mt.SkillTime = _dateTime;

                mt.Level = _battery.skillLevel; ;
                mt.MyRanking = _msg.Response.contribute;
                mt.SkillDesc = Table.GetSkill(7499 + _battery.skillLevel).Desc;
                MonsterMiniature.MyGongxian = _msg.Response.contribute;
            }
        }
    }

    #endregion
    #region 请求炮台灭世数据
   
    private IEnumerator ApplyBatteryMsg()
    {

        var _msg = NetManager.Instance.ApplyBatteryData(PlayerDataManager.Instance.ServerId, MonsterMiniature.CurActivityID);
        yield return _msg.SendAndWaitUntilDone();
        if (_msg.State == MessageState.Reply)
        {
            if (_msg.ErrorCode == (int)ErrorCodes.OK)
            {
                Debug.Log("请求炮台信息成功");
                List<ActivityBatteryOne> _BatteryOne = _msg.Response.batterys;

                for (int j = 0; j < _BatteryOne.Count; j++)
                {

                    MonsterTowerDataModel _mt = MonsterMiniature.MonsterTowers[j];

                    // if (BatteryOne[j].curMaxHP <= BatteryOne[j].curMaxHP)
                    {
                        var _battery = _BatteryOne[j];
                        _mt.BloodCount = _battery.curMaxHP;
                        _mt.Level = _battery.skillLevel;
                        _mt.TowerId = _battery.batteryId;
                        var _dateTime = DateTime.FromBinary((long)_battery.skillLvlEndTime);
                        _mt.SkillTime = _dateTime;
                        _mt.BloodPer = _battery.promoteCount * 100 / (Table.GetMieShiPublic(1).MaxRaiseHP / 5);
                        _mt.BloodBizhi = (float)_mt.BloodPer / 500f;
                        _mt.SkillDesc = Table.GetSkill(7499 + _battery.skillLevel).Desc;
                    }
                }
                EventDispatcher.Instance.DispatchEvent(new MieShiRefreshTowers_Event());

            }
        }
    }
    #endregion
    float GetBloodstreamPer(ActivityBatteryOne battery)
    {
        if (battery.maxHP != 0)
        {
            return (float)battery.curMaxHP / (float)battery.maxHP * 100f;

        }
        return 0;
    }
    #region 灭世排名的固定奖励显示
    private void AddSetUpItem()
    {
        if (MonsterMiniature != null)
        {
            MonsterMiniature.GongxianList.Clear();
            for (int i = 0; ; i++)
            {
                DefendCityDevoteRewardRecord _dcrr = Table.GetDefendCityDevoteReward(i + 1);
                if (_dcrr == null)
                {
                    break;
                }
                GongxianJianliItem _jiangliItem = new GongxianJianliItem();
                if (i < 3)
                {
                    _jiangliItem.NubIcon = _dcrr.ContributionIcon.ToInt();
                }
                else
                {
                    _jiangliItem.Numb = string.Format("{0} - {1}", _dcrr.Rank[0].ToString(), _dcrr.Rank[_dcrr.Rank.Count - 1].ToString());
                }

                for (int j = 0; j < _dcrr.RankItemCount.Count; j++)
                {
                    if (_dcrr.RankItemID[j] > 0)
                    {
                        GongxianJianliItem.JiangliItem _item = new GongxianJianliItem.JiangliItem();

                        ItemBaseRecord _dbd = Table.GetItemBase(_dcrr.RankItemID[j]);
                        _item.IconId = _dcrr.RankItemID[j];
                        _item.Icon = _dbd.Icon;
                        _item.count = _dcrr.RankItemCount[j].ToString();
                        _jiangliItem.Rewards.Add(_item);
                    }

                }
                MonsterMiniature.GongxianList.Add(_jiangliItem);
            }
            MonsterMiniature.JifenList.Clear();
            for (int q = 0; ; q++)
            {
                DefendCityRewardRecord _dcrr2 = Table.GetDefendCityReward(q + 1);
                if (_dcrr2 == null)
                {
                    break;
                }
                GongxianJianliItem _jiangliItem = new GongxianJianliItem();
                if (q < 3)
                {
                    _jiangliItem.NubIcon = _dcrr2.RankIcon.ToInt();
                }
                else
                {
                    _jiangliItem.Numb = string.Format("{0} - {1}", _dcrr2.Rank[0].ToString(), _dcrr2.Rank[_dcrr2.Rank.Count - 1].ToString());
                }
                for (int k = 0; k < _dcrr2.RankItemCount.Count; k++)
                {
                    if (_dcrr2.RankItemID[k] > 0)
                    {
                        GongxianJianliItem.JiangliItem _item = new GongxianJianliItem.JiangliItem();
                        ItemBaseRecord _dbd = Table.GetItemBase(_dcrr2.RankItemID[k]);
                        _item.Icon = _dbd.Icon;
                        _item.IconId = _dcrr2.RankItemID[k];
                        _item.count = _dcrr2.RankItemCount[k].ToString();
                        _jiangliItem.Rewards.Add(_item);
                    }
                }
                MonsterMiniature.JifenList.Add(_jiangliItem);
            }
        }
    }
    #endregion
   
    #region 灭世排行数据
    public IEnumerator ApplyContriLevelMsg()
    {
        var _msg = NetManager.Instance.ApplyContriRankingData(PlayerDataManager.Instance.ServerId, MonsterMiniature.CurActivityID);
        yield return _msg.SendAndWaitUntilDone();
        if (_msg.State == MessageState.Reply)
        {
            if (_msg.ErrorCode == (int)ErrorCodes.OK)
            {
                var _id = SceneManager.Instance.CurrentSceneTypeId;
                var _tbScene = Table.GetScene(_id);
                GXCortributionRank_Event _gre = new GXCortributionRank_Event(_msg.Response);
                var _e = _gre;
                var _rankData = _e.RankData;
                var _rankList = _rankData.Datas;
                var _entrys = MonsterMiniature.FubenContributionRank.Entrys;
                MonsterMiniature.MyRanking = _msg.Response.MyRank;
                MonsterMiniature.MyGongxian = (_msg.Response.MyRank != 0 ? _msg.Response.Datas[_msg.Response.MyRank - 1].value : 0);
                MonsterMiniature.MyName = PlayerDataManager.Instance.GetName();
                MonsterMiniature.MyGongxianItem = GetMineCortributeAwardPropList(MonsterMiniature.MyRanking);
                _entrys.Clear();
                for (int i = 0, imax = _rankList.Count; i < imax; ++i)
                {

                    CortributionRankEntry cre = new CortributionRankEntry();
                    cre.Name = _rankList[i].name;
                    cre.Rank = _rankList[i].rank.ToString();
                    cre.Damage = _rankList[i].value.ToString();
                    cre.ItemList = GetMineCortributeAwardPropList(_rankList[i].rank);
                    DefendCityDevoteRewardRecord CortributionData = Table.GetDefendCityDevoteReward(GetIdCortributeByGrade(_rankList[i].rank));

                    cre.IconId = CortributionData.ContributionIcon.ToInt();

                    _entrys.Add(cre);
                }
            }
        }
    }
    #endregion
    #region 灭世贡献排行显示
    private void SetupCopyContributeInto(RankingInfoOne resData, CortributionRankEntry toData)
    {
        if (toData == null || resData == null)
        {
            return;
        }
        toData.Name = resData.name;
        toData.Rank = Convert.ToString(resData.rank);
        toData.Damage = Convert.ToString(resData.value);
        toData.ItemList = GetMineCortributeAwardPropList(resData.rank);
        toData.Show = true;
    }
    private GongxianJianliItem GetMineCortributeAwardPropList(int iMyRank)
    {
        GongxianJianliItem _MyCortributionRewardItemList = new GongxianJianliItem();
        int _iCortributionDataId = GetIdCortributeByGrade(iMyRank);
        if (_iCortributionDataId >= 0)
        {
            DefendCityDevoteRewardRecord _CortributionData = Table.GetDefendCityDevoteReward(_iCortributionDataId);
            if (_CortributionData != null)
            {
                _MyCortributionRewardItemList.NubIcon = _CortributionData.ContributionIcon.ToInt();

                for (int j = 0; j < _CortributionData.RankItemCount.Count; j++)
                {
                    if (_CortributionData.RankItemID[j] > 0)
                    {
                        GongxianJianliItem.JiangliItem _item = new GongxianJianliItem.JiangliItem();
                        ItemBaseRecord _dbd = Table.GetItemBase(_CortributionData.RankItemID[j]);
                        _item.IconId = _CortributionData.RankItemID[j];
                        _item.Icon = _dbd.Icon;
                        _item.count = _CortributionData.RankItemCount[j].ToString();

                        _MyCortributionRewardItemList.Rewards.Add(_item);
                    }

                }
            }
        }
        return _MyCortributionRewardItemList;
    }
    private int GetIdCortributeByGrade(int iRank)
    {
        int _idxCortribution = -1;
        for (int i = 1; ; i++)
        {
            DefendCityDevoteRewardRecord _CortributionData = Table.GetDefendCityDevoteReward(i);
            if (_CortributionData == null)
            {
                break;
            }

            if (_CortributionData.Rank[0] >= iRank && iRank <= _CortributionData.Rank[1])
            {
                _idxCortribution = i;
                break;
            }
        }
        return _idxCortribution;
    }
    
    #endregion


    #region 灭世结果
   
    public IEnumerator MieShiWarResult()
    {
        yield return null;
        //var msg = NetManager.Instance.ApplyPromoteHP(PlayerDataManager.Instance.ServerId, DataModule.CurMonsterFuben.activity.ActivityId);
        //yield return msg.SendAndWaitUntilDone();
        //if (msg.State == MessageState.Reply)
        //{

        //    if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_CanNotPromote)
        //    { }
        //}
    }
    #endregion
    private void SetupContributeGoIn(RankingInfoOne resData, BossRankEntry toData)
    {
        if (toData == null || resData == null)
        {
            return;
        }
        toData.Name = resData.name;
        toData.Rank = Convert.ToString(resData.value);
        toData.Damage = Convert.ToString(resData.value);
        toData.Show = true;
    }
    private void NotQualifiedContributeList()
    {
        var _entrys = MonsterMiniature.ContributionRank.Entrys;
        if (_entrys == null)
        {
            return;
        }
        for (int i = 0, imax = _entrys.Count; i < imax; ++i)
        {
            if (_entrys[i] != null)
            {
                _entrys[i].Show = false;
            }
        }
    }
    private void NotQualifiedCopyContributeList()
    {
        var _entrys = MonsterMiniature.FubenContributionRank.Entrys;
        if (_entrys == null)
        {
            return;
        }
        for (int i = 0, imax = _entrys.Count; i < imax; ++i)
        {
            if (_entrys[i] != null)
            {
                _entrys[i].Show = false;
            }
        }
    }

    #endregion


    #region 事件

    private void OnUpgradeButtonEvent(IEvent e)
    {

        BattryLevelUpUpLevelBtn_Event be = e as BattryLevelUpUpLevelBtn_Event;
        NetManager.Instance.StartCoroutine(RecoverSkilledness(be.ID, be.BtnID));

    }
    public void OnGetBatteryMsgEvent(IEvent e)
    {

        NetManager.Instance.StartCoroutine(ApplyBatteryMsg());

    }
    public void OnGXLevelButtonEvent(IEvent e)
    {
        //if (IsMonsterControll())
        //{
        NetManager.Instance.StartCoroutine(ApplyContriLevelMsg());
        AddSetUpItem();
        // }
    }
    //灭世战斗结果
    public void OnShowWarResultEvent(IEvent e)
    {

        NetManager.Instance.StartCoroutine(MieShiWarResult());

    }
    private void OnRenewalCortributeGradeEvent(IEvent ievent)
    {
        NotQualifiedContributeList();
        var _e = ievent as GXCortributionRank_Event;
        var _rankData = _e.RankData;
        var _rankList = _rankData.Datas;
        var _entrys = MonsterMiniature.FubenContributionRank.Entrys;
        _entrys.Clear();
        for (int i = 0, imax = _rankList.Count; i < imax; ++i)
        {
            CortributionRankEntry cre = new CortributionRankEntry();
            cre.Name = _rankList[i].name;
            cre.Rank = _rankList[i].rank.ToString();
            _entrys.Add(cre);
        }
    }
    private void OnRenewalCopyCortributeGradeEvent(IEvent ievent)
    {
        NotQualifiedCopyContributeList();
        var _e = ievent as FubenGXCortributionRank_Event;
        var _rankData = _e.RankData;
        var _rankList = _rankData.Datas;
        var _entrys = MonsterMiniature.FubenContributionRank.Entrys;

        _entrys.Clear();
        for (int i = 0, imax = _rankList.Count; i < imax; ++i)
        {

            CortributionRankEntry cre = new CortributionRankEntry();
            cre.Name = _rankList[i].name;
            cre.Rank = _rankList[i].rank.ToString();
            _entrys.Add(cre);
        }

    }
    private void OnPropChangeEvent(IEvent ievent)
    {
        var _e = ievent as UIEvent_BagChange;
        if (_e.HasType(eBagType.BaseItem))
        {
            SetPropValue();
        }
    }
    private void OnShowSkillnessTipEvent(IEvent e)
    {
        {
            var _arg = new SkillTipFrameArguments();
            _arg.idSkill = Table.GetBatteryLevel(MonsterMiniature.MonsterTowers[MonsterMiniature.MonsterTowerIdx].Level).BatterySkillId;
            var _skill = Table.GetBatteryLevel(MonsterMiniature.MonsterTowers[MonsterMiniature.MonsterTowerIdx].Level + 1);
            if (_skill != null)
            {
                _arg.idNextSkill = _skill.BatterySkillId;
            }
            else
            {
                _arg.idNextSkill = -1;
            }
            _arg.iLevel = MonsterMiniature.MonsterTowers[MonsterMiniature.MonsterTowerIdx].Level;



            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.SkillTipFrameUI, _arg));
        }
    }
    //升级事件的响应函数
    public void OnUpGradeEvent(IEvent ievent)
    {

    }

    //ExData初始化事件的响应函数
    public void OnInitializeExMsgEvent(IEvent ievent)
    {

    }

    public void OnExMsgUpMsgEvent(IEvent ievent)
    {
    }

    //副本次数发生变化的响应函数
    public void OnDungeonIntoCountUpEvent(IEvent ievent)
    {
    }
    public void OnMieShiDataEvent(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ApplyDownWorldCoroutine());
        AddSetUpItem();
    }
    public void OnApplyPortraitMsgEvent(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ApplyPersonDatumMsg());
    }
    private void OnApplyPortraitRewardEvent(IEvent ievent)
    {
        var _e = ievent as ApplyPortraitAward_Event;
        var _idNpc = _e.idNpc;
        NetManager.Instance.StartCoroutine(ApplyPeoplePrizeInfo(_idNpc));
    }

    #endregion 
}