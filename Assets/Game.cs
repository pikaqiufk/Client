#region using

using System;
using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using ClientService;
using DataContract;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class Game : MonoBehaviour
{
    public static Game Instance;
    public GameObject DelayShowObj;
    private List<IManager> mMgrList;
    public RechargeActivityData RechargeActivityData;
    public bool ServerInfoCached;
    public DateTime LoginTime { set; private get; }

    public List<IManager> MgrList
    {
        get { return mMgrList; }
        private set { }
    }

    public int OnLineSeconds
    {
        get { return (int) (ServerTime - LoginTime).TotalSeconds; }
    }

    public DateTime ServerTime
    {
        get { return DateTime.Now - ServerTimeDiff; }
    }

    public TimeSpan ServerTimeDiff { set; get; }
    public TimeSpan ServerZoneDiff { set; get; }

    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif


#if !UNITY_EDITOR
        StartCoroutine(DelayShowScreen());
        PlatformHelper.PlayLogoMovie();
#endif
        TargetBindingProperty.Register();

        // var channel = PlatformHelper.GetChannelString();
        // Logger.Debug("PlatformHelper.GetChannelString = " + channel);
        GameSetting.Channel = GameUtils.GetChannelString();

        PlatformHelper.Initialize();
        if (null != Instance)
        {
            Logger.Fatal("ERROR!!!!!!!!!!!!!!!!!!! Game has been created");
            return;
        }
        Instance = this;

        var Fps = PlayerPrefs.GetInt(GameSetting.LowFpsKey, 30);
        Application.targetFrameRate = Fps;
#if UNITY_EDITOR
        Application.targetFrameRate = 80;
#endif
        //注册所有解析表中表达式中的函数
        ExpressionHelper.RegisterAllFunction();
        PlayCG.Instance.Init();

        mMgrList = new List<IManager>
        {
            ObjManager.Instance,
            SceneManager.Instance,
            PlayerDataManager.Instance,
            ResourceManager.Instance,
            EffectManager.Instance,
            UIManager.Instance,
            SoundManager.Instance
        };

        DontDestroyOnLoad(gameObject);


#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    /// <summary>
    ///     更新完成后，重新加载资源前调用
    /// </summary>
    public void BeforeStartLoading()
    {
        CleanupAllGameData();
    }

    public void ChangeSceneToLogin()
    {
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ServerListUI));
        GameUtils.ExitLogin(null);
        CleanupAllGameData(false);

        const string sceneName = "Login";
        ResourceManager.PrepareScene(Resource.GetScenePath(sceneName), www =>
        {
            LoginWindow.State = LoginWindow.LoginState.BeforeLogin;
            ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl(sceneName, www));
        });

