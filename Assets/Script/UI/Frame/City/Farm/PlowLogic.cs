#region using

using System;
using System.Collections;
using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using Shared;
using UnityEngine;

#endregion

namespace GameUI
{
	public class PlowLogic : MonoBehaviour
	{
	    private static readonly List<Vector3> pos = new List<Vector3>
	    {
	        new Vector3(0.0f, 0.0f, 0),
	        new Vector3(0.037f, 1.998876474f, 0),
	        new Vector3(0.148f, 3.997013205f, 0),
	        new Vector3(0.333f, 5.993669991f, 0),
	        new Vector3(0.591f, 7.981369484f, 0),
	        new Vector3(0.921f, 9.958043645f, 0),
	        new Vector3(1.321f, 11.91804452f, 0),
	        new Vector3(1.791f, 13.86626247f, 0),
	        new Vector3(2.33f, 15.80144285f, 0),
	        new Vector3(2.93f, 17.7016789f, 0),
	        new Vector3(3.599f, 19.59665767f, 0),
	        new Vector3(4.329f, 21.46591515f, 0),
	        new Vector3(5.119f, 23.31133849f, 0),
	        new Vector3(5.959f, 25.11550348f, 0),
	        new Vector3(6.859f, 26.9041908f, 10),
	        new Vector3(7.809f, 28.660f, 10),
	        new Vector3(8.809f, 30.3880f, 0),
	        new Vector3(9.859f, 32.09018608f, 0),
	        new Vector3(10.959f, 33.7688631f, 0),
	        new Vector3(12.099f, 35.41176516f, 0),
	        new Vector3(13.279f, 37.02232485f, 0),
	        new Vector3(14.499f, 38.60326981f, 0),
	        new Vector3(15.759f, 40.15678536f, 0),
	        new Vector3(17.059f, 41.6846325f, 0),
	        new Vector3(18.389f, 43.17726351f, 0),
	        new Vector3(19.749f, 44.63724588f, 0),
	        new Vector3(21.149f, 46.07679598f, 0),
	        new Vector3(22.569f, 47.47717858f, 0),
	        new Vector3(24.019f, 48.85042016f, 0),
	        new Vector3(25.499f, 50.1978123f, 0),
	        new Vector3(26.999f, 51.51185727f, 0),
	        new Vector3(28.529f, 52.80271411f, 0),
	        new Vector3(30.079f, 54.06314228f, 0),
	        new Vector3(31.649f, 55.29461717f, 0),
	        new Vector3(33.249f, 56.50587084f, 0),
	        new Vector3(34.859f, 57.68290945f, 0),
	        new Vector3(36.489f, 58.8344375f, 0),
	        new Vector3(38.139f, 59.96133758f, 0),
	        new Vector3(39.809f, 61.06438952f, 0),
	        new Vector3(41.489f, 62.13799672f, 0),
	        new Vector3(43.189f, 63.18945878f, 0),
	        new Vector3(44.899f, 64.21339991f, 0),
	        new Vector3(46.629f, 65.21654f, 0),
	        new Vector3(48.369f, 66.1937334f, 0),
	        new Vector3(50.119f, 67.1458331f, 0),
	        new Vector3(51.889f, 68.0788029f, 0),
	        new Vector3(53.669f, 68.98783833f, 0),
	        new Vector3(55.469f, 69.87846889f, 0),
	        new Vector3(57.269f, 70.74140365f, 0),
	        new Vector3(59.079f, 71.5822173f, 0),
	        new Vector3(60.899f, 72.40141054f, 0),
	        new Vector3(62.729f, 73.19944072f, 0),
	        new Vector3(64.569f, 73.97672557f, 0),
	        new Vector3(66.419f, 74.73364649f, 0),
	        new Vector3(68.279f, 75.47055146f, 0),
	        new Vector3(70.139f, 76.18398417f, 0),
	        new Vector3(72.009f, 76.87825057f, 0),
	        new Vector3(73.889f, 77.55360497f, 0),
	        new Vector3(75.769f, 78.20686102f, 0),
	        new Vector3(77.669f, 78.84517435f, 0),
	        new Vector3(79.569f, 79.46200807f, 0),
	        new Vector3(81.469f, 80.05785867f, 0),
	        new Vector3(83.379f, 80.63616589f, 0),
	        new Vector3(85.299f, 81.19705054f, 0),
	        new Vector3(87.219f, 81.73785276f, 0),
	        new Vector3(89.149f, 82.26163193f, 0),
	        new Vector3(91.079f, 82.7658968f, 0),
	        new Vector3(93.009f, 83.25100198f, 0),
	        new Vector3(94.949f, 83.71964801f, 0),
	        new Vector3(96.889f, 84.16958955f, 0),
	        new Vector3(98.839f, 84.60330223f, 0),
	        new Vector3(100.789f, 85.01870319f, 0),
	        new Vector3(102.739f, 85.41605959f, 0),
	        new Vector3(104.699f, 85.79752318f, 0),
	        new Vector3(106.659f, 86.16124871f, 0),
	        new Vector3(108.619f, 86.50745992f, 0),
	        new Vector3(110.589f, 86.83800037f, 0),
	        new Vector3(112.559f, 87.15125786f, 0),
	        new Vector3(114.529f, 87.44741813f, 0),
	        new Vector3(116.509f, 87.72802888f, 0),
	        new Vector3(118.489f, 87.99170686f, 0),
	        new Vector3(120.469f, 88.23860388f, 0),
	        new Vector3(122.449f, 88.46886042f, 0),
	        new Vector3(124.429f, 88.6826061f, 0),
	        new Vector3(126.429f, 88.88187028f, 0),
	        new Vector3(128.429f, 89.06452215f, 0),
	        new Vector3(130.429f, 89.23066371f, 0),
	        new Vector3(132.429f, 89.38038703f, 0),
	        new Vector3(134.429f, 89.5137745f, 0),
	        new Vector3(136.429f, 89.63089905f, 0),
	        new Vector3(138.429f, 89.73182437f, 0),
	        new Vector3(140.429f, 89.81660506f, 0),
	        new Vector3(142.429f, 89.88528681f, 0),
	        new Vector3(144.429f, 89.9379065f, 0),
	        new Vector3(146.429f, 89.9744923f, 0),
	        new Vector3(148.429f, 89.99506378f, 0),
	        new Vector3(150.429f, 89.99963192f, 0)
	    };
	
