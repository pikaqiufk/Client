using System;
#region using

using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IconIdCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickIcon()
	    {
	        var data = ItemLogic.Item as ItemIdDataModel;
	        if (data != null)
	        {
	            if (data.ItemId != -1)
	            {
	                GameUtils.ShowItemIdTip(data.ItemId);
	            }
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