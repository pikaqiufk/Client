#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface,
                                  ITeam9xServiceInterface
{
    private int ReconnectToServerTimes;

    public void CallEnterGame(ulong characterId)
    {
        PlatformHelper.ShowToolBar();
        //后台下载可以开始了.
        BundleLoader.Instance.DownLoadCanStart = true;
        Instance.StartCoroutine(CallEnterGameCoroutine(characterId));
    }

    private IEnumerator CallEnterGameCoroutine(ulong characterId)
    {
        var ret = new List<int>();
        var co = Instance.StartCoroutine(EnterGameCoroutine(characterId, ret));
        yield return co;
        if (ret[0] == (int) MessageState.Reply)
        {
            var err = (ErrorCodes) ret[1];
            switch (err)
            {
                case ErrorCodes.OK:
                    break;
                case ErrorCodes.Error_EnterGameWait:
                {
                    Instance.NeedReconnet = false;
                    PlayerDataManager.Instance.AccountDataModel.LineUpShow = true;
                    var e = new Show_UI_Event(UIConfig.ServerListUI);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                    break;
                default:
                {
                    UIManager.Instance.ShowNetError(ret[1]);
                }
                    break;
            }
        }
    }

    public Coroutine CallReEnterGame(ulong characterId, List<int> ret)
    {
        return Instance.StartCoroutine(EnterGameCoroutine(characterId, ret));
    }

    public Coroutine CallReLoginByUserNamePassword(List<int> ret)
    {
        return Instance.StartCoroutine(LoginByUserNamePasswordGameCoroutine(ret));
    }

    private IEnumerator ConnectCoroutine(AsyncResult<int> result)
    {
        if (Instance.Connected)
        {
            IsReconnecting = false;
            Logger.Debug("Connect to [" + ServerAddress + "] succeed!");
        }
        else
        {
            yield return Instance.StartAndWaitConnect(TimeSpan.FromSeconds(3));
            if (Instance.Connected)
            {
                Logger.Debug("Connect to [" + ServerAddress + "] succeed!");
                IsReconnecting = false;
            }
            else
            {
                yield return Instance.StartAndWaitConnect(TimeSpan.FromSeconds(3));
                if (Instance.Connected)
                {
                    Logger.Debug("Connect to [" + ServerAddress + "] succeed!");
                    IsReconnecting = false;
                }
                else
                {
                    yield return Instance.StartAndWaitConnect(TimeSpan.FromSeconds(3));
                    if (Instance.Connected)
                    {
                        Logger.Debug("Connect to [" + ServerAddress + "] succeed!");
                        IsReconnecting = false;
                    }
                    else
                    {
                        Logger.Debug("Connect to [" + ServerAddress + "] failed!");
                        result.Result = 0;
                        yield return 0;
                        yield break;
                    }
                }
            }
        }

        //SendReconnectMessageToGate();

        var game = Game.Instance;
        var msg1 = Instance.QueryServerTimezone(0);
        yield return msg1.SendAndWaitUntilDone();
        if (msg1.State == MessageState.Reply && msg1.ErrorCode == 0)
        {
            var now = DateTime.Now;
            var diff = (int) Math.Round((now - DateTime.UtcNow).TotalMinutes);
            game.ServerZoneDiff = TimeSpan.FromMinutes(msg1.Response - diff);
            if (TimeZone.CurrentTimeZone.IsDaylightSavingTime(now))
            {
                game.ServerZoneDiff = game.ServerZoneDiff.Add(TimeSpan.FromHours(1));
            }
        }

        var msg = Instance.SyncTime(0);
        var watch = new Stopwatch();
        watch.Start();
        yield return msg.SendAndWaitUntilDone();
        watch.Stop();

        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                game.ServerTimeDiff = (DateTime.Now - Extension.FromServerBinary((long) msg.Response)) -
                                      TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds/2);

                var now = game.ServerTime;
                var targetTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 10).AddDays(1);

                //半夜十二点重置某些状态
                if (ZeroReplyTrigger != null)
                {
                    TimeManager.Instance.DeleteTrigger(ZeroReplyTrigger);
                    ZeroReplyTrigger = null;
                }
                ZeroReplyTrigger = TimeManager.Instance.CreateTrigger(targetTime, () =>
                {
                    game.LoginTime = game.ServerTime;
                    PlayerDataManager.Instance.ApplyAcitvityCompensate();
                    var cl = UIManager.Instance.GetController(UIConfig.PlayFrame);
                    if (cl != null)
                    {
                        cl.CallFromOtherClass("RefreshCells", null);
                    }
                }, TimeSpan.FromDays(1).Milliseconds);

                //战场双倍提示
                var time = Table.GetClientConfig(282).ToInt();
                var hour = time/100;
                var min = time%100;
                targetTime = new DateTime(now.Year, now.Month, now.Day, hour, min, 0);
                if (targetTime < now)
                {
                    targetTime = targetTime.AddDays(1);
                }
                if (ZeroBattleReplyTrigger != null)
                {
                    TimeManager.Instance.DeleteTrigger(ZeroBattleReplyTrigger);
                    ZeroBattleReplyTrigger = null;
                }
                ZeroBattleReplyTrigger = TimeManager.Instance.CreateTrigger(targetTime, () =>
                {
                    TimeManager.Instance.DeleteTrigger(ZeroBattleReplyTrigger);
                    ZeroBattleReplyTrigger = null;
                    var content = new ChatMessageContent();
                    content.Content = GameUtils.GetDictionaryText(220455);
                    GameUtils.OnReceiveChatMsg((int) eChatChannel.SystemScroll, 0, string.Empty, content);
                }, TimeSpan.FromDays(1).Milliseconds);
                result.Result = 1;
                yield break;
            }
            UIManager.Instance.ShowNetError(msg.ErrorCode);
        }
        result.Result = 0;
        yield return 0;
    }

    private IEnumerator EnterGameCoroutine(ulong characterId, List<int> ret)
    {
        using (new BlockingLayerHelper(0))
        {
            var playerData = PlayerDataManager.Instance;
            playerData.Guid = characterId;
            var msg = Instance.EnterGame(characterId);
            yield return msg.SendAndWaitUntilDone();

            Logger.Debug(msg.State.ToString());

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Logger.Debug("EnterGame.Reply");
                    playerData.CreateSysnc(characterId);
                    playerData.ResetLoginApplyState();
                    playerData.ApplyBags();
                    playerData.ApplySkills();
                    playerData.ApplyMissions(-1);
                    playerData.ApplyFlagData();
                    playerData.ApplyExtData();
                    playerData.ApplyExtData64();
                    playerData.ApplyMails();
                    playerData.ApplyBooks();
                    playerData.ApplyQueueData();
                    // playerData.ApplytReviewState();
                    playerData.ServerId = msg.Response.ServerId;
                    playerData.CharacterGuid = characterId;
                    playerData.CharacterFoundTime = Extension.FromServerBinary(msg.Response.FoundTime);

                    var friend = UIManager.Instance.GetController(UIConfig.FriendUI);
                    if (friend != null)
                    {
                        friend.CallFromOtherClass("ApplyAllInfo", new object[] {});
                    }

                    var chat = UIManager.Instance.GetController(UIConfig.ChatMainFrame);
                    if (chat != null)
                    {
                        chat.CallFromOtherClass("EnterGame", new object[] {});
                    }

                    var url = @"http://ip.ws.126.net/ipquery";
                    GameUtils.InvokeWebServer(url, AnalyzeIpAddress);
                    playerData.ApplyCityData();
                    playerData.ApplyTrading();
                    playerData.ApplyTeam();
                    playerData.InitTrigger();
                    playerData.ApplyActivityState();
                    playerData.ApplyAcitvityCompensate();
	                playerData.ApplyOperationActivity();

                    //bug统计id
                    PlatformHelper.BuglySetUserId(characterId.ToString());

                    //umeng登录
                    PlatformHelper.ProfileSignIn(characterId.ToString());
                    Game.Instance.LoginTime = Game.Instance.ServerTime;
                    GameLogic.IsFristLogin = true;

                    PlayerDataManager.Instance.ReviewState = GameSetting.Instance.ReviewState;

                    // 增加长时间不上线推送
                    int[] days = {3, 7, 15};
                    for (var i = 0; i < 3; i++)
                    {
                        var key = string.Format("OffLineDays{0}", days[i]);
                        PlatformHelper.DeleteLocalNotificationWithKey(key);

                        var date = Game.Instance.ServerTime;
                        var targetTime = date.AddDays(days[i]);
                        var time = (targetTime - Game.Instance.ServerTime).TotalSeconds;
                        var message = GameUtils.GetDictionaryText(240161 + i);
                        PlatformHelper.SetLocalNotification(key, message, time);
                    }
                }
                else
                {
                    PlayCG.Instance.Destroy();
                    //UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Debug("EnterGame..................." + msg.ErrorCode);
                }
            }
            else
            {
                PlayCG.Instance.Destroy();
                GameUtils.ShowLoginTimeOutTip();
            }

            if (ret != null)
            {
                ret.Clear();
                ret.Add((int) msg.State);
                ret.Add(msg.ErrorCode);
            }
        }
    }

    private IEnumerator LoginByUserNamePasswordGameCoroutine(List<int> ret)
    {
        var msg = Instance.PlayerLoginByUserNamePassword(PlayerDataManager.Instance.UserName,
            PlayerDataManager.Instance.Password);
        yield return msg.SendAndWaitUntilDone();
        ret.Clear();
        ret.Add((int) msg.State);
        ret.Add(msg.ErrorCode);
    }

    public IEnumerator NotifyReConnetCoroutine()
    {
        Logger.Warn("Fast Reconnect to Server success...step 3");
        ObjManager.Instance.RemoveObjExceptPlayer();
        Logger.Warn("Reconnect to Server RemoveObjExceptPlayer step 4");


        var createObj = Instance.CreateObjAround(0);
        yield return createObj.SendAndWaitUntilDone();

        if (createObj.State == MessageState.Reply && createObj.ErrorCode == (int) ErrorCodes.OK)
        {
            CreateObj(createObj.Response);

            IsReconnecting = false;
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBox));
            Logger.Warn("Reconnect to Server Fast reconnect to server over!!");
        }
        else
        {
            IsReconnecting = false;
            Game.Instance.ExitToLogin();
            Logger.Error("Reconnect to Server  CreateObjAround message state:{0},errorcode:{1}, error:5",
                createObj.State, createObj.ErrorCode);
            StartCoroutine(ReconnectToLoginCoroutine());
        }
    }

    private void OnCheckResVersionFinish(UpdateHelper.CheckVersionResult result, string message)
    {
        if (result == UpdateHelper.CheckVersionResult.NEEDUPDATE)
        {
            //startup包括资源更新流程
            Game.Instance.EnterStartup();
        }
        else
        {
            Logger.Debug("ReconnectToServer CheckUpdate ,no need update!");
            StartCoroutine(ReconnectToServerCoroutine());
        }
    }

    public override IEnumerator OnServerLost()
    {
        yield return new WaitForEndOfFrame();
        Logger.Debug("OnServerLost");

        if (!NeedReconnet)
        {
            yield break;
        }

        if (IsReconnecting)
        {
            yield break;
        }

        Logger.Warn("Show reconnecting hit box.");
        //正在重新连接...
        UIManager.Instance.ShowMessage(MessageBoxType.Cancel, 306, "", null, () =>
        {
            IsReconnecting = false;
            Game.Instance.ExitToLogin();
        }, true);
        ReconnectToServer();
    }

    private IEnumerator ReconnectToLoginCoroutine()
    {
        Logger.Warn("Reconnect to Server Relogin and enter game... step 5");
        ObjManager.Instance.RemoveObjExceptPlayer();
        Logger.Warn("Reconnect to Server checkChannel... step 6");
        if (!GameUtils.IsOurChannel())
        {
            Logger.Warn("Reconnect to Server checkChannel  Channel!=our ... step 7");
            IsReconnecting = false;
            Game.Instance.ExitToLogin();
            yield break;
        }

        var ret = new List<int>();
        // 2.
        var co = CallReLoginByUserNamePassword(ret);
        yield return co;
        if (ret.Count == 2 && ret[0] == (int) MessageState.Reply)
        {
            Logger.Warn("Reconnect to Server CallReLoginByUserNamePassword message replay ... step 8");
            if (ret[1] == (int) ErrorCodes.OK)
            {
                Logger.Warn("Reconnect to Server CallReLoginByUserNamePassword ErrorCode=OK ... step 9");
                // 3.
                co = CallReEnterGame(PlayerDataManager.Instance.Guid, ret);
                yield return co;
                if (ret.Count == 2 && ret[0] == (int) MessageState.Reply)
                {
                    if (ret[1] == (int) ErrorCodes.OK)
                    {
                        Logger.Warn("Reconnect to Server CallReEnterGame OK... step 10");
                        NeedReconnet = true;
                        if (ObjManager.Instance != null && ObjManager.Instance.MyPlayer != null)
                        {
                            ObjManager.Instance.MyPlayer.RemoveAllBuff();
                        }
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_ClearBuffList());

                        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBox));

                        Logger.Warn("Reconnect to Server Close_UI_Event UIConfig.MessageBox ... step 11");

                        // 4. 重新设置角色坐标
                        var data = PlayerDataManager.Instance.mInitBaseAttr;
                        if (ObjManager.Instance.MyPlayer != null)
                        {
                            ObjManager.Instance.MyPlayer.Position = GameLogic.GetTerrainPosition(data.X, data.Y);
                        }
                        Logger.Warn("Reconnect to Server Reset Player Postion ... step 12");

                        //5. 重新刷新主界面buff列表
                        var count = data.Buff.Count;
                        for (var i = 0; i < count; i++)
                        {
                            var buffResult = data.Buff[i];
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_SyncBuffCell(buffResult));
                        }
                        Logger.Warn("Reconnect to Server refresh buff ... step 13");

                        Logger.Warn("Reconnect to Server Successed!!!! ... step 14");
                    }
                    else if (ret[1] == (int) ErrorCodes.Error_EnterGameWait)
                    {
                        IsReconnecting = false;
                        Logger.Error("Reconnect EnterGame failed! errorcode = {0}", ret[1]);
                        Game.Instance.ExitToLogin();
                    }
                    else
                    {
                        NeedReconnet = false;
                        UIManager.Instance.ShowNetError(ret[1]);
                    }
                }
            }
            // 排队
            else if (ret[1] == (int) ErrorCodes.Error_PLayerLoginWait)
            {
                IsReconnecting = false;
                Logger.Warn("Reconnect to Server ErrorCodes.Error_PLayerLoginWait step 15");
                Game.Instance.ExitToServerList();
            }
            else if (ret[1] == (int) ErrorCodes.PasswordIncorrect)
            {
                IsReconnecting = false;
                Logger.Error("Reconnect to Server ErrorCodes.PasswordIncorrect username:{0},password{1} error:6",
                    PlayerDataManager.Instance.UserName, PlayerDataManager.Instance.Password);
                Game.Instance.ExitToLogin();
            }
            else
            {
                Logger.Error("Reconnect to Server CallReEnterGame timeout! error:7");
                IsReconnecting = false;
                StartCoroutine(OnServerLost());
            }
        }
        else
        {
            ReconnectToServerTimes++;
            if (ReconnectToServerTimes > 3)
            {
                ReconnectToServerTimes = 0;
                IsReconnecting = false;
                Logger.Error("Reconnect to Server CallReLoginByUserNamePassword timeout! retrytimes more then 3 error:8");
                Game.Instance.ExitToLogin();
                yield break;
            }

            Logger.Error("Reconnect to Server CallReLoginByUserNamePassword timeout! error:9");
            IsReconnecting = false;
            StartCoroutine(OnServerLost());
        }
    }

    public void ReconnectToServer()
    {
        Logger.Warn("befor ReconnectToServer checkUpdate!");
        //先检查版本更新在重连
        if (GameSetting.Instance.UpdateEnable)
        {
            Logger.Warn("befor ReconnectToServer checkUpdate! updateEnable = true");
            var updateHelper = new UpdateHelper();
            updateHelper.CheckVersion(OnCheckResVersionFinish);
        }
        else
        {
            Logger.Warn("befor ReconnectToServer checkUpdate! updateEnable = false");
            OnCheckResVersionFinish(UpdateHelper.CheckVersionResult.NONEEDUPDATE, "");
        }
    }

    private IEnumerator ReconnectToServerCoroutine()
    {
        Logger.Warn("Start Reconnect to Server...");

        if (IsReconnecting)
        {
            Logger.Error("----ReconnectToServerCoroutine --- isReconnecting error:0");
            yield break;
        }

        IsReconnecting = true;

        if (Instance && Instance.Connected)
        {
            Logger.Error("----ReconnectToServerCoroutine --- Instance == null && Instance.Connected=true error:1");
            yield break;
        }

        if (PlayerDataManager.Instance == null || SceneManager.Instance == null)
        {
            Logger.Error(
                "----ReconnectToServerCoroutine ---PlayerDataManager.Instance == null || SceneManager.Instance == null error:2");
            yield break;
        }

        // 1.

        Logger.Warn("Reconnect to Server PrepareForReconnect step 1 ");
        Instance.PrepareForReconnect();
        Logger.Warn("Reconnect to Server ConnectToGate step 2 ");
        var result = new AsyncResult<int>();
        var co = Instance.ConnectToGate(result);
        yield return co;
        Logger.Warn("Reconnect to Server ConnectToGate return {0} step 3 ", co);

        if (result.Result != 1)
        {
            Logger.Error("Reconnect to Server ConnectToGate return {0} error:3 ", co);
            OnReconnectFail();
            yield break;
        }

        if (!Instance.Connected)
        {
            Logger.Error("Reconnect to Server !Instance.Connected error:4");
            OnReconnectFail();
            yield break;
        }

        var characterId = PlayerDataManager.Instance.CharacterGuid;

        if (characterId != 0ul)
        {
            var msg = Instance.ReConnet(0, characterId);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //重连成功，等待NotifyReConnet返回处理
                    Logger.Warn("Fast Reconnect to Server success...step 4");
                    yield break;
                }
            }
        }

        //         if (Instance.ReconnectSuccess)
        //         {
        //             Logger.Warn("Fast Reconnect to Server success...step 3");
        //             ObjManager.Instance.RemoveObjExceptPlayer();
        //             Logger.Warn("Reconnect to Server RemoveObjExceptPlayer step 4");
        // 
        // 
        //             var createObj = Instance.CreateObjAround(0);
        //             yield return createObj.SendAndWaitUntilDone();
        // 
        //             if (createObj.State == MessageState.Reply && createObj.ErrorCode == (int) ErrorCodes.OK)
        //             {
        //                 CreateObj(createObj.Response);
        // 
        //                 IsReconnecting = false;
        //                 EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MessageBox));
        //                 Logger.Warn("Reconnect to Server Fast reconnect to server over!!");
        //                 yield break;
        //             }
        //             else
        //             {
        //                 Logger.Error("Reconnect to Server  CreateObjAround message state:{0},errorcode:{1}, error:5", createObj.State, createObj.ErrorCode);
        //                 IsReconnecting = false;
        //                 Game.Instance.ExitToLogin();
        //             }
        //         }

        StartCoroutine(ReconnectToLoginCoroutine());
    }

    public void Kick(int type)
    {
        NeedReconnet = false;
        Instance.Stop();
        PlayerDataManager.Instance.Guid = 0ul;
        var str = "";
        var kickType = (KickClientType) type;
        switch (kickType)
        {
            case KickClientType.OtherLogin:
                //您的账号已经在其他设备登陆，请注意账号安全！
                str = GameUtils.GetDictionaryText(300853);
                break;
            case KickClientType.ChangeServer:
                //请重新登录游戏完成角色转移操作！
                str = GameUtils.GetDictionaryText(300854);
                break;
            case KickClientType.ChangeServerOK:
                //角色转移完成，请重新登录游戏！
                str = GameUtils.GetDictionaryText(300855);
                break;

            case KickClientType.GmKick:
                //您已被管理员强制下线！
                str = GameUtils.GetDictionaryText(300856);
                break;

            case KickClientType.LoginTimeOut:
                //登陆超时
                str = GameUtils.GetDictionaryText(300857);
                break;
//             case KickClientType.LostLine:
//             case KickClientType.BacktoLogin:                
            default:
                return;
                break;
        }
        UIManager.Instance.ShowMessage(MessageBoxType.Ok, str, "", () => { Game.Instance.ExitToLogin(); }, null, true);
    }

    public void NotifyReConnet(int result)
    {
        StartCoroutine(NotifyReConnetCoroutine());
    }
}