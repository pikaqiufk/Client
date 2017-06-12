using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;
using DataTable;
#endregion

namespace GameUI
{
	public class VoyageFrame : MonoBehaviour //,IChainRoot,IChainListener
	{
	    public List<UIEventTrigger> AddSpeedButton;
        public List<UIEventTrigger> AccessBtns;
	    public BindDataRoot Binding;
	    public List<UIButton> EquipButtons;
	    public List<GameObject> FlyFromObjs;
	    public List<UIEventTrigger> GetButton;
	    public GameObject HomeObj;
	    //  public GameObject labelParent;
	    // public StackLayout AttributeLayout;
	    public UISpriteAnimation LevelUpAnimation;
	    public List<UIEventTrigger> LightPoint;
	    public List<UIEventTrigger> LineButton;
	    private GameObject mFlyPrefabExp;
	    private GameObject MyPrefab;
	    public GameObject ScrollViewTempBag;
	    public List<GameObject> Ships;
	    //线的点击
	    public void AutoShipClick()
	    {
	        var e = new UIEvent_SailingOperation(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void CloseWoodTips()
	    {
	        var e = new UIEvent_SailingOperation(8);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
// 	    private void ExpFlyAnim(IEvent ievent)
// 	    {
// 	        var e = ievent as UIEvent_SailingFlyAnim;
// 	        var obj = Instantiate(mFlyPrefabExp) as GameObject;
// 	        if (e.Idx >= 0 && e.Idx < FlyFromObjs.Count)
// 	        {
// 	            var mTrans = FlyFromObjs[e.Idx].transform;
// 	            PlayerDataManager.Instance.PlayFlyItem(obj, mTrans, HomeObj.transform, 12, e.Exp);
// 	        }
// 	    }
	
	    public void LevelBackClick()
	    {
	        var e = new UIEvent_SailingOperation(5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void LevelUpClick()
	    {
	        var e = new UIEvent_SailingOperation(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //亮点的点击
	    public void LightPointClick(int index)
	    {
	        if (index == 5)
	        {
	            var e = new UIEvent_SailingLineButton(4);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	        else
	        {
	            var e = new UIEvent_SailingLightPoint(index);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
        public void OnClickAccess(int index)
        {
            var tb = Table.GetSailing(index);
            if(tb == null || tb.CanCall <= 0)
                return;

            if (PlayerDataManager.Instance.GetItemCount(tb.NeedType) < tb.ItemCount)
            {
                EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(210102));
                return;
            }
            string str = string.Format(GameUtils.GetDictionaryText(100002105), tb.ItemCount);
            UIManager.Instance.ShowMessage(MessageBoxType.OkCancel, str, "", () =>
            {
                EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingLightPointAccess(index));
            });            



        }	
	    //线的点击
	    public void LineButtonClick(int index)
	    {
	        var e = new UIEvent_SailingLineButton(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickArrangeBtn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingOperation(0));
	    }
	
	    public void OnClickBtnBack()
	    {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingReturnBtn(1));
            //var e = new Close_UI_Event(UIConfig.SailingUI);
            //EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.SailingUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnclickEatAll()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingOperation(1));
	    }
	
	    public void OnClickEquipItem(int index)
	    {
	        var e = new UIEvent_SailingPackItemUI();
	        e.PutOnOrOff = 0;
	        e.Index = index;
	        e.BagId = (int) eBagType.MedalUsed;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPickAll()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingPickAll());
	    }
	
	    public void OnClickReturnBtn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingReturnBtn(1));
	    }
	
	    public void OnClickReturnShipBtn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingReturnBtn(0));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_SailingPlayAnimation.EVENT_TYPE, PlayLevelUpAnimation);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_SailingPlayEatAnim.EVENT_TYPE, PlayEatAllAnim);
	        //EventDispatcher.Instance.RemoveEventListener(UIEvent_SailingFlyAnim.EVENT_TYPE, ExpFlyAnim);
	
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingOperation(6));
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.SailingUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        //EventDispatcher.Instance.DispatchEvent(new UIEvent_SailingReturnBtn(1));
	        EventDispatcher.Instance.AddEventListener(UIEvent_SailingPlayAnimation.EVENT_TYPE, PlayLevelUpAnimation);
	        EventDispatcher.Instance.AddEventListener(UIEvent_SailingPlayEatAnim.EVENT_TYPE, PlayEatAllAnim);
	        //EventDispatcher.Instance.AddEventListener(UIEvent_SailingFlyAnim.EVENT_TYPE, ExpFlyAnim);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void PlayEatAllAnim(IEvent ievent)
	    {
	        var e = ievent as UIEvent_SailingPlayEatAnim;
	        var List = e.List;
	        var mTime = 0.1f;
	        if (ScrollViewTempBag != null)
	        {
	            var to = ScrollViewTempBag.transform.GetChild(e.Index);
	            for (var i = 0; i < List.Count; i++)
	            {
	                var item = ScrollViewTempBag.transform.GetChild(List[i]);
	
	                //var itemData = new ItemIdDataModel();
	                //itemData.ItemId = e.ItemIds[i];
	                //var move = Instantiate(ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFrameCellAnim.prefab")) as GameObject;
	                ////var move = ComplexObjectPool.NewObjectSync("UI/Icon/IconIdFrameCellAnim.prefab");
	                ////move.GetComponent<UIWidget>().Invalidate(true);
	
	
	                //var moveTrans = move.transform;
	                //moveTrans.parent = item.transform.parent;
	                //moveTrans.localScale = Vector3.one;
	                //move.SetDataSource(itemData);
	
	                //var curve = move.GetComponent<CurveMove>();
	                //curve.From = item.localPosition;
	                //curve.To = to.localPosition;
	                //curve.HighPostion = new Vector3(0f, MyRandom.Random(60, 100), 0f);
	                //curve.Play();
	                var obj = Instantiate(MyPrefab) as GameObject;
	                mTime += 0.2f;
	                PlayerDataManager.Instance.PlayFlyItem(obj, item, to, e.ItemIds[i], 0, new Vector3(0, 0, 0), mTime,
	                    new Vector3(0, 50, 0));
	            }
	        }
	    }
	
