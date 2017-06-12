#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class ShareFrameController : IControllerBase
{
    public ShareFrameController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UI_EVENT_ShareBtnShow.EVENT_TYPE, SetShareBtnVisible);
    }

    private ShareDataModel mDataModel;

    public void RefreshDate(object data, object dataEx = null)
    {
    }

    private void SetShareBtnVisible(IEvent ievent)
    {
        var e = ievent as UI_EVENT_ShareBtnShow;
        SetShareBtnVisible(e != null && e.isVisible);
    }

    public void SetShareBtnVisible(bool IsVisible)
    {
#if UNITY_IPHONE || UNITY_EDITOR
        var configTable = Table.GetClientConfig(210);
        mDataModel.IsShareButtonShow = false;
        mDataModel.IsShareButtonShow = (configTable.Value == "1") && IsVisible;
#else
        mDataModel.IsShareButtonShow = false;
#endif
    }

    public void Tick(float f)
    {
    }

    public void CleanUp()
    {
        mDataModel = new ShareDataModel();
        SetShareBtnVisible(true);
    }

    public void Close()
    {
    }

    public void Tick()
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

    public void RefreshData(UIInitArguments data)
    {
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return mDataModel;
    }

    public FrameState State { get; set; }
}