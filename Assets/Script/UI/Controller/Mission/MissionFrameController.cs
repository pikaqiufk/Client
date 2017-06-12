#region using 

using System;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using Random = UnityEngine.Random;

#endregion

public class TaskFrameCtrler : IControllerBase
{
	public TaskFrameCtrler()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(UIEvent_UpdateCurrentMission.EVENT_TYPE, UpdateCurrentMission);
    }

    public MissionContentDataModel DataModel = new MissionContentDataModel();

    public void UpdateCurrentMission(IEvent ievent)
    {
        var evn = ievent as UIEvent_UpdateCurrentMission;

        var currentMission = MissionManager.Instance.CurrentMissionData;
        var tableData = MissionManager.Instance.CurrentMissionTableData;
        var npcId = MissionManager.Instance.NpcId;

        var tableCharacter = Table.GetCharacterBase(npcId);
        if (null != tableCharacter)
        {
            var tableNpc = Table.GetNpcBase(tableCharacter.ExdataId);
            DataModel.NpcName = tableNpc.Name;
            DataModel.NpcDataId = npcId;
        }
        else
        {
            DataModel.NpcName = "";
            DataModel.NpcDataId = -1;
        }

        DataModel.ShowReward = false;
        var MissionContentDataModelRewardItemCount0 = MissionContentDataModel.RewardItemCount;
        for (var i = 0; i < MissionContentDataModelRewardItemCount0; i++)
        {
            DataModel.RewardItem[i].ItemId = -1;
        }

        //无任务，显示对话和服务
        if (null == currentMission || null == tableData)
        {
            DataModel.MissionId = -1;
            var tableNpc = Table.GetNpcBase(tableCharacter.ExdataId);
            if (null != tableNpc)
            {
                //显示对话
                DataModel.MissionDialogContent = tableNpc.pop[Random.Range(0, tableNpc.pop.Length)];

                //显示NPC服务
                for (var i = 0; i < tableNpc.Service.Length; i++)
                {
                    var serviceId = tableNpc.Service[i];
                    if (-1 != serviceId)
                    {
                        DataModel.ServiceId[i] = serviceId;
                        DataModel.ShowService[i] = true;
                        DataModel.ServiceName[i] = "[u]" + Table.GetService(serviceId).Name + "[/u]";
                    }
                    else
                    {
                        DataModel.ShowService[i] = false;
                        DataModel.ServiceId[i] = serviceId;
                    }
                }
                //任务声音
                var soundId = tableNpc.DialogSound;
                if (-1 != soundId)
                {
                    SoundManager.Instance.StopSoundEffect(ObjNPC.LastNpcSoundId);
                    if (!SoundManager.Instance.IsPlaying(soundId))
                    {
                        var isPlayingNpcSound = Table.GetClientConfig(1204);
                        if (int.Parse(isPlayingNpcSound.Value) == 1)
                        {
                            if (!SoundManager.Instance.IsPlaying(ObjNPC.MissionSoundId))
                            {
                                SoundManager.Instance.PlaySoundEffect(soundId);
                                ObjNPC.LastSoundTime = DateTime.Now;
                                ObjNPC.LastNpcSoundId = soundId;
                            }
                        }
                    }
                }
            }
        }
        else
        {
//显示任务

            //关闭NPC服务
            for (var i = 0; i < DataModel.ShowService.Count; i++)
            {
                DataModel.ShowService[i] = false;
                DataModel.ServiceId[i] = -1;
            }

            DataModel.MissionId = tableData.Id;

            const int ExpItemId = 1;

            var state = (eMissionState) currentMission.Exdata[0];
            if (eMissionState.Finished == state)
            {
                var playerLevel = Math.Max(1, PlayerDataManager.Instance.GetLevel());

                DataModel.MissionDialogContent = GameUtils.GetDictionaryText(tableData.DialogueFinish);
                //data.BtnName = Table.GetDictionary(Resource.Dictrionary.ClaimReward).Desc[1];
                DataModel.BtnStr = GameUtils.GetDictionaryText(1031);

                //重置
                for (var i = 0; i < DataModel.RewardItem.Count; i++)
                {
                    DataModel.RewardItem[i].ItemId = -1;
                }

                var roleId = PlayerDataManager.Instance.GetRoleId();
                var start = 0;

                //职业奖励
                for (var i = 0; i < 2; i++)
                {
                    if (tableData.RoleRewardId[roleId, i] != -1)
                    {
                        DataModel.RewardItem[start].ItemId = tableData.RoleRewardId[roleId, i];
                        DataModel.RewardItem[start].Count = tableData.RoleRewardCount[roleId, i];
                        start++;
                    }
                }

                //等级系数经验奖励
                if (0 != tableData.IsDynamicExp)
                {
                    var expCount = GameUtils.CalculateExpByLevel(tableData.DynamicExpRatio, playerLevel);
                    DataModel.RewardItem[start].ItemId = ExpItemId;
                    DataModel.RewardItem[start].Count = expCount;
                    start++;
                }

                //普通任务奖励
                var DataModelRewardItemCount1 = DataModel.RewardItem.Count - start;
                for (var i = 0; i < DataModelRewardItemCount1; i++)
                {
                    if (start >= DataModel.RewardItem.Count)
                    {
                        Logger.Debug("DataModel.RewardItem[{0}] out of index", start);
                        break;
                    }
                    if (-1 == tableData.RewardItem[i])
                    {
                        continue;
                    }
                    DataModel.RewardItem[start].ItemId = tableData.RewardItem[i];
                    DataModel.RewardItem[start].Count = SkillExtension.ModifyByLevel(tableData.RewardItemCount[i],
                        playerLevel, 100000000);

                    start++;
                }

                DataModel.ShowReward = true;

                //任务声音
                var soundId = tableData.DeliveryTaskMusic;
                if (-1 != soundId)
                {
                    SoundManager.Instance.StopSoundEffect(ObjNPC.LastNpcSoundId);

                    if (!SoundManager.Instance.IsPlaying(soundId))
                    {
                        var isPlayingNpcSound = Table.GetClientConfig(1204);
                        if (int.Parse(isPlayingNpcSound.Value) == 1)
                        {
                            SoundManager.Instance.StopSoundEffect(ObjNPC.MissionSoundId);
                            SoundManager.Instance.PlaySoundEffect(soundId);
                            ObjNPC.LastSoundTime = DateTime.Now;
                            ObjNPC.MissionSoundId = soundId;
                        }
                    }
                }
            }
            else if (eMissionState.Acceptable == state)
            {
                DataModel.MissionDialogContent = GameUtils.GetDictionaryText(tableData.DialogueNpc);
                //data.BtnName = Table.GetDictionary(Resource.Dictrionary.ClaimReward).Desc[1];
                DataModel.BtnStr = GameUtils.GetDictionaryText(1032);

                //任务声音
                var soundId = tableData.AcceptTaskMusic;
                if (-1 != soundId)
                {
                    SoundManager.Instance.StopSoundEffect(ObjNPC.LastNpcSoundId);
                    if (!SoundManager.Instance.IsPlaying(soundId))
                    {
                        var isPlayingNpcSound = Table.GetClientConfig(1204);
                        if (int.Parse(isPlayingNpcSound.Value) == 1)
                        {
                            SoundManager.Instance.StopSoundEffect(ObjNPC.MissionSoundId);
                            SoundManager.Instance.PlaySoundEffect(soundId);
                            ObjNPC.LastSoundTime = DateTime.Now;
                            ObjNPC.MissionSoundId = soundId;    
                        }
                    }
                }
            }
        }
    }

    public void Close()
    {
        MissionManager.Instance.NpcId = -1;
        DataModel.NpcDataId = -1;
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

    public FrameState State { get; set; }
}