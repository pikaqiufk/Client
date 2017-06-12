#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using UnityEngine;

#endregion

public class ArenaController : IControllerBase
{
    public ArenaController()
    {
        IsInit = false;
        CleanUp();
        //event
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit);
        EventDispatcher.Instance.AddEventListener(ArenaExdataUpdate.EVENT_TYPE, OnUpdateExData);
        EventDispatcher.Instance.AddEventListener(ExData64InitEvent.EVENT_TYPE, OnExDataInit64);
        EventDispatcher.Instance.AddEventListener(ExData64UpDataEvent.EVENT_TYPE, OnUpdateExData64);
        EventDispatcher.Instance.AddEventListener(FlagUpdateEvent.EVENT_TYPE, OnFlagUpdate);
        EventDispatcher.Instance.AddEventListener(CityDataInitEvent.EVENT_TYPE, OnCityDataInit);
        EventDispatcher.Instance.AddEventListener(VipLevelChangedEvent.EVENT_TYPE, OnVipLevelChanged);
        //arena
        EventDispatcher.Instance.AddEventListener(Resource_Change_Event.EVENT_TYPE, OnUpdateHonor);
        EventDispatcher.Instance.AddEventListener(ArenaOperateEvent.EVENT_TYPE, OnArenaOperateEvent);
        EventDispatcher.Instance.AddEventListener(AreanOppentCellClick.EVENT_TYPE, OnAreanOppentCellClick);
        EventDispatcher.Instance.AddEventListener(ArenaFightRecoardChange.EVENT_TYPE, OnArenaFightRecoardChange);

        //statue
        EventDispatcher.Instance.AddEventListener(UIEvent_SeeSkills.EVENT_TYPE, SeePetSkills);
        EventDispatcher.Instance.AddEventListener(UIEvent_Promotion_Rank.EVENT_TYPE, PromotionRank);
        EventDispatcher.Instance.AddEventListener(UIEvent_CityUpdateBuilding.EVENT_TYPE, OnUpdateBuilding);
        EventDispatcher.Instance.AddEventListener(SatueOperateEvent.EVENT_TYPE, OnSatueOperate);
        EventDispatcher.Instance.AddEventListener(ArenaPetListEvent.EVENT_TYPE, OnArenaPetListEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_CityEvent.EVENT_TYPE, OnCityEvent);
        EventDispatcher.Instance.AddEventListener(UIEvent_PetLevelup.EVENT_TYPE, PetLevelup);
        EventDispatcher.Instance.AddEventListener(UIEvent_OnClickRankBtn.EVENT_TYPE, OnClickRank);
    }

    public ArenaDataModel ArenaDataModel;
    public ReadonlyObjectList<AttributeBaseDataModel> AttributeList = new ReadonlyObjectList<AttributeBaseDataModel>(3);
    public BuildingData BuildingData;
    public bool IsInit;
    public BuildingRecord mTbBuilding;
    public BuildingServiceRecord mTbBuildingService;
    public List<int> StatueOpenLevel;
    //--------------------------------------------------------------Notice--------
    public Coroutine NoticeArenaCoroutine;
    public Coroutine NoticeStatuCoroutine;
    public StatueDataModel StatueDataModel;

    public void AddStatusExp(StatueInfoDataModel infoData, int exp)
    {
        var statusId = infoData.DataIndex;
        var lastExp = infoData.CurExp;
        lastExp += exp;
        var tbStatus = Table.GetStatue(statusId);


        var needExp = tbStatus.LevelUpExp;
        var maxList = new List<int>();
        maxList.Add(needExp);
        var isLvUp = false;
        while (lastExp >= needExp)
        {
            if (tbStatus.NextLevelID == -1)
            {
                lastExp = tbStatus.LevelUpExp;
                break;
            }
            lastExp -= needExp;
            statusId = tbStatus.NextLevelID;
            tbStatus = Table.GetStatue(tbStatus.NextLevelID);
            needExp = tbStatus.LevelUpExp;
            maxList.Add(tbStatus.LevelUpExp);
            isLvUp = true;
        }

        infoData.DataIndex = statusId;
        infoData.CurExp = lastExp;

        var index = StatueDataModel.SelectStatue.Index;

        BuildingData.Exdata[index] = statusId;
        BuildingData.Exdata64[index] = lastExp;
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Statue);

