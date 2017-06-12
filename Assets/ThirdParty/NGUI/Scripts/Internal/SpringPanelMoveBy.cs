//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Similar to SpringPosition, but also moves the panel's clipping. Works in local coordinates.
/// </summary>

[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Internal/Spring Panel")]
public class SpringPanelMoveBy : MonoBehaviour
{
    static public SpringPanelMoveBy current;

	/// <summary>
	/// Target position to spring the panel to.
	/// </summary>

	public Vector3 target = Vector3.zero;

	/// <summary>
	/// Strength of the spring. The higher the value, the faster the movement.
	/// </summary>

	public float strength = 10f;

	public delegate void OnFinished ();

	/// <summary>
	/// Delegate function to call when the operation finishes.
	/// </summary>

    public SpringPanel.OnFinished onFinished;

	UIPanel mPanel;
	Transform mTrans;
	UIScrollView mDrag;

	/// <summary>
	/// Cache the transform.
	/// </summary>

	void Start ()
	{
		mPanel = GetComponent<UIPanel>();
		mDrag = GetComponent<UIScrollView>();
		mTrans = transform;
	}

	/// <summary>
	/// Advance toward the target position.
	/// </summary>

	void Update ()
	{
	    AdvanceTowardsPosition();
	}

    /// <summary>
    /// Advance toward the target position.
	/// </summary>

	protected virtual void AdvanceTowardsPosition ()
    {
        float delta = RealTime.deltaTime;

		bool trigger = false;
		Vector3 before = mTrans.localPosition;
        Vector3 after = NGUIMath.SpringLerp(before, before+target, strength, delta);
        Vector3 offset = after - before;
        target = target - offset;
        //bool isLast = false;
        if (target.sqrMagnitude < 0.01f)
		{
			after = target;
			enabled = false;
			trigger = true;
		    //isLast = true;
		}
        UIScrollViewSimple tempScrollViewSimple = mTrans.GetComponent<UIScrollViewSimple>();
        if (tempScrollViewSimple!=null)
        {
            tempScrollViewSimple.MoveRelative(offset, true);
            //if (isLast)
            //{
            //    if (tempScrollViewSimple.centerOnChild != null)
            //    {
            //        tempScrollViewSimple.centerOnChild.Recenter();
            //    }
            //}
        }
        else
        {
            mTrans.localPosition = after;
            Vector2 cr = mPanel.clipOffset;
            cr.x -= offset.x;
            cr.y -= offset.y;
            mPanel.clipOffset = cr;
        }

		if (mDrag != null) mDrag.UpdateScrollbars(false);
        if (tempScrollViewSimple != null) tempScrollViewSimple.UpdateScrollbars(false);

		if (trigger && onFinished != null)
		{
			current = this;
			onFinished();
			current = null;
		}
    }

	/// <summary>
	/// Start the tweening process.
	/// </summary>

    static public SpringPanelMoveBy Begin(GameObject go, Vector3 pos, float strength,SpringPanel.OnFinished act = null)
	{
        SpringPanelMoveBy sp = go.GetComponent<SpringPanelMoveBy>();
        if (sp == null) sp = go.AddComponent<SpringPanelMoveBy>();
		sp.target = pos;
		sp.strength = strength;
        sp.onFinished = act;
		sp.enabled = true;
		return sp;
	}
}
