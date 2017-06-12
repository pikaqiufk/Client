using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TitleUIFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	
	    public void Close()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.TitleUI));
	    }
	
	    public void CloseTip()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TitleItemOption(2, 0));
	    }
	
	    public void OnBackBtn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TitleItemOption(3, 0));
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
	
	    
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	}
	
	    public void OnFrontBtn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TitleItemOption(4, 0));
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.TitleUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	    
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