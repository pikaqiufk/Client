#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using LZ4s;
using ScorpionNetLib;
using MuUtility;
using ObjCommand;
using PigeonCoopToolkit.Effects.Trails;
using ProtoBuf;
using Shared;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

#if UNITY_ANDROID && !UNITY_EDITOR
using ICSharpCode.SharpZipLib.Zip;
#endif


public static class GameObjectExtension
{
    private static readonly Dictionary<int, int> sRenderQueueDict = new Dictionary<int, int>();
    private static readonly List<int> sRenderQueueList = new List<int>();
    private static readonly HashSet<int> sRenderQueueSet = new HashSet<int>();
//     public static void SetLayerRecursive(this GameObject go, int layer, int layerMask = 0)
//     {
//         if ((1 << go.layer & layerMask) == 0)
//             go.layer = layer;
//         var goTransform = go.transform;
//         int c = goTransform.childCount;
//         for (int i = 0; i < c; i++)
//         {
//             SetLayerRecursive(goTransform.GetChild(i).gameObject, layer, layerMask);
//         }
//     }

    public static void SetLayerRecursive(this GameObject go, int layer, int layerMask = 0)
    {
        if ((1 << go.layer & layerMask) == 0)
        {
            go.layer = layer;
        }
        OptList<Transform>.List.Clear();
        go.GetComponentsInChildren(true, OptList<Transform>.List);
        var s = OptList<Transform>.List.Count;
        var x = OptList<Transform>.List;
        for (var j = 0; j < s; ++j)
        {
            var child = x[j].gameObject;
            if ((1 << child.layer & layerMask) == 0)
            {
                child.layer = layer;
            }
        }
    }

    public static void SetRenderQueue(this GameObject go, int queue)
    {
        if (null == go)
        {
            return;
        }

        if (-1 == queue)
        {
            return;
        }

        sRenderQueueSet.Clear();
        OptList<Renderer>.List.Clear();
        go.GetComponentsInChildren(true, OptList<Renderer>.List);
        {
            var __array12 = OptList<Renderer>.List;
            var __arrayLength12 = __array12.Count;
            for (var __i12 = 0; __i12 < __arrayLength12; ++__i12)
            {
                var renderer = __array12[__i12];
                {
                    var mats = renderer.materials;
                    for (var i = 0; i < mats.Length; i++)
                    {
                        sRenderQueueSet.Add(mats[i].renderQueue != -1 ? mats[i].renderQueue : mats[i].shader.renderQueue);
                    }
                }
            }
        }
        sRenderQueueList.Clear();
        sRenderQueueList.AddRange(sRenderQueueSet);
        sRenderQueueList.Sort();

        sRenderQueueDict.Clear();
        for (var i = 0; i < sRenderQueueList.Count; ++i)
        {
            sRenderQueueDict[sRenderQueueList[i]] = i;
        }
        {
            var __array13 = OptList<Renderer>.List;
            var __arrayLength13 = __array13.Count;
            for (var __i13 = 0; __i13 < __arrayLength13; ++__i13)
            {
                var renderer = __array13[__i13];
                {
                    var mats = renderer.materials;
                    for (var i = 0; i < mats.Length; i++)
                    {
                        mats[i].renderQueue = queue +
                                              sRenderQueueDict[
                                                  mats[i].renderQueue != -1
                                                      ? mats[i].renderQueue
                                                      : mats[i].shader.renderQueue];
                    }
                }
            }
        }
    }
}

public static class Extension
{
    public static DateTime FromServerBinary(long t)
    {
        var d = DateTime.FromBinary(t);
        if (d.Year <= 2000)
        {
            return d;
        }
        return d.Add(Game.Instance.ServerZoneDiff);
    }

    public static void SetDataSource(this GameObject go, object data)
    {
        var bind = go.GetComponent<BindDataRoot>();
        if (bind)
        {
            bind.SetBindDataSource(data);
        }
    }

    //获取服务器参数的Int值
    public static int ToInt(this ClientConfigRecord tbConfig)
    {
        int temp;
        if (Int32.TryParse(tbConfig.Value, out temp))
        {
            return temp;
        }
        return -1;
    }

    public static long ToServerBinary(this DateTime t)
    {
        return t.Add(-Game.Instance.ServerZoneDiff).ToBinary();
    }
}

public static class CityPetSkill
{
    public static int GetBSParamByIndex(BuildingType Type, BuildingServiceRecord tbBS, int index, List<int> petItems)
        //index是技能表中参数2或参数4的值
    {
        var list = new List<PetSkillRecord>();
        for (var idx = 0; idx < petItems.Count; idx++)
        {
            var petId = petItems[idx];
            if (petId == -1)
            {
                continue;
            }
            var petinfo = Table.GetPet(petId);
            if (petinfo == null)
            {
                continue;
            }
            int[] skillLevelLimit =
            {
                petinfo.Speciality[0],
                petinfo.Speciality[1],
                petinfo.Speciality[2]
            };
            //特殊技能
            const int PetItemDataModelMaxSkill2 = PetItemDataModel.MaxSkill;
            var petdata = CityManager.Instance.GetPetById(petId);
            for (var i = 0; i < PetItemDataModelMaxSkill2; i++)
            {
                if (petdata.Exdata[PetItemExtDataIdx.Level] < skillLevelLimit[i] ||
                    petdata.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + i] == -1)
                {
                    continue;
                }
                var skill = Table.GetPetSkill(petdata.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + i]);
                list.Add(skill);
            }
        }

        var tbValue = tbBS.Param[index];
        var bili = 10000;
        var bili2 = 0;
        {
            var __list14 = list;
            var __listCount14 = __list14.Count;
            for (var __i14 = 0; __i14 < __listCount14; ++__i14)
            {
                var skill = __list14[__i14];
                {
                    if (skill != null)
                    {
                        if ((int) Type == skill.Param[0] && skill.EffectId == 0)
                        {
                            if (index == skill.Param[1])
                            {
                                bili += skill.Param[2];
                                bili2 += skill.Param[2];
                            }
                            if (index == skill.Param[3])
                            {
                                bili += skill.Param[4];
                                bili2 += skill.Param[4];
                            }
                        }
                    }
                }
            }
        }
        switch (Type)
        {
            case BuildingType.ArenaTemple:
            case BuildingType.CompositeHouse:
                return bili2/100;
            case BuildingType.LogPlace:
            case BuildingType.Mine:
            case BuildingType.MercenaryCamp:
            case BuildingType.BlacksmithShop:
            case BuildingType.BraveHarbor:
            case BuildingType.Exchange:
                return tbValue*bili/10000;
        }
        return 0;
    }
}

public static class TransformExtension
{
    public static Transform FindChildRecursive(this Transform t, string name)
    {
        if (t)
        {
            // foreach(var c in t)
            var __enumerator3 = (t).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var c = (Transform) __enumerator3.Current;
                {
                    var partName = c.name;
                    if (partName == name)
                    {
                        return c;
                    }
                    var r = FindChildRecursive(c, name);
                    if (r != null)
                    {
                        return r;
                    }
                }
            }
        }
        return null;
    }

    public static string FullPath(this Transform o)
    {
        if (null != o.parent)
        {
            return FullPath(o.parent) + "/" + o.name;
        }
        return o.name;
    }

    public static int GetChildIndex(this Transform parent, Transform child)
    {
        for (int i = 0, imax = parent.childCount; i < imax; ++i)
        {
            var t = parent.GetChild(i);
            if (t == child)
            {
                return i;
            }
        }
        return -1;
    }
}

public static class SkillExtension
{
    public static int GetSkillUpgradingValue(this SkillUpgradingRecord tbUpgrade, int nLevel)
    {
        switch (tbUpgrade.Type)
        {
            case 0: //枚举
            {
                if (nLevel >= tbUpgrade.Values.Count || nLevel < 0)
                {
                    Logger.Warn("SkillUpgradingRecord=[{0}]  Level=[{1}]  is Out", tbUpgrade.Id, nLevel);
                    return tbUpgrade.Values[tbUpgrade.Values.Count - 1];
                }
                var result = tbUpgrade.Values[nLevel];
                return result;
            }
            case 1: //等差
            {
                var result = tbUpgrade.Param[0] + tbUpgrade.Param[1]*nLevel;
                return result;
            }
            case 2: //等比
            {
                if (nLevel < 1)
                {
                    return 1;
                }
                var result = tbUpgrade.Param[0]*Pow(tbUpgrade.Param[1], nLevel - 1);
                return result;
            }
            case 3: //等级阶跃
            {
                var length = tbUpgrade.Values.Count;
                if (length < 2)
                {
                    return 0;
                }
                var lastIndex = 1;
                for (var i = 0; i < tbUpgrade.Values.Count; i = i + 2)
                {
                    var lvl = tbUpgrade.Values[i];
                    if (nLevel < lvl)
                    {
                        return tbUpgrade.Values[lastIndex];
                    }
                    lastIndex = i + 1;
                }
                return tbUpgrade.Values[length - 1];
            }
            case 4: //跨等级枚举
            {
                var valueLevel = nLevel/tbUpgrade.Param[0];
                if (valueLevel >= tbUpgrade.Values.Count || valueLevel < 0)
                {
                    Logger.Warn("SkillUpgradingRecord=[{0}]  Level=[{1}] valueLevel=[{2}]  is Out", tbUpgrade.Id, nLevel,
                        valueLevel);
                    return tbUpgrade.Values[tbUpgrade.Values.Count - 1];
                }
                var result = tbUpgrade.Values[valueLevel];
                return result;
            }
        }
        return -1;
    }

    public static int ModifyByLevel(int nOldValue, int nLevel, int limitValue)
    {
        if (nOldValue < limitValue)
        {
            return nOldValue;
        }
        var tbUpgrade = Table.GetSkillUpgrading(nOldValue%limitValue);
        if (tbUpgrade == null)
        {
            return nOldValue;
        }
        return tbUpgrade.GetSkillUpgradingValue(nLevel);
    }

    public static int Pow(int baseValue, int count)
    {
        if (count < 1)
        {
            return 1;
        }
        var value = baseValue;
        for (var i = 1; i < count; i++)
        {
            value *= baseValue;
        }
        return value;
    }

    public static float Pow(float baseValue, int count)
    {
        if (count < 1)
        {
            return 1;
        }
        var value = baseValue;
        for (var i = 1; i < count; i++)
        {
            value *= baseValue;
        }
        return value;
    }
}

public static class FightAttribute
{
    private static readonly Dictionary<int, int> AttrRef_IdtoRefIndex = new Dictionary<int, int>
    {
        {1, 0},
        {2, 1},
        {3, 2},
        {4, 3},
        {5, 4},
        {6, 5},
        {7, 6},
        {8, 7},
        {10, 8},
        {11, 9},
        {13, 10},
        {14, 11},
        {19, 12},
        {20, 13}
    };

    //计算宠物战斗力
    public static int CalculatePetFightPower(PetRecord tablePet, int level, Dictionary<eAttributeType, int> Ref = null)
    {
        var power = 0;
        if (null == Ref)
        {
            Ref = new Dictionary<eAttributeType, int>();
        }

        Ref.Add(eAttributeType.PhyPowerMin, GetPetAttribut(tablePet.Id, eAttributeType.PhyPowerMin, level));
        Ref.Add(eAttributeType.PhyPowerMax, GetPetAttribut(tablePet.Id, eAttributeType.PhyPowerMax, level));
        Ref.Add(eAttributeType.MagPowerMin, GetPetAttribut(tablePet.Id, eAttributeType.MagPowerMin, level));
        Ref.Add(eAttributeType.MagPowerMax, GetPetAttribut(tablePet.Id, eAttributeType.MagPowerMax, level));
        Ref.Add(eAttributeType.PhyArmor, GetPetAttribut(tablePet.Id, eAttributeType.PhyArmor, level));
        Ref.Add(eAttributeType.MagArmor, GetPetAttribut(tablePet.Id, eAttributeType.MagArmor, level));
        Ref.Add(eAttributeType.HpMax, GetPetAttribut(tablePet.Id, eAttributeType.HpMax, level));


        var tbLevel = Table.GetLevelData(level);
        var dataId = tablePet.CharacterID;
        {
            // foreach(var pair in Ref)
            var __enumerator5 = (Ref).GetEnumerator();
            while (__enumerator5.MoveNext())
            {
                var pair = __enumerator5.Current;
                {
                    var type = pair.Key;
                    //基础固定属性
                    //int nValue = AttrBaseManager.GetAttrValue(dataId, level, type);
                    var nValue = pair.Value; //随从的直接用算好的属性
                    var BeRefList = AttrBaseManager.GetBeRefAttrList(dataId, type);
                    if (BeRefList != null)
                    {
                        {
                            var __list11 = BeRefList;
                            var __listCount11 = __list11.Count;
                            for (var __i11 = 0; __i11 < __listCount11; ++__i11)
                            {
                                var i = __list11[__i11];
                                {
                                    if (i == 0)
                                    {
                                        continue;
                                    }
                                    var refAttrType = (eAttributeType) i;
                                    if (Ref.ContainsKey(refAttrType))
                                    {
                                        nValue += Ref[refAttrType]*
                                                  AttrBaseManager.GetAttrRef(dataId, (eAttributeType) i, type);
                                    }
                                }
                            }
                        }
                    }

                    switch ((int) type)
                    {
                        case 15:
                        {
                            power += nValue*tbLevel.LuckyProFightPoint/100;
                        }
                            break;
                        case 17:
                        {
                            power += nValue*tbLevel.ExcellentProFightPoint/100;
                        }
                            break;
                        case 21:
                        {
                            power += nValue*tbLevel.DamageAddProFightPoint/100;
                        }
                            break;
                        case 22:
                        {
                            power += nValue*tbLevel.DamageResProFightPoint/100;
                        }
                            break;
                        case 23:
                        {
                            power += nValue*tbLevel.DamageReboundProFightPoint/100;
                        }
                            break;
                        case 24:
                        {
                            power += nValue*tbLevel.IgnoreArmorProFightPoint/100;
                        }
                            break;
                        default:
                        {
                            var tbState = Table.GetStats((int) type);
                            if (tbState == null)
                            {
                                continue;
                            }
                            power += tbState.PetFight*nValue/100;
                            //int careerId = PlayerDataManager.Instance.GetRoleId();
                            //var dict = PlayerDataManager.Instance.CareeridToStatsPointIndex;
                            //if (dict.ContainsValue(careerId))
                            //{
                            //    power += tbState.PetFight * nValue / 100;
                            //}
                            //else
                            //{
                            //    Logger.Error(" CareeridToStatsPointIndex1  error {0}", careerId);
                            //}
                        }
                            break;
                    }
                }
            }
        }

        return Math.Max(0, power);
    }

    private static int GetAttrRef_Id2Index(int attrId)
    {
        int index;
        if (AttrRef_IdtoRefIndex.TryGetValue(attrId, out index))
        {
            return index;
        }
        return -1;
    }

    public static int GetPetAttribut(int petId, eAttributeType attrId, int level)
    {
        var tbPet = Table.GetPet(petId);
        if (tbPet == null)
        {
            return -1;
        }
        var tbCharacter = Table.GetCharacterBase(tbPet.CharacterID);
        if (tbCharacter == null)
        {
            return -1;
        }
        if (attrId < eAttributeType.Level || attrId > eAttributeType.HitRecovery)
        {
            return -1;
        }
        var value = tbCharacter.Attr[(int) attrId];
        for (var i = 1; i != 4; ++i)
        {
            var skillId = tbCharacter.InitSkill[i];
            var tbSkil = Table.GetSkill(skillId);
            if (tbSkil == null)
            {
                continue;
            }
            if (tbSkil.CastType != 3)
            {
                continue;
            }
            var tbBuff = Table.GetBuff(tbSkil.CastParam[0]);
            if (tbBuff == null)
            {
                continue;
            }
            for (var j = 0; j < tbBuff.effectid.Length; j++)
            {
                var effectId = tbBuff.effectid[j];
                if (effectId != 2)
                {
                    continue;
                }
                if (tbBuff.effectparam[j, 0] != (int) attrId)
                {
                    continue;
                }
                var skillUp = Table.GetSkillUpgrading(tbBuff.effectparam[j, 3] - 10000000);
                if (skillUp == null)
                {
                    continue;
                }
                value += skillUp.GetSkillUpgradingValue(level - 1);
            }
        }

        //var tbAttrRef = Table.GetAttrRef(tbPet.AttrRef);
        //if (tbAttrRef == null) return value;
        //int index = GetAttrRef_Id2Index((int)attrId);
        //if (index < 0) return value;
        //value += tbAttrRef.Attr[index] * (level - 1);
        return value;
    }
}

