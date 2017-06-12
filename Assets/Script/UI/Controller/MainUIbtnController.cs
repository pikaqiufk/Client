#region using

using System;
using System.ComponentModel;
using EventSystem;

#endregion

public class MainUIbtnController : IControllerBase
{
    public MainUIbtnController()
    {
        EventDispatcher.Instance.AddEventListener(UIEvent_MainUIbtn_BtnTranfer.EVENT_TYPE, UIanimation);
        // EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillSelect.EVENT_TYPE, OnClicSkillItem);
    }

    public void UIanimation(IEvent ievent)
    {
        var e = ievent as UIEvent_MainUIbtn_BtnTranfer;
    }

    public void CleanUp()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        throw new NotImplementedException();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
        throw new NotImplementedException();
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