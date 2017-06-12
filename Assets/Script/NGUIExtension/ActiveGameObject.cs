using System;
#region using

using UnityEngine;

#endregion

public class ActiveGameObject : MonoBehaviour
{
    public void SetTargetActive(GameObject o)
    {
        o.SetActive(true);
    }

    public void SetTargetDeactive(GameObject o)
    {
        o.SetActive(false);
    }

    // Use this for initialization
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
}