//        StatueDataModel.ExpSlider.MaxValues = maxList;
//        StatueDataModel.ExpSlider.TargetValue = infoData.CurExp/(float) tbStatus.LevelUpExp + (maxList.Count - 1);
        if (isLvUp)
        {
            RefreshStatueMaintain(infoData);
            RefreshStatueAttribute(infoData);
        }
    }

    public void AnalyseNoticeArena()
    {
        if (NoticeArenaCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(NoticeArenaCoroutine);
            NoticeArenaCoroutine = null;
        }

        if (ArenaDataModel.EnterCount > 0)
        {
            if (ArenaDataModel.RefreshTime <= Game.Instance.ServerTime)
            {
                PlayerDataManager.Instance.NoticeData.ArenaCount = true;
            }
            else
            {
                var scends = (int) (ArenaDataModel.RefreshTime - Game.Instance.ServerTime).TotalSeconds;
                NoticeArenaCoroutine = NetManager.Instance.StartCoroutine(AnalyseNoticeArenaCoroutine(scends));
                PlayerDataManager.Instance.NoticeData.ArenaCount = false;
            }
        }
        else
        {
            PlayerDataManager.Instance.NoticeData.ArenaCount = false;
        }
    }

    public IEnumerator AnalyseNoticeArenaCoroutine(int scends)
    {
        yield return new WaitForSeconds(scends);
        AnalyseNoticeArena();
    }

    public void AnalyseNoticeStatue()
    {
        if (StatueDataModel.HasStatueOpen == false)
        {
            return;
        }
        if (NoticeStatuCoroutine != null)
        {
            NetManager.Instance.StopCoroutine(NoticeStatuCoroutine);
            NoticeStatuCoroutine = null;
        }

        if (StatueDataModel.ChallengeCount > 0)
        {
            if (StatueDataModel.MaintainCd <= Game.Instance.ServerTime)
            {
                PlayerDataManager.Instance.NoticeData.ArenaStatus = PlayerDataManager.Instance.CheckCondition(2014) == 0;
            }
            else
            {
                if (StatueDataModel.MaintainCdFlag)
                {
                    var scends = (int) (StatueDataModel.MaintainCd - Game.Instance.ServerTime).TotalSeconds;
                    NoticeArenaCoroutine = NetManager.Instance.StartCoroutine(AnalyseNoticStatuCoroutine(scends));
                    PlayerDataManager.Instance.NoticeData.ArenaStatus = false;
                }
                else
                {
                    PlayerDataManager.Instance.NoticeData.ArenaStatus = PlayerDataManager.Instance.CheckCondition(2014) == 0;
                }
            }
        }
        else
        {
            PlayerDataManager.Instance.NoticeData.ArenaStatus = false;
        }
    }

    public IEnumerator AnalyseNoticStatuCoroutine(int scends)
    {
        yield return new WaitForSeconds(scends);
        AnalyseNoticeStatue();
    }

    public IEnumerator ApplyArenaInfoCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.GetP1vP1LadderPlayer(-1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var info = msg.Response;
                    var infoPlayersCount6 = info.Players.Count;
                    for (var i = 0; i < infoPlayersCount6; i++)
                    {
                        var playerInfo = info.Players[i];
                        var opponent = ArenaDataModel.OpponentList[i];
                        opponent.FightValue = playerInfo.FightPoint;
                        opponent.Guid = playerInfo.Id;
                        opponent.Name = playerInfo.Name;
                        opponent.RoleId = playerInfo.TypeId;
                        opponent.Rank = info.Ranks[i];
                        opponent.Level = playerInfo.Level;
                        opponent.Reincarnation = playerInfo.Ladder;
                    }
                    UpdateRank(info.NowLadder);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....GetP1vP1LadderPlayer...ErrorCode....{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....GetP1vP1LadderPlayer...State....{0}.", msg.State);
            }
        }
    }

    public IEnumerator ApplyFightRecoardCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            ArenaDataModel.RecoardList.Clear();
            ArenaDataModel.RecordCount = 0;
            var msg = NetManager.Instance.GetP1vP1LadderOldList(-1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var list = new List<ArenaRecoardDataModel>();
                    {
                        var __list6 = msg.Response.Data;
                        var __listCount6 = __list6.Count;
                        for (var __i6 = 0; __i6 < __listCount6; ++__i6)
                        {
                            var changeOne = __list6[__i6];
                            {
                                var recoardData = new ArenaRecoardDataModel();
                                recoardData.Name = changeOne.Name;
                                recoardData.NewRank = changeOne.NewRank;
                                recoardData.OldRank = changeOne.OldRank;
                                recoardData.Type = changeOne.Type;
                                FormatRecoard(recoardData);
                                list.Insert(0, recoardData);
                            }
                        }
                    }
                    ArenaDataModel.RecoardList = new ObservableCollection<ArenaRecoardDataModel>(list);
                    ArenaDataModel.RecordCount = list.Count;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Info("GetP1vP1LadderOldList error=[{0}]", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Info("GetP1vP1LadderOldList State=[{0}]", msg.State);
            }
        }
    }

    public IEnumerator ApplyPromotionRankCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var tbHonor = Table.GetHonor(ArenaDataModel.MilitaryRank);
            var honor = PlayerDataManager.Instance.GetRes((int) eResourcesType.Honor);
            if (tbHonor == null)
            {
                yield break;
            }
            var msg = NetManager.Instance.UpgradeHonor(ArenaDataModel.MilitaryRank);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.SetExData((int) eExdataDefine.e250, tbHonor.NextRank);
                    var newHonour = honor - tbHonor.NeedHonor;
                    PlayerDataManager.Instance.SetRes((int) eResourcesType.Honor, newHonour);

                    ArenaDataModel.MilitaryRank = tbHonor.NextRank;
                    ArenaDataModel.HonorCount = newHonour;
                    UpdateHonorRank();
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_ResNoEnough)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(GameUtils.GetDictionaryText(210104)));
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                        Logger.Error(".....UpgradeHonor...ErrorCode....{0}.", msg.ErrorCode);
                    }
                }
            }
            else
            {
                Logger.Error(".....UpgradeHonor...State....{0}.", msg.State);
            }
        }
    }

    public void AreanOppentFight(int index)
    {
        var data = ArenaDataModel.OpponentList[index];

        var sceneId = GameLogic.Instance.Scene.SceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene.Type == (int)eSceneType.Pvp)
        {
            //"竞技场不能直接进入竞技场战斗
            var e1 = new ShowUIHintBoard(270003);
            EventDispatcher.Instance.DispatchEvent(e1);
            return;
        }

        if (ArenaDataModel.EnterCount > 0 && ArenaDataModel.RefreshTime > Game.Instance.ServerTime)
        {
            var str = GameUtils.GetDictionaryText(220400);
            var price = 0;
            var tbClientConfig = Table.GetClientConfig(203);
            int.TryParse(tbClientConfig.Value, out price);
            var strInfo = string.Format(str, price);

            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, strInfo, "",
                () => { NetManager.Instance.StartCoroutine(FightOppentInfoCoroutine(data, 1)); });
            return;
        }

        if (ArenaDataModel.EnterCount <= 0)
        {
            if (ArenaDataModel.BuyCount <= 0)
            {
                var tbVip = PlayerDataManager.Instance.TbVip;
                var oldAdd = tbVip.PKBuyCount;
                do
                {
                    tbVip = Table.GetVIP(tbVip.Id + 1);
                } while (tbVip != null && tbVip.PKBuyCount <= oldAdd);

                if (tbVip == null)
                {
                    var e1 = new ShowUIHintBoard(220401);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
                else
                {
                    GameUtils.GuideToBuyVip(tbVip.Id, 270297);
                }
                return;
            }
            var str = GameUtils.GetDictionaryText(220431);
            var tbUpgrade = Table.GetSkillUpgrading(19999);
            var result = tbUpgrade.GetSkillUpgradingValue(ArenaDataModel.BuyMax - ArenaDataModel.BuyCount);
            var strInfo = string.Format(str, result);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, strInfo, "",
                () => { NetManager.Instance.StartCoroutine(FightOppentInfoCoroutine(data, 2)); });
            return;
        }
        NetManager.Instance.StartCoroutine(FightOppentInfoCoroutine(data, 0));
    }

    public void AreanOppentInfo(int index)
    {
        var data = ArenaDataModel.OpponentList[index];
        PlayerDataManager.Instance.ShowCharacterPopMenu(data.Guid, data.Name, 17, data.Level, data.Reincarnation,
            data.RoleId);
    }

    private void BeginClean()
    {
        if (!CheckBuildService(2))
        {
            return;
        }
        var arg = new CleanDustArguments();
        arg.StatueIndex = StatueDataModel.SelectStatue.DataIndex;
        var e = new Show_UI_Event(UIConfig.CleanDust, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void BeginPuzzel()
    {
        if (!CheckBuildService(1))
        {
            return;
        }
        var arg = new PuzzleImageArguments();
        arg.StatueIndex = StatueDataModel.SelectStatue.DataIndex;
        var e = new Show_UI_Event(UIConfig.PuzzleImage, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public bool CheckBuildService(int type)
    {
        if (type < 0 || type > 2)
        {
            return false;
        }
        var selectStaue = StatueDataModel.SelectStatue;
        if (selectStaue.IsOpen == false)
        {
            //当前神像不能维护
            var e = new ShowUIHintBoard(270007);
            EventDispatcher.Instance.DispatchEvent(e);
            return false;
        }

        var tbStatue = Table.GetStatue(selectStaue.DataIndex);
        if (tbStatue.NextLevelID == -1 && tbStatue.LevelUpExp == selectStaue.CurExp)
        {
            //已经生到最大的等级了
            var e = new ShowUIHintBoard(270010);
            EventDispatcher.Instance.DispatchEvent(e);
            return false;
        }

        var itemId = selectStaue.MaintainItemId[type];
        if (selectStaue.MaintainItemCount[type] > PlayerDataManager.Instance.GetItemCount(itemId))
        {
            var tbItem = Table.GetItemBase(itemId);
            //{0}不足
            var str = string.Format(GameUtils.GetDictionaryText(270011), tbItem.Name);
            var e = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e);
            PlayerDataManager.Instance.ShowItemInfoGet(itemId);
            return false;
        }

        if (StatueDataModel.ChallengeCount == 0)
        {
            //剩余维护次数不足
            var tbVip = PlayerDataManager.Instance.TbVip;
            var oldAddCount = tbVip.StatueAddCount;
            do
            {
                tbVip = Table.GetVIP(tbVip.Id + 1);
            } while (tbVip != null && oldAddCount >= tbVip.StatueAddCount);

            if (tbVip == null)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300130));
            }
            else
            {
                GameUtils.GuideToBuyVip(tbVip.Id);
            }
            return false;
        }

        if (Game.Instance.ServerTime < StatueDataModel.MaintainCd)
        {
            if (StatueDataModel.MaintainCdFlag)
            {
                //正在冷却中
                var e = new ShowUIHintBoard(270009);
                EventDispatcher.Instance.DispatchEvent(e);
                return false;
            }
        }
        return true;
    }

    private IEnumerator CoolingCoroutine(int needDia)
    {
        using (new BlockingLayerHelper(0))
        {
            var ary = new Int32Array();
            ary.Items.Add(1);
            ary.Items.Add(needDia);
            var msg = NetManager.Instance.UseBuildService(BuildingData.AreaId, mTbBuilding.ServiceId, ary);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    // StatueDataModel.IsShowCd = false;
                    AnalyseNoticeStatue();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....UseBuildService...ErrorCode....{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....UseBuildService...State....{0}.", msg.State);
            }
        }
    }

    public IEnumerator FightOppentInfoCoroutine(ArenaOpponentDataModel data, int type)
    {
        using (new BlockingLayerHelper(0))
        {
            //0  正常
            //1  cd购买
            //2  次数购买
            var rank = data.Rank - 1;
            var guid = data.Guid;
            if (type == 1)
            {
                var price = 0;
                var tbClientConfig = Table.GetClientConfig(203);
                int.TryParse(tbClientConfig.Value, out price);
                if (price > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                    yield break;
                }
            }
            else if (type == 2)
            {
                var tbUpgrade = Table.GetSkillUpgrading(19999);
                var price = tbUpgrade.GetSkillUpgradingValue(ArenaDataModel.BuyMax - ArenaDataModel.BuyCount);
                if (price > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Diamond)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                    yield break;
                }
            }
            var msg = NetManager.Instance.GetP1vP1FightPlayer(rank, guid, type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlatformHelper.Event("city", "arenaFight");
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_LadderChange)
                {
                    //对手的名次已经改变
                    UIManager.Instance.ShowMessage(MessageBoxType.Ok, 220399);
                    NetManager.Instance.StartCoroutine(ApplyArenaInfoCoroutine());
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Info("GetP1vP1FightPlayer error=[{0}]", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Info("GetP1vP1FightPlayer State=[{0}]", msg.State);
            }
        }
    }

    public void FormatRecoard(ArenaRecoardDataModel recoard)
    {
        if (recoard.Type == 0)
        {
//主动进攻
            if (recoard.NewRank == -1)
            {
                //你挑战了{0}获得胜利，排名不变
                var str = GameUtils.GetDictionaryText(220446);
                recoard.Content = string.Format(str, recoard.Name);
            }
            else if (recoard.NewRank < recoard.OldRank)
            {
//你挑战了{0}获得胜利，排名上升至{1}
                var str = GameUtils.GetDictionaryText(220402);
                recoard.Content = string.Format(str, recoard.Name, recoard.NewRank);
            }
            else
            {
                //你挑战了{0}失败了，排名不变
                var str = GameUtils.GetDictionaryText(220403);
                recoard.Content = string.Format(str, recoard.Name);
            }
        }
        else
        {
            if (recoard.NewRank == -1)
            {
//{0}挑战了你，你失败了，排名不变
                var str = GameUtils.GetDictionaryText(220449);
                recoard.Content = string.Format(str, recoard.Name);
            }
            else if (recoard.NewRank > recoard.OldRank)
            {
                //{0}挑战了你，你失败了，排名下降至{1}
                var str = GameUtils.GetDictionaryText(220405);
                recoard.Content = string.Format(str, recoard.Name, recoard.NewRank);
            }
            else
            {
                //{0}挑战了你，你胜利了，排名不变
                var str = GameUtils.GetDictionaryText(220404);
                recoard.Content = string.Format(str, recoard.Name);
            }
        }
    }

    public int GetSatueOpenLevel(int index, int serverId)
    {
        //for (var i = serverId%10 + 1; i < 9; i++)
        //{
        //    var tableIndex = (serverId/10)*10 + i;
        //    var varBulidingServer = Table.GetBuildingService(tableIndex);
        //    if (varBulidingServer != null)
        //    {
        //        if (varBulidingServer.Param[0] > index)
        //        {
        //            return i + 1;
        //        }
        //    }
        //}

        if (index >= 0 && index < StatueOpenLevel.Count)
        {
            return StatueOpenLevel[index];
        }

        return -1;
    }

    public int GetStatueOpenCount()
    {
        var playerLevel = PlayerDataManager.Instance.GetLevel();
        var count = 0;
        var enumerator = StatueOpenLevel.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (playerLevel >= enumerator.Current)
            {
                ++count;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    public void Init()
    {
        if (IsInit)
            return;

        IsInit = true;
        InitRankAward();
        InItRanklist();
        var count = 0;
        var tbMaxBs = Table.GetBuildingService(60);
        if (tbMaxBs != null)
        {
            if (tbMaxBs.Param.Length > 0)
            {
                count = tbMaxBs.Param[0];
            }
        }
        StatueDataModel.StatueLimitCount = count;
        StatueOpenLevel = new List<int>(StatueDataModel.StatueLimitCount);
        StatueOpenLevel.Add(Table.GetClientConfig(930).ToInt());
        StatueOpenLevel.Add(Table.GetClientConfig(931).ToInt());
        StatueOpenLevel.Add(Table.GetClientConfig(932).ToInt());
        StatueOpenLevel.Add(Table.GetClientConfig(933).ToInt());

        NetManager.Instance.StartCoroutine(ApplyFightRecoardCoroutine());
    }

    public void InitRankAward()
    {
        var last = 1;
        var list = new List<ArenaRankAwardDataModel>();
        Table.ForeachArenaReward(recoard =>
        {
            var cell = new ArenaRankAwardDataModel();
            cell.Id = recoard.Id;
            cell.Form = last;
            last = cell.Id + 1;
            list.Add(cell);
            return true;
        });
        ArenaDataModel.RankAwards = new ObservableCollection<ArenaRankAwardDataModel>(list);
    }

    public void InItRanklist()
    {
        if (ArenaDataModel.RankList.Count != 0)
        {
            return;
        }
        var list = new List<ArenaRankDataModel>();
        Table.ForeachHonor(act =>
        {
            if (act != null && act.Id != 0)
            {
                var rank = new ArenaRankDataModel();
                var tbNameTitle = Table.GetNameTitle(act.TitleId);
                rank.HonorId = act.Id;
                var propCount = tbNameTitle.PropValue.Length;
                var propStr1 = string.Empty;
                var propStr2 = string.Empty;
                rank.Item.Id = act.TitleId;
                for (var i = 0; i < propCount; i++)
                {
                    var propId = tbNameTitle.PropId[i];
                    if (propId == -1)
                    {
                        continue;
                    }
                    var value = tbNameTitle.PropValue[i];
                    if (propId == 5)
                    {
                        propStr1 = GameUtils.GetDictionaryText(222001) + ":" + value + "-";
                    }
                    else if (propId == 6)
                    {
                        propStr1 += value;
                    }
                    else if (propId == 7)
                    {
                        propStr2 = GameUtils.GetDictionaryText(222002) + ":" + value + "-";
                    }
                    else if (propId == 8)
                    {
                        propStr2 += value;
                    }
                    else
                    {
                        var attr = new AttributeStringDataModel();
                        var str = ExpressionHelper.AttrName[propId] + ":";

                        str += GameUtils.AttributeValue(propId, value);
                        attr.LabelString = str;
                        rank.Item.Attributes.Add(attr);
                    }
                }
                if (propStr1 != string.Empty)
                {
                    var attr = new AttributeStringDataModel();
                    attr.LabelString = propStr1;
                    rank.Item.Attributes.Insert(0, attr);
                }
                if (propStr2 != string.Empty)
                {
                    var attr = new AttributeStringDataModel();
                    attr.LabelString = propStr2;
                    rank.Item.Attributes.Insert(0, attr);
                }
                if (act.Id > ArenaDataModel.MilitaryRank)
                {
                    rank.Item.State = 0;
                }
                else
                {
                    rank.Item.State = 1;
                }
                list.Add(rank);
            }
            return true;
        });
        ArenaDataModel.RankList = new ObservableCollection<ArenaRankDataModel>(list);
    }

    public void OnAreanOppentCellClick(IEvent ievent)
    {
        var e = ievent as AreanOppentCellClick;

        var index = e.Index;
        switch (e.Type)
        {
            case 0:
            {
                AreanOppentFight(index);
            }
                break;
            case 1:
            {
                AreanOppentInfo(index);
            }
                break;
        }
    }

    public void OnArenaFightRecoardChange(IEvent ievent)
    {
        var e = ievent as ArenaFightRecoardChange;
        var changeOne = e.Data;
        if (ArenaDataModel.RecoardList.Count > 50)
        {
//数据太多时，需要删除一部分
            var list = new List<ArenaRecoardDataModel>(ArenaDataModel.RecoardList.ToArray());
            list.RemoveRange(0, 10);
            ArenaDataModel.RecoardList = new ObservableCollection<ArenaRecoardDataModel>(list);
        }
        var recoardData = new ArenaRecoardDataModel();
        recoardData.Name = changeOne.Name;
        recoardData.NewRank = changeOne.NewRank;
        recoardData.OldRank = changeOne.OldRank;
        recoardData.Type = changeOne.Type;
        FormatRecoard(recoardData);
        ArenaDataModel.RecoardList.Insert(0, recoardData);
        ArenaDataModel.RecordCount = ArenaDataModel.RecoardList.Count;
        if (recoardData.NewRank != -1)
        {
            NetManager.Instance.StartCoroutine(ApplyArenaInfoCoroutine());
        }
    }

    public void OnArenaOperateEvent(IEvent ievent)
    {
        var e = ievent as ArenaOperateEvent;
        switch (e.Type)
        {
            case 0:
            {
                NetManager.Instance.StartCoroutine(ApplyArenaInfoCoroutine());
            }
                break;
            case 1:
            {
                ShowHonorExchangeIndex();
            }
                break;
        }
    }

    public void OnArenaPetListEvent(IEvent ievent)
    {
        var e = ievent as ArenaPetListEvent;

        var petlist = new List<PetItemDataModel>();
        if (e.IsShow == false)
        {
            return;
        }

        StatueDataModel.PetList.Clear();
        PetItemDataModel checkModel = null;
        {
            var __list2 = CityManager.Instance.GetAllPetByFilter(PetListFileterType.Employ);
            var __listCount2 = __list2.Count;
            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var pet = __list2[__i2];
                {
                    var data = CityManager.PetItem2DataModel(pet);
                    {
                        for (var i = 0; i < data.Skill.SpecialSkills.Count; i++)
                        {
                            if (data.Skill.SpecialSkills[i].SkillId != -1)
                            {
                                if (Table.GetPetSkill(data.Skill.SpecialSkills[i].SkillId).Param[0] == 6 &&
                                    Table.GetPetSkill(data.Skill.SpecialSkills[i].SkillId).Param[1] == 1)
                                {
                                    data.BuffIconIdlist[i].Active = true;
                                    data.BuffIconIdlist[i].BuffId =
                                        Table.GetPetSkill(data.Skill.SpecialSkills[i].SkillId).SkillIcon;
                                }
                            }
                        }

                        if (data.State == (int) PetStateType.Idle)
                        {
                            data.ShowCheck = true;
                            data.ShowMask = false;
                        }
                        else
                        {
                            data.ShowCheck = false;
                            data.ShowMask = true;
                        }
                        // foreach(var info in StatueDataModel.StatueInfos)
                        var __enumerator10 = (StatueDataModel.StatueInfos).GetEnumerator();
                        while (__enumerator10.MoveNext())
                        {
                            var info = __enumerator10.Current;
                            {
                                if (info.IsSelect)
                                {
                                    if (data.ItemId == info.ItemId)
                                    {
                                        data.Checked = true;
                                        data.ShowMask = false;
                                        data.ShowCheck = true;
                                        checkModel = data;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    if (data.Checked)
                    {
                        continue;
                    }
                    petlist.Add(data);
                }
            }
        }
        petlist.Sort((x, y) =>
        {
            var xcount = 0;
            var ycount = 0;
            {
                // foreach(var data in x.BuffIconIdlist)
                var __enumerator16 = (x.BuffIconIdlist).GetEnumerator();
                while (__enumerator16.MoveNext())
                {
                    var data = __enumerator16.Current;
                    {
                        if (data.Active)
                        {
                            xcount++;
                        }
                    }
                }
            }
            {
                // foreach(var data in y.BuffIconIdlist)
                var __enumerator17 = (y.BuffIconIdlist).GetEnumerator();
                while (__enumerator17.MoveNext())
                {
                    var data = __enumerator17.Current;
                    {
                        if (data.Active)
                        {
                            ycount++;
                        }
                    }
                }
            }
            if (xcount > ycount)
            {
                return 1;
            }
            if (xcount == ycount)
            {
                return 0;
            }
            return -1;
        });
        {
            var __list18 = petlist;
            var __listCount18 = __list18.Count;
            for (var __i18 = 0; __i18 < __listCount18; ++__i18)
            {
                var data = __list18[__i18];
                {
                    if (data.ShowCheck)
                    {
                        StatueDataModel.PetList.Insert(0, data);
                    }
                    else
                    {
                        StatueDataModel.PetList.Add(data);
                        data.BuffIconIdlist[0].Active = false;
                    }
                }
            }
        }
        if (checkModel != null)
        {
            StatueDataModel.PetList.Insert(0, checkModel);
            checkModel.BuffIconIdlist[0].Active = false;
        }
        StatueDataModel.PetCount = StatueDataModel.PetList.Count;
    }

    public void OnCityDataInit(IEvent ievent)
    {
        BuildingData = null;
        {
            // foreach(var buildingData in CityManager.Instance.BuildingDataList)
            var __enumerator12 = (CityManager.Instance.BuildingDataList).GetEnumerator();
            while (__enumerator12.MoveNext())
            {
                var buildingData = __enumerator12.Current;
                {
                    var typeId = buildingData.TypeId;
                    var tbBuild = Table.GetBuilding(typeId);
                    if (tbBuild == null)
                    {
                        continue;
                    }
                    if (tbBuild.Type == 6)
                    {
                        BuildingData = buildingData;
                        break;
                    }
                }
            }
        }
        if (BuildingData != null)
        {
            RefreshBuildingDataModel(BuildingData);
        }

        AnalyseNoticeArena();
        UpdateStatusCount();
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Statue);
    }

    public void OnCityEvent(IEvent ievent)
    {
        if (State != FrameState.Open)
        {
            return;
        }
        var e = ievent as UIEvent_CityEvent;

        if (e == null)
        {
            return;
        }
        if (e.IntParam == null || e.IntParam.Count != 1)
        {
            return;
        }

        var index = e.IntParam[0];

        if (index >= StatueDataModel.PetList.Count)
        {
            return;
        }


        StatueDataModel.PetList[index].Checked = !StatueDataModel.PetList[index].Checked;

        if (StatueDataModel.PetList[index].Checked)
        {
            var c = StatueDataModel.PetList.Count;
            for (var i = 0; i < c; i++)
            {
                var d = StatueDataModel.PetList[i];
                if (d.Checked && d.ItemId != StatueDataModel.PetList[index].ItemId)
                {
                    d.Checked = false;
                }
            }
        }

        switch (e.StringParam)
        {
            case "ChoosePet":
            {
                OperateStatuePet();
            }
                break;
        }
    }

    public void OnClickRank(IEvent ievent)
    {
        var e = ievent as UIEvent_OnClickRankBtn;
        {
            // foreach(var data in ArenaDataModel.RankList)
            var __enumerator11 = (ArenaDataModel.RankList).GetEnumerator();
            while (__enumerator11.MoveNext())
            {
                var data = __enumerator11.Current;
                {
                    data.Item.IsSelect = false;
                }
            }
        }
        ArenaDataModel.RankList[e.Idx].Item.IsSelect = true;
    }

    private void OnClickStatu(int index)
    {
        if (index < 0 || index > 4)
        {
            return;
        }
        SetSatueInfo(index);
    }

    public void OnExDataInit(IEvent ievent)
    {
        var e = ievent as ExDataInitEvent;
        var topRank = PlayerDataManager.Instance.GetExData(eExdataDefine.e93);
        if (topRank > 1000)
        {
            ArenaDataModel.TopRank = -1;
        }
        else
        {
            ArenaDataModel.TopRank = topRank;
        }

        var tbVip = PlayerDataManager.Instance.TbVip;
        var tbExdata = Table.GetExdata((int) eExdataDefine.e98);
        ArenaDataModel.EnterMax = tbExdata.InitValue;
        ArenaDataModel.EnterCount = PlayerDataManager.Instance.GetExData(eExdataDefine.e98);
        tbExdata = Table.GetExdata((int) eExdataDefine.e99);
        ArenaDataModel.BuyMax = tbExdata.InitValue + tbVip.PKBuyCount;
        ArenaDataModel.BuyCount = PlayerDataManager.Instance.GetExData(eExdataDefine.e99) + tbVip.PKBuyCount;

        UpdateHonorRank();
        NetManager.Instance.StartCoroutine(ApplyArenaInfoCoroutine());
    }

    public void OnExDataInit64(IEvent ievent)
    {
        var time = PlayerDataManager.Instance.GetExData64((int) Exdata64TimeType.P1vP1CoolDown);
        RefreshFightTime(time);

        var time1 = PlayerDataManager.Instance.GetExData64((int) Exdata64TimeType.StatueCdTime);
        RefreshStatusCdTime(time1);
    }

    public void OnFlagUpdate(IEvent ievent)
    {
        var e = ievent as FlagUpdateEvent;
        switch (e.Index)
        {
            case 487:
            {
                RefreshStatusCdTime();
            }
                break;
        }
    }

    //--------------------------------------------------------------Statue--------
    public void OnSatueOperate(IEvent ievent)
    {
        var e = ievent as SatueOperateEvent;
        switch (e.Type)
        {
            case 1:
            {
                //挑战成功
                UseBuildService(2);
            }
                break;
            case 2:
            {
                //挑战失败
                var e1 = new ShowUIHintBoard(270005);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
            case 3:
            {
                PlatformHelper.Event("city", "arenaService", 1);
                BeginPuzzel();
            }
                break;
            case 4:
            {
                PlatformHelper.Event("city", "arenaService", 2);
                BeginClean();
            }
                break;
            case 11:
            {
                //挑战成功
                UseBuildService(1);
            }
                break;
            case 12:
            {
                //挑战失败
                var e1 = new ShowUIHintBoard(270005);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
            case 13:
            {
                //cool
                StatusCooling();
            }
                break;
            case 20:
            {
                PlatformHelper.Event("city", "arenaService", 0);
                UseBuildService(0);
            }
                break;
            case 30:
            {
                RefreshStatusCdTime();
            }
                break;
            case 41:
            {
                ShowBeforeSatueInfo();
            }
                break;
            case 42:
            {
                ShowNextSatueInfo();
            }
                break;
            case 100:
            {
                OnClickStatu(e.Index);
            }
                break;
        }
    }

    //-------------------------------------------------------------Event-------------------
    public void OnUpdateBuilding(IEvent ievent)
    {
        var e = ievent as UIEvent_CityUpdateBuilding;

        var building = CityManager.Instance.GetBuildingByAreaId(e.Idx);
        if (null == building)
        {
            return;
        }
        var bulidId = building.TypeId;
        var tbBuilding = Table.GetBuilding(bulidId);
        if (tbBuilding == null)
        {
            return;
        }
        if (tbBuilding.Type != (int) BuildingType.ArenaTemple)
        {
            return;
        }
        RefreshBuildingDataModel(building);
    }

    public void OnUpdateExData(IEvent ievent)
    {
        var e = ievent as ArenaExdataUpdate;
        var hasUpdate = false;
        switch (e.Type)
        {
            case eExdataDefine.e93:
            {
                hasUpdate = true;
                if (e.Value > 1000)
                {
                    ArenaDataModel.TopRank = -1;
                }
                else
                {
                    ArenaDataModel.TopRank = e.Value;
                }
            }
                break;
            case eExdataDefine.e98:
            {
                hasUpdate = true;
                var tbExdata = Table.GetExdata((int) eExdataDefine.e98);
                ArenaDataModel.EnterMax = tbExdata.InitValue;
                ArenaDataModel.EnterCount = e.Value;
            }
                break;
            case eExdataDefine.e99:
            {
                hasUpdate = true;
                var tbVip = PlayerDataManager.Instance.TbVip;
                var tbExdata = Table.GetExdata((int) eExdataDefine.e99);
                ArenaDataModel.BuyMax = tbExdata.InitValue + tbVip.PKBuyCount;
                ArenaDataModel.BuyCount = e.Value + tbVip.PKBuyCount;
            }
                break;
            case eExdataDefine.e400:
            {
                hasUpdate = true;
                UpdateStatusCount();
            }
                break;

            case eExdataDefine.e250:
            {
                UpdateHonorRank();
            }
                break;
        }
        if (hasUpdate)
        {
            AnalyseNoticeArena();
        }
    }

    public void OnUpdateExData64(IEvent ievent)
    {
        var e = ievent as ExData64UpDataEvent;
        switch ((Exdata64TimeType) e.Key)
        {
            case Exdata64TimeType.P1vP1CoolDown:
            {
                RefreshFightTime(e.Value);
            }
                break;
            case Exdata64TimeType.StatueCdTime:
            {
                RefreshStatusCdTime(e.Value);
            }
                break;
        }
    }

    public void OnUpdateHonor(IEvent ievent)
    {
        var e = ievent as Resource_Change_Event;
        if (e.Type == eResourcesType.Honor)
        {
            ArenaDataModel.HonorCount = e.NewValue;
            UpdateHonorRank();
        }
    }

    private void OnVipLevelChanged(IEvent ievent)
    {
        var tbVip = PlayerDataManager.Instance.TbVip;
        var tbExdata = Table.GetExdata((int) eExdataDefine.e99);
        ArenaDataModel.BuyMax = tbExdata.InitValue + tbVip.PKBuyCount;
        ArenaDataModel.BuyCount = PlayerDataManager.Instance.GetExData(eExdataDefine.e99) + tbVip.PKBuyCount;
        if (mTbBuildingService != null)
        {
            StatueDataModel.ChallengeCount = mTbBuildingService.Param[5] + tbVip.StatueAddCount -
                                             PlayerDataManager.Instance.GetExData(eExdataDefine.e400);
        }
        AnalyseNoticeStatue();
    }

    private void OperateStatuePet()
    {
        var list = new List<int>();
        list.Add(StatueDataModel.SelectStatue.Index);
        var isCheck = false;
        {
            // foreach(var itemDataModel in StatueDataModel.PetList)
            var __enumerator4 = (StatueDataModel.PetList).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var itemDataModel = __enumerator4.Current;
                {
                    if (itemDataModel.Checked)
                    {
                        list.Add(itemDataModel.PetId);
                        isCheck = true;

                        break;
                    }
                }
            }
        }
        if (isCheck == false)
        {
            list.Add(-1);
        }
        SendCityOptRequest(CityOperationType.ASSIGNPETINDEX, BuildingData.AreaId, list);
    }

    public void PetLevelup(IEvent ievent)
    {
        var e = ievent as UIEvent_PetLevelup;
        var list = new List<int>();
        {
            // foreach(var data in StatueDataModel.StatueInfos)
            var __enumerator15 = (StatueDataModel.StatueInfos).GetEnumerator();
            while (__enumerator15.MoveNext())
            {
                var data = __enumerator15.Current;
                {
                    if (data.ItemId == e.PetId)
                    {
                        list.Add(data.Index);
                    }
                }
            }
        }
        if (list.Count == 0)
        {
            return;
        }
        list.Add(e.PetId);
        SendCityOptRequest(CityOperationType.ASSIGNPETINDEX, BuildingData.AreaId, list);
    }

    public void PromotionRank(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ApplyPromotionRankCoroutine());
    }

    public void RefreshArena()
    {
    }

    public void RefreshBuildingDataModel(BuildingData buildingData)
    {
        if (buildingData == null)
        {
            return;
        }

        Init();

        BuildingData = buildingData;
        mTbBuilding = Table.GetBuilding(BuildingData.TypeId);
        mTbBuildingService = Table.GetBuildingService(mTbBuilding.ServiceId);
        PlayerDataManager.Instance.NoticeData.ArenaTotalIcon = mTbBuildingService.TipsIndex;
        var isShowStatue = false;
        //var varBulidingServerParam04 = mTbBuildingService.Param[0];
        var openStatueCount = GetStatueOpenCount();
        for (var i = 0; i < openStatueCount; i++)
        {
            var dataInfo = StatueDataModel.StatueInfos[i];
            dataInfo.Index = i;
            dataInfo.Condition = "";
            dataInfo.IsOpen = true;
            isShowStatue = true;
            SetStatueInfo(dataInfo, i);
            if (i >= StatueDataModel.StatueLimitCount)
            {
                dataInfo.IsShow = false;
            }
            else
            {
                dataInfo.IsShow = true;
            }
        }
        StatueDataModel.HasStatueOpen = isShowStatue;
        var buildingDataExdataCount5 = buildingData.Exdata.Count;
        for (var i = openStatueCount; i < buildingDataExdataCount5; i++)
        {
            var dataInfo = StatueDataModel.StatueInfos[i];
            dataInfo.IsOpen = false;
            dataInfo.Index = i;
            var tbStatu = Table.GetStatue(i*100);
            var openLv = GetSatueOpenLevel(i, mTbBuilding.ServiceId);
            //"级开启" 
            dataInfo.Condition = string.Format(GameUtils.GetDictionaryText(270025), openLv);
            dataInfo.Name = tbStatu.Name;
            SetStatueInfo(dataInfo, i);
            if (i >= StatueDataModel.StatueLimitCount)
            {
                dataInfo.IsShow = false;
            }
            else
            {
                dataInfo.IsShow = true;
            }
        }

        if (StatueDataModel.SelectStatue.DataIndex == -1)
        {
            //重置成第一个
            SetSatueInfo(0);
        }
        UpdateStatusCount();
    }

    public void RefreshFightTime(long time)
    {
        ArenaDataModel.RefreshTime = Extension.FromServerBinary(time);
        AnalyseNoticeArena();
    }

    public void RefreshStatueAttribute(StatueInfoDataModel dataInfo)
    {
        var tbStatue = Table.GetStatue(dataInfo.DataIndex);
        if (tbStatue == null)
        {
            return;
        }
        if (tbStatue.Level == 0)
        {
            tbStatue = Table.GetStatue(dataInfo.DataIndex + 1);
        }
        dataInfo.Name = tbStatue.Name;
        dataInfo.StatuAttribute.Type = tbStatue.PropID[0];
        dataInfo.StatuAttribute.Value = tbStatue.propValue[0];
        dataInfo.Fuse = tbStatue.FuseValue[0];
        if (dataInfo.ItemId != -1)
        {
            var ret = CityPetSkill.GetBSParamByIndex(BuildingType.ArenaTemple, mTbBuildingService, 1,
                new List<int> {dataInfo.ItemId});
            dataInfo.Fuse += ret*100;

            var rate = dataInfo.Fuse/10000.0f;
            var pet = CityManager.Instance.GetAllPetByFilterItemId(PetListFileterType.Employ, dataInfo.ItemId);

            if (pet != null)
            {
                var data = CityManager.PetItem2DataModel(pet);
                var petId = pet.ItemId;
                var level = pet.Exdata[PetItemExtDataIdx.Level];
                var type = tbStatue.FuseID[0];
                if (type == 5)
                {
                    var attributeValue =
                        FightAttribute.GetPetAttribut(petId, (eAttributeType) tbStatue.FuseID[0], level)*rate;
                    type = 105;
                    dataInfo.PetAttribute.Type = type;
                    dataInfo.PetAttribute.Value = (int) attributeValue;
                    if (tbStatue.FuseID[1] == 6)
                    {
                        var attributeValueEx =
                            FightAttribute.GetPetAttribut(petId, (eAttributeType) tbStatue.FuseID[1], level)*rate;
                        dataInfo.PetAttribute.ValueEx = (int) attributeValueEx;
                    }
                    else
                    {
                        dataInfo.PetAttribute.ValueEx = 0;
                    }
                }
                else
                {
                    dataInfo.PetAttribute.Type = type;
                    var attributeValue =
                        FightAttribute.GetPetAttribut(petId, (eAttributeType) tbStatue.FuseID[0], level)*rate;
                    dataInfo.PetAttribute.Value = (int) attributeValue;
                    dataInfo.PetAttribute.ValueEx = 0;
                }
            }
        }
        else
        {
            dataInfo.PetAttribute.Type = -1;
            dataInfo.PetAttribute.Value = 0;
            dataInfo.PetAttribute.ValueEx = 0;
        }

        var attributeType = dataInfo.StatuAttribute.Type;
        dataInfo.TotalAttribute.Type = attributeType;
        dataInfo.TotalAttribute.Value = dataInfo.PetAttribute.Value + dataInfo.StatuAttribute.Value;

        if (dataInfo.PetAttribute.ValueEx > 0)
        {
            var ex = dataInfo.StatuAttribute.ValueEx;
            if (ex <= 0)
            {
                ex = dataInfo.StatuAttribute.Value;
            }
            dataInfo.TotalAttribute.ValueEx = dataInfo.PetAttribute.ValueEx + ex;
        }
        else
        {
            dataInfo.TotalAttribute.ValueEx = 0;
        }


        dataInfo.TotalAttributeStr = GameUtils.AttributeName(attributeType) + "+" +
                                     GameUtils.AttributeValue(attributeType, dataInfo.TotalAttribute.Value);
        if (dataInfo.TotalAttribute.ValueEx != 0)
        {
            dataInfo.TotalAttributeStr += "-" + GameUtils.AttributeValue(attributeType, dataInfo.TotalAttribute.ValueEx);
        }

        dataInfo.StatuAttributeStr = GameUtils.AttributeName(attributeType) + "+" +
                                     GameUtils.AttributeValue(attributeType, dataInfo.StatuAttribute.Value);
        if (dataInfo.PetAttribute.Value != 0)
        {
            var petAttr = "";
            petAttr = GameUtils.AttributeValue(attributeType, dataInfo.PetAttribute.Value);
            if (dataInfo.PetAttribute.ValueEx != 0)
            {
                petAttr += "-" + GameUtils.AttributeValue(attributeType, dataInfo.PetAttribute.ValueEx);
            }
            dataInfo.StatuAttributeStr += "(" + petAttr + ")";
        }
    }

    public void RefreshStatueMaintain(StatueInfoDataModel dataInfo)
    {
        if (mTbBuildingService == null)
        {
            return;
        }
        var tbStatue = Table.GetStatue(dataInfo.DataIndex);
        if (tbStatue == null)
        {
            return;
        }

        var param = mTbBuildingService.Param[2];
        var skillUp = Table.GetSkillUpgrading(param);
        var skillUpLevel = skillUp.GetSkillUpgradingValue(tbStatue.Level);
        var skillUpValue = Table.GetSkillUpgrading(skillUpLevel);
        for (var i = 0; i < 3; i++)
        {
            dataInfo.MaintainItemId[i] = skillUpValue.GetSkillUpgradingValue(i);
        }

        param = mTbBuildingService.Param[3];
        skillUp = Table.GetSkillUpgrading(param);
        skillUpLevel = skillUp.GetSkillUpgradingValue(tbStatue.Level);
        skillUpValue = Table.GetSkillUpgrading(skillUpLevel);
        for (var i = 0; i < 3; i++)
        {
            dataInfo.MaintainItemCount[i] = skillUpValue.GetSkillUpgradingValue(i);
        }

        param = mTbBuildingService.Param[4];
        skillUp = Table.GetSkillUpgrading(param);
        skillUpLevel = skillUp.GetSkillUpgradingValue(tbStatue.Level);
        skillUpValue = Table.GetSkillUpgrading(skillUpLevel);
        for (var i = 0; i < 3; i++)
        {
            dataInfo.MaintainItemExp[i] = skillUpValue.GetSkillUpgradingValue(i);
        }
    }

    public void RefreshStatusCdTime(long time = 0)
    {
        if (time != 0)
        {
            StatueDataModel.MaintainCd = Extension.FromServerBinary(time);
        }
        var flag = PlayerDataManager.Instance.GetFlag(487);
        if (StatueDataModel.MaintainCd < Game.Instance.ServerTime)
        {
            flag = false;
            PlayerDataManager.Instance.SetFlag(487, flag);
        }
        if (StatueDataModel.ChallengeCount == 0)
        {
            StatueDataModel.IsShowCd = false;
        }
        else
        {
            StatueDataModel.IsShowCd = flag;
        }
        StatueDataModel.MaintainCdFlag = flag;

        AnalyseNoticeStatue();
    }

    public void SeePetSkills(IEvent ievent)
    {
        var e = ievent as UIEvent_SeeSkills;
        var flag = e.Flag;
        if (flag)
        {
            ArenaDataModel.ShowPetSkills = true;
            ArenaDataModel.PetSkills = e.Idx;
        }
        else
        {
            ArenaDataModel.ShowPetSkills = false;
        }
    }

    public void SendCityOptRequest(CityOperationType opt, int buildingIdx, List<int> param = null)
    {
        NetManager.Instance.StartCoroutine(SendCityOptRequestCoroutine(opt, buildingIdx, param));
    }

    private IEnumerator SendCityOptRequestCoroutine(CityOperationType opt, int buildingIdx, List<int> param)
    {
        using (new BlockingLayerHelper(0))
        {
            var array = new Int32Array();
            if (null != param)
            {
                {
                    var __list7 = param;
                    var __listCount7 = __list7.Count;
                    for (var __i7 = 0; __i7 < __listCount7; ++__i7)
                    {
                        var value = __list7[__i7];
                        {
                            array.Items.Add(value);
                        }
                    }
                }
            }

            var msg = NetManager.Instance.CityOperationRequest((int) opt, buildingIdx, array);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    switch (opt)
                    {
                        case CityOperationType.ASSIGNPETINDEX:
                        {
                            var info = StatueDataModel.StatueInfos[param[0]];
                            var oldPetId = info.ItemId;
                            if (oldPetId != -1)
                            {
                                var pet = CityManager.Instance.GetPetById(oldPetId);
                                pet.Exdata[3] = (int) PetStateType.Idle;
                            }
                            var newPetId = param[1];
                            if (param[1] != -1)
                            {
                                var pet = CityManager.Instance.GetPetById(newPetId);
                                pet.Exdata[3] = (int) PetStateType.Building;
                            }
                            StatueDataModel.StatueInfos[param[0]].ItemId = param[1];

                            RefreshStatueAttribute(info);

                            var index = StatueDataModel.SelectStatue.Index;
                            BuildingData.PetList[index] = param[1];
                            PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Statue);


                            if (oldPetId != -1)
                            {
                                UpdatePetState(oldPetId);
                            }
                            if (newPetId != -1)
                            {
                                UpdatePetState(newPetId);
                            }
                        }
                            break;
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_BuildPetMax)
                {
                    //该建筑的宠物已满
                    var e = new ShowUIHintBoard(270006);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard("Error:" + msg.ErrorCode));
                    Logger.Debug("SendBuildRequestCoroutine error=[{0}]", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Debug("SendBuildRequestCoroutine:MessageState.Timeout");
            }

            //AnalyseNoticeStatue();
        }
    }

    public void SetSatueInfo(int index)
    {
        StatueDataModel.SelectStatue = StatueDataModel.StatueInfos[index];
        var count = StatueDataModel.StatueInfos.Count;
        for (var i = 0; i < count; i++)
        {
            StatueDataModel.StatueInfos[i].IsSelect = i == index;
        }

//         var tbStatue = Table.GetStatue(StatueDataModel.SelectStatue.DataIndex);
// 
//         StatueDataModel.ExpSlider.MaxValues = new List<int> {tbStatue.LevelUpExp};
//         if (tbStatue.LevelUpExp == 0)
//         {
//             StatueDataModel.ExpSlider.BeginValue = 0;
//         }
//         else
//         {
//             StatueDataModel.ExpSlider.BeginValue = StatueDataModel.SelectStatue.CurExp/(float) tbStatue.LevelUpExp;
//         }
//         StatueDataModel.ExpSlider.TargetValue = StatueDataModel.ExpSlider.BeginValue;
    }

    public void RefreshExpValue(StatueInfoDataModel dataModel)
    {
      //  var dataModel = StatueDataModel.StatueInfos[index];
        var tbStatue = Table.GetStatue(dataModel.DataIndex);

        dataModel.ExpSlider.MaxValues = new List<int> { tbStatue.LevelUpExp };
        if (tbStatue.LevelUpExp == 0)
        {
            dataModel.ExpSlider.BeginValue = 0;
        }
        else
        {
            dataModel.ExpSlider.BeginValue = dataModel.CurExp / (float)tbStatue.LevelUpExp;
        }
        dataModel.ExpSlider.TargetValue = dataModel.ExpSlider.BeginValue;
    }


    public void SetStatueInfo(StatueInfoDataModel dataInfo, int index)
    {
        var tableIndex = BuildingData.Exdata[index];
        var petId = BuildingData.PetList[index];
        var exp = (int) BuildingData.Exdata64[index];
        dataInfo.DataIndex = tableIndex;
        RefreshStatueAttribute(dataInfo);
        RefreshStatueMaintain(dataInfo);
        dataInfo.ItemId = petId;
        dataInfo.CurExp = exp;
    }

    public void ShowBeforeSatueInfo()
    {
        var index = StatueDataModel.SelectStatue.Index;
        index--;
        if (index == -1)
        {
            index = StatueDataModel.StatueLimitCount - 1;
        }
        SetSatueInfo(index);
    }

    //--------------------------------------------------------------Arena--------
    public void ShowHonorExchangeIndex()
    {
        var index = -1;
        {
            // foreach(var dataModel in ArenaDataModel.RankList)
            var __enumerator14 = (ArenaDataModel.RankList).GetEnumerator();
            while (__enumerator14.MoveNext())
            {
                var dataModel = __enumerator14.Current;
                {
                    if (dataModel.Item.State == 1)
                    {
                        index++;
                    }
                }
            }
        }

        //看到中间移动一位
        index--;

        if (index < 0)
        {
            index = 0;
        }
        var e = new ArenaNotifyLogic(1, index);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void ShowNextSatueInfo()
    {
        var index = StatueDataModel.SelectStatue.Index;
        index++;
        if (index == StatueDataModel.StatueLimitCount)
        {
            index = 0;
        }
        SetSatueInfo(index);
    }

    private void StatusCooling()
    {
        if (StatueDataModel.ChallengeCount == 0)
        {
            //剩余维护次数不足
            var e = new ShowUIHintBoard(300130);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        if (Game.Instance.ServerTime > StatueDataModel.MaintainCd)
        {
            return;
        }
        var tt = StatueDataModel.MaintainCd - Game.Instance.ServerTime;
        var needDia = (int) Math.Ceiling((tt.Minutes + 1)*float.Parse(Table.GetClientConfig(572).Value));
        if (needDia > PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes))
        {
            var ee = new ShowUIHintBoard(210102);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        var str = string.Format(GameUtils.GetDictionaryText(270244), needDia);
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
            () => { NetManager.Instance.StartCoroutine(CoolingCoroutine(needDia)); });
    }

    public void UpdateHonorRank()
    {
        ArenaDataModel.MilitaryRank = PlayerDataManager.Instance.GetExData(eExdataDefine.e250);
        ArenaDataModel.HonorCount = PlayerDataManager.Instance.GetRes((int) eResourcesType.Honor);

        var tbHonor = Table.GetHonor(ArenaDataModel.MilitaryRank);
        if (tbHonor == null)
        {
            return;
        }
        ArenaDataModel.NextMilitaryRank = tbHonor.NextRank;
        if (tbHonor.NeedHonor == -1)
        {
            ArenaDataModel.HonorProgressBar = 1;
        }
        else
        {
            ArenaDataModel.HonorProgressBar = 1f/tbHonor.NeedHonor
                                              *ArenaDataModel.HonorCount;
        }

        if (ArenaDataModel.HonorProgressBar >= 1 && tbHonor.NeedHonor != -1)
        {
            PlayerDataManager.Instance.NoticeData.ArenaMilitary = true;
        }
        else
        {
            PlayerDataManager.Instance.NoticeData.ArenaMilitary = false;
        }
        if (ArenaDataModel.MilitaryRank != 0)
        {
            {
                // foreach(var data in ArenaDataModel.RankList)
                var __enumerator13 = (ArenaDataModel.RankList).GetEnumerator();
                while (__enumerator13.MoveNext())
                {
                    var data = __enumerator13.Current;
                    {
                        data.Item.State = 1;
                        if (data.HonorId == ArenaDataModel.MilitaryRank)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void UpdatePetState(int petId)
    {
        if (petId == -1)
        {
            return;
        }

        var pet = CityManager.Instance.GetPetById(petId);

        PetItemDataModel petItemData = null;

        var flag = -1;
        var c = StatueDataModel.PetList.Count;
        for (var i = 0; i < c; i++)
        {
            var d = StatueDataModel.PetList[i];
            if (d.PetId == petId)
            {
                flag = i;
                break;
            }
        }
        if (flag == -1)
        {
            return;
        }

        var data = CityManager.PetItem2DataModel(pet);
        {
            for (var i = 0; i < data.Skill.SpecialSkills.Count; i++)
            {
                if (data.Skill.SpecialSkills[i].SkillId != -1)
                {
                    if (Table.GetPetSkill(data.Skill.SpecialSkills[i].SkillId).Param[0] == 6 &&
                        Table.GetPetSkill(data.Skill.SpecialSkills[i].SkillId).Param[1] == 1)
                    {
                        data.BuffIconIdlist[i].Active = true;
                        data.BuffIconIdlist[i].BuffId = Table.GetPetSkill(data.Skill.SpecialSkills[i].SkillId).SkillIcon;
                    }
                }
            }

            if (data.State == (int) PetStateType.Idle)
            {
                data.ShowCheck = true;
                data.ShowMask = false;
            }
            else
            {
                data.ShowCheck = false;
                data.ShowMask = true;
            }
            // foreach(var info in StatueDataModel.StatueInfos)
            var __enumerator10 = (StatueDataModel.StatueInfos).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var info = __enumerator10.Current;
                {
                    if (info.IsSelect)
                    {
                        if (data.ItemId == info.ItemId)
                        {
                            data.Checked = true;
                            data.ShowMask = false;
                            data.ShowCheck = true;
                        }
                        break;
                    }
                }
            }
        }

        StatueDataModel.PetList[flag] = data;
    }

    public void UpdateRank(int rank)
    {
        if (ArenaDataModel.CurrentRank == rank)
        {
            return;
        }
        ArenaDataModel.CurrentRank = rank;

        var isFind = false;
        Table.ForeachArenaReward(recoard =>
        {
            if (rank <= recoard.Id)
            {
                ArenaDataModel.RankIndex = recoard.Id;
                isFind = true;
                return false;
            }
            return true;
        });
        if (isFind == false)
        {
            ArenaDataModel.RankIndex = -1;
        }
    }

    public void UpdateStatusCount()
    {
        if (mTbBuildingService != null)
        {
            var tbVip = PlayerDataManager.Instance.TbVip;
            StatueDataModel.ChallengeCount = mTbBuildingService.Param[5] + tbVip.StatueAddCount -
                                             PlayerDataManager.Instance.GetExData(400);
            if (StatueDataModel.ChallengeCount > 0 && StatueDataModel.MaintainCd >= Game.Instance.ServerTime)
            {
                StatueDataModel.IsShowCd = true;
            }
            else
            {
                StatueDataModel.IsShowCd = false;
            }
            AnalyseNoticeStatue();
        }
    }

    public void UseBuildService(int type)
    {
        if (!CheckBuildService(type))
        {
            return;
        }
        NetManager.Instance.StartCoroutine(UseBuildServiceCoroutine(type));
    }

    private IEnumerator UseBuildServiceCoroutine(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var chellageCount = StatueDataModel.ChallengeCount;
            var selectStaue = StatueDataModel.SelectStatue;
            var ary = new Int32Array();
            ary.Items.Add(0);
            ary.Items.Add(selectStaue.Index);
            ary.Items.Add(type);
            var msg = NetManager.Instance.UseBuildService(BuildingData.AreaId, mTbBuilding.ServiceId, ary);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    chellageCount--;
                    PlayerDataManager.Instance.SetExData(400, chellageCount);
                    StatueDataModel.ChallengeCount = chellageCount;
                    var str = "";
                    switch (type)
                    {
                        case 0:
                        {
                            //膜拜成功
                            str = GameUtils.GetDictionaryText(270026);
                        }
                            break;
                        case 1:
                        {
                            //打扫成功
                            str = GameUtils.GetDictionaryText(270027);
                        }
                            break;
                        case 2:
                        {
                            //修复成功
                            str = GameUtils.GetDictionaryText(270028);
                        }
                            break;
                    }
                   // EventDispatcher.Instance.DispatchEvent(new UIEvent_ArenaFlyAnim(type,
                   //     selectStaue.MaintainItemExp[type]*mTbBuildingService.Param[6]/10000));
                    var e = new ShowUIHintBoard(str);
                    EventDispatcher.Instance.DispatchEvent(e);
                    AddStatusExp(selectStaue, selectStaue.MaintainItemExp[type]);
                    RefreshExpValue(selectStaue);
                    AnalyseNoticeStatue();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(".....UseBuildService...ErrorCode....{0}.", msg.ErrorCode);
                }
            }
            else
            {
                Logger.Error(".....UseBuildService...State....{0}.", msg.State);
            }
        }
    }

    public void CleanUp()
    {
        ArenaDataModel = new ArenaDataModel();
        StatueDataModel = new StatueDataModel();
        BuildingData = null;
        IsInit = false;
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        if (name == "PlayerDataModel")
        {
            return PlayerDataManager.Instance.PlayerDataModel;
        }
        if (name == "Arena")
        {
            return ArenaDataModel;
        }
        if (name == "Statue")
        {
            return StatueDataModel;
        }
        return null;
    }

    public void Close()
    {
        ArenaDataModel.ShowPetSkills = false;
        ArenaDataModel.TabPage = 0;
        PlayerDataManager.Instance.CloseCharacterPopMenu();
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

    public void RefreshData(UIInitArguments data)
    {
        Init();
        var args = data as ArenaArguments;
        if (args == null)
        {
            return;
        }
        if (args.Tab > 1)
        {
            return;
        }
        ArenaDataModel.TabPage = args.Tab;
        NetManager.Instance.StartCoroutine(ApplyArenaInfoCoroutine());
        RefreshArena();
        var buildingData = args.BuildingData;
        RefreshBuildingDataModel(buildingData);

        AnalyseNoticeArena();
        AnalyseNoticeStatue();

        SetSatueInfo(0);
        for (int i = 0; i < StatueDataModel.StatueInfos.Count; i++)
        {
            RefreshExpValue(StatueDataModel.StatueInfos[i]);
        }
        
        //重置页面显示状态
        var e = new ArenaNotifyLogic(0);
        EventDispatcher.Instance.DispatchEvent(e);

        ArenaDataModel.ShowPetSkills = false;
    }

    //-------------------------------------------------------------Base-------------------
    public FrameState State { get; set; }
}
