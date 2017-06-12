#region using

using System;
using System.Collections;
using System.IO;
using DataTable;
using ToLuaEx;
using UnityEngine;

#endregion

public class StartupLogic : MonoBehaviour
{
    public static StartupLogic Instance;
    public GameObject BigUpdateTip;
    public UILabel CountLabel = null;
    public UILabel ErrorLabel;
    public GameObject ErrorTip;
    public LoadResourceHelper helper = new LoadResourceHelper();
    private float mCacheSize;
    private float mExLoadingPercent;
    public UILabel ProgressLabel = null;
    public UILabel StatusLabel;
    public UILabel TotalSize;
    private UpdateHelper updateHelper;
    public GameObject UpdatePanel = null;
    public UISlider UpdateProgress = null;
    private string UpdateUrl;
    public GameObject WaitingTip;
    public GameObject WifiTip;

    public UILabel UpdateTip;
    public UILabel LateUpdateTip;
    public UILabel VersionLabel;

    public GameObject ResetTip;
    public UILabel ResetLabel;
    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        Instance = this;
        initVersion();
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    private void initVersion()
    {
        var version = "-1";
        string versionConfig;
        var gameVersionPath = Path.Combine(Application.streamingAssetsPath, "Game.ver");
        //先读包内版本号
        if (GameUtils.GetStringFromPackage(gameVersionPath, out versionConfig))
        {
            var config = versionConfig.Split(',');
            var resourceVersion = config[4];

            //读取之前更新过的版本号
            var downLoadVersionPath = Path.Combine(UpdateHelper.DownloadRoot, "Resources.ver"); 
            if (File.Exists(downLoadVersionPath))
            {
                int localVersion;
                if (GameUtils.GetIntFromFile(downLoadVersionPath, out localVersion))
                {
                    resourceVersion = localVersion.ToString();
                }
            }

            version = string.Format("version {0}.{1}", config[3], resourceVersion);
        }

        VersionLabel.text = version;
    }

    private void BegainLoading()
    {
        {
            var i = 0;
            var __enumerator1 = (Table.GetTableNames()).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var tableName = __enumerator1.Current;
                {
                    helper.AddLoadInfo("Table/" + tableName + ".txt", tableName + ".txt", true, true, (++i)%40 != 0);
                }
            }
        }

