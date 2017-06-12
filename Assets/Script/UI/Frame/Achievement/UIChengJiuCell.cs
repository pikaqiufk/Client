using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class UIChengJiuCell : MonoBehaviour
	{
	    public void OnBtnClick()
	    {
	        var item = GetComponent<ListItemLogic>();
	        if (null != item)
	        {
	            var dateModel = item.Item as AchievementItemDataModel;
	            if (null != dateModel)
	            {
	                EventDispatcher.Instance.DispatchEvent(new UI_EventApplyChengJiuItem(dateModel.Id));
	                //AchievementManager.Instance.ClaimAchievementReward(dateModel.Id);
	            }
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
	}
}