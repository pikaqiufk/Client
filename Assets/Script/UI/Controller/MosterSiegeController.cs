using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientDataModel.Annotations;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;
using DataContract;
using Umeng;

public class MosterSiegeController : IControllerBase
{

    #region 数据

    private int mNextFubenIdx = -1;
    public bool isMonster { get; set; }
	public MonsterDataModel MonsterModel;
    //State
    public FrameState State { get; set; }

    //显示boss或者怪物的形象
    public ObjFakeCharacter PetCharacter;
    //活动状态ID
    public int ActivityState { get; set; }


    public int mTowersID = 0;
    public int mTowerIndex = 0;

    public int CruActivityID;

    //tab页id
    public int mActivityId = -1;
    public int mMaxTowerUpTimes = 100;
    public int ActivityId
    {
        get { return mActivityId; }
        set
        {
            mActivityId = value;
            var playerData = PlayerDataManager.Instance;
            DateTime dt = Game.Instance.ServerTime.Date.AddDays(1);
            var sp = (dt - Game.Instance.ServerTime);
           
        }
    }

    //副本型活动的个数
    public const int DungeonActivityCount = 3;
    //Boss型活动的个数
    public const int BossActivityCount = 2;
    //滚动活动的个数
    public int ScrollActivityCount;

    //恶魔，血色，世界boss 的副本id（最小id，最大id，引导关id）
    public static readonly int[][] SceneIds =
    { new[] {5001, 5007, 5000},
        new[] {4001, 4007, 4000},
        new[] {6000, 6000, 6000},
        new[] {6110, 6110, 6110},
        new[] {6111, 6111, 6111},
        new[] {6112, 6112, 6112}
    };

    //各个类型副本的 TodayCount Exdata index
    public List<int> TodayCountExDataIndex = new List<int>();

    //
    public List<DynamicActivityRecord> DynamicActRecords;

