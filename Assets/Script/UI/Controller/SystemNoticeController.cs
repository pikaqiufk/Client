#region using

using System.Collections.Generic;
using System.ComponentModel;
using EventSystem;

#endregion

public class SystemNoticeController : IControllerBase
{
    public SystemNoticeController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(SystemNoticeOperate.EVENT_TYPE, OnSystemNoticeOperate);
    }

    public bool mIsSend;
    public readonly List<string> SystemLableStringList = new List<string>();

    public void AddContentInfo(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return;
        }
        if (SystemLableStringList.Count > GameUtils.SystemNoticeRollingScreenLimit)
        {
            SystemLableStringList.RemoveRange(0, 5);
        }

        SystemLableStringList.Add(content);
    }

    private void ClearContentInfo()
    {
        SystemLableStringList.Clear();
        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SystemNoticeFrame));
    }

    public void NotifyShowNotice()
    {
        if (SystemLableStringList.Count == 0)
        {
            var e = new SystemNoticeNotify("");
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var e = new SystemNoticeNotify(SystemLableStringList[0]);
            EventDispatcher.Instance.DispatchEvent(e);
            SystemLableStringList.RemoveAt(0);
        }
    }

    public void OnSystemNoticeOperate(IEvent ievent)
    {
        var e = ievent as SystemNoticeOperate;
        switch (e.Type)
        {
            case 0:
                NotifyShowNotice();
                break;
            case 1:
                ClearContentInfo();
                break;
        }
    }

    public void CleanUp()
    {
        SystemLableStringList.Clear();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return null;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
        if (SystemLableStringList.Count > 0)
        {
            var e = new Show_UI_Event(UIConfig.SystemNoticeFrame, new SystemNoticeArguments());
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
        if (mIsSend == false)
        {
            NotifyShowNotice();
            mIsSend = true;
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as SystemNoticeArguments;
        if (arg == null)
        {
            return;
        }
        AddContentInfo(arg.NoticeInfo);

        if (State == FrameState.Open)
        {
            mIsSend = true;
        }
        else
        {
            mIsSend = false;
        }
    }

    public FrameState State { get; set; }
}