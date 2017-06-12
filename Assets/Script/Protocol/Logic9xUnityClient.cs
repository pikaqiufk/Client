using System;
using System.Collections;
using System.IO;
using ScorpionNetLib;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ClientService
{

	public interface ILogic9xServiceInterface : IAgentBase
    {
        /// <summary>
        /// 同步任务数据
        /// </summary>
        void SyncMission(int missionId, int state, int param);
        /// <summary>
        /// 同步标记位
        /// </summary>
        void SyncFlag(int flagId, int param);
        /// <summary>
        /// 同步标记位
        /// </summary>
        void SyncFlagList(Int32Array trueList, Int32Array falseList);
        /// <summary>
        /// 同步扩展数据
        /// </summary>
        void SyncExdata(int exdataId, int value);
        /// <summary>
        /// 同步扩展数据
        /// </summary>
        void SyncExdataList(Dict_int_int_Data diff);
        /// <summary>
        /// 同步64位扩展数据
        /// </summary>
        void SyncExdata64(int exdataId, long value);
        /// <summary>
        /// 同步资源数据
        /// </summary>
        void SyncResources(int resId, int value);
        /// <summary>
        /// 同步包裹变化
        /// </summary>
        void SyncItems(BagsChangeData bag);
        /// <summary>
        /// 同步任务变化
        /// </summary>
        void SyncMissions(MissionDataMessage missions);
        /// <summary>
        /// 成就完成提示
        /// </summary>
        void FinishAchievement(int achievementId);
        /// <summary>
        /// </summary>
        void SeekCharactersReceive(CharacterSimpleDataList result);
        /// <summary>
        /// </summary>
        void SeekFriendsReceive(CharacterSimpleDataList result);
        /// <summary>
        /// 副本结束，通知结果
        /// </summary>
        void DungeonComplete(FubenResult result);
        /// <summary>
        /// 主动更新邮件
        /// </summary>
        void SyncMails(MailList mails);
        /// <summary>
        /// 同步家园建筑数据
        /// </summary>
        void SyncCityBuildingData(BuildingList data);
        /// <summary>
        /// 同步随从任务数据
        /// </summary>
        void SyncPetMission(PetMissionList msg);
        /// <summary>
        /// 有装备损坏
        /// </summary>
        void EquipDurableBroken(int partId, int value);
        /// <summary>
        /// 装备耐久第一次变化，希望客户端可以下次开界面时请求耐久
        /// </summary>
        void EquipDurableChange(int placeholder);
        /// <summary>
        /// 删除宠物任务
        /// </summary>
        void DeletePetMission(int missionId);
        /// <summary>
        /// 天梯的奖励界面
        /// </summary>
        void LogicP1vP1FightResult(P1vP1RewardData data);
        /// <summary>
        /// 战盟信息通知 type：0=name1邀请您加入name2的战盟
        /// </summary>
        void LogicSyncAllianceMessage(int type, string name1, int allianceId, string name2);
        /// <summary>
        /// 精灵的抽奖结果
        /// </summary>
        void ElfDrawOver(DrawItemResult Items, long getTime);
        /// <summary>
        /// 增加天赋数量变化的通知界面
        /// </summary>
        void TalentCountChange(int talentId, int value);
        /// <summary>
        /// 通知有东西被买了
        /// </summary>
        void NotifyStoreBuyed(long storeId, ulong Aid, string Aname);
        /// <summary>
        /// 占星台抽奖结果
        /// </summary>
        void AstrologyDrawOver(DrawItemResult Items, long getTime);
        /// <summary>
        /// 额外增加了仇人
        /// </summary>
        void SyncAddFriend(int type, CharacterSimpleData character);
        /// <summary>
        /// 通知一些消息 type:0提示字典
        /// </summary>
        void LogicNotifyMessage(int type, string info, int addChat);
        /// <summary>
        /// 通知客户端获得经验
        /// </summary>
        void NotifGainRes(DataChangeList changes);
        /// <summary>
        /// 通知战场的结果界面信息
        /// </summary>
        void BattleResult(int dungeonId, int resultType, int first);
        /// <summary>
        /// 自己被打了
        /// </summary>
        void NotifyP1vP1Change(P1vP1Change_One one);
        /// <summary>
        /// 通知好友数据变化
        /// </summary>
        void SyncFriendDataChange(CharacterSimpleDataList Changes);
        /// <summary>
        /// 通知好友删除
        /// </summary>
        void SyncFriendDelete(int type, ulong characterId);
        /// <summary>
        /// 通知客户端，充值成功
        /// </summary>
        void NotifyRechargeSuccess(int rechargeId);
        /// <summary>
        /// 同步运营活动内容
        /// </summary>
        void SyncOperationActivityItem(MsgOperActivtyItemList items);
        /// <summary>
        /// 同步运营活动内容
        /// </summary>
        void SyncOperationActivityTerm(int id, int param);
    }
    public static class Logic9xServiceInterfaceExtension
    {

        public static GMLogicOutMessage GMLogic(this ILogic9xServiceInterface agent, string commond)
        {
            return new GMLogicOutMessage(agent, commond);
        }

        public static ApplySkillOutMessage ApplySkill(this ILogic9xServiceInterface agent, uint placeholder)
        {
            return new ApplySkillOutMessage(agent, placeholder);
        }

        public static UpgradeInnateOutMessage UpgradeInnate(this ILogic9xServiceInterface agent, int innateId)
        {
            return new UpgradeInnateOutMessage(agent, innateId);
        }

        public static ClearInnateOutMessage ClearInnate(this ILogic9xServiceInterface agent, int innateId)
        {
            return new ClearInnateOutMessage(agent, innateId);
        }

        public static ApplyBagsOutMessage ApplyBags(this ILogic9xServiceInterface agent, int type)
        {
            return new ApplyBagsOutMessage(agent, type);
        }

        public static ApplyFlagOutMessage ApplyFlag(this ILogic9xServiceInterface agent, int flagId)
        {
            return new ApplyFlagOutMessage(agent, flagId);
        }

        public static ApplyExdataOutMessage ApplyExdata(this ILogic9xServiceInterface agent, int exdataId)
        {
            return new ApplyExdataOutMessage(agent, exdataId);
        }

        public static ApplyExdata64OutMessage ApplyExdata64(this ILogic9xServiceInterface agent, int exdataId)
        {
            return new ApplyExdata64OutMessage(agent, exdataId);
        }

        public static ApplyMissionOutMessage ApplyMission(this ILogic9xServiceInterface agent, int missionId)
        {
            return new ApplyMissionOutMessage(agent, missionId);
        }

        public static ApplyBooksOutMessage ApplyBooks(this ILogic9xServiceInterface agent, uint placeholder)
        {
            return new ApplyBooksOutMessage(agent, placeholder);
        }

        public static ReplaceEquipOutMessage ReplaceEquip(this ILogic9xServiceInterface agent, int bagItemId, int part, int index)
        {
            return new ReplaceEquipOutMessage(agent, bagItemId, part, index);
        }

        public static AcceptMissionOutMessage AcceptMission(this ILogic9xServiceInterface agent, int missionId)
        {
            return new AcceptMissionOutMessage(agent, missionId);
        }

        public static CommitMissionOutMessage CommitMission(this ILogic9xServiceInterface agent, int missionId)
        {
            return new CommitMissionOutMessage(agent, missionId);
        }

        public static CompleteMissionOutMessage CompleteMission(this ILogic9xServiceInterface agent, int missionId)
        {
            return new CompleteMissionOutMessage(agent, missionId);
        }

        public static DropMissionOutMessage DropMission(this ILogic9xServiceInterface agent, int missionId)
        {
            return new DropMissionOutMessage(agent, missionId);
        }

        public static EquipSkillOutMessage EquipSkill(this ILogic9xServiceInterface agent, Int32Array EquipSkills)
        {
            return new EquipSkillOutMessage(agent, EquipSkills);
        }

        public static UpgradeSkillOutMessage UpgradeSkill(this ILogic9xServiceInterface agent, int skillId)
        {
            return new UpgradeSkillOutMessage(agent, skillId);
        }

        public static SellBagItemOutMessage SellBagItem(this ILogic9xServiceInterface agent, int bagType, int itemId, int index, int count)
        {
            return new SellBagItemOutMessage(agent, bagType, itemId, index, count);
        }

        public static RecycleBagItemOutMessage RecycleBagItem(this ILogic9xServiceInterface agent, int bagType, int itemId, int index, int count)
        {
            return new RecycleBagItemOutMessage(agent, bagType, itemId, index, count);
        }

        public static EnchanceEquipOutMessage EnchanceEquip(this ILogic9xServiceInterface agent, int bagType, int bagIndex, int blessing, int upRate)
        {
            return new EnchanceEquipOutMessage(agent, bagType, bagIndex, blessing, upRate);
        }

        public static AppendEquipOutMessage AppendEquip(this ILogic9xServiceInterface agent, int bagType, int bagIndex)
        {
            return new AppendEquipOutMessage(agent, bagType, bagIndex);
        }

        public static ResetExcellentEquipOutMessage ResetExcellentEquip(this ILogic9xServiceInterface agent, int bagType, int bagIndex)
        {
            return new ResetExcellentEquipOutMessage(agent, bagType, bagIndex);
        }

        public static ConfirmResetExcellentEquipOutMessage ConfirmResetExcellentEquip(this ILogic9xServiceInterface agent, int bagType, int bagIndex, int ok)
        {
            return new ConfirmResetExcellentEquipOutMessage(agent, bagType, bagIndex, ok);
        }

        public static SuperExcellentEquipOutMessage SuperExcellentEquip(this ILogic9xServiceInterface agent, int bagType, int bagIndex, Int32Array LockList)
        {
            return new SuperExcellentEquipOutMessage(agent, bagType, bagIndex, LockList);
        }

        public static SmritiEquipOutMessage SmritiEquip(this ILogic9xServiceInterface agent, int smritiType, int moneyType, int fromBagType, int fromBagIndex, int toBagType, int toBagIndex)
        {
            return new SmritiEquipOutMessage(agent, smritiType, moneyType, fromBagType, fromBagIndex, toBagType, toBagIndex);
        }

        public static UseItemOutMessage UseItem(this ILogic9xServiceInterface agent, int bagType, int bagIndex, int count)
        {
            return new UseItemOutMessage(agent, bagType, bagIndex, count);
        }

        public static ActivationRewardOutMessage ActivationReward(this ILogic9xServiceInterface agent, int typeId, int giftId)
        {
            return new ActivationRewardOutMessage(agent, typeId, giftId);
        }

        public static ComposeItemOutMessage ComposeItem(this ILogic9xServiceInterface agent, int composeId, int count)
        {
            return new ComposeItemOutMessage(agent, composeId, count);
        }

        public static RewardAchievementOutMessage RewardAchievement(this ILogic9xServiceInterface agent, int achievementId)
        {
            return new RewardAchievementOutMessage(agent, achievementId);
        }

        public static DistributionAttrPointOutMessage DistributionAttrPoint(this ILogic9xServiceInterface agent, int Strength, int Agility, int Intelligence, int Endurance)
        {
            return new DistributionAttrPointOutMessage(agent, Strength, Agility, Intelligence, Endurance);
        }

        public static RefreshAttrPointOutMessage RefreshAttrPoint(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new RefreshAttrPointOutMessage(agent, placeholder);
        }

        public static SetAttributeAutoAddOutMessage SetAttributeAutoAdd(this ILogic9xServiceInterface agent, int isAuto)
        {
            return new SetAttributeAutoAddOutMessage(agent, isAuto);
        }

        public static ApplyFriendsOutMessage ApplyFriends(this ILogic9xServiceInterface agent, int type)
        {
            return new ApplyFriendsOutMessage(agent, type);
        }

        public static SeekCharactersOutMessage SeekCharacters(this ILogic9xServiceInterface agent, string name)
        {
            return new SeekCharactersOutMessage(agent, name);
        }

        public static SeekFriendsOutMessage SeekFriends(this ILogic9xServiceInterface agent, string name)
        {
            return new SeekFriendsOutMessage(agent, name);
        }

        public static AddFriendByIdOutMessage AddFriendById(this ILogic9xServiceInterface agent, ulong characterId, int type)
        {
            return new AddFriendByIdOutMessage(agent, characterId, type);
        }

        public static AddFriendByNameOutMessage AddFriendByName(this ILogic9xServiceInterface agent, string Name, int type)
        {
            return new AddFriendByNameOutMessage(agent, Name, type);
        }

        public static DelFriendByIdOutMessage DelFriendById(this ILogic9xServiceInterface agent, ulong characterId, int type)
        {
            return new DelFriendByIdOutMessage(agent, characterId, type);
        }

        public static SelectDungeonRewardOutMessage SelectDungeonReward(this ILogic9xServiceInterface agent, int fubenId, int select)
        {
            return new SelectDungeonRewardOutMessage(agent, fubenId, select);
        }

        public static EnterFubenOutMessage EnterFuben(this ILogic9xServiceInterface agent, int fubenId)
        {
            return new EnterFubenOutMessage(agent, fubenId);
        }

        public static ResetFubenOutMessage ResetFuben(this ILogic9xServiceInterface agent, int fubenId)
        {
            return new ResetFubenOutMessage(agent, fubenId);
        }

        public static SweepFubenOutMessage SweepFuben(this ILogic9xServiceInterface agent, int fubenId)
        {
            return new SweepFubenOutMessage(agent, fubenId);
        }

        public static ApplyStoresOutMessage ApplyStores(this ILogic9xServiceInterface agent, int type, int serviceType)
        {
            return new ApplyStoresOutMessage(agent, type, serviceType);
        }

        public static ActivateBookOutMessage ActivateBook(this ILogic9xServiceInterface agent, int itemId, int groupId, int index)
        {
            return new ActivateBookOutMessage(agent, itemId, groupId, index);
        }

        public static SortBagOutMessage SortBag(this ILogic9xServiceInterface agent, int bagId)
        {
            return new SortBagOutMessage(agent, bagId);
        }

        public static ApplyPlayerInfoOutMessage ApplyPlayerInfo(this ILogic9xServiceInterface agent, ulong characterId)
        {
            return new ApplyPlayerInfoOutMessage(agent, characterId);
        }

        public static SetFlagOutMessage SetFlag(this ILogic9xServiceInterface agent, Int32Array trueDatas, Int32Array falseDatas)
        {
            return new SetFlagOutMessage(agent, trueDatas, falseDatas);
        }

        public static SetExDataOutMessage SetExData(this ILogic9xServiceInterface agent, Dict_int_int_Data datas)
        {
            return new SetExDataOutMessage(agent, datas);
        }

        public static ApplyMailsOutMessage ApplyMails(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new ApplyMailsOutMessage(agent, placeholder);
        }

        public static ApplyMailInfoOutMessage ApplyMailInfo(this ILogic9xServiceInterface agent, ulong mailId)
        {
            return new ApplyMailInfoOutMessage(agent, mailId);
        }

        public static ReceiveMailOutMessage ReceiveMail(this ILogic9xServiceInterface agent, Uint64Array mails)
        {
            return new ReceiveMailOutMessage(agent, mails);
        }

        public static DeleteMailOutMessage DeleteMail(this ILogic9xServiceInterface agent, Uint64Array mails)
        {
            return new DeleteMailOutMessage(agent, mails);
        }

        public static RepairEquipOutMessage RepairEquip(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new RepairEquipOutMessage(agent, placeholder);
        }

        public static DepotTakeOutOutMessage DepotTakeOut(this ILogic9xServiceInterface agent, int index)
        {
            return new DepotTakeOutOutMessage(agent, index);
        }

        public static DepotPutInOutMessage DepotPutIn(this ILogic9xServiceInterface agent, int bagId, int index)
        {
            return new DepotPutInOutMessage(agent, bagId, index);
        }

        public static WishingPoolDepotTakeOutOutMessage WishingPoolDepotTakeOut(this ILogic9xServiceInterface agent, int index)
        {
            return new WishingPoolDepotTakeOutOutMessage(agent, index);
        }

        public static StoreBuyOutMessage StoreBuy(this ILogic9xServiceInterface agent, int storeId, int count, int serviceType)
        {
            return new StoreBuyOutMessage(agent, storeId, count, serviceType);
        }

        public static ApplyCityDataOutMessage ApplyCityData(this ILogic9xServiceInterface agent, int buildingId)
        {
            return new ApplyCityDataOutMessage(agent, buildingId);
        }

        public static CityOperationRequestOutMessage CityOperationRequest(this ILogic9xServiceInterface agent, int opType, int buildingIdx, Int32Array param)
        {
            return new CityOperationRequestOutMessage(agent, opType, buildingIdx, param);
        }

        public static EnterCityOutMessage EnterCity(this ILogic9xServiceInterface agent, int cityId)
        {
            return new EnterCityOutMessage(agent, cityId);
        }

        public static ApplyEquipDurableOutMessage ApplyEquipDurable(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new ApplyEquipDurableOutMessage(agent, placeholder);
        }

        public static ElfOperateOutMessage ElfOperate(this ILogic9xServiceInterface agent, int index, int type, int targetIndex)
        {
            return new ElfOperateOutMessage(agent, index, type, targetIndex);
        }

        public static ElfReplaceOutMessage ElfReplace(this ILogic9xServiceInterface agent, int from, int to)
        {
            return new ElfReplaceOutMessage(agent, from, to);
        }

        public static WingFormationOutMessage WingFormation(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new WingFormationOutMessage(agent, placeholder);
        }

        public static WingTrainOutMessage WingTrain(this ILogic9xServiceInterface agent, int type)
        {
            return new WingTrainOutMessage(agent, type);
        }

        public static OperatePetOutMessage OperatePet(this ILogic9xServiceInterface agent, int petId, int type, int param)
        {
            return new OperatePetOutMessage(agent, petId, type, param);
        }

        public static OperatePetMissionOutMessage OperatePetMission(this ILogic9xServiceInterface agent, int id, int type, Int32Array param)
        {
            return new OperatePetMissionOutMessage(agent, id, type, param);
        }

        public static PickUpMedalOutMessage PickUpMedal(this ILogic9xServiceInterface agent, int index)
        {
            return new PickUpMedalOutMessage(agent, index);
        }

        public static EnchanceMedalOutMessage EnchanceMedal(this ILogic9xServiceInterface agent, int bagId, int bagIndex, Int32Array medalTempBag, Int32Array medalUseBag)
        {
            return new EnchanceMedalOutMessage(agent, bagId, bagIndex, medalTempBag, medalUseBag);
        }

        public static EquipMedalOutMessage EquipMedal(this ILogic9xServiceInterface agent, int bagId, int bagIndex)
        {
            return new EquipMedalOutMessage(agent, bagId, bagIndex);
        }

        public static BuySpaceBagOutMessage BuySpaceBag(this ILogic9xServiceInterface agent, int bagId, int bagIndex, int needCount)
        {
            return new BuySpaceBagOutMessage(agent, bagId, bagIndex, needCount);
        }

        public static UseBuildServiceOutMessage UseBuildService(this ILogic9xServiceInterface agent, int areaId, int serviceId, Int32Array param)
        {
            return new UseBuildServiceOutMessage(agent, areaId, serviceId, param);
        }

        public static GetP1vP1LadderPlayerOutMessage GetP1vP1LadderPlayer(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new GetP1vP1LadderPlayerOutMessage(agent, placeholder);
        }

        public static GetP1vP1FightPlayerOutMessage GetP1vP1FightPlayer(this ILogic9xServiceInterface agent, int rank, ulong guid, int type)
        {
            return new GetP1vP1FightPlayerOutMessage(agent, rank, guid, type);
        }

        public static GetP1vP1LadderOldListOutMessage GetP1vP1LadderOldList(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new GetP1vP1LadderOldListOutMessage(agent, placeholder);
        }

        public static BuyP1vP1CountOutMessage BuyP1vP1Count(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new BuyP1vP1CountOutMessage(agent, placeholder);
        }

        public static DrawLotteryPetEggOutMessage DrawLotteryPetEgg(this ILogic9xServiceInterface agent, int type)
        {
            return new DrawLotteryPetEggOutMessage(agent, type);
        }

        public static RecoveryEquipOutMessage RecoveryEquip(this ILogic9xServiceInterface agent, int type, Int32Array indexList)
        {
            return new RecoveryEquipOutMessage(agent, type, indexList);
        }

        public static DrawWishingPoolOutMessage DrawWishingPool(this ILogic9xServiceInterface agent, int type)
        {
            return new DrawWishingPoolOutMessage(agent, type);
        }

        public static ResetSkillTalentOutMessage ResetSkillTalent(this ILogic9xServiceInterface agent, int skillId)
        {
            return new ResetSkillTalentOutMessage(agent, skillId);
        }

        public static RobotcFinishFubenOutMessage RobotcFinishFuben(this ILogic9xServiceInterface agent, int fubenId)
        {
            return new RobotcFinishFubenOutMessage(agent, fubenId);
        }

        public static CreateAllianceOutMessage CreateAlliance(this ILogic9xServiceInterface agent, string name)
        {
            return new CreateAllianceOutMessage(agent, name);
        }

        public static AllianceOperationOutMessage AllianceOperation(this ILogic9xServiceInterface agent, int type, int value)
        {
            return new AllianceOperationOutMessage(agent, type, value);
        }

        public static AllianceOperationCharacterOutMessage AllianceOperationCharacter(this ILogic9xServiceInterface agent, int type, ulong guid)
        {
            return new AllianceOperationCharacterOutMessage(agent, type, guid);
        }

        public static AllianceOperationCharacterByNameOutMessage AllianceOperationCharacterByName(this ILogic9xServiceInterface agent, int type, string name)
        {
            return new AllianceOperationCharacterByNameOutMessage(agent, type, name);
        }

        public static WorshipCharacterOutMessage WorshipCharacter(this ILogic9xServiceInterface agent, ulong guid)
        {
            return new WorshipCharacterOutMessage(agent, guid);
        }

        public static DonationAllianceItemOutMessage DonationAllianceItem(this ILogic9xServiceInterface agent, int type)
        {
            return new DonationAllianceItemOutMessage(agent, type);
        }

        public static CityMissionOperationOutMessage CityMissionOperation(this ILogic9xServiceInterface agent, int type, int missIndex, int cost)
        {
            return new CityMissionOperationOutMessage(agent, type, missIndex, cost);
        }

        public static DropCityMissionOutMessage DropCityMission(this ILogic9xServiceInterface agent, int missIndex)
        {
            return new DropCityMissionOutMessage(agent, missIndex);
        }

        public static CityRefreshMissionOutMessage CityRefreshMission(this ILogic9xServiceInterface agent, int type)
        {
            return new CityRefreshMissionOutMessage(agent, type);
        }

        public static StoreOperationAddOutMessage StoreOperationAdd(this ILogic9xServiceInterface agent, int type, int bagId, int bagIndex, int count, int needCount, int storeIndex)
        {
            return new StoreOperationAddOutMessage(agent, type, bagId, bagIndex, count, needCount, storeIndex);
        }

        public static StoreOperationBroadcastOutMessage StoreOperationBroadcast(this ILogic9xServiceInterface agent, int type)
        {
            return new StoreOperationBroadcastOutMessage(agent, type);
        }

        public static StoreOperationBuyOutMessage StoreOperationBuy(this ILogic9xServiceInterface agent, ulong guid, long storeId)
        {
            return new StoreOperationBuyOutMessage(agent, guid, storeId);
        }

        public static StoreOperationCancelOutMessage StoreOperationCancel(this ILogic9xServiceInterface agent, long storeId)
        {
            return new StoreOperationCancelOutMessage(agent, storeId);
        }

        public static StoreOperationLookOutMessage StoreOperationLook(this ILogic9xServiceInterface agent, ulong guid)
        {
            return new StoreOperationLookOutMessage(agent, guid);
        }

        public static StoreOperationLookSelfOutMessage StoreOperationLookSelf(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new StoreOperationLookSelfOutMessage(agent, placeholder);
        }

        public static StoreOperationHarvestOutMessage StoreOperationHarvest(this ILogic9xServiceInterface agent, long storeId)
        {
            return new StoreOperationHarvestOutMessage(agent, storeId);
        }

        public static SSStoreOperationExchangeOutMessage SSStoreOperationExchange(this ILogic9xServiceInterface agent, int trade, int itemCount)
        {
            return new SSStoreOperationExchangeOutMessage(agent, trade, itemCount);
        }

        public static ApplyGroupShopItemsOutMessage ApplyGroupShopItems(this ILogic9xServiceInterface agent, Int32Array types)
        {
            return new ApplyGroupShopItemsOutMessage(agent, types);
        }

        public static BuyGroupShopItemOutMessage BuyGroupShopItem(this ILogic9xServiceInterface agent, long guid, int gropId, int count)
        {
            return new BuyGroupShopItemOutMessage(agent, guid, gropId, count);
        }

        public static GetBuyedGroupShopItemsOutMessage GetBuyedGroupShopItems(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new GetBuyedGroupShopItemsOutMessage(agent, placeholder);
        }

        public static GetGroupShopHistoryOutMessage GetGroupShopHistory(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new GetGroupShopHistoryOutMessage(agent, placeholder);
        }

        public static AcceptBattleAwardOutMessage AcceptBattleAward(this ILogic9xServiceInterface agent, int fubenId)
        {
            return new AcceptBattleAwardOutMessage(agent, fubenId);
        }

        public static AstrologyLevelUpOutMessage AstrologyLevelUp(this ILogic9xServiceInterface agent, int bagId, int bagIndex, Int32Array needList)
        {
            return new AstrologyLevelUpOutMessage(agent, bagId, bagIndex, needList);
        }

        public static AstrologyEquipOnOutMessage AstrologyEquipOn(this ILogic9xServiceInterface agent, int bagIndex, int astrologyId, int Index)
        {
            return new AstrologyEquipOnOutMessage(agent, bagIndex, astrologyId, Index);
        }

        public static AstrologyEquipOffOutMessage AstrologyEquipOff(this ILogic9xServiceInterface agent, int astrologyId, int Index)
        {
            return new AstrologyEquipOffOutMessage(agent, astrologyId, Index);
        }

        public static UsePetExpItemOutMessage UsePetExpItem(this ILogic9xServiceInterface agent, int petId, int itemId, int itemCount)
        {
            return new UsePetExpItemOutMessage(agent, petId, itemId, itemCount);
        }

        public static ReincarnationOutMessage Reincarnation(this ILogic9xServiceInterface agent, int typeId)
        {
            return new ReincarnationOutMessage(agent, typeId);
        }

        public static UpgradeHonorOutMessage UpgradeHonor(this ILogic9xServiceInterface agent, int typeId)
        {
            return new UpgradeHonorOutMessage(agent, typeId);
        }

        public static ApplyCityBuildingDataOutMessage ApplyCityBuildingData(this ILogic9xServiceInterface agent, int areaId)
        {
            return new ApplyCityBuildingDataOutMessage(agent, areaId);
        }

        public static ApplyBagByTypeOutMessage ApplyBagByType(this ILogic9xServiceInterface agent, int bagType)
        {
            return new ApplyBagByTypeOutMessage(agent, bagType);
        }

        public static StoreBuyEquipOutMessage StoreBuyEquip(this ILogic9xServiceInterface agent, int storeId, int bagId, int bagIndex, int serviceType)
        {
            return new StoreBuyEquipOutMessage(agent, storeId, bagId, bagIndex, serviceType);
        }

        public static GetQuestionDataOutMessage GetQuestionData(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new GetQuestionDataOutMessage(agent, placeholder);
        }

        public static AnswerQuestionOutMessage AnswerQuestion(this ILogic9xServiceInterface agent, int answer)
        {
            return new AnswerQuestionOutMessage(agent, answer);
        }

        public static RemoveErrorAnswerOutMessage RemoveErrorAnswer(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new RemoveErrorAnswerOutMessage(agent, placeholder);
        }

        public static AnswerQuestionUseItemOutMessage AnswerQuestionUseItem(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new AnswerQuestionUseItemOutMessage(agent, placeholder);
        }

        public static ApplyPlayerHeadInfoOutMessage ApplyPlayerHeadInfo(this ILogic9xServiceInterface agent, ulong characterId)
        {
            return new ApplyPlayerHeadInfoOutMessage(agent, characterId);
        }

        public static GetCompensationListOutMessage GetCompensationList(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new GetCompensationListOutMessage(agent, placeholder);
        }

        public static ReceiveCompensationOutMessage ReceiveCompensation(this ILogic9xServiceInterface agent, int indexType, int type)
        {
            return new ReceiveCompensationOutMessage(agent, indexType, type);
        }

        public static SelectTitleOutMessage SelectTitle(this ILogic9xServiceInterface agent, int id)
        {
            return new SelectTitleOutMessage(agent, id);
        }

        public static RetrainPetOutMessage RetrainPet(this ILogic9xServiceInterface agent, int petId)
        {
            return new RetrainPetOutMessage(agent, petId);
        }

        public static UpgradeAllianceBuffOutMessage UpgradeAllianceBuff(this ILogic9xServiceInterface agent, int buffId)
        {
            return new UpgradeAllianceBuffOutMessage(agent, buffId);
        }

        public static InvestmentOutMessage Investment(this ILogic9xServiceInterface agent, int id)
        {
            return new InvestmentOutMessage(agent, id);
        }

        public static GainRewardOutMessage GainReward(this ILogic9xServiceInterface agent, int type, int id)
        {
            return new GainRewardOutMessage(agent, type, id);
        }

        public static WorshipOutMessage Worship(this ILogic9xServiceInterface agent, int type)
        {
            return new WorshipOutMessage(agent, type);
        }

        public static UseGiftCodeOutMessage UseGiftCode(this ILogic9xServiceInterface agent, string code)
        {
            return new UseGiftCodeOutMessage(agent, code);
        }

        public static ApplyRechargeTablesOutMessage ApplyRechargeTables(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new ApplyRechargeTablesOutMessage(agent, placeholder);
        }

        public static ApplyFirstChargeItemOutMessage ApplyFirstChargeItem(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new ApplyFirstChargeItemOutMessage(agent, placeholder);
        }

        public static ApplyGetFirstChargeItemOutMessage ApplyGetFirstChargeItem(this ILogic9xServiceInterface agent, int index)
        {
            return new ApplyGetFirstChargeItemOutMessage(agent, index);
        }

        public static TakeMultyExpAwardOutMessage TakeMultyExpAward(this ILogic9xServiceInterface agent, int id)
        {
            return new TakeMultyExpAwardOutMessage(agent, id);
        }

        public static RandEquipSkillOutMessage RandEquipSkill(this ILogic9xServiceInterface agent, int bagType, int bagIndex, int itemId)
        {
            return new RandEquipSkillOutMessage(agent, bagType, bagIndex, itemId);
        }

        public static UseEquipSkillOutMessage UseEquipSkill(this ILogic9xServiceInterface agent, int bagType, int bagIndex, int type)
        {
            return new UseEquipSkillOutMessage(agent, bagType, bagIndex, type);
        }

        public static GetReviewStateOutMessage GetReviewState(this ILogic9xServiceInterface agent, int type)
        {
            return new GetReviewStateOutMessage(agent, type);
        }

        public static OnItemAuctionOutMessage OnItemAuction(this ILogic9xServiceInterface agent, int type, int bagId, int bagIndex, int count, int needCount, int storeIndex)
        {
            return new OnItemAuctionOutMessage(agent, type, bagId, bagIndex, count, needCount, storeIndex);
        }

        public static BuyItemAuctionOutMessage BuyItemAuction(this ILogic9xServiceInterface agent, ulong characterId, long guid, long managerId)
        {
            return new BuyItemAuctionOutMessage(agent, characterId, guid, managerId);
        }

        public static ApplySellHistoryOutMessage ApplySellHistory(this ILogic9xServiceInterface agent, int type)
        {
            return new ApplySellHistoryOutMessage(agent, type);
        }

        public static DrawWishItemOutMessage DrawWishItem(this ILogic9xServiceInterface agent, Int32Array param)
        {
            return new DrawWishItemOutMessage(agent, param);
        }

        public static ApplyOperationActivityOutMessage ApplyOperationActivity(this ILogic9xServiceInterface agent, int serverId)
        {
            return new ApplyOperationActivityOutMessage(agent, serverId);
        }

        public static ClaimOperationRewardOutMessage ClaimOperationReward(this ILogic9xServiceInterface agent, int type, int Id)
        {
            return new ClaimOperationRewardOutMessage(agent, type, Id);
        }

        public static ApplyPromoteHPOutMessage ApplyPromoteHP(this ILogic9xServiceInterface agent, int serverId, int activityId, int batteryId, int promoteType)
        {
            return new ApplyPromoteHPOutMessage(agent, serverId, activityId, batteryId, promoteType);
        }

        public static ApplyPromoteSkillOutMessage ApplyPromoteSkill(this ILogic9xServiceInterface agent, int serverId, int activityId, int batteryId, ulong batteryGuid, int promoteType)
        {
            return new ApplyPromoteSkillOutMessage(agent, serverId, activityId, batteryId, batteryGuid, promoteType);
        }

        public static ApplyPickUpBoxOutMessage ApplyPickUpBox(this ILogic9xServiceInterface agent, int serverId, int activityId, int npcId)
        {
            return new ApplyPickUpBoxOutMessage(agent, serverId, activityId, npcId);
        }

        public static ApplyJoinActivityOutMessage ApplyJoinActivity(this ILogic9xServiceInterface agent, int serverId, int activityId)
        {
            return new ApplyJoinActivityOutMessage(agent, serverId, activityId);
        }

        public static ApplyPortraitAwardOutMessage ApplyPortraitAward(this ILogic9xServiceInterface agent, int serverId)
        {
            return new ApplyPortraitAwardOutMessage(agent, serverId);
        }

        public static ApplyGetTowerRewardOutMessage ApplyGetTowerReward(this ILogic9xServiceInterface agent, int serverId, int activityId, int idx)
        {
            return new ApplyGetTowerRewardOutMessage(agent, serverId, activityId, idx);
        }

        public static SendJsonDataOutMessage SendJsonData(this ILogic9xServiceInterface agent, string json)
        {
            return new SendJsonDataOutMessage(agent, json);
        }

        public static BuyWingChargeOutMessage BuyWingCharge(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new BuyWingChargeOutMessage(agent, placeholder);
        }

        public static PetIsLandBuyTiliOutMessage PetIsLandBuyTili(this ILogic9xServiceInterface agent, int placeholder)
        {
            return new PetIsLandBuyTiliOutMessage(agent, placeholder);
        }

        public static ClientErrorMessageOutMessage ClientErrorMessage(this ILogic9xServiceInterface agent, int errorType, string errorMsg)
        {
            return new ClientErrorMessageOutMessage(agent, errorType, errorMsg);
        }

        public static TowerSweepOutMessage TowerSweep(this ILogic9xServiceInterface agent, int param)
        {
            return new TowerSweepOutMessage(agent, param);
        }

        public static void Init(this ILogic9xServiceInterface agent)
        {
            agent.AddPublishDataFunc(ServiceType.Logic, (p, list) =>
            {
                switch (p)
                {
                    case 1060:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncMission_ARG_int32_missionId_int32_state_int32_param__>(ms);
                        }
                        break;
                    case 1061:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncFlag_ARG_int32_flagId_int32_param__>(ms);
                        }
                        break;
                    case 1062:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncFlagList_ARG_Int32Array_trueList_Int32Array_falseList__>(ms);
                        }
                        break;
                    case 1063:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncExdata_ARG_int32_exdataId_int32_value__>(ms);
                        }
                        break;
                    case 1064:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncExdataList_ARG_Dict_int_int_Data_diff__>(ms);
                        }
                        break;
                    case 1065:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncExdata64_ARG_int32_exdataId_int64_value__>(ms);
                        }
                        break;
                    case 1066:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncResources_ARG_int32_resId_int32_value__>(ms);
                        }
                        break;
                    case 1067:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncItems_ARG_BagsChangeData_bag__>(ms);
                        }
                        break;
                    case 1082:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncMissions_ARG_MissionDataMessage_missions__>(ms);
                        }
                        break;
                    case 1087:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_FinishAchievement_ARG_int32_achievementId__>(ms);
                        }
                        break;
                    case 1393:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SeekCharactersReceive_ARG_CharacterSimpleDataList_result__>(ms);
                        }
                        break;
                    case 1394:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SeekFriendsReceive_ARG_CharacterSimpleDataList_result__>(ms);
                        }
                        break;
                    case 1101:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_DungeonComplete_ARG_FubenResult_result__>(ms);
                        }
                        break;
                    case 1116:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncMails_ARG_MailList_mails__>(ms);
                        }
                        break;
                    case 1126:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncCityBuildingData_ARG_BuildingList_data__>(ms);
                        }
                        break;
                    case 1127:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncPetMission_ARG_PetMissionList_msg__>(ms);
                        }
                        break;
                    case 1129:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_EquipDurableBroken_ARG_int32_partId_int32_value__>(ms);
                        }
                        break;
                    case 1130:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_EquipDurableChange_ARG_int32_placeholder__>(ms);
                        }
                        break;
                    case 1144:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_DeletePetMission_ARG_int32_missionId__>(ms);
                        }
                        break;
                    case 1152:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_LogicP1vP1FightResult_ARG_P1vP1RewardData_data__>(ms);
                        }
                        break;
                    case 1163:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_LogicSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__>(ms);
                        }
                        break;
                    case 1170:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_ElfDrawOver_ARG_DrawItemResult_Items_int64_getTime__>(ms);
                        }
                        break;
                    case 1171:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_TalentCountChange_ARG_int32_talentId_int32_value__>(ms);
                        }
                        break;
                    case 1186:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_NotifyStoreBuyed_ARG_int64_storeId_uint64_Aid_string_Aname__>(ms);
                        }
                        break;
                    case 1197:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_AstrologyDrawOver_ARG_DrawItemResult_Items_int64_getTime__>(ms);
                        }
                        break;
                    case 1198:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncAddFriend_ARG_int32_type_CharacterSimpleData_character__>(ms);
                        }
                        break;
                    case 1203:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_LogicNotifyMessage_ARG_int32_type_string_info_int32_addChat__>(ms);
                        }
                        break;
                    case 1204:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_NotifGainRes_ARG_DataChangeList_changes__>(ms);
                        }
                        break;
                    case 1205:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_BattleResult_ARG_int32_dungeonId_int32_resultType_int32_first__>(ms);
                        }
                        break;
                    case 1208:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_NotifyP1vP1Change_ARG_P1vP1Change_One_one__>(ms);
                        }
                        break;
                    case 1210:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncFriendDataChange_ARG_CharacterSimpleDataList_Changes__>(ms);
                        }
                        break;
                    case 1211:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncFriendDelete_ARG_int32_type_uint64_characterId__>(ms);
                        }
                        break;
                    case 1231:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_NotifyRechargeSuccess_ARG_int32_rechargeId__>(ms);
                        }
                        break;
                    case 1514:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncOperationActivityItem_ARG_MsgOperActivtyItemList_items__>(ms);
                        }
                        break;
                    case 1532:
                        using (var ms = new MemoryStream(list, false))
                        {
                            return Serializer.Deserialize<__RPC_Logic_SyncOperationActivityTerm_ARG_int32_id_int32_param__>(ms);
                        }
                        break;
                    default:
                        break;
                }

                return null;
            });


        agent.AddPublishMessageFunc(ServiceType.Logic, (evt) =>
            {
                switch (evt.Message.FuncId)
                {
                    case 1060:
                        {
                            var data = evt.Data as __RPC_Logic_SyncMission_ARG_int32_missionId_int32_state_int32_param__;
                            agent.SyncMission(data.MissionId, data.State, data.Param);
                        }
                        break;
                    case 1061:
                        {
                            var data = evt.Data as __RPC_Logic_SyncFlag_ARG_int32_flagId_int32_param__;
                            agent.SyncFlag(data.FlagId, data.Param);
                        }
                        break;
                    case 1062:
                        {
                            var data = evt.Data as __RPC_Logic_SyncFlagList_ARG_Int32Array_trueList_Int32Array_falseList__;
                            agent.SyncFlagList(data.TrueList, data.FalseList);
                        }
                        break;
                    case 1063:
                        {
                            var data = evt.Data as __RPC_Logic_SyncExdata_ARG_int32_exdataId_int32_value__;
                            agent.SyncExdata(data.ExdataId, data.Value);
                        }
                        break;
                    case 1064:
                        {
                            var data = evt.Data as __RPC_Logic_SyncExdataList_ARG_Dict_int_int_Data_diff__;
                            agent.SyncExdataList(data.Diff);
                        }
                        break;
                    case 1065:
                        {
                            var data = evt.Data as __RPC_Logic_SyncExdata64_ARG_int32_exdataId_int64_value__;
                            agent.SyncExdata64(data.ExdataId, data.Value);
                        }
                        break;
                    case 1066:
                        {
                            var data = evt.Data as __RPC_Logic_SyncResources_ARG_int32_resId_int32_value__;
                            agent.SyncResources(data.ResId, data.Value);
                        }
                        break;
                    case 1067:
                        {
                            var data = evt.Data as __RPC_Logic_SyncItems_ARG_BagsChangeData_bag__;
                            agent.SyncItems(data.Bag);
                        }
                        break;
                    case 1082:
                        {
                            var data = evt.Data as __RPC_Logic_SyncMissions_ARG_MissionDataMessage_missions__;
                            agent.SyncMissions(data.Missions);
                        }
                        break;
                    case 1087:
                        {
                            var data = evt.Data as __RPC_Logic_FinishAchievement_ARG_int32_achievementId__;
                            agent.FinishAchievement(data.AchievementId);
                        }
                        break;
                    case 1393:
                        {
                            var data = evt.Data as __RPC_Logic_SeekCharactersReceive_ARG_CharacterSimpleDataList_result__;
                            agent.SeekCharactersReceive(data.Result);
                        }
                        break;
                    case 1394:
                        {
                            var data = evt.Data as __RPC_Logic_SeekFriendsReceive_ARG_CharacterSimpleDataList_result__;
                            agent.SeekFriendsReceive(data.Result);
                        }
                        break;
                    case 1101:
                        {
                            var data = evt.Data as __RPC_Logic_DungeonComplete_ARG_FubenResult_result__;
                            agent.DungeonComplete(data.Result);
                        }
                        break;
                    case 1116:
                        {
                            var data = evt.Data as __RPC_Logic_SyncMails_ARG_MailList_mails__;
                            agent.SyncMails(data.Mails);
                        }
                        break;
                    case 1126:
                        {
                            var data = evt.Data as __RPC_Logic_SyncCityBuildingData_ARG_BuildingList_data__;
                            agent.SyncCityBuildingData(data.Data);
                        }
                        break;
                    case 1127:
                        {
                            var data = evt.Data as __RPC_Logic_SyncPetMission_ARG_PetMissionList_msg__;
                            agent.SyncPetMission(data.Msg);
                        }
                        break;
                    case 1129:
                        {
                            var data = evt.Data as __RPC_Logic_EquipDurableBroken_ARG_int32_partId_int32_value__;
                            agent.EquipDurableBroken(data.PartId, data.Value);
                        }
                        break;
                    case 1130:
                        {
                            var data = evt.Data as __RPC_Logic_EquipDurableChange_ARG_int32_placeholder__;
                            agent.EquipDurableChange(data.Placeholder);
                        }
                        break;
                    case 1144:
                        {
                            var data = evt.Data as __RPC_Logic_DeletePetMission_ARG_int32_missionId__;
                            agent.DeletePetMission(data.MissionId);
                        }
                        break;
                    case 1152:
                        {
                            var data = evt.Data as __RPC_Logic_LogicP1vP1FightResult_ARG_P1vP1RewardData_data__;
                            agent.LogicP1vP1FightResult(data.Data);
                        }
                        break;
                    case 1163:
                        {
                            var data = evt.Data as __RPC_Logic_LogicSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__;
                            agent.LogicSyncAllianceMessage(data.Type, data.Name1, data.AllianceId, data.Name2);
                        }
                        break;
                    case 1170:
                        {
                            var data = evt.Data as __RPC_Logic_ElfDrawOver_ARG_DrawItemResult_Items_int64_getTime__;
                            agent.ElfDrawOver(data.Items, data.GetTime);
                        }
                        break;
                    case 1171:
                        {
                            var data = evt.Data as __RPC_Logic_TalentCountChange_ARG_int32_talentId_int32_value__;
                            agent.TalentCountChange(data.TalentId, data.Value);
                        }
                        break;
                    case 1186:
                        {
                            var data = evt.Data as __RPC_Logic_NotifyStoreBuyed_ARG_int64_storeId_uint64_Aid_string_Aname__;
                            agent.NotifyStoreBuyed(data.StoreId, data.Aid, data.Aname);
                        }
                        break;
                    case 1197:
                        {
                            var data = evt.Data as __RPC_Logic_AstrologyDrawOver_ARG_DrawItemResult_Items_int64_getTime__;
                            agent.AstrologyDrawOver(data.Items, data.GetTime);
                        }
                        break;
                    case 1198:
                        {
                            var data = evt.Data as __RPC_Logic_SyncAddFriend_ARG_int32_type_CharacterSimpleData_character__;
                            agent.SyncAddFriend(data.Type, data.Character);
                        }
                        break;
                    case 1203:
                        {
                            var data = evt.Data as __RPC_Logic_LogicNotifyMessage_ARG_int32_type_string_info_int32_addChat__;
                            agent.LogicNotifyMessage(data.Type, data.Info, data.AddChat);
                        }
                        break;
                    case 1204:
                        {
                            var data = evt.Data as __RPC_Logic_NotifGainRes_ARG_DataChangeList_changes__;
                            agent.NotifGainRes(data.Changes);
                        }
                        break;
                    case 1205:
                        {
                            var data = evt.Data as __RPC_Logic_BattleResult_ARG_int32_dungeonId_int32_resultType_int32_first__;
                            agent.BattleResult(data.DungeonId, data.ResultType, data.First);
                        }
                        break;
                    case 1208:
                        {
                            var data = evt.Data as __RPC_Logic_NotifyP1vP1Change_ARG_P1vP1Change_One_one__;
                            agent.NotifyP1vP1Change(data.One);
                        }
                        break;
                    case 1210:
                        {
                            var data = evt.Data as __RPC_Logic_SyncFriendDataChange_ARG_CharacterSimpleDataList_Changes__;
                            agent.SyncFriendDataChange(data.Changes);
                        }
                        break;
                    case 1211:
                        {
                            var data = evt.Data as __RPC_Logic_SyncFriendDelete_ARG_int32_type_uint64_characterId__;
                            agent.SyncFriendDelete(data.Type, data.CharacterId);
                        }
                        break;
                    case 1231:
                        {
                            var data = evt.Data as __RPC_Logic_NotifyRechargeSuccess_ARG_int32_rechargeId__;
                            agent.NotifyRechargeSuccess(data.RechargeId);
                        }
                        break;
                    case 1514:
                        {
                            var data = evt.Data as __RPC_Logic_SyncOperationActivityItem_ARG_MsgOperActivtyItemList_items__;
                            agent.SyncOperationActivityItem(data.Items);
                        }
                        break;
                    case 1532:
                        {
                            var data = evt.Data as __RPC_Logic_SyncOperationActivityTerm_ARG_int32_id_int32_param__;
                            agent.SyncOperationActivityTerm(data.Id, data.Param);
                        }
                        break;
                    default:
                        break;
                }
            });
        }
    }

    public class GMLogicOutMessage : OutMessage
    {
        public GMLogicOutMessage(IAgentBase sender, string commond)
            : base(sender, ServiceType.Logic, 1041)
        {
            Request = new __RPC_Logic_GMLogic_ARG_string_commond__();
            Request.Commond=commond;

        }

        public __RPC_Logic_GMLogic_ARG_string_commond__ Request { get; private set; }

            private __RPC_Logic_GMLogic_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_GMLogic_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplySkillOutMessage : OutMessage
    {
        public ApplySkillOutMessage(IAgentBase sender, uint placeholder)
            : base(sender, ServiceType.Logic, 1046)
        {
            Request = new __RPC_Logic_ApplySkill_ARG_uint32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_ApplySkill_ARG_uint32_placeholder__ Request { get; private set; }

            private __RPC_Logic_ApplySkill_RET_SkillDataMsg__ mResponse;
            public SkillDataMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplySkill_RET_SkillDataMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UpgradeInnateOutMessage : OutMessage
    {
        public UpgradeInnateOutMessage(IAgentBase sender, int innateId)
            : base(sender, ServiceType.Logic, 1047)
        {
            Request = new __RPC_Logic_UpgradeInnate_ARG_int32_innateId__();
            Request.InnateId=innateId;

        }

        public __RPC_Logic_UpgradeInnate_ARG_int32_innateId__ Request { get; private set; }

            private __RPC_Logic_UpgradeInnate_RET_SkillDataMsg__ mResponse;
            public SkillDataMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_UpgradeInnate_RET_SkillDataMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ClearInnateOutMessage : OutMessage
    {
        public ClearInnateOutMessage(IAgentBase sender, int innateId)
            : base(sender, ServiceType.Logic, 1048)
        {
            Request = new __RPC_Logic_ClearInnate_ARG_int32_innateId__();
            Request.InnateId=innateId;

        }

        public __RPC_Logic_ClearInnate_ARG_int32_innateId__ Request { get; private set; }

            private __RPC_Logic_ClearInnate_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ClearInnate_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyBagsOutMessage : OutMessage
    {
        public ApplyBagsOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1049)
        {
            Request = new __RPC_Logic_ApplyBags_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_ApplyBags_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_ApplyBags_RET_BagData__ mResponse;
            public BagData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyBags_RET_BagData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyFlagOutMessage : OutMessage
    {
        public ApplyFlagOutMessage(IAgentBase sender, int flagId)
            : base(sender, ServiceType.Logic, 1050)
        {
            Request = new __RPC_Logic_ApplyFlag_ARG_int32_flagId__();
            Request.FlagId=flagId;

        }

        public __RPC_Logic_ApplyFlag_ARG_int32_flagId__ Request { get; private set; }

            private __RPC_Logic_ApplyFlag_RET_Int32Array__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyFlag_RET_Int32Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyExdataOutMessage : OutMessage
    {
        public ApplyExdataOutMessage(IAgentBase sender, int exdataId)
            : base(sender, ServiceType.Logic, 1051)
        {
            Request = new __RPC_Logic_ApplyExdata_ARG_int32_exdataId__();
            Request.ExdataId=exdataId;

        }

        public __RPC_Logic_ApplyExdata_ARG_int32_exdataId__ Request { get; private set; }

            private __RPC_Logic_ApplyExdata_RET_Int32Array__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyExdata_RET_Int32Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyExdata64OutMessage : OutMessage
    {
        public ApplyExdata64OutMessage(IAgentBase sender, int exdataId)
            : base(sender, ServiceType.Logic, 1052)
        {
            Request = new __RPC_Logic_ApplyExdata64_ARG_int32_exdataId__();
            Request.ExdataId=exdataId;

        }

        public __RPC_Logic_ApplyExdata64_ARG_int32_exdataId__ Request { get; private set; }

            private __RPC_Logic_ApplyExdata64_RET_Int64Array__ mResponse;
            public Int64Array Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyExdata64_RET_Int64Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyMissionOutMessage : OutMessage
    {
        public ApplyMissionOutMessage(IAgentBase sender, int missionId)
            : base(sender, ServiceType.Logic, 1053)
        {
            Request = new __RPC_Logic_ApplyMission_ARG_int32_missionId__();
            Request.MissionId=missionId;

        }

        public __RPC_Logic_ApplyMission_ARG_int32_missionId__ Request { get; private set; }

            private __RPC_Logic_ApplyMission_RET_MissionDataMessage__ mResponse;
            public MissionDataMessage Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyMission_RET_MissionDataMessage__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyBooksOutMessage : OutMessage
    {
        public ApplyBooksOutMessage(IAgentBase sender, uint placeholder)
            : base(sender, ServiceType.Logic, 1054)
        {
            Request = new __RPC_Logic_ApplyBooks_ARG_uint32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_ApplyBooks_ARG_uint32_placeholder__ Request { get; private set; }

            private __RPC_Logic_ApplyBooks_RET_BookDatas__ mResponse;
            public BookDatas Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyBooks_RET_BookDatas__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ReplaceEquipOutMessage : OutMessage
    {
        public ReplaceEquipOutMessage(IAgentBase sender, int bagItemId, int part, int index)
            : base(sender, ServiceType.Logic, 1055)
        {
            Request = new __RPC_Logic_ReplaceEquip_ARG_int32_bagItemId_int32_part_int32_index__();
            Request.BagItemId=bagItemId;
            Request.Part=part;
            Request.Index=index;

        }

        public __RPC_Logic_ReplaceEquip_ARG_int32_bagItemId_int32_part_int32_index__ Request { get; private set; }

            private __RPC_Logic_ReplaceEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ReplaceEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AcceptMissionOutMessage : OutMessage
    {
        public AcceptMissionOutMessage(IAgentBase sender, int missionId)
            : base(sender, ServiceType.Logic, 1056)
        {
            Request = new __RPC_Logic_AcceptMission_ARG_int32_missionId__();
            Request.MissionId=missionId;

        }

        public __RPC_Logic_AcceptMission_ARG_int32_missionId__ Request { get; private set; }

            private __RPC_Logic_AcceptMission_RET_MissionBaseData__ mResponse;
            public MissionBaseData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_AcceptMission_RET_MissionBaseData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class CommitMissionOutMessage : OutMessage
    {
        public CommitMissionOutMessage(IAgentBase sender, int missionId)
            : base(sender, ServiceType.Logic, 1057)
        {
            Request = new __RPC_Logic_CommitMission_ARG_int32_missionId__();
            Request.MissionId=missionId;

        }

        public __RPC_Logic_CommitMission_ARG_int32_missionId__ Request { get; private set; }

            private __RPC_Logic_CommitMission_RET_MissionDataMessage__ mResponse;
            public MissionDataMessage Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_CommitMission_RET_MissionDataMessage__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class CompleteMissionOutMessage : OutMessage
    {
        public CompleteMissionOutMessage(IAgentBase sender, int missionId)
            : base(sender, ServiceType.Logic, 1058)
        {
            Request = new __RPC_Logic_CompleteMission_ARG_int32_missionId__();
            Request.MissionId=missionId;

        }

        public __RPC_Logic_CompleteMission_ARG_int32_missionId__ Request { get; private set; }

            private __RPC_Logic_CompleteMission_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_CompleteMission_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DropMissionOutMessage : OutMessage
    {
        public DropMissionOutMessage(IAgentBase sender, int missionId)
            : base(sender, ServiceType.Logic, 1059)
        {
            Request = new __RPC_Logic_DropMission_ARG_int32_missionId__();
            Request.MissionId=missionId;

        }

        public __RPC_Logic_DropMission_ARG_int32_missionId__ Request { get; private set; }

            private __RPC_Logic_DropMission_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DropMission_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncMissionOutMessage : OutMessage
    {
        public SyncMissionOutMessage(IAgentBase sender, int missionId, int state, int param)
            : base(sender, ServiceType.Logic, 1060)
        {
            Request = new __RPC_Logic_SyncMission_ARG_int32_missionId_int32_state_int32_param__();
            Request.MissionId=missionId;
            Request.State=state;
            Request.Param=param;

        }

        public __RPC_Logic_SyncMission_ARG_int32_missionId_int32_state_int32_param__ Request { get; private set; }


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

    public class SyncFlagOutMessage : OutMessage
    {
        public SyncFlagOutMessage(IAgentBase sender, int flagId, int param)
            : base(sender, ServiceType.Logic, 1061)
        {
            Request = new __RPC_Logic_SyncFlag_ARG_int32_flagId_int32_param__();
            Request.FlagId=flagId;
            Request.Param=param;

        }

        public __RPC_Logic_SyncFlag_ARG_int32_flagId_int32_param__ Request { get; private set; }


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

    public class SyncFlagListOutMessage : OutMessage
    {
        public SyncFlagListOutMessage(IAgentBase sender, Int32Array trueList, Int32Array falseList)
            : base(sender, ServiceType.Logic, 1062)
        {
            Request = new __RPC_Logic_SyncFlagList_ARG_Int32Array_trueList_Int32Array_falseList__();
            Request.TrueList=trueList;
            Request.FalseList=falseList;

        }

        public __RPC_Logic_SyncFlagList_ARG_Int32Array_trueList_Int32Array_falseList__ Request { get; private set; }


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

    public class SyncExdataOutMessage : OutMessage
    {
        public SyncExdataOutMessage(IAgentBase sender, int exdataId, int value)
            : base(sender, ServiceType.Logic, 1063)
        {
            Request = new __RPC_Logic_SyncExdata_ARG_int32_exdataId_int32_value__();
            Request.ExdataId=exdataId;
            Request.Value=value;

        }

        public __RPC_Logic_SyncExdata_ARG_int32_exdataId_int32_value__ Request { get; private set; }


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

    public class SyncExdataListOutMessage : OutMessage
    {
        public SyncExdataListOutMessage(IAgentBase sender, Dict_int_int_Data diff)
            : base(sender, ServiceType.Logic, 1064)
        {
            Request = new __RPC_Logic_SyncExdataList_ARG_Dict_int_int_Data_diff__();
            Request.Diff=diff;

        }

        public __RPC_Logic_SyncExdataList_ARG_Dict_int_int_Data_diff__ Request { get; private set; }


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

    public class SyncExdata64OutMessage : OutMessage
    {
        public SyncExdata64OutMessage(IAgentBase sender, int exdataId, long value)
            : base(sender, ServiceType.Logic, 1065)
        {
            Request = new __RPC_Logic_SyncExdata64_ARG_int32_exdataId_int64_value__();
            Request.ExdataId=exdataId;
            Request.Value=value;

        }

        public __RPC_Logic_SyncExdata64_ARG_int32_exdataId_int64_value__ Request { get; private set; }


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

    public class SyncResourcesOutMessage : OutMessage
    {
        public SyncResourcesOutMessage(IAgentBase sender, int resId, int value)
            : base(sender, ServiceType.Logic, 1066)
        {
            Request = new __RPC_Logic_SyncResources_ARG_int32_resId_int32_value__();
            Request.ResId=resId;
            Request.Value=value;

        }

        public __RPC_Logic_SyncResources_ARG_int32_resId_int32_value__ Request { get; private set; }


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

    public class SyncItemsOutMessage : OutMessage
    {
        public SyncItemsOutMessage(IAgentBase sender, BagsChangeData bag)
            : base(sender, ServiceType.Logic, 1067)
        {
            Request = new __RPC_Logic_SyncItems_ARG_BagsChangeData_bag__();
            Request.Bag=bag;

        }

        public __RPC_Logic_SyncItems_ARG_BagsChangeData_bag__ Request { get; private set; }


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

    public class EquipSkillOutMessage : OutMessage
    {
        public EquipSkillOutMessage(IAgentBase sender, Int32Array EquipSkills)
            : base(sender, ServiceType.Logic, 1068)
        {
            Request = new __RPC_Logic_EquipSkill_ARG_Int32Array_EquipSkills__();
            Request.EquipSkills=EquipSkills;

        }

        public __RPC_Logic_EquipSkill_ARG_Int32Array_EquipSkills__ Request { get; private set; }

            private __RPC_Logic_EquipSkill_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_EquipSkill_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UpgradeSkillOutMessage : OutMessage
    {
        public UpgradeSkillOutMessage(IAgentBase sender, int skillId)
            : base(sender, ServiceType.Logic, 1069)
        {
            Request = new __RPC_Logic_UpgradeSkill_ARG_int32_skillId__();
            Request.SkillId=skillId;

        }

        public __RPC_Logic_UpgradeSkill_ARG_int32_skillId__ Request { get; private set; }

            private __RPC_Logic_UpgradeSkill_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UpgradeSkill_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SellBagItemOutMessage : OutMessage
    {
        public SellBagItemOutMessage(IAgentBase sender, int bagType, int itemId, int index, int count)
            : base(sender, ServiceType.Logic, 1070)
        {
            Request = new __RPC_Logic_SellBagItem_ARG_int32_bagType_int32_itemId_int32_index_int32_count__();
            Request.BagType=bagType;
            Request.ItemId=itemId;
            Request.Index=index;
            Request.Count=count;

        }

        public __RPC_Logic_SellBagItem_ARG_int32_bagType_int32_itemId_int32_index_int32_count__ Request { get; private set; }

            private __RPC_Logic_SellBagItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SellBagItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class RecycleBagItemOutMessage : OutMessage
    {
        public RecycleBagItemOutMessage(IAgentBase sender, int bagType, int itemId, int index, int count)
            : base(sender, ServiceType.Logic, 1071)
        {
            Request = new __RPC_Logic_RecycleBagItem_ARG_int32_bagType_int32_itemId_int32_index_int32_count__();
            Request.BagType=bagType;
            Request.ItemId=itemId;
            Request.Index=index;
            Request.Count=count;

        }

        public __RPC_Logic_RecycleBagItem_ARG_int32_bagType_int32_itemId_int32_index_int32_count__ Request { get; private set; }

            private __RPC_Logic_RecycleBagItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RecycleBagItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EnchanceEquipOutMessage : OutMessage
    {
        public EnchanceEquipOutMessage(IAgentBase sender, int bagType, int bagIndex, int blessing, int upRate)
            : base(sender, ServiceType.Logic, 1072)
        {
            Request = new __RPC_Logic_EnchanceEquip_ARG_int32_bagType_int32_bagIndex_int32_blessing_int32_upRate__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;
            Request.Blessing=blessing;
            Request.UpRate=upRate;

        }

        public __RPC_Logic_EnchanceEquip_ARG_int32_bagType_int32_bagIndex_int32_blessing_int32_upRate__ Request { get; private set; }

            private __RPC_Logic_EnchanceEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_EnchanceEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AppendEquipOutMessage : OutMessage
    {
        public AppendEquipOutMessage(IAgentBase sender, int bagType, int bagIndex)
            : base(sender, ServiceType.Logic, 1073)
        {
            Request = new __RPC_Logic_AppendEquip_ARG_int32_bagType_int32_bagIndex__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;

        }

        public __RPC_Logic_AppendEquip_ARG_int32_bagType_int32_bagIndex__ Request { get; private set; }

            private __RPC_Logic_AppendEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AppendEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ResetExcellentEquipOutMessage : OutMessage
    {
        public ResetExcellentEquipOutMessage(IAgentBase sender, int bagType, int bagIndex)
            : base(sender, ServiceType.Logic, 1074)
        {
            Request = new __RPC_Logic_ResetExcellentEquip_ARG_int32_bagType_int32_bagIndex__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;

        }

        public __RPC_Logic_ResetExcellentEquip_ARG_int32_bagType_int32_bagIndex__ Request { get; private set; }

            private __RPC_Logic_ResetExcellentEquip_RET_Int32Array__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ResetExcellentEquip_RET_Int32Array__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ConfirmResetExcellentEquipOutMessage : OutMessage
    {
        public ConfirmResetExcellentEquipOutMessage(IAgentBase sender, int bagType, int bagIndex, int ok)
            : base(sender, ServiceType.Logic, 1075)
        {
            Request = new __RPC_Logic_ConfirmResetExcellentEquip_ARG_int32_bagType_int32_bagIndex_int32_ok__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;
            Request.Ok=ok;

        }

        public __RPC_Logic_ConfirmResetExcellentEquip_ARG_int32_bagType_int32_bagIndex_int32_ok__ Request { get; private set; }

            private __RPC_Logic_ConfirmResetExcellentEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ConfirmResetExcellentEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SuperExcellentEquipOutMessage : OutMessage
    {
        public SuperExcellentEquipOutMessage(IAgentBase sender, int bagType, int bagIndex, Int32Array LockList)
            : base(sender, ServiceType.Logic, 1076)
        {
            Request = new __RPC_Logic_SuperExcellentEquip_ARG_int32_bagType_int32_bagIndex_Int32Array_LockList__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;
            Request.LockList=LockList;

        }

        public __RPC_Logic_SuperExcellentEquip_ARG_int32_bagType_int32_bagIndex_Int32Array_LockList__ Request { get; private set; }

            private __RPC_Logic_SuperExcellentEquip_RET_TwoList__ mResponse;
            public TwoList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_SuperExcellentEquip_RET_TwoList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SmritiEquipOutMessage : OutMessage
    {
        public SmritiEquipOutMessage(IAgentBase sender, int smritiType, int moneyType, int fromBagType, int fromBagIndex, int toBagType, int toBagIndex)
            : base(sender, ServiceType.Logic, 1077)
        {
            Request = new __RPC_Logic_SmritiEquip_ARG_int32_smritiType_int32_moneyType_int32_fromBagType_int32_fromBagIndex_int32_toBagType_int32_toBagIndex__();
            Request.SmritiType=smritiType;
            Request.MoneyType=moneyType;
            Request.FromBagType=fromBagType;
            Request.FromBagIndex=fromBagIndex;
            Request.ToBagType=toBagType;
            Request.ToBagIndex=toBagIndex;

        }

        public __RPC_Logic_SmritiEquip_ARG_int32_smritiType_int32_moneyType_int32_fromBagType_int32_fromBagIndex_int32_toBagType_int32_toBagIndex__ Request { get; private set; }

            private __RPC_Logic_SmritiEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SmritiEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UseItemOutMessage : OutMessage
    {
        public UseItemOutMessage(IAgentBase sender, int bagType, int bagIndex, int count)
            : base(sender, ServiceType.Logic, 1078)
        {
            Request = new __RPC_Logic_UseItem_ARG_int32_bagType_int32_bagIndex_int32_count__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;
            Request.Count=count;

        }

        public __RPC_Logic_UseItem_ARG_int32_bagType_int32_bagIndex_int32_count__ Request { get; private set; }

            private __RPC_Logic_UseItem_RET_Dict_int_int_Data__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UseItem_RET_Dict_int_int_Data__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncMissionsOutMessage : OutMessage
    {
        public SyncMissionsOutMessage(IAgentBase sender, MissionDataMessage missions)
            : base(sender, ServiceType.Logic, 1082)
        {
            Request = new __RPC_Logic_SyncMissions_ARG_MissionDataMessage_missions__();
            Request.Missions=missions;

        }

        public __RPC_Logic_SyncMissions_ARG_MissionDataMessage_missions__ Request { get; private set; }


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

    public class ActivationRewardOutMessage : OutMessage
    {
        public ActivationRewardOutMessage(IAgentBase sender, int typeId, int giftId)
            : base(sender, ServiceType.Logic, 1084)
        {
            Request = new __RPC_Logic_ActivationReward_ARG_int32_typeId_int32_giftId__();
            Request.TypeId=typeId;
            Request.GiftId=giftId;

        }

        public __RPC_Logic_ActivationReward_ARG_int32_typeId_int32_giftId__ Request { get; private set; }

            private __RPC_Logic_ActivationReward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ActivationReward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ComposeItemOutMessage : OutMessage
    {
        public ComposeItemOutMessage(IAgentBase sender, int composeId, int count)
            : base(sender, ServiceType.Logic, 1086)
        {
            Request = new __RPC_Logic_ComposeItem_ARG_int32_composeId_int32_count__();
            Request.ComposeId=composeId;
            Request.Count=count;

        }

        public __RPC_Logic_ComposeItem_ARG_int32_composeId_int32_count__ Request { get; private set; }

            private __RPC_Logic_ComposeItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ComposeItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class FinishAchievementOutMessage : OutMessage
    {
        public FinishAchievementOutMessage(IAgentBase sender, int achievementId)
            : base(sender, ServiceType.Logic, 1087)
        {
            Request = new __RPC_Logic_FinishAchievement_ARG_int32_achievementId__();
            Request.AchievementId=achievementId;

        }

        public __RPC_Logic_FinishAchievement_ARG_int32_achievementId__ Request { get; private set; }


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

    public class RewardAchievementOutMessage : OutMessage
    {
        public RewardAchievementOutMessage(IAgentBase sender, int achievementId)
            : base(sender, ServiceType.Logic, 1088)
        {
            Request = new __RPC_Logic_RewardAchievement_ARG_int32_achievementId__();
            Request.AchievementId=achievementId;

        }

        public __RPC_Logic_RewardAchievement_ARG_int32_achievementId__ Request { get; private set; }

            private __RPC_Logic_RewardAchievement_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RewardAchievement_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DistributionAttrPointOutMessage : OutMessage
    {
        public DistributionAttrPointOutMessage(IAgentBase sender, int Strength, int Agility, int Intelligence, int Endurance)
            : base(sender, ServiceType.Logic, 1089)
        {
            Request = new __RPC_Logic_DistributionAttrPoint_ARG_int32_Strength_int32_Agility_int32_Intelligence_int32_Endurance__();
            Request.Strength=Strength;
            Request.Agility=Agility;
            Request.Intelligence=Intelligence;
            Request.Endurance=Endurance;

        }

        public __RPC_Logic_DistributionAttrPoint_ARG_int32_Strength_int32_Agility_int32_Intelligence_int32_Endurance__ Request { get; private set; }

            private __RPC_Logic_DistributionAttrPoint_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DistributionAttrPoint_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class RefreshAttrPointOutMessage : OutMessage
    {
        public RefreshAttrPointOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1090)
        {
            Request = new __RPC_Logic_RefreshAttrPoint_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_RefreshAttrPoint_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_RefreshAttrPoint_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RefreshAttrPoint_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SetAttributeAutoAddOutMessage : OutMessage
    {
        public SetAttributeAutoAddOutMessage(IAgentBase sender, int isAuto)
            : base(sender, ServiceType.Logic, 1091)
        {
            Request = new __RPC_Logic_SetAttributeAutoAdd_ARG_int32_isAuto__();
            Request.IsAuto=isAuto;

        }

        public __RPC_Logic_SetAttributeAutoAdd_ARG_int32_isAuto__ Request { get; private set; }

            private __RPC_Logic_SetAttributeAutoAdd_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SetAttributeAutoAdd_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyFriendsOutMessage : OutMessage
    {
        public ApplyFriendsOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1092)
        {
            Request = new __RPC_Logic_ApplyFriends_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_ApplyFriends_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_ApplyFriends_RET_CharacterSimpleDataList__ mResponse;
            public CharacterSimpleDataList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyFriends_RET_CharacterSimpleDataList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SeekCharactersOutMessage : OutMessage
    {
        public SeekCharactersOutMessage(IAgentBase sender, string name)
            : base(sender, ServiceType.Logic, 1093)
        {
            Request = new __RPC_Logic_SeekCharacters_ARG_string_name__();
            Request.Name=name;

        }

        public __RPC_Logic_SeekCharacters_ARG_string_name__ Request { get; private set; }

            private __RPC_Logic_SeekCharacters_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SeekCharacters_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SeekCharactersReceiveOutMessage : OutMessage
    {
        public SeekCharactersReceiveOutMessage(IAgentBase sender, CharacterSimpleDataList result)
            : base(sender, ServiceType.Logic, 1393)
        {
            Request = new __RPC_Logic_SeekCharactersReceive_ARG_CharacterSimpleDataList_result__();
            Request.Result=result;

        }

        public __RPC_Logic_SeekCharactersReceive_ARG_CharacterSimpleDataList_result__ Request { get; private set; }


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

    public class SeekFriendsOutMessage : OutMessage
    {
        public SeekFriendsOutMessage(IAgentBase sender, string name)
            : base(sender, ServiceType.Logic, 1094)
        {
            Request = new __RPC_Logic_SeekFriends_ARG_string_name__();
            Request.Name=name;

        }

        public __RPC_Logic_SeekFriends_ARG_string_name__ Request { get; private set; }

            private __RPC_Logic_SeekFriends_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SeekFriends_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SeekFriendsReceiveOutMessage : OutMessage
    {
        public SeekFriendsReceiveOutMessage(IAgentBase sender, CharacterSimpleDataList result)
            : base(sender, ServiceType.Logic, 1394)
        {
            Request = new __RPC_Logic_SeekFriendsReceive_ARG_CharacterSimpleDataList_result__();
            Request.Result=result;

        }

        public __RPC_Logic_SeekFriendsReceive_ARG_CharacterSimpleDataList_result__ Request { get; private set; }


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

    public class AddFriendByIdOutMessage : OutMessage
    {
        public AddFriendByIdOutMessage(IAgentBase sender, ulong characterId, int type)
            : base(sender, ServiceType.Logic, 1095)
        {
            Request = new __RPC_Logic_AddFriendById_ARG_uint64_characterId_int32_type__();
            Request.CharacterId=characterId;
            Request.Type=type;

        }

        public __RPC_Logic_AddFriendById_ARG_uint64_characterId_int32_type__ Request { get; private set; }

            private __RPC_Logic_AddFriendById_RET_CharacterSimpleData__ mResponse;
            public CharacterSimpleData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_AddFriendById_RET_CharacterSimpleData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AddFriendByNameOutMessage : OutMessage
    {
        public AddFriendByNameOutMessage(IAgentBase sender, string Name, int type)
            : base(sender, ServiceType.Logic, 1097)
        {
            Request = new __RPC_Logic_AddFriendByName_ARG_string_Name_int32_type__();
            Request.Name=Name;
            Request.Type=type;

        }

        public __RPC_Logic_AddFriendByName_ARG_string_Name_int32_type__ Request { get; private set; }

            private __RPC_Logic_AddFriendByName_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AddFriendByName_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DelFriendByIdOutMessage : OutMessage
    {
        public DelFriendByIdOutMessage(IAgentBase sender, ulong characterId, int type)
            : base(sender, ServiceType.Logic, 1098)
        {
            Request = new __RPC_Logic_DelFriendById_ARG_uint64_characterId_int32_type__();
            Request.CharacterId=characterId;
            Request.Type=type;

        }

        public __RPC_Logic_DelFriendById_ARG_uint64_characterId_int32_type__ Request { get; private set; }

            private __RPC_Logic_DelFriendById_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DelFriendById_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DungeonCompleteOutMessage : OutMessage
    {
        public DungeonCompleteOutMessage(IAgentBase sender, FubenResult result)
            : base(sender, ServiceType.Logic, 1101)
        {
            Request = new __RPC_Logic_DungeonComplete_ARG_FubenResult_result__();
            Request.Result=result;

        }

        public __RPC_Logic_DungeonComplete_ARG_FubenResult_result__ Request { get; private set; }


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

    public class SelectDungeonRewardOutMessage : OutMessage
    {
        public SelectDungeonRewardOutMessage(IAgentBase sender, int fubenId, int select)
            : base(sender, ServiceType.Logic, 1102)
        {
            Request = new __RPC_Logic_SelectDungeonReward_ARG_int32_fubenId_int32_select__();
            Request.FubenId=fubenId;
            Request.Select=select;

        }

        public __RPC_Logic_SelectDungeonReward_ARG_int32_fubenId_int32_select__ Request { get; private set; }

            private __RPC_Logic_SelectDungeonReward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SelectDungeonReward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EnterFubenOutMessage : OutMessage
    {
        public EnterFubenOutMessage(IAgentBase sender, int fubenId)
            : base(sender, ServiceType.Logic, 1103)
        {
            Request = new __RPC_Logic_EnterFuben_ARG_int32_fubenId__();
            Request.FubenId=fubenId;

        }

        public __RPC_Logic_EnterFuben_ARG_int32_fubenId__ Request { get; private set; }

            private __RPC_Logic_EnterFuben_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_EnterFuben_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ResetFubenOutMessage : OutMessage
    {
        public ResetFubenOutMessage(IAgentBase sender, int fubenId)
            : base(sender, ServiceType.Logic, 1104)
        {
            Request = new __RPC_Logic_ResetFuben_ARG_int32_fubenId__();
            Request.FubenId=fubenId;

        }

        public __RPC_Logic_ResetFuben_ARG_int32_fubenId__ Request { get; private set; }

            private __RPC_Logic_ResetFuben_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ResetFuben_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SweepFubenOutMessage : OutMessage
    {
        public SweepFubenOutMessage(IAgentBase sender, int fubenId)
            : base(sender, ServiceType.Logic, 1105)
        {
            Request = new __RPC_Logic_SweepFuben_ARG_int32_fubenId__();
            Request.FubenId=fubenId;

        }

        public __RPC_Logic_SweepFuben_ARG_int32_fubenId__ Request { get; private set; }

            private __RPC_Logic_SweepFuben_RET_DrawResult__ mResponse;
            public DrawResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_SweepFuben_RET_DrawResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyStoresOutMessage : OutMessage
    {
        public ApplyStoresOutMessage(IAgentBase sender, int type, int serviceType)
            : base(sender, ServiceType.Logic, 1106)
        {
            Request = new __RPC_Logic_ApplyStores_ARG_int32_type_int32_serviceType__();
            Request.Type=type;
            Request.ServiceType=serviceType;

        }

        public __RPC_Logic_ApplyStores_ARG_int32_type_int32_serviceType__ Request { get; private set; }

            private __RPC_Logic_ApplyStores_RET_StoneItems__ mResponse;
            public StoneItems Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyStores_RET_StoneItems__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ActivateBookOutMessage : OutMessage
    {
        public ActivateBookOutMessage(IAgentBase sender, int itemId, int groupId, int index)
            : base(sender, ServiceType.Logic, 1107)
        {
            Request = new __RPC_Logic_ActivateBook_ARG_int32_itemId_int32_groupId_int32_index__();
            Request.ItemId=itemId;
            Request.GroupId=groupId;
            Request.Index=index;

        }

        public __RPC_Logic_ActivateBook_ARG_int32_itemId_int32_groupId_int32_index__ Request { get; private set; }

            private __RPC_Logic_ActivateBook_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ActivateBook_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SortBagOutMessage : OutMessage
    {
        public SortBagOutMessage(IAgentBase sender, int bagId)
            : base(sender, ServiceType.Logic, 1108)
        {
            Request = new __RPC_Logic_SortBag_ARG_int32_bagId__();
            Request.BagId=bagId;

        }

        public __RPC_Logic_SortBag_ARG_int32_bagId__ Request { get; private set; }

            private __RPC_Logic_SortBag_RET_BagBaseData__ mResponse;
            public BagBaseData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_SortBag_RET_BagBaseData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPlayerInfoOutMessage : OutMessage
    {
        public ApplyPlayerInfoOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Logic, 1109)
        {
            Request = new __RPC_Logic_ApplyPlayerInfo_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Logic_ApplyPlayerInfo_ARG_uint64_characterId__ Request { get; private set; }

            private __RPC_Logic_ApplyPlayerInfo_RET_PlayerInfoMsg__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyPlayerInfo_RET_PlayerInfoMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SetFlagOutMessage : OutMessage
    {
        public SetFlagOutMessage(IAgentBase sender, Int32Array trueDatas, Int32Array falseDatas)
            : base(sender, ServiceType.Logic, 1110)
        {
            Request = new __RPC_Logic_SetFlag_ARG_Int32Array_trueDatas_Int32Array_falseDatas__();
            Request.TrueDatas=trueDatas;
            Request.FalseDatas=falseDatas;

        }

        public __RPC_Logic_SetFlag_ARG_Int32Array_trueDatas_Int32Array_falseDatas__ Request { get; private set; }

            private __RPC_Logic_SetFlag_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SetFlag_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SetExDataOutMessage : OutMessage
    {
        public SetExDataOutMessage(IAgentBase sender, Dict_int_int_Data datas)
            : base(sender, ServiceType.Logic, 1111)
        {
            Request = new __RPC_Logic_SetExData_ARG_Dict_int_int_Data_datas__();
            Request.Datas=datas;

        }

        public __RPC_Logic_SetExData_ARG_Dict_int_int_Data_datas__ Request { get; private set; }

            private __RPC_Logic_SetExData_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SetExData_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyMailsOutMessage : OutMessage
    {
        public ApplyMailsOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1112)
        {
            Request = new __RPC_Logic_ApplyMails_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_ApplyMails_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_ApplyMails_RET_MailList__ mResponse;
            public MailList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyMails_RET_MailList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyMailInfoOutMessage : OutMessage
    {
        public ApplyMailInfoOutMessage(IAgentBase sender, ulong mailId)
            : base(sender, ServiceType.Logic, 1113)
        {
            Request = new __RPC_Logic_ApplyMailInfo_ARG_uint64_mailId__();
            Request.MailId=mailId;

        }

        public __RPC_Logic_ApplyMailInfo_ARG_uint64_mailId__ Request { get; private set; }

            private __RPC_Logic_ApplyMailInfo_RET_MailInfo__ mResponse;
            public MailInfo Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyMailInfo_RET_MailInfo__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ReceiveMailOutMessage : OutMessage
    {
        public ReceiveMailOutMessage(IAgentBase sender, Uint64Array mails)
            : base(sender, ServiceType.Logic, 1114)
        {
            Request = new __RPC_Logic_ReceiveMail_ARG_Uint64Array_mails__();
            Request.Mails=mails;

        }

        public __RPC_Logic_ReceiveMail_ARG_Uint64Array_mails__ Request { get; private set; }

            private __RPC_Logic_ReceiveMail_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ReceiveMail_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DeleteMailOutMessage : OutMessage
    {
        public DeleteMailOutMessage(IAgentBase sender, Uint64Array mails)
            : base(sender, ServiceType.Logic, 1115)
        {
            Request = new __RPC_Logic_DeleteMail_ARG_Uint64Array_mails__();
            Request.Mails=mails;

        }

        public __RPC_Logic_DeleteMail_ARG_Uint64Array_mails__ Request { get; private set; }

            private __RPC_Logic_DeleteMail_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DeleteMail_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncMailsOutMessage : OutMessage
    {
        public SyncMailsOutMessage(IAgentBase sender, MailList mails)
            : base(sender, ServiceType.Logic, 1116)
        {
            Request = new __RPC_Logic_SyncMails_ARG_MailList_mails__();
            Request.Mails=mails;

        }

        public __RPC_Logic_SyncMails_ARG_MailList_mails__ Request { get; private set; }


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

    public class RepairEquipOutMessage : OutMessage
    {
        public RepairEquipOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1118)
        {
            Request = new __RPC_Logic_RepairEquip_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_RepairEquip_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_RepairEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RepairEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DepotTakeOutOutMessage : OutMessage
    {
        public DepotTakeOutOutMessage(IAgentBase sender, int index)
            : base(sender, ServiceType.Logic, 1120)
        {
            Request = new __RPC_Logic_DepotTakeOut_ARG_int32_index__();
            Request.Index=index;

        }

        public __RPC_Logic_DepotTakeOut_ARG_int32_index__ Request { get; private set; }

            private __RPC_Logic_DepotTakeOut_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DepotTakeOut_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DepotPutInOutMessage : OutMessage
    {
        public DepotPutInOutMessage(IAgentBase sender, int bagId, int index)
            : base(sender, ServiceType.Logic, 1121)
        {
            Request = new __RPC_Logic_DepotPutIn_ARG_int32_bagId_int32_index__();
            Request.BagId=bagId;
            Request.Index=index;

        }

        public __RPC_Logic_DepotPutIn_ARG_int32_bagId_int32_index__ Request { get; private set; }

            private __RPC_Logic_DepotPutIn_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DepotPutIn_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class WishingPoolDepotTakeOutOutMessage : OutMessage
    {
        public WishingPoolDepotTakeOutOutMessage(IAgentBase sender, int index)
            : base(sender, ServiceType.Logic, 1122)
        {
            Request = new __RPC_Logic_WishingPoolDepotTakeOut_ARG_int32_index__();
            Request.Index=index;

        }

        public __RPC_Logic_WishingPoolDepotTakeOut_ARG_int32_index__ Request { get; private set; }

            private __RPC_Logic_WishingPoolDepotTakeOut_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_WishingPoolDepotTakeOut_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreBuyOutMessage : OutMessage
    {
        public StoreBuyOutMessage(IAgentBase sender, int storeId, int count, int serviceType)
            : base(sender, ServiceType.Logic, 1123)
        {
            Request = new __RPC_Logic_StoreBuy_ARG_int32_storeId_int32_count_int32_serviceType__();
            Request.StoreId=storeId;
            Request.Count=count;
            Request.ServiceType=serviceType;

        }

        public __RPC_Logic_StoreBuy_ARG_int32_storeId_int32_count_int32_serviceType__ Request { get; private set; }

            private __RPC_Logic_StoreBuy_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreBuy_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyCityDataOutMessage : OutMessage
    {
        public ApplyCityDataOutMessage(IAgentBase sender, int buildingId)
            : base(sender, ServiceType.Logic, 1124)
        {
            Request = new __RPC_Logic_ApplyCityData_ARG_int32_buildingId__();
            Request.BuildingId=buildingId;

        }

        public __RPC_Logic_ApplyCityData_ARG_int32_buildingId__ Request { get; private set; }

            private __RPC_Logic_ApplyCityData_RET_CityDataMsg__ mResponse;
            public CityDataMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyCityData_RET_CityDataMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class CityOperationRequestOutMessage : OutMessage
    {
        public CityOperationRequestOutMessage(IAgentBase sender, int opType, int buildingIdx, Int32Array param)
            : base(sender, ServiceType.Logic, 1125)
        {
            Request = new __RPC_Logic_CityOperationRequest_ARG_int32_opType_int32_buildingIdx_Int32Array_param__();
            Request.OpType=opType;
            Request.BuildingIdx=buildingIdx;
            Request.Param=param;

        }

        public __RPC_Logic_CityOperationRequest_ARG_int32_opType_int32_buildingIdx_Int32Array_param__ Request { get; private set; }

            private __RPC_Logic_CityOperationRequest_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_CityOperationRequest_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncCityBuildingDataOutMessage : OutMessage
    {
        public SyncCityBuildingDataOutMessage(IAgentBase sender, BuildingList data)
            : base(sender, ServiceType.Logic, 1126)
        {
            Request = new __RPC_Logic_SyncCityBuildingData_ARG_BuildingList_data__();
            Request.Data=data;

        }

        public __RPC_Logic_SyncCityBuildingData_ARG_BuildingList_data__ Request { get; private set; }


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

    public class SyncPetMissionOutMessage : OutMessage
    {
        public SyncPetMissionOutMessage(IAgentBase sender, PetMissionList msg)
            : base(sender, ServiceType.Logic, 1127)
        {
            Request = new __RPC_Logic_SyncPetMission_ARG_PetMissionList_msg__();
            Request.Msg=msg;

        }

        public __RPC_Logic_SyncPetMission_ARG_PetMissionList_msg__ Request { get; private set; }


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

    public class EnterCityOutMessage : OutMessage
    {
        public EnterCityOutMessage(IAgentBase sender, int cityId)
            : base(sender, ServiceType.Logic, 1128)
        {
            Request = new __RPC_Logic_EnterCity_ARG_int32_cityId__();
            Request.CityId=cityId;

        }

        public __RPC_Logic_EnterCity_ARG_int32_cityId__ Request { get; private set; }

            private __RPC_Logic_EnterCity_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_EnterCity_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EquipDurableBrokenOutMessage : OutMessage
    {
        public EquipDurableBrokenOutMessage(IAgentBase sender, int partId, int value)
            : base(sender, ServiceType.Logic, 1129)
        {
            Request = new __RPC_Logic_EquipDurableBroken_ARG_int32_partId_int32_value__();
            Request.PartId=partId;
            Request.Value=value;

        }

        public __RPC_Logic_EquipDurableBroken_ARG_int32_partId_int32_value__ Request { get; private set; }


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

    public class EquipDurableChangeOutMessage : OutMessage
    {
        public EquipDurableChangeOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1130)
        {
            Request = new __RPC_Logic_EquipDurableChange_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_EquipDurableChange_ARG_int32_placeholder__ Request { get; private set; }


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

    public class ApplyEquipDurableOutMessage : OutMessage
    {
        public ApplyEquipDurableOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1131)
        {
            Request = new __RPC_Logic_ApplyEquipDurable_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_ApplyEquipDurable_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_ApplyEquipDurable_RET_Dict_int_int_Data__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyEquipDurable_RET_Dict_int_int_Data__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ElfOperateOutMessage : OutMessage
    {
        public ElfOperateOutMessage(IAgentBase sender, int index, int type, int targetIndex)
            : base(sender, ServiceType.Logic, 1132)
        {
            Request = new __RPC_Logic_ElfOperate_ARG_int32_index_int32_type_int32_targetIndex__();
            Request.Index=index;
            Request.Type=type;
            Request.TargetIndex=targetIndex;

        }

        public __RPC_Logic_ElfOperate_ARG_int32_index_int32_type_int32_targetIndex__ Request { get; private set; }

            private __RPC_Logic_ElfOperate_RET_uint64__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ElfOperate_RET_uint64__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ElfReplaceOutMessage : OutMessage
    {
        public ElfReplaceOutMessage(IAgentBase sender, int from, int to)
            : base(sender, ServiceType.Logic, 1133)
        {
            Request = new __RPC_Logic_ElfReplace_ARG_int32_from_int32_to__();
            Request.From=from;
            Request.To=to;

        }

        public __RPC_Logic_ElfReplace_ARG_int32_from_int32_to__ Request { get; private set; }

            private __RPC_Logic_ElfReplace_RET_uint64__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ElfReplace_RET_uint64__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class WingFormationOutMessage : OutMessage
    {
        public WingFormationOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1136)
        {
            Request = new __RPC_Logic_WingFormation_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_WingFormation_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_WingFormation_RET_WingAdvanceResult__ mResponse;
            public WingAdvanceResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_WingFormation_RET_WingAdvanceResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class WingTrainOutMessage : OutMessage
    {
        public WingTrainOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1137)
        {
            Request = new __RPC_Logic_WingTrain_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_WingTrain_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_WingTrain_RET_WingTrainResult__ mResponse;
            public WingTrainResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_WingTrain_RET_WingTrainResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class OperatePetOutMessage : OutMessage
    {
        public OperatePetOutMessage(IAgentBase sender, int petId, int type, int param)
            : base(sender, ServiceType.Logic, 1138)
        {
            Request = new __RPC_Logic_OperatePet_ARG_int32_petId_int32_type_int32_param__();
            Request.PetId=petId;
            Request.Type=type;
            Request.Param=param;

        }

        public __RPC_Logic_OperatePet_ARG_int32_petId_int32_type_int32_param__ Request { get; private set; }

            private __RPC_Logic_OperatePet_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_OperatePet_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class OperatePetMissionOutMessage : OutMessage
    {
        public OperatePetMissionOutMessage(IAgentBase sender, int id, int type, Int32Array param)
            : base(sender, ServiceType.Logic, 1139)
        {
            Request = new __RPC_Logic_OperatePetMission_ARG_int32_id_int32_type_Int32Array_param__();
            Request.Id=id;
            Request.Type=type;
            Request.Param=param;

        }

        public __RPC_Logic_OperatePetMission_ARG_int32_id_int32_type_Int32Array_param__ Request { get; private set; }

            private __RPC_Logic_OperatePetMission_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_OperatePetMission_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class PickUpMedalOutMessage : OutMessage
    {
        public PickUpMedalOutMessage(IAgentBase sender, int index)
            : base(sender, ServiceType.Logic, 1140)
        {
            Request = new __RPC_Logic_PickUpMedal_ARG_int32_index__();
            Request.Index=index;

        }

        public __RPC_Logic_PickUpMedal_ARG_int32_index__ Request { get; private set; }

            private __RPC_Logic_PickUpMedal_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_PickUpMedal_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EnchanceMedalOutMessage : OutMessage
    {
        public EnchanceMedalOutMessage(IAgentBase sender, int bagId, int bagIndex, Int32Array medalTempBag, Int32Array medalUseBag)
            : base(sender, ServiceType.Logic, 1141)
        {
            Request = new __RPC_Logic_EnchanceMedal_ARG_int32_bagId_int32_bagIndex_Int32Array_medalTempBag_Int32Array_medalUseBag__();
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;
            Request.MedalTempBag=medalTempBag;
            Request.MedalUseBag=medalUseBag;

        }

        public __RPC_Logic_EnchanceMedal_ARG_int32_bagId_int32_bagIndex_Int32Array_medalTempBag_Int32Array_medalUseBag__ Request { get; private set; }

            private __RPC_Logic_EnchanceMedal_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_EnchanceMedal_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class EquipMedalOutMessage : OutMessage
    {
        public EquipMedalOutMessage(IAgentBase sender, int bagId, int bagIndex)
            : base(sender, ServiceType.Logic, 1142)
        {
            Request = new __RPC_Logic_EquipMedal_ARG_int32_bagId_int32_bagIndex__();
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;

        }

        public __RPC_Logic_EquipMedal_ARG_int32_bagId_int32_bagIndex__ Request { get; private set; }

            private __RPC_Logic_EquipMedal_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_EquipMedal_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class BuySpaceBagOutMessage : OutMessage
    {
        public BuySpaceBagOutMessage(IAgentBase sender, int bagId, int bagIndex, int needCount)
            : base(sender, ServiceType.Logic, 1143)
        {
            Request = new __RPC_Logic_BuySpaceBag_ARG_int32_bagId_int32_bagIndex_int32_needCount__();
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;
            Request.NeedCount=needCount;

        }

        public __RPC_Logic_BuySpaceBag_ARG_int32_bagId_int32_bagIndex_int32_needCount__ Request { get; private set; }

            private __RPC_Logic_BuySpaceBag_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_BuySpaceBag_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DeletePetMissionOutMessage : OutMessage
    {
        public DeletePetMissionOutMessage(IAgentBase sender, int missionId)
            : base(sender, ServiceType.Logic, 1144)
        {
            Request = new __RPC_Logic_DeletePetMission_ARG_int32_missionId__();
            Request.MissionId=missionId;

        }

        public __RPC_Logic_DeletePetMission_ARG_int32_missionId__ Request { get; private set; }


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

    public class UseBuildServiceOutMessage : OutMessage
    {
        public UseBuildServiceOutMessage(IAgentBase sender, int areaId, int serviceId, Int32Array param)
            : base(sender, ServiceType.Logic, 1145)
        {
            Request = new __RPC_Logic_UseBuildService_ARG_int32_areaId_int32_serviceId_Int32Array_param__();
            Request.AreaId=areaId;
            Request.ServiceId=serviceId;
            Request.Param=param;

        }

        public __RPC_Logic_UseBuildService_ARG_int32_areaId_int32_serviceId_Int32Array_param__ Request { get; private set; }

            private __RPC_Logic_UseBuildService_RET_UseBuildServiceResult__ mResponse;
            public UseBuildServiceResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_UseBuildService_RET_UseBuildServiceResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetP1vP1LadderPlayerOutMessage : OutMessage
    {
        public GetP1vP1LadderPlayerOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1146)
        {
            Request = new __RPC_Logic_GetP1vP1LadderPlayer_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_GetP1vP1LadderPlayer_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_GetP1vP1LadderPlayer_RET_P1vP1Ladder__ mResponse;
            public P1vP1Ladder Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetP1vP1LadderPlayer_RET_P1vP1Ladder__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetP1vP1FightPlayerOutMessage : OutMessage
    {
        public GetP1vP1FightPlayerOutMessage(IAgentBase sender, int rank, ulong guid, int type)
            : base(sender, ServiceType.Logic, 1147)
        {
            Request = new __RPC_Logic_GetP1vP1FightPlayer_ARG_int32_rank_uint64_guid_int32_type__();
            Request.Rank=rank;
            Request.Guid=guid;
            Request.Type=type;

        }

        public __RPC_Logic_GetP1vP1FightPlayer_ARG_int32_rank_uint64_guid_int32_type__ Request { get; private set; }

            private __RPC_Logic_GetP1vP1FightPlayer_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetP1vP1FightPlayer_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetP1vP1LadderOldListOutMessage : OutMessage
    {
        public GetP1vP1LadderOldListOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1151)
        {
            Request = new __RPC_Logic_GetP1vP1LadderOldList_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_GetP1vP1LadderOldList_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_GetP1vP1LadderOldList_RET_P1vP1ChangeList__ mResponse;
            public P1vP1ChangeList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetP1vP1LadderOldList_RET_P1vP1ChangeList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class LogicP1vP1FightResultOutMessage : OutMessage
    {
        public LogicP1vP1FightResultOutMessage(IAgentBase sender, P1vP1RewardData data)
            : base(sender, ServiceType.Logic, 1152)
        {
            Request = new __RPC_Logic_LogicP1vP1FightResult_ARG_P1vP1RewardData_data__();
            Request.Data=data;

        }

        public __RPC_Logic_LogicP1vP1FightResult_ARG_P1vP1RewardData_data__ Request { get; private set; }


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

    public class BuyP1vP1CountOutMessage : OutMessage
    {
        public BuyP1vP1CountOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1153)
        {
            Request = new __RPC_Logic_BuyP1vP1Count_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_BuyP1vP1Count_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_BuyP1vP1Count_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_BuyP1vP1Count_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DrawLotteryPetEggOutMessage : OutMessage
    {
        public DrawLotteryPetEggOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1154)
        {
            Request = new __RPC_Logic_DrawLotteryPetEgg_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_DrawLotteryPetEgg_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_DrawLotteryPetEgg_RET_LotteryResult__ mResponse;
            public LotteryResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_DrawLotteryPetEgg_RET_LotteryResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class RecoveryEquipOutMessage : OutMessage
    {
        public RecoveryEquipOutMessage(IAgentBase sender, int type, Int32Array indexList)
            : base(sender, ServiceType.Logic, 1155)
        {
            Request = new __RPC_Logic_RecoveryEquip_ARG_int32_type_Int32Array_indexList__();
            Request.Type=type;
            Request.IndexList=indexList;

        }

        public __RPC_Logic_RecoveryEquip_ARG_int32_type_Int32Array_indexList__ Request { get; private set; }

            private __RPC_Logic_RecoveryEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RecoveryEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DrawWishingPoolOutMessage : OutMessage
    {
        public DrawWishingPoolOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1156)
        {
            Request = new __RPC_Logic_DrawWishingPool_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_DrawWishingPool_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_DrawWishingPool_RET_BagsChangeData__ mResponse;
            public BagsChangeData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_DrawWishingPool_RET_BagsChangeData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ResetSkillTalentOutMessage : OutMessage
    {
        public ResetSkillTalentOutMessage(IAgentBase sender, int skillId)
            : base(sender, ServiceType.Logic, 1157)
        {
            Request = new __RPC_Logic_ResetSkillTalent_ARG_int32_skillId__();
            Request.SkillId=skillId;

        }

        public __RPC_Logic_ResetSkillTalent_ARG_int32_skillId__ Request { get; private set; }

            private __RPC_Logic_ResetSkillTalent_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ResetSkillTalent_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class RobotcFinishFubenOutMessage : OutMessage
    {
        public RobotcFinishFubenOutMessage(IAgentBase sender, int fubenId)
            : base(sender, ServiceType.Logic, 1158)
        {
            Request = new __RPC_Logic_RobotcFinishFuben_ARG_int32_fubenId__();
            Request.FubenId=fubenId;

        }

        public __RPC_Logic_RobotcFinishFuben_ARG_int32_fubenId__ Request { get; private set; }

            private __RPC_Logic_RobotcFinishFuben_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RobotcFinishFuben_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class CreateAllianceOutMessage : OutMessage
    {
        public CreateAllianceOutMessage(IAgentBase sender, string name)
            : base(sender, ServiceType.Logic, 1159)
        {
            Request = new __RPC_Logic_CreateAlliance_ARG_string_name__();
            Request.Name=name;

        }

        public __RPC_Logic_CreateAlliance_ARG_string_name__ Request { get; private set; }

            private __RPC_Logic_CreateAlliance_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_CreateAlliance_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AllianceOperationOutMessage : OutMessage
    {
        public AllianceOperationOutMessage(IAgentBase sender, int type, int value)
            : base(sender, ServiceType.Logic, 1160)
        {
            Request = new __RPC_Logic_AllianceOperation_ARG_int32_type_int32_value__();
            Request.Type=type;
            Request.Value=value;

        }

        public __RPC_Logic_AllianceOperation_ARG_int32_type_int32_value__ Request { get; private set; }

            private __RPC_Logic_AllianceOperation_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AllianceOperation_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AllianceOperationCharacterOutMessage : OutMessage
    {
        public AllianceOperationCharacterOutMessage(IAgentBase sender, int type, ulong guid)
            : base(sender, ServiceType.Logic, 1161)
        {
            Request = new __RPC_Logic_AllianceOperationCharacter_ARG_int32_type_uint64_guid__();
            Request.Type=type;
            Request.Guid=guid;

        }

        public __RPC_Logic_AllianceOperationCharacter_ARG_int32_type_uint64_guid__ Request { get; private set; }

            private __RPC_Logic_AllianceOperationCharacter_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AllianceOperationCharacter_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AllianceOperationCharacterByNameOutMessage : OutMessage
    {
        public AllianceOperationCharacterByNameOutMessage(IAgentBase sender, int type, string name)
            : base(sender, ServiceType.Logic, 1162)
        {
            Request = new __RPC_Logic_AllianceOperationCharacterByName_ARG_int32_type_string_name__();
            Request.Type=type;
            Request.Name=name;

        }

        public __RPC_Logic_AllianceOperationCharacterByName_ARG_int32_type_string_name__ Request { get; private set; }

            private __RPC_Logic_AllianceOperationCharacterByName_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AllianceOperationCharacterByName_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class LogicSyncAllianceMessageOutMessage : OutMessage
    {
        public LogicSyncAllianceMessageOutMessage(IAgentBase sender, int type, string name1, int allianceId, string name2)
            : base(sender, ServiceType.Logic, 1163)
        {
            Request = new __RPC_Logic_LogicSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__();
            Request.Type=type;
            Request.Name1=name1;
            Request.AllianceId=allianceId;
            Request.Name2=name2;

        }

        public __RPC_Logic_LogicSyncAllianceMessage_ARG_int32_type_string_name1_int32_allianceId_string_name2__ Request { get; private set; }


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

    public class WorshipCharacterOutMessage : OutMessage
    {
        public WorshipCharacterOutMessage(IAgentBase sender, ulong guid)
            : base(sender, ServiceType.Logic, 1166)
        {
            Request = new __RPC_Logic_WorshipCharacter_ARG_uint64_guid__();
            Request.Guid=guid;

        }

        public __RPC_Logic_WorshipCharacter_ARG_uint64_guid__ Request { get; private set; }

            private __RPC_Logic_WorshipCharacter_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_WorshipCharacter_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DonationAllianceItemOutMessage : OutMessage
    {
        public DonationAllianceItemOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1168)
        {
            Request = new __RPC_Logic_DonationAllianceItem_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_DonationAllianceItem_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_DonationAllianceItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_DonationAllianceItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ElfDrawOverOutMessage : OutMessage
    {
        public ElfDrawOverOutMessage(IAgentBase sender, DrawItemResult Items, long getTime)
            : base(sender, ServiceType.Logic, 1170)
        {
            Request = new __RPC_Logic_ElfDrawOver_ARG_DrawItemResult_Items_int64_getTime__();
            Request.Items=Items;
            Request.GetTime=getTime;

        }

        public __RPC_Logic_ElfDrawOver_ARG_DrawItemResult_Items_int64_getTime__ Request { get; private set; }


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

    public class TalentCountChangeOutMessage : OutMessage
    {
        public TalentCountChangeOutMessage(IAgentBase sender, int talentId, int value)
            : base(sender, ServiceType.Logic, 1171)
        {
            Request = new __RPC_Logic_TalentCountChange_ARG_int32_talentId_int32_value__();
            Request.TalentId=talentId;
            Request.Value=value;

        }

        public __RPC_Logic_TalentCountChange_ARG_int32_talentId_int32_value__ Request { get; private set; }


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

    public class CityMissionOperationOutMessage : OutMessage
    {
        public CityMissionOperationOutMessage(IAgentBase sender, int type, int missIndex, int cost)
            : base(sender, ServiceType.Logic, 1172)
        {
            Request = new __RPC_Logic_CityMissionOperation_ARG_int32_type_int32_missIndex_int32_cost__();
            Request.Type=type;
            Request.MissIndex=missIndex;
            Request.Cost=cost;

        }

        public __RPC_Logic_CityMissionOperation_ARG_int32_type_int32_missIndex_int32_cost__ Request { get; private set; }

            private __RPC_Logic_CityMissionOperation_RET_BagsChangeData__ mResponse;
            public BagsChangeData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_CityMissionOperation_RET_BagsChangeData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DropCityMissionOutMessage : OutMessage
    {
        public DropCityMissionOutMessage(IAgentBase sender, int missIndex)
            : base(sender, ServiceType.Logic, 1173)
        {
            Request = new __RPC_Logic_DropCityMission_ARG_int32_missIndex__();
            Request.MissIndex=missIndex;

        }

        public __RPC_Logic_DropCityMission_ARG_int32_missIndex__ Request { get; private set; }

            private __RPC_Logic_DropCityMission_RET_BuildMissionOne__ mResponse;
            public BuildMissionOne Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_DropCityMission_RET_BuildMissionOne__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class CityRefreshMissionOutMessage : OutMessage
    {
        public CityRefreshMissionOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1174)
        {
            Request = new __RPC_Logic_CityRefreshMission_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_CityRefreshMission_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_CityRefreshMission_RET_CityMissionDataMsg__ mResponse;
            public CityMissionDataMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_CityRefreshMission_RET_CityMissionDataMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationAddOutMessage : OutMessage
    {
        public StoreOperationAddOutMessage(IAgentBase sender, int type, int bagId, int bagIndex, int count, int needCount, int storeIndex)
            : base(sender, ServiceType.Logic, 1175)
        {
            Request = new __RPC_Logic_StoreOperationAdd_ARG_int32_type_int32_bagId_int32_bagIndex_int32_count_int32_needCount_int32_storeIndex__();
            Request.Type=type;
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;
            Request.Count=count;
            Request.NeedCount=needCount;
            Request.StoreIndex=storeIndex;

        }

        public __RPC_Logic_StoreOperationAdd_ARG_int32_type_int32_bagId_int32_bagIndex_int32_count_int32_needCount_int32_storeIndex__ Request { get; private set; }

            private __RPC_Logic_StoreOperationAdd_RET_int64__ mResponse;
            public long Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationAdd_RET_int64__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationBroadcastOutMessage : OutMessage
    {
        public StoreOperationBroadcastOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1176)
        {
            Request = new __RPC_Logic_StoreOperationBroadcast_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_StoreOperationBroadcast_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_StoreOperationBroadcast_RET_StoreBroadcastList__ mResponse;
            public StoreBroadcastList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationBroadcast_RET_StoreBroadcastList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationBuyOutMessage : OutMessage
    {
        public StoreOperationBuyOutMessage(IAgentBase sender, ulong guid, long storeId)
            : base(sender, ServiceType.Logic, 1177)
        {
            Request = new __RPC_Logic_StoreOperationBuy_ARG_uint64_guid_int64_storeId__();
            Request.Guid=guid;
            Request.StoreId=storeId;

        }

        public __RPC_Logic_StoreOperationBuy_ARG_uint64_guid_int64_storeId__ Request { get; private set; }

            private __RPC_Logic_StoreOperationBuy_RET_int64__ mResponse;
            public long Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationBuy_RET_int64__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationCancelOutMessage : OutMessage
    {
        public StoreOperationCancelOutMessage(IAgentBase sender, long storeId)
            : base(sender, ServiceType.Logic, 1178)
        {
            Request = new __RPC_Logic_StoreOperationCancel_ARG_int64_storeId__();
            Request.StoreId=storeId;

        }

        public __RPC_Logic_StoreOperationCancel_ARG_int64_storeId__ Request { get; private set; }

            private __RPC_Logic_StoreOperationCancel_RET_int64__ mResponse;
            public long Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationCancel_RET_int64__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationLookOutMessage : OutMessage
    {
        public StoreOperationLookOutMessage(IAgentBase sender, ulong guid)
            : base(sender, ServiceType.Logic, 1179)
        {
            Request = new __RPC_Logic_StoreOperationLook_ARG_uint64_guid__();
            Request.Guid=guid;

        }

        public __RPC_Logic_StoreOperationLook_ARG_uint64_guid__ Request { get; private set; }

            private __RPC_Logic_StoreOperationLook_RET_OtherStoreList__ mResponse;
            public OtherStoreList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationLook_RET_OtherStoreList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationLookSelfOutMessage : OutMessage
    {
        public StoreOperationLookSelfOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1180)
        {
            Request = new __RPC_Logic_StoreOperationLookSelf_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_StoreOperationLookSelf_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_StoreOperationLookSelf_RET_SelfStoreList__ mResponse;
            public SelfStoreList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationLookSelf_RET_SelfStoreList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class StoreOperationHarvestOutMessage : OutMessage
    {
        public StoreOperationHarvestOutMessage(IAgentBase sender, long storeId)
            : base(sender, ServiceType.Logic, 1181)
        {
            Request = new __RPC_Logic_StoreOperationHarvest_ARG_int64_storeId__();
            Request.StoreId=storeId;

        }

        public __RPC_Logic_StoreOperationHarvest_ARG_int64_storeId__ Request { get; private set; }

            private __RPC_Logic_StoreOperationHarvest_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreOperationHarvest_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SSStoreOperationExchangeOutMessage : OutMessage
    {
        public SSStoreOperationExchangeOutMessage(IAgentBase sender, int trade, int itemCount)
            : base(sender, ServiceType.Logic, 1183)
        {
            Request = new __RPC_Logic_SSStoreOperationExchange_ARG_int32_trade_int32_itemCount__();
            Request.Trade=trade;
            Request.ItemCount=itemCount;

        }

        public __RPC_Logic_SSStoreOperationExchange_ARG_int32_trade_int32_itemCount__ Request { get; private set; }

            private __RPC_Logic_SSStoreOperationExchange_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SSStoreOperationExchange_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyGroupShopItemsOutMessage : OutMessage
    {
        public ApplyGroupShopItemsOutMessage(IAgentBase sender, Int32Array types)
            : base(sender, ServiceType.Logic, 1185)
        {
            Request = new __RPC_Logic_ApplyGroupShopItems_ARG_Int32Array_types__();
            Request.Types=types;

        }

        public __RPC_Logic_ApplyGroupShopItems_ARG_Int32Array_types__ Request { get; private set; }

            private __RPC_Logic_ApplyGroupShopItems_RET_GroupShopItemAll__ mResponse;
            public GroupShopItemAll Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyGroupShopItems_RET_GroupShopItemAll__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyStoreBuyedOutMessage : OutMessage
    {
        public NotifyStoreBuyedOutMessage(IAgentBase sender, long storeId, ulong Aid, string Aname)
            : base(sender, ServiceType.Logic, 1186)
        {
            Request = new __RPC_Logic_NotifyStoreBuyed_ARG_int64_storeId_uint64_Aid_string_Aname__();
            Request.StoreId=storeId;
            Request.Aid=Aid;
            Request.Aname=Aname;

        }

        public __RPC_Logic_NotifyStoreBuyed_ARG_int64_storeId_uint64_Aid_string_Aname__ Request { get; private set; }


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

    public class BuyGroupShopItemOutMessage : OutMessage
    {
        public BuyGroupShopItemOutMessage(IAgentBase sender, long guid, int gropId, int count)
            : base(sender, ServiceType.Logic, 1187)
        {
            Request = new __RPC_Logic_BuyGroupShopItem_ARG_int64_guid_int32_gropId_int32_count__();
            Request.Guid=guid;
            Request.GropId=gropId;
            Request.Count=count;

        }

        public __RPC_Logic_BuyGroupShopItem_ARG_int64_guid_int32_gropId_int32_count__ Request { get; private set; }

            private __RPC_Logic_BuyGroupShopItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_BuyGroupShopItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetBuyedGroupShopItemsOutMessage : OutMessage
    {
        public GetBuyedGroupShopItemsOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1188)
        {
            Request = new __RPC_Logic_GetBuyedGroupShopItems_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_GetBuyedGroupShopItems_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_GetBuyedGroupShopItems_RET_GroupShopItemAll__ mResponse;
            public GroupShopItemAll Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetBuyedGroupShopItems_RET_GroupShopItemAll__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetGroupShopHistoryOutMessage : OutMessage
    {
        public GetGroupShopHistoryOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1189)
        {
            Request = new __RPC_Logic_GetGroupShopHistory_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_GetGroupShopHistory_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_GetGroupShopHistory_RET_GroupShopItemAll__ mResponse;
            public GroupShopItemAll Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetGroupShopHistory_RET_GroupShopItemAll__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AcceptBattleAwardOutMessage : OutMessage
    {
        public AcceptBattleAwardOutMessage(IAgentBase sender, int fubenId)
            : base(sender, ServiceType.Logic, 1193)
        {
            Request = new __RPC_Logic_AcceptBattleAward_ARG_int32_fubenId__();
            Request.FubenId=fubenId;

        }

        public __RPC_Logic_AcceptBattleAward_ARG_int32_fubenId__ Request { get; private set; }

            private __RPC_Logic_AcceptBattleAward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AcceptBattleAward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AstrologyLevelUpOutMessage : OutMessage
    {
        public AstrologyLevelUpOutMessage(IAgentBase sender, int bagId, int bagIndex, Int32Array needList)
            : base(sender, ServiceType.Logic, 1194)
        {
            Request = new __RPC_Logic_AstrologyLevelUp_ARG_int32_bagId_int32_bagIndex_Int32Array_needList__();
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;
            Request.NeedList=needList;

        }

        public __RPC_Logic_AstrologyLevelUp_ARG_int32_bagId_int32_bagIndex_Int32Array_needList__ Request { get; private set; }

            private __RPC_Logic_AstrologyLevelUp_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AstrologyLevelUp_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AstrologyEquipOnOutMessage : OutMessage
    {
        public AstrologyEquipOnOutMessage(IAgentBase sender, int bagIndex, int astrologyId, int Index)
            : base(sender, ServiceType.Logic, 1195)
        {
            Request = new __RPC_Logic_AstrologyEquipOn_ARG_int32_bagIndex_int32_astrologyId_int32_Index__();
            Request.BagIndex=bagIndex;
            Request.AstrologyId=astrologyId;
            Request.Index=Index;

        }

        public __RPC_Logic_AstrologyEquipOn_ARG_int32_bagIndex_int32_astrologyId_int32_Index__ Request { get; private set; }

            private __RPC_Logic_AstrologyEquipOn_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AstrologyEquipOn_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AstrologyEquipOffOutMessage : OutMessage
    {
        public AstrologyEquipOffOutMessage(IAgentBase sender, int astrologyId, int Index)
            : base(sender, ServiceType.Logic, 1196)
        {
            Request = new __RPC_Logic_AstrologyEquipOff_ARG_int32_astrologyId_int32_Index__();
            Request.AstrologyId=astrologyId;
            Request.Index=Index;

        }

        public __RPC_Logic_AstrologyEquipOff_ARG_int32_astrologyId_int32_Index__ Request { get; private set; }

            private __RPC_Logic_AstrologyEquipOff_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_AstrologyEquipOff_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AstrologyDrawOverOutMessage : OutMessage
    {
        public AstrologyDrawOverOutMessage(IAgentBase sender, DrawItemResult Items, long getTime)
            : base(sender, ServiceType.Logic, 1197)
        {
            Request = new __RPC_Logic_AstrologyDrawOver_ARG_DrawItemResult_Items_int64_getTime__();
            Request.Items=Items;
            Request.GetTime=getTime;

        }

        public __RPC_Logic_AstrologyDrawOver_ARG_DrawItemResult_Items_int64_getTime__ Request { get; private set; }


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

    public class SyncAddFriendOutMessage : OutMessage
    {
        public SyncAddFriendOutMessage(IAgentBase sender, int type, CharacterSimpleData character)
            : base(sender, ServiceType.Logic, 1198)
        {
            Request = new __RPC_Logic_SyncAddFriend_ARG_int32_type_CharacterSimpleData_character__();
            Request.Type=type;
            Request.Character=character;

        }

        public __RPC_Logic_SyncAddFriend_ARG_int32_type_CharacterSimpleData_character__ Request { get; private set; }


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

    public class UsePetExpItemOutMessage : OutMessage
    {
        public UsePetExpItemOutMessage(IAgentBase sender, int petId, int itemId, int itemCount)
            : base(sender, ServiceType.Logic, 1199)
        {
            Request = new __RPC_Logic_UsePetExpItem_ARG_int32_petId_int32_itemId_int32_itemCount__();
            Request.PetId=petId;
            Request.ItemId=itemId;
            Request.ItemCount=itemCount;

        }

        public __RPC_Logic_UsePetExpItem_ARG_int32_petId_int32_itemId_int32_itemCount__ Request { get; private set; }

            private __RPC_Logic_UsePetExpItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UsePetExpItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ReincarnationOutMessage : OutMessage
    {
        public ReincarnationOutMessage(IAgentBase sender, int typeId)
            : base(sender, ServiceType.Logic, 1200)
        {
            Request = new __RPC_Logic_Reincarnation_ARG_int32_typeId__();
            Request.TypeId=typeId;

        }

        public __RPC_Logic_Reincarnation_ARG_int32_typeId__ Request { get; private set; }

            private __RPC_Logic_Reincarnation_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_Reincarnation_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UpgradeHonorOutMessage : OutMessage
    {
        public UpgradeHonorOutMessage(IAgentBase sender, int typeId)
            : base(sender, ServiceType.Logic, 1201)
        {
            Request = new __RPC_Logic_UpgradeHonor_ARG_int32_typeId__();
            Request.TypeId=typeId;

        }

        public __RPC_Logic_UpgradeHonor_ARG_int32_typeId__ Request { get; private set; }

            private __RPC_Logic_UpgradeHonor_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UpgradeHonor_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class LogicNotifyMessageOutMessage : OutMessage
    {
        public LogicNotifyMessageOutMessage(IAgentBase sender, int type, string info, int addChat)
            : base(sender, ServiceType.Logic, 1203)
        {
            Request = new __RPC_Logic_LogicNotifyMessage_ARG_int32_type_string_info_int32_addChat__();
            Request.Type=type;
            Request.Info=info;
            Request.AddChat=addChat;

        }

        public __RPC_Logic_LogicNotifyMessage_ARG_int32_type_string_info_int32_addChat__ Request { get; private set; }


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

    public class NotifGainResOutMessage : OutMessage
    {
        public NotifGainResOutMessage(IAgentBase sender, DataChangeList changes)
            : base(sender, ServiceType.Logic, 1204)
        {
            Request = new __RPC_Logic_NotifGainRes_ARG_DataChangeList_changes__();
            Request.Changes=changes;

        }

        public __RPC_Logic_NotifGainRes_ARG_DataChangeList_changes__ Request { get; private set; }


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

    public class BattleResultOutMessage : OutMessage
    {
        public BattleResultOutMessage(IAgentBase sender, int dungeonId, int resultType, int first)
            : base(sender, ServiceType.Logic, 1205)
        {
            Request = new __RPC_Logic_BattleResult_ARG_int32_dungeonId_int32_resultType_int32_first__();
            Request.DungeonId=dungeonId;
            Request.ResultType=resultType;
            Request.First=first;

        }

        public __RPC_Logic_BattleResult_ARG_int32_dungeonId_int32_resultType_int32_first__ Request { get; private set; }


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

    public class ApplyCityBuildingDataOutMessage : OutMessage
    {
        public ApplyCityBuildingDataOutMessage(IAgentBase sender, int areaId)
            : base(sender, ServiceType.Logic, 1206)
        {
            Request = new __RPC_Logic_ApplyCityBuildingData_ARG_int32_areaId__();
            Request.AreaId=areaId;

        }

        public __RPC_Logic_ApplyCityBuildingData_ARG_int32_areaId__ Request { get; private set; }

            private __RPC_Logic_ApplyCityBuildingData_RET_BuildingData__ mResponse;
            public BuildingData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyCityBuildingData_RET_BuildingData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyBagByTypeOutMessage : OutMessage
    {
        public ApplyBagByTypeOutMessage(IAgentBase sender, int bagType)
            : base(sender, ServiceType.Logic, 1207)
        {
            Request = new __RPC_Logic_ApplyBagByType_ARG_int32_bagType__();
            Request.BagType=bagType;

        }

        public __RPC_Logic_ApplyBagByType_ARG_int32_bagType__ Request { get; private set; }

            private __RPC_Logic_ApplyBagByType_RET_BagBaseData__ mResponse;
            public BagBaseData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyBagByType_RET_BagBaseData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyP1vP1ChangeOutMessage : OutMessage
    {
        public NotifyP1vP1ChangeOutMessage(IAgentBase sender, P1vP1Change_One one)
            : base(sender, ServiceType.Logic, 1208)
        {
            Request = new __RPC_Logic_NotifyP1vP1Change_ARG_P1vP1Change_One_one__();
            Request.One=one;

        }

        public __RPC_Logic_NotifyP1vP1Change_ARG_P1vP1Change_One_one__ Request { get; private set; }


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

    public class SyncFriendDataChangeOutMessage : OutMessage
    {
        public SyncFriendDataChangeOutMessage(IAgentBase sender, CharacterSimpleDataList Changes)
            : base(sender, ServiceType.Logic, 1210)
        {
            Request = new __RPC_Logic_SyncFriendDataChange_ARG_CharacterSimpleDataList_Changes__();
            Request.Changes=Changes;

        }

        public __RPC_Logic_SyncFriendDataChange_ARG_CharacterSimpleDataList_Changes__ Request { get; private set; }


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

    public class SyncFriendDeleteOutMessage : OutMessage
    {
        public SyncFriendDeleteOutMessage(IAgentBase sender, int type, ulong characterId)
            : base(sender, ServiceType.Logic, 1211)
        {
            Request = new __RPC_Logic_SyncFriendDelete_ARG_int32_type_uint64_characterId__();
            Request.Type=type;
            Request.CharacterId=characterId;

        }

        public __RPC_Logic_SyncFriendDelete_ARG_int32_type_uint64_characterId__ Request { get; private set; }


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

    public class StoreBuyEquipOutMessage : OutMessage
    {
        public StoreBuyEquipOutMessage(IAgentBase sender, int storeId, int bagId, int bagIndex, int serviceType)
            : base(sender, ServiceType.Logic, 1216)
        {
            Request = new __RPC_Logic_StoreBuyEquip_ARG_int32_storeId_int32_bagId_int32_bagIndex_int32_serviceType__();
            Request.StoreId=storeId;
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;
            Request.ServiceType=serviceType;

        }

        public __RPC_Logic_StoreBuyEquip_ARG_int32_storeId_int32_bagId_int32_bagIndex_int32_serviceType__ Request { get; private set; }

            private __RPC_Logic_StoreBuyEquip_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_StoreBuyEquip_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetQuestionDataOutMessage : OutMessage
    {
        public GetQuestionDataOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1217)
        {
            Request = new __RPC_Logic_GetQuestionData_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_GetQuestionData_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_GetQuestionData_RET_QuestionMessage__ mResponse;
            public QuestionMessage Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetQuestionData_RET_QuestionMessage__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AnswerQuestionOutMessage : OutMessage
    {
        public AnswerQuestionOutMessage(IAgentBase sender, int answer)
            : base(sender, ServiceType.Logic, 1218)
        {
            Request = new __RPC_Logic_AnswerQuestion_ARG_int32_answer__();
            Request.Answer=answer;

        }

        public __RPC_Logic_AnswerQuestion_ARG_int32_answer__ Request { get; private set; }

            private __RPC_Logic_AnswerQuestion_RET_QuestionMessage__ mResponse;
            public QuestionMessage Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_AnswerQuestion_RET_QuestionMessage__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class RemoveErrorAnswerOutMessage : OutMessage
    {
        public RemoveErrorAnswerOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1220)
        {
            Request = new __RPC_Logic_RemoveErrorAnswer_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_RemoveErrorAnswer_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_RemoveErrorAnswer_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RemoveErrorAnswer_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class AnswerQuestionUseItemOutMessage : OutMessage
    {
        public AnswerQuestionUseItemOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1221)
        {
            Request = new __RPC_Logic_AnswerQuestionUseItem_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_AnswerQuestionUseItem_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_AnswerQuestionUseItem_RET_QuestionMessage__ mResponse;
            public QuestionMessage Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_AnswerQuestionUseItem_RET_QuestionMessage__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPlayerHeadInfoOutMessage : OutMessage
    {
        public ApplyPlayerHeadInfoOutMessage(IAgentBase sender, ulong characterId)
            : base(sender, ServiceType.Logic, 1222)
        {
            Request = new __RPC_Logic_ApplyPlayerHeadInfo_ARG_uint64_characterId__();
            Request.CharacterId=characterId;

        }

        public __RPC_Logic_ApplyPlayerHeadInfo_ARG_uint64_characterId__ Request { get; private set; }

            private __RPC_Logic_ApplyPlayerHeadInfo_RET_PlayerHeadInfoMsg__ mResponse;
            public PlayerHeadInfoMsg Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyPlayerHeadInfo_RET_PlayerHeadInfoMsg__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetCompensationListOutMessage : OutMessage
    {
        public GetCompensationListOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1224)
        {
            Request = new __RPC_Logic_GetCompensationList_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_GetCompensationList_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_GetCompensationList_RET_CompensationList__ mResponse;
            public CompensationList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetCompensationList_RET_CompensationList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ReceiveCompensationOutMessage : OutMessage
    {
        public ReceiveCompensationOutMessage(IAgentBase sender, int indexType, int type)
            : base(sender, ServiceType.Logic, 1225)
        {
            Request = new __RPC_Logic_ReceiveCompensation_ARG_int32_indexType_int32_type__();
            Request.IndexType=indexType;
            Request.Type=type;

        }

        public __RPC_Logic_ReceiveCompensation_ARG_int32_indexType_int32_type__ Request { get; private set; }

            private __RPC_Logic_ReceiveCompensation_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ReceiveCompensation_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SelectTitleOutMessage : OutMessage
    {
        public SelectTitleOutMessage(IAgentBase sender, int id)
            : base(sender, ServiceType.Logic, 1228)
        {
            Request = new __RPC_Logic_SelectTitle_ARG_int32_id__();
            Request.Id=id;

        }

        public __RPC_Logic_SelectTitle_ARG_int32_id__ Request { get; private set; }

            private __RPC_Logic_SelectTitle_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_SelectTitle_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class NotifyRechargeSuccessOutMessage : OutMessage
    {
        public NotifyRechargeSuccessOutMessage(IAgentBase sender, int rechargeId)
            : base(sender, ServiceType.Logic, 1231)
        {
            Request = new __RPC_Logic_NotifyRechargeSuccess_ARG_int32_rechargeId__();
            Request.RechargeId=rechargeId;

        }

        public __RPC_Logic_NotifyRechargeSuccess_ARG_int32_rechargeId__ Request { get; private set; }


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

    public class RetrainPetOutMessage : OutMessage
    {
        public RetrainPetOutMessage(IAgentBase sender, int petId)
            : base(sender, ServiceType.Logic, 1233)
        {
            Request = new __RPC_Logic_RetrainPet_ARG_int32_petId__();
            Request.PetId=petId;

        }

        public __RPC_Logic_RetrainPet_ARG_int32_petId__ Request { get; private set; }

            private __RPC_Logic_RetrainPet_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RetrainPet_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UpgradeAllianceBuffOutMessage : OutMessage
    {
        public UpgradeAllianceBuffOutMessage(IAgentBase sender, int buffId)
            : base(sender, ServiceType.Logic, 1234)
        {
            Request = new __RPC_Logic_UpgradeAllianceBuff_ARG_int32_buffId__();
            Request.BuffId=buffId;

        }

        public __RPC_Logic_UpgradeAllianceBuff_ARG_int32_buffId__ Request { get; private set; }

            private __RPC_Logic_UpgradeAllianceBuff_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UpgradeAllianceBuff_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class InvestmentOutMessage : OutMessage
    {
        public InvestmentOutMessage(IAgentBase sender, int id)
            : base(sender, ServiceType.Logic, 1237)
        {
            Request = new __RPC_Logic_Investment_ARG_int32_id__();
            Request.Id=id;

        }

        public __RPC_Logic_Investment_ARG_int32_id__ Request { get; private set; }

            private __RPC_Logic_Investment_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_Investment_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GainRewardOutMessage : OutMessage
    {
        public GainRewardOutMessage(IAgentBase sender, int type, int id)
            : base(sender, ServiceType.Logic, 1238)
        {
            Request = new __RPC_Logic_GainReward_ARG_int32_type_int32_id__();
            Request.Type=type;
            Request.Id=id;

        }

        public __RPC_Logic_GainReward_ARG_int32_type_int32_id__ Request { get; private set; }

            private __RPC_Logic_GainReward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_GainReward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class WorshipOutMessage : OutMessage
    {
        public WorshipOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1240)
        {
            Request = new __RPC_Logic_Worship_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_Worship_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_Worship_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_Worship_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UseGiftCodeOutMessage : OutMessage
    {
        public UseGiftCodeOutMessage(IAgentBase sender, string code)
            : base(sender, ServiceType.Logic, 1242)
        {
            Request = new __RPC_Logic_UseGiftCode_ARG_string_code__();
            Request.Code=code;

        }

        public __RPC_Logic_UseGiftCode_ARG_string_code__ Request { get; private set; }

            private __RPC_Logic_UseGiftCode_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UseGiftCode_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyRechargeTablesOutMessage : OutMessage
    {
        public ApplyRechargeTablesOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1243)
        {
            Request = new __RPC_Logic_ApplyRechargeTables_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_ApplyRechargeTables_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_ApplyRechargeTables_RET_RechargeActivityData__ mResponse;
            public RechargeActivityData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyRechargeTables_RET_RechargeActivityData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyFirstChargeItemOutMessage : OutMessage
    {
        public ApplyFirstChargeItemOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1244)
        {
            Request = new __RPC_Logic_ApplyFirstChargeItem_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_ApplyFirstChargeItem_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_ApplyFirstChargeItem_RET_FirstChargeData__ mResponse;
            public FirstChargeData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyFirstChargeItem_RET_FirstChargeData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyGetFirstChargeItemOutMessage : OutMessage
    {
        public ApplyGetFirstChargeItemOutMessage(IAgentBase sender, int index)
            : base(sender, ServiceType.Logic, 1245)
        {
            Request = new __RPC_Logic_ApplyGetFirstChargeItem_ARG_int32_index__();
            Request.Index=index;

        }

        public __RPC_Logic_ApplyGetFirstChargeItem_ARG_int32_index__ Request { get; private set; }

            private __RPC_Logic_ApplyGetFirstChargeItem_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyGetFirstChargeItem_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class TakeMultyExpAwardOutMessage : OutMessage
    {
        public TakeMultyExpAwardOutMessage(IAgentBase sender, int id)
            : base(sender, ServiceType.Logic, 1246)
        {
            Request = new __RPC_Logic_TakeMultyExpAward_ARG_int32_id__();
            Request.Id=id;

        }

        public __RPC_Logic_TakeMultyExpAward_ARG_int32_id__ Request { get; private set; }

            private __RPC_Logic_TakeMultyExpAward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_TakeMultyExpAward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class RandEquipSkillOutMessage : OutMessage
    {
        public RandEquipSkillOutMessage(IAgentBase sender, int bagType, int bagIndex, int itemId)
            : base(sender, ServiceType.Logic, 1247)
        {
            Request = new __RPC_Logic_RandEquipSkill_ARG_int32_bagType_int32_bagIndex_int32_itemId__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;
            Request.ItemId=itemId;

        }

        public __RPC_Logic_RandEquipSkill_ARG_int32_bagType_int32_bagIndex_int32_itemId__ Request { get; private set; }

            private __RPC_Logic_RandEquipSkill_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_RandEquipSkill_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class UseEquipSkillOutMessage : OutMessage
    {
        public UseEquipSkillOutMessage(IAgentBase sender, int bagType, int bagIndex, int type)
            : base(sender, ServiceType.Logic, 1248)
        {
            Request = new __RPC_Logic_UseEquipSkill_ARG_int32_bagType_int32_bagIndex_int32_type__();
            Request.BagType=bagType;
            Request.BagIndex=bagIndex;
            Request.Type=type;

        }

        public __RPC_Logic_UseEquipSkill_ARG_int32_bagType_int32_bagIndex_int32_type__ Request { get; private set; }

            private __RPC_Logic_UseEquipSkill_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_UseEquipSkill_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class GetReviewStateOutMessage : OutMessage
    {
        public GetReviewStateOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1505)
        {
            Request = new __RPC_Logic_GetReviewState_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_GetReviewState_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_GetReviewState_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_GetReviewState_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class OnItemAuctionOutMessage : OutMessage
    {
        public OnItemAuctionOutMessage(IAgentBase sender, int type, int bagId, int bagIndex, int count, int needCount, int storeIndex)
            : base(sender, ServiceType.Logic, 1507)
        {
            Request = new __RPC_Logic_OnItemAuction_ARG_int32_type_int32_bagId_int32_bagIndex_int32_count_int32_needCount_int32_storeIndex__();
            Request.Type=type;
            Request.BagId=bagId;
            Request.BagIndex=bagIndex;
            Request.Count=count;
            Request.NeedCount=needCount;
            Request.StoreIndex=storeIndex;

        }

        public __RPC_Logic_OnItemAuction_ARG_int32_type_int32_bagId_int32_bagIndex_int32_count_int32_needCount_int32_storeIndex__ Request { get; private set; }

            private __RPC_Logic_OnItemAuction_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_OnItemAuction_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class BuyItemAuctionOutMessage : OutMessage
    {
        public BuyItemAuctionOutMessage(IAgentBase sender, ulong characterId, long guid, long managerId)
            : base(sender, ServiceType.Logic, 1508)
        {
            Request = new __RPC_Logic_BuyItemAuction_ARG_uint64_characterId_int64_guid_int64_managerId__();
            Request.CharacterId=characterId;
            Request.Guid=guid;
            Request.ManagerId=managerId;

        }

        public __RPC_Logic_BuyItemAuction_ARG_uint64_characterId_int64_guid_int64_managerId__ Request { get; private set; }

            private __RPC_Logic_BuyItemAuction_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_BuyItemAuction_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplySellHistoryOutMessage : OutMessage
    {
        public ApplySellHistoryOutMessage(IAgentBase sender, int type)
            : base(sender, ServiceType.Logic, 1510)
        {
            Request = new __RPC_Logic_ApplySellHistory_ARG_int32_type__();
            Request.Type=type;

        }

        public __RPC_Logic_ApplySellHistory_ARG_int32_type__ Request { get; private set; }

            private __RPC_Logic_ApplySellHistory_RET_SellHistoryList__ mResponse;
            public SellHistoryList Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplySellHistory_RET_SellHistoryList__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class DrawWishItemOutMessage : OutMessage
    {
        public DrawWishItemOutMessage(IAgentBase sender, Int32Array param)
            : base(sender, ServiceType.Logic, 1511)
        {
            Request = new __RPC_Logic_DrawWishItem_ARG_Int32Array_param__();
            Request.Param=param;

        }

        public __RPC_Logic_DrawWishItem_ARG_Int32Array_param__ Request { get; private set; }

            private __RPC_Logic_DrawWishItem_RET_DrawWishItemResult__ mResponse;
            public DrawWishItemResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_DrawWishItem_RET_DrawWishItemResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyOperationActivityOutMessage : OutMessage
    {
        public ApplyOperationActivityOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Logic, 1512)
        {
            Request = new __RPC_Logic_ApplyOperationActivity_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Logic_ApplyOperationActivity_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Logic_ApplyOperationActivity_RET_MsgOperActivty__ mResponse;
            public MsgOperActivty Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyOperationActivity_RET_MsgOperActivty__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ClaimOperationRewardOutMessage : OutMessage
    {
        public ClaimOperationRewardOutMessage(IAgentBase sender, int type, int Id)
            : base(sender, ServiceType.Logic, 1513)
        {
            Request = new __RPC_Logic_ClaimOperationReward_ARG_int32_type_int32_Id__();
            Request.Type=type;
            Request.Id=Id;

        }

        public __RPC_Logic_ClaimOperationReward_ARG_int32_type_int32_Id__ Request { get; private set; }

            private __RPC_Logic_ClaimOperationReward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ClaimOperationReward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncOperationActivityItemOutMessage : OutMessage
    {
        public SyncOperationActivityItemOutMessage(IAgentBase sender, MsgOperActivtyItemList items)
            : base(sender, ServiceType.Logic, 1514)
        {
            Request = new __RPC_Logic_SyncOperationActivityItem_ARG_MsgOperActivtyItemList_items__();
            Request.Items=items;

        }

        public __RPC_Logic_SyncOperationActivityItem_ARG_MsgOperActivtyItemList_items__ Request { get; private set; }


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

    public class ApplyPromoteHPOutMessage : OutMessage
    {
        public ApplyPromoteHPOutMessage(IAgentBase sender, int serverId, int activityId, int batteryId, int promoteType)
            : base(sender, ServiceType.Logic, 1520)
        {
            Request = new __RPC_Logic_ApplyPromoteHP_ARG_int32_serverId_int32_activityId_int32_batteryId_int32_promoteType__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;
            Request.BatteryId=batteryId;
            Request.PromoteType=promoteType;

        }

        public __RPC_Logic_ApplyPromoteHP_ARG_int32_serverId_int32_activityId_int32_batteryId_int32_promoteType__ Request { get; private set; }

            private __RPC_Logic_ApplyPromoteHP_RET_BatteryUpdateData__ mResponse;
            public BatteryUpdateData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyPromoteHP_RET_BatteryUpdateData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPromoteSkillOutMessage : OutMessage
    {
        public ApplyPromoteSkillOutMessage(IAgentBase sender, int serverId, int activityId, int batteryId, ulong batteryGuid, int promoteType)
            : base(sender, ServiceType.Logic, 1521)
        {
            Request = new __RPC_Logic_ApplyPromoteSkill_ARG_int32_serverId_int32_activityId_int32_batteryId_uint64_batteryGuid_int32_promoteType__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;
            Request.BatteryId=batteryId;
            Request.BatteryGuid=batteryGuid;
            Request.PromoteType=promoteType;

        }

        public __RPC_Logic_ApplyPromoteSkill_ARG_int32_serverId_int32_activityId_int32_batteryId_uint64_batteryGuid_int32_promoteType__ Request { get; private set; }

            private __RPC_Logic_ApplyPromoteSkill_RET_BatteryUpdateData__ mResponse;
            public BatteryUpdateData Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyPromoteSkill_RET_BatteryUpdateData__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPickUpBoxOutMessage : OutMessage
    {
        public ApplyPickUpBoxOutMessage(IAgentBase sender, int serverId, int activityId, int npcId)
            : base(sender, ServiceType.Logic, 1522)
        {
            Request = new __RPC_Logic_ApplyPickUpBox_ARG_int32_serverId_int32_activityId_int32_npcId__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;
            Request.NpcId=npcId;

        }

        public __RPC_Logic_ApplyPickUpBox_ARG_int32_serverId_int32_activityId_int32_npcId__ Request { get; private set; }

            private __RPC_Logic_ApplyPickUpBox_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyPickUpBox_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyJoinActivityOutMessage : OutMessage
    {
        public ApplyJoinActivityOutMessage(IAgentBase sender, int serverId, int activityId)
            : base(sender, ServiceType.Logic, 1523)
        {
            Request = new __RPC_Logic_ApplyJoinActivity_ARG_int32_serverId_int32_activityId__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;

        }

        public __RPC_Logic_ApplyJoinActivity_ARG_int32_serverId_int32_activityId__ Request { get; private set; }

            private __RPC_Logic_ApplyJoinActivity_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyJoinActivity_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ApplyPortraitAwardOutMessage : OutMessage
    {
        public ApplyPortraitAwardOutMessage(IAgentBase sender, int serverId)
            : base(sender, ServiceType.Logic, 1524)
        {
            Request = new __RPC_Logic_ApplyPortraitAward_ARG_int32_serverId__();
            Request.ServerId=serverId;

        }

        public __RPC_Logic_ApplyPortraitAward_ARG_int32_serverId__ Request { get; private set; }

            private __RPC_Logic_ApplyPortraitAward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyPortraitAward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SyncOperationActivityTermOutMessage : OutMessage
    {
        public SyncOperationActivityTermOutMessage(IAgentBase sender, int id, int param)
            : base(sender, ServiceType.Logic, 1532)
        {
            Request = new __RPC_Logic_SyncOperationActivityTerm_ARG_int32_id_int32_param__();
            Request.Id=id;
            Request.Param=param;

        }

        public __RPC_Logic_SyncOperationActivityTerm_ARG_int32_id_int32_param__ Request { get; private set; }


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

    public class ApplyGetTowerRewardOutMessage : OutMessage
    {
        public ApplyGetTowerRewardOutMessage(IAgentBase sender, int serverId, int activityId, int idx)
            : base(sender, ServiceType.Logic, 1533)
        {
            Request = new __RPC_Logic_ApplyGetTowerReward_ARG_int32_serverId_int32_activityId_int32_idx__();
            Request.ServerId=serverId;
            Request.ActivityId=activityId;
            Request.Idx=idx;

        }

        public __RPC_Logic_ApplyGetTowerReward_ARG_int32_serverId_int32_activityId_int32_idx__ Request { get; private set; }

            private __RPC_Logic_ApplyGetTowerReward_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ApplyGetTowerReward_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class SendJsonDataOutMessage : OutMessage
    {
        public SendJsonDataOutMessage(IAgentBase sender, string json)
            : base(sender, ServiceType.Logic, 1600)
        {
            Request = new __RPC_Logic_SendJsonData_ARG_string_json__();
            Request.Json=json;

        }

        public __RPC_Logic_SendJsonData_ARG_string_json__ Request { get; private set; }

            private __RPC_Logic_SendJsonData_RET_string__ mResponse;
            public string Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_SendJsonData_RET_string__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class BuyWingChargeOutMessage : OutMessage
    {
        public BuyWingChargeOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1601)
        {
            Request = new __RPC_Logic_BuyWingCharge_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_BuyWingCharge_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_BuyWingCharge_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_BuyWingCharge_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class PetIsLandBuyTiliOutMessage : OutMessage
    {
        public PetIsLandBuyTiliOutMessage(IAgentBase sender, int placeholder)
            : base(sender, ServiceType.Logic, 1602)
        {
            Request = new __RPC_Logic_PetIsLandBuyTili_ARG_int32_placeholder__();
            Request.Placeholder=placeholder;

        }

        public __RPC_Logic_PetIsLandBuyTili_ARG_int32_placeholder__ Request { get; private set; }

            private __RPC_Logic_PetIsLandBuyTili_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_PetIsLandBuyTili_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class ClientErrorMessageOutMessage : OutMessage
    {
        public ClientErrorMessageOutMessage(IAgentBase sender, int errorType, string errorMsg)
            : base(sender, ServiceType.Logic, 1999)
        {
            Request = new __RPC_Logic_ClientErrorMessage_ARG_int32_errorType_string_errorMsg__();
            Request.ErrorType=errorType;
            Request.ErrorMsg=errorMsg;

        }

        public __RPC_Logic_ClientErrorMessage_ARG_int32_errorType_string_errorMsg__ Request { get; private set; }

            private __RPC_Logic_ClientErrorMessage_RET_int32__ mResponse;
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
                mResponse = Serializer.Deserialize<__RPC_Logic_ClientErrorMessage_RET_int32__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

    public class TowerSweepOutMessage : OutMessage
    {
        public TowerSweepOutMessage(IAgentBase sender, int param)
            : base(sender, ServiceType.Logic, 1314)
        {
            Request = new __RPC_Logic_TowerSweep_ARG_int32_param__();
            Request.Param=param;

        }

        public __RPC_Logic_TowerSweep_ARG_int32_param__ Request { get; private set; }

            private __RPC_Logic_TowerSweep_RET_TowerSweepResult__ mResponse;
            public TowerSweepResult Response { get { return mResponse.ReturnValue; } }

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
                mResponse = Serializer.Deserialize<__RPC_Logic_TowerSweep_RET_TowerSweepResult__>(ms);
            }
            State = MessageState.Reply;
            ErrorCode = (int) error;
        }
        public override bool HasReturnValue { get { return true; } }
    }

}
