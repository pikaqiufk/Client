using System;
using UnityEngine;
using System.Collections;

public class UIDragRotate : MonoBehaviour {

	public Transform Target;
	public bool Horizontal = true;
	public bool Vertical = false;
	public float Step = 1.0f;
    public bool WorldDirection = false;
	// Use this for initialization
	void Start () {
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
	
	void OnDrag (Vector2 delta)
	{
		if(null==Target)
		{
			return;
		}

		if(Horizontal)
		{
		    if (WorldDirection)
            {
                Target.Rotate(Vector3.up, -delta.x * Step);
		    }
		    else
		    {
		        Target.Rotate(Target.transform.up, -delta.x*Step);
		    }
		}
		if (Vertical)
		{
		    if (WorldDirection)
            {
                Target.Rotate(Vector3.right, delta.y * Step);
		    }
		    else
		    {
		        Target.Rotate(Target.transform.right, delta.y*Step);
		    }
		}
	}

}
