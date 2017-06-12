#region using

using System;
using System.ComponentModel;
using EventSystem;

#endregion

public class CharacterInfoController : IControllerBase
{
    public CharacterInfoController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnRefrehEquipBagItemStatus);
        EventDispatcher.Instance.AddEventListener(Attr_Change_Event.EVENT_TYPE, OnAttrChange);
    }

    public IControllerBase Attribute;
    public IControllerBase BackPack;
    public FrameState mState;

    private void OnAttrChange(IEvent ievent)
    {
        var e = ievent as Attr_Change_Event;
        if (e.Type == eAttributeType.Strength
            || e.Type == eAttributeType.Agility
            || e.Type == eAttributeType.Intelligence
            || e.Type == eAttributeType.Endurance)
        {
            PlayerDataManager.Instance.RefreshEquipStatus();
            PlayerDataManager.Instance.RefreshEquipBagStatus();
        }
    }

    public void OnRefrehEquipBagItemStatus(IEvent ievent)
    {
        var e = ievent as UIEvent_BagChange;
        if (e.HasType(eBagType.Equip))
        {
            if (State == FrameState.Open)
            {
                PlayerDataManager.Instance.RefreshEquipBagStatus();
            }
        }
    }

    public void CleanUp()
    {
        BackPack = UIManager.Instance.GetController(UIConfig.BackPackUI);
        Attribute = UIManager.Instance.GetController(UIConfig.AttriFrameUI);
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
        if (BackPack != null)
        {
            BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Character});
        }
    }

    public void Close()
    {
        BackPack.Close();
        Attribute.Close();
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var packArg = new BackPackArguments();
        if (data == null)
        {
            packArg.Tab = 1;
        }
        else
        {
            packArg.Tab = data.Tab;
        }
        PlayerDataManager.Instance.RefreshEquipStatus();
        BackPack.RefreshData(packArg);
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Character});
        Attribute.RefreshData(data);
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return null;
    }

    public FrameState State
    {
        get { return mState; }
        set { mState = value; }
    }
}