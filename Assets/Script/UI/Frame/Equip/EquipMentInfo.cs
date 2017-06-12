using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentInfo : MonoBehaviour, IChainRoot, IChainListener
	{
	    public UIWidget BackGround;
	    public BindDataRoot Binding;
	    public StackLayout Layout;
	    private bool mFlag = true;
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mFlag)
	        {
	            Layout.ResetLayout();
	            SetCenter();
	            mFlag = false;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClickBtnClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.EquipInfoUI));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        Binding.RemoveBinding();
            EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(-1));

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
	        var controllerBase = UIManager.Instance.GetController(UIConfig.EquipInfoUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        mFlag = true;

            EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(UIConfig.EquipInfoUI.UiRecord.Id));

	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void SetCenter()
	    {
	        var height = Layout.height;
	        if (height > 460)
	        {
	            BackGround.height = 550;
	            BackGround.transform.localPosition = new Vector3(8, 253, 0);
	        }
	        else
	        {
	            var h = 100 + height;
	            BackGround.height = h;
	            var c = h/2;
	            BackGround.transform.localPosition = new Vector3(8, c, 0);
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
	
	    public void Listen<T>(T message)
	    {
	        if (message is string && (message as string) == "ActiveChanged")
	        {
	            mFlag = true;
	        }
	    }
	}
}