using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class ZhanXinCell : MonoBehaviour
	{
	    private ListItemLogic listItemLogic;
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_AstrologyBagItemClick(listItemLogic.Index));
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        listItemLogic = gameObject.GetComponent<ListItemLogic>();
	
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