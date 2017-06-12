#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using GameUI;
using ScorpionNetLib;
using ServiceBase;
using Shared;
using UnityEngine;

#endregion

public class PlayerDataManager : Singleton<PlayerDataManager>, IManager
{
    private static readonly Dictionary<int, List<int>> DungeonEnterExData = new Dictionary<int, List<int>>();
    private static readonly Dictionary<int, List<int>> DungeonResetExData = new Dictionary<int, List<int>>();
    private static readonly CharacterBaseDataModel EmptyCharacterBaseDataModel = new CharacterBaseDataModel();

    public PlayerDataManager()
    {
        FlagInited = false;
        IsLevelInited = false;
        Reset();
    }

    public Dictionary<int, BattleCityData> _battleCityDic = new Dictionary<int, BattleCityData>(); // 攻城战 <战盟Id,数据>
    //活动状态，用来记住世界boss的状态(目前只用来记本次世界boss是否还能打)
    public Dictionary<int, int> ActivityState = new Dictionary<int, int>();
    //----------------------------------------------------------------AttributeFix------------------
    public Dictionary<int, int> AttributeFix = new Dictionary<int, int>
    {
        {106, 0},
        {111, 0},
        {113, 0},
        {114, 0},
        {119, 0},
        {120, 0}
    };

    //------------------------------------------------------------------Equip-----
    public Dictionary<int, int> BagIdToEquipType = new Dictionary<int, int>
    {
        {7, 0},
        {8, 1},
        {11, 2},
        {14, 5},
        {15, 6},
        {16, 7},
        {17, 8},
        {18, 9}
    };

    public string BattleName = "";
    public BattleUnionDataModel BattleUnionDataModel;
    public PlayerInfoMsg BattleMishiMaster;
    //战盟盟主信息
    public PlayerInfoMsg BattleUnionMaster;

    public Dictionary<int, int> CareeridToStatsPointIndex = new Dictionary<int, int>
    {
        {0, 0},
        {1, 1},
        {2, 2}
    };

    //用来存储退出到角色选择界面后请求的新数据,老数据被清除了,用新数据刷新界面
    public List<CharacterSimpleInfo> CharacterLists = new List<CharacterSimpleInfo>();

    public Dictionary<int, int> EquipTypeToPart = new Dictionary<int, int>
    {
        {0, 0},
        {1, 1},
        {2, 4},
        {3, 6},
        {4, 6},
        {5, 7},
        {6, 8},
        {7, 9},
        {8, 10},
        {9, 11}
    };

    public FirstChargeData FirstChargeData;
    public int LastLoginServerId;
    public Dictionary<int, SkillItemDataModel> mAllSkills;
    public Dictionary<int, TalentCellDataModel> mAllTalents;
    //-------------------------------------------------------------------------Bag-------
    private readonly Dictionary<int, TotalCount> mBagItemCountData = new Dictionary<int, TotalCount>();
    private Dictionary<int, int> mBookGropData = new Dictionary<int, int>();
    private List<int> mBountyBooks = new List<int>();
    public PlayerData mInitBaseAttr;
    private readonly BitFlag mLoginApplyState = new BitFlag((int) eLoginApplyType.Trade + 1);
    public object mPickIntervalTrigger = null;
    private int mServerId;
    public Dictionary<int, int> mSkillTalent;
    public Dictionary<ulong, CharacterBaseInfoDataModel> mUnionMembers; //战盟成员
    public NoticeDataModel NoticeData;
    public DateTime OpenTime; //本角色开服时间

    private readonly Dictionary<ulong, PlayerInfoMsgCache> PlayerInfoMsgCaches =
        new Dictionary<ulong, PlayerInfoMsgCache>();

    public ulong SelectedRoleIndex;
    public string ServerName;
    public DateTime CharacterFoundTime; //角色的创建时间
    //服务器id和name
    public Dictionary<int, string> ServerNames = new Dictionary<int, string>();
    public VIPRecord TbVip;
    public TeamDataModel TeamDataModel;
    public Dictionary<int, int> TitleList = new Dictionary<int, int>();
    public int TotalBountyCount = 0; // 怪物悬赏数量 
    public int TotalGroupCount = 0; //图鉴组合激活数量
    public string UidForPay;
    public WeakNoticeDataModel WeakNoticeData;
    public AccountDataModel AccountDataModel { get; set; }

    public Dictionary<int, int> BookGropData
    {
        get { return mBookGropData; }
    }

    public List<int> BountyBooks
    {
        get { return mBountyBooks; }
    }

    //玩家登录角色的唯一id
    public ulong CharacterGuid { get; set; }
    public List<int> ExtData { get; private set; }
    public List<long> ExtData64 { get; private set; }
    public BitFlag FlagData { get; private set; }
    public bool FlagInited { get; private set; }
    public ulong Guid { get; set; }
    public bool IsLevelInited { get; private set; }
    public string Password { get; set; }
    public PlayerDataModel PlayerDataModel { get; set; }
    //审核状态
    public int ReviewState
    {
        get { return PlayerDataModel.ReviewState; }
        set { PlayerDataModel.ReviewState = value; }
    }

    //---------------------------------------------------------------SelectTarget--------
    public SelectTargetDataModel SelectTargetData { get; set; }

