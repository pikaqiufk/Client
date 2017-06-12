#region using

using System;
using System.Collections.Generic;
using ClientDataModel;
using DataTable;
using Shared;

#endregion

//组织静态数据
public static class AttrBaseManager
{
    #region  静态变量

    public static readonly int CharacterCount = 3; //职业数量
    public static readonly int Attr1Count = 5; //1级属性数量
    public static readonly int CharacterLevelMax = 500; //职业最大等级
    public static readonly int LevelMax = 500; //LevelData最大等级
    public static readonly int DodgeMax = 8000; //最大闪避率
    public static readonly int DamageCritical = 8000; //最大伤害暴击率
    public static readonly int HealthCritical = 10000; //最大治疗暴击率

    #endregion

    #region  数据结构

    public static readonly string[] AttrName = new string[(int) eAttributeType.Count]
    {
        GameUtils.GetDictionaryText(221000), //  "等级"    ,
        GameUtils.GetDictionaryText(221001), //  "力量"    ,
        GameUtils.GetDictionaryText(221002), //  "敏捷"    ,
        GameUtils.GetDictionaryText(221003), //  "智力"    ,
        GameUtils.GetDictionaryText(221004), //  "体力"    ,
        GameUtils.GetDictionaryText(221005), //  "物攻最小值" ,
        GameUtils.GetDictionaryText(221006), //  "物攻最大值" ,
        GameUtils.GetDictionaryText(221007), //  "魔攻最小值" ,
        GameUtils.GetDictionaryText(221008), //  "魔攻最大值" ,
        GameUtils.GetDictionaryText(221009), //  "附加伤害"  ,
        GameUtils.GetDictionaryText(221010), //  "物理防御"  ,
        GameUtils.GetDictionaryText(221011), //  "魔法防御"  ,
        GameUtils.GetDictionaryText(221012), //  "伤害抵挡"  ,
        GameUtils.GetDictionaryText(221013), //  "生命上限"  ,
        GameUtils.GetDictionaryText(221014), //  "魔法上限"  ,
        GameUtils.GetDictionaryText(221015), //  "幸运一击率" ,
        GameUtils.GetDictionaryText(221016), //  "幸运一击伤害率"   ,
        GameUtils.GetDictionaryText(221017), //  "卓越一击率" ,
        GameUtils.GetDictionaryText(221018), //  "卓越一击伤害率"   ,
        GameUtils.GetDictionaryText(221019), //  "命中"    ,
        GameUtils.GetDictionaryText(221020), //  "闪避"    ,
        GameUtils.GetDictionaryText(221021), //  "伤害加成率" ,
        GameUtils.GetDictionaryText(221022), //  "伤害减少率" ,
        GameUtils.GetDictionaryText(221023), //  "伤害反弹率" ,
        GameUtils.GetDictionaryText(221024), //  "无视防御率" ,
        GameUtils.GetDictionaryText(221025), //  "移动速度"  ,
        GameUtils.GetDictionaryText(221026), //  "击中回复"
        GameUtils.GetDictionaryText(221031), //  "火属性攻击"
        GameUtils.GetDictionaryText(221032), //  "冰属性攻击"
        GameUtils.GetDictionaryText(221033), //  "毒属性攻击"
        GameUtils.GetDictionaryText(221034), //  "火属性抗性"
        GameUtils.GetDictionaryText(221035), //  "冰属性抗性"
        GameUtils.GetDictionaryText(221036), //  "毒属性抗性"
    };

    public static readonly int[,,] CharacterAttr =
        new int[CharacterCount, CharacterLevelMax, (int) eAttributeType.Count]; //各角色各等级的各基础属性

    public static readonly int[,,] CharacterAttrRef = new int[CharacterCount, Attr1Count, (int) eAttributeType.Count];
        //各职业各属性之间的影响

    public static readonly List<int>[,] AttrBeRefList = new List<int>[CharacterCount, (int) eAttributeType.Count];
        //各职业各属性被哪些属性影响

    public static readonly List<int>[,] AttrCanRefList = new List<int>[CharacterCount, (int) eAttributeType.Count];
        //各职业各属性影响哪些属性

    public static readonly double[] HitReduce = new double[LevelMax + 1];
    public static readonly double[] DodgeReduce = new double[LevelMax + 1];
    public static readonly double[] Critical = new double[LevelMax + 1];
    public static readonly double[] Toughness = new double[LevelMax + 1];
    public static readonly double[] Dodge = new double[LevelMax + 1];
    public static readonly double[] Hit = new double[LevelMax + 1];

    #endregion

    #region  初始化

    //初始化属性
    public static void Init()
    {
        InitRefAttr();
        InitCharacter();
        InitLevelData();
    }

    //初始化等级系数
    public static void InitLevelData()
    {
        Table.ForeachLevelData(record =>
        {
            if (record.Id > LevelMax)
            {
                return false;
            }
            HitReduce[record.Id] = record.Hit;
            DodgeReduce[record.Id] = record.Dodge;
            return true;
        });
    }

    //初始化属性修正
    private static readonly List<int> RefIndextoAttrId = new List<int>
    {
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        10,
        11,
        13,
        14,
        19,
        20
    };

    private static void InitRefAttr()
    {
        //var tbAttrRef = Table.AttrRef;

        for (var c = 0; c != CharacterCount; ++c) //角色
        {
            AttrBeRefList[c, 0] = new List<int>();
            for (var i = 0; i != Attr1Count; ++i) //影响别的属性 的属性
            {
                AttrCanRefList[c, i] = new List<int>();
                var IsFind = false;

                Table.ForeachAttrRef(record =>
                {
                    if (record.CharacterId != c || i != record.AttrId)
                    {
                        return true;
                    }
                    //AttrName[i] = record.Desc;
                    IsFind = true;
                    for (var j = 1; j != (int) eAttributeType.Count; ++j) //被影响的属性
                    {
                        if (j > record.Attr.Length)
                        {
                            continue;
                        }
                        var AttrId = RefIndextoAttrId[j - 1];
                        if (AttrBeRefList[c, AttrId] == null)
                        {
                            AttrBeRefList[c, AttrId] = new List<int>();
                        }
                        var nValue = record.Attr[j - 1];
                        CharacterAttrRef[c, i, AttrId] = nValue;
                        if (nValue > 0)
                        {
                            AttrCanRefList[c, i].Add(AttrId);
                            AttrBeRefList[c, AttrId].Add(i);
                        }
                    }
                    return false;
                });
                if (!IsFind)
                {
                    Logger.Warn("AttrBaseManager::InitRefAttr NoThisLine Character={0} AttrBaseId={1} ", c, i);
                }
            }
        }
    }

    //初始化等级属性
    private static void InitCharacter()
    {
        for (var i = 0; i != CharacterCount; ++i)
        {
            var thisCharacter = Table.GetCharacterBase(i);
            if (thisCharacter == null)
            {
                continue;
            }
            for (var k = 0; k != (int) eAttributeType.Count; ++k)
            {
                var nBaseAttr = thisCharacter.Attr[k];
                var nLevelUpAttr = GetAttrRef(i, eAttributeType.Level, (eAttributeType) k);
                for (var j = 0; j != CharacterLevelMax; ++j)
                {
                    CharacterAttr[i, j, k] = nBaseAttr + nLevelUpAttr*j/100;
                }
            }
        }
    }

    #endregion

    #region  获取数据

