using GameUI;
using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class StoreHouserFrame : MonoBehaviour
	{
	    public Transform BackPackRoot;
	    public BindDataRoot BindData;
	    public BagFrame mBackPack;
	    private bool removeBind = true;
	
	    private void CreateBackPack()
	    {
	        var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/BackPack.prefab");
	        var obj = Instantiate(objres) as GameObject;
	        if (null != BackPackRoot)
	        {
	            var objTransform = obj.transform;
	            //objTransform.parent = BackPackRoot;
	            objTransform.SetParentEX(BackPackRoot);
	            objTransform.localScale = Vector3.one;
	            objTransform.localPosition = Vector3.zero;
	            obj.SetActive(true);
	
	            mBackPack = obj.GetComponent<BagFrame>();
	            if (mBackPack)
	            {
	                mBackPack.AddBindEvent();
	            }
	        }
	    }
	
	    public void OnClickAddCapacity()
	    {
	        var e = new PackCapacityEventUi((int) eBagType.Depot);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickArrange()
	    {
	        var e = new PackArrangeEventUi((int) eBagType.Depot);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.DepotUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.DepotUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            removeBind = false;
	        }
	        else
	        {
	            if (removeBind == false)
	            {
	                RemoveBindEvent();
	            }
	            removeBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (removeBind == false)
	        {
	            RemoveBindEvent();
	        }
	        removeBind = true;
	
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
	        if (removeBind)
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
	        if (removeBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	
	            var data = PlayerDataManager.Instance.GetBag((int) eBagType.Depot);
	            if (data != null)
	            {
	                BindData.SetBindDataSource(data);
	            }
	            if (mBackPack != null)
	            {
	                mBackPack.AddBindEvent();
	            }
	        }
	        removeBind = true;
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
	        BindData.RemoveBinding();
	        if (mBackPack != null)
	        {
	            mBackPack.RemoveBindEvent();
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        CreateBackPack();
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