using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class WishingGoodsCell : MonoBehaviour
	{
	    private ListItemLogic listItemLogic;
	
	    public void ItemBuyClick()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as WishingGoodsDataModel;
	            var e = new UIEvent_WishingGoodsItemBuy();
	            e.item = itemData;
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as WishingGoodsDataModel;
	
	            if (itemData.bagItem.ItemId != -1)
	            {
	                var e = new UIEvent_WishingInfoItem();
	                e.item = itemData.bagItem;
	                e.Type = 1;
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
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