    //获取属性
    public static int GetAttrValue(int nCharacterId, int nLevel, eAttributeType type)
    {
        if (nCharacterId >= CharacterCount || nCharacterId < 0)
        {
            //Logger.Error("AttrBaseManager::GetAttrValue Error CharacterId={0}", nCharacterId);
            var tbmonster = Table.GetCharacterBase(nCharacterId);
            if (tbmonster == null)
            {
                Logger.Error("Character ID not find ! CharacterId={0}", nCharacterId);
                return 0;
            }
            return tbmonster.Attr[(int) type];
        }
        nLevel = nLevel - 1;
        if (nLevel >= CharacterLevelMax || nLevel < 0)
        {
            Logger.Error("AttrBaseManager::GetAttrValue Error Level={0}", nLevel);
            return 0;
        }
        if (type >= eAttributeType.Count || type < 0)
        {
            Logger.Error("AttrBaseManager::GetAttrValue Error AttrId={0}", type);
            return 0;
        }
        return CharacterAttr[nCharacterId, nLevel, (int) type];
    }

    //被哪些属性修正的列表
    public static List<int> GetBeRefAttrList(int nCharacterId, eAttributeType reftype)
    {
        if (nCharacterId >= CharacterCount || nCharacterId < 0)
        {
            //因为怪物也会调用这里，就不报错了 Logger.Error("AttrBaseManager::GetBeRefAttrList Error CharacterId={0}", nCharacterId);
            return new List<int>();
        }
        if (reftype >= eAttributeType.Count || reftype < 0)
        {
            Logger.Error("AttrBaseManager::GetBeRefAttrList Error BaseAttrId={0}", reftype);
            return new List<int>();
        }
        return AttrBeRefList[nCharacterId, (int) reftype];
    }

    //会修改哪些属性
    public static List<int> GetCanRefAttrList(int nCharacterId, eAttributeType basetype)
    {
        if (nCharacterId >= CharacterCount || nCharacterId < 0)
        {
            Logger.Error("AttrBaseManager::GetCanRefAttrList Error CharacterId={0}", nCharacterId);
            return new List<int>();
        }
        if (basetype >= eAttributeType.Count || basetype < 0)
        {
            Logger.Error("AttrBaseManager::GetCanRefAttrList Error BaseAttrId={0}", basetype);
            return new List<int>();
        }
        return AttrCanRefList[nCharacterId, (int) basetype];
    }

    //获得属性修正
    public static int GetAttrRef(int nCharacterId, eAttributeType basetype, eAttributeType reftype)
    {
        if (nCharacterId >= CharacterCount || nCharacterId < 0)
        {
            Logger.Error("AttrBaseManager::GetAttrRef Error CharacterId={0}", nCharacterId);
            return 0;
        }
        if (basetype >= eAttributeType.Count || basetype < 0)
        {
            Logger.Error("AttrBaseManager::GetAttrRef Error BaseAttrId={0}", basetype);
            return 0;
        }
        if (reftype >= eAttributeType.Count || reftype < 0)
        {
            Logger.Error("AttrBaseManager::GetAttrRef Error RefAttrId={0}", reftype);
            return 0;
        }
        return CharacterAttrRef[nCharacterId, (int) basetype, (int) reftype];
    }

    #endregion
}

public class PlayerAttr
{
    #region  数据结构

    public enum PlayerAttrChange
    {
        AddPoint = 0, //加点
        Equip = 1, //装备
        Wing = 2, //翅膀
        Elf = 3, //精灵
        Talant = 4, //修炼
        HandBook = 5, //图鉴
        Medal = 6, //勋章
        Statue = 7, //神像
        //占星宝石
        EquipSkill = 8, //装备技能
        MaxCount
    }

    public int mDataId
    {
        get { return PlayerDataManager.Instance.GetRoleId(); }
    }

    //属性
    private readonly int[] mData = new int[(int) eAttributeType.AttrCount];
    public int[] mRryAddPoint = new int[4];
    public int Ladder = 0;

    private int mLevel
    {
        get { return GetLevel(); }
    }

    public static PlayerAttr _Instance;
    public bool InitOver;

    public static PlayerAttr Instance
    {
        get
        {
            if (_Instance == null)
            {
                AttrBaseManager.Init();
                _Instance = new PlayerAttr();
                //_Instance.EquipRefresh();
                //_Instance.BookRefresh();
                //_Instance.AddPointRefresh();
                //_Instance.TalentRefresh();
                //_Instance.InitAttributesAll();
            }
            return _Instance;
        }
    }

    //获取等级
    public int GetLevel()
    {
        return PlayerDataManager.Instance.GetLevel();
    }

    //
    private readonly bool[] AttrFlag = new bool[(int) PlayerAttrChange.MaxCount];
    //属性脏标记
    public BitFlag mFlag = new BitFlag((int) eAttributeType.AttrCount, -1);
    //加点数据
    public int[] mAddPointData = new int[(int) eAttributeType.AttrCount];
    public int[] mAddPointDataRef = new int[(int) eAttributeType.AttrCount];
    //装备数据
    public int[] mEquipData = new int[(int) eAttributeType.AttrCount];
    public int[] mEquipDataRef = new int[(int) eAttributeType.AttrCount];
    //图鉴数据
    public int[] mBookData = new int[(int) eAttributeType.AttrCount];
    public int[] mBookDataRef = new int[(int) eAttributeType.AttrCount];
    //天赋数据
    public int[] mTalentData = new int[(int) eAttributeType.AttrCount];
    public int[] mTalentDataRef = new int[(int) eAttributeType.AttrCount];
    //套装Buff
    public int EquipTieFightPoint;

    //public Dictionary<int, BuffData> mEquipBuff = new Dictionary<int, BuffData>();

    //战斗力
    private int FightPoint;
    private bool FightPointFlag = true;
    private bool FightSkillFlag = true;
    private int mSkillPoint;

    #endregion

    #region  初始化

    public void CleanUp()
    {
        InitOver = false;
    }

    public int GetAttackType()
    {
        if (mDataId == 1)
        {
            return 1;
        }
        return 0;
    }

