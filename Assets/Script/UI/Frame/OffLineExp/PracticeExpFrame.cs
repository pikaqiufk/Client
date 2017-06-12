using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PracticeExpFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    //public void OnCloseBtnClick()
	    //{
	    //    EventDispatcher.Instance.DispatchEvent(new Ui_OffLineFrame_SetVisible(false));
	    //}
	    public void OnBeginBtnClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UI_Event_OffLineExp(3));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        //EventDispatcher.Instance.DispatchEvent(new Ui_OffLineFrame_SetVisible(false));
	        Binding.RemoveBinding();
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnDoubleBtnClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UI_Event_OffLineExp(1));
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.OffLineExpFrame);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	        //EventDispatcher.Instance.DispatchEvent(new Ui_OffLineFrame_SetVisible(true));
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnNormalBtnClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UI_Event_OffLineExp(0));
	    }
	
	    public void OnQuadruplingBtnClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UI_Event_OffLineExp(2));
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