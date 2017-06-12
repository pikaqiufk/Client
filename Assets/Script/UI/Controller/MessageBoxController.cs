#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using EventSystem;

#endregion

public class MessageBoxController : IControllerBase
{
    public MessageBoxController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(MessageBoxClick.EVENT_TYPE, OnMessageBoxClick);
    }

    public MessageBoxDataModel DataModel;
    private readonly Queue<object[]> Datas = new Queue<object[]>();
    private bool KeepOpen;

    public void OnMessageBoxClick(IEvent ievent)
    {
        var e = ievent as MessageBoxClick;
        if (e.Type == 0)
        {
            if (DataModel.CacelAction != null)
            {
                DataModel.CacelAction();
            }
        }
        else if (e.Type == 1)
        {
            if (DataModel.OkAction != null)
            {
                DataModel.OkAction();
            }
        }
    }

    public void RefrehMessge(MessageBoxType boxType,
                             string info,
                             string title = "",
                             Action okAction = null,
                             Action cancelAction = null,
                             bool isSystemInfo = false)
    {
        DataModel.BoxType = (int) boxType;
        DataModel.Info = info;

        if (string.IsNullOrEmpty(title))
        {
            title = GameUtils.GetDictionaryText(200099);
        }
        DataModel.Title = title;
        DataModel.OkAction = okAction;
        DataModel.CacelAction = cancelAction;
        if (isSystemInfo)
        {
            DataModel.Depth = 100000;
        }
        else
        {
            DataModel.Depth = 10000;
        }
    }

    public void CleanUp()
    {
        DataModel = new MessageBoxDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
        if (KeepOpen)
        {
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MessageBox));
        }
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "RefrehMessge")
        {
            var refreshMessage = true;
            var keepOpen = (bool) param[6];
            if (keepOpen)
            {
                if (KeepOpen)
                {
                    Datas.Enqueue(param);
                    refreshMessage = false;
                }
                else
                {
                    KeepOpen = true;
                }
            }
            else if (KeepOpen)
            {
                return null;
            }

            if (refreshMessage)
            {
                RefrehMessge((MessageBoxType) param[0], param[1] as string, param[2] as string, param[3] as Action,
                    param[4] as Action, (bool) param[5]);
            }
        }

        return null;
    }

    public void OnShow()
    {
    }

    public void Close()
    {
        if (KeepOpen)
        {
            if (Datas.Count > 0)
            {
                var param = Datas.Dequeue();
                RefrehMessge((MessageBoxType) param[0], param[1] as string, param[2] as string, param[3] as Action,
                    param[4] as Action, (bool) param[5]);
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MessageBox));
            }
            else
            {
                KeepOpen = false;
            }
        }
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

    public FrameState State { get; set; }
}