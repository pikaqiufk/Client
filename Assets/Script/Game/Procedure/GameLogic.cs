#region using

using System;
using System.Collections;
using ClientService;
using DataTable;
using EventSystem;
using FastShadowReceiver;
using GameUI;
using ScorpionNetLib;
using Shared;
using Thinksquirrel.Utilities;
using ToLuaEx;
using UnityEngine;

#endregion

public class GameLogic : MonoBehaviour
{
    #region 成员

    //单例(注意他的生命周期只在游戏场景)
    public static GameLogic Instance;

    //主摄像机
    public Camera MainCamera = null;

    [HideInInspector] public Scene Scene;

    [HideInInspector] public SceneEffectManager SceneEffect;

    [HideInInspector] public GuideTrigger GuideTrigger;

    [HideInInspector]
    public int MultiTouch { get; set; }

    [HideInInspector]
    public float MultiDistance { get; set; }

    [NonSerialized] public float[] MultiPos = {0, 0};

    private BattleSkillRootFrame SkillBar;

    //ShadowReceiver Mask，保存一下，用以获得地面高度的射线
    private static int ShadowReceiverLayerMask = -1;
    public static bool IsFristLogin;

    //是否是新创建的角色进来的，是就播放CG （被CLSharp调用的不要用bool）
    public static int PlayFirstEnterGameCG;
    public bool LoadOver { get; private set; }

    public string ScenePrefab;

    private GameControl mControl;

    #endregion

    #region Mono

    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        Instance = this;
        MainCamera.gameObject.AddComponent<CameraController>();
        MainCamera.gameObject.AddComponent<CameraShake>();
        gameObject.AddComponent<InputManager>();
        //Scene = gameObject.GetComponent<Scene>();
        SceneEffect = gameObject.AddComponent<SceneEffectManager>();
        GuideTrigger = gameObject.AddComponent<GuideTrigger>();
        mControl = gameObject.AddComponent<GameControl>();
        var mTime = Game.Instance.ServerTime;

        ShadowReceiverLayerMask = LayerMask.GetMask(GAMELAYER.ShadowReceiver, GAMELAYER.Terrain);

        LoadOver = false;

        EventDispatcher.Instance.AddEventListener(DungeonCompleteEvent.EVENT_TYPE, OnDungeonComplete);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif


        if (true != GameSetting.Instance.LoadingProcessGameInit)
        {
            StartCoroutine(EnterGameCoroutine());
        }
        bool flag = GameSetting.Instance.ShowOtherPlayer;
        GameSetting.Instance.ShowOtherPlayer = flag;

        //InvokeRepeating("OnLineTickSecond", 1, 1);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

//     void OnLineTickSecond()
//     {
//         EventDispatcher.Instance.DispatchEvent(new UIEvent_PerSecond());
// 
//         if (0 == OnLineSeconds % 60)
//         {
//             EventDispatcher.Instance.DispatchEvent(new UIEvent_PerMinute());
//         }
//     }

    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (null != SkillBar)
        {
            TickSkillCd();
            SkillBar.Tick();
        }

        if (!LoadOver)
        {
            return;
        }

        if (0 == Time.frameCount%30)
        {
            EventDispatcher.Instance.DispatchEvent(new Event_UpdateOnLineReward());
        }
        if (0 == Time.frameCount%30)
        {
            CityManager.Instance.UpdatePetMissionState();
        }
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        Instance = null;
        PlayCG.Instance.Reset();
        UIManager.Instance.Destroy();
        //ObjManager.Instance.RemoveAllObj();
        ObjManager.Instance.Reset();
        ComplexObjectPool.Destroy();
        GuideManager.Instance.StopGuiding();

        EventDispatcher.Instance.RemoveEventListener(DungeonCompleteEvent.EVENT_TYPE, OnDungeonComplete);

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    #endregion

    #region 事件响应

    //响应副本结束事件，副本结束时，要播放慢镜头
    private object CompTrigger;

