using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface ITeam9xServiceInterface : IAgentBase
    {
        /// <summary>
        /// 组队信息通知
        /// </summary>
        void NotifyTeamMessage(int type, ulong teamId, ulong characterId);
        /// <summary>
        /// 通知排队成功
        /// </summary>
        void MatchingSuccess(int queueId);
        /// <summary>
        /// 通知排队移出
        /// </summary>
        void NotifyMatchingData(QueueInfo queueInfo);
        /// <summary>
        /// 匹配结果失败通知
        /// </summary>
        void TeamServerMessage(int resultType, string args);
        /// <summary>
        /// 请求提示
        /// </summary>
        void SyncTeamEnterFuben(int fubenId);
        /// <summary>
        /// 队员希望邀请
        /// </summary>
        void MemberWantInvite(int type, string memberName, string toName, ulong toId);
        /// <summary>
        /// 战盟信息通知 			type：0=name1邀请您加入name2的战盟
        /// </summary>
        void SyncAllianceMessage(int type, string name1, int allianceId, string name2);
        /// <summary>
        /// 通知玩家的排队状态
        /// </summary>
        void SendMatchingMessage(int NowCount);
        /// <summary>
        /// 升级Buff
        /// CS int32  				UpgradeAllianceBuff(int32 allianceId,int32 buffId)=7077;
        /// 战盟信息通知 		type：0=name1邀请您加入name2的战盟 1=同意申请 2拒绝申请
        /// </summary>
        void TeamSyncAllianceMessage(int type, string name1, int allianceId, string name2);
        /// <summary>
        /// 战盟信息改变  type = 0 说明战盟升级：param1等级 param2总资金
        /// </summary>
        void ChangeAllianceData(int type, int param1, int param2);
        /// <summary>
        /// 缓存许愿池的团购信息
        /// </summary>
        void SendGroupMessage(StringArray contents);
        /// <summary>
        /// 通知组队进入信息
        /// </summary>
        void NotifyQueueMessage(TeamCharacterMessage tcm);
        /// <summary>
        /// 通知某人进入结果
        /// </summary>
        void NotifyQueueResult(ulong characterId, int result);
        /// <summary>
        /// 服务器主动推送城主信息
        /// </summary>
        void NotifyAllianceWarOccupantData(AllianceWarOccupantData data);
        /// <summary>
        /// 服务器主动推送进攻方信息
        /// </summary>
        void NotifyAllianceWarChallengerData(AllianceWarChallengerData data);
        /// <summary>
        /// 聊天广播
        /// </summary>
        void SyncAllianceChatMessage(int chatType, ulong characterId, string characterName, ChatMessageContent content);
    }
    public static class Team9xServiceInterfaceExtension
    {

        public static TeamMessageOutMessage TeamMessage(this ITeam9xServiceInterface agent, ulong characterId, int type, ulong teamId, ulong otherId)
        {
            return new TeamMessageOutMessage(agent, characterId, type, teamId, otherId);
        }

        public static ApplyTeamOutMessage ApplyTeam(this ITeam9xServiceInterface agent, ulong characterId)
        {
            return new ApplyTeamOutMessage(agent, characterId);
        }

        public static TeamChatMessageOutMessage TeamChatMessage(this ITeam9xServiceInterface agent, int chatType, ChatMessageContent Content, ulong characterId)
        {
            return new TeamChatMessageOutMessage(agent, chatType, Content, characterId);
        }

        public static TeamDungeonLineUpOutMessage TeamDungeonLineUp(this ITeam9xServiceInterface agent, int dungeonId)
        {
            return new TeamDungeonLineUpOutMessage(agent, dungeonId);
        }

        public static MatchingStartOutMessage MatchingStart(this ITeam9xServiceInterface agent, int queueId)
        {
            return new MatchingStartOutMessage(agent, queueId);
        }

        public static MatchingCancelOutMessage MatchingCancel(this ITeam9xServiceInterface agent, int queueId)
        {
            return new MatchingCancelOutMessage(agent, queueId);
        }

        public static MatchingBackOutMessage MatchingBack(this ITeam9xServiceInterface agent, int result)
        {
            return new MatchingBackOutMessage(agent, result);
        }

        public static ApplyQueueDataOutMessage ApplyQueueData(this ITeam9xServiceInterface agent, int placeholder)
        {
            return new ApplyQueueDataOutMessage(agent, placeholder);
        }

        public static TeamEnterFubenOutMessage TeamEnterFuben(this ITeam9xServiceInterface agent, int fubenId, int serverId)
        {
            return new TeamEnterFubenOutMessage(agent, fubenId, serverId);
        }

        public static ResultTeamEnterFubenOutMessage ResultTeamEnterFuben(this ITeam9xServiceInterface agent, int fubenID, int isOk)
        {
            return new ResultTeamEnterFubenOutMessage(agent, fubenID, isOk);
        }

        public static GMTeamOutMessage GMTeam(this ITeam9xServiceInterface agent, string commond)
        {
            return new GMTeamOutMessage(agent, commond);
        }

        public static ApplyAllianceDataOutMessage ApplyAllianceData(this ITeam9xServiceInterface agent, int allianceId)
        {
            return new ApplyAllianceDataOutMessage(agent, allianceId);
        }

        public static ApplyAllianceDataByServerIdOutMessage ApplyAllianceDataByServerId(this ITeam9xServiceInterface agent, int serverId, int type)
        {
            return new ApplyAllianceDataByServerIdOutMessage(agent, serverId, type);
        }

        public static ChangeAllianceNoticeOutMessage ChangeAllianceNotice(this ITeam9xServiceInterface agent, int allianceId, string Content)
        {
            return new ChangeAllianceNoticeOutMessage(agent, allianceId, Content);
        }

        public static GetServerAllianceOutMessage GetServerAlliance(this ITeam9xServiceInterface agent, int serverId)
        {
            return new GetServerAllianceOutMessage(agent, serverId);
        }

        public static ChangeJurisdictionOutMessage ChangeJurisdiction(this ITeam9xServiceInterface agent, int allianceId, ulong guid, int type)
        {
            return new ChangeJurisdictionOutMessage(agent, allianceId, guid, type);
        }

        public static ChangeAllianceAutoJoinOutMessage ChangeAllianceAutoJoin(this ITeam9xServiceInterface agent, int allianceId, int value)
        {
            return new ChangeAllianceAutoJoinOutMessage(agent, allianceId, value);
        }

        public static AllianceAgreeApplyListOutMessage AllianceAgreeApplyList(this ITeam9xServiceInterface agent, int allianceId, int type, Uint64Array guids)
        {
            return new AllianceAgreeApplyListOutMessage(agent, allianceId, type, guids);
        }

        public static ApplyAllianceMissionDataOutMessage ApplyAllianceMissionData(this ITeam9xServiceInterface agent, int allianceId)
        {
            return new ApplyAllianceMissionDataOutMessage(agent, allianceId);
        }

        public static UpgradeAllianceLevelOutMessage UpgradeAllianceLevel(this ITeam9xServiceInterface agent, int allianceId)
        {
            return new UpgradeAllianceLevelOutMessage(agent, allianceId);
        }

        public static ApplyAllianceEnjoyListOutMessage ApplyAllianceEnjoyList(this ITeam9xServiceInterface agent, int allianceId)
        {
            return new ApplyAllianceEnjoyListOutMessage(agent, allianceId);
        }

        public static ApplyAllianceDonationListOutMessage ApplyAllianceDonationList(this ITeam9xServiceInterface agent, int allianceId)
        {
            return new ApplyAllianceDonationListOutMessage(agent, allianceId);
        }

        public static ApplyAllianceWarOccupantDataOutMessage ApplyAllianceWarOccupantData(this ITeam9xServiceInterface agent, int serverId)
        {
            return new ApplyAllianceWarOccupantDataOutMessage(agent, serverId);
        }

        public static ApplyAllianceWarChallengerDataOutMessage ApplyAllianceWarChallengerData(this ITeam9xServiceInterface agent, int serverId)
        {
            return new ApplyAllianceWarChallengerDataOutMessage(agent, serverId);
        }

        public static ApplyAllianceWarDataOutMessage ApplyAllianceWarData(this ITeam9xServiceInterface agent, int serverId)
        {
            return new ApplyAllianceWarDataOutMessage(agent, serverId);
        }

        public static BidAllianceWarOutMessage BidAllianceWar(this ITeam9xServiceInterface agent, int price)
        {
            return new BidAllianceWarOutMessage(agent, price);
        }

        public static EnterAllianceWarOutMessage EnterAllianceWar(this ITeam9xServiceInterface agent, int placeholder)
        {
            return new EnterAllianceWarOutMessage(agent, placeholder);
        }

        public static CSSelectItemAuctionOutMessage CSSelectItemAuction(this ITeam9xServiceInterface agent, int serverId, int type)
        {
            return new CSSelectItemAuctionOutMessage(agent, serverId, type);
        }

        public static void Init(this ITeam9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Team, (p, list) =>
            {
                switch (p)
                {
                    case 7042:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_NotifyTeamMessage_ARG_int32_type_uint64_teamId_uint64_characterId__>(ms);
                        }
                        break;
                    case 7049:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_MatchingSuccess_ARG_int32_queueId__>(ms);
                        }
                        break;
                    case 7050:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_NotifyMatchingData_ARG_QueueInfo_queueInfo__>(ms);
                        }
                        break;
                    case 7052:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_TeamServerMessage_ARG_int32_resultType_string_args__>(ms);
                        }
                        break;
                    case 7056:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_SyncTeamEnterFuben_ARG_int32_fubenId__>(ms);
                        }
                        break;
                    case 7060:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_MemberWantInvite_ARG_int32_type_string_memberName_string_toName_uint64_toId__>(ms);
                        }
                        break;
                    case 7064:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_SyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__>(ms);
                        }
                        break;
                    case 7075:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_SendMatchingMessage_ARG_int32_NowCount__>(ms);
                        }
                        break;
                    case 7078:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_TeamSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__>(ms);
                        }
                        break;
                    case 7080:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_ChangeAllianceData_ARG_int32_type_int32_param1_int32_param2__>(ms);
                        }
                        break;
                    case 7100:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_SendGroupMessage_ARG_StringArray_contents__>(ms);
                        }
                        break;
                    case 7102:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_NotifyQueueMessage_ARG_TeamCharacterMessage_tcm__>(ms);
                        }
                        break;
                    case 7103:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_NotifyQueueResult_ARG_uint64_characterId_int32_result__>(ms);
                        }
                        break;
                    case 7105:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_NotifyAllianceWarOccupantData_ARG_AllianceWarOccupantData_data__>(ms);
                        }
                        break;
                    case 7107:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_NotifyAllianceWarChallengerData_ARG_AllianceWarChallengerData_data__>(ms);
                        }
                        break;
                    case 7113:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Team_SyncAllianceChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__>(ms);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Team, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    case 7042:
                        {
                            var data = evt.Data as __RPC_Team_NotifyTeamMessage_ARG_int32_type_uint64_teamId_uint64_characterId__;
                            agent.NotifyTeamMessage(data.Type, data.TeamId, data.CharacterId);
                        }
                        break;
                    case 7049:
                        {
                            var data = evt.Data as __RPC_Team_MatchingSuccess_ARG_int32_queueId__;
                            agent.MatchingSuccess(data.QueueId);
                        }
                        break;
                    case 7050:
                        {
                            var data = evt.Data as __RPC_Team_NotifyMatchingData_ARG_QueueInfo_queueInfo__;
                            agent.NotifyMatchingData(data.QueueInfo);
                        }
                        break;
                    case 7052:
                        {
                            var data = evt.Data as __RPC_Team_TeamServerMessage_ARG_int32_resultType_string_args__;
                            agent.TeamServerMessage(data.ResultType, data.Args);
                        }
                        break;
                    case 7056:
                        {
                            var data = evt.Data as __RPC_Team_SyncTeamEnterFuben_ARG_int32_fubenId__;
                            agent.SyncTeamEnterFuben(data.FubenId);
                        }
                        break;
                    case 7060:
                        {
                            var data = evt.Data as __RPC_Team_MemberWantInvite_ARG_int32_type_string_memberName_string_toName_uint64_toId__;
                            agent.MemberWantInvite(data.Type, data.MemberName, data.ToName, data.ToId);
                        }
                        break;
                    case 7064:
                        {
                            var data = evt.Data as __RPC_Team_SyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__;
                            agent.SyncAllianceMessage(data.Type, data.Name1, data.AllianceId, data.Name2);
                        }
                        break;
                    case 7075:
                        {
                            var data = evt.Data as __RPC_Team_SendMatchingMessage_ARG_int32_NowCount__;
                            agent.SendMatchingMessage(data.NowCount);
                        }
                        break;
                    case 7078:
                        {
                            var data = evt.Data as __RPC_Team_TeamSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__;
                            agent.TeamSyncAllianceMessage(data.Type, data.Name1, data.AllianceId, data.Name2);
                        }
                        break;
                    case 7080:
                        {
                            var data = evt.Data as __RPC_Team_ChangeAllianceData_ARG_int32_type_int32_param1_int32_param2__;
                            agent.ChangeAllianceData(data.Type, data.Param1, data.Param2);
                        }
                        break;
                    case 7100:
                        {
                            var data = evt.Data as __RPC_Team_SendGroupMessage_ARG_StringArray_contents__;
                            agent.SendGroupMessage(data.Contents);
                        }
                        break;
                    case 7102:
                        {
                            var data = evt.Data as __RPC_Team_NotifyQueueMessage_ARG_TeamCharacterMessage_tcm__;
                            agent.NotifyQueueMessage(data.Tcm);
                        }
                        break;
                    case 7103:
                        {
                            var data = evt.Data as __RPC_Team_NotifyQueueResult_ARG_uint64_characterId_int32_result__;
                            agent.NotifyQueueResult(data.CharacterId, data.Result);
                        }
                        break;
                    case 7105:
                        {
                            var data = evt.Data as __RPC_Team_NotifyAllianceWarOccupantData_ARG_AllianceWarOccupantData_data__;
                            agent.NotifyAllianceWarOccupantData(data.Data);
                        }
                        break;
                    case 7107:
                        {
                            var data = evt.Data as __RPC_Team_NotifyAllianceWarChallengerData_ARG_AllianceWarChallengerData_data__;
                            agent.NotifyAllianceWarChallengerData(data.Data);
                        }
                        break;
                    case 7113:
                        {
                            var data = evt.Data as __RPC_Team_SyncAllianceChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__;
                            agent.SyncAllianceChatMessage(data.ChatType, data.CharacterId, data.CharacterName, data.Content);
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class TeamMessageOutMessage : OutMessage
    {
        public TeamMessageOutMessage(IAgentBase sender, ulong characterId, int type, ulong teamId, ulong otherId)
            : base(sender, ServiceType.Team, 7041)
        {
            Request = new __RPC_Team_TeamMessage_ARG_uint64_characterId_int32_type_uint64_teamId_uint64_otherId__();
            Request.CharacterId=characterId;
            Request.Type=type;
            Request.TeamId=teamId;
            Request.OtherId=otherId;

        }

        public __RPC_Team_TeamMessage_ARG_uint64_characterId_int32_type_uint64_teamId_uint64_otherId__ Request { get; private set; }

            private __RPC_Team_TeamMessage_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_TeamMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyTeamMessageOutMessage : OutMessage
    {
        public NotifyTeamMessageOutMessage(IAgentBase sender, int type, ulong teamId, ulong characterId)
            : base(sender, ServiceType.Team, 7042)
        {
            Request = new __RPC_Team_NotifyTeamMessage_ARG_int32_type_uint64_teamId_uint64_characterId__();
            Request.Type=type;
            Request.TeamId=teamId;
            Request.CharacterId=characterId;

        }

        public __RPC_Team_NotifyTeamMessage_ARG_int32_type_uint64_teamId_uint64_characterId__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyTeamOutMessage : OutMessage
    {
        public ApplyTeamOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Team, 7043)
        {
            Request = new __RPC_Team_ApplyTeam_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Team_ApplyTeam_ARG_uint64_characterId__ Request { get; private set; }

            private __RPC_Team_ApplyTeam_RET_TeamMsg__ mResponse;
            public TeamMsg Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyTeam_RET_TeamMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class TeamChatMessageOutMessage : OutMessage
    {
        public TeamChatMessageOutMessage(IAgentBase sender, int chatType, ChatMessageContent Content, ulong characterId)
            : base(sender, ServiceType.Team, 7044)
        {
            Request = new __RPC_Team_TeamChatMessage_ARG_int32_chatType_ChatMessageContent_Content_uint64_characterId__();
            Request.ChatType=chatType;
            Request.Content=Content;
            Request.CharacterId=characterId;

        }

        public __RPC_Team_TeamChatMessage_ARG_int32_chatType_ChatMessageContent_Content_uint64_characterId__ Request { get; private set; }

            private __RPC_Team_TeamChatMessage_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_TeamChatMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class TeamDungeonLineUpOutMessage : OutMessage
    {
        public TeamDungeonLineUpOutMessage(IAgentBase sender, int dungeonId)
            : base(sender, ServiceType.Team, 7045)
        {
            Request = new __RPC_Team_TeamDungeonLineUp_ARG_int32_dungeonId__();
            Request.DungeonId=dungeonId;

        }

        public __RPC_Team_TeamDungeonLineUp_ARG_int32_dungeonId__ Request { get; private set; }

            private __RPC_Team_TeamDungeonLineUp_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_TeamDungeonLineUp_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class MatchingStartOutMessage : OutMessage
    {
        public MatchingStartOutMessage(IAgentBase sender, int queueId)
            : base(sender, ServiceType.Team, 7047)
        {
            Request = new __RPC_Team_MatchingStart_ARG_int32_queueId__();
            Request.QueueId=queueId;

        }

        public __RPC_Team_MatchingStart_ARG_int32_queueId__ Request { get; private set; }

            private __RPC_Team_MatchingStart_RET_MatchingResult__ mResponse;
            public MatchingResult Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_MatchingStart_RET_MatchingResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class MatchingCancelOutMessage : OutMessage
    {
        public MatchingCancelOutMessage(IAgentBase sender, int queueId)
            : base(sender, ServiceType.Team, 7048)
        {
            Request = new __RPC_Team_MatchingCancel_ARG_int32_queueId__();
            Request.QueueId=queueId;

        }

        public __RPC_Team_MatchingCancel_ARG_int32_queueId__ Request { get; private set; }

            private __RPC_Team_MatchingCancel_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_MatchingCancel_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class MatchingSuccessOutMessage : OutMessage
    {
        public MatchingSuccessOutMessage(IAgentBase sender, int queueId)
            : base(sender, ServiceType.Team, 7049)
        {
            Request = new __RPC_Team_MatchingSuccess_ARG_int32_queueId__();
            Request.QueueId=queueId;

        }

        public __RPC_Team_MatchingSuccess_ARG_int32_queueId__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class NotifyMatchingDataOutMessage : OutMessage
    {
        public NotifyMatchingDataOutMessage(IAgentBase sender, QueueInfo queueInfo)
            : base(sender, ServiceType.Team, 7050)
        {
            Request = new __RPC_Team_NotifyMatchingData_ARG_QueueInfo_queueInfo__();
            Request.QueueInfo=queueInfo;

        }

        public __RPC_Team_NotifyMatchingData_ARG_QueueInfo_queueInfo__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class MatchingBackOutMessage : OutMessage
    {
        public MatchingBackOutMessage(IAgentBase sender, int result)
            : base(sender, ServiceType.Team, 7051)
        {
            Request = new __RPC_Team_MatchingBack_ARG_int32_result__();
            Request.Result=result;

        }

        public __RPC_Team_MatchingBack_ARG_int32_result__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class TeamServerMessageOutMessage : OutMessage
    {
        public TeamServerMessageOutMessage(IAgentBase sender, int resultType, string args)
            : base(sender, ServiceType.Team, 7052)
        {
            Request = new __RPC_Team_TeamServerMessage_ARG_int32_resultType_string_args__();
            Request.ResultType=resultType;
            Request.Args=args;

        }

        public __RPC_Team_TeamServerMessage_ARG_int32_resultType_string_args__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyQueueDataOutMessage : OutMessage
    {
        public ApplyQueueDataOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Team, 7054)
        {
            Request = new __RPC_Team_ApplyQueueData_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Team_ApplyQueueData_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Team_ApplyQueueData_RET_QueueInfo__ mResponse;
            public QueueInfo Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyQueueData_RET_QueueInfo__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class TeamEnterFubenOutMessage : OutMessage
    {
        public TeamEnterFubenOutMessage(IAgentBase sender, int fubenId, int serverId)
            : base(sender, ServiceType.Team, 7055)
        {
            Request = new __RPC_Team_TeamEnterFuben_ARG_int32_fubenId_int32_serverId__();
            Request.FubenId=fubenId;
            Request.ServerId=serverId;

        }

        public __RPC_Team_TeamEnterFuben_ARG_int32_fubenId_int32_serverId__ Request { get; private set; }

            private __RPC_Team_TeamEnterFuben_RET_Uint64Array__ mResponse;
            public Uint64Array Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_TeamEnterFuben_RET_Uint64Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncTeamEnterFubenOutMessage : OutMessage
    {
        public SyncTeamEnterFubenOutMessage(IAgentBase sender, int fubenId)
            : base(sender, ServiceType.Team, 7056)
        {
            Request = new __RPC_Team_SyncTeamEnterFuben_ARG_int32_fubenId__();
            Request.FubenId=fubenId;

        }

        public __RPC_Team_SyncTeamEnterFuben_ARG_int32_fubenId__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ResultTeamEnterFubenOutMessage : OutMessage
    {
        public ResultTeamEnterFubenOutMessage(IAgentBase sender, int fubenID, int isOk)
            : base(sender, ServiceType.Team, 7057)
        {
            Request = new __RPC_Team_ResultTeamEnterFuben_ARG_int32_fubenID_int32_isOk__();
            Request.FubenID=fubenID;
            Request.IsOk=isOk;

        }

        public __RPC_Team_ResultTeamEnterFuben_ARG_int32_fubenID_int32_isOk__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class GMTeamOutMessage : OutMessage
    {
        public GMTeamOutMessage(IAgentBase sender, string commond)
            : base(sender, ServiceType.Team, 7059)
        {
            Request = new __RPC_Team_GMTeam_ARG_string_commond__();
            Request.Commond=commond;

        }

        public __RPC_Team_GMTeam_ARG_string_commond__ Request { get; private set; }

            private __RPC_Team_GMTeam_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_GMTeam_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class MemberWantInviteOutMessage : OutMessage
    {
        public MemberWantInviteOutMessage(IAgentBase sender, int type, string memberName, string toName, ulong toId)
            : base(sender, ServiceType.Team, 7060)
        {
            Request = new __RPC_Team_MemberWantInvite_ARG_int32_type_string_memberName_string_toName_uint64_toId__();
            Request.Type=type;
            Request.MemberName=memberName;
            Request.ToName=toName;
            Request.ToId=toId;

        }

        public __RPC_Team_MemberWantInvite_ARG_int32_type_string_memberName_string_toName_uint64_toId__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class SyncAllianceMessageOutMessage : OutMessage
    {
        public SyncAllianceMessageOutMessage(IAgentBase sender, int type, string name1, int allianceId, string name2)
            : base(sender, ServiceType.Team, 7064)
        {
            Request = new __RPC_Team_SyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__();
            Request.Type=type;
            Request.Name1=name1;
            Request.AllianceId=allianceId;
            Request.Name2=name2;

        }

        public __RPC_Team_SyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyAllianceDataOutMessage : OutMessage
    {
        public ApplyAllianceDataOutMessage(IAgentBase sender, int allianceId)
            : base(sender, ServiceType.Team, 7065)
        {
            Request = new __RPC_Team_ApplyAllianceData_ARG_int32_allianceId__();
            Request.AllianceId=allianceId;

        }

        public __RPC_Team_ApplyAllianceData_ARG_int32_allianceId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceData_RET_AllianceData__ mResponse;
            public AllianceData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceData_RET_AllianceData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyAllianceDataByServerIdOutMessage : OutMessage
    {
        public ApplyAllianceDataByServerIdOutMessage(IAgentBase sender, int serverId, int type)
            : base(sender, ServiceType.Team, 7066)
        {
            Request = new __RPC_Team_ApplyAllianceDataByServerId_ARG_int32_serverId_int32_type__();
            Request.ServerId=serverId;
            Request.Type=type;

        }

        public __RPC_Team_ApplyAllianceDataByServerId_ARG_int32_serverId_int32_type__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceDataByServerId_RET_AllianceData__ mResponse;
            public AllianceData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceDataByServerId_RET_AllianceData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChangeAllianceNoticeOutMessage : OutMessage
    {
        public ChangeAllianceNoticeOutMessage(IAgentBase sender, int allianceId, string Content)
            : base(sender, ServiceType.Team, 7067)
        {
            Request = new __RPC_Team_ChangeAllianceNotice_ARG_int32_allianceId_string_Content__();
            Request.AllianceId=allianceId;
            Request.Content=Content;

        }

        public __RPC_Team_ChangeAllianceNotice_ARG_int32_allianceId_string_Content__ Request { get; private set; }

            private __RPC_Team_ChangeAllianceNotice_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ChangeAllianceNotice_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetServerAllianceOutMessage : OutMessage
    {
        public GetServerAllianceOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Team, 7068)
        {
            Request = new __RPC_Team_GetServerAlliance_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Team_GetServerAlliance_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Team_GetServerAlliance_RET_AllianceSimpleDataList__ mResponse;
            public AllianceSimpleDataList Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_GetServerAlliance_RET_AllianceSimpleDataList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChangeJurisdictionOutMessage : OutMessage
    {
        public ChangeJurisdictionOutMessage(IAgentBase sender, int allianceId, ulong guid, int type)
            : base(sender, ServiceType.Team, 7069)
        {
            Request = new __RPC_Team_ChangeJurisdiction_ARG_int32_allianceId_uint64_guid_int32_type__();
            Request.AllianceId=allianceId;
            Request.Guid=guid;
            Request.Type=type;

        }

        public __RPC_Team_ChangeJurisdiction_ARG_int32_allianceId_uint64_guid_int32_type__ Request { get; private set; }

            private __RPC_Team_ChangeJurisdiction_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ChangeJurisdiction_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChangeAllianceAutoJoinOutMessage : OutMessage
    {
        public ChangeAllianceAutoJoinOutMessage(IAgentBase sender, int allianceId, int value)
            : base(sender, ServiceType.Team, 7070)
        {
            Request = new __RPC_Team_ChangeAllianceAutoJoin_ARG_int32_allianceId_int32_value__();
            Request.AllianceId=allianceId;
            Request.Value=value;

        }

        public __RPC_Team_ChangeAllianceAutoJoin_ARG_int32_allianceId_int32_value__ Request { get; private set; }

            private __RPC_Team_ChangeAllianceAutoJoin_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ChangeAllianceAutoJoin_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AllianceAgreeApplyListOutMessage : OutMessage
    {
        public AllianceAgreeApplyListOutMessage(IAgentBase sender, int allianceId, int type, Uint64Array guids)
            : base(sender, ServiceType.Team, 7071)
        {
            Request = new __RPC_Team_AllianceAgreeApplyList_ARG_int32_allianceId_int32_type_Uint64Array_guids__();
            Request.AllianceId=allianceId;
            Request.Type=type;
            Request.Guids=guids;

        }

        public __RPC_Team_AllianceAgreeApplyList_ARG_int32_allianceId_int32_type_Uint64Array_guids__ Request { get; private set; }

            private __RPC_Team_AllianceAgreeApplyList_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_AllianceAgreeApplyList_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SendMatchingMessageOutMessage : OutMessage
    {
        public SendMatchingMessageOutMessage(IAgentBase sender, int NowCount)
            : base(sender, ServiceType.Team, 7075)
        {
            Request = new __RPC_Team_SendMatchingMessage_ARG_int32_NowCount__();
            Request.NowCount=NowCount;

        }

        public __RPC_Team_SendMatchingMessage_ARG_int32_NowCount__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class TeamSyncAllianceMessageOutMessage : OutMessage
    {
        public TeamSyncAllianceMessageOutMessage(IAgentBase sender, int type, string name1, int allianceId, string name2)
            : base(sender, ServiceType.Team, 7078)
        {
            Request = new __RPC_Team_TeamSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__();
            Request.Type=type;
            Request.Name1=name1;
            Request.AllianceId=allianceId;
            Request.Name2=name2;

        }

        public __RPC_Team_TeamSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ChangeAllianceDataOutMessage : OutMessage
    {
        public ChangeAllianceDataOutMessage(IAgentBase sender, int type, int param1, int param2)
            : base(sender, ServiceType.Team, 7080)
        {
            Request = new __RPC_Team_ChangeAllianceData_ARG_int32_type_int32_param1_int32_param2__();
            Request.Type=type;
            Request.Param1=param1;
            Request.Param2=param2;

        }

        public __RPC_Team_ChangeAllianceData_ARG_int32_type_int32_param1_int32_param2__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyAllianceMissionDataOutMessage : OutMessage
    {
        public ApplyAllianceMissionDataOutMessage(IAgentBase sender, int allianceId)
            : base(sender, ServiceType.Team, 7081)
        {
            Request = new __RPC_Team_ApplyAllianceMissionData_ARG_int32_allianceId__();
            Request.AllianceId=allianceId;

        }

        public __RPC_Team_ApplyAllianceMissionData_ARG_int32_allianceId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceMissionData_RET_AllianceMissionData__ mResponse;
            public AllianceMissionData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceMissionData_RET_AllianceMissionData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UpgradeAllianceLevelOutMessage : OutMessage
    {
        public UpgradeAllianceLevelOutMessage(IAgentBase sender, int allianceId)
            : base(sender, ServiceType.Team, 7082)
        {
            Request = new __RPC_Team_UpgradeAllianceLevel_ARG_int32_allianceId__();
            Request.AllianceId=allianceId;

        }

        public __RPC_Team_UpgradeAllianceLevel_ARG_int32_allianceId__ Request { get; private set; }

            private __RPC_Team_UpgradeAllianceLevel_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_UpgradeAllianceLevel_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyAllianceEnjoyListOutMessage : OutMessage
    {
        public ApplyAllianceEnjoyListOutMessage(IAgentBase sender, int allianceId)
            : base(sender, ServiceType.Team, 7090)
        {
            Request = new __RPC_Team_ApplyAllianceEnjoyList_ARG_int32_allianceId__();
            Request.AllianceId=allianceId;

        }

        public __RPC_Team_ApplyAllianceEnjoyList_ARG_int32_allianceId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceEnjoyList_RET_AllianceEnjoyData__ mResponse;
            public AllianceEnjoyData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceEnjoyList_RET_AllianceEnjoyData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyAllianceDonationListOutMessage : OutMessage
    {
        public ApplyAllianceDonationListOutMessage(IAgentBase sender, int allianceId)
            : base(sender, ServiceType.Team, 7091)
        {
            Request = new __RPC_Team_ApplyAllianceDonationList_ARG_int32_allianceId__();
            Request.AllianceId=allianceId;

        }

        public __RPC_Team_ApplyAllianceDonationList_ARG_int32_allianceId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceDonationList_RET_AllianceDonationData__ mResponse;
            public AllianceDonationData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceDonationList_RET_AllianceDonationData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SendGroupMessageOutMessage : OutMessage
    {
        public SendGroupMessageOutMessage(IAgentBase sender, StringArray contents)
            : base(sender, ServiceType.Team, 7100)
        {
            Request = new __RPC_Team_SendGroupMessage_ARG_StringArray_contents__();
            Request.Contents=contents;

        }

        public __RPC_Team_SendGroupMessage_ARG_StringArray_contents__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class NotifyQueueMessageOutMessage : OutMessage
    {
        public NotifyQueueMessageOutMessage(IAgentBase sender, TeamCharacterMessage tcm)
            : base(sender, ServiceType.Team, 7102)
        {
            Request = new __RPC_Team_NotifyQueueMessage_ARG_TeamCharacterMessage_tcm__();
            Request.Tcm=tcm;

        }

        public __RPC_Team_NotifyQueueMessage_ARG_TeamCharacterMessage_tcm__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class NotifyQueueResultOutMessage : OutMessage
    {
        public NotifyQueueResultOutMessage(IAgentBase sender, ulong characterId, int result)
            : base(sender, ServiceType.Team, 7103)
        {
            Request = new __RPC_Team_NotifyQueueResult_ARG_uint64_characterId_int32_result__();
            Request.CharacterId=characterId;
            Request.Result=result;

        }

        public __RPC_Team_NotifyQueueResult_ARG_uint64_characterId_int32_result__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyAllianceWarOccupantDataOutMessage : OutMessage
    {
        public ApplyAllianceWarOccupantDataOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Team, 7104)
        {
            Request = new __RPC_Team_ApplyAllianceWarOccupantData_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Team_ApplyAllianceWarOccupantData_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceWarOccupantData_RET_AllianceWarOccupantData__ mResponse;
            public AllianceWarOccupantData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceWarOccupantData_RET_AllianceWarOccupantData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyAllianceWarOccupantDataOutMessage : OutMessage
    {
        public NotifyAllianceWarOccupantDataOutMessage(IAgentBase sender, AllianceWarOccupantData data)
            : base(sender, ServiceType.Team, 7105)
        {
            Request = new __RPC_Team_NotifyAllianceWarOccupantData_ARG_AllianceWarOccupantData_data__();
            Request.Data=data;

        }

        public __RPC_Team_NotifyAllianceWarOccupantData_ARG_AllianceWarOccupantData_data__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyAllianceWarChallengerDataOutMessage : OutMessage
    {
        public ApplyAllianceWarChallengerDataOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Team, 7106)
        {
            Request = new __RPC_Team_ApplyAllianceWarChallengerData_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Team_ApplyAllianceWarChallengerData_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceWarChallengerData_RET_AllianceWarChallengerData__ mResponse;
            public AllianceWarChallengerData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceWarChallengerData_RET_AllianceWarChallengerData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyAllianceWarChallengerDataOutMessage : OutMessage
    {
        public NotifyAllianceWarChallengerDataOutMessage(IAgentBase sender, AllianceWarChallengerData data)
            : base(sender, ServiceType.Team, 7107)
        {
            Request = new __RPC_Team_NotifyAllianceWarChallengerData_ARG_AllianceWarChallengerData_data__();
            Request.Data=data;

        }

        public __RPC_Team_NotifyAllianceWarChallengerData_ARG_AllianceWarChallengerData_data__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class ApplyAllianceWarDataOutMessage : OutMessage
    {
        public ApplyAllianceWarDataOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Team, 7108)
        {
            Request = new __RPC_Team_ApplyAllianceWarData_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Team_ApplyAllianceWarData_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Team_ApplyAllianceWarData_RET_AllianceWarData__ mResponse;
            public AllianceWarData Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_ApplyAllianceWarData_RET_AllianceWarData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class BidAllianceWarOutMessage : OutMessage
    {
        public BidAllianceWarOutMessage(IAgentBase sender, int price)
            : base(sender, ServiceType.Team, 7109)
        {
            Request = new __RPC_Team_BidAllianceWar_ARG_int32_price__();
            Request.Price=price;

        }

        public __RPC_Team_BidAllianceWar_ARG_int32_price__ Request { get; private set; }

            private __RPC_Team_BidAllianceWar_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_BidAllianceWar_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EnterAllianceWarOutMessage : OutMessage
    {
        public EnterAllianceWarOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Team, 7110)
        {
            Request = new __RPC_Team_EnterAllianceWar_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Team_EnterAllianceWar_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Team_EnterAllianceWar_RET_int32__ mResponse;
            public int Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_EnterAllianceWar_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncAllianceChatMessageOutMessage : OutMessage
    {
        public SyncAllianceChatMessageOutMessage(IAgentBase sender, int chatType, ulong characterId, string characterName, ChatMessageContent content)
            : base(sender, ServiceType.Team, 7113)
        {
            Request = new __RPC_Team_SyncAllianceChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__();
            Request.ChatType=chatType;
            Request.CharacterId=characterId;
            Request.CharacterName=characterName;
            Request.Content=content;

        }

        public __RPC_Team_SyncAllianceChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__ Request { get; private set; }


        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
        }
        public override bool HasReturnValue { get { return false; } }
    }

    public class CSSelectItemAuctionOutMessage : OutMessage
    {
        public CSSelectItemAuctionOutMessage(IAgentBase sender, int serverId, int type)
            : base(sender, ServiceType.Team, 7116)
        {
            Request = new __RPC_Team_CSSelectItemAuction_ARG_int32_serverId_int32_type__();
            Request.ServerId=serverId;
            Request.Type=type;

        }

        public __RPC_Team_CSSelectItemAuction_ARG_int32_serverId_int32_type__ Request { get; private set; }

            private __RPC_Team_CSSelectItemAuction_RET_AuctionItemList__ mResponse;
            public AuctionItemList Response { get { return mResponse.ReturnValue; } }

        protected override byte[] Serialize(MemoryStream s)
        {
            Serializer.Serialize(s, Request);
            return s.ToArray();
        }

        public override void SetResponse(uint error, byte[] data)
        {
            if (data != null)
            {
                var ms = new MemoryStream(data, false);
                mResponse = Serializer.Deserialize<__RPC_Team_CSSelectItemAuction_RET_AuctionItemList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

}
