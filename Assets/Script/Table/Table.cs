//----------------------------------------------
//--------------以下需要自动生成代码-------------
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using ClientDataModel;
using Assets.Script.Utility;
using UnityEngine;
using CLRSharp;
namespace DataTable
{
    public enum TableType
    {
        Icon,
        Sound,
        ConditionTable,
        Exdata,
        Dictionary,
        SceneNpc,
        CharModel,
        Animation,
        Skill,
        Scene,
        CharacterBase,
        EquipBase,
        Actor,
        Talent,
        BagBase,
        ItemBase,
        ItemType,
        ColorBase,
        Buff,
        MissionBase,
        AttrRef,
        EquipRelate,
        EquipEnchant,
        EquipEnchantChance,
        Title,
        EquipEnchance,
        LevelData,
        Bullet,
        NpcBase,
        SkillUpgrading,
        Achievement,
        EquipTie,
        Effect,
        Transfer,
        ClientConfig,
        WeaponMount,
        CombatText,
        RandName,
        OperationList,
        UI,
        Gift,
        EquipBlessing,
        EquipAdditional,
        EquipExcellent,
        EquipModelView,
        Talk,
        ChatInfo,
        HandBook,
        BookGroup,
        ItemCompose,
        Camp,
        DropModel,
        Fuben,
        Stats,
        PlotFuben,
        Store,
        Story,
        Building,
        BuildingRes,
        BuildingRule,
        BuildingService,
        HomeSence,
        Pet,
        PetSkill,
        Service,
        StoreType,
        Elf,
        ElfGroup,
        Queue,
        Draw,
        Plant,
        SeqFrame,
        Medal,
        Sailing,
        WingTrain,
        WingQuality,
        PVPRule,
        ArenaReward,
        ArenaLevel,
        Honor,
        JJCRoot,
        Statue,
        EquipAdditional1,
        TriggerArea,
        Guild,
        GuildBuff,
        GuildBoss,
        GuildAccess,
        ExpInfo,
        GroupShop,
        PKMode,
        forged,
        EquipUpdate,
        GuildMission,
        OrderForm,
        OrderUpdate,
        Trade,
        Gem,
        GemGroup,
        SensitiveWord,
        Guidance,
        MapTransfer,
        SceneEffect,
        PVPBattle,
        StepByStep,
        WorldBOSS,
        PKValue,
        Transmigration,
        AttachPoint,
        FubenInfo,
        FubenLogic,
        Face,
        ServerName,
        LoadingTest,
        GetMissionInfo,
        MissionConditionInfo,
        GetMissionReward,
        GetMissionIcon,
        Subject,
        ItemGetInfo,
        DynamicActivity,
        Compensation,
        CityTalk,
        DailyActivity,
        Recharge,
        RewardInfo,
        NameTitle,
        VIP,
        LevelupTips,
        StrongType,
        StrongData,
        Mail,
        GMCommand,
        AuctionType1,
        AuctionType2,
        AuctionType3,
        FirstRecharge,
        MieShi,
        MieShiPublic,
        DefendCityReward,
        DefendCityDevoteReward,
        BatteryLevel,
        BatteryBase,
        MieShiFighting,
        FunctionOn,
        BangBuff,
        BuffGroup,
        MieshiTowerReward,
        ClimbingTower,
        AcientBattleField,
        ElfStarShader,
        ConsumArray,
    }
    public static class Table
    {
        public static Func<IRecord> NewTableRecord(TableType type)
        {
            switch (type)
            {
                case TableType.Icon: return () => new IconRecord();
                case TableType.Sound: return () => new SoundRecord();
                case TableType.ConditionTable: return () => new ConditionTableRecord();
                case TableType.Exdata: return () => new ExdataRecord();
                case TableType.Dictionary: return () => new DictionaryRecord();
                case TableType.SceneNpc: return () => new SceneNpcRecord();
                case TableType.CharModel: return () => new CharModelRecord();
                case TableType.Animation: return () => new AnimationRecord();
                case TableType.Skill: return () => new SkillRecord();
                case TableType.Scene: return () => new SceneRecord();
                case TableType.CharacterBase: return () => new CharacterBaseRecord();
                case TableType.EquipBase: return () => new EquipBaseRecord();
                case TableType.Actor: return () => new ActorRecord();
                case TableType.Talent: return () => new TalentRecord();
                case TableType.BagBase: return () => new BagBaseRecord();
                case TableType.ItemBase: return () => new ItemBaseRecord();
                case TableType.ItemType: return () => new ItemTypeRecord();
                case TableType.ColorBase: return () => new ColorBaseRecord();
                case TableType.Buff: return () => new BuffRecord();
                case TableType.MissionBase: return () => new MissionBaseRecord();
                case TableType.AttrRef: return () => new AttrRefRecord();
                case TableType.EquipRelate: return () => new EquipRelateRecord();
                case TableType.EquipEnchant: return () => new EquipEnchantRecord();
                case TableType.EquipEnchantChance: return () => new EquipEnchantChanceRecord();
                case TableType.Title: return () => new TitleRecord();
                case TableType.EquipEnchance: return () => new EquipEnchanceRecord();
                case TableType.LevelData: return () => new LevelDataRecord();
                case TableType.Bullet: return () => new BulletRecord();
                case TableType.NpcBase: return () => new NpcBaseRecord();
                case TableType.SkillUpgrading: return () => new SkillUpgradingRecord();
                case TableType.Achievement: return () => new AchievementRecord();
                case TableType.EquipTie: return () => new EquipTieRecord();
                case TableType.Effect: return () => new EffectRecord();
                case TableType.Transfer: return () => new TransferRecord();
                case TableType.ClientConfig: return () => new ClientConfigRecord();
                case TableType.WeaponMount: return () => new WeaponMountRecord();
                case TableType.CombatText: return () => new CombatTextRecord();
                case TableType.RandName: return () => new RandNameRecord();
                case TableType.OperationList: return () => new OperationListRecord();
                case TableType.UI: return () => new UIRecord();
                case TableType.Gift: return () => new GiftRecord();
                case TableType.EquipBlessing: return () => new EquipBlessingRecord();
                case TableType.EquipAdditional: return () => new EquipAdditionalRecord();
                case TableType.EquipExcellent: return () => new EquipExcellentRecord();
                case TableType.EquipModelView: return () => new EquipModelViewRecord();
                case TableType.Talk: return () => new TalkRecord();
                case TableType.ChatInfo: return () => new ChatInfoRecord();
                case TableType.HandBook: return () => new HandBookRecord();
                case TableType.BookGroup: return () => new BookGroupRecord();
                case TableType.ItemCompose: return () => new ItemComposeRecord();
                case TableType.Camp: return () => new CampRecord();
                case TableType.DropModel: return () => new DropModelRecord();
                case TableType.Fuben: return () => new FubenRecord();
                case TableType.Stats: return () => new StatsRecord();
                case TableType.PlotFuben: return () => new PlotFubenRecord();
                case TableType.Store: return () => new StoreRecord();
                case TableType.Story: return () => new StoryRecord();
                case TableType.Building: return () => new BuildingRecord();
                case TableType.BuildingRes: return () => new BuildingResRecord();
                case TableType.BuildingRule: return () => new BuildingRuleRecord();
                case TableType.BuildingService: return () => new BuildingServiceRecord();
                case TableType.HomeSence: return () => new HomeSenceRecord();
                case TableType.Pet: return () => new PetRecord();
                case TableType.PetSkill: return () => new PetSkillRecord();
                case TableType.Service: return () => new ServiceRecord();
                case TableType.StoreType: return () => new StoreTypeRecord();
                case TableType.Elf: return () => new ElfRecord();
                case TableType.ElfGroup: return () => new ElfGroupRecord();
                case TableType.Queue: return () => new QueueRecord();
                case TableType.Draw: return () => new DrawRecord();
                case TableType.Plant: return () => new PlantRecord();
                case TableType.SeqFrame: return () => new SeqFrameRecord();
                case TableType.Medal: return () => new MedalRecord();
                case TableType.Sailing: return () => new SailingRecord();
                case TableType.WingTrain: return () => new WingTrainRecord();
                case TableType.WingQuality: return () => new WingQualityRecord();
                case TableType.PVPRule: return () => new PVPRuleRecord();
                case TableType.ArenaReward: return () => new ArenaRewardRecord();
                case TableType.ArenaLevel: return () => new ArenaLevelRecord();
                case TableType.Honor: return () => new HonorRecord();
                case TableType.JJCRoot: return () => new JJCRootRecord();
                case TableType.Statue: return () => new StatueRecord();
                case TableType.EquipAdditional1: return () => new EquipAdditional1Record();
                case TableType.TriggerArea: return () => new TriggerAreaRecord();
                case TableType.Guild: return () => new GuildRecord();
                case TableType.GuildBuff: return () => new GuildBuffRecord();
                case TableType.GuildBoss: return () => new GuildBossRecord();
                case TableType.GuildAccess: return () => new GuildAccessRecord();
                case TableType.ExpInfo: return () => new ExpInfoRecord();
                case TableType.GroupShop: return () => new GroupShopRecord();
                case TableType.PKMode: return () => new PKModeRecord();
                case TableType.forged: return () => new forgedRecord();
                case TableType.EquipUpdate: return () => new EquipUpdateRecord();
                case TableType.GuildMission: return () => new GuildMissionRecord();
                case TableType.OrderForm: return () => new OrderFormRecord();
                case TableType.OrderUpdate: return () => new OrderUpdateRecord();
                case TableType.Trade: return () => new TradeRecord();
                case TableType.Gem: return () => new GemRecord();
                case TableType.GemGroup: return () => new GemGroupRecord();
                case TableType.SensitiveWord: return () => new SensitiveWordRecord();
                case TableType.Guidance: return () => new GuidanceRecord();
                case TableType.MapTransfer: return () => new MapTransferRecord();
                case TableType.SceneEffect: return () => new SceneEffectRecord();
                case TableType.PVPBattle: return () => new PVPBattleRecord();
                case TableType.StepByStep: return () => new StepByStepRecord();
                case TableType.WorldBOSS: return () => new WorldBOSSRecord();
                case TableType.PKValue: return () => new PKValueRecord();
                case TableType.Transmigration: return () => new TransmigrationRecord();
                case TableType.AttachPoint: return () => new AttachPointRecord();
                case TableType.FubenInfo: return () => new FubenInfoRecord();
                case TableType.FubenLogic: return () => new FubenLogicRecord();
                case TableType.Face: return () => new FaceRecord();
                case TableType.ServerName: return () => new ServerNameRecord();
                case TableType.LoadingTest: return () => new LoadingTestRecord();
                case TableType.GetMissionInfo: return () => new GetMissionInfoRecord();
                case TableType.MissionConditionInfo: return () => new MissionConditionInfoRecord();
                case TableType.GetMissionReward: return () => new GetMissionRewardRecord();
                case TableType.GetMissionIcon: return () => new GetMissionIconRecord();
                case TableType.Subject: return () => new SubjectRecord();
                case TableType.ItemGetInfo: return () => new ItemGetInfoRecord();
                case TableType.DynamicActivity: return () => new DynamicActivityRecord();
                case TableType.Compensation: return () => new CompensationRecord();
                case TableType.CityTalk: return () => new CityTalkRecord();
                case TableType.DailyActivity: return () => new DailyActivityRecord();
                case TableType.Recharge: return () => new RechargeRecord();
                case TableType.RewardInfo: return () => new RewardInfoRecord();
                case TableType.NameTitle: return () => new NameTitleRecord();
                case TableType.VIP: return () => new VIPRecord();
                case TableType.LevelupTips: return () => new LevelupTipsRecord();
                case TableType.StrongType: return () => new StrongTypeRecord();
                case TableType.StrongData: return () => new StrongDataRecord();
                case TableType.Mail: return () => new MailRecord();
                case TableType.GMCommand: return () => new GMCommandRecord();
                case TableType.AuctionType1: return () => new AuctionType1Record();
                case TableType.AuctionType2: return () => new AuctionType2Record();
                case TableType.AuctionType3: return () => new AuctionType3Record();
                case TableType.FirstRecharge: return () => new FirstRechargeRecord();
                case TableType.MieShi: return () => new MieShiRecord();
                case TableType.MieShiPublic: return () => new MieShiPublicRecord();
                case TableType.DefendCityReward: return () => new DefendCityRewardRecord();
                case TableType.DefendCityDevoteReward: return () => new DefendCityDevoteRewardRecord();
                case TableType.BatteryLevel: return () => new BatteryLevelRecord();
                case TableType.BatteryBase: return () => new BatteryBaseRecord();
                case TableType.MieShiFighting: return () => new MieShiFightingRecord();
                case TableType.FunctionOn: return () => new FunctionOnRecord();
                case TableType.BangBuff: return () => new BangBuffRecord();
                case TableType.BuffGroup: return () => new BuffGroupRecord();
                case TableType.MieshiTowerReward: return () => new MieshiTowerRewardRecord();
                case TableType.ClimbingTower: return () => new ClimbingTowerRecord();
                case TableType.AcientBattleField: return () => new AcientBattleFieldRecord();
                case TableType.ElfStarShader: return () => new ElfStarShaderRecord();
                case TableType.ConsumArray: return () => new ConsumArrayRecord();
            }
            return null;
        }
        private static Dictionary<string, object> mTables = new Dictionary<string, object>();
        private static Dictionary<int, IRecord> Icon = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Sound = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ConditionTable = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Exdata = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Dictionary = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> SceneNpc = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> CharModel = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Animation = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Skill = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Scene = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> CharacterBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Actor = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Talent = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BagBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ItemBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ItemType = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ColorBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Buff = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MissionBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> AttrRef = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipRelate = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipEnchant = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipEnchantChance = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Title = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipEnchance = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> LevelData = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Bullet = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> NpcBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> SkillUpgrading = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Achievement = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipTie = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Effect = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Transfer = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ClientConfig = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> WeaponMount = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> CombatText = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> RandName = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> OperationList = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> UI = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Gift = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipBlessing = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipAdditional = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipExcellent = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipModelView = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Talk = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ChatInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> HandBook = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BookGroup = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ItemCompose = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Camp = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> DropModel = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Fuben = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Stats = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> PlotFuben = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Store = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Story = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Building = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BuildingRes = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BuildingRule = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BuildingService = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> HomeSence = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Pet = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> PetSkill = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Service = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> StoreType = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Elf = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ElfGroup = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Queue = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Draw = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Plant = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> SeqFrame = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Medal = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Sailing = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> WingTrain = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> WingQuality = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> PVPRule = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ArenaReward = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ArenaLevel = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Honor = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> JJCRoot = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Statue = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipAdditional1 = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> TriggerArea = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Guild = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GuildBuff = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GuildBoss = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GuildAccess = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ExpInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GroupShop = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> PKMode = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> forged = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> EquipUpdate = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GuildMission = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> OrderForm = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> OrderUpdate = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Trade = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Gem = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GemGroup = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> SensitiveWord = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Guidance = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MapTransfer = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> SceneEffect = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> PVPBattle = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> StepByStep = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> WorldBOSS = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> PKValue = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Transmigration = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> AttachPoint = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> FubenInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> FubenLogic = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Face = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ServerName = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> LoadingTest = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GetMissionInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MissionConditionInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GetMissionReward = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GetMissionIcon = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Subject = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ItemGetInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> DynamicActivity = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Compensation = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> CityTalk = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> DailyActivity = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Recharge = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> RewardInfo = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> NameTitle = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> VIP = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> LevelupTips = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> StrongType = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> StrongData = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> Mail = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> GMCommand = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> AuctionType1 = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> AuctionType2 = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> AuctionType3 = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> FirstRecharge = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MieShi = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MieShiPublic = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> DefendCityReward = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> DefendCityDevoteReward = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BatteryLevel = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BatteryBase = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MieShiFighting = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> FunctionOn = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BangBuff = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> BuffGroup = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> MieshiTowerReward = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ClimbingTower = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> AcientBattleField = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ElfStarShader = new Dictionary<int, IRecord>();
        private static Dictionary<int, IRecord> ConsumArray = new Dictionary<int, IRecord>();
        static Table()
        {
            mTables["Icon"] = Icon;
            Delegate_Binder.RegBind(typeof(Func<IconRecord, bool>), new Delegate_BindTool_Ret<IconRecord, bool>());
            mTables["Sound"] = Sound;
            Delegate_Binder.RegBind(typeof(Func<SoundRecord, bool>), new Delegate_BindTool_Ret<SoundRecord, bool>());
            mTables["ConditionTable"] = ConditionTable;
            Delegate_Binder.RegBind(typeof(Func<ConditionTableRecord, bool>), new Delegate_BindTool_Ret<ConditionTableRecord, bool>());
            mTables["Exdata"] = Exdata;
            Delegate_Binder.RegBind(typeof(Func<ExdataRecord, bool>), new Delegate_BindTool_Ret<ExdataRecord, bool>());
            mTables["Dictionary"] = Dictionary;
            Delegate_Binder.RegBind(typeof(Func<DictionaryRecord, bool>), new Delegate_BindTool_Ret<DictionaryRecord, bool>());
            mTables["SceneNpc"] = SceneNpc;
            Delegate_Binder.RegBind(typeof(Func<SceneNpcRecord, bool>), new Delegate_BindTool_Ret<SceneNpcRecord, bool>());
            mTables["CharModel"] = CharModel;
            Delegate_Binder.RegBind(typeof(Func<CharModelRecord, bool>), new Delegate_BindTool_Ret<CharModelRecord, bool>());
            mTables["Animation"] = Animation;
            Delegate_Binder.RegBind(typeof(Func<AnimationRecord, bool>), new Delegate_BindTool_Ret<AnimationRecord, bool>());
            mTables["Skill"] = Skill;
            Delegate_Binder.RegBind(typeof(Func<SkillRecord, bool>), new Delegate_BindTool_Ret<SkillRecord, bool>());
            mTables["Scene"] = Scene;
            Delegate_Binder.RegBind(typeof(Func<SceneRecord, bool>), new Delegate_BindTool_Ret<SceneRecord, bool>());
            mTables["CharacterBase"] = CharacterBase;
            Delegate_Binder.RegBind(typeof(Func<CharacterBaseRecord, bool>), new Delegate_BindTool_Ret<CharacterBaseRecord, bool>());
            mTables["EquipBase"] = EquipBase;
            Delegate_Binder.RegBind(typeof(Func<EquipBaseRecord, bool>), new Delegate_BindTool_Ret<EquipBaseRecord, bool>());
            mTables["Actor"] = Actor;
            Delegate_Binder.RegBind(typeof(Func<ActorRecord, bool>), new Delegate_BindTool_Ret<ActorRecord, bool>());
            mTables["Talent"] = Talent;
            Delegate_Binder.RegBind(typeof(Func<TalentRecord, bool>), new Delegate_BindTool_Ret<TalentRecord, bool>());
            mTables["BagBase"] = BagBase;
            Delegate_Binder.RegBind(typeof(Func<BagBaseRecord, bool>), new Delegate_BindTool_Ret<BagBaseRecord, bool>());
            mTables["ItemBase"] = ItemBase;
            Delegate_Binder.RegBind(typeof(Func<ItemBaseRecord, bool>), new Delegate_BindTool_Ret<ItemBaseRecord, bool>());
            mTables["ItemType"] = ItemType;
            Delegate_Binder.RegBind(typeof(Func<ItemTypeRecord, bool>), new Delegate_BindTool_Ret<ItemTypeRecord, bool>());
            mTables["ColorBase"] = ColorBase;
            Delegate_Binder.RegBind(typeof(Func<ColorBaseRecord, bool>), new Delegate_BindTool_Ret<ColorBaseRecord, bool>());
            mTables["Buff"] = Buff;
            Delegate_Binder.RegBind(typeof(Func<BuffRecord, bool>), new Delegate_BindTool_Ret<BuffRecord, bool>());
            mTables["MissionBase"] = MissionBase;
            Delegate_Binder.RegBind(typeof(Func<MissionBaseRecord, bool>), new Delegate_BindTool_Ret<MissionBaseRecord, bool>());
            mTables["AttrRef"] = AttrRef;
            Delegate_Binder.RegBind(typeof(Func<AttrRefRecord, bool>), new Delegate_BindTool_Ret<AttrRefRecord, bool>());
            mTables["EquipRelate"] = EquipRelate;
            Delegate_Binder.RegBind(typeof(Func<EquipRelateRecord, bool>), new Delegate_BindTool_Ret<EquipRelateRecord, bool>());
            mTables["EquipEnchant"] = EquipEnchant;
            Delegate_Binder.RegBind(typeof(Func<EquipEnchantRecord, bool>), new Delegate_BindTool_Ret<EquipEnchantRecord, bool>());
            mTables["EquipEnchantChance"] = EquipEnchantChance;
            Delegate_Binder.RegBind(typeof(Func<EquipEnchantChanceRecord, bool>), new Delegate_BindTool_Ret<EquipEnchantChanceRecord, bool>());
            mTables["Title"] = Title;
            Delegate_Binder.RegBind(typeof(Func<TitleRecord, bool>), new Delegate_BindTool_Ret<TitleRecord, bool>());
            mTables["EquipEnchance"] = EquipEnchance;
            Delegate_Binder.RegBind(typeof(Func<EquipEnchanceRecord, bool>), new Delegate_BindTool_Ret<EquipEnchanceRecord, bool>());
            mTables["LevelData"] = LevelData;
            Delegate_Binder.RegBind(typeof(Func<LevelDataRecord, bool>), new Delegate_BindTool_Ret<LevelDataRecord, bool>());
            mTables["Bullet"] = Bullet;
            Delegate_Binder.RegBind(typeof(Func<BulletRecord, bool>), new Delegate_BindTool_Ret<BulletRecord, bool>());
            mTables["NpcBase"] = NpcBase;
            Delegate_Binder.RegBind(typeof(Func<NpcBaseRecord, bool>), new Delegate_BindTool_Ret<NpcBaseRecord, bool>());
            mTables["SkillUpgrading"] = SkillUpgrading;
            Delegate_Binder.RegBind(typeof(Func<SkillUpgradingRecord, bool>), new Delegate_BindTool_Ret<SkillUpgradingRecord, bool>());
            mTables["Achievement"] = Achievement;
            Delegate_Binder.RegBind(typeof(Func<AchievementRecord, bool>), new Delegate_BindTool_Ret<AchievementRecord, bool>());
            mTables["EquipTie"] = EquipTie;
            Delegate_Binder.RegBind(typeof(Func<EquipTieRecord, bool>), new Delegate_BindTool_Ret<EquipTieRecord, bool>());
            mTables["Effect"] = Effect;
            Delegate_Binder.RegBind(typeof(Func<EffectRecord, bool>), new Delegate_BindTool_Ret<EffectRecord, bool>());
            mTables["Transfer"] = Transfer;
            Delegate_Binder.RegBind(typeof(Func<TransferRecord, bool>), new Delegate_BindTool_Ret<TransferRecord, bool>());
            mTables["ClientConfig"] = ClientConfig;
            Delegate_Binder.RegBind(typeof(Func<ClientConfigRecord, bool>), new Delegate_BindTool_Ret<ClientConfigRecord, bool>());
            mTables["WeaponMount"] = WeaponMount;
            Delegate_Binder.RegBind(typeof(Func<WeaponMountRecord, bool>), new Delegate_BindTool_Ret<WeaponMountRecord, bool>());
            mTables["CombatText"] = CombatText;
            Delegate_Binder.RegBind(typeof(Func<CombatTextRecord, bool>), new Delegate_BindTool_Ret<CombatTextRecord, bool>());
            mTables["RandName"] = RandName;
            Delegate_Binder.RegBind(typeof(Func<RandNameRecord, bool>), new Delegate_BindTool_Ret<RandNameRecord, bool>());
            mTables["OperationList"] = OperationList;
            Delegate_Binder.RegBind(typeof(Func<OperationListRecord, bool>), new Delegate_BindTool_Ret<OperationListRecord, bool>());
            mTables["UI"] = UI;
            Delegate_Binder.RegBind(typeof(Func<UIRecord, bool>), new Delegate_BindTool_Ret<UIRecord, bool>());
            mTables["Gift"] = Gift;
            Delegate_Binder.RegBind(typeof(Func<GiftRecord, bool>), new Delegate_BindTool_Ret<GiftRecord, bool>());
            mTables["EquipBlessing"] = EquipBlessing;
            Delegate_Binder.RegBind(typeof(Func<EquipBlessingRecord, bool>), new Delegate_BindTool_Ret<EquipBlessingRecord, bool>());
            mTables["EquipAdditional"] = EquipAdditional;
            Delegate_Binder.RegBind(typeof(Func<EquipAdditionalRecord, bool>), new Delegate_BindTool_Ret<EquipAdditionalRecord, bool>());
            mTables["EquipExcellent"] = EquipExcellent;
            Delegate_Binder.RegBind(typeof(Func<EquipExcellentRecord, bool>), new Delegate_BindTool_Ret<EquipExcellentRecord, bool>());
            mTables["EquipModelView"] = EquipModelView;
            Delegate_Binder.RegBind(typeof(Func<EquipModelViewRecord, bool>), new Delegate_BindTool_Ret<EquipModelViewRecord, bool>());
            mTables["Talk"] = Talk;
            Delegate_Binder.RegBind(typeof(Func<TalkRecord, bool>), new Delegate_BindTool_Ret<TalkRecord, bool>());
            mTables["ChatInfo"] = ChatInfo;
            Delegate_Binder.RegBind(typeof(Func<ChatInfoRecord, bool>), new Delegate_BindTool_Ret<ChatInfoRecord, bool>());
            mTables["HandBook"] = HandBook;
            Delegate_Binder.RegBind(typeof(Func<HandBookRecord, bool>), new Delegate_BindTool_Ret<HandBookRecord, bool>());
            mTables["BookGroup"] = BookGroup;
            Delegate_Binder.RegBind(typeof(Func<BookGroupRecord, bool>), new Delegate_BindTool_Ret<BookGroupRecord, bool>());
            mTables["ItemCompose"] = ItemCompose;
            Delegate_Binder.RegBind(typeof(Func<ItemComposeRecord, bool>), new Delegate_BindTool_Ret<ItemComposeRecord, bool>());
            mTables["Camp"] = Camp;
            Delegate_Binder.RegBind(typeof(Func<CampRecord, bool>), new Delegate_BindTool_Ret<CampRecord, bool>());
            mTables["DropModel"] = DropModel;
            Delegate_Binder.RegBind(typeof(Func<DropModelRecord, bool>), new Delegate_BindTool_Ret<DropModelRecord, bool>());
            mTables["Fuben"] = Fuben;
            Delegate_Binder.RegBind(typeof(Func<FubenRecord, bool>), new Delegate_BindTool_Ret<FubenRecord, bool>());
            mTables["Stats"] = Stats;
            Delegate_Binder.RegBind(typeof(Func<StatsRecord, bool>), new Delegate_BindTool_Ret<StatsRecord, bool>());
            mTables["PlotFuben"] = PlotFuben;
            Delegate_Binder.RegBind(typeof(Func<PlotFubenRecord, bool>), new Delegate_BindTool_Ret<PlotFubenRecord, bool>());
            mTables["Store"] = Store;
            Delegate_Binder.RegBind(typeof(Func<StoreRecord, bool>), new Delegate_BindTool_Ret<StoreRecord, bool>());
            mTables["Story"] = Story;
            Delegate_Binder.RegBind(typeof(Func<StoryRecord, bool>), new Delegate_BindTool_Ret<StoryRecord, bool>());
            mTables["Building"] = Building;
            Delegate_Binder.RegBind(typeof(Func<BuildingRecord, bool>), new Delegate_BindTool_Ret<BuildingRecord, bool>());
            mTables["BuildingRes"] = BuildingRes;
            Delegate_Binder.RegBind(typeof(Func<BuildingResRecord, bool>), new Delegate_BindTool_Ret<BuildingResRecord, bool>());
            mTables["BuildingRule"] = BuildingRule;
            Delegate_Binder.RegBind(typeof(Func<BuildingRuleRecord, bool>), new Delegate_BindTool_Ret<BuildingRuleRecord, bool>());
            mTables["BuildingService"] = BuildingService;
            Delegate_Binder.RegBind(typeof(Func<BuildingServiceRecord, bool>), new Delegate_BindTool_Ret<BuildingServiceRecord, bool>());
            mTables["HomeSence"] = HomeSence;
            Delegate_Binder.RegBind(typeof(Func<HomeSenceRecord, bool>), new Delegate_BindTool_Ret<HomeSenceRecord, bool>());
            mTables["Pet"] = Pet;
            Delegate_Binder.RegBind(typeof(Func<PetRecord, bool>), new Delegate_BindTool_Ret<PetRecord, bool>());
            mTables["PetSkill"] = PetSkill;
            Delegate_Binder.RegBind(typeof(Func<PetSkillRecord, bool>), new Delegate_BindTool_Ret<PetSkillRecord, bool>());
            mTables["Service"] = Service;
            Delegate_Binder.RegBind(typeof(Func<ServiceRecord, bool>), new Delegate_BindTool_Ret<ServiceRecord, bool>());
            mTables["StoreType"] = StoreType;
            Delegate_Binder.RegBind(typeof(Func<StoreTypeRecord, bool>), new Delegate_BindTool_Ret<StoreTypeRecord, bool>());
            mTables["Elf"] = Elf;
            Delegate_Binder.RegBind(typeof(Func<ElfRecord, bool>), new Delegate_BindTool_Ret<ElfRecord, bool>());
            mTables["ElfGroup"] = ElfGroup;
            Delegate_Binder.RegBind(typeof(Func<ElfGroupRecord, bool>), new Delegate_BindTool_Ret<ElfGroupRecord, bool>());
            mTables["Queue"] = Queue;
            Delegate_Binder.RegBind(typeof(Func<QueueRecord, bool>), new Delegate_BindTool_Ret<QueueRecord, bool>());
            mTables["Draw"] = Draw;
            Delegate_Binder.RegBind(typeof(Func<DrawRecord, bool>), new Delegate_BindTool_Ret<DrawRecord, bool>());
            mTables["Plant"] = Plant;
            Delegate_Binder.RegBind(typeof(Func<PlantRecord, bool>), new Delegate_BindTool_Ret<PlantRecord, bool>());
            mTables["SeqFrame"] = SeqFrame;
            Delegate_Binder.RegBind(typeof(Func<SeqFrameRecord, bool>), new Delegate_BindTool_Ret<SeqFrameRecord, bool>());
            mTables["Medal"] = Medal;
            Delegate_Binder.RegBind(typeof(Func<MedalRecord, bool>), new Delegate_BindTool_Ret<MedalRecord, bool>());
            mTables["Sailing"] = Sailing;
            Delegate_Binder.RegBind(typeof(Func<SailingRecord, bool>), new Delegate_BindTool_Ret<SailingRecord, bool>());
            mTables["WingTrain"] = WingTrain;
            Delegate_Binder.RegBind(typeof(Func<WingTrainRecord, bool>), new Delegate_BindTool_Ret<WingTrainRecord, bool>());
            mTables["WingQuality"] = WingQuality;
            Delegate_Binder.RegBind(typeof(Func<WingQualityRecord, bool>), new Delegate_BindTool_Ret<WingQualityRecord, bool>());
            mTables["PVPRule"] = PVPRule;
            Delegate_Binder.RegBind(typeof(Func<PVPRuleRecord, bool>), new Delegate_BindTool_Ret<PVPRuleRecord, bool>());
            mTables["ArenaReward"] = ArenaReward;
            Delegate_Binder.RegBind(typeof(Func<ArenaRewardRecord, bool>), new Delegate_BindTool_Ret<ArenaRewardRecord, bool>());
            mTables["ArenaLevel"] = ArenaLevel;
            Delegate_Binder.RegBind(typeof(Func<ArenaLevelRecord, bool>), new Delegate_BindTool_Ret<ArenaLevelRecord, bool>());
            mTables["Honor"] = Honor;
            Delegate_Binder.RegBind(typeof(Func<HonorRecord, bool>), new Delegate_BindTool_Ret<HonorRecord, bool>());
            mTables["JJCRoot"] = JJCRoot;
            Delegate_Binder.RegBind(typeof(Func<JJCRootRecord, bool>), new Delegate_BindTool_Ret<JJCRootRecord, bool>());
            mTables["Statue"] = Statue;
            Delegate_Binder.RegBind(typeof(Func<StatueRecord, bool>), new Delegate_BindTool_Ret<StatueRecord, bool>());
            mTables["EquipAdditional1"] = EquipAdditional1;
            Delegate_Binder.RegBind(typeof(Func<EquipAdditional1Record, bool>), new Delegate_BindTool_Ret<EquipAdditional1Record, bool>());
            mTables["TriggerArea"] = TriggerArea;
            Delegate_Binder.RegBind(typeof(Func<TriggerAreaRecord, bool>), new Delegate_BindTool_Ret<TriggerAreaRecord, bool>());
            mTables["Guild"] = Guild;
            Delegate_Binder.RegBind(typeof(Func<GuildRecord, bool>), new Delegate_BindTool_Ret<GuildRecord, bool>());
            mTables["GuildBuff"] = GuildBuff;
            Delegate_Binder.RegBind(typeof(Func<GuildBuffRecord, bool>), new Delegate_BindTool_Ret<GuildBuffRecord, bool>());
            mTables["GuildBoss"] = GuildBoss;
            Delegate_Binder.RegBind(typeof(Func<GuildBossRecord, bool>), new Delegate_BindTool_Ret<GuildBossRecord, bool>());
            mTables["GuildAccess"] = GuildAccess;
            Delegate_Binder.RegBind(typeof(Func<GuildAccessRecord, bool>), new Delegate_BindTool_Ret<GuildAccessRecord, bool>());
            mTables["ExpInfo"] = ExpInfo;
            Delegate_Binder.RegBind(typeof(Func<ExpInfoRecord, bool>), new Delegate_BindTool_Ret<ExpInfoRecord, bool>());
            mTables["GroupShop"] = GroupShop;
            Delegate_Binder.RegBind(typeof(Func<GroupShopRecord, bool>), new Delegate_BindTool_Ret<GroupShopRecord, bool>());
            mTables["PKMode"] = PKMode;
            Delegate_Binder.RegBind(typeof(Func<PKModeRecord, bool>), new Delegate_BindTool_Ret<PKModeRecord, bool>());
            mTables["forged"] = forged;
            Delegate_Binder.RegBind(typeof(Func<forgedRecord, bool>), new Delegate_BindTool_Ret<forgedRecord, bool>());
            mTables["EquipUpdate"] = EquipUpdate;
            Delegate_Binder.RegBind(typeof(Func<EquipUpdateRecord, bool>), new Delegate_BindTool_Ret<EquipUpdateRecord, bool>());
            mTables["GuildMission"] = GuildMission;
            Delegate_Binder.RegBind(typeof(Func<GuildMissionRecord, bool>), new Delegate_BindTool_Ret<GuildMissionRecord, bool>());
            mTables["OrderForm"] = OrderForm;
            Delegate_Binder.RegBind(typeof(Func<OrderFormRecord, bool>), new Delegate_BindTool_Ret<OrderFormRecord, bool>());
            mTables["OrderUpdate"] = OrderUpdate;
            Delegate_Binder.RegBind(typeof(Func<OrderUpdateRecord, bool>), new Delegate_BindTool_Ret<OrderUpdateRecord, bool>());
            mTables["Trade"] = Trade;
            Delegate_Binder.RegBind(typeof(Func<TradeRecord, bool>), new Delegate_BindTool_Ret<TradeRecord, bool>());
            mTables["Gem"] = Gem;
            Delegate_Binder.RegBind(typeof(Func<GemRecord, bool>), new Delegate_BindTool_Ret<GemRecord, bool>());
            mTables["GemGroup"] = GemGroup;
            Delegate_Binder.RegBind(typeof(Func<GemGroupRecord, bool>), new Delegate_BindTool_Ret<GemGroupRecord, bool>());
            mTables["SensitiveWord"] = SensitiveWord;
            Delegate_Binder.RegBind(typeof(Func<SensitiveWordRecord, bool>), new Delegate_BindTool_Ret<SensitiveWordRecord, bool>());
            mTables["Guidance"] = Guidance;
            Delegate_Binder.RegBind(typeof(Func<GuidanceRecord, bool>), new Delegate_BindTool_Ret<GuidanceRecord, bool>());
            mTables["MapTransfer"] = MapTransfer;
            Delegate_Binder.RegBind(typeof(Func<MapTransferRecord, bool>), new Delegate_BindTool_Ret<MapTransferRecord, bool>());
            mTables["SceneEffect"] = SceneEffect;
            Delegate_Binder.RegBind(typeof(Func<SceneEffectRecord, bool>), new Delegate_BindTool_Ret<SceneEffectRecord, bool>());
            mTables["PVPBattle"] = PVPBattle;
            Delegate_Binder.RegBind(typeof(Func<PVPBattleRecord, bool>), new Delegate_BindTool_Ret<PVPBattleRecord, bool>());
            mTables["StepByStep"] = StepByStep;
            Delegate_Binder.RegBind(typeof(Func<StepByStepRecord, bool>), new Delegate_BindTool_Ret<StepByStepRecord, bool>());
            mTables["WorldBOSS"] = WorldBOSS;
            Delegate_Binder.RegBind(typeof(Func<WorldBOSSRecord, bool>), new Delegate_BindTool_Ret<WorldBOSSRecord, bool>());
            mTables["PKValue"] = PKValue;
            Delegate_Binder.RegBind(typeof(Func<PKValueRecord, bool>), new Delegate_BindTool_Ret<PKValueRecord, bool>());
            mTables["Transmigration"] = Transmigration;
            Delegate_Binder.RegBind(typeof(Func<TransmigrationRecord, bool>), new Delegate_BindTool_Ret<TransmigrationRecord, bool>());
            mTables["AttachPoint"] = AttachPoint;
            Delegate_Binder.RegBind(typeof(Func<AttachPointRecord, bool>), new Delegate_BindTool_Ret<AttachPointRecord, bool>());
            mTables["FubenInfo"] = FubenInfo;
            Delegate_Binder.RegBind(typeof(Func<FubenInfoRecord, bool>), new Delegate_BindTool_Ret<FubenInfoRecord, bool>());
            mTables["FubenLogic"] = FubenLogic;
            Delegate_Binder.RegBind(typeof(Func<FubenLogicRecord, bool>), new Delegate_BindTool_Ret<FubenLogicRecord, bool>());
            mTables["Face"] = Face;
            Delegate_Binder.RegBind(typeof(Func<FaceRecord, bool>), new Delegate_BindTool_Ret<FaceRecord, bool>());
            mTables["ServerName"] = ServerName;
            Delegate_Binder.RegBind(typeof(Func<ServerNameRecord, bool>), new Delegate_BindTool_Ret<ServerNameRecord, bool>());
            mTables["LoadingTest"] = LoadingTest;
            Delegate_Binder.RegBind(typeof(Func<LoadingTestRecord, bool>), new Delegate_BindTool_Ret<LoadingTestRecord, bool>());
            mTables["GetMissionInfo"] = GetMissionInfo;
            Delegate_Binder.RegBind(typeof(Func<GetMissionInfoRecord, bool>), new Delegate_BindTool_Ret<GetMissionInfoRecord, bool>());
            mTables["MissionConditionInfo"] = MissionConditionInfo;
            Delegate_Binder.RegBind(typeof(Func<MissionConditionInfoRecord, bool>), new Delegate_BindTool_Ret<MissionConditionInfoRecord, bool>());
            mTables["GetMissionReward"] = GetMissionReward;
            Delegate_Binder.RegBind(typeof(Func<GetMissionRewardRecord, bool>), new Delegate_BindTool_Ret<GetMissionRewardRecord, bool>());
            mTables["GetMissionIcon"] = GetMissionIcon;
            Delegate_Binder.RegBind(typeof(Func<GetMissionIconRecord, bool>), new Delegate_BindTool_Ret<GetMissionIconRecord, bool>());
            mTables["Subject"] = Subject;
            Delegate_Binder.RegBind(typeof(Func<SubjectRecord, bool>), new Delegate_BindTool_Ret<SubjectRecord, bool>());
            mTables["ItemGetInfo"] = ItemGetInfo;
            Delegate_Binder.RegBind(typeof(Func<ItemGetInfoRecord, bool>), new Delegate_BindTool_Ret<ItemGetInfoRecord, bool>());
            mTables["DynamicActivity"] = DynamicActivity;
            Delegate_Binder.RegBind(typeof(Func<DynamicActivityRecord, bool>), new Delegate_BindTool_Ret<DynamicActivityRecord, bool>());
            mTables["Compensation"] = Compensation;
            Delegate_Binder.RegBind(typeof(Func<CompensationRecord, bool>), new Delegate_BindTool_Ret<CompensationRecord, bool>());
            mTables["CityTalk"] = CityTalk;
            Delegate_Binder.RegBind(typeof(Func<CityTalkRecord, bool>), new Delegate_BindTool_Ret<CityTalkRecord, bool>());
            mTables["DailyActivity"] = DailyActivity;
            Delegate_Binder.RegBind(typeof(Func<DailyActivityRecord, bool>), new Delegate_BindTool_Ret<DailyActivityRecord, bool>());
            mTables["Recharge"] = Recharge;
            Delegate_Binder.RegBind(typeof(Func<RechargeRecord, bool>), new Delegate_BindTool_Ret<RechargeRecord, bool>());
            mTables["RewardInfo"] = RewardInfo;
            Delegate_Binder.RegBind(typeof(Func<RewardInfoRecord, bool>), new Delegate_BindTool_Ret<RewardInfoRecord, bool>());
            mTables["NameTitle"] = NameTitle;
            Delegate_Binder.RegBind(typeof(Func<NameTitleRecord, bool>), new Delegate_BindTool_Ret<NameTitleRecord, bool>());
            mTables["VIP"] = VIP;
            Delegate_Binder.RegBind(typeof(Func<VIPRecord, bool>), new Delegate_BindTool_Ret<VIPRecord, bool>());
            mTables["LevelupTips"] = LevelupTips;
            Delegate_Binder.RegBind(typeof(Func<LevelupTipsRecord, bool>), new Delegate_BindTool_Ret<LevelupTipsRecord, bool>());
            mTables["StrongType"] = StrongType;
            Delegate_Binder.RegBind(typeof(Func<StrongTypeRecord, bool>), new Delegate_BindTool_Ret<StrongTypeRecord, bool>());
            mTables["StrongData"] = StrongData;
            Delegate_Binder.RegBind(typeof(Func<StrongDataRecord, bool>), new Delegate_BindTool_Ret<StrongDataRecord, bool>());
            mTables["Mail"] = Mail;
            Delegate_Binder.RegBind(typeof(Func<MailRecord, bool>), new Delegate_BindTool_Ret<MailRecord, bool>());
            mTables["GMCommand"] = GMCommand;
            Delegate_Binder.RegBind(typeof(Func<GMCommandRecord, bool>), new Delegate_BindTool_Ret<GMCommandRecord, bool>());
            mTables["AuctionType1"] = AuctionType1;
            Delegate_Binder.RegBind(typeof(Func<AuctionType1Record, bool>), new Delegate_BindTool_Ret<AuctionType1Record, bool>());
            mTables["AuctionType2"] = AuctionType2;
            Delegate_Binder.RegBind(typeof(Func<AuctionType2Record, bool>), new Delegate_BindTool_Ret<AuctionType2Record, bool>());
            mTables["AuctionType3"] = AuctionType3;
            Delegate_Binder.RegBind(typeof(Func<AuctionType3Record, bool>), new Delegate_BindTool_Ret<AuctionType3Record, bool>());
            mTables["FirstRecharge"] = FirstRecharge;
            Delegate_Binder.RegBind(typeof(Func<FirstRechargeRecord, bool>), new Delegate_BindTool_Ret<FirstRechargeRecord, bool>());
            mTables["MieShi"] = MieShi;
            Delegate_Binder.RegBind(typeof(Func<MieShiRecord, bool>), new Delegate_BindTool_Ret<MieShiRecord, bool>());
            mTables["MieShiPublic"] = MieShiPublic;
            Delegate_Binder.RegBind(typeof(Func<MieShiPublicRecord, bool>), new Delegate_BindTool_Ret<MieShiPublicRecord, bool>());
            mTables["DefendCityReward"] = DefendCityReward;
            Delegate_Binder.RegBind(typeof(Func<DefendCityRewardRecord, bool>), new Delegate_BindTool_Ret<DefendCityRewardRecord, bool>());
            mTables["DefendCityDevoteReward"] = DefendCityDevoteReward;
            Delegate_Binder.RegBind(typeof(Func<DefendCityDevoteRewardRecord, bool>), new Delegate_BindTool_Ret<DefendCityDevoteRewardRecord, bool>());
            mTables["BatteryLevel"] = BatteryLevel;
            Delegate_Binder.RegBind(typeof(Func<BatteryLevelRecord, bool>), new Delegate_BindTool_Ret<BatteryLevelRecord, bool>());
            mTables["BatteryBase"] = BatteryBase;
            Delegate_Binder.RegBind(typeof(Func<BatteryBaseRecord, bool>), new Delegate_BindTool_Ret<BatteryBaseRecord, bool>());
            mTables["MieShiFighting"] = MieShiFighting;
            Delegate_Binder.RegBind(typeof(Func<MieShiFightingRecord, bool>), new Delegate_BindTool_Ret<MieShiFightingRecord, bool>());
            mTables["FunctionOn"] = FunctionOn;
            Delegate_Binder.RegBind(typeof(Func<FunctionOnRecord, bool>), new Delegate_BindTool_Ret<FunctionOnRecord, bool>());
            mTables["BangBuff"] = BangBuff;
            Delegate_Binder.RegBind(typeof(Func<BangBuffRecord, bool>), new Delegate_BindTool_Ret<BangBuffRecord, bool>());
            mTables["BuffGroup"] = BuffGroup;
            Delegate_Binder.RegBind(typeof(Func<BuffGroupRecord, bool>), new Delegate_BindTool_Ret<BuffGroupRecord, bool>());
            mTables["MieshiTowerReward"] = MieshiTowerReward;
            Delegate_Binder.RegBind(typeof(Func<MieshiTowerRewardRecord, bool>), new Delegate_BindTool_Ret<MieshiTowerRewardRecord, bool>());
            mTables["ClimbingTower"] = ClimbingTower;
            Delegate_Binder.RegBind(typeof(Func<ClimbingTowerRecord, bool>), new Delegate_BindTool_Ret<ClimbingTowerRecord, bool>());
            mTables["AcientBattleField"] = AcientBattleField;
            Delegate_Binder.RegBind(typeof(Func<AcientBattleFieldRecord, bool>), new Delegate_BindTool_Ret<AcientBattleFieldRecord, bool>());
            mTables["ElfStarShader"] = ElfStarShader;
            Delegate_Binder.RegBind(typeof(Func<ElfStarShaderRecord, bool>), new Delegate_BindTool_Ret<ElfStarShaderRecord, bool>());
            mTables["ConsumArray"] = ConsumArray;
            Delegate_Binder.RegBind(typeof(Func<ConsumArrayRecord, bool>), new Delegate_BindTool_Ret<ConsumArrayRecord, bool>());
        }
        public static void Init()
        {
            TableManager.InitTable("Icon", Icon, TableType.Icon);
            TableManager.InitTable("Sound", Sound, TableType.Sound);
            TableManager.InitTable("ConditionTable", ConditionTable, TableType.ConditionTable);
            TableManager.InitTable("Exdata", Exdata, TableType.Exdata);
            TableManager.InitTable("Dictionary", Dictionary, TableType.Dictionary);
            TableManager.InitTable("SceneNpc", SceneNpc, TableType.SceneNpc);
            TableManager.InitTable("CharModel", CharModel, TableType.CharModel);
            TableManager.InitTable("Animation", Animation, TableType.Animation);
            TableManager.InitTable("Skill", Skill, TableType.Skill);
            TableManager.InitTable("Scene", Scene, TableType.Scene);
            TableManager.InitTable("CharacterBase", CharacterBase, TableType.CharacterBase);
            TableManager.InitTable("EquipBase", EquipBase, TableType.EquipBase);
            TableManager.InitTable("Actor", Actor, TableType.Actor);
            TableManager.InitTable("Talent", Talent, TableType.Talent);
            TableManager.InitTable("BagBase", BagBase, TableType.BagBase);
            TableManager.InitTable("ItemBase", ItemBase, TableType.ItemBase);
            TableManager.InitTable("ItemType", ItemType, TableType.ItemType);
            TableManager.InitTable("ColorBase", ColorBase, TableType.ColorBase);
            TableManager.InitTable("Buff", Buff, TableType.Buff);
            TableManager.InitTable("MissionBase", MissionBase, TableType.MissionBase);
            TableManager.InitTable("AttrRef", AttrRef, TableType.AttrRef);
            TableManager.InitTable("EquipRelate", EquipRelate, TableType.EquipRelate);
            TableManager.InitTable("EquipEnchant", EquipEnchant, TableType.EquipEnchant);
            TableManager.InitTable("EquipEnchantChance", EquipEnchantChance, TableType.EquipEnchantChance);
            TableManager.InitTable("Title", Title, TableType.Title);
            TableManager.InitTable("EquipEnchance", EquipEnchance, TableType.EquipEnchance);
            TableManager.InitTable("LevelData", LevelData, TableType.LevelData);
            TableManager.InitTable("Bullet", Bullet, TableType.Bullet);
            TableManager.InitTable("NpcBase", NpcBase, TableType.NpcBase);
            TableManager.InitTable("SkillUpgrading", SkillUpgrading, TableType.SkillUpgrading);
            TableManager.InitTable("Achievement", Achievement, TableType.Achievement);
            TableManager.InitTable("EquipTie", EquipTie, TableType.EquipTie);
            TableManager.InitTable("Effect", Effect, TableType.Effect);
            TableManager.InitTable("Transfer", Transfer, TableType.Transfer);
            TableManager.InitTable("ClientConfig", ClientConfig, TableType.ClientConfig);
            TableManager.InitTable("WeaponMount", WeaponMount, TableType.WeaponMount);
            TableManager.InitTable("CombatText", CombatText, TableType.CombatText);
            TableManager.InitTable("RandName", RandName, TableType.RandName);
            TableManager.InitTable("OperationList", OperationList, TableType.OperationList);
            TableManager.InitTable("UI", UI, TableType.UI);
            TableManager.InitTable("Gift", Gift, TableType.Gift);
            TableManager.InitTable("EquipBlessing", EquipBlessing, TableType.EquipBlessing);
            TableManager.InitTable("EquipAdditional", EquipAdditional, TableType.EquipAdditional);
            TableManager.InitTable("EquipExcellent", EquipExcellent, TableType.EquipExcellent);
            TableManager.InitTable("EquipModelView", EquipModelView, TableType.EquipModelView);
            TableManager.InitTable("Talk", Talk, TableType.Talk);
            TableManager.InitTable("ChatInfo", ChatInfo, TableType.ChatInfo);
            TableManager.InitTable("HandBook", HandBook, TableType.HandBook);
            TableManager.InitTable("BookGroup", BookGroup, TableType.BookGroup);
            TableManager.InitTable("ItemCompose", ItemCompose, TableType.ItemCompose);
            TableManager.InitTable("Camp", Camp, TableType.Camp);
            TableManager.InitTable("DropModel", DropModel, TableType.DropModel);
            TableManager.InitTable("Fuben", Fuben, TableType.Fuben);
            TableManager.InitTable("Stats", Stats, TableType.Stats);
            TableManager.InitTable("PlotFuben", PlotFuben, TableType.PlotFuben);
            TableManager.InitTable("Store", Store, TableType.Store);
            TableManager.InitTable("Story", Story, TableType.Story);
            TableManager.InitTable("Building", Building, TableType.Building);
            TableManager.InitTable("BuildingRes", BuildingRes, TableType.BuildingRes);
            TableManager.InitTable("BuildingRule", BuildingRule, TableType.BuildingRule);
            TableManager.InitTable("BuildingService", BuildingService, TableType.BuildingService);
            TableManager.InitTable("HomeSence", HomeSence, TableType.HomeSence);
            TableManager.InitTable("Pet", Pet, TableType.Pet);
            TableManager.InitTable("PetSkill", PetSkill, TableType.PetSkill);
            TableManager.InitTable("Service", Service, TableType.Service);
            TableManager.InitTable("StoreType", StoreType, TableType.StoreType);
            TableManager.InitTable("Elf", Elf, TableType.Elf);
            TableManager.InitTable("ElfGroup", ElfGroup, TableType.ElfGroup);
            TableManager.InitTable("Queue", Queue, TableType.Queue);
            TableManager.InitTable("Draw", Draw, TableType.Draw);
            TableManager.InitTable("Plant", Plant, TableType.Plant);
            TableManager.InitTable("SeqFrame", SeqFrame, TableType.SeqFrame);
            TableManager.InitTable("Medal", Medal, TableType.Medal);
            TableManager.InitTable("Sailing", Sailing, TableType.Sailing);
            TableManager.InitTable("WingTrain", WingTrain, TableType.WingTrain);
            TableManager.InitTable("WingQuality", WingQuality, TableType.WingQuality);
            TableManager.InitTable("PVPRule", PVPRule, TableType.PVPRule);
            TableManager.InitTable("ArenaReward", ArenaReward, TableType.ArenaReward);
            TableManager.InitTable("ArenaLevel", ArenaLevel, TableType.ArenaLevel);
            TableManager.InitTable("Honor", Honor, TableType.Honor);
            TableManager.InitTable("JJCRoot", JJCRoot, TableType.JJCRoot);
            TableManager.InitTable("Statue", Statue, TableType.Statue);
            TableManager.InitTable("EquipAdditional1", EquipAdditional1, TableType.EquipAdditional1);
            TableManager.InitTable("TriggerArea", TriggerArea, TableType.TriggerArea);
            TableManager.InitTable("Guild", Guild, TableType.Guild);
            TableManager.InitTable("GuildBuff", GuildBuff, TableType.GuildBuff);
            TableManager.InitTable("GuildBoss", GuildBoss, TableType.GuildBoss);
            TableManager.InitTable("GuildAccess", GuildAccess, TableType.GuildAccess);
            TableManager.InitTable("ExpInfo", ExpInfo, TableType.ExpInfo);
            TableManager.InitTable("GroupShop", GroupShop, TableType.GroupShop);
            TableManager.InitTable("PKMode", PKMode, TableType.PKMode);
            TableManager.InitTable("forged", forged, TableType.forged);
            TableManager.InitTable("EquipUpdate", EquipUpdate, TableType.EquipUpdate);
            TableManager.InitTable("GuildMission", GuildMission, TableType.GuildMission);
            TableManager.InitTable("OrderForm", OrderForm, TableType.OrderForm);
            TableManager.InitTable("OrderUpdate", OrderUpdate, TableType.OrderUpdate);
            TableManager.InitTable("Trade", Trade, TableType.Trade);
            TableManager.InitTable("Gem", Gem, TableType.Gem);
            TableManager.InitTable("GemGroup", GemGroup, TableType.GemGroup);
            TableManager.InitTable("SensitiveWord", SensitiveWord, TableType.SensitiveWord);
            TableManager.InitTable("Guidance", Guidance, TableType.Guidance);
            TableManager.InitTable("MapTransfer", MapTransfer, TableType.MapTransfer);
            TableManager.InitTable("SceneEffect", SceneEffect, TableType.SceneEffect);
            TableManager.InitTable("PVPBattle", PVPBattle, TableType.PVPBattle);
            TableManager.InitTable("StepByStep", StepByStep, TableType.StepByStep);
            TableManager.InitTable("WorldBOSS", WorldBOSS, TableType.WorldBOSS);
            TableManager.InitTable("PKValue", PKValue, TableType.PKValue);
            TableManager.InitTable("Transmigration", Transmigration, TableType.Transmigration);
            TableManager.InitTable("AttachPoint", AttachPoint, TableType.AttachPoint);
            TableManager.InitTable("FubenInfo", FubenInfo, TableType.FubenInfo);
            TableManager.InitTable("FubenLogic", FubenLogic, TableType.FubenLogic);
            TableManager.InitTable("Face", Face, TableType.Face);
            TableManager.InitTable("ServerName", ServerName, TableType.ServerName);
            TableManager.InitTable("LoadingTest", LoadingTest, TableType.LoadingTest);
            TableManager.InitTable("GetMissionInfo", GetMissionInfo, TableType.GetMissionInfo);
            TableManager.InitTable("MissionConditionInfo", MissionConditionInfo, TableType.MissionConditionInfo);
            TableManager.InitTable("GetMissionReward", GetMissionReward, TableType.GetMissionReward);
            TableManager.InitTable("GetMissionIcon", GetMissionIcon, TableType.GetMissionIcon);
            TableManager.InitTable("Subject", Subject, TableType.Subject);
            TableManager.InitTable("ItemGetInfo", ItemGetInfo, TableType.ItemGetInfo);
            TableManager.InitTable("DynamicActivity", DynamicActivity, TableType.DynamicActivity);
            TableManager.InitTable("Compensation", Compensation, TableType.Compensation);
            TableManager.InitTable("CityTalk", CityTalk, TableType.CityTalk);
            TableManager.InitTable("DailyActivity", DailyActivity, TableType.DailyActivity);
            TableManager.InitTable("Recharge", Recharge, TableType.Recharge);
            TableManager.InitTable("RewardInfo", RewardInfo, TableType.RewardInfo);
            TableManager.InitTable("NameTitle", NameTitle, TableType.NameTitle);
            TableManager.InitTable("VIP", VIP, TableType.VIP);
            TableManager.InitTable("LevelupTips", LevelupTips, TableType.LevelupTips);
            TableManager.InitTable("StrongType", StrongType, TableType.StrongType);
            TableManager.InitTable("StrongData", StrongData, TableType.StrongData);
            TableManager.InitTable("Mail", Mail, TableType.Mail);
            TableManager.InitTable("GMCommand", GMCommand, TableType.GMCommand);
            TableManager.InitTable("AuctionType1", AuctionType1, TableType.AuctionType1);
            TableManager.InitTable("AuctionType2", AuctionType2, TableType.AuctionType2);
            TableManager.InitTable("AuctionType3", AuctionType3, TableType.AuctionType3);
            TableManager.InitTable("FirstRecharge", FirstRecharge, TableType.FirstRecharge);
            TableManager.InitTable("MieShi", MieShi, TableType.MieShi);
            TableManager.InitTable("MieShiPublic", MieShiPublic, TableType.MieShiPublic);
            TableManager.InitTable("DefendCityReward", DefendCityReward, TableType.DefendCityReward);
            TableManager.InitTable("DefendCityDevoteReward", DefendCityDevoteReward, TableType.DefendCityDevoteReward);
            TableManager.InitTable("BatteryLevel", BatteryLevel, TableType.BatteryLevel);
            TableManager.InitTable("BatteryBase", BatteryBase, TableType.BatteryBase);
            TableManager.InitTable("MieShiFighting", MieShiFighting, TableType.MieShiFighting);
            TableManager.InitTable("FunctionOn", FunctionOn, TableType.FunctionOn);
            TableManager.InitTable("BangBuff", BangBuff, TableType.BangBuff);
            TableManager.InitTable("BuffGroup", BuffGroup, TableType.BuffGroup);
            TableManager.InitTable("MieshiTowerReward", MieshiTowerReward, TableType.MieshiTowerReward);
            TableManager.InitTable("ClimbingTower", ClimbingTower, TableType.ClimbingTower);
            TableManager.InitTable("AcientBattleField", AcientBattleField, TableType.AcientBattleField);
            TableManager.InitTable("ElfStarShader", ElfStarShader, TableType.ElfStarShader);
            TableManager.InitTable("ConsumArray", ConsumArray, TableType.ConsumArray);
        }
        public static IEnumerable<string> GetTableNames()
        {
            return mTables.Keys;
        }
        public static object GetTableByFileName(string name)
        {
            object tt;
            if (mTables.TryGetValue(name, out tt))
            {
                return tt;
            }
            Logger.Error("GetTableByFileName is Faild [{0}].txt", name);
            return null;
        }
        public static object GetTableData(string name,int RowId,string ColumnName)
        {
             IDictionary tt = GetTableByFileName(name) as IDictionary;
             if (tt == null) return null;
             IRecord record = tt[RowId] as IRecord;
             return record.GetField(ColumnName);
        }
        public static void ForeachIcon(Func<IconRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Icon act is null");
                return;
            }
            foreach (var tempRecord in Icon)
            {
                try
                {
                    if (!act((IconRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static IconRecord GetIcon(int nId)
        {
            IRecord tbIcon;
            if (!Icon.TryGetValue(nId, out tbIcon))
            {
                Logger.Info("Icon[{0}] not find by Table", nId);
                return null;
            }
            return (IconRecord)tbIcon;
        }
        public static void ForeachSound(Func<SoundRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Sound act is null");
                return;
            }
            foreach (var tempRecord in Sound)
            {
                try
                {
                    if (!act((SoundRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SoundRecord GetSound(int nId)
        {
            IRecord tbSound;
            if (!Sound.TryGetValue(nId, out tbSound))
            {
                Logger.Info("Sound[{0}] not find by Table", nId);
                return null;
            }
            return (SoundRecord)tbSound;
        }
        public static void ForeachConditionTable(Func<ConditionTableRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ConditionTable act is null");
                return;
            }
            foreach (var tempRecord in ConditionTable)
            {
                try
                {
                    if (!act((ConditionTableRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ConditionTableRecord GetConditionTable(int nId)
        {
            IRecord tbConditionTable;
            if (!ConditionTable.TryGetValue(nId, out tbConditionTable))
            {
                Logger.Info("ConditionTable[{0}] not find by Table", nId);
                return null;
            }
            return (ConditionTableRecord)tbConditionTable;
        }
        public static void ForeachExdata(Func<ExdataRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Exdata act is null");
                return;
            }
            foreach (var tempRecord in Exdata)
            {
                try
                {
                    if (!act((ExdataRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ExdataRecord GetExdata(int nId)
        {
            IRecord tbExdata;
            if (!Exdata.TryGetValue(nId, out tbExdata))
            {
                Logger.Info("Exdata[{0}] not find by Table", nId);
                return null;
            }
            return (ExdataRecord)tbExdata;
        }
        public static void ForeachDictionary(Func<DictionaryRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Dictionary act is null");
                return;
            }
            foreach (var tempRecord in Dictionary)
            {
                try
                {
                    if (!act((DictionaryRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DictionaryRecord GetDictionary(int nId)
        {
            IRecord tbDictionary;
            if (!Dictionary.TryGetValue(nId, out tbDictionary))
            {
                Logger.Info("Dictionary[{0}] not find by Table", nId);
                return null;
            }
            return (DictionaryRecord)tbDictionary;
        }
        public static void ForeachSceneNpc(Func<SceneNpcRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach SceneNpc act is null");
                return;
            }
            foreach (var tempRecord in SceneNpc)
            {
                try
                {
                    if (!act((SceneNpcRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SceneNpcRecord GetSceneNpc(int nId)
        {
            IRecord tbSceneNpc;
            if (!SceneNpc.TryGetValue(nId, out tbSceneNpc))
            {
                Logger.Info("SceneNpc[{0}] not find by Table", nId);
                return null;
            }
            return (SceneNpcRecord)tbSceneNpc;
        }
        public static void ForeachCharModel(Func<CharModelRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach CharModel act is null");
                return;
            }
            foreach (var tempRecord in CharModel)
            {
                try
                {
                    if (!act((CharModelRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static CharModelRecord GetCharModel(int nId)
        {
            IRecord tbCharModel;
            if (!CharModel.TryGetValue(nId, out tbCharModel))
            {
                Logger.Info("CharModel[{0}] not find by Table", nId);
                return null;
            }
            return (CharModelRecord)tbCharModel;
        }
        public static void ForeachAnimation(Func<AnimationRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Animation act is null");
                return;
            }
            foreach (var tempRecord in Animation)
            {
                try
                {
                    if (!act((AnimationRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AnimationRecord GetAnimation(int nId)
        {
            IRecord tbAnimation;
            if (!Animation.TryGetValue(nId, out tbAnimation))
            {
                Logger.Info("Animation[{0}] not find by Table", nId);
                return null;
            }
            return (AnimationRecord)tbAnimation;
        }
        public static void ForeachSkill(Func<SkillRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Skill act is null");
                return;
            }
            foreach (var tempRecord in Skill)
            {
                try
                {
                    if (!act((SkillRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SkillRecord GetSkill(int nId)
        {
            IRecord tbSkill;
            if (!Skill.TryGetValue(nId, out tbSkill))
            {
                Logger.Info("Skill[{0}] not find by Table", nId);
                return null;
            }
            return (SkillRecord)tbSkill;
        }
        public static void ForeachScene(Func<SceneRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Scene act is null");
                return;
            }
            foreach (var tempRecord in Scene)
            {
                try
                {
                    if (!act((SceneRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SceneRecord GetScene(int nId)
        {
            IRecord tbScene;
            if (!Scene.TryGetValue(nId, out tbScene))
            {
                Logger.Info("Scene[{0}] not find by Table", nId);
                return null;
            }
            return (SceneRecord)tbScene;
        }
        public static void ForeachCharacterBase(Func<CharacterBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach CharacterBase act is null");
                return;
            }
            foreach (var tempRecord in CharacterBase)
            {
                try
                {
                    if (!act((CharacterBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static CharacterBaseRecord GetCharacterBase(int nId)
        {
            IRecord tbCharacterBase;
            if (!CharacterBase.TryGetValue(nId, out tbCharacterBase))
            {
                Logger.Info("CharacterBase[{0}] not find by Table", nId);
                return null;
            }
            return (CharacterBaseRecord)tbCharacterBase;
        }
        public static void ForeachEquipBase(Func<EquipBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipBase act is null");
                return;
            }
            foreach (var tempRecord in EquipBase)
            {
                try
                {
                    if (!act((EquipBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipBaseRecord GetEquipBase(int nId)
        {
            IRecord tbEquipBase;
            if (!EquipBase.TryGetValue(nId, out tbEquipBase))
            {
                Logger.Info("EquipBase[{0}] not find by Table", nId);
                return null;
            }
            return (EquipBaseRecord)tbEquipBase;
        }
        public static void ForeachActor(Func<ActorRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Actor act is null");
                return;
            }
            foreach (var tempRecord in Actor)
            {
                try
                {
                    if (!act((ActorRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ActorRecord GetActor(int nId)
        {
            IRecord tbActor;
            if (!Actor.TryGetValue(nId, out tbActor))
            {
                Logger.Info("Actor[{0}] not find by Table", nId);
                return null;
            }
            return (ActorRecord)tbActor;
        }
        public static void ForeachTalent(Func<TalentRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Talent act is null");
                return;
            }
            foreach (var tempRecord in Talent)
            {
                try
                {
                    if (!act((TalentRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TalentRecord GetTalent(int nId)
        {
            IRecord tbTalent;
            if (!Talent.TryGetValue(nId, out tbTalent))
            {
                Logger.Info("Talent[{0}] not find by Table", nId);
                return null;
            }
            return (TalentRecord)tbTalent;
        }
        public static void ForeachBagBase(Func<BagBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BagBase act is null");
                return;
            }
            foreach (var tempRecord in BagBase)
            {
                try
                {
                    if (!act((BagBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BagBaseRecord GetBagBase(int nId)
        {
            IRecord tbBagBase;
            if (!BagBase.TryGetValue(nId, out tbBagBase))
            {
                Logger.Info("BagBase[{0}] not find by Table", nId);
                return null;
            }
            return (BagBaseRecord)tbBagBase;
        }
        public static void ForeachItemBase(Func<ItemBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ItemBase act is null");
                return;
            }
            foreach (var tempRecord in ItemBase)
            {
                try
                {
                    if (!act((ItemBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ItemBaseRecord GetItemBase(int nId)
        {
            IRecord tbItemBase;
            if (!ItemBase.TryGetValue(nId, out tbItemBase))
            {
                Logger.Info("ItemBase[{0}] not find by Table", nId);
                return null;
            }
            return (ItemBaseRecord)tbItemBase;
        }
        public static void ForeachItemType(Func<ItemTypeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ItemType act is null");
                return;
            }
            foreach (var tempRecord in ItemType)
            {
                try
                {
                    if (!act((ItemTypeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ItemTypeRecord GetItemType(int nId)
        {
            IRecord tbItemType;
            if (!ItemType.TryGetValue(nId, out tbItemType))
            {
                Logger.Info("ItemType[{0}] not find by Table", nId);
                return null;
            }
            return (ItemTypeRecord)tbItemType;
        }
        public static void ForeachColorBase(Func<ColorBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ColorBase act is null");
                return;
            }
            foreach (var tempRecord in ColorBase)
            {
                try
                {
                    if (!act((ColorBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ColorBaseRecord GetColorBase(int nId)
        {
            IRecord tbColorBase;
            if (!ColorBase.TryGetValue(nId, out tbColorBase))
            {
                Logger.Info("ColorBase[{0}] not find by Table", nId);
                return null;
            }
            return (ColorBaseRecord)tbColorBase;
        }
        public static void ForeachBuff(Func<BuffRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Buff act is null");
                return;
            }
            foreach (var tempRecord in Buff)
            {
                try
                {
                    if (!act((BuffRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BuffRecord GetBuff(int nId)
        {
            IRecord tbBuff;
            if (!Buff.TryGetValue(nId, out tbBuff))
            {
                Logger.Info("Buff[{0}] not find by Table", nId);
                return null;
            }
            return (BuffRecord)tbBuff;
        }
        public static void ForeachMissionBase(Func<MissionBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MissionBase act is null");
                return;
            }
            foreach (var tempRecord in MissionBase)
            {
                try
                {
                    if (!act((MissionBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MissionBaseRecord GetMissionBase(int nId)
        {
            IRecord tbMissionBase;
            if (!MissionBase.TryGetValue(nId, out tbMissionBase))
            {
                Logger.Info("MissionBase[{0}] not find by Table", nId);
                return null;
            }
            return (MissionBaseRecord)tbMissionBase;
        }
        public static void ForeachAttrRef(Func<AttrRefRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach AttrRef act is null");
                return;
            }
            foreach (var tempRecord in AttrRef)
            {
                try
                {
                    if (!act((AttrRefRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AttrRefRecord GetAttrRef(int nId)
        {
            IRecord tbAttrRef;
            if (!AttrRef.TryGetValue(nId, out tbAttrRef))
            {
                Logger.Info("AttrRef[{0}] not find by Table", nId);
                return null;
            }
            return (AttrRefRecord)tbAttrRef;
        }
        public static void ForeachEquipRelate(Func<EquipRelateRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipRelate act is null");
                return;
            }
            foreach (var tempRecord in EquipRelate)
            {
                try
                {
                    if (!act((EquipRelateRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipRelateRecord GetEquipRelate(int nId)
        {
            IRecord tbEquipRelate;
            if (!EquipRelate.TryGetValue(nId, out tbEquipRelate))
            {
                Logger.Info("EquipRelate[{0}] not find by Table", nId);
                return null;
            }
            return (EquipRelateRecord)tbEquipRelate;
        }
        public static void ForeachEquipEnchant(Func<EquipEnchantRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipEnchant act is null");
                return;
            }
            foreach (var tempRecord in EquipEnchant)
            {
                try
                {
                    if (!act((EquipEnchantRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipEnchantRecord GetEquipEnchant(int nId)
        {
            IRecord tbEquipEnchant;
            if (!EquipEnchant.TryGetValue(nId, out tbEquipEnchant))
            {
                Logger.Info("EquipEnchant[{0}] not find by Table", nId);
                return null;
            }
            return (EquipEnchantRecord)tbEquipEnchant;
        }
        public static void ForeachEquipEnchantChance(Func<EquipEnchantChanceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipEnchantChance act is null");
                return;
            }
            foreach (var tempRecord in EquipEnchantChance)
            {
                try
                {
                    if (!act((EquipEnchantChanceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipEnchantChanceRecord GetEquipEnchantChance(int nId)
        {
            IRecord tbEquipEnchantChance;
            if (!EquipEnchantChance.TryGetValue(nId, out tbEquipEnchantChance))
            {
                Logger.Info("EquipEnchantChance[{0}] not find by Table", nId);
                return null;
            }
            return (EquipEnchantChanceRecord)tbEquipEnchantChance;
        }
        public static void ForeachTitle(Func<TitleRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Title act is null");
                return;
            }
            foreach (var tempRecord in Title)
            {
                try
                {
                    if (!act((TitleRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TitleRecord GetTitle(int nId)
        {
            IRecord tbTitle;
            if (!Title.TryGetValue(nId, out tbTitle))
            {
                Logger.Info("Title[{0}] not find by Table", nId);
                return null;
            }
            return (TitleRecord)tbTitle;
        }
        public static void ForeachEquipEnchance(Func<EquipEnchanceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipEnchance act is null");
                return;
            }
            foreach (var tempRecord in EquipEnchance)
            {
                try
                {
                    if (!act((EquipEnchanceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipEnchanceRecord GetEquipEnchance(int nId)
        {
            IRecord tbEquipEnchance;
            if (!EquipEnchance.TryGetValue(nId, out tbEquipEnchance))
            {
                Logger.Info("EquipEnchance[{0}] not find by Table", nId);
                return null;
            }
            return (EquipEnchanceRecord)tbEquipEnchance;
        }
        public static void ForeachLevelData(Func<LevelDataRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach LevelData act is null");
                return;
            }
            foreach (var tempRecord in LevelData)
            {
                try
                {
                    if (!act((LevelDataRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static LevelDataRecord GetLevelData(int nId)
        {
            IRecord tbLevelData;
            if (!LevelData.TryGetValue(nId, out tbLevelData))
            {
                Logger.Info("LevelData[{0}] not find by Table", nId);
                return null;
            }
            return (LevelDataRecord)tbLevelData;
        }
        public static void ForeachBullet(Func<BulletRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Bullet act is null");
                return;
            }
            foreach (var tempRecord in Bullet)
            {
                try
                {
                    if (!act((BulletRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BulletRecord GetBullet(int nId)
        {
            IRecord tbBullet;
            if (!Bullet.TryGetValue(nId, out tbBullet))
            {
                Logger.Info("Bullet[{0}] not find by Table", nId);
                return null;
            }
            return (BulletRecord)tbBullet;
        }
        public static void ForeachNpcBase(Func<NpcBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach NpcBase act is null");
                return;
            }
            foreach (var tempRecord in NpcBase)
            {
                try
                {
                    if (!act((NpcBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static NpcBaseRecord GetNpcBase(int nId)
        {
            IRecord tbNpcBase;
            if (!NpcBase.TryGetValue(nId, out tbNpcBase))
            {
                Logger.Info("NpcBase[{0}] not find by Table", nId);
                return null;
            }
            return (NpcBaseRecord)tbNpcBase;
        }
        public static void ForeachSkillUpgrading(Func<SkillUpgradingRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach SkillUpgrading act is null");
                return;
            }
            foreach (var tempRecord in SkillUpgrading)
            {
                try
                {
                    if (!act((SkillUpgradingRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SkillUpgradingRecord GetSkillUpgrading(int nId)
        {
            IRecord tbSkillUpgrading;
            if (!SkillUpgrading.TryGetValue(nId, out tbSkillUpgrading))
            {
                Logger.Info("SkillUpgrading[{0}] not find by Table", nId);
                return null;
            }
            return (SkillUpgradingRecord)tbSkillUpgrading;
        }
        public static void ForeachAchievement(Func<AchievementRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Achievement act is null");
                return;
            }
            foreach (var tempRecord in Achievement)
            {
                try
                {
                    if (!act((AchievementRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AchievementRecord GetAchievement(int nId)
        {
            IRecord tbAchievement;
            if (!Achievement.TryGetValue(nId, out tbAchievement))
            {
                Logger.Info("Achievement[{0}] not find by Table", nId);
                return null;
            }
            return (AchievementRecord)tbAchievement;
        }
        public static void ForeachEquipTie(Func<EquipTieRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipTie act is null");
                return;
            }
            foreach (var tempRecord in EquipTie)
            {
                try
                {
                    if (!act((EquipTieRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipTieRecord GetEquipTie(int nId)
        {
            IRecord tbEquipTie;
            if (!EquipTie.TryGetValue(nId, out tbEquipTie))
            {
                Logger.Info("EquipTie[{0}] not find by Table", nId);
                return null;
            }
            return (EquipTieRecord)tbEquipTie;
        }
        public static void ForeachEffect(Func<EffectRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Effect act is null");
                return;
            }
            foreach (var tempRecord in Effect)
            {
                try
                {
                    if (!act((EffectRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EffectRecord GetEffect(int nId)
        {
            IRecord tbEffect;
            if (!Effect.TryGetValue(nId, out tbEffect))
            {
                Logger.Info("Effect[{0}] not find by Table", nId);
                return null;
            }
            return (EffectRecord)tbEffect;
        }
        public static void ForeachTransfer(Func<TransferRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Transfer act is null");
                return;
            }
            foreach (var tempRecord in Transfer)
            {
                try
                {
                    if (!act((TransferRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TransferRecord GetTransfer(int nId)
        {
            IRecord tbTransfer;
            if (!Transfer.TryGetValue(nId, out tbTransfer))
            {
                Logger.Info("Transfer[{0}] not find by Table", nId);
                return null;
            }
            return (TransferRecord)tbTransfer;
        }
        public static void ForeachClientConfig(Func<ClientConfigRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ClientConfig act is null");
                return;
            }
            foreach (var tempRecord in ClientConfig)
            {
                try
                {
                    if (!act((ClientConfigRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ClientConfigRecord GetClientConfig(int nId)
        {
            IRecord tbClientConfig;
            if (!ClientConfig.TryGetValue(nId, out tbClientConfig))
            {
                Logger.Info("ClientConfig[{0}] not find by Table", nId);
                return null;
            }
            return (ClientConfigRecord)tbClientConfig;
        }
        public static void ForeachWeaponMount(Func<WeaponMountRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach WeaponMount act is null");
                return;
            }
            foreach (var tempRecord in WeaponMount)
            {
                try
                {
                    if (!act((WeaponMountRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static WeaponMountRecord GetWeaponMount(int nId)
        {
            IRecord tbWeaponMount;
            if (!WeaponMount.TryGetValue(nId, out tbWeaponMount))
            {
                Logger.Info("WeaponMount[{0}] not find by Table", nId);
                return null;
            }
            return (WeaponMountRecord)tbWeaponMount;
        }
        public static void ForeachCombatText(Func<CombatTextRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach CombatText act is null");
                return;
            }
            foreach (var tempRecord in CombatText)
            {
                try
                {
                    if (!act((CombatTextRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static CombatTextRecord GetCombatText(int nId)
        {
            IRecord tbCombatText;
            if (!CombatText.TryGetValue(nId, out tbCombatText))
            {
                Logger.Info("CombatText[{0}] not find by Table", nId);
                return null;
            }
            return (CombatTextRecord)tbCombatText;
        }
        public static void ForeachRandName(Func<RandNameRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach RandName act is null");
                return;
            }
            foreach (var tempRecord in RandName)
            {
                try
                {
                    if (!act((RandNameRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static RandNameRecord GetRandName(int nId)
        {
            IRecord tbRandName;
            if (!RandName.TryGetValue(nId, out tbRandName))
            {
                Logger.Info("RandName[{0}] not find by Table", nId);
                return null;
            }
            return (RandNameRecord)tbRandName;
        }
        public static void ForeachOperationList(Func<OperationListRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach OperationList act is null");
                return;
            }
            foreach (var tempRecord in OperationList)
            {
                try
                {
                    if (!act((OperationListRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static OperationListRecord GetOperationList(int nId)
        {
            IRecord tbOperationList;
            if (!OperationList.TryGetValue(nId, out tbOperationList))
            {
                Logger.Info("OperationList[{0}] not find by Table", nId);
                return null;
            }
            return (OperationListRecord)tbOperationList;
        }
        public static void ForeachUI(Func<UIRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach UI act is null");
                return;
            }
            foreach (var tempRecord in UI)
            {
                try
                {
                    if (!act((UIRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static UIRecord GetUI(int nId)
        {
            IRecord tbUI;
            if (!UI.TryGetValue(nId, out tbUI))
            {
                Logger.Info("UI[{0}] not find by Table", nId);
                return null;
            }
            return (UIRecord)tbUI;
        }
        public static void ForeachGift(Func<GiftRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Gift act is null");
                return;
            }
            foreach (var tempRecord in Gift)
            {
                try
                {
                    if (!act((GiftRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GiftRecord GetGift(int nId)
        {
            IRecord tbGift;
            if (!Gift.TryGetValue(nId, out tbGift))
            {
                Logger.Info("Gift[{0}] not find by Table", nId);
                return null;
            }
            return (GiftRecord)tbGift;
        }
        public static void ForeachEquipBlessing(Func<EquipBlessingRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipBlessing act is null");
                return;
            }
            foreach (var tempRecord in EquipBlessing)
            {
                try
                {
                    if (!act((EquipBlessingRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipBlessingRecord GetEquipBlessing(int nId)
        {
            IRecord tbEquipBlessing;
            if (!EquipBlessing.TryGetValue(nId, out tbEquipBlessing))
            {
                Logger.Info("EquipBlessing[{0}] not find by Table", nId);
                return null;
            }
            return (EquipBlessingRecord)tbEquipBlessing;
        }
        public static void ForeachEquipAdditional(Func<EquipAdditionalRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipAdditional act is null");
                return;
            }
            foreach (var tempRecord in EquipAdditional)
            {
                try
                {
                    if (!act((EquipAdditionalRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipAdditionalRecord GetEquipAdditional(int nId)
        {
            IRecord tbEquipAdditional;
            if (!EquipAdditional.TryGetValue(nId, out tbEquipAdditional))
            {
                Logger.Info("EquipAdditional[{0}] not find by Table", nId);
                return null;
            }
            return (EquipAdditionalRecord)tbEquipAdditional;
        }
        public static void ForeachEquipExcellent(Func<EquipExcellentRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipExcellent act is null");
                return;
            }
            foreach (var tempRecord in EquipExcellent)
            {
                try
                {
                    if (!act((EquipExcellentRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipExcellentRecord GetEquipExcellent(int nId)
        {
            IRecord tbEquipExcellent;
            if (!EquipExcellent.TryGetValue(nId, out tbEquipExcellent))
            {
                Logger.Info("EquipExcellent[{0}] not find by Table", nId);
                return null;
            }
            return (EquipExcellentRecord)tbEquipExcellent;
        }
        public static void ForeachEquipModelView(Func<EquipModelViewRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipModelView act is null");
                return;
            }
            foreach (var tempRecord in EquipModelView)
            {
                try
                {
                    if (!act((EquipModelViewRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipModelViewRecord GetEquipModelView(int nId)
        {
            IRecord tbEquipModelView;
            if (!EquipModelView.TryGetValue(nId, out tbEquipModelView))
            {
                Logger.Info("EquipModelView[{0}] not find by Table", nId);
                return null;
            }
            return (EquipModelViewRecord)tbEquipModelView;
        }
        public static void ForeachTalk(Func<TalkRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Talk act is null");
                return;
            }
            foreach (var tempRecord in Talk)
            {
                try
                {
                    if (!act((TalkRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TalkRecord GetTalk(int nId)
        {
            IRecord tbTalk;
            if (!Talk.TryGetValue(nId, out tbTalk))
            {
                Logger.Info("Talk[{0}] not find by Table", nId);
                return null;
            }
            return (TalkRecord)tbTalk;
        }
        public static void ForeachChatInfo(Func<ChatInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ChatInfo act is null");
                return;
            }
            foreach (var tempRecord in ChatInfo)
            {
                try
                {
                    if (!act((ChatInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ChatInfoRecord GetChatInfo(int nId)
        {
            IRecord tbChatInfo;
            if (!ChatInfo.TryGetValue(nId, out tbChatInfo))
            {
                Logger.Info("ChatInfo[{0}] not find by Table", nId);
                return null;
            }
            return (ChatInfoRecord)tbChatInfo;
        }
        public static void ForeachHandBook(Func<HandBookRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach HandBook act is null");
                return;
            }
            foreach (var tempRecord in HandBook)
            {
                try
                {
                    if (!act((HandBookRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static HandBookRecord GetHandBook(int nId)
        {
            IRecord tbHandBook;
            if (!HandBook.TryGetValue(nId, out tbHandBook))
            {
                Logger.Info("HandBook[{0}] not find by Table", nId);
                return null;
            }
            return (HandBookRecord)tbHandBook;
        }
        public static void ForeachBookGroup(Func<BookGroupRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BookGroup act is null");
                return;
            }
            foreach (var tempRecord in BookGroup)
            {
                try
                {
                    if (!act((BookGroupRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BookGroupRecord GetBookGroup(int nId)
        {
            IRecord tbBookGroup;
            if (!BookGroup.TryGetValue(nId, out tbBookGroup))
            {
                Logger.Info("BookGroup[{0}] not find by Table", nId);
                return null;
            }
            return (BookGroupRecord)tbBookGroup;
        }
        public static void ForeachItemCompose(Func<ItemComposeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ItemCompose act is null");
                return;
            }
            foreach (var tempRecord in ItemCompose)
            {
                try
                {
                    if (!act((ItemComposeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ItemComposeRecord GetItemCompose(int nId)
        {
            IRecord tbItemCompose;
            if (!ItemCompose.TryGetValue(nId, out tbItemCompose))
            {
                Logger.Info("ItemCompose[{0}] not find by Table", nId);
                return null;
            }
            return (ItemComposeRecord)tbItemCompose;
        }
        public static void ForeachCamp(Func<CampRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Camp act is null");
                return;
            }
            foreach (var tempRecord in Camp)
            {
                try
                {
                    if (!act((CampRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static CampRecord GetCamp(int nId)
        {
            IRecord tbCamp;
            if (!Camp.TryGetValue(nId, out tbCamp))
            {
                Logger.Info("Camp[{0}] not find by Table", nId);
                return null;
            }
            return (CampRecord)tbCamp;
        }
        public static void ForeachDropModel(Func<DropModelRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach DropModel act is null");
                return;
            }
            foreach (var tempRecord in DropModel)
            {
                try
                {
                    if (!act((DropModelRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DropModelRecord GetDropModel(int nId)
        {
            IRecord tbDropModel;
            if (!DropModel.TryGetValue(nId, out tbDropModel))
            {
                Logger.Info("DropModel[{0}] not find by Table", nId);
                return null;
            }
            return (DropModelRecord)tbDropModel;
        }
        public static void ForeachFuben(Func<FubenRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Fuben act is null");
                return;
            }
            foreach (var tempRecord in Fuben)
            {
                try
                {
                    if (!act((FubenRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static FubenRecord GetFuben(int nId)
        {
            IRecord tbFuben;
            if (!Fuben.TryGetValue(nId, out tbFuben))
            {
                Logger.Info("Fuben[{0}] not find by Table", nId);
                return null;
            }
            return (FubenRecord)tbFuben;
        }
        public static void ForeachStats(Func<StatsRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Stats act is null");
                return;
            }
            foreach (var tempRecord in Stats)
            {
                try
                {
                    if (!act((StatsRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StatsRecord GetStats(int nId)
        {
            IRecord tbStats;
            if (!Stats.TryGetValue(nId, out tbStats))
            {
                Logger.Info("Stats[{0}] not find by Table", nId);
                return null;
            }
            return (StatsRecord)tbStats;
        }
        public static void ForeachPlotFuben(Func<PlotFubenRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach PlotFuben act is null");
                return;
            }
            foreach (var tempRecord in PlotFuben)
            {
                try
                {
                    if (!act((PlotFubenRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PlotFubenRecord GetPlotFuben(int nId)
        {
            IRecord tbPlotFuben;
            if (!PlotFuben.TryGetValue(nId, out tbPlotFuben))
            {
                Logger.Info("PlotFuben[{0}] not find by Table", nId);
                return null;
            }
            return (PlotFubenRecord)tbPlotFuben;
        }
        public static void ForeachStore(Func<StoreRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Store act is null");
                return;
            }
            foreach (var tempRecord in Store)
            {
                try
                {
                    if (!act((StoreRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StoreRecord GetStore(int nId)
        {
            IRecord tbStore;
            if (!Store.TryGetValue(nId, out tbStore))
            {
                Logger.Info("Store[{0}] not find by Table", nId);
                return null;
            }
            return (StoreRecord)tbStore;
        }
        public static void ForeachStory(Func<StoryRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Story act is null");
                return;
            }
            foreach (var tempRecord in Story)
            {
                try
                {
                    if (!act((StoryRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StoryRecord GetStory(int nId)
        {
            IRecord tbStory;
            if (!Story.TryGetValue(nId, out tbStory))
            {
                Logger.Info("Story[{0}] not find by Table", nId);
                return null;
            }
            return (StoryRecord)tbStory;
        }
        public static void ForeachBuilding(Func<BuildingRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Building act is null");
                return;
            }
            foreach (var tempRecord in Building)
            {
                try
                {
                    if (!act((BuildingRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BuildingRecord GetBuilding(int nId)
        {
            IRecord tbBuilding;
            if (!Building.TryGetValue(nId, out tbBuilding))
            {
                Logger.Info("Building[{0}] not find by Table", nId);
                return null;
            }
            return (BuildingRecord)tbBuilding;
        }
        public static void ForeachBuildingRes(Func<BuildingResRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BuildingRes act is null");
                return;
            }
            foreach (var tempRecord in BuildingRes)
            {
                try
                {
                    if (!act((BuildingResRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BuildingResRecord GetBuildingRes(int nId)
        {
            IRecord tbBuildingRes;
            if (!BuildingRes.TryGetValue(nId, out tbBuildingRes))
            {
                Logger.Info("BuildingRes[{0}] not find by Table", nId);
                return null;
            }
            return (BuildingResRecord)tbBuildingRes;
        }
        public static void ForeachBuildingRule(Func<BuildingRuleRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BuildingRule act is null");
                return;
            }
            foreach (var tempRecord in BuildingRule)
            {
                try
                {
                    if (!act((BuildingRuleRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BuildingRuleRecord GetBuildingRule(int nId)
        {
            IRecord tbBuildingRule;
            if (!BuildingRule.TryGetValue(nId, out tbBuildingRule))
            {
                Logger.Info("BuildingRule[{0}] not find by Table", nId);
                return null;
            }
            return (BuildingRuleRecord)tbBuildingRule;
        }
        public static void ForeachBuildingService(Func<BuildingServiceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BuildingService act is null");
                return;
            }
            foreach (var tempRecord in BuildingService)
            {
                try
                {
                    if (!act((BuildingServiceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BuildingServiceRecord GetBuildingService(int nId)
        {
            IRecord tbBuildingService;
            if (!BuildingService.TryGetValue(nId, out tbBuildingService))
            {
                Logger.Info("BuildingService[{0}] not find by Table", nId);
                return null;
            }
            return (BuildingServiceRecord)tbBuildingService;
        }
        public static void ForeachHomeSence(Func<HomeSenceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach HomeSence act is null");
                return;
            }
            foreach (var tempRecord in HomeSence)
            {
                try
                {
                    if (!act((HomeSenceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static HomeSenceRecord GetHomeSence(int nId)
        {
            IRecord tbHomeSence;
            if (!HomeSence.TryGetValue(nId, out tbHomeSence))
            {
                Logger.Info("HomeSence[{0}] not find by Table", nId);
                return null;
            }
            return (HomeSenceRecord)tbHomeSence;
        }
        public static void ForeachPet(Func<PetRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Pet act is null");
                return;
            }
            foreach (var tempRecord in Pet)
            {
                try
                {
                    if (!act((PetRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PetRecord GetPet(int nId)
        {
            IRecord tbPet;
            if (!Pet.TryGetValue(nId, out tbPet))
            {
                Logger.Info("Pet[{0}] not find by Table", nId);
                return null;
            }
            return (PetRecord)tbPet;
        }
        public static void ForeachPetSkill(Func<PetSkillRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach PetSkill act is null");
                return;
            }
            foreach (var tempRecord in PetSkill)
            {
                try
                {
                    if (!act((PetSkillRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PetSkillRecord GetPetSkill(int nId)
        {
            IRecord tbPetSkill;
            if (!PetSkill.TryGetValue(nId, out tbPetSkill))
            {
                Logger.Info("PetSkill[{0}] not find by Table", nId);
                return null;
            }
            return (PetSkillRecord)tbPetSkill;
        }
        public static void ForeachService(Func<ServiceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Service act is null");
                return;
            }
            foreach (var tempRecord in Service)
            {
                try
                {
                    if (!act((ServiceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ServiceRecord GetService(int nId)
        {
            IRecord tbService;
            if (!Service.TryGetValue(nId, out tbService))
            {
                Logger.Info("Service[{0}] not find by Table", nId);
                return null;
            }
            return (ServiceRecord)tbService;
        }
        public static void ForeachStoreType(Func<StoreTypeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach StoreType act is null");
                return;
            }
            foreach (var tempRecord in StoreType)
            {
                try
                {
                    if (!act((StoreTypeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StoreTypeRecord GetStoreType(int nId)
        {
            IRecord tbStoreType;
            if (!StoreType.TryGetValue(nId, out tbStoreType))
            {
                Logger.Info("StoreType[{0}] not find by Table", nId);
                return null;
            }
            return (StoreTypeRecord)tbStoreType;
        }
        public static void ForeachElf(Func<ElfRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Elf act is null");
                return;
            }
            foreach (var tempRecord in Elf)
            {
                try
                {
                    if (!act((ElfRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ElfRecord GetElf(int nId)
        {
            IRecord tbElf;
            if (!Elf.TryGetValue(nId, out tbElf))
            {
                Logger.Info("Elf[{0}] not find by Table", nId);
                return null;
            }
            return (ElfRecord)tbElf;
        }
        public static void ForeachElfGroup(Func<ElfGroupRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ElfGroup act is null");
                return;
            }
            foreach (var tempRecord in ElfGroup)
            {
                try
                {
                    if (!act((ElfGroupRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ElfGroupRecord GetElfGroup(int nId)
        {
            IRecord tbElfGroup;
            if (!ElfGroup.TryGetValue(nId, out tbElfGroup))
            {
                Logger.Info("ElfGroup[{0}] not find by Table", nId);
                return null;
            }
            return (ElfGroupRecord)tbElfGroup;
        }
        public static void ForeachQueue(Func<QueueRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Queue act is null");
                return;
            }
            foreach (var tempRecord in Queue)
            {
                try
                {
                    if (!act((QueueRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static QueueRecord GetQueue(int nId)
        {
            IRecord tbQueue;
            if (!Queue.TryGetValue(nId, out tbQueue))
            {
                Logger.Info("Queue[{0}] not find by Table", nId);
                return null;
            }
            return (QueueRecord)tbQueue;
        }
        public static void ForeachDraw(Func<DrawRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Draw act is null");
                return;
            }
            foreach (var tempRecord in Draw)
            {
                try
                {
                    if (!act((DrawRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DrawRecord GetDraw(int nId)
        {
            IRecord tbDraw;
            if (!Draw.TryGetValue(nId, out tbDraw))
            {
                Logger.Info("Draw[{0}] not find by Table", nId);
                return null;
            }
            return (DrawRecord)tbDraw;
        }
        public static void ForeachPlant(Func<PlantRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Plant act is null");
                return;
            }
            foreach (var tempRecord in Plant)
            {
                try
                {
                    if (!act((PlantRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PlantRecord GetPlant(int nId)
        {
            IRecord tbPlant;
            if (!Plant.TryGetValue(nId, out tbPlant))
            {
                Logger.Info("Plant[{0}] not find by Table", nId);
                return null;
            }
            return (PlantRecord)tbPlant;
        }
        public static void ForeachSeqFrame(Func<SeqFrameRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach SeqFrame act is null");
                return;
            }
            foreach (var tempRecord in SeqFrame)
            {
                try
                {
                    if (!act((SeqFrameRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SeqFrameRecord GetSeqFrame(int nId)
        {
            IRecord tbSeqFrame;
            if (!SeqFrame.TryGetValue(nId, out tbSeqFrame))
            {
                Logger.Info("SeqFrame[{0}] not find by Table", nId);
                return null;
            }
            return (SeqFrameRecord)tbSeqFrame;
        }
        public static void ForeachMedal(Func<MedalRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Medal act is null");
                return;
            }
            foreach (var tempRecord in Medal)
            {
                try
                {
                    if (!act((MedalRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MedalRecord GetMedal(int nId)
        {
            IRecord tbMedal;
            if (!Medal.TryGetValue(nId, out tbMedal))
            {
                Logger.Info("Medal[{0}] not find by Table", nId);
                return null;
            }
            return (MedalRecord)tbMedal;
        }
        public static void ForeachSailing(Func<SailingRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Sailing act is null");
                return;
            }
            foreach (var tempRecord in Sailing)
            {
                try
                {
                    if (!act((SailingRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SailingRecord GetSailing(int nId)
        {
            IRecord tbSailing;
            if (!Sailing.TryGetValue(nId, out tbSailing))
            {
                Logger.Info("Sailing[{0}] not find by Table", nId);
                return null;
            }
            return (SailingRecord)tbSailing;
        }
        public static void ForeachWingTrain(Func<WingTrainRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach WingTrain act is null");
                return;
            }
            foreach (var tempRecord in WingTrain)
            {
                try
                {
                    if (!act((WingTrainRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static WingTrainRecord GetWingTrain(int nId)
        {
            IRecord tbWingTrain;
            if (!WingTrain.TryGetValue(nId, out tbWingTrain))
            {
                Logger.Info("WingTrain[{0}] not find by Table", nId);
                return null;
            }
            return (WingTrainRecord)tbWingTrain;
        }
        public static void ForeachWingQuality(Func<WingQualityRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach WingQuality act is null");
                return;
            }
            foreach (var tempRecord in WingQuality)
            {
                try
                {
                    if (!act((WingQualityRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static WingQualityRecord GetWingQuality(int nId)
        {
            IRecord tbWingQuality;
            if (!WingQuality.TryGetValue(nId, out tbWingQuality))
            {
                Logger.Info("WingQuality[{0}] not find by Table", nId);
                return null;
            }
            return (WingQualityRecord)tbWingQuality;
        }
        public static void ForeachPVPRule(Func<PVPRuleRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach PVPRule act is null");
                return;
            }
            foreach (var tempRecord in PVPRule)
            {
                try
                {
                    if (!act((PVPRuleRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PVPRuleRecord GetPVPRule(int nId)
        {
            IRecord tbPVPRule;
            if (!PVPRule.TryGetValue(nId, out tbPVPRule))
            {
                Logger.Info("PVPRule[{0}] not find by Table", nId);
                return null;
            }
            return (PVPRuleRecord)tbPVPRule;
        }
        public static void ForeachArenaReward(Func<ArenaRewardRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ArenaReward act is null");
                return;
            }
            foreach (var tempRecord in ArenaReward)
            {
                try
                {
                    if (!act((ArenaRewardRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ArenaRewardRecord GetArenaReward(int nId)
        {
            IRecord tbArenaReward;
            if (!ArenaReward.TryGetValue(nId, out tbArenaReward))
            {
                Logger.Info("ArenaReward[{0}] not find by Table", nId);
                return null;
            }
            return (ArenaRewardRecord)tbArenaReward;
        }
        public static void ForeachArenaLevel(Func<ArenaLevelRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ArenaLevel act is null");
                return;
            }
            foreach (var tempRecord in ArenaLevel)
            {
                try
                {
                    if (!act((ArenaLevelRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ArenaLevelRecord GetArenaLevel(int nId)
        {
            IRecord tbArenaLevel;
            if (!ArenaLevel.TryGetValue(nId, out tbArenaLevel))
            {
                Logger.Info("ArenaLevel[{0}] not find by Table", nId);
                return null;
            }
            return (ArenaLevelRecord)tbArenaLevel;
        }
        public static void ForeachHonor(Func<HonorRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Honor act is null");
                return;
            }
            foreach (var tempRecord in Honor)
            {
                try
                {
                    if (!act((HonorRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static HonorRecord GetHonor(int nId)
        {
            IRecord tbHonor;
            if (!Honor.TryGetValue(nId, out tbHonor))
            {
                Logger.Info("Honor[{0}] not find by Table", nId);
                return null;
            }
            return (HonorRecord)tbHonor;
        }
        public static void ForeachJJCRoot(Func<JJCRootRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach JJCRoot act is null");
                return;
            }
            foreach (var tempRecord in JJCRoot)
            {
                try
                {
                    if (!act((JJCRootRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static JJCRootRecord GetJJCRoot(int nId)
        {
            IRecord tbJJCRoot;
            if (!JJCRoot.TryGetValue(nId, out tbJJCRoot))
            {
                Logger.Info("JJCRoot[{0}] not find by Table", nId);
                return null;
            }
            return (JJCRootRecord)tbJJCRoot;
        }
        public static void ForeachStatue(Func<StatueRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Statue act is null");
                return;
            }
            foreach (var tempRecord in Statue)
            {
                try
                {
                    if (!act((StatueRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StatueRecord GetStatue(int nId)
        {
            IRecord tbStatue;
            if (!Statue.TryGetValue(nId, out tbStatue))
            {
                Logger.Info("Statue[{0}] not find by Table", nId);
                return null;
            }
            return (StatueRecord)tbStatue;
        }
        public static void ForeachEquipAdditional1(Func<EquipAdditional1Record, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipAdditional1 act is null");
                return;
            }
            foreach (var tempRecord in EquipAdditional1)
            {
                try
                {
                    if (!act((EquipAdditional1Record)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipAdditional1Record GetEquipAdditional1(int nId)
        {
            IRecord tbEquipAdditional1;
            if (!EquipAdditional1.TryGetValue(nId, out tbEquipAdditional1))
            {
                Logger.Info("EquipAdditional1[{0}] not find by Table", nId);
                return null;
            }
            return (EquipAdditional1Record)tbEquipAdditional1;
        }
        public static void ForeachTriggerArea(Func<TriggerAreaRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach TriggerArea act is null");
                return;
            }
            foreach (var tempRecord in TriggerArea)
            {
                try
                {
                    if (!act((TriggerAreaRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TriggerAreaRecord GetTriggerArea(int nId)
        {
            IRecord tbTriggerArea;
            if (!TriggerArea.TryGetValue(nId, out tbTriggerArea))
            {
                Logger.Info("TriggerArea[{0}] not find by Table", nId);
                return null;
            }
            return (TriggerAreaRecord)tbTriggerArea;
        }
        public static void ForeachGuild(Func<GuildRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Guild act is null");
                return;
            }
            foreach (var tempRecord in Guild)
            {
                try
                {
                    if (!act((GuildRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GuildRecord GetGuild(int nId)
        {
            IRecord tbGuild;
            if (!Guild.TryGetValue(nId, out tbGuild))
            {
                Logger.Info("Guild[{0}] not find by Table", nId);
                return null;
            }
            return (GuildRecord)tbGuild;
        }
        public static void ForeachGuildBuff(Func<GuildBuffRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GuildBuff act is null");
                return;
            }
            foreach (var tempRecord in GuildBuff)
            {
                try
                {
                    if (!act((GuildBuffRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GuildBuffRecord GetGuildBuff(int nId)
        {
            IRecord tbGuildBuff;
            if (!GuildBuff.TryGetValue(nId, out tbGuildBuff))
            {
                Logger.Info("GuildBuff[{0}] not find by Table", nId);
                return null;
            }
            return (GuildBuffRecord)tbGuildBuff;
        }
        public static void ForeachGuildBoss(Func<GuildBossRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GuildBoss act is null");
                return;
            }
            foreach (var tempRecord in GuildBoss)
            {
                try
                {
                    if (!act((GuildBossRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GuildBossRecord GetGuildBoss(int nId)
        {
            IRecord tbGuildBoss;
            if (!GuildBoss.TryGetValue(nId, out tbGuildBoss))
            {
                Logger.Info("GuildBoss[{0}] not find by Table", nId);
                return null;
            }
            return (GuildBossRecord)tbGuildBoss;
        }
        public static void ForeachGuildAccess(Func<GuildAccessRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GuildAccess act is null");
                return;
            }
            foreach (var tempRecord in GuildAccess)
            {
                try
                {
                    if (!act((GuildAccessRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GuildAccessRecord GetGuildAccess(int nId)
        {
            IRecord tbGuildAccess;
            if (!GuildAccess.TryGetValue(nId, out tbGuildAccess))
            {
                Logger.Info("GuildAccess[{0}] not find by Table", nId);
                return null;
            }
            return (GuildAccessRecord)tbGuildAccess;
        }
        public static void ForeachExpInfo(Func<ExpInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ExpInfo act is null");
                return;
            }
            foreach (var tempRecord in ExpInfo)
            {
                try
                {
                    if (!act((ExpInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ExpInfoRecord GetExpInfo(int nId)
        {
            IRecord tbExpInfo;
            if (!ExpInfo.TryGetValue(nId, out tbExpInfo))
            {
                Logger.Info("ExpInfo[{0}] not find by Table", nId);
                return null;
            }
            return (ExpInfoRecord)tbExpInfo;
        }
        public static void ForeachGroupShop(Func<GroupShopRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GroupShop act is null");
                return;
            }
            foreach (var tempRecord in GroupShop)
            {
                try
                {
                    if (!act((GroupShopRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GroupShopRecord GetGroupShop(int nId)
        {
            IRecord tbGroupShop;
            if (!GroupShop.TryGetValue(nId, out tbGroupShop))
            {
                Logger.Info("GroupShop[{0}] not find by Table", nId);
                return null;
            }
            return (GroupShopRecord)tbGroupShop;
        }
        public static void ForeachPKMode(Func<PKModeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach PKMode act is null");
                return;
            }
            foreach (var tempRecord in PKMode)
            {
                try
                {
                    if (!act((PKModeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PKModeRecord GetPKMode(int nId)
        {
            IRecord tbPKMode;
            if (!PKMode.TryGetValue(nId, out tbPKMode))
            {
                Logger.Info("PKMode[{0}] not find by Table", nId);
                return null;
            }
            return (PKModeRecord)tbPKMode;
        }
        public static void Foreachforged(Func<forgedRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach forged act is null");
                return;
            }
            foreach (var tempRecord in forged)
            {
                try
                {
                    if (!act((forgedRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static forgedRecord Getforged(int nId)
        {
            IRecord tbforged;
            if (!forged.TryGetValue(nId, out tbforged))
            {
                Logger.Info("forged[{0}] not find by Table", nId);
                return null;
            }
            return (forgedRecord)tbforged;
        }
        public static void ForeachEquipUpdate(Func<EquipUpdateRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach EquipUpdate act is null");
                return;
            }
            foreach (var tempRecord in EquipUpdate)
            {
                try
                {
                    if (!act((EquipUpdateRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static EquipUpdateRecord GetEquipUpdate(int nId)
        {
            IRecord tbEquipUpdate;
            if (!EquipUpdate.TryGetValue(nId, out tbEquipUpdate))
            {
                Logger.Info("EquipUpdate[{0}] not find by Table", nId);
                return null;
            }
            return (EquipUpdateRecord)tbEquipUpdate;
        }
        public static void ForeachGuildMission(Func<GuildMissionRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GuildMission act is null");
                return;
            }
            foreach (var tempRecord in GuildMission)
            {
                try
                {
                    if (!act((GuildMissionRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GuildMissionRecord GetGuildMission(int nId)
        {
            IRecord tbGuildMission;
            if (!GuildMission.TryGetValue(nId, out tbGuildMission))
            {
                Logger.Info("GuildMission[{0}] not find by Table", nId);
                return null;
            }
            return (GuildMissionRecord)tbGuildMission;
        }
        public static void ForeachOrderForm(Func<OrderFormRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach OrderForm act is null");
                return;
            }
            foreach (var tempRecord in OrderForm)
            {
                try
                {
                    if (!act((OrderFormRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static OrderFormRecord GetOrderForm(int nId)
        {
            IRecord tbOrderForm;
            if (!OrderForm.TryGetValue(nId, out tbOrderForm))
            {
                Logger.Info("OrderForm[{0}] not find by Table", nId);
                return null;
            }
            return (OrderFormRecord)tbOrderForm;
        }
        public static void ForeachOrderUpdate(Func<OrderUpdateRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach OrderUpdate act is null");
                return;
            }
            foreach (var tempRecord in OrderUpdate)
            {
                try
                {
                    if (!act((OrderUpdateRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static OrderUpdateRecord GetOrderUpdate(int nId)
        {
            IRecord tbOrderUpdate;
            if (!OrderUpdate.TryGetValue(nId, out tbOrderUpdate))
            {
                Logger.Info("OrderUpdate[{0}] not find by Table", nId);
                return null;
            }
            return (OrderUpdateRecord)tbOrderUpdate;
        }
        public static void ForeachTrade(Func<TradeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Trade act is null");
                return;
            }
            foreach (var tempRecord in Trade)
            {
                try
                {
                    if (!act((TradeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TradeRecord GetTrade(int nId)
        {
            IRecord tbTrade;
            if (!Trade.TryGetValue(nId, out tbTrade))
            {
                Logger.Info("Trade[{0}] not find by Table", nId);
                return null;
            }
            return (TradeRecord)tbTrade;
        }
        public static void ForeachGem(Func<GemRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Gem act is null");
                return;
            }
            foreach (var tempRecord in Gem)
            {
                try
                {
                    if (!act((GemRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GemRecord GetGem(int nId)
        {
            IRecord tbGem;
            if (!Gem.TryGetValue(nId, out tbGem))
            {
                Logger.Info("Gem[{0}] not find by Table", nId);
                return null;
            }
            return (GemRecord)tbGem;
        }
        public static void ForeachGemGroup(Func<GemGroupRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GemGroup act is null");
                return;
            }
            foreach (var tempRecord in GemGroup)
            {
                try
                {
                    if (!act((GemGroupRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GemGroupRecord GetGemGroup(int nId)
        {
            IRecord tbGemGroup;
            if (!GemGroup.TryGetValue(nId, out tbGemGroup))
            {
                Logger.Info("GemGroup[{0}] not find by Table", nId);
                return null;
            }
            return (GemGroupRecord)tbGemGroup;
        }
        public static void ForeachSensitiveWord(Func<SensitiveWordRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach SensitiveWord act is null");
                return;
            }
            foreach (var tempRecord in SensitiveWord)
            {
                try
                {
                    if (!act((SensitiveWordRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SensitiveWordRecord GetSensitiveWord(int nId)
        {
            IRecord tbSensitiveWord;
            if (!SensitiveWord.TryGetValue(nId, out tbSensitiveWord))
            {
                Logger.Info("SensitiveWord[{0}] not find by Table", nId);
                return null;
            }
            return (SensitiveWordRecord)tbSensitiveWord;
        }
        public static void ForeachGuidance(Func<GuidanceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Guidance act is null");
                return;
            }
            foreach (var tempRecord in Guidance)
            {
                try
                {
                    if (!act((GuidanceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GuidanceRecord GetGuidance(int nId)
        {
            IRecord tbGuidance;
            if (!Guidance.TryGetValue(nId, out tbGuidance))
            {
                Logger.Info("Guidance[{0}] not find by Table", nId);
                return null;
            }
            return (GuidanceRecord)tbGuidance;
        }
        public static void ForeachMapTransfer(Func<MapTransferRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MapTransfer act is null");
                return;
            }
            foreach (var tempRecord in MapTransfer)
            {
                try
                {
                    if (!act((MapTransferRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MapTransferRecord GetMapTransfer(int nId)
        {
            IRecord tbMapTransfer;
            if (!MapTransfer.TryGetValue(nId, out tbMapTransfer))
            {
                Logger.Info("MapTransfer[{0}] not find by Table", nId);
                return null;
            }
            return (MapTransferRecord)tbMapTransfer;
        }
        public static void ForeachSceneEffect(Func<SceneEffectRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach SceneEffect act is null");
                return;
            }
            foreach (var tempRecord in SceneEffect)
            {
                try
                {
                    if (!act((SceneEffectRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SceneEffectRecord GetSceneEffect(int nId)
        {
            IRecord tbSceneEffect;
            if (!SceneEffect.TryGetValue(nId, out tbSceneEffect))
            {
                Logger.Info("SceneEffect[{0}] not find by Table", nId);
                return null;
            }
            return (SceneEffectRecord)tbSceneEffect;
        }
        public static void ForeachPVPBattle(Func<PVPBattleRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach PVPBattle act is null");
                return;
            }
            foreach (var tempRecord in PVPBattle)
            {
                try
                {
                    if (!act((PVPBattleRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PVPBattleRecord GetPVPBattle(int nId)
        {
            IRecord tbPVPBattle;
            if (!PVPBattle.TryGetValue(nId, out tbPVPBattle))
            {
                Logger.Info("PVPBattle[{0}] not find by Table", nId);
                return null;
            }
            return (PVPBattleRecord)tbPVPBattle;
        }
        public static void ForeachStepByStep(Func<StepByStepRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach StepByStep act is null");
                return;
            }
            foreach (var tempRecord in StepByStep)
            {
                try
                {
                    if (!act((StepByStepRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StepByStepRecord GetStepByStep(int nId)
        {
            IRecord tbStepByStep;
            if (!StepByStep.TryGetValue(nId, out tbStepByStep))
            {
                Logger.Info("StepByStep[{0}] not find by Table", nId);
                return null;
            }
            return (StepByStepRecord)tbStepByStep;
        }
        public static void ForeachWorldBOSS(Func<WorldBOSSRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach WorldBOSS act is null");
                return;
            }
            foreach (var tempRecord in WorldBOSS)
            {
                try
                {
                    if (!act((WorldBOSSRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static WorldBOSSRecord GetWorldBOSS(int nId)
        {
            IRecord tbWorldBOSS;
            if (!WorldBOSS.TryGetValue(nId, out tbWorldBOSS))
            {
                Logger.Info("WorldBOSS[{0}] not find by Table", nId);
                return null;
            }
            return (WorldBOSSRecord)tbWorldBOSS;
        }
        public static void ForeachPKValue(Func<PKValueRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach PKValue act is null");
                return;
            }
            foreach (var tempRecord in PKValue)
            {
                try
                {
                    if (!act((PKValueRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static PKValueRecord GetPKValue(int nId)
        {
            IRecord tbPKValue;
            if (!PKValue.TryGetValue(nId, out tbPKValue))
            {
                Logger.Info("PKValue[{0}] not find by Table", nId);
                return null;
            }
            return (PKValueRecord)tbPKValue;
        }
        public static void ForeachTransmigration(Func<TransmigrationRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Transmigration act is null");
                return;
            }
            foreach (var tempRecord in Transmigration)
            {
                try
                {
                    if (!act((TransmigrationRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static TransmigrationRecord GetTransmigration(int nId)
        {
            IRecord tbTransmigration;
            if (!Transmigration.TryGetValue(nId, out tbTransmigration))
            {
                Logger.Info("Transmigration[{0}] not find by Table", nId);
                return null;
            }
            return (TransmigrationRecord)tbTransmigration;
        }
        public static void ForeachAttachPoint(Func<AttachPointRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach AttachPoint act is null");
                return;
            }
            foreach (var tempRecord in AttachPoint)
            {
                try
                {
                    if (!act((AttachPointRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AttachPointRecord GetAttachPoint(int nId)
        {
            IRecord tbAttachPoint;
            if (!AttachPoint.TryGetValue(nId, out tbAttachPoint))
            {
                Logger.Info("AttachPoint[{0}] not find by Table", nId);
                return null;
            }
            return (AttachPointRecord)tbAttachPoint;
        }
        public static void ForeachFubenInfo(Func<FubenInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach FubenInfo act is null");
                return;
            }
            foreach (var tempRecord in FubenInfo)
            {
                try
                {
                    if (!act((FubenInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static FubenInfoRecord GetFubenInfo(int nId)
        {
            IRecord tbFubenInfo;
            if (!FubenInfo.TryGetValue(nId, out tbFubenInfo))
            {
                Logger.Info("FubenInfo[{0}] not find by Table", nId);
                return null;
            }
            return (FubenInfoRecord)tbFubenInfo;
        }
        public static void ForeachFubenLogic(Func<FubenLogicRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach FubenLogic act is null");
                return;
            }
            foreach (var tempRecord in FubenLogic)
            {
                try
                {
                    if (!act((FubenLogicRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static FubenLogicRecord GetFubenLogic(int nId)
        {
            IRecord tbFubenLogic;
            if (!FubenLogic.TryGetValue(nId, out tbFubenLogic))
            {
                Logger.Info("FubenLogic[{0}] not find by Table", nId);
                return null;
            }
            return (FubenLogicRecord)tbFubenLogic;
        }
        public static void ForeachFace(Func<FaceRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Face act is null");
                return;
            }
            foreach (var tempRecord in Face)
            {
                try
                {
                    if (!act((FaceRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static FaceRecord GetFace(int nId)
        {
            IRecord tbFace;
            if (!Face.TryGetValue(nId, out tbFace))
            {
                Logger.Info("Face[{0}] not find by Table", nId);
                return null;
            }
            return (FaceRecord)tbFace;
        }
        public static void ForeachServerName(Func<ServerNameRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ServerName act is null");
                return;
            }
            foreach (var tempRecord in ServerName)
            {
                try
                {
                    if (!act((ServerNameRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ServerNameRecord GetServerName(int nId)
        {
            IRecord tbServerName;
            if (!ServerName.TryGetValue(nId, out tbServerName))
            {
                Logger.Info("ServerName[{0}] not find by Table", nId);
                return null;
            }
            return (ServerNameRecord)tbServerName;
        }
        public static void ForeachLoadingTest(Func<LoadingTestRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach LoadingTest act is null");
                return;
            }
            foreach (var tempRecord in LoadingTest)
            {
                try
                {
                    if (!act((LoadingTestRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static LoadingTestRecord GetLoadingTest(int nId)
        {
            IRecord tbLoadingTest;
            if (!LoadingTest.TryGetValue(nId, out tbLoadingTest))
            {
                Logger.Info("LoadingTest[{0}] not find by Table", nId);
                return null;
            }
            return (LoadingTestRecord)tbLoadingTest;
        }
        public static void ForeachGetMissionInfo(Func<GetMissionInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GetMissionInfo act is null");
                return;
            }
            foreach (var tempRecord in GetMissionInfo)
            {
                try
                {
                    if (!act((GetMissionInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GetMissionInfoRecord GetGetMissionInfo(int nId)
        {
            IRecord tbGetMissionInfo;
            if (!GetMissionInfo.TryGetValue(nId, out tbGetMissionInfo))
            {
                Logger.Info("GetMissionInfo[{0}] not find by Table", nId);
                return null;
            }
            return (GetMissionInfoRecord)tbGetMissionInfo;
        }
        public static void ForeachMissionConditionInfo(Func<MissionConditionInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MissionConditionInfo act is null");
                return;
            }
            foreach (var tempRecord in MissionConditionInfo)
            {
                try
                {
                    if (!act((MissionConditionInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MissionConditionInfoRecord GetMissionConditionInfo(int nId)
        {
            IRecord tbMissionConditionInfo;
            if (!MissionConditionInfo.TryGetValue(nId, out tbMissionConditionInfo))
            {
                Logger.Info("MissionConditionInfo[{0}] not find by Table", nId);
                return null;
            }
            return (MissionConditionInfoRecord)tbMissionConditionInfo;
        }
        public static void ForeachGetMissionReward(Func<GetMissionRewardRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GetMissionReward act is null");
                return;
            }
            foreach (var tempRecord in GetMissionReward)
            {
                try
                {
                    if (!act((GetMissionRewardRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GetMissionRewardRecord GetGetMissionReward(int nId)
        {
            IRecord tbGetMissionReward;
            if (!GetMissionReward.TryGetValue(nId, out tbGetMissionReward))
            {
                Logger.Info("GetMissionReward[{0}] not find by Table", nId);
                return null;
            }
            return (GetMissionRewardRecord)tbGetMissionReward;
        }
        public static void ForeachGetMissionIcon(Func<GetMissionIconRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GetMissionIcon act is null");
                return;
            }
            foreach (var tempRecord in GetMissionIcon)
            {
                try
                {
                    if (!act((GetMissionIconRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GetMissionIconRecord GetGetMissionIcon(int nId)
        {
            IRecord tbGetMissionIcon;
            if (!GetMissionIcon.TryGetValue(nId, out tbGetMissionIcon))
            {
                Logger.Info("GetMissionIcon[{0}] not find by Table", nId);
                return null;
            }
            return (GetMissionIconRecord)tbGetMissionIcon;
        }
        public static void ForeachSubject(Func<SubjectRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Subject act is null");
                return;
            }
            foreach (var tempRecord in Subject)
            {
                try
                {
                    if (!act((SubjectRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static SubjectRecord GetSubject(int nId)
        {
            IRecord tbSubject;
            if (!Subject.TryGetValue(nId, out tbSubject))
            {
                Logger.Info("Subject[{0}] not find by Table", nId);
                return null;
            }
            return (SubjectRecord)tbSubject;
        }
        public static void ForeachItemGetInfo(Func<ItemGetInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ItemGetInfo act is null");
                return;
            }
            foreach (var tempRecord in ItemGetInfo)
            {
                try
                {
                    if (!act((ItemGetInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ItemGetInfoRecord GetItemGetInfo(int nId)
        {
            IRecord tbItemGetInfo;
            if (!ItemGetInfo.TryGetValue(nId, out tbItemGetInfo))
            {
                Logger.Info("ItemGetInfo[{0}] not find by Table", nId);
                return null;
            }
            return (ItemGetInfoRecord)tbItemGetInfo;
        }
        public static void ForeachDynamicActivity(Func<DynamicActivityRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach DynamicActivity act is null");
                return;
            }
            foreach (var tempRecord in DynamicActivity)
            {
                try
                {
                    if (!act((DynamicActivityRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DynamicActivityRecord GetDynamicActivity(int nId)
        {
            IRecord tbDynamicActivity;
            if (!DynamicActivity.TryGetValue(nId, out tbDynamicActivity))
            {
                Logger.Info("DynamicActivity[{0}] not find by Table", nId);
                return null;
            }
            return (DynamicActivityRecord)tbDynamicActivity;
        }
        public static void ForeachCompensation(Func<CompensationRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Compensation act is null");
                return;
            }
            foreach (var tempRecord in Compensation)
            {
                try
                {
                    if (!act((CompensationRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static CompensationRecord GetCompensation(int nId)
        {
            IRecord tbCompensation;
            if (!Compensation.TryGetValue(nId, out tbCompensation))
            {
                Logger.Info("Compensation[{0}] not find by Table", nId);
                return null;
            }
            return (CompensationRecord)tbCompensation;
        }
        public static void ForeachCityTalk(Func<CityTalkRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach CityTalk act is null");
                return;
            }
            foreach (var tempRecord in CityTalk)
            {
                try
                {
                    if (!act((CityTalkRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static CityTalkRecord GetCityTalk(int nId)
        {
            IRecord tbCityTalk;
            if (!CityTalk.TryGetValue(nId, out tbCityTalk))
            {
                Logger.Info("CityTalk[{0}] not find by Table", nId);
                return null;
            }
            return (CityTalkRecord)tbCityTalk;
        }
        public static void ForeachDailyActivity(Func<DailyActivityRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach DailyActivity act is null");
                return;
            }
            foreach (var tempRecord in DailyActivity)
            {
                try
                {
                    if (!act((DailyActivityRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DailyActivityRecord GetDailyActivity(int nId)
        {
            IRecord tbDailyActivity;
            if (!DailyActivity.TryGetValue(nId, out tbDailyActivity))
            {
                Logger.Info("DailyActivity[{0}] not find by Table", nId);
                return null;
            }
            return (DailyActivityRecord)tbDailyActivity;
        }
        public static void ForeachRecharge(Func<RechargeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Recharge act is null");
                return;
            }
            foreach (var tempRecord in Recharge)
            {
                try
                {
                    if (!act((RechargeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static RechargeRecord GetRecharge(int nId)
        {
            IRecord tbRecharge;
            if (!Recharge.TryGetValue(nId, out tbRecharge))
            {
                Logger.Info("Recharge[{0}] not find by Table", nId);
                return null;
            }
            return (RechargeRecord)tbRecharge;
        }
        public static void ForeachRewardInfo(Func<RewardInfoRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach RewardInfo act is null");
                return;
            }
            foreach (var tempRecord in RewardInfo)
            {
                try
                {
                    if (!act((RewardInfoRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static RewardInfoRecord GetRewardInfo(int nId)
        {
            IRecord tbRewardInfo;
            if (!RewardInfo.TryGetValue(nId, out tbRewardInfo))
            {
                Logger.Info("RewardInfo[{0}] not find by Table", nId);
                return null;
            }
            return (RewardInfoRecord)tbRewardInfo;
        }
        public static void ForeachNameTitle(Func<NameTitleRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach NameTitle act is null");
                return;
            }
            foreach (var tempRecord in NameTitle)
            {
                try
                {
                    if (!act((NameTitleRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static NameTitleRecord GetNameTitle(int nId)
        {
            IRecord tbNameTitle;
            if (!NameTitle.TryGetValue(nId, out tbNameTitle))
            {
                Logger.Info("NameTitle[{0}] not find by Table", nId);
                return null;
            }
            return (NameTitleRecord)tbNameTitle;
        }
        public static void ForeachVIP(Func<VIPRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach VIP act is null");
                return;
            }
            foreach (var tempRecord in VIP)
            {
                try
                {
                    if (!act((VIPRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static VIPRecord GetVIP(int nId)
        {
            IRecord tbVIP;
            if (!VIP.TryGetValue(nId, out tbVIP))
            {
                Logger.Info("VIP[{0}] not find by Table", nId);
                return null;
            }
            return (VIPRecord)tbVIP;
        }
        public static void ForeachLevelupTips(Func<LevelupTipsRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach LevelupTips act is null");
                return;
            }
            foreach (var tempRecord in LevelupTips)
            {
                try
                {
                    if (!act((LevelupTipsRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static LevelupTipsRecord GetLevelupTips(int nId)
        {
            IRecord tbLevelupTips;
            if (!LevelupTips.TryGetValue(nId, out tbLevelupTips))
            {
                Logger.Info("LevelupTips[{0}] not find by Table", nId);
                return null;
            }
            return (LevelupTipsRecord)tbLevelupTips;
        }
        public static void ForeachStrongType(Func<StrongTypeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach StrongType act is null");
                return;
            }
            foreach (var tempRecord in StrongType)
            {
                try
                {
                    if (!act((StrongTypeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StrongTypeRecord GetStrongType(int nId)
        {
            IRecord tbStrongType;
            if (!StrongType.TryGetValue(nId, out tbStrongType))
            {
                Logger.Info("StrongType[{0}] not find by Table", nId);
                return null;
            }
            return (StrongTypeRecord)tbStrongType;
        }
        public static void ForeachStrongData(Func<StrongDataRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach StrongData act is null");
                return;
            }
            foreach (var tempRecord in StrongData)
            {
                try
                {
                    if (!act((StrongDataRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static StrongDataRecord GetStrongData(int nId)
        {
            IRecord tbStrongData;
            if (!StrongData.TryGetValue(nId, out tbStrongData))
            {
                Logger.Info("StrongData[{0}] not find by Table", nId);
                return null;
            }
            return (StrongDataRecord)tbStrongData;
        }
        public static void ForeachMail(Func<MailRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach Mail act is null");
                return;
            }
            foreach (var tempRecord in Mail)
            {
                try
                {
                    if (!act((MailRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MailRecord GetMail(int nId)
        {
            IRecord tbMail;
            if (!Mail.TryGetValue(nId, out tbMail))
            {
                Logger.Info("Mail[{0}] not find by Table", nId);
                return null;
            }
            return (MailRecord)tbMail;
        }
        public static void ForeachGMCommand(Func<GMCommandRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach GMCommand act is null");
                return;
            }
            foreach (var tempRecord in GMCommand)
            {
                try
                {
                    if (!act((GMCommandRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static GMCommandRecord GetGMCommand(int nId)
        {
            IRecord tbGMCommand;
            if (!GMCommand.TryGetValue(nId, out tbGMCommand))
            {
                Logger.Info("GMCommand[{0}] not find by Table", nId);
                return null;
            }
            return (GMCommandRecord)tbGMCommand;
        }
        public static void ForeachAuctionType1(Func<AuctionType1Record, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach AuctionType1 act is null");
                return;
            }
            foreach (var tempRecord in AuctionType1)
            {
                try
                {
                    if (!act((AuctionType1Record)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AuctionType1Record GetAuctionType1(int nId)
        {
            IRecord tbAuctionType1;
            if (!AuctionType1.TryGetValue(nId, out tbAuctionType1))
            {
                Logger.Info("AuctionType1[{0}] not find by Table", nId);
                return null;
            }
            return (AuctionType1Record)tbAuctionType1;
        }
        public static void ForeachAuctionType2(Func<AuctionType2Record, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach AuctionType2 act is null");
                return;
            }
            foreach (var tempRecord in AuctionType2)
            {
                try
                {
                    if (!act((AuctionType2Record)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AuctionType2Record GetAuctionType2(int nId)
        {
            IRecord tbAuctionType2;
            if (!AuctionType2.TryGetValue(nId, out tbAuctionType2))
            {
                Logger.Info("AuctionType2[{0}] not find by Table", nId);
                return null;
            }
            return (AuctionType2Record)tbAuctionType2;
        }
        public static void ForeachAuctionType3(Func<AuctionType3Record, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach AuctionType3 act is null");
                return;
            }
            foreach (var tempRecord in AuctionType3)
            {
                try
                {
                    if (!act((AuctionType3Record)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AuctionType3Record GetAuctionType3(int nId)
        {
            IRecord tbAuctionType3;
            if (!AuctionType3.TryGetValue(nId, out tbAuctionType3))
            {
                Logger.Info("AuctionType3[{0}] not find by Table", nId);
                return null;
            }
            return (AuctionType3Record)tbAuctionType3;
        }
        public static void ForeachFirstRecharge(Func<FirstRechargeRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach FirstRecharge act is null");
                return;
            }
            foreach (var tempRecord in FirstRecharge)
            {
                try
                {
                    if (!act((FirstRechargeRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static FirstRechargeRecord GetFirstRecharge(int nId)
        {
            IRecord tbFirstRecharge;
            if (!FirstRecharge.TryGetValue(nId, out tbFirstRecharge))
            {
                Logger.Info("FirstRecharge[{0}] not find by Table", nId);
                return null;
            }
            return (FirstRechargeRecord)tbFirstRecharge;
        }
        public static void ForeachMieShi(Func<MieShiRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MieShi act is null");
                return;
            }
            foreach (var tempRecord in MieShi)
            {
                try
                {
                    if (!act((MieShiRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MieShiRecord GetMieShi(int nId)
        {
            IRecord tbMieShi;
            if (!MieShi.TryGetValue(nId, out tbMieShi))
            {
                Logger.Info("MieShi[{0}] not find by Table", nId);
                return null;
            }
            return (MieShiRecord)tbMieShi;
        }
        public static void ForeachMieShiPublic(Func<MieShiPublicRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MieShiPublic act is null");
                return;
            }
            foreach (var tempRecord in MieShiPublic)
            {
                try
                {
                    if (!act((MieShiPublicRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MieShiPublicRecord GetMieShiPublic(int nId)
        {
            IRecord tbMieShiPublic;
            if (!MieShiPublic.TryGetValue(nId, out tbMieShiPublic))
            {
                Logger.Info("MieShiPublic[{0}] not find by Table", nId);
                return null;
            }
            return (MieShiPublicRecord)tbMieShiPublic;
        }
        public static void ForeachDefendCityReward(Func<DefendCityRewardRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach DefendCityReward act is null");
                return;
            }
            foreach (var tempRecord in DefendCityReward)
            {
                try
                {
                    if (!act((DefendCityRewardRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DefendCityRewardRecord GetDefendCityReward(int nId)
        {
            IRecord tbDefendCityReward;
            if (!DefendCityReward.TryGetValue(nId, out tbDefendCityReward))
            {
                Logger.Info("DefendCityReward[{0}] not find by Table", nId);
                return null;
            }
            return (DefendCityRewardRecord)tbDefendCityReward;
        }
        public static void ForeachDefendCityDevoteReward(Func<DefendCityDevoteRewardRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach DefendCityDevoteReward act is null");
                return;
            }
            foreach (var tempRecord in DefendCityDevoteReward)
            {
                try
                {
                    if (!act((DefendCityDevoteRewardRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static DefendCityDevoteRewardRecord GetDefendCityDevoteReward(int nId)
        {
            IRecord tbDefendCityDevoteReward;
            if (!DefendCityDevoteReward.TryGetValue(nId, out tbDefendCityDevoteReward))
            {
                Logger.Info("DefendCityDevoteReward[{0}] not find by Table", nId);
                return null;
            }
            return (DefendCityDevoteRewardRecord)tbDefendCityDevoteReward;
        }
        public static void ForeachBatteryLevel(Func<BatteryLevelRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BatteryLevel act is null");
                return;
            }
            foreach (var tempRecord in BatteryLevel)
            {
                try
                {
                    if (!act((BatteryLevelRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BatteryLevelRecord GetBatteryLevel(int nId)
        {
            IRecord tbBatteryLevel;
            if (!BatteryLevel.TryGetValue(nId, out tbBatteryLevel))
            {
                Logger.Info("BatteryLevel[{0}] not find by Table", nId);
                return null;
            }
            return (BatteryLevelRecord)tbBatteryLevel;
        }
        public static void ForeachBatteryBase(Func<BatteryBaseRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BatteryBase act is null");
                return;
            }
            foreach (var tempRecord in BatteryBase)
            {
                try
                {
                    if (!act((BatteryBaseRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BatteryBaseRecord GetBatteryBase(int nId)
        {
            IRecord tbBatteryBase;
            if (!BatteryBase.TryGetValue(nId, out tbBatteryBase))
            {
                Logger.Info("BatteryBase[{0}] not find by Table", nId);
                return null;
            }
            return (BatteryBaseRecord)tbBatteryBase;
        }
        public static void ForeachMieShiFighting(Func<MieShiFightingRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MieShiFighting act is null");
                return;
            }
            foreach (var tempRecord in MieShiFighting)
            {
                try
                {
                    if (!act((MieShiFightingRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MieShiFightingRecord GetMieShiFighting(int nId)
        {
            IRecord tbMieShiFighting;
            if (!MieShiFighting.TryGetValue(nId, out tbMieShiFighting))
            {
                Logger.Info("MieShiFighting[{0}] not find by Table", nId);
                return null;
            }
            return (MieShiFightingRecord)tbMieShiFighting;
        }
        public static void ForeachFunctionOn(Func<FunctionOnRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach FunctionOn act is null");
                return;
            }
            foreach (var tempRecord in FunctionOn)
            {
                try
                {
                    if (!act((FunctionOnRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static FunctionOnRecord GetFunctionOn(int nId)
        {
            IRecord tbFunctionOn;
            if (!FunctionOn.TryGetValue(nId, out tbFunctionOn))
            {
                Logger.Info("FunctionOn[{0}] not find by Table", nId);
                return null;
            }
            return (FunctionOnRecord)tbFunctionOn;
        }
        public static void ForeachBangBuff(Func<BangBuffRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BangBuff act is null");
                return;
            }
            foreach (var tempRecord in BangBuff)
            {
                try
                {
                    if (!act((BangBuffRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BangBuffRecord GetBangBuff(int nId)
        {
            IRecord tbBangBuff;
            if (!BangBuff.TryGetValue(nId, out tbBangBuff))
            {
                Logger.Info("BangBuff[{0}] not find by Table", nId);
                return null;
            }
            return (BangBuffRecord)tbBangBuff;
        }
        public static void ForeachBuffGroup(Func<BuffGroupRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach BuffGroup act is null");
                return;
            }
            foreach (var tempRecord in BuffGroup)
            {
                try
                {
                    if (!act((BuffGroupRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static BuffGroupRecord GetBuffGroup(int nId)
        {
            IRecord tbBuffGroup;
            if (!BuffGroup.TryGetValue(nId, out tbBuffGroup))
            {
                Logger.Info("BuffGroup[{0}] not find by Table", nId);
                return null;
            }
            return (BuffGroupRecord)tbBuffGroup;
        }
        public static void ForeachMieshiTowerReward(Func<MieshiTowerRewardRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach MieshiTowerReward act is null");
                return;
            }
            foreach (var tempRecord in MieshiTowerReward)
            {
                try
                {
                    if (!act((MieshiTowerRewardRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static MieshiTowerRewardRecord GetMieshiTowerReward(int nId)
        {
            IRecord tbMieshiTowerReward;
            if (!MieshiTowerReward.TryGetValue(nId, out tbMieshiTowerReward))
            {
                Logger.Info("MieshiTowerReward[{0}] not find by Table", nId);
                return null;
            }
            return (MieshiTowerRewardRecord)tbMieshiTowerReward;
        }
        public static void ForeachClimbingTower(Func<ClimbingTowerRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ClimbingTower act is null");
                return;
            }
            foreach (var tempRecord in ClimbingTower)
            {
                try
                {
                    if (!act((ClimbingTowerRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ClimbingTowerRecord GetClimbingTower(int nId)
        {
            IRecord tbClimbingTower;
            if (!ClimbingTower.TryGetValue(nId, out tbClimbingTower))
            {
                Logger.Info("ClimbingTower[{0}] not find by Table", nId);
                return null;
            }
            return (ClimbingTowerRecord)tbClimbingTower;
        }
        public static void ForeachAcientBattleField(Func<AcientBattleFieldRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach AcientBattleField act is null");
                return;
            }
            foreach (var tempRecord in AcientBattleField)
            {
                try
                {
                    if (!act((AcientBattleFieldRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static AcientBattleFieldRecord GetAcientBattleField(int nId)
        {
            IRecord tbAcientBattleField;
            if (!AcientBattleField.TryGetValue(nId, out tbAcientBattleField))
            {
                Logger.Info("AcientBattleField[{0}] not find by Table", nId);
                return null;
            }
            return (AcientBattleFieldRecord)tbAcientBattleField;
        }
        public static void ForeachElfStarShader(Func<ElfStarShaderRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ElfStarShader act is null");
                return;
            }
            foreach (var tempRecord in ElfStarShader)
            {
                try
                {
                    if (!act((ElfStarShaderRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ElfStarShaderRecord GetElfStarShader(int nId)
        {
            IRecord tbElfStarShader;
            if (!ElfStarShader.TryGetValue(nId, out tbElfStarShader))
            {
                Logger.Info("ElfStarShader[{0}] not find by Table", nId);
                return null;
            }
            return (ElfStarShaderRecord)tbElfStarShader;
        }
        public static void ForeachConsumArray(Func<ConsumArrayRecord, bool> act)
        {
            if (act == null)
            {
                Logger.Error("Foreach ConsumArray act is null");
                return;
            }
            foreach (var tempRecord in ConsumArray)
            {
                try
                {
                    if (!act((ConsumArrayRecord)tempRecord.Value))
                        break;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        public static ConsumArrayRecord GetConsumArray(int nId)
        {
            IRecord tbConsumArray;
            if (!ConsumArray.TryGetValue(nId, out tbConsumArray))
            {
                Logger.Info("ConsumArray[{0}] not find by Table", nId);
                return null;
            }
            return (ConsumArrayRecord)tbConsumArray;
        }
    }
    public class IconRecord :IRecord
    {
        public static string __TableName = "Icon.txt";
        public int Id { get; private set; }
        public string Atlas { get; private set; }
        public string Sprite { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Atlas = temp[__column++];
                Sprite = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((IconRecord)_this).Id;
        }
        static object get_Atlas(IRecord _this)
        {
            return ((IconRecord)_this).Atlas;
        }
        static object get_Sprite(IRecord _this)
        {
            return ((IconRecord)_this).Sprite;
        }
        static IconRecord()
        {
            mField["Id"] = get_Id;
            mField["Atlas"] = get_Atlas;
            mField["Sprite"] = get_Sprite;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SoundRecord :IRecord
    {
        public static string __TableName = "Sound.txt";
        public int Id { get; private set; }
        public string FullPathName { get; private set; }
        public float PanLevel { get;        set; }
        public float Volume { get;        set; }
        public float MinDistance { get;        set; }
        public float Spread { get;        set; }
        public int IsLoop { get; private set; }
        public float Delay { get;        set; }
        public float FadeInTime { get;        set; }
        public float FadeOutTime { get;        set; }
        public int CurMaxPlayingCount { get; private set; }
        public int Priority { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                FullPathName = temp[__column++];
                PanLevel = Table_Tamplet.Convert_Float(temp[__column++]);
                Volume = Table_Tamplet.Convert_Float(temp[__column++]);
                MinDistance = Table_Tamplet.Convert_Float(temp[__column++]);
                Spread = Table_Tamplet.Convert_Float(temp[__column++]);
                IsLoop = Table_Tamplet.Convert_Int(temp[__column++]);
                Delay = Table_Tamplet.Convert_Float(temp[__column++]);
                FadeInTime = Table_Tamplet.Convert_Float(temp[__column++]);
                FadeOutTime = Table_Tamplet.Convert_Float(temp[__column++]);
                CurMaxPlayingCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Priority = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SoundRecord)_this).Id;
        }
        static object get_FullPathName(IRecord _this)
        {
            return ((SoundRecord)_this).FullPathName;
        }
        static object get_PanLevel(IRecord _this)
        {
            return ((SoundRecord)_this).PanLevel;
        }
        static object get_Volume(IRecord _this)
        {
            return ((SoundRecord)_this).Volume;
        }
        static object get_MinDistance(IRecord _this)
        {
            return ((SoundRecord)_this).MinDistance;
        }
        static object get_Spread(IRecord _this)
        {
            return ((SoundRecord)_this).Spread;
        }
        static object get_IsLoop(IRecord _this)
        {
            return ((SoundRecord)_this).IsLoop;
        }
        static object get_Delay(IRecord _this)
        {
            return ((SoundRecord)_this).Delay;
        }
        static object get_FadeInTime(IRecord _this)
        {
            return ((SoundRecord)_this).FadeInTime;
        }
        static object get_FadeOutTime(IRecord _this)
        {
            return ((SoundRecord)_this).FadeOutTime;
        }
        static object get_CurMaxPlayingCount(IRecord _this)
        {
            return ((SoundRecord)_this).CurMaxPlayingCount;
        }
        static object get_Priority(IRecord _this)
        {
            return ((SoundRecord)_this).Priority;
        }
        static SoundRecord()
        {
            mField["Id"] = get_Id;
            mField["FullPathName"] = get_FullPathName;
            mField["PanLevel"] = get_PanLevel;
            mField["Volume"] = get_Volume;
            mField["MinDistance"] = get_MinDistance;
            mField["Spread"] = get_Spread;
            mField["IsLoop"] = get_IsLoop;
            mField["Delay"] = get_Delay;
            mField["FadeInTime"] = get_FadeInTime;
            mField["FadeOutTime"] = get_FadeOutTime;
            mField["CurMaxPlayingCount"] = get_CurMaxPlayingCount;
            mField["Priority"] = get_Priority;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ConditionTableRecord :IRecord
    {
        public static string __TableName = "ConditionTable.txt";
        public int Id { get; private set; }
        public int[] TrueFlag = new int[1];
        public int FlagTrueDict { get; private set; }
        public int[] FalseFlag = new int[1];
        public int FlagFalseDict { get; private set; }
        public int[] ExdataId = new int[4];
        public int[] ExdataMin = new int[4];
        public int[] ExdataMax = new int[4];
        public int[] ExdataDict = new int[4];
        public int[] ItemId = new int[4];
        public int[] ItemCountMin = new int[4];
        public int[] ItemCountMax = new int[4];
        public int[] ItemDict = new int[4];
        public int Role { get; private set; }
        public int RoleDict { get; private set; }
        public int[] OpenTime = new int[2];
        public int[] OpenTimeDict = new int[2];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                TrueFlag[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagTrueDict = Table_Tamplet.Convert_Int(temp[__column++]);
                FalseFlag[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagFalseDict = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMin[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMax[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataDict[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMin[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMax[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataDict[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMin[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMax[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataDict[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMin[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataMax[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataDict[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMin[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMax[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDict[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMin[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMax[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDict[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMin[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMax[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDict[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMin[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCountMax[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDict[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Role = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleDict = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenTime[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenTimeDict[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenTime[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenTimeDict[1] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ConditionTableRecord)_this).Id;
        }
        static object get_TrueFlag(IRecord _this)
        {
            return ((ConditionTableRecord)_this).TrueFlag;
        }
        static object get_FlagTrueDict(IRecord _this)
        {
            return ((ConditionTableRecord)_this).FlagTrueDict;
        }
        static object get_FalseFlag(IRecord _this)
        {
            return ((ConditionTableRecord)_this).FalseFlag;
        }
        static object get_FlagFalseDict(IRecord _this)
        {
            return ((ConditionTableRecord)_this).FlagFalseDict;
        }
        static object get_ExdataId(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ExdataId;
        }
        static object get_ExdataMin(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ExdataMin;
        }
        static object get_ExdataMax(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ExdataMax;
        }
        static object get_ExdataDict(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ExdataDict;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ItemId;
        }
        static object get_ItemCountMin(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ItemCountMin;
        }
        static object get_ItemCountMax(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ItemCountMax;
        }
        static object get_ItemDict(IRecord _this)
        {
            return ((ConditionTableRecord)_this).ItemDict;
        }
        static object get_Role(IRecord _this)
        {
            return ((ConditionTableRecord)_this).Role;
        }
        static object get_RoleDict(IRecord _this)
        {
            return ((ConditionTableRecord)_this).RoleDict;
        }
        static object get_OpenTime(IRecord _this)
        {
            return ((ConditionTableRecord)_this).OpenTime;
        }
        static object get_OpenTimeDict(IRecord _this)
        {
            return ((ConditionTableRecord)_this).OpenTimeDict;
        }
        static ConditionTableRecord()
        {
            mField["Id"] = get_Id;
            mField["TrueFlag"] = get_TrueFlag;
            mField["FlagTrueDict"] = get_FlagTrueDict;
            mField["FalseFlag"] = get_FalseFlag;
            mField["FlagFalseDict"] = get_FlagFalseDict;
            mField["ExdataId"] = get_ExdataId;
            mField["ExdataMin"] = get_ExdataMin;
            mField["ExdataMax"] = get_ExdataMax;
            mField["ExdataDict"] = get_ExdataDict;
            mField["ItemId"] = get_ItemId;
            mField["ItemCountMin"] = get_ItemCountMin;
            mField["ItemCountMax"] = get_ItemCountMax;
            mField["ItemDict"] = get_ItemDict;
            mField["Role"] = get_Role;
            mField["RoleDict"] = get_RoleDict;
            mField["OpenTime"] = get_OpenTime;
            mField["OpenTimeDict"] = get_OpenTimeDict;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ExdataRecord :IRecord
    {
        public static string __TableName = "Exdata.txt";
        public int Id { get; private set; }
        public int InitValue { get; private set; }
        public int Change { get; private set; }
        public int RefreshRule { get; private set; }
        public int[] RefreshValue = new int[2];
        public int IsRefresh { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                InitValue = Table_Tamplet.Convert_Int(temp[__column++]);
                Change = Table_Tamplet.Convert_Int(temp[__column++]);
                RefreshRule = Table_Tamplet.Convert_Int(temp[__column++]);
                RefreshValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RefreshValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                IsRefresh = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ExdataRecord)_this).Id;
        }
        static object get_InitValue(IRecord _this)
        {
            return ((ExdataRecord)_this).InitValue;
        }
        static object get_Change(IRecord _this)
        {
            return ((ExdataRecord)_this).Change;
        }
        static object get_RefreshRule(IRecord _this)
        {
            return ((ExdataRecord)_this).RefreshRule;
        }
        static object get_RefreshValue(IRecord _this)
        {
            return ((ExdataRecord)_this).RefreshValue;
        }
        static object get_IsRefresh(IRecord _this)
        {
            return ((ExdataRecord)_this).IsRefresh;
        }
        static ExdataRecord()
        {
            mField["Id"] = get_Id;
            mField["InitValue"] = get_InitValue;
            mField["Change"] = get_Change;
            mField["RefreshRule"] = get_RefreshRule;
            mField["RefreshValue"] = get_RefreshValue;
            mField["IsRefresh"] = get_IsRefresh;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DictionaryRecord :IRecord
    {
        public static string __TableName = "Dictionary.txt";
        public int Id { get; private set; }
        public string[] Desc = new string[2];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc[0]  = Table_Tamplet.Convert_String(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DictionaryRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((DictionaryRecord)_this).Desc;
        }
        static DictionaryRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SceneNpcRecord :IRecord
    {
        public static string __TableName = "SceneNpc.txt";
        public int Id { get; private set; }
        public int DataID { get; private set; }
        [TableBinding("Scene")]
        public int SceneID { get; private set; }
        public double PosX { get; private set; }
        public double PosZ { get; private set; }
        public double FaceDirection { get; private set; }
        public int ViewMiniMap { get; private set; }
        public int RandomStartID { get; private set; }
        public int RandomEndID { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                DataID = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneID = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Double(temp[__column++]);
                PosZ = Table_Tamplet.Convert_Double(temp[__column++]);
                FaceDirection = Table_Tamplet.Convert_Double(temp[__column++]);
                ViewMiniMap = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomStartID = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomEndID = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SceneNpcRecord)_this).Id;
        }
        static object get_DataID(IRecord _this)
        {
            return ((SceneNpcRecord)_this).DataID;
        }
        static object get_SceneID(IRecord _this)
        {
            return ((SceneNpcRecord)_this).SceneID;
        }
        static object get_PosX(IRecord _this)
        {
            return ((SceneNpcRecord)_this).PosX;
        }
        static object get_PosZ(IRecord _this)
        {
            return ((SceneNpcRecord)_this).PosZ;
        }
        static object get_FaceDirection(IRecord _this)
        {
            return ((SceneNpcRecord)_this).FaceDirection;
        }
        static object get_ViewMiniMap(IRecord _this)
        {
            return ((SceneNpcRecord)_this).ViewMiniMap;
        }
        static object get_RandomStartID(IRecord _this)
        {
            return ((SceneNpcRecord)_this).RandomStartID;
        }
        static object get_RandomEndID(IRecord _this)
        {
            return ((SceneNpcRecord)_this).RandomEndID;
        }
        static SceneNpcRecord()
        {
            mField["Id"] = get_Id;
            mField["DataID"] = get_DataID;
            mField["SceneID"] = get_SceneID;
            mField["PosX"] = get_PosX;
            mField["PosZ"] = get_PosZ;
            mField["FaceDirection"] = get_FaceDirection;
            mField["ViewMiniMap"] = get_ViewMiniMap;
            mField["RandomStartID"] = get_RandomStartID;
            mField["RandomEndID"] = get_RandomEndID;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class CharModelRecord :IRecord
    {
        public static string __TableName = "CharModel.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string ResPath { get; private set; }
        public string AnimPath { get; private set; }
        public string HeadPic { get; private set; }
        public string NPCSpriteName { get; private set; }
        public double HeadInfoHeight { get; private set; }
        public double Scale { get; private set; }
        public int DeadSound { get; private set; }
        public float ShadowSize { get;        set; }
        public int RefreshAnimation { get; private set; }
        public int DieAnimation { get; private set; }
        public int BornEffectId { get; private set; }
        public int DieEffectId { get; private set; }
        public int RandomStand { get; private set; }
        public int WingTop { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                ResPath = temp[__column++];
                AnimPath = temp[__column++];
                HeadPic = temp[__column++];
                NPCSpriteName = temp[__column++];
                HeadInfoHeight = Table_Tamplet.Convert_Double(temp[__column++]);
                Scale = Table_Tamplet.Convert_Double(temp[__column++]);
                DeadSound = Table_Tamplet.Convert_Int(temp[__column++]);
                ShadowSize = Table_Tamplet.Convert_Float(temp[__column++]);
                RefreshAnimation = Table_Tamplet.Convert_Int(temp[__column++]);
                DieAnimation = Table_Tamplet.Convert_Int(temp[__column++]);
                BornEffectId = Table_Tamplet.Convert_Int(temp[__column++]);
                DieEffectId = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomStand = Table_Tamplet.Convert_Int(temp[__column++]);
                WingTop = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((CharModelRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((CharModelRecord)_this).Name;
        }
        static object get_ResPath(IRecord _this)
        {
            return ((CharModelRecord)_this).ResPath;
        }
        static object get_AnimPath(IRecord _this)
        {
            return ((CharModelRecord)_this).AnimPath;
        }
        static object get_HeadPic(IRecord _this)
        {
            return ((CharModelRecord)_this).HeadPic;
        }
        static object get_NPCSpriteName(IRecord _this)
        {
            return ((CharModelRecord)_this).NPCSpriteName;
        }
        static object get_HeadInfoHeight(IRecord _this)
        {
            return ((CharModelRecord)_this).HeadInfoHeight;
        }
        static object get_Scale(IRecord _this)
        {
            return ((CharModelRecord)_this).Scale;
        }
        static object get_DeadSound(IRecord _this)
        {
            return ((CharModelRecord)_this).DeadSound;
        }
        static object get_ShadowSize(IRecord _this)
        {
            return ((CharModelRecord)_this).ShadowSize;
        }
        static object get_RefreshAnimation(IRecord _this)
        {
            return ((CharModelRecord)_this).RefreshAnimation;
        }
        static object get_DieAnimation(IRecord _this)
        {
            return ((CharModelRecord)_this).DieAnimation;
        }
        static object get_BornEffectId(IRecord _this)
        {
            return ((CharModelRecord)_this).BornEffectId;
        }
        static object get_DieEffectId(IRecord _this)
        {
            return ((CharModelRecord)_this).DieEffectId;
        }
        static object get_RandomStand(IRecord _this)
        {
            return ((CharModelRecord)_this).RandomStand;
        }
        static object get_WingTop(IRecord _this)
        {
            return ((CharModelRecord)_this).WingTop;
        }
        static CharModelRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["ResPath"] = get_ResPath;
            mField["AnimPath"] = get_AnimPath;
            mField["HeadPic"] = get_HeadPic;
            mField["NPCSpriteName"] = get_NPCSpriteName;
            mField["HeadInfoHeight"] = get_HeadInfoHeight;
            mField["Scale"] = get_Scale;
            mField["DeadSound"] = get_DeadSound;
            mField["ShadowSize"] = get_ShadowSize;
            mField["RefreshAnimation"] = get_RefreshAnimation;
            mField["DieAnimation"] = get_DieAnimation;
            mField["BornEffectId"] = get_BornEffectId;
            mField["DieEffectId"] = get_DieEffectId;
            mField["RandomStand"] = get_RandomStand;
            mField["WingTop"] = get_WingTop;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AnimationRecord :IRecord
    {
        public static string __TableName = "Animation.txt";
        public int Id { get; private set; }
        public string AinmName { get; private set; }
        public int WrapMode { get; private set; }
        public double SPEED { get; private set; }
        public double TransitTime { get; private set; }
        public int LoopTime { get; private set; }
        public int LoopOverAnimId { get; private set; }
        public int IsCanBreak { get; private set; }
        public int[] StartEffect = new int[6];
        public int EndEffect { get; private set; }
        public int IsCallEnd { get; private set; }
        public int NextAnimId { get; private set; }
        public int SoundID { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                AinmName = temp[__column++];
                WrapMode = Table_Tamplet.Convert_Int(temp[__column++]);
                SPEED = Table_Tamplet.Convert_Double(temp[__column++]);
                TransitTime = Table_Tamplet.Convert_Double(temp[__column++]);
                LoopTime = Table_Tamplet.Convert_Int(temp[__column++]);
                LoopOverAnimId = Table_Tamplet.Convert_Int(temp[__column++]);
                IsCanBreak = Table_Tamplet.Convert_Int(temp[__column++]);
                StartEffect[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                StartEffect[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                StartEffect[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                StartEffect[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                StartEffect[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                StartEffect[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                EndEffect = Table_Tamplet.Convert_Int(temp[__column++]);
                IsCallEnd = Table_Tamplet.Convert_Int(temp[__column++]);
                NextAnimId = Table_Tamplet.Convert_Int(temp[__column++]);
                SoundID = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AnimationRecord)_this).Id;
        }
        static object get_AinmName(IRecord _this)
        {
            return ((AnimationRecord)_this).AinmName;
        }
        static object get_WrapMode(IRecord _this)
        {
            return ((AnimationRecord)_this).WrapMode;
        }
        static object get_SPEED(IRecord _this)
        {
            return ((AnimationRecord)_this).SPEED;
        }
        static object get_TransitTime(IRecord _this)
        {
            return ((AnimationRecord)_this).TransitTime;
        }
        static object get_LoopTime(IRecord _this)
        {
            return ((AnimationRecord)_this).LoopTime;
        }
        static object get_LoopOverAnimId(IRecord _this)
        {
            return ((AnimationRecord)_this).LoopOverAnimId;
        }
        static object get_IsCanBreak(IRecord _this)
        {
            return ((AnimationRecord)_this).IsCanBreak;
        }
        static object get_StartEffect(IRecord _this)
        {
            return ((AnimationRecord)_this).StartEffect;
        }
        static object get_EndEffect(IRecord _this)
        {
            return ((AnimationRecord)_this).EndEffect;
        }
        static object get_IsCallEnd(IRecord _this)
        {
            return ((AnimationRecord)_this).IsCallEnd;
        }
        static object get_NextAnimId(IRecord _this)
        {
            return ((AnimationRecord)_this).NextAnimId;
        }
        static object get_SoundID(IRecord _this)
        {
            return ((AnimationRecord)_this).SoundID;
        }
        static AnimationRecord()
        {
            mField["Id"] = get_Id;
            mField["AinmName"] = get_AinmName;
            mField["WrapMode"] = get_WrapMode;
            mField["SPEED"] = get_SPEED;
            mField["TransitTime"] = get_TransitTime;
            mField["LoopTime"] = get_LoopTime;
            mField["LoopOverAnimId"] = get_LoopOverAnimId;
            mField["IsCanBreak"] = get_IsCanBreak;
            mField["StartEffect"] = get_StartEffect;
            mField["EndEffect"] = get_EndEffect;
            mField["IsCallEnd"] = get_IsCallEnd;
            mField["NextAnimId"] = get_NextAnimId;
            mField["SoundID"] = get_SoundID;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SkillRecord :IRecord
    {
        public static string __TableName = "Skill.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int Type { get; private set; }
        public int NeedHp { get; private set; }
        public int NeedMp { get; private set; }
        public int NeedAnger { get; private set; }
        public int Cd { get; private set; }
        public int Layer { get; private set; }
        public int CommonCd { get; private set; }
        public int BulletId { get; private set; }
        public int ActionId { get; private set; }
        public int NoMove { get; private set; }
        public int CastType { get; private set; }
        public int[] CastParam = new int[4];
        public int ControlType { get; private set; }
        public int[] BeforeBuff = new int[2];
        public int TargetType { get; private set; }
        public int[] TargetParam = new int[6];
        public int CampType { get; private set; }
        public int DelayTarget { get; private set; }
        public int DelayView { get; private set; }
        public int TargetCount { get; private set; }
        public int[] AfterBuff = new int[2];
        public int[] MainTarget = new int[4];
        public int[] OtherTarget = new int[2];
        public int ExdataChange { get; private set; }
        public int HitType { get; private set; }
        public int Effect { get; private set; }
        public int NeedMoney { get; private set; }
        public int TalentMax { get; private set; }
        public int FightPoint { get; private set; }
        public int AutoEnemy { get; private set; }
        public int SkillID { get; private set; }
        public int ResetCount { get; private set; }
        public int IsEquipCanUse { get; private set; }
        public int SkillSound { get; private set; }
        public int ReleaseLevel { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedHp = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedMp = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedAnger = Table_Tamplet.Convert_Int(temp[__column++]);
                Cd = Table_Tamplet.Convert_Int(temp[__column++]);
                Layer = Table_Tamplet.Convert_Int(temp[__column++]);
                CommonCd = Table_Tamplet.Convert_Int(temp[__column++]);
                BulletId = Table_Tamplet.Convert_Int(temp[__column++]);
                ActionId = Table_Tamplet.Convert_Int(temp[__column++]);
                NoMove = Table_Tamplet.Convert_Int(temp[__column++]);
                CastType = Table_Tamplet.Convert_Int(temp[__column++]);
                CastParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                CastParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                CastParam[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                CastParam[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ControlType = Table_Tamplet.Convert_Int(temp[__column++]);
                BeforeBuff[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BeforeBuff[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetType = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetParam[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetParam[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetParam[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetParam[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                CampType = Table_Tamplet.Convert_Int(temp[__column++]);
                DelayTarget = Table_Tamplet.Convert_Int(temp[__column++]);
                DelayView = Table_Tamplet.Convert_Int(temp[__column++]);
                TargetCount = Table_Tamplet.Convert_Int(temp[__column++]);
                AfterBuff[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AfterBuff[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                MainTarget[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                MainTarget[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                MainTarget[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                MainTarget[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                OtherTarget[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                OtherTarget[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataChange = Table_Tamplet.Convert_Int(temp[__column++]);
                HitType = Table_Tamplet.Convert_Int(temp[__column++]);
                Effect = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                TalentMax = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                AutoEnemy = Table_Tamplet.Convert_Int(temp[__column++]);
                SkillID = Table_Tamplet.Convert_Int(temp[__column++]);
                ResetCount = Table_Tamplet.Convert_Int(temp[__column++]);
                IsEquipCanUse = Table_Tamplet.Convert_Int(temp[__column++]);
                SkillSound = Table_Tamplet.Convert_Int(temp[__column++]);
                ReleaseLevel = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SkillRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((SkillRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((SkillRecord)_this).Desc;
        }
        static object get_Icon(IRecord _this)
        {
            return ((SkillRecord)_this).Icon;
        }
        static object get_Type(IRecord _this)
        {
            return ((SkillRecord)_this).Type;
        }
        static object get_NeedHp(IRecord _this)
        {
            return ((SkillRecord)_this).NeedHp;
        }
        static object get_NeedMp(IRecord _this)
        {
            return ((SkillRecord)_this).NeedMp;
        }
        static object get_NeedAnger(IRecord _this)
        {
            return ((SkillRecord)_this).NeedAnger;
        }
        static object get_Cd(IRecord _this)
        {
            return ((SkillRecord)_this).Cd;
        }
        static object get_Layer(IRecord _this)
        {
            return ((SkillRecord)_this).Layer;
        }
        static object get_CommonCd(IRecord _this)
        {
            return ((SkillRecord)_this).CommonCd;
        }
        static object get_BulletId(IRecord _this)
        {
            return ((SkillRecord)_this).BulletId;
        }
        static object get_ActionId(IRecord _this)
        {
            return ((SkillRecord)_this).ActionId;
        }
        static object get_NoMove(IRecord _this)
        {
            return ((SkillRecord)_this).NoMove;
        }
        static object get_CastType(IRecord _this)
        {
            return ((SkillRecord)_this).CastType;
        }
        static object get_CastParam(IRecord _this)
        {
            return ((SkillRecord)_this).CastParam;
        }
        static object get_ControlType(IRecord _this)
        {
            return ((SkillRecord)_this).ControlType;
        }
        static object get_BeforeBuff(IRecord _this)
        {
            return ((SkillRecord)_this).BeforeBuff;
        }
        static object get_TargetType(IRecord _this)
        {
            return ((SkillRecord)_this).TargetType;
        }
        static object get_TargetParam(IRecord _this)
        {
            return ((SkillRecord)_this).TargetParam;
        }
        static object get_CampType(IRecord _this)
        {
            return ((SkillRecord)_this).CampType;
        }
        static object get_DelayTarget(IRecord _this)
        {
            return ((SkillRecord)_this).DelayTarget;
        }
        static object get_DelayView(IRecord _this)
        {
            return ((SkillRecord)_this).DelayView;
        }
        static object get_TargetCount(IRecord _this)
        {
            return ((SkillRecord)_this).TargetCount;
        }
        static object get_AfterBuff(IRecord _this)
        {
            return ((SkillRecord)_this).AfterBuff;
        }
        static object get_MainTarget(IRecord _this)
        {
            return ((SkillRecord)_this).MainTarget;
        }
        static object get_OtherTarget(IRecord _this)
        {
            return ((SkillRecord)_this).OtherTarget;
        }
        static object get_ExdataChange(IRecord _this)
        {
            return ((SkillRecord)_this).ExdataChange;
        }
        static object get_HitType(IRecord _this)
        {
            return ((SkillRecord)_this).HitType;
        }
        static object get_Effect(IRecord _this)
        {
            return ((SkillRecord)_this).Effect;
        }
        static object get_NeedMoney(IRecord _this)
        {
            return ((SkillRecord)_this).NeedMoney;
        }
        static object get_TalentMax(IRecord _this)
        {
            return ((SkillRecord)_this).TalentMax;
        }
        static object get_FightPoint(IRecord _this)
        {
            return ((SkillRecord)_this).FightPoint;
        }
        static object get_AutoEnemy(IRecord _this)
        {
            return ((SkillRecord)_this).AutoEnemy;
        }
        static object get_SkillID(IRecord _this)
        {
            return ((SkillRecord)_this).SkillID;
        }
        static object get_ResetCount(IRecord _this)
        {
            return ((SkillRecord)_this).ResetCount;
        }
        static object get_IsEquipCanUse(IRecord _this)
        {
            return ((SkillRecord)_this).IsEquipCanUse;
        }
        static object get_SkillSound(IRecord _this)
        {
            return ((SkillRecord)_this).SkillSound;
        }
        static object get_ReleaseLevel(IRecord _this)
        {
            return ((SkillRecord)_this).ReleaseLevel;
        }
        static SkillRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["Icon"] = get_Icon;
            mField["Type"] = get_Type;
            mField["NeedHp"] = get_NeedHp;
            mField["NeedMp"] = get_NeedMp;
            mField["NeedAnger"] = get_NeedAnger;
            mField["Cd"] = get_Cd;
            mField["Layer"] = get_Layer;
            mField["CommonCd"] = get_CommonCd;
            mField["BulletId"] = get_BulletId;
            mField["ActionId"] = get_ActionId;
            mField["NoMove"] = get_NoMove;
            mField["CastType"] = get_CastType;
            mField["CastParam"] = get_CastParam;
            mField["ControlType"] = get_ControlType;
            mField["BeforeBuff"] = get_BeforeBuff;
            mField["TargetType"] = get_TargetType;
            mField["TargetParam"] = get_TargetParam;
            mField["CampType"] = get_CampType;
            mField["DelayTarget"] = get_DelayTarget;
            mField["DelayView"] = get_DelayView;
            mField["TargetCount"] = get_TargetCount;
            mField["AfterBuff"] = get_AfterBuff;
            mField["MainTarget"] = get_MainTarget;
            mField["OtherTarget"] = get_OtherTarget;
            mField["ExdataChange"] = get_ExdataChange;
            mField["HitType"] = get_HitType;
            mField["Effect"] = get_Effect;
            mField["NeedMoney"] = get_NeedMoney;
            mField["TalentMax"] = get_TalentMax;
            mField["FightPoint"] = get_FightPoint;
            mField["AutoEnemy"] = get_AutoEnemy;
            mField["SkillID"] = get_SkillID;
            mField["ResetCount"] = get_ResetCount;
            mField["IsEquipCanUse"] = get_IsEquipCanUse;
            mField["SkillSound"] = get_SkillSound;
            mField["ReleaseLevel"] = get_ReleaseLevel;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SceneRecord :IRecord
    {
        public static string __TableName = "Scene.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string ResName { get; private set; }
        public int Type { get; private set; }
        public string Stem { get; private set; }
        public string MiniMap { get; private set; }
        public int Sound { get; private set; }
        public int PrepareSound { get; private set; }
        public int[] ReliveType = new int[3];
        public int CityId { get; private set; }
        public double Entry_x { get; private set; }
        public double Entry_z { get; private set; }
        public double Safe_x { get; private set; }
        public double Safe_z { get; private set; }
        public int PvPRule { get; private set; }
        public int SwapLine { get; private set; }
        public int PlayersMaxA { get; private set; }
        public int PlayersMaxB { get; private set; }
        public int TerrainHeightMapWidth { get; private set; }
        public int TerrainHeightMapLength { get; private set; }
        public int FubenId { get; private set; }
        public int SeeArea { get; private set; }
        public int DeadDisplaySurface { get; private set; }
        public double PVPPosX { get; private set; }
        public double PVPPosZ { get; private set; }
        public int IsPublic { get; private set; }
        public int LevelLimit { get; private set; }
        public int ConsumeMoney { get; private set; }
        public int ShowNameIcon { get; private set; }
        public int SwitchSceneMap { get; private set; }
        public int TrackInfo { get; private set; }
        public int IsOpenMap { get; private set; }
        public int CanUseHangBtn { get; private set; }
        public int IsShowMainUI { get; private set; }
        public int SafeReliveCD { get; private set; }
        public int IsHideName { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                ResName = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Stem = temp[__column++];
                MiniMap = temp[__column++];
                Sound = Table_Tamplet.Convert_Int(temp[__column++]);
                PrepareSound = Table_Tamplet.Convert_Int(temp[__column++]);
                ReliveType[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ReliveType[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityId = Table_Tamplet.Convert_Int(temp[__column++]);
                ReliveType[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_x = Table_Tamplet.Convert_Double(temp[__column++]);
                Entry_z = Table_Tamplet.Convert_Double(temp[__column++]);
                Safe_x = Table_Tamplet.Convert_Double(temp[__column++]);
                Safe_z = Table_Tamplet.Convert_Double(temp[__column++]);
                PvPRule = Table_Tamplet.Convert_Int(temp[__column++]);
                SwapLine = Table_Tamplet.Convert_Int(temp[__column++]);
                PlayersMaxA = Table_Tamplet.Convert_Int(temp[__column++]);
                PlayersMaxB = Table_Tamplet.Convert_Int(temp[__column++]);
                TerrainHeightMapWidth = Table_Tamplet.Convert_Int(temp[__column++]);
                TerrainHeightMapLength = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenId = Table_Tamplet.Convert_Int(temp[__column++]);
                SeeArea = Table_Tamplet.Convert_Int(temp[__column++]);
                DeadDisplaySurface = Table_Tamplet.Convert_Int(temp[__column++]);
                PVPPosX = Table_Tamplet.Convert_Double(temp[__column++]);
                PVPPosZ = Table_Tamplet.Convert_Double(temp[__column++]);
                IsPublic = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                ConsumeMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowNameIcon = Table_Tamplet.Convert_Int(temp[__column++]);
                SwitchSceneMap = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackInfo = Table_Tamplet.Convert_Int(temp[__column++]);
                IsOpenMap = Table_Tamplet.Convert_Int(temp[__column++]);
                CanUseHangBtn = Table_Tamplet.Convert_Int(temp[__column++]);
                IsShowMainUI = Table_Tamplet.Convert_Int(temp[__column++]);
                SafeReliveCD = Table_Tamplet.Convert_Int(temp[__column++]);
                IsHideName = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SceneRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((SceneRecord)_this).Name;
        }
        static object get_ResName(IRecord _this)
        {
            return ((SceneRecord)_this).ResName;
        }
        static object get_Type(IRecord _this)
        {
            return ((SceneRecord)_this).Type;
        }
        static object get_Stem(IRecord _this)
        {
            return ((SceneRecord)_this).Stem;
        }
        static object get_MiniMap(IRecord _this)
        {
            return ((SceneRecord)_this).MiniMap;
        }
        static object get_Sound(IRecord _this)
        {
            return ((SceneRecord)_this).Sound;
        }
        static object get_PrepareSound(IRecord _this)
        {
            return ((SceneRecord)_this).PrepareSound;
        }
        static object get_ReliveType(IRecord _this)
        {
            return ((SceneRecord)_this).ReliveType;
        }
        static object get_CityId(IRecord _this)
        {
            return ((SceneRecord)_this).CityId;
        }
        static object get_Entry_x(IRecord _this)
        {
            return ((SceneRecord)_this).Entry_x;
        }
        static object get_Entry_z(IRecord _this)
        {
            return ((SceneRecord)_this).Entry_z;
        }
        static object get_Safe_x(IRecord _this)
        {
            return ((SceneRecord)_this).Safe_x;
        }
        static object get_Safe_z(IRecord _this)
        {
            return ((SceneRecord)_this).Safe_z;
        }
        static object get_PvPRule(IRecord _this)
        {
            return ((SceneRecord)_this).PvPRule;
        }
        static object get_SwapLine(IRecord _this)
        {
            return ((SceneRecord)_this).SwapLine;
        }
        static object get_PlayersMaxA(IRecord _this)
        {
            return ((SceneRecord)_this).PlayersMaxA;
        }
        static object get_PlayersMaxB(IRecord _this)
        {
            return ((SceneRecord)_this).PlayersMaxB;
        }
        static object get_TerrainHeightMapWidth(IRecord _this)
        {
            return ((SceneRecord)_this).TerrainHeightMapWidth;
        }
        static object get_TerrainHeightMapLength(IRecord _this)
        {
            return ((SceneRecord)_this).TerrainHeightMapLength;
        }
        static object get_FubenId(IRecord _this)
        {
            return ((SceneRecord)_this).FubenId;
        }
        static object get_SeeArea(IRecord _this)
        {
            return ((SceneRecord)_this).SeeArea;
        }
        static object get_DeadDisplaySurface(IRecord _this)
        {
            return ((SceneRecord)_this).DeadDisplaySurface;
        }
        static object get_PVPPosX(IRecord _this)
        {
            return ((SceneRecord)_this).PVPPosX;
        }
        static object get_PVPPosZ(IRecord _this)
        {
            return ((SceneRecord)_this).PVPPosZ;
        }
        static object get_IsPublic(IRecord _this)
        {
            return ((SceneRecord)_this).IsPublic;
        }
        static object get_LevelLimit(IRecord _this)
        {
            return ((SceneRecord)_this).LevelLimit;
        }
        static object get_ConsumeMoney(IRecord _this)
        {
            return ((SceneRecord)_this).ConsumeMoney;
        }
        static object get_ShowNameIcon(IRecord _this)
        {
            return ((SceneRecord)_this).ShowNameIcon;
        }
        static object get_SwitchSceneMap(IRecord _this)
        {
            return ((SceneRecord)_this).SwitchSceneMap;
        }
        static object get_TrackInfo(IRecord _this)
        {
            return ((SceneRecord)_this).TrackInfo;
        }
        static object get_IsOpenMap(IRecord _this)
        {
            return ((SceneRecord)_this).IsOpenMap;
        }
        static object get_CanUseHangBtn(IRecord _this)
        {
            return ((SceneRecord)_this).CanUseHangBtn;
        }
        static object get_IsShowMainUI(IRecord _this)
        {
            return ((SceneRecord)_this).IsShowMainUI;
        }
        static object get_SafeReliveCD(IRecord _this)
        {
            return ((SceneRecord)_this).SafeReliveCD;
        }
        static object get_IsHideName(IRecord _this)
        {
            return ((SceneRecord)_this).IsHideName;
        }
        static SceneRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["ResName"] = get_ResName;
            mField["Type"] = get_Type;
            mField["Stem"] = get_Stem;
            mField["MiniMap"] = get_MiniMap;
            mField["Sound"] = get_Sound;
            mField["PrepareSound"] = get_PrepareSound;
            mField["ReliveType"] = get_ReliveType;
            mField["CityId"] = get_CityId;
            mField["Entry_x"] = get_Entry_x;
            mField["Entry_z"] = get_Entry_z;
            mField["Safe_x"] = get_Safe_x;
            mField["Safe_z"] = get_Safe_z;
            mField["PvPRule"] = get_PvPRule;
            mField["SwapLine"] = get_SwapLine;
            mField["PlayersMaxA"] = get_PlayersMaxA;
            mField["PlayersMaxB"] = get_PlayersMaxB;
            mField["TerrainHeightMapWidth"] = get_TerrainHeightMapWidth;
            mField["TerrainHeightMapLength"] = get_TerrainHeightMapLength;
            mField["FubenId"] = get_FubenId;
            mField["SeeArea"] = get_SeeArea;
            mField["DeadDisplaySurface"] = get_DeadDisplaySurface;
            mField["PVPPosX"] = get_PVPPosX;
            mField["PVPPosZ"] = get_PVPPosZ;
            mField["IsPublic"] = get_IsPublic;
            mField["LevelLimit"] = get_LevelLimit;
            mField["ConsumeMoney"] = get_ConsumeMoney;
            mField["ShowNameIcon"] = get_ShowNameIcon;
            mField["SwitchSceneMap"] = get_SwitchSceneMap;
            mField["TrackInfo"] = get_TrackInfo;
            mField["IsOpenMap"] = get_IsOpenMap;
            mField["CanUseHangBtn"] = get_CanUseHangBtn;
            mField["IsShowMainUI"] = get_IsShowMainUI;
            mField["SafeReliveCD"] = get_SafeReliveCD;
            mField["IsHideName"] = get_IsHideName;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class CharacterBaseRecord :IRecord
    {
        public static string __TableName = "CharacterBase.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int CharModelID { get; private set; }
        public int Type { get; private set; }
        public int ExdataId { get; private set; }
        public int Sex { get; private set; }
        public int Camp { get; private set; }
        public int[] Attr = new int[33];
        public int[] InitSkill = new int[20];
        public int[] HitEffectId = new int[2];
        public int[] Idle = new int[3];
        public float CameraMult { get;        set; }
        public float CameraHeight { get;        set; }
        [TableBinding("Icon")]
        public int HeadIcon { get; private set; }
        public int BloodType { get; private set; }
        public float BloodCount { get;        set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                CharModelID = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataId = Table_Tamplet.Convert_Int(temp[__column++]);
                Sex = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[14] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[15] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[16] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[17] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[18] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[19] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[20] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[21] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[22] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[23] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[24] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[25] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[26] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[27] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[28] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[29] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[30] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[31] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[32] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[14] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[15] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[16] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[17] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[18] = Table_Tamplet.Convert_Int(temp[__column++]);
                InitSkill[19] = Table_Tamplet.Convert_Int(temp[__column++]);
                HitEffectId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                HitEffectId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Idle[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Idle[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Idle[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                CameraMult = Table_Tamplet.Convert_Float(temp[__column++]);
                CameraHeight = Table_Tamplet.Convert_Float(temp[__column++]);
                HeadIcon = Table_Tamplet.Convert_Int(temp[__column++]);
                BloodType = Table_Tamplet.Convert_Int(temp[__column++]);
                BloodCount = Table_Tamplet.Convert_Float(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Name;
        }
        static object get_CharModelID(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).CharModelID;
        }
        static object get_Type(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Type;
        }
        static object get_ExdataId(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).ExdataId;
        }
        static object get_Sex(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Sex;
        }
        static object get_Camp(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Camp;
        }
        static object get_Attr(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Attr;
        }
        static object get_InitSkill(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).InitSkill;
        }
        static object get_HitEffectId(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).HitEffectId;
        }
        static object get_Idle(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).Idle;
        }
        static object get_CameraMult(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).CameraMult;
        }
        static object get_CameraHeight(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).CameraHeight;
        }
        static object get_HeadIcon(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).HeadIcon;
        }
        static object get_BloodType(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).BloodType;
        }
        static object get_BloodCount(IRecord _this)
        {
            return ((CharacterBaseRecord)_this).BloodCount;
        }
        static CharacterBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["CharModelID"] = get_CharModelID;
            mField["Type"] = get_Type;
            mField["ExdataId"] = get_ExdataId;
            mField["Sex"] = get_Sex;
            mField["Camp"] = get_Camp;
            mField["Attr"] = get_Attr;
            mField["InitSkill"] = get_InitSkill;
            mField["HitEffectId"] = get_HitEffectId;
            mField["Idle"] = get_Idle;
            mField["CameraMult"] = get_CameraMult;
            mField["CameraHeight"] = get_CameraHeight;
            mField["HeadIcon"] = get_HeadIcon;
            mField["BloodType"] = get_BloodType;
            mField["BloodCount"] = get_BloodCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipBaseRecord :IRecord
    {
        public static string __TableName = "EquipBase.txt";
        public int Id { get; private set; }
        [TableBinding("EquipExcellent")]
        public int Ladder { get; private set; }
        [TableBinding("Actor")]
        public int Occupation { get; private set; }
        public int Part { get; private set; }
        public int EquipModel { get; private set; }
        public int DurableType { get; private set; }
        public int Durability { get; private set; }
        public int DurableMoney { get; private set; }
        public int TieId { get; private set; }
        public int TieIndex { get; private set; }
        [ListSize(2)]
        public ReadonlyList<int> NeedAttrId { get; private set; } 
        [ListSize(2)]
        public ReadonlyList<int> NeedAttrValue { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> BaseAttr { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> BaseValue { get; private set; } 
        public int MaxLevel { get; private set; }
        [ListSize(2)]
        public ReadonlyList<int> BaseFixedAttrId { get; private set; } 
        [ListSize(2)]
        public ReadonlyList<int> BaseFixedAttrValue { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> ExcellentAttrId { get; private set; } 
        public int ExcellentAttrCount { get; private set; }
        public int ExcellentAttrValue { get; private set; }
        public int ExcellentAttrInterval { get; private set; }
        public int ExcellentValueMin { get; private set; }
        public int ExcellentValueMax { get; private set; }
        public int AddAttrId { get; private set; }
        public int AddAttrUpMinValue { get; private set; }
        public int AddAttrUpMaxValue { get; private set; }
        public int AddAttrMaxValue { get; private set; }
        [TableBinding("EquipAdditional1")]
        public int AddIndexID { get; private set; }
        public int RandomAttrCount { get; private set; }
        public int RandomAttrPro { get; private set; }
        public int RandomAttrValue { get; private set; }
        public int RandomAttrInterval { get; private set; }
        public int RandomValueMin { get; private set; }
        [TableBinding("Rebound")]
        public int RandomValueMax { get; private set; }
        public int RandomSlotCount { get; private set; }
        public string AnimPath { get; private set; }
        public string ViceHandPath { get; private set; }
        public string NullHandPath { get; private set; }
        public int EquipUpdateLogic { get; private set; }
        public int UpdateEquipID { get; private set; }
        public int BuffGroupId { get; private set; }
        public int AddBuffSkillLevel { get; private set; }
        public int JingLianDescId { get; private set; }
        public int ZhuoYueDescId { get; private set; }
        public int LingHunDescId { get; private set; }
        public int FIghtNumDesc { get; private set; }
        public int ShowEquip { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Ladder = Table_Tamplet.Convert_Int(temp[__column++]);
                Occupation = Table_Tamplet.Convert_Int(temp[__column++]);
                Part = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipModel = Table_Tamplet.Convert_Int(temp[__column++]);
                DurableType = Table_Tamplet.Convert_Int(temp[__column++]);
                Durability = Table_Tamplet.Convert_Int(temp[__column++]);
                DurableMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                TieId = Table_Tamplet.Convert_Int(temp[__column++]);
                TieIndex = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedAttrId=new ReadonlyList<int>(2);
                NeedAttrId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedAttrValue=new ReadonlyList<int>(2);
                NeedAttrValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedAttrId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedAttrValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseAttr=new ReadonlyList<int>(4);
                BaseAttr[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseValue=new ReadonlyList<int>(4);
                BaseValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseAttr[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseAttr[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseAttr[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseFixedAttrId=new ReadonlyList<int>(2);
                BaseFixedAttrId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseFixedAttrValue=new ReadonlyList<int>(2);
                BaseFixedAttrValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseFixedAttrId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BaseFixedAttrValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrId=new ReadonlyList<int>(4);
                ExcellentAttrId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrCount = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrValue = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentAttrInterval = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentValueMin = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentValueMax = Table_Tamplet.Convert_Int(temp[__column++]);
                AddAttrId = Table_Tamplet.Convert_Int(temp[__column++]);
                AddAttrUpMinValue = Table_Tamplet.Convert_Int(temp[__column++]);
                AddAttrUpMaxValue = Table_Tamplet.Convert_Int(temp[__column++]);
                AddAttrMaxValue = Table_Tamplet.Convert_Int(temp[__column++]);
                AddIndexID = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomAttrCount = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomAttrPro = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomAttrValue = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomAttrInterval = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomValueMin = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomValueMax = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomSlotCount = Table_Tamplet.Convert_Int(temp[__column++]);
                AnimPath = temp[__column++];
                ViceHandPath = temp[__column++];
                NullHandPath = temp[__column++];
                EquipUpdateLogic = Table_Tamplet.Convert_Int(temp[__column++]);
                UpdateEquipID = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGroupId = Table_Tamplet.Convert_Int(temp[__column++]);
                AddBuffSkillLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                JingLianDescId = Table_Tamplet.Convert_Int(temp[__column++]);
                ZhuoYueDescId = Table_Tamplet.Convert_Int(temp[__column++]);
                LingHunDescId = Table_Tamplet.Convert_Int(temp[__column++]);
                FIghtNumDesc = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowEquip = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipBaseRecord)_this).Id;
        }
        static object get_Ladder(IRecord _this)
        {
            return ((EquipBaseRecord)_this).Ladder;
        }
        static object get_Occupation(IRecord _this)
        {
            return ((EquipBaseRecord)_this).Occupation;
        }
        static object get_Part(IRecord _this)
        {
            return ((EquipBaseRecord)_this).Part;
        }
        static object get_EquipModel(IRecord _this)
        {
            return ((EquipBaseRecord)_this).EquipModel;
        }
        static object get_DurableType(IRecord _this)
        {
            return ((EquipBaseRecord)_this).DurableType;
        }
        static object get_Durability(IRecord _this)
        {
            return ((EquipBaseRecord)_this).Durability;
        }
        static object get_DurableMoney(IRecord _this)
        {
            return ((EquipBaseRecord)_this).DurableMoney;
        }
        static object get_TieId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).TieId;
        }
        static object get_TieIndex(IRecord _this)
        {
            return ((EquipBaseRecord)_this).TieIndex;
        }
        static object get_NeedAttrId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).NeedAttrId;
        }
        static object get_NeedAttrValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).NeedAttrValue;
        }
        static object get_BaseAttr(IRecord _this)
        {
            return ((EquipBaseRecord)_this).BaseAttr;
        }
        static object get_BaseValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).BaseValue;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((EquipBaseRecord)_this).MaxLevel;
        }
        static object get_BaseFixedAttrId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).BaseFixedAttrId;
        }
        static object get_BaseFixedAttrValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).BaseFixedAttrValue;
        }
        static object get_ExcellentAttrId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ExcellentAttrId;
        }
        static object get_ExcellentAttrCount(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ExcellentAttrCount;
        }
        static object get_ExcellentAttrValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ExcellentAttrValue;
        }
        static object get_ExcellentAttrInterval(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ExcellentAttrInterval;
        }
        static object get_ExcellentValueMin(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ExcellentValueMin;
        }
        static object get_ExcellentValueMax(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ExcellentValueMax;
        }
        static object get_AddAttrId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AddAttrId;
        }
        static object get_AddAttrUpMinValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AddAttrUpMinValue;
        }
        static object get_AddAttrUpMaxValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AddAttrUpMaxValue;
        }
        static object get_AddAttrMaxValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AddAttrMaxValue;
        }
        static object get_AddIndexID(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AddIndexID;
        }
        static object get_RandomAttrCount(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomAttrCount;
        }
        static object get_RandomAttrPro(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomAttrPro;
        }
        static object get_RandomAttrValue(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomAttrValue;
        }
        static object get_RandomAttrInterval(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomAttrInterval;
        }
        static object get_RandomValueMin(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomValueMin;
        }
        static object get_RandomValueMax(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomValueMax;
        }
        static object get_RandomSlotCount(IRecord _this)
        {
            return ((EquipBaseRecord)_this).RandomSlotCount;
        }
        static object get_AnimPath(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AnimPath;
        }
        static object get_ViceHandPath(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ViceHandPath;
        }
        static object get_NullHandPath(IRecord _this)
        {
            return ((EquipBaseRecord)_this).NullHandPath;
        }
        static object get_EquipUpdateLogic(IRecord _this)
        {
            return ((EquipBaseRecord)_this).EquipUpdateLogic;
        }
        static object get_UpdateEquipID(IRecord _this)
        {
            return ((EquipBaseRecord)_this).UpdateEquipID;
        }
        static object get_BuffGroupId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).BuffGroupId;
        }
        static object get_AddBuffSkillLevel(IRecord _this)
        {
            return ((EquipBaseRecord)_this).AddBuffSkillLevel;
        }
        static object get_JingLianDescId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).JingLianDescId;
        }
        static object get_ZhuoYueDescId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ZhuoYueDescId;
        }
        static object get_LingHunDescId(IRecord _this)
        {
            return ((EquipBaseRecord)_this).LingHunDescId;
        }
        static object get_FIghtNumDesc(IRecord _this)
        {
            return ((EquipBaseRecord)_this).FIghtNumDesc;
        }
        static object get_ShowEquip(IRecord _this)
        {
            return ((EquipBaseRecord)_this).ShowEquip;
        }
        static EquipBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["Ladder"] = get_Ladder;
            mField["Occupation"] = get_Occupation;
            mField["Part"] = get_Part;
            mField["EquipModel"] = get_EquipModel;
            mField["DurableType"] = get_DurableType;
            mField["Durability"] = get_Durability;
            mField["DurableMoney"] = get_DurableMoney;
            mField["TieId"] = get_TieId;
            mField["TieIndex"] = get_TieIndex;
            mField["NeedAttrId"] = get_NeedAttrId;
            mField["NeedAttrValue"] = get_NeedAttrValue;
            mField["BaseAttr"] = get_BaseAttr;
            mField["BaseValue"] = get_BaseValue;
            mField["MaxLevel"] = get_MaxLevel;
            mField["BaseFixedAttrId"] = get_BaseFixedAttrId;
            mField["BaseFixedAttrValue"] = get_BaseFixedAttrValue;
            mField["ExcellentAttrId"] = get_ExcellentAttrId;
            mField["ExcellentAttrCount"] = get_ExcellentAttrCount;
            mField["ExcellentAttrValue"] = get_ExcellentAttrValue;
            mField["ExcellentAttrInterval"] = get_ExcellentAttrInterval;
            mField["ExcellentValueMin"] = get_ExcellentValueMin;
            mField["ExcellentValueMax"] = get_ExcellentValueMax;
            mField["AddAttrId"] = get_AddAttrId;
            mField["AddAttrUpMinValue"] = get_AddAttrUpMinValue;
            mField["AddAttrUpMaxValue"] = get_AddAttrUpMaxValue;
            mField["AddAttrMaxValue"] = get_AddAttrMaxValue;
            mField["AddIndexID"] = get_AddIndexID;
            mField["RandomAttrCount"] = get_RandomAttrCount;
            mField["RandomAttrPro"] = get_RandomAttrPro;
            mField["RandomAttrValue"] = get_RandomAttrValue;
            mField["RandomAttrInterval"] = get_RandomAttrInterval;
            mField["RandomValueMin"] = get_RandomValueMin;
            mField["RandomValueMax"] = get_RandomValueMax;
            mField["RandomSlotCount"] = get_RandomSlotCount;
            mField["AnimPath"] = get_AnimPath;
            mField["ViceHandPath"] = get_ViceHandPath;
            mField["NullHandPath"] = get_NullHandPath;
            mField["EquipUpdateLogic"] = get_EquipUpdateLogic;
            mField["UpdateEquipID"] = get_UpdateEquipID;
            mField["BuffGroupId"] = get_BuffGroupId;
            mField["AddBuffSkillLevel"] = get_AddBuffSkillLevel;
            mField["JingLianDescId"] = get_JingLianDescId;
            mField["ZhuoYueDescId"] = get_ZhuoYueDescId;
            mField["LingHunDescId"] = get_LingHunDescId;
            mField["FIghtNumDesc"] = get_FIghtNumDesc;
            mField["ShowEquip"] = get_ShowEquip;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ActorRecord :IRecord
    {
        public static string __TableName = "Actor.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public int Class { get; private set; }
        public int Sex { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int[] AutoAddAttr = new int[4];
        public int[] Equip = new int[7];
        public int[] Action = new int[3];
        public int[] Dubbing = new int[3];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                Class = Table_Tamplet.Convert_Int(temp[__column++]);
                Sex = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                AutoAddAttr[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AutoAddAttr[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AutoAddAttr[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AutoAddAttr[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Equip[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Action[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Action[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Action[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Dubbing[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Dubbing[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Dubbing[2] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ActorRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ActorRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((ActorRecord)_this).Desc;
        }
        static object get_Class(IRecord _this)
        {
            return ((ActorRecord)_this).Class;
        }
        static object get_Sex(IRecord _this)
        {
            return ((ActorRecord)_this).Sex;
        }
        static object get_Icon(IRecord _this)
        {
            return ((ActorRecord)_this).Icon;
        }
        static object get_AutoAddAttr(IRecord _this)
        {
            return ((ActorRecord)_this).AutoAddAttr;
        }
        static object get_Equip(IRecord _this)
        {
            return ((ActorRecord)_this).Equip;
        }
        static object get_Action(IRecord _this)
        {
            return ((ActorRecord)_this).Action;
        }
        static object get_Dubbing(IRecord _this)
        {
            return ((ActorRecord)_this).Dubbing;
        }
        static ActorRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["Class"] = get_Class;
            mField["Sex"] = get_Sex;
            mField["Icon"] = get_Icon;
            mField["AutoAddAttr"] = get_AutoAddAttr;
            mField["Equip"] = get_Equip;
            mField["Action"] = get_Action;
            mField["Dubbing"] = get_Dubbing;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TalentRecord :IRecord
    {
        public static string __TableName = "Talent.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int ActiveSkillId { get; private set; }
        public int ForgetSkillId { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int BeforeId { get; private set; }
        public int BeforeLayer { get; private set; }
        public int AttrId { get; private set; }
        public int SkillupgradingId { get; private set; }
        public int MaxLayer { get; private set; }
        public int HuchiId { get; private set; }
        public int ModifySkill { get; private set; }
        public int FightPointBySkillUpgrading { get; private set; }
        public int[] BuffId = new int[10];
        public string[] BuffDesc = new string[10];
        public int CastItemId { get; private set; }
        public int CastItemCount { get; private set; }
        public int NeedLevel { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                ActiveSkillId = Table_Tamplet.Convert_Int(temp[__column++]);
                ForgetSkillId = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                BeforeId = Table_Tamplet.Convert_Int(temp[__column++]);
                BeforeLayer = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId = Table_Tamplet.Convert_Int(temp[__column++]);
                SkillupgradingId = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLayer = Table_Tamplet.Convert_Int(temp[__column++]);
                HuchiId = Table_Tamplet.Convert_Int(temp[__column++]);
                ModifySkill = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPointBySkillUpgrading = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[0]  = temp[__column++];
                BuffId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[1]  = temp[__column++];
                BuffId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[2]  = temp[__column++];
                BuffId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[3]  = temp[__column++];
                BuffId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[4]  = temp[__column++];
                BuffId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[5]  = temp[__column++];
                BuffId[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[6]  = temp[__column++];
                BuffId[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[7]  = temp[__column++];
                BuffId[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[8]  = temp[__column++];
                BuffId[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDesc[9]  = temp[__column++];
                CastItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                CastItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedLevel = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TalentRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((TalentRecord)_this).Name;
        }
        static object get_ActiveSkillId(IRecord _this)
        {
            return ((TalentRecord)_this).ActiveSkillId;
        }
        static object get_ForgetSkillId(IRecord _this)
        {
            return ((TalentRecord)_this).ForgetSkillId;
        }
        static object get_Icon(IRecord _this)
        {
            return ((TalentRecord)_this).Icon;
        }
        static object get_BeforeId(IRecord _this)
        {
            return ((TalentRecord)_this).BeforeId;
        }
        static object get_BeforeLayer(IRecord _this)
        {
            return ((TalentRecord)_this).BeforeLayer;
        }
        static object get_AttrId(IRecord _this)
        {
            return ((TalentRecord)_this).AttrId;
        }
        static object get_SkillupgradingId(IRecord _this)
        {
            return ((TalentRecord)_this).SkillupgradingId;
        }
        static object get_MaxLayer(IRecord _this)
        {
            return ((TalentRecord)_this).MaxLayer;
        }
        static object get_HuchiId(IRecord _this)
        {
            return ((TalentRecord)_this).HuchiId;
        }
        static object get_ModifySkill(IRecord _this)
        {
            return ((TalentRecord)_this).ModifySkill;
        }
        static object get_FightPointBySkillUpgrading(IRecord _this)
        {
            return ((TalentRecord)_this).FightPointBySkillUpgrading;
        }
        static object get_BuffId(IRecord _this)
        {
            return ((TalentRecord)_this).BuffId;
        }
        static object get_BuffDesc(IRecord _this)
        {
            return ((TalentRecord)_this).BuffDesc;
        }
        static object get_CastItemId(IRecord _this)
        {
            return ((TalentRecord)_this).CastItemId;
        }
        static object get_CastItemCount(IRecord _this)
        {
            return ((TalentRecord)_this).CastItemCount;
        }
        static object get_NeedLevel(IRecord _this)
        {
            return ((TalentRecord)_this).NeedLevel;
        }
        static TalentRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["ActiveSkillId"] = get_ActiveSkillId;
            mField["ForgetSkillId"] = get_ForgetSkillId;
            mField["Icon"] = get_Icon;
            mField["BeforeId"] = get_BeforeId;
            mField["BeforeLayer"] = get_BeforeLayer;
            mField["AttrId"] = get_AttrId;
            mField["SkillupgradingId"] = get_SkillupgradingId;
            mField["MaxLayer"] = get_MaxLayer;
            mField["HuchiId"] = get_HuchiId;
            mField["ModifySkill"] = get_ModifySkill;
            mField["FightPointBySkillUpgrading"] = get_FightPointBySkillUpgrading;
            mField["BuffId"] = get_BuffId;
            mField["BuffDesc"] = get_BuffDesc;
            mField["CastItemId"] = get_CastItemId;
            mField["CastItemCount"] = get_CastItemCount;
            mField["NeedLevel"] = get_NeedLevel;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BagBaseRecord :IRecord
    {
        public static string __TableName = "BagBase.txt";
        public int Id { get; private set; }
        public int InitCapacity { get; private set; }
        public int MaxCapacity { get; private set; }
        public int ChangeBagCount { get; private set; }
        public int TimeMult { get; private set; }
        [TableBinding("SkillUpgrading")]
        public int Expression { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                InitCapacity = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxCapacity = Table_Tamplet.Convert_Int(temp[__column++]);
                ChangeBagCount = Table_Tamplet.Convert_Int(temp[__column++]);
                TimeMult = Table_Tamplet.Convert_Int(temp[__column++]);
                Expression = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BagBaseRecord)_this).Id;
        }
        static object get_InitCapacity(IRecord _this)
        {
            return ((BagBaseRecord)_this).InitCapacity;
        }
        static object get_MaxCapacity(IRecord _this)
        {
            return ((BagBaseRecord)_this).MaxCapacity;
        }
        static object get_ChangeBagCount(IRecord _this)
        {
            return ((BagBaseRecord)_this).ChangeBagCount;
        }
        static object get_TimeMult(IRecord _this)
        {
            return ((BagBaseRecord)_this).TimeMult;
        }
        static object get_Expression(IRecord _this)
        {
            return ((BagBaseRecord)_this).Expression;
        }
        static BagBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["InitCapacity"] = get_InitCapacity;
            mField["MaxCapacity"] = get_MaxCapacity;
            mField["ChangeBagCount"] = get_ChangeBagCount;
            mField["TimeMult"] = get_TimeMult;
            mField["Expression"] = get_Expression;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ItemBaseRecord :IRecord
    {
        public static string __TableName = "ItemBase.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int InitInBag { get; private set; }
        public int CanInBag { get; private set; }
        public int MaxCount { get; private set; }
        [TableBinding("ItemType")]
        public int Type { get; private set; }
        public string Desc { get; private set; }
        public int Quality { get; private set; }
        public int Color { get; private set; }
        public int UseLevel { get; private set; }
        public int OccupationLimit { get; private set; }
        public int BuyNeedType { get; private set; }
        public int BuyNeedCount { get; private set; }
        public int CallBackType { get; private set; }
        public int CallBackPrice { get; private set; }
        public int Sell { get; private set; }
        [TableBinding("Gem")]
        [ListSize(4)]
        public ReadonlyList<int> Exdata { get; private set; } 
        public int DropModel { get; private set; }
        public int CanTrade { get; private set; }
        public int TradeMaxCount { get; private set; }
        public int TradeMin { get; private set; }
        public int TradeMax { get; private set; }
        public int LevelLimit { get; private set; }
        public int CanUse { get; private set; }
        public int GetShowTip { get; private set; }
        public int ItemValue { get; private set; }
        public int GetWay { get; private set; }
        public int[] AuctionType = new int[3];
        public int DependItemId { get; private set; }
        public int DependItemNum { get; private set; }
        [TableBinding("Store")]
        public int StoreID { get; private set; }
        public string BoxOut { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                InitInBag = Table_Tamplet.Convert_Int(temp[__column++]);
                CanInBag = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = Table_Tamplet.Convert_String(temp[__column++]);
                Quality = Table_Tamplet.Convert_Int(temp[__column++]);
                Color = Table_Tamplet.Convert_Int(temp[__column++]);
                UseLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                OccupationLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyNeedType = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyNeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackType = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackPrice = Table_Tamplet.Convert_Int(temp[__column++]);
                Sell = Table_Tamplet.Convert_Int(temp[__column++]);
                Exdata=new ReadonlyList<int>(4);
                Exdata[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exdata[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exdata[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exdata[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropModel = Table_Tamplet.Convert_Int(temp[__column++]);
                CanTrade = Table_Tamplet.Convert_Int(temp[__column++]);
                TradeMaxCount = Table_Tamplet.Convert_Int(temp[__column++]);
                TradeMin = Table_Tamplet.Convert_Int(temp[__column++]);
                TradeMax = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                CanUse = Table_Tamplet.Convert_Int(temp[__column++]);
                GetShowTip = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemValue = Table_Tamplet.Convert_Int(temp[__column++]);
                GetWay = Table_Tamplet.Convert_Int(temp[__column++]);
                AuctionType[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AuctionType[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AuctionType[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DependItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                DependItemNum = Table_Tamplet.Convert_Int(temp[__column++]);
                StoreID = Table_Tamplet.Convert_Int(temp[__column++]);
                BoxOut = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Name;
        }
        static object get_Icon(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Icon;
        }
        static object get_InitInBag(IRecord _this)
        {
            return ((ItemBaseRecord)_this).InitInBag;
        }
        static object get_CanInBag(IRecord _this)
        {
            return ((ItemBaseRecord)_this).CanInBag;
        }
        static object get_MaxCount(IRecord _this)
        {
            return ((ItemBaseRecord)_this).MaxCount;
        }
        static object get_Type(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Type;
        }
        static object get_Desc(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Desc;
        }
        static object get_Quality(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Quality;
        }
        static object get_Color(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Color;
        }
        static object get_UseLevel(IRecord _this)
        {
            return ((ItemBaseRecord)_this).UseLevel;
        }
        static object get_OccupationLimit(IRecord _this)
        {
            return ((ItemBaseRecord)_this).OccupationLimit;
        }
        static object get_BuyNeedType(IRecord _this)
        {
            return ((ItemBaseRecord)_this).BuyNeedType;
        }
        static object get_BuyNeedCount(IRecord _this)
        {
            return ((ItemBaseRecord)_this).BuyNeedCount;
        }
        static object get_CallBackType(IRecord _this)
        {
            return ((ItemBaseRecord)_this).CallBackType;
        }
        static object get_CallBackPrice(IRecord _this)
        {
            return ((ItemBaseRecord)_this).CallBackPrice;
        }
        static object get_Sell(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Sell;
        }
        static object get_Exdata(IRecord _this)
        {
            return ((ItemBaseRecord)_this).Exdata;
        }
        static object get_DropModel(IRecord _this)
        {
            return ((ItemBaseRecord)_this).DropModel;
        }
        static object get_CanTrade(IRecord _this)
        {
            return ((ItemBaseRecord)_this).CanTrade;
        }
        static object get_TradeMaxCount(IRecord _this)
        {
            return ((ItemBaseRecord)_this).TradeMaxCount;
        }
        static object get_TradeMin(IRecord _this)
        {
            return ((ItemBaseRecord)_this).TradeMin;
        }
        static object get_TradeMax(IRecord _this)
        {
            return ((ItemBaseRecord)_this).TradeMax;
        }
        static object get_LevelLimit(IRecord _this)
        {
            return ((ItemBaseRecord)_this).LevelLimit;
        }
        static object get_CanUse(IRecord _this)
        {
            return ((ItemBaseRecord)_this).CanUse;
        }
        static object get_GetShowTip(IRecord _this)
        {
            return ((ItemBaseRecord)_this).GetShowTip;
        }
        static object get_ItemValue(IRecord _this)
        {
            return ((ItemBaseRecord)_this).ItemValue;
        }
        static object get_GetWay(IRecord _this)
        {
            return ((ItemBaseRecord)_this).GetWay;
        }
        static object get_AuctionType(IRecord _this)
        {
            return ((ItemBaseRecord)_this).AuctionType;
        }
        static object get_DependItemId(IRecord _this)
        {
            return ((ItemBaseRecord)_this).DependItemId;
        }
        static object get_DependItemNum(IRecord _this)
        {
            return ((ItemBaseRecord)_this).DependItemNum;
        }
        static object get_StoreID(IRecord _this)
        {
            return ((ItemBaseRecord)_this).StoreID;
        }
        static object get_BoxOut(IRecord _this)
        {
            return ((ItemBaseRecord)_this).BoxOut;
        }
        static ItemBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Icon"] = get_Icon;
            mField["InitInBag"] = get_InitInBag;
            mField["CanInBag"] = get_CanInBag;
            mField["MaxCount"] = get_MaxCount;
            mField["Type"] = get_Type;
            mField["Desc"] = get_Desc;
            mField["Quality"] = get_Quality;
            mField["Color"] = get_Color;
            mField["UseLevel"] = get_UseLevel;
            mField["OccupationLimit"] = get_OccupationLimit;
            mField["BuyNeedType"] = get_BuyNeedType;
            mField["BuyNeedCount"] = get_BuyNeedCount;
            mField["CallBackType"] = get_CallBackType;
            mField["CallBackPrice"] = get_CallBackPrice;
            mField["Sell"] = get_Sell;
            mField["Exdata"] = get_Exdata;
            mField["DropModel"] = get_DropModel;
            mField["CanTrade"] = get_CanTrade;
            mField["TradeMaxCount"] = get_TradeMaxCount;
            mField["TradeMin"] = get_TradeMin;
            mField["TradeMax"] = get_TradeMax;
            mField["LevelLimit"] = get_LevelLimit;
            mField["CanUse"] = get_CanUse;
            mField["GetShowTip"] = get_GetShowTip;
            mField["ItemValue"] = get_ItemValue;
            mField["GetWay"] = get_GetWay;
            mField["AuctionType"] = get_AuctionType;
            mField["DependItemId"] = get_DependItemId;
            mField["DependItemNum"] = get_DependItemNum;
            mField["StoreID"] = get_StoreID;
            mField["BoxOut"] = get_BoxOut;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ItemTypeRecord :IRecord
    {
        public static string __TableName = "ItemType.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Info { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Info = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ItemTypeRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ItemTypeRecord)_this).Name;
        }
        static object get_Info(IRecord _this)
        {
            return ((ItemTypeRecord)_this).Info;
        }
        static ItemTypeRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Info"] = get_Info;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ColorBaseRecord :IRecord
    {
        public static string __TableName = "ColorBase.txt";
        public int Id { get; private set; }
        public string Desc { get; private set; }
        public int Red { get; private set; }
        public int Green { get; private set; }
        public int Blue { get; private set; }
        public int Alpha { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                Red = Table_Tamplet.Convert_Int(temp[__column++]);
                Green = Table_Tamplet.Convert_Int(temp[__column++]);
                Blue = Table_Tamplet.Convert_Int(temp[__column++]);
                Alpha = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ColorBaseRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((ColorBaseRecord)_this).Desc;
        }
        static object get_Red(IRecord _this)
        {
            return ((ColorBaseRecord)_this).Red;
        }
        static object get_Green(IRecord _this)
        {
            return ((ColorBaseRecord)_this).Green;
        }
        static object get_Blue(IRecord _this)
        {
            return ((ColorBaseRecord)_this).Blue;
        }
        static object get_Alpha(IRecord _this)
        {
            return ((ColorBaseRecord)_this).Alpha;
        }
        static ColorBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
            mField["Red"] = get_Red;
            mField["Green"] = get_Green;
            mField["Blue"] = get_Blue;
            mField["Alpha"] = get_Alpha;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BuffRecord :IRecord
    {
        public static string __TableName = "Buff.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int IsView { get; private set; }
        public int Sound { get; private set; }
        public int[] Effect = new int[2];
        public int Duration { get; private set; }
        public int Type { get; private set; }
        public int DownLine { get; private set; }
        public int Die { get; private set; }
        public int SceneDisappear { get; private set; }
        public int DieDisappear { get; private set; }
        public int HuchiId { get; private set; }
        public int TihuanId { get; private set; }
        public int PriorityId { get; private set; }
        public int BearMax { get; private set; }
        public int LayerMax { get; private set; }
        public int DamageNumShowTimes { get; private set; }
        public int DamageDeltaTime { get; private set; }
        public int[] effectid = new int[4];
        public int[] effectpoint = new int[4];
        public int[,] effectparam = new int[4,6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                IsView = Table_Tamplet.Convert_Int(temp[__column++]);
                Sound = Table_Tamplet.Convert_Int(temp[__column++]);
                Effect[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Effect[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Duration = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                DownLine = Table_Tamplet.Convert_Int(temp[__column++]);
                Die = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneDisappear = Table_Tamplet.Convert_Int(temp[__column++]);
                DieDisappear = Table_Tamplet.Convert_Int(temp[__column++]);
                HuchiId = Table_Tamplet.Convert_Int(temp[__column++]);
                TihuanId = Table_Tamplet.Convert_Int(temp[__column++]);
                PriorityId = Table_Tamplet.Convert_Int(temp[__column++]);
                BearMax = Table_Tamplet.Convert_Int(temp[__column++]);
                LayerMax = Table_Tamplet.Convert_Int(temp[__column++]);
                DamageNumShowTimes = Table_Tamplet.Convert_Int(temp[__column++]);
                DamageDeltaTime = Table_Tamplet.Convert_Int(temp[__column++]);
                effectid[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectpoint[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[0,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[0,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[0,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[0,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[0,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[0,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectid[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectpoint[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[1,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[1,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[1,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[1,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[1,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[1,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectid[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectpoint[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[2,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[2,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[2,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[2,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[2,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[2,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectid[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectpoint[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[3,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[3,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[3,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[3,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[3,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                effectparam[3,5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BuffRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((BuffRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((BuffRecord)_this).Desc;
        }
        static object get_Icon(IRecord _this)
        {
            return ((BuffRecord)_this).Icon;
        }
        static object get_IsView(IRecord _this)
        {
            return ((BuffRecord)_this).IsView;
        }
        static object get_Sound(IRecord _this)
        {
            return ((BuffRecord)_this).Sound;
        }
        static object get_Effect(IRecord _this)
        {
            return ((BuffRecord)_this).Effect;
        }
        static object get_Duration(IRecord _this)
        {
            return ((BuffRecord)_this).Duration;
        }
        static object get_Type(IRecord _this)
        {
            return ((BuffRecord)_this).Type;
        }
        static object get_DownLine(IRecord _this)
        {
            return ((BuffRecord)_this).DownLine;
        }
        static object get_Die(IRecord _this)
        {
            return ((BuffRecord)_this).Die;
        }
        static object get_SceneDisappear(IRecord _this)
        {
            return ((BuffRecord)_this).SceneDisappear;
        }
        static object get_DieDisappear(IRecord _this)
        {
            return ((BuffRecord)_this).DieDisappear;
        }
        static object get_HuchiId(IRecord _this)
        {
            return ((BuffRecord)_this).HuchiId;
        }
        static object get_TihuanId(IRecord _this)
        {
            return ((BuffRecord)_this).TihuanId;
        }
        static object get_PriorityId(IRecord _this)
        {
            return ((BuffRecord)_this).PriorityId;
        }
        static object get_BearMax(IRecord _this)
        {
            return ((BuffRecord)_this).BearMax;
        }
        static object get_LayerMax(IRecord _this)
        {
            return ((BuffRecord)_this).LayerMax;
        }
        static object get_DamageNumShowTimes(IRecord _this)
        {
            return ((BuffRecord)_this).DamageNumShowTimes;
        }
        static object get_DamageDeltaTime(IRecord _this)
        {
            return ((BuffRecord)_this).DamageDeltaTime;
        }
        static object get_effectid(IRecord _this)
        {
            return ((BuffRecord)_this).effectid;
        }
        static object get_effectpoint(IRecord _this)
        {
            return ((BuffRecord)_this).effectpoint;
        }
        static object get_effectparam(IRecord _this)
        {
            return ((BuffRecord)_this).effectparam;
        }
        static BuffRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["Icon"] = get_Icon;
            mField["IsView"] = get_IsView;
            mField["Sound"] = get_Sound;
            mField["Effect"] = get_Effect;
            mField["Duration"] = get_Duration;
            mField["Type"] = get_Type;
            mField["DownLine"] = get_DownLine;
            mField["Die"] = get_Die;
            mField["SceneDisappear"] = get_SceneDisappear;
            mField["DieDisappear"] = get_DieDisappear;
            mField["HuchiId"] = get_HuchiId;
            mField["TihuanId"] = get_TihuanId;
            mField["PriorityId"] = get_PriorityId;
            mField["BearMax"] = get_BearMax;
            mField["LayerMax"] = get_LayerMax;
            mField["DamageNumShowTimes"] = get_DamageNumShowTimes;
            mField["DamageDeltaTime"] = get_DamageDeltaTime;
            mField["effectid"] = get_effectid;
            mField["effectpoint"] = get_effectpoint;
            mField["effectparam"] = get_effectparam;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MissionBaseRecord :IRecord
    {
        public static string __TableName = "MissionBase.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Desc { get; private set; }
        public int ViewType { get; private set; }
        public int Condition { get; private set; }
        public int CanDrop { get; private set; }
        public int TimeLimit { get; private set; }
        public int NpcStart { get; private set; }
        public int NpcScene { get; private set; }
        public float PosX { get;        set; }
        public float PosY { get;        set; }
        public string TrackDescription { get; private set; }
        public int TrackType { get; private set; }
        public int[] TrackParam = new int[4];
        public int FinishCondition { get; private set; }
        [ListSize(3)]
        public ReadonlyList<int> FinishParam { get; private set; } 
        public string FinishDescription { get; private set; }
        public int FinishNpcId { get; private set; }
        public int FinishSceneId { get; private set; }
        public float FinishPosX { get;        set; }
        public float FinishPosY { get;        set; }
        public int TalkId { get; private set; }
        public int DialogueNpc { get; private set; }
        public string PlayCollectionAct { get; private set; }
        public int DialogueFinish { get; private set; }
        public int NextMission { get; private set; }
        public int JbId { get; private set; }
        public int[] RewardItem = new int[3];
        public int[] RewardItemCount = new int[3];
        public int[] StoryId = new int[3];
        public int TriggerActive { get; private set; }
        public int TriggerClose { get; private set; }
        public int FlagId { get; private set; }
        public int BuffAdd { get; private set; }
        public int BuffClean { get; private set; }
        public int RewardTitle { get; private set; }
        public int[,] RoleRewardId = new int[3,2];
        public int[,] RoleRewardCount = new int[3,2];
        public int GetPlay { get; private set; }
        public int FinishPlay { get; private set; }
        public int PayPlay { get; private set; }
        public int GetStop { get; private set; }
        public int FinishStop { get; private set; }
        public int PayStop { get; private set; }
        public int AcceptTaskMusic { get; private set; }
        public int DeliveryTaskMusic { get; private set; }
        public int IsDynamicExp { get; private set; }
        public int DynamicExpRatio { get; private set; }
        public int UIGuideId { get; private set; }
        public int UIGuideTab { get; private set; }
        public int RewardDictId { get; private set; }
        public int MissionBianHao { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = Table_Tamplet.Convert_Int(temp[__column++]);
                ViewType = Table_Tamplet.Convert_Int(temp[__column++]);
                Condition = Table_Tamplet.Convert_Int(temp[__column++]);
                CanDrop = Table_Tamplet.Convert_Int(temp[__column++]);
                TimeLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                NpcStart = Table_Tamplet.Convert_Int(temp[__column++]);
                NpcScene = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PosY = Table_Tamplet.Convert_Float(temp[__column++]);
                TrackDescription = temp[__column++];
                TrackType = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackParam[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackParam[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishCondition = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishParam=new ReadonlyList<int>(3);
                FinishParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishParam[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishDescription = temp[__column++];
                FinishNpcId = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishSceneId = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishPosX = Table_Tamplet.Convert_Float(temp[__column++]);
                FinishPosY = Table_Tamplet.Convert_Float(temp[__column++]);
                TalkId = Table_Tamplet.Convert_Int(temp[__column++]);
                DialogueNpc = Table_Tamplet.Convert_Int(temp[__column++]);
                PlayCollectionAct = temp[__column++];
                DialogueFinish = Table_Tamplet.Convert_Int(temp[__column++]);
                NextMission = Table_Tamplet.Convert_Int(temp[__column++]);
                JbId = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardItem[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardItem[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardItem[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                StoryId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                StoryId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                StoryId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                TriggerActive = Table_Tamplet.Convert_Int(temp[__column++]);
                TriggerClose = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagId = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffClean = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardTitle = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardId[0,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardCount[0,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardId[0,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardCount[0,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardId[1,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardCount[1,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardId[1,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardCount[1,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardId[2,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardCount[2,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardId[2,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleRewardCount[2,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GetPlay = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishPlay = Table_Tamplet.Convert_Int(temp[__column++]);
                PayPlay = Table_Tamplet.Convert_Int(temp[__column++]);
                GetStop = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishStop = Table_Tamplet.Convert_Int(temp[__column++]);
                PayStop = Table_Tamplet.Convert_Int(temp[__column++]);
                AcceptTaskMusic = Table_Tamplet.Convert_Int(temp[__column++]);
                DeliveryTaskMusic = Table_Tamplet.Convert_Int(temp[__column++]);
                IsDynamicExp = Table_Tamplet.Convert_Int(temp[__column++]);
                DynamicExpRatio = Table_Tamplet.Convert_Int(temp[__column++]);
                UIGuideId = Table_Tamplet.Convert_Int(temp[__column++]);
                UIGuideTab = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardDictId = Table_Tamplet.Convert_Int(temp[__column++]);
                MissionBianHao = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MissionBaseRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((MissionBaseRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((MissionBaseRecord)_this).Desc;
        }
        static object get_ViewType(IRecord _this)
        {
            return ((MissionBaseRecord)_this).ViewType;
        }
        static object get_Condition(IRecord _this)
        {
            return ((MissionBaseRecord)_this).Condition;
        }
        static object get_CanDrop(IRecord _this)
        {
            return ((MissionBaseRecord)_this).CanDrop;
        }
        static object get_TimeLimit(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TimeLimit;
        }
        static object get_NpcStart(IRecord _this)
        {
            return ((MissionBaseRecord)_this).NpcStart;
        }
        static object get_NpcScene(IRecord _this)
        {
            return ((MissionBaseRecord)_this).NpcScene;
        }
        static object get_PosX(IRecord _this)
        {
            return ((MissionBaseRecord)_this).PosX;
        }
        static object get_PosY(IRecord _this)
        {
            return ((MissionBaseRecord)_this).PosY;
        }
        static object get_TrackDescription(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TrackDescription;
        }
        static object get_TrackType(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TrackType;
        }
        static object get_TrackParam(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TrackParam;
        }
        static object get_FinishCondition(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishCondition;
        }
        static object get_FinishParam(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishParam;
        }
        static object get_FinishDescription(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishDescription;
        }
        static object get_FinishNpcId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishNpcId;
        }
        static object get_FinishSceneId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishSceneId;
        }
        static object get_FinishPosX(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishPosX;
        }
        static object get_FinishPosY(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishPosY;
        }
        static object get_TalkId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TalkId;
        }
        static object get_DialogueNpc(IRecord _this)
        {
            return ((MissionBaseRecord)_this).DialogueNpc;
        }
        static object get_PlayCollectionAct(IRecord _this)
        {
            return ((MissionBaseRecord)_this).PlayCollectionAct;
        }
        static object get_DialogueFinish(IRecord _this)
        {
            return ((MissionBaseRecord)_this).DialogueFinish;
        }
        static object get_NextMission(IRecord _this)
        {
            return ((MissionBaseRecord)_this).NextMission;
        }
        static object get_JbId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).JbId;
        }
        static object get_RewardItem(IRecord _this)
        {
            return ((MissionBaseRecord)_this).RewardItem;
        }
        static object get_RewardItemCount(IRecord _this)
        {
            return ((MissionBaseRecord)_this).RewardItemCount;
        }
        static object get_StoryId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).StoryId;
        }
        static object get_TriggerActive(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TriggerActive;
        }
        static object get_TriggerClose(IRecord _this)
        {
            return ((MissionBaseRecord)_this).TriggerClose;
        }
        static object get_FlagId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FlagId;
        }
        static object get_BuffAdd(IRecord _this)
        {
            return ((MissionBaseRecord)_this).BuffAdd;
        }
        static object get_BuffClean(IRecord _this)
        {
            return ((MissionBaseRecord)_this).BuffClean;
        }
        static object get_RewardTitle(IRecord _this)
        {
            return ((MissionBaseRecord)_this).RewardTitle;
        }
        static object get_RoleRewardId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).RoleRewardId;
        }
        static object get_RoleRewardCount(IRecord _this)
        {
            return ((MissionBaseRecord)_this).RoleRewardCount;
        }
        static object get_GetPlay(IRecord _this)
        {
            return ((MissionBaseRecord)_this).GetPlay;
        }
        static object get_FinishPlay(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishPlay;
        }
        static object get_PayPlay(IRecord _this)
        {
            return ((MissionBaseRecord)_this).PayPlay;
        }
        static object get_GetStop(IRecord _this)
        {
            return ((MissionBaseRecord)_this).GetStop;
        }
        static object get_FinishStop(IRecord _this)
        {
            return ((MissionBaseRecord)_this).FinishStop;
        }
        static object get_PayStop(IRecord _this)
        {
            return ((MissionBaseRecord)_this).PayStop;
        }
        static object get_AcceptTaskMusic(IRecord _this)
        {
            return ((MissionBaseRecord)_this).AcceptTaskMusic;
        }
        static object get_DeliveryTaskMusic(IRecord _this)
        {
            return ((MissionBaseRecord)_this).DeliveryTaskMusic;
        }
        static object get_IsDynamicExp(IRecord _this)
        {
            return ((MissionBaseRecord)_this).IsDynamicExp;
        }
        static object get_DynamicExpRatio(IRecord _this)
        {
            return ((MissionBaseRecord)_this).DynamicExpRatio;
        }
        static object get_UIGuideId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).UIGuideId;
        }
        static object get_UIGuideTab(IRecord _this)
        {
            return ((MissionBaseRecord)_this).UIGuideTab;
        }
        static object get_RewardDictId(IRecord _this)
        {
            return ((MissionBaseRecord)_this).RewardDictId;
        }
        static object get_MissionBianHao(IRecord _this)
        {
            return ((MissionBaseRecord)_this).MissionBianHao;
        }
        static MissionBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["ViewType"] = get_ViewType;
            mField["Condition"] = get_Condition;
            mField["CanDrop"] = get_CanDrop;
            mField["TimeLimit"] = get_TimeLimit;
            mField["NpcStart"] = get_NpcStart;
            mField["NpcScene"] = get_NpcScene;
            mField["PosX"] = get_PosX;
            mField["PosY"] = get_PosY;
            mField["TrackDescription"] = get_TrackDescription;
            mField["TrackType"] = get_TrackType;
            mField["TrackParam"] = get_TrackParam;
            mField["FinishCondition"] = get_FinishCondition;
            mField["FinishParam"] = get_FinishParam;
            mField["FinishDescription"] = get_FinishDescription;
            mField["FinishNpcId"] = get_FinishNpcId;
            mField["FinishSceneId"] = get_FinishSceneId;
            mField["FinishPosX"] = get_FinishPosX;
            mField["FinishPosY"] = get_FinishPosY;
            mField["TalkId"] = get_TalkId;
            mField["DialogueNpc"] = get_DialogueNpc;
            mField["PlayCollectionAct"] = get_PlayCollectionAct;
            mField["DialogueFinish"] = get_DialogueFinish;
            mField["NextMission"] = get_NextMission;
            mField["JbId"] = get_JbId;
            mField["RewardItem"] = get_RewardItem;
            mField["RewardItemCount"] = get_RewardItemCount;
            mField["StoryId"] = get_StoryId;
            mField["TriggerActive"] = get_TriggerActive;
            mField["TriggerClose"] = get_TriggerClose;
            mField["FlagId"] = get_FlagId;
            mField["BuffAdd"] = get_BuffAdd;
            mField["BuffClean"] = get_BuffClean;
            mField["RewardTitle"] = get_RewardTitle;
            mField["RoleRewardId"] = get_RoleRewardId;
            mField["RoleRewardCount"] = get_RoleRewardCount;
            mField["GetPlay"] = get_GetPlay;
            mField["FinishPlay"] = get_FinishPlay;
            mField["PayPlay"] = get_PayPlay;
            mField["GetStop"] = get_GetStop;
            mField["FinishStop"] = get_FinishStop;
            mField["PayStop"] = get_PayStop;
            mField["AcceptTaskMusic"] = get_AcceptTaskMusic;
            mField["DeliveryTaskMusic"] = get_DeliveryTaskMusic;
            mField["IsDynamicExp"] = get_IsDynamicExp;
            mField["DynamicExpRatio"] = get_DynamicExpRatio;
            mField["UIGuideId"] = get_UIGuideId;
            mField["UIGuideTab"] = get_UIGuideTab;
            mField["RewardDictId"] = get_RewardDictId;
            mField["MissionBianHao"] = get_MissionBianHao;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AttrRefRecord :IRecord
    {
        public static string __TableName = "AttrRef.txt";
        public int Id { get; private set; }
        public int CharacterId { get; private set; }
        public int AttrId { get; private set; }
        public string Desc { get; private set; }
        public int[] Attr = new int[14];
        public int[] PropPercent = new int[12];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                CharacterId = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                Attr[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPercent[11] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AttrRefRecord)_this).Id;
        }
        static object get_CharacterId(IRecord _this)
        {
            return ((AttrRefRecord)_this).CharacterId;
        }
        static object get_AttrId(IRecord _this)
        {
            return ((AttrRefRecord)_this).AttrId;
        }
        static object get_Desc(IRecord _this)
        {
            return ((AttrRefRecord)_this).Desc;
        }
        static object get_Attr(IRecord _this)
        {
            return ((AttrRefRecord)_this).Attr;
        }
        static object get_PropPercent(IRecord _this)
        {
            return ((AttrRefRecord)_this).PropPercent;
        }
        static AttrRefRecord()
        {
            mField["Id"] = get_Id;
            mField["CharacterId"] = get_CharacterId;
            mField["AttrId"] = get_AttrId;
            mField["Desc"] = get_Desc;
            mField["Attr"] = get_Attr;
            mField["PropPercent"] = get_PropPercent;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipRelateRecord :IRecord
    {
        public static string __TableName = "EquipRelate.txt";
        public int Id { get; private set; }
        public int[] AttrCount = new int[7];
        public int[] Value = new int[4];
        public int[] Slot = new int[5];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrCount[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Slot[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Slot[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Slot[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Slot[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Slot[4] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipRelateRecord)_this).Id;
        }
        static object get_AttrCount(IRecord _this)
        {
            return ((EquipRelateRecord)_this).AttrCount;
        }
        static object get_Value(IRecord _this)
        {
            return ((EquipRelateRecord)_this).Value;
        }
        static object get_Slot(IRecord _this)
        {
            return ((EquipRelateRecord)_this).Slot;
        }
        static EquipRelateRecord()
        {
            mField["Id"] = get_Id;
            mField["AttrCount"] = get_AttrCount;
            mField["Value"] = get_Value;
            mField["Slot"] = get_Slot;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipEnchantRecord :IRecord
    {
        public static string __TableName = "EquipEnchant.txt";
        public int Id { get; private set; }
        public int[] Attr = new int[23];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[14] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[15] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[16] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[17] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[18] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[19] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[20] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[21] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[22] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipEnchantRecord)_this).Id;
        }
        static object get_Attr(IRecord _this)
        {
            return ((EquipEnchantRecord)_this).Attr;
        }
        static EquipEnchantRecord()
        {
            mField["Id"] = get_Id;
            mField["Attr"] = get_Attr;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipEnchantChanceRecord :IRecord
    {
        public static string __TableName = "EquipEnchantChance.txt";
        public int Id { get; private set; }
        public int[] Attr = new int[23];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[14] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[15] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[16] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[17] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[18] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[19] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[20] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[21] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr[22] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipEnchantChanceRecord)_this).Id;
        }
        static object get_Attr(IRecord _this)
        {
            return ((EquipEnchantChanceRecord)_this).Attr;
        }
        static EquipEnchantChanceRecord()
        {
            mField["Id"] = get_Id;
            mField["Attr"] = get_Attr;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TitleRecord :IRecord
    {
        public static string __TableName = "Title.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public int NameType { get; private set; }
        public int Mutex { get; private set; }
        public int Level { get; private set; }
        public int[] Buff = new int[2];
        public int Time { get; private set; }
        public string Icon { get; private set; }
        public int XDeviation { get; private set; }
        public int YDeviation { get; private set; }
        public int FrameColor { get; private set; }
        public int[] ChangeColor = new int[2];
        public int ExtraData1 { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                NameType = Table_Tamplet.Convert_Int(temp[__column++]);
                Mutex = Table_Tamplet.Convert_Int(temp[__column++]);
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                Buff[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Buff[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Time = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = temp[__column++];
                XDeviation = Table_Tamplet.Convert_Int(temp[__column++]);
                YDeviation = Table_Tamplet.Convert_Int(temp[__column++]);
                FrameColor = Table_Tamplet.Convert_Int(temp[__column++]);
                ChangeColor[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ChangeColor[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExtraData1 = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TitleRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((TitleRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((TitleRecord)_this).Desc;
        }
        static object get_NameType(IRecord _this)
        {
            return ((TitleRecord)_this).NameType;
        }
        static object get_Mutex(IRecord _this)
        {
            return ((TitleRecord)_this).Mutex;
        }
        static object get_Level(IRecord _this)
        {
            return ((TitleRecord)_this).Level;
        }
        static object get_Buff(IRecord _this)
        {
            return ((TitleRecord)_this).Buff;
        }
        static object get_Time(IRecord _this)
        {
            return ((TitleRecord)_this).Time;
        }
        static object get_Icon(IRecord _this)
        {
            return ((TitleRecord)_this).Icon;
        }
        static object get_XDeviation(IRecord _this)
        {
            return ((TitleRecord)_this).XDeviation;
        }
        static object get_YDeviation(IRecord _this)
        {
            return ((TitleRecord)_this).YDeviation;
        }
        static object get_FrameColor(IRecord _this)
        {
            return ((TitleRecord)_this).FrameColor;
        }
        static object get_ChangeColor(IRecord _this)
        {
            return ((TitleRecord)_this).ChangeColor;
        }
        static object get_ExtraData1(IRecord _this)
        {
            return ((TitleRecord)_this).ExtraData1;
        }
        static TitleRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["NameType"] = get_NameType;
            mField["Mutex"] = get_Mutex;
            mField["Level"] = get_Level;
            mField["Buff"] = get_Buff;
            mField["Time"] = get_Time;
            mField["Icon"] = get_Icon;
            mField["XDeviation"] = get_XDeviation;
            mField["YDeviation"] = get_YDeviation;
            mField["FrameColor"] = get_FrameColor;
            mField["ChangeColor"] = get_ChangeColor;
            mField["ExtraData1"] = get_ExtraData1;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipEnchanceRecord :IRecord
    {
        public static string __TableName = "EquipEnchance.txt";
        public int Id { get; private set; }
        public int[] Count = new int[5];
        public int[] Color = new int[5];
        public int[] Level = new int[4];
        public int[] Value = new int[5];
        public int[] Need = new int[5];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Color[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Color[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Color[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Color[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Color[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Level[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Level[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Level[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Level[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Value[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Need[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Need[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Need[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Need[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Need[4] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipEnchanceRecord)_this).Id;
        }
        static object get_Count(IRecord _this)
        {
            return ((EquipEnchanceRecord)_this).Count;
        }
        static object get_Color(IRecord _this)
        {
            return ((EquipEnchanceRecord)_this).Color;
        }
        static object get_Level(IRecord _this)
        {
            return ((EquipEnchanceRecord)_this).Level;
        }
        static object get_Value(IRecord _this)
        {
            return ((EquipEnchanceRecord)_this).Value;
        }
        static object get_Need(IRecord _this)
        {
            return ((EquipEnchanceRecord)_this).Need;
        }
        static EquipEnchanceRecord()
        {
            mField["Id"] = get_Id;
            mField["Count"] = get_Count;
            mField["Color"] = get_Color;
            mField["Level"] = get_Level;
            mField["Value"] = get_Value;
            mField["Need"] = get_Need;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class LevelDataRecord :IRecord
    {
        public static string __TableName = "LevelData.txt";
        public int Id { get; private set; }
        public int NeedExp { get; private set; }
        public int ExpMax { get; private set; }
        public double Dodge { get; private set; }
        public double Hit { get; private set; }
        public int PhyPowerMinScale { get; private set; }
        public int PhyPowerMinFix { get; private set; }
        public int PhyPowerMaxScale { get; private set; }
        public int PhyPowerMaxFix { get; private set; }
        public int MagPowerMinScale { get; private set; }
        public int MagPowerMinFix { get; private set; }
        public int MagPowerMaxScale { get; private set; }
        public int MagPowerMaxFix { get; private set; }
        public int PhyArmorScale { get; private set; }
        public int PhyArmorFix { get; private set; }
        public int MagArmorScale { get; private set; }
        public int MagArmorFix { get; private set; }
        public int HpMaxScale { get; private set; }
        public int HpMaxFix { get; private set; }
        public int PowerFightPoint { get; private set; }
        public int ArmorFightPoint { get; private set; }
        public int HpFightPoint { get; private set; }
        public int MpFightPoint { get; private set; }
        public int HitFightPoint { get; private set; }
        public int DodgeFightPoint { get; private set; }
        public int IgnoreArmorProFightPoint { get; private set; }
        public int DamageAddProFightPoint { get; private set; }
        public int DamageResProFightPoint { get; private set; }
        public int ExcellentProFightPoint { get; private set; }
        public int LuckyProFightPoint { get; private set; }
        public int DamageReboundProFightPoint { get; private set; }
        public int LeaveExpBase { get; private set; }
        public int FightingWayExp { get; private set; }
        public int FightingWayIncome { get; private set; }
        public int ElfExp { get; private set; }
        public int ElfResolveValue { get; private set; }
        public int[] Exp = new int[5];
        public int FixedGorw { get; private set; }
        public int PercentGrow { get; private set; }
        public int UpNeedExp { get; private set; }
        public int[] FruitLimit = new int[4];
        public int DynamicExp { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedExp = Table_Tamplet.Convert_Int(temp[__column++]);
                ExpMax = Table_Tamplet.Convert_Int(temp[__column++]);
                Dodge = Table_Tamplet.Convert_Double(temp[__column++]);
                Hit = Table_Tamplet.Convert_Double(temp[__column++]);
                PhyPowerMinScale = Table_Tamplet.Convert_Int(temp[__column++]);
                PhyPowerMinFix = Table_Tamplet.Convert_Int(temp[__column++]);
                PhyPowerMaxScale = Table_Tamplet.Convert_Int(temp[__column++]);
                PhyPowerMaxFix = Table_Tamplet.Convert_Int(temp[__column++]);
                MagPowerMinScale = Table_Tamplet.Convert_Int(temp[__column++]);
                MagPowerMinFix = Table_Tamplet.Convert_Int(temp[__column++]);
                MagPowerMaxScale = Table_Tamplet.Convert_Int(temp[__column++]);
                MagPowerMaxFix = Table_Tamplet.Convert_Int(temp[__column++]);
                PhyArmorScale = Table_Tamplet.Convert_Int(temp[__column++]);
                PhyArmorFix = Table_Tamplet.Convert_Int(temp[__column++]);
                MagArmorScale = Table_Tamplet.Convert_Int(temp[__column++]);
                MagArmorFix = Table_Tamplet.Convert_Int(temp[__column++]);
                HpMaxScale = Table_Tamplet.Convert_Int(temp[__column++]);
                HpMaxFix = Table_Tamplet.Convert_Int(temp[__column++]);
                PowerFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                ArmorFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                HpFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                MpFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                HitFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                DodgeFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                IgnoreArmorProFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                DamageAddProFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                DamageResProFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                ExcellentProFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                LuckyProFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                DamageReboundProFightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                LeaveExpBase = Table_Tamplet.Convert_Int(temp[__column++]);
                FightingWayExp = Table_Tamplet.Convert_Int(temp[__column++]);
                FightingWayIncome = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfExp = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfResolveValue = Table_Tamplet.Convert_Int(temp[__column++]);
                Exp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Exp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                FixedGorw = Table_Tamplet.Convert_Int(temp[__column++]);
                PercentGrow = Table_Tamplet.Convert_Int(temp[__column++]);
                UpNeedExp = Table_Tamplet.Convert_Int(temp[__column++]);
                FruitLimit[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FruitLimit[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FruitLimit[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FruitLimit[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                DynamicExp = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((LevelDataRecord)_this).Id;
        }
        static object get_NeedExp(IRecord _this)
        {
            return ((LevelDataRecord)_this).NeedExp;
        }
        static object get_ExpMax(IRecord _this)
        {
            return ((LevelDataRecord)_this).ExpMax;
        }
        static object get_Dodge(IRecord _this)
        {
            return ((LevelDataRecord)_this).Dodge;
        }
        static object get_Hit(IRecord _this)
        {
            return ((LevelDataRecord)_this).Hit;
        }
        static object get_PhyPowerMinScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).PhyPowerMinScale;
        }
        static object get_PhyPowerMinFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).PhyPowerMinFix;
        }
        static object get_PhyPowerMaxScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).PhyPowerMaxScale;
        }
        static object get_PhyPowerMaxFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).PhyPowerMaxFix;
        }
        static object get_MagPowerMinScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).MagPowerMinScale;
        }
        static object get_MagPowerMinFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).MagPowerMinFix;
        }
        static object get_MagPowerMaxScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).MagPowerMaxScale;
        }
        static object get_MagPowerMaxFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).MagPowerMaxFix;
        }
        static object get_PhyArmorScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).PhyArmorScale;
        }
        static object get_PhyArmorFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).PhyArmorFix;
        }
        static object get_MagArmorScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).MagArmorScale;
        }
        static object get_MagArmorFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).MagArmorFix;
        }
        static object get_HpMaxScale(IRecord _this)
        {
            return ((LevelDataRecord)_this).HpMaxScale;
        }
        static object get_HpMaxFix(IRecord _this)
        {
            return ((LevelDataRecord)_this).HpMaxFix;
        }
        static object get_PowerFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).PowerFightPoint;
        }
        static object get_ArmorFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).ArmorFightPoint;
        }
        static object get_HpFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).HpFightPoint;
        }
        static object get_MpFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).MpFightPoint;
        }
        static object get_HitFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).HitFightPoint;
        }
        static object get_DodgeFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).DodgeFightPoint;
        }
        static object get_IgnoreArmorProFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).IgnoreArmorProFightPoint;
        }
        static object get_DamageAddProFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).DamageAddProFightPoint;
        }
        static object get_DamageResProFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).DamageResProFightPoint;
        }
        static object get_ExcellentProFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).ExcellentProFightPoint;
        }
        static object get_LuckyProFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).LuckyProFightPoint;
        }
        static object get_DamageReboundProFightPoint(IRecord _this)
        {
            return ((LevelDataRecord)_this).DamageReboundProFightPoint;
        }
        static object get_LeaveExpBase(IRecord _this)
        {
            return ((LevelDataRecord)_this).LeaveExpBase;
        }
        static object get_FightingWayExp(IRecord _this)
        {
            return ((LevelDataRecord)_this).FightingWayExp;
        }
        static object get_FightingWayIncome(IRecord _this)
        {
            return ((LevelDataRecord)_this).FightingWayIncome;
        }
        static object get_ElfExp(IRecord _this)
        {
            return ((LevelDataRecord)_this).ElfExp;
        }
        static object get_ElfResolveValue(IRecord _this)
        {
            return ((LevelDataRecord)_this).ElfResolveValue;
        }
        static object get_Exp(IRecord _this)
        {
            return ((LevelDataRecord)_this).Exp;
        }
        static object get_FixedGorw(IRecord _this)
        {
            return ((LevelDataRecord)_this).FixedGorw;
        }
        static object get_PercentGrow(IRecord _this)
        {
            return ((LevelDataRecord)_this).PercentGrow;
        }
        static object get_UpNeedExp(IRecord _this)
        {
            return ((LevelDataRecord)_this).UpNeedExp;
        }
        static object get_FruitLimit(IRecord _this)
        {
            return ((LevelDataRecord)_this).FruitLimit;
        }
        static object get_DynamicExp(IRecord _this)
        {
            return ((LevelDataRecord)_this).DynamicExp;
        }
        static LevelDataRecord()
        {
            mField["Id"] = get_Id;
            mField["NeedExp"] = get_NeedExp;
            mField["ExpMax"] = get_ExpMax;
            mField["Dodge"] = get_Dodge;
            mField["Hit"] = get_Hit;
            mField["PhyPowerMinScale"] = get_PhyPowerMinScale;
            mField["PhyPowerMinFix"] = get_PhyPowerMinFix;
            mField["PhyPowerMaxScale"] = get_PhyPowerMaxScale;
            mField["PhyPowerMaxFix"] = get_PhyPowerMaxFix;
            mField["MagPowerMinScale"] = get_MagPowerMinScale;
            mField["MagPowerMinFix"] = get_MagPowerMinFix;
            mField["MagPowerMaxScale"] = get_MagPowerMaxScale;
            mField["MagPowerMaxFix"] = get_MagPowerMaxFix;
            mField["PhyArmorScale"] = get_PhyArmorScale;
            mField["PhyArmorFix"] = get_PhyArmorFix;
            mField["MagArmorScale"] = get_MagArmorScale;
            mField["MagArmorFix"] = get_MagArmorFix;
            mField["HpMaxScale"] = get_HpMaxScale;
            mField["HpMaxFix"] = get_HpMaxFix;
            mField["PowerFightPoint"] = get_PowerFightPoint;
            mField["ArmorFightPoint"] = get_ArmorFightPoint;
            mField["HpFightPoint"] = get_HpFightPoint;
            mField["MpFightPoint"] = get_MpFightPoint;
            mField["HitFightPoint"] = get_HitFightPoint;
            mField["DodgeFightPoint"] = get_DodgeFightPoint;
            mField["IgnoreArmorProFightPoint"] = get_IgnoreArmorProFightPoint;
            mField["DamageAddProFightPoint"] = get_DamageAddProFightPoint;
            mField["DamageResProFightPoint"] = get_DamageResProFightPoint;
            mField["ExcellentProFightPoint"] = get_ExcellentProFightPoint;
            mField["LuckyProFightPoint"] = get_LuckyProFightPoint;
            mField["DamageReboundProFightPoint"] = get_DamageReboundProFightPoint;
            mField["LeaveExpBase"] = get_LeaveExpBase;
            mField["FightingWayExp"] = get_FightingWayExp;
            mField["FightingWayIncome"] = get_FightingWayIncome;
            mField["ElfExp"] = get_ElfExp;
            mField["ElfResolveValue"] = get_ElfResolveValue;
            mField["Exp"] = get_Exp;
            mField["FixedGorw"] = get_FixedGorw;
            mField["PercentGrow"] = get_PercentGrow;
            mField["UpNeedExp"] = get_UpNeedExp;
            mField["FruitLimit"] = get_FruitLimit;
            mField["DynamicExp"] = get_DynamicExp;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BulletRecord :IRecord
    {
        public static string __TableName = "Bullet.txt";
        public int Id { get; private set; }
        public string Path { get; private set; }
        public float Speed { get;        set; }
        public int FlySound { get; private set; }
        public string HitEffect { get; private set; }
        public int HitSound { get; private set; }
        public int[] Buff = new int[2];
        public int CasterMountPoint { get; private set; }
        public int BearMountPoint { get; private set; }
        public float DirRangeX { get;        set; }
        public float DirRangeY { get;        set; }
        public float DirRangeZ { get;        set; }
        public int ShotDelay { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Path = temp[__column++];
                Speed = Table_Tamplet.Convert_Float(temp[__column++]);
                FlySound = Table_Tamplet.Convert_Int(temp[__column++]);
                HitEffect = temp[__column++];
                HitSound = Table_Tamplet.Convert_Int(temp[__column++]);
                Buff[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Buff[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                CasterMountPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                BearMountPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                DirRangeX = Table_Tamplet.Convert_Float(temp[__column++]);
                DirRangeY = Table_Tamplet.Convert_Float(temp[__column++]);
                DirRangeZ = Table_Tamplet.Convert_Float(temp[__column++]);
                ShotDelay = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BulletRecord)_this).Id;
        }
        static object get_Path(IRecord _this)
        {
            return ((BulletRecord)_this).Path;
        }
        static object get_Speed(IRecord _this)
        {
            return ((BulletRecord)_this).Speed;
        }
        static object get_FlySound(IRecord _this)
        {
            return ((BulletRecord)_this).FlySound;
        }
        static object get_HitEffect(IRecord _this)
        {
            return ((BulletRecord)_this).HitEffect;
        }
        static object get_HitSound(IRecord _this)
        {
            return ((BulletRecord)_this).HitSound;
        }
        static object get_Buff(IRecord _this)
        {
            return ((BulletRecord)_this).Buff;
        }
        static object get_CasterMountPoint(IRecord _this)
        {
            return ((BulletRecord)_this).CasterMountPoint;
        }
        static object get_BearMountPoint(IRecord _this)
        {
            return ((BulletRecord)_this).BearMountPoint;
        }
        static object get_DirRangeX(IRecord _this)
        {
            return ((BulletRecord)_this).DirRangeX;
        }
        static object get_DirRangeY(IRecord _this)
        {
            return ((BulletRecord)_this).DirRangeY;
        }
        static object get_DirRangeZ(IRecord _this)
        {
            return ((BulletRecord)_this).DirRangeZ;
        }
        static object get_ShotDelay(IRecord _this)
        {
            return ((BulletRecord)_this).ShotDelay;
        }
        static BulletRecord()
        {
            mField["Id"] = get_Id;
            mField["Path"] = get_Path;
            mField["Speed"] = get_Speed;
            mField["FlySound"] = get_FlySound;
            mField["HitEffect"] = get_HitEffect;
            mField["HitSound"] = get_HitSound;
            mField["Buff"] = get_Buff;
            mField["CasterMountPoint"] = get_CasterMountPoint;
            mField["BearMountPoint"] = get_BearMountPoint;
            mField["DirRangeX"] = get_DirRangeX;
            mField["DirRangeY"] = get_DirRangeY;
            mField["DirRangeZ"] = get_DirRangeZ;
            mField["ShotDelay"] = get_ShotDelay;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class NpcBaseRecord :IRecord
    {
        public static string __TableName = "NpcBase.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int mAI { get; private set; }
        public int NpcType { get; private set; }
        public string[] pop = new string[3];
        public int Level { get; private set; }
        public int DialogRadius { get; private set; }
        public int Patrol { get; private set; }
        public double PatrolRadius { get; private set; }
        public double ViewDistance { get; private set; }
        public double MaxCombatDistance { get; private set; }
        public int BornEffctID { get; private set; }
        public int DieEffectID { get; private set; }
        public int IsAttackFly { get; private set; }
        public int CorpseTime { get; private set; }
        public int IsReviveTime { get; private set; }
        public int ReviveTime { get; private set; }
        public int Exp { get; private set; }
        public int AIID { get; private set; }
        public int BelongType { get; private set; }
        public int DropId { get; private set; }
        public int Interactive { get; private set; }
        public int[] Service = new int[2];
        public int Spare { get; private set; }
        public int IsForwardYou { get; private set; }
        public float NPCStopRadius { get;        set; }
        public int DialogSound { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                mAI = Table_Tamplet.Convert_Int(temp[__column++]);
                NpcType = Table_Tamplet.Convert_Int(temp[__column++]);
                pop[0]  = temp[__column++];
                pop[1]  = temp[__column++];
                pop[2]  = temp[__column++];
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                DialogRadius = Table_Tamplet.Convert_Int(temp[__column++]);
                Patrol = Table_Tamplet.Convert_Int(temp[__column++]);
                PatrolRadius = Table_Tamplet.Convert_Double(temp[__column++]);
                ViewDistance = Table_Tamplet.Convert_Double(temp[__column++]);
                MaxCombatDistance = Table_Tamplet.Convert_Double(temp[__column++]);
                BornEffctID = Table_Tamplet.Convert_Int(temp[__column++]);
                DieEffectID = Table_Tamplet.Convert_Int(temp[__column++]);
                IsAttackFly = Table_Tamplet.Convert_Int(temp[__column++]);
                CorpseTime = Table_Tamplet.Convert_Int(temp[__column++]);
                IsReviveTime = Table_Tamplet.Convert_Int(temp[__column++]);
                ReviveTime = Table_Tamplet.Convert_Int(temp[__column++]);
                Exp = Table_Tamplet.Convert_Int(temp[__column++]);
                AIID = Table_Tamplet.Convert_Int(temp[__column++]);
                BelongType = Table_Tamplet.Convert_Int(temp[__column++]);
                DropId = Table_Tamplet.Convert_Int(temp[__column++]);
                Interactive = Table_Tamplet.Convert_Int(temp[__column++]);
                Service[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Service[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Spare = Table_Tamplet.Convert_Int(temp[__column++]);
                IsForwardYou = Table_Tamplet.Convert_Int(temp[__column++]);
                NPCStopRadius = Table_Tamplet.Convert_Float(temp[__column++]);
                DialogSound = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Name;
        }
        static object get_mAI(IRecord _this)
        {
            return ((NpcBaseRecord)_this).mAI;
        }
        static object get_NpcType(IRecord _this)
        {
            return ((NpcBaseRecord)_this).NpcType;
        }
        static object get_pop(IRecord _this)
        {
            return ((NpcBaseRecord)_this).pop;
        }
        static object get_Level(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Level;
        }
        static object get_DialogRadius(IRecord _this)
        {
            return ((NpcBaseRecord)_this).DialogRadius;
        }
        static object get_Patrol(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Patrol;
        }
        static object get_PatrolRadius(IRecord _this)
        {
            return ((NpcBaseRecord)_this).PatrolRadius;
        }
        static object get_ViewDistance(IRecord _this)
        {
            return ((NpcBaseRecord)_this).ViewDistance;
        }
        static object get_MaxCombatDistance(IRecord _this)
        {
            return ((NpcBaseRecord)_this).MaxCombatDistance;
        }
        static object get_BornEffctID(IRecord _this)
        {
            return ((NpcBaseRecord)_this).BornEffctID;
        }
        static object get_DieEffectID(IRecord _this)
        {
            return ((NpcBaseRecord)_this).DieEffectID;
        }
        static object get_IsAttackFly(IRecord _this)
        {
            return ((NpcBaseRecord)_this).IsAttackFly;
        }
        static object get_CorpseTime(IRecord _this)
        {
            return ((NpcBaseRecord)_this).CorpseTime;
        }
        static object get_IsReviveTime(IRecord _this)
        {
            return ((NpcBaseRecord)_this).IsReviveTime;
        }
        static object get_ReviveTime(IRecord _this)
        {
            return ((NpcBaseRecord)_this).ReviveTime;
        }
        static object get_Exp(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Exp;
        }
        static object get_AIID(IRecord _this)
        {
            return ((NpcBaseRecord)_this).AIID;
        }
        static object get_BelongType(IRecord _this)
        {
            return ((NpcBaseRecord)_this).BelongType;
        }
        static object get_DropId(IRecord _this)
        {
            return ((NpcBaseRecord)_this).DropId;
        }
        static object get_Interactive(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Interactive;
        }
        static object get_Service(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Service;
        }
        static object get_Spare(IRecord _this)
        {
            return ((NpcBaseRecord)_this).Spare;
        }
        static object get_IsForwardYou(IRecord _this)
        {
            return ((NpcBaseRecord)_this).IsForwardYou;
        }
        static object get_NPCStopRadius(IRecord _this)
        {
            return ((NpcBaseRecord)_this).NPCStopRadius;
        }
        static object get_DialogSound(IRecord _this)
        {
            return ((NpcBaseRecord)_this).DialogSound;
        }
        static NpcBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["mAI"] = get_mAI;
            mField["NpcType"] = get_NpcType;
            mField["pop"] = get_pop;
            mField["Level"] = get_Level;
            mField["DialogRadius"] = get_DialogRadius;
            mField["Patrol"] = get_Patrol;
            mField["PatrolRadius"] = get_PatrolRadius;
            mField["ViewDistance"] = get_ViewDistance;
            mField["MaxCombatDistance"] = get_MaxCombatDistance;
            mField["BornEffctID"] = get_BornEffctID;
            mField["DieEffectID"] = get_DieEffectID;
            mField["IsAttackFly"] = get_IsAttackFly;
            mField["CorpseTime"] = get_CorpseTime;
            mField["IsReviveTime"] = get_IsReviveTime;
            mField["ReviveTime"] = get_ReviveTime;
            mField["Exp"] = get_Exp;
            mField["AIID"] = get_AIID;
            mField["BelongType"] = get_BelongType;
            mField["DropId"] = get_DropId;
            mField["Interactive"] = get_Interactive;
            mField["Service"] = get_Service;
            mField["Spare"] = get_Spare;
            mField["IsForwardYou"] = get_IsForwardYou;
            mField["NPCStopRadius"] = get_NPCStopRadius;
            mField["DialogSound"] = get_DialogSound;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SkillUpgradingRecord :IRecord
    {
        public static string __TableName = "SkillUpgrading.txt";
        public int Id { get; private set; }
        public int Type { get; private set; }
        public List<int> Values = new List<int>();
        public int[] Param = new int[5];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(Values,temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SkillUpgradingRecord)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((SkillUpgradingRecord)_this).Type;
        }
        static object get_Values(IRecord _this)
        {
            return ((SkillUpgradingRecord)_this).Values;
        }
        static object get_Param(IRecord _this)
        {
            return ((SkillUpgradingRecord)_this).Param;
        }
        static SkillUpgradingRecord()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["Values"] = get_Values;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AchievementRecord :IRecord
    {
        public static string __TableName = "Achievement.txt";
        public int Id { get; private set; }
        public int Type { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int ViewLevel { get; private set; }
        public int Exdata { get; private set; }
        public int ExdataCount { get; private set; }
        public List<int> FlagList = new List<int>();
        public int AchievementPoint { get; private set; }
        [TableBinding("ItemBase")]
        public int[] ItemId = new int[3];
        public int[] ItemCount = new int[3];
        public int RewardFlagId { get; private set; }
        public int Priority { get; private set; }
        public int FinishFlagId { get; private set; }
        public int ClientDisplay { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                ViewLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                Exdata = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(FlagList,temp[__column++]);
                AchievementPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardFlagId = Table_Tamplet.Convert_Int(temp[__column++]);
                Priority = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishFlagId = Table_Tamplet.Convert_Int(temp[__column++]);
                ClientDisplay = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AchievementRecord)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((AchievementRecord)_this).Type;
        }
        static object get_Name(IRecord _this)
        {
            return ((AchievementRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((AchievementRecord)_this).Desc;
        }
        static object get_Icon(IRecord _this)
        {
            return ((AchievementRecord)_this).Icon;
        }
        static object get_ViewLevel(IRecord _this)
        {
            return ((AchievementRecord)_this).ViewLevel;
        }
        static object get_Exdata(IRecord _this)
        {
            return ((AchievementRecord)_this).Exdata;
        }
        static object get_ExdataCount(IRecord _this)
        {
            return ((AchievementRecord)_this).ExdataCount;
        }
        static object get_FlagList(IRecord _this)
        {
            return ((AchievementRecord)_this).FlagList;
        }
        static object get_AchievementPoint(IRecord _this)
        {
            return ((AchievementRecord)_this).AchievementPoint;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((AchievementRecord)_this).ItemId;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((AchievementRecord)_this).ItemCount;
        }
        static object get_RewardFlagId(IRecord _this)
        {
            return ((AchievementRecord)_this).RewardFlagId;
        }
        static object get_Priority(IRecord _this)
        {
            return ((AchievementRecord)_this).Priority;
        }
        static object get_FinishFlagId(IRecord _this)
        {
            return ((AchievementRecord)_this).FinishFlagId;
        }
        static object get_ClientDisplay(IRecord _this)
        {
            return ((AchievementRecord)_this).ClientDisplay;
        }
        static AchievementRecord()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["Icon"] = get_Icon;
            mField["ViewLevel"] = get_ViewLevel;
            mField["Exdata"] = get_Exdata;
            mField["ExdataCount"] = get_ExdataCount;
            mField["FlagList"] = get_FlagList;
            mField["AchievementPoint"] = get_AchievementPoint;
            mField["ItemId"] = get_ItemId;
            mField["ItemCount"] = get_ItemCount;
            mField["RewardFlagId"] = get_RewardFlagId;
            mField["Priority"] = get_Priority;
            mField["FinishFlagId"] = get_FinishFlagId;
            mField["ClientDisplay"] = get_ClientDisplay;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipTieRecord :IRecord
    {
        public static string __TableName = "EquipTie.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int AllCount { get; private set; }
        [ListSize(4)]
        public ReadonlyList<string> Desc { get; private set; } 
        public int[] NeedCount = new int[4];
        public int[] Attr1Id = new int[4];
        public int[] Attr1Value = new int[4];
        public int[] Attr2Id = new int[4];
        public int[] Attr2Value = new int[4];
        public int[] BuffId = new int[4];
        public int[] FightPoint = new int[4];
        [ListSize(10)]
        public ReadonlyList<string> Names { get; private set; } 
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                AllCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc=new ReadonlyList<string>(4);
                Desc[0]  = temp[__column++];
                NeedCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Id[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Value[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Id[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Value[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc[1]  = temp[__column++];
                NeedCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Id[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Value[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Id[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Value[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc[2]  = temp[__column++];
                NeedCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Id[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Value[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Id[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Value[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc[3]  = temp[__column++];
                NeedCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Id[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr1Value[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Id[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Attr2Value[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Names=new ReadonlyList<string>(10);
                Names[0]  = temp[__column++];
                Names[1]  = temp[__column++];
                Names[2]  = temp[__column++];
                Names[3]  = temp[__column++];
                Names[4]  = temp[__column++];
                Names[5]  = temp[__column++];
                Names[6]  = temp[__column++];
                Names[7]  = temp[__column++];
                Names[8]  = temp[__column++];
                Names[9]  = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipTieRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((EquipTieRecord)_this).Name;
        }
        static object get_AllCount(IRecord _this)
        {
            return ((EquipTieRecord)_this).AllCount;
        }
        static object get_Desc(IRecord _this)
        {
            return ((EquipTieRecord)_this).Desc;
        }
        static object get_NeedCount(IRecord _this)
        {
            return ((EquipTieRecord)_this).NeedCount;
        }
        static object get_Attr1Id(IRecord _this)
        {
            return ((EquipTieRecord)_this).Attr1Id;
        }
        static object get_Attr1Value(IRecord _this)
        {
            return ((EquipTieRecord)_this).Attr1Value;
        }
        static object get_Attr2Id(IRecord _this)
        {
            return ((EquipTieRecord)_this).Attr2Id;
        }
        static object get_Attr2Value(IRecord _this)
        {
            return ((EquipTieRecord)_this).Attr2Value;
        }
        static object get_BuffId(IRecord _this)
        {
            return ((EquipTieRecord)_this).BuffId;
        }
        static object get_FightPoint(IRecord _this)
        {
            return ((EquipTieRecord)_this).FightPoint;
        }
        static object get_Names(IRecord _this)
        {
            return ((EquipTieRecord)_this).Names;
        }
        static EquipTieRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["AllCount"] = get_AllCount;
            mField["Desc"] = get_Desc;
            mField["NeedCount"] = get_NeedCount;
            mField["Attr1Id"] = get_Attr1Id;
            mField["Attr1Value"] = get_Attr1Value;
            mField["Attr2Id"] = get_Attr2Id;
            mField["Attr2Value"] = get_Attr2Value;
            mField["BuffId"] = get_BuffId;
            mField["FightPoint"] = get_FightPoint;
            mField["Names"] = get_Names;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EffectRecord :IRecord
    {
        public static string __TableName = "Effect.txt";
        public int Id { get; private set; }
        public string Path { get; private set; }
        public float DelayTime { get;        set; }
        public int MaxOwnNum { get; private set; }
        public int BroadcastType { get; private set; }
        public int[] ShakeType = new int[4];
        public int[] ShakeDelayTime = new int[4];
        public float[] ShakeMagnitude = new float[4];
        public float[] ShakeCount = new float[4];
        public float[] ShakeSpeed = new float[4];
        public float[] ShakeReduction = new float[4];
        public float X { get;        set; }
        public float Y { get;        set; }
        public float Z { get;        set; }
        public float RotationX { get;        set; }
        public float RotationY { get;        set; }
        public float RotationZ { get;        set; }
        public float Duration { get;        set; }
        public float LoopTime { get;        set; }
        public int MountPoint { get; private set; }
        public int Follow { get; private set; }
        public int IsZoom { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Path = temp[__column++];
                DelayTime = Table_Tamplet.Convert_Float(temp[__column++]);
                MaxOwnNum = Table_Tamplet.Convert_Int(temp[__column++]);
                BroadcastType = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeType[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeDelayTime[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeMagnitude[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeCount[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeSpeed[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeReduction[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeType[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeDelayTime[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeMagnitude[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeCount[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeSpeed[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeReduction[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeType[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeDelayTime[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeMagnitude[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeCount[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeSpeed[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeReduction[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeType[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeDelayTime[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShakeMagnitude[3] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeCount[3] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeSpeed[3] = Table_Tamplet.Convert_Float(temp[__column++]);
                ShakeReduction[3] = Table_Tamplet.Convert_Float(temp[__column++]);
                X = Table_Tamplet.Convert_Float(temp[__column++]);
                Y = Table_Tamplet.Convert_Float(temp[__column++]);
                Z = Table_Tamplet.Convert_Float(temp[__column++]);
                RotationX = Table_Tamplet.Convert_Float(temp[__column++]);
                RotationY = Table_Tamplet.Convert_Float(temp[__column++]);
                RotationZ = Table_Tamplet.Convert_Float(temp[__column++]);
                Duration = Table_Tamplet.Convert_Float(temp[__column++]);
                LoopTime = Table_Tamplet.Convert_Float(temp[__column++]);
                MountPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                Follow = Table_Tamplet.Convert_Int(temp[__column++]);
                IsZoom = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EffectRecord)_this).Id;
        }
        static object get_Path(IRecord _this)
        {
            return ((EffectRecord)_this).Path;
        }
        static object get_DelayTime(IRecord _this)
        {
            return ((EffectRecord)_this).DelayTime;
        }
        static object get_MaxOwnNum(IRecord _this)
        {
            return ((EffectRecord)_this).MaxOwnNum;
        }
        static object get_BroadcastType(IRecord _this)
        {
            return ((EffectRecord)_this).BroadcastType;
        }
        static object get_ShakeType(IRecord _this)
        {
            return ((EffectRecord)_this).ShakeType;
        }
        static object get_ShakeDelayTime(IRecord _this)
        {
            return ((EffectRecord)_this).ShakeDelayTime;
        }
        static object get_ShakeMagnitude(IRecord _this)
        {
            return ((EffectRecord)_this).ShakeMagnitude;
        }
        static object get_ShakeCount(IRecord _this)
        {
            return ((EffectRecord)_this).ShakeCount;
        }
        static object get_ShakeSpeed(IRecord _this)
        {
            return ((EffectRecord)_this).ShakeSpeed;
        }
        static object get_ShakeReduction(IRecord _this)
        {
            return ((EffectRecord)_this).ShakeReduction;
        }
        static object get_X(IRecord _this)
        {
            return ((EffectRecord)_this).X;
        }
        static object get_Y(IRecord _this)
        {
            return ((EffectRecord)_this).Y;
        }
        static object get_Z(IRecord _this)
        {
            return ((EffectRecord)_this).Z;
        }
        static object get_RotationX(IRecord _this)
        {
            return ((EffectRecord)_this).RotationX;
        }
        static object get_RotationY(IRecord _this)
        {
            return ((EffectRecord)_this).RotationY;
        }
        static object get_RotationZ(IRecord _this)
        {
            return ((EffectRecord)_this).RotationZ;
        }
        static object get_Duration(IRecord _this)
        {
            return ((EffectRecord)_this).Duration;
        }
        static object get_LoopTime(IRecord _this)
        {
            return ((EffectRecord)_this).LoopTime;
        }
        static object get_MountPoint(IRecord _this)
        {
            return ((EffectRecord)_this).MountPoint;
        }
        static object get_Follow(IRecord _this)
        {
            return ((EffectRecord)_this).Follow;
        }
        static object get_IsZoom(IRecord _this)
        {
            return ((EffectRecord)_this).IsZoom;
        }
        static EffectRecord()
        {
            mField["Id"] = get_Id;
            mField["Path"] = get_Path;
            mField["DelayTime"] = get_DelayTime;
            mField["MaxOwnNum"] = get_MaxOwnNum;
            mField["BroadcastType"] = get_BroadcastType;
            mField["ShakeType"] = get_ShakeType;
            mField["ShakeDelayTime"] = get_ShakeDelayTime;
            mField["ShakeMagnitude"] = get_ShakeMagnitude;
            mField["ShakeCount"] = get_ShakeCount;
            mField["ShakeSpeed"] = get_ShakeSpeed;
            mField["ShakeReduction"] = get_ShakeReduction;
            mField["X"] = get_X;
            mField["Y"] = get_Y;
            mField["Z"] = get_Z;
            mField["RotationX"] = get_RotationX;
            mField["RotationY"] = get_RotationY;
            mField["RotationZ"] = get_RotationZ;
            mField["Duration"] = get_Duration;
            mField["LoopTime"] = get_LoopTime;
            mField["MountPoint"] = get_MountPoint;
            mField["Follow"] = get_Follow;
            mField["IsZoom"] = get_IsZoom;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TransferRecord :IRecord
    {
        public static string __TableName = "Transfer.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int FromSceneId { get; private set; }
        public float FromX { get;        set; }
        public float FromY { get;        set; }
        public int NeedTime { get; private set; }
        public int ToSceneId { get; private set; }
        public float ToX { get;        set; }
        public float ToY { get;        set; }
        public float TransferRadius { get;        set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                FromSceneId = Table_Tamplet.Convert_Int(temp[__column++]);
                FromX = Table_Tamplet.Convert_Float(temp[__column++]);
                FromY = Table_Tamplet.Convert_Float(temp[__column++]);
                NeedTime = Table_Tamplet.Convert_Int(temp[__column++]);
                ToSceneId = Table_Tamplet.Convert_Int(temp[__column++]);
                ToX = Table_Tamplet.Convert_Float(temp[__column++]);
                ToY = Table_Tamplet.Convert_Float(temp[__column++]);
                TransferRadius = Table_Tamplet.Convert_Float(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TransferRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((TransferRecord)_this).Name;
        }
        static object get_FromSceneId(IRecord _this)
        {
            return ((TransferRecord)_this).FromSceneId;
        }
        static object get_FromX(IRecord _this)
        {
            return ((TransferRecord)_this).FromX;
        }
        static object get_FromY(IRecord _this)
        {
            return ((TransferRecord)_this).FromY;
        }
        static object get_NeedTime(IRecord _this)
        {
            return ((TransferRecord)_this).NeedTime;
        }
        static object get_ToSceneId(IRecord _this)
        {
            return ((TransferRecord)_this).ToSceneId;
        }
        static object get_ToX(IRecord _this)
        {
            return ((TransferRecord)_this).ToX;
        }
        static object get_ToY(IRecord _this)
        {
            return ((TransferRecord)_this).ToY;
        }
        static object get_TransferRadius(IRecord _this)
        {
            return ((TransferRecord)_this).TransferRadius;
        }
        static TransferRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["FromSceneId"] = get_FromSceneId;
            mField["FromX"] = get_FromX;
            mField["FromY"] = get_FromY;
            mField["NeedTime"] = get_NeedTime;
            mField["ToSceneId"] = get_ToSceneId;
            mField["ToX"] = get_ToX;
            mField["ToY"] = get_ToY;
            mField["TransferRadius"] = get_TransferRadius;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ClientConfigRecord :IRecord
    {
        public static string __TableName = "ClientConfig.txt";
        public int Id { get; private set; }
        public string Value { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Value = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ClientConfigRecord)_this).Id;
        }
        static object get_Value(IRecord _this)
        {
            return ((ClientConfigRecord)_this).Value;
        }
        static ClientConfigRecord()
        {
            mField["Id"] = get_Id;
            mField["Value"] = get_Value;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class WeaponMountRecord :IRecord
    {
        public static string __TableName = "WeaponMount.txt";
        public int Id { get; private set; }
        public string Path { get; private set; }
        public int Mount { get; private set; }
        public float PosX { get;        set; }
        public float PosY { get;        set; }
        public float PosZ { get;        set; }
        public float DirX { get;        set; }
        public float DirY { get;        set; }
        public float DirZ { get;        set; }
        public int[] Enchance = new int[16];
        public string ShowPath { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Path = temp[__column++];
                Mount = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PosY = Table_Tamplet.Convert_Float(temp[__column++]);
                PosZ = Table_Tamplet.Convert_Float(temp[__column++]);
                DirX = Table_Tamplet.Convert_Float(temp[__column++]);
                DirY = Table_Tamplet.Convert_Float(temp[__column++]);
                DirZ = Table_Tamplet.Convert_Float(temp[__column++]);
                Enchance[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[14] = Table_Tamplet.Convert_Int(temp[__column++]);
                Enchance[15] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowPath = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((WeaponMountRecord)_this).Id;
        }
        static object get_Path(IRecord _this)
        {
            return ((WeaponMountRecord)_this).Path;
        }
        static object get_Mount(IRecord _this)
        {
            return ((WeaponMountRecord)_this).Mount;
        }
        static object get_PosX(IRecord _this)
        {
            return ((WeaponMountRecord)_this).PosX;
        }
        static object get_PosY(IRecord _this)
        {
            return ((WeaponMountRecord)_this).PosY;
        }
        static object get_PosZ(IRecord _this)
        {
            return ((WeaponMountRecord)_this).PosZ;
        }
        static object get_DirX(IRecord _this)
        {
            return ((WeaponMountRecord)_this).DirX;
        }
        static object get_DirY(IRecord _this)
        {
            return ((WeaponMountRecord)_this).DirY;
        }
        static object get_DirZ(IRecord _this)
        {
            return ((WeaponMountRecord)_this).DirZ;
        }
        static object get_Enchance(IRecord _this)
        {
            return ((WeaponMountRecord)_this).Enchance;
        }
        static object get_ShowPath(IRecord _this)
        {
            return ((WeaponMountRecord)_this).ShowPath;
        }
        static WeaponMountRecord()
        {
            mField["Id"] = get_Id;
            mField["Path"] = get_Path;
            mField["Mount"] = get_Mount;
            mField["PosX"] = get_PosX;
            mField["PosY"] = get_PosY;
            mField["PosZ"] = get_PosZ;
            mField["DirX"] = get_DirX;
            mField["DirY"] = get_DirY;
            mField["DirZ"] = get_DirZ;
            mField["Enchance"] = get_Enchance;
            mField["ShowPath"] = get_ShowPath;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class CombatTextRecord :IRecord
    {
        public static string __TableName = "CombatText.txt";
        public int Id { get; private set; }
        public string Desc { get; private set; }
        public string FontAdd { get; private set; }
        public int FontType { get; private set; }
        public string FontPath { get; private set; }
        public int StartX { get; private set; }
        public int StartY { get; private set; }
        public int MinY { get; private set; }
        public int MaxY { get; private set; }
        public int Shadow { get; private set; }
        public int Color1 { get; private set; }
        public int Color2 { get; private set; }
        public int FontSize1 { get; private set; }
        public int SpeedX1 { get; private set; }
        public int SpeedY1 { get; private set; }
        public int AddSpeedX1 { get; private set; }
        public int AddSpeedY1 { get; private set; }
        public int MoveTime1 { get; private set; }
        public int EndFontSize1 { get; private set; }
        public int StopTime1 { get; private set; }
        public int SpeedX2 { get; private set; }
        public int SpeedY2 { get; private set; }
        public int AddSpeedX2 { get; private set; }
        public int AddSpeedY2 { get; private set; }
        public int MoveTime2 { get; private set; }
        public int EndFontSize2 { get; private set; }
        public int IntervalTime { get; private set; }
        public int QueueLimit { get; private set; }
        public int Group { get; private set; }
        public int IsShowShade { get; private set; }
        public int TextLayer { get; private set; }
        public int DelayTime { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                FontAdd = temp[__column++];
                FontType = Table_Tamplet.Convert_Int(temp[__column++]);
                FontPath = temp[__column++];
                StartX = Table_Tamplet.Convert_Int(temp[__column++]);
                StartY = Table_Tamplet.Convert_Int(temp[__column++]);
                MinY = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxY = Table_Tamplet.Convert_Int(temp[__column++]);
                Shadow = Table_Tamplet.Convert_Int(temp[__column++]);
                Color1 = Table_Tamplet.Convert_Int(temp[__column++]);
                Color2 = Table_Tamplet.Convert_Int(temp[__column++]);
                FontSize1 = Table_Tamplet.Convert_Int(temp[__column++]);
                SpeedX1 = Table_Tamplet.Convert_Int(temp[__column++]);
                SpeedY1 = Table_Tamplet.Convert_Int(temp[__column++]);
                AddSpeedX1 = Table_Tamplet.Convert_Int(temp[__column++]);
                AddSpeedY1 = Table_Tamplet.Convert_Int(temp[__column++]);
                MoveTime1 = Table_Tamplet.Convert_Int(temp[__column++]);
                EndFontSize1 = Table_Tamplet.Convert_Int(temp[__column++]);
                StopTime1 = Table_Tamplet.Convert_Int(temp[__column++]);
                SpeedX2 = Table_Tamplet.Convert_Int(temp[__column++]);
                SpeedY2 = Table_Tamplet.Convert_Int(temp[__column++]);
                AddSpeedX2 = Table_Tamplet.Convert_Int(temp[__column++]);
                AddSpeedY2 = Table_Tamplet.Convert_Int(temp[__column++]);
                MoveTime2 = Table_Tamplet.Convert_Int(temp[__column++]);
                EndFontSize2 = Table_Tamplet.Convert_Int(temp[__column++]);
                IntervalTime = Table_Tamplet.Convert_Int(temp[__column++]);
                QueueLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                Group = Table_Tamplet.Convert_Int(temp[__column++]);
                IsShowShade = Table_Tamplet.Convert_Int(temp[__column++]);
                TextLayer = Table_Tamplet.Convert_Int(temp[__column++]);
                DelayTime = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((CombatTextRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((CombatTextRecord)_this).Desc;
        }
        static object get_FontAdd(IRecord _this)
        {
            return ((CombatTextRecord)_this).FontAdd;
        }
        static object get_FontType(IRecord _this)
        {
            return ((CombatTextRecord)_this).FontType;
        }
        static object get_FontPath(IRecord _this)
        {
            return ((CombatTextRecord)_this).FontPath;
        }
        static object get_StartX(IRecord _this)
        {
            return ((CombatTextRecord)_this).StartX;
        }
        static object get_StartY(IRecord _this)
        {
            return ((CombatTextRecord)_this).StartY;
        }
        static object get_MinY(IRecord _this)
        {
            return ((CombatTextRecord)_this).MinY;
        }
        static object get_MaxY(IRecord _this)
        {
            return ((CombatTextRecord)_this).MaxY;
        }
        static object get_Shadow(IRecord _this)
        {
            return ((CombatTextRecord)_this).Shadow;
        }
        static object get_Color1(IRecord _this)
        {
            return ((CombatTextRecord)_this).Color1;
        }
        static object get_Color2(IRecord _this)
        {
            return ((CombatTextRecord)_this).Color2;
        }
        static object get_FontSize1(IRecord _this)
        {
            return ((CombatTextRecord)_this).FontSize1;
        }
        static object get_SpeedX1(IRecord _this)
        {
            return ((CombatTextRecord)_this).SpeedX1;
        }
        static object get_SpeedY1(IRecord _this)
        {
            return ((CombatTextRecord)_this).SpeedY1;
        }
        static object get_AddSpeedX1(IRecord _this)
        {
            return ((CombatTextRecord)_this).AddSpeedX1;
        }
        static object get_AddSpeedY1(IRecord _this)
        {
            return ((CombatTextRecord)_this).AddSpeedY1;
        }
        static object get_MoveTime1(IRecord _this)
        {
            return ((CombatTextRecord)_this).MoveTime1;
        }
        static object get_EndFontSize1(IRecord _this)
        {
            return ((CombatTextRecord)_this).EndFontSize1;
        }
        static object get_StopTime1(IRecord _this)
        {
            return ((CombatTextRecord)_this).StopTime1;
        }
        static object get_SpeedX2(IRecord _this)
        {
            return ((CombatTextRecord)_this).SpeedX2;
        }
        static object get_SpeedY2(IRecord _this)
        {
            return ((CombatTextRecord)_this).SpeedY2;
        }
        static object get_AddSpeedX2(IRecord _this)
        {
            return ((CombatTextRecord)_this).AddSpeedX2;
        }
        static object get_AddSpeedY2(IRecord _this)
        {
            return ((CombatTextRecord)_this).AddSpeedY2;
        }
        static object get_MoveTime2(IRecord _this)
        {
            return ((CombatTextRecord)_this).MoveTime2;
        }
        static object get_EndFontSize2(IRecord _this)
        {
            return ((CombatTextRecord)_this).EndFontSize2;
        }
        static object get_IntervalTime(IRecord _this)
        {
            return ((CombatTextRecord)_this).IntervalTime;
        }
        static object get_QueueLimit(IRecord _this)
        {
            return ((CombatTextRecord)_this).QueueLimit;
        }
        static object get_Group(IRecord _this)
        {
            return ((CombatTextRecord)_this).Group;
        }
        static object get_IsShowShade(IRecord _this)
        {
            return ((CombatTextRecord)_this).IsShowShade;
        }
        static object get_TextLayer(IRecord _this)
        {
            return ((CombatTextRecord)_this).TextLayer;
        }
        static object get_DelayTime(IRecord _this)
        {
            return ((CombatTextRecord)_this).DelayTime;
        }
        static CombatTextRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
            mField["FontAdd"] = get_FontAdd;
            mField["FontType"] = get_FontType;
            mField["FontPath"] = get_FontPath;
            mField["StartX"] = get_StartX;
            mField["StartY"] = get_StartY;
            mField["MinY"] = get_MinY;
            mField["MaxY"] = get_MaxY;
            mField["Shadow"] = get_Shadow;
            mField["Color1"] = get_Color1;
            mField["Color2"] = get_Color2;
            mField["FontSize1"] = get_FontSize1;
            mField["SpeedX1"] = get_SpeedX1;
            mField["SpeedY1"] = get_SpeedY1;
            mField["AddSpeedX1"] = get_AddSpeedX1;
            mField["AddSpeedY1"] = get_AddSpeedY1;
            mField["MoveTime1"] = get_MoveTime1;
            mField["EndFontSize1"] = get_EndFontSize1;
            mField["StopTime1"] = get_StopTime1;
            mField["SpeedX2"] = get_SpeedX2;
            mField["SpeedY2"] = get_SpeedY2;
            mField["AddSpeedX2"] = get_AddSpeedX2;
            mField["AddSpeedY2"] = get_AddSpeedY2;
            mField["MoveTime2"] = get_MoveTime2;
            mField["EndFontSize2"] = get_EndFontSize2;
            mField["IntervalTime"] = get_IntervalTime;
            mField["QueueLimit"] = get_QueueLimit;
            mField["Group"] = get_Group;
            mField["IsShowShade"] = get_IsShowShade;
            mField["TextLayer"] = get_TextLayer;
            mField["DelayTime"] = get_DelayTime;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class RandNameRecord :IRecord
    {
        public static string __TableName = "RandName.txt";
        public int Id { get; private set; }
        public string BoySurname { get; private set; }
        public string GirlSurname { get; private set; }
        public string Man { get; private set; }
        public string Woman { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                BoySurname = temp[__column++];
                GirlSurname = temp[__column++];
                Man = temp[__column++];
                Woman = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((RandNameRecord)_this).Id;
        }
        static object get_BoySurname(IRecord _this)
        {
            return ((RandNameRecord)_this).BoySurname;
        }
        static object get_GirlSurname(IRecord _this)
        {
            return ((RandNameRecord)_this).GirlSurname;
        }
        static object get_Man(IRecord _this)
        {
            return ((RandNameRecord)_this).Man;
        }
        static object get_Woman(IRecord _this)
        {
            return ((RandNameRecord)_this).Woman;
        }
        static RandNameRecord()
        {
            mField["Id"] = get_Id;
            mField["BoySurname"] = get_BoySurname;
            mField["GirlSurname"] = get_GirlSurname;
            mField["Man"] = get_Man;
            mField["Woman"] = get_Woman;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class OperationListRecord :IRecord
    {
        public static string __TableName = "OperationList.txt";
        public int Id { get; private set; }
        public int Speek { get; private set; }
        public int Attribute { get; private set; }
        public int AddFriend { get; private set; }
        public int AddEnemy { get; private set; }
        public int AddShield { get; private set; }
        public int DelFriend { get; private set; }
        public int DelEnemy { get; private set; }
        public int DelShield { get; private set; }
        public int InviteTeam { get; private set; }
        public int ApplyTeam { get; private set; }
        public int UpLeader { get; private set; }
        public int KickTeam { get; private set; }
        public int LeaveTeam { get; private set; }
        public int JoinUnion { get; private set; }
        public int UpChief { get; private set; }
        public int UpAccess { get; private set; }
        public int DownAccess { get; private set; }
        public int QuitUnion { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Speek = Table_Tamplet.Convert_Int(temp[__column++]);
                Attribute = Table_Tamplet.Convert_Int(temp[__column++]);
                AddFriend = Table_Tamplet.Convert_Int(temp[__column++]);
                AddEnemy = Table_Tamplet.Convert_Int(temp[__column++]);
                AddShield = Table_Tamplet.Convert_Int(temp[__column++]);
                DelFriend = Table_Tamplet.Convert_Int(temp[__column++]);
                DelEnemy = Table_Tamplet.Convert_Int(temp[__column++]);
                DelShield = Table_Tamplet.Convert_Int(temp[__column++]);
                InviteTeam = Table_Tamplet.Convert_Int(temp[__column++]);
                ApplyTeam = Table_Tamplet.Convert_Int(temp[__column++]);
                UpLeader = Table_Tamplet.Convert_Int(temp[__column++]);
                KickTeam = Table_Tamplet.Convert_Int(temp[__column++]);
                LeaveTeam = Table_Tamplet.Convert_Int(temp[__column++]);
                JoinUnion = Table_Tamplet.Convert_Int(temp[__column++]);
                UpChief = Table_Tamplet.Convert_Int(temp[__column++]);
                UpAccess = Table_Tamplet.Convert_Int(temp[__column++]);
                DownAccess = Table_Tamplet.Convert_Int(temp[__column++]);
                QuitUnion = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((OperationListRecord)_this).Id;
        }
        static object get_Speek(IRecord _this)
        {
            return ((OperationListRecord)_this).Speek;
        }
        static object get_Attribute(IRecord _this)
        {
            return ((OperationListRecord)_this).Attribute;
        }
        static object get_AddFriend(IRecord _this)
        {
            return ((OperationListRecord)_this).AddFriend;
        }
        static object get_AddEnemy(IRecord _this)
        {
            return ((OperationListRecord)_this).AddEnemy;
        }
        static object get_AddShield(IRecord _this)
        {
            return ((OperationListRecord)_this).AddShield;
        }
        static object get_DelFriend(IRecord _this)
        {
            return ((OperationListRecord)_this).DelFriend;
        }
        static object get_DelEnemy(IRecord _this)
        {
            return ((OperationListRecord)_this).DelEnemy;
        }
        static object get_DelShield(IRecord _this)
        {
            return ((OperationListRecord)_this).DelShield;
        }
        static object get_InviteTeam(IRecord _this)
        {
            return ((OperationListRecord)_this).InviteTeam;
        }
        static object get_ApplyTeam(IRecord _this)
        {
            return ((OperationListRecord)_this).ApplyTeam;
        }
        static object get_UpLeader(IRecord _this)
        {
            return ((OperationListRecord)_this).UpLeader;
        }
        static object get_KickTeam(IRecord _this)
        {
            return ((OperationListRecord)_this).KickTeam;
        }
        static object get_LeaveTeam(IRecord _this)
        {
            return ((OperationListRecord)_this).LeaveTeam;
        }
        static object get_JoinUnion(IRecord _this)
        {
            return ((OperationListRecord)_this).JoinUnion;
        }
        static object get_UpChief(IRecord _this)
        {
            return ((OperationListRecord)_this).UpChief;
        }
        static object get_UpAccess(IRecord _this)
        {
            return ((OperationListRecord)_this).UpAccess;
        }
        static object get_DownAccess(IRecord _this)
        {
            return ((OperationListRecord)_this).DownAccess;
        }
        static object get_QuitUnion(IRecord _this)
        {
            return ((OperationListRecord)_this).QuitUnion;
        }
        static OperationListRecord()
        {
            mField["Id"] = get_Id;
            mField["Speek"] = get_Speek;
            mField["Attribute"] = get_Attribute;
            mField["AddFriend"] = get_AddFriend;
            mField["AddEnemy"] = get_AddEnemy;
            mField["AddShield"] = get_AddShield;
            mField["DelFriend"] = get_DelFriend;
            mField["DelEnemy"] = get_DelEnemy;
            mField["DelShield"] = get_DelShield;
            mField["InviteTeam"] = get_InviteTeam;
            mField["ApplyTeam"] = get_ApplyTeam;
            mField["UpLeader"] = get_UpLeader;
            mField["KickTeam"] = get_KickTeam;
            mField["LeaveTeam"] = get_LeaveTeam;
            mField["JoinUnion"] = get_JoinUnion;
            mField["UpChief"] = get_UpChief;
            mField["UpAccess"] = get_UpAccess;
            mField["DownAccess"] = get_DownAccess;
            mField["QuitUnion"] = get_QuitUnion;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class UIRecord :IRecord
    {
        public static string __TableName = "UI.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int GroupId { get; private set; }
        public List<int> HuchiGroupId = new List<int>();
        public int IsStack { get; private set; }
        public int DadId { get; private set; }
        public float posX { get;        set; }
        public float posY { get;        set; }
        public float posZ { get;        set; }
        public int CleanRes { get; private set; }
        public int Swallow { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Path = temp[__column++];
                GroupId = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(HuchiGroupId,temp[__column++]);
                IsStack = Table_Tamplet.Convert_Int(temp[__column++]);
                DadId = Table_Tamplet.Convert_Int(temp[__column++]);
                posX = Table_Tamplet.Convert_Float(temp[__column++]);
                posY = Table_Tamplet.Convert_Float(temp[__column++]);
                posZ = Table_Tamplet.Convert_Float(temp[__column++]);
                CleanRes = Table_Tamplet.Convert_Int(temp[__column++]);
                Swallow = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((UIRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((UIRecord)_this).Name;
        }
        static object get_Path(IRecord _this)
        {
            return ((UIRecord)_this).Path;
        }
        static object get_GroupId(IRecord _this)
        {
            return ((UIRecord)_this).GroupId;
        }
        static object get_HuchiGroupId(IRecord _this)
        {
            return ((UIRecord)_this).HuchiGroupId;
        }
        static object get_IsStack(IRecord _this)
        {
            return ((UIRecord)_this).IsStack;
        }
        static object get_DadId(IRecord _this)
        {
            return ((UIRecord)_this).DadId;
        }
        static object get_posX(IRecord _this)
        {
            return ((UIRecord)_this).posX;
        }
        static object get_posY(IRecord _this)
        {
            return ((UIRecord)_this).posY;
        }
        static object get_posZ(IRecord _this)
        {
            return ((UIRecord)_this).posZ;
        }
        static object get_CleanRes(IRecord _this)
        {
            return ((UIRecord)_this).CleanRes;
        }
        static object get_Swallow(IRecord _this)
        {
            return ((UIRecord)_this).Swallow;
        }
        static UIRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Path"] = get_Path;
            mField["GroupId"] = get_GroupId;
            mField["HuchiGroupId"] = get_HuchiGroupId;
            mField["IsStack"] = get_IsStack;
            mField["DadId"] = get_DadId;
            mField["posX"] = get_posX;
            mField["posY"] = get_posY;
            mField["posZ"] = get_posZ;
            mField["CleanRes"] = get_CleanRes;
            mField["Swallow"] = get_Swallow;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GiftRecord :IRecord
    {
        public static string __TableName = "Gift.txt";
        public int Id { get; private set; }
        public int Type { get; private set; }
        public int Flag { get; private set; }
        public int Exdata { get; private set; }
        [TableBinding("ItemBase")]
        [ListSize(12)]
        public ReadonlyList<int> Param { get; private set; } 
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Flag = Table_Tamplet.Convert_Int(temp[__column++]);
                Exdata = Table_Tamplet.Convert_Int(temp[__column++]);
                Param=new ReadonlyList<int>(12);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GiftRecord)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((GiftRecord)_this).Type;
        }
        static object get_Flag(IRecord _this)
        {
            return ((GiftRecord)_this).Flag;
        }
        static object get_Exdata(IRecord _this)
        {
            return ((GiftRecord)_this).Exdata;
        }
        static object get_Param(IRecord _this)
        {
            return ((GiftRecord)_this).Param;
        }
        static GiftRecord()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["Flag"] = get_Flag;
            mField["Exdata"] = get_Exdata;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipBlessingRecord :IRecord
    {
        public static string __TableName = "EquipBlessing.txt";
        public int Id { get; private set; }
        public int Probability { get; private set; }
        [TableBinding("ItemBase")]
        [ListSize(3)]
        public ReadonlyList<int> NeedItemId { get; private set; } 
        [ListSize(3)]
        public ReadonlyList<int> NeedItemCount { get; private set; } 
        public int NeedMoney { get; private set; }
        [TableBinding("ItemBase")]
        public int WarrantItemId { get; private set; }
        public int WarrantItemCount { get; private set; }
        public int FalseLevel { get; private set; }
        public int SpecialId { get; private set; }
        public int SmritiMoney { get; private set; }
        public int SmritiGold { get; private set; }
        public int[] CallBackItem = new int[3];
        public int[] CallBackCount = new int[3];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Probability = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId=new ReadonlyList<int>(3);
                NeedItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount=new ReadonlyList<int>(3);
                NeedItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                WarrantItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                WarrantItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                FalseLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                SpecialId = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiGold = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackItem[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackItem[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackItem[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).Id;
        }
        static object get_Probability(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).Probability;
        }
        static object get_NeedItemId(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).NeedItemId;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).NeedItemCount;
        }
        static object get_NeedMoney(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).NeedMoney;
        }
        static object get_WarrantItemId(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).WarrantItemId;
        }
        static object get_WarrantItemCount(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).WarrantItemCount;
        }
        static object get_FalseLevel(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).FalseLevel;
        }
        static object get_SpecialId(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).SpecialId;
        }
        static object get_SmritiMoney(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).SmritiMoney;
        }
        static object get_SmritiGold(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).SmritiGold;
        }
        static object get_CallBackItem(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).CallBackItem;
        }
        static object get_CallBackCount(IRecord _this)
        {
            return ((EquipBlessingRecord)_this).CallBackCount;
        }
        static EquipBlessingRecord()
        {
            mField["Id"] = get_Id;
            mField["Probability"] = get_Probability;
            mField["NeedItemId"] = get_NeedItemId;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["NeedMoney"] = get_NeedMoney;
            mField["WarrantItemId"] = get_WarrantItemId;
            mField["WarrantItemCount"] = get_WarrantItemCount;
            mField["FalseLevel"] = get_FalseLevel;
            mField["SpecialId"] = get_SpecialId;
            mField["SmritiMoney"] = get_SmritiMoney;
            mField["SmritiGold"] = get_SmritiGold;
            mField["CallBackItem"] = get_CallBackItem;
            mField["CallBackCount"] = get_CallBackCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipAdditionalRecord :IRecord
    {
        public static string __TableName = "EquipAdditional.txt";
        public int Id { get; private set; }
        public int HpMax { get; private set; }
        public int Power { get; private set; }
        [TableBinding("ItemBase")]
        public int NeedItemId { get; private set; }
        public int NeedItemCount { get; private set; }
        public int NeedMoney { get; private set; }
        public int SmritiMoney { get; private set; }
        public int SmritiGold { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                HpMax = Table_Tamplet.Convert_Int(temp[__column++]);
                Power = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiGold = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).Id;
        }
        static object get_HpMax(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).HpMax;
        }
        static object get_Power(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).Power;
        }
        static object get_NeedItemId(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).NeedItemId;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).NeedItemCount;
        }
        static object get_NeedMoney(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).NeedMoney;
        }
        static object get_SmritiMoney(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).SmritiMoney;
        }
        static object get_SmritiGold(IRecord _this)
        {
            return ((EquipAdditionalRecord)_this).SmritiGold;
        }
        static EquipAdditionalRecord()
        {
            mField["Id"] = get_Id;
            mField["HpMax"] = get_HpMax;
            mField["Power"] = get_Power;
            mField["NeedItemId"] = get_NeedItemId;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["NeedMoney"] = get_NeedMoney;
            mField["SmritiMoney"] = get_SmritiMoney;
            mField["SmritiGold"] = get_SmritiGold;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipExcellentRecord :IRecord
    {
        public static string __TableName = "EquipExcellent.txt";
        public int Id { get; private set; }
        [TableBinding("ItemBase")]
        public int GreenItemId { get; private set; }
        public int GreenItemCount { get; private set; }
        public int GreenMoney { get; private set; }
        [TableBinding("ItemBase")]
        public int ItemId { get; private set; }
        public int ItemCount { get; private set; }
        [TableBinding("ItemBase")]
        public int LockId { get; private set; }
        public int[] LockCount = new int[5];
        public int[] Money = new int[6];
        public int SmritiMoney { get; private set; }
        public int SmritiGold { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                GreenItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                GreenItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                GreenMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                LockId = Table_Tamplet.Convert_Int(temp[__column++]);
                LockCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                LockCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                LockCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                LockCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                LockCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Money[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Money[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Money[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Money[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Money[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Money[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiGold = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).Id;
        }
        static object get_GreenItemId(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).GreenItemId;
        }
        static object get_GreenItemCount(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).GreenItemCount;
        }
        static object get_GreenMoney(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).GreenMoney;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).ItemId;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).ItemCount;
        }
        static object get_LockId(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).LockId;
        }
        static object get_LockCount(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).LockCount;
        }
        static object get_Money(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).Money;
        }
        static object get_SmritiMoney(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).SmritiMoney;
        }
        static object get_SmritiGold(IRecord _this)
        {
            return ((EquipExcellentRecord)_this).SmritiGold;
        }
        static EquipExcellentRecord()
        {
            mField["Id"] = get_Id;
            mField["GreenItemId"] = get_GreenItemId;
            mField["GreenItemCount"] = get_GreenItemCount;
            mField["GreenMoney"] = get_GreenMoney;
            mField["ItemId"] = get_ItemId;
            mField["ItemCount"] = get_ItemCount;
            mField["LockId"] = get_LockId;
            mField["LockCount"] = get_LockCount;
            mField["Money"] = get_Money;
            mField["SmritiMoney"] = get_SmritiMoney;
            mField["SmritiGold"] = get_SmritiGold;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipModelViewRecord :IRecord
    {
        public static string __TableName = "EquipModelView.txt";
        public int Id { get; private set; }
        public int SepcularRed { get; private set; }
        public int SepcularGreen { get; private set; }
        public int SepcularBlue { get; private set; }
        public int SepcularAlpha { get; private set; }
        public int FlowRed { get; private set; }
        public int FlowGreen { get; private set; }
        public int FlowBlue { get; private set; }
        public int FlowAlpha { get; private set; }
        public string[] EffectPath = new string[3];
        public int[] EffectMount = new int[3];
        public float[] EffectPosX = new float[3];
        public float[] EffectPosY = new float[3];
        public float[] EffectPosZ = new float[3];
        public float[] EffectDirX = new float[3];
        public float[] EffectDirY = new float[3];
        public float[] EffectDirZ = new float[3];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                SepcularRed = Table_Tamplet.Convert_Int(temp[__column++]);
                SepcularGreen = Table_Tamplet.Convert_Int(temp[__column++]);
                SepcularBlue = Table_Tamplet.Convert_Int(temp[__column++]);
                SepcularAlpha = Table_Tamplet.Convert_Int(temp[__column++]);
                FlowRed = Table_Tamplet.Convert_Int(temp[__column++]);
                FlowGreen = Table_Tamplet.Convert_Int(temp[__column++]);
                FlowBlue = Table_Tamplet.Convert_Int(temp[__column++]);
                FlowAlpha = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectPath[0]  = temp[__column++];
                EffectMount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectPosX[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPosY[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPosZ[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirX[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirY[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirZ[0] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPath[1]  = temp[__column++];
                EffectMount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectPosX[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPosY[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPosZ[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirX[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirY[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirZ[1] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPath[2]  = temp[__column++];
                EffectMount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectPosX[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPosY[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectPosZ[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirX[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirY[2] = Table_Tamplet.Convert_Float(temp[__column++]);
                EffectDirZ[2] = Table_Tamplet.Convert_Float(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).Id;
        }
        static object get_SepcularRed(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).SepcularRed;
        }
        static object get_SepcularGreen(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).SepcularGreen;
        }
        static object get_SepcularBlue(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).SepcularBlue;
        }
        static object get_SepcularAlpha(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).SepcularAlpha;
        }
        static object get_FlowRed(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).FlowRed;
        }
        static object get_FlowGreen(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).FlowGreen;
        }
        static object get_FlowBlue(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).FlowBlue;
        }
        static object get_FlowAlpha(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).FlowAlpha;
        }
        static object get_EffectPath(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectPath;
        }
        static object get_EffectMount(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectMount;
        }
        static object get_EffectPosX(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectPosX;
        }
        static object get_EffectPosY(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectPosY;
        }
        static object get_EffectPosZ(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectPosZ;
        }
        static object get_EffectDirX(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectDirX;
        }
        static object get_EffectDirY(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectDirY;
        }
        static object get_EffectDirZ(IRecord _this)
        {
            return ((EquipModelViewRecord)_this).EffectDirZ;
        }
        static EquipModelViewRecord()
        {
            mField["Id"] = get_Id;
            mField["SepcularRed"] = get_SepcularRed;
            mField["SepcularGreen"] = get_SepcularGreen;
            mField["SepcularBlue"] = get_SepcularBlue;
            mField["SepcularAlpha"] = get_SepcularAlpha;
            mField["FlowRed"] = get_FlowRed;
            mField["FlowGreen"] = get_FlowGreen;
            mField["FlowBlue"] = get_FlowBlue;
            mField["FlowAlpha"] = get_FlowAlpha;
            mField["EffectPath"] = get_EffectPath;
            mField["EffectMount"] = get_EffectMount;
            mField["EffectPosX"] = get_EffectPosX;
            mField["EffectPosY"] = get_EffectPosY;
            mField["EffectPosZ"] = get_EffectPosZ;
            mField["EffectDirX"] = get_EffectDirX;
            mField["EffectDirY"] = get_EffectDirY;
            mField["EffectDirZ"] = get_EffectDirZ;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TalkRecord :IRecord
    {
        public static string __TableName = "Talk.txt";
        public int Id { get; private set; }
        public int Model { get; private set; }
        public int Action { get; private set; }
        public int Sound { get; private set; }
        public int Content { get; private set; }
        public int NextTalk { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Model = Table_Tamplet.Convert_Int(temp[__column++]);
                Action = Table_Tamplet.Convert_Int(temp[__column++]);
                Sound = Table_Tamplet.Convert_Int(temp[__column++]);
                Content = Table_Tamplet.Convert_Int(temp[__column++]);
                NextTalk = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TalkRecord)_this).Id;
        }
        static object get_Model(IRecord _this)
        {
            return ((TalkRecord)_this).Model;
        }
        static object get_Action(IRecord _this)
        {
            return ((TalkRecord)_this).Action;
        }
        static object get_Sound(IRecord _this)
        {
            return ((TalkRecord)_this).Sound;
        }
        static object get_Content(IRecord _this)
        {
            return ((TalkRecord)_this).Content;
        }
        static object get_NextTalk(IRecord _this)
        {
            return ((TalkRecord)_this).NextTalk;
        }
        static TalkRecord()
        {
            mField["Id"] = get_Id;
            mField["Model"] = get_Model;
            mField["Action"] = get_Action;
            mField["Sound"] = get_Sound;
            mField["Content"] = get_Content;
            mField["NextTalk"] = get_NextTalk;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ChatInfoRecord :IRecord
    {
        public static string __TableName = "ChatInfo.txt";
        public int Id { get; private set; }
        public string Desc { get; private set; }
        public int[] Channal = new int[9];
        public int MaxWord { get; private set; }
        public int CD { get; private set; }
        public int NeedLevel { get; private set; }
        public int[] ColorId = new int[2];
        public int Stroke { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                Channal[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Channal[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxWord = Table_Tamplet.Convert_Int(temp[__column++]);
                CD = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                ColorId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ColorId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Stroke = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ChatInfoRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((ChatInfoRecord)_this).Desc;
        }
        static object get_Channal(IRecord _this)
        {
            return ((ChatInfoRecord)_this).Channal;
        }
        static object get_MaxWord(IRecord _this)
        {
            return ((ChatInfoRecord)_this).MaxWord;
        }
        static object get_CD(IRecord _this)
        {
            return ((ChatInfoRecord)_this).CD;
        }
        static object get_NeedLevel(IRecord _this)
        {
            return ((ChatInfoRecord)_this).NeedLevel;
        }
        static object get_ColorId(IRecord _this)
        {
            return ((ChatInfoRecord)_this).ColorId;
        }
        static object get_Stroke(IRecord _this)
        {
            return ((ChatInfoRecord)_this).Stroke;
        }
        static ChatInfoRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
            mField["Channal"] = get_Channal;
            mField["MaxWord"] = get_MaxWord;
            mField["CD"] = get_CD;
            mField["NeedLevel"] = get_NeedLevel;
            mField["ColorId"] = get_ColorId;
            mField["Stroke"] = get_Stroke;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class HandBookRecord :IRecord
    {
        public static string __TableName = "HandBook.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int PieceId { get; private set; }
        public int Count { get; private set; }
        public int AttrId { get; private set; }
        public int AttrValue { get; private set; }
        public int RewardGroupId { get; private set; }
        public int RewardGroupIndex { get; private set; }
        public int Money { get; private set; }
        public int NpcId { get; private set; }
        public int TrackType { get; private set; }
        public string TrackString { get; private set; }
        public int[] TrackParam = new int[3];
        public int ListSort { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                PieceId = Table_Tamplet.Convert_Int(temp[__column++]);
                Count = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardGroupId = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardGroupIndex = Table_Tamplet.Convert_Int(temp[__column++]);
                Money = Table_Tamplet.Convert_Int(temp[__column++]);
                NpcId = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackType = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackString = temp[__column++];
                TrackParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                TrackParam[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ListSort = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((HandBookRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((HandBookRecord)_this).Name;
        }
        static object get_Icon(IRecord _this)
        {
            return ((HandBookRecord)_this).Icon;
        }
        static object get_PieceId(IRecord _this)
        {
            return ((HandBookRecord)_this).PieceId;
        }
        static object get_Count(IRecord _this)
        {
            return ((HandBookRecord)_this).Count;
        }
        static object get_AttrId(IRecord _this)
        {
            return ((HandBookRecord)_this).AttrId;
        }
        static object get_AttrValue(IRecord _this)
        {
            return ((HandBookRecord)_this).AttrValue;
        }
        static object get_RewardGroupId(IRecord _this)
        {
            return ((HandBookRecord)_this).RewardGroupId;
        }
        static object get_RewardGroupIndex(IRecord _this)
        {
            return ((HandBookRecord)_this).RewardGroupIndex;
        }
        static object get_Money(IRecord _this)
        {
            return ((HandBookRecord)_this).Money;
        }
        static object get_NpcId(IRecord _this)
        {
            return ((HandBookRecord)_this).NpcId;
        }
        static object get_TrackType(IRecord _this)
        {
            return ((HandBookRecord)_this).TrackType;
        }
        static object get_TrackString(IRecord _this)
        {
            return ((HandBookRecord)_this).TrackString;
        }
        static object get_TrackParam(IRecord _this)
        {
            return ((HandBookRecord)_this).TrackParam;
        }
        static object get_ListSort(IRecord _this)
        {
            return ((HandBookRecord)_this).ListSort;
        }
        static HandBookRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Icon"] = get_Icon;
            mField["PieceId"] = get_PieceId;
            mField["Count"] = get_Count;
            mField["AttrId"] = get_AttrId;
            mField["AttrValue"] = get_AttrValue;
            mField["RewardGroupId"] = get_RewardGroupId;
            mField["RewardGroupIndex"] = get_RewardGroupIndex;
            mField["Money"] = get_Money;
            mField["NpcId"] = get_NpcId;
            mField["TrackType"] = get_TrackType;
            mField["TrackString"] = get_TrackString;
            mField["TrackParam"] = get_TrackParam;
            mField["ListSort"] = get_ListSort;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BookGroupRecord :IRecord
    {
        public static string __TableName = "BookGroup.txt";
        public int Id { get; private set; }
        public string Desc { get; private set; }
        public int[] ItemId = new int[6];
        public int[] AttrId = new int[6];
        public int[] AttrValue = new int[6];
        public int[] GroupAttrId = new int[4];
        public int[] GroupAttrValue = new int[4];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                ItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupAttrValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BookGroupRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((BookGroupRecord)_this).Desc;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((BookGroupRecord)_this).ItemId;
        }
        static object get_AttrId(IRecord _this)
        {
            return ((BookGroupRecord)_this).AttrId;
        }
        static object get_AttrValue(IRecord _this)
        {
            return ((BookGroupRecord)_this).AttrValue;
        }
        static object get_GroupAttrId(IRecord _this)
        {
            return ((BookGroupRecord)_this).GroupAttrId;
        }
        static object get_GroupAttrValue(IRecord _this)
        {
            return ((BookGroupRecord)_this).GroupAttrValue;
        }
        static BookGroupRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
            mField["ItemId"] = get_ItemId;
            mField["AttrId"] = get_AttrId;
            mField["AttrValue"] = get_AttrValue;
            mField["GroupAttrId"] = get_GroupAttrId;
            mField["GroupAttrValue"] = get_GroupAttrValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ItemComposeRecord :IRecord
    {
        public static string __TableName = "ItemCompose.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public int ComposeView { get; private set; }
        [TableBinding("ItemBase")]
        [ListSize(4)]
        public ReadonlyList<int> NeedId { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> NeedCount { get; private set; } 
        public int NeedRes { get; private set; }
        public int NeedValue { get; private set; }
        public int Pro { get; private set; }
        public int SortByCareer { get; private set; }
        public int ComposeOpenLevel { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                ComposeView = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedId=new ReadonlyList<int>(4);
                NeedId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedCount=new ReadonlyList<int>(4);
                NeedCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedRes = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedValue = Table_Tamplet.Convert_Int(temp[__column++]);
                Pro = Table_Tamplet.Convert_Int(temp[__column++]);
                SortByCareer = Table_Tamplet.Convert_Int(temp[__column++]);
                ComposeOpenLevel = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ItemComposeRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ItemComposeRecord)_this).Name;
        }
        static object get_Type(IRecord _this)
        {
            return ((ItemComposeRecord)_this).Type;
        }
        static object get_ComposeView(IRecord _this)
        {
            return ((ItemComposeRecord)_this).ComposeView;
        }
        static object get_NeedId(IRecord _this)
        {
            return ((ItemComposeRecord)_this).NeedId;
        }
        static object get_NeedCount(IRecord _this)
        {
            return ((ItemComposeRecord)_this).NeedCount;
        }
        static object get_NeedRes(IRecord _this)
        {
            return ((ItemComposeRecord)_this).NeedRes;
        }
        static object get_NeedValue(IRecord _this)
        {
            return ((ItemComposeRecord)_this).NeedValue;
        }
        static object get_Pro(IRecord _this)
        {
            return ((ItemComposeRecord)_this).Pro;
        }
        static object get_SortByCareer(IRecord _this)
        {
            return ((ItemComposeRecord)_this).SortByCareer;
        }
        static object get_ComposeOpenLevel(IRecord _this)
        {
            return ((ItemComposeRecord)_this).ComposeOpenLevel;
        }
        static ItemComposeRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Type"] = get_Type;
            mField["ComposeView"] = get_ComposeView;
            mField["NeedId"] = get_NeedId;
            mField["NeedCount"] = get_NeedCount;
            mField["NeedRes"] = get_NeedRes;
            mField["NeedValue"] = get_NeedValue;
            mField["Pro"] = get_Pro;
            mField["SortByCareer"] = get_SortByCareer;
            mField["ComposeOpenLevel"] = get_ComposeOpenLevel;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class CampRecord :IRecord
    {
        public static string __TableName = "Camp.txt";
        public int Id { get; private set; }
        public int[] Camp = new int[10];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp[9] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((CampRecord)_this).Id;
        }
        static object get_Camp(IRecord _this)
        {
            return ((CampRecord)_this).Camp;
        }
        static CampRecord()
        {
            mField["Id"] = get_Id;
            mField["Camp"] = get_Camp;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DropModelRecord :IRecord
    {
        public static string __TableName = "DropModel.txt";
        public int Id { get; private set; }
        public string ModelPath { get; private set; }
        public int SoundId { get; private set; }
        public float Scale { get;        set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                ModelPath = temp[__column++];
                SoundId = Table_Tamplet.Convert_Int(temp[__column++]);
                Scale = Table_Tamplet.Convert_Float(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DropModelRecord)_this).Id;
        }
        static object get_ModelPath(IRecord _this)
        {
            return ((DropModelRecord)_this).ModelPath;
        }
        static object get_SoundId(IRecord _this)
        {
            return ((DropModelRecord)_this).SoundId;
        }
        static object get_Scale(IRecord _this)
        {
            return ((DropModelRecord)_this).Scale;
        }
        static DropModelRecord()
        {
            mField["Id"] = get_Id;
            mField["ModelPath"] = get_ModelPath;
            mField["SoundId"] = get_SoundId;
            mField["Scale"] = get_Scale;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class FubenRecord :IRecord
    {
        public static string __TableName = "Fuben.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int MainType { get; private set; }
        public int AssistType { get; private set; }
        public int Difficulty { get; private set; }
        public int BeforeStoryId { get; private set; }
        public int AfterStoryId { get; private set; }
        public int TodayCount { get; private set; }
        public int TodayBuyCount { get; private set; }
        [TableBinding("ItemBase")]
        [ListSize(2)]
        public ReadonlyList<int> NeedItemId { get; private set; } 
        [ListSize(2)]
        public ReadonlyList<int> NeedItemCount { get; private set; } 
        [TableBinding("ItemBase")]
        public int ResetItemId { get; private set; }
        public int ResetItemCount { get; private set; }
        public int ViewConditionId { get; private set; }
        public int EnterConditionId { get; private set; }
        public List<int> OpenTime = new List<int>();
        public int CanEnterTime { get; private set; }
        public int OpenLastMinutes { get; private set; }
        public float TimeLimitMinutes { get;        set; }
        public int SweepLimitMinutes { get; private set; }
        public int FightPoint { get; private set; }
        public string Desc { get; private set; }
        [TableBinding("ItemBase")]
        [ListSize(4)]
        public ReadonlyList<int> RewardId { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> RewardCount { get; private set; } 
        public int ScriptId { get; private set; }
        public int FlagId { get; private set; }
        public int TodayCountExdata { get; private set; }
        public int ResetExdata { get; private set; }
        public int TotleExdata { get; private set; }
        public int SceneId { get; private set; }
        public int TimeExdata { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        [TableBinding("Queue")]
        public int QueueParam { get; private set; }
        public int DrawReward { get; private set; }
        public int ScanExp { get; private set; }
        public int ScanGold { get; private set; }
        public int[] ScanReward = new int[2];
        public int FubenInfoParam { get; private set; }
        public int FubenLogicID { get; private set; }
        public int IsDynamicExp { get; private set; }
        public int DynamicExpRatio { get; private set; }
        public int ScanDynamicExpRatio { get; private set; }
        [ListSize(8)]
        public ReadonlyList<int> DisplayReward { get; private set; } 
        [ListSize(8)]
        public ReadonlyList<int> DisplayCount { get; private set; } 
        [TableBinding("BangBuff")]
        public int CanInspire { get; private set; }
        public int CanGroupEnter { get; private set; }
        public int IsPlaySlow { get; private set; }
        public int IsDyncReward { get; private set; }
        public int IsStarReward { get; private set; }
        public int StarRewardProb { get; private set; }
        public string MieshiScoreDesc { get; private set; }
        public string MieshiRewardDesc { get; private set; }
        [TableBinding("Icon")]
        public int FaceIcon { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                MainType = Table_Tamplet.Convert_Int(temp[__column++]);
                AssistType = Table_Tamplet.Convert_Int(temp[__column++]);
                Difficulty = Table_Tamplet.Convert_Int(temp[__column++]);
                BeforeStoryId = Table_Tamplet.Convert_Int(temp[__column++]);
                AfterStoryId = Table_Tamplet.Convert_Int(temp[__column++]);
                TodayCount = Table_Tamplet.Convert_Int(temp[__column++]);
                TodayBuyCount = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId=new ReadonlyList<int>(2);
                NeedItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount=new ReadonlyList<int>(2);
                NeedItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ResetItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                ResetItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                ViewConditionId = Table_Tamplet.Convert_Int(temp[__column++]);
                EnterConditionId = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(OpenTime,temp[__column++]);
                CanEnterTime = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenLastMinutes = Table_Tamplet.Convert_Int(temp[__column++]);
                TimeLimitMinutes = Table_Tamplet.Convert_Float(temp[__column++]);
                SweepLimitMinutes = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = Table_Tamplet.Convert_String(temp[__column++]);
                RewardId=new ReadonlyList<int>(4);
                RewardId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardCount=new ReadonlyList<int>(4);
                RewardCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ScriptId = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagId = Table_Tamplet.Convert_Int(temp[__column++]);
                TodayCountExdata = Table_Tamplet.Convert_Int(temp[__column++]);
                ResetExdata = Table_Tamplet.Convert_Int(temp[__column++]);
                TotleExdata = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneId = Table_Tamplet.Convert_Int(temp[__column++]);
                TimeExdata = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                QueueParam = Table_Tamplet.Convert_Int(temp[__column++]);
                DrawReward = Table_Tamplet.Convert_Int(temp[__column++]);
                ScanExp = Table_Tamplet.Convert_Int(temp[__column++]);
                ScanGold = Table_Tamplet.Convert_Int(temp[__column++]);
                ScanReward[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ScanReward[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenInfoParam = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenLogicID = Table_Tamplet.Convert_Int(temp[__column++]);
                IsDynamicExp = Table_Tamplet.Convert_Int(temp[__column++]);
                DynamicExpRatio = Table_Tamplet.Convert_Int(temp[__column++]);
                ScanDynamicExpRatio = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward=new ReadonlyList<int>(8);
                DisplayReward[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount=new ReadonlyList<int>(8);
                DisplayCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayReward[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                CanInspire = Table_Tamplet.Convert_Int(temp[__column++]);
                CanGroupEnter = Table_Tamplet.Convert_Int(temp[__column++]);
                IsPlaySlow = Table_Tamplet.Convert_Int(temp[__column++]);
                IsDyncReward = Table_Tamplet.Convert_Int(temp[__column++]);
                IsStarReward = Table_Tamplet.Convert_Int(temp[__column++]);
                StarRewardProb = Table_Tamplet.Convert_Int(temp[__column++]);
                MieshiScoreDesc = Table_Tamplet.Convert_String(temp[__column++]);
                MieshiRewardDesc = Table_Tamplet.Convert_String(temp[__column++]);
                FaceIcon = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((FubenRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((FubenRecord)_this).Name;
        }
        static object get_MainType(IRecord _this)
        {
            return ((FubenRecord)_this).MainType;
        }
        static object get_AssistType(IRecord _this)
        {
            return ((FubenRecord)_this).AssistType;
        }
        static object get_Difficulty(IRecord _this)
        {
            return ((FubenRecord)_this).Difficulty;
        }
        static object get_BeforeStoryId(IRecord _this)
        {
            return ((FubenRecord)_this).BeforeStoryId;
        }
        static object get_AfterStoryId(IRecord _this)
        {
            return ((FubenRecord)_this).AfterStoryId;
        }
        static object get_TodayCount(IRecord _this)
        {
            return ((FubenRecord)_this).TodayCount;
        }
        static object get_TodayBuyCount(IRecord _this)
        {
            return ((FubenRecord)_this).TodayBuyCount;
        }
        static object get_NeedItemId(IRecord _this)
        {
            return ((FubenRecord)_this).NeedItemId;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((FubenRecord)_this).NeedItemCount;
        }
        static object get_ResetItemId(IRecord _this)
        {
            return ((FubenRecord)_this).ResetItemId;
        }
        static object get_ResetItemCount(IRecord _this)
        {
            return ((FubenRecord)_this).ResetItemCount;
        }
        static object get_ViewConditionId(IRecord _this)
        {
            return ((FubenRecord)_this).ViewConditionId;
        }
        static object get_EnterConditionId(IRecord _this)
        {
            return ((FubenRecord)_this).EnterConditionId;
        }
        static object get_OpenTime(IRecord _this)
        {
            return ((FubenRecord)_this).OpenTime;
        }
        static object get_CanEnterTime(IRecord _this)
        {
            return ((FubenRecord)_this).CanEnterTime;
        }
        static object get_OpenLastMinutes(IRecord _this)
        {
            return ((FubenRecord)_this).OpenLastMinutes;
        }
        static object get_TimeLimitMinutes(IRecord _this)
        {
            return ((FubenRecord)_this).TimeLimitMinutes;
        }
        static object get_SweepLimitMinutes(IRecord _this)
        {
            return ((FubenRecord)_this).SweepLimitMinutes;
        }
        static object get_FightPoint(IRecord _this)
        {
            return ((FubenRecord)_this).FightPoint;
        }
        static object get_Desc(IRecord _this)
        {
            return ((FubenRecord)_this).Desc;
        }
        static object get_RewardId(IRecord _this)
        {
            return ((FubenRecord)_this).RewardId;
        }
        static object get_RewardCount(IRecord _this)
        {
            return ((FubenRecord)_this).RewardCount;
        }
        static object get_ScriptId(IRecord _this)
        {
            return ((FubenRecord)_this).ScriptId;
        }
        static object get_FlagId(IRecord _this)
        {
            return ((FubenRecord)_this).FlagId;
        }
        static object get_TodayCountExdata(IRecord _this)
        {
            return ((FubenRecord)_this).TodayCountExdata;
        }
        static object get_ResetExdata(IRecord _this)
        {
            return ((FubenRecord)_this).ResetExdata;
        }
        static object get_TotleExdata(IRecord _this)
        {
            return ((FubenRecord)_this).TotleExdata;
        }
        static object get_SceneId(IRecord _this)
        {
            return ((FubenRecord)_this).SceneId;
        }
        static object get_TimeExdata(IRecord _this)
        {
            return ((FubenRecord)_this).TimeExdata;
        }
        static object get_Icon(IRecord _this)
        {
            return ((FubenRecord)_this).Icon;
        }
        static object get_QueueParam(IRecord _this)
        {
            return ((FubenRecord)_this).QueueParam;
        }
        static object get_DrawReward(IRecord _this)
        {
            return ((FubenRecord)_this).DrawReward;
        }
        static object get_ScanExp(IRecord _this)
        {
            return ((FubenRecord)_this).ScanExp;
        }
        static object get_ScanGold(IRecord _this)
        {
            return ((FubenRecord)_this).ScanGold;
        }
        static object get_ScanReward(IRecord _this)
        {
            return ((FubenRecord)_this).ScanReward;
        }
        static object get_FubenInfoParam(IRecord _this)
        {
            return ((FubenRecord)_this).FubenInfoParam;
        }
        static object get_FubenLogicID(IRecord _this)
        {
            return ((FubenRecord)_this).FubenLogicID;
        }
        static object get_IsDynamicExp(IRecord _this)
        {
            return ((FubenRecord)_this).IsDynamicExp;
        }
        static object get_DynamicExpRatio(IRecord _this)
        {
            return ((FubenRecord)_this).DynamicExpRatio;
        }
        static object get_ScanDynamicExpRatio(IRecord _this)
        {
            return ((FubenRecord)_this).ScanDynamicExpRatio;
        }
        static object get_DisplayReward(IRecord _this)
        {
            return ((FubenRecord)_this).DisplayReward;
        }
        static object get_DisplayCount(IRecord _this)
        {
            return ((FubenRecord)_this).DisplayCount;
        }
        static object get_CanInspire(IRecord _this)
        {
            return ((FubenRecord)_this).CanInspire;
        }
        static object get_CanGroupEnter(IRecord _this)
        {
            return ((FubenRecord)_this).CanGroupEnter;
        }
        static object get_IsPlaySlow(IRecord _this)
        {
            return ((FubenRecord)_this).IsPlaySlow;
        }
        static object get_IsDyncReward(IRecord _this)
        {
            return ((FubenRecord)_this).IsDyncReward;
        }
        static object get_IsStarReward(IRecord _this)
        {
            return ((FubenRecord)_this).IsStarReward;
        }
        static object get_StarRewardProb(IRecord _this)
        {
            return ((FubenRecord)_this).StarRewardProb;
        }
        static object get_MieshiScoreDesc(IRecord _this)
        {
            return ((FubenRecord)_this).MieshiScoreDesc;
        }
        static object get_MieshiRewardDesc(IRecord _this)
        {
            return ((FubenRecord)_this).MieshiRewardDesc;
        }
        static object get_FaceIcon(IRecord _this)
        {
            return ((FubenRecord)_this).FaceIcon;
        }
        static FubenRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["MainType"] = get_MainType;
            mField["AssistType"] = get_AssistType;
            mField["Difficulty"] = get_Difficulty;
            mField["BeforeStoryId"] = get_BeforeStoryId;
            mField["AfterStoryId"] = get_AfterStoryId;
            mField["TodayCount"] = get_TodayCount;
            mField["TodayBuyCount"] = get_TodayBuyCount;
            mField["NeedItemId"] = get_NeedItemId;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["ResetItemId"] = get_ResetItemId;
            mField["ResetItemCount"] = get_ResetItemCount;
            mField["ViewConditionId"] = get_ViewConditionId;
            mField["EnterConditionId"] = get_EnterConditionId;
            mField["OpenTime"] = get_OpenTime;
            mField["CanEnterTime"] = get_CanEnterTime;
            mField["OpenLastMinutes"] = get_OpenLastMinutes;
            mField["TimeLimitMinutes"] = get_TimeLimitMinutes;
            mField["SweepLimitMinutes"] = get_SweepLimitMinutes;
            mField["FightPoint"] = get_FightPoint;
            mField["Desc"] = get_Desc;
            mField["RewardId"] = get_RewardId;
            mField["RewardCount"] = get_RewardCount;
            mField["ScriptId"] = get_ScriptId;
            mField["FlagId"] = get_FlagId;
            mField["TodayCountExdata"] = get_TodayCountExdata;
            mField["ResetExdata"] = get_ResetExdata;
            mField["TotleExdata"] = get_TotleExdata;
            mField["SceneId"] = get_SceneId;
            mField["TimeExdata"] = get_TimeExdata;
            mField["Icon"] = get_Icon;
            mField["QueueParam"] = get_QueueParam;
            mField["DrawReward"] = get_DrawReward;
            mField["ScanExp"] = get_ScanExp;
            mField["ScanGold"] = get_ScanGold;
            mField["ScanReward"] = get_ScanReward;
            mField["FubenInfoParam"] = get_FubenInfoParam;
            mField["FubenLogicID"] = get_FubenLogicID;
            mField["IsDynamicExp"] = get_IsDynamicExp;
            mField["DynamicExpRatio"] = get_DynamicExpRatio;
            mField["ScanDynamicExpRatio"] = get_ScanDynamicExpRatio;
            mField["DisplayReward"] = get_DisplayReward;
            mField["DisplayCount"] = get_DisplayCount;
            mField["CanInspire"] = get_CanInspire;
            mField["CanGroupEnter"] = get_CanGroupEnter;
            mField["IsPlaySlow"] = get_IsPlaySlow;
            mField["IsDyncReward"] = get_IsDyncReward;
            mField["IsStarReward"] = get_IsStarReward;
            mField["StarRewardProb"] = get_StarRewardProb;
            mField["MieshiScoreDesc"] = get_MieshiScoreDesc;
            mField["MieshiRewardDesc"] = get_MieshiRewardDesc;
            mField["FaceIcon"] = get_FaceIcon;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StatsRecord :IRecord
    {
        public static string __TableName = "Stats.txt";
        public int Id { get; private set; }
        public int[] FightPoint = new int[3];
        public int PetFight { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FightPoint[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PetFight = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StatsRecord)_this).Id;
        }
        static object get_FightPoint(IRecord _this)
        {
            return ((StatsRecord)_this).FightPoint;
        }
        static object get_PetFight(IRecord _this)
        {
            return ((StatsRecord)_this).PetFight;
        }
        static StatsRecord()
        {
            mField["Id"] = get_Id;
            mField["FightPoint"] = get_FightPoint;
            mField["PetFight"] = get_PetFight;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PlotFubenRecord :IRecord
    {
        public static string __TableName = "PlotFuben.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int[] OpenLevel = new int[3];
        [TableBinding("Fuben")]
        [ListSize(3)]
        public ReadonlyList<int> Difficulty { get; private set; } 
        public string ViewCondition { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int FubenType { get; private set; }
        public int ShowCondition { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                OpenLevel[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenLevel[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenLevel[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Difficulty=new ReadonlyList<int>(3);
                Difficulty[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Difficulty[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Difficulty[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ViewCondition = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenType = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowCondition = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PlotFubenRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((PlotFubenRecord)_this).Name;
        }
        static object get_OpenLevel(IRecord _this)
        {
            return ((PlotFubenRecord)_this).OpenLevel;
        }
        static object get_Difficulty(IRecord _this)
        {
            return ((PlotFubenRecord)_this).Difficulty;
        }
        static object get_ViewCondition(IRecord _this)
        {
            return ((PlotFubenRecord)_this).ViewCondition;
        }
        static object get_Icon(IRecord _this)
        {
            return ((PlotFubenRecord)_this).Icon;
        }
        static object get_FubenType(IRecord _this)
        {
            return ((PlotFubenRecord)_this).FubenType;
        }
        static object get_ShowCondition(IRecord _this)
        {
            return ((PlotFubenRecord)_this).ShowCondition;
        }
        static PlotFubenRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["OpenLevel"] = get_OpenLevel;
            mField["Difficulty"] = get_Difficulty;
            mField["ViewCondition"] = get_ViewCondition;
            mField["Icon"] = get_Icon;
            mField["FubenType"] = get_FubenType;
            mField["ShowCondition"] = get_ShowCondition;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StoreRecord :IRecord
    {
        public static string __TableName = "Store.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
       // [TableBinding("ItemBase")]
        public int ItemId { get; private set; }
        public int ItemCount { get; private set; }
        public int Type { get; private set; }
       // [TableBinding("ItemBase")]
        public int NeedType { get; private set; }
        public int NeedValue { get; private set; }
        public int DayCount { get; private set; }
        public int WeekCount { get; private set; }
        public int MonthCount { get; private set; }
        public int SpecialOffer { get; private set; }
      //  [TableBinding("ItemBase")]
        public int NeedItem { get; private set; }
        public int NeedCount { get; private set; }
        public int SeeCharacterID { get; private set; }
        public int BugSign { get; private set; }
        public int DisplayCondition { get; private set; }
        public int BuyCondition { get; private set; }
        public int GoodsType { get; private set; }
        public int Old { get; private set; }
      //  [TableBinding("SkillUpgrading")]
        public int WaveValue { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = Table_Tamplet.Convert_String(temp[__column++]);
                ItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedType = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedValue = Table_Tamplet.Convert_Int(temp[__column++]);
                DayCount = Table_Tamplet.Convert_Int(temp[__column++]);
                WeekCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MonthCount = Table_Tamplet.Convert_Int(temp[__column++]);
                SpecialOffer = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItem = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                SeeCharacterID = Table_Tamplet.Convert_Int(temp[__column++]);
                BugSign = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCondition = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyCondition = Table_Tamplet.Convert_Int(temp[__column++]);
                GoodsType = Table_Tamplet.Convert_Int(temp[__column++]);
                Old = Table_Tamplet.Convert_Int(temp[__column++]);
                WaveValue = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StoreRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((StoreRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((StoreRecord)_this).Desc;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((StoreRecord)_this).ItemId;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((StoreRecord)_this).ItemCount;
        }
        static object get_Type(IRecord _this)
        {
            return ((StoreRecord)_this).Type;
        }
        static object get_NeedType(IRecord _this)
        {
            return ((StoreRecord)_this).NeedType;
        }
        static object get_NeedValue(IRecord _this)
        {
            return ((StoreRecord)_this).NeedValue;
        }
        static object get_DayCount(IRecord _this)
        {
            return ((StoreRecord)_this).DayCount;
        }
        static object get_WeekCount(IRecord _this)
        {
            return ((StoreRecord)_this).WeekCount;
        }
        static object get_MonthCount(IRecord _this)
        {
            return ((StoreRecord)_this).MonthCount;
        }
        static object get_SpecialOffer(IRecord _this)
        {
            return ((StoreRecord)_this).SpecialOffer;
        }
        static object get_NeedItem(IRecord _this)
        {
            return ((StoreRecord)_this).NeedItem;
        }
        static object get_NeedCount(IRecord _this)
        {
            return ((StoreRecord)_this).NeedCount;
        }
        static object get_SeeCharacterID(IRecord _this)
        {
            return ((StoreRecord)_this).SeeCharacterID;
        }
        static object get_BugSign(IRecord _this)
        {
            return ((StoreRecord)_this).BugSign;
        }
        static object get_DisplayCondition(IRecord _this)
        {
            return ((StoreRecord)_this).DisplayCondition;
        }
        static object get_BuyCondition(IRecord _this)
        {
            return ((StoreRecord)_this).BuyCondition;
        }
        static object get_GoodsType(IRecord _this)
        {
            return ((StoreRecord)_this).GoodsType;
        }
        static object get_Old(IRecord _this)
        {
            return ((StoreRecord)_this).Old;
        }
        static object get_WaveValue(IRecord _this)
        {
            return ((StoreRecord)_this).WaveValue;
        }
        static StoreRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["ItemId"] = get_ItemId;
            mField["ItemCount"] = get_ItemCount;
            mField["Type"] = get_Type;
            mField["NeedType"] = get_NeedType;
            mField["NeedValue"] = get_NeedValue;
            mField["DayCount"] = get_DayCount;
            mField["WeekCount"] = get_WeekCount;
            mField["MonthCount"] = get_MonthCount;
            mField["SpecialOffer"] = get_SpecialOffer;
            mField["NeedItem"] = get_NeedItem;
            mField["NeedCount"] = get_NeedCount;
            mField["SeeCharacterID"] = get_SeeCharacterID;
            mField["BugSign"] = get_BugSign;
            mField["DisplayCondition"] = get_DisplayCondition;
            mField["BuyCondition"] = get_BuyCondition;
            mField["GoodsType"] = get_GoodsType;
            mField["Old"] = get_Old;
            mField["WaveValue"] = get_WaveValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StoryRecord :IRecord
    {
        public static string __TableName = "Story.txt";
        public int Id { get; private set; }
        public int AnimType { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int IsPassAnimation { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                AnimType = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Path = temp[__column++];
                IsPassAnimation = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StoryRecord)_this).Id;
        }
        static object get_AnimType(IRecord _this)
        {
            return ((StoryRecord)_this).AnimType;
        }
        static object get_Name(IRecord _this)
        {
            return ((StoryRecord)_this).Name;
        }
        static object get_Path(IRecord _this)
        {
            return ((StoryRecord)_this).Path;
        }
        static object get_IsPassAnimation(IRecord _this)
        {
            return ((StoryRecord)_this).IsPassAnimation;
        }
        static StoryRecord()
        {
            mField["Id"] = get_Id;
            mField["AnimType"] = get_AnimType;
            mField["Name"] = get_Name;
            mField["Path"] = get_Path;
            mField["IsPassAnimation"] = get_IsPassAnimation;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BuildingRecord :IRecord
    {
        public static string __TableName = "Building.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        [TableBinding("BuildingRes")]
        public int ResId { get; private set; }
        public int PetCount { get; private set; }
        public int Type { get; private set; }
        public int AreaType { get; private set; }
        public int Level { get; private set; }
        public int BuildMaxLevel { get; private set; }
        public int NextId { get; private set; }
        public int GetMainHouseExp { get; private set; }
        public int NeedHomeLevel { get; private set; }
        public int[] NeedItemId = new int[2];
        public int[] NeedItemCount = new int[2];
        public int NeedMinutes { get; private set; }
        public int CanRemove { get; private set; }
        public int RemoveNeedCityLevel { get; private set; }
        public int RemoveNeedRes { get; private set; }
        public int RemoveNeedCount { get; private set; }
        public int RemovedBuildID { get; private set; }
        public int ServiceId { get; private set; }
        public int FlagId { get; private set; }
        public int OrderRefleshRule { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                ResId = Table_Tamplet.Convert_Int(temp[__column++]);
                PetCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                AreaType = Table_Tamplet.Convert_Int(temp[__column++]);
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                BuildMaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                NextId = Table_Tamplet.Convert_Int(temp[__column++]);
                GetMainHouseExp = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedHomeLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedMinutes = Table_Tamplet.Convert_Int(temp[__column++]);
                CanRemove = Table_Tamplet.Convert_Int(temp[__column++]);
                RemoveNeedCityLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                RemoveNeedRes = Table_Tamplet.Convert_Int(temp[__column++]);
                RemoveNeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                RemovedBuildID = Table_Tamplet.Convert_Int(temp[__column++]);
                ServiceId = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagId = Table_Tamplet.Convert_Int(temp[__column++]);
                OrderRefleshRule = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BuildingRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((BuildingRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((BuildingRecord)_this).Desc;
        }
        static object get_ResId(IRecord _this)
        {
            return ((BuildingRecord)_this).ResId;
        }
        static object get_PetCount(IRecord _this)
        {
            return ((BuildingRecord)_this).PetCount;
        }
        static object get_Type(IRecord _this)
        {
            return ((BuildingRecord)_this).Type;
        }
        static object get_AreaType(IRecord _this)
        {
            return ((BuildingRecord)_this).AreaType;
        }
        static object get_Level(IRecord _this)
        {
            return ((BuildingRecord)_this).Level;
        }
        static object get_BuildMaxLevel(IRecord _this)
        {
            return ((BuildingRecord)_this).BuildMaxLevel;
        }
        static object get_NextId(IRecord _this)
        {
            return ((BuildingRecord)_this).NextId;
        }
        static object get_GetMainHouseExp(IRecord _this)
        {
            return ((BuildingRecord)_this).GetMainHouseExp;
        }
        static object get_NeedHomeLevel(IRecord _this)
        {
            return ((BuildingRecord)_this).NeedHomeLevel;
        }
        static object get_NeedItemId(IRecord _this)
        {
            return ((BuildingRecord)_this).NeedItemId;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((BuildingRecord)_this).NeedItemCount;
        }
        static object get_NeedMinutes(IRecord _this)
        {
            return ((BuildingRecord)_this).NeedMinutes;
        }
        static object get_CanRemove(IRecord _this)
        {
            return ((BuildingRecord)_this).CanRemove;
        }
        static object get_RemoveNeedCityLevel(IRecord _this)
        {
            return ((BuildingRecord)_this).RemoveNeedCityLevel;
        }
        static object get_RemoveNeedRes(IRecord _this)
        {
            return ((BuildingRecord)_this).RemoveNeedRes;
        }
        static object get_RemoveNeedCount(IRecord _this)
        {
            return ((BuildingRecord)_this).RemoveNeedCount;
        }
        static object get_RemovedBuildID(IRecord _this)
        {
            return ((BuildingRecord)_this).RemovedBuildID;
        }
        static object get_ServiceId(IRecord _this)
        {
            return ((BuildingRecord)_this).ServiceId;
        }
        static object get_FlagId(IRecord _this)
        {
            return ((BuildingRecord)_this).FlagId;
        }
        static object get_OrderRefleshRule(IRecord _this)
        {
            return ((BuildingRecord)_this).OrderRefleshRule;
        }
        static BuildingRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["ResId"] = get_ResId;
            mField["PetCount"] = get_PetCount;
            mField["Type"] = get_Type;
            mField["AreaType"] = get_AreaType;
            mField["Level"] = get_Level;
            mField["BuildMaxLevel"] = get_BuildMaxLevel;
            mField["NextId"] = get_NextId;
            mField["GetMainHouseExp"] = get_GetMainHouseExp;
            mField["NeedHomeLevel"] = get_NeedHomeLevel;
            mField["NeedItemId"] = get_NeedItemId;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["NeedMinutes"] = get_NeedMinutes;
            mField["CanRemove"] = get_CanRemove;
            mField["RemoveNeedCityLevel"] = get_RemoveNeedCityLevel;
            mField["RemoveNeedRes"] = get_RemoveNeedRes;
            mField["RemoveNeedCount"] = get_RemoveNeedCount;
            mField["RemovedBuildID"] = get_RemovedBuildID;
            mField["ServiceId"] = get_ServiceId;
            mField["FlagId"] = get_FlagId;
            mField["OrderRefleshRule"] = get_OrderRefleshRule;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BuildingResRecord :IRecord
    {
        public static string __TableName = "BuildingRes.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int Scale { get; private set; }
        public int Sound { get; private set; }
        public int Effect { get; private set; }
        public int UpGradeSound { get; private set; }
        public int OperationSound { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Path = temp[__column++];
                Scale = Table_Tamplet.Convert_Int(temp[__column++]);
                Sound = Table_Tamplet.Convert_Int(temp[__column++]);
                Effect = Table_Tamplet.Convert_Int(temp[__column++]);
                UpGradeSound = Table_Tamplet.Convert_Int(temp[__column++]);
                OperationSound = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BuildingResRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((BuildingResRecord)_this).Name;
        }
        static object get_Path(IRecord _this)
        {
            return ((BuildingResRecord)_this).Path;
        }
        static object get_Scale(IRecord _this)
        {
            return ((BuildingResRecord)_this).Scale;
        }
        static object get_Sound(IRecord _this)
        {
            return ((BuildingResRecord)_this).Sound;
        }
        static object get_Effect(IRecord _this)
        {
            return ((BuildingResRecord)_this).Effect;
        }
        static object get_UpGradeSound(IRecord _this)
        {
            return ((BuildingResRecord)_this).UpGradeSound;
        }
        static object get_OperationSound(IRecord _this)
        {
            return ((BuildingResRecord)_this).OperationSound;
        }
        static object get_Icon(IRecord _this)
        {
            return ((BuildingResRecord)_this).Icon;
        }
        static BuildingResRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Path"] = get_Path;
            mField["Scale"] = get_Scale;
            mField["Sound"] = get_Sound;
            mField["Effect"] = get_Effect;
            mField["UpGradeSound"] = get_UpGradeSound;
            mField["OperationSound"] = get_OperationSound;
            mField["Icon"] = get_Icon;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BuildingRuleRecord :IRecord
    {
        public static string __TableName = "BuildingRule.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int[] CityLevel = new int[10];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                CityLevel[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                CityLevel[9] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BuildingRuleRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((BuildingRuleRecord)_this).Name;
        }
        static object get_CityLevel(IRecord _this)
        {
            return ((BuildingRuleRecord)_this).CityLevel;
        }
        static BuildingRuleRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["CityLevel"] = get_CityLevel;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BuildingServiceRecord :IRecord
    {
        public static string __TableName = "BuildingService.txt";
        public int Id { get; private set; }
        public int RewardParam { get; private set; }
        public int BuildingType { get; private set; }
        public int[] Param = new int[8];
        public int TipsIndex { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardParam = Table_Tamplet.Convert_Int(temp[__column++]);
                BuildingType = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                TipsIndex = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BuildingServiceRecord)_this).Id;
        }
        static object get_RewardParam(IRecord _this)
        {
            return ((BuildingServiceRecord)_this).RewardParam;
        }
        static object get_BuildingType(IRecord _this)
        {
            return ((BuildingServiceRecord)_this).BuildingType;
        }
        static object get_Param(IRecord _this)
        {
            return ((BuildingServiceRecord)_this).Param;
        }
        static object get_TipsIndex(IRecord _this)
        {
            return ((BuildingServiceRecord)_this).TipsIndex;
        }
        static BuildingServiceRecord()
        {
            mField["Id"] = get_Id;
            mField["RewardParam"] = get_RewardParam;
            mField["BuildingType"] = get_BuildingType;
            mField["Param"] = get_Param;
            mField["TipsIndex"] = get_TipsIndex;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class HomeSenceRecord :IRecord
    {
        public static string __TableName = "HomeSence.txt";
        public int Id { get; private set; }
        public string Desc { get; private set; }
        public int BuildId { get; private set; }
        public float PosX { get;        set; }
        public float PosY { get;        set; }
        public float PosZ { get;        set; }
        public float RetinuePosX { get;        set; }
        public float RetinuePosY { get;        set; }
        public int BuildType { get; private set; }
        public float FaceCorrection { get;        set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                BuildId = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PosY = Table_Tamplet.Convert_Float(temp[__column++]);
                PosZ = Table_Tamplet.Convert_Float(temp[__column++]);
                RetinuePosX = Table_Tamplet.Convert_Float(temp[__column++]);
                RetinuePosY = Table_Tamplet.Convert_Float(temp[__column++]);
                BuildType = Table_Tamplet.Convert_Int(temp[__column++]);
                FaceCorrection = Table_Tamplet.Convert_Float(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((HomeSenceRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((HomeSenceRecord)_this).Desc;
        }
        static object get_BuildId(IRecord _this)
        {
            return ((HomeSenceRecord)_this).BuildId;
        }
        static object get_PosX(IRecord _this)
        {
            return ((HomeSenceRecord)_this).PosX;
        }
        static object get_PosY(IRecord _this)
        {
            return ((HomeSenceRecord)_this).PosY;
        }
        static object get_PosZ(IRecord _this)
        {
            return ((HomeSenceRecord)_this).PosZ;
        }
        static object get_RetinuePosX(IRecord _this)
        {
            return ((HomeSenceRecord)_this).RetinuePosX;
        }
        static object get_RetinuePosY(IRecord _this)
        {
            return ((HomeSenceRecord)_this).RetinuePosY;
        }
        static object get_BuildType(IRecord _this)
        {
            return ((HomeSenceRecord)_this).BuildType;
        }
        static object get_FaceCorrection(IRecord _this)
        {
            return ((HomeSenceRecord)_this).FaceCorrection;
        }
        static HomeSenceRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
            mField["BuildId"] = get_BuildId;
            mField["PosX"] = get_PosX;
            mField["PosY"] = get_PosY;
            mField["PosZ"] = get_PosZ;
            mField["RetinuePosX"] = get_RetinuePosX;
            mField["RetinuePosY"] = get_RetinuePosY;
            mField["BuildType"] = get_BuildType;
            mField["FaceCorrection"] = get_FaceCorrection;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PetRecord :IRecord
    {
        public static string __TableName = "Pet.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        [TableBinding("CharacterBase")]
        public int CharacterID { get; private set; }
        [TableBinding("Icon")]
        public int IconID { get; private set; }
        public int Sound { get; private set; }
        public int Type { get; private set; }
        public int Ladder { get; private set; }
        public int MaxLadder { get; private set; }
        public int NeedItemId { get; private set; }
        public int NeedItemCount { get; private set; }
        public int NeedTime { get; private set; }
        public int NextId { get; private set; }
        public int[] Skill = new int[4];
        public int[] ActiveLadder = new int[4];
        public int[] Speciality = new int[3];
        public int NeedExp { get; private set; }
        public int AttrRef { get; private set; }
        public int[] SpecialityLibrary = new int[3];
        public int ResolvePartCount { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                CharacterID = Table_Tamplet.Convert_Int(temp[__column++]);
                IconID = Table_Tamplet.Convert_Int(temp[__column++]);
                Sound = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Ladder = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLadder = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedTime = Table_Tamplet.Convert_Int(temp[__column++]);
                NextId = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveLadder[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveLadder[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveLadder[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveLadder[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Speciality[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Speciality[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Speciality[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedExp = Table_Tamplet.Convert_Int(temp[__column++]);
                AttrRef = Table_Tamplet.Convert_Int(temp[__column++]);
                SpecialityLibrary[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                SpecialityLibrary[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                SpecialityLibrary[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ResolvePartCount = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PetRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((PetRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((PetRecord)_this).Desc;
        }
        static object get_CharacterID(IRecord _this)
        {
            return ((PetRecord)_this).CharacterID;
        }
        static object get_IconID(IRecord _this)
        {
            return ((PetRecord)_this).IconID;
        }
        static object get_Sound(IRecord _this)
        {
            return ((PetRecord)_this).Sound;
        }
        static object get_Type(IRecord _this)
        {
            return ((PetRecord)_this).Type;
        }
        static object get_Ladder(IRecord _this)
        {
            return ((PetRecord)_this).Ladder;
        }
        static object get_MaxLadder(IRecord _this)
        {
            return ((PetRecord)_this).MaxLadder;
        }
        static object get_NeedItemId(IRecord _this)
        {
            return ((PetRecord)_this).NeedItemId;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((PetRecord)_this).NeedItemCount;
        }
        static object get_NeedTime(IRecord _this)
        {
            return ((PetRecord)_this).NeedTime;
        }
        static object get_NextId(IRecord _this)
        {
            return ((PetRecord)_this).NextId;
        }
        static object get_Skill(IRecord _this)
        {
            return ((PetRecord)_this).Skill;
        }
        static object get_ActiveLadder(IRecord _this)
        {
            return ((PetRecord)_this).ActiveLadder;
        }
        static object get_Speciality(IRecord _this)
        {
            return ((PetRecord)_this).Speciality;
        }
        static object get_NeedExp(IRecord _this)
        {
            return ((PetRecord)_this).NeedExp;
        }
        static object get_AttrRef(IRecord _this)
        {
            return ((PetRecord)_this).AttrRef;
        }
        static object get_SpecialityLibrary(IRecord _this)
        {
            return ((PetRecord)_this).SpecialityLibrary;
        }
        static object get_ResolvePartCount(IRecord _this)
        {
            return ((PetRecord)_this).ResolvePartCount;
        }
        static PetRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["CharacterID"] = get_CharacterID;
            mField["IconID"] = get_IconID;
            mField["Sound"] = get_Sound;
            mField["Type"] = get_Type;
            mField["Ladder"] = get_Ladder;
            mField["MaxLadder"] = get_MaxLadder;
            mField["NeedItemId"] = get_NeedItemId;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["NeedTime"] = get_NeedTime;
            mField["NextId"] = get_NextId;
            mField["Skill"] = get_Skill;
            mField["ActiveLadder"] = get_ActiveLadder;
            mField["Speciality"] = get_Speciality;
            mField["NeedExp"] = get_NeedExp;
            mField["AttrRef"] = get_AttrRef;
            mField["SpecialityLibrary"] = get_SpecialityLibrary;
            mField["ResolvePartCount"] = get_ResolvePartCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PetSkillRecord :IRecord
    {
        public static string __TableName = "PetSkill.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public int SkillType { get; private set; }
        [TableBinding("Icon")]
        public int SkillIcon { get; private set; }
        public int MatchType { get; private set; }
        public int EffectId { get; private set; }
        public int[] Param = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                SkillType = Table_Tamplet.Convert_Int(temp[__column++]);
                SkillIcon = Table_Tamplet.Convert_Int(temp[__column++]);
                MatchType = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectId = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PetSkillRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((PetSkillRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((PetSkillRecord)_this).Desc;
        }
        static object get_SkillType(IRecord _this)
        {
            return ((PetSkillRecord)_this).SkillType;
        }
        static object get_SkillIcon(IRecord _this)
        {
            return ((PetSkillRecord)_this).SkillIcon;
        }
        static object get_MatchType(IRecord _this)
        {
            return ((PetSkillRecord)_this).MatchType;
        }
        static object get_EffectId(IRecord _this)
        {
            return ((PetSkillRecord)_this).EffectId;
        }
        static object get_Param(IRecord _this)
        {
            return ((PetSkillRecord)_this).Param;
        }
        static PetSkillRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["SkillType"] = get_SkillType;
            mField["SkillIcon"] = get_SkillIcon;
            mField["MatchType"] = get_MatchType;
            mField["EffectId"] = get_EffectId;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ServiceRecord :IRecord
    {
        public static string __TableName = "Service.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public int[] Param = new int[4];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ServiceRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ServiceRecord)_this).Name;
        }
        static object get_Type(IRecord _this)
        {
            return ((ServiceRecord)_this).Type;
        }
        static object get_Param(IRecord _this)
        {
            return ((ServiceRecord)_this).Param;
        }
        static ServiceRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Type"] = get_Type;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StoreTypeRecord :IRecord
    {
        public static string __TableName = "StoreType.txt";
        public int Id { get; private set; }
        public string Type { get; private set; }
        public string Name { get; private set; }
        [TableBinding("ItemBase")]
        public int ResType { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = temp[__column++];
                Name = temp[__column++];
                ResType = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StoreTypeRecord)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((StoreTypeRecord)_this).Type;
        }
        static object get_Name(IRecord _this)
        {
            return ((StoreTypeRecord)_this).Name;
        }
        static object get_ResType(IRecord _this)
        {
            return ((StoreTypeRecord)_this).ResType;
        }
        static StoreTypeRecord()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["Name"] = get_Name;
            mField["ResType"] = get_ResType;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ElfRecord :IRecord
    {
        public static string __TableName = "Elf.txt";
        public int Id { get; private set; }
        public string ElfName { get; private set; }
        public int ElfModel { get; private set; }
        public int Offset { get; private set; }
        public int CameraZoom { get; private set; }
        public int ElfType { get; private set; }
        public int[] ElfInitProp = new int[6];
        public int[] ElfProp = new int[6];
        public int[] GrowAddValue = new int[6];
        public int RandomPropCount { get; private set; }
        public int RandomPropPro { get; private set; }
        public int RandomPropValue { get; private set; }
        public int[] BelongGroup = new int[3];
        public int MaxLevel { get; private set; }
        public int[] ResolveCoef = new int[2];
        public int ElfShader { get; private set; }
        public List<int> ElfStarUp = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfName = temp[__column++];
                ElfModel = Table_Tamplet.Convert_Int(temp[__column++]);
                Offset = Table_Tamplet.Convert_Int(temp[__column++]);
                CameraZoom = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfType = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfInitProp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfProp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfInitProp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfProp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfInitProp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfProp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfInitProp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfProp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfInitProp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfProp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfInitProp[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfProp[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomPropCount = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomPropPro = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomPropValue = Table_Tamplet.Convert_Int(temp[__column++]);
                BelongGroup[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BelongGroup[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BelongGroup[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                ResolveCoef[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ResolveCoef[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfShader = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(ElfStarUp,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ElfRecord)_this).Id;
        }
        static object get_ElfName(IRecord _this)
        {
            return ((ElfRecord)_this).ElfName;
        }
        static object get_ElfModel(IRecord _this)
        {
            return ((ElfRecord)_this).ElfModel;
        }
        static object get_Offset(IRecord _this)
        {
            return ((ElfRecord)_this).Offset;
        }
        static object get_CameraZoom(IRecord _this)
        {
            return ((ElfRecord)_this).CameraZoom;
        }
        static object get_ElfType(IRecord _this)
        {
            return ((ElfRecord)_this).ElfType;
        }
        static object get_ElfInitProp(IRecord _this)
        {
            return ((ElfRecord)_this).ElfInitProp;
        }
        static object get_ElfProp(IRecord _this)
        {
            return ((ElfRecord)_this).ElfProp;
        }
        static object get_GrowAddValue(IRecord _this)
        {
            return ((ElfRecord)_this).GrowAddValue;
        }
        static object get_RandomPropCount(IRecord _this)
        {
            return ((ElfRecord)_this).RandomPropCount;
        }
        static object get_RandomPropPro(IRecord _this)
        {
            return ((ElfRecord)_this).RandomPropPro;
        }
        static object get_RandomPropValue(IRecord _this)
        {
            return ((ElfRecord)_this).RandomPropValue;
        }
        static object get_BelongGroup(IRecord _this)
        {
            return ((ElfRecord)_this).BelongGroup;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((ElfRecord)_this).MaxLevel;
        }
        static object get_ResolveCoef(IRecord _this)
        {
            return ((ElfRecord)_this).ResolveCoef;
        }
        static object get_ElfShader(IRecord _this)
        {
            return ((ElfRecord)_this).ElfShader;
        }
        static object get_ElfStarUp(IRecord _this)
        {
            return ((ElfRecord)_this).ElfStarUp;
        }
        static ElfRecord()
        {
            mField["Id"] = get_Id;
            mField["ElfName"] = get_ElfName;
            mField["ElfModel"] = get_ElfModel;
            mField["Offset"] = get_Offset;
            mField["CameraZoom"] = get_CameraZoom;
            mField["ElfType"] = get_ElfType;
            mField["ElfInitProp"] = get_ElfInitProp;
            mField["ElfProp"] = get_ElfProp;
            mField["GrowAddValue"] = get_GrowAddValue;
            mField["RandomPropCount"] = get_RandomPropCount;
            mField["RandomPropPro"] = get_RandomPropPro;
            mField["RandomPropValue"] = get_RandomPropValue;
            mField["BelongGroup"] = get_BelongGroup;
            mField["MaxLevel"] = get_MaxLevel;
            mField["ResolveCoef"] = get_ResolveCoef;
            mField["ElfShader"] = get_ElfShader;
            mField["ElfStarUp"] = get_ElfStarUp;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ElfGroupRecord :IRecord
    {
        public static string __TableName = "ElfGroup.txt";
        public int Id { get; private set; }
        public string GroupName { get; private set; }
        [TableBinding("Elf")]
        [ListSize(3)]
        public ReadonlyList<int> ElfID { get; private set; } 
        public int[] GroupPorp = new int[6];
        public int[] PropValue = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupName = temp[__column++];
                ElfID=new ReadonlyList<int>(3);
                ElfID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ElfID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupPorp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupPorp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupPorp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupPorp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupPorp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                GroupPorp[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ElfGroupRecord)_this).Id;
        }
        static object get_GroupName(IRecord _this)
        {
            return ((ElfGroupRecord)_this).GroupName;
        }
        static object get_ElfID(IRecord _this)
        {
            return ((ElfGroupRecord)_this).ElfID;
        }
        static object get_GroupPorp(IRecord _this)
        {
            return ((ElfGroupRecord)_this).GroupPorp;
        }
        static object get_PropValue(IRecord _this)
        {
            return ((ElfGroupRecord)_this).PropValue;
        }
        static ElfGroupRecord()
        {
            mField["Id"] = get_Id;
            mField["GroupName"] = get_GroupName;
            mField["ElfID"] = get_ElfID;
            mField["GroupPorp"] = get_GroupPorp;
            mField["PropValue"] = get_PropValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class QueueRecord :IRecord
    {
        public static string __TableName = "Queue.txt";
        public int Id { get; private set; }
        public int CountLimit { get; private set; }
        public int AppType { get; private set; }
        public int Param { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                CountLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                AppType = Table_Tamplet.Convert_Int(temp[__column++]);
                Param = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((QueueRecord)_this).Id;
        }
        static object get_CountLimit(IRecord _this)
        {
            return ((QueueRecord)_this).CountLimit;
        }
        static object get_AppType(IRecord _this)
        {
            return ((QueueRecord)_this).AppType;
        }
        static object get_Param(IRecord _this)
        {
            return ((QueueRecord)_this).Param;
        }
        static QueueRecord()
        {
            mField["Id"] = get_Id;
            mField["CountLimit"] = get_CountLimit;
            mField["AppType"] = get_AppType;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DrawRecord :IRecord
    {
        public static string __TableName = "Draw.txt";
        public int Id { get; private set; }
        public int[] DropItem = new int[4];
        public int[] Count = new int[4];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Count[3] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DrawRecord)_this).Id;
        }
        static object get_DropItem(IRecord _this)
        {
            return ((DrawRecord)_this).DropItem;
        }
        static object get_Count(IRecord _this)
        {
            return ((DrawRecord)_this).Count;
        }
        static DrawRecord()
        {
            mField["Id"] = get_Id;
            mField["DropItem"] = get_DropItem;
            mField["Count"] = get_Count;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PlantRecord :IRecord
    {
        public static string __TableName = "Plant.txt";
        public int Id { get; private set; }
        public string PlantName { get; private set; }
        public string Disc { get; private set; }
        public int PlantItemID { get; private set; }
        public int PlantLevel { get; private set; }
        public int CanRemove { get; private set; }
        public int BirthResAnimation { get; private set; }
        public int MatureCycle { get; private set; }
        public int MatureReAnimation { get; private set; }
        public int HarvestItemID { get; private set; }
        public int[] HarvestCount = new int[2];
        public int RetinueGettingExp { get; private set; }
        public int ExtraRandomDrop { get; private set; }
        public int PlantType { get; private set; }
        public int GetHomeExp { get; private set; }
        public string GrowAtlasName { get; private set; }
        public string IconName { get; private set; }
        public int GrowStepCount { get; private set; }
        public int[] StepPicID = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                PlantName = temp[__column++];
                Disc = temp[__column++];
                PlantItemID = Table_Tamplet.Convert_Int(temp[__column++]);
                PlantLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                CanRemove = Table_Tamplet.Convert_Int(temp[__column++]);
                BirthResAnimation = Table_Tamplet.Convert_Int(temp[__column++]);
                MatureCycle = Table_Tamplet.Convert_Int(temp[__column++]);
                MatureReAnimation = Table_Tamplet.Convert_Int(temp[__column++]);
                HarvestItemID = Table_Tamplet.Convert_Int(temp[__column++]);
                HarvestCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                HarvestCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RetinueGettingExp = Table_Tamplet.Convert_Int(temp[__column++]);
                ExtraRandomDrop = Table_Tamplet.Convert_Int(temp[__column++]);
                PlantType = Table_Tamplet.Convert_Int(temp[__column++]);
                GetHomeExp = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAtlasName = temp[__column++];
                IconName = temp[__column++];
                GrowStepCount = Table_Tamplet.Convert_Int(temp[__column++]);
                StepPicID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                StepPicID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                StepPicID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                StepPicID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                StepPicID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                StepPicID[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PlantRecord)_this).Id;
        }
        static object get_PlantName(IRecord _this)
        {
            return ((PlantRecord)_this).PlantName;
        }
        static object get_Disc(IRecord _this)
        {
            return ((PlantRecord)_this).Disc;
        }
        static object get_PlantItemID(IRecord _this)
        {
            return ((PlantRecord)_this).PlantItemID;
        }
        static object get_PlantLevel(IRecord _this)
        {
            return ((PlantRecord)_this).PlantLevel;
        }
        static object get_CanRemove(IRecord _this)
        {
            return ((PlantRecord)_this).CanRemove;
        }
        static object get_BirthResAnimation(IRecord _this)
        {
            return ((PlantRecord)_this).BirthResAnimation;
        }
        static object get_MatureCycle(IRecord _this)
        {
            return ((PlantRecord)_this).MatureCycle;
        }
        static object get_MatureReAnimation(IRecord _this)
        {
            return ((PlantRecord)_this).MatureReAnimation;
        }
        static object get_HarvestItemID(IRecord _this)
        {
            return ((PlantRecord)_this).HarvestItemID;
        }
        static object get_HarvestCount(IRecord _this)
        {
            return ((PlantRecord)_this).HarvestCount;
        }
        static object get_RetinueGettingExp(IRecord _this)
        {
            return ((PlantRecord)_this).RetinueGettingExp;
        }
        static object get_ExtraRandomDrop(IRecord _this)
        {
            return ((PlantRecord)_this).ExtraRandomDrop;
        }
        static object get_PlantType(IRecord _this)
        {
            return ((PlantRecord)_this).PlantType;
        }
        static object get_GetHomeExp(IRecord _this)
        {
            return ((PlantRecord)_this).GetHomeExp;
        }
        static object get_GrowAtlasName(IRecord _this)
        {
            return ((PlantRecord)_this).GrowAtlasName;
        }
        static object get_IconName(IRecord _this)
        {
            return ((PlantRecord)_this).IconName;
        }
        static object get_GrowStepCount(IRecord _this)
        {
            return ((PlantRecord)_this).GrowStepCount;
        }
        static object get_StepPicID(IRecord _this)
        {
            return ((PlantRecord)_this).StepPicID;
        }
        static PlantRecord()
        {
            mField["Id"] = get_Id;
            mField["PlantName"] = get_PlantName;
            mField["Disc"] = get_Disc;
            mField["PlantItemID"] = get_PlantItemID;
            mField["PlantLevel"] = get_PlantLevel;
            mField["CanRemove"] = get_CanRemove;
            mField["BirthResAnimation"] = get_BirthResAnimation;
            mField["MatureCycle"] = get_MatureCycle;
            mField["MatureReAnimation"] = get_MatureReAnimation;
            mField["HarvestItemID"] = get_HarvestItemID;
            mField["HarvestCount"] = get_HarvestCount;
            mField["RetinueGettingExp"] = get_RetinueGettingExp;
            mField["ExtraRandomDrop"] = get_ExtraRandomDrop;
            mField["PlantType"] = get_PlantType;
            mField["GetHomeExp"] = get_GetHomeExp;
            mField["GrowAtlasName"] = get_GrowAtlasName;
            mField["IconName"] = get_IconName;
            mField["GrowStepCount"] = get_GrowStepCount;
            mField["StepPicID"] = get_StepPicID;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SeqFrameRecord :IRecord
    {
        public static string __TableName = "SeqFrame.txt";
        public int Id { get; private set; }
        public string AtlasName { get; private set; }
        public string PicName { get; private set; }
        public int Frame { get; private set; }
        public int DeltaTime { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                AtlasName = temp[__column++];
                PicName = temp[__column++];
                Frame = Table_Tamplet.Convert_Int(temp[__column++]);
                DeltaTime = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SeqFrameRecord)_this).Id;
        }
        static object get_AtlasName(IRecord _this)
        {
            return ((SeqFrameRecord)_this).AtlasName;
        }
        static object get_PicName(IRecord _this)
        {
            return ((SeqFrameRecord)_this).PicName;
        }
        static object get_Frame(IRecord _this)
        {
            return ((SeqFrameRecord)_this).Frame;
        }
        static object get_DeltaTime(IRecord _this)
        {
            return ((SeqFrameRecord)_this).DeltaTime;
        }
        static SeqFrameRecord()
        {
            mField["Id"] = get_Id;
            mField["AtlasName"] = get_AtlasName;
            mField["PicName"] = get_PicName;
            mField["Frame"] = get_Frame;
            mField["DeltaTime"] = get_DeltaTime;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MedalRecord :IRecord
    {
        public static string __TableName = "Medal.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public int CanEquipment { get; private set; }
        public int InitExp { get; private set; }
        public int MedalType { get; private set; }
        public int Quality { get; private set; }
        public int LevelUpExp { get; private set; }
        public int MaxLevel { get; private set; }
        public int[] AddPropID = new int[2];
        public int[] PropValue = new int[2];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                CanEquipment = Table_Tamplet.Convert_Int(temp[__column++]);
                InitExp = Table_Tamplet.Convert_Int(temp[__column++]);
                MedalType = Table_Tamplet.Convert_Int(temp[__column++]);
                Quality = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelUpExp = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MedalRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((MedalRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((MedalRecord)_this).Desc;
        }
        static object get_CanEquipment(IRecord _this)
        {
            return ((MedalRecord)_this).CanEquipment;
        }
        static object get_InitExp(IRecord _this)
        {
            return ((MedalRecord)_this).InitExp;
        }
        static object get_MedalType(IRecord _this)
        {
            return ((MedalRecord)_this).MedalType;
        }
        static object get_Quality(IRecord _this)
        {
            return ((MedalRecord)_this).Quality;
        }
        static object get_LevelUpExp(IRecord _this)
        {
            return ((MedalRecord)_this).LevelUpExp;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((MedalRecord)_this).MaxLevel;
        }
        static object get_AddPropID(IRecord _this)
        {
            return ((MedalRecord)_this).AddPropID;
        }
        static object get_PropValue(IRecord _this)
        {
            return ((MedalRecord)_this).PropValue;
        }
        static MedalRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["CanEquipment"] = get_CanEquipment;
            mField["InitExp"] = get_InitExp;
            mField["MedalType"] = get_MedalType;
            mField["Quality"] = get_Quality;
            mField["LevelUpExp"] = get_LevelUpExp;
            mField["MaxLevel"] = get_MaxLevel;
            mField["AddPropID"] = get_AddPropID;
            mField["PropValue"] = get_PropValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SailingRecord :IRecord
    {
        public static string __TableName = "Sailing.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int CanCall { get; private set; }
        public int NeedType { get; private set; }
        public int ItemCount { get; private set; }
        public int Distance { get; private set; }
        public int distanceParam { get; private set; }
        public int ConsumeType { get; private set; }
        public int SuccessGetExp { get; private set; }
        public int FailedGetExp { get; private set; }
        public int Direction { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                CanCall = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedType = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Distance = Table_Tamplet.Convert_Int(temp[__column++]);
                distanceParam = Table_Tamplet.Convert_Int(temp[__column++]);
                ConsumeType = Table_Tamplet.Convert_Int(temp[__column++]);
                SuccessGetExp = Table_Tamplet.Convert_Int(temp[__column++]);
                FailedGetExp = Table_Tamplet.Convert_Int(temp[__column++]);
                Direction = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SailingRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((SailingRecord)_this).Name;
        }
        static object get_CanCall(IRecord _this)
        {
            return ((SailingRecord)_this).CanCall;
        }
        static object get_NeedType(IRecord _this)
        {
            return ((SailingRecord)_this).NeedType;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((SailingRecord)_this).ItemCount;
        }
        static object get_Distance(IRecord _this)
        {
            return ((SailingRecord)_this).Distance;
        }
        static object get_distanceParam(IRecord _this)
        {
            return ((SailingRecord)_this).distanceParam;
        }
        static object get_ConsumeType(IRecord _this)
        {
            return ((SailingRecord)_this).ConsumeType;
        }
        static object get_SuccessGetExp(IRecord _this)
        {
            return ((SailingRecord)_this).SuccessGetExp;
        }
        static object get_FailedGetExp(IRecord _this)
        {
            return ((SailingRecord)_this).FailedGetExp;
        }
        static object get_Direction(IRecord _this)
        {
            return ((SailingRecord)_this).Direction;
        }
        static SailingRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["CanCall"] = get_CanCall;
            mField["NeedType"] = get_NeedType;
            mField["ItemCount"] = get_ItemCount;
            mField["Distance"] = get_Distance;
            mField["distanceParam"] = get_distanceParam;
            mField["ConsumeType"] = get_ConsumeType;
            mField["SuccessGetExp"] = get_SuccessGetExp;
            mField["FailedGetExp"] = get_FailedGetExp;
            mField["Direction"] = get_Direction;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class WingTrainRecord :IRecord
    {
        public static string __TableName = "WingTrain.txt";
        public int Id { get; private set; }
        public int WingPostion { get; private set; }
        public int TrainCount { get; private set; }
        public int TrainStar { get; private set; }
        public int Condition { get; private set; }
        public int MaterialID { get; private set; }
        public int MaterialCount { get; private set; }
        public int UsedMoney { get; private set; }
        public int AddExp { get; private set; }
        public int CritAddExp { get; private set; }
        public int ExpLimit { get; private set; }
        public int[] AddPropID = new int[10];
        public int[] AddPropValue = new int[10];
        public int UpStarID { get; private set; }
        public int PosX { get; private set; }
        public int PoxY { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                WingPostion = Table_Tamplet.Convert_Int(temp[__column++]);
                TrainCount = Table_Tamplet.Convert_Int(temp[__column++]);
                TrainStar = Table_Tamplet.Convert_Int(temp[__column++]);
                Condition = Table_Tamplet.Convert_Int(temp[__column++]);
                MaterialID = Table_Tamplet.Convert_Int(temp[__column++]);
                MaterialCount = Table_Tamplet.Convert_Int(temp[__column++]);
                UsedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                AddExp = Table_Tamplet.Convert_Int(temp[__column++]);
                CritAddExp = Table_Tamplet.Convert_Int(temp[__column++]);
                ExpLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                UpStarID = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Int(temp[__column++]);
                PoxY = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((WingTrainRecord)_this).Id;
        }
        static object get_WingPostion(IRecord _this)
        {
            return ((WingTrainRecord)_this).WingPostion;
        }
        static object get_TrainCount(IRecord _this)
        {
            return ((WingTrainRecord)_this).TrainCount;
        }
        static object get_TrainStar(IRecord _this)
        {
            return ((WingTrainRecord)_this).TrainStar;
        }
        static object get_Condition(IRecord _this)
        {
            return ((WingTrainRecord)_this).Condition;
        }
        static object get_MaterialID(IRecord _this)
        {
            return ((WingTrainRecord)_this).MaterialID;
        }
        static object get_MaterialCount(IRecord _this)
        {
            return ((WingTrainRecord)_this).MaterialCount;
        }
        static object get_UsedMoney(IRecord _this)
        {
            return ((WingTrainRecord)_this).UsedMoney;
        }
        static object get_AddExp(IRecord _this)
        {
            return ((WingTrainRecord)_this).AddExp;
        }
        static object get_CritAddExp(IRecord _this)
        {
            return ((WingTrainRecord)_this).CritAddExp;
        }
        static object get_ExpLimit(IRecord _this)
        {
            return ((WingTrainRecord)_this).ExpLimit;
        }
        static object get_AddPropID(IRecord _this)
        {
            return ((WingTrainRecord)_this).AddPropID;
        }
        static object get_AddPropValue(IRecord _this)
        {
            return ((WingTrainRecord)_this).AddPropValue;
        }
        static object get_UpStarID(IRecord _this)
        {
            return ((WingTrainRecord)_this).UpStarID;
        }
        static object get_PosX(IRecord _this)
        {
            return ((WingTrainRecord)_this).PosX;
        }
        static object get_PoxY(IRecord _this)
        {
            return ((WingTrainRecord)_this).PoxY;
        }
        static WingTrainRecord()
        {
            mField["Id"] = get_Id;
            mField["WingPostion"] = get_WingPostion;
            mField["TrainCount"] = get_TrainCount;
            mField["TrainStar"] = get_TrainStar;
            mField["Condition"] = get_Condition;
            mField["MaterialID"] = get_MaterialID;
            mField["MaterialCount"] = get_MaterialCount;
            mField["UsedMoney"] = get_UsedMoney;
            mField["AddExp"] = get_AddExp;
            mField["CritAddExp"] = get_CritAddExp;
            mField["ExpLimit"] = get_ExpLimit;
            mField["AddPropID"] = get_AddPropID;
            mField["AddPropValue"] = get_AddPropValue;
            mField["UpStarID"] = get_UpStarID;
            mField["PosX"] = get_PosX;
            mField["PoxY"] = get_PoxY;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class WingQualityRecord :IRecord
    {
        public static string __TableName = "WingQuality.txt";
        public int Id { get; private set; }
        public int Career { get; private set; }
        public int Segment { get; private set; }
        public int MaterialNeed { get; private set; }
        public int MaterialCount { get; private set; }
        public int UsedMoney { get; private set; }
        public int GrowAddValue { get; private set; }
        public int ValueLimit { get; private set; }
        public int LevelLimit { get; private set; }
        public int[] AddPropID = new int[11];
        public int[] AddPropValue = new int[11];
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        [TableBinding("ItemBase")]
        public int BreakNeedItem { get; private set; }
        public int BreakNeedCount { get; private set; }
        public int BreakNeedMoney { get; private set; }
        public int GrowProgress { get; private set; }
        public int[] GrowPropID = new int[6];
        public int[] GrowMinProp = new int[6];
        public int[] GrowMaxProp = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Career = Table_Tamplet.Convert_Int(temp[__column++]);
                Segment = Table_Tamplet.Convert_Int(temp[__column++]);
                MaterialNeed = Table_Tamplet.Convert_Int(temp[__column++]);
                MaterialCount = Table_Tamplet.Convert_Int(temp[__column++]);
                UsedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowAddValue = Table_Tamplet.Convert_Int(temp[__column++]);
                ValueLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropID[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropValue[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                BreakNeedItem = Table_Tamplet.Convert_Int(temp[__column++]);
                BreakNeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                BreakNeedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowProgress = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowPropID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMinProp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMaxProp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowPropID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMinProp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMaxProp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowPropID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMinProp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMaxProp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowPropID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMinProp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMaxProp[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowPropID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMinProp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMaxProp[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowPropID[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMinProp[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                GrowMaxProp[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((WingQualityRecord)_this).Id;
        }
        static object get_Career(IRecord _this)
        {
            return ((WingQualityRecord)_this).Career;
        }
        static object get_Segment(IRecord _this)
        {
            return ((WingQualityRecord)_this).Segment;
        }
        static object get_MaterialNeed(IRecord _this)
        {
            return ((WingQualityRecord)_this).MaterialNeed;
        }
        static object get_MaterialCount(IRecord _this)
        {
            return ((WingQualityRecord)_this).MaterialCount;
        }
        static object get_UsedMoney(IRecord _this)
        {
            return ((WingQualityRecord)_this).UsedMoney;
        }
        static object get_GrowAddValue(IRecord _this)
        {
            return ((WingQualityRecord)_this).GrowAddValue;
        }
        static object get_ValueLimit(IRecord _this)
        {
            return ((WingQualityRecord)_this).ValueLimit;
        }
        static object get_LevelLimit(IRecord _this)
        {
            return ((WingQualityRecord)_this).LevelLimit;
        }
        static object get_AddPropID(IRecord _this)
        {
            return ((WingQualityRecord)_this).AddPropID;
        }
        static object get_AddPropValue(IRecord _this)
        {
            return ((WingQualityRecord)_this).AddPropValue;
        }
        static object get_Icon(IRecord _this)
        {
            return ((WingQualityRecord)_this).Icon;
        }
        static object get_BreakNeedItem(IRecord _this)
        {
            return ((WingQualityRecord)_this).BreakNeedItem;
        }
        static object get_BreakNeedCount(IRecord _this)
        {
            return ((WingQualityRecord)_this).BreakNeedCount;
        }
        static object get_BreakNeedMoney(IRecord _this)
        {
            return ((WingQualityRecord)_this).BreakNeedMoney;
        }
        static object get_GrowProgress(IRecord _this)
        {
            return ((WingQualityRecord)_this).GrowProgress;
        }
        static object get_GrowPropID(IRecord _this)
        {
            return ((WingQualityRecord)_this).GrowPropID;
        }
        static object get_GrowMinProp(IRecord _this)
        {
            return ((WingQualityRecord)_this).GrowMinProp;
        }
        static object get_GrowMaxProp(IRecord _this)
        {
            return ((WingQualityRecord)_this).GrowMaxProp;
        }
        static WingQualityRecord()
        {
            mField["Id"] = get_Id;
            mField["Career"] = get_Career;
            mField["Segment"] = get_Segment;
            mField["MaterialNeed"] = get_MaterialNeed;
            mField["MaterialCount"] = get_MaterialCount;
            mField["UsedMoney"] = get_UsedMoney;
            mField["GrowAddValue"] = get_GrowAddValue;
            mField["ValueLimit"] = get_ValueLimit;
            mField["LevelLimit"] = get_LevelLimit;
            mField["AddPropID"] = get_AddPropID;
            mField["AddPropValue"] = get_AddPropValue;
            mField["Icon"] = get_Icon;
            mField["BreakNeedItem"] = get_BreakNeedItem;
            mField["BreakNeedCount"] = get_BreakNeedCount;
            mField["BreakNeedMoney"] = get_BreakNeedMoney;
            mField["GrowProgress"] = get_GrowProgress;
            mField["GrowPropID"] = get_GrowPropID;
            mField["GrowMinProp"] = get_GrowMinProp;
            mField["GrowMaxProp"] = get_GrowMaxProp;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PVPRuleRecord :IRecord
    {
        public static string __TableName = "PVPRule.txt";
        public int Id { get; private set; }
        public int CanPK { get; private set; }
        public int ProtectLevel { get; private set; }
        public int NameColorRule { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                CanPK = Table_Tamplet.Convert_Int(temp[__column++]);
                ProtectLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                NameColorRule = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PVPRuleRecord)_this).Id;
        }
        static object get_CanPK(IRecord _this)
        {
            return ((PVPRuleRecord)_this).CanPK;
        }
        static object get_ProtectLevel(IRecord _this)
        {
            return ((PVPRuleRecord)_this).ProtectLevel;
        }
        static object get_NameColorRule(IRecord _this)
        {
            return ((PVPRuleRecord)_this).NameColorRule;
        }
        static PVPRuleRecord()
        {
            mField["Id"] = get_Id;
            mField["CanPK"] = get_CanPK;
            mField["ProtectLevel"] = get_ProtectLevel;
            mField["NameColorRule"] = get_NameColorRule;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ArenaRewardRecord :IRecord
    {
        public static string __TableName = "ArenaReward.txt";
        public int Id { get; private set; }
        public int MaxDiamond { get; private set; }
        public int DayMoney { get; private set; }
        public int DayDiamond { get; private set; }
        [TableBinding("ItemBase")]
        [ListSize(3)]
        public ReadonlyList<int> DayItemID { get; private set; } 
        [ListSize(3)]
        public ReadonlyList<int> DayItemCount { get; private set; } 
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxDiamond = Table_Tamplet.Convert_Int(temp[__column++]);
                DayMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                DayDiamond = Table_Tamplet.Convert_Int(temp[__column++]);
                DayItemID=new ReadonlyList<int>(3);
                DayItemID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DayItemCount=new ReadonlyList<int>(3);
                DayItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DayItemID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DayItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DayItemID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DayItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ArenaRewardRecord)_this).Id;
        }
        static object get_MaxDiamond(IRecord _this)
        {
            return ((ArenaRewardRecord)_this).MaxDiamond;
        }
        static object get_DayMoney(IRecord _this)
        {
            return ((ArenaRewardRecord)_this).DayMoney;
        }
        static object get_DayDiamond(IRecord _this)
        {
            return ((ArenaRewardRecord)_this).DayDiamond;
        }
        static object get_DayItemID(IRecord _this)
        {
            return ((ArenaRewardRecord)_this).DayItemID;
        }
        static object get_DayItemCount(IRecord _this)
        {
            return ((ArenaRewardRecord)_this).DayItemCount;
        }
        static ArenaRewardRecord()
        {
            mField["Id"] = get_Id;
            mField["MaxDiamond"] = get_MaxDiamond;
            mField["DayMoney"] = get_DayMoney;
            mField["DayDiamond"] = get_DayDiamond;
            mField["DayItemID"] = get_DayItemID;
            mField["DayItemCount"] = get_DayItemCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ArenaLevelRecord :IRecord
    {
        public static string __TableName = "ArenaLevel.txt";
        public int Id { get; private set; }
        public int SuccessExp { get; private set; }
        public int SuccessMoney { get; private set; }
        public int SuccessItemID { get; private set; }
        public int SuccessCount { get; private set; }
        public int FailedExp { get; private set; }
        public int FailedMoney { get; private set; }
        public int FailedItemID { get; private set; }
        public int FailedCount { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                SuccessExp = Table_Tamplet.Convert_Int(temp[__column++]);
                SuccessMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                SuccessItemID = Table_Tamplet.Convert_Int(temp[__column++]);
                SuccessCount = Table_Tamplet.Convert_Int(temp[__column++]);
                FailedExp = Table_Tamplet.Convert_Int(temp[__column++]);
                FailedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                FailedItemID = Table_Tamplet.Convert_Int(temp[__column++]);
                FailedCount = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).Id;
        }
        static object get_SuccessExp(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).SuccessExp;
        }
        static object get_SuccessMoney(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).SuccessMoney;
        }
        static object get_SuccessItemID(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).SuccessItemID;
        }
        static object get_SuccessCount(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).SuccessCount;
        }
        static object get_FailedExp(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).FailedExp;
        }
        static object get_FailedMoney(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).FailedMoney;
        }
        static object get_FailedItemID(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).FailedItemID;
        }
        static object get_FailedCount(IRecord _this)
        {
            return ((ArenaLevelRecord)_this).FailedCount;
        }
        static ArenaLevelRecord()
        {
            mField["Id"] = get_Id;
            mField["SuccessExp"] = get_SuccessExp;
            mField["SuccessMoney"] = get_SuccessMoney;
            mField["SuccessItemID"] = get_SuccessItemID;
            mField["SuccessCount"] = get_SuccessCount;
            mField["FailedExp"] = get_FailedExp;
            mField["FailedMoney"] = get_FailedMoney;
            mField["FailedItemID"] = get_FailedItemID;
            mField["FailedCount"] = get_FailedCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class HonorRecord :IRecord
    {
        public static string __TableName = "Honor.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int TitleId { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int NextRank { get; private set; }
        public int NeedHonor { get; private set; }
        public int[] PropID = new int[10];
        public int[] PropValue = new int[10];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                TitleId = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                NextRank = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedHonor = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[9] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((HonorRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((HonorRecord)_this).Name;
        }
        static object get_TitleId(IRecord _this)
        {
            return ((HonorRecord)_this).TitleId;
        }
        static object get_Icon(IRecord _this)
        {
            return ((HonorRecord)_this).Icon;
        }
        static object get_NextRank(IRecord _this)
        {
            return ((HonorRecord)_this).NextRank;
        }
        static object get_NeedHonor(IRecord _this)
        {
            return ((HonorRecord)_this).NeedHonor;
        }
        static object get_PropID(IRecord _this)
        {
            return ((HonorRecord)_this).PropID;
        }
        static object get_PropValue(IRecord _this)
        {
            return ((HonorRecord)_this).PropValue;
        }
        static HonorRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["TitleId"] = get_TitleId;
            mField["Icon"] = get_Icon;
            mField["NextRank"] = get_NextRank;
            mField["NeedHonor"] = get_NeedHonor;
            mField["PropID"] = get_PropID;
            mField["PropValue"] = get_PropValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class JJCRootRecord :IRecord
    {
        public static string __TableName = "JJCRoot.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public int Career { get; private set; }
        public int CombatValue { get; private set; }
        public int EquipHand { get; private set; }
        public int EquipHead { get; private set; }
        public int EquipChest { get; private set; }
        public int EquipGlove { get; private set; }
        public int EquipTrouser { get; private set; }
        public int EquipShoes { get; private set; }
        public int EquipRange { get; private set; }
        public int EquipNecklace { get; private set; }
        public int EquipLevel { get; private set; }
        [TableBinding("ItemBase")]
        public int WingID { get; private set; }
        public int[] Skill = new int[4];
        public int Power { get; private set; }
        public int Agility { get; private set; }
        public int Intelligence { get; private set; }
        public int physical { get; private set; }
        public int AttackMin { get; private set; }
        public int AttackMax { get; private set; }
        public int LifeLimit { get; private set; }
        public int MagicLimit { get; private set; }
        public int PhysicsDefense { get; private set; }
        public int MagicDefense { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                Career = Table_Tamplet.Convert_Int(temp[__column++]);
                CombatValue = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipHand = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipHead = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipChest = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipGlove = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipTrouser = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipShoes = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipRange = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipNecklace = Table_Tamplet.Convert_Int(temp[__column++]);
                EquipLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                WingID = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Skill[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Power = Table_Tamplet.Convert_Int(temp[__column++]);
                Agility = Table_Tamplet.Convert_Int(temp[__column++]);
                Intelligence = Table_Tamplet.Convert_Int(temp[__column++]);
                physical = Table_Tamplet.Convert_Int(temp[__column++]);
                AttackMin = Table_Tamplet.Convert_Int(temp[__column++]);
                AttackMax = Table_Tamplet.Convert_Int(temp[__column++]);
                LifeLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                MagicLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                PhysicsDefense = Table_Tamplet.Convert_Int(temp[__column++]);
                MagicDefense = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((JJCRootRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((JJCRootRecord)_this).Name;
        }
        static object get_Level(IRecord _this)
        {
            return ((JJCRootRecord)_this).Level;
        }
        static object get_Career(IRecord _this)
        {
            return ((JJCRootRecord)_this).Career;
        }
        static object get_CombatValue(IRecord _this)
        {
            return ((JJCRootRecord)_this).CombatValue;
        }
        static object get_EquipHand(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipHand;
        }
        static object get_EquipHead(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipHead;
        }
        static object get_EquipChest(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipChest;
        }
        static object get_EquipGlove(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipGlove;
        }
        static object get_EquipTrouser(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipTrouser;
        }
        static object get_EquipShoes(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipShoes;
        }
        static object get_EquipRange(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipRange;
        }
        static object get_EquipNecklace(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipNecklace;
        }
        static object get_EquipLevel(IRecord _this)
        {
            return ((JJCRootRecord)_this).EquipLevel;
        }
        static object get_WingID(IRecord _this)
        {
            return ((JJCRootRecord)_this).WingID;
        }
        static object get_Skill(IRecord _this)
        {
            return ((JJCRootRecord)_this).Skill;
        }
        static object get_Power(IRecord _this)
        {
            return ((JJCRootRecord)_this).Power;
        }
        static object get_Agility(IRecord _this)
        {
            return ((JJCRootRecord)_this).Agility;
        }
        static object get_Intelligence(IRecord _this)
        {
            return ((JJCRootRecord)_this).Intelligence;
        }
        static object get_physical(IRecord _this)
        {
            return ((JJCRootRecord)_this).physical;
        }
        static object get_AttackMin(IRecord _this)
        {
            return ((JJCRootRecord)_this).AttackMin;
        }
        static object get_AttackMax(IRecord _this)
        {
            return ((JJCRootRecord)_this).AttackMax;
        }
        static object get_LifeLimit(IRecord _this)
        {
            return ((JJCRootRecord)_this).LifeLimit;
        }
        static object get_MagicLimit(IRecord _this)
        {
            return ((JJCRootRecord)_this).MagicLimit;
        }
        static object get_PhysicsDefense(IRecord _this)
        {
            return ((JJCRootRecord)_this).PhysicsDefense;
        }
        static object get_MagicDefense(IRecord _this)
        {
            return ((JJCRootRecord)_this).MagicDefense;
        }
        static JJCRootRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Level"] = get_Level;
            mField["Career"] = get_Career;
            mField["CombatValue"] = get_CombatValue;
            mField["EquipHand"] = get_EquipHand;
            mField["EquipHead"] = get_EquipHead;
            mField["EquipChest"] = get_EquipChest;
            mField["EquipGlove"] = get_EquipGlove;
            mField["EquipTrouser"] = get_EquipTrouser;
            mField["EquipShoes"] = get_EquipShoes;
            mField["EquipRange"] = get_EquipRange;
            mField["EquipNecklace"] = get_EquipNecklace;
            mField["EquipLevel"] = get_EquipLevel;
            mField["WingID"] = get_WingID;
            mField["Skill"] = get_Skill;
            mField["Power"] = get_Power;
            mField["Agility"] = get_Agility;
            mField["Intelligence"] = get_Intelligence;
            mField["physical"] = get_physical;
            mField["AttackMin"] = get_AttackMin;
            mField["AttackMax"] = get_AttackMax;
            mField["LifeLimit"] = get_LifeLimit;
            mField["MagicLimit"] = get_MagicLimit;
            mField["PhysicsDefense"] = get_PhysicsDefense;
            mField["MagicDefense"] = get_MagicDefense;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StatueRecord :IRecord
    {
        public static string __TableName = "Statue.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string PicPath { get; private set; }
        public int Level { get; private set; }
        public int MaxLevel { get; private set; }
        public int NextLevelID { get; private set; }
        public int Type { get; private set; }
        public int LevelUpExp { get; private set; }
        public int[] PropID = new int[3];
        public int[] propValue = new int[3];
        public int[] FuseID = new int[3];
        public int[] FuseValue = new int[3];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                PicPath = temp[__column++];
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                NextLevelID = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelUpExp = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                propValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                propValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                propValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuseID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuseValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuseID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuseValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuseID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuseValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StatueRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((StatueRecord)_this).Name;
        }
        static object get_PicPath(IRecord _this)
        {
            return ((StatueRecord)_this).PicPath;
        }
        static object get_Level(IRecord _this)
        {
            return ((StatueRecord)_this).Level;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((StatueRecord)_this).MaxLevel;
        }
        static object get_NextLevelID(IRecord _this)
        {
            return ((StatueRecord)_this).NextLevelID;
        }
        static object get_Type(IRecord _this)
        {
            return ((StatueRecord)_this).Type;
        }
        static object get_LevelUpExp(IRecord _this)
        {
            return ((StatueRecord)_this).LevelUpExp;
        }
        static object get_PropID(IRecord _this)
        {
            return ((StatueRecord)_this).PropID;
        }
        static object get_propValue(IRecord _this)
        {
            return ((StatueRecord)_this).propValue;
        }
        static object get_FuseID(IRecord _this)
        {
            return ((StatueRecord)_this).FuseID;
        }
        static object get_FuseValue(IRecord _this)
        {
            return ((StatueRecord)_this).FuseValue;
        }
        static StatueRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["PicPath"] = get_PicPath;
            mField["Level"] = get_Level;
            mField["MaxLevel"] = get_MaxLevel;
            mField["NextLevelID"] = get_NextLevelID;
            mField["Type"] = get_Type;
            mField["LevelUpExp"] = get_LevelUpExp;
            mField["PropID"] = get_PropID;
            mField["propValue"] = get_propValue;
            mField["FuseID"] = get_FuseID;
            mField["FuseValue"] = get_FuseValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipAdditional1Record :IRecord
    {
        public static string __TableName = "EquipAdditional1.txt";
        public int Id { get; private set; }
        public int AddPropArea { get; private set; }
        public int MaterialID { get; private set; }
        public int MaterialCount { get; private set; }
        public int Money { get; private set; }
        public int SmritiMoney { get; private set; }
        public int SmritiDiamond { get; private set; }
        public int CallBackItem { get; private set; }
        public int CallBackCount { get; private set; }
        public int MinSection { get; private set; }
        public int MaxSection { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                AddPropArea = Table_Tamplet.Convert_Int(temp[__column++]);
                MaterialID = Table_Tamplet.Convert_Int(temp[__column++]);
                MaterialCount = Table_Tamplet.Convert_Int(temp[__column++]);
                Money = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                SmritiDiamond = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackItem = Table_Tamplet.Convert_Int(temp[__column++]);
                CallBackCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MinSection = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxSection = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).Id;
        }
        static object get_AddPropArea(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).AddPropArea;
        }
        static object get_MaterialID(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).MaterialID;
        }
        static object get_MaterialCount(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).MaterialCount;
        }
        static object get_Money(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).Money;
        }
        static object get_SmritiMoney(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).SmritiMoney;
        }
        static object get_SmritiDiamond(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).SmritiDiamond;
        }
        static object get_CallBackItem(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).CallBackItem;
        }
        static object get_CallBackCount(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).CallBackCount;
        }
        static object get_MinSection(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).MinSection;
        }
        static object get_MaxSection(IRecord _this)
        {
            return ((EquipAdditional1Record)_this).MaxSection;
        }
        static EquipAdditional1Record()
        {
            mField["Id"] = get_Id;
            mField["AddPropArea"] = get_AddPropArea;
            mField["MaterialID"] = get_MaterialID;
            mField["MaterialCount"] = get_MaterialCount;
            mField["Money"] = get_Money;
            mField["SmritiMoney"] = get_SmritiMoney;
            mField["SmritiDiamond"] = get_SmritiDiamond;
            mField["CallBackItem"] = get_CallBackItem;
            mField["CallBackCount"] = get_CallBackCount;
            mField["MinSection"] = get_MinSection;
            mField["MaxSection"] = get_MaxSection;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TriggerAreaRecord :IRecord
    {
        public static string __TableName = "TriggerArea.txt";
        public int Id { get; private set; }
        public int SceneId { get; private set; }
        public float PosX { get;        set; }
        public float PosZ { get;        set; }
        public float Radius { get;        set; }
        public int ClientAnimation { get; private set; }
        public int OffLineTrigger { get; private set; }
        public int SoundID { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneId = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PosZ = Table_Tamplet.Convert_Float(temp[__column++]);
                Radius = Table_Tamplet.Convert_Float(temp[__column++]);
                ClientAnimation = Table_Tamplet.Convert_Int(temp[__column++]);
                OffLineTrigger = Table_Tamplet.Convert_Int(temp[__column++]);
                SoundID = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).Id;
        }
        static object get_SceneId(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).SceneId;
        }
        static object get_PosX(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).PosX;
        }
        static object get_PosZ(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).PosZ;
        }
        static object get_Radius(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).Radius;
        }
        static object get_ClientAnimation(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).ClientAnimation;
        }
        static object get_OffLineTrigger(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).OffLineTrigger;
        }
        static object get_SoundID(IRecord _this)
        {
            return ((TriggerAreaRecord)_this).SoundID;
        }
        static TriggerAreaRecord()
        {
            mField["Id"] = get_Id;
            mField["SceneId"] = get_SceneId;
            mField["PosX"] = get_PosX;
            mField["PosZ"] = get_PosZ;
            mField["Radius"] = get_Radius;
            mField["ClientAnimation"] = get_ClientAnimation;
            mField["OffLineTrigger"] = get_OffLineTrigger;
            mField["SoundID"] = get_SoundID;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GuildRecord :IRecord
    {
        public static string __TableName = "Guild.txt";
        public int Id { get; private set; }
        public int MaxCount { get; private set; }
        public int MaintainMoney { get; private set; }
        public int IsJoinCityWar { get; private set; }
        public int StoreParam { get; private set; }
        public int moneyCountLimit { get; private set; }
        public int LessGetGongji { get; private set; }
        public int LessNeedCount { get; private set; }
        public int LessUnionMoney { get; private set; }
        public int LessUnionDonation { get; private set; }
        public int MoreGetGongji { get; private set; }
        public int MoreNeedCount { get; private set; }
        public int MoreUnionMoney { get; private set; }
        public int MoreUnionDonation { get; private set; }
        public int DiamondGetGongji { get; private set; }
        public int DiaNeedCount { get; private set; }
        public int DiaUnionMoney { get; private set; }
        public int DiaUnionDonation { get; private set; }
        public int TaskRefresh { get; private set; }
        public int ConsumeUnionMoney { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MaintainMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                IsJoinCityWar = Table_Tamplet.Convert_Int(temp[__column++]);
                StoreParam = Table_Tamplet.Convert_Int(temp[__column++]);
                moneyCountLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                LessGetGongji = Table_Tamplet.Convert_Int(temp[__column++]);
                LessNeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                LessUnionMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                LessUnionDonation = Table_Tamplet.Convert_Int(temp[__column++]);
                MoreGetGongji = Table_Tamplet.Convert_Int(temp[__column++]);
                MoreNeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MoreUnionMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                MoreUnionDonation = Table_Tamplet.Convert_Int(temp[__column++]);
                DiamondGetGongji = Table_Tamplet.Convert_Int(temp[__column++]);
                DiaNeedCount = Table_Tamplet.Convert_Int(temp[__column++]);
                DiaUnionMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                DiaUnionDonation = Table_Tamplet.Convert_Int(temp[__column++]);
                TaskRefresh = Table_Tamplet.Convert_Int(temp[__column++]);
                ConsumeUnionMoney = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GuildRecord)_this).Id;
        }
        static object get_MaxCount(IRecord _this)
        {
            return ((GuildRecord)_this).MaxCount;
        }
        static object get_MaintainMoney(IRecord _this)
        {
            return ((GuildRecord)_this).MaintainMoney;
        }
        static object get_IsJoinCityWar(IRecord _this)
        {
            return ((GuildRecord)_this).IsJoinCityWar;
        }
        static object get_StoreParam(IRecord _this)
        {
            return ((GuildRecord)_this).StoreParam;
        }
        static object get_moneyCountLimit(IRecord _this)
        {
            return ((GuildRecord)_this).moneyCountLimit;
        }
        static object get_LessGetGongji(IRecord _this)
        {
            return ((GuildRecord)_this).LessGetGongji;
        }
        static object get_LessNeedCount(IRecord _this)
        {
            return ((GuildRecord)_this).LessNeedCount;
        }
        static object get_LessUnionMoney(IRecord _this)
        {
            return ((GuildRecord)_this).LessUnionMoney;
        }
        static object get_LessUnionDonation(IRecord _this)
        {
            return ((GuildRecord)_this).LessUnionDonation;
        }
        static object get_MoreGetGongji(IRecord _this)
        {
            return ((GuildRecord)_this).MoreGetGongji;
        }
        static object get_MoreNeedCount(IRecord _this)
        {
            return ((GuildRecord)_this).MoreNeedCount;
        }
        static object get_MoreUnionMoney(IRecord _this)
        {
            return ((GuildRecord)_this).MoreUnionMoney;
        }
        static object get_MoreUnionDonation(IRecord _this)
        {
            return ((GuildRecord)_this).MoreUnionDonation;
        }
        static object get_DiamondGetGongji(IRecord _this)
        {
            return ((GuildRecord)_this).DiamondGetGongji;
        }
        static object get_DiaNeedCount(IRecord _this)
        {
            return ((GuildRecord)_this).DiaNeedCount;
        }
        static object get_DiaUnionMoney(IRecord _this)
        {
            return ((GuildRecord)_this).DiaUnionMoney;
        }
        static object get_DiaUnionDonation(IRecord _this)
        {
            return ((GuildRecord)_this).DiaUnionDonation;
        }
        static object get_TaskRefresh(IRecord _this)
        {
            return ((GuildRecord)_this).TaskRefresh;
        }
        static object get_ConsumeUnionMoney(IRecord _this)
        {
            return ((GuildRecord)_this).ConsumeUnionMoney;
        }
        static GuildRecord()
        {
            mField["Id"] = get_Id;
            mField["MaxCount"] = get_MaxCount;
            mField["MaintainMoney"] = get_MaintainMoney;
            mField["IsJoinCityWar"] = get_IsJoinCityWar;
            mField["StoreParam"] = get_StoreParam;
            mField["moneyCountLimit"] = get_moneyCountLimit;
            mField["LessGetGongji"] = get_LessGetGongji;
            mField["LessNeedCount"] = get_LessNeedCount;
            mField["LessUnionMoney"] = get_LessUnionMoney;
            mField["LessUnionDonation"] = get_LessUnionDonation;
            mField["MoreGetGongji"] = get_MoreGetGongji;
            mField["MoreNeedCount"] = get_MoreNeedCount;
            mField["MoreUnionMoney"] = get_MoreUnionMoney;
            mField["MoreUnionDonation"] = get_MoreUnionDonation;
            mField["DiamondGetGongji"] = get_DiamondGetGongji;
            mField["DiaNeedCount"] = get_DiaNeedCount;
            mField["DiaUnionMoney"] = get_DiaUnionMoney;
            mField["DiaUnionDonation"] = get_DiaUnionDonation;
            mField["TaskRefresh"] = get_TaskRefresh;
            mField["ConsumeUnionMoney"] = get_ConsumeUnionMoney;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GuildBuffRecord :IRecord
    {
        public static string __TableName = "GuildBuff.txt";
        public int Id { get; private set; }
        public int BuffLevel { get; private set; }
        public int NextLevel { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public string Desc { get; private set; }
        public int NeedUnionLevel { get; private set; }
        public int BuffID { get; private set; }
        public int LevelLimit { get; private set; }
        public int UpConsumeGongji { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                NextLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = Table_Tamplet.Convert_String(temp[__column++]);
                NeedUnionLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffID = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                UpConsumeGongji = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GuildBuffRecord)_this).Id;
        }
        static object get_BuffLevel(IRecord _this)
        {
            return ((GuildBuffRecord)_this).BuffLevel;
        }
        static object get_NextLevel(IRecord _this)
        {
            return ((GuildBuffRecord)_this).NextLevel;
        }
        static object get_Icon(IRecord _this)
        {
            return ((GuildBuffRecord)_this).Icon;
        }
        static object get_Desc(IRecord _this)
        {
            return ((GuildBuffRecord)_this).Desc;
        }
        static object get_NeedUnionLevel(IRecord _this)
        {
            return ((GuildBuffRecord)_this).NeedUnionLevel;
        }
        static object get_BuffID(IRecord _this)
        {
            return ((GuildBuffRecord)_this).BuffID;
        }
        static object get_LevelLimit(IRecord _this)
        {
            return ((GuildBuffRecord)_this).LevelLimit;
        }
        static object get_UpConsumeGongji(IRecord _this)
        {
            return ((GuildBuffRecord)_this).UpConsumeGongji;
        }
        static GuildBuffRecord()
        {
            mField["Id"] = get_Id;
            mField["BuffLevel"] = get_BuffLevel;
            mField["NextLevel"] = get_NextLevel;
            mField["Icon"] = get_Icon;
            mField["Desc"] = get_Desc;
            mField["NeedUnionLevel"] = get_NeedUnionLevel;
            mField["BuffID"] = get_BuffID;
            mField["LevelLimit"] = get_LevelLimit;
            mField["UpConsumeGongji"] = get_UpConsumeGongji;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GuildBossRecord :IRecord
    {
        public static string __TableName = "GuildBoss.txt";
        public int Id { get; private set; }
        public int MonsterID { get; private set; }
        public int CycleZhangong { get; private set; }
        public int ActiveBossID { get; private set; }
        public int ChallengeCount { get; private set; }
        public int ChallengeTime { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                MonsterID = Table_Tamplet.Convert_Int(temp[__column++]);
                CycleZhangong = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveBossID = Table_Tamplet.Convert_Int(temp[__column++]);
                ChallengeCount = Table_Tamplet.Convert_Int(temp[__column++]);
                ChallengeTime = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GuildBossRecord)_this).Id;
        }
        static object get_MonsterID(IRecord _this)
        {
            return ((GuildBossRecord)_this).MonsterID;
        }
        static object get_CycleZhangong(IRecord _this)
        {
            return ((GuildBossRecord)_this).CycleZhangong;
        }
        static object get_ActiveBossID(IRecord _this)
        {
            return ((GuildBossRecord)_this).ActiveBossID;
        }
        static object get_ChallengeCount(IRecord _this)
        {
            return ((GuildBossRecord)_this).ChallengeCount;
        }
        static object get_ChallengeTime(IRecord _this)
        {
            return ((GuildBossRecord)_this).ChallengeTime;
        }
        static GuildBossRecord()
        {
            mField["Id"] = get_Id;
            mField["MonsterID"] = get_MonsterID;
            mField["CycleZhangong"] = get_CycleZhangong;
            mField["ActiveBossID"] = get_ActiveBossID;
            mField["ChallengeCount"] = get_ChallengeCount;
            mField["ChallengeTime"] = get_ChallengeTime;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GuildAccessRecord :IRecord
    {
        public static string __TableName = "GuildAccess.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int MaxCount { get; private set; }
        public int CanAddMember { get; private set; }
        public int CanLevelBuff { get; private set; }
        public int CanOperation { get; private set; }
        public int CanModifyNotice { get; private set; }
        public int CanModifyAttackCity { get; private set; }
        public int CanRebornGuard { get; private set; }
        public int MailId { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                MaxCount = Table_Tamplet.Convert_Int(temp[__column++]);
                CanAddMember = Table_Tamplet.Convert_Int(temp[__column++]);
                CanLevelBuff = Table_Tamplet.Convert_Int(temp[__column++]);
                CanOperation = Table_Tamplet.Convert_Int(temp[__column++]);
                CanModifyNotice = Table_Tamplet.Convert_Int(temp[__column++]);
                CanModifyAttackCity = Table_Tamplet.Convert_Int(temp[__column++]);
                CanRebornGuard = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GuildAccessRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((GuildAccessRecord)_this).Name;
        }
        static object get_MaxCount(IRecord _this)
        {
            return ((GuildAccessRecord)_this).MaxCount;
        }
        static object get_CanAddMember(IRecord _this)
        {
            return ((GuildAccessRecord)_this).CanAddMember;
        }
        static object get_CanLevelBuff(IRecord _this)
        {
            return ((GuildAccessRecord)_this).CanLevelBuff;
        }
        static object get_CanOperation(IRecord _this)
        {
            return ((GuildAccessRecord)_this).CanOperation;
        }
        static object get_CanModifyNotice(IRecord _this)
        {
            return ((GuildAccessRecord)_this).CanModifyNotice;
        }
        static object get_CanModifyAttackCity(IRecord _this)
        {
            return ((GuildAccessRecord)_this).CanModifyAttackCity;
        }
        static object get_CanRebornGuard(IRecord _this)
        {
            return ((GuildAccessRecord)_this).CanRebornGuard;
        }
        static object get_MailId(IRecord _this)
        {
            return ((GuildAccessRecord)_this).MailId;
        }
        static GuildAccessRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["MaxCount"] = get_MaxCount;
            mField["CanAddMember"] = get_CanAddMember;
            mField["CanLevelBuff"] = get_CanLevelBuff;
            mField["CanOperation"] = get_CanOperation;
            mField["CanModifyNotice"] = get_CanModifyNotice;
            mField["CanModifyAttackCity"] = get_CanModifyAttackCity;
            mField["CanRebornGuard"] = get_CanRebornGuard;
            mField["MailId"] = get_MailId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ExpInfoRecord :IRecord
    {
        public static string __TableName = "ExpInfo.txt";
        public int Id { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ExpInfoRecord)_this).Id;
        }
        static ExpInfoRecord()
        {
            mField["Id"] = get_Id;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GroupShopRecord :IRecord
    {
        public static string __TableName = "GroupShop.txt";
        public int Id { get; private set; }
        public int LuckNumber { get; private set; }
        public int SaleType { get; private set; }
        public int SaleCount { get; private set; }
        public int BuyLimit { get; private set; }
        public int ExistTime { get; private set; }
        public int LimitMinCount { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                LuckNumber = Table_Tamplet.Convert_Int(temp[__column++]);
                SaleType = Table_Tamplet.Convert_Int(temp[__column++]);
                SaleCount = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyLimit = Table_Tamplet.Convert_Int(temp[__column++]);
                ExistTime = Table_Tamplet.Convert_Int(temp[__column++]);
                LimitMinCount = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GroupShopRecord)_this).Id;
        }
        static object get_LuckNumber(IRecord _this)
        {
            return ((GroupShopRecord)_this).LuckNumber;
        }
        static object get_SaleType(IRecord _this)
        {
            return ((GroupShopRecord)_this).SaleType;
        }
        static object get_SaleCount(IRecord _this)
        {
            return ((GroupShopRecord)_this).SaleCount;
        }
        static object get_BuyLimit(IRecord _this)
        {
            return ((GroupShopRecord)_this).BuyLimit;
        }
        static object get_ExistTime(IRecord _this)
        {
            return ((GroupShopRecord)_this).ExistTime;
        }
        static object get_LimitMinCount(IRecord _this)
        {
            return ((GroupShopRecord)_this).LimitMinCount;
        }
        static GroupShopRecord()
        {
            mField["Id"] = get_Id;
            mField["LuckNumber"] = get_LuckNumber;
            mField["SaleType"] = get_SaleType;
            mField["SaleCount"] = get_SaleCount;
            mField["BuyLimit"] = get_BuyLimit;
            mField["ExistTime"] = get_ExistTime;
            mField["LimitMinCount"] = get_LimitMinCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PKModeRecord :IRecord
    {
        public static string __TableName = "PKMode.txt";
        public int Id { get; private set; }
        public string Mode { get; private set; }
        public int NomalTeam { get; private set; }
        public int NomalUnion { get; private set; }
        public int NomalState { get; private set; }
        public int RedTeam { get; private set; }
        public int RedUnion { get; private set; }
        public int RedState { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Mode = temp[__column++];
                NomalTeam = Table_Tamplet.Convert_Int(temp[__column++]);
                NomalUnion = Table_Tamplet.Convert_Int(temp[__column++]);
                NomalState = Table_Tamplet.Convert_Int(temp[__column++]);
                RedTeam = Table_Tamplet.Convert_Int(temp[__column++]);
                RedUnion = Table_Tamplet.Convert_Int(temp[__column++]);
                RedState = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PKModeRecord)_this).Id;
        }
        static object get_Mode(IRecord _this)
        {
            return ((PKModeRecord)_this).Mode;
        }
        static object get_NomalTeam(IRecord _this)
        {
            return ((PKModeRecord)_this).NomalTeam;
        }
        static object get_NomalUnion(IRecord _this)
        {
            return ((PKModeRecord)_this).NomalUnion;
        }
        static object get_NomalState(IRecord _this)
        {
            return ((PKModeRecord)_this).NomalState;
        }
        static object get_RedTeam(IRecord _this)
        {
            return ((PKModeRecord)_this).RedTeam;
        }
        static object get_RedUnion(IRecord _this)
        {
            return ((PKModeRecord)_this).RedUnion;
        }
        static object get_RedState(IRecord _this)
        {
            return ((PKModeRecord)_this).RedState;
        }
        static object get_Icon(IRecord _this)
        {
            return ((PKModeRecord)_this).Icon;
        }
        static PKModeRecord()
        {
            mField["Id"] = get_Id;
            mField["Mode"] = get_Mode;
            mField["NomalTeam"] = get_NomalTeam;
            mField["NomalUnion"] = get_NomalUnion;
            mField["NomalState"] = get_NomalState;
            mField["RedTeam"] = get_RedTeam;
            mField["RedUnion"] = get_RedUnion;
            mField["RedState"] = get_RedState;
            mField["Icon"] = get_Icon;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class forgedRecord :IRecord
    {
        public static string __TableName = "forged.txt";
        public int Id { get; private set; }
        public string FormulaName { get; private set; }
        public int Type { get; private set; }
        public int NeedLevel { get; private set; }
        public int NeedTime { get; private set; }
        public int ProductID { get; private set; }
        public int[] NeedItemID = new int[5];
        public int[] NeedItemCount = new int[5];
        public int[] NeedResID = new int[3];
        public int[] NeedResCount = new int[3];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                FormulaName = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedTime = Table_Tamplet.Convert_Int(temp[__column++]);
                ProductID = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((forgedRecord)_this).Id;
        }
        static object get_FormulaName(IRecord _this)
        {
            return ((forgedRecord)_this).FormulaName;
        }
        static object get_Type(IRecord _this)
        {
            return ((forgedRecord)_this).Type;
        }
        static object get_NeedLevel(IRecord _this)
        {
            return ((forgedRecord)_this).NeedLevel;
        }
        static object get_NeedTime(IRecord _this)
        {
            return ((forgedRecord)_this).NeedTime;
        }
        static object get_ProductID(IRecord _this)
        {
            return ((forgedRecord)_this).ProductID;
        }
        static object get_NeedItemID(IRecord _this)
        {
            return ((forgedRecord)_this).NeedItemID;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((forgedRecord)_this).NeedItemCount;
        }
        static object get_NeedResID(IRecord _this)
        {
            return ((forgedRecord)_this).NeedResID;
        }
        static object get_NeedResCount(IRecord _this)
        {
            return ((forgedRecord)_this).NeedResCount;
        }
        static forgedRecord()
        {
            mField["Id"] = get_Id;
            mField["FormulaName"] = get_FormulaName;
            mField["Type"] = get_Type;
            mField["NeedLevel"] = get_NeedLevel;
            mField["NeedTime"] = get_NeedTime;
            mField["ProductID"] = get_ProductID;
            mField["NeedItemID"] = get_NeedItemID;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["NeedResID"] = get_NeedResID;
            mField["NeedResCount"] = get_NeedResCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class EquipUpdateRecord :IRecord
    {
        public static string __TableName = "EquipUpdate.txt";
        public int Id { get; private set; }
        public int[] NeedItemID = new int[4];
        public int[] NeedItemCount = new int[4];
        public int[] NeedResID = new int[3];
        public int[] NeedResCount = new int[3];
        public int NeedEquipCount { get; private set; }
        public int SuccessGetExp { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedResCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedEquipCount = Table_Tamplet.Convert_Int(temp[__column++]);
                SuccessGetExp = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).Id;
        }
        static object get_NeedItemID(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).NeedItemID;
        }
        static object get_NeedItemCount(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).NeedItemCount;
        }
        static object get_NeedResID(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).NeedResID;
        }
        static object get_NeedResCount(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).NeedResCount;
        }
        static object get_NeedEquipCount(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).NeedEquipCount;
        }
        static object get_SuccessGetExp(IRecord _this)
        {
            return ((EquipUpdateRecord)_this).SuccessGetExp;
        }
        static EquipUpdateRecord()
        {
            mField["Id"] = get_Id;
            mField["NeedItemID"] = get_NeedItemID;
            mField["NeedItemCount"] = get_NeedItemCount;
            mField["NeedResID"] = get_NeedResID;
            mField["NeedResCount"] = get_NeedResCount;
            mField["NeedEquipCount"] = get_NeedEquipCount;
            mField["SuccessGetExp"] = get_SuccessGetExp;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GuildMissionRecord :IRecord
    {
        public static string __TableName = "GuildMission.txt";
        public int Id { get; private set; }
        public int ItemID { get; private set; }
        public int MinCount { get; private set; }
        public int MaxCount { get; private set; }
        public int MinLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public int GetGongJi { get; private set; }
        public int GetDonation { get; private set; }
        public int GetMoney { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemID = Table_Tamplet.Convert_Int(temp[__column++]);
                MinCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MinLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                GetGongJi = Table_Tamplet.Convert_Int(temp[__column++]);
                GetDonation = Table_Tamplet.Convert_Int(temp[__column++]);
                GetMoney = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GuildMissionRecord)_this).Id;
        }
        static object get_ItemID(IRecord _this)
        {
            return ((GuildMissionRecord)_this).ItemID;
        }
        static object get_MinCount(IRecord _this)
        {
            return ((GuildMissionRecord)_this).MinCount;
        }
        static object get_MaxCount(IRecord _this)
        {
            return ((GuildMissionRecord)_this).MaxCount;
        }
        static object get_MinLevel(IRecord _this)
        {
            return ((GuildMissionRecord)_this).MinLevel;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((GuildMissionRecord)_this).MaxLevel;
        }
        static object get_GetGongJi(IRecord _this)
        {
            return ((GuildMissionRecord)_this).GetGongJi;
        }
        static object get_GetDonation(IRecord _this)
        {
            return ((GuildMissionRecord)_this).GetDonation;
        }
        static object get_GetMoney(IRecord _this)
        {
            return ((GuildMissionRecord)_this).GetMoney;
        }
        static GuildMissionRecord()
        {
            mField["Id"] = get_Id;
            mField["ItemID"] = get_ItemID;
            mField["MinCount"] = get_MinCount;
            mField["MaxCount"] = get_MaxCount;
            mField["MinLevel"] = get_MinLevel;
            mField["MaxLevel"] = get_MaxLevel;
            mField["GetGongJi"] = get_GetGongJi;
            mField["GetDonation"] = get_GetDonation;
            mField["GetMoney"] = get_GetMoney;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class OrderFormRecord :IRecord
    {
        public static string __TableName = "OrderForm.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Quality { get; private set; }
        public int Item { get; private set; }
        public int RewardLess100 { get; private set; }
        public int RewardMore100 { get; private set; }
        public int ExtraDropID { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Quality = Table_Tamplet.Convert_Int(temp[__column++]);
                Item = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardLess100 = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardMore100 = Table_Tamplet.Convert_Int(temp[__column++]);
                ExtraDropID = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((OrderFormRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((OrderFormRecord)_this).Name;
        }
        static object get_Quality(IRecord _this)
        {
            return ((OrderFormRecord)_this).Quality;
        }
        static object get_Item(IRecord _this)
        {
            return ((OrderFormRecord)_this).Item;
        }
        static object get_RewardLess100(IRecord _this)
        {
            return ((OrderFormRecord)_this).RewardLess100;
        }
        static object get_RewardMore100(IRecord _this)
        {
            return ((OrderFormRecord)_this).RewardMore100;
        }
        static object get_ExtraDropID(IRecord _this)
        {
            return ((OrderFormRecord)_this).ExtraDropID;
        }
        static OrderFormRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Quality"] = get_Quality;
            mField["Item"] = get_Item;
            mField["RewardLess100"] = get_RewardLess100;
            mField["RewardMore100"] = get_RewardMore100;
            mField["ExtraDropID"] = get_ExtraDropID;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class OrderUpdateRecord :IRecord
    {
        public static string __TableName = "OrderUpdate.txt";
        public int Id { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((OrderUpdateRecord)_this).Id;
        }
        static OrderUpdateRecord()
        {
            mField["Id"] = get_Id;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TradeRecord :IRecord
    {
        public static string __TableName = "Trade.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        [TableBinding("ItemBase")]
        public int ItemID { get; private set; }
        public int Count { get; private set; }
        public int MoneyType { get; private set; }
        public int Price { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                ItemID = Table_Tamplet.Convert_Int(temp[__column++]);
                Count = Table_Tamplet.Convert_Int(temp[__column++]);
                MoneyType = Table_Tamplet.Convert_Int(temp[__column++]);
                Price = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TradeRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((TradeRecord)_this).Name;
        }
        static object get_ItemID(IRecord _this)
        {
            return ((TradeRecord)_this).ItemID;
        }
        static object get_Count(IRecord _this)
        {
            return ((TradeRecord)_this).Count;
        }
        static object get_MoneyType(IRecord _this)
        {
            return ((TradeRecord)_this).MoneyType;
        }
        static object get_Price(IRecord _this)
        {
            return ((TradeRecord)_this).Price;
        }
        static TradeRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["ItemID"] = get_ItemID;
            mField["Count"] = get_Count;
            mField["MoneyType"] = get_MoneyType;
            mField["Price"] = get_Price;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GemRecord :IRecord
    {
        public static string __TableName = "Gem.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public int Combination { get; private set; }
        public int Quality { get; private set; }
        public int InitExp { get; private set; }
        public int MaxLevel { get; private set; }
        public int[] ActiveCondition = new int[6];
        public int[] Param = new int[6];
        public int[] Prop1 = new int[6];
        public int[] PropValue1 = new int[6];
        public int[] Prop2 = new int[6];
        public int[] PropValue2 = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Combination = Table_Tamplet.Convert_Int(temp[__column++]);
                Quality = Table_Tamplet.Convert_Int(temp[__column++]);
                InitExp = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveCondition[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop1[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue1[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop2[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue2[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveCondition[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop1[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue1[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop2[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue2[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveCondition[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop1[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue1[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop2[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue2[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveCondition[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop1[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue1[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop2[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue2[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveCondition[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop1[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue1[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop2[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue2[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ActiveCondition[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop1[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue1[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Prop2[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue2[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GemRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((GemRecord)_this).Name;
        }
        static object get_Type(IRecord _this)
        {
            return ((GemRecord)_this).Type;
        }
        static object get_Combination(IRecord _this)
        {
            return ((GemRecord)_this).Combination;
        }
        static object get_Quality(IRecord _this)
        {
            return ((GemRecord)_this).Quality;
        }
        static object get_InitExp(IRecord _this)
        {
            return ((GemRecord)_this).InitExp;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((GemRecord)_this).MaxLevel;
        }
        static object get_ActiveCondition(IRecord _this)
        {
            return ((GemRecord)_this).ActiveCondition;
        }
        static object get_Param(IRecord _this)
        {
            return ((GemRecord)_this).Param;
        }
        static object get_Prop1(IRecord _this)
        {
            return ((GemRecord)_this).Prop1;
        }
        static object get_PropValue1(IRecord _this)
        {
            return ((GemRecord)_this).PropValue1;
        }
        static object get_Prop2(IRecord _this)
        {
            return ((GemRecord)_this).Prop2;
        }
        static object get_PropValue2(IRecord _this)
        {
            return ((GemRecord)_this).PropValue2;
        }
        static GemRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Type"] = get_Type;
            mField["Combination"] = get_Combination;
            mField["Quality"] = get_Quality;
            mField["InitExp"] = get_InitExp;
            mField["MaxLevel"] = get_MaxLevel;
            mField["ActiveCondition"] = get_ActiveCondition;
            mField["Param"] = get_Param;
            mField["Prop1"] = get_Prop1;
            mField["PropValue1"] = get_PropValue1;
            mField["Prop2"] = get_Prop2;
            mField["PropValue2"] = get_PropValue2;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GemGroupRecord :IRecord
    {
        public static string __TableName = "GemGroup.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int DiaID { get; private set; }
        public int CrystalID { get; private set; }
        public int AgateID { get; private set; }
        public int[] Towprop = new int[2];
        public int[] TowValue = new int[2];
        public int[] Threeprop = new int[2];
        public int[] ThreeValue = new int[2];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                DiaID = Table_Tamplet.Convert_Int(temp[__column++]);
                CrystalID = Table_Tamplet.Convert_Int(temp[__column++]);
                AgateID = Table_Tamplet.Convert_Int(temp[__column++]);
                Towprop[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                TowValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Towprop[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                TowValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Threeprop[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ThreeValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Threeprop[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ThreeValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GemGroupRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((GemGroupRecord)_this).Name;
        }
        static object get_DiaID(IRecord _this)
        {
            return ((GemGroupRecord)_this).DiaID;
        }
        static object get_CrystalID(IRecord _this)
        {
            return ((GemGroupRecord)_this).CrystalID;
        }
        static object get_AgateID(IRecord _this)
        {
            return ((GemGroupRecord)_this).AgateID;
        }
        static object get_Towprop(IRecord _this)
        {
            return ((GemGroupRecord)_this).Towprop;
        }
        static object get_TowValue(IRecord _this)
        {
            return ((GemGroupRecord)_this).TowValue;
        }
        static object get_Threeprop(IRecord _this)
        {
            return ((GemGroupRecord)_this).Threeprop;
        }
        static object get_ThreeValue(IRecord _this)
        {
            return ((GemGroupRecord)_this).ThreeValue;
        }
        static GemGroupRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["DiaID"] = get_DiaID;
            mField["CrystalID"] = get_CrystalID;
            mField["AgateID"] = get_AgateID;
            mField["Towprop"] = get_Towprop;
            mField["TowValue"] = get_TowValue;
            mField["Threeprop"] = get_Threeprop;
            mField["ThreeValue"] = get_ThreeValue;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SensitiveWordRecord :IRecord
    {
        public static string __TableName = "SensitiveWord.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SensitiveWordRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((SensitiveWordRecord)_this).Name;
        }
        static SensitiveWordRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GuidanceRecord :IRecord
    {
        public static string __TableName = "Guidance.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }
        public int TaskID { get; private set; }
        public int State { get; private set; }
        public int FlagPrepose { get; private set; }
        public int Career { get; private set; }
        public int Flag { get; private set; }
        public int UIID { get; private set; }
        public int FlagPreposeFalse { get; private set; }
        public int GuideRule { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                TaskID = Table_Tamplet.Convert_Int(temp[__column++]);
                State = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagPrepose = Table_Tamplet.Convert_Int(temp[__column++]);
                Career = Table_Tamplet.Convert_Int(temp[__column++]);
                Flag = Table_Tamplet.Convert_Int(temp[__column++]);
                UIID = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagPreposeFalse = Table_Tamplet.Convert_Int(temp[__column++]);
                GuideRule = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GuidanceRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((GuidanceRecord)_this).Name;
        }
        static object get_Level(IRecord _this)
        {
            return ((GuidanceRecord)_this).Level;
        }
        static object get_TaskID(IRecord _this)
        {
            return ((GuidanceRecord)_this).TaskID;
        }
        static object get_State(IRecord _this)
        {
            return ((GuidanceRecord)_this).State;
        }
        static object get_FlagPrepose(IRecord _this)
        {
            return ((GuidanceRecord)_this).FlagPrepose;
        }
        static object get_Career(IRecord _this)
        {
            return ((GuidanceRecord)_this).Career;
        }
        static object get_Flag(IRecord _this)
        {
            return ((GuidanceRecord)_this).Flag;
        }
        static object get_UIID(IRecord _this)
        {
            return ((GuidanceRecord)_this).UIID;
        }
        static object get_FlagPreposeFalse(IRecord _this)
        {
            return ((GuidanceRecord)_this).FlagPreposeFalse;
        }
        static object get_GuideRule(IRecord _this)
        {
            return ((GuidanceRecord)_this).GuideRule;
        }
        static GuidanceRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Level"] = get_Level;
            mField["TaskID"] = get_TaskID;
            mField["State"] = get_State;
            mField["FlagPrepose"] = get_FlagPrepose;
            mField["Career"] = get_Career;
            mField["Flag"] = get_Flag;
            mField["UIID"] = get_UIID;
            mField["FlagPreposeFalse"] = get_FlagPreposeFalse;
            mField["GuideRule"] = get_GuideRule;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MapTransferRecord :IRecord
    {
        public static string __TableName = "MapTransfer.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        [TableBinding("Scene")]
        public int SceneID { get; private set; }
        public float PosX { get;        set; }
        public float PosZ { get;        set; }
        public int DisplayColor { get; private set; }
        public string ShowChar { get; private set; }
        public int DisplaySort { get; private set; }
        public int NpcID { get; private set; }
        public float OffsetX { get;        set; }
        public float OffsetY { get;        set; }
        public int Mark { get; private set; }
        [TableBinding("Icon")]
        public int LiveIcon { get; private set; }
        [TableBinding("Icon")]
        public int LiveIcon2 { get; private set; }
        [TableBinding("Icon")]
        public int DeadIcon { get; private set; }
        public int Camp { get; private set; }
        public int PicWidth { get; private set; }
        public int PIcHight { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneID = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PosZ = Table_Tamplet.Convert_Float(temp[__column++]);
                DisplayColor = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowChar = temp[__column++];
                DisplaySort = Table_Tamplet.Convert_Int(temp[__column++]);
                NpcID = Table_Tamplet.Convert_Int(temp[__column++]);
                OffsetX = Table_Tamplet.Convert_Float(temp[__column++]);
                OffsetY = Table_Tamplet.Convert_Float(temp[__column++]);
                Mark = Table_Tamplet.Convert_Int(temp[__column++]);
                LiveIcon = Table_Tamplet.Convert_Int(temp[__column++]);
                LiveIcon2 = Table_Tamplet.Convert_Int(temp[__column++]);
                DeadIcon = Table_Tamplet.Convert_Int(temp[__column++]);
                Camp = Table_Tamplet.Convert_Int(temp[__column++]);
                PicWidth = Table_Tamplet.Convert_Int(temp[__column++]);
                PIcHight = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MapTransferRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((MapTransferRecord)_this).Name;
        }
        static object get_Type(IRecord _this)
        {
            return ((MapTransferRecord)_this).Type;
        }
        static object get_SceneID(IRecord _this)
        {
            return ((MapTransferRecord)_this).SceneID;
        }
        static object get_PosX(IRecord _this)
        {
            return ((MapTransferRecord)_this).PosX;
        }
        static object get_PosZ(IRecord _this)
        {
            return ((MapTransferRecord)_this).PosZ;
        }
        static object get_DisplayColor(IRecord _this)
        {
            return ((MapTransferRecord)_this).DisplayColor;
        }
        static object get_ShowChar(IRecord _this)
        {
            return ((MapTransferRecord)_this).ShowChar;
        }
        static object get_DisplaySort(IRecord _this)
        {
            return ((MapTransferRecord)_this).DisplaySort;
        }
        static object get_NpcID(IRecord _this)
        {
            return ((MapTransferRecord)_this).NpcID;
        }
        static object get_OffsetX(IRecord _this)
        {
            return ((MapTransferRecord)_this).OffsetX;
        }
        static object get_OffsetY(IRecord _this)
        {
            return ((MapTransferRecord)_this).OffsetY;
        }
        static object get_Mark(IRecord _this)
        {
            return ((MapTransferRecord)_this).Mark;
        }
        static object get_LiveIcon(IRecord _this)
        {
            return ((MapTransferRecord)_this).LiveIcon;
        }
        static object get_LiveIcon2(IRecord _this)
        {
            return ((MapTransferRecord)_this).LiveIcon2;
        }
        static object get_DeadIcon(IRecord _this)
        {
            return ((MapTransferRecord)_this).DeadIcon;
        }
        static object get_Camp(IRecord _this)
        {
            return ((MapTransferRecord)_this).Camp;
        }
        static object get_PicWidth(IRecord _this)
        {
            return ((MapTransferRecord)_this).PicWidth;
        }
        static object get_PIcHight(IRecord _this)
        {
            return ((MapTransferRecord)_this).PIcHight;
        }
        static MapTransferRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Type"] = get_Type;
            mField["SceneID"] = get_SceneID;
            mField["PosX"] = get_PosX;
            mField["PosZ"] = get_PosZ;
            mField["DisplayColor"] = get_DisplayColor;
            mField["ShowChar"] = get_ShowChar;
            mField["DisplaySort"] = get_DisplaySort;
            mField["NpcID"] = get_NpcID;
            mField["OffsetX"] = get_OffsetX;
            mField["OffsetY"] = get_OffsetY;
            mField["Mark"] = get_Mark;
            mField["LiveIcon"] = get_LiveIcon;
            mField["LiveIcon2"] = get_LiveIcon2;
            mField["DeadIcon"] = get_DeadIcon;
            mField["Camp"] = get_Camp;
            mField["PicWidth"] = get_PicWidth;
            mField["PIcHight"] = get_PIcHight;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SceneEffectRecord :IRecord
    {
        public static string __TableName = "SceneEffect.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public int LastTime { get; private set; }
        public int Zoom { get; private set; }
        public float CameraShockRange { get;        set; }
        public int CameraShockDelay { get; private set; }
        [TableBinding("Scene")]
        public int SceneID { get; private set; }
        public float PosX { get;        set; }
        public float PosY { get;        set; }
        public float PosZ { get;        set; }
        public int TriggerMark { get; private set; }
        public int StopMark { get; private set; }
        public int Sound { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Path = temp[__column++];
                LastTime = Table_Tamplet.Convert_Int(temp[__column++]);
                Zoom = Table_Tamplet.Convert_Int(temp[__column++]);
                CameraShockRange = Table_Tamplet.Convert_Float(temp[__column++]);
                CameraShockDelay = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneID = Table_Tamplet.Convert_Int(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PosY = Table_Tamplet.Convert_Float(temp[__column++]);
                PosZ = Table_Tamplet.Convert_Float(temp[__column++]);
                TriggerMark = Table_Tamplet.Convert_Int(temp[__column++]);
                StopMark = Table_Tamplet.Convert_Int(temp[__column++]);
                Sound = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SceneEffectRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((SceneEffectRecord)_this).Name;
        }
        static object get_Path(IRecord _this)
        {
            return ((SceneEffectRecord)_this).Path;
        }
        static object get_LastTime(IRecord _this)
        {
            return ((SceneEffectRecord)_this).LastTime;
        }
        static object get_Zoom(IRecord _this)
        {
            return ((SceneEffectRecord)_this).Zoom;
        }
        static object get_CameraShockRange(IRecord _this)
        {
            return ((SceneEffectRecord)_this).CameraShockRange;
        }
        static object get_CameraShockDelay(IRecord _this)
        {
            return ((SceneEffectRecord)_this).CameraShockDelay;
        }
        static object get_SceneID(IRecord _this)
        {
            return ((SceneEffectRecord)_this).SceneID;
        }
        static object get_PosX(IRecord _this)
        {
            return ((SceneEffectRecord)_this).PosX;
        }
        static object get_PosY(IRecord _this)
        {
            return ((SceneEffectRecord)_this).PosY;
        }
        static object get_PosZ(IRecord _this)
        {
            return ((SceneEffectRecord)_this).PosZ;
        }
        static object get_TriggerMark(IRecord _this)
        {
            return ((SceneEffectRecord)_this).TriggerMark;
        }
        static object get_StopMark(IRecord _this)
        {
            return ((SceneEffectRecord)_this).StopMark;
        }
        static object get_Sound(IRecord _this)
        {
            return ((SceneEffectRecord)_this).Sound;
        }
        static SceneEffectRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Path"] = get_Path;
            mField["LastTime"] = get_LastTime;
            mField["Zoom"] = get_Zoom;
            mField["CameraShockRange"] = get_CameraShockRange;
            mField["CameraShockDelay"] = get_CameraShockDelay;
            mField["SceneID"] = get_SceneID;
            mField["PosX"] = get_PosX;
            mField["PosY"] = get_PosY;
            mField["PosZ"] = get_PosZ;
            mField["TriggerMark"] = get_TriggerMark;
            mField["StopMark"] = get_StopMark;
            mField["Sound"] = get_Sound;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PVPBattleRecord :IRecord
    {
        public static string __TableName = "PVPBattle.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int[] Fuben = new int[8];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Fuben[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Fuben[7] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PVPBattleRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((PVPBattleRecord)_this).Name;
        }
        static object get_Fuben(IRecord _this)
        {
            return ((PVPBattleRecord)_this).Fuben;
        }
        static PVPBattleRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Fuben"] = get_Fuben;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StepByStepRecord :IRecord
    {
        public static string __TableName = "StepByStep.txt";
        public int Id { get; private set; }
        public string Desc { get; private set; }
        public float FontX { get;        set; }
        public float FontY { get;        set; }
        public float PosX { get;        set; }
        public float PoxY { get;        set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public float IconX { get;        set; }
        public float IconY { get;        set; }
        public int NextStep { get; private set; }
        public int Color { get; private set; }
        public int Transparency { get; private set; }
        public float SeeSizeX { get;        set; }
        public float SeeSizeY { get;        set; }
        public float SeePosX { get;        set; }
        public float SeePosY { get;        set; }
        public int IsShowPointer { get; private set; }
        public float PointerX { get;        set; }
        public float PointerY { get;        set; }
        public int Rotation { get; private set; }
        public int IsSkip { get; private set; }
        public int Music { get; private set; }
        public int PosMark { get; private set; }
        public int NextIndexID { get; private set; }
        public int DelayTime { get; private set; }
        public int CenterPoint { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                FontX = Table_Tamplet.Convert_Float(temp[__column++]);
                FontY = Table_Tamplet.Convert_Float(temp[__column++]);
                PosX = Table_Tamplet.Convert_Float(temp[__column++]);
                PoxY = Table_Tamplet.Convert_Float(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                IconX = Table_Tamplet.Convert_Float(temp[__column++]);
                IconY = Table_Tamplet.Convert_Float(temp[__column++]);
                NextStep = Table_Tamplet.Convert_Int(temp[__column++]);
                Color = Table_Tamplet.Convert_Int(temp[__column++]);
                Transparency = Table_Tamplet.Convert_Int(temp[__column++]);
                SeeSizeX = Table_Tamplet.Convert_Float(temp[__column++]);
                SeeSizeY = Table_Tamplet.Convert_Float(temp[__column++]);
                SeePosX = Table_Tamplet.Convert_Float(temp[__column++]);
                SeePosY = Table_Tamplet.Convert_Float(temp[__column++]);
                IsShowPointer = Table_Tamplet.Convert_Int(temp[__column++]);
                PointerX = Table_Tamplet.Convert_Float(temp[__column++]);
                PointerY = Table_Tamplet.Convert_Float(temp[__column++]);
                Rotation = Table_Tamplet.Convert_Int(temp[__column++]);
                IsSkip = Table_Tamplet.Convert_Int(temp[__column++]);
                Music = Table_Tamplet.Convert_Int(temp[__column++]);
                PosMark = Table_Tamplet.Convert_Int(temp[__column++]);
                NextIndexID = Table_Tamplet.Convert_Int(temp[__column++]);
                DelayTime = Table_Tamplet.Convert_Int(temp[__column++]);
                CenterPoint = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StepByStepRecord)_this).Id;
        }
        static object get_Desc(IRecord _this)
        {
            return ((StepByStepRecord)_this).Desc;
        }
        static object get_FontX(IRecord _this)
        {
            return ((StepByStepRecord)_this).FontX;
        }
        static object get_FontY(IRecord _this)
        {
            return ((StepByStepRecord)_this).FontY;
        }
        static object get_PosX(IRecord _this)
        {
            return ((StepByStepRecord)_this).PosX;
        }
        static object get_PoxY(IRecord _this)
        {
            return ((StepByStepRecord)_this).PoxY;
        }
        static object get_Icon(IRecord _this)
        {
            return ((StepByStepRecord)_this).Icon;
        }
        static object get_IconX(IRecord _this)
        {
            return ((StepByStepRecord)_this).IconX;
        }
        static object get_IconY(IRecord _this)
        {
            return ((StepByStepRecord)_this).IconY;
        }
        static object get_NextStep(IRecord _this)
        {
            return ((StepByStepRecord)_this).NextStep;
        }
        static object get_Color(IRecord _this)
        {
            return ((StepByStepRecord)_this).Color;
        }
        static object get_Transparency(IRecord _this)
        {
            return ((StepByStepRecord)_this).Transparency;
        }
        static object get_SeeSizeX(IRecord _this)
        {
            return ((StepByStepRecord)_this).SeeSizeX;
        }
        static object get_SeeSizeY(IRecord _this)
        {
            return ((StepByStepRecord)_this).SeeSizeY;
        }
        static object get_SeePosX(IRecord _this)
        {
            return ((StepByStepRecord)_this).SeePosX;
        }
        static object get_SeePosY(IRecord _this)
        {
            return ((StepByStepRecord)_this).SeePosY;
        }
        static object get_IsShowPointer(IRecord _this)
        {
            return ((StepByStepRecord)_this).IsShowPointer;
        }
        static object get_PointerX(IRecord _this)
        {
            return ((StepByStepRecord)_this).PointerX;
        }
        static object get_PointerY(IRecord _this)
        {
            return ((StepByStepRecord)_this).PointerY;
        }
        static object get_Rotation(IRecord _this)
        {
            return ((StepByStepRecord)_this).Rotation;
        }
        static object get_IsSkip(IRecord _this)
        {
            return ((StepByStepRecord)_this).IsSkip;
        }
        static object get_Music(IRecord _this)
        {
            return ((StepByStepRecord)_this).Music;
        }
        static object get_PosMark(IRecord _this)
        {
            return ((StepByStepRecord)_this).PosMark;
        }
        static object get_NextIndexID(IRecord _this)
        {
            return ((StepByStepRecord)_this).NextIndexID;
        }
        static object get_DelayTime(IRecord _this)
        {
            return ((StepByStepRecord)_this).DelayTime;
        }
        static object get_CenterPoint(IRecord _this)
        {
            return ((StepByStepRecord)_this).CenterPoint;
        }
        static StepByStepRecord()
        {
            mField["Id"] = get_Id;
            mField["Desc"] = get_Desc;
            mField["FontX"] = get_FontX;
            mField["FontY"] = get_FontY;
            mField["PosX"] = get_PosX;
            mField["PoxY"] = get_PoxY;
            mField["Icon"] = get_Icon;
            mField["IconX"] = get_IconX;
            mField["IconY"] = get_IconY;
            mField["NextStep"] = get_NextStep;
            mField["Color"] = get_Color;
            mField["Transparency"] = get_Transparency;
            mField["SeeSizeX"] = get_SeeSizeX;
            mField["SeeSizeY"] = get_SeeSizeY;
            mField["SeePosX"] = get_SeePosX;
            mField["SeePosY"] = get_SeePosY;
            mField["IsShowPointer"] = get_IsShowPointer;
            mField["PointerX"] = get_PointerX;
            mField["PointerY"] = get_PointerY;
            mField["Rotation"] = get_Rotation;
            mField["IsSkip"] = get_IsSkip;
            mField["Music"] = get_Music;
            mField["PosMark"] = get_PosMark;
            mField["NextIndexID"] = get_NextIndexID;
            mField["DelayTime"] = get_DelayTime;
            mField["CenterPoint"] = get_CenterPoint;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class WorldBOSSRecord :IRecord
    {
        public static string __TableName = "WorldBOSS.txt";
        public int Id { get; private set; }
        public int Type { get; private set; }
        public string Name { get; private set; }
        [TableBinding("SceneNpc")]
        public int SceneNpc { get; private set; }
        public int AdviceFighting { get; private set; }
        public string RefleshTime { get; private set; }
        [ListSize(4)]
        public ReadonlyList<int> DropItem { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> DropCount { get; private set; } 
        public int IsDisplayClient { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                SceneNpc = Table_Tamplet.Convert_Int(temp[__column++]);
                AdviceFighting = Table_Tamplet.Convert_Int(temp[__column++]);
                RefleshTime = temp[__column++];
                DropItem=new ReadonlyList<int>(4);
                DropItem[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropCount=new ReadonlyList<int>(4);
                DropCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropItem[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                DropCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                IsDisplayClient = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).Type;
        }
        static object get_Name(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).Name;
        }
        static object get_SceneNpc(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).SceneNpc;
        }
        static object get_AdviceFighting(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).AdviceFighting;
        }
        static object get_RefleshTime(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).RefleshTime;
        }
        static object get_DropItem(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).DropItem;
        }
        static object get_DropCount(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).DropCount;
        }
        static object get_IsDisplayClient(IRecord _this)
        {
            return ((WorldBOSSRecord)_this).IsDisplayClient;
        }
        static WorldBOSSRecord()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["Name"] = get_Name;
            mField["SceneNpc"] = get_SceneNpc;
            mField["AdviceFighting"] = get_AdviceFighting;
            mField["RefleshTime"] = get_RefleshTime;
            mField["DropItem"] = get_DropItem;
            mField["DropCount"] = get_DropCount;
            mField["IsDisplayClient"] = get_IsDisplayClient;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class PKValueRecord :IRecord
    {
        public static string __TableName = "PKValue.txt";
        public int Id { get; private set; }
        public int NameColor { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                NameColor = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((PKValueRecord)_this).Id;
        }
        static object get_NameColor(IRecord _this)
        {
            return ((PKValueRecord)_this).NameColor;
        }
        static PKValueRecord()
        {
            mField["Id"] = get_Id;
            mField["NameColor"] = get_NameColor;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class TransmigrationRecord :IRecord
    {
        public static string __TableName = "Transmigration.txt";
        public int Id { get; private set; }
        public int ConditionCount { get; private set; }
        public int TransLevel { get; private set; }
        public int PropPoint { get; private set; }
        public int NeedMoney { get; private set; }
        public int NeedDust { get; private set; }
        public int AttackAdd { get; private set; }
        public int PhyDefAdd { get; private set; }
        public int MagicDefAdd { get; private set; }
        public int HitAdd { get; private set; }
        public int DodgeAdd { get; private set; }
        public int LifeAdd { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                ConditionCount = Table_Tamplet.Convert_Int(temp[__column++]);
                TransLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                PropPoint = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedMoney = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedDust = Table_Tamplet.Convert_Int(temp[__column++]);
                AttackAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                PhyDefAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                MagicDefAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                HitAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                DodgeAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                LifeAdd = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((TransmigrationRecord)_this).Id;
        }
        static object get_ConditionCount(IRecord _this)
        {
            return ((TransmigrationRecord)_this).ConditionCount;
        }
        static object get_TransLevel(IRecord _this)
        {
            return ((TransmigrationRecord)_this).TransLevel;
        }
        static object get_PropPoint(IRecord _this)
        {
            return ((TransmigrationRecord)_this).PropPoint;
        }
        static object get_NeedMoney(IRecord _this)
        {
            return ((TransmigrationRecord)_this).NeedMoney;
        }
        static object get_NeedDust(IRecord _this)
        {
            return ((TransmigrationRecord)_this).NeedDust;
        }
        static object get_AttackAdd(IRecord _this)
        {
            return ((TransmigrationRecord)_this).AttackAdd;
        }
        static object get_PhyDefAdd(IRecord _this)
        {
            return ((TransmigrationRecord)_this).PhyDefAdd;
        }
        static object get_MagicDefAdd(IRecord _this)
        {
            return ((TransmigrationRecord)_this).MagicDefAdd;
        }
        static object get_HitAdd(IRecord _this)
        {
            return ((TransmigrationRecord)_this).HitAdd;
        }
        static object get_DodgeAdd(IRecord _this)
        {
            return ((TransmigrationRecord)_this).DodgeAdd;
        }
        static object get_LifeAdd(IRecord _this)
        {
            return ((TransmigrationRecord)_this).LifeAdd;
        }
        static TransmigrationRecord()
        {
            mField["Id"] = get_Id;
            mField["ConditionCount"] = get_ConditionCount;
            mField["TransLevel"] = get_TransLevel;
            mField["PropPoint"] = get_PropPoint;
            mField["NeedMoney"] = get_NeedMoney;
            mField["NeedDust"] = get_NeedDust;
            mField["AttackAdd"] = get_AttackAdd;
            mField["PhyDefAdd"] = get_PhyDefAdd;
            mField["MagicDefAdd"] = get_MagicDefAdd;
            mField["HitAdd"] = get_HitAdd;
            mField["DodgeAdd"] = get_DodgeAdd;
            mField["LifeAdd"] = get_LifeAdd;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AttachPointRecord :IRecord
    {
        public static string __TableName = "AttachPoint.txt";
        public int Id { get; private set; }
        public string Path { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Path = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AttachPointRecord)_this).Id;
        }
        static object get_Path(IRecord _this)
        {
            return ((AttachPointRecord)_this).Path;
        }
        static AttachPointRecord()
        {
            mField["Id"] = get_Id;
            mField["Path"] = get_Path;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class FubenInfoRecord :IRecord
    {
        public static string __TableName = "FubenInfo.txt";
        public int Id { get; private set; }
        public int Type { get; private set; }
        [TableBinding("Dictionary")]
        public int Desc { get; private set; }
        public int[] Param = new int[6];
        public int Stage { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Stage = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((FubenInfoRecord)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((FubenInfoRecord)_this).Type;
        }
        static object get_Desc(IRecord _this)
        {
            return ((FubenInfoRecord)_this).Desc;
        }
        static object get_Param(IRecord _this)
        {
            return ((FubenInfoRecord)_this).Param;
        }
        static object get_Stage(IRecord _this)
        {
            return ((FubenInfoRecord)_this).Stage;
        }
        static FubenInfoRecord()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["Desc"] = get_Desc;
            mField["Param"] = get_Param;
            mField["Stage"] = get_Stage;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class FubenLogicRecord :IRecord
    {
        public static string __TableName = "FubenLogic.txt";
        public int Id { get; private set; }
        public int[] FubenInfo = new int[4];
        public int[] FubenParam1 = new int[4];
        public int[] FubenParam2 = new int[4];
        public int Hang1PosX { get; private set; }
        public int Hang1PosZ { get; private set; }
        public int[] AdvanceOpTYpe = new int[2];
        public int[] AdvanceParam1 = new int[2];
        public int[] AdvanceParam2 = new int[2];
        public int DelayToNextState { get; private set; }
        public int DelayHang { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenInfo[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam1[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam2[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenInfo[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam1[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam2[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenInfo[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam1[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam2[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenInfo[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam1[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenParam2[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Hang1PosX = Table_Tamplet.Convert_Int(temp[__column++]);
                Hang1PosZ = Table_Tamplet.Convert_Int(temp[__column++]);
                AdvanceOpTYpe[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AdvanceParam1[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AdvanceParam2[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                AdvanceOpTYpe[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AdvanceParam1[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                AdvanceParam2[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DelayToNextState = Table_Tamplet.Convert_Int(temp[__column++]);
                DelayHang = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((FubenLogicRecord)_this).Id;
        }
        static object get_FubenInfo(IRecord _this)
        {
            return ((FubenLogicRecord)_this).FubenInfo;
        }
        static object get_FubenParam1(IRecord _this)
        {
            return ((FubenLogicRecord)_this).FubenParam1;
        }
        static object get_FubenParam2(IRecord _this)
        {
            return ((FubenLogicRecord)_this).FubenParam2;
        }
        static object get_Hang1PosX(IRecord _this)
        {
            return ((FubenLogicRecord)_this).Hang1PosX;
        }
        static object get_Hang1PosZ(IRecord _this)
        {
            return ((FubenLogicRecord)_this).Hang1PosZ;
        }
        static object get_AdvanceOpTYpe(IRecord _this)
        {
            return ((FubenLogicRecord)_this).AdvanceOpTYpe;
        }
        static object get_AdvanceParam1(IRecord _this)
        {
            return ((FubenLogicRecord)_this).AdvanceParam1;
        }
        static object get_AdvanceParam2(IRecord _this)
        {
            return ((FubenLogicRecord)_this).AdvanceParam2;
        }
        static object get_DelayToNextState(IRecord _this)
        {
            return ((FubenLogicRecord)_this).DelayToNextState;
        }
        static object get_DelayHang(IRecord _this)
        {
            return ((FubenLogicRecord)_this).DelayHang;
        }
        static FubenLogicRecord()
        {
            mField["Id"] = get_Id;
            mField["FubenInfo"] = get_FubenInfo;
            mField["FubenParam1"] = get_FubenParam1;
            mField["FubenParam2"] = get_FubenParam2;
            mField["Hang1PosX"] = get_Hang1PosX;
            mField["Hang1PosZ"] = get_Hang1PosZ;
            mField["AdvanceOpTYpe"] = get_AdvanceOpTYpe;
            mField["AdvanceParam1"] = get_AdvanceParam1;
            mField["AdvanceParam2"] = get_AdvanceParam2;
            mField["DelayToNextState"] = get_DelayToNextState;
            mField["DelayHang"] = get_DelayHang;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class FaceRecord :IRecord
    {
        public static string __TableName = "Face.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((FaceRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((FaceRecord)_this).Name;
        }
        static FaceRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ServerNameRecord :IRecord
    {
        public static string __TableName = "ServerName.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ServerNameRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ServerNameRecord)_this).Name;
        }
        static ServerNameRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class LoadingTestRecord :IRecord
    {
        public static string __TableName = "LoadingTest.txt";
        public int Id { get; private set; }
        public int MinLevel { get; private set; }
        public int MaxLevel { get; private set; }
        public int FlagTrue { get; private set; }
        public int FlagFalse { get; private set; }
        [TableBinding("Dictionary")]
        public int DictIndex { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                MinLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagTrue = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagFalse = Table_Tamplet.Convert_Int(temp[__column++]);
                DictIndex = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((LoadingTestRecord)_this).Id;
        }
        static object get_MinLevel(IRecord _this)
        {
            return ((LoadingTestRecord)_this).MinLevel;
        }
        static object get_MaxLevel(IRecord _this)
        {
            return ((LoadingTestRecord)_this).MaxLevel;
        }
        static object get_FlagTrue(IRecord _this)
        {
            return ((LoadingTestRecord)_this).FlagTrue;
        }
        static object get_FlagFalse(IRecord _this)
        {
            return ((LoadingTestRecord)_this).FlagFalse;
        }
        static object get_DictIndex(IRecord _this)
        {
            return ((LoadingTestRecord)_this).DictIndex;
        }
        static LoadingTestRecord()
        {
            mField["Id"] = get_Id;
            mField["MinLevel"] = get_MinLevel;
            mField["MaxLevel"] = get_MaxLevel;
            mField["FlagTrue"] = get_FlagTrue;
            mField["FlagFalse"] = get_FlagFalse;
            mField["DictIndex"] = get_DictIndex;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GetMissionInfoRecord :IRecord
    {
        public static string __TableName = "GetMissionInfo.txt";
        public int Id { get; private set; }
        public int[] HomeExp = new int[3];
        public int PetGetExp { get; private set; }
        public int RandomShowCount { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                HomeExp[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                HomeExp[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                HomeExp[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PetGetExp = Table_Tamplet.Convert_Int(temp[__column++]);
                RandomShowCount = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GetMissionInfoRecord)_this).Id;
        }
        static object get_HomeExp(IRecord _this)
        {
            return ((GetMissionInfoRecord)_this).HomeExp;
        }
        static object get_PetGetExp(IRecord _this)
        {
            return ((GetMissionInfoRecord)_this).PetGetExp;
        }
        static object get_RandomShowCount(IRecord _this)
        {
            return ((GetMissionInfoRecord)_this).RandomShowCount;
        }
        static GetMissionInfoRecord()
        {
            mField["Id"] = get_Id;
            mField["HomeExp"] = get_HomeExp;
            mField["PetGetExp"] = get_PetGetExp;
            mField["RandomShowCount"] = get_RandomShowCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MissionConditionInfoRecord :IRecord
    {
        public static string __TableName = "MissionConditionInfo.txt";
        public int Id { get; private set; }
        public string RoleName { get; private set; }
        public int Type { get; private set; }
        public int Param { get; private set; }
        public string[] ParamName = new string[10];
        public int[] ParamIcon = new int[10];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                RoleName = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Param = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[0]  = temp[__column++];
                ParamIcon[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[1]  = temp[__column++];
                ParamIcon[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[2]  = temp[__column++];
                ParamIcon[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[3]  = temp[__column++];
                ParamIcon[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[4]  = temp[__column++];
                ParamIcon[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[5]  = temp[__column++];
                ParamIcon[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[6]  = temp[__column++];
                ParamIcon[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[7]  = temp[__column++];
                ParamIcon[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[8]  = temp[__column++];
                ParamIcon[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                ParamName[9]  = temp[__column++];
                ParamIcon[9] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MissionConditionInfoRecord)_this).Id;
        }
        static object get_RoleName(IRecord _this)
        {
            return ((MissionConditionInfoRecord)_this).RoleName;
        }
        static object get_Type(IRecord _this)
        {
            return ((MissionConditionInfoRecord)_this).Type;
        }
        static object get_Param(IRecord _this)
        {
            return ((MissionConditionInfoRecord)_this).Param;
        }
        static object get_ParamName(IRecord _this)
        {
            return ((MissionConditionInfoRecord)_this).ParamName;
        }
        static object get_ParamIcon(IRecord _this)
        {
            return ((MissionConditionInfoRecord)_this).ParamIcon;
        }
        static MissionConditionInfoRecord()
        {
            mField["Id"] = get_Id;
            mField["RoleName"] = get_RoleName;
            mField["Type"] = get_Type;
            mField["Param"] = get_Param;
            mField["ParamName"] = get_ParamName;
            mField["ParamIcon"] = get_ParamIcon;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GetMissionRewardRecord :IRecord
    {
        public static string __TableName = "GetMissionReward.txt";
        public int Id { get; private set; }
        public int Level { get; private set; }
        public int Type { get; private set; }
        public int Quality { get; private set; }
        public int PersonCount { get; private set; }
        public int RewardType { get; private set; }
        public int RewardCount { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Level = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Quality = Table_Tamplet.Convert_Int(temp[__column++]);
                PersonCount = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardType = Table_Tamplet.Convert_Int(temp[__column++]);
                RewardCount = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).Id;
        }
        static object get_Level(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).Level;
        }
        static object get_Type(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).Type;
        }
        static object get_Quality(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).Quality;
        }
        static object get_PersonCount(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).PersonCount;
        }
        static object get_RewardType(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).RewardType;
        }
        static object get_RewardCount(IRecord _this)
        {
            return ((GetMissionRewardRecord)_this).RewardCount;
        }
        static GetMissionRewardRecord()
        {
            mField["Id"] = get_Id;
            mField["Level"] = get_Level;
            mField["Type"] = get_Type;
            mField["Quality"] = get_Quality;
            mField["PersonCount"] = get_PersonCount;
            mField["RewardType"] = get_RewardType;
            mField["RewardCount"] = get_RewardCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GetMissionIconRecord :IRecord
    {
        public static string __TableName = "GetMissionIcon.txt";
        public int Id { get; private set; }
        [TableBinding("Icon")]
        public int IconId { get; private set; }
        public int Priority { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                IconId = Table_Tamplet.Convert_Int(temp[__column++]);
                Priority = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GetMissionIconRecord)_this).Id;
        }
        static object get_IconId(IRecord _this)
        {
            return ((GetMissionIconRecord)_this).IconId;
        }
        static object get_Priority(IRecord _this)
        {
            return ((GetMissionIconRecord)_this).Priority;
        }
        static GetMissionIconRecord()
        {
            mField["Id"] = get_Id;
            mField["IconId"] = get_IconId;
            mField["Priority"] = get_Priority;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class SubjectRecord :IRecord
    {
        public static string __TableName = "Subject.txt";
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string RightKey { get; private set; }
        public string[] Wrong = new string[4];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Title = temp[__column++];
                RightKey = temp[__column++];
                Wrong[0]  = temp[__column++];
                Wrong[1]  = temp[__column++];
                Wrong[2]  = temp[__column++];
                Wrong[3]  = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((SubjectRecord)_this).Id;
        }
        static object get_Title(IRecord _this)
        {
            return ((SubjectRecord)_this).Title;
        }
        static object get_RightKey(IRecord _this)
        {
            return ((SubjectRecord)_this).RightKey;
        }
        static object get_Wrong(IRecord _this)
        {
            return ((SubjectRecord)_this).Wrong;
        }
        static SubjectRecord()
        {
            mField["Id"] = get_Id;
            mField["Title"] = get_Title;
            mField["RightKey"] = get_RightKey;
            mField["Wrong"] = get_Wrong;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ItemGetInfoRecord :IRecord
    {
        public static string __TableName = "ItemGetInfo.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int UIName { get; private set; }
        public int[] Param = new int[4];
        public int IsShow { get; private set; }
        public string Desc { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                UIName = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                IsShow = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).Name;
        }
        static object get_Icon(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).Icon;
        }
        static object get_UIName(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).UIName;
        }
        static object get_Param(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).Param;
        }
        static object get_IsShow(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).IsShow;
        }
        static object get_Desc(IRecord _this)
        {
            return ((ItemGetInfoRecord)_this).Desc;
        }
        static ItemGetInfoRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Icon"] = get_Icon;
            mField["UIName"] = get_UIName;
            mField["Param"] = get_Param;
            mField["IsShow"] = get_IsShow;
            mField["Desc"] = get_Desc;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DynamicActivityRecord :IRecord
    {
        public static string __TableName = "DynamicActivity.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public int[] FuBenID = new int[7];
        public int FrontID { get; private set; }
        public int Sort { get; private set; }
        public int SurfaceInfo { get; private set; }
        public int UIID { get; private set; }
        [TableBinding("Icon")]
        public int DisplayPic { get; private set; }
        public int SufaceTab { get; private set; }
        public int OpenCondition { get; private set; }
        public string NoOpenStr { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                FrontID = Table_Tamplet.Convert_Int(temp[__column++]);
                Sort = Table_Tamplet.Convert_Int(temp[__column++]);
                SurfaceInfo = Table_Tamplet.Convert_Int(temp[__column++]);
                UIID = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayPic = Table_Tamplet.Convert_Int(temp[__column++]);
                SufaceTab = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenCondition = Table_Tamplet.Convert_Int(temp[__column++]);
                NoOpenStr = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).Name;
        }
        static object get_Type(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).Type;
        }
        static object get_FuBenID(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).FuBenID;
        }
        static object get_FrontID(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).FrontID;
        }
        static object get_Sort(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).Sort;
        }
        static object get_SurfaceInfo(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).SurfaceInfo;
        }
        static object get_UIID(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).UIID;
        }
        static object get_DisplayPic(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).DisplayPic;
        }
        static object get_SufaceTab(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).SufaceTab;
        }
        static object get_OpenCondition(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).OpenCondition;
        }
        static object get_NoOpenStr(IRecord _this)
        {
            return ((DynamicActivityRecord)_this).NoOpenStr;
        }
        static DynamicActivityRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Type"] = get_Type;
            mField["FuBenID"] = get_FuBenID;
            mField["FrontID"] = get_FrontID;
            mField["Sort"] = get_Sort;
            mField["SurfaceInfo"] = get_SurfaceInfo;
            mField["UIID"] = get_UIID;
            mField["DisplayPic"] = get_DisplayPic;
            mField["SufaceTab"] = get_SufaceTab;
            mField["OpenCondition"] = get_OpenCondition;
            mField["NoOpenStr"] = get_NoOpenStr;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class CompensationRecord :IRecord
    {
        public static string __TableName = "Compensation.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Type { get; private set; }
        public int Sign { get; private set; }
        public int ExtraData { get; private set; }
        public int MaxCount { get; private set; }
        public int ExpType { get; private set; }
        public int UnitExp { get; private set; }
        public int GoldType { get; private set; }
        public int UnitGold { get; private set; }
        public int[] ItemYype = new int[4];
        public int[] UnitItem = new int[4];
        public int[] ItemCount = new int[4];
        public int ConditionId { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                Sign = Table_Tamplet.Convert_Int(temp[__column++]);
                ExtraData = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxCount = Table_Tamplet.Convert_Int(temp[__column++]);
                ExpType = Table_Tamplet.Convert_Int(temp[__column++]);
                UnitExp = Table_Tamplet.Convert_Int(temp[__column++]);
                GoldType = Table_Tamplet.Convert_Int(temp[__column++]);
                UnitGold = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemYype[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                UnitItem[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemYype[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                UnitItem[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemYype[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                UnitItem[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemYype[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                UnitItem[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ConditionId = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((CompensationRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((CompensationRecord)_this).Name;
        }
        static object get_Type(IRecord _this)
        {
            return ((CompensationRecord)_this).Type;
        }
        static object get_Sign(IRecord _this)
        {
            return ((CompensationRecord)_this).Sign;
        }
        static object get_ExtraData(IRecord _this)
        {
            return ((CompensationRecord)_this).ExtraData;
        }
        static object get_MaxCount(IRecord _this)
        {
            return ((CompensationRecord)_this).MaxCount;
        }
        static object get_ExpType(IRecord _this)
        {
            return ((CompensationRecord)_this).ExpType;
        }
        static object get_UnitExp(IRecord _this)
        {
            return ((CompensationRecord)_this).UnitExp;
        }
        static object get_GoldType(IRecord _this)
        {
            return ((CompensationRecord)_this).GoldType;
        }
        static object get_UnitGold(IRecord _this)
        {
            return ((CompensationRecord)_this).UnitGold;
        }
        static object get_ItemYype(IRecord _this)
        {
            return ((CompensationRecord)_this).ItemYype;
        }
        static object get_UnitItem(IRecord _this)
        {
            return ((CompensationRecord)_this).UnitItem;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((CompensationRecord)_this).ItemCount;
        }
        static object get_ConditionId(IRecord _this)
        {
            return ((CompensationRecord)_this).ConditionId;
        }
        static CompensationRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Type"] = get_Type;
            mField["Sign"] = get_Sign;
            mField["ExtraData"] = get_ExtraData;
            mField["MaxCount"] = get_MaxCount;
            mField["ExpType"] = get_ExpType;
            mField["UnitExp"] = get_UnitExp;
            mField["GoldType"] = get_GoldType;
            mField["UnitGold"] = get_UnitGold;
            mField["ItemYype"] = get_ItemYype;
            mField["UnitItem"] = get_UnitItem;
            mField["ItemCount"] = get_ItemCount;
            mField["ConditionId"] = get_ConditionId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class CityTalkRecord :IRecord
    {
        public static string __TableName = "CityTalk.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int IsParent { get; private set; }
        public int Param { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                IsParent = Table_Tamplet.Convert_Int(temp[__column++]);
                Param = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((CityTalkRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((CityTalkRecord)_this).Name;
        }
        static object get_IsParent(IRecord _this)
        {
            return ((CityTalkRecord)_this).IsParent;
        }
        static object get_Param(IRecord _this)
        {
            return ((CityTalkRecord)_this).Param;
        }
        static CityTalkRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["IsParent"] = get_IsParent;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DailyActivityRecord :IRecord
    {
        public static string __TableName = "DailyActivity.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public int Type { get; private set; }
        public int DetailType { get; private set; }
        public int OpenCondition { get; private set; }
        public int WillOpenCondition { get; private set; }
        public string Desc { get; private set; }
        public int UIId { get; private set; }
        public int UITab { get; private set; }
        public int ActivityValue { get; private set; }
        public int ActivityCount { get; private set; }
        public int DisplayCount { get; private set; }
        public int SortPriority { get; private set; }
        public int FinishCanJoin { get; private set; }
        public int[] CommonParam = new int[2];
        public int ExDataId { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                DetailType = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenCondition = Table_Tamplet.Convert_Int(temp[__column++]);
                WillOpenCondition = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                UIId = Table_Tamplet.Convert_Int(temp[__column++]);
                UITab = Table_Tamplet.Convert_Int(temp[__column++]);
                ActivityValue = Table_Tamplet.Convert_Int(temp[__column++]);
                ActivityCount = Table_Tamplet.Convert_Int(temp[__column++]);
                DisplayCount = Table_Tamplet.Convert_Int(temp[__column++]);
                SortPriority = Table_Tamplet.Convert_Int(temp[__column++]);
                FinishCanJoin = Table_Tamplet.Convert_Int(temp[__column++]);
                CommonParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                CommonParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ExDataId = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DailyActivityRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((DailyActivityRecord)_this).Name;
        }
        static object get_Icon(IRecord _this)
        {
            return ((DailyActivityRecord)_this).Icon;
        }
        static object get_Type(IRecord _this)
        {
            return ((DailyActivityRecord)_this).Type;
        }
        static object get_DetailType(IRecord _this)
        {
            return ((DailyActivityRecord)_this).DetailType;
        }
        static object get_OpenCondition(IRecord _this)
        {
            return ((DailyActivityRecord)_this).OpenCondition;
        }
        static object get_WillOpenCondition(IRecord _this)
        {
            return ((DailyActivityRecord)_this).WillOpenCondition;
        }
        static object get_Desc(IRecord _this)
        {
            return ((DailyActivityRecord)_this).Desc;
        }
        static object get_UIId(IRecord _this)
        {
            return ((DailyActivityRecord)_this).UIId;
        }
        static object get_UITab(IRecord _this)
        {
            return ((DailyActivityRecord)_this).UITab;
        }
        static object get_ActivityValue(IRecord _this)
        {
            return ((DailyActivityRecord)_this).ActivityValue;
        }
        static object get_ActivityCount(IRecord _this)
        {
            return ((DailyActivityRecord)_this).ActivityCount;
        }
        static object get_DisplayCount(IRecord _this)
        {
            return ((DailyActivityRecord)_this).DisplayCount;
        }
        static object get_SortPriority(IRecord _this)
        {
            return ((DailyActivityRecord)_this).SortPriority;
        }
        static object get_FinishCanJoin(IRecord _this)
        {
            return ((DailyActivityRecord)_this).FinishCanJoin;
        }
        static object get_CommonParam(IRecord _this)
        {
            return ((DailyActivityRecord)_this).CommonParam;
        }
        static object get_ExDataId(IRecord _this)
        {
            return ((DailyActivityRecord)_this).ExDataId;
        }
        static DailyActivityRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Icon"] = get_Icon;
            mField["Type"] = get_Type;
            mField["DetailType"] = get_DetailType;
            mField["OpenCondition"] = get_OpenCondition;
            mField["WillOpenCondition"] = get_WillOpenCondition;
            mField["Desc"] = get_Desc;
            mField["UIId"] = get_UIId;
            mField["UITab"] = get_UITab;
            mField["ActivityValue"] = get_ActivityValue;
            mField["ActivityCount"] = get_ActivityCount;
            mField["DisplayCount"] = get_DisplayCount;
            mField["SortPriority"] = get_SortPriority;
            mField["FinishCanJoin"] = get_FinishCanJoin;
            mField["CommonParam"] = get_CommonParam;
            mField["ExDataId"] = get_ExDataId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class RechargeRecord :IRecord
    {
        public static string __TableName = "Recharge.txt";
        public int Id { get; private set; }
        public string Platfrom { get; private set; }
        public string Name { get; private set; }
        public int ItemId { get; private set; }
        public int Visible { get; private set; }
        public int Type { get; private set; }
        public string ExDesc { get; private set; }
        public string Desc { get; private set; }
        public int Price { get; private set; }
        public int Diamond { get; private set; }
        public int VipExp { get; private set; }
        public int ExTimes { get; private set; }
        public int ExdataId { get; private set; }
        public int ExDiamond { get; private set; }
        public int NormalDiamond { get; private set; }
        public int[] Param = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Platfrom = temp[__column++];
                Name = temp[__column++];
                ItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                Visible = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
                ExDesc = temp[__column++];
                Desc = temp[__column++];
                Price = Table_Tamplet.Convert_Int(temp[__column++]);
                Diamond = Table_Tamplet.Convert_Int(temp[__column++]);
                VipExp = Table_Tamplet.Convert_Int(temp[__column++]);
                ExTimes = Table_Tamplet.Convert_Int(temp[__column++]);
                ExdataId = Table_Tamplet.Convert_Int(temp[__column++]);
                ExDiamond = Table_Tamplet.Convert_Int(temp[__column++]);
                NormalDiamond = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((RechargeRecord)_this).Id;
        }
        static object get_Platfrom(IRecord _this)
        {
            return ((RechargeRecord)_this).Platfrom;
        }
        static object get_Name(IRecord _this)
        {
            return ((RechargeRecord)_this).Name;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((RechargeRecord)_this).ItemId;
        }
        static object get_Visible(IRecord _this)
        {
            return ((RechargeRecord)_this).Visible;
        }
        static object get_Type(IRecord _this)
        {
            return ((RechargeRecord)_this).Type;
        }
        static object get_ExDesc(IRecord _this)
        {
            return ((RechargeRecord)_this).ExDesc;
        }
        static object get_Desc(IRecord _this)
        {
            return ((RechargeRecord)_this).Desc;
        }
        static object get_Price(IRecord _this)
        {
            return ((RechargeRecord)_this).Price;
        }
        static object get_Diamond(IRecord _this)
        {
            return ((RechargeRecord)_this).Diamond;
        }
        static object get_VipExp(IRecord _this)
        {
            return ((RechargeRecord)_this).VipExp;
        }
        static object get_ExTimes(IRecord _this)
        {
            return ((RechargeRecord)_this).ExTimes;
        }
        static object get_ExdataId(IRecord _this)
        {
            return ((RechargeRecord)_this).ExdataId;
        }
        static object get_ExDiamond(IRecord _this)
        {
            return ((RechargeRecord)_this).ExDiamond;
        }
        static object get_NormalDiamond(IRecord _this)
        {
            return ((RechargeRecord)_this).NormalDiamond;
        }
        static object get_Param(IRecord _this)
        {
            return ((RechargeRecord)_this).Param;
        }
        static RechargeRecord()
        {
            mField["Id"] = get_Id;
            mField["Platfrom"] = get_Platfrom;
            mField["Name"] = get_Name;
            mField["ItemId"] = get_ItemId;
            mField["Visible"] = get_Visible;
            mField["Type"] = get_Type;
            mField["ExDesc"] = get_ExDesc;
            mField["Desc"] = get_Desc;
            mField["Price"] = get_Price;
            mField["Diamond"] = get_Diamond;
            mField["VipExp"] = get_VipExp;
            mField["ExTimes"] = get_ExTimes;
            mField["ExdataId"] = get_ExdataId;
            mField["ExDiamond"] = get_ExDiamond;
            mField["NormalDiamond"] = get_NormalDiamond;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class RewardInfoRecord :IRecord
    {
        public static string __TableName = "RewardInfo.txt";
        public int Id { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public string SurfaceName { get; private set; }
        public string GetLabel { get; private set; }
        public int IsEffect { get; private set; }
        public int UIId { get; private set; }
        public int UIParam { get; private set; }
        public int Sort { get; private set; }
        public int ConditionId { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                SurfaceName = temp[__column++];
                GetLabel = temp[__column++];
                IsEffect = Table_Tamplet.Convert_Int(temp[__column++]);
                UIId = Table_Tamplet.Convert_Int(temp[__column++]);
                UIParam = Table_Tamplet.Convert_Int(temp[__column++]);
                Sort = Table_Tamplet.Convert_Int(temp[__column++]);
                ConditionId = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((RewardInfoRecord)_this).Id;
        }
        static object get_Icon(IRecord _this)
        {
            return ((RewardInfoRecord)_this).Icon;
        }
        static object get_SurfaceName(IRecord _this)
        {
            return ((RewardInfoRecord)_this).SurfaceName;
        }
        static object get_GetLabel(IRecord _this)
        {
            return ((RewardInfoRecord)_this).GetLabel;
        }
        static object get_IsEffect(IRecord _this)
        {
            return ((RewardInfoRecord)_this).IsEffect;
        }
        static object get_UIId(IRecord _this)
        {
            return ((RewardInfoRecord)_this).UIId;
        }
        static object get_UIParam(IRecord _this)
        {
            return ((RewardInfoRecord)_this).UIParam;
        }
        static object get_Sort(IRecord _this)
        {
            return ((RewardInfoRecord)_this).Sort;
        }
        static object get_ConditionId(IRecord _this)
        {
            return ((RewardInfoRecord)_this).ConditionId;
        }
        static RewardInfoRecord()
        {
            mField["Id"] = get_Id;
            mField["Icon"] = get_Icon;
            mField["SurfaceName"] = get_SurfaceName;
            mField["GetLabel"] = get_GetLabel;
            mField["IsEffect"] = get_IsEffect;
            mField["UIId"] = get_UIId;
            mField["UIParam"] = get_UIParam;
            mField["Sort"] = get_Sort;
            mField["ConditionId"] = get_ConditionId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class NameTitleRecord :IRecord
    {
        public static string __TableName = "NameTitle.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int Pos { get; private set; }
        public string GainDesc { get; private set; }
        public int PeriodEndSort { get; private set; }
        public int PropAdd { get; private set; }
        public int[] PicId = new int[3];
        public string[] MyLabel = new string[3];
        public int[] MyFont = new int[3];
        public int[] MyFontColorA = new int[3];
        public int[] MyFontColorB = new int[3];
        public int[] EffectType = new int[3];
        public int[] StrokeColor = new int[3];
        public int[] PropId = new int[9];
        public int[] PropValue = new int[9];
        public int ValidityPeriod { get; private set; }
        public int ConditionID { get; private set; }
        public int FlagId { get; private set; }
        public int FrontId { get; private set; }
        public int PostId { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Pos = Table_Tamplet.Convert_Int(temp[__column++]);
                GainDesc = temp[__column++];
                PeriodEndSort = Table_Tamplet.Convert_Int(temp[__column++]);
                PropAdd = Table_Tamplet.Convert_Int(temp[__column++]);
                PicId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyLabel[0]  = temp[__column++];
                MyFont[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyFontColorA[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyFontColorB[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectType[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                StrokeColor[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PicId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyLabel[1]  = temp[__column++];
                MyFont[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyFontColorA[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyFontColorB[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectType[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                StrokeColor[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PicId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyLabel[2]  = temp[__column++];
                MyFont[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyFontColorA[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                MyFontColorB[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                EffectType[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                StrokeColor[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropId[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                PropValue[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                ValidityPeriod = Table_Tamplet.Convert_Int(temp[__column++]);
                ConditionID = Table_Tamplet.Convert_Int(temp[__column++]);
                FlagId = Table_Tamplet.Convert_Int(temp[__column++]);
                FrontId = Table_Tamplet.Convert_Int(temp[__column++]);
                PostId = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((NameTitleRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((NameTitleRecord)_this).Name;
        }
        static object get_Pos(IRecord _this)
        {
            return ((NameTitleRecord)_this).Pos;
        }
        static object get_GainDesc(IRecord _this)
        {
            return ((NameTitleRecord)_this).GainDesc;
        }
        static object get_PeriodEndSort(IRecord _this)
        {
            return ((NameTitleRecord)_this).PeriodEndSort;
        }
        static object get_PropAdd(IRecord _this)
        {
            return ((NameTitleRecord)_this).PropAdd;
        }
        static object get_PicId(IRecord _this)
        {
            return ((NameTitleRecord)_this).PicId;
        }
        static object get_MyLabel(IRecord _this)
        {
            return ((NameTitleRecord)_this).MyLabel;
        }
        static object get_MyFont(IRecord _this)
        {
            return ((NameTitleRecord)_this).MyFont;
        }
        static object get_MyFontColorA(IRecord _this)
        {
            return ((NameTitleRecord)_this).MyFontColorA;
        }
        static object get_MyFontColorB(IRecord _this)
        {
            return ((NameTitleRecord)_this).MyFontColorB;
        }
        static object get_EffectType(IRecord _this)
        {
            return ((NameTitleRecord)_this).EffectType;
        }
        static object get_StrokeColor(IRecord _this)
        {
            return ((NameTitleRecord)_this).StrokeColor;
        }
        static object get_PropId(IRecord _this)
        {
            return ((NameTitleRecord)_this).PropId;
        }
        static object get_PropValue(IRecord _this)
        {
            return ((NameTitleRecord)_this).PropValue;
        }
        static object get_ValidityPeriod(IRecord _this)
        {
            return ((NameTitleRecord)_this).ValidityPeriod;
        }
        static object get_ConditionID(IRecord _this)
        {
            return ((NameTitleRecord)_this).ConditionID;
        }
        static object get_FlagId(IRecord _this)
        {
            return ((NameTitleRecord)_this).FlagId;
        }
        static object get_FrontId(IRecord _this)
        {
            return ((NameTitleRecord)_this).FrontId;
        }
        static object get_PostId(IRecord _this)
        {
            return ((NameTitleRecord)_this).PostId;
        }
        static NameTitleRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Pos"] = get_Pos;
            mField["GainDesc"] = get_GainDesc;
            mField["PeriodEndSort"] = get_PeriodEndSort;
            mField["PropAdd"] = get_PropAdd;
            mField["PicId"] = get_PicId;
            mField["MyLabel"] = get_MyLabel;
            mField["MyFont"] = get_MyFont;
            mField["MyFontColorA"] = get_MyFontColorA;
            mField["MyFontColorB"] = get_MyFontColorB;
            mField["EffectType"] = get_EffectType;
            mField["StrokeColor"] = get_StrokeColor;
            mField["PropId"] = get_PropId;
            mField["PropValue"] = get_PropValue;
            mField["ValidityPeriod"] = get_ValidityPeriod;
            mField["ConditionID"] = get_ConditionID;
            mField["FlagId"] = get_FlagId;
            mField["FrontId"] = get_FrontId;
            mField["PostId"] = get_PostId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class VIPRecord :IRecord
    {
        public static string __TableName = "VIP.txt";
        public int Id { get; private set; }
        public int NeedVipExp { get; private set; }
        public int[] PackItemParam = new int[3];
        public int GetItem { get; private set; }
        public int GetBuff { get; private set; }
        public int GetTitle { get; private set; }
        public int PackageId { get; private set; }
        public string Desc { get; private set; }
        public int[] BuyItemId = new int[10];
        public int[] BuyItemCount = new int[10];
        public int PKBuyCount { get; private set; }
        public int PKChallengeCD { get; private set; }
        public int Depot { get; private set; }
        public int Repair { get; private set; }
        public int WingAdvanced { get; private set; }
        public int Muse2Reward { get; private set; }
        public int Muse4Reward { get; private set; }
        public int StatueAddCount { get; private set; }
        public int FarmAddRefleshCount { get; private set; }
        public int SceneBossTrans { get; private set; }
        public int EnhanceRatio { get; private set; }
        public int PlotFubenResetCount { get; private set; }
        public int AreaLimitTrans { get; private set; }
        public int DevilBuyCount { get; private set; }
        public int BloodBuyCount { get; private set; }
        public int VipGoldTemple { get; private set; }
        public int VipBossTemple { get; private set; }
        public int SailScanCount { get; private set; }
        public int WishPoolFilterNum { get; private set; }
        public int SentTimes { get; private set; }
        public int PetIslandBuyTimes { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                NeedVipExp = Table_Tamplet.Convert_Int(temp[__column++]);
                PackItemParam[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                PackItemParam[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                PackItemParam[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                GetItem = Table_Tamplet.Convert_Int(temp[__column++]);
                GetBuff = Table_Tamplet.Convert_Int(temp[__column++]);
                GetTitle = Table_Tamplet.Convert_Int(temp[__column++]);
                PackageId = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                BuyItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemId[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuyItemCount[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                PKBuyCount = Table_Tamplet.Convert_Int(temp[__column++]);
                PKChallengeCD = Table_Tamplet.Convert_Int(temp[__column++]);
                Depot = Table_Tamplet.Convert_Int(temp[__column++]);
                Repair = Table_Tamplet.Convert_Int(temp[__column++]);
                WingAdvanced = Table_Tamplet.Convert_Int(temp[__column++]);
                Muse2Reward = Table_Tamplet.Convert_Int(temp[__column++]);
                Muse4Reward = Table_Tamplet.Convert_Int(temp[__column++]);
                StatueAddCount = Table_Tamplet.Convert_Int(temp[__column++]);
                FarmAddRefleshCount = Table_Tamplet.Convert_Int(temp[__column++]);
                SceneBossTrans = Table_Tamplet.Convert_Int(temp[__column++]);
                EnhanceRatio = Table_Tamplet.Convert_Int(temp[__column++]);
                PlotFubenResetCount = Table_Tamplet.Convert_Int(temp[__column++]);
                AreaLimitTrans = Table_Tamplet.Convert_Int(temp[__column++]);
                DevilBuyCount = Table_Tamplet.Convert_Int(temp[__column++]);
                BloodBuyCount = Table_Tamplet.Convert_Int(temp[__column++]);
                VipGoldTemple = Table_Tamplet.Convert_Int(temp[__column++]);
                VipBossTemple = Table_Tamplet.Convert_Int(temp[__column++]);
                SailScanCount = Table_Tamplet.Convert_Int(temp[__column++]);
                WishPoolFilterNum = Table_Tamplet.Convert_Int(temp[__column++]);
                SentTimes = Table_Tamplet.Convert_Int(temp[__column++]);
                PetIslandBuyTimes = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((VIPRecord)_this).Id;
        }
        static object get_NeedVipExp(IRecord _this)
        {
            return ((VIPRecord)_this).NeedVipExp;
        }
        static object get_PackItemParam(IRecord _this)
        {
            return ((VIPRecord)_this).PackItemParam;
        }
        static object get_GetItem(IRecord _this)
        {
            return ((VIPRecord)_this).GetItem;
        }
        static object get_GetBuff(IRecord _this)
        {
            return ((VIPRecord)_this).GetBuff;
        }
        static object get_GetTitle(IRecord _this)
        {
            return ((VIPRecord)_this).GetTitle;
        }
        static object get_PackageId(IRecord _this)
        {
            return ((VIPRecord)_this).PackageId;
        }
        static object get_Desc(IRecord _this)
        {
            return ((VIPRecord)_this).Desc;
        }
        static object get_BuyItemId(IRecord _this)
        {
            return ((VIPRecord)_this).BuyItemId;
        }
        static object get_BuyItemCount(IRecord _this)
        {
            return ((VIPRecord)_this).BuyItemCount;
        }
        static object get_PKBuyCount(IRecord _this)
        {
            return ((VIPRecord)_this).PKBuyCount;
        }
        static object get_PKChallengeCD(IRecord _this)
        {
            return ((VIPRecord)_this).PKChallengeCD;
        }
        static object get_Depot(IRecord _this)
        {
            return ((VIPRecord)_this).Depot;
        }
        static object get_Repair(IRecord _this)
        {
            return ((VIPRecord)_this).Repair;
        }
        static object get_WingAdvanced(IRecord _this)
        {
            return ((VIPRecord)_this).WingAdvanced;
        }
        static object get_Muse2Reward(IRecord _this)
        {
            return ((VIPRecord)_this).Muse2Reward;
        }
        static object get_Muse4Reward(IRecord _this)
        {
            return ((VIPRecord)_this).Muse4Reward;
        }
        static object get_StatueAddCount(IRecord _this)
        {
            return ((VIPRecord)_this).StatueAddCount;
        }
        static object get_FarmAddRefleshCount(IRecord _this)
        {
            return ((VIPRecord)_this).FarmAddRefleshCount;
        }
        static object get_SceneBossTrans(IRecord _this)
        {
            return ((VIPRecord)_this).SceneBossTrans;
        }
        static object get_EnhanceRatio(IRecord _this)
        {
            return ((VIPRecord)_this).EnhanceRatio;
        }
        static object get_PlotFubenResetCount(IRecord _this)
        {
            return ((VIPRecord)_this).PlotFubenResetCount;
        }
        static object get_AreaLimitTrans(IRecord _this)
        {
            return ((VIPRecord)_this).AreaLimitTrans;
        }
        static object get_DevilBuyCount(IRecord _this)
        {
            return ((VIPRecord)_this).DevilBuyCount;
        }
        static object get_BloodBuyCount(IRecord _this)
        {
            return ((VIPRecord)_this).BloodBuyCount;
        }
        static object get_VipGoldTemple(IRecord _this)
        {
            return ((VIPRecord)_this).VipGoldTemple;
        }
        static object get_VipBossTemple(IRecord _this)
        {
            return ((VIPRecord)_this).VipBossTemple;
        }
        static object get_SailScanCount(IRecord _this)
        {
            return ((VIPRecord)_this).SailScanCount;
        }
        static object get_WishPoolFilterNum(IRecord _this)
        {
            return ((VIPRecord)_this).WishPoolFilterNum;
        }
        static object get_SentTimes(IRecord _this)
        {
            return ((VIPRecord)_this).SentTimes;
        }
        static object get_PetIslandBuyTimes(IRecord _this)
        {
            return ((VIPRecord)_this).PetIslandBuyTimes;
        }
        static VIPRecord()
        {
            mField["Id"] = get_Id;
            mField["NeedVipExp"] = get_NeedVipExp;
            mField["PackItemParam"] = get_PackItemParam;
            mField["GetItem"] = get_GetItem;
            mField["GetBuff"] = get_GetBuff;
            mField["GetTitle"] = get_GetTitle;
            mField["PackageId"] = get_PackageId;
            mField["Desc"] = get_Desc;
            mField["BuyItemId"] = get_BuyItemId;
            mField["BuyItemCount"] = get_BuyItemCount;
            mField["PKBuyCount"] = get_PKBuyCount;
            mField["PKChallengeCD"] = get_PKChallengeCD;
            mField["Depot"] = get_Depot;
            mField["Repair"] = get_Repair;
            mField["WingAdvanced"] = get_WingAdvanced;
            mField["Muse2Reward"] = get_Muse2Reward;
            mField["Muse4Reward"] = get_Muse4Reward;
            mField["StatueAddCount"] = get_StatueAddCount;
            mField["FarmAddRefleshCount"] = get_FarmAddRefleshCount;
            mField["SceneBossTrans"] = get_SceneBossTrans;
            mField["EnhanceRatio"] = get_EnhanceRatio;
            mField["PlotFubenResetCount"] = get_PlotFubenResetCount;
            mField["AreaLimitTrans"] = get_AreaLimitTrans;
            mField["DevilBuyCount"] = get_DevilBuyCount;
            mField["BloodBuyCount"] = get_BloodBuyCount;
            mField["VipGoldTemple"] = get_VipGoldTemple;
            mField["VipBossTemple"] = get_VipBossTemple;
            mField["SailScanCount"] = get_SailScanCount;
            mField["WishPoolFilterNum"] = get_WishPoolFilterNum;
            mField["SentTimes"] = get_SentTimes;
            mField["PetIslandBuyTimes"] = get_PetIslandBuyTimes;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class LevelupTipsRecord :IRecord
    {
        public static string __TableName = "LevelupTips.txt";
        public int Id { get; private set; }
        public int IsShow { get; private set; }
        public int[] DictTip = new int[5];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                IsShow = Table_Tamplet.Convert_Int(temp[__column++]);
                DictTip[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                DictTip[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                DictTip[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                DictTip[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                DictTip[4] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((LevelupTipsRecord)_this).Id;
        }
        static object get_IsShow(IRecord _this)
        {
            return ((LevelupTipsRecord)_this).IsShow;
        }
        static object get_DictTip(IRecord _this)
        {
            return ((LevelupTipsRecord)_this).DictTip;
        }
        static LevelupTipsRecord()
        {
            mField["Id"] = get_Id;
            mField["IsShow"] = get_IsShow;
            mField["DictTip"] = get_DictTip;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StrongTypeRecord :IRecord
    {
        public static string __TableName = "StrongType.txt";
        public int Id { get; private set; }
        public string Title { get; private set; }
        public int ConditionId { get; private set; }
        [TableBinding("Icon")]
        public int Icon { get; private set; }
        public string Desc { get; private set; }
        public int UiId { get; private set; }
        public int Tab { get; private set; }
        public int Sort { get; private set; }
        public string ShowStr { get; private set; }
        public int[] Param = new int[2];
        public string OpenDesc { get; private set; }
        public string BtnText { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Title = temp[__column++];
                ConditionId = Table_Tamplet.Convert_Int(temp[__column++]);
                Icon = Table_Tamplet.Convert_Int(temp[__column++]);
                Desc = temp[__column++];
                UiId = Table_Tamplet.Convert_Int(temp[__column++]);
                Tab = Table_Tamplet.Convert_Int(temp[__column++]);
                Sort = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowStr = temp[__column++];
                Param[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenDesc = temp[__column++];
                BtnText = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Id;
        }
        static object get_Title(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Title;
        }
        static object get_ConditionId(IRecord _this)
        {
            return ((StrongTypeRecord)_this).ConditionId;
        }
        static object get_Icon(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Icon;
        }
        static object get_Desc(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Desc;
        }
        static object get_UiId(IRecord _this)
        {
            return ((StrongTypeRecord)_this).UiId;
        }
        static object get_Tab(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Tab;
        }
        static object get_Sort(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Sort;
        }
        static object get_ShowStr(IRecord _this)
        {
            return ((StrongTypeRecord)_this).ShowStr;
        }
        static object get_Param(IRecord _this)
        {
            return ((StrongTypeRecord)_this).Param;
        }
        static object get_OpenDesc(IRecord _this)
        {
            return ((StrongTypeRecord)_this).OpenDesc;
        }
        static object get_BtnText(IRecord _this)
        {
            return ((StrongTypeRecord)_this).BtnText;
        }
        static StrongTypeRecord()
        {
            mField["Id"] = get_Id;
            mField["Title"] = get_Title;
            mField["ConditionId"] = get_ConditionId;
            mField["Icon"] = get_Icon;
            mField["Desc"] = get_Desc;
            mField["UiId"] = get_UiId;
            mField["Tab"] = get_Tab;
            mField["Sort"] = get_Sort;
            mField["ShowStr"] = get_ShowStr;
            mField["Param"] = get_Param;
            mField["OpenDesc"] = get_OpenDesc;
            mField["BtnText"] = get_BtnText;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class StrongDataRecord :IRecord
    {
        public static string __TableName = "StrongData.txt";
        public int Id { get; private set; }
        public int SujectForce { get; private set; }
        public int[] TypeId = new int[20];
        public int[,] Param = new int[20,6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                SujectForce = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[0,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[1,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[2,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[3,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[4,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[5,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[6,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[7,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[8,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[9,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[10] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[10,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[11] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[11,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[12] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[12,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[12,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[12,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[12,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[12,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[12,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[13] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[13,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[13,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[13,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[13,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[13,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[13,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[14] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[14,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[14,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[14,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[14,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[14,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[14,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[15] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[15,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[15,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[15,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[15,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[15,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[15,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[16] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[16,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[16,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[16,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[16,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[16,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[16,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[17] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[17,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[17,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[17,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[17,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[17,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[17,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[18] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[18,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[18,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[18,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[18,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[18,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[18,5] = Table_Tamplet.Convert_Int(temp[__column++]);
                TypeId[19] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[19,0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[19,1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[19,2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[19,3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[19,4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Param[19,5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((StrongDataRecord)_this).Id;
        }
        static object get_SujectForce(IRecord _this)
        {
            return ((StrongDataRecord)_this).SujectForce;
        }
        static object get_TypeId(IRecord _this)
        {
            return ((StrongDataRecord)_this).TypeId;
        }
        static object get_Param(IRecord _this)
        {
            return ((StrongDataRecord)_this).Param;
        }
        static StrongDataRecord()
        {
            mField["Id"] = get_Id;
            mField["SujectForce"] = get_SujectForce;
            mField["TypeId"] = get_TypeId;
            mField["Param"] = get_Param;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MailRecord :IRecord
    {
        public static string __TableName = "Mail.txt";
        public int Id { get; private set; }
        public string Sender { get; private set; }
        public string Title { get; private set; }
        public string Text { get; private set; }
        public int Condition { get; private set; }
        public int[] ItemId = new int[5];
        public int[] ItemCount = new int[5];
        public int Flag { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Sender = temp[__column++];
                Title = temp[__column++];
                Text = temp[__column++];
                Condition = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Flag = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MailRecord)_this).Id;
        }
        static object get_Sender(IRecord _this)
        {
            return ((MailRecord)_this).Sender;
        }
        static object get_Title(IRecord _this)
        {
            return ((MailRecord)_this).Title;
        }
        static object get_Text(IRecord _this)
        {
            return ((MailRecord)_this).Text;
        }
        static object get_Condition(IRecord _this)
        {
            return ((MailRecord)_this).Condition;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((MailRecord)_this).ItemId;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((MailRecord)_this).ItemCount;
        }
        static object get_Flag(IRecord _this)
        {
            return ((MailRecord)_this).Flag;
        }
        static MailRecord()
        {
            mField["Id"] = get_Id;
            mField["Sender"] = get_Sender;
            mField["Title"] = get_Title;
            mField["Text"] = get_Text;
            mField["Condition"] = get_Condition;
            mField["ItemId"] = get_ItemId;
            mField["ItemCount"] = get_ItemCount;
            mField["Flag"] = get_Flag;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class GMCommandRecord :IRecord
    {
        public static string __TableName = "GMCommand.txt";
        public int Id { get; private set; }
        public string Command { get; private set; }
        public int Type { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Command = temp[__column++];
                Type = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((GMCommandRecord)_this).Id;
        }
        static object get_Command(IRecord _this)
        {
            return ((GMCommandRecord)_this).Command;
        }
        static object get_Type(IRecord _this)
        {
            return ((GMCommandRecord)_this).Type;
        }
        static GMCommandRecord()
        {
            mField["Id"] = get_Id;
            mField["Command"] = get_Command;
            mField["Type"] = get_Type;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AuctionType1Record :IRecord
    {
        public static string __TableName = "AuctionType1.txt";
        public int Id { get; private set; }
        public string Type { get; private set; }
        public List<int> SonList = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Type = temp[__column++];
                Table_Tamplet.Convert_Value(SonList,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AuctionType1Record)_this).Id;
        }
        static object get_Type(IRecord _this)
        {
            return ((AuctionType1Record)_this).Type;
        }
        static object get_SonList(IRecord _this)
        {
            return ((AuctionType1Record)_this).SonList;
        }
        static AuctionType1Record()
        {
            mField["Id"] = get_Id;
            mField["Type"] = get_Type;
            mField["SonList"] = get_SonList;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AuctionType2Record :IRecord
    {
        public static string __TableName = "AuctionType2.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<int> SonList = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Table_Tamplet.Convert_Value(SonList,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AuctionType2Record)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((AuctionType2Record)_this).Name;
        }
        static object get_SonList(IRecord _this)
        {
            return ((AuctionType2Record)_this).SonList;
        }
        static AuctionType2Record()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["SonList"] = get_SonList;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AuctionType3Record :IRecord
    {
        public static string __TableName = "AuctionType3.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        [TableBinding("Icon")]
        public int IconId { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                IconId = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AuctionType3Record)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((AuctionType3Record)_this).Name;
        }
        static object get_IconId(IRecord _this)
        {
            return ((AuctionType3Record)_this).IconId;
        }
        static AuctionType3Record()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["IconId"] = get_IconId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class FirstRechargeRecord :IRecord
    {
        public static string __TableName = "FirstRecharge.txt";
        public int Id { get; private set; }
        public int diamond { get; private set; }
        [TableBinding("Dictionary")]
        public int label { get; private set; }
        [TableBinding("Dictionary")]
        public int desc { get; private set; }
        [TableBinding("Flag")]
        public int flag { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                diamond = Table_Tamplet.Convert_Int(temp[__column++]);
                label = Table_Tamplet.Convert_Int(temp[__column++]);
                desc = Table_Tamplet.Convert_Int(temp[__column++]);
                flag = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((FirstRechargeRecord)_this).Id;
        }
        static object get_diamond(IRecord _this)
        {
            return ((FirstRechargeRecord)_this).diamond;
        }
        static object get_label(IRecord _this)
        {
            return ((FirstRechargeRecord)_this).label;
        }
        static object get_desc(IRecord _this)
        {
            return ((FirstRechargeRecord)_this).desc;
        }
        static object get_flag(IRecord _this)
        {
            return ((FirstRechargeRecord)_this).flag;
        }
        static FirstRechargeRecord()
        {
            mField["Id"] = get_Id;
            mField["diamond"] = get_diamond;
            mField["label"] = get_label;
            mField["desc"] = get_desc;
            mField["flag"] = get_flag;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MieShiRecord :IRecord
    {
        public static string __TableName = "MieShi.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public int ReviveCostDiamond { get; private set; }
        public int ReviveCostItemId { get; private set; }
        public int ReviveCostItemNum { get; private set; }
        public int ReviveTime { get; private set; }
        public int ReviveAddTime { get; private set; }
        public int MaxReviveTime { get; private set; }
        public int PriorityEnterCount { get; private set; }
        public int MaxActivityCount { get; private set; }
        public int FirstMobTime { get; private set; }
        public int MobIntervalTime { get; private set; }
        public List<int> Monster1IdList = new List<int>();
        public List<int> Monster1NumList = new List<int>();
        public List<int> Monster2IdList = new List<int>();
        public List<int> Monster2NumList = new List<int>();
        public List<int> Monster3IdList = new List<int>();
        public List<int> Monster3NumList = new List<int>();
        public List<int> Monster4IdList = new List<int>();
        public List<int> Monster4NumList = new List<int>();
        public int OpenDay { get; private set; }
        public int OpenIntervalDay { get; private set; }
        public int FuBenID { get; private set; }
        public int BossCgID { get; private set; }
        public int BossDropBoxId { get; private set; }
        public int BoxAwardId { get; private set; }
        [ListSize(5)]
        public ReadonlyList<int> Entry_x { get; private set; } 
        [ListSize(5)]
        public ReadonlyList<int> Entry_z { get; private set; } 
        [ListSize(5)]
        public ReadonlyList<int> Safe_x { get; private set; } 
        [ListSize(5)]
        public ReadonlyList<int> Safe_z { get; private set; } 
        public string OpenTime { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                ReviveCostDiamond = Table_Tamplet.Convert_Int(temp[__column++]);
                ReviveCostItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                ReviveCostItemNum = Table_Tamplet.Convert_Int(temp[__column++]);
                ReviveTime = Table_Tamplet.Convert_Int(temp[__column++]);
                ReviveAddTime = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxReviveTime = Table_Tamplet.Convert_Int(temp[__column++]);
                PriorityEnterCount = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxActivityCount = Table_Tamplet.Convert_Int(temp[__column++]);
                FirstMobTime = Table_Tamplet.Convert_Int(temp[__column++]);
                MobIntervalTime = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(Monster1IdList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster1NumList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster2IdList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster2NumList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster3IdList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster3NumList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster4IdList,temp[__column++]);
                Table_Tamplet.Convert_Value(Monster4NumList,temp[__column++]);
                OpenDay = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenIntervalDay = Table_Tamplet.Convert_Int(temp[__column++]);
                FuBenID = Table_Tamplet.Convert_Int(temp[__column++]);
                BossCgID = Table_Tamplet.Convert_Int(temp[__column++]);
                BossDropBoxId = Table_Tamplet.Convert_Int(temp[__column++]);
                BoxAwardId = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_x=new ReadonlyList<int>(5);
                Entry_x[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_z=new ReadonlyList<int>(5);
                Entry_z[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_x[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_z[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_x[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_z[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_x[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_z[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_x[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Entry_z[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_x=new ReadonlyList<int>(5);
                Safe_x[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_z=new ReadonlyList<int>(5);
                Safe_z[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_x[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_z[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_x[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_z[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_x[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_z[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_x[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Safe_z[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenTime = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MieShiRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((MieShiRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((MieShiRecord)_this).Desc;
        }
        static object get_ReviveCostDiamond(IRecord _this)
        {
            return ((MieShiRecord)_this).ReviveCostDiamond;
        }
        static object get_ReviveCostItemId(IRecord _this)
        {
            return ((MieShiRecord)_this).ReviveCostItemId;
        }
        static object get_ReviveCostItemNum(IRecord _this)
        {
            return ((MieShiRecord)_this).ReviveCostItemNum;
        }
        static object get_ReviveTime(IRecord _this)
        {
            return ((MieShiRecord)_this).ReviveTime;
        }
        static object get_ReviveAddTime(IRecord _this)
        {
            return ((MieShiRecord)_this).ReviveAddTime;
        }
        static object get_MaxReviveTime(IRecord _this)
        {
            return ((MieShiRecord)_this).MaxReviveTime;
        }
        static object get_PriorityEnterCount(IRecord _this)
        {
            return ((MieShiRecord)_this).PriorityEnterCount;
        }
        static object get_MaxActivityCount(IRecord _this)
        {
            return ((MieShiRecord)_this).MaxActivityCount;
        }
        static object get_FirstMobTime(IRecord _this)
        {
            return ((MieShiRecord)_this).FirstMobTime;
        }
        static object get_MobIntervalTime(IRecord _this)
        {
            return ((MieShiRecord)_this).MobIntervalTime;
        }
        static object get_Monster1IdList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster1IdList;
        }
        static object get_Monster1NumList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster1NumList;
        }
        static object get_Monster2IdList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster2IdList;
        }
        static object get_Monster2NumList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster2NumList;
        }
        static object get_Monster3IdList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster3IdList;
        }
        static object get_Monster3NumList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster3NumList;
        }
        static object get_Monster4IdList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster4IdList;
        }
        static object get_Monster4NumList(IRecord _this)
        {
            return ((MieShiRecord)_this).Monster4NumList;
        }
        static object get_OpenDay(IRecord _this)
        {
            return ((MieShiRecord)_this).OpenDay;
        }
        static object get_OpenIntervalDay(IRecord _this)
        {
            return ((MieShiRecord)_this).OpenIntervalDay;
        }
        static object get_FuBenID(IRecord _this)
        {
            return ((MieShiRecord)_this).FuBenID;
        }
        static object get_BossCgID(IRecord _this)
        {
            return ((MieShiRecord)_this).BossCgID;
        }
        static object get_BossDropBoxId(IRecord _this)
        {
            return ((MieShiRecord)_this).BossDropBoxId;
        }
        static object get_BoxAwardId(IRecord _this)
        {
            return ((MieShiRecord)_this).BoxAwardId;
        }
        static object get_Entry_x(IRecord _this)
        {
            return ((MieShiRecord)_this).Entry_x;
        }
        static object get_Entry_z(IRecord _this)
        {
            return ((MieShiRecord)_this).Entry_z;
        }
        static object get_Safe_x(IRecord _this)
        {
            return ((MieShiRecord)_this).Safe_x;
        }
        static object get_Safe_z(IRecord _this)
        {
            return ((MieShiRecord)_this).Safe_z;
        }
        static object get_OpenTime(IRecord _this)
        {
            return ((MieShiRecord)_this).OpenTime;
        }
        static MieShiRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["ReviveCostDiamond"] = get_ReviveCostDiamond;
            mField["ReviveCostItemId"] = get_ReviveCostItemId;
            mField["ReviveCostItemNum"] = get_ReviveCostItemNum;
            mField["ReviveTime"] = get_ReviveTime;
            mField["ReviveAddTime"] = get_ReviveAddTime;
            mField["MaxReviveTime"] = get_MaxReviveTime;
            mField["PriorityEnterCount"] = get_PriorityEnterCount;
            mField["MaxActivityCount"] = get_MaxActivityCount;
            mField["FirstMobTime"] = get_FirstMobTime;
            mField["MobIntervalTime"] = get_MobIntervalTime;
            mField["Monster1IdList"] = get_Monster1IdList;
            mField["Monster1NumList"] = get_Monster1NumList;
            mField["Monster2IdList"] = get_Monster2IdList;
            mField["Monster2NumList"] = get_Monster2NumList;
            mField["Monster3IdList"] = get_Monster3IdList;
            mField["Monster3NumList"] = get_Monster3NumList;
            mField["Monster4IdList"] = get_Monster4IdList;
            mField["Monster4NumList"] = get_Monster4NumList;
            mField["OpenDay"] = get_OpenDay;
            mField["OpenIntervalDay"] = get_OpenIntervalDay;
            mField["FuBenID"] = get_FuBenID;
            mField["BossCgID"] = get_BossCgID;
            mField["BossDropBoxId"] = get_BossDropBoxId;
            mField["BoxAwardId"] = get_BoxAwardId;
            mField["Entry_x"] = get_Entry_x;
            mField["Entry_z"] = get_Entry_z;
            mField["Safe_x"] = get_Safe_x;
            mField["Safe_z"] = get_Safe_z;
            mField["OpenTime"] = get_OpenTime;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MieShiPublicRecord :IRecord
    {
        public static string __TableName = "MieShiPublic.txt";
        public int Id { get; private set; }
        public int CostType { get; private set; }
        public int CostNum { get; private set; }
        public int ItemId { get; private set; }
        public int ItemNum { get; private set; }
        public int RaiseHP { get; private set; }
        public int GainContribute { get; private set; }
        public int LevelKeepTime { get; private set; }
        public int MineId { get; private set; }
        public int MaxMineNum { get; private set; }
        public int BoxId { get; private set; }
        public int MaxBoxNum { get; private set; }
        public int NormalDamageScore { get; private set; }
        public int EliteDamageScore { get; private set; }
        public int BossDamageScore { get; private set; }
        public int BigBossDamageScore { get; private set; }
        public int MineGainProb { get; private set; }
        [ListSize(2)]
        public ReadonlyList<int> MineScore { get; private set; } 
        public int MaxRaiseHP { get; private set; }
        public int MaxBatteryLevel { get; private set; }
        public int CanApplyTime { get; private set; }
        public int BatteryPromoteTime { get; private set; }
        public int BuffId { get; private set; }
        public int BuffLevel { get; private set; }
        public int PromoteAwardId { get; private set; }
        public int WorshipItemId { get; private set; }
        public int WorshipItemNum { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                CostType = Table_Tamplet.Convert_Int(temp[__column++]);
                CostNum = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemNum = Table_Tamplet.Convert_Int(temp[__column++]);
                RaiseHP = Table_Tamplet.Convert_Int(temp[__column++]);
                GainContribute = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelKeepTime = Table_Tamplet.Convert_Int(temp[__column++]);
                MineId = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxMineNum = Table_Tamplet.Convert_Int(temp[__column++]);
                BoxId = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxBoxNum = Table_Tamplet.Convert_Int(temp[__column++]);
                NormalDamageScore = Table_Tamplet.Convert_Int(temp[__column++]);
                EliteDamageScore = Table_Tamplet.Convert_Int(temp[__column++]);
                BossDamageScore = Table_Tamplet.Convert_Int(temp[__column++]);
                BigBossDamageScore = Table_Tamplet.Convert_Int(temp[__column++]);
                MineGainProb = Table_Tamplet.Convert_Int(temp[__column++]);
                MineScore=new ReadonlyList<int>(2);
                MineScore[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                MineScore[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxRaiseHP = Table_Tamplet.Convert_Int(temp[__column++]);
                MaxBatteryLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                CanApplyTime = Table_Tamplet.Convert_Int(temp[__column++]);
                BatteryPromoteTime = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffId = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                PromoteAwardId = Table_Tamplet.Convert_Int(temp[__column++]);
                WorshipItemId = Table_Tamplet.Convert_Int(temp[__column++]);
                WorshipItemNum = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).Id;
        }
        static object get_CostType(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).CostType;
        }
        static object get_CostNum(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).CostNum;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).ItemId;
        }
        static object get_ItemNum(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).ItemNum;
        }
        static object get_RaiseHP(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).RaiseHP;
        }
        static object get_GainContribute(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).GainContribute;
        }
        static object get_LevelKeepTime(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).LevelKeepTime;
        }
        static object get_MineId(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MineId;
        }
        static object get_MaxMineNum(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MaxMineNum;
        }
        static object get_BoxId(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).BoxId;
        }
        static object get_MaxBoxNum(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MaxBoxNum;
        }
        static object get_NormalDamageScore(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).NormalDamageScore;
        }
        static object get_EliteDamageScore(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).EliteDamageScore;
        }
        static object get_BossDamageScore(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).BossDamageScore;
        }
        static object get_BigBossDamageScore(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).BigBossDamageScore;
        }
        static object get_MineGainProb(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MineGainProb;
        }
        static object get_MineScore(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MineScore;
        }
        static object get_MaxRaiseHP(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MaxRaiseHP;
        }
        static object get_MaxBatteryLevel(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).MaxBatteryLevel;
        }
        static object get_CanApplyTime(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).CanApplyTime;
        }
        static object get_BatteryPromoteTime(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).BatteryPromoteTime;
        }
        static object get_BuffId(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).BuffId;
        }
        static object get_BuffLevel(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).BuffLevel;
        }
        static object get_PromoteAwardId(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).PromoteAwardId;
        }
        static object get_WorshipItemId(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).WorshipItemId;
        }
        static object get_WorshipItemNum(IRecord _this)
        {
            return ((MieShiPublicRecord)_this).WorshipItemNum;
        }
        static MieShiPublicRecord()
        {
            mField["Id"] = get_Id;
            mField["CostType"] = get_CostType;
            mField["CostNum"] = get_CostNum;
            mField["ItemId"] = get_ItemId;
            mField["ItemNum"] = get_ItemNum;
            mField["RaiseHP"] = get_RaiseHP;
            mField["GainContribute"] = get_GainContribute;
            mField["LevelKeepTime"] = get_LevelKeepTime;
            mField["MineId"] = get_MineId;
            mField["MaxMineNum"] = get_MaxMineNum;
            mField["BoxId"] = get_BoxId;
            mField["MaxBoxNum"] = get_MaxBoxNum;
            mField["NormalDamageScore"] = get_NormalDamageScore;
            mField["EliteDamageScore"] = get_EliteDamageScore;
            mField["BossDamageScore"] = get_BossDamageScore;
            mField["BigBossDamageScore"] = get_BigBossDamageScore;
            mField["MineGainProb"] = get_MineGainProb;
            mField["MineScore"] = get_MineScore;
            mField["MaxRaiseHP"] = get_MaxRaiseHP;
            mField["MaxBatteryLevel"] = get_MaxBatteryLevel;
            mField["CanApplyTime"] = get_CanApplyTime;
            mField["BatteryPromoteTime"] = get_BatteryPromoteTime;
            mField["BuffId"] = get_BuffId;
            mField["BuffLevel"] = get_BuffLevel;
            mField["PromoteAwardId"] = get_PromoteAwardId;
            mField["WorshipItemId"] = get_WorshipItemId;
            mField["WorshipItemNum"] = get_WorshipItemNum;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DefendCityRewardRecord :IRecord
    {
        public static string __TableName = "DefendCityReward.txt";
        public int Id { get; private set; }
        public List<int> Rank = new List<int>();
        public int ActivateTitle { get; private set; }
        [ListSize(4)]
        public ReadonlyList<int> RankItemID { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> RankItemCount { get; private set; } 
        public int MailId { get; private set; }
        public int MailId2 { get; private set; }
        public int MailId3 { get; private set; }
        public int MailId4 { get; private set; }
        public int MailId5 { get; private set; }
        public int MailId6 { get; private set; }
        public string RankIcon { get; private set; }
        public List<int> Rate = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(Rank,temp[__column++]);
                ActivateTitle = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemID=new ReadonlyList<int>(4);
                RankItemID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount=new ReadonlyList<int>(4);
                RankItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemID[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId2 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId3 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId4 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId5 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId6 = Table_Tamplet.Convert_Int(temp[__column++]);
                RankIcon = temp[__column++];
                Table_Tamplet.Convert_Value(Rate,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).Id;
        }
        static object get_Rank(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).Rank;
        }
        static object get_ActivateTitle(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).ActivateTitle;
        }
        static object get_RankItemID(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).RankItemID;
        }
        static object get_RankItemCount(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).RankItemCount;
        }
        static object get_MailId(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).MailId;
        }
        static object get_MailId2(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).MailId2;
        }
        static object get_MailId3(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).MailId3;
        }
        static object get_MailId4(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).MailId4;
        }
        static object get_MailId5(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).MailId5;
        }
        static object get_MailId6(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).MailId6;
        }
        static object get_RankIcon(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).RankIcon;
        }
        static object get_Rate(IRecord _this)
        {
            return ((DefendCityRewardRecord)_this).Rate;
        }
        static DefendCityRewardRecord()
        {
            mField["Id"] = get_Id;
            mField["Rank"] = get_Rank;
            mField["ActivateTitle"] = get_ActivateTitle;
            mField["RankItemID"] = get_RankItemID;
            mField["RankItemCount"] = get_RankItemCount;
            mField["MailId"] = get_MailId;
            mField["MailId2"] = get_MailId2;
            mField["MailId3"] = get_MailId3;
            mField["MailId4"] = get_MailId4;
            mField["MailId5"] = get_MailId5;
            mField["MailId6"] = get_MailId6;
            mField["RankIcon"] = get_RankIcon;
            mField["Rate"] = get_Rate;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class DefendCityDevoteRewardRecord :IRecord
    {
        public static string __TableName = "DefendCityDevoteReward.txt";
        public int Id { get; private set; }
        public List<int> Rank = new List<int>();
        [ListSize(4)]
        public ReadonlyList<int> RankItemID { get; private set; } 
        [ListSize(4)]
        public ReadonlyList<int> RankItemCount { get; private set; } 
        public int MailId { get; private set; }
        public int MailId2 { get; private set; }
        public int MailId3 { get; private set; }
        public int MailId4 { get; private set; }
        public int MailId5 { get; private set; }
        public int MailId6 { get; private set; }
        public string ContributionIcon { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(Rank,temp[__column++]);
                RankItemID=new ReadonlyList<int>(4);
                RankItemID[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount=new ReadonlyList<int>(4);
                RankItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemID[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemID[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                RankItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId2 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId3 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId4 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId5 = Table_Tamplet.Convert_Int(temp[__column++]);
                MailId6 = Table_Tamplet.Convert_Int(temp[__column++]);
                ContributionIcon = temp[__column++];
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).Id;
        }
        static object get_Rank(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).Rank;
        }
        static object get_RankItemID(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).RankItemID;
        }
        static object get_RankItemCount(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).RankItemCount;
        }
        static object get_MailId(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).MailId;
        }
        static object get_MailId2(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).MailId2;
        }
        static object get_MailId3(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).MailId3;
        }
        static object get_MailId4(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).MailId4;
        }
        static object get_MailId5(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).MailId5;
        }
        static object get_MailId6(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).MailId6;
        }
        static object get_ContributionIcon(IRecord _this)
        {
            return ((DefendCityDevoteRewardRecord)_this).ContributionIcon;
        }
        static DefendCityDevoteRewardRecord()
        {
            mField["Id"] = get_Id;
            mField["Rank"] = get_Rank;
            mField["RankItemID"] = get_RankItemID;
            mField["RankItemCount"] = get_RankItemCount;
            mField["MailId"] = get_MailId;
            mField["MailId2"] = get_MailId2;
            mField["MailId3"] = get_MailId3;
            mField["MailId4"] = get_MailId4;
            mField["MailId5"] = get_MailId5;
            mField["MailId6"] = get_MailId6;
            mField["ContributionIcon"] = get_ContributionIcon;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BatteryLevelRecord :IRecord
    {
        public static string __TableName = "BatteryLevel.txt";
        public int Id { get; private set; }
        public int BatterySkillId { get; private set; }
        public int BatterySkillDesc { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                BatterySkillId = Table_Tamplet.Convert_Int(temp[__column++]);
                BatterySkillDesc = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BatteryLevelRecord)_this).Id;
        }
        static object get_BatterySkillId(IRecord _this)
        {
            return ((BatteryLevelRecord)_this).BatterySkillId;
        }
        static object get_BatterySkillDesc(IRecord _this)
        {
            return ((BatteryLevelRecord)_this).BatterySkillDesc;
        }
        static BatteryLevelRecord()
        {
            mField["Id"] = get_Id;
            mField["BatterySkillId"] = get_BatterySkillId;
            mField["BatterySkillDesc"] = get_BatterySkillDesc;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BatteryBaseRecord :IRecord
    {
        public static string __TableName = "BatteryBase.txt";
        public int Id { get; private set; }
        [ListSize(3)]
        public ReadonlyList<int> BatteryNpcId { get; private set; } 
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                BatteryNpcId=new ReadonlyList<int>(3);
                BatteryNpcId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BatteryNpcId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BatteryNpcId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BatteryBaseRecord)_this).Id;
        }
        static object get_BatteryNpcId(IRecord _this)
        {
            return ((BatteryBaseRecord)_this).BatteryNpcId;
        }
        static BatteryBaseRecord()
        {
            mField["Id"] = get_Id;
            mField["BatteryNpcId"] = get_BatteryNpcId;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MieShiFightingRecord :IRecord
    {
        public static string __TableName = "MieShiFighting.txt";
        public int Id { get; private set; }
        public int LevelFighting { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                LevelFighting = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MieShiFightingRecord)_this).Id;
        }
        static object get_LevelFighting(IRecord _this)
        {
            return ((MieShiFightingRecord)_this).LevelFighting;
        }
        static MieShiFightingRecord()
        {
            mField["Id"] = get_Id;
            mField["LevelFighting"] = get_LevelFighting;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class FunctionOnRecord :IRecord
    {
        public static string __TableName = "FunctionOn.txt";
        public int Id { get; private set; }
        public int OpenNum { get; private set; }
        public int NextNum { get; private set; }
        public int OpenLevel { get; private set; }
        [TableBinding("Icon")]
        public int IconId { get; private set; }
        public string IconDesc { get; private set; }
        public string FrameDesc { get; private set; }
        public string Name { get; private set; }
        [TableBinding("MissionBase")]
        public int TaskID { get; private set; }
        public int State { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenNum = Table_Tamplet.Convert_Int(temp[__column++]);
                NextNum = Table_Tamplet.Convert_Int(temp[__column++]);
                OpenLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                IconId = Table_Tamplet.Convert_Int(temp[__column++]);
                IconDesc = Table_Tamplet.Convert_String(temp[__column++]);
                FrameDesc = Table_Tamplet.Convert_String(temp[__column++]);
                Name = Table_Tamplet.Convert_String(temp[__column++]);
                TaskID = Table_Tamplet.Convert_Int(temp[__column++]);
                State = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((FunctionOnRecord)_this).Id;
        }
        static object get_OpenNum(IRecord _this)
        {
            return ((FunctionOnRecord)_this).OpenNum;
        }
        static object get_NextNum(IRecord _this)
        {
            return ((FunctionOnRecord)_this).NextNum;
        }
        static object get_OpenLevel(IRecord _this)
        {
            return ((FunctionOnRecord)_this).OpenLevel;
        }
        static object get_IconId(IRecord _this)
        {
            return ((FunctionOnRecord)_this).IconId;
        }
        static object get_IconDesc(IRecord _this)
        {
            return ((FunctionOnRecord)_this).IconDesc;
        }
        static object get_FrameDesc(IRecord _this)
        {
            return ((FunctionOnRecord)_this).FrameDesc;
        }
        static object get_Name(IRecord _this)
        {
            return ((FunctionOnRecord)_this).Name;
        }
        static object get_TaskID(IRecord _this)
        {
            return ((FunctionOnRecord)_this).TaskID;
        }
        static object get_State(IRecord _this)
        {
            return ((FunctionOnRecord)_this).State;
        }
        static FunctionOnRecord()
        {
            mField["Id"] = get_Id;
            mField["OpenNum"] = get_OpenNum;
            mField["NextNum"] = get_NextNum;
            mField["OpenLevel"] = get_OpenLevel;
            mField["IconId"] = get_IconId;
            mField["IconDesc"] = get_IconDesc;
            mField["FrameDesc"] = get_FrameDesc;
            mField["Name"] = get_Name;
            mField["TaskID"] = get_TaskID;
            mField["State"] = get_State;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BangBuffRecord :IRecord
    {
        public static string __TableName = "BangBuff.txt";
        public int Id { get; private set; }
        public int[] BuffGoldId = new int[5];
        public int[] BuffGoldPrice = new int[5];
        public int[] BuffDiamodId = new int[5];
        public int[] BuffDiamodPrice = new int[5];
        public int ShowPointer { get; private set; }
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldPrice[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodPrice[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldPrice[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodPrice[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldPrice[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodPrice[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldPrice[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodPrice[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffGoldPrice[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                BuffDiamodPrice[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ShowPointer = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BangBuffRecord)_this).Id;
        }
        static object get_BuffGoldId(IRecord _this)
        {
            return ((BangBuffRecord)_this).BuffGoldId;
        }
        static object get_BuffGoldPrice(IRecord _this)
        {
            return ((BangBuffRecord)_this).BuffGoldPrice;
        }
        static object get_BuffDiamodId(IRecord _this)
        {
            return ((BangBuffRecord)_this).BuffDiamodId;
        }
        static object get_BuffDiamodPrice(IRecord _this)
        {
            return ((BangBuffRecord)_this).BuffDiamodPrice;
        }
        static object get_ShowPointer(IRecord _this)
        {
            return ((BangBuffRecord)_this).ShowPointer;
        }
        static BangBuffRecord()
        {
            mField["Id"] = get_Id;
            mField["BuffGoldId"] = get_BuffGoldId;
            mField["BuffGoldPrice"] = get_BuffGoldPrice;
            mField["BuffDiamodId"] = get_BuffDiamodId;
            mField["BuffDiamodPrice"] = get_BuffDiamodPrice;
            mField["ShowPointer"] = get_ShowPointer;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class BuffGroupRecord :IRecord
    {
        public static string __TableName = "BuffGroup.txt";
        public int Id { get; private set; }
        public List<int> BuffID = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(BuffID,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((BuffGroupRecord)_this).Id;
        }
        static object get_BuffID(IRecord _this)
        {
            return ((BuffGroupRecord)_this).BuffID;
        }
        static BuffGroupRecord()
        {
            mField["Id"] = get_Id;
            mField["BuffID"] = get_BuffID;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class MieshiTowerRewardRecord :IRecord
    {
        public static string __TableName = "MieshiTowerReward.txt";
        public int Id { get; private set; }
        public List<int> TimesStep = new List<int>();
        public int DiamondCost { get; private set; }
        public List<int> ShowValue = new List<int>();
        public int StepReward { get; private set; }
        public List<int> StepRewardList = new List<int>();
        public List<int> StepNumList = new List<int>();
        public int OnceReward { get; private set; }
        public List<int> OnceRewardList = new List<int>();
        public List<int> OnceNumList = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(TimesStep,temp[__column++]);
                DiamondCost = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(ShowValue,temp[__column++]);
                StepReward = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(StepRewardList,temp[__column++]);
                Table_Tamplet.Convert_Value(StepNumList,temp[__column++]);
                OnceReward = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(OnceRewardList,temp[__column++]);
                Table_Tamplet.Convert_Value(OnceNumList,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).Id;
        }
        static object get_TimesStep(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).TimesStep;
        }
        static object get_DiamondCost(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).DiamondCost;
        }
        static object get_ShowValue(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).ShowValue;
        }
        static object get_StepReward(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).StepReward;
        }
        static object get_StepRewardList(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).StepRewardList;
        }
        static object get_StepNumList(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).StepNumList;
        }
        static object get_OnceReward(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).OnceReward;
        }
        static object get_OnceRewardList(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).OnceRewardList;
        }
        static object get_OnceNumList(IRecord _this)
        {
            return ((MieshiTowerRewardRecord)_this).OnceNumList;
        }
        static MieshiTowerRewardRecord()
        {
            mField["Id"] = get_Id;
            mField["TimesStep"] = get_TimesStep;
            mField["DiamondCost"] = get_DiamondCost;
            mField["ShowValue"] = get_ShowValue;
            mField["StepReward"] = get_StepReward;
            mField["StepRewardList"] = get_StepRewardList;
            mField["StepNumList"] = get_StepNumList;
            mField["OnceReward"] = get_OnceReward;
            mField["OnceRewardList"] = get_OnceRewardList;
            mField["OnceNumList"] = get_OnceNumList;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ClimbingTowerRecord :IRecord
    {
        public static string __TableName = "ClimbingTower.txt";
        public int Id { get; private set; }
        public int FubenId { get; private set; }
        public int Boss { get; private set; }
        public int HiddenMosterId { get; private set; }
        public List<int> RewardList = new List<int>();
        public List<int> NumList = new List<int>();
        public List<int> OnceRewardList = new List<int>();
        public List<int> OnceNumList = new List<int>();
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                FubenId = Table_Tamplet.Convert_Int(temp[__column++]);
                Boss = Table_Tamplet.Convert_Int(temp[__column++]);
                HiddenMosterId = Table_Tamplet.Convert_Int(temp[__column++]);
                Table_Tamplet.Convert_Value(RewardList,temp[__column++]);
                Table_Tamplet.Convert_Value(NumList,temp[__column++]);
                Table_Tamplet.Convert_Value(OnceRewardList,temp[__column++]);
                Table_Tamplet.Convert_Value(OnceNumList,temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).Id;
        }
        static object get_FubenId(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).FubenId;
        }
        static object get_Boss(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).Boss;
        }
        static object get_HiddenMosterId(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).HiddenMosterId;
        }
        static object get_RewardList(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).RewardList;
        }
        static object get_NumList(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).NumList;
        }
        static object get_OnceRewardList(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).OnceRewardList;
        }
        static object get_OnceNumList(IRecord _this)
        {
            return ((ClimbingTowerRecord)_this).OnceNumList;
        }
        static ClimbingTowerRecord()
        {
            mField["Id"] = get_Id;
            mField["FubenId"] = get_FubenId;
            mField["Boss"] = get_Boss;
            mField["HiddenMosterId"] = get_HiddenMosterId;
            mField["RewardList"] = get_RewardList;
            mField["NumList"] = get_NumList;
            mField["OnceRewardList"] = get_OnceRewardList;
            mField["OnceNumList"] = get_OnceNumList;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class AcientBattleFieldRecord :IRecord
    {
        public static string __TableName = "AcientBattleField.txt";
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public int NeedLevel { get; private set; }
        [TableBinding("CharacterBase")]
        public int CharacterBaseId { get; private set; }
        public int CostEnergy { get; private set; }
        public int SpawnTimeHour { get; private set; }
        public int[] Item = new int[4];
        public int[] ItemCount = new int[4];
        public int[] ItemDropWight = new int[4];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Name = temp[__column++];
                Desc = temp[__column++];
                NeedLevel = Table_Tamplet.Convert_Int(temp[__column++]);
                CharacterBaseId = Table_Tamplet.Convert_Int(temp[__column++]);
                CostEnergy = Table_Tamplet.Convert_Int(temp[__column++]);
                SpawnTimeHour = Table_Tamplet.Convert_Int(temp[__column++]);
                Item[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDropWight[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Item[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDropWight[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Item[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDropWight[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Item[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemDropWight[3] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).Id;
        }
        static object get_Name(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).Name;
        }
        static object get_Desc(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).Desc;
        }
        static object get_NeedLevel(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).NeedLevel;
        }
        static object get_CharacterBaseId(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).CharacterBaseId;
        }
        static object get_CostEnergy(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).CostEnergy;
        }
        static object get_SpawnTimeHour(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).SpawnTimeHour;
        }
        static object get_Item(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).Item;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).ItemCount;
        }
        static object get_ItemDropWight(IRecord _this)
        {
            return ((AcientBattleFieldRecord)_this).ItemDropWight;
        }
        static AcientBattleFieldRecord()
        {
            mField["Id"] = get_Id;
            mField["Name"] = get_Name;
            mField["Desc"] = get_Desc;
            mField["NeedLevel"] = get_NeedLevel;
            mField["CharacterBaseId"] = get_CharacterBaseId;
            mField["CostEnergy"] = get_CostEnergy;
            mField["SpawnTimeHour"] = get_SpawnTimeHour;
            mField["Item"] = get_Item;
            mField["ItemCount"] = get_ItemCount;
            mField["ItemDropWight"] = get_ItemDropWight;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ElfStarShaderRecord :IRecord
    {
        public static string __TableName = "ElfStarShader.txt";
        public int Id { get; private set; }
        public int[] Star = new int[6];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                Star[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                Star[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                Star[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                Star[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                Star[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                Star[5] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ElfStarShaderRecord)_this).Id;
        }
        static object get_Star(IRecord _this)
        {
            return ((ElfStarShaderRecord)_this).Star;
        }
        static ElfStarShaderRecord()
        {
            mField["Id"] = get_Id;
            mField["Star"] = get_Star;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
    public class ConsumArrayRecord :IRecord
    {
        public static string __TableName = "ConsumArray.txt";
        public int Id { get; private set; }
        public int[] ItemId = new int[10];
        public int[] ItemCount = new int[10];
        public void Init(string[] temp)
        {
            int __column = 0;
            try
            {
                Id = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[0] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[1] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[2] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[3] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[4] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[5] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[6] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[7] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[8] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemId[9] = Table_Tamplet.Convert_Int(temp[__column++]);
                ItemCount[9] = Table_Tamplet.Convert_Int(temp[__column++]);
            }
            catch (Exception ex)
            {
                string s = string.Format("ERROR:Load table[{0}] id=[{1}] column=[{2}]", __TableName, temp[0], __column);
                Logger.Debug(s);
                throw ;
            }
        }
        static object get_Id(IRecord _this)
        {
            return ((ConsumArrayRecord)_this).Id;
        }
        static object get_ItemId(IRecord _this)
        {
            return ((ConsumArrayRecord)_this).ItemId;
        }
        static object get_ItemCount(IRecord _this)
        {
            return ((ConsumArrayRecord)_this).ItemCount;
        }
        static ConsumArrayRecord()
        {
            mField["Id"] = get_Id;
            mField["ItemId"] = get_ItemId;
            mField["ItemCount"] = get_ItemCount;
        }
        private static Dictionary<string, Func<IRecord, object>> mField = new Dictionary<string, Func<IRecord, object>>();
        public object GetField(string name)
        {
            return mField[name](this);
        }
    }
}
