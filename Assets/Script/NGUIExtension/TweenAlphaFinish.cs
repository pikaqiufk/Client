using System;
using UnityEngine;
using System.Collections;

public class TweenAlphaFinish : MonoBehaviour
{
    public TweenAlpha AlphaTween;
    private bool mReverse;
    private float mDuation;
    private float mDelay;
	void Start ()
	{
#if !UNITY_EDITOR
try
{
#endif

        mDuation = AlphaTween.duration;
	    mDelay = AlphaTween.delay;
	    mReverse = false;
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
    public void OnTweenFinfish()
    {
        mReverse = !mReverse;
        if (mReverse)
        {
            AlphaTween.delay = 0;
        }
        else
        {
            AlphaTween.delay = mDelay;
        }
    }
}
