using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PayActivityInvestItem : MonoBehaviour
	{
	    public void OnGainClick()
	    {
	        var ListItem = GetComponent<ListItemLogic>();
	        if (null != ListItem)
	        {
	            EventDispatcher.Instance.DispatchEvent(new RechageActivityInvestmentOperation(0, ListItem.Index));
	        }
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