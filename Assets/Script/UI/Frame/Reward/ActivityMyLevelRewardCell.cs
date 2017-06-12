using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ActivityMyLevelRewardCell : MonoBehaviour
	{
	    public void OnClaimClick()
	    {
	        var dataModel = GetComponent<ListItemLogic>().Item as LevelRewardItemDataModel;
	        if (null != dataModel)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_CliamReward(UIEvent_CliamReward.Type.Level, dataModel.Id));
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