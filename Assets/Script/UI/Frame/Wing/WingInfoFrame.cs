using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WingInfoFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public BindDataRoot BindRoot;
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
	            Layout.ResetLayout();
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
	        var e = new Close_UI_Event(UIConfig.WingInfoUi);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        BindRoot.RemoveBinding();
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.WingInfoUi);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        BindRoot.SetBindDataSource(controllerBase.GetDataModel(""));
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
	
	    public void Listen<T>(T message)
	    {
	        mFlag = true;
	    }
	}
}