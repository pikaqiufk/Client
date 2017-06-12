#region using

using System;
using System.Collections.Generic;
using ClientDataModel;
using DataContract;
using UnityEngine;

#endregion

namespace EventSystem
{
    public class LineMemberClickEvent : EventBase
    {
        public static string EVENT_TYPE = "LineMemberClickEvent";

        public LineMemberClickEvent(int t, int i = -1)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class LineMemberConfirmEvent : EventBase
    {
        public static string EVENT_TYPE = "LineMemberConfirmEvent";

        public LineMemberConfirmEvent(ulong id, int t)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
            Type = t;
        }

        public ulong CharacterId { get; set; }
        public int Type { get; set; }
    }

    public class EquipBagNotFullChange : EventBase
    {
        public static string EVENT_TYPE = "EquipBagNotFullChange";

        public EquipBagNotFullChange()
            : base(EVENT_TYPE)
        {
        }
    }

    public class IpAddressSet : EventBase
    {
        public static string EVENT_TYPE = "IpAddressSet";

        public IpAddressSet(string p, string c)
            : base(EVENT_TYPE)
        {
            Province = p;
            City = c;
        }

        public string City { get; set; }
        public string Province { get; set; }
    }

    public class PlayerCampChangeEvent : EventBase
    {
        public static string EVENT_TYPE = "PlayerCampChangeEvent";

        public PlayerCampChangeEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class FrindChatNotifyProvider : EventBase
    {
        public static string EVENT_TYPE = "FrindChatNotifyProvider";

        public FrindChatNotifyProvider(UIWidget w)
            : base(EVENT_TYPE)
        {
            Widget = w;
        }

        public UIWidget Widget { get; set; }
    }

    public class ChatCityCellClick : EventBase
    {
        public static string EVENT_TYPE = "ChatCityCellClick";

