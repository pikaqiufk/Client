using System;
#region using

using ClientDataModel;
using DataTable;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentReplace : MonoBehaviour
	{
	    public UISprite LeftIco;
	    public UILabel LeftName;
	    public UISprite RightIco;
	    public UILabel RightName;
	    public BagItemDataModel BagItemData { get; set; }
	
	    public void OnClickClose()
	    {
	    }
	
	    public void OnClickEquip()
	    {
	    }
	
	    public void OnClickGem()
	    {
	    }
	
	    public void Refresh()
	    {
	        var nTableIndex = BagItemData.ItemId;
	        var record = Table.GetEquipBase(nTableIndex);
	        RightName.text = record.Id.ToString();
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