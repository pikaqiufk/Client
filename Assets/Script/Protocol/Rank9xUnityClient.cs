using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface IRank9xServiceInterface : IAgentBase
    {
    }
    public static class Rank9xServiceInterfaceExtension
    {

        public static GetRankListOutMessage GetRankList(this IRank9xServiceInterface agent, int serverId, int rankType)
        {
            return new GetRankListOutMessage(agent, serverId, rankType);
        }

        public static GMRankOutMessage GMRank(this IRank9xServiceInterface agent, string commond)
        {
            return new GMRankOutMessage(agent, commond);
        }

        public static ApplyServerActivityDataOutMessage ApplyServerActivityData(this IRank9xServiceInterface agent, int serverId)
        {
            return new ApplyServerActivityDataOutMessage(agent, serverId);
        }

        public static void Init(this IRank9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Rank, (p, list) =>
            {
                switch (p)
                {
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Rank, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    default:
                        break;
                }
            });
        }
    }

    public class GetRankListOutMessage : OutMessage
    {
        public GetRankListOutMessage(IAgentBase sender, int serverId, int rankType)
            : base(sender, ServiceType.Rank, 6043)
        {
            Request = new __RPC_Rank_GetRankList_ARG_int32_serverId_int32_rankType__();
            Request.ServerId=serverId;
            Request.RankType=rankType;

        }

        public __RPC_Rank_GetRankList_ARG_int32_serverId_int32_rankType__ Request { get; private set; }

            private __RPC_Rank_GetRankList_RET_RankList__ mResponse;
            public RankList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Rank_GetRankList_RET_RankList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GMRankOutMessage : OutMessage
    {
        public GMRankOutMessage(IAgentBase sender, string commond)
            : base(sender, ServiceType.Rank, 6047)
        {
            Request = new __RPC_Rank_GMRank_ARG_string_commond__();
            Request.Commond=commond;

        }

        public __RPC_Rank_GMRank_ARG_string_commond__ Request { get; private set; }

            private __RPC_Rank_GMRank_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Rank_GMRank_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyServerActivityDataOutMessage : OutMessage
    {
        public ApplyServerActivityDataOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Rank, 6053)
        {
            Request = new __RPC_Rank_ApplyServerActivityData_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Rank_ApplyServerActivityData_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Rank_ApplyServerActivityData_RET_ServerActivityDatas__ mResponse;
            public ServerActivityDatas Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Rank_ApplyServerActivityData_RET_ServerActivityDatas__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

}
