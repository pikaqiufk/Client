using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class SelfStallCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnBtnAdd()
	    {
	        var e = new UIEvent_TradingFrameButton(0, ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnBtnCancel()
	    {
	        var e = new UIEvent_TradingFrameButton(9, ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnBtnHarvest()
	    {
	        var e = new UIEvent_TradingFrameButton(14, ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
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