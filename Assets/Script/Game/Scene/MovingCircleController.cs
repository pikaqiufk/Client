using System;
#region using

using UnityEngine;

#endregion

public class MovingCircleController : MonoBehaviour
{
    private ParticleSystem[] mPS;

    public Color Col
    {
        set
        {
            if (null == mPS)
            {
                mPS = gameObject.GetComponentsInChildren<ParticleSystem>();
            }
            if (null == mPS)
            {
                return;
            }

            {
                var __array1 = mPS;
                var __arrayLength1 = __array1.Length;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var p = __array1[__i1];
                    {
	                    if (null != p.renderer.sharedMaterial)
	                    {
		                    p.renderer.sharedMaterial.SetColor("_TintColor", value);
	                    }
                    }
                }
            }
        }
    }

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