        public ChatCityCellClick(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class ChatMainOperate : EventBase
    {
        public static string EVENT_TYPE = "ChatMainOperate";

        public ChatMainOperate(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ChatSoundTranslateAddEvent : EventBase
    {
        public static string EVENT_TYPE = "ChatSoundTranslateAddEvent";

        public ChatSoundTranslateAddEvent(byte[] chat)
            : base(EVENT_TYPE)
        {
            SoundData = chat;
        }

        public byte[] SoundData { get; set; }
    }

    public class AddFaceNode : EventBase
    {
        public static string EVENT_TYPE = "AddFaceNode";

        public AddFaceNode(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            FaceId = i;
        }

        public int FaceId { get; set; }
        public int Type { get; set; }
    }

    public class FaceListClickIndex : EventBase
    {
        public static string EVENT_TYPE = "FaceListClickIndex";

        public FaceListClickIndex(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class CloseUiBindRemove : EventBase
    {
        public static string EVENT_TYPE = "CloseUiBindRemove";

        public CloseUiBindRemove(UIConfig config, int need)
            : base(EVENT_TYPE)
        {
            NeedRemove = need;
            Config = config;
        }

        public UIConfig Config { get; set; }
        public int NeedRemove { get; set; } // 0 设置 1 移出
    }

    public class RefreshReliveInfoEvent : EventBase
    {
        public static string EVENT_TYPE = "RefreshReliveInfoEvent";

        public RefreshReliveInfoEvent( string n)
            : base(EVENT_TYPE)
        {
            KillerName = n;
        }

        public DateTime FreeTime { get; set; }
        public string KillerName { get; set; }
    }

    public class SystemNoticeOperate : EventBase
    {
        public static string EVENT_TYPE = "SystemNoticeOperate";

        public SystemNoticeOperate(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class SystemNoticeNotify : EventBase
    {
        public static string EVENT_TYPE = "SystemNoticeNotify";

        public SystemNoticeNotify(string c)
            : base(EVENT_TYPE)
        {
            Content = c;
        }

        public string Content { get; set; }
    }

    public class UiFrameCloseEvent : EventBase
    {
        public static string EVENT_TYPE = "UiFrameCloseEvent";

        public UiFrameCloseEvent(UIConfig c)
            : base(EVENT_TYPE)
        {
            Config = c;
        }

        public UIConfig Config { get; set; }
    }

    public class UiFrameShowEvent : EventBase
    {
        public static string EVENT_TYPE = "UIFrameShowEvent";

        public UiFrameShowEvent(UIConfig c, UIInitArguments arg = null)
            : base(EVENT_TYPE)
        {
            Config = c;
            Arg = arg;
        }

        public UIInitArguments Arg { get; set; }
        public UIConfig Config { get; set; }
    }

    public class BattleResultClick : EventBase
    {
        public static string EVENT_TYPE = "BattleResultClick";

        public BattleResultClick()
            : base(EVENT_TYPE)
        {
        }
    }

    public class MainUiCharRadar : EventBase
    {
        public static string EVENT_TYPE = "MainUiCharRadar";
        public RararCharDataModel DataModel { get; set; }
        public int Type { get; set; }
        public MainUiCharRadar(RararCharDataModel d, int t)
            : base(EVENT_TYPE)
        {
            DataModel = d;
            Type = t;
        }
    }

    public class MieShiSceneMapRadar : EventBase
    {
        public static string EVENT_TYPE = "MieShiSceneMapRadar";
        public MapRadarDataModel DataModel { get; set; }
        public string Prefab { get; set; }
        public int Type { get; set; }
        public MieShiSceneMapRadar(MapRadarDataModel d, int t, string prefab)
            : base(EVENT_TYPE)
        {
            DataModel = d;
            Type = t;
            Prefab = prefab;
        }
    }

    public class MieShiSceneMapRemoveRadar : EventBase
    {
        public static string EVENT_TYPE = "MieShiSceneMapRadar";
        public ulong id { get; set; }
        public MieShiSceneMapRemoveRadar(ulong d)
            : base(EVENT_TYPE)
        {
            id = d;
        }
    }


    public class CityDataInitEvent : EventBase
    {
        public static string EVENT_TYPE = "CityDataInitEvent";

        public CityDataInitEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class CityWeakNoticeRefreshEvent : EventBase
    {
        public static string EVENT_TYPE = "CityWeakNoticeRefreshEvent";

        public CityWeakNoticeRefreshEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ConditionChangeEvent : EventBase
    {
        public static string EVENT_TYPE = "ConditionChangeEvent";

        public ConditionChangeEvent(int id)
            : base(EVENT_TYPE)
        {
            ConditionId = id;
        }

        public int ConditionId { get; set; }
    }

    public class BagItemCountChangeEvent : EventBase
    {
        public static string EVENT_TYPE = "BagItemCountChangeEvent";

        public BagItemCountChangeEvent(Dictionary<int, int> itemchange)
            : base(EVENT_TYPE)
        {
            ItemChanges = itemchange;
        }

        public Dictionary<int, int> ItemChanges;
    }

    public class RebornOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "RebornOperateEvent";

        public RebornOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class RebornPlayAnimation : EventBase
    {
        public static string EVENT_TYPE = "RebornPlayAnimation";

        public RebornPlayAnimation()
            : base(EVENT_TYPE)
        {
        }
    }

    public class SkillEquipMainUiAnime : EventBase
    {
        public static string EVENT_TYPE = "SkillEquipMainUiAnime";

        public SkillEquipMainUiAnime(int id, int i)
            : base(EVENT_TYPE)
        {
            SkillId = id;
            Index = i;
        }

        public int Index { get; set; }
        public int SkillId { get; set; }
    }

    public class ChatItemListClick : EventBase
    {
        public static string EVENT_TYPE = "ChatItemListClick";

        public ChatItemListClick(BagItemDataModel d)
            : base(EVENT_TYPE)
        {
            DataModel = d;
        }

        public BagItemDataModel DataModel { get; set; }
    }

    public class ChatShareItemEvent : EventBase
    {
        public static string EVENT_TYPE = "ChatShareItemEvent";

        public ChatShareItemEvent(int t, BagItemDataModel data)
            : base(EVENT_TYPE)
        {
            Type = t;
            Data = data;
        }

        public BagItemDataModel Data { get; set; }
        public int Type { get; set; }
    }

    public class ShowCountdownEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowCountdownEvent";

        public ShowCountdownEvent(DateTime time, eCountdownType type)
            : base(EVENT_TYPE)
        {
            Time = time;
            Type = type;
        }

        public DateTime Time { get; set; }
        public eCountdownType Type { get; set; }
    }

    public class FriendClickTabEvent : EventBase
    {
        public static string EVENT_TYPE = "FriendClickTabEvent";

        public FriendClickTabEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class BattleCellClick : EventBase
    {
        public static string EVENT_TYPE = "BattleCellClick";

        public BattleCellClick(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class BattleOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "BattleOperateEvent";

        public BattleOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class BattleQueueEvent : EventBase
    {
        public static string EVENT_TYPE = "BattleQueueEvent";

        public BattleQueueEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class AttriFrameOperate : EventBase
    {
        public static string EVENT_TYPE = "AttriFrameOperate";

        public AttriFrameOperate(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class FarmOrderListClick : EventBase
    {
        public static string EVENT_TYPE = "FarmOrderListClick";

        public FarmOrderListClick(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class PvpFightReadyEent : EventBase
    {
        public static string EVENT_TYPE = "PvpFightReadyEent";

        public PvpFightReadyEent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class SkillBookTalentChange : EventBase
    {
        public static string EVENT_TYPE = "eventName";

        public SkillBookTalentChange(int id)
            : base(EVENT_TYPE)
        {
            SkillId = id;
        }

        public int SkillId { get; set; }
    }

    public class SkillBookOperate : EventBase
    {
        public static string EVENT_TYPE = "SkillBookOperate";

        public SkillBookOperate(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class FightValueChange : EventBase
    {
        public static string EVENT_TYPE = "FightValueChange";

        public FightValueChange(int begin, int end)
            : base(EVENT_TYPE)
        {
            BeginValue = begin;
            EndValue = end;
        }

        public int BeginValue { get; set; }
        public int EndValue { get; set; }
    }

    public class AttributeDistributionChange : EventBase
    {
        public static string EVENT_TYPE = "AttributeDistributionChange";

        public AttributeDistributionChange(int c)
            : base(EVENT_TYPE)
        {
            Count = c;
        }

        public int Count { get; set; }
    }

    public class MainUISwithState : EventBase
    {
        public static string EVENT_TYPE = "MainUISwithState";

        public MainUISwithState(bool att)
            : base(EVENT_TYPE)
        {
            IsAttack = att;
        }

        public bool IsAttack { get; set; }
    }

//     public class OpenCompseUI : EventBase
//     {
//         public static string EVENT_TYPE = "ItemInfoCountChange";
// 
// 
//         public OpenCompseUI()
//             : base(EVENT_TYPE)
//         {
// 
//         }
//     }
//     
//         public class UIEvet_ShowPageOther : EventBase
//     {
//         public static string EVENT_TYPE = "ItemInfoCountChange";
// 
// 
//         public UIEvet_ShowPageOther()
//             : base(EVENT_TYPE)
//         {
// 
//         }
//     }
    public class ItemInfoCountChange : EventBase
    {
        public static string EVENT_TYPE = "ItemInfoCountChange";

        public ItemInfoCountChange(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class Event_ItemInfoClick : EventBase
    {
        public static string EVENT_TYPE = "Event_ItemInfoClick";

        public Event_ItemInfoClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class Event_ItemInfoGetClick : EventBase
    {
        public static string EVENT_TYPE = "Event_ItemInfoGetClick";

        public Event_ItemInfoGetClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class EquipUiNotifyLogic : EventBase
    {
        public static string EVENT_TYPE = "EquipUiNotifyLogic";

        public EquipUiNotifyLogic(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }


    public class EquipUiNotifyRefreshCoumuseList : EventBase
    {
        public static string EVENT_TYPE = "EquipUiNotifyRefreshCoumuseList";

        public EquipUiNotifyRefreshCoumuseList()
            : base(EVENT_TYPE)
        {
        }

        public int Type { get; set; }
    }

    public class EquipUIPackStart : EventBase
    {
        public static string EVENT_TYPE = "EquipUIPackStart";

        public EquipUIPackStart(int p, int s)
            : base(EVENT_TYPE)
        {
            Pack = p;
            Start = s;
        }

        public int Pack { get; set; }
        public int Start { get; set; }
    }

    public class MessageBoxClick : EventBase
    {
        public static string EVENT_TYPE = "MessageBoxClick";

        public MessageBoxClick(int t = 0)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class DungeonCompleteEvent : EventBase
    {
        public static string EVENT_TYPE = "DungeonCompleteEvent";

        public DungeonCompleteEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class BattleUnionExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "BattleUnionExdataUpdate";

        public BattleUnionExdataUpdate(eExdataDefine t, int v)
            : base(EVENT_TYPE)
        {
            Type = t;
            Value = v;
        }

        public eExdataDefine Type { get; set; }
        public int Value { get; set; }
    }

    public class SettingExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "SettingExdataUpdate";

        public SettingExdataUpdate(eExdataDefine t, int v)
            : base(EVENT_TYPE)
        {
            Type = t;
            Value = v;
        }

        public eExdataDefine Type { get; set; }
        public int Value { get; set; }
    }

    public class ArenaExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "ArenaExdataUpdate";

        public ArenaExdataUpdate(eExdataDefine t, int v)
            : base(EVENT_TYPE)
        {
            Type = t;
            Value = v;
        }

        public eExdataDefine Type { get; set; }
        public int Value { get; set; }
    }

    public class UIEvent_ArenaFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ArenaFlyAnim";

        public UIEvent_ArenaFlyAnim(int idx, int count)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Count = count;
        }

        public int Count { get; set; }
        public int Idx { get; set; }
    }

    public class FruitExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "FruitExdataUpdate";

        public FruitExdataUpdate(eExdataDefine t, int v)
            : base(EVENT_TYPE)
        {
            Type = t;
            Value = v;
        }

        public eExdataDefine Type { get; set; }
        public int Value { get; set; }
    }

    public class ElfExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "ElfExdataUpdate";

        public ElfExdataUpdate(eExdataDefine t, int v)
            : base(EVENT_TYPE)
        {
            Type = t;
            Value = v;
        }

        public eExdataDefine Type { get; set; }
        public int Value { get; set; }
    }

    public class DungeonResetCountUpdate : EventBase
    {
        public static string EVENT_TYPE = "DungeonResetCountUpdate";

        public DungeonResetCountUpdate(int id, int count)
            : base(EVENT_TYPE)
        {
            DungeonId = id;
            Count = count;
        }

        public int Count { get; set; }
        public int DungeonId { get; set; }
    }

    public class DungeonEnterCountUpdate : EventBase
    {
        public static string EVENT_TYPE = "DungeonEnterCountUpdate";

        public DungeonEnterCountUpdate(int id, int count)
            : base(EVENT_TYPE)
        {
            DungeonId = id;
            Count = count;
        }

        public int Count { get; set; }
        public int DungeonId { get; set; }
    }

    public class ArenaBulindNoticeRefresh : EventBase
    {
        public static string EVENT_TYPE = "ArenaBulindNoticeRefresh";

        public ArenaBulindNoticeRefresh(BuildingData data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public BuildingData Data { get; set; }
    }

    public class CityNoticeFlagRefrsh : EventBase
    {
        public static string EVENT_TYPE = "CityNoticeFlagRefrsh";

        public CityNoticeFlagRefrsh(string name, bool flag)
            : base(EVENT_TYPE)
        {
            Name = name;
            Flag = flag;
        }

        public bool Flag { get; set; }
        public string Name { get; set; }
    }

    public class CityNoticeOnClick : EventBase
    {
        public static string EVENT_TYPE = "CityNoticeOnClick";

        public CityNoticeOnClick(int area)
            : base(EVENT_TYPE)
        {
            Area = area;
        }

        public int Area { get; set; }
    }

    public class CityWeakNoticeOnClick : EventBase
    {
        public static string EVENT_TYPE = "CityWeakNoticeOnClick";

        public CityWeakNoticeOnClick(int area)
            : base(EVENT_TYPE)
        {
            Area = area;
        }

        public int Area { get; set; }
    }

    public class CityBulidingNoticeRefresh : EventBase
    {
        public static string EVENT_TYPE = "CityBulidingNoticeRefresh";

        public CityBulidingNoticeRefresh(BuildingData data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public BuildingData Data { get; set; }
    }

    public class CityBulidingWeakNoticeRefresh : EventBase
    {
        public static string EVENT_TYPE = "CityBulidingWeakNoticeRefresh";

        public CityBulidingWeakNoticeRefresh(BuildingData data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public BuildingData Data { get; set; }
    }

    public class CityBulidingNoticeAdd : EventBase
    {
        public static string EVENT_TYPE = "CityBulidingNoticeAdd";

        public CityBulidingNoticeAdd(int data)
            : base(EVENT_TYPE)
        {
            AreaId = data;
        }

        public int AreaId { get; set; }
    }

    public class CityGetResAnim : EventBase
    {
        public static string EVENT_TYPE = "CityGetResAnim";

        public CityGetResAnim(int areaId, int type, int itemId, int count)
            : base(EVENT_TYPE)
        {
            ItemId = itemId;
            AreaId = areaId;
            Count = count;
            Type = type;
        }

        public int AreaId { get; set; }
        public int Count { get; set; }
        public int ItemId { get; set; }
        public int Type { get; set; }
    }

    public class DungeonResultChoose : EventBase
    {
        public static string EVENT_TYPE = "DungeonResultChoose";

        public DungeonResultChoose(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class ArenaNotifyLogic : EventBase
    {
        public static string EVENT_TYPE = "ArenaNotifyLogic";

        public ArenaNotifyLogic(int t, int i = -1)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class SatueNotifyEvent : EventBase
    {
        public static string EVENT_TYPE = "SatueNotifyEvent";

        public SatueNotifyEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ArenaSatueCellClick : EventBase
    {
        public static string EVENT_TYPE = "ArenaSatueCellClick";

        public ArenaSatueCellClick()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ArenaPetListEvent : EventBase
    {
        public static string EVENT_TYPE = "ArenaPetListEvent";

        public ArenaPetListEvent(bool show)
            : base(EVENT_TYPE)
        {
            IsShow = show;
        }

        public bool IsShow { get; set; }
    }

    public class RelieveOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "RelieveOperateEvent";

        public RelieveOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class MapSceneCancelPath : EventBase
    {
        public static string EVENT_TYPE = "MapSceneCancelPath";

        public MapSceneCancelPath()
            : base(EVENT_TYPE)
        {
        }
    }

    public class MapSceneDrawPath : EventBase
    {
        public static string EVENT_TYPE = "MapSceneDrawPath";

        public MapSceneDrawPath(Vector3 p, float offset = 0.05f)
            : base(EVENT_TYPE)
        {
            Postion = p;
            Offset = offset;
        }

        public float Offset { get; set; }
        public Vector3 Postion { get; set; }
    }

    public class MapSceneMsgOperation : EventBase
    {
        public static string EVENT_TYPE = "MapSceneMsgOperation";

        public MapSceneMsgOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class MapSceneClickCell : EventBase
    {
        public static string EVENT_TYPE = "MapSceneClickCell";

        public MapSceneClickCell(SceneNpcDataModel data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public SceneNpcDataModel Data { get; set; }
    }

    public class MapSceneClickLoction : EventBase
    {
        public static string EVENT_TYPE = "MapSceneClickLoction";

        public MapSceneClickLoction(Vector3 loc)
            : base(EVENT_TYPE)
        {
            Loction = loc;
        }

        public Vector3 Loction { get; set; }
    }

    public class MieShiMapSceneClickLoction : EventBase
    {
        public static string EVENT_TYPE = "MieShiMapSceneClickLoction";

        public MieShiMapSceneClickLoction(Vector3 loc)
            : base(EVENT_TYPE)
        {
            Loction = loc;
        }

        public Vector3 Loction { get; set; }
    }

    public class ArenaFightRecoardChange : EventBase
    {
        public static string EVENT_TYPE = "ArenaFightRecoardChange";

        public ArenaFightRecoardChange(P1vP1Change_One d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public P1vP1Change_One Data { get; set; }
    }

    public class AreanResultExitEvent : EventBase
    {
        public static string EVENT_TYPE = "AreanResultExitEvent";

        public AreanResultExitEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class AreanOppentCellClick : EventBase
    {
        public static string EVENT_TYPE = "AreanOppentCellClick";

        public AreanOppentCellClick(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class SatueOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "SatueOperateEvent";

        public SatueOperateEvent(int t, int i = -1)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class ArenaOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "ArenaOperateEvent";

        public ArenaOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    /// <summary>
    ///     显示荣誉界面
    /// </summary>
    public class UIEvent_RankList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RankList";

        public UIEvent_RankList(bool isShow)
            : base(EVENT_TYPE)
        {
            IsShow = isShow;
        }

        public bool IsShow { get; set; }
    }

//     /// <summary>
//     /// 显示荣誉界面
//     /// </summary>
//     public class UIEvent_ShowArena : EventBase
//     {
//         public static string EVENT_TYPE = "UIEvent_ShowArena";
//         public bool IsShow { get; set; }
// 
//         public UIEvent_ShowArena(bool isShow)
//             : base(EVENT_TYPE)
//         {
//             IsShow = isShow;
//         }
//     }
//     /// <summary>
//     /// 显示神像界面
//     /// </summary>
//     public class UIEvent_ShowStatue: EventBase
//     {
//         public static string EVENT_TYPE = "UIEvent_ShowStatue";
//         public bool IsShow { get; set; }
// 
//         public UIEvent_ShowStatue(bool isShow)
//             : base(EVENT_TYPE)
//         {
//             IsShow = isShow;
//         }
//     }
    /// <summary>
    ///     升级军衔
    /// </summary>
    public class UIEvent_Promotion_Rank : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_Promotion_Rank";

        public UIEvent_Promotion_Rank(bool isShow)
            : base(EVENT_TYPE)
        {
            IsShow = isShow;
        }

        public bool IsShow { get; set; }
    }

    /// <summary>
    ///     点击军衔
    /// </summary>
    public class UIEvent_OnClickRankBtn : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OnClickRankBtn";

        public UIEvent_OnClickRankBtn(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx { get; set; }
    }

    //public class FarmMenuCountRefresh : EventBase
    //{
    //    public static string EVENT_TYPE = "FarmMenuCountRefresh";
    //    public int Count { get; set; }

    //    public FarmMenuCountRefresh(int c)
    //        : base(EVENT_TYPE)
    //    {
    //        Count = c;
    //    }
    //}

    public class FramDragRefreshCount : EventBase
    {
        public static string EVENT_TYPE = "FramDragRefreshCount";

        public FramDragRefreshCount(int c, int i)
            : base(EVENT_TYPE)
        {
            Count = c;
            Index = i;
        }

        public int Count { get; set; }
        public int Index { get; set; }
    }

    public class FarmMatureRefresh : EventBase
    {
        public static string EVENT_TYPE = "FarmMatureRefresh";

        public FarmMatureRefresh(int s, int max)
            : base(EVENT_TYPE)
        {
            Scends = s;
            MaxTimer = max;
        }

        public int MaxTimer { get; set; }
        public int Scends { get; set; }
    }

    public class FarmMenuClickEvent : EventBase
    {
        public static string EVENT_TYPE = "FarmMenuClickEvent";

        public FarmMenuClickEvent(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class FarmMenuDragEvent : EventBase
    {
        public static string EVENT_TYPE = "FarmMenuDragEvent";

        public FarmMenuDragEvent(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class FarmLandCellClick : EventBase
    {
        public static string EVENT_TYPE = "FarmLandCellClick";

        public FarmLandCellClick(int i, bool drag)
            : base(EVENT_TYPE)
        {
            Index = i;
            IsDraging = drag;
        }

        public int Index { get; set; }
        public bool IsDraging { get; set; }
    }

    public class FarmOrderFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "FarmOrderFlyAnim";

        public FarmOrderFlyAnim()
            : base(EVENT_TYPE)
        {
        }
    }

    public class FarmCellTipEvent : EventBase
    {
        public static string EVENT_TYPE = "FarmCellTipEvent";

        public FarmCellTipEvent(OperateType type, int index, int id, int count)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
            PlantId = id;
            Count = count;
        }

        public int Count { get; set; }
        public int Exp { get; set; }
        public int Index { get; set; }
        public int PlantId { get; set; }
        public OperateType Type { get; set; }
    }

    public class FarmOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "FarmOperateEvent";

        public FarmOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class CommonEvent : EventBase
    {
        public CommonEvent(string eventName, string eventArg)
            : base(eventName)
        {
            EventName = eventName;
            EventArg = eventArg;
        }

        public string EventArg { get; set; }
        public string EventName { get; set; }
    }


    public class MainUiOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "MainUIOperateEvent";

        public MainUiOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class WingModelRefreh : EventBase
    {
        public static string EVENT_TYPE = "WingModelRefreh";

        public WingModelRefreh(int id)
            : base(EVENT_TYPE)
        {
            TableId = id;
        }

        public int TableId { get; set; }
    }

    public class WingRefreshTrainCount : EventBase
    {
        public static string EVENT_TYPE = "WingRefreshTrainCount";

        public WingRefreshTrainCount(int t)
            : base(EVENT_TYPE)
        {
            TrainCount = t;
        }

        public int TrainCount { get; set; }
    }

    public class WingRefreshStarCount : EventBase
    {
        public static string EVENT_TYPE = "WingRefreshStarCount";

        public WingRefreshStarCount(int s)
            : base(EVENT_TYPE)
        {
            Star = s;
        }

        public int Star { get; set; }
    }

    public class WingNotifyLogicEvent : EventBase
    {
        public static string EVENT_TYPE = "WingNotifyLogicEvent";

        public WingNotifyLogicEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class WingRefreshStarPage : EventBase
    {
        public static string EVENT_TYPE = "WingRefreshStarPage";

        public WingRefreshStarPage(int l, int s, int p, bool b)
            : base(EVENT_TYPE)
        {
            Layer = l;
            Star = s;
            Part = p;
            ShowBegin = b;
        }

        public int Layer { get; set; }
        public int Part { get; set; }
        public bool ShowBegin { get; set; }
        public int Star { get; set; }
    }

    public class WingQuailtyCellClick : EventBase
    {
        public static string EVENT_TYPE = "WingQuailtyCellClick";

        public WingQuailtyCellClick(WingQualityData d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public WingQualityData Data { get; set; }
    }

    public class WingOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "WingOperateEvent";

        public WingOperateEvent(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class DungeonNetRetCallBack : EventBase
    {
        public static string EVENT_TYPE = "DungeonNetRetCallBack";

        public DungeonNetRetCallBack(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class DungeonSweepRandAward : EventBase
    {
        public static string EVENT_TYPE = "DungeonSweepRandAward";

        public DungeonSweepRandAward(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class DungeonInfosMainInfo : EventBase
    {
        public static string EVENT_TYPE = "DungeonSelectMainInfo";

        public DungeonInfosMainInfo(int i, eDungeonType t)
            : base(EVENT_TYPE)
        {
            Index = i;
            Type = t;
        }

        public int Index { get; set; }
        public eDungeonType Type { get; set; }
    }

    public class DungeonBtnClick : EventBase
    {
        public static string EVENT_TYPE = "DungeonBtnClick";

        public DungeonBtnClick(int i, eDungeonType t, int d = -1)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
            ExData = d;
        }

        public int ExData { get; set; }
        public int Index { get; set; }
        public eDungeonType Type { get; set; }
    }

    public class DungeonSetScan : EventBase
    {
        public static string EVENT_TYPE = "DungeonSetScan";

        public DungeonSetScan(int showScan)
            : base(EVENT_TYPE)
        {
            ShowScan = showScan;
        }

        public int ShowScan { get; set; }
    }


    public class DungeonGroupCellClick : EventBase
    {
        public static string EVENT_TYPE = "DungeonGroupCellClick";

        public DungeonGroupCellClick(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class DungeonGroupCellClick2 : EventBase
    {
        public static string EVENT_TYPE = "DungeonGroupCellClick2";

        public DungeonGroupCellClick2(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class ElfReplaceEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfReplaceEvent";

        public ElfReplaceEvent(int f, int t)
            : base(EVENT_TYPE)
        {
            From = f;
            To = t;
        }

        public int From { get; set; }
        public int To { get; set; }
    }

    public class UIEvent_ElfSetGridLookIndex : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ElfSetGridLookIndex";

        public UIEvent_ElfSetGridLookIndex(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }


    public class ElfModelRefreshEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfModelRefreshEvent";

        public ElfModelRefreshEvent(int id, int o = 0)
            : base(EVENT_TYPE)
        {
            CharId = id;
            Offset = o;
        }

        public int CharId { get; set; }
        public int Offset { get; set; }
    }

    public class FormationElfModelRefreshEvent : EventBase
    {
        public static string EVENT_TYPE = "FormationElfModelRefreshEvent";

        public FormationElfModelRefreshEvent(int idx)
            : base(EVENT_TYPE)
        {
            Index = idx;
        }

        public int Index { get; set; }
    }

    public class ElfOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfOperateEvent";

        public ElfOperateEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }


    public class ElfGetOneShowEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfGetOneShowEvent";

        public ElfGetOneShowEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ElfShowCloseEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfShowCloseEvent";

        public ElfShowCloseEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ElfOneDrawInfoEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfOneDrawInfoEvent";

        public ElfOneDrawInfoEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ElfGetDrawResultBack : EventBase
    {
        public static string EVENT_TYPE = "ElfGetDrawResultBack";

        public ElfGetDrawResultBack(int type)
            : base(EVENT_TYPE)
        {
            DrawType = type;
        }

        public int DrawType { get; set; }
    }

    public class ElfPlayAnimationEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfPlayAnimationEvent";

        public ElfPlayAnimationEvent(int type, bool isInverse, bool isInstant)
            : base(EVENT_TYPE)
        {
            Type = type;
            IsForward = isInverse;
            IsInstant = isInstant;
        }

        public bool IsForward { get; set; }
        public bool IsInstant { get; set; }
        public int Type { get; set; }
    }

    public class ElfGetDrawResult : EventBase
    {
        public static string EVENT_TYPE = "ElfGetDrawResult";

        public ElfGetDrawResult()
            : base(EVENT_TYPE)
        {
        }

        public DrawItemResult DrawItems { get; set; }
        public long DrawTime { get; set; }
    }

    public class NameChange : EventBase
    {
        public static string EVENT_TYPE = "NameChange";

        public NameChange(bool idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public bool Idx { get; set; }
    }

    public class ElfCellClickEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfCellClickEvent";

        public ElfCellClickEvent(ElfItemDataModel data, int index)
            : base(EVENT_TYPE)
        {
            DataModel = data;
            Index = index;
        }

        public ElfItemDataModel DataModel { get; set; }
        public int Index { get; set; }
    }

    public class ElfCell1ClickEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfCell1ClickEvent";

        public ElfCell1ClickEvent(ElfItemDataModel data, GameObject go)
            : base(EVENT_TYPE)
        {
            DataModel = data;
            Go = go;
        }

        public GameObject Go;
        public ElfItemDataModel DataModel { get; set; }
    }

    public class ElfFlyEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfFlyEvent";

        public ElfFlyEvent(int fromIdx, int toIdx, bool needOverEvent = false)
            : base(EVENT_TYPE)
        {
            FromIdx = fromIdx;
            ToIdx = toIdx;
            NeedOverEvent = needOverEvent;
        }

        public int FromIdx { get; set; }
        public bool NeedOverEvent { get; set; }
        public int ToIdx { get; set; }
    }

    public class FormationLevelupEvent : EventBase
    {
        public static string EVENT_TYPE = "FormationLevelupEvent";

        public FormationLevelupEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ElfLevelupEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfLevelupEvent";

        public ElfLevelupEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ElfFlyOverEvent : EventBase
    {
        public static string EVENT_TYPE = "ElfFlyOverEvent";

        public ElfFlyOverEvent(int fromIdx, int toIdx)
            : base(EVENT_TYPE)
        {
            FromIdx = fromIdx;
            ToIdx = toIdx;
        }

        public int FromIdx { get; set; }
        public int ToIdx { get; set; }
    }

    public class NotifyDungeonTime : EventBase
    {
        public static string EVENT_TYPE = "NotifyDungeonTime";

        public NotifyDungeonTime(long ct)
            : base(EVENT_TYPE)
        {
            CloseTime = ct;
        }

        public long CloseTime { get; set; }
    }

    public class BagDataInitEvent : EventBase
    {
        public static string EVENT_TYPE = "BagDataInitEvent";

        public BagDataInitEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class EquipDurableChange : EventBase
    {
        public static string EVENT_TYPE = "EquipDurableChange";

        public EquipDurableChange(int state)
            : base(EVENT_TYPE)
        {
            State = state;
        }

        public int State { get; set; }
    }

    public class StoreOperaEvent : EventBase
    {
        public static string EVENT_TYPE = "StoreOperaEvent";

        public StoreOperaEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class StoreCellClick : EventBase
    {
        public static string EVENT_TYPE = "StoreCellClick";

        public StoreCellClick(StoreCellData data)
            : base(EVENT_TYPE)
        {
            CellData = data;
        }

        public StoreCellData CellData { get; set; }
    }

    public class MailOperactionEvent : EventBase
    {
        public static string EVENT_TYPE = "MailOperactionEvent";

        public MailOperactionEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class MailCellClickEvent : EventBase
    {
        public static string EVENT_TYPE = "MailCellClickEvent";

        public MailCellClickEvent(int i, int t, int v = -1)
            : base(EVENT_TYPE)
        {
            Index = i;
            Type = t;
            Value = v;
        }

        public int Index { get; set; }
        public int Type { get; set; }
        public int Value { get; set; }
    }

    public class MailSyncEvent : EventBase
    {
        public static string EVENT_TYPE = "MailSyncEvent";

        public MailSyncEvent(MailList l)
            : base(EVENT_TYPE)
        {
            List = l;
        }

        public MailList List { get; set; }
    }

    public class RankNotifyLogic : EventBase
    {
        public static string EVENT_TYPE = "RankNotifyLogic";

        public RankNotifyLogic(int t, int i = -1)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class RankOperationEvent : EventBase
    {
        public static string EVENT_TYPE = "RankOperationEvent";

        public RankOperationEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class PlayerInfoOperation : EventBase
    {
        public static string EVENT_TYPE = "PlayerInfoOperation";

        public PlayerInfoOperation(int type, int index = -1)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class PlayerInfoRefreshModelView : EventBase
    {
        public static string EVENT_TYPE = "PlayerInfoRefreshModelView";

        public PlayerInfoRefreshModelView(int type, Dictionary<int, int> equips, int elfId)
            : base(EVENT_TYPE)
        {
            Type = type;
            EquipModels = equips;
            ElfId = elfId;
        }

        public int ElfId { get; set; }
        public Dictionary<int, int> EquipModels { get; set; }
        public int Type { get; set; }
    }

    public class RankRefreshModelView : EventBase
    {
        public static string EVENT_TYPE = "RankRefreshModelView";

        public RankRefreshModelView(PlayerInfoMsg info, bool iselfrank)
            : base(EVENT_TYPE)
        {
            Info = info;
            Iselfrank = iselfrank;
        }

        public PlayerInfoMsg Info { get; set; }
        public bool Iselfrank { get; set; }
    }

    public class RankCellClick : EventBase
    {
        public static string EVENT_TYPE = "RankCellClick";

        public RankCellClick(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class DungeonFightOver : EventBase
    {
        public static string EVENT_TYPE = "DungeonFightOver";

        public DungeonFightOver()
            : base(EVENT_TYPE)
        {
        }
    }

    public class SkillReleaseNetBack : EventBase
    {
        public static string EVENT_TYPE = "SkillReleaseNetBack";

        public SkillReleaseNetBack(int id, bool ok)
            : base(EVENT_TYPE)
        {
            SkillId = id;
            IsOk = ok;
        }

        public bool IsOk { get; set; }
        public int SkillId { get; set; }
    }

    public class ComposeItemOnClick : EventBase
    {
        public static string EVENT_TYPE = "ComposeItemOnClick";

        public ComposeItemOnClick()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ComposeItemEffectEvent : EventBase
    {
        public static string EVENT_TYPE = "ComposeItemEffectEvent";

        public ComposeItemEffectEvent(bool isSuccess)
            : base(EVENT_TYPE)
        {
            IsSuccess = isSuccess;
        }

        public bool IsSuccess;
    }

    public class ComposeMenuCellClick : EventBase
    {
        public static string EVENT_TYPE = "ComposeMenuCellClick";

        public ComposeMenuCellClick(ComposeMenuDataModel data)
            : base(EVENT_TYPE)
        {
            MenuData = data;
        }

        public ComposeMenuDataModel MenuData;
    }

    public class BossCellClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "BossBtnClickedEvent";

        public BossCellClickedEvent(BtnState state)
            : base(EVENT_TYPE)
        {
            BtnState = state;
        }

        public BtnState BtnState;
    }

    public class SmithyCellClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "SmithyCellClickedEvent";

        public SmithyCellClickedEvent(CastMenuDataModel data)
            : base(EVENT_TYPE)
        {
            MenuItemData = data;
        }

        public CastMenuDataModel MenuItemData;
    }

    public class SmithyFurnaceCellEvent : EventBase
    {
        public static string EVENT_TYPE = "SmithyFurnaceCellEvent";

        public SmithyFurnaceCellEvent()
            : base(EVENT_TYPE)
        {
        }

        public int Index;
        public int Type;
    }

    public class SmithyOnPlayTweenAnim : EventBase
    {
        public static string EVENT_TYPE = "SmithyOnPlayTweenAnim";

        public SmithyOnPlayTweenAnim(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index;
    }


    public class AttributePointOperate : EventBase
    {
        public static string EVENT_TYPE = "AttributePointOperate";

        public AttributePointOperate(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class FlagInitEvent : EventBase
    {
        public static string EVENT_TYPE = "FlagInitEvent";

        public FlagInitEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ExData64InitEvent : EventBase
    {
        public static string EVENT_TYPE = "ExData64InitEvent";

        public ExData64InitEvent(List<int> data)
            : base(EVENT_TYPE)
        {
            ExtData = data;
        }

        public List<int> ExtData { get; set; }
    }

    public class ExData64UpDataEvent : EventBase
    {
        public static string EVENT_TYPE = "ExData64UpDataEvent";

        public ExData64UpDataEvent(int k, long v)
            : base(EVENT_TYPE)
        {
            Key = k;
            Value = v;
        }

        public int Key { get; set; }
        public long Value { get; set; }
    }

    public class ExDataInitEvent : EventBase
    {
        public static string EVENT_TYPE = "ExDataInitEvent";

        public ExDataInitEvent(List<int> data)
            : base(EVENT_TYPE)
        {
            ExtData = data;
        }

        public List<int> ExtData { get; set; }
    }

    public class ExDataUpDataEvent : EventBase
    {
        public static string EVENT_TYPE = "ExDataUpDataEvent";

        public ExDataUpDataEvent(int k, int v)
            : base(EVENT_TYPE)
        {
            Key = k;
            Value = v;
        }

        public int Key { get; set; }
        public int Value { get; set; }
    }

    public class FriendUpdateSyncEvent : EventBase
    {
        public static string EVENT_TYPE = "FriendUpdateSyncEvent";

        public FriendUpdateSyncEvent(CharacterSimpleDataList d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public CharacterSimpleDataList Data { get; set; }
    }

    public class FriendAddSyncEvent : EventBase
    {
        public static string EVENT_TYPE = "FriendAddSyncEvent";

        public FriendAddSyncEvent(int t, CharacterSimpleData d)
            : base(EVENT_TYPE)
        {
            Type = t;
            Data = d;
        }

        public CharacterSimpleData Data { get; set; }
        public int Type { get; set; }
    }

    public class FriendDelSyncEvent : EventBase
    {
        public static string EVENT_TYPE = "FriendDelSyncEvent";

        public FriendDelSyncEvent(int t, ulong id)
            : base(EVENT_TYPE)
        {
            Type = t;
            CharacterId = id;
        }

        public ulong CharacterId { get; set; }
        public int Type { get; set; }
    }

    public class FriendClickType : EventBase
    {
        public static string EVENT_TYPE = "FriendClickType";

        public FriendClickType(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class FriendOperationEvent : EventBase
    {
        public static string EVENT_TYPE = "FriendOperationEvent";

        public FriendOperationEvent(int ft, int ot, string n = "", ulong id = 0)
            : base(EVENT_TYPE)
        {
            FriendType = ft;
            OperationType = ot;
            Name = n;
            Id = id;
        }

        public int FriendType { get; set; }
        public ulong Id { get; set; }
        public string Name { get; set; }
        public int OperationType { get; set; }
    }

    public class FriendClickShowInfo : EventBase
    {
        public static string EVENT_TYPE = "FriendClickShowInfo";

        public FriendClickShowInfo(FriendInfoDataModel d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public FriendInfoDataModel Data { get; set; }
    }

    public class FriendContactCell : EventBase
    {
        public static string EVENT_TYPE = "FriendContactCell";

        public FriendContactCell(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class FriendContactClickAddFriend : EventBase
    {
        public static string EVENT_TYPE = "FriendContactClickAddFriend";

        public FriendContactClickAddFriend(FriendInfoDataModel d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public FriendInfoDataModel Data { get; set; }
    }

    public class FriendNotify : EventBase
    {
        public static string EVENT_TYPE = "FriendNotify";

        public FriendNotify(int i)
            : base(EVENT_TYPE)
        {
            Type = i;
        }

        public int Type { get; set; }
    }

    public class FriendReceive : EventBase
    {
        public static string EVENT_TYPE = "FriendReceive";

        public FriendReceive(CharacterSimpleDataList d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public CharacterSimpleDataList Data { get; set; }
    }


    public class FriendSeekBtnClick : EventBase
    {
        public static string EVENT_TYPE = "FriendSeekBtnClick";

        public FriendSeekBtnClick(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ChatVoiceContent : EventBase
    {
        public static string EVENT_TYPE = "ChatVoiceContent";

        public ChatVoiceContent(byte[] data, string c)
            : base(EVENT_TYPE)
        {
            SoundData = data;
            Content = c;
        }

        public string Content { get; set; }
        public byte[] SoundData { get; set; }
    }

    public class ChatMainRefreshContent : EventBase
    {
        public static string EVENT_TYPE = "ChatMainRefreshContent";

        public ChatMainRefreshContent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ChatWordCountChage : EventBase
    {
        public static string EVENT_TYPE = "ChatWordCountChage";

        public ChatWordCountChage(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ChatVoiceContentRefresh : EventBase
    {
        public static string EVENT_TYPE = "ChatVoiceContentRefresh";

        public ChatVoiceContentRefresh(UIWidget w)
            : base(EVENT_TYPE)
        {
            Widget = w;
        }

        public UIWidget Widget { get; set; }
    }

    public class ChatTrumpetWordCountCheck : EventBase
    {
        public static string EVENT_TYPE = "ChatTrumpetWordCountCheck";

        public ChatTrumpetWordCountCheck(int c)
            : base(EVENT_TYPE)
        {
            Count = c;
        }

        public int Count { get; set; }
    }

    public class ChatMainNewTrumpet : EventBase
    {
        public static string EVENT_TYPE = "ChatMainNewTrumpet";

        public ChatMainNewTrumpet()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ChatTrumpetVisibleChange : EventBase
    {
        public static string EVENT_TYPE = "ChatTrumpetVisibleChange";

        public ChatTrumpetVisibleChange(bool vis)
            : base(EVENT_TYPE)
        {
            IsVisible = vis;
        }

        public bool IsVisible { get; set; }
    }

    public class ChatMainSendBtnClick : EventBase
    {
        public static string EVENT_TYPE = "ChatMainSendBtnClick";

        public ChatMainSendBtnClick(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ChatMainSendVoiceData : EventBase
    {
        public static string EVENT_TYPE = "ChatMainSendVoiceData";

        public ChatMainSendVoiceData(byte[] data, float length, bool whisper)
            : base(EVENT_TYPE)
        {
            VoiceData = data;
            VoiceLength = length;
            IsWhisper = whisper;
        }

        public bool IsWhisper { get; set; }
        public byte[] VoiceData { get; set; }
        public float VoiceLength { get; set; }
    }

    public class ChatMainPlayVoice : EventBase
    {
        public static string EVENT_TYPE = "ChatMainPlayVoice";

        public ChatMainPlayVoice(byte[] datas)
            : base(EVENT_TYPE)
        {
            SoundData = datas;
        }

        public byte[] SoundData { get; set; }
    }

    public class ChatMainSpeechRecognized : EventBase
    {
        public static string EVENT_TYPE = "ChatMainSpeechRecognized";

        public ChatMainSpeechRecognized(string content)
            : base(EVENT_TYPE)
        {
            Content = content;
        }

        public string Content { get; set; }
    }


    public class ChatMainStopVoiceAnimation : EventBase
    {
        public static string EVENT_TYPE = "ChatMainStopVoiceAnimation";

        public ChatMainStopVoiceAnimation()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ChatMainPrivateChar : EventBase
    {
        public static string EVENT_TYPE = "ChatMainPrivateChar";

        public ChatMainPrivateChar(OperationListData d)
            : base(EVENT_TYPE)
        {
            Data = d;
        }

        public OperationListData Data { get; set; }
    }

    public class ChatMainFaceAdd : EventBase
    {
        public static string EVENT_TYPE = "ChatMainFaceAdd";

        public ChatMainFaceAdd(int t, int f)
            : base(EVENT_TYPE)
        {
            Type = t;
            FaceId = f;
        }

        public int FaceId { get; set; }
        public int Type { get; set; }
    }

    public class ChatMainChangeChannel : EventBase
    {
        public static string EVENT_TYPE = "ChatMainChangeChannel";

        public ChatMainChangeChannel(int c)
            : base(EVENT_TYPE)
        {
            Channel = c;
        }

        public int Channel { get; set; }
    }

    public class ItemInfoNotifyEvent : EventBase
    {
        public static string EVENT_TYPE = "ItemInfoNotifyEvent";

        public ItemInfoNotifyEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ItemInfoOperate : EventBase
    {
        public static string EVENT_TYPE = "ItemInfoOperate";

        public ItemInfoOperate(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class UIEvent_EquipCompareBtnClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompareBtnClick";

        public UIEvent_EquipCompareBtnClick(int type)
            : base(EVENT_TYPE)
        {
            BtnType = type;
        }

        public int BtnType { get; set; }
    }

    public class EquipOperateClick : EventBase
    {
        public static string EVENT_TYPE = "EquipOperateClick";

        public EquipOperateClick(int type)
            : base(EVENT_TYPE)
        {
            OperateType = type;
        }

        public int Index { get; set; }
        public int OperateType { get; set; }
    }

    public class EquipCellSelect : EventBase
    {
        public static string EVENT_TYPE = "EquipCellListSelect";

        public EquipCellSelect(BagItemDataModel data, int idx = -1)
            : base(EVENT_TYPE)
        {
            ItemData = data;
            Index = idx;
        }

        public int Index { get; set; }
        public BagItemDataModel ItemData { get; set; }
    }

    public class EquipCellSwap : EventBase
    {
        public static string EVENT_TYPE = "EquipCellSwap";

        public EquipCellSwap(int fromIdx, int toIdx)
            : base(EVENT_TYPE)
        {
            FromIdx = fromIdx;
            ToIdx = toIdx;
        }

        public int FromIdx;
        public int ToIdx;
    }

    public class Attr_Change_Event : EventBase
    {
        public static string EVENT_TYPE = "Attr_Change_Event";

        public Attr_Change_Event(eAttributeType type, int oldValue, int newValue)
            : base(EVENT_TYPE)
        {
            Type = type;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int NewValue { get; set; }
        public int OldValue { get; set; }
        public eAttributeType Type { get; set; }
    }

    public class AttrUIReflesh_Event : EventBase
    {
        public static string EVENT_TYPE = "AttrUIReflesh_Event";

        public AttrUIReflesh_Event(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class OpenAdvancedProperty_Event : EventBase
    {
        public static string EVENT_TYPE = "OpenAdvancedProperty_Event";

        public OpenAdvancedProperty_Event(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class CloseAdvancedProperty_Event : EventBase
    {
        public static string EVENT_TYPE = "CloseAdvancedProperty_Event";

        public CloseAdvancedProperty_Event(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class Resource_Change_Event : EventBase
    {
        public static string EVENT_TYPE = "Resource_Change_Event";

        public Resource_Change_Event(eResourcesType type, int oldValue, int newValue)
            : base(EVENT_TYPE)
        {
            Type = type;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int NewValue { get; set; }
        public int OldValue { get; set; }
        public eResourcesType Type { get; set; }
    }

    public class Show_UI_Event : EventBase
    {
        public static string EVENT_TYPE = "Show_UI_Event";

        public Show_UI_Event(UIConfig c, UIInitArguments args = null)
            : base(EVENT_TYPE)
        {
            config = c;
            Args = args;
        }

        public UIConfig config;
        public UIInitArguments Args { get; set; }
    }

    public class Close_UI_Event : EventBase
    {
        public static string EVENT_TYPE = "Close_UI_Event";

        public Close_UI_Event(UIConfig c, bool isBack = true)
            : base(EVENT_TYPE)
        {
            config = c;
            IsBack = isBack;
        }

        public UIConfig config;
        public bool IsBack { get; set; }
    }

    public class TakeScreenShotAndOpenShareFrame : EventBase
    {
        public static string EVENT_TYPE = "TakeScreenShotAndOpenShareFrame";

        public TakeScreenShotAndOpenShareFrame()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UI_EVENT_ShareBtnShow : EventBase
    {
        public static string EVENT_TYPE = "UI_EVENT_ShareBtnShow";

        public UI_EVENT_ShareBtnShow(bool bShow)
            : base(EVENT_TYPE)
        {
            isVisible = bShow;
        }

        public bool isVisible { get; set; }
    }

    public class MissionTrackUpdateTimerEvent : EventBase
    {
        public static string EVENT_TYPE = "MissionTrackUpdateTimerEvent";

        public MissionTrackUpdateTimerEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UI_Event_OffLineExp : EventBase
    {
        public static string EVENT_TYPE = "UI_Event_OffLineExp";

        public UI_Event_OffLineExp(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type;
    }

    public class UI_Event_OffLineFrameEnable : EventBase
    {
        public static string EVENT_TYPE = "UI_Event_OffLineFrameEnable";

        public UI_Event_OffLineFrameEnable()
            : base(EVENT_TYPE)
        {
        }
    }

    //开始练功
    public class UI_Event_IsExercising : EventBase
    {
        public static string EVENT_TYPE = "UI_Event_IsExercising";

        public UI_Event_IsExercising()
            : base(EVENT_TYPE)
        {
        }
    }

//            public class UI_Event_OffLineFrameDisable : EventBase
//     {
//         public static string EVENT_TYPE = "UI_Event_OffLineFrameDisable";
// 
//         public UI_Event_OffLineFrameDisable()
//             : base(EVENT_TYPE)
//         {
// 
//         }
//     }
//     
    public class UiEventChangeOutLineTime : EventBase
    {
        public static string EVENT_TYPE = "UI_Event_ChangeOutLineTime";

        public UiEventChangeOutLineTime()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ShowUIHintBoard : EventBase
    {
        public static string EVENT_TYPE = "ShowUIHintBoard";

        public ShowUIHintBoard(string info, int id = 14, int waitSec = -1)
            : base(EVENT_TYPE)
        {
            Info = info;
            TableId = id;
            WaitSec = waitSec;
            DicId = 0;
        }

        public ShowUIHintBoard(int dicid, int textid = 14)
            : base(EVENT_TYPE)
        {
            Info = GameUtils.GetDictionaryText(dicid);
            TableId = textid;
            DicId = dicid;
        }

        public string Info { get; set; }
        public int TableId { get; set; }
        public int WaitSec { get; set; }
        public int DicId { get; set; }
    }

    public class ShowDamageBoardEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowDamageBoardEvent";

        public ShowDamageBoardEvent(Vector3 pos, BuffResult result)
            : base(EVENT_TYPE)
        {
            Position = pos;
            Result = result;
        }

        public Vector3 Position { get; set; }
        public BuffResult Result { get; set; }
    }

    public class RefresSceneMap : EventBase
    {
        public static string EVENT_TYPE = "RefresSceneMap";

        public RefresSceneMap(int sceneId)
            : base(EVENT_TYPE)
        {
            SceneId = sceneId;
        }

        public int SceneId { get; set; }
    }

    public class ScenePlayerInfoEvent : EventBase
    {
        public static string EVENT_TYPE = "ScenePlayerInfoEvent";

        public ScenePlayerInfoEvent(ScenePlayerInfos info)
            : base(EVENT_TYPE)
        {
            Info = info;
        }

        public ScenePlayerInfos Info { get; set; }
    }

    public class Enter_Scene_Event : EventBase
    {
        public static string EVENT_TYPE = "Enter_Scene_Event";

        public Enter_Scene_Event(int sceneId)
            : base(EVENT_TYPE)
        {
            SceneId = sceneId;
        }

        public int SceneId { get; set; }
    }

    public class Hide_MapName_Event : EventBase
    {
        public static string EVENT_TYPE = "Hide_MapName_Event";

        public Hide_MapName_Event()
            : base(EVENT_TYPE)
        {
        }
    }

    public class LoadSceneOverEvent : EventBase
    {
        public static string EVENT_TYPE = "LoadSceneOver_Event";

        public LoadSceneOverEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Postion_Change_Event : EventBase
    {
        public static string EVENT_TYPE = "Postion_Change_Event ";

        public Postion_Change_Event(Vector3 loc)
            : base(EVENT_TYPE)
        {
            Loction = loc;
        }

        public Vector3 Loction { get; set; }
    }

    public class Character_Create_Event : EventBase
    {
        public static string EVENT_TYPE = "Character_Create_Event";

        public Character_Create_Event(ulong obj)
            : base(EVENT_TYPE)
        {
            CharacterId = obj;
        }

        public ulong CharacterId;
    }

    public class Character_Remove_Event : EventBase
    {
        public static string EVENT_TYPE = "Character_Remove_Event ";

        public Character_Remove_Event(ulong obj)
            : base(EVENT_TYPE)
        {
            CharacterId = obj;
        }

        public ulong CharacterId;
    }

    public class SkillSelectTargetEvent : EventBase
    {
        public static string EVENT_TYPE = "SkillSelectTargetEvent";

        public SkillSelectTargetEvent(ObjCharacter obj, int type)
            : base(EVENT_TYPE)
        {
            Target = obj;
            Type = type;
        }

        public ObjCharacter Target { get; set; }
        public int Type { get; set; }
    }

    public class ShowMissionProgressEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowMissionProgressEvent ";

        public ShowMissionProgressEvent(string label)
            : base(EVENT_TYPE)
        {
            ProgressName = label;
        }

        public string ProgressName { get; set; }
    }

    public class UpdateMissionProgressEvent : EventBase
    {
        public static string EVENT_TYPE = "UpdateMissionProgressEvent ";

        public UpdateMissionProgressEvent(float percent)
            : base(EVENT_TYPE)
        {
            Percent = percent;
        }

        public float Percent { get; set; }
    }

    public class MainUI_OnClickSwitch : EventBase
    {
        public static string EVENT_TYPE = "MainUI_OnClickSwitch ";

        public MainUI_OnClickSwitch(bool isShowSKill)
            : base(EVENT_TYPE)
        {
            IsShowSKill = isShowSKill;
        }

        public bool IsShowSKill { get; set; }
    }

    public class HideMissionProgressEvent : EventBase
    {
        public static string EVENT_TYPE = "HideMissionProgressEvent";

        public HideMissionProgressEvent(int id)
            : base(EVENT_TYPE)
        {
            MissionId = id;
        }

        public int MissionId { get; set; }
    }

    #region BackPack

    public class PackUnlockOperate : EventBase
    {
        public static string EVENT_TYPE = "PackUnlockOperate";

        public PackUnlockOperate(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class SetBagFreeIconEvent : EventBase
    {
        public static string EVENT_TYPE = "SetBagFreeIconEvent";

        public SetBagFreeIconEvent()
            : base(EVENT_TYPE)
        {
        }
    }


    public class PackUnlockUIEvent : EventBase
    {
        public static string EVENT_TYPE = "PackUnlockUIEvent";

        public PackUnlockUIEvent(int count, int cost, DateTime time, bool refresh)
            : base(EVENT_TYPE)
        {
            Count = count;
            Cost = cost;
            Time = time;
            IsRefresh = refresh;
        }

        public int Cost { get; set; }
        public int Count { get; set; }
        public bool IsRefresh { get; set; }
        public DateTime Time { get; set; }
    }

    public class PackUnlockEvent : EventBase
    {
        public static string EVENT_TYPE = "PackUnlockEvent";

        public PackUnlockEvent(BagItemDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public BagItemDataModel DataModel { get; set; }
    }


    public class PackTradingSellPage : EventBase
    {
        public static string EVENT_TYPE = "PackTradingSellPage";

        public PackTradingSellPage(int index, int bagPage)
            : base(EVENT_TYPE)
        {
            Index = index;
            BagPage = bagPage;
        }

        public int BagPage { get; set; }
        public int Index { get; set; }
    }

    public class StoreCacheTriggerEvent : EventBase
    {
        public static string EVENT_TYPE = "StoreCacheTriggerEvent";

        public StoreCacheTriggerEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ShowPackPageEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowPackPageEvent";

        public ShowPackPageEvent()
            : base(EVENT_TYPE)
        {
        }

        public int PackPage { get; set; }
    }

    public class PackArrangeEventUi : EventBase
    {
        public static string EVENT_TYPE = "PackArrangeEventUi";

        public PackArrangeEventUi(int packId)
            : base(EVENT_TYPE)
        {
            PackId = packId;
        }

        public int PackId { get; set; }
    }

    public class PackCapacityEventUi : EventBase
    {
        public static string EVENT_TYPE = "PackCapacityEventUi";

        public PackCapacityEventUi(int t)
            : base(EVENT_TYPE)
        {
            BagType = t;
        }

        public int BagType { get; set; }
    }

    #endregion

    public class PackItemClickEvent : Event
    {
        public static string EVENT_TYPE = "PackItemClickEvent";

        public PackItemClickEvent()
            : base(EVENT_TYPE)
        {
        }

        public int BagId { get; set; }
        public int Index { get; set; }
        public int TableId { get; set; }
    }

    public class EquipEnchanceEvent : EventBase
    {
        public static string EVENT_TYPE = "EquipEnchanceEvent";

        public EquipEnchanceEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class CharacterEquipChange : EventBase
    {
        public static string EVENT_TYPE = "CharacterEquipChange";

        public CharacterEquipChange(ulong id, int part, int itemId)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
            Part = part;
            ItemId = itemId;
        }

        public ulong CharacterId { get; set; }
        public int ItemId { get; set; }
        public int Part { get; set; }
    }

    public class MyEquipChangedEvent : EventBase
    {
        public static string EVENT_TYPE = "MyEquipChangedEvent";

        public MyEquipChangedEvent(int part, int item)
            : base(EVENT_TYPE)
        {
            Part = part;
            Item = item;
        }

        public int Item { get; set; }
        public int Part { get; set; }
    }


    //附魔按钮事件
    public class UIEvent_EquipFrame_Enchant : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipFrame_Enchant";

        public UIEvent_EquipFrame_Enchant()
            : base(EVENT_TYPE)
        {
        }
    }


    public class UIEvent_SelectEquip_OK : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectEquip_OK";

        public UIEvent_SelectEquip_OK()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SelectEquip_Cancel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectEquip_Cancel";

        public UIEvent_SelectEquip_Cancel()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SelectEquips_SelectIndex : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectEquips_SelectIndex";

        public UIEvent_SelectEquips_SelectIndex(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_EquipFrame_EnchanceItem : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipFrame_EnchanceItem";

        public UIEvent_EquipFrame_EnchanceItem(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_EquipFrame_EnchantItem : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipFrame_EnchantItem";

        public UIEvent_EquipFrame_EnchantItem(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_SelectRole_Enter : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectRole_Enter";

        public UIEvent_SelectRole_Enter()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SelectCharacter_ShowUIAnimation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectCharacter_ShowUIAnimation";
        public int Direction { get; set; }
        public SelectCharacterLogic.StateType StateType { get; set; }
        public int Index { get; set;}
        public UIEvent_SelectCharacter_ShowUIAnimation(int direction, int index, SelectCharacterLogic.StateType stateType)
            : base(EVENT_TYPE)
        {
            Direction = direction;
            Index = index;
            StateType = stateType;
        }
    }

    public class UIEvent_SelectRole_Back : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectRole_Back";

        public UIEvent_SelectRole_Back()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SelectRole_Index : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectRole_Index";

        public UIEvent_SelectRole_Index(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_ShowCreateRole : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowCreateRole";

        public UIEvent_ShowCreateRole(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_GetRandomName : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_GetRandomName";

        public UIEvent_GetRandomName()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_CreateRoleType_Change : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CreateRoleType_Change";

        public UIEvent_CreateRoleType_Change(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_CreateRole : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CreateRole";

        public UIEvent_CreateRole()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RefreshSelectRoleModel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RefreshSelectRoleModel";

        public UIEvent_RefreshSelectRoleModel(int i)
            : base(EVENT_TYPE)
        {
            index = i;
        }

        public int index { get; set; }
    }

    public class UIEvent_EquipCompare_Input : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompare_Input";

        public UIEvent_EquipCompare_Input()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_EquipCompare_Sell : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompare_Sell";

        public UIEvent_EquipCompare_Sell()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_EquipCompare_Close : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompare_Close";

        public UIEvent_EquipCompare_Close()
            : base(EVENT_TYPE)
        {
        }
    }


    public class UIEvent_EquipCompare_Share : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompare_Share";

        public UIEvent_EquipCompare_Share()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_EquipCompare_Use : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompare_Use";

        public UIEvent_EquipCompare_Use(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_EquipCompare_Reclaim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_EquipCompare_Reclaim";

        public UIEvent_EquipCompare_Reclaim()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SkillFrame_SkillSelect : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_SkillSelect";

        public UIEvent_SkillFrame_SkillSelect(SkillItemDataModel dm)
            : base(EVENT_TYPE)
        {
            DataModel = dm;
        }

        public SkillItemDataModel DataModel { get; set; }
    }

    public class UIEvent_SkillFrame_SkillLevelUpEffect : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_SkillLevelUpEffect";

        public UIEvent_SkillFrame_SkillLevelUpEffect(int id)
            : base(EVENT_TYPE)
        {
            skillId = id;
        }

        public int skillId { get; set; }
    }

    public class UIEvent_SkillFrame_SkillTalentChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_SkillTalentChange";

        public UIEvent_SkillFrame_SkillTalentChange()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SkillFrame_NetSyncTalentCount : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_NetSyncTalentCount";

        public UIEvent_SkillFrame_NetSyncTalentCount(int talentId, int value)
            : base(EVENT_TYPE)
        {
            TalentId = talentId;
            Value = value;
        }

        public int TalentId { get; set; }
        public int Value { get; set; }
    }

    public class UIEvent_SkillFrame_UpgradeSkill : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_UpgradeSkill";

        public UIEvent_SkillFrame_UpgradeSkill()
            : base(EVENT_TYPE)
        {
        }
    }


    public class UIEvent_SkillFrame_EquipSkill : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_EquipSkill";

        public UIEvent_SkillFrame_EquipSkill(int index, int skillId, bool syncToServer = true)
            : base(EVENT_TYPE)
        {
            Index = index;
            SkillId = skillId;
            BSyncToServer = syncToServer;
        }

        public bool BSyncToServer { get; set; }
        public int Index { get; set; }
        public int SkillId { get; set; }
    }

    public class UIEvent_SkillFrame_SwapEquipSkill : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_SwapEquipSkill";

        public UIEvent_SkillFrame_SwapEquipSkill(int fromIndex, int targetIndex)
            : base(EVENT_TYPE)
        {
            FromIndex = fromIndex;
            TargetIndex = targetIndex;
        }

        public int FromIndex { get; set; }
        public int TargetIndex { get; set; }
    }

    public class UIEvent_SkillFrame_UnEquipSkill : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_UnEquipSkill";

        public UIEvent_SkillFrame_UnEquipSkill(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }


    public class UIEvent_SkillFrame_OnDisable : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnDisable";

        public UIEvent_SkillFrame_OnDisable()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SkillFrame_AddTalentPoint : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_AddTalentPoint";

        public UIEvent_SkillFrame_AddTalentPoint()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SkillFrame_RefreshTalentPanel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_RefreshTalentPanel";

        public UIEvent_SkillFrame_RefreshTalentPanel()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SkillFrame_TalentBallClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_TalentBallClick";

        public UIEvent_SkillFrame_TalentBallClick(int id)
            : base(EVENT_TYPE)
        {
            TalentId = id;
        }

        public int TalentId { get; set; }
    }

    public class UIEvent_SkillFrame_AddSkillBoxDataModel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_AddSkillBoxDataModel";

        public UIEvent_SkillFrame_AddSkillBoxDataModel(SkillBoxDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public SkillBoxDataModel DataModel { get; set; }
    }

    public class UIEvent_SkillFrame_OnSkillBallOpen : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnSkillBallOpen";

        public UIEvent_SkillFrame_OnSkillBallOpen(SkillBoxDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public SkillBoxDataModel DataModel { get; set; }
    }

    public class UIEvent_SkillFrame_OnSkillBallPlayTween : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnSkillBallPlayTween";

        public UIEvent_SkillFrame_OnSkillBallPlayTween(GameObject data, bool foward)
            : base(EVENT_TYPE)
        {
            obj = data;
            bFoward = foward;
        }

        public bool bFoward { get; set; }
        public GameObject obj { get; set; }
    }

    public class UIEvent_SkillFrame_OnResetSkillTalent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnResetSkillTalent";

        public UIEvent_SkillFrame_OnResetSkillTalent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SkillFrame_OnSkillTalentSelected : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnSkillTalentSelected";

        public UIEvent_SkillFrame_OnSkillTalentSelected(int id)
            : base(EVENT_TYPE)
        {
            skillid = id;
        }

        public int skillid;
    }

    public class UIEvent_SkillFrame_OnResetTalent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnResetTalent";

        public UIEvent_SkillFrame_OnResetTalent()
            : base(EVENT_TYPE)
        {
        }
    }


    public class UIEvent_SkillFrame_OnSkillBallClose : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_OnSkillBallClose";

        public UIEvent_SkillFrame_OnSkillBallClose(SkillBoxDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public SkillBoxDataModel DataModel { get; set; }
    }

    public class UIEvent_SkillFrame_AddUnLearnedTalent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillFrame_AddUnLearnedTalent";

        public UIEvent_SkillFrame_AddUnLearnedTalent(TalentCellDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public TalentCellDataModel DataModel { get; set; }
    }

    public class UIEvent_TouchOrMouseRelease : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TouchOrMouseRelease";

        public UIEvent_TouchOrMouseRelease()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_ShowDownloadingSceneTipEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowDownloadingSceneTipEvent";

        public UIEvent_ShowDownloadingSceneTipEvent()
            : base(EVENT_TYPE)
        {
           
        }

    }

    public class UIEvent_SceneMap_AddSceneItemDataModel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SceneMap_AddSceneItemDataModel";

        public UIEvent_SceneMap_AddSceneItemDataModel(SceneItemDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public SceneItemDataModel DataModel { get; set; }
    }

    public class UIEvent_SceneMap_BtnTranfer : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SceneMap_BtnTranfer";

        public UIEvent_SceneMap_BtnTranfer(int id)
            : base(EVENT_TYPE)
        {
            SceneId = id;
        }

        public int SceneId { get; set; }
    }

    public class UIEvent_MainUIbtn_BtnTranfer : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SceneMap_BtnTranfer";

        public UIEvent_MainUIbtn_BtnTranfer(int id)
            : base(EVENT_TYPE)
        {
            SceneId = id;
        }

        public int SceneId { get; set; }
    }

    public class UIEvent_HandBookFrame_ShowAnimationBlocker : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_ShowAnimationBlocker";

        public UIEvent_HandBookFrame_ShowAnimationBlocker(bool bshow)
            : base(EVENT_TYPE)
        {
            bShow = bshow;
        }

        public bool bShow { get; set; }
    }


    public class UIEvent_HandBookFrame_ComposeBookPiece : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_ComposeBookPiece";

        public UIEvent_HandBookFrame_ComposeBookPiece(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_HandBookFrame_OnBookGroupToggled : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_OnBookGroupToggled";

        public UIEvent_HandBookFrame_OnBookGroupToggled(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_HandBookFrame_OnBountyGroupToggled : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_OnBountyGroupToggled";

        public UIEvent_HandBookFrame_OnBountyGroupToggled(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_HandBookFrame_ComposeBookPieceFromBookInfo : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_ComposeBookPieceFromBookInfo";

        public UIEvent_HandBookFrame_ComposeBookPieceFromBookInfo()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_HandBookFrame_RestScrollViewPostion : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_RestScrollViewPos";

        public UIEvent_HandBookFrame_RestScrollViewPostion()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_HandBookFrame_SetScrollViewLastPostion : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_SetScrollViewLastPostion";

        public UIEvent_HandBookFrame_SetScrollViewLastPostion(Vector3 pos, float off)
            : base(EVENT_TYPE)
        {
            postion = pos;
            offset = off;
        }

        public float offset;
        public Vector3 postion;
    }

    public class UIEvent_HandBookFrame_ComposeBookCardFromBookInfo : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_ComposeBookCardFromBookInfo";

        public UIEvent_HandBookFrame_ComposeBookCardFromBookInfo()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_HandBookFrame_OnBookClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_OnBookClick";

        public UIEvent_HandBookFrame_OnBookClick(HandBookItemDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public HandBookItemDataModel DataModel { get; set; }
    }

    public class UIEvent_HandBookFrame_OnGetBookClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_OnGetBookClick";

        public UIEvent_HandBookFrame_OnGetBookClick()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_HandBookFrame_OnGroupBookActive : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_OnGroupBookActive";

        public UIEvent_HandBookFrame_OnGroupBookActive(HandBookItemDataModel data, int idx)
            : base(EVENT_TYPE)
        {
            DataModel = data;
            index = idx;
        }

        public HandBookItemDataModel DataModel { get; set; }
        public int index { get; set; }
    }

    public class UIEvent_HandBookFrame_OnBountyBookActive : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HandBookFrame_OnBountyBookActive";

        public UIEvent_HandBookFrame_OnBountyBookActive(HandBookItemDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public HandBookItemDataModel DataModel { get; set; }
    }

    public class UIEvent_BuffListBtn : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_BuffListBtn";

        public UIEvent_BuffListBtn(int index, int data = -1)
            : base(EVENT_TYPE)
        {
            ButtonIndex = index;
            Data = data;
        }

        public int ButtonIndex { get; set; }
        public int Data { get; set; }
    }

    public class UIEvent_SyncBuffCell : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SyncBuffCell";

        public UIEvent_SyncBuffCell(BuffResult buff)
            : base(EVENT_TYPE)
        {
            Data = buff;
        }

        public BuffResult Data { get; set; }
    }

    public class UIEvent_ClearBuffList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ClearBuffList";

        public UIEvent_ClearBuffList()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RemoveBuffsOnDead : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RemoveBuffsOnDead";

        public UIEvent_RemoveBuffsOnDead()
            : base(EVENT_TYPE)
        {
        }
    }


    public class UIEvent_BuffIncreaseAnimation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_BuffIncreaseAnimation";

        public UIEvent_BuffIncreaseAnimation()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_TradingFrameButton : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TradingFrameButton";

        public UIEvent_TradingFrameButton(int index, int data = -1)
            : base(EVENT_TYPE)
        {
            ButtonIndex = index;
            Data = data;
        }

        public int ButtonIndex { get; set; }
        public int Data { get; set; }
    }

    public class UIEvent_TradingCoolDownChanged : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TradingCoolDownChanged";

        public UIEvent_TradingCoolDownChanged(TimeSpan cd, TimeSpan lasttime)
            : base(EVENT_TYPE)
        {
            CD = cd;
            LastTime = lasttime;
        }

        public TimeSpan CD { get; set; }
        public TimeSpan LastTime { get; set; }
    }

    public class UIEvent_TradingBagItemClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TradingBagItemClick";

        public UIEvent_TradingBagItemClick(BagItemDataModel item)
            : base(EVENT_TYPE)
        {
            BagItem = item;
        }

        public BagItemDataModel BagItem { get; set; }
    }

    public class UIEvent_TradingEquipTabPage : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TradingEquipTabPage";

        public UIEvent_TradingEquipTabPage(int page)
            : base(EVENT_TYPE)
        {
            Page = page;
        }

        public int Page { get; set; }
    }

    public class UIEvent_OnTradingItemSelled : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OnTradingItemSelled";

        public UIEvent_OnTradingItemSelled(long id)
            : base(EVENT_TYPE)
        {
            itemId = id;
        }

        public long itemId { get; set; }
    }

    public class UIEvent_OnTradingEquipOperation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OnTradingEquipOperation";

        public UIEvent_OnTradingEquipOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
        public int Value { get; set; }
    }


    public class UIEvent_DeviceInfo_NetWorkStateChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_DeviceInfo_NetWorkStateChange";

        public UIEvent_DeviceInfo_NetWorkStateChange()
            : base(EVENT_TYPE)
        {
        }
    }


    //public class UIEvent_ItemInfoFrame_ChangeItemCount : EventBase
    //{
    //    public static string EVENT_TYPE = "UIEvent_ItemInfoFrame_ChangeItemCount";
    //    public int Count { get; set; }
    //    public int Type { get; set; }

    //    public UIEvent_ItemInfoFrame_ChangeItemCount()
    //        : base(EVENT_TYPE)
    //    {
    //    }
    //}

    public class UIEvent_ItemInfoFrame_BtnAffirmClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ItemInfoFrame_BtnAffirmClick";

        public UIEvent_ItemInfoFrame_BtnAffirmClick(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class UIEvent_UpdateCurrentMission : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OpenMission";

        public UIEvent_UpdateCurrentMission(int npc)
            : base(EVENT_TYPE)
        {
            npcId = npc;
        }

        public int npcId { get; set; }
    }

    public class UIEvent_ShowMissionInfo : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowMissionInfo";

        public UIEvent_ShowMissionInfo(int id)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class Event_UpdateMissionData : EventBase
    {
        public static string EVENT_TYPE = "Event_UpdateMissionData";

        public Event_UpdateMissionData(int id = -1)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id { get; set; }
    }


    public class Event_MieShiStartCountDownData : EventBase
    {
        public static string EVENT_TYPE = "Event_MieShiStartCountDownData";

        public Event_MieShiStartCountDownData(int second)
            : base(EVENT_TYPE)
        {
            Second = second;
        }

        public int Second { get; set; }
    }


    public class Event_RefreshFuctionOnState : EventBase
    {
        public static string EVENT_TYPE = "Event_RefreshFuctionOnState";

        public Event_RefreshFuctionOnState()
            : base(EVENT_TYPE)
        {
        }
        
    }

    public class Event_ShowMissionDataDetail : EventBase
    {
        public static string EVENT_TYPE = "Event_ShowMissionDataDetail";

        public Event_ShowMissionDataDetail(int id)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class Event_MissionList_AutoNext : EventBase
    {
        public static string EVENT_TYPE = "Event_MissionList_AutoNext";

        public Event_MissionList_AutoNext()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_MissionList_TapIndex : EventBase
    {
        public static string EVENT_TYPE = "Event_MissionList_TapIndex";

        public Event_MissionList_TapIndex(int index)
            : base(EVENT_TYPE)
        {
            nIndex = index;
        }

        public int nIndex { get; set; }
    }

    public class FlagUpdateEvent : EventBase
    {
        public static string EVENT_TYPE = "FlagUpdateEvent";

        public FlagUpdateEvent(int i, bool v)
            : base(EVENT_TYPE)
        {
            Index = i;
            Value = v;
        }

        public int Index { get; set; }
        public bool Value { get; set; }
    }


    public class Event_ShowAchievementPage : EventBase
    {
        public static string EVENT_TYPE = "Event_ShowAchievementPage";

        public Event_ShowAchievementPage(int id, float percent = 0.0f)
            : base(EVENT_TYPE)
        {
            Id = id;
            Percent = percent;
        }

        public int Id { get; set; }
        public float Percent { get; set; }
    }

    public class Event_ScrollAchievement : EventBase
    {
        public static string EVENT_TYPE = "Event_ScrollAchievement";

        public Event_ScrollAchievement(float percent)
            : base(EVENT_TYPE)
        {
            Percent = percent;
        }

        public float Percent { get; set; }
    }

    public class Event_UpdateOnLineReward : EventBase
    {
        public static string EVENT_TYPE = "Event_UpdateOnLineReward";

        public Event_UpdateOnLineReward()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_UpdateLevelReward : EventBase
    {
        public static string EVENT_TYPE = "Event_UpdateLevelReward";

        public Event_UpdateLevelReward()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_ShowMieshiFubenInfo : EventBase
    {
        public static string EVENT_TYPE = "Event_ShowMieshiFubenInfo";

        public Event_ShowMieshiFubenInfo()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_ShowMieshiRankingInfo : EventBase
    {
        public static string EVENT_TYPE = "Event_ShowMieshiRankingInfo";

        public Event_ShowMieshiRankingInfo()
            : base(EVENT_TYPE)
        {
        }
    }


    public class LevelUpInitEvent : EventBase
    {
        public static string EVENT_TYPE = "LevelUpInitEvent";

        public LevelUpInitEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_LevelUp : EventBase
    {
        public static string EVENT_TYPE = "Event_LevelUp";

        public Event_LevelUp()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_LevelChange : EventBase
    {
        public static string EVENT_TYPE = "Event_LevelChange";

        public Event_LevelChange()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_CityResChange : EventBase
    {
        public static string EVENT_TYPE = "Event_CityResChange";

        public Event_CityResChange()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_UpdateContinuesLoginReward : EventBase
    {
        public static string EVENT_TYPE = "Event_UpdateContinuesLoginReward";

        public Event_UpdateContinuesLoginReward()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_UpdateMonthCheckinReward : EventBase
    {
        public static string EVENT_TYPE = "Event_UpdateMonthCheckinReward";

        public Event_UpdateMonthCheckinReward()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_UpdateActivityReward : EventBase
    {
        public static string EVENT_TYPE = "Event_UpdateActivityReward";

        public Event_UpdateActivityReward()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_AchievementTip : EventBase
    {
        public static string EVENT_TYPE = "Event_AchievementTip";

        public Event_AchievementTip(int id)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class Event_NextAchievementTip : EventBase
    {
        public static string EVENT_TYPE = "Event_NextAchievementTip";

        public Event_NextAchievementTip()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_ShowDialogue : EventBase
    {
        public static string EVENT_TYPE = "Event_ShowDialogue";

        public Event_ShowDialogue(List<DialogueData> dialogue, Action callback = null)
            : base(EVENT_TYPE)
        {
            Dialogue = dialogue;
            Callback = callback;
        }

        public Action Callback;
        public List<DialogueData> Dialogue { get; set; }
    }

    public class Event_ShowNextDialogue : EventBase
    {
        public static string EVENT_TYPE = "Event_ShowNextDialogue";

        public Event_ShowNextDialogue()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_TeamFrame_Leave : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_Leave";

        public UIEvent_TeamFrame_Leave()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_TeamFrame_Kick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_Kick";

        public UIEvent_TeamFrame_Kick(int nIndex)
            : base(EVENT_TYPE)
        {
            Index = nIndex;
        }

        public int Index { get; set; }
    }

    public class TeamMemberShowMenu : EventBase
    {
        public static string EVENT_TYPE = "TeamMemberShowMenu";

        public TeamMemberShowMenu(int i)
            : base(EVENT_TYPE)
        {
            Index = i;
        }

        public int Index { get; set; }
    }

    public class UIEvent_TeamFrame_AutoJion : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_AutoJion";

        public UIEvent_TeamFrame_AutoJion()
            : base(EVENT_TYPE)
        {
        }
    }

    public class SceneMapNotifyTeam : EventBase
    {
        public static string EVENT_TYPE = "SceneMapNotifyTeam";

        public SceneMapNotifyTeam(bool open)
            : base(EVENT_TYPE)
        {
            IsOpen = open;
        }

        public bool IsOpen { get; set; }
    }

    public class UIEvent_TeamFrame_AutoAccept : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_AutoAccept";

        public UIEvent_TeamFrame_AutoAccept()
            : base(EVENT_TYPE)
        {
        }
    }

    public class TeamChangeEvent : EventBase
    {
        public static string EVENT_TYPE = "TeamChangeEvent";

        public TeamChangeEvent(int t = 0)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class UIEvent_TeamFrame_Message : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_Message";

        public UIEvent_TeamFrame_Message(int type, ulong teamId, ulong characterId)
            : base(EVENT_TYPE)
        {
            Type = type;
            TeamId = teamId;
            CharacterId = characterId;
        }

        public ulong CharacterId { get; set; }
        public ulong TeamId { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_TeamFrame_NearTeam : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_NearTeam";

        public UIEvent_TeamFrame_NearTeam()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_TeamFrame_NearPlayer : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_NearPlayer";

        public UIEvent_TeamFrame_NearPlayer()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_TeamFrame_RefreshModel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TeamFrame_RefreshModel";

        public UIEvent_TeamFrame_RefreshModel(int nIndex)
            : base(EVENT_TYPE)
        {
            Index = nIndex;
        }

        public int Index { get; set; }
    }

    public class UIEvent_OperationList_Button : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OperationList_Button";

        public UIEvent_OperationList_Button(int nIndex)
            : base(EVENT_TYPE)
        {
            Index = nIndex;
        }

        public int Index { get; set; }
    }

    public class UIEvent_OperationList_AcceptInvite : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OperationList_AcceptInvite";

        public UIEvent_OperationList_AcceptInvite(ulong playerId, ulong teamId)
            : base(EVENT_TYPE)
        {
            PlayerId = playerId;
            TeamId = teamId;
        }

        public ulong PlayerId { get; set; }
        public ulong TeamId { get; set; }
    }

    public class UIEvent_OperationList_RefuseInvite : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OperationList_RefuseInvite";

        public UIEvent_OperationList_RefuseInvite(ulong playerId, ulong teamId)
            : base(EVENT_TYPE)
        {
            PlayerId = playerId;
            TeamId = teamId;
        }

        public ulong PlayerId { get; set; }
        public ulong TeamId { get; set; }
    }

    public class TeamOperateEvent : EventBase
    {
        public static string EVENT_TYPE = "TeamOperateEvent";

        public TeamOperateEvent(int t, bool isTeamShow)
            : base(EVENT_TYPE)
        {
            Type = t;
            misTeamShow = isTeamShow;
        }

        public int Type { get; set; }

        public bool misTeamShow { get; set; }
    }

    public class TeamApplyEvent : EventBase
    {
        public static string EVENT_TYPE = "TeamApplyEvent";

        public TeamApplyEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class TeamNearbyPlayerClick : EventBase
    {
        public static string EVENT_TYPE = "TeamNearbyPlayerClick";

        public TeamNearbyPlayerClick(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; } //0 Invite 1 pop menu
    }

    public class TeamNearbyTeamClick : EventBase
    {
        public static string EVENT_TYPE = "TeamNearbyTeamClick";

        public TeamNearbyTeamClick(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; } //0 Apply 1 pop menu
    }


    public class UIEvent_NumberPad_Click : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_NumberPad_Click";

        public UIEvent_NumberPad_Click(int value)
            : base(EVENT_TYPE)
        {
            keyValue = value;
        }

        public int keyValue;
    }

    public class UIEvent_UseSkill : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UseSkill";

        public UIEvent_UseSkill(int skillId)
            : base(EVENT_TYPE)
        {
            SkillId = skillId;
        }

        public int SkillId;
    }

    /// <summary>
    ///     技能按钮按下
    /// </summary>
    public class UIEvent_SkillButtonPressed : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillButtonPressed";

        public UIEvent_SkillButtonPressed(int skillId)
            : base(EVENT_TYPE)
        {
            SkillId = skillId;
        }

        public int SkillId;
    }

    /// <summary>
    ///     技能按钮抬起
    /// </summary>
    public class UIEvent_SkillButtonReleased : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SkillButtonReleased";

        public UIEvent_SkillButtonReleased(bool useSkill, int skillId)
            : base(EVENT_TYPE)
        {
            UseSkill = useSkill;
            SkillId = skillId;
        }

        public int SkillId;
        public bool UseSkill;
    }

    public class UIEvent_MatchingBack_Event : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MatchingBack_Event";

        public UIEvent_MatchingBack_Event(int nResult)
            : base(EVENT_TYPE)
        {
            Result = nResult;
        }

        public int Result { get; set; }
    }

    /// <summary>
    ///     出发点触发
    /// </summary>
    public class SceneEvent_Trigger : EventBase
    {
        public static string EVENT_TYPE = "SceneEvent_Trigger";

        public SceneEvent_Trigger(int triggerId)
            : base(EVENT_TYPE)
        {
            TriggerId = triggerId;
        }

        public int TriggerId { get; set; }
    }

    ///// <summary>
    ///// 出售点击确定按钮提交出售数量
    ///// </summary>
    //public class UIEven_ItemInfoFram_SellSubmitCount : EventBase
    //{
    //    public static string EVENT_TYPE = "UIEven_ItemInfoFram_SellSubmitCount";
    //    public int Count { get; set; }

    //    public UIEven_ItemInfoFram_SellSubmitCount()
    //        : base(EVENT_TYPE)
    //    {
    //    }
    //}

    /// <summary>
    ///     选择建筑
    /// </summary>
    public class UIEvent_CityTapBuildingIcon : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityTapBuildingIcon";

        public UIEvent_CityTapBuildingIcon(int areaId)
            : base(EVENT_TYPE)
        {
            AreaId = areaId;
        }

        public int AreaId { get; set; }
    }

    /// <summary>
    ///     在可建造的列表里选择建筑
    /// </summary>
    public class UIEvent_CitySelectBuilding : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CitySelectBuilding";

        public UIEvent_CitySelectBuilding(int buildingId)
            : base(EVENT_TYPE)
        {
            BuildingId = buildingId;
        }

        public int BuildingId { get; set; }
    }

    /// <summary>
    ///     取消选择建筑
    /// </summary>
    public class UIEvent_CityOnUnSelectBuilding : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityOnUnSelectBuilding";

        public UIEvent_CityOnUnSelectBuilding()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     建筑操作
    /// </summary>
    public class UIEvent_CityOperate : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityOperate";

        public UIEvent_CityOperate(int idx, CityOperationType opType)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            OpType = opType;
        }

        public int Idx;
        public CityOperationType OpType { get; set; }
    }

    /// <summary>
    ///     更新建筑信息
    /// </summary>
    public class UIEvent_CityUpdateBuilding : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityUpdateBuilding";

        public UIEvent_CityUpdateBuilding(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx { get; set; }
    }

    /// <summary>
    ///     选定建筑，准备建造
    /// </summary>
    public class UIEvent_ShowBuildingRequirement : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowBuildingRequirement";

        public UIEvent_ShowBuildingRequirement()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     随从列表过滤变化
    /// </summary>
    public class UIEvent_PetListFilterChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PetListFilterChange";

        public UIEvent_PetListFilterChange(int type = -1)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type;
    }

    /// <summary>
    ///     吃药
    /// </summary>
    public class UIEvent_UseDrug : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UseDrug";

        public UIEvent_UseDrug(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx;
    }

    /// <summary>
    ///     停止吃药
    /// </summary>
    public class UIEvent_StopUseDrug : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_StopUseDrug";

        public UIEvent_StopUseDrug(bool idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public bool Idx;
    }

    /// <summary>
    ///     查看技能
    /// </summary>
    public class UIEvent_SeeSkills : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SeeSkills";

        public UIEvent_SeeSkills(PetSkillInfoDataModel idx, bool flag)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Flag = flag;
        }

        public bool Flag;
        public PetSkillInfoDataModel Idx;
    }

    /// <summary>
    ///     点击显示随从吃药列表
    /// </summary>
    public class UIEvent_ShowDruglist : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowDruglist";

        public UIEvent_ShowDruglist(bool flag)
            : base(EVENT_TYPE)
        {
            Flag = flag;
        }

        public bool Flag;
    }

    /// <summary>
    ///     查看Buff
    /// </summary>
    public class UIEvent_SeeBuffs : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SeeBuffs";

        public UIEvent_SeeBuffs()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     选择随从列表里的随从
    /// </summary>
    public class UIEvent_ChoosePetList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ChoosePetList";

        public UIEvent_ChoosePetList(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx;
    }

    /// <summary>
    ///     选择随从任务
    /// </summary>
    public class UIEvent_ChoosePetMissionList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ChoosePetMissionList";

        public UIEvent_ChoosePetMissionList(int missionId)
            : base(EVENT_TYPE)
        {
            MissionId = missionId;
        }

        public int MissionId;
    }


    /// <summary>
    ///     刷新随从任务
    /// </summary>
    public class UIEvent_UpdatePetMissionList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UpdatePetMissionList";

        public UIEvent_UpdatePetMissionList()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     选择任务随从
    /// </summary>
    public class UIEvent_ChooseMissionPet : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ChooseMissionPet";

        public UIEvent_ChooseMissionPet(int idx, bool selected)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Selected = selected;
        }

        public int Idx;
        public bool Selected;
    }

    /// <summary>
    ///     选择任务随从
    /// </summary>
    public class UIEvent_SelectMissionPet : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectMissionPet";

        public UIEvent_SelectMissionPet(int idx, bool selected)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Selected = selected;
        }

        public int Idx;
        public bool Selected;
    }

    /// <summary>
    ///     操作随从任务
    /// </summary>
    public class UIEvent_OperatePetMission : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OperatePetMission";

        public UIEvent_OperatePetMission(int opt, int missionId = -1)
            : base(EVENT_TYPE)
        {
            Opt = opt;
            MissionId = missionId;
        }

        public int MissionId;
        public int Opt;
    }

    /// <summary>
    ///     随从一键添加
    /// </summary>
    public class UIEvent_MissionPetAutoSelect : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MissionPetAutoSelect";

        public UIEvent_MissionPetAutoSelect()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     每秒钟事件
    /// </summary>
    /// <summary>
    ///     没分钟时间
    /// </summary>
    /// <summary>
    ///     打开建筑界面
    /// </summary>
    public class UIEvent_OpenCityBuilding : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OpenCityBuilding";

        public UIEvent_OpenCityBuilding(int areaId, int type)
            : base(EVENT_TYPE)
        {
            AreaId = areaId;
            BuildingType = type;
        }

        public int AreaId;
        public int BuildingType;
    }

    /// <summary>
    ///     家园事件
    /// </summary>
    public class UIEvent_CityEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityEvent";

        public UIEvent_CityEvent(string stringParam, List<int> intParam = null)
            : base(EVENT_TYPE)
        {
            StringParam = stringParam;
            IntParam = intParam;
        }

        public List<int> IntParam;
        public string StringParam;
    }

    /// <summary>
    ///     家园界面时间更新
    /// </summary>
    public class UIEvent_CityUpdateTimeEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityUpdateTimeEvent";

        public UIEvent_CityUpdateTimeEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     家园界面资源更新
    /// </summary>
    public class UIEvent_CityUpdateResourceEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityUpdateResourceEvent";

        public UIEvent_CityUpdateResourceEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     孵化室打开蛋列表
    /// </summary>
    public class UIEvent_HatchingHouseOpenList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HatchingHouseOpenList";

        public UIEvent_HatchingHouseOpenList(int type, int idx, bool open)
            : base(EVENT_TYPE)
        {
            Type = type;
            Idx = idx;
            Open = open;
        }

        public int Idx;
        public bool Open;
        public int Type;
    }

    /// <summary>
    ///     收回主菜单功能滚轮
    /// </summary>
    public class UIEvent_MainuiCloseList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MainuiCloseList";

        public UIEvent_MainuiCloseList()
            : base(EVENT_TYPE)
        {
        }
    }

    /// <summary>
    ///     孵化室选中
    /// </summary>
    public class UIEvent_HatchingHouseCheck : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HatchingHouseCheck";

        public UIEvent_HatchingHouseCheck(int type, int idx, bool check)
            : base(EVENT_TYPE)
        {
            Type = type;
            Idx = idx;
            Check = check;
        }

        public bool Check;
        public int Idx;
        public int Type;
    }

    /// <summary>
    ///     孵化室操作
    /// </summary>
    public class UIEvent_HatchingOperate : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HatchingOperate";

        public UIEvent_HatchingOperate(int type, int idx)
            : base(EVENT_TYPE)
        {
            Type = type;
            Idx = idx;
        }

        public int Idx;
        public int Type;
    }

    /// <summary>
    ///     包裹变化事件
    /// </summary>
    public class UIEvent_BagChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_BagChange";

        public UIEvent_BagChange()
            : base(EVENT_TYPE)
        {
        }

        private readonly List<int> BagTypeList = new List<int>();

        public void AddType(int bagType)
        {
            if (BagTypeList.Contains(bagType))
            {
                return;
            }

            BagTypeList.Add(bagType);
        }

        public bool HasType(eBagType type)
        {
            if (BagTypeList.Contains((int) type))
            {
                return true;
            }

            return false;
        }
    }

    public class UIEvent_BagItemCountChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_BagItemCountChange";

        public UIEvent_BagItemCountChange(int id, int count)
            : base(EVENT_TYPE)
        {
            ItemId = id;
            ChangeCount = count;
        }

        public int ChangeCount { get; set; }
        public int ItemId { get; set; }
    }

    /// <summary>
    ///     显隐排队等待界面
    /// </summary>
    public class UIEvent_ShowDungeonQueue : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowDungeonQueue";

        public UIEvent_ShowDungeonQueue(int isShow)
            : base(EVENT_TYPE)
        {
            IsShow = isShow;
        }

        public int IsShow { get; set; }
    }

    public class UIEvent_CloseDungeonQueue : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CloseDungeonQueue";

        public UIEvent_CloseDungeonQueue(int showtype)
            : base(EVENT_TYPE)
        {
            ShowMessageType = showtype;
        }

        public int ShowMessageType { get; set; }
    }

    public class QueueCanceledEvent : EventBase
    {
        public static string EVENT_TYPE = "QueueCanceledEvent";

        public QueueCanceledEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_WindowShowDungeonQueue : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WindowShowDungeonQueue";

        public UIEvent_WindowShowDungeonQueue(DateTime queueDateTime, int queueID)
            : base(EVENT_TYPE)
        {
            QueueDateTime = queueDateTime;
            QueueID = queueID;
        }

        public DateTime QueueDateTime { get; set; }
        public int QueueID { get; set; }
    }

    public class UIEvent_SelectServer : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectServer";

        public UIEvent_SelectServer(int serverId)
            : base(EVENT_TYPE)
        {
            ServerId = serverId;
        }

        public int ServerId { get; set; }
    }

    public class UIEvent_SailingReturnBtn : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingReturnBtn";

        public UIEvent_SailingReturnBtn(int showType)
            : base(EVENT_TYPE)
        {
            Showtype = showType;
        }

        public int Showtype { get; set; }
    }

    public class QueneUpdateEvent : EventBase
    {
        public static string EVENT_TYPE = "QueneUpdateEvent";

        public QueneUpdateEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ChatMainHelpMeesage : EventBase
    {
        public static string EVENT_TYPE = "ChatMainHelpMeesage";

        public ChatMainHelpMeesage(string c)
            : base(EVENT_TYPE)
        {
            Content = c;
        }

        public string Content { get; set; }
    }

    public class WhisperChatMessage : EventBase
    {
        public static string EVENT_TYPE = "WhisperChatMessage";

        public WhisperChatMessage(ChatMessageData d)
            : base(EVENT_TYPE)
        {
            DataModel = d;
        }

        public ChatMessageData DataModel;
    }

    public class Event_PushMessage : EventBase
    {
        public static string EVENT_TYPE = "Event_PushMessage";

        public Event_PushMessage(ChatMessageDataModel data)
            : base(EVENT_TYPE)
        {
            DataModel = data;
        }

        public ChatMessageDataModel DataModel;
    }

    public class Event_TeamInvitePlayer : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamInvitePlayer";

        public Event_TeamInvitePlayer(ulong id)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
        }

        public ulong CharacterId;
    }

    public class Event_TeamSwapLeader : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamSwapLeader";

        public Event_TeamSwapLeader(ulong id)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
        }

        public ulong CharacterId;
    }

    public class Event_TeamKickPlayer : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamKickPlayer";

        public Event_TeamKickPlayer(ulong id)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
        }

        public ulong CharacterId;
    }

    public class Event_TeamAcceptJoin : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamAcceptJoin";

        public Event_TeamAcceptJoin(ulong id)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
        }

        public ulong CharacterId;
    }

    public class Event_TeamRefuseJoin : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamRefuseJoin";

        public Event_TeamRefuseJoin(ulong id)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
        }

        public ulong CharacterId;
    }

    public class Event_TeamApplyOtherTeam : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamApplyOtherTeam";

        public Event_TeamApplyOtherTeam(ulong id)
            : base(EVENT_TYPE)
        {
            CharacterId = id;
        }

        public ulong CharacterId;
    }

    public class Event_TeamLeaveTeam : EventBase
    {
        public static string EVENT_TYPE = "Event_TeamLeaveTeam";

        public Event_TeamLeaveTeam()
            : base(EVENT_TYPE)
        {
        }
    }

    public class Event_ServerListButton : EventBase
    {
        public static string EVENT_TYPE = "Event_ServerListButton";

        public Event_ServerListButton(int type)
            : base(EVENT_TYPE)
        {
            ButtonType = type;
        }

        public int ButtonType;
    }

    public class Event_ServerGroupListCellIndex : EventBase
    {
        public static string EVENT_TYPE = "Event_ServerGroupListCellIndex";

        public Event_ServerGroupListCellIndex(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index;
    }

    public class Event_ServerListCellIndex : EventBase
    {
        public static string EVENT_TYPE = "Event_ServerListCellIndex";

        public Event_ServerListCellIndex(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index;
    }

    public class UIEvent_SailingPackItemClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPackItemClick";

        public UIEvent_SailingPackItemClick()
            : base(EVENT_TYPE)
        {
        }

        public int BagId { get; set; }
        public int Index { get; set; }
    }

    public class UIEvent_SailingPickAll : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPickAll";

        public UIEvent_SailingPickAll()
            : base(EVENT_TYPE)
        {
        }

        public int ItemIndex { get; set; }
    }

    public class UIEvent_SailingPickOne : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPickOne";

        public UIEvent_SailingPickOne()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_SailingPutOnClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPutOnClick";

        public UIEvent_SailingPutOnClick()
            : base(EVENT_TYPE)
        {
        }

        public int BagId { get; set; }
        public int Index { get; set; }
        public int PutOnOrOff { get; set; }
    }

    public class UIEvent_SailingPackItemUI : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPackItemUI";

        public UIEvent_SailingPackItemUI()
            : base(EVENT_TYPE)
        {
        }

        public int BagId { get; set; }
        public int Index { get; set; }
        public int PutOnOrOff { get; set; }
    }

    public class UIEvent_SailingLightPoint : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingLightPoint";

        public UIEvent_SailingLightPoint(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }
    public class UIEvent_SailingLightPointAccess : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingLightPointAccess";

        public UIEvent_SailingLightPointAccess(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_SailingLineButton : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingLineButton";

        public UIEvent_SailingLineButton(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_SailingOperation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingOperation";

        public UIEvent_SailingOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    //public class UIEvent_SailingAnim : EventBase
    //{
    //    public static string EVENT_TYPE = "UIEvent_SailingAnim";
    //    public int Type { get; set; }

    //    public int Index { get; set; }
    //    public UIEvent_SailingAnim(int index,int type)
    //        : base(EVENT_TYPE)
    //    {
    //        Index = index;
    //        Type = type;
    //    }
    //}

    public class UIEvent_SailingPlayAnimation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPlayAnimation";

        public UIEvent_SailingPlayAnimation()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SailingPlayEatAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingPlayEatAnim";

        public UIEvent_SailingPlayEatAnim()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
        public List<int> ItemIds { get; set; }
        public List<int> List { get; set; }
    }

    public class UIEvent_SailingFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SailingFlyAnim";

        public UIEvent_SailingFlyAnim(int idx, int exp)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Exp = exp;
        }

        public int Exp { get; set; }
        public int Idx { get; set; }
    }

    public class UIEvent_ComposeFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ComposeFlyAnim";

        public UIEvent_ComposeFlyAnim(int exp)
            : base(EVENT_TYPE)
        {
            Exp = exp;
        }

        public int Exp { get; set; }
    }

    public class UIEvent_TradingFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TradingFlyAnim";

        public UIEvent_TradingFlyAnim(int exp)
            : base(EVENT_TYPE)
        {
            Exp = exp;
        }

        public int Exp { get; set; }
    }

    public class UIEvent_CityRefreshTradingNotice : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityRefreshTradingNotice";

        public UIEvent_CityRefreshTradingNotice(bool state)
            : base(EVENT_TYPE)
        {
            NoticeState = state;
        }

        public bool NoticeState { get; set; }
    }

    #region 许愿池

    public class UIEvent_WishingOperation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingOperation";

        public UIEvent_WishingOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class UIEvent_WishPlayFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishPlayFlyAnim";

        public UIEvent_WishPlayFlyAnim()
            : base(EVENT_TYPE)
        {
        }

        public int AnimIndex { get; set; }
        public int DrawType { get; set; }
        public int Exp { get; set; }
        public int Index { get; set; }
        public List<int> ItemIds { get; set; }
        public List<int> List { get; set; }
    }


    public class UIEvent_WishingGetDrawResultBack : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingGetDrawResultBack";

        public UIEvent_WishingGetDrawResultBack()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_WishingInfoItem : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingInfoItem";

        public UIEvent_WishingInfoItem()
            : base(EVENT_TYPE)
        {
        }

        public BagItemDataModel item { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_WishingInfoWillItemBtn : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingInfoWillItemBtn";

        public UIEvent_WishingInfoWillItemBtn()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_WishingBtnWishingBag : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingBtnWishingBag";

        public UIEvent_WishingBtnWishingBag()
            : base(EVENT_TYPE)
        {
        }

        public int Isback { get; set; }
    }

    public class UIEvent_WishingItemOperation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingItemOperation";

        public UIEvent_WishingItemOperation()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
        public int Operation { get; set; }
    }

    public class UIEvent_WishingBtnTreeList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingBtnTreeList";

        public UIEvent_WishingBtnTreeList()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_WishingBtnBuyAddOrReduce : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingBtnBuyAddOrReduce";

        public UIEvent_WishingBtnBuyAddOrReduce()
            : base(EVENT_TYPE)
        {
        }

        public int Type { get; set; }
    }

    public class UIEvent_WishingGoodsItemBuy : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingGoodsItemBuy";

        public UIEvent_WishingGoodsItemBuy()
            : base(EVENT_TYPE)
        {
        }

        public WishingGoodsDataModel item { get; set; }
    }

    public class UIEvent_ElfBaoXiangOverEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ElfBaoXiangOverEvent";

        public UIEvent_ElfBaoXiangOverEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_ElfShowDrawGetEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ElfShowDrawGetEvent";

        public UIEvent_ElfShowDrawGetEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_WishingTenDrawStop : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishingTenDrawStop";

        public UIEvent_WishingTenDrawStop()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
        public bool IsPet { get; set; }
        public int ItemId { get; set; }
    }

    public class UIEvent_WishShowDrawGetEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_WishShowDrawGetEvent";

        public UIEvent_WishShowDrawGetEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    #endregion

    public class UIEvent_CityWishReflesh : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityWishReflesh";

        public UIEvent_CityWishReflesh()
            : base(EVENT_TYPE)
        {
        }

        //    public int FreeCount { get; set; }
        public BuildingData BuildData { get; set; }
        public long MyTime { get; set; }
    }

    public class UIEvent_CityTradingStack : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityTradingStack";

        public UIEvent_CityTradingStack(int currentCount, int stackCount)
            : base(EVENT_TYPE)
        {
            CurrentCount = currentCount;
            StackCount = stackCount;
        }

        public int CurrentCount { get; set; }
        public int StackCount { get; set; }
    }

    public class UIEvent_RecycleBtn : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleBtn";

        public UIEvent_RecycleBtn(int mtype)
            : base(EVENT_TYPE)
        {
            Type = mtype;
        }

        public int Type { get; set; }
    }

    public class UIEvent_RecycleBack : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleBack";

        public UIEvent_RecycleBack()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RecycleGetOK : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleGetOK";

        public UIEvent_RecycleGetOK()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RecycleGetCancel : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleGetCancel";

        public UIEvent_RecycleGetCancel()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RecycleArrange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleArrange";

        public UIEvent_RecycleArrange(int tabPage)
            : base(EVENT_TYPE)
        {
            TabPage = tabPage;
        }

        public int TabPage { get; set; }
    }

    public class UIEvent_RecycleItemSelect : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleItemSelect";

        public UIEvent_RecycleItemSelect()
            : base(EVENT_TYPE)
        {
        }

        public BagItemDataModel Item { get; set; }
        public int type { get; set; }
    }

    public class UIEvent_RecycleSetGridCenter : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RecycleSetGridCenter";

        public UIEvent_RecycleSetGridCenter()
            : base(EVENT_TYPE)
        {
        }

        public int index { get; set; }
    }

    #region 战盟

    public class UIEvent_UnionBtnCreateUnion : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionBtnCreateUnion";

        public UIEvent_UnionBtnCreateUnion()
            : base(EVENT_TYPE)
        {
        }

        public string Name;
    }

    public class UIEvent_UnionBtnPassApply : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionBtnPassApply";

        public UIEvent_UnionBtnPassApply()
            : base(EVENT_TYPE)
        {
        }

        public int Type { get; set; }
    }

    public class UIEvent_UnionCharacterClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionCharacterClick";

        public UIEvent_UnionCharacterClick(CharacterBaseInfoDataModel data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public CharacterBaseInfoDataModel Data;
    }

    public class UIEvent_UnionBtnDonation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionBtnDonation";

        public UIEvent_UnionBtnDonation()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionDonationItem : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionDonationItem";

        public UIEvent_UnionDonationItem()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionDonationItemClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionDonationItemClick";

        public UIEvent_UnionDonationItemClick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionBuffUpShow : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionBuffUpShow";

        public UIEvent_UnionBuffUpShow()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionBossClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionBossClick";

        public UIEvent_UnionBossClick()
            : base(EVENT_TYPE)
        {
        }

        public int BossIndex { get; set; }
        public int type { get; set; }
    }

    public class UIEvent_UnionTabPageClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionTabPageClick";

        public UIEvent_UnionTabPageClick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionAnim";

        public UIEvent_UnionAnim()
            : base(EVENT_TYPE)
        {
        }

        public int Type { get; set; }
    }


    public class UIEvent_UnionTabPageClick2 : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionTabPageClick2";

        public UIEvent_UnionTabPageClick2()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionBattlePageCLick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionBattlePageCLick";

        public UIEvent_UnionBattlePageCLick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionOperation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionOperation";

        public UIEvent_UnionOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class UIEvent_UnionCommunication : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionCommunication";

        public UIEvent_UnionCommunication(int index, ulong characterid)
            : base(EVENT_TYPE)
        {
            ListIndex = index;
            CharacterId = characterid;
        }

        public ulong CharacterId { get; set; }
        public int ListIndex { get; set; }
    }

    public class UIEvent_UnionOtherListClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionOtherListClick";

        public UIEvent_UnionOtherListClick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_UnionJoinReply : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionJoinReply";

        public UIEvent_UnionJoinReply()
            : base(EVENT_TYPE)
        {
        }

        public int AllianceId { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_UnionSyncDataChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionSyncDataChange";

        public UIEvent_UnionSyncDataChange()
            : base(EVENT_TYPE)
        {
        }

        public int param1 { get; set; }
        public int param2 { get; set; }
        public int Type { get; set; }
    }

    public class RefreshDamageListEvent : EventBase
    {
        public static string EVENT_TYPE = "RefreshDamageListEvent";

        public RefreshDamageListEvent(DamageList damageList)
            : base(EVENT_TYPE)
        {
            DamageList = damageList;
        }

        public DamageList DamageList { get; set; }
    }

    public class RefreshDungeonInfo_Event : EventBase
    {
        public static string EVENT_TYPE = "RefreshDungeonInfo_Event";

        public RefreshDungeonInfo_Event(DungeonInfo info)
            : base(EVENT_TYPE)
        {
            Info = info;
        }

        public DungeonInfo Info { get; set; }
    }

    public class RefreshMieshiDamageListEvent : EventBase
    {
        public static string EVENT_TYPE = "RefreshDamageMieshiListEvent";

        public RefreshMieshiDamageListEvent(PointList damageList)
            : base(EVENT_TYPE)
        {
            DamageList = damageList;
        }

        public PointList DamageList { get; set; }
    }

    public class RefreshDungeonInfoEvent : EventBase
    {
        public static string EVENT_TYPE = "RefreshDungeonInfoEvent";

        public RefreshDungeonInfoEvent(FubenInfoMsg info)
            : base(EVENT_TYPE)
        {
            FubenInfo = info;
        }

        public FubenInfoMsg FubenInfo { get; set; }
    }

    public class StrongpointStateChangedEvent : EventBase
    {
        public static string EVENT_TYPE = "StrongpointStateChangedEvent";

        public StrongpointStateChangedEvent(int camp, int index, int state, float time)
            : base(EVENT_TYPE)
        {
            Camp = camp;
            Index = index;
            State = state;
            Time = time;
        }

        public int Camp { get; set; }
        public int Index { get; set; }
        public int State { get; set; }
        public float Time { get; set; }
    }

    public class UIEvent_UnionGetCharacterID : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UnionGetCharacterID";

        public UIEvent_UnionGetCharacterID(ulong characterID)
            : base(EVENT_TYPE)
        {
            CharacterID = characterID;
        }

        public ulong CharacterID { get; set; }
    }

    public class UIEvent_BattleShopCellClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_BattleShopCellClick";

        public UIEvent_BattleShopCellClick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_BattleBtnAutoAccept : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_BattleBtnAutoAccept";

        public UIEvent_BattleBtnAutoAccept()
            : base(EVENT_TYPE)
        {
        }
    }

    #endregion

    public class UIEvent_ErrorTip : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ErrorTip";

        public UIEvent_ErrorTip(ErrorCodes errorCode)
            : base(EVENT_TYPE)
        {
            ErrorCode = errorCode;
        }

        public ErrorCodes ErrorCode { get; set; }
    }

    public class UIEvent_MainUIButtonShowEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MainUIButtonShowEvent";

        public UIEvent_MainUIButtonShowEvent(int param)
            : base(EVENT_TYPE)
        {
            Param = param;
        }

        public int Param { get; set; }
    }

    //显示场景图片
    public class UIEvent_MainUIMapnameShowEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MainUIMapnameShowEvent";

        public UIEvent_MainUIMapnameShowEvent(int icon)
            : base(EVENT_TYPE)
        {
            Icon = icon;
        }

        public int Icon { get; set; }
    }

    public class UIEvent_HatchingRoomEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HatchingRoomEvent";

        public UIEvent_HatchingRoomEvent(string tag, int param = -1)
            : base(EVENT_TYPE)
        {
            Tag = tag;
            Param = param;
        }

        public int Param { get; set; }
        public string Tag { get; set; }
    }

    public class UIEvent_HintEquipEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HintEquipEvent";

        public UIEvent_HintEquipEvent(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_HintUseItemEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HintUseItemEvent";

        public UIEvent_HintUseItemEvent(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_HintCloseEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HintCloseEvent";

        public UIEvent_HintCloseEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SmithyBtnClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SmithyBtnClickedEvent";

        public UIEvent_SmithyBtnClickedEvent(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class UIEvent_SpecialItemShowEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SpecialItemShowEvent";

        public UIEvent_SpecialItemShowEvent()
            : base(EVENT_TYPE)
        {
        }
        
    }

    public class UIEvent_SmithTabPageEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SmithTabPageEvent";

        public UIEvent_SmithTabPageEvent(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }


    public class RemoveEvoEquipEvent : EventBase
    {
        public static string EVENT_TYPE = "RemoveEvoEquipEvent";

        public RemoveEvoEquipEvent(BagItemDataModel itemData)
            : base(EVENT_TYPE)
        {
            ItemData = itemData;
        }

        public BagItemDataModel ItemData { get; set; }
    }

    public class ArtifactRemoveEvoEquipEvent : EventBase
    {
        public static string EVENT_TYPE = "ArtifactRemoveEvoEquipEvent";

        public ArtifactRemoveEvoEquipEvent(BagItemDataModel itemData)
            : base(EVENT_TYPE)
        {
            ItemData = itemData;
        }

        public BagItemDataModel ItemData { get; set; }
    }

    public class UIEvent_SmithySetGridCenter : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SmithySetGridCenter";

        public UIEvent_SmithySetGridCenter(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    #region 占星术

    public class UIEvent_AstrologyDrawResult : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyDrawResult";

        public UIEvent_AstrologyDrawResult()
            : base(EVENT_TYPE)
        {
        }

        public DrawItemResult DrawItems { get; set; }
        public long DrawTime { get; set; }
    }

    public class UIEvent_AstrologyBtnBuyList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBtnBuyList";

        public UIEvent_AstrologyBtnBuyList()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyBtnDiamonds : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBtnDiamonds";

        public UIEvent_AstrologyBtnDiamonds()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyBtnPutOn : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBtnPutOn";

        public UIEvent_AstrologyBtnPutOn()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyBtnPutOff : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBtnPutOff";

        public UIEvent_AstrologyBtnPutOff()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyMainListClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyMainListClick";

        public UIEvent_AstrologyMainListClick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
        public int ItemIndex { get; set; }
    }

    public class UIEvent_AstrologyBack : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBack";

        public UIEvent_AstrologyBack()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }


    public class UIEvent_AstrologyBagItemClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBagItemClick";

        public UIEvent_AstrologyBagItemClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologySimpleListClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologySimpleListClick";

        public UIEvent_AstrologySimpleListClick()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyOperation : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyOperation";

        public UIEvent_AstrologyOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class UIEvent_AstrologyBagTabClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyBagTabClick";

        public UIEvent_AstrologyBagTabClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyMainIconClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyMainIconClick";

        public UIEvent_AstrologyMainIconClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologyArrangeClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologyArrangeClick";

        public UIEvent_AstrologyArrangeClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_AstrologySetGridLookIndex : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AstrologySetGridLookIndex";

        public UIEvent_AstrologySetGridLookIndex()
            : base(EVENT_TYPE)
        {
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    #endregion

    public class UIEvent_UpdateGuideEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UpdateGuideEvent";

        public UIEvent_UpdateGuideEvent(int step)
            : base(EVENT_TYPE)
        {
            Step = step;
        }

        public int Step = -1;
    }

    public class UIEvent_NextGuideEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_NextGuideEvent";

        public UIEvent_NextGuideEvent(float delay)
            : base(EVENT_TYPE)
        {
            Delay = delay;
        }

        public float Delay;
    }

    public class UpdateTimerEvent : EventBase
    {
        public static string EVENT_TYPE = "UpdateTimerEvent";

        public UpdateTimerEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public enum BtnType
    {
        //活动界面
        Activity_Enter, //进入活动副本按钮
        Activity_Queue, //“预约”或者“取消”按钮
        Activity_GotoMonster, //“前往剿灭”怪物按钮
        Activity_FlytoMonster,// 同上，vip飞过去
        DynamicActivity_Enter, //进入动态活动副本
        DynamicActivity_Queue, //“预约”或者“取消”动态活动副本
        MieShiActivity_Queue, //灭世活动副本
        Activity_GetDoubleExp // 获取累计经验
    }

    public class UIEvent_ButtonClicked : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ButtonClicked";

        public UIEvent_ButtonClicked(BtnType type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public BtnType Type;
    }

    public class UIEvent_ButtonClicked_1 : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ButtonClicked_1";

        public UIEvent_ButtonClicked_1(int idx)
            : base(EVENT_TYPE)
        {
            Index = idx;
        }

        public int Index;
    }

    public class UIEvent_ActivityTabSelectEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ActivityTabSelectEvent";

        public UIEvent_ActivityTabSelectEvent(int idx)
            : base(EVENT_TYPE)
        {
            TabIdx = idx;
        }

        public int TabIdx;
    }

    public class UIEvent_UpdateSkillAndHpEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UpdateSkillAndHpEvent";

        public UIEvent_UpdateSkillAndHpEvent(int iType)
            : base(EVENT_TYPE)
        {
            Type = iType;
        }

        public int Type;
    }

    public class UIEvent_UpdateUseItemEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UpdateUseItemEvent";

        public UIEvent_UpdateUseItemEvent()
            : base(EVENT_TYPE)
        {
        }
        
    }

