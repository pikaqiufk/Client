#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;

#endregion

public class GuardController : IControllerBase
{
    public GuardController()
    {
        EventDispatcher.Instance.AddEventListener(GuardStateChange.EVENT_TYPE, OnGuardStateChange);
        EventDispatcher.Instance.AddEventListener(GuardItemOperation.EVENT_TYPE, ItemOperation);
        EventDispatcher.Instance.AddEventListener(GuardUIOperation.EVENT_TYPE, GuardOperation);
        CleanUp();
    }

    private int _mSe = 73000; //复活SkillUpgrading id
    private int _mSelectedIndex;
    private GuardItemDataModel _mSelectedItem;
    private readonly int _mSkillUpgradingId = 73000; //复活SkillUpgrading id
    private readonly int _TotalRebornCount = 20;
    public GuardDataModel DataModel;

    private IEnumerator AllianceWarRespawnGuard(int selectIndex)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AllianceWarRespawnGuard(selectIndex);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void GuardOperation(IEvent ievent)
    {
        var e = ievent as GuardUIOperation;
        switch (e.Type)
        {
            case 0:
            {
                RebornGuard();
            }
                break;
        }
    }

    private void ItemOperation(IEvent ievent)
    {
        var e = ievent as GuardItemOperation;
        var index = e.Index;
        if (e.Type == 0)
        {
            SetSelectedItem(index);
        }
    }

    private void OnGuardStateChange(IEvent ievent)
    {
        var e = ievent as GuardStateChange;
        var count = e.Lists.Count;
        if (count <= 4)
        {
            for (var i = 0; i < e.Lists.Count; i++)
            {
                DataModel.Lists[i].State = e.Lists[i];
            }
        }
        DataModel.RebornCount = _TotalRebornCount - e.ReliveCount;
    }

    private void RebornGuard()
    {
        if (DataModel.RebornCount <= 0)
        {
            // "守卫已经复活了20次，无法继续复活！"
            var ee = new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(271000), 20));
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        if (DataModel.NeedDiaCount > PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes))
        {
            var ee = new ShowUIHintBoard(300401);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        NetManager.Instance.StartCoroutine(AllianceWarRespawnGuard(_mSelectedIndex));
    }

    private void RefleshNeed()
    {
        var values = Table.GetSkillUpgrading(_mSkillUpgradingId).Values;
        if (DataModel.RebornCount <= 0)
        {
            DataModel.NeedDiaCount = values[_TotalRebornCount - 1];
        }
        else
        {
            DataModel.NeedDiaCount = values[_TotalRebornCount - DataModel.RebornCount];
        }
    }

    private void SetSelectedItem(int index)
    {
        if (_mSelectedItem != null)
        {
            _mSelectedItem.Selected = false;
        }
        _mSelectedItem = DataModel.Lists[index];
        _mSelectedItem.Selected = true;
        _mSelectedIndex = index;
        RefleshNeed();
    }

    public void RefreshData(UIInitArguments data)
    {
        var count = DataModel.Lists.Count;
        var selectId = 0;
        for (var i = 0; i < count; i++)
        {
            var item = DataModel.Lists[i];
            if (item.State == (int) eGuardState.Dead)
            {
                selectId = i;
                break;
            }
        }
        SetSelectedItem(selectId);
    }

    public void CleanUp()
    {
        DataModel = new GuardDataModel();
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

    private enum eGuardState
    {
        ALive = 0,
        Dead = 1
    }
}