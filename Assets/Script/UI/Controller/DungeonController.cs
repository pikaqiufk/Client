#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class DungeonController : IControllerBase
{
    public static Dictionary<int, int> DealErrs = new Dictionary<int, int>
    {
        {(int) ErrorCodes.Error_LevelNoEnough, 300901},
        {(int) ErrorCodes.Error_FubenCountNotEnough, 466},
        {(int) ErrorCodes.ItemNotEnough, 467},
        {(int) ErrorCodes.Error_FubenRewardNotReceived, 497},
        {(int) ErrorCodes.Unline, 498},
        {(int) ErrorCodes.Error_CharacterOutLine, 498},
        {(int) ErrorCodes.Error_AlreadyInThisDungeon, 493},
        {(int) ErrorCodes.Error_CharacterCantQueue, 544}
    };

    public DungeonController()
    {
        CleanUp();
        EventDispatcher.Instance.AddEventListener(DungeonGroupCellClick2.EVENT_TYPE, OnClickDungeonGroupCell);
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnInitExData);
        EventDispatcher.Instance.AddEventListener(DungeonBtnClick.EVENT_TYPE, OnClickBtn);
        EventDispatcher.Instance.AddEventListener(DungeonInfosMainInfo.EVENT_TYPE, OnClickSelectMainDungeon);
        EventDispatcher.Instance.AddEventListener(QueneUpdateEvent.EVENT_TYPE, OnUpdateQuene);
        EventDispatcher.Instance.AddEventListener(DungeonSweepRandAward.EVENT_TYPE, OnDungeonSweepRandAward);
        EventDispatcher.Instance.AddEventListener(DungeonResetCountUpdate.EVENT_TYPE, OnDungeonResetCountUpdate);
        EventDispatcher.Instance.AddEventListener(DungeonEnterCountUpdate.EVENT_TYPE, OnDungeonEnterCountUpdate);
        EventDispatcher.Instance.AddEventListener(DungeonSetScan.EVENT_TYPE, SetShowScan);
        EventDispatcher.Instance.AddEventListener(ExitFuBenWithOutMessageBoxEvent.EVENT_TYPE, ExitFuBenWithOutMessageBox);
        
    }

    public int mDrawId;
    public int mDrawIndex;
    //0 none 1 rank 2 fight 3 rank & fight
    public DungeonDataModel DataModel { get; set; }

    public QueueUpDataModel QueueUpData
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.QueueUpData; }
    }

    public void AnalyseNotice()
    {
        var count = 0;
        {
            var __enumerator14 = (DataModel.MainInfos).GetEnumerator();
            while (__enumerator14.MoveNext())
            {
                var info = __enumerator14.Current;
                {
                    var restCount = 0;
                    {
                        var __enumerator25 = (info.Infos).GetEnumerator();
                        while (__enumerator25.MoveNext())
                        {
                            var i = __enumerator25.Current;
                            {
                                if (i.IsLock == false)
                                {
                                    restCount += i.TotalCount - i.EnterCount;
                                }
                            }
                        }
                    }
                    info.DungeonCount = restCount;
                    count += restCount;
                }
            }
        }
        PlayerDataManager.Instance.NoticeData.DungeonMain = count;
    }

    public IEnumerator ConfirmEnterTeamDungeonCoroutine(int deungeonId, int isOk)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ResultTeamEnterFuben(deungeonId, isOk);
            yield return msg.SendAndWaitUntilDone();

            //副本是否进入成功无任何返回值。
            //         if (msg.State == MessageState.Reply)
            //         {
            //             if (msg.ErrorCode == (int)ErrorCodes.OK)
            //             {
            //                 //                 QueueUpData.QueueId = -1;
            //             }
            //             else
            //             {
            //                 UIManager.Instance.ShowNetError(msg.ErrorCode);
            //                 Logger.Warn(".....ResultTeamEnterFuben.......{0}.", msg.ErrorCode);
            //             }
            //         }
            //         else
            //         {
            //             Logger.Warn(".....ResultTeamEnterFuben.......{0}.", msg.State);
            //         }
            //副本是否进入成功无任何返回值。
            if (isOk == 1)
            {
                QueueUpData.QueueId = -1;
                RefreshQueueInfo();
                EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(Game.Instance.ServerTime, -1));
            }
        }
    }

    //处理网络错误消息
    public bool DealWithErrorCode(int errCode, int fubenId, List<ulong> playerIds)
    {
        if (DealErrs.Keys.Contains(errCode))
        {
            var dicId = DealErrs[errCode];
            if (playerIds.Count <= 0)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicId));
            }
            else
            {
                var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
                var team = teamData.TeamList.Where(p => p.Guid != 0ul && p.Level > 0);
                var players = team.Where(p => playerIds.Contains(p.Guid));
                var names = players.Aggregate(string.Empty, (current, p) => current + (p.Name + ","));
                if (names.Length <= 0)
                {
                    return true;
                }
                //特殊处理！！！
                if (errCode == (int) ErrorCodes.Error_LevelNoEnough)
                {
                    var tbFuben = Table.GetFuben(fubenId);
                    var assistType = (eDungeonAssistType) tbFuben.AssistType;
                    if (assistType == eDungeonAssistType.BloodCastle || assistType == eDungeonAssistType.DevilSquare)
                    {
                        var playerData = PlayerDataManager.Instance;
                        var fubenCount = playerData.GetExData(tbFuben.TotleExdata);
                        if (fubenCount > 0)
                        {
                            dicId = 489;
                        }
                        else
                        {
                            dicId = 491;
                        }
                    }
                }
                names = names.Substring(0, names.Length - 1);
                var content = string.Format(GameUtils.GetDictionaryText(dicId), names);
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(content));
            }
            return true;
        }
        return false;
    }

    public void EnterMainDungeon()
    {
        if (PlayerDataManager.Instance.IsInPvPScnen())
        {
            GameUtils.ShowHintTip(456);
            return;
        }
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        if (sceneId == tbDungeon.SceneId)
        {
            //已经在此副本当中了
            var e = new ShowUIHintBoard(270081);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var playerData = PlayerDataManager.Instance;
        var dicCom = playerData.CheckCondition(tbDungeon.EnterConditionId);
        if (dicCom != 0)
        {
            //不符合副本进入条件 270234
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicCom));
            return;
        }
        if (data.EnterCount == data.TotalCount)
        {
            if (data.ResetCount < tbDungeon.TodayBuyCount)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(434));
            }
            else
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(438));
            }
            return;
        }

        var tbDungeonNeedItemIdLength1 = tbDungeon.NeedItemId.Count;
        for (var i = 0; i < tbDungeonNeedItemIdLength1; i++)
        {
            if (tbDungeon.NeedItemId[i] != -1)
            {
                if (playerData.GetItemCount(tbDungeon.NeedItemId[i]) < tbDungeon.NeedItemCount[i])
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                    return;
                }
            }
        }
        //GameUtils.EnterFuben(DataModel.SelectDungeon.InfoData.Id);
        NoticeEnterDungeonFight(tbDungeon.FightPoint,
            () =>
            {
                if (DataModel != null)
                    GameUtils.EnterFuben(DataModel.SelectDungeon.InfoData.Id);                    
            });
    }

    public void NoticeEnterDungeonFight(int needFightValue, Action callback)
    {
        var noticePercent = 100;
        var confRecord = Table.GetClientConfig(498);
        if (confRecord != null)
        {
            var intValue = confRecord.ToInt();
            if (intValue != -1)
                noticePercent = intValue;
        }
        var myFightValue = PlayerDataManager.Instance.PlayerDataModel.Attributes.FightValue;
        if (myFightValue < needFightValue * noticePercent / 100)
        {
            UIManager.Instance.ShowMessage(
                MessageBoxType.OkCancel,
                1712,
                "",
                callback);
        }
        else
        {
            callback();
        }
    }

    public void EnterVipDungeon()
    {
        if (PlayerDataManager.Instance.IsInPvPScnen())
        {
            GameUtils.ShowHintTip(456);
            return;
        }
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        if (sceneId == tbDungeon.SceneId)
        {
            //已经在此副本当中了
            var e = new ShowUIHintBoard(270081);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var playerData = PlayerDataManager.Instance;
        var dicCom = playerData.CheckCondition(tbDungeon.EnterConditionId);
        if (dicCom != 0)
        {
            //不符合副本进入条件 270234
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicCom));
            return;
        }
        if (data.EnterCount == data.TotalCount)
        {
            if (data.ResetCount < tbDungeon.TodayBuyCount)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(490));
            }
            else
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(490));
            }
            return;
        }

        var tbDungeonNeedItemIdLength1 = tbDungeon.NeedItemId.Count;
        for (var i = 0; i < tbDungeonNeedItemIdLength1; i++)
        {
            if (tbDungeon.NeedItemId[i] != -1)
            {
                if (playerData.GetItemCount(tbDungeon.NeedItemId[i]) < tbDungeon.NeedItemCount[i])
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                    return;
                }
            }
        }
        //GameUtils.EnterFuben(DataModel.SelectDungeon.InfoData.Id);
        NoticeEnterDungeonFight(tbDungeon.FightPoint,
            () =>
            {
                if (DataModel != null)
                    GameUtils.EnterFuben(DataModel.SelectDungeon.InfoData.Id);
            });
    }

    public void EnterTeamDungeon()
    {
        if (PlayerDataManager.Instance.IsInPvPScnen())
        {
            GameUtils.ShowHintTip(456);
            return;
        }
        var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
        var count = teamData.TeamList.Count(i => i.Guid != 0);
        if (count == 0)
        {
            //
            var e = new ShowUIHintBoard(439);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        if (teamData.TeamList[0].Guid != ObjManager.Instance.MyPlayer.GetObjId())
        {
            //
            var e = new ShowUIHintBoard(440);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        if (sceneId == tbDungeon.SceneId)
        {
            //已经在此副本当中了
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270081));
            return;
        }

        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            var oldTbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            var newTbScene = Table.GetScene(sceneId);

            if (oldTbScene != null && newTbScene != null)
            {
                if (oldTbScene.FubenId != -1 && newTbScene.FubenId != -1)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210123));
                    return;
                }
            }
        }

        var playerData = PlayerDataManager.Instance;
        var dicCom = playerData.CheckCondition(tbDungeon.EnterConditionId);
        if (dicCom != 0)
        {
            //不符合副本进入条件 270234
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicCom));
            return;
        }

        if (tbDungeon.QueueParam == -1)
        {
            return;
        }

        var tbQueue = Table.GetQueue(tbDungeon.QueueParam);


        if (count < tbQueue.CountLimit)
        {
//             UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 441, "",
//                 () => { NetManager.Instance.StartCoroutine(EnterTeamDungeonCoroutine()); });
//             return;
        }
        if (count > tbQueue.CountLimit)
        {
            //队伍人数大于副本的要求人数
            var e = new ShowUIHintBoard(469);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        if (data.EnterCount == data.TotalCount)
        {
            if (data.ResetCount < tbDungeon.TodayBuyCount)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(434));
            }
            else
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(438));
            }
            return;
        }

        NoticeEnterDungeonFight(tbDungeon.FightPoint,
                () =>
                {
                    NetManager.Instance.StartCoroutine(EnterTeamDungeonCoroutine());
                }
            );
        //NetManager.Instance.StartCoroutine(EnterTeamDungeonCoroutine());
    }

    private IEnumerator EnterTeamDungeonCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var id = DataModel.SelectDungeon.InfoData.Id;
            var msg = NetManager.Instance.TeamEnterFuben(id, -1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    QueueUpData.QueueId = -1;
                    RefreshQueueInfo();
                    PlatformHelper.UMEvent("Fuben", "Enter", id);
                }
                else if (DealWithErrorCode(msg.ErrorCode, id, msg.Response.Items))
                {
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenID)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_QueueCountMax)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveQueue)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unline)
                {
                    //有队友不在线
                    var e = new ShowUIHintBoard(448);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenCountNotEnough)
                {
                    //{0}副本次数不够
                    var charId = msg.Response.Items[0];
                    var name = PlayerDataManager.Instance.GetTeamMemberName(charId);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var str = GameUtils.GetDictionaryText(466);
                        str = string.Format(str, name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    //{{0}道具不足
                    var charId = msg.Response.Items[0];
                    var name = PlayerDataManager.Instance.GetTeamMemberName(charId);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var str = GameUtils.GetDictionaryText(467);
                        str = string.Format(str, name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_LevelNoEnough)
                {
                    //{{0}不符合副本条件
                    var charId = msg.Response.Items[0];
                    var name = PlayerDataManager.Instance.GetTeamMemberName(charId);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var str = GameUtils.GetDictionaryText(468);
                        str = string.Format(str, name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNoTeam
                         || msg.ErrorCode == (int) ErrorCodes.Error_CharacterOutLine
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamNotSame
                         || msg.ErrorCode == (int) ErrorCodes.Error_TeamNotFind)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....EnterTeamDungeonCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....EnterTeamDungeonCoroutine.......{0}.", msg.State);
            }
        }
    }

    public IEnumerator ExitDungeonCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ExitDungeon(-1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var id = DataModel.SelectDungeon.InfoData.Id;
                    PlatformHelper.UMEvent("Fuben", "Exit", id.ToString());
                }
                else
                {
                    Logger.Error(".....ExitDungeon.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....ExitDungeon.......{0}.", msg.State);
            }
        }
    }

    public int GetQueneDungeonId()
    {
        if (QueueUpData.QueueId != -1)
        {
            var tbQuene = Table.GetQueue(QueueUpData.QueueId);
            if (tbQuene != null)
            {
                if (tbQuene.AppType == 0)
                {
                    return tbQuene.Param;
                }
            }
        }
        return -1;
    }

    public void OnClickBtn(IEvent ievent)
    {
        var e = ievent as DungeonBtnClick;
        switch (e.Type)
        {
            case eDungeonType.Main:
            {
                switch (e.Index)
                {
                    case 1:
                    {
                        EnterMainDungeon();
                    }
                        break;
                    case 2:
                    {
                        ResetMainDungeon();
                    }
                        break;
                    case 3:
                    {
                        SweepMainDungeon();
                    }
                        break;
                }
            }
                break;
            case eDungeonType.Team:
            {
                switch (e.Index)
                {
                    case 1:
                    {
                        EnterTeamDungeon();
                    }
                        break;
                    case 2:
                    {
                        TeamDungeonLineup();
                    }
                        break;
                    case 3:
                    {
                        ResetTeamDungeon();
                    }
                        break;
                    case 4:
                    {
                        NetManager.Instance.StartCoroutine(ConfirmEnterTeamDungeonCoroutine(e.ExData, 1));
                    }
                        break;
                    case 5:
                    {
                        NetManager.Instance.StartCoroutine(ConfirmEnterTeamDungeonCoroutine(e.ExData, 0));
                    }
                        break;
                }
            }
                break;
            case eDungeonType.Vip:
            {
                if (e.Index == 1)
                {
                    EnterVipDungeon();
                }
            }
                break;

            case eDungeonType.Invalid:
            {
                switch (e.Index)
                {
                    case 100:
                    {
                        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
                        var tbScene = Table.GetScene(sceneId);
                        var tbFuben = Table.GetFuben(tbScene.FubenId);
                        if (tbFuben == null)
                        {
                            return;
                        }
                        switch (tbFuben.AssistType)
                        {
                            case 4:
                            case 5:
                            {
                                if (PlayerDataManager.Instance.PlayerDataModel.DungeonState == (int) eDungeonState.Start)
                                {
//活动未完成，此时退出只能获得极少数参与奖励！
                                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 220444, "",
                                        () => { NetManager.Instance.StartCoroutine(ExitDungeonCoroutine()); });
                                    return;
                                }
                            }
                                break;
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                            case 15:
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                            {
                                //是否退出战场
                                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                                    GameUtils.GetDictionaryText(270080), "",
                                    () => { NetManager.Instance.StartCoroutine(ExitDungeonCoroutine()); });
                            }
                                return;
                        }
                        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 220504, "",
                            () => { NetManager.Instance.StartCoroutine(ExitDungeonCoroutine()); });
                    }
                        break;
                }
            }
                break;
        }
    }

    public void OnClickDungeonGroupCell(IEvent ievent)
    {
        var e = ievent as DungeonGroupCellClick2;
        var dungeonType = eDungeonType.Main;
        if (e.Type == 1)
        {
            dungeonType = eDungeonType.Team;
        }
        else if (e.Type == 2)
        {
            dungeonType = eDungeonType.Vip;
        }
        else
        {
            dungeonType = eDungeonType.Main;
        }
        DataModel.IsShowScan = 0;
        switch (dungeonType)
        {
            case eDungeonType.Main:
            {
                SelectMainDungeonGroup(e.Index);
            }
                break;
            case eDungeonType.Exp:
                break;
            case eDungeonType.Gold:
                break;
            case eDungeonType.Team:
            {
                SelectTeamDungeonGroup(e.Index);
            }
                break;
            case eDungeonType.Vip:
            {
                SelectVipDungeonGroup(e.Index);
            }
                break;
            default:
                break;
        }
    }

    public void OnClickSelectMainDungeon(IEvent ievent)
    {
        var e = ievent as DungeonInfosMainInfo;
        switch (e.Type)
        {
            case eDungeonType.Main:
            {
                SelectMainDungeonInfo(e.Index);
            }
                break;
            case eDungeonType.Exp:
                break;
            case eDungeonType.Team:
            {
                SelectTeamDungeonInfo(e.Index);
            }
                break;
            case eDungeonType.Gold:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void OnDungeonEnterCountUpdate(IEvent ievent)
    {
        var e = ievent as DungeonEnterCountUpdate;
        {
            // foreach(var info in DataModel.MainInfos)
            var __enumerator1 = (DataModel.MainInfos).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var info = __enumerator1.Current;
                {
                    {
                        // foreach(var i in info.Infos)
                        var __enumerator15 = (info.Infos).GetEnumerator();
                        while (__enumerator15.MoveNext())
                        {
                            var i = __enumerator15.Current;
                            {
                                var id = i.Id;
                                if (id != -1 && e.DungeonId == id)
                                {
                                    var tbDungeon = Table.GetFuben(id);

                                    i.ResetCount = PlayerDataManager.Instance.GetExData(tbDungeon.ResetExdata);
                                    i.EnterCount = PlayerDataManager.Instance.GetExData(tbDungeon.TodayCountExdata);
                                    i.TotalCount = i.ResetCount + tbDungeon.TodayCount;
                                    i.CompleteCount = i.EnterCount.ToString();
                                    UpdateCurCount(id, i.EnterCount, i.TotalCount);
                                    if (i.TotalCount > 0)
                                    {
                                        i.CompleteCount += "/" + i.TotalCount;
                                    }
                                    AnalyseNotice();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        {
            // foreach(var info in DataModel.TeamInfos)
            var __enumerator2 = (DataModel.TeamInfos).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var info = __enumerator2.Current;
                {
                    {
                        // foreach(var i in info.Infos)
                        var __enumerator16 = (info.Infos).GetEnumerator();
                        while (__enumerator16.MoveNext())
                        {
                            var i = __enumerator16.Current;
                            {
                                var id = i.Id;
                                if (id != -1 && e.DungeonId == id)
                                {
                                    var tbFuben = Table.GetFuben(id);

                                    i.ResetCount = PlayerDataManager.Instance.GetExData(tbFuben.ResetExdata);
                                    i.EnterCount = PlayerDataManager.Instance.GetExData(tbFuben.TodayCountExdata);
                                    i.TotalCount = i.ResetCount + tbFuben.TodayCount;
                                    UpdateCurCount(id, i.EnterCount, i.TotalCount);
                                    i.CompleteCount = i.EnterCount.ToString();
                                    if (i.TotalCount > 0)
                                    {
                                        i.CompleteCount += "/" + i.TotalCount;
                                    }
                                    if (i.IsDynamicReward)
                                    {
                                        for (int j = 0, jmax = tbFuben.DisplayCount.Count; j < jmax; ++j)
                                        {
                                            var itemId = tbFuben.DisplayReward[j];
                                            var skillUpgradeId = tbFuben.DisplayCount[j];
                                            if (itemId == -1)
                                            {
                                                break;
                                            }
                                            var itemCount =
                                                Table.GetSkillUpgrading(skillUpgradeId)
                                                    .GetSkillUpgradingValue(i.EnterCount + 1);
                                            i.RewardCount[j] = itemCount;
                                        }
                                    }
                                    AnalyseNotice();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        // vip
        foreach (var varInfo in DataModel.VipInfos)
        {
            foreach (var iValue in varInfo.Infos)
            {
                var i = iValue;
                {
                    var id = i.Id;
                    if (id != -1 && e.DungeonId == id)
                    {
                        var tbDungeon = Table.GetFuben(id);

                        i.ResetCount = PlayerDataManager.Instance.GetExData(tbDungeon.ResetExdata);
                        i.EnterCount = PlayerDataManager.Instance.GetExData(tbDungeon.TodayCountExdata);
                        i.TotalCount = i.ResetCount + tbDungeon.TodayCount;
                        UpdateCurCount(id, i.EnterCount, i.TotalCount);
                        i.CompleteCount = i.EnterCount.ToString();
                        if (i.TotalCount > 0)
                        {
                            i.CompleteCount += "/" + i.TotalCount;
                        }
                        AnalyseNotice();
                        return;
                    }
                }
            }            
        }
    }
    public void UpdateCurCount(int id,int cur,int max)
    {
        if (id != DataModel.SelectDungeon.InfoData.Id)
            return;
        DataModel.CurCanSweep = max > cur;
        DataModel.CurSweepTimes = string.Format(GameUtils.GetDictionaryText(100001160), max - cur);
    }
    public void OnDungeonResetCountUpdate(IEvent ievent)

    {
        var e = ievent as DungeonResetCountUpdate;
        {
            // foreach(var info in DataModel.MainInfos)
            var __enumerator3 = (DataModel.MainInfos).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var info = __enumerator3.Current;
                {
                    {
                        // foreach(var i in info.Infos)
                        var __enumerator17 = (info.Infos).GetEnumerator();
                        while (__enumerator17.MoveNext())
                        {
                            var i = __enumerator17.Current;
                            {
                                var id = i.Id;
                                if (id != -1 && e.DungeonId == id)
                                {
                                    var tbDungeon = Table.GetFuben(id);

                                    i.ResetCount = PlayerDataManager.Instance.GetExData(tbDungeon.ResetExdata);
                                    i.EnterCount = PlayerDataManager.Instance.GetExData(tbDungeon.TodayCountExdata);
                                    i.TotalCount = i.ResetCount + tbDungeon.TodayCount;
                                    UpdateCurCount(id, i.EnterCount, i.TotalCount);
                                    i.CompleteCount = i.EnterCount.ToString();
                                    if (i.TotalCount > 0)
                                    {
                                        i.CompleteCount += "/" + i.TotalCount;
                                    }
                                    AnalyseNotice();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        {
            // foreach(var info in DataModel.TeamInfos)
            var __enumerator4 = (DataModel.TeamInfos).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var info = __enumerator4.Current;
                {
                    {
                        // foreach(var i in info.Infos)
                        var __enumerator18 = (info.Infos).GetEnumerator();
                        while (__enumerator18.MoveNext())
                        {
                            var i = __enumerator18.Current;
                            {
                                var id = i.Id;
                                if (id != -1 && e.DungeonId == id)
                                {
                                    var tbDungeon = Table.GetFuben(id);

                                    i.ResetCount = PlayerDataManager.Instance.GetExData(tbDungeon.ResetExdata);
                                    i.EnterCount = PlayerDataManager.Instance.GetExData(tbDungeon.TodayCountExdata);
                                    i.TotalCount = i.ResetCount + tbDungeon.TodayCount;
                                    UpdateCurCount(id, i.EnterCount, i.TotalCount);
                                    i.CompleteCount = i.EnterCount.ToString();
                                    if (i.TotalCount > 0)
                                    {
                                        i.CompleteCount += "/" + i.TotalCount;
                                    }
                                    AnalyseNotice();
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnDungeonSweepRandAward(IEvent ievent)
    {
        var e = ievent as DungeonSweepRandAward;
        var choose = e.Index;
        var sweepData = DataModel.SweepData;
        var tbDraw = Table.GetDraw(mDrawId);
        sweepData.AwardItems[choose].ItemId = tbDraw.DropItem[mDrawIndex];
        sweepData.AwardItems[choose].Count = tbDraw.Count[mDrawIndex];

        var flag1 = 0;
        var flag2 = 0;
        for (var i = 0; i < 3; i++)
        {
            if (flag1 == choose)
            {
                flag1++;
            }
            if (flag2 == mDrawIndex)
            {
                flag2++;
            }
            var itemId = tbDraw.DropItem[flag2];
            sweepData.AwardItems[flag1].ItemId = itemId;
            sweepData.AwardItems[flag1].Count = tbDraw.Count[flag2];
            var tbItem = Table.GetItemBase(itemId);
            if (tbItem.Type >= 10000 && tbItem.Type <= 10099)
            {
                GameUtils.EquipRandomAttribute(sweepData.AwardItems[flag1]);
            }
            flag1++;
            flag2++;
        }
    }

    public void OnInitExData(IEvent ievent)
    {
        var playerData = PlayerDataManager.Instance;
        if (DataModel.MainInfos.Count == 0)
        {
            Table.ForeachPlotFuben(record =>
            {
                if (record.FubenType != (int) eDungeonType.Main)
                {
                    return true;
                }
                var mddata = new DungeonGroupDataModel();
                mddata.Id = record.Id;
                var fubenTable = Table.GetFuben(record.Difficulty[0]);
                mddata.IconId = fubenTable.FaceIcon;
                for (var i = 0; i < 3; i++)
                {
                    var info = mddata.Infos[i];
                    info.Id = record.Difficulty[i];

                    var tbFuben = Table.GetFuben(info.Id);
                    if (tbFuben != null)
                    {
                        for (int j = 0, jmax = tbFuben.DisplayCount.Count; j < jmax; ++j)
                        {
                            info.RewardCount[j] = tbFuben.DisplayCount[j];
                        }
                    }                    
                }

                DataModel.MainInfos.Add(mddata);
                return true;
            });
        }

        if (DataModel.TeamInfos.Count == 0)
        {
            Table.ForeachPlotFuben(record =>
            {
                if (record.FubenType != (int) eDungeonType.Team)
                {
                    return true;
                }
                var mddata = new DungeonGroupDataModel();
                mddata.Id = record.Id;
                for (var i = 0; i < 3; i++)
                {
                    var info = mddata.Infos[i];
                    info.Id = record.Difficulty[i];

                    var tbFuben = Table.GetFuben(info.Id);
                    if (tbFuben != null)
                    {
                        info.IsDynamicReward = tbFuben.IsDyncReward == 1;
                        if (!info.IsDynamicReward)
                        {
                            for (int j = 0, jmax = tbFuben.DisplayCount.Count; j < jmax; ++j)
                            {
                                info.RewardCount[j] = tbFuben.DisplayCount[j];
                            }
                        }
                    }

                }
                var tbFuben2 = Table.GetFuben(record.Difficulty[0]);
                if (tbFuben2 != null)
                {
                    var tbQueue = Table.GetQueue(tbFuben2.QueueParam);
                    if (tbQueue != null)
                    {
                        mddata.PlayerCount = tbQueue.CountLimit;
                    }
                }

                DataModel.TeamInfos.Add(mddata);
                return true;
            });
        }

        // vip副本
        if (DataModel.VipInfos.Count == 0)
        {
            Table.ForeachPlotFuben(record =>
            {
                if (record.FubenType != (int)eDungeonType.Vip)
                {
                    return true;
                }
                var mddata = new DungeonGroupDataModel();
                mddata.Id = record.Id;
                for (var i = 0; i < 3; i++)
                {
                    var info = mddata.Infos[i];
                    info.Id = record.Difficulty[i];

                    var tbFuben = Table.GetFuben(info.Id);
                    if (tbFuben != null)
                    {
                        for (int j = 0, jmax = tbFuben.DisplayCount.Count; j < jmax; ++j)
                        {
                            info.RewardCount[j] = tbFuben.DisplayCount[j];
                        }
                    }
                }
                var tbFuben2 = Table.GetFuben(record.Difficulty[0]);
                if (tbFuben2 != null)
                {
                    var tbCondition = Table.GetConditionTable(tbFuben2.EnterConditionId);
                    if (tbCondition.ItemId[0] == 15) //15号物品代表VIP等级
                    {
                        mddata.NeedVipLevel = tbCondition.ItemCountMin[0];
                    }
                }

                DataModel.VipInfos.Add(mddata);
                return true;
            });
        }




        {
            var __enumerator5 = (DataModel.MainInfos).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var info = __enumerator5.Current;
                {
                    {
                        var __enumerator19 = (info.Infos).GetEnumerator();
                        while (__enumerator19.MoveNext())
                        {
                            var i = __enumerator19.Current;
                            {
                                var id = i.Id;
                                if (id != -1)
                                {
                                    var tbDungeon = Table.GetFuben(id);
                                    if (tbDungeon != null)
                                    {
                                        i.EnterCount = playerData.GetExData(tbDungeon.TodayCountExdata);
                                        i.ResetCount = playerData.GetExData(tbDungeon.ResetExdata);
                                        i.TotalCount = i.ResetCount + tbDungeon.TodayCount;
                                        i.CompleteCount = i.EnterCount.ToString();
                                        if (i.TotalCount > 0)
                                        {
                                            i.CompleteCount += "/" + i.TotalCount;
                                        }
                                        i.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                                    }
                                    
                                }
                            }
                        }
                    }
                }
            }
        }


        {
            var __enumerator6 = (DataModel.TeamInfos).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var info = __enumerator6.Current;
                {
                    {
                        var __enumerator20 = (info.Infos).GetEnumerator();
                        while (__enumerator20.MoveNext())
                        {
                            var i = __enumerator20.Current;
                            {
                                var id = i.Id;
                                if (id != -1)
                                {
                                    var tbFuben = Table.GetFuben(id);
                                    if (tbFuben == null) continue;
                                    i.EnterCount = playerData.GetExData(tbFuben.TodayCountExdata);
                                    i.ResetCount = playerData.GetExData(tbFuben.ResetExdata);
                                    i.TotalCount = i.ResetCount + tbFuben.TodayCount;
                                    UpdateCurCount(id, i.EnterCount, i.TotalCount);
                                    i.CompleteCount = i.EnterCount.ToString();
                                    if (i.TotalCount > 0)
                                    {
                                        i.CompleteCount += "/" + i.TotalCount;
                                    }
                                    i.IsLock = playerData.CheckCondition(tbFuben.EnterConditionId) != 0;
                                    if (i.IsDynamicReward)
                                    {
                                        for (int j = 0, jmax = tbFuben.DisplayCount.Count; j < jmax; ++j)
                                        {
                                            var itemId = tbFuben.DisplayReward[j];
                                            var skillUpgradeId = tbFuben.DisplayCount[j];
                                            if (itemId == -1)
                                            {
                                                break;
                                            }
                                            var itemCount = Table.GetSkillUpgrading(skillUpgradeId)
                                                .GetSkillUpgradingValue(i.EnterCount + 1);
                                            i.RewardCount[j] = itemCount;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        //VIP
        foreach (var vipData in DataModel.VipInfos)
        {
            foreach (var info in vipData.Infos)
            {
                var id = info.Id;
                if (id != -1)
                {
                    var tbDungeon = Table.GetFuben(id);
                    info.EnterCount = playerData.GetExData(tbDungeon.TodayCountExdata);
                    info.ResetCount = playerData.GetExData(tbDungeon.ResetExdata);
                    info.TotalCount = info.ResetCount + tbDungeon.TodayCount;
                    info.CompleteCount = info.EnterCount.ToString();
                    UpdateCurCount(id, info.EnterCount, info.TotalCount);
                    if (info.TotalCount > 0)
                    {
                        info.CompleteCount += "/" + info.TotalCount;
                    }
                    info.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                }
            }
        }

        AnalyseNotice();
    }

    public void OnUpdateQuene(IEvent ievent)
    {
        RefreshQueueInfo();
    }

    private void RefleshUIState()
    {
        var queueId = PlayerDataManager.Instance.PlayerDataModel.QueueUpData.QueueId;
        if (queueId == -1)
        {
            DataModel.IsQueue = false;
            RefreshQueueInfo();
        }
    }

    public void RefreshDungeonState()
    {
        var sceneId = SceneManager.Instance.CurrentSceneTypeId;
        var selectInfo = DataModel.SelectDungeon.InfoData;

        var dungeonId = selectInfo.Id;

        var tbScene = Table.GetScene(sceneId);
        if (tbScene != null)
        {
            if (tbScene.FubenId == dungeonId)
            {
                selectInfo.State = 2;
                return;
            }
        }
        selectInfo.State = 0;
//         var lineupDungeon = GetQueneDungeonId();
//         {
//             // foreach(var group in DataModel.MainInfos)
//             var __enumerator26 = (DataModel.MainInfos).GetEnumerator();
//             while (__enumerator26.MoveNext())
//             {
//                 var group = __enumerator26.Current;
//                 {
//                     var state = 0;
//                     for (int i = 0; i < 3; i++)
//                     {
//                         var info = group.Infos[i];
//                         var tbDungeon = Table.GetFuben(info.Id);
//                         if (tbDungeon == null)
//                         {
//                             continue;
//                         }
//                         if (tbDungeon.SceneId == sceneId)
//                         {
//                             info.State = 1;
//                             state = 1;
//                         }
//                         else
//                         {
//                             info.State = 0;
//                         }
//                     }
//                     group.State = state;
//                 }
//             }
//         }
//         {
//             // foreach(var group in DataModel.TeamInfos)
//             var __enumerator27 = (DataModel.TeamInfos).GetEnumerator();
//             while (__enumerator27.MoveNext())
//             {
//                 var group = __enumerator27.Current;
//                 {
//                     bool isFight = false;
//                     bool isRank = false;
//                     for (int i = 0; i < 3; i++)
//                     {
//                         var info = group.Infos[i];
//                         var tbDungeon = Table.GetFuben(info.Id);
//                         if (tbDungeon == null)
//                         {
//                             continue;
//                         }
// 
//                         if (info.Id == lineupDungeon)
//                         {
//                             info.State = 2;
//                             isRank = true;
//                             continue;
//                         }
//                         if (tbDungeon.SceneId == sceneId)
//                         {
//                             info.State = 1;
//                             isFight = true;
//                             continue;
//                         }
//                         info.State = 0;
//                     }
// 
//                     if (isRank && isFight)
//                     {
//                         group.State = 3;
//                     }
//                     else if (isFight)
//                     {
//                         group.State = 1;
//                     }
//                     else if (isRank)
//                     {
//                         group.State = 2;
//                     }
//                     else
//                     {
//                         group.State = 0;
//                     }
//                 }
//             }
//         }
    }

    public void RefreshQueueInfo()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var dungeonId = data.Id;
        var tbDungeon = Table.GetFuben(dungeonId);
        if (tbDungeon == null)
        {
            return;
        }
        if (tbDungeon.QueueParam != -1 && tbDungeon.QueueParam == QueueUpData.QueueId)
        {
            DataModel.IsQueue = true;
        }
        else
        {
            DataModel.IsQueue = false;
        }

        RefreshDungeonState();
    }

    public IEnumerator ResetDungeonCoroutine(int dungeonId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ResetFuben(dungeonId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var e = new ShowUIHintBoard(436);
                    EventDispatcher.Instance.DispatchEvent(e);
                    DataModel.ResetCount++;
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenResetCountNotEnough
                         || msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....ResetMainDungeonCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....ResetMainDungeonCoroutine.......{0}.", msg.State);
            }
        }
    }

    public void ResetMainDungeon()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        var tbItemBase = Table.GetItemBase(tbDungeon.ResetItemId);

        if (DataModel.SelectDungeon.InfoData.IsLock)
        {
            return;
        }

        var tbVip = PlayerDataManager.Instance.TbVip;
        if (tbDungeon.TodayBuyCount + tbVip.PlotFubenResetCount <= data.ResetCount)
        {
            var oldCount = tbVip.PlotFubenResetCount;
            do
            {
                tbVip = Table.GetVIP(tbVip.Id + 1);
            } while (tbVip != null && oldCount >= tbVip.PlotFubenResetCount);

            if (tbVip == null)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(437));
            }
            else
            {
                GameUtils.GuideToBuyVip(tbVip.Id);
            }
            return;
        }


        //重置一次副本次数需要消耗{0}×{1}，是否继续?
        var content = string.Format(GameUtils.GetDictionaryText(463), tbItemBase.Name, tbDungeon.ResetItemCount);
	    var call = new Action(() =>
	    {
			ResetMainDungeonOk(tbDungeon.ResetItemId, tbDungeon.ResetItemCount);
	    });
		UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, content, "", call);
    }

	public void ResetMainDungeonOk(int item,int needCount)
    {
		var count = PlayerDataManager.Instance.GetItemCount(item);
		if (count < needCount)
		{
			EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
			return;
		}

        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        NetManager.Instance.StartCoroutine(ResetDungeonCoroutine(id));
    }

    public void ResetTeamDungeon()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        if (tbDungeon.TodayBuyCount <= data.ResetCount)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(437));
            return;
        }
        var count = PlayerDataManager.Instance.GetItemCount(tbDungeon.ResetItemId);
        if (count < tbDungeon.ResetItemCount)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(437));
            return;
        }
        var tbItemBase = Table.GetItemBase(tbDungeon.ResetItemId);
        //重置一次副本次数需要消耗{0}×{1}，是否继续?
        var content = string.Format(GameUtils.GetDictionaryText(463), tbItemBase.Name, tbDungeon.ResetItemCount);
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, content, "", ResetTeamDungeonOk);
    }

    public void ResetTeamDungeonOk()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;

        NetManager.Instance.StartCoroutine(ResetDungeonCoroutine(id));
    }

    //--------------------------------------------------------------------Main-----------------
    private void SelectMainDungeonGroup(int index, int subIndex = 0)
    {
        var data = DataModel.MainInfos[index];
        var count = DataModel.MainInfos.Count;
        for (var j = 0; j < count; j++)
        {
            var info = DataModel.MainInfos[j];
            if (j == index)
            {
                info.IsSelect = true;
            }
            else
            {
                if (info.IsSelect)
                {
                    info.IsSelect = false;
                }
            }
        }

        DataModel.SelectDungeon.GroupData = data;
        var playerData = PlayerDataManager.Instance;
        {
            var __enumerator7 = (data.Infos).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var t = __enumerator7.Current;
                {
                    var id = t.Id;
                    if (id != -1)
                    {
                        var tbDungeon = Table.GetFuben(id);
                        t.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                    }
                }
            }
        }
        SelectMainDungeonInfo(subIndex);
        var e1 = new DungeonNetRetCallBack(11);
        EventDispatcher.Instance.DispatchEvent(e1);
    }

    public void SelectMainDungeonInfo(int i)
    {
        var data = DataModel.SelectDungeon.GroupData;
        DataModel.SelectDungeon.InfoData = data.Infos[i];
        DataModel.SelectDungeon.Index = i;
        DataModel.SelectDungeon.IsDiffictLevelShow = 1;

        var id = data.Infos[i].Id;
        var tbDungeon = Table.GetFuben(id);
        if (tbDungeon == null)
        {
            return;
        }
        var time = PlayerDataManager.Instance.GetExData(tbDungeon.TimeExdata);
        if (time <= 0)
        {
            DataModel.MainTime = GameUtils.GetDictionaryText(460);
        }
        else
        {
            DataModel.MainTime = GameUtils.TimeStringMS(time/60, time%60);
        }
        var tbVip = PlayerDataManager.Instance.TbVip;
        DataModel.ResetCount = DataModel.SelectDungeon.GroupData.Infos[i].ResetCount;
        DataModel.ResetMaxCount = tbDungeon.TodayBuyCount + tbVip.PlotFubenResetCount;
        data.NeedLevel = Table.GetPlotFuben(data.Id).OpenLevel[i];
        DataModel.ScanCount = PlayerDataManager.Instance.GetItemCount(DataModel.ScanItemId);
        DataModel.ScanGrey = SweepCondition() ? 0 : 1;
        RefreshDungeonState();
    }

    //----------------------------------------------------------------------Team-----------------
    private void SelectTeamDungeonGroup(int index, int subIndex = 0)
    {
        var data = DataModel.TeamInfos[index];

        var count = DataModel.TeamInfos.Count;
        for (var j = 0; j < count; j++)
        {
            var info = DataModel.TeamInfos[j];
            if (j == index)
            {
                info.IsSelect = true;
            }
            else
            {
                if (info.IsSelect)
                {
                    info.IsSelect = false;
                }
            }
        }

        DataModel.SelectDungeon.GroupData = data;
        var playerData = PlayerDataManager.Instance;
        {
            var __enumerator8 = (data.Infos).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var t = __enumerator8.Current;
                {
                    var id = t.Id;
                    if (id != -1)
                    {
                        var tbDungeon = Table.GetFuben(id);
                        t.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                    }
                }
            }
        }
        SelectTeamDungeonInfo(subIndex);
        var e1 = new DungeonNetRetCallBack(12);
        EventDispatcher.Instance.DispatchEvent(e1);
    }

    public void SelectTeamDungeonInfo(int i)
    {
        var data = DataModel.SelectDungeon.GroupData;
        DataModel.SelectDungeon.InfoData = data.Infos[i];
        DataModel.SelectDungeon.Index = i;
        DataModel.SelectDungeon.IsDiffictLevelShow = 1;
        data.NeedLevel = Table.GetPlotFuben(data.Id).OpenLevel[i];

        RefreshQueueInfo();
    }

    //----------------------------------------------------------------------VIP------------------
    public void SelectVipDungeonGroup(int index, int subIndex = 0)
    {
        var data = DataModel.VipInfos[index];
        var count = DataModel.VipInfos.Count;
        for (var j = 0; j < count; j++)
        {
            var info = DataModel.VipInfos[j];
            if (j == index)
            {
                info.IsSelect = true;
            }
            else
            {
                if (info.IsSelect)
                {
                    info.IsSelect = false;
                }
            }
        }

        DataModel.SelectDungeon.GroupData = data;
        var playerData = PlayerDataManager.Instance;
        {
            var __enumerator7 = (data.Infos).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var t = __enumerator7.Current;
                {
                    var id = t.Id;
                    if (id != -1)
                    {
                        var tbDungeon = Table.GetFuben(id);
                        t.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                    }
                }
            }
        }
        SelectVipDungeonInfo(subIndex);
        var e1 = new DungeonNetRetCallBack(13);
        EventDispatcher.Instance.DispatchEvent(e1);
    }

    public void SelectVipDungeonInfo(int i)
    {
        var data = DataModel.SelectDungeon.GroupData;
        DataModel.SelectDungeon.InfoData = data.Infos[i];
        DataModel.SelectDungeon.Index = i;
        DataModel.SelectDungeon.IsDiffictLevelShow = 0;

        var id = data.Infos[i].Id;
        var tbDungeon = Table.GetFuben(id);
        if (tbDungeon == null)
        {
            return;
        }
        var time = PlayerDataManager.Instance.GetExData(tbDungeon.TimeExdata);
        if (time <= 0)
        {
            DataModel.MainTime = GameUtils.GetDictionaryText(460);
        }
        else
        {
            DataModel.MainTime = GameUtils.TimeStringMS(time / 60, time % 60);
        }
        var tbVip = PlayerDataManager.Instance.TbVip;
        DataModel.ResetCount = DataModel.SelectDungeon.GroupData.Infos[i].ResetCount;
        DataModel.ResetMaxCount = tbDungeon.TodayBuyCount + tbVip.PlotFubenResetCount;
        data.NeedLevel = Table.GetPlotFuben(data.Id).OpenLevel[i];
        DataModel.ScanCount = PlayerDataManager.Instance.GetItemCount(DataModel.ScanItemId);
        DataModel.ScanGrey = SweepCondition() ? 0 : 1;

        RefreshDungeonState();
    }

    private void SetShowScan(IEvent ievent)
    {
        var e = ievent as DungeonSetScan;
        DataModel.IsShowScan = e.ShowScan;
    }

    private void ExitFuBenWithOutMessageBox(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ExitDungeonCoroutine());
    }

    private bool SweepCondition()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        var playerData = PlayerDataManager.Instance;

        var dic = playerData.CheckCondition(tbDungeon.EnterConditionId);
        if (dic != 0)
        {
            //不符合副本扫荡条件   270233
            return false;
        }
        if (tbDungeon.TimeExdata == -1)
        {
            return false;
        }
        var time = playerData.GetExData(tbDungeon.TimeExdata);
        if (time == 0 || time > tbDungeon.SweepLimitMinutes*60)
        {
            return false;
        }
        if (DataModel.ScanCount <= 0)
        {
            return false;
        }
        return true;
    }

    public void SweepMainDungeon()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var id = data.Id;
        var tbDungeon = Table.GetFuben(id);
        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        if (sceneId == tbDungeon.SceneId)
        {
            //已经在此副本当中了
            var e = new ShowUIHintBoard(270081);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var playerData = PlayerDataManager.Instance;
        var dic = playerData.CheckCondition(tbDungeon.EnterConditionId);
        if (dic != 0)
        {
            //不符合副本扫荡条件   270233
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dic));
            return;
        }
        if (tbDungeon.TimeExdata == -1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(464));
            return;
        }
        var time = playerData.GetExData(tbDungeon.TimeExdata);
        if (time == 0 || time > tbDungeon.SweepLimitMinutes*60)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(435));
            return;
        }
        if (data.EnterCount >= data.TotalCount)
        {
            if (data.ResetCount < tbDungeon.TodayBuyCount)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(434));
            }
            else
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(438));
            }
            return;
        }
        if (data.State == 1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270220));
            return;
        }

        var tbDungeonNeedItemIdLength0 = tbDungeon.NeedItemId.Count;
        for (var i = 0; i < tbDungeonNeedItemIdLength0; i++)
        {
            if (tbDungeon.NeedItemId[i] != -1)
            {
                if (playerData.GetItemCount(tbDungeon.NeedItemId[i]) < tbDungeon.NeedItemCount[i])
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210101));
                    return;
                }
            }
        }
        if (playerData.GetItemCount(GameUtils.SweepCouponId) < 1)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200005024));
            PlayerDataManager.Instance.ShowItemInfoGet(GameUtils.SweepCouponId);
            return;
        }
        NetManager.Instance.StartCoroutine(SweepMainDungeonCoroutine());
    }

    public IEnumerator SweepMainDungeonCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var data = DataModel.SelectDungeon.InfoData;
            var id = data.Id;
            var msg = NetManager.Instance.SweepFuben(id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //                     if (msg.ErrorCode == (int)ErrorCodes.Error_ItemNoInBag_All)
                    //                     {
                    //                         var e1 = new ShowUIHintBoard(445);
                    //                         EventDispatcher.Instance.DispatchEvent(e1);    
                    //                     }
                    DataModel.ScanCount -= 1;
                    DataModel.ScanGrey = SweepCondition() ? 0 : 1;
                    DataModel.SweepData.ItemInfos.Clear();
                    DataModel.SweepData.Gold = 0;
                    DataModel.SweepData.Exp = 0;
                    {
                        var __list13 = msg.Response.Items;
                        var __listCount13 = __list13.Count;
                        for (var __i13 = 0; __i13 < __listCount13; ++__i13)
                        {
                            var i = __list13[__i13];
                            {
                                if (i.ItemId == -1)
                                {
                                    continue;
                                }
                                if (i.ItemId == 2)
                                {
                                    DataModel.SweepData.Gold = i.Count;
                                }
                                else if (i.ItemId == 1)
                                {
                                    DataModel.SweepData.Exp = i.Count;
                                }
                                else
                                {
                                    var idData = new BagItemDataModel
                                    {
                                        ItemId = i.ItemId,
                                        Count = i.Count
                                    };
                                    idData.Exdata.InstallData(i.Exdata);
                                    DataModel.SweepData.ItemInfos.Add(idData);
                                }
                            }
                        }
                    }
                    var awardModel = new BagItemDataModel();
                    mDrawId = msg.Response.DrawId;
                    mDrawIndex = msg.Response.SelectIndex;
                    DataModel.SweepData.AwardItems[mDrawIndex].ItemId = msg.Response.DrawItem.ItemId;
                    DataModel.SweepData.AwardItems[mDrawIndex].Count = msg.Response.DrawItem.Count;
                    DataModel.SweepData.AwardItems[mDrawIndex].Exdata.InstallData(msg.Response.DrawItem.Exdata);
                    DataModel.IsShowScan = 1;
                    var e = new DungeonNetRetCallBack(10);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenID
                         || msg.ErrorCode == (int) ErrorCodes.Error_FubenNoPass
                         || msg.ErrorCode == (int) ErrorCodes.Error_PassFubenTimeNotEnough
                         || msg.ErrorCode == (int) ErrorCodes.Error_LevelNoEnough
                         || msg.ErrorCode == (int) ErrorCodes.ItemNotEnough
                         || msg.ErrorCode == (int) ErrorCodes.Error_FubenCountNotEnough
                         || msg.ErrorCode == (int) ErrorCodes.Error_ItemNoInBag_All)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....ResetMainDungeonCoroutine.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....ResetMainDungeonCoroutine.......{0}.", msg.State);
            }
        }
    }

    public void TeamDungeonLineup()
    {
        var data = DataModel.SelectDungeon.InfoData;
        var dungeonId = data.Id;
        if (DataModel.IsQueue)
        {
            NetManager.Instance.StartCoroutine(TeamDungeonLineupCancelCoroutine());
        }
        else
        {
            if (PlayerDataManager.Instance.IsInPvPScnen())
            {
                GameUtils.ShowHintTip(456);
                return;
            }

            var tbDungeon = Table.GetFuben(dungeonId);
            var sceneId = GameLogic.Instance.Scene.SceneTypeId;
            if (sceneId == tbDungeon.SceneId)
            {
                ////已经在此副本当中了
                var e = new ShowUIHintBoard(270081);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
            var playerData = PlayerDataManager.Instance;
            var dicCom = playerData.CheckCondition(tbDungeon.EnterConditionId);
            if (dicCom != 0)
            {
                //不符合副本进入条件 270234
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicCom));
                return;
            }
            if (data.EnterCount == data.TotalCount)
            {
                if (data.ResetCount < tbDungeon.TodayBuyCount)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(434));
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(438));
                }
                return;
            }

            var dic = PlayerDataManager.Instance.CheckCondition(tbDungeon.EnterConditionId);
            if (dic != 0)
            {
                //不符合副本扫荡条件
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dic));
                return;
            }

            //if (tbDungeon.FightPoint > PlayerDataManager.Instance.PlayerDataModel.Attributes.FightValue)
            //{
            //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(444));
            //    return;
            //}

            var teamData = UIManager.Instance.GetController(UIConfig.TeamFrame).GetDataModel("") as TeamDataModel;
            var teamCount = teamData.TeamList.Count;
            var memberCount = 0;
            for (var i = 0; i < teamCount; i++)
            {
                if (teamData.TeamList[i].Guid != 0ul)
                {
                    memberCount++;
                }
            }
            if (memberCount > 0)
            {
                var tbQuenu = Table.GetQueue(tbDungeon.QueueParam);
                if (memberCount > tbQuenu.CountLimit)
                {
                    //var e = new ShowUIHintBoard("队伍人数超出副本所需人数上限");
                    var e = new ShowUIHintBoard(465);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
            }

            //如果在排其它的队
            if (QueueUpData.QueueId != -1)
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 41004, "", () =>
                {
                    //EventDispatcher.Instance.DispatchEvent(new UIEvent_CloseDungeonQueue(1));
                    NetManager.Instance.StartCoroutine(TeamDungeonLineupCoroutine());
                });
                return;
            }

            //NetManager.Instance.StartCoroutine(TeamDungeonLineupCoroutine());
            NoticeEnterDungeonFight(tbDungeon.FightPoint,
                () =>
                {
                    NetManager.Instance.StartCoroutine(TeamDungeonLineupCoroutine());
                }
            );
        }
    }

    public IEnumerator TeamDungeonLineupCancelCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var data = DataModel.SelectDungeon.InfoData;
            var dungeonId = data.Id;
            var tbDungeon = Table.GetFuben(dungeonId);
            var msg = NetManager.Instance.MatchingCancel(tbDungeon.QueueParam);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    QueueUpData.QueueId = -1;
                    RefreshQueueInfo();
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(Game.Instance.ServerTime,
                        -1));
                    EventDispatcher.Instance.DispatchEvent(new QueueCanceledEvent());
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_QueueCountMax)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveQueue)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....MatchingCancel.......{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....MatchingCancel.......{0}.", msg.State);
            }
        }
    }

    public IEnumerator TeamDungeonLineupCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var data = DataModel.SelectDungeon.InfoData;
            var dungeonId = data.Id;
            var tbDungeon = Table.GetFuben(dungeonId);

            var msg = NetManager.Instance.MatchingStart(tbDungeon.QueueParam);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.InitQueneData(msg.Response.Info);
                    RefreshQueueInfo();
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterNotLeader)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenID)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_QueueCountMax)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_CharacterHaveQueue)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Unline)
                {
                    //有队友不在线
                    var e = new ShowUIHintBoard(448);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenCountNotEnough)
                {
                    //{0}副本次数不够
                    var charId = msg.Response.CharacterId[0];
                    var name = PlayerDataManager.Instance.GetTeamMemberName(charId);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var str = GameUtils.GetDictionaryText(466);
                        str = string.Format(str, name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                {
                    //{{0}道具不足
                    var charId = msg.Response.CharacterId[0];
                    var name = PlayerDataManager.Instance.GetTeamMemberName(charId);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var str = GameUtils.GetDictionaryText(467);
                        str = string.Format(str, name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_LevelNoEnough)
                {
                    //{{0}不符合副本条件
                    var charId = msg.Response.CharacterId[0];
                    var name = PlayerDataManager.Instance.GetTeamMemberName(charId);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var str = GameUtils.GetDictionaryText(468);
                        str = string.Format(str, name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenID
                         || msg.ErrorCode == (int) ErrorCodes.Error_FubenNotInOpenTime)
                {
                    var e = new ShowUIHintBoard(msg.ErrorCode + 200000000);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....MatchingStart.......{0}........", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Warn(".....MatchingStart.......{0}.", msg.State);
            }
        }
    }

    public void CleanUp()
    {
        DataModel = new DungeonDataModel();
        DataModel.ScanItemId = 22053;
    }

    public void Close()
    {
    }

    public void Tick()
    {
    }

    public void RefreshData(UIInitArguments data)
    {
        DataModel.IsShowScan = 0;
        var playerData = PlayerDataManager.Instance;
        {
            // foreach(var info in DataModel.MainInfos)
            var __enumerator9 = (DataModel.MainInfos).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var info = __enumerator9.Current;
                {
                    {
                        // foreach(var i in info.Infos)
                        var __enumerator21 = (info.Infos).GetEnumerator();
                        while (__enumerator21.MoveNext())
                        {
                            var i = __enumerator21.Current;
                            {
                                var id = i.Id;
                                if (id != -1)
                                {
                                    var tbDungeon = Table.GetFuben(id);
                                    i.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                                }
                            }
                        }
                    }
                }
            }
        }
        {
            // foreach(var info in DataModel.TeamInfos)
            var __enumerator10 = (DataModel.TeamInfos).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var info = __enumerator10.Current;
                {
                    {
                        // foreach(var i in info.Infos)
                        var __enumerator22 = (info.Infos).GetEnumerator();
                        while (__enumerator22.MoveNext())
                        {
                            var i = __enumerator22.Current;
                            {
                                var id = i.Id;
                                if (id != -1)
                                {
                                    var tbDungeon = Table.GetFuben(id);
                                    i.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        // vip
        {
            foreach (var vipData in DataModel.VipInfos)
            {
                foreach (var info in vipData.Infos)
                {
                    var i = info;
                    {
                        var id = i.Id;
                        if (id != -1)
                        {
                            var tbDungeon = Table.GetFuben(id);
                            i.IsLock = playerData.CheckCondition(tbDungeon.EnterConditionId) != 0;
                        }
                    }
                }
            }
        }
            


        var arg = data as DungeonArguments;
        if (arg != null)
        {
            var dungeondId = arg.Tab;
            var tbFuben = Table.GetPlotFuben(dungeondId);
            var diffcult = 0;
            if (arg.Args != null && arg.Args.Count > 0)
            {
                diffcult = arg.Args[0];
                if (diffcult < 0 || diffcult > 3)
                {
                    diffcult = 0;
                }
            }
            if (tbFuben != null)
            {
                if (tbFuben.FubenType == (int)eDungeonType.Main)
                {
                    DataModel.ToggleSelect = 0;
                    var index = 0;
                    var count = DataModel.MainInfos.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var d = DataModel.MainInfos[i];
                        if (d.Id == dungeondId)
                        {
                            index = i;
                            break;
                        }
                    }
                    SelectMainDungeonGroup(index, diffcult);
                }
                else if (tbFuben.FubenType == (int)eDungeonType.Team)
                {
                    DataModel.ToggleSelect = 1;
                    var index = 0;
                    var count = DataModel.TeamInfos.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var d = DataModel.TeamInfos[i];
                        if (d.Id == dungeondId)
                        {
                            index = i;
                            break;
                        }
                    }
                    SelectTeamDungeonGroup(index, diffcult);
                }
                else if (tbFuben.FubenType == (int)eDungeonType.Vip)
                {
                    DataModel.ToggleSelect = 2;
                    var index = 0;
                    var count = DataModel.VipInfos.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var d = DataModel.VipInfos[i];
                        if (d.Id == dungeondId)
                        {
                            index = i;
                            break;
                        }
                    }
                    SelectVipDungeonGroup(index, diffcult);
                }
                return;
            }
        }
        DataModel.ToggleSelect = 0;
        SelectMainDungeonGroup(0);
        RefleshUIState();
        AnalyseNotice();
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public void OnChangeScene(int sceneId)
    {
        if (sceneId == -1)
        {
            return;
        }
        RefreshQueueInfo();
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        return null;
    }

    public void OnShow()
    {
        DataModel.IsShowScan = 0;
    }

    public FrameState State { get; set; }
}