	    //void RefreshAttrPanel(IEvent unuse)
	    //{
	    //    IControllerBase controllerBase = UIManager.Instance.GetController(UIConfig.SailingUI);
	    //    SailingDataModel infoData = controllerBase.GetDataModel("") as SailingDataModel;
	    //    var labels = labelParent.GetComponentsInChildren<UILabel>();
	    //    int i = 0;
	    //    foreach (var label in labels)
	    //    {
	    //        if (i < infoData.TempData.ItemPropUI.Count)
	    //        {
	    //            var attr = infoData.TempData.ItemPropUI[i];
	    //            label.text = string.Format("{0}    +{1}", attr.AttrName, attr.AttrValue);
	    //        }
	    //        else
	    //        {
	    //            label.text = string.Empty;
	    //        }
	    //        ++i;
	    //    }
	    //}
	
	    //public void Listen<T>(T message)
	    //{
	    //    if (message is string && (message as string) == "ActiveChanged")
	    //    {
	    //        AttributeLayout.ResetLayout();
	    //    }
	    //}
	
	    public void PlayLevelUpAnimation(IEvent ievent)
	    {
	        LevelUpAnimation.Play();
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var OperateButtonCount0 = EquipButtons.Count;
	        for (var i = 0; i < OperateButtonCount0; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { OnClickEquipItem(j); });
	            EquipButtons[i].onClick.Add(deleget);
	        }
	
	        var LightPointCount1 = LightPoint.Count;
	        for (var i = 0; i < LightPointCount1; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { LightPointClick(j); });
	            LightPoint[i].onClick.Add(deleget);
	        }
	        var LineButtonCount2 = AddSpeedButton.Count;
	        for (var i = 0; i < LineButtonCount2; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { LineButtonClick(j); });
	            AddSpeedButton[i].onClick.Add(deleget);
	        }
	        var LineButtonCoun3 = GetButton.Count;
	        for (var i = 0; i < LineButtonCoun3; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { LineButtonClick(j); });
	            GetButton[i].onClick.Add(deleget);
	        }
	        var LineButtonCoun4 = LineButton.Count;
	        for (var i = 0; i < LineButtonCoun4; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { LineButtonClick(j); });
	            LineButton[i].onClick.Add(deleget);
	        }
            var AccessBtnCount = AccessBtns.Count;
            for (var i = 0; i < AccessBtnCount;i++ )
            {
                var j = i;
                var deleget = new EventDelegate(() => { OnClickAccess(j); });
                AccessBtns[i].onClick.Add(deleget);
            }
            MyPrefab = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFlySailing.prefab");
	        mFlyPrefabExp = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    // Update is called once per frame
	    private void Update()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        // RefreshAttrPanel(null);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void WoodTipsOk()
	    {
	        var e = new UIEvent_SailingOperation(7);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}