//         if (LoginWindow.instance )
//         {
//             const string sceneName = "Login";
//             ResourceManager.PrepareScene(Resource.GetScenePath(sceneName), (www) =>
//             {
//                 LoginWindow.State = LoginWindow.LoginState.BeforeLogin;
//                 ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl(sceneName, www));
//             });    
//         }
//         else
//         {
//             LoginWindow.State = LoginWindow.LoginState.BeforeLogin;
//             LoginWindow.instance.Init();    
//         }
    }

    public void ChangeSceneToLoginAndAutoLogin(Action afterChange)
    {
        CleanupAllGameData();
        const string sceneName = "Login";
        ResourceManager.PrepareScene(Resource.GetScenePath(sceneName), www =>
        {
            LoginWindow.State = LoginWindow.LoginState.ThirdLogin;
            LoginWindow.ThirdLoginAction = afterChange;
            ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl(sceneName, www));
        });
    }

    private void CleanupAllGameData(bool stopNet = true)
    {
        try
        {
            ServerInfoCached = false;
            PlatformHelper.CloseToolBar();
            //正在loading过程中,如果被踢下线,删除掉全局的uiroot
            if (null != LoadingLogic.Instance)
            {
                var loadingRoot = GameObject.Find("LoadingObject");
                if (null != loadingRoot)
                {
                    Destroy(loadingRoot);
                }
                var uiroot = GameObject.Find("UIRoot");
                if (null != uiroot)
                {
                    Destroy(uiroot);
                }
            }

            var bc = Instance.GetComponent<BrightnessController>();
            if (bc)
            {
                bc.OnTouchOrMouseRelease();
            }

            NetManager.Instance.SyncCenter.Clear();
            EventDispatcher.Instance.RemoveAllEventListeners();
            CleanUpManagers();
            NetManager.Instance.StopAllCoroutines();
            ConditionTrigger.Instance.Init();
            TimeManager.Instance.CleanUp();
            PlayerAttr.Instance.CleanUp();
            if (stopNet)
            {
                NetManager.Instance.Stop();
            }
        }
        catch (Exception e)
        {
            Logger.Log2Bugly("----CleanupAllGameData throw exception:{0}", e);
            throw;
        }
    }

    public void CleanUpManagers()
    {
        {
            var __list3 = Instance.MgrList;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var manager = __list3[__i3];
                {
                    manager.Reset();
                }
            }
        }
    }

    //异步播放片头logo会显示一帧游戏画面,所以先隐藏一秒之后再显示
    private IEnumerator DelayShowScreen()
    {
        if (null == DelayShowObj)
        {
            yield break;
        }

        DelayShowObj.SetActive(false);
        yield return new WaitForSeconds(1);
        DelayShowObj.SetActive(true);
    }

    public void EnterStartup()
    {
       //PlatformHelper.UserLogout();
        CleanupAllGameData();
        ResourceManager.Instance.UnloadCommonBundle();
        ResourceManager.Instance.ClearCache(true);
        var game = GameObject.Find("Game");
        DestroyImmediate(game);
        Application.LoadLevel("Startup");
    }

    private IEnumerator ExitSelectCharacterCoroutine(Action callback = null)
    {
        var characterId = PlayerDataManager.Instance.CharacterGuid;

        if (characterId == 0ul)
        {
            if (null != callback)
            {
                callback();
            }
            yield break;
        }

        var msg = NetManager.Instance.ExitSelectCharacter(characterId);
        UIManager.Instance.ShowBlockLayer();
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                ObjManager.Instance.Reset();
                PlayerDataManager.Instance.CharacterLists = msg.Response.Info;
                PlayerDataManager.Instance.SelectedRoleIndex = msg.Response.SelectId;
                PlayerDataManager.Instance.CharacterGuid = 0ul;
                //umeng登出
                PlatformHelper.ProfileSignOff();
                if (null != callback)
                {
                    callback();
                }
            }
            else
            {
                UIManager.Instance.RemoveBlockLayer();
                Logger.Error(".....ExitSelectCharacter.......{0}.", msg.ErrorCode);
                //UIManager.Instance.ShowMessage(MessageBoxType.Ok, "ExitSelectCharacter error:"+ msg.ErrorCode);
                ExitToLogin();
            }
        }
        else
        {
            UIManager.Instance.RemoveBlockLayer();
            Logger.Error(".....ExitSelectCharacter.......time out!");
            ExitToLogin();
            //UIManager.Instance.ShowMessage(MessageBoxType.Ok, "ExitSelectCharacter time out!");
        }
    }

    //所有退回登陆界面总入口
    public void ExitToLogin()
    {
        if (GameUtils.IsOurChannel())
        {
            ChangeSceneToLogin();
        }
        else
        {
            PlatformHelper.UserLogout();
        }
    }

    public void ExitToSelectRole()
    {
        Action action = () =>
        {
            CleanupAllGameData(false);
            ResourceManager.PrepareScene(Resource.GetScenePath("SelectCharacter"), www =>
            {
                ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl("SelectCharacter", www, () =>
                {
                    LoginWindow.State = LoginWindow.LoginState.LoginSuccess;
                    var serverName = string.Empty;
                    var controller = UIManager.Instance.GetController(UIConfig.ServerListUI);
                    if (null != controller)
                    {
                        var datamodel = controller.GetDataModel("") as ServerListDataModel;
                        if (null != datamodel)
                        {
                            serverName = datamodel.SelectedServer.ServerName;
                        }
                    }

                    
                }));
            });
        };
        NetManager.Instance.StartCoroutine(ExitSelectCharacterCoroutine(action));
    }

    public void ExitToServerList(bool bQuick = false)
    {
        Action action = () =>
        {
            const string sceneName = "Login";
            ResourceManager.PrepareScene(Resource.GetScenePath(sceneName), www =>
            {
                LoginWindow.State = LoginWindow.LoginState.LoginSuccess;
                ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl(sceneName, www, () =>
                {
                    if (ServerInfoCached && bQuick)
                    {
                        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ServerListUI));
                    }
                    else
                    {
                        CleanupAllGameData(false);
                        NetManager.Instance.StartCoroutine(LoginWindow.LoginSuccess());
                    }
                }));
            });
        };

        NetManager.Instance.StartCoroutine(ExitSelectCharacterCoroutine(action));
    }

    private void LateUpdate()
    {
#if !UNITY_EDITOR
        try
        {
#endif
//         if (ChangeSceneList.Count > 0 && ChangeScenestate == eChangeSceneState.Finished)
//         {
//             Action act = ChangeSceneList.Dequeue();
//             act();
//         }

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void LocalNotificationTest()
    {
//         PlatformHelper.ClearAllLocalNotification();
//         PlatformHelper.SetLocalNotification("key1","key1 测试本地通知,这条消息应该在启动游戏一分钟后弹出!",60);
//         PlatformHelper.SetLocalNotification("key2", "key2 测试本地通知,这条消息你看不到!", 120);
//         PlatformHelper.DeleteLocalNotificationWithKey("key2");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
#if !UNITY_EDITOR
        if (!pauseStatus)
        {
            if (!NetManager.Instance.Connected)
            {
                //正在重新连接...
                this.StartCoroutine(NetManager.Instance.OnServerLost());
            }
        }
        else
        {
            if (NetManager.Instance.Connected && ObjManager.Instance.MyPlayer != null)
            {
                ObjManager.Instance.MyPlayer.StopMove();
            }
        }
#endif
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        {
            var __list2 = mMgrList;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var mgr = __list2[__i2];
                {
                    if (mgr != null)
                    {
                        mgr.Destroy();
                    }
                }
            }
        }
        mMgrList.Clear();

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public static void SetGameQuality()
    {
        var defaultQualityLevel = 3;
#if UNITY_IOS
        iPhoneGeneration iOSGen = iPhone.generation;
        if (iOSGen == iPhoneGeneration.iPhone3GS) 
        {
            defaultQualityLevel = 1;
        } 
        else if (iOSGen == iPhoneGeneration.iPhone4)
        {
            defaultQualityLevel = 1;
        } 
        else if (iOSGen == iPhoneGeneration.iPhone4S)
        {
           defaultQualityLevel = 1;
        }
        else if (iOSGen == iPhoneGeneration.iPhone5 || iOSGen == iPhoneGeneration.iPhone5C) 
        {
           defaultQualityLevel = 2;
        } else if (iOSGen == iPhoneGeneration.iPhone5S || iOSGen == iPhoneGeneration.iPhone6 ||
                   iOSGen == iPhoneGeneration.iPhone6Plus)
        {
            defaultQualityLevel = 3;
        }
        else if (iOSGen == iPhoneGeneration.iPad1Gen) 
        {
            defaultQualityLevel = 1;
        } 
        else if (iOSGen == iPhoneGeneration.iPad2Gen) 
        {
            defaultQualityLevel = 1;
        } 
        else if (iOSGen == iPhoneGeneration.iPad3Gen) 
        {
           defaultQualityLevel = 2;
        } 
        else if (iOSGen == iPhoneGeneration.iPodTouch3Gen) 
        {
            defaultQualityLevel = 2;
        } 
        else if (iOSGen == iPhoneGeneration.iPodTouch4Gen)
        {
           defaultQualityLevel = 2;
        }
        else
        {
            string device = SystemInfo.deviceModel;

            if (device == "iPhone")
            {
                defaultQualityLevel = 3;
            }
            else if (device == "iPad")
            {
                defaultQualityLevel = 3;
            }
            else if (device == "iPod")
            {
                defaultQualityLevel = 1;
            }
        }
#endif

#if UNITY_ANDROID
        var shaderLevel = SystemInfo.graphicsShaderLevel;
        var fillrate = SystemInfo.graphicsPixelFillrate;
        var vram = SystemInfo.graphicsMemorySize;
        var cpus = SystemInfo.processorCount;
        if (fillrate < 0)
        {
            if (shaderLevel < 10)
            {
                fillrate = 1000;
            }
            else if (shaderLevel < 20)
            {
                fillrate = 1300;
            }
            else if (shaderLevel < 30)
            {
                fillrate = 2000;
            }
            else
            {
                fillrate = 3000;
            }
            if (cpus >= 6)
            {
                fillrate *= 3;
            }
            else if (cpus >= 3)
            {
                fillrate *= 2;
            }
            if (vram >= 512)
            {
                fillrate *= 2;
            }
            else if (vram <= 100)
            {
                fillrate /= 2;
            }
        }

        var resx = Screen.width;
        var resy = Screen.height;
        var fillneed = (float) ((resx*resy + 400*300)*(30.0/1000000.0));
        float[] levelmult = {0, 30f, 60f, 120f};
        var level = 0;
        while (level < 3 && fillrate > fillneed*levelmult[level])
        {
            ++level;
        }


        Logger.Debug(String.Format("-------------------------shaderLevel {0},cpus {1}, vram {2}", shaderLevel, cpus,
            vram));
        Logger.Debug(String.Format("-------------------------{0}x{1} need {2} has {3} = {4} level", resx, resy, fillneed,
            fillrate, level));
        defaultQualityLevel = level;
#endif

        if (null == GameSetting.Instance)
        {
            Logger.Error("GameSetting.Instance = null , call SetGameQuality too early!");
            return;
        }
        GameSetting.Instance.GameQualityLevel = PlayerPrefs.GetInt(GameSetting.GameQuilatyKey, defaultQualityLevel);
        GameSetting.Instance.GameResolutionLevel = PlayerPrefs.GetInt(GameSetting.GameResolutionKey, 3);
    }

    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        Logger.LogLevel = GameSetting.Instance.LogLevel;

        SetGameQuality();


#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlatformHelper.Exit();
        }

        {
            var __list1 = mMgrList;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var mgr = __list1[__i1];
                {
                    try
                    {
                        Profiler.BeginSample(mgr.GetType().ToString());
                        mgr.Tick(Time.deltaTime);
                        Profiler.EndSample();
                    }
                    catch
                    {
                        // some mgr failed.
                    }
                }
            }
        }

        Profiler.BeginSample("TimeManager");
        TimeManager.Instance.Updata();
        Profiler.EndSample();


#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }
}