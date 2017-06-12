#region using

using System.ComponentModel;
using ClientDataModel;
using DataContract;
using DataTable;
using EventSystem;
using Shared;

#endregion

public class LineConfirmController : IControllerBase
{
    public LineConfirmController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(LineMemberConfirmEvent.EVENT_TYPE, OnLineMemberConfirm);
        EventDispatcher.Instance.AddEventListener(LineMemberClickEvent.EVENT_TYPE, OnLineMemberClick);
        EventDispatcher.Instance.AddEventListener(TeamChangeEvent.EVENT_TYPE, OnTeamChange);
    }

    public LineConfirmDataModel DataModel;
    public LineMemberDataModel EmptyMemberData = new LineMemberDataModel();
    private object mCloseTrigger;

    private void CheckConfirmCount()
    {
        var count = 0;
        for (var i = 0; i < 5; i++)
        {
            var member = DataModel.SelfList[i];
            if (member.ChararterId != 0 && member.IsConfirm)
            {
                count++;
            }
            member = DataModel.OtherList[i];
            if (member.ChararterId != 0 && member.IsConfirm)
            {
                count++;
            }
        }
        DataModel.ConfirmCount = count;
    }

    private void OnCancelSelect()
    {
        DataModel.SelectData = EmptyMemberData;
    }

    private void OnClickCharInfo(ReadonlyObjectList<LineMemberDataModel> list, int index)
    {
        if (index < 0 || index >= list.Count)
        {
            return;
        }
        DataModel.SelectData = list[index];
    }

    private void OnLineMemberClick(IEvent ievent)
    {
        var e = ievent as LineMemberClickEvent;
        switch (e.Type)
        {
            case 0:
            {
                OnClickCharInfo(DataModel.SelfList, e.Index);
            }
                break;
            case 1:
            {
                OnClickCharInfo(DataModel.OtherList, e.Index);
            }
                break;
            case 2:
            {
                OnCancelSelect();
            }
                break;
            case 3:
            {
                DataModel.IsShowMini = false;
            }
                break;
            case 4:
            {
                DataModel.IsShowMini = true;
            }
                break;
        }
    }

    private void OnLineMemberConfirm(IEvent ievent)
    {
        var e = ievent as LineMemberConfirmEvent;

        var isRet = e.Type != 0;
        for (var i = 0; i < 5; i++)
        {
            var member = DataModel.SelfList[i];
            if (member.ChararterId == e.CharacterId)
            {
                if (isRet)
                {
                    member.IsConfirm = true;
                }
                else
                {
                    member.IsConcel = true;
                }
                break;
            }

            member = DataModel.OtherList[i];
            if (member.ChararterId == e.CharacterId)
            {
                if (isRet)
                {
                    member.IsConfirm = true;
                }
                else
                {
                    member.IsConcel = true;
                }
                break;
            }
        }
        if (isRet == false)
        {
            //有人取消后3秒钟关闭
            var t = Game.Instance.ServerTime.AddSeconds(3);
            if (mCloseTrigger != null)
            {
                TimeManager.Instance.DeleteTrigger(mCloseTrigger);
            }
            mCloseTrigger = TimeManager.Instance.CreateTrigger(t, () =>
            {
                TimeManager.Instance.DeleteTrigger(mCloseTrigger);
                var e1 = new Close_UI_Event(UIConfig.LineConfim);
                EventDispatcher.Instance.DispatchEvent(e1);
                mCloseTrigger = null;
            });
        }
        else
        {
            CheckConfirmCount();
        }
    }

    private void OnTeamChange(IEvent ievent)
    {
        //TeamChangeEvent e = ievent as TeamChangeEvent;
        if (State != FrameState.Open)
        {
            return;
        }
        if (DataModel.QueueId != -1)
        {
            return;
        }
        if (mCloseTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(mCloseTrigger);
            mCloseTrigger = null;
        }
        var e = new Close_UI_Event(UIConfig.LineConfim);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void SetMemberInfo(LineMemberDataModel member, TeamCharacterOne charInfo)
    {
        member.ChararterId = charInfo.CharacterId;
        member.Type = charInfo.RoleId;
        member.Ladder = charInfo.Ladder;
        member.Level = charInfo.Level;
        member.Name = charInfo.CharacterName;
        member.FightPoint = charInfo.FightPoint;
        member.IsConfirm = charInfo.QueueResult == 1;
        member.IsConcel = charInfo.QueueResult == 0;
    }

    public void CleanUp()
    {
        if (mCloseTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(mCloseTrigger);
            mCloseTrigger = null;
        }
        DataModel = new LineConfirmDataModel();
    }

    public void RefreshData(UIInitArguments data)
    {
        var arg = data as LineConfirmArguments;
        if (arg == null)
        {
            return;
        }
        OnCancelSelect();
        var msg = arg.Msg;

        var hasDouble = false;

        var leaderCheck = false;

        if (msg.Characters.Count == 0)
        {
            return;
        }
        DataModel.TotalCount = msg.Characters.Count;
        DataModel.ConfirmCount = 0;
        DataModel.IsShowMini = false;
        DataModel.QueueId = msg.QueueId;
        if (msg.QueueId != -1)
        {
            var tbQueue = Table.GetQueue(msg.QueueId);
            if (tbQueue != null)
            {
                if (tbQueue.AppType == 1)
                {
                    hasDouble = true;
                }
            }
        }
        else
        {
            if (msg.Characters[0].CharacterId == PlayerDataManager.Instance.GetGuid())
            {
                leaderCheck = true;
            }
        }


        if (hasDouble == false)
        {
            DataModel.IsBattle = false;

            for (var i = 0; i < 5; i++)
            {
                var member = DataModel.OtherList[i];
                member.Reset();
            }

            var c = msg.Characters.Count;
            for (var i = 0; i < c; i++)
            {
                var member = DataModel.SelfList[i];
                var charInfo = msg.Characters[i];
                SetMemberInfo(member, charInfo);
            }
            for (var i = c; i < 5; i++)
            {
                var member = DataModel.SelfList[i];
                member.Reset();
            }

            if (leaderCheck)
            {
                DataModel.SelfList[0].IsConfirm = true;
                DataModel.ConfirmCount = 1;
            }
        }
        else
        {
            DataModel.IsBattle = true;
            var c = msg.Characters.Count;
            var half = (c + 1)/2;

            for (int i = 0, j = half; i < half; i++, j++)
            {
                var member = DataModel.SelfList[i];
                var charInfo = msg.Characters[i];
                SetMemberInfo(member, charInfo);

                member = DataModel.OtherList[i];
                if (j >= msg.Characters.Count)
                {
                    member.Reset();
                    break;
                }
                charInfo = msg.Characters[j];
                SetMemberInfo(member, charInfo);
            }
            for (var i = half; i < 5; i++)
            {
                var member = DataModel.SelfList[i];
                member.Reset();

                member = DataModel.OtherList[i];
                member.Reset();
            }
        }

        var countDown = Table.GetClientConfig(222).ToInt();
        DataModel.CoolDown = Game.Instance.ServerTime.AddSeconds(countDown);

        if (mCloseTrigger != null)
        {
            TimeManager.Instance.DeleteTrigger(mCloseTrigger);
        }

        mCloseTrigger = TimeManager.Instance.CreateTrigger(DataModel.CoolDown, () =>
        {
            TimeManager.Instance.DeleteTrigger(mCloseTrigger);
            var e = new Close_UI_Event(UIConfig.LineConfim);
            EventDispatcher.Instance.DispatchEvent(e);
            mCloseTrigger = null;
        });
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
    }

    public FrameState State { get; set; }
}