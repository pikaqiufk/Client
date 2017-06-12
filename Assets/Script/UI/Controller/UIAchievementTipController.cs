#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class UIAchievementTipController : IControllerBase
{
    public static int MaxAchievement = 10;

    public UIAchievementTipController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(Event_AchievementTip.EVENT_TYPE, OnAchievementTipEvent);
        EventDispatcher.Instance.AddEventListener(Event_NextAchievementTip.EVENT_TYPE, OnNextAchievementTipEvent);
    }

    public List<int> AchievementTipQueue = new List<int>();
    public FrameState mState;
    public AchievementTipDataModel DataModel { get; set; }

    private void OnAchievementTipEvent(IEvent ievent)
    {
        var a1 = Table_Tamplet.Convert_Int(Table.GetClientConfig(562).Value);
        if (!PlayerDataManager.Instance.GetFlag(a1))
        {
            return;
        }

        if (GuideManager.Instance.IsGuiding())
        {
            return;
        }

        var e = ievent as Event_AchievementTip;
        if (null == e)
        {
            return;
        }

        var id = e.Id;
        AchievementTipQueue.Add(id);

        if (1 == AchievementTipQueue.Count)
        {
            DataModel.Id = id;
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.AchievementTip));
        }
        else if (AchievementTipQueue.Count > MaxAchievement)
        {
            AchievementTipQueue.RemoveAt(0);
        }
    }

    private void OnNextAchievementTipEvent(IEvent ievent)
    {
        if (AchievementTipQueue.Count > 0)
        {
            DataModel.Id = AchievementTipQueue[0];
            AchievementTipQueue.RemoveAt(0);
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.AchievementTip));
        }
    }

    public void CleanUp()
    {
        DataModel = new AchievementTipDataModel();
        AchievementTipQueue.Clear();
    }

    public void OnChangeScene(int sceneId)
    {
        AchievementTipQueue.Clear();
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
    }

    public void OnShow()
    {
    }

    public void Close()
    {
        AchievementTipQueue.Clear();
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}