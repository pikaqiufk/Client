#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ObjCommand;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class MainUIController : IControllerBase
{
    public MainUIController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(RefresSceneMap.EVENT_TYPE, OnRefresSceneMap);
        EventDispatcher.Instance.AddEventListener(MainUiOperateEvent.EVENT_TYPE, OnMainUiOperate);
        EventDispatcher.Instance.AddEventListener(Postion_Change_Event.EVENT_TYPE, OnPostionChange);
        //EventDispatcher.Instance.AddEventListener(Character_Create_Event.EVENT_TYPE, OnCreateCharacter);
        EventDispatcher.Instance.AddEventListener(Character_Remove_Event.EVENT_TYPE, OnRemoveCharacter);
        EventDispatcher.Instance.AddEventListener(ShowCharacterInMinimap.EVENT_TYPE, UpdateMinimapCharacter);

        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
        EventDispatcher.Instance.AddEventListener(ShowMissionProgressEvent.EVENT_TYPE, OnShowMissionProgress);
        EventDispatcher.Instance.AddEventListener(UpdateMissionProgressEvent.EVENT_TYPE, OnUpdateMissionProgress);
        EventDispatcher.Instance.AddEventListener(HideMissionProgressEvent.EVENT_TYPE, OnHideMissionProgress);
        EventDispatcher.Instance.AddEventListener(MainUI_OnClickSwitch.EVENT_TYPE, OnShowSkill);
        EventDispatcher.Instance.AddEventListener(EquipDurableChange.EVENT_TYPE, OnEquipDurableChange);
        EventDispatcher.Instance.AddEventListener(BagDataInitEvent.EVENT_TYPE, OnBagDataInit);
        EventDispatcher.Instance.AddEventListener(NotifyDungeonTime.EVENT_TYPE, OnNotifyDungeonTime);
        EventDispatcher.Instance.AddEventListener(UIEvent_ShowDungeonQueue.EVENT_TYPE, OnShowDungeonQueue);
        EventDispatcher.Instance.AddEventListener(UIEvent_WindowShowDungeonQueue.EVENT_TYPE, OnShowWinsDungeonQueue);
        EventDispatcher.Instance.AddEventListener(UIEvent_CloseDungeonQueue.EVENT_TYPE, CloseDungeonQueue);
        EventDispatcher.Instance.AddEventListener(UIEvent_SelectServer.EVENT_TYPE, OnSelectServer);
        EventDispatcher.Instance.AddEventListener(DungeonCompleteEvent.EVENT_TYPE, OnDungeonCompleteEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_MainUIButtonShowEvent.EVENT_TYPE, MainUIButtonShowEvent);
        EventDispatcher.Instance.AddEventListener(RefreshDamageListEvent.EVENT_TYPE, RefreshDamageList);
        EventDispatcher.Instance.AddEventListener(RefreshMieshiDamageListEvent.EVENT_TYPE, RefreshMieshiDamageList);
        EventDispatcher.Instance.AddEventListener(ShowActivityTipEvent.EVENT_TYPE, ShowActivityTip);
        EventDispatcher.Instance.AddEventListener(ActivityTipClickedEvent.EVENT_TYPE, OnActivityTipClicked);
        EventDispatcher.Instance.AddEventListener(UIEvent_BuffListBtn.EVENT_TYPE, OnBuffListButtonClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_SyncBuffCell.EVENT_TYPE, OnSyncBuffCell);
        EventDispatcher.Instance.AddEventListener(UIEvent_ClearBuffList.EVENT_TYPE, ClearBuffList);
        EventDispatcher.Instance.AddEventListener(UIEvent_DeviceInfo_NetWorkStateChange.EVENT_TYPE, OnNetWorkStateChanged);
        EventDispatcher.Instance.AddEventListener(UIEvent_RemoveBuffsOnDead.EVENT_TYPE, RemoveBuffsOnDead);
        EventDispatcher.Instance.AddEventListener(UpdateActivityTipTimerEvent.EVENT_TYPE, UpdateTimer);
        EventDispatcher.Instance.AddEventListener(Enter_Scene_Event.EVENT_TYPE, OnShowSenceUIEvent);
        EventDispatcher.Instance.AddEventListener(ActivityStateChangedEvent.EVENT_TYPE, OnActivityStateChanged);
        EventDispatcher.Instance.AddEventListener(Event_LevelChange.EVENT_TYPE, OnLevelChange);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(RewardMessageOpetionClick.EVENT_TYPE, RewardMessageOpetion);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnPlayerLevelUpGrade);
        EventDispatcher.Instance.AddEventListener(Event_ShowMieshiFubenInfo.EVENT_TYPE, ShowMieshiFubenInfo);
        EventDispatcher.Instance.AddEventListener(Event_ShowMieshiRankingInfo.EVENT_TYPE, ShowMieshiRankingInfo);
        EventDispatcher.Instance.AddEventListener(Event_MieShiStartCountDownData.EVENT_TYPE, OnEvent_UpdateMieShiStartCountDowm);
        EventDispatcher.Instance.AddEventListener(Event_RefreshFuctionOnState.EVENT_TYPE, OnEvent_RefreshFuctionOnState);
        EventDispatcher.Instance.AddEventListener(ShowSceneMapEvent.EVENT_TYPE, OnEvent_ShowSceneMap);
        EventDispatcher.Instance.AddEventListener(ShowFastReachEvent.EVENT_TYPE, OnShowFastRecach);
        EventDispatcher.Instance.AddEventListener(ClickReachBtnEvent.EVENT_TYPE, OnClickFastRecach);

        EventDispatcher.Instance.AddEventListener(OnClickFastReachMessageBoxOKEvent.EVENT_TYPE, OnClickFastRecachMsgOK);
        EventDispatcher.Instance.AddEventListener(OnClickFastReachMessageBoxCancleEvent.EVENT_TYPE, OnClickFastRecachMsgCancle);

        EventDispatcher.Instance.AddEventListener(RefreshDungeonInfo_Event.EVENT_TYPE, OnRefreshDungeonInfo);
        
    }

    private static float fastReachTimeMax = 1.0f;
    private float fastReachTime = -1.0f;
    private int _bossMaxHp;
    public float batteryLevelDuration;
    public List<int> BlacksceneNpcIds = new List<int> {8, 16, 17, 33};
    public float buffRefreshDuration;
    public string DungeonTimeFormater = "{0:D2}:{1:D2}";
    //public string MieshiTimeFormater = "{0:D2}:{1:D2}";
    public float duration;
    public bool isMvoe;
    public int IsShowButton;
    public Dictionary<uint, BuffCell> mBuffCellDictionary = new Dictionary<uint, BuffCell>();
    public float mConvertRate = 5.0f;
    public int mLastShowBuffInfoIndex = -1;
    public FrameState mState;
    public bool NoticeMoving = false;
    public DateTime ShowTime = DateTime.MaxValue;
    public DateTime Speed = DateTime.MaxValue;
    public Coroutine TeamTimteRefresh;
    public float timeDuration;
    public int TipDicId;
    //---------------------------------------------------------------------Buff
    public int TipFubenId;
    public DateTime TipStartTime = DateTime.Now;
    public DateTime TipTargetTime = DateTime.Now;
    public BuffListDataModel BuffListDataModel { get; set; }
    //手机状态信息
    public DeviceInfoDataModel DeviceInfoData { get; set; }
    public Coroutine DungeonCoroutine { get; set; }
    public Coroutine MieshiStartCoroutine { get; set; }

    public DateTime DungeonTime
    {
        get { return MainDataModel.DungeonTime; }
        set
        {
            if (MainDataModel.DungeonTime == value)
            {
                return;
            }
            MainDataModel.DungeonTime = value;
            StartDungeonCountdown();
        }
    }

    public MainUIDataModel MainDataModel { get; set; }
    public RadarMapDataModel RadarMapData { get; set; }
    private Dictionary<ulong, RararCharDataModel> radarDataDict = new Dictionary<ulong, RararCharDataModel>();

    public void AddBuff(BuffResult buff)
    {
        BuffCell buffCell;
        if (!mBuffCellDictionary.TryGetValue(buff.BuffId, out buffCell))
        {
            buffCell = new BuffCell();
            mBuffCellDictionary.Add(buff.BuffId, buffCell);
            BuffListDataModel.ShowBuffs.Add(buffCell);
            EventDispatcher.Instance.DispatchEvent(new UIEvent_BuffIncreaseAnimation());

            //是否鼓舞  第一次加 把特效搞消失
            if (CheckGuWu(buff.BuffTypeId))
            {
                MainDataModel.IsShowGuwuPoint = false;
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(535));
            }
        }
        else
        {
            //是否鼓舞 叠加 弹一句提示
            if (CheckGuWu(buff.BuffTypeId))
            {
                MainDataModel.IsShowGuwuPoint = false;
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100002111));
            }
        }

        buffCell.BuffId = buff.BuffTypeId;
        if (buff.Param.Count >= 3)
        {
            buffCell.BuffLastTime = buff.Param[0];
            buffCell.BuffLayer = buff.Param[1];
            buffCell.BuffLevel = buff.Param[2];
        }

        BuffListDataModel.BuffCount = BuffListDataModel.ShowBuffs.Count;
    }

    // 0 == 不是鼓舞buff   1 == 是鼓舞
    private bool CheckGuWu(int buffId)
    {
        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return false;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return false;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return false;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return false;
        }

        var tbInspire = Table.GetBangBuff(tbFuben.CanInspire);
        if (tbInspire == null)
        {
            return false;
        }

        if (tbInspire.BuffDiamodId.Any(diamod => diamod == buffId))
        {
            return true;
        }

        if (tbInspire.BuffGoldId.Any(glod => glod == buffId))
        {
            return true;
        }

        return false;
    }

    private bool CheckGuWuPoint(int buffId, bool withOutBuff = false)
    {
        if (withOutBuff == false)
        {
            if (!CheckGuWu(buffId)) return false;
        }

        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return false;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return false;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return false;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return false;
        }

        var tbInspire = Table.GetBangBuff(tbFuben.CanInspire);
        if (tbInspire == null)
        {
            return false;
        }

        if (tbInspire.ShowPointer == 0)
        {
            return false;
        }
        else if (tbInspire.ShowPointer == 1)
        {
            return true;
        }

        return false;
    }


    private void OnEvent_UpdateMieShiStartCountDowm(IEvent ievent)
    {
        var e = ievent as Event_MieShiStartCountDownData;
        if (e != null)
        {
            int sec = e.Second;
            MainDataModel.MieshiStartCounTime = Game.Instance.ServerTime.AddSeconds(sec);
            StartMieshiStartCountdown();
        }
    }

    private void OnEvent_RefreshFuctionOnState(IEvent ievent)
    {
        MainDataModel.IsShowFuctionOn = false;

        Table.ForeachFunctionOn((record) =>
        {
            if (record == null)
            {
                return false;
            }
            var condition = GameUtils.CheckFuctionOnCondition(record.OpenLevel, record.TaskID, record.State);
            if (condition != 0)
            {
                MainDataModel.IsShowFuctionOn = true;
                MainDataModel.FuctionOn.IconId = record.IconId;
                MainDataModel.FuctionOn.FuctionCondition = record.FrameDesc;
                MainDataModel.FuctionOn.FuctionName = record.Name;
                MainDataModel.FuctionOn.FuctionDes = record.IconDesc;

                if (GameUtils.CheckFuctionOnConditionByLevel(record.OpenLevel) != 0)
                {
                    MainDataModel.FuctionOn.ProgressType = 2;
                    MainDataModel.FuctionOn.ProgressValue = PlayerDataManager.Instance.GetLevel();
                    if (MainDataModel.FuctionOn.ProgressValue < 0)
                    {
                        MainDataModel.FuctionOn.ProgressValue = 0;
                    }
                    MainDataModel.FuctionOn.ProgressMaxVale = record.OpenLevel;
                }
                if (GameUtils.CheckFuctionOnConditionByMission(record.TaskID) != 0)
                {
                    MainDataModel.FuctionOn.ProgressType = 1;
                    int lastMissionOrder = GameUtils.GetMainMissionOrderByFunctionId(record.Id - 1);
                    MainDataModel.FuctionOn.ProgressValue = Table.GetMissionBase(GameUtils.GetCurMainMissionId()).MissionBianHao - lastMissionOrder;
                    if (MainDataModel.FuctionOn.ProgressValue < 0)
                    {
                        MainDataModel.FuctionOn.ProgressValue = 0;
                    }
                    MainDataModel.FuctionOn.ProgressMaxVale = Table.GetMissionBase(record.TaskID).MissionBianHao - lastMissionOrder;
                }
                
                return false;
            }
            return true;
        });
    }

    public void OnEvent_ShowSceneMap(IEvent ievent)
    {
        var e = ievent as ShowSceneMapEvent;
        if (e != null)
        {
            if (SceneManager.Instance.isInMieshiFuben())
            {
                var ev = new Show_UI_Event(UIConfig.MieShiSceneMapUI);
                EventDispatcher.Instance.DispatchEvent(ev);                
            }
            else
            {
                var ev = new Show_UI_Event(UIConfig.SceneMapUI);
                EventDispatcher.Instance.DispatchEvent(ev);                
            }
        }       
    }

    public void OnShowFastRecach(IEvent ievent)
    {
        var e = ievent as ShowFastReachEvent;
        if (e != null)
        {
            if (e.IsShow)
            {
                fastReachTime = fastReachTimeMax;
            }
            else
            {
                fastReachTime = -1.0f;
            }

            if (MainDataModel.IsShowFastReach != false)
            {
                MainDataModel.IsShowFastReach = false;
            }
        }
    }

    public void OnClickFastRecachMsgOK(IEvent ievent)
    {
        var myPlayer = ObjManager.Instance.MyPlayer;
        if (myPlayer == null)
        {
            return;
        }

        var freeTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e700);
        //var doneTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e701);
        var vipDoneTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e702);
        var vipAdd = Table.GetVIP(PlayerDataManager.Instance.GetRes((int)eResourcesType.VipLevel)).SentTimes;
        var leftTimes = freeTimes + vipAdd - vipDoneTimes;
        if (leftTimes <= 5 && leftTimes > 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200009001));
        }
        else if (leftTimes <= 0)
        {
            var needDiamond = Table.GetClientConfig(1207).ToInt();
            if (PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes) < needDiamond)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));

                var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                EventDispatcher.Instance.DispatchEvent(e);

                return;
            }
        }

        
        myPlayer.LeaveAutoCombat();
        GameUtils.FastReach(myPlayer.fastReachSceneID, myPlayer.fastReachPos.x, myPlayer.fastReachPos.z);
        MainDataModel.IsShowFastReachMessageBox = false;
    }

    public void OnClickFastRecachMsgCancle(IEvent ievent)
    {
        MainDataModel.IsShowFastReachMessageBox = false;
    }

    public void OnClickFastRecach(IEvent ievent)
    {
        var myPlayer = ObjManager.Instance.MyPlayer;
        if (myPlayer == null)
        {
            return;
        }

        var freeTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e700);
        //var doneTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e701);
        var vipDoneTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e702);
        var vipAdd = Table.GetVIP(PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel)).SentTimes;
        var leftTimes = freeTimes + vipAdd - vipDoneTimes;
        if (leftTimes <= 5 && leftTimes > 0)
        {
            MainDataModel.FastReachMsgDesc = string.Format(GameUtils.GetDictionaryText(100002116),leftTimes);
            MainDataModel.IsShowFastReachMessageBox = true;
            MainDataModel.IsShowFastReachCheckBox = false;
        }
        else if (leftTimes <= 0)
        {
            var needDiamond =  Table.GetClientConfig(1207).ToInt();
            MainDataModel.FastReachMsgDesc = string.Format(GameUtils.GetDictionaryText(100002117), needDiamond);
            MainDataModel.IsShowFastReachMessageBox = true;
            MainDataModel.IsShowFastReachCheckBox = true;
        }
        else if (leftTimes > 5)
        {
            myPlayer.LeaveAutoCombat();
            GameUtils.FastReach(myPlayer.fastReachSceneID, myPlayer.fastReachPos.x, myPlayer.fastReachPos.z);
        }
    }

    public void OnRefreshDungeonInfo(IEvent ievent)
    {
        var e = ievent as RefreshDungeonInfo_Event;
        if (e != null && e.Info != null)
        {
            for (int i = 0; i < MainDataModel.TdungeonInfo.Int32OneList.Count; i++)
            {
                MainDataModel.TdungeonInfo.Int32OneList[i] = 0;
            }

            for (int i = 0; i < MainDataModel.TdungeonInfo.Int32TwoList.Count; i++)
            {
                MainDataModel.TdungeonInfo.Int32TwoList[i] = -1;
            }

            for (int i = 0; i < MainDataModel.TdungeonInfo.Int64List.Count; i++)
            {
                MainDataModel.TdungeonInfo.Int64List[i] = 0;
            }

            for (int i = 0; i < MainDataModel.TdungeonInfo.FloatList.Count; i++)
            {
                MainDataModel.TdungeonInfo.FloatList[i] = 0;
            }

            for (int i = 0; i < MainDataModel.TdungeonInfo.StringList.Count; i++)
            {
                MainDataModel.TdungeonInfo.StringList[i] = "";
            }
     
            MainDataModel.TdungeonInfo.MyName = PlayerDataManager.Instance.GetName();
            MainDataModel.TdungeonInfo.MyRank = 99;


            MainDataModel.TdungeonInfo.Type = e.Info.Type;
            for (int i = 0; i < e.Info.Int32OneList.Count; i++)
            {
                if (MainDataModel.TdungeonInfo.Int32OneList.Count > i)
                {
                    MainDataModel.TdungeonInfo.Int32OneList[i] = e.Info.Int32OneList[i];
                }
            }

            for (int i = 0; i < e.Info.Int64List.Count; i++)
            {
                if (MainDataModel.TdungeonInfo.Int64List.Count > i)
                {
                    MainDataModel.TdungeonInfo.Int64List[i] = e.Info.Int64List[i];
                }
                if (e.Info.Int64List[i] == PlayerDataManager.Instance.GetGuid())
                {
                    MainDataModel.TdungeonInfo.MyName = PlayerDataManager.Instance.GetName();
                    MainDataModel.TdungeonInfo.MyRank = i + 1;
                }
            }

            for (int i = 0; i < e.Info.Int32TwoList.Count; i++)
            {
                if (MainDataModel.TdungeonInfo.Int32TwoList.Count > i)
                {
                    MainDataModel.TdungeonInfo.Int32TwoList[i] = e.Info.Int32TwoList[i];
                }
            }

            for (int i = 0; i < e.Info.FloatList.Count; i++)
            {
                if (MainDataModel.TdungeonInfo.FloatList.Count > i)
                {
                    MainDataModel.TdungeonInfo.FloatList[i] = e.Info.FloatList[i];
                }
            }

            for (int i = 0; i < e.Info.StringList.Count; i++)
            {
                if (MainDataModel.TdungeonInfo.StringList.Count > i)
                {
                    MainDataModel.TdungeonInfo.StringList[i] = e.Info.StringList[i];
                }
            }
        }
    }


    public void OnRemoveCharacter(IEvent ievent)
    {
        var e = ievent as Character_Remove_Event;
        if (e != null)
        {
            var charId = e.CharacterId;
            RemoveMinimapCharacter(charId);
        }
    }

    public void UpdateMinimapCharacter(IEvent ievent)
    {
        var e = ievent as ShowCharacterInMinimap;
        if (e != null)
        {
            if (e.Show)
            {
                CreateMinimapCharacter(e.CharId);
            }
            else
            {
                RemoveMinimapCharacter(e.CharId);
            }
        }
    }

    private void RemoveMinimapCharacter(ulong charId)
    {
        RararCharDataModel radarData;
        if (radarDataDict.TryGetValue(charId, out radarData))
        {
            var e1 = new MainUiCharRadar(radarData, 0);
            EventDispatcher.Instance.DispatchEvent(e1);
            RadarMapData.CharaModels.Remove(radarData);
            radarDataDict.Remove(charId);
        }
    }

    private void CreateMinimapCharacter(ulong charId)
    {
        var dataMode = new RararCharDataModel();
        var obj = ObjManager.Instance.FindCharacterById(charId);
        if (obj == null)
        {
            return;
        }
        dataMode.CharacterId = charId;
        var pos = RadarConvertLoction(obj.Position);
        dataMode.Loction = pos;
        switch (obj.GetObjType())
        {
            case OBJ.TYPE.INVALID:
                break;
            case OBJ.TYPE.OTHERPLAYER:
                break;
            case OBJ.TYPE.NPC:
                {
                    var npc = obj as ObjNPC;
                    bool interactive = false;
                    if (null != npc && null != npc.TableNPC)
                    {
                        interactive = npc.TableNPC.Interactive != 0;
                    }
                    if (!interactive)
                    {
                        return;
                    }
                    if (ObjManager.Instance.MyPlayer != null)
                    {
                        dataMode.Width = 10;
                        dataMode.Height = 10;
                        var isEnemy = ObjManager.Instance.MyPlayer.IsMyEnemy(npc);
                        if (isEnemy)
                        {
                            dataMode.SpriteName = "map_icon_monster";
                            dataMode.CharType = 2;
                        }
                        else
                        {
                            dataMode.SpriteName = "map_icon_npc";
                            dataMode.CharType = 1;
                        }
                        RadarMapData.CharaModels.Add(dataMode);
                        radarDataDict[dataMode.CharacterId] = dataMode;
                        var e1 = new MainUiCharRadar(dataMode, 1);
                        EventDispatcher.Instance.DispatchEvent(e1);
                    }
                }
                break;
            case OBJ.TYPE.MYPLAYER:
                break;
            case OBJ.TYPE.FAKE_CHARACTER:
                break;
        }
    }

    public void ChangeSelfPostion(Vector3 objLoction)
    {
        if (ObjManager.Instance.MyPlayer == null || ObjManager.Instance.MyPlayer.ObjTransform == null)
        {
            return;
        }
        RadarMapData.PalyerLocX = (int) objLoction.x;
        RadarMapData.PalyerLocY = (int) objLoction.z;
        var arrow = RadarConvertLoction(objLoction);
        var arrowX = arrow.x;
        var arrowY = arrow.y;

        var heightOffset = 170.0f/2;
        var wigthOffset = 170.0f/2;

        var mapX = -(arrowX - wigthOffset);
        var mapY = -(arrowY - heightOffset);

        if (mapX > 0.0f)
        {
            mapX = 0.0f;
        }
        else if (mapX < -(RadarMapData.MapWidth - wigthOffset))
        {
            mapX = -(RadarMapData.MapWidth - wigthOffset);
        }

        if (mapY > 0.0f)
        {
            mapY = 0.0f;
        }
        else if (mapY < -(RadarMapData.MapHeight - heightOffset))
        {
            mapY = -(RadarMapData.MapHeight - heightOffset);
        }

        RadarMapData.ArrowLoc = new Vector3(arrowX, arrowY);
        RadarMapData.MapLoc = new Vector3(mapX, mapY);

        RadarMapData.MapLoc = RotaVector3(RadarMapData.MapLoc, new Vector3(86f, 84f), -45);

        var v = ObjManager.Instance.MyPlayer.ObjTransform.eulerAngles;
        RadarMapData.ArrowRotate = new Vector3(0, 0, -v.y - 45f);
    }

    public void ClearBuffList(IEvent ievent)
    {
        mBuffCellDictionary.Clear();
        BuffListDataModel.BuffInfoShow = false;
        BuffListDataModel.ShowBuffs.Clear();
        BuffListDataModel.BuffCount = BuffListDataModel.ShowBuffs.Count;
    }

    public void CloseDungeonQueue(IEvent ievnet)
    {
        var ee = ievnet as UIEvent_CloseDungeonQueue;
        switch (ee.ShowMessageType)
        {
            case 0:
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(220499), "",
                    () => { NetManager.Instance.StartCoroutine(TeamCancelCoroutine()); });
            }
                break;
            case 1:
            {
                NetManager.Instance.StartCoroutine(TeamCancelCoroutine());
            }
                break;
        }
    }

    public IEnumerator DungeonCountdownCoroutine()
    {
        bool isPlayedAnita = false;
        MainDataModel.Color = MColor.white;
        while (true)
        {
            if (DungeonTime < Game.Instance.ServerTime)
            {
                MainDataModel.DungeonTimeStr = "00:00";
                DungeonCoroutine = null;
                MainDataModel.IsPlayFuBenCollDownAni = false; 
                yield break;
            }
            var dif = DungeonTime - Game.Instance.ServerTime;
            MainDataModel.DungeonTimeStr = string.Format(DungeonTimeFormater, dif.Minutes, dif.Seconds);

            // 少于60秒开始闪烁
            if (dif.TotalSeconds < 60 && isPlayedAnita == false)
            {
                if (isPlayedAnita == false)
                {
                    MainDataModel.Color = MColor.red;
                    MainDataModel.IsPlayFuBenCollDownAni = true;
                    isPlayedAnita = true;
                }
            }
            else if (dif.TotalSeconds > 60)
            {
                isPlayedAnita = false;
                if (MainDataModel.Color != MColor.white)
                {
                    MainDataModel.Color = MColor.white;
                }
                
                if (MainDataModel.IsPlayFuBenCollDownAni == true)
                {
                    MainDataModel.IsPlayFuBenCollDownAni = false;
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    public IEnumerator MieShiStartCountdownCoroutine()
    {
        while (true)
        {
            if (MainDataModel.MieshiStartCounTime < Game.Instance.ServerTime)
            {
                MainDataModel.MieshiShutCountdown = "00:00";
                MieshiStartCoroutine = null;
                MainDataModel.IsShowMishiStart = false;
                yield break;
            }
            var dif = MainDataModel.MieshiStartCounTime - Game.Instance.ServerTime;
            MainDataModel.MieshiShutCountdown = string.Format(DungeonTimeFormater, dif.Minutes, dif.Seconds);
            MainDataModel.IsShowMishiStart = true;
            yield return new WaitForSeconds(1.0f);
        }
    }

    public string GetBuffTimeString(int seconds)
    {
        string ret;
        if (seconds > 3600)
        {
            ret = string.Format("{0}h{1}m", seconds/3600, (seconds/60)%60);
        }
        else if (seconds > 60)
        {
            ret = string.Format("{0}m{1}s", seconds/60, seconds%60);
        }
        else
        {
            ret = string.Format("{0}s", seconds);
        }

        return ret;
    }

    private void Inspire()
    {
        if (ObjManager.Instance.MyPlayer.Dead)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000104));
            return;
        }
        NetManager.Instance.StartCoroutine(InspireCoroutine());
    }

    private IEnumerator InspireCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var type = MainDataModel.InspireGold ? 0 : 1;
            var msg = NetManager.Instance.Inspire(type);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State != MessageState.Reply)
            {
                yield break;
            }
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                MainDataModel.ShowInspireMessage = false;
                //EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(535));
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
            }
        }
    }

    public void MainUIButtonShowEvent(IEvent ievent)
    {
        var e = ievent as UIEvent_MainUIButtonShowEvent;
        IsShowButton = e.Param;
        PlayerDataManager.Instance.SetCanShowTarget(IsShowButton == 0);
    }

    public void OnActivityStateChanged(IEvent ievent)
    {
        if (!MainDataModel.ShowActivityTip)
        {
            return;
        }

        var activityState = PlayerDataManager.Instance.ActivityState;
        var key = (int) eActivity.WorldBoss;
        if (activityState.ContainsKey(key))
        {
            var state = activityState[key];
            if (state >= (int) eActivityState.WillEnd)
            {
                MainDataModel.ShowActivityTip = false;
            }
        }
    }

    public void OnActivityTipClicked(IEvent ievent)
    {
        EventDispatcher.Instance.DispatchEvent(new DungeonTipClickedEvent(TipFubenId));
    }

    public void OnBagDataInit(IEvent ievent)
    {
        PlayerDataManager.Instance.ForeachEquip(item =>
        {
            if (item.ItemId != -1 && item.Exdata != null && item.Exdata.Count > 22)
            {
                var value = item.Exdata.Durability;
                var tbEquip = Table.GetEquipBase(item.ItemId);
                if (value <= 0)
                {
                    MainDataModel.Equipdurable = 2;
                }
                else if (tbEquip.Durability >= value*10)
                {
                    if (MainDataModel.Equipdurable == 0)
                    {
                        MainDataModel.Equipdurable = 1;
                    }
                }
            }
        });
    }

    public void OnBuffListButtonClick(IEvent ievent)
    {
        var e = ievent as UIEvent_BuffListBtn;

        if (e.ButtonIndex == 0)
        {
            BuffListDataModel.BuffListShow = !BuffListDataModel.BuffListShow;
        }
        else if (e.ButtonIndex == 1)
        {
            OnShowBuffInfo(true, e.Data);
        }
        else if (e.ButtonIndex == 2)
        {
            OnShowBuffInfo(false, e.Data);
        }
    }

    public void OnClickDungeonAutoFight()
    {
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null)
        {
            return;
        }
        if (tbScene.Type != 2)
        {
            return;
        }
        var logciId = Scene.LogicId;
        var tbDungeonLogic = Table.GetFubenLogic(logciId);
        if (tbDungeonLogic == null)
        {
            return;
        }
        //var loc = new Vector3(tbDungeonLogic.Hang1PosX, 0, tbDungeonLogic.Hang1PosZ);

        GameControl.Executer.Stop();
        if (tbDungeonLogic.Hang1PosX != -1 && tbDungeonLogic.Hang1PosZ != -1)
        {
            var command = GameControl.GoToCommand(sceneId, tbDungeonLogic.Hang1PosX, tbDungeonLogic.Hang1PosZ);
            GameControl.Executer.ExeCommand(command);
        }
        var command1 = new FuncCommand(() =>
        {
            GameControl.Instance.TargetObj = null;
            ObjManager.Instance.MyPlayer.EnterAutoCombat();
        });
        GameControl.Executer.PushCommand(command1);
    }

    public void OnClickInspire()
    {
        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return;
        }

        var tbInspire = Table.GetBangBuff(tbFuben.CanInspire);
        if (tbInspire == null)
        {
            return;
        }

        var curDangci = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e666);
        if (curDangci < 0)
        {
            return;
        }

        if (MainDataModel.InspireGold)
        {
            var needGold = 0; //Table.GetClientConfig(381).ToInt();
            var needDiamond = 0;
            var buffId = 0;

            if (curDangci < tbInspire.BuffGoldId.Length)
            {
                buffId = tbInspire.BuffGoldId[curDangci];
            }
            else
            {
                buffId = tbInspire.BuffGoldId[tbInspire.BuffGoldId.Length - 1];
            }

            if (curDangci < tbInspire.BuffGoldPrice.Length)
            {
                needGold = tbInspire.BuffGoldPrice[curDangci];
            }
            else
            {
                needGold = tbInspire.BuffGoldPrice[tbInspire.BuffGoldPrice.Length - 1];
            }

            if (curDangci < tbInspire.BuffDiamodPrice.Length)
            {
                needDiamond = tbInspire.BuffDiamodPrice[curDangci];
            }
            else
            {
                needDiamond = tbInspire.BuffDiamodPrice[tbInspire.BuffDiamodPrice.Length - 1];
            }

            var tbBuff = Table.GetBuff(buffId);
            if (tbBuff == null)
            {
                UIManager.Instance.ShowNetError((int)ErrorCodes.Error_CanNot_Inspire);
                return;
            }
            MainDataModel.InspireString = string.Format(GameUtils.GetDictionaryText(100002110), tbBuff.Desc);
            MainDataModel.NeedGold = needGold;
            MainDataModel.NeedDiamond = needDiamond;
        }
        else
        {
            //var needGold = Table.GetClientConfig(386).ToInt();
            var needGold = 0;
            var needDiamond = 0; //Table.GetClientConfig(381).ToInt();
            var buffId = 0;

            if (curDangci < tbInspire.BuffDiamodId.Length)
            {
                buffId = tbInspire.BuffDiamodId[curDangci];
            }
            else
            {
                buffId = tbInspire.BuffDiamodId[tbInspire.BuffDiamodId.Length - 1];
            }

            if (curDangci < tbInspire.BuffGoldPrice.Length)
            {
                needGold = tbInspire.BuffGoldPrice[curDangci];
            }
            else
            {
                needGold = tbInspire.BuffGoldPrice[tbInspire.BuffGoldPrice.Length - 1];
            }

            if (curDangci < tbInspire.BuffDiamodPrice.Length)
            {
                needDiamond = tbInspire.BuffDiamodPrice[curDangci];
            }
            else
            {
                needDiamond = tbInspire.BuffDiamodPrice[tbInspire.BuffDiamodPrice.Length - 1];
            }

            var tbBuff = Table.GetBuff(buffId);
            if (tbBuff == null)
            {
                UIManager.Instance.ShowNetError((int)ErrorCodes.Error_CanNot_Inspire);
                return;
            }

            MainDataModel.InspireString = string.Format(GameUtils.GetDictionaryText(100002110), tbBuff.Desc);
            MainDataModel.NeedGold = needGold;
            MainDataModel.NeedDiamond = needDiamond;
        }
        MainDataModel.ShowInspireMessage = true;
    }

    public void OnClickUseMedicine()
    {
        if (PlayerDataManager.Instance.PlayerDataModel.Attributes.HpPercent > 0.999f)
        {
            var e = new ShowUIHintBoard(210201);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        var bag = PlayerDataManager.Instance.GetBag((int) eBagType.BaseItem);
        if (bag == null)
        {
            return;
        }
        //  List<BagItemDataModel> canUseList = new List<BagItemDataModel>();
        BagItemDataModel canUse = null;
        for (var i = 0; i < bag.Size; i++)
        {
            var item = bag.Items[i];
            if (item.ItemId == -1)
            {
                continue;
            }
            var tbItem = Table.GetItemBase(item.ItemId);
            if (tbItem == null)
            {
                continue;
            }
            if (tbItem.Type != 24000)
            {
                continue;
            }
            if (tbItem.UseLevel > PlayerDataManager.Instance.GetLevel())
            {
                continue;
            }
            if (tbItem.Exdata[2] != 0)
            {
                continue;
            }
            // canUseList.Add(item);
            if (canUse == null)
            {
                canUse = item;
            }
            else
            {
                if (canUse.ItemId < item.ItemId)
                {
                    canUse = item;
                }
            }                      
        }
        if (canUse != null)
        {
            GameUtils.UseItem(canUse);
        }
        return;
    }

    public void OnDungeonCompleteEvent(IEvent ievent)
    {
    }

    public void OnEquipDurableChange(IEvent ievent)
    {
        var e = ievent as EquipDurableChange;
        var state = e.State;
        if (MainDataModel.Equipdurable == 0 || MainDataModel.Equipdurable == 1)
        {
            MainDataModel.Equipdurable = state;
        }
        else if (MainDataModel.Equipdurable == 2)
        {
            if (state == 0)
            {
                MainDataModel.Equipdurable = state;
            }
        }
    }

    private void OnExDataInit(IEvent ievent)
    {
        var loginCount = PlayerDataManager.Instance.GetExData(eExdataDefine.e94);
        if (loginCount < 8)
        {
            MainDataModel.IsShowSevenBtn = true;
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SevenRewardInit());
        }
        else
        {
            MainDataModel.IsShowSevenBtn = false;
        }
        
        
        var Tili = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e630);
        Tili = Math.Max(Tili, 0);
        MainDataModel.TdungeonInfo.Int32OneList[0] = Tili;
    }

    private void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        if (e.Key == (int)eExdataDefine.e630)
        {
            var Tili = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e630);
            Tili = Math.Max(Tili, 0);
            MainDataModel.TdungeonInfo.Int32OneList[0] = Tili;
        }
    }

    public void OnHideMissionProgress(IEvent ievent)
    {
        MainDataModel.MissionProgress.Show = false;
        MainDataModel.MissionProgress.CurrentValue = 1.0f;
    }

    public void OnInspireCancel()
    {
        MainDataModel.ShowInspireMessage = false;
    }

    private void OnInspireGold(int type)
    {
        MainDataModel.InspireGold = type == 0;
        OnClickInspire();
    }

    public void OnInspireOk()
    {
        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return;
        }

        var tbInspire = Table.GetBangBuff(tbFuben.CanInspire);
        if (tbInspire == null)
        {
            return;
        }

        var curDangci = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e666);
        if (curDangci < 0)
        {
            return;
        }

        var needGold = 0;
        var needDiamond = 0;

        if (curDangci < tbInspire.BuffGoldPrice.Length)
        {
            needGold = tbInspire.BuffGoldPrice[curDangci];
        }
        else
        {
            needGold = tbInspire.BuffGoldPrice[tbInspire.BuffGoldPrice.Length - 1];
        }

        if (curDangci < tbInspire.BuffDiamodPrice.Length)
        {
            needDiamond = tbInspire.BuffDiamodPrice[curDangci];
        }
        else
        {
            needDiamond = tbInspire.BuffDiamodPrice[tbInspire.BuffDiamodPrice.Length - 1];
        }

        if (MainDataModel.InspireGold)
        {
            var goldCount = PlayerDataManager.Instance.GetItemCount((int) eResourcesType.GoldRes);
            if (goldCount < needGold)
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));

                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210100));
                return;
            }
        }
        else
        {
            var goldCount = PlayerDataManager.Instance.GetItemCount((int) eResourcesType.DiamondRes);
            if (goldCount < needDiamond)
            {
                var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                EventDispatcher.Instance.DispatchEvent(e);

                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                return;
            }
        }
        Inspire();
    }

    private void OnLevelChange(IEvent ievent)
    {
        if (!MainDataModel.ShowDailyActivityBtn)
        {
            var showLevel = Table.GetClientConfig(108).ToInt();
            var myLevel = PlayerDataManager.Instance.GetLevel();
            MainDataModel.ShowDailyActivityBtn = myLevel >= showLevel;
        }
        OnEvent_RefreshFuctionOnState(null);
    }

    public void OnMainUiOperate(IEvent ievent)
    {
        var e = ievent as MainUiOperateEvent;
        switch (e.Type)
        {
            case 0:
            {
                SetAutoCombat();
            }
                break;
            case 1:
            {
                OnShowTargetMenu();
            }
                break;
            case 2:
            {
                OnTransmitDurable();
            }
                break;
            case 3:
            {
                OnClickDungeonAutoFight();
            }
                break;
            case 4:
            {
                OnClickUseMedicine();
            }
                break;
            case 5:
            {
                OnClickInspire();
            }
                break;
            case 6:
            {
                OnInspireOk();
            }
                break;
            case 7:
            {
                OnInspireCancel();
            }
                break;
            case 8:
            {
                OnInspireGold(0);
            }
                break;
            case 9:
            {
                OnInspireGold(1);
            }
                break;
            case 10:
            {
                var conllor = UIManager.GetInstance().GetController(UIConfig.MissionTrackList);
                if (conllor != null)
                {
                    var data = conllor.GetDataModel("") as MissionTrackListDataModel;
                    if (data != null)
                    {
                        var missionId = data.List[0].MissionId;
                        MissionManager.Instance.GoToMissionPlace(missionId);
                    }
                }

                EventDispatcher.Instance.DispatchEvent(new FirstEnterGameEvent(false));                
            }
                break;
        }
    }

    public void OnNetWorkStateChanged(IEvent ievent)
    {
        if (null != DeviceInfoData)
        {
            DeviceInfoData.DeviceNetWorkStatus = (NetworkStatus) PlatformHelper.GetNetworkState();
        }
    }

    public void OnNotifyDungeonTime(IEvent ievent)
    {
        var e = ievent as NotifyDungeonTime;
        DungeonTime = DateTime.FromBinary(e.CloseTime);
        SetDungeonTimerString();
    }

    public void OnPostionChange(IEvent ievent)
    {
        if (RadarMapData.SceneId == -1)
        {
            return;
        }
        var e = ievent as Postion_Change_Event;
        ChangeSelfPostion(e.Loction);
    }

    public void OnRefresSceneMap(IEvent ievent)
    {
        var e = ievent as RefresSceneMap;
        OnRefresSceneMap(e.SceneId);
    }

    public void OnRefresSceneMap(int scendId)
    {
        RadarMapData.SceneId = scendId;
        var tbScene = Table.GetScene(RadarMapData.SceneId);
        if (tbScene == null)
        {
            return;
        }

        RadarMapData.MapWidth = tbScene.TerrainHeightMapWidth*mConvertRate;
        RadarMapData.MapHeight = tbScene.TerrainHeightMapLength*mConvertRate;
        var obj = ObjManager.Instance.MyPlayer;
        if (obj)
        {
            ChangeSelfPostion(obj.Position);
        }

        var enumerator1 = (RadarMapData.CharaModels).GetEnumerator();
        while (enumerator1.MoveNext())
        {
            var radarData = enumerator1.Current;
            {
                var e1 = new MainUiCharRadar(radarData, 0);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
        }

        RadarMapData.CharaModels.Clear();
        radarDataDict.Clear();
    }

    public void OnResourceChange(IEvent ievent)
    {
        var e = ievent as Resource_Change_Event;

        if (e.Type == eResourcesType.LevelRes)
        {
            if (e.OldValue == -1)
            {
                return;
            }

            MainDataModel.Level = e.NewValue;

            MainDataModel.MaxValues.Clear();
            MainDataModel.MaxValues = new List<int>();
            if (e.OldValue != 0)
            {
                var eNewValue0 = e.NewValue;
                for (var i = e.OldValue; i <= eNewValue0; i++)
                {
                    var tbLevel = Table.GetLevelData(i);
                    MainDataModel.MaxValues.Add(tbLevel.NeedExp);
                }
            }
            else
            {
                var tbLevel = Table.GetLevelData(e.NewValue);
                MainDataModel.MaxValues.Add(tbLevel.NeedExp);
            }
        }
        else if (e.Type == eResourcesType.ExpRes)
        {
            var lv = MainDataModel.Level;
            var tbLevel = Table.GetLevelData(lv);
            MainDataModel.ExpRate = lv + (float) e.NewValue/tbLevel.NeedExp;
        }
    }

    public void OnSelectServer(IEvent ievent)
    {
        var e = ievent as UIEvent_SelectServer;
        if (e == null)
        {
            return;
        }
        RadarMapData.ServerId = e.ServerId + 1;
    }

    public void OnShowBuffInfo(bool bShow, int index)
    {
        BuffListDataModel.BuffInfoShow = bShow;
        if (bShow)
        {
            mLastShowBuffInfoIndex = index;
            var buff = BuffListDataModel.ShowBuffs[index];
            var buffTable = Table.GetBuff(buff.BuffId);
            BuffListDataModel.BuffInfoName = buffTable.Name;
            BuffListDataModel.BuffLevel = buff.BuffLevel;
            BuffListDataModel.BuffInfoDesc = buffTable.Desc.Replace("\\n", "\n");
        }
    }

    public void OnShowDungeonQueue(IEvent ievent)
    {
        var e = ievent as UIEvent_ShowDungeonQueue;
        MainDataModel.Dungeon.IsShow = e.IsShow;
    }

    public void OnShowMissionProgress(IEvent ievent)
    {
        var e = ievent as ShowMissionProgressEvent;

        MainDataModel.MissionProgress.ProgressName = e.ProgressName;
        MainDataModel.MissionProgress.Show = true;
        //MainDataModel.MissionProgress.Speed = 1.0f / tbMission.FinishParam[0];
        MainDataModel.MissionProgress.CurrentValue = 0.0f;
    }

    public void OnShowSenceUIEvent(IEvent ievent)
    {
        var e = ievent as Enter_Scene_Event;
        MainDataModel.SceneId = e.SceneId;
        ShowTime = DateTime.Now.AddSeconds(5);
        MainDataModel.ShowMapName = true;
    }

    public void OnShowSkill(IEvent ievent)
    {
    }

    public void OnShowTargetMenu()
    {
        var characterId = PlayerDataManager.Instance.SelectTargetData.CharacterBase.CharacterId;
        if (characterId == 0)
        {
            return;
        }

        var obj = ObjManager.Instance.FindCharacterById(characterId);

        if (obj == null)
        {
            return;
        }

        if (obj.GetObjType() != OBJ.TYPE.OTHERPLAYER)
        {
            return;
        }
        PlayerDataManager.Instance.ShowCharacterPopMenu(characterId, obj.Name, 1, obj.GetLevel(), obj.Reborn, obj.RoleId);
    }

    public void OnShowWinsDungeonQueue(IEvent ievent)
    {
        var e = ievent as UIEvent_WindowShowDungeonQueue;
        if (e.QueueID == -1)
        {
            MainDataModel.Dungeon.IsWindowShow = false;
            if (TeamTimteRefresh != null)
            {
                NetManager.Instance.StopCoroutine(TeamTimteRefresh);
                TeamTimteRefresh = null;
            }
            return;
        }
        MainDataModel.Dungeon.IsWindowShow = true;
        MainDataModel.Dungeon.QueueID = e.QueueID;
        MainDataModel.Dungeon.StartTime = e.QueueDateTime;
        MainDataModel.Dungeon.FubenName = Table.GetFuben(Table.GetQueue(e.QueueID).Param).Name;
        var t = Game.Instance.ServerTime - MainDataModel.Dungeon.StartTime;
        if (TeamTimteRefresh != null)
        {
            NetManager.Instance.StopCoroutine(TeamTimteRefresh);
        }
        TeamTimteRefresh = NetManager.Instance.StartCoroutine(RefreshTeamTime((int) t.TotalSeconds));
    }

    public void OnSyncBuffCell(IEvent ievent)
    {
        var e = ievent as UIEvent_SyncBuffCell;
        var buffResult = e.Data;
        if (buffResult == null)
        {
            return;
        }

        if (null == ObjManager.Instance.MyPlayer || buffResult.TargetObjId != ObjManager.Instance.MyPlayer.GetObjId())
        {
            return;
        }

        if (Table.GetBuff(buffResult.BuffTypeId) == null)
        {
            return;
        }

        if (Table.GetBuff(buffResult.BuffTypeId).IsView != 1)
        {
            return;
        }

        if (buffResult.Type == BuffType.HT_ADDBUFF || buffResult.Type == BuffType.HT_EFFECT ||
            buffResult.Type == BuffType.HT_CHANGE_SCENE)
        {
            AddBuff(buffResult);
        }
        else if (buffResult.Type == BuffType.HT_DELBUFF)
        {
            RemoveBuff(buffResult);
        }
    }

    public void OnTransmitDurable()
    {
        var vipLevel = PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel);
        var table = Table.GetVIP(vipLevel);

        if (table.Repair == 1)
        {
            GameUtils.OnQuickRepair();
        }
        else
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 223052, "", OnTransmitDurableConfirm);
        }
    }

    public void OnTransmitDurableConfirm()
    {
        var scendId = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(scendId);
        var sceneType = tbScene.Type;
        if (sceneType != 0 && sceneType != 1)
        {
            //当前场景不能直接传送
            var e = new ShowUIHintBoard(270112);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        var tbSceneNpc = Table.GetMapTransfer(1);
        GameUtils.FlyTo(tbSceneNpc.SceneID, tbSceneNpc.PosX, tbSceneNpc.PosZ);
    }

    public void OnUpdateMissionProgress(IEvent ievent)
    {
        var e = ievent as UpdateMissionProgressEvent;
        MainDataModel.MissionProgress.CurrentValue = e.Percent;
    }

    public Vector3 RadarConvertLoction(Vector3 loc)
    {
        var x = loc.x*mConvertRate;
        var y = loc.z*mConvertRate;
        return new Vector3(x, y, 0);
    }

    public void RefreshDamageList(IEvent ievent)
    {
        var e = ievent as RefreshDamageListEvent;
        var damageList = e.DamageList;
        var damageUnits = damageList.Data;
        var topPlayers = damageList.TopPlayers;
        _bossMaxHp = damageList.NpcMaxHp;
        var entrys = MainDataModel.BossRank.Entrys;
        for (int i = 0, imax = topPlayers.Count; i < imax; ++i)
        {
            SetupEntry(topPlayers[i], entrys[i]);

        }
        var myUnit = damageUnits[0];
        SetupEntry(myUnit, entrys[5]);
    }


    private void ShowMieshiFubenInfo(IEvent ievent)
    {
        MainDataModel.IsShowMishiInfo = true;
        MainDataModel.IsShowMishiRanking = false;
    }


    private void ShowMieshiRankingInfo(IEvent ievent)
    {
        MainDataModel.IsShowMishiInfo = false;
        MainDataModel.IsShowMishiRanking = true;
    }

    public void RefreshMieshiDamageList(IEvent ievent)
    {
        var e = ievent as RefreshMieshiDamageListEvent;
        var damageList = e.DamageList;
        var topPlayers = damageList.TopPlayers;
        MainDataModel.MieshiRank.Entrys.Clear();
        var entrys = MainDataModel.MieshiRank.Entrys;
        for (int i = 0, imax = topPlayers.Count; i < imax; ++i)
        {
            //SetupEntry(topPlayers[i], entrys[i]);
            MishiRankEntry t = new MishiRankEntry();
            var damageUnit = topPlayers[i];
            var rank = damageUnit.Rank;
            var damageStr = GameUtils.GetBigValueStr(damageUnit.Damage);
            if (_bossMaxHp > 0)
            {
                var percent = 100f * damageUnit.Damage / _bossMaxHp;
                if (percent > 0f)
                {
                    percent = Math.Max(percent, 0.1f);
                }
                damageStr += "(" + percent.ToString("f1") + "%)";
            }
            t.Name = damageUnit.Name;
            t.Damage = damageStr;
            t.Rank = rank > 99 ? "99+" : rank.ToString();
            t.Show = true;
            if (i < 3)
            {
                t.IconId = 2310053 + i;
                t.ShowIcon = true;
                t.Rank = "";
            }
            MainDataModel.MieshiRank.Entrys.Add(t);
            
        }
        //var myUnit = e.DamageList.myRank;


        if (ObjManager.Instance.MyPlayer != null)
        {
            var myRanking = MainDataModel.myMieshiRanking;
            myRanking.Name = ObjManager.Instance.MyPlayer.Name;
            myRanking.Damage = GameUtils.GetBigValueStr(damageList.myPoint);
            myRanking.Rank = damageList.myRank > 99 ? "99+" : damageList.myRank.ToString();
            myRanking.Show = true;
        }


    }
    public IEnumerator RefreshTeamTime(int time)
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            time++;
            MainDataModel.Dungeon.UITime = GameUtils.GetTimeDiffString(time);
        }
        yield break;
    }

    public void RemoveBuff(BuffResult buff)
    {
        BuffCell buffCell;
        if (mBuffCellDictionary.TryGetValue(buff.BuffId, out buffCell))
        {
            mBuffCellDictionary.Remove(buff.BuffId);
            var index = BuffListDataModel.ShowBuffs.IndexOf(buffCell);
            if (index == mLastShowBuffInfoIndex)
            {
                BuffListDataModel.BuffInfoShow = false;
            }
            BuffListDataModel.ShowBuffs.Remove(buffCell);
            if (BuffListDataModel.ShowBuffs.Count == 0)
            {
                BuffListDataModel.BuffListShow = false;
            }
            BuffListDataModel.BuffCount = BuffListDataModel.ShowBuffs.Count;

            //是否鼓舞  鼓舞结束了把特效搞出来
            if (CheckGuWuPoint(buff.BuffTypeId))
            {
                MainDataModel.IsShowGuwuPoint = true;
            }
        }
    }

    public void RemoveBuffsOnDead(IEvent ievent)
    {
        var keyList = new List<uint>();

        var enumerator = mBuffCellDictionary.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var buff = enumerator.Current.Value;
            if (Table.GetBuff(buff.BuffId).DieDisappear == 1)
            {
                keyList.Add(enumerator.Current.Key);
                //是否鼓舞  鼓舞结束了把特效搞出来
                if (CheckGuWuPoint(buff.BuffId))
                {
                    MainDataModel.IsShowGuwuPoint = true;
                }
            }
        }

        var count = keyList.Count;
        for (var i = 0; i < count; i++)
        {
            BuffListDataModel.ShowBuffs.Remove(mBuffCellDictionary[keyList[i]]);
            mBuffCellDictionary.Remove(keyList[i]);
        }

        BuffListDataModel.BuffCount = BuffListDataModel.ShowBuffs.Count;
    }

    private void OnPlayerLevelUpGrade(IEvent ievent)
    {
        PlatformHelper.OnPlayerLevelUpGrade();
    }

    private void RewardMessageOpetion(IEvent ievent)
    {
        var e = ievent as RewardMessageOpetionClick;
        switch (e.Type)
        {
            case 0:
            {
                MainDataModel.IsShowGotoMsg = false;
                GameUtils.GotoUiTab(79, 0);
            }
                break;
            case 1:
            {
                MainDataModel.IsShowGotoMsg = false;
                var instance = PlayerDataManager.Instance;
                if (instance.RewardFastKeySortId == instance.NoticeStrToSort["DepotBagFree"])
                {
                    SceneNpcRecord tbSeneId = null;
                    var nowSceneId = GameLogic.Instance.Scene.SceneTypeId;
                    var tbScene = Table.GetScene(nowSceneId);
                    if (tbScene == null)
                    {
                        return;
                    }
                    if (tbScene.Type != 0 && tbScene.Type != 1)
                    {
//不在野外主城中//当前在副本中，无法前往仓库，请副本结束后再试
                        GameUtils.ShowHintTip(270296);
                        return;
                    }


                    var varSceneNpcId = BlacksceneNpcIds[0];
                    for (var i = 0; i < BlacksceneNpcIds.Count; i++)
                    {
                        tbSeneId = Table.GetSceneNpc(BlacksceneNpcIds[i]);
                        if (tbSeneId.SceneID == nowSceneId)
                        {
                            varSceneNpcId = BlacksceneNpcIds[i];
                            break;
                        }
                    }
                    tbSeneId = Table.GetSceneNpc(varSceneNpcId);
                    {
                        GameControl.Executer.Stop();
                        ObjManager.Instance.MyPlayer.LeaveAutoCombat();
                        //判断距离，如果还远，就寻路
                        var gotoCommand = GameControl.GoToCommand(tbSeneId.SceneID, (float) tbSeneId.PosX,
                            (float) tbSeneId.PosZ);
                        GameControl.Executer.PushCommand(gotoCommand);
                        var command1 =
                            new FuncCommand(
                                () => { EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.DepotUI)); });
                        GameControl.Executer.PushCommand(command1);
                    }
                }
            }
                break;
            case 2:
            {
                MainDataModel.IsShowGotoMsg = true;
            }
                break;
        }
    }

    public Vector3 RotaVector3(Vector3 start, Vector3 axit, float angle)
    {
        var aaa =
            new Vector3(
                (start.x - axit.x)*Mathf.Cos(angle*Mathf.PI/180) -
                (start.y - axit.y)*Mathf.Sin(angle*Mathf.PI/180)
                + axit.x,
                (start.x - axit.x)*Mathf.Sin(angle*Mathf.PI/180)
                + (start.y -
                   axit.y)
                *Mathf.Cos(angle*Mathf.PI/180) + axit.y);

        return aaa;
    }

    //设置自动战斗状态
    public void SetAutoCombat()
    {
        //if (isAuto == -1)
        //{
        //    MainDataModel.IsAutoFight = (MainDataModel.IsAutoFight + 1) % 2;
        //}
        //else
        //{
        //    MainDataModel.IsAutoFight = isAuto;
        //}
        //MainDataModel.IsAutoFight = (MainDataModel.IsAutoFight + 1) % 2;
        //ObjManager.Instance.MyPlayer.AutoCombatToggle(MainDataModel.IsAutoFight == 1);
        if (MainDataModel.IsAutoFight == 0)
        {
            ObjManager.Instance.MyPlayer.EnterAutoCombat();
        }
        else
        {
            ObjManager.Instance.MyPlayer.LeaveAutoCombat();
        }
    }

    private void SetDungeonTimerString()
    {
        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return;
        }
        var type = (eDungeonAssistType)tbFuben.AssistType;
        var state = (eDungeonState)PlayerDataManager.Instance.PlayerDataModel.DungeonState;
        var dictId = 41021;
        switch (type)
        {
            case eDungeonAssistType.AllianceWar:
                {
                    switch (state)
                    {
                        case eDungeonState.WillStart:
                            dictId = 41016;
                            break;
                        case eDungeonState.Start:
                            dictId = 41017;
                            break;
                        case eDungeonState.ExtraTime:
                            dictId = 41018;
                            break;
                    }
                }
                break;
            case eDungeonAssistType.MieShiWar:
                {
                    dictId = 300000034;
                }
                break;
        }
        DungeonTimeFormater = GameUtils.GetDictionaryText(dictId);

        if ((type == eDungeonAssistType.DevilSquare || type == eDungeonAssistType.BloodCastle || type == eDungeonAssistType.FrozenThrone
            || type == eDungeonAssistType.CastleCraft1 || type == eDungeonAssistType.CastleCraft2 || type == eDungeonAssistType.CastleCraft3
            || type == eDungeonAssistType.CastleCraft4 || type == eDungeonAssistType.CastleCraft5 || type == eDungeonAssistType.CastleCraft6) 
            && state == eDungeonState.WillStart)
        {
            MainDataModel.IsShowMiddleStartTime = true;
        }
        else
        {
            MainDataModel.IsShowMiddleStartTime = false;
        }

        if (state == eDungeonState.WillClose)
        {
            MainDataModel.IsShowInsprie = false;
        }
    }

    public void SetupEntry(DamageUnit damageUnit, BossRankEntry entry)
    {
        var rank = damageUnit.Rank;
        var damageStr = GameUtils.GetBigValueStr(damageUnit.Damage);
        if (_bossMaxHp > 0)
        {
            var percent = 100f*damageUnit.Damage/_bossMaxHp;
            if (percent > 0f)
            {
                percent = Math.Max(percent, 0.1f);
            }
            damageStr += "(" + percent.ToString("f1") + "%)";
        }
        entry.Name = damageUnit.Name;
        entry.Damage = damageStr;
        entry.Rank = rank > 99 ? "99+" : rank.ToString();
        entry.Show = true;
    }

    public void ShowActivityTip(IEvent ievent)
    {
        var e = ievent as ShowActivityTipEvent;
        if (MainDataModel.ShowActivityTip && TipStartTime > e.StartTime)
        {
            return;
        }
        TipFubenId = e.FubenId;
        TipDicId = e.DicId;
        TipStartTime = e.StartTime;
        TipTargetTime = e.TargetTime;
        MainDataModel.ShowActivityTip = true;
    }

    private void StartDungeonCountdown()
    {
        if (DungeonCoroutine == null && MainDataModel.DungeonTime > Game.Instance.ServerTime)
        {
            DungeonCoroutine = NetManager.Instance.StartCoroutine(DungeonCountdownCoroutine());
        }
    }

    private void StartMieshiStartCountdown()
    {
        if (MieshiStartCoroutine == null && MainDataModel.MieshiStartCounTime > Game.Instance.ServerTime)
        {
            MieshiStartCoroutine = NetManager.Instance.StartCoroutine(MieShiStartCountdownCoroutine());
        }
    }

    public IEnumerator TeamCancelCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var playerData = PlayerDataManager.Instance.PlayerDataModel.QueueUpData;
            var teamId = playerData.QueueId;
            var msg = NetManager.Instance.MatchingCancel(teamId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    playerData.QueueId = -1;
                    MainDataModel.Dungeon.IsWindowShow = false;
                    EventDispatcher.Instance.DispatchEvent(new QueueCanceledEvent());
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....MatchingCancel.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....MatchingCancel.......{0}.", msg.State);
            }
        }
    }

    public void UpDateDeviceInfo()
    {
        timeDuration += Time.deltaTime;
        if (timeDuration > 1f)
        {
            var ping = NetManager.Instance.Latency;
            DeviceInfoData.PingString = String.Format("ping:{0}ms", ping);
            if (ping < 200)
            {
                DeviceInfoData.PingColor = MColor.green;
            }
            else if (ping < 500)
            {
                DeviceInfoData.PingColor = MColor.yellow;
            }
            else
            {
                DeviceInfoData.PingColor = MColor.red;
            }

            DeviceInfoData.TimeString = DateTime.Now.ToString("HH:mm");
            timeDuration = 0;
        }

        batteryLevelDuration += Time.deltaTime;
        if (batteryLevelDuration > 60f)
        {
            DeviceInfoData.BatteryProgress = PlatformHelper.GetBatteryLevel();
            if (DeviceInfoData.BatteryProgress > 0.2f)
            {
                DeviceInfoData.BatteryProgressColor = MColor.white;
            }
            else
            {
                DeviceInfoData.BatteryProgressColor = MColor.red;
            }

            batteryLevelDuration = 0;
        }
    }

    public void UpdateTimer(IEvent ievent)
    {
        if (TipTargetTime >= Game.Instance.ServerTime)
        {
            MainDataModel.ActivityTip = string.Format(GameUtils.GetDictionaryText(TipDicId),
                Table.GetFuben(TipFubenId).Name,
                GameUtils.GetTimeDiffString(TipTargetTime));
        }
        else
        {
            MainDataModel.ShowActivityTip = false;
        }
    }

    public void CleanUp()
    {
        MainDataModel = new MainUIDataModel();
        RadarMapData = new RadarMapDataModel();
        BuffListDataModel = new BuffListDataModel();
        RadarMapData.SceneId = SceneManager.Instance.CurrentSceneTypeId;
        //MainDataModel.SkillTarget.IsResetTarget = 1;

        DeviceInfoData = new DeviceInfoDataModel();
        DeviceInfoData.DeviceNetWorkStatus = (NetworkStatus) PlatformHelper.GetNetworkState();
        DeviceInfoData.TimeString = DateTime.Now.ToString("HH:mm");
        DeviceInfoData.BatteryProgress = PlatformHelper.GetBatteryLevel();
        if (DeviceInfoData.BatteryProgress > 0.1f)
        {
            DeviceInfoData.BatteryProgressColor = Color.white;
        }
        else
        {
            DeviceInfoData.BatteryProgressColor = MColor.red;
        }
    }

    public void OnShow()
    {
        //if (GameLogic.Instance.Scene != null)
        //{
        //    Table.ForeachMapTransfer((record =>
        //    {
        //        if (record.SceneID != GameLogic.Instance.Scene.SceneTypeId)
        //        {
        //            return true;
        //        }
        //        if (record.Type == 0)
        //        {
        //            RararCharDataModel dataMode = new RararCharDataModel();
        //            dataMode.SpriteName = "map_icon_Transfer";
        //            dataMode.Width = 20;
        //            dataMode.Height = 20;

        //            var pos = RadarConvertLoction(new Vector3(record.PosX, 0, record.PosZ));
        //            dataMode.Loction = pos;
        //            dataMode.CharType = 0;
        //            RadarMapData.CharaModels.Add(dataMode);

        //            MainUiCharRadar e1 = new MainUiCharRadar(dataMode, 1);
        //            EventDispatcher.Instance.DispatchEvent(e1);
        //        }
        //        return true;
        //    }));
        //}
    }

    public void Close()
    {
        PlayerDataManager.Instance.CloseCharacterPopMenu();
    }

    public void Tick()
    {
        var f = Time.deltaTime;


        if (DateTime.Now > ShowTime)
        {
            MainDataModel.ShowMapName = false;
        }
        if (RadarMapData.SceneId == -1)
        {
            return;
        }

        duration += Time.deltaTime;
        if (duration > 0.5f)
        {
            ObjManager.Instance.UpdateRadarMapLoction(RadarMapData.CharaModels, mConvertRate);
            duration = 0;
        }
        var mp = MainDataModel.MissionProgress;
        if (mp.Speed > 0.0f)
        {
            mp.CurrentValue += mp.Speed*f;
            if (mp.CurrentValue >= 1.0f)
            {
                mp.CurrentValue = 1.0f;
                mp.Speed = 0.0f;
            }
        }


        buffRefreshDuration += Time.deltaTime;

        if (buffRefreshDuration >= 1)
        {
            buffRefreshDuration -= 1;

            var enumerator = mBuffCellDictionary.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var buff = enumerator.Current.Value;
                if (buff.BuffLastTime > 0)
                {
                    buff.BuffLastTime -= 1;
                    buff.TimeString = GetBuffTimeString((int) buff.BuffLastTime);
                    buff.BuffTimeShow = true;
                }
                else
                {
                    buff.BuffTimeShow = false;
                }
            }
        }

        if (fastReachTime >= 0)
        {
            fastReachTime -= Time.deltaTime;
            if (fastReachTime <= 0)
            {
                // 显示
                if (MainDataModel.IsShowFastReach != true)
                {
                    var freeTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e700);
                    //var doneTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e701);
                    var vipDoneTimes = PlayerDataManager.Instance.GetExData((int)eExdataDefine.e702);
                    var vipAdd = Table.GetVIP(PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel)).SentTimes;
                    var leftTimes = freeTimes + vipAdd - vipDoneTimes;
                    if (leftTimes <= 0)
                    {
                        if (!MainDataModel.CheckShowFastReachMessageBox)
                        {
                            MainDataModel.IsShowFastReach = true;
                            MainDataModel.FastReachTimes = "0";
                        }
                    }
                    else if (leftTimes > 99)
                    {
                        MainDataModel.IsShowFastReach = true;
                        MainDataModel.FastReachTimes = "";
                    }
                    else
                    {
                        MainDataModel.IsShowFastReach = true;
                        MainDataModel.FastReachTimes = leftTimes.ToString();
                    }
                }
            }
        }

        UpDateDeviceInfo();
        InitPetIslandBackInfo();
    }

    public void RefreshData(UIInitArguments data)
    {
        var id = SceneManager.Instance.CurrentSceneTypeId;
        var tbScene = Table.GetScene(id);
        if (tbScene != null)
        {
            MainDataModel.IsDungeon = Scene.IsDungeon(tbScene) ? 1 : 0;
            var isInFuben = Scene.IsInFuben(tbScene) ? 1 : 0;
            if (isInFuben == 0)
            {
                if (PlayerDataManager.Instance.CheckCondition(52) == 0 || !GameSetting.Instance.EnableNewFunctionTip)
                {
                    isInFuben = 0;
                }
                else
                {
                    isInFuben = 1;
                }
            }
            MainDataModel.IsInFuben = isInFuben;

            InitPetIslandBackInfo();
        }
        MainDataModel.IsMieshiDungeon = isInMieshiFuben() ? 1 : 0;
        MainDataModel.IsShowFastReachMessageBox = false;
    }

    private void InitPetIslandBackInfo()
    {
        var id = SceneManager.Instance.CurrentSceneTypeId;
        var tbScene = Table.GetScene(id);
        if (tbScene != null)
        {
            if (tbScene.TrackInfo != 5 || tbScene.FubenId == -1)
            {
                return;
            }
        }

        var str1 = Table.GetClientConfig(936).Value.ToString();
        if (str1 != null)
        {
            var vec1 = str1.Split('|');
            var temp = -1;
            foreach (var datas in vec1)
            {
                var tData = int.Parse(datas);
                var tableTime = Game.Instance.ServerTime.Date.AddSeconds(tData);
                var deltaSecond = (tableTime - DateTime.Now).TotalSeconds;
                if (deltaSecond > 0)
                {
                    temp = (int)deltaSecond;
                }
            }
            if (temp == -1)
            {
                // 超过今天所有时间了
                var tData = int.Parse(vec1[0]);
                var tableTime = Game.Instance.ServerTime.Date.AddDays(1); // 加一天
                tableTime = tableTime.AddSeconds(tData);
                var deltaSecond = (tableTime - DateTime.Now).TotalSeconds;
                temp = (int)deltaSecond;
            }
            MainDataModel.TdungeonInfo.StringList[0] = GameUtils.GetTimeDiffString(Game.Instance.ServerTime.AddSeconds(temp));
        }

        var str2 = Table.GetClientConfig(937).Value.ToString();
        if (str2 != null)
        {
            var vec2 = str2.Split('|');
            var temp = -1;
            foreach (var datas in vec2)
            {
                var tData = int.Parse(datas);
                var tableTime = Game.Instance.ServerTime.Date.AddSeconds(tData);
                var deltaSecond = (tableTime - DateTime.Now).TotalSeconds;
                if (deltaSecond > 0)
                {
                    temp = (int)deltaSecond;
                }
            }
            if (temp == -1)
            {
                // 超过今天所有时间了
                var tData = int.Parse(vec2[0]);
                var tableTime = Game.Instance.ServerTime.Date.AddDays(1); // 加一天
                tableTime = tableTime.AddSeconds(tData);
                var deltaSecond = (tableTime - DateTime.Now).TotalSeconds;
                temp = (int)deltaSecond;
            }
            MainDataModel.TdungeonInfo.StringList[1] = GameUtils.GetTimeDiffString(Game.Instance.ServerTime.AddSeconds(temp));
        }
    }
    public bool isInMieshiFuben()
    {

        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return false;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return false;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return false;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return false;
        }
        var type = (eDungeonAssistType)tbFuben.AssistType;
        return type == eDungeonAssistType.MieShiWar;
    }


    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "Radar")
        {
            return RadarMapData;
        }
        if (name == "MainUI")
        {
            return MainDataModel;
        }
        if (name.Equals("BuffList"))
        {
            return BuffListDataModel;
        }
        if (name.Equals("SelectTarget"))
        {
            return PlayerDataManager.Instance.SelectTargetData;
        }
        if (name.Equals("DeviceInfo"))
        {
            return DeviceInfoData;
        }

        return null;
    }

    public void OnChangeScene(int sceneId)
    {
        IsShowButton = 0;
        PlayerDataManager.Instance.SetCanShowTarget(IsShowButton == 0);
        PlayerDataManager.Instance.ResetSelectTarget();
        MainDataModel.MissionProgress.Show = false;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null || tbScene.FubenId == -1)
        {
            DungeonTime = Game.Instance.ServerTime.AddYears(-1);
        }
        MainDataModel.IsAutoFight = 0;
        MainDataModel.DungeonTimeStr = "";
        {
            // foreach(var entry in MainDataModel.BossRank.Entrys)
            var __enumerator2 = (MainDataModel.BossRank.Entrys).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var entry = __enumerator2.Current;
                {
                    entry.Damage = string.Empty;
                    entry.Name = string.Empty;
                    entry.Rank = string.Empty;
                    entry.Show = false;
                }
            }
        }
        MainDataModel.IsShowInsprie = false;
        MainDataModel.IsShowGuwuPoint = false;
        MainDataModel.IsShowMiddleStartTime = false;
        if (tbScene != null)
        {
            var tbFuben = Table.GetFuben(tbScene.FubenId);
            if (tbFuben != null)
            {
                var type = (eDungeonAssistType) tbFuben.AssistType;
                switch (type)
                {
                    case eDungeonAssistType.WorldBoss:
                        MainDataModel.BossRank.TitleId = 100000903;
                        break;
                    case eDungeonAssistType.CastleCraft1:
                    case eDungeonAssistType.CastleCraft2:
                    case eDungeonAssistType.CastleCraft3:
                    case eDungeonAssistType.CastleCraft4:
                    case eDungeonAssistType.CastleCraft5:
                    case eDungeonAssistType.CastleCraft6:
                        MainDataModel.BossRank.TitleId = 100000904;
                        break;
					case eDungeonAssistType.KillZone:
						MainDataModel.BossRank.TitleId = 271051;
						break;
                }
                MainDataModel.IsShowInsprie = tbFuben.CanInspire != -1;
                //鼓舞特效搞出来
                if (CheckGuWuPoint(0, true))
                {
                    MainDataModel.IsShowGuwuPoint = true;
                }

                if (tbFuben.AssistType == 4 || tbFuben.AssistType == 5 || tbFuben.AssistType == 6)
                {
//如果是恶魔，血色，或者世界boss，关闭活动副本tip
                    MainDataModel.ShowActivityTip = false;
                }

                //
                if (DungeonTime >= Game.Instance.ServerTime)
                {
                    SetDungeonTimerString();
                }
            }
        }
        if (sceneId == 9000)
        {
            var instance = PlayerDataManager.Instance;
            var mUnionMembers = instance.mUnionMembers;
            var playerId = instance.GetGuid();
            var battleCityDic = instance._battleCityDic;
            CharacterBaseInfoDataModel info;
            var IsDefence = false;
            foreach (var item in battleCityDic)
            {
                if (item.Value.Type == 0)
                {
                    if (instance.BattleUnionDataModel.MyUnion.UnionID == item.Key)
                    {
                        IsDefence = true;
                        break;
                    }
                }
            }
            if (IsDefence)
            {
                if (mUnionMembers.TryGetValue(playerId, out info))
                {
                    var tbAccess = Table.GetGuildAccess(info.Ladder);
                    if (tbAccess.CanRebornGuard == 1)
                    {
                        MainDataModel.IsShowAttackCityBtn = true;
                    }
                }
            }
        }
        else
        {
            MainDataModel.IsShowAttackCityBtn = false;
        }
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}