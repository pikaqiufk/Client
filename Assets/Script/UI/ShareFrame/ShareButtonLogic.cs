using System;
#region using

using System.Collections;
using System.IO;
using EventSystem;
using UnityEngine;

#endregion

public class ShareButtonLogic : MonoBehaviour
{
    public static Texture2D ScreenShot;
    public static string SharePath = Path.Combine(Application.temporaryCachePath, "share.JPG");
    public GameObject ButtonShare;
    private bool m_Update;
    private string mDrawString;
    private Rect startRect;
    private GUIStyle style;

    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        var button = GetComponent<UIButton>();
        if (null != button)
        {
            button.onClick.Add(new EventDelegate(OnBtnShare));
        }

        m_Update = false;
        startRect = new Rect(Screen.width/2f - 300, Screen.height - 40, Screen.width, Screen.height);
        var name = PlayerDataManager.Instance.PlayerDataModel.CharacterBase.Name;
        var servername = PlayerDataManager.Instance.ServerName;
        var formatString = GameUtils.GetDictionaryText(240164);
        mDrawString = string.Format(formatString, name, servername);
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    public void OnBtnShare()
    {
        //OpenShareFrame();
    }

    private void OnGUI()
    {
        if (!m_Update)
        {
            return;
        }

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            // style.alignment = TextAnchor.MiddleCenter;
            var defaultSize = 26;
            var radio = Screen.height/750f;
            style.fontSize = (int) (26*radio);
        }

        GUI.Label(startRect, mDrawString, style);
    }

    private void OpenShareFrame()
    {
        ResourceManager.Instance.StartCoroutine(ScreenShoot());
    }

    private IEnumerator ScreenShoot()
    {
        m_Update = true;
        ButtonShare.SetActive(false);

        yield return new WaitForEndOfFrame();
        var rect = new Rect(0, 0, Screen.width, Screen.height);
        ScreenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
        ScreenShot.ReadPixels(rect, 0, 0);
        ScreenShot.Apply();
        var bytes = ScreenShot.EncodeToJPG();
        File.WriteAllBytes(SharePath, bytes);
        EventDispatcher.Instance.DispatchEvent(new UI_EVENT_ShareBtnShow(true));
        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ShareFrame));
        m_Update = false;
    }
}