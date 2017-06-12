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
using Shared;
using ScorpionNetLib;

#endregion

public class ClimbingTowerController : IControllerBase
{
    public ClimbingTowerController()
    {
        EventDispatcher.Instance.AddEventListener(TowerFloorClickEvent.EVENT_TYPE, OnClickCell);
        EventDispatcher.Instance.AddEventListener(TowerBtnClickEvent.EVENT_TYPE, OnTowerEvent);

        CleanUp();
    }

    public ClimbingTowerDataModel DataModel;
    public void CleanUp()
    {
        DataModel = new ClimbingTowerDataModel();
    }



    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
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

    public void RefreshData(UIInitArguments data)
    {
        
        InitLeft();
        InitReward(DataModel.NextFloor);
    }

    public void InitLeft()
    {
        int max = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e621);
        int cur = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e623);
        int sweep = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e622);
        DataModel.CanSweep = max > cur + 10;
        DataModel.CurFloor = cur;
        DataModel.MaxFloor = max;
        DataModel.SelectIdx = DataModel.NextFloor;

        
        int _maxId = cur + 1;
        for (int id = cur + 1; id < cur + 2 + 1; id++)
        {
            var tb = Table.GetClimbingTower(id);
            if (tb == null)
                break;
            _maxId = id;
        }
        int _minId = _maxId - 4;

        if (_minId < 1)
        {
            _minId = 1;
            _maxId = _minId + 4;
        }
        

        for (int id =_minId,i=0; id < _maxId; id++,i++)
        {
            var tb = Table.GetClimbingTower(id);
            if (tb == null)
                continue;
            DataModel.FloorList[i].nIndex = id ;
            DataModel.FloorList[i].bSelect = id == cur+1;
            DataModel.FloorList[i].bSweep = max > (id - 1);
            DataModel.FloorList[i].strName = id.ToString();
        }
    }
    public void OnClickCell(IEvent ievent)
    {
        TowerFloorClickEvent e = ievent as TowerFloorClickEvent;
        if (e == null)
            return;
        int nIndex = e.nIndex - 1;
        if (nIndex >= DataModel.FloorList.Count)
            return;

        if (DataModel.SelectIdx-1 < DataModel.FloorList.Count)
        {
            DataModel.FloorList[DataModel.SelectIdx-1].bSelect = false;
        }
        DataModel.FloorList[nIndex].bSelect = true;
        DataModel.SelectIdx = e.nIndex;
        InitReward(e.nIndex);
    }

    private void InitReward(int id)
    {
        var tbTower = Table.GetClimbingTower(id);
        if (tbTower == null)
            return;
        var tbFuben = Table.GetFuben(tbTower.FubenId);
        if (tbFuben == null)
            return;
        DataModel.FightPoint = tbFuben.FightPoint;
        DataModel.AwardItems.Clear();
        for (int i = 0; i < tbTower.RewardList.Count && i < tbTower.NumList.Count; i++)
        {
            BagItemDataModel item = new BagItemDataModel();
            item.ItemId = tbTower.RewardList[i];
            item.Count = tbTower.NumList[i];
            DataModel.AwardItems.Add(item);
        }
        
        DataModel.OnceRewards.Clear();
        for (int i = 0; i < tbTower.OnceRewardList.Count && i < tbTower.OnceNumList.Count; i++)
        {
            BagItemDataModel item = new BagItemDataModel();
            item.ItemId = tbTower.OnceRewardList[i];
            item.Count = tbTower.OnceNumList[i];
            DataModel.OnceRewards.Add(item);
        }
    }

    public void OnTowerEvent(IEvent ievent)
    {
        TowerBtnClickEvent e = ievent as TowerBtnClickEvent;
        if (e == null)
            return;
        if (e.nType == 0)
        {
            var tb = Table.GetClimbingTower(DataModel.NextFloor);
            if (tb != null)
            {
                GameUtils.EnterFuben(tb.FubenId);
            }
        }
        else
        {//扫荡

        }
    }

    public IEnumerator OnSweepTower()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TowerSweep(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {

                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error(".....OnSweepTower.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Warn(".....OnSweepTower.......{0}.", msg.State);
            }
        }
    }
}