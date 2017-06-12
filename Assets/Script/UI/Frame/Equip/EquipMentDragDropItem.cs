#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentDragDropItem : UIDragDropItem
	{
	    public GameObject Ico;
	    public ListItemLogic ItemLogic;
	
	    protected override void OnDragDropRelease(GameObject surface)
	    {
	        var targetObj = surface.GetComponentInParent<EquipMentDragDropItem1>();
	        if (targetObj)
	        {
	            var data = ItemLogic.Item as EquipItemDataModel;
	            EventDispatcher.Instance.DispatchEvent(new EquipCellSelect(data.BagItemData, targetObj.Index));
	        }
	        NGUITools.Destroy(gameObject);
	    }
	
	    protected override void StartDragging()
	    {
	        if (!interactable)
	        {
	            return;
	        }
	        if (mDragging)
	        {
	            return;
	        }
	
	        if (cloneOnDrag)
	        {
	            mPressed = false;
	            var pressPos = mTouch.pressedCam.ScreenToWorldPoint(mTouch.pos);
	            var clone = NGUITools.AddChild(mTrans.parent.gameObject, Ico);
	            var t = clone.transform;
	            t.localPosition = mTrans.parent.InverseTransformPoint(pressPos);
	            t.localRotation = mTrans.localRotation;
	            t.localScale = mTrans.localScale;
	
	            if (mTouch != null && mTouch.pressed == gameObject)
	            {
	                mTouch.current = clone;
	                mTouch.pressed = clone;
	                mTouch.dragged = clone;
	                mTouch.last = clone;
	            }
	
	            var item = clone.GetComponent<EquipMentDragDropItem>();
	            item.mTouch = mTouch;
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
	        else
	        {
	            mDragging = true;
	            OnDragDropStart();
	        }
	    }
	}
}