
﻿using System;
using System.Collections;
using System.Collections.Generic;
using ClientService;
using DataContract;
using ScorpionNetLib;
#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;


#endregion
namespace GameUI
{
    public class MonsterLogic : MonoBehaviour
    {
        public BindDataRoot Binding;

        public MonsterDataModel MonsterModel;

        public UIDragRotate DragRorate;
        public TimerLogic HomePageTimerLogic;
        private IControllerBase mController;
        public CreateFakeCharacter ModelRoot;
        private Vector3 ModelRootOriPos;
        private Transform ModelRootTransform;
        private bool mRemoveBind = true;


        public UIButton EnterGameButton;
        public UIButton NoOpenBtn;
        public UILabel EnterGameBtnLabel;
        public UILabel BossNameLabel;

        //六个炮台按钮
        public List<UIButton> PaoTaiBtnList = new List<UIButton>();
        //提升血量的道具按钮
        public List<UIButton> UpHpBtnList = new List<UIButton>();
        //提升技能的道具按钮
        public List<UIButton> UpLevelBtnList = new List<UIButton>();
        public GameObject HpInfoPanel, SkillInfoPanel;

        public GameObject[] MieshiActivityBtn;
        public GameObject[] HpSkillBtn;



        public UILabel ScoreDescLabel;
        public UILabel GXDescLabel;


        public UILabel PaoTaiNameLabel;
        public UISliderControl TowerUpTimes;


        public UISprite sprite;

        public UIButton PaotaiInfoBtn;

        public GameObject PanelTwo;

        public UIToggle TowerTogle;

        public UILabel LB;

        public UILabel BaoMingTime;


        public UIButton FightingResultBtn;


        public UILabel SkillTime;

        public UILabel UseDiamondCount, UsePropCount;

        public GameObject LiuGuang;

        public GameObject TishengLiuGuang;