	    public BindDataRoot Binding;
	    public GameObject Depot; //包裹的位置
	    public List<PlowLandFrame> FarmLands;
	    public GameObject HarvestDemo;
	    public GameObject HomeObj;
	    public GameObject LandMenu;
	    public UILabel LockTip;
	    public UISlider MatureProgress;
	    public UILabel MatureTime;
	    public GameObject MatureTimeBg;
	    public GameObject MenuObject;
	    private GameObject mFlyPrefab;
	    private GameObject mFlyPrefab1;
	    private GameObject mFlyPrefabExp;
	    private bool mIsClick;
	    public Transform MissionFrame;
	    private int mMatureScend;
	    private int mMatureScendMax;
	    private Coroutine mMatureTimer;
	    private PlowLandFrame mTouchingFarmLand;
	    private Transform mTrans;
	    public GameObject OrderObj;
	    public GameObject PlantDemo;
	    public List<UIButton> SeedButtons; //种子等菜单
	    //public void OnClickPageDown()
	    //{
	    //    FarmOperateEvent e = new FarmOperateEvent(21);
	    //    EventDispatcher.Instance.DispatchEvent(e);
	    //    // mTouchingFarmLand = null;
	    //}
	
	    private string TimeFormatStr = "";
	    public GameObject Tip;
	    private readonly List<PlowLandFrame> TouchFarmLands = new List<PlowLandFrame>();
	
	    public enum LandDragState
	    {
	        Invalid,
	        Operate,
	        Draging
	    }
	
	    public LandDragState DragState { get; set; }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        mTrans = transform;
	        mFlyPrefab = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/ItemTip.prefab");
	        mFlyPrefab1 = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/ItemTip1.prefab");
	        mFlyPrefabExp = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFly.prefab");
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void CloseHarvestDemo()
	    {
	        HarvestDemo.SetActive(false);
	    }
	