        //editor在不用bundle下不用加载common
        if (ResourceManager.Instance.UseAssetBundle)
        {
            {
                var __array1 = GameSetting.Instance.CommonBundleList;
                var __arrayLength1 = __array1.Length;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var str = __array1[__i1];
                    {
                        helper.AddLoadInfo(str, false, false);
                    }
                }
            }
        }

        {
            var __list2 = GameSetting.Instance.ResourceList;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var str = __list2[__i2];
                {
                    helper.AddLoadInfo(str);
                }
            }
        }

        //速度慢等待提示
        if (PlayerPrefs.GetInt(GameSetting.ShowWaitingTipKey, 0) == 0)
        {
            if (null != WaitingTip)
            {
                WaitingTip.SetActive(true);
            }
        }


        //表太多了,先把缓存扩大,加载表格完成后再把缓存改回来

        mCacheSize = GameSetting.Instance.ResourceCacheMaxSize;
        GameSetting.Instance.ResourceCacheMaxSize = 500f;

        helper.BeginLoad(() =>
        {
            PlayerPrefs.SetInt(GameSetting.ShowWaitingTipKey, 1);
            StartCoroutine(OnLoadOver());
        });
    }

    public void ErrorTipButton()
    {
        updateHelper = null;
        ErrorTip.gameObject.SetActive(false);
        Start();
    }

    public float GetLoadingPercent()
    {
#if !UNITY_EDITOR
try
{
#endif

        return helper.GetLoadingPrecent()*0.5f + mExLoadingPercent*0.5f;
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
    return 0;
}
#endif
    }

    private void OnCheckResVersion(UpdateHelper.CheckVersionResult result, string message)
    {
        if (result == UpdateHelper.CheckVersionResult.NONEEDUPDATE)
        {
            BegainLoading();
        }
        else if (result == UpdateHelper.CheckVersionResult.NEEDUPDATE)
        {
            StartCoroutine(updateHelper.UpdateMd5List(OnUpdateMd5List));
        }
        else if (result == UpdateHelper.CheckVersionResult.GAMENEEDUPDATE)
        {
            UpdateUrl = message;
            BigUpdateTip.SetActive(true);
        }
        else if (UpdateHelper.CheckVersionResult.ERROR == result)
        {
            ShowErrorTip(message);
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        Instance = null;

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void OnGotoUpdate()
    {
        Application.OpenURL(UpdateUrl);
    }

    private IEnumerator OnLoadOver()
    {
        {
            var __list3 = Game.Instance.MgrList;
            var __listCount3 = __list3.Count;
            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var mgr = __list3[__i3];
                {
                    var co = StartCoroutine(mgr.Init());
                    yield return co;
                }
            }
        }

        //加载Lua bundle
        yield return StartCoroutine(LuaManager.Instance.LoadLuaRes());

        try
        {
            //初始化Lua
            LuaManager.Instance.InitLua();
            LuaComponent.s_luaState = LuaManager.Instance.Lua;
            LuaManager.Instance.Lua.DoFile("Main.lua");
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }


        try
        {
            Table.Init();
            mExLoadingPercent = 0.6f;
            GameSetting.Instance.ResourceCacheMaxSize = mCacheSize;
            ExpressionHelper.initializeStaticString();
            Game.Instance.BeforeStartLoading();
            PlayerDataManager.InitExtDataEvent();
            Dijkstra.Init();
            GameUtils.Init();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }

        yield return new WaitForEndOfFrame();

        const string sceneName = "Login";
        ResourceManager.PrepareScene(Resource.GetScenePath(sceneName), www =>
        {
            mExLoadingPercent = 0.8f;
            LoginWindow.State = LoginWindow.LoginState.BeforeLogin;
            ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl(sceneName, www,
                () =>
                {
                    mExLoadingPercent = 1.0f;
                    ResourceManager.Instance.LoadLateCommonBundle();
                }));
        });
    }

    private void OnUpdateFinish(bool success, string message = "")
    {
        if (success)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ShowRestTip(message);
            }
            else
            {
                UpdatePanel.SetActive(false);
                BegainLoading();
            }
        }
        else
        {
            ShowErrorTip(message);
        }
    }

    private void OnUpdateMd5List(UpdateHelper.UpdateResult result, string message)
    {
        if (result == UpdateHelper.UpdateResult.GetMd5ListFail)
        {
            ShowErrorTip(message);
            return;
        }

        if (/*UpdateHelper.CheckWiFi()*/true)
        {
            WifiTip.SetActive(false);
            UpdatePanel.SetActive(true);
            StartCoroutine(updateHelper.StartUpdateAll(OnUpdateFinish));
        }
        else
        {
            TotalSize.text = message;
            WifiTip.SetActive(true);
            switch (result)
            {
                case UpdateHelper.UpdateResult.GetMd5ListSuccess:
                    LateUpdateTip.gameObject.SetActive(false);
                    UpdateTip.gameObject.SetActive(true);
                    break;
                case UpdateHelper.UpdateResult.GetMd5ListSuccessAndLateUpdate:
                    LateUpdateTip.gameObject.SetActive(true);
                    UpdateTip.gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void ShowErrorTip(string message)
    {
        ErrorLabel.text = message;
        ErrorTip.SetActive(true);
    }

    private void ShowRestTip(string message)
    {
        ResetLabel.text = message;
        ResetTip.SetActive(true);
    }

    public void ResetTipOk()
    {
        PlatformHelper.RestartApp();
    }

    private void ShowLogo()
    {
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        updateHelper = new UpdateHelper();
        if (GameSetting.Instance.UpdateEnable)
        {
            updateHelper.CheckVersion(OnCheckResVersion, true);
        }
        else
        {
            OnUpdateFinish(true);
        }

        UpdatePanel.SetActive(false);

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        if (null != updateHelper)
        {
            UpdateProgress.value = updateHelper.UpdatePrecent;
            ProgressLabel.text = string.Format("{0}/{1}", updateHelper.DownloadedSize, updateHelper.TotalSize);
            CountLabel.text = string.Format("{0}/{1}", updateHelper.CurrentCount, updateHelper.TotalCount);
            StatusLabel.text = updateHelper.UpdateStatus;
        }

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void WifiTipCancel()
    {
        Application.Quit();
    }

    public void WifiTipOk()
    {
#if !UNITY_EDITOR
try
{
#endif

        WifiTip.SetActive(false);
        UpdatePanel.SetActive(true);
        StartCoroutine(updateHelper.StartUpdateAll(OnUpdateFinish));

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}