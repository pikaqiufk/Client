#region using

using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using EventSystem;
using SignalChain;
using UnityEngine;
using ClientDataModel;
using ClientService;
using DataContract;
using Mono.Collections.Generic;
#endregion

namespace GameUI
{
    public class MainScreenFrame : MonoBehaviour, IChainRoot, IChainListener
    {
        private const float BlockContinueTime = 8.0f;
        //是否已经准备过字
        private static bool s_isPretreatString;
        public GameObject Arrow;
        public BindDataRoot Binding;
        //block input
        public UIWidget BlockInputWidget;
        public Transform BuffRoot;
        public Transform CharCursor;
        private readonly Dictionary<ulong, ListItemLogic> itemLogicDict = new Dictionary<ulong, ListItemLogic>();
        public GameObject DownArrow;
        public GameObject ExpLable;
        public UILabel FightReady;
        private Coroutine HideExpCo;
        public GameObject JoyStick;
        public List<StackLayout> Layout;
        private float currentBlockTime;
        private bool isEnable;
        private bool showSKill;
        public GameObject MissionRoot;
        private int learnSkillID = -1;
        private Coroutine countdownCoroutine;
        private DateTime countdownTime;
        private eCountdownType countdownType;
        private bool displayCountdownTimer;
        //offlineframe
        public Transform OffLineRoot;
        public GameObject PkBg;
        public GameObject AutoFightBtn;
        public List<UIButton> PkBtn;
        public GameObject RadarTitle;
        public GameObject SkillBar;
        public UISpriteAnimation SkillEffectAni;
        //buff list
        public List<Transform> SkillList;
        // move learn skill
        public UISprite SkillMove;
        public UILabel SkillName;
        public TweenAlpha SkillNameTween;
        public Transform SkillTarget;
        public UIToggle Team;
        public GameObject Transition;
        public GameObject PlayerHpTransition;
        private int PlayTransitionTime;
        public GameObject UpArrow;
        public StackLayout MissionLayout;
        public GameObject FirstEnterGameObject;

        public UILabel TimeLabel;

        public GameObject LiuGuang;

        public GameObject MieshiBtn;

        public int m_ActivityState;

        public UILabel BossTalk;
        public float mBossTalkTimer = -1.0f;
        public float Playerhp = 50;
	    public GameObject HideBtn;
		public GameObject ShowBtn;
	    public GameObject MainBtnRoot;

		//public GameObject 
	    //-------------------------------------------------PkModel
	    public void ChangePkModel(int value)
	    {
            
	        PlayerDataManager.Instance.ChangePkModel(value);
	        ChangeModel(false);
	    }
	
	    private void SetupBufferList()
	    {
	        ComplexObjectPool.NewObject("UI/MainUI/BuffList.prefab", gameObject =>
	        {
	            if (null != BuffRoot)
	            {
	                var objTransform = gameObject.transform;
	                //objTransform.parent = BuffRoot;
	                objTransform.SetParentEX(BuffRoot);
	                objTransform.localScale = Vector3.one;
	                objTransform.localPosition = Vector3.zero;
	                gameObject.SetActive(true);
	            }
	        });
	    }
	
	    private void CreateOffineFrame()
	    {
	        if (null == OffLineRoot)
	        {
	            return;
	        }
	
	        ComplexObjectPool.NewObject("UI/OffLineExp/OffLineExpFrame.prefab", gameObject =>
	        {
	            if (null != OffLineRoot)
	            {
	                var objTransform = gameObject.transform;
	                //objTransform.parent = OffLineRoot;
	                objTransform.SetParentEX(OffLineRoot);
	                objTransform.localScale = Vector3.one;
	                objTransform.localPosition = Vector3.zero;
	                gameObject.SetActive(true);
	            }
	        });
	    }
	
	    public void CreateOffineFrameAndBegainExesering()
	    {
	        //EventDispatcher.Instance.DispatchEvent(new Ui_OffLineFrame_SetVisible(true));
	    }
	
