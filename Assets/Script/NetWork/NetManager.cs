#region using

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;
using UnityEngine;

#endregion

public class DirectoryNetwork : ClientAgentBase, IDirectory9xServiceInterface
{
    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        this.Init();
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public override bool OnMessageTimeout(OutMessage message)
    {
        return true;
    }

    public override IEnumerator OnServerLost()
    {
        yield break;
    }
}

public partial class NetManager : ClientAgentBase, ILogin9xServiceInterface, ILogic9xServiceInterface,
                                  IScene9xServiceInterface, IRank9xServiceInterface, IActivity9xServiceInterface,
                                  IChat9xServiceInterface, ITeam9xServiceInterface
{
    static NetManager()
    {
        Instance = null;
    }

    private object ZeroBattleReplyTrigger;
    private object ZeroReplyTrigger;
    public static NetManager Instance { get; private set; }

    public void AnalyzeIpAddress(Stream stream)
    {
        try
        {
            var reader = new StreamReader(stream, Encoding.GetEncoding("gb2312"));
            var read = reader.ReadToEnd();

            Logger.Info("---AnalyzeIpAddress --- Stream---{0}", read);

            var m = Regex.Match(read, "city:\"(.*?)\"");
            var city = m.Groups[1].Captures[0].Value;

            m = Regex.Match(read, "province:\"(.*?)\"");
            var province = m.Groups[1].Captures[0].Value;


            var e = new IpAddressSet(province, city);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        catch (Exception)
        {
            var e = new IpAddressSet("", "");
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    private void Awake()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        ((ILogin9xServiceInterface) this).Init();
        ((ILogic9xServiceInterface) this).Init();
        ((IScene9xServiceInterface) this).Init();
        ((IActivity9xServiceInterface) this).Init();
        ((IRank9xServiceInterface) this).Init();
        ((IChat9xServiceInterface) this).Init();
        ((ITeam9xServiceInterface) this).Init();
        Instance = this;

        NeedReconnet = false;
        IsReconnecting = false;
        EventDispatcher.Instance.AddEventListener(UIEvent_DeviceInfo_NetWorkStateChange.EVENT_TYPE, NetWorkStateChanged);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public Coroutine ConnectToGate(AsyncResult<int> result)
    {
        Logger.Debug("Try to connect [" + ServerAddress + "]");
        return StartCoroutine(ConnectCoroutine(result));
    }

    public IEnumerator LogicP1vP1FightResultCoroutine(P1vP1RewardData data)
    {
        var delay = 0.0f;
        if (data.Result == 1)
        {
            delay = GameUtils.DungeonShowDelay/1000.0f;
        }
        yield return new WaitForSeconds(delay);

        //当前不是pvp场景 就不不弹结果对话框了
        var tbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
        if (tbScene == null || tbScene.FubenId < 0)
        {
            yield break;
        }
        var tbFuben = Table.GetFuben(tbScene.FubenId);
        if (tbFuben.AssistType != (int) eDungeonAssistType.Pvp1v1)
        {
            yield break;
        }
        var e = new Show_UI_Event(UIConfig.AreanaResult, new ArenaResultArguments {RewardData = data});
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void NetWorkStateChanged(IEvent ievent)
    {
        var status = (NetworkStatus) PlatformHelper.GetNetworkState();
        if (status == NetworkStatus.ReachableViaWWAN)
        {
            PrepareForReconnect();
        }
    }

    public void NotifyQueueIndex(int index)
    {
        PlayerDataManager.Instance.AccountDataModel.LineUpIndex = index;
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(UIEvent_DeviceInfo_NetWorkStateChange.EVENT_TYPE, NetWorkStateChanged);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    public void OnReconnectFail()
    {
        //连接失败，是否重新连接
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 307, "",
            () =>
            {
                IsReconnecting = false;
                StartCoroutine(OnServerLost());
            }
            , () =>
            {
                IsReconnecting = false;
                Game.Instance.ExitToLogin();
            }, true);
    }

    private void QueueSuccess(QueueSuccessData data)
    {
        PlayerDataManager.Instance.AccountDataModel.LineUpShow = false;
        PlayerDataManager.Instance.AccountDataModel.LineUpIndex = 0;
        var type = (QueueType) data.Type;
        switch (type)
        {
            case QueueType.Login:
                Instance.StartCoroutine(LoginWindow.LoginSuccess());
                break;
            case QueueType.EnterGame:
                CallEnterGame(PlayerDataManager.Instance.Guid);
                break;
        }
    }

    public void SceneLinkLogic(ulong chararcterId)
    {
    }

    public void ShowDictionaryTip(int dicId, bool addChat)
    {
        var strDic = GameUtils.GetDictionaryText(dicId);
        ShowDictionaryTip(strDic, addChat);
    }

    public void ShowDictionaryTip(string strDic, bool addChat)
    {
        var e = new ShowUIHintBoard(strDic);
        EventDispatcher.Instance.DispatchEvent(e);

        if (addChat)
        {
            var e1 = new ChatMainHelpMeesage(strDic);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
    }

    private IEnumerator ShowReliveCoroutine(string killName)
    {
        yield return new WaitForSeconds(2.8f);

        var content = "";
        var tempKillName = "";
        if (killName == PlayerDataManager.Instance.GetName())
        {
            content = GameUtils.GetDictionaryText(242013);
        }
        else
        {
            var str = GameUtils.GetDictionaryText(242014);
            content = string.Format(str, killName);
            tempKillName = killName;
        }

        var e = new RefreshReliveInfoEvent(tempKillName);
        EventDispatcher.Instance.DispatchEvent(e);

        GameUtils.ShowHintTip(content);
		var chat = new ChatMessageDataModel
		{
			Type = (int)eChatChannel.Scene,
			CharId = 0,
			Content = content
		};
		EventDispatcher.Instance.DispatchEvent(new Event_PushMessage(chat));
    }

    public IEnumerator WaitAndDoQueueSuccess(QueueSuccessData data)
    {
        yield return new WaitForSeconds(1.5f);
        QueueSuccess(data);
    }

    public void SyncMission(int missionId, int state, int param)
    {
        throw new NotImplementedException();
    }

    public void SyncFlag(int flagId, int param)
    {
        PlayerDataManager.Instance.SetFlag(flagId, param == 1);
    }

    public void SyncFlagList(Int32Array trueList, Int32Array falseList)
    {
        {
            var __list1 = trueList.Items;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var i = __list1[__i1];
                {
                    PlayerDataManager.Instance.SetFlag(i, true);
                }
            }
        }
        {
            var __list2 = falseList.Items;
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var i = __list2[__i2];
                {
                    PlayerDataManager.Instance.SetFlag(i, false);
                }
            }
        }
    }

    public void SyncExdata(int exdataId, int value)
    {
        PlayerDataManager.Instance.SetExData(exdataId, value);
    }

    public void SyncExdataList(Dict_int_int_Data diff)
    {
        {
            // foreach(var i in diff.Data)
            var __enumerator3 = (diff.Data).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var i = __enumerator3.Current;
                {
                    PlayerDataManager.Instance.SetExData(i.Key, i.Value);
                }
            }
        }
    }

    public void SyncExdata64(int exdataId, long value)
    {
        PlayerDataManager.Instance.SetExData64(exdataId, value);
    }

    public void SyncResources(int resId, int value)
    {
        throw new NotImplementedException();
    }

    public void SyncItems(BagsChangeData bag)
    {
        var bags = PlayerDataManager.Instance.PlayerDataModel.Bags.Bags;
        var bagEvent = new UIEvent_BagChange();
        PlayerDataManager.Instance.BagItemCountChange(bag);
        {
            // foreach(var changeData in bag.BagsChange)
            var __enumerator4 = (bag.BagsChange).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var changeData = __enumerator4.Current;
                {
                    var bagid = changeData.Key;
                    if (bagid == (int) eBagType.Wing)
                    {
                        var wingCon = UIManager.Instance.GetController(UIConfig.WingUI);
                        if (wingCon != null)
                        {
                            wingCon.CallFromOtherClass("UpdateWingItem", new[] {changeData.Value});
                        }
                        continue;
                    }
                    if (bagid == (int) eBagType.Elf)
                    {
                        var elfCon = UIManager.Instance.GetController(UIConfig.ElfUI);
                        if (elfCon != null)
                        {
                            elfCon.CallFromOtherClass("UpdateElfBag", new[] {changeData.Value});
                        }
                        continue;
                    }
                    if (bagid >= (int) eBagType.Equip01 && bagid <= (int) eBagType.Equip12)
                    {
                        PlayerDataManager.Instance.UpdateEquipData(bagid, changeData.Value);
                        continue;
                    }
                    if (bagid == (int) eBagType.MedalBag || bagid == (int) eBagType.MedalUsed ||
                        bagid == (int) eBagType.MedalTemp)
                    {
                        var MedalCon = UIManager.Instance.GetController(UIConfig.SailingUI);
                        if (MedalCon != null)
                        {
                            MedalCon.CallFromOtherClass("UpdateMedalBag", new object[] {bagid, changeData.Value});
                        }
                        if (bagid == (int) eBagType.MedalUsed)
                        {
                            PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Medal);
                        }
                        continue;
                    }
                    if (bagid == (int) eBagType.WishingPool)
                    {
                        var MedalCon = UIManager.Instance.GetController(UIConfig.WishingUI);
                        if (MedalCon != null)
                        {
                            MedalCon.CallFromOtherClass("UpdateWishingBag", new object[] {bagid, changeData.Value});
                        }
                        continue;
                    }
                    if (bagid == (int) eBagType.Piece)
                    {
                        var HandbookCon = UIManager.Instance.GetController(UIConfig.HandBook);
                        if (null != HandbookCon)
                        {
                            HandbookCon.CallFromOtherClass("RefreshCount", null);
                        }
                        continue;
                    }
                    if (bagid == (int) eBagType.GemBag || bagid == (int) eBagType.GemEquip)
                    {
                        var AstrCon = UIManager.Instance.GetController(UIConfig.AstrologyUI);
                        if (null != AstrCon)
                        {
                            AstrCon.CallFromOtherClass("UpdateAstrologyData", new object[] {bagid, changeData.Value});
                        }
                        continue;
                    }
                    if (bagid == (int) eBagType.Pet)
                    {
                        {
                            var __enumerator5 = (changeData.Value.ItemsChange).GetEnumerator();
                            while (__enumerator5.MoveNext())
                            {
                                var item = __enumerator5.Current;
                                {
                                    EventDispatcher.Instance.DispatchEvent(new UIEvent_PetChangeEvent(item.Key));
                                }
                            }
                        }
                        continue;
                    }
                    bagEvent.AddType(bagid);
                }
            }
        }
        EventDispatcher.Instance.DispatchEvent(bagEvent);
    }

    public void LogicP1vP1FightResult(P1vP1RewardData data)
    {
        var one = new P1vP1Change_One();
        one.Type = 0;
        one.NewRank = data.NewRank;
        one.OldRank = data.OldRank;
        one.Name = data.OpponentName;
        var e = new ArenaFightRecoardChange(one);
        EventDispatcher.Instance.DispatchEvent(e);
        StartCoroutine(LogicP1vP1FightResultCoroutine(data));
    }

    public void NotifyDungeonTime(int state, ulong time)
    {
        PlayerDataManager.Instance.PlayerDataModel.DungeonState = state;
        EventDispatcher.Instance.DispatchEvent(new NotifyDungeonTime(Extension.FromServerBinary((long) time).ToBinary()));
    }

    public void SyncCharacterPostion(ulong characterId, PositionData pos)
    {
        var character = ObjManager.Instance.FindCharacterById(characterId);
        if (null == character)
        {
            return;
        }
        var loc = GameLogic.GetTerrainPosition(GameUtils.DividePrecision(pos.Pos.x),
            GameUtils.DividePrecision(pos.Pos.y));
        character.Position = loc;
        character.TargetDirection = new Vector3(GameUtils.DividePrecision(pos.Dir.x), 0,
            GameUtils.DividePrecision(pos.Dir.y));
        character.StopMove();
        if (character.GetObjType() == OBJ.TYPE.MYPLAYER)
        {
            (character as ObjMyPlayer).AdjustHeightPosition();
            var e = new Postion_Change_Event(character.Position);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var navAgent = character.GetNavMeshAgent();
            if (null != navAgent)
            {
                navAgent.Warp(loc);
            }
        }
    }

    public void NotifyCountdown(ulong time, int type)
    {
        var e = new ShowCountdownEvent(Extension.FromServerBinary((long) time), (eCountdownType) type);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void SyncTeamEnterFuben(int fubenId)
    {
        //"是否现在进入：{0}"
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
            string.Format(GameUtils.GetDictionaryText(270012), Table.GetFuben(fubenId).Name), "",
            () =>
            {
                var e = new DungeonBtnClick(4, eDungeonType.Team, fubenId);
                EventDispatcher.Instance.DispatchEvent(e);
            },
            () =>
            {
                var e = new DungeonBtnClick(5, eDungeonType.Team, fubenId);
                EventDispatcher.Instance.DispatchEvent(e);
            }, false, true);
    }

    public void SendMatchingMessage(int NowCount)
    {
    }

    public void TalentCountChange(int talentId, int value)
    {
        EventDispatcher.Instance.DispatchEvent(new UIEvent_SkillFrame_NetSyncTalentCount(talentId, value));
    }

    public void Discard0(PlayerLoginData plData)
    {
        //do nothing
    }

    public void NotifyQueueSuccess(QueueSuccessData data)
    {
#if UNITY_EDITOR
        Instance.StartCoroutine(WaitAndDoQueueSuccess(data));
#else
        QueueSuccess(data);
#endif
    }

    public void NotifyMessage(int type, string info, int addChat)
    {
        switch ((eSceneNotifyType) type)
        {
            case eSceneNotifyType.Dictionary:
            {
                var dicId = info.ToInt();
                if (dicId == -1)
                {
                    ShowDictionaryTip(info, addChat == 1);
                }
                else
                {
                    ShowDictionaryTip(dicId, addChat == 1);
                }
            }
                break;
            case eSceneNotifyType.DictionaryWrap:
            {
                info = GameUtils.ConvertChatContent(info);
                ShowDictionaryTip(info, addChat == 1);
            }
                break;
            case eSceneNotifyType.Die:
            {
                var tbScene = Table.GetScene(SceneManager.Instance.CurrentSceneTypeId);
                if (tbScene != null && tbScene.Type != (int)eSceneType.Pvp)
                {
                    StartCoroutine(ShowReliveCoroutine(info));
                }
            }
                break;
        }
    }

    public void LogicNotifyMessage(int type, string info, int addChat)
    {
        switch ((eLogicNotifyType) type)
        {
            case eLogicNotifyType.Dictionary:
            {
                var dicId = info.ToInt();
                if (dicId == -1)
                {
                    ShowDictionaryTip(info, addChat == 1);
                }
                else
                {
                    ShowDictionaryTip(dicId, addChat == 1);
                }
            }
                break;
            case eLogicNotifyType.BagFull:
            {
                var id = info.ToInt();
                var e = new ShowUIHintBoard(id);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException("type");
        }
    }

	public IEnumerator SendJsonCoroutine(string str, Action<int, string> call,Action<int>errorCall)
	{
		var msg = NetManager.Instance.SendJsonData(str);
		yield return msg.SendAndWaitUntilDone();
		if (msg.State != MessageState.Reply)
		{
			if (null != errorCall)
			{
				errorCall((int) msg.State);
			}
			yield break;
		}

		if (null != call)
		{
			call(msg.ErrorCode, msg.Response);
		}
	}
}