using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ActivityRewardCell : MonoBehaviour
	{
	    private ListItemLogic listCell;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        listCell = GetComponent<ListItemLogic>();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClick()
	    {
	        if (null != listCell)
	        {
	            var dataModel = listCell.Item as ActivityRewardItemDataModel;
	            if (null != dataModel)
	            {
	                EventDispatcher.Instance.DispatchEvent(new UIEvent_CliamReward(UIEvent_CliamReward.Type.Activity,
	                    dataModel.Id));
	            }
	        }
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