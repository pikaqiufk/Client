using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MainBufferCell : MonoBehaviour
	{
	    private MainBufferInfo buffer;
	    public ListItemLogic ListItem;
	    private bool canTouch;
	    private int curTouchID;
	
	    public void OnPress()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_BuffListBtn(1, ListItem.Index));
	        canTouch = true;
	        curTouchID = UICamera.currentTouchID;
	        if (null != buffer)
	        {
	            buffer.CalculateBackGroundHeight();
	        }
	    }
	
	    public void OnRelease()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_BuffListBtn(2, ListItem.Index));
	        canTouch = false;
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        buffer = transform.parent.parent.parent.parent.Find("Info").Find("BuffInfo").GetComponent<MainBufferInfo>();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    // Update is called once per frame
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (canTouch)
	        {
	            if (null != buffer)
	            {
	                var touch = UICamera.GetTouch(curTouchID);
	                if (null != touch)
	                {
	                    buffer.UpdatePostion(touch.pos);
	                }
	            }
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	}
}