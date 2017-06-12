#region using

using System;
using System.Collections;
using DataTable;
using EventSystem;
using ToLuaEx;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class LoadingLogic : MonoBehaviour
{
    public static LoadingLogic Instance;
    private AsyncOperation ChangeSceneAsync;
    public GameObject DestroyObj;
    public float LoadingDelay = 0.5f;
    private float mLoadingProgressReal;
    private float mLoadingProgressShow;

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        Instance = this;

        var res = ResourceManager.PrepareResourceSync<GameObject>("UI/LoadingRoot");
        var go = Instantiate(res) as GameObject;
        go.name = "UIRoot";
        DontDestroyOnLoad(go);
        DestroyObj = go.GetComponentInChildren<LoadingWindow>().gameObject;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public float GetLoadingProgress()
    {
        return mLoadingProgressShow;
    }

    private IEnumerator AfterLoad(string sceneName)
    {

        while (!ResourceManager.Instance.LoadCommonFinish)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (GameSetting.Instance.LoadingProcessGameInit)
        {
            if (null != GameLogic.Instance)
            {
                yield return ResourceManager.Instance.StartCoroutine(GameLogic.Instance.EnterGameCoroutine());
            }

            try
            {
                if (null != DestroyObj)
                {
                    Destroy(DestroyObj);
                }
                if (null != gameObject)
                {
                    Destroy(gameObject);
                }

                Resources.UnloadUnusedAssets();
                GC.Collect();
                LuaManager.Instance.Lua.Collect();
                EventDispatcher.Instance.DispatchEvent(new UIEvent_RefleshNameBoard());
            }
            catch (Exception e)
            {
                Logger.Error("LoadSceneImpl------------\n" + e.Message);
            }
        }
    }

    // Update is called once per frame
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

    public void SetLoadingProgress(float progress)
    {
        mLoadingProgressReal = progress;
        if (Math.Abs(progress - 1.0f) < 0.001)
        {
            mLoadingProgressShow = 1.0f;
        }
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif
        mLoadingProgressReal = 0.5f;
        mLoadingProgressShow = 0;

        var tbscene = Table.GetScene(SceneManager.Instance.CurrentSceneTypeId);
        var sceneName = tbscene.ResName;

        ResourceManager.PrepareScene(Resource.GetScenePath(sceneName), www =>
        {
            if (!ResourceManager.Instance.UseAssetBundle || www.error == null)
            {
                ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl(sceneName, www,null, AfterLoad));
            }
        });

        if (true == GameSetting.Instance.LoadingProcessGameInit)
        {
            DontDestroyOnLoad(gameObject);
        }
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

        if (mLoadingProgressShow < mLoadingProgressReal)
        {
            mLoadingProgressShow += Random.Range(0.0555f, 0.0666f);
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}