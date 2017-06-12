using System;
#region using

using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TradingEquipHistoryCell : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void IconClick()
	    {
	        if (listItemLogic != null)
	        {
	            var item = listItemLogic.Item as ExchangeEquipHistoryItemDataModel;
	            if (item == null)
	            {
	                return;
	            }
	            GameUtils.ShowItemDataTip(item.Item);
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