    //public PlayerAttr(ObjCharacter obj)
    //{
    //    mObj = obj;
    //    mDataId = obj.GetDataId();
    //    mData[0] = 1;
    //}
    public static void AttrConvert(Dictionary<int, int> AttrList,
                                   int[] attr,
                                   int[] attrRef,
                                   int attackType,
                                   bool IsEquip = false)
    {
        {
            // foreach(var i in AttrList)
            var __enumerator2 = (AttrList).GetEnumerator();
            while (__enumerator2.MoveNext())
            {
                var i = __enumerator2.Current;
                {
                    if (i.Key < (int) eAttributeType.AttrCount)
                    {
                        attr[i.Key] += i.Value;
                    }
                    else
                    {
                        switch (i.Key)
                        {
                            case 105:
                            {
                                if (attackType == 1)
                                {
                                    attr[(int) eAttributeType.MagPowerMin] += i.Value;
                                    attr[(int) eAttributeType.MagPowerMax] += i.Value;
                                }
                                else
                                {
                                    attr[(int) eAttributeType.PhyPowerMin] += i.Value;
                                    attr[(int) eAttributeType.PhyPowerMax] += i.Value;
                                }
                            }
                                break;
                            case 106:
                            {
                                if (IsEquip)
                                {
                                    attrRef[(int) eAttributeType.MagPowerMin] += i.Value*100;
                                    attrRef[(int) eAttributeType.MagPowerMax] += i.Value*100;
                                    attrRef[(int) eAttributeType.PhyPowerMin] += i.Value*100;
                                    attrRef[(int) eAttributeType.PhyPowerMax] += i.Value*100;
                                }
                                else
                                {
                                    attrRef[(int) eAttributeType.MagPowerMin] += i.Value;
                                    attrRef[(int) eAttributeType.MagPowerMax] += i.Value;
                                    attrRef[(int) eAttributeType.PhyPowerMin] += i.Value;
                                    attrRef[(int) eAttributeType.PhyPowerMax] += i.Value;
                                }
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
                                if (IsEquip)
                                {
                                    attrRef[(int) eAttributeType.PhyArmor] += i.Value*100;
                                    attrRef[(int) eAttributeType.MagArmor] += i.Value*100;
                                }
                                else
                                {
                                    attrRef[(int) eAttributeType.PhyArmor] += i.Value;
                                    attrRef[(int) eAttributeType.MagArmor] += i.Value;
                                }
                            }
                                break;
                            case 113:
                            {
                                if (IsEquip)
                                {
                                    attrRef[(int) eAttributeType.HpMax] += i.Value*100;
                                }
                                else
                                {
                                    attrRef[(int) eAttributeType.HpMax] += i.Value;
                                }
                            }
                                break;
                            case 114:
                            {
                                if (IsEquip)
                                {
                                    attrRef[(int) eAttributeType.MpMax] += i.Value*100;
                                }
                                else
                                {
                                    attrRef[(int) eAttributeType.MpMax] += i.Value;
                                }
                            }
                                break;
                            case 119:
                            {
                                if (IsEquip)
                                {
                                    attrRef[(int) eAttributeType.Hit] += i.Value*100;
                                }
                                else
                                {
                                    attrRef[(int) eAttributeType.Hit] += i.Value;
                                }
                            }
                                break;
                            case 120:
                            {
                                if (IsEquip)
                                {
                                    attrRef[(int) eAttributeType.Dodge] += i.Value*100;
                                }
                                else
                                {
                                    attrRef[(int) eAttributeType.Dodge] += i.Value;
                                }
                            }
                                break;
                        }
                    }
                }
            }
        }
    }

    public static void PushEquipAttr(Dictionary<int, int> AttrList, int AttrId, int AttrValue, int characterLevel)
    {
        if (AttrId == -1)
        {
        }
        else if (AttrId == 98)
        {
            var nValue = characterLevel/AttrValue;
            AttrList.modifyValue((int) eAttributeType.MagPowerMin, nValue);
            AttrList.modifyValue((int) eAttributeType.MagPowerMax, nValue);
            AttrList.modifyValue((int) eAttributeType.PhyPowerMin, nValue);
            AttrList.modifyValue((int) eAttributeType.PhyPowerMax, nValue);
        }
        else if (AttrId == 99)
        {
            var nValue = characterLevel/AttrValue;
            AttrList.modifyValue((int) eAttributeType.PhyArmor, nValue);
            AttrList.modifyValue((int) eAttributeType.MagArmor, nValue);
        }
        else
        {
            AttrList.modifyValue(AttrId, AttrValue);
        }
    }

    //获取装备属性
    private void BatchEquipAttr(Dictionary<int, int> AttrList)
    {
        var Tao = new Dictionary<int, int>(); //套装ID  件数
        var indexCount = PlayerDataManager.Instance.PlayerDataModel.EquipList.Count;
        for (var index = 0; index < indexCount; index++)
        {
            var equip = PlayerDataManager.Instance.PlayerDataModel.EquipList[index];
            if (equip.ItemId == -1)
            {
                continue;
            }
            if (equip.Exdata[22] <= 0)
            {
                continue;
            }
            var tbitem = Table.GetItemBase(equip.ItemId);
            if (tbitem == null)
            {
                Logger.Error("BatchEquipAttr itemId  Id={0} not find by Table", equip.ItemId);
                continue;
            }
            var equipid = tbitem.Exdata[0];
            var tbEquip = Table.GetEquipBase(equipid);
            if (tbEquip == null)
            {
                Logger.Error("BatchEquipAttr itemId  Id={0} not find equip={1}", equip.ItemId, equipid);
                continue;
            }
            if (tbEquip.TieId >= 0)
            {
                int n;
                if (Tao.TryGetValue(tbEquip.TieId, out n))
                {
                    Tao[tbEquip.TieId] = n + 1;
                }
                else
                {
                    Tao[tbEquip.TieId] = 1;
                }
            }
            var attrs = PlayerDataManager.Instance.GetEquipAttributeFix(equip);
            {
                // foreach(var i in attrs)
                var __enumerator3 = (attrs).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var i = __enumerator3.Current;
                    {
                        AttrList.modifyValue(i.Key, i.Value);
                    }
                }
            }
        }

