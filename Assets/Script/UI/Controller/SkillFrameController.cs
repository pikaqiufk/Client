#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ClientDataModel;
using ClientService;
using DataContract;
using DataTable;
using EventSystem;
using ScorpionNetLib;
using Shared;

#endregion

public class SkillFrameController : IControllerBase
{
    public SkillFrameController()
    {
        CleanUp();

        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillSelect.EVENT_TYPE, OnClicSkillItem);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_EquipSkill.EVENT_TYPE, OnSkillEquip);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SwapEquipSkill.EVENT_TYPE, OnSwapSkillEquip);
        //EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnDisable.EVENT_TYPE, SyncSkillEquipData);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_UpgradeSkill.EVENT_TYPE, UpGradeSkill);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_AddUnLearnedTalent.EVENT_TYPE, AddUnLearnedTalent);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_TalentBallClick.EVENT_TYPE, TalentBallClick);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_AddTalentPoint.EVENT_TYPE, AddTalentPoint);
        //EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnSkillBallOpen.EVENT_TYPE, OnSkillBallOpen);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_AddSkillBoxDataModel.EVENT_TYPE, AddSkillBoxes);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnSkillBallClose.EVENT_TYPE, OnSkillBallClose);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_UnEquipSkill.EVENT_TYPE, UnEquipSkill);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnResetSkillTalent.EVENT_TYPE, ResetSkillTalent);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnPlayerLevelUpGrade);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnResetTalent.EVENT_TYPE, ResetTalent);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_NetSyncTalentCount.EVENT_TYPE, NetSyncTalentCount);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillTalentChange.EVENT_TYPE, SkillTalentChange);
        EventDispatcher.Instance.AddEventListener(UIEvent_UseSkill.EVENT_TYPE, UseSkillSuccess);
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_OnSkillTalentSelected.EVENT_TYPE,OnSkillBoxSelected);
        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnLevelUp);
    }

    private int currentShowTalentId;
    public SkillUiDataModel DataModel;
    public int mEquipSkillDirtyMark;
    public bool skillChanged;

    public SkillDataModel SkillDataModel
    {
        get { return PlayerDataManager.Instance.PlayerDataModel.SkillData; }
    }

    public void AddSkillBoxes(IEvent ievent)
    {
        var skilldata = SkillDataModel;
        var e = ievent as UIEvent_SkillFrame_AddSkillBoxDataModel;
        var skillId = e.DataModel.SkillId;
        e.DataModel.MaxCount = Table.GetSkill(skillId).TalentMax;
        {
            // foreach(var talent in skilldata.Talents)
            var __enumerator10 = (skilldata.Talents).GetEnumerator();
            while (__enumerator10.MoveNext())
            {
                var talent = __enumerator10.Current;
                {
                    if (skillId == Table.GetTalent(talent.TalentId).ModifySkill)
                    {
                        e.DataModel.CurrentCount += talent.Count;
                    }
                }
            }
        }
        int lastCount;
        PlayerDataManager.Instance.mSkillTalent.TryGetValue(e.DataModel.SkillId, out lastCount);
        e.DataModel.LastCount = lastCount;
        SkillItemDataModel skillItem;
        PlayerDataManager.Instance.mAllSkills.TryGetValue(e.DataModel.SkillId, out skillItem);
        if (skillItem != null)
        {
            e.DataModel.skillItem = skillItem;
        }
        SkillDataModel.SkillBoxes.Add(e.DataModel);

        //红点
        if (e.DataModel.LastCount != 0)
        {
            PlayerDataManager.Instance.NoticeData.SkillTalentStatus = true;
        }
    }

    public void AddTalentPoint(IEvent ievent)
    {
        var skilldata = SkillDataModel;
        var talentId = skilldata.TalentIdSelected;
        var table = Table.GetTalent(talentId);
        var beforeId = table.BeforeId;

        //检查前置天赋
        if (beforeId > 0)
        {
            TalentCellDataModel talent = null;
            var skilldataTalentsCount1 = skilldata.Talents.Count;
            for (var i = 0; i < skilldataTalentsCount1; i++)
            {
                if (beforeId == skilldata.Talents[i].TalentId)
                {
                    talent = skilldata.Talents[i];
                    break;
                }
            }

            if (talent.Count < table.BeforeLayer)
            {
                var e = new ShowUIHintBoard(705);
                EventDispatcher.Instance.DispatchEvent(e);
                return;
            }
        }

        //检查技能点
        var skillid = Table.GetTalent(talentId).ModifySkill;
        if (skillid < 1)
        {
            //不在消耗技能点了
//             if (skilldata.TalentCount < 1)
//             {
//                 var e = new ShowUIHintBoard(706);
//                 EventDispatcher.Instance.DispatchEvent(e);
//                 return;
//             }
        }
        else
        {
            if (PlayerDataManager.Instance.mSkillTalent.ContainsKey(skillid))
            {
                if (PlayerDataManager.Instance.mSkillTalent[skillid] < 1)
                {
                    var e = new ShowUIHintBoard(706);
                    EventDispatcher.Instance.DispatchEvent(e);
                    return;
                    
                }
            }
        }

        if (talentId >= 0)
        {
            NetManager.Instance.StartCoroutine(SendAddTalentPointMassage(talentId));
        }
        else
        {
            Logger.Error("SelectedTalentId error id:" + talentId);
        }
    }

    public void AddUnLearnedTalent(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_AddUnLearnedTalent;
        e.DataModel.InitializeTalentCell();
        SkillDataModel.Talents.Add(e.DataModel);
        var Talents = PlayerDataManager.Instance.mAllTalents;
        if (!Talents.ContainsKey(e.DataModel.TalentId))
        {
            Talents.Add(e.DataModel.TalentId, e.DataModel);
        }
    }

    public void ChangeSkillLevelFromDataModel(int skillId)
    {
        var skillData = SkillDataModel;
        {
            // foreach(var skill in skillData.OtherSkillss)
            var __enumerator3 = (skillData.OtherSkills).GetEnumerator();
            while (__enumerator3.MoveNext())
            {
                var skill = __enumerator3.Current;
                {
                    skill.ShowToggle = false;
                    if (skill.SkillId == skillId)
                    {
                        skill.SkillLv++;
                        var gold = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold - skill.SkillCost;
                        PlayerDataManager.Instance.SetRes(2, gold);
                        var spar = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Spar - skill.SkillSparCost;
                        PlayerDataManager.Instance.SetRes(5, spar);

                        skill.RefreshCast();
                        RefreshSelected(skill);
                        skill.ShowToggle = true;
                    }
                }
            }
        }
        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.EquipSkill);
    }

    public void ChangeTalentCountToDataModel(int talentId)
    {
        var skillData = SkillDataModel;
        var skillID = Table.GetTalent(talentId).ModifySkill;
        if (skillID != -1)
        {
            {
                // foreach(var skillBoxDataModel in skillData.SkillBoxes)
                var __enumerator5 = (skillData.SkillBoxes).GetEnumerator();
                while (__enumerator5.MoveNext())
                {
                    var skillBoxDataModel = __enumerator5.Current;
                    {
                        if (skillBoxDataModel.SkillId == skillID)
                        {
                            skillBoxDataModel.LastCount -= 1;
                            skillBoxDataModel.CurrentCount += 1;
                            PlayerDataManager.Instance.SkillTalentPointChange(skillBoxDataModel.SkillId, -1);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            //skillData.TalentCount--;
        }

        //去掉修炼小红点
        PlayerDataManager.Instance.NoticeData.PlayerTalentCount = 0;//skillData.TalentCount;
        var talentData = skillData.Talents;
        var list = new List<TalentCellDataModel>();
        {
            // foreach(var talent in talentData)
            var __enumerator7 = (talentData).GetEnumerator();
            while (__enumerator7.MoveNext())
            {
                var talent = __enumerator7.Current;
                {
                    if (talent.TalentId == talentId)
                    {
                        talent.Count++;
                        list.Add(talent);
                    }

                    if (Table.GetTalent(talent.TalentId).BeforeId == talentId)
                    {
                        list.Add(talent);
                    }
                }
            }
        }
        {
            var __list8 = list;
            var __listCount8 = __list8.Count;
            for (var __i8 = 0; __i8 < __listCount8; ++__i8)
            {
                var talentCellDataModel = __list8[__i8];
                {
                    talentCellDataModel.InitializeTalentCell();
                }
            }
        }
        //刷新面板
        if (skillData.AttrPanel.Count > 0)
        {
            skillData.AttrPanel.Clear();
        }
        {
            // foreach(var talentCellDataModel in talentData)
            var __enumerator9 = (talentData).GetEnumerator();
            while (__enumerator9.MoveNext())
            {
                var talentCellDataModel = __enumerator9.Current;
                {
                    if (talentCellDataModel.Count > 0)
                    {
                        SkillDataModelExtension.AddTalentAttrToPanel(skillData.AttrPanel, talentCellDataModel);
                    }
                }
            }
        }

        SkillItemDataModel skillItem;
        if (PlayerDataManager.Instance.mAllSkills.TryGetValue(skillID, out skillItem))
        {
            skillItem.RefreshSkillData();
        }

        RefreshTalentBoardDesc(talentId);

        var e = new UIEvent_SkillFrame_RefreshTalentPanel();
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void ClearCoolDownDirty()
    {
        for (var i = 0; i < 4; i++)
        {
            if (BitFlag.GetLow(mEquipSkillDirtyMark, i))
            {
                var skill = SkillDataModel.EquipSkills[i];
                skill.CoolDownTime = skill.CoolDownTimeTotal;
                if (skill.ChargeLayerTotal > 1)
                {
                    skill.ChargeLayer = 0;
                }
            }
        }
        mEquipSkillDirtyMark = 0;
    }

    public int GetInnateCount()
    {
        var count = 0;
        var talents = SkillDataModel.Talents;
        var c = talents.Count;
        for (var i = 0; i < c; i++)
        {
            var table = Table.GetTalent(talents[i].TalentId);
            if (table.ModifySkill == -1)
            {
                count += talents[i].Count;
            }
        }

        return count;
    }

    public string getSkillTypeString(int type)
    {
        var id = 100000;
        if (type == 0)
        {
            id = 100001;
        }
        return GameUtils.GetDictionaryText(id);
    }

    public void NetSyncTalentCount(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_NetSyncTalentCount;
        if (e.TalentId == -1)
        {
            SkillDataModel.TalentCount = e.Value;
            PlayerDataManager.Instance.NoticeData.PlayerTalentCount = 0; //e.Value;
        }
    }

    //查看某个技能信息
    public void OnClicSkillItem(IEvent ievent)
    {
        var ee = ievent as UIEvent_SkillFrame_SkillSelect;
        var data = ee.DataModel;
        RefreshSelected(data);
        var enumerator = SkillDataModel.OtherSkills.GetEnumerator();

        while (enumerator.MoveNext())
        {
            var skill = enumerator.Current;
            skill.ShowToggle = false;
        }

        data.ShowToggle = true;
    }

    private void OnLevelUp(IEvent ievent)
    {
        var level = PlayerDataManager.Instance.GetLevel();

        var count2 = SkillDataModel.Talents.Count;
        for (var i = 0; i < count2; i++)
        {
            var talents = SkillDataModel.Talents[i];
            if (talents.NeedLevel <= level)
            {
                talents.LevelLock = false;
            }
        }


        if (level%5 != 0)
        {
            return;
        }

        var count = SkillDataModel.OtherSkills.Count;
        for (var i = 0; i < count; i++)
        {
            var skill = SkillDataModel.OtherSkills[i];
            skill.RefreshCast();
            if (!skill.ShowUpGradeBtn)
            {
                continue;
            }
            var type = StringConvert.Level_Value_Ref(10000000 + 999, skill.SkillLv - 1);
            if (type == 5)
            {
                var spar = PlayerDataManager.Instance.GetRes((int) eResourcesType.Spar);
                if (spar < skill.SkillSparCost)
                {
                    continue;
                }
                PlayerDataManager.Instance.WeakNoticeData.SkillCanUpgrade = true;
                PlayerDataManager.Instance.WeakNoticeData.SkillTotal = true;
                return;
            }
            var gold = PlayerDataManager.Instance.GetRes((int) eResourcesType.GoldRes);
            if (gold < skill.SkillCost)
            {
                continue;
            }
            PlayerDataManager.Instance.WeakNoticeData.SkillCanUpgrade = true;
            PlayerDataManager.Instance.WeakNoticeData.SkillTotal = true;
            return;
        }
    }

    //玩家升级之后刷新技能学习数据
    public void OnPlayerLevelUpGrade(IEvent ievent)
    {
        var skilldata = SkillDataModel;
        {
            // foreach(var skillItemDataModel in skilldata.AllSkills)
            var __enumerator1 = (skilldata.AllSkills).GetEnumerator();
            while (__enumerator1.MoveNext())
            {
                var skillItemDataModel = __enumerator1.Current;
                {
                    skillItemDataModel.RefreshLevelCast();
                }
            }
        }
    }

    public void OnSkillBallClose(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_OnSkillBallClose;
        e.DataModel.ShowSkillBox = 0;
    }

//     public void OnSkillBallOpen(IEvent ievent)
//     {
//         var e = ievent as UIEvent_SkillFrame_OnSkillBallOpen;
//         var skillData = SkillDataModel;
//         var skillDataSkillBoxesCount2 = skillData.SkillBoxes.Count;
//         for (var i = 0; i < skillDataSkillBoxesCount2; i++)
//         {
//             skillData.SkillBoxes[i].ShowSkillBox = 0;
//         }
//         e.DataModel.ShowSkillBox = 1;
//         OnSkillBoxSelected(new UIEvent_SkillFrame_OnSkillTalentSelected(e.DataModel.skillItem.SkillId));
//     }

    public void OnSkillBoxSelected(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_OnSkillTalentSelected;
        var c = SkillDataModel.SkillBoxes.Count;
        for (var i = 0; i < c; i++)
        {
            var box = SkillDataModel.SkillBoxes[i];
            if (e.skillid == box.SkillId)
            {
                SkillDataModel.SelectedSkillBox = box;
                box.ShowSkillBox = 1;
                if (null != box.SkillTalents[0])
                {
                    TalentBallClick(new UIEvent_SkillFrame_TalentBallClick(box.SkillTalents[0].TalentId));
                }
            }
            else
            {
                box.ShowSkillBox = 0;
            }
        }
    }

    //拖拽技能到技能bar
    public void OnSkillEquip(IEvent ievent)
    {
        var ee = ievent as UIEvent_SkillFrame_EquipSkill;
        var nIndex = ee.Index;
        var skillId = ee.SkillId;
        var skillData = SkillDataModel;

        SkillItemDataModel equipSkill;
        if (!PlayerDataManager.Instance.mAllSkills.TryGetValue(skillId, out equipSkill))
        {
            Logger.Error("player dont have this skill -----skillID = {0}--", skillId);
            return;
        }

        //如果当前槽位冷却中,新加入技能重新冷却
        var bNewSkill = true;
        var lastSkillCD = (Math.Abs(skillData.EquipSkills[nIndex].CoolDownTime) > 0.0001f);

        //如果技能在别的槽位,把原来的槽位置空
        var equipindex = skillData.EquipSkills.IndexOf(equipSkill);
        if (equipindex != -1)
        {
            var nullSkill = new SkillItemDataModel();
            nullSkill.SkillId = -1;
            skillData.EquipSkills[equipindex] = nullSkill;
            bNewSkill = false;
        }

        if (bNewSkill && lastSkillCD)
        {
            mEquipSkillDirtyMark = BitFlag.IntSetFlag(mEquipSkillDirtyMark, nIndex);
        }

        skillData.EquipSkills[nIndex] = equipSkill;
        skillChanged = true;


        if (ee.BSyncToServer)
        {
            SyncSkillEquipData(null);
        }
        else
        {
            PlayerDataManager.Instance.RefrehEquipPriority();
        }
    }

    //交换装备中的技能
    public void OnSwapSkillEquip(IEvent ievent)
    {
        var ee = ievent as UIEvent_SkillFrame_SwapEquipSkill;
        var skillData = SkillDataModel;
        var swaptemp = skillData.EquipSkills[ee.FromIndex];
        skillData.EquipSkills[ee.FromIndex] = skillData.EquipSkills[ee.TargetIndex];
        skillData.EquipSkills[ee.TargetIndex] = swaptemp;
        skillChanged = true;

        SyncSkillEquipData(null);
    }

    public void RefreshSelected(SkillItemDataModel dataModel)
    {
        SkillDataModel.SelectSkill = dataModel;
        var skilldata = SkillDataModel;
        var selected = skilldata.SelectedSkillList[0];
        selected.SkillId = dataModel.SkillId;
        selected.SkillLv = dataModel.SkillLv;
        var skillTable = Table.GetSkill(dataModel.SkillId);
        selected.ManaCast = StringConvert.Level_Value_Ref(skillTable.NeedMp, dataModel.SkillLv - 1);
        selected.SkillType = getSkillTypeString(skillTable.ControlType);
        selected.SkillItem = dataModel;

        var current = 0;
//         var skillTalent = PlayerDataManager.Instance.mSkillTalent; 
//         if (skillTalent.ContainsKey(dataModel.SkillId))
//         {
//             current = skillTalent[dataModel.SkillId];
//         }

        var skillboxs = PlayerDataManager.Instance.PlayerDataModel.SkillData.SkillBoxes;
        SkillBoxDataModel box = null;
        for (var i = 0; i < skillboxs.Count; i++)
        {
            if (skillboxs[i].SkillId == dataModel.SkillId)
            {
                box = skillboxs[i];
                break;
            }
        }

        if (null != box)
        {
            current = box.LastCount + box.CurrentCount;
        }

        selected.TalentCount = string.Format("{0}/{1}", current, skillTable.TalentMax);

        var selected2 = skilldata.SelectedSkillList[1];
        selected2.SkillId = selected.SkillId;
        selected2.SkillLv = selected.SkillLv == 0 ? 2 : selected.SkillLv + 1;
        selected2.ManaCast = selected.ManaCast;
        selected2.SkillType = selected.SkillType;
    }

    public void RefreshTalentBoardDesc(int talentId)
    {
        if (talentId < 0)
        {
            return;
        }
        var skillData = SkillDataModel;
        var descIndex = 0;
        TalentCellDataModel talentCellData = null;
        {
            // foreach(var talent in skillData.Talents)
            var __enumerator4 = (skillData.Talents).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var talent = __enumerator4.Current;
                {
                    if (talent.TalentId == talentId)
                    {
                        descIndex = talent.Count;
                        talentCellData = talent;
                    }
                }
            }
        }

        var talentTable = Table.GetTalent(talentId);

        if (talentTable.ModifySkill == -1 && talentTable.BeforeId != -1)
        {
            if (null == talentCellData || talentCellData.ShowLock)
            {
                skillData.TalentIdSelected = -1;
                return;
            }
        }
        
        skillData.SelectedTalentLevel = string.Format("{0}/{1}", descIndex, talentTable.MaxLayer);
        skillData.ShowDesBoardAddButton = 1;
        //         if (talentTable.AttrId >= ExpressionHelper.AttrName.Count || talentTable.AttrId < 0)
        //         {
        //             return;
        //         }
        if (!ExpressionHelper.AttrName.ContainsKey(talentTable.AttrId))
        {
            if (descIndex == 0)
            {
                skillData.SelectedTalentDesc = talentTable.BuffDesc[0];
                skillData.ShowDesBoardNext = 0;
                skillData.SelectedTalentDescNext = string.Empty;
            }
            else if (descIndex == talentTable.MaxLayer)
            {
                skillData.SelectedTalentDesc = talentTable.BuffDesc[talentTable.MaxLayer - 1];
                skillData.SelectedTalentDescNext = string.Empty;
                skillData.ShowDesBoardNext = 0;
                skillData.ShowDesBoardAddButton = 0;
            }
            else
            {
                skillData.SelectedTalentDesc = talentTable.BuffDesc[descIndex - 1];
                skillData.SelectedTalentDescNext = talentTable.BuffDesc[descIndex];
                skillData.ShowDesBoardNext = 1;
            }
        }
        else
        {
            var attrName = ExpressionHelper.AttrName[talentTable.AttrId];
            if (descIndex == 0)
            {
                var AttrValue = StringConvert.Level_Value_Ref(10000000 + talentTable.SkillupgradingId,
                    talentCellData.Count + 1);
                skillData.SelectedTalentDesc = string.Format("{0}+{1}", attrName,
                    GameUtils.AttributeValue(talentTable.AttrId, AttrValue));
                skillData.SelectedTalentDescNext = string.Empty;
                skillData.ShowDesBoardNext = 0;
            }
            else if (descIndex == talentTable.MaxLayer)
            {
                var AttrValue = StringConvert.Level_Value_Ref(10000000 + talentTable.SkillupgradingId,
                    talentCellData.Count);
                skillData.SelectedTalentDesc = string.Format("{0}+{1}", attrName,
                    GameUtils.AttributeValue(talentTable.AttrId, AttrValue));
                skillData.SelectedTalentDescNext = string.Empty;
                skillData.ShowDesBoardNext = 0;
                skillData.ShowDesBoardAddButton = 0;
            }
            else
            {
                var AttrValue = StringConvert.Level_Value_Ref(10000000 + talentTable.SkillupgradingId,
                    talentCellData.Count);
                var nextAttrValue = StringConvert.Level_Value_Ref(10000000 + talentTable.SkillupgradingId,
                    talentCellData.Count + 1);
                skillData.SelectedTalentDesc = string.Format("{0}+{1}", attrName,
                    GameUtils.AttributeValue(talentTable.AttrId, AttrValue));
                skillData.SelectedTalentDescNext = string.Format("{0}+{1}", attrName,
                    GameUtils.AttributeValue(talentTable.AttrId, nextAttrValue));
                skillData.ShowDesBoardNext = 1;
            }
        }


        //天赋消耗刷新
        if (talentTable.ModifySkill == -1)
        {
            var tbUpgrade = Table.GetSkillUpgrading(talentTable.CastItemCount);
            skillData.TalentPanelUpgradeCast = tbUpgrade.GetSkillUpgradingValue(talentCellData.Count);
        }
    }

    //重置技能天赋
    public void ResetSkillTalent(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_OnResetSkillTalent;
        var dataModel = SkillDataModel.SelectedSkillBox;
        var skillId = dataModel.SkillId;

        var canReset = dataModel.SkillTalents.All(talent => talent.Count <= 0);

        if (canReset)
        {
            var e2 = new ShowUIHintBoard(707);
            EventDispatcher.Instance.DispatchEvent(e2);
            return;
        }

        var desc = GameUtils.GetDictionaryText(702);
        var skillName = Table.GetSkill(skillId).Name;
        var upgradeCast = Table.GetSkill(skillId).ResetCount*dataModel.CurrentCount;
        var message = string.Format(desc, upgradeCast, skillName);

        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, message, "",
            () =>
            {
                var itemCount = PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Spar;

                if (itemCount < upgradeCast)
                {
                    var e1 = new ShowUIHintBoard(703);
                    EventDispatcher.Instance.DispatchEvent(e1);
                    return;
                }

                NetManager.Instance.StartCoroutine(SendResetSkillTalentMsg(dataModel));
            },
            () => { });
    }

    //重置天赋
    public void ResetTalent(IEvent ievent)
    {
        if (GetInnateCount() == 0)
        {
            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(707));
            return;
        }

        var type = int.Parse(Table.GetClientConfig(258).Value);
        var count = Table.GetClientConfig(259).Value;
        var name = Table.GetItemBase(type).Name;

        var message = string.Format(GameUtils.GetDictionaryText(708), count, name);

        UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, message, "",
            () => { NetManager.Instance.StartCoroutine(SendResetTalentMsg()); },
            () => { });
    }

    public void RestSkillTalentData(SkillBoxDataModel skillBoxData)
    {
        var count = 0;
        var skillBoxDataSkillTalentsCount3 = skillBoxData.SkillTalents.Count;
        for (var i = 0; i < skillBoxDataSkillTalentsCount3; i++)
        {
            count += skillBoxData.SkillTalents[i].Count;
        }
        count += skillBoxData.LastCount;
        skillBoxData.LastCount = count;
        skillBoxData.CurrentCount = 0;
        if (PlayerDataManager.Instance.mSkillTalent.ContainsKey(skillBoxData.SkillId))
        {
            PlayerDataManager.Instance.mSkillTalent[skillBoxData.SkillId] = count;
        }


        {
            // foreach(var talent in skillBoxData.SkillTalents)
            var __enumerator12 = (skillBoxData.SkillTalents).GetEnumerator();
            while (__enumerator12.MoveNext())
            {
                var talent = __enumerator12.Current;
                {
                    talent.Count = 0;
                    talent.InitializeTalentCell();
                }
            }
        }

        //刷新受天赋影响的技能
        PlayerDataManager.Instance.mAllSkills[skillBoxData.SkillId].RefreshSkillData();
    }

    public IEnumerator SendAddTalentPointMassage(int talentId)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.UpgradeInnate(talentId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlatformHelper.Event("skill", "addInnate");
                    PlatformHelper.UMEvent("skill", "addInnate");
                    ChangeTalentCountToDataModel(talentId);
                    if (Table.GetTalent(talentId).ModifySkill == -1)
                    {
                        SkillDataModel.TalentResetButtonShow = true;
                        PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Talant);
                    }
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_InnateNoPoint)
                    {
                        var e = new ShowUIHintBoard(706);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else if (msg.ErrorCode == (int) ErrorCodes.Error_InnateNoBefore)
                    {
                        var e = new ShowUIHintBoard(705);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else if (msg.ErrorCode == (int) ErrorCodes.Error_ResNoEnough)
                    {
                        var ee = new ShowUIHintBoard(210108);
                        EventDispatcher.Instance.DispatchEvent(ee);
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
        }
    }

    public IEnumerator SendResetSkillTalentMsg(SkillBoxDataModel skillBoxData)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.ResetSkillTalent(skillBoxData.SkillId);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlatformHelper.Event("skill", "ResetSkillTalent", skillBoxData.SkillId);
                    PlatformHelper.UMEvent("skill", "ResetSkillTalent", skillBoxData.SkillId);

                    RestSkillTalentData(skillBoxData);
                    RefreshTalentBoardDesc(currentShowTalentId);
                    var e = new ShowUIHintBoard(709);
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.ItemNotEnough)
                    {
                        var e = new ShowUIHintBoard(703);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
        }
    }

    public IEnumerator SendResetTalentMsg()
    {
        using (new BlockingLayerHelper(0))
        {
            var type = int.Parse(Table.GetClientConfig(258).Value);
            var count = int.Parse(Table.GetClientConfig(259).Value);
            var bagCount = PlayerDataManager.Instance.GetRes(type);
            //检查需求道具
            if (bagCount < count)
            {
                var name = Table.GetItemBase(type).Name;
                var message = string.Format(GameUtils.GetDictionaryText(701), name);

                var e = new ShowUIHintBoard(message);
                EventDispatcher.Instance.DispatchEvent(e);
                yield break;
            }

            var msg = NetManager.Instance.ClearInnate(-1);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlatformHelper.UMEvent("skill", "ResetXiuLian");
                    var skillData = SkillDataModel;
                    var talents = skillData.Talents;
                    skillData.AttrPanel.Clear();
                    skillData.TalentResetButtonShow = false;
                    var ee = new UIEvent_SkillFrame_RefreshTalentPanel();
                    EventDispatcher.Instance.DispatchEvent(ee);
                    // foreach (var talent in talents)
                    var enumerator = talents.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var talent = enumerator.Current;
                        if (Table.GetTalent(talent.TalentId).ModifySkill == -1)
                        {
                            talent.Count = 0;
                        }
                    }
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        var talent = enumerator.Current;
                        if (Table.GetTalent(talent.TalentId).ModifySkill == -1)
                        {
                            talent.InitializeTalentCell();
                        }
                    }
                    skillData.TalentCount = msg.Response;
                    var e = new ShowUIHintBoard(709);
                    EventDispatcher.Instance.DispatchEvent(e);
                    PlayerAttr.Instance.SetAttrChange(PlayerAttr.PlayerAttrChange.Talant);
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                }
            }
        }
    }

    //升级技能
    public IEnumerator SendUpGradeSkillMassage(int skillid)
    {
        using (new BlockingLayerHelper(0))
        {
            var msg = NetManager.Instance.UpgradeSkill(skillid);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    PlayerDataManager.Instance.WeakNoticeData.SkillCanUpgrade = false;
                    PlayerDataManager.Instance.WeakNoticeData.SkillTotal = false;

                    PlatformHelper.Event("skill", "upgrade", skillid);
                    PlatformHelper.UMEvent("skill", "upgrade", skillid);
                    EventDispatcher.Instance.DispatchEvent(new UIEvent_SkillFrame_SkillLevelUpEffect(skillid));
                    ChangeSkillLevelFromDataModel(skillid);
                }
                else
                {
                    if (msg.ErrorCode == (int) ErrorCodes.Error_SkillLevelMax)
                    {
                        var e = new ShowUIHintBoard(700);
                        EventDispatcher.Instance.DispatchEvent(e);
                    }
                    else
                    {
                        UIManager.Instance.ShowNetError(msg.ErrorCode);
                    }
                }
            }
        }
    }

    public void SkillTalentChange(IEvent ievent)
    {
        RefreshData(new SkillFrameArguments
        {
            Tab = 1
        });
    }

    public IEnumerator SyncEquipSkillData()
    {
        using (new BlockingLayerHelper(0))
        {
            var skillArray = new Int32Array();
            {
                // foreach(var skillItemDataModel in SkillDataModel.EquipSkills)
                var __enumerator2 = (SkillDataModel.EquipSkills).GetEnumerator();
                while (__enumerator2.MoveNext())
                {
                    var skillItemDataModel = __enumerator2.Current;
                    {
                        skillArray.Items.Add(skillItemDataModel.SkillId == 0 ? -1 : skillItemDataModel.SkillId);
                    }
                }
            }
            var msg = NetManager.Instance.EquipSkill(skillArray);
            yield return msg.SendAndWaitUntilDone();
            if (msg.State == MessageState.Reply)
            {
                if (msg.ErrorCode == (int) ErrorCodes.OK)
                {
                    ClearCoolDownDirty();
                    skillChanged = false;
                    ObjManager.Instance.PrepareMainPlayerSkillResources();
                    PlayerDataManager.Instance.RefrehEquipPriority();
                }
                else
                {
                    UIManager.Instance.ShowNetError(msg.ErrorCode);
                    Logger.Error(string.Format("SyncEquipSkillData error! errorcode :{0}", msg.ErrorCode));
                }
            }
            else
            {
                Logger.Error(string.Format("SyncEquipSkillData error! State :{0}", msg.State));
            }
        }
    }

    public void SyncSkillEquipData(IEvent ievent)
    {
        if (!skillChanged)
        {
            return;
        }

        NetManager.Instance.StartCoroutine(SyncEquipSkillData());
    }

    public void TalentBallClick(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_TalentBallClick;
        if (currentShowTalentId == -1 && e.TalentId == -1)
        {
            return;
        }

        var skillData = SkillDataModel;
        skillData.TalentIdSelected = e.TalentId;
        currentShowTalentId = e.TalentId;
        RefreshTalentBoardDesc(e.TalentId);
    }

    //卸下装备中的技能
    public void UnEquipSkill(IEvent ievent)
    {
        var ee = ievent as UIEvent_SkillFrame_UnEquipSkill;
        var skillitem = new SkillItemDataModel();
        skillitem.SkillId = -1;
        mEquipSkillDirtyMark = BitFlag.IntSetFlag(mEquipSkillDirtyMark, ee.Index);
        SkillDataModel.EquipSkills[ee.Index] = skillitem;
        skillChanged = true;

        SyncSkillEquipData(null);
    }

    public void UpGradeSkill(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_UpgradeSkill;

        var skillData = SkillDataModel.SelectSkill;

        //检查钱
        if (skillData.SkillCost > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Gold)
        {
			/*
            var str = string.Format(GameUtils.GetDictionaryText(270255), Table.GetItemBase(2).Name);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "", () =>
            {
                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));
            });
			 * */
            var ee = new ShowUIHintBoard(210100);
            EventDispatcher.Instance.DispatchEvent(ee);
			EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ExchangeUI));
            return;
        }
        //检查技能水晶
        if (skillData.SkillSparCost > PlayerDataManager.Instance.PlayerDataModel.Bags.Resources.Spar)
        {
            var ee = new ShowUIHintBoard(210108);
            EventDispatcher.Instance.DispatchEvent(ee);
            return;
        }

        var skillid = skillData.SkillId;
        if (skillid != 0)
        {
            NetManager.Instance.StartCoroutine(SendUpGradeSkillMassage(skillid));
        }
        else
        {
            Logger.Debug("upgrade skill index error!");
        }
    }

    public void UseSkillSuccess(IEvent ievent)
    {
        var e = ievent as UIEvent_UseSkill;
        var id = e.SkillId;
        SkillItemDataModel skill;
        if (PlayerDataManager.Instance.mAllSkills.TryGetValue(id, out skill))
        {
            var skilldata = SkillDataModel;
            skilldata.CommonCoolDownTotal = Table.GetSkill(id).CommonCd*0.001f;
            skilldata.CommonCoolDown = skilldata.CommonCoolDownTotal;

            if (skill.ChargeLayer == skill.ChargeLayerTotal)
            {
                skill.CoolDownTime = skill.CoolDownTimeTotal;
            }

            if (skill.ChargeLayerTotal > 1)
            {
                skill.ChargeLayer--;
            }
        }
    }

    public void CleanUp()
    {
        var skilldata = SkillDataModel;
        skilldata.SelectedSkillList[0] = new SkillItemDataSelected();
        skilldata.SelectedSkillList[1] = new SkillItemDataSelected();

        DataModel = new SkillUiDataModel();
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

    public void Close()
    {
    }

    public void Tick()
    {
//         var skillData = SkillDataModel;
// 
//         //公共cd
//         if (skillData.CommonCoolDown > 0)
//         {
//             skillData.CommonCoolDown -= Time.deltaTime;
//             if (skillData.CommonCoolDown <= 0)
//             {
//                 skillData.CommonCoolDown = 0;
//             }
//         }
// 
//         //技能cd
//         int count = skillData.AllSkills.Count;
//         for (int i = 0; i < count; i++)
//         {
//             var skill = skillData.AllSkills[i];
//             if (skill.CoolDownTime > 0)
//             {
//                 skill.CoolDownTime -= Time.deltaTime;
//                 if (skill.CoolDownTime <= 0)
//                 {
//                     skill.CoolDownTime = 0;
//                     if (skill.ChargeLayer != skill.ChargeLayerTotal)
//                     {
//                         skill.ChargeLayer++;
//                         if (skill.ChargeLayer != skill.ChargeLayerTotal)
//                         {
//                             skill.CoolDownTime = skill.CoolDownTimeTotal;
//                         }
//                     }
//                 }
//             }
//         }
    }

    public void RefreshData(UIInitArguments data)
    {
        var args = data as SkillFrameArguments;

        if (args != null && args.Tab != -1)
        {
            SkillDataModel.TabSelectIndex = args.Tab;
        }
        else
        {
            SkillDataModel.TabSelectIndex = 0;
        }

        OnPlayerLevelUpGrade(null);

        //刷新技能天赋数据,使用技能书后会变动
        var boxes = SkillDataModel.SkillBoxes;
        var skillTalentData = PlayerDataManager.Instance.mSkillTalent;
        var boxesCount0 = boxes.Count;
        for (var i = 0; i < boxesCount0; i++)
        {
            var box = boxes[i];
            if (skillTalentData.ContainsKey(box.SkillId))
            {
                box.LastCount = PlayerDataManager.Instance.mSkillTalent[box.SkillId];
            }
        }

        if (GetInnateCount() == 0)
        {
            SkillDataModel.TalentResetButtonShow = false;
        }
        else
        {
            SkillDataModel.TalentResetButtonShow = true;
        }

        RefreshSelected(SkillDataModel.OtherSkills[0]);

        var enumerator = SkillDataModel.OtherSkills.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var skill = enumerator.Current;
            if (skill != null)
            {
                skill.ShowToggle = false;
            }
        }
        SkillDataModel.OtherSkills[0].ShowToggle = true;

        if (null == data)
        {
            return;
        }
    }

    public INotifyPropertyChanged GetDataModel(string name)
    {
        return DataModel;
    }

    public FrameState State { get; set; }
}