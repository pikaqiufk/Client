using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class BlacksmithShopFurnaceCell : MonoBehaviour
	{
	    public int Index;
	    public TweenPosition TweenPos;
	
	    public void OnClickAdd()
	    {
	        var e = new SmithyFurnaceCellEvent();
	        e.Type = 0;
	        e.Index = Index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickCancel()
	    {
	        var e = new SmithyFurnaceCellEvent();
	        e.Type = 1;
	        e.Index = Index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickFinish()
	    {
	        var e = new SmithyFurnaceCellEvent();
	        e.Type = 2;
	        e.Index = Index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickShowInfo()
	    {
	        var e = new SmithyFurnaceCellEvent();
	        e.Type = 4;
	        e.Index = Index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSpeedUp()
	    {
	        var e = new SmithyFurnaceCellEvent();
	        e.Type = 3;
	        e.Index = Index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickStart()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(0));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(SmithyOnPlayTweenAnim.EVENT_TYPE, OnPlayAnim);
	    
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
	
	        EventDispatcher.Instance.AddEventListener(SmithyOnPlayTweenAnim.EVENT_TYPE, OnPlayAnim);
	        // TweenPos.ResetForPlay();
	    
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	}
	
	    private void OnPlayAnim(IEvent ievent)
	    {
	        var e = ievent as SmithyOnPlayTweenAnim;
	        if (e.Index == Index)
	        {
	            TweenPos.ResetForPlay();
	            TweenPos.PlayForward();
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