#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentDragDropItem1 : UIDragDropItem
	{
	    public GameObject Ico;
	    public int Index = -1;
	
	    protected override void OnDragDropRelease(GameObject surface)
	    {
	        var targetObj = surface.GetComponentInParent<EquipMentDragDropItem1>();
	        if (targetObj && targetObj != this)
	        {
	            EventDispatcher.Instance.DispatchEvent(new EquipCellSwap(Index, targetObj.Index));
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
	        var icon = mTrans.FindChild("Icon").GetComponent<UISprite>();
	        if (icon.spriteName == "equip_weapon")
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
	
	            var item = clone.GetComponent<EquipMentDragDropItem1>();
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