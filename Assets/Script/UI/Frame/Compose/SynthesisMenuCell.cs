using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SynthesisMenuCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnCliclMenuCell()
	    {
	        var menuData = ItemLogic.Item as ComposeMenuDataModel;
	        if (menuData != null)
	        {
	            var e = new ComposeMenuCellClick(menuData);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
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