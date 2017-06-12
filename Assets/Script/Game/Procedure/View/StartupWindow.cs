using System;
#region using

using UnityEngine;

#endregion

public class StartupWindow : MonoBehaviour
{
    public UIProgressBar ProgressBar = null;

    private void Awake()
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

    private void Start()
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

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        ProgressBar.value = StartupLogic.Instance.GetLoadingPercent();

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}