#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SkillDragItem : UIDragDropItem
	{
	    private int skillID;
	
	    public int SkillID
	    {
	        get { return skillID; }
	        set
	        {
	            if (skillID == value)
	            {
	                return;
	            }
	            skillID = value;
	        }
	    }
	
	    protected override void OnDragDropRelease(GameObject surface)
	    {
	        var targetObj = surface.GetComponent<SkillDragEquip>();
	        if (targetObj)
	        {
	            var ee = new UIEvent_SkillFrame_EquipSkill(targetObj.itemLogic.Index, SkillID);
	            EventDispatcher.Instance.DispatchEvent(ee);
	        }
	        NGUITools.Destroy(gameObject);
	    }
	
	    protected override void StartDragging()
	    {
	        if (PlayerDataManager.Instance.GetSkillLevel(skillID) < 1)
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
	            var clone = NGUITools.AddChild(mTrans.parent.gameObject, gameObject);
	            var t = clone.transform;
	            t.localPosition = mTrans.localPosition;
	            t.localRotation = mTrans.localRotation;
	            t.localScale = mTrans.localScale;
	
	            var sp = clone.GetComponent<UISprite>();
	            sp.SetRect(-45, -45, 90, 90);
	
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
	
	            var item = clone.GetComponent<SkillDragItem>();
	            item.SkillID = SkillID;
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
	    }
	}
}