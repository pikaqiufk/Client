using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipSelectCell : MonoBehaviour
	{
	    private ListItemLogic itemList;
	
	    public void OnClickEquip()
	    {
	        if (itemList != null)
	        {
	            var e = new UIEvent_SelectEquips_SelectIndex(itemList.Index);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    // Use this for initialization
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