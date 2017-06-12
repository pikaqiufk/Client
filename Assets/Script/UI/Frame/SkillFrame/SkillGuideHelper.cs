using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventSystem;
using GameUI;

public class SkillGuideHelper : MonoBehaviour
{
    private bool isSkillTalentGuide;
    private readonly List<SkillOutBox> skillBoxList = new List<SkillOutBox>();


    public void Initialize(Transform t)
    {
        var skillboxs = t.gameObject.GetComponentsInChildren<SkillOutBox>(true);
        var count = skillboxs.Length;

        skillBoxList.Clear();

        for (var i = 0; i < count; ++i)
        {
            var skillbox = skillboxs[i];
            skillBoxList.Add(skillbox);
        }
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
        try
        {
#endif
        EventDispatcher.Instance.AddEventListener(UIEvent_UpdateGuideEvent.EVENT_TYPE, OnGuideEvent);
        StartCoroutine(RefreshUi());
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
        EventDispatcher.Instance.RemoveEventListener(UIEvent_UpdateGuideEvent.EVENT_TYPE, OnGuideEvent);
#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }


    public void OnGuideEvent(IEvent ievent)
    {
        var e = ievent as UIEvent_UpdateGuideEvent;
        var guideid = e.Step;
        if (guideid == 120 || guideid == 130 || guideid == 140)
        {
            isSkillTalentGuide = true;
        }
    }

    private void RunGuide()
    {
        if (isSkillTalentGuide)
        {
            var roleClass = PlayerDataManager.Instance.GetRoleId();
            var skillid = -1;
            switch (roleClass)
            {
                case 0:
                    skillid = 6;
                    break;
                case 1:
                    skillid = 105;
                    break;
                case 2:
                    skillid = 204;
                    break;
            }

            var index = -1;
            var c = skillBoxList.Count;
            for (var i = 0; i < c; i++)
            {
                var skill = skillBoxList[i];
                if (skill.SkillId == skillid)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                SkillTalentSelected(skillid);
            }
            isSkillTalentGuide = false;
        } //补充功能 移动到有技能点的技能上
        //补充2 变为2d之后不需要旋转了，直接选择
        else
        {
            var c = skillBoxList.Count;
            var skillid = skillBoxList[0].BoxDataModel.SkillId;
            for (var i = 0; i < c; i++)
            {
                if (skillBoxList[i].BoxDataModel.LastCount >= 1)
                {
                    skillid = skillBoxList[i].BoxDataModel.SkillId;
                    break;
                }
            }

            SkillTalentSelected(skillid);
        }
    }

    private IEnumerator RefreshUi()
    {
        yield return new WaitForSeconds(0.2f);
        RunGuide();
    }

    private void SkillTalentSelected(int skillid)
    {
        EventDispatcher.Instance.DispatchEvent(
            new UIEvent_SkillFrame_OnSkillTalentSelected(skillid));
    }
}