    private void OnDungeonComplete(IEvent ievent)
    {
        var sceneId = Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null)
        {
            return;
        }
        if (tbScene.FubenId == -1)
        {
            return;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return;
        }
        if (tbFuben.IsPlaySlow != 1)
        {
            return;
        }
        Time.timeScale = Table.GetClientConfig(701).Value.ToInt()/10000f;
        var lastTime = Table.GetClientConfig(700).Value.ToInt();
        if (CompTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(CompTrigger);
            CompTrigger = null;
        }
        CompTrigger = TimeManager.Instance.CreateTrigger(Game.Instance.ServerTime.AddMilliseconds(lastTime), () =>
        {
            TimeManager.Instance.DeleteTrigger(CompTrigger);
            CompTrigger = null;
            Time.timeScale = 1f;
        });
    }

    #endregion

    #region 逻辑方法

    public IEnumerator EnterGameCoroutine()
    {
        //加载场景Prefab
        if (!string.IsNullOrEmpty(ScenePrefab))
        {
            var ret = ResourceManager.PrepareResourceWithHolder<GameObject>(ScenePrefab, true, false);
            yield return ret.Wait();

            try
            {
                var sceneRoot = Instantiate(ret.Resource) as GameObject;
                if (null != sceneRoot)
                {
                    sceneRoot.transform.parent = transform;

                    // 优化场景特效
                    OptList<ParticleSystem>.List.Clear();
                    sceneRoot.transform.GetComponentsInChildren(true, OptList<ParticleSystem>.List);
                    foreach (var particle in OptList<ParticleSystem>.List)
                    {
                        if (!particle.CompareTag("NoPauseEffect"))
                        {
                            if (particle.gameObject.GetComponent<ParticleOptimizer>() == null)
                            {
                                particle.gameObject.AddComponent<ParticleOptimizer>();
                            }
                        }
                    }

                    Scene = sceneRoot.GetComponent<Scene>();
                    if (null != Scene)
                    {
                        Scene.InitPortal();
                    }
                    else
                    {
                        Logger.Error("cant find Scene in ScenePerfab!!!");
                    }
                }



                SoundManager.Instance.SetAreaSoundMute(SoundManager.Instance.EnableBGM);
//                 var sceneCacheKey = string.Format("{0}.unity3d", ScenePrefab);
//                 ResourceManager.Instance.RemoveFromCache(sceneCacheKey);
            }
            catch (Exception e)
            {
				Logger.Error("step 0------------------{0}\n{1}" , e.Message, e.StackTrace);
            }
        }


        if (ObjManager.Instance == null)
        {
            Logger.Log2Bugly("EnterGameCoroutine ObjManager.Instance = null ");
            yield break;
        }

        //清除ObjManager
        ObjManager.Instance.Reset();


        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.mInitBaseAttr == null)
        {
            Logger.Log2Bugly("EnterGameCoroutine PlayerDataManager.Instance = null ");
            yield break;
        }
        var data = PlayerDataManager.Instance.mInitBaseAttr;

        var attr = new InitMyPlayerData();

        //初始化造主角的数据
        try
        {
            attr.ObjId = data.CharacterId;
            attr.DataId = data.RoleId;
            attr.Name = data.Name;
            attr.Camp = data.Camp;
            attr.IsDead = data.IsDead == 1;
            attr.HpMax = data.HpMax;
            attr.HpNow = data.HpNow;
            attr.MpMax = data.MpMax;
            attr.MpNow = data.MpMow;
            attr.X = data.X;
            attr.Y = GetTerrainHeight(data.X, data.Y);
            attr.Z = data.Y;
            attr.MoveSpeed = data.MoveSpeed;
            attr.AreaState = (eAreaState) data.AreaState;
            attr.EquipModel = data.EquipsModel;
            attr.ModelId = data.ModelId;
            {
                var __list1 = data.Buff;
                var __listCount1 = __list1.Count;
                for (var __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var buff = __list1[__i1];
                    {
                        attr.Buff.Add(buff.BuffId, buff.BuffTypeId);
                    }
                }
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 1------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //造主角
        ObjMyPlayer player = null;
        try
        {
            player = ObjManager.Instance.CreateMainPlayer(attr);
            if (player == null)
            {
                Logger.Log2Bugly("EnterGameCoroutine player = null ");
                yield break;
            }
            player.AdjustHeightPosition();
        }
        catch (Exception e)
        {
			Logger.Error("step 2------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //设置buff
        try
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ClearBuffList());

            var count = data.Buff.Count;
            for (var i = 0; i < count; i++)
            {
                var buffResult = data.Buff[i];
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SyncBuffCell(buffResult));
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 3------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //预加载技能资源
        try
        {
            ObjManager.Instance.PrepareMainPlayerSkillResources();
        }
        catch (Exception e)
        {
			Logger.Error("step 4------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //给主摄像机设置跟随，设置声音
        try
        {
            if (MainCamera == null)
            {
                Logger.Log2Bugly("EnterGameCoroutine MainCamera = null ");
                yield break;
            }
            MainCamera.GetComponent<CameraController>().FollowObj = player.gameObject;
            {
//audio listener
                var audioListerner = MainCamera.gameObject.GetComponent<AudioListener>();
                if (null != audioListerner)
                {
                    DestroyObject(audioListerner);
                }
                var playerAudio = player.gameObject.GetComponent<AudioListener>();
                if (null == playerAudio)
                {
                    player.gameObject.AddComponent<AudioListener>();
                }
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 5------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //初始化UI
        Coroutine co = null;
        try
        {
            co = StartCoroutine(InitUI());
            if (null != LoadingLogic.Instance)
            {
                LoadingLogic.Instance.SetLoadingProgress(0.95f);
            }
            else
            {
                Logger.Error("LoadingLogic.Instance==null");
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 6------------------{0}\n{1}", e.Message, e.StackTrace);
        }
        if (null != co)
        {
            yield return co;
        }


        //控制模块
        try
        {
            InputManager.Instance.OnMoveDestination = mControl.MoveTo;
            InputManager.Instance.SelectTarget = mControl.SelectTarget;

            if (UIManager.Instance.MainUIFrame != null)
            {
                var main = UIManager.Instance.MainUIFrame.GetComponent<MainScreenFrame>();
                var joystick = main.GetComponentInChildren<JoyStickLogic>();
                if (joystick != null)
                {
                    joystick.OnMoveDirection = mControl.MoveDirection;
                }
                SkillBar = main.SkillBar.GetComponent<BattleSkillRootFrame>();
                if (SkillBar != null)
                {
                    SkillBar.OnClickEvent = mControl.OnClickEvent;
                }
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 7------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //UI
        try
        {
            EventDispatcher.Instance.DispatchEvent(new Enter_Scene_Event(Scene.SceneTypeId));
            EventDispatcher.Instance.DispatchEvent(new RefresSceneMap(Scene.SceneTypeId));
            UIManager.Instance.OpenDefaultFrame();

            player.CreateNameBoard();

            if (PlayerDataManager.Instance != null)
            {
//根据场景不一样，自动战斗的优先级也不一样
                PlayerDataManager.Instance.RefrehEquipPriority();
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 8------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        //向服务器请求场景参数
        yield return StartCoroutine(AskSceneExtData());

        //向服务器发送切换场景结束的包
        if (SceneManager.Instance != null)
        {
            yield return StartCoroutine(SceneManager.Instance.ChangeSceneOverCoroutine());
        }
        else
        {
            Logger.Log2Bugly("EnterGameCoroutine SceneManager.Instance = null ");
        }

        //客户端切换场景结束事件
        try
        {
            SceneEffect.OnEnterScecne();
            LoadingLogic.Instance.SetLoadingProgress(1.0f);
            SceneManager.Instance.OnLoadSceneOver();
            EventDispatcher.Instance.DispatchEvent(new LoadSceneOverEvent());
        }
        catch (Exception e)
        {
			Logger.Error("step 9------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        yield return new WaitForSeconds(0.1f);

        //播放CG
        try
        {
            Action brightnessStartWork = () =>
            {
                LoginWindow.State = LoginWindow.LoginState.InGaming;
                var bc = Game.Instance.GetComponent<BrightnessController>();
                if (bc)
                {
                    bc.ResetTimer();
                }
            };

            if (1 == PlayFirstEnterGameCG)
            {
#if UNITY_EDITOR
                var skip = true;
#else
			bool skip = true;
#endif
                if (0 == PlayerDataManager.Instance.GetRoleId() ||
                    1 == PlayerDataManager.Instance.GetRoleId() ||
                    2 == PlayerDataManager.Instance.GetRoleId())
                {
                    if (int.Parse(Table.GetClientConfig(1205).Value) == 1)
                    {
					    PlayCG.Instance.PlayCGFile("Video/HeroBorn.txt", brightnessStartWork, skip);
                        PlatformHelper.UMEvent("PlayCG", "play", "Video/HeroBorn.txt");
                    }
                }

                EventDispatcher.Instance.DispatchEvent(new FirstEnterGameEvent(true));

                PlayFirstEnterGameCG = 0;
            }
            else
            {
                brightnessStartWork();
            }
        }
        catch (Exception e)
        {
			Logger.Error("step 10------------------{0}\n{1}", e.Message, e.StackTrace);
        }

        mControl.OnLoadSceneOver();

        LoadOver = true;

        //优化loading读条速度，所以meshtree放在读条之后再加载
        if (null != Scene)
        {
            StartCoroutine(DelayLoadTerrainMeshTree(ScenePrefab));
        }

        if (!HasAdjustSetting)
        {
            HasAdjustSetting = true;
            StartCoroutine(ChangeSetting());
        }

        LuaEventManager.Instance.PushEvent("OnEnterGameOver", Scene.SceneTypeId);
    }

    private static bool HasAdjustSetting;

    private IEnumerator ChangeSetting()
    {
        yield return new WaitForSeconds(5.0f);

        mLoadCompletedTime = Time.fixedTime;
        mLoadCompletedFrame = Time.frameCount;

        yield return new WaitForSeconds(3.0f);

        var frameTime = (Time.fixedTime - mLoadCompletedTime)/(Time.frameCount - mLoadCompletedFrame);

        var willLevel = 1;
        if (frameTime < 0.04f)
        {
            // 高
            willLevel = 3;
        }
        else if (frameTime < 0.07f)
        {
            // 中
            willLevel = 2;
        }
        else
        {
            // 低
            willLevel = 1;
        }

        if (willLevel < GameSetting.Instance.GameQualityLevel)
        {
            //GameSetting.Instance.GameQualityLevel = willLevel;
            EventDispatcher.Instance.DispatchEvent(new UIEvent_QualitySetting(willLevel));
        }
    }

    public IEnumerator InitUI()
    {
        var root = GameObject.Find("UIRoot");
        if (null == root)
        {
            var prefabPath = "UI/UIRoot";
            var ret = ResourceManager.PrepareResourceWithHolder<GameObject>(prefabPath);
            yield return ret.Wait();
            root = Instantiate(ret.Resource) as GameObject;
        }

        //var rootLogic = root.AddComponent<UIRootLogic>();
        var camera = root.gameObject.transform.Find("Camera").gameObject;
        UIManager.Instance.ResetUIRoot(root, camera.GetComponent<Camera>());

        UIManager.Instance.GetController(UIConfig.MainUI).RefreshData(null);
        var co = StartCoroutine(UIManager.Instance.ShowUICoroutine(UIConfig.MainUI));
        yield return co;


        //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MianUI));
        //EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MissionTrackList));
        //InputManager.Instance.OnMoveDestination = MoveTo;
        //UIManager.Instance.JoyStick.GetComponent<JoyStickLogic>().OnMoveDirection = MoveDirection;
        //JoyStickLogic.Instance().OnMoveDirection = MoveDirection;
        //UIManager.Instance.SkillBar.GetComponent<SkillBarLogic>().OnClickEvent = OnClickEvent;

        DebugHelper.CreateDebugHelper();

        if (IsFristLogin)
        {
            //初始化推送
            EventDispatcher.Instance.DispatchEvent(new UIEvent_RefreshPush(-1, 0));

            if (PlayerDataManager.Instance.CheckCondition(40000) == 0)
            {
                UIManager.Instance.ShowUI(UIConfig.RewardFrame, new UIRewardFrameArguments
                {
                    Tab = 2
                });
                IsFristLogin = false;
            }

            //统计数据forkuaifa
            var characterId = PlayerDataManager.Instance.GetGuid().ToString();
            var characterName = PlayerDataManager.Instance.PlayerDataModel.CharacterBase.Name;
            var level = PlayerDataManager.Instance.GetLevel();
            var serverId = PlayerDataManager.Instance.ServerId.ToString();
            var serverName = PlayerDataManager.Instance.ServerName;
            var vipLevel = PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel);
            var battleName = PlayerDataManager.Instance.BattleName;
            var ts = PlayerDataManager.Instance.CharacterFoundTime - DateTime.Parse("1970-1-1");
            var time = (int)ts.TotalSeconds;
            PlatformHelper.CollectionEnterGameDataForKuaifa(characterId, characterName, level, serverId, serverName, vipLevel, battleName, time.ToString());
        }
    }

    private IEnumerator DelayLoadTerrainMeshTree(string scenePrefab)
    {
        yield return new WaitForSeconds(2);

        if (null == Scene)
        {
             yield break;
        }

        var path = string.Format("TerrainMeshTree/{0}.asset", scenePrefab.Replace("Terrain/",""));
        ResourceManager.PrepareResource<BinaryMeshTree>(path, (meshTree) =>
        {
            if (null == meshTree)
            {
                Logger.Error(string.Format("加载资源失败，检查Res/{0}文件是否存在！！", path));
            }
            else
            {
                Scene.MeshTree = meshTree;
                if (null != ObjManager.Instance.MyPlayer)
                {
                    ObjManager.Instance.MyPlayer.InitShadow(GameSetting.Instance.ShowDynamicShadow);
                }
            }
        },true, true, false,false,true);
    }
    // 问服务器要场景状态
    public IEnumerator AskSceneExtData()
    {
        var msg = NetManager.Instance.ApplySceneExdata(0);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State != MessageState.Reply)
        {
            Logger.Debug("AskSceneExtData-------msg.State != MessageState.Reply");
            yield break;
        }

        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            Logger.Debug("AskSceneExtData-------msg.ErrorCode[{0}]", msg.ErrorCode);
            yield break;
        }

        var data = msg.Response;

        var tbScene = Table.GetScene(Scene.SceneTypeId);
        if (tbScene.Type == (int)eSceneType.Fuben)
        {
            OptList<SceneAnimationTrigger>.List.Clear();
            gameObject.transform.GetComponentsInChildren(OptList<SceneAnimationTrigger>.List);
            var triggers = OptList<SceneAnimationTrigger>.List;
            if (triggers.Count <= 0)
            {
                yield break;
                ;
            }

            Table.ForeachTriggerArea(table =>
            {
                if (Scene.SceneTypeId != table.SceneId ||
                    -1 == table.OffLineTrigger ||
                    !BitFlag.GetLow(data, table.OffLineTrigger))
                {
                    return true;
                }
                {
                    var __array2 = triggers;
                    var __arrayLength2 = __array2.Count;
                    for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var trigger = __array2[__i2];
                        {
                            if (trigger.TriggerId == table.ClientAnimation)
                            {
                                trigger.RunToEnd();
                                break;
                            }
                        }
                    }
                }
                return true;
            });
        }
        else if (tbScene.Type == (int)eSceneType.Pvp)
        {
            if (data == 1)
            {
                var e = new PvpFightReadyEent();
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //获得地面高度
    public static float GetTerrainHeight(float x, float z)
    {
        var ray = new Ray(new Vector3(x, 110, z), Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 120, ShadowReceiverLayerMask))
        {
            return hit.point.y;
        }
        return 0;
    }
    
    //获得地面高度
    public static float GetTerrainHeight(Vector3 p)
    {
        return GetTerrainHeight(p.x, p.z);
    }

    //获得地面高度位置点
    public static Vector3 GetTerrainPosition(float x, float z)
    {
        return new Vector3(x, GetTerrainHeight(x, z), z);
    }

    //tick技能cd
    private static float mLastRealTime;
    private float mLoadCompletedTime;
    private int mLoadCompletedFrame;

    private void TickSkillCd()
    {
        var skillData = PlayerDataManager.Instance.PlayerDataModel.SkillData;
        var deltaTime = (Time.realtimeSinceStartup - mLastRealTime);
        //公共cd
        if (skillData.CommonCoolDown > 0)
        {
            skillData.CommonCoolDown -= deltaTime;
            if (skillData.CommonCoolDown <= 0)
            {
                skillData.CommonCoolDown = 0;
            }
        }
        //技能cd
        var count = skillData.AllSkills.Count;
        for (var i = 0; i < count; i++)
        {
            var skill = skillData.AllSkills[i];
            if (skill.CoolDownTime > 0)
            {
                skill.CoolDownTime -= deltaTime;
                if (skill.CoolDownTime <= 0)
                {
                    skill.CoolDownTime = 0;
                    if (skill.ChargeLayer != skill.ChargeLayerTotal)
                    {
                        skill.ChargeLayer++;
                        if (skill.ChargeLayer != skill.ChargeLayerTotal)
                        {
                            skill.CoolDownTime = skill.CoolDownTimeTotal;
                        }
                    }
                }
            }
        }

        mLastRealTime = Time.realtimeSinceStartup;
    }

    #endregion
}