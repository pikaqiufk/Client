using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;


public class WingChargeController : IControllerBase
{
    public WingChargeDataModel DataModel;
    public float duration;
    public WingChargeController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpData);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, OnFlagInitData);
        EventDispatcher.Instance.AddEventListener(WingChargeCloseBtnClick_Event.EVENT_TYPE, WingChargeCloseBtnClick);
        EventDispatcher.Instance.AddEventListener(WingChargeItemClick_Event.EVENT_TYPE, WingChargeItemBtnClick);
        EventDispatcher.Instance.AddEventListener(WingChargeBuyClick_Event.EVENT_TYPE, WingChargebuyBtnClick);
        EventDispatcher.Instance.AddEventListener(ExData64InitEvent.EVENT_TYPE, OnExDataInitData);
        
    }
    public FrameState State { get; set; }
    public void CleanUp()
    {
        DataModel = new WingChargeDataModel();
    }
    public void WingChargeCloseBtnClick(IEvent ievent)
    {
        var e = new Close_UI_Event(UIConfig.WingChargeFrame);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void WingChargeItemBtnClick(IEvent ievent)
    {
        var e = ievent as WingChargeItemClick_Event;
        if (e != null)
        {
            if (e.index == 1)
            {
                var data = new BagItemDataModel();
                data.ItemId = DataModel.WingChargeItems[0].ItemId;
                data.Count = DataModel.WingChargeItems[0].Count;
                GameUtils.ShowItemDataTip(data, eEquipBtnShow.Share);
            }
            else if(e.index == 2)
            {
                var data = new BagItemDataModel();
                data.ItemId = DataModel.WingChargeItems[1].ItemId;
                data.Count = DataModel.WingChargeItems[1].Count;
                GameUtils.ShowItemDataTip(data, eEquipBtnShow.Share);
            }
        }  
    }

    public void OnExDataInitData(IEvent ievent)
    {
        RefreshMainUI();
    }

    private void RefreshMainUI()
    {
        var temp = DateTime.FromBinary(PlayerDataManager.Instance.GetExData64((int) Exdata64TimeType.ServerStartTime));
        var time = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0);
        var delta = Game.Instance.ServerTime - time;
        if (delta.Days >= 2)
        {
            DataModel.IsShowWingCharge = 0;
        }
        else
        {
            var tbGift = Table.GetGift(4000);
            if (!PlayerDataManager.Instance.GetFlag(tbGift.Flag) && PlayerDataManager.Instance.GetFlag(41)) // 没领取过 且完成了获得翅膀的任务
            {
                DataModel.IsShowWingCharge = 1;
            }
            else
            {
                DataModel.IsShowWingCharge = 0;
            }
        }
    }

    public void WingChargebuyBtnClick(IEvent ievent)
    {
        var msgStr = string.Format(GameUtils.GetDictionaryText(100001148), DataModel.Diamond);
        var diamond = PlayerDataManager.Instance.GetRes((int)eResourcesType.DiamondRes);
        if (diamond < DataModel.Diamond)
        {
            var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
            EventDispatcher.Instance.DispatchEvent(e);

            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
            return;
        }

        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, msgStr, "", () =>
        {
            NetManager.Instance.StartCoroutine(ApplyBuyChagreItem());
        });
    }
    private IEnumerator ApplyBuyChagreItem()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.BuyWingCharge(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply && msg.ErrorCode == (int)ErrorCodes.OK)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(431));

                if (DataModel != null && DataModel.WingChargeItems != null && DataModel.WingChargeItems.Count > 1)
                {
                    var tempDic = new Dictionary<int, int>();
                    tempDic.Add(DataModel.WingChargeItems[0].ItemId, DataModel.WingChargeItems[0].Count);
                    tempDic.Add(DataModel.WingChargeItems[1].ItemId, DataModel.WingChargeItems[1].Count);
                    var e = new ShowItemsArguments
                    {
                        Items = tempDic
                    };
                    EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ShowItemsFrame, e));
                }
            }
            else
            {
                GameUtils.ShowNetErrorHint(msg.ErrorCode);
            }
        }
    }

    public void OnFlagInitData(IEvent ievent)
    {
        var tbGift = Table.GetGift(4000);
        if (tbGift == null)
        {
            return;
        }
        var ev = new FlagUpdateEvent(tbGift.Flag, true);
        OnFlagUpData(ev);
    }

    public void OnFlagUpData(IEvent ievent)
    {
        var tbGift = Table.GetGift(4000);
        if (tbGift == null)
        {
            return;
        }
        var e = ievent as FlagUpdateEvent;
        if (e.Index != tbGift.Flag)
        {
            return;
        }
        
        if (!PlayerDataManager.Instance.GetFlag(tbGift.Flag)) // 没领取过
        {
            DataModel.BtnState = 1;
        }
        else
        {
            DataModel.BtnState = 0;
        }
        RefreshMainUI();
    }

    public void RefreshData(UIInitArguments data)
    {
        var tbGift = Table.GetGift(4000);
        if (tbGift != null)
        {
            DataModel.Diamond = tbGift.Param[7];

            var temp1 = new BagItemDataModel();
            temp1.ItemId = tbGift.Param[0];
            temp1.Count = tbGift.Param[1];

            var temp2 = new BagItemDataModel();
            temp2.ItemId = tbGift.Param[2];
            temp2.Count = tbGift.Param[3];

            DataModel.WingChargeItems[0] = temp1;
            DataModel.WingChargeItems[1] = temp2;

            if (!PlayerDataManager.Instance.GetFlag(tbGift.Flag))// 领取过了
            {
                DataModel.BtnState = 1;
            }
            else
            {
                DataModel.BtnState = 0;
            }

            DataModel.ItemNameList[0] = Table.GetItemBase(temp1.ItemId).Name;
            DataModel.ItemNameList[1] = Table.GetItemBase(temp2.ItemId).Name;

            RefreshMainUI();
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
        duration += Time.deltaTime;
        if (duration > 1.0f)
        {
            RefreshMainUI();
            var formateStr = GameUtils.GetDictionaryText(41001);
            var temp = DateTime.FromBinary(PlayerDataManager.Instance.GetExData64((int)Exdata64TimeType.ServerStartTime));
            var time = new DateTime(temp.Year, temp.Month, temp.Day, 0, 0, 0).AddDays(2);
            var stopTime = time;
            var timeStr = GameUtils.GetTimeDiffString(stopTime);
            DataModel.TimeStr = string.Format(formateStr, "", timeStr);
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
        RefreshMainUI();
    }
}
