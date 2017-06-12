using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class LevelUpShowFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	
	    public void Close()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.LevelUpTip));
	    }
	
	    // Update is called once per frame
	    public void GotoAttrUI()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.LevelUpTip, false));
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.AttributeUI));
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        //todo
	        var controllerBase = UIManager.Instance.GetController(UIConfig.LevelUpTip);
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