#region using

using System;
using EventSystem;
using UnityEngine;


#endregion

public class RebornFunctionHint : MonoBehaviour
{
    public TweenPosition Tween;

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif

        EventDispatcher.Instance.RemoveEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, TriggerEvent);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void OnPlayOver()
    {
        gameObject.SetActive(false);
    }

    private void ShowBtn(Action callback = null)
    {
        gameObject.SetActive(true);

        Tween.enabled = true;
        Tween.onFinished.Clear();
        if (null != callback)
        {
            Tween.SetOnFinished(new EventDelegate(() => { callback(); }));
        }
        Tween.PlayForward();
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        gameObject.SetActive(false);

        if (true != GameSetting.Instance.EnableNewFunctionTip)
        {
            return;
        }

        if (null == GameLogic.Instance)
        {
            return;
        }

        EventDispatcher.Instance.AddEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, TriggerEvent);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    public void TriggerEvent(IEvent ievent)
    {
        if (true != GameSetting.Instance.EnableNewFunctionTip)
        {
            return;
        }

        if (null == GameLogic.Instance)
        {
            return;
        }

        var e = ievent as UIEvent_PlayMainUIBtnAnimEvent;
        if (null == e)
        {
            return;
        }

        if (0 != e.BtnName.CompareTo("BtnReborn"))
        {
            return;
        }

        ShowBtn(e.CallBack);
    }
}