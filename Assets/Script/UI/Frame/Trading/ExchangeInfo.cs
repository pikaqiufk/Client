using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class ExchangeInfo : MonoBehaviour
	{
	    public void OnBtnBack()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(19));
	    }
	
	    public void OnBtnExchange()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_TradingFrameButton(18));
	    }
	
	    // Use this for initialization
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