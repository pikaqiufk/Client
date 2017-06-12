using System;

using UnityEngine;
using System.Collections;

public class HorizontalEffect : MonoBehaviour {

    Transform mTransform;
	// Use this for initialization
	void Awake () {
#if !UNITY_EDITOR
try
{
#endif

        mTransform = gameObject.transform;
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
	
    void LateUpdate()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTransform.rotation = Quaternion.identity;
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
}
