using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class UIChengJiuZhaiYaoButton : MonoBehaviour
	{
	    public void OnClick()
	    {
	        var dateModel = GetComponent<ListItemLogic>().Item as AchievementSummaryBtnDataModel;
	        EventDispatcher.Instance.DispatchEvent(new Event_ShowAchievementPage(dateModel.TypeId));
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