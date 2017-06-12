using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class GroundMenuDragDropItem : UIDragDropItem
	{
	    public UILabel CountLabel;
	    public ListItemLogic ItemLogic;
	    private Transform memTransform;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        memTransform = transform;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(FramDragRefreshCount.EVENT_TYPE, OnRefreshFramDragCount);
	        // EventDispatcher.Instance.RemoveEventListener(FarmMenuCountRefresh.EVENT_TYPE, OnFarmMenuCuontRefresh);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    protected override void OnDragDropRelease(GameObject surface)
	    {
	        if (surface != null)
	        {
	            var e = new FarmMenuDragEvent(-1);
	            EventDispatcher.Instance.DispatchEvent(e);
	            NGUITools.Destroy(gameObject);
	        }
	    }
	
	    //private void OnFarmMenuCuontRefresh(IEvent ievent)
	    //{
	    //    FarmMenuCountRefresh e = ievent as FarmMenuCountRefresh;
	    //    if (e.Count <= 3)
	    //    {
	    //        this.restriction = Restriction.None;
	    //    }
	    //    else
	    //    {
	    //        this.restriction = Restriction.LeftTopRightButtom;
	    //    }
	    //}
	
	    private void OnRefreshFramDragCount(IEvent ievent)
	    {
	        var e = ievent as FramDragRefreshCount;
	
	        //Logger.Info("GameObj e.Index = {0},ItemLogic.Index = {1}", gameObject.name, ItemLogic.Index);
	        //if (e.Index != ItemLogic.Index)
	        //{
	        //    return;
	        //}
	        if (CountLabel != null)
	        {
	            CountLabel.text = e.Count.ToString();
	        }
	
	        Logger.Info("GameObj Name = {0},Count = {1}", gameObject.name, e.Count);
	    }
	
	    protected override void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        base.Start();
	        EventDispatcher.Instance.AddEventListener(FramDragRefreshCount.EVENT_TYPE, OnRefreshFramDragCount);
	        //EventDispatcher.Instance.AddEventListener(FarmMenuCountRefresh.EVENT_TYPE, OnFarmMenuCuontRefresh);
	
	        //var gridParent = mTransform.parent.parent.GetComponent<UIGridSimple>();
	        //if (gridParent != null)
	        //{
	        //    if (gridParent.GetDataCount() <= 3)
	        //    {
	        //        this.restriction = Restriction.None;
	        //    }
	        //    else
	        //    {
	        //        this.restriction = Restriction.LeftTopRightButtom;
	        //    }
	        //}
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    protected override void StartDragging()
	    {
	        if (!interactable)
	        {
	            return;
	        }
	
	        if (!mDragging)
	        {
	            if (cloneOnDrag)
	            {
	                mPressed = false;
	                var clone = NGUITools.AddChild(memTransform.parent.gameObject, gameObject);
	                var cloneTrans = clone.transform;
	                cloneTrans.localPosition = memTransform.localPosition;
	                cloneTrans.localRotation = memTransform.localRotation;
	                cloneTrans.localScale = memTransform.localScale;
	
	                var bc = clone.GetComponent<UIButtonColor>();
	                if (bc != null)
	                {
	                    bc.defaultColor = GetComponent<UIButtonColor>().defaultColor;
	                }
	
	                if (mTouch != null && mTouch.pressed == gameObject)
	                {
	                    mTouch.current = clone;
	                    mTouch.pressed = clone;
	                    mTouch.dragged = clone;
	                    mTouch.last = clone;
	                }
	
	                var item = clone.GetComponent<GroundMenuDragDropItem>();
	
	                item.CountLabel = item.GetComponentInChildren<UILabel>();
	                item.ItemLogic = ItemLogic;
	                item.mTouch = mTouch;
	                item.mPressed = true;
	                item.mDragging = true;
	                item.Start();
	                item.OnDragDropStart();
	
	                var boxCollider = clone.GetComponent<UIDragScrollViewSimple>();
	                if (boxCollider != null)
	                {
	                    boxCollider.enabled = false;
	                }
	
	                if (UICamera.currentTouch == null)
	                {
	                    UICamera.currentTouch = mTouch;
	                }
	
	                mTouch = null;
	
	                UICamera.Notify(gameObject, "OnPress", false);
	                UICamera.Notify(gameObject, "OnHover", false);
	
	                var e = new FarmMenuDragEvent(ItemLogic.Index);
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	            else
	            {
	                mDragging = true;
	                OnDragDropStart();
	            }
	        }
	    }
	}
}