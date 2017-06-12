using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TaskHintFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    public MissionTipDataModel DataModel;
	
	    public void OnCloseClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionTip));
	    }
	
	    // Update is called once per frame
	    private void OnDestroy()
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MissionTip);
	        DataModel = controllerBase.GetDataModel("") as MissionTipDataModel;
	        BindRoot.SetBindDataSource(DataModel);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnShowMeClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_MissionTipEvent());
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionTip));
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