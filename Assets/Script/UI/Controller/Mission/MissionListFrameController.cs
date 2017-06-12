#region using 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using DataContract;
using DataTable;
using EventSystem;

#endregion

public class TaskListCtrler : IControllerBase
{
	public TaskListCtrler()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent);
        EventDispatcher.Instance.AddEventListener(Event_ShowMissionDataDetail.EVENT_TYPE, OnShowMissionDataDetailEvent);
        EventDispatcher.Instance.AddEventListener(Event_MissionList_AutoNext.EVENT_TYPE, AutoNext);
        EventDispatcher.Instance.AddEventListener(Event_MissionList_TapIndex.EVENT_TYPE, SelectIndex);
    }

    public MissionListDataModel DataModel;
    public int ShowDetailMissionId = -1;

    public void AutoNext(IEvent ievent)
    {
        var table = Table.GetMissionBase(DataModel.MissionId);
        var type = (eMissionMainType) table.ViewType;
        var state = MissionManager.Instance.GetMissionState(DataModel.MissionId);
        if (eMissionMainType.Daily == type ||
            eMissionMainType.Circle == type)
        {
            if (eMissionState.Finished == state)
            {
                MissionManager.Instance.CommitMission(DataModel.MissionId);
            }
            else
            {
                MissionManager.Instance.GoToMissionPlace(DataModel.MissionId);
            }
        }
        else if (eMissionMainType.SubStoryLine == type && -1 == table.NpcStart && eMissionState.Acceptable == state)
        {
            MissionManager.Instance.AcceptMission(DataModel.MissionId);
        }
        else if (eMissionMainType.SubStoryLine == type && -1 == table.FinishNpcId && eMissionState.Finished == state)
        {
            MissionManager.Instance.CommitMission(DataModel.MissionId);
        }
        else
        {
            MissionManager.Instance.GoToMissionPlace(DataModel.MissionId);
        }
        if (!PlayerDataManager.Instance.GetFlag(517))
        {
            var list = new Int32Array();
            list.Items.Add(517);
            PlayerDataManager.Instance.SetFlagNet(list);
        }
    }

    public MissionListItemSimpleDataModel CreateMissionDataModel(MissionBaseModel mis,
                                                                 MissionListItemSimpleDataModel data = null)
    {
        //任务id和数据
        var missionId = mis.MissionId;
        var missionData = mis;

        //查表
        var tableData = Table.GetMissionBase(missionId);
        if (null == tableData)
        {
            return null;
        }

        //new个数据源
        if (null == data)
        {
            data = new MissionListItemSimpleDataModel();
        }


        //任务数据
        data.MissionId = tableData.Id;
        data.MissionName = tableData.Name;
        data.State = missionData.Exdata[0];
        data.Type = tableData.ViewType;
        //data.MissionDesc = tableData.TrackDescription + MissionManager.MissionContent(tableData, missionData.Exdata);
        var state = (eMissionState) data.State;
        ////跟据任务状态设置按钮文字

        if (eMissionState.Unfinished == state)
        {
            //data.BtnName = Table.GetDictionary(Resource.Dictrionary.GoToMission).Desc[1];
            data.BtnName = GameUtils.GetDictionaryText(522);
        }
        else if (eMissionState.Finished == state)
        {
            //data.BtnName = Table.GetDictionary(Resource.Dictrionary.ClaimReward).Desc[1];
            data.BtnName = GameUtils.GetDictionaryText(523);
        }
        else if (eMissionState.Acceptable == state)
        {
            //data.BtnName = Table.GetDictionary(Resource.Dictrionary.ClaimReward).Desc[1];
            data.BtnName = GameUtils.GetDictionaryText(524);
        }


        return data;
    }

    public void LookMission(int missionId)
    {
        if (missionId == -1)
        {
            return;
        }
        {
            // foreach(var dataModel in DataModel.SelectMission)
            var __enumerator3 = (DataModel.SelectMission).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var dataModel = __enumerator3.Current;
                {
                    if (dataModel.MissionId == missionId)
                    {
                        dataModel.Selected = 1;
                    }
                    else
                    {
                        dataModel.Selected = 0;
                    }
                }
            }
        }
        var tableData = Table.GetMissionBase(missionId);
        if (tableData == null)
        {
            return;
        }
        var missionData = MissionManager.Instance.GetMissionById(missionId);
        if (missionData == null)
        {
            return;
        }

        //任务数据
        DataModel.MissionId = tableData.Id;
        DataModel.MissionName = tableData.Name;
        DataModel.MissionDesc = tableData.TrackDescription +
                                MissionManager.MissionContent(tableData, missionData.Exdata);
        DataModel.MissionDetail = GameUtils.GetDictionaryText(tableData.Desc);
        var state = (eMissionState) missionData.Exdata[0];

        //跟据任务状态设置按钮文字
        if (eMissionState.Unfinished == state)
        {
            //data.BtnName = Table.GetDictionary(Resource.Dictrionary.GoToMission).Desc[1];
            DataModel.BtnName = GameUtils.GetDictionaryText(525);
        }
        else if (eMissionState.Finished == state)
        {
            //data.BtnName = Table.GetDictionary(Resource.Dictrionary.ClaimReward).Desc[1];
            DataModel.BtnName = GameUtils.GetDictionaryText(526);
        }
        else if (eMissionState.Acceptable == state)
        {
            //data.BtnName = Table.GetDictionary(Resource.Dictrionary.ClaimReward).Desc[1];
            DataModel.BtnName = GameUtils.GetDictionaryText(527);
        }

        MissinReward(missionId, DataModel.Rewards);
        //MissionManager.Instance.GetCurrentMissionId()
    }

    public static void MissinReward(int missionId, ReadonlyObjectList<ItemIdDataModel> dataModel)
    {
        var tableData = Table.GetMissionBase(missionId);
        if (null == tableData)
        {
            return;
        }
        //重置
        for (var i = 0; i < dataModel.Count; i++)
        {
            dataModel[i].ItemId = -1;
            dataModel[i].Count = -1;
        }

        //物品id
        const int expItemId = 1;

        var playerLevel = Math.Max(1, PlayerDataManager.Instance.GetLevel());
        var roleId = PlayerDataManager.Instance.GetRoleId();
        var start = 0;

        //职业奖励
        for (var i = 0; i < 2; i++)
        {
            if (tableData.RoleRewardId[roleId, i] != -1)
            {
                dataModel[start].ItemId = tableData.RoleRewardId[roleId, i];
                dataModel[start].Count = tableData.RoleRewardCount[roleId, i];
                start++;
            }
        }

        //等级系数经验奖励
        if (0 != tableData.IsDynamicExp)
        {
            var expCount = GameUtils.CalculateExpByLevel(tableData.DynamicExpRatio, playerLevel);
            dataModel[start].ItemId = expItemId;
            dataModel[start].Count = expCount;
            start++;
        }

        //普通任务奖励
        for (var i = 0; i < tableData.RewardItem.Length && start < dataModel.Count; i++)
        {
            if (-1 == tableData.RewardItem[i])
            {
                continue;
            }

            dataModel[start].ItemId = tableData.RewardItem[i];
            dataModel[start].Count = SkillExtension.ModifyByLevel(tableData.RewardItemCount[i], playerLevel, 100000000);
            start++;
        }

        //普通任务奖励
        /*
        var DataModelRewardItemCount1 = dataModel.Count - start;
        for (int i = 0; i < DataModelRewardItemCount1; i++)
        {
            if (start > dataModel.Count)
            {
                Logger.Debug("DataModel.RewardItem[{0}] out of index", start);
                
                break;
            }
            if (-1 == tableData.RewardItem[i])
            {
               
                continue;
            }
            dataModel[start].ItemId = tableData.RewardItem[i];
            dataModel[start].Count = tableData.RewardItemCount[i];
            start++;
        }
		*/
    }

    public static int MissionSort(MissionListItemSimpleDataModel a, MissionListItemSimpleDataModel b)
    {
        var v1 = 0;
        var v2 = 0;
        if (eMissionState.Finished == (eMissionState) a.State)
        {
            v1 += 100000;
        }
        else if (eMissionState.Unfinished == (eMissionState) a.State)
        {
            v1 += 1000000;
        }
        else if (eMissionState.Acceptable == (eMissionState) a.State)
        {
            v1 += 10000000;
        }
        
        try
        {
            v1 += Table.GetMissionBase(a.MissionId).MissionBianHao;
        }
        catch
        { }

        if (eMissionState.Finished == (eMissionState) b.State)
        {
            v2 += 100000;
        }
        else if (eMissionState.Unfinished == (eMissionState) b.State)
        {
            v2 += 1000000;
        }
        else if (eMissionState.Acceptable == (eMissionState) b.State)
        {
            v2 += 10000000;
        }
        try
        {
            v2 += Table.GetMissionBase(b.MissionId).MissionBianHao;
        }
        catch
        { }

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
        var e = ievent as Event_UpdateMissionData;
        if (null == e)
        {
            return;
        }

        var mission = MissionManager.Instance.MissionData;
        if (null == mission)
        {
            return;
        }

        //变化的任务id，如果是-1说明是全部任务
        var missionId = e.Id;

        //是否只处理当前任务
        var processed = false;

        if (-1 != missionId)
        {
            foreach (var data in DataModel.SelectMission)
            {
                if (data.MissionId != missionId)
                {
                    continue;
                }

                var missions = MissionManager.Instance.MissionData.Datas;
                foreach (var pair in missions)
                {
                    if (pair.Value.MissionId != data.MissionId)
                    {
                        continue;
                    }
                    if (1 == pair.Value.Exdata[0])
                    {
                        break;
                    }

                    //只处理变化的任务
                    CreateMissionDataModel(pair.Value, data);
                    if (data.MissionId == DataModel.MissionId)
                    {
                        LookMission(data.MissionId);
                    }
                    processed = true;
                    break;
                }
                break;
            }
        }

        if (!processed)
        {
            SelectTap(DataModel.MissionIndex);
        }


        //ResetMissionDatamodel(DataModel.MissionIndex);
        OnMissionDataChange();
    }

    private void OnMissionDataChange()
    {
        var count = 0;
        var missions = MissionManager.Instance.MissionData;
        {
            // foreach(var mission in missions.Datas)
            var __enumerator4 = (missions.Datas).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var mission = __enumerator4.Current;
                {
                    var missionData = mission.Value;
                    var table = Table.GetMissionBase(missionData.MissionId);
                    if (null == table)
                    {
                        continue;
                    }
                    if (eMissionMainType.Daily != (eMissionMainType) table.ViewType &&
                        eMissionMainType.Circle != (eMissionMainType) table.ViewType)
                    {
                        continue;
                    }
                    if (eMissionState.Finished != (eMissionState) missionData.Exdata[0])
                    {
                        continue;
                    }
                    count++;
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.DailyMissionCount = count;
    }

    private void OnShowMissionDataDetailEvent(IEvent ievent)
    {
        var evn = ievent as Event_ShowMissionDataDetail;
        var missionId = evn.Id;
        LookMission(missionId);
    }

    public void ResetMissionDatamodel(int index)
    {
        var missions = MissionManager.Instance.MissionData.Datas;

        if (1 == index)
        {
            //主线任务
            foreach (var pair in missions)
            {
                var tableData = Table.GetMissionBase(pair.Value.MissionId);
                if (null == tableData)
                {
                    continue;
                }

                var type = (eMissionMainType) tableData.ViewType;

                if (eMissionMainType.MainStoryLine != type)
                {
                    continue;
                }

                var data = CreateMissionDataModel(pair.Value);
                DataModel.StoryLineMission.Clear();
                DataModel.StoryLineMission.Add(data);
                return;
            }
        }
        else if (2 == index || 3 == index)
        {
            var temp = new List<MissionListItemSimpleDataModel>();
            foreach (var pair in missions)
            {
                var mission = pair.Value;
                var tableData = Table.GetMissionBase(mission.MissionId);
                if (null == tableData)
                {
                    continue;
                }
                var type = (eMissionMainType) tableData.ViewType;


                if (index == 2)
                {
                    if (eMissionMainType.SubStoryLine != type)
                    {
                        continue;
                    }
                }
                else if (index == 3)
                {
                    if (eMissionMainType.Daily != type && eMissionMainType.Circle != type)
                    {
                        continue;
                    }
                }

                var data = CreateMissionDataModel(pair.Value);
                temp.Add(data);
            }
            temp.Sort(MissionSort);
            DataModel.SelectMission = new ObservableCollection<MissionListItemSimpleDataModel>(temp);
        }

        /*
        if (index < 0 || index > 3) return;
        DataModel.SelectMission.Clear();
        List<KeyValuePair<MissionBaseModel, int>> sortTemp = new List<KeyValuePair<MissionBaseModel, int>>();
        //排序任务
        var mission = MissionManager.Instance.MissionData;
        {
            // foreach(var pair in mission.Datas)
            var __enumerator1 = (mission.Datas).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var pair = __enumerator1.Current;
                {
                    //任务id和数据
                    int missionId = pair.Key;
                    var missionData = pair.Value;

                    //查表
                    var tableData = Table.GetMissionBase(missionId);
                    if (null == tableData) continue;

                    //排序列表
                    eMissionState state = (eMissionState)missionData.Exdata[0];
                    int sortValue = 0;
                    if (eMissionState.Unfinished == state)
                    {
                        sortValue = 1001;
                    }
                    else if (eMissionState.Finished == state)
                    {
                        sortValue = 1000;
                    }
                    else if (eMissionState.Acceptable == state)
                    {
                        sortValue = 1002;
                    }
                    //根据任务类型归类
                    eMissionMainType type = (eMissionMainType)tableData.ViewType;
                    if (eMissionMainType.MainStoryLine == type && index == 1)
                    {
                        DataModel.StoryLineMission.Clear();
                        var data = CreateMissionDataModel(missionData);
                        if (data != null)
                        {
                            DataModel.StoryLineMission.Add(data);
                        }
                        return;
                        //sortTemp.Add(new KeyValuePair<MissionBaseModel, int>(missionData, sortValue));
                    }
					else if ((eMissionMainType.Daily == type || eMissionMainType.Circle == type) && index == 3)
                    {
                        sortTemp.Add(new KeyValuePair<MissionBaseModel, int>(missionData, sortValue));
                    }
                    else if (eMissionMainType.SubStoryLine == type && index == 2)
                    {
                        sortTemp.Add(new KeyValuePair<MissionBaseModel, int>(missionData, sortValue));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        ObservableCollection<MissionListItemSimpleDataModel> missionList = DataModel.SelectMission;
        sortTemp.Sort((a, b) => a.Value - b.Value);
        {
            var __list2 = sortTemp;
            var __listCount2 = __list2.Count;
            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var valuePair = (KeyValuePair<MissionBaseModel, int>)__list2[__i2];
                {
                    //new个数据源
                    var data = CreateMissionDataModel(valuePair.Key);
                    if (data != null)
                    {
                        missionList.Add(data);
                    }
                }
            }
        }
		 * 
		 */
    }

    public void SelectIndex(IEvent ievent)
    {
        var evn = ievent as Event_MissionList_TapIndex;
        if (evn == null)
        {
            return;
        }
        SelectTap(evn.nIndex);
    }

    public void SelectTap(int index, int loolkMissionId = -1)
    {
        var missionId = loolkMissionId;
        switch (index)
        {
            case 1:
            {
                DataModel.MissionIndex = index;
                ResetMissionDatamodel(index);
                if (DataModel.StoryLineMission.Count < 1)
                {
                    DataModel.NowLookCount = 0;
                    return;
                }
                DataModel.NowLookCount = 1;
                if (-1 == loolkMissionId)
                {
                    missionId = DataModel.StoryLineMission[0].MissionId;
                }
            }
                break;
            case 2:
            {
                DataModel.MissionIndex = index;
                ResetMissionDatamodel(index);
                if (DataModel.SelectMission.Count < 1)
                {
                    DataModel.NowLookCount = 0;
                    return;
                }
                DataModel.NowLookCount = 1;
                if (-1 == loolkMissionId)
                {
                    missionId = DataModel.SelectMission[0].MissionId;
                }
            }
                break;
            case 3:
            {
                DataModel.MissionIndex = index;
                ResetMissionDatamodel(index);
                if (DataModel.SelectMission.Count < 1)
                {
                    DataModel.NowLookCount = 0;
                    return;
                }
                DataModel.NowLookCount = 1;
                if (-1 == loolkMissionId)
                {
                    missionId = DataModel.SelectMission[0].MissionId;
                }
            }
                break;
        }

        LookMission(missionId);
    }

    public void CleanUp()
    {
        DataModel = new MissionListDataModel();
        DataModel.MissionIndex = 1;
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

    public void Close()
    {
        //DataModel.MissionIndex = 1;
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        if (null == data)
        {
            return;
        }

        var arg = data as MissionListArguments;

        if (null == arg)
        {
            return;
        }

        var idx = arg.Tab;
        var mId = arg.MissionId;
        if (idx > 0 && idx <= 3)
        {
            DataModel.MissionIndex = idx;
        }
	    if (arg.IsFromMisson)
	    {
		    int type2Count = 0;
		    int type3Count = 0;
			var missions = MissionManager.Instance.MissionData.Datas;
			foreach (var miss in missions)
		    {
				var tableData = Table.GetMissionBase(miss.Value.MissionId);
                if (null == tableData)
                {
                    continue;
                }
				var type = (eMissionMainType)tableData.ViewType;

				if (eMissionMainType.SubStoryLine == type)
				{
					type2Count++;
				}
				else if (eMissionMainType.Daily == type && eMissionMainType.Circle == type)
				{
					type3Count++;
				}
		    }

		    if (2 == idx)
		    {
			    if (type2Count <= 0)
			    {
				    DataModel.MissionIndex = 3;
			    }
		    }
			else if (3 == idx)
			{
				if (type3Count <= 0)
				{
					DataModel.MissionIndex = 2;
				}
			}
	    }
        SelectTap(DataModel.MissionIndex, mId);
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}