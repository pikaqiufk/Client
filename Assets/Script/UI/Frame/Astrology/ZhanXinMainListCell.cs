using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ZhanXinMainListCell : MonoBehaviour
	{
	    public List<UIEventTrigger> ItemList;
	    private ListItemLogic itemlistLogic;
	    //主界面列表选中
	    public void ItemClick()
	    {
	        if (itemlistLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_AstrologyMainIconClick(itemlistLogic.Index));
	        }
	    }
	
	    //主界面列表宝石点击事件
	    public void OnItemListClick(int index)
	    {
	        if (itemlistLogic != null)
	        {
	            var e = new UIEvent_AstrologyMainListClick();
	            e.Index = itemlistLogic.Index;
	            e.ItemIndex = index;
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    //详细界面simplelist点击事件
	    public void SimpleIconClick()
	    {
	        if (itemlistLogic != null)
	        {
	            var e = new UIEvent_AstrologySimpleListClick();
	            e.Index = itemlistLogic.Index;
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        itemlistLogic = gameObject.GetComponent<ListItemLogic>();
	
	        for (var i = 0; i < ItemList.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { OnItemListClick(j); });
	            ItemList[i].onClick.Add(deleget);
	        }
	
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