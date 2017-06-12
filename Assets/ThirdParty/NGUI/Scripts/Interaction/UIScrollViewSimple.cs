//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using ClientDataModel;
using UnityEngine;

/// <summary>
/// This script, when attached to a panel turns it into a scroll view.
/// You can then attach UIDragScrollView to colliders within to make it draggable.
/// </summary>

[ExecuteInEditMode]
[RequireComponent(typeof(UIPanel))]
[AddComponentMenu("NGUI/Interaction/Scroll View Simple")]
public class UIScrollViewSimple : MonoBehaviour
{
    static public BetterList<UIScrollViewSimple> list = new BetterList<UIScrollViewSimple>();

    public enum Movement
    {
        Horizontal,
        Vertical,
        Unrestricted,
        Custom,
    }

    public enum DragEffect
    {
        None,
        Momentum,
        MomentumAndSpring,
    }

    public enum ShowCondition
    {
        Always,
        OnlyIfNeeded,
        WhenDragging,
    }

	public delegate void OnDragNotification ();

    public UIGridSimple thisGrid;
	/// <summary>
	/// Type of movement allowed by the scroll view.
	/// </summary>

	public Movement movement = Movement.Horizontal;

	/// <summary>
	/// Effect to apply when dragging.
	/// </summary>

	public DragEffect dragEffect = DragEffect.MomentumAndSpring;

	/// <summary>
	/// Whether the dragging will be restricted to be within the scroll view's bounds.
	/// </summary>

	public bool restrictWithinPanel = true;

	/// <summary>
	/// Whether dragging will be disabled if the contents fit.
	/// </summary>

	public bool disableDragIfFits = false;

	/// <summary>
	/// Whether the drag operation will be started smoothly, or if if it will be precise (but will have a noticeable "jump").
	/// </summary>

	public bool smoothDragStart = true;

	/// <summary>
	/// Whether to use iOS drag emulation, where the content only drags at half the speed of the touch/mouse movement when the content edge is within the clipping area.
	/// </summary>	
	
	public bool iOSDragEmulation = true;

	/// <summary>
	/// Effect the scroll wheel will have on the momentum.
	/// </summary>

	public float scrollWheelFactor = 0.25f;

	/// <summary>
	/// How much momentum gets applied when the press is released after dragging.
	/// </summary>

	public float momentumAmount = 35f;

    public Vector3 maxMomentumAmount;
	
	/// <summary>
	/// Horizontal scrollbar used for visualization.
	/// </summary>

	public UIProgressBar horizontalScrollBar;

    public UIPageBar PageBar;

    public GameObject DargBegin;
    public GameObject DargEnd;

    public GameObject UndargBegin;
    public GameObject UndargEnd;

	/// <summary>
	/// Vertical scrollbar used for visualization.
	/// </summary>

	public UIProgressBar verticalScrollBar;

	/// <summary>
	/// Condition that must be met for the scroll bars to become visible.
	/// </summary>

	public ShowCondition showScrollBars = ShowCondition.OnlyIfNeeded;

	/// <summary>
	/// Custom movement, if the 'movement' field is set to 'Custom'.
	/// </summary>

	public Vector2 customMovement = new Vector2(1f, 0f);

	/// <summary>
	/// Content's pivot point -- where it originates from by default.
	/// </summary>

	public UIWidget.Pivot contentPivot = UIWidget.Pivot.TopLeft;

	/// <summary>
	/// Event callback to trigger when the drag process begins.
	/// </summary>

	public OnDragNotification onDragStarted;

	/// <summary>
	/// Event callback to trigger when the drag process finished. Can be used for additional effects, such as centering on some object.
	/// </summary>

	public OnDragNotification onDragFinished;

	/// <summary>
	/// Event callback triggered when the scroll view is moving as a result of momentum in between of OnDragFinished and OnStoppedMoving.
	/// </summary>

	public OnDragNotification onMomentumMove;

	/// <summary>
	/// Event callback to trigger when the scroll view's movement ends.
	/// </summary>

	public OnDragNotification onStoppedMoving;

	// Deprecated functionality. Use 'movement' instead.
	[HideInInspector][SerializeField] Vector3 scale = new Vector3(1f, 0f, 0f);

	// Deprecated functionality. Use 'contentPivot' instead.
	[SerializeField][HideInInspector] Vector2 relativePositionOnReset = Vector2.zero;

	protected Transform mTrans;
	protected UIPanel mPanel;
	protected Plane mPlane;
	protected Vector3 mLastPos;
	protected bool mPressed = false;
	protected Vector3 mMomentum = Vector3.zero;
	protected float mScroll = 0f;
	protected Bounds mBounds;
	protected bool mCalculatedBounds = false;
	protected bool mShouldMove = false;
    protected bool mRestrict = false;
	protected bool mIgnoreCallbacks = false;
	protected int mDragID = -10;
	protected Vector2 mDragStartOffset = Vector2.zero;
	protected bool mDragStarted = false;

    /// <summary>
    /// Panel that's being dragged.
    /// </summary>

    public UIPanel panel
    {
        get { return mPanel ?? (mPanel = GetComponent<UIPanel>()); }
    }

	/// <summary>
	/// Whether the scroll view is being dragged.
	/// </summary>

	public bool isDragging { get { return mPressed && mDragStarted; } }

	/// <summary>
	/// Calculate the bounds used by the widgets.
	/// </summary>

	public virtual Bounds bounds
	{
		get
		{
			if (!mCalculatedBounds)
			{
                mCalculatedBounds = true;
                mTrans = transform;
				mBounds = NGUIMath.CalculateRelativeWidgetBounds(mTrans, mTrans);
			    //mBounds = new Bounds(mTrans.position, new Vector3(10000, 150));
			}
			return mBounds;
		}
	}

	/// <summary>
	/// Whether the scroll view can move horizontally.
	/// </summary>

	public bool canMoveHorizontally
	{
		get
		{
			return movement == Movement.Horizontal ||
				movement == Movement.Unrestricted ||
				(movement == Movement.Custom && customMovement.x != 0f);
		}
	}

	/// <summary>
	/// Whether the scroll view can move vertically.
	/// </summary>

	public bool canMoveVertically
	{
		get
		{
			return movement == Movement.Vertical ||
				movement == Movement.Unrestricted ||
				(movement == Movement.Custom && customMovement.y != 0f);
		}
	}

	/// <summary>
	/// Whether the scroll view should be able to move horizontally (contents don't fit).
	/// </summary>

	public virtual bool shouldMoveHorizontally
	{
		get
		{
            var ret = CalculateBoundary();
		    if (ret != 0)
		    {
		        return true;
		    }
		    return false;
		    //float size = bounds.size.x;
		    //if (mPanel.clipping == UIDrawCall.Clipping.SoftClip) size += mPanel.clipSoftness.x * 2f;
		    //return Mathf.RoundToInt(size - mPanel.width) > 0;
		}
	}

