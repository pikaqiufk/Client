using System;
#region using

using CinemaDirector;
using UnityEngine;

#endregion

public class CutSceneLogic : MonoBehaviour
{
    public UITexture BottomBoder;
    public bool ShowBoder = true;
    public UIButton SkipButton;
    public bool Skippable = true;
    public UITexture TopBoder;
    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        var Scene = gameObject.GetComponent<Cutscene>();

        if (true != ShowBoder)
        {
            TopBoder.active = false;
            BottomBoder.active = false;
        }

        SkipButton.onClick.Add(new EventDelegate(Scene, "Skip"));
        if (true != Skippable)
        {
            SkipButton.active = false;
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Update is called once per frame
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