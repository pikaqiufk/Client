#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class ServerListController : IControllerBase
{
    private static int refreshMark = 1;

    public ServerListController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(Event_ServerListCellIndex.EVENT_TYPE, ServerListCellIndex);
        EventDispatcher.Instance.AddEventListener(Event_ServerGroupListCellIndex.EVENT_TYPE, ServerGroupListCellIndex);
        EventDispatcher.Instance.AddEventListener(Event_ServerListButton.EVENT_TYPE, ServerListButton);
    }

    public ServerListDataModel mServerListDataModel;
    private Coroutine refreshCoroutine = null;

    public bool IsServerOpen()
    {
        var serverState = (ServerStateType) mServerListDataModel.SelectedServer.State;
        switch (serverState)
        {
            case ServerStateType.Prepare:
            case ServerStateType.Repair:
                return false;
            case ServerStateType.Fine:
            case ServerStateType.Busy:
            case ServerStateType.Crowded:
            case ServerStateType.Full:
                return true;
        }
        return false;
    }

    public IEnumerator PlayerSelectServerIdCoroutine()
    {
        if (!IsServerOpen())
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 300832);
            yield break;
        }

        //using (new BlockingLayerHelper(1))
        var block = new BlockingLayerHelper(1);
        {
            var msg = NetManager.Instance.PlayerSelectServerId(mServerListDataModel.SelectedServer.ServerId);
            yield return msg.SendAndWaitUntilDone();

            Logger.Debug(msg.State.ToString());
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.ServerId = msg.Request.ServerId;
                    PlayerDataManager.Instance.LastLoginServerId = msg.Request.ServerId;
                    PlayerDataManager.Instance.CharacterLists = msg.Response.Info;
					PlayerDataManager.Instance.SelectedRoleIndex = msg.Response.SelectId;
                    PlayerDataManager.Instance.ServerName = mServerListDataModel.SelectedServer.ServerName;
                    ResourceManager.PrepareScene(Resource.GetScenePath("SelectCharacter"), www =>
                    {
                        ResourceManager.Instance.StartCoroutine(ResourceManager.LoadSceneImpl("SelectCharacter", www,
                            () =>
                            {
								/*
                                UIManager.Instance.ShowUI(UIConfig.SelectRoleUI,
                                    new SelectRoleArguments
                                    {
                                        CharacterSimpleInfos = PlayerDataManager.Instance.CharacterLists,
                                        SelectId = msg.Response.SelectId,
                                        ServerName = mServerListDataModel.SelectedServer.ServerName
                                    });*/
                                block.Dispose();
                            }));
                    });
                }
                else
                {
                    GameUtils.ShowLoginTimeOutTip();
                    block.Dispose();
                }
            }
            else
            {
                block.Dispose();
            }
        }
    }

    public IEnumerator RefreshServerList(int seconds, int mark)
    {
        yield return new WaitForSeconds(seconds + 3);
        if (mark != refreshMark)
        {
            yield break;
        }
        if (State == FrameState.Open)
        {
            const int placeHolder = 0;
            var serverListMsg = NetManager.Instance.GetServerList(placeHolder);
            yield return serverListMsg.SendAndWaitUntilDone();

            if (serverListMsg.State == MessageState.Reply)
            {
                if (serverListMsg.ErrorCode == (int) ErrorCodes.OK)
                {
                    RefreshData(new ServerListArguments
                    {
                        Data = serverListMsg.Response
                    });
                }
            }
        }
    }

    public void ServerGroupListCellIndex(IEvent ievent)
    {
        var ee = ievent as Event_ServerGroupListCellIndex;
        if (ee.Index < 0 || ee.Index >= mServerListDataModel.ServerList.Count)
        {
            return;
        }
        mServerListDataModel.SelectGroupData = mServerListDataModel.ServerList[ee.Index];
        var c = mServerListDataModel.ServerList.Count;
        for (var i = 0; i < c; i++)
        {
            mServerListDataModel.ServerList[i].IsSelected = false;
        }
        mServerListDataModel.SelectGroupData.IsSelected = true;
    }

    public void ServerListButton(IEvent ievent)
    {
        var ee = ievent as Event_ServerListButton;
        switch (ee.ButtonType)
        {
            case 0:
            {
                NetManager.Instance.StartCoroutine(PlayerSelectServerIdCoroutine());
            }
                break;
            case 1:
            {
                mServerListDataModel.SelectedServer = mServerListDataModel.LastServer;
            }
                break;
            //公告按钮
            case 2:
            {
                mServerListDataModel.AnnouncementShow = !mServerListDataModel.AnnouncementShow;
                if (mServerListDataModel.AnnouncementShow)
                {
                    mServerListDataModel.ServerViewShow = false;
                }
            }
                break;
            //服务器列表开关按钮
            case 3:
            {
                mServerListDataModel.ServerViewShow = !mServerListDataModel.ServerViewShow;

                if (mServerListDataModel.ServerViewShow)
                {
                    mServerListDataModel.AnnouncementShow = false;
                }
            }
                break;
        }
    }

    public void ServerListCellIndex(IEvent ievent)
    {
        var ee = ievent as Event_ServerListCellIndex;
        if (ee.Index < 0 || ee.Index >= mServerListDataModel.SelectGroupData.ServerGroup.Count)
        {
            return;
        }
        mServerListDataModel.SelectedServer = mServerListDataModel.SelectGroupData.ServerGroup[ee.Index];
        mServerListDataModel.ServerViewShow = false;
    }

    public void CleanUp()
    {
        mServerListDataModel = new ServerListDataModel();
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
        LoginWindow.InvisibleLoginFrame();
        var showAnn = PlayerPrefs.GetInt(GameSetting.ShowAnnouncementKey, -1);
        var today = Game.Instance.ServerTime.Day;
        if (today != showAnn)
        {
            mServerListDataModel.AnnouncementShow = true;
            PlayerPrefs.SetInt(GameSetting.ShowAnnouncementKey, today);
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
        var args = data as ServerListArguments;
        if (args == null || args.Data == null)
        {
            return;
        }

        var netdata = args.Data;
        var plData = netdata.PlayerData;

        //下次刷新时间
        var waitSec = netdata.WaitSec;
        if (waitSec > 0)
        {
            refreshMark++;
            NetManager.Instance.StartCoroutine(RefreshServerList(waitSec, refreshMark));
        }

        //所有服务器数据
        mServerListDataModel.ServerList.Clear();
        var dataModel = mServerListDataModel;
        var responseData = netdata.Data;
        PlayerDataManager.Instance.ServerNames.Clear();

        var index = 1;
        var minIndex = 0;
        ServerGroupData groupdata = null;
        Uint64Array characterList;
        {
            var __list1 = responseData;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var state = __list1[__i1];
                {
                    if (index%10 == 1)
                    {
                        groupdata = new ServerGroupData();
                        dataModel.ServerList.Add(groupdata);
                        minIndex = index;
                    }
                    var serverInfo = new ServerInfoData();
                    serverInfo.ServerName = state.Name;
                    serverInfo.ServerId = state.ServerId;
                    PlayerDataManager.Instance.ServerNames.Add(serverInfo.ServerId, serverInfo.ServerName);
                    serverInfo.State = state.State;
                    serverInfo.isNew = (state.IsNew != 0);
                    serverInfo.MieShiIconStata = state.actiResult;
                    if (plData.TryGetValue(state.ServerId, out characterList))
                    {
                        serverInfo.CharacterCount = characterList.Items.Count;
                        if (serverInfo.CharacterCount != 0)
                        {
                            groupdata.RedPromptShow = 1;
                        }
                    }
                    if (groupdata != null)
                    {
                        groupdata.ServerGroup.Add(serverInfo);
                        //{0}-{1}区
                        groupdata.GroupName = string.Format(GameUtils.GetDictionaryText(270117), minIndex, index);
                    }

                    if (serverInfo.ServerId == PlayerDataManager.Instance.LastLoginServerId)
                    {
                        dataModel.SelectedServer = serverInfo;
                        dataModel.LastServer = serverInfo;
                        dataModel.SelectGroupData = groupdata;
                        dataModel.SelectGroupData.IsSelected = true;
                    }

                    index++;
                }
            }
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return mServerListDataModel;
    }

    public FrameState State { get; set; }
}