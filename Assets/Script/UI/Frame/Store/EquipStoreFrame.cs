using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipStoreFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public Transform BackPackRoot;
	    public BindDataRoot BindData;
	    public StackLayout Layout;
	    private BagFrame theBag;
	    private bool isEnable;
	    private bool delBinding = true;
	
	    private void CreateBagBackup()
	    {
	        var objres = ResourceManager.PrepareResourceSync<GameObject>("UI/BackPack.prefab");
	        var obj = Instantiate(objres) as GameObject;
	        if (null != BackPackRoot && obj != null)
	        {
	            var objTransform = obj.transform;
	            //objTransform.parent = BackPackRoot;
	            objTransform.SetParentEX(BackPackRoot);
	            objTransform.localScale = Vector3.one;
	            objTransform.localPosition = Vector3.zero;
	            obj.SetActive(true);
	
	            theBag = obj.GetComponent<BagFrame>();
	            if (theBag)
	            {
	                theBag.AddBindEvent();
	            }
	        }
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (isEnable)
	        {
	            Layout.ResetLayout();
	            isEnable = false;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClickBuyInfoBuy()
	    {
	        var e = new StoreOperaEvent(12);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBuyInfoClose()
	    {
	        var e = new StoreOperaEvent(11);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.StoreEquip);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickReplace()
	    {
	        var e = new StoreOperaEvent(10);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickShowSelectIcon()
	    {
	        var e = new StoreOperaEvent(9);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.StoreEquip)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            delBinding = false;
	        }
	        else
	        {
	            if (delBinding == false)
	            {
	                ClearListener();
	            }
	            delBinding = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (delBinding == false)
	        {
	            ClearListener();
	        }
	        delBinding = true;
	
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
	        if (delBinding)
	        {
	            ClearListener();
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
	        if (delBinding)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	            var control = UIManager.Instance.GetController(UIConfig.StoreEquip);
	            BindData.SetBindDataSource(control.GetDataModel(""));
	            BindData.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	            if (theBag)
	            {
	                theBag.AddBindEvent();
	            }
	        }
	        delBinding = true;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void ClearListener()
	    {
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	        BindData.RemoveBinding();
	        if (theBag != null)
	        {
	            theBag.RemoveBindEvent();
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        CreateBagBackup();
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void Listen<T>(T message)
	    {
	        isEnable = true;
	    }
	}
}