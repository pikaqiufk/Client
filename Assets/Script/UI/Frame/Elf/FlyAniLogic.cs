#region using

using System;
using UnityEngine;

#endregion

namespace GameUI
{
	[RequireComponent(typeof (TweenPosition))]
	[RequireComponent(typeof (TweenScale))]
	public class FlyAniLogic : MonoBehaviour
	{
	    private Vector3 OriPos;
	    private Vector3 OriScale;
	    private Transform ParentTrans;
	    private Transform Trans;
	    private TweenPosition TweenPos;
	    private TweenScale TweenScale;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        TweenPos = GetComponent<TweenPosition>();
	        TweenScale = GetComponent<TweenScale>();
	        Trans = transform;
	        ParentTrans = Trans.parent;
	        OriPos = Trans.localPosition;
	        OriScale = Trans.localScale;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        Reset();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void Reset()
	    {
	        if (TweenPos != null && TweenScale != null)
	        {
	            TweenPos.ResetForPlay();
	            TweenScale.ResetForPlay();
	            TweenPos.enabled = false;
	            TweenScale.enabled = false;
	            Trans.localPosition = OriPos;
	            Trans.localScale = OriScale;
	        }
	    }
	
	    public void StartFly(Vector3 to, bool needScale, bool inverse, Action onFinished)
	    {
	        if (!isActiveAndEnabled)
	        {
	            return;
	        }
	
	        var posFrom = Trans.localPosition;
	        var posTo = ParentTrans.InverseTransformPoint(to);
	        if (inverse)
	        {
	            TweenPos.from = posTo;
	            TweenPos.to = posFrom;
	        }
	        else
	        {
	            TweenPos.from = posFrom;
	            TweenPos.to = posTo;
	        }
	        TweenPos.onFinished.Clear();
	        TweenPos.AddOnFinished(() =>
	        {
	            Reset();
	            if (onFinished != null)
	            {
	                onFinished();
	            }
	        });
	        TweenPos.ResetForPlay();
	        TweenPos.PlayForward();
	
	        if (needScale)
	        {
	            var scaleFrom = Trans.localScale;
	            var scaleTo = Vector3.one;
	
	            if (inverse)
	            {
	                TweenScale.from = scaleTo;
	                TweenScale.to = scaleFrom;
	            }
	            else
	            {
	                TweenScale.from = scaleFrom;
	                TweenScale.to = scaleTo;
	            }
	            TweenScale.ResetForPlay();
	            TweenScale.PlayForward();
	        }
	    }
	}
}