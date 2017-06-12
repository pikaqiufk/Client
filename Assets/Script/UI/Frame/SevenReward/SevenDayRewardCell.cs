using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SevenDayRewardCell : MonoBehaviour
	{
	    // Use this for initialization
	    private ListItemLogic itemList;
	
	    public void GetClick()
	    {
	        if (itemList != null)
	        {
	            var e = new UIEvent_SevenRewardItemClick(itemList.Index);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
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
	}
}