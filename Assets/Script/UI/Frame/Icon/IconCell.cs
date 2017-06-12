using System;
#region using

using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IconCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickIcon()
	    {
	        var data = ItemLogic.Item as BagItemDataModel;
	        if (data != null)
	        {
	            GameUtils.ShowItemDataTip(data, eEquipBtnShow.OperateBag);
	        }
	    }
	
	    public void OnClickIconNone()
	    {
	        var data = ItemLogic.Item as BagItemDataModel;
	        if (data != null)
	        {
	            GameUtils.ShowItemDataTip(data, eEquipBtnShow.None);
	        }
	    }
	
	    public void OnClickIconShare()
	    {
	        var data = ItemLogic.Item as BagItemDataModel;
	        if (data != null)
	        {
	            GameUtils.ShowItemDataTip(data, eEquipBtnShow.Share);
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