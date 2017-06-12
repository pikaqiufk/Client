#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;

#endregion

public class OperationlistController : IControllerBase
{
    public OperationlistController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(UIEvent_OperationList_Button.EVENT_TYPE, Buttons); //下拉菜单点击事件
    }

    public OperationListData Data;
    public OperationListDataModel DataModel;
    //弹出的下拉菜单按钮相应事件
    public void Buttons(IEvent ievent)
    {
        var e2 = new Close_UI_Event(UIConfig.OperationList);
        EventDispatcher.Instance.DispatchEvent(e2);
        var ee = ievent as UIEvent_OperationList_Button;

        if (GameUtils.CharacterIdIsRobot(Data.CharacterId) && ee.Index != 1)
        {
//玩家不在线
            GameUtils.ShowHintTip(200000003);
            return;
        }

        switch (ee.Index)
        {
            //发起聊天
            case 0:
            {
                var e = new ChatMainPrivateChar(Data);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //查看属性
            case 1:
            {
                PlayerDataManager.Instance.ShowPlayerInfo(Data.CharacterId);
            }
                break;
            //加为好友
            case 2:
            {
                var e = new FriendOperationEvent(0, 1, Data.CharacterName, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //加为仇人
            case 3:
            {
                var e = new FriendOperationEvent(1, 1, Data.CharacterName, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //屏蔽
            case 4:
            {
                var e = new FriendOperationEvent(2, 1, Data.CharacterName, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //删除好友
            case 5:
            {
                var e = new FriendOperationEvent(0, 0, Data.CharacterName, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //解除仇人
            case 6:
            {
                var e = new FriendOperationEvent(1, 0, Data.CharacterName, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //取消屏蔽
            case 7:
            {
                var e = new FriendOperationEvent(2, 0, Data.CharacterName, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            //邀请组队
            case 8:
            {
                EventDispatcher.Instance.DispatchEvent(new Event_TeamInvitePlayer(Data.CharacterId));
            }
                break;
            //申请进队
            case 9:
            {
                EventDispatcher.Instance.DispatchEvent(new Event_TeamApplyOtherTeam(Data.CharacterId));
            }
                break;
            //提升队长
            case 10:
            {
                EventDispatcher.Instance.DispatchEvent(new Event_TeamSwapLeader(Data.CharacterId));
            }
                break;
            //请出队伍
            case 11:
            {
                EventDispatcher.Instance.DispatchEvent(new Event_TeamKickPlayer(Data.CharacterId));
            }
                break;
            //离开队伍
            case 12:
            {
                EventDispatcher.Instance.DispatchEvent(new Event_TeamLeaveTeam());
            }
                break;
            //13 邀请入盟，14提升领袖，15提升权限，16降低权限，17请出战盟
            case 13:
            case 14:
            case 15:
            case 16:
            case 17:
            {
                var e = new UIEvent_UnionCommunication(ee.Index, Data.CharacterId);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
        }
    }

    public void CleanUp()
    {
        DataModel = new OperationListDataModel();
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

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as OperationlistArguments;
        if (args == null)
        {
            return;
        }

        Data = args.Data;
        if (Data == null)
        {
            return;
        }
        var noticeData = PlayerDataManager.Instance.NoticeData;
        var listRecord = Table.GetOperationList(Data.TableId);

        DataModel.Speek = listRecord.Speek;

        DataModel.Attribute = listRecord.Attribute;

        if (noticeData.FriendOpenFlag)
        {
            var fiend = UIManager.Instance.GetController(UIConfig.FriendUI);

            if (listRecord.AddFriend == 1)
            {
                var ret = (bool) fiend.CallFromOtherClass("IsInFriendListId", new object[] {Data.CharacterId});
                DataModel.AddFriend = ret == false ? 1 : 0;
            }
            else
            {
                DataModel.AddFriend = 0;
            }

            if (listRecord.AddEnemy == 1)
            {
                var ret = (bool) fiend.CallFromOtherClass("IsInEnemyListId", new object[] {Data.CharacterId});
                DataModel.AddEnemy = ret == false ? 1 : 0;
            }
            else
            {
                DataModel.AddEnemy = 0;
            }

            if (listRecord.AddShield == 1)
            {
                var ret = (bool) fiend.CallFromOtherClass("IsInBalckListId", new object[] {Data.CharacterId});
                DataModel.AddShield = ret == false ? 1 : 0;
            }
            else
            {
                DataModel.AddShield = 0;
            }

            if (listRecord.DelFriend == 1)
            {
                var ret = (bool) fiend.CallFromOtherClass("IsInFriendListId", new object[] {Data.CharacterId});
                DataModel.DelFriend = ret ? 1 : 0;
            }
            else
            {
                DataModel.DelFriend = 0;
            }

            if (listRecord.DelEnemy == 1)
            {
                var ret = (bool) fiend.CallFromOtherClass("IsInEnemyListId", new object[] {Data.CharacterId});
                DataModel.DelEnemy = ret ? 1 : 0;
            }
            else
            {
                DataModel.DelEnemy = 0;
            }

            if (listRecord.DelShield == 1)
            {
                var ret = (bool) fiend.CallFromOtherClass("IsInBalckListId", new object[] {Data.CharacterId});
                DataModel.DelShield = ret ? 1 : 0;
            }
            else
            {
                DataModel.DelShield = 0;
            }
        }
        else
        {
            DataModel.AddFriend = 0;
            DataModel.AddEnemy = 0;
            DataModel.AddShield = 0;
            DataModel.DelFriend = 0;
            DataModel.DelEnemy = 0;
            DataModel.DelShield = 0;
        }


        if (noticeData.TeamOpenFlag)
        {
            var team = UIManager.Instance.GetController(UIConfig.TeamFrame);

            if (listRecord.InviteTeam == 1)
            {
                var ret = (bool) team.CallFromOtherClass("IsInTeam", new object[] {Data.CharacterId});
                DataModel.InviteTeam = ret == false ? 1 : 0;
            }
            else
            {
                DataModel.InviteTeam = 0;
            }

            var hasTeam = (bool) team.CallFromOtherClass("HasTeam", null);
            if (listRecord.ApplyTeam == 1 && hasTeam == false)
            {
                DataModel.ApplyTeam = 1;
            }
            else
            {
                DataModel.ApplyTeam = 0;
            }

            if (listRecord.UpLeader == 1 && hasTeam)
            {
                DataModel.UpLeader = 1;
            }
            else
            {
                DataModel.UpLeader = 0;
            }

            if (listRecord.KickTeam == 1 && hasTeam)
            {
                DataModel.KickTeam = 1;
            }
            else
            {
                DataModel.KickTeam = 0;
            }

            if (listRecord.LeaveTeam == 1 && hasTeam)
            {
                DataModel.LeaveTeam = 1;
            }
            else
            {
                DataModel.LeaveTeam = 0;
            }
        }
        else
        {
            DataModel.InviteTeam = 0;
            DataModel.ApplyTeam = 0;
            DataModel.UpLeader = 0;
            DataModel.KickTeam = 0;
            DataModel.LeaveTeam = 0;
        }

        if (noticeData.UnionOpenFlag)
        {
            var union = UIManager.Instance.GetController(UIConfig.BattleUnionUI);
            var hasUnion = (bool) union.CallFromOtherClass("HasUnion", null);

            if (listRecord.JoinUnion == 1 && hasUnion)
            {
                DataModel.JoinUnion = 1;
            }
            else
            {
                DataModel.JoinUnion = 0;
            }

            DataModel.UpToChief = listRecord.UpChief;
            DataModel.UpAccess = listRecord.UpAccess;
            DataModel.DownAccess = listRecord.DownAccess;
            DataModel.KickUnion = listRecord.QuitUnion;
        }
        else
        {
            DataModel.JoinUnion = 0;
            DataModel.UpToChief = 0;
            DataModel.UpAccess = 0;
            DataModel.DownAccess = 0;
            DataModel.KickUnion = 0;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}