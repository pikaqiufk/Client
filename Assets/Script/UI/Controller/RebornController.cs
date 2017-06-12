#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class RebornController : IControllerBase
{
    public RebornController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(ConditionChangeEvent.EVENT_TYPE, OnConditionChange);
        EventDispatcher.Instance.AddEventListener(RebornOperateEvent.EVENT_TYPE, OnRebornOperate);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(ExDataUpDataEvent.EVENT_TYPE, OnExDataUpData);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUpChange);
        EventDispatcher.Instance.AddEventListener(LevelUpInitEvent.EVENT_TYPE, OnLevelUpChange);
    }

    public RebornDataModel DataModel;

    public void OnConditionChange(IEvent ievent)
    {
        var e = ievent as ConditionChangeEvent;
        TransmigrationRecord tbReborn = null;
        if (DataModel.RebornId != -1)
        {
            tbReborn = Table.GetTransmigration(DataModel.RebornId);
            if (tbReborn == null)
            {
                return;
            }
            if (tbReborn.ConditionCount == e.ConditionId)
            {
                RefreshShowReborn();
            }
        }
    }

    public void OnExDataInit(IEvent ievent)
    {
        var e = ievent as ExDataInitEvent;
        SetRebornId();
        RefreshShowReborn();
        RefresNoticeData();
    }

    public void OnExDataUpData(IEvent ievent)
    {
        var e = ievent as ExDataUpDataEvent;
        if (e.Key == 51)
        {
            SetRebornId();
            RefreshShowReborn();
            RefresNoticeData();
        }
    }

    public void OnLevelUpChange(IEvent ievent)
    {
        RefresNoticeData();
    }

    public void OnRebornOperate(IEvent ievent)
    {
        var e = ievent as RebornOperateEvent;
        switch (e.Type)
        {
            case 0:
            {
                Reincarnation();
            }
                break;
        }
    }

    public void RefreshShowReborn()
    {
        var robornId = PlayerDataManager.Instance.GetExData(51);
        var rebornId = robornId;
        var tbReborn = Table.GetTransmigration(rebornId);
        if (tbReborn == null || tbReborn.ConditionCount == -1)
        {
            DataModel.ShowReborn = false;
            return;
        }

        var lv = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Level;
        var dicCon = PlayerDataManager.Instance.CheckCondition(tbReborn.ConditionCount);
        if (dicCon == 0)
        {
            DataModel.ShowReborn = true;
        }
        else
        {
            DataModel.ShowReborn = false;
        }
    }

    public void RefresNoticeData()
    {
        var tbReborn = Table.GetTransmigration(DataModel.RebornId);
        if (tbReborn == null)
        {
            PlayerDataManager.Instance.NoticeData.Reborn = false;
            return;
        }
        var lv = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Level;

        if (DataModel.ShowReborn && lv >= tbReborn.TransLevel)
        {
            PlayerDataManager.Instance.NoticeData.Reborn = true;
        }
        else
        {
            PlayerDataManager.Instance.NoticeData.Reborn = false;
        }
    }

    public void Reincarnation()
    {
        var tbReborn = Table.GetTransmigration(DataModel.RebornId);
        var lv = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Level;
        if (lv < tbReborn.TransLevel)
        {
            //"等级不足"
            var e = new ShowUIHintBoard(300102);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var dicCon = PlayerDataManager.Instance.CheckCondition(tbReborn.ConditionCount);
        if (dicCon != 0)
        {
            //"条件不足"
            var e = new ShowUIHintBoard(dicCon);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold < tbReborn.NeedMoney)
        {
            //"金币不足"
            var e = new ShowUIHintBoard(210100);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.MagicDust < tbReborn.NeedDust)
        {
            //魔尘不足
            var e = new ShowUIHintBoard(210112);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        NetManager.Instance.StartCoroutine(ReincarnationCoroutine());
    }

    public IEnumerator ReincarnationCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.Reincarnation(DataModel.RebornId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new RebornPlayAnimation());

                    //var e = new Close_UI_Event(UIConfig.RebornUi, true); 
                    //EventDispatcher.Instance.DispatchEvent(e);
                    PlayerDataManager.Instance.SetExData(51, DataModel.RebornId + 1);
                    PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.AddPoint);

                    PlatformHelper.UMEvent("Reborn", DataModel.RebornId.ToString());
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_TransmigrationID
                         || msg.ErrorCode == (int) ErrorCodes.Error_ConditionNoEnough
                         || msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough
                         || msg.ErrorCode == (int) ErrorCodes.Error_DataOverflow)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error("Reincarnation Error!............ErrorCode..." + msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error("Reincarnation Error!............State..." + msg.State);
            }
        }
    }

    public void SetRebornId()
    {
        TransmigrationRecord tbReborn = null;
        if (DataModel.RebornId != -1)
        {
            tbReborn = Table.GetTransmigration(DataModel.RebornId);
            if (tbReborn == null)
            {
                return;
            }
            ConditionTrigger.Instance.RemoveCondition(tbReborn.ConditionCount);
        }
        var robornId = PlayerDataManager.Instance.GetExData(51);
        PlayerDataManager.Instance.PlayerDataModel.Attributes.Resurrection = robornId;
        DataModel.RebornId = robornId;
        tbReborn = Table.GetTransmigration(DataModel.RebornId);
        if (tbReborn == null)
        {
            return;
        }
        ConditionTrigger.Instance.PushCondition(tbReborn.ConditionCount);
        DataModel.Attributes[0].Type = 1003;
        DataModel.Attributes[0].Value = tbReborn.AttackAdd;
        DataModel.Attributes[1].Type = 10;
        DataModel.Attributes[1].Value = tbReborn.PhyDefAdd;
        DataModel.Attributes[2].Type = 11;
        DataModel.Attributes[2].Value = tbReborn.MagicDefAdd;
        DataModel.Attributes[3].Type = 19;
        DataModel.Attributes[3].Value = tbReborn.HitAdd;
        DataModel.Attributes[4].Type = 20;
        DataModel.Attributes[4].Value = tbReborn.DodgeAdd;
        DataModel.Attributes[5].Type = 13;
        DataModel.Attributes[5].Value = tbReborn.LifeAdd;

        var tbRebornNext = Table.GetTransmigration(DataModel.RebornId + 1);
        if (tbRebornNext == null)
        {
            for (var i = 0; i < 6; i++)
            {
                DataModel.Attributes[5].Change = -1;
            }
            DataModel.NextId = -1;
        }
        else
        {
            DataModel.NextId = DataModel.RebornId + 1;
            DataModel.Attributes[0].Change = tbRebornNext.AttackAdd;
            DataModel.Attributes[1].Change = tbRebornNext.PhyDefAdd;
            DataModel.Attributes[2].Change = tbRebornNext.MagicDefAdd;
            DataModel.Attributes[3].Change = tbRebornNext.HitAdd;
            DataModel.Attributes[4].Change = tbRebornNext.DodgeAdd;
            DataModel.Attributes[5].Change = tbRebornNext.LifeAdd;
        }
    }

    public void CleanUp()
    {
        DataModel = new RebornDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        if (!PlayerDataManager.Instance.GetFlag(511))
        {
            var list = new Int32Array();
            list.Items.Add(511);
            PlayerDataManager.Instance.SetFlagNet(list);
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
    }

    public void OnShow()
    {
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public FrameState State { get; set; }
}