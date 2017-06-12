using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MailFame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private bool delBind = true;
	    public UIToggle Toggle;
	
	    public void DelectAll()
	    {
	        var e = new MailOperactionEvent(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MailUI));
	    }
	
	    public void OnClickCheck()
	    {
	        if (Toggle.value)
	        {
	            var e = new MailOperactionEvent(4);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        else
	        {
	            var e = new MailOperactionEvent(5);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.MailUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            delBind = false;
	        }
	        else
	        {
	            if (delBind == false)
	            {
	                RemoveListener();
	            }
	            delBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (delBind == false)
	        {
	            RemoveListener();
	        }
	        delBind = true;
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
	        if (delBind)
	        {
	            RemoveListener();
	        }
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
	        if (delBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	
	            var controllerBase = UIManager.Instance.GetController(UIConfig.MailUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        }
	        delBind = true;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void RecieveAll()
	    {
	        var e = new MailOperactionEvent(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void RecieveAwardItem()
	    {
	        var e = new MailOperactionEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void RemoveListener()
	    {
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
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
	}
}