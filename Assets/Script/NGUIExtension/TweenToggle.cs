using System;
#region using

using UnityEngine;

#endregion

public class TweenToggle : MonoBehaviour
{
    private bool mToggle;
    public UITweener[] Tweens;

    public bool Toggle
    {
        get { return mToggle; }
        set
        {
            if (mToggle == value)
            {
                return;
            }
            mToggle = value;
            DoAnimation();
        }
    }

    private void DoAnimation()
    {
        if (Tweens == null)
        {
            return;
        }
        foreach (var tween in Tweens)
        {
            tween.ResetForPlay();
            if (mToggle)
            {
                tween.PlayForward();
            }
            else
            {
                tween.PlayReverse();
            }
        }
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (Tweens != null)
        {
            foreach (var tween in Tweens)
            {
                tween.ResetForPlay();
            }
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        //Tweens = GetComponents<UITweener>();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}