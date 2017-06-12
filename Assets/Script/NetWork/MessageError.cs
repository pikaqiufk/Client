#region using

using ClientService;
using ScorpionNetLib;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    public override bool OnMessageTimeout(OutMessage message)
    {
        if (message.mMessage.FuncId == 1999)
        {
            return true;
        }
        Logger.Error("Message time out, {0} {1}", message.GetType().Name, message.mMessage.FuncId);
        switch (message.mMessage.FuncId)
        {
            case 1046: // ApplySkill
            case 1049: // ApplyBags
            case 1050: // ApplyFlag
            case 1051: // ApplyExdata
            case 1052: // ApplyExdata64
            case 1053: // ApplyMission
            case 1054: // ApplyBooks
            case 1124: // ApplyCityData
            case 1112: // ApplyMails
            case 1180: // StoreOperationLookSelf
            case 7054: // ApplyQueueData
                GameUtils.ShowLoginTimeOutTip();
                break;

            case 1047: // UpgradeInnate ////升级天赋
            case 1048: // ClearInnate ////重置天赋
            case 1055: // ReplaceEquip ////换装备
            case 1056: // AcceptMission ////接受任务
            case 1057: // CommitMission ////提交任务
            case 1058: // CompleteMission ////完成任务
            case 1059: // DropMission ////放弃任务
            case 1068: // EquipSkill ////装备技能
            case 1069: // UpgradeSkill ////升级技能
            case 1070: // SellBagItem ////道具出售
            case 1071: // RecycleBagItem ////道具回收
            case 1072: // EnchanceEquip //
            case 1073: // AppendEquip ////强化追加
            case 1074: // ResetExcellentEquip ////装备洗练
            case 1075: // ConfirmResetExcellentEquip ////洗炼结果确认
            case 1076: // SuperExcellentEquip ////锁定洗炼
            case 1077: // SmritiEquip ////传承
            case 1078: // UseItem ////使用道具
            case 1084: // ActivationReward ////游戏领奖
            case 1085: // LogicChatMessage ////接受聊天数据
            case 1086: // ComposeItem ////合成道具
            case 1088: // RewardAchievement ////成就领奖
            case 1089: // DistributionAttrPoint ////分配属性点
            case 1090: // RefreshAttrPoint ////洗点
            case 1091: // SetAttributeAutoAdd ////设置自动加点
            case 1092: // ApplyFriends ////请求好友数据0:好友，1:敌人，2:屏蔽
            case 1093: // SeekCharacters ////查找玩家
            case 1094: // SeekFriends ////查找好友
            case 1095: // AddFriendById ////添加好友
            case 1097: // AddFriendByName ////添加好友
            case 1098: // DelFriendById ////删除好友
            case 1102: // SelectDungeonReward ////通知logic，我选择的奖励
            case 1103: // EnterFuben ////进入副本
            case 1104: // ResetFuben ////重置副本
            case 1105: // SweepFuben ////扫荡副本
            case 1106: // ApplyStores ////获得商店的道具列表
            case 1107: // ActivateBook ////激活图鉴
            case 1108: // SortBag ////整理包裹
            case 1109: // ApplyPlayerInfo ////请求玩家的详细信息
            case 1110: // SetFlag ////通知服务器修改标志位
            case 1111: // SetExData ////通知服务器修改扩展数据
            case 1113: // ApplyMailInfo ////请求邮件详细数据
            case 1114: // ReceiveMail ////领取邮件
            case 1115: // DeleteMail ////删除邮件
            case 1118: // RepairEquip ////远程修理
            case 1120: // DepotTakeOut ////取出仓库
            case 1121: // DepotPutIn ////放进仓库
            case 1122: // WishingPoolDepotTakeOut ////取出许愿池仓库,index = -1全部取出
            case 1123: // StoreBuy ////商店购买
            case 1125: // CityOperationRequest ////家园操作
            case 1128: // EnterCity ////请求进入家园
            case 1131: // ApplyEquipDurable ////有装备损坏
            case 1132: // ElfOperate ////精灵相关接口//Type:0休息, 1出战，2展示 //3休息//4升级阵法//5升级//6分解
            case 1133: // ElfReplace //
            case 1136: // WingFormation ////翅膀升阶
            case 1137: // WingTrain ////翅膀培养
            case 1138: // OperatePet ////宠物操作
            case 1139: // OperatePetMission ////操作宠任务
            case 1140: // PickUpMedal ////拾取勋章
            case 1141: // EnchanceMedal ////强化勋章
            case 1142: // EquipMedal ////装备勋章
            case 1143: // BuySpaceBag ////开启包裹
            case 1145: // UseBuildService ////建筑服务通用接口
            case 1146: // GetP1vP1LadderPlayer ////请求天梯玩家信息
            case 1147: // GetP1vP1FightPlayer ////攻击某个天梯的玩家//0  正常//1  cd购买//2  次数购买
            case 1151: // GetP1vP1LadderOldList ////天梯战斗历史
            case 1153: // BuyP1vP1Count ////购买天梯次数（这个接口不用了）
            case 1154: // DrawLotteryPetEgg ////宠物蛋抽奖
            case 1155: // RecoveryEquip ////回收装备
            case 1156: // DrawWishingPool ////许愿池抽奖（这个接口不用了）
            case 1157: // ResetSkillTalent ////重置技能天赋
            case 1158: // RobotcFinishFuben ////机器人完成副本
            case 1159: // CreateAlliance ////战盟操作:创建
            case 1160:
                // AllianceOperation ////战盟操作:其他操作 type：0=申请加入（value=战盟ID）  1=取消申请（value=战盟ID）  2=退出战盟   3=同意邀请（value=战盟ID）  4=拒绝邀请（value=战盟ID）
            case 1161: // AllianceOperationCharacter ////战盟操作:其他操作 type：0=邀请加入 1=同意申请加入 2：拒绝申请
            case 1162: // AllianceOperationCharacterByName ////战盟操作:其他操作 type：0=邀请加入
            case 1166: // WorshipCharacter ////崇拜
            case 1168: // DonationAllianceItem ////战盟捐献
            case 1172: // CityMissionOperation ////家园任务：type 0=提交 2=购买
            case 1173: // DropCityMission ////家园任务：放弃
            case 1174: // CityRefreshMission ////家园任务刷新
            case 1175: // StoreOperationAdd ////交易系统：上架道具
            case 1176: // StoreOperationBroadcast ////交易系统：广播道具
            case 1177: // StoreOperationBuy ////交易系统：购买道具
            case 1178: // StoreOperationCancel ////交易系统：收回道具
            case 1179: // StoreOperationLook ////交易系统：查看某人
            case 1181: // StoreOperationHarvest ////交易系统：获取自己已贩卖的收获
            case 1183: // SSStoreOperationExchange ////交易系统：兑换道具
            case 1185: // ApplyGroupShopItems ////团购申请
            case 1187: // BuyGroupShopItem ////购买团购
            case 1188: // GetBuyedGroupShopItems ////获取我当前的愿望
            case 1189: // GetGroupShopHistory ////获取团购历史
            case 1193: // AcceptBattleAward //// 领取战场奖励
            case 1194: // AstrologyLevelUp //// 占星台宝石升级
            case 1195: // AstrologyEquipOn //// 占星台宝石装备
            case 1196: // AstrologyEquipOff //// 占星台宝石卸载
            case 1199: // UsePetExpItem ////对随从使用经验药
            case 1200: // Reincarnation ////转生接口
            case 1201: // UpgradeHonor ////升级军衔
            case 1206: // ApplyCityBuildingData ////根据区域id请求建筑数据

            case 6043: // GetRankList ////获取排行榜数据

            case 7041: // TeamMessage ////组队接口
            case 7043: // ApplyTeam ////获取组队信息
            case 7044: // TeamChatMessage ////接受聊天数据
            case 7045: // TeamDungeonLineUp ////副本排队
            case 7047: // MatchingStart ////排队开始
            case 7048: // MatchingCancel ////取消排队
            case 7051: // MatchingBack ////通知反馈
            case 7055: // TeamEnterFuben ////请求进入多人副本
            case 7057: // ResultTeamEnterFuben ////同意进入多人副本
            case 7059: // GMTeam ////GM命令
            case 7065: // ApplyAllianceData ////获取战盟信息
            case 7066: // ApplyAllianceDataByServerId ////获取战盟信息          type:  0 = 详细信息, 1 = 简单信息
            case 7067: // ChangeAllianceNotice ////修改战盟公告
            case 7068: // GetServerAlliance ////获取服务器战盟信息
            case 7069: // ChangeJurisdiction ////修改角色权限
            case 7070: // ChangeAllianceAutoJoin ////修改战盟设置为是否自动同意申请
            case 7071: // AllianceAgreeApplyList ////批量同意的战盟申请
            case 7077: // UpgradeAllianceBuff ////升级Buff
            case 7081: // ApplyAllianceMissionData ////请求战盟信息任务数据
            case 7082: // UpgradeAllianceLevel ////升级战盟等级
            case 7090: // ApplyAllianceEnjoyList ////获取战盟信息申请列表
            case 7091: // ApplyAllianceDonationList ////获取战盟捐献记录

                EventSystem.EventDispatcher.Instance.DispatchEvent(new EventSystem.ShowUIHintBoard(199999999));

                break;

            default:
                break;
        }

        return true;
    }
}