#region using

using System.ComponentModel;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

public class ForceController : IControllerBase
{
    public ForceController()
    {
        CleanUp();
    }

    public ForceDataModel DataModel;
    public Coroutine RefreshCoroutine;
    public bool HasSend { get; set; }

    public void CleanUp()
    {
        DataModel = new ForceDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as ForceArguments;
        if (args == null)
        {
            return;
        }
        HasSend = false;
        var oldValue = args.OldValue;
        var newValue = args.NewValue;
        DataModel.BeginValue = oldValue;
        DataModel.EndValue = newValue;
        if (State == FrameState.Open)
        {
            HasSend = true;
            var e = new FightValueChange(DataModel.BeginValue, DataModel.EndValue);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
        //DataModel.ChangeValue = 0;
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
        if (HasSend == false)
        {
            var e = new FightValueChange(DataModel.BeginValue, DataModel.EndValue);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public FrameState State { get; set; }
}