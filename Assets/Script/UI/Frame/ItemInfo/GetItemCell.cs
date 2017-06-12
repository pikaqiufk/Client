using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class GetItemCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickGetFrame()
	    {
	        if (ItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new Event_ItemInfoGetClick(ItemLogic.Index));
	        }
	    }
	
	    public void OnClickIcon()
	    {
	        if (ItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new Event_ItemInfoClick(ItemLogic.Index));
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
	
	    // Update is called once per frame
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