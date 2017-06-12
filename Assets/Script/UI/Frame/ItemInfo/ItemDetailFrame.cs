using System;
#region using

using System.Collections.Generic;
using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ItemDetailFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public BindDataRoot BindRoot;
	    public UIWidget ButtonList;
	    public UIWidget Content;
	    public Transform DescList;
	    public List<StackLayout> Layout;
	    private Transform theTransform;
	
	    private readonly List<List<Vector3>> infoPosList = new List<List<Vector3>>
	    {
	        new List<Vector3> {new Vector3(0, -23, 0)},
	        new List<Vector3> {new Vector3(0, -23, 0), new Vector3(0, -46, 0)},
	        new List<Vector3> {new Vector3(0, -16, 0), new Vector3(0, -40, 0), new Vector3(0, -64, 0)},
	        new List<Vector3>
	        {
	            new Vector3(0, -10, 0),
	            new Vector3(0, -30, 0),
	            new Vector3(0, -50, 0),
	            new Vector3(0, -70, 0)
	        }
	    };
	
	    private bool isEnable;
	    public UISlider RecycleSlider;
	    public UIButton SellAffirmBtn;
	    public Transform SellContent;
	    public GameObject SellMessage;
	    public UISlider SellSlider;
	    public List<GameObject> ShowContent;
	    public UIButton UseAffirmBtn;
	    public GameObject UseMessage;
	    public UISlider UseSlider;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        theTransform = Content.transform;
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void CloseSellMessage()
	    {
	        SellMessage.SetActive(false);
	    }
	
	    public void CloseUseMessage()
	    {
	        UseMessage.SetActive(false);
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        if (isEnable)
	        {
	            {
	                var __list2 = Layout;
	                var __listCount2 = __list2.Count;
	                for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	                {
	                    var layout = __list2[__i2];
	                    {
	                        layout.ResetLayout();
	                    }
	                }
	            }
	            PutInCenter();
	
	            ResetDescribe();
	
	            ResetSellPut();
	
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
	
	    public void OnClickClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ItemInfoUI));
	    }
	
	    public void OnClickCloseGet()
	    {
	        var e = new ItemInfoOperate(5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickCompose()
	    {
	        var e = new ItemInfoOperate(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickGet()
	    {
	        var e = new ItemInfoOperate(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycle()
	    {
	        var e = new ItemInfoOperate(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycleAdd()
	    {
	        var e = new ItemInfoCountChange(3, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycleAddPress()
	    {
	        var e = new ItemInfoCountChange(3, 2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycleAddRelease()
	    {
	        var e = new ItemInfoCountChange(3, 4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycleDelPress()
	    {
	        var e = new ItemInfoCountChange(3, 3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycleDelRelease()
	    {
	        var e = new ItemInfoCountChange(3, 5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRecycleSub()
	    {
	        var e = new ItemInfoCountChange(3, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellAdd()
	    {
	        var e = new ItemInfoCountChange(1, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellAddPress()
	    {
	        var e = new ItemInfoCountChange(1, 2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellAddRelease()
	    {
	        var e = new ItemInfoCountChange(1, 4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellDelPress()
	    {
	        var e = new ItemInfoCountChange(1, 3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellDelRelease()
	    {
	        var e = new ItemInfoCountChange(1, 5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickSellSub()
	    {
	        var e = new ItemInfoCountChange(1, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUse()
	    {
	        var e = new ItemInfoOperate(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUseAdd()
	    {
	        var e = new ItemInfoCountChange(2, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUseAddPress()
	    {
	        var e = new ItemInfoCountChange(2, 2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUseAddRelease()
	    {
	        var e = new ItemInfoCountChange(2, 4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUseDelPress()
	    {
	        var e = new ItemInfoCountChange(2, 3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUseDelRelease()
	    {
	        var e = new ItemInfoCountChange(2, 5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickUseSub()
	    {
	        var e = new ItemInfoCountChange(2, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(ItemInfoNotifyEvent.EVENT_TYPE, OnEvent_ItemNotify);
	
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
            EventDispatcher.Instance.DispatchEvent(new MieShiDisappearModelRoot_Event(true));
            SellMessage.SetActive(false);
	        UseMessage.SetActive(false);
	        BindRoot.RemoveBinding();
	        {
	            var __list1 = ShowContent;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var o = __list1[__i1];
	                {
	                    if (o)
	                    {
	                        o.SetActive(false);
	                    }
	                }
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
	
	    private void OnEnable()
	    {
#if !UNITY_EDITOR
	        try
	        {
#endif
            EventDispatcher.Instance.DispatchEvent(new MieShiDisappearModelRoot_Event(false));
            SellMessage.SetActive(false);
	        UseMessage.SetActive(false);
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ItemInfoUI);
	        BindRoot.SetBindDataSource(controllerBase.GetDataModel(""));
	        isEnable = true;
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnEvent_ItemNotify(IEvent ievent)
	    {
	        var e = ievent as ItemInfoNotifyEvent;
	        switch (e.Type)
	        {
	            case 0:
	            {
	                OpenUseMessage();
	            }
	                break;
	        }
	    }
	
	    public void OnRecycleCountCancel()
	    {
	        var e = new UIEvent_ItemInfoFrame_BtnAffirmClick(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnRecycleCountOk()
	    {
	        var e = new UIEvent_ItemInfoFrame_BtnAffirmClick(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //public void OnUseCountChange()
	    //{
	    //    var rate = UseSlider.value;
	    //    var step = UseSlider.numberOfSteps;
	    //    if (step == 1)
	    //    {
	    //        UseSlider.value = 1.0f;
	    //        return;
	    //    }
	    //    int count = (int)Mathf.Floor((step - 1) * rate + 0.5f);
	    //    //count += 1;
	    //    UIEvent_ItemInfoFrame_ChangeItemCount e = new UIEvent_ItemInfoFrame_ChangeItemCount();
	    //    e.Count = count;
	    //    e.Type = 0;
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	    //public void OnSellCountChange()
	    //{
	    //    var rate = SellSlider.value;
	    //    var step = SellSlider.numberOfSteps;
	    //    if (step == 1)
	    //    {
	    //        SellSlider.value = 1.0f;
	    //        return;
	    //    }
	    //    int count = (int)Mathf.Floor((SellSlider.numberOfSteps - 1) * rate + 0.5f);
	    //    //count += 1;
	    //    UIEvent_ItemInfoFrame_ChangeItemCount e = new UIEvent_ItemInfoFrame_ChangeItemCount();
	    //    e.Count = count;
	    //    e.Type = 1;
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	    //public void OnRecycleCountChange()
	    //{
	    //    var rate = RecycleSlider.value;
	    //    var step = RecycleSlider.numberOfSteps;
	    //    if (step == 1)
	    //    {
	    //        RecycleSlider.value = 1.0f;
	    //        return;
	    //    }
	    //    int count = (int)Mathf.Floor((RecycleSlider.numberOfSteps - 1) * rate + 0.5f);
	    //    //count += 1;
	    //    UIEvent_ItemInfoFrame_ChangeItemCount e = new UIEvent_ItemInfoFrame_ChangeItemCount();
	    //    e.Count = count;
	    //    e.Type = 2;
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	
	    public void OnSellCountOk()
	    {
	        var e = new UIEvent_ItemInfoFrame_BtnAffirmClick(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	        SellMessage.SetActive(false);
	    }
	
	    public void OnUseCountOk()
	    {
	        var e = new UIEvent_ItemInfoFrame_BtnAffirmClick(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	        UseMessage.SetActive(false);
	    }
	
	    public void OpenSellMessage()
	    {
	        SellSlider.value = 1.0f;
	        SellMessage.SetActive(true);
	        UseMessage.SetActive(false);
	    }
	
	    public void OpenUseMessage()
	    {
	        UseSlider.value = 1.0f;
	        UseMessage.SetActive(true);
	        SellMessage.SetActive(false);
	    }
	
	    private void ResetDescribe()
	    {
	        var count = 0;
	        var childCount = DescList.childCount;
	        for (var i = 0; i < childCount; i++)
	        {
	            var t = DescList.GetChild(i);
	            if (t.gameObject.activeSelf)
	            {
	                count++;
	            }
	        }
	
	        count--;
	        if (count < 0 || count >= 4)
	        {
	            return;
	        }
	        var list = infoPosList[count];
	        var activeFlag = 0;
	
	        for (var i = 0; i < childCount; i++)
	        {
	            var t = DescList.GetChild(i);
	            if (t.gameObject.activeSelf)
	            {
	                t.transform.localPosition = list[activeFlag];
	                activeFlag++;
	            }
	        }
	    }
	
	    private void ResetSellPut()
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
	
	    private void PutInCenter()
	    {
	        var pos = theTransform.localPosition;
	        var center = Content.height/2.0f;
	        theTransform.localPosition = new Vector3(pos.x, center, pos.z);
	
	        ButtonList.transform.localPosition = new Vector3(315, -center + 98, 0);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        var root = transform.root.GetComponent<UIRoot>();
	        var s = (float) root.activeHeight/Screen.height;
	
	        EventDispatcher.Instance.AddEventListener(ItemInfoNotifyEvent.EVENT_TYPE, OnEvent_ItemNotify);
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