#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

public class BattleUnionController : IControllerBase
{
    public const int INTERVEL_TIME = 5;
    public const int REFLESH_COUNT = 6;

    public BattleUnionController()
    {
        //按钮事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionBtnCreateUnion.EVENT_TYPE, BtnCreateUnion); //创建战盟
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionBtnPassApply.EVENT_TYPE, BtnPassApply); //批量同意申请
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionCharacterClick.EVENT_TYPE, CharacterClick); //人物点击返回事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionBtnDonation.EVENT_TYPE, BtnDonation); // 捐赠按钮组
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionDonationItemClick.EVENT_TYPE, DonationItemClick);
            //捐赠的物品点击
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionBuffUpShow.EVENT_TYPE, OnBuffIcon);
            //buff按钮点击事件，显示buff页面信息
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionBossClick.EVENT_TYPE, BossClick); //bossList点击
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionOtherListClick.EVENT_TYPE, UnionOtherListClick);
            //其他战盟列表点击事件
        //其他消息事件
        EventDispatcher.Instance.AddEventListener(ExDataInitEvent.EVENT_TYPE, OnExDataInit); //初始化ExData
        EventDispatcher.Instance.AddEventListener(BattleUnionExdataUpdate.EVENT_TYPE, OnUpdateExData); //ExData变更相应
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionCommunication.EVENT_TYPE, CommunicationRequest); //菜单点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionJoinReply.EVENT_TYPE, UnionSyncMessage); //菜单点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionGetCharacterID.EVENT_TYPE, InvitationSetCharacter);
            //操作列表请求战盟
        EventDispatcher.Instance.AddEventListener(UIEvent_BattleShopCellClick.EVENT_TYPE, BuyItemClick); // 商品购买
        //    EventDispatcher.Instance.AddEventListener(BattleUnionExdataUpdate.EVENT_TYPE, UpdateExData);  // 同意玩家申请加入战盟
        EventDispatcher.Instance.AddEventListener(UIEvent_BattleBtnAutoAccept.EVENT_TYPE, BtnSetAutoAccept); // 战盟自动申请成功
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionTabPageClick.EVENT_TYPE, TabPageClick); // tab点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionTabPageClick2.EVENT_TYPE, TabPageClick2); // tab点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionDonationItem.EVENT_TYPE, BtnDonationItem); // 捐献物品点击事件
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionSyncDataChange.EVENT_TYPE, SyncDataChange); // 异步数据变化同步
        EventDispatcher.Instance.AddEventListener(UIEvent_UnionBattlePageCLick.EVENT_TYPE, AttackPageClick); // tab点击事件

        EventDispatcher.Instance.AddEventListener(UIEvent_UnionOperation.EVENT_TYPE, UnionOperation); // 功能操作
        EventDispatcher.Instance.AddEventListener(BattleUnionCountChange.EVENT_TYPE, OnBingingCountChange); // 竞价数量变化
        EventDispatcher.Instance.AddEventListener(BattleUnionSyncOccupantChange.EVENT_TYPE, SyncOccupantChange);
            // 城主信息变化同步
        EventDispatcher.Instance.AddEventListener(BattleUnionSyncChallengerDataChange.EVENT_TYPE,
            SyncChallengerDataChange); // 攻城方信息变化同步

        CleanUp();
    }

    private GuildRecord _mGuildRecord;
    public List<int> BuffIdInit = new List<int> {101, 201, 301, 401}; //默认0级buff索引的下一级buffid
    public int BuffSelected; //buff选择id
    public ulong CharacterID; //被操作玩家id
    private string InputStr = string.Empty;
    private readonly Dictionary<int, List<StoreRecord>> mCacheDic = new Dictionary<int, List<StoreRecord>>();
    private readonly Dictionary<int, int> mCacheDicLP = new Dictionary<int, int>();

    private readonly List<BattleUnionTeamSimpleDataModel> mCatchOhterUnion = new List<BattleUnionTeamSimpleDataModel>();
        //战盟临时列表

    public ulong MemberId; //成员id
    public GuildRecord mGuildRecord;
    private readonly Dictionary<int, int> mOtherUnionDict = new Dictionary<int, int>(); //其他战盟id ，inde 字典

    public List<DateTime> mRefleshTime = new List<DateTime>();
        // 0 战盟信息刷新  //1战盟捐献刷新    //2 申请列表  //3 简单信息  //4其他战盟  //5攻城战

    public RefleshType mRefleshType = RefleshType.UnionData; //根据时间是否刷新战盟相关界面
    public int OtherUnionSelectIndex = -1; //其他战盟选择index 
    public string strOnline = ""; //dictinory 字符串
    public UnionIDState UionState = UnionIDState.OtherJoin; //union加入状态
    public int unionLevelChanged; //战盟等级是否变化
    // public int LevelChange = 0; // 判断等级是否变化  
    public int unionMaxLevel; //战盟最大可以达到的等级
    //捐赠物品的状态
    public enum ItemState
    {
        Wait = 0, //等待刷新
        Normal = 1 //有任务
    }

    //战盟信息刷新类型
    public enum RefleshType
    {
        UnionData = 0, //战盟信息刷新 
        DonationData = 1, //战盟捐献刷新
        ApplyData = 2, //申请列表
        MemberDetailData = 3, //成员详细信息
        UnionDataSimple = 4, //战盟信息简单信息 
        OtherUnion = 5, //其他战盟
        AttackData = 6 //攻城战信息
    }

    //Page页
    public enum TabPage
    {
        PageInfo = 0, //信息
        PageMember = 1, //信息
        PageBuild = 2, //建设
        PageLevel = 3, //升级
        PageShop = 4, //Boss
        PageCity = 5, //城市
        PageOther = 6 //其他战盟
        //   PageQuit = 6,        //退出
    }

    //战盟id变化时产生的原因
    public enum UnionIDState
    {
        CreateJoin = 0, //创建战盟
        OtherJoin = 1 //被邀请，被审批进入战盟
    }

    public BattleUnionAttackCityDataModel AttackCity { get; set; }

    public BattleUnionDataModel BattleData
    {
        get { return PlayerDataManager.Instance.BattleUnionDataModel; }
        set { PlayerDataManager.Instance.BattleUnionDataModel = value; }
    }

    public BattleUnionBossDataModel Boss { get; set; }
    public BattleUnionBuffDataModel Buff { get; set; }
    public BattleUnionBuildDataModel Build { get; set; }
    public BattleUnionInfoDataModel Info { get; set; }

    public Dictionary<ulong, CharacterBaseInfoDataModel> mUnionMembers
    {
        get { return PlayerDataManager.Instance.mUnionMembers; }
        set { PlayerDataManager.Instance.mUnionMembers = value; }
    }

    public BattleUnionOtherUnionDataModel OtherUnion { get; set; }
    public BattleUnionShopDataModel Shop { get; set; }

    #region 消息的处理

    //战盟操作事件
    public void UnionOperation(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionOperation;
        switch (e.Type)
        {
            //case 0:
            //    BtnOtherUnion();   //其他战盟
            //    break;
            case 1:
                BtnOtherReturn(); //其他战盟页面返回
                break;
            case 2:
                BtnApplyJoin(); //加入其他战盟
                break;
            case 3:
                BtnApplyList(); //战盟申请列表
                break;
            //case 4:
            //    BtnMemberInfo();  //成员信息
            //    break;
            case 5:
                BtnAddMember(); //添加成员按钮事件
                break;
            case 6:
                FindMemberOK(); //添加战盟成员按钮
                break;
            case 7:
                FindUIClose(); //关闭添加成员页面
                break;
            case 8:
                BtnInfoReturn(); //ShowInfo(0)
                break;
            case 9:
                BtnnDetailBack(); //详细信息返回
                break;
            case 10:
                BtnModifyNotice(); //修改公告
                break;
            case 11:
                BtnSeeLog(); //显示日志
                break;
            case 12:
                BtnCloseSeeLog(); //关闭日志
                break;
            //case 13:
            //    BtnDonationItem();//捐献物品
            //    break;
            //case 14:
            //    BtnUnionBuffUpShow();  //buff页面显示
            //    break;
            //case 15:
            //    BtnUnionBuffActive();  //buff激活
            //    break;
            case 16:
                BtnBuffUpOK(); //buff升级按钮
                break;
            case 17:
                BtnUnionLevelup(); //战盟升级
                break;
            case 18:
                BtnUpBuffCancel(); //buff升级页面关闭
                break;
            case 19:
                BtnBossGetReward(); //boss获得奖励
                break;
            case 20:
                TabOutUnion(); //退出战盟按钮
                break;
            case 21:
                NoticeSaveShow(); //公告修改时，显示保存按钮
                break;
            case 22:
                BuildShowHelp(); //显示帮助
                break;
            case 23:
                BuildShowGoldPage(); //显示捐赠金币page
                break;
            case 24:
                BuildShowItemPage(); //显示捐赠物品page
                break;
            case 25:
                BtnUnionBuffUpShow(); //显示buff升级提示
                break;
            case 26:
                BtnAddBidding(); //竞价
                break;
            case 27:
                AttackJoin(); //加入城战
                break;
            //case 28:
            //    BattleBiddingOk();  //确定竞价
            //    break;
            case 29:
                BattleBiddingClose(); //关闭竞价窗口
                break;
            case 30:
                BtnBiddingSub(); //竞价确定
                break;
            case 31:
                ClearCreateInput(); //清除创建名称
                break;
            case 32:
                InitCreateInput(); //初始化创建名称
                break;
        }
    }

    public void BuildShowGoldPage()
    {
        Build.MontyOrItemPage = 0;
    }

    public void BuildShowItemPage()
    {
        Build.MontyOrItemPage = 1;
    }

    //sc 战盟等级变化更新。
    private void SyncDataChange(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionSyncDataChange;
        if (e.Type == 0)
        {
            BattleData.MyUnion.Money = e.param2;
            BattleData.MyUnion.Level = e.param1;
            mGuildRecord = Table.GetGuild(BattleData.MyUnion.Level);
            unionLevelChanged = BattleData.MyUnion.Level;
            InitShop();
            InitBuff();
            RefleshUnionLevel();
            UnionCanLevel();
            RefleshBuffUI(BuffSelected);
        }
    }

    //设置战盟升级颜色显示
    private void SetBattleMoneyString()
    {
        if (mGuildRecord.ConsumeUnionMoney <= 0)
        {
            BattleData.ContributionStr = string.Empty;
        }
        else
        {
            if (BattleData.MyUnion.Money >= mGuildRecord.ConsumeUnionMoney)
            {
                BattleData.ContributionStr = "[CFE5FF]" + mGuildRecord.ConsumeUnionMoney + "[-]";
            }
            else
            {
                BattleData.ContributionStr = "[FF0000]" + mGuildRecord.ConsumeUnionMoney + "[-]";
            }
        }
    }

    //根据服务器地址获取战盟信息
    public void GetMyUnionInfoByServerId(int isSimple, int listState)
    {
        NetManager.Instance.StartCoroutine(ApplyUnionDataByServerId(PlayerDataManager.Instance.ServerId, isSimple,
            listState));
    }

    public IEnumerator ApplyUnionDataByServerId(int ServerID, int isSimple, int listState)
    {
        using (new BlockingLayerHelper(0))
        {
            //0 详细，1简单
            var msg = NetManager.Instance.ApplyAllianceDataByServerId(ServerID, isSimple);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    SetApplyUnionResponse(msg.Response, isSimple);
                    UpdatePage();
                    if (mRefleshTime.Count > 0)
                    {
                        mRefleshTime[0] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                    }
                    ShowInfo(listState);
                }
                else
                {
                    // ServerID   Error_CharacterNoAlliance    Error_CharacterNoAlliance  Error_AllianceNotFind
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //设置战盟信息申请response返回的数据
    public void SetApplyUnionResponse(AllianceData data, int isSimple)
    {
        BattleData.MyUnion.UnionID = data.Id;
        BattleData.MyUnion.UnionName = data.Name;
        BattleData.MyUnion.ChiefID = data.Leader;
        BattleData.MyUnion.ServerID = data.ServerId;
        BattleData.MyUnion.Notice = data.Notice;
        BattleData.MyUnion.Force = data.FightPoint;
        BattleData.MyUnion.Level = data.Level;
        BattleData.MyUnion.Money = data.Money;
        mGuildRecord = Table.GetGuild(BattleData.MyUnion.Level);
        SetBattleMoneyString();
        BattleData.MyUnion.AutoAccept = data.AutoAgree;
        if (string.IsNullOrEmpty(data.Notice))
        {
            BattleData.MyUnion.Notice = GameUtils.GetDictionaryText(100000905);
        }
        Info.VarNotice = BattleData.MyUnion.Notice;
        if (isSimple == 1)
        {
            return;
        }

        var count = 0;
        if (strOnline == "")
        {
            strOnline = GameUtils.GetDictionaryText(220953);
        }
        var infoData = new List<CharacterBaseInfoDataModel>();
        mUnionMembers.Clear();
        for (var i = 0; i < data.Members.Count; i++)
        {
            var item = data.Members[i];
            var baseData = new CharacterBaseInfoDataModel();
            baseData.Index = i;
            baseData.ID = item.Guid;
            baseData.Name = item.Name;
            baseData.Ladder = item.Ladder;
            baseData.Level = item.Level;
            baseData.CareerId = item.TypeId;
            var tbCharacterBase = Table.GetCharacterBase(item.TypeId);
            if (tbCharacterBase != null)
            {
                baseData.Career = tbCharacterBase.Name;
            }
            baseData.DonationCount = item.MeritPoint;
            baseData.Force = item.FightPoint;
            baseData.Online = item.Online;
            if (baseData.Online == 0)
            {
                //baseData.Scene = "";
                var mLostTime = Game.Instance.ServerTime;
                if (item.LostTime != 0)
                {
                    mLostTime = Extension.FromServerBinary(item.LostTime);
                }
                baseData.LastTime = GameUtils.GetLastTimeDiffString(mLostTime);
            }
            else
            {
                baseData.LastTime = strOnline;
            }
            var tbSene = Table.GetScene(item.SceneId);
            if (tbSene == null)
            {
                baseData.Scene = "";
            }
            else
            {
                baseData.Scene = tbSene.Name;
            }
            infoData.Add(baseData);
            if (data.Leader == item.Guid)
            {
                BattleData.MyUnion.ChiefName = item.Name;
            }
        }
        //排序
        MemberListSort(infoData);
        for (var i = 0; i < infoData.Count; i++)
        {
            mUnionMembers.Add(infoData[i].ID, infoData[i]);
        }
        var chacterid = PlayerDataManager.Instance.GetGuid();
        if (mUnionMembers.ContainsKey(chacterid))
        {
            BattleData.MyPorp = new CharacterBaseInfoDataModel(mUnionMembers[chacterid]);
        }
        SetAccess();
        BattleData.MyUnion.NowCount = mUnionMembers.Count;
        BattleData.MyUnion.TotalCount = mGuildRecord.MaxCount;
        // SetDonationItem(data.Missions);
    }

    //设置捐赠物品
    public void SetDonationItem(List<AllianceMissionDataOne> Missions)
    {
        var count = 0;
        for (var i = 0; i < Missions.Count; i++)
        {
            var ii = Build.DonationItem[i];
            ii.TotalCount = Missions[i].MaxCount;
            ii.TaskID = Missions[i].Id;
            var tbMission = Table.GetGuildMission(Missions[i].Id);
            if (tbMission == null)
            {
                return;
            }
            ii.ItemIDData.ItemId = tbMission.ItemID;
            ii.LeftCount = Missions[i].MaxCount - Missions[i].NowCount;
            ii.State = Missions[i].State;
            ii.NextTime = Missions[i].NextTime;
            if (ii.State == (int) ItemState.Wait)
            {
                SetDonationIitemNull(i);
            }
            //if (Missions[i].Id == -1)
            //{
            //    SetDonationIitemNull(i);
            //}
        }
    }

    //设置被操作的人物id 如被邀请
    public void InvitationSetCharacter(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionGetCharacterID;
        CharacterID = e.CharacterID;
    }

    //战盟消息通知SC包
    public void UnionSyncMessage(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionJoinReply;
        // Logger.Info(String.Format("Type ={0},id ={1},name1 ={2},name2 ={3}",e.Type,e.AllianceId,e.Name1,e.Name2));
        switch (e.Type)
        {
            case 0: //邀请加入战盟
            {
                joinUnionReply(e.Name1, e.AllianceId, e.Name2);
            }
                break;
            case 1:
            {
            }
                break;
            case 2: //你被拒绝加入战盟{0} Name1 = 拒绝你的名称   Name2 = 战盟名
            {
                UIManager.Instance.ShowMessage(MessageBoxType.Ok,
                    string.Format(GameUtils.GetDictionaryText(220955), e.Name1, e.Name2));
            }
                break;
            case 3: //被同意邀请后反馈  Name1 = 同意你的名称   Name2 = 战盟名
            {
                UIManager.Instance.ShowMessage(MessageBoxType.Ok,
                    string.Format(GameUtils.GetDictionaryText(220978), e.Name1, e.Name2));
            }
                break;
            case 4: //被拒绝邀请后反馈   Name1 = 拒绝你的名称   Name2 = 战盟名
            {
                UIManager.Instance.ShowMessage(MessageBoxType.Ok,
                    string.Format(GameUtils.GetDictionaryText(220979), e.Name1, e.Name2));
            }
                break;
            case 5: //邀请超时后反馈 Name1 = 拒绝你的名称   Name2 = 战盟名
            {
                UIManager.Instance.ShowMessage(MessageBoxType.Ok,
                    string.Format(GameUtils.GetDictionaryText(220979), e.Name1, e.Name2));
            }
                break;
            case 6: //有成员申请
            {
                if (e.AllianceId == 0)
                {
                    PlayerDataManager.Instance.NoticeData.BattleList = false;
                }
                else
                {
                    PlayerDataManager.Instance.NoticeData.BattleList = true;
                }
            }
                break;
        }
    }

    //被邀请加入后的应答 
    public void joinUnionReply(string name1, int allianceId, string name2)
    {
        //自动加入邀请的战盟
        if (BattleData.CheckBox.CreateAutoAccept)
        {
            NetManager.Instance.StartCoroutine(ApplyOperation(3, allianceId));
        }
        else
        {
            //A邀请B加入战盟
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                string.Format(GameUtils.GetDictionaryText(220900), name1, name2), "",
                () => { NetManager.Instance.StartCoroutine(ApplyOperation(3, allianceId)); },
                () => { NetManager.Instance.StartCoroutine(ApplyOperation(4, allianceId)); });
        }
    }

    //列表处理函数 3同意申请 4拒绝申请 13邀请入盟 14提升领袖 15提升权限 16降低权限 17请出战盟
    public void CommunicationRequest(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionCommunication;
        CharacterID = e.CharacterId;
        var accessId = -1; //权限ID（值）
        switch (e.ListIndex)
        {
            case 3: //同意申请   state=1;
                NetManager.Instance.StartCoroutine(ApplyOperationCharacter(1, MemberId));
                break;

            case 4: //拒绝申请  state=1;
                NetManager.Instance.StartCoroutine(ApplyOperationCharacter(2, MemberId));
                break;

            case 13: //邀请入盟  
            {
                NetManager.Instance.StartCoroutine(ApplyOperationCharacter(0, CharacterID));
            }
                break;
            case 14: //提升领袖   state=0
            {
                //无权提升为领袖
                if (BattleData.MyPorp.Ladder != (int) battleAccess.Chief)
                {
                    return;
                }
                var msgStr = GameUtils.GetDictionaryText(220981);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, msgStr, "",
                    () =>
                    {
                        NetManager.Instance.StartCoroutine(ChangeJurisdiction(BattleData.MyUnion.UnionID, MemberId,
                            (int) battleAccess.Chief));
                    });
            }
                break;
            case 15: //提升权限   state=0
            {
                accessId = GetAccess(MemberId);
                if (accessId == -1)
                {
                    return;
                }
                if (BattleData.MyPorp.Ladder > accessId)
                {
                    if (!JudgeMemberAccess(accessId, accessId + 1))
                    {
                        return;
                    }
                    var tbAccess = Table.GetGuildAccess(accessId + 1);
                    var msgStr = string.Format(GameUtils.GetDictionaryText(220980), tbAccess.Name);
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, msgStr, "", () =>
                    {
                        NetManager.Instance.StartCoroutine(ChangeJurisdiction(BattleData.MyUnion.UnionID, MemberId,
                            accessId + 1));
                    });
                }
                else
                {
                    var ee = new ShowUIHintBoard(220901);
                    EventDispatcher.Instance.DispatchEvent(ee);
                }
            }
                break;
            case 16: //降低权限   state=0
            {
                accessId = GetAccess(MemberId);
                if (accessId == -1)
                {
                    return;
                }
                //玩家已经是最低权限了
                if (accessId == (int) battleAccess.People0)
                {
                    var ee = new ShowUIHintBoard(220902);
                    EventDispatcher.Instance.DispatchEvent(ee);
                    return;
                }
                //需要副帮主以上权限
                if (BattleData.MyPorp.Ladder > accessId && BattleData.MyPorp.Ladder >= (int) battleAccess.AssistantChief)
                {
                    if (!JudgeMemberAccess(accessId, accessId - 1))
                    {
                        return;
                    }
                    var tbAccess = Table.GetGuildAccess(accessId - 1);
                    var msgStr = string.Format(GameUtils.GetDictionaryText(220980), tbAccess.Name);
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, msgStr, "",
                        () =>
                        {
                            NetManager.Instance.StartCoroutine(ChangeJurisdiction(BattleData.MyUnion.UnionID, MemberId,
                                accessId - 1));
                        });
                }
                else
                {
                    var ee = new ShowUIHintBoard(220901);
                    EventDispatcher.Instance.DispatchEvent(ee);
                }
            }
                break;
            case 17: //请出战盟
            {
                accessId = GetAccess(MemberId);
                //需要副帮主以上权限
                if (BattleData.MyPorp.Ladder > accessId && BattleData.MyPorp.Ladder >= (int) battleAccess.AssistantChief)
                {
                    var msgStr = GameUtils.GetDictionaryText(220982);
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, msgStr, "",
                        () =>
                        {
                            NetManager.Instance.StartCoroutine(ChangeJurisdiction(BattleData.MyUnion.UnionID, MemberId,
                                -1));
                        });
                }
                else
                {
                    var ee = new ShowUIHintBoard(220904);
                    EventDispatcher.Instance.DispatchEvent(ee);
                }
            }
                break;
        }
    }

    //取得权限id 
    public int GetAccess(ulong playerId)
    {
        var accessId = -1;
        if (mUnionMembers == null)
        {
            return accessId;
        }
        if (mUnionMembers.ContainsKey(playerId))
        {
            return mUnionMembers[playerId].Ladder;
        }
        return accessId;
    }

    //设置权限
    public void SetAccess()
    {
        var vartbAccess = Table.GetGuildAccess(BattleData.MyPorp.Ladder);
        BattleData.Access.CanAddMember = vartbAccess.CanAddMember;
        BattleData.Access.CanlevelBuff = vartbAccess.CanLevelBuff;
        BattleData.Access.CanOperation = vartbAccess.CanOperation;
        BattleData.Access.CanModifyNotice = vartbAccess.CanModifyNotice;
    }

    // 判断是否成员可被操作 type  =  0 提升权限   1 降低权限 
    public bool JudgeMemberAccess(int orgvalue, int willvalue)
    {
        var varvalue = -1;
        if (orgvalue > willvalue)
        {
            if (willvalue < 0)
            {
                var e = new ShowUIHintBoard(220902);
                EventDispatcher.Instance.DispatchEvent(e);
                return false;
            }
        }
        else if (orgvalue < willvalue)
        {
            if (willvalue > (int) battleAccess.Chief)
            {
                return false;
            }
        }
        varvalue = willvalue;
        var vartbAccess = Table.GetGuildAccess(varvalue);
        var count = 0;
        {
            // foreach(var item in mUnionMembers)
            var __enumerator1 = (mUnionMembers).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var item = __enumerator1.Current;
                {
                    if (item.Value.Ladder == varvalue)
                    {
                        count++;
                    }
                }
            }
        }
        //{0}最多{1}个
        if (vartbAccess.MaxCount - count <= 0)
        {
            var ss = string.Format(GameUtils.GetDictionaryText(220914), vartbAccess.Name, count);
            var e = new ShowUIHintBoard(ss);
            EventDispatcher.Instance.DispatchEvent(e);
            return false;
        }
        return true;
    }

    //type 0申请加入,1取消申请 ,2退出战盟,3同意邀请加入,4拒绝邀请加入
    public IEnumerator ApplyOperation(int type, int value)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AllianceOperation(type, value);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (type == 0)
                    {
                        var index = mOtherUnionDict[value];
                        mCatchOhterUnion[index].IsApplyJoin = 1;
                        OtherUnion.OtherUnionList[OtherUnionSelectIndex].IsApplyJoin = 1;
                        OtherUnion.JoinBtnText = GameUtils.GetDictionaryText(230300);
                        var OtherUnionApplyUnionListCount0 = OtherUnion.ApplyUnionList.Count;
                        for (var i = 0; i < OtherUnionApplyUnionListCount0; i++)
                        {
                            if (OtherUnion.ApplyUnionList[i] == 0)
                            {
                                OtherUnion.ApplyUnionList[i] = value;
                                break;
                            }
                        }
                        var ee = new ShowUIHintBoard(220906);
                        EventDispatcher.Instance.DispatchEvent(ee);

                        PlatformHelper.UMEvent("Union", "Apply");
                    }
                    else if (type == 1)
                    {
                        var index = mOtherUnionDict[value];
                        mCatchOhterUnion[index].IsApplyJoin = 0;
                        OtherUnion.OtherUnionList[OtherUnionSelectIndex].IsApplyJoin = 0;
                        OtherUnion.JoinBtnText = GameUtils.GetDictionaryText(230301);
                        var OtherUnionApplyUnionListCount1 = OtherUnion.ApplyUnionList.Count;
                        for (var i = 0; i < OtherUnionApplyUnionListCount1; i++)
                        {
                            if (OtherUnion.ApplyUnionList[i] == value)
                            {
                                OtherUnion.ApplyUnionList[i] = 0;
                                break;
                            }
                        }
                    }
                    else if (type == 2)
                    {
                        //var ee = new ShowUIHintBoard(220951);
                        //EventDispatcher.Instance.DispatchEvent(ee);
                        BattleData.CreateName = string.Empty;
                        InitCreateInput();
                        BattleData.MyUnion.UnionID = 0;
                        unionLevelChanged = 0;
                    }
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_AllianceApplyJoinOK) //自动申请，成功
                    {
                        BattleData.MyUnion.UnionID = value;
                        PlayerDataManager.Instance.SetExData((int) eExdataDefine.e282, value);
                        MainUIInit();
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //type 0邀请加入,1同意申请 1,2拒绝申请
    public IEnumerator ApplyOperationCharacter(int type, ulong value)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AllianceOperationCharacter(type, value);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (type == 1)
                    {
                        //Logger.Info("申请加入成功");
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //权限设置
    public IEnumerator ChangeJurisdiction(int UnionID, ulong memberId, int access)
    {
        using (new BlockingLayerHelper(0))
        {
            if (memberId <= 0)
            {
                yield break;
            }
            var msg = NetManager.Instance.ChangeJurisdiction(UnionID, memberId, access);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (access == -1)
                    {
                        //踢出玩家
                        if (mUnionMembers.ContainsKey(memberId))
                        {
                            mUnionMembers.Remove(memberId);
                        }
                        var e = new ShowUIHintBoard(220911);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else if (access == (int) battleAccess.Chief)
                    {
                        var e = new ShowUIHintBoard(220912);
                        EventDispatcher.Instance.DispatchEvent(e);
                        BattleData.MyPorp.Ladder = (int) battleAccess.People0;
                        if (mUnionMembers.ContainsKey(BattleData.MyPorp.ID))
                        {
                            mUnionMembers[BattleData.MyPorp.ID].Ladder = 0;
                        }

                        if (mUnionMembers.ContainsKey(memberId))
                        {
                            mUnionMembers[memberId].Ladder = access;
                            BattleData.MyUnion.ChiefName = mUnionMembers[memberId].Name;
                        }
                        SetAccess();
                    }
                    else
                    {
                        var tbAccess = Table.GetGuildAccess(access);
                        var str = string.Format(GameUtils.GetDictionaryText(220913), tbAccess.Name);
                        var e = new ShowUIHintBoard(str);
                        EventDispatcher.Instance.DispatchEvent(e);
                        if (mUnionMembers.ContainsKey(memberId))
                        {
                            mUnionMembers[memberId].Ladder = access;
                        }
                    }
                    ShowInfo(2);
                    //GetMyUnionInfoByServerId(0, Info.ListState);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //ExDataInit时请求玩家战盟信息
    public void OnExDataInit(IEvent ievent)
    {
        //TODO
        var unionId = PlayerDataManager.Instance.GetExData(eExdataDefine.e282);
        BattleData.MyUnion.UnionID = unionId;
        if (unionId > 0)
        {
            GetMyUnionInfoByServerId(0, 0);
            ApplyList();
        }
        ApplyAllianceWarData();
        ApplyAllianceWarOccupantData();
        ApplyAllianceWarChallengerData();
    }

    //更新玩家扩展数据，包括申请的战盟，玩家的战盟变化等
    public void OnUpdateExData(IEvent ievent)
    {
        var e = ievent as BattleUnionExdataUpdate;
        if (e.Type == eExdataDefine.e282)
        {
            BattleData.MyUnion.UnionID = e.Value;
            {
                if (e.Value <= 0)
                {
                    BattleData.ShowWitchUI = 1;
                    //清空battleData数据
                    {
                        var ee = new ShowUIHintBoard(220951);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        //UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(220951), "", () =>
                        //{
                        //});
                    }
                }
                else
                {
                    if (UionState == UnionIDState.OtherJoin)
                    {
                        var ee = new ShowUIHintBoard(220964);
                        EventDispatcher.Instance.DispatchEvent(ee);
                        MainUIInit();

                        PlatformHelper.UMEvent("Union", "Enter");
                        //UIManager.Instance.ShowMessage(MessageBoxType.Ok, GameUtils.GetDictionaryText(220964), "", () =>
                        //{
                        //     MainUIInit();
                        //});
                    }
                    UionState = UnionIDState.OtherJoin;
                }
            }
        }
        if (e.Type == eExdataDefine.e286)
        {
            OtherUnion.ApplyUnionList[0] = e.Value;
        }
        if (e.Type == eExdataDefine.e287)
        {
            OtherUnion.ApplyUnionList[1] = e.Value;
        }
        if (e.Type == eExdataDefine.e288)
        {
            OtherUnion.ApplyUnionList[2] = e.Value;
        }
    }

    // 人物点击，弹出加好友下拉框
    public void CharacterClick(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionCharacterClick;
        ShowOperateUI(e.Data);
    }

    //显示下拉列表
    public void ShowOperateUI(CharacterBaseInfoDataModel data)
    {
        CharacterBaseInfoDataModel varData = null;
        if (!mUnionMembers.ContainsKey(data.ID))
        {
            MemberId = 0;
            return;
        }
        MemberId = data.ID;

        var characterid = PlayerDataManager.Instance.GetGuid();
        var member = mUnionMembers[MemberId];
        var characterName = member.Name;
        if (data.ID == characterid)
        {
            return;
        }

        var ladder = 0;

        switch (BattleData.MyPorp.Ladder)
        {
            case (int) battleAccess.People0:
                ladder = 11;
                break;
            case (int) battleAccess.People1:
                ladder = 11;
                break;
            case (int) battleAccess.AssistantChief:
                ladder = 12;
                break;
            case (int) battleAccess.Chief:
                ladder = 13;
                break;
        }
        PlayerDataManager.Instance.ShowCharacterPopMenu(MemberId, characterName, ladder, member.Level, member.Ladder,
            member.CareerId);
    }

    #endregion

    #region 战盟初始化

    //战盟界面初始化
    public void MainUIInit(int tabId = 0)
    {
        if (tabId < 0)
        {
            tabId = 0;
        }
        BattleData.TabPage = tabId;
        BattleData.TabPage2 = 0;
        AttackCity.TabPage = -1;
        Build.NowCount = PlayerDataManager.Instance.GetExData(eExdataDefine.e285);
        BattleData.MyUnion.UnionID = PlayerDataManager.Instance.GetExData(eExdataDefine.e282);
        Build.TodayDonation = PlayerDataManager.Instance.GetExData(eExdataDefine.e284);
        mRefleshTime.Clear();
        for (var i = 0; i < REFLESH_COUNT; i++)
        {
            mRefleshTime.Add(Game.Instance.ServerTime);
        }
        if (BattleData.MyUnion.UnionID > 0)
        {
            Build.MontyOrItemPage = 0;
            //BattleData.TabPage = (int)TabPage.PageInfo;
            Buff.ShowUpUI = 0;
            BattleData.ShowWitchUI = 0;
            if (tabId == 0)
            {
                CanRefleshData(RefleshType.UnionData);
            }
            if (tabId == 0)
            {
                CanRefleshData(RefleshType.UnionData);
            }
            else if (tabId == (int) TabPage.PageBuild)
            {
                CanRefleshData(RefleshType.DonationData);
            }
            else if (tabId == (int) TabPage.PageCity)
            {
                CanRefleshData(RefleshType.AttackData);
            }
        }
        else
        {
            if (BattleData.CreateName == string.Empty)
            {
                BattleData.CreateName = InputStr;
            }
            BattleData.ShowWitchUI = 1;
        }
        //InitBuff();
        OtherUnion.ApplyUnionList[0] = PlayerDataManager.Instance.GetExData(eExdataDefine.e286);
        OtherUnion.ApplyUnionList[1] = PlayerDataManager.Instance.GetExData(eExdataDefine.e287);
        OtherUnion.ApplyUnionList[2] = PlayerDataManager.Instance.GetExData(eExdataDefine.e288);
    }

    //升级等级后刷新界面信息
    public void RefleshUnionLevel()
    {
        Build.DonationIndex = BattleData.MyUnion.Level; //建设临时索引，主要用来设置建设页面的绑定值，
        SetBattleMoneyString();
        Build.TotalCount = mGuildRecord.moneyCountLimit;
        BattleData.MyUnion.TotalCount = mGuildRecord.MaxCount;
        Info.ReduceString = string.Format(GameUtils.GetDictionaryText(220972), mGuildRecord.MaintainMoney); //维护资金
        _mGuildRecord = Table.GetGuild(BattleData.MyUnion.Level);
    }

    //初始化商店
    public void InitShop()
    {
        var list = new List<BattleUnionShopItemDataModel>();
        for (var i = 1; i <= unionMaxLevel; i++)
        {
            var tbGuild = Table.GetGuild(i);
            if (tbGuild == null)
            {
                return;
            }
            List<StoreRecord> varlist;
            if (mCacheDic.TryGetValue(tbGuild.StoreParam, out varlist))
            {
                for (var j = 0; j < varlist.Count; j++)
                {
                    var data = new BattleUnionShopItemDataModel();
                    data.ShopID = varlist[j].Id;
                    data.ItemID = varlist[j].ItemId;
                    data.Zhangong = varlist[j].NeedValue;
                    data.ItemCount = varlist[j].ItemCount;
                    data.BuyLevel = mCacheDicLP[varlist[j].Type];
                    data.ItemType = varlist[j].NeedType;
                    data.BuyCount = PlayerDataManager.Instance.GetExData(varlist[j].DayCount);
                    if (varlist[j].Type <= mGuildRecord.StoreParam)
                    {
                        data.CanBuy = 1;
                    }
                    else
                    {
                        data.CanBuy = 0;
                    }
                    list.Add(data);
                }
            }
        }
        Shop.ShopList = new ObservableCollection<BattleUnionShopItemDataModel>(list);
    }

    //商店初始化字典
    public void InitShopDictionary()
    {
        mCacheDic.Clear();
        var tbminGuild = Table.GetGuild(1);
        var tbmaxGuild = Table.GetGuild(unionMaxLevel);
        if (tbminGuild == null || tbmaxGuild == null)
        {
            return;
        }
        Table.ForeachStore(record =>
        {
            var key = record.Type;
            if (key >= tbminGuild.StoreParam && key <= tbmaxGuild.StoreParam)
            {
                if (mCacheDic.ContainsKey(key))
                {
                    mCacheDic[key].Add(record);
                }
                else
                {
                    var list = new List<StoreRecord>();
                    list.Add(record);
                    mCacheDic.Add(key, list);
                }
            }
            return true;
        });
    }

    // 0 战盟信息刷新  //1战盟捐献刷新    //2 申请列表
    public void CanRefleshData(RefleshType type)
    {
        switch (type)
        {
            case RefleshType.UnionData:
            {
                if (Game.Instance.ServerTime >= mRefleshTime[0])
                {
                    GetMyUnionInfoByServerId(0, 0);
                }
            }
                break;
            case RefleshType.DonationData:
            {
                if (Game.Instance.ServerTime >= mRefleshTime[1])
                {
                    NetManager.Instance.StartCoroutine(GetDonationInfoCoroutine(BattleData.MyUnion.UnionID));
                }
            }
                break;
            case RefleshType.UnionDataSimple:
            {
                if (Game.Instance.ServerTime >= mRefleshTime[3])
                {
                    GetMyUnionInfoByServerId(1, 0);
                }
            }
                break;
            case RefleshType.ApplyData:
            {
                if (Game.Instance.ServerTime >= mRefleshTime[2])
                {
                    NetManager.Instance.StartCoroutine(ApplyAllianceEnjoyList(BattleData.MyUnion.UnionID));
                }
                else
                {
                    //没有玩家申请加入战盟
                    if (BattleData.MyUnion.ApplyList.Count == 0)
                    {
                        PlayerDataManager.Instance.NoticeData.BattleList = false;
                        var e = new ShowUIHintBoard(220965);
                        EventDispatcher.Instance.DispatchEvent(e);
                        return;
                    }
                    BattleData.CheckBox.SelectAll = false;
                    //Info.JoinShow = 1;
                    ShowInfo(1);
                }
            }
                break;
            case RefleshType.MemberDetailData:
            {
                Info.ShowDetail = 1;
                if (Game.Instance.ServerTime >= mRefleshTime[0])
                {
                    GetMyUnionInfoByServerId(0, 2);
                }
                else
                {
                    var a = 3;
                    //Info.ShowDetail = 1;
                    ShowInfo(2);
                }
            }
                break;
            case RefleshType.OtherUnion:
            {
                if (Game.Instance.ServerTime >= mRefleshTime[4])
                {
                    BtnOtherUnion();
                }
            }
                break;
            case RefleshType.AttackData:
            {
                if (Game.Instance.ServerTime >= mRefleshTime[5])
                {
                    ApplyAllianceWarData();
                }
            }
                break;
        }
    }

    //刷新tabPage
    public void RefleshTabPage(int mPage)
    {
        switch (mPage)
        {
            case 0: //信息                                                 
            {
                CanRefleshData(RefleshType.UnionDataSimple);
            }
                break;
            case 1: //成员
            {
                CanRefleshData(RefleshType.MemberDetailData);
            }
                break;
            case 2: //建设
            {
                CanRefleshData(RefleshType.DonationData);
            }
                break;
            case 3: //技能
            {
                CanRefleshData(RefleshType.UnionDataSimple);
            }
                break;
            case 4: //商店
            {
                CanRefleshData(RefleshType.UnionDataSimple);
            }
                break;
            case 5: //刷
            {
                CanRefleshData(RefleshType.AttackData);
            }
                break;
            case 6: //其他战盟
            {
                CanRefleshData(RefleshType.OtherUnion);
            }
                break;
        }
    }

    #endregion

    #region 战盟创建

    //判断战盟名称合法性
    public static bool CheckName(string input)
    {
        var regex = new Regex(@"^[\u4E00-\u9FFFA-Za-z0-9]{1,14}$");
        if (!regex.IsMatch(input))
        {
            return false;
        }
        var length = Regex.Replace(input, @"[\u4E00-\u9FFF]", "aa").Length;
        if (length > 14 || length <= 0)
        {
            return false;
        }
        return true;
    }

    //创建战盟  
    public void BtnCreateUnion(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionBtnCreateUnion;
        var name = e.Name.Trim();
        BattleData.CreateName = name;
        //战盟不能为空
        if (name == "")
        {
            var ee = new ShowUIHintBoard(220949);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        if (!GameUtils.CheckName(name))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 220950);
            return;
        }
        if (GameUtils.CheckSensitiveName(name))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 220950);
            return;
        }
        if (GameUtils.ContainEmoji(name))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 220950);
            return;
        }

        if (!GameUtils.CheckLanguageName(name))
        {
            UIManager.Instance.ShowMessage(MessageBoxType.Ok, 220950);
            return;
        }

        var tbClient = Table.GetClientConfig(241);
        var needmoney = int.Parse(tbClient.Value);
        if (PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes) < needmoney)
        {
            //金币不足
            var ee = new ShowUIHintBoard(210100);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        tbClient = Table.GetClientConfig(242);
        if (PlayerDataManager.Instance.GetLevel() < int.Parse(tbClient.Value))
        {
            //等级不足
            var ee = new ShowUIHintBoard(210110);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
            string.Format(GameUtils.GetDictionaryText(220917), needmoney/10000, name), "",
            () => { NetManager.Instance.StartCoroutine(CreateUnion(BattleData.CreateName)); });
    }

    public IEnumerator CreateUnion(string name)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.CreateAlliance(name);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    BattleData.ShowWitchUI = 0;
                    BattleData.MyUnion.UnionID = msg.Response;
                    UionState = UnionIDState.CreateJoin;
                    //GetMyUnionInfoById();
                    GetMyUnionInfoByServerId(0, 0);

                    PlatformHelper.UMEvent("Union", "Create");
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //其他战盟
    public void BtnOtherUnion()
    {
        OtherUnionSelectIndex = -1;
        NetManager.Instance.StartCoroutine(GetOtherUnion(PlayerDataManager.Instance.ServerId));
    }

    public IEnumerator GetOtherUnion(int ServerID)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.GetServerAlliance(ServerID);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    OtherUnion.OtherUnionList.Clear();
                    mCatchOhterUnion.Clear();
                    mOtherUnionDict.Clear();
                    var otherList = new List<BattleUnionTeamSimpleDataModel>();
                    for (var i = 0; i < msg.Response.Alliances.Count; i++)
                    {
                        var item = msg.Response.Alliances[i];
                        var unionTeam = new BattleUnionTeamSimpleDataModel();
                        unionTeam.Index = i;
                        unionTeam.UnionID = item.Id;
                        unionTeam.UnionName = item.Name;
                        unionTeam.ChiefName = item.LeaderName;
                        unionTeam.Level = item.Level;
                        unionTeam.NowCount = item.NowCount;
                        unionTeam.TotalCount = item.MaxCount;
                        unionTeam.Force = item.FightPoint;
                        if (item.AutoAgree == 1)
                        {
                            unionTeam.AutoAccept = true;
                        }
                        else
                        {
                            unionTeam.AutoAccept = false;
                        }
                        //是否已申请图标设置
                        for (var j = 0; j < OtherUnion.ApplyUnionList.Count; j++)
                        {
                            if (OtherUnion.ApplyUnionList[j] == 0)
                            {
                                continue;
                            }
                            if (unionTeam.UnionID == OtherUnion.ApplyUnionList[j])
                            {
                                unionTeam.IsApplyJoin = 1;
                            }
                        }

                        var unionTeam2 = new BattleUnionTeamSimpleDataModel(unionTeam);
                        mOtherUnionDict.Add(unionTeam.UnionID, i);
                        mCatchOhterUnion.Add(unionTeam2);
                        //按照选中状态筛选其他战盟显示
                        if (BattleData.CheckBox.ShowAutoJoin)
                        {
                            if (unionTeam.AutoAccept)
                            {
                                otherList.Add(unionTeam); //OtherUnion.OtherUnionList.Add(unionTeam);
                            }
                        }
                        else
                        {
                            otherList.Add(unionTeam); //OtherUnion.OtherUnionList.Add(unionTeam);
                        }
                    }
                    OtherUnion.OtherUnionList = new ObservableCollection<BattleUnionTeamSimpleDataModel>(otherList);
                    if (OtherUnion.OtherUnionList.Count > 0)
                    {
                        SelectOtherUnion(0);
                    }
                    mRefleshTime[4] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                    OtherUnion.JoinBtnText = GameUtils.GetDictionaryText(230301);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    public void ClearCreateInput()
    {
        if (BattleData.CreateName == InputStr)
        {
            BattleData.CreateName = string.Empty;
        }
    }

    public void InitCreateInput()
    {
        if (String.IsNullOrEmpty(BattleData.CreateName))
        {
            BattleData.CreateName = InputStr;
        }
    }

    #endregion

    #region 其他战盟

    //加入其他战盟
    public void BtnApplyJoin()
    {
        //加入战盟等级不足
        var tbClient = Table.GetClientConfig(243);
        var varlevel = int.Parse(tbClient.Value);
        if (varlevel > PlayerDataManager.Instance.GetLevel())
        {
            var str = string.Format(GameUtils.GetDictionaryText(220944), varlevel);
            var e = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        //你已经加入战盟，不可加入新的战盟
        if (BattleData.MyUnion.UnionID > 0)
        {
            var e = new ShowUIHintBoard(220918);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        //选择你想要加入的战盟
        if (OtherUnionSelectIndex == -1)
        {
            var e = new ShowUIHintBoard(220919);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var selectedItem = OtherUnion.OtherUnionList[OtherUnionSelectIndex];
        //战盟已经满
        if (selectedItem.NowCount == selectedItem.TotalCount)
        {
            var e = new ShowUIHintBoard(220905);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var count = 0;
        if (selectedItem.IsApplyJoin == 0)
        {
            var OtherUnionApplyUnionListCount4 = OtherUnion.ApplyUnionList.Count;
            for (var i = 0; i < OtherUnionApplyUnionListCount4; i++)
            {
                if (OtherUnion.ApplyUnionList[i] > 0)
                {
                    count++;
                }
            }
            //您已达到战盟最大申请数量
            if (count >= OtherUnion.ApplyUnionList.Count)
            {
                var e = new ShowUIHintBoard(220920);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
            NetManager.Instance.StartCoroutine(ApplyOperation(0, selectedItem.UnionID));
        }
        //取消加入此战盟的申请
        else if (selectedItem.IsApplyJoin == 1)
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(220921), "",
                () => { NetManager.Instance.StartCoroutine(ApplyOperation(1, selectedItem.UnionID)); });
        }
    }

    //其他战盟页面返回
    public void BtnOtherReturn()
    {
        if (BattleData.MyUnion.UnionID > 0)
        {
            BattleData.ShowWitchUI = 0;
        }
        else
        {
            BattleData.ShowWitchUI = 1;
            //EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.BattleUnionUI));
        }
    }

    //其他战盟list点击事件
    public void UnionOtherListClick(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionOtherListClick;
        SelectOtherUnion(e.Index);
    }

    //其他战盟选择
    private void SelectOtherUnion(int index)
    {
        if (OtherUnionSelectIndex != -1)
        {
            OtherUnion.OtherUnionList[OtherUnionSelectIndex].Selected = 0;
        }
        OtherUnionSelectIndex = index;
        if (OtherUnion.OtherUnionList[index].IsApplyJoin == 1)
        {
            //取消申请
            OtherUnion.JoinBtnText = GameUtils.GetDictionaryText(230300);
        }
        else
        {
            //申请加入
            OtherUnion.JoinBtnText = GameUtils.GetDictionaryText(230301);
        }
        OtherUnion.OtherUnionList[index].Selected = 1;
    }

    #endregion

    #region 战盟信息

    //战盟申请列表
    public void BtnApplyList()
    {
        //Info.JoinShow = Info.JoinShow == 0 ? 1 : 0;

        CanRefleshData(RefleshType.ApplyData);
    }

    //请求战盟申请列表
    private void ApplyList()
    {
        NetManager.Instance.StartCoroutine(ApplyListCoroutine(BattleData.MyUnion.UnionID));
    }

    //战盟申请列表
    public IEnumerator ApplyListCoroutine(int BattleUnionId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceEnjoyList(BattleUnionId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    if (msg.Response.Applys.Count > 0)
                    {
                        if (BattleData.Access.CanAddMember == 1)
                        {
                            PlayerDataManager.Instance.NoticeData.BattleList = true;
                        }
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    //战盟申请列表
    public IEnumerator ApplyAllianceEnjoyList(int BattleUnionId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceEnjoyList(BattleUnionId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var count = 0;
                    BattleData.MyUnion.ApplyList.Clear();
                    var list = new List<CharacterBaseInfoDataModel>();
                    {
                        // foreach(var item in msg.Response.Applys)
                        var __enumerator2 = (msg.Response.Applys).GetEnumerator();
                        while (__enumerator2.MoveNext())
                        {
                            var item = __enumerator2.Current;
                            {
                                var baseData = new CharacterBaseInfoDataModel();
                                var white = GameUtils.GetTableColor(0);
                                baseData.ColorOnLine = white;
                                baseData.Selected = 0;
                                baseData.DonationCount = item.MeritPoint;
                                baseData.Force = item.FightPoint;
                                baseData.Online = item.Online;
                                if (baseData.Online == 0)
                                {
                                    //baseData.Scene = "";
                                    var mLostTime = Game.Instance.ServerTime;
                                    if (item.LostTime != 0)
                                    {
                                        mLostTime = Extension.FromServerBinary(item.LostTime);
                                    }
                                    baseData.LastTime = GameUtils.GetLastTimeDiffString(mLostTime);
                                }
                                else
                                {
                                    baseData.LastTime = strOnline;
                                }
                                var tbSene = Table.GetScene(item.SceneId);
                                if (tbSene == null)
                                {
                                    baseData.Scene = "";
                                }
                                else
                                {
                                    baseData.Scene = tbSene.Name;
                                }
                                baseData.Index = count;
                                baseData.ID = item.Guid;
                                baseData.Name = item.Name;
                                baseData.Ladder = item.Ladder;
                                baseData.Level = item.Level;
                                baseData.CareerId = item.TypeId;
                                var tbCharacterBase = Table.GetCharacterBase(item.TypeId);
                                if (tbCharacterBase != null)
                                {
                                    baseData.Career = tbCharacterBase.Name;
                                }
                                list.Add(baseData);
                                count++;
                            }
                        }
                        BattleData.MyUnion.ApplyList = new ObservableCollection<CharacterBaseInfoDataModel>(list);
                    }
                    //没有玩家申请加入战盟
                    if (BattleData.MyUnion.ApplyList.Count != 0)
                    {
                        PlayerDataManager.Instance.NoticeData.BattleList = true;
                    }
                    else
                    {
                        //Info.JoinShow = 0;
                        //ShowInfo(0);
                        var e = new ShowUIHintBoard(220965);
                        EventDispatcher.Instance.DispatchEvent(e);
                        PlayerDataManager.Instance.NoticeData.BattleList = false;
                        yield break;
                    }
                    BattleData.CheckBox.SelectAll = false;
                    //Info.JoinShow = 1;
                    ShowInfo(1);
                    mRefleshTime[2] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //详细信息返回
    public void BtnnDetailBack()
    {
        Info.ShowDetail = 1;
    }

    //添加成员按钮事件
    public void BtnAddMember()
    {
        Info.ShowFindUI = 1;
    }

    //添加战盟成员按钮
    public void FindMemberOK()
    {
        Info.FindName = Info.FindName.Trim();
        NetManager.Instance.StartCoroutine(FindMember(Info.FindName));
    }

    public IEnumerator FindMember(string name)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AllianceOperationCharacterByName(0, name);
            //"已发送邀请玩家加入战盟的请求"
            Info.ShowFindUI = 0;
            var ee = new ShowUIHintBoard(220974);
            EventDispatcher.Instance.DispatchEvent(ee);
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

    //关闭添加成员页面
    public void FindUIClose()
    {
        Info.ShowFindUI = 0;
        Info.FindName = "";
    }

    //通过申请
    public void BtnPassApply(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionBtnPassApply;
        var count = 0;
        var totalCount = 0;
        var idList = new Uint64Array();
        var indexList = new List<int>();
        //成员已满

        if (e.Type == 0 && BattleData.MyUnion.TotalCount <= BattleData.MyUnion.NowCount)
        {
            var ee = new ShowUIHintBoard(220940);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        {
            // foreach(var item in Info.ShowList)
            var __enumerator3 = (BattleData.MyUnion.ApplyList).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var item = __enumerator3.Current;
                {
                    if (item.Selected == 1)
                    {
                        //if (totalCount < BattleData.MyUnion.TotalCount - BattleData.MyUnion.NowCount)
                        //{
                        idList.Items.Add(BattleData.MyUnion.ApplyList[count].ID);
                        indexList.Add(count);
                        totalCount++;
                        //}
                        //else
                        //{
                        //    break;
                    }
                    count++;
                    //}
                }
            }
        }
        if (idList.Items.Count > 0)
        {
            NetManager.Instance.StartCoroutine(AllianceAgreeApplyList(BattleData.MyUnion.UnionID, e.Type, idList,
                indexList));
        }
    }

    // type = 0 批量申请    //type =1 批量拒绝
    public IEnumerator AllianceAgreeApplyList(int BattleUnionId, int type, Uint64Array IDlist, List<int> indexList)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.AllianceAgreeApplyList(BattleUnionId, type, IDlist);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    for (var i = indexList.Count - 1; i >= 0; i--)
                    {
                        BattleData.MyUnion.ApplyList.RemoveAt(indexList[i]);
                    }
                    ShowInfo(1);
                    if (type == 0)
                    {
                        //刷新成员信息
                        GetMyUnionInfoByServerId(0, 1);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    public void BtnDefuseApply()
    {
        var count = 0;
        var idList = new Uint64Array();
        var indexList = new List<int>();
        {
            // foreach(var item in Info.ShowList)
            var __enumerator4 = (BattleData.MyUnion.ApplyList).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var item = __enumerator4.Current;
                {
                    if (item.Selected == 1)
                    {
                        //    if (count < BattleData.MyUnion.TotalCount - BattleData.MyUnion.NowCount)
                        //    {
                        idList.Items.Add(BattleData.MyUnion.ApplyList[count].ID);
                        indexList.Add(count);
                        //    }
                        //    else
                        //    {
                        //        break;
                        //    }
                        //}
                        count++;
                    }
                }
            }
        }
        if (idList.Items.Count > 0)
        {
            NetManager.Instance.StartCoroutine(AllianceAgreeApplyList(1, BattleData.MyUnion.UnionID, idList, indexList));
        }
    }

    // ShowInfo(0); //信息界面返回
    public void BtnInfoReturn()
    {
        ShowInfo(1);
    }

    //// ShowInfo(3);//成员信息
    //public void BtnMemberInfo()
    //{
    //    CanRefleshData(RefleshType.MemberDetailData);

    //    //UIEvent_UnionBtnMemberInfo e = ievent as UIEvent_UnionBtnMemberInfo;
    //    //Info.ShowList.Clear();
    //    //if (e.Index == 0)
    //    //{
    //    //    ShowInfo(0);
    //    //}
    //    //else if(e.Index==1)
    //    //{
    //    //    ShowInfo(1);
    //    //}
    //}
    public void MemberListSort(List<CharacterBaseInfoDataModel> list)
    {
        // var varList = list;
        list.Sort((a, b) =>
        {
            if (a.Ladder < b.Ladder)
            {
                return 1;
            }
            if (a.Ladder == b.Ladder)
            {
                if (a.Level < b.Level)
                {
                    return 1;
                }
                if (a.Level == b.Level)
                {
                    return a.Index - b.Index;
                }
                return -1;
            }
            return -1;
        });

        // return array;
    }

    //0 基本信息，1 其他信息，2 成员详细信息
    public void ShowInfo(int type)
    {
        var white = GameUtils.GetTableColor(0);
        var Gray = GameUtils.GetTableColor(96);

        #region

        //if (type == 0) //0 基本信息
        //{
        //    Info.ListState = 0;
        //    Info.JoinShow = 0;
        //    Info.ShowList.Clear();
        //    if (mUnionMembers != null)
        //    {
        //        if (BattleData.CheckBox.ShowOffLine)
        //        {
        //            ObservableCollection<CharacterBaseInfoDataModel> varList = new ObservableCollection<CharacterBaseInfoDataModel>();
        //            {
        //                // foreach(var item in mUnionMembers)
        //                var __enumerator5 = (mUnionMembers).GetEnumerator();
        //                while (__enumerator5.MoveNext())
        //                {
        //                    var item = __enumerator5.Current;
        //                    {
        //                        CharacterBaseInfoDataModel varitem = new CharacterBaseInfoDataModel(item.Value);
        //                        string str = "";
        //                        if (item.Value.Online == 1)
        //                        {
        //                            varitem.ColorOnLine = white;
        //                        }
        //                        else
        //                        {
        //                            varitem.ColorOnLine = Gray;
        //                        }
        //                        varitem.State = 0;
        //                        varList.Add(varitem);
        //                    }
        //                }
        //            }
        //            Info.ShowList = varList;
        //        }
        //        else
        //        {
        //            ObservableCollection<CharacterBaseInfoDataModel> varList = new ObservableCollection<CharacterBaseInfoDataModel>();
        //            {
        //                // foreach(var item in mUnionMembers)
        //                var __enumerator6 = (mUnionMembers).GetEnumerator();
        //                while (__enumerator6.MoveNext())
        //                {
        //                    var item = __enumerator6.Current;
        //                    {
        //                        if (item.Value.Online == 0)
        //                        {
        //                            continue;
        //                        }
        //                        CharacterBaseInfoDataModel varitem = new CharacterBaseInfoDataModel(item.Value);
        //                        varitem.ColorOnLine = white;
        //                        varitem.State = 0;
        //                        varList.Add(varitem);
        //                    }
        //                }
        //            }
        //            Info.ShowList = varList;
        //        }
        //    }
        //}
        //else

        #endregion

        if (type == 1) //1 申请信息
        {
            //Info.ListState = 2;
            //Info.JoinShow = 1;
            Info.ShowDetail = 0;
            //Info.ShowList.Clear();
            //if (BattleData.MyUnion.ApplyList != null)
            //{
            //    ObservableCollection<CharacterBaseInfoDataModel> varList = new ObservableCollection<CharacterBaseInfoDataModel>();
            //    {
            //        // foreach(var item in BattleData.MyUnion.ApplyList)
            //        var __enumerator7 = (BattleData.MyUnion.ApplyList).GetEnumerator();
            //        while (__enumerator7.MoveNext())
            //        {
            //            var item = __enumerator7.Current;
            //            {
            //                CharacterBaseInfoDataModel varitem = new CharacterBaseInfoDataModel(item);
            //                varitem.Selected = 0;
            //                varitem.ColorOnLine = white;
            //                varitem.State = 1;
            //                varList.Add(varitem);
            //            }
            //        }
            //    }
            //    Info.ShowList = varList;
            //}
        }
        else if (type == 2) //成员详细列表
        {
            Info.MembersDetail.Clear();
            Info.ShowDetail = 1;
            var DetailList = new ObservableCollection<CharacterBaseInfoDataModel>();
            {
                // foreach(var item in mUnionMembers)
                var __enumerator8 = (mUnionMembers).GetEnumerator();
                while (__enumerator8.MoveNext())
                {
                    var item = __enumerator8.Current;
                    {
                        item.Value.ListShow = 1;
                        if (!BattleData.CheckBox.ShowOffLineDetail)
                        {
                            if (item.Value.Online == 1)
                            {
                                var data = new CharacterBaseInfoDataModel(item.Value);
                                data.ColorOnLine = white;
                                item.Value.ListShow = 1;
                                DetailList.Add(data);
                            }
                        }
                        else
                        {
                            var data = new CharacterBaseInfoDataModel(item.Value);
                            if (item.Value.Online == 1)
                            {
                                data.ColorOnLine = white;
                            }
                            else
                            {
                                data.ColorOnLine = Gray;
                            }
                            item.Value.ListShow = 1;
                            DetailList.Add(data);
                        }
                    }
                }
            }
            Info.MembersDetail = DetailList;
        }
    }

    //修改公告
    public void BtnModifyNotice()
    {
        if (Info.VarNotice == string.Empty)
        {
            return;
        }
        //公告内容没有改变
        if (Info.VarNotice == BattleData.MyUnion.Notice)
        {
            Info.IsNotice = 0;
            var e = new ShowUIHintBoard(220922);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        //字数判断
        if (Info.VarNotice.Length > 120)
        {
            var e = new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220962), 120));
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        NetManager.Instance.StartCoroutine(ChangeUnionNotice(BattleData.MyUnion.UnionID, Info.VarNotice));
    }

    public IEnumerator ChangeUnionNotice(int UnionID, string content)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ChangeAllianceNotice(UnionID, content);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Info.IsNotice = 0;
                    var e = new ShowUIHintBoard(220922);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //公告修改时，显示保存按钮
    public void NoticeSaveShow()
    {
        Info.IsNotice = 1;
    }

    //自动加入战盟checkbox
    public IEnumerator ChangeAllianceAutoJoin(int BattleUnionId)
    {
        using (new BlockingLayerHelper(0))
        {
            var value = BattleData.MyUnion.AutoAccept == 0 ? 1 : 0;
            var msg = NetManager.Instance.ChangeAllianceAutoJoin(BattleUnionId, value);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    BattleData.MyUnion.AutoAccept = value;
                    var count = 0;
                    var idList = new Uint64Array();
                    var indexList = new List<int>();

                    for (var i = 0; i < BattleData.MyUnion.ApplyList.Count; i++)
                    {
                        var item = BattleData.MyUnion.ApplyList[i];
                        idList.Items.Add(item.ID);
                        indexList.Add(i);
                    }
                    if (idList.Items.Count > 0)
                    {
                        NetManager.Instance.StartCoroutine(AllianceAgreeApplyList(BattleData.MyUnion.UnionID, 0, idList,
                            indexList));
                    }
                    //自动加入战盟设置成功
                    var e = new ShowUIHintBoard(220923);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    #endregion

    #region  战盟建设

    //捐献金币
    public void BtnDonation(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionBtnDonation;
        if (Build.NowCount >= mGuildRecord.moneyCountLimit)
        {
            var ee = new ShowUIHintBoard(220935);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        if (e.Index == 0)
        {
            //金币不足
            if (PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes) < mGuildRecord.LessNeedCount)
            {
                var ee = new ShowUIHintBoard(210100);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
        }
        else if (e.Index == 1)
        {
            //金币不足
            if (PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes) < mGuildRecord.MoreNeedCount)
            {
                var ee = new ShowUIHintBoard(210100);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
        }
        else if (e.Index == 2)
        {
            //钻石不足
            if (PlayerDataManager.Instance.GetRes((int) eResourcesType.DiamondRes) < mGuildRecord.DiaNeedCount)
            {
                var ee = new ShowUIHintBoard(210102);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
        }
        NetManager.Instance.StartCoroutine(DonationAllianceItem(e.Index));
    }

    public IEnumerator DonationAllianceItem(int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DonationAllianceItem(type);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Build.NowCount++;
                    if (type == 0)
                    {
                        BattleData.MyUnion.Money += mGuildRecord.LessUnionMoney;
                        BattleData.MyPorp.DonationCount += mGuildRecord.LessGetGongji;
                    }
                    else if (type == 1)
                    {
                        BattleData.MyUnion.Money += mGuildRecord.MoreUnionMoney;
                        BattleData.MyPorp.DonationCount += mGuildRecord.MoreGetGongji;
                    }
                    else if (type == 2)
                    {
                        BattleData.MyUnion.Money += mGuildRecord.DiaUnionMoney;
                        BattleData.MyPorp.DonationCount += mGuildRecord.DiamondGetGongji;
                    }
                    SetBattleMoneyString();
                    var e = new ShowUIHintBoard(220924);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //捐献物品点击
    public void DonationItemClick(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionDonationItemClick;
        var selectedItemId = Build.DonationItem[e.Index].ItemIDData.ItemId;
        if (selectedItemId == -1)
        {
            return;
        }
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ItemInfoUI,
            new ItemInfoArguments
            {
                ItemId = selectedItemId,
                ShowType = (int) eEquipBtnShow.Share
            }));
    }

    //捐献物品
    public void BtnDonationItem(IEvent ievent)
    {
        var ee = ievent as UIEvent_UnionDonationItem;
        var select = ee.Index;

        if (Build.DonationItem[select].ItemIDData.ItemId == -1)
        {
            return;
        }
        //等待刷新
        if (Build.DonationItem[select].State == (int) ItemState.Wait)
        {
            var e = new ShowUIHintBoard(220938);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        //道具不足
        if (PlayerDataManager.Instance.GetItemCount(Build.DonationItem[select].ItemIDData.ItemId) <= 0)
        {
            var e = new ShowUIHintBoard(200000005);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var tbMission = Table.GetGuildMission(Build.DonationItem[select].TaskID);
        if (Build.TodayDonation + tbMission.GetGongJi > Build.MaxDonation)
        {
            //每日捐赠道具获得的功绩不能超过{0}
            var e = new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220915), Build.MaxDonation));
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        NetManager.Instance.StartCoroutine(BtnDonationItemCoroutine(Build.DonationItem[select].TaskID, select));
    }

    //捐赠物品
    public IEnumerator BtnDonationItemCoroutine(int taskID, int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.DonationAllianceItem(taskID);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Build.DonationItem[index].LeftCount = Build.DonationItem[index].TotalCount - msg.Response;
                    var tbGuildMission = Table.GetGuildMission(Build.DonationItem[index].TaskID);
                    if (tbGuildMission == null)
                    {
                        yield break;
                    }
                    BattleData.MyUnion.Money += tbGuildMission.GetMoney;
                    SetBattleMoneyString();
                    BattleData.MyPorp.DonationCount += tbGuildMission.GetGongJi;
                    Build.TodayDonation += tbGuildMission.GetGongJi;
                    if (Build.DonationItem[index].LeftCount == 0)
                    {
                        NetManager.Instance.StartCoroutine(GetDonationInfoCoroutine(BattleData.MyUnion.UnionID));
                        //Build.DonationItem[index].State = (int) ItemState.Wait;
                        //Build.DonationItem[index].NextTime = Game.Instance.ServerTime.ToBinary();
                        //SteDonationIitemNull(index);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    NetManager.Instance.StartCoroutine(GetDonationInfoCoroutine(BattleData.MyUnion.UnionID));
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //捐献时间刷新
    public void SetDonationIitemNull(int index)
    {
        var item = Build.DonationItem[index];
        item.ItemIDData.ItemId = -1;

        var vartime = Extension.FromServerBinary(item.NextTime);
        if (vartime >= Game.Instance.ServerTime)
        {
            if (item.TimerCoroutine != null)
            {
                NetManager.Instance.StopCoroutine(item.TimerCoroutine);
            }
            item.TimerCoroutine = NetManager.Instance.StartCoroutine(TimerCoroutine(vartime, index));
        }
    }

    //捐赠物品刷新
    public IEnumerator TimerCoroutine(DateTime time, int index)
    {
        if (time < Game.Instance.ServerTime)
        {
            yield break;
        }
        var str = GameUtils.GetDictionaryText(220977);
        while (time > Game.Instance.ServerTime)
        {
            yield return new WaitForSeconds(1.0f);

            Build.DonationItem[index].RefleshTime = GameUtils.GetTimeDiffString(time) + "\r\n" + str;
        }
        yield return new WaitForSeconds(2f); //延迟2秒
        NetManager.Instance.StartCoroutine(GetDonationInfoCoroutine(BattleData.MyUnion.UnionID));
    }

    //请求战盟捐赠物品列表
    public IEnumerator GetDonationInfoCoroutine(int UnionID)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceMissionData(UnionID);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    SetDonationItem(msg.Response.Missions);
                    mRefleshTime[1] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //显示日志
    public void BtnSeeLog()
    {
        Build.ShowDonation = 1;
        NetManager.Instance.StartCoroutine(ApplyAllianceDonationList(BattleData.MyUnion.UnionID));
    }

    public IEnumerator ApplyAllianceDonationList(int BattleUnionId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceDonationList(BattleUnionId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Build.LogList.Clear();
                    var getstr = GameUtils.GetDictionaryText(220946);
                    var mList = new List<BattleUnionDonationLogDataModel>();
                    {
                        // foreach(var data in msg.Response.Datas)
                        var __enumerator9 = (msg.Response.Datas).GetEnumerator();
                        while (__enumerator9.MoveNext())
                        {
                            var data = __enumerator9.Current;
                            {
                                var i = new BattleUnionDonationLogDataModel();
                                var str = Extension.FromServerBinary(data.Time).ToString("HH:mm:ss");
                                var tbBaseItem = Table.GetItemBase(data.ItemId);
                                //if (tbBaseItem != null)
                                //{
                                //  i.Label = string.Format(GameUtils.GetDictionaryText(220863), str , data.Name, data.Count, tbBaseItem.Name);
                                //    Build.LogList.Add( i);
                                //}
                                //test
                                if (tbBaseItem != null)
                                {
                                    if (data.ItemId == 2 || data.ItemId == 3)
                                    {
                                        i.Label = str + ": " + "[4FC012]" + data.Name + "[-]" +
                                                  getstr + "[4FC012]" + data.Count + "[-]" + tbBaseItem.Name;
                                    }
                                    else
                                    {
                                        i.Label = str + ": " + "[4FC012]" + data.Name + "[-]" +
                                                  getstr + "[4FC012]" + tbBaseItem.Name + "[-]";
                                    }
                                    mList.Insert(0, i);
                                }
                                Build.LogList = new ObservableCollection<BattleUnionDonationLogDataModel>(mList);
                            }
                        }
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //关闭日志
    public void BtnCloseSeeLog()
    {
        Build.ShowDonation = 0;
    }

    //设置自动申请加入
    public void BtnSetAutoAccept(IEvent ievent)
    {
        NetManager.Instance.StartCoroutine(ChangeAllianceAutoJoin(BattleData.MyUnion.UnionID));
    }

    //显示帮助
    public void BuildShowHelp()
    {
        Build.ShowHelp = Build.ShowHelp == 1 ? 0 : 1;
    }

    #endregion

    #region 战盟升级

    //buff页面显示
    public void BtnUnionBuffUpShow()
    {
        var itemSelect = Buff.BuffList[BuffSelected];
        GuildBuffRecord tbGuildBuff = null;
        if (itemSelect.BuffID == 0 || itemSelect.BuffID == -1)
        {
            tbGuildBuff = Table.GetGuildBuff(BuffIdInit[BuffSelected]);
        }
        else
        {
            var tb = Table.GetGuildBuff(itemSelect.BuffID);
            //已经是最高级
            if (tb.NextLevel == -1)
            {
                var ee = new ShowUIHintBoard(220934);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
            tbGuildBuff = Table.GetGuildBuff(tb.NextLevel);
        }
        if (tbGuildBuff == null)
        {
            return;
        }
        //等级不足
        if (PlayerDataManager.Instance.GetLevel() < tbGuildBuff.LevelLimit)
        {
            var ee = new ShowUIHintBoard(210110);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        if (BattleData.MyUnion.Level < tbGuildBuff.NeedUnionLevel)
        {
            //升级buff需要战盟等级为{0}级
            var e = new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(220963), tbGuildBuff.NeedUnionLevel));
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        //功绩不足
        if (tbGuildBuff.UpConsumeGongji > PlayerDataManager.Instance.GetRes((int) eResourcesType.Contribution))
        {
            var e = new ShowUIHintBoard(210111);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        Buff.UpConsume = tbGuildBuff.UpConsumeGongji.ToString();
        Buff.ShowUpUI = 1;
    }

    //战盟是否可升级
    public void UnionCanLevel()
    {
        if (BattleData.MyUnion.Money >= mGuildRecord.ConsumeUnionMoney)
        {
            Info.CanLevel = 1;
        }
        else
        {
            Info.CanLevel = 0;
        }
        if (mGuildRecord.ConsumeUnionMoney <= 0)
        {
            Info.CanLevel = -1;
        }
        Buff.NeedDonation = mGuildRecord.ConsumeUnionMoney;
    }


    private void InitBuffName()
    {
        var count = Buff.BuffList.Count;
        for (var i = 0; i < count; i++)
        {
            var tbBuff = Table.GetBuff(500 + i);
            if (tbBuff == null)
            {
                return;
            }
            Buff.BuffList[i].Name = tbBuff.Name;
        }
    }

    public void OnBuffIcon(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionBuffUpShow;
        RefleshBuffUI(e.Index);
    }

    //刷新buff页面
    public void RefleshBuffUI(int index)
    {
        Buff.BuffList[BuffSelected].Selected = false;
        var selectItem = Buff.BuffList[index];
        selectItem.Selected = true;
        BuffSelected = index;
        Buff.ShowMinMaxStr = 0; //当前级别和下一级都显示。
        GuildBuffRecord tbGuildBuff = null;
        if (selectItem.BuffID <= 0)
        {
            tbGuildBuff = Table.GetGuildBuff(BuffIdInit[index]);
            Buff.BuffBtnStr = GameUtils.GetDictionaryText(270257);
            Buff.BuffEffect2 = tbGuildBuff.Desc;
            Buff.BuffNextLevel = tbGuildBuff.BuffLevel;
            Buff.BuffNextID = BuffIdInit[index];
            Buff.ShowMinMaxStr = 1; //显示minstr
        }
        else
        {
            var tb = Table.GetGuildBuff(selectItem.BuffID);
            Buff.BuffEffect1 = tb.Desc;
            Buff.BuffLevel = tb.BuffLevel;
            if (tb.NextLevel == -1)
            {
                Buff.ShowMinMaxStr = 2; //显示Maxstr
                return;
            }
            tbGuildBuff = Table.GetGuildBuff(tb.NextLevel);
            Buff.BuffEffect2 = tbGuildBuff.Desc;
            Buff.BuffNextLevel = tbGuildBuff.BuffLevel;
            Buff.BuffNextID = tbGuildBuff.Id;
            Buff.BuffBtnStr = GameUtils.GetDictionaryText(270258);
        }


        var tbBuff = Table.GetBuff(tbGuildBuff.BuffID);
        if (tbBuff == null)
        {
            return;
        }
        selectItem.Name = tbBuff.Name;
        Buff.BuffName = selectItem.Name;
        Buff.NeedGongji = tbGuildBuff.UpConsumeGongji;
        if (PlayerDataManager.Instance.GetLevel() >= tbGuildBuff.LevelLimit)
        {
            Buff.NeedCharacterLevel = "[4BE127]" + "Lv." + tbGuildBuff.LevelLimit + "[-]";
        }
        else
        {
            Buff.NeedCharacterLevel = "[FF0000]" + "Lv." + tbGuildBuff.LevelLimit + "[-]";
        }
        if (BattleData.MyUnion.Level >= tbGuildBuff.NeedUnionLevel)
        {
            Buff.NeedBattleLevel = "[4BE127]" + "Lv." + tbGuildBuff.NeedUnionLevel + "[-]";
        }
        else
        {
            Buff.NeedBattleLevel = "[FF0000]" + "Lv." + tbGuildBuff.NeedUnionLevel + "[-]";
        }
    }


    //buff升级按钮
    public void BtnBuffUpOK()
    {
        NetManager.Instance.StartCoroutine(UpgradeAllianceBuff(BattleData.MyUnion.UnionID, Buff.BuffNextID));
    }

    public IEnumerator UpgradeAllianceBuff(int UnionID, int BuffID)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.UpgradeAllianceBuff(BuffID);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    Buff.BuffList[BuffSelected].BuffID = BuffID;
                    RefleshBuffUI(BuffSelected);
                    Buff.ShowUpUI = 0;
                    var e = new ShowUIHintBoard(220975);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //战盟升级
    public void BtnUnionLevelup()
    {
        //战盟贡献度要大于消耗
        if (BattleData.MyUnion.Money < mGuildRecord.ConsumeUnionMoney)
        {
            var e = new ShowUIHintBoard(220930);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        //升级战盟等级需要消耗战盟贡献度{0}，确认升级么？
        var str = string.Format(GameUtils.GetDictionaryText(220976), mGuildRecord.ConsumeUnionMoney);
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
            () => { NetManager.Instance.StartCoroutine(BtnUnionLevelupCoroutine(BattleData.MyUnion.UnionID)); }
            );
    }

    public IEnumerator BtnUnionLevelupCoroutine(int unionID)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.UpgradeAllianceLevel(unionID);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_UnionAnim());

                    var e = new ShowUIHintBoard(220932);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    //buff升级页面关闭
    public void BtnUpBuffCancel()
    {
        Buff.ShowUpUI = 0;
    }

    #endregion

    #region 商店

    //购买商品
    public void BuyItemClick(IEvent ievent)
    {
        var e = ievent as UIEvent_BattleShopCellClick;
        var tbShop = Table.GetStore(Shop.ShopList[e.Index].ShopID);
        if (tbShop == null)
        {
            return;
        }
//         var roleType = PlayerDataManager.Instance.GetRoleId();
//         if (BitFlag.GetLow(tbShop.SeeCharacterID, roleType) == false)
//         {
//             return;
//         }
        // 可购买次数小于单次股买数量
        if (Shop.ShopList[e.Index].BuyCount - tbShop.ItemCount < 0)
        {
            var ee = new ShowUIHintBoard(220939);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }
        //自身战功小于消耗
        if (PlayerDataManager.Instance.GetRes((int) eResourcesType.Contribution) < Shop.ShopList[e.Index].Zhangong)
        {
            var ee = new ShowUIHintBoard(210111);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        NetManager.Instance.StartCoroutine(ShopBuyCoroutine(Shop.ShopList[e.Index].ShopID,
            Shop.ShopList[e.Index].ItemCount, e.Index));
    }

    public IEnumerator ShopBuyCoroutine(int ShopId, int count, int index)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.StoreBuy(ShopId, count, -1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //购买成功
                    var tbShop = Table.GetStore(Shop.ShopList[index].ShopID);
                    if (tbShop == null)
                    {
                        yield break;
                    }
                    Shop.ShopList[index].BuyCount -= tbShop.ItemCount;
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
            else
            {
                var e = new ShowUIHintBoard(220821);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }
    }

    #endregion

    #region boss

    //boss获得奖励
    public void BtnBossGetReward()
    {
    }

    public void BossClick(IEvent ievent)
    {
    }

    #endregion

    #region 攻城战

    //每次竞价钱数
    private int _addPerCount = 10000;
    private int _limitMin;
    private readonly int _titleId = 5000;
    private readonly int _fuBenId = 9000;
    public Coroutine _buttonPress;
    private object _attackCityStateTrigger;
    private object _attackCityReadyTrigger;

    //攻城战几个按钮点击打开相应page页
    private void AttackPageClick(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionBattlePageCLick;
        //竞价按钮请求竞价金额
        if (e.Index == 2)
        {
            if (BattleData.MyUnion.Level < 3)
            {
                //"战盟等级至少需要3级才可报名！"
                var ee = new ShowUIHintBoard(270275);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
            if (AttackCity.OpenState != (int) eAllianceWarState.Bid)
            {
                //"现在不是竞价时间段！"
                var ee = new ShowUIHintBoard(270276);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
            if (AttackCity.CastellanId == BattleData.MyUnion.UnionID)
            {
                //"守城方不能报名！"
                var ee = new ShowUIHintBoard(270277);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }

            if (Table.GetGuildAccess(BattleData.MyPorp.Ladder).CanModifyAttackCity != 1)
            {
                //"你没有权限报名！"
                var ee = new ShowUIHintBoard(270278);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
            NetManager.Instance.StartCoroutine(ApplyBid(0, 0));
        }
        AttackCity.TabPage = e.Index;
    }

    //加入城战
    private void AttackJoin()
    {
        var mErrorCode = CheckAttackJoin();
        switch (mErrorCode)
        {
            case 1:
            {
                //"你没有参赛资格！"
                var ee = new ShowUIHintBoard(270279);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                return;
            //无人竞标，本次城战取消s
            case 2:
            {
                var ee = new ShowUIHintBoard(200005038);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                return;
            case 3:
            {
                //"攻城战准备中！"
                var ee = new ShowUIHintBoard(270280);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                return;
            case 4:
            {
                //"等级小于100级不能参加攻城战！"
                var ee = new ShowUIHintBoard(270281);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                return;
            case 5:
            {
            }
                return;
        }
        NetManager.Instance.StartCoroutine(EnterAllianceWar());
    }

    //进入城战条件检查
    private int CheckAttackJoin()
    {
        var battleCityDic = PlayerDataManager.Instance._battleCityDic;
        var isCanJoin = false;
        var unionId = BattleData.MyUnion.UnionID;
        foreach (var item in battleCityDic)
        {
            if (item.Key == unionId)
            {
                isCanJoin = true;
                break;
            }
        }

        if (!isCanJoin)
        {
            return 1;
        }

        isCanJoin = false;
        for (var i = 0; i < AttackCity.AttackName.Count; i++)
        {
            if (!string.IsNullOrEmpty(AttackCity.AttackName[i]))
            {
                isCanJoin = true;
                break;
            }
        }
        if (!isCanJoin)
        {
            return 2;
        }

        if (AttackCity.OpenState != (int) eAllianceWarState.WaitStart &&
            AttackCity.OpenState != (int) eAllianceWarState.Fight)
        {
            return 3;
        }

        if (PlayerDataManager.Instance.GetLevel() < Table.GetClientConfig(1153).Value.ToInt())
        {
            return 4;
        }

        if (SceneManager.Instance.CurrentSceneTypeId == _fuBenId)
        {
            return 5;
        }
        return 0;
    }

    //请求攻城战数据
    public IEnumerator EnterAllianceWar()
    {
        var unionId = BattleData.MyUnion.UnionID;
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.EnterAllianceWar(unionId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenNotInOpenTime)
                {
                    ApplyAllianceWarData();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    //攻城战初始化
    private void AttackInit()
    {
        _addPerCount = Table.GetClientConfig(902).Value.ToInt();
        _limitMin = Table.GetClientConfig(903).Value.ToInt();
        AttackCity.BindTips = string.Format(GameUtils.GetDictionaryText(270284), _limitMin, _addPerCount);
        AttackCity.TitleItem.Id = _titleId;
        GameUtils.TitleAddAttr(AttackCity.TitleItem, Table.GetNameTitle(_titleId));
        for (var i = 0; i < 4; i++)
        {
            AttackCity.EldersName[i] = string.Empty;
            if (i < 2)
            {
                AttackCity.AttackName[i] = string.Empty;
            }
        }

        var lists = new ReadonlyList<BattleUnionRewardItemDataModel>(4);
        for (var j = 0; j < 4; j++)
        {
            var item = new BattleUnionRewardItemDataModel();
            var tbGuildAccess = Table.GetGuildAccess(j);
            var tbMail = Table.GetMail(tbGuildAccess.MailId);
            for (var k = 0; k < 4; k++)
            {
                var tt = new ItemIconDataModel();
                tt.ItemId = tbMail.ItemId[k];
                tt.Count = tbMail.ItemCount[k];
                item.Items[k] = tt;
            }
            lists[3 - j] = item;
        }
        AttackCity.RewardItems = lists;
    }

    private void ApplyAllianceWarData()
    {
        NetManager.Instance.StartCoroutine(ApplyAllianceWarDataCoroutine());
    }

    private ulong _mModelGuid;
    //请求攻城战数据
    public IEnumerator ApplyAllianceWarDataCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceWarData(PlayerDataManager.Instance.ServerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var nonStr = GameUtils.GetDictionaryText(270292);
                    AttackCity.ViceCastellanName = nonStr;
                    AttackCity.CastellanName = nonStr;
                    for (var i = 0; i < AttackCity.EldersName.Count; i++)
                    {
                        AttackCity.EldersName[i] = nonStr;
                    }
                    var response = msg.Response;
                    AttackCity.BiddingCountStr = string.Format(GameUtils.GetDictionaryText(270263), response.SignUpCount);
                    AttackCity.BiddingCount = response.SignUpCount;
                    var memberCount = response.Members.Count;
                    var count = 0;
                    for (var i = 0; i < memberCount; i++)
                    {
                        var item = response.Members[i];
                        if (item.Ladder == 3)
                        {
                            AttackCity.CastellanName = item.Name;
                            _mModelGuid = item.Guid;
                        }
                        else if (item.Ladder == 2)
                        {
                            AttackCity.ViceCastellanName = item.Name;
                        }
                        else
                        {
                            AttackCity.EldersName[count] = item.Name;
                            count++;
                        }
                    }
                    if (mRefleshTime.Count > 0)
                    {
                        mRefleshTime[5] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                    }
                    AttackCity.OpenState = response.State;
                    RefleshAttackCityState(response.State);
                    if (_mModelGuid > 0)
                    {
                        PlayerDataManager.Instance.ApplyPlayerInfo(_mModelGuid, RefresCharacter);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    //请求城主信息
    private void ApplyAllianceWarOccupantData()
    {
        NetManager.Instance.StartCoroutine(ApplyAllianceWarOccupantDataCoroutine());
    }

    private void SyncOccupantChange(IEvent ievent)
    {
        var e = ievent as BattleUnionSyncOccupantChange;
        if (e.Data.OccupantId > 0)
        {
            var chat = new ChatMessageDataModel
            {
                Type = (int) eChatChannel.SystemScroll,
                CharId = 0,
                Content = string.Format(GameUtils.GetDictionaryText(300932), e.Data.OccupantName)
            };

            EventDispatcher.Instance.DispatchEvent(new Event_PushMessage(chat));
        }
        SetOccupantData(e.Data);
        CleanChallengerData();
        ApplyAllianceWarData();
    }

    private void CleanChallengerData()
    {
        var battleCityDic = PlayerDataManager.Instance._battleCityDic;
        var removeList = new List<int>();
        foreach (var i in battleCityDic)
        {
            if (i.Value.Type == 1)
            {
                removeList.Add(i.Key);
            }
        }
        for (var i = 0; i < removeList.Count; i++)
        {
            battleCityDic.Remove(removeList[i]);
        }

        for (var i = 0; i < AttackCity.AttackName.Count; i++)
        {
            AttackCity.AttackName[i] = string.Empty;
        }
    }

    //请求城主信息
    public IEnumerator ApplyAllianceWarOccupantDataCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceWarOccupantData(PlayerDataManager.Instance.ServerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    SetOccupantData(msg.Response);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void SetOccupantData(AllianceWarOccupantData data)
    {
        var unionId = BattleData.MyUnion.UnionID;
        var battleCityDic = PlayerDataManager.Instance._battleCityDic;
        var response = data;
        AttackCity.CastellanId = response.OccupantId;
        var nonStr = GameUtils.GetDictionaryText(270292);
        if (response.OccupantId > 0)
        {
            AttackCity.CastellanIsExist = 1;
        }
        else
        {
            AttackCity.CastellanIsExist = 0;
        }
        if (string.IsNullOrEmpty(response.OccupantName))
        {
            AttackCity.CityUnionName = nonStr;
        }
        else
        {
            AttackCity.CityUnionName = response.OccupantName;
        }

        var removeList = new List<int>();
        foreach (var i in battleCityDic)
        {
            if (i.Value.Type == 0)
            {
                removeList.Add(i.Key);
            }
        }
        for (var i = 0; i < removeList.Count; i++)
        {
            battleCityDic.Remove(removeList[i]);
        }

        if (battleCityDic.ContainsKey(response.OccupantId))
        {
            battleCityDic.Remove(response.OccupantId);
        }

        battleCityDic[response.OccupantId] = new PlayerDataManager.BattleCityData
        {
            Type = 0,
            Name = response.OccupantName
        };

        //如果是竞标阶段，城主不显示竞标功能。
        if (response.OccupantId == unionId)
        {
            if (AttackCity.OpenState == (int) eAllianceWarState.WaitBid)
            {
                AttackCity.OpenState = -1;
            }
        }
    }

    private void SyncChallengerDataChange(IEvent ievent)
    {
        var e = ievent as BattleUnionSyncChallengerDataChange;
        ChallengerDataChange(e.Data);
    }

    private void ApplyAllianceWarChallengerData()
    {
        NetManager.Instance.StartCoroutine(ApplyAllianceWarChallengerDataCoroutine());
    }

    //请求攻城战数据
    public IEnumerator ApplyAllianceWarChallengerDataCoroutine()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ApplyAllianceWarChallengerData(PlayerDataManager.Instance.ServerId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    ChallengerDataChange(msg.Response);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    private void ChallengerDataChange(AllianceWarChallengerData data)
    {
        var battleCityDic = PlayerDataManager.Instance._battleCityDic;
        var response = data;
        var removeList = new List<int>();
        foreach (var i in battleCityDic)
        {
            if (i.Value.Type == 1)
            {
                removeList.Add(i.Key);
            }
        }
        for (var i = 0; i < removeList.Count; i++)
        {
            battleCityDic.Remove(removeList[i]);
        }

        for (var i = 0; i < response.ChallengerName.Count; i++)
        {
            var id = response.ChallengerId[i];
            AttackCity.AttackName[i] = response.ChallengerName[i];
            if (battleCityDic.ContainsKey(response.ChallengerId[i]))
            {
                battleCityDic.Remove(id);
            }
            battleCityDic[id] = new PlayerDataManager.BattleCityData {Type = 1, Name = response.ChallengerName[i]};
        }
    }

    private void RefresCharacter(PlayerInfoMsg info)
    {
        PlayerDataManager.Instance.BattleUnionMaster = info;

        var e = new BattleUnionRefreshModelView(info);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void RefresCharacter2(PlayerInfoMsg info)
    {
        PlayerDataManager.Instance.BattleUnionMaster = info;

        var e = new BattleUnionRefreshModelViewLogic(info);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void RefleshAttackCityState(int state)
    {
        var mState = (eAllianceWarState) state;
        switch (mState)
        {
            case eAllianceWarState.WaitBid:
            case eAllianceWarState.Bid:
            {
                var timeStr = Table.GetClientConfig(901).Value;
                var tbFuben = Table.GetFuben(_fuBenId);
                var tt = timeStr.Split('|');
                var days = new List<int>();
                var nowDay = (int) Game.Instance.ServerTime.DayOfWeek;
                if (nowDay == 0)
                {
                    nowDay = 7;
                }

                for (var i = 0; i < tt.Count(); i++)
                {
                    days.Add(tt[i].ToInt());
                }
                //活动感觉结束，刷新状态
                if (days.Contains(nowDay))
                {
                    var startTime =
                        Game.Instance.ServerTime.Date.AddMinutes(tbFuben.OpenTime[0]/100*60 + tbFuben.OpenTime[0]%100);
                    if (Game.Instance.ServerTime > startTime)
                    {
                        nowDay++;
                        nowDay = nowDay > 7 ? 0 : nowDay;
                    }
                }

                var nextDay = tt[0].ToInt();
                for (var i = 0; i < tt.Length; i++)
                {
                    var date = tt[i].ToInt();
                    if (nowDay <= date)
                    {
                        nextDay = date;
                        break;
                    }
                }
                var strDay = (tbFuben.OpenTime[0]/100) + ":" + (tbFuben.OpenTime[0]%100).ToString("00");
                var dayDictId = 270263 + nextDay;
                AttackCity.OpenDayStr = string.Format(GameUtils.GetDictionaryText(270271),
                    GameUtils.GetDictionaryText(dayDictId), strDay);

                var time = tbFuben.OpenTime[0]/100*60 + tbFuben.OpenTime[0]%100;
                AttackCity.NextAttackTime =
                    GameUtils.GetTimeDiffString(Game.Instance.ServerTime.AddDays(1).Date.AddMinutes(time));
                AttackCity.BiddingFinishTime = Game.Instance.ServerTime.AddDays(1).Date;
            }
                break;
            case eAllianceWarState.WaitEnter:
            case eAllianceWarState.WaitStart:
            {
                var tbFuben = Table.GetFuben(_fuBenId);
                    if (tbFuben == null)
                    {
                        return;
                    }
                var time = tbFuben.OpenTime[0]/100*60 + tbFuben.OpenTime[0]%100;
                var startTime = Game.Instance.ServerTime.Date.AddMinutes(time);
                var timeReady = Game.Instance.ServerTime.Date.AddMinutes(time + tbFuben.CanEnterTime);


                if (Game.Instance.ServerTime < startTime)
                {
                    if (_attackCityStateTrigger != null)
                    {
                        TimeManager.Instance.DeleteTrigger(_attackCityStateTrigger);
                        _attackCityStateTrigger = null;
                    }
                    _attackCityStateTrigger = TimeManager.Instance.CreateTrigger(startTime, () =>
                    {
                        if (_attackCityStateTrigger != null)
                        {
                            TimeManager.Instance.DeleteTrigger(_attackCityStateTrigger);
                            _attackCityStateTrigger = null;
                        }
                        AttackCity.OpenState = (int) eAllianceWarState.WaitStart;
                        if (SceneManager.Instance.CurrentSceneTypeId != _fuBenId)
                        {
                            if (CheckAttackJoin() != 0)
                            {
                                return;
                            }
                            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 300921, "", () => { AttackJoin(); });
                        }
                    });
                }
                else if (Game.Instance.ServerTime < timeReady)
                {
                    if (_attackCityStateTrigger != null)
                    {
                        TimeManager.Instance.DeleteTrigger(_attackCityReadyTrigger);
                        _attackCityReadyTrigger = null;
                    }
                    _attackCityReadyTrigger = TimeManager.Instance.CreateTrigger(timeReady, () =>
                    {
                        if (_attackCityReadyTrigger != null)
                        {
                            TimeManager.Instance.DeleteTrigger(_attackCityReadyTrigger);
                            _attackCityReadyTrigger = null;
                        }
                        AttackCity.OpenState = (int) eAllianceWarState.Fight;
                        if (SceneManager.Instance.CurrentSceneTypeId != _fuBenId)
                        {
                            if (CheckAttackJoin() != 0)
                            {
                                return;
                            }
                            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, 300921, "", () => { AttackJoin(); });
                        }
                    });
                }
            }
                break;
            case eAllianceWarState.Fight:
            {
            }
                break;
        }
    }

    //竞价
    private void BtnAddBidding()
    {
        var bidMoney = AttackCity.BiddingMoney;
        //选择显示竞价或者增价界面
        if (bidMoney <= 0)
        {
            var money = _limitMin + _addPerCount + mGuildRecord.MaintainMoney;
            if (BattleData.MyUnion.Money < money)
            {
                //"战盟资金小于{0}，无法参与竞价"
                var e = new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(270282), money));
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }

            AttackCity.BiddingState = 2;
            AttackCity.BiddingTextMoney = _limitMin;
            SetVarBattleMoney();
        }
        else
        {
            var money = mGuildRecord.MaintainMoney + _addPerCount;
            if (BattleData.MyUnion.Money < money)
            {
                //"战盟资金小于{0}，无法参与竞价"
                var e = new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(270282), money));
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
            AttackCity.BiddingState = 3;
            AttackCity.BiddingTextMoney = _addPerCount;
            AttackCity.BiddingTotalMoney = AttackCity.BiddingMoney + _addPerCount;
            SetVarBattleMoney();
        }
        SetBtnState();
    }

    //关闭竞价窗口
    private void BattleBiddingClose()
    {
        var bidMoney = AttackCity.BiddingMoney;
        if (bidMoney <= 0)
        {
            AttackCity.BiddingState = 0;
            AttackCity.BiddingTextMoney = _limitMin;
        }
        else
        {
            AttackCity.BiddingState = 1;
        }
    }

    ////确定竞价
    //private void BattleBiddingOk()
    //{
    //    AttackCity.BiddingState = 0;
    //}


    //竞价
    private void BtnBiddingSub()
    {
        NetManager.Instance.StartCoroutine(ApplyBid(AttackCity.BiddingTextMoney, 1));
    }

    public IEnumerator ApplyBid(int price, int type)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.BidAllianceWar(price);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    var oldMoney = AttackCity.BiddingMoney;
                    AttackCity.BiddingMoney = msg.Response;
                    if (type == 0)
                    {
                        if (msg.Response <= 0) //竞价
                        {
                            AttackCity.BiddingTextMoney = _limitMin;
                            //lable 出价
                            AttackCity.BiddingState = 0;
                        }
                        else
                        {
                            AttackCity.BiddingTextMoney = msg.Response;
                            //lable 增价
                            AttackCity.BiddingState = 1;
                        }
                    }
                    else
                    {
                        AttackCity.BiddingState = 1;
                        BattleData.MyUnion.Money -= price;
                        if (oldMoney <= 0)
                        {
                            AttackCity.BiddingCountStr = string.Format(GameUtils.GetDictionaryText(270263),
                                AttackCity.BiddingCount + 1);
                        }
                        //"您已成功竞价，金额为：{0}"
                        var ee =
                            new ShowUIHintBoard(string.Format(GameUtils.GetDictionaryText(270283),
                                AttackCity.BiddingMoney));
                        EventDispatcher.Instance.DispatchEvent(ee);
                    }
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public void OnBingingCountChange(IEvent ievent)
    {
        var e = ievent as BattleUnionCountChange;
        var type = e.Type;
        if (e.Index == 0)
        {
            OnAdd(type);
        }
        else if (e.Index == 1)
        {
            OnDel(type);
        }
        else if (e.Index == 2) //2 Add Press
        {
            _buttonPress = NetManager.Instance.StartCoroutine(ButtonAddOnPress(type));
        }
        else if (e.Index == 3) //3 Del Press
        {
            _buttonPress = NetManager.Instance.StartCoroutine(ButtonDelOnPress(type));
        }
        else if (e.Index == 4) //Add Release
        {
            if (_buttonPress != null)
            {
                NetManager.Instance.StopCoroutine(_buttonPress);
                _buttonPress = null;
            }
        }
        else if (e.Index == 5) //Del Release
        {
            if (_buttonPress != null)
            {
                NetManager.Instance.StopCoroutine(_buttonPress);
                _buttonPress = null;
            }
        }
    }

    public bool OnAdd(int type)
    {
        if (type == 0)
        {
            if (AttackCity.BiddingTextMoney + _addPerCount + _mGuildRecord.MaintainMoney > BattleData.MyUnion.Money)
            {
                SetBtnState();
                return false;
            }
            AttackCity.BiddingTextMoney += _addPerCount;
            AttackCity.BiddingTotalMoney += _addPerCount;
            SetVarBattleMoney();
            SetBtnState();
            return true;
        }
        return false;
    }

    public bool OnDel(int type)
    {
        var limit = 0;
        if (AttackCity.BiddingState == 2)
        {
            limit = _limitMin;
        }
        else
        {
            limit = _addPerCount;
        }

        if (type == 0)
        {
            if (AttackCity.BiddingTextMoney - _addPerCount < limit)
            {
                SetBtnState();
                return false;
            }
            AttackCity.BiddingTextMoney -= _addPerCount;
            AttackCity.BiddingTotalMoney -= _addPerCount;
            SetVarBattleMoney();
            SetBtnState();
            return true;
        }
        return false;
    }

    private void SetVarBattleMoney()
    {
        AttackCity.VarBattleMoney = BattleData.MyUnion.Money - AttackCity.BiddingTextMoney;
        if (AttackCity.BiddingTextMoney + _addPerCount + _mGuildRecord.MaintainMoney > BattleData.MyUnion.Money)
        {
            //红
            AttackCity.VarBattleMoneyColor = GameUtils.GetTableColor(10);
        }
        else
        {
            //白
            AttackCity.VarBattleMoneyColor = GameUtils.GetTableColor(0);
        }
    }

    private void SetBtnState()
    {
        if (AttackCity.BiddingTextMoney + _addPerCount + _mGuildRecord.MaintainMoney > BattleData.MyUnion.Money)
        {
            AttackCity.BtnAddIsGray = true;
        }
        else
        {
            AttackCity.BtnAddIsGray = false;
        }
        var limit = 0;
        if (AttackCity.BiddingState == 2)
        {
            limit = _limitMin;
        }
        else
        {
            limit = _addPerCount;
        }
        if (AttackCity.BiddingTextMoney - _addPerCount < limit)
        {
            AttackCity.BtnDelIsGray = true;
        }
        else
        {
            AttackCity.BtnDelIsGray = false;
        }
    }


    public IEnumerator ButtonAddOnPress(int type)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnAdd(type) == false)
            {
                NetManager.Instance.StopCoroutine(_buttonPress);
                _buttonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    public IEnumerator ButtonDelOnPress(int type)
    {
        var pressCd = 0.25f;
        while (true)
        {
            yield return new WaitForSeconds(pressCd);
            if (OnDel(type) == false)
            {
                NetManager.Instance.StopCoroutine(_buttonPress);
                _buttonPress = null;
                yield break;
            }
            if (pressCd > 0.01)
            {
                pressCd = pressCd*0.8f;
            }
        }
        yield break;
    }

    #endregion

    #region Tab事件

    public void TabPageClick(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionTabPageClick;
        BattleData.TabPage = e.Index;
        RefleshTabPage(e.Index);
    }

    public void TabPageClick2(IEvent ievent)
    {
        var e = ievent as UIEvent_UnionTabPageClick2;
        BattleData.TabPage2 = e.Index;
        switch (e.Index)
        {
            case 0:
            {
            }
                break;
            case 1: //其他战盟
            {
                CanRefleshData(RefleshType.OtherUnion);
            }
                break;
        }
    }

    //更新page页面信息
    public void UpdatePage()
    {
        if (unionLevelChanged != BattleData.MyUnion.Level)
        {
            unionLevelChanged = BattleData.MyUnion.Level;
            RefleshUnionLevel();
            InitBuff();
            InitShop();
            InitBuffName();
            RefleshBuffUI(BuffSelected);
        }
        switch (BattleData.TabPage)
        {
            case (int) TabPage.PageInfo:
            {
                UnionCanLevel();
            }
                break;
            case (int) TabPage.PageBuild:
            {
            }
                break;
            case (int) TabPage.PageLevel:
            {
                mRefleshTime[3] = Game.Instance.ServerTime.AddSeconds(INTERVEL_TIME);
                RefleshBuffUI(BuffSelected);
            }
                break;
            case (int) TabPage.PageCity:
            {
            }
                break;
            case (int) TabPage.PageShop:
            {
            }
                break;
        }
    }

    private void InitBuff()
    {
        for (var i = 0; i < 4; i++)
        {
            Buff.BuffList[i].BuffID = PlayerDataManager.Instance.GetExData(550 + i);
        }
    }

    //退出战盟按钮
    public void TabOutUnion()
    {
        if (BattleData.MyPorp.Ladder == (int) battleAccess.Chief)
        {
            if (mUnionMembers.Count > 1)
            {
                QuitUnionMsg();
            }
            else
            {
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(220961), "",
                    () => { NetManager.Instance.StartCoroutine(ApplyOperation(2, BattleData.MyUnion.UnionID)); }
                    );
            }
        }
        else
        {
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, GameUtils.GetDictionaryText(220929), "",
                () => { NetManager.Instance.StartCoroutine(ApplyOperation(2, BattleData.MyUnion.UnionID)); }
                );
        }
    }

    //退出战盟按钮
    public void QuitUnionMsg()
    {
        var varLadder = -1;
        ulong maxLadder = 0;
        {
            // foreach(var varitem in mUnionMembers)
            var __enumerator11 = (mUnionMembers).GetEnumerator();
            while (__enumerator11.MoveNext())
            {
                var varitem = __enumerator11.Current;
                {
                    var item = varitem.Value;
                    if (item.Ladder == (int) battleAccess.Chief)
                    {
                        continue;
                    }
                    if (item.Ladder > varLadder)
                    {
                        varLadder = item.Ladder;
                        maxLadder = varitem.Key;
                    }
                    else if (item.Ladder == varLadder)
                    {
                        if (item.DonationCount > mUnionMembers[maxLadder].DonationCount)
                        {
                            varLadder = item.Ladder;
                            maxLadder = varitem.Key;
                        }
                    }
                }
            }
        }
        //转让
        var str = string.Format(GameUtils.GetDictionaryText(220945), mUnionMembers[maxLadder].Name);
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
            () => { NetManager.Instance.StartCoroutine(ApplyOperation(2, BattleData.MyUnion.UnionID)); }
            );
    }

    #endregion

    #region 基础接口

    public INotifyPropertyChanged GetDataModel(string name)
    {
        switch (name)
        {
            case "Info":
            {
                return Info;
            }
                break;
            case "Build":
            {
                return Build;
            }
                break;
            case "Buff":
            {
                return Buff;
            }
                break;
            case "Boss":
            {
                return Boss;
            }
                break;
            case "Shop":
            {
                return Shop;
            }
                break;
            case "OtherUnion":
            {
                return OtherUnion;
            }
                break;
            case "BattleData":
            {
                return BattleData;
            }
                break;
            case "AttackCity":
            {
                return AttackCity;
            }
                break;
        }
        return null;
    }

    public void CleanUp()
    {
        if (BattleData != null)
        {
            BattleData.CheckBox.PropertyChanged -= OnToggleChange;
        }

        BattleData = new BattleUnionDataModel();
        Info = new BattleUnionInfoDataModel();
        Build = new BattleUnionBuildDataModel();
        Buff = new BattleUnionBuffDataModel();
        Boss = new BattleUnionBossDataModel();
        Shop = new BattleUnionShopDataModel();
        OtherUnion = new BattleUnionOtherUnionDataModel();
        AttackCity = new BattleUnionAttackCityDataModel();

        var tbClient = Table.GetClientConfig(241);
        var money = int.Parse(tbClient.Value)/10000;
        Build.MaxDonation = int.Parse(Table.GetClientConfig(280).Value);
        BattleData.CreateUIStr1 = string.Format(GameUtils.GetDictionaryText(220967), money);

        tbClient = Table.GetClientConfig(242);
        BattleData.CreateUIStr2 = string.Format(GameUtils.GetDictionaryText(220968), tbClient.Value);


        BattleData.CheckBox.PropertyChanged += OnToggleChange;

        InputStr = GameUtils.GetDictionaryText(100000996);
        BattleData.CreateName = InputStr;
        mCacheDicLP.Clear();
        mCacheDic.Clear();
        unionMaxLevel = 0;
        Table.ForeachGuild(record =>
        {
            var value = record.Id;
            if (!mCacheDicLP.ContainsKey(record.StoreParam))
            {
                mCacheDicLP.Add(record.StoreParam, value);
            }
            unionMaxLevel++;
            return true;
        });
        if (unionMaxLevel >= 1)
        {
            InitShopDictionary();
        }
        AttackInit();
    }

    public void OnChangeScene(int sceneId)
    {
    }

    public object CallFromOtherClass(string name, object[] param)
    {
        if (name == "HasUnion")
        {
            return HasUnion();
        }
        return null;
    }

    private bool HasUnion()
    {
        return BattleData.MyUnion.UnionID > 0;
    }

    public FrameState State { get; set; }

    public void Close()
    {
        PlayerDataManager.Instance.CloseCharacterPopMenu();
        for (var i = 0; i < Build.DonationItem.Count; i++)
        {
            var item = Build.DonationItem[i];
            if (item.TimerCoroutine != null)
            {
                NetManager.Instance.StopCoroutine(item.TimerCoroutine);
                item.TimerCoroutine = null;
            }
        }
        //mCatchOhterUnion.Clear();
        //mOtherUnionDict.Clear();
        //mRefleshTime.Clear();
    }

    public void Tick()
    {
    }

    public void OnShow()
    {
        if (_mModelGuid > 0)
        {
            PlayerDataManager.Instance.ApplyPlayerInfo(_mModelGuid, RefresCharacter2);
        }
    }

    public void RefreshData(UIInitArguments data)
    {
        var tabId = 0;
        var args = data as BattleUnionArguments;
        if (args != null)
        {
            tabId = args.Tab;
        }
        mRefleshTime.Clear();
        for (var i = 0; i < REFLESH_COUNT; i++)
        {
            mRefleshTime.Add(Game.Instance.ServerTime);
        }
        MainUIInit(tabId);

        CanRefleshData(RefleshType.OtherUnion);
    }

    #region OnToggleChange

    //对勾等的响应事件
    public void OnToggleChange(object sender, PropertyChangedEventArgs e)
    {
        //if (e.PropertyName == "ShowOffLine")
        //{
        //    ShowInfo(0);
        //}
        //else
        if (e.PropertyName == "SelectAll")
        {
            {
                // foreach(var item in Info.ShowList)
                var __enumerator12 = (BattleData.MyUnion.ApplyList).GetEnumerator();
                while (__enumerator12.MoveNext())
                {
                    var item = __enumerator12.Current;
                    {
                        if (BattleData.CheckBox.SelectAll)
                        {
                            item.Selected = 1;
                        }
                        else
                        {
                            item.Selected = 0;
                        }
                    }
                }
            }
        }
        else if (e.PropertyName == "CreateAutoAccept")
        {
        }
        else if (e.PropertyName == "ShowAutoJoin")
        {
            OtherUnion.OtherUnionList.Clear();
            OtherUnionSelectIndex = -1;
            if (BattleData.CheckBox.ShowAutoJoin)
            {
                for (var i = 0; i < mCatchOhterUnion.Count; i++)
                {
                    var item = mCatchOhterUnion[i];
                    if (item.AutoAccept)
                    {
                        var vardata = new BattleUnionTeamSimpleDataModel(item);
                        vardata.Selected = 0;
                        OtherUnion.OtherUnionList.Add(vardata);
                    }
                }
            }
            else
            {
                for (var i = 0; i < mCatchOhterUnion.Count; i++)
                {
                    var item = mCatchOhterUnion[i];
                    var vardata = new BattleUnionTeamSimpleDataModel(item);
                    vardata.Selected = 0;
                    OtherUnion.OtherUnionList.Add(vardata);
                }
            }
            //申请加入
            OtherUnion.JoinBtnText = GameUtils.GetDictionaryText(230301);
        }
        else if (e.PropertyName == "ShowOffLineDetail")
        {
            ShowInfo(2);
        }
    }

    #endregion

    #endregion
}