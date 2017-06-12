#region using

using System.Collections.Generic;
using System.Collections.ObjectModel;
using ClientDataModel;
using DataTable;

#endregion

public static class SkillDataModelExtension
{
    public static bool ttt = true;

    public static void AddTalentAttrToPanel(ObservableCollection<TalentAttr> attrPanel, TalentCellDataModel talent)
    {
        if (talent.Count <= 0)
        {
            return;
        }

        var TalentTable = Table.GetTalent(talent.TalentId);
        if (TalentTable.AttrId != -1)
        {
            var talentAttr = new TalentAttr();
            talentAttr.AttrId = TalentTable.AttrId;
            talentAttr.AttrName = ExpressionHelper.AttrName[TalentTable.AttrId];
            talentAttr.AttrValue = StringConvert.Level_Value_Ref(10000000 + TalentTable.SkillupgradingId, talent.Count);
            var bExist = false;
            {
                // foreach(var attr in attrPanel)
                var __enumerator3 = (attrPanel).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var attr = __enumerator3.Current;
                    {
                        if (attr.AttrName == talentAttr.AttrName)
                        {
                            attr.AttrValue = attr.AttrValue + talentAttr.AttrValue;
                            attr.AttrStringValue = GameUtils.AttributeValue(TalentTable.AttrId, attr.AttrValue);
                            bExist = true;
                        }
                    }
                }
            }
            if (!bExist)
            {
                talentAttr.AttrStringValue = GameUtils.AttributeValue(TalentTable.AttrId, talentAttr.AttrValue);
                attrPanel.Add(talentAttr);
            }
            PlayerDataManager.Instance.PlayerDataModel.SkillData.ShowAttrPanelButton = true;
        }
    }

    public static void Clone(this SkillItemDataModel dataModel, SkillItemDataModel otherModel)
    {
        dataModel.SkillId = otherModel.SkillId;
        dataModel.SkillLv = otherModel.SkillLv;
        dataModel.CoolDownTime = otherModel.CoolDownTime;
        dataModel.ChargeLayer = otherModel.ChargeLayer;
        dataModel.ChargeLayerTotal = otherModel.ChargeLayerTotal;
        dataModel.MaxTargetCount = otherModel.MaxTargetCount;
        dataModel.ControlType = otherModel.ControlType;
        dataModel.RefreshCast();
    }

    public static void InitializeTalentCell(this TalentCellDataModel dataModel)
    {
        var tableTalent = Table.GetTalent(dataModel.TalentId);
        if (null == tableTalent)
        {
            Logger.Error(string.Format("Could not find Talent id: {0}", dataModel.TalentId));
            return;
        }
        dataModel.NeedLevel = tableTalent.NeedLevel;
        dataModel.LevelLock = PlayerDataManager.Instance.GetLevel() < dataModel.NeedLevel;
        dataModel.ShowLock = true;
        if (dataModel.Count > 0)
        {
            dataModel.ShowLine = true;
            dataModel.ShowLock = false;
        }
        else
        {
            dataModel.ShowLine = false;
        }

        dataModel.TalentEnable = false;

        if (dataModel.Count < tableTalent.MaxLayer)
        {
            if (tableTalent.BeforeId == -1)
            {
                dataModel.TalentEnable = true;
            }
            else
            {
                TalentCellDataModel talent = null;
                if (PlayerDataManager.Instance.mAllTalents.TryGetValue(tableTalent.BeforeId, out talent))
                {
                    if (talent.Count >= tableTalent.BeforeLayer)
                    {
                        dataModel.TalentEnable = true;
                        dataModel.ShowLock = false;
                    }
                }
            }
        }
    }

    public static void InitTalents(this SkillDataModel skillData, Dictionary<int, int> talentDictionary)
    {
        skillData.Talents.Clear();
        skillData.AttrPanel.Clear();
        PlayerDataManager.Instance.mAllTalents.Clear();
        var talentCache = PlayerDataManager.Instance.mAllTalents;
        var enumerator = talentDictionary.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var i = enumerator.Current;
            var talentCell = new TalentCellDataModel();
            talentCell.TalentId = i.Key;
            talentCell.Count = i.Value;
            talentCell.InitializeTalentCell();
            skillData.Talents.Add(talentCell);
            AddTalentAttrToPanel(skillData.AttrPanel, talentCell);
            talentCache.Add(i.Key, talentCell);
        }

        //刷新天赋影响的技能,必须天赋初始化完成后
        var enumerator2 = PlayerDataManager.Instance.mAllSkills.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            var skill = enumerator2.Current.Value;
            skill.RefreshSkillData();
        }
    }

    public static void RefreshCast(this SkillItemDataModel dataModel)
    {
        if (dataModel.SkillId == -1 || dataModel.SkillId == 0)
        {
            dataModel.SkillCost = -1;
            return;
        }
        if (dataModel.SkillLv == 0)
        {
            return;
        }
        RefreshLevelCast(dataModel);
        var type = StringConvert.Level_Value_Ref(10000000 + 999, dataModel.SkillLv - 1);
        var upgradeTableIndex = Table.GetSkill(dataModel.SkillId).NeedMoney;
        var cost = StringConvert.Level_Value_Ref(10000000 + upgradeTableIndex, dataModel.SkillLv - 1);

        dataModel.SkillCost = 0;
        dataModel.SkillSparCost = 0;

        if (type == 5)
        {
            dataModel.SkillSparCost = cost;
        }
        if (type == 2)
        {
            dataModel.SkillCost = cost;
        }
    }

    public static void RefreshLevelCast(this SkillItemDataModel dataModel)
    {
        if (dataModel.SkillLv == 0)
        {
            return;
        }

        var level = PlayerDataManager.Instance.GetLevel();
        dataModel.SkillUpGradeLv = dataModel.SkillLv*5;
        dataModel.ShowUpGradeBtn = dataModel.SkillUpGradeLv <= level;

        var type = StringConvert.Level_Value_Ref(10000000 + 999, dataModel.SkillLv - 1);
        if (type == 5)
        {
            dataModel.ShowGoldCost = false;
            dataModel.ShowSparCost = true;
        }
        else if (type == 2)
        {
            dataModel.ShowGoldCost = true;
            dataModel.ShowSparCost = false;
        }
    }

    public static void RefreshSkill(this SkillDataModel skillData)
    {
        if (skillData.AllSkills.Count < 4)
        {
            Logger.Error("Missing normalAttack skill .allSkills < 4 !");
            return;
        }

        skillData.NormailAttack.Clear();
        skillData.OtherSkills.Clear();


        {
            // foreach(var skillItemDataModel in skillData.AllSkills)
            var __enumerator1 = (skillData.AllSkills).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var skillItemDataModel = __enumerator1.Current;
                {
                    skillItemDataModel.RefreshCast();
                    var tbSkill = Table.GetSkill(skillItemDataModel.SkillId);
                    if (tbSkill.Type == 0)
                    {
                        skillData.NormailAttack.Add(skillItemDataModel);
                    }
                    else
                    {
                        skillData.OtherSkills.Add(skillItemDataModel);
                    }
                }
            }
        }
    }

    public static void RefreshSkillData(this SkillItemDataModel item)
    {
        var tbSkill = Table.GetSkill(item.SkillId);
        item.ChargeLayerTotal = ObjMyPlayer.GetSkillData_Data(tbSkill, eModifySkillType.Layer);
        item.ChargeLayer = item.ChargeLayerTotal;
        item.CoolDownTimeTotal = ObjMyPlayer.GetSkillData_Data(tbSkill, eModifySkillType.Cd)/1000.0f;
        item.ControlType = ObjMyPlayer.GetSkillData_Data(tbSkill, eModifySkillType.ControlType);
        item.MaxTargetCount = ObjMyPlayer.GetSkillData_Data(tbSkill, eModifySkillType.TargetCount);
    }
}