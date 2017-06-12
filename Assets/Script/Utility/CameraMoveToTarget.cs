using UnityEngine;
using System.Collections;
using BehaviourMachine;
using System;

public class CameraMoveToTarget : MonoBehaviour 
{
	public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
	public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public float zSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
	public Vector3 Destination = Vector3.zero;
	public bool IsMoving { get; private set; }

	void Awake ()
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

	IEnumerator MoveToCoroutine(Vector3 p,Action call)
	{
		IsMoving = true;
		while (true)
		{
			yield return new WaitForFixedUpdate();
			if (TrackPlayer())
			{
				break;
			}
		}
		if (null != call)
		{
			call();
		}
		IsMoving = false;

		yield break;
	}

	bool TrackPlayer ()
	{
		// By default the target x and y coordinates of the camera are it's current x and y coordinates.
		float targetX = transform.position.x;
		float targetY = transform.position.y;
		float targetZ = transform.position.z;
		targetX = Mathf.Lerp(transform.position.x, Destination.x, xSmooth * Time.deltaTime);
		targetY = Mathf.Lerp(transform.position.y, Destination.y, ySmooth * Time.deltaTime);
		targetZ = Mathf.Lerp(transform.position.z, Destination.z, zSmooth * Time.deltaTime);

		transform.position = new Vector3(targetX, targetY, targetZ);

		if ((xSmooth > 0 && Mathf.Abs(transform.position.x - Destination.x) > 0.01f) ||
			(ySmooth > 0 && Mathf.Abs(transform.position.y - Destination.y) > 0.01f) ||
			(zSmooth > 0 && Mathf.Abs(transform.position.z - Destination.z) > 0.01f))
		{
			return false;
		}

		return true;
	}

	public void MoveTo(Vector3 p, Action call = null)
	{
		Destination = p;
		StopAllCoroutines();
		StartCoroutine(MoveToCoroutine(p, call));
	}
}