    public int ServerId
    {
        get { return mServerId; }
        set
        {
            mServerId = value;
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SelectServer(mServerId));
        }
    }

    public string UserName { get; set; }

    private IEnumerator AcitvityCompensateCoroutine()
    {
        Logger.Info(".............ApplyBooks..................");
        var msg = NetManager.Instance.GetCompensationList(0);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        var controller = UIManager.Instance.GetController(UIConfig.RewardFrame);
        if (null != controller)
        {
            controller.CallFromOtherClass("InitCompensate", new[] {msg.Response.Compensations});
        }
    }

    public TotalCount AddBagItemCount(int bagType, int itemId)
    {
        if (itemId == -1)
        {
            return null;
        }
        //GameUtils.GotoUiTab(92, -1, itemId);
        var eBagType = (eBagType) bagType;
        if (eBagType == eBagType.BaseItem
            || eBagType == eBagType.Equip
            || eBagType == eBagType.Piece)
        {
            TotalCount totalCount;
            if (!mBagItemCountData.TryGetValue(itemId, out totalCount))
            {
                totalCount = new TotalCount();
                mBagItemCountData.Add(itemId, totalCount);
            }

            totalCount.Count = GetItemCount(bagType, itemId);
            return totalCount;
        }

        return null;
    }

    public void ApplyAcitvityCompensate()
    {
        NetManager.Instance.StartCoroutine(AcitvityCompensateCoroutine());
    }

    //---------------------------------------------------------------Activity--------
    public void ApplyActivityState()
    {
        NetManager.Instance.StartCoroutine(ApplyActivityStateCoroutine());
    }

    private IEnumerator ApplyActivityStateCoroutine()
    {
        Logger.Info(".............ApplyTrading..................");
        var msg = NetManager.Instance.ApplyActivityState(ServerId);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                var data = msg.Response.Data;
                foreach (var i in data)
                {
                    ActivityState[i.Key] = i.Value;
                }
                EventDispatcher.Instance.DispatchEvent(new ActivityStateChangedEvent());
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
            }
        }
    }

    public void ApplyBags()
    {
        NetManager.Instance.StartCoroutine(ApplyBagsCoroutine());
    }

    private IEnumerator ApplyBagsCoroutine()
    {
        Logger.Info(".............ApplyBags..................");
        var msg = NetManager.Instance.ApplyBags(0);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                SetLoginApplyState(eLoginApplyType.Bag);

                InitItemWithSkill();
                PlayerDataModel.Bags.Resources.Resources.Clear();
                var resCount = msg.Response.Resources.Count;
                for (var i = 0; i < resCount; i++)
                {
                    PlayerDataModel.Bags.Resources.Resources.Add(-1);
                    SetRes(i, msg.Response.Resources[i]);
                }

                OnInitLevel();
                var __enumerator7 = (msg.Response.Bags).GetEnumerator();
                while (__enumerator7.MoveNext())
                {
                    var bag = __enumerator7.Current;
                    {
                        var bagType = (eBagType) bag.Key;

                        if (bagType == eBagType.Wing)
                        {
                            var wingCon = UIManager.Instance.GetController(UIConfig.WingUI);
                            if (wingCon != null)
                            {
                                wingCon.CallFromOtherClass("InitWingItem", new[] {bag.Value});
                                //wingCon.InitWingItem(bag.Value);
                            }
                            continue;
                        }
                        if (bagType == eBagType.Elf)
                        {
                            var elfCon = UIManager.Instance.GetController(UIConfig.ElfUI);
                            if (elfCon != null)
                            {
                                elfCon.CallFromOtherClass("InitElfBag", new[] {bag.Value});
                                //elfCon.InitElfBag(bag.Value);  
                            }
                            continue;
                        }
                        if (bagType >= eBagType.Equip01 && bagType <= eBagType.Equip12)
                        {
                            InitEquipData(bag.Value);
                            continue;
                        }
                        if (bagType == eBagType.MedalBag || bagType == eBagType.MedalTemp ||
                            bagType == eBagType.MedalUsed)
                        {
                            var MedalCon = UIManager.Instance.GetController(UIConfig.SailingUI);
                            if (MedalCon != null)
                            {
                                MedalCon.CallFromOtherClass("InitMedalBag", new[] {bag.Value});
                            }
                        }
                        else if (bagType == eBagType.WishingPool)
                        {
                            var MedalCon = UIManager.Instance.GetController(UIConfig.WishingUI);
                            if (MedalCon != null)
                            {
                                MedalCon.CallFromOtherClass("InitWishingBag", new[] {bag.Value});
                            }
                        }
                        else if (bagType == eBagType.GemBag || bagType == eBagType.GemEquip)
                        {
                            var AstrCon = UIManager.Instance.GetController(UIConfig.AstrologyUI);
                            if (AstrCon != null)
                            {
                                AstrCon.CallFromOtherClass("InitAstrologyData", new[] {bag.Value});
                            }
                        }

                        InitBagData(bag.Value);

                        if (bagType == eBagType.Piece)
                        {
                            var HandBookCon = UIManager.Instance.GetController(UIConfig.HandBook);
                            if (null != HandBookCon)
                            {
                                HandBookCon.CallFromOtherClass("UpdateNotice", new[] {bag.Value});
                            }
                        }
                    }
                }
                CheckEquipDurable();
                EventDispatcher.Instance.DispatchEvent(new BagDataInitEvent());
                //设置背包的免费开包裹图标
                EventDispatcher.Instance.DispatchEvent(new SetBagFreeIconEvent());
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
            }
        }
    }

    public void ApplyBooks()
    {
        NetManager.Instance.StartCoroutine(ApplyBooksCoroutine());
    }

    private IEnumerator ApplyBooksCoroutine()
    {
        Logger.Info(".............ApplyBooks..................");
        var msg = NetManager.Instance.ApplyBooks(0);
        yield return msg.SendAndWaitUntilDone();

        //skills
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.Book);
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        var books = msg.Response.Books;
        var groups = msg.Response.Group;
        mBookGropData = new Dictionary<int, int>(groups);
        mBountyBooks = new List<int>(books);
        var controller = UIManager.Instance.GetController(UIConfig.HandBook);
        if (null != controller)
        {
            controller.CallFromOtherClass("UpdateNotice", new object[] {});
        }
    }

    //-------------------------------------------------------------------------City-------
    public void ApplyCityData()
    {
        NetManager.Instance.StartCoroutine(ApplyCityDataCoroutine());
    }

    private IEnumerator ApplyCityDataCoroutine()
    {
        Logger.Info(".............ApplyCityDataCoroutine..................");

        var msg = NetManager.Instance.ApplyCityData(-1);
        yield return msg.SendAndWaitUntilDone();
        if (MessageState.Reply != msg.State)
        {
            Logger.Info("NetSyncPostion:MessageState.Timeout");
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.City);
        if ((int) ErrorCodes.OK != msg.ErrorCode)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            Logger.Info("ApplyBuildingDataCoroutine error=[{0}]", msg.ErrorCode);
            yield break;
        }
        if (null == msg.Response)
        {
            Logger.Info("null==msg.Response");
            yield break;
        }
        CityManager.Instance.Reset();
        CityManager.Instance.BuildingDataList = msg.Response.Buildings.Data;
        CityManager.Instance.PetMissionDataList = msg.Response.Missions.Data;
        //解析随从任务名称
        var missionCount = msg.Response.Missions.Data.Count;
        for (var i = 0; i < missionCount; i++)
        {
            msg.Response.Missions.Data[i].Name = GameUtils.ServerStringAnalysis(msg.Response.Missions.Data[i].Name);
        }

        CityManager.Instance.UpdatePetMissionState(false);
        //潜规则一下，任务类型是必有的条件
        {
            foreach (var mission in CityManager.Instance.PetMissionDataList)
            {
                if (mission.ConditionIds.Count > 0)
                {
                    if (0 != mission.ConditionIds[1])
                    {
                        mission.ConditionIds.Insert(1, 0);
                        mission.ConditionParam.Insert(1, mission.Type);
                    }
                }
            }
        }


        CityManager.Instance.BuildingMissionList = msg.Response.CityMissions;
        CityManager.Instance.Inited = true;

        var e1 = new CityDataInitEvent();
        EventDispatcher.Instance.DispatchEvent(e1);
    }

    public IEnumerator ApplyEquipDurableCoroutine()
    {
        var msg = NetManager.Instance.ApplyEquipDurable(-1);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                PlayerDataModel.Bags.IsDuableChange = false;
                var ret = msg.Response.Data;
                {
                    // foreach(var i in ret)
                    var __enumerator1 = (ret).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var i = __enumerator1.Current;
                        {
                            var bagId = i.Key/10;
                            var bagIndex = i.Key%10;
                            var equipType = ChangeBagIdToEquipType(bagId, bagIndex);
                            if (equipType != -1)
                            {
                                var equipData = GetEquipData((eEquipType) equipType);
                                if (equipData != null)
                                {
                                    equipData.Exdata[22] = i.Value;
                                }
                            }
                        }
                    }
                }
                CheckEquipDurable();
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("ApplyEquipDurable Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("ApplyEquipDurable Error!............State..." + msg.State);
        }
    }

    ////请求审核状态
    //public void ApplytReviewState()
    //{
    //    NetManager.Instance.StartCoroutine(ApplytReviewStateCoroutine(0));
    //}

    ////请求标记位数据
    //private IEnumerator ApplytReviewStateCoroutine(int nId)
    //{
    //    Logger.Info(".............ApplyFlagData..................");
    //    var msg = NetManager.Instance.GetReviewState(nId);
    //    yield return msg.SendAndWaitUntilDone();
    //    if (msg.State != MessageState.Reply) yield break;
    //    if (msg.ErrorCode != (int)ErrorCodes.OK)
    //    {
    //        UIManager.Instance.ShowNetError(msg.ErrorCode);
    //        yield break;
    //    }
    //    ReviewState = msg.Response;
    //}

    //请求扩展数据数据
    public void ApplyExtData(int nId = -1)
    {
        NetManager.Instance.StartCoroutine(ApplyExtDataCoroutine(nId));
    }

    //---------------------------------------------------------------ExtData64----
    public void ApplyExtData64(int nId = -1)
    {
        NetManager.Instance.StartCoroutine(ApplyExtData64Coroutine(nId));
    }

    //请求扩展数据数据
    private IEnumerator ApplyExtData64Coroutine(int nId)
    {
        Logger.Info(".............ApplyExtData..................");
        var msg = NetManager.Instance.ApplyExdata64(nId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.Exdata64);
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        if (nId < 0)
        {
            ExtData64.Clear();
            ExtData64.AddRange(msg.Response.Items);
        }
        else
        {
            var ret = msg.Response.Items[0];
            ExtData64[nId] = ret;
        }
        OpenTime = Extension.FromServerBinary(ExtData64[(int) Exdata64TimeType.ServerStartTime]);
        var e = new ExData64InitEvent(ExtData);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    //请求扩展数据数据
    private IEnumerator ApplyExtDataCoroutine(int nId)
    {
        Logger.Info(".............ApplyExtData..................");
        var msg = NetManager.Instance.ApplyExdata(nId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.Exdata);
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        if (nId < 0)
        {
            ExtData.Clear();
            ExtData.AddRange(msg.Response.Items);
            PlayerAttr.Instance.AddPointRefresh();
            SetMainNotice();
            var e = new ExDataInitEvent(ExtData);
            EventDispatcher.Instance.DispatchEvent(e);
        }
        else
        {
            var ret = msg.Response.Items[0];
            SetExData(nId, ret);
        }
        RefleshRewardInfo(); //刷新主界面icon快捷键
        RefrehNoticeFlagLevelUpCondition();
    }

    //请求标记位数据
    public void ApplyFlagData(int nId = -1)
    {
        NetManager.Instance.StartCoroutine(ApplyFlagDataCoroutine(nId));
    }

    //请求标记位数据
    private IEnumerator ApplyFlagDataCoroutine(int nId)
    {
        Logger.Info(".............ApplyFlagData..................");
        var msg = NetManager.Instance.ApplyFlag(nId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.Flag);
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        if (nId < 0)
        {
            FlagData.ReSetAllFlag();
            FlagData.Init(msg.Response.Items);
            var e = new FlagInitEvent();
            EventDispatcher.Instance.DispatchEvent(e);
            RefrehNoticeFlagCondition();
            FlagInited = true;
        }
        else
        {
            var ret = msg.Response.Items[0];
            if (1 == ret)
            {
                FlagData.SetFlag(nId);
            }
            else
            {
                FlagData.CleanFlag(nId);
            }
        }
    }

    //-------------------------------------------------------------------------Mail-------
    public void ApplyMails()
    {
        NetManager.Instance.StartCoroutine(ApplyMailsCoroutine());
    }

    private IEnumerator ApplyMailsCoroutine()
    {
        var msg = NetManager.Instance.ApplyMails(-1);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            SetLoginApplyState(eLoginApplyType.Mail);
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                var mailCon = UIManager.Instance.GetController(UIConfig.MailUI);
                mailCon.CallFromOtherClass("AddMailData", new[] {msg.Response.Mails, (object) true});
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("ApplyMails Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("ApplyMails Error!............State..." + msg.State);
        }
    }

    public void ApplyMissions(int nId)
    {
        NetManager.Instance.StartCoroutine(ApplyMissionsCoroutine(nId));
    }

    private IEnumerator ApplyMissionsCoroutine(int nId)
    {
        Logger.Info(".............ApplyMission..................");
        var msg = NetManager.Instance.ApplyMission(nId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.Mission);
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        if (PlayerDataModel == null)
        {
            yield break;
        }
        if (nId < 0)
        {
            if (PlayerDataModel.Missions.Datas.Count > 0)
            {
                PlayerDataModel.Missions.Datas.Clear();
            }
        }
        {
            // foreach(var missionBaseData in msg.Response.Missions)
            var __enumerator1 = (msg.Response.Missions).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var missionBaseData = __enumerator1.Current;
                {
                    var missionBaseModel = new MissionBaseModel
                    {
                        MissionId = missionBaseData.Key
                    };
                    for (var i = 0; i != 5; ++i)
                    {
                        missionBaseModel.Exdata[i] = missionBaseData.Value.Exdata[i];
                    }
                    //missionBaseModel.Exdata.AddRange(missionBaseData.Value.Exdata);
                    PlayerDataModel.Missions.Datas.Add(missionBaseData.Key, missionBaseModel);
                }
            }
        }
        EventDispatcher.Instance.DispatchEvent(new Event_UpdateMissionData());
    }

    public void ApplyPlayerInfo(ulong uuid, Action<PlayerInfoMsg> action)
    {
        PlayerInfoMsgCache cache;

        var charTarget = ObjManager.Instance.FindObjById(uuid);

        if (charTarget)
        {
            if (charTarget.GetObjType() == OBJ.TYPE.OTHERPLAYER)
            {
                var otherPlayer = charTarget as ObjOtherPlayer;
                if (otherPlayer && otherPlayer.RobotId != 0ul)
                {
                    uuid = otherPlayer.RobotId;
                }
            }
        }

        if (charTarget == null //不在当前可视范围内
            && PlayerInfoMsgCaches.TryGetValue(uuid, out cache))
        {
            if (cache.Time.AddSeconds(GameUtils.PlayerInfoCacheTime) > Game.Instance.ServerTime)
            {
                action(cache.Info);
                return;
            }
            PlayerInfoMsgCaches.Remove(uuid);
        }
        NetManager.Instance.StartCoroutine(ApplyPlayerInfoCoroutine(uuid, action));
    }

    private IEnumerator ApplyPlayerInfoCoroutine(ulong characterId, Action<PlayerInfoMsg> action)
    {
        var msg = NetManager.Instance.ApplyPlayerInfo(characterId);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                var info = msg.Response;
                var cache = new PlayerInfoMsgCache
                {
                    Info = info,
                    Time = Game.Instance.ServerTime
                };
                PlayerInfoMsgCaches[characterId] = cache;
                action(info);
            }
            else
            {
                //得不到角色{0}信息  ErrorCode = {1}
                var str = string.Format(GameUtils.GetDictionaryText(270022), characterId, msg.ErrorCode);
                var e = new ShowUIHintBoard(str);
                EventDispatcher.Instance.DispatchEvent(e);
                Logger.Error("ApplyPlayerInfo Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            //得不到角色{0}信息  State = {1}
            var str = string.Format(GameUtils.GetDictionaryText(270023), characterId, msg.State);
            var e = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e);
            Logger.Error("ApplyPlayerInfo Error!............State..." + msg.State);
        }
    }

    //----------------------------------------------------------------Queue------------------
    public void ApplyQueueData()
    {
        NetManager.Instance.StartCoroutine(ApplyQueueDataCoroutine());
    }

    private IEnumerator ApplyQueueDataCoroutine()
    {
        Logger.Info(".............ApplyQuene..................");
        var msg = NetManager.Instance.ApplyQueueData(-1);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            SetLoginApplyState(eLoginApplyType.Quene);
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                var queneInfo = msg.Response;
                InitQueneData(queneInfo);
            }
            else if (msg.ErrorCode == (int) ErrorCodes.Unknow)
            {
                // do nothing.
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Info("ApplyQuene Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("ApplyQuene Error!............State..." + msg.State);
        }
    }

    //---------------------------------------------------------------Skill----
    public void ApplySkills()
    {
        NetManager.Instance.StartCoroutine(ApplySkillsCoroutine());
    }

    private IEnumerator ApplySkillsCoroutine()
    {
        Logger.Info(".............ApplySkills..................");
        var msg = NetManager.Instance.ApplySkill(0);
        yield return msg.SendAndWaitUntilDone();

        //skills
        if (msg.State != MessageState.Reply)
        {
            yield break;
        }
        SetLoginApplyState(eLoginApplyType.Skill);
        if (msg.ErrorCode != (int) ErrorCodes.OK)
        {
            UIManager.Instance.ShowNetError(msg.ErrorCode);
            yield break;
        }
        var skillData = PlayerDataModel.SkillData;
        // skillData.PlayerClass = PlayerDataManager.Instance.GetRoleId();
        if (PlayerDataModel != null && skillData.AllSkills.Count > 0)
        {
            skillData.AllSkills.Clear();
        }
        mAllSkills.Clear();

        {
            // foreach(var skill in msg.Response.Skill)
            var __enumerator15 = (msg.Response.Skill).GetEnumerator();
            while (__enumerator15.MoveNext())
            {
                var skill = __enumerator15.Current;
                {
                    var skillItem = new SkillItemDataModel();
                    skillItem.SkillId = skill.Key;
                    skillItem.SkillLv = skill.Value;
                    skillData.AllSkills.Add(skillItem);
                    mAllSkills.Add(skill.Key, skillItem);
                }
            }
        }
        skillData.RefreshSkill();

        var msgResponseEquipSkillsCount10 = msg.Response.EquipSkills.Count;
        for (var i = 0; i < msgResponseEquipSkillsCount10; i++)
        {
            if (i > 3)
            {
                break;
            }
            if (skillData.EquipSkills.Count <= i)
            {
                skillData.EquipSkills.Add(new SkillItemDataModel());
            }

            var skillId = msg.Response.EquipSkills[i];
            if (skillId == -1)
            {
                var skillEquip = new SkillItemDataModel();
                skillEquip.SkillId = -1;
                skillData.EquipSkills[i] = skillEquip;
                continue;
            }

            SkillItemDataModel otherSkill;
            mAllSkills.TryGetValue(skillId, out otherSkill);
            skillData.EquipSkills[i] = otherSkill;
        }

        //Talent 
        skillData.InitTalents(msg.Response.Innate);
        skillData.TalentCount = msg.Response.InnateCount;
        NoticeData.PlayerTalentCount = skillData.TalentCount;
        mSkillTalent = new Dictionary<int, int>(msg.Response.SkillCount);
        {
            // foreach(var i in mSkillTalent)
            var __enumerator17 = (mSkillTalent).GetEnumerator();
            while (__enumerator17.MoveNext())
            {
                var i = __enumerator17.Current;
                {
                    if (i.Value > 0)
                    {
                        NoticeData.SkillTalentStatus = true;
                        break;
                    }
                }
            }
        }

        RefrehEquipPriority();
        PlayerAttr.Instance.TalentRefresh();
    }

    //--------------------------------------------------------------Team
    public void ApplyTeam()
    {
        var e = new TeamApplyEvent();
        EventDispatcher.Instance.DispatchEvent(e);
    }

    //---------------------------------------------------------------trading----
    public void ApplyTrading()
    {
        NetManager.Instance.StartCoroutine(StoreOperationLookSelf());
    }

	//
	public void ApplyOperationActivity()
	{
		NetManager.Instance.StartCoroutine(ApplyOperationActivityCoroutine());
	}

	private IEnumerator ApplyOperationActivityCoroutine()
	{
		var msg = NetManager.Instance.ApplyOperationActivity(0);
		yield return msg.SendAndWaitUntilDone();
		if (msg.State != MessageState.Reply)
		{
			EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200005000));
			yield break;
		}

		if (msg.ErrorCode != 0)
		{
			UIManager.Instance.ShowNetError(msg.ErrorCode);
			yield break;
		}

		EventDispatcher.Instance.DispatchEvent(new OperationActivityDataInitEvent(msg.Response));
	}

    //属性转换 fromList toList toRefList
    public void AttrConvert(Dictionary<int, int> AttrList, int[] attr, int[] attrRef)
    {
        {
            // foreach(var i in AttrList)
            var __enumerator3 = (AttrList).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var i = __enumerator3.Current;
                {
                    if (i.Key < (int) eAttributeType.AttrCount)
                    {
                        attr[i.Key] = i.Value;
                    }
                    else
                    {
                        switch (i.Key)
                        {
                            case 105:
                            {
                                if (GetRoleId() != 1)
                                {
                                    attr[(int) eAttributeType.PhyPowerMin] += i.Value;
                                    attr[(int) eAttributeType.PhyPowerMax] += i.Value;
                                }
                                else
                                {
                                    attr[(int) eAttributeType.MagPowerMin] += i.Value;
                                    attr[(int) eAttributeType.MagPowerMax] += i.Value;
                                }
                            }
                                break;
                            case 106:
                            {
                                attrRef[(int) eAttributeType.MagPowerMin] += i.Value*100;
                                attrRef[(int) eAttributeType.MagPowerMax] += i.Value*100;
                                attrRef[(int) eAttributeType.PhyPowerMin] += i.Value*100;
                                attrRef[(int) eAttributeType.PhyPowerMax] += i.Value*100;
                            }
                                break;
                            case 110:
                            {
                                attr[(int) eAttributeType.PhyArmor] += i.Value;
                                attr[(int) eAttributeType.MagArmor] += i.Value;
                            }
                                break;
                            case 111:
                            {
                                attrRef[(int) eAttributeType.PhyArmor] += i.Value*100;
                                attrRef[(int) eAttributeType.MagArmor] += i.Value*100;
                            }
                                break;
                            case 113:
                            {
                                attrRef[(int) eAttributeType.HpMax] += i.Value*100;
                            }
                                break;
                            case 114:
                            {
                                attrRef[(int) eAttributeType.MpMax] += i.Value*100;
                            }
                                break;
                            case 119:
                            {
                                attrRef[(int) eAttributeType.Hit] += i.Value*100;
                            }
                                break;
                            case 120:
                            {
                                attrRef[(int) eAttributeType.Dodge] += i.Value*100;
                            }
                                break;
                        }
                    }
                }
            }
        }
    }

    public void BagItemCountChange(BagsChangeData bag)
    {
        if (PlayerDataModel.Bags.Bags.Count == 0)
        {
            return;
        }
        var newEquip = new List<KeyValuePair<int, int>>();
        var itemIdxs = new Dictionary<int, int>();
        var changeCount = new Dictionary<int, int>();
        {
            var __enumerator12 = (bag.BagsChange).GetEnumerator();
            while (__enumerator12.MoveNext())
            {
                var changeData = __enumerator12.Current;
                {
                    var bagId = changeData.Key;
                    if (bagId == (int) eBagType.Wing
                        || bagId == (int) eBagType.Elf
                        || bagId == (int) eBagType.MedalBag
                        || bagId == (int) eBagType.MedalUsed
                        || bagId == (int) eBagType.MedalTemp
                        || bagId == (int) eBagType.WishingPool)
                    {
                    }

                    else if (bagId >= (int) eBagType.Equip01 && bagId <= (int) eBagType.Equip12)
                    {
                        if (bagId == (int) eBagType.Equip07)
                        {
                            {
                                // foreach(var baseData in changeData.Value.ItemsChange)
                                var __enumerator22 = (changeData.Value.ItemsChange).GetEnumerator();
                                while (__enumerator22.MoveNext())
                                {
                                    var baseData = __enumerator22.Current;
                                    {
                                        var item = baseData.Value;
                                        var equipData = PlayerDataModel.EquipList[(int) eEquipType.RingL + baseData.Key];
                                        if (item.ItemId != -1)
                                        {
                                            changeCount.modifyValue(item.ItemId, 1);
                                        }
                                        if (equipData.ItemId != -1)
                                        {
                                            changeCount.modifyValue(equipData.ItemId, -1);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            ItemBaseData item = null;
                            var itemsChange = changeData.Value.ItemsChange;
                            if (itemsChange.Count > 0)
                            {
                                if (!itemsChange.TryGetValue(0, out item))
                                {
                                    Logger.Error("BagItemCountChange   Error");
                                    continue;
                                }
                                item = itemsChange[0];
                                var bagType = BagIdToEquipType[bagId];
                                var equipData = PlayerDataModel.EquipList[bagType];
                                if (item.ItemId != -1)
                                {
                                    changeCount.modifyValue(item.ItemId, 1);
                                }
                                if (equipData.ItemId != -1)
                                {
                                    changeCount.modifyValue(equipData.ItemId, -1);
                                }
                            }
                        }
                    }
                    else
                    {
                        var bagData = GetBag(bagId);
                        {
                            var __enumerator23 = (changeData.Value.ItemsChange).GetEnumerator();
                            while (__enumerator23.MoveNext())
                            {
                                var itemBaseData = __enumerator23.Current;
                                {
                                    var index = itemBaseData.Key;
                                    if (bagData == null || index >= bagData.Items.Count)
                                    {
                                        continue;
                                    }
                                    var bagItem = bagData.Items[index];
                                    var changeItem = itemBaseData.Value;
                                    var isAddPiece = false;
                                    if (bagId == (int) eBagType.Pet)
                                    {
//随从的数量在扩展数据当中
                                        if (bagItem.Exdata[3] == 0 && changeItem.Exdata[3] == 2)
                                        {
//获得随从
                                            GainNewItem(changeItem.ItemId, 1, index);
                                            isAddPiece = true;
                                        }
                                        else if (bagItem.Exdata[3] == changeItem.Exdata[3])
                                        {
                                            isAddPiece = true;
                                        }
                                        if (isAddPiece)
                                        {
                                            var chg = changeItem.Exdata[PetItemExtDataIdx.FragmentNum] -
                                                      bagItem.Exdata[PetItemExtDataIdx.FragmentNum];
                                            if (chg > 0)
                                            {
                                                GainNewPet(bagItem.ItemId, chg);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (bagItem.ItemId != -1)
                                        {
                                            changeCount.modifyValue(bagItem.ItemId, -bagItem.Count);
                                        }
                                        if (changeItem.ItemId != -1)
                                        {
                                            changeCount.modifyValue(changeItem.ItemId, changeItem.Count);
                                            if (bagId == (int) eBagType.Equip)
                                            {
                                                newEquip.Add(new KeyValuePair<int, int>(changeItem.ItemId,
                                                    changeItem.Index));
                                            }
                                            else
                                            {
                                                itemIdxs[changeItem.ItemId] = changeItem.Index;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        UpdateBagData(bagId, changeData.Value);
                    }
                }
            }
        }

        if (changeCount.Count > 0)
        {
            var __enumerator13 = (changeCount).GetEnumerator();
            while (__enumerator13.MoveNext())
            {
                var i = __enumerator13.Current;
                {
                    var e = new UIEvent_BagItemCountChange(i.Key, i.Value);
                    EventDispatcher.Instance.DispatchEvent(e);
                    if (i.Value > 0)
                    {
                        var itemId = i.Key;
                        var count = 0;
                        {
                            var __list29 = newEquip;
                            var __listCount29 = __list29.Count;
                            for (var __i29 = 0; __i29 < __listCount29; ++__i29)
                            {
                                var pair = __list29[__i29];
                                {
                                    if (pair.Key == itemId)
                                    {
                                        GainNewItem(i.Key, 1, pair.Value);
                                        count++;
                                        if (count == i.Value)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        if (i.Value > count)
                        {
                            GainNewItem(i.Key, i.Value, itemIdxs[i.Key]);
                        }
                    }
                }
            }
        }
    }

    public void ChangeAttributeHp(int value)
    {
        const int attributeId = (int)eAttributeType.HpNow;
        var oldValue = PlayerDataModel.Attributes[attributeId];
//         var attributeValue = value + oldValue;
//         var max = PlayerDataModel.Attributes[(int) eSceneSyncId.SyncHpMax];
//         if (attributeValue < 0)
//         {
//             attributeValue = 0;
//         }
//         else if(attributeValue > max)
//         {
//             attributeValue = max;
//         }
        PlayerDataModel.Attributes[attributeId] = value;
        UpdateAttributre(attributeId, oldValue, value);
    }

    public int ChangeBagIdToEquipType(int bagId, int index = 0)
    {
        var ret = -1;
        if (bagId == 13)
        {
            if (index == 0)
            {
                ret = 3;
            }
            else if (index == 1)
            {
                ret = 4;
            }
        }
        else
        {
            if (!BagIdToEquipType.TryGetValue(bagId, out ret))
            {
                ret = -1;
            }
        }
        return ret;
    }

    public int ChangeEquipTypeToPart(int type)
    {
        var part = 0;
        if (!EquipTypeToPart.TryGetValue(type, out part))
        {
            return -1;
        }
        return part;
    }

    public void ChangePkModel(int value = -1)
    {
        NetManager.Instance.StartCoroutine(ChangePkModelCoroutine(value));
    }

    private IEnumerator ChangePkModelCoroutine(int value)
    {
	    var rule = GameLogic.Instance.Scene.TableScene.PvPRule;
	    var tb = Table.GetPVPRule(rule);
	    if (null!=tb && PlayerDataManager.Instance.GetLevel() < tb.ProtectLevel)
	    {
			var str = string.Format(GameUtils.GetDictionaryText(100001180), tb.ProtectLevel);
			UIManager.Instance.ShowMessage(MessageBoxType.Ok, str);
		    yield break;
	    }

        Logger.Info(".............ChangePKModel..................");
        var msg = NetManager.Instance.ChangePKModel(value);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                SetPkModel(msg.Response);
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("ChangePKModel Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("ChangePKModel Error!............State..." + msg.State);
        }
    }

    //----------------------------------------------------------------Dungeon------------------
    public bool CheckDungeonEnter(int dungeonId)
    {
        var tbDungeon = Table.GetFuben(dungeonId);
        if (tbDungeon == null)
        {
            return false;
        }
        for (var i = 0; i < tbDungeon.NeedItemId.Count; i++)
        {
            var itemId = tbDungeon.NeedItemId[i];
            var itemCount = tbDungeon.NeedItemCount[i];
            if (itemId != -1 && itemCount > 0)
            {
                var count = GetItemCount(itemId);
                if (count < itemCount)
                {
                    //物品不足
                    var e = new ShowUIHintBoard(270002);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return false;
                }
            }
        }
        var condition = tbDungeon.EnterConditionId;
        if (condition != -1)
        {
            var dicCon = CheckCondition(condition);
            if (dicCon != 0)
            {
                var e = new ShowUIHintBoard(dicCon);
                EventDispatcher.Instance.DispatchEvent(e);
                return false;
            }
        }

        return true;
    }

    public void CheckEquipDurable()
    {
        var state = 0;
        ForeachEquip(item =>
        {
            var equipId = item.ItemId;
            if (equipId == -1)
            {
                return;
            }
            var tbEquip = Table.GetEquipBase(equipId);
            if (tbEquip == null)
            {
                return;
            }
            var dur = item.Exdata[22];

            if (dur == 0)
            {
                state = 2;
                return;
            }

            if (tbEquip.Durability >= dur*10)
            {
                if (state == 0)
                {
                    state = 1;
                }
            }
        });

        EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(state));
    }

    public eEquipLimit CheckItemEquip(int itemId)
    {
        var tbItem = Table.GetItemBase(itemId);
        var tbEqup = Table.GetEquipBase(itemId);

        return CheckItemEquip(tbItem, tbEqup);
    }

    public eEquipLimit CheckItemEquip(ItemBaseRecord tbItem, EquipBaseRecord tbEqup)
    {
        if (tbItem == null || tbEqup == null)
        {
            return eEquipLimit.Occupation;
        }
        var roleType = GetRoleId();
        if (roleType != tbEqup.Occupation)
        {
            return eEquipLimit.Occupation;
        }

        if (GetLevel() < tbItem.UseLevel)
        {
            return eEquipLimit.Level;
        }

        var tbEqupNeedAttrIdCount7 = tbEqup.NeedAttrId.Count;
        for (var i = 0; i < tbEqupNeedAttrIdCount7; i++)
        {
            if (tbEqup.NeedAttrId[i] != -1)
            {
                if (PlayerDataModel.Attributes[tbEqup.NeedAttrId[i]] < tbEqup.NeedAttrValue[i])
                {
                    return eEquipLimit.Attribute;
                }
            }
        }
        return eEquipLimit.OK;
    }

    public bool CheckLoginApplyState()
    {
        for (var i = 0; i <= (int) eLoginApplyType.Trade; i++)
        {
            var flag = mLoginApplyState.GetFlag(i);
            if (flag == 0)
            {
                return false;
            }
        }
        return true;
    }

    //是否和某人一队
    public bool CheckSameTeam(ulong guid)
    {
        if (TeamDataModel.TeamId == 0)
        {
            return false;
        }
        {
            // foreach(var i in TeamDataModel.TeamList)
            var __enumerator24 = (TeamDataModel.TeamList).GetEnumerator();
            while (__enumerator24.MoveNext())
            {
                var i = __enumerator24.Current;
                {
                    if (i.Guid == guid)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool CheckSameUnion(ulong objId)
    {
        if (BattleUnionDataModel.MyUnion.UnionID <= 0)
        {
            return false;
        }
        return mUnionMembers.ContainsKey(objId);

        //         foreach (var dataModel in BattleUnionDataMidel.MyUnion.MemberList)
        //         {
        //             if (dataModel.ID == objId)
        //             {
        //                 return true;
        //             }
        //         }
        //         return false;
    }

    public void CloseCharacterPopMenu()
    {
        var e = new Close_UI_Event(UIConfig.OperationList);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    // 比较两件装备，返回值说明了这两件装备是否是同一部位，fightValueAdd 表示 (item2-item1).FightValue
    public bool CompareEquips(BagItemDataModel equip1, BagItemDataModel equip2, out int fightValueAdd)
    {
        fightValueAdd = 0;

        if (equip1.ItemId != -1 && equip2.ItemId != -1)
        {
            var tbItem1 = Table.GetItemBase(equip1.ItemId);
            if (tbItem1.Type < 10000 || tbItem1.Type > 10099)
            {
//不是装备
                return false;
            }
            var tbItem2 = Table.GetItemBase(equip2.ItemId);
            if (tbItem2.Type < 10000 || tbItem2.Type > 10099)
            {
//不是装备
                return false;
            }
            var tbEquip1 = Table.GetEquipBase(tbItem1.Exdata[0]);
            var tbEquip2 = Table.GetEquipBase(tbItem2.Exdata[0]);
            if ((tbEquip1.Part & tbEquip2.Part) == 0)
            {
//不是同一部位的装备
                return false;
            }
        }

        fightValueAdd = equip2.FightValue - equip1.FightValue;
        return true;
    }

    public void CreateAttributeSync(ulong characterId)
    {
        for (var i = (int) eSceneSyncId.SyncLevel; i < (int) eSceneSyncId.SyncMax; i++)
        {
            if (i >= (int)eSceneSyncId.Count && i < (int)eSceneSyncId.SyncCountNext)
            {
                continue;
            }
            var i1 = i;

            if (i1 == (int) eSceneSyncId.SyncAllianceName)
            {
                NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) i,
                    (string name) =>
                    {
                        var objMyPlayer = ObjManager.Instance.MyPlayer;
                        if (name != null)
                        {
                            BattleName = name;
                        }
                        if (objMyPlayer)
                        {
                            objMyPlayer.AllianceName = name;
                            objMyPlayer.NameBoardUpdate();
                        }
                    });

                continue;
            }
            NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Scene, characterId, (uint) i,
                (int attributeValue) =>
                {
                    var objMyPlayer = ObjManager.Instance.MyPlayer;

                    if (i1 == (int) eSceneSyncId.SyncFightValue)
                    {
                        var old = PlayerDataModel.Attributes.FightValue;
                        if (old > 0 && old != attributeValue)
                        {
                            var e1 = new Show_UI_Event(UIConfig.ForceUI,
                                new ForceArguments {OldValue = old, NewValue = attributeValue});
                            EventDispatcher.Instance.DispatchEvent(e1);
                        }
                        PlayerDataModel.Attributes.FightValue = attributeValue;
                    }
                    else if (i1 == (int) eSceneSyncId.SyncAreaState)
                    {
                        var state = attributeValue;
                        mInitBaseAttr.AreaState = state;
                        var myPlayer = objMyPlayer;
                        if (null != myPlayer && myPlayer.AreaState != (eAreaState) state)
                        {
                            myPlayer.AreaState = (eAreaState) state;
                            // 如果在播放技能，就不要打断了
                            // 等切换到下一个状态，会自己把动作刷新了
                            if (myPlayer.GetCurrentStateName() != OBJ.CHARACTER_STATE.ATTACK)
                            {
                                myPlayer.RefreshAnimation();
                            }
//                             if (myPlayer.IsInSafeArea())
//                             {
//                                 var e = new MainUISwithState(false);
//                                 EventDispatcher.Instance.DispatchEvent(e);
//                             }
//                             else
//                             {
//                                 vawwr e = new MainUISwithState(true);
//                                 EventDispatcher.Instance.DispatchEvent(e);
//                             }
                        }
                    }
                    else if (i1 == (int) eSceneSyncId.SyncPkModel)
                    {
                        SetPkModel(attributeValue);
                    }
                    else if (i1 == (int) eSceneSyncId.SyncReborn)
                    {
                        var charBaseData = objMyPlayer.CharacterBaseData;
                        if (charBaseData != null)
                        {
                            charBaseData.Reborn = attributeValue;
                        }
                    }
                    else if (i1 == (int) eSceneSyncId.SyncPkValue)
                    {
                        SetPkValue(attributeValue);
                        if (objMyPlayer)
                        {
                            objMyPlayer.RefreshnameBarod();
                        }
                    }
                    else if (i1 == (int) eSceneSyncId.SyncTitle0
                             || i1 == (int) eSceneSyncId.SyncTitle1
                             || i1 == (int) eSceneSyncId.SyncTitle2
                             || i1 == (int) eSceneSyncId.SyncTitle3
                        )
                    {
                        var ii = i1 - (int) eSceneSyncId.SyncTitle0;

                        if (TitleList.ContainsKey(ii))
                        {
                            TitleList[ii] = attributeValue;
                        }
                        else
                        {
                            TitleList.Add(i1 - (int) eSceneSyncId.SyncTitle0, attributeValue);
                        }

                        if (objMyPlayer)
                        {
                            objMyPlayer.NameBoardUpdate();
                            //同步刷新称号页面
                            //var oldValue = PlayerDataModel.Attributes[i1];
                            //var e = new Attr_TitleChange_Event(i1 - (int)eSceneSyncId.SyncTitle0, oldValue,
                            //    attributeValue);
                            //EventDispatcher.Instance.DispatchEvent(e);
                        }
                    }
                    else
                    {
                        //人物属性
                        var attributeId = i1;
                        var oldValue = PlayerDataModel.Attributes[attributeId];
                        if (oldValue == attributeValue)
                        {
                            return;
                        }

                        PlayerDataModel.Attributes[attributeId] = attributeValue;

                        UpdateAttributre(attributeId, oldValue, attributeValue);
                        if (i1 == (uint) eSceneSyncId.SyncMoveSpeed)
                        {
                            var moveSpeed = PlayerDataModel.Attributes.MoveSpeed*ObjCharacter.MOVESPEED_RATE;
                            if (null != objMyPlayer)
                            {
                                objMyPlayer.SetMoveSpeed(moveSpeed);
                            }
                        }
                    }
                });
        }
    }

    public void CreateResourcesSync(ulong characterId)
    {
        for (var i = 0; i < (int) eResourcesType.CountRes; i++)
        {
            var type = i;
            NetManager.Instance.SyncCenter.RequestSyncData(ServiceType.Logic, characterId, (uint) i,
                value => { SetRes(type, value); });
        }
    }

    public void CreateSysnc(ulong characterId)
    {
        CreateAttributeSync(characterId);
        CreateResourcesSync(characterId);
    }

    //属性转换 fromList toList toRefList
    public void ElfAttrConvert(Dictionary<int, int> AttrList, int[] attr, int[] attrRef)
    {
        {
            // foreach(var i in AttrList)
            var __enumerator3 = (AttrList).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var i = __enumerator3.Current;
                {
                    if (i.Key < (int) eAttributeType.AttrCount)
                    {
                        attr[i.Key] = i.Value;
                    }
                    else
                    {
                        switch (i.Key)
                        {
                            case 105:
                            {
                                if (GetRoleId() != 1)
                                {
                                    attr[(int) eAttributeType.PhyPowerMin] += i.Value;
                                    attr[(int) eAttributeType.PhyPowerMax] += i.Value;
                                }
                                else
                                {
                                    attr[(int) eAttributeType.MagPowerMin] += i.Value;
                                    attr[(int) eAttributeType.MagPowerMax] += i.Value;
                                }
                            }
                                break;
                            case 106:
                            {
                                attrRef[(int) eAttributeType.MagPowerMin] += i.Value;
                                attrRef[(int) eAttributeType.MagPowerMax] += i.Value;
                                attrRef[(int) eAttributeType.PhyPowerMin] += i.Value;
                                attrRef[(int) eAttributeType.PhyPowerMax] += i.Value;
                            }
                                break;
                            case 110:
                            {
                                attr[(int) eAttributeType.PhyArmor] += i.Value;
                                attr[(int) eAttributeType.MagArmor] += i.Value;
                            }
                                break;
                            case 111:
                            {
                                attrRef[(int) eAttributeType.PhyArmor] += i.Value;
                                attrRef[(int) eAttributeType.MagArmor] += i.Value;
                            }
                                break;
                            case 113:
                            {
                                attrRef[(int) eAttributeType.HpMax] += i.Value;
                            }
                                break;
                            case 114:
                            {
                                attrRef[(int) eAttributeType.MpMax] += i.Value;
                            }
                                break;
                            case 119:
                            {
                                attrRef[(int) eAttributeType.Hit] += i.Value;
                            }
                                break;
                            case 120:
                            {
                                attrRef[(int) eAttributeType.Dodge] += i.Value;
                            }
                                break;
                        }
                    }
                }
            }
        }
    }

    public BagItemDataModel GetBagItemByItemId(int bagId, int itemId)
    {
        var enumerator = EnumBagItem(bagId).GetEnumerator();
        while (enumerator.MoveNext())
        {
            var tempItem = enumerator.Current;
            if (tempItem != null && tempItem.ItemId == itemId && tempItem.Count > 0)
            {
                return tempItem;
            }
        }
        return null;
    }

    //遍历某个包裹
    public IEnumerable<BagItemDataModel> EnumBagItem(int nBagId)
    {
        var bag = GetBag(nBagId);
        if (null == bag)
        {
            yield break;
        }
        {
            // foreach(var item in bag.Items)
            var __enumerator10 = (bag.Items).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var item = __enumerator10.Current;
                {
                    if (-1 != item.ItemId)
                    {
                        yield return item;
                    }
                }
            }
        }
    }

    //枚举装备
    public IEnumerable<BagItemDataModel> EnumEquip()
    {
        var inteEquipTypeCount3 = (int) eEquipType.Count;
        for (var i = (int) eEquipType.Begin; i < inteEquipTypeCount3; ++i)
        {
            var equip = PlayerDataModel.EquipList[i];
            if (null != equip)
            {
                if (-1 != equip.ItemId)
                {
                    yield return equip;
                }
            }
        }
    }

    public void EquipDurableBroken(int bagId, int index, int value)
    {
        var equipType = ChangeBagIdToEquipType(bagId, index);
        var item = GetEquipData((eEquipType) equipType);
        if (item == null || item.Exdata.Count < 23)
        {
            return;
        }
        item.Exdata[22] = value;
        var tbEquip = Table.GetEquipBase(item.ItemId);
        if (value <= 0)
        {
            EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(2));
        }
        else if (tbEquip.Durability >= value*10)
        {
            EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(1));
        }
    }

    //找到身上穿的同部位（跟装备 itemId 相同的）所有装备
    public List<BagItemDataModel> FindEquipedEquips(int itemId)
    {
        var equips = new List<BagItemDataModel>();
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem.Type < 10000 || tbItem.Type > 10099)
        {
//不是装备
            return equips;
        }
        var tbEquip = Table.GetEquipBase(tbItem.Exdata[0]);
        var bagType = (eBagType) GameUtils.GetEquipBagId(tbEquip);
        var equipList = PlayerDataModel.EquipList;
        switch (tbItem.Type)
        {
            case 10006:
            {
//戒指
                for (var type = eEquipType.RingL; type <= eEquipType.RingR; ++type)
                {
                    var equip = equipList[(int) type];
                    equips.Add(equip);
                }
            }
                break;
            case 10010:
            {
//主手
                var equip = equipList[(int) eEquipType.WeaponMain];
                equips.Add(equip);
            }
                break;
            case 10011:
            {
//副手
                var equip = equipList[(int) eEquipType.WeaponMain];
                //先看主手是不是双手武器
                if (equip.ItemId != -1)
                {
                    var tbItem1 = Table.GetItemBase(equip.ItemId);
                    if (tbItem1 != null && tbItem1.Type == 10099)
                    {
//是双手的，则把主手武器加入列表
                        equips.Add(equip);
                        break;
                    }
                }
                //否则，直接把副手武器加入列表，不管副手装没装武器
                equip = equipList[(int) eEquipType.WeaponScend];
                equips.Add(equip);
            }
                break;
            case 10098:
            {
//单手
                var equip = equipList[(int) eEquipType.WeaponMain];
                //先看主手是不是双手武器
                if (equip.ItemId != -1)
                {
                    var tbItem1 = Table.GetItemBase(equip.ItemId);
                    if (tbItem1 != null && tbItem1.Type == 10099)
                    {
//是双手的，则把主手武器加入列表
                        equips.Add(equip);
                        break;
                    }
                }
                //否则，把主副手武器都加入列表，不管有没有装备武器
                equips.Add(equip);
                equip = equipList[(int) eEquipType.WeaponScend];
                equips.Add(equip);
            }
                break;
            case 10099:
            {
//双手
                var equip = equipList[(int) eEquipType.WeaponMain];
                equips.Add(equip);
            }
                break;
            default:
            {
                var equipType = BagIdToEquipType[(int) bagType];
                var equip = equipList[equipType];
                if (equip.ItemId != -1)
                {
                    equips.Add(equip);
                }
            }
                break;
        }
        {
            var __list28 = equips;
            var __listCount28 = __list28.Count;
            for (var __i28 = 0; __i28 < __listCount28; ++__i28)
            {
                var equip = __list28[__i28];
                {
                    if (equip.ItemId != -1 && equip.FightValue == 0)
                    {
                        GetBagItemFightPoint(equip);
                    }
                }
            }
        }
        return equips;
    }

    //找出 equips 里最差的那件装备
    public BagItemDataModel FindWorstEquip(BagItemDataModel bagItem)
    {
        var equipedEquips = FindEquipedEquips(bagItem.ItemId);
        //将 equipedEquips 里面最差的那个替换成 bagItem
        if (equipedEquips.Count == 0)
        {
            equipedEquips.Add(bagItem);
        }
        else if (bagItem != null)
        {
            if (equipedEquips.Count == 0)
            {
                equipedEquips[0] = bagItem;
            }
            else
            {
                var id = FindWorstEquipIndex(equipedEquips);
                equipedEquips[id] = bagItem;
            }
        }
        var id1 = FindWorstEquipIndex(equipedEquips);
        if (id1 < 0)
        {
            return null;
        }
        return equipedEquips[id1];
    }

    //找出 equips 里最差的那件装备的 index
    public int FindWorstEquipIndex(List<BagItemDataModel> equips)
    {
        var worstId = -1;
        var lowestFightValue = int.MaxValue;
        for (int i = 0, imax = equips.Count; i < imax; ++i)
        {
            var f = equips[i].FightValue;
            if (f < lowestFightValue)
            {
                lowestFightValue = f;
                worstId = i;
            }
        }
        return worstId;
    }

    public static int FixAttrubuteRatio(int attrId, int attrValue)
    {
        switch (attrId)
        {
            case 15:
            case 17:
            case 21:
            case 22:
            case 23:
            case 24:
            case 106:
            case 111:
            case 113:
            case 114:
            case 119:
            case 120:
            {
                return attrValue/100;
            }
                break;
            case 16:
            case 19:
            {
                return attrValue/10000;
            }
                break;
        }
        return attrValue;
    }

    public void ForeachEquip(Action<BagItemDataModel> act)
    {
        var inteEquipTypeCount5 = (int) eEquipType.Count;
        for (var i = 0; i < inteEquipTypeCount5; i++)
        {
            var item = PlayerDataModel.EquipList[i];
            act(item);
        }
    }

    public void ForeachEquip2(Func<BagItemDataModel, bool> act)
    {
        var inteEquipTypeCount5 = (int)eEquipType.Count;
        for (var i = 0; i < inteEquipTypeCount5; i++)
        {
            var item = PlayerDataModel.EquipList[i];
            if (!act(item))
                break;
        }
    }

    public void ForeachEquip(List<eEquipType> order, Action<BagItemDataModel> act)
    {
        {
            var __list30 = order;
            var __listCount30 = __list30.Count;
            for (var __i30 = 0; __i30 < __listCount30; ++__i30)
            {
                var i = __list30[__i30];
                {
                    var item = PlayerDataModel.EquipList[(int) i];
                    act(item);
                }
            }
        }
    }

    public void GainNewItem(int itemId, int count, int index)
    {
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null)
        {
            Logger.Error("GainNewItem GetItemBase == null itemId ={0}", itemId);
            return;
        }
        //var tbDic = Table.GetDictionary(210200);
        var tbDIcStr = GameUtils.GetDictionaryText(210200);
        var name = tbItem.Name;
        if (tbItem.Quality > 0)
        {
            var tbColor = Table.GetColorBase(tbItem.Quality);
            if (tbColor == null)
            {
                Logger.Error("GainNewItem GetColorBase == null Quality ={0}", tbItem.Quality);
                return;
            }
            name = string.Format("[{0:X2}{1:X2}{2:X2}]{3}[-]", tbColor.Red, tbColor.Green, tbColor.Blue, name);
        }
        var text = string.Format(tbDIcStr, name, count);
        var e = new ShowUIHintBoard(text);
        EventDispatcher.Instance.DispatchEvent(e);

        if (GameUtils.ShowEquipModel(itemId))
        {
            return;
        }

        // 显示hint对话框
        if (tbItem.GetShowTip != 1)
        {
            return;
        }
        if (tbItem.InitInBag < 0)
        {
            return;
        }
        if (tbItem.UseLevel > PlayerDataModel.Bags.Resources.Level)
        {
            return;
        }
        if (GetGainItemHintEntryArgs(itemId, index) == null)
        {
            return;
        }

        // 模型展示时，不显示当前物品的快捷使用
        EventDispatcher.Instance.DispatchEvent(new ShowItemHint(itemId, index));
    }

    public void GainNewPet(int itemId, int count)
    {
        var tbPet = Table.GetPet(itemId);
        if (tbPet == null)
        {
            return;
        }
        var petItem = tbPet.NeedItemId;
        var tbItem = Table.GetItemBase(petItem);
        if (tbItem == null)
        {
            Logger.Error("GainNewPet GetItemBase == null itemId ={0}", itemId);
            return;
        }
        var name = tbItem.Name;
        if (tbItem.Quality > 0)
        {
            var tbColor = Table.GetColorBase(tbItem.Quality);
            if (tbColor == null)
            {
                Logger.Error("GainNewItem GetColorBase == null Quality ={0}", tbItem.Quality);
                return;
            }
            name = string.Format("[{0:X2}{1:X2}{2:X2}]{3}[-]", tbColor.Red, tbColor.Green, tbColor.Blue, name);
        }
        var text = string.Format(GameUtils.GianItemTip.Desc[0], name, count);
        var e = new ShowUIHintBoard(text);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public int GetAttrFightPoint(Dictionary<int, int> fightAttr, int level = -1, int careerId = -1)
    {
        if (level == -1)
        {
            level = GetLevel();
        }
        if (careerId == -1)
        {
            careerId = GetRoleId();
        }
        var talentData = new int[(int) eAttributeType.AttrCount];
        var talentDataRef = new int[(int) eAttributeType.AttrCount];
        AttrConvert(fightAttr, talentData, talentDataRef);
        var fightPoint = GetFightPoint(talentData, talentDataRef, level, careerId);
        return fightPoint;
    }

    //获取玩家的属性
    public int GetAttribute(int type)
    {
        if (type < 0 || type >= (int) eAttributeType.AttrCount)
        {
            return -1;
        }
        return PlayerDataModel.Attributes[type];
    }

    //获取玩家的属性
    public int GetAttribute(eAttributeType type)
    {
        if (type >= eAttributeType.Count && type < eAttributeType.CountNext)
        {
            return -1;
        }

        if (type < 0 || type >= eAttributeType.AttrCount)
        {
            return -1;
        }
        return PlayerDataModel.Attributes[(int) type];
    }

    //获取某个包裹
    public BagBaseDataModel GetBag(int nBagId)
    {
        BagBaseDataModel bag;
        if (PlayerDataModel.Bags.Bags.TryGetValue(nBagId, out bag))
        {
            return bag;
        }
        return null;
    }

    public int GetBagItemFightPoint(BagItemDataModel bagItem, int level = -1)
    {
        if (bagItem.ItemId == -1)
        {
            bagItem.FightValue = 0;
            return bagItem.FightValue;
        }
        var tbItem = Table.GetItemBase(bagItem.ItemId);
        if (tbItem.Type < 10000 || tbItem.Type > 10099)
        {
            bagItem.FightValue = 0;
            return bagItem.FightValue;
        }

        var attributes = GetEquipAttributeFix(bagItem, level);
        var fightPoint = GetAttrFightPoint(attributes, level);
        bagItem.FightValue = fightPoint;
        return fightPoint;
    }

    public int GetBaseValueRef(int attrId, int value, int level)
    {
        var attributeType = (eAttributeType) attrId;
        if (attributeType >= eAttributeType.Count && attributeType < eAttributeType.CountNext)
        {
            return 0;
        }
        
        var tblevel = Table.GetLevelData(level);
        if (tblevel == null)
        {
            return value;
        }
        switch (attributeType)
        {
            case eAttributeType.Level:
                break;
            case eAttributeType.Strength:
                break;
            case eAttributeType.Agility:
                break;
            case eAttributeType.Intelligence:
                break;
            case eAttributeType.Endurance:
                break;
            case eAttributeType.PhyPowerMin:
            {
                return value*tblevel.PhyPowerMinScale/100 + tblevel.PhyPowerMinFix;
            }
                break;
            case eAttributeType.PhyPowerMax:
            {
                return value*tblevel.PhyPowerMaxScale/100 + tblevel.PhyPowerMaxFix;
            }
                break;
            case eAttributeType.MagPowerMin:
            {
                return value*tblevel.MagPowerMinScale/100 + tblevel.MagPowerMinFix;
            }
                break;
            case eAttributeType.MagPowerMax:
            {
                return value*tblevel.MagPowerMaxScale/100 + tblevel.MagPowerMaxFix;
            }
                break;
            case eAttributeType.AddPower:
                break;
            case eAttributeType.PhyArmor:
            {
                return value*tblevel.PhyArmorScale/100 + tblevel.PhyArmorFix;
            }
                break;
            case eAttributeType.MagArmor:
            {
                return value*tblevel.MagArmorScale/100 + tblevel.MagArmorFix;
            }
                break;
            case eAttributeType.DamageResistance:
                break;
            case eAttributeType.HpMax:
            {
                return value*tblevel.HpMaxScale/100 + tblevel.HpMaxFix;
            }
                break;
            case eAttributeType.MpMax:
                break;
            case eAttributeType.LuckyPro:
                break;
            case eAttributeType.LuckyDamage:
                break;
            case eAttributeType.ExcellentPro:
                break;
            case eAttributeType.ExcellentDamage:
                break;
            case eAttributeType.Hit:
                break;
            case eAttributeType.Dodge:
                break;
            case eAttributeType.DamageAddPro:
                break;
            case eAttributeType.DamageResPro:
                break;
            case eAttributeType.DamageReboundPro:
                break;
            case eAttributeType.IgnoreArmorPro:
                break;
            case eAttributeType.MoveSpeed:
                break;
            case eAttributeType.HitRecovery:
                break;
            case eAttributeType.FireAttack:
            case eAttributeType.IceAttack:
            case eAttributeType.PoisonAttack:
            case eAttributeType.FireResistance:
            case eAttributeType.IceResistance:
            case eAttributeType.PoisonResistance:
                break;
            case eAttributeType.HpNow:
                break;
            case eAttributeType.MpNow:
                break;
            case eAttributeType.AttrCount:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return value;
    }

    public bool GetBookGropEnable(int groupId, int index)
    {
        int data;
        if (mBookGropData.TryGetValue(groupId, out data))
        {
            return Convert.ToBoolean((data >> index) & 1);
        }
        return false;
    }

    public bool GetBountyBookEnable(int bookId)
    {
        return mBountyBooks.Contains(bookId);
    }

    //-------------------------------------------------CharacterBase--------------
    public CharacterBaseDataModel GetCharacterBaseData()
    {
        return PlayerDataModel.CharacterBase;
    }

    public PlayerInfoMsg GetCharacterSimpleInfo(ulong id, bool isTime = false)
    {
        PlayerInfoMsgCache cache;
        if (PlayerInfoMsgCaches.TryGetValue(id, out cache))
        {
            if (cache.Time.AddSeconds(60*5) > Game.Instance.ServerTime || isTime == false)
            {
                return cache.Info;
            }
        }
        return null;
    }

    public int GetCurrentEquipSkillCount()
    {
        var count = 0;
        var equipskills = PlayerDataModel.SkillData.EquipSkills;
        var c = equipskills.Count;

        for (var j = 0; j < c; j++)
        {
            var equip = equipskills[j];
            if (equip.SkillId > 0)
            {
                count++;
            }
        }
        return count;
    }

    public PlayerData GetData()
    {
        return mInitBaseAttr;
    }

    public int GetElfAttrFightPoint(Dictionary<int, int> fightAttr, int level = -1, int careerId = -1)
    {
        if (level == -1)
        {
            level = GetLevel();
        }
        if (careerId == -1)
        {
            careerId = GetRoleId();
        }
        var talentData = new int[(int) eAttributeType.AttrCount];
        var talentDataRef = new int[(int) eAttributeType.AttrCount];
        ElfAttrConvert(fightAttr, talentData, talentDataRef);
        var fightPoint = GetElfFightPoint(talentData, talentDataRef, level, careerId);
        return fightPoint;
    }

    public int GetElfFightPoint(int[] attr, int[] attrRef, int level, int careerId)
    {
        //var level = GetLevel();
        var tbLevel = Table.GetLevelData(level);
        if (tbLevel == null)
        {
            return 0;
        }
        var FightPoint = 0;
        for (var type = eAttributeType.PhyPowerMin; type != eAttributeType.HitRecovery; ++type)
        {
            //基础固定属性
            var nValue = attr[(int) type];
            switch ((int) type)
            {
                case 15:
                {
                    FightPoint += nValue*tbLevel.LuckyProFightPoint/10000;
                }
                    break;
                case 17:
                {
                    FightPoint += nValue*tbLevel.ExcellentProFightPoint/10000;
                }
                    break;
                case 21:
                {
                    FightPoint += nValue*tbLevel.DamageAddProFightPoint/10000;
                }
                    break;
                case 22:
                {
                    FightPoint += nValue*tbLevel.DamageResProFightPoint/10000;
                }
                    break;
                case 23:
                {
                    FightPoint += nValue*tbLevel.DamageReboundProFightPoint/10000;
                }
                    break;
                case 24:
                {
                    FightPoint += nValue*tbLevel.IgnoreArmorProFightPoint/10000;
                }
                    break;
                default:
                {
                    var tbState = Table.GetStats((int) type);
                    if (tbState == null)
                    {
                        continue;
                    }

                    if (careerId == -1)
                    {
                        var roleId = GetRoleId();
                        if (CareeridToStatsPointIndex.ContainsValue(roleId))
                        {
                            FightPoint += tbState.FightPoint[CareeridToStatsPointIndex[roleId]]*nValue/100;
                        }
                        else
                        {
                            Logger.Error(" CareeridToStatsPointIndex2  error {0}", careerId);
                        }
                    }
                    else if (careerId == -2)
                    {
// -2 表示宠物
                        FightPoint += tbState.PetFight*nValue/100;
                    }
                    else
                    {
                        Logger.Error(" CareeridToStatsPointIndex2  error {0}", careerId);
                    }
                }
                    break;
            }
        }

        //百分比计算
        FightPoint += attrRef[(int) eAttributeType.MagPowerMin]*tbLevel.PowerFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.PhyArmor]*tbLevel.ArmorFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.HpMax]*tbLevel.HpFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.MpMax]*tbLevel.MpFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.Hit]*tbLevel.HitFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.Dodge]*tbLevel.DodgeFightPoint/10000;
        return FightPoint;
    }

    public Dictionary<int, int> GetEquipAttributeFix(BagItemDataModel bagItem, int characterLevel = -1)
    {
        var attributes = new Dictionary<int, int>();
        var tbItem = Table.GetItemBase(bagItem.ItemId);
        var tbEquip = Table.GetEquipBase(tbItem.Exdata[0]);

        var enchanceLevel = 0;//bagItem.Exdata[0];
        if (characterLevel == -1)
        {
            characterLevel = GetLevel();
        }

        for (var i = 0; i < 4; i++)
        {
            var attrId = tbEquip.BaseAttr[i];
            if (attrId == -1)
            {
                continue;
            }
            var baseValue = tbEquip.BaseValue[i];
            baseValue = GetBaseValueRef(attrId, baseValue, enchanceLevel);
            ModifyEquipAttribute(attributes, attrId, baseValue, characterLevel);
        }

        for (var i = 0; i < 2; i++)
        {
            var attrId = tbEquip.BaseFixedAttrId[i];
            var baseValue = tbEquip.BaseFixedAttrValue[i];

            if (attrId != -1 && baseValue > 0)
            {
                ModifyEquipAttribute(attributes, attrId, baseValue, characterLevel);
            }
        }

        for (var i = 0; i < 4; i++)
        {
            var baseValue = bagItem.Exdata[2 + i];
            var attrId = tbEquip.ExcellentAttrId[i];
            if (attrId != -1 && baseValue > 0)
            {
                ModifyEquipAttribute(attributes, attrId, baseValue, characterLevel);
            }
        }
        if (tbEquip.AddAttrId != -1)
        {
            int addtional = 0; //bagItem.Exdata[1]
            ModifyEquipAttribute(attributes, tbEquip.AddAttrId, addtional, characterLevel);
        }
        for (var i = 0; i < 6; i++)
        {
            var attrId = bagItem.Exdata[6 + i];
            var baseValue = bagItem.Exdata[12 + i];
            if (attrId != -1 && baseValue > 0)
            {
                ModifyEquipAttribute(attributes, attrId, baseValue, characterLevel);
            }
        }
        return attributes;
    }

    public BagItemDataModel GetEquipData(eEquipType type)
    {
        var index = (int) type;
        if (index < 0 || index >= PlayerDataModel.EquipList.Count)
        {
            return null;
        }
        return PlayerDataModel.EquipList[index];
    }

    public List<long> GetExData64()
    {
        return ExtData64;
    }

    public long GetExData64(int index)
    {
        if (0 > index || ExtData64.Count <= index)
        {
            return 0;
        }

        return ExtData64[index];
    }

    public int GetExp()
    {
        return PlayerDataModel.Bags.Resources.Exp;
    }

    //---------------------------------------------------------------friend--------
    public FriendInfoDataModel GetFiendInfo(int type, ulong guid)
    {
        var controller = UIManager.Instance.GetController(UIConfig.FriendUI);
        var ret = controller.CallFromOtherClass("GetFiendInfo", new[] {type, (object) guid});
        return ret as FriendInfoDataModel;
    }

    public int GetFightPoint(int[] attr, int[] attrRef, int level, int careerId)
    {
        //var level = GetLevel();
        var tbLevel = Table.GetLevelData(level);
        if (tbLevel == null)
        {
            return 0;
        }
        var FightPoint = 0;
        for (var type = eAttributeType.PhyPowerMin; type != eAttributeType.Count; ++type)
        {
            //基础固定属性
            var nValue = attr[(int) type];
            switch ((int) type)
            {
                case 15:
                {
                    FightPoint += nValue*tbLevel.LuckyProFightPoint/100;
                }
                    break;
                case 17:
                {
                    FightPoint += nValue*tbLevel.ExcellentProFightPoint/100;
                }
                    break;
                case 21:
                {
                    FightPoint += nValue*tbLevel.DamageAddProFightPoint/100;
                }
                    break;
                case 22:
                {
                    FightPoint += nValue*tbLevel.DamageResProFightPoint/100;
                }
                    break;
                case 23:
                {
                    FightPoint += nValue*tbLevel.DamageReboundProFightPoint/100;
                }
                    break;
                case 24:
                {
                    FightPoint += nValue*tbLevel.IgnoreArmorProFightPoint/100;
                }
                    break;
                default:
                {
                    var tbState = Table.GetStats((int) type);
                    if (tbState == null)
                    {
                        continue;
                    }

                    if (careerId == -1)
                    {
                        careerId = GetRoleId();
                    }
                    if (careerId >= 0)
                    {
                        if (CareeridToStatsPointIndex.ContainsValue(careerId))
                        {
                            FightPoint += tbState.FightPoint[CareeridToStatsPointIndex[careerId]]*nValue/100;
                        }
                        else
                        {
                            Logger.Error(" CareeridToStatsPointIndex2  error {0}", careerId);
                        }
                    }
                    else if (careerId == -2)
                    {
// -2 表示宠物
                        FightPoint += tbState.PetFight*nValue/100;
                    }
                    else
                    {
                        Logger.Error(" CareeridToStatsPointIndex2  error {0}", careerId);
                    }
                }
                    break;
            }
        }

        //百分比计算
        FightPoint += attrRef[(int) eAttributeType.MagPowerMin]*tbLevel.PowerFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.PhyArmor]*tbLevel.ArmorFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.HpMax]*tbLevel.HpFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.MpMax]*tbLevel.MpFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.Hit]*tbLevel.HitFightPoint/10000;
        FightPoint += attrRef[(int) eAttributeType.Dodge]*tbLevel.DodgeFightPoint/10000;
        return FightPoint;
    }

    //给GainItemHintController.cs用的，用以生成一个条目数据
    public GainItemHintEntryArgs GetGainItemHintEntryArgs(int itemId, int index)
    {
        var tbItem = Table.GetItemBase(itemId);
        var bagItem = GetItem(tbItem.InitInBag, index);

        var ret = new GainItemHintEntryArgs();
        ret.ItemData = bagItem;

        // 如果是装备，则需要跟身上的装备作比较，战力是否提升了
        if (tbItem.Type >= 10000 && tbItem.Type <= 10099)
        {
            // 先判断这个装备是否穿得上
            var equipId = tbItem.Exdata[0];
            var tbEquip = Table.GetEquipBase(equipId);

            // 穿不上，返回
            if (CheckItemEquip(tbItem, tbEquip) != eEquipLimit.OK)
            {
                return null;
            }

            var equips = FindEquipedEquips(itemId);
            var worstEquipId = -1;
            var oldEquipFightValue = 0;
            var newEquipFightValue = bagItem.FightValue;
            if (equips.Count > 0)
            {
                worstEquipId = FindWorstEquipIndex(equips);
                oldEquipFightValue = equips[worstEquipId].FightValue;
            }

            // 战力没提高，返回
            if (oldEquipFightValue >= newEquipFightValue)
            {
                return null;
            }

            ret.Count = 1;
            ret.FightValueOld = oldEquipFightValue;
            ret.OldEquipIdx = worstEquipId;
        }
        else
        {
            //技能书
            if (tbItem.Type == 21000)
            {
                var itemInfo = new ItemInfoDataModel();
                itemInfo.ItemData = bagItem;
                GameUtils.InitSkillBook(itemInfo);
                if (tbItem.OccupationLimit != -1)
                {
                    if (tbItem.OccupationLimit != GetRoleId())
                    {
//职业不符不能使用，返回
                        return null;
                    }
                }
                var attr = GetAttribute(itemInfo.AttributeType);
                if (attr < itemInfo.AttributeValue)
                {
//属性不足，返回
                    return null;
                }
                if (IsSkillTalentMax(tbItem.Exdata[0]))
                {
//技能已满，返回
                    return null;
                }
            }

            ret.Count = GetItemTotalCount(itemId).Count;
            ret.FightValueOld = 0;
            ret.OldEquipIdx = 0;
        }
        return ret;
    }

    //获取玩家的Guid
    public ulong GetGuid()
    {
        return Guid;
        //return ObjManager.Instance.MyPlayer.GetObjId();
    }

    //获取某个包裹
    public BagItemDataModel GetItem(int nBagId, int nIndex)
    {
        var bag = GetBag(nBagId);
        if (bag == null)
        {
            return null;
        }
        if (bag.Items.Count > nIndex && nIndex >= 0)
        {
            return bag.Items[nIndex];
        }
        return null;
    }

    public int GetItemCount(int itemId)
    {
        var tbItemBase = Table.GetItemBase(itemId);
        if (tbItemBase.InitInBag < 0)
        {
            return PlayerDataModel.Bags.Resources[itemId];
        }
        var total = GetItemTotalCount(itemId);
        if (total != null)
        {
            return total.Count;
        }
        return 0;
    }

    public int GetItemCount(int bagType, int itemId)
    {
        if (bagType == -1)
        {
            if (itemId >= (int) eResourcesType.LevelRes && itemId < (int) eResourcesType.CountRes)
            {
                return PlayerDataModel.Bags.Resources[itemId];
            }
            return 0;
        }
        var count = 0;
        var bag = GetBag(bagType);
        if (bag == null)
        {
            return count;
        }
        {
            // foreach(var itemData in bag.Items)
            var __enumerator11 = (bag.Items).GetEnumerator();
            while (__enumerator11.MoveNext())
            {
                var itemData = __enumerator11.Current;
                {
                    if (itemData.ItemId == itemId)
                    {
                        count += itemData.Count;
                    }
                }
            }
        }
        return count;
    }

    public TotalCount GetItemTotalCount(int itemId)
    {
        TotalCount totalCount;
        mBagItemCountData.TryGetValue(itemId, out totalCount);
        if (totalCount == null)
        {
            totalCount = new TotalCount();
            mBagItemCountData.Add(itemId, totalCount);
        }
        return totalCount;
    }

    private Dictionary<int, TotalCount> elfCountDictionary = new Dictionary<int, TotalCount>();
    public TotalCount GetElfTotalCount(int itemId)
    {
        TotalCount totalCount;
        elfCountDictionary.TryGetValue(itemId, out totalCount);
        if (totalCount == null)
        {
            totalCount = new TotalCount();
            elfCountDictionary.Add(itemId, totalCount);
        }
        return totalCount;
    }

    public void ClearElfTotalCount()
    {
        elfCountDictionary.Clear();
    }

    public void AddElfTotalCount(int itemId, int count)
    {
        var totalCount = GetElfTotalCount(itemId);
        totalCount.Count += count;
    }

//获取玩家等级
    public int GetLevel()
    {
        return PlayerDataModel.Bags.Resources.Level;
    }

    public int GetLockIndex(int bagType)
    {
        if (bagType == (int) eBagType.Equip
            || bagType == (int) eBagType.BaseItem
            || bagType == (int) eBagType.Depot)
        {
            var bags = PlayerDataModel.Bags.Bags[bagType];
            var tbBagBase = Table.GetBagBase(bagType);
            if (tbBagBase != null)
            {
                if (bags.Capacity <= tbBagBase.MaxCapacity)
                {
                    return bags.Capacity;
                }
            }
        }
        return -1;
    }

    public BitFlag GetLoginApplyState()
    {
        return mLoginApplyState;
    }

    public string GetName()
    {
        return PlayerDataModel.CharacterBase.Name;
    }

    //获得普攻技能id

    public int GetNormalSkill(bool weapon = false)
    {
        if (PlayerDataModel.EquipList[(int) (eEquipType.WeaponMain)].ItemId == -1 && weapon)
        {
//没有主手武器就用空手武器
            return PlayerDataModel.SkillData.SkillNoWeapon;
        }
        if (PlayerDataModel.SkillData.NormailAttack.Count > 0)
        {
            return PlayerDataModel.SkillData.NormailAttack[0].SkillId;
        }
        return -1;
    }

    public int GetPkModel()
    {
        return PlayerDataModel.CharacterBase.PkModel;
    }

    public int GetPkValue()
    {
        return PlayerDataModel.CharacterBase.PkValue;
    }

    public int GetRemaindCapacity(eBagType type)
    {
        BagBaseDataModel bagBaseData;
        if (PlayerDataModel.Bags.Bags.TryGetValue((int) type, out bagBaseData))
        {
            return bagBaseData.Capacity - bagBaseData.Size;
        }
        return 0;
    }

    //获取玩家某个资源
    public int GetRes(int resType)
    {
        if (resType < 0 || resType >= PlayerDataModel.Bags.Resources.Resources.Count)
        {
            return -1;
        }
        return PlayerDataModel.Bags.Resources.Resources[resType];
    }

    //获得职业
    public int GetRoleId()
    {
        return Table.GetCharacterBase(mInitBaseAttr.RoleId).ExdataId;
    }

    //获得技能等级
    public int GetSkillLevel(int nSkillId)
    {
        {
            // foreach(var skill in PlayerDataModel.SkillData.AllSkills)
            var __enumerator19 = (PlayerDataModel.SkillData.AllSkills).GetEnumerator();
            while (__enumerator19.MoveNext())
            {
                var skill = __enumerator19.Current;
                {
                    if (skill.SkillId == nSkillId)
                    {
                        return skill.SkillLv;
                    }
                }
            }
        }
        return 0;
    }

    public int GetSkillNoWeapon()
    {
        return PlayerDataModel.SkillData.SkillNoWeapon;
    }

    //获得天赋层数
    public int GetTalentLayer(int nTalentId)
    {
        {
            // foreach(var talent in PlayerDataModel.SkillData.Talents)
            var __enumerator18 = (PlayerDataModel.SkillData.Talents).GetEnumerator();
            while (__enumerator18.MoveNext())
            {
                var talent = __enumerator18.Current;
                {
                    if (talent.TalentId == nTalentId)
                    {
                        return talent.Count;
                    }
                }
            }
        }
        return 0;
    }

    public string GetTeamMemberName(ulong guid)
    {
        if (guid == Guid)
        {
            return GetName();
        }
        if (TeamDataModel.TeamId == 0)
        {
            return "";
        }
        {
            // foreach(var i in TeamDataModel.TeamList)
            var __enumerator25 = (TeamDataModel.TeamList).GetEnumerator();
            while (__enumerator25.MoveNext())
            {
                var i = __enumerator25.Current;
                {
                    if (i.Guid == guid)
                    {
                        return i.Name;
                    }
                }
            }
        }
        return "";
    }

    //---------------------------------- Buy -----------------------------
    // -1:不限
    public int GetMaxBuyCount(int itemId)
    {
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null || tbItem.StoreID == -1)
        {
            return 0;
        }

        var tbStore = Table.GetStore(tbItem.StoreID);
        if (tbStore == null)
        {
            return 0;
        }

        var limit = tbStore.DayCount;
        if (limit == -1)
        {
            limit = tbStore.WeekCount;
        }
        if (limit == -1)
        {
            limit = tbStore.MonthCount;
        }
        if (limit == -1)
        {
            return -1;
        }

        var canbuyCount = GetExData(limit);
        var table = Table.GetVIP(GetRes((int)eResourcesType.VipLevel));
        for (var j = 0; j < table.BuyItemId.Length; j++)
        {
            if (table.BuyItemId[j] == tbItem.StoreID)
            {
                canbuyCount += table.BuyItemCount[j];
            }
        }
        return canbuyCount;
    }
    public int GetNeedBuyCount(int itemId, int needCount)
    {
        var haveItemCount = GetItemCount(itemId);
        if (haveItemCount >= needCount)
        { // 拥有个数足够多
            return 0;
        }

        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null || tbItem.StoreID == -1)
        {
            return -1;
        }

        var tbStore = Table.GetStore(tbItem.StoreID);
        if (tbStore == null || tbStore.ItemCount == 0)
        {
            return -1;
        }

        if (GameUtils.IsQuickBuyGift(tbStore.ItemId))
        { // 礼包
            return 1;
        }
        

        var canbuyCount = GetMaxBuyCount(itemId);
        var needbuyCount = (int)Math.Ceiling((double)(needCount - haveItemCount) / tbStore.ItemCount);
        if (canbuyCount == -1)
        { // 无限买
            return needbuyCount;
        }

        if (canbuyCount >= needbuyCount)
        { // 可购买的数量多于需要数量
            return needbuyCount;
        }

        return -1;
    }



    //---------------------------------------------------------------Wing----
    public int GetWingId()
    {
        var wingCon = UIManager.Instance.GetController(UIConfig.WingUI);
        if (wingCon != null)
        {
            var dataModel = wingCon.GetDataModel("") as WingDataModel;
            if (dataModel != null)
            {
                return dataModel.ItemData.ItemId;
            }
        }
        return -1;
    }

    public void InitBagData(BagBaseData bagBase)
    {
        var bagId = bagBase.BagId;
        var tbBagBase = Table.GetBagBase(bagId);
        if (tbBagBase == null)
        {
            return;
        }
        BagBaseDataModel bagBaseData;
        if (!PlayerDataModel.Bags.Bags.TryGetValue(bagId, out bagBaseData))
        {
            bagBaseData = new BagBaseDataModel();
            PlayerDataModel.Bags.Bags[bagId] = bagBaseData;
            bagBaseData.BagId = bagId;
            bagBaseData.MaxCapacity = tbBagBase.MaxCapacity;
        }
        bagBaseData.Capacity = bagBase.NowCount;
        var itemList = new List<BagItemDataModel>();
        var bagBaseDataMaxCapacity1 = bagBaseData.MaxCapacity;
        for (var i = 0; i < bagBaseDataMaxCapacity1; i++)
        {
            var bagItemData = new BagItemDataModel();
            bagItemData.Status = i < bagBaseData.Capacity ? 0 : 1;
            itemList.Add(bagItemData);
            bagItemData.BagId = bagId;
            bagItemData.Index = i;
        }
        {
            var __list8 = bagBase.Items;
            var __listCount8 = __list8.Count;
            for (var __i8 = 0; __i8 < __listCount8; ++__i8)
            {
                var item = __list8[__i8];
                {
                    var bagItemData = itemList[item.Index];
                    bagItemData.BagId = bagId;
                    bagItemData.Count = item.Count;
                    bagItemData.Index = item.Index;
                    bagItemData.ItemId = item.ItemId;
                    bagItemData.Exdata.InstallData(item.Exdata);
                    var tbItem = Table.GetItemBase(item.ItemId);
                    bagItemData.Exdata.IsntEquip = tbItem == null || tbItem.Type < 10000 || tbItem.Type > 10099;
//                     if (bagId != (int)eBagType.Depot)
//                     {
//                         NotifyBagItemCountChanged(bagId, bagItemData.ItemId);
//                     }

                    if (bagId == (int) eBagType.Equip
                        || bagId == (int) eBagType.Depot)
                    {
                        GetBagItemFightPoint(bagItemData);
                    }

                    if (tbItem != null && tbItem.Type == 24000)
                    {
                        if (tbItem.Exdata[2] == 0)
                        {
                            bagItemData.itemWithSkill = PlayerDataModel.Bags.ItemWithSkillList[1];
                        }
                        if (tbItem.Exdata[2] == 1)
                        {
                            bagItemData.itemWithSkill = PlayerDataModel.Bags.ItemWithSkillList[2];
                        }
                    }
                    else
                    {
                        bagItemData.itemWithSkill = PlayerDataModel.Bags.ItemWithSkillList[0];
                    }
                }
            }
        }

        bagBaseData.Items = new ObservableCollection<BagItemDataModel>(itemList);
        bagBaseData.Size = bagBase.Items.Count;
        if (bagId == 0)
        {
            if (bagBaseData.Size == bagBaseData.Capacity)
            {
                PlayerDataModel.Bags.IsEquipFull = true;
            }
            else
            {
                PlayerDataModel.Bags.IsEquipFull = false;
            }
        }
        if (bagBase.NextSecond > 0)
        {
            bagBaseData.UnlockTime = Game.Instance.ServerTime.AddSeconds(bagBase.NextSecond);
        }
        else
        {
            bagBaseData.UnlockTime = Game.Instance.ServerTime;
        }

        if (bagId != (int) eBagType.Depot)
        {
            {
                // foreach(var itemBaseData in bagBase.Items)
                var __enumerator27 = (bagBase.Items).GetEnumerator();
                while (__enumerator27.MoveNext())
                {
                    var itemBaseData = __enumerator27.Current;
                    {
                        NotifyBagItemCountChanged(bagId, itemBaseData.ItemId);
                    }
                }
            }
        }
        if (bagId == (int) eBagType.BaseItem)
        {
            RefreshMedicineWarn();
        }
    }

    private void InitEquipData(BagBaseData data)
    {
        var bagId = data.BagId;
        switch (bagId)
        {
            case 9:
            case 10:
            case 12:
                break;
            case 13:
            {
                var dataItemsCount4 = data.Items.Count;
                for (var i = 0; i < dataItemsCount4; i++)
                {
                    var item = data.Items[i];
                    var equipData = PlayerDataModel.EquipList[(int) eEquipType.RingL + i];
                    equipData.BagId = bagId;
                    equipData.ItemId = item.ItemId;
                    equipData.Index = item.Index;
                    equipData.Exdata.InstallData(item.Exdata);

                    GetBagItemFightPoint(equipData);
                }
            }
                break;
            default:
            {
                if (data.Items.Count == 1)
                {
                    var item = data.Items[0];
                    var bagType = BagIdToEquipType[bagId];
                    var equipData = PlayerDataModel.EquipList[bagType];
                    equipData.BagId = bagId;
                    equipData.ItemId = item.ItemId;
                    equipData.Index = item.Index;
                    equipData.Exdata.InstallData(item.Exdata);

                    GetBagItemFightPoint(equipData);
                }
            }
                break;
        }
    }

    public static void InitExtDataEvent()
    {
        Table.ForeachFuben(recoard =>
        {
            List<int> dungeonList;
            if (DungeonEnterExData.TryGetValue(recoard.TodayCountExdata, out dungeonList))
            {
                dungeonList.Add(recoard.Id);
            }
            else
            {
                dungeonList = new List<int>();
                dungeonList.Add(recoard.Id);
                DungeonEnterExData.Add(recoard.TodayCountExdata, dungeonList);
            }


            if (DungeonResetExData.TryGetValue(recoard.ResetExdata, out dungeonList))
            {
                dungeonList.Add(recoard.Id);
            }
            else
            {
                dungeonList = new List<int>();
                dungeonList.Add(recoard.Id);
                DungeonResetExData.Add(recoard.ResetExdata, dungeonList);
            }

            return true;
        });
    }

    private void InitItemWithSkill()
    {
        for (var i = 0; i < 3; i++)
        {
            var skillItem = PlayerDataModel.Bags.ItemWithSkillList[i];
            if (i == 0)
            {
                skillItem.HasCoolDown = false;
            }

            //血瓶
            if (i == 1)
            {
                skillItem.HasCoolDown = true;
                var tbSkill = Table.GetSkill(300);
                skillItem.MaxTime = tbSkill.Cd/1000.0f;
            }

            //魔瓶
            if (i == 2)
            {
                skillItem.HasCoolDown = true;
                var tbSkill = Table.GetSkill(301);
                skillItem.MaxTime = tbSkill.Cd/1000.0f;
            }
        }
    }

    public void InitQueneData(QueueInfo queneInfo)
    {
        var queueData = PlayerDataModel.QueueUpData;
        queueData.QueueId = queneInfo.QueueId;
        queueData.StartTime = Extension.FromServerBinary(queneInfo.StartTime);
        queueData.ExpectScend = queneInfo.NeedSeconds;
        EventDispatcher.Instance.DispatchEvent(new QueneUpdateEvent());
        EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(queueData.StartTime,
            queueData.QueueId));
    }

    public bool IsInFubenScnen()
    {
        var sceneId = SceneManager.Instance.CurrentSceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null)
        {
            return false;
        }
        if (tbScene.Type == (int)eSceneType.City || tbScene.Type == (int)eSceneType.Normal)
        {
            return false;
        }
        return true;
    }

    public bool IsInPvPScnen()
    {
        var sceneId = SceneManager.Instance.CurrentSceneTypeId;
        var tbScene = Table.GetScene(sceneId);
        if (tbScene == null)
        {
            return false;
        }
        if (tbScene.Type == (int)eSceneType.Pvp)
        {
            return true;
        }
        return false;
    }

    //获取天赋是否加满了
    public bool IsSkillTalentMax(int skillid)
    {
        var talentData = PlayerDataModel.SkillData.Talents;
        var dic = new Dictionary<int, int>();

        var talentDataCount13 = talentData.Count;
        for (var i = 0; i < talentDataCount13; i++)
        {
            if (talentData[i].TalentId == 0)
            {
                continue;
            }
            dic.Add(talentData[i].TalentId, talentData[i].Count);
        }

        var list = new List<int>();
        Table.ForeachTalent(table =>
        {
            if (table.ModifySkill == skillid)
            {
                list.Add(table.Id);
            }
            return true;
        });

        var currentCount = 0;
        var maxCount = Table.GetSkill(skillid).TalentMax;

        mSkillTalent.TryGetValue(skillid, out currentCount);

        var listCount14 = list.Count;
        for (var i = 0; i < listCount14; i++)
        {
            var count = 0;
            dic.TryGetValue(list[i], out count);
            currentCount += count;
        }

        return currentCount >= maxCount;
    }

    /// <summary>
    ///     物品或者装备是否可装备
    /// </summary>
    public bool ItemOrEquipCanUse(ItemBaseRecord tbItemBase)
    {
        if (tbItemBase == null)
        {
            return false;
        }
        if (tbItemBase.Type >= 10000 && tbItemBase.Type <= 10099)
        {
            var tbEquip = Table.GetEquipBase(tbItemBase.Id);
            if (tbEquip == null)
            {
                return false;
            }
            var result = Instance.CheckItemEquip(tbItemBase, tbEquip);
            if (result != eEquipLimit.OK)
            {
                return false;
            }
        }
        else
        {
            if (tbItemBase.UseLevel > Instance.GetLevel())
            {
                return false;
            }
        }
        return true;
    }

    //技能书学习技能
    public void LearnSkill(int skillid, bool equipNewSkill = true)
    {
        PlatformHelper.Event("skill", "Learn", skillid);
        PlatformHelper.UMEvent("skill", "Learn", skillid);
        var talentPoint = 0;
        if (!mSkillTalent.TryGetValue(skillid, out talentPoint))
        {
            mSkillTalent.Add(skillid, 0);
        }

        var skills = PlayerDataModel.SkillData.OtherSkills;
        var count = skills.Count;
        for (var i = 0; i < count; i++)
        {
            var skill = skills[i];
            if (skill.SkillId == skillid)
            {
                skill.SkillLv = 1;
                skill.RefreshCast();

                if (equipNewSkill)
                {
                    var equipskills = PlayerDataModel.SkillData.EquipSkills;
                    var c = equipskills.Count;
                    for (var j = 0; j < c; j++)
                    {
                        var equip = equipskills[j];
                        if (equip.SkillId <= 0)
                        {
                            EventDispatcher.Instance.DispatchEvent(new UIEvent_SkillFrame_EquipSkill(j, skill.SkillId,
                                false));
                            break;
                        }
                    }
                }
            }
        }
    }

    public void ModifyEquipAttribute(Dictionary<int, int> AttrList, int AttrId, int AttrValue, int characterLevel)
    {
        if (AttrId == 98)
        {
            var nValue = characterLevel/AttrValue;
            if (GetRoleId() != 1)
            {
                AttrList.modifyValue((int) eAttributeType.PhyPowerMin, nValue);
                AttrList.modifyValue((int) eAttributeType.PhyPowerMax, nValue);
            }
            else
            {
                AttrList.modifyValue((int) eAttributeType.MagPowerMin, nValue);
                AttrList.modifyValue((int) eAttributeType.MagPowerMax, nValue);
            }
        }
        else if (AttrId == 99)
        {
            var nValue = characterLevel/AttrValue;
            AttrList.modifyValue((int) eAttributeType.PhyArmor, nValue);
            AttrList.modifyValue((int) eAttributeType.MagArmor, nValue);
        }
        else if (AttrId == 105)
        {
            if (GetRoleId() != 1)
            {
                AttrList.modifyValue((int) eAttributeType.PhyPowerMin, AttrValue);
                AttrList.modifyValue((int) eAttributeType.PhyPowerMax, AttrValue);
            }
            else
            {
                AttrList.modifyValue((int) eAttributeType.MagPowerMin, AttrValue);
                AttrList.modifyValue((int) eAttributeType.MagPowerMax, AttrValue);
            }
        }
        else if (AttrId == 110)
        {
            AttrList.modifyValue((int) eAttributeType.PhyArmor, AttrValue);
            AttrList.modifyValue((int) eAttributeType.MagArmor, AttrValue);
        }
        else
        {
            AttrList.modifyValue(AttrId, AttrValue);
        }
    }

    public void NotifyBagItemCountChanged(int bagType, int itemId)
    {
        if (itemId == -1)
        {
            return;
        }

        if (bagType == -1)
        {
            if (itemId >= (int) eResourcesType.LevelRes && itemId < (int) eResourcesType.CountRes)
            {
                TotalCount totalCount;
                if (!mBagItemCountData.TryGetValue(itemId, out totalCount))
                {
                    totalCount = new TotalCount();
                    mBagItemCountData.Add(itemId, totalCount);
                }

                totalCount.Count = GetItemCount(bagType, itemId);

                if (itemId == (int) eResourcesType.VipLevel)
                {
                    TbVip = Table.GetVIP(totalCount.Count);
                    EventDispatcher.Instance.DispatchEvent(new VipLevelChangedEvent());
                    RefleshRewardInfo();
                }

                return;
            }
        }

        var eBagType = (eBagType) bagType;
        if (eBagType == eBagType.BaseItem
            || eBagType == eBagType.Equip
            || eBagType == eBagType.Piece
            || eBagType == eBagType.FarmDepot
            || eBagType == eBagType.Elf)
        {
            TotalCount totalCount;
            if (!mBagItemCountData.TryGetValue(itemId, out totalCount))
            {
                totalCount = new TotalCount();
                mBagItemCountData.Add(itemId, totalCount);
            }

            totalCount.Count = GetItemCount(bagType, itemId);
        }
    }

    public void NotifyMatchingData(QueueInfo queueInfo)
    {
        var queueData = PlayerDataModel.QueueUpData;
        queueData.QueueId = queueInfo.QueueId;
        if (queueData.QueueId == -1)
        {
            //取消排队
            var e1 = new ShowUIHintBoard(270001);
            EventDispatcher.Instance.DispatchEvent(e1);
            EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(Game.Instance.ServerTime, -1));
        }
        else
        {
            queueData.StartTime = Extension.FromServerBinary(queueInfo.StartTime);
            queueData.ExpectScend = queueInfo.NeedSeconds;
            EventDispatcher.Instance.DispatchEvent(new UIEvent_WindowShowDungeonQueue(queueData.StartTime,
                queueData.QueueId));
        }
        //         eLeaveMatchingType eType = (eLeaveMatchingType)type;
        //         switch (eType)
        //         {
        //             case eLeaveMatchingType.Unknow:
        //                 break;
        //             case eLeaveMatchingType.TimeOut:
        //             {
        //                 ShowUIHintBoard e1 = new ShowUIHintBoard("超时未响应");
        //                 EventDispatcher.Instance.DispatchEvent(e1);
        //             }
        //                 break;
        //             case eLeaveMatchingType.Cannel:
        //             {
        //                 ShowUIHintBoard e1 = new ShowUIHintBoard("取消了排队");
        //                 EventDispatcher.Instance.DispatchEvent(e1);
        //             }
        //                 break;
        //             case eLeaveMatchingType.Refuse:
        //                 {
        //                     ShowUIHintBoard e1 = new ShowUIHintBoard("拒绝了该匹配");
        //                     EventDispatcher.Instance.DispatchEvent(e1);
        //                 }
        //                 break;
        //             case eLeaveMatchingType.Success:
        //                 {
        //                     //ShowUIHintBoard e1 = new ShowUIHintBoard("队伍其他人取消了排队");
        //                     //EventDispatcher.Instance.DispatchEvent(e1);
        //                 }
        //                 break;
        //             case eLeaveMatchingType.InTemp:
        //                 {
        //                     ShowUIHintBoard e1 = new ShowUIHintBoard("临时进队");
        //                     EventDispatcher.Instance.DispatchEvent(e1);
        //                 }
        //                 break;
        //             case eLeaveMatchingType.Onlost:
        //                 break;
        //             case eLeaveMatchingType.TeamOther:
        //             {
        //                 ShowUIHintBoard e1 = new ShowUIHintBoard("队伍其他人取消了排队");
        //                 EventDispatcher.Instance.DispatchEvent(e1);
        //             }
        //                 break;
        //             default:
        //                 throw new ArgumentOutOfRangeException();
        //         }
        var e = new QueneUpdateEvent();
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnInitLevel()
    {
        var e = new LevelUpInitEvent();
        EventDispatcher.Instance.DispatchEvent(e);
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Equip);
        IsLevelInited = true;
        RefreshMedicineWarn();
    }

    //----------------------------------------------------------------Notice------------------
    private void OnPropertyNoticeChange(object sender, PropertyChangedEventArgs e)
    {
        if (sender == NoticeData)
        {
            if (e.PropertyName == "ArenaTotal"
                || e.PropertyName == "ArenaTotalIcon"
                || e.PropertyName == "FarmTotal"
                || e.PropertyName == "FarmTotalIcon"
                || e.PropertyName == "HomeBattle"
                || e.PropertyName == "HomeBattleIco"
                || e.PropertyName == "WishDrawFree"
                )
            {
                var e1 = new CityNoticeFlagRefrsh(e.PropertyName, NoticeData.ArenaTotal);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
            RewardNoticeReflesh(e.PropertyName);
        }
    }

    public void OnUpdataResourcesChange(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Level")
        {
            UpdateExpPercent();
        }
        else if (e.PropertyName == "Exp")
        {
            UpdateExpPercent();
        }
        if (e.PropertyName == "HomeMaterial1")
        {
            UpdateFortressExpPercent();
        }
        else if (e.PropertyName == "Achievement")
        {
            UpdateFortressExpPercent();
        }
    }

    //obj:飞的物体，from:飞的起始地点   to:飞的终止位置 itemid:物品id  count :数量   startaddpos 起始位置坐标加成,  duation 动画持续时间  highvecotr:整体动画移动位置
    public void PlayFlyItem(GameObject obj,
                            Transform from,
                            Transform to,
                            int itemId,
                            int count = 0,
                            Vector3 StartAddPos = default(Vector3),
                            float Duation = 0.5f,
                            Vector3 highVecotr = default(Vector3))
    {
        var cityAnim = obj.GetComponent<HomeExpAnimation>();
        if (cityAnim != null)
        {
            var item = new BagItemDataModel();
            item.ItemId = itemId;
            item.Count = count;
            cityAnim.Init(from, to, item, StartAddPos, Duation, highVecotr);
            cityAnim.CityHomeExpPlay();
        }
    }

    public void RefrehEquipPriority()
    {
        var ret = new List<int>();

        var skill = PlayerDataModel.SkillData.EquipSkills;
        var skillStates = PlayerDataModel.SkillData.SkillStates;

        var isPvp = false;
        if (GameLogic.Instance
            && GameLogic.Instance.Scene
            && GameLogic.Instance.Scene.SceneTypeId != -1)
        {
            var tbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            if (tbScene != null && tbScene.Type == 3)
            {
                isPvp = true;
            }
        }

        for (var i = 0; i < skill.Count; i++)
        {
            var skillId = skill[i].SkillId;
            var skillTable = Table.GetSkill(skillId);
            if (skillTable == null)
            {
                continue;
            }
            if (skillTable.ReleaseLevel < 0)
            {
                continue;
            }
            if (isPvp == false)
            {
                if (skillTable.ReleaseLevel >= 100)
                {
                    continue;
                }
            }

            var index = 0;
            for (var j = 0; j < ret.Count; j++)
            {
                var tbSkill = Table.GetSkill(ret[j]);
                var level1 = skillTable.ReleaseLevel;
                var level2 = tbSkill.ReleaseLevel;
                if (isPvp)
                {
                    if (level1 > 100)
                    {
                        level1 -= 100;
                    }
                    if (level2 > 100)
                    {
                        level2 -= 100;
                    }
                }
                if (level1 < level2)
                {
                    break;
                }
                index++;
            }
            ret.Insert(index, skillId);

            SkillStateData skillState;
            if (skillStates.TryGetValue(skillId, out skillState))
            {
                skillState.State = SkillState.Rece;
            }
            else
            {
                skillState = new SkillStateData();
                skillState.SkillId = skillId;
                skillState.State = SkillState.Rece;
                skillStates.Add(skillId, skillState);
            }
        }

        PlayerDataModel.SkillData.EquipSkillsPriority = ret;
    }

    private void RefrehNoticeFlagCondition()
    {
        if (GameSetting.Instance.EnableNewFunctionTip == false)
        {
            return;
        }

        NoticeData.ElfFlag = CheckCondition(GameUtils.ElfFlagConfig) == 0;
        NoticeData.HandBookWantedFlag = CheckCondition(GameUtils.HandBookWantedFlagConfig) == 0;
        NoticeData.SkillTalentFlag = CheckCondition(GameUtils.SkillTalentFlagConfig) == 0;
        NoticeData.AchievementFlag = CheckCondition(GameUtils.AchieveFlagConfig) == 0;
        //NoticeData.MailFlag = CheckCondition(GameUtils.MailFlagConfig) == 0;

        NoticeData.FriendOpenFlag = CheckCondition(GameUtils.FriendFlagConfig) == 0;
        NoticeData.TeamOpenFlag = CheckCondition(GameUtils.TeamFlagConfig) == 0;
        NoticeData.UnionOpenFlag = CheckCondition(GameUtils.UnionFlagConfig) == 0;
        NoticeData.WishFlag = CheckCondition(GameUtils.WishFlagReward) == 0;
        NoticeData.RankCanLikeFlag = CheckCondition(GameUtils.RankFlagConfig) == 0;
    }

    private void RefrehNoticeFlagCondition(int flagId, bool flagValue)
    {
        if (GameSetting.Instance.EnableNewFunctionTip == false)
        {
            return;
        }
        if (flagId == GameUtils.ConditionToFlag[GameUtils.ElfFlagConfig])
        {
            NoticeData.ElfFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.HandBookGroupFlagConfig])
        {
            NoticeData.HandBookGroupFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.SkillTalentFlagConfig])
        {
            NoticeData.SkillTalentFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.AchieveFlagConfig])
        {
            NoticeData.AchievementFlag = flagValue;
        }
        //else if (flagId == GameUtils.ConditionToFlag[GameUtils.MailFlagConfig])
        //{
        //    NoticeData.MailFlag = flagValue;
        //}
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.FriendFlagConfig])
        {
            NoticeData.FriendOpenFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.TeamFlagConfig])
        {
            NoticeData.TeamOpenFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.UnionFlagConfig])
        {
            NoticeData.UnionOpenFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.WishFlagReward])
        {
            NoticeData.WishFlag = flagValue;
        }
        else if (flagId == GameUtils.ConditionToFlag[GameUtils.RankFlagConfig])
        {
            NoticeData.RankCanLikeFlag = flagValue;
        }
    }

    private void RefrehNoticeFlagLevelUpCondition()
    {
        NoticeData.HandBookGroupFlag = CheckCondition(GameUtils.HandBookGroupFlagConfig) == 0;
        NoticeData.SkillInnateFlag = CheckCondition(GameUtils.SkillInnateFlagConfig) == 0;
        var skilltotal = NoticeData.SkillTotal;
    }

    public void RefreshEquipBagStatus(eBagType bagType = eBagType.Equip)
    {
        var bag = GetBag((int) bagType);
        if (bag != null)
        {
            var bagCapacity6 = bag.Capacity;
            for (var i = 0; i < bagCapacity6; i++)
            {
                var bagItem = bag.Items[i];
                if (bagItem.ItemId != -1)
                {
                    RefreshEquipBagStatus(bagItem);
                }
            }
        }
    }

    public void RefreshEquipBagStatus(BagItemDataModel bagItem)
    {
        var itemId = bagItem.ItemId;
        var tbItem = Table.GetItemBase(itemId);


        if (tbItem.Type < 10000 || tbItem.Type > 10099)
        {
            return;
        }

        var equipId = tbItem.Exdata[0];
        var tbEquip = Table.GetEquipBase(equipId);

        var canEquip = CheckItemEquip(tbItem, tbEquip);

        if (canEquip == eEquipLimit.Occupation)
        {
            bagItem.Status = (int) eBagItemType.No;
            return;
        }
        var fightcompare = 0;

        var isDoubHand = false;


        if (tbItem.Type == 10010 || tbItem.Type == 10011 || tbItem.Type == 10098 || tbItem.Type == 10099)
        {
            var eEquipTypeWeaponScend8 = eEquipType.WeaponScend;
            for (var i = eEquipType.WeaponMain; i <= eEquipTypeWeaponScend8; i++)
            {
                var equipData = GetEquipData(i);
                if (equipData.ItemId != -1)
                {
                    var tbItemEquip = Table.GetItemBase(equipData.ItemId);
                    if (tbItemEquip.Type == 10099)
                    {
                        isDoubHand = true;
                        if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                        {
                            fightcompare = equipData.FightValue;
                        }
                    }
                    break;
                }
            }
        }

        if (isDoubHand == false)
        {
            switch (tbItem.Type)
            {
                case 10006: //
                {
                    var equipData = GetEquipData(eEquipType.RingL);
                    if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                    {
                        fightcompare = equipData.FightValue;
                    }

                    equipData = GetEquipData(eEquipType.RingR);
                    if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                    {
                        if (fightcompare > equipData.FightValue)
                        {
                            fightcompare = equipData.FightValue;
                        }
                    }
                }
                    break;
                case 100098:
                {
                    var part = ChangeEquipTypeToPart((int) eEquipType.WeaponMain);
                    if (BitFlag.GetLow(tbEquip.Part, part))
                    {
                        var equipData = GetEquipData(eEquipType.WeaponMain);
                        if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                        {
                            fightcompare = equipData.FightValue;
                        }
                    }
                    part = ChangeEquipTypeToPart((int) eEquipType.WeaponScend);
                    if (BitFlag.GetLow(tbEquip.Part, part))
                    {
                        var equipData = GetEquipData(eEquipType.WeaponScend);
                        if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                        {
                            if (fightcompare > equipData.FightValue)
                            {
                                fightcompare = equipData.FightValue;
                            }
                        }
                    }
                }
                    break;
                case 100099:
                {
                    var equipData = GetEquipData(eEquipType.WeaponMain);
                    if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                    {
                        fightcompare = equipData.FightValue;
                    }


                    equipData = GetEquipData(eEquipType.WeaponScend);
                    if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK || canEquip == eEquipLimit.Attribute)
                    {
                        if (fightcompare > equipData.FightValue)
                        {
                            fightcompare += equipData.FightValue;
                        }
                    }
                }
                    break;
                default:
                {
                    var inteEquipTypeCount9 = (int) eEquipType.Count;
                    for (var i = 0; i < inteEquipTypeCount9; i++)
                    {
                        var part = ChangeEquipTypeToPart(i);
                        if (BitFlag.GetLow(tbEquip.Part, part))
                        {
                            var equipData = GetEquipData((eEquipType) i);
                            if (null != equipData)
                            {
                                if (equipData.ItemId != -1)
                                {
                                    if (CheckItemEquip(equipData.ItemId) == eEquipLimit.OK
                                        || canEquip == eEquipLimit.Attribute)
                                    {
                                        fightcompare = equipData.FightValue;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
                    break;
            }
        }

        if (canEquip == eEquipLimit.OK)
        {
            if (bagItem.FightValue > fightcompare)
            {
                bagItem.Status = (int) eBagItemType.FightDown;
            }
            else if (bagItem.FightValue == fightcompare)
            {
                bagItem.Status = (int) eBagItemType.UnLock;
            }
            else
            {
                bagItem.Status = (int) eBagItemType.FightUp;
            }
        }
        else if (canEquip == eEquipLimit.Attribute)
        {
            if (bagItem.FightValue > fightcompare)
            {
                bagItem.Status = (int) eBagItemType.NoFightDown;
            }
            else if (bagItem.FightValue == fightcompare)
            {
                bagItem.Status = (int) eBagItemType.No;
            }
            else
            {
                bagItem.Status = (int) eBagItemType.NoFightUp;
            }
        }
    }

    //刷新装备是否可用
    public void RefreshEquipStatus()
    {
        for (var i = 0; i < PlayerDataModel.EquipList.Count; i++)
        {
            var item = PlayerDataModel.EquipList[i];
            if (item.ItemId != -1)
            {
                var tbItem = Table.GetItemBase(item.ItemId);
                var equipId = tbItem.Exdata[0];
                var tbEquip = Table.GetEquipBase(equipId);
                var canEquip = CheckItemEquip(tbItem, tbEquip);
                if (canEquip != eEquipLimit.OK)
                {
                    item.Status = (int) eBagItemType.No;
                }
                else
                {
                    item.Status = (int) eBagItemType.UnLock;
                }
                GetBagItemFightPoint(item);
            }
        }
    }

    public void RefreshMedicineWarn()
    {
        var lv = GetLevel();
        if (lv <= 0)
        {
            return;
        }
        var baseBag = GetBag((int) eBagType.BaseItem);
        if (baseBag == null)
        {
            return;
        }
        var count = baseBag.Items.Count;
        var hasHp = false;
        var hasMp = false;
        for (var i = 0; i < count; i++)
        {
            var item = baseBag.Items[i];
            if (item.ItemId == -1)
            {
                continue;
            }
            var tbItem = Table.GetItemBase(item.ItemId);
            if (tbItem == null)
            {
                continue;
            }
            if (tbItem.Type != 24000)
            {
                continue;
            }
            if (tbItem.UseLevel > lv)
            {
                continue;
            }

            if (tbItem.Exdata[2] == 0)
            {
                hasHp = true;
            }
            else if (tbItem.Exdata[2] == 1)
            {
                hasMp = true;
            }
        }
        if (hasHp == false)
        {
            PlayerDataModel.Bags.MedicineWarn = 2;
        }
        else if (hasMp == false)
        {
            PlayerDataModel.Bags.MedicineWarn = 1;
        }
        else
        {
            PlayerDataModel.Bags.MedicineWarn = 0;
        }
    }

    public void RefrshEquipDurable()
    {
        if (PlayerDataModel.Bags.IsDuableChange)
        {
//有变化才请求
            NetManager.Instance.StartCoroutine(ApplyEquipDurableCoroutine());
        }
        else
        {
//无变化本地重新算一遍，比如单纯换个装备
            CheckEquipDurable();
        }
    }

    public void RemoveBagItemCount(int type)
    {
        mBagItemCountData.Remove(type);
    }

    public void RemoveSelectTarget(ObjCharacter obj)
    {
        if (obj == null)
        {
            return;
        }
        var characterInfo = SelectTargetData.CharacterBase;
        if (obj.GetObjId() != characterInfo.CharacterId)
        {
            return;
        }
        SelectTargetData.Type = 0;
        SelectTargetData.CharacterBase = EmptyCharacterBaseDataModel;
        SelectTargetData.IsHasTarget = false;
        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            GameLogic.Instance.Scene.HideSelectReminder();
        }
    }

    public void ResetLoginApplyState()
    {
        mLoginApplyState.ReSetAllFlag();
    }

    public void ResetSelectTarget()
    {
        SelectTargetData.Type = 0;
        SelectTargetData.IsHasTarget = false;
        SelectTargetData.CharacterBase = EmptyCharacterBaseDataModel;
        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            GameLogic.Instance.Scene.HideSelectReminder();
        }
    }

    public void ResGainTip(int resType, int count)
    {
        switch ((eResourcesType) resType)
        {
            case eResourcesType.ExpRes:
            {
                var e1 = new ShowUIHintBoard(count.ToString(), 99);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
            case eResourcesType.LevelRes:
                break;
            case eResourcesType.GoldRes:
            {
                var e1 = new ShowUIHintBoard(count.ToString(), 98);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
            default:
            {
                var tbItem = Table.GetItemBase(resType);
                var str = String.Format(GameUtils.GianItemTip.Desc[0], tbItem.Name, count);
                var e = new ShowUIHintBoard(str);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
        }
    }

    public void SetAttribute(eAttributeType type, int value)
    {
        if (type < 0 || type >= eAttributeType.AttrCount)
        {
        }

        PlayerDataModel.Attributes[(int) type] = value;
    }

    public void SetBookGroupEnable(int groupId, int index)
    {
        var data = 0;
        if (mBookGropData.TryGetValue(groupId, out data))
        {
            mBookGropData[groupId] = data | (1 << index);
        }
        else
        {
            data = data | (1 << index);
            mBookGropData.Add(groupId, data);
        }
    }

    public void SetBountyBookEnable(int bookId)
    {
        if (mBountyBooks.Contains(bookId))
        {
            Logger.Error("BountyBook already active!!! bookId ={0}", bookId);
            return;
        }
        mBountyBooks.Add(bookId);
    }

    public void SetCanShowTarget(bool canShow)
    {
        SelectTargetData.IsCanShow = canShow;
    }

    //设置扩展计数
    public void SetExData64(int index, long value)
    {
        if (0 > index || ExtData64.Count <= index)
        {
            return;
        }
        if (value == ExtData64[index])
        {
            return;
        }
        ExtData64[index] = value;

        EventDispatcher.Instance.DispatchEvent(new ExData64UpDataEvent(index, value));
    }

    public void SetLoginApplyState(eLoginApplyType type)
    {
        mLoginApplyState.SetFlag((int) type);
    }

    //---------------------------------------------------------------Notice--------
    private void SetMainNotice()
    {
        var freeElfDraw = Instance.GetExData(eExdataDefine.e411);
        if (freeElfDraw > 0)
        {
            NoticeData.ElfDraw = true;
        }
        else
        {
            NoticeData.ElfDraw = false;
        }
    }

    //设置空手技能
    public void SetNoWeaponSkill(int skillId)
    {
        PlayerDataModel.SkillData.SkillNoWeapon = skillId;
    }

    public void SetPkModel(int value)
    {
        PlayerDataModel.CharacterBase.PkModel = value;
    }

    public void SetPkValue(int value)
    {
        PlayerDataModel.CharacterBase.PkValue = value;
    }

    public void SetRes(int resType, int count)
    {
        if (resType < 0 || resType >= PlayerDataModel.Bags.Resources.Resources.Count)
        {
            return;
        }
        var oldValue = PlayerDataModel.Bags.Resources[resType];
        if (oldValue == count)
        {
            return;
        }
        PlayerDataModel.Bags.Resources[resType] = count;
        var ee = new Resource_Change_Event((eResourcesType) resType, oldValue, count);
        EventDispatcher.Instance.DispatchEvent(ee);
        NotifyBagItemCountChanged(-1, resType);
        switch ((eResourcesType) resType)
        {
            case eResourcesType.LevelRes:
            {
                if (oldValue != count)
                {
                    EventDispatcher.Instance.DispatchEvent(new Event_LevelChange());
                }
                if (oldValue == -1)
                {
                }
                else if (count > oldValue)
                {
                    var e1 = new Event_LevelUp();
                    EventDispatcher.Instance.DispatchEvent(e1);
                    RefrehNoticeFlagLevelUpCondition();

                    for (int i = oldValue; i < count; i++)
                    {
                        var reBorm = GetExData(eExdataDefine.e51);
                        var lv = i + 1;
                        PlatformHelper.UMEvent("Level", reBorm + "|" + lv);
                    }

                    // var reBorm = GetExData(eExdataDefine.e51);
                    // PlatformHelper.UMEvent("Level", reBorm + "|" + count);
                    //sdk统计数据
                    var ts = Game.Instance.ServerTime - DateTime.Parse("1970-1-1");
                    PlatformHelper.CollectionLevelUpDataForKuaifa(count,  ((int)ts.TotalSeconds).ToString());
                }
                if (ObjManager.Instance.MyPlayer)
                {
                    ObjManager.Instance.MyPlayer.SetLevel(count);
                }
            }
                break;
        }
    }

    //---------------------------------------------------------------Target--------
    public void SetSelectTargetData(ObjCharacter obj, int type = 0)
    {
        if (obj == null)
        {
            return;
        }
        if (obj.Dead)
        {
            return;
        }

        var npc = obj as ObjNPC;
        if (npc != null)
        {
            var interactive = false;
            if (null != npc.TableNPC)
            {
                interactive = npc.TableNPC.Interactive != 0;
            }
            if (!interactive)
            {
                return;
            }
        }

        var characterInfo = SelectTargetData.CharacterBase;
        if (obj.GetObjId() == characterInfo.CharacterId)
        {
            return;
        }

        if (characterInfo.Hp != 0)
        {
            if (type < SelectTargetData.Type)
            {
                return;
            }
        }
        SelectTargetData.Type = type;
        SelectTargetData.IsResetTarget = 1;
        SelectTargetData.CharacterBase = obj.CharacterBaseData;
        SelectTargetData.IsHasTarget = true;
        SelectTargetData.IsResetTarget = 0;

        var isEnemy = false;
        if (ObjManager.Instance != null
            && ObjManager.Instance.MyPlayer != null)
        {
            isEnemy = ObjManager.Instance.MyPlayer.IsMyEnemy(obj);
        }

        SelectTargetData.IsEnemy = isEnemy;

        if (isEnemy)
        {
            SelectTargetData.HpBarIco = 18;
        }
        else
        {
            SelectTargetData.HpBarIco = 17;
        }

        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            GameLogic.Instance.Scene.ShowSelectReminder(obj, isEnemy ? Color.red : Color.green);
        }
    }

    //---------------------------------------------------------------Target--------
    public void ShowCharacterPopMenu(ulong characterId,
                                     string characterName,
                                     int tableId,
                                     int lv = -1,
                                     int ladder = -1,
                                     int roleId = -1)
    {
        var operateData = new OperationListData
        {
            CharacterId = characterId,
            CharacterName = characterName,
            TableId = tableId,
            Level = lv,
            Ladder = ladder,
            RoleId = roleId
        };

        var e = new Show_UI_Event(UIConfig.OperationList, new OperationlistArguments {Data = operateData});
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void ShowItemInfoGet(int itemId)
    {
        if (itemId == -1)
        {
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        if (tbItemBase.GetWay == -1)
        {
            return;
        }
        switch(itemId)
        {
            case (int) eResourcesType.DiamondRes:
                //GameUtils.GotoUiTab(79, 0);
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.RechargeFrame,
                    new RechargeFrameArguments { Tab = 0 }));
                break ;
            default:
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ItemInfoGetUI,
                   new ItemInfoGetArguments { ItemId = itemId }));
                break ;
        }

    }

    //------------------------------------------------------------------------------------Cache-------
    public void ShowPlayerInfo(ulong characterId)
    {
        ApplyPlayerInfo(characterId, msg =>
        {
            var arg = new PlayerInfoArguments {PlayerInfoMsg = msg};
            var e = new Show_UI_Event(UIConfig.PlayerInfoUI, arg);
            EventDispatcher.Instance.DispatchEvent(e);
        });
    }

    //技能天赋点数变更
    public void SkillTalentPointChange(int skillid, int count)
    {
        var c = 0;
        if (mSkillTalent.TryGetValue(skillid, out c))
        {
            mSkillTalent[skillid] = c + count;
        }
        else
        {
            mSkillTalent.Add(skillid, count);
        }


        //技能的天赋红点
        Instance.NoticeData.SkillTalentStatus = false;
        var enumerator = mSkillTalent.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value > 0)
            {
                Instance.NoticeData.SkillTalentStatus = true;
                break;
            }
        }

        EventDispatcher.Instance.DispatchEvent(new UIEvent_SkillFrame_SkillTalentChange());
    }

    private IEnumerator StoreOperationLookSelf()
    {
        Logger.Info(".............ApplyTrading..................");
        var msg = NetManager.Instance.StoreOperationLookSelf(0);
        yield return msg.SendAndWaitUntilDone();

        if (msg.State == MessageState.Reply)
        {
            SetLoginApplyState(eLoginApplyType.Trade);
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                var controller = UIManager.Instance.GetController(UIConfig.TradingUI);
                controller.CallFromOtherClass("Init", new[] {msg.Response});
                //yield return new  WaitForSeconds(5);
                //PlayerAttr.Instance.BookRefresh();
                //PlayerAttr.Instance.InitAttributesAll();
                //PlayerAttr.Instance.CompareAttr();
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
            }
        }
    }

    public void SyncResources(Dictionary<int, int> ress)
    {
        if (ress == null)
        {
            return;
        }
        {
            // foreach(var i in ress)
            var __enumerator26 = (ress).GetEnumerator();
            while (__enumerator26.MoveNext())
            {
                var i = __enumerator26.Current;
                {
                    SetRes(i.Key, i.Value);
                }
            }
        }
    }

    private void UpdateAttributre(int id, int oldValue, int newValue)
    {
        if (id == (int) eAttributeType.MpNow
            || id == (int) eAttributeType.Strength
            || id == (int) eAttributeType.Agility
            || id == (int) eAttributeType.Intelligence
            || id == (int) eAttributeType.Endurance)
        {
            var e = new Attr_Change_Event((eAttributeType) id, oldValue, newValue);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        if (oldValue == -1)
        {
            return;
        }
        var attriId = (eAttributeType) id;
        if (ObjManager.Instance.MyPlayer != null)
        {
            var charBaseData = ObjManager.Instance.MyPlayer.CharacterBaseData;
            if (charBaseData != null)
            {
                switch (attriId)
                {
                    case eAttributeType.HpMax:
                    {
                        charBaseData.MaxHp = newValue;
                    }
                        break;
                    case eAttributeType.MpMax:
                    {
                        charBaseData.MaxMp = newValue;
                    }
                        break;
                    case eAttributeType.HpNow:
                    {
                        charBaseData.Hp = newValue;
                    }
                        break;
                    case eAttributeType.MpNow:
                    {
                        charBaseData.Mp = newValue;
                    }
                        break;
                    default:
                        break;
                }
            }
        }

        var dif = newValue - oldValue;
        if (dif > 0)
        {
            if (attriId != eAttributeType.MpNow
                && attriId != eAttributeType.HpNow
                //&& attriId != eAttributeType.HpMax
                //&& attriId != eAttributeType.MpMax
                && attriId != eAttributeType.MoveSpeed
                && attriId < eAttributeType.Count)
            {
                var roleId = GetRoleId();
                var isShowBoard = true;
                if ((attriId == eAttributeType.PhyPowerMin ||
                     attriId == eAttributeType.PhyPowerMax) && roleId == 1)
                {
                    isShowBoard = false;
                }
                if ((attriId == eAttributeType.MagPowerMin ||
                     attriId == eAttributeType.MagPowerMax) && (roleId == 0 || roleId == 2))
                {
                    isShowBoard = false;
                }
                if (isShowBoard)
                {
                    var attrName = GameUtils.AttributeName(id);
                    var attrValue = GameUtils.AttributeValue(id, dif);
                    var strDic = GameUtils.GetDictionaryText(270214);
                    var str = string.Format(strDic, attrName, attrValue);
                    var e1 = new ShowUIHintBoard(str, 97);
                    EventDispatcher.Instance.DispatchEvent(e1);
                }
            }
        }
        EventDispatcher.Instance.DispatchEvent(new AttrUIReflesh_Event(id));
    }

    public void UpdateBagData(int bagId, ItemsChangeData chagneData)
    {
        BagBaseDataModel bagBase;
        if (!PlayerDataModel.Bags.Bags.TryGetValue(bagId, out bagBase))
        {
            return;
        }
        {
            // foreach(var itemBaseData in chagneData.ItemsChange)
            var __enumerator9 = (chagneData.ItemsChange).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var itemBaseData = __enumerator9.Current;
                {
                    var isFull = bagBase.Size == bagBase.Capacity;
                    var itemBase = bagBase.Items[itemBaseData.Key];
                    var oldItemId = itemBase.ItemId;
                    if (itemBase.ItemId != -1 && itemBaseData.Value.ItemId == -1)
                    {
                        bagBase.Size--;
                    }
                    else if (itemBase.ItemId == -1 && itemBaseData.Value.ItemId != -1)
                    {
                        bagBase.Size++;
                        itemBase.TotalCount = AddBagItemCount(bagId, itemBaseData.Value.ItemId);
                    }
                    else
                    {
                        if (itemBase.ItemId != itemBaseData.Value.ItemId)
                        {
                            if (itemBaseData.Value.ItemId != -1)
                            {
                                itemBase.TotalCount = AddBagItemCount(bagId, itemBaseData.Value.ItemId);
                            }
                        }
                    }
                    itemBase.ItemId = itemBaseData.Value.ItemId;
                    itemBase.Count = itemBaseData.Value.Count;
                    itemBase.Index = itemBaseData.Value.Index;
                    itemBase.BagId = bagId;
                    itemBase.Status = 0;
                    itemBase.Exdata.InstallData(itemBaseData.Value.Exdata);
                    var tbItem = Table.GetItemBase(itemBase.ItemId);
                    itemBase.Exdata.IsntEquip = tbItem == null || tbItem.Type < 10000 || tbItem.Type > 10099;
                    NotifyBagItemCountChanged(bagId, itemBase.ItemId);

                    if (bagId == 0)
                    {
                        if (bagBase.Size == bagBase.Capacity)
                        {
                            PlayerDataModel.Bags.IsEquipFull = true;
                        }
                        else
                        {
                            PlayerDataModel.Bags.IsEquipFull = false;
                        }

                        if (isFull && !PlayerDataModel.Bags.IsEquipFull)
                        {
                            var e = new EquipBagNotFullChange();
                            EventDispatcher.Instance.DispatchEvent(e);
                        }
                    }

                    if (oldItemId != -1 && oldItemId != itemBase.ItemId)
                    {
                        NotifyBagItemCountChanged(bagId, oldItemId);
                    }
                    if (bagId == (int) eBagType.Equip
                        || bagId == (int) eBagType.Depot)
                    {
                        GetBagItemFightPoint(itemBase);
                    }


                    if (tbItem != null && tbItem.Type == 24000)
                    {
                        if (tbItem.Exdata[2] == 0)
                        {
                            itemBase.itemWithSkill = PlayerDataModel.Bags.ItemWithSkillList[1];
                        }
                        if (tbItem.Exdata[2] == 1)
                        {
                            itemBase.itemWithSkill = PlayerDataModel.Bags.ItemWithSkillList[2];
                        }
                    }
                    else
                    {
                        itemBase.itemWithSkill = PlayerDataModel.Bags.ItemWithSkillList[0];
                    }
                }
            }
        }
        if (bagId == (int) eBagType.BaseItem)
        {
            RefreshMedicineWarn();
        }
    }

    public void UpdateEquipData(int bagId, ItemsChangeData data)
    {
        if (bagId == 13)
        {
            {
                // foreach(var baseData in data.ItemsChange)
                var __enumerator14 = (data.ItemsChange).GetEnumerator();
                while (__enumerator14.MoveNext())
                {
                    var baseData = __enumerator14.Current;
                    {
                        var item = baseData.Value;
                        var equipData = PlayerDataModel.EquipList[(int) eEquipType.RingL + baseData.Key];
                        equipData.BagId = bagId;
                        equipData.ItemId = item.ItemId;
                        equipData.Index = item.Index;
                        equipData.Exdata.InstallData(item.Exdata);
                        GetBagItemFightPoint(equipData);
                    }
                }
            }
        }
        else
        {
            if (data.ItemsChange.Count == 1)
            {
                var item = data.ItemsChange[0];
                var bagType = BagIdToEquipType[bagId];
                var equipData = PlayerDataModel.EquipList[bagType];
                equipData.BagId = bagId;
                equipData.ItemId = item.ItemId;
                equipData.Index = item.Index;
                equipData.Exdata.InstallData(item.Exdata);
                GetBagItemFightPoint(equipData);

                EventDispatcher.Instance.DispatchEvent(new EquipChangeEndEvent(equipData));
            }
        }

        //检查新装备的耐久度
        RefrshEquipDurable();
        RefreshEquipStatus();
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Equip);
    }

    private void UpdateExpPercent()
    {
        var resources = PlayerDataModel.Bags.Resources;
        var tbLevelData = Table.GetLevelData(resources.Level);
        var rate = (float) resources.Exp/tbLevelData.NeedExp;
        rate = Mathf.Clamp(rate, 0.0f, 1.0f);
        resources.ExpPercent = rate;
    }

    private void UpdateFortressExpPercent()
    {
        return;

        var resources = PlayerDataModel.Bags.Resources;
        var tbLevelData = Table.GetLevelData(resources.HomeMaterial1);
        if (tbLevelData == null)
        {
            Logger.Log2Bugly(" UpdateFortressExpPercent tbLevelData =null");
            return;
        }
        resources.FortressExpNeed = tbLevelData.UpNeedExp;
        var rate = (float) resources.Achievement/tbLevelData.UpNeedExp;
        rate = Mathf.Clamp(rate, 0.0f, 1.0f);
        resources.FortressExpPercent = rate;
    }

    public IEnumerator Init()
    {
        yield return null;
    }

    public void Tick(float delta)
    {
        if (IsLevelInited)
        {
            PlayerAttr.Instance.Updata();
        }
        var mainObj = ObjManager.Instance.MyPlayer;
        if (mainObj != null)
        {
            if (mainObj.IsAutoFight() == false)
            {
                var characterInfo = SelectTargetData.CharacterBase;
                if (characterInfo != null)
                {
                    var charId = characterInfo.CharacterId;
                    var obj = ObjManager.Instance.FindCharacterById(charId);
                    if (obj != null)
                    {
                        var dis = GameUtils.Vector3PlaneDistance(mainObj.Position, obj.Position);
                        if (dis > GameUtils.DistanceRemoveTarget)
                        {
                            if (GameControl.Instance != null)
                            {
                                if (GameControl.Instance.TargetObj == obj)
                                {
                                    GameControl.Instance.TargetObj = null;
                                }
                            }
                            RemoveSelectTarget(obj);
                        }
                    }
                }
            }
        }
    }

    public void Reset()
    {
        mInitBaseAttr = new PlayerData();
        mBagItemCountData.Clear();
        mBookGropData.Clear();
        mBountyBooks.Clear();

        ResetLoginApplyState();
        if (NoticeData != null)
        {
            NoticeData.PropertyChanged -= OnPropertyNoticeChange;
        }
        NoticeData = new NoticeDataModel();
        NoticeData.PropertyChanged += OnPropertyNoticeChange;

        WeakNoticeData = new WeakNoticeDataModel();

        if (PlayerDataModel != null)
        {
            PlayerDataModel.Bags.Resources.PropertyChanged -= OnUpdataResourcesChange;
        }
        PlayerDataModel = new PlayerDataModel();
        AccountDataModel = new AccountDataModel();
        SelectTargetData = new SelectTargetDataModel();
        RewardNotice = new RewardNoticeItemDataModel();
        InitRewardGet();
        FlagData = new BitFlag(1);
        ExtData = new List<int>();
        ExtData64 = new List<long>();
        PlayerDataModel.Bags.Resources.PropertyChanged += OnUpdataResourcesChange;

        if (NetManager.Instance != null)
        {
            NetManager.Instance.StopAllCoroutines();
        }

        mAllSkills = new Dictionary<int, SkillItemDataModel>();
        mAllTalents = new Dictionary<int, TalentCellDataModel>();
        mUnionMembers = new Dictionary<ulong, CharacterBaseInfoDataModel>();
        IsLevelInited = false;
        FlagInited = false;
        PlayerInfoMsgCaches.Clear();
        if (mSkillTalent != null)
        {
            mSkillTalent.Clear();
        }
        Guid = 0;

        if (null != CityManager.Instance)
        {
            CityManager.Instance.Reset();
        }

        BattleUnionMaster = null;
    }

    public void Destroy()
    {
    }

    public class BattleCityData
    {
        public string Name;
        public int Type; //0 守城方，1攻城方
    }

    public class PlayerInfoMsgCache
    {
        public PlayerInfoMsg Info { get; set; }
        public DateTime Time { get; set; }
    }

    #region 标记位相关

    public bool GetFlag(int index)
    {
        return FlagData.GetFlag(index) == 1;
    }

    //设置扩展标记
    public void SetFlag(int index, bool flag = true)
    {
        if (-1 == index)
        {
            return;
        }
        if ((FlagData.GetFlag(index) == 1) == flag)
        {
            return;
        }
        if (flag)
        {
            FlagData.SetFlag(index);
        }
        else
        {
            FlagData.CleanFlag(index);
        }
        RefrehNoticeFlagCondition(index, flag);
        EventDispatcher.Instance.DispatchEvent(new FlagUpdateEvent(index, flag));
    }

    #endregion

    #region 扩展计数相关

    public List<int> GetExData()
    {
        return ExtData;
    }

    public int GetExData(int index)
    {
        if (0 > index || ExtData.Count <= index)
        {
            return 0;
        }

        return ExtData[index];
    }

    public int GetExData(eExdataDefine eIndex)
    {
        var index = (int) eIndex;
        if (0 > index || ExtData.Count <= index)
        {
            return 0;
        }

        return ExtData[index];
    }

    //设置扩展计数
    public void SetExData(int index, int value)
    {
        if (0 > index || ExtData.Count <= index)
        {
            return;
        }
        if (value == ExtData[index])
        {
            return;
        }
        ExtData[index] = value;


        switch (index)
        {
            case (int) eExdataDefine.e46:
            {
                var type = (int) eResourcesType.LevelRes;
                var oldValue = PlayerDataModel.Bags.Resources[type];
                if (value > oldValue)
                {
                    SetRes(type, value);
                }
            }
                break;
            case (int) eExdataDefine.e51:
            {
                PlayerDataModel.Attributes.Resurrection = value;
                if (ObjManager.Instance.MyPlayer != null)
                {
                    var charBaseData = ObjManager.Instance.MyPlayer.CharacterBaseData;
                    charBaseData.Reborn = value;
                    ObjManager.Instance.MyPlayer.RefreshnameBarod();
                }
            }
                break;
            case (int) eExdataDefine.e52:
            {
                var e = new AttributeDistributionChange(value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e59:
            case (int) eExdataDefine.e60:
            case (int) eExdataDefine.e61:
            {
                var e = new SettingExdataUpdate((eExdataDefine) index, value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e82:
            {
                var e = new ElfExdataUpdate((eExdataDefine) index, value);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case (int) eExdataDefine.e93:
            case (int) eExdataDefine.e98:
            case (int) eExdataDefine.e99:
            case (int) eExdataDefine.e250:
            case (int) eExdataDefine.e400:
            {
                var e = new ArenaExdataUpdate((eExdataDefine) index, value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e9:
            case (int) eExdataDefine.e10:
            case (int) eExdataDefine.e11:
            case (int) eExdataDefine.e12:
            {
                var e = new FruitExdataUpdate((eExdataDefine) index, value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e282:
            case (int) eExdataDefine.e286:
            case (int) eExdataDefine.e287:
            case (int) eExdataDefine.e288:
            {
                var e = new BattleUnionExdataUpdate((eExdataDefine) index, value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e312:
            {
                var e = new RankOperationEvent(3);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e66:
            case (int) eExdataDefine.e67:
            {
                var e = new UIEvent_Answer_ExdataUpdate(index, value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            case (int) eExdataDefine.e71:
            {
                var e = new UIEvent_Sail_ExdataUpdate(index, value);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
                break;
            default:
            {
                var dungeonId = 0;
                List<int> dungeonList;
                if (DungeonResetExData.TryGetValue(index, out dungeonList))
                {
                    {
                        var __list4 = dungeonList;
                        var __listCount4 = __list4.Count;
                        for (var __i4 = 0; __i4 < __listCount4; ++__i4)
                        {
                            var i = __list4[__i4];
                            {
                                var e = new DungeonResetCountUpdate(i, value);
                                EventDispatcher.Instance.DispatchEvent(e);
                            }
                        }
                    }
                    return;
                }
                if (DungeonEnterExData.TryGetValue(index, out dungeonList))
                {
                    {
                        var __list5 = dungeonList;
                        var __listCount5 = __list5.Count;
                        for (var __i5 = 0; __i5 < __listCount5; ++__i5)
                        {
                            var i = __list5[__i5];
                            {
                                var e = new DungeonEnterCountUpdate(i, value);
                                EventDispatcher.Instance.DispatchEvent(e);
                            }
                        }
                    }
                    return;
                }
            }
                break;
        }
        EventDispatcher.Instance.DispatchEvent(new ExDataUpDataEvent(index, value));
    }

    public void SetExDataNet(Dictionary<int, int> dic)
    {
        NetManager.Instance.StartCoroutine(SetExDataCoroutine(dic));
    }

    private IEnumerator SetExDataCoroutine(Dictionary<int, int> dic)
    {
        var data = new Dict_int_int_Data();
        {
            // foreach(var i in dic)
            var __enumerator6 = (dic).GetEnumerator();
            while (__enumerator6.MoveNext())
            {
                var i = __enumerator6.Current;
                {
                    data.Data.Add(i.Key, i.Value);
                }
            }
        }
        var msg = NetManager.Instance.SetExData(data);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("SetExData Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("SetExData Error!............State..." + msg.State);
        }
    }


    public void SetFlagNet(Int32Array tureArray, Int32Array falseArray = null)
    {
        if (null == falseArray)
        {
            falseArray = new Int32Array();
        }
        NetManager.Instance.StartCoroutine(SetFlagCoroutine(tureArray, falseArray));
    }

    private IEnumerator SetFlagCoroutine(Int32Array tureArray, Int32Array falseaArray)
    {
        var msg = NetManager.Instance.SetFlag(tureArray, falseaArray);
        yield return msg.SendAndWaitUntilDone();
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("SetExData Error!............ErrorCode..." + msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("SetExData Error!............State..." + msg.State);
        }
    }

    /// <summary>
    ///     检查条件表中的条件是否满足
    /// </summary>
    /// <param name="nConditionId">条件表id</param>
    /// <returns>条件满足时返回0， 不满足时返回字典id</returns>
    public int CheckCondition(int nConditionId)
    {
        if (nConditionId == -1)
        {
            return 0;
        }
        var tbcon = Table.GetConditionTable(nConditionId);
        if (tbcon == null)
        {
            Logger.Error("In CheckCondition() tbcon == null. condition id = {0}", nConditionId);
            return 0;
        }
        if (tbcon.Role != -1)
        {
            if (!BitFlag.GetLow(tbcon.Role, GetRoleId()))
            {
                return tbcon.RoleDict;
            }
        }
        for (var i = 0; i != tbcon.TrueFlag.Length; ++i)
        {
            if (tbcon.TrueFlag[i] == -1)
            {
                continue;
            }
            if (!GetFlag(tbcon.TrueFlag[i]))
            {
                return tbcon.FlagTrueDict;
            }
        }
        for (var i = 0; i != tbcon.FalseFlag.Length; ++i)
        {
            if (tbcon.FalseFlag[i] == -1)
            {
                continue;
            }
            if (GetFlag(tbcon.FalseFlag[i]))
            {
                return tbcon.FlagFalseDict;
            }
        }
        for (var i = 0; i != tbcon.ExdataId.Length; ++i)
        {
            if (tbcon.ExdataId[i] == -1)
            {
                continue;
            }
            var nExdataValue = GetExData(tbcon.ExdataId[i]);
            if (tbcon.ExdataMin[i] != -1 && nExdataValue < tbcon.ExdataMin[i])
            {
                return tbcon.ExdataDict[i];
            }
            if (tbcon.ExdataMax[i] != -1 && nExdataValue > tbcon.ExdataMax[i])
            {
                return tbcon.ExdataDict[i];
            }
        }

        for (var i = 0; i != tbcon.ItemId.Length; ++i)
        {
            var nItemId = tbcon.ItemId[i];
            if (nItemId < 0)
            {
                continue;
            }
            var nCount = GetItemCount(nItemId);
            if (tbcon.ItemCountMin[i] != -1 && nCount < tbcon.ItemCountMin[i])
            {
                return tbcon.ItemDict[i];
            }
            if (tbcon.ItemCountMax[i] != -1 && nCount > tbcon.ItemCountMax[i])
            {
                return tbcon.ItemDict[i];
            }
        }
        if (tbcon.OpenTime[0] > 0)
        {
            var endTime = Game.Instance.ServerTime;
            var sm = OpenTime.Month;
            var em = endTime.Month;
            var sy = OpenTime.Year;
            var ey = endTime.Year;
            var diffMonth = (ey - sy)*12 + (em - sm);
            if (diffMonth < tbcon.OpenTime[0])
            {
                return tbcon.OpenTimeDict[0];
            }
        }
        if (tbcon.OpenTime[1] > 0)
        {
            var endTime = Game.Instance.ServerTime;
            var sy = OpenTime.Year;
            var ey = endTime.Year;
            var diffYear = ey - sy;
            if (diffYear < tbcon.OpenTime[1])
            {
                return tbcon.OpenTimeDict[1];
            }
        }
        return 0;
    }

    //初始化一些trigger
    public void InitTrigger()
    {
        EventDispatcher.Instance.DispatchEvent(new StoreCacheTriggerEvent());
    }

    #endregion

    #region 成就快捷icon

    public SortedDictionary<int, NoticeClass> NociceFlag = new SortedDictionary<int, NoticeClass>();

    public class NoticeClass
    {
        public NoticeClass()
        {
            Flag = false;
            Count = -1;
            Record = null;
        }

        public int Count;
        public bool Flag;
        public RewardInfoRecord Record;
    }

    public RewardNoticeItemDataModel RewardNotice; //主界面快捷button数据
    public static List<GiftRecord> RewardGiftRecord = new List<GiftRecord>();
    public int RewardFastKeySortId = 9999;
    private Coroutine RewardCoroutine;
    //初始化数据
    public void InitRewardGet()
    {
        NociceFlag.Clear();
        Table.ForeachRewardInfo(table =>
        {
            if (table.Sort == -1)
            {
                return true;
            }
            var noticeClass = new NoticeClass();
            noticeClass.Record = table;
            NociceFlag.Add(table.Sort, noticeClass);
            return true;
        });
        RewardGiftRecord.Clear();
        Table.ForeachGift(record =>
        {
            if (record.Type == (int) eRewardType.OnlineReward)
            {
                RewardGiftRecord.Add(record);
            }
            return true;
        });
    }

    //设置RewardNotice数据  count
    public void SetNoticeCount(int sort, int mCount, bool IsReflesh = true)
    {
        if (NociceFlag.ContainsKey(sort))
        {
            var ii = NociceFlag[sort];
            if ((ii.Count <= 0) != (mCount <= 0))
            {
                ii.Count = mCount;
                ii.Flag = (mCount > 0);
                if (IsReflesh)
                {
                    RefleshRewardInfo();
                }
            }
            if (ii.Record.Id == RewardNotice.Id)
            {
                RewardNotice.Count = mCount;
            }
        }
    }

    //设置按钮flag
    public void SetNoticeFlag(int sort, bool bValue, bool IsReflesh = true)
    {
        if (NociceFlag.ContainsKey(sort))
        {
            var item = NociceFlag[sort];
            if (item.Flag == bValue)
            {
                return;
            }
            item.Flag = bValue;
            if (sort > RewardFastKeySortId)
            {
                return;
            }
            if (IsReflesh)
            {
                RefleshRewardInfo();
            }
        }
    }

    //刷新按钮显示
    public void RefleshRewardInfo()
    {
        //if (RewardNotice.ShowButtonState == 2) //当副本时按钮隐藏，不走逻辑
        //{
        //    return;
        //}
        foreach (var item in NociceFlag)
        {
            if (item.Key == NoticeStrToSort["LongTime"])
            {
                if (SetOnlineTime())
                {
                    return;
                }
            }

            if (!item.Value.Flag)
            {
                continue;
            }
            var record = item.Value.Record;
            if (record.ConditionId != -1)
            {
                var ok = CheckCondition(record.ConditionId);
                if (ok != 0)
                {
                    continue;
                }
            }

            RewardNotice.Count = item.Value.Count;
            RewardNotice.Id = record.Id;
            RewardFastKeySortId = item.Key;
            RewardNotice.ShowButtonState = 1; //显示button按钮
            if (item.Value.Count > 0)
            {
                RewardNotice.FlagState = 2; //显示数字
            }
            else
            {
                RewardNotice.FlagState = 1; //显示叹号
            }
            RewardNotice.State = 0; //不显示时间label
            if (RewardCoroutine != null)
            {
                NetManager.Instance.StopCoroutine(RewardCoroutine);
                RewardCoroutine = null;
            }
            return;
        }
        RewardNotice.ShowButtonState = 0; //所有都隐藏
        RewardFastKeySortId = 9999;
    }

    //noticestr 对应优先级的映射关系
    public Dictionary<string, int> NoticeStrToSort = new Dictionary<string, int>
    {
        {"WishDrawFree", 0},
        {"WishFlag", 0},
        {"ElfDraw", 1},
        {"ElfFlag", 1},
        {"ActivityLevel", 2},
        {"ActivityTimeLength", 3},
        {"ActivityLoginSeries", 4},
        {"ActivityLoginAddup", 5},
        {"OffLine", 6},
        {"SevenDay", 10},
        {"MailNew", 20},
        {"MailCount", 20},
        {"HasAchievement", 21},
        {"AchievementFlag", 21},
        {"HatchFinish", 22},
        {"PetMission", 23},
        {"FarmTotal", 24},
        {"MineComplete4", 25},
        {"LogPlaceComplete4", 26},
        {"SailingNotice", 27},
        {"RankingCanLike", 28},
        {"RankCanLikeFlag", 28},
        {"LongTime", 99},
        {"EquipBagFree", 100},
        {"ItemBagFree", 101},
        {"DepotBagFree", 102},
        {"ActivityCompensateActive", 103}
    };

    //根据notice字符数串刷新按钮
    public void RewardNoticeReflesh(string str)
    {
        if (NoticeStrToSort.ContainsKey(str))
        {
            var index = NoticeStrToSort[str];
            switch (str)
            {
                case "WishDrawFree":
                {
                    SetNoticeFlag(index, NoticeData.WishDrawFree, NoticeData.WishFlag);
                    break;
                }
                case "ElfDraw":
                {
                    SetNoticeFlag(index, NoticeData.ElfDraw, NoticeData.ElfFlag);
                    break;
                }
                case "ActivityLevel":
                {
                    SetNoticeCount(index, NoticeData.ActivityLevel);
                    break;
                }
                case "ActivityTimeLength":
                {
                    SetNoticeCount(index, NoticeData.ActivityTimeLength);
                    break;
                }
                case "ActivityLoginSeries":
                {
                    SetNoticeCount(index, NoticeData.ActivityLoginSeries);
                    break;
                }
                case "ActivityLoginAddup":
                {
                    SetNoticeCount(index, NoticeData.ActivityLoginAddup);
                    break;
                }
                case "ActivityCompensateActive":
                {
                    SetNoticeCount(index, NoticeData.ActivityCompensateActive);
                    break;
                }
                case "MailNew":
                {
                    SetNoticeFlag(index, NoticeData.MailNew);
                    break;
                }
                case "MailCount":
                {
                    SetNoticeCount(index, NoticeData.MailCount);
                    break;
                }
                case "HasAchievement":
                {
                    SetNoticeFlag(index, NoticeData.HasAchievement, NoticeData.AchievementFlag);
                    break;
                }
                case "HatchFinish":
                {
                    SetNoticeFlag(index, NoticeData.HatchFinish);
                    break;
                }
                case "PetMission":
                {
                    SetNoticeFlag(index, NoticeData.PetMission);
                    break;
                }
                case "FarmTotal":
                {
                    SetNoticeFlag(index, NoticeData.FarmTotal);
                    break;
                }
                case "MineComplete4":
                {
                    SetNoticeFlag(index, NoticeData.MineComplete4);
                    break;
                }
                case "LogPlaceComplete4":
                {
                    SetNoticeFlag(index, NoticeData.LogPlaceComplete4);
                    break;
                }
                case "SailingNotice":
                {
                    SetNoticeFlag(index, NoticeData.SailingNotice);
                    break;
                }
                case "RankingCanLike":
                {
                    SetNoticeFlag(index, NoticeData.RankingCanLike, NoticeData.RankCanLikeFlag);
                    break;
                }
                case "OffLine":
                {
                    SetNoticeFlag(index, NoticeData.OffLine);
                    break;
                }
                case "EquipBagFree":
                {
                    SetNoticeFlag(index, NoticeData.EquipBagFree);
                    break;
                }
                case "ItemBagFree":
                {
                    SetNoticeFlag(index, NoticeData.ItemBagFree);
                    break;
                }
                case "DepotBagFree":
                {
                    SetNoticeFlag(index, NoticeData.DepotBagFree);
                    break;
                }
                case "SevenDay":
                {
                    SetNoticeFlag(index, NoticeData.SevenDay);
                    break;
                }
                case "RankCanLikeFlag":
                case "WishFlag":
                case "AchievementFlag":
                case "ElfFlag":
                {
                    RefleshRewardInfo();
                    break;
                }
                default:
                    break;
            }
        }
    }

    //快捷按钮跳转
    //潜规则铁匠人物sceneNpcId
    public void RewardGotoUI()
    {
        if (RewardFastKeySortId != -1)
        {
            NoticeClass item;
            if (NociceFlag.TryGetValue(RewardFastKeySortId, out item))
            {
                var record = item.Record;
                if (RewardFastKeySortId == NoticeStrToSort["EquipBagFree"] ||
                    RewardFastKeySortId == NoticeStrToSort["ItemBagFree"] ||
                    RewardFastKeySortId == NoticeStrToSort["DepotBagFree"])
                {
                    if (GetRes((int) eResourcesType.VipLevel) < 3
                        && RewardFastKeySortId != NoticeStrToSort["EquipBagFree"]
                        && RewardFastKeySortId != NoticeStrToSort["ItemBagFree"])
                    {
                        EventDispatcher.Instance.DispatchEvent(new RewardMessageOpetionClick(2));
                    }
                    else
                    {
                        GameUtils.GotoUiTab(record.UIId, record.UIParam);
                    }
                }
                else
                {
                    GameUtils.GotoUiTab(record.UIId, record.UIParam);
                }
            }
        }
    }

    //显示成就时间倒计时
    private bool SetOnlineTime()
    {
        if (RewardGiftRecord != null)
        {
            var timeSeconds = Game.Instance.OnLineSeconds > 0 ? Game.Instance.OnLineSeconds : 0;
            var seconds = GetExData(31) + timeSeconds;
            for (var i = 0; i < RewardGiftRecord.Count; i++)
            {
                var record = RewardGiftRecord[i];
                if (!GetFlag(record.Flag))
                {
                    if (seconds < record.Param[0])
                    {
                        if (RewardCoroutine != null)
                        {
                            NetManager.Instance.StopCoroutine(RewardCoroutine);
                        }
                        var sortId = NoticeStrToSort["LongTime"];
                        RewardNotice.State = 1; //显示时间label
                        RewardNotice.Id = NociceFlag[sortId].Record.Id; //默认优先级99
                        RewardNotice.ShowButtonState = 1;
                        RewardFastKeySortId = sortId;
                        RewardNotice.FlagState = 0;
                        RewardCoroutine = NetManager.Instance.StartCoroutine(OnLineCoroutine(record.Param[0] - seconds));
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public int GetAchievementPoint()
    {
        return GetRes((int) eResourcesType.AchievementScore);
        //return GetExData(eExdataDefine.e50);
    }

    //在线时长显示
    private IEnumerator OnLineCoroutine(int seconds)
    {
        var finishTime = Game.Instance.ServerTime.AddSeconds(seconds);
        while (finishTime > Game.Instance.ServerTime)
        {
            yield return new WaitForSeconds(0.3f);
            RewardNotice.LabelStr = GameUtils.GetTimeDiffString(finishTime);
        }
        RewardNotice.ShowButtonState = 0; //快捷按钮隐藏
    }

    #endregion
}