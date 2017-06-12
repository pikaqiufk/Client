#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class SettingController : IControllerBase
{
    public SettingController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(SettingExdataUpdate.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpdateEvent);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, OnFlagDataInit);
        EventDispatcher.Instance.AddEventListener(UIEvent_RefreshPush.EVENT_TYPE, RefreshPush);
        EventDispatcher.Instance.AddEventListener(UIEvent_QualitySetting.EVENT_TYPE, QualityChange);
        EventDispatcher.Instance.AddEventListener(UIEvent_ResolutionSetting.EVENT_TYPE, ResolutionChange);
        EventDispatcher.Instance.AddEventListener(UIEvent_VisibleEyeClick.EVENT_TYPE, OnVisibleEyeClick);
    }

    private Action QualityChanged;
    public SettingDataModel BackUpModel { get; set; }

    public AutoCombatData CombatData
    {
        get { return DataModel.AutoCombat; }
    }

    public SettingDataModel DataModel { get; set; }
    //技能自动瞄准开关
    public void AutoAim()
    {
        var visible = DataModel.SystemSetting.Other[4];
        GameSetting.Instance.TargetSelectionAssistant = !visible;
    }

    //摄像机震动开关
    public void CameraShake()
    {
        var visible = DataModel.SystemSetting.Other[5];
        GameSetting.Instance.CameraShakeEnable = !visible;
    }

    public bool CanPiackUpItem(int itemId)
    {
        var SetData = DataModel.AutoCombat;
        if (itemId == 2)
        {
            if (SetData.Pickups[4])
            {
                return true;
            }
            return false;
        }

        var tbItem = Table.GetItemBase(itemId);
        if (tbItem.Type >= 10000 && tbItem.Type <= 10099)
        {
            if (SetData.Pickups[tbItem.Quality])
            {
                return true;
            }
            return false;
        }
        if (tbItem.Type == 30000)
        {
            if (SetData.Pickups[5])
            {
                return true;
            }
            return false;
        }

        if (tbItem.Type == 24000)
        {
            if (SetData.Pickups[6])
            {
                return true;
            }
            return false;
        }
        if (SetData.Pickups[7])
        {
            return true;
        }
        return false;
    }

    private void ChangeFps()
    {
        PlayerPrefs.SetInt(GameSetting.LowFpsKey, DataModel.SystemSetting.Other[7] ? 30 : 60);
        var fps = DataModel.SystemSetting.Other[7] ? 30 : 60;
        Application.targetFrameRate = fps;
    }

    public void OnExDataInit(IEvent ievent)
    {
        var e = ievent as ExDataInitEvent;
        var playerData = PlayerDataManager.Instance;
        CombatData.Hp = playerData.GetExData(59)/100.0f;
        CombatData.Mp = playerData.GetExData(60)/100.0f;
        var pick = playerData.GetExData(61);
        for (var i = 0; i < 8; i++)
        {
            CombatData.Pickups[i] = BitFlag.GetLow(pick, i);
        }
        if (BitFlag.GetLow(pick, 9))
        {
            CombatData.Ranges[2] = true;
            CombatData.Ranges[0] = false;
            CombatData.Ranges[1] = false;
        }
        else
        {
            if (BitFlag.GetLow(pick, 8))
            {
                CombatData.Ranges[1] = true;
                CombatData.Ranges[0] = false;
                CombatData.Ranges[2] = false;
            }
            else
            {
                CombatData.Ranges[0] = true;
                CombatData.Ranges[1] = false;
                CombatData.Ranges[2] = false;
            }
        }
    }

    public void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as SettingExdataUpdate;
        var playerData = PlayerDataManager.Instance;
        switch (e.Type)
        {
            case eExdataDefine.e59:
            {
                CombatData.Hp = playerData.GetExData(eExdataDefine.e59)/100.0f;
            }
                break;
            case eExdataDefine.e60:
            {
                CombatData.Mp = playerData.GetExData(eExdataDefine.e60)/100.0f;
            }
                break;
            case eExdataDefine.e61:
            {
                var pick = playerData.GetExData(eExdataDefine.e61);
                if (BitFlag.GetLow(pick, 9))
                {
                    CombatData.Ranges[2] = true;
                    CombatData.Ranges[0] = false;
                    CombatData.Ranges[1] = false;
                }
                else
                {
                    if (BitFlag.GetLow(pick, 8))
                    {
                        CombatData.Ranges[1] = true;
                        CombatData.Ranges[0] = false;
                        CombatData.Ranges[2] = false;
                    }
                    else
                    {
                        CombatData.Ranges[0] = true;
                        CombatData.Ranges[1] = false;
                        CombatData.Ranges[2] = false;
                    }
                }
                for (var i = 0; i < 8; i++)
                {
                    CombatData.Pickups[i] = BitFlag.GetLow(pick, i);
                }
            }
                break;
        }
    }

    public void OnVisibleEyeClick(IEvent ievent)
    {
        var e = ievent as UIEvent_VisibleEyeClick;
        if (e == null ) return;

        DataModel.SystemSetting.Other[2] = e.Visible;
        DataModel.SystemSetting.Other[3] = e.Visible;
    }

    public void OnFlagDataInit(IEvent ievent)
    {
        DataModel.SystemSetting.Other[0] = PlayerDataManager.Instance.GetFlag(480);
        DataModel.SystemSetting.Other[1] = PlayerDataManager.Instance.GetFlag(481);
        DataModel.SystemSetting.Other[2] = PlayerDataManager.Instance.GetFlag(482);
        DataModel.SystemSetting.Other[3] = PlayerDataManager.Instance.GetFlag(483);
        DataModel.SystemSetting.Other[4] = PlayerDataManager.Instance.GetFlag(488);
        DataModel.SystemSetting.Other[5] = PlayerDataManager.Instance.GetFlag(489);
        DataModel.SystemSetting.Other[6] = PlayerDataManager.Instance.GetFlag(490);
        RefreshVisibleEye();
        OtherPlayerVisable();
        OtherPlayerEffectVisable();
        CameraShake();
        AutoAim();
        PowerSave();
    }

    public void OnFlagUpdateEvent(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        var index = e.Index;
        //拒绝组队
        if (index == 480)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[0] != e.Value)
            {
                DataModel.SystemSetting.Other[0] = e.Value;
            }
        }
        //拒绝私聊
        else if (index == 481)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[1] != e.Value)
            {
                DataModel.SystemSetting.Other[1] = e.Value;
            }
        }
        //屏蔽其他玩家
        else if (index == 482)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[2] != e.Value)
            {
                DataModel.SystemSetting.Other[2] = e.Value;
            }
        }
        //屏蔽他人特效
        else if (index == 483)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[3] != e.Value)
            {
                DataModel.SystemSetting.Other[3] = e.Value;
            }
        }
        //技能自动瞄准
        else if (index == 488)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[4] != e.Value)
            {
                DataModel.SystemSetting.Other[4] = e.Value;
            }
        }
        //摄像机震动
        else if (index == 489)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[5] != e.Value)
            {
                DataModel.SystemSetting.Other[5] = e.Value;
            }
        }
        //屏幕节电
        else if (index == 490)
        {
            if (DataModel.SystemSetting.Other != null && DataModel.SystemSetting.Other[6] != e.Value)
            {
                DataModel.SystemSetting.Other[6] = e.Value;
            }
        }
    }

    public void OtherPlayerEffectVisable()
    {
        var visible = DataModel.SystemSetting.Other[3];
        GameSetting.Instance.ShowEffect = !visible;
    }

    public void OtherPlayerVisable()
    {
        var visible = DataModel.SystemSetting.Other[2];
        GameSetting.Instance.ShowOtherPlayer = !visible;
    }

    //屏幕节电
    public void PowerSave()
    {
        var visible = DataModel.SystemSetting.Other[6];
        GameSetting.Instance.PowerSaveEnabe = !visible;
    }

    private void QualityChange(IEvent ievent)
    {
        var e = ievent as UIEvent_QualitySetting;
        DataModel.SystemSetting.QualityToggle = e.level;
        GameSetting.Instance.GameQualityLevel = e.level;
    }

    public void RegisterPropertyChanged()
    {
        DataModel.PushList.PropertyChanged += (sender, args) =>
        {
            int id;
            if (int.TryParse(args.PropertyName, out id))
            {
                var key = string.Format("PushKey{0}", id);
                var value = DataModel.PushList[id] ? 1 : 0;
                PlayerPrefs.SetInt(key, value);
                RefreshPushById(id);
            }
        };

        DataModel.SystemSetting.Other.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "0")
            {
                PlayerDataManager.Instance.SetFlag(480, DataModel.SystemSetting.Other[0]);
                PlatformHelper.Event("setting", "other", 0);
            }
            else if (args.PropertyName == "1")
            {
                PlayerDataManager.Instance.SetFlag(481, DataModel.SystemSetting.Other[1]);
                PlatformHelper.Event("setting", "other", 1);
            }
            else if (args.PropertyName == "2")
            {
                OtherPlayerVisable();
                PlayerDataManager.Instance.SetFlag(482, DataModel.SystemSetting.Other[2]);
                PlatformHelper.Event("setting", "other", 2);
                RefreshVisibleEye();
            }
            else if (args.PropertyName == "3")
            {
                OtherPlayerEffectVisable();
                PlayerDataManager.Instance.SetFlag(483, DataModel.SystemSetting.Other[3]);
                PlatformHelper.Event("setting", "other", 3);
                RefreshVisibleEye();
            }
            else if (args.PropertyName == "4")
            {
                AutoAim();
                PlayerDataManager.Instance.SetFlag(488, DataModel.SystemSetting.Other[4]);
                PlatformHelper.Event("setting", "other", 4);
            }
            else if (args.PropertyName == "5")
            {
                CameraShake();
                PlayerDataManager.Instance.SetFlag(489, DataModel.SystemSetting.Other[5]);
                PlatformHelper.Event("setting", "other", 5);
            }
            else if (args.PropertyName == "6")
            {
                PowerSave();
                PlayerDataManager.Instance.SetFlag(490, DataModel.SystemSetting.Other[6]);
                PlatformHelper.Event("setting", "other", 6);
            }
            else if (args.PropertyName == "7")
            {
                ChangeFps();
            }
        };

        DataModel.SystemSetting.Sound.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "0")
            {
                SoundManager.Instance.EnableBGM = !DataModel.SystemSetting.Sound[0];
                PlayerPrefs.SetInt(SoundManager.BGMPrefsKey, SoundManager.Instance.EnableBGM ? 1 : 0);
                PlatformHelper.Event("setting", "sound", 0);
            }
            else if (args.PropertyName == "1")
            {
                SoundManager.Instance.EnableSFX = !DataModel.SystemSetting.Sound[1];
                PlayerPrefs.SetInt(SoundManager.SFXPrefsKey, SoundManager.Instance.EnableSFX ? 1 : 0);
                PlatformHelper.Event("setting", "sound", 1);
            }
        };

        DataModel.AutoCombat.Pickups.PropertyChanged +=
            (sender, args) => { EventDispatcher.Instance.DispatchEvent(new UIEvent_PickSettingChanged()); };
    }

    private void RefreshVisibleEye()
    {
        if (DataModel.SystemSetting.Other[3] && DataModel.SystemSetting.Other[2])
        {
            DataModel.SystemSetting.VisibleEye = 0;
            DataModel.SystemSetting.VisibleEyeTipShow = false;
        }
        else
        {
            DataModel.SystemSetting.VisibleEye = 1;
        }
    }
    public IEnumerator ReloadSceneCorotinue(int level)
    {
        using (new BlockingLayerHelper(0))
        {
            var placeHolder = 0;
            var msg = NetManager.Instance.ApplyPlayerData(placeHolder);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //目前只把buff列表发过来了,其他东西客户端自己都知道
                    var oldData = PlayerDataManager.Instance.mInitBaseAttr;
                    var myPlayer = ObjManager.Instance.MyPlayer;
                    var data = new PlayerData();
                    data.CharacterId = myPlayer.CharacterBaseData.CharacterId;
                    data.SceneId = oldData.SceneId;
                    data.Name = myPlayer.Name;
                    data.RoleId = myPlayer.RoleId;
                    data.Level = myPlayer.GetLevel();
                    data.MoveSpeed = myPlayer.GetMoveSpeed();
                    data.Camp = myPlayer.GetCamp();
                    data.X = myPlayer.Position.x;
                    data.Y = myPlayer.Position.z;
                    data.MpMax = PlayerDataManager.Instance.GetAttribute(eAttributeType.MpMax);
                    data.MpMow = PlayerDataManager.Instance.GetAttribute(eAttributeType.MpNow);
                    data.HpMax = PlayerDataManager.Instance.GetAttribute(eAttributeType.HpMax);
                    data.HpNow = PlayerDataManager.Instance.GetAttribute(eAttributeType.HpNow);
                    data.AreaState = (int) myPlayer.AreaState;
                    var enumeraotr = myPlayer.EquipList.GetEnumerator();
                    while (enumeraotr.MoveNext())
                    {
                        var equip = enumeraotr.Current;
                        data.EquipsModel.Add(equip.Key, equip.Value);
                    }
                    //暂时没有用到
                    data.SceneGuid = oldData.SceneGuid;

                    var buffs = msg.Response.Buff;
                    data.Buff.AddRange(buffs);
                    PlayerDataManager.Instance.mInitBaseAttr = data;
                    yield return new WaitForSeconds(0.1f);
                    //QualityChanged = () =>
                    {
                        //手机上设置质量等级会卡主,所以挪到这里执行,缓存里的资源也需要从新加载
                        ResourceManager.Instance.ClearCache(true);
                        GameSetting.Instance.GameQualityLevel = level;
                        PlayerPrefs.SetInt(GameSetting.GameQuilatyKey, level);
                    }
                    ;
                    Application.LoadLevel("Loading");
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void ResolutionChange(IEvent ievent)
    {
        var e = ievent as UIEvent_ResolutionSetting;
        DataModel.SystemSetting.Resolution = e.level;
        GameSetting.Instance.GameResolutionLevel = e.level;
    }

    public void CleanUp()
    {
        DataModel = new SettingDataModel();
        BackUpModel = new SettingDataModel();

        DataModel.SystemSetting.Sound[0] = PlayerPrefs.GetInt(SoundManager.BGMPrefsKey, 1) == 0;
        DataModel.SystemSetting.Sound[1] = PlayerPrefs.GetInt(SoundManager.SFXPrefsKey, 1) == 0;
        SoundManager.Instance.EnableBGM = !DataModel.SystemSetting.Sound[0];
        SoundManager.Instance.EnableSFX = !DataModel.SystemSetting.Sound[1];

        var QualityLevel = PlayerPrefs.GetInt(GameSetting.GameQuilatyKey, GameSetting.Instance.GameQualityLevel);
        DataModel.SystemSetting.QualityToggle = QualityLevel;
        var resolutionLevel = PlayerPrefs.GetInt(GameSetting.GameResolutionKey, GameSetting.Instance.GameResolutionLevel);
        DataModel.SystemSetting.Resolution = resolutionLevel;

        var lowFps = PlayerPrefs.GetInt(GameSetting.LowFpsKey, 30);
        DataModel.SystemSetting.Other[7] = (lowFps == 30);

        RegisterPropertyChanged();

        var tbConfig = Table.GetClientConfig(1007);
        var lowfps = tbConfig.Value.ToInt();
        if (lowfps > 1)
        {
            lowFrameTime = 1f / lowfps;
        }
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "CanPiackUpItem")
        {
            return CanPiackUpItem((int) param[0]);
        }
        if (name.Equals("GetQualityChanged"))
        {
            return QualityChanged;
        }
        if (name.Equals("SetQualityChanged"))
        {
            QualityChanged = null;
        }
        if(name.Equals("GetEyeIsOpen"))
        {
            return (DataModel.SystemSetting.VisibleEye == 1);
        }

        return null;
    }

    public void OnShow()
    {
    }

    private float timeinterval = 0;
    private float lastTime = 0;
    private int lastFrameCount = 0;
    private float lowFrameTime = 0.1f;
    public void Tick()
    {
        timeinterval += Time.deltaTime;

        if (timeinterval < 5)
        {
            return;
        }
        timeinterval -= 5;

        if (lastFrameCount == 0)
        {
            lastFrameCount = Time.frameCount;
            lastTime = Time.fixedTime;
            return;
        }

        var frameTime = (Time.fixedTime - lastTime)/(Time.frameCount - lastFrameCount);

        if (DataModel.SystemSetting.VisibleEye == 1 && frameTime > lowFrameTime)
        {
            DataModel.SystemSetting.VisibleEyeTipShow = true;
        }
        else
        {
            DataModel.SystemSetting.VisibleEyeTipShow = false;
        }

        lastFrameCount = Time.frameCount;
        lastTime = Time.fixedTime;
    }

    public void RefreshData(UIInitArguments data)
    {
        BackUpModel.AutoCombat.Hp = DataModel.AutoCombat.Hp;
        BackUpModel.AutoCombat.Mp = DataModel.AutoCombat.Mp;
        for (var i = 0; i < 3; i++)
        {
            BackUpModel.AutoCombat.Ranges[i] = DataModel.AutoCombat.Ranges[i];
        }
        for (var i = 0; i < 8; i++)
        {
            BackUpModel.AutoCombat.Pickups[i] = DataModel.AutoCombat.Pickups[i];
        }

        for (var i = 0; i < DataModel.SystemSetting.Other.Count; i++)
        {
            BackUpModel.SystemSetting.Other[i] = DataModel.SystemSetting.Other[i];
        }

        var QualityLevel = PlayerPrefs.GetInt(GameSetting.GameQuilatyKey, GameSetting.Instance.GameQualityLevel);
        var ResolutionLevel = PlayerPrefs.GetInt(GameSetting.GameResolutionKey, GameSetting.Instance.GameResolutionLevel);

        DataModel.SystemSetting.QualityToggle = QualityLevel;
        DataModel.SystemSetting.Resolution = ResolutionLevel;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "AutoCombat")
        {
            return DataModel.AutoCombat;
        }
        return DataModel;
    }

    public void Close()
    {
        var dic = new Dictionary<int, int>();
        if (Math.Abs(BackUpModel.AutoCombat.Hp - DataModel.AutoCombat.Hp) > 0.01)
        {
            dic.Add(59, (int) (DataModel.AutoCombat.Hp*100));
        }
        if (Math.Abs(BackUpModel.AutoCombat.Mp - DataModel.AutoCombat.Mp) > 0.01)
        {
            dic.Add(60, (int) (DataModel.AutoCombat.Mp*100));
        }

        BackUpModel.AutoCombat.Mp = DataModel.AutoCombat.Mp;

        var chgPick = false;
        var flag61 = 0;
        for (var i = 0; i < 8; i++)
        {
            if (BackUpModel.AutoCombat.Pickups[i] != DataModel.AutoCombat.Pickups[i])
            {
                chgPick = true;
                break;
            }
        }

        flag61 = 0;

        for (var i = 0; i < 8; i++)
        {
            if (DataModel.AutoCombat.Pickups[i])
            {
                flag61 = BitFlag.IntSetFlag(flag61, i);
            }
        }


        var chgRang = false;
        for (var i = 0; i < 3; i++)
        {
            if (BackUpModel.AutoCombat.Ranges[i] != DataModel.AutoCombat.Ranges[i])
            {
                chgRang = true;
                break;
            }
        }

        for (var i = 0; i < 3; i++)
        {
            if (DataModel.AutoCombat.Ranges[i])
            {
                if (i == 1)
                {
                    flag61 = BitFlag.IntSetFlag(flag61, 8);
                }
                else if (i == 2)
                {
                    flag61 = BitFlag.IntSetFlag(flag61, 9);
                }
            }
        }

        if (chgPick || chgRang)
        {
            dic.Add(61, flag61);
        }

        if (dic.Count > 0)
        {
            PlayerDataManager.Instance.SetExDataNet(dic);
        }

        var tureArray = new Int32Array();
        var falseArray = new Int32Array();

        for (var i = 0; i < 4; i++)
        {
            if (DataModel.SystemSetting.Other[i] != BackUpModel.SystemSetting.Other[i])
            {
                if (DataModel.SystemSetting.Other[i])
                {
                    tureArray.Items.Add(480 + i);
                }
                else
                {
                    falseArray.Items.Add(480 + i);
                }
            }
        }


        //后添加的两个设置
        //又增加一个屏幕节能开关
        if (DataModel.SystemSetting.Other[4] != BackUpModel.SystemSetting.Other[4])
        {
            if (DataModel.SystemSetting.Other[4])
            {
                tureArray.Items.Add(488);
            }
            else
            {
                falseArray.Items.Add(488);
            }
        }

        if (DataModel.SystemSetting.Other[5] != BackUpModel.SystemSetting.Other[5])
        {
            if (DataModel.SystemSetting.Other[5])
            {
                tureArray.Items.Add(489);
            }
            else
            {
                falseArray.Items.Add(489);
            }
        }

        if (DataModel.SystemSetting.Other[6] != BackUpModel.SystemSetting.Other[6])
        {
            if (DataModel.SystemSetting.Other[6])
            {
                tureArray.Items.Add(490);
            }
            else
            {
                falseArray.Items.Add(490);
            }
        }


        if (tureArray.Items.Count > 0 || falseArray.Items.Count > 0)
        {
            PlayerDataManager.Instance.SetFlagNet(tureArray, falseArray);
        }
    }

    public FrameState State { get; set; }

    #region 推送相关

    /// <summary>
    ///     0 世界boss
    ///     1 古堡争霸
    ///     2 邪恶监牢
    ///     3 诅咒堡垒
    ///     4 黄金部队刷新
    ///     5 地图统领刷新
    ///     6 头脑风暴
    ///     7 免费精灵抽奖
    ///     8 免费许愿池抽奖
    ///     9 孵化室完成
    ///     10 矿洞满
    ///     11 伐木场满
    ///     12 预留----------
    ///     13
    ///     14
    ///     15
    ///     16
    ///     17
    /// </summary>
    private void initPush()
    {
        var count = DataModel.PushList.Count;
        for (var i = 0; i < count; i++)
        {
            var key = string.Format("PushKey{0}", i);
            var defaultValue = 0;
            if (i == 0 || i == 1 || i > 6)
            {
                defaultValue = 1;
            }
            else
            {
                defaultValue = 0;
            }
            DataModel.PushList[i] = PlayerPrefs.GetInt(key, defaultValue) == 1;
        }

        RefreshAllPush();
    }

    private void RefreshPush(IEvent ievent)
    {
        var e = ievent as UIEvent_RefreshPush;
        RefreshPushById(e.id);
    }

    private void RefreshPushById(int id)
    {
        switch (id)
        {
            case -1:
                initPush();
                break;
            case 0:
                WorldBossPush();
                break;
            case 1:
                CityBattlePush();
                break;
            case 2:
                EvilDungeonPush();
                break;
            case 3:
                CurseCastlePush();
                break;
            case 4:
                GoldenArmyPush();
                break;
            case 5:
                MapLordPush();
                break;
            case 6:
                BrainStromPush();
                break;
            case 7:
                ElfDrawPush();
                break;
            case 8:
                WishingPoolPush();
                break;
            case 9:
                HatchingHousePush();
                break;
            case 10:
                MinePush();
                break;
            case 11:
                LogPlacePush();
                break;
        }
    }


    private void RefreshAllPush()
    {
        WorldBossPush();
        CityBattlePush();
        EvilDungeonPush();
        CurseCastlePush();
        GoldenArmyPush();
        MapLordPush();
        BrainStromPush();
        ElfDrawPush();
        WishingPoolPush();
        HatchingHousePush();
        MinePush();
        LogPlacePush();
    }

    private void WorldBossPush()
    {
        for (var i = 0; i < 7; i++)
        {
            var key = string.Format("worldboss{0}", i);
            PlatformHelper.DeleteLocalNotificationWithKey(key);
        }

        if (!CheckCondition(0))
        {
            return;
        }
        var now = Game.Instance.ServerTime;
        var targetTime = new DateTime(now.Year, now.Month, now.Day, 12, 05, 0);
        for (var i = 0; i < 7; i++)
        {
            var key = string.Format("worldboss{0}", i);
            var target = targetTime.AddDays(i);
            if (target < now)
            {
                continue;
            }
            var diff = target - now;
            PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240136), diff.TotalSeconds);
        }
    }

    // 古堡争霸
    private void CityBattlePush()
    {
        for (var i = 0; i < 7; i++)
        {
            var key = string.Format("CityBattle{0}", i);
            PlatformHelper.DeleteLocalNotificationWithKey(key);
        }

        if (!CheckCondition(1))
        {
            return;
        }
        var now = Game.Instance.ServerTime;
        var targetTime = new DateTime(now.Year, now.Month, now.Day, 21, 0, 0);
        for (var i = 0; i < 7; i++)
        {
            var key = string.Format("CityBattle{0}", i);
            var target = targetTime.AddDays(i);
            if (target < now)
            {
                continue;
            }
            var diff = target - now;
            PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240149), diff.TotalSeconds);
        }
    }

    private void EvilDungeonPush()
    {
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var key = string.Format("EvilDungeon{0}H{1}", i, j);
                PlatformHelper.DeleteLocalNotificationWithKey(key);
            }
        }

        if (!CheckCondition(2))
        {
            return;
        }

        var controller = UIManager.Instance.GetController(UIConfig.ActivityUI);
        var IsMaxCount = controller.CallFromOtherClass("IsDevilSquareMaxCount", null);
        var now = Game.Instance.ServerTime;
        var targetTime = new DateTime(now.Year, now.Month, now.Day, 10, 15, 0);
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var key = string.Format("EvilDungeon{0}H{1}", i, j);
                var diff = targetTime - now;
                targetTime = targetTime.AddHours(1);
                if (i == 0 && (bool) IsMaxCount)
                {
                    continue;
                }
                PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240150), diff.TotalSeconds);
            }
            targetTime = new DateTime(now.Year, now.Month, now.AddDays(1).Day, 10, 15, 0);
        }
    }

    private void CurseCastlePush()
    {
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var key = string.Format("CurseCastleD{0}H{1}", i, j);
                PlatformHelper.DeleteLocalNotificationWithKey(key);
            }
        }

        if (!CheckCondition(3))
        {
            return;
        }

        var controller = UIManager.Instance.GetController(UIConfig.ActivityUI);
        var IsMaxCount = controller.CallFromOtherClass("IsBloodCastleMaxCount", null);
        var now = Game.Instance.ServerTime;
        var targetTime = new DateTime(now.Year, now.Month, now.Day, 10, 35, 0);
        for (var i = 0; i < 2; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var key = string.Format("CurseCastleD{0}H{1}", i, j);
                var diff = targetTime - now;
                targetTime = targetTime.AddHours(1);
                if (i == 0 && (bool) IsMaxCount)
                {
                    continue;
                }
                PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240151), diff.TotalSeconds);
            }
            targetTime = new DateTime(now.Year, now.Month, now.AddDays(1).Day, 10, 35, 0);
        }
    }

    private void GoldenArmyPush()
    {
        for (var i = 0; i < 7; i++)
        {
            var key1 = string.Format("GoldenArmyAM{0}", i);
            var key2 = string.Format("GoldenArmyPM{0}", i);
            PlatformHelper.DeleteLocalNotificationWithKey(key1);
            PlatformHelper.DeleteLocalNotificationWithKey(key2);
        }

        if (!CheckCondition(4))
        {
            return;
        }

        var now = Game.Instance.ServerTime;
        var targetTime1 = new DateTime(now.Year, now.Month, now.Day, 11, 55, 0);
        var targetTime2 = new DateTime(now.Year, now.Month, now.Day, 19, 55, 0);
        for (var i = 0; i < 7; i++)
        {
            var key1 = string.Format("GoldenArmyAM{0}", i);
            var key2 = string.Format("GoldenArmyPM{0}", i);
            var diff1 = targetTime1 - now;
            var diff2 = targetTime2 - now;
            targetTime1 = targetTime1.AddDays(1);
            targetTime2 = targetTime2.AddDays(1);

            PlatformHelper.SetLocalNotification(key1, GameUtils.GetDictionaryText(240152), diff1.TotalSeconds);
            PlatformHelper.SetLocalNotification(key2, GameUtils.GetDictionaryText(240152), diff2.TotalSeconds);
        }
    }

    private void MapLordPush()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 5; j++)
            {
                var key = string.Format("MapLordD{0}H{1}", i, j);
                PlatformHelper.DeleteLocalNotificationWithKey(key);
            }
        }

        if (!CheckCondition(5))
        {
            return;
        }

        var now = Game.Instance.ServerTime;
        for (var i = 0; i < 3; i++)
        {
            var targetTime = new DateTime(now.Year, now.Month, now.Day, 8, 55, 0);
            for (var j = 0; j < 5; j++)
            {
                var key = string.Format("MapLordD{0}H{1}", i, j);
                PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240153),
                    (targetTime - Game.Instance.ServerTime).TotalSeconds);
                if (j == 0)
                {
                    targetTime = targetTime.AddHours(4);
                }
                else
                {
                    targetTime = targetTime.AddHours(3);
                }
            }
            now = now.AddDays(1);
        }
    }

    private void BrainStromPush()
    {
        for (var i = 0; i < 7; i++)
        {
            var key = string.Format("BrainStrom{0}", i);
            PlatformHelper.DeleteLocalNotificationWithKey(key);
        }

        if (!CheckCondition(6))
        {
            return;
        }
        var controller = UIManager.Instance.GetController(UIConfig.AnswerUI);
        var ret = controller.CallFromOtherClass("IsMaxAnser", null);

        var now = Game.Instance.ServerTime;

        for (var i = 0; i < 7; i++)
        {
            var key = string.Format("BrainStrom{0}", i);
            var targetTime = new DateTime(now.Year, now.Month, now.Day, 21, 30, 0);
            if (i == 0)
            {
                if (!(bool) ret)
                {
                    PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240154),
                        (targetTime - Game.Instance.ServerTime).TotalSeconds);
                }
            }
            else
            {
                PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240154),
                    (targetTime - Game.Instance.ServerTime).TotalSeconds);
            }

            now = now.AddDays(1);
        }
    }


    private void ElfDrawPush()
    {
        const string key = "ElfDraw";
        PlatformHelper.DeleteLocalNotificationWithKey(key);

        if (!CheckCondition(7))
        {
            return;
        }
        var controller = UIManager.Instance.GetController(UIConfig.ElfUI);
        var ret = controller.CallFromOtherClass("GetIsFreeDraw", null);
        if ((int?) ret == 0)
        {
            var targetTime = Game.Instance.ServerTime.AddDays(1).Date;
            PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240155),
                (targetTime - Game.Instance.ServerTime).TotalSeconds);
        }
    }

    private void WishingPoolPush()
    {
        const string key = "WishingPool";
        PlatformHelper.DeleteLocalNotificationWithKey(key);

        if (!CheckCondition(8))
        {
            return;
        }

        var controller = UIManager.Instance.GetController(UIConfig.WishingUI);
        var ret = controller.CallFromOtherClass("GetNextFreeTime", null);

        if (ret != null)
        {
            var dateData = (long) ret;
            var now = Game.Instance.ServerTime;
            var targetTime = Extension.FromServerBinary(dateData);

            if (targetTime > now)
            {
                PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240156),
                    (targetTime - now).TotalSeconds);
            }
        }
    }

    private void HatchingHousePush()
    {
//         for (var i = 0; i < 5; i++)
//         {
//             var key = string.Format("HatchingHouse{0}", i);
//             PlatformHelper.DeleteLocalNotificationWithKey(key);
//         }
// 
//         if (!CheckCondition(9))
//         {
//             return;
//         }
// 
//         var controller = UIManager.Instance.GetController(UIConfig.HatchingHouse);
//         var ret = controller.CallFromOtherClass("GetLastTimeList", null);
//         var list = ret as List<long>;
//         if (list == null)
//         {
//             return;
//         }
// 
//         var c = Math.Min(5, list.Count);
//         var now = Game.Instance.ServerTime;
//         for (var i = 0; i < c; i++)
//         {
//             var key = string.Format("HatchingHouse{0}", i);
//             var targetTime = Extension.FromServerBinary(list[i]);
//             PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240157),
//                 (targetTime - now).TotalSeconds);
//         }
    }


    private void MinePush()
    {
//         const string key = "MinePush";
//         PlatformHelper.DeleteLocalNotificationWithKey(key);
// 
//         if (!CheckCondition(10))
//         {
//             return;
//         }
//         var controller = UIManager.Instance.GetController(UIConfig.CityUI);
//         var ret = controller.CallFromOtherClass("GetMineMaxTime", null);
//         if (ret != null)
//         {
//             var now = Game.Instance.ServerTime;
//             var targetTime = ret as DateTime? ?? new DateTime();
//             PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240158),
//                 (targetTime - now).TotalSeconds);
//         }
    }

    private void LogPlacePush()
    {
//         const string key = "LogPlacePush";
//         PlatformHelper.DeleteLocalNotificationWithKey(key);
// 
//         if (!CheckCondition(11))
//         {
//             return;
//         }
// 
//         var controller = UIManager.Instance.GetController(UIConfig.CityUI);
//         var ret = controller.CallFromOtherClass("GetWoodMaxTime", null);
//         if (ret != null)
//         {
//             var now = Game.Instance.ServerTime;
//             var targetTime = ret as DateTime? ?? new DateTime();
//             PlatformHelper.SetLocalNotification(key, GameUtils.GetDictionaryText(240159),
//                 (targetTime - now).TotalSeconds);
//         }
    }

    private bool CheckCondition(int id)
    {
        var conditionId = -1;
        DailyActivityRecord table;
        RewardInfoRecord table2;
        switch (id)
        {
            case 0:
                table = Table.GetDailyActivity(1002);
                conditionId = table.OpenCondition;
                break;
            case 1:
                table = Table.GetDailyActivity(1006);
                conditionId = table.OpenCondition;
                break;
            case 2:
                table = Table.GetDailyActivity(1000);
                conditionId = table.OpenCondition;
                break;
            case 3:
                table = Table.GetDailyActivity(1001);
                conditionId = table.OpenCondition;
                break;
            case 4:
                table = Table.GetDailyActivity(1003);
                conditionId = table.OpenCondition;
                break;
            case 5:
                table = Table.GetDailyActivity(1004);
                conditionId = table.OpenCondition;
                break;
            case 6:
                table = Table.GetDailyActivity(1005);
                conditionId = table.OpenCondition;
                break;
            case 7:
                table2 = Table.GetRewardInfo(1);
                conditionId = table2.ConditionId;
                break;
            case 8:
                table2 = Table.GetRewardInfo(0);
                conditionId = table2.ConditionId;
                break;
            case 9:
//                 table2 = Table.GetRewardInfo(9);
//                 conditionId = table2.ConditionId;
                break;
            case 10:
//                 table2 = Table.GetRewardInfo(12);
//                 conditionId = table2.ConditionId;
                break;
            case 11:
//                 table2 = Table.GetRewardInfo(13);
//                 conditionId = table2.ConditionId;
                break;
        }

        if (conditionId == -1)
        {
            return false;
        }

        var result = DataModel.PushList[id] && PlayerDataManager.Instance.CheckCondition(conditionId) == 0;
        return result;
    }

    #endregion
}