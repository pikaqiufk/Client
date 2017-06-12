using System.ComponentModel;
using System;
#region using

using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SkillFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public UIButton BtnClose;
	    private bool deleteBind = true;
	
	    private readonly string[] skillTalentPrefab =
	    {
	        "UI/SkillFrame/TalentJianShi01.prefab",
	        "UI/SkillFrame/TalentFaShi01.prefab",
	        "UI/SkillFrame/TalentGongShou01.prefab"
	    };
	
	    private readonly string[] talentPrefab =
	    {
	        "UI/SkillFrame/PracticeJianShi.prefab",
	        "UI/SkillFrame/PracticeFaShi.prefab",
	        "UI/SkillFrame/PracticeGongShou.prefab"
	    };

	    private GameObject skillTalentSelect = null;
	    private int roleID;
	    public Transform SkillTalentRoot;
	    public UISprite SprClose;
	    public UISprite SprOpen;
	    public Transform TalentRoot;
	    public TouchSpinning touch;
	
	    private IEnumerator SetupSkillTalent(SkillDataModel dataModel)
	    {
	        var holder = ResourceManager.PrepareResourceWithHolder<GameObject>(skillTalentPrefab[roleID]);
	        yield return holder.Wait();
	        var skillTalentFrame = Instantiate(holder.Resource) as GameObject;
	        var t = skillTalentFrame.transform;
	        //t.parent = SkillTalentRoot;
	        t.SetParentEX(SkillTalentRoot);
	        //touch.Initialize(t);
            var skillGuide = SkillTalentRoot.GetComponentsInChildren<SkillGuideHelper>(true);

	        for (var i = 0; i < skillGuide.Length; i++)
	        {
                skillGuide[i].Initialize(t);
	        }

	        t.localPosition = Vector3.zero;
	        t.localRotation = Quaternion.identity;
	        t.localScale = Vector3.one;
	        if (!t.gameObject.activeSelf)
	        {
	            t.gameObject.SetActive(true);
	        }
	        var talentDic = new Dictionary<int, TalentCellDataModel>();
	        {
	            // foreach(var talent in dataModel.Talents)
	            var __enumerator2 = (dataModel.Talents).GetEnumerator();
	            while (__enumerator2.MoveNext())
	            {
	                var talent = __enumerator2.Current;
	                {
	                    talentDic.Add(talent.TalentId, talent);
	                }
	            }
	        }
	
	        var cacheDic = new Dictionary<int, List<TalentCellDataModel>>();
	
	        var skillCellLogicContainer = skillTalentFrame.GetComponentsInChildren<SkillTalentCell>(true);
	        {
	            var __array3 = skillCellLogicContainer;
	            var __arrayLength3 = __array3.Length;
	            for (var __i3 = 0; __i3 < __arrayLength3; ++__i3)
	            {
	                var logic = __array3[__i3];
	                {
	                    var talentEnable = false;
	                    TalentCellDataModel talentDataModel;
	                    if (talentDic.TryGetValue(logic.SkillTalentId, out talentDataModel))
	                    {
	                        talentEnable = true;
	                        logic.CellDataModel = talentDataModel;
	                    }
	
	                    if (!talentEnable)
	                    {
	                        var talent = new TalentCellDataModel();
	                        talent.TalentId = logic.SkillTalentId;
	                        talent.Count = 0;
	                        logic.CellDataModel = talent;
	                        var e = new UIEvent_SkillFrame_AddUnLearnedTalent(talent);
	                        EventDispatcher.Instance.DispatchEvent(e);
	                    }
	
	                    var talentTable = Table.GetTalent(logic.CellDataModel.TalentId);
	                    if (talentTable.ModifySkill != -1)
	                    {
	                        List<TalentCellDataModel> list;
	                        if (cacheDic.TryGetValue(talentTable.ModifySkill, out list))
	                        {
	                            list.Add(logic.CellDataModel);
	                        }
	                        else
	                        {
	                            var l = new List<TalentCellDataModel>();
	                            l.Add(logic.CellDataModel);
	                            cacheDic.Add(talentTable.ModifySkill, l);
	                        }
	                    }
	                }
	            }
	        }
	        var skillBoxContainer = skillTalentFrame.GetComponentsInChildren<SkillOutBox>(true);
	        {
	            dataModel.SkillBoxes.Clear();
	            var skillbox = skillBoxContainer;
	            var length = skillbox.Length;
	            var fristId = 0;
	            for (var i = 0; i < length; ++i)
	            {
	                var skillBoxLogic = skillbox[i];
	                {
	                    var skillBox = new SkillBoxDataModel();
	                    skillBox.SkillId = skillBoxLogic.SkillId;
	                    List<TalentCellDataModel> talentList;
	                    if (cacheDic.TryGetValue(skillBox.SkillId, out talentList))
	                    {
	                        {
	                            var __list5 = talentList;
	                            var __listCount5 = __list5.Count;
	                            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
	                            {
	                                var cellDataModel = __list5[__i5];
	                                {
	                                    skillBox.SkillTalents.Add(cellDataModel);
	                                }
	                            }
	                        }
	                    }
	                    skillBox.ShowSkillBox = 0;
	                    skillBoxLogic.BoxDataModel = skillBox;
	                    var e = new UIEvent_SkillFrame_AddSkillBoxDataModel(skillBox);
	                    EventDispatcher.Instance.DispatchEvent(e);
	                    if (i == 0)
	                    {
	                        fristId = skillBox.SkillId;
	                    }
	                }
	            }

// 	            EventDispatcher.Instance.DispatchEvent(
// 	                new UIEvent_SkillFrame_OnSkillTalentSelected(fristId));
	            var skilldata = PlayerDataManager.Instance.mAllSkills;
	            var ievent = new UIEvent_SkillFrame_SkillSelect(skilldata[fristId]);
	            EventDispatcher.Instance.DispatchEvent(ievent);
	        }
	
	        //天赋球数据好了初始化开放和已加点特效
	        var c = skillCellLogicContainer.Length;
	        for (var i = 0; i < c; i++)
	        {
	            var logic = skillCellLogicContainer[i];
	            logic.InitEffect();
	        }
	    }
	
	    private IEnumerator SetupTalent(SkillDataModel dataModel)
	    {
	        var holder = ResourceManager.PrepareResourceWithHolder<GameObject>(talentPrefab[roleID]);
	        yield return holder.Wait();
	        var talentFrame = Instantiate(holder.Resource) as GameObject;
	        var t = talentFrame.transform;
	        //t.parent = TalentRoot;
	        t.SetParentEX(TalentRoot);
	        t.localScale = Vector3.one;
            t.localPosition = Vector3.zero;
	        talentFrame.SetActive(true);
	        var background = t.FindChild("bg");
	        var trigger = background.gameObject.AddComponent<UIEventTrigger>();
	        trigger.onPress.Add(new EventDelegate(
	            () =>
	            {
	                var e = new UIEvent_SkillFrame_TalentBallClick(-1);
	                EventDispatcher.Instance.DispatchEvent(e);
	            }));
	
	        var talentListDataModel = dataModel.Talents;
	        var talentFrametransformchildCount0 = t.childCount;
	        for (var i = 0; i < talentFrametransformchildCount0; i++)
	        {
	            var child = t.GetChild(i);
	            var cellLogic = child.GetComponent<TalentCellLogic>();
	            if (cellLogic == null)
	            {
	                continue;
	            }
	
	            var talentEnable = false;
	            {
	                // foreach(var cellData in talentListDataModel)
	                var __enumerator1 = (talentListDataModel).GetEnumerator();
	                while (__enumerator1.MoveNext())
	                {
	                    var cellData = __enumerator1.Current;
	                    {
	                        if (cellLogic.TalentId == cellData.TalentId)
	                        {
	                            cellLogic.CellDataModel = cellData;
	                            talentEnable = true;
	                            break;
	                        }
	                    }
	                }
	            }
	            if (!talentEnable)
	            {
	                var talent = new TalentCellDataModel();
	                talent.TalentId = cellLogic.TalentId;
	                talent.Count = 0;
	                cellLogic.CellDataModel = talent;
	                var e = new UIEvent_SkillFrame_AddUnLearnedTalent(talent);
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	        }
	    }
	
	    public void OnClickUpgradeSkill()
	    {
	        var e = new UIEvent_SkillFrame_UpgradeSkill();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnCloseClick()
	    {
	        PlayerDataManager.Instance.WeakNoticeData.SkillCanUpgrade = false;
	        PlayerDataManager.Instance.WeakNoticeData.SkillTotal = false;
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SkillFrameUI));
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.SkillFrameUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            deleteBind = false;
	        }
	        else
	        {
	            if (deleteBind == false)
	            {
	                RemoveBindEvent();
	            }
	            deleteBind = true;
	        }
	    }

        private void onSkillTalentBallSelect(int id)
        {
            {
                if (null == skillTalentSelect)
                {
                    var res = ResourceManager.PrepareResourceSync<GameObject>
                        ("Effect/UI/SkillFrame/UI_Talent_Select.prefab");
                    skillTalentSelect = Instantiate(res) as GameObject;
                }

                var talentcells = SkillTalentRoot.GetComponentsInChildren<SkillTalentCell>();

                Transform talentBall = null;


                for (int i = 0; i < talentcells.Length; i++)
                {
                    if (id == talentcells[i].SkillTalentId)
                    {
                        talentBall = talentcells[i].gameObject.transform;
                        break;
                    }
                }

                if (null != talentBall)
                {
                    var parent = talentBall;
                    Transform t = skillTalentSelect.transform;
                    t.parent = parent.transform;
                    t.localPosition = Vector3.zero;
                    t.localRotation = Quaternion.identity;
                    t.localScale = Vector3.one;
                    skillTalentSelect.layer = parent.gameObject.layer;
                }
            }
        }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
            PlayerDataManager.Instance.PlayerDataModel.SkillData.PropertyChanged -= OnEvent_PropertyChange;
	        if (deleteBind == false)
	        {
	            RemoveBindEvent();
	        }
	        deleteBind = true;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var e = new UIEvent_SkillFrame_OnDisable();
	        EventDispatcher.Instance.DispatchEvent(e);
	        if (deleteBind)
	        {
	            RemoveBindEvent();
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (deleteBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	            var controllerBase = UIManager.Instance.GetController(UIConfig.SkillFrameUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.SkillData);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.WeakNoticeData);
	
	            var controller = UIManager.Instance.GetController(UIConfig.ShareFrame);
	            Binding.SetBindDataSource(controller.GetDataModel(""));
	
	            //默认选中第一个技能
	            //UIEvent_SkillFrame_SkillSelect e = new UIEvent_SkillFrame_SkillSelect();
	            //EventDispatcher.Instance.DispatchEvent(e);
	
	            //如果之前打开过天赋描述则关闭
	            var logic = gameObject.GetComponentInChildren<TalentPanel>();
	            if (null != logic)
	            {
	                logic.OnCloseClick();
	            }
	        }
	        deleteBind = true;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnResetSkillTalent()
	    {
	        var e = new UIEvent_SkillFrame_OnResetSkillTalent();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnResetTanentClick()
	    {
	        var e = new UIEvent_SkillFrame_OnResetTalent();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void RemoveBindEvent()
	    {
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	    }
	
	    public void SkillTab()
	    {
	        // PlayerDataManager.Instance.WeakNoticeData.SkillCanUpgrade = false;
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        roleID = PlayerDataManager.Instance.GetRoleId();
	        var dataModel = PlayerDataManager.Instance.PlayerDataModel.SkillData;
	        BtnClose.onClick.Add(new EventDelegate(OnCloseClick));
	
	        //修炼界面
	        StartCoroutine(SetupTalent(dataModel));
	
	        //技能天赋界面
	        StartCoroutine(SetupSkillTalent(dataModel));
            dataModel.PropertyChanged += OnEvent_PropertyChange;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void ToggleSpr()
	    {
	        SprOpen.active = !SprOpen.active;
	        SprClose.active = !SprClose.active;
	    }

        private void OnEvent_PropertyChange(object o, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "TalentIdSelected")
            {
                var datamodel = o as SkillDataModel;
                onSkillTalentBallSelect(datamodel.TalentIdSelected);
            }
        }



	}
}