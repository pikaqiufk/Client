using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BattleItemConsumeFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    public ItemIdDataModel ItemIdData = new ItemIdDataModel();
	    public TotalCount ItemTotalCount = new TotalCount();
	    public UILabel Lable;
	    private int leftCount;
	    public Color ColorLable { get; set; }
	
	    public ItemIdDataModel ItemData
	    {
	        get { return ItemIdData; }
	        set
	        {
	            ItemIdData = value;
	            BindRoot.SetBindDataSource(ItemIdData);
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
	            ItemTotalCount.PropertyChanged -= OnUpdateBindingData;
	            ItemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(ItemIdData.ItemId);
	            ItemTotalCount.PropertyChanged += OnUpdateBindingData;
	            BindRoot.SetBindDataSource(ItemTotalCount);
	        }
	    }
	
	    public int LeftCount
	    {
	        get { return leftCount; }
	        set
	        {
	            leftCount = value;
	            UpdateBindingData();
	        }
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (ItemTotalCount != null)
	        {
	            ItemTotalCount.PropertyChanged -= OnUpdateBindingData;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (ItemTotalCount != null)
	        {
	            ItemTotalCount.PropertyChanged -= OnUpdateBindingData;
	        }
	        ItemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(ItemIdData.ItemId);
	        ItemTotalCount.PropertyChanged += OnUpdateBindingData;
	        UpdateBindingData();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnUpdateBindingData(object sender, PropertyChangedEventArgs e)
	    {
	        if (e.PropertyName == "Count")
	        {
	            UpdateBindingData();
	        }
	    }
	
	    public void UpdateBindingData()
	    {
	        ColorLable = ItemTotalCount.Count <= 0 ? MColor.red : MColor.white;
	        Lable.color = ColorLable;
	        Lable.text = string.Format("{0}/{1}", ItemTotalCount.Count, leftCount);
	    }
	}
}