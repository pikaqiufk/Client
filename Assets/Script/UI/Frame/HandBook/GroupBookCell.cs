using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class GroupBookCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnActiveClick()
	    {
	        var dataModel = ItemLogic.Item as OneGroupDataModel;
	        var e =
	            new UIEvent_HandBookFrame_OnGroupBookActive(dataModel.book,
	                ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnComposeClick()
	    {
	        var dataModel = ItemLogic.Item as OneGroupDataModel;
	        var e =
	            new UIEvent_HandBookFrame_OnBookClick(dataModel.book);
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