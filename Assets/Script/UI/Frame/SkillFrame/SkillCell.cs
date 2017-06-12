using System;
#region using

using System.Collections;
using System.ComponentModel;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

public class SkillCell : MonoBehaviour
{
    private static GameObject s_objEffect;
    private Coroutine coroutine;
    public SkillItemDataModel ItemLogic;
    private GameObject levelUpEffect;

    private IEnumerator DestroyEffectLevelup()
    {
        yield return new WaitForSeconds(1);
        if (levelUpEffect)
        {
            NGUITools.Destroy(levelUpEffect);
            levelUpEffect = null;
        }
        coroutine = null;
    }

    public void OnClickCellItem()
    {
        if (ItemLogic != null)
        {
            var e = new UIEvent_SkillFrame_SkillSelect(ItemLogic);
            EventDispatcher.Instance.DispatchEvent(e);
        }
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        ItemLogic.PropertyChanged -= OnEvent_PropertyChange;
        EventDispatcher.Instance.RemoveEventListener(UIEvent_SkillFrame_SkillLevelUpEffect.EVENT_TYPE, StartEffectLevelUp);

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

        ItemLogic.PropertyChanged += OnEvent_PropertyChange;
        EventDispatcher.Instance.AddEventListener(UIEvent_SkillFrame_SkillLevelUpEffect.EVENT_TYPE, StartEffectLevelUp);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void OnEvent_PropertyChange(object o, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == "ShowToggle")
        {
            ToggleSelectEffect(ItemLogic.ShowToggle);
        }
    }

    private void ToggleSelectEffect(bool bShow)
    {
        if (bShow)
        {
            ResourceManager.PrepareResource<GameObject>
                ("Effect/UI/JiNengXuanZhong.prefab", res =>
                {
                    if (gameObject)
                    {
                        s_objEffect = NGUITools.AddChild(gameObject, res);
                    }
                });
        }
        else
        {
            if (s_objEffect)
            {
                NGUITools.Destroy(s_objEffect);
                s_objEffect = null;
            }
        }
    }

    private void StartEffectLevelUp(IEvent ievent)
    {
        var e = ievent as UIEvent_SkillFrame_SkillLevelUpEffect;
        if (ItemLogic.SkillId == e.skillId)
        {
            ResourceManager.PrepareResource<GameObject>
                ("Effect/UI/JiNengShengJi.prefab", res =>
                {
                    if (levelUpEffect)
                    {
                        NGUITools.Destroy(levelUpEffect);
                        if (null != coroutine)
                        {
                            ResourceManager.Instance.StopCoroutine(coroutine);
                        }
                    }
                    levelUpEffect = NGUITools.AddChild(gameObject, res);
                    coroutine = ResourceManager.Instance.StartCoroutine(DestroyEffectLevelup());
                });
        }
    }
}