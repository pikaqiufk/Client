using System;
#region using

using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BlacksmithShopFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public UIGridSimple EvoResourceGrid;
	    public List<GameObject> FlyGameObjects;
	    public GameObject HomeObj;
	    private IControllerBase controller;
	    private GameObject fyPrefabExp;
	    public List<UIEventTrigger> TabButtons;
	    public GameObject Tip;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        fyPrefabExp = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
	        for (var i = 0; i < TabButtons.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { TabPageButton(j); });
	            TabButtons[i].onClick.Add(deleget);
	        }
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void CloseTip()
	    {
	        Tip.SetActive(false);
	    }
	
	    public void OnButtonCastDone()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(0));
	    }
	
	    public void OnButtonClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(3));
	    }
	
	    public void OnButtonEvoEquip()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(1));
	    }
	
	    public void OnButtonEvoEquipUIOk()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithyBtnClickedEvent(2));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_SmithyFlyAnim.EVENT_TYPE, BlacksmithFlyAnim);
	        Binding.RemoveBinding();
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
	        controller = UIManager.Instance.GetController(UIConfig.SmithyUI);
	        if (controller == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controller.GetDataModel(""));
	        Binding.SetBindDataSource(controller.GetDataModel("EquipPack"));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        EventDispatcher.Instance.AddEventListener(UIEvent_SmithyFlyAnim.EVENT_TYPE, BlacksmithFlyAnim);
	        Tip.SetActive(false);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnRemoveEvoEquip(BagItemDataModel bagItem)
	    {
	        EventDispatcher.Instance.DispatchEvent(new RemoveEvoEquipEvent(bagItem));
	    }
	
	    public void ShowTip()
	    {
	        Tip.SetActive(true);
	    }

        public void OnClickOpenCompose()
        {
            var e = new Show_UI_Event(UIConfig.ComposeUI);
            EventDispatcher.Instance.DispatchEvent(e);
        }


	    //public UIGridSimple FlySimpleObj;
	    private void BlacksmithFlyAnim(IEvent ievent)
	    {
	        var e = ievent as UIEvent_SmithyFlyAnim; //0?????  //1?????
	        var obj = Instantiate(fyPrefabExp) as GameObject;
	        if (e.Idx == 0)
	        {
	            //var from = FlySimpleObj.transform.GetChild(e.Index);
	            //FlyGameObjects[0].transform.position = from.position;
	            //FlyGameObjects[0].transform.localPosition+=new Vector3(0f,20f,0f);
	            PlayerDataManager.Instance.PlayFlyItem(obj, FlyGameObjects[0].transform, HomeObj.transform, 12, e.Count);
	        }
	        else if (e.Idx == 1)
	        {
	            PlayerDataManager.Instance.PlayFlyItem(obj, FlyGameObjects[1].transform, HomeObj.transform, 12, e.Count);
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
	
	    private void TabPageButton(int index)
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SmithTabPageEvent(index));
	    }
	}
}