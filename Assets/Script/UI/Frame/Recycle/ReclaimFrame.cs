using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ReclaimFrame : MonoBehaviour
	{
	    public Transform BackPackRoot;
	    public BindDataRoot Binding;
	    public List<UIButton> ColorBtn;
	    public UIGridSimple getGrid;
	    private bool hasBindRemoved = true;
	
	    public void BtnBack()
	    {
	        var e = new Close_UI_Event(UIConfig.RecycleUI, true);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnGetCancel()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_RecycleGetCancel());
	    }
	
	    public void BtnGetOK()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_RecycleGetOK());
	    }
	
	    public void BtnRecycle()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_RecycleBtn(1));
	    }
	
	    public void BtnSell()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_RecycleBtn(0));
	    }
	
	    public void Close()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.RecycleUI));
	    }
	
	    private void OnCloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.RecycleUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            hasBindRemoved = false;
	        }
	        else
	        {
	            if (hasBindRemoved == false)
	            {
	                RemoveBindEvent();
	            }
	            hasBindRemoved = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        if (hasBindRemoved == false)
	        {
	            RemoveBindEvent();
	        }
	        hasBindRemoved = true;
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (hasBindRemoved)
	        {
	            RemoveBindEvent();
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
	        if (hasBindRemoved)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	            EventDispatcher.Instance.AddEventListener(UIEvent_RecycleSetGridCenter.EVENT_TYPE, SetGridCenter);
	            var controllerBase = UIManager.Instance.GetController(UIConfig.RecycleUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        }
	        hasBindRemoved = true;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void RemoveBindEvent()
	    {
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_RecycleSetGridCenter.EVENT_TYPE, SetGridCenter);
	        Binding.RemoveBinding();
	    }
	
	    private void SetGridCenter(IEvent ievent)
	    {
	        var e = ievent as UIEvent_RecycleSetGridCenter;
	        getGrid.SetLookCenter();
	        // StartCoroutine(SetGridCenterIEnumerator());
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        //CreateBackPack();
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