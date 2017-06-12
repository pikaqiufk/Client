using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class ExchangeItem : MonoBehaviour
	{
	    // Use this for initialization
	    public ListItemLogic ListItem;
	
	    public void OnExchangeClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(17, ListItem.Index));
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