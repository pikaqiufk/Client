using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface IActivity9xServiceInterface : IAgentBase
    {
        /// <summary>
        /// 通知全服，某个活动的状态
        /// </summary>
        void NotifyActivityState(int activityId, int state);
        /// <summary>
        /// 通知所有在线客户端某些表格刷新了
        /// </summary>
        void NotifyTableChange(int flag);
        /// <summary>
        /// 通知全服，某个活动某个炮台的数据
        /// </summary>
        void NotifyBatteryData(int activityId, ActivityBatteryOne battery);
        /// <summary>
        /// 通知全服灭世活动的状态
        /// </summary>
        void NotifyMieShiActivityState(int activityId, int state);
        /// <summary>
        /// 通知客户端某个炮台数据更新
        /// SC ActivityBatteryOne         NotifyBatteryDataOne(int32 activityId, ActivityBatteryOne battery) = 4108;
        /// 通知报名的玩家可以进入
        /// </summary>
        void NotifyPlayerCanIn(int fubenId, long canInEndTime);
    }
    public static class Activity9xServiceInterfaceExtension
    {

        public static ApplyActivityStateOutMessage ApplyActivityState(this IActivity9xServiceInterface agent, int serverId)
        {
            return new ApplyActivityStateOutMessage(agent, serverId);
        }

        public static ApplyOrderSerialOutMessage ApplyOrderSerial(this IActivity9xServiceInterface agent, ApplyOrderMessage msg)
        {
            return new ApplyOrderSerialOutMessage(agent, msg);
        }

        public static ApplyMieShiDataOutMessage ApplyMieShiData(this IActivity9xServiceInterface agent, int serverId)
        {
            return new ApplyMieShiDataOutMessage(agent, serverId);
        }

        public static ApplyBatteryDataOutMessage ApplyBatteryData(this IActivity9xServiceInterface agent, int serverId, int activityId)
        {
            return new ApplyBatteryDataOutMessage(agent, serverId, activityId);
        }

        public static ApplyContriRankingDataOutMessage ApplyContriRankingData(this IActivity9xServiceInterface agent, int serverId, int activityId)
        {
            return new ApplyContriRankingDataOutMessage(agent, serverId, activityId);
        }

        public static ApplyPointRankingDataOutMessage ApplyPointRankingData(this IActivity9xServiceInterface agent, int serverId, int activityId)
        {
            return new ApplyPointRankingDataOutMessage(agent, serverId, activityId);
        }

        public static ApplyPortraitDataOutMessage ApplyPortraitData(this IActivity9xServiceInterface agent, int serverId)
        {
            return new ApplyPortraitDataOutMessage(agent, serverId);
        }

        public static void Init(this IActivity9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Activity, (p, list) =>
            {
                switch (p)
                {
                    case 4045:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Activity_NotifyActivityState_ARG_int32_activityId_int32_state__>(ms);
                        }
                        break;
                    case 4047:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Activity_NotifyTableChange_ARG_int32_flag__>(ms);
                        }
                        break;
                    case 4107:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Activity_NotifyBatteryData_ARG_int32_activityId_ActivityBatteryOne_battery__>(ms);
                        }
                        break;
                    case 4108:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Activity_NotifyMieShiActivityState_ARG_int32_activityId_int32_state__>(ms);
                        }
                        break;
                    case 4109:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Activity_NotifyPlayerCanIn_ARG_int32_fubenId_int64_canInEndTime__>(ms);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Activity, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    case 4045:
                        {
                            var data = evt.Data as __RPC_Activity_NotifyActivityState_ARG_int32_activityId_int32_state__;
                            agent.NotifyActivityState(data.ActivityId, data.State);
                        }
                        break;
                    case 4047:
                        {
                            var data = evt.Data as __RPC_Activity_NotifyTableChange_ARG_int32_flag__;
                            agent.NotifyTableChange(data.Flag);
                        }
                        break;
                    case 4107:
                        {
                            var data = evt.Data as __RPC_Activity_NotifyBatteryData_ARG_int32_activityId_ActivityBatteryOne_battery__;
                            agent.NotifyBatteryData(data.ActivityId, data.Battery);
                        }
                        break;
                    case 4108:
                        {
                            var data = evt.Data as __RPC_Activity_NotifyMieShiActivityState_ARG_int32_activityId_int32_state__;
                            agent.NotifyMieShiActivityState(data.ActivityId, data.State);
                        }
                        break;
                    case 4109:
                        {
                            var data = evt.Data as __RPC_Activity_NotifyPlayerCanIn_ARG_int32_fubenId_int64_canInEndTime__;
                            agent.NotifyPlayerCanIn(data.FubenId, data.CanInEndTime);
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class ApplyActivityStateOutMessage : OutMessage
    {
        public ApplyActivityStateOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Activity, 4044)
        {
            Request = new __RPC_Activity_ApplyActivityState_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Activity_ApplyActivityState_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Activity_ApplyActivityState_RET_Dict_int_int_Data__ mResponse;
            public Dict_int_int_Data Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyActivityState_RET_Dict_int_int_Data__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyActivityStateOutMessage : OutMessage
    {
        public NotifyActivityStateOutMessage(IAgentBase sender, int activityId, int state)
            : base(sender, ServiceType.Activity, 4045)
        {
            Request = new __RPC_Activity_NotifyActivityState_ARG_int32_activityId_int32_state__();
            Request.ActivityId=activityId;
            Request.State=state;

        }

        public __RPC_Activity_NotifyActivityState_ARG_int32_activityId_int32_state__ Request { get; private set; }


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

    public class ApplyOrderSerialOutMessage : OutMessage
    {
        public ApplyOrderSerialOutMessage(IAgentBase sender, ApplyOrderMessage msg)
            : base(sender, ServiceType.Activity, 4046)
        {
            Request = new __RPC_Activity_ApplyOrderSerial_ARG_ApplyOrderMessage_msg__();
            Request.Msg=msg;

        }

        public __RPC_Activity_ApplyOrderSerial_ARG_ApplyOrderMessage_msg__ Request { get; private set; }

            private __RPC_Activity_ApplyOrderSerial_RET_OrderSerialData__ mResponse;
            public OrderSerialData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyOrderSerial_RET_OrderSerialData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyTableChangeOutMessage : OutMessage
    {
        public NotifyTableChangeOutMessage(IAgentBase sender, int flag)
            : base(sender, ServiceType.Activity, 4047)
        {
            Request = new __RPC_Activity_NotifyTableChange_ARG_int32_flag__();
            Request.Flag=flag;

        }

        public __RPC_Activity_NotifyTableChange_ARG_int32_flag__ Request { get; private set; }


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

    public class ApplyMieShiDataOutMessage : OutMessage
    {
        public ApplyMieShiDataOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Activity, 4100)
        {
            Request = new __RPC_Activity_ApplyMieShiData_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Activity_ApplyMieShiData_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Activity_ApplyMieShiData_RET_CommonActivityData__ mResponse;
            public CommonActivityData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyMieShiData_RET_CommonActivityData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyBatteryDataOutMessage : OutMessage
    {
        public ApplyBatteryDataOutMessage(IAgentBase sender, int serverId, int activityId)
            : base(sender, ServiceType.Activity, 4101)
        {
            Request = new __RPC_Activity_ApplyBatteryData_ARG_int32_serverId_int32_activityId__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;

        }

        public __RPC_Activity_ApplyBatteryData_ARG_int32_serverId_int32_activityId__ Request { get; private set; }

            private __RPC_Activity_ApplyBatteryData_RET_BatteryDatas__ mResponse;
            public BatteryDatas Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyBatteryData_RET_BatteryDatas__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyContriRankingDataOutMessage : OutMessage
    {
        public ApplyContriRankingDataOutMessage(IAgentBase sender, int serverId, int activityId)
            : base(sender, ServiceType.Activity, 4104)
        {
            Request = new __RPC_Activity_ApplyContriRankingData_ARG_int32_serverId_int32_activityId__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;

        }

        public __RPC_Activity_ApplyContriRankingData_ARG_int32_serverId_int32_activityId__ Request { get; private set; }

            private __RPC_Activity_ApplyContriRankingData_RET_ContriRankingData__ mResponse;
            public ContriRankingData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyContriRankingData_RET_ContriRankingData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPointRankingDataOutMessage : OutMessage
    {
        public ApplyPointRankingDataOutMessage(IAgentBase sender, int serverId, int activityId)
            : base(sender, ServiceType.Activity, 4105)
        {
            Request = new __RPC_Activity_ApplyPointRankingData_ARG_int32_serverId_int32_activityId__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;

        }

        public __RPC_Activity_ApplyPointRankingData_ARG_int32_serverId_int32_activityId__ Request { get; private set; }

            private __RPC_Activity_ApplyPointRankingData_RET_PointRankingData__ mResponse;
            public PointRankingData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyPointRankingData_RET_PointRankingData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyBatteryDataOutMessage : OutMessage
    {
        public NotifyBatteryDataOutMessage(IAgentBase sender, int activityId, ActivityBatteryOne battery)
            : base(sender, ServiceType.Activity, 4107)
        {
            Request = new __RPC_Activity_NotifyBatteryData_ARG_int32_activityId_ActivityBatteryOne_battery__();
            Request.ActivityId=activityId;
            Request.Battery=battery;

        }

        public __RPC_Activity_NotifyBatteryData_ARG_int32_activityId_ActivityBatteryOne_battery__ Request { get; private set; }


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

    public class NotifyMieShiActivityStateOutMessage : OutMessage
    {
        public NotifyMieShiActivityStateOutMessage(IAgentBase sender, int activityId, int state)
            : base(sender, ServiceType.Activity, 4108)
        {
            Request = new __RPC_Activity_NotifyMieShiActivityState_ARG_int32_activityId_int32_state__();
            Request.ActivityId=activityId;
            Request.State=state;

        }

        public __RPC_Activity_NotifyMieShiActivityState_ARG_int32_activityId_int32_state__ Request { get; private set; }


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

    public class NotifyPlayerCanInOutMessage : OutMessage
    {
        public NotifyPlayerCanInOutMessage(IAgentBase sender, int fubenId, long canInEndTime)
            : base(sender, ServiceType.Activity, 4109)
        {
            Request = new __RPC_Activity_NotifyPlayerCanIn_ARG_int32_fubenId_int64_canInEndTime__();
            Request.FubenId=fubenId;
            Request.CanInEndTime=canInEndTime;

        }

        public __RPC_Activity_NotifyPlayerCanIn_ARG_int32_fubenId_int64_canInEndTime__ Request { get; private set; }


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

    public class ApplyPortraitDataOutMessage : OutMessage
    {
        public ApplyPortraitDataOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Activity, 4018)
        {
            Request = new __RPC_Activity_ApplyPortraitData_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Activity_ApplyPortraitData_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Activity_ApplyPortraitData_RET_PlayerInfoMsg__ mResponse;
            public PlayerInfoMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Activity_ApplyPortraitData_RET_PlayerInfoMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

}
