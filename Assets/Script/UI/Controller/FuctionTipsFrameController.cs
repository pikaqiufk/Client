#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class FuctionTipsFrameController : IControllerBase
{
    public FuctionTipsFrameController()
    {
        CleanUp();
    }

    private int currentShowTalentId;
    public FuctionDataModel DataModel;
    public int mEquipSkillDirtyMark;
    public bool skillChanged;



    public void CleanUp()
    {
        DataModel = new FuctionDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
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
        if (DataModel != null)
        {
            Table.ForeachFunctionOn((record) =>
            {
                var condition = GameUtils.CheckFuctionOnCondition(record.OpenLevel,record.TaskID,record.State);
                if (condition != 0)
                {
                    DataModel.FuctionCondition = record.FrameDesc;
                    DataModel.FuctionName = record.Name;
                    DataModel.FuctionDes = record.IconDesc;
                    DataModel.IconId = record.IconId;
                    if (GameUtils.CheckFuctionOnConditionByLevel(record.OpenLevel) != 0)
                    {
                        DataModel.ProgressType = 2;
                        DataModel.ProgressValue = PlayerDataManager.Instance.GetLevel();
                        if (DataModel.ProgressValue < 0)
                        {
                            DataModel.ProgressValue = 0;
                        }
                        DataModel.ProgressMaxVale = record.OpenLevel;
                    }
                    if (GameUtils.CheckFuctionOnConditionByMission(record.TaskID) != 0)
                    {
                        DataModel.ProgressType = 1;
                        int lastMissionOrder = GameUtils.GetMainMissionOrderByFunctionId(record.Id - 1);
                        DataModel.ProgressValue = Table.GetMissionBase(GameUtils.GetCurMainMissionId()).MissionBianHao - lastMissionOrder;
                        if (DataModel.ProgressValue < 0)
                        {
                            DataModel.ProgressValue = 0;
                        }
                        DataModel.ProgressMaxVale = Table.GetMissionBase(record.TaskID).MissionBianHao - lastMissionOrder;
                    }
                    return false;
                }
                return true;
            });
        }
    }

    

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}