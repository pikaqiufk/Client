#region using

using System;
using System.Collections;
using UnityEngine;

#endregion

public class NcDrawFpsRect : MonoBehaviour
{
    public static Rect startRect = new Rect(0, 0, 100, 100); // The rect the window is initially displayed at.
    private float accum; // FPS accumulated over the interval
    public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
    public bool centerTop = true;
    private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    private int frames; // Frames drawn over the interval
    public float frequency = 0.5F; // The update frequency of the fps
    private float m_AndroidFPS;
    private bool m_bUpdate;
    public int nbDecimal = 1; // How many decimal do you want to display
    private readonly Rect rect1 = new Rect(0, -40, startRect.width, startRect.height);
    private readonly Rect rect2 = new Rect(0, -25, startRect.width, startRect.height);
    private readonly Rect rect3 = new Rect(0, -10, startRect.width, startRect.height);
    private readonly Rect rect4 = new Rect(0, 5, startRect.width, startRect.height);
    private readonly Rect rect5 = new Rect(0, 20, startRect.width, startRect.height);
    private readonly Rect rect6 = new Rect(0, 35, startRect.width, startRect.height);
    private readonly Rect rect7 = new Rect(0, 0, Screen.width, Screen.height);
    private string sFPS = ""; // The fps formatted into a string.
    private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.
    public bool updateColor = true; // Do you want the color to change if the FPS gets low

    private void DoMyWindow(int windowID)
    {
        GUI.Label(rect1, sFPS + " FPS", style);
        GUI.Label(rect2, "M: " + (GC.GetTotalMemory(false)/(1024*1024f)).ToString("f1"), style);
        GUI.Label(rect3, "C:" + (int) (ResourceManager.Instance.mfCacheMB), style);
        GUI.Label(rect4, "Delay:" + (int) (NetManager.Instance.AverageDelay*1000), style);
        GUI.Label(rect5, "mps:" + NetManager.Instance.MessagePerSecond, style);
        GUI.Label(rect6, "wmc:" + NetManager.Instance.WaitingMessageCount, style);

        if (allowDrag)
        {
            GUI.DragWindow(rect7);
        }
    }

    private IEnumerator FPS()
    {
        while (true)
        {
            // Update the FPS
            var fps = accum/frames;
            sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

            //Update the color
            color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.yellow : Color.red);

            accum = 0.0F;
            frames = 0;

            yield return new WaitForSeconds(frequency);
        }
    }

    private void OnGUI()
    {
        if (!m_bUpdate)
        {
            return;
        }
        // Copy the default label skin, change the color and the alignement
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
        }

        GUI.color = updateColor ? color : Color.white;
        var rect = startRect;
        if (centerTop)
        {
            rect.x += Screen.width/2 - rect.width/2;
        }
        startRect = GUI.Window(0, rect, DoMyWindow, "");
        if (centerTop)
        {
            startRect.x -= Screen.width/2 - rect.width/2;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    if(m_AndroidFPS <= Time.time)
     {
            Logger.Debug("CTCFPS:" + sFPS);
            m_AndroidFPS = Time.time + 1f;
     }
#endif
    }

    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (!PlatformHelper.IsEnableDebugMode())
        {
            return;
        }

        m_AndroidFPS = Time.time + 1f;
        m_bUpdate = true;
        StartCoroutine(FPS());

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (!m_bUpdate)
        {
            return;
        }
        accum += Time.timeScale/Time.deltaTime;
        ++frames;


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}