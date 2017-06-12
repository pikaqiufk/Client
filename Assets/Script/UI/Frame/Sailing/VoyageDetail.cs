using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class VoyageDetail : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public GameObject labelParent;
	
	    public void Close()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MedalInfoUI));
	    }
	
	    public void EquipLevelUp()
	    {
	        var e = new UIEvent_SailingOperation(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	        Close();
	    }
	
	    public void OnclickPutOff()
	    {
	        var e = new UIEvent_SailingPutOnClick();
	        e.PutOnOrOff = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	        Close();
	    }
	
	    public void OnclickPutOn()
	    {
	        var e = new UIEvent_SailingPutOnClick();
	        e.PutOnOrOff = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	        Close();
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        Binding.RemoveBinding();
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    // Update is called once per frame
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MedalInfoUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        RefreshAttrPanel(null);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void RefreshAttrPanel(IEvent unuse)
	    {
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MedalInfoUI);
	        var infoData = controllerBase.GetDataModel("") as MedalInfoDataModel;
	
	
	        var labels = labelParent.GetComponentsInChildren<UILabel>();
	        var i = 0;
	        {
	            var __array1 = labels;
	            var __arrayLength1 = __array1.Length;
	            for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
	            {
	                var label = __array1[__i1];
	                {
	                    if (i < infoData.ItemPropUI.Count)
	                    {
	                        var attr = infoData.ItemPropUI[i];
	                        label.text = string.Format("{0}    +{1}", attr.AttrName, attr.AttrValue);
	                    }
	                    else
	                    {
	                        label.text = string.Empty;
	                    }
	                    ++i;
	                }
	            }
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