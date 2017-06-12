#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using Shared;
using ClientService;
using ScorpionNetLib;
using UnityEngine;
using System.Linq;

#endregion

public class ClimbingTowerRewardController : IControllerBase
{
    public ClimbingTowerRewardController()
    {
        CleanUp();
    }

    public TowerResultDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new TowerResultDataModel();
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
    public void RefreshResultData(IEvent ievent)
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as BlockLayerArguments;
        if (args == null)
        {
            return;
        }
        if (args.Type == (int) eDungeonCompleteType.Success)
        {
            var cur = PlayerDataManager.Instance.GetExData((int) eExdataDefine.e623);
            var max = PlayerDataManager.Instance.GetExData((int) eExdataDefine.e621);
            DataModel.AwardItems.Clear();
            DataModel.OnceRewards.Clear();
            var tbTower = Table.GetClimbingTower(cur);
            if (tbTower != null)
            {
                for (int i = 0; i < tbTower.RewardList.Count&&i<tbTower.NumList.Count; i++)
                {
                    var bagItemData = new ItemIdDataModel();
                    bagItemData.ItemId = tbTower.RewardList[i];
                    bagItemData.Count = tbTower.NumList[i];
                    DataModel.AwardItems.Add(bagItemData);
                }
                if (max == cur)
                {
                    for (int i = 0; i < tbTower.OnceRewardList.Count && i < tbTower.OnceNumList.Count; i++)
                    {
                        var bagItemData = new ItemIdDataModel();
                        bagItemData.ItemId= tbTower.OnceRewardList[i];
                        bagItemData.Count = tbTower.OnceNumList[i];
                        DataModel.OnceRewards.Add(bagItemData);
                    }
                }
                DataModel.bWin = args.Type == (int) eDungeonCompleteType.Success;
                DataModel.bFirst = max == cur;
            }
        }
    }
}