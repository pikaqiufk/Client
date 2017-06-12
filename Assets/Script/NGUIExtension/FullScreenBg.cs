using System;
#region using

using UnityEngine;

#endregion

public class FullScreenBg : MonoBehaviour
{
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif
        var BackGround = GetComponent<UIWidget>();
        var UiRoot = BackGround.root;
        var height = 1024;
        var width = 2048;
        var s = Screen.height/(float) UiRoot.activeHeight;
        BackGround.height = height;
        BackGround.width = width;
        BackGround.transform.localScale = Vector3.one*(UiRoot.activeHeight/640.0f*0.625f);
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


#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}