	/// <summary>
	/// Whether the scroll view should be able to move vertically (contents don't fit).
	/// </summary>

	public virtual bool shouldMoveVertically
	{
		get
		{
            var ret = CalculateBoundary();
            if (ret != 0)
            {
                return true;
            }
            return false;
            //float size = bounds.size.y;
            //if (mPanel.clipping == UIDrawCall.Clipping.SoftClip) size += mPanel.clipSoftness.y * 2f;
            //return Mathf.RoundToInt(size - mPanel.height) > 0;
		}
	}

	/// <summary>
	/// Whether the contents of the scroll view should actually be draggable depends on whether they currently fit or not.
	/// </summary>

	protected virtual bool shouldMove
	{
		get
		{
			if (!disableDragIfFits) return true;

			if (mPanel == null) mPanel = GetComponent<UIPanel>();
			Vector4 clip = mPanel.finalClipRegion;
			Bounds b = bounds;

			float hx = (clip.z == 0f) ? Screen.width  : clip.z * 0.5f;
			float hy = (clip.w == 0f) ? Screen.height : clip.w * 0.5f;

			if (canMoveHorizontally)
			{
				if (b.min.x < clip.x - hx) return true;
				if (b.max.x > clip.x + hx) return true;
			}

			if (canMoveVertically)
			{
				if (b.min.y < clip.y - hy) return true;
				if (b.max.y > clip.y + hy) return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Current momentum, exposed just in case it's needed.
	/// </summary>

	public Vector3 currentMomentum
	{
		get
		{
			return mMomentum;
		}
		set
		{
			mMomentum = value;
			mShouldMove = true;
		}
	}

	/// <summary>
	/// Cache the transform and the panel.
	/// </summary>

	void Awake ()
	{
		mTrans = transform;
		mPanel = GetComponent<UIPanel>();
        maxMomentumAmount = new Vector3(0.01f*momentumAmount, 0.01f*momentumAmount, 0.01f*momentumAmount);

		if (mPanel.clipping == UIDrawCall.Clipping.None)
			mPanel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
		
		// Auto-upgrade
		if (movement != Movement.Custom && scale.sqrMagnitude > 0.001f)
		{
			if (scale.x == 1f && scale.y == 0f)
			{
				movement = Movement.Horizontal;
			}
			else if (scale.x == 0f && scale.y == 1f)
			{
				movement = Movement.Vertical;
			}
			else if (scale.x == 1f && scale.y == 1f)
			{
				movement = Movement.Unrestricted;
			}
			else
			{
				movement = Movement.Custom;
				customMovement.x = scale.x;
				customMovement.y = scale.y;
			}
			scale = Vector3.zero;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}

		// Auto-upgrade
		if (contentPivot == UIWidget.Pivot.TopLeft && relativePositionOnReset != Vector2.zero)
		{
			contentPivot = NGUIMath.GetPivot(new Vector2(relativePositionOnReset.x, 1f - relativePositionOnReset.y));
			relativePositionOnReset = Vector2.zero;
#if UNITY_EDITOR
			NGUITools.SetDirty(this);
#endif
		}
	}

	[System.NonSerialized] bool mStarted = false;

	void OnEnable ()
    {
		list.Add(this);
		if (mStarted && Application.isPlaying) CheckScrollbars();

	    if (thisGrid)
	    {
	        thisGrid.enabled = true;
	    }
	}

	void Start ()
    {
		mStarted = true;
        if (Application.isPlaying) CheckScrollbars();
	}

	void CheckScrollbars ()
	{
        
// 		if (horizontalScrollBar != null)
// 		{
// 			EventDelegate.Add(horizontalScrollBar.onChange, OnScrollBar);
// 			horizontalScrollBar.alpha = ((showScrollBars == ShowCondition.Always) || shouldMoveHorizontally) ? 1f : 0f;
// 		}
// 
// 		if (verticalScrollBar != null)
// 		{
// 			EventDelegate.Add(verticalScrollBar.onChange, OnScrollBar);
// 			verticalScrollBar.alpha = ((showScrollBars == ShowCondition.Always) || shouldMoveVertically) ? 1f : 0f;
// 		}
	}

	void OnDisable () { list.Remove(this); }

	/// <summary>
	/// Restrict the scroll view's contents to be within the scroll view's bounds.
	/// </summary>

	public bool RestrictWithinBounds (bool instant) { return RestrictWithinBounds(instant, true, true); }

    /// <summary>
    /// Restrict the scroll view's contents to be within the scroll view's bounds.
    /// </summary>


	public bool RestrictWithinBounds (bool instant, bool horizontal, bool vertical)
	{
		Bounds b = bounds;
        if (b.size == Vector3.zero)
        {
            return true;
        }
	    Vector3 constraint = mPanel.CalculateConstrainOffset(b.min, b.max,true);

		if (!horizontal) constraint.x = 0f;
		if (!vertical) constraint.y = 0f;

		if (constraint.sqrMagnitude > 0.1f)
		{
		    if (CheckPos() != 0) return false;
			if (!instant && dragEffect == DragEffect.MomentumAndSpring)
			{
				// Spring back into place
				Vector3 pos = mTrans.localPosition + constraint;
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
// 			    if (vertical)
// 			    {
//                     if (pos.y - (mTrans.localPosition.y + mPanel.clipOffset.y) > cellLenth)
//                     {
//                         pos.y -= cellLenth;
//                     }  
// 			    }
//                 if (horizontal)
//                 {
//                     if (pos.x + (mTrans.localPosition.x + mPanel.clipOffset.x) < -cellLenth)
//                     {
//                         pos.x += cellLenth;
//                     }
//                 }
                SpringPanel.Begin(mPanel.gameObject, pos, 13f).strength = 8f;
			}
			else
			{
				// Jump back into place
				MoveRelative(constraint);

				// Clear the momentum in the constrained direction
				if (Mathf.Abs(constraint.x) > 0.01f) mMomentum.x = 0;
				if (Mathf.Abs(constraint.y) > 0.01f) mMomentum.y = 0;
				if (Mathf.Abs(constraint.z) > 0.01f) mMomentum.z = 0;
				mScroll = 0f;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Disable the spring movement.
	/// </summary>

	public void DisableSpring ()
	{
		SpringPanel sp = GetComponent<SpringPanel>();
		if (sp != null) sp.enabled = false;
	}

	/// <summary>
	/// Update the values of the associated scroll bars.
	/// </summary>

	public void UpdateScrollbars () { UpdateScrollbars(true); }


    //0 none
    //1 begin
    //2 end
    //3 both
    public int CalculateBoundary()
    {
        float offset = 0.0f;
        float size = 0.0f;
        float softness = 0.0f;
        if (movement == UIScrollViewSimple.Movement.Horizontal) //横向移动
        {
            offset = -transform.localPosition.x;
            size = panel.baseClipRegion.z;
            softness = panel.clipSoftness.x;
        }
        else
        {
            offset = transform.localPosition.y;
            size = panel.baseClipRegion.w;
            softness = panel.clipSoftness.y;
        }

        var totalCount = thisGrid.GetDataCount();
        var gridCount = thisGrid.GetGridCount() - thisGrid.maxPerLine;
        var beginIndex = thisGrid.GetBeginIndex();
        var endIndex = thisGrid.GetEndIndex();

        int ret = 0;
        var isBegin = false;
        var isEnd = false;

        if (totalCount < gridCount - 1)
        {
            ret = 0;
        }
        else
        {
            if (beginIndex > 0 || offset > softness)
            {
                isBegin = true;
            }

            if (endIndex < totalCount - 1)
            {
                isEnd = true;
            }
            else
            {
                var showCount = thisGrid.GetShowCount();
                var length = showCount * cellLenth;
                if (length > size + offset + softness)
                {
                    isEnd = true;
                }
            }

            if (isEnd && isBegin)
            {
                ret = 3;
            }
            else if (!isEnd && isBegin)
            {
                ret = 1;
            }
            else if (isEnd && !isBegin)
            {
                ret = 2;
            }
        }
        return ret;
    }

    /// <summary>
    /// Update the values of the associated scroll bars.
    /// </summary>
	public virtual void UpdateScrollbars (bool recalculateBounds)
	{
		if (mPanel == null) return;

        if (DargBegin != null
            || UndargBegin != null
            || DargEnd != null
            || UndargEnd != null)
        {

            var ret = CalculateBoundary();
            if (ret == 0)
            {
                if (DargBegin != null)
                {
                    DargBegin.SetActive(false);
                }
                if (DargEnd != null)
                {
                    DargEnd.SetActive(false);
                }
                if (UndargBegin != null)
                {
                    UndargBegin.SetActive(false);
                }
                if (UndargEnd != null)
                {
                    UndargEnd.SetActive(false);
                }
            }
            else
            {

                if (UndargBegin != null)
                {
                    UndargBegin.SetActive(true);
                }
                if (UndargEnd != null)
                {
                    UndargEnd.SetActive(true);
                }

                if (DargBegin != null)
                {
                    DargBegin.SetActive(false);
                }
                if (DargEnd != null)
                {
                    DargEnd.SetActive(false);
                }


                if (ret == 1)
                {
                    if (DargBegin != null)
                    {
                        DargBegin.SetActive(true);
                    }
                }
                else if (ret == 2)
                {
                    if (DargEnd != null)
                    {
                        DargEnd.SetActive(true);
                    }  
                }
                else if (ret == 3)
                {
                    if (DargBegin != null)
                    {
                        DargBegin.SetActive(true);
                    }
                    if (DargEnd != null)
                    {
                        DargEnd.SetActive(true);
                    } 
                }
            }
        }

		if (horizontalScrollBar != null || verticalScrollBar != null || PageBar != null)
		{
			if (recalculateBounds)
			{
				mCalculatedBounds = false;
				mShouldMove = shouldMove;
			}

			Bounds b = bounds;
			Vector2 bmin = b.min;
			Vector2 bmax = b.max;
            
            if (movement == UIScrollViewSimple.Movement.Horizontal) //横向移动
            {
                var maxLine = GetMaxLine();
                var maxLenth = maxLine * cellLenth;
                bmin = new Vector2(0, b.min.y);
                bmax = new Vector2(maxLenth, b.max.y);
            }
            else
            {
                var maxLine = GetMaxLine();
                var maxLenth = maxLine * cellLenth;
                bmin = new Vector2(b.min.x, -maxLenth);
                bmax = new Vector2(b.max.x, 0);
            }
		    if (PageBar != null)
		    {
                var rate = thisGrid.GetProgress();
		        var page = thisGrid.GetTotalPage();
                PageBar.RefreshLight(rate, page);
		    }

			if (horizontalScrollBar != null && bmax.x > bmin.x)
			{
				Vector4 clip = mPanel.finalClipRegion;
				int intViewSize = Mathf.RoundToInt(clip.z);
				if ((intViewSize & 1) != 0) intViewSize -= 1;
				float halfViewSize = intViewSize * 0.5f;
				halfViewSize = Mathf.Round(halfViewSize);

				if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
					halfViewSize -= mPanel.clipSoftness.x;
                float viewMin = 0.0f;
                float viewMax = 0.0f;

                var offset = oldoffset ;
			    
                viewMin = -offset ;
                viewMax = -offset + halfViewSize * 2;

				float contentSize = bmax.x - bmin.x;
				float viewSize = halfViewSize * 2f;
				float contentMin = bmin.x;
                float contentMax = bmax.x;

                contentMin = viewMin - contentMin;
				contentMax = contentMax - viewMax;

				UpdateScrollbars(horizontalScrollBar, contentMin, contentMax, contentSize, viewSize, false);
			}

			if (verticalScrollBar != null && bmax.y > bmin.y)
			{
				Vector4 clip = mPanel.finalClipRegion;
				int intViewSize = Mathf.RoundToInt(clip.w);
				if ((intViewSize & 1) != 0) intViewSize -= 1;
				float halfViewSize = intViewSize * 0.5f;
				halfViewSize = Mathf.Round(halfViewSize);

				if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
					halfViewSize -= mPanel.clipSoftness.y;

				float contentSize = bmax.y - bmin.y;
				float viewSize = halfViewSize * 2f;
				float contentMin = bmin.y;
				float contentMax = bmax.y;
			    float viewMin = 0.0f;
                float viewMax = 0.0f;
			    var offset = -oldoffset;


                viewMin = offset - halfViewSize * 2;
                viewMax = offset ;  

				contentMin = viewMin - contentMin;
				contentMax = contentMax - viewMax;

				UpdateScrollbars(verticalScrollBar, contentMin, contentMax, contentSize, viewSize, true);
			}

		}
		else if (recalculateBounds)
		{
			mCalculatedBounds = false;
		}
	}

	/// <summary>
	/// Helper function used in UpdateScrollbars(float) function above.
	/// </summary>

	protected void UpdateScrollbars (UIProgressBar slider, float contentMin, float contentMax, float contentSize, float viewSize, bool inverted)
	{
		if (slider == null) return;

		mIgnoreCallbacks = true;
		{
			float contentPadding;

			if (viewSize < contentSize)
			{
				contentMin = Mathf.Clamp01(contentMin / contentSize);
				contentMax = Mathf.Clamp01(contentMax / contentSize);

				contentPadding = contentMin + contentMax;
			    if (inverted)
			    {
			        slider.value = ((contentPadding > 0.001f) ? 1f - contentMin/contentPadding : 0f);
			    }
			    else
			    {
                    slider.value = ((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);
			    }
// 				slider.value = inverted ? ((contentPadding > 0.001f) ? 1f - contentMin / contentPadding : 0f) :
// 					((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);
			}
			else
			{
				contentMin = Mathf.Clamp01(-contentMin / contentSize);
				contentMax = Mathf.Clamp01(-contentMax / contentSize);

				contentPadding = contentMin + contentMax;
				slider.value = inverted ? ((contentPadding > 0.001f) ? 1f - contentMin / contentPadding : 0f) :
					((contentPadding > 0.001f) ? contentMin / contentPadding : 1f);

				if (contentSize > 0)
				{
					contentMin = Mathf.Clamp01(contentMin / contentSize);
					contentMax = Mathf.Clamp01(contentMax / contentSize);
					contentPadding = contentMin + contentMax;
				}
			}

			UIScrollBar sb = slider as UIScrollBar;
			if (sb != null) sb.barSize = 1f - contentPadding;
		}
		mIgnoreCallbacks = false;
	}

	/// <summary>
	/// Changes the drag amount of the scroll view to the specified 0-1 range values.
	/// (0, 0) is the top-left corner, (1, 1) is the bottom-right.
	/// </summary>

	public virtual void SetDragAmount (float x, float y, bool updateScrollbars)
	{
		if (mPanel == null) mPanel = GetComponent<UIPanel>();

		DisableSpring();

		Bounds b = bounds;
		if (b.min.x == b.max.x || b.min.y == b.max.y) return;

		Vector4 clip = mPanel.finalClipRegion;

		float hx = clip.z * 0.5f;
		float hy = clip.w * 0.5f;
		float left = b.min.x + hx;
		float right = b.max.x - hx;
		float bottom = b.min.y + hy;
		float top = b.max.y - hy;

		if (mPanel.clipping == UIDrawCall.Clipping.SoftClip)
		{
			left -= mPanel.clipSoftness.x;
			right += mPanel.clipSoftness.x;
			bottom -= mPanel.clipSoftness.y;
			top += mPanel.clipSoftness.y;
		}

		// Calculate the offset based on the scroll value
		float ox = Mathf.Lerp(left, right, x);
		float oy = Mathf.Lerp(top, bottom, y);

		// Update the position
		if (!updateScrollbars)
		{
			Vector3 pos = mTrans.localPosition;
			if (canMoveHorizontally) pos.x += clip.x - ox;
			if (canMoveVertically) pos.y += clip.y - oy;
			mTrans.localPosition = pos;
		}

		if (canMoveHorizontally) clip.x = ox;
		if (canMoveVertically) clip.y = oy;

		// Update the clipping offset
		Vector4 cr = mPanel.baseClipRegion;
		mPanel.clipOffset = new Vector2(clip.x - cr.x, clip.y - cr.y);

		// Update the scrollbars, reflecting this change
		if (updateScrollbars) UpdateScrollbars(mDragID == -10);
	}

	/// <summary>
	/// Manually invalidate the scroll view's bounds so that they update next time.
	/// </summary>

	public void InvalidateBounds () { mCalculatedBounds = false; }

	/// <summary>
	/// Reset the scroll view's position to the top-left corner.
	/// It's recommended to call this function before AND after you re-populate the scroll view's contents (ex: switching window tabs).
	/// Another option is to populate the scroll view's contents, reset its position, then call this function to reposition the clipping.
	/// </summary>

	[ContextMenu("Reset Clipping Position")]
	public void ResetPosition()
	{
		if (NGUITools.GetActive(this))
		{
			// Invalidate the bounds
			mCalculatedBounds = false;
			Vector2 pv = NGUIMath.GetPivotOffset(contentPivot);

			// First move the position back to where it would be if the scroll bars got reset to zero
			SetDragAmount(pv.x, 1f - pv.y, false);

			// Next move the clipping area back and update the scroll bars
			SetDragAmount(pv.x, 1f - pv.y, true);
		}
	}

    public void ResetPositionBottom()
    {
        if (NGUITools.GetActive(this))
        {
            // Invalidate the bounds
            mCalculatedBounds = false;
            Vector2 pv = NGUIMath.GetPivotOffset(contentPivot);

            // First move the position back to where it would be if the scroll bars got reset to zero
            SetDragAmount(pv.x, 1f, false);

            // Next move the clipping area back and update the scroll bars
            SetDragAmount(pv.x, 1f, true);
        }
    }

	/// <summary>
	/// Call this function after you adjust the scroll view's bounds if you want it to maintain the current scrolled position
	/// </summary>

	public void UpdatePosition ()
	{
		if (!mIgnoreCallbacks && (horizontalScrollBar != null || verticalScrollBar != null))
		{
			mIgnoreCallbacks = true;
			mCalculatedBounds = false;
			Vector2 pv = NGUIMath.GetPivotOffset(contentPivot);
			float x = (horizontalScrollBar != null) ? horizontalScrollBar.value : pv.x;
			float y = (verticalScrollBar != null) ? verticalScrollBar.value : 1f - pv.y;
			SetDragAmount(x, y, false);
			UpdateScrollbars(true);
			mIgnoreCallbacks = false;
		}
	}

	/// <summary>
	/// Triggered by the scroll bars when they change.
	/// </summary>

	public void OnScrollBar ()
	{
		if (!mIgnoreCallbacks)
		{
			mIgnoreCallbacks = true;
			float x = (horizontalScrollBar != null) ? horizontalScrollBar.value : 0f;
			float y = (verticalScrollBar != null) ? verticalScrollBar.value : 0f;
			SetDragAmount(x, y, false);
			mIgnoreCallbacks = false;
		}
	}

    /// <summary>
    /// Move the scroll view by the specified local space amount.
    /// </summary>
    private float moldoffset;

    public float oldoffset
    {
        get { return moldoffset; }
        set { moldoffset = value; }
    }


    public float cellLenth = 100;
	public virtual void MoveRelative (Vector3 relative, bool moving = false)
	{
        //Logger.Info("MoveRelative oldoffset={0},{1},{2}, pos={3}", relative.x, relative.y, relative.z, mTrans.localPosition.y);
        int ischange = 0;
        if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
        {
            ischange = Modifyoffset(ref relative.x);
        }
        else if (movement == UIScrollViewSimple.Movement.Vertical)//竖向移动
        {
            ischange = Modifyoffset(ref relative.y);
        }

	    mTrans = transform;
        SetScrollViewPostionOffset(relative);
        if (ischange != 0)
        {
            if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
            {
                if (mTrans.localPosition.x > 0)
                {
                    int x = (int)Mathf.Ceil(Mathf.Abs(mTrans.localPosition.x) / cellLenth);
                    var modifyOffset = new Vector3(-cellLenth * x, 0, 0);
                    if (ischange == 2)
                    {
                        modifyOffset.x = oldoffset - mTrans.localPosition.x;
                    }
                    SetScrollViewPostionOffset(modifyOffset);
                    ResetObj();
                }
                else if (mTrans.localPosition.x < -cellLenth)
                {
                    int x = (int)Mathf.Floor(Mathf.Abs(mTrans.localPosition.x) / cellLenth);
                    var modifyOffset = new Vector3(cellLenth * x, 0, 0);
                    if (ischange == 2)
                    {
                        var maxCount = GetDataCount();
                        var rowCount = (maxCount + thisGrid.PerLineCount - 1) / thisGrid.PerLineCount;
                        var GCount = thisGrid.GetGridCount() / thisGrid.PerLineCount;
                        modifyOffset.x = oldoffset + cellLenth * (rowCount - GCount) - mTrans.localPosition.x;
                    }
                    SetScrollViewPostionOffset(modifyOffset);
                    ResetObj();
                }
            }
            else if (movement == UIScrollViewSimple.Movement.Vertical)//竖向移动
            {
                if (mTrans.localPosition.y < 0)
                {
                    int x = (int)Mathf.Ceil(Mathf.Abs(mTrans.localPosition.y) / cellLenth);
                    var modifyOffset = new Vector3(0, cellLenth * x, 0);
                    if (ischange == 2)
                    {
                        modifyOffset.y = oldoffset - mTrans.localPosition.y;
                    }
                    SetScrollViewPostionOffset(modifyOffset);
                    ResetObj();
                }
                else if (mTrans.localPosition.y > cellLenth)
                {
                    int x = (int)Mathf.Floor(Mathf.Abs(mTrans.localPosition.y) / cellLenth);
                    var modifyOffset = new Vector3(0, -cellLenth * x, 0);
                    if (ischange == 2)
                    {
                        var maxCount = GetDataCount();
                        var rowCount = (maxCount + thisGrid.PerLineCount - 1) / thisGrid.PerLineCount;
                        var GCount = thisGrid.GetGridCount() / thisGrid.PerLineCount;
                        modifyOffset.y = oldoffset - cellLenth * (rowCount - GCount) - mTrans.localPosition.y;
                    }
                    SetScrollViewPostionOffset(modifyOffset);
                    ResetObj();
                }
            }
        }

        if (moving)
	    {
	        return;
	    }

		// Update the scroll bars
		UpdateScrollbars(false);
	}

    private void SetScrollViewPostionOffset(Vector3 offset)
    {
        
//         Vector2 modifyOffset = mPanel.clipOffset;
//         modifyOffset.x -= offset.x;
//         modifyOffset.y -= offset.y;
//         mPanel.clipOffset = modifyOffset;
        if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
        {
            mTrans.localPosition += offset;
            mPanel.clipOffset = new Vector2(-mTrans.localPosition.x, mPanel.clipOffset.y);
        }
        else if (movement == UIScrollViewSimple.Movement.Vertical)//竖向移动
        {
            mTrans.localPosition += offset;
            mPanel.clipOffset = new Vector2(mPanel.clipOffset.x, -mTrans.localPosition.y);
        }
    }

    public void ResetScrollViewPostionOffset()
    {
        if (mPanel == null || mTrans == null)
        {
            return;
        }
        mTrans.localPosition = Vector3.zero;
        var pos = Vector3.zero;
//         if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
//         {
//             var x = mPanel.clipSoftness.x / 2;
//             oldoffset = x;
//             SetScrollViewPostionOffset(new Vector3(-x, 0, 0));
//             pos.x = x;
//         }
//         else if (movement == UIScrollViewSimple.Movement.Vertical)//竖向移动
//         {
//             var y = mPanel.clipSoftness.y / 2;
//             oldoffset = -y;
//             SetScrollViewPostionOffset(new Vector3(0, y, 0));
//             pos.y = y;
//         }
        oldoffset = 0;
        SetScrollViewPostionOffset(Vector3.zero);
        if (pos != Vector3.zero)
        {
            //MoveRelative(pos,false);
            //SpringPanel.Begin(mPanel.gameObject, pos, 13f).strength = 8f;        
        }

        mRestrict = true;
    }
    public void ResetObj()
    {
        if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
        {
            if (oldoffset > 0)
            {
                int gridCount = thisGrid.GetGridCount();
                for (int i = 0; i != gridCount; ++i)
                {
                    thisGrid.UpdataCellDataIndex(i, thisGrid.getGameObjectByIndex(i));
                }
            }
            else
            {
                int beforeLook = (int)(-oldoffset / cellLenth);
                int perline = thisGrid.PerLineCount;
                int maxCount = GetDataCount();
                int LastCount = (maxCount + perline - 1) / perline;
                int gridCount = thisGrid.GetGridCount();
                if (maxCount < gridCount)
                {
                    beforeLook = perline * beforeLook;
                    for (int i = 0; i != gridCount; ++i)
                    {
                        thisGrid.UpdataCellDataIndex(beforeLook + i, thisGrid.getGameObjectByIndex(i));
                    }
                    return;
                }
                if (oldoffset < -cellLenth * LastCount + panel.GetViewSize().x)
                {
                    beforeLook--;
                    var maxBeforeLook = (int)((cellLenth * LastCount - panel.GetViewSize().x) / cellLenth);
                    beforeLook = Mathf.Clamp(beforeLook, 0, maxBeforeLook);
                }
                beforeLook = perline * beforeLook;
                for (int i = 0; i != gridCount; ++i)
                {
                    thisGrid.UpdataCellDataIndex(beforeLook + i, thisGrid.getGameObjectByIndex(i));
                }
                //int beforeLook = (int)(-oldoffset / cellLenth);
                //int perline = thisGrid.maxPerLine;
                //if (perline < 1) perline = 1;
                //int maxCount = GetDataCount();
                //maxCount = (maxCount + perline - 1) / perline;
                //if (oldoffset < -cellLenth * maxCount + mPanel.GetViewSize().x)
                //{
                //    return;
                //}
                //int gridCount = thisGrid.GetGridCount();
                //beforeLook = perline * beforeLook;
                //for (int i = 0; i != gridCount; ++i)
                //{
                //    thisGrid.UpdataCellDataIndex(beforeLook + i, thisGrid.getGameObjectByIndex(i));
                //}
            }
        }
        else if (movement == UIScrollViewSimple.Movement.Vertical) //竖向移动
        {

            if (oldoffset < 0)
            {
                int gridCount = thisGrid.GetGridCount();
                for (int i = 0; i != gridCount; ++i)
                {
                    thisGrid.UpdataCellDataIndex(i, thisGrid.getGameObjectByIndex(i));
                }
            }
            else
            {
                int beforeLook = (int)(oldoffset / cellLenth);
                int perline = thisGrid.PerLineCount;
                int maxCount = GetDataCount();
                int LastCount = (maxCount + perline - 1) / perline;
                int gridCount = thisGrid.GetGridCount();
                if (maxCount < gridCount)
                {
                    beforeLook = perline * beforeLook;
                    for (int i = 0; i != gridCount; ++i)
                    {
                        thisGrid.UpdataCellDataIndex(beforeLook+i, thisGrid.getGameObjectByIndex(i));
                    }
                    return;
                }
                if (oldoffset > cellLenth * LastCount - panel.GetViewSize().y)
                {
                    beforeLook--;
                    var maxBeforeLook = (int)((cellLenth*LastCount - panel.GetViewSize().y)/cellLenth);
                    beforeLook = Mathf.Clamp(beforeLook, 0, maxBeforeLook);
                }
                beforeLook = perline * beforeLook;
                for (int i = 0; i != gridCount; ++i)
                {
                    thisGrid.UpdataCellDataIndex(beforeLook + i, thisGrid.getGameObjectByIndex(i));
                }

                //int beforeLook = (int)(oldoffset / cellLenth);
                //int perline = thisGrid.maxPerLine;
                //if (perline < 1) perline = 1;
                //int maxCount = GetDataCount();
                //maxCount = (maxCount + perline - 1) / perline;
                //if (oldoffset > cellLenth * maxCount - mPanel.GetViewSize().y)
                //{
                //    return;
                //}
                //int gridCount = thisGrid.GetGridCount();
                //beforeLook = perline * beforeLook;
                //for (int i = 0; i != gridCount; ++i)
                //{
                //    thisGrid.UpdataCellDataIndex(beforeLook + i, thisGrid.getGameObjectByIndex(i));
                //}
            }
        }
        mCalculatedBounds = false;
    }

    private float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// 0 表示不需要调整数据
    /// 1 表示需要调整数据
    /// 2 表示需要调整数据，而且需要把mTrans.y设置成oldoffset
    /// </returns>
    public int CheckPos()
    {
        if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
        {
            if (oldoffset > 0)
            {
                if (thisGrid.GetBeginIndex() == 0)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (oldoffset < 0)
            {
                int maxCount = GetDataCount();
                int perline = thisGrid.PerLineCount;
                var columnCount = (maxCount + perline - 1) / perline;
                if (oldoffset < -cellLenth * columnCount + mPanel.GetViewSize().x)
                {
                    //Logger.Info("MoveRelative oldoffset={0},maxCount={1}", oldoffset, maxCount);
                    var endIndex = thisGrid.GetEndIndex();
                    if (endIndex == maxCount - 1)
                    {
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else if (movement == UIScrollViewSimple.Movement.Vertical) //竖向移动
        {
            if (oldoffset < 0)
            {
                if (thisGrid.GetBeginIndex() == 0)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (oldoffset > 0)
            {
                int maxCount = GetDataCount();
                int perline = thisGrid.PerLineCount;
                var rowCount = (maxCount + perline - 1) / perline;
                if (oldoffset > cellLenth * rowCount - mPanel.GetViewSize().y)
                {
                    //Logger.Info("MoveRelative oldoffset={0},maxCount={1}", oldoffset, maxCount);
                    var endIndex = thisGrid.GetEndIndex();
                    if (endIndex == maxCount - 1)
                    {
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }
        return 0;
    }
    public int Modifyoffset(ref float offset)
    {
        oldoffset += offset;
        return CheckPos();
    }

    public void MoveToOffset(Vector3 pos ,float offset)
    {
        if (!mTrans)
            mTrans = transform;
        mTrans.localPosition = Vector3.zero;
        SetScrollViewPostionOffset(pos);
        if (offset < 0.0f || thisGrid.GetDataCount() <= thisGrid.DragCount + 1)
        {
            offset = 0.0f;
        }
        var maxLength = thisGrid.GetMaxLength();
        if (offset > maxLength)
        {
            offset = maxLength;
        }
        oldoffset = offset;
        ResetObj();
        RestrictWithinBounds(true);
        UpdateScrollbars();
    }

    public void MoveToOffset2(Vector3 pos, float offset)
    {
        if (!mTrans)
            mTrans = transform;
        mTrans.localPosition = Vector3.zero;
        SetScrollViewPostionOffset(pos);
        oldoffset = offset;
        ResetObj();
        RestrictWithinBounds(true);
        UpdateScrollbars();
    }

    //直接设置看第几个
    public void SetLookIndex(int index, bool moveing = false)
    {
        if (!mTrans)
            mTrans = transform;
        mTrans.localPosition = Vector3.zero;
        oldoffset = 0.0f;
        //SetScrollViewPostionOffset(Vector3.zero);
        ResetScrollViewPostionOffset();
        Vector3 relative = new Vector3() { };
        if (movement == Movement.Horizontal) //横向移动
        {
            float willPos = -index * cellLenth;
            if (moveing)
            {
                relative.x = willPos - oldoffset;
                SpringPanelMoveBy.Begin(mPanel.gameObject, relative, 13f).strength = 8f;
            }
            else
            {
                oldoffset = willPos;
                ResetObj();
                UpdateScrollbars();
            }
        }
        else if (movement == Movement.Vertical) //竖向移动
        {
            int maxdata = GetDataCount();
            int gridCount = thisGrid.GetGridCount();
            if (index + gridCount - 1 > maxdata)
            {
                index = maxdata - gridCount + 2;
            }
            float willPos = index * cellLenth;
            if (moveing)
            {
                relative.y = willPos - oldoffset;
                SpringPanelMoveBy.Begin(mPanel.gameObject, relative, 13f).strength = 8f;
            }
            else
            {
                oldoffset = willPos;
                ResetObj();
                UpdateScrollbars();
            }
        }
    }
    //向前看一个
    public bool SetLookBefore(bool moveing = false)
    {
        Vector3 relative = new Vector3() {};
        if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
        {
            if (oldoffset > 0 || Mathf.Abs(oldoffset) < 0.5f)
            {
                return false;
            }
            relative.x = cellLenth + (-oldoffset) % cellLenth;
            if (oldoffset + relative.x >= 0)
            {
                relative.x = -oldoffset;
            }

            if (relative.x > cellLenth * 2 - 3)
            {
                relative.x -= cellLenth;
            }
            if (moveing)
            {
                SpringPanelMoveBy.Begin(mPanel.gameObject, relative, 13f).strength = 8f;
            }
            else
            {
                MoveRelative(relative, moveing);
            }
            //MoveRelative(relative, moveing);
            //relative.x = Mathf.Round(relative.x) + mTrans.localPosition.x + 0.1f;
        }
        else if (movement == UIScrollViewSimple.Movement.Vertical)//竖向移动
        {
            if (oldoffset <= 0 || Mathf.Abs(oldoffset) < 0.05f)
            {
                return false; 
            }
            relative.y = -cellLenth;
            MoveRelative(relative, moveing);
        }
        return true;
    }

    //向后看一个
    public bool SetLookAfter(bool moveing = false)
    {
        Vector3 relative = new Vector3() { };
        if (movement == UIScrollViewSimple.Movement.Horizontal)//横向移动
        {
            int maxCount = GetDataCount();
            int perline = thisGrid.maxPerLine;
            if (perline < 1) perline = 1;
            maxCount = (maxCount + perline - 1) / perline;
            if (oldoffset + 5 <= -cellLenth * (maxCount-1) + mPanel.GetViewSize().x)
            {
                return false;
            }
            relative.x = -cellLenth * 2 + (-oldoffset) % cellLenth;

            if (oldoffset + relative.x <= -cellLenth * (maxCount - 1) + mPanel.GetViewSize().x)
            {
                relative.x = -cellLenth*maxCount + mPanel.GetViewSize().x - oldoffset;
            }
            if (relative.x < -cellLenth * 2 + 3)
            {
                relative.x += cellLenth;
            }
            if (moveing)
            {
                SpringPanelMoveBy.Begin(mPanel.gameObject, relative, 13f).strength = 8f;
            }
            else
            {
                MoveRelative(relative, moveing);
            }
        }
        else if (movement == UIScrollViewSimple.Movement.Vertical)//竖向移动
        {
            int maxCount = GetDataCount();
            int perline = thisGrid.maxPerLine;
            if (perline < 1) perline = 1;
            maxCount = (maxCount + perline - 1) / perline;
            if (oldoffset >= cellLenth*maxCount - mPanel.GetViewSize().y)
            {
                return false;;
            }
            relative.y = cellLenth;
            MoveRelative(relative, moveing);
            //ischange = Modifyoffset(ref relative.y);
        }
        return true;
    }
    public int GetDataCount()
    {
        if (thisGrid != null)
        {
            return thisGrid.GetDataCount();
        }
        return 0;
    }
    public int GetMaxLine()
    {
        int perline = thisGrid.maxPerLine;
        if (perline < 1) perline = 1;
        int maxCount = GetDataCount();
        int LastCount = (maxCount + perline - 1) / perline;
        return LastCount;
    }
	/// <summary>
	/// Move the scroll view by the specified world space amount.
	/// </summary>

	public void MoveAbsolute (Vector3 absolute)
	{
        //Logger.Info("MoveAbsolute={0},{1},{2}", absolute.x, absolute.y, absolute.z);
		Vector3 a = mTrans.InverseTransformPoint(absolute);
        Vector3 b = mTrans.InverseTransformPoint(Vector3.zero);
        //Logger.Info("MoveAbsolute a={0},{1},{2}", a.x, a.y, a.z);
        //Logger.Info("MoveAbsolute b={0},{1},{2}", b.x, b.y, b.z);
		MoveRelative(a - b);
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	public void Press (bool pressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

		if (smoothDragStart && pressed)
		{
			mDragStarted = false;
			mDragStartOffset = Vector2.zero;
		}

		if (enabled && NGUITools.GetActive(gameObject))
		{
			if (!pressed && mDragID == UICamera.currentTouchID) mDragID = -10;

			mCalculatedBounds = false;
			mShouldMove = shouldMove;
			if (!mShouldMove) return;
			mPressed = pressed;

			if (pressed)
			{
				// Remove all momentum on press
				mMomentum = Vector3.zero;
				mScroll = 0f;

				// Disable the spring movement
				DisableSpring();

// 				// Remember the hit position
 				mLastPos = UICamera.lastWorldPosition;

				// Create the plane to drag along
				mPlane = new Plane(mTrans.rotation * Vector3.back, mLastPos);

				// Ensure that we're working with whole numbers, keeping everything pixel-perfect
				Vector2 co = mPanel.clipOffset;
				co.x = Mathf.Round(co.x);
				co.y = Mathf.Round(co.y);
				mPanel.clipOffset = co;

				if (!smoothDragStart)
				{
					mDragStarted = true;
					mDragStartOffset = Vector2.zero;
					if (onDragStarted != null) onDragStarted();
				}
			}
			else if (centerOnChild)
			{
				centerOnChild.Recenter();
			}
			else
			{
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
					RestrictWithinBounds(dragEffect == DragEffect.None, canMoveHorizontally, canMoveVertically);

				if (mDragStarted && onDragFinished != null) onDragFinished();
				if (!mShouldMove && onStoppedMoving != null)
					onStoppedMoving();
			}
		}
	}

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	public void Drag ()
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

		if (enabled && NGUITools.GetActive(gameObject) && mShouldMove)
		{
			if (mDragID == -10) mDragID = UICamera.currentTouchID;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

			// Prevents the drag "jump". Contributed by 'mixd' from the Tasharen forums.
			if (smoothDragStart && !mDragStarted)
			{
				mDragStarted = true;
				mDragStartOffset = UICamera.currentTouch.totalDelta;
				if (onDragStarted != null) onDragStarted();
			}

			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);

			float dist = 0f;

			if (mPlane.Raycast(ray, out dist))
			{
                Vector3 currentPos = ray.GetPoint(dist);
                //Logger.Info("Drag mLastPos={0},{1},{2}", mLastPos.x, mLastPos.y, mLastPos.z);
                //Logger.Info("Drag currentPos={0},{1},{2}", currentPos.x, currentPos.y, currentPos.z);
                Vector3 offset = currentPos - mLastPos;
                

                //Logger.Info("Drag offset={0},{1},{2}", offset.x, offset.y, offset.z);
				mLastPos = currentPos;

				if (offset.x != 0f || offset.y != 0f || offset.z != 0f)
				{
					offset = mTrans.InverseTransformDirection(offset);

					if (movement == Movement.Horizontal)
					{
						offset.y = 0f;
						offset.z = 0f;
					}
					else if (movement == Movement.Vertical)
					{
						offset.x = 0f;
						offset.z = 0f;
					}
					else if (movement == Movement.Unrestricted)
					{
						offset.z = 0f;
					}
					else
					{
						offset.Scale((Vector3)customMovement);
					}
                    offset = mTrans.TransformDirection(offset);
                    //Logger.Info("Drag TransformDirection offset={0},{1},{2}", offset.x, offset.y, offset.z);
				}

                //Logger.Info("Drag iOSDragEmulation={0},dragEffect={1},dragEffect={2}", iOSDragEmulation, dragEffect, dragEffect);
				// Adjust the momentum
			    if (dragEffect == DragEffect.None) mMomentum = Vector3.zero;
			    else mMomentum = Vector3.Max(Vector3.Min(Vector3.Lerp(mMomentum, mMomentum + offset*(0.01f*momentumAmount), 0.67f),
			            maxMomentumAmount), -maxMomentumAmount);

				// Move the scroll view
				if (!iOSDragEmulation || dragEffect != DragEffect.MomentumAndSpring)
				{
					MoveAbsolute(offset);
				}
				else
				{
				    var ret = CalculateBoundary();
				    if (ret == 3)
				    {
                        MoveAbsolute(offset);
				    }
				    else
				    {
                        MoveAbsolute(offset * 0.5f);
                        mMomentum *= 0.5f;
				    }
// 					Vector3 constraint = mPanel.CalculateConstrainOffset(bounds.min, bounds.max);
// 					if (constraint.magnitude > 1f)
// 					{
// 						MoveAbsolute(offset * 0.5f);
// 						mMomentum *= 0.5f;
// 					}
// 					else
// 					{
// 						MoveAbsolute(offset);
// 					}
				}

				// We want to constrain the UI to be within bounds
				if (restrictWithinPanel &&
					mPanel.clipping != UIDrawCall.Clipping.None &&
					dragEffect != DragEffect.MomentumAndSpring)
				{
					RestrictWithinBounds(true, canMoveHorizontally, canMoveVertically);
				}
			}
		}
	}

	[HideInInspector]
	public UICenterOnChildSimple centerOnChild = null;

	/// <summary>
	/// If the object should support the scroll wheel, do it.
	/// </summary>

	public void Scroll (float delta)
	{
		if (enabled && NGUITools.GetActive(gameObject) && scrollWheelFactor != 0f)
		{
			DisableSpring();
			mShouldMove |= shouldMove;
			if (Mathf.Sign(mScroll) != Mathf.Sign(delta)) mScroll = 0f;
			mScroll += delta * scrollWheelFactor;
		}
	}

	/// <summary>
	/// Apply the dragging momentum.
	/// </summary>

	void LateUpdate ()
	{
		if (!Application.isPlaying) return;
		float delta = RealTime.deltaTime;

		// Fade the scroll bars if needed
		if (showScrollBars != ShowCondition.Always && (verticalScrollBar || horizontalScrollBar))
		{
			bool vertical = false;
			bool horizontal = false;

			if (showScrollBars != ShowCondition.WhenDragging || mDragID != -10 || mMomentum.magnitude > 0.01f)
			{
				vertical = shouldMoveVertically;
				horizontal = shouldMoveHorizontally;
			}

			if (verticalScrollBar)
			{
				float alpha = verticalScrollBar.alpha;
				alpha += vertical ? delta * 6f : -delta * 3f;
				alpha = Mathf.Clamp01(alpha);
				if (verticalScrollBar.alpha != alpha) verticalScrollBar.alpha = alpha;
			}

			if (horizontalScrollBar)
			{
				float alpha = horizontalScrollBar.alpha;
				alpha += horizontal ? delta * 6f : -delta * 3f;
				alpha = Mathf.Clamp01(alpha);
				if (horizontalScrollBar.alpha != alpha) horizontalScrollBar.alpha = alpha;
			}
		}
// 
//         if (mRestrict)
//         {
//             var ret = RestrictWithinBounds(true);
//             if (ret == false)
//             {
//                 var pos = Vector3.zero;
//                 if (movement == Movement.Horizontal)
//                 {
//                     pos.x = -mPanel.clipSoftness.x / 2.0f;
//                 }
//                 else if (movement == Movement.Vertical)
//                 {
//                     pos.y = mPanel.clipSoftness.y / 2.0f;
//                 }
//                 if (pos != Vector3.zero)
//                 {
//                     MoveRelative(pos, false);
//                 }                
//             }
//             mRestrict = false;
//         }

		if (!mShouldMove) return;

		// Apply momentum
		if (!mPressed)
		{
			if (mMomentum.magnitude > 0.0001f || mScroll != 0f)
			{
				if (movement == Movement.Horizontal)
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(mScroll * 0.05f, 0f, 0f));
				}
				else if (movement == Movement.Vertical)
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(0f, mScroll * 0.05f, 0f));
				}
				else if (movement == Movement.Unrestricted)
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(mScroll * 0.05f, mScroll * 0.05f, 0f));
				}
				else
				{
					mMomentum -= mTrans.TransformDirection(new Vector3(
						mScroll * customMovement.x * 0.05f,
						mScroll * customMovement.y * 0.05f, 0f));
				}
				mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, delta);

				// Move the scroll view
				Vector3 offset = NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
				MoveAbsolute(offset);

				// Restrict the contents to be within the scroll view's bounds
				if (restrictWithinPanel && mPanel.clipping != UIDrawCall.Clipping.None)
				{
					if (NGUITools.GetActive(centerOnChild))
					{
						if (centerOnChild.nextPageThreshold != 0f)
						{
							mMomentum = Vector3.zero;
							mScroll = 0f;
						}
						else centerOnChild.Recenter();
					}
					else
					{
						RestrictWithinBounds(false, canMoveHorizontally, canMoveVertically);
					}
				}

				if (onMomentumMove != null)
					onMomentumMove();
			}
			else
			{
				mScroll = 0f;
				mMomentum = Vector3.zero;

				SpringPanel sp = GetComponent<SpringPanel>();
				if (sp != null && sp.enabled) return;

				mShouldMove = false;
				if (onStoppedMoving != null)
					onStoppedMoving();
			}
		}
		else
		{
			// Dampen the momentum
			mScroll = 0f;
			NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
		}
	}

#if UNITY_EDITOR

	/// <summary>
	/// Draw a visible orange outline of the bounds.
	/// </summary>

	void OnDrawGizmos ()
	{
		if (mPanel != null)
		{
			if (!Application.isPlaying) mCalculatedBounds = false;
			Bounds b = bounds;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = new Color(1f, 0.4f, 0f);
			Gizmos.DrawWireCube(new Vector3(b.center.x, b.center.y, b.min.z), new Vector3(b.size.x, b.size.y, 0f));
		}
	}
#endif
}
