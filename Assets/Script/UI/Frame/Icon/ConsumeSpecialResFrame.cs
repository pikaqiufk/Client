using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ConsumeSpecialResFrame : MonoBehaviour
	{
	    public UILabel CountLable;
	    public TotalCount ItemTotalCount;
	    private int mCount;
	    public int ResourceId;
	
	    public int Count
	    {
	        get { return mCount; }
	        set
	        {
	            mCount = value;
	            UpdataBindingData();
	            CountLable.text = mCount.ToString();
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
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (ItemTotalCount != null)
	        {
	            ItemTotalCount.PropertyChanged -= OnUpdataBindingData;
	        }
	        ItemTotalCount = PlayerDataManager.Instance.GetItemTotalCount(ResourceId);
	        ItemTotalCount.PropertyChanged += OnUpdataBindingData;
	        UpdataBindingData();
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void UpdataBindingData()
	    {
	        if (ItemTotalCount == null)
	        {
	            return;
	        }
	        CountLable.text = mCount.ToString();
	        if (ItemTotalCount.Count >= mCount)
	        {
	            CountLable.color = MColor.white;
	        }
	        else
	        {
	            CountLable.color = MColor.red;
	        }
	    }
	}
}