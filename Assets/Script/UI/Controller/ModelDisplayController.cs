#region using

using System;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class ModelDisplayController : IControllerBase
{
    public ModelDisplayController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ModelDisplay_Equip_Event.EVENT_TYPE, OnEvent_Equip);
        EventDispatcher.Instance.AddEventListener(ShowItemHint.EVENT_TYPE, OnEvent_ShowItemHint);
    }

    public ModelDisplayDataModel DataModel;
    private int jumpToWingUI;

    public void OnEvent_ShowItemHint(IEvent ievent)
    {
        var e = ievent as ShowItemHint;
        if (e == null)
            return;

        if (State == FrameState.Open || State == FrameState.Loading)
        {
            if (e.ItemId == DataModel.Item.ItemId)
            {
                return;
            }
        }
        
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.GainItemHintUI,
            new GainItemHintArguments { ItemId = e.ItemId, BagIndex = e.BagIndex }));
    }

    public void OnEvent_Equip(IEvent ievent)
    {
        var e = ievent as ModelDisplay_Equip_Event;
        if (e != null)
        {
            var e1 = new Close_UI_Event(UIConfig.ModelDisplayFrame);
            EventDispatcher.Instance.DispatchEvent(e1);
   
            Equip(e.ItemId);
        }
    }

    public void CleanUp()
    {
        DataModel = new ModelDisplayDataModel();
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void OnChangeScene(int sceneId)
    {
        
    }
    public void OnShow()
    {
        var e = new ModelDisplay_ShowModel_Event(DataModel.Item.ItemId, 0);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void RefreshData(UIInitArguments data)
    {
        if (data.Args.Count <= 0)
            return;

        var showItemId = data.Args[0];
        DataModel.Item.ItemId = showItemId;
        DataModel.Item.Count = 1;
        DataModel.Effect = 1;
        if (data.Args.Count >= 2)
        {
            jumpToWingUI = 0;
            DataModel.ButtonName = GameUtils.GetDictionaryText(210000);
        }
        else
        {
            jumpToWingUI = 1;
            DataModel.ButtonName = GameUtils.GetDictionaryText(100000908);
        }

        var tbEquip = Table.GetItemBase(showItemId);
        if (tbEquip != null)
        {
            DataModel.ItemName = tbEquip.Name;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }

    private void Equip(int equipId)
    {
        if (equipId <= 0)
        {
            return;
        }

        var tbItemBase = Table.GetItemBase(equipId);
        var itemType = GameUtils.GetItemInfoType(tbItemBase.Type);
        if (itemType == eItemInfoType.Equip)
        {
            var bagItem = PlayerDataManager.Instance.GetBagItemByItemId((int)eBagType.Equip, equipId);
            if (bagItem != null)
            {
                var bagId = -1;
                var bagIndex = -1;
                EquipCompareController.GetBeReplacedEquip(bagItem, ref bagId, ref bagIndex);

                var equipType = PlayerDataManager.Instance.ChangeBagIdToEquipType(bagId);
                if (equipType != -1)
                {
                    var equipItem = PlayerDataManager.Instance.GetEquipData((eEquipType)equipType);
                    if (equipItem != null)
                    {
                        if (bagItem.FightValue < equipItem.FightValue)
                        {
                            return;
                        }
                    }                        
                }

                EquipCompareController.ReplaceEquip(bagItem);
            }
        }
        else if (itemType == eItemInfoType.Wing)
        {
            if (jumpToWingUI == 1)
            {
                //GameUtils.GotoUiTab(38, 1);
            }
        }
    }
}