	    private void PlayBuffIncreaseAnim(IEvent ievent)
	    {
	        if (null != BuffRoot)
	        {
	            var buffLogic = BuffRoot.GetComponentInChildren<MainBufferList>();
	            if (null != buffLogic && buffLogic.BuffAnimation != null)
	            {
	                if (!buffLogic.BuffAnimation.IsPlaying("BuffIncrease"))
	                {
	                    buffLogic.BuffAnimation.Play("BuffIncrease");
	                }
	            }
	        }
	    }
	
	    private IEnumerator HideExpLableCoroutine()
	    {
	        yield return new WaitForSeconds(1.8f);
	        ExpLable.SetActive(false);
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
// 	        if (isEnable)
// 	        {
// 	            {
// 	                var __list2 = Layout;
// 	                var __listCount2 = __list2.Count;
// 	                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
// 	                {
// 	                    var layout = __list2[__i2];
// 	                    {
// 	                        layout.ResetLayout();
// 	                    }
// 	                }
// 	            }
// 	            isEnable = false;
// 	        }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void OnActivityTipClicked()
	    {
	        EventDispatcher.Instance.DispatchEvent(new ActivityTipClickedEvent());
	    }
	
	    public void OnClickAuto()
	    {
	        var e = new MainUiOperateEvent(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClosePkModel()
	    {
	        ChangeModel(false);
	    }
	
	    public void OnClickContactChat()
	    {
	        var arg = new FriendArguments();
	        arg.Type = 2;
	        var e = new Show_UI_Event(UIConfig.FriendUI, arg);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickDailyActivity()
	    {
	        var e = new Show_UI_Event(UIConfig.PlayFrame);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickDungeonAuto()
	    {
	        var e = new MainUiOperateEvent(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
        public void OnClickFastReach()
        {
            var e = new ClickReachBtnEvent();
            EventDispatcher.Instance.DispatchEvent(e);
        }
	
	    public void OnClickDurable()
	    {
	        var e = new MainUiOperateEvent(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickExitDungeon()
	    {
	        var e = new DungeonBtnClick(100, eDungeonType.Invalid);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickHead()
	    {
	        var e = new Show_UI_Event(UIConfig.CharacterUI, new CharacterArguments());
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickInspire()
	    {
	        var e = new MainUiOperateEvent(5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMedicineWarn()
	    {
	        var arg = new StoreArguments {Tab = 0};
	        var e = new Show_UI_Event(UIConfig.StoreUI, arg);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
        public void OnClickFastReachMessageBoxOK()
        {
            var e = new OnClickFastReachMessageBoxOKEvent();
	        EventDispatcher.Instance.DispatchEvent(e);
            
        }

        public void OnClickFastReachMessageBoxCancle()
        {
            var e = new OnClickFastReachMessageBoxCancleEvent();
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnClickPetIslandBuyTili()
        {
            EventDispatcher.Instance.DispatchEvent(new OnClickBuyTiliEvent(0));
        }
	
	
	    public void OnClickMinimap()
	    {
            var e = new ShowSceneMapEvent(-1);
	        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
	        {
	            var sceneId = GameLogic.Instance.Scene.SceneTypeId;
	            var tbScene = Table.GetScene(sceneId);
	            if (tbScene == null)
	            {
	                return;
	            }
	            if (tbScene.IsOpenMap == 0)
	            {
	                return;
	            }

                e.SceneId = tbScene.Id;

	        }

	        EventDispatcher.Instance.DispatchEvent(e);
	    }

        public void OnClickModel()
	    {
	        var flag = !PkBg.activeSelf;
	
	        ChangeModel(flag);
	    }
	
	    public void OnClickRardarTitle()
	    {
	        OnClickMinimap();
	//         RadarTitle.SetActive(!RadarTitle.activeSelf);
	//         if (RadarTitle.activeSelf)
	//         {
	//             UpArrow.SetActive(true);
	//             DownArrow.SetActive(false);
	//         }
	//         else
	//         {
	//             UpArrow.SetActive(false);
	//             DownArrow.SetActive(true);
	//         }
	    }
	
	    public void OnClickShowRechargeFrame()
	    {
	        var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments {Tab = 1});
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSwitch()
	    {
	        showSKill = !showSKill;
	        ChangeState(showSKill);
	    }
	
	    public void OnClickUseMedicine()
	    {
	        var e = new MainUiOperateEvent(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(ShowCountdownEvent.EVENT_TYPE, OnEvent_ShowCountdown);
	        EventDispatcher.Instance.RemoveEventListener(SkillEquipMainUiAnime.EVENT_TYPE, OnEvent_EquipSkillAnim);
            EventDispatcher.Instance.RemoveEventListener(MainUiCharRadar.EVENT_TYPE, OnMainUiCharRadar);
            EventDispatcher.Instance.RemoveEventListener(UIEvent_BuffIncreaseAnimation.EVENT_TYPE, PlayBuffIncreaseAnim);
	        EventDispatcher.Instance.RemoveEventListener(UI_BlockMainUIInputEvent.EVENT_TYPE, OnEvent_BlockMainScreen);
	        EventDispatcher.Instance.RemoveEventListener(SceneTransition_Event.EVENT_TYPE, OnEvent_SceneChange);
			EventDispatcher.Instance.RemoveEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent_UpdateMission);
            EventDispatcher.Instance.RemoveEventListener(FirstEnterGameEvent.EVENT_TYPE, OnEvent_ShowFirstEnterGame);
			EventDispatcher.Instance.RemoveEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, OnNewFuncionGuideEvent);
			EventDispatcher.Instance.RemoveEventListener(ShowPopTalk_Event.EVENT_TYPE, ShowBossTalk);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        var evt = new UIEvent_SkillButtonReleased(false, 0);
	        EventDispatcher.Instance.DispatchEvent(evt);
	
	        if (learnSkillID != -1)
	        {
	            PlayerDataManager.Instance.LearnSkill(learnSkillID);
	            learnSkillID = -1;
	        }
	        //防止重新进来时被阻止输入
	        BlockMainScreen(false);
            BossTalk.gameObject.SetActive(false);
	        StopTransition();
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
            if (!EventDispatcher.Instance.HasEventListener(MainUiCharRadar.EVENT_TYPE, OnMainUiCharRadar))
            {
                EventDispatcher.Instance.AddEventListener(MainUiCharRadar.EVENT_TYPE, OnMainUiCharRadar);
            }

            FightReady.gameObject.SetActive(false);
	        PkBg.SetActive(false);
	        var myPlayer = ObjManager.Instance.MyPlayer;
	        if (null != myPlayer)
	        {
	            if (myPlayer.IsInSafeArea())
	            {
	                showSKill = false;
	            }
	            else
	            {
	                showSKill = true;
	            }
	            ChangeState(showSKill);
	        }
	
	        SkillMove.gameObject.SetActive(false);
	        learnSkillID = -1;
	        isEnable = true;
	
	        if (countdownTime > Game.Instance.ServerTime)
	        {
	            if (countdownCoroutine != null)
	            {
	                StopCoroutine(countdownCoroutine);
	            }
	            if (displayCountdownTimer)
	            {
	                countdownCoroutine = StartCoroutine(ShowCountDown());
	            }
	        }
	        currentBlockTime = 0;

            IControllerBase monsterController = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI);
           

             EventDispatcher.Instance.DispatchEvent(new ShowComposFlag_Event());
		    
#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void OnExpHoverOut()
	    {
	        HideExpCo = NetManager.Instance.StartCoroutine(HideExpLableCoroutine());
	    }
	
	    public void OnExpHoverOver()
	    {
	        ExpLable.SetActive(true);
	        if (HideExpCo != null)
	        {
	            NetManager.Instance.StopCoroutine(HideExpCo);
	        }
	    }
	
	    public void OnInspireCancel()
	    {
	        var e = new MainUiOperateEvent(7);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnInspireDia()
	    {
	        var e = new MainUiOperateEvent(9);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnInspireGold()
	    {
	        var e = new MainUiOperateEvent(8);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnInspireOk()
	    {
	        var e = new MainUiOperateEvent(6);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }

        public void OnFirstEnterGameOk()
        {
            var e = new MainUiOperateEvent(10);
            EventDispatcher.Instance.DispatchEvent(e);
        }
	
	    private void OnMainUISwithState(IEvent ievent)
	    {
	        var e = ievent as MainUISwithState;
	        showSKill = e.IsAttack;
	        ChangeState(showSKill);
	    }
	
	    public void OnMissionClick()
	    {
	        if (!Team.value)
	        {
	            var scene = GameLogic.Instance.Scene;
	            if (null != scene)
	            {
	                if (null != scene.TableScene)
	                {
	                    if (-1 == scene.TableScene.FubenId)
	                    {
	                        var e = new Show_UI_Event(UIConfig.MissionList);
	                        EventDispatcher.Instance.DispatchEvent(new Event_MissionList_TapIndex(1));
	                        EventDispatcher.Instance.DispatchEvent(e);
	                    }
	                }
	            }
	        }
	    }
	
        public void OnMishiMissionClick()
        {
            EventDispatcher.Instance.DispatchEvent(new Event_ShowMieshiFubenInfo());
        }

        public void OnMishiRankingClick()
        {
            EventDispatcher.Instance.DispatchEvent(new Event_ShowMieshiRankingInfo());
        }


	    public void OnRechageActivity()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeActivityUI));
	    }
	
	    public void OnRewardFastKey()
	    {
	        PlayerDataManager.Instance.RewardGotoUI();
	    }
	
	    private void OnEvent_SceneChange(IEvent ievent)
	    {
	        if (!gameObject.active)
	        {
	            return;
	        }
	
	        if (null != Transition)
	        {
	            Transition.SetActive(true);
	            var tween = Transition.GetComponentInChildren<TweenAlpha>();
	            if (null != tween)
	            {
	                tween.ResetToBeginning();
	                tween.SetOnFinished(() => {
                        
                        if (PlayTransitionTime < 5)
                        {
                            tween.ResetToBeginning();
                            tween.PlayForward();
                        }
                        else
                        {
                            PlayTransitionTime = 0;
                            Transition.SetActive(false);
                        }
                        PlayTransitionTime++;

                    });
	                tween.PlayForward();
	            }
	        }
	    }

		private void OnEvent_UpdateMission(IEvent ievent)
		{
			if (null != MissionLayout)
			{
				MissionLayout.ResetLayout();
			}
		}

        private void OnMainUiCharRadar(IEvent ievent)
	    {
            var e = ievent as MainUiCharRadar;
            if (e == null)
                return;

            var type = e.Type;
            var data = e.DataModel;
            if (type == 1)
            {
                CreateCharRadar(data);
            }
            else
            {
                RemoveCharRadar(data);
            }
	     }
	     private void CreateCharRadar(RararCharDataModel data)
	     {
	         var id = data.CharacterId;
	         ComplexObjectPool.NewObject("UI/MainUI/CharCursor.prefab", o =>
	         {
	             if (data.CharType != 0)
	             {
	                 var charObj = ObjManager.Instance.FindCharacterById(id);
	                 if (charObj == null || charObj.Dead)
	                 {
	                     ComplexObjectPool.Release(o, false, false);
	                     return;
	                 }
	             }
	             var oTransform = o.transform;
	             //oTransform.parent = CharCursor.transform;
	             oTransform.SetParentEX(CharCursor.transform);
	             oTransform.localScale = Vector3.one;
                 if(!o.activeSelf)
	                o.SetActive(true);
	             var i = o.GetComponent<ListItemLogic>();
	             i.Item = data;
	             var r = o.GetComponent<BindDataRoot>();
	             r.Source = data;

                 itemLogicDict[data.CharacterId] = i;
	         }, null, null, false, false, false);
	     }
	     private void RemoveCharRadar(RararCharDataModel data)
	     {
	         ListItemLogic obj;
	         if (itemLogicDict.TryGetValue(data.CharacterId, out obj))
	         {
                 ComplexObjectPool.Release(obj.gameObject, false, false);
                 itemLogicDict.Remove(data.CharacterId);
	         }
	     }

         private void OnEvent_ShowCountdown(IEvent ievent)
	    {
	        var e = ievent as ShowCountdownEvent;
	        switch (e.Type)
	        {
	            case eCountdownType.BattleFight:
	            case eCountdownType.BattleRelive:
	            {
	                if (countdownCoroutine != null)
	                {
	                    StopCoroutine(countdownCoroutine);
	                }
	                displayCountdownTimer = true;
	                countdownTime = e.Time.AddSeconds(1);
	                countdownType = e.Type;
	                if (gameObject.activeSelf)
	                {
	                    countdownCoroutine = StartCoroutine(ShowCountDown());
	                }
	            }
	                break;
	            default:
	                break;
	        }
	    }
	
	    public void OnShowFirstCharge()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.FirstChargeFrame));
        }

        public void OnFunctionOnShow()
        {
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.FuctionTipFrame));
        }

        public void OnShowWingCharge()
        {
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.WingChargeFrame));
        }
	
	    public void OnShowGuardFrame()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.GuardUI));
	    }
	
	    private void OnEvent_BlockMainScreen(IEvent ievent)
	    {
	        var e = ievent as UI_BlockMainUIInputEvent;
	        var duration = e.Duration;
	        BlockMainScreen(duration > 0);
	    }
	
	    public void OnShowSevenDay()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.SevenDayReward));
	    }
	
	    public void OnShowStrong()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.StrongUI));
	    }
        public void OnBatteryLevelUpBtn()
        {
            Show_UI_Event eventMonster = new Show_UI_Event(UIConfig.BattryLevelUpUI);
            EventDispatcher.Instance.DispatchEvent(eventMonster);
        EventDispatcher.Instance.DispatchEvent(new MieShiGetInfo_Event());
        }
	
	    public void OnShowTargetMenu()
	    {
	        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	        var localPos = SkillTarget.transform.root.InverseTransformPoint(worldPos);
	        UIConfig.OperationList.Loction = localPos;
	
	        var e = new MainUiOperateEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }

        private void OnEvent_ShowFirstEnterGame(IEvent ievent)
        {
            var e = ievent as FirstEnterGameEvent;
            if (FirstEnterGameObject != null && e != null)
            {
                FirstEnterGameObject.SetActive(e.Type);
            }
	    }
	
	    private void OnEvent_EquipSkillAnim(IEvent ievent)
	    {
	        var e = ievent as SkillEquipMainUiAnime;
	        var skillId = e.SkillId;
	        var index = e.Index;
	        if (index < 0 || index >= 4)
	        {
	            Logger.Error("OnSkillEquipMainUiAnime Error Index  = {0}", index);
	        }
	        learnSkillID = skillId;
	        var tbSkill = Table.GetSkill(skillId);
	        GameUtils.SetSpriteIcon(SkillMove, tbSkill.Icon);
	        SkillMove.gameObject.SetActive(true);
	        SkillName.text = tbSkill.Name;
	        var tween = SkillMove.GetComponent<TweenPosition>();
	        tween.to = SkillList[index].transform.localPosition;
	        tween.ResetToBeginning();
	        tween.PlayForward();
	        SkillNameTween.ResetToBeginning();
	        SkillNameTween.PlayForward();
	
	        if (GameSetting.Instance.EnableNewFunctionTip)
	        {
	            BlockMainScreen(true);
	        }
	
	        tween.onFinished.Clear();
	        tween.onFinished.Add(new EventDelegate(() =>
	        {
	            learnSkillID = -1;
	            PlayerDataManager.Instance.LearnSkill(skillId);
	            SkillMove.gameObject.SetActive(false);
	            BlockMainScreen(false);
	        }));
	
	        if (null != SkillEffectAni)
	        {
	            // 			var spr = SkillEffectAni.GetComponent<UISprite>();
	            // 			if(null!=spr)
	            // 			{
	            // 				spr.enabled = true;
	            // 			}
	            SkillEffectAni.Play();
	        }
	    }



        //灭世之战按钮
        void InitMonsterSiege()
        {
            Show_UI_Event eventMonster = new Show_UI_Event(UIConfig.MonsterSiegeUI);
            eventMonster.Args = new MonsterSiegeUIFrameArguments();
            eventMonster.Args.Tab = 5;
            EventDispatcher.Instance.DispatchEvent(eventMonster);
        }
        public void OnMonsterSiegeBtn()
        {
            //   EventDispatcher.Instance.DispatchEvent(new MieShiSetActivityId_Event(5));
            Show_UI_Event eventMonster = new Show_UI_Event(UIConfig.MonsterSiegeUI);            
            EventDispatcher.Instance.DispatchEvent(eventMonster);
        }




	
	    public void RewardMessageGoto()
	    {
	        EventDispatcher.Instance.DispatchEvent(new RewardMessageOpetionClick(1));
	    }
	
	    public void RewardMessageRecharge()
	    {
	        EventDispatcher.Instance.DispatchEvent(new RewardMessageOpetionClick(0));
	    }
	
	    private void ChangeModel(bool show)
	    {
	        if (show)
	        {
	            if (PlayerDataManager.Instance.IsInPvPScnen())
	            {
	                return;
	            }
	
	            //MissionRoot.SetActive(false);
	        }
	        else
	        {
	            //MissionRoot.SetActive(true);
	        }
	        PkBg.SetActive(show);
	    }

        private void ChangeState(bool state)
        { }
	
	    //-------------------------------------------------FightReady
	    private IEnumerator ShowCountDown()
	    {
	        var target = countdownTime;
	        var type = countdownType;
	        while (target >= Game.Instance.ServerTime)
	        {
	            yield return new WaitForSeconds(0.3f);
	            var dif = (int) ((target - Game.Instance.ServerTime).TotalSeconds);
	            if (dif == 1)
	            {
	                if (type == eCountdownType.BattleFight)
	                {
	                    // 开始战斗！
	                    FightReady.text = GameUtils.GetDictionaryText(270113);
	                }
	                else
	                {
	                    FightReady.gameObject.SetActive(false);
	                }
	            }
	            else if (dif == 0)
	            {
	                FightReady.gameObject.SetActive(false);
	            }
	            else
	            {
	                FightReady.text = (dif - 1).ToString();
	                if (FightReady.gameObject.activeSelf == false)
	                {
	                    FightReady.gameObject.SetActive(true);
	                }
	            }
	        }
	        FightReady.gameObject.SetActive(false);
	        FightReady.text = "";
	        displayCountdownTimer = false;
	        countdownCoroutine = null;
	    }
	
	    private void BlockMainScreen(bool flag)
	    {
	        if (null == BlockInputWidget)
	        {
	            return;
	        }
	
	        if (flag)
	        {
	            BlockInputWidget.gameObject.SetActive(true);
	        }
	        else
	        {
	            BlockInputWidget.gameObject.SetActive(false);
	        }
	        currentBlockTime = 0;
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MainUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel("Radar"));
	        Binding.SetBindDataSource(controllerBase.GetDataModel("MainUI"));
	        Binding.SetBindDataSource(controllerBase.GetDataModel("SelectTarget"));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.RewardNotice);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.SkillData);
	        var chatController = UIManager.Instance.GetController(UIConfig.ChatMainFrame);
	        Binding.SetBindDataSource(chatController.GetDataModel(""));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	        var missionController = UIManager.Instance.GetController(UIConfig.MissionTrackList);
	        Binding.SetBindDataSource(missionController.GetDataModel(""));
	        Binding.SetBindDataSource(controllerBase.GetDataModel("DeviceInfo"));
	        var teamController = UIManager.Instance.GetController(UIConfig.TeamFrame);
	        Binding.SetBindDataSource(teamController.GetDataModel(""));
	        var rechargeController = UIManager.Instance.GetController(UIConfig.RechargeFrame);
	        Binding.SetBindDataSource(rechargeController.GetDataModel("RechargeDataModel"));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.WeakNoticeData);
	        var firstChargeController = UIManager.Instance.GetController(UIConfig.FirstChargeFrame);
	        if (firstChargeController != null)
	        {
	            Binding.SetBindDataSource(firstChargeController.GetDataModel(""));
	        }

            var activityController = UIManager.Instance.GetController(UIConfig.ActivityUI);
            if (activityController != null)
            {
                Binding.SetBindDataSource(activityController.GetDataModel(""));
            }

            var wingChargeController = UIManager.Instance.GetController(UIConfig.WingChargeFrame);
            if (wingChargeController != null)
            {
                Binding.SetBindDataSource(wingChargeController.GetDataModel(""));
            }

            var rechargeActivityController = UIManager.Instance.GetController(UIConfig.RechargeActivityUI);
            if (rechargeActivityController != null)
            {
                Binding.SetBindDataSource(rechargeActivityController.GetDataModel(""));
            }

	        var settingController = UIManager.Instance.GetController(UIConfig.SettingUI);
	        if (null != settingController)
	        {
                Binding.SetBindDataSource(settingController.GetDataModel(""));
	        }

			var operationActivityController = UIManager.Instance.GetController(UIConfig.OperationActivityFrame);
			if (null != operationActivityController)
			{
				Binding.SetBindDataSource(operationActivityController.GetDataModel(""));
			}

	        EventDispatcher.Instance.AddEventListener(ShowCountdownEvent.EVENT_TYPE, OnEvent_ShowCountdown);
	        EventDispatcher.Instance.AddEventListener(SkillEquipMainUiAnime.EVENT_TYPE, OnEvent_EquipSkillAnim);
	        EventDispatcher.Instance.AddEventListener(UIEvent_BuffIncreaseAnimation.EVENT_TYPE, PlayBuffIncreaseAnim);
	        EventDispatcher.Instance.AddEventListener(UI_BlockMainUIInputEvent.EVENT_TYPE, OnEvent_BlockMainScreen);
	        EventDispatcher.Instance.AddEventListener(SceneTransition_Event.EVENT_TYPE, OnEvent_SceneChange);
			EventDispatcher.Instance.AddEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent_UpdateMission);
            EventDispatcher.Instance.AddEventListener(FirstEnterGameEvent.EVENT_TYPE, OnEvent_ShowFirstEnterGame);
            EventDispatcher.Instance.AddEventListener(HiedMieShiIcon_Event.EVENT_TYPE,HiedMieshiIcon);
            EventDispatcher.Instance.AddEventListener(ShowPopTalk_Event.EVENT_TYPE, ShowBossTalk);
			EventDispatcher.Instance.AddEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, OnNewFuncionGuideEvent);
	        var PkBtnCount0 = PkBtn.Count;
	        for (var i = 0; i < PkBtnCount0; i++)
	        {
	            var btn = PkBtn[i];
	            var j = i;
	            btn.onClick.Add(new EventDelegate(() => { ChangePkModel(j); }));
	        }
	
	        SetupBufferList();
	        //CreateOffineFrame();
	
	        if (null != BlockInputWidget)
	        {
	            if (BlockInputWidget.gameObject.activeSelf)
	            {
	                BlockInputWidget.gameObject.SetActive(false);
	            }
	        }
	
            countdownTime = Game.Instance.ServerTime;
            countdownType = eCountdownType.BattleFight;
	
            //撑大字的纹理
            if (!s_isPretreatString)
            {
                s_isPretreatString = true;
                var txt = ExpLable.GetComponent<UILabel>();
                if (null != txt && null != txt.font && null != txt.font.dynamicFont)
                {
                    txt.font.dynamicFont.RequestCharactersInTexture(GameSetting.Instance.PrepareString);
                }
            }
            IControllerBase monsterController = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI);
            if (monsterController != null)
            {
                MonsterSiegeUIFrameArguments ms = new MonsterSiegeUIFrameArguments();
                ms.Tab = 5;
                monsterController.RefreshData(ms);
                // return;
            }

            EventDispatcher.Instance.DispatchEvent(new MieShiGetInfo_Event());

			var scene = GameLogic.Instance.Scene;
		    if (null != scene)
		    {
			    if (null != scene.TableScene)
			    {
				    if (1==scene.TableScene.IsShowMainUI)
				    {
						ShowMainButton();
				    }
				    else
				    {
						HideMainButton();
				    }
			    }
		    }
