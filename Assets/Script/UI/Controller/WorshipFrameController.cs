#region using

using System;
using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class WorshipFrameController : IControllerBase
{
    public WorshipFrameController()
    {
        EventDispatcher.Instance.AddEventListener(WorshipOpetion.EVENT_TYPE, OnOperation);
        EventDispatcher.Instance.AddEventListener(BattleUnionRefreshModelView.EVENT_TYPE, OnModelInfo);
        CleanUp();
    }

    public PlayerInfoMsg _modelInfo;
    public WorshipDataModel DataModel;

    private void OnModelInfo(IEvent ievent)
    {
        var e = ievent as BattleUnionRefreshModelView;
        _modelInfo = e.Info;
    }

    private void OnOperation(IEvent ievent)
    {
        var e = ievent as WorshipOpetion;
        switch (e.Type)
        {
            case 0:
            {
                Worship();
            }
                break;
            case 1:
            {
                ShowInfoUI();
            }
                break;
            case 2:
            {
                ShowBattleUI();
            }
                break;
            case 3:
            {
                SetCheckIndex(0);
            }
                break;
            case 4:
            {
                SetCheckIndex(1);
            }
                break;
        }
    }

    private void SetCheckIndex(int index)
    {
        DataModel.CheckSelectIndex = index;
    }

    private void ShowBattleUI()
    {
        if (PlayerDataManager.Instance.GetExData(eExdataDefine.e282) <= 0)
        {
            //先加入公会才能参加城战
            var e = new ShowUIHintBoard(270289);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.BattleUnionUI,
            new BattleUnionArguments {Tab = 5}));
    }

    private void ShowInfoUI()
    {
        if (_modelInfo == null)
        {
            return;
        }
        var charId = _modelInfo.Id;
        if (charId == ObjManager.Instance.MyPlayer.GetObjId())
        {
            var e1 = new Show_UI_Event(UIConfig.CharacterUI);
            EventDispatcher.Instance.DispatchEvent(e1);
            return;
        }
        PlayerDataManager.Instance.ShowPlayerInfo(charId);
    }

    private void Worship()
    {
        var instance = PlayerDataManager.Instance;
        var errorId = instance.CheckCondition(Table.GetClientConfig(397).Value.ToInt());
        if (errorId != 0)
        {
            //膜拜次数不足
            var e = new ShowUIHintBoard(errorId);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        if (instance.GetExData(eExdataDefine.e72) >= DataModel.WorshipTotalCount)
        {
            //膜拜次数不足
            var e = new ShowUIHintBoard(270290);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        if (DataModel.CheckSelectIndex == 0)
        {
            if (instance.GetRes((int) eResourcesType.GoldRes) < Table.GetClientConfig(391).Value.ToInt())
            {
                var e = new ShowUIHintBoard(210100);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }
        else
        {
            if (instance.GetRes((int) eResourcesType.DiamondRes) < Table.GetClientConfig(394).Value.ToInt())
            {
                var e = new ShowUIHintBoard(210102);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }
        NetManager.Instance.StartCoroutine(WorshipCoroutine());
    }

    public IEnumerator WorshipCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.Worship(DataModel.CheckSelectIndex);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    DataModel.WorshipCount++;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new WorshipDataModel();
    }

    public void OnShow()
    {
        if (_modelInfo != null)
        {
            EventDispatcher.Instance.DispatchEvent(new WorshipRefreshModelView(_modelInfo));
        }
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var instance = PlayerDataManager.Instance;

        var moneyStr = GameUtils.GetDictionaryText(270287);
        var diaStr = GameUtils.GetDictionaryText(270288);
        var money = int.Parse(Table.GetClientConfig(391).Value);
        var level = instance.GetLevel();
        var moneyGet1 =
            Table.GetSkillUpgrading(int.Parse(Table.GetClientConfig(392).Value)).GetSkillUpgradingValue(level);
        var moneyGet2 = int.Parse(Table.GetClientConfig(393).Value);
        var diamond = int.Parse(Table.GetClientConfig(394).Value);
        var diamondGet1 =
            Table.GetSkillUpgrading(int.Parse(Table.GetClientConfig(395).Value)).GetSkillUpgradingValue(level);
        var diamondGet2 = int.Parse(Table.GetClientConfig(396).Value);
        DataModel.WorshipTotalCount = int.Parse(Table.GetClientConfig(390).Value);

        DataModel.MoneyStr = String.Format(moneyStr, money, moneyGet1, moneyGet2);
        DataModel.DiaStr = String.Format(diaStr, diamond, diamondGet1, diamondGet2);
        DataModel.WorshipCount = instance.GetExData(eExdataDefine.e72);

        if (_modelInfo != null)
        {
            DataModel.CastellanName = _modelInfo.Name;
        }
        var titleName = string.Empty;
        foreach (var item in instance._battleCityDic)
        {
            if (item.Value.Type == 0)
            {
                titleName = item.Value.Name;
                break;
            }
        }
        titleName += GameUtils.GetDictionaryText(270293);
        DataModel.TitleName = titleName;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
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