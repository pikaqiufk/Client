using System;
#region using

using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class StoreCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickBuyBtn()
	    {
	        var e = new StoreCellClick(ItemLogic.Item as StoreCellData);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickItemIcon()
	    {
	        var cell = ItemLogic.Item as StoreCellData;
	        if (cell == null)
	        {
	            return;
	        }
	        var storeId = cell.StoreIndex;
	        var tbStore = Table.GetStore(storeId);
	        if (tbStore == null)
	        {
	            return;
	        }
	        var itemId = tbStore.ItemId;
	        var tbItem = Table.GetItemBase(itemId);
	        if (tbItem == null)
	        {
	            return;
	        }
	        if (tbStore.NeedItem == -1)
	        {
	            GameUtils.ShowItemIdTip(itemId);
	            return;
	        }
	
	        var bagItemData = new BagItemDataModel();
	        bagItemData.ItemId = itemId;
	
	        PlayerDataManager.Instance.ForeachEquip(equip =>
	        {
	            if (equip.ItemId == tbStore.NeedItem)
	            {
	                bagItemData.Exdata.InstallData(equip.Exdata);
	            }
	        });
	
	        if (bagItemData.Exdata.Count > 0)
	        {
	            GameUtils.ShowItemDataTip(bagItemData);
	        }
	        else
	        {
	            GameUtils.ShowItemIdTip(itemId, 1);
	        }
	    }
	
	    private void OnEnable()
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