using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TradingEquipCell : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void IconClick()
	    {
	        if (listItemLogic != null)
	        {
	            var item = listItemLogic.Item as ExchangeEquipItemDataModel;
	            GameUtils.ShowItemDataTip(item.Item);
	        }
	    }
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            var e = new UIEvent_OnTradingEquipOperation(7);
	            e.Value = listItemLogic.Index;
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