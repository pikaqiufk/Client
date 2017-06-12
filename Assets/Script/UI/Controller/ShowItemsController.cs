#region using

using System;
using System.ComponentModel;
using ClientDataModel;
using EventSystem;
using Shared;

#endregion

public class ShowItemsController : IControllerBase
{
    public float LastTime = 5.0f;
    private object timer;    //定时器handler，用来自动关闭提示界面

    public ShowItemsController()
    {
        CleanUp();
    }

    public ShowItemsDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new ShowItemsDataModel();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void Close()
    {
        if (timer != null)
        {
            TimeManager.Instance.DeleteTrigger(timer);
            timer = null;
        }
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
        
    }

    public void OnShow()
    {
    }

    public void RefreshData(UIInitArguments ievent)
    {
        var e = ievent as ShowItemsArguments;
        if (e == null)
        {
            return;
        }

        ResetTimer();

        DataModel.Items.Clear();
        var enumerator = e.Items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var itemData = new BagItemDataModel();
            itemData.ItemId = enumerator.Current.Key;
            itemData.Count = enumerator.Current.Value;

            DataModel.Items.Add(itemData);
        }

        var ex = new ShowItemsFrameEffectEvent();
        EventDispatcher.Instance.DispatchEvent(ex);

        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
    }

    private void ResetTimer()
    {
        var time = Game.Instance.ServerTime.AddSeconds(LastTime);
        if (timer != null)
        {
            TimeManager.Instance.ChangeTime(timer, time);
        }
        else
        {
            timer = TimeManager.Instance.CreateTrigger(time, CloseUI);
        }
    }

    private void CloseUI()
    {
        if (timer != null)
        {
            TimeManager.Instance.DeleteTrigger(timer);
            timer = null;
        }

        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ShowItemsFrame));
    }

    public FrameState State { get; set; }
}
