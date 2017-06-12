using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class GroupBookCellFrame : MonoBehaviour
	{
	    // Use this for initialization
	    private ListItemLogic itemList;
	
	    public void OnGroupBookToggleClick()
	    {
	        if (itemList == null)
	        {
	            return;
	        }
	        var e = new UIEvent_HandBookFrame_OnBookGroupToggled(itemList.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        itemList = gameObject.GetComponent<ListItemLogic>();
	
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