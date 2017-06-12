using System;
#region using

using System.Collections.Generic;
using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentCompareFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public BindDataRoot EquipCompareBindData;
	    public List<StackLayout> LayoutList;
	    public UIScrollView LookEquipView;
	    private bool mFlag = true;
	    public Transform SellContent;
	    public UIScrollView UsedEquipView;
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mFlag)
	        {
	            mFlag = false;
	            var __list2 = LayoutList;
	            var __listCount2 = __list2.Count;
	            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	            {
	                var layout = __list2[__i2];
	                {
	                    layout.ResetLayout();
	                }
	            }
	            ResetSellLayout();
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClick_Close()
	    {
	        var e = new UIEvent_EquipCompare_Close();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClick_Reclaim()
	    {
	        var e = new UIEvent_EquipCompare_Reclaim();
	        EventDispatcher.Instance.DispatchEvent(e);
	        //OnClick_Close();
	    }
	
	    public void OnClick_Use()
	    {
	        var e = new UIEvent_EquipCompare_Use(-1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBlank()
	    {
	        var e = new UIEvent_EquipCompare_Close();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickEquipReplace1()
	    {
	        var e = new UIEvent_EquipCompare_Use(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickEquipReplace2()
	    {
	        var e = new UIEvent_EquipCompare_Use(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickInput()
	    {
	        var e = new UIEvent_EquipCompare_Input();
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        var e1 = new Close_UI_Event(UIConfig.EquipComPareUI);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    public void OnClickOperate()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_EquipCompareBtnClick(3));
	    }
	
	    public void OnClickSell()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_EquipCompare_Sell());
	        //OnClick_Close();
	    }
	
	    public void OnClickShare()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_EquipCompare_Share());
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        //EquipCompareBindData.RemoveBinding();
	
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
	        {
	            var __list1 = LayoutList;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var layout = __list1[__i1];
	                {
	                    if (layout.gameObject.activeSelf && layout.enabled)
	                    {
	                        layout.ResetLayout();
	                    }
	                }
	            }
	        }
	        if (LookEquipView)
	        {
	            LookEquipView.ResetPosition();
	        }
	        if (UsedEquipView)
	        {
	            UsedEquipView.ResetPosition();
	        }
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void ResetSellLayout()
	    {
	        var length = 0;
	        for (var i = 0; i < 4; i++)
	        {
	            var t = SellContent.GetChild(i);
	            if (t.gameObject.activeSelf == false)
	            {
	                continue;
	            }
	            var loc = t.localPosition;
	            var w = t.GetComponent<UIWidget>();
	            loc.x = length;
	            if (w == null)
	            {
	                continue;
	            }
	            t.localPosition = loc;
	            length += w.width;
	        }
	    }
	
	    //public UISprite Background;
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        Logger.Info("EquipCompareLogic StartOver");
	        var controllerBase = UIManager.Instance.GetController(UIConfig.EquipComPareUI);
	        if (controllerBase != null)
	        {
	            EquipCompareBindData.SetBindDataSource(controllerBase.GetDataModel(""));
	        }
	        //UIRoot root = this.transform.root.GetComponent<UIRoot>();
	        //var s = (float)root.activeHeight / Screen.height;
	        //Background.width = (int)(Screen.width * s + 10);
	        //Background.height = (int)(Screen.height * s + 10);
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