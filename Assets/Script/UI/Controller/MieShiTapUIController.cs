

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

public class MieShiTapUIController : IControllerBase
{
    public MieShiTapUIController()
    {
        CleanUp();
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

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {

    }

    
    public INotifyPropertyChanged GetDataModel(string name)
    {
        return null;
    }

    public FrameState State { get; set; }
}