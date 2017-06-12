using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ItemGetDetailFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	
	    public void Close()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoGetUI));
	        //EventDispatcher.Instance.DispatchEvent(new Event_ItemInfoGetOperation(0));
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        //todo
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ItemInfoGetUI);
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