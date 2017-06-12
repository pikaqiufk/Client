using GameUI;
using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class CharacterFrame : MonoBehaviour
	{
	    public CharacterAttrFrame Attribute;
	    public Transform BackPackRoot;
	    public BindDataRoot Binding;
	    private BagFrame backPack;
	    private bool removeBind = true;
	
	    private void CreateBag()
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
	
	            backPack = obj.GetComponent<BagFrame>();
	            if (backPack)
	            {
	                backPack.AddBindEvent();
	            }
	        }
	    }
	
	    public void OnBtnShare()
	    {
	        EventDispatcher.Instance.DispatchEvent(new TakeScreenShotAndOpenShareFrame());
	    }
	
	    public void OnClickClose()
	    {

	        var e = new Close_UI_Event(UIConfig.CharacterUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }

        public void VipFunctionDrugStore()
        {
            var e = new UIEvent_RechargeFrame_OnClick(2);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void VipFunctionStore()
        {
            if (PlayerDataManager.Instance.GetRes((int) eResourcesType.VipLevel) < 3)
            {
                var str = GameUtils.GetDictionaryText(100000675);
                GameUtils.ShowHintTip(str);
            }
            else
            {
                var e = new UIEvent_RechargeFrame_OnClick(3);
                EventDispatcher.Instance.DispatchEvent(e);
            }
        }

	    public void OnClickOpenTitleUI()
	    {
	        var e = new Show_UI_Event(UIConfig.TitleUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseUIBindingRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.CharacterUI)
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
	                RemoveBindingEvent();
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
	            RemoveBindingEvent();
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
	        Attribute.DestroyCharacterModel();
	        if (removeBind)
	        {
	            RemoveBindingEvent();
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
	        Attribute.CreateCharacterModel();
	        if (removeBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUIBindingRemove);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.WeakNoticeData);
	            var controller = UIManager.Instance.GetController(UIConfig.ShareFrame);
	            Binding.SetBindDataSource(controller.GetDataModel(""));
	            if (backPack)
	            {
	                backPack.AddBindEvent();
	            }
	            Attribute.AddEvent();
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
	
	    private void RemoveBindingEvent()
	    {
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUIBindingRemove);
	        Binding.RemoveBinding();
	        Attribute.RemoveEvent();
	        if (backPack)
	        {
	            backPack.RemoveBindEvent();
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        CreateBag();
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