        EquipTieFightPoint = 0;
        {
            // foreach(var Equiptie in Tao)
            var __enumerator4 = (Tao).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var Equiptie = __enumerator4.Current;
                {
                    var tbTie = Table.GetEquipTie(Equiptie.Key);
                    if (tbTie == null)
                    {
                        Logger.Error("BatchEquipAttr GetEquipTie  Id={0} not find", Equiptie.Key);
                        continue;
                    }
                    for (var i = 0; i != tbTie.Attr1Id.Length; ++i)
                    {
                        if (Equiptie.Value >= tbTie.NeedCount[i])
                        {
                            var attrid = tbTie.Attr1Id[i];
                            var attrValue = tbTie.Attr1Value[i];
                            if (attrid > 0 && attrid < (int) eAttributeType.AttrCount)
                            {
                                PushEquipAttr(AttrList, attrid, attrValue, mLevel);
                                //attr[attrid] += attrValue;
                            }
                            attrid = tbTie.Attr2Id[i];
                            attrValue = tbTie.Attr2Value[i];
                            if (attrid > 0 && attrid < (int) eAttributeType.AttrCount)
                            {
                                PushEquipAttr(AttrList, attrid, attrValue, mLevel);
                            }
                            EquipTieFightPoint += tbTie.FightPoint[i];
                        }
                    }
                }
            }
        }

        var wingCon = UIManager.Instance.GetController(UIConfig.WingUI);
        if (wingCon != null)
        {
            var dataModel = wingCon.GetDataModel("") as WingDataModel;
            if (dataModel != null)
            {
                GetWingAttrList(AttrList, dataModel.ItemData);
            }
        }
    }

    //获取翅膀属性
    private static void GetWingAttrList(Dictionary<int, int> AttrList, WingItemData wing)
    {
        var tbWing = Table.GetWingQuality(wing.ItemId);
        if (tbWing == null)
        {
            return;
        }
        var characterLevel = PlayerDataManager.Instance.GetLevel();
        var Quality = tbWing.Segment;
        if (wing.ExtraData.Count != 11)
        {
            return;
        }
        FillWingAdvanceAttr(AttrList, wing);
        //培养属性
        for (var i = 0; i != 5; ++i)
        {
            var tbWingTrain = Table.GetWingTrain(wing.ExtraData[1 + i*2]);
            if (tbWingTrain == null)
            {
                continue;
            }
            if (tbWingTrain.Condition > Quality)
            {
                continue;
            }
            for (var j = 0; j != tbWingTrain.AddPropID.Length; ++j)
            {
                var nAttrId = tbWingTrain.AddPropID[j];
                var nValue = tbWingTrain.AddPropValue[j];
                if (nAttrId < 0 || nValue <= 0)
                {
                    break;
                }
                if (nValue > 0 && nAttrId != -1)
                {
                    PushEquipAttr(AttrList, nAttrId, nValue, characterLevel);
                }
            }
        }
    }

    // 填充翅膀进阶总属性
    public static void FillWingAdvanceAttr(Dictionary<int, int> AttrList, WingItemData wing)
    {
        FillWingBreakAttr(AttrList, wing.ItemId);
        FillWingGrowAttr(AttrList, wing);
    }

    // 突破属性
    public static void FillWingBreakAttr(Dictionary<int, int> AttrList, int itemId)
    {
        var tbWing = Table.GetWingQuality(itemId);
        if (tbWing == null)
        {
            return;
        }

        var characterLevel = PlayerDataManager.Instance.GetLevel();
        for (var i = 0; i != tbWing.AddPropID.Length; ++i)
        {
            var nAttrId = tbWing.AddPropID[i];
            if (nAttrId < 0)
            {
                break;
            }
            var nValue = tbWing.AddPropValue[i];
            if (nValue > 0 && nAttrId != -1)
            {
                PushEquipAttr(AttrList, nAttrId, nValue, characterLevel);
            }
        }        
    }

    // 填充翅膀当前成长的属性
    public static void FillWingGrowAttr(Dictionary<int, int> AttrList, WingItemData wing)
    {
        var characterLevel = PlayerDataManager.Instance.GetLevel();
        for (var i = (int)eWingExDefine.eGrowProperty; i < wing.ExtraData.Count; ++i)
        {
            var attrId = GetWingGrowAttrId(wing, i);
            if (attrId > 0)
            {
                var value = wing.ExtraData[i + 1];
                if (value > 0)
                {
                    PushEquipAttr(AttrList, attrId, value, characterLevel);
                }
                ++i;
            }
        }
    }

    private static int GetWingGrowAttrId(WingItemData wing, int index)
    {
        if (index >= wing.ExtraData.Count)
            return -1;
        var attrId = wing.ExtraData[index];
        return attrId;
    }

    //获取精灵属性
    private void BatchElfAttrList(Dictionary<int, int> AttrList)
    {
        var elfCon = UIManager.Instance.GetController(UIConfig.ElfUI);
        if (elfCon == null)
        {
            return;
        }
        var dataModel = elfCon.GetDataModel("") as ElfDataModel;
        if (dataModel == null)
        {
            return;
        }
        var tbLevel = Table.GetLevelData(dataModel.FormationLevel);
        var groupList = new Dictionary<int, int>();
        for (var index = 0; index < 3; index++)
        {
            var elf = dataModel.Formations[index].ElfData;
            if (elf.ItemId == -1)
            {
                continue;
            }
            //if (elf.Exdata[1] == 0) continue;
            var tbElf = Table.GetElf(elf.ItemId);
            if (tbElf == null)
            {
                continue;
            }

            GetElfAttrList(AttrList, elf, tbLevel.FightingWayIncome);
            {
                // foreach(var groupId in tbElf.BelongGroup)
                var __enumerator5 = (tbElf.BelongGroup).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var groupId = (int) __enumerator5.Current;
                    {
                        if (groupId != -1)
                        {
                            groupList.modifyValue(groupId, 1);
                        }
                    }
                }
            }
        }
        {
            // foreach(var pair in groupList)
            var __enumerator7 = (groupList).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var pair = __enumerator7.Current;
                {
                    var tbElfGroup = Table.GetElfGroup(pair.Key);
                    var count = 0;
                    foreach (var i in tbElfGroup.ElfID)
                    {
                        if (i != -1)
                        {
                            count++;
                        }
                    }
                    if (count == pair.Value)
                    {
                        for (var i = 0; i < tbElfGroup.GroupPorp.Length; i++)
                        {
                            var attrId = tbElfGroup.GroupPorp[i];
                            if (attrId == -1)
                            {
                                break;
                            }
                            var attrValue = tbElfGroup.PropValue[i];
                            AttrList.modifyValue(attrId, attrValue);
                        }
                    }
                }
            }
        }
    }

    private static readonly int BattleRef = Table_Tamplet.Convert_Int(Table.GetClientConfig(101).Value);
    private static readonly int NoBattleRef = Table_Tamplet.Convert_Int(Table.GetClientConfig(102).Value);

    private static void GetElfAttrList(Dictionary<int, int> AttrList, ElfItemDataModel elf, int baseValueBili)
    {
        var tbElf = Table.GetElf(elf.ItemId);
        var elfLevel = elf.Exdata[0];
        var isBattle = elf.Index;
        //基础属性
        for (var i = 0; i < tbElf.ElfInitProp.Length; i++)
        {
            var attrId = tbElf.ElfInitProp[i];
            if (attrId == -1)
            {
                break;
            }
            var attrValue = tbElf.ElfProp[i];
            if (elfLevel > 1)
            {
                var upvalue = tbElf.GrowAddValue[i];
                attrValue += upvalue*(elfLevel - 1);
            }
            if (isBattle == 0)
            {
                AttrList.modifyValue(attrId, (int) (attrValue/10000.0f*BattleRef*(10000 + baseValueBili)/10000));
            }
            else
            {
                AttrList.modifyValue(attrId, (int) (attrValue/10000.0f*NoBattleRef*(10000 + baseValueBili)/10000));
            }
        }
        //随机属性
        if (isBattle == 0)
        {
            for (var i = 2; i < 8; ++i)
            {
                var attrId = elf.Exdata[i];
                if (attrId == -1)
                {
                    break;
                }
                var attrValue = elf.Exdata[i + 6];
                AttrList.modifyValue(attrId, attrValue);
            }
        }
    }

    //获取加点属性
    private void BatchAddAttrList(Dictionary<int, int> AttrList)
    {
        for (var i = 1; i < 5; ++i)
        {
            AttrList.modifyValue(i,
                PlayerDataManager.Instance.GetExData(i + 4) + PlayerDataManager.Instance.GetExData(i + 8));
        }
        var ladder = PlayerDataManager.Instance.GetExData(51);
        var tbL = Table.GetTransmigration(ladder);
        if (tbL == null)
        {
            return;
        }
        AttrList.modifyValue(105, tbL.AttackAdd);
        AttrList.modifyValue(10, tbL.PhyDefAdd);
        AttrList.modifyValue(11, tbL.MagicDefAdd);
        AttrList.modifyValue(19, tbL.HitAdd);
        AttrList.modifyValue(20, tbL.DodgeAdd);
        AttrList.modifyValue(13, tbL.LifeAdd);
    }

    //获取勋章属性
    private void BatchMedalAttrList(Dictionary<int, int> AttrList)
    {
        var SailingCon = UIManager.Instance.GetController(UIConfig.SailingUI);
        if (SailingCon == null)
        {
            return;
        }
        var dataModel = SailingCon.GetDataModel("") as SailingDataModel;
        if (dataModel == null)
        {
            return;
        }
        for (var index = 0; index < dataModel.ShipEquip.EquipItem.Count; index++)
        {
            var Medala = dataModel.ShipEquip.EquipItem[index];
            if (Medala.BaseItemId == -1)
            {
                continue;
            }
            var tbMedal = Table.GetMedal(Medala.BaseItemId);
            if (tbMedal == null)
            {
                continue;
            }
            for (var i = 0; i < tbMedal.AddPropID.Length; i++)
            {
                if (tbMedal.AddPropID[i] != -1)
                {
                    AttrList.modifyValue(tbMedal.AddPropID[i],
                        Table.GetSkillUpgrading(tbMedal.PropValue[i]).GetSkillUpgradingValue(Medala.Exdata[0]));
                }
            }
        }
    }

    //获取图鉴属性
    private void BatchBookAttrList(Dictionary<int, int> AttrList)
    {
        {
            // foreach(var book in PlayerDataManager.Instance.BountyBooks)
            var __enumerator8 = (PlayerDataManager.Instance.BountyBooks).GetEnumerator();
            while (__enumerator8.MoveNext())
            {
                var book = __enumerator8.Current;
                {
                    var tbbook = Table.GetHandBook(book);
                    if (tbbook == null)
                    {
                        continue;
                    }
                    PushEquipAttr(AttrList, tbbook.AttrId, tbbook.AttrValue, GetLevel());
                }
            }
        }
        //组收集
        var groupOkCount = 0;
        {
            // foreach(var i in PlayerDataManager.Instance.BookGropData)
            var __enumerator10 = (PlayerDataManager.Instance.BookGropData).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var i = __enumerator10.Current;
                {
                    var tbGroupBook = Table.GetBookGroup(i.Key);
                    if (tbGroupBook == null)
                    {
                        continue;
                    }
                    var isok = true;
                    var index = -1;
                    foreach (var itemid in tbGroupBook.ItemId)
                    {
                        index++;
                        if (itemid == -1)
                        {
                            continue;
                        }
                        if (BitFlag.GetLow(i.Value, index))
                        {
                            PushEquipAttr(AttrList, tbGroupBook.AttrId[index], tbGroupBook.AttrValue[index], GetLevel());
                        }
                        else
                        {
                            isok = false;
                        }
                    }
                    if (isok)
                    {
                        groupOkCount++;
                        for (var j = 0; j != tbGroupBook.GroupAttrId.Length; j++)
                        {
                            if (tbGroupBook.GroupAttrId[j] > 0)
                            {
                                PushEquipAttr(AttrList, tbGroupBook.GroupAttrId[j], tbGroupBook.GroupAttrValue[j],
                                    GetLevel());
                            }
                        }
                    }
                }
            }
        }
    }

    //获取修炼属性
    private void BatchTalantAttrList(Dictionary<int, int> AttrList)
    {
        var TalCount = PlayerDataManager.Instance.PlayerDataModel.SkillData.AttrPanel.Count;
        for (var index = 0; index < TalCount; index++)
        {
            var Tal = PlayerDataManager.Instance.PlayerDataModel.SkillData.AttrPanel[index];
            AttrList.modifyValue(Tal.AttrId, Tal.AttrValue);
        }
    }


    //获取神像属性
    private void BatchStatueAttrList(Dictionary<int, int> AttrList)
    {
        var buildingData = CityManager.Instance.GetBuildingDataByType(BuildingType.ArenaTemple);
        if (buildingData == null)
        {
            return;
        }
        var tbBuild = Table.GetBuilding(buildingData.TypeId);
        var petList = buildingData.PetList;
        var buildRef = GameUtils.GetBSParamByIndex(buildingData.TypeId, Table.GetBuildingService(tbBuild.ServiceId),
            petList, 1);
        for (var i = 0; i != 5; ++i)
        {
            var tbStatue = Table.GetStatue(buildingData.Exdata[i]);
            //基础属性
            var index = 0;
            {
                // foreach(var attrId in tbStatue.PropID)
                var __enumerator11 = (tbStatue.PropID).GetEnumerator();
                while (__enumerator11.MoveNext())
                {
                    var attrId = (int) __enumerator11.Current;
                    {
                        if (attrId != -1)
                        {
                            AttrList.modifyValue(attrId, tbStatue.propValue[index]);
                        }
                        index++;
                    }
                }
            }
            //继承属性
            index = 0;
            var petId = buildingData.PetList[i];
            var pet = CityManager.Instance.GetPetById(petId);
            if (pet == null)
            {
                continue;
            }
            if (pet.Exdata[3] != (int) PetStateType.Building)
            {
                continue;
            }
            {
                // foreach(var attrId in tbStatue.FuseID)
                var __enumerator12 = (tbStatue.FuseID).GetEnumerator();
                while (__enumerator12.MoveNext())
                {
                    var attrId = (int) __enumerator12.Current;
                    {
                        if (attrId != -1)
                        {
                            var aId = attrId;
                            if (aId == 5 && PlayerDataManager.Instance.GetRoleId() == 1)
                            {
                                aId = 7;
                            }
                            else if (aId == 6 && PlayerDataManager.Instance.GetRoleId() == 1)
                            {
                                aId = 8;
                            }
                            var value = FightAttribute.GetPetAttribut(petId, (eAttributeType) aId, pet.Exdata[0]);
                            if (value > 0)
                            {
                                var f = (float) value*tbStatue.FuseValue[0]*buildRef/10000/10000;
                                AttrList.modifyValue(aId, (int) f);
                            }
                        }
                        index++;
                    }
                }
            }
        }
    }

    //初始化武将后开始计算属性
    public void InitAttributesAll()
    {
        InitOver = true;
        SetAttrChange(PlayerAttrChange.EquipSkill);
        mFlag.ReSetAllFlag(true);
        mFlag.CleanFlag((int) eAttributeType.HpNow);
        mFlag.CleanFlag((int) eAttributeType.MpNow);
        Updata();
        //初始化值
        for (var i = eAttributeType.Level; i < eAttributeType.AttrCount; ++i)
        {
            GetDataValue(i);
        }
        CompareAttr();
    }

    #endregion

    #region  数据存取

    //获得属性
    public int GetDataValue(eAttributeType type)
    {
        if (type >= eAttributeType.Count && type < eAttributeType.CountNext)
        {
            return 0;
        }

        if (type == eAttributeType.Level)
        {
            return GetLevel();
        }
        //int a = mFlag.GetFlag((int)type);
        if (1 == mFlag.GetFlag((int) type))
        {
            return UpdateDataValue(type);
        }
        if (type < eAttributeType.Level || type >= eAttributeType.AttrCount)
        {
            return 0;
        }
        return mData[(int) type];
    }

    //获得属性
    public int GetTryDataValue(eAttributeType type)
    {
        double nValue = 0;
        switch (type)
        {
            case eAttributeType.Level:
                return GetLevel();
            case eAttributeType.Strength:
            case eAttributeType.Agility:
            case eAttributeType.Intelligence:
            case eAttributeType.Endurance:
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                var BeRefList = AttrBaseManager.GetBeRefAttrList(mDataId, type);
                if (BeRefList != null)
                {
                    {
                        var __list13 = BeRefList;
                        var __listCount13 = __list13.Count;
                        for (var __i13 = 0; __i13 < __listCount13; ++__i13)
                        {
                            var i = __list13[__i13];
                            {
                                if (i == 0)
                                {
                                    continue;
                                }
                                // ReSharper disable once PossibleLossOfFraction
                                nValue += GetTryDataValue((eAttributeType) i)*
                                          AttrBaseManager.GetAttrRef(mDataId, (eAttributeType) i, type)/100;
                            }
                        }
                    }
                }
                //装备属性
                nValue += mRryAddPoint[(int) type - 1];
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type];
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.PhyPowerMin:
            case eAttributeType.PhyPowerMax:
            case eAttributeType.MagPowerMin:
            case eAttributeType.MagPowerMax:
            case eAttributeType.AddPower:
            case eAttributeType.PhyArmor:
            case eAttributeType.MagArmor:
            case eAttributeType.DamageResistance:
            case eAttributeType.HpMax:
            case eAttributeType.MpMax:
            case eAttributeType.LuckyDamage:
            case eAttributeType.ExcellentDamage:
            case eAttributeType.Hit:
            case eAttributeType.Dodge:
            case eAttributeType.HitRecovery:
            case eAttributeType.FireAttack:
            case eAttributeType.IceAttack:
            case eAttributeType.PoisonAttack:
            case eAttributeType.FireResistance:
            case eAttributeType.IceResistance:
            case eAttributeType.PoisonResistance:                
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                var BeRefList = AttrBaseManager.GetBeRefAttrList(mDataId, type);
                if (BeRefList != null)
                {
                    {
                        var __list14 = BeRefList;
                        var __listCount14 = __list14.Count;
                        for (var __i14 = 0; __i14 < __listCount14; ++__i14)
                        {
                            var i = __list14[__i14];
                            {
                                if (i == 0)
                                {
                                    continue;
                                }
                                // ReSharper disable once PossibleLossOfFraction
                                nValue += GetTryDataValue((eAttributeType) i)*
                                          AttrBaseManager.GetAttrRef(mDataId, (eAttributeType) i, type)/100;
                            }
                        }
                    }
                }
                //装备属性
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type];
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.LuckyPro:
            case eAttributeType.ExcellentPro:
            case eAttributeType.DamageAddPro:
            case eAttributeType.DamageResPro:
            case eAttributeType.DamageReboundPro:
            case eAttributeType.IgnoreArmorPro:
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                //装备属性
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type]*100;
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.MoveSpeed:
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                //装备属性
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type];
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.HpNow:
            {
                nValue = GetDataValue(eAttributeType.HpMax);
            }
                break;
            case eAttributeType.MpNow:
            {
                nValue = GetDataValue(eAttributeType.MpMax);
            }
                break;
            case eAttributeType.AttrCount:
            {
                Logger.Warn("UpdateDataValue eAttributeType={0}!Why???", type);
            }
                break;
            default:
                break;
        }
        return (int) nValue;
    }

    //设置武将属性值
    public void SetDataValue(eAttributeType type, int value)
    {
        mFlag.CleanFlag((int) type);
        if (type == eAttributeType.HpNow)
        {
            value = Math.Min(value, GetDataValue(eAttributeType.HpMax));
        }
        else if (type == eAttributeType.MpNow)
        {
            value = Math.Min(value, GetDataValue(eAttributeType.MpMax));
        }
        else if (type == eAttributeType.HpMax)
        {
            if (GetDataValue(eAttributeType.HpNow) > value)
            {
                SetDataValue(eAttributeType.HpNow, value);
            }
        }
        else if (type == eAttributeType.MpMax)
        {
            if (GetDataValue(eAttributeType.MpNow) > value)
            {
                SetDataValue(eAttributeType.MpNow, value);
            }
        }
        if (type < eAttributeType.Level || type >= eAttributeType.AttrCount)
        {
            return;
        }
        mData[(int) type] = value;
        UpdateOtherFlag(type);
    }

    //设置属性脏标记
    public void SetFlag(eAttributeType type)
    {
        mFlag.SetFlag((int) type);
        UpdateOtherFlag(type);
    }

    //由属性ID修改标记位
    public void SetFlagByAttrId(int attrId)
    {
        switch (attrId)
        {
            case 105:
            {
                SetFlag(eAttributeType.MagPowerMin);
                SetFlag(eAttributeType.MagPowerMax);
                SetFlag(eAttributeType.PhyPowerMin);
                SetFlag(eAttributeType.PhyPowerMax);
            }
                break;
            case 106:
            {
                SetFlag(eAttributeType.MagPowerMin);
                SetFlag(eAttributeType.MagPowerMax);
                SetFlag(eAttributeType.PhyPowerMin);
                SetFlag(eAttributeType.PhyPowerMax);
            }
                break;
            case 110:
            {
                SetFlag(eAttributeType.PhyArmor);
                SetFlag(eAttributeType.MagArmor);
            }
                break;
            case 111:
            {
                SetFlag(eAttributeType.PhyArmor);
                SetFlag(eAttributeType.MagArmor);
            }
                break;
            case 113:
            {
                SetFlag(eAttributeType.HpMax);
            }
                break;
            case 114:
            {
                SetFlag(eAttributeType.MpMax);
            }
                break;
            case 119:
            {
                SetFlag(eAttributeType.Hit);
            }
                break;
            case 120:
            {
                SetFlag(eAttributeType.Dodge);
            }
                break;
            default:
                SetFlag((eAttributeType) attrId);
                break;
        }
    }

    //更新属性
    private int UpdateDataValue(eAttributeType type)
    {
        if (type < eAttributeType.Level || type >= eAttributeType.AttrCount)
        {
            return 0;
        }
        double nValue = 0;
        mFlag.CleanFlag((int) type); //清除脏标记
        //var hero = Table.hero[DataID];
        switch (type)
        {
            case eAttributeType.Level:
                nValue = mData[(int) type];
                break;
            case eAttributeType.Strength:
            case eAttributeType.Agility:
            case eAttributeType.Intelligence:
            case eAttributeType.Endurance:
            case eAttributeType.PhyPowerMin:
            case eAttributeType.PhyPowerMax:
            case eAttributeType.MagPowerMin:
            case eAttributeType.MagPowerMax:
            case eAttributeType.AddPower:
            case eAttributeType.PhyArmor:
            case eAttributeType.MagArmor:
            case eAttributeType.DamageResistance:
            case eAttributeType.HpMax:
            case eAttributeType.MpMax:
            case eAttributeType.LuckyDamage:
            case eAttributeType.ExcellentDamage:
            case eAttributeType.Hit:
            case eAttributeType.Dodge:
            case eAttributeType.HitRecovery:
            case eAttributeType.FireAttack:
            case eAttributeType.IceAttack:
            case eAttributeType.PoisonAttack:
            case eAttributeType.FireResistance:
            case eAttributeType.IceResistance:
            case eAttributeType.PoisonResistance:
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                var BeRefList = AttrBaseManager.GetBeRefAttrList(mDataId, type);
                if (BeRefList != null)
                {
                    {
                        var __list15 = BeRefList;
                        var __listCount15 = __list15.Count;
                        for (var __i15 = 0; __i15 < __listCount15; ++__i15)
                        {
                            var i = __list15[__i15];
                            {
                                if (i == 0)
                                {
                                    continue;
                                }
                                // ReSharper disable once PossibleLossOfFraction
                                nValue += GetDataValue((eAttributeType) i)*
                                          AttrBaseManager.GetAttrRef(mDataId, (eAttributeType) i, type)/100;
                            }
                        }
                    }
                }
                //装备属性
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type];
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.LuckyPro:
            case eAttributeType.ExcellentPro:
            case eAttributeType.DamageAddPro:
            case eAttributeType.DamageResPro:
            case eAttributeType.DamageReboundPro:
            case eAttributeType.IgnoreArmorPro:
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                //装备属性
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type]*100;
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.MoveSpeed:
            {
                double fBili = 1.0f;
                //基础属性
                nValue = AttrBaseManager.GetAttrValue(mDataId, GetDataValue(eAttributeType.Level), type);
                //装备属性
                nValue += mAddPointData[(int) type];
                nValue += mEquipData[(int) type];
                nValue += mBookData[(int) type];
                nValue += mTalentData[(int) type];
                //计算
                nValue =
                    (int)
                        (fBili*nValue*
                         (10000 + mAddPointDataRef[(int) type] + mEquipDataRef[(int) type] + mBookDataRef[(int) type] +
                          mTalentDataRef[(int) type])/10000);
            }
                break;
            case eAttributeType.HpNow:
            {
                nValue = GetDataValue(eAttributeType.HpMax);
            }
                break;
            case eAttributeType.MpNow:
            {
                nValue = GetDataValue(eAttributeType.MpMax);
            }
                break;
            case eAttributeType.AttrCount:
            {
                Logger.Warn("UpdateDataValue eAttributeType={0}!Why???", type);
            }
                break;
            default:
                break;
        }

        if (nValue == mData[(int) type])
        {
            return (int) nValue;
        }

        SetDataValue(type, (int) nValue);
        return mData[(int) type];
    }

    //设置其它属性脏标记
    private void UpdateOtherFlag(eAttributeType type)
    {
        if (type == eAttributeType.Level)
        {
            SetFlag(eAttributeType.HpMax);
            SetFlag(eAttributeType.MpMax);
            SetFlag(eAttributeType.PhyPowerMin);
            SetFlag(eAttributeType.PhyPowerMax);
            SetFlag(eAttributeType.MagPowerMin);
            SetFlag(eAttributeType.MagPowerMax);
        }
        if (type >= eAttributeType.Count)
        {
            return;
        }
        var CanRefList = AttrBaseManager.GetCanRefAttrList(mDataId, type);
        if (CanRefList == null)
        {
            return;
        }
        {
            var __list16 = CanRefList;
            var __listCount16 = __list16.Count;
            for (var __i16 = 0; __i16 < __listCount16; ++__i16)
            {
                var i = __list16[__i16];
                {
                    SetFlag((eAttributeType) i);
                }
            }
        }
    }

    //把属性复制到这个列表
    public void CopyToAttr(List<int> list)
    {
        list.Clear();
        {
            var __array17 = mData;
            var __arrayLength17 = __array17.Length;
            for (var __i17 = 0; __i17 < __arrayLength17; ++__i17)
            {
                var i = __array17[__i17];
                {
                    list.Add(i);
                }
            }
        }
    }

    #endregion

    #region 战斗力相关

    //获取战斗力
    public int GetFightPoint()
    {
        var fp = GetAttrFightPoint() + GetSkillPoint();
        return fp;
    }

    //获取技能战斗力
    private int GetSkillPoint()
    {
        if (!GetAttrFlag(PlayerAttrChange.EquipSkill))
        {
            return mSkillPoint;
        }
        SetAttrFlag(PlayerAttrChange.EquipSkill, false);
        mSkillPoint = 0;
        //PlayerDataManager.Instance.ApplySkills();
        var equipSkillCount = PlayerDataManager.Instance.PlayerDataModel.SkillData.EquipSkills.Count;
        for (var i = 0; i < equipSkillCount; i++)
        {
            var skill = PlayerDataManager.Instance.PlayerDataModel.SkillData.EquipSkills[i];
            if (skill.SkillId == -1)
            {
                continue;
            }
            //if (skill.SkillLv < 1) continue;
            var tbSkill = Table.GetSkill(skill.SkillId);
            if (tbSkill == null)
            {
                continue;
            }
            mSkillPoint += Table.GetSkillUpgrading(tbSkill.FightPoint).GetSkillUpgradingValue(skill.SkillLv);
        }
        return mSkillPoint;
    }

    // 属性战斗力
    private int GetAttrFightPoint()
    {
        if (!FightPointFlag)
        {
            return FightPoint;
        }
        FightPointFlag = false;
        var level = GetDataValue(eAttributeType.Level);
        var tbLevel = Table.GetLevelData(level);
        if (tbLevel == null)
        {
            return 0;
        }
        FightPoint = 0;
        var Ref = new Dictionary<eAttributeType, int>();
        for (var type = eAttributeType.PhyPowerMin; type != eAttributeType.Count; ++type)
        {
            //基础固定属性
            var nValue = AttrBaseManager.GetAttrValue(mDataId, level, type);
            var BeRefList = AttrBaseManager.GetBeRefAttrList(mDataId, type);
            if (BeRefList != null)
            {
                {
                    var __list18 = BeRefList;
                    var __listCount18 = __list18.Count;
                    for (var __i18 = 0; __i18 < __listCount18; ++__i18)
                    {
                        var i = __list18[__i18];
                        {
                            if (i == 0)
                            {
                                continue;
                            }
                            nValue += GetDataValue((eAttributeType) i)*
                                      AttrBaseManager.GetAttrRef(mDataId, (eAttributeType) i, type)/100;
                        }
                    }
                }
            }
            nValue += mAddPointData[(int) type]; //图鉴固定的
            nValue += mBookData[(int) type]; //图鉴固定的
            nValue += mTalentData[(int) type]; //天赋固定的
            switch ((int) type)
            {
                case 15:
                {
                    nValue += mEquipData[(int) type]*100; //装备固定的
                    FightPoint += nValue*tbLevel.LuckyProFightPoint/10000;
                }
                    break;
                case 17:
                {
                    nValue += mEquipData[(int) type]*100; //装备固定的
                    FightPoint += nValue*tbLevel.ExcellentProFightPoint/10000;
                }
                    break;
                case 21:
                {
                    nValue += mEquipData[(int) type]*100; //装备固定的
                    FightPoint += nValue*tbLevel.DamageAddProFightPoint/10000;
                }
                    break;
                case 22:
                {
                    nValue += mEquipData[(int) type]*100; //装备固定的
                    FightPoint += nValue*tbLevel.DamageResProFightPoint/10000;
                }
                    break;
                case 23:
                {
                    nValue += mEquipData[(int) type]*100; //装备固定的
                    FightPoint += nValue*tbLevel.DamageReboundProFightPoint/10000;
                }
                    break;
                case 24:
                {
                    nValue += mEquipData[(int) type]*100; //装备固定的
                    FightPoint += nValue*tbLevel.IgnoreArmorProFightPoint/10000;
                }
                    break;
                default:
                {
                    nValue += mEquipData[(int) type]; //装备固定的
                    var tbState = Table.GetStats((int) type);
                    if (tbState == null)
                    {
                        continue;
                    }
                    var careerId = PlayerDataManager.Instance.GetRoleId();
                    var dict = PlayerDataManager.Instance.CareeridToStatsPointIndex;
                    if (dict.ContainsValue(careerId))
                    {
                        FightPoint += tbState.FightPoint[dict[careerId]]*nValue/100;
                    }
                    else
                    {
                        Logger.Error(" CareeridToStatsPointIndex3  error {0}", careerId);
                    }
                }
                    break;
            }
        }
        //图鉴百分比
        //装备百分比
        //天赋百分比
        Ref[eAttributeType.MagPowerMin] = mBookDataRef[(int) eAttributeType.MagPowerMin] +
                                          mEquipDataRef[(int) eAttributeType.MagPowerMin] +
                                          mTalentDataRef[(int) eAttributeType.MagPowerMin] +
                                          mAddPointDataRef[(int) eAttributeType.MagPowerMin];
        Ref[eAttributeType.PhyArmor] = mBookDataRef[(int) eAttributeType.PhyArmor] +
                                       mEquipDataRef[(int) eAttributeType.PhyArmor] +
                                       mTalentDataRef[(int) eAttributeType.PhyArmor] +
                                       mAddPointDataRef[(int) eAttributeType.PhyArmor];
        Ref[eAttributeType.HpMax] = mBookDataRef[(int) eAttributeType.HpMax] + mEquipDataRef[(int) eAttributeType.HpMax] +
                                    mTalentDataRef[(int) eAttributeType.HpMax] +
                                    mAddPointDataRef[(int) eAttributeType.HpMax];
        Ref[eAttributeType.MpMax] = mBookDataRef[(int) eAttributeType.MpMax] + mEquipDataRef[(int) eAttributeType.MpMax] +
                                    mTalentDataRef[(int) eAttributeType.MpMax] +
                                    mAddPointDataRef[(int) eAttributeType.MpMax];
        Ref[eAttributeType.Hit] = mBookDataRef[(int) eAttributeType.Hit] + mEquipDataRef[(int) eAttributeType.Hit] +
                                  mTalentDataRef[(int) eAttributeType.Hit] + mAddPointDataRef[(int) eAttributeType.Hit];
        Ref[eAttributeType.Dodge] = mBookDataRef[(int) eAttributeType.Dodge] + mEquipDataRef[(int) eAttributeType.Dodge] +
                                    mTalentDataRef[(int) eAttributeType.Dodge] +
                                    mAddPointDataRef[(int) eAttributeType.Dodge];
        //百分比计算
        FightPoint += Ref[eAttributeType.MagPowerMin]*tbLevel.PowerFightPoint/10000;
        FightPoint += Ref[eAttributeType.PhyArmor]*tbLevel.ArmorFightPoint/10000;
        FightPoint += Ref[eAttributeType.HpMax]*tbLevel.HpFightPoint/10000;
        FightPoint += Ref[eAttributeType.MpMax]*tbLevel.MpFightPoint/10000;
        FightPoint += Ref[eAttributeType.Hit]*tbLevel.HitFightPoint/10000;
        FightPoint += Ref[eAttributeType.Dodge]*tbLevel.DodgeFightPoint/10000;
        //套装
        FightPoint += EquipTieFightPoint;
        //技能
        //FightPoint += GetSkillPoint();
        return FightPoint;
    }

    //设置战斗力标记
    public void SetFightPointFalg(bool b = true)
    {
        FightPointFlag = b;
    }

    #endregion

    #region 标记相关

    private bool GetAttrFlag(PlayerAttrChange pac)
    {
        return AttrFlag[(int) pac];
    }

    private void SetAttrFlag(PlayerAttrChange pac, bool b = true)
    {
        AttrFlag[(int) pac] = b;
    }

    public void Updata()
    {
        if (!InitOver)
        {
            return;
        }
        var isChange = false;
        for (var i = PlayerAttrChange.AddPoint; i < PlayerAttrChange.MaxCount; i++)
        {
            if (GetAttrFlag(i))
            {
                switch (i)
                {
                    case PlayerAttrChange.AddPoint:
                        AddPointRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.Equip:
                        EquipRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.Wing:
                        EquipRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.Elf:
                        BookRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.Talant:
                        TalentRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.HandBook:
                        BookRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.Medal:
                        BookRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.Statue:
                        AddPointRefresh();
                        isChange = true;
                        break;
                    case PlayerAttrChange.EquipSkill:
                        GetSkillPoint();
                        break;
                }
            }
        }
        if (isChange)
        {
            CompareAttr();
        }
    }

    public void SetAttrChange(PlayerAttrChange pac)
    {
        SetAttrFlag(pac);
    }


    public void CompareAttr()
    {
        //for (var i = eAttributeType.Level; i <= eAttributeType.HitRecovery; i++)
        //{
        //    int serverData = PlayerDataManager.Instance.PlayerDataModel.Attributes[(int)i];
        //    int clientData = GetDataValue(i);
        //    if (serverData != clientData)
        //    {
        //        Logger.Error("e={0},s={1},c={2}", i, serverData, clientData);
        //    }
        //    else
        //    {
        //        Logger.Info("e={0},s={1},c={2}", i, serverData, clientData);
        //    }
        //}
        //int cfp = GetFightPoint();
        //int sfp = PlayerDataManager.Instance.PlayerDataModel.Attributes.FightValue;
        //if (cfp != sfp)
        //{
        //    Logger.Error("fp,s={0},c={1}",  sfp, cfp);
        //}
        //else
        //{
        //    Logger.Info("fp,s={0},c={1}", sfp, cfp);
        //}
    }

    //刷新加点数据
    public void AddPointRefresh()
    {
        Array.Clear(mAddPointData, 0, (int) eAttributeType.AttrCount);
        Array.Clear(mAddPointDataRef, 0, (int) eAttributeType.AttrCount);
        var pointAttrs = new Dictionary<int, int>();
        BatchAddAttrList(pointAttrs); //获取加点，果子属性
        BatchStatueAttrList(pointAttrs); //获取加点，果子属性
        AttrConvert(pointAttrs, mAddPointData, mAddPointDataRef, GetAttackType());
        mFlag.ReSetAllFlag(true);
        SetAttrFlag(PlayerAttrChange.Statue, false);
        SetAttrFlag(PlayerAttrChange.AddPoint, false);
        SetFightPointFalg();
    }

    //刷新修炼数据
    public void TalentRefresh()
    {
        Array.Clear(mTalentData, 0, (int) eAttributeType.AttrCount);
        Array.Clear(mTalentDataRef, 0, (int) eAttributeType.AttrCount);
        var talentAttrs = new Dictionary<int, int>();
        BatchTalantAttrList(talentAttrs);
        SetAttrFlag(PlayerAttrChange.Talant, false);
        AttrConvert(talentAttrs, mTalentData, mTalentDataRef, GetAttackType());
        mFlag.ReSetAllFlag(true);
        SetFightPointFalg();
    }

    //刷新图鉴数据
    public void BookRefresh()
    {
        Array.Clear(mBookData, 0, (int) eAttributeType.AttrCount);
        Array.Clear(mBookDataRef, 0, (int) eAttributeType.AttrCount);
        var bookAttrs = new Dictionary<int, int>();
        BatchElfAttrList(bookAttrs); //获取精灵属性
        SetAttrFlag(PlayerAttrChange.Elf, false);
        BatchMedalAttrList(bookAttrs); //勋章属性
        SetAttrFlag(PlayerAttrChange.Medal, false);
        BatchBookAttrList(bookAttrs); //图鉴属性
        SetAttrFlag(PlayerAttrChange.HandBook, false);
        AttrConvert(bookAttrs, mBookData, mBookDataRef, GetAttackType());
        mFlag.ReSetAllFlag(true);
        SetFightPointFalg();
    }

    //刷新装备数据
    public void EquipRefresh()
    {
        Array.Clear(mEquipData, 0, (int) eAttributeType.AttrCount);
        Array.Clear(mEquipDataRef, 0, (int) eAttributeType.AttrCount);
        var EquipAttrs = new Dictionary<int, int>();
        BatchEquipAttr(EquipAttrs);
        SetAttrFlag(PlayerAttrChange.Equip, false);
        SetAttrFlag(PlayerAttrChange.Wing, false);
        AttrConvert(EquipAttrs, mEquipData, mEquipDataRef, GetAttackType(), true);
        mFlag.ReSetAllFlag(true);
        SetFightPointFalg();
    }

    #endregion
}