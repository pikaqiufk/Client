#region using

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class FirstChargeController : IControllerBase
{
    public FirstChargeController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(FirstChargeBtnClick_Event.EVENT_TYPE, FirstChargeBtnClick);
        EventDispatcher.Instance.AddEventListener(FirstChargeCloseBtnClick_Event.EVENT_TYPE, FirstChargeCloseBtnClick);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, ApplyFirstChargeItem);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpData);
        EventDispatcher.Instance.AddEventListener(FlagInitEvent.EVENT_TYPE, OnFlagInitData);
        EventDispatcher.Instance.AddEventListener(FirstChargeToggleSuccess_Event.EVENT_TYPE, ToggleClick);
        
    }

    public FirstChargeDataModel DataModel;
    private int isCharged1 = 0;
    private int isCharged2 = 0;
    private int isCharged3 = 0;

    public void ApplyFirstChargeItem(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ApplyFirstChargeItemCoroutine());
    }

    private IEnumerator ApplyFirstChargeItemCoroutine()
    {
        var msg = NetManager.Instance.ApplyFirstChargeItem(0);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply && msg.ErrorCode == (int) ErrorCodes.OK)
        {
            PlayerDataManager.Instance.FirstChargeData = msg.Response;
            RefreshMainUIFirstChagreBtn();
            InitBtnState();
            RefreshItems();
        }
    }

    private IEnumerator ApplyGetFirstChagreItem(int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyGetFirstChargeItem(index);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply && msg.ErrorCode == (int) ErrorCodes.OK)
            {
                if (msg.Response == 0) // 失敗
                {
                }
                else if (msg.Response == 1) // 成功
                {
                    var tempDic = new Dictionary<int, int>();
                    if (index == 0 && DataModel != null)
                    {
                        foreach (var data in DataModel.RechargeItemsTab1)
                        {
                            tempDic.Add(data.ItemId, data.Count);
                        }
                    }
                    else if (index == 1 && DataModel != null)
                    {
                        foreach (var data in DataModel.RechargeItemsTab2)
                        {
                            tempDic.Add(data.ItemId, data.Count);
                        }
                    }
                    else if (index == 2 && DataModel != null)
                    {
                        foreach (var data in DataModel.RechargeItemsTab3)
                        {
                            tempDic.Add(data.ItemId, data.Count);
                        }
                    }
                    
                    var e = new ShowItemsArguments
                    {
                        Items = tempDic
                    };
                    EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ShowItemsFrame, e));

                    var iEvent = new FirstChargeGetItemSuccess_Event();
                    EventDispatcher.Instance.DispatchEvent(iEvent);
                }
            }
            else
            {
                GameUtils.ShowNetErrorHint(msg.ErrorCode);
            }
        }
    }

    public void ToggleClick(IEvent ievent)
    {
        var e = ievent as FirstChargeToggleSuccess_Event;
        if (e != null)
        {
            DataModel.ToggleSelect = e.index;
        }
        else
        {
            DataModel.ToggleSelect = 0;
        }

        InitBtnState();
        if (State == FrameState.Open)
        {
            if (PlayerDataManager.Instance.FirstChargeData != null && PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList != null &&
                PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList.Count > DataModel.ToggleSelect)
            {
                var temp = "";
                if (PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath.Count > 0)
                {
                    if (0 == PlayerDataManager.Instance.GetRoleId())
                    {
                        temp = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath[0];
                        var ex = new FirstChargeModelDisplay_Event(temp);
                        EventDispatcher.Instance.DispatchEvent(ex);
                    }
                    if (1 == PlayerDataManager.Instance.GetRoleId())
                    {
                        temp = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath[1];
                        var ex = new FirstChargeModelDisplay_Event(temp);
                        EventDispatcher.Instance.DispatchEvent(ex);
                    }
                    if (2 == PlayerDataManager.Instance.GetRoleId())
                    {
                        temp = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath[2];
                        var ex = new FirstChargeModelDisplay_Event(temp);
                        EventDispatcher.Instance.DispatchEvent(ex);
                    }
                }
            }
        }
    }

    public void FirstChargeBtnClick(IEvent ievent)
    {
        //FirstChargeControlBtnClick_Event e = ievent as FirstChargeControlBtnClick_Event;
        var flag = -1;
        if (PlayerDataManager.Instance.FirstChargeData != null && PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList.Count > DataModel.ToggleSelect)
        {
            flag = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].flag;
        }
        else
        {
            return;
        }

        if (DataModel.ToggleSelect == 0) 
        {
            if (isCharged1 == 0) // 未充值过
            {
                var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                EventDispatcher.Instance.DispatchEvent(e);
            }
            else if (isCharged1 == 1) // 充值过了
            {
                if (PlayerDataManager.Instance.GetFlag(flag))// 领取过了
                {
                    var e = new ShowUIHintBoard(100001115);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else// 没领取过
                {
                    // 领取逻辑
                    NetManager.Instance.StartCoroutine(ApplyGetFirstChagreItem(0));
                }
            }
        }
        else if (DataModel.ToggleSelect == 1) 
        {
            if (isCharged2 == 0) // 未充值过
            {
                var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                EventDispatcher.Instance.DispatchEvent(e);
            }
            else if (isCharged2 == 1) // 充值过了
            {
                if (PlayerDataManager.Instance.GetFlag(flag))// 领取过了
                {
                    var e = new ShowUIHintBoard(100001115);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else// 没领取过
                {
                    // 领取逻辑
                    NetManager.Instance.StartCoroutine(ApplyGetFirstChagreItem(1));
                }
            }
        }
        else if (DataModel.ToggleSelect == 2)
        {
            if (isCharged3 == 0) // 未充值过
            {
                var e = new Show_UI_Event(UIConfig.RechargeFrame, new RechargeFrameArguments { Tab = 0 });
                EventDispatcher.Instance.DispatchEvent(e);
            }
            else if (isCharged3 == 1) // 充值过了
            {
                if (PlayerDataManager.Instance.GetFlag(flag))// 领取过了
                {
                    var e = new ShowUIHintBoard(100001115);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else// 没领取过
                {
                    // 领取逻辑
                    NetManager.Instance.StartCoroutine(ApplyGetFirstChagreItem(2));
                }
            }
        }
    }

    public void FirstChargeCloseBtnClick(IEvent ievent)
    {
        var e = new Close_UI_Event(UIConfig.FirstChargeFrame);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;

        if (e.Key == (int) eExdataDefine.e561)
        {
            InitBtnState();
        }
    }

    public void OnFlagInitData(IEvent ievent)
    {
        RefreshMainUIFirstChagreBtn();
    }

    public void OnFlagUpData(IEvent ievent)
    {
        if (State == FrameState.Open)
        {
            InitBtnState();
        }
        RefreshMainUIFirstChagreBtn();
    }

    private void InitBtnState()
    {
        isCharged1 = 0;
        DataModel.WanChengState1 = 0;
        DataModel.btnState1 = 1;

        isCharged2 = 0;
        DataModel.WanChengState2 = 0;
        DataModel.btnState2 = 1;

        isCharged3 = 0;
        DataModel.WanChengState3 = 0;
        DataModel.btnState3 = 1;

        if (PlayerDataManager.Instance.FirstChargeData == null || PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList == null)
        {
            return;
        }
        var index = 0;
        foreach (var data in PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList)
        {
            index++;
            if (index == 1)
            {
                if (PlayerDataManager.Instance.GetFlag(data.flag)) // 领取过
                {
                    DataModel.WanChengState1 = 1;
                    DataModel.btnState1 = 0;
                }

                if (PlayerDataManager.Instance.GetExData((int)eExdataDefine.e561) >= data.diamond)
                {
                    isCharged1 = 1;
                }
                else
                {
                    isCharged1 = 0;
                }
            }
            if (index == 2)
            {
                if (PlayerDataManager.Instance.GetFlag(data.flag)) // 领取过
                {
                    DataModel.WanChengState2 = 1;
                    DataModel.btnState2 = 0; 
                }

                if (PlayerDataManager.Instance.GetExData((int)eExdataDefine.e561) >= data.diamond)
                {
                    isCharged2 = 1;
                }
                else
                {
                    isCharged2 = 0;
                }
            }
            if (index == 3)
            {
                if (PlayerDataManager.Instance.GetFlag(data.flag)) // 领取过
                {
                    DataModel.WanChengState3 = 1;
                    DataModel.btnState3 = 0;
                }

                if (PlayerDataManager.Instance.GetExData((int)eExdataDefine.e561) >= data.diamond)
                {
                    isCharged3 = 1;
                }
                else
                {
                    isCharged3 = 0;
                }
            }
        }
    }

    private void RefreshItems()
    {
        var chargeDataList = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList;
        int index = 0;
        foreach (var dataList in chargeDataList)
        {
            index++;
            if (index == 1)
            {
                DataModel.RechargeItemsTab1.Clear();
                foreach (var item in dataList.items)
                {
                    var temp = new ItemIdDataModel();
                    temp.ItemId = item.itemid;
                    temp.Count = item.count;
                    DataModel.RechargeItemsTab1.Add(temp);
                }
                isCharged1 = dataList.isCharged;
            }

            if (index == 2)
            {
                DataModel.RechargeItemsTab2.Clear();
                foreach (var item in dataList.items)
                {
                    var temp = new ItemIdDataModel();
                    temp.ItemId = item.itemid;
                    temp.Count = item.count;
                    DataModel.RechargeItemsTab2.Add(temp);
                }
                isCharged2 = dataList.isCharged;
            }

            if (index == 3)
            {
                DataModel.RechargeItemsTab3.Clear();
                foreach (var item in dataList.items)
                {
                    var temp = new ItemIdDataModel();
                    temp.ItemId = item.itemid;
                    temp.Count = item.count;
                    DataModel.RechargeItemsTab3.Add(temp);
                }
                isCharged3 = dataList.isCharged;
            }
        }
    }

    public void RefreshMainUIFirstChagreBtn()
    {
        // 主界面首冲图标是否显示
        if (PlayerDataManager.Instance.FirstChargeData == null || PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList == null)
        {
            return;
        }

        var isAllGet = true;

        foreach (var data in PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList)
        {
            if (!PlayerDataManager.Instance.GetFlag(data.flag))
            {
                isAllGet = false;
            }
        }

        if (isAllGet)
        {
            DataModel.isShowMainUIFirstChagre = 0;
        }
        else
        {
            DataModel.isShowMainUIFirstChagre = 1;
        }
    }

    private void UpdateWithOutItems()
    {
        // 剑士100001111 魔法师100001112 弓箭手100001113
        var str = "";
        if (0 == PlayerDataManager.Instance.GetRoleId())
        {
            str = GameUtils.GetDictionaryText(100001111);
        }
        else if (1 == PlayerDataManager.Instance.GetRoleId())
        {
            str = GameUtils.GetDictionaryText(100001112);
        }
        else if (2 == PlayerDataManager.Instance.GetRoleId())
        {
            str = GameUtils.GetDictionaryText(100001113);
        }
        DataModel.MainStr = str;

        InitBtnState();
    }

    public void CleanUp()
    {
        DataModel = new FirstChargeDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        UpdateWithOutItems();
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
        if (PlayerDataManager.Instance.FirstChargeData != null && PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList != null &&
                PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList.Count > DataModel.ToggleSelect)
        {
            var temp = "";
            if (PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath.Count > 0)
            {
                if (0 == PlayerDataManager.Instance.GetRoleId())
                {
                    temp = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath[0];
                    var ex = new FirstChargeModelDisplay_Event(temp);
                    EventDispatcher.Instance.DispatchEvent(ex);
                }
                if (1 == PlayerDataManager.Instance.GetRoleId())
                {
                    temp = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath[1];
                    var ex = new FirstChargeModelDisplay_Event(temp);
                    EventDispatcher.Instance.DispatchEvent(ex);
                }
                if (2 == PlayerDataManager.Instance.GetRoleId())
                {
                    temp = PlayerDataManager.Instance.FirstChargeData.FirstChagreItemList[DataModel.ToggleSelect].modelPath[2];
                    var ex = new FirstChargeModelDisplay_Event(temp);
                    EventDispatcher.Instance.DispatchEvent(ex);
                }
            }
        }
    }

    public FrameState State { get; set; }
}