	    public void ClosePlantDemo()
	    {
	        PlantDemo.SetActive(false);
	    }
	
	    public void CloseTip()
	    {
	        Tip.SetActive(false);
	    }
	
	    private void FlyOrderExp(IEvent ievent)
	    {
	        var obj = Instantiate(mFlyPrefabExp) as GameObject;
	        PlayerDataManager.Instance.PlayFlyItem(obj, OrderObj.transform, HomeObj.transform, 12);
	    }
	
	    private static Vector3 GetPosByIndex(int index)
	    {
	        if (index < 0)
	        {
	            return new Vector3(pos[0].x, pos[0].y + index*2, pos[0].z);
	        }
	        if (index > 96)
	        {
	            return new Vector3(pos[96].x + (index - 96)*2, pos[96].y, pos[96].z);
	        }
	        return pos[index];
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        if (LandMenu.gameObject.activeSelf)
	        {
	//             var x = LandMenuScroll.transform.localPosition.x;
	//             int beginIndex = 0;
	//             var LandMenusCount0 = LandMenus.Count;
	//             for (int i = 0; i < LandMenusCount0; i++)
	//             {
	//                 var landMenu = LandMenus[i];
	//                 if (i == 0)
	//                 {
	//                     beginIndex = (int) x/2;
	//                 }
	//                 int thisIndex = beginIndex + i*32;
	// 
	//                 Vector3 thisPos = GetPosByIndex(thisIndex);
	//                 if (x < 0)
	//                 {
	//                     landMenu.BtnMenu.transform.localPosition = new Vector3(thisPos.x - 60*i - x, thisPos.y - 45, 0);
	//                 }
	//                 else
	//                 {
	//                     landMenu.BtnMenu.transform.localPosition = new Vector3(thisPos.x - 60*i - x, thisPos.y - 45, 0);
	//                 }
	//             }
	            // UpdatMenuPostion();
	        }
	
	        if (Input.touchCount == 1
	            || Input.GetMouseButton((int) InputManager.MOUSE_BUTTON.MOUSE_BUTTON_LEFT))
	        {
	            var pos = Input.mousePosition;
	            var nUIRay = UICamera.mainCamera.ScreenPointToRay(pos);
	            RaycastHit nHit;
	
	            if (Physics.Raycast(nUIRay, out nHit, 10, LayerMask.GetMask("UI")))
	            {
	                var hitObj = nHit.collider.gameObject;
	                if (hitObj)
	                {
	                    {
	                        var __list1 = FarmLands;
	                        var __listCount1 = __list1.Count;
	                        for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	                        {
	                            var landLogic = __list1[__i1];
	                            {
	                                if (landLogic.ColliderObject == hitObj)
	                                {
	                                    OnTouchFarmLand(landLogic);
	                                    return;
	                                }
	                            }
	                        }
	                    }
	                }
	            }
	
	            if (TouchFarmLands.Count == 0)
	            {
	                mIsClick = false;
	            }
	        }
	        else
	        {
	            if (DragState == LandDragState.Operate)
	            {
	                if (mIsClick && TouchFarmLands.Count == 1)
	                {
	                    Logger.Info("Click Land");
	                    DragState = LandDragState.Invalid;
	                    OnClickLand(TouchFarmLands[0]);
	                    mIsClick = false;
	                    return;
	                }
	                TouchFarmLands.Clear();
	            }
	
	            DragState = LandDragState.Invalid;
	            mIsClick = true;
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
	        var e = new FarmOperateEvent(16);
	        EventDispatcher.Instance.DispatchEvent(e);
	
            //var e1 = new Show_UI_Event(UIConfig.CityUI);
            //EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    public void OnClickCloseLandMenu()
	    {
	        var e = new FarmOperateEvent(15);
	        EventDispatcher.Instance.DispatchEvent(e);
	        mTouchingFarmLand = null;
	    }
	
	    public void OnClickDepotOpen()
	    {
	        //DepotFrame.gameObject.SetActive(true);
	        SetLandCollider(false);
	        //NewScrollView.ResetPosition();
	        //LandMenu.SetActive(false);
	        var arg = new StoreArguments {Tab = 2};
	        var e = new Show_UI_Event(UIConfig.StoreFarm, arg);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickLand(PlowLandFrame land)
	    {
	        if (land == null)
	        {
	            return;
	        }
	        land.OnClickLand();
	        if (mTouchingFarmLand != null)
	        {
	            //mTouchingFarmLand = null;
	            return;
	        }
	        MatureTimeBg.gameObject.SetActive(false);
	        if (mMatureTimer != null)
	        {
	            StopCoroutine(mMatureTimer);
	            mMatureTimer = null;
	        }
	        mMatureScend = 0;
	        if (land.DataModel.State == (int) LandState.Lock)
	        {
	            //这块土地还未开垦,请提升农场等级！
	            var e1 = new ShowUIHintBoard(300302);
	            EventDispatcher.Instance.DispatchEvent(e1);
	            return;
	        }
	        var parent = UIManager.GetInstance().GetUIRoot(UIType.TYPE_TIP);
	        var loc = parent.transform.worldToLocalMatrix*land.LockSprite.worldCenter;
	        loc.x -= 100;
	        loc.y += 70;
	        loc.z = -300;
	        LandMenu.transform.localPosition = loc;
	        LandMenu.transform.localPosition += new Vector3(-80, 0, 0);
	        SpringPanel.Stop(MenuObject.gameObject);
	        ResetScrollViewPostion();
	        EventDispatcher.Instance.DispatchEvent(new FarmLandCellClick(land.DataModel.Index, false));
	        mTouchingFarmLand = land;
	        DragState = LandDragState.Invalid;
	    }
	
	    public void OnClickMenu(int index)
	    {
	        var e = new FarmMenuClickEvent(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMissionClose()
	    {
	        MissionFrame.gameObject.SetActive(false);
	        SetLandCollider(true);
	    }
	
	//     public void OnClickDepotClose()
	//     {
	//         DepotFrame.gameObject.SetActive(false);
	//         SetLandCollider(true);
	//     }
	    public void OnClickMissionOpen()
	    {
	        MissionFrame.gameObject.SetActive(true);
	        EventDispatcher.Instance.DispatchEvent(new FarmOperateEvent(22));
	        SetLandCollider(false);
	        //NewScrollView.ResetPosition();
	        LandMenu.SetActive(false);
	    }
	
	    public void OnClickOrderDelect()
	    {
	        var e = new FarmOperateEvent(18);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOrderRefresh()
	    {
	        var e = new FarmOperateEvent(19);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickOrderSubmit()
	    {
	        var e = new FarmOperateEvent(17);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPageUp()
	    {
	        var e = new FarmOperateEvent(20);
	        EventDispatcher.Instance.DispatchEvent(e);
	        //mTouchingFarmLand = null;
	    }
	
	    public void OnClickReturn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new FarmOperateEvent(16));
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.FarmUI));
            //var e1 = new Show_UI_Event(UIConfig.CityUI);
            //EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    public void OnCliclFarmOrder(int idnex)
	    {
	        var e = new FarmOrderListClick(idnex);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        EventDispatcher.Instance.RemoveEventListener(FarmCellTipEvent.EVENT_TYPE, OnShowFarmCellTip);
	        EventDispatcher.Instance.RemoveEventListener(FarmMenuDragEvent.EVENT_TYPE, OnFarmMenuDrag);
	        EventDispatcher.Instance.RemoveEventListener(FarmMatureRefresh.EVENT_TYPE, OnFarmMatureRefresh);
	        EventDispatcher.Instance.RemoveEventListener(FarmOrderFlyAnim.EVENT_TYPE, FlyOrderExp);
	
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
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_ShowPlantDemo.EVENT_TYPE, ShowPlantDemo);
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
	        EventDispatcher.Instance.AddEventListener(UIEvent_ShowPlantDemo.EVENT_TYPE, ShowPlantDemo);
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.FarmUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel.Bags.Resources);
	        LandMenu.SetActive(false);
	        MissionFrame.gameObject.SetActive(false);
	        SetLandCollider(true);
	        Tip.SetActive(false);
	
	        if (PlayerDataManager.Instance.GetFlag(522) &&
	            PlayerDataManager.Instance.GetFlag(521))
	        {
	            HarvestDemo.SetActive(true);
	        }
	        else
	        {
	            HarvestDemo.SetActive(false);
	        }
	
	        ShowPlantDemo(null);
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnFarmMatureRefresh(IEvent ievent)
	    {
	        var e = ievent as FarmMatureRefresh;
	        if (mMatureTimer != null)
	        {
	            StopCoroutine(mMatureTimer);
	        }
	        mMatureScend = e.Scends;
	        if (mMatureScend > 0)
	        {
	            mMatureScendMax = e.MaxTimer*60;
	            mMatureTimer = StartCoroutine(RefreshMatureTimer());
	            MatureTimeBg.gameObject.SetActive(true);
	        }
	        else
	        {
	            MatureTimeBg.gameObject.SetActive(false);
	        }
	    }
	
	    private void OnFarmMenuDrag(IEvent ievent)
	    {
	        var e = ievent as FarmMenuDragEvent;
	        if (e.Index == -1)
	        {
	            DragState = LandDragState.Invalid;
	            TouchFarmLands.Clear();
	            mTouchingFarmLand = null;
	        }
	        else
	        {
	            DragState = LandDragState.Draging;
	            TouchFarmLands.Clear();
	        }
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
	        if (TimeFormatStr == "")
	        {
	            TimeFormatStr = GameUtils.GetDictionaryText(270236);
	        }
	        var taget = timer.TargetTime;
	        if (taget > Game.Instance.ServerTime)
	        {
	            var dif = (taget - Game.Instance.ServerTime);
	            var str = string.Format("{0:D2}:{1:D2}:{2:D2}", dif.Hours, dif.Minutes, dif.Seconds);
	            lable.text = string.Format(TimeFormatStr, str);
	
	            var totalCost = (int) Math.Ceiling((float) dif.TotalSeconds/(60.0f*5))*GameUtils.OrderRefreshCost;
	            var dicStr = GameUtils.GetDictionaryText(270098);
	            str = string.Format(dicStr, totalCost);
	            LockTip.text = str;
	            ;
	        }
	        else
	        {
	            var str = "";
	            lable.text = str;
	            LockTip.text = "";
	        }
	    }
	
	    public string OnFormatOrderCool(DateTime time)
	    {
	        if (Game.Instance.ServerTime > time)
	        {
	            return "";
	        }
	        var dif = time - Game.Instance.ServerTime;
	        var ret = string.Format("{0}:{1}:{2}", dif.Hours, dif.Minutes, dif.Seconds);
	        return ret;
	    }
	
	    private void OnShowFarmCellTip(IEvent ievent)
	    {
	        var e = ievent as FarmCellTipEvent;
	        var index = e.Index;
	        var land = FarmLands.Find(l => l.DataModel.Index == index);
	        GameObject go;
	        var from = mTrans.InverseTransformPoint(land.transform.position);
	        Vector3 to;
	        Vector3 highPos;
	        switch (e.Type)
	        {
	            case OperateType.Seed:
	            case OperateType.Speedup:
	                go = mFlyPrefab;
	                to = from + new Vector3(0, 100);
	                highPos = Vector3.zero;
	                break;
	            case OperateType.Mature:
	                go = mFlyPrefab1;
	                to = mTrans.InverseTransformPoint(Depot.transform.position);
	                highPos = new Vector3(0f, MyRandom.Random(60, 200), 0f);
	                var obj = Instantiate(mFlyPrefabExp) as GameObject;
	                PlayerDataManager.Instance.PlayFlyItem(obj, land.transform, HomeObj.transform, 12, e.Exp,
	                    new Vector3(0, 100f, 0));
	                break;
	            default:
	                return;
	        }
	        ShowTip(go, from, to, highPos, e.PlantId, e.Count);
	    }
	
	    public void OnTouchFarmLand(PlowLandFrame land)
	    {
	        if (DragState == LandDragState.Invalid)
	        {
	            DragState = LandDragState.Operate;
	            TouchFarmLands.Clear();
	            TouchFarmLands.Add(land);
	        }
	        else if (DragState == LandDragState.Draging)
	        {
	            if (mTouchingFarmLand != null && mTouchingFarmLand != land)
	            {
	//第一个必须是选中的那个土地
	                if (!TouchFarmLands.Contains(mTouchingFarmLand))
	                {
	                    return;
	                }
	            }
	
	            if (!TouchFarmLands.Contains(land))
	            {
	                TouchFarmLands.Add(land);
	                EventDispatcher.Instance.DispatchEvent(new FarmLandCellClick(land.DataModel.Index, true));
	            }
	        }
	
	        Logger.Info("TouchFarmLand......{0}", land.name);
	    }
	
	    public void OpenTip()
	    {
	        Tip.SetActive(true);
	    }
	
	    private IEnumerator RefreshMatureTimer()
	    {
	        var endTime = Game.Instance.ServerTime.AddSeconds(mMatureScend);
	        while (endTime > Game.Instance.ServerTime)
	        {
	            var str = GameUtils.GetTimeDiffString(endTime);
	            MatureTime.text = str;
	            if (mMatureScendMax != 0)
	            {
	                MatureProgress.value = 1.0f - mMatureScend/(float) mMatureScendMax;
	            }
	            yield return new WaitForSeconds(0.3f);
	        }
	        MatureTimeBg.gameObject.SetActive(false);
	    }
	
	    private void ResetScrollViewPostion()
	    {
	        var pannel = MenuObject.GetComponent<UIPanel>();
	        if (pannel)
	        {
	            pannel.clipOffset = Vector2.zero;
	            MenuObject.transform.localPosition = Vector3.zero;
	        }
	    }
	
	    public void SetLandCollider(bool isEnable)
	    {
	        var __list2 = FarmLands;
	        var __listCount2 = __list2.Count;
	        for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	        {
	            var land = __list2[__i2];
	            {
	                land.ColliderObject.GetComponent<MeshCollider>().enabled = isEnable;
	            }
	        }
	    }
	
	    public void ShowPlantDemo(IEvent ievent)
	    {
	        if (PlayerDataManager.Instance.GetFlag(523) &&
	            !PlayerDataManager.Instance.GetFlag(524))
	        {
	            PlantDemo.SetActive(true);
	        }
	        else
	        {
	            PlantDemo.SetActive(false);
	        }
	    }
	
	    private void ShowTip(GameObject go, Vector3 from, Vector3 to, Vector3 highPos, int itemId, int itemCount)
	    {
	        var o = Instantiate(go) as GameObject;
	        var itemData = new ItemIdDataModel();
	        itemData.ItemId = itemId;
	        itemData.Count = itemCount;
	
	        var oTransform = o.transform;
	        //oTransform.parent = mTrans;
	        oTransform.SetParentEX(mTrans);
	        //o.SetActive(false);
	        o.SetActive(true);
	        o.SetDataSource(itemData);
	        oTransform.localScale = Vector3.one;
	        oTransform.localPosition = from;
	
	        var itemCurve = o.GetComponent<CurveMove>();
	        itemCurve.From = from;
	        itemCurve.To = to;
	        itemCurve.HighPostion = highPos;
	        itemCurve.Play();
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	        mIsClick = true;
	        DragState = LandDragState.Invalid;
	
	        EventDispatcher.Instance.AddEventListener(FarmCellTipEvent.EVENT_TYPE, OnShowFarmCellTip);
	        EventDispatcher.Instance.AddEventListener(FarmMenuDragEvent.EVENT_TYPE, OnFarmMenuDrag);
	        EventDispatcher.Instance.AddEventListener(FarmMatureRefresh.EVENT_TYPE, OnFarmMatureRefresh);
	        EventDispatcher.Instance.AddEventListener(FarmOrderFlyAnim.EVENT_TYPE, FlyOrderExp);
	
	
	        for (var i = 0; i < SeedButtons.Count; i++)
	        {
	            var j = i;
	            var deleget = new EventDelegate(() => { OnClickMenu(j); });
	            SeedButtons[i].onClick.Add(deleget);
	            var listItemLogic = SeedButtons[i].gameObject.GetComponent<ListItemLogic>();
	            listItemLogic.Index = j;
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