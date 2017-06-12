using System;
using UnityEngine;
using System.Collections;

public class TweenButton : MonoBehaviour
{
    public BoxCollider Button;
	void Start () 
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

    public void OnClickBtn()
    {
        Button.enabled = false;
    }

    public void OnClickFinish()
    {
        Button.enabled = true;
    }
}
