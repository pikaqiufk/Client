using System;
using UnityEngine;
using System.Collections;
using BehaviourMachine;

public class BundleLoaderDebugWindow : MonoBehaviour {


    private bool m_bUpdate;
    public static Rect startRect = new Rect(Screen.width - 400, 0, 500, 200);
    public bool allowDrag = true;
    private readonly Rect rect1 = new Rect(10, 10, startRect.width, startRect.height);
    private readonly Rect rect2 = new Rect(10, 50, startRect.width, startRect.height);
    private readonly Rect rect3 = new Rect(10, 90 , startRect.width, startRect.height);
    private readonly Rect rect4 = new Rect(10, 160, 300, 20);
    private readonly Rect rect7 = new Rect(0, 0, Screen.width, Screen.height);
    private GUIStyle style;

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (ResourceManager.Instance.UseAssetBundle)
        {
            m_bUpdate = true;
        }
        else
        {
            m_bUpdate = false;
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    private void OnGUI()
    {

        if (!m_bUpdate)
        {
            return;
        }

        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.fontSize = 20;
            style.alignment = TextAnchor.UpperLeft;
        }
        var rect = startRect;
        startRect = GUI.Window(1, rect, DoMyWindow, "");
    }

    private void DoMyWindow(int windowID)
    {
        lock (BundleLoader.Instance.mWaitingDownloadBundles)
        {
            GUI.Label(rect1, string.Format("剩余文件数量  {0} 个", BundleLoader.Instance.mWaitingDownloadBundles.Count), style);
        }

        GUI.Label(rect2, string.Format("错误 {0}" , BundleLoader.Instance.ErrorMessage), style);

        if (BundleLoader.Instance.FirstPriorityDownLoading)
        {
            style.normal.textColor = Color.red;
        }
        else
        {
            style.normal.textColor = Color.white;
        }
        GUI.Label(rect3, string.Format("下载 {0}" , BundleLoader.Instance.DownLoadingFileName), style);
        if (allowDrag)
        {
            GUI.DragWindow(rect7);
        }

        GUI.HorizontalScrollbar(rect4, 1.0f, BundleLoader.Instance.GetWwwProgress(), 1.0f, 0);



    }


}