    public class ActivityFuben_ResetQueue_Event : EventBase
    {
        public static string EVENT_TYPE = "ActivityFuben_ResetQueue_Event";

        public ActivityFuben_ResetQueue_Event(int id)
            : base(EVENT_TYPE)
        {
            QueueId = id;
        }

        public int QueueId;
    }

    public enum UpdateActivityTimerType
    {
        Single, //刷新某一个timer
        MainPage //刷新主界面上所有的timer
    }

    public class UpdateActivityTimerEvent : EventBase
    {
        public static string EVENT_TYPE = "UpdateActivityTimerEvent";

        public UpdateActivityTimerEvent(UpdateActivityTimerType type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public UpdateActivityTimerType Type;
    }

    public class UpdateActivityTipTimerEvent : EventBase
    {
        public static string EVENT_TYPE = "UpdateActivityTipTimerEvent";

        public UpdateActivityTipTimerEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ActivityCellClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "ActivityCellClickedEvent";

        public ActivityCellClickedEvent(int idx)
            : base(EVENT_TYPE)
        {
            Index = idx;
        }

        public int Index;
    }

    public class ShowActivityTipEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowActivityTipEvent";

        public ShowActivityTipEvent(int fubenId, int dicId, DateTime startTime, DateTime targetTime)
            : base(EVENT_TYPE)
        {
            FubenId = fubenId;
            DicId = dicId;
            StartTime = startTime;
            TargetTime = targetTime;
        }

