using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PayActivityMenuItem : MonoBehaviour
	{
	    public ListItemLogic ListItem;
	
	    public void OnItemClick()
	    {
	        if (null != ListItem)
	        {
	            EventDispatcher.Instance.DispatchEvent(new RechageActivityMenuItemClick(ListItem.Index));
	        }
	    }
	
	    public void OnTimeFormat(UILabel label)
	    {
	        if (label == null)
	        {
	            return;
	        }
	        var timer = label.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        label.text = GameUtils.GetTimeDiffString(timer.TargetTime);
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