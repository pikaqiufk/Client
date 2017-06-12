using System;
#region using

using UnityEngine;


#endregion

namespace GameUI
{
	public class WishingAnimation : MonoBehaviour
	{
	    //index 停止播放抽奖动画的index
	    //public void StopAnimation(int index)
	    //{
	    //    UIEvent_WishingTenDrawStopEvent e = new UIEvent_WishingTenDrawStopEvent();
	    //    e.Index = index;
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	
	
	    public void ElfBaoxiangAnimOver()
	    {
	        //UIEvent_ElfBaoXiangOverEvent e = new UIEvent_ElfBaoXiangOverEvent();
	        //EventDispatcher.Instance.DispatchEvent(e);
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