public static class CalculatePetMissionEffect
{
    //循环随从所有技能
    public static void ForeachSkill(int petId, int petLevel, Func<PetSkillRecord, bool> act)
    {
        if (act == null)
        {
            Logger.Error("Foreach Actor act is null");
            return;
        }
        var tbPet = Table.GetPet(petId);
        var ladder = tbPet.Ladder;
        var tbPetSkillLength0 = tbPet.Skill.Length;
        for (var i = 0; i < tbPetSkillLength0; i++)
        {
            if (ladder >= tbPet.ActiveLadder[i] && tbPet.Skill[i] != -1)
            {
                try
                {
                    if (!act(Table.GetPetSkill(tbPet.Skill[i])))
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }

    //计算任务时间
    public static int GetFinishTime(int time,
                                    List<BagItemDataModel> petItems,
                                    int petCount,
                                    List<PetSkillRecord> buffList)
    {
        var seconds = 0;
        //表修改注释     
        // 		if(petCount-1>=0 && petCount-1<table.NeedTime.Length)
        // 		{
        // 			seconds = table.NeedTime[petCount-1] * 60;
        // 		}
        //表修改注释,临时注释
        seconds = 1*60;
        var refEffectId = new List<int>();
        //表修改注释
        //         foreach (int i in table .EffectRef)
        //         {
        //             if (i == -1) break;
        //             refEffectId.Add(i);
        //         }
        refEffectId.Add(GetMissionTime(time, petCount));
        {
            var __list18 = refEffectId;
            var __listCount18 = __list18.Count;
            for (var __i18 = 0; __i18 < __listCount18; ++__i18)
            {
                var i = __list18[__i18];
                {
                    foreach (var petItem in petItems)
                    {
                        ForeachSkill(petItem.ItemId, petItem.Exdata[0], skill =>
                        {
                            if (i == skill.EffectId)
                            {
                                if (skill.Param[0] == -1)
                                {
                                    return true;
                                }
                                seconds = seconds*(10000 + skill.Param[0])/10000;
                                buffList.Add(skill);
                            }
                            return true;
                        });
                    }
                }
            }
        } //伙伴
        seconds = PetSkillDataRef(petItems, seconds, 0, buffList);
        return seconds;
    }

    //返回任务时间修正的效果ID
    //时间  int 8小时 ：小于137  等于大于136
    public static int GetMissionTime(int time, int Petcount)
    {
        if (time
            //表修改注释
            //  [Petcount - 1] 
            < 480)
        {
            return 137;
        }
        return 136;
    }

    public static int PetSkillDataRef(List<BagItemDataModel> petItems,
                                      int value,
                                      int paramIndex,
                                      List<PetSkillRecord> buffList)
    {
        {
            var __list16 = petItems;
            var __listCount16 = __list16.Count;
            for (var __i16 = 0; __i16 < __listCount16; ++__i16)
            {
                var petItem = __list16[__i16];
                {
                    ForeachSkill(petItem.ItemId, petItem.Exdata[0], skill =>
                    {
                        if (100 == skill.EffectId)
                        {
                            if (skill.Param[paramIndex] == -1)
                            {
                                return true;
                            }
                            foreach (var petItem2 in petItems)
                            {
                                if (petItem == petItem2)
                                {
                                    continue;
                                }
                                if (Table.GetPet(petItem2.ItemId).Type == skill.Param[4])
                                {
                                    value = value*(10000 + skill.Param[paramIndex])/10000;
                                    buffList.Add(skill);
                                    break;
                                }
                            }
                        }
                        return true;
                    });
                }
            }
        }
        return value;
    }
}

// do not alloc memory every time you call getcomponents in children.
public static class OptList
{
    public static List<IList> AllLists = new List<IList>();

    public static void ClearAll()
    {
        for (var i = 0; i < AllLists.Count; ++i)
        {
            AllLists[i].Clear();
        }
    }
}

public static class OptList<T>
{
    private static List<T> mList;

    public static List<T> List
    {
        get
        {
            if (mList == null)
            {
                mList = new List<T>();
                OptList.AllLists.Add(mList);
            }
            return mList;
        }
    }
}

public static class MColor
{
    public static Color black;
    public static Color blue;
    public static Color clear;
    public static Color cyan;
    public static Color green;
    public static Color greenlight;
    public static Color grey;
    public static Color magenta;
    public static Color orange;
    public static Color purple;
    public static Color red;
    public static Color white;
    public static Color yellow;

    public static void Init()
    {
        magenta = Color.magenta;
        cyan = Color.cyan;
        clear = Color.clear;
        black = Color.black;
        yellow = Color.yellow;
        white = GameUtils.GetTableColor(0);
        green = GameUtils.GetTableColor(1);
        blue = GameUtils.GetTableColor(2);
        purple = GameUtils.GetTableColor(3);
        orange = GameUtils.GetTableColor(4);
        red = GameUtils.GetTableColor(10);
        greenlight = GameUtils.GetTableColor(14);
        grey = GameUtils.GetTableColor(96);
    }
}

public static class GameUtils
{
    public const float _PRECISION = 1/PRECISION;
    public static int AchieveFlagConfig;

    private static readonly Dictionary<int, int> AttrIdtoIndex = new Dictionary<int, int>
    {
        {13, 0},
        {14, 1},
        {9, 2},
        {12, 3},
        {19, 4},
        {20, 5},
        {17, 6},
        {21, 7},
        {22, 8},
        {23, 9},
        {24, 10},
        {26, 11},
        {25, 12},
        {105, 13},
        {110, 14},
        {113, 15},
        {114, 16},
        {119, 17},
        {120, 18},
        {106, 19},
        {111, 20},
        {98, 21},
        {99, 22}
    };

    private static readonly Dictionary<int, int> AttrIdtoIndexElf = new Dictionary<int, int>
    {
        {105, 0},
        {17, 1},
        {21, 2},
        {22, 3},
        {23, 4},
        {24, 5},
        {106, 6},
        {111, 7},
        {113, 8},
        {114, 9},
        {119, 10},
        {120, 11}
    };

    public static Dictionary<int, string> AttrName = new Dictionary<int, string>();
    public static int AutoFightLongDistance;
    public static int AutoFightShortDistance;
    public static int AutoMedicineHpCd;
    public static int AutoMedicineMpCd;
    //自动拾取距离
    public static float AutoPickUpDistance = 5.0f;
    //挂机拾取检索距离最大值
    public static float AutoPickUpDistanceMax = 5.0f;
    public static string BeginCoclorStrRegex = @"[[][A-Fa-f0-9]{6}[]]";
    public static float BlockLayerDuration;
    //聊天常量
    public static int ChatWorldCount;
    public static Dictionary<int, int> ConditionToFlag = new Dictionary<int, int>();
    public static int DistanceRemoveTarget = 10;
    public static int DungeonShowDelay;
    public static int ElfFlagConfig;
    public static int ElfSecondCondition = 1;
    public static int ElfThirdCondition = 1;
    public static string EndCoclorStrRegex = "[-]";

    public static Dictionary<int, int> EquipValueRef = new Dictionary<int, int>
    {
        {15, 100},
        {16, 10000},
        {17, 100},
        {18, 10000},
        {21, 100},
        {22, 100},
        {23, 100},
        {24, 100},
        {106, 100},
        {111, 100},
        {113, 100},
        {114, 100},
        {119, 100},
        {120, 100}
    };

    public static int FriendFlagConfig;
    public static int FubenStar1Time;
    public static int FubenStar2Time;
    public static int FubenStar3Time;
    //210200 你获得了：{0} ×{1}
    public static DictionaryRecord GianItemTip;
    public static Color green = new Color(173/255.0f, 1.0f, 0);
    public static Color grey = new Color(127/255.0f, 127/255.0f, 127/255.0f);
    public static int HandBookGroupFlagConfig;
    public static int HandBookWantedFlagConfig;
    public static int HornWorldCount;
    //表格索引
    private static readonly List<int> IndextoAttrId = new List<int>
    {
        13,
        14,
        9,
        12,
        19,
        20,
        17,
        21,
        22,
        23,
        24,
        26,
        25,
        105,
        110,
        113,
        114,
        119,
        120,
        106,
        111,
        98,
        99
    };

    public static int MailFlagConfig;
    public static int MaxLevel = 400;
    public static int MaxMailCount = 50; //最大邮件数量
    public static float OfflineExpRatelimit = 0.6f;
    public static int OrderRefreshCost = 1;
    //查看其他玩家属性客户端缓存时间（秒）
    public static int PlayerInfoCacheTime = 100;
    public const float PRECISION = 100.0f;
    public static int RankFlagConfig;
    public static int[] RankWorshipAction = {117, 324, 219};
    //private static Dictionary<int, int> AttrIdtoRefIndex = new Dictionary<int, int>
    //    {
    //        {1,0},{2,1},{3,2},{4,3},{5,4},{6,5},{7,6},{8,7},{10,8},{11,9},{13,10},{14,11},{19,12},{20,13}
    //    };
//     public static int GetBaseAttrIndex(int attrId)
//     {
//         int index;
//         if (AttrIdtoRefIndex.TryGetValue(attrId, out index))
//         {
//             return index;
//         }
//         //Logger.Error("Elf GetBaseAttrIndex attrId={0}", attrId);
//         return -1;
//     }
//    private static List<int> IndextoAttrId = new List<int>() { 105, 17, 21, 22, 23, 24, 106, 111, 113, 114, 119, 120 };
//     public static int GetAttrId(int index)
//     {
//         if (index > IndextoAttrId.Count || index < 0)
//         {
//             Logger.Error("Elf GetAttrId index={0}", index);
//             return -1;
//         }
//         return IndextoAttrId[index];
//     }

    //初始属性修正
    private static readonly List<int> RefIndextoAttrIdElf = new List<int>
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

    public static int SkillInnateFlagConfig;
    public static int SkillTalentFlagConfig;
    public static string StarIcon = ""; // 金色 [FFC000]★[-]
    //
    public static int SweepCouponId;
    public static int SystemNoticeRollingScreenLimit;
    public static int SystemNoticeScrollingSpeed;
    public static int TeamFlagConfig;
    public static string TimeOver;
    //喇叭广播移动速度
    public static int TrumpeMoveSpeedt = 10;
    //喇叭广播时间间隔秒
    public static float TrumpetDurationTime = 5.0f;
    public static int UnionFlagConfig;
    public static int WingQualityMax = 1;
    public static int WishFlagReward;
    public static int LanguageIndex { get; set; }

    public static void AddAttribute(ObservableCollection<AttributeChangeDataModel> attributes,
                                    int id,
                                    int value,
                                    int change = -1)
    {
    }

    public static string AnalyseChatInfoNodeArg(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
        if (str[0] != '#')
        {
            return str;
        }
        var args = str.Split(':');
        if (args.Length == 0)
        {
            return str;
        }
        if (args[0] == "#ItemBase.Name")
        {
            if (args.Length != 2)
            {
                return str;
            }

            var itemId = args[1].ToInt();
            if (itemId == -1)
            {
                return str;
            }
            var tbItem = Table.GetItemBase(itemId);
            if (tbItem == null)
            {
                return str;
            }
            var itemName = tbItem.Name;
            var itemColor = GetTableColorString(tbItem.Quality);
            var ret = string.Format("[[{0}]{1}[-]]", itemColor, itemName);
            return ret;
        }
        if (args[0] == "#Character")
        {
            var name = args[1];
            var ret = string.Format("[{0}]", name);
            return ret;
        }
        if (args[0] == "#Scene.Name")
        {
            var ret = "";
            var sceneId = args[1].ToInt();
            var tbScene = Table.GetScene(sceneId);
            if (tbScene != null)
            {
                ret += tbScene.Name;
            }
            if (args.Length > 2)
            {
                for (var i = 2; i < args.Length; i++)
                {
                    sceneId = args[i].ToInt();
                    tbScene = Table.GetScene(sceneId);
                    if (tbScene != null)
                    {
                        ret += "," + tbScene.Name;
                    }
                }
            }

            return ret;
        }
        if (args[0] == "#Dictionary")
        {
            var ret = "";
            var dicId = args[1].ToInt();
            return GetDictionaryText(dicId);
        }
        if (args[0] == "#Date")
        {
            var ret = "";
            var date = Extension.FromServerBinary(long.Parse(args[1]));
            var formatStr = GetDictionaryText(args[2].ToInt());
            return date.ToString(formatStr);
        }
        return str;
    }

    public static string AttributeName(int attrId)
    {
        var ret = "";
        if (AttrName.TryGetValue(attrId, out ret))
        {
        }
        return ret;
    }

    public static string AttributeValue(int attrId, int attrValue, bool fix = true)
    {
        var ret = "";
        if (fix == false)
        {
            ret = string.Format("{0}", attrValue);
            return ret;
        }
        switch (attrId)
        {
            case 16:
            case 18:
            {
                ret = string.Format("{0}%", attrValue/10000.0f);
            }
                break;
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
                ret = string.Format("{0}%", attrValue/100.0f);
            }
                break;
            case 98:
            case 99:
            {
                //等级/
                ret = string.Format(GetDictionaryText(221000) + "/" + "{0}", attrValue);
            }
                break;
            //             case 0:
            //             case 1:
            //             case 2:
            //             case 3:
            //             case 4:
            //             case 5:
            //             case 6:
            //             case 7:
            //             case 8:
            //             case 9:
            //             case 10:
            //             case 11:
            //             case 12:
            //             case 13:
            //             case 14:
            //             case 19:
            //             case 20:
            //             case 25:
            //             case 26:
            //             case 27:
            //             case 28:
            //             case 29:
            //             case 30:
            //             case 105:
            //             case 110:
            default:
            {
                ret = string.Format("{0}", attrValue);
            }
                break;
        }
        return ret;
    }

    public static int CalculateExpByLevel(int exp, int level)
    {
        var table = Table.GetLevelData(level);
        if (null != table)
        {
            return (int) (exp/10000.0f*table.DynamicExp);
        }
        return exp;
    }

    public static bool CharacterIdIsRobot(ulong id)
    {
        if (id < 10000)
        {
            return true;
        }
        return false;
    }

    //判断武器是否可相互传承
    public static bool CheckInheritType(ItemBaseRecord tbFromEquip, ItemBaseRecord tbToEquip)
    {
        var isOk = true;
        // 10010:主手
        // 10098:单手
        // 10099:双手
        if (tbFromEquip.Type == 10010 || tbFromEquip.Type == 10098 || tbFromEquip.Type == 10099)
        {
            if (tbToEquip.Type == 10010 || tbToEquip.Type == 10098 || tbToEquip.Type == 10099)
            {
                isOk = true;
            }
            else
            {
                isOk = false;
            }
        }
        else
        {
            if (tbFromEquip.Type != tbToEquip.Type)
            {
                isOk = false;
            }
        }
        return isOk;
    }

    public static bool CheckLanguageName(string input)
    {
        var regex = new Regex(@"^[\u4E00-\u9FFFA-Za-z0-9]{0,20}$");
        return regex.IsMatch(input);
    }

    public static bool CheckName(string input)
    {
        var len = 0;

        var chineseBegain = false;
        for (var i = 0; i < input.Length; i++)
        {
            var byte_len = Encoding.Default.GetBytes(input.Substring(i, 1));
            if (byte_len.Length > 1)
            {
                len += 2; //如果长度大于1，是中文，占两个字节，+2
                if (i < 2)
                {
                    chineseBegain = true;
                }
            }
            else
            {
                len += 1; //如果长度等于1，是英文，占一个字节，+1
            }
        }

        var min = chineseBegain ? 4 : 2;

        if (min <= len && len <= 12)
        {
            return true;
        }

        return false;
    }

    public static bool CheckSensitiveName(string name)
    {
        var isSensitive = false;
        Table.ForeachSensitiveWord(record =>
        {
            if (name.IndexOf(record.Name, 0, StringComparison.OrdinalIgnoreCase) != -1)
            {
                isSensitive = true;
                return false;
            }
            return true;
        });
        return isSensitive;
    }

    public static void CleanGrid(GameObject grid)
    {
        if (null == grid)
        {
            return;
        }
        var t = grid.transform;
        for (int i = 0, count = t.childCount; i < count; i++)
        {
            Object.Destroy(t.GetChild(i).gameObject);
        }
        t.DetachChildren();
    }

    public static string ColorToString(Color c)
    {
        var ret = string.Format("{0:X2}{1:X2}{2:X2}", (int) (c.r*255), (int) (c.g*255), (int) (c.b*255));
        return ret;
    }

    public static bool ContainEmoji(string input)
    {
        var regex = new Regex(@"\uD83C[\uDF00-\uDFFF]|\uD83D[\uDC00-\uDEFF]|[\u2600-\u26FF]");
        return regex.IsMatch(input);
    }

    private static byte[] decodeBuffer = new byte[1024 * 32]; 
    public static string ConvertChatContent(string str)
    {
        ChatInfoNodeData data = null;

        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        var startIndex = str.IndexOf(SpecialCode.ChatBegin, StringComparison.Ordinal);
        if (startIndex != 0)
        {
            return str;
        }
        var endIndex = str.IndexOf(SpecialCode.ChatEnd, StringComparison.Ordinal);
        str = str.Substring(startIndex + SpecialCode.ChatBegin.Length,
            endIndex - startIndex - SpecialCode.ChatEnd.Length);
        try
        {
            var bytes = Convert.FromBase64String(str);
            var unwrapped = LZ4Codec.Decode32(bytes, 0, bytes.Length, decodeBuffer, 0, decodeBuffer.Length, false);
            using (var ms = new MemoryStream(decodeBuffer, 0, unwrapped, false))
            {
                data = Serializer.Deserialize<ChatInfoNodeData>(ms);
            }
        }
        catch (Exception)
        {
            // ignored
        }

        if (data == null)
        {
            return str;
        }
        if (data.Type != (int) eChatLinkType.Dictionary)
        {
            return str;
        }
        var ret = ConvertChatInfoNode(data);
        return ret;
    }

    public static string ConvertChatInfoNode(ChatInfoNodeData data)
    {
        var ret = "";
        if (data.Type != (int) eChatLinkType.Dictionary)
        {
            return ret;
        }

        var strDic = GetDictionaryText(data.Id);

        if (data.StrExData.Count == 0)
        {
            ret = strDic;
        }
        else
        {
            var args = new List<string>();
            {
                // foreach(var s in data.StrExData)
                var __enumerator21 = (data.StrExData).GetEnumerator();
                while (__enumerator21.MoveNext())
                {
                    var s = __enumerator21.Current;
                    {
                        args.Add(AnalyseChatInfoNodeArg(s));
                    }
                }
            }
            ret = string.Format(strDic, args.ToArray());
        }
        return ret;
    }

    //除以比例
    public static float DividePrecision(int value)
    {
        return value*_PRECISION;
    }

    public static void ExitFuben()
    {
        NetManager.Instance.StartCoroutine(ExitFubenCoroutine());
    }

    private static IEnumerator ExitFubenCoroutine()
    {
        var msg = NetManager.Instance.ExitDungeon(-1);
        yield return msg.SendAndWaitUntilDone();
    }
    public static void EnterFuben(int id)
    {
        //先检查一下，如果已经在当前副本内，则不能再次进入同一副本
        var tbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
        if (tbScene != null && tbScene.FubenId == id)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(270081));
            return;
        }

        if (GameLogic.Instance != null && GameLogic.Instance.Scene != null)
        {
            var oldTbScene = Table.GetScene(GameLogic.Instance.Scene.SceneTypeId);
            if (oldTbScene != null)
            {
                if (oldTbScene.FubenId != -1 && id != -1)
                {
                    var tbFrom = Table.GetFuben(oldTbScene.FubenId);
                    var tbTo = Table.GetFuben(id);
                    if (tbFrom != null && tbTo != null && tbFrom.AssistType != (int) eDungeonAssistType.ClimbingTower &&
                        tbTo.AssistType != (int) eDungeonAssistType.ClimbingTower)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210123));
                        return;
                    }
                }
            }
        }

        NetManager.Instance.StartCoroutine(EnterFubenCoroutine(id));
    }

    private static IEnumerator EnterFubenCoroutine(int id)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.EnterFuben(id);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //umeng
                    PlatformHelper.UMEvent("Fuben", "Enter", id);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_FubenRewardNotReceived)
                {
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_Condition)
                {
                    var strId = msg.Response;
                    if (strId != -1)
                    {
                        EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(strId));
                    }
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_MieShi_WaitTime)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(300000072));
                }
                else
                {
                    var dicId = msg.ErrorCode + 200000000;
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(dicId));
                }
            }
            else
            {
                Logger.Error(".....EnterMainDungeonCoroutine.......{0}.", msg.State);
            }
        }
    }

    public static int EquipAttrValueRef(int attrId, int attrValue)
    {
        int refValue;
        if (EquipValueRef.TryGetValue(attrId, out refValue))
        {
            return attrValue/refValue;
        }
        return attrValue;
    }

    public static int EquipAttrValueRefEx(int attrId, int attrValue)
    {
        int refValue;
        if (EquipValueRef.TryGetValue(attrId, out refValue))
        {
            return attrValue*refValue;
        }
        return attrValue;
    }

    //装备的随机属性
    public static void EquipRandomAttribute(BagItemDataModel bagItem)
    {
        var tbItem = Table.GetItemBase(bagItem.ItemId);
        var equipId = tbItem.Exdata[0];
        var tbEquip = Table.GetEquipBase(equipId);
        if (tbEquip == null)
        {
            Logger.Error("itemId  Id={0} not find equip={1}", bagItem.ItemId, equipId);
            return;
        }
        var list = new List<int>();
        //初始强化等级
        list.Add(0);
        //初始话追加属性
        list.Add(MyRandom.Random(tbEquip.AddAttrUpMinValue, tbEquip.AddAttrUpMaxValue));
        //随机卓越属性
        for (var i = 0; i != 4; ++i)
        {
            list.Add(0);
        }
        var ExcellentCount = RandomAddCount(tbEquip, tbEquip.ExcellentAttrCount);
        for (var i = 0; i != 12; ++i)
        {
            list.Add(-1);
        }
        var addCount = RandomAddCount(tbEquip, tbEquip.RandomAttrCount);
        //洗随机属性
        for (var i = 0; i != 4; ++i)
        {
            list.Add(-1);
        }
        //耐久度
        list.Add(tbEquip.Durability);
        bagItem.Exdata.InstallData(list);

        InitExcellentData(bagItem, ExcellentCount);
        InitAddAttr(bagItem, addCount);
    }

    public static void ExitLogin(Action act)
    {
        PlayerDataManager.Instance.CharacterGuid = 0ul;
        NetManager.Instance.StartCoroutine(ExitLoginCoroutine(act));
    }

    private static IEnumerator ExitLoginCoroutine(Action act)
    {
        var msg = NetManager.Instance.ExitLogin(0);
        yield return msg.SendAndWaitUntilDone();
        if (act != null)
        {
            act();
        }
    }

    public static void FlyTo(int sceneId, float x, float y, Action<int> act = null)
    {
        NetManager.Instance.StartCoroutine(FlyToCoroutine(sceneId, x, y, act));
    }

    private static IEnumerator FlyToCoroutine(int sceneId, float x, float y, Action<int> act = null)
    {
        var player = ObjManager.Instance.MyPlayer;
        if (null == player)
        {
            Logger.Debug("FlyToCoroutine null==player");
            yield break;
        }
        var vec = new Vector2Int32();
        vec.x = MultiplyPrecision(x);
        vec.y = MultiplyPrecision(y);
        var msg = NetManager.Instance.FlyTo(sceneId, vec);
        yield return msg.SendAndWaitUntilDone();

        if (act != null)
        {
            act(msg.State != MessageState.Reply ? -1 : msg.ErrorCode);
        }
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int) ErrorCodes.OK)
            {
                //player.AdjustHeightPosition(); 这里不用调，因为之后会受到SyncCharacterPostion
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                Logger.Error("FlyTo--msg.ErrorCode--{0}", msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("FlyTo--msg.State--{0}", msg.State);
        }
    }

    public static void FastReach(int sceneId, float x, float y, Action<int> act = null)
    {
        NetManager.Instance.StartCoroutine(FastReachCoroutine(sceneId, x, y, act));
    }

    private static IEnumerator FastReachCoroutine(int sceneId, float x, float y, Action<int> act = null)
    {
        var player = ObjManager.Instance.MyPlayer;
        if (null == player)
        {
            Logger.Debug("FlyToCoroutine null==player");
            yield break;
        }
        var vec = new Vector2Int32();
        vec.x = MultiplyPrecision(x);
        vec.y = MultiplyPrecision(y);
        var msg = NetManager.Instance.FastReach(sceneId, vec);
        yield return msg.SendAndWaitUntilDone();

        if (act != null)
        {
            act(msg.State != MessageState.Reply ? -1 : msg.ErrorCode);
        }
        if (msg.State == MessageState.Reply)
        {
            if (msg.ErrorCode == (int)ErrorCodes.OK)
            {
                var tableData = Table.GetEffect(2002);
                if (tableData != null)
                {
                    EffectManager.Instance.CreateEffect(tableData, player, null, null, null,
                     (tableData.BroadcastType == 0 && player.GetObjType() == OBJ.TYPE.MYPLAYER) || tableData.BroadcastType == 1); 
                }
                
                //player.AdjustHeightPosition(); 这里不用调，因为之后会受到SyncCharacterPostion
            }
            else
            {
                UIManager.Instance.ShowNetError(msg.ErrorCode);
                //Logger.Error("FastReach--msg.ErrorCode--{0}", msg.ErrorCode);
            }
        }
        else
        {
            Logger.Error("FastReach--msg.State--{0}", msg.State);
        }
    }

    public static int GetAllSkillTalentCount()
    {
        var ret = 0;
        var talentData = PlayerDataManager.Instance.mAllTalents;
        var skillTalentData = PlayerDataManager.Instance.mSkillTalent;
        var skills = PlayerDataManager.Instance.PlayerDataModel.SkillData.OtherSkills;
        var count = skills.Count;


        var enumerator = talentData.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var talent = enumerator.Current.Value;
            var modifySkillId = Table.GetTalent(talent.TalentId).ModifySkill;
            if (modifySkillId != -1)
            {
                ret += talent.Count;
            }
        }

        for (var i = 0; i < count; i++)
        {
            var skill = skills[i];
            if (skillTalentData.ContainsKey(skill.SkillId))
            {
                ret += skillTalentData[skill.SkillId];
            }
        }


        return ret;
    }

    public static string GetAttibuteFormat(int attrId, int attrValue, int attrValueEx = -1)
    {
        var ret = "";
        var name = AttributeName(attrId);
        var valueStr = AttributeValue(attrId, attrValue);
        ret = name + ":+" + valueStr;
        if (attrValueEx > 0)
        {
            var valueStrEx = AttributeValue(attrId, attrValueEx);
            ret = ret + "~" + valueStrEx;
        }
        return ret;
    }

    public static int GetAttrId(int index)
    {
        if (index > IndextoAttrId.Count || index < 0)
        {
            Logger.Error("GetAttrId index={0}", index);
            return -1;
        }
        return IndextoAttrId[index];
    }

    public static int GetAttrIndex(int attrId)
    {
        int index;
        if (AttrIdtoIndex.TryGetValue(attrId, out index))
        {
            return index;
        }
        Logger.Error("GetAttrIndex attrId={0}", attrId);
        return -1;
    }

    public static int GetAttrIndexElf(int attrId)
    {
        int index;
        if (AttrIdtoIndexElf.TryGetValue(attrId, out index))
        {
            return index;
        }
        //Logger.Error("Elf GetAttrIndex attrId={0}", attrId);
        return -1;
    }

    public static int GetBaseAttr(EquipBaseRecord tbEquip, int nLevel, int nIndex, int nAttrId)
    {
        var nValue = tbEquip.BaseValue[nIndex];

        if (nValue == 0)
        {
            return 0;
        }
        var attr = (eAttributeType) nAttrId;
        switch (attr)
        {
            case eAttributeType.PhyPowerMin:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.PhyPowerMinScale/100 + tblevel.PhyPowerMinFix;
                }
            }
                break;
            case eAttributeType.PhyPowerMax:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.PhyPowerMaxScale/100 + tblevel.PhyPowerMaxFix;
                }
            }
                break;
            case eAttributeType.MagPowerMin:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.MagPowerMinScale/100 + tblevel.MagPowerMinFix;
                }
            }
                break;
            case eAttributeType.MagPowerMax:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.MagPowerMaxScale/100 + tblevel.MagPowerMaxFix;
                }
            }
                break;
            case eAttributeType.PhyArmor:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.PhyArmorScale/100 + tblevel.PhyArmorFix;
                }
            }
                break;
            case eAttributeType.MagArmor:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.MagArmorScale/100 + tblevel.MagArmorFix;
                }
            }
                break;
            case eAttributeType.HpMax:
            {
                var tblevel = Table.GetLevelData(nLevel);
                if (tblevel != null)
                {
                    return nValue*tblevel.HpMaxScale/100 + tblevel.HpMaxFix;
                }
            }
                break;

            default:
                return nValue;
        }

        return nValue;
    }

    //索引属性修正
    public static int GetBaseAttrIdElf(int index)
    {
        if (index > RefIndextoAttrIdElf.Count || index < 0)
        {
            Logger.Error("Elf GetBaseAttrId index={0}", index);
            return -1;
        }
        return RefIndextoAttrIdElf[index];
    }

    //大数字显示
    public static string GetBigValueStr(int value)
    {
        if (value >= 100000000)
        {
            return string.Format(GetDictionaryText(1505), value/1000000/100.0f);
        }
        if (value >= 1000000)
        {
            return string.Format(GetDictionaryText(1506), value/1000/10.0f);
        }
        return value.ToString();
    }

    public static int GetBSParamByIndex(int typeId, BuildingServiceRecord tbBS, List<int> pets, int index)
    {
        var tbValue = tbBS.Param[index];

        var table = Table.GetBuilding(typeId);

        for (var idx = 0; idx < pets.Count; idx++)
        {
            var petId = pets[idx];
            var petdata = CityManager.Instance.GetPetById(petId);
            if (null == petdata)
            {
                continue;
            }

            var petinfo = Table.GetPet(petId);
            if (petinfo == null)
            {
                continue;
            }
            int[] SkillLevelLimit =
            {
                petinfo.Speciality[0],
                petinfo.Speciality[1],
                petinfo.Speciality[2]
            };
            //特殊技能
            const int PetItemDataModelMaxSkill2 = PetItemDataModel.MaxSkill;


            for (var i = 0; i < PetItemDataModelMaxSkill2; i++)
            {
                if (petdata.Exdata[PetItemExtDataIdx.Level] < SkillLevelLimit[i] ||
                    petdata.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + i] == -1)
                {
                    continue;
                }
                var Bili = 10000;
                var skill = Table.GetPetSkill(petdata.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + i]);
                if (table.Type == skill.Param[0] && skill.EffectId == 0)
                {
                    if (index == skill.Param[1])
                    {
                        Bili += skill.Param[2];
                    }
                    if (index == skill.Param[3])
                    {
                        Bili += skill.Param[4];
                    }
                    tbValue *= Bili/10000;
                }
            }
        }

        return tbValue;
    }

    public static string GetChannelString()
    {
#if UNITY_EDITOR
        return "Uborm";
#endif

        string versionConfig;
        var gameVersionPath = Path.Combine(Application.streamingAssetsPath, "Game.ver");
        if (!GetStringFromPackage(gameVersionPath, out versionConfig))
        {
            Logger.Error("cant find Game.ver at{0}", gameVersionPath);
            return "";
        }

        var config = versionConfig.Split(',');
        if (config.Length < 3)
        {
            Logger.Error("parse game.ver failed:{0}", gameVersionPath);
            return "";
        }

        return config[2];
    }

    public static string GetDictionaryText(int id)
    {
        var tbDic = Table.GetDictionary(id);
        if (tbDic == null)
        {
            return "";
        }
        return tbDic.Desc[LanguageIndex];
    }

    public static int GetEquipBagId(EquipBaseRecord tbEquip)
    {
        for (var i = 7; i <= 18; i++)
        {
            if (IsCanEquip(tbEquip, i))
            {
                return i;
            }
        }
        return -1;
    }

    public static string GetFullPath(Transform o)
    {
        var l = new List<string>();
        while (o.parent != null)
        {
            l.Add(o.name);
            o = o.parent;
        }

        l.Reverse();
        var sb = new StringBuilder();
        {
            var __list1 = l;
            var __listCount1 = __list1.Count;
            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var str = __list1[__i1];
                {
                    sb.Append(str);
                    sb.Append(".");
                }
            }
        }
        return sb.ToString();
    }

    public static bool GetIntFromFile(string path, out int retInt)
    {
        try
        {
            if (!File.Exists(path))
            {
                retInt = 0;
                return false;
            }
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var sr = new StreamReader(fs);
            var text = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            sr.Dispose();
            fs.Dispose();

            if (!int.TryParse(text, out retInt))
            {
                Logger.Error("parse int from file error , path:" + path);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("GetIntFromFile throw exception!");
            Logger.Error(ex.ToString());
            retInt = 0;
            return false;
        }
    }

    public static eItemInfoType GetItemInfoType(int type)
    {
        if (type == 10005)
        {
            return eItemInfoType.Wing;
        }
        if (type == 40000)
        {
            return eItemInfoType.Elf;
        }
        if (type < 10000
            || type > 10099)
        {
            return eItemInfoType.Item;
        }
        return eItemInfoType.Equip;
    }

    //到目前为止持续时间
    public static string GetLastTimeDiffString(DateTime serverTime)
    {
        return GetLastTimeDiffString(Game.Instance.ServerTime - serverTime);
    }

    public static string GetLastTimeDiffString(TimeSpan span)
    {
        if (span.Days > 365)
        {
            return string.Format(GetDictionaryText(220960), span.Days/365); //n年前
        }
        if (span.Days > 30)
        {
            return string.Format(GetDictionaryText(220959), span.Days/30); //n月前
        }
        if (span.Days > 0)
        {
            return string.Format(GetDictionaryText(220958), span.Days); //n天前
        }
        if (span.Minutes > 60)
        {
            return string.Format(GetDictionaryText(220957), span.Minutes/60); //n小时前
        }
        if (span.Minutes > 0)
        {
            return string.Format(GetDictionaryText(220956), span.Minutes); //n分前
        }

        return string.Format(GetDictionaryText(220954)); //离线
    }

    public static string GetMd5Hash(string fileName)
    {
        var md5 = new MD5();
        var fs = new FileStream(fileName, FileMode.Open);
        var bytes = new byte[fs.Length];
        fs.Read(bytes, 0, bytes.Length);
        fs.Close();
        md5.ValueAsByte = bytes;
        return md5.FingerPrint.ToLower();
    }

    public static string GetNoCacheUrl(string url)
    {
        return url;
        var sb = new StringBuilder(url);
        sb.Append("?nocache=");
        sb.Append(Guid.NewGuid());
        return sb.ToString();
    }

    public static float GetResolutionRadio()
    {
        var radios = GameSetting.Instance.ResolutionRadio;
        var level = GameSetting.Instance.GameResolutionLevel;
        if (level > 0 && level < radios.Length)
        {
            return radios[level];
        }
        return 1.0f;
    }

    /// <summary>
    /// </summary>
    /// <param name="tbFuben">FubenRecord的引用</param>
    /// <param name="count">表里配的奖励数量</param>
    /// <param name="level1">用来计算星级奖励的level</param>
    /// <param name="level2">用来计算动态奖励的level</param>
    /// <returns></returns>
    public static int GetRewardCount(FubenRecord tbFuben, int count, int level1 = 0, int level2 = 0)
    {
        if (tbFuben.IsStarReward == 1)
        {
//星级奖励
            if (tbFuben.IsDyncReward == 1)
            {
//动态奖励
                if (count <= 0)
                {
                    return -1;
                }
                var tbUp1 = Table.GetSkillUpgrading(count);
                if (tbUp1 == null)
                {
                    Logger.Error("In GetRewardCount() tbUp1 == null! fuben id = {0}, ", tbFuben.Id);
                    return -1;
                }
                var suId = tbUp1.GetSkillUpgradingValue(level1);
                var tbUp2 = Table.GetSkillUpgrading(suId);
                if (tbUp2 == null)
                {
                    Logger.Error("In GetRewardCount() tbUp2 == null! fuben id = {0}, ", tbFuben.Id);
                    return 0;
                }
                return tbUp2.GetSkillUpgradingValue(level2);
            }
            else
            {
                var tbUp1 = Table.GetSkillUpgrading(count);
                if (tbUp1 == null)
                {
                    Logger.Error("In GetRewardCount() tbUp1 == null! fuben id = {0}, ", tbFuben.Id);
                    return -1;
                }
                return tbUp1.GetSkillUpgradingValue(level1);
            }
        }
        if (tbFuben.IsDyncReward == 1)
        {
//动态奖励
            var tbUp1 = Table.GetSkillUpgrading(count);
            if (tbUp1 == null)
            {
                Logger.Error("In GetRewardCount() tbUp1 == null! fuben id = {0}, ", tbFuben.Id);
                return -1;
            }
            return tbUp1.GetSkillUpgradingValue(level2);
        }
        return count;
    }

    public static string GetServerAddress()
    {
        return UpdateHelper.GateAddress;

//         var path1 = Path.Combine(UpdateHelper.DownloadRoot, "ip.txt");
//         if (File.Exists(path1))
//         {
//             return File.ReadAllText(path1).Trim();
//         }
// 
//         var path2 = Path.Combine(Application.streamingAssetsPath, "ip.txt");
//         string ip;
//         if (!GameUtils.GetStringFromPackage(path2, out ip))
//         {
//             Logger.Error("cant find ip.txt on streamingAssetPath");
//             return "";
//         }
//         Logger.Debug("get Ip address : {0}", ip);
// 
//         return ip;
    }

    public static string GetServerName(int serverId)
    {
        var tbServerName = Table.GetServerName(serverId);
        if (tbServerName != null)
        {
            return tbServerName.Name;
        }
        return GetDictionaryText(2000006);
    }

    public static string GetStreamingAssetPath()
    {
        var strStreamingPath = "";
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            strStreamingPath = Application.dataPath + "/Raw";
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            strStreamingPath = Application.streamingAssetsPath;
        }
        else
        {
            strStreamingPath = Application.streamingAssetsPath; //File
        }
        return strStreamingPath;
    }

    public static bool GetStringFromFile(string path, out string retString)
    {
        try
        {
            if (!File.Exists(path))
            {
                retString = "";
                return false;
            }
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var sr = new StreamReader(fs);
            retString = sr.ReadToEnd().Trim();
            sr.Close();
            fs.Close();
            sr.Dispose();
            fs.Dispose();

            if (string.IsNullOrEmpty(retString))
            {
                Logger.Error("get string from file error , path:" + path);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("GetStringFromFile throw exception!");
            Logger.Error(ex.ToString());
            retString = "";
            return false;
        }
    }

    public static bool GetStringFromPackage(string path, out string retString)
    {
        try
        {
            Stream stream = null;
#if UNITY_ANDROID && !UNITY_EDITOR
        var apkPath = Application.dataPath;
        var filestream = new FileStream(apkPath, FileMode.Open, FileAccess.Read);
        var ZipFile = new ZipFile(filestream);
        var filepath = path.Substring(path.IndexOf("assets/"));
        var item = ZipFile.GetEntry(filepath);
        if (null != item)
        {
            stream = ZipFile.GetInputStream(item);
        }
        else
        {
            Logger.Error("GetStringFromPackage error , filepath:" + filepath);
            ZipFile.Close();
            filestream.Close();
            retString = "";
            return false;
        }
#else
            if (!File.Exists(path))
            {
                retString = "";
                return false;
            }
            stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
#endif
            var sr = new StreamReader(stream);
            retString = sr.ReadToEnd().Trim();
#if UNITY_ANDROID && !UNITY_EDITOR
            ZipFile.Close();
            filestream.Close();
#endif
            sr.Close();
            stream.Close();
            sr.Dispose();
            stream.Dispose();

            if (string.IsNullOrEmpty(retString))
            {
                Logger.Error("parse int from file error , path:" + path);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("GetStringFromFile throw exception!");
            Logger.Error(ex.ToString());
            retString = "";
            return false;
        }
    }

    public static Color GetTableColor(int id)
    {
        var tb = Table.GetColorBase(id);
        if (tb == null)
        {
            return Color.white;
        }
        return new Color(tb.Red/255.0f, tb.Green/255.0f, tb.Blue/255.0f, tb.Alpha/255.0f);
    }

    public static string GetTableColorString(int id)
    {
        Color color;
        var tb = Table.GetColorBase(id);
        if (tb == null)
        {
            return "FFFFFF";
        }
        var ret = string.Format("{0:X2}{1:X2}{2:X2}", tb.Red, tb.Green, tb.Blue);
        return ret;
    }

    public static string GetTimeDiffString(TimeSpan span, bool trim = false)
    {
        if (span.Days > 0)
        {
            if (trim && span.Hours == 0)
            {
                return string.Format(GetDictionaryText(210406), span.Days);
            }
            return string.Format(GetDictionaryText(210402), span.Days, span.Hours);
        }
        if (span.Hours > 0)
        {
            if (trim && span.Minutes == 0)
            {
                return string.Format(GetDictionaryText(210407), span.Hours);
            }
            return string.Format(GetDictionaryText(210403), span.Hours, span.Minutes);
        }
        if (span.Minutes > 0)
        {
            if (trim && span.Seconds == 0)
            {
                return string.Format(GetDictionaryText(210408), span.Minutes);
            }
            return string.Format(GetDictionaryText(210404), span.Minutes, span.Seconds);
        }
        if (span.Seconds >= 0)
        {
            return string.Format(GetDictionaryText(210405), span.Seconds);
        }
        return string.Empty;
    }

    public static string GetTimeDiffString(int seconds, bool trim = false)
    {
        return GetTimeDiffString(new TimeSpan(0, 0, seconds), trim);
    }

    //倒计时  
    public static string GetTimeDiffString(DateTime serverTime, bool trim = false)
    {
        return GetTimeDiffString(serverTime - Game.Instance.ServerTime, trim);
    }

    private static List<int> ShowEquipModelId = null;
    public static bool ShowEquipModel(int itemId, bool force = false)
    {
        if (null == ShowEquipModelId)
        {
            ShowEquipModelId = new List<int>();
            var temp = Table.GetClientConfig(1206).Value.Split('|');
            foreach (var t in temp)
            {
                ShowEquipModelId.Add(int.Parse(t));
            }
        }

        if (force || ShowEquipModelId.Contains(itemId))
        {
            GameUtils.GotoUiTab(92, -1, itemId);
            return true;
        }

        return false;
    }

    //显示某个UI的某个Tab页
    public static void GotoUiTab(int uiId, int arg0, int arg1 = -1, int arg2 = -1)
    {
        if (uiId == -1)
        {
            return;
        }
        var c = UIConfig.GetConfig(uiId);
        if (c == null)
        {
            return;
        }
        var arg = c.NewArgument();
        arg.Tab = arg0;
        if (arg1 != -1)
        {
            arg.Args = new List<int>();
            arg.Args.Add(arg1);
            if (arg2 != -1)
            {
                arg.Args.Add(arg2);
            }
        }
        if (c.UiRecord == UIConfig.AreanaUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator19 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator19.MoveNext())
                {
                    var data = __enumerator19.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.ArenaTemple)
                        {
                            var ee = new Show_UI_Event(UIConfig.AreanaUI, new ArenaArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId),
                                Tab = arg.Tab
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.SailingUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.BraveHarbor)
                        {
                            var ee = new Show_UI_Event(UIConfig.SailingUI, new SailingArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId),
                                Tab = arg.Tab
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.SmithyUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.BlacksmithShop)
                        {
                            var ee = new Show_UI_Event(UIConfig.SmithyUI, new SmithyFrameArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId),
                                Tab = arg.Tab
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.ComposeUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.CompositeHouse)
                        {
                            var ee = new Show_UI_Event(UIConfig.ComposeUI, new ComposeArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId),
                                Tab = arg0
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.FarmUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.Farm)
                        {
                            var ee = new Show_UI_Event(UIConfig.FarmUI, new FarmArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId)
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.WishingUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.WishingPool)
                        {
                            var ee = new Show_UI_Event(UIConfig.WishingUI, new WishingArguments
                            {
                                Tab = arg0,
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId)
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.BattleUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.WarHall)
                        {
                            var ee = new Show_UI_Event(UIConfig.BattleUI, new BattleArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId)
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }
        if (c.UiRecord == UIConfig.TradingUI.UiRecord)
        {
            {
                // foreach(var data in CityManager.Instance.BuildingDataList)
                var __enumerator20 = (CityManager.Instance.BuildingDataList).GetEnumerator();
                while (__enumerator20.MoveNext())
                {
                    var data = __enumerator20.Current;
                    {
                        var tbBuilding = Table.GetBuilding(data.TypeId);
                        if (tbBuilding == null)
                        {
                            continue;
                        }
                        if ((BuildingType) tbBuilding.Type == BuildingType.Exchange)
                        {
                            var ee = new Show_UI_Event(UIConfig.TradingUI, new TradingArguments
                            {
                                BuildingData = CityManager.Instance.GetBuildingByAreaId(data.AreaId)
                            });
                            EventDispatcher.Instance.DispatchEvent(ee);
                            return;
                        }
                    }
                }
            }
        }

        var e = new Show_UI_Event(c, arg);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public static void GuideToBuyVip(int vipLevel, int dicId = 270247)
    {
        var content = string.Format(GetDictionaryText(dicId), vipLevel);
        GuideToBuyVip(content);
    }

    public static void GuideToBuyVip(string content)
    {
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, content, GetDictionaryText(0),
            () => { GotoUiTab(79, 0); });
    }

    public static void Init()
    {
        AttrName.Clear();
        AttrName[0] = GetDictionaryText(221000);
        AttrName[1] = GetDictionaryText(221001);
        AttrName[2] = GetDictionaryText(221002);
        AttrName[3] = GetDictionaryText(221003);
        AttrName[4] = GetDictionaryText(221004);
        AttrName[5] = GetDictionaryText(221005);
        AttrName[6] = GetDictionaryText(221006);
        AttrName[7] = GetDictionaryText(221007);
        AttrName[8] = GetDictionaryText(221008);
        AttrName[9] = GetDictionaryText(221009);
        AttrName[10] = GetDictionaryText(221010);
        AttrName[11] = GetDictionaryText(221011);
        AttrName[12] = GetDictionaryText(221012);
        AttrName[13] = GetDictionaryText(221013);
        AttrName[14] = GetDictionaryText(221014);
        AttrName[15] = GetDictionaryText(221015);
        AttrName[16] = GetDictionaryText(221016);
        AttrName[17] = GetDictionaryText(221017);
        AttrName[18] = GetDictionaryText(221018);
        AttrName[19] = GetDictionaryText(221019);
        AttrName[20] = GetDictionaryText(221020);
        AttrName[21] = GetDictionaryText(221021);
        AttrName[22] = GetDictionaryText(221022);
        AttrName[23] = GetDictionaryText(221023);
        AttrName[24] = GetDictionaryText(221024);
        AttrName[25] = GetDictionaryText(221025);
        AttrName[26] = GetDictionaryText(221026);

        AttrName[(int)eAttributeType.FireAttack] = GetDictionaryText(221031); //"火属性攻击"
        AttrName[(int)eAttributeType.IceAttack] = GetDictionaryText(221032); //"冰属性攻击"
        AttrName[(int)eAttributeType.PoisonAttack] = GetDictionaryText(221033); //"毒属性攻击"
        AttrName[(int)eAttributeType.FireResistance] = GetDictionaryText(221034); //"火属性抗性"
        AttrName[(int)eAttributeType.IceResistance] = GetDictionaryText(221035); //"冰属性抗性"
        AttrName[(int)eAttributeType.PoisonResistance] = GetDictionaryText(221036); //"毒属性抗性"
        AttrName[(int)eAttributeType.HpNow] = GetDictionaryText(221027);
        AttrName[(int)eAttributeType.MpNow] = GetDictionaryText(221028);
        //AttrName[29] = GetDictionaryText(221029);

        AttrName[98] = GetDictionaryText(221098);
        AttrName[99] = GetDictionaryText(221099);

        AttrName[105] = GetDictionaryText(221105);
        AttrName[106] = GetDictionaryText(221106);

        AttrName[110] = GetDictionaryText(221110);
        AttrName[111] = GetDictionaryText(221111);

        AttrName[113] = GetDictionaryText(221113);
        AttrName[114] = GetDictionaryText(221114);

        AttrName[119] = GetDictionaryText(221119);
        AttrName[120] = GetDictionaryText(221120);

        AttrName[1001] = GetDictionaryText(222001);
        AttrName[1002] = GetDictionaryText(222002);
        AttrName[1003] = GetDictionaryText(221098); //"攻击";


        TimeOver = GetDictionaryText(1051);
        AutoPickUpDistance = Table.GetClientConfig(401).Value.ToInt()/100.0f;
        AutoPickUpDistanceMax = Table.GetClientConfig(402).Value.ToInt()/100.0f;

        ChatWorldCount = Table.GetClientConfig(310).Value.ToInt();
        HornWorldCount = Table.GetClientConfig(311).Value.ToInt();

        DungeonShowDelay = Table.GetClientConfig(416).Value.ToInt();


        AutoFightShortDistance = Table.GetClientConfig(1000).Value.ToInt();
        AutoFightLongDistance = Table.GetClientConfig(999).Value.ToInt();

        ConditionToFlag.Clear();
        ElfFlagConfig = Table.GetClientConfig(563).Value.ToInt();
        ConditionToFlag.Add(ElfFlagConfig, Table.GetConditionTable(ElfFlagConfig).TrueFlag[0]);
        HandBookGroupFlagConfig = Table.GetClientConfig(570).Value.ToInt();
        ConditionToFlag.Add(HandBookGroupFlagConfig, Table.GetConditionTable(HandBookGroupFlagConfig).TrueFlag[0]);
        HandBookWantedFlagConfig = Table.GetClientConfig(564).Value.ToInt();
        ConditionToFlag.Add(HandBookWantedFlagConfig, Table.GetConditionTable(HandBookWantedFlagConfig).TrueFlag[0]);
        SkillTalentFlagConfig = Table.GetClientConfig(565).Value.ToInt();
        ConditionToFlag.Add(SkillTalentFlagConfig, Table.GetConditionTable(SkillTalentFlagConfig).TrueFlag[0]);
        SkillInnateFlagConfig = Table.GetClientConfig(569).Value.ToInt();
        ConditionToFlag.Add(SkillInnateFlagConfig, Table.GetConditionTable(SkillInnateFlagConfig).TrueFlag[0]);
        AchieveFlagConfig = Table.GetClientConfig(566).Value.ToInt();
        ConditionToFlag.Add(AchieveFlagConfig, Table.GetConditionTable(AchieveFlagConfig).TrueFlag[0]);
        MailFlagConfig = Table.GetClientConfig(567).Value.ToInt();
        ConditionToFlag.Add(MailFlagConfig, Table.GetConditionTable(MailFlagConfig).TrueFlag[0]);
        RankFlagConfig = Table.GetClientConfig(568).Value.ToInt();
        ConditionToFlag.Add(RankFlagConfig, Table.GetConditionTable(RankFlagConfig).TrueFlag[0]);

        FriendFlagConfig = Table.GetClientConfig(220).Value.ToInt();
        ConditionToFlag.Add(FriendFlagConfig, Table.GetConditionTable(FriendFlagConfig).TrueFlag[0]);
        TeamFlagConfig = Table.GetClientConfig(219).Value.ToInt();
        ConditionToFlag.Add(TeamFlagConfig, Table.GetConditionTable(TeamFlagConfig).TrueFlag[0]);
        UnionFlagConfig = Table.GetClientConfig(221).Value.ToInt();
        ConditionToFlag.Add(UnionFlagConfig, Table.GetConditionTable(UnionFlagConfig).TrueFlag[0]);
        WishFlagReward = Table.GetRewardInfo(0).ConditionId;
        ConditionToFlag.Add(WishFlagReward, Table.GetConditionTable(WishFlagReward).TrueFlag[0]);


        AutoMedicineHpCd = Table.GetClientConfig(1001).Value.ToInt();
        AutoMedicineMpCd = Table.GetClientConfig(1002).Value.ToInt();

        //订单获取花费钻石（每5分钟）
        OrderRefreshCost = Table.GetClientConfig(571).Value.ToInt();
        //翅膀最大阶数
        WingQualityMax = Table.GetClientConfig(499).Value.ToInt();

        //精灵助战栏1开启条件
        ElfSecondCondition = Table.GetClientConfig(105).Value.ToInt();
        //精灵助战栏2开启条件
        ElfThirdCondition = Table.GetClientConfig(106).Value.ToInt();
        //喇叭广播时间间隔秒
        TrumpetDurationTime = Table.GetClientConfig(339).Value.ToInt()/1000.0f;
        //喇叭广播移动速度
        TrumpeMoveSpeedt = Table.GetClientConfig(340).Value.ToInt();
        //
        PlayerInfoCacheTime = Table.GetClientConfig(107).Value.ToInt();

        SystemNoticeRollingScreenLimit = Table.GetClientConfig(341).ToInt();
        SystemNoticeScrollingSpeed = Table.GetClientConfig(342).ToInt();

        FubenStar1Time = Table.GetClientConfig(382).ToInt();
        FubenStar2Time = Table.GetClientConfig(383).ToInt();
        FubenStar3Time = Table.GetClientConfig(384).ToInt();
        MaxMailCount = 50;
        //-------------------------------------------------dictionary-----------------
        GianItemTip = Table.GetDictionary(210200);
        StarIcon = GetDictionaryText(270235);

        RankWorshipAction[0] = Table.GetClientConfig(247).ToInt();
        RankWorshipAction[1] = Table.GetClientConfig(248).ToInt();
        RankWorshipAction[2] = Table.GetClientConfig(249).ToInt();
        DistanceRemoveTarget = Table.GetClientConfig(997).ToInt();

        BlockLayerDuration = Table.GetClientConfig(281).ToInt()/1000.0f;

        OfflineExpRatelimit = Table.GetClientConfig(588).ToInt()/100.0f;
        MaxLevel = Table.GetClientConfig(103).ToInt();

        SweepCouponId = Table.GetClientConfig(385).ToInt();

        MaxMailCount = Table.GetClientConfig(284).ToInt();

        MColor.Init();
    }

    public static void InitAddAttr(BagItemDataModel bagItem, int addCount)
    {
        var tbItem = Table.GetItemBase(bagItem.ItemId);
        var equipId = tbItem.Exdata[0];
        var tbEquip = Table.GetEquipBase(equipId);

        if (addCount <= 0 || addCount > 6)
        {
            return;
        }
        int nRandom, nTotleRandom;
        var TbAttrPro = Table.GetEquipEnchantChance(tbEquip.RandomAttrPro);
        if (TbAttrPro == null)
        {
            Logger.Error("Equip InitAddAttr Id={0} not find EquipEnchantChance Id={1}", tbEquip.Id,
                tbEquip.RandomAttrPro);
            return;
        }
        var tempAttrPro = new Dictionary<int, int>();
        var nTotleAttrPro = 0;
        for (var i = 0; i != 20; ++i)
        {
            var nAttrpro = TbAttrPro.Attr[i];
            if (nAttrpro > 0)
            {
                nTotleAttrPro += nAttrpro;
                tempAttrPro[i] = nAttrpro;
            }
        }
        //属性值都在这里
        var tbEnchant = Table.GetEquipEnchant(tbEquip.RandomAttrValue);
        if (tbEnchant == null)
        {
            Logger.Error("Equip InitAddAttr Id={0} not find tbEquipEnchant Id={1}", tbEquip.Id, tbEquip.RandomAttrValue);
            return;
        }
        //整理概率
        var AttrValue = new Dictionary<int, int>();
        for (var i = 0; i != addCount; ++i)
        {
            nRandom = MyRandom.Random(nTotleAttrPro);
            nTotleRandom = 0;
            {
                // foreach(var i1 in tempAttrPro)
                var __enumerator2 = (tempAttrPro).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var i1 = __enumerator2.Current;
                    {
                        nTotleRandom += i1.Value;
                        if (nRandom < nTotleRandom)
                        {
                            //AddCount = i1.Key;
                            AttrValue[i1.Key] = tbEnchant.Attr[i1.Key];
                            nTotleAttrPro -= i1.Value;
                            tempAttrPro.Remove(i1.Key);
                            break;
                        }
                    }
                }
            }
        }
        var NowAttrCount = AttrValue.Count;
        if (NowAttrCount < addCount)
        {
            Logger.Error("Equip InitAddAttr AddAttr Not Enough AddCount={0},NowAttrCount={1}", addCount, NowAttrCount);
        }

        var tbAttrRelate = Table.GetEquipRelate(tbEquip.RandomAttrInterval);
        if (tbAttrRelate == null)
        {
            Logger.Error("Equip tbAttrRelate Id={0} not find EquipRelate Id={1}", tbEquip.Id, tbEquip.RandomAttrInterval);
            return;
        }

        for (var i = 0; i != NowAttrCount; ++i)
        {
            var nKey = AttrValue.Keys.Min();
            var nAttrId = GetAttrId(nKey);
            if (nAttrId == -1)
            {
                continue;
            }
            var fValue = tbEnchant.Attr[nKey];
            var AttrValueMin = fValue*tbEquip.RandomValueMin/100;
            var AttrValueMax = fValue*tbEquip.RandomValueMax/100;
            var AttrValueDiff = AttrValueMax - AttrValueMin;
            nRandom = MyRandom.Random(10000);
            nTotleRandom = 0;
            var rMin = 0;
            var rMax = 10000;
            for (var j = 0; j != tbAttrRelate.Value.Length; ++j)
            {
                nTotleRandom += tbAttrRelate.Value[j];
                if (nRandom < nTotleRandom)
                {
                    switch (j)
                    {
                        case 0:
                        {
                            rMin = 0;
                            rMax = 5000;
                        }
                            break;
                        case 1:
                        {
                            rMin = 5000;
                            rMax = 7500;
                        }
                            break;
                        case 2:
                        {
                            rMin = 7500;
                            rMax = 9000;
                        }
                            break;
                        case 3:
                        {
                            rMin = 9000;
                            rMax = 10000;
                        }
                            break;
                    }
                    break;
                }
            }
            var AttrValueMin2 = AttrValueMin + AttrValueDiff*rMin/10000;
            var AttrValueMax2 = AttrValueMin + AttrValueDiff*rMax/10000;
            var AttrRealValue = MyRandom.Random(AttrValueMin2, AttrValueMax2);

            bagItem.Exdata[i + 6] = nAttrId;
            bagItem.Exdata[i + 12] = AttrRealValue;

            AttrValue.Remove(nKey);
        }
    }

    private static void InitExcellentData(BagItemDataModel bagItem, int NowAttrCount)
    {
        var tbItem = Table.GetItemBase(bagItem.ItemId);
        var equipId = tbItem.Exdata[0];
        var tbEquip = Table.GetEquipBase(equipId);

        if (NowAttrCount <= 0 || NowAttrCount > 4)
        {
            return;
        }
        var tbEnchant = Table.GetEquipEnchant(tbEquip.ExcellentAttrValue);
        if (tbEnchant == null)
        {
            Logger.Error("InitExcellentData:Equip Id={0} not find ExcellentAttrValue Id={1}", tbEquip.Id,
                tbEquip.ExcellentAttrValue);
            return;
        }
        var tbAttrRelate = Table.GetEquipRelate(tbEquip.ExcellentAttrInterval);
        if (tbAttrRelate == null)
        {
            Logger.Error("InitExcellentData:Equip Id={0} not find EquipRelate Id={1}", tbEquip.Id,
                tbEquip.ExcellentAttrInterval);
            return;
        }
        int nRandom, nTotleRandom;
        for (var i = 0; i != NowAttrCount; ++i)
        {
            var nAttrId = tbEquip.ExcellentAttrId[i];
            var index = GetAttrIndex(nAttrId);
            if (index == -1)
            {
                continue;
            }
            var fValue = tbEnchant.Attr[index];
            var AttrValueMin = fValue*tbEquip.ExcellentValueMin/100;
            var AttrValueMax = fValue*tbEquip.ExcellentValueMax/100;
            var AttrValueDiff = AttrValueMax - AttrValueMin;
            //int nRandomAttr = tbEquip.ExcellentValueMax - tbEquip.ExcellentValueMin;
            nRandom = MyRandom.Random(10000);
            nTotleRandom = 0;
            //int AttrValueMAX = tbEquip.RandomValueMax * fValue;
            var rMin = 0;
            var rMax = 10000;
            for (var j = 0; j != tbAttrRelate.Value.Length; ++j)
            {
                nTotleRandom += tbAttrRelate.Value[j];
                if (nRandom < nTotleRandom)
                {
                    switch (j)
                    {
                        case 0:
                        {
                            rMin = 0;
                            rMax = 5000;
                        }
                            break;
                        case 1:
                        {
                            rMin = 5000;
                            rMax = 7500;
                        }
                            break;
                        case 2:
                        {
                            rMin = 7500;
                            rMax = 9000;
                        }
                            break;
                        case 3:
                        {
                            rMin = 9000;
                            rMax = 10000;
                        }
                            break;
                    }
                    break;
                }
            }
            var AttrValueMin2 = AttrValueMin + AttrValueDiff*rMin/10000;
            var AttrValueMax2 = AttrValueMin + AttrValueDiff*rMax/10000;
            var AttrValue = MyRandom.Random(AttrValueMin2, AttrValueMax2);
            bagItem.Exdata[i + 2] = AttrValue;
        }
    }

    public static void InitSkillBook(ItemInfoDataModel dataModel)
    {
        var itemId = dataModel.ItemData.ItemId;
        var tbItem = Table.GetItemBase(itemId);

        dataModel.AttributeType = tbItem.Exdata[2];
        dataModel.AttributeValue = tbItem.Exdata[3];

        var str = AttributeName(dataModel.AttributeType);
        var dic = GetDictionaryText(240203);
        dataModel.AttributeName = string.Format(dic, str);
        var attr = PlayerDataManager.Instance.GetAttribute(dataModel.AttributeType);
        if (attr < dataModel.AttributeValue)
        {
            dataModel.AttributeLimit = string.Format("{0}", dataModel.AttributeValue);
            dataModel.AttributeColor = MColor.red;
        }
        else
        {
            dataModel.AttributeLimit = string.Format("{0}", dataModel.AttributeValue);
            dataModel.AttributeColor = MColor.green;
        }
        var skillid = Table.GetItemBase(dataModel.ItemData.ItemId).Exdata[0];
        if (skillid == -1)
        {
            return;
        }
        var skillTalentData = PlayerDataManager.Instance.mSkillTalent;

        var talentData = PlayerDataManager.Instance.mAllTalents;
        var talentAddCount = 0;
        {
            // foreach(var talent in talentData)
            var __enumerator4 = (talentData).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var talent = __enumerator4.Current;
                {
                    if (skillid == Table.GetTalent(talent.Key).ModifySkill)
                    {
                        talentAddCount += talent.Value.Count;
                    }
                }
            }
        }
        var cur = 0;
        if (!skillTalentData.TryGetValue(skillid, out cur))
        {
            cur = 0;
        }
        else
        {
            cur = skillTalentData[skillid];
        }
        var table = Table.GetSkill(skillid);
        if (table == null)
        {
            return;
        }
        dataModel.Tips = string.Format(Table.GetItemBase(dataModel.ItemData.ItemId).Desc,
            table.Name,
            (cur + talentAddCount) + "/" +
            table.TalentMax);
    }

    public static void InvokeWebServer(string url, Action<Stream> act)
    {
        var request = WebRequest.Create(url) as HttpWebRequest;
        request.Method = "GET";
        //request.Method = "POST";
        request.UserAgent = string.Empty;
        request.Timeout = 10;
        request.BeginGetResponse(result =>
        {
            var webRequest = ((HttpWebRequest) result.AsyncState);
            WebResponse webResponse = null;
            try
            {
                webResponse = webRequest.EndGetResponse(result);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            if (webResponse == null)
            {
                return;
            }
            lock (NetManager.Instance.EventQueue)
            {
                NetManager.Instance.EventQueue.Enqueue(new DoSomethingEvent(() =>
                {
                    var stream = webResponse.GetResponseStream();
                    if (act != null)
                    {
                        act(stream);
                    }
                }));
            }
        }, request);
    }

    public static bool IsCanEquip(EquipBaseRecord tbEquip, int PartBag)
    {
        return BitFlag.GetLow(tbEquip.Part, PartBag - 7);
    }

    public static bool IsMyEnemy(int Camp1, int Camp2)
    {
        var TableCamp = Table.GetCamp(Camp1);
        if (null == TableCamp)
        {
            return false;
        }

        if (Camp2 < 0 || Camp2 >= TableCamp.Camp.Length)
        {
            return false;
        }

        return 1 == TableCamp.Camp[Camp2];
    }

    public static bool IsOurChannel()
    {
        var debug = PlayerPrefs.GetInt(GameSetting.LoginAssistantKey, 0);
        return GameSetting.Channel.Equals("Uborm") || debug == 1;
    }

    //读条动作
    public static IEnumerator LaunchAction(int animationId, float time, Action endCallback, Action interruptCallback)
    {
        time = time/1000; // 表格中填的是毫秒
        var player = ObjManager.Instance.MyPlayer;

        player.PlayAnimation(animationId);

        float elapse = 0;
        while (elapse < time)
        {
            if (player.GetCurrentStateName() != OBJ.CHARACTER_STATE.IDLE)
            {
                interruptCallback();
                yield break;
            }
            elapse += Time.deltaTime;
            EventDispatcher.Instance.DispatchEvent(new UpdateMissionProgressEvent(elapse/time));

            yield return null;
        }
        player.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
        endCallback();

        yield return null;
    }

    //乘以比例
    public static int MultiplyPrecision(float value)
    {
        return (int) (value*PRECISION);
    }

    public static string NumEntoCh(int num)
    {
        var ret = "";
        if (num > 100000)
        {
            //数目太长了
            return GetDictionaryText(270213);
        }

        var isLastZero = false;
        var isStart = false;
        for (var i = 5 - 1; i >= 1; i--)
        {
            var n = (int) Mathf.Pow(10, i);
            var num1 = num/n;
            num = num - num1*n;
            if (num1 != 0)
            {
                if (!(isStart == false && num1 == 1 && i == 1))
                {
                    ret += OneNumEntoCh(num1);
                }

                ret += OneNumEntoCh(n);
                if (isStart == false)
                {
                    isStart = true;
                }
                isLastZero = false;
            }
            else
            {
                if (isStart)
                {
                    if (isLastZero == false)
                    {
                        ret += OneNumEntoCh(0);
                        isLastZero = true;
                    }
                }
            }
            if (i == 1)
            {
                if (num != 0)
                {
                    ret += OneNumEntoCh(num);
                }

                if (ret.Length > 1 && ret[ret.Length - 1] == '零')
                {
                    ret = ret.Substring(0, ret.Length - 1);
                }
            }
        }
        return ret;
    }

    public static void OnchangeBuildingPet(ReadonlyList<int> petItems, int maxPetNumber, List<PetSkillRecord> list)
    {
        OnchangeBuildingPet(petItems.Cast<int>().ToList(), maxPetNumber, list);
    }

    public static void OnchangeBuildingPet(List<int> petItems, int maxPetNumber, List<PetSkillRecord> list)
    {
        var nWorkingPetCount = 0;
        for (var i = 0; i < petItems.Count && i < maxPetNumber; i++)
        {
            if (-1 != petItems[i])
            {
                ++nWorkingPetCount;
            }
        }

        for (var idx = 0; idx < nWorkingPetCount; idx++)
        {
            var petId = petItems[idx];
            var petinfo = Table.GetPet(petId);
            if (petinfo == null)
            {
                continue;
            }
            int[] SkillLevelLimit =
            {
                petinfo.Speciality[0],
                petinfo.Speciality[1],
                petinfo.Speciality[2]
            };
            //特殊技能
            const int PetItemDataModelMaxSkill2 = PetItemDataModel.MaxSkill;

            var petdata = CityManager.Instance.GetPetById(petId);

            for (var i = 0; i < PetItemDataModelMaxSkill2; i++)
            {
                var skillIdx = PetItemExtDataIdx.SpecialSkill_Begin + i;
                if (skillIdx < 0 || idx >= petdata.Exdata.Count)
                {
                    continue;
                }
                var skillId = petdata.Exdata[skillIdx];

                if (-1 == skillId)
                {
                    continue;
                }

                if (petdata.Exdata[PetItemExtDataIdx.Level] < SkillLevelLimit[i] ||
                    petdata.Exdata[PetItemExtDataIdx.SpecialSkill_Begin + i] == -1)
                {
                    continue;
                }

                var tableSkill = Table.GetPetSkill(skillId);
                if (null != tableSkill)
                {
                    list.Add(tableSkill);
                }
            }
        }
    }

    private static string OneNumEntoCh(int num)
    {
        var ret = "";
        switch (num)
        {
            case 0:
            {
                //ret = "零";
                ret = GetDictionaryText(210300);
            }
                break;
            case 1:
            {
                //ret = "一";
                ret = GetDictionaryText(210301);
            }
                break;
            case 2:
            {
                //ret = "二";
                ret = GetDictionaryText(210302);
            }
                break;
            case 3:
            {
                //ret = "三";
                ret = GetDictionaryText(210303);
            }
                break;
            case 4:
            {
                //ret = "四";
                ret = GetDictionaryText(210304);
            }
                break;
            case 5:
            {
                //ret = "五";
                ret = GetDictionaryText(210305);
            }
                break;
            case 6:
            {
                //ret = "六";
                ret = GetDictionaryText(210306);
            }
                break;
            case 7:
            {
                //ret = "七";
                ret = GetDictionaryText(210307);
            }
                break;
            case 8:
            {
                //ret = "八";
                ret = GetDictionaryText(210308);
            }
                break;
            case 9:
            {
                //ret = "九";
                ret = GetDictionaryText(210309);
            }
                break;
            case 10:
            {
                //ret = "十";
                ret = GetDictionaryText(210310);
            }
                break;
            case 100:
            {
                //ret = "百";
                ret = GetDictionaryText(210311);
            }
                break;
            case 1000:
            {
                //ret = "千";
                ret = GetDictionaryText(210312);
            }
                break;
            case 10000:
            {
                //ret = "万";
                ret = GetDictionaryText(210313);
            }
                break;
            case 100000000:
            {
                //ret = "亿";
                ret = GetDictionaryText(210314);
            }
                break;
            case 1000000000:
            {
                //ret = "兆";
                ret = GetDictionaryText(210315);
            }
                break;
        }
        return ret;
    }

    public static void OnQuickRepair()
    {
        var needMoney = 0;
        {
            var __enumerator1 = (PlayerDataManager.Instance.EnumEquip()).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var quip = __enumerator1.Current;
                {
                    if (quip.ItemId < 0)
                    {
                        continue;
                    }
                    var tbEquip = Table.GetEquipBase(quip.ItemId);
                    if (tbEquip.DurableType == 0)
                    {
                        continue;
                    }
                    if (quip.Exdata.Count < 23)
                    {
                        continue;
                    }
                    var durable = quip.Exdata[22];
                    needMoney += (tbEquip.Durability - durable)*tbEquip.DurableMoney;
                }
            }
        }

        if (needMoney <= 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(423));
            return;
        }

        if (needMoney > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(426));
            return;
        }

        var vipLevel = PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel);
        var table = Table.GetVIP(vipLevel);
        if (table.Repair == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000014));
            return;
        }

        Action callback = () => { NetManager.Instance.StartCoroutine(RepairEquip()); };

        var str = string.Format(GetDictionaryText(424), needMoney);
        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "", callback);
    }

    public static void OnReceiveChatMsg(int chatType,
                                        ulong characterId,
                                        string characterName,
                                        ChatMessageContent Content,
                                        string channelName = "")
    {
        var chat = new ChatMessageDataModel
        {
            Type = chatType,
            Name = characterName,
            CharId = characterId,
            ChannelName = channelName,
            VipLevel = Content.Vip
        };

        if (Content.SoundData == null || Content.SoundData.Length == 0)
        {
            var content = Content.Content;
            //var content = Content.Content.CheckSensitive();
            chat.Content = content;
        }
        else
        {
            chat.Content = "";
            chat.SoundData = Content.SoundData;
            chat.Seconds = Content.Content.ToInt();
        }
        EventDispatcher.Instance.DispatchEvent(new Event_PushMessage(chat));
    }

    public static int CheckFuctionOnCondition(int Level,int iMissionId,int iMissionState)
    {
        int iCurrentMissionId = GetCurMainMissionId();

        if (iCurrentMissionId  != iMissionId)
        {
            int index = Table.GetMissionBase(iMissionId).FlagId;
            if (PlayerDataManager.Instance.FlagData.GetFlag(index) == 0)
            {
                return -1;
            }
                
        }
        else
        {
            if (iMissionState == (int)eMissionState.Unfinished)
            {
                if ((int)MissionManager.Instance.GetMissionState(iCurrentMissionId) == (int)eMissionState.Acceptable)
                {
                    return -1;
                }
            }
            else if (iMissionState == (int)eMissionState.Finished)
            {
                if ((int)MissionManager.Instance.GetMissionState(iCurrentMissionId) != (int)eMissionState.Finished)
                {
                    return -1;
                }
            }
        }           

        //判断等级
        if (Level > 0)
        {
            if (PlayerDataManager.Instance.GetLevel() < Level)
            {
                return -1;
            }
        }
        return 0;
    }

    public static int CheckFuctionOnConditionByMission(int MissionId)
    {
        int iCurrentMissionId = GetCurMainMissionId();
        
        if (iCurrentMissionId != MissionId)
        {
            int index = Table.GetMissionBase(MissionId).FlagId;
            if (PlayerDataManager.Instance.FlagData.GetFlag(index) == 0)
            {
                return -1;
            }

        }
        else
        {
            if (MissionId == (int)eMissionState.Unfinished)
            {
                if ((int)MissionManager.Instance.GetMissionState(iCurrentMissionId) == (int)eMissionState.Acceptable)
                {
                    return -1;
                }
            }
            else if (MissionId == (int)eMissionState.Finished)
            {
                if ((int)MissionManager.Instance.GetMissionState(iCurrentMissionId) != (int)eMissionState.Finished)
                {
                    return -1;
                }
            }
        }
        return 0;
    }

    public static int GetCurMainMissionId()
    {
        int iCurrentMissionId = -1;

        var missions = MissionManager.Instance.MissionData.Datas;
        foreach (var pair in missions)
        {
            var tableData = Table.GetMissionBase(pair.Value.MissionId);
            if (null == tableData)
            {
                continue;
            }

            var type = (eMissionMainType)tableData.ViewType;

            if (eMissionMainType.MainStoryLine != type)
            {
                continue;
            }
            iCurrentMissionId = pair.Value.MissionId;
            break;
        }
        return iCurrentMissionId;
    }

    public static int GetMainMissionOrderByFunctionId(int id)
    {        
        if (id > 0)
        {
            int MissionId = Table.GetFunctionOn(id).TaskID;
            return Table.GetMissionBase(MissionId).MissionBianHao;
        }
        return 0;
    }

    public static int CheckFuctionOnConditionByLevel(int Level)
    {
        //判断等级
        if (Level > 0)
        {
            if (PlayerDataManager.Instance.GetLevel() < Level)
            {
                return -1;
            }
        }
        return 0;
    }

