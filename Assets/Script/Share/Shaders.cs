using System;
#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

public class Shaders : MonoBehaviour
{
    public List<Shader> AutoShaders;
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