        public UISprite Xueliang;
        private List<DateTime> MieShiActivityTimeList = new List<DateTime>();
        public TowerUpRewardLogic mReward;
        //存储炮台数据
        public List<UISprite> TowerStataIcons = new List<UISprite>();
        //推荐战力
        public GameObject Power;
        private void Awake()
        {
#if !UNITY_EDITOR
try
{
#endif
            if (ModelRoot != null)
            {
                ModelRootTransform = ModelRoot.transform;
            }
            
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }

        /**private void DisappearModelRoot(IEvent ievent)
        {
            if (ModelRoot != null)
            {
                var e = ievent as MieShiDisappearModelRoot_Event;
                bool bDisappear = e.bDisappear;
                ModelRoot.SetActiveFakeCharacter(bDisappear);
            }
        }**/

        private void RefreshTowers(IEvent ievent)
        {
            RefreshTwoerIconStata();
        }


        private void UIToggleClick(IEvent ievent)
        {
            var e = ievent as MieShiUiToggle_Event;
            if (e != null)
            {
                int idxUIToggle = e.idxUIToggle;
                switch (idxUIToggle)
                {
                    case 0:
                        OnMissionBtnOne();
                        break;
                    case 1:
                        OnMissionBtnTwo();
                        break;
                    case 2:
                        OnMissionBtnThree();
                        break;
                    default:
                        break;
                }
            }
            
        }

        private void UpdateMieshiActivtyTimeList(IEvent ievent)
        {
            var eventUpdate = ievent as MieShiAddActvityTime_Event;
            int iType = eventUpdate.iType;
            if (iType == 0)
            {
                MieShiActivityTimeList.Clear();
            }
            else
            {
                MieShiActivityTimeList.Add(eventUpdate.activityTime);
            }
        }

        private int BossID;

        private void CreateFakeObj(IEvent ievent)
        {
            int dataId = (ievent as MieShiRefreshBoss_Event).idBosd;
            if (ModelRoot != null)
            {
                ModelRoot.DestroyFakeCharacter();
            }
            if (-1 == dataId)
            {
                return;
            }

            var tableNpc = Table.GetCharacterBase(dataId);
            if (null == tableNpc)
            {
                return;
            }
            if (ModelRoot != null)
            {
                ModelRoot.Create(dataId, null, character =>
                {
                    character.SetScale(tableNpc.CameraMult / 10000f);
                    character.ObjTransform.localRotation = Quaternion.identity;
                    ModelRootTransform.localPosition = ModelRootOriPos + new Vector3(0, tableNpc.CameraHeight / 10000.0f, 0);
                    character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                    DragRorate.Target = character.transform;
                });
            }
            

            BossID = dataId;
            if (BossNameLabel != null)
            {
                BossNameLabel.text = tableNpc.Name;
            }
        }

        public void OnBtnDynamicActivityQueueClicked()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.DynamicActivity_Queue));
        }

        public void OnBtnGotoBoss()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_GotoMonster));
        }

        public void OnBtnQueueClicked()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_Queue));
        }

        public void OnButtonClose()
        {
        }

        public void OnClossBtn()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MonsterSiegeUI));
        }

        public void OnClosePage()
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiCLosePage_Event());
        }

        public void OnClossBtn1()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.BattryLevelUpUI));
        }



        public void OnButtonEnterActivity()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_Enter));
        }

        public void OnButtonEnterDynamicActivity()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.DynamicActivity_Enter));
        }

        //三种活动的按钮
        public void OnMissionBtnOne()
        {
            SetActivityID(1);
            RefreshBtnState();
            CanOnClick();
            int FubenId = MonsterModel.MonsterFubens[0].Fuben.FubenId;
            //MonsterModel.MonsterFubens[0].NeedLevel = Table.GetScene(FubenId).LevelLimit;
        }
        public bool CanOnClick()
        {
            if (MonsterModel.ActivityId != MonsterModel.CurActivityID)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(280000));
                if (TowerTogle != null)
                {
                    TowerTogle.enabled = false;
                }
                return false;
            }
            else
            {
                if (TowerTogle != null)
                {
                    RefreshBtnState();
                    EnterGameButton.enabled = true;
                    TowerTogle.enabled = true;
                }
                return true;
            }
        }
        public void OnMissionBtnTwo()
        {
            SetActivityID(2);
            CanOnClick();
            RefreshBtnState();

            int FubenId = MonsterModel.MonsterFubens[1].Fuben.FubenId;
            //MonsterModel.MonsterFubens[1].activity.NeedLevel = Table.GetScene(FubenId).LevelLimit;
        }
        public void OnMissionBtnThree()
        {
            SetActivityID(3);
            CanOnClick();
            RefreshBtnState();
            int FubenId = MonsterModel.MonsterFubens[2].Fuben.FubenId;
            //MonsterModel.MonsterFubens[2].activity.NeedLevel = Table.GetScene(FubenId).LevelLimit;
        }
        private void OnCloseUiBindRemove(IEvent ievent)
        {
            var e = ievent as CloseUiBindRemove;
            if (e.Config != UIConfig.ActivityUI)
            {
                return;
            }
            if (e.NeedRemove == 0)
            {
                mRemoveBind = false;
            }
            else
            {
                if (mRemoveBind == false)
                {
                    RemoveBindEvent();
                }
                mRemoveBind = true;
            }
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
try
{
#endif
            if (ModelRoot != null)
            {
                ModelRoot.DestroyFakeCharacter();
            }
            if (mRemoveBind == false)
            {
                RemoveBindEvent();
            }
            mRemoveBind = true;
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
            if (ModelRoot != null)
            {
                ModelRoot.DestroyFakeCharacter();
            }
            EventDispatcher.Instance.DispatchEvent(new MieShiRefreshBoss_Event(-1));
            if (mRemoveBind)
            {
                RemoveBindEvent();
            }

            //EventDispatcher.Instance.RemoveEventListener(MieShiDisappearModelRoot_Event.EVENT_TYPE, DisappearModelRoot);
            EventDispatcher.Instance.RemoveEventListener(MieShiRefreshTowers_Event.EVENT_TYPE, RefreshTowers);
            EventDispatcher.Instance.RemoveEventListener(MieShiAddActvityTime_Event.EVENT_TYPE, UpdateMieshiActivtyTimeList);
            EventDispatcher.Instance.RemoveEventListener(MieShiUiToggle_Event.EVENT_TYPE, UIToggleClick);
            EventDispatcher.Instance.RemoveEventListener(MieShiRefreshBoss_Event.EVENT_TYPE, CreateFakeObj);
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
            //EventDispatcher.Instance.AddEventListener(MieShiDisappearModelRoot_Event.EVENT_TYPE, DisappearModelRoot);
            EventDispatcher.Instance.AddEventListener(MieShiRefreshTowers_Event.EVENT_TYPE, RefreshTowers);
            EventDispatcher.Instance.AddEventListener(MieShiAddActvityTime_Event.EVENT_TYPE, UpdateMieshiActivtyTimeList);
            EventDispatcher.Instance.AddEventListener(MieShiUiToggle_Event.EVENT_TYPE, UIToggleClick);
            EventDispatcher.Instance.AddEventListener(MieShiRefreshBoss_Event.EVENT_TYPE, CreateFakeObj);
            if (mRemoveBind)
            {
                EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);

                mController = UIManager.Instance.GetController(UIConfig.MonsterSiegeUI);


                if (mController == null)
                {
                    return;
                }

                MonsterModel = mController.GetDataModel("") as MonsterDataModel;
                MonsterModel.PropertyChanged += OnMonsterPropertyChangedEvent;

                Binding.SetBindDataSource(MonsterModel);
				Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
            }
            mRemoveBind = true;
            var main = UIManager.Instance.MainUIFrame.GetComponent<MainScreenFrame>();
            RefreshBtnState();

            //报名时间
            //  MonsterDataModel ad = DataModel.CurMonsterFuben.activity;
            if (MieShiActivityTimeList.Count > 0)
            {
                DateTime time = MieShiActivityTimeList[0];
                if (time != null)
                {
                    DateTime tm = time.AddMinutes(-(double)(Table.GetMieShiPublic(1).CanApplyTime));
                    DateTime tm2 = time;
                    BaoMingTime.text = string.Format("{0}--{1}", string.Format("{0:yyyy/MM/dd HH:mm}", tm), string.Format("{0:yyyy/MM/dd HH:mm}", tm2));// tm + "--" + tm2;
                }
            }

           
            int FubenId = MonsterModel.MonsterFubens[0].Fuben.FubenId;
            //MonsterModel.MonsterFubens[0].activity.NeedLevel = Table.GetScene(FubenId).LevelLimit;
         //   MonsterModel.UseDiamond = Table.GetMieShiPublic(1).CostNum;
         //   MonsterModel.UseProp = Table.GetMieShiPublic(1).ItemNum;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }


        private void OnMonsterPropertyChangedEvent(object o, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "BossDataId")
            {
                //CreateFakeObj(DataModel.BossDataId);
            }
            else if (args.PropertyName == "ActivityState" || args.PropertyName == "BaoMingState")
            {
                RefreshBtnState();
            }
            else if (args.PropertyName == "CurMonsterTowers" || args.PropertyName == "UseDiamond")
            {
                RefreshTwoerIconStata();
               
            }
            Debug.Log(args.PropertyName+"............................");
        }
        void RefreshTwoerIconStata()
        {
            for (int i = 0; i < TowerStataIcons.Count; i++)
            {
                if (MonsterModel != null && MonsterModel.MonsterTowers != null)
                {
                    MonsterTowerDataModel mtdm = MonsterModel.MonsterTowers[i];
                    if (TowerStataIcons != null && TowerStataIcons[i] != null)
                    {
                        if (mtdm.BloodPer <= 150)
                        {
                            TowerStataIcons[i].spriteName = Table.GetIcon(2310000).Sprite;
                            TowerStataIcons[i].transform.GetComponent<UIButton>().normalSprite = TowerStataIcons[i].spriteName;

                        }
                        else if (mtdm.BloodPer > 150 && mtdm.BloodPer <= 300)
                        {
                            TowerStataIcons[i].spriteName = Table.GetIcon(2310001).Sprite;
                            TowerStataIcons[i].transform.GetComponent<UIButton>().normalSprite = TowerStataIcons[i].spriteName;
                        }
                        else if (mtdm.BloodPer > 300 && mtdm.BloodPer <= 500)
                        {
                            TowerStataIcons[i].spriteName = Table.GetIcon(2310002).Sprite;
                            TowerStataIcons[i].transform.GetComponent<UIButton>().normalSprite = TowerStataIcons[i].spriteName;
                        }
                    }
                }
               
            }

        }
        private void SetActivityID(int id)
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiSetActivityId_Event(id));

            int idx = id - 1;
            if (MieShiActivityTimeList.Count> 0&& idx >= 0&& idx < MieShiActivityTimeList.Count)
            {
                DateTime time = MieShiActivityTimeList[idx];
                if (time != null)
                {
                    DateTime tm = time.AddMinutes(-(double)(Table.GetMieShiPublic(1).CanApplyTime));
                    DateTime tm2 = time;
                    BaoMingTime.text = string.Format("{0}--{1}", string.Format("{0:yyyy/MM/dd HH:mm}", tm), string.Format("{0:yyyy/MM/dd HH:mm}", tm2));// tm + "--" + tm2;
                }
            }
        }
        public void OnTabBloodCastle()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(1));
        }

        public void OnTabDevilSquare()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(0));
        }

        public void OnTabGoldArmy()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(4));
        }

        public void OnTabMapCommander()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(3));
        }

        public void OnTabWorldBoss()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(2));
        }

        public void OnUpdateTowerHpClick()
        {//实际使用的是这个,技能提升也集成到这个协议里
            EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateSkillAndHpEvent(1));
        }

        public void OnUpdateTowerSkillClick()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateSkillAndHpEvent(2));
        }

        public void CloseUpdateTowerMessageBox()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateSkillAndHpEvent(0));
        }

        private void RemoveBindEvent()
        {
            Binding.RemoveBinding();
            if (MonsterModel != null)
            {
                MonsterModel.PropertyChanged -= OnMonsterPropertyChangedEvent;
            }

            EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
        }

        private void Start()
        {
#if !UNITY_EDITOR
try
{
#endif

            for (int i = 0; i < PaoTaiBtnList.Count; i++)
            {

                int PaotaiIndexd = i;
                PaoTaiBtnList[i].onClick.Add(new EventDelegate(() => ShowPaotaiInfo(PaotaiIndexd)));
            }

            for (int i = 0; i < UpHpBtnList.Count; i++)
            {
                if (UpHpBtnList[i] != null)
                {
                    int Index = i;
                    UpHpBtnList[i].onClick.Add(new EventDelegate(() => UpHpBtn(ID, Index)));
                }

            }

            /**for (int i = 0; i < UpLevelBtnList.Count; i++)
            {
                int idx = i;
                UpLevelBtnList[i].onClick.Add(new EventDelegate(() => UpLevelBtn(ID, idx)));
            }**/

            if (ModelRootTransform != null)
            {
                ModelRootOriPos = ModelRootTransform.localPosition;
            }

            if (HomePageTimerLogic != null)
                HomePageTimerLogic.TargetTime = Game.Instance.ServerTime.AddYears(10);

         

           
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
        }
       
        private void ShowPaotaiInfo(int PaotaiIndexd)
        {
            if (PaotaiIndexd < 0 || PaotaiIndexd >= PaoTaiBtnList.Count)
            {
                return;
            }
            MonsterModel.MonsterTowerIdx = PaotaiIndexd;
            ID = MonsterModel.CurMonsterTowers.TowerId;
            PaoTaiNameLabel.text = Table.GetNpcBase(226000 + PaotaiIndexd).Name.ToString();
            EventDispatcher.Instance.DispatchEvent(new MieShiOnPaotaiBtn_Event());
            for (int i = 0; i < PaoTaiBtnList.Count; i++)
            {
                
                if (i==PaotaiIndexd)
                {
                    PaoTaiBtnList[PaotaiIndexd].transform.FindChild("guang").gameObject.SetActive(true);
                }
                else
                {
                    PaoTaiBtnList[i].transform.FindChild("guang").gameObject.SetActive(false);
                }
            }
           
        }

        private int ID { get; set; }




        //提升炮台
        public void OnTowerUp()
        {
           //当前炮台  MonsterModel.MonsterTowerIdx;
            EventDispatcher.Instance.DispatchEvent(new MieShiUpHpBtn_Event());
        }
        //
        /// <summary>
        /// 提升炮台血量
        /// </summary>
        /// <param name="id">炮台ID</param>
        /// <param name="index">提升类型ID</param>
        /// 
        /// 
        public void UpHpBtn(int id, int index)
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiUpHpBtn_Event());
            ShowPaotaiInfo(id - 1);
            ShowLiuGuang();
          
        }

        public void UpdateUseItem()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateUseItemEvent());
        }

        public void OnUpdateHpClick()
        {
            int id = MonsterModel.CurMonsterTowers.TowerId;
            EventDispatcher.Instance.DispatchEvent(new MieShiUpHpBtn_Event());
            ShowPaotaiInfo(id - 1);
            ShowLiuGuang();
        }

        public void ShowLiuGuang()
        {
          
        }
        public void RefreshBtnState()
        {
            var main = UIManager.Instance.MainUIFrame.GetComponent<MainScreenFrame>();
            if (EnterGameButton == null)
            {
                return;
            }
            LB.gameObject.SetActive(false);
            FightingResultBtn.gameObject.SetActive(false);
          
            if (MonsterModel.ActivityId == MonsterModel.CurActivityID)
            {
                Power.SetActive(true);
                if (MonsterModel.ActivityState == 0)
                {
                    // EnterGameButton.enabled = false;
                    NoOpenBtn.gameObject.SetActive(true);
                    EnterGameButton.gameObject.SetActive(false);
                    //EnterGameBtnLabel.text = Table.GetDictionary(300000020).Desc[0];
                }
                else if (MonsterModel.ActivityState == 1)
                {
                    SetBaoMingState();
                    LiuGuang.SetActive(true);
                }
                else if (MonsterModel.ActivityState == 2)
                {
                    SetEnterState();
                    LiuGuang.SetActive(true);
                }

                else if (MonsterModel.ActivityState == 3)
                {
                    SetEnterState();
                    LiuGuang.SetActive(true);
                }
                else if (MonsterModel.ActivityState == 4)
                {
                    SetWillEndState();
                    LiuGuang.SetActive(false);
                }
                //else if (DataModel.ActivityState == 5)
                //{
                //    SetFightingResState();
                //}
                Debug.Log(MonsterModel.ActivityState + "活动状态==================");

            }

            else
            {
                LiuGuang.SetActive(false);
                NoOpenBtn.gameObject.SetActive(true);
                EnterGameButton.gameObject.SetActive(false);
                Power.SetActive(false);
            }

        }
        bool isTrue = false;

        public void EnterGame()
        {
            var main = UIManager.Instance.MainUIFrame.GetComponent<MainScreenFrame>();


            if (MonsterModel.ActivityId != MonsterModel.CurActivityID)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270229));

            }
            else
            {
                if (MonsterModel.ActivityState == 0)
                {
                    EnterGameBtnLabel.text = Table.GetDictionary(300000021).Desc[0];
                }
                else if (MonsterModel.ActivityState == 1)
                {
                   // EnterGameBtnLabel.text = Table.GetDictionary(300000024).Desc[0];
                    EventDispatcher.Instance.DispatchEvent(new MieShiOnYibaomingBtn_Event());
                  
                }
                else if (MonsterModel.ActivityState == 2)
                {
                    if (MonsterModel.BaoMingState == true)
                    {
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.MieShiActivity_Queue));
                    }
                    else
                    {
                        EventDispatcher.Instance.DispatchEvent(new MieShiOnYibaomingBtn_Event());
                       
                    }
                }
                else if (MonsterModel.ActivityState == 3)
                {
                    if (MonsterModel.BaoMingState == true)
                    {
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.MieShiActivity_Queue));
                    }
                    else
                    {
                        EventDispatcher.Instance.DispatchEvent(new MieShiOnYibaomingBtn_Event());
                    }
                }
                else if (MonsterModel.ActivityState == 4)
                {
                    SetWillEndState();
                    LiuGuang.SetActive(false);
                }
            }
            #region MyRegion
            #endregion

        }
        //报名按钮设置

       
        public void ShowSkillTip()
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiShowSkillTip_Event());
        }
        public void SetBaoMingState()
        {
            if (MonsterModel.BaoMingState == false)
            {
                NoOpenBtn.gameObject.SetActive(false);
                EnterGameButton.gameObject.SetActive(true);
                EnterGameBtnLabel.text = Table.GetDictionary(300000021).Desc[0];
            }
            else
            {
                NoOpenBtn.gameObject.SetActive(false);
                EnterGameButton.gameObject.SetActive(true);
                EnterGameBtnLabel.DictionaryId = 300000024;//.text = Table.GetDictionary(300000024).Desc[0];
            }
        }
        //进入活动按钮设置
        public void SetEnterState()
        {
            if (MonsterModel.BaoMingState == true)
            {
                // EnterGameButton.enabled = true;
                NoOpenBtn.gameObject.SetActive(false);
                EnterGameButton.gameObject.SetActive(true);
                EnterGameBtnLabel.text = Table.GetDictionary(300000023).Desc[0];
            }
            else
            {
                NoOpenBtn.gameObject.SetActive(false);
                EnterGameButton.gameObject.SetActive(true);
                EnterGameBtnLabel.text = Table.GetDictionary(300000021).Desc[0];
            }
        }
        //即将结束按钮设置
        public void SetWillEndState()
        {
            NoOpenBtn.gameObject.SetActive(false);
            EnterGameButton.gameObject.SetActive(false);
            LB.gameObject.SetActive(true);
        }
        //战斗结束按钮设置
        public void SetFightingResState()
        {
            NoOpenBtn.gameObject.SetActive(false);
            EnterGameButton.gameObject.SetActive(false);
            LB.gameObject.SetActive(false);
            FightingResultBtn.gameObject.SetActive(true);
        }

        //请求炮台数据
        public void OnPaotaiBtn()
        {
            if (CanOnClick())
            {
                EventDispatcher.Instance.DispatchEvent(new MieShiOnPaotaiBtn_Event());
                ShowPaotaiInfo(0);
                EventDispatcher.Instance.DispatchEvent(new MieShiOnGXRankingBtn_Event());
            }
        }

        public void ShowRankingReward()
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiShowRankingReward_Event());
        }

        public void ShowMieshiRule()
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiShowRules_Event());
            
        }

        /// <summary>
        /// 贡献排行榜
        /// </summary>
        public void OnGXRankingBtn()
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiOnGXRankingBtn_Event());
        }
        public void UpdateMainPageTimer()
        {
            EventDispatcher.Instance.DispatchEvent(new UpdateActivityTimerEvent(UpdateActivityTimerType.MainPage));
        }

        public void UpdateTimer()
        {
            EventDispatcher.Instance.DispatchEvent(new UpdateActivityTimerEvent(UpdateActivityTimerType.Single));
        }
        private void UpLevelBtn(int id, int idx)
        {
            EventDispatcher.Instance.DispatchEvent(new MonsterSiegeUpLevelBtn_Event(id, idx));
            ShowLiuGuang();
        }
        //战斗结果按钮
        public void OnFightingResultBtn()
        {
            EventDispatcher.Instance.DispatchEvent(new MieShiShowFightingResult_Event());
        }

        public void Update()
        {
          
         
        }

        public void OnClickShop()
        {
            var arg = new StoreArguments { Tab = 106 };
            var e = new Show_UI_Event(UIConfig.StoreEquip, arg);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }
   
}
