#region using

using System;
using UnityEngine;

#endregion

public class NcDrawFpsText : MonoBehaviour
{
    private float accum;
    private int frames;
    private bool m_bUpdate;
    private float timeleft;
    // -------------------------------------------------------------------------------------------
    public float updateInterval = 0.5F;
    // -------------------------------------------------------------------------------------------
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

        m_bUpdate = true;
        if (!guiText)
        {
            //LogModule.DebugLog("UtilityFramesPerSecond needs a GUIText component!");
            enabled = false;
            return;
        }
        timeleft = updateInterval;

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
        timeleft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0)
        {
            // display two fractional digits (f2 format)
            var fps = accum/frames;
            var format = String.Format("{0:F2} FPS", fps);
            guiText.text = format;

            if (fps < 10)
            {
                guiText.material.color = Color.red;
            }
            else if (fps < 30)
            {
                guiText.material.color = Color.yellow;
            }
            else
            {
                guiText.material.color = Color.green;
            }

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}