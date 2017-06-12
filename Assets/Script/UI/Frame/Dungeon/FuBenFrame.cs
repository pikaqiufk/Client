
#region using

using System.Collections;
using System.Collections.Generic;
using EventSystem;
using UnityEngine;
using DataTable;
using System;
using Shared;
#endregion

namespace GameUI
{
	public class FuBenFrame : MonoBehaviour
	{
	    public List<GameObject> AwardBack;
	    public List<GameObject> AwardFront;
	    public List<GameObject> AwardList;
	    public BindDataRoot Binding;
	    public List<UIButton> DungeonList;
	    public List<UIButton> DungeonSelects;
	    public GameObject MainInfo;
	    public GameObject MainScroll;
	    //public GameObject MainSweep;
	    public BoxCollider MainSweepCollider;
	    private bool mRemoveBind = true;
	    public GameObject ScrollContainer;
	    public GameObject SweepBack;
        public GameObject SweepBtn;
	    public GameObject SweepConfirm;
	    public GameObject TeamInfo;
	    public GameObject TeamScroll;
	    public GameObject VipInfo;
	    private void InitFuBenSelect()
	    {
	        for (var i = 0; i < DungeonSelects.Count; i++)
	        {
	            var item = DungeonSelects[i];
	            var toggle = item.GetComponent<UIToggle>();
	            if (!toggle)
	            {
	                continue;
	            }
	            if (i == 0)
	            {
	                toggle.Set(true);
	            }
	            else
	            {
	                toggle.Set(false);
	            }
	        }
	    }
	
