#region using

using System.ComponentModel;
using EventSystem;

#endregion

public class DepotController : IControllerBase
{
    public DepotController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_BagChange.EVENT_TYPE, OnRefrehEquipBagItemStatus);
    }

    public IControllerBase BackPack;
    public FrameState mState;

    public void OnRefrehEquipBagItemStatus(IEvent ievent)
    {
        var e = ievent as UIEvent_BagChange;
        if (State == FrameState.Open)
        {
            if (e.HasType(eBagType.Equip))
            {
                PlayerDataManager.Instance.RefreshEquipBagStatus();
            }
            if (e.HasType(eBagType.Depot))
            {
                PlayerDataManager.Instance.RefreshEquipBagStatus(eBagType.Depot);
            }
        }
    }

    public void CleanUp()
    {
        BackPack = UIManager.Instance.GetController(UIConfig.BackPackUI);
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
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Depot});
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        BackPack.RefreshData(data);
        PlayerDataManager.Instance.RefreshEquipBagStatus(eBagType.Depot);
        BackPack.CallFromOtherClass("SetPackType", new object[] {BackPackController.BackPackType.Depot});
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