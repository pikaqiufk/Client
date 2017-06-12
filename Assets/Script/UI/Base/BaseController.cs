
using UnityEngine; 
using System;
using System.Collections;
using System.ComponentModel;

public enum FrameState
{
    Loading,
    Close,
    Open,
    Bad,
}

public interface IControllerBase
{
    FrameState State { get; set; }
    void CleanUp();
    void OnShow();
    void Close();
    void Tick();
    void RefreshData(UIInitArguments data);
    INotifyPropertyChanged GetDataModel(string name);
    void OnChangeScene(int sceneId);
    object CallFromOtherClass(string name, object[] param);
}
