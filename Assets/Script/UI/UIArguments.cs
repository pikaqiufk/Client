#region using

using System;
using System.Collections.Generic;
using ClientDataModel;
using DataContract;
using DataTable;

#endregion

public class UIInitArguments
{
    public List<int> Args;
    public int Tab;
}

public class UIAchievementFrameArguments : UIInitArguments
{
}

public class UIAchievementTipArguments : UIInitArguments
{
}

public class ActivityArguments : UIInitArguments
{
}

public class GXRankingArguments : UIInitArguments
{
}
public class ActivityRewardArguments : UIInitArguments
{
    public eDungeonCompleteType CompleteType;
    public int FubenId;
    public int PlayerLevel;
    public int Seconds;
}

public class BossRewardArguments : UIInitArguments
{
    public eDungeonCompleteType CompleteType;
    public List<ItemBaseData> Items;
    public int Seconds;
}

public class ArenaArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class ArenaResultArguments : UIInitArguments
{
    public P1vP1RewardData RewardData;
}

public class AstrologyArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class AttributeArguments : UIInitArguments
{
}

public class AttriFrameArguments : UIInitArguments
{
}

public class BackPackArguments : UIInitArguments
{
}

public class BattleArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class BattleResultArguments : UIInitArguments
{
    public int BattleResult;
    public int DungeonId;
    public int First;
}

public class BattleUnionArguments : UIInitArguments
{
}

public class CharacterArguments : UIInitArguments
{
}

public class ChatMainArguments : UIInitArguments
{
    public BagItemDataModel ItemDataModel;
    public int Type; //0物品链接 1 位置
}

public class SmithyFrameArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class UICityArguments : UIInitArguments
{
}

public class UIHatchingHouseArguments : UIInitArguments
{
    public BuildingData BuildingData;
    public int ItemId = -1;
}

public class UIPetFrameArguments : UIInitArguments
{
    public int PetIdx = -1;
}

public class ComposeArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class BlockLayerArguments : UIInitArguments
{
    public int Type;
}

public class CrossBind_IContArguments : UIInitArguments
{
}

public class ScriptableArguments : UIInitArguments
{
}

public class DepotArguments : UIInitArguments
{
}

public class DialogFrameArguments : UIInitArguments
{
}

public class DungeonArguments : UIInitArguments
{
}

public class DungeonResultArguments : UIInitArguments
{
    public int DrawId;
    public int DrawIndex;
    public int FubenId;
    public ItemBaseData ItemBase;
    public int Second;
}

public class ElfArguments : UIInitArguments
{
}

public class ElfInfoArguments : UIInitArguments
{
    public int CharLevel = -1;
    public ElfItemDataModel DataModel;
    public int ItemId = -1;
    public bool ShowButton;
}

public class EquipInfoArguments : UIInitArguments
{
    public int ItemId;
    public int Type;
}

public class EquipUIArguments : UIInitArguments
{
    public BagItemDataModel Data;
    public int ResourceType; // 0 背包 1身上
}

public class EquipCompareArguments : UIInitArguments
{
    public int CharLevel = -1;
    public BagItemDataModel Data;
    public int ResourceType; // 0 背包 1身上
    public eEquipBtnShow ShowType = eEquipBtnShow.OperateBag;
}

public class EquipFrameArguments : UIInitArguments
{
    public BagItemDataModel ItemDataModel;
}

public class EquipPackArguments : UIInitArguments
{
    public BagItemDataModel DataModel;
    public bool RefreshForEvoEquip;
}

public class FarmArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class ForceArguments : UIInitArguments
{
    public int NewValue;
    public int OldValue;
}

public class FriendArguments : UIInitArguments
{
    public OperationListData Data;
    public int Type = 0;
}

public class GainItemHintArguments : UIInitArguments
{
    public int BagIndex;
    public int ItemId;
}

public class UINewbieGuideArguments : UIInitArguments
{
}

public class HandBookArguments : UIInitArguments
{
}

public class ItemInfoArguments : UIInitArguments
{
    public BagItemDataModel DataModel;
    public int ItemId;
    public int ShowType;
}

public class MailArguments : UIInitArguments
{
}

public class MainUIArguments : UIInitArguments
{
}

public class MainUIbtnArguments : UIInitArguments
{
}

public class MessageBoxArguments : UIInitArguments
{
}

public class MissionFrameArguments : UIInitArguments
{
}

public class MissionListArguments : UIInitArguments
{
    public int MissionId = -1;
	public bool IsFromMisson = false;
}

public class UIMissionTrackListArguments : UIInitArguments
{
}

public class OperationlistArguments : UIInitArguments
{
    public OperationListData Data;
}

public class PlayerInfoArguments : UIInitArguments
{
    public PlayerInfoMsg PlayerInfoMsg;
}

public class RankArguments : UIInitArguments
{
}

public class RebornArguments : UIInitArguments
{
}

public class RecycleArguments : UIInitArguments
{
    public BagItemDataModel ItemDataModel;
}

public class ReliveArguments : UIInitArguments
{
    public DateTime FreeTime;
    public string KillerName;
}

public class UIRewardFrameArguments : UIInitArguments
{
}

