#region using

using System;
using System.Collections;
using System.Collections.Generic;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WingFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public GameObject CacelAuto;
	    public Animation CritEffect;
	    public GameObject LightEffect;
	    public List<WingLinkedNodeTransfer> LinkedNodelList;
	    private bool animationLock;
	    private WingLinkedNodeTransfer linkedNode;
	    public UIDragRotate ModelDrag;
	
	    private readonly List<Vector3> PosList = new List<Vector3>
	    {
	        new Vector3(615, 270, 00),
	        new Vector3(315, -100, 0),
	        new Vector3(-100, -200, 0),
	        new Vector3(185, 95, 0),
	        new Vector3(-230, 300, 0)
	    };
	
	    public List<UIButton> PartMenus;
	    public TweenPosition PartMoveProgress;
	    public TweenPosition PartMoveRight;
	    public TweenScale PartScale;
	    public int RenderQueue = 3004;
	    //public GameObject Sky;
	    public TweenAlpha WholeAlpha;
	    public TweenPosition WholeMoveLeft;
	    public TweenPosition WholePostion;
	    public TweenScale WholeScale;
	    public AnimationModel WingModel;
	    public GameObject WingPart;
	    public GameObject WingWhole;
	
	    private IEnumerator EndAnimation(float delay, GameObject obj)
	    {
	        yield return new WaitForSeconds(delay);
	        obj.SetActive(false);
	    }
	
	    private IEnumerator AfterLightBall(int index, Action act)
	    {
 	        yield return new WaitForSeconds(0.1f);
// 	        linkedNode.SetBallActive(index, true);
// 	        yield return new WaitForSeconds(0.3f);
// 	        LightEffect.SetActive(false);
// 	        animationLock = false;
	        SetBallActive(index, true, true);
	        act();
	    }

	    private void ResetBall(int star)
	    {
            // 	            for (var i = 0; i < starCount; i++)
            // 	            {
            // 	                linkedNode.SetBallActive(i, true);
            // 	            }
            // 	            for (var i = starCount; i < 10; i++)
            // 	            {
            // 	                linkedNode.SetBallActive(i, false);
            // 	            }
            // 	            if (starCount == 10)
            // 	            {
            // 	                starCount--;
            // 	            }
            // 	            linkedNode.MoveTo(starCount, false);
            for (var i = 0; i < star; i++)
            {
                SetBallActive(i, true, false);
            }
            for (var i = star; i < 10; i++)
            {
                SetBallActive(i, false, false);
            }

	    }

	    private void SetBallActive(int index, bool active, bool doAnimation)
	    {
            var helper = WingPart.GetComponentInChildren<WingPartTrainningHelper>();
	        if (null != helper)
	        {
                helper.RefreshTrainningPart(index, active, doAnimation);
	        }
	        else
	        {
                Logger.Error("can't find WingPartTrainningHelper");
	        }
	    }
	    private void CreateModel(int id)
	    {
	        DestroyModel();
	        var tbEquip = Table.GetEquipBase(id);
	        if (tbEquip == null)
	        {
	            return;
	        }
	        var tbMont = Table.GetWeaponMount(tbEquip.EquipModel);
	        if (tbMont == null)
	        {
	            return;
	        }
	        WingModel.CreateModel(tbMont.Path, tbEquip.AnimPath + "/FlyIdle.anim", model =>
	        {
	            ModelDrag.Target = model.transform;
	            WingModel.PlayAnimation();
	            model.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.UI));
	            model.gameObject.SetRenderQueue(RenderQueue);
	        });
	    }
	
	    //-----------------------------------------------Model-----
	    private void DestroyModel()
	    {
	        if (WingModel)
	        {
	            WingModel.DestroyModel();
	        }
	    }
	
	    private WingLinkedNodeTransfer GetNodeTransfer(int index, bool setShow = true)
	    {
// 	        var count = LinkedNodelList.Count;
// 	        if (index < 0 || index >= count)
// 	        {
// 	            return null;
// 	        }
// 	        var node = LinkedNodelList[index];
// 	
// 	        for (var i = 0; i < count; i++)
// 	        {
// 	            if (index == i)
// 	            {
// 	                LinkedNodelList[i].gameObject.SetActive(true);
// 	            }
// 	            else
// 	            {
// 	                LinkedNodelList[i].gameObject.SetActive(false);
// 	            }
// 	        }
//	        return node;
	        return null;
	    }
	
	    public void OnClickAdvanced()
	    {
	        var e = new WingOperateEvent(1, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickAdvancedAuto()
	    {
	        var e = new WingOperateEvent(1, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickClose()
	    {
	        PlayerDataManager.Instance.WeakNoticeData.WingAdvance = false;
	        PlayerDataManager.Instance.WeakNoticeData.WingTraining = false;
	        var e = new WingOperateEvent(-2, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.WingUI));
	    }


	    private void OnClickPart(int i)
	    {
	        animationLock = false;
	        var e = new WingOperateEvent(0, i);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPartBack()
	    {
	        animationLock = false;
	        WholeScale.onFinished.Clear();
	        PartScale.onFinished.Clear();
	        PartScale.PlayReverse();
	        PartScale.onFinished.Add(new EventDelegate(PartToWholeTweenFinish));
	        PartMoveRight.PlayReverse();
	        PartMoveProgress.PlayReverse();
	        var e = new WingOperateEvent(-2, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPartTrain()
	    {
	        if (animationLock)
	        {
	            return;
	        }
	        var e = new WingOperateEvent(-1, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPartTrainAuto()
	    {
	        if (animationLock && CacelAuto.activeSelf == false)
	        {
	            return;
	        }
	        animationLock = false;
	        var e = new WingOperateEvent(-1, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPartTrainIfAuto()
	    {
	        animationLock = false;
	        var e = new WingOperateEvent(-1, 2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickTabMenuAdvanced()
	    {
	        PlayerDataManager.Instance.WeakNoticeData.WingAdvance = false;
	        var e = new WingOperateEvent(-2, 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	        WingModel.PlayAnimation();
	    }

        private int lastClickIndex = -1;

	    public void OnClickTabMenuTrain()
	    {
	        PlayerDataManager.Instance.WeakNoticeData.WingTraining = false;
	        var e = new WingOperateEvent(-2, 0);
	        EventDispatcher.Instance.DispatchEvent(e);
            if (lastClickIndex == -1)
	        {
                lastClickIndex = 0;
                OnClickPart(0);
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        EventDispatcher.Instance.RemoveEventListener(WingNotifyLogicEvent.EVENT_TYPE, OnNotifyWingLogicEvent);
	        EventDispatcher.Instance.RemoveEventListener(WingRefreshStarPage.EVENT_TYPE, OnRefreshWingStarPage);
	        EventDispatcher.Instance.RemoveEventListener(WingModelRefreh.EVENT_TYPE, OnRefreshWingModel);
	        EventDispatcher.Instance.RemoveEventListener(WingRefreshStarCount.EVENT_TYPE, OnRefreshWingStarCount);
	        EventDispatcher.Instance.RemoveEventListener(WingRefreshTrainCount.EVENT_TYPE, OnRefreshWingTrainCount);
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
	        //Sky.SetActive(false);
	
// 	        if (UICamera.mainCamera)
// 	        {
// 	            UICamera.mainCamera.nearClipPlane = -10f;
// 	        }
	
	        Binding.RemoveBinding();
	        DestroyModel();
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
	        LightEffect.SetActive(false);
	        CritEffect.gameObject.SetActive(false);
	        OnPartBack();
	        animationLock = false;
	        if (!EventDispatcher.Instance.HasEventListener(WingRefreshStarPage.EVENT_TYPE, OnRefreshWingStarPage))
	        {
	            EventDispatcher.Instance.AddEventListener(WingModelRefreh.EVENT_TYPE, OnRefreshWingModel);
	        }
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.WingUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.WeakNoticeData);
	
// 	        var xform = Sky.transform;
// 	        Sky.SetActive(true);
// 	        xform.parent = UICamera.mainCamera.transform;
// 	        xform.localPosition = Vector3.zero;
// 	        xform.localScale = Vector3.one*3000;
// 	        Sky.SetRenderQueue(3005);
// 	
// 	        UICamera.mainCamera.nearClipPlane = -1f;
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    public void OnPartBack()
	    {
// 	        animationLock = false;
// 	        WingWhole.SetActive(true);
// 	        WingPart.SetActive(false);
// 	
// 	        WholePostion.ResetForPlay();
// 	        WholePostion.enabled = false;
// 	        WholeScale.ResetForPlay();
// 	        WholeScale.enabled = false;
// 	        WholeAlpha.ResetForPlay();
// 	        WholeAlpha.enabled = false;
// 	
// 	        WholeMoveLeft.ResetForPlay();
// 	        WholeMoveLeft.enabled = false;
// 	
// 	        {
// 	            var __list5 = PartMenus;
// 	            var __listCount5 = __list5.Count;
// 	            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
// 	            {
// 	                var button = __list5[__i5];
// 	                {
// 	                    if (button.collider.enabled == false)
// 	                    {
// 	                        button.ResetDefaultColor();
// 	
// 	                        button.collider.enabled = true;
// 	                        button.SetState(UIButtonColor.State.Normal, true);
// 	                    }
// 	                }
// 	            }
// 	        }
	        var e = new WingOperateEvent(-2, -1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnPartTrainShowOver()
	    {
	        animationLock = false;
	    }
	
	    private void OnRefreshWingModel(IEvent ievent)
	    {
	        var e = ievent as WingModelRefreh;
	        CreateModel(e.TableId);
	    }
	
	    private void OnNotifyWingLogicEvent(IEvent ievent)
	    {
	        var e = ievent as WingNotifyLogicEvent;
	        switch (e.Type)
	        {
	            case 1:
	            {
	                CritEffect.gameObject.SetActive(true);
	                CritEffect[CritEffect.clip.name].time = 0;
	                CritEffect.Sample();
	                CritEffect.Play(PlayMode.StopAll);
	                StartCoroutine(EndAnimation(0.6f, CritEffect.gameObject));
	            }
	                break;
	        }
	    }
	
	    private void OnRefreshWingStarCount(IEvent ievent)
	    {
	        var e = ievent as WingRefreshStarCount;
	        var star = e.Star;
	        animationLock = true;
	        SetLightBall(star - 1, () =>
	        {
	            //linkedNode.MoveTo(star, true, OnClickPartTrainIfAuto);
	            OnClickPartTrainIfAuto();

	        });
	    }
	
	    private void OnRefreshWingStarPage(IEvent ievent)
	    {
// 	        var e = ievent as WingRefreshStarPage;
// 	        {
// 	            var __list3 = PartMenus;
// 	            var __listCount3 = __list3.Count;
// 	            for (var __i3 = 0; __i3 < __listCount3; ++__i3)
// 	            {
// 	                var button = __list3[__i3];
// 	                {
// 	                    button.collider.enabled = false;
// 	                }
// 	            }
// 	        }
// 	        PartScale.onFinished.Clear();
// 	        var index = e.Part;
// 	        var star = e.Star;
// 	        var showBeing = e.ShowBegin;
// 	        WholePostion.ResetForPlay();
// 	        WholePostion.to = PosList[index];
// 	        WholePostion.PlayForward();
// 	        WholeAlpha.ResetForPlay();
// 	        WholeAlpha.PlayForward();
// 	        WholeMoveLeft.ResetForPlay();
// 	        WholeMoveLeft.PlayForward();
// 	        WholeScale.enabled = false;
// 	        WholeScale.onFinished.Clear();
// 	        WholeScale.ResetForPlay();
// 	        WholeScale.PlayForward();
// 	        var layer = e.Layer;
// 	        linkedNode = GetNodeTransfer(layer);
// 	        WholeScale.SetOnFinished(new EventDelegate(() => { WholeToPartTweenFinish(star, showBeing); }));
            var e = ievent as WingRefreshStarPage;
            var star = e.Star;
            ResetBall(star);
	    }
	
	    private void OnRefreshWingTrainCount(IEvent ievent)
	    {
// 	//翅膀升阶
// 	        var e = ievent as WingRefreshTrainCount;
// 	        var trainCount = e.TrainCount;
// 	
// 	        SetLightBall(9, () =>
// 	        {
//  	            animationLock = true;
// // 	            linkedNode.ZoomOut(() =>
// // 	            {
// // 	                linkedNode = GetNodeTransfer(trainCount);
// // 	                linkedNode.ZoomIn(OnClickPartTrainIfAuto);
// // 	            });
// 	            OnClickPartTrainIfAuto();
// 	        });
            OnClickPartTrainIfAuto();
	    }
	
	    public void PartShowFinish()
	    {
	        PartScale.onFinished.Clear();
	        linkedNode.ZoomIn(OnPartTrainShowOver);
	    }
	
	    public void PartToWholeTweenFinish()
	    {
	        WingPart.SetActive(false);
	        WholeScale.onFinished.Clear();
	        WholeScale.PlayReverse();
	        WholePostion.PlayReverse();
	        WholeAlpha.PlayReverse();
	        WholeScale.PlayReverse();
	        WholeMoveLeft.PlayReverse();
	        {
	            var __list5 = PartMenus;
	            var __listCount5 = __list5.Count;
	            for (var __i5 = 0; __i5 < __listCount5; ++__i5)
	            {
	                var button = __list5[__i5];
	                {
	                    button.collider.enabled = true;
	                }
	            }
	        }
	    }
	
	    private void SetLightBall(int index, Action act)
	    {
	       // LightEffect.SetActive(true);
	
	        StartCoroutine(AfterLightBall(index, act));
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        animationLock = false;
	        var index = 0;
	        {
	            var __list1 = PartMenus;
	            var __listCount1 = __list1.Count;
	            for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var button = __list1[__i1];
	                {
	                    var i = index;
	                    button.onClick.Add(new EventDelegate(() => { OnClickPart(i); }));
	                    index++;
	                }
	            }
	        }
	        {
	            var __list2 = LinkedNodelList;
	            var __listCount2 = __list2.Count;
	            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	            {
	                var nodeTransfer = __list2[__i2];
	                {
	                    nodeTransfer.gameObject.SetActive(false);
	                }
	            }
	        }
	        OnPartBack();
	        EventDispatcher.Instance.AddEventListener(WingRefreshStarPage.EVENT_TYPE, OnRefreshWingStarPage);
	        EventDispatcher.Instance.AddEventListener(WingRefreshStarCount.EVENT_TYPE, OnRefreshWingStarCount);
	        EventDispatcher.Instance.AddEventListener(WingRefreshTrainCount.EVENT_TYPE, OnRefreshWingTrainCount);
	        EventDispatcher.Instance.AddEventListener(WingNotifyLogicEvent.EVENT_TYPE, OnNotifyWingLogicEvent);
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
	
	    public void WholeToPartTweenFinish(int starCount, bool showBegin)
	    {
// 	        WholeScale.onFinished.Clear();
// 	        WingPart.SetActive(true);
// 	        PartScale.onFinished.Clear();
// 	        PartScale.ResetForPlay();
// 	        PartScale.PlayForward();
// 	        linkedNode.Reset();
// 	        if (showBegin == false)
// 	        {
// 	            for (var i = 0; i < starCount; i++)
// 	            {
// 	                linkedNode.SetBallActive(i, true);
// 	            }
// 	            for (var i = starCount; i < 10; i++)
// 	            {
// 	                linkedNode.SetBallActive(i, false);
// 	            }
// 	            if (starCount == 10)
// 	            {
// 	                starCount--;
// 	            }
// 	            linkedNode.MoveTo(starCount, false);
// 	        }
// 	        else
// 	        {
// 	            animationLock = true;
// 	            PartScale.SetOnFinished(new EventDelegate(PartShowFinish));
// 	        }
// 	        {
// 	            var __list4 = PartMenus;
// 	            var __listCount4 = __list4.Count;
// 	            for (var __i4 = 0; __i4 < __listCount4; ++__i4)
// 	            {
// 	                var button = __list4[__i4];
// 	                {
// 	                    button.collider.enabled = true;
// 	                }
// 	            }
// 	        }
// 	        PartMoveRight.ResetForPlay();
// 	        PartMoveRight.PlayForward();
// 	        PartMoveProgress.ResetForPlay();
// 	        PartMoveProgress.PlayForward();
// 	        WholeScale.onFinished.Clear();
	    }
	}
}