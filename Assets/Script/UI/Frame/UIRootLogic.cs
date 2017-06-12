using System;
#region using

using UnityEngine;

#endregion

public class UIRootLogic : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        UIManager.Instance.ResetUIRoot(gameObject, gameObject.GetComponentInChildren<Camera>());

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}