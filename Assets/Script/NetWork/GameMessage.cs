#region using

using System;
using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    public void ActivityBroadcast(int placeholder)
    {
        throw new NotImplementedException();
    }

    private IEnumerator DelayAchievement(int achievementId)
    {
        yield return new WaitForSeconds(5f);
        EventDispatcher.Instance.DispatchEvent(new Event_AchievementTip(achievementId));
        //         var a1 = Table_Tamplet.Convert_Int(Table.GetClientConfig(562).Value);
        //         if (PlayerDataManager.Instance.GetFlag(a1))
        // 		{
        // 			EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.AchievementTip));
        // 		}
    }

    public void DoNpcServeice(int npcId,ulong objId, int serveiceId)
    {
		StartCoroutine(DoNpcServiceCoroutinue(npcId,objId, serveiceId));
    }

    public IEnumerator DoNpcServiceCoroutinue(int npcId,ulong objId, int serveiceId)
    {
        var obj = ObjManager.Instance.FindObjById(objId);
        if (null == obj)
        {
			ObjManager.Instance.SelectObj((tempObj) =>
			{
				if (tempObj.GetObjType() == OBJ.TYPE.NPC && tempObj.GetDataId() == npcId)
				{
					objId = tempObj.GetObjId();
					obj = tempObj;
					return false;
				}
				return true;
			});
	        if (null == obj)
	        {
				Logger.Warn("obj[{0}] is null", objId);
				yield break;    
	        }
        }
        if (obj.GetObjType() != OBJ.TYPE.NPC)
        {
            Logger.Warn("obj[{0}] is not NPC", objId);
            yield break;
        }

        var msg = Instance.NpcService(objId, serveiceId);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State != MessageState.Reply)
        {
            Logger.Debug("DoNpcServiceCoroutinue:MessageState.Timeout");
            yield break;
        }

        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            Logger.Debug("DoNpcServiceCoroutinue error=[{0}]", msg.ErrorCode);
            yield break;
        }

        var serviceType = (NpcServeType) Table.GetService(serveiceId).Type;
        switch (serviceType)
        {
            case NpcServeType.Repair:
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(425));
                EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(0));
            }
                break;
            case NpcServeType.Treat:
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(428));
            }
                break;
        }
    }

    public void Kick(ulong characterId)
    {
        NeedReconnet = false;
        Game.Instance.ExitToLogin();
    }

    private IEnumerator PickUpItemCoroutine(ulong objId)
    {
        //Logger.Debug("PickUpItemCoroutine({0})--------------", objId);

        var obj = ObjManager.Instance.FindObjById(objId);
        if (null == obj)
        {
            Logger.Debug("obj[{0}] is null", objId); //新增一种可以所有人拾取的，所以null是有可能的
            yield break;
        }
        if (obj.GetObjType() != OBJ.TYPE.DROPITEM)
        {
            Logger.Warn("obj[{0}] is not DROPITEM", objId);
            yield break;
        }
        //var dropItem = obj as ObjDropItem;
        var msg = Instance.PickUpItem(objId);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State != MessageState.Reply)
        {
            Logger.Warn("NetSyncPostion:MessageState.Timeout");
			ObjManager.Instance.RemoveObj(objId);
            yield break;
        }


        if (msg.ErrorCode == (int) ErrorCodes.OK)
        {
			yield break;
        }
        else if (msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
        {
            //var e = new ShowUIHintBoard(302);
            //EventDispatcher.Instance.DispatchEvent(e);
        }
        else if (msg.ErrorCode == (int) ErrorCodes.Error_NoObj)
        {
            Logger.Warn("msg.ErrorCode == (int)ErrorCodes.Error_NoObj");
			//ObjManager.Instance.RemoveObj(objId);
        }
        else if (msg.ErrorCode == (int) ErrorCodes.Error_NotTheOwner)
        {
            Logger.Warn("msg.ErrorCode == (int)ErrorCodes.Error_NotTheOwner");
        }
        else
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            Logger.Error("PickUpItemCoroutine error=[{0}]", msg.ErrorCode);
        }
		ObjManager.Instance.RemoveObj(objId);
    }

    public void SendPickUpItemRequest(ulong objId)
    {
        StartCoroutine(PickUpItemCoroutine(objId));
    }

    public void BagisFull(ulong dropItemId, int itemId, int itemCount)
    {
        var e = new ShowUIHintBoard(302);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void NotifyBattleReminder(int type, string info, int param)
    {
        if (info.Length > 0 && (info[0] == '#' || info[0] == '^'))
        {
            info = GameUtils.ServerStringAnalysis(info);
        }
        else
        {
            info = GameUtils.ConvertChatContent(info);
        }

        var e = new ShowUIHintBoard(info, type, param >> 1);
        EventDispatcher.Instance.DispatchEvent(e);

        var addChat = (param & 1) != 0;
        if (addChat)
        {
            var e1 = new ChatMainHelpMeesage(info);
            EventDispatcher.Instance.DispatchEvent(e1);                    
        }
        //EventDispatcher.Instance.DispatchEvent(new ShowPopTalk_Event(info));
    }

    //队员希望邀请
    public void MemberWantInvite(int type, string memberName, string toName, ulong toId)
    {
        var noticeData = PlayerDataManager.Instance.NoticeData;
        if (noticeData.TeamOpenFlag == false)
        {
//功能未开放，不处理队伍消息
            return;
        }
        switch (type)
        {
            case 1: //被characterId邀请
            {
                //判断是否是自动接受申请
                var controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
                if (controllerBase == null)
                {
                    return;
                }
                var myModel = controllerBase.GetDataModel("") as TeamDataModel;

                if (myModel.AutoAccept)
                {
                    var e = new UIEvent_OperationList_AcceptInvite(toId, toId);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(220113), memberName), "",
                    () =>
                    {
                        var e = new UIEvent_OperationList_AcceptInvite(toId, toId);
                        EventDispatcher.Instance.DispatchEvent(e);
                    },
                    () =>
                    {
                        var e = new UIEvent_OperationList_RefuseInvite(toId, toId);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }, false, true);
            }
                break;
            case 2: //队员推荐人加入队伍
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(220114), memberName, toName), "",
                    () =>
                    {
                        var e = new Event_TeamInvitePlayer(toId);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }, null, false, true);
            }
                break;
            case 3: //characterId加入了队伍
            {
                EventDispatcher.Instance.DispatchEvent(
                    new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220101), memberName)));
                var e = new UIEvent_TeamFrame_Message(type, 0, toId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case 4: //characterId想要加入队伍，是否同意
            {
                var controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
                if (controllerBase == null)
                {
                    return;
                }
                var myModel = controllerBase.GetDataModel("") as TeamDataModel;

                if (myModel.AutoJoin)
                {
                    var e = new Event_TeamAcceptJoin(toId);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                    string.Format(GameUtils.GetDictionaryText(220120), memberName), "",
                    () =>
                    {
//同意申请
                        var e = new Event_TeamAcceptJoin(toId);
                        EventDispatcher.Instance.DispatchEvent(e);
                    },
                    () =>
                    {
//不同意申请
                        var e = new Event_TeamRefuseJoin(toId);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }, false, true);
            }
                break;
            case 5: //characterId退出了队伍
            {
                EventDispatcher.Instance.DispatchEvent(
                    new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220102), memberName)));
                var e = new UIEvent_TeamFrame_Message(type, 0, toId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case 8: //队伍中的characterId 下线了
            {
                var controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
                if (controllerBase == null)
                {
                    return;
                }
                controllerBase.CallFromOtherClass("SetLeaveState", new object[2] {true, toId});
                EventDispatcher.Instance.DispatchEvent(
                    new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220123), memberName)));
            }
                break;
            case 9: //队伍中的characterId 上线了
            {
                var controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
                if (controllerBase == null)
                {
                    return;
                }
                controllerBase.CallFromOtherClass("SetLeaveState", new object[2] {false, toId});
                EventDispatcher.Instance.DispatchEvent(
                    new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220122), memberName)));
            }
                break;
            case 11: //有人拒绝了你的邀请
            {
                EventDispatcher.Instance.DispatchEvent(
                    new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220116), memberName)));
            }
                break;
            case 14: //有了新队长
            {
                EventDispatcher.Instance.DispatchEvent(
                    new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220110), memberName)));
                var e = new UIEvent_TeamFrame_Message(type, 0, toId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
        }
    }

    //组队信息
    public void NotifyTeamMessage(int type, ulong teamId, ulong characterId)
    {
        var noticeData = PlayerDataManager.Instance.NoticeData;
        if (noticeData.TeamOpenFlag == false)
        {
//功能未开放，不处理队伍消息
            return;
        }

        switch (type)
        {
            case 2: //characterId2 推荐 characterId
            {
                //
                //UIManager.Instance.ShowMessage(MessageBoxType.Ok , string.Format("{0}推荐{1}加入队伍", characterId2,characterId));
            }
                break;
            case 6: //成为新队长了
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220109)));
                var e = new UIEvent_TeamFrame_Message(type, teamId, characterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case 7: //队伍解散了
            {
                //todo
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220115)));
                var e = new UIEvent_TeamFrame_Message(type, teamId, characterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case 10: //被踢出队伍
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220107)));
                var e = new UIEvent_TeamFrame_Message(type, teamId, characterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case 12: //队长拒绝了你的申请
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220121)));
            }
                break;
            case 13: //队长同意了你的申请
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220106)));
                var e = new TeamApplyEvent();
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
        }
    }

    public void MatchingSuccess(int fubenId)
    {
        var tbQueue = Table.GetQueue(fubenId);
        var tbFuben = Table.GetFuben(tbQueue.Param);
        //是否现在进入：{0}
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
            string.Format(GameUtils.GetDictionaryText(270012), tbFuben.Name), "",
            () =>
            {
                var e = new UIEvent_MatchingBack_Event(1);
                EventDispatcher.Instance.DispatchEvent(e);
            },
            () =>
            {
                var e = new UIEvent_MatchingBack_Event(0);
                EventDispatcher.Instance.DispatchEvent(e);
            }, false, true);
    }

    public void NotifyMatchingData(QueueInfo queueInfo)
    {
        PlayerDataManager.Instance.NotifyMatchingData(queueInfo);
    }

    public void TeamServerMessage(int resultType, string args)
    {
        var queueData = PlayerDataManager.Instance.PlayerDataModel.QueueUpData;
        switch ((eLeaveMatchingType) resultType)
        {
            case eLeaveMatchingType.TimeOut: //通知玩家已经离开匹配
            {
                queueData.QueueId = -1;
                //超时排队确认时间
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270013));
            }
                break;
            case eLeaveMatchingType.TeamOther: //通知玩家有玩家取消了匹配  
                //排队有玩家未确认
            {
                queueData.QueueId = -1;
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270014));
            }
                break;
            case eLeaveMatchingType.Refuse: //我自己拒绝
            {
                queueData.QueueId = -1;
                //已取消排队
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270015));
            }
                break;
            case eLeaveMatchingType.TeamRefuse: //队伍中有人拒绝
            {
                queueData.QueueId = -1;
                //同队中有玩家拒绝
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270016));
            }
                break;
            case eLeaveMatchingType.Cannel: //自己取消了
            case eLeaveMatchingType.PushCannel: //自己取消了
            case eLeaveMatchingType.MatchingBackCannel: //自己取消了
            {
                queueData.QueueId = -1;
                //已取消排队
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270017));
            }
                break;
            case eLeaveMatchingType.TeamCannel: //自己取消了
            {
                queueData.QueueId = -1;
                //队伍中有玩家取消排队
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270018));
            }
                break;
            case eLeaveMatchingType.LeaderLost: //队长离线
            case eLeaveMatchingType.MemberLost: //队长离线
            {
                GameUtils.ShowHintTip(448);
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBox));
            }
                break;
            case eLeaveMatchingType.LeaderLeave: //队长离队
            case eLeaveMatchingType.MemberLeave: //队员离队
            {
                var content = GameUtils.GetDictionaryText(492);
                GameUtils.ShowHintTip(string.Format(content, args));
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBox));
            }
                break;
            case eLeaveMatchingType.TeamChange: //队伍发生改变
            {
                queueData.QueueId = -1;
                //同队发生改变，排队取消
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270019));
            }
                break;
            case eLeaveMatchingType.OtherRefuse: //其他玩家取消，重新进入排队前列
            {
                //其他玩家取消了匹配，重新进入排队前列
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270020));
            }
                break;
            case eLeaveMatchingType.Success: //进入成功
                queueData.QueueId = -1;
                break;
            case eLeaveMatchingType.SceneOver: //其他玩场景已结束，进入排队首
            {
                //目标场景已关闭，重新进入排队前列
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(270021));
            }
                break;
            case eLeaveMatchingType.InTemp:
                break;
            case eLeaveMatchingType.Onlost:
                break;
        }

        //取消主界面排队显示
        if (queueData.QueueId == -1)
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(Game.Instance.ServerTime, -1));
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(queueData.StartTime,
                queueData.QueueId));
        }

        var e = new QueneUpdateEvent();
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncChatMessage(int chatType, ulong characterId, string characterName, ChatMessageContent content)
    {
        GameUtils.OnReceiveChatMsg(chatType, characterId, characterName, content);
    }

    public void SendGroupMessage(StringArray contents)
    {
    }

    public void BroadcastWorldMessage(int chatType, ulong characterId, string characterName, ChatMessageContent content)
    {
        GameUtils.OnReceiveChatMsg(chatType, characterId, characterName, content);
    }

    public void SyncToListCityChatMessage(int chatType,
                                          ulong characterId,
                                          string characterName,
                                          ChatMessageContent content,
                                          string channelName)
    {
        GameUtils.OnReceiveChatMsg(chatType, characterId, characterName, content, channelName);
    }

    public void SyncMissions(MissionDataMessage missions)
    {
        var data = MissionManager.Instance.MissionData.Datas;
        var listCompletedMission = new List<int>();
        var listAcceptableMission = new List<int>();
        {
            // foreach(var pair in missions.Missions)
            var __enumerator1 = (missions.Missions).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var pair = __enumerator1.Current;
                {
                    var newMission = pair.Value;
                    MissionBaseModel mission = null;
                    if (!data.TryGetValue(newMission.MissionId, out mission))
                    {
                        mission = new MissionBaseModel
                        {
                            MissionId = newMission.MissionId
                        };
                        data.Add(newMission.MissionId, mission);
                    }
                    if (eMissionState.Finished != (eMissionState) mission.Exdata[0] &&
                        eMissionState.Finished == (eMissionState) newMission.Exdata[0])
                    {
                        listCompletedMission.Add(mission.MissionId);
                    }
                    else if (eMissionState.Acceptable == (eMissionState) newMission.Exdata[0])
                    {
                        listAcceptableMission.Add(mission.MissionId);
                    }
                    for (var i = 0; i != 5; ++i)
                    {
                        mission.Exdata[i] = newMission.Exdata[i];
                    }

                    EventDispatcher.Instance.DispatchEvent(new Event_UpdateMissionData(newMission.MissionId));
                }
            }
        }

        if (null != GameLogic.Instance && null != GameLogic.Instance.GuideTrigger)
        {
            {
                var __list3 = listCompletedMission;
                var __listCount3 = __list3.Count;
                for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                {
                    var id = __list3[__i3];
                    {
                        MissionManager.Instance.OnMissionComplete(id);
                    }
                }
            }
            {
                var __list4 = listCompletedMission;
                var __listCount4 = __list4.Count;
                for (var __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var id = __list4[__i4];
                    {
                        if (GameLogic.Instance.GuideTrigger.OnMissionComplete(id))
                        {
                            return;
                        }
                    }
                }
            }
            {
                var __list5 = listAcceptableMission;
                var __listCount5 = __list5.Count;
                for (var __i5 = 0; __i5 < __listCount5; ++__i5)
                {
                    var id = __list5[__i5];
                    {
                        if (GameLogic.Instance.GuideTrigger.OnMissionBecomeAcceptable(id))
                        {
                            return;
                        }
                    }
                }
            }
            //自动提交
            MissionManager.Instance.CheckAndAutoCommitMission();
        }
    }

    public void PickUpItemSuccess(ulong dropItemId)
    {
        //Logger.Debug("PickUpItemSuccess obj=[{0}]", dropItemId);

        var obj = ObjManager.Instance.FindObjById(dropItemId);
        if (null == obj)
        {
            Logger.Warn("drop item[{0}] is null", dropItemId);
            return;
        }

        if (obj.GetObjType() != OBJ.TYPE.DROPITEM)
        {
            Logger.Warn("obj[{0}] is not DROPITEM", dropItemId);
            return;
        }

        var dropItem = obj as ObjDropItem;
        if (null != dropItem)
        {
            dropItem.FlyToPlayer();
        }
    }

    public void NotifySceneAction(int ActionId)
    {
        EventDispatcher.Instance.DispatchEvent(new SceneEvent_Trigger(ActionId));
    }

    public void FinishAchievement(int achievementId)
    {
        Logger.Debug("FinishAchievement[{0}]", achievementId);
        if (GameLogic.Instance == null)
        {
            StartCoroutine(DelayAchievement(achievementId));
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(new Event_AchievementTip(achievementId));
            //             var a1 = Table_Tamplet.Convert_Int(Table.GetClientConfig(562).Value);
            //             if (PlayerDataManager.Instance.GetFlag(a1))
            // 			{
            // 				EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.AchievementTip));
            // 			}
        }
    }

    public void SeekCharactersReceive(CharacterSimpleDataList result)
    {
        var e = new FriendReceive(result);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SeekFriendsReceive(CharacterSimpleDataList result)
    {
        var e = new FriendReceive(result);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncMails(MailList mails)
    {
        var e = new MailSyncEvent(mails);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void EquipDurableBroken(int partId, int value)
    {
        var bagId = partId/10;
        var bagIndex = partId%10;
        PlayerDataManager.Instance.EquipDurableBroken(bagId, bagIndex, value);
        //         var item = PlayerDataManager.Instance.GetItem(bagId, bagIndex);
        //         if (item == null || item.Exdata.Count < 23)
        //         {
        //             return;
        //         }
        //         item.Exdata[22] = value;
        //         var tbEquip = Table.GetEquipBase(item.ItemId);
        //         if (value <= 0)
        //         {
        //             EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(2));
        //         }
        //         else if (tbEquip.Durability >= value * 10)
        //         {
        //             EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(1));
        //         }
    }

    public void EquipDurableChange(int placeholder)
    {
        PlayerDataManager.Instance.PlayerDataModel.Bags.IsDuableChange = true;
    }

    public void NotifGainRes(DataChangeList changes)
    {
        foreach (var change in changes.Chaneges)
        {
            PlayerDataManager.Instance.ResGainTip(change.ChangeId, change.ChangeValue);
        }
    }

    public void BattleResult(int dungeonId, int resultType, int first)
    {
        var e1 = new DungeonFightOver();
        EventDispatcher.Instance.DispatchEvent(e1);

        var e = new Show_UI_Event(UIConfig.BattleResult, new BattleResultArguments
        {
            DungeonId = dungeonId,
            BattleResult = resultType,
            First = first
        });
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void NotifyP1vP1Change(P1vP1Change_One one)
    {
        var e = new ArenaFightRecoardChange(one);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncFriendDataChange(CharacterSimpleDataList Changes)
    {
        var e = new FriendUpdateSyncEvent(Changes);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncAddFriend(int type, CharacterSimpleData character)
    {
        var e = new FriendAddSyncEvent(type, character);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncFriendDelete(int type, ulong characterId)
    {
        var e = new FriendDelSyncEvent(type, characterId);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncCityBuildingData(BuildingList data)
    {
        {
            var __list2 = data.Data;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var building = __list2[__i2];
                {
                    var buildingData = CityManager.Instance.GetBuildingByAreaId(building.AreaId);
                    if (null == buildingData)
                    {
                        continue;
                    }
                    var oldType = buildingData.TypeId;
                    buildingData.Guid = building.Guid;
                    buildingData.TypeId = building.TypeId;
                    buildingData.AreaId = building.AreaId;
                    buildingData.State = building.State;
                    buildingData.Exdata.Clear();
                    buildingData.Exdata.AddRange(building.Exdata);
                    buildingData.PetList.Clear();
                    buildingData.PetList.AddRange(building.PetList);
                    buildingData.Exdata64.Clear();
                    buildingData.Exdata64.AddRange(building.Exdata64);
                    buildingData.PetTime.Clear();
                    buildingData.PetTime.AddRange(building.PetTime);
                    buildingData.OverTime = building.OverTime;
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_CityUpdateBuilding(building.AreaId));

                    //判断是否是升级
                    if (-1 != oldType && -1 != buildingData.TypeId)
                    {
                        var tableOldBuilding = Table.GetBuilding(oldType);
                        var tableBuilding = Table.GetBuilding(buildingData.TypeId);
                        if (null != tableOldBuilding && null != tableBuilding)
                        {
                            if (tableOldBuilding.Type == tableBuilding.Type)
                            {
                                if (tableBuilding.Level > tableOldBuilding.Level)
                                {
                                    var e = new CityBuildingLevelupEvent(building.AreaId);
                                    var tbBuilding = Table.GetBuilding(buildingData.TypeId);
                                    e.HomeExp = tbBuilding.GetMainHouseExp;
                                    EventDispatcher.Instance.DispatchEvent(e);
                                }
                            }
                        }
                    }
                }
            }
        }
    }


	public void BroadcastSceneChat(string content, int dictId)
	{
		string str = content;
		if (-1 != dictId)
		{
			str = GameUtils.GetDictionaryText(dictId);
			if (!string.IsNullOrEmpty(content))
			{
				str = string.Format(str, content.Split('|'));
			}
		}
		var c = new ChatMessageContent();
		c.Content = str;
		GameUtils.OnReceiveChatMsg((int)eChatChannel.Scene, 0uL, "", c);
	}


}