#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
        }

        float HP;

        //判断血量显示警告
        public void ShowPlayerHpTransition()
        {

            if (PlayerHpTransition != null)
            {
                HP = (PlayerDataManager.Instance.PlayerDataModel.Attributes.HpPercent) * 100;
                if (HP < Playerhp && HP > 0)
                {
                    PlayerHpTransition.SetActive(true);
                }
                else
                {
                    PlayerHpTransition.SetActive(false);
                }
            }
           
        }
        public void ShowBossTalk(IEvent e)
        {
            var eventShow = e as ShowPopTalk_Event;
            if (BossTalk != null && BossTalk.gameObject != null)
            {
                BossTalk.gameObject.SetActive(true);
                BossTalk.text = eventShow.talk;
                BossTalk.transform.GetComponent<UILabel>().color = Color.red;
                // StartCoroutine(Show()); 
                mBossTalkTimer = Time.realtimeSinceStartup + 10.0f;
             }
         
        }

	    public void OnNewFuncionGuideEvent(IEvent e)
	    {
		    ShowMainButton();
	    }
        public IEnumerator Show()
        {
            yield return new WaitForSeconds(4);
            BossTalk.text = "";
            BossTalk.gameObject.SetActive(false);
        }
        public void HiedMieshiIcon(IEvent e)
        {
            MieshiBtn.SetActive(false);
        }
        private void StopTransition()
        {
            if (null != Transition)
            {
                var tween = Transition.GetComponentInChildren<TweenAlpha>();
                if (null != tween)
                {
                    tween.enabled = false;
                }
                Transition.SetActive(false);
            }
        }
	
        private void Update()
        {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
            if (null != BlockInputWidget)
            {
                if (BlockInputWidget.gameObject.active)
                {
                    currentBlockTime += Time.deltaTime;
                    if (currentBlockTime > BlockContinueTime)
                    {
                        BlockInputWidget.gameObject.SetActive(false);
                        currentBlockTime = 0;
                    }
                }
                else
                {
                    if (0 != currentBlockTime)
                    {
                        currentBlockTime = 0;
                    }
                }
            }

            ShowPlayerHpTransition();
            RenfreshTime();
            CheckBossTalk();           
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
        }
        void CheckBossTalk()
        {
            if (mBossTalkTimer < 0f)
                return;
            if (mBossTalkTimer < Time.realtimeSinceStartup)
            {
                BossTalk.gameObject.SetActive(false);
            }
        }
        void RenfreshTime()//刷新时间
        {

            MonsterDataModel DataModel = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI).GetDataModel(string.Empty) as MonsterDataModel;
            TimeSpan tsSpan = DataModel.ActivityTime - DateTime.Now;



            if(DataModel.CurActivityID <= 0)
            {
                return;
            }
            //报名时间
            DateTime tm = DataModel.ActivityTime.AddMinutes(-Table.GetMieShiPublic(1).CanApplyTime);
            //报名阶段的时候
            //if (DataModel.ActivityState==1)
            //{
            //    TimeLabel.text = string.Format("{0}", string.Format("{0:HH:MM}", tm));
            //}
            //活动开始的时候
                 if (DataModel.ActivityState == 2 || DataModel.ActivityState==3)
                {
                    TimeLabel.text = Table.GetDictionary(300000076).Desc[0];
                    TimeLabel.GetComponent<UILabel>().color = Color.red;
                }
                else if ((DataModel.ActivityState>4))
                {
                    TimeLabel.text = Table.GetDictionary(300000077).Desc[0];
                }
                else
                {
                    if (tsSpan.Milliseconds<=0)
                    {
                        this.TimeLabel.text = null;
                    }
                    else
                    {
                        this.TimeLabel.text =
                   tsSpan.Days.ToString().PadLeft(1, '0') + "天" + tsSpan.Hours.ToString().PadLeft(2, '0') + ":" + tsSpan.Minutes.ToString().PadLeft(2, '0') + ":" + tsSpan.Seconds.ToString().PadLeft(2, '0');
                    }
                    
                }
               
                if (tsSpan.Days<1)
                {
                    LiuGuang.SetActive(true);
                }
                else
                {
                    LiuGuang.SetActive(false);
                }
            



        }
	    public void UpdateTimer()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UpdateActivityTipTimerEvent());
	    }
	
	    public void Listen<T>(T message)
	    {
	        isEnable = true;
	    }

	    public void HideMainButton()
	    {
		    HideBtn.SetActive(false);
			ShowBtn.SetActive(true);
			MainBtnRoot.SetActive(false);
	    }
		public void ShowMainButton()
		{
			HideBtn.SetActive(true);
			ShowBtn.SetActive(false);
			MainBtnRoot.SetActive(true);
		}
	    #region mainui Dungeon
	
	    public void OnclickDungeonQueueShow()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ShowDungeonQueue(1));
	    }
	
	    public void OnclickDungeonQueueHide()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ShowDungeonQueue(0));
	    }
	
	    public void OnclickDungeonQueueCloseWins()
	    {
	        var e = new UIEvent_CloseDungeonQueue(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
    }
}