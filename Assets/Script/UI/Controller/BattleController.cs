/********************************************************************************* 
                         Scorpion

  *FileName:VersusFrameCtrler
  *Version:1.0
  *Date:2017-06-06
  *Description:
**********************************************************************************/  

#region using
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

#endregion

public class VersusFrameCtrler : IControllerBase
{

    #region 静态变量
    #endregion

    #region 成员变量

    private BattleDataModel m_DataModel;
    private bool m_bIsInit;

    #endregion

    #region 构造函数

    public VersusFrameCtrler()
    {
        CleanUp();
        //通用事件
        //EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, OnFlagInit);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnMarkRenwalEvent);
        //EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnInitExData);

        //排队相关
        EventDispatcher.Instance.AddEventListener(BattleQueueEvent.EVENT_TYPE, OnFightQueueBuildEvent);
        EventDispatcher.Instance.AddEventListener(QueneUpdateEvent.EVENT_TYPE, OnRenewalQueueEvent);
        //操作相关
        EventDispatcher.Instance.AddEventListener(DungeonEnterCountUpdate.EVENT_TYPE, OnPitEnterCountRenwalEvent);
        EventDispatcher.Instance.AddEventListener(BattleCellClick.EVENT_TYPE, OnFightCellClickEvent);
        EventDispatcher.Instance.AddEventListener(BattleOperateEvent.EVENT_TYPE, OnFightOperateEvent);
    }

    #endregion


