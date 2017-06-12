#region using

using System;
using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WishingFrame : MonoBehaviour
	{
	    public GameObject AnimBoxCollider;
	    public GameObject BagPos;
	    private GameObject baoXiangAni; //宝箱动画
	    public GameObject BaoXiangpos; //宝箱动画
	    public BindDataRoot Binding;
	    public List<GameObject> FlyExpPosObj;
	    public GameObject FlyToObj;
	    //public GameObject HomeObj;
	    // public GameObject BlurObject;       //模糊层
	
	    private GameObject flyPrefab;
	    //private GameObject flyPrefabExp;
	    private GameObject flyPrefabItem;
	    public GameObject OnePos;
	    public List<UIButton> OperateButton;
	    public List<GameObject> TenDrawObj; //十连抽父物体
	    public List<GameObject> TenPos;
	    public List<GameObject> Tips;
	    //public List<UIEventTrigger> OperateButton2;
	    public List<UIButton> TreeButton;
	    private List<GameObject> varTenList; //动态加载的十连抽prefab
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        //EventDispatcher.Instance.RemoveEventListener(UIEvent_ElfBaoXiangOverEvent.EVENT_TYPE, BaoxiangAnimOver);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_WishPlayFlyAnim.EVENT_TYPE, ExpFlyAnimation);
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
	        // EventDispatcher.Instance.RemoveEventListener(ElfGetDrawResultBack.EVENT_TYPE, DrawAnimationStart);
	        //EventDispatcher.Instance.RemoveEventListener(UIEvent_WishingGetDrawResultBack.EVENT_TYPE, CreateTenPrefab);
	        //EventDispatcher.Instance.RemoveEventListener(UIEvent_WishingTenDrawStop.EVENT_TYPE, TenDrawAnimationStop);
	        //EventDispatcher.Instance.RemoveEventListener(UIEvent_WishingEatAllAnim.EVENT_TYPE, EatAllAnim);
	        while (varTenList.Count > 10)
	        {
	            Destroy(varTenList[0]);
	            varTenList.RemoveAt(0);
	        }
	        Binding.RemoveBinding();
	        //CreateFakeObj(-1);
	        Destroy(baoXiangAni);
	        baoXiangAni = null;
	
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
	        var controllerBase = UIManager.Instance.GetController(UIConfig.WishingUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel("WishData"));
	        Binding.SetBindDataSource(controllerBase.GetDataModel("Tree"));
	        Binding.SetBindDataSource(controllerBase.GetDataModel("Draw"));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	        Binding.SetBindDataSource(UIManager.Instance.GetController(UIConfig.ChatMainFrame).GetDataModel(""));
	
	        //EventDispatcher.Instance.AddEventListener(ElfGetDrawResultBack.EVENT_TYPE, DrawAnimationStart);
	
	
	        //EventDispatcher.Instance.AddEventListener(UIEvent_WishingGetDrawResultBack.EVENT_TYPE, CreateTenPrefab);
	        //EventDispatcher.Instance.AddEventListener(UIEvent_WishingTenDrawStop.EVENT_TYPE, TenDrawAnimationStop);
	        //EventDispatcher.Instance.AddEventListener(UIEvent_WishingEatAllAnim.EVENT_TYPE, EatAllAnim);
	        AnimBoxCollider.SetActive(false);
	        for (var i = 0; i < Tips.Count; i++)
	        {
	            Tips[i].SetActive(false);
	        }
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    // public CreateFakeCharacter ModelRoot;
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        varTenList = new List<GameObject>();
	        for (var i = 0; i < OperateButton.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { InfoWillItemBtn(j); });
	
	            OperateButton[i].onClick.Add(deleget);
	        }
	
	        //for (int i = 0; i < TenDrawObj.Count; i++)
	        //{
	        //    int j = i;
	        //    var deleget = new EventDelegate(() =>
	        //    {
	        //        InfoTenBtn(j);
	        //    });
	
	        //    OperateButton2[i].onClick.Add(deleget);
	        //}
	        for (var i = 0; i < TreeButton.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { BtnTreeList(j); });
	
	            TreeButton[i].onClick.Add(deleget);
	        }
	        flyPrefab = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFlyWishing.prefab");
	        flyPrefabItem = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFlyWishingBag.prefab");
	        //flyPrefabExp = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
	
	        // EventDispatcher.Instance.AddEventListener(UIEvent_ElfBaoXiangOverEvent.EVENT_TYPE, BaoxiangAnimOver);
	        EventDispatcher.Instance.AddEventListener(UIEvent_WishPlayFlyAnim.EVENT_TYPE, ExpFlyAnimation);
	
	
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
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    #region 主界面按钮操作
	
	    public void BtnClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.WishingUI));
	    }
	
	    public void BtnBack()
	    {
			var e = new UIEvent_WishingOperation(3);
			EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBackCity()
	    {
			EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.WishingUI));
	    }
	
	    public void BtnTree()
	    {
	        var e = new UIEvent_WishingOperation(5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnWishing()
	    {
	        var e = new UIEvent_WishingOperation(6);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	
	    public void BtnGetBag()
	    {
	        var e = new UIEvent_WishingOperation(16);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnShowLog()
	    {
	        var e = new UIEvent_WishingOperation(17);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnCloseLog()
	    {
	        var e = new UIEvent_WishingOperation(18);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OpenTips1()
	    {
	        Tips[0].SetActive(true);
	    }
	
	    public void CloseTips1()
	    {
	        Tips[0].SetActive(false);
	    }
	
	    public void OpenTips2()
	    {
	        Tips[1].SetActive(true);
	    }
	
	    public void CloseTips2()
	    {
	        Tips[1].SetActive(false);
	    }
	
	    #endregion
	
	    #region 许愿池操作
	
	    public void OkBtn()
	    {
	        var e = new UIEvent_WishingOperation(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	        //TenAnimationStart();    //动画播放
	    }
	
	    public void OneDrawBtn()
	    {
	        var e = new UIEvent_WishingOperation(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void TenDrawBtn()
	    {
	        var e = new UIEvent_WishingOperation(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void InfoWillItemBtn(int index)
	    {
	        var e = new UIEvent_WishingInfoWillItemBtn();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void InfoOneBtn()
	    {
	        var e = new UIEvent_WishingInfoItem();
	        e.Type = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	
	    //public void BtnPetOk()
	    //{
	    //    UIEvent_WishingOperation e = new UIEvent_WishingOperation(7);
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //    CreateFakeObj(-1);
	    //    TenAnimation["TenDraw"].speed = 1;
	    //}
	
	    public void BtnPickAllItem()
	    {
	        var e = new UIEvent_WishingOperation(14);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnWishingBag()
	    {
	        var e = new UIEvent_WishingBtnWishingBag();
	        e.Isback = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnWishingBagBack()
	    {
	        var e = new UIEvent_WishingBtnWishingBag();
	        e.Isback = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnWishingArrange()
	    {
	        var e = new UIEvent_WishingOperation(15);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnFormatTimer(UILabel lable)
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
	        var str = "";
	        if (target > Game.Instance.ServerTime)
	        {
	            var timeSpan = target - Game.Instance.ServerTime;
	            str = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Days*24 + timeSpan.Hours, timeSpan.Minutes,
	                timeSpan.Seconds);
	            str += GameUtils.GetDictionaryText(300404);
	        }
	        lable.text = str;
	    }
	
	    #endregion
	
	    #region 许愿树操作
	
	    public void BtnTreeList(int index)
	    {
	        var e = new UIEvent_WishingBtnTreeList();
	        e.Index = index;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //public void BtnMyWish()
	    //{
	    //    UIEvent_WishingOperation e = new UIEvent_WishingOperation(8);
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //}
	
	    public void BtnCloseBuy()
	    {
	        var e = new UIEvent_WishingOperation(9);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyReduceClick()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 4;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyReducePress()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 0;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyReduceRelease()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 2;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyAddClick()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 5;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyAddPress()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 1;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyAddRelease()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 3;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyMax()
	    {
	        var e = new UIEvent_WishingBtnBuyAddOrReduce();
	        e.Type = 6;
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnBuyOk()
	    {
	        var e = new UIEvent_WishingOperation(10);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnCloseMyBuy()
	    {
	        var e = new UIEvent_WishingOperation(11);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPage1()
	    {
	        var e = new UIEvent_WishingOperation(19);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPage2()
	    {
	        var e = new UIEvent_WishingOperation(20);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void BtnPage3()
	    {
	        var e = new UIEvent_WishingOperation(21);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    #endregion
	
	    #region 十连抽动画效果
	
	    private readonly string prefabPath = "UI/Wishing/WishingDrawCell.prefab";
	    public bool IsOneDraw;
	
	    private void DrawAnimationStart()
	    {
	        //ElfGetDrawResultBack e = ievent as ElfGetDrawResultBack;
	        //BlurObject.SetActive(true);
	        if (baoXiangAni == null)
	        {
	            baoXiangAni =
	                Instantiate(ResourceManager.PrepareResourceSync<GameObject>("Model/UI/UI_BaoXiang.prefab")) as
	                    GameObject;
	            baoXiangAni.transform.parent = BaoXiangpos.transform;
	            baoXiangAni.transform.localPosition = Vector3.zero;
	            baoXiangAni.transform.localScale = new Vector3(318f, 318f, 318f);
	            baoXiangAni.AddComponent<WishingAnimation>();
	            var baoxiangAnim = baoXiangAni.GetComponent<Animation>();
	            baoXiangAni.SetActive(true);
	            baoxiangAnim.Play();
	        }
	        else
	        {
	            var baoxiangAnim = baoXiangAni.GetComponent<Animation>();
	            baoXiangAni.SetActive(true);
	            baoxiangAnim.Play();
	        }
	        BaoxiangAnimOver();
	    }
	
	    public void BaoxiangAnimOver()
	    {
	        if (IsOneDraw)
	        {
	            NetManager.Instance.StartCoroutine(WaitSecondsDraw(1.5f, () =>
	            {
	                EventDispatcher.Instance.DispatchEvent(new UIEvent_WishShowDrawGetEvent());
	                //BlurObject.SetActive(false);
	                if (baoXiangAni != null)
	                {
	                    baoXiangAni.SetActive(false);
	                    AnimBoxCollider.SetActive(false);
	                }
	            }));
	        }
	        else
	        {
	            CreateTenPrefabs();
	            NetManager.Instance.StartCoroutine(WaitSecondsDraw(1.5f, () =>
	            {
	                EventDispatcher.Instance.DispatchEvent(new UIEvent_WishShowDrawGetEvent());
	                //BlurObject.SetActive(false);
	                if (baoXiangAni != null)
	                {
	                    baoXiangAni.SetActive(false);
	                    AnimBoxCollider.SetActive(false);
	                }
	            }));
	        }
	    }
	
	    //创建10个抽奖物品
	    private void CreateTenPrefabs()
	    {
	        for (var i = 0; i < varTenList.Count; i++)
	        {
	            Destroy(varTenList[i]);
	        }
	        varTenList.Clear();
	        StartCoroutine(InitPreBagCellfab());
	    }
	
	    //加载资源，绑定十连抽源
	    private IEnumerator InitPreBagCellfab()
	    {
	        var controllerBase = UIManager.Instance.GetController(UIConfig.WishingUI);
	        if (controllerBase == null)
	        {
	            yield break;
	        }
	        var holder = ResourceManager.PrepareResourceWithHolder<GameObject>(prefabPath);
	        var draw = controllerBase.GetDataModel("Draw") as WishingDrawDataModel;
	        for (var i = 0; i < draw.TenGetItem.Count; i++)
	        {
	            yield return holder.Wait();
	            var BagCell = Instantiate(holder.Resource) as GameObject;
	            BagCell.GetComponent<UIWidget>().depth = 400;
	            var t = BagCell.transform;
	            //t.parent = TenDrawObj[i].transform;
	            t.SetParentEX(TenDrawObj[i].transform);
	            t.localScale = Vector3.one;
	            t.localPosition = Vector3.zero;
	            t.localRotation = new Quaternion(0, 0, 0, 0);
	            BagCell.SetActive(true);
	            var bindroot = TenDrawObj[i].GetComponent<BindDataRoot>();
	            var itemDataModel = draw.TenGetItem[i];
	            bindroot.SetBindDataSource(itemDataModel);
	            var itemLogic = BagCell.GetComponent<ListItemLogic>();
	            itemLogic.Index = i;
	            itemLogic.Item = itemDataModel;
	            varTenList.Add(itemLogic.gameObject);
	        }
	    }
	
	
	    private UIEvent_WishPlayFlyAnim mFlyUIEvent;
	
	    private void ExpFlyAnimation(IEvent ievent)
	    {
	        mFlyUIEvent = ievent as UIEvent_WishPlayFlyAnim;
	        GameObject go = null;
	        if (mFlyUIEvent.AnimIndex == 0) //十连抽结束物品飞到背包里动画
	        {
	            AnimBoxCollider.SetActive(true);
	            StartCoroutine(WaitSecondsDraw(0.5f, () =>
	            {
	                var list = mFlyUIEvent.List;
	                for (var i = 0; i < list.Count; i++)
	                {
	                    var obj = Instantiate(flyPrefab) as GameObject;
	                    var mTrans = OperateButton[list[i]].transform;
	                    PlayerDataManager.Instance.PlayFlyItem(obj, mTrans, FlyToObj.transform, mFlyUIEvent.ItemIds[i], 0,
	                        new Vector3(0, 20f, 0));
	                }
	                if (mFlyUIEvent.DrawType == 1)
	                {
	                    IsOneDraw = true;
	                }
	                else if (mFlyUIEvent.DrawType == 10)
	                {
	                    IsOneDraw = false;
	                }
	            }));
// 	            var expObj = Instantiate(flyPrefabExp) as GameObject;
// 	            if (mFlyUIEvent.DrawType == 1)
// 	            {
// 	                go = FlyExpPosObj[1];
// 	            }
// 	            else if (mFlyUIEvent.DrawType == 10)
// 	            {
// 	                go = FlyExpPosObj[2];
// 	            }
	            //PlayerDataManager.Instance.PlayFlyItem(expObj, go.transform, HomeObj.transform, 12, mFlyUIEvent.Exp);
	
	            DrawAnimationStart();
	        }
	        else if (mFlyUIEvent.AnimIndex == 1) //十连抽开始被排除物品飞到宝箱里动画
	        {
	            AnimBoxCollider.SetActive(true);
	            var list = mFlyUIEvent.ItemIds;
	            for (var i = 0; i < list.Count; i++)
	            {
	                var obj = Instantiate(flyPrefabItem) as GameObject;
	                if (list.Count == 1)
	                {
	                    var mTrans = OnePos.transform;
	                    PlayerDataManager.Instance.PlayFlyItem(obj, mTrans, BagPos.transform, list[i]);
	                }
	                else
	                {
	                    var mTrans = TenPos[i].transform;
	                    PlayerDataManager.Instance.PlayFlyItem(obj, mTrans, BagPos.transform, list[i]);
	                }
	            }
	            StartCoroutine(WaitSecondsDraw(0.8f, () => { AnimBoxCollider.SetActive(false); }));
	        }
	        else if (mFlyUIEvent.AnimIndex == 2)
	        {
	            //var expObj = Instantiate(flyPrefabExp) as GameObject;
	            //go = FlyExpPosObj[0];
	            //PlayerDataManager.Instance.PlayFlyItem(expObj, go.transform, HomeObj.transform, 12, mFlyUIEvent.Exp);
	        }
	    }
	
	    private IEnumerator WaitSecondsDraw(float seconds, Action act)
	    {
	        yield return new WaitForSeconds(seconds);
	        if (act != null)
	        {
	            act();
	        }
	    }
	
	    #endregion
	}
}