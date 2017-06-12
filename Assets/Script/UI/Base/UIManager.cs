
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CLRSharp;
using DataTable;
using EventSystem;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using UnityEngine;

public enum UIType
{
    TYPE_ITEM = -1,         //只是用资源路径
    TYPE_BASE = 0,          //基础 ： 初始层级（100） 摇杆，技能栏，角色头像等，与场景紧密相关的界面
    TYPE_POP = 1,           //全屏界面： 200
    TYPE_POPLEFT = 2,       //坐半屏： 300 左边的，比如角色界面
    TYPE_POPRight = 3,      //右半屏： 400 右边的，比如包裹界面
    TYPE_TIP = 4,         //提示框： 500 悬浮提示
    TYPE_MESSAGE,           //信息框： 600 消息提示UI 在三级之上 一般是最高层级 不互斥 不阻止移动 可操作后面UI
    TYPE_BLOCK,             //屏蔽层： 999
    Type_Max
}

public enum MessageBoxType
{
    OkCancel = 1,
    Ok = 2,
    No = 3,
    Cancel = 4,
}

public class UIConfig
{
    public UIType TypeUI;
    public Vector3 Loction;
    public UIRecord UiRecord;
    private int mVisibliltyCount;
    private Type mArgumnetType;

	//重载等行操作符
	public static bool operator ==(UIConfig rec1, UIConfig rec2)
	{
		return System.Object.Equals(rec1, rec2);
	}

	//重载不等操作符
	public static bool operator !=(UIConfig rec1, UIConfig rec2)
	{
		return !System.Object.Equals(rec1, rec2);
	}  
    public string NameUI
    {
        get { return UiRecord.Name; }
        //set { UiRecord.Name = value; }
    }
    public string PrefabPath
    {
        get { return UiRecord.Path; }
    }
    public void IncReference()
    {
        mVisibliltyCount++;
        if (mVisibliltyCount != 1)
        {
            //Logger.Error("SeeUI to More={0}:{1}", IsCanSee,NameUI);
        }
    }
    public void DecReference()
    {
        mVisibliltyCount--;
        if (mVisibliltyCount < 0)
        {
            //Logger.Error("UnSeeUI to Less={0}:{1}", IsCanSee, NameUI);
            mVisibliltyCount = 0;
        }
    }
    public bool Visible()
    {
        return mVisibliltyCount != 0;
    }
    public void CleanReferenceCount()
    {
        if (mVisibliltyCount > 0)
        {
            //Logger.Error("CleanSee to More={0}:{1}", IsCanSee, NameUI);
        }
        mVisibliltyCount = 0;
    }
    public static UIConfig SceneMapUI;
    public static UIConfig MainUI;
    public static UIConfig BackPackUI;
    public static UIConfig AttriFrameUI;
    public static UIConfig SelectEquipsUI;
    public static UIConfig SelectRoleUI;
    
    public static UIConfig BlockLayer;
    public static UIConfig SkillFrameUI;
    public static UIConfig SkillTipFrameUI;
    public static UIConfig MissionFrame;
    public static UIConfig DialogFrame;
    public static UIConfig TeamFrame;
    public static UIConfig MissionList;
    public static UIConfig MissionTrackList;
    
    public static UIConfig OperationList;
    public static UIConfig MessageBox;
    public static UIConfig RewardFrame;
    public static UIConfig EquipUI;
    public static UIConfig AchievementFrame;
    public static UIConfig ChatMainFrame;
    public static UIConfig AchievementTip;
    public static UIConfig HandBook;
    public static UIConfig FriendUI;
    public static UIConfig DungeonUI;
    public static UIConfig ComposeUI;
    public static UIConfig AttributeUI;
    
    public static UIConfig SettingUI;
    public static UIConfig RankUI;
    public static UIConfig PlayerInfoUI;
    public static UIConfig MailUI;
    public static UIConfig CharacterUI;
    public static UIConfig DepotUI;
    public static UIConfig StoreUI;
    public static UIConfig StoreEquip;
    public static UIConfig StoreFarm;
    public static UIConfig ElfUI;
    public static UIConfig WingUI;
    public static UIConfig SailingUI;
    public static UIConfig FarmUI;
    public static UIConfig ServerListUI;
    public static UIConfig MedalInfoUI;
    public static UIConfig AreanaUI;
    public static UIConfig AreanaResult;
    public static UIConfig ReliveUI;
    public static UIConfig WishingUI;
    public static UIConfig SmithyUI;
    public static UIConfig TradingUI;
    public static UIConfig DungeonResult;
    public static UIConfig RecycleUI;
    public static UIConfig BattleUnionUI;
    public static UIConfig ForceUI;
    //
    public static UIConfig ItemInfoUI;
    public static UIConfig EquipComPareUI;
    public static UIConfig EquipInfoUI;
    public static UIConfig GainItemHintUI;
    public static UIConfig ElfInfoUI;
    public static UIConfig BattleUI;
    public static UIConfig BattleResult; 
    public static UIConfig AstrologyUI;
    public static UIConfig NewbieGuide;
    public static UIConfig ActivityUI;
    public static UIConfig ActivityRewardFrame;
    public static UIConfig BossRewardFrame;

    public static UIConfig RebornUi;
    public static UIConfig OffLineExpFrame;
    public static UIConfig SystemNoticeFrame;

    public static UIConfig WingInfoUi;

    public static Dictionary<int, UIConfig> SConfigs =new Dictionary<int, UIConfig>();
    //
    public static UIConfig EquipPack;
    public static UIConfig ItemInfoGetUI;
    public static UIConfig DungeonRewardFrame;
	public static UIConfig CustomShopFrame;
    public static UIConfig AnswerUI;
    public static UIConfig FaceList;
    public static UIConfig ChatItemList;
    public static UIConfig PuzzleImage;
    public static UIConfig CleanDust;
    public static UIConfig LineConfim;
    public static UIConfig RechargeFrame;
    public static UIConfig PlayFrame;
    public static UIConfig TitleUI;
    public static UIConfig LevelUpTip;
	public static UIConfig MissionTip;
    public static UIConfig SevenDayReward;
    public static UIConfig StrongUI;
    public static UIConfig GuardUI;
    public static UIConfig RechargeActivityUI;
 	public static UIConfig ShareFrame;
    public static UIConfig WorshipFrame;
	public static UIConfig OperationActivityFrame;
	public static UIConfig AcientBattleFieldFrame;
    public static UIConfig ModelDisplayFrame;
    public static UIConfig ShowItemsFrame;
	public static UIConfig FirstChargeFrame;
    public static UIConfig WingChargeFrame;
    public static UIConfig QuickBuyUi;
    public static UIConfig ArtifactUi;
    public static UIConfig MyArtifactUI;
    public static UIConfig BufferListUI;
    public static UIConfig EquipSkillTipUI;

