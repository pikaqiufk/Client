using System;
#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class TrainingFunc : MonoBehaviour
	{
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (UICamera.mainCamera == null)
	        {
	            return;
	        }
	        var skycube = UICamera.mainCamera.transform.Find("SkyCube");
	        if (skycube != null)
	        {
	            skycube.gameObject.SetActive(false);
	            UICamera.mainCamera.nearClipPlane = -10f;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var skycube = UICamera.mainCamera.transform.Find("SkyCube");
	        if (skycube != null)
	        {
	            skycube.gameObject.SetActive(true);
	            UICamera.mainCamera.nearClipPlane = -1f;
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
}