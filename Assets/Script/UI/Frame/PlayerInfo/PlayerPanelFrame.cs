using System;
#region using

using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PlayerPanelFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public List<UIEventTrigger> EquipTriggers;
	    public UIDragRotate ModelDrag;
	    public CreateFakeCharacter ModelRoot;
	    private bool isDelBinding = true;
	
	    public void OnClickApplyTeam()
	    {
	        var e = new PlayerInfoOperation(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.PlayerInfoUI, true);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickChat()
	    {
	        var e = new PlayerInfoOperation(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickElf()
	    {
	        var e = new PlayerInfoOperation(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickEquipType(int index)
	    {
	        var e = new PlayerInfoOperation(10, index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickFriend()
	    {
	        var e = new PlayerInfoOperation(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTrade()
	    {
	        var e = new PlayerInfoOperation(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.PlayerInfoUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            isDelBinding = false;
	        }
	        else
	        {
	            if (isDelBinding == false)
	            {
	                DeleteListener();
	            }
	            isDelBinding = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        if (isDelBinding == false)
	        {
	            DeleteListener();
	        }
	        isDelBinding = true;
	
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
	        if (isDelBinding)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	            var controllerBase = UIManager.Instance.GetController(UIConfig.PlayerInfoUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            var dataModel = controllerBase.GetDataModel("") as PlayerInfoDataModel;
	            Binding.SetBindDataSource(dataModel);
	        }
	        isDelBinding = true;
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnEvent_PlayerDataChange(IEvent ievent)
	    {
	        var e = ievent as PlayerInfoRefreshModelView;
	
	        UpdateModelView(e.Type, e.EquipModels, e.ElfId);
	    }
	
	    private void UpdateModelView(int type, Dictionary<int, int> equips, int elfId)
	    {
	        if (ModelRoot.Character != null)
	        {
	            ModelRoot.DestroyFakeCharacter();
	        }
	
	        ModelRoot.Create(type, equips, character => { ModelDrag.Target = character.transform; }, elfId, true);
	    }
	
	    private void DeleteListener()
	    {
	        if (null != ModelRoot.Character)
	        {
	            ModelRoot.DestroyFakeCharacter();
	        }
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(PlayerInfoRefreshModelView.EVENT_TYPE, OnEvent_PlayerDataChange);
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        var controllerBase = UIManager.Instance.GetController(UIConfig.PlayerInfoUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        var dataModel = controllerBase.GetDataModel("") as PlayerInfoDataModel;
	        EventDispatcher.Instance.AddEventListener(PlayerInfoRefreshModelView.EVENT_TYPE, OnEvent_PlayerDataChange);
	        if (dataModel != null)
	        {
	            UpdateModelView(dataModel.Type, dataModel.EquipsModel, dataModel.ElfData.ItemId);
	        }
	
	        var c = EquipTriggers.Count;
	        for (var i = 0; i < c; i++)
	        {
	            var e = EquipTriggers[i];
	            if (e != null)
	            {
	                var j = i;
	                e.onClick.Add(new EventDelegate(() => { OnClickEquipType(j); }));
	            }
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