using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SetupFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private bool deleteBind = true;
	    public UILabel QualityLabel;
	    public UIPopupList QualityList;
	    public UILabel ResolutionLabel;
	    public UIPopupList ResolutionList;
	
	    public void ExitToLogin()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SettingUI));
	        Game.Instance.ExitToLogin();
	    }
	
	    public void ExitToSelectRole()
	    {
	        EventDispatcher.Instance.DispatchEvent(new SystemNoticeOperate(1));
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SettingUI));
	        Game.Instance.ExitToSelectRole();
	    }
	
	    public void ExitToServerList()
	    {
	        EventDispatcher.Instance.DispatchEvent(new SystemNoticeOperate(1));
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SettingUI));
	        Game.Instance.ExitToServerList();
	    }
	
	    public void OnBtnQuitGame()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SettingUI));
	        Application.Quit();
	    }
	
	    public void OnBtnUserCenter()
	    {
	        PlatformHelper.UserCenter();
	    }
	
	    public void OnClickBtnClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SettingUI));
	    }
	
	    private void OnEvent_CloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.SettingUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            deleteBind = false;
	        }
	        else
	        {
	            if (deleteBind == false)
	            {
	                DeleteListener();
	            }
	            deleteBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (deleteBind == false)
	        {
	            DeleteListener();
	        }
	        deleteBind = true;
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
	        if (deleteBind)
	        {
	            DeleteListener();
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
	        if (deleteBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	
	            var controllerBase = UIManager.Instance.GetController(UIConfig.SettingUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            var source = controllerBase.GetDataModel("");
	            var dataModel = source as SettingDataModel;
	            if (dataModel != null)
	            {
	                var quality = dataModel.SystemSetting.QualityToggle;
	                QualityLabel.text = QualityList.items[quality - 1];
	                // QualityList.value = QualityLabel.text;
	                var resolution = dataModel.SystemSetting.Resolution;
	                ResolutionLabel.text = ResolutionList.items[resolution - 1];
	                // ResolutionList.value = ResolutionLabel.text;
	            }
	            Binding.SetBindDataSource(source);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        }
	        deleteBind = true;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void QualityChange()
	    {
	        var v = QualityList.value;
	        QualityLabel.text = v;
	        var index = QualityList.items.IndexOf(v);
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_QualitySetting(index + 1));
	    }
	
	    private void DeleteListener()
	    {
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnEvent_CloseUI);
	    }
	
	    public void ResoultionChange()
	    {
	        var v = ResolutionList.value;
	        ResolutionLabel.text = v;
	        var index = ResolutionList.items.IndexOf(v);
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ResolutionSetting(index + 1));
	    }
	}
}