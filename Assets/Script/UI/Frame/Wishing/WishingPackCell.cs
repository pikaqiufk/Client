using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WishingPackCell : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as BagItemDataModel;
	
	            if (itemData.ItemId != -1)
	            {
	                var e = new UIEvent_WishingInfoItem();
	                e.item = itemData;
	                e.Type = 1;
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	        }
	    }
	
	    public void ItemPress()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as BagItemDataModel;
	            if (itemData.ItemId != -1)
	            {
	                var e = new UIEvent_WishingItemOperation();
	                e.Index = listItemLogic.Index;
	                e.Operation = 0;
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	        }
	    }
	
	    public void ItemRelease()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as BagItemDataModel;
	
	            if (itemData.ItemId != -1)
	            {
	                var e = new UIEvent_WishingItemOperation();
	                e.Index = listItemLogic.Index;
	                e.Operation = 1;
	                EventDispatcher.Instance.DispatchEvent(e);
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