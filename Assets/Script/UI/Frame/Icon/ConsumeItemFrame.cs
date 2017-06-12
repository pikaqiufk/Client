using System;
#region using

using System.Collections.Generic;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ConsumeItemFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    public ItemIdDataModel ItemIdData = new ItemIdDataModel();
	    public TotalCount ItemTotalCount;
	    public List<UILabel> Lables;
	    public Color ColorLable { get; set; }
	
	    public int Count
	    {
	        get { return ItemIdData.Count; }
	        set
	        {
	            ItemIdData.Count = value;
	            UpdataBindingData();
	        }
	    }
	
	    public ItemIdDataModel ItemData
	    {
	        get { return ItemIdData; }
	        set
	        {
	            ItemIdData = value;
	            BindRoot.SetBindDataSource(ItemIdData);
	            NeedShowAll();
	            if (ItemTotalCount != null)
	            {
	                ItemTotalCount.PropertyChanged -= OnUpdataBindingData;
	            }
	            ItemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(ItemIdData.ItemId);
	            ItemTotalCount.PropertyChanged += OnUpdataBindingData;
	            BindRoot.SetBindDataSource(ItemTotalCount);
	            UpdataBindingData();
	        }
	    }
	
	    [TableBinding("ItemBase")]
	    public int ItemId
	    {
	        get { return ItemIdData.ItemId; }
	        set
	        {
	            ItemIdData.ItemId = value;
	            BindRoot.SetBindDataSource(ItemIdData);
	            NeedShowAll();
	            if (ItemTotalCount != null)
	            {
	                ItemTotalCount.PropertyChanged -= OnUpdataBindingData;
	            }
	            ItemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(ItemIdData.ItemId);
	            ItemTotalCount.PropertyChanged += OnUpdataBindingData;
	            BindRoot.SetBindDataSource(ItemTotalCount);
	            UpdataBindingData();
	        }
	    }
	
	    private void NeedShowAll()
	    {
	        var tbItem = Table.GetItemBase(ItemIdData.ItemId);
	        if (tbItem == null)
	        {
	            return;
	        }
	        if (Lables.Count != 2)
	        {
	            return;
	        }
	        var lab = Lables[1];
	        if (tbItem.CanInBag == -1)
	        {
	            if (lab.enabled)
	            {
	                lab.enabled = false;
	            }
	        }
	        else
	        {
	            if (lab.enabled == false)
	            {
	                lab.enabled = true;
	            }
	        }
	    }
	
	    public void OnClickIcon()
	    {
	        if (ItemId != -1)
	        {
	            GameUtils.ShowItemIdTip(ItemId);
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (ItemTotalCount != null)
	        {
	            ItemTotalCount.PropertyChanged -= OnUpdataBindingData;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnUpdataBindingData(object sender, PropertyChangedEventArgs e)
	    {
	        if (e.PropertyName == "Count")
	        {
	            UpdataBindingData();
	        }
	    }
	
	    public void UpdataBindingData()
	    {
	        if (ItemTotalCount == null)
	        {
	            return;
	        }
	        if (Lables.Count == 1 && ItemTotalCount.Count == 0)
	        {
	            ColorLable = MColor.red;
	        }
	        else
	        {
	            ColorLable = ItemTotalCount.Count < Count ? MColor.red : MColor.white;
	        }
	        {
	            var __list1 = Lables;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var lable = __list1[__i1];
	                {
	                    if (lable.color != ColorLable)
	                    {
	                        lable.color = ColorLable;
	                    }
	                }
	            }
	        }
	    }
	}
}