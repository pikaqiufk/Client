using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface IScene9xServiceInterface : IAgentBase
    {
        /// <summary>
        /// 服务端返回进入场景结果(服务端可主动发，让玩家强制进入某个场景)
        /// </summary>
        void ReplyChangeScene(PlayerData data);
        /// <summary>
        /// 通知创建角色
        /// </summary>
        void CreateObj(CreateObjMsg msg);
        /// <summary>
        /// 广播删除角色
        /// 创建OBJ的原因，2种，0不可见 1死亡移除
        /// </summary>
        void DeleteObj(Uint64Array objs, uint reason);
        /// <summary>
        /// </summary>
        void DeleteObjList(DeleteObjMsgList dels);
        /// <summary>
        /// 同步移动
        /// </summary>
        void SyncMoveTo(CharacterMoveMsg msg);
        /// <summary>
        /// 同步移动
        /// </summary>
        void SyncMoveToList(CharacterMoveMsgList msg);
        /// <summary>
        /// 同步停止移动
        /// </summary>
        void SyncStopMove(SyncPostionMsg msg);
        /// <summary>
        /// 同步主角
        /// </summary>
        void SyncDirection(ulong characterId, int dirX, int dirZ);
        /// <summary>
        /// 广播使用技能
        /// </summary>
        void NotifyUseSkill(CharacterUseSkillMsg msg);
        /// <summary>
        /// 广播使用一组技能
        /// </summary>
        void NotifyUseSkillList(CharacterUseSkillMsgList msg);
        /// <summary>
        /// 广播Buff结果
        /// </summary>
        void SyncBuff(BuffResultMsg msg);
        /// <summary>
        /// 广播释放子弹
        /// </summary>
        void NotifyShootBullet(BulletMsg msg);
        /// <summary>
        /// 广播释放子弹
        /// </summary>
        void NotifyShootBulletList(BulletMsgList msg);
        /// <summary>
        /// 通知客户端修改装备模型
        /// </summary>
        void NotifyEquipChanged(ulong characterId, int part, int ItemId);
        /// <summary>
        /// 拾取某个物品
        /// </summary>
        void PickUpItemSuccess(ulong dropItemId);
        /// <summary>
        /// 通知客户端场景动画
        /// </summary>
        void NotifySceneAction(int ActionId);
        /// <summary>
        /// 包裹已满的提示
        /// </summary>
        void BagisFull(ulong dropItemId, int itemId, int itemCount);
        /// <summary>
        /// 同步副本时间
        /// </summary>
        void NotifyDungeonTime(int state, ulong time);
        /// <summary>
        /// 同步家园场景数据
        /// </summary>
        void SyncSceneBuilding(BuildingList data);
        /// <summary>
        /// Debug模式下客户端坐标
        /// </summary>
        void DebugObjPosition(ulong characterId, PositionData pos);
        /// <summary>
        /// 强制客户端改变坐标
        /// </summary>
        void SyncCharacterPostion(ulong characterId, PositionData pos);
        /// <summary>
        /// 广播战场提示信息
        /// </summary>
        void NotifyBattleReminder(int type, string info, int param);
        /// <summary>
        /// 同步一个倒计时
        /// </summary>
        void NotifyCountdown(ulong time, int type);
        /// <summary>
        /// 通知客户端，某只怪物的伤害列表
        /// </summary>
        void NotifyDamageList(DamageList list);
        /// <summary>
        /// 通知客户端，任务进度
        /// </summary>
        void NotifyFubenInfo(FubenInfoMsg info);
        /// <summary>
        /// 通知一些消息 type:0死亡
        /// </summary>
        void NotifyMessage(int type, string info, int addChat);
        /// <summary>
        /// 通知客户端自己的阵营发生变化
        /// </summary>
        void NotifyCampChange(int campId, Vector2Int32 pos);
        /// <summary>
        ///  同步客户端数据
        /// </summary>
        void SyncDataToClient(SceneSyncData data);
        /// <summary>
        ///  同步自己客户端数据
        /// </summary>
        void SyncMyDataToClient(SceneSyncData data);
        /// <summary>
        /// 通知客户端，据点状态改变了
        /// </summary>
        void NotifyStrongpointStateChanged(int camp, int index, int state, float time);
        /// <summary>
        /// 同步NPC位置
        /// </summary>
        void SyncObjPosition(SyncPathPosMsg msg);
        /// <summary>
        /// 服务器通知客户端，某个id的obj说了一句话,如果字典id不为空，就说字典，如果为空，就说字符串
        /// </summary>
        void ObjSpeak(ulong id, int dictId, string content);
        /// <summary>
        /// 玩家等级变化同步属性变化值
        /// </summary>
        void SyncLevelChange(LevelUpAttrData Attr);
        /// <summary>
        /// 向玩家通知攻城战npc信息
        /// </summary>
        void NotifyAllianceWarNpcData(int reliveCount, Int32Array data);
        /// <summary>
        /// 向玩家通知场景内玩家的信息，主要是位置信息
        /// </summary>
        void NotifyScenePlayerInfos(ScenePlayerInfos info);
        /// <summary>
        /// 向玩家通知小地图怪物的存活状态
        /// </summary>
        void NotifyNpcStatus(MapNpcInfos infos);
        /// <summary>
        /// 通知客户端，积分列表
        /// </summary>
        void NotifyPointList(PointList list);
        /// <summary>
        /// 通知客户端开始预警
        /// </summary>
        void NotifyStartWarning(ulong timeOut);
        /// <summary>
        /// </summary>
        void SendMieshiResult(MieshiResultMsg msg);
        /// <summary>
        /// 通知客户端，刷新副本信息
        /// </summary>
        void NotifyRefreshDungeonInfo(DungeonInfo info);
        /// <summary>
        /// 同步模型Id
        /// </summary>
        void SyncModelId(int model);
        /// <summary>
        /// 刷新副本商店购买数据
        /// </summary>
        void SyncFuBenStore(StoneItems itemlst, int storeType);
        /// <summary>
        /// 场景内广播消息，dictId不为-1时，优先用字典内容， 字符串可以用"|"分开进行格式化
        /// </summary>
        void BroadcastSceneChat(string content, int dictId);
    }
    public static class Scene9xServiceInterfaceExtension
    {

        public static CreateObjAroundOutMessage CreateObjAround(this IScene9xServiceInterface agent, uint placeholder)
        {
            return new CreateObjAroundOutMessage(agent, placeholder);
        }

        public static ApplyPlayerDataOutMessage ApplyPlayerData(this IScene9xServiceInterface agent, int placeholder)
        {
            return new ApplyPlayerDataOutMessage(agent, placeholder);
        }

        public static ChangeSceneOverOutMessage ChangeSceneOver(this IScene9xServiceInterface agent, int sceneId, ulong sceneGuid)
        {
            return new ChangeSceneOverOutMessage(agent, sceneId, sceneGuid);
        }

        public static ApplyAttributeOutMessage ApplyAttribute(this IScene9xServiceInterface agent, int placeholder)
        {
            return new ApplyAttributeOutMessage(agent, placeholder);
        }

        public static MoveToOutMessage MoveTo(this IScene9xServiceInterface agent, Vec2Array targetList, float offset, long time)
        {
            return new MoveToOutMessage(agent, targetList, offset, time);
        }

        public static StopMoveOutMessage StopMove(this IScene9xServiceInterface agent, PositionData pos)
        {
            return new StopMoveOutMessage(agent, pos);
        }

        public static DirectToOutMessage DirectTo(this IScene9xServiceInterface agent, int dirX, int dirZ)
        {
            return new DirectToOutMessage(agent, dirX, dirZ);
        }

        public static SendUseSkillRequestOutMessage SendUseSkillRequest(this IScene9xServiceInterface agent, CharacterUseSkillMsg msg)
        {
            return new SendUseSkillRequestOutMessage(agent, msg);
        }

        public static GMSceneOutMessage GMScene(this IScene9xServiceInterface agent, string commond)
        {
            return new GMSceneOutMessage(agent, commond);
        }

        public static SendTeleportRequestOutMessage SendTeleportRequest(this IScene9xServiceInterface agent, int type)
        {
            return new SendTeleportRequestOutMessage(agent, type);
        }

        public static ChangeSceneRequestOutMessage ChangeSceneRequest(this IScene9xServiceInterface agent, int sceneId)
        {
            return new ChangeSceneRequestOutMessage(agent, sceneId);
        }

        public static ApplySceneObjOutMessage ApplySceneObj(this IScene9xServiceInterface agent, int placeholder)
        {
            return new ApplySceneObjOutMessage(agent, placeholder);
        }

        public static PickUpItemOutMessage PickUpItem(this IScene9xServiceInterface agent, ulong dropItemId)
        {
            return new PickUpItemOutMessage(agent, dropItemId);
        }

        public static SceneChatMessageOutMessage SceneChatMessage(this IScene9xServiceInterface agent, int chatType, ChatMessageContent Content, ulong characterId)
        {
            return new SceneChatMessageOutMessage(agent, chatType, Content, characterId);
        }

        public static ExitDungeonOutMessage ExitDungeon(this IScene9xServiceInterface agent, int type)
        {
            return new ExitDungeonOutMessage(agent, type);
        }

        public static NotifySomeClientMessageOutMessage NotifySomeClientMessage(this IScene9xServiceInterface agent, int type, int value)
        {
            return new NotifySomeClientMessageOutMessage(agent, type, value);
        }

        public static MoveToRobotOutMessage MoveToRobot(this IScene9xServiceInterface agent, Vector2Int32 postion)
        {
            return new MoveToRobotOutMessage(agent, postion);
        }

        public static NpcServiceOutMessage NpcService(this IScene9xServiceInterface agent, ulong npcGuid, int serviceId)
        {
            return new NpcServiceOutMessage(agent, npcGuid, serviceId);
        }

        public static ReliveTypeOutMessage ReliveType(this IScene9xServiceInterface agent, int type)
        {
            return new ReliveTypeOutMessage(agent, type);
        }

        public static ApplySceneExdataOutMessage ApplySceneExdata(this IScene9xServiceInterface agent, int placeholder)
        {
            return new ApplySceneExdataOutMessage(agent, placeholder);
        }

        public static ChangePKModelOutMessage ChangePKModel(this IScene9xServiceInterface agent, int pkModel)
        {
            return new ChangePKModelOutMessage(agent, pkModel);
        }

        public static FlyToOutMessage FlyTo(this IScene9xServiceInterface agent, int sceneId, Vector2Int32 postion)
        {
            return new FlyToOutMessage(agent, sceneId, postion);
        }

        public static ApplySceneTeamLeaderObjOutMessage ApplySceneTeamLeaderObj(this IScene9xServiceInterface agent, int placeholder)
        {
            return new ApplySceneTeamLeaderObjOutMessage(agent, placeholder);
        }

        public static GetLeaveExpOutMessage GetLeaveExp(this IScene9xServiceInterface agent, int type, int needCount)
        {
            return new GetLeaveExpOutMessage(agent, type, needCount);
        }

        public static ApplyLeaveExpOutMessage ApplyLeaveExp(this IScene9xServiceInterface agent, int placeholder)
        {
            return new ApplyLeaveExpOutMessage(agent, placeholder);
        }

        public static ChangeSceneRequestByMissionOutMessage ChangeSceneRequestByMission(this IScene9xServiceInterface agent, int sceneId, int missionId)
        {
            return new ChangeSceneRequestByMissionOutMessage(agent, sceneId, missionId);
        }

        public static ApplyPlayerPostionListOutMessage ApplyPlayerPostionList(this IScene9xServiceInterface agent, Uint64Array characterIds)
        {
            return new ApplyPlayerPostionListOutMessage(agent, characterIds);
        }

        public static InspireOutMessage Inspire(this IScene9xServiceInterface agent, int placeholder)
        {
            return new InspireOutMessage(agent, placeholder);
        }

        public static AllianceWarRespawnGuardOutMessage AllianceWarRespawnGuard(this IScene9xServiceInterface agent, int index)
        {
            return new AllianceWarRespawnGuardOutMessage(agent, index);
        }

        public static GetSceneNpcPosOutMessage GetSceneNpcPos(this IScene9xServiceInterface agent, uint placeholder)
        {
            return new GetSceneNpcPosOutMessage(agent, placeholder);
        }

        public static FastReachOutMessage FastReach(this IScene9xServiceInterface agent, int sceneId, Vector2Int32 postion)
        {
            return new FastReachOutMessage(agent, sceneId, postion);
        }

        public static void Init(this IScene9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Scene, (p, list) =>
            {
                switch (p)
                {
                    case 3074:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_ReplyChangeScene_ARG_PlayerData_data__>(ms);
                        }
                        break;
                    case 3078:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_CreateObj_ARG_CreateObjMsg_msg__>(ms);
                        }
                        break;
                    case 3079:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_DeleteObj_ARG_Uint64Array_objs_uint32_reason__>(ms);
                        }
                        break;
                    case 3080:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_DeleteObjList_ARG_DeleteObjMsgList_dels__>(ms);
                        }
                        break;
                    case 3082:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncMoveTo_ARG_CharacterMoveMsg_msg__>(ms);
                        }
                        break;
                    case 3083:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncMoveToList_ARG_CharacterMoveMsgList_msg__>(ms);
                        }
                        break;
                    case 3085:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncStopMove_ARG_SyncPostionMsg_msg__>(ms);
                        }
                        break;
                    case 3087:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncDirection_ARG_uint64_characterId_int32_dirX_int32_dirZ__>(ms);
                        }
                        break;
                    case 3089:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyUseSkill_ARG_CharacterUseSkillMsg_msg__>(ms);
                        }
                        break;
                    case 3090:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyUseSkillList_ARG_CharacterUseSkillMsgList_msg__>(ms);
                        }
                        break;
                    case 3091:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncBuff_ARG_BuffResultMsg_msg__>(ms);
                        }
                        break;
                    case 3098:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyShootBullet_ARG_BulletMsg_msg__>(ms);
                        }
                        break;
                    case 3099:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyShootBulletList_ARG_BulletMsgList_msg__>(ms);
                        }
                        break;
                    case 3102:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyEquipChanged_ARG_uint64_characterId_int32_part_int32_ItemId__>(ms);
                        }
                        break;
                    case 3112:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_PickUpItemSuccess_ARG_uint64_dropItemId__>(ms);
                        }
                        break;
                    case 3113:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifySceneAction_ARG_int32_ActionId__>(ms);
                        }
                        break;
                    case 3114:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_BagisFull_ARG_uint64_dropItemId_int32_itemId_int32_itemCount__>(ms);
                        }
                        break;
                    case 3118:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyDungeonTime_ARG_int32_state_uint64_time__>(ms);
                        }
                        break;
                    case 3120:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncSceneBuilding_ARG_BuildingList_data__>(ms);
                        }
                        break;
                    case 3122:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_DebugObjPosition_ARG_uint64_characterId_PositionData_pos__>(ms);
                        }
                        break;
                    case 3124:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncCharacterPostion_ARG_uint64_characterId_PositionData_pos__>(ms);
                        }
                        break;
                    case 3132:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyBattleReminder_ARG_int32_type_string_info_int32_param__>(ms);
                        }
                        break;
                    case 3133:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyCountdown_ARG_uint64_time_int32_type__>(ms);
                        }
                        break;
                    case 3137:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyDamageList_ARG_DamageList_list__>(ms);
                        }
                        break;
                    case 3138:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyFubenInfo_ARG_FubenInfoMsg_info__>(ms);
                        }
                        break;
                    case 3141:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyMessage_ARG_int32_type_string_info_int32_addChat__>(ms);
                        }
                        break;
                    case 3142:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyCampChange_ARG_int32_campId_Vector2Int32_pos__>(ms);
                        }
                        break;
                    case 3150:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncDataToClient_ARG_SceneSyncData_data__>(ms);
                        }
                        break;
                    case 3151:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncMyDataToClient_ARG_SceneSyncData_data__>(ms);
                        }
                        break;
                    case 3153:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyStrongpointStateChanged_ARG_int32_camp_int32_index_int32_state_float_time__>(ms);
                        }
                        break;
                    case 3156:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncObjPosition_ARG_SyncPathPosMsg_msg__>(ms);
                        }
                        break;
                    case 3160:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_ObjSpeak_ARG_uint64_id_int32_dictId_string_content__>(ms);
                        }
                        break;
                    case 3163:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncLevelChange_ARG_LevelUpAttrData_Attr__>(ms);
                        }
                        break;
                    case 3166:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyAllianceWarNpcData_ARG_int32_reliveCount_Int32Array_data__>(ms);
                        }
                        break;
                    case 3167:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyScenePlayerInfos_ARG_ScenePlayerInfos_info__>(ms);
                        }
                        break;
                    case 3168:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyNpcStatus_ARG_MapNpcInfos_infos__>(ms);
                        }
                        break;
                    case 3173:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyPointList_ARG_PointList_list__>(ms);
                        }
                        break;
                    case 3174:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyStartWarning_ARG_uint64_timeOut__>(ms);
                        }
                        break;
                    case 3602:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SendMieshiResult_ARG_MieshiResultMsg_msg__>(ms);
                        }
                        break;
                    case 3604:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_NotifyRefreshDungeonInfo_ARG_DungeonInfo_info__>(ms);
                        }
                        break;
                    case 3992:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncModelId_ARG_int32_model__>(ms);
                        }
                        break;
                    case 3997:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_SyncFuBenStore_ARG_StoneItems_itemlst_int32_storeType__>(ms);
                        }
                        break;
                    case 3999:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Scene_BroadcastSceneChat_ARG_string_content_int32_dictId__>(ms);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Scene, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    case 3074:
                        {
                            var data = evt.Data as __RPC_Scene_ReplyChangeScene_ARG_PlayerData_data__;
                            agent.ReplyChangeScene(data.Data);
                        }
                        break;
                    case 3078:
                        {
                            var data = evt.Data as __RPC_Scene_CreateObj_ARG_CreateObjMsg_msg__;
                            agent.CreateObj(data.Msg);
                        }
                        break;
                    case 3079:
                        {
                            var data = evt.Data as __RPC_Scene_DeleteObj_ARG_Uint64Array_objs_uint32_reason__;
                            agent.DeleteObj(data.Objs, data.Reason);
                        }
                        break;
                    case 3080:
                        {
                            var data = evt.Data as __RPC_Scene_DeleteObjList_ARG_DeleteObjMsgList_dels__;
                            agent.DeleteObjList(data.Dels);
                        }
                        break;
                    case 3082:
                        {
                            var data = evt.Data as __RPC_Scene_SyncMoveTo_ARG_CharacterMoveMsg_msg__;
                            agent.SyncMoveTo(data.Msg);
                        }
                        break;
                    case 3083:
                        {
                            var data = evt.Data as __RPC_Scene_SyncMoveToList_ARG_CharacterMoveMsgList_msg__;
                            agent.SyncMoveToList(data.Msg);
                        }
                        break;
                    case 3085:
                        {
                            var data = evt.Data as __RPC_Scene_SyncStopMove_ARG_SyncPostionMsg_msg__;
                            agent.SyncStopMove(data.Msg);
                        }
                        break;
                    case 3087:
                        {
                            var data = evt.Data as __RPC_Scene_SyncDirection_ARG_uint64_characterId_int32_dirX_int32_dirZ__;
                            agent.SyncDirection(data.CharacterId, data.DirX, data.DirZ);
                        }
                        break;
                    case 3089:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyUseSkill_ARG_CharacterUseSkillMsg_msg__;
                            agent.NotifyUseSkill(data.Msg);
                        }
                        break;
                    case 3090:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyUseSkillList_ARG_CharacterUseSkillMsgList_msg__;
                            agent.NotifyUseSkillList(data.Msg);
                        }
                        break;
                    case 3091:
                        {
                            var data = evt.Data as __RPC_Scene_SyncBuff_ARG_BuffResultMsg_msg__;
                            agent.SyncBuff(data.Msg);
                        }
                        break;
                    case 3098:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyShootBullet_ARG_BulletMsg_msg__;
                            agent.NotifyShootBullet(data.Msg);
                        }
                        break;
                    case 3099:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyShootBulletList_ARG_BulletMsgList_msg__;
                            agent.NotifyShootBulletList(data.Msg);
                        }
                        break;
                    case 3102:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyEquipChanged_ARG_uint64_characterId_int32_part_int32_ItemId__;
                            agent.NotifyEquipChanged(data.CharacterId, data.Part, data.ItemId);
                        }
                        break;
                    case 3112:
                        {
                            var data = evt.Data as __RPC_Scene_PickUpItemSuccess_ARG_uint64_dropItemId__;
                            agent.PickUpItemSuccess(data.DropItemId);
                        }
                        break;
                    case 3113:
                        {
                            var data = evt.Data as __RPC_Scene_NotifySceneAction_ARG_int32_ActionId__;
                            agent.NotifySceneAction(data.ActionId);
                        }
                        break;
                    case 3114:
                        {
                            var data = evt.Data as __RPC_Scene_BagisFull_ARG_uint64_dropItemId_int32_itemId_int32_itemCount__;
                            agent.BagisFull(data.DropItemId, data.ItemId, data.ItemCount);
                        }
                        break;
                    case 3118:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyDungeonTime_ARG_int32_state_uint64_time__;
                            agent.NotifyDungeonTime(data.State, data.Time);
                        }
                        break;
                    case 3120:
                        {
                            var data = evt.Data as __RPC_Scene_SyncSceneBuilding_ARG_BuildingList_data__;
                            agent.SyncSceneBuilding(data.Data);
                        }
                        break;
                    case 3122:
                        {
                            var data = evt.Data as __RPC_Scene_DebugObjPosition_ARG_uint64_characterId_PositionData_pos__;
                            agent.DebugObjPosition(data.CharacterId, data.Pos);
                        }
                        break;
                    case 3124:
                        {
                            var data = evt.Data as __RPC_Scene_SyncCharacterPostion_ARG_uint64_characterId_PositionData_pos__;
                            agent.SyncCharacterPostion(data.CharacterId, data.Pos);
                        }
                        break;
                    case 3132:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyBattleReminder_ARG_int32_type_string_info_int32_param__;
                            agent.NotifyBattleReminder(data.Type, data.Info, data.Param);
                        }
                        break;
                    case 3133:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyCountdown_ARG_uint64_time_int32_type__;
                            agent.NotifyCountdown(data.Time, data.Type);
                        }
                        break;
                    case 3137:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyDamageList_ARG_DamageList_list__;
                            agent.NotifyDamageList(data.List);
                        }
                        break;
                    case 3138:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyFubenInfo_ARG_FubenInfoMsg_info__;
                            agent.NotifyFubenInfo(data.Info);
                        }
                        break;
                    case 3141:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyMessage_ARG_int32_type_string_info_int32_addChat__;
                            agent.NotifyMessage(data.Type, data.Info, data.AddChat);
                        }
                        break;
                    case 3142:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyCampChange_ARG_int32_campId_Vector2Int32_pos__;
                            agent.NotifyCampChange(data.CampId, data.Pos);
                        }
                        break;
                    case 3150:
                        {
                            var data = evt.Data as __RPC_Scene_SyncDataToClient_ARG_SceneSyncData_data__;
                            agent.SyncDataToClient(data.Data);
                        }
                        break;
                    case 3151:
                        {
                            var data = evt.Data as __RPC_Scene_SyncMyDataToClient_ARG_SceneSyncData_data__;
                            agent.SyncMyDataToClient(data.Data);
                        }
                        break;
                    case 3153:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyStrongpointStateChanged_ARG_int32_camp_int32_index_int32_state_float_time__;
                            agent.NotifyStrongpointStateChanged(data.Camp, data.Index, data.State, data.Time);
                        }
                        break;
                    case 3156:
                        {
                            var data = evt.Data as __RPC_Scene_SyncObjPosition_ARG_SyncPathPosMsg_msg__;
                            agent.SyncObjPosition(data.Msg);
                        }
                        break;
                    case 3160:
                        {
                            var data = evt.Data as __RPC_Scene_ObjSpeak_ARG_uint64_id_int32_dictId_string_content__;
                            agent.ObjSpeak(data.Id, data.DictId, data.Content);
                        }
                        break;
                    case 3163:
                        {
                            var data = evt.Data as __RPC_Scene_SyncLevelChange_ARG_LevelUpAttrData_Attr__;
                            agent.SyncLevelChange(data.Attr);
                        }
                        break;
                    case 3166:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyAllianceWarNpcData_ARG_int32_reliveCount_Int32Array_data__;
                            agent.NotifyAllianceWarNpcData(data.ReliveCount, data.Data);
                        }
                        break;
                    case 3167:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyScenePlayerInfos_ARG_ScenePlayerInfos_info__;
                            agent.NotifyScenePlayerInfos(data.Info);
                        }
                        break;
                    case 3168:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyNpcStatus_ARG_MapNpcInfos_infos__;
                            agent.NotifyNpcStatus(data.Infos);
                        }
                        break;
                    case 3173:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyPointList_ARG_PointList_list__;
                            agent.NotifyPointList(data.List);
                        }
                        break;
                    case 3174:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyStartWarning_ARG_uint64_timeOut__;
                            agent.NotifyStartWarning(data.TimeOut);
                        }
                        break;
                    case 3602:
                        {
                            var data = evt.Data as __RPC_Scene_SendMieshiResult_ARG_MieshiResultMsg_msg__;
                            agent.SendMieshiResult(data.Msg);
                        }
                        break;
                    case 3604:
                        {
                            var data = evt.Data as __RPC_Scene_NotifyRefreshDungeonInfo_ARG_DungeonInfo_info__;
                            agent.NotifyRefreshDungeonInfo(data.Info);
                        }
                        break;
                    case 3992:
                        {
                            var data = evt.Data as __RPC_Scene_SyncModelId_ARG_int32_model__;
                            agent.SyncModelId(data.Model);
                        }
                        break;
                    case 3997:
                        {
                            var data = evt.Data as __RPC_Scene_SyncFuBenStore_ARG_StoneItems_itemlst_int32_storeType__;
                            agent.SyncFuBenStore(data.Itemlst, data.StoreType);
                        }
                        break;
                    case 3999:
                        {
                            var data = evt.Data as __RPC_Scene_BroadcastSceneChat_ARG_string_content_int32_dictId__;
                            agent.BroadcastSceneChat(data.Content, data.DictId);
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class CreateObjAroundOutMessage : OutMessage
    {
        public CreateObjAroundOutMessage(IAgentBase sender, uint placeholder)
            : base(sender, ServiceType.Scene, 3065)
        {
            Request = new __RPC_Scene_CreateObjAround_ARG_uint32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_CreateObjAround_ARG_uint32_placeholder__ Request { get; private set; }

            private __RPC_Scene_CreateObjAround_RET_CreateObjMsg__ mResponse;
            public CreateObjMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_CreateObjAround_RET_CreateObjMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ReplyChangeSceneOutMessage : OutMessage
    {
        public ReplyChangeSceneOutMessage(IAgentBase sender, PlayerData data)
            : base(sender, ServiceType.Scene, 3074)
        {
            Request = new __RPC_Scene_ReplyChangeScene_ARG_PlayerData_data__();
            Request.Data=data;

        }

        public __RPC_Scene_ReplyChangeScene_ARG_PlayerData_data__ Request { get; private set; }


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

    public class ApplyPlayerDataOutMessage : OutMessage
    {
        public ApplyPlayerDataOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3075)
        {
            Request = new __RPC_Scene_ApplyPlayerData_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_ApplyPlayerData_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_ApplyPlayerData_RET_PlayerData__ mResponse;
            public PlayerData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplyPlayerData_RET_PlayerData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChangeSceneOverOutMessage : OutMessage
    {
        public ChangeSceneOverOutMessage(IAgentBase sender, int sceneId, ulong sceneGuid)
            : base(sender, ServiceType.Scene, 3076)
        {
            Request = new __RPC_Scene_ChangeSceneOver_ARG_int32_sceneId_uint64_sceneGuid__();
            Request.SceneId=sceneId;
            Request.SceneGuid=sceneGuid;

        }

        public __RPC_Scene_ChangeSceneOver_ARG_int32_sceneId_uint64_sceneGuid__ Request { get; private set; }


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

    public class ApplyAttributeOutMessage : OutMessage
    {
        public ApplyAttributeOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3077)
        {
            Request = new __RPC_Scene_ApplyAttribute_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_ApplyAttribute_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_ApplyAttribute_RET_Int32Array__ mResponse;
            public Int32Array Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplyAttribute_RET_Int32Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class CreateObjOutMessage : OutMessage
    {
        public CreateObjOutMessage(IAgentBase sender, CreateObjMsg msg)
            : base(sender, ServiceType.Scene, 3078)
        {
            Request = new __RPC_Scene_CreateObj_ARG_CreateObjMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_CreateObj_ARG_CreateObjMsg_msg__ Request { get; private set; }


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

    public class DeleteObjOutMessage : OutMessage
    {
        public DeleteObjOutMessage(IAgentBase sender, Uint64Array objs, uint reason)
            : base(sender, ServiceType.Scene, 3079)
        {
            Request = new __RPC_Scene_DeleteObj_ARG_Uint64Array_objs_uint32_reason__();
            Request.Objs=objs;
            Request.Reason=reason;

        }

        public __RPC_Scene_DeleteObj_ARG_Uint64Array_objs_uint32_reason__ Request { get; private set; }


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

    public class DeleteObjListOutMessage : OutMessage
    {
        public DeleteObjListOutMessage(IAgentBase sender, DeleteObjMsgList dels)
            : base(sender, ServiceType.Scene, 3080)
        {
            Request = new __RPC_Scene_DeleteObjList_ARG_DeleteObjMsgList_dels__();
            Request.Dels=dels;

        }

        public __RPC_Scene_DeleteObjList_ARG_DeleteObjMsgList_dels__ Request { get; private set; }


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

    public class MoveToOutMessage : OutMessage
    {
        public MoveToOutMessage(IAgentBase sender, Vec2Array targetList, float offset, long time)
            : base(sender, ServiceType.Scene, 3081)
        {
            Request = new __RPC_Scene_MoveTo_ARG_Vec2Array_targetList_float_offset_int64_time__();
            Request.TargetList=targetList;
            Request.Offset=offset;
            Request.Time=time;

        }

        public __RPC_Scene_MoveTo_ARG_Vec2Array_targetList_float_offset_int64_time__ Request { get; private set; }

            private __RPC_Scene_MoveTo_RET_PositionData__ mResponse;
            public PositionData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_MoveTo_RET_PositionData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncMoveToOutMessage : OutMessage
    {
        public SyncMoveToOutMessage(IAgentBase sender, CharacterMoveMsg msg)
            : base(sender, ServiceType.Scene, 3082)
        {
            Request = new __RPC_Scene_SyncMoveTo_ARG_CharacterMoveMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SyncMoveTo_ARG_CharacterMoveMsg_msg__ Request { get; private set; }


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

    public class SyncMoveToListOutMessage : OutMessage
    {
        public SyncMoveToListOutMessage(IAgentBase sender, CharacterMoveMsgList msg)
            : base(sender, ServiceType.Scene, 3083)
        {
            Request = new __RPC_Scene_SyncMoveToList_ARG_CharacterMoveMsgList_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SyncMoveToList_ARG_CharacterMoveMsgList_msg__ Request { get; private set; }


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

    public class StopMoveOutMessage : OutMessage
    {
        public StopMoveOutMessage(IAgentBase sender, PositionData pos)
            : base(sender, ServiceType.Scene, 3084)
        {
            Request = new __RPC_Scene_StopMove_ARG_PositionData_pos__();
            Request.Pos=pos;

        }

        public __RPC_Scene_StopMove_ARG_PositionData_pos__ Request { get; private set; }


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

    public class SyncStopMoveOutMessage : OutMessage
    {
        public SyncStopMoveOutMessage(IAgentBase sender, SyncPostionMsg msg)
            : base(sender, ServiceType.Scene, 3085)
        {
            Request = new __RPC_Scene_SyncStopMove_ARG_SyncPostionMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SyncStopMove_ARG_SyncPostionMsg_msg__ Request { get; private set; }


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

    public class DirectToOutMessage : OutMessage
    {
        public DirectToOutMessage(IAgentBase sender, int dirX, int dirZ)
            : base(sender, ServiceType.Scene, 3086)
        {
            Request = new __RPC_Scene_DirectTo_ARG_int32_dirX_int32_dirZ__();
            Request.DirX=dirX;
            Request.DirZ=dirZ;

        }

        public __RPC_Scene_DirectTo_ARG_int32_dirX_int32_dirZ__ Request { get; private set; }

            private __RPC_Scene_DirectTo_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_DirectTo_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncDirectionOutMessage : OutMessage
    {
        public SyncDirectionOutMessage(IAgentBase sender, ulong characterId, int dirX, int dirZ)
            : base(sender, ServiceType.Scene, 3087)
        {
            Request = new __RPC_Scene_SyncDirection_ARG_uint64_characterId_int32_dirX_int32_dirZ__();
            Request.CharacterId=characterId;
            Request.DirX=dirX;
            Request.DirZ=dirZ;

        }

        public __RPC_Scene_SyncDirection_ARG_uint64_characterId_int32_dirX_int32_dirZ__ Request { get; private set; }


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

    public class SendUseSkillRequestOutMessage : OutMessage
    {
        public SendUseSkillRequestOutMessage(IAgentBase sender, CharacterUseSkillMsg msg)
            : base(sender, ServiceType.Scene, 3088)
        {
            Request = new __RPC_Scene_SendUseSkillRequest_ARG_CharacterUseSkillMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SendUseSkillRequest_ARG_CharacterUseSkillMsg_msg__ Request { get; private set; }

            private __RPC_Scene_SendUseSkillRequest_RET_Int32Array__ mResponse;
            public Int32Array Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_SendUseSkillRequest_RET_Int32Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyUseSkillOutMessage : OutMessage
    {
        public NotifyUseSkillOutMessage(IAgentBase sender, CharacterUseSkillMsg msg)
            : base(sender, ServiceType.Scene, 3089)
        {
            Request = new __RPC_Scene_NotifyUseSkill_ARG_CharacterUseSkillMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_NotifyUseSkill_ARG_CharacterUseSkillMsg_msg__ Request { get; private set; }


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

    public class NotifyUseSkillListOutMessage : OutMessage
    {
        public NotifyUseSkillListOutMessage(IAgentBase sender, CharacterUseSkillMsgList msg)
            : base(sender, ServiceType.Scene, 3090)
        {
            Request = new __RPC_Scene_NotifyUseSkillList_ARG_CharacterUseSkillMsgList_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_NotifyUseSkillList_ARG_CharacterUseSkillMsgList_msg__ Request { get; private set; }


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

    public class SyncBuffOutMessage : OutMessage
    {
        public SyncBuffOutMessage(IAgentBase sender, BuffResultMsg msg)
            : base(sender, ServiceType.Scene, 3091)
        {
            Request = new __RPC_Scene_SyncBuff_ARG_BuffResultMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SyncBuff_ARG_BuffResultMsg_msg__ Request { get; private set; }


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

    public class GMSceneOutMessage : OutMessage
    {
        public GMSceneOutMessage(IAgentBase sender, string commond)
            : base(sender, ServiceType.Scene, 3097)
        {
            Request = new __RPC_Scene_GMScene_ARG_string_commond__();
            Request.Commond=commond;

        }

        public __RPC_Scene_GMScene_ARG_string_commond__ Request { get; private set; }

            private __RPC_Scene_GMScene_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_GMScene_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyShootBulletOutMessage : OutMessage
    {
        public NotifyShootBulletOutMessage(IAgentBase sender, BulletMsg msg)
            : base(sender, ServiceType.Scene, 3098)
        {
            Request = new __RPC_Scene_NotifyShootBullet_ARG_BulletMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_NotifyShootBullet_ARG_BulletMsg_msg__ Request { get; private set; }


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

    public class NotifyShootBulletListOutMessage : OutMessage
    {
        public NotifyShootBulletListOutMessage(IAgentBase sender, BulletMsgList msg)
            : base(sender, ServiceType.Scene, 3099)
        {
            Request = new __RPC_Scene_NotifyShootBulletList_ARG_BulletMsgList_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_NotifyShootBulletList_ARG_BulletMsgList_msg__ Request { get; private set; }


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

    public class SendTeleportRequestOutMessage : OutMessage
    {
        public SendTeleportRequestOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Scene, 3100)
        {
            Request = new __RPC_Scene_SendTeleportRequest_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Scene_SendTeleportRequest_ARG_int32_type__ Request { get; private set; }

            private __RPC_Scene_SendTeleportRequest_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_SendTeleportRequest_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChangeSceneRequestOutMessage : OutMessage
    {
        public ChangeSceneRequestOutMessage(IAgentBase sender, int sceneId)
            : base(sender, ServiceType.Scene, 3101)
        {
            Request = new __RPC_Scene_ChangeSceneRequest_ARG_int32_sceneId__();
            Request.SceneId=sceneId;

        }

        public __RPC_Scene_ChangeSceneRequest_ARG_int32_sceneId__ Request { get; private set; }

            private __RPC_Scene_ChangeSceneRequest_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_ChangeSceneRequest_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyEquipChangedOutMessage : OutMessage
    {
        public NotifyEquipChangedOutMessage(IAgentBase sender, ulong characterId, int part, int ItemId)
            : base(sender, ServiceType.Scene, 3102)
        {
            Request = new __RPC_Scene_NotifyEquipChanged_ARG_uint64_characterId_int32_part_int32_ItemId__();
            Request.CharacterId=characterId;
            Request.Part=part;
            Request.ItemId=ItemId;

        }

        public __RPC_Scene_NotifyEquipChanged_ARG_uint64_characterId_int32_part_int32_ItemId__ Request { get; private set; }


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

    public class ApplySceneObjOutMessage : OutMessage
    {
        public ApplySceneObjOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3104)
        {
            Request = new __RPC_Scene_ApplySceneObj_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_ApplySceneObj_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_ApplySceneObj_RET_ObjDataListMsg__ mResponse;
            public ObjDataListMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplySceneObj_RET_ObjDataListMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class PickUpItemOutMessage : OutMessage
    {
        public PickUpItemOutMessage(IAgentBase sender, ulong dropItemId)
            : base(sender, ServiceType.Scene, 3105)
        {
            Request = new __RPC_Scene_PickUpItem_ARG_uint64_dropItemId__();
            Request.DropItemId=dropItemId;

        }

        public __RPC_Scene_PickUpItem_ARG_uint64_dropItemId__ Request { get; private set; }

            private __RPC_Scene_PickUpItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_PickUpItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SceneChatMessageOutMessage : OutMessage
    {
        public SceneChatMessageOutMessage(IAgentBase sender, int chatType, ChatMessageContent Content, ulong characterId)
            : base(sender, ServiceType.Scene, 3106)
        {
            Request = new __RPC_Scene_SceneChatMessage_ARG_int32_chatType_ChatMessageContent_Content_uint64_characterId__();
            Request.ChatType=chatType;
            Request.Content=Content;
            Request.CharacterId=characterId;

        }

        public __RPC_Scene_SceneChatMessage_ARG_int32_chatType_ChatMessageContent_Content_uint64_characterId__ Request { get; private set; }

            private __RPC_Scene_SceneChatMessage_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_SceneChatMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ExitDungeonOutMessage : OutMessage
    {
        public ExitDungeonOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Scene, 3111)
        {
            Request = new __RPC_Scene_ExitDungeon_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Scene_ExitDungeon_ARG_int32_type__ Request { get; private set; }

            private __RPC_Scene_ExitDungeon_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_ExitDungeon_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class PickUpItemSuccessOutMessage : OutMessage
    {
        public PickUpItemSuccessOutMessage(IAgentBase sender, ulong dropItemId)
            : base(sender, ServiceType.Scene, 3112)
        {
            Request = new __RPC_Scene_PickUpItemSuccess_ARG_uint64_dropItemId__();
            Request.DropItemId=dropItemId;

        }

        public __RPC_Scene_PickUpItemSuccess_ARG_uint64_dropItemId__ Request { get; private set; }


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

    public class NotifySceneActionOutMessage : OutMessage
    {
        public NotifySceneActionOutMessage(IAgentBase sender, int ActionId)
            : base(sender, ServiceType.Scene, 3113)
        {
            Request = new __RPC_Scene_NotifySceneAction_ARG_int32_ActionId__();
            Request.ActionId=ActionId;

        }

        public __RPC_Scene_NotifySceneAction_ARG_int32_ActionId__ Request { get; private set; }


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

    public class BagisFullOutMessage : OutMessage
    {
        public BagisFullOutMessage(IAgentBase sender, ulong dropItemId, int itemId, int itemCount)
            : base(sender, ServiceType.Scene, 3114)
        {
            Request = new __RPC_Scene_BagisFull_ARG_uint64_dropItemId_int32_itemId_int32_itemCount__();
            Request.DropItemId=dropItemId;
            Request.ItemId=itemId;
            Request.ItemCount=itemCount;

        }

        public __RPC_Scene_BagisFull_ARG_uint64_dropItemId_int32_itemId_int32_itemCount__ Request { get; private set; }


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

    public class NotifySomeClientMessageOutMessage : OutMessage
    {
        public NotifySomeClientMessageOutMessage(IAgentBase sender, int type, int value)
            : base(sender, ServiceType.Scene, 3115)
        {
            Request = new __RPC_Scene_NotifySomeClientMessage_ARG_int32_type_int32_value__();
            Request.Type=type;
            Request.Value=value;

        }

        public __RPC_Scene_NotifySomeClientMessage_ARG_int32_type_int32_value__ Request { get; private set; }

            private __RPC_Scene_NotifySomeClientMessage_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_NotifySomeClientMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class MoveToRobotOutMessage : OutMessage
    {
        public MoveToRobotOutMessage(IAgentBase sender, Vector2Int32 postion)
            : base(sender, ServiceType.Scene, 3116)
        {
            Request = new __RPC_Scene_MoveToRobot_ARG_Vector2Int32_postion__();
            Request.Postion=postion;

        }

        public __RPC_Scene_MoveToRobot_ARG_Vector2Int32_postion__ Request { get; private set; }

            private __RPC_Scene_MoveToRobot_RET_Vec2Array__ mResponse;
            public Vec2Array Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_MoveToRobot_RET_Vec2Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NpcServiceOutMessage : OutMessage
    {
        public NpcServiceOutMessage(IAgentBase sender, ulong npcGuid, int serviceId)
            : base(sender, ServiceType.Scene, 3117)
        {
            Request = new __RPC_Scene_NpcService_ARG_uint64_npcGuid_int32_serviceId__();
            Request.NpcGuid=npcGuid;
            Request.ServiceId=serviceId;

        }

        public __RPC_Scene_NpcService_ARG_uint64_npcGuid_int32_serviceId__ Request { get; private set; }

            private __RPC_Scene_NpcService_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_NpcService_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyDungeonTimeOutMessage : OutMessage
    {
        public NotifyDungeonTimeOutMessage(IAgentBase sender, int state, ulong time)
            : base(sender, ServiceType.Scene, 3118)
        {
            Request = new __RPC_Scene_NotifyDungeonTime_ARG_int32_state_uint64_time__();
            Request.State=state;
            Request.Time=time;

        }

        public __RPC_Scene_NotifyDungeonTime_ARG_int32_state_uint64_time__ Request { get; private set; }


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

    public class SyncSceneBuildingOutMessage : OutMessage
    {
        public SyncSceneBuildingOutMessage(IAgentBase sender, BuildingList data)
            : base(sender, ServiceType.Scene, 3120)
        {
            Request = new __RPC_Scene_SyncSceneBuilding_ARG_BuildingList_data__();
            Request.Data=data;

        }

        public __RPC_Scene_SyncSceneBuilding_ARG_BuildingList_data__ Request { get; private set; }


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

    public class ReliveTypeOutMessage : OutMessage
    {
        public ReliveTypeOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Scene, 3121)
        {
            Request = new __RPC_Scene_ReliveType_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Scene_ReliveType_ARG_int32_type__ Request { get; private set; }

            private __RPC_Scene_ReliveType_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_ReliveType_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DebugObjPositionOutMessage : OutMessage
    {
        public DebugObjPositionOutMessage(IAgentBase sender, ulong characterId, PositionData pos)
            : base(sender, ServiceType.Scene, 3122)
        {
            Request = new __RPC_Scene_DebugObjPosition_ARG_uint64_characterId_PositionData_pos__();
            Request.CharacterId=characterId;
            Request.Pos=pos;

        }

        public __RPC_Scene_DebugObjPosition_ARG_uint64_characterId_PositionData_pos__ Request { get; private set; }


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

    public class SyncCharacterPostionOutMessage : OutMessage
    {
        public SyncCharacterPostionOutMessage(IAgentBase sender, ulong characterId, PositionData pos)
            : base(sender, ServiceType.Scene, 3124)
        {
            Request = new __RPC_Scene_SyncCharacterPostion_ARG_uint64_characterId_PositionData_pos__();
            Request.CharacterId=characterId;
            Request.Pos=pos;

        }

        public __RPC_Scene_SyncCharacterPostion_ARG_uint64_characterId_PositionData_pos__ Request { get; private set; }


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

    public class ApplySceneExdataOutMessage : OutMessage
    {
        public ApplySceneExdataOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3125)
        {
            Request = new __RPC_Scene_ApplySceneExdata_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_ApplySceneExdata_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_ApplySceneExdata_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplySceneExdata_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ChangePKModelOutMessage : OutMessage
    {
        public ChangePKModelOutMessage(IAgentBase sender, int pkModel)
            : base(sender, ServiceType.Scene, 3126)
        {
            Request = new __RPC_Scene_ChangePKModel_ARG_int32_pkModel__();
            Request.PkModel=pkModel;

        }

        public __RPC_Scene_ChangePKModel_ARG_int32_pkModel__ Request { get; private set; }

            private __RPC_Scene_ChangePKModel_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_ChangePKModel_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class FlyToOutMessage : OutMessage
    {
        public FlyToOutMessage(IAgentBase sender, int sceneId, Vector2Int32 postion)
            : base(sender, ServiceType.Scene, 3128)
        {
            Request = new __RPC_Scene_FlyTo_ARG_int32_sceneId_Vector2Int32_postion__();
            Request.SceneId=sceneId;
            Request.Postion=postion;

        }

        public __RPC_Scene_FlyTo_ARG_int32_sceneId_Vector2Int32_postion__ Request { get; private set; }

            private __RPC_Scene_FlyTo_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_FlyTo_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyBattleReminderOutMessage : OutMessage
    {
        public NotifyBattleReminderOutMessage(IAgentBase sender, int type, string info, int param)
            : base(sender, ServiceType.Scene, 3132)
        {
            Request = new __RPC_Scene_NotifyBattleReminder_ARG_int32_type_string_info_int32_param__();
            Request.Type=type;
            Request.Info=info;
            Request.Param=param;

        }

        public __RPC_Scene_NotifyBattleReminder_ARG_int32_type_string_info_int32_param__ Request { get; private set; }


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

    public class NotifyCountdownOutMessage : OutMessage
    {
        public NotifyCountdownOutMessage(IAgentBase sender, ulong time, int type)
            : base(sender, ServiceType.Scene, 3133)
        {
            Request = new __RPC_Scene_NotifyCountdown_ARG_uint64_time_int32_type__();
            Request.Time=time;
            Request.Type=type;

        }

        public __RPC_Scene_NotifyCountdown_ARG_uint64_time_int32_type__ Request { get; private set; }


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

    public class ApplySceneTeamLeaderObjOutMessage : OutMessage
    {
        public ApplySceneTeamLeaderObjOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3135)
        {
            Request = new __RPC_Scene_ApplySceneTeamLeaderObj_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_ApplySceneTeamLeaderObj_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_ApplySceneTeamLeaderObj_RET_ObjDataListMsg__ mResponse;
            public ObjDataListMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplySceneTeamLeaderObj_RET_ObjDataListMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyDamageListOutMessage : OutMessage
    {
        public NotifyDamageListOutMessage(IAgentBase sender, DamageList list)
            : base(sender, ServiceType.Scene, 3137)
        {
            Request = new __RPC_Scene_NotifyDamageList_ARG_DamageList_list__();
            Request.List=list;

        }

        public __RPC_Scene_NotifyDamageList_ARG_DamageList_list__ Request { get; private set; }


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

    public class NotifyFubenInfoOutMessage : OutMessage
    {
        public NotifyFubenInfoOutMessage(IAgentBase sender, FubenInfoMsg info)
            : base(sender, ServiceType.Scene, 3138)
        {
            Request = new __RPC_Scene_NotifyFubenInfo_ARG_FubenInfoMsg_info__();
            Request.Info=info;

        }

        public __RPC_Scene_NotifyFubenInfo_ARG_FubenInfoMsg_info__ Request { get; private set; }


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

    public class NotifyMessageOutMessage : OutMessage
    {
        public NotifyMessageOutMessage(IAgentBase sender, int type, string info, int addChat)
            : base(sender, ServiceType.Scene, 3141)
        {
            Request = new __RPC_Scene_NotifyMessage_ARG_int32_type_string_info_int32_addChat__();
            Request.Type=type;
            Request.Info=info;
            Request.AddChat=addChat;

        }

        public __RPC_Scene_NotifyMessage_ARG_int32_type_string_info_int32_addChat__ Request { get; private set; }


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

    public class NotifyCampChangeOutMessage : OutMessage
    {
        public NotifyCampChangeOutMessage(IAgentBase sender, int campId, Vector2Int32 pos)
            : base(sender, ServiceType.Scene, 3142)
        {
            Request = new __RPC_Scene_NotifyCampChange_ARG_int32_campId_Vector2Int32_pos__();
            Request.CampId=campId;
            Request.Pos=pos;

        }

        public __RPC_Scene_NotifyCampChange_ARG_int32_campId_Vector2Int32_pos__ Request { get; private set; }


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

    public class GetLeaveExpOutMessage : OutMessage
    {
        public GetLeaveExpOutMessage(IAgentBase sender, int type, int needCount)
            : base(sender, ServiceType.Scene, 3143)
        {
            Request = new __RPC_Scene_GetLeaveExp_ARG_int32_type_int32_needCount__();
            Request.Type=type;
            Request.NeedCount=needCount;

        }

        public __RPC_Scene_GetLeaveExp_ARG_int32_type_int32_needCount__ Request { get; private set; }

            private __RPC_Scene_GetLeaveExp_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_GetLeaveExp_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyLeaveExpOutMessage : OutMessage
    {
        public ApplyLeaveExpOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3144)
        {
            Request = new __RPC_Scene_ApplyLeaveExp_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_ApplyLeaveExp_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_ApplyLeaveExp_RET_LeaveExpData__ mResponse;
            public LeaveExpData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplyLeaveExp_RET_LeaveExpData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncDataToClientOutMessage : OutMessage
    {
        public SyncDataToClientOutMessage(IAgentBase sender, SceneSyncData data)
            : base(sender, ServiceType.Scene, 3150)
        {
            Request = new __RPC_Scene_SyncDataToClient_ARG_SceneSyncData_data__();
            Request.Data=data;

        }

        public __RPC_Scene_SyncDataToClient_ARG_SceneSyncData_data__ Request { get; private set; }


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

    public class SyncMyDataToClientOutMessage : OutMessage
    {
        public SyncMyDataToClientOutMessage(IAgentBase sender, SceneSyncData data)
            : base(sender, ServiceType.Scene, 3151)
        {
            Request = new __RPC_Scene_SyncMyDataToClient_ARG_SceneSyncData_data__();
            Request.Data=data;

        }

        public __RPC_Scene_SyncMyDataToClient_ARG_SceneSyncData_data__ Request { get; private set; }


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

    public class NotifyStrongpointStateChangedOutMessage : OutMessage
    {
        public NotifyStrongpointStateChangedOutMessage(IAgentBase sender, int camp, int index, int state, float time)
            : base(sender, ServiceType.Scene, 3153)
        {
            Request = new __RPC_Scene_NotifyStrongpointStateChanged_ARG_int32_camp_int32_index_int32_state_float_time__();
            Request.Camp=camp;
            Request.Index=index;
            Request.State=state;
            Request.Time=time;

        }

        public __RPC_Scene_NotifyStrongpointStateChanged_ARG_int32_camp_int32_index_int32_state_float_time__ Request { get; private set; }


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

    public class ChangeSceneRequestByMissionOutMessage : OutMessage
    {
        public ChangeSceneRequestByMissionOutMessage(IAgentBase sender, int sceneId, int missionId)
            : base(sender, ServiceType.Scene, 3154)
        {
            Request = new __RPC_Scene_ChangeSceneRequestByMission_ARG_int32_sceneId_int32_missionId__();
            Request.SceneId=sceneId;
            Request.MissionId=missionId;

        }

        public __RPC_Scene_ChangeSceneRequestByMission_ARG_int32_sceneId_int32_missionId__ Request { get; private set; }

            private __RPC_Scene_ChangeSceneRequestByMission_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_ChangeSceneRequestByMission_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPlayerPostionListOutMessage : OutMessage
    {
        public ApplyPlayerPostionListOutMessage(IAgentBase sender, Uint64Array characterIds)
            : base(sender, ServiceType.Scene, 3155)
        {
            Request = new __RPC_Scene_ApplyPlayerPostionList_ARG_Uint64Array_characterIds__();
            Request.CharacterIds=characterIds;

        }

        public __RPC_Scene_ApplyPlayerPostionList_ARG_Uint64Array_characterIds__ Request { get; private set; }

            private __RPC_Scene_ApplyPlayerPostionList_RET_Vec2Array__ mResponse;
            public Vec2Array Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_ApplyPlayerPostionList_RET_Vec2Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncObjPositionOutMessage : OutMessage
    {
        public SyncObjPositionOutMessage(IAgentBase sender, SyncPathPosMsg msg)
            : base(sender, ServiceType.Scene, 3156)
        {
            Request = new __RPC_Scene_SyncObjPosition_ARG_SyncPathPosMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SyncObjPosition_ARG_SyncPathPosMsg_msg__ Request { get; private set; }


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

    public class InspireOutMessage : OutMessage
    {
        public InspireOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Scene, 3159)
        {
            Request = new __RPC_Scene_Inspire_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_Inspire_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Scene_Inspire_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_Inspire_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ObjSpeakOutMessage : OutMessage
    {
        public ObjSpeakOutMessage(IAgentBase sender, ulong id, int dictId, string content)
            : base(sender, ServiceType.Scene, 3160)
        {
            Request = new __RPC_Scene_ObjSpeak_ARG_uint64_id_int32_dictId_string_content__();
            Request.Id=id;
            Request.DictId=dictId;
            Request.Content=content;

        }

        public __RPC_Scene_ObjSpeak_ARG_uint64_id_int32_dictId_string_content__ Request { get; private set; }


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

    public class SyncLevelChangeOutMessage : OutMessage
    {
        public SyncLevelChangeOutMessage(IAgentBase sender, LevelUpAttrData Attr)
            : base(sender, ServiceType.Scene, 3163)
        {
            Request = new __RPC_Scene_SyncLevelChange_ARG_LevelUpAttrData_Attr__();
            Request.Attr=Attr;

        }

        public __RPC_Scene_SyncLevelChange_ARG_LevelUpAttrData_Attr__ Request { get; private set; }


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

    public class AllianceWarRespawnGuardOutMessage : OutMessage
    {
        public AllianceWarRespawnGuardOutMessage(IAgentBase sender, int index)
            : base(sender, ServiceType.Scene, 3164)
        {
            Request = new __RPC_Scene_AllianceWarRespawnGuard_ARG_int32_index__();
            Request.Index=index;

        }

        public __RPC_Scene_AllianceWarRespawnGuard_ARG_int32_index__ Request { get; private set; }

            private __RPC_Scene_AllianceWarRespawnGuard_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_AllianceWarRespawnGuard_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyAllianceWarNpcDataOutMessage : OutMessage
    {
        public NotifyAllianceWarNpcDataOutMessage(IAgentBase sender, int reliveCount, Int32Array data)
            : base(sender, ServiceType.Scene, 3166)
        {
            Request = new __RPC_Scene_NotifyAllianceWarNpcData_ARG_int32_reliveCount_Int32Array_data__();
            Request.ReliveCount=reliveCount;
            Request.Data=data;

        }

        public __RPC_Scene_NotifyAllianceWarNpcData_ARG_int32_reliveCount_Int32Array_data__ Request { get; private set; }


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

    public class NotifyScenePlayerInfosOutMessage : OutMessage
    {
        public NotifyScenePlayerInfosOutMessage(IAgentBase sender, ScenePlayerInfos info)
            : base(sender, ServiceType.Scene, 3167)
        {
            Request = new __RPC_Scene_NotifyScenePlayerInfos_ARG_ScenePlayerInfos_info__();
            Request.Info=info;

        }

        public __RPC_Scene_NotifyScenePlayerInfos_ARG_ScenePlayerInfos_info__ Request { get; private set; }


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

    public class NotifyNpcStatusOutMessage : OutMessage
    {
        public NotifyNpcStatusOutMessage(IAgentBase sender, MapNpcInfos infos)
            : base(sender, ServiceType.Scene, 3168)
        {
            Request = new __RPC_Scene_NotifyNpcStatus_ARG_MapNpcInfos_infos__();
            Request.Infos=infos;

        }

        public __RPC_Scene_NotifyNpcStatus_ARG_MapNpcInfos_infos__ Request { get; private set; }


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

    public class NotifyPointListOutMessage : OutMessage
    {
        public NotifyPointListOutMessage(IAgentBase sender, PointList list)
            : base(sender, ServiceType.Scene, 3173)
        {
            Request = new __RPC_Scene_NotifyPointList_ARG_PointList_list__();
            Request.List=list;

        }

        public __RPC_Scene_NotifyPointList_ARG_PointList_list__ Request { get; private set; }


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

    public class NotifyStartWarningOutMessage : OutMessage
    {
        public NotifyStartWarningOutMessage(IAgentBase sender, ulong timeOut)
            : base(sender, ServiceType.Scene, 3174)
        {
            Request = new __RPC_Scene_NotifyStartWarning_ARG_uint64_timeOut__();
            Request.TimeOut=timeOut;

        }

        public __RPC_Scene_NotifyStartWarning_ARG_uint64_timeOut__ Request { get; private set; }


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

    public class GetSceneNpcPosOutMessage : OutMessage
    {
        public GetSceneNpcPosOutMessage(IAgentBase sender, uint placeholder)
            : base(sender, ServiceType.Scene, 3601)
        {
            Request = new __RPC_Scene_GetSceneNpcPos_ARG_uint32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Scene_GetSceneNpcPos_ARG_uint32_placeholder__ Request { get; private set; }

            private __RPC_Scene_GetSceneNpcPos_RET_SceneNpcPosList__ mResponse;
            public SceneNpcPosList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Scene_GetSceneNpcPos_RET_SceneNpcPosList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SendMieshiResultOutMessage : OutMessage
    {
        public SendMieshiResultOutMessage(IAgentBase sender, MieshiResultMsg msg)
            : base(sender, ServiceType.Scene, 3602)
        {
            Request = new __RPC_Scene_SendMieshiResult_ARG_MieshiResultMsg_msg__();
            Request.Msg=msg;

        }

        public __RPC_Scene_SendMieshiResult_ARG_MieshiResultMsg_msg__ Request { get; private set; }


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

    public class NotifyRefreshDungeonInfoOutMessage : OutMessage
    {
        public NotifyRefreshDungeonInfoOutMessage(IAgentBase sender, DungeonInfo info)
            : base(sender, ServiceType.Scene, 3604)
        {
            Request = new __RPC_Scene_NotifyRefreshDungeonInfo_ARG_DungeonInfo_info__();
            Request.Info=info;

        }

        public __RPC_Scene_NotifyRefreshDungeonInfo_ARG_DungeonInfo_info__ Request { get; private set; }


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

    public class SyncModelIdOutMessage : OutMessage
    {
        public SyncModelIdOutMessage(IAgentBase sender, int model)
            : base(sender, ServiceType.Scene, 3992)
        {
            Request = new __RPC_Scene_SyncModelId_ARG_int32_model__();
            Request.Model=model;

        }

        public __RPC_Scene_SyncModelId_ARG_int32_model__ Request { get; private set; }


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

    public class SyncFuBenStoreOutMessage : OutMessage
    {
        public SyncFuBenStoreOutMessage(IAgentBase sender, StoneItems itemlst, int storeType)
            : base(sender, ServiceType.Scene, 3997)
        {
            Request = new __RPC_Scene_SyncFuBenStore_ARG_StoneItems_itemlst_int32_storeType__();
            Request.Itemlst=itemlst;
            Request.StoreType=storeType;

        }

        public __RPC_Scene_SyncFuBenStore_ARG_StoneItems_itemlst_int32_storeType__ Request { get; private set; }


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

    public class FastReachOutMessage : OutMessage
    {
        public FastReachOutMessage(IAgentBase sender, int sceneId, Vector2Int32 postion)
            : base(sender, ServiceType.Scene, 3998)
        {
            Request = new __RPC_Scene_FastReach_ARG_int32_sceneId_Vector2Int32_postion__();
            Request.SceneId=sceneId;
            Request.Postion=postion;

        }

        public __RPC_Scene_FastReach_ARG_int32_sceneId_Vector2Int32_postion__ Request { get; private set; }

            private __RPC_Scene_FastReach_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Scene_FastReach_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class BroadcastSceneChatOutMessage : OutMessage
    {
        public BroadcastSceneChatOutMessage(IAgentBase sender, string content, int dictId)
            : base(sender, ServiceType.Scene, 3999)
        {
            Request = new __RPC_Scene_BroadcastSceneChat_ARG_string_content_int32_dictId__();
            Request.Content=content;
            Request.DictId=dictId;

        }

        public __RPC_Scene_BroadcastSceneChat_ARG_string_content_int32_dictId__ Request { get; private set; }


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
