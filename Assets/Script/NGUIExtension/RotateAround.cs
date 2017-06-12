using System;
#region using

using UnityEngine;

#endregion

public class RotateAround : MonoBehaviour
{
    private Transform mTransform;
    public float Speed = 2;
	public bool Forward = true;
	public bool Up = false;
	public bool Right = false;
    private void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        mTransform = transform;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif
		var dir = mTransform.right;
		if (Forward)
	    {
			dir = mTransform.forward;
	    }
		else if (Up)
	    {
			dir = mTransform.up; 
	    }

		mTransform.RotateAround(dir, Speed * Time.deltaTime);

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }
}