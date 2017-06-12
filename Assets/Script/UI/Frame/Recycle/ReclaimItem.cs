using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ReclaimItem : MonoBehaviour
	{
	    private ListItemLogic listItemLogic;
	
	    public void ItemBackClick()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as BagItemDataModel;
	
	            if (itemData.ItemId != -1)
	            {
	                //PackItemClickEvent e = new PackItemClickEvent();
	                //e.BagId = itemData.BagId;
	                //e.Index = itemData.Index;
	                //EventDispatcher.Instance.DispatchEvent(e);
	
	                var ee = new UIEvent_RecycleItemSelect();
	                ee.type = 1;
	                ee.Item = itemData;
	                EventDispatcher.Instance.DispatchEvent(ee);
	            }
	        }
	    }
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as BagItemDataModel;
	
	            if (itemData.ItemId != -1)
	            {
	                //PackItemClickEvent e = new PackItemClickEvent();
	                //e.BagId = itemData.BagId;
	                //e.Index = listItemLogic.Index;
	                //EventDispatcher.Instance.DispatchEvent(e);
	                if (itemData.BagId == (int) eBagType.Equip)
	                {
	                    var ee = new UIEvent_RecycleItemSelect();
	                    ee.Item = itemData;
	                    ee.type = 0;
	                    EventDispatcher.Instance.DispatchEvent(ee);
	                }
	            }
	            else
	            {
	                if (itemData.Status == (int) eBagItemType.Lock)
	                {
	                    var e = new PackUnlockEvent(itemData);
	                    EventDispatcher.Instance.DispatchEvent(e);
	                }
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