    //
    public QueueUpDataModel QueueUpData
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.QueueUpData; }
    }

    private readonly bool[] mSkipOnce = { false, false, false };


    private Coroutine TimteRefresh;
    #endregion

    #region 初始化以及虚函数

	private static int ItemId = -1;
    public MosterSiegeController()
    {

        EventDispatcher.Instance.AddEventListener(UIEvent_ButtonClicked.EVENT_TYPE, OnBtnClicked);
        EventDispatcher.Instance.AddEventListener(ApplyPortraitAward_Event.EVENT_TYPE, ApplyPortraitAward);
        EventDispatcher.Instance.AddEventListener(MieShiSetActivityId_Event.EVENT_TYPE, SetActivityID);
        EventDispatcher.Instance.AddEventListener(MieShiGetInfo_Event.EVENT_TYPE, mieshishuju);
        EventDispatcher.Instance.AddEventListener(MonsterSiegeUpLevelBtn_Event.EVENT_TYPE, UpLevelBtn);
        EventDispatcher.Instance.AddEventListener(MieShiUpHpBtn_Event.EVENT_TYPE, UpHPBtn);
        EventDispatcher.Instance.AddEventListener(MieShiOnPaotaiBtn_Event.EVENT_TYPE, GetBatteryInfoData);
        EventDispatcher.Instance.AddEventListener(MieShiOnYibaomingBtn_Event.EVENT_TYPE, OnYibaomingBtn);
        EventDispatcher.Instance.AddEventListener(MieShiOnGXRankingBtn_Event.EVENT_TYPE, OnGXRankingBtn);
        EventDispatcher.Instance.AddEventListener(GXCortributionRank_Event.EVENT_TYPE, RefrashCortributionRank);
        EventDispatcher.Instance.AddEventListener(FubenGXCortributionRank_Event.EVENT_TYPE, RefrashFubrnCortributionRank);
        EventDispatcher.Instance.AddEventListener(PickUpNpc_Event.EVENT_TYPE, ApplyPickUpNpc);
        EventDispatcher.Instance.AddEventListener(ApplyMishiPortrait_Event.EVENT_TYPE, ApplyPortraitData);

        EventDispatcher.Instance.AddEventListener(MieShiShowSkillTip_Event.EVENT_TYPE, ShowSkillTip);
		EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnItemChange);
        EventDispatcher.Instance.AddEventListener(MieShiCLosePage_Event.EVENT_TYPE, ClosePage);
        EventDispatcher.Instance.AddEventListener(MieShiShowRankingReward_Event.EVENT_TYPE, ShowRankingRewardPage);
        EventDispatcher.Instance.AddEventListener(MieShiShowRules_Event.EVENT_TYPE, ShowRulesPage);
        EventDispatcher.Instance.AddEventListener(UIEvent_UpdateSkillAndHpEvent.EVENT_TYPE, UpdateTowerHpSkillType);
        EventDispatcher.Instance.AddEventListener(UIEvent_UpdateUseItemEvent.EVENT_TYPE, UpdateUserHpSkillType);

        EventDispatcher.Instance.AddEventListener(UIEvent_ClickTowerReward.EVENT_TYPE, OnClickTowerReward);
        


        // 
        isMonster = false;

        CleanUp();
    }


    public void ShowSkillTip(IEvent e)
    {
        if (isMonster)
        {
            var arg = new SkillTipFrameArguments();
            arg.idSkill = Table.GetBatteryLevel(MonsterModel.MonsterTowers[MonsterModel.MonsterTowerIdx].Level).BatterySkillId;
            var skill = Table.GetBatteryLevel(MonsterModel.MonsterTowers[MonsterModel.MonsterTowerIdx].Level + 1);
            if (skill != null)
            {
                arg.idNextSkill = skill.BatterySkillId;
            }
            else
            {
                arg.idNextSkill = -1;
            }
            arg.iLevel = MonsterModel.MonsterTowers[MonsterModel.MonsterTowerIdx].Level;



            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.SkillTipFrameUI, arg));
        }
    }


    public void OnShow()
    {
        EventDispatcher.Instance.DispatchEvent(new MieShiGetInfo_Event());   
    }

    public void Close()
    {
        
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args1 = data as MonsterSiegeUIFrameArguments;
        if (args1 != null)
        {
            isMonster = true;
            if (args1.Tab >= -1)
            {
                ActivityId = args1.Tab;


            }
            else
            {
                Logger.Error("Tab = {0}", args1.Tab);
            }
        }

	    SetItemCount();
    }





    public INotifyPropertyChanged GetDataModel(string name)
    {
        return MonsterModel;
    }

    public void CleanUp()
    {
		MonsterModel = new MonsterDataModel();

    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    #endregion

    #region 事件响应
    //界面上各种按钮的响应函数
    private void OnBtnClicked(IEvent ievent)
    {
        var e = ievent as UIEvent_ButtonClicked;
        switch (e.Type)
        {
            case BtnType.MieShiActivity_Queue:
                {
                    if (ActivityId  >= 1 && ActivityId <= MonsterModel.MonsterFubens.Count)
                    {
                        int FubenId = MonsterModel.MonsterFubens[ActivityId-1].Fuben.FubenId;
                        GameUtils.EnterFuben(FubenId);
                    }
                }
                break;
        }
    }
    public void mieshishuju(IEvent ievent)
    {
       
            NetManager.Instance.StartCoroutine(ApplyOfftheworldCoroutine());
            AddJIanliItem();
        
    }
    private IEnumerator ApplyOfftheworldCoroutine()
    {

        var msg = NetManager.Instance.ApplyMieShiData(PlayerDataManager.Instance.ServerId);
        yield return msg.SendAndWaitUntilDone();
        if (msg == null || msg.Response == null || msg.Response.Datas == null || msg.Response.Datas.Count == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new HiedMieShiIcon_Event());
        }
        else if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                // mieShiData = msg.Response;

                if (msg.Response.Datas.Count != 0)
                {
                    ///保存活动中炮塔guid信息
                    TowerGUIDFac.Clear();
                    
                    EventDispatcher.Instance.DispatchEvent(new MieShiAddActvityTime_Event(0, new DateTime()));
                    for (int i = 0; i < msg.Response.Datas.Count; i++)
                    {
                        CommonActivityInfo acd = msg.Response.Datas[i];

                        for (int j = 0; j < msg.Response.Datas[i].batterys.Count; j++)
                        {
                            ActivityBatteryOne abo = msg.Response.Datas[i].batterys[j];
                            TowerGUIDFac.Add(i * 1000 + abo.batteryId, abo.batteryGuid);
                        }
                        DateTime t = DateTime.FromBinary((long)acd.actiTime);
                        EventDispatcher.Instance.DispatchEvent(new MieShiAddActvityTime_Event(1, t));
                    }
                }

                var listInfoData = msg.Response.Datas;

                var nearlyId = msg.Response.currentActivityId;
                MonsterModel.CurActivityID = nearlyId;
                CruActivityID = nearlyId;

                MonsterModel.UIToggleSelect = -1;
                for (int i = 0;i<listInfoData.Count;i++)
                {
                    if (listInfoData[i] != null && listInfoData[i].activityId == nearlyId)
                    {
                        MonsterModel.UIToggleSelect = i;                     
                        EventDispatcher.Instance.DispatchEvent(new MieShiUiToggle_Event(MonsterModel.UIToggleSelect));
                        break;
                    }
                }

                if (MonsterModel.UIToggleSelect < 0)
                {
                    MonsterModel.UIToggleSelect = 0;
                    EventDispatcher.Instance.DispatchEvent(new MieShiUiToggle_Event(MonsterModel.UIToggleSelect));
                }

                List<ActivityBatteryOne> BatteryOne = msg.Response.Datas[nearlyId - 1].batterys;

                for (int i = 0; i < MonsterModel.MonsterFubens.Count; i++)
                {
                    /**if (MonsterModel.MonsterFubens[i].activity.ActivityId == msg.Response.currentActivityId)
                    {
                        MieShiFightingRecord fiting = Table.GetMieShiFighting(BatteryOne[nearlyId].level);
                        MonsterModel.MonsterFubens[i].activity.Fiting = fiting.LevelFighting;                        
                        break;
                    }**/
                }
                MieShiFightingRecord fiting = Table.GetMieShiFighting(BatteryOne[nearlyId].level);

                MonsterModel.Fiting = fiting.LevelFighting;

                for (int i = 0; i < listInfoData.Count; i++)
                {
                    var unit = listInfoData[i];
                    if (unit.activityId != nearlyId)
                        continue;
                    var temp = (long)unit.actiTime;
                    var dateTime = DateTime.FromBinary(temp);
                    MonsterModel.ActivityTime = dateTime;

                    MonsterModel.ActivityState = unit.state;
                    MonsterModel.BaoMingState = unit.applyState;
                }
                NetManager.Instance.StartCoroutine(ApplyPortraitDataMsg());
                
            }
        }

    }
    public void ApplyPortraitData(IEvent ievent)
    {
         NetManager.Instance.StartCoroutine(ApplyPortraitDataMsg());
        
    }
    

    private void ApplyPortraitAward(IEvent ievent)
    {
          var e = ievent as ApplyPortraitAward_Event;
            var idNpc = e.idNpc;
            NetManager.Instance.StartCoroutine(ApplyPortaitAwardMsg(idNpc));
        

    }


    public IEnumerator ApplyPortraitDataMsg()
    {
        var msg = NetManager.Instance.ApplyPortraitData(PlayerDataManager.Instance.ServerId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                PlayerDataManager.Instance.BattleMishiMaster = msg.Response;
                var e = new BattleMishiRefreshModelMaster(PlayerDataManager.Instance.BattleMishiMaster);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }


    #endregion

    #region 进入活动

    #region 检查进入活动的条件

    //检查结果
    public enum CheckConditionResult
    {
        None, //不符合条件
        Single, //符合条件，且是单人
        Team //符合条件，且是组队
    }

    //检查物品是否够
    private CheckConditionResult CheckItemEnough(FubenRecord tbFuben)
    {
        for (int i = 0, imax = tbFuben.NeedItemId.Count; i < imax; ++i)
        {
            var id = tbFuben.NeedItemId[i];
            var count = tbFuben.NeedItemCount[i];
            if (id == -1)
            {
                break;
            }
            if (PlayerDataManager.Instance.GetItemCount(id) < count)
            {
                //材料不足
                var tbItem = Table.GetItemBase(id);
                var content = string.Format(GameUtils.GetDictionaryText(270246), tbItem.Name);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, content, GameUtils.GetDictionaryText(1503),
                    () => { GameUtils.GotoUiTab(26, tbItem.Exdata[3]); });
                return CheckConditionResult.None;
            }
        }
        return CheckConditionResult.Single;
    }

    //检查进副本的条件（是队伍还是个人，以及是否满足进入条件）
    public CheckConditionResult CheckCondition(ActivityData activity)
    {
        // Team
        var tbFuben = Table.GetFuben(activity.Fuben.FubenId);
        var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
        var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);
        var teamCount = team.Count();
        if (teamCount == 0 || tbFuben.CanGroupEnter != 1)
        {
            return CheckSingleCondition(activity);
        }
        return CheckTeamCondition(activity);
    }

    //检查单人进副本的条件
    public CheckConditionResult CheckSingleCondition(ActivityData activity)
    {
        var fubenId = activity.Fuben.FubenId;
        var tbFuben = Table.GetFuben(fubenId);
        if (tbFuben == null)
        {
            Logger.Error("In CheckSingleCondition(), tbFuben == null, fubenId = {0}", fubenId);
            return CheckConditionResult.None;
        }

        var playerData = PlayerDataManager.Instance;
        var maxCount = tbFuben.TodayCount;
        if (tbFuben.AssistType == 4)
        {
            //恶魔
            maxCount += playerData.TbVip.DevilBuyCount;
        }
        else if (tbFuben.AssistType == 5)
        {
            //血色
            maxCount += playerData.TbVip.BloodBuyCount;
        }
        if (tbFuben != null && tbFuben.TodayCount != -1 && activity.EnterCount >= maxCount)
        {
            //副本次数达到上限
            GameUtils.ShowHintTip(490);
            return CheckConditionResult.None;
        }

        return CheckItemEnough(tbFuben);
    }

    //检查组队进副本的条件
    public CheckConditionResult CheckTeamCondition(ActivityData activity)
    {
        // Team
        var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
        var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);

        if (teamData.TeamList[0].Guid != ObjManager.Instance.MyPlayer.GetObjId())
        {
            //我不是队长
            GameUtils.ShowHintTip(440);
            return CheckConditionResult.None;
        }

        var fuben = activity.Fuben;
        var fubenId1 = fuben.FubenId;
        var tbFuben1 = Table.GetFuben(fubenId1);
        var tbScene1 = Table.GetScene(tbFuben1.SceneId);
        var tbScene2 = Table.GetScene(tbFuben1.SceneId + 1);
        var lvMin = tbScene1.LevelLimit;
        var lvMax = Constants.LevelMax;
        if (tbFuben1.OpenTime[0] != -1 && tbScene2 != null && tbScene2.Name == tbScene1.Name)
        {
            lvMax = tbScene2.LevelLimit;
        }

        //检查等级
        var name = string.Empty;
        {
            // foreach(var t in team)
            var __enumerator5 = (team).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var t = __enumerator5.Current;
                {
                    if (t.Level < lvMin || t.Level > lvMax)
                    {
                        name += t.Name + ",";
                    }
                }
            }
        }
        if (name.Length > 0)
        {
            name = name.Substring(0, name.Length - 1);
            EventDispatcher.Instance.DispatchEvent(
                new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(495), name)));
            return CheckConditionResult.None;
        }
        if (CheckSingleCondition(activity) == CheckConditionResult.None)
        {
            return CheckConditionResult.None;
        }
        return CheckConditionResult.Team;
    }

    #endregion

    //组队进入活动副本
    public void EnterTeamDungeon(int fubenId)
    {
        var tbFuben = Table.GetFuben(fubenId);
        if (tbFuben == null)
        {
            return;
        }

        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        if (sceneId == tbFuben.SceneId)
        {
            //已经在此副本当中了
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270081));
            return;
        }

        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            var oldTbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            var newTbScene = Table.GetScene(sceneId);

            if (oldTbScene != null && newTbScene != null)
            {
                if (oldTbScene.FubenId != -1 && newTbScene.FubenId != -1)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210123));
                    return;
                }
            }
        }
        NetManager.Instance.StartCoroutine(EnterTeamDungeonCoroutine(fubenId));
    }

    public IEnumerator EnterTeamDungeonCoroutine(int fubenId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.TeamEnterFuben(fubenId, PlayerDataManager.Instance.ServerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                }
                else if (DealWithErrorCode(msg.ErrorCode, fubenId, msg.Response.Items))
                {
                }
                else
                {
                    GameUtils.ShowNetErrorHint(msg.ErrorCode);
                    Logger.Error(".....EnterTeamDungeonCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....EnterTeamDungeonCoroutine.......{0}.", msg.State);
            }
        }
    }

    //预约活动副本

    //取消预约活动副本

    public static Dictionary<int, int> DealErrs = new Dictionary<int, int>
    {
        {(int) ErrorCodes.Error_LevelNoEnough, 300901},
        {(int) ErrorCodes.Error_FubenCountNotEnough, 466},
        {(int) ErrorCodes.ItemNotEnough, 467},
        {(int) ErrorCodes.Error_FubenRewardNotReceived, 497},
        {(int) ErrorCodes.Unline, 498},
        {(int) ErrorCodes.Error_CharacterOutLine, 498},
        {(int) ErrorCodes.Error_AlreadyInThisDungeon, 493},
        {(int) ErrorCodes.Error_CharacterCantQueue, 544}
    };

    //处理网络错误消息
    public bool DealWithErrorCode(int errCode, int fubenId, List<ulong> playerIds)
    {
        if (DealErrs.Keys.Contains(errCode))
        {
            var dicId = DealErrs[errCode];
            if (playerIds.Count <= 0)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicId));
            }
            else
            {
                var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
                var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);
                var players = team.Where(p => playerIds.Contains(p.Guid));
                var names = players.Aggregate(string.Empty, (current, p) => current + (p.Name + ","));
                if (names.Length <= 0)
                {
                    return true;
                }
                //特殊处理！！！
                if (errCode == (int)ErrorCodes.Error_LevelNoEnough)
                {
                    var tbFuben = Table.GetFuben(fubenId);
                    var assistType = (eDungeonAssistType)tbFuben.AssistType;
                    if (assistType == eDungeonAssistType.BloodCastle || assistType == eDungeonAssistType.DevilSquare)
                    {
                        var playerData = PlayerDataManager.Instance;
                        var fubenCount = playerData.GetExData(tbFuben.TotleExdata);
                        if (fubenCount > 0)
                        {
                            dicId = 489;
                        }
                        else
                        {
                            dicId = 491;
                        }
                    }
                }
                names = names.Substring(0, names.Length - 1);
                var content = string.Format(GameUtils.GetDictionaryText(dicId), names);
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(content));
            }
            return true;
        }
        return false;
    }

    #endregion

    #region 其它

    public object mFubenTimer;


	private void SetItemCount()
	{
		if (-1 == ItemId)
		{
			ItemId = Table.GetMieShiPublic(1).ItemId;
		}

		MonsterModel.ItemCount = PlayerDataManager.Instance.GetItemCount(ItemId);

	}

	private void OnItemChange(IEvent ievent)
	{
		var e = ievent as UIEvent_BagChange;
		if (e.HasType(eBagType.BaseItem))
		{
			SetItemCount();
		}
	}


    private void ClosePage(IEvent ievent)
    {
        if (MonsterModel != null)
        {
            MonsterModel.ShowPageId = 0;
        }
    }

    private void ShowRankingRewardPage(IEvent ievent)
    {
        if (MonsterModel != null)
        {
            MonsterModel.ShowPageId = 2;
        }
    }

    private void ShowRulesPage(IEvent ievent)
    {
        if (MonsterModel != null)
        {
            MonsterModel.ShowPageId = 1;
        }
    }


    private void UpdateTowerHpSkillType(IEvent ievent)
    {
        var e = ievent as UIEvent_UpdateSkillAndHpEvent;
        if (e != null)
        {
            MonsterModel.UpdateTowerType = e.Type;
            MonsterModel.UpdateUseItem = 0;
            try
            {
                if (MonsterModel.UpdateTowerType == 1)
                {//提升hp
                    string TowerName = Table.GetNpcBase(226000 + MonsterModel.MonsterTowerIdx).Name.ToString();
                    string UpdateDes = GameUtils.GetDictionaryText(300000122);
                    if (TowerName != null && UpdateDes != null)
                    {
                        MonsterModel.UpdatePrompt = string.Format(UpdateDes, TowerName);
                    }
                    
                }
                else if (MonsterModel.UpdateTowerType == 2)
                {
                    string TowerName = Table.GetNpcBase(226000 + MonsterModel.MonsterTowerIdx).Name.ToString();
                    string UpdateDes = GameUtils.GetDictionaryText(300000123);
                    if (TowerName != null && UpdateDes != null)
                    {
                        MonsterModel.UpdatePrompt = string.Format(UpdateDes, TowerName);
                    }
                    
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }

    private void UpdateUserHpSkillType(IEvent ievent)
    {
        MonsterModel.UpdateUseItem = 1 - MonsterModel.UpdateUseItem;
    }

    //用来计算一个副本活动的时间，并负责通知主界面显示活动提示
    public void CalculateFubenIdAndStartTime(int index)
    {

    }

    public void OnClickTowerReward(IEvent ievent)
    {
        MonsterModel.OnceReward.Clear();
        MonsterModel.StepReward.Clear();
        
        UIEvent_ClickTowerReward e = ievent as UIEvent_ClickTowerReward;
        
        var tb = Table.GetMieshiTowerReward(e.Index);
        for (int i = 0; i < tb.OnceRewardList.Count && i < tb.OnceNumList.Count; i++)
        {
            var temp = new BagItemDataModel();
            temp.ItemId = tb.OnceRewardList[i];
            temp.Count = tb.OnceNumList[i];
            MonsterModel.OnceReward.Add(temp);
        }
        for (int i = 0; i < tb.StepRewardList.Count && i < tb.StepNumList.Count; i++)
        {
            var temp = new BagItemDataModel();
            temp.ItemId = tb.StepRewardList[i];
            temp.Count = tb.StepNumList[i];
            MonsterModel.StepReward.Add(temp);
        }
        if (tb.TimesStep.Count > 0)
        {
            if (MonsterModel.MyUpTimes >= tb.TimesStep[0])
            {
                int temp = 1 << e.Index;
                if ((temp & MonsterModel.RewardFlag) == 0)
                {//领取逻辑
                    NetManager.Instance.StartCoroutine(AskGetTowerReward(e.Index));
                    return;
                }
            }
        }
        EventDispatcher.Instance.DispatchEvent(new UIEvent_TowerRewardCallBack(0, 0));
    }

    public IEnumerator AskGetTowerReward(int idx)
    {
        var msg = NetManager.Instance.ApplyGetTowerReward(PlayerDataManager.Instance.ServerId,MonsterModel.CurActivityID,idx);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                MonsterModel.RewardFlag = msg.Response;
                EventDispatcher.Instance.DispatchEvent(new UIEvent_TowerRewardCallBack(1, idx));
            }
        }
        yield break;
    }


    public void CalculateFubenIdAndStartTimeEX(int index)
    {
        if (index >= SceneIds.Length)
        {
            return;
        }
        if (index < 0)
        {
            Logger.Error("--------------Error!!!sceneIdx = {0}!!!----------------", index);
            return;
        }

        var playerData = PlayerDataManager.Instance;
        var myLevel = playerData.GetLevel();
        if (myLevel < 0)
        {
            return;
        }
        var ids = SceneIds[3];
        var tbScene = Table.GetScene(ids[index]);
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        var totalCount = playerData.GetExData(tbFuben.TotleExdata);

        var fubenId = -1;


        var activity = MonsterModel.MonsterFubens[index];
        var fuben = activity.Fuben;
        fubenId = fuben.FubenId;
        //   fuben.FubenId = fubenId;
        if (fubenId == -1)
        {
            activity.IsGrey = 1;
            return;
        }
        tbFuben = Table.GetFuben(fubenId);
        if (tbFuben == null)
        {
            activity.IsGrey = 1;
            return;
        }

        SetActivityConditionId(activity, tbFuben.EnterConditionId);
        ///初始化灭世之战表ID
        for (int i = 0; i < MonsterModel.MonsterFubens.Count; i++)
        {
            //MonsterModel.MonsterFubens[i].activity.ActivityId = i + 1;
        }

        #region 计算奖励

        var items = new Dictionary<int, int>();
        for (int i = 0, imax = tbFuben.RewardCount.Count; i < imax; ++i)
        {
            var itemId = tbFuben.DisplayReward[i];
            if (itemId == -1)
            {
                break;
            }
            var itemCount = tbFuben.RewardCount[i];
            items.modifyValue(itemId, itemCount);
        }
        var keys = items.Keys.ToList();

        for (int i = 0, imax = fuben.ItemId.Count, keyLen = keys.Count; i < imax; i++)
        {
            if (i < keyLen)
            {
                var key = keys[i];
                fuben.ItemId[i] = key;
                fuben.ItemCount[i] = items[key];
            }
            else
            {
                fuben.ItemId[i] = -1;
            }
        }

        #endregion

        activity.IsShowCount = tbFuben.TodayCount > 1;
        if (tbFuben.OpenTime[0] == -1)
        {
            activity.TimerState = 1;
            return;
        }

        //计算活动进入时间
        var now = Game.Instance.ServerTime;
        var skipOnce = mSkipOnce[index];
        var bMatchOpenTime = false;
        {
            // foreach(var time in tbFuben.OpenTime)
            var __enumerator6 = (tbFuben.OpenTime).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                if (skipOnce)
                {
                    skipOnce = false;
                    continue;
                }

                var time = __enumerator6.Current;
                {
                    var startTime = new DateTime(now.Year, now.Month, now.Day, time / 100, time % 100, 0);
                    if (startTime >= now)
                    {
                        //活动尚未开始进入
                        bMatchOpenTime = true;
                        activity.TargetTime = startTime;
                        activity.TimerState = 0;
                        break;
                    }

                    //判断是否在活动进入时间内
                    var endTime = startTime.AddMinutes(tbFuben.CanEnterTime);
                    if (endTime >= now)
                    {
                        bMatchOpenTime = true;
                        activity.TargetTime = endTime;
                        activity.TimerState = 1;

                        //通知主界面显示，活动副本的提示
                        EventDispatcher.Instance.DispatchEvent(new ShowActivityTipEvent(fubenId, 41000, startTime,
                            endTime));

                        break;
                    }
                }
            }
            AddJIanliItem();
        }
        //如果没有匹配上开启时间，则下一次开启时间应该是在第二天
        if (!bMatchOpenTime)
        {
            var time = tbFuben.OpenTime[0];
            var tarTime = new DateTime(now.Year, now.Month, now.Day, time / 100, time % 100, 0).AddDays(1);
            activity.TargetTime = tarTime;
            activity.TimerState = 0;
        }
        if (index == 2)
        {
            var worldBossTime = activity.TargetTime;
            fuben.StartTime = worldBossTime.Hour + ":" + worldBossTime.Minute;
        }
    }
    private void SetActivityConditionId(ActivityData activity, int id)
    {
        var result = PlayerDataManager.Instance.CheckCondition(id);
        activity.IsGrey = result != 0 ? 1 : 0;
        activity.ClickConditionId = id;
    }

    //用来计算一个BOSS活动的时间
    public void CalculateBossActTime(ActivityData bossUi)
    {
        var now = Game.Instance.ServerTime;
        var destTime = now.AddYears(10);
        foreach (var btn in bossUi.BtnList.Btns)
        {
            var tbWolrdBoss = Table.GetWorldBOSS(btn.TableId);
            var times = tbWolrdBoss.RefleshTime.Split('|');
            if (!times[0].Contains(':'))
            {
                continue;
            }
            var bMatchOpenTime = false;
            foreach (var t in times)
            {
                var tt = t.Split(':');
                var time = new DateTime(now.Year, now.Month, now.Day, tt[0].ToInt(), tt[1].ToInt(), 0);
                if (time >= now && destTime > time)
                {
                    bMatchOpenTime = true;
                    destTime = time;
                    break;
                }
            }
            if (!bMatchOpenTime)
            {
                var tt = times[0].Split(':');
                var time = new DateTime(now.Year, now.Month, now.Day, tt[0].ToInt(), tt[1].ToInt(), 0).AddDays(1);
                if (destTime > time)
                {
                    destTime = time;
                }
            }
        }
        bossUi.WaitTime = GameUtils.GetTimeDiffString(destTime, true);
        bossUi.TargetTime = destTime;
    }
    public Coroutine mCoSafeRemoveListener;
    public int NextQueueId;



    public IEnumerator ApplyPortaitAwardMsg(int idNpc)
    {
        var msg = NetManager.Instance.ApplyPortraitAward(PlayerDataManager.Instance.ServerId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
            }
        }
    }

        #endregion
    #region MieshiFun
    private Dictionary<int, ulong> TowerGUIDFac = new Dictionary<int, ulong>();
    private ulong GetTowerGuid(int id)
    {
        ulong guid = 0;
        TowerGUIDFac.TryGetValue(MonsterModel.MonsterFubenIdx * 1000 + id, out guid);
        return guid;
    }
    private void SetActivityID(IEvent e)
    {
        ActivityId = (e as MieShiSetActivityId_Event).ActivityID;

        MonsterModel.ActivityId = ActivityId;

        MieShiRecord record = Table.GetMieShi(ActivityId);
        if (record != null)
        {
            MonsterModel.FubenId = record.FuBenID;
            FubenRecord t= Table.GetFuben(MonsterModel.FubenId);
            if (t != null)
            {
                SceneRecord scene = Table.GetScene(t.SceneId);
                MonsterModel.NeedLevel = scene.LevelLimit;

                int BossId = t.StarRewardProb;
                EventDispatcher.Instance.DispatchEvent(new MieShiRefreshBoss_Event(BossId));
            }
            
        }
    }


    #region 灭世炮台技能提升
    public void UpLevelBtn(IEvent e)
    {

        MonsterSiegeUpLevelBtn_Event be = e as MonsterSiegeUpLevelBtn_Event;
            NetManager.Instance.StartCoroutine(RefreshiSkill(be.ID, be.BtnID));
        
    }

    public void ShowEffect()
    {
        MonsterModel.isSurc = true;
        NetManager.Instance.StartCoroutine(EndAnimation(1.1f));

    }
    private IEnumerator EndAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        MonsterModel.isSurc = false;
    }

    public IEnumerator RefreshiSkill(int batteryId, int LevelID)
    {
           ulong guid = GetTowerGuid(batteryId);
           var msg = NetManager.Instance.ApplyPromoteSkill(PlayerDataManager.Instance.ServerId,
           MonsterModel.CurActivityID,
           batteryId,
           guid,
           LevelID);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_CanNotPromote)
                {
                    //不在可提升阶段
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000027));
                }
                else if (msg.ErrorCode == (int)ErrorCodes.ItemNotEnough)
                {
                    //道具不足
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000030));
                }
                else if (msg.ErrorCode == (int)ErrorCodes.DiamondNotEnough)
                {
                    //钻石不足
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000031));
                }
                else if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_BatteryDestory)
                {
                    //炮台已摧毁
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000028));
                }
                else if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_MaxSkillLvl)
                {
                    //达到最高可提升的等级
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000032));
                }
                else if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    var battery = msg.Response.battery;
                    var LV = Table.GetMieShiPublic(1);
                
                    MonsterTowerDataModel mt = MonsterModel.MonsterTowers[battery.batteryId - 1];
                    var dateTime = DateTime.FromBinary((long)battery.skillLvlEndTime);
                    mt.SkillTime = dateTime;
                    mt.Level = battery.skillLevel; ;
                    mt.MyRanking = msg.Response.contribute;
                    mt.SkillDesc = Table.GetSkill(7499 + battery.skillLevel).Desc;
                    MonsterModel.MyGongxian = msg.Response.contribute;
                    ShowEffect();
                    SetTowerUpTimes(msg.Response.times);
                }
        }
    }
    #endregion
    #region 灭世炮台血量提升
    public void UpHPBtn(IEvent e)
    {
   //test     MonsterModel.SliderValue += 0.1f;
        NetManager.Instance.StartCoroutine(RefreshiHp());
    }
    
    public IEnumerator RefreshiHp()
    {
        var msg = NetManager.Instance.ApplyPromoteHP(PlayerDataManager.Instance.ServerId, MonsterModel.CurActivityID, MonsterModel.CurMonsterTowers.TowerId, MonsterModel.MyUpTimes+1);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {

            if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_CanNotPromote)
            {
                //不在可提升阶段 炮台已摧毁   达到最高可提升的血量  道具不足   钻石不足  达到最高可提升的等级
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000027));

            }
            else if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_BatteryDestory)
            {
                //炮台已摧毁
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000028));

                // DataModule.MonsterTowers[batteryId - 1].MomsterTower.BloodPer = 0;

            }
            else if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_MaxHP)
            {
                //达到最高可提升的血量
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000029));
            }
            else if (msg.ErrorCode == (int)ErrorCodes.ItemNotEnough)
            {
                //道具不足
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000030));
            }
            else if (msg.ErrorCode == (int)ErrorCodes.DiamondNotEnough)
            {
                //钻石不足
                var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                EventDispatcher.Instance.DispatchEvent(e);

                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000031));
            }
            else if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                var battery = msg.Response.battery;
                ShowEffect();

                for (int i = 0; i < MonsterModel.MonsterTowers.Count; i++)
                {
                    MonsterTowerDataModel mtdm = MonsterModel.MonsterTowers[i];
                    if (mtdm.TowerId == battery.batteryId)
                    {
                        mtdm.BloodCount = battery.curMaxHP;

                        break;
                    }
                }
                MonsterTowerDataModel mt2 = MonsterModel.MonsterTowers[battery.batteryId - 1];

                mt2.BloodCount = battery.curMaxHP;


                mt2.BloodPer = battery.promoteCount * 100 / (Table.GetMieShiPublic(1).MaxRaiseHP / 5);
                mt2.BloodBizhi = (float)mt2.BloodPer / 500f;

                MonsterModel.MyGongxian = msg.Response.contribute;
                SetTowerUpTimes(msg.Response.times);
            }
        }
    }
    #endregion
    #region 请求炮台灭世数据
    public void GetBatteryInfoData(IEvent e)
    {

            NetManager.Instance.StartCoroutine(ApplyBatteryInfoData());
        

    }
    public IEnumerator ApplyBatteryInfoData()
    {

        var msg = NetManager.Instance.ApplyBatteryData(PlayerDataManager.Instance.ServerId, MonsterModel.CurActivityID);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                Debug.Log("请求炮台信息成功");
                List<ActivityBatteryOne> BatteryOne = msg.Response.batterys;

                for (int j = 0; j < BatteryOne.Count; j++)
                {

                    MonsterTowerDataModel mt = MonsterModel.MonsterTowers[j];

                    // if (BatteryOne[j].curMaxHP <= BatteryOne[j].curMaxHP)
                    {
                        var battery = BatteryOne[j];
                        mt.BloodCount = battery.curMaxHP;
                        mt.Level = battery.skillLevel;
                        mt.TowerId = battery.batteryId;
                        var dateTime = DateTime.FromBinary((long)battery.skillLvlEndTime);
                        mt.SkillTime = dateTime;
                        mt.BloodPer = battery.promoteCount * 100 / (Table.GetMieShiPublic(1).MaxRaiseHP / 5);
                        mt.BloodBizhi = (float)mt.BloodPer / 500f;
                        mt.SkillDesc = Table.GetSkill(7499 + battery.skillLevel).Desc;
                    }
                }
                var itemid=  Table.GetMieShiPublic(1).ItemId;
                MonsterModel.UsePropItemId = itemid;
                MonsterModel.RewardFlag = msg.Response.flag;
                SetTowerUpTimes(msg.Response.times);

                EventDispatcher.Instance.DispatchEvent(new MieShiRefreshTowers_Event());
                MonsterModel.ShowPageId = 3;
            }
        }
    }
    #endregion
    float GetBloodPer(ActivityBatteryOne battery)
    {
        if (battery.maxHP != 0)
        {
            return (float)battery.curMaxHP / (float)battery.maxHP * 100f;

        }
        return 0;
    }
    #region 灭世排名的固定奖励显示
    private void AddJIanliItem()
    {
        if (MonsterModel != null)
        {
            MonsterModel.GongxianList.Clear();
            for (int i = 0; ; i++)
            {
                DefendCityDevoteRewardRecord dcrr = Table.GetDefendCityDevoteReward(i + 1);
                if (dcrr == null)
                {
                    break;
                }
                GongxianJianliItem jiangliItem = new GongxianJianliItem();
                if (i < 3)
                {
                    jiangliItem.NubIcon = dcrr.ContributionIcon.ToInt();
                }
                else
                {
                    jiangliItem.Numb = string.Format("{0} - {1}", dcrr.Rank[0].ToString(), dcrr.Rank[dcrr.Rank.Count - 1].ToString());
                }

                for (int j = 0; j < dcrr.RankItemCount.Count; j++)
                {
                    if (dcrr.RankItemID[j] > 0)
                    {
                        GongxianJianliItem.JiangliItem item = new GongxianJianliItem.JiangliItem();
                        item.IconId = dcrr.RankItemID[j];
                        ItemBaseRecord dbd = Table.GetItemBase(dcrr.RankItemID[j]);
                        item.Icon = dbd.Icon;
                        item.count = dcrr.RankItemCount[j].ToString();
                        jiangliItem.Rewards.Add(item);
                    }

                }
                MonsterModel.GongxianList.Add(jiangliItem);
            }
            MonsterModel.JifenList.Clear();
            for (int q = 0; ; q++)
            {
                DefendCityRewardRecord dcrr2 = Table.GetDefendCityReward(q + 1);
                if (dcrr2 == null)
                {
                    break;
                }
                GongxianJianliItem jiangliItem = new GongxianJianliItem();
                if (q < 3)
                {
                    jiangliItem.NubIcon = dcrr2.RankIcon.ToInt();
                }
                else
                {
                    jiangliItem.Numb = string.Format("{0} - {1}", dcrr2.Rank[0].ToString(), dcrr2.Rank[dcrr2.Rank.Count - 1].ToString());
                }
                for (int k = 0; k < dcrr2.RankItemCount.Count; k++)
                {
                    if (dcrr2.RankItemID[k] > 0)
                    {
                        GongxianJianliItem.JiangliItem item = new GongxianJianliItem.JiangliItem();
                        item.IconId = dcrr2.RankItemID[k];
                        ItemBaseRecord dbd = Table.GetItemBase(dcrr2.RankItemID[k]);
                        item.Icon = dbd.Icon;
                        item.count = dcrr2.RankItemCount[k].ToString();
                        jiangliItem.Rewards.Add(item);
                    }

                }
                MonsterModel.JifenList.Add(jiangliItem);
            }
        }
    }
    #endregion
    public void OnGXRankingBtn(IEvent e)
    {
       
            NetManager.Instance.StartCoroutine(ApplyContriRankingInfoData());
            AddJIanliItem();
        
    }
    #region 灭世排行数据
    public IEnumerator ApplyContriRankingInfoData()
    {
        var msg = NetManager.Instance.ApplyContriRankingData(PlayerDataManager.Instance.ServerId, MonsterModel.CurActivityID);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                var id = SceneManager.Instance.CurrentSceneTypeId;
                var tbScene = Table.GetScene(id);
                var rankData = msg.Response;
                var RankList = rankData.Datas;
                var entrys = MonsterModel.FubenContributionRank.Entrys;
                MonsterModel.MyRanking = msg.Response.MyRank;
                MonsterModel.MyGongxian = (msg.Response.MyRank != 0 ? msg.Response.Datas[msg.Response.MyRank - 1].value : 0);
                MonsterModel.MyName = PlayerDataManager.Instance.GetName();
                MonsterModel.MyGongxianItem = getMyCortributionRewardItemList(MonsterModel.MyRanking);
                entrys.Clear();
                for (int i = 0, imax = RankList.Count; i < imax; ++i)
                {

                    CortributionRankEntry cre = new CortributionRankEntry();
                    cre.Name = RankList[i].name;
                    cre.Rank = RankList[i].rank.ToString();
                    cre.Damage = RankList[i].value.ToString();
                    cre.ItemList = getMyCortributionRewardItemList(RankList[i].rank);
                    DefendCityDevoteRewardRecord CortributionData = Table.GetDefendCityDevoteReward(getIndexCortributionByRank(RankList[i].rank));

                    cre.IconId = CortributionData.ContributionIcon.ToInt();

                    entrys.Add(cre);
                }
               
            }
        }
    }
    #endregion

    #region 灭世报名
    public void OnYibaomingBtn(IEvent e)
    {
       
            NetManager.Instance.StartCoroutine(ApplyBaomingInfoData());
        
    }
    public IEnumerator ApplyBaomingInfoData()
    {
        var msg = NetManager.Instance.ApplyJoinActivity(PlayerDataManager.Instance.ServerId, MonsterModel.CurActivityID);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.Error_LevelNoEnough)
            {
                //等级不足
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210110));

            }

            else if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                Debug.Log("请求报名活动成功");
                var net = msg.Response;
                MonsterModel.BaoMingState = true;
            }
        }
    }
    #endregion
   

    private void ApplyPickUpNpc(IEvent ievent)
    {
        if (CruActivityID <= 0)
        {
            return;
        }
        var e = ievent as PickUpNpc_Event;
        var idNpc = e.idNpc;
        var ObjId = e.ObjId;
        NetManager.Instance.StartCoroutine(ApplyPickUpNpc(idNpc, ObjId));
    }
    public IEnumerator ApplyPickUpNpc(int idNpc,ulong ObjId)
    {
        var msg = NetManager.Instance.ApplyPickUpBox(PlayerDataManager.Instance.ServerId, CruActivityID, idNpc);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                var npc = ObjManager.Instance.FindObjByIdNoInvisable(ObjId);
                ((ObjCharacter)npc).PlayAnimation(OBJ.CHARACTER_ANI.HIT);
            }
            else if (msg.ErrorCode == (int)ErrorCodes.Error_MieShi_BossHadPickUp)
            {
                GameUtils.ShowHintTip(300000095);
            }
        }
    }

    #region 灭世贡献排行显示
    private void SetupFubenContributionEntry(RankingInfoOne resData, CortributionRankEntry toData)
    {
        if (toData == null || resData == null)
        {
            return;
        }
        toData.Name = resData.name;
        toData.Rank = Convert.ToString(resData.rank);
        toData.Damage = Convert.ToString(resData.value);
        toData.ItemList = getMyCortributionRewardItemList(resData.rank);
        toData.Show = true;
    }
    private GongxianJianliItem getMyCortributionRewardItemList(int iMyRank)
    {
        GongxianJianliItem MyCortributionRewardItemList = new GongxianJianliItem();
        int iCortributionDataId = getIndexCortributionByRank(iMyRank);
        if (iCortributionDataId >= 0)
        {
            DefendCityDevoteRewardRecord CortributionData = Table.GetDefendCityDevoteReward(iCortributionDataId);
            if (CortributionData != null)
            {
                MyCortributionRewardItemList.NubIcon = CortributionData.ContributionIcon.ToInt();

                for (int j = 0; j < CortributionData.RankItemCount.Count; j++)
                {
                    /**GongxianJianliItem.JiangliItem item = new GongxianJianliItem.JiangliItem();
                    ItemBaseRecord dbd = Table.GetItemBase(CortributionData.RankItemID[j]);
                    if (dbd.Icon <= 0)
                    {
                        continue;
                    }
                    item.IconId = dbd.Icon;
                    item.count = CortributionData.RankItemCount[j].ToString();

                    MyCortributionRewardItemList.Rewards.Add(item);
                    **/
                    if (CortributionData.RankItemID[j] > 0)
                    {
                        GongxianJianliItem.JiangliItem item = new GongxianJianliItem.JiangliItem();
                        ItemBaseRecord dbd = Table.GetItemBase(CortributionData.RankItemID[j]);
                        item.IconId = CortributionData.RankItemID[j];
                        item.Icon = dbd.Icon;
                        item.count = CortributionData.RankItemCount[j].ToString();

                        MyCortributionRewardItemList.Rewards.Add(item);
                    }
                }
            }
        }
        return MyCortributionRewardItemList;
    }
    private int getIndexCortributionByRank(int iRank)
    {
        int idxCortribution = -1;
        for (int i = 1; ; i++)
        {
            DefendCityDevoteRewardRecord CortributionData = Table.GetDefendCityDevoteReward(i);
            if (CortributionData == null)
            {
                break;
            }

            if (CortributionData.Rank[0] >= iRank && iRank <= CortributionData.Rank[1])
            {
                idxCortribution = i;
                break;
            }
        }
        return idxCortribution;
    }
    private void RefrashCortributionRank(IEvent ievent)
    {
        UndisableContributionList();
        var e = ievent as GXCortributionRank_Event;
        var rankData = e.RankData;
        var RankList = rankData.Datas;
        var entrys = MonsterModel.FubenContributionRank.Entrys;
        entrys.Clear();
        for (int i = 0, imax = RankList.Count; i < imax; ++i)
        {
            CortributionRankEntry cre = new CortributionRankEntry();
            cre.Name = RankList[i].name;
            cre.Rank = RankList[i].rank.ToString();
            entrys.Add(cre);
        }
    }
    private void RefrashFubrnCortributionRank(IEvent ievent)
    {
        UndisableFubenContributionList();
        var e = ievent as FubenGXCortributionRank_Event;
        var rankData = e.RankData;
        var RankList = rankData.Datas;
        var entrys = MonsterModel.FubenContributionRank.Entrys;

        entrys.Clear();
        for (int i = 0, imax = RankList.Count; i < imax; ++i)
        {

            CortributionRankEntry cre = new CortributionRankEntry();
            cre.Name = RankList[i].name;
            cre.Rank = RankList[i].rank.ToString();
            entrys.Add(cre);
        }

    }
    #endregion


    private void SetupContributionEntry(RankingInfoOne resData, BossRankEntry toData)
    {
        if (toData == null || resData == null)
        {
            return;
        }
        toData.Name = resData.name;
        toData.Rank = Convert.ToString(resData.value);
        toData.Damage = Convert.ToString(resData.value);
        toData.Show = true;
    }

    private void UndisableContributionList()
    {
        var entrys = MonsterModel.ContributionRank.Entrys;
        if (entrys == null)
        {
            return;
        }
        for (int i = 0, imax = entrys.Count; i < imax; ++i)
        {
            if (entrys[i] != null)
            {
                entrys[i].Show = false;
            }
        }
    }


    private void UndisableFubenContributionList()
    {
        var entrys = MonsterModel.FubenContributionRank.Entrys;
        if (entrys == null)
        {
            return;
        }
        for (int i = 0, imax = entrys.Count; i < imax; ++i)
        {
            if (entrys[i] != null)
            {
                entrys[i].Show = false;
            }
        }
    }

    private void SetTowerUpTimes(int nTimes)
    {
        MonsterModel.MyUpTimes = nTimes;
        
        float sliderBase = 0.0f;
        int id = 0;
        Table.ForeachMieshiTowerReward(tb =>
        {
            if (tb.TimesStep.Count == 2)
            {
                if (tb.TimesStep[0] <= nTimes+1 && tb.TimesStep[1] >= nTimes+1)
                {//消耗是下次的
                    MonsterModel.UseDiamond = tb.DiamondCost;
                    id = tb.Id;
                }
                if (tb.TimesStep[0] <= nTimes && tb.TimesStep[1] >= nTimes)
                {//进度是当前的  进度 = (当前阶段下线显示 + (当前次数 - 当前阶段次数下限) / (当前阶段上限 - 当前阶段下限) * (当前显示上限-当前显示下限) ) /100.0f
                    float baseStep = (float) tb.ShowValue[0];
                    float difTimes = (float) nTimes - tb.TimesStep[0];
                    float difStep = (float) tb.TimesStep[1] - tb.TimesStep[0];
                    float difShow = (float) tb.ShowValue[1] - tb.ShowValue[0];
                    MonsterModel.SliderValue = (baseStep + difTimes/difStep*difShow)/100.0f; 
                }
                if (tb.TimesStep[0] == nTimes && (MonsterModel.RewardFlag & (1 << tb.Id)) == 0)
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_TowerRewardCallBack(2, tb.Id));
                }
            }
            return true;
        });
    }


    /// laojiao end
    #endregion
}