public class MedalInfoArguments : UIInitArguments
{
    public MedalInfoDataModel MedalInfoData;
}

public class SailingArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class SceneMapArguments : UIInitArguments
{
}

public class OffLineExpArguments : UIInitArguments
{
}

public class SystemNoticeArguments : UIInitArguments
{
    public string NoticeInfo;
}

public class SelectEquipArguments : UIInitArguments
{
    public int nBagId = 0;
    public int nColorId = -1;
    public int nNeedLevel = -1;
    public int nTypeId = -1;

    public List<int> GetItemList()
    {
        var temp = new List<int>();
        BagBaseDataModel bag;
        if (PlayerDataManager.Instance.PlayerDataModel.Bags.Bags.TryGetValue(nBagId, out bag))
        {
            {
                // foreach(var item in bag.Items)
                var __enumerator1 = (bag.Items).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var item = __enumerator1.Current;
                    {
                        if (item.ItemId == -1)
                        {
                            continue;
                        }
                        var tbItem = Table.GetItemBase(item.ItemId);
                        if (nColorId != -1 && nColorId != tbItem.Quality)
                        {
                            continue;
                        }
                        if (nNeedLevel != -1 && nNeedLevel != tbItem.UseLevel)
                        {
                            continue;
                        }
                        //var tbEquip = Table.GetEquipBase(tbItem.Exdata[0]);
                        if (nTypeId != -1 && nTypeId != tbItem.Type)
                        {
                            continue;
                        }
                        temp.Add(item.Index);
                        //items.Add(item.ItemId);
                        //SelectItemDataModel temp = new SelectItemDataModel()
                        //{
                        //    item = item,
                        //};
                        //DataModel.Items.Add(temp);
                    }
                }
            }
        }
        return temp;
    }
}

public class SelectRoleArguments : UIInitArguments
{
    public List<CharacterSimpleInfo> CharacterSimpleInfos;
    public ulong SelectId;
    public string ServerName;
	public enum OptType
	{
		SelectMyRole,
		CreateNewRole
	}
	public OptType Type = OptType.SelectMyRole;
}

public class ServerListArguments : UIInitArguments
{
    public ServerListData Data;
}

public class SettingArguments : UIInitArguments
{
}

public class SkillFrameArguments : UIInitArguments
{
    public int ShowType;
}

public class SkillTipFrameArguments : UIInitArguments
{
    public int idSkill;
    public int idNextSkill;
    public int iLevel;
}

public class StoreArguments : UIInitArguments
{
}

public class TeamFrameArguments : UIInitArguments
{
}

public class TradingArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class WingArguments : UIInitArguments
{
}

public class WishingArguments : UIInitArguments
{
    public BuildingData BuildingData;
}

public class WingInfogArguments : UIInitArguments
{
    public int CharLevel = -1;
    public BagItemDataModel ItemData;
}

public class ItemInfoGetArguments : UIInitArguments
{
    public BagItemDataModel ItemData;
    public int ItemId = -1;
}

public class DungeonRewardArguments : UIInitArguments
{
    public int FubenId;
    public int Seconds;
}

public class CustomShopFrameArguments : UIInitArguments
{
    public int ResId;
    public int SaleType;
}

public class AnswerArguments : UIInitArguments
{
}

public class FaceListArguments : UIInitArguments
{
    public int Type;
}

public class ChatItemListArguments : UIInitArguments
{
    public int Type;
}

public class CleanDustArguments : UIInitArguments
{
    public int StatueIndex;
}

public class PuzzleImageArguments : UIInitArguments
{
    public int StatueIndex;
}

public class LineConfirmArguments : UIInitArguments
{
    public TeamCharacterMessage Msg;
}

public class RechargeFrameArguments : UIInitArguments
{
}

public class PlayFrameArguments : UIInitArguments
{
}

public class TitleUIArguments : UIInitArguments
{
}

public class LevelUpTipArguments : UIInitArguments
{
}

public class SevenDayRewardArguments : UIInitArguments
{
}

public class StrongArguments : UIInitArguments
{
}

public class GuardArguments : UIInitArguments
{
}

public class RechargeActivityArguments : UIInitArguments
{
}

public class MissionTipArguments : UIInitArguments
{
    public MissionTipArguments(int missionId)
    {
        Id = missionId;
    }

    public int Id;
}

public class ShareFrameArguments : UIInitArguments
{
}

public class WorshipFrameArguments : UIInitArguments
{
}

public class ShowItemsArguments : UIInitArguments
{
    public Dictionary<int, int> Items;
}

public class QuickBuyArguments : UIInitArguments
{
    public Dictionary<int, int> Items;
}

public class FirstChargeFrameArguments : UIInitArguments
{
}
public class OperationActivityFrameArguments : UIInitArguments
{
}
public class AcientBattleFieldFrameArguments : UIInitArguments
{
}
public class MonsterSiegeUIFrameArguments : UIInitArguments
{
    public int activityId = 5;
}

public class MieShiTapUIArguments : UIInitArguments
{
}

public class ExchangeFrameArguments : UIInitArguments
{
}

public class FuctionTipFrameArguments : UIInitArguments
{
}
public class ChestInfoUIArguments : UIInitArguments
{
}
