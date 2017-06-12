using System;
#region using

using UnityEngine;

#endregion

public class LoginLogic : MonoBehaviour
{
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