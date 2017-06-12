using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface IChat9xServiceInterface : IAgentBase
    {
        /// <summary>
        /// 接受聊天数据
        /// </summary>
        void ChatNotify(int chatType, ulong characterId, string characterName, ChatMessageContent content);
        /// <summary>
        /// 聊天广播
        /// </summary>
        void SyncChatMessage(int chatType, ulong characterId, string characterName, ChatMessageContent content);
        /// <summary>
        /// 广播服务器数据
        /// </summary>
        void BroadcastWorldMessage(int chatType, ulong characterId, string characterName, ChatMessageContent content);
        /// <summary>
        /// 同城频道的广播
        /// </summary>
        void SyncToListCityChatMessage(int chatType, ulong characterId, string characterName, ChatMessageContent Content, string ChannelName);
    }
    public static class Chat9xServiceInterfaceExtension
    {

        public static GMChatOutMessage GMChat(this IChat9xServiceInterface agent, string commond)
        {
            return new GMChatOutMessage(agent, commond);
        }

        public static ChatChatMessageOutMessage ChatChatMessage(this IChat9xServiceInterface agent, int chatType, ChatMessageContent Content, ulong characterId)
        {
            return new ChatChatMessageOutMessage(agent, chatType, Content, characterId);
        }

        public static SendHornMessageOutMessage SendHornMessage(this IChat9xServiceInterface agent, uint serverId, int chatType, ulong characterId, string characterName, ChatMessageContent content)
        {
            return new SendHornMessageOutMessage(agent, serverId, chatType, characterId, characterName, content);
        }

        public static GetRecentcontactsOutMessage GetRecentcontacts(this IChat9xServiceInterface agent, int placeholder)
        {
            return new GetRecentcontactsOutMessage(agent, placeholder);
        }

        public static DeleteRecentcontactsOutMessage DeleteRecentcontacts(this IChat9xServiceInterface agent, ulong characterId)
        {
            return new DeleteRecentcontactsOutMessage(agent, characterId);
        }

        public static EnterChannelOutMessage EnterChannel(this IChat9xServiceInterface agent, ulong channelId, string password)
        {
            return new EnterChannelOutMessage(agent, channelId, password);
        }

        public static LeaveChannelOutMessage LeaveChannel(this IChat9xServiceInterface agent, ulong channelId)
        {
            return new LeaveChannelOutMessage(agent, channelId);
        }

        public static void Init(this IChat9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Chat, (p, list) =>
            {
                switch (p)
                {
                    case 5043:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Chat_ChatNotify_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__>(ms);
                        }
                        break;
                    case 5046:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Chat_SyncChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__>(ms);
                        }
                        break;
                    case 5047:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Chat_BroadcastWorldMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__>(ms);
                        }
                        break;
                    case 5056:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Chat_SyncToListCityChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_Content_string_ChannelName__>(ms);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Chat, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    case 5043:
                        {
                            var data = evt.Data as __RPC_Chat_ChatNotify_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__;
                            agent.ChatNotify(data.ChatType, data.CharacterId, data.CharacterName, data.Content);
                        }
                        break;
                    case 5046:
                        {
                            var data = evt.Data as __RPC_Chat_SyncChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__;
                            agent.SyncChatMessage(data.ChatType, data.CharacterId, data.CharacterName, data.Content);
                        }
                        break;
                    case 5047:
                        {
                            var data = evt.Data as __RPC_Chat_BroadcastWorldMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__;
                            agent.BroadcastWorldMessage(data.ChatType, data.CharacterId, data.CharacterName, data.Content);
                        }
                        break;
                    case 5056:
                        {
                            var data = evt.Data as __RPC_Chat_SyncToListCityChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_Content_string_ChannelName__;
                            agent.SyncToListCityChatMessage(data.ChatType, data.CharacterId, data.CharacterName, data.Content, data.ChannelName);
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class GMChatOutMessage : OutMessage
    {
        public GMChatOutMessage(IAgentBase sender, string commond)
            : base(sender, ServiceType.Chat, 5041)
        {
            Request = new __RPC_Chat_GMChat_ARG_string_commond__();
            Request.Commond=commond;

        }

        public __RPC_Chat_GMChat_ARG_string_commond__ Request { get; private set; }

            private __RPC_Chat_GMChat_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Chat_GMChat_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChatChatMessageOutMessage : OutMessage
    {
        public ChatChatMessageOutMessage(IAgentBase sender, int chatType, ChatMessageContent Content, ulong characterId)
            : base(sender, ServiceType.Chat, 5042)
        {
            Request = new __RPC_Chat_ChatChatMessage_ARG_int32_chatType_ChatMessageContent_Content_uint64_characterId__();
            Request.ChatType=chatType;
            Request.Content=Content;
            Request.CharacterId=characterId;

        }

        public __RPC_Chat_ChatChatMessage_ARG_int32_chatType_ChatMessageContent_Content_uint64_characterId__ Request { get; private set; }

            private __RPC_Chat_ChatChatMessage_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Chat_ChatChatMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChatNotifyOutMessage : OutMessage
    {
        public ChatNotifyOutMessage(IAgentBase sender, int chatType, ulong characterId, string characterName, ChatMessageContent content)
            : base(sender, ServiceType.Chat, 5043)
        {
            Request = new __RPC_Chat_ChatNotify_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__();
            Request.ChatType=chatType;
            Request.CharacterId=characterId;
            Request.CharacterName=characterName;
            Request.Content=content;

        }

        public __RPC_Chat_ChatNotify_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__ Request { get; private set; }


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

    public class SendHornMessageOutMessage : OutMessage
    {
        public SendHornMessageOutMessage(IAgentBase sender, uint serverId, int chatType, ulong characterId, string characterName, ChatMessageContent content)
            : base(sender, ServiceType.Chat, 5044)
        {
            Request = new __RPC_Chat_SendHornMessage_ARG_uint32_serverId_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__();
            Request.ServerId=serverId;
            Request.ChatType=chatType;
            Request.CharacterId=characterId;
            Request.CharacterName=characterName;
            Request.Content=content;

        }

        public __RPC_Chat_SendHornMessage_ARG_uint32_serverId_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__ Request { get; private set; }

            private __RPC_Chat_SendHornMessage_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Chat_SendHornMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncChatMessageOutMessage : OutMessage
    {
        public SyncChatMessageOutMessage(IAgentBase sender, int chatType, ulong characterId, string characterName, ChatMessageContent content)
            : base(sender, ServiceType.Chat, 5046)
        {
            Request = new __RPC_Chat_SyncChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__();
            Request.ChatType=chatType;
            Request.CharacterId=characterId;
            Request.CharacterName=characterName;
            Request.Content=content;

        }

        public __RPC_Chat_SyncChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__ Request { get; private set; }


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

    public class BroadcastWorldMessageOutMessage : OutMessage
    {
        public BroadcastWorldMessageOutMessage(IAgentBase sender, int chatType, ulong characterId, string characterName, ChatMessageContent content)
            : base(sender, ServiceType.Chat, 5047)
        {
            Request = new __RPC_Chat_BroadcastWorldMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__();
            Request.ChatType=chatType;
            Request.CharacterId=characterId;
            Request.CharacterName=characterName;
            Request.Content=content;

        }

        public __RPC_Chat_BroadcastWorldMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_content__ Request { get; private set; }


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

    public class GetRecentcontactsOutMessage : OutMessage
    {
        public GetRecentcontactsOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Chat, 5051)
        {
            Request = new __RPC_Chat_GetRecentcontacts_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Chat_GetRecentcontacts_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Chat_GetRecentcontacts_RET_PlayerHeadInfoMsgList__ mResponse;
            public PlayerHeadInfoMsgList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Chat_GetRecentcontacts_RET_PlayerHeadInfoMsgList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DeleteRecentcontactsOutMessage : OutMessage
    {
        public DeleteRecentcontactsOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Chat, 5052)
        {
            Request = new __RPC_Chat_DeleteRecentcontacts_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Chat_DeleteRecentcontacts_ARG_uint64_characterId__ Request { get; private set; }

            private __RPC_Chat_DeleteRecentcontacts_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Chat_DeleteRecentcontacts_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EnterChannelOutMessage : OutMessage
    {
        public EnterChannelOutMessage(IAgentBase sender, ulong channelId, string password)
            : base(sender, ServiceType.Chat, 5054)
        {
            Request = new __RPC_Chat_EnterChannel_ARG_uint64_channelId_string_password__();
            Request.ChannelId=channelId;
            Request.Password=password;

        }

        public __RPC_Chat_EnterChannel_ARG_uint64_channelId_string_password__ Request { get; private set; }

            private __RPC_Chat_EnterChannel_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Chat_EnterChannel_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class LeaveChannelOutMessage : OutMessage
    {
        public LeaveChannelOutMessage(IAgentBase sender, ulong channelId)
            : base(sender, ServiceType.Chat, 5055)
        {
            Request = new __RPC_Chat_LeaveChannel_ARG_uint64_channelId__();
            Request.ChannelId=channelId;

        }

        public __RPC_Chat_LeaveChannel_ARG_uint64_channelId__ Request { get; private set; }

            private __RPC_Chat_LeaveChannel_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Chat_LeaveChannel_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncToListCityChatMessageOutMessage : OutMessage
    {
        public SyncToListCityChatMessageOutMessage(IAgentBase sender, int chatType, ulong characterId, string characterName, ChatMessageContent Content, string ChannelName)
            : base(sender, ServiceType.Chat, 5056)
        {
            Request = new __RPC_Chat_SyncToListCityChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_Content_string_ChannelName__();
            Request.ChatType=chatType;
            Request.CharacterId=characterId;
            Request.CharacterName=characterName;
            Request.Content=Content;
            Request.ChannelName=ChannelName;

        }

        public __RPC_Chat_SyncToListCityChatMessage_ARG_int32_chatType_uint64_characterId_string_characterName_ChatMessageContent_Content_string_ChannelName__ Request { get; private set; }


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

}