	    //------------------------------------------------------------------------Sweep----
	    private void OnClickAward(int index)
	    {
	        var e = new DungeonSweepRandAward(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	
	        var awardListCount5 = AwardList.Count;
	        for (var i = 0; i < awardListCount5; i++)
	        {
	            var o = AwardList[i];
	            o.GetComponent<BoxCollider>().enabled = false;

                var item = o.GetComponentInChildren<IconFrame>();
                item.GetComponent<BoxCollider>().enabled = (i == index);
	            if (i == index)
	            {
	                var tweens = o.GetComponentsInChildren<TweenRotation>(true);
	                {
	                    var array4 = tweens;
	                    // var arrayLength4 = array4.Length;
	
	                    //for (int i4 = 0; i4 < arrayLength4; ++i4)
	                    {
	                        var position = array4[0];
	                        {
	                            position.ResetForPlay();
	                            position.PlayForward();
	                        }
	                    }
	                }
	            }
	        }
//	        StartCoroutine(ShowOtherAward(index));
	    }
	
	    public void OnClickBtnClose()
	    {
	        //         var dungeonTypesCount4 = DungeonList.Count;
	        //         for (int i = 0; i < dungeonTypesCount4; i++)
	        //         {
	        //             var toggle = DungeonList[i];
	        //             if (i == 0)
	        //             {
	        //                 toggle.value = true;
	        //                 toggle.mIsActive = true;
	        //                 if (toggle.activeSprite != null)
	        //                 {
	        //                     toggle.activeSprite.alpha = 1.0f;
	        //                 }
	        //             }
	        //             else
	        //             {
	        //                 toggle.value = false;
	        //                 toggle.mIsActive = false;
	        //                 if (toggle.activeSprite != null)
	        //                 {
	        //                     toggle.activeSprite.alpha = 0.0f;
	        //                 }
	        //             }
	        //         }
	
	        var e = new Close_UI_Event(UIConfig.DungeonUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnClickFuBenGroupCell(IEvent ievent)
	    {
	        var e = ievent as DungeonGroupCellClick;
	        InitFuBenSelect();
	
	        var type = 0;
	        var toggle = DungeonList[1].GetComponent<UIToggle>();
	        if (toggle.value)
	        {
	            type = 1;
	        }

            var toggle2 = DungeonList[2].GetComponent<UIToggle>();
            if (toggle2.value)
            {
                type = 2;
            }

	        var ee = new DungeonGroupCellClick2(type, e.Index);
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }
	
	    public void OnClickDungeonSweepClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new DungeonSetScan(0));
	        //MainSweep.SetActive(false);
	    }
	
	    public void OnClickEnterMainDungeon()
	    {
	        var e = new DungeonBtnClick(1, eDungeonType.Main);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }

	    public void OnClickEnterVipDungen()
	    {
            var e = new DungeonBtnClick(1, eDungeonType.Vip);
            EventDispatcher.Instance.DispatchEvent(e);
	    }
	    //-----------------------------------------------------------------------Main----
	    public void OnClickMainDungeon()
	    {
	//         ScrollContainer.SetActive(true);
	//         MainScroll.SetActive(true);
	//         TeamScroll.SetActive(false);
	        var ee = new DungeonGroupCellClick2(0, 0);
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }
	
	    public void OnClickMainDungeonReset()
	    {
	        var e = new DungeonBtnClick(2, eDungeonType.Main);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMainDungeonSweep()
	    {
	        {
	            var __list1 = AwardList;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var o = __list1[__i1];
	                {
	                    o.transform.localRotation = Quaternion.Euler(0, 0, 0);
	                    o.GetComponent<BoxCollider>().enabled = true;
	                    var tweens = o.GetComponentsInChildren<TweenRotation>(true);
	                    {
	                        var __array6 = tweens;
	                        var __arrayLength6 = __array6.Length;
	                        for (var __i6 = 0; __i6 < __arrayLength6; ++__i6)
	                        {
	                            var position = __array6[__i6];
	                            {
	                                position.enabled = false;
	                            }
	                        }
	                    }
	                }
	            }
	        }
	        {
	            var __list2 = AwardBack;
	            var __listCount2 = __list2.Count;
	            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	            {
	                var o = __list2[__i2];
	                {
	                    o.gameObject.SetActive(true);
	                }
	            }
	        }
	        {
	            var __list3 = AwardFront;
	            var __listCount3 = __list3.Count;
	            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
	            {
	                var o = __list3[__i3];
	                {
	                    o.gameObject.SetActive(false);
	                }
	            }
	        }
	       // SweepBack.SetActive(false);
	        MainSweepCollider.enabled = true;
	        var e = new DungeonBtnClick(3, eDungeonType.Main);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnClickSelectDungeon(int i)
	    {
	        switch (i)
	        {
	            case 0:
	            {
	                OnClickMainDungeon();
	            }
	                break;
	            case 1:
	            {
	                OnClickTeamDungeon();
	            }
	                break;
                case 2:
	            {
	                OnClickVipDungeon();
	            }
	                break;
	        }
	    }
	
	    //----------------------------------------------------General----
	    public void OnClickSelectDungeonInfo(int i)
	    {
	        var type = eDungeonType.Main;
	        if (TeamInfo.activeSelf)
	        {
	            type = eDungeonType.Team;
	        }
	        var e = new DungeonInfosMainInfo(i, type);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //------------------------------------------------------------------------Team----
	    public void OnClickTeamDungeon()
	    {
	//         ScrollContainer.SetActive(true);
	//         MainScroll.SetActive(false);
	//         TeamScroll.SetActive(true);
	        var ee = new DungeonGroupCellClick2(1, 0);
	        EventDispatcher.Instance.DispatchEvent(ee);
	    }

        public void OnClickVipDungeon()
        {
            //         ScrollContainer.SetActive(true);
            //         MainScroll.SetActive(false);
            //         TeamScroll.SetActive(true);
            var ee = new DungeonGroupCellClick2(2, 0);
            EventDispatcher.Instance.DispatchEvent(ee);
        }

	    public void OnClickTeamDungeonEnter()
	    {
	        var e = new DungeonBtnClick(1, eDungeonType.Team);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTeamDungeonLineup()
	    {
	        var e = new DungeonBtnClick(2, eDungeonType.Team);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTeamDungeonReset()
	    {
	        var e = new DungeonBtnClick(3, eDungeonType.Team);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnCloseUiBindRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.DungeonUI)
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
	
	    private void OnFuBenNetRetCallBack(IEvent ievent)
	    {
	        var e = ievent as DungeonNetRetCallBack;
	        switch (e.Type)
	        {
                case 10:
                    {
                        int idx = MyRandom.Random(AwardList.Count-1);
                        OnClickAward(idx);
                    }
                    break;
	            case 11:
	            {
	                ScrollContainer.SetActive(false);
	                MainInfo.SetActive(true);
	                TeamInfo.SetActive(false);
                    VipInfo.SetActive(false);
	            }
	                break;
	            case 12:
	            {
	                ScrollContainer.SetActive(false);
	                MainInfo.SetActive(false);
	                TeamInfo.SetActive(true);
                    VipInfo.SetActive(false);
	            }
	                break;
                case 13:
	            {
                    ScrollContainer.SetActive(false);
                    MainInfo.SetActive(false);
                    TeamInfo.SetActive(false);
                    VipInfo.SetActive(true);
	            }
	                break;
	        }
	    }
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        if (mRemoveBind)
	        {
	            var selectsCount1 = DungeonSelects.Count;
	            for (var i = 0; i < selectsCount1; i++)
	            {
	                var s = DungeonSelects[i];
	            }
	
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	            EventDispatcher.Instance.AddEventListener(DungeonNetRetCallBack.EVENT_TYPE, OnFuBenNetRetCallBack);
	            EventDispatcher.Instance.AddEventListener(DungeonGroupCellClick.EVENT_TYPE, OnClickFuBenGroupCell);

	            var controllerBase = UIManager.Instance.GetController(UIConfig.DungeonUI);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.QueueUpData);
	            Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	
	            var controller = UIManager.Instance.GetController(UIConfig.ShareFrame);
	            Binding.SetBindDataSource(controller.GetDataModel(""));
	
	            //OnClickMainDungeon();
	            //TeamInfo.SetActive(false);
	            //MainSweep.SetActive(false);
	            //InitDungeonSelect();
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
	
	    public void OnFormatExpect(UILabel lable)
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
	        var taget = timer.TargetTime;
	        if (taget > Game.Instance.ServerTime)
	        {
	            var dif = (taget - Game.Instance.ServerTime);
	            var str = GameUtils.TimeString(dif.Hours, dif.Minutes, dif.Seconds);
	            lable.text = str;
	        }
	        else
	        {
	            //无法确定时间
	            var str = GameUtils.GetDictionaryText(270082);
	            lable.text = str;
	        }
	    }
	
	    public void OnFormatLineup(UILabel lable)
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
	        var taget = timer.TargetTime;
	        var dif = (Game.Instance.ServerTime - taget);
	        var str = GameUtils.TimeString(dif.Hours, dif.Minutes, dif.Seconds);
	        lable.text = str;
	    }
	
	    private void RemoveBindEvent()
	    {
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUiBindRemove);
	        EventDispatcher.Instance.RemoveEventListener(DungeonNetRetCallBack.EVENT_TYPE, OnFuBenNetRetCallBack);
	        EventDispatcher.Instance.RemoveEventListener(DungeonGroupCellClick.EVENT_TYPE, OnClickFuBenGroupCell);
        }
	
	    private IEnumerator ShowOtherAward(int index)
	    {
	        yield return new WaitForSeconds(1.0f);
	        var awardListCount6 = AwardList.Count;
	        for (var i = 0; i < awardListCount6; i++)
	        {
	            if (i == index)
	            {
	                continue;
	            }
	            var o = AwardList[i];
	            var tweens = o.GetComponentsInChildren<TweenRotation>(true);
	            {
	                var array5 = tweens;
	                var arrayLength5 = array5.Length;
	                //for (int i5 = 0; i5 < arrayLength5; ++i5)
	                {
	                    var position = array5[0];
	                    {
	                        position.ResetForPlay();
	                        position.PlayForward();
	                    }
	                }
	            }
	        }
	        yield return new WaitForSeconds(1.0f);
	       // SweepBack.SetActive(true);
	        MainSweepCollider.enabled = false;
            //这里要加入一个逻辑,根据扫荡次数,决定显示扫荡按钮或者返回按钮
            {
              //  var tbDungeon = Table.GetFuben(id);
              //  i.ResetCount = PlayerDataManager.Instance.GetExData(tbDungeon.ResetExdata);
              //  i.EnterCount = PlayerDataManager.Instance.GetExData(tbDungeon.TodayCountExdata);
              //  SweepBtn.SetActive(true);
            }
                
         
        }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif

	        var dungeonTypesCount0 = DungeonList.Count;
	        for (var i = 0; i < dungeonTypesCount0; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { OnClickSelectDungeon(j); });
	            DungeonList[i].onClick.Add(deleget);
	        }
	
	        var selectsCount1 = DungeonSelects.Count;
	        for (var i = 0; i < selectsCount1; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { OnClickSelectDungeonInfo(j); });
	            DungeonSelects[i].onClick.Add(deleget);
	        }
	
	
	        var awardListCount3 = AwardList.Count;
	        for (var i = 0; i < awardListCount3; i++)
	        {
	            var tweens = AwardList[i].GetComponents<TweenRotation>();
	            var tween = tweens[0];
	            tween.enabled = false;
	            var j = i;
	            var deleget = new EventDelegate(() =>
	            {
	                AwardFront[j].gameObject.SetActive(true);
	                AwardBack[j].gameObject.SetActive(false);
	                var tween2 = tweens[1];
	                tween2.ResetForPlay();
	                tween2.PlayForward();
	            });
	            tween.onFinished.Add(deleget);
	            var deleget1 = new EventDelegate(() => { OnClickAward(j); });
	            var btn = AwardList[i].GetComponent<UIEventTrigger>();
	            btn.onClick.Add(deleget1);
	        }
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