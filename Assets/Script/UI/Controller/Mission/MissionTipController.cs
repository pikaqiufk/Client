#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class MissionTipController : IControllerBase
{
    public MissionTipController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_MissionTipEvent.EVENT_TYPE, OnMissionTipEvent);
    }

    public MissionTipDataModel DataModel = new MissionTipDataModel();
    private int mMissionId = -1;

    private void OnMissionTipEvent(IEvent ievent)
    {
        var e = ievent as UIEvent_MissionTipEvent;
        var table = Table.GetMissionBase(mMissionId);
        
        if (null != table && -1 != table.UIGuideId)
        {
            GameUtils.GotoUiTab(table.UIGuideId, table.UIGuideTab);
        }
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as MissionTipArguments;
        mMissionId = arg.Id;
        var table = Table.GetMissionBase(mMissionId);
        if (null != table)
        {
            var idx = table.RewardItem.Length - 1;
            DataModel.Item.ItemId = table.RewardItem[idx];
            DataModel.Item.Count = table.RewardItemCount[idx];
            DataModel.Tip = GameUtils.GetDictionaryText(table.RewardDictId);
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void CleanUp()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
    }

    public FrameState State { get; set; }
}