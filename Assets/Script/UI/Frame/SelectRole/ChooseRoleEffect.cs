using System;
#region using

using System.Collections;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ChooseRoleEffect : MonoBehaviour
	{
	    public GameObject Camera;
	    public Vector3 Dir;
	    public float MoveTime = 1.8f;
	    public Vector3 Pos;
	
	    private IEnumerator CameraCoroutine(float time, Vector3 pos, Vector3 angle)
	    {
	        while (!ChooseRoleFrame.s_IsModelLoaded)
	        {
	            yield return new WaitForSeconds(0.2f);
	        }
	
	        var forward = Quaternion.Euler(angle.x, angle.y, angle.z)*Vector3.forward;
	
	        while (time > 0)
	        {
	            time -= Time.deltaTime;
	            Camera.transform.position =
	                Vector3.Lerp(Camera.transform.position,
	                    pos,
	                    Time.deltaTime/time);
	
	            Camera.transform.forward =
	                Vector3.Slerp(Camera.transform.forward,
	                    forward,
	                    Time.deltaTime/time);
	
	            yield return null;
	        }
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        StartCoroutine(CameraCoroutine(MoveTime, Pos, Dir));
	    
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