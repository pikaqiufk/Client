#region using

using System.ComponentModel;
using ClientDataModel;
using EventSystem;

#endregion

public class AttriFrameController : IControllerBase
{
    public AttriFrameController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(AttriFrameOperate.EVENT_TYPE, OnAttriFrameOperate);
    }

    public FrameState mState;

    public void OnAttriFrameOperate(IEvent ievent)
    {
        var e = ievent as AttriFrameOperate;
        switch (e.Type)
        {
            case 1:
            {
                OnClickWing();
            }
                break;
            case 2:
            {
                OnClickElf();
            }
                break;
        }
    }

    public void OnClickElf()
    {
        var data = UIManager.Instance.GetController(UIConfig.ElfUI).GetDataModel("") as ElfDataModel;
        if (data != null && data.FightElf.ItemId != -1)
        {
            var e = new Show_UI_Event(UIConfig.ElfInfoUI,
                new ElfInfoArguments {DataModel = data.FightElf, ShowButton = true});
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    public void OnClickWing()
    {
        if (PlayerDataManager.Instance.GetWingId() == -1)
        {
            return;
        }
        var e = new Show_UI_Event(UIConfig.WingUI);
        EventDispatcher.Instance.DispatchEvent(e);
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

    public void RefreshData(UIInitArguments data)
    {
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.IsDuableChange)
        {
//有变化才请求
            PlayerDataManager.Instance.RefrshEquipDurable();
            //NetManager.Instance.StartCoroutine(ApplyEquipDurableCoroutine());
        }
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}