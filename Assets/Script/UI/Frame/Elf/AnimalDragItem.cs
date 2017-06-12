using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class AnimalDragItem : UIDragDropItem
	{
	    public GameObject Ico;
	    public AnimalItemCell ItemCell;
	    private Transform mTransform;
	    public int Type;
	
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
	
	    private bool CheckEnd(AnimalDragItem targetObj)
	    {
	        if (targetObj.Type == 1)
	        {
	            return false;
	        }
	        return true;
	    }
	
	    private bool CheckStart()
	    {
	        var data = (ElfItemDataModel) ItemCell.ListItem.Item;
	        if (data == null)
	        {
	            return false;
	        }
	        if (data.ItemId == -1)
	        {
	            return false;
	        }
	        return true;
	    }
	
	    protected override void OnDragDropRelease(GameObject surface)
	    {
	        var targetObj = surface.GetComponent<AnimalDragItem>();
	        if (targetObj)
	        {
	            if (CheckEnd(targetObj))
	            {
	                var f = ItemCell.ItemData.Index;
	                var t = targetObj.ItemCell.ItemData.Index;
	                var e = new ElfReplaceEvent(f, t);
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	        }
	        NGUITools.Destroy(gameObject);
	    }
	
	    protected override void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        base.Start();
	
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
	        if (!CheckStart())
	        {
	            return;
	        }
	        if (!interactable)
	        {
	            return;
	        }
	
	        if (!mDragging)
	        {
	            mPressed = false;
	            var clone = NGUITools.AddChild(mTransform.parent.gameObject, Ico);
	            var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	            var localPos = mTransform.parent.InverseTransformPoint(worldPos);
	
	            var cloneTrans = clone.transform;
	            cloneTrans.localPosition = localPos;
	            cloneTrans.localRotation = mTransform.localRotation;
	            cloneTrans.localScale = mTransform.localScale;
	
	            if (mTouch != null && mTouch.pressed == gameObject)
	            {
	                mTouch.current = clone;
	                mTouch.pressed = clone;
	                mTouch.dragged = clone;
	                mTouch.last = clone;
	            }
	
	            AnimalDragItem item = null;
	            item = clone.GetComponent<AnimalDragItem>() ?? clone.AddComponent<AnimalDragItem>();
	            item.mTouch = mTouch;
	            if (item.ItemCell == null)
	            {
	                item.ItemCell = item.gameObject.AddComponent<AnimalItemCell>();
	            }
	            if (ItemCell.ItemData == null)
	            {
	                item.ItemCell.ItemData = ItemCell.ListItem.Item as ElfItemDataModel;
	            }
	            else
	            {
	                item.ItemCell.ItemData = ItemCell.ItemData;
	            }
	
	            item.mPressed = true;
	            item.mDragging = true;
	            item.Start();
	            item.OnDragDropStart();
	
	            if (UICamera.currentTouch == null)
	            {
	                UICamera.currentTouch = mTouch;
	            }
	
	            mTouch = null;
	
	
	            UICamera.Notify(gameObject, "OnPress", false);
	            UICamera.Notify(gameObject, "OnHover", false);
	        }
	    }
	}
}