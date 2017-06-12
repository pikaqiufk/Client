
using System;
#region using

using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipCell : MonoBehaviour
	{
	    public IconFrame IconData;
	
	    public void OnClickEquip()
	    {
	        if (IconData.BagItemData.ItemId != -1)
	        {
                var tbItemBase = Table.GetItemBase(IconData.BagItemData.ItemId);
                if (tbItemBase.Type < 20000)
                {
                    var e = new Show_UI_Event(UIConfig.EquipComPareUI,
                        new EquipCompareArguments
                        {
                            Data = IconData.BagItemData,
                            ShowType = eEquipBtnShow.EquipPack,
                            ResourceType = 1
                        });
                    EventDispatcher.Instance.DispatchEvent(e);
                }
                //EventDispatcher.Instance.DispatchEvent(new EquipCellSelect(IconData.BagItemData, IconData.BagItemData.Index));
	        }
	    }

	    public void OnClickEquipEx()
	    {//增强界面点击选中
            EventDispatcher.Instance.DispatchEvent(new EquipCellSelect(IconData.BagItemData, IconData.BagItemData.Index));
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