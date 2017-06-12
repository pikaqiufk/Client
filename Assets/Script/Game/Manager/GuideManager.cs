#region using

using System.Collections.Generic;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

public class GuideManager : Singleton<GuideManager>
{
    private const string Guilde = "Guide_";
    private int CurrentFrame = -1;
    public List<StepByStepRecord> DataList = new List<StepByStepRecord>();
    public int mCurrentGuideId = -1;
    public int mStep;

    public bool Enable
    {
        get { return GameSetting.Instance.EnableGuide; }
    }

    public void FinishGuide()
    {
        //PlayerPrefs.SetInt(Guilde + mCurrentGuideTypeId.ToString(), 1);
        StopGuiding();
    }

    public StepByStepRecord GetCurrentGuideData()
    {
        if (!IsGuiding())
        {
            return null;
        }
        if (mStep >= 0 && mStep < DataList.Count)
        {
            return DataList[mStep];
        }
        return null;
    }

    //获得当前引导的步骤id
    public int GetCurrentGuidingStepId()
    {
        var data = GetCurrentGuideData();
        if (null != data)
        {
            return data.Id;
        }
        return -1;
    }

    //是否在引导中
    public bool IsGuiding()
    {
        return -1 != mCurrentGuideId;
    }

    //进行下一步引导
    public void NextStep()
    {
        if (!Enable)
        {
            return;
        }

        if (!IsGuiding())
        {
            return;
        }
        if (CurrentFrame == Time.frameCount)
        {
            return;
        }
        CurrentFrame = Time.frameCount;
        mStep++;

        if (mStep >= DataList.Count)
        {
            FinishGuide();
            return;
        }

        EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateGuideEvent(DataList[0].Id));

        if (DataList.Count > mStep)
        {
            PlatformHelper.UMEvent("Guide", "Enter", DataList[mStep].Id);
        }

#if UNITY_EDITOR
        Logger.Debug("NextStep[{0}]", DataList[mStep].Id);
#endif
    }

    //跳过引导
    public void Skip()
    {
        if (IsGuiding())
        {
            FinishGuide();
            return;
        }
        StopGuiding();
    }

    //开启某个引导
    public bool StartGuide(int id)
    {
        if (!Enable)
        {
            return false;
        }

        CurrentFrame = Time.frameCount;
// 		if (IsGuiding())
// 		{
// 			return false;
// 		}

// 		int val = PlayerPrefs.GetInt(Guilde + id.ToString(), 0);
// 		if (0 != val)
// 		{
// 			return false;
// 		}

        var table = Table.GetStepByStep(id);
        if (null == table)
        {
            Logger.Error("null == table StartGuide({0})", id);
            return false;
        }

        mCurrentGuideId = id;
        mStep = 0;
        DataList.Clear();
        DataList.Add(Table.GetStepByStep(mCurrentGuideId));

        PlatformHelper.UMEvent("Guide", "Enter", mCurrentGuideId);

        while (-1 != DataList[DataList.Count - 1].NextIndexID)
        {
            var tableNext = Table.GetStepByStep(DataList[DataList.Count - 1].NextIndexID);
            if (null == tableNext)
            {
                Logger.Debug("null==table");
                break;
            }
            if (DataList.Contains(tableNext))
            {
                Logger.Debug("DataList.Contains(tableNext)", tableNext.Id);
                break;
            }
            DataList.Add(tableNext);

            //safe
            if (DataList.Count > 20)
            {
                Logger.Debug("DataList>20 [{0}]", mCurrentGuideId);
                break;
            }
        }
        UIManager.Instance.ShowUI(UIConfig.NewbieGuide);
        EventDispatcher.Instance.DispatchEvent(new UIEvent_UpdateGuideEvent(DataList[0].Id));
#if UNITY_EDITOR
        Logger.Debug("StartGuide[{0}]", mCurrentGuideId);
#endif
        return true;
    }

    //停止引导
    public void StopGuiding()
    {
#if UNITY_EDITOR
        Logger.Debug("StopGuiding[{0}]", mCurrentGuideId);
#endif

        mCurrentGuideId = -1;
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.NewbieGuide));
    }
}