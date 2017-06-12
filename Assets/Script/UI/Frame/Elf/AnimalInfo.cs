using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;


#endregion

namespace GameUI
{
	public class AnimalInfo : MonoBehaviour, IChainRoot, IChainListener
	{
	    public BindDataRoot Binding;
	    public StackLayout Layout;
	    private bool mFlag;
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mFlag)
	        {
	            if (Layout)
	            {
	                Layout.ResetLayout();
	            }
	            mFlag = false;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.ElfInfoUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickShowElf()
	    {
	        var e = new Show_UI_Event(UIConfig.ElfUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	        var e1 = new Close_UI_Event(UIConfig.ElfInfoUI);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        Binding.RemoveBinding();
	
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
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ElfInfoUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        mFlag = true;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
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
	
	    public void Listen<T>(T message)
	    {
	        mFlag = true;
	    }
	}
}