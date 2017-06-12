using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BagPackCellForTradingUI : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void ItemClick()
	    {
	        var item = listItemLogic.Item as BagItemDataModel;
	        if (item.ItemId != -1)
	        {
	            var e = new UIEvent_TradingBagItemClick(item);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        else
	        {
	            if (item.Status == (int) eBagItemType.Lock)
	            {
	                var e = new PackUnlockEvent(item);
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	        }
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        //if (null != listItemLogic)
	        //{
	        //    if (listItemLogic.Index != 0)
	        //    {
	        //        return;
	        //    }
	
	        //    if (listItemLogic.Item == null)
	        //    {
	        //        var item = new BagItemDataModel();
	        //        UIEvent_TradingBagItemClick e = new UIEvent_TradingBagItemClick(item);
	        //        EventDispatcher.Instance.DispatchEvent(e);
	        //    }
	        //    else
	        //    {
	        //        ItemClick();
	        //    }
	
	        //}
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (null != listItemLogic && listItemLogic.Item != null)
	        {
	            if (listItemLogic.Index == 0)
	            {
	                ItemClick();
	            }
	        }
	
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