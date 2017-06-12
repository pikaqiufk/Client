using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PayCell : MonoBehaviour
	{
	    public ListItemLogic ListItem;
	
	    private void OnClick()
	    {
	        var datamodel = ListItem.Item as RechargeItemDataModel;
	        var e = new UIEvent_RechargeFrame_OnClick(0, datamodel.TableId);
	        EventDispatcher.Instance.DispatchEvent(e);
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