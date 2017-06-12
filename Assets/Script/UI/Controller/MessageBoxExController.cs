using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;


public class MessageBoxExController : IControllerBase
{

    public MessageBoxExController()
    {
       // EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillSelect.EVENT_TYPE, OnClicSkillItem);
	   CleanUp();
       EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
    }

    public MessageBoxExDataModel DataModel;

    private float deltaTime = 0;

    public FrameState State { get; set; }

    public void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        if (e == null || e.Key != (int) eExdataDefine.e630) return;

        var logic = GameLogic.Instance;
        if (logic == null)
        {
            return;
        }
        var scene = logic.Scene;
        if (scene == null)
        {
            return;
        }
        var tbScene = Table.GetScene(scene.SceneTypeId);
        if (tbScene == null)
        {
            return;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben == null)
        {
            return;
        }

        if (e.Value <= 0 && State == FrameState.Close)
        {
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MessageBoxEx));
        }

        if (e.Value > 0 && State == FrameState.Open)
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBoxEx));
        }
        
    }

    public void CleanUp()
    {
        DataModel = new MessageBoxExDataModel();
    }
    public void RefreshData(UIInitArguments data)
    {
        DataModel.TimeDown = 11;
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
        if (State != FrameState.Open)
        {
            return;
        }

        deltaTime += Time.deltaTime;
        if (deltaTime > 1.0f)
        {
            deltaTime = 0;

            var logic = GameLogic.Instance;
            if (logic == null)
            {
                return;
            }
            var scene = logic.Scene;
            if (scene == null)
            {
                return;
            }
            var tbScene = Table.GetScene(scene.SceneTypeId);
            if (tbScene == null)
            {
                return;
            }
            var tbFuben = Table.GetFuben(tbScene.FubenId);
            if (tbFuben == null)
            {
                return;
            }

            DataModel.TimeDown -= 1;
            if (DataModel.TimeDown < 0.001f)
            {
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBoxEx));
            }
        }
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
}
