using System;
#region using

using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IconFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    private BagItemDataModel bagItemData;
	
	    public BagItemDataModel BagItemData
	    {
	        get { return bagItemData; }
	        set
	        {
	            bagItemData = value;
	            BindRoot.SetBindDataSource(value);
	        }
	    }
	
	    public void OnClickIcon()
	    {
	        if (GuideManager.Instance.IsGuiding())
	        {
	            return;
	        }
	        if (BagItemData.ItemId != -1)
	        {
	            GameUtils.ShowItemDataTip(BagItemData, eEquipBtnShow.OperateBag);
	        }
	    }
	
	    public void OnClickIconNone()
	    {
	        if (BagItemData.ItemId != -1)
	        {
	            GameUtils.ShowItemDataTip(BagItemData, eEquipBtnShow.None);
	        }
	    }
	
	    public void OnClickIconShare()
	    {
	        if (BagItemData.ItemId != -1)
	        {
	            GameUtils.ShowItemDataTip(BagItemData, eEquipBtnShow.Share);
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