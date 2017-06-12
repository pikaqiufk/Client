/********************************************************************************* 
                         Scorpion

  *FileName:FightSettlementFrameCtrler
  *Version:1.0
  *Date:2017-06-06
  *Description:
**********************************************************************************/  
#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class FightSettlementFrameCtrler : IControllerBase
{
    #region 静态变量
    #endregion

    #region 成员变量

    public BattleResultDataModel DataModel;

    #endregion

    #region 构造函数

    public FightSettlementFrameCtrler()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(BattleResultClick.EVENT_TYPE, OnFightResultTipEvent);
    }


    #endregion

    #region 固有函数

    public void CleanUp()
    {
        DataModel = new BattleResultDataModel();
    }

    public void OnShow()
    {

    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var _args = (BattleResultArguments)data;
        if (_args == null)
        {
            return;
        }
        DataModel.DungeonId = _args.DungeonId;
        var _tbFuben = Table.GetFuben(DataModel.DungeonId);
        if (_tbFuben == null)
        {
            DataModel.Exp = 0;
            DataModel.Gold = 0;
            DataModel.Honour = 0;
            return;
        }
        var _rewards = new List<ItemIdDataModel>();
        for (int i = 0, imax = _tbFuben.RewardId.Count; i < imax; i++)
        {
            var _itemId = _tbFuben.RewardId[i];
            var _itemCount = _tbFuben.RewardCount[i];
            if (_itemId == -1)
            {
                continue;
            }
            var _itemData = new ItemIdDataModel();
            _itemData.ItemId = _itemId;
            _itemData.Count = _itemCount;
            _rewards.Add(_itemData);
        }
        DataModel.Rewards = new ObservableCollection<ItemIdDataModel>(_rewards);

        DataModel.BattleResult = _args.BattleResult;
        DataModel.First = _args.First;

        //正常奖励
        var _todayCount = PlayerDataManager.Instance.GetExData(_tbFuben.TodayCountExdata);
        var _todayWinCount = PlayerDataManager.Instance.GetExData(_tbFuben.ResetExdata);
        var _extraCount = PlayerDataManager.Instance.GetExData(_tbFuben.TimeExdata);

        //额外奖励
        var _now = Game.Instance.ServerTime;
        var _begin = Table.GetClientConfig(282).ToInt();
        var _end = Table.GetClientConfig(283).ToInt();
        var _beginTime = new DateTime(_now.Year, _now.Month, _now.Day, _begin / 100, _begin % 100, 0);
        var _endTime = new DateTime(_now.Year, _now.Month, _now.Day, _end / 100, _end % 100, 0);
        var _hasExtraReward = _now >= _beginTime && _now <= _endTime;

        var _record = Table.GetSkillUpgrading(_tbFuben.ScanExp);
        if (_record != null)
        {
            DataModel.Exp = _record.GetSkillUpgradingValue(_todayCount);
            if (_hasExtraReward)
            {
                DataModel.Exp += _record.GetSkillUpgradingValue(_extraCount);
            }
        }
        else
        {
            DataModel.Exp = 0;
        }

        _record = Table.GetSkillUpgrading(_tbFuben.ScanGold);
        if (_record != null)
        {
            DataModel.Gold = _record.GetSkillUpgradingValue(_todayCount);
            if (_hasExtraReward)
            {
                DataModel.Gold += _record.GetSkillUpgradingValue(_extraCount);
            }
        }
        else
        {
            DataModel.Gold = 0;
        }

        if (DataModel.BattleResult == 1)
        {
            _record = Table.GetSkillUpgrading(_tbFuben.ScanReward[0]);
            if (_record != null)
            {
                DataModel.Honour = _record.GetSkillUpgradingValue(_todayWinCount);
            }
            else
            {
                DataModel.Honour = 0;
            }
        }
        else
        {
            _record = Table.GetSkillUpgrading(_tbFuben.ScanReward[1]);
            if (_record != null)
            {
                DataModel.Honour = _record.GetSkillUpgradingValue(_todayCount);
            }
            else
            {
                DataModel.Honour = 0;
            }
        }
        _record = Table.GetSkillUpgrading(_tbFuben.ScanReward[0]);
        if (_hasExtraReward && _record != null)
        {
            DataModel.Honour += _record.GetSkillUpgradingValue(_extraCount);
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
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

    private IEnumerator OutFightCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var _msg = NetManager.Instance.ExitDungeon(-1);
            yield return _msg.SendAndWaitUntilDone();
            if (_msg.State == MessageState.Reply)
            {
                if (_msg.ErrorCode == (int)ErrorCodes.OK)
                {
                }
                else
                {
                    Logger.Error(".....AcceptBattleAward.......{0}.", _msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....AcceptBattleAward.......{0}.", _msg.State);
            }
        }
    }

    #endregion

    #region 事件

    public void OnFightResultTipEvent(IEvent ievent)
    {
        //var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        //var tbScene = Table.GetScene(sceneId);
        //if (tbScene != null)
        //{
        //    var tbFuben = Table.GetFuben(tbScene.FubenId);
        //    if (tbFuben != null)
        //    {

        //    }
        //}
        NetManager.Instance.StartCoroutine(OutFightCoroutine());
    }

    #endregion
}