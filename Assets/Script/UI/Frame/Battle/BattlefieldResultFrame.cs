using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class BattlefieldResultFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.BattleResult);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOk()
	    {
	        var e = new BattleResultClick();
	        EventDispatcher.Instance.DispatchEvent(e);
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.BattleResult);
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