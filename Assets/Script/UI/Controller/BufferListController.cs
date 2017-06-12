#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion


public class BufferListController : IControllerBase
{
    public BufferListController()
    {
        CleanUp();
    }

    private EquipBufferListDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new EquipBufferListDataModel();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        throw new NotImplementedException(name);
    }

    public void OnShow()
    {
        EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(1));
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        if (data.Args == null)
            return;

        DataModel.BuffList.Clear();

        var enumorator = data.Args.GetEnumerator();
        while (enumorator.MoveNext())
        {
            var cell = new BuffInfoCell();
            cell.BuffId = enumorator.Current;
            cell.BuffLevel = 1;
            DataModel.BuffList.Add(cell);
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
        EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(-1));
    }


    public void OnEvent_SelectModel(IEvent ievent)
    {
    }

    public FrameState State { get; set; }
}