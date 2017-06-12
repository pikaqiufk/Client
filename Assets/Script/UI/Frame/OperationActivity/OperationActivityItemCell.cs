using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class OperationActivityItemCell : MonoBehaviour
	{
	    public void OnClaimClick()
	    {
			var dataModel = GetComponent<ListItemLogic>().Item as OperationActivityItemDataModel;
	        if (null != dataModel)
	        {
				EventDispatcher.Instance.DispatchEvent(new OperationActivityClaimReward(dataModel.ItemId));
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