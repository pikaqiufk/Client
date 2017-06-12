using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

public class ShareLogic : MonoBehaviour
{
    public UITexture SharePic;
    public GameObject TweenObj;

    public void OnBtnClose()
    {
        var e = new Close_UI_Event(UIConfig.ShareFrame);
        EventDispatcher.Instance.DispatchEvent(e);
    }

    public void OnBtnShare()
    {
        var path = ShareButtonLogic.SharePath;
        var content = GameUtils.GetDictionaryText(240160);
        PlatformHelper.ShareToPlatfrom(content, path, "");
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (null != ShareButtonLogic.ScreenShot)
        {
            Destroy(ShareButtonLogic.ScreenShot);
            ShareButtonLogic.ScreenShot = null;
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

        if (null != ShareButtonLogic.ScreenShot)
        {
            ShareButtonLogic.ScreenShot.filterMode = FilterMode.Bilinear;
            ShareButtonLogic.ScreenShot.anisoLevel = 2;
            SharePic.mainTexture = ShareButtonLogic.ScreenShot;
        }

        PlayTween();
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void PlayTween()
    {
        TweenObj.transform.localScale = new Vector3(1.7f, 1.7f, 1);
        iTween.ScaleTo(TweenObj,
            iTween.Hash("scale", new Vector3(1.9f, 1.9f, 1f), "time", 0.1f, "easetype", iTween.EaseType.linear));
        iTween.ScaleTo(TweenObj,
            iTween.Hash("scale", new Vector3(1.7f, 1.7f, 1f), "time", 0.1f, "easetype", iTween.EaseType.linear, "delay",
                0.1f));
        iTween.ScaleTo(TweenObj,
            iTween.Hash("scale", new Vector3(1f, 1f, 1f), "time", 0.4f, "easetype", iTween.EaseType.linear, "delay",
                0.4f));
    }
}