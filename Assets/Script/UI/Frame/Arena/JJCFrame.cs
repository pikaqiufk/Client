using System;
#region using

using System.Collections.Generic;
using DataContract;
using EventSystem;
using SignalChain;
using UnityEngine;

#endregion

namespace GameUI
{
	public class JJCFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public GameObject ArenaInfo;
	    public GameObject AttributeList;
	    public GameObject BackBtn;
	    public BindDataRoot Binding;
	    public GameObject FightRecord;
	    public List<GameObject> FlyGameObjects;
	    public GameObject HomeObj;
	    private bool flag;
	    private GameObject flyExpPrefab;
	    private bool deleteBind = true;
	    public List<UIButton> OpponentBtns;
	    public List<UIEventTrigger> OpponentInfos;
	    public GameObject OpponentList;
	    public GameObject PetList;
	    public GameObject RankHonorList;
	    public UIScrollViewSimple RankHonorScroll;
	    public GameObject RuleDesc;
	    public UIWidget StatueAttributeBackground;
	    public GameObject StatueInfo;
	    public UILabel StatusCd;
	    public List<UIEventTrigger> StatusTriggers;
	    public GameObject TabBtn;
	
	    private void JJCFlyAnim(IEvent ievent)
	    {
	        var e = ievent as UIEvent_ArenaFlyAnim;
	        var obj = Instantiate(flyExpPrefab) as GameObject;
	        if (e.Idx >= 0 && e.Idx < FlyGameObjects.Count)
	        {
	            PlayerDataManager.Instance.PlayFlyItem(obj, FlyGameObjects[e.Idx].transform, HomeObj.transform, 12, e.Count);
	        }
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (flag)
	        {
	            flag = false;
	            //StatueAttributeLayout.ResetLayout();
	            //var transform = StatueAttributeLayout.transform;
	            //var pos = transform.localPosition;
	            //var h = StatueAttributeLayout.height - StatueAttributeBackground.height / 2 + 30.0f;
	            //transform.localPosition = new Vector3(pos.x, h, pos.z);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnNotifyJJCLogic(IEvent ievent)
	    {
	        var e = ievent as ArenaNotifyLogic;
	        switch (e.Type)
	        {
	            case 0:
	            {
	//重置一些页面状态
                    ShowOpponentList();
	            }
	                break;
	            case 1:
	            {
	//打开荣誉兑换界面，并显示相应的位置
	                ShowExchangeHonor(e.Index);
	            }
	                break;
	        }
	    }
	
	    private void OnJJCSatueCell(IEvent ievent)
	    {
            OnTogglePetList();
	    }
	
	    public void OnClickBtnBack()
	    {
	        var e = new Close_UI_Event(UIConfig.AreanaUI, true);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnClose()
	    {
	        var e = new Close_UI_Event(UIConfig.AreanaUI, false);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnDesc()
	    {
	        RuleDesc.gameObject.SetActive(true);
	    }
	
	    public void OnClickBtnDescClose()
	    {
	        RuleDesc.gameObject.SetActive(false);
	    }
	
	    //军衔打开事件
	    public void OnClickBtnExchange()
	    {
	        if (RankHonorList.activeSelf)
	        {
	            return;
	        }
	        var e = new ArenaOperateEvent(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnPromotionRank()
	    {
	        var e = new UIEvent_Promotion_Rank(true);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnRank()
	    {
	        var e = new Show_UI_Event(UIConfig.RankUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnRecord()
	    {
	        FightRecord.gameObject.SetActive(true);
	    }
	
	    public void OnClickBtnRecordClose()
	    {
	        FightRecord.gameObject.SetActive(false);
	    }
	
	    public void OnClickBtnRefreshOpponent()
	    {
	        var e = new ArenaOperateEvent(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBtnShop()
	    {
	        var arg = new StoreArguments {Tab = 105};
	        var e = new Show_UI_Event(UIConfig.StoreEquip, arg);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        //潜规则引导标记位
	        if (!PlayerDataManager.Instance.GetFlag(532))
	        {
	            var list = new Int32Array();
	            list.Items.Add(532);
	
	            var list1 = new Int32Array();
	            list1.Items.Add(531);
	            PlayerDataManager.Instance.SetFlagNet(list, list1);
	        }
	    }
	
	    public void OnClickBuyCool()
	    {
	        var e = new SatueOperateEvent(13);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickCleanShow()
	    {
	        var e = new SatueOperateEvent(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClosePetSkill()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SeeSkills(null, false));
	    }
	
	    public void OnClickLeft()
	    {
	        var e = new SatueOperateEvent(41);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOpponentFightBtn(int index)
	    {
	        var e = new AreanOppentCellClick(0, index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOpponentInfoBtn(int index)
	    {
	        var evt = OpponentInfos[index];
	        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	        var localPos = evt.transform.root.InverseTransformPoint(worldPos);
	        UIConfig.OperationList.Loction = localPos;
	
	        var e = new AreanOppentCellClick(1, index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPuzzelShow()
	    {
	        var e = new SatueOperateEvent(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickRgiht()
	    {
	        var e = new SatueOperateEvent(42);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickStatu(int index)
	    {
	        var e = new SatueOperateEvent(100, index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickWirship()
	    {
	        var e = new SatueOperateEvent(20);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseRemoveBinding(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.AreanaUI)
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
	                DeleteBindEvent();
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
	            DeleteBindEvent();
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
	            DeleteBindEvent();
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
	        RuleDesc.gameObject.SetActive(false);
	        FightRecord.gameObject.SetActive(false);
	
	        RankHonorList.SetActive(false);
	        OpponentList.SetActive(true);
	        RuleDesc.SetActive(false);
	        if (deleteBind)
	        {
	            SetVisibleMenu(true);
	            var controllerBase = UIManager.Instance.GetController(UIConfig.AreanaUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Arena"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Statue"));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	
	            EventDispatcher.Instance.AddEventListener(ArenaSatueCellClick.EVENT_TYPE, OnJJCSatueCell);
	            EventDispatcher.Instance.AddEventListener(SatueNotifyEvent.EVENT_TYPE, OnNotifySatue);
	            EventDispatcher.Instance.AddEventListener(ArenaNotifyLogic.EVENT_TYPE, OnNotifyJJCLogic);
	            EventDispatcher.Instance.AddEventListener(UIEvent_ArenaFlyAnim.EVENT_TYPE, JJCFlyAnim);
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseRemoveBinding);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnFormatFightCd(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var time = timer.TargetTime;
	
	        var ret = "";
	        if (time < Game.Instance.ServerTime)
	        {
	            lable.gameObject.SetActive(false);
	        }
	        else
	        {
	            var dif = time - Game.Instance.ServerTime;
	            ret = string.Format("{0}:{1}", dif.Minutes, dif.Seconds);
	        }
	        lable.text = ret;
	    }
	
	    //---------------------------------------------Status--------------
	    public void OnFormatStatusCd(UILabel lable)
	    {
	        if (lable == null)
	        {
	            return;
	        }
	        var timer = lable.GetComponent<TimerLogic>();
	        if (timer == null)
	        {
	            return;
	        }
	        var target = timer.TargetTime;
	        if (target > Game.Instance.ServerTime)
	        {
	            //if (StatusCd.gameObject.activeSelf == false)
	            //{
	            //    StatusCd.gameObject.SetActive(true);
	            //}
	
	            var str = GameUtils.GetTimeDiffString(target);
	            lable.text = str;
	        }
	        else
	        {
	            var e = new SatueOperateEvent(30);
	            EventDispatcher.Instance.DispatchEvent(e);
	            //   StatusCd.gameObject.SetActive(false);
	        }
	    }
	
	    private void OnNotifySatue(IEvent ievent)
	    {
	        //SatueNotifyEvent e = ievent as SatueNotifyEvent;
	        //switch (e.Type)
	        //{
	        //    case 1:
	        //    {
	        //        OnShowClean();
	        //    }
	        //        break;
	        //    case 2:
	        //    {
	        //        OnShowPuzzel();
	        //    }
	        //        break;
	        //}
	    }
	
	    public void OnTogglePetList()
	    {
	        var show = !PetList.activeSelf;
	        var e = new ArenaPetListEvent(show);
	        EventDispatcher.Instance.DispatchEvent(e);
	        PetList.SetActive(show);
	        AttributeList.gameObject.SetActive(!show);
	    }
	
	    private void DeleteBindEvent()
	    {
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(ArenaSatueCellClick.EVENT_TYPE, OnJJCSatueCell);
	        EventDispatcher.Instance.RemoveEventListener(SatueNotifyEvent.EVENT_TYPE, OnNotifySatue);
	        EventDispatcher.Instance.RemoveEventListener(ArenaNotifyLogic.EVENT_TYPE, OnNotifyJJCLogic);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_ArenaFlyAnim.EVENT_TYPE, JJCFlyAnim);
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseRemoveBinding);
	    }
	
	    private void SetVisibleMenu(bool isShow)
	    {
	        TabBtn.SetActive(isShow);
	        BackBtn.SetActive(isShow);
	    }
	
	    private void ShowExchangeHonor(int index)
	    {
	        RankHonorList.SetActive(true);
	        OpponentList.SetActive(false);
	        RankHonorScroll.SetLookIndex(index);
	    }
	
	    public void ShowOpponentList()
	    {
	        if (OpponentList.activeSelf)
	        {
	            return;
	        }
	        RankHonorList.SetActive(false);
	        OpponentList.SetActive(true);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        for (var i = 0; i < 3; i++)
	        {
	            var j = i;
	            var btn = OpponentBtns[i];
                btn.onClick.Add(new EventDelegate(() => { OnClickOpponentFightBtn(j); }));
	            var evt = OpponentInfos[i];
                evt.onClick.Add(new EventDelegate(() => { OnClickOpponentInfoBtn(j); }));
	        }
	
	        var c = StatusTriggers.Count;
	        for (var i = 0; i < c; i++)
	        {
	            var t = StatusTriggers[i];
	            if (t != null)
	            {
	                var j = i;
	                t.onClick.Add(new EventDelegate(() => { OnClickStatu(j); }));
	            }
	        }
	        PetList.gameObject.SetActive(false);
	        AttributeList.gameObject.SetActive(true);
	
	        RankHonorList.SetActive(false);
	        OpponentList.SetActive(true);
	
	        if (TabBtn.activeSelf == false)
	        {
	            if (ArenaInfo.activeSelf == false)
	            {
	                ArenaInfo.SetActive(true);
	            }
	            if (StatueInfo.activeSelf)
	            {
	                StatueInfo.SetActive(false);
	            }
	        }
	        flyExpPrefab = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
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
	            flag = true;
	        }
	    }
	}
}