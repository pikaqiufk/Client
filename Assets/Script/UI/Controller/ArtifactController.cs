#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion


public class ArtifactController : IControllerBase
{
    private bool isInitDict = false;
    private Dictionary<int, List<EuqipInfo>> equipRecordDict = new Dictionary<int, List<EuqipInfo>>();
    private List<EuqipInfo> equipList;
    private int lastSelectIndex = -1;

    public ArtifactController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(ArtifactOpEvent.EVENT_TYPE, OnEvent_Operate);
        EventDispatcher.Instance.AddEventListener(ArtifactSelectEvent.EVENT_TYPE, OnEvent_SelectModel);
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnResourceChange);
    }

    private ArtifactDataModel DataModel;

    public void CleanUp()
    {
        DataModel = new ArtifactDataModel();
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
    }

    public void Tick()
    {
    }

    public void InitEquipConfig()
    {
        if (isInitDict)
        {
            return;
        }
        isInitDict = true;

        equipRecordDict.Clear();

        Table.ForeachEquipBase(record =>
        {
            if (record.ShowEquip != 1)
                return true;

            if (record.Occupation == -1)
                return true;

            List<EuqipInfo> equipList;
            if (!equipRecordDict.TryGetValue(record.Occupation, out equipList))
            {
                equipRecordDict[record.Occupation] = new List<EuqipInfo>();
                equipList = equipRecordDict[record.Occupation];
            }

            var equipInfo = new EuqipInfo();
            equipInfo.Record = record;
            equipList.Add(equipInfo);

            return true;
        });

        var enumerator = equipRecordDict.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var equipList = enumerator.Current.Value;
            if (equipList != null)
            {
                equipList.Sort((l, r) =>
                {
                    if (l.Record.Ladder > r.Record.Ladder)
                        return 1;
                    else if (l.Record.Ladder == r.Record.Ladder)
                        return 0;
                    else
                        return -1;
                });

                var enumerator1 = equipList.GetEnumerator();
                var minLadder = -1;
                while (enumerator1.MoveNext())
                {
                    if (enumerator1.Current == null)
                        continue;
                    if (enumerator1.Current.Record.Ladder == minLadder || minLadder == -1)
                    {
                        minLadder = enumerator1.Current.Record.Ladder;
                        enumerator1.Current.CanBuy = true;
                        var tbItem = Table.GetItemBase(enumerator1.Current.Record.Id);
                        if (tbItem == null || tbItem.StoreID < 0)
                        {
                            return;
                        }
                        var tbStore = Table.GetStore(tbItem.StoreID);
                        if (tbStore == null)
                        {
                            return;
                        }
                        enumerator1.Current.BuyCost = tbStore.NeedValue;
                    }
                    else
                    {
                        enumerator1.Current.CanBuy = false;
                    }
                }
            }
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        InitEquipConfig();

        var roleId = PlayerDataManager.Instance.GetRoleId();
        if (!equipRecordDict.TryGetValue(roleId, out equipList))
        {
            return;
        }

        DataModel.Career = roleId;

        if (DataModel.WeaponItems.Count == 0)
        {
            DataModel.WeaponItems.Clear();
            DataModel.Models.Clear();
            var enumerator = equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == null)
                    continue;

                var itemDm = new ItemIdDataModel();
                itemDm.ItemId = enumerator.Current.Record.Id;
                itemDm.Count = 0;
                DataModel.WeaponItems.Add(itemDm);

                var dm = new EquipModelDataModel();
                dm.EquipId = enumerator.Current.Record.Id;
                dm.Select = false;
                DataModel.Models.Add(dm);
            }            
        }
    }

    private void OnResourceChange(IEvent ievent)
    {
        var e = ievent as Resource_Change_Event;
        if (e == null)
            return;

        if (e.Type == eResourcesType.AchievementScore)
        {
            InitEquipConfig();
            DataModel.AchiventmentPoint = e.NewValue;

            var roleId = PlayerDataManager.Instance.GetRoleId();
            if (!equipRecordDict.TryGetValue(roleId, out equipList))
            {
                return;
            }

            PlayerDataManager.Instance.NoticeData.MayaNotice = false;
            var enumerator = equipList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.CanBuy)
                {
                    if (DataModel.AchiventmentPoint >= enumerator.Current.BuyCost)
                    {
                        PlayerDataManager.Instance.NoticeData.MayaNotice = true;
                        break;
                    }
                }
            }
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void Close()
    {
    }

    public void BuyEquip(int equipId)
    {
        var tbItem = Table.GetItemBase(equipId);
        if (tbItem == null || tbItem.StoreID < 0)
        {
            return;
        }

        var tbStore = Table.GetStore(tbItem.StoreID);
        if (tbStore == null)
        {
            return;
        }

        var count = 1;
        var cost = tbStore.NeedValue * count;
        if (PlayerDataManager.Instance.GetRes(tbStore.NeedType) < cost)
        {
            var tbItemCost = Table.GetItemBase(tbStore.NeedType);
            //{0}不足！
            var str = GameUtils.GetDictionaryText(701);
            str = string.Format(str, tbItemCost.Name);
            GameUtils.ShowHintTip(str);
            PlayerDataManager.Instance.ShowItemInfoGet(tbStore.NeedType);

            if ((int)eResourcesType.GoldRes == tbStore.NeedType)
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));
            }
            return;
        }

        NetManager.Instance.StartCoroutine(StoreBuyCoroutine(tbItem.StoreID, count));
    }

    public IEnumerator StoreBuyCoroutine(int index, int count = 1)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreBuy(index, count, -1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int)ErrorCodes.OK)
                {
                    var tbStore = Table.GetStore(index);
                    //购买成功
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(431));

                    if (tbStore == null)
                    {
                        yield break;
                    }

                    PlatformHelper.UMEvent("BuyItem", tbStore.Name.ToString(), count);
                }
                else if (msg.ErrorCode == (int)ErrorCodes.Error_ItemNoInBag_All)
                {
                    var e = new ShowUIHintBoard(430);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("StoreBuy....StoreId= {0}...ErrorCode...{1}", index, msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("StoreBuy............State..." + msg.State);
            }
        }
    }


    public void ViewSkill(int equipId)
    {
        var buffList = Table.GetBuffGroup(1000).BuffID;
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.BufferListUI, new UIInitArguments()
        {
            Args = buffList
        }));
    }

    public void GotoAdvance(int equipId)
    {
        
    }

    public void OnEvent_Operate(IEvent ievent)
    {
        var e = ievent as ArtifactOpEvent;
        if (e == null)
            return;

        switch (e.Idx)
        {
            case 0:
            {
                if (DataModel.CanBuy == false)
                    return;

                var tbItem = Table.GetItemBase((int) eResourcesType.AchievementScore);
                if (DataModel.AchiventmentPoint >= DataModel.NeedPoint)
                {
                    var notice = string.Format(GameUtils.GetDictionaryText(100002118), tbItem.Name);
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, notice, "", () =>
                    {
                        BuyEquip(DataModel.SelectEquipId);
                    });
                }
                else
                {
                    var notice = GameUtils.GetDictionaryText(100002123);
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(notice));
                }
            }
                break;
            case 1:
            {
                ViewSkill(DataModel.SelectEquipId);
            }
                break;
            case 2:
            {
                GotoAdvance(DataModel.SelectEquipId);
            }
                break;
            case 3:
            {
            }
                break;
            case 4:
            {
                var have = false;
                PlayerDataManager.Instance.ForeachEquip2(bagItem =>
                {
                    if (bagItem.ItemId != -1)
                    {
                        var tbRecord = Table.GetEquipBase(bagItem.ItemId);
                        if (tbRecord != null && tbRecord.ShowEquip == 1)
                        {
                            have = true;
                            return false;
                        }
                    }
                    return true;
                });

                if (!have)
                {
                    var __enumerator6 = (PlayerDataManager.Instance.GetBag((int)eBagType.Equip).Items).GetEnumerator();
                    while (__enumerator6.MoveNext())
                    {
                        var bagData = __enumerator6.Current;
                        if (bagData != null && bagData.ItemId != -1)
                        {
                            var tbRecord = Table.GetEquipBase(bagData.ItemId);
                            if (tbRecord != null && tbRecord.ShowEquip == 1)
                            {
                                have = true;
                                break;
                            }
                        }
                    }
                }

                if (have)
                {
                    EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MyArtifactUI));
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(100002121));
                }
            }
                break;
            case 5:
            {
                DataModel.ShowTips = true;
                EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(1));
            }
                break;
            case 6:
            {
                DataModel.ShowTips = false;
                EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(-1));
            }
                break;
        }
    }

    private void SelectModel(int index)
    {
        if (index >= 0 && index < equipList.Count)
        {
            DataModel.SelectEquipId = equipList[index].Record.Id;
            DataModel.CanBuy = equipList[index].CanBuy;

            DataModel.AchiventmentPoint = PlayerDataManager.Instance.GetAchievementPoint();
            DataModel.NeedPoint = equipList[index].BuyCost;

            if (lastSelectIndex >= 0 && lastSelectIndex < DataModel.Models.Count)
            {
                DataModel.Models[lastSelectIndex].Select = false;
            }

            if (index >= 0 && index < DataModel.Models.Count)
            {
                lastSelectIndex = index;
                DataModel.Models[index].Select = true;
            }
        }        
    }

    public void OnEvent_SelectModel(IEvent ievent)
    {
        var e = ievent as ArtifactSelectEvent;
        if (e == null || e.ListItem == null)
            return;

        var index = e.ListItem.Index;
        SelectModel(index);

        //var modelDataModel = e.ListItem.Item as EquipModelDataModel;
    }

    public FrameState State { get; set; }
}