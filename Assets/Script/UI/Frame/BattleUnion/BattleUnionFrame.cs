using System;
#region using

using System.Collections;
using System.Collections.Generic;
using DataContract;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class BattleUnionFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public List<UIButton> BtattleTabPageClick; //攻城战按钮点击
	    public List<UIButton> BuffButton;
	    public List<UIButton> BuildItemClick; //物品捐赠buttons
	    public UIInput CreateInput;
	    public List<UIButton> DonationButton;
	    public List<UIButton> DonationItemBtns;
	    public GameObject LevelUpAnim;
	    private Coroutine lvUpAnimCoroutine;
	    private GameObject lvUpObj;
	    public UIDragRotate ModelDrag;
	    public CreateFakeCharacter ModelRoot;
	    private bool removeBind = true;
	    public BindDataRoot PlayerBinding;
	    private readonly string ResourcePath = "Effect/UI/UI_JingLingZhenFa.prefab";
	    public List<UIButton> TabPageBtn; //pagebuttons
	    public List<UIButton> TabPageBtn2; //pagebuttons
	
	    #region boss
	
	    public void BtnBossGetReward()
	    {
	        var e = new UIEvent_UnionOperation(19);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    public void BtnClose()
	    {
	        ModelRoot.DestroyFakeCharacter();
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.BattleUnionUI));
	    }
	
	    private IEnumerator DestroyLvUpObj()
	    {
	        yield return new WaitForSeconds(4f);
	        Destroy(lvUpObj);
	        LevelUpAnim.SetActive(false);
	        lvUpObj = null;
	        lvUpAnimCoroutine = null;
	    }
	
	    private void OnCloseUIRemoveBinding(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.BattleUnionUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            removeBind = false;
	        }
	        else
	        {
	            if (removeBind == false)
	            {
	                RemoveBindingEvent();
	            }
	            removeBind = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (removeBind == false)
	        {
	            RemoveBindingEvent();
	        }
	        removeBind = true;
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
	        if (removeBind)
	        {
	            if (ModelRoot.Character)
	            {
	                ModelRoot.Character.gameObject.SetActive(false);
	            }
	            RemoveBindingEvent();
	        }
	        LevelUpAnim.SetActive(false);
	        if (lvUpObj != null && lvUpAnimCoroutine != null)
	        {
	            Destroy(lvUpObj);
	            LevelUpAnim.SetActive(false);
	            lvUpObj = null;
	            lvUpAnimCoroutine = null;
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
	        if (removeBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUIRemoveBinding);
	            EventDispatcher.Instance.AddEventListener(UIEvent_UnionAnim.EVENT_TYPE, UnionAnimationControl);
	            EventDispatcher.Instance.AddEventListener(BattleUnionRefreshModelViewLogic.EVENT_TYPE, OnRefreshRankModelView);
	            var controllerBase = UIManager.Instance.GetController(UIConfig.BattleUnionUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel("BattleData"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Info"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Build"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Buff"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Boss"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("Shop"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("OtherUnion"));
	            Binding.SetBindDataSource(controllerBase.GetDataModel("AttackCity"));
	
	            PlayerBinding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	            PlayerBinding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	        }
	        removeBind = true;
	        if (ModelRoot.Character)
	        {
	            ModelRoot.Character.gameObject.SetActive(true);
	        }
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnRefreshRankModelView(IEvent ievent)
	    {
	        var e = ievent as BattleUnionRefreshModelViewLogic;
	        var info = e.Info;
	        ModelRoot.DestroyFakeCharacter();
	        ItemBaseData elfData = null;
	        var elfId = -1;
	        if (info.Equips.ItemsChange.TryGetValue((int) eBagType.Elf, out elfData))
	        {
	            elfId = elfData.ItemId;
	        }
	        ModelRoot.Create(info.TypeId, info.EquipsModel, character => { ModelDrag.Target = character.transform; }, elfId,
	            true);
	    }
	
	    private void RemoveBindingEvent()
	    {
	        Binding.RemoveBinding();
	        PlayerBinding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUIRemoveBinding);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_UnionAnim.EVENT_TYPE, UnionAnimationControl);
	        EventDispatcher.Instance.RemoveEventListener(BattleUnionRefreshModelViewLogic.EVENT_TYPE, OnRefreshRankModelView);
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var DonationButtonCount0 = DonationButton.Count;
	        for (var i = 0; i < DonationButtonCount0; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { ButtonDonation(j); });
	            DonationButton[i].onClick.Add(deleget);
	        }
	        for (var i = 0; i < DonationItemBtns.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { DonationItem(j); });
	            DonationItemBtns[i].onClick.Add(deleget);
	        }
	
	
	        var BuffButtonCount1 = BuffButton.Count;
	        for (var i = 0; i < BuffButtonCount1; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { ButtonBuffIconClick(j); });
	            BuffButton[i].onClick.Add(deleget);
	        }
	
	        var BuffButtonCount2 = BuffButton.Count;
	        for (var i = 0; i < BuffButtonCount2; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { DonationItemClick(j); });
	            BuildItemClick[i].onClick.Add(deleget);
	        }
	        for (var i = 0; i < TabPageBtn.Count; i++)
	        {
                if (TabPageBtn[i] == null)
	            {
	                continue;
	            }
	            var j = i;
	            var deleget = new EventDelegate(() => { TabPageBtnClick(j); });
	            TabPageBtn[i].onClick.Add(deleget);
	        }
	        for (var i = 0; i < TabPageBtn2.Count; i++)
	        {
                if (TabPageBtn2[i] == null)
                {
                    continue;
                }
	            var j = i;
	            var deleget = new EventDelegate(() => { TabPageBtnClick2(j); });
	            TabPageBtn2[i].onClick.Add(deleget);
	        }
	
	        for (var i = 0; i < BtattleTabPageClick.Count; i++)
	        {
                if (BtattleTabPageClick[i] == null)
                {
                    continue;
                }
	            var j = i;
	            var deleget = new EventDelegate(() => { BattleTabPageBtnClick(j); });
	            BtattleTabPageClick[i].onClick.Add(deleget);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void UnionAnimationControl(IEvent ievent)
	    {
	        if (lvUpObj != null)
	        {
	            LevelUpAnim.SetActive(false);
	            LevelUpAnim.SetActive(true);
	
	            if (lvUpAnimCoroutine == null)
	            {
	                lvUpAnimCoroutine = StartCoroutine(DestroyLvUpObj());
	            }
	        }
	        else
	        {
	            LevelUpAnim.SetActive(true);
	
	            ResourceManager.PrepareResource<GameObject>(ResourcePath, (res) =>
	            {
	                lvUpObj = Instantiate(res) as GameObject;
	                if (lvUpObj != null)
	                {
	                    var trans = lvUpObj.transform;
	                    trans.SetParentEX(LevelUpAnim.transform);
	                    trans.localPosition = Vector3.zero;
	                    trans.localScale = Vector3.one;
	                }
	            });
	
	            if (lvUpAnimCoroutine == null)
	            {
	                lvUpAnimCoroutine = StartCoroutine(DestroyLvUpObj());
	            }
	        }
	
	
	    }
	
	    // Update is called once per frame
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
	
	    #region 战盟创建
	
	    public void BtnCreateUnion()
	    {
	        var e = new UIEvent_UnionBtnCreateUnion();
	        e.Name = CreateInput.value;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnOtherUnion()
	    {
	        var e = new UIEvent_UnionOperation(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnInputForcus()
	    {
	        var e = new UIEvent_UnionOperation(31);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnInputLostForcus()
	    {
	        var e = new UIEvent_UnionOperation(32);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region 其他战盟
	
	    public void BtnApplyJoin()
	    {
	        var e = new UIEvent_UnionOperation(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnOtherReturn()
	    {
	        var e = new UIEvent_UnionOperation(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region 战盟信息
	
	    public void BtnApplyList()
	    {
	        var e = new UIEvent_UnionOperation(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //public void BtnMemberInfo()
	    //{
	    //    UIEvent_UnionOperation e = new UIEvent_UnionOperation(4);
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	
	    public void BtnAddMember()
	    {
	        var e = new UIEvent_UnionOperation(5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void FindMemberOK()
	    {
	        var e = new UIEvent_UnionOperation(6);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void FindUIClose()
	    {
	        var e = new UIEvent_UnionOperation(7);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPassApply()
	    {
	        var e = new UIEvent_UnionBtnPassApply();
	        e.Type = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnDefuseApply()
	    {
	        var e = new UIEvent_UnionBtnPassApply();
	        e.Type = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnInfoReturn()
	    {
	        var e = new UIEvent_UnionOperation(8);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnModifyNotice()
	    {
	        var e = new UIEvent_UnionOperation(10);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnDetailBack()
	    {
	        var e = new UIEvent_UnionOperation(9);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnSetAutoAccept()
	    {
	        var e = new UIEvent_BattleBtnAutoAccept();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void NoticeSaveShow()
	    {
	        var e = new UIEvent_UnionOperation(21);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region 战盟建设
	
	    public void ButtonDonation(int index)
	    {
	        var e = new UIEvent_UnionBtnDonation();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void DonationItem(int index)
	    {
	        var e = new UIEvent_UnionDonationItem();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnSeeLog()
	    {
	        var e = new UIEvent_UnionOperation(11);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnCloseSeeLog()
	    {
	        var e = new UIEvent_UnionOperation(12);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnDonationItem()
	    {
	        var e = new UIEvent_UnionOperation(13);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void DonationItemClick(int index)
	    {
	        var e = new UIEvent_UnionDonationItemClick();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BuildShowHelp()
	    {
	        var e = new UIEvent_UnionOperation(22);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BuildShowGoldPage()
	    {
	        var e = new UIEvent_UnionOperation(23);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BuildShowItemPage()
	    {
	        var e = new UIEvent_UnionOperation(24);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region 战盟升级
	
	    public void BtnUnionBuffUpShow()
	    {
	        var e = new UIEvent_UnionOperation(25);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void ButtonBuffIconClick(int index)
	    {
	        var e = new UIEvent_UnionBuffUpShow();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnUnionBuffActive()
	    {
	        var e = new UIEvent_UnionOperation(15);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuffUpOK()
	    {
	        var e = new UIEvent_UnionOperation(16);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnUnionLevelup()
	    {
	        var e = new UIEvent_UnionOperation(17);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnUpBuffCancel()
	    {
	        var e = new UIEvent_UnionOperation(18);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region Tab事件
	
	    public void TabOutUnion()
	    {
	        var e = new UIEvent_UnionOperation(20);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void TabPageBtnClick(int index)
	    {
	        var e = new UIEvent_UnionTabPageClick();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void TabPageBtnClick2(int index)
	    {
	        var e = new UIEvent_UnionTabPageClick2();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region 攻城战
	
	    public void BattleTabPageBtnClick(int index)
	    {
	        var e = new UIEvent_UnionBattlePageCLick();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BattleTabPageClose(int index)
	    {
	        var e = new UIEvent_UnionBattlePageCLick();
	        e.Index = -1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BattleAddBidding()
	    {
	        var e = new UIEvent_UnionOperation(26);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void AttackJoin()
	    {
	        var e = new UIEvent_UnionOperation(27);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BattleBiddingOk()
	    {
	        var e = new UIEvent_UnionOperation(28);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BattleBiddingClose()
	    {
	        var e = new UIEvent_UnionOperation(29);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBiddingSub()
	    {
	        var e = new UIEvent_UnionOperation(30);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBiddingAdd()
	    {
	        var e = new BattleUnionCountChange(0, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBiddingDel()
	    {
	        var e = new BattleUnionCountChange(0, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBiddingAddPress()
	    {
	        var e = new BattleUnionCountChange(0, 2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBiddingAddRelease()
	    {
	        var e = new BattleUnionCountChange(0, 4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBiddingDelPress()
	    {
	        var e = new BattleUnionCountChange(0, 3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickBiddingDelRelease()
	    {
	        var e = new BattleUnionCountChange(0, 5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	
	    public void OnTimerFormat(UILabel lable)
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
	            ret = GameUtils.GetTimeDiffString(time);
	            //   var dif = time - Game.Instance.ServerTime;
	            //   ret = string.Format("{0}:{1}", dif.Minutes, dif.Seconds);
	        }
	        lable.text = ret;
	    }
	
	    #endregion
	}
}