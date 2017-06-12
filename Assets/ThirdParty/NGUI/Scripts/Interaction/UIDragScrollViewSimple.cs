//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Allows dragging of the specified scroll view by mouse or touch.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Drag Scroll View Simple")]
public class UIDragScrollViewSimple : MonoBehaviour
{
	/// <summary>
	/// Reference to the scroll view that will be dragged by the script.
	/// </summary>

	public UIScrollViewSimple scrollView;

	// Legacy functionality, kept for backwards compatibility. Use 'scrollView' instead.
    [HideInInspector]
    [SerializeField]
    UIScrollViewSimple draggablePanel;

	Transform mTrans;
    UIScrollViewSimple mScroll;
	bool mAutoFind = false;
	bool mStarted = false;

	/// <summary>
	/// Automatically find the scroll view if possible.
	/// </summary>

	void OnEnable ()
	{
		mTrans = transform;

		// Auto-upgrade
		if (scrollView == null && draggablePanel != null)
		{
			scrollView = draggablePanel;
			draggablePanel = null;
		}

		if (mStarted && (mAutoFind || mScroll == null))
			FindScrollView();
	}

	/// <summary>
	/// Find the scroll view.
	/// </summary>

	void Start ()
	{
		mStarted = true;
		FindScrollView();
	}

	/// <summary>
	/// Find the scroll view to work with.
	/// </summary>

	void FindScrollView ()
	{
		// If the scroll view is on a parent, don't try to remember it (as we want it to be dynamic in case of re-parenting)
        UIScrollViewSimple sv = NGUITools.FindInParents<UIScrollViewSimple>(mTrans);

		if (scrollView == null || (mAutoFind && sv != scrollView))
		{
			scrollView = sv;
			mAutoFind = true;
		}
		else if (scrollView == sv)
		{
			mAutoFind = true;
		}
		mScroll = scrollView;
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	void OnPress (bool pressed)
    {
		//硬编码，如果在引导中限制滚动
		if (null != GuideManager.Instance && GuideManager.Instance.IsGuiding())
		{
			return;
		}

		// If the scroll view has been set manually, don't try to find it again
		if (mAutoFind && mScroll != scrollView)
		{
			mScroll = scrollView;
			mAutoFind = false;
		}

		if (scrollView && enabled && NGUITools.GetActive(gameObject))
		{
			scrollView.Press(pressed);
			
			if (!pressed && mAutoFind)
			{
                scrollView = NGUITools.FindInParents<UIScrollViewSimple>(mTrans);
				mScroll = scrollView;
			}
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		//硬编码，如果在引导中限制滚动
		if (null != GuideManager.Instance && GuideManager.Instance.IsGuiding())
		{
			return;
		}

       // Debug.Log(delta);
		if (scrollView && NGUITools.GetActive(this))
			scrollView.Drag();
	}

	/// <summary>
	/// If the object should support the scroll wheel, do it.
	/// </summary>

	void OnScroll (float delta)
	{
		//硬编码，如果在引导中限制滚动
		if (null != GuideManager.Instance && GuideManager.Instance.IsGuiding())
		{
			return;
		}

		if (scrollView && NGUITools.GetActive(this))
			scrollView.Scroll(delta);
	}
}