    public static UIConfig MonsterSiegeUI;//灭世之战面板
    public static UIConfig GXRankingUI;//贡献排行面板
    public static UIConfig MishiResultUI;//灭世之战结束界面
    public static UIConfig BattryLevelUpUI;//灭世之战战斗中提升炮台界面
    public static UIConfig MieShiTapUI;//进入蔑视后的提示面板
    public static UIConfig ExchangeUI; // 兑换金币UI
    public static UIConfig FuctionTipFrame; // ICON开启UI
    public static UIConfig MieShiSceneMapUI;

    public static UIConfig ChestInfoUI;
    public static UIConfig ClimbingTowerRewardUI;
    public static UIConfig ClimbingTowerUI;

    public static UIConfig MessageBoxEx;
    

    private class ScriptLogger : ICLRSharp_Logger
    {
        public void Log(string str)
        {
            Logger.Info(str);
        }
        public void Log_Warning(string str)
        {
            Logger.Warn(str);
        }
        public void Log_Error(string str)
        {
            Logger.Error(str);
        }
    }

    private static bool virtualMachineInited = false;
    private static CLRSharp_Environment environment = null;

    private static UIConfig RegisterUIType(int id, IControllerBase controllerBase, Type argumentType)
    {
        return new UIConfig(id, controllerBase, argumentType);
    }
    private static UIConfig RegisterUIType(int id, string type, Type argumentType)
    {
        if (GameSetting.Instance.UseUIScript)
        {
            if (!virtualMachineInited)
            {
                virtualMachineInited = true;

                var bytes = ResourceManager.PrepareResourceSync<TextAsset>("Script/ScriptHolder.dll.bytes").bytes;

                MemoryStream ms = new MemoryStream(bytes);

                var env = new CLRSharp_Environment(new ScriptLogger());
                environment = env;

                ThreadContext.activeContext = null;
                ThreadContext context = new ThreadContext(env, 0);
                ThreadContext.activeContext.SetNoTry = false;

                env.RegCrossBind(new CrossBind_IControllerBase());

                try
                {
                    FastCallInit.Init();
                }
                catch (Exception ex)
                {
                    Logger.Error("RegisterFastCall error." + ex.ToString());
                }
                
                try
                {
                    AotInit.Inited = true;
                }
                catch (Exception ex)
                {
                    Logger.Error("RegisterAot error." + ex.ToString());
                }

                try
                {
                    var pdb = ResourceManager.PrepareResourceSync<TextAsset>("Script/ScriptHolder.pdb.bytes");
                    if (pdb == null)
                    {
                        var mdb = ResourceManager.PrepareResourceSync<TextAsset>("Script/ScriptHolder.mdb.bytes");
                        if (mdb == null)
                        {
                            env.LoadModule(ms);
                        }
                        else
                        {
                            MemoryStream msmdb = new MemoryStream(mdb.bytes);
                            env.LoadModule(ms, msmdb, new MdbReaderProvider());
                        }
                    }
                    else
                    {
                        MemoryStream mspdb = new MemoryStream(pdb.bytes);
                        env.LoadModule(ms, mspdb, new PdbReaderProvider());
                    }
                }
                catch (Exception err)
                {
                    Logger.Error(err.ToString());
                    Logger.Error("模块未加载完成，请检查错误");
                }
            }

            try
            {
                var t = environment.GetType(type) as Type_Common_CLRSharp;
                return new UIConfig(id, new ScriptableController(environment, t, type), argumentType);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    "Load {0} error, Please add coresponding files into ScriptHolder and compiler it, then try again. {1}",
                    type, ex.ToString());
                throw;
            }
        }
        else
        {
#if true

            try
            {
                var t = Assembly.GetExecutingAssembly().GetType(type);
                return new UIConfig(id, (IControllerBase)Activator.CreateInstance(t), argumentType);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    "Load {0} error, Please add coresponding files into ScriptHolder and compiler it, then try again. {1}",
                    type, ex.ToString());
                throw;
            }

#else
            try
            {
                if (ass == null)
                {
                    var bytes = ResourceManager.PrepareResourceSync<TextAsset>("Script/ScriptHolder.dll.bytes").bytes;
                    ass = Assembly.Load(bytes);
                }

                return new UIConfig(id, (IControllerBase) Activator.CreateInstance(ass.GetType(type)), argumentType);
        }
            catch (Exception ex)
            {
                Logger.Error(
                    "Load {0} error, Please add coresponding files into ScriptHolder and compiler it, then try again. {1}",
                    type, ex.ToString());
                throw;
    }
#endif
        }
    }


    static UIConfig()
    {
        //Init();
    }
    public static void Reset()
    {
        virtualMachineInited = false;
        SConfigs.Clear();
        CleanupControllers();
        Init();
    }

    public static void CleanupControllers()
    {


        if (MonsterSiegeUI != null)
        {
            var controller = UIManager.Instance.GetController(MonsterSiegeUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (GXRankingUI != null)
        {
            var controller = UIManager.Instance.GetController(GXRankingUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
         
         if (OffLineExpFrame != null)
         {
             var controller = UIManager.Instance.GetController(OffLineExpFrame);
             if (controller != null)
             {
                 controller.CleanUp();
             }
         }

         if (SystemNoticeFrame != null)
         {
             var controller = UIManager.Instance.GetController(SystemNoticeFrame);
             if (controller != null)
             {
                 controller.CleanUp();
             }
         }
        if (SceneMapUI != null)
        {
            var controller = UIManager.Instance.GetController(SceneMapUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MainUI != null)
        {
            var controller = UIManager.Instance.GetController(MainUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BackPackUI != null)
        {
            var controller = UIManager.Instance.GetController(BackPackUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AttriFrameUI != null)
        {
            var controller = UIManager.Instance.GetController(AttriFrameUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (SelectRoleUI != null)
        {
            var controller = UIManager.Instance.GetController(SelectRoleUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (EquipComPareUI != null)
        {
            var controller = UIManager.Instance.GetController(EquipComPareUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BlockLayer != null)
        {
            var controller = UIManager.Instance.GetController(BlockLayer);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (SkillFrameUI != null)
        {
            var controller = UIManager.Instance.GetController(SkillFrameUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (SkillTipFrameUI != null)
        {
            var controller = UIManager.Instance.GetController(SkillTipFrameUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (MissionFrame != null)
        {
            var controller = UIManager.Instance.GetController(MissionFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (DialogFrame != null)
        {
            var controller = UIManager.Instance.GetController(DialogFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (TeamFrame != null)
        {
            var controller = UIManager.Instance.GetController(TeamFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MissionList != null)
        {
            var controller = UIManager.Instance.GetController(MissionList);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MissionTrackList != null)
        {
            var controller = UIManager.Instance.GetController(MissionTrackList);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ItemInfoUI != null)
        {
            var controller = UIManager.Instance.GetController(ItemInfoUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (OperationList != null)
        {
            var controller = UIManager.Instance.GetController(OperationList);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MessageBox != null)
        {
            var controller = UIManager.Instance.GetController(MessageBox);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (RewardFrame != null)
        {
            var controller = UIManager.Instance.GetController(RewardFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (EquipUI != null)
        {
            var controller = UIManager.Instance.GetController(EquipUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AchievementFrame != null)
        {
            var controller = UIManager.Instance.GetController(AchievementFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ChatMainFrame != null)
        {
            var controller = UIManager.Instance.GetController(ChatMainFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AchievementTip != null)
        {
            var controller = UIManager.Instance.GetController(AchievementTip);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (HandBook != null)
        {
            var controller = UIManager.Instance.GetController(HandBook);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (FriendUI != null)
        {
            var controller = UIManager.Instance.GetController(FriendUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (DungeonUI != null)
        {
            var controller = UIManager.Instance.GetController(DungeonUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ComposeUI != null)
        {
            var controller = UIManager.Instance.GetController(ComposeUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AttributeUI != null)
        {
            var controller = UIManager.Instance.GetController(AttributeUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (EquipInfoUI != null)
        {
            var controller = UIManager.Instance.GetController(EquipInfoUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (SettingUI != null)
        {
            var controller = UIManager.Instance.GetController(SettingUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (RankUI != null)
        {
            var controller = UIManager.Instance.GetController(RankUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (PlayerInfoUI != null)
        {
            var controller = UIManager.Instance.GetController(PlayerInfoUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MailUI != null)
        {
            var controller = UIManager.Instance.GetController(MailUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (CharacterUI != null)
        {
            var controller = UIManager.Instance.GetController(CharacterUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (DepotUI != null)
        {
            var controller = UIManager.Instance.GetController(DepotUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (StoreUI != null || StoreEquip != null)
        {
            var controller = UIManager.Instance.GetController(StoreUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (ElfUI != null)
        {
            var controller = UIManager.Instance.GetController(ElfUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (WingUI != null)
        {
            var controller = UIManager.Instance.GetController(WingUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (SailingUI != null)
        {
            var controller = UIManager.Instance.GetController(SailingUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ServerListUI != null)
        {
            var controller = UIManager.Instance.GetController(ServerListUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (FarmUI != null)
        {
            var controller = UIManager.Instance.GetController(FarmUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MedalInfoUI != null)
        {
            var controller = UIManager.Instance.GetController(MedalInfoUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (DungeonResult != null)
        {
            var controller = UIManager.Instance.GetController(DungeonResult);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AreanaUI != null)
        {
            var controller = UIManager.Instance.GetController(AreanaUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AreanaResult != null)
        {
            var controller = UIManager.Instance.GetController(AreanaResult);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ReliveUI != null)
        {
            var controller = UIManager.Instance.GetController(ReliveUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (WishingUI != null)
        {
            var controller = UIManager.Instance.GetController(WishingUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (RecycleUI != null)
        {
            var controller = UIManager.Instance.GetController(RecycleUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BattleUnionUI != null)
        {
            var controller = UIManager.Instance.GetController(BattleUnionUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ForceUI != null)
        {
            var controller = UIManager.Instance.GetController(ForceUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BattleUI != null)
        {
            var controller = UIManager.Instance.GetController(BattleUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (GainItemHintUI != null)
        {
            var controller = UIManager.Instance.GetController(GainItemHintUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ElfInfoUI != null)
        {
            var controller = UIManager.Instance.GetController(ElfInfoUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (SmithyUI != null)
        {
            var controller = UIManager.Instance.GetController(SmithyUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (TradingUI != null)
        {
            var controller = UIManager.Instance.GetController(TradingUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AstrologyUI != null)
        {
            var controller = UIManager.Instance.GetController(AstrologyUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (NewbieGuide != null)
        {
            var controller = UIManager.Instance.GetController(NewbieGuide);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ActivityUI != null)
        {
            var controller = UIManager.Instance.GetController(ActivityUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ActivityRewardFrame != null)
        {
            var controller = UIManager.Instance.GetController(ActivityRewardFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BossRewardFrame != null)
        {
            var controller = UIManager.Instance.GetController(BossRewardFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (RebornUi != null)
        {
            var controller = UIManager.Instance.GetController(RebornUi);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BattleResult != null)
        {
            var controller = UIManager.Instance.GetController(BattleResult);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (WingInfoUi != null)
        {
            var controller = UIManager.Instance.GetController(WingInfoUi);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (AnswerUI != null)
        {
            var controller = UIManager.Instance.GetController(AnswerUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (FaceList != null)
        {
            var controller = UIManager.Instance.GetController(FaceList);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ChatItemList != null)
        {
            var controller = UIManager.Instance.GetController(ChatItemList);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (CleanDust != null)
        {
            var controller = UIManager.Instance.GetController(CleanDust);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (PuzzleImage != null)
        {
            var controller = UIManager.Instance.GetController(PuzzleImage);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (LineConfim != null)
        {
            var controller = UIManager.Instance.GetController(LineConfim);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (RechargeFrame != null)
        {
            var controller = UIManager.Instance.GetController(RechargeFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (PlayFrame != null)
        {
            var controller = UIManager.Instance.GetController(PlayFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (TitleUI != null)
        {
            var controller = UIManager.Instance.GetController(TitleUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (LevelUpTip != null)
        {
            var controller = UIManager.Instance.GetController(LevelUpTip);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

		if (MissionTip != null)
		{
			var controller = UIManager.Instance.GetController(MissionTip);
			if (controller != null)
			{
				controller.CleanUp();
			}
		}
        if (SevenDayReward != null)
        {
            var controller = UIManager.Instance.GetController(SevenDayReward);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (StrongUI != null)
        {
            var controller = UIManager.Instance.GetController(StrongUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (GuardUI != null)
        {
            var controller = UIManager.Instance.GetController(GuardUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ShareFrame != null)
        {
            var controller = UIManager.Instance.GetController(ShareFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
		if (RechargeActivityUI != null)
        {
            var controller = UIManager.Instance.GetController(RechargeActivityUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (WorshipFrame != null)
        {
            var controller = UIManager.Instance.GetController(WorshipFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
		if (OperationActivityFrame != null)
        {
			var controller = UIManager.Instance.GetController(OperationActivityFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
		}
		if (AcientBattleFieldFrame != null)
		{
			var controller = UIManager.Instance.GetController(AcientBattleFieldFrame);
			if (controller != null)
			{
				controller.CleanUp();
			}
 		}
        if (ModelDisplayFrame != null)
        {
            var controller = UIManager.Instance.GetController(ModelDisplayFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ShowItemsFrame != null)
        {
            var controller = UIManager.Instance.GetController(ShowItemsFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }            
        }
		if (FirstChargeFrame != null)
        {
            var controller = UIManager.Instance.GetController(FirstChargeFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (WingChargeFrame != null)
        {
            var controller = UIManager.Instance.GetController(WingChargeFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (QuickBuyUi != null)
        {
            var controller = UIManager.Instance.GetController(QuickBuyUi);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ArtifactUi != null)
        {
            var controller = UIManager.Instance.GetController(ArtifactUi);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MyArtifactUI != null)
        {
            var controller = UIManager.Instance.GetController(MyArtifactUI);
            if (controller != null)
            {
                controller.CleanUp();
            }            
        }
        if (MishiResultUI != null)
        {
            var controller = UIManager.Instance.GetController(MishiResultUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (BufferListUI != null)
        {
            var controller = UIManager.Instance.GetController(BufferListUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (EquipSkillTipUI != null)
        {
            var controller = UIManager.Instance.GetController(EquipSkillTipUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (BattryLevelUpUI != null)
        {
            var controller = UIManager.Instance.GetController(BattryLevelUpUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MieShiTapUI != null)
        {
            var controller = UIManager.Instance.GetController(MieShiTapUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if(ExchangeUI != null)
        {
            var controller = UIManager.Instance.GetController(ExchangeUI);
            if(controller != null)
            {
                controller.CleanUp();
            }
        }
        if (FuctionTipFrame != null)
        {
            var controller = UIManager.Instance.GetController(FuctionTipFrame);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (MieShiSceneMapUI != null)
        {
            var controller = UIManager.Instance.GetController(MieShiSceneMapUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (ChestInfoUI != null)
        {           
            var controller = UIManager.Instance.GetController(ChestInfoUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ClimbingTowerRewardUI != null)
        {
            var controller = UIManager.Instance.GetController(ClimbingTowerRewardUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
        if (ClimbingTowerUI != null)
        {
            var controller = UIManager.Instance.GetController(ClimbingTowerUI);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }

        if (MessageBoxEx != null)
        {
            var controller = UIManager.Instance.GetController(MessageBoxEx);
            if (controller != null)
            {
                controller.CleanUp();
            }
        }
    }

    private static Assembly ass = null; 
    public static void Init()
    {
        SceneMapUI = RegisterUIType(0, "SceneMapController", typeof(SceneMapArguments));
        MainUI = RegisterUIType(1, "MainUIController", typeof(MainUIArguments));
        BackPackUI = RegisterUIType(2, "BackPackController", typeof(BackPackArguments));
        AttriFrameUI = RegisterUIType(3, "AttriFrameController", typeof(AttriFrameArguments));
        EquipPack = RegisterUIType(4, "EquipPackController", typeof(EquipPackArguments));
        SelectRoleUI = RegisterUIType(6, "SelectRoleController", typeof(SelectRoleArguments));
        EquipComPareUI = RegisterUIType(7, "EquipCompareController", typeof(EquipCompareArguments));
        BlockLayer = RegisterUIType(8, "BottleneckLayerFrameCtrler", typeof(BlockLayerArguments));
        SkillFrameUI = RegisterUIType(9, "SkillFrameController", typeof(SkillFrameArguments));

		MissionFrame = RegisterUIType(10, "TaskFrameCtrler", typeof(MissionFrameArguments));
        DialogFrame = RegisterUIType(11, "DialogFrameController", typeof(DialogFrameArguments));
        TeamFrame = RegisterUIType(12, "TeamFrameController", typeof(TeamFrameArguments));
		MissionList = RegisterUIType(13, "TaskListCtrler", typeof(MissionListArguments));
        MissionTrackList = RegisterUIType(14, "UIMissionTrackListController", typeof(UIMissionTrackListArguments));
        ItemInfoUI = RegisterUIType(15, "ItemInfoController", typeof(ItemInfoArguments));
        OperationList = RegisterUIType(16, "OperationlistController", typeof(OperationlistArguments));
        MessageBox = RegisterUIType(17, "MessageBoxController", typeof(MessageBoxArguments));
        RewardFrame = RegisterUIType(18, "UIRewardFrameController", typeof(UIRewardFrameArguments));
        SmithyUI = RegisterUIType(56, "SmithyFrameController", typeof(SmithyFrameArguments));
        EquipUI = RegisterUIType(19, "EquipUIController", typeof(EquipUIArguments));
        AchievementFrame = RegisterUIType(20, "UIAchievementFrameController", typeof(UIAchievementFrameArguments));
        ChatMainFrame = RegisterUIType(21, "ChattingMajorFrameCtrler", typeof(ChatMainArguments));
        AchievementTip = RegisterUIType(22, "UIAchievementTipController", typeof(UIAchievementTipArguments));
        HandBook = RegisterUIType(24, "HandBookController", typeof(HandBookArguments));
        FriendUI = RegisterUIType(23, "FriendController", typeof(FriendArguments));
        DungeonUI = RegisterUIType(25, "DungeonController", typeof(DungeonArguments));
        ComposeUI = RegisterUIType(26, "ComposeController", typeof(ComposeArguments));
        AttributeUI = RegisterUIType(27, "AttributeController", typeof(AttributeArguments));
        EquipInfoUI = RegisterUIType(28, "EquipInfoController", typeof(EquipInfoArguments));
        SettingUI = RegisterUIType(29, "SettingController", typeof(SettingArguments));
        RankUI = RegisterUIType(30, "RankController", typeof(RankArguments));
        PlayerInfoUI = RegisterUIType(31, "PlayerInfoController", typeof(PlayerInfoArguments));
        MailUI = RegisterUIType(32, "MailController", typeof(MailArguments));
		CharacterUI = RegisterUIType(33, "CharacterInfoController", typeof(CharacterArguments));
        DepotUI = RegisterUIType(34, "DepotController", typeof(DepotArguments));
        StoreUI = RegisterUIType(35, "StoreController", typeof(StoreArguments));
        ElfUI = RegisterUIType(37, "ElfController", typeof(ElfArguments));
        WingUI = RegisterUIType(38, "WingController", typeof(WingArguments));
        SailingUI = RegisterUIType(39, "SailingController", typeof(SailingArguments));
        ServerListUI = RegisterUIType(41, "ServerListController", typeof(ServerListArguments));
        FarmUI = RegisterUIType(40, "FarmController", typeof(FarmArguments));
        MedalInfoUI = RegisterUIType(42, "MedalInfoController", typeof(MedalInfoArguments));
        DungeonResult = RegisterUIType(43, "DungeonResultController", typeof(DungeonResultArguments));
        AreanaUI = RegisterUIType(45, "ArenaController", typeof(ArenaArguments));
        AreanaResult = RegisterUIType(46, "ColiseumResultFrameCtrler", typeof(ArenaResultArguments));
        ReliveUI = RegisterUIType(47, "ReliveController", typeof(ReliveArguments));
        WishingUI = RegisterUIType(48, "AspirationFrameCtrler", typeof(WishingArguments));
        RecycleUI = RegisterUIType(49, "RecycleController", typeof(RecycleArguments));
        BattleUnionUI = RegisterUIType(50, "BattleUnionController", typeof(BattleUnionArguments));
        ForceUI = RegisterUIType(52, "ForceController", typeof(ForceArguments));
        BattleUI = RegisterUIType(53, "VersusFrameCtrler", typeof(BattleArguments));
        GainItemHintUI = RegisterUIType(54, "GainItemHintController", typeof(GainItemHintArguments));
        ElfInfoUI = RegisterUIType(55, "ElfInfoController", typeof(ElfInfoArguments));
        TradingUI = RegisterUIType(57, "TradingController", typeof(TradingArguments));
        AstrologyUI = RegisterUIType(58, "AstrologyController", typeof(AstrologyArguments));
        NewbieGuide = RegisterUIType(59, "UINewbieGuideController", typeof(UINewbieGuideArguments));
        ActivityUI = RegisterUIType(60, "ActivityController", typeof(ActivityArguments));
        ActivityRewardFrame = RegisterUIType(61, "ActiveAwardFarmeCtrler", typeof(ActivityRewardArguments));
        BossRewardFrame = RegisterUIType(62, "BossAwardFrameCtrler", typeof(BossRewardArguments));
        RebornUi = RegisterUIType(63, "RebornController", typeof(RebornArguments));
        BattleResult = RegisterUIType(64, "FightSettlementFrameCtrler", typeof(BattleResultArguments));
        OffLineExpFrame = RegisterUIType(65, "OffLineExpController", typeof(OffLineExpArguments));
        SystemNoticeFrame = RegisterUIType(66, "SystemNoticeController", typeof(SystemNoticeArguments));
        WingInfoUi = RegisterUIType(67, "WingInfoController", typeof(WingInfogArguments));
        ItemInfoGetUI = RegisterUIType(68, "ItemInfoGetController", typeof(ItemInfoGetArguments));
        DungeonRewardFrame = RegisterUIType(69, "DungeonRewardController", typeof(DungeonRewardArguments));
        
		var storeConfig = UIManager.Instance.GetController(StoreUI);
        StoreEquip = RegisterUIType(70, storeConfig, typeof(StoreArguments));
		CustomShopFrame = RegisterUIType(71, storeConfig, typeof(StoreArguments));
        AnswerUI = RegisterUIType(72, "AnswerQuestionFrameCtrler", typeof(AnswerArguments));
        StoreFarm = RegisterUIType(73, storeConfig, typeof(StoreArguments));
        FaceList = RegisterUIType(74, "FaceChainFrameCtrler", typeof(FaceListArguments));
        ChatItemList = RegisterUIType(75, "ChattingRecordsListCtrler", typeof(ChatItemListArguments));
        PuzzleImage = RegisterUIType(76, "PuzzleImageController", typeof(PuzzleImageArguments));
        CleanDust = RegisterUIType(77, "PurgeDirtFramectrler", typeof(CleanDustArguments));
        LineConfim = RegisterUIType(78, "LineConfirmController", typeof(LineConfirmArguments));
        RechargeFrame = RegisterUIType(79, "RechargeFrameController", typeof(RechargeFrameArguments));
        PlayFrame = RegisterUIType(80, "PlayFrameController", typeof (PlayFrameArguments));
        TitleUI = RegisterUIType(81, "TitleUIController", typeof(TitleUIArguments));
        LevelUpTip = RegisterUIType(82, "LevelUpTipController", typeof(LevelUpTipArguments));
		MissionTip = RegisterUIType(83, "MissionTipController", typeof(MissionTipArguments));
        SevenDayReward = RegisterUIType(84, "SevenRewardController", typeof(SevenDayRewardArguments));
        StrongUI = RegisterUIType(85, "StrongController", typeof(StrongArguments));
        GuardUI = RegisterUIType(86, "GuardController", typeof(GuardArguments));
        RechargeActivityUI = RegisterUIType(87, "RechargeActivityController", typeof(RechargeActivityArguments));
		ShareFrame = RegisterUIType(88, "ShareFrameController", typeof(ShareFrameArguments));
        WorshipFrame = RegisterUIType(89, "WorshipFrameController", typeof(WorshipFrameArguments));
		OperationActivityFrame = RegisterUIType(90, "UIOperationActivityFrameController", typeof(OperationActivityFrameArguments));
		AcientBattleFieldFrame = RegisterUIType(91, "AcientBattleFieldFrameCtrler", typeof(AcientBattleFieldFrameArguments)); 
        ModelDisplayFrame = RegisterUIType(92, "ModelDisplayController", typeof(UIInitArguments));
        ShowItemsFrame = RegisterUIType(93, "ShowItemsController", typeof(UIInitArguments));
        QuickBuyUi = RegisterUIType(94, "QuickBuyController", typeof(UIInitArguments));
        FuctionTipFrame = RegisterUIType(95, "FuctionTipsFrameController", typeof(FuctionTipFrameArguments));
        ArtifactUi = RegisterUIType(96, "ArtifactController", typeof(UIInitArguments));
        MyArtifactUI = RegisterUIType(97, "MyArtifactController", typeof(UIInitArguments));
        BufferListUI = RegisterUIType(98, "BufferListController", typeof(UIInitArguments));
        EquipSkillTipUI = RegisterUIType(99, "EquipSkillTipController", typeof(UIInitArguments));

		FirstChargeFrame = RegisterUIType(102, "FirstChargeController", typeof(FirstChargeFrameArguments));
        WingChargeFrame = RegisterUIType(103, "WingChargeController", typeof(UIInitArguments));

        MishiResultUI = RegisterUIType(115, "MieShiResultController", typeof(DungeonResultArguments));
        BattryLevelUpUI = RegisterUIType(116, "BattryUpgradeFrameCtrler", typeof(MonsterSiegeUIFrameArguments));


        MonsterSiegeUI = RegisterUIType(110, "MosterSiegeController", typeof(MonsterSiegeUIFrameArguments));
       
        GXRankingUI = RegisterUIType(111, "GXRankingUIController", typeof(GXRankingArguments));


        MieShiTapUI = RegisterUIType(112, "MieShiTapUIController", typeof(MieShiTapUIArguments));

        SkillTipFrameUI = RegisterUIType(117, "SkillFrameTipController", typeof(SkillTipFrameArguments));
        ExchangeUI = RegisterUIType(118, "CommutationFrameCtrler", typeof(ExchangeFrameArguments));
        MieShiSceneMapUI = RegisterUIType(119, "MieShiSceneMapController", typeof(UIInitArguments));
        
        ChestInfoUI = RegisterUIType(120, "ChestInfoController", typeof(ChestInfoUIArguments));

        ClimbingTowerRewardUI = RegisterUIType(121, "ClimbingTowerRewardController", typeof(BlockLayerArguments));
        ClimbingTowerUI = RegisterUIType(122, "ClimbingTowerController", typeof(UIInitArguments));

        MessageBoxEx = RegisterUIType(123, "MessageBoxExController", typeof(UIInitArguments));
        ass = null;
    }

    public UIInitArguments NewArgument()
    {
        return (UIInitArguments) Activator.CreateInstance(mArgumnetType);
    }

    public UIConfig(int uuid, IControllerBase controllerBase, Type argumentType)
    {
        UiRecord = Table.GetUI(uuid);
        if (UiRecord == null)
        {
            Logger.Error("not find Table->UI[{0}]", uuid);
            return;
        }
        TypeUI = (UIType)UiRecord.GroupId;
        Loction = new Vector3(UiRecord.posX, UiRecord.posY, UiRecord.posZ);
        if (controllerBase == null) return;
        UIManager.Instance.RegisterUI(this, controllerBase);
        controllerBase.State = FrameState.Close;
        SConfigs.Add(uuid, this);
        mArgumnetType = argumentType;
    }

    public static UIConfig GetConfig(int id)
    {
        UIConfig c;
        return SConfigs.TryGetValue(id, out c) ? c : null;
    }

}


public class UIManager : Singleton<UIManager>, IManager
{
    public GameObject UIRoot { get; set; }
    public Camera UICamera { get; set; }
    //UiRoot的不同类型的根节点
    private Transform[] mUiRootList = new Transform[(int)UIType.Type_Max];
    //做互斥用的
    private UIConfig[] mUiMutexList = new UIConfig[(int)UIType.Type_Max];
    //目前所有加载过的界面Frames
    private Dictionary<UIConfig, GameObject> mLayers = new Dictionary<UIConfig, GameObject>();
    //释放资源用的
    private Dictionary<UIConfig, IControllerBase> UIControllers = new Dictionary<UIConfig, IControllerBase>();
    //界面ID的管理器
    private Dictionary<int, UIConfig> uiConfigs = new Dictionary<int, UIConfig>();
    private Transform mBlockLayer;
    //操作历史，只记录pop层的历史
    class UIConfigRecord
    {
        public UIConfig Config;
        public UIInitArguments Args;
    }
    private List<UIConfigRecord> mRecordStack = new List<UIConfigRecord>();
	public GameObject HeadBoardRoot
	{
		get;
		private set;
	}
    public GameObject MainUIFrame
    {
        get
        {
            GameObject result;
            if (mLayers.TryGetValue(UIConfig.MainUI, out result))
            {
                return result;
            }
            Logger.Warn("MainUIFrame is Error!");
            return null;
        }
    }
    public UIManager()
    {
    }
    private void OnCloseUI(IEvent ievent)
    {
        Close_UI_Event e = ievent as Close_UI_Event;
        if (!UiVisible(e.config))
        {
            return;
        }
        CloseUI(e.config,e.IsBack);
    }
    private void OnShowUI(IEvent ievent)
    {
        Show_UI_Event e = ievent as Show_UI_Event;
        ShowUI(e.config, e.Args);

    }

    public void DestoryCloseUi()
    {
        var list = new List<UIConfig>();

        foreach (var o in mLayers)
        {
            var config = o.Key;
            if (config == UIConfig.MainUI)
            {
                continue;   
            }
            var controller = GetController(config);
            if (controller == null)
            {
                continue;
            }
            if (controller.State != FrameState.Close)
            {
                continue;
            }
            list.Add(config);
        }

        foreach (var config in list)
        {
            GameObject o;
            if (mLayers.TryGetValue(config, out o))
            {
                GameObject.Destroy(o);
                mLayers.Remove(config);
            }
        }
    }
    #region 查询方法
    public IControllerBase GetController(UIConfig config)
    {
        if (config == null)
        {
            return null;
        }
        IControllerBase controllerBase;
        if (UIControllers != null && UIControllers.TryGetValue(config, out controllerBase))
        {
            return controllerBase;
        }
        //Logger.Error("UIManage Not Init Add {0}", config.NameUI);
        return null;
    }

	public IControllerBase GetControllerById(int controllerId)
	{
		foreach (var pair in UIControllers)
		{
			if (controllerId == pair.Key.UiRecord.Id)
			{
				return pair.Value;
			}
		}
		return null;
	}

    public UIConfig GetUIbyId(int id)
    {
        UIConfig uiConfig;
        if (uiConfigs.TryGetValue(id, out uiConfig))
        {
            return uiConfig;
        }
        return null;
    }
    public GameObject GetLayer(UIConfig config)
    {
        GameObject obj;
        if (mLayers.TryGetValue(config, out obj))
        {
            return obj;
        }
        return null;
    }

    public Transform GetUIRoot(UIType eType)
    {
        //         if (eType == UIType.TYPE_BLOCK)
        //         {
        //             return mUiRootList[1];
        //         }
        return mUiRootList[0];//RootUI.transform;
        if (eType < UIType.TYPE_BASE || eType > UIType.Type_Max)
        {
            return null;
        }
        return mUiRootList[(int)eType];
    }
    #endregion
    #region 不需要使用者调用的
    public void RegisterUI(UIConfig config, IControllerBase controllerBase)
    {
        UIControllers.Add(config, controllerBase);
        uiConfigs.Add(config.UiRecord.Id, config);
    }

    public void SetActive(UIConfig config, bool isLook)
    {
        GameObject obj;
        if (mLayers.TryGetValue(config, out obj))
        {
            if (obj != null)
            {

                if (isLook)
                {
                    obj.transform.localPosition = config.Loction;
                    obj.SetActive(true);
                }
                else
                {
                    if (config.UiRecord.CleanRes == 1)
                    {
                        mLayers.Remove(config);
                        GameObject.Destroy(obj);
                    }
                    else
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }

    // 打开这个类型的界面
    private void ShowUIType(UIConfig config)
    {
        //打开主界面时，检查清空堆栈
        if (config.TypeUI == UIType.TYPE_BASE && mRecordStack.Count > 0)
        {
            foreach (var record in mRecordStack)
            {
                CloseUiBindRemove e = new CloseUiBindRemove(record.Config, 1);
                EventDispatcher.Instance.DispatchEvent(e);
            }
            mRecordStack.Clear();
        }

        if (config == UIConfig.MainUI)
        {
            //HideBlur();
        }
        else
        {
            if (config.TypeUI != UIType.TYPE_TIP
                && config.TypeUI != UIType.TYPE_BLOCK
                && config.TypeUI != UIType.TYPE_MESSAGE)
            {
                //ShowBlur();
            }
        }

        UIType type = config.TypeUI;
        if (type < 0 || type >= UIType.Type_Max)
        {
            Logger.Warn("OpenType = {0}", config.NameUI);
            return;
        }

        switch (type)
        {
            case UIType.TYPE_ITEM:
                break;
            case UIType.TYPE_BASE:
                {
                    CloseUIType(UIType.TYPE_POP);
                    CloseUIType(UIType.TYPE_BASE);
                    break;
                }
            case UIType.TYPE_POP:
                {
                    CloseUIType(UIType.TYPE_POP);
                    CloseUIType(UIType.TYPE_BASE);
                }
                break;
            case UIType.TYPE_TIP:
                break;
            case UIType.TYPE_MESSAGE:
                break;
            case UIType.TYPE_BLOCK:
                break;
            case UIType.Type_Max:
                break;
            default:
                throw new ArgumentOutOfRangeException("type");
        }
        if (UiVisible(config))
        {
            mUiMutexList[(int)type] = config;
            SetActive(config, true);
            if (GetUIRoot(type))
            {
                GetUIRoot(type).gameObject.SetActive(true);
            }
        }
    }

    // 关闭这个类型的界面
    private void CloseUIType(UIType type)
    {
        if (type < 0 || type >= UIType.Type_Max)
        {
            Logger.Warn("OpenType = {0}", (int)type);
            return;
        }
        var config = mUiMutexList[(int)type];
        if (config == null) return;
        if (type != UIType.TYPE_BASE)
        {
            CloseUiBindRemove e = new CloseUiBindRemove(config, 0);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        CloseUI(config,false,false);
    }

    public void ResetUIRoot(GameObject root, Camera camera = null)
    {
        mLayers.Clear();
        mRecordStack.Clear();
        UIRoot = root;
        UICamera = camera;

        var mUiMutexListLength0 =  mUiMutexList.Length;
        for (int i = 0; i <mUiMutexListLength0; i++)
        {
            mUiMutexList[i] = null;
        }
        {
            // foreach(var controller in UIControllers)
            var __enumerator1 = (UIControllers).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var controller = (KeyValuePair<UIConfig, IControllerBase>)__enumerator1.Current;
                {
					var sceneId = SceneManager.GetInstance().CurrentSceneTypeId;
                    controller.Key.CleanReferenceCount();
                    if (controller.Value.State == FrameState.Open)
                    {
                        controller.Value.State = FrameState.Close;
                    }
                    else if (controller.Value.State == FrameState.Loading)
                    {//之前正在打开的再打开一遍
                        controller.Value.State = FrameState.Close;
                        if (controller.Key != UIConfig.BlockLayer)
                        {
                            Game.Instance.StartCoroutine(ShowUICoroutine(controller.Key, null));    
                        }
                    }
                    controller.Value.OnChangeScene(sceneId);
                }
            }
        }
        var intUITypeType_Max1 =  (int)UIType.Type_Max;
        for (int i = 0; i <intUITypeType_Max1; i++)
        {
            mUiRootList[i] = UIRoot.transform.FindChild(((UIType)i).ToString());
            if (mUiRootList[i] == null)
            {
                mUiRootList[i] = AddObjToRoot(((UIType)i).ToString(), i == (int)UIType.TYPE_BASE).transform;

            }
        }

		HeadBoardRoot = GameObject.Find("HeadBoardRoot");
		if (null == HeadBoardRoot)
		{
			Logger.Warn("null == HeadBoardRoot");
		}


        //切场景后重置Brightness的状态
        var game = GameObject.Find("Game");
        if (null != game)
        {
            var con = game.GetComponentInChildren<BrightnessController>();
            if (con.GetBrightnessState() == BrightnessController.BrightnessState.Low)
            {
                con.CreateBlockingLayer();
            }
        }
    }

    public GameObject InitUI(UIConfig config, GameObject res)
    {
        Transform temp = GetUIRoot(config.TypeUI);
        if (temp == null) return null;

        var uiObj = GameObject.Instantiate(res) as GameObject;
        if (uiObj == null)
        {
            return null;
        }
        var t = uiObj.transform;
        t.parent = temp;
        t.localScale = Vector3.one;
        t.localPosition = config.Loction;

        // add a collider for this UI according to configuration.
        if (config.UiRecord.Swallow == 0) //config.UiRecord.Swallow
        {
            var collider = uiObj.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = uiObj.AddComponent<BoxCollider>();
            }

            collider.center = Vector3.zero;

            UIRoot root = t.root.GetComponent<UIRoot>();
            if (root != null)
            {
                var s = (float)root.activeHeight / Screen.height;

                collider.center = Vector3.zero;
                collider.size = new Vector3(Screen.width * s, Screen.height * s, 0f);    
            }
            else
            {
                Logger.Error("root not find");
            }
        }

        uiObj.SetActive(true);
        mLayers.Add(config, uiObj);
        return uiObj;
    }

    private void ShowBlur()
    {
        if (GameSetting.Instance == null)
        {
            return;
        }
        GameSetting.Instance.Blur = true;
        if (HeadBoardRoot)
        {
            HeadBoardRoot.SetActive(false);    
        }
    }
    private void HideBlur()
    {
        if (GameSetting.Instance == null)
        {
            return;
        }
        GameSetting.Instance.Blur = false;
        if (HeadBoardRoot)
        {
            HeadBoardRoot.SetActive(true);    
        }
    }
    public IEnumerator ShowUICoroutine(UIConfig config, UIInitArguments args = null)
    {
        if (config == null)
        {
            Logger.Log2Bugly("1 GetLayer(config) Argument =null ");
           yield break;
        }
        GameObject UIObj = GetLayer(config);
        config.IncReference();
        var controller = GetController(config);
	    //yield return new WaitForSeconds(2);
        if (controller == null)
        {
			Logger.Error("controller == null  [{0}]", config.NameUI);
            yield break;
        }
        if (config.TypeUI == UIType.TYPE_POP)
        {
//             if (mRecordStack.Count > 10)
//             {
//                 mRecordStack.RemoveAt(0);
//             }
            if (config.UiRecord.IsStack != 0 
                && (mRecordStack.Count == 0 || mRecordStack[mRecordStack.Count - 1].Config != config))
            {
                UIConfigRecord r = new UIConfigRecord();
                r.Config = config;
                r.Args = args;
                mRecordStack.Add(r);
            }
        }

        if (UIObj == null)
        {
            controller.State = FrameState.Loading;
            var ret = ResourceManager.PrepareResourceWithHolder<GameObject>(config.PrefabPath);
            yield return ret.Wait();
            if (ret.Resource == null)
            {
                Logger.Error("{0}", config.PrefabPath);
                controller.State = FrameState.Close;
                yield break;
            }
            if (controller.State == FrameState.Loading )
            {
                if (UIRoot && UIRoot.activeSelf == false)
                {
                    if (config == UIConfig.ForceUI)
                    {
                        controller.State = FrameState.Close;
                        yield break;
                    }
                }
                if (UiVisible(config))
                {
                    InitUI(config, ret.Resource);
                    ShowUIType(config);
                    controller.State = FrameState.Open;
                    controller.OnShow();
                    //统计打开页面
                    PlatformHelper.PageBegain(config.NameUI);
					UiFrameShowEvent e = new UiFrameShowEvent(config, args);
                    EventDispatcher.Instance.DispatchEvent(e);    
                }
                else
                {//重新进入场景后等到完全切换后处理
                    
                }    
            }
        }
        else
        {
            // if (UIObj.activeSelf == false)
            {
                ShowUIType(config);
                controller.State = FrameState.Open;
                controller.OnShow();
                //统计打开页面
                PlatformHelper.PageBegain(config.NameUI);
                UiFrameShowEvent e = new UiFrameShowEvent(config,args);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    private GameObject AddObjToRoot(string name, bool bBase)
    {
        var obj = new GameObject();
        var t = obj.transform;
        t.parent = UIRoot.transform;
        t.localPosition = Vector3.zero;
        t.localScale = Vector3.one;
        if (!bBase)
        {
            var collider = obj.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(1136, 1136, 0);
            obj.SetActive(false);
        }
        obj.tag = "UI";
        obj.name = name;

        return obj;
    }
    #endregion
    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="config"></param>
    /// <param name="data"></param>
    public void ShowUI(UIConfig config, UIInitArguments args = null)
    {
        IControllerBase controller = GetController(config);
        if (controller != null)
        {
            controller.RefreshData(args);
			if (FrameState.Close!=controller.State)
			{
				return;
			}
        }
		//umeng
        Game.Instance.StartCoroutine(ShowUICoroutine(config, args));
    }
    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="config"></param>
    private void CloseUI(UIConfig config,bool isBack = false,bool isEvent = true)
    {
        IControllerBase controller = GetController(config);
        config.DecReference();
        SetActive(config, false);
        controller.State = FrameState.Close;
        controller.Close();
        //统计关闭页面
        PlatformHelper.PageEnd(config.NameUI);
        var closeFrame = new UiFrameCloseEvent(config);
        EventDispatcher.Instance.DispatchEvent(closeFrame);
        mUiMutexList[(int)config.TypeUI] = null;
        if (config.TypeUI == UIType.TYPE_POP)
        {
            if (isBack)
            {
                if (mRecordStack.Count > 0)
                {
                    var record = mRecordStack[mRecordStack.Count - 1];
                    if (record.Config == config)
                    {
                        mRecordStack.RemoveAt(mRecordStack.Count - 1);
                    }
                }
                if (mRecordStack.Count > 0)
                {
                    var record = mRecordStack[mRecordStack.Count - 1];
                    var recordController = GetController(record.Config);
                    if (recordController != null)
                    {
                        GameObject UIObj = GetLayer(record.Config);
                        if (UIObj != null)
                        {
                            record.Config.IncReference();
                            recordController.State = FrameState.Open;
                            ShowUIType(record.Config);
                            recordController.OnShow();
                            UiFrameShowEvent e = new UiFrameShowEvent(record.Config);
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                        else
                        {
                            ShowUI(record.Config, record.Args);
                        }
                    }
                }
                else
                {
                    OpenDefaultFrame();
                }
            }
            else
            {
                if (isEvent)
                {
                    OpenDefaultFrame();    
                }
            }
        }
    }
    public void OpenDefaultFrame()
    {
        var Attributes = PlayerDataManager.Instance.PlayerDataModel.Attributes;
        if (ObjManager.Instance.MyPlayer == null)
        {
            ShowUI(UIConfig.MainUI);
            return;
        }
        bool isPvp = false;
		var tbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
        if (tbScene != null && tbScene.Type == (int)eSceneType.Pvp)
        {
            isPvp = true;
        }
        if (isPvp == false &&
            (ObjManager.Instance.MyPlayer.Dead
                || (Attributes.HpNow == 0 && Attributes.HpMax != 0)))
        {
            ShowUI(UIConfig.ReliveUI);
        }
        else
        {
            ShowUI(UIConfig.MainUI);
        }

    }
    public void OnClickEquipCell(IEvent e)
    {

    }
    public bool UiVisible(UIConfig config)
    {
        if (config == null)
        {
            return false;
        }
        return config.Visible();
    }

    private float mBlockLayerDuration = -1.0f;
    public void ShowBlockLayer(int i = 0)
    {
        mBlockLayerDuration = GameUtils.BlockLayerDuration;

        if (UiVisible(UIConfig.BlockLayer))
        {
            UIConfig.BlockLayer.IncReference();
            return;
        }
        ShowUI(UIConfig.BlockLayer, new BlockLayerArguments {Type = i});
    }
    public void RemoveBlockLayer(bool force = false)
    {
        if (UIConfig.BlockLayer==null)
        {
            return;
        }
        if (force)
        {
            UIConfig.BlockLayer.CleanReferenceCount();
            CloseUI(UIConfig.BlockLayer);
        }
        else
        {
            UIConfig.BlockLayer.DecReference();
            if (!UiVisible(UIConfig.BlockLayer))
            {
                CloseUI(UIConfig.BlockLayer);
            }
        }
    }
    public IEnumerator Init()
    {
        yield break;
    }
    public void Reset()
    {
        EventDispatcher.Instance.AddEventListener(Show_UI_Event.EVENT_TYPE, OnShowUI);
        EventDispatcher.Instance.AddEventListener(Close_UI_Event.EVENT_TYPE, OnCloseUI);

        Game.Instance.StopAllCoroutines();

        // 重置所有controller
        UIControllers.Clear();
        uiConfigs.Clear();
        UIConfig.Reset();
    }
    public void Tick(float delta)
    {
        {
            // foreach(var controller in UIControllers)
            var __enumerator3 = (UIControllers).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var controller = __enumerator3.Current.Value;
                {
                    //if (controller.State == FrameState.Close && __enumerator3.Current.Key != UIConfig.SkillFrameUI) continue;
                    if (controller is ScriptableController)
                    {
                        Profiler.BeginSample((controller as ScriptableController).GetName());
                        controller.Tick();
                        Profiler.EndSample();
                    }
                    else
                    {
                        controller.Tick();
                    }
                }
            }
        }

        if (mBlockLayerDuration > 0.0f)
        {
            mBlockLayerDuration -= delta;
            if (mBlockLayerDuration < 0.0f)
            {
                RemoveBlockLayer(true);
            }
        }
    }
    public void Destroy()
    {
		HeadBoardRoot = null;
        GameObject.Destroy(UIRoot);
    }
    /// <summary>
    /// 打开MessageBox
    /// </summary>
    /// <param name="boxType"></param>
    /// <param name="info"></param>
    /// <param name="title"></param>
    /// <param name="okAction"></param>
    /// <param name="cancelAction"></param>
    public void ShowMessage(MessageBoxType boxType, int dicId, string title = "", Action okAction = null,
        Action cancelAction = null, bool isSystemInfo = false, bool keepOpen = false)
    {
        ShowMessage(boxType, GameUtils.GetDictionaryText(dicId), title, okAction, cancelAction, isSystemInfo, keepOpen);
    }

    public void ShowMessage(MessageBoxType boxType, string info, string title = "", Action okAction = null,
        Action cancelAction = null, bool isSystemInfo = false, bool keepOpen = false)
    {
        IControllerBase controller = GetController(UIConfig.MessageBox);
        if (controller == null)
        {
            return;
        }
        controller.CallFromOtherClass("RefrehMessge",
            new object[] { boxType, info, title, okAction, cancelAction, isSystemInfo, keepOpen });

        RemoveBlockLayer();
        ShowUI(UIConfig.MessageBox);
    }

    public void ShowNetError(int errorCode, Action okAction = null)
    {
        var dicId = errorCode + 200000000;
        var tbDic = Table.GetDictionary(dicId);
        string info = "";
        if (tbDic == null)
        {
            info = GameUtils.GetDictionaryText(200000001) + errorCode;
            Logger.Error(GameUtils.GetDictionaryText(200098), errorCode);
        }
        else
        {
           // info = tbDic.Desc[GameUtils.LanguageIndex] + errorCode;
            info = tbDic.Desc[GameUtils.LanguageIndex];
        }
#if UNITY_EDITOR
        Logger.Error(info);
        ShowMessage(MessageBoxType.Ok, info, "", okAction);
#else
	    if (!string.IsNullOrEmpty(info))
	    {
			var e = new ShowUIHintBoard(info);
			EventDispatcher.Instance.DispatchEvent(e);    
	    }
		
#endif

	}
    public bool IsAttackState { get; set; }

}