    #region  固有函数
    public void CleanUp()
    {
        m_DataModel = new BattleDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var _args = data as BattleArguments;

        Initialize();
        AnalyseGiveNoticeBattle();
        if (_args != null)
        {
            SetChooseCell(_args.Tab);
        }
        else
        {
            SetChooseCell(0);
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return m_DataModel;
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

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }

    #endregion

    #region 逻辑函数

   private QueueUpDataModel QueueInformation
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.QueueUpData; }
    }

    private IEnumerator AdopttBattleDecideCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var _dungeonId = m_DataModel.DungeonId;
            var _msg = NetManager.Instance.AcceptBattleAward(_dungeonId);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    m_DataModel.HasAccept = true;
                    m_DataModel.SelectCell.ShowNotice = false;
                    var _tbFuben = Table.GetFuben(_dungeonId);
                    if (_tbFuben != null)
                    {
                        PlayerDataManager.Instance.SetFlag(_tbFuben.FlagId);
                        
                    }

                    RefreshRemuneration();
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                    Logger.Error(".....AcceptBattleAward.......{0}.", _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....AcceptBattleAward.......{0}.", _msg.State);
            }
        }
    }

    //--------------------------------------------------------------Notice--------

    private void AnalyseGiveNoticeBattle()
    {
        var _show = false;
        var _c = m_DataModel.BattleCells.Count;
        for (var i = 0; i < _c; i++)
        {
            var _cell = m_DataModel.BattleCells[i];
            if (_cell == null)
            {
                continue;
            }
            var _flag = false;
            var _record = Table.GetPVPBattle(_cell.Id);
            for (var j = 0; j < _record.Fuben.Length; j++)
            {
                var _tbFuben = Table.GetFuben(_record.Fuben[j]);
                if (_tbFuben != null)
                {
                    var _condition = PlayerDataManager.Instance.CheckCondition(_tbFuben.EnterConditionId);
                    if (_condition == 0)
                    {
                        var _exdataCount = PlayerDataManager.Instance.GetExData(_tbFuben.TodayCountExdata);
                        if (_exdataCount == 0)
                        {
                            _flag = true;
                            _show = true;
                            break;
                        }
                        if (!PlayerDataManager.Instance.GetFlag(_tbFuben.FlagId))
                        {
                            _flag = true;
                            _show = true;
                            break;
                        }
                    }
                }
            }
            _cell.ShowNotice = _flag;
        }
        
    }


    private void Initialize()
    {
        m_bIsInit = true;
        var _flag = 0;
        var _cellCount = 0;
        var _list = new List<BattleCellData>();
        Table.ForeachPVPBattle((record =>
        {
            var _cell = new BattleCellData();
            _cell.Id = record.Id;
            _cell.IsSelect = false;
            _cell.Index = _flag;
            _flag++;
            _list.Add(_cell);
            // var dungeondId = record.Fuben[0];
            //if (dungeondId != -1)
            //{
            //    var tbDungeon = Table.GetFuben(dungeondId);
            //    if (tbDungeon != null)
            //    {
            //        cell.FlagReceive = tbDungeon.FlagId;
            //        bool recive = PlayerDataManager.Instance.GetFlag(cell.FlagReceive);
            //        if (!recive)
            //        {
            //            cell.ShowNotice = true;
            //        }
            //        else
            //        {
            //            cell.ShowNotice = false;
            //        }
            //    }
            //}
	        return true;
        }));
        m_DataModel.BattleCells = new ObservableCollection<BattleCellData>(_list);
    }

    private void OnAdoptBattleDecide()
    {
        NetManager.Instance.StartCoroutine(AdopttBattleDecideCoroutine());
    }
 
    private void OnCallOffQueue()
    {
        NetManager.Instance.StartCoroutine(QueueGetOnCancelCoroutine());
    }

    //public void OnInitExData(IEvent ievent)
    //{
    //    if (mIsInit == false)
    //    {
    //        Init();
    //    }
    //    RefreshReward();
    //}
   

    private void OnBeginQueue()
    {
        if (PlayerDataManager.Instance.IsInPvPScnen())
        {
            GameUtils.ShowHintTip(456);
            return;
        }

        var _tbScene = Table.GetScene(m_DataModel.DungeonId);
        if (_tbScene == null)
        {
            return;
        }
        var _level = PlayerDataManager.Instance.GetLevel();
        if (_tbScene.LevelLimit > _level)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300102));
            return;
        }
        var _teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;

        if (_teamData != null)
        {
            if (_teamData.TeamList[0].Guid != 0 && _teamData.TeamList[0].Guid != ObjManager.Instance.MyPlayer.GetObjId())
            {
//只有队长才能进行此操作
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(440));
                return;
            }

            //检查其他人的等级
        }

        var _isEnter = PlayerDataManager.Instance.CheckDungeonEnter(m_DataModel.DungeonId);
        if (!_isEnter)
        {
            return;
        }

        //如果在排其它的队
        var _tbDungeon = Table.GetFuben(m_DataModel.DungeonId);
        var _queueUpData = PlayerDataManager.Instance.PlayerDataModel.QueueUpData;
        if (_queueUpData.QueueId != -1 && _queueUpData.QueueId != _tbDungeon.QueueParam)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 41004, "", () =>
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                NetManager.Instance.StartCoroutine(QueueGetOnCoroutine());
            });
            return;
        }
        NetManager.Instance.StartCoroutine(QueueGetOnCoroutine());
    }

    private IEnumerator QueueGetOnCancelCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var _dungeonId = m_DataModel.DungeonId;
            var _tbDungeon = Table.GetFuben(_dungeonId);
            var _msg = NetManager.Instance.MatchingCancel(_tbDungeon.QueueParam);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    QueueInformation.QueueId = -1;
                    m_DataModel.IsQueue = false;
                    m_DataModel.StateType = 0;
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(m_DataModel.StartTime,
                        QueueInformation.QueueId));
                    EventDispatcher.Instance.DispatchEvent(new QueueCanceledEvent());
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                    Logger.Error(".....MatchingCancel.......{0}.", _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....MatchingCancel.......{0}.", _msg.State);
            }
        }
    }

    private IEnumerator QueueGetOnCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var _tbDungeon = Table.GetFuben(m_DataModel.DungeonId);
            var _msg = NetManager.Instance.MatchingStart(_tbDungeon.QueueParam);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.InitQueneData(_msg.Response.Info);
                    m_DataModel.IsQueue = true;
                    m_DataModel.StartTime = QueueInformation.StartTime;
                    m_DataModel.ExpectTime = QueueInformation.StartTime.AddSeconds(QueueInformation.ExpectScend);
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Error_FubenID)
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Error_QueueCountMax)
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveQueue)
                {
                    var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Unline)
                {
                    //有队友不在线
                    var _e = new ShowUIHintBoard(448);
                    EventDispatcher.Instance.DispatchEvent(_e);
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Error_FubenCountNotEnough)
                {
                    //{0}副本次数不够
                    var _charId = _msg.Response.CharacterId[0];
                    var _name = PlayerDataManager.Instance.GetTeamMemberName(_charId);
                    if (!string.IsNullOrEmpty(_name))
                    {
                        var _str = GameUtils.GetDictionaryText(466);
                        _str = string.Format(_str, _name);
                        var _e = new ShowUIHintBoard(_str);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                    else
                    {
                        var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    //{{0}道具不足
                    var _charId = _msg.Response.CharacterId[0];
                    var _name = PlayerDataManager.Instance.GetTeamMemberName(_charId);
                    if (!string.IsNullOrEmpty(_name))
                    {
                        var _str = GameUtils.GetDictionaryText(467);
                        _str = string.Format(_str, _name);
                        var _e = new ShowUIHintBoard(_str);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                    else
                    {
                        var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                }
                else if (_msg.ErrorCode == (int) ErrorCodes.Error_LevelNoEnough)
                {
                    //{{0}不符合副本条件
                    var _charId = _msg.Response.CharacterId[0];
                    var _name = PlayerDataManager.Instance.GetTeamMemberName(_charId);
                    if (!string.IsNullOrEmpty(_name))
                    {
                        var _str = GameUtils.GetDictionaryText(468);
                        _str = string.Format(_str, _name);
                        var _e = new ShowUIHintBoard(_str);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                    else
                    {
                        var _e = new ShowUIHintBoard(_msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(_e);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(_msg.ErrorCode);
                    Logger.Error(".....MatchingStart.......{0}........", _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Warn(".....MatchingStart.......{0}.", _msg.State);
            }
        }
    }

    private void RefreshRemuneration()
    {
        var _cell = m_DataModel.SelectCell;
        if (_cell == null)
        {
            return;
        }
        var _tbFuben = Table.GetFuben(m_DataModel.DungeonId);
        if (_tbFuben == null)
        {
            return;
        }
        var _todayCount = PlayerDataManager.Instance.GetExData(_tbFuben.TodayCountExdata);
        var _todayWinCount = PlayerDataManager.Instance.GetExData(_tbFuben.ResetExdata);
        var _extraCount = PlayerDataManager.Instance.GetExData(_tbFuben.TimeExdata);
        m_DataModel.PlayedCount = _todayCount;
        m_DataModel.ShowFirst = _tbFuben.IsDyncReward != 1 || _todayWinCount == 0 || m_DataModel.ShowAccpet;
        var _rewards = m_DataModel.Rewards;
        if (m_DataModel.ShowFirst)
        {
            for (int j = 0, jmax = _tbFuben.RewardId.Count; j < jmax; ++j)
            {
                var _reward = _rewards[j];
                _reward.ItemId = _tbFuben.RewardId[j];
                _reward.Count = _tbFuben.RewardCount[j];
            }
        }
        else
        {
            //正常奖励
            var _rewardIdx = 0;
            var _record = Table.GetSkillUpgrading(_tbFuben.ScanReward[0]);
            if (_record != null)
            {
                var _reward = _rewards[_rewardIdx];
                _reward.ItemId = (int) eResourcesType.Honor;
                _reward.Count = _record.GetSkillUpgradingValue(_todayWinCount);
                if (_reward.Count > 0)
                {
                    ++_rewardIdx;
                }
            }
            _record = Table.GetSkillUpgrading(_tbFuben.ScanExp);
            if (_record != null)
            {
                var _reward = _rewards[_rewardIdx];
                _reward.ItemId = (int) eResourcesType.ExpRes;
                _reward.Count = _record.GetSkillUpgradingValue(_todayCount);
                if (_reward.Count > 0)
                {
                    ++_rewardIdx;
                }
            }
            _record = Table.GetSkillUpgrading(_tbFuben.ScanGold);
            if (_record != null)
            {
                var _reward = _rewards[_rewardIdx++];
                _reward.ItemId = (int) eResourcesType.GoldRes;
                _reward.Count = _record.GetSkillUpgradingValue(_todayCount);
                if (_reward.Count > 0)
                {
                    ++_rewardIdx;
                }
            }
            for (var _imax = _rewards.Count; _rewardIdx < _imax; _rewardIdx++)
            {
                _rewards[_rewardIdx++].ItemId = -1;
            }
        }

        //额外奖励
        {
            var _rewardIdx = 0;
            var _extraRewards = m_DataModel.ExtraRewards;
            var _record = Table.GetSkillUpgrading(_tbFuben.ScanReward[0]);
            if (_record != null)
            {
                var _reward = _extraRewards[_rewardIdx];
                _reward.ItemId = (int) eResourcesType.Honor;
                _reward.Count = _record.GetSkillUpgradingValue(_extraCount);
                if (_reward.Count > 0)
                {
                    ++_rewardIdx;
                }
            }
            _record = Table.GetSkillUpgrading(_tbFuben.ScanExp);
            if (_record != null)
            {
                var _reward = _extraRewards[_rewardIdx];
                _reward.ItemId = (int) eResourcesType.ExpRes;
                _reward.Count = _record.GetSkillUpgradingValue(_extraCount);
                if (_reward.Count > 0)
                {
                    ++_rewardIdx;
                }
            }
            _record = Table.GetSkillUpgrading(_tbFuben.ScanGold);
            if (_record != null)
            {
                var _reward = _extraRewards[_rewardIdx];
                _reward.ItemId = (int) eResourcesType.GoldRes;
                _reward.Count = _record.GetSkillUpgradingValue(_extraCount);
                if (_reward.Count > 0)
                {
                    ++_rewardIdx;
                }
            }
            for (var _imax = _extraRewards.Count; _rewardIdx < _imax; _rewardIdx++)
            {
                _extraRewards[_rewardIdx++].ItemId = -1;
            }
        }
    }

    private void SetChooseCell(int index)
    {
        if (index < 0 || index >= m_DataModel.BattleCells.Count)
        {
            return;
        }

        foreach (var battleCell in m_DataModel.BattleCells)
        {
            battleCell.IsSelect = battleCell.Index == index;
        }

        var _cellData = m_DataModel.BattleCells[index];
        m_DataModel.SelectCell = _cellData;

        var _battleId = _cellData.Id;
        var _tbPvpBattle = Table.GetPVPBattle(_battleId);
        var _playerCount = 0;
        var _playerData = PlayerDataManager.Instance;

        FubenRecord tbDungeon = null;
        for (var i = _tbPvpBattle.Fuben.Length - 1; i >= 0; i--)
        {
            var _dungeonId = _tbPvpBattle.Fuben[i];
            if (_dungeonId != -1)
            {
                tbDungeon = Table.GetFuben(_dungeonId);
                if (tbDungeon == null)
                {
                    continue;
                }
                if (_playerData.CheckCondition(tbDungeon.EnterConditionId) == 0 || i == 0)
                {
                    m_DataModel.DungeonId = _dungeonId;
                    var _tbFuben = Table.GetFuben(_dungeonId);
                    if (_tbFuben != null)
                    {
                        var _tbQueue = Table.GetQueue(_tbFuben.QueueParam);
                        _playerCount = _tbQueue.CountLimit;

                        m_DataModel.SceneId = _tbFuben.SceneId;
                    }
                    break;
                }
            }
        }

        m_DataModel.PlayerCount = _playerCount;
        m_DataModel.CanAccept = PlayerDataManager.Instance.GetFlag(tbDungeon.ScriptId);
        m_DataModel.HasAccept = PlayerDataManager.Instance.GetFlag(tbDungeon.FlagId);

        RenewalQueueMsg();
        RenewalStateType();
        RefreshRemuneration();
    }

    private void RenewalQueueMsg()
    {
        if (QueueInformation.QueueId == -1)
        {
            m_DataModel.IsQueue = false;
            return;
        }
        var _tbPvpBattle = Table.GetPVPBattle(m_DataModel.SelectCell.Index);
        if (_tbPvpBattle == null)
        {
            return;
        }

        for (var i = _tbPvpBattle.Fuben.Length - 1; i >= 0; i--)
        {
            var _tbDungeon = Table.GetFuben(_tbPvpBattle.Fuben[i]);
            if (_tbDungeon != null)
            {
                if (QueueInformation.QueueId == _tbDungeon.QueueParam)
                {
                    m_DataModel.IsQueue = true;
                    m_DataModel.StartTime = QueueInformation.StartTime;
                    m_DataModel.ExpectTime = QueueInformation.StartTime.AddSeconds(QueueInformation.ExpectScend);
                    return;
                }
            }
        }
        m_DataModel.IsQueue = false;
    }

    private void RenewalStateType()
    {
        if (m_DataModel.IsQueue)
        {
            m_DataModel.StateType = 1;
        }
        else
        {
            if (GameLogic.Instance && GameLogic.Instance.Scene)
            {
                var _sceneId = GameLogic.Instance.Scene.SceneTypeId;
                var _tbScene = Table.GetScene(_sceneId);
                if (_tbScene != null)
                {
                    if (_tbScene.FubenId == m_DataModel.DungeonId)
                    {
                        m_DataModel.StateType = 2;
                        return;
                    }
                }
            }
            m_DataModel.StateType = 0;
        }
    }

  

    #endregion


    #region 事件

    private void OnFightQueueBuildEvent(IEvent ievent)
    {
        if (m_DataModel.DungeonId != -1)
        {
            OnCallOffQueue();
        }
    }

    //public void OnFlagInit(IEvent ievent)
    //{
    //    if (mIsInit == false)
    //    {
    //        Init();
    //    }
    //    AnalyseNoticeBattle();
    //}
    private void OnRenewalQueueEvent(IEvent ievent)
    {
        RenewalQueueMsg();
        RenewalStateType();
    }

    private void OnFightCellClickEvent(IEvent ievent)
    {
        var _e = ievent as BattleCellClick;
        var _index = _e.Index;

        SetChooseCell(_index);
    }

    private void OnFightOperateEvent(IEvent ievent)
    {
        var _e = ievent as BattleOperateEvent;
        switch (_e.Type)
        {
            case 0:
                {
                    if (!m_DataModel.IsQueue)
                    {
                        OnBeginQueue();
                    }
                    else
                    {
                        OnCallOffQueue();
                    }
                }
                break;
            case 1:
                {
                    OnAdoptBattleDecide();
                }
                break;
        }
    }
    private void OnPitEnterCountRenwalEvent(IEvent ievent)
    {
        RefreshRemuneration();
    }

    private void OnMarkRenwalEvent(IEvent ievent)
    {
        var _e = ievent as FlagUpdateEvent;
        var _c = m_DataModel.BattleCells.Count;
        for (var i = 0; i < _c; i++)
        {
            var _cell = m_DataModel.BattleCells[i];
            if (_cell.FlagReceive == _e.Index)
            {
                AnalyseGiveNoticeBattle();
                return;
            }
        }
    }
    #endregion

}