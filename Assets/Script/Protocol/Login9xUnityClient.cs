using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface ILogin9xServiceInterface : IAgentBase
    {
        /// <summary>
        /// </summary>
        void Kick(int type);
        /// <summary>
        ///  通知其他服务器，这个character已经退出了，调用了这个函数以后，Broker和其他服务器中，关于这个Character的数据都会被清理，相当于这个character下线了
        /// </summary>
        void Logout(ulong characterId);
        /// <summary>
        /// 通知排队名次
        /// </summary>
        void NotifyQueueIndex(int index);
        /// <summary>
        /// 通知登录排队成功
        /// </summary>
        void Discard0(PlayerLoginData plData);
        /// <summary>
        /// 通用，通知排队成功
        /// </summary>
        void NotifyQueueSuccess(QueueSuccessData data);
        /// <summary>
        /// 通知重连
        /// </summary>
        void NotifyReConnet(int result);
    }
    public static class Login9xServiceInterfaceExtension
    {

        public static PlayerLoginByUserNamePasswordOutMessage PlayerLoginByUserNamePassword(this ILogin9xServiceInterface agent, string username, string password)
        {
            return new PlayerLoginByUserNamePasswordOutMessage(agent, username, password);
        }

        public static PlayerLoginByThirdKeyOutMessage PlayerLoginByThirdKey(this ILogin9xServiceInterface agent, string platform, string channel, string userId, string accessToken)
        {
            return new PlayerLoginByThirdKeyOutMessage(agent, platform, channel, userId, accessToken);
        }

        public static PlayerSelectServerIdOutMessage PlayerSelectServerId(this ILogin9xServiceInterface agent, int serverId)
        {
            return new PlayerSelectServerIdOutMessage(agent, serverId);
        }

        public static CreateCharacterOutMessage CreateCharacter(this ILogin9xServiceInterface agent, int serverId, int type, string name)
        {
            return new CreateCharacterOutMessage(agent, serverId, type, name);
        }

        public static EnterGameOutMessage EnterGame(this ILogin9xServiceInterface agent, ulong characterId)
        {
            return new EnterGameOutMessage(agent, characterId);
        }

        public static SyncTimeOutMessage SyncTime(this ILogin9xServiceInterface agent, int placeholder)
        {
            return new SyncTimeOutMessage(agent, placeholder);
        }

        public static GetServerListOutMessage GetServerList(this ILogin9xServiceInterface agent, int placeholder)
        {
            return new GetServerListOutMessage(agent, placeholder);
        }

        public static ExitLoginOutMessage ExitLogin(this ILogin9xServiceInterface agent, int placeholder)
        {
            return new ExitLoginOutMessage(agent, placeholder);
        }

        public static ExitSelectCharacterOutMessage ExitSelectCharacter(this ILogin9xServiceInterface agent, ulong characterId)
        {
            return new ExitSelectCharacterOutMessage(agent, characterId);
        }

        public static QueryServerTimezoneOutMessage QueryServerTimezone(this ILogin9xServiceInterface agent, int placeholder)
        {
            return new QueryServerTimezoneOutMessage(agent, placeholder);
        }

        public static GateDisconnectOutMessage GateDisconnect(this ILogin9xServiceInterface agent, ulong clientId, ulong characterId)
        {
            return new GateDisconnectOutMessage(agent, clientId, characterId);
        }

        public static ReConnetOutMessage ReConnet(this ILogin9xServiceInterface agent, ulong clientId, ulong characterId)
        {
            return new ReConnetOutMessage(agent, clientId, characterId);
        }

        public static SendDeviceUdidOutMessage SendDeviceUdid(this ILogin9xServiceInterface agent, string deviceUdid)
        {
            return new SendDeviceUdidOutMessage(agent, deviceUdid);
        }

        public static void Init(this ILogin9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Login, (p, list) =>
            {
                switch (p)
                {
                    case 2010:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Login_Kick_ARG_int32_type__>(ms);
                        }
                        break;
                    case 2011:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Login_Logout_ARG_uint64_characterId__>(ms);
                        }
                        break;
                    case 2021:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Login_NotifyQueueIndex_ARG_int32_index__>(ms);
                        }
                        break;
                    case 2022:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Login_Discard0_ARG_PlayerLoginData_plData__>(ms);
                        }
                        break;
                    case 2030:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Login_NotifyQueueSuccess_ARG_QueueSuccessData_data__>(ms);
                        }
                        break;
                    case 2035:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Login_NotifyReConnet_ARG_int32_result__>(ms);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Login, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    case 2010:
                        {
                            var data = evt.Data as __RPC_Login_Kick_ARG_int32_type__;
                            agent.Kick(data.Type);
                        }
                        break;
                    case 2011:
                        {
                            var data = evt.Data as __RPC_Login_Logout_ARG_uint64_characterId__;
                            agent.Logout(data.CharacterId);
                        }
                        break;
                    case 2021:
                        {
                            var data = evt.Data as __RPC_Login_NotifyQueueIndex_ARG_int32_index__;
                            agent.NotifyQueueIndex(data.Index);
                        }
                        break;
                    case 2022:
                        {
                            var data = evt.Data as __RPC_Login_Discard0_ARG_PlayerLoginData_plData__;
                            agent.Discard0(data.PlData);
                        }
                        break;
                    case 2030:
                        {
                            var data = evt.Data as __RPC_Login_NotifyQueueSuccess_ARG_QueueSuccessData_data__;
                            agent.NotifyQueueSuccess(data.Data);
                        }
                        break;
                    case 2035:
                        {
                            var data = evt.Data as __RPC_Login_NotifyReConnet_ARG_int32_result__;
                            agent.NotifyReConnet(data.Result);
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class PlayerLoginByUserNamePasswordOutMessage : OutMessage
    {
        public PlayerLoginByUserNamePasswordOutMessage(IAgentBase sender, string username, string password)
            : base(sender, ServiceType.Login, 2001)
        {
            Request = new __RPC_Login_PlayerLoginByUserNamePassword_ARG_string_username_string_password__();
            Request.Username=username;
            Request.Password=password;

        }

        public __RPC_Login_PlayerLoginByUserNamePassword_ARG_string_username_string_password__ Request { get; private set; }

            private __RPC_Login_PlayerLoginByUserNamePassword_RET_PlayerLoginData__ mResponse;
            public PlayerLoginData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_PlayerLoginByUserNamePassword_RET_PlayerLoginData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class PlayerLoginByThirdKeyOutMessage : OutMessage
    {
        public PlayerLoginByThirdKeyOutMessage(IAgentBase sender, string platform, string channel, string userId, string accessToken)
            : base(sender, ServiceType.Login, 2002)
        {
            Request = new __RPC_Login_PlayerLoginByThirdKey_ARG_string_platform_string_channel_string_userId_string_accessToken__();
            Request.Platform=platform;
            Request.Channel=channel;
            Request.UserId=userId;
            Request.AccessToken=accessToken;

        }

        public __RPC_Login_PlayerLoginByThirdKey_ARG_string_platform_string_channel_string_userId_string_accessToken__ Request { get; private set; }

            private __RPC_Login_PlayerLoginByThirdKey_RET_PlayerLoginData__ mResponse;
            public PlayerLoginData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_PlayerLoginByThirdKey_RET_PlayerLoginData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class PlayerSelectServerIdOutMessage : OutMessage
    {
        public PlayerSelectServerIdOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Login, 2003)
        {
            Request = new __RPC_Login_PlayerSelectServerId_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Login_PlayerSelectServerId_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Login_PlayerSelectServerId_RET_CharacterListLoginMsg__ mResponse;
            public CharacterListLoginMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_PlayerSelectServerId_RET_CharacterListLoginMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class KickOutMessage : OutMessage
    {
        public KickOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Login, 2010)
        {
            Request = new __RPC_Login_Kick_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Login_Kick_ARG_int32_type__ Request { get; private set; }


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

    public class LogoutOutMessage : OutMessage
    {
        public LogoutOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Login, 2011)
        {
            Request = new __RPC_Login_Logout_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Login_Logout_ARG_uint64_characterId__ Request { get; private set; }


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

    public class CreateCharacterOutMessage : OutMessage
    {
        public CreateCharacterOutMessage(IAgentBase sender, int serverId, int type, string name)
            : base(sender, ServiceType.Login, 2012)
        {
            Request = new __RPC_Login_CreateCharacter_ARG_int32_serverId_int32_type_string_name__();
            Request.ServerId=serverId;
            Request.Type=type;
            Request.Name=name;

        }

        public __RPC_Login_CreateCharacter_ARG_int32_serverId_int32_type_string_name__ Request { get; private set; }

            private __RPC_Login_CreateCharacter_RET_CharacterListLoginMsg__ mResponse;
            public CharacterListLoginMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_CreateCharacter_RET_CharacterListLoginMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EnterGameOutMessage : OutMessage
    {
        public EnterGameOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Login, 2013)
        {
            Request = new __RPC_Login_EnterGame_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Login_EnterGame_ARG_uint64_characterId__ Request { get; private set; }

            private __RPC_Login_EnterGame_RET_EnterGameData__ mResponse;
            public EnterGameData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_EnterGame_RET_EnterGameData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncTimeOutMessage : OutMessage
    {
        public SyncTimeOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Login, 2014)
        {
            Request = new __RPC_Login_SyncTime_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Login_SyncTime_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Login_SyncTime_RET_uint64__ mResponse;
            public ulong Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_SyncTime_RET_uint64__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetServerListOutMessage : OutMessage
    {
        public GetServerListOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Login, 2015)
        {
            Request = new __RPC_Login_GetServerList_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Login_GetServerList_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Login_GetServerList_RET_ServerListData__ mResponse;
            public ServerListData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_GetServerList_RET_ServerListData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyQueueIndexOutMessage : OutMessage
    {
        public NotifyQueueIndexOutMessage(IAgentBase sender, int index)
            : base(sender, ServiceType.Login, 2021)
        {
            Request = new __RPC_Login_NotifyQueueIndex_ARG_int32_index__();
            Request.Index=index;

        }

        public __RPC_Login_NotifyQueueIndex_ARG_int32_index__ Request { get; private set; }


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

    public class Discard0OutMessage : OutMessage
    {
        public Discard0OutMessage(IAgentBase sender, PlayerLoginData plData)
            : base(sender, ServiceType.Login, 2022)
        {
            Request = new __RPC_Login_Discard0_ARG_PlayerLoginData_plData__();
            Request.PlData=plData;

        }

        public __RPC_Login_Discard0_ARG_PlayerLoginData_plData__ Request { get; private set; }


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

    public class ExitLoginOutMessage : OutMessage
    {
        public ExitLoginOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Login, 2025)
        {
            Request = new __RPC_Login_ExitLogin_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Login_ExitLogin_ARG_int32_placeholder__ Request { get; private set; }


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

    public class ExitSelectCharacterOutMessage : OutMessage
    {
        public ExitSelectCharacterOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Login, 2026)
        {
            Request = new __RPC_Login_ExitSelectCharacter_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Login_ExitSelectCharacter_ARG_uint64_characterId__ Request { get; private set; }

            private __RPC_Login_ExitSelectCharacter_RET_CharacterListLoginMsg__ mResponse;
            public CharacterListLoginMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Login_ExitSelectCharacter_RET_CharacterListLoginMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyQueueSuccessOutMessage : OutMessage
    {
        public NotifyQueueSuccessOutMessage(IAgentBase sender, QueueSuccessData data)
            : base(sender, ServiceType.Login, 2030)
        {
            Request = new __RPC_Login_NotifyQueueSuccess_ARG_QueueSuccessData_data__();
            Request.Data=data;

        }

        public __RPC_Login_NotifyQueueSuccess_ARG_QueueSuccessData_data__ Request { get; private set; }


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

    public class QueryServerTimezoneOutMessage : OutMessage
    {
        public QueryServerTimezoneOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Login, 2031)
        {
            Request = new __RPC_Login_QueryServerTimezone_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Login_QueryServerTimezone_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Login_QueryServerTimezone_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Login_QueryServerTimezone_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GateDisconnectOutMessage : OutMessage
    {
        public GateDisconnectOutMessage(IAgentBase sender, ulong clientId, ulong characterId)
            : base(sender, ServiceType.Login, 2033)
        {
            Request = new __RPC_Login_GateDisconnect_ARG_uint64_clientId_uint64_characterId__();
            Request.ClientId=clientId;
            Request.CharacterId=characterId;

        }

        public __RPC_Login_GateDisconnect_ARG_uint64_clientId_uint64_characterId__ Request { get; private set; }


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

    public class ReConnetOutMessage : OutMessage
    {
        public ReConnetOutMessage(IAgentBase sender, ulong clientId, ulong characterId)
            : base(sender, ServiceType.Login, 2034)
        {
            Request = new __RPC_Login_ReConnet_ARG_uint64_clientId_uint64_characterId__();
            Request.ClientId=clientId;
            Request.CharacterId=characterId;

        }

        public __RPC_Login_ReConnet_ARG_uint64_clientId_uint64_characterId__ Request { get; private set; }

            private __RPC_Login_ReConnet_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Login_ReConnet_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyReConnetOutMessage : OutMessage
    {
        public NotifyReConnetOutMessage(IAgentBase sender, int result)
            : base(sender, ServiceType.Login, 2035)
        {
            Request = new __RPC_Login_NotifyReConnet_ARG_int32_result__();
            Request.Result=result;

        }

        public __RPC_Login_NotifyReConnet_ARG_int32_result__ Request { get; private set; }


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

    public class SendDeviceUdidOutMessage : OutMessage
    {
        public SendDeviceUdidOutMessage(IAgentBase sender, string deviceUdid)
            : base(sender, ServiceType.Login, 2037)
        {
            Request = new __RPC_Login_SendDeviceUdid_ARG_string_deviceUdid__();
            Request.DeviceUdid=deviceUdid;

        }

        public __RPC_Login_SendDeviceUdid_ARG_string_deviceUdid__ Request { get; private set; }


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