public static int PkValueToColorId(int pkValue)
    {
        if (pkValue <= 0)
        {
            return -1;
        }
        var colorId = -1;
        Table.ForeachPKValue(recoard =>
        {
            colorId = recoard.NameColor;
            if (pkValue <= recoard.Id)
            {
                return false;
            }
            return true;
        });
        return colorId;
    }

    //游戏提示
    public static void PushMessageTip(int type, string text)
    {
        switch (type)
        {
            case 0:
            {
                //漂字提示
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(text));
            }
                break;
            case 1:
            {
                //确认框
                UIManager.Instance.ShowMessage(MessageBoxType.Ok, text);
            }
                break;
            case 2:
            {
                //聊天提示
                var e1 = new ChatMainHelpMeesage(text);
                EventDispatcher.Instance.DispatchEvent(e1);
            }
                break;
        }
    }

    private static int RandomAddCount(EquipBaseRecord tbEquip, int EquipRelateId)
    {
        if (EquipRelateId == -1)
        {
            return 0;
        }
        //EquipRelateRecord tbRelate = Table.GetEquipRelate(EquipRelateId);
        //if (tbRelate == null)
        //{
        //    Logger.Error("EquipRelate Id={0} not find", EquipRelateId);
        //    return 0;
        //}
        var AddCount = 0;
        var nRandom = MyRandom.Random(10000);
        var nTotleRandom = 0;
        //for (int i = 0; i != tbRelate.AttrCount.Length; ++i)
        //{
        //    nTotleRandom += tbRelate.AttrCount[i];
        //    if (nRandom < nTotleRandom)
        //    {
        //        if (i == 0) return 0;
        //        AddCount = i;
        //        break;
        //    }
        //}
        return AddCount;
    }

    private static IEnumerator RepairEquip()
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.RepairEquip(0);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(425));
                    EventDispatcher.Instance.DispatchEvent(new EquipDurableChange(0));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.VipLevelNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(200000014));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.MoneyNotEnough)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(426));
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    public static string ReplaceSensitiveString(string str)
    {
        Table.ForeachSensitiveWord(record =>
        {
            var start = str.IndexOf(record.Name, 0, StringComparison.OrdinalIgnoreCase);
            if (start != -1)
            {
                var newValue = "";
                var end = record.Name.Length;
                for (var i = 0; i < end; i++)
                {
                    newValue += "*";
                }
                str = str.Replace(record.Name, newValue);
            }
            return true;
        });
        return str;
    }

    //重置特效从effectController移到这里,为了给创建人物时候重置特效用
    public static void ResetEffect(GameObject o)
    {
        OptList<EffectEnableBehaviour>.List.Clear();
        o.GetComponentsInChildren(true, OptList<EffectEnableBehaviour>.List);
        {
            var __array2 = OptList<EffectEnableBehaviour>.List;
            var __arrayLength2 = __array2.Count;
            for (var __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var behaviour = __array2[__i2];
                {
                    behaviour.enabled = false;
                    behaviour.enabled = true;
                }
            }
        }

        OptList<EffectDisableBehaviour>.List.Clear();
        o.GetComponentsInChildren(true, OptList<EffectDisableBehaviour>.List);
        {
            var __array3 = OptList<EffectDisableBehaviour>.List;
            var __arrayLength3 = __array3.Count;
            for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var behaviour = __array3[__i3];
                {
                    behaviour.enabled = false;
                    behaviour.enabled = true;
                }
            }
        }
        OptList<Animation>.List.Clear();
        o.GetComponentsInChildren(true, OptList<Animation>.List);
        {
            var __array4 = OptList<Animation>.List;
            var __arrayLength4 = __array4.Count;
            for (var __i4 = 0; __i4 < __arrayLength4; ++__i4)
            {
                var anim = __array4[__i4];
                {
                    anim[anim.clip.name].time = 0;
                    anim.Sample();
                    anim.Play(PlayMode.StopAll);
                }
            }
        }
        OptList<Animator>.List.Clear();
        o.GetComponentsInChildren(true, OptList<Animator>.List);
        {
            var __array5 = OptList<Animator>.List;
            var __arrayLength5 = __array5.Count;
            for (var __i5 = 0; __i5 < __arrayLength5; ++__i5)
            {
                var anim = __array5[__i5];
                {
                    anim.Rebind();
                    anim.Play(anim.GetCurrentAnimatorStateInfo(0).nameHash, -1, 0);
                }
            }
        }
        OptList<UITweener>.List.Clear();
        o.GetComponentsInChildren(true, OptList<UITweener>.List);
        {
            var __array6 = OptList<UITweener>.List;
            var __arrayLength6 = __array6.Count;
            for (var __i6 = 0; __i6 < __arrayLength6; ++__i6)
            {
                var tw = __array6[__i6];
                {
                    tw.enabled = true;
                    tw.ResetToBeginning();
                }
            }
        }
        OptList<TrailRenderer_Base>.List.Clear();
        o.GetComponentsInChildren(true, OptList<TrailRenderer_Base>.List);
        {
            var __array7 = OptList<TrailRenderer_Base>.List;
            var __arrayLength7 = __array7.Count;
            for (var __i7 = 0; __i7 < __arrayLength7; ++__i7)
            {
                var t = __array7[__i7];
                {
                    t.ClearSystem(false);
                    t.Emit = true;
                }
            }
        }

        var effect = o.GetComponent<EffectController>();
        if (effect != null)
        {
            effect.ResetLoop();
        }

        OptList<UVColorAnimation>.List.Clear();
        o.GetComponentsInChildren(OptList<UVColorAnimation>.List);
        {
            var __array8 = OptList<UVColorAnimation>.List;
            var __arrayLength8 = __array8.Count;
            for (var __i8 = 0; __i8 < __arrayLength8; ++__i8)
            {
                var uvColorAnimation = __array8[__i8];
                {
                    uvColorAnimation.enabled = true;
                }
            }
        }

        OptList<ParticleSystem>.List.Clear();
        o.GetComponentsInChildren(OptList<ParticleSystem>.List);
        {
            var __array8 = OptList<ParticleSystem>.List;
            var __arrayLength8 = __array8.Count;
            for (var __i8 = 0; __i8 < __arrayLength8; ++__i8)
            {
                var particleSystem = __array8[__i8];
                {
                    particleSystem.Simulate(0, true, true);
                    particleSystem.Play();
                }
            }
        }

        OptList<Delay>.List.Clear();
        o.GetComponentsInChildren(OptList<Delay>.List);
        {
            var __array8 = OptList<Delay>.List;
            var __arrayLength8 = __array8.Count;
            for (var __i8 = 0; __i8 < __arrayLength8; ++__i8)
            {
                var delay = __array8[__i8];
                {
                    delay.StartDelay();
                }
            }
        }
    }

    //public static void Test()
    //{
    //    string[] att = new string[5]
    //    {
    //        "^11",
    //         "#11#11",
    //        "^408^11",
    //        "^471^ss|ssss",
    //        "^471^^11|ssss",
    //    };
    //    for (int i = 0; i < att.Length; i++)
    //    {
    //        string str = string.Empty;
    //        str = ServerStringAnalysis(att[i]);
    //        Logger.Error("Test-------:{0}", str);
    //    }


    //}
    //服务器发字典解析
    public static string ServerStringAnalysis(string str)
    {
        if (str.Length <= 0 || (str[0] != '^' && str[0] != '#'))
        {
            return str;
        }
        var varStr = str;
        var result = string.Empty;
        var dicId = -1;
        var obj = new List<object>();
        if (str[0] == '^')
        {
            varStr = varStr.Substring(1, varStr.Length - 1);
            var index2 = varStr.IndexOf('^');
            if (index2 < 0)
            {
                dicId = int.Parse(varStr);
                result += GetDictionaryText(dicId);
                return result;
            }
            dicId = int.Parse(varStr.Substring(0, index2));
            var varStr2 = varStr.Substring(index2 + 1, varStr.Length - index2 - 1);
            while (varStr2.Length > 0)
            {
                var i = varStr2.IndexOf('|');
                var id = -1;
                if (i < 0)
                {
                    if (varStr2[0] == '^')
                    {
                        id = int.Parse(varStr2.Substring(1, varStr2.Length - 1));
                        obj.Add(GetDictionaryText(id));
                    }
                    else
                    {
                        obj.Add(varStr2);
                    }
                    break;
                }

                var ss = varStr2.Substring(0, i);
                if (varStr2[0] == '^')
                {
                    id = int.Parse(ss.Substring(1, ss.Length - 1));
                    obj.Add(GetDictionaryText(id));
                }
                else
                {
                    obj.Add(ss);
                }

                varStr2 = varStr2.Substring(i + 1, varStr2.Length - i - 1);
            }

            result = string.Format(GetDictionaryText(dicId), obj.ToArray());
            return result;
        }
        if (str[0] == '#')
        {
            while (varStr.Length > 0)
            {
                var index2 = varStr.IndexOf('#');
                if (index2 < 0)
                {
                    return str;
                }
                varStr = varStr.Substring(1, varStr.Length - 1);
                index2 = varStr.IndexOf('#');
                if (index2 < 0)
                {
                    result += GetDictionaryText(int.Parse(varStr));
                    return result;
                }
                dicId = int.Parse(varStr.Substring(0, index2));
                result += GetDictionaryText(dicId);
                varStr = varStr.Substring(index2, varStr.Length - index2);
            }
        }
        return result;
    }

    public static void SetAttribute(ReadonlyObjectList<AttributeChangeDataModel> attributes,
                                    int index,
                                    int id,
                                    int value,
                                    int change = 0)
    {
        if (id == 5)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 6)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = attributes[fixIndex].Value;
                attributes[fixIndex].Value = value;

                attributes[fixIndex].ChangeEx = attributes[fixIndex].Change;
                attributes[fixIndex].Change = change;
                attributes[fixIndex].Type = 1001;
            }
        }
        else if (id == 6)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 5)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = value;
                attributes[fixIndex].ChangeEx = change;
                attributes[fixIndex].Type = 1001;
                return;
            }
        }
        else if (id == 7)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 8)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = attributes[fixIndex].Value;
                attributes[fixIndex].Value = value;

                attributes[fixIndex].ChangeEx = attributes[fixIndex].Change;
                attributes[fixIndex].Change = change;
                attributes[fixIndex].Type = 1002;
                return;
            }
        }
        else if (id == 8)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 7)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = value;
                attributes[fixIndex].ChangeEx = change;
                attributes[fixIndex].Type = 1002;
                return;
            }
        }

        attributes[index].Value = value;
        attributes[index].Change = change;
        attributes[index].Type = id;
        attributes[index].ValueEx = 0;
        attributes[index].ChangeEx = 0;
    }

    public static void SetAttributeBase(ReadonlyObjectList<AttributeBaseDataModel> attributes,
                                        int index,
                                        int id,
                                        int value)
    {
        if (id == 5)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 6)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = attributes[fixIndex].Value;
                attributes[fixIndex].Value = value;

                attributes[fixIndex].Type = 1001;
            }
        }
        else if (id == 6)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 5)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = value;
                attributes[fixIndex].Type = 1001;
                return;
            }
        }
        else if (id == 7)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 8)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = attributes[fixIndex].Value;
                attributes[fixIndex].Value = value;
                attributes[fixIndex].Type = 1002;
                return;
            }
        }
        else if (id == 8)
        {
            var fixIndex = -1;
            for (var i = 0; i < index; i++)
            {
                if (attributes[i].Type == 7)
                {
                    fixIndex = i;
                    break;
                }
            }
            if (fixIndex != -1)
            {
                attributes[index].Type = -1;
                attributes[fixIndex].ValueEx = value;
                attributes[fixIndex].Type = 1002;
                return;
            }
        }
        attributes[index].Type = id;
        attributes[index].Value = value;
    }

    public static void SetSpriteGrey(UISprite sprite, bool grey)
    {
        if (sprite == null)
        {
            return;
        }
        var atlas = sprite.atlas;
        if (atlas == null)
        {
            return;
        }
        var currentValue = atlas.name;
        var targetValue = "";
        var isGrey = currentValue.Contains("Grey");
        if (grey)
        {
            if (isGrey)
            {
                return;
            }
            targetValue = currentValue + "Grey";
        }
        else
        {
            if (!isGrey)
            {
                return;
            }
            targetValue = currentValue.Remove(currentValue.Length - 4, 4);
        }
        ResourceManager.PrepareResource<GameObject>("UI/Atlas/" + targetValue + ".prefab",
            res => { sprite.atlas = res.GetComponent<UIAtlas>(); }, true, true, true, false, true);
    }

    public static void SetSpriteIcon(UISprite sprite, int icoId)
    {
        if (sprite == null)
        {
            return;
        }
        var tbIco = Table.GetIcon(icoId);
        if (tbIco == null)
        {
            return;
        }
        SetSpriteIcon(sprite, tbIco.Atlas, tbIco.Sprite);
    }

    public static void SetSpriteIcon(UISprite sprite, string atlas, string name)
    {
        if (sprite.atlas != null)
        {
            var stringname = sprite.atlas.name;
            if (stringname.Contains(atlas))
            {
                sprite.spriteName = name;
                return;
            }
        }
        ResourceManager.PrepareResource<GameObject>("UI/Atlas/" + atlas + ".prefab",
            res =>
            {
                sprite.atlas = res.GetComponent<UIAtlas>();
                sprite.spriteName = name;
            });
    }

    public static void ShowHintTip(int dicId)
    {
        var info = GetDictionaryText(dicId);
        ShowHintTip(info);
    }

    public static void ShowHintTip(string content)
    {
        var e = new ShowUIHintBoard(content);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public static void ShowItemDataTip(BagItemDataModel data,
                                       eEquipBtnShow showType = eEquipBtnShow.None,
                                       int charLv = -1)
    {
        if (data == null)
        {
            return;
        }
        var itemId = data.ItemId;
        if (itemId == -1)
        {
            return;
        }
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null)
        {
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        var type = GetItemInfoType(tbItemBase.Type);
        switch (type)
        {
            case eItemInfoType.Item:
            {
                var itemData = new BagItemDataModel();
                itemData.ItemId = data.ItemId;
                itemData.Index = data.Index;
                itemData.Count = data.Count;
                itemData.BagId = data.BagId;
                itemData.Exdata.InstallData(data.Exdata);
                var arg = new ItemInfoArguments {DataModel = itemData, ShowType = (int) showType};
                var e = new Show_UI_Event(UIConfig.ItemInfoUI, arg);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case eItemInfoType.Equip:
            {
                var itemData = new BagItemDataModel();
                itemData.ItemId = data.ItemId;
                itemData.Index = data.Index;
                itemData.Count = data.Count;
                itemData.BagId = data.BagId;
                itemData.Exdata.InstallData(data.Exdata);
                var arg = new EquipCompareArguments
                {
                    Data = itemData,
                    ShowType = showType,
                    CharLevel = charLv,
                    ResourceType = 0
                };
                var e = new Show_UI_Event(UIConfig.EquipComPareUI, arg);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case eItemInfoType.Wing:
            {
                var itemData = new BagItemDataModel();
                itemData.ItemId = data.ItemId;
                itemData.Index = data.Index;
                itemData.Count = data.Count;
                itemData.BagId = data.BagId;
                itemData.Exdata.InstallData(data.Exdata);
                var arg = new WingInfogArguments {ItemData = itemData, CharLevel = charLv};
                var e = new Show_UI_Event(UIConfig.WingInfoUi, arg);
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case eItemInfoType.Elf:
            {
                var elfData = new ElfItemDataModel();
                elfData.ItemId = data.ItemId;
                var list = new List<int>();
                for (var i = 0; i < data.Exdata.Count; i++)
                {
                    list.Add(data.Exdata[i]);
                }
                elfData.Exdata.InstallData(list);
                var arg = new ElfInfoArguments {DataModel = elfData, ShowButton = false, CharLevel = charLv};
                var ee = new Show_UI_Event(UIConfig.ElfInfoUI, arg);
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                break;
            default:
                break;
        }
    }

    public static void ShowItemIdTip(int itemId, int type = -1)
    {
        if (itemId == -1)
        {
            return;
        }
        var tbItem = Table.GetItemBase(itemId);
        if (tbItem == null)
        {
            return;
        }
        var tbItemBase = Table.GetItemBase(itemId);
        var itemType = GetItemInfoType(tbItemBase.Type);
        switch (itemType)
        {
            case eItemInfoType.Item:
            {
                var ee = new Show_UI_Event(UIConfig.ItemInfoUI,
                    new ItemInfoArguments {ItemId = itemId, ShowType = (int) eEquipBtnShow.None});
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                break;
            case eItemInfoType.Equip:
            {
                var e = new Show_UI_Event(UIConfig.EquipInfoUI, new EquipInfoArguments {ItemId = itemId, Type = type});
                EventDispatcher.Instance.DispatchEvent(e);
            }
                break;
            case eItemInfoType.Wing:
            {
                var ee = new Show_UI_Event(UIConfig.ItemInfoUI,
                    new ItemInfoArguments {ItemId = itemId, ShowType = (int) eEquipBtnShow.None});
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                break;
            case eItemInfoType.Elf:
            {
                var ee = new Show_UI_Event(UIConfig.ElfInfoUI,
                    new ElfInfoArguments {ItemId = itemId, ShowButton = false});
                EventDispatcher.Instance.DispatchEvent(ee);
            }
                break;
            //case eItemInfoType.Chest:
            //{
            //    //if (UIManager.GetInstance().GetController(UIConfig.ChestInfoUI).State == FrameState.Open)
            //    //{
            //    //    var e = new PackItemClickEvent();
            //    //    e.BagId = itemData.BagId;
            //    //    e.Index = itemList.Index;
            //    //    e.TableId = itemData.ItemId;
            //    //    EventDispatcher.Instance.DispatchEvent(e);
            //    //}
            //    //else
            //    {
            //        var e = new UIEvent_ClickChest(itemData.ItemId);
            //        //e.Args.Tab = itemData.ItemId;
            //        e.From = "Bag";
            //        e.BagDataModel = itemData;
            //        EventDispatcher.Instance.DispatchEvent(e);
            //    }
            //}
            //    break;
            default:
                break;
        }
    }

    public static void ShowLoginTimeOutTip()
    {
        UIManager.Instance.ShowMessage(MessageBoxType.Ok, 220821, "", () =>
        {
            PlatformHelper.UserLogout();
            Game.Instance.ExitToLogin();
        }, null);
    }

    public static void ShowNetErrorHint(int errorCode)
    {
        var dicId = errorCode + 200000000;
        var info = GetDictionaryText(dicId);
        var e = new ShowUIHintBoard(info);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public static Color StringToColor(string str)
    {
        var color = MColor.white;
        if (string.IsNullOrEmpty(str) || (str.Length != 6 && str.Length != 8))
        {
            return MColor.white;
        }
        var flag = 0;
        while (str.Length > 0)
        {
            var unm = 0;
            var t = str.Substring(0, 2);

            for (var i = 0; i < 2; i++)
            {
                var s = t[i];
                if (s == 'a' || s == 'A')
                {
                    if (i == 1)
                    {
                        unm += 10;
                    }
                    else if (i == 0)
                    {
                        unm += 10*16;
                    }
                }
                else if (s == 'b' || s == 'B')
                {
                    if (i == 1)
                    {
                        unm += 11;
                    }
                    else if (i == 0)
                    {
                        unm += 11*16;
                    }
                }
                else if (s == 'c' || s == 'C')
                {
                    if (i == 1)
                    {
                        unm += 12;
                    }
                    else if (i == 0)
                    {
                        unm += 12*16;
                    }
                }
                else if (s == 'd' || s == 'D')
                {
                    if (i == 1)
                    {
                        unm += 13;
                    }
                    else if (i == 0)
                    {
                        unm += 13*16;
                    }
                }
                else if (s == 'e' || s == 'E')
                {
                    if (i == 1)
                    {
                        unm += 14;
                    }
                    else if (i == 0)
                    {
                        unm += 14*16;
                    }
                }
                else if (s == 'f' || s == 'F')
                {
                    if (i == 1)
                    {
                        unm += 15;
                    }
                    else if (i == 0)
                    {
                        unm += 15*16;
                    }
                }
                else
                {
                    var ns = 0;
                    if (Int32.TryParse(s.ToString(), out ns) == false)
                    {
                        return MColor.white;
                    }
                    if (i == 1)
                    {
                        unm += ns;
                    }
                    else if (i == 0)
                    {
                        unm += ns*16;
                    }
                }
            }

            if (flag == 0)
            {
                color.r = unm/255.0f;
            }
            else if (flag == 1)
            {
                color.g = unm/255.0f;
            }
            else if (flag == 2)
            {
                color.b = unm/255.0f;
            }
            else if (flag == 3)
            {
                color.a = unm/255.0f;
            }

            str = str.Remove(0, 2);
            flag++;
        }

        return color;
    }

    //生成时间字符串
    public static string TimeString(int h, int m, int s = 0)
    {
        if (h > 0)
        {
            return TimeStringHM(h, m);
        }
        return TimeStringMS(m, s);
    }

    //生成时间字符串
    public static string TimeString(TimeSpan t)
    {
        if (t.Hours > 0)
        {
            return TimeStringHM(t.Hours, t.Minutes);
        }
        return TimeStringMS(t.Minutes, t.Seconds);
    }

    //生成xx小时xx分钟格式
    public static string TimeStringHM(int h, int m)
    {
        var str = "";
        if (h > 0)
        {
            str += h + GetDictionaryText(1040);
            if (m > 0)
            {
                str += m + GetDictionaryText(1041);
            }
        }
        else
        {
            if (m > 0)
            {
                str += m + GetDictionaryText(1041);
            }
            else
            {
                str += "<1" + GetDictionaryText(1041);
            }
        }

        return str;
    }

    //生成xx分钟xx秒格式
    public static string TimeStringMS(int m, int s)
    {
        var str = m + GetDictionaryText(1041);
        if (s > 0)
        {
            str += s + GetDictionaryText(1045);
        }
        return str;
    }

    //称号title属性赋值
    public static void TitleAddAttr(TitleItemDataModel item, NameTitleRecord record)
    {
        var list = new List<AttributeStringDataModel>();
        var propStr1 = string.Empty;
        var propStr2 = string.Empty;
        var propCount = record.PropValue.Length;
        for (var i = 0; i < propCount; i++)
        {
            var propId = record.PropId[i];
            if (propId == -1)
            {
                continue;
            }
            var value = record.PropValue[i];
            if (propId == 5)
            {
                propStr1 = GetDictionaryText(222001) + ":" + value + "-";
            }
            else if (propId == 6)
            {
                propStr1 += value;
            }
            else if (propId == 7)
            {
                propStr2 = GetDictionaryText(222002) + ":" + value + "-";
            }
            else if (propId == 8)
            {
                propStr2 += value;
            }
            else
            {
                var attr = new AttributeStringDataModel();
                var str = ExpressionHelper.AttrName[propId] + ":";
                str += AttributeValue(propId, value);
                attr.LabelString = str;
                list.Add(attr);
            }
        }
        if (propStr1 != string.Empty)
        {
            var attr = new AttributeStringDataModel();
            attr.LabelString = propStr1;
            list.Insert(0, attr);
        }
        if (propStr2 != string.Empty)
        {
            var attr = new AttributeStringDataModel();
            attr.LabelString = propStr2;
            list.Insert(0, attr);
        }
        item.Attributes = new ObservableCollection<AttributeStringDataModel>(list);
    }

    public static string UnicodeToString(string str)
    {
        var outStr = "";
        str = str.Replace("//", "");
        str = str.Replace("\\", "");
        if (!string.IsNullOrEmpty(str))
        {
            var strlist = str.Split('u');
            try
            {
                for (var i = 1; i < strlist.Length; i++)
                {
                    //将unicode字符转为10进制整数，然后转为char中文字符  
                    outStr += (char) int.Parse(strlist[i], NumberStyles.HexNumber);
                }
            }
            catch (FormatException ex)
            {
                outStr = ex.Message;
            }
        }
        return outStr;
    }

    public static String[] SplitString(String str,char ch)
    {
        var args = str.Split(ch);
        return args;
    }

    public static void UseItem(BagItemDataModel bagItem)
    {
        var dataModel = new ItemInfoDataModel();
        dataModel.ItemData = bagItem;
        dataModel.UseCount = 1;
        if (bagItem == null)
        {
            Logger.Error(" UseItem error , bagItem=null");
            return;
        }
        var tbItem = Table.GetItemBase(bagItem.ItemId);
        if (tbItem == null)
        {
            Logger.Error(" UseItem error, bagItem.ItemId={0}", bagItem.ItemId);
            return;
        }
        if (tbItem.Type == 21000)
        {
//技能书
            InitSkillBook(dataModel);
        }

        UseItem(dataModel);
    }

    public static bool IsQuickBuyGift(int itemId)
    {
        var item = Table.GetItemBase(itemId);
        if (item == null)
        {
            return false;
        }

        if (item.Type == 23500)
            return true;

        return false;
    }

    public static bool CheckEnoughItems(int itemId, int itemCount, bool showItemGet = true)
    {
        var items = new Dictionary<int, int>();
        items[itemId] = itemCount;
        return CheckEnoughItems(items, showItemGet);
    }

    public static bool CheckEnoughItems(Dictionary<int, int> items, bool showItemGet = true)
    {
        var lackItems = new Dictionary<int, int>();
        var enumerator = items.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var itemId = enumerator.Current.Key;
            var itemCount = enumerator.Current.Value;
            var buyCount = PlayerDataManager.Instance.GetNeedBuyCount(itemId, itemCount);
            if (buyCount < 0)
            { // 不能买
                if (showItemGet)
                {
                    PlayerDataManager.Instance.ShowItemInfoGet(itemId);
                }
                lackItems.Clear();
                return false;
            }
            else if (buyCount > 0)
            { // 可以买
                lackItems[itemId] = buyCount;
            }
            // else 身上够了
        }

        if (lackItems.Count > 0)
        {
            var e = new QuickBuyArguments()
            {
                Items = lackItems
            };
            EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.QuickBuyUi, e));
            return false;
        }

        return true;
    }

    public static void UseItem(ItemInfoDataModel dataModel)
    {
        var bagItem = dataModel.ItemData;
        var itemID = bagItem.ItemId;
        var tbItemBase = Table.GetItemBase(itemID);
        var playerData = PlayerDataManager.Instance;
        if (tbItemBase.UseLevel > PlayerDataManager.Instance.GetLevel())
        {
            var dicText = GetDictionaryText(220307);
            var str = string.Format(dicText, tbItemBase.UseLevel);
            var e = new ShowUIHintBoard(str);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }

        var exData = tbItemBase.Exdata;
        Action afterSuccess = null;
        switch (tbItemBase.Type)
        {
            case 21000: //技能书
            {
                UseSkillBook(dataModel);
                return;
            }
                break;
            case 22000: //材料
            {
                var player = PlayerDataManager.Instance;
                var conditionId = exData[0];
                var errDic = player.CheckCondition(conditionId);
                if (errDic != 0)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(errDic));
                    return;
                }

                GotoUiTab(exData[1], exData[2]);
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
                return;
            }
                break;
            case 23000: //固定礼包
            {
            }
            break;
            //case 23900: //升級丹
            //{
            //}
            //break;
            case 23500: //随机礼包
            {
                    // if(dataModel.UseCount == dataModel.cou)
                    EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ChestInfoUI));
            }
                break;
            case 24000: //红蓝药类
            {
                if (exData[2] == 0) //HP
                {
                    if (playerData.PlayerDataModel.Attributes.HpPercent > 0.999f)
                    {
                        var e = new ShowUIHintBoard(210201);
                        EventDispatcher.Instance.DispatchEvent(e);
                        return;
                    }
                    afterSuccess = () =>
                    {
                        var skill = playerData.PlayerDataModel.Bags.ItemWithSkillList[1];
                        skill.LastTime = skill.MaxTime;
                    };
                }
                else if (exData[2] == 1)
                {
                    if (playerData.PlayerDataModel.Attributes.MpPercent > 0.999f)
                    {
                        var e = new ShowUIHintBoard(210202);
                        EventDispatcher.Instance.DispatchEvent(e);
                        return;
                    }
                    afterSuccess = () =>
                    {
                        var skill = playerData.PlayerDataModel.Bags.ItemWithSkillList[2];
                        skill.LastTime = skill.MaxTime;
                    };
                }
            }
                break;
            case 24500: //加BUFF药
            {
                //判断逻辑，高级覆盖低级时给提示，低级不能覆盖低级
                var buffTypeId = tbItemBase.Exdata[0];
                if (-1 == buffTypeId)
                {
                    break;
                }
                if (null == ObjManager.Instance.MyPlayer)
                {
                    break;
                }

                //如果有这种buff肯定可以用,时间会增长
                if (ObjManager.Instance.MyPlayer.GetBuffManager().HasBuff(buffTypeId))
                {
                    break;
                }

                var tabBuff = Table.GetBuff(buffTypeId);
                if (null == tabBuff)
                {
                    Logger.Debug("null!=tabBuff[0]", buffTypeId);
                    break;
                }

                var buffList = ObjManager.Instance.MyPlayer.GetBuffManager().GetBuffData();
                {
                    // foreach(var buff in buffList)
                    var __enumerator1 = (buffList).GetEnumerator();
                    while (__enumerator1.MoveNext())
                    {
                        var buff = __enumerator1.Current;
                        {
                            var tbBuffTmpe = Table.GetBuff(buff.BuffTypeId);
                            if (null == tbBuffTmpe)
                            {
                                continue;
                            }

                            if (tabBuff.HuchiId != tbBuffTmpe.HuchiId || tabBuff.TihuanId != tbBuffTmpe.TihuanId)
                            {
                                continue;
                            }

                            if (tabBuff.PriorityId < tbBuffTmpe.PriorityId)
                            {
//如果当前要获得buff 优先级低于已有的buff优先级，那就不能再吃了
                                var e = new ShowUIHintBoard(727);
                                EventDispatcher.Instance.DispatchEvent(e);
                                return;
                            }
                            if (tabBuff.PriorityId > tbBuffTmpe.PriorityId)
                            {
//如果当前要获得buff优先级高于已有的buff优先级，那就弹出确认

                                var count = dataModel.UseCount;
                                var str = string.Format(GetDictionaryText(726), tbItemBase.Name, tbBuffTmpe.Name);
                                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
                                    () => { UseItemWrap(dataModel, -1, count); });
                                return;
                            }
                        }
                    }
                }
            }
                break;
            case 21900: //喇叭
            {
                var e = new Show_UI_Event(UIConfig.ChatMainFrame);
                EventDispatcher.Instance.DispatchEvent(e);

                var e1 = new ChatTrumpetVisibleChange(true);
                EventDispatcher.Instance.DispatchEvent(e1);

                var e2 = new Close_UI_Event(UIConfig.ItemInfoUI);
                EventDispatcher.Instance.DispatchEvent(e2);
                return;
            }
                break;
            case 25000: //永久加成道具
            {
                var tbLv = Table.GetLevelData(PlayerDataManager.Instance.GetLevel());
                if (tbLv == null)
                {
                    return;
                }
                var exId = exData[0];
                var exAdd = exData[1]*dataModel.UseCount;
                var oldValue = PlayerDataManager.Instance.GetExData(exId);
                var maxValue = tbLv.FruitLimit[exData[2]];
                if (oldValue + exAdd > maxValue)
                {
                    var e = new ShowUIHintBoard(534);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }
            }
                break;
            case 26500: //随从经验药
            {
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
                //GotoUiTab 51 = PetFrame, 1 = tab index
                GotoUiTab(51, 1);
                return;
            }
                break;
				/*
            case 26000: //随从蛋
            case 26100: //随机随从蛋
            {
                var list = CityManager.Instance.BuildingDataList;
                BuildingData buildingData = null;
                {
                    // foreach(var data in list)
                    var __enumerator2 = (list).GetEnumerator();
                    while (__enumerator2.MoveNext())
                    {
                        var data = __enumerator2.Current;
                        {
                            var tabel = Table.GetBuilding(data.TypeId);
                            if (tabel.Type == 5)
                            {
                                buildingData = data;
                                break;
                            }
                        }
                    }
                }


                if (buildingData == null)
                {
                    var e = new ShowUIHintBoard(300006);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }

                var tbBuild = Table.GetBuilding(buildingData.TypeId);
                var tbBuildSerive = Table.GetBuildingService(tbBuild.ServiceId);


                var hasEmpty = false;
                var tbBuildSeriveParam20 = tbBuildSerive.Param[2];
                for (var i = 0; i < tbBuildSeriveParam20; i++)
                {
                    if (buildingData.Exdata.Count > i)
                    {
                        var id = buildingData.Exdata[i];
                        if (id == -1)
                        {
                            hasEmpty = true;
                        }
                    }
                }

                if (hasEmpty == false)
                {
                    var e = new ShowUIHintBoard(300007);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                }

                var ee = new Show_UI_Event(UIConfig.HatchingHouse,
                    new UIHatchingHouseArguments {BuildingData = buildingData, ItemId = itemID});
                EventDispatcher.Instance.DispatchEvent(ee);

                var e1 = new Close_UI_Event(UIConfig.ItemInfoUI);
                EventDispatcher.Instance.DispatchEvent(e1);
                return;
            }
                break;
				*/
				/*
            case 26200: //复训卷轴
            {
                var conditionId = Table.GetClientConfig(211).ToInt();
                var result = playerData.CheckCondition(conditionId);
                if (result != 0)
                {
                    ShowHintTip(912);
                    return;
                }
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));

                var arg = new UIPetFrameArguments();
                arg.Tab = 2;
                arg.PetIdx = 0;
                var ee = new Show_UI_Event(UIConfig.PetFrameUI, arg);
                EventDispatcher.Instance.DispatchEvent(ee);
                return;
            }
                break;
				*/
            case 26300: //藏宝图
            {
                var sceneId = bagItem.Exdata[0];
                var x = bagItem.Exdata[1];
                var y = bagItem.Exdata[2];

                GameControl.Executer.Stop();
                ObjManager.Instance.MyPlayer.LeaveAutoCombat();

                if (GameLogic.Instance.Scene.SceneTypeId != sceneId)
                {
//如果不在当前场景，就飞过去
                    var flyToSceneCommand = GameControl.FlyToSceneCommand(sceneId);
                    GameControl.Executer.PushCommand(flyToSceneCommand);
                }

                var nowPos = ObjManager.Instance.MyPlayer.Position;
                if (Math.Abs(x - nowPos.x) > 2 || Math.Abs(y - nowPos.z) > 2)
                {
//判断距离，如果还远，就寻路
                    var gotoCommand = GameControl.GoToCommand(sceneId, x, y, 2.0f);
                    GameControl.Executer.PushCommand(gotoCommand);
                }

                //使用藏宝图，挖取物品
                var funcCommand = new FuncCommand(() =>
                {
                    if (OBJ.CHARACTER_STATE.IDLE != ObjManager.Instance.MyPlayer.GetCurrentStateName())
                    {
                        return;
                    }

                    var dicId = 728;
                    var e = new ShowMissionProgressEvent(GetDictionaryText(dicId));
                    EventDispatcher.Instance.DispatchEvent(e);
                    Logger.Info("FuncCommand CallBackFun.......................1");

                    var endCallback = new Action(() =>
                    {
                        ObjManager.Instance.MyPlayer.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
                        EventDispatcher.Instance.DispatchEvent(new HideMissionProgressEvent(0));

                        #region 找到之前用的物品，避免整理包裹导致的问题

                        var items = playerData.PlayerDataModel.Bags.Bags[bagItem.BagId].Items;
                        var itemData =
                            items.FirstOrDefault(
                                i =>
                                    i.ItemId == itemID && i.Exdata[0] == sceneId && i.Exdata[1] == x &&
                                    i.Exdata[2] == y);
                        if (itemData == null)
                        {
                            Logger.Error("Use CangBaoTu failed! find itemData == null");
                            return;
                        }

                        //不能直接用背包的索引，不然会修改背包数据
                        var newData = new BagItemDataModel();
                        newData.ItemId = itemData.ItemId;
                        newData.Index = itemData.Index;
                        newData.Count = itemData.Count;
                        newData.BagId = itemData.BagId;
                        newData.Exdata.InstallData(itemData.Exdata);
                        dataModel.ItemData = newData;

                        #endregion

                        UseItemWrap(dataModel, -1, dataModel.UseCount);
                    });

                    var interruptCallback =
                        new Action(() => { EventDispatcher.Instance.DispatchEvent(new HideMissionProgressEvent(0)); });

                    var time = Table.GetClientConfig(255).ToInt()/1000f;
                    GameLogic.Instance.StopCoroutine("LaunchAction");
                    GameLogic.Instance.StartCoroutine(LaunchAction(11, time, endCallback, interruptCallback));
                });
                GameControl.Executer.PushCommand(funcCommand);
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.CharacterUI));
            }
                return;

            case 30000: //图鉴碎片
            case 35000: //怪物图鉴
            {
                var player = PlayerDataManager.Instance;
                var conditionId = exData[2];
                var errDic = player.CheckCondition(conditionId);
                if (errDic != 0)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(errDic));
                    return;
                }

                GotoUiTab(exData[3], -1);
                EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
                //背包点击使用图鉴和碎片不发包,直接跳转界面
                return;
            }
                break;
            case 27000: //装备直接强化
            {
                var bagId = PlayerDataManager.Instance.ChangeBagIdToEquipType(tbItemBase.Exdata[1]);
                var item = PlayerDataManager.Instance.GetEquipData((eEquipType) bagId);
                if (item == null)
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(224603));
                    return;
                }
                //if (bags.Items.Count == 0)
                //{
                //    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(224603));
                //    return;
                //}
                if (item.Exdata.Enchance >= tbItemBase.Exdata[0])
                {
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(224604));
                    return;
                }

                var str2 = string.Format(GetDictionaryText(224605),
                    GetDictionaryText(tbItemBase.Exdata[2]));
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str2, "",
                    () => { UseItemWrap(dataModel, -1, dataModel.UseCount, afterSuccess); });
                return;
                break;
            }
        }

        var needItemId = tbItemBase.DependItemId;
        var needItemCount = tbItemBase.DependItemNum * dataModel.UseCount;
        Action callBack = () =>
        {
            var itemRecord = Table.GetItemBase(needItemId);
            if (itemRecord != null)
            {
                var haveItemCount = PlayerDataManager.Instance.GetItemTotalCount(needItemId).Count;
                if (haveItemCount < needItemCount)
                {
                    var str = GetDictionaryText(701);
                    str = string.Format(str, itemRecord.Name);
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
                }
                else
                {
                    UseItemWrap(dataModel, -1, dataModel.UseCount, afterSuccess);
                }
            }        
        };

        if (needItemId >= 0)
        { // 消耗物品检查
            var itemRecord = Table.GetItemBase(needItemId);
            if (itemRecord != null)
            {
                var haveCount = PlayerDataManager.Instance.GetItemTotalCount(needItemId).Count;
                if (haveCount < needItemCount)
                {
                    var str = GetDictionaryText(200014);
                    str = string.Format(str, needItemCount, GetTableColorString(itemRecord.Quality), itemRecord.Name, haveCount);
                    EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(str));
                    PlayerDataManager.Instance.ShowItemInfoGet(needItemId);
                }
                else
                {
                    var content = GetDictionaryText(200013);
                    content = string.Format(content, needItemCount, GetTableColorString(itemRecord.Quality), itemRecord.Name);
                    UIManager.Instance.ShowMessage(MessageBoxType.OkCancel,
                        content,
                        "",
                        callBack
                    );                         
                }
            }
        }
        else
        {
            UseItemWrap(dataModel, -1, dataModel.UseCount, afterSuccess);
        }  
    }

    private static IEnumerator UseItemCorotion(ItemInfoDataModel infoData, int type, int count, Action afterSuccess)
    {
        var bagItem = infoData.ItemData;
        if (count == 0)
        {
            yield break;
        }
        using (var blockingLayer = new BlockingLayerHelper(0))
        {
            //var msg = NetManager.Instance.UseItem(item.BagId,item.Index, DataModel.UseCount);
            var itemId = bagItem.ItemId;
            var msg = NetManager.Instance.UseItem(bagItem.BagId, bagItem.Index, count);
            yield return msg.SendAndWaitUntilDone();

            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    //使用成功
                    if (null != afterSuccess)
                    {
                        afterSuccess();
                    }

                    if (msg.Response.Data.Count > 0) // 展示
                    {
                        var e = new ShowItemsArguments
                        {
                            Items = msg.Response.Data
                        };

                        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ShowItemsFrame, e));
                    }

                    var e2 = new ShowUIHintBoard(270109);
                    EventDispatcher.Instance.DispatchEvent(e2);

                    var tbItem = Table.GetItemBase(itemId);
                    switch (tbItem.Type)
                    {
                        case 25000:
                        {
                            var tbLv = Table.GetLevelData(PlayerDataManager.Instance.GetLevel());
                            if (tbLv == null)
                            {
                                break;
                            }
                            var exId = tbItem.Exdata[0];
                            var exAdd = tbItem.Exdata[1]*count;
                            var oldValue = PlayerDataManager.Instance.GetExData(exId);
                            var maxValue = tbLv.FruitLimit[tbItem.Exdata[2]];
                            var newValue = oldValue + exAdd;
                            if (newValue > maxValue)
                            {
                                newValue = maxValue;
                            }
                            PlayerDataManager.Instance.SetExData(exId, newValue);
                            PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.AddPoint);
                        }
                            break;
                        case 21000:
                        {
                            var tbSkill = Table.GetSkill(tbItem.Exdata[0]);
                            if (type == 0)
                            {
                                //学会技能{0}
                                var str = string.Format(GetDictionaryText(270107), tbSkill.Name);
                                var e1 = new ShowUIHintBoard(str);
                                EventDispatcher.Instance.DispatchEvent(e1);
                                //引导硬编码
                                if ((itemId == 20001 && 0 == PlayerDataManager.Instance.GetRoleId()) ||
                                    (itemId == 20101 && 1 == PlayerDataManager.Instance.GetRoleId()) ||
                                    (itemId == 20201 && 2 == PlayerDataManager.Instance.GetRoleId()))
                                {
                                    //学会时需要初始化技能数据
                                    PlayerDataManager.Instance.LearnSkill(tbSkill.Id, false);

                                    var equipskills = PlayerDataManager.Instance.PlayerDataModel.SkillData.EquipSkills;
                                    var c = equipskills.Count;
                                    var slot = -1;
                                    for (var j = 0; j < c; j++)
                                    {
                                        var equip = equipskills[j];
                                        if (equip.SkillId < 0)
                                        {
                                            slot = j;
                                            break;
                                        }
                                    }
                                    if (-1 != slot)
                                    {
                                        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.CharacterUI));
                                        var newSkillId = -1;
                                        if (0 == PlayerDataManager.Instance.GetRoleId())
                                        {
                                            newSkillId = 5;
                                        }
                                        else if (1 == PlayerDataManager.Instance.GetRoleId())
                                        {
                                            newSkillId = 105;
                                        }
                                        else if (2 == PlayerDataManager.Instance.GetRoleId())
                                        {
                                            newSkillId = 205;
                                        }
                                        var evn = new SkillEquipMainUiAnime(newSkillId, slot);
                                        EventDispatcher.Instance.DispatchEvent(evn);
                                    }
                                }
                                else
                                {
                                    PlayerDataManager.Instance.LearnSkill(tbSkill.Id);
                                }
                            }
                            else
                            {
                                PlayerDataManager.Instance.SkillTalentPointChange(tbSkill.Id, 1);
                                var str = string.Format(GetDictionaryText(270108), tbSkill.Name);
                                var e1 = new ShowUIHintBoard(str);
                                EventDispatcher.Instance.DispatchEvent(e1);
                                InitSkillBook(infoData);
                                PlatformHelper.UMEvent("skill", "GetTalent");
                            }
                        }
                            break;
                        default:
                        {
                        }
                            break;
                    }
                    if (bagItem.Count == count || bagItem.Count == 0)
                    {
//完全用完数量才关闭窗口
                        bagItem.Count = 0;
                        var e = new Close_UI_Event(UIConfig.ItemInfoUI);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        bagItem.Count -= count;
                    }
                    var item = PlayerDataManager.Instance.GetItem(bagItem.BagId, bagItem.Index);
                    EventDispatcher.Instance.DispatchEvent(new UseItemEvent(item));
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_HpMax)
                {
                    var e = new ShowUIHintBoard(210201);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_MpMax)
                {
                    var e = new ShowUIHintBoard(210202);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_SkillNoCD)
                {
//物品还在冷却中
                    var e = new ShowUIHintBoard(481);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else if (msg.ErrorCode == (int) ErrorCodes.Error_Item_Use_Fruit_Limit)
                {
                    var e = new ShowUIHintBoard(200000000 + msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    var e = new ShowUIHintBoard(200000000 + msg.ErrorCode);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
            }
            else
            {
                Logger.Info(string.Format("SellItemCorotion....State = {0} ErroeCode = {1}", msg.State, msg.ErrorCode));
            }
        }
    }

    private static void UseItemWrap(ItemInfoDataModel itemInfo, int type, int count, Action afterSuccess = null)
    {
        NetManager.Instance.StartCoroutine(UseItemCorotion(itemInfo, type, count, afterSuccess));
    }

    private static void UseSkillBook(ItemInfoDataModel dataModel)
    {
        var showType = 0; //0 Skill 1 Talent
        var tbItemBase = Table.GetItemBase(dataModel.ItemData.ItemId);

        if (tbItemBase == null)
        {
            return;
        }

        if (tbItemBase.OccupationLimit != -1)
        {
            var roleType = PlayerDataManager.Instance.GetRoleId();
            if (roleType != tbItemBase.OccupationLimit)
            {
                //职业不符！
                var e = new ShowUIHintBoard(270104);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }


        var playerData = PlayerDataManager.Instance.PlayerDataModel;
        if (tbItemBase.UseLevel > playerData.Bags.Resources.Level)
        {
            var e = new ShowUIHintBoard(1504);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var attr = PlayerDataManager.Instance.GetAttribute(dataModel.AttributeType);
        if (attr < dataModel.AttributeValue)
        {
            //属性点不足！
            var e = new ShowUIHintBoard(270105);
            EventDispatcher.Instance.DispatchEvent(e);
            return;
        }
        var skillId = tbItemBase.Exdata[0];
        var tbSkill = Table.GetSkill(skillId);
        var skillLv = 0;
        var skillData = PlayerDataManager.Instance.PlayerDataModel.SkillData;
        {
            // foreach(var data in skillData.OtherSkills)
            var __enumerator3 = (skillData.OtherSkills).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var data = __enumerator3.Current;
                {
                    if (data.SkillId == skillId)
                    {
                        skillLv = data.SkillLv;
                        break;
                    }
                }
            }
        }

        if (skillLv == 0)
        {
            showType = 0;
            var str = GetDictionaryText(711);
            str = string.Format(str, tbItemBase.Name);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
                () => { UseItemWrap(dataModel, showType, dataModel.UseCount); });
        }
        else
        {
            if (PlayerDataManager.Instance.IsSkillTalentMax(skillId))
            {
                //当前技能天赋点数已达上限！
                var e = new ShowUIHintBoard(270106);
                EventDispatcher.Instance.DispatchEvent(e);
            }
            else
            {
                showType = 1;
                var str = GetDictionaryText(721);
                str = string.Format(str, tbItemBase.Name, tbSkill.Name);
                UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "",
                    () => { UseItemWrap(dataModel, showType, dataModel.UseCount); });
            }
        }
    }

    public static float Vector3PlaneDistance(Vector3 v1, Vector3 v2)
    {
        var v11 = new Vector2(v1.x, v1.z);
        var v22 = new Vector2(v2.x, v2.z);
        return Vector2.Distance(v11, v22);
    }

    public static string ViewBigValueStr(int value)
    {
        if (value >= 100000000)
        {
            return string.Format(GetDictionaryText(1505), value/1000000/100.0f);
        }
        if (value > 1000000)
        {
            return string.Format(GetDictionaryText(1507), value/10000);
        }
        return value.ToString();
    }
}

