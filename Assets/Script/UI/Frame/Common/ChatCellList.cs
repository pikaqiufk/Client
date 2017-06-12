using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ChatCellList : MonoBehaviour
	{
	    public BindDataRoot Binding;
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.ChatItemList);
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ChatItemList);
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
	}
}