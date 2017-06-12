using System;
#region using

using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentListCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickCell()
	    {
	        var data = ItemLogic.Item as EquipItemDataModel;
	        EventDispatcher.Instance.DispatchEvent(new EquipCellSelect(data.BagItemData, ItemLogic.Index));
	    }
	
	    public void OnClickIcon()
	    {
	        if (GuideManager.Instance.IsGuiding())
	        {
	            return;
	        }
	
	        var data = ItemLogic.Item as EquipItemDataModel;
	        var BagItemData = data.BagItemData;
	        if (BagItemData.ItemId != -1)
	        {
	            var tbItemBase = Table.GetItemBase(BagItemData.ItemId);
	            if (GameUtils.GetItemInfoType(tbItemBase.Type) == eItemInfoType.Item)
	            {
    	            var showType = 0;
                    if (data.TipButtonShow >= 0)
                    {
                        showType = data.TipButtonShow;
                    }
	                var e = new Show_UI_Event(UIConfig.ItemInfoUI, new ItemInfoArguments
	                {
	                    DataModel = BagItemData,
                        ItemId = 0,
                        ShowType = showType
	                });
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	            else
	            {
	                if (BagItemData.BagId == (int) eBagType.Equip)
	                {
	                    var showType = eEquipBtnShow.OperateBag;
	                    if (data.TipButtonShow >= 0)
	                    {
                            showType = (eEquipBtnShow)data.TipButtonShow;
	                    }
	                    var e = new Show_UI_Event(UIConfig.EquipComPareUI,
                            new EquipCompareArguments { Data = BagItemData, ShowType = showType });
	                    EventDispatcher.Instance.DispatchEvent(e);
	                }
	                else
	                {
                        var showType = eEquipBtnShow.Input;
                        if (data.TipButtonShow >= 0)
                        {
                            showType = (eEquipBtnShow)data.TipButtonShow;
                        }
                        var e = new Show_UI_Event(UIConfig.EquipComPareUI,
                            new EquipCompareArguments { Data = BagItemData, ShowType = showType });
	                    EventDispatcher.Instance.DispatchEvent(e);
	                }
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