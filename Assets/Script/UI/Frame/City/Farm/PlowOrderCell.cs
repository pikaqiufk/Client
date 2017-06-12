using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PlowOrderCell : MonoBehaviour
	{
	    public ListItemLogic ListItem;
	
	    public void OnCliclFarmOrder()
	    {
	        if (ListItem == null)
	        {
	            return;
	        }
	        var idnex = ListItem.Index;
	        var e = new FarmOrderListClick(idnex);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnFormatExpect(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var taget = timer.TargetTime;
	        if (taget > Game.Instance.ServerTime)
	        {
	            var dif = (taget - Game.Instance.ServerTime);
	            var str = string.Format("{0}:{1}:{2}", dif.Hours, dif.Minutes, dif.Seconds);
	            lable.text = str;
	        }
	        else
	        {
	            var str = "";
	            lable.text = str;
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