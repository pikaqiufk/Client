using ClientDataModel;
using GameUI;
using System;
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class EquipMentUIFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public List<Transform> AttributeFrames;
	    public BindDataRoot BindRoot;
	    public List<Transform> EnchanceConsumes;
	
	    private readonly List<List<Vector3>> EnchanceConsumesPosition = new List<List<Vector3>>
	    {
	        new List<Vector3> {new Vector3(-58, 45, 0)},
	        new List<Vector3> {new Vector3(-108, 45, 0), new Vector3(-11, 45, 0)},
	        new List<Vector3> {new Vector3(-155, 45, 0), new Vector3(-70, 45, 0), new Vector3(15, 45, 0)},
	        new List<Vector3>
	        {
	            new Vector3(-240, 45, 0),
	            new Vector3(-155, 45, 0),
	            new Vector3(-70, 45, 0),
	            new Vector3(15, 45, 0)
	        }
	    };
	
	    public Transform EnchangeAttr;
	    public EquipMentPackFrame EquipPack;
	    public Animation FailEffect;
	    public GameObject FailItemAni;
	    public Transform GeneralLightContainer;
        public GameObject GeneralSuccess;
	    public GameObject InheritTip;
	    public Transform LightContainer;
	    private readonly int EnchangeAttrUpOffset = 70;
	    private bool mflag = true;
	    private readonly List<Vector3> LightPos = new List<Vector3> {new Vector3(0, 0, 0), new Vector3(80, 0, 0)};
	    private bool lockAnimation;
	    private int pack = -1;
	    private bool mRemoveBind = true;
	    private int mStart = -1;
	    public List<GameObject> OperateFrames;
	    public List<UIToggle> OperateMenus;
	    public Animation SuccessEffect;
	    public List<Transform> SupperConsumes;
	
	    private readonly List<List<Vector3>> SupperConsumesPosition = new List<List<Vector3>>
	    {
	        new List<Vector3> {new Vector3(51, -5, 0)},
	        new List<Vector3> {new Vector3(3, -5, 0), new Vector3(103, -5, 0)}
	    };
	
	    private IEnumerator EndAnimation(float delay, GameObject obj)
	    {
	        yield return new WaitForSeconds(delay);
	        obj.SetActive(false);
	        lockAnimation = false;
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mflag)
	        {
	            mflag = false;
	            ResetAttrLayout();
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
	        for (var i = 0; i < 6; i++)
	        {
	            OperateMenus[i].value = false;
	            OperateMenus[i].mIsActive = false;
	            if (OperateMenus[i].activeSprite != null)
	            {
	                OperateMenus[i].activeSprite.alpha = 0.0f;
	            }
	            OperateFrames[i].SetActive(false);
	        }
	
	        var e = new Close_UI_Event(UIConfig.EquipUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	        PlayerDataManager.Instance.WeakNoticeData.Additional = false;
	    }
	
	    public void OnClickBtnInherit()
	    {
	        if (lockAnimation)
	        {
	            return;
	        }
	        var e = new EquipOperateClick(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickEquipAppend()
	    {
	        if (lockAnimation)
	        {
	            return;
	        }
	        var e = new EquipOperateClick(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickEquipEnchance()
	    {
	        if (lockAnimation)
	        {
	            return;
	        }
	        var e = new EquipOperateClick(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickExcellentReset()
	    {
	        if (lockAnimation)
	        {
	            return;
	        }
	        var e = new EquipOperateClick(20);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickExcellentResetCancel()
	    {
	        var e = new EquipOperateClick(22);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickExcellentResetOk()
	    {
	        var e = new EquipOperateClick(21);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickInheritedItem()
	    {
	        var e = new EquipOperateClick(42);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickInheritItem()
	    {
	        var e = new EquipOperateClick(41);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickInheritTipClose()
	    {
	        InheritTip.SetActive(false);
	    }
	
	    public void OnClickInheritTipShow()
	    {
	        InheritTip.SetActive(true);
	    }
	
	    public void OnClickSuperExcellent()
	    {
	        if (lockAnimation)
	        {
	            return;
	        }
	        var e = new EquipOperateClick(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTipsClose()
	    {
	        var e = new EquipOperateClick(44);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTipsOpen()
	    {
	        var e = new EquipOperateClick(43);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.EquipUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            mRemoveBind = false;
	        }
	        else
	        {
	            if (mRemoveBind == false)
	            {
	                RemoveBindEvent();
	            }
	            mRemoveBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (mRemoveBind == false)
	        {
	            RemoveBindEvent();
	        }
	        mRemoveBind = true;
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
	        if (mRemoveBind)
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
	        OnClickInheritTipClose();
	        lockAnimation = false;
	        SuccessEffect.gameObject.SetActive(false);
	        FailEffect.gameObject.SetActive(false);
            FailItemAni.gameObject.SetActive(false);
	        GeneralSuccess.gameObject.SetActive(false);
	
	        if (mRemoveBind)
	        {
	            foreach (var o in OperateFrames)
	            {
	                o.SetActive(false);
	            }
	
	            EventDispatcher.Instance.AddEventListener(EquipUIPackStart.EVENT_TYPE, OnEquipMentUiPackStart);
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	
	            EventDispatcher.Instance.AddEventListener(EquipUiNotifyLogic.EVENT_TYPE, OnEquipMentUiNotify);
                EventDispatcher.Instance.AddEventListener(EquipUiNotifyRefreshCoumuseList.EVENT_TYPE, OnEquipMentUiRefreshConsumeList);
                

                var controllerBase2 = UIManager.Instance.GetController(UIConfig.SmithyUI);
                if (controllerBase2 != null)
                {
                    BindRoot.SetBindDataSource(controllerBase2.GetDataModel(""));
                }


	            var controllerBase = UIManager.Instance.GetController(UIConfig.EquipUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            BindRoot.SetBindDataSource(controllerBase.GetDataModel(""));
	            BindRoot.SetBindDataSource(controllerBase.GetDataModel("EquipPack"));
	            BindRoot.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	            BindRoot.SetBindDataSource(PlayerDataManager.Instance.WeakNoticeData);
	        }
	        mRemoveBind = true;
	        mflag = true;
	//         if (mPack == 0)
	//         {
	//             EquipPack.EquipScroll.SetLookIndex(mStart);
	//         }
	//         else if (mPack == 1)
	//         {
	//             EquipPack.PackScroll.SetLookIndex(mStart);
	//         }
	        ResetAttrLayout();
	        pack = -1;
	        mStart = -1;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }

        private void OnEquipMentUiRefreshConsumeList(IEvent ievent)
        {
            RefreshEnchanceExpend();
        }

        private void OnEquipMentUiNotify(IEvent ievent)
	    {
	        var e = ievent as EquipUiNotifyLogic;
	        switch (e.Type)
	        {
	            case 1:
	            {
	                if (SuccessEffect && SuccessEffect.clip)
	                {
	                    lockAnimation = true;
	                    SuccessEffect.gameObject.SetActive(true);
	                    SuccessEffect[SuccessEffect.clip.name].time = 0;
	                    SuccessEffect.Sample();
	                    SuccessEffect.Play(PlayMode.StopAll);
	                    StartCoroutine(EndAnimation(1.1f, SuccessEffect.gameObject));
	
	                    GeneralSuccess.gameObject.SetActive(true);
	                    //GeneralSuccess.Simulate(0, true, true);
	                    //GeneralSuccess.Play();
	                    StartCoroutine(EndAnimation(1.1f, GeneralSuccess.gameObject));
	
	
	                    if (OperateFrames[4].activeSelf)
	                    {
	                        LightContainer.localPosition = LightPos[1];
	                        GeneralLightContainer.localPosition = LightPos[1];
	                    }
	                    else
	                    {
	                        LightContainer.localPosition = LightPos[0];
	                        GeneralLightContainer.localPosition = LightPos[0];
	                    }
	                }
	            }
	                break;
	            case 2:
	            {
	                if (FailEffect && FailEffect.clip)
	                {
	                    lockAnimation = true;
	                    FailEffect.gameObject.SetActive(true);
	                    FailEffect[FailEffect.clip.name].time = 0;
	                    FailEffect.Sample();
	                    FailEffect.Play(PlayMode.StopAll);
	                    StartCoroutine(EndAnimation(0.5f, FailEffect.gameObject));
	                }

	                if (FailItemAni && FailItemAni.gameObject)
	                {
                        FailItemAni.gameObject.SetActive(true);
                        StartCoroutine(EndAnimation(1.1f, FailItemAni.gameObject));
	                }
	            }
	                break;
	        }
	    }
	
	    private void OnEquipMentUiPackStart(IEvent ievent)
	    {
	        var e = ievent as EquipUIPackStart;
	        mStart = e.Start;
	        pack = e.Pack;
	        if (pack == 0)
	        {
	            EquipPack.EquipScroll.SetLookIndex(mStart);
	        }
	        else if (pack == 1)
	        {
	            EquipPack.PackScroll.SetLookIndex(mStart);
	        }
	        pack = -1;
	        mStart = -1;
	    }
	
	    private void RefresAttribute(Transform attribute)
	    {
	        if (attribute == null)
	        {
	            return;
	        }
	        var activeCount = 0;
	        var attributechildCount1 = attribute.childCount;
	        for (var i = 0; i < attributechildCount1; i++)
	        {
	            var t = attribute.GetChild(i);
	            if (t.gameObject.activeSelf)
	            {
	                activeCount++;
	            }
	        }
	        var activeFlag = 0;
	
	        var attributechildCount2 = attribute.childCount;
	        for (var i = 0; i < attributechildCount2; i++)
	        {
	            var t = attribute.GetChild(i);
	            if (t.gameObject.activeSelf)
	            {
	                t.transform.localPosition = new Vector3(0, -activeFlag*25 + (activeCount - 1)*14 - 30);
	                activeFlag ++;
	            }
	
	            if (EnchangeAttr == attribute)
	            {
	                var ad = t.GetComponent<PropertyDisplayD>();
	                if (ad.ValueLabel.width > EnchangeAttrUpOffset)
	                {
	                    var len = 245/2 + (100 + ad.ValueLabel.width)/2;
	                    ad.UpSprite.transform.localPosition = new Vector3(len, 0, 0);
	                }
	                else
	                {
	                    ad.UpSprite.transform.localPosition = new Vector3(100 + EnchangeAttrUpOffset + 20, 0, 0);
	                }
	            }
	        }
	    }
	
	    private void RefreshEnchanceExpend()
	    {
            //var count = 0;
            //for (var i = 0; i < 4; i++)
            //{
            //    var t = EnchanceConsumes[i];
            //    if (t.gameObject.activeSelf&& t.gameObject.active)
            //    {
            //        count++;
            //    }
            //}
            //count--;
            //if (count < 0 || count >= 4)
            //{
            //    return;
            //}
            //var list = EnchanceConsumesPosition[count];
	
            //var flag = 0;
            //for (var i = 0; i < 4; i++)
            //{
            //    var t = EnchanceConsumes[i];
            //    if (t.gameObject.activeSelf && t.gameObject.active)
            //    {
            //        t.localPosition = list[flag];
            //        flag++;
            //    }
            //}
	    }
	
	    private void RefreshSupperExpend()
	    {
	        var count = 0;
	        for (var i = 0; i < 2; i++)
	        {
	            var t = SupperConsumes[i];
	            if (t.gameObject.activeSelf)
	            {
	                count++;
	            }
	        }
	        count--;
	        if (count < 0 || count >= 2)
	        {
	            return;
	        }
	        var list = SupperConsumesPosition[count];
	
	        var flag = 0;
	        for (var i = 0; i < 2; i++)
	        {
	            var t = SupperConsumes[i];
	            if (t.gameObject.activeSelf)
	            {
	                t.localPosition = list[flag];
	                flag++;
	            }
	        }
	    }
	
	    private void RemoveBindEvent()
	    {
	        BindRoot.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(EquipUIPackStart.EVENT_TYPE, OnEquipMentUiPackStart);
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	        EventDispatcher.Instance.RemoveEventListener(EquipUiNotifyLogic.EVENT_TYPE, OnEquipMentUiNotify);
            EventDispatcher.Instance.RemoveEventListener(EquipUiNotifyRefreshCoumuseList.EVENT_TYPE, OnEquipMentUiRefreshConsumeList);
        }
	
	    private void ResetAttrLayout()
	    {
	        var operateFramesCount0 = OperateMenus.Count;
	        for (var j = 0; j < operateFramesCount0; j++)
	        {
	            var menu = OperateMenus[j].value;
	            if (menu)
	            {
	                var attribute = AttributeFrames[j];
	                RefresAttribute(attribute);
	
	                if (j == 0)
	                {
	                    RefreshEnchanceExpend();
	                }
	                else if (j == 3)
	                {
	                    RefreshSupperExpend();
	                }
	                break;
	            }
	        }
	    }
        public void OnRemoveEvoEquip(BagItemDataModel bagItem)
        {
            EventDispatcher.Instance.DispatchEvent(new RemoveEvoEquipEvent(bagItem));
        }
        public void OnButtonEvoEquip()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(1));
        }

        public void OnButtonEvoEquipUIOk()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(2));
        }


        public void OnButtonSpecailItemShow()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SpecialItemShowEvent());
        }
        
        private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        for (var i = 0; i < 6; i++)
	        {
	            var btn = OperateMenus[i].gameObject.GetComponent<UIButton>();
	            var j = i;
	            btn.onClick.Add(new EventDelegate(() =>
	            {
	                lockAnimation = false;
	                var e = new EquipOperateClick(100);
	                e.Index = j;
	                EventDispatcher.Instance.DispatchEvent(e);
	                var attribute = AttributeFrames[j];
	                RefresAttribute(attribute);
	                if (j == 3)
	                {
	                    RefreshSupperExpend();
	                }
	            }));
	
	            mflag = true;
	        }
	
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
	            mflag = true;
	        }
	    }
	}
}