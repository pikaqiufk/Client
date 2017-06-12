using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class OtherStallCell : MonoBehaviour
	{
	    // Use this for initialization
	    public ListItemLogic ItemLogic;
	
	    public void OnBtnBuy()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(13, ItemLogic.Index));
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
	}
}