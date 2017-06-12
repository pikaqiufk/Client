using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class MonsterAwardFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private IControllerBase controller;
	
	    public void OnBtnConfirm()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.BossRewardFrame));
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        controller = UIManager.Instance.GetController(UIConfig.BossRewardFrame);
	        if (controller == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controller.GetDataModel(""));
	
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