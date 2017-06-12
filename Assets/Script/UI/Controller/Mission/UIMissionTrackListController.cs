#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class UIMissionTrackListController : IControllerBase
{
    public UIMissionTrackListController()
    {
        CleanUp();

        var DataModelListCount0 = DataModel.List.Count;
        for (var i = 0; i < DataModelListCount0; i++)
        {
            DataModel.List[i] = new MissionTrackItemDataModel
            {
                MissionId = -1,
                Title = "",
                Track = ""
            };
        }
        EventDispatcher.Instance.AddEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_DoMissionGoTo.EVENT_TYPE, OnEvent);
        EventDispatcher.Instance.AddEventListener(RefreshDungeonInfoEvent.EVENT_TYPE, RefreshDungeonInfo);
        EventDispatcher.Instance.AddEventListener(MissionTrackUpdateTimerEvent.EVENT_TYPE, OnMissionTrackUpdateTimer);
    }

    public MissionTrackListDataModel DataModel;
    public int nLastLogicId = -1;
    public string UpdateTimeFormatStr;
    public int UpdateTimeIdx;
    public int UpdateTimeType;

    public static int MissionCompare(MissionBaseModel a, MissionBaseModel b)
    {
        var v1 = a.MissionId;
        var v2 = b.MissionId;
        var s1 = a.Exdata[0];
        var s2 = b.Exdata[0];
        if (eMissionState.Finished == (eMissionState) s1)
        {
            v1 += 100000;
        }
        else if (eMissionState.Unfinished == (eMissionState) s1)
        {
            v1 += 1000000;
        }
        else if (eMissionState.Acceptable == (eMissionState) s1)
        {
            v1 += 10000000;
        }

        if (eMissionState.Finished == (eMissionState) s2)
        {
            v2 += 100000;
        }
        else if (eMissionState.Unfinished == (eMissionState) s2)
        {
            v2 += 1000000;
        }
        else if (eMissionState.Acceptable == (eMissionState) s2)
        {
            v2 += 10000000;
        }

        if (v1 > v2)
        {
            return 1;
        }
        if (v1 < v2)
        {
            return -1;
        }
        return a.MissionId - b.MissionId;
    }

    public void OnEvent(IEvent ievent)
    {
        var missionData = MissionManager.Instance.MissionData;
        var idx = 0;
        int[] ColArray = {100, 101, 102};

        //任务数据 填入3个列表
        var missionList = new List<MissionBaseModel>[3];
        for (var i = 0; i < missionList.Length; i++)
        {
            missionList[i] = new List<MissionBaseModel>();
        }
        {
            // foreach(var pair in missionData.Datas)
            var __enumerator2 = (missionData.Datas).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var pair = __enumerator2.Current;
                {
                    var m = pair.Value;
                    var table = Table.GetMissionBase(m.MissionId);
                    if (null == table)
                    {
                        continue;
                    }
                    var type = table.ViewType;
                    if (eMissionMainType.Circle == (eMissionMainType) type)
                    {
                        type = (int) eMissionMainType.Daily;
                    }
                    if (type >= 0 && type < 3)
                    {
                        missionList[type].Add(m);
                    }
                }
            }
        }

        //根据任务状态排序
        for (var i = 1; i < missionList.Length; i++)
        {
            missionList[i].Sort(MissionCompare);
        }

        //特殊处理环任务，如果当前正在做的环任务排在前面
        var tempList = missionList[2];
        if (tempList.Count > 0)
        {
            var id = MissionManager.Instance.CurrentDoingCircleMission;
            if (-1 != id)
            {
                for (var i = 0; i < tempList.Count; i++)
                {
                    if (id == tempList[i].MissionId)
                    {
                        var temp = tempList[i];
                        tempList.RemoveAt(i);
                        tempList.Insert(0, temp);
                    }
                }
            }
        }

        //取3个任务的第一，显示出来
        var DataModelListCount1 = DataModel.List.Count;

        //任务数据
        for (var i = 0; i < DataModelListCount1 && i < missionList.Length; i++)
        {
            DataModel.List[i].MissionId = -1;

            if (missionList[i].Count <= 0)
            {
                continue;
            }

            var mission = missionList[i][0];
            var id = mission.MissionId;
            var table = Table.GetMissionBase(id);

            var dataModel = DataModel.List[i];
            dataModel.MissionId = table.Id;
            dataModel.Title = table.Name;
            var state = (eMissionState) mission.Exdata[0];
            dataModel.state = (int)state;
            dataModel.Track = (eMissionState.Finished == state
                ? table.FinishDescription
                : table.TrackDescription) + MissionManager.MissionContent(table, mission.Exdata);

            dataModel.Col = GameUtils.GetTableColor(ColArray[i]);
        }

        /*
        var DataModelListCount1 =  DataModel.List.Count;

        for (int i = 0; i <DataModelListCount1; i++)
        {
            DataModel.List[i].MissionId = -1;

            eMissionMainType type = (eMissionMainType)i;

            {
                // foreach(var pair in missionData.Datas)
                var __enumerator1 = (missionData.Datas).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var pair = __enumerator1.Current;
                    {
                        var mission = pair.Value;
                        int id = mission.MissionId;
                        var table = Table.GetMissionBase(id);
                        if (type != (eMissionMainType)table.ViewType)
                            continue;

                        var dataModel = DataModel.List[idx++];
                        dataModel.MissionId = table.Id;
                        dataModel.Title = table.Name;
                        var state = (eMissionState)mission.Exdata[0];
                        dataModel.Track = (eMissionState.Finished == state
                            ? table.FinishDescription
                            : table.TrackDescription) + MissionManager.MissionContent(table, mission.Exdata);

                        if((int)type>=0 && (int)type<ColArray.Length)
                        {
                            dataModel.Col = GameUtils.GetTableColor(ColArray[(int)type]);
                        }

                        break;
                    }
                }
            }
        }
         *
         *
         */
    }

    public void OnMissionTrackUpdateTimer(IEvent ievent)
    {
        var now = Game.Instance.ServerTime;
        if (DataModel.TargetTime < now)
        {
            return;
        }
        var infoList = DataModel.FubenInfoList;
        var info = infoList[UpdateTimeIdx];
        var str = string.Format(UpdateTimeFormatStr, GameUtils.GetTimeDiffString(DataModel.TargetTime));
        if (UpdateTimeType == 0)
        {
            info.Title = str;
        }
        else
        {
            info.Track = str;
        }
    }

    public void RefreshDungeonInfo(IEvent ievent)
    {
        var e = ievent as RefreshDungeonInfoEvent;
        var fubenInfo = e.FubenInfo;
        var fubenInfoUnits = fubenInfo.Units;
        var infoList = DataModel.FubenInfoList;

        if (-1 == fubenInfo.LogicId)
        {
//这种就是发字典id和服务端发来的参数
            for (var j = 0; j < DataModel.FubenInfoList.Count; j++)
            {
                var data = DataModel.FubenInfoList[j];
                if (j >= fubenInfo.Units.Count)
                {
                    data.Title = "";
                    data.Track = "";
                    continue;
                }
                var unit = fubenInfo.Units[j];
                if (-1 == unit.Index)
                {
                    data.Title = "";
                    data.Track = "";
                    continue;
                }

                var temp = new List<object>();
                for (var idx = 0; idx < unit.Params.Count; idx++)
                {
                    temp.Add(unit.Params[idx]);
                }
                var str = string.Format(GameUtils.GetDictionaryText(unit.Index), temp.ToArray());
                data.Title = str;
                data.Track = "";
                data.InfoIdx = 0;
            }
            return;
        }

        var tbFubenLogic = Table.GetFubenLogic(fubenInfo.LogicId);
        if (tbFubenLogic == null)
        {
            return;
        }
        var now = Game.Instance.ServerTime;

        if (nLastLogicId != fubenInfo.LogicId)
        {
            nLastLogicId = fubenInfo.LogicId;
            if (DataModel.TargetTime >= now)
            {
                DataModel.TargetTime = now.AddYears(-10);
            }
        }

        //显示副本信息
        var i = 0;
        for (var imax = fubenInfoUnits.Count; i < imax; ++i)
        {
            var unit = fubenInfoUnits[i];
            var info = infoList[i];
            var type = (eFubenInfoType) tbFubenLogic.FubenInfo[i];
            var dicId = tbFubenLogic.FubenParam1[i];
            info.InfoIdx = unit.Index;
            info.Title = GameUtils.GetDictionaryText(dicId);
            switch (type)
            {
                case eFubenInfoType.KillMonster:
                {
                    info.Track = unit.Params[0] + "/" + tbFubenLogic.FubenParam2[i];
                }
                    break;
                case eFubenInfoType.Percent:
                {
                    var infoIdx = i;
                    var camp = ObjManager.Instance.MyPlayer.GetCamp();
                    if (camp == 5)
                    {
//红方，需要反过来显示
                        infoIdx = 1 - i;
                        unit = fubenInfoUnits[infoIdx];
                    }

                    var descStr = GameUtils.GetDictionaryText(tbFubenLogic.FubenParam2[infoIdx]);
                    info.Track = string.Format(descStr, unit.Params[0], unit.Params[1]);
                }
                    break;
                case eFubenInfoType.Score:
                case eFubenInfoType.PlayerCount:
                {
                    info.Track = unit.Params[0].ToString();
                }
                    break;
                case eFubenInfoType.BattleFieldScore: //寒霜据点的战场积分
                {
                    var infoIdx = i;
                    if (ObjManager.Instance.MyPlayer.GetCamp() == 4)
                    {
//蓝方，需要反过来显示
                        infoIdx = 1 - i;
                        unit = fubenInfoUnits[infoIdx];
                    }
                    info.Track = unit.Params[0] + "/" + tbFubenLogic.FubenParam2[infoIdx];
                }
                    break;
                case eFubenInfoType.StrongpointInfo: //据点信息
                {
                    if (info.Type != 1)
                    {
                        info.Type = 1;
                    }
                    for (int j = 0, jmax = info.States.Count; j < jmax; j++)
                    {
                        info.States[j] = unit.Params[j];
                    }
                }
                    break;
                case eFubenInfoType.Timer:
                {
                    if (DataModel.TargetTime >= now)
                    {
                        break;
                    }
                    UpdateTimeIdx = i;
                    UpdateTimeType = 1;
                    UpdateTimeFormatStr = "{0}";
                    var seconds = unit.Params[0]; //tbFubenLogic.FubenParam2[i];
                    DataModel.TargetTime = now.AddSeconds(seconds);
                    info.Track = GameUtils.GetTimeDiffString(DataModel.TargetTime);
                }
                    break;
                case eFubenInfoType.Timer2:
                {
                    info.Title = string.Empty;
                    info.Track = string.Empty;
                    var seconds = unit.Params[0]; //tbFubenLogic.FubenParam2[i];
                    EventDispatcher.Instance.DispatchEvent(new Event_MieShiStartCountDownData(seconds));
                }                

                    break;
                case eFubenInfoType.ShowDictionary0:
                {
                    var descStr = GameUtils.GetDictionaryText(tbFubenLogic.FubenParam2[i]);
                    info.Track = descStr;
                }
                    break;
                case eFubenInfoType.ShowDictionary1:
                {
                    var descStr = GameUtils.GetDictionaryText(tbFubenLogic.FubenParam2[i]);
                    info.Track = string.Format(descStr, unit.Params[0]);
                        if (i == 2)//取副本逻辑信息第三个是圣坛血量
                        {
                            DataModel.iChancelHpPer = unit.Params[0];
                            DataModel.ChanceName = info.Title;
                        }
                }
                    break;
                case eFubenInfoType.ShowDictionary2:
                {
                    var descStr = GameUtils.GetDictionaryText(tbFubenLogic.FubenParam2[i]);
                    info.Track = string.Format(descStr, unit.Params[0], unit.Params[1]);
                }
                    break;
                case eFubenInfoType.ShowDictionary3:
                {
                    var descStr = GameUtils.GetDictionaryText(tbFubenLogic.FubenParam2[i]);
                    info.Track = string.Format(descStr, unit.Params[0], unit.Params[1], unit.Params[2]);
                }
                    break;
                case eFubenInfoType.ShowDictionary6:
                    {
                        var descStr = GameUtils.GetDictionaryText(tbFubenLogic.FubenParam2[i]);
                        info.Track = string.Format(descStr, unit.Params[0], unit.Params[1], unit.Params[2], unit.Params[3], unit.Params[4], unit.Params[5]);
                        String[] NpcNameList = GameUtils.SplitString(info.Title, '\n');
                        for (int m = 0; m < DataModel.MonsterHpPerList.Count; m++)
                        {
                            DataModel.MonsterHpPerList[m] = unit.Params[m];
                            if (m < NpcNameList.Length)
                            {
                                DataModel.BatteryNameList[m] = NpcNameList[m];
                            }                            
                        }
                        if(unit.Params.Count==7)
                        {
                            DataModel.iPlayerCount = unit.Params[6];
                        }
                    }
                    break;
                case eFubenInfoType.AllianceWarInfo:
                {
                    info.Track = string.Empty;
                    var allianceId = unit.Params[0];
                    var count = unit.Params[1];
                    var dic = PlayerDataManager.Instance._battleCityDic;
                    if (i == 0)
                    {
                        if (!dic.ContainsKey(allianceId))
                        {
                            info.Title = string.Format(info.Title, GameUtils.GetDictionaryText(270024), count);
                        }
                        else
                        {
                            info.Title = string.Format(info.Title, dic[allianceId].Name, count);
                        }
                        continue;
                    }
                    if (!dic.ContainsKey(allianceId))
                    {
                        info.Title = string.Empty;
                        continue;
                    }
                    var title = string.Format(info.Title, dic[allianceId].Name, count);
                    var state = (eAllianceWarState) fubenInfoUnits[3].Params[0];
                    if (state == eAllianceWarState.ExtraTime)
                    {
                        UpdateTimeIdx = 0;
                        UpdateTimeType = 0;
                        UpdateTimeFormatStr = string.Empty;
                        DataModel.TargetTime = now.AddYears(-10);
                    }
                    else if (count >= 3)
                    {
                        title += GameUtils.GetDictionaryText(41013);
                        UpdateTimeIdx = i;
                        UpdateTimeType = 0;
                        UpdateTimeFormatStr = title;

                        var time = unit.Params[2];
                        var hour = time/10000;
                        var min = (time/100)%100;
                        var sec = time%100;
                        DataModel.TargetTime = new DateTime(now.Year, now.Month, now.Day, hour, min, sec);

                        title = string.Format(title, GameUtils.GetTimeDiffString(DataModel.TargetTime));
                    }
                    else
                    {
                        var j = 1;
                        for (; j < 3; j++)
                        {
                            if (fubenInfoUnits[j].Params[1] >= 3)
                            {
                                break;
                            }
                        }
                        if (j == 3)
                        {
                            DataModel.TargetTime = now.AddYears(-10);
                        }
                    }
                    info.Title = title;
                }
                    break;
                case eFubenInfoType.AllianceWarState:
                {
                    var state = (eAllianceWarState) unit.Params[0];
                    if (state == eAllianceWarState.ExtraTime)
                    {
                        if (DataModel.TargetTime >= now)
                        {
                            break;
                        }
                        var title = GameUtils.GetDictionaryText(41014);

                        UpdateTimeIdx = i;
                        UpdateTimeType = 0;
                        UpdateTimeFormatStr = title;

                        var time = unit.Params[1];
                        var hour = time/10000;
                        var min = (time/100)%100;
                        var sec = time%100;
                        DataModel.TargetTime = new DateTime(now.Year, now.Month, now.Day, hour, min, sec);
                        info.Title = title + string.Format(title, GameUtils.GetTimeDiffString(DataModel.TargetTime));
                    }
                    else
                    {
                        info.Title = string.Empty;
                    }
                    info.Track = string.Empty;
                }
                    break;
            }
        }
        for (var imax = infoList.Count; i < imax; ++i)
        {
            var info = infoList[i];
            info.InfoIdx = -1;
            info.Type = 0;
        }
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
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
        DataModel = new MissionTrackListDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
        if (-1 == sceneId)
        {
            return;
        }

        DataModel.TargetTime = Game.Instance.ServerTime.AddYears(-10);
        var tbScene = Table.GetScene(sceneId);
        if (null == tbScene)
        {
            return;
        }
        foreach (var info in DataModel.FubenInfoList)
        {
            for (int i = 0, imax = info.States.Count; i < imax; i++)
            {
                info.States[i] = -1;
            }
        }
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
    }

    public FrameState State { get; set; }
}