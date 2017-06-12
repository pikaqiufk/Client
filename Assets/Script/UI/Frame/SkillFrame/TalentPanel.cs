using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

public class TalentPanel : MonoBehaviour
{
    private TweenAlpha alphaTween;
    private TweenScale scaleTween;

    public void OnAddPointClick()
    {
        var e = new UIEvent_SkillFrame_AddTalentPoint();
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnCloseClick()
    {
        //选中天赋id为-1时自动关闭描述界面
        var e = new UIEvent_SkillFrame_TalentBallClick(-1);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif


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

        if (null != alphaTween)
        {
            alphaTween.ResetToBeginning();
            alphaTween.PlayForward();
        }

        if (null != scaleTween)
        {
            scaleTween.ResetToBeginning();
            scaleTween.PlayForward();
        }


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void OnToggleValueChange()
    {
        OnCloseClick();
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif
        scaleTween = gameObject.GetComponent<TweenScale>();
        alphaTween = gameObject.GetComponent<TweenAlpha>();


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}