public class GainItemHintEntryArgs
{
    public int Count;
    public int FightValueOld;
    public BagItemDataModel ItemData;
    public int OldEquipIdx;
}


public class CaculatePetMissionRewardArgs
{
    public int itemCount;
    public int itemId;
}

public class CreateCellDataArgs
{
    public int maxCount;
    public int nowCount;
    public ActivityCellState state;
    public int textId;
}

//缓存的数据结构
public class CacheEntry
{
    public int BagIdx;
    public int ItemId;
}

//邮件比对处理类
public class MailCellDataComplarer : IEqualityComparer<MailCellData>
{
    public bool Equals(MailCellData x, MailCellData y)
    {
        return x.Id == y.Id;
    }

    public int GetHashCode(MailCellData obj)
    {
        return obj.GetHashCode();
    }
}

//ElfItemDataModel 排序
public class ElfItemDataModelComparer : IComparer<ElfItemDataModel>
{
    public int Compare(ElfItemDataModel x, ElfItemDataModel y)
    {
        if (x.FightPoint < y.FightPoint)
        {
            return 1;
        }
        if (x.FightPoint > y.FightPoint)
        {
            return -1;
        }
        return 0;
    }
}

public class EuqipInfo
{
    public EquipBaseRecord Record;
    public bool CanBuy;
    public int BuyCost;
}


public enum LandState
{
    Lock = -1, //锁定
    Blank = 0, //未种植
    Growing = 1, //成长中
    Mature = 2 //成熟
}

public enum MenuState
{
    Invalid = -1, //不可用
    Seed = 0, //种子菜单
    Growing = 1, //生长菜单
    Mature = 2 //收获菜单
}

public enum OperateType
{
    Seed = 0, //种植
    Mature = 1, //收获
    Speedup = 2, //快速生长
    Wipeout = 3, //铲除
    Grow = 10 //????
}

//PlayFrame的Tab页
public enum PlayFrameTab
{
    DailyActivity, //日常活动
    TimedActivity, //限时活动
    AboutToOpen, //即将开启
    Count
}

//PlayFrameCell 的State
public enum ActivityCellState
{
    Start, //开始了
    WillStart, //即将开始
    Complete, //完成了
    CanNotAttend //不可再参加了
}

//PVPRule 的 NameColorRule
public enum NameColorRule
{
    None = -1, //无选择
    PkValue, //按杀气显示
    Camp, //按阵营显示
    FightingEachOther //各自为战（自己是绿色，别人都是红色，并且别人都只显示职业名，不显示真实名字）
}