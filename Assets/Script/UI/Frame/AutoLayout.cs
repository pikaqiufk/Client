using System;
#region using

using UnityEngine;

#endregion

public class AutoLayout : MonoBehaviour
{
    public void OnEnable()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        var grid = gameObject.GetComponentInParent<UIGrid>();
        if (grid)
        {
            grid.Reposition();
        }

#if !UNITY_EDITOR
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
        }
#endif
    }

    public void OnDisable()
    {
#if !UNITY_EDITOR
        try
        {
#endif

        var grid = gameObject.GetComponentInParent<UIGrid>();
        if (grid)
        {
            grid.Reposition();
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