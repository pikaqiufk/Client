using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BattleOtherUnionFrame : MonoBehaviour
	{
	    private ListItemLogic itemListLogic;
	
	    public void ItemClick()
	    {
	        if (itemListLogic != null)
	        {
	            var itemData = itemListLogic.Item as UIEvent_UnionOtherListClick;
	            var e = new UIEvent_UnionOtherListClick();
	            e.Index = itemListLogic.Index;
	            EventDispatcher.Instance.DispatchEvent(e);
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