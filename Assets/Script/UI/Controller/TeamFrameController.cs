#region using

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class TeamFrameController : IControllerBase
{
    public static CharacterBaseDataModel EmptyCharacterBaseData;

    public TeamFrameController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnUpdateFlagData);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, OnFlagInit);

        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_Message.EVENT_TYPE, TeamMessage);
        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_Leave.EVENT_TYPE, OnClickLeaveTeam);
        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_Kick.EVENT_TYPE, OnClickKickTeam);

        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_NearTeam.EVENT_TYPE, Button_NearTeam);
        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_NearPlayer.EVENT_TYPE, Button_NearPlayer);

        EventDispatcher.Instance.AddEventListener(Event_TeamApplyOtherTeam.EVENT_TYPE, ApplyOtherTeam);

        EventDispatcher.Instance.AddEventListener(Event_TeamSwapLeader.EVENT_TYPE, SwapLeader);
        EventDispatcher.Instance.AddEventListener(Event_TeamKickPlayer.EVENT_TYPE, OnKickTeam);
        EventDispatcher.Instance.AddEventListener(Event_TeamLeaveTeam.EVENT_TYPE, OnLeaveTeam);
        EventDispatcher.Instance.AddEventListener(UIEvent_MatchingBack_Event.EVENT_TYPE, SendMatchingBack);
        EventDispatcher.Instance.AddEventListener(Event_TeamAcceptJoin.EVENT_TYPE, TeamAcceptJoin);
        EventDispatcher.Instance.AddEventListener(Event_TeamRefuseJoin.EVENT_TYPE, TeamRefuseJoin);
        EventDispatcher.Instance.AddEventListener(Enter_Scene_Event.EVENT_TYPE, OnEnterScene);

        EventDispatcher.Instance.AddEventListener(CharacterEquipChange.EVENT_TYPE, OnCharacterEquipChange);
        //Invite
        EventDispatcher.Instance.AddEventListener(Event_TeamInvitePlayer.EVENT_TYPE, InvitePlayer);
        EventDispatcher.Instance.AddEventListener(UIEvent_OperationList_AcceptInvite.EVENT_TYPE, AcceptInvite);
        EventDispatcher.Instance.AddEventListener(UIEvent_OperationList_RefuseInvite.EVENT_TYPE, RefuseInvite);

        //common
        EventDispatcher.Instance.AddEventListener(TeamOperateEvent.EVENT_TYPE, OnTeamOperate);
        EventDispatcher.Instance.AddEventListener(TeamApplyEvent.EVENT_TYPE, OnApplyTeam);
        EventDispatcher.Instance.AddEventListener(TeamMemberShowMenu.EVENT_TYPE, OnTeamMemberShowMenu);
        //cell
        EventDispatcher.Instance.AddEventListener(TeamNearbyPlayerClick.EVENT_TYPE, OnTeamNearbyOtherClick);
        EventDispatcher.Instance.AddEventListener(TeamNearbyTeamClick.EVENT_TYPE, OnTeamNearbyTeamClick);
        //charater
        EventDispatcher.Instance.AddEventListener(Character_Create_Event.EVENT_TYPE, OnCreateCharacter);
        //setting
        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_AutoJion.EVENT_TYPE, OnClickAutoJion);
        EventDispatcher.Instance.AddEventListener(UIEvent_TeamFrame_AutoAccept.EVENT_TYPE, OnClickAutoAccept);
        //map
        EventDispatcher.Instance.AddEventListener(SceneMapNotifyTeam.EVENT_TYPE, OnSceneMapNotify);
    }

    private readonly OtherPlayerDataModel MyotherPlayer = new OtherPlayerDataModel();
    public object PostionTrigger;

    public TeamDataModel DataModel
    {
        get { return PlayerDataManager.Instance.TeamDataModel; }
        set { PlayerDataManager.Instance.TeamDataModel = value; }
    }

    public void AcceptInvite(IEvent ievent)
    {
        var ee = ievent as UIEvent_OperationList_AcceptInvite;
        NetManager.Instance.StartCoroutine(AcceptInvite(ee.PlayerId, ee.TeamId));
    }

    private IEnumerator AcceptInvite(ulong PlayerId, ulong TeamId)
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(PlayerDataManager.Instance.GetGuid(), 2, TeamId, 0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220106)));
                    if (!PlayerDataManager.Instance.GetFlag(500))
                    {
                        var flagList = new Int32Array();
                        flagList.Items.Add(500);
                        PlayerDataManager.Instance.SetFlagNet(flagList);
                    }
                    ApplyTeam();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_TeamIsFull)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220111)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveTeam)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220105)));
                }
            }
            else
            {
                Logger.Error("AcceptInvite Error!............State..." + msg.State);
            }
        }
    }

    public void AcceptJoinTeam(ulong toCharacterId)
    {
        if (DataModel.TeamId == 0)
        {
//没有队伍
            ApplyTeam();

            Logger.Error("----------------Team-----AcceptJoinTeam------DataModel.TeamId");
            return;
        }
        if (IsLeader() == false)
        {
//不是队长
            ApplyTeam();
            Logger.Error("----------------Team-----AcceptJoinTeam------IsLeader() == false");
            return;
        }
        if (GetTeamMemberCount() == 5)
        {
            //"你的队伍已经满了"
            GameUtils.ShowHintTip(220111);
            return;
        }
        var uGuid = PlayerDataManager.Instance.GetGuid();
        NetManager.Instance.StartCoroutine(AcceptJoinTeamEnumerator(uGuid, 0ul, toCharacterId));
    }

    private IEnumerator AcceptJoinTeamEnumerator(ulong characterId, ulong teamId, ulong toCharacterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(characterId, 4, teamId, toCharacterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //同意对方的申请
                    GameUtils.ShowHintTip(271008);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_OtherHasTeam)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unknow)
                {
                    //TODO
                    //"对方已经不在申请列表中了"
                    GameUtils.ShowHintTip(271009);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotTeam
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamNotFind
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamIsFull
                         || msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    ApplyTeam();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("AcceptJoinTeam Error!............State..." + msg.State);
            }
        }
    }

    private void AddTeamMember(ulong characterId)
    {
        var count = GetTeamMemberCount();
        if (count == 0)
        {
//组建了队伍
            ApplyTeam();
        }
        else if (count == 5)
        {
//队伍已满，状态错误
            ApplyTeam();
            return;
        }

        var index = GetMemberIndex(characterId);
        if (index != -1)
        {
//已在队伍
            return;
        }

        NotifyTeamChange();
        var teamData = DataModel.TeamList[count];
        teamData.Guid = characterId;
        PlayerDataManager.Instance.ApplyPlayerInfo(characterId, ApplyTeamMemberInfo);
    }

    private void ApplyMemberPostion()
    {
        if (DataModel.TeamId == 0)
        {
            return;
        }
        var ary = new Uint64Array();
        var myGuid = PlayerDataManager.Instance.GetGuid();
        for (var i = 0; i < 5; i++)
        {
            var one = DataModel.TeamList[i];
            if (myGuid != 0 && myGuid != one.Guid)
            {
                ary.Items.Add(one.Guid);
            }
        }
        NetManager.Instance.StartCoroutine(ApplyMemberPostionCoroutine(ary));
    }

    private IEnumerator ApplyMemberPostionCoroutine(Uint64Array ary)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyPlayerPostionList(ary);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var sceneMapController = UIManager.Instance.GetController(UIConfig.SceneMapUI);
                    var ret = msg.Response.List;
                    for (var i = 0; i < ret.Count; i++)
                    {
                        var guid = ary.Items[i];
                        var index = GetMemberIndex(guid);
                        var member = DataModel.TeamList[index];
                        var pos = ret[i];
                        if (pos.x == -1 || pos.y == -1)
                        {
                            member.ShowMap = false;
                        }
                        else
                        {
                            var v3 = new Vector3(GameUtils.DividePrecision(pos.x), 0, GameUtils.DividePrecision(pos.y));
                            member.ShowMap = true;
                            var loc =
                                (Vector3) sceneMapController.CallFromOtherClass("ConvertSceneToMap", new object[] {v3});
                            member.MapLoction = loc;
                        }
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("SendMatchingBack Error!............State..." + msg.State);
            }
        }
    }

    //----------------------------------------------------------申请加入-----------------------------
    public void ApplyOtherTeam(IEvent evt)
    {
        var e = evt as Event_TeamApplyOtherTeam;
        ApplyOtherTeam(e.CharacterId);
    }

    public void ApplyOtherTeam(ulong toCharacterId)
    {
        if (DataModel.TeamId != 0)
        {
            GameUtils.ShowHintTip(413);
            return;
        }
        var uGuid = PlayerDataManager.Instance.GetGuid();
        NetManager.Instance.StartCoroutine(ApplyOtherTeamEnumerator(uGuid, 0ul, toCharacterId));
    }

    private IEnumerator ApplyOtherTeamEnumerator(ulong characterId, ulong teamId, ulong toCharacterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(characterId, 3, teamId, toCharacterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220100)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveTeam)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220105)));
                    ApplyTeam();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unline)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220103)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_TeamIsFull)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220111)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotTeam
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamNotFind)
                {
                    //TODO
                    //GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    GameUtils.ShowHintTip(200002408);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("Invite Error!............State..." + msg.State);
            }
        }
    }

    public void ApplySceneObj()
    {
        NetManager.Instance.StartCoroutine(ApplySceneObjEnumerator());
    }

    private IEnumerator ApplySceneObjEnumerator()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplySceneObj(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.NearPlayerList.Clear();
                    {
                        var __list3 = msg.Response.Data;
                        var __listCount3 = __list3.Count;
                        if (__list3.Count == 0)
                        {
                            DataModel.EmptyTips[2] = true;
                        }
                        else
                        {
                            DataModel.EmptyTips[2] = false;
                        }
                        for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                        {
                            var simpleInfo = __list3[__i3];
                            {
                                string serverName;
                                PlayerDataManager.Instance.ServerNames.TryGetValue(simpleInfo.Serverid, out serverName);

                                var otherPlayer = new OtherPlayerDataModel
                                {
                                    Guid = simpleInfo.CharacterId,
                                    Name = simpleInfo.Name,
                                    Level = simpleInfo.Level,
                                    TypeId = simpleInfo.Type,
                                    FightValue = simpleInfo.FightValue,
                                    SceneId = MyotherPlayer.SceneId,
                                    Ladder = simpleInfo.Ladder,
                                    ServerName = serverName
                                };
                                DataModel.NearPlayerList.Add(otherPlayer);
                            }
                        }
                    }
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    OtherPlayerDataModel otherPlayer = new OtherPlayerDataModel()
                    //    {
                    //        Guid = ObjManager.Instance.MyPlayer.GetObjId(),
                    //        Name = ObjManager.Instance.MyPlayer.Name,
                    //        Level = i*10 + 5,
                    //        TypeId = i%3,
                    //    };
                    //    DataModel.NearPlayerList.Add(otherPlayer);
                    //}
                }
                else
                {
                    UIManager.Instance.RemoveBlockLayer();
                    Logger.Debug("ApplySceneObj..................." + msg.ErrorCode);
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                UIManager.Instance.RemoveBlockLayer();
                Logger.Debug("ApplySceneObj..................." + msg.ErrorCode);
            }
        }
    }

    private IEnumerator ApplySceneTeamEnumerator()
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplySceneTeamLeaderObj(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.NearTeamList.Clear();
                    var __list3 = msg.Response.Data;
                    var __listCount3 = __list3.Count;
                    if (__listCount3 == 0)
                    {
                        DataModel.EmptyTips[1] = true;
                    }
                    else
                    {
                        DataModel.EmptyTips[1] = false;
                    }
                    for (var __i3 = 0; __i3 < __listCount3; ++__i3)
                    {
                        var simpleInfo = __list3[__i3];
                        {
                            string serverName;
                            PlayerDataManager.Instance.ServerNames.TryGetValue(simpleInfo.Serverid, out serverName);

                            var otherPlayer = new OtherTeamDataModel
                            {
                                Guid = simpleInfo.CharacterId,
                                Name = simpleInfo.Name,
                                Level = simpleInfo.Level,
                                TypeId = simpleInfo.Type,
                                FightValue = simpleInfo.FightValue,
                                SceneId = MyotherPlayer.SceneId,
                                Count = simpleInfo.RoleId,
                                Ladder = simpleInfo.Ladder,
                                ServeName = serverName
                            };
                            DataModel.NearTeamList.Add(otherPlayer);
                        }
                    }
//                     for (int i = 0; i < 5; i++)
//                     {
//                         OtherTeamDataModel otherTeam = new OtherTeamDataModel();
//                         //{
//                         msg.Response.Data
//                         otherTeam.Guid = ObjManager.Instance.MyPlayer.GetObjId();
//                         otherTeam.Name = ObjManager.Instance.MyPlayer.Name;
//                         otherTeam.Level = i * 10 + 5;
//                         otherTeam.TypeId = i % 3;
//                         otherTeam.Count = (i + 5) % 5 + 1;
//                         otherTeam.TeamId = DataModel.TeamId + (ulong)i;
//                         //};
//                         DataModel.NearTeamList.Add(otherTeam);
//                     }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    UIManager.Instance.RemoveBlockLayer();
                    Logger.Debug("ApplyNearTeam..................." + msg.ErrorCode);
                }
            }
            else
            {
                UIManager.Instance.RemoveBlockLayer();
                Logger.Debug("ApplySceneObj..................." + msg.ErrorCode);
            }
        }
    }

    public void ApplyTeam()
    {
        NetManager.Instance.StartCoroutine(ApplyTeamCoroutine());
    }

    private IEnumerator ApplyTeamCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyTeam(PlayerDataManager.Instance.GetGuid());
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.TeamId = msg.Response.TeamId;
                    var index = 0;
                    {
                        var __list4 = msg.Response.Teams;
                        var __listCount4 = __list4.Count;
                        if (__list4.Count == 0)
                        {
                            DataModel.EmptyTips[0] = true;
                        }
                        else
                        {
                            DataModel.EmptyTips[0] = false;
                        }
                        for (var __i4 = 0; __i4 < __listCount4; ++__i4)
                        {
                            var simpleInfo = __list4[__i4];
                            {
                                DataModel.TeamList[index].Guid = simpleInfo.CharacterId;
                                DataModel.TeamList[index].Name = simpleInfo.Name;
                                DataModel.TeamList[index].Level = simpleInfo.Level;
                                DataModel.TeamList[index].TypeId = simpleInfo.Type;
                                DataModel.TeamList[index].FightValue = simpleInfo.FightValue;
                                DataModel.TeamList[index].Ladder = simpleInfo.Ladder;
                                DataModel.TeamList[index].Equips.Clear();
                                DataModel.TeamList[index].Equips = new Dictionary<int, int>(simpleInfo.EquipsModel);
                                DataModel.TeamList[index].IsLeave = !simpleInfo.OnLine;
                                SetTeamMemberCharacterBase(DataModel.TeamList[index]);
                                index++;
                            }
                        }
                    }
                    for (var i = index; i != 5; ++i)
                    {
                        CleanTeamOne(DataModel.TeamList[i]);
                    }
                    CheckTeamOperation();

                    NotifyModelView();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("ApplyTeam..................." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("ApplyTeam..................." + msg.State);
            }
        }
    }

    private void ApplyTeamMemberInfo(PlayerInfoMsg msg)
    {
        var index = GetMemberIndex(msg.Id);
        if (index == -1)
        {
            return;
        }
        var teamData = DataModel.TeamList[index];
        teamData.Name = msg.Name;
        teamData.Level = msg.Level;
        teamData.TypeId = msg.TypeId;
        teamData.FightValue = msg.FightPoint;
        teamData.Ladder = msg.Ladder;
        teamData.Equips = new Dictionary<int, int>(msg.EquipsModel);
        SetTeamMemberCharacterBase(teamData);
        NotifyModelView();
    }

    //tab：附近玩家
    private void Button_NearPlayer(IEvent ievent)
    {
        ApplySceneObj();
    }

    //-------------------------------------------
    private void Button_NearTeam(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ApplySceneTeamEnumerator());
    }

    private void CheckAutoSetting()
    {
        var join = PlayerDataManager.Instance.GetFlag(485);
        var accept = PlayerDataManager.Instance.GetFlag(486);
        if (DataModel.AutoJoin == join && DataModel.AutoAccept == accept)
        {
            return;
        }
        var tureArray = new Int32Array();
        var falseArray = new Int32Array();
        if (DataModel.AutoJoin != join)
        {
            if (DataModel.AutoJoin)
            {
                tureArray.Items.Add(485);
            }
            else
            {
                falseArray.Items.Add(485);
            }
        }

        if (DataModel.AutoAccept != accept)
        {
            if (DataModel.AutoAccept)
            {
                tureArray.Items.Add(486);
            }
            else
            {
                falseArray.Items.Add(486);
            }
        }
        PlayerDataManager.Instance.SetFlagNet(tureArray, falseArray);
    }

    private void CheckTeamOperation()
    {
        var myUid = PlayerDataManager.Instance.GetGuid();
        var isLeader = IsLeader();
        for (var i = 0; i < 5; i++)
        {
            var one = DataModel.TeamList[i];
            if (one.Guid == myUid)
            {
                one.Operation = 1;
            }
            else
            {
                if (isLeader)
                {
                    one.Operation = 2;
                }
                else
                {
                    one.Operation = 0;
                }
            }
        }
    }

    public void CleanTeamOne(TeamOneDataModel teamOne)
    {
        teamOne.Guid = 0;
        teamOne.TypeId = -1;
        teamOne.Level = 0;
        teamOne.Name = "";
        teamOne.BaseDataModel = EmptyCharacterBaseData;
    }

    //解散队伍
    public void DisbandTeam()
    {
        NotifyTeamChange(10);
        DataModel.EmptyTips[0] = true;
        DataModel.TeamId = 0;
        {
            // foreach(var i in DataModel.TeamList)
            var __enumerator2 = (DataModel.TeamList).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var i = __enumerator2.Current;
                {
                    CleanTeamOne(i);
                }
            }
        }
        var e2 = new Close_UI_Event(UIConfig.OperationList);
        EventDispatcher.Instance.DispatchEvent(e2);
        NotifyModelView();
    }

    private void EnterScene(int scendId)
    {
        MyotherPlayer.SceneId = scendId;
    }

    //base
    private int GetMemberIndex(ulong characterId)
    {
        if (DataModel.TeamId == 0)
        {
            return -1;
        }
        for (var i = 0; i < 5; i++)
        {
            var one = DataModel.TeamList[i];
            if (one.Guid == characterId)
            {
                return i;
            }
        }
        return -1;
    }

    private int GetTeamMemberCount()
    {
        var count = 0;
        for (var i = 0; i < 5; i++)
        {
            var one = DataModel.TeamList[i];
            if (one.Guid != 0)
            {
                count++;
            }
        }
        return count;
    }

    private bool HasTeam()
    {
        return DataModel.TeamId != 0;
    }

    //-------------------------------------------------------邀请玩家----------------------------
    public void InvitePlayer(IEvent evt)
    {
        var e = evt as Event_TeamInvitePlayer;
        InvitePlayer(e.CharacterId);
    }

    public void InvitePlayer(ulong toCharacterId)
    {
        var uGuid = PlayerDataManager.Instance.GetGuid();
        if (uGuid == toCharacterId)
        {
            return;
        }
        if (IsInTeam(toCharacterId))
        {
            //"已经是同一队伍了"
            GameUtils.ShowHintTip(300858);
            return;
        }
        NetManager.Instance.StartCoroutine(InvitePlayerEnumerator(uGuid, DataModel.TeamId, toCharacterId));
    }

    private IEnumerator InvitePlayerEnumerator(ulong characterId, ulong teamId, ulong toCharacterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(characterId, 1, teamId, toCharacterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220117)));
                    PlatformHelper.UMEvent("Team", "Invite");
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unline)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220103)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveTeam)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220104)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_TeamIsFull)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220112)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_AlreadyToLeader)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220118)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_TeamFunctionNotOpen)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220124)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_SetRefuseTeam)
                {
                    var e1 = new ChatMainHelpMeesage(string.Format(GameUtils.GetDictionaryText(997), ""));
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("Invite Error!............State..." + msg.State);
            }
        }
    }

    private bool IsInTeam(ulong characterId)
    {
        var index = GetMemberIndex(characterId);
        if (index != -1)
        {
            return true;
        }
        return false;
    }

    private bool IsLeader()
    {
        var myUid = PlayerDataManager.Instance.GetGuid();
        var isLeader = myUid == DataModel.TeamList[0].Guid;
        return isLeader;
    }

    public void KickTeam(ulong characterId)
    {
        if (DataModel.TeamId == 0)
        {
//没有队伍,状态错误
            ApplyTeam();

            Logger.Error("------------Team--------KickTeam-----DataModel.TeamId");
            return;
        }
        var uGuid = PlayerDataManager.Instance.GetGuid();
        if (uGuid == characterId)
        {
            return;
        }
        if (IsLeader() == false)
        {
//权限不足,状态错误
            Logger.Error("------------Team--------KickTeam-----IsLeader() == false");
            ApplyTeam();
            return;
        }
        NetManager.Instance.StartCoroutine(KickTeamEnumerator(uGuid, DataModel.TeamId, characterId));
    }

    private IEnumerator KickTeamEnumerator(ulong characterId, ulong teamId, ulong toCharacterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(characterId, 8, teamId, toCharacterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    RemoveTeamMember(toCharacterId);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unknow
                         || msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotTeam
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamNotSame
                         || msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error("---------------Team----TeamMessage---{0}", msg.ErrorCode);
                    ApplyTeam();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("KickTeam Error!............State..." + msg.State);
            }
        }
    }

    public void LeaveTeam()
    {
        if (DataModel.TeamId == 0)
        {
//没有队伍
            Logger.Error("----------Team------------LeaveTeam----DataModel.TeamId == 0");
            ApplyTeam();
            return;
        }
        NetManager.Instance.StartCoroutine(LeaveTeamEnumerator());
    }

    private IEnumerator LeaveTeamEnumerator()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(ObjManager.Instance.MyPlayer.GetObjId(), 5, DataModel.TeamId, 0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DisbandTeam();

                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220108)));

                    PlatformHelper.UMEvent("Team", "Leave");
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_TeamNotFind
                         || msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotTeam)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    ApplyTeam();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("LeaveTeam Error!............State..." + msg.State);
            }
        }
    }

    private void NotifyModelView()
    {
        var e = new UIEvent_TeamFrame_RefreshModel(0);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void NotifyTeamChange(int type = 0)
    {
        var e = new TeamChangeEvent(type);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    //请求队伍信息
    private void OnApplyTeam(IEvent ievent)
    {
        ApplyTeam();
    }

    private void OnCharacterEquipChange(IEvent ievent)
    {
        var e = ievent as CharacterEquipChange;
        var index = GetMemberIndex(e.CharacterId);
        if (index == -1)
        {
            return;
        }
        var one = DataModel.TeamList[index];
        one.Equips[e.Part] = e.ItemId;
        if (State == FrameState.Open)
        {
            NotifyModelView();
        }
    }

    public void OnClickAutoAccept(IEvent ievent)
    {
        DataModel.AutoAccept = !DataModel.AutoAccept;
    }

    //----------------------------------------------------Setting-------
    public void OnClickAutoJion(IEvent ievent)
    {
        DataModel.AutoJoin = !DataModel.AutoJoin;
    }

    //踢出队伍
    private void OnClickKickTeam(IEvent ievent)
    {
        var ee = ievent as UIEvent_TeamFrame_Kick;
        KickTeam(DataModel.TeamList[ee.Index].Guid);
    }

    //离开队伍
    private void OnClickLeaveTeam(IEvent ievent)
    {
        LeaveTeam();
    }

    private void OnCloseSceneMap()
    {
        if (PostionTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(PostionTrigger);
            PostionTrigger = null;
        }
        for (var i = 0; i < 5; i++)
        {
            var member = DataModel.TeamList[i];
            if (member.ShowMap)
            {
                member.ShowMap = false;
            }
        }
    }

    private void OnCreateCharacter(IEvent ievent)
    {
        var e = ievent as Character_Create_Event;
        var charId = e.CharacterId;
        var index = GetMemberIndex(charId);
        if (index == -1)
        {
            return;
        }
        var obj = ObjManager.Instance.FindCharacterById(charId);
        if (obj != null)
        {
            DataModel.TeamList[index].BaseDataModel = obj.CharacterBaseData;
            DataModel.TeamList[index].Equips = new Dictionary<int, int>(obj.EquipList);

            if (State == FrameState.Open)
            {
                NotifyModelView();
            }
        }
    }

    private void OnEnterScene(IEvent ievent)
    {
        var e = ievent as Enter_Scene_Event;
        EnterScene(e.SceneId);
    }

    //auto flag
    private void OnFlagInit(IEvent ievent)
    {
        DataModel.AutoJoin = PlayerDataManager.Instance.GetFlag(485);
        DataModel.AutoAccept = PlayerDataManager.Instance.GetFlag(486);
    }

    private void OnKickTeam(IEvent ievent)
    {
        var ee = ievent as Event_TeamKickPlayer;
        KickTeam(ee.CharacterId);
    }

    public void OnLeaveTeam(IEvent evt)
    {
        LeaveTeam();
    }

    //----------------------------------------------------MapLoction-------
    private void OnOpenSceneMap()
    {
        if (PostionTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(PostionTrigger);
            PostionTrigger = null;
        }
        PostionTrigger = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime, ApplyMemberPostion, 3000);
    }

    private void OnSceneMapNotify(IEvent ievent)
    {
        var e = ievent as SceneMapNotifyTeam;
        var isOpen = e.IsOpen;
        if (isOpen)
        {
            OnOpenSceneMap();
        }
        else
        {
            OnCloseSceneMap();
        }
    }

    private void OnTeamMemberShowMenu(IEvent ievent)
    {
        var e = ievent as TeamMemberShowMenu;
        ShowTeamMemberMenu(e.Index);
    }

    private void OnTeamNearbyOtherClick(IEvent ievent)
    {
        var e = ievent as TeamNearbyPlayerClick;

        if (DataModel.NearPlayerList.Count <= e.Index || e.Index < 0)
        {
            Logger.Error("Button_OtherPlayer_Tip  index[{0}] is out", e.Index);
            return;
        }
        var playerData = DataModel.NearPlayerList[e.Index];
        if (e.Type == 0)
        {
            InvitePlayer(playerData.Guid);
        }
        else if (e.Type == 1)
        {
            PlayerDataManager.Instance.ShowCharacterPopMenu(playerData.Guid, playerData.Name, 5, playerData.Level,
                playerData.Ladder, playerData.TypeId);
        }
    }

    //-----------------------------------------event----------------
    private void OnTeamNearbyTeamClick(IEvent ievent)
    {
        var e = ievent as TeamNearbyTeamClick;
        if (DataModel.NearTeamList.Count <= e.Index || e.Index < 0)
        {
            Logger.Error("Button_OtherPlayer_Tip  index[{0}] is out", e.Index);
            return;
        }
        var teamData = DataModel.NearTeamList[e.Index];
        if (e.Type == 0)
        {
            ApplyOtherTeam(teamData.Guid);
        }
        else if (e.Type == 1)
        {
            PlayerDataManager.Instance.ShowCharacterPopMenu(teamData.Guid, teamData.Name, 4, teamData.Level,
                teamData.Ladder, teamData.TypeId);
        }
    }

    private void OnTeamOperate(IEvent ievent)
    {
        var e = ievent as TeamOperateEvent;
        switch (e.Type)
        {
            case 0:
            {
                if (DataModel.HasTeam == false)
                {
                    var e1 = new Show_UI_Event(UIConfig.TeamFrame);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else
                {
                    if (e.misTeamShow == true)
                    {
                        var e1 = new Show_UI_Event(UIConfig.TeamFrame);
                        EventDispatcher.Instance.DispatchEvent(e1);
                        var ee = new UIEvent_TeamFrame_NearTeam();
                        EventDispatcher.Instance.DispatchEvent(ee);
                    }
                }
            }
                break;
        }
    }

    private void OnUpdateFlagData(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        if (e.Index == 485)
        {
            DataModel.AutoJoin = e.Value;
        }
        else if (e.Index == 485)
        {
            DataModel.AutoAccept = e.Value;
        }
    }

    public void RefuseInvite(IEvent ievent)
    {
        var ee = ievent as UIEvent_OperationList_RefuseInvite;
        NetManager.Instance.StartCoroutine(RefuseInvite(ee.PlayerId, ee.TeamId));
    }

    private IEnumerator RefuseInvite(ulong PlayerId, ulong TeamId)
    {
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(PlayerDataManager.Instance.GetGuid(), 9, TeamId, 0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
            }
            else
            {
                Logger.Error("RefuseInvite Error!............State..." + msg.State);
            }
        }
    }

    private void RemoveTeamMember(ulong characterId)
    {
        var index = GetMemberIndex(characterId);
        if (index == -1)
        {
            return;
        }
        NotifyTeamChange();
        for (var i = index; i < 5; i++)
        {
            if (i + 1 < 5)
            {
                //DataModel.TeamList[i] = DataModel.TeamList[i + 1];
                DataModel.TeamList[i].Guid = DataModel.TeamList[i + 1].Guid;
                DataModel.TeamList[i].Name = DataModel.TeamList[i + 1].Name;
                DataModel.TeamList[i].FightValue = DataModel.TeamList[i + 1].FightValue;
                DataModel.TeamList[i].TypeId = DataModel.TeamList[i + 1].TypeId;
                DataModel.TeamList[i].Operation = DataModel.TeamList[i + 1].Operation;
                DataModel.TeamList[i].Level = DataModel.TeamList[i + 1].Level;
                DataModel.TeamList[i].Ladder = DataModel.TeamList[i + 1].Ladder;
                DataModel.TeamList[i].Equips = DataModel.TeamList[i + 1].Equips;
                DataModel.TeamList[i].BaseDataModel = DataModel.TeamList[i + 1].BaseDataModel;
                DataModel.TeamList[i].ShowMap = DataModel.TeamList[i + 1].ShowMap;
                DataModel.TeamList[i].MapLoction = DataModel.TeamList[i + 1].MapLoction;
                DataModel.TeamList[i].IsLeave = DataModel.TeamList[i + 1].IsLeave;
            }
            else
            {
                CleanTeamOne(DataModel.TeamList[4]);
            }
        }
        NotifyModelView();
    }

    //----------------------------------------------------Match-------
    private void SendMatchingBack(IEvent ievent)
    {
        var ee = ievent as UIEvent_MatchingBack_Event;
        SendMatchingBack(ee.Result);
    }

    public void SendMatchingBack(int result)
    {
        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            var nowTbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            if (nowTbScene != null)
            {
                if (nowTbScene.FubenId != -1 && result == 1) //当前正在副本中 就取消预约
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210123));
                    result = 0;
                }
            }
        }

        NetManager.Instance.StartCoroutine(SendMatchingBackEnumerator(result));
    }

    private IEnumerator SendMatchingBackEnumerator(int result)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.MatchingBack(result);
            yield return msg.SendAndWaitUntilDone();
        }
    }

    public void SetLeaveState(bool isLeave, ulong characterId)
    {
        foreach (var item in DataModel.TeamList)
        {
            if (item.Guid == characterId)
            {
                item.IsLeave = isLeave;
                break;
            }
        }
    }

    private void SetTeamMemberCharacterBase(TeamOneDataModel oneData)
    {
        var myGuid = PlayerDataManager.Instance.GetGuid();
        if (oneData.Guid != 0)
        {
            var obj = ObjManager.Instance.FindCharacterById(oneData.Guid);
            if (obj != null)
            {
                oneData.BaseDataModel = obj.CharacterBaseData;
                return;
            }
        }

        oneData.BaseDataModel = new CharacterBaseDataModel();
        oneData.BaseDataModel.Level = oneData.Level;
        oneData.BaseDataModel.Reborn = oneData.Ladder;
        oneData.BaseDataModel.MaxHp = 100;
        oneData.BaseDataModel.Hp = 100;

        oneData.BaseDataModel.MaxMp = 100;
        oneData.BaseDataModel.Mp = 100;
    }

    private void ShowTeamMemberMenu(int memberIndex)
    {
        if (5 <= memberIndex || memberIndex < 0)
        {
            Logger.Error("ShowTeamMemberMenu  index[{0}] is out", memberIndex);
            return;
        }
        var teamMember = DataModel.TeamList[memberIndex];
        if (0 == teamMember.Level)
        {
            return;
        }
        var selfId = PlayerDataManager.Instance.GetGuid();
        var index = 6;
        if (teamMember.Guid != selfId)
        {
            index = IsLeader() ? 3 : 2;
        }
        PlayerDataManager.Instance.ShowCharacterPopMenu(teamMember.Guid, teamMember.Name, index, teamMember.Level,
            teamMember.Ladder, teamMember.TypeId);
    }

    //更换队长
    public void SwapLeader(IEvent ievent)
    {
        var ee = ievent as Event_TeamSwapLeader;
        SwapLeader(ee.CharacterId);
    }

    public void SwapLeader(ulong characterId)
    {
        var uGuid = PlayerDataManager.Instance.GetGuid();
        if (IsLeader() == false)
        {
//权限不足,,状态错误
            Logger.Error("----------------Team-----SwapLeader------IsLeader() == false");
            ApplyTeam();
            return;
        }
        foreach (var item in DataModel.TeamList)
        {
            if (item.IsLeave)
            {
                //玩家已离线
                var e = new ShowUIHintBoard(200002404);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }


        NetManager.Instance.StartCoroutine(SwapLeaderEnumerator(uGuid, DataModel.TeamId, characterId));
    }

    private IEnumerator SwapLeaderEnumerator(ulong characterId, ulong teamId, ulong toCharacterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(characterId, 6, teamId, toCharacterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    SwapTeamMember(characterId, toCharacterId);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unline)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(220103)));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotTeam
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamNotSame
                         || msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader
                         || msg.ErrorCode == (int) ErrorCodes.Unknow)
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    ApplyTeam();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("KickTeam Error!............State..." + msg.State);
            }
        }
    }

    private void SwapTeamMember(ulong characterFrom, ulong characterTo)
    {
        var indexFrom = GetMemberIndex(characterFrom);
        var indexTo = GetMemberIndex(characterTo);
        if (indexFrom == -1 || indexTo == -1)
        {
            return;
        }
        var from = DataModel.TeamList[indexFrom];
        DataModel.TeamList[indexFrom] = DataModel.TeamList[indexTo];
        DataModel.TeamList[indexTo] = from;
        NotifyTeamChange();
        NotifyModelView();
    }

    public void TeamAcceptJoin(IEvent evt)
    {
        var e = evt as Event_TeamAcceptJoin;
        AcceptJoinTeam(e.CharacterId);
    }

    //有队伍消息
    public void TeamMessage(IEvent ievent)
    {
        var ee = ievent as UIEvent_TeamFrame_Message;
        //TODO
        Logger.Warn("-----TeamMessage----Type = {0}---- TeamId={1}-----CharacterID= {2}", ee.Type, ee.TeamId,
            ee.CharacterId);
        //TODO
        switch (ee.Type)
        {
            case 1: //被characterId邀请
            {
            }
                break;
            case 2: //characterId2 推荐 characterId
            {
            }
                break;
            case 3: //characterId加入了队伍
            {
                if (!PlayerDataManager.Instance.GetFlag(500))
                {
                    var flagList = new Int32Array();
                    flagList.Items.Add(500);
                    PlayerDataManager.Instance.SetFlagNet(flagList);
                }
                AddTeamMember(ee.CharacterId);
            }
                break;
            case 4: //characterId想要加入队伍，是否同意
            {
            }
                break;
            case 5: //characterId退出了队伍
            {
                RemoveTeamMember(ee.CharacterId);
            }
                break;
            case 8: //队伍中的characterId 下线了
            {
            }
                break;
            case 9: //队伍中的characterId 上线了
            {
            }
                break;
            case 7: //队伍解散了
            case 10: //被踢出队伍
            {
                DisbandTeam();
            }
                break;
            case 6: //成为新队长了
            case 14: //换队长
            {
                var leader = DataModel.TeamList[0].Guid;
                SwapTeamMember(leader, ee.CharacterId);
            }
                break;
        }
    }

    public void TeamRefuseJoin(IEvent evt)
    {
        var e = evt as Event_TeamRefuseJoin;
        TeamRefuseJoin(e.CharacterId);
    }

    public void TeamRefuseJoin(ulong toCharacterId)
    {
        if (DataModel.TeamId == 0)
        {
//没有队伍
            ApplyTeam();
            Logger.Error("----------------Team-----TeamRefuseJoin------DataModel.TeamId");
            return;
        }
        if (IsLeader() == false)
        {
//不是队长
            ApplyTeam();
            Logger.Error("----------------Team-----TeamRefuseJoin------IsLeader() == false");
            return;
        }
        NetManager.Instance.StartCoroutine(TeamRefuseJoinEnumerator(0ul, 0ul, toCharacterId));
    }

    private IEnumerator TeamRefuseJoinEnumerator(ulong characterId, ulong teamId, ulong toCharacterId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamMessage(characterId, 10, teamId, toCharacterId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    GameUtils.ShowHintTip(271006);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotTeam)
                {
//发现自己没有队伍
                    ApplyTeam();
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
//自己不是队长
                    ApplyTeam();
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unknow)
                {
//TODO
                    //GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    GameUtils.ShowHintTip(271007);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("AcceptJoinTeam Error!............State..." + msg.State);
            }
        }
    }

    private void UpdataCharacterInfo()
    {
        for (var i = 0; i < 5; i++)
        {
            var one = DataModel.TeamList[i];
            if (one.Level == 0)
            {
                continue;
            }

            if (one.BaseDataModel == null)
            {
                continue;
            }
            if (one.BaseDataModel.Level == 0)
            {
                continue;
            }
            if (one.BaseDataModel.Level > one.Level)
            {
                one.Level = one.BaseDataModel.Level;
            }
            if (one.BaseDataModel.Reborn > one.Ladder)
            {
                one.Ladder = one.BaseDataModel.Reborn;
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new TeamDataModel();
        if (PostionTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(PostionTrigger);
            PostionTrigger = null;
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "HasTeam")
        {
            return HasTeam();
        }
        if (name == "IsInTeam")
        {
            var id = (ulong) param[0];
            return IsInTeam(id);
        }
        if (name == "SetLeaveState")
        {
            var isLeave = (bool) param[0];
            var id = (ulong) param[1];
            SetLeaveState(isLeave, id);
        }
        return null;
    }

    public void OnShow()
    {
        //ApplyTeam();
    }

    public void Close()
    {
        PlayerDataManager.Instance.CloseCharacterPopMenu();
        CheckAutoSetting();
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        UpdataCharacterInfo();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}