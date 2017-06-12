using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TradingEquipSonCell : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void ItemClick()
	    {
	        if (listItemLogic != null)
	        {
	            var e = new UIEvent_OnTradingEquipOperation(6);
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