        public int DicId;
        public int FubenId;
        public DateTime StartTime;
        public DateTime TargetTime;
    }

    public class DungeonTipClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "DungeonTipClickedEvent";

        public DungeonTipClickedEvent(int fubenId)
            : base(EVENT_TYPE)
        {
            FubenId = fubenId;
        }

        public int FubenId;
    }

    public class ActivityTipClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "ActivityTipClickedEvent";

        public ActivityTipClickedEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class NotifyGameObjectEvent : EventBase
    {
        public static string EVENT_TYPE = "NotifyGameObjectEvent";

        public NotifyGameObjectEvent(Transform obj)
            : base(EVENT_TYPE)
        {
            Object = obj;
        }

        public Transform Object;
    }

    public class UIEvent_PlayMainUIBtnAnimEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PlayMainUIBtnAnimEvent";

        public UIEvent_PlayMainUIBtnAnimEvent(string btnName, Action callBack = null)
            : base(EVENT_TYPE)
        {
            BtnName = btnName;
            CallBack = callBack;
        }

        public string BtnName = string.Empty;
        public Action CallBack;
    }

    public class UIEvent_PetChangeEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PetChangeEvent";

        public UIEvent_PetChangeEvent(int bagItemIndex)
            : base(EVENT_TYPE)
        {
            BagItemIndex = bagItemIndex;
        }

