#region using

using System;
using System.Collections.Generic;
using DataTable;
using EventSystem;
using UnityEngine;
using Random = UnityEngine.Random;

#endregion

public class LoadingWindow : MonoBehaviour
{
    public UITexture Background;
    public UISlider bar;
    private string mBkgImagePath = "";
    private int mCurrentIdx;
    private DateTime mNextRandTime = DateTime.Now;
    public float NexTipInterval = 1;
    private readonly List<int> TipDicList = new List<int>();
    public UILabel TipLabel;
    public GameObject DownLoadingTip;

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (!string.IsNullOrEmpty(mBkgImagePath))
        {
            var key = mBkgImagePath.Substring(0, mBkgImagePath.LastIndexOf('.')) + ".unity3d";
            ResourceManager.Instance.RemoveFromCache(key);
        }

        if (null != Background && null != Background.mainTexture)
        {
            var tex = Background.mainTexture;
            Background.mainTexture = null;
            Resources.UnloadAsset(tex);
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif


        var picCount = int.Parse(Table.GetClientConfig(122).Value);
        var index = Random.Range(0, picCount);
        mBkgImagePath = "Texture/Background/loading" + index + ".jpg";
        var tex = ResourceManager.PrepareResourceSync<Texture>(mBkgImagePath);
        if (tex == null)
        {
            mBkgImagePath = "Texture/Background/login" + ".jpg";
            tex = ResourceManager.PrepareResourceSync<Texture>(mBkgImagePath);
        }
        Background.mainTexture = tex;
        TipDicList.Clear();

        var level = PlayerDataManager.Instance.IsLevelInited ? PlayerDataManager.Instance.GetLevel() : 0;
        var flagInited = PlayerDataManager.Instance.FlagInited;

        //小提示根据条件
        Table.ForeachLoadingTest(table =>
        {
            if (table.MaxLevel > 0)
            {
                if (0 == level)
                {
//0级说明还没有等级数据还没有初始化玩家还没有数据
                    return true;
                }
                if (level > table.MaxLevel || level < table.MinLevel)
                {
                    return true;
                }
            }

            if (table.MinLevel > 0)
            {
                if (0 == level)
                {
//0级说明还没有等级数据还没有初始化玩家还没有数据
                    return true;
                }
                if (level < table.MinLevel)
                {
                    return true;
                }
            }

            if (-1 != table.FlagTrue)
            {
                if (!flagInited)
                {
                    return true;
                }
                if (!PlayerDataManager.Instance.GetFlag(table.FlagTrue))
                {
                    return true;
                }
            }

            if (-1 != table.FlagFalse)
            {
                if (!flagInited)
                {
                    return true;
                }
                if (PlayerDataManager.Instance.GetFlag(table.FlagFalse))
                {
                    return true;
                }
            }

            TipDicList.Add(table.DictIndex);

            return true;
        });

        //random
        var TipDicListCount0 = TipDicList.Count;
        for (var i = 0; i < TipDicListCount0; i++)
        {
            var temp = TipDicList[i];
            var ranIdx = Random.Range(0, TipDicList.Count);
            TipDicList[i] = TipDicList[ranIdx];
            TipDicList[ranIdx] = temp;
        }
        UpdateTip();
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
        if (null != LoadingLogic.Instance)
        {
            bar.value = LoadingLogic.Instance.GetLoadingProgress();
        }

        if (DateTime.Now >= mNextRandTime)
        {
            UpdateTip();
        }
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void UpdateTip()
    {
        TipLabel.text = GameUtils.GetDictionaryText(TipDicList[mCurrentIdx]);
        mCurrentIdx++;
        mCurrentIdx = mCurrentIdx%TipDicList.Count;
        mNextRandTime = DateTime.Now.AddSeconds(NexTipInterval);
    }

    private void ShowDownloadingTip(IEvent ievent)
    {
        if (!DownLoadingTip.activeSelf)
        {
            DownLoadingTip.SetActive(true);
        }
    }

    void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (DownLoadingTip.activeSelf)
        {
            DownLoadingTip.SetActive(false);
        }
        EventDispatcher.Instance.AddEventListener(UIEvent_ShowDownloadingSceneTipEvent.EVENT_TYPE, ShowDownloadingTip);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(UIEvent_ShowDownloadingSceneTipEvent.EVENT_TYPE, ShowDownloadingTip);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
}