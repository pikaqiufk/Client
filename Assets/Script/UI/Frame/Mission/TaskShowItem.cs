using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TaskShowItem : MonoBehaviour
	{
	    public void OnBtnClick()
	    {
	        var data = gameObject.GetComponent<ListItemLogic>().Item as MissionListItemSimpleDataModel;
	        if (data == null)
	        {
	            return;
	        }
	        //MissionManager.Instance.GoToMissionPlace(data.MissionId);
	        EventDispatcher.Instance.DispatchEvent(new Event_ShowMissionDataDetail(data.MissionId));
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        //GetComponent<BindDataRoot>().RemoveBinding();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnShowDetailBtnClick()
	    {
	        var item = gameObject.GetComponent<ListItemLogic>().Item;
	        var data = gameObject.GetComponent<ListItemLogic>().Item as MissionListItemSimpleDataModel;
	        if (data == null)
	        {
	            return;
	        }
	        EventDispatcher.Instance.DispatchEvent(new Event_ShowMissionDataDetail(data.MissionId));
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