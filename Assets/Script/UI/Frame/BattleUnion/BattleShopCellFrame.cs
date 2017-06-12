using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BattleShopCellFrame : MonoBehaviour
	{
	    // Use this for initialization
	    private ListItemLogic itemListLogic;
	
	    public void ItemClick()
	    {
	        if (itemListLogic != null)
	        {
	            var itemData = itemListLogic.Item as BattleUnionShopItemDataModel;
	
	            if (itemData.ItemID != -1)
	            {
	                var e = new UIEvent_BattleShopCellClick();
	                e.Index = itemListLogic.Index;
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
	        itemListLogic = gameObject.GetComponent<ListItemLogic>();
	
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