        public int BagItemIndex = -1;
    }

    public class UIEvent_PetLevelup : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PetLevelup";

        public UIEvent_PetLevelup(int petId)
            : base(EVENT_TYPE)
        {
            PetId = petId;
        }

        public int PetId = -1;
    }

    public class UIEvent_AutoAssignEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_AutoAssignEgg";

        public UIEvent_AutoAssignEgg(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_openeggEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_openeggEgg";

        public UIEvent_openeggEgg(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_CloseEggEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CloseEggEgg";

        public UIEvent_CloseEggEgg(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_CloseAllEggEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CloseAllEggEgg";

        public UIEvent_CloseAllEggEgg()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_CloseOtherEggEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CloseOtherEggEgg";

        public UIEvent_CloseOtherEggEgg(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_DisableOtherEggEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_DisableOtherEggEgg";

        public UIEvent_DisableOtherEggEgg()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_CloseEggMessage : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CloseEggMessage";

        public UIEvent_CloseEggMessage(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_ShowEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowEgg";

        public UIEvent_ShowEgg(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_NotShowEgg : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_NotShowEgg";

        public UIEvent_NotShowEgg(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx = -1;
    }

    public class UIEvent_PetFrameEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PetFrameEvent";

        public UIEvent_PetFrameEvent(string eventTag, int tag = -1)
            : base(EVENT_TYPE)
        {
            EventTag = eventTag;
            Tag = tag;
        }

        public string EventTag;
        public int Tag;
    }

    public class UIEvent_PetMissionDetailPress : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PetMissionDetailPress";

        public UIEvent_PetMissionDetailPress(int type, int idx = -1)
            : base(EVENT_TYPE)
        {
            Type = type;
            Idx = idx;
        }

        public int Idx;
        public int Type;
    }

    public class EquipChangeEvent : EventBase
    {
        public static string EVENT_TYPE = "EquipChangeEvent";

        public EquipChangeEvent(BagItemDataModel item)
            : base(EVENT_TYPE)
        {
            Item = item;
        }

        public BagItemDataModel Item { get; set; }
    }

    public class EquipChangeEndEvent : EventBase
    {
        public static string EVENT_TYPE = "EquipChangeEndEvent";

        public EquipChangeEndEvent(BagItemDataModel item)
            : base(EVENT_TYPE)
        {
            Item = item;
        }

        public BagItemDataModel Item { get; set; }
    }

    public class UseItemEvent : EventBase
    {
        public static string EVENT_TYPE = "UseItemEvent";

        public UseItemEvent(BagItemDataModel item)
            : base(EVENT_TYPE)
        {
            Item = item;
        }

        public BagItemDataModel Item { get; set; }
    }

    //家园建筑升级事件
    public class CityBuildingLevelupEvent : EventBase
    {
        public static string EVENT_TYPE = "CityBuildingLevelupEvent";

        public CityBuildingLevelupEvent(int areaId)
            : base(EVENT_TYPE)
        {
            AreaId = areaId;
        }

        public int AreaId;
        public int HomeExp;
    }

    //打开主界面拦截层
    public class UI_BlockMainUIInputEvent : EventBase
    {
        public static string EVENT_TYPE = "UI_BlockMainUIInputEvent";

        public UI_BlockMainUIInputEvent(float duration)
            : base(EVENT_TYPE)
        {
            Duration = duration;
        }

        public float Duration;
    }

    ////设置离线经验界面显隐
    //public class Ui_OffLineFrame_SetVisible : EventBase
    //{
    //    public static string EVENT_TYPE = "Ui_OffLineFrame_SetVisible";
    //    public bool visible;
    //    public Ui_OffLineFrame_SetVisible(bool b)
    //        : base(EVENT_TYPE)
    //    {
    //        visible = b;
    //    }
    //}

    //打开主界面拦截层
    public class UI_RefreshConstructionListEvent : EventBase
    {
        public static string EVENT_TYPE = "UI_RefreshConstructionListEvent";

        public UI_RefreshConstructionListEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    //宠物任务界面时间刷新
    public class UI_UpdatePetMissionTimeEvent : EventBase
    {
        public static string EVENT_TYPE = "UI_UpdatePetMissionTimeEvent";

        public UI_UpdatePetMissionTimeEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    //请求获得成就奖励
    public class UI_EventApplyChengJiuItem : EventBase
    {
        public static string EVENT_TYPE = "UI_ClaimAchievementRewardEvent";

        public UI_EventApplyChengJiuItem(int id)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id;
    }

    //public class Event_ItemInfoGetOperation : EventBase
    //{
    //    public static string EVENT_TYPE = "Event_ItemInfoGetOperation";
    //    public int Type { get; set; }

    //    public Event_ItemInfoGetOperation(int type)
    //        : base(EVENT_TYPE)
    //    {
    //        Type = type;
    //    }
    //}

    //请求获得成就奖励
    public class UI_PetFrame_RecycleSoulEvent : EventBase
    {
        public static string EVENT_TYPE = "UI_PetFrame_RecycleSoulEvent";

        public UI_PetFrame_RecycleSoulEvent(Opt opt, int id = -1)
            : base(EVENT_TYPE)
        {
            OpType = opt;
            Id = id;
        }

        public int Id;
        public Opt OpType;

        public enum Opt
        {
            Open = 0,
            Add,
            Minus,
            Confirm
        }
    }

    //“特长洗练”按钮被按下
    public class OnBtnRetrainEvent : EventBase
    {
        public static string EVENT_TYPE = "OnBtnRetrainEvent";

        public OnBtnRetrainEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_Answer_AnswerClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_Answer_AnswerClick";

        public UIEvent_Answer_AnswerClick(int type, int index)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_CityDungeonResult : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityDungeonResult";

        public UIEvent_CityDungeonResult(int seconds, List<int> param = null, string leaderName = "")
            : base(EVENT_TYPE)
        {
            Seconds = seconds;
            Param = param;
            LeaderName = leaderName;
        }

        public string LeaderName { get; set; }
        public List<int> Param { get; set; }
        public int Seconds { get; set; }
    }



    public class UIEvent_MieShiResult : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MieShiResult";

        public UIEvent_MieShiResult(List<int> param = null)
            : base(EVENT_TYPE)
        {
            Param = param;
        }

        public List<int> Param { get; set; }
    }


    public class UIEvent_Answer_ExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_Answer_ExdataUpdate";

        public UIEvent_Answer_ExdataUpdate(int index, int value)
            : base(EVENT_TYPE)
        {
            Index = index;
            Value = value;
        }

        public int Index { get; set; }
        public int Value { get; set; }
    }

    public class UIEvent_Sail_ExdataUpdate : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_Sail_ExdataUpdate";

        public UIEvent_Sail_ExdataUpdate(int index, int value)
            : base(EVENT_TYPE)
        {
            Index = index;
            Value = value;
        }

        public int Index { get; set; }
        public int Value { get; set; }
    }

    public class UIEvent_ShowHatchingEggList : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ShowHatchingEggList";

        public UIEvent_ShowHatchingEggList(bool show)
            : base(EVENT_TYPE)
        {
            Show = show;
        }

        public bool Show { get; set; }
    }

    public class UIEvent_SelectHatchingCell : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SelectHatchingCell";

        public UIEvent_SelectHatchingCell(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx { get; set; }
    }

    public class UIEvent_OnSelectHatchingCell : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OnSelectHatchingCell";

        public UIEvent_OnSelectHatchingCell(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx { get; set; }
    }

    public class UIEvent_HatchingFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_HatchingFlyAnim";

        public UIEvent_HatchingFlyAnim(int idx, int expCount)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            ExpCount = expCount;
        }

        public int ExpCount { get; set; }
        public int Idx { get; set; }
    }

    public class UIEvent_DoMissionGoTo : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_DoMissionGoTo";

        public UIEvent_DoMissionGoTo()
            : base(EVENT_TYPE)
        {
        }

        public int Idx { get; set; }
    }

    public class UIEvent_OnCityBuildingOptEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OnCityBuildingOptEvent";

        public UIEvent_OnCityBuildingOptEvent(int areaId, CityOperationType opt)
            : base(EVENT_TYPE)
        {
            AreaId = areaId;
            Opt = opt;
        }

        public int AreaId { get; set; }
        public CityOperationType Opt { get; set; }
    }

    public class UIEvent_CityAssignPetEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CityAssignPetEvent";

        public UIEvent_CityAssignPetEvent(int areaId)
            : base(EVENT_TYPE)
        {
            AreaId = areaId;
        }

        public int AreaId { get; set; }
    }

    public class ActivityStateChangedEvent : EventBase
    {
        public static string EVENT_TYPE = "ActivityStateChangedEvent";

        public ActivityStateChangedEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SmithyFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SmithyFlyAnim";

        public UIEvent_SmithyFlyAnim(int idx, int count)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Count = count;
        }

        public int Count { get; set; }
        public int Idx { get; set; }
        public int Index { get; set; }
    }

    public class UIEvent_PetFlyAnim : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PetFlyAnim";

        public UIEvent_PetFlyAnim(int exp)
            : base(EVENT_TYPE)
        {
            Exp = exp;
        }

        public int Exp { get; set; }
    }

    public class UIEvent_CliamReward : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_CliamActivityReward";

        public UIEvent_CliamReward(Type type, int idx = -1)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            RewardType = type;
        }

        public enum Type
        {
            OnLine = 0,
            Level,
            CheckinToday,
            ReCheckinToday,
            ClaimContinuesLoginReward,
            Activity,
            Compensate
        }

        public int Idx { get; set; }
        public Type RewardType { get; set; }
    }

    public class UIEvent_ActivityCompensateItem : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ActivityCompensateItem";

        public UIEvent_ActivityCompensateItem(int type, int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Type = type;
        }

        public int Idx { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_UseGiftCodeEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UseGiftCodeEvent";

        public UIEvent_UseGiftCodeEvent(string code)
            : base(EVENT_TYPE)
        {
            Code = code;
        }

        public string Code { get; set; }
    }

    public class UIEvent_GetOnLineSeconds : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_GetOnLienSeconds";

        public UIEvent_GetOnLineSeconds()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_UpdateOnLineSeconds : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_UpdateOnLineSeconds";

        public UIEvent_UpdateOnLineSeconds(int seconds)
            : base(EVENT_TYPE)
        {
            Seconds = seconds;
        }

        public int Seconds { get; private set; }
    }

    public class UIEvent_PlayFrameTabSelectEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PlayFrameTabSelectEvent";

        public UIEvent_PlayFrameTabSelectEvent(int idx)
            : base(EVENT_TYPE)
        {
            TabIdx = idx;
        }

        public int TabIdx;
    }

    public class UIEvent_PlayFrameRewardClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PlayFrameRewardClick";

        public UIEvent_PlayFrameRewardClick(int idx)
            : base(EVENT_TYPE)
        {
            Index = idx;
        }

        public int Index;
    }

    /// <summary>
    ///     点击参加任务按钮
    /// </summary>
    public class UIEvent_OnClickGotoActivity : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OnClickGotoActivity";

        public UIEvent_OnClickGotoActivity(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }

        public int Idx { get; set; }
    }

    public class UIEvent_SmithItemClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SmithItemClick";

        public UIEvent_SmithItemClick(int type, int index)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class RefleshMainUINoticeEvent : EventBase
    {
        public static string EVENT_TYPE = "RefleshMainUINoticeEvent";

        public RefleshMainUINoticeEvent(int id)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class UIEvent_TitleItemOption : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TitleItemOption";

        public UIEvent_TitleItemOption(int type, int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
            Type = type;
        }

        public int Idx { get; set; }
        public int Type { get; set; }
    }

    public class UIEvent_RechargeFrame_OnClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RechargeFrame_OnClick";

        public UIEvent_RechargeFrame_OnClick(int idx, object exdata = null)
            : base(EVENT_TYPE)
        {
            index = idx;
            exData = exdata;
        }

        public object exData { get; set; }
        public int index { get; set; }
    }

    public class RechargeSuccessEvent : EventBase
    {
        public static string EVENT_TYPE = "RechargeSuccessEvent";

        public RechargeSuccessEvent(int id)
            : base(EVENT_TYPE)
        {
            RechargeId = id;
        }

        public int RechargeId;
    }

    public class VipLevelChangedEvent : EventBase
    {
        public static string EVENT_TYPE = "VipLevelChangedEvent";

        public VipLevelChangedEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_ShowPlantDemo : EventBase
    {
        public static string EVENT_TYPE = "ShowPlantDemo";

        public UIEvent_ShowPlantDemo()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RewardShortcutClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RewardShortcutClick";

        public UIEvent_RewardShortcutClick()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RefleshNameBoard : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RefleshNameBoard";

        public UIEvent_RefleshNameBoard()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_PickSettingChanged : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_PickSettingChanged";

        public UIEvent_PickSettingChanged()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_RefreshPush : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_RefreshPush";

        public UIEvent_RefreshPush(int pushId, double timeInterval)
            : base(EVENT_TYPE)
        {
            id = pushId;
            time = timeInterval;
        }

        public int id { get; set; }
        public double time { get; set; }
    }

    public class UIEvent_QualitySetting : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_QualitySetting";

        public UIEvent_QualitySetting(int index)
            : base(EVENT_TYPE)
        {
            level = index;
        }

        public int level { get; set; }
    }

    public class UIEvent_ResolutionSetting : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ResolutionSetting";

        public UIEvent_ResolutionSetting(int index)
            : base(EVENT_TYPE)
        {
            level = index;
        }

        public int level { get; set; }
    }

    public class UIEvent_VisibleEyeClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_VisibleEyeClick";

        public UIEvent_VisibleEyeClick(bool visible)
            : base(EVENT_TYPE)
        {
            Visible = visible;
        }
        public bool Visible { get; set; }
    }


    public class UIEvent_MissionTipEvent : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_MissionTipEvent";

        public UIEvent_MissionTipEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SevenRewardInit : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SevenRewardInit";

        public UIEvent_SevenRewardInit()
            : base(EVENT_TYPE)
        {
        }
    }

    public class UIEvent_SevenRewardItemClick : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SevenRewardItemClick";

        public UIEvent_SevenRewardItemClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class UIEvent_SyncLevelUpAttrChange : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_SyncLevelUpAttrChange";

        public UIEvent_SyncLevelUpAttrChange(LevelUpAttrData attrData)
            : base(EVENT_TYPE)
        {
            AttrData = attrData;
        }

        public LevelUpAttrData AttrData { get; set; }
    }

    public class StrongCellClickedEvent : EventBase
    {
        public static string EVENT_TYPE = "StrongCellClickedEvent";

        public StrongCellClickedEvent(int idx)
            : base(EVENT_TYPE)
        {
            Index = idx;
        }

        public int Index;
    }

    public class UIEvent_StrongSetGridLookIndex : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_StrongSetGridLookIndex";

        public UIEvent_StrongSetGridLookIndex(int type, int index)
            : base(EVENT_TYPE)
        {
            Index = index;
            Type = type;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class BattleMishiRefreshModelMaster : EventBase
    {
        public static string EVENT_TYPE = "BattleMishiRefreshModelMaster";

        public BattleMishiRefreshModelMaster(PlayerInfoMsg id)
            : base(EVENT_TYPE)
        {
            idMaster = id;
        }

        public PlayerInfoMsg idMaster { get; set; }
    }
    public class BattleUnionRefreshModelView : EventBase
    {
        public static string EVENT_TYPE = "BattleUnionRefreshModelView";

        public BattleUnionRefreshModelView(PlayerInfoMsg info)
            : base(EVENT_TYPE)
        {
            Info = info;
        }

        public PlayerInfoMsg Info { get; set; }
    }
    public class BattleUnionRefreshModelViewLogic : EventBase
    {
        public static string EVENT_TYPE = "BattleUnionRefreshModelViewLogic";

        public BattleUnionRefreshModelViewLogic(PlayerInfoMsg info)
            : base(EVENT_TYPE)
        {
            Info = info;
        }

        public PlayerInfoMsg Info { get; set; }
    }

    public class BattleUnionCountChange : EventBase
    {
        public static string EVENT_TYPE = "BattleUnionCountChange";

        public BattleUnionCountChange(int t, int i)
            : base(EVENT_TYPE)
        {
            Type = t;
            Index = i;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class BattleUnionSyncOccupantChange : EventBase
    {
        public static string EVENT_TYPE = "BattleUnionSyncOccupantChange";

        public BattleUnionSyncOccupantChange(AllianceWarOccupantData data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public AllianceWarOccupantData Data { get; set; }
    }

    public class BattleUnionSyncChallengerDataChange : EventBase
    {
        public static string EVENT_TYPE = "BattleUnionSyncChallengerDataChange";

        public BattleUnionSyncChallengerDataChange(AllianceWarChallengerData data)
            : base(EVENT_TYPE)
        {
            Data = data;
        }

        public AllianceWarChallengerData Data { get; set; }
    }

    public class GuardStateChange : EventBase
    {
        public static string EVENT_TYPE = "GuardStateChange";

        public GuardStateChange(List<int> lists, int reliveCount)
            : base(EVENT_TYPE)
        {
            Lists = lists;
            ReliveCount = reliveCount;
        }

        public List<int> Lists { get; set; }
        public int ReliveCount { get; set; }
    }

    public class GuardItemOperation : EventBase
    {
        public static string EVENT_TYPE = "GuardItemOperation";

        public GuardItemOperation(int type, int index)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class GuardUIOperation : EventBase
    {
        public static string EVENT_TYPE = "GuardUIOperation";

        public GuardUIOperation(int type, int index)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class RechageActivityOperation : EventBase
    {
        public static string EVENT_TYPE = "RechageActivityOperation";

        public RechageActivityOperation(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class RechageActivityRewardOperation : EventBase
    {
        public static string EVENT_TYPE = "RechageActivityRewardOperation";

        public RechageActivityRewardOperation(int type, int index)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class RechageActivityInvestmentOperation : EventBase
    {
        public static string EVENT_TYPE = "RechageActivityInvestmentOperation";

        public RechageActivityInvestmentOperation(int type, int index)
            : base(EVENT_TYPE)
        {
            Type = type;
            Index = index;
        }

        public int Index { get; set; }
        public int Type { get; set; }
    }

    public class RechageActivityInitTables : EventBase
    {
        public static string EVENT_TYPE = "RechageActivityInitTables";

        public RechageActivityInitTables()
            : base(EVENT_TYPE)
        {
        }
    }

    public class RechageActivityMenuItemClick : EventBase
    {
        public static string EVENT_TYPE = "RechageActivityMenuItemClick";

        public RechageActivityMenuItemClick(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index { get; set; }
    }

    public class RewardMessageOpetionClick : EventBase
    {
        public static string EVENT_TYPE = "RewardMessageOpetionClick";

        public RewardMessageOpetionClick(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class MapNpcInfoEvent : EventBase
    {
        public static string EVENT_TYPE = "MapNpcInfoEvent";

        public MapNpcInfoEvent(MapNpcInfos info)
            : base(EVENT_TYPE)
        {
            Info = info;
        }

        public MapNpcInfos Info { get; set; }
    }

    public class WorshipOpetion : EventBase
    {
        public static string EVENT_TYPE = "WorshipOpetion";

        public WorshipOpetion(int type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public int Type { get; set; }
    }

    public class WorshipRefreshModelView : EventBase
    {
        public static string EVENT_TYPE = "WorshipRefreshModelView";

        public WorshipRefreshModelView(PlayerInfoMsg info)
            : base(EVENT_TYPE)
        {
            Info = info;
        }

        public PlayerInfoMsg Info { get; set; }
    }

    public class SceneTransition_Event : EventBase
    {
        public static string EVENT_TYPE = "SceneTransition_Event";

        public SceneTransition_Event()
            : base(EVENT_TYPE)
        {
        }
    }

    public class FirstRechargeTextSet_Event : EventBase
    {
        public static string EVENT_TYPE = "FirstRechargeTextSet_Event";

        public FirstRechargeTextSet_Event(string str)
            : base(EVENT_TYPE)
        {
            Str = str;
        }

        public string Str { get; set; }
    }

    public class FirstChargeBtnClick_Event : EventBase
    {
        public static string EVENT_TYPE = "FirstChargeBtnClick_Event";

        public FirstChargeBtnClick_Event()
            : base(EVENT_TYPE)
        {
        }
    }

    public class FirstChargeCloseBtnClick_Event : EventBase
    {
        public static string EVENT_TYPE = "FirstChargeCloseBtnClick_Event";

        public FirstChargeCloseBtnClick_Event()
            : base(EVENT_TYPE)
        {
        }
    }

    public class FirstChargeGetItemSuccess_Event : EventBase
    {
        public static string EVENT_TYPE = "FirstChargeGetItemSuccess_Event";

        public FirstChargeGetItemSuccess_Event()
            : base(EVENT_TYPE)
        {
        }
    }
    public class FirstChargeToggleSuccess_Event : EventBase
    {
        public static string EVENT_TYPE = "FirstChargeToggleSuccess_Event";

        public FirstChargeToggleSuccess_Event(int idx)
            : base(EVENT_TYPE)
        {
            index = idx;
        }

        public int index = 0;
    }

    public class UpdateFuBenStoreStore_Event : EventBase
    {
        public static string EVENT_TYPE = "UpdateFuBenStoreStore_Event";

        public UpdateFuBenStoreStore_Event(StoneItems itemlst, int storeType)
            : base(EVENT_TYPE)
        {
            Items = itemlst;
            mStoreType = storeType;
        }

        public StoneItems Items { get; set; }
        public int mStoreType { get; set; }
    }

    public class ActivityClose_Event : EventBase
    {
        public static string EVENT_TYPE = "ActivityClose_Event";

        public ActivityClose_Event()
            : base(EVENT_TYPE)
        {
        }
    }

    public class GXCortributionRank_Event : EventBase
    {
        public static string EVENT_TYPE = "GXCortributionRank_Event";

        public GXCortributionRank_Event(ContriRankingData rankList) : base(EVENT_TYPE)
        {
            RankData = rankList;
        }

        public ContriRankingData RankData { get; set; }
    }

    public class ModelDisplay_Equip_Event : EventBase
    {
        public static string EVENT_TYPE = "ModelDisplay_Equip_Event";

        public ModelDisplay_Equip_Event(int itemId)
            : base(EVENT_TYPE)
        {
            ItemId = itemId;
        }

        public int ItemId { get; set; }
    }
    

    public class ModelDisplay_ShowModel_Event : EventBase
    {
        public static string EVENT_TYPE = "ModelDisplay_ShowModel_Event";

        public ModelDisplay_ShowModel_Event(int equipId, int type)
            : base(EVENT_TYPE)
        {
            EquipId = equipId;
            UIType = type;
        }

        public int EquipId { get; set; }
        public int UIType { get; set; }
    }

    public class FirstChargeModelDisplay_Event : EventBase
    {
        public static string EVENT_TYPE = "FirstChargeModelDisplay_Event";

        public FirstChargeModelDisplay_Event(string path)
            : base(EVENT_TYPE)
        {
            PerfabPath = path;
        }

        public string PerfabPath { get; set; }
    }

    public class ShowItemHint : EventBase
    {
        public static string EVENT_TYPE = "ShowItemHint";

        public ShowItemHint(int itemId, int bagIndex)
            : base(EVENT_TYPE)
        {
            ItemId = itemId;
            BagIndex = bagIndex;
        }

        public int ItemId { get; set; }
        public int BagIndex { get; set; }
    }

	public class MyPlayerMoveBegin : EventBase
	{
		public static string EVENT_TYPE = "MyPlayerMoveBegin";

		public MyPlayerMoveBegin()
            : base(EVENT_TYPE)
        {
        }
	}

	public class OperationActivityPageClickEvent : EventBase
	{
		public static string EVENT_TYPE = "OperationActivityPageClick";
		public int Idx;
		public OperationActivityPageClickEvent(int idx)
			: base(EVENT_TYPE)
		{
			Idx = idx;
		}
	}

	public class OperationActivitySubPageClickEvent : EventBase
	{
		public static string EVENT_TYPE = "OperationActivitySubPageClickEvent";
		public int Idx;
		public OperationActivitySubPageClickEvent(int idx)
			: base(EVENT_TYPE)
		{
			Idx = idx;
		}
	}

	public class OperationActivityClaimReward : EventBase
	{
		public static string EVENT_TYPE = "OperationActivityClaimReward";
		public int Id;
		public OperationActivityClaimReward(int id)
			: base(EVENT_TYPE)
		{
			Id = id;
		}
	}

	public class SyncOperationActivityItemEvent : EventBase
	{
		public static string EVENT_TYPE = "SyncOperationActivityItem";
		public MsgOperActivtyItem Data;
		public SyncOperationActivityItemEvent(MsgOperActivtyItem data)
			: base(EVENT_TYPE)
		{
			Data = data;
		}
	}

	public class SyncOperationActivityTermEvent : EventBase
	{
		public static string EVENT_TYPE = "SyncOperationActivityTerm";
		public int Id;
		public int Param;
		public SyncOperationActivityTermEvent(int id, int param)
			: base(EVENT_TYPE)
		{
			Id = id;
			Param = param;
		}
	}

    public class FirstEnterGameEvent : EventBase
    {
        public static string EVENT_TYPE = "FirstEnterGameEvent";
        public FirstEnterGameEvent(bool type)
            : base(EVENT_TYPE)
        {
            Type = type;
        }

        public bool Type;
    }

    public class WingChargeCloseBtnClick_Event : EventBase
    {
        public static string EVENT_TYPE = "WingChargeCloseBtnClick_Event";

        public WingChargeCloseBtnClick_Event()
            : base(EVENT_TYPE)
        {

        }
    }

    public class WingChargeItemClick_Event : EventBase
    {
        public static string EVENT_TYPE = "WingChargeItemClick_Event";

        public WingChargeItemClick_Event(int idx)
            : base(EVENT_TYPE)
        {
            index = idx;
        }

        public int index;
    }

    public class WingChargeBuyClick_Event : EventBase
    {
        public static string EVENT_TYPE = "WingChargeBuyClick_Event";

        public WingChargeBuyClick_Event()
            : base(EVENT_TYPE)
        {
        }
    }
	public class OperationActivityPage_Event : EventBase
	{
		public static string EVENT_TYPE = "OperationActivityPage_Event";
		public int Idx;
		public OperationActivityPage_Event(int idx)
			: base(EVENT_TYPE)
		{
			Idx=idx;
		}
	}

	
	public class OperationActivitySubPagekEvent : EventBase
	{
		public static string EVENT_TYPE = "OperationActivitySubPagekEvent";
		public int Idx;
		public OperationActivitySubPagekEvent(int idx)
			: base(EVENT_TYPE)
		{
			Idx = idx;
		}
	}

	public class CreateRole_DragEvent : EventBase
	{
		public static string EVENT_TYPE = "CreateRole_DragEvent";
		public Vector2 Delta;
		public CreateRole_DragEvent(Vector2 delta)
			: base(EVENT_TYPE)
		{
			Delta = delta;
		}
	}
	  public class FubenGXCortributionRank_Event : EventBase
    {
        public static string EVENT_TYPE = "FubenGXCortributionRank_Event";

        public FubenGXCortributionRank_Event(ContriRankingData rankList)
            : base(EVENT_TYPE)
        {
            RankData = rankList;
        }

        public ContriRankingData RankData { get; set; }
    }
    public class MieShiGetInfo_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiGetInfo_Event";
        public MieShiGetInfo_Event()
            : base(EVENT_TYPE)
        {

        }

    }

    public class PickUpNpc_Event : EventBase
    {
        public static string EVENT_TYPE = "PickUpNpc_Event";
        public int idNpc { get; set; }
        public ulong ObjId { get; set; }
        public PickUpNpc_Event(int id,ulong InstanceId)
            : base(EVENT_TYPE)
        {
            idNpc = id;
            ObjId = InstanceId;
        }
    }
    public class ApplyPortraitAward_Event : EventBase
    {
        public static string EVENT_TYPE = "ApplyPortraitAward_Event";
        public int idNpc { get; set; }
        public ApplyPortraitAward_Event(int id)
            : base(EVENT_TYPE)
        {
            idNpc = id;
        }
    }
	
	    public class MieShiSetActivityId_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiSetActivityId_Event";
        public int ActivityID { get; set; }
        public MieShiSetActivityId_Event(int id)
            : base(EVENT_TYPE)
        {
            ActivityID = id;
        }
    }

    public class ApplyMishiPortrait_Event : EventBase
    {
        public static string EVENT_TYPE = "ApplyMishiPortrait_Event";
        public ApplyMishiPortrait_Event()
            : base(EVENT_TYPE)
        {
        }
    }



    public class MonsterSiegeUpLevelBtn_Event : EventBase
    {
        public static string EVENT_TYPE = "MonsterSiegeUpLevelBtn_Event";
        public int ID { get; set; }
        public int BtnID { get; set; }
        public MonsterSiegeUpLevelBtn_Event(int id, int UpLevelBtnID)
            : base(EVENT_TYPE)
        {
            ID = id;
            BtnID = UpLevelBtnID;
        }
    }
    public class BattryLevelUpUpLevelBtn_Event : EventBase
    {
        public static string EVENT_TYPE = "BattryLevelUpUpLevelBtn_Event";
        public int ID { get; set; }
        public int BtnID { get; set; }
        public BattryLevelUpUpLevelBtn_Event(int id, int UpLevelBtnID)
            : base(EVENT_TYPE)
        {
            ID = id;
            BtnID = UpLevelBtnID;
        }
    }

    public class MieShiUpHpBtn_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiUpHpBtn_Event";
        public MieShiUpHpBtn_Event()
            : base(EVENT_TYPE)
        {

        }
    }

    public class MieShiOnPaotaiBtn_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiOnPaotaiBtn_Event";

        public MieShiOnPaotaiBtn_Event()
            : base(EVENT_TYPE)
        {
        }
    }
    public class MieShiOnGXRankingBtn_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiOnGXRankingBtn_Event";

        
        public MieShiOnGXRankingBtn_Event()
            : base(EVENT_TYPE)
        {
            
        }
    }

        public class UpdateMieShiOnGXRankingData_Event : EventBase
    {
        public static string EVENT_TYPE = "UpdateMieShiOnGXRankingData_Event";
        public ContriRankingData RankData { get; set; }
        public UpdateMieShiOnGXRankingData_Event(ContriRankingData data)
            : base(EVENT_TYPE)
        {
            RankData = data;
        }
    }

    public class MieShiOnYibaomingBtn_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiOnYibaomingBtn_Event";

        public MieShiOnYibaomingBtn_Event()
            : base(EVENT_TYPE)
        {
        }
    }

    public class MieShiDisappearModelRoot_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiDisappearModelRoot_Event";

        public bool bDisappear;
        public MieShiDisappearModelRoot_Event(bool bDis)
            : base(EVENT_TYPE)
        {
            bDisappear = bDis;
        }
    }
    //灭世战斗结果点击事件
    public class MieShiShowFightingResult_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiShowFightingResult_Event";

        public bool bDisappear;
        public MieShiShowFightingResult_Event()
            : base(EVENT_TYPE)
        {
           
        }
    }

    //灭世战斗结果点击事件
    public class MieShiShowSkillTip_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiShowSkillTip_Event";
        public MieShiShowSkillTip_Event()
            : base(EVENT_TYPE)
        {

        }
    }

    public class MieShiCLosePage_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiCLosePage_Event";
        public MieShiCLosePage_Event()
            : base(EVENT_TYPE)
        {

        }
    }

    public class MieShiShowRankingReward_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiShowRankingReward_Event";
        public MieShiShowRankingReward_Event()
            : base(EVENT_TYPE)
        {

        }
    }

    public class MieShiShowRules_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiShowRules_Event";
        public MieShiShowRules_Event()
            : base(EVENT_TYPE)
        {

        }
    }

    //灭世战斗结果点击事件
    public class MieShiAddActvityTime_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiAddActvityTime_Event";
        public int iType;
        public DateTime activityTime;
        public MieShiAddActvityTime_Event(int type,DateTime time)
            : base(EVENT_TYPE)
        {
            activityTime = time;
            iType = type;
        }
    }

    //灭世战斗结果点击事件
    public class MieShiUiToggle_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiUiToggle_Event";
        public int idxUIToggle;
        public MieShiUiToggle_Event(int UIToggle)
            : base(EVENT_TYPE)
        {
            idxUIToggle = UIToggle;
        }
    }

    //灭世刷新boss事件
    public class MieShiRefreshBoss_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiRefreshBoss_Event";
        public int idBosd;
        public MieShiRefreshBoss_Event(int BossId)
            : base(EVENT_TYPE)
        {
            idBosd = BossId;
        }
    }

    //灭世战斗结果点击事件
    public class MieShiRefreshTowers_Event : EventBase
    {
        public static string EVENT_TYPE = "MieShiRefreshTowers_Event";
        public MieShiRefreshTowers_Event()
            : base(EVENT_TYPE)
        {

        }
    }
    //隐藏灭世图标
    public class HiedMieShiIcon_Event : EventBase
    {
        public static string EVENT_TYPE = "HiedMieShiIcon_Event";
        public HiedMieShiIcon_Event()
            : base(EVENT_TYPE)
        {

        }
    }
    public class ExChange_Event : EventBase
    {
        public static string EVENT_TYPE = "ExChange_Event";
        public ExChange_Event()
            : base(EVENT_TYPE)
        {

        }
    }
    public class ExChangeInit_Event : EventBase
    {
        public static string EVENT_TYPE = "ExChangeInit_Event";
        public ExChangeInit_Event()
            : base(EVENT_TYPE)
        {

        }
    }
    public class OnTouZiBtnClick_Event : EventBase
    {
        public static string EVENT_TYPE = "OnTouZiBtnClick_Event";

        public OnTouZiBtnClick_Event(int id)
            : base(EVENT_TYPE)
        {
            TableId = id;
        }

        public int TableId;
    }

    public class ShowPopTalk_Event : EventBase
    {
        public static string EVENT_TYPE = "ShowPopTalk_Event";
        public int id;
        public string talk;
        public ShowPopTalk_Event(string Talk)
            : base(EVENT_TYPE)
        {
            
            talk = Talk;
        }
    }
    public class ShowComposFlag_Event : EventBase
    {
        public static string EVENT_TYPE = "ShowComposFlag_Event";
       
        public ShowComposFlag_Event()
            : base(EVENT_TYPE)
        {
           
        }
    }
    public class QuickBuyOperaEvent : EventBase
    {
        public static string EVENT_TYPE = "QuickBuyOperaEvent";

        public QuickBuyOperaEvent(int t)
            : base(EVENT_TYPE)
        {
            Type = t;
        }

        public int Type { get; set; }
    }

    public class ShowItemsFrameEffectEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowItemsFrameEffectEvent";

        public ShowItemsFrameEffectEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class OnCLickGoToActivityByMainUIEvent : EventBase
    {
        public static string EVENT_TYPE = "OnCLickGoToActivityByMainUIEvent";

        public OnCLickGoToActivityByMainUIEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class ShowCharacterInMinimap : EventBase
    {
        public static string EVENT_TYPE = "ShowCharacterInMinimap";

        public ShowCharacterInMinimap(bool show, ulong charId)
            : base(EVENT_TYPE)
        {
            Show = show;
            CharId = charId;
        }

        public bool Show;
        public ulong CharId;
    }

    public class ShowSceneMapEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowSceneMapEvent";

        public ShowSceneMapEvent(int sceneid)
            : base(EVENT_TYPE)
        {
            SceneId = sceneid;
        }

        public int SceneId;
    }

    public class MieshiResultEvent : EventBase
    {
        public static string EVENT_TYPE = "MieshiResultEvent";

        public MieshiResultEvent(MieshiResultMsg _msg)
            : base(EVENT_TYPE)
        {
            msg = _msg;
        }
        public MieshiResultMsg msg { get; set; }
    }
	public class OperationActivityDrawLotteryEvent : EventBase
	{
		public static string EVENT_TYPE = "OperationActivityDrawLotteryEvent";
		public int Idx { get; set; }
		public int ItemId { get; set; }
		public int ItemCount { get; set; }
		public OperationActivityDrawLotteryEvent(int idx, int itemId=-1, int itemCount=0)
			: base(EVENT_TYPE)
		{
			Idx = idx;
			ItemId = itemId;
			ItemCount = itemCount;
		}
		
	}

    public class ShowFastReachEvent : EventBase
    {
        public static string EVENT_TYPE = "ShowFastReachEvent";

        public ShowFastReachEvent(bool ishow)
            : base(EVENT_TYPE)
        {
            IsShow = ishow;
        }

        public bool IsShow { get; set; }
    }

    public class ClickReachBtnEvent : EventBase
    {
        public static string EVENT_TYPE = "ClickReachBtnEvent";

        public ClickReachBtnEvent()
            : base(EVENT_TYPE)
        {
        }
    }
    public class UIEvent_ClickChest : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ClickChest";

        public UIEvent_ClickChest(int idx)
            : base(EVENT_TYPE)
        {
            TabIdx = idx;
        }

        public int TabIdx;
        public string From = string.Empty;
        public BagItemDataModel BagDataModel { get; set; }
    }
    public class UIEvent_ClickChestItem : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ClickChestItem";

        public UIEvent_ClickChestItem(int idx)
            : base(EVENT_TYPE)
        {
            TabIdx = idx;
        }
        public int TabIdx;
    }
    public class UIEvent_OpenChest : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_OpenChest";

        public UIEvent_OpenChest()
            : base(EVENT_TYPE)
        {

        }

    }

    public class UIEvent_ClickTowerReward : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_ClickTowerReward";

        public UIEvent_ClickTowerReward(int idx)
            : base(EVENT_TYPE)
        {
            Index = idx;
        }

        public int Index;
    }
    public class UIEvent_TowerRewardCallBack : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_TowerRewardCallBack";

        public UIEvent_TowerRewardCallBack(int _type,int _param =0)
            : base(EVENT_TYPE)
        {
            nType = _type;
            nParam = _param;
        }

        public int nType;
        public int nParam;
    }

    public class ArtifactOpEvent : EventBase
    {
        public static string EVENT_TYPE = "ArtifactOpEvent";
        public ArtifactOpEvent(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }
        public int Idx { get; set; }
    }

    public class ArtifactSelectEvent : EventBase
    {
        public static string EVENT_TYPE = "ArtifactSelectEvent";
        public ArtifactSelectEvent(ListItemLogic item)
            : base(EVENT_TYPE)
        {
            ListItem = item;
        }
        public ListItemLogic ListItem { get; set; }
    }

    public class MyArtifactOpEvent : EventBase
    {
        public static string EVENT_TYPE = "MyArtifactOpEvent";
        public MyArtifactOpEvent(int idx)
            : base(EVENT_TYPE)
        {
            Idx = idx;
        }
        public int Idx { get; set; }
    }

    public class MyArtifactShowEquipEvent : EventBase
    {
        public static string EVENT_TYPE = "MyArtifactShowEquipEvent";

        public MyArtifactShowEquipEvent(int equipId)
            : base(EVENT_TYPE)
        {
            EquipId = equipId;
        }

        public int EquipId { get; set; }
    }
    public class UIEvent_DungeonReward : EventBase
    {
        public static string EVENT_TYPE = "UIEvent_DungeonReward";

        public UIEvent_DungeonReward(DrawResult _reward)
            : base(EVENT_TYPE)
        {
            reward  = _reward;
        }

        public DrawResult reward { set; get; }
    }
    public class OnClickFastReachMessageBoxOKEvent : EventBase
    {
        public static string EVENT_TYPE = "OnClickFastReachMessageBoxOKEvent";
        public OnClickFastReachMessageBoxOKEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class OnClickFastReachMessageBoxCancleEvent : EventBase
    {
        public static string EVENT_TYPE = "OnClickFastReachMessageBoxCancleEvent";
        public OnClickFastReachMessageBoxCancleEvent()
            : base(EVENT_TYPE)
        {
        }
    }

    public class EnableFrameEvent : EventBase
    {
        public static string EVENT_TYPE = "EquipmentInfoEvent";
        public EnableFrameEvent(int id)
            : base(EVENT_TYPE)
        {
            Id = id;
        }

        public int Id;
    }

    public class ArtifactSelectModelEvent : EventBase
    {
        public static string EVENT_TYPE = "ArtifactSelectModelEvent";
        public ArtifactSelectModelEvent(int index)
            : base(EVENT_TYPE)
        {
            Index = index;
        }

        public int Index;
    }

	public class OperationActivityDataInitEvent : EventBase
	{
		public static string EVENT_TYPE = "OperationActivityDataInitEvent";
		public MsgOperActivty Msg;
		public OperationActivityDataInitEvent(MsgOperActivty msg)
			: base(EVENT_TYPE)
		{
			Msg = msg;
		}
	}

    public class OnClickBuyTiliEvent : EventBase
    {
        public static string EVENT_TYPE = "OnClickBuyTiliEvent";
        public OnClickBuyTiliEvent(int type)
            : base(EVENT_TYPE)
        {
            mType = type;
        }

        public int mType { get; set; }
    }
    public class TowerFloorClickEvent : EventBase
    {
        public static string EVENT_TYPE = "TowerFloorClickEvent";
        public TowerFloorClickEvent(int idx)
            : base(EVENT_TYPE)
        {
            nIndex = idx;
        }
        public int nIndex { get; set; }
    }
    public class TowerBtnClickEvent : EventBase
    {
        public static string EVENT_TYPE = "TowerBtnClickEvent";
        public TowerBtnClickEvent(int _type)
            : base(EVENT_TYPE)
        {
            nType = _type;
        }
        public int nType { get; set; }
    }

	public class UIAcientBattleFieldMenuItemClickEvent : EventBase
	{
		public static string EVENT_TYPE = "UIAcientBattleFieldMenuItem";
		public int Idx;
		public UIAcientBattleFieldMenuItemClickEvent(int idx)
			: base(EVENT_TYPE)
		{
			Idx = idx;
		}
	}

    public class ExitFuBenWithOutMessageBoxEvent : EventBase
    {
        public static string EVENT_TYPE = "ExitFuBenWithOutMessageBoxEvent";

        public ExitFuBenWithOutMessageBoxEvent()
            : base(EVENT_TYPE)
        {
        }
    }
}
