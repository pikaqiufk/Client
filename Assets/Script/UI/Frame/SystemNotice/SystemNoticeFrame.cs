using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SystemNoticeFrame : MonoBehaviour
	{
	    public UILabel Content;
	    public int Length = 190;
	    private bool mIsActive;
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(SystemNoticeNotify.EVENT_TYPE, OnNotifySystemNotice);
	
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
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (!EventDispatcher.Instance.HasEventListener(SystemNoticeNotify.EVENT_TYPE, OnNotifySystemNotice))
	        {
	            EventDispatcher.Instance.AddEventListener(SystemNoticeNotify.EVENT_TYPE, OnNotifySystemNotice);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnNotifySystemNotice(IEvent ievent)
	    {
	        var e = ievent as SystemNoticeNotify;
	        ShowNoticeInfo(e.Content);
	    }
	
	    private void ShowNoticeInfo(string content)
	    {
	        if (string.IsNullOrEmpty(content))
	        {
	            mIsActive = false;
	            var e = new Close_UI_Event(UIConfig.SystemNoticeFrame);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        else
	        {
	            Content.text = content;
	            Content.transform.localPosition = new Vector3(Length, 0, 0);
	            mIsActive = true;
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mIsActive == false)
	        {
	            return;
	        }
	
	        var oldLoc = Content.transform.localPosition;
	        if (oldLoc.x < -Length - Content.width)
	        {
	            mIsActive = false;
	            var e = new SystemNoticeOperate(0);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        else
	        {
	            oldLoc.x -= Time.deltaTime*GameUtils.SystemNoticeScrollingSpeed;
	            Content.transform.localPosition = oldLoc;
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