using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class AnimalBattleCell : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            var itemData = listItemLogic.Item as ElfItemDataModel;
	
	            if (itemData.ItemId != -1)
	            {
	                EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.ElfInfoUI,
	                    new ElfInfoArguments {DataModel = itemData}));
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
	}
}