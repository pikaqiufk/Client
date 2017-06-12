#region using

using System;
using System.Collections.Generic;
using GameUI.Assets.Script.UI.MainUIbtn;
using EventSystem;
using GameUI;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MainButtonAnimation : MonoBehaviour
	{
        //public static Vector3 s_buttonPos = Vector3.zero;
	    public static Dictionary<string, Vector3> s_ButtonPosList = new Dictionary<string, Vector3>();
	    public static Dictionary<string, Vector3> s_NamePosList = new Dictionary<string, Vector3>();
	    public static MainButtonAnimation s_MainBtn;
	    private static bool s_isInit;
	    private string theName = "";
	    //每个按钮的便宜
	    public float BtnOffset = 76;
	    //功能按钮飞的Tick
	    public float FlyTick = 20;
	    //起飞坐标
	    private Vector3 startPos = Vector3.zero;
	    private int accumulateHit;
	    private float accuFly;
	    private GameObject gameObj;
	    private int thePos;
	    private bool inAnim; //正在动画
	    public bool ischange;
	    private bool isFlying;
	    private bool isShining;
	    //主按钮的偏移
	    public float MainBtnOffset = 80;
	    private Vector3 curPosition = Vector3.zero;
	    private Action theCallback;
	    private Transform objTrans;
	    private readonly Dictionary<int, List<GameObject>> buttonDict = new Dictionary<int, List<GameObject>>();
	    private GameObject theAnim;
	    private MainScreenButtonFrame mainScreenButton;
	    private bool isActive;
	    //先功能按钮在中间闪的时间
	    public float ShineDelayTime = 2.2f;
	    private DateTime shiningContinue;
	    public GameObject UiAnim;
	
	    public Vector3 GetInsertObjPosition()
	    {
	        var p = Vector3.zero;
	        if (null != gameObj)
	        {
	            var root = objTrans.root;
	            if (null != root)
	            {
	                p = root.transform.InverseTransformPoint(objTrans.position);
	            }
	        }
	
	        return p;
	    }
	
	    public Vector3 GetPosition(string str)
	    {
	        var t = gameObject.transform;
	        var root = t.root;
	        var parent = t.parent;
	        if (null != root && null != parent)
	        {
	            var go = parent.FindChild(str);
	            if (null != go)
	            {
	                return root.transform.InverseTransformPoint(go.transform.position);
	            }
	        }
	
	        {
	            var go = t.FindChildRecursive(str);
	            if (null != go)
	            {
	                return root.transform.InverseTransformPoint(go.transform.position);
	            }
	        }
	
	
	        return Vector3.zero;
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        s_MainBtn = null;
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, OnEvent_RunAnim);
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnEvent_RunAnim(IEvent ievent)
	    {
	        var e = ievent as UIEvent_PlayMainUIBtnAnimEvent;
	        if (null == e)
	        {
	            return;
	        }
	
	        var objTransform = gameObject.transform;
	        for (int i = 0, imax = objTransform.childCount; i < imax; ++i)
	        {
	            var root = objTransform.GetChild(i);
	            var rootTransform = root.transform;
	            if (rootTransform.Find(e.BtnName) != null)
	            {
	                var datafind = rootTransform.Find(e.BtnName).gameObject;
	                gameObj = datafind;
	            }
	        }
	        mainScreenButton = gameObject.GetComponent<MainScreenButtonFrame>();
	        {
	            // foreach(var data in myMainUiButtonLogic.BtnList)
	            var __enumerator12 = (mainScreenButton.BtnList).GetEnumerator();
	            while (__enumerator12.MoveNext())
	            {
	                var data = __enumerator12.Current;
	                {
	                    if (data.name == e.BtnName)
	                    {
	                        return;
	                    }
	                }
	            }
	        }
	        {
	            // foreach(var data in myMainUiButtonLogic.BtnList1)
	            var __enumerator13 = (mainScreenButton.BtnList1).GetEnumerator();
	            while (__enumerator13.MoveNext())
	            {
	                var data = __enumerator13.Current;
	                {
	                    if (data.name == e.BtnName)
	                    {
	                        if (mainScreenButton.mNowLook != 0)
	                        {
	                            mainScreenButton.OnClickType1();
	                        }
	                        return;
	                    }
	                }
	            }
	        }
	        {
	            // foreach(var data in myMainUiButtonLogic.BtnList2)
	            var __enumerator14 = (mainScreenButton.BtnList2).GetEnumerator();
	            while (__enumerator14.MoveNext())
	            {
	                var data = __enumerator14.Current;
	                {
	                    if (data.name == e.BtnName)
	                    {
	                        if (mainScreenButton.mNowLook != 1)
	                        {
	                            mainScreenButton.OnClickType2();
	                        }
	                        return;
	                    }
	                }
	            }
	        }
	        {
	            // foreach(var data in myMainUiButtonLogic.BtnList3)
	            var __enumerator15 = (mainScreenButton.BtnList3).GetEnumerator();
	            while (__enumerator15.MoveNext())
	            {
	                var data = __enumerator15.Current;
	                {
	                    if (data.name == e.BtnName)
	                    {
	                        if (mainScreenButton.mNowLook != 2)
	                        {
	                            mainScreenButton.OnClickType3();
	                        }
	                        return;
	                    }
	                }
	            }
	        }
	        {
	            // foreach(var data in myMainUiButtonLogic.BtnList4)
	            var __enumerator16 = (mainScreenButton.BtnList4).GetEnumerator();
	            while (__enumerator16.MoveNext())
	            {
	                var data = __enumerator16.Current;
	                {
	                    if (data.name == e.BtnName)
	                    {
	                        if (mainScreenButton.mNowLook != 3)
	                        {
	                            mainScreenButton.OnClickType4();
	                        }
	                        return;
	                    }
	                }
	            }
	        }
	
	        UIaminition(e.BtnName);
	        theCallback = e.CallBack;
	    }
	
	    //固定帧数刷新动画
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
		    return;

	        mainScreenButton = gameObject.GetComponent<MainScreenButtonFrame>();
	        BtnOffset = mainScreenButton.mLenth;
	        if (mainScreenButton == null)
	        {
	            Logger.Log2Bugly(" myMainUiButtonLogic == null");
	            return;
	        }
	        s_MainBtn = this;
	
	        buttonDict.Add(0, mainScreenButton.BtnList1);
	        buttonDict.Add(1, mainScreenButton.BtnList2);
	        buttonDict.Add(2, mainScreenButton.BtnList3);
	        buttonDict.Add(3, mainScreenButton.BtnList4);
	
	        if (GameSetting.Instance.EnableNewFunctionTip == true)
	        {
	            if (!s_isInit)
	            {
	                s_isInit = true;
	                var root =
	                    mainScreenButton.BtnList1[0].gameObject.transform.root.GetComponent<UIRoot>()
	                        .gameObject.transform;
	                {
	                    // foreach(var go in myMainUiButtonLogic.BtnList1)
	                    var __enumerator1 = (mainScreenButton.BtnList1).GetEnumerator();
	                    while (__enumerator1.MoveNext())
	                    {
	                        var go = __enumerator1.Current;
	                        {
	                            var goTransform = go.transform;
	
	                            var p = root.InverseTransformPoint(goTransform.position);
	                            s_NamePosList.Add(go.name, p);
	
	                            p = root.InverseTransformPoint(goTransform.parent.position);
	                            s_ButtonPosList.Add(go.name, p);
	                        }
	                    }
	                }
	                {
	                    // foreach(var go in myMainUiButtonLogic.BtnList2)
	                    var __enumerator2 = (mainScreenButton.BtnList2).GetEnumerator();
	                    while (__enumerator2.MoveNext())
	                    {
	                        var go = __enumerator2.Current;
	                        {
	                            var goTransform = go.transform;
	
	                            var p = root.InverseTransformPoint(goTransform.position);
	                            s_NamePosList.Add(go.name, p);
	
	                            p = root.InverseTransformPoint(goTransform.parent.position);
	                            s_ButtonPosList.Add(go.name, p);
	                        }
	                    }
	                }
	                {
	                    // foreach(var go in myMainUiButtonLogic.BtnList3)
	                    var __enumerator3 = (mainScreenButton.BtnList3).GetEnumerator();
	                    while (__enumerator3.MoveNext())
	                    {
	                        var go = __enumerator3.Current;
	                        {
	                            var goTransform = go.transform;
	
	                            var p = root.InverseTransformPoint(goTransform.position);
	                            s_NamePosList.Add(go.name, p);
	
	                            p = root.InverseTransformPoint(goTransform.parent.position);
	                            s_ButtonPosList.Add(go.name, p);
	                        }
	                    }
	                }
	                {
	                    // foreach(var go in myMainUiButtonLogic.BtnList4)
	                    var __enumerator4 = (mainScreenButton.BtnList4).GetEnumerator();
	                    while (__enumerator4.MoveNext())
	                    {
	                        var go = __enumerator4.Current;
	                        {
	                            var goTransform = go.transform;
	
	                            var p = root.InverseTransformPoint(goTransform.position);
	                            s_NamePosList.Add(go.name, p);
	
	                            p = root.InverseTransformPoint(goTransform.parent.position);
	                            s_ButtonPosList.Add(go.name, p);
	                        }
	                    }
	                }
	            }
	            {
	                // foreach(var table in GameLogic.Instance.GuideTrigger.NewFunction)
	                var __enumerator7 = (GameLogic.Instance.GuideTrigger.NewFunction).GetEnumerator();
	                while (__enumerator7.MoveNext())
	                {
	                    var table = __enumerator7.Current;
	                    {
	//判断哪些新系统是开放的
	                        if (-1 == table.FlagPrepose || PlayerDataManager.Instance.GetFlag(table.FlagPrepose))
	                        {
	//判断标记位
	                            continue;
	                        }
	
	                        foreach (var pair in buttonDict)
	                        {
	                            foreach (var btn in pair.Value)
	                            {
	                                if (0 == btn.gameObject.name.CompareTo(table.Name))
	                                {
	                                    pair.Value.Remove(btn);
	                                    break;
	                                }
	                            }
	                        }
	                    }
	                }
	            }
	            {
	                // foreach(var pair in mRelationship)
	                var __enumerator10 = (buttonDict).GetEnumerator();
	                while (__enumerator10.MoveNext())
	                {
	                    var pair = __enumerator10.Current;
	                    {
	                        foreach (var go in pair.Value)
	                        {
	                            var btn = go.GetComponentInChildren<UIButton>();
	                            if (null != btn)
	                            {
	                                var next = btn.gameObject.GetComponent<OnClickNextGuide>();
	                                if (null == next)
	                                {
	                                    next = btn.gameObject.AddComponent<OnClickNextGuide>();
	                                }
	                                next.GuideStepList.Clear();
	
	                                foreach (var table in GameLogic.Instance.GuideTrigger.NewFunction)
	                                {
	                                    if (0 == table.Name.CompareTo(go.name))
	                                    {
	                                        next.GuideStepList.Add(table.Flag);
	                                        break;
	                                    }
	                                }
	                            }
	                        }
	                    }
	                }
	            }
	            {
	                // foreach(var pair in mRelationship)
	                var __enumerator11 = (buttonDict).GetEnumerator();
	                while (__enumerator11.MoveNext())
	                {
	                    var pair = __enumerator11.Current;
	                    {
	                        if (0 == pair.Value.Count)
	                        {
	                            mainScreenButton.BtnList[pair.Key].gameObject.SetActive(false);
	                        }
	                    }
	                }
	            }
	
	            /*
	                myMainUiButtonLogic.BtnList[0].gameObject.SetActive(false);
	                myMainUiButtonLogic.BtnList[3].gameObject.SetActive(false);
	                myMainUiButtonLogic.BtnList2.RemoveAt(2);
	                myMainUiButtonLogic.BtnList2.RemoveAt(0);
	                */
	        }
	        EventDispatcher.Instance.AddEventListener(UIEvent_PlayMainUIBtnAnimEvent.EVENT_TYPE, OnEvent_RunAnim);
	
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    //公用动画播放函数:找到gameobject，初始化系数。
	    public void UIaminition(string data)
	    {
		    return;

	        var mytTrigger = gameObject.GetComponent<MainButtonAnimeTrigger>();
	        mainScreenButton = gameObject.GetComponent<MainScreenButtonFrame>();
	        Transform root;
	        var objTransform = gameObject.transform;
	        for (int i = 0, imax = objTransform.childCount; i < imax; ++i)
	        {
	            root = objTransform.GetChild(i);
	            var rootTrans = root.transform;
	            if (rootTrans.Find(data) != null)
	            {
	                gameObj = rootTrans.Find(data).gameObject;
	                if (gameObj)
	                {
	                    objTrans = gameObj.transform;
	                }
	            }
	        }
	        if (gameObj != null)
	        {
	            switch (objTrans.parent.name)
	            {
	                case "C1":
	                    if (mytTrigger.MainBtn[0].activeSelf == false)
	                    {
	                        if (mainScreenButton.BtnList[0].gameObject == mytTrigger.MainBtn[0])
	                        {
	                            isFlying = true;
	                            curPosition =
	                                transform.Find("C").Find(mainScreenButton.BtnList[0].gameObject.name).localPosition +
	                                new Vector3(MainBtnOffset, 0f, 0f);
	
	                            mainScreenButton.BtnList.RemoveAt(0);
	                            mainScreenButton.BtnList.Insert(0, gameObj.GetComponent<UIButton>());
	                        }
	                        else
	                        {
	                            mainScreenButton.BtnList[0].gameObject.SetActive(false);
	                            mainScreenButton.BtnList[0].gameObject.transform.parent = objTrans.parent;
	                            if (1 == thePos && 0 == objTrans.name.CompareTo("BtnSkill"))
	                            {
	                                thePos = 0; //暂时先临时解决
	                            }
	                            mainScreenButton.BtnList1.Insert(thePos, mainScreenButton.BtnList[0].gameObject);
	                            mainScreenButton.BtnList[0] = mytTrigger.MainBtn[0].GetComponent<UIButton>();
	                            mainScreenButton.BtnList[0].gameObject.SetActive(true);
	                            if (mainScreenButton.mNowLook == 0)
	                            {
	                                break;
	                            }
	                            mainScreenButton.OnClickType1();
	                        }
	                        break;
	                    }
	                    if (mainScreenButton.mNowLook == 0)
	                    {
	                        break;
	                    }
	                    mainScreenButton.OnClickType1();
	                    break;
	                case "C2":
	                    if (mytTrigger.MainBtn[1].activeSelf == false)
	                    {
	                        if (mainScreenButton.BtnList[1].gameObject == mytTrigger.MainBtn[1])
	                        {
	                            isFlying = true;
	                            curPosition =
	                                transform.Find("C").Find(mainScreenButton.BtnList[1].gameObject.name).localPosition +
	                                new Vector3(MainBtnOffset, 0f, 0f);
	
	                            mainScreenButton.BtnList.RemoveAt(1);
	                            mainScreenButton.BtnList.Insert(1, gameObj.GetComponent<UIButton>());
	                        }
	                        else
	                        {
	                            mainScreenButton.BtnList[1].gameObject.SetActive(false);
	                            mainScreenButton.BtnList[1].gameObject.transform.parent = objTrans.parent;
	                            mainScreenButton.BtnList1.Insert(thePos, mainScreenButton.BtnList[1].gameObject);
	                            mainScreenButton.BtnList[1] = mytTrigger.MainBtn[1].GetComponent<UIButton>();
	                            mainScreenButton.BtnList[1].gameObject.SetActive(true);
	                            if (mainScreenButton.mNowLook == 1)
	                            {
	                                break;
	                            }
	                            mainScreenButton.OnClickType2();
	                        }
	                        break;
	                    }
	                    if (mainScreenButton.mNowLook == 1)
	                    {
	                        break;
	                    }
	                    mainScreenButton.OnClickType2();
	                    break;
	                case "C3":
	                    if (mytTrigger.MainBtn[2].activeSelf == false)
	                    {
	                        if (mainScreenButton.BtnList[2].gameObject == mytTrigger.MainBtn[2])
	                        {
	                            isFlying = true;
	                            curPosition =
	                                transform.Find("C").Find(mainScreenButton.BtnList[2].gameObject.name).localPosition +
	                                new Vector3(MainBtnOffset, 0f, 0f);
	
	                            mainScreenButton.BtnList.RemoveAt(2);
	                            mainScreenButton.BtnList.Insert(2, gameObj.GetComponent<UIButton>());
	                        }
	                        else
	                        {
	                            mainScreenButton.BtnList[2].gameObject.SetActive(false);
	                            mainScreenButton.BtnList[2].gameObject.transform.parent = objTrans.parent;
	                            mainScreenButton.BtnList1.Insert(thePos, mainScreenButton.BtnList[2].gameObject);
	                            mainScreenButton.BtnList[2] = mytTrigger.MainBtn[2].GetComponent<UIButton>();
	                            mainScreenButton.BtnList[2].gameObject.SetActive(true);
	                            if (mainScreenButton.mNowLook == 2)
	                            {
	                                break;
	                            }
	                            mainScreenButton.OnClickType3();
	                        }
	                        break;
	                    }
	                    if (mainScreenButton.mNowLook == 2)
	                    {
	                        break;
	                    }
	                    mainScreenButton.OnClickType3();
	                    break;
	                case "C4":
	                    if (mytTrigger.MainBtn[3].activeSelf == false)
	                    {
	                        if (mainScreenButton.BtnList[3].gameObject == mytTrigger.MainBtn[3])
	                        {
	                            isFlying = true;
	                            curPosition =
	                                transform.Find("C").Find(mainScreenButton.BtnList[3].gameObject.name).localPosition +
	                                new Vector3(MainBtnOffset, 0f, 0f);
	                            mainScreenButton.BtnList.RemoveAt(3);
	                            mainScreenButton.BtnList.Insert(3, gameObj.GetComponent<UIButton>());
	                        }
	                        else
	                        {
	                            mainScreenButton.BtnList[3].gameObject.SetActive(false);
	                            mainScreenButton.BtnList[3].gameObject.transform.parent = objTrans.parent;
	                            mainScreenButton.BtnList1.Insert(thePos, mainScreenButton.BtnList[3].gameObject);
	                            mainScreenButton.BtnList[3] = mytTrigger.MainBtn[3].GetComponent<UIButton>();
	                            mainScreenButton.BtnList[3].gameObject.SetActive(true);
	                            if (mainScreenButton.mNowLook == 3)
	                            {
	                                break;
	                            }
	                            mainScreenButton.OnClickType4();
	                        }
	                        break;
	                    }
	                    if (mainScreenButton.mNowLook == 3)
	                    {
	                        break;
	                    }
	                    mainScreenButton.OnClickType4();
	                    break;
	                default:
	                    return;
	            }
	            inAnim = true;
	            thePos = mytTrigger.FindBtnByname(data);
	            isShining = true;
	            shiningContinue = DateTime.Now.AddSeconds(ShineDelayTime);
	            gameObj.SetActive(true);
	            objTrans.position = Vector3.zero;
	            startPos = objTrans.localPosition;
	            gameObj.gameObject.SetActive(true);
	            theName = objTrans.parent.name;
	            var notice = objTrans.FindChild("NoticeStatus");
	            if (notice != null)
	            {
	                isActive = notice.gameObject.activeSelf;
	                notice.gameObject.SetActive(false);
	            }
	        }
	    }
	
	    private void Update()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        if (inAnim)
	        {
	            if (isShining)
	            {
	                if (theAnim == null)
	                {
	                    if (UiAnim != null)
	                    {
	                        theAnim = Instantiate(UiAnim, Vector3.zero, Quaternion.identity) as GameObject;
	                        if (theAnim != null)
	                        {
	                            var myAnimTrans = theAnim.transform;
	                            myAnimTrans.parent = gameObj.gameObject.transform;
	                            myAnimTrans.localScale = new Vector3(1f, 1f, 1f);
	                            var TiShiAnNiu = myAnimTrans.FindChild("TiShiAnNiu");
	                            TiShiAnNiu.GetComponent<UISprite>().depth = 100;
	                        }
	                    }
	                }
	                if (DateTime.Now > shiningContinue)
	                {
	                    if (theAnim != null)
	                    {
	                        Destroy(theAnim);
	                    }
	                    Transform childTransform;
	                    int count;
	                    if (isFlying)
	                    {
	                        ischange = true;
	                        if (accuFly <= FlyTick)
	                        {
	                            objTrans.localPosition =
	                                Vector3.Lerp
	                                    (
	                                        startPos,
	                                        curPosition,
	                                        accuFly/FlyTick
	                                    );
	                            accuFly++;
	                        }
	                        else
	                        {
	                            objTrans.parent =
	                                gameObject.GetComponent<MainButtonAnimeTrigger>().MainBtn[0].transform.parent;
	                            switch (theName)
	                            {
	                                case "C1":
	
	                                    mainScreenButton.OnClickType1();
	                                    break;
	                                case "C2":
	
	                                    mainScreenButton.OnClickType2();
	                                    break;
	                                case "C3":
	
	                                    mainScreenButton.OnClickType3();
	                                    break;
	                                case "C4":
	
	                                    mainScreenButton.OnClickType4();
	                                    break;
	                                default:
	                                    return;
	                            }
	                            isShining = false;
	                        }
	                    }
	                    else
	                    {
	                        for (var i = 0; i < thePos; i++)
	                        {
	                            switch (objTrans.parent.name)
	                            {
	                                case "C1":
	                                    childTransform = mainScreenButton.BtnList1[i].transform;
	                                    break;
	                                case "C2":
	                                    childTransform = mainScreenButton.BtnList2[i].transform;
	                                    break;
	                                case "C3":
	                                    childTransform = mainScreenButton.BtnList3[i].transform;
	                                    break;
	                                case "C4":
	                                    childTransform = mainScreenButton.BtnList4[i].transform;
	                                    break;
	                                default:
	                                    return;
	                            }
	                            if (accumulateHit < 10)
	                            {
	                                childTransform.localPosition = new Vector3(childTransform.localPosition.x - 8f, 0f, 0f);
	                            }
	                        }
	                        accumulateHit++;
	                        switch (objTrans.parent.name)
	                        {
	                            case "C1":
	                                count = mainScreenButton.BtnList1.Count;
	                                break;
	                            case "C2":
	                                count = mainScreenButton.BtnList2.Count;
	                                break;
	                            case "C3":
	                                count = mainScreenButton.BtnList3.Count;
	                                break;
	                            case "C4":
	                                count = mainScreenButton.BtnList4.Count;
	                                break;
	                            default:
	                                return;
	                        }
	                        if (accuFly <= FlyTick)
	                        {
	                            objTrans.localPosition =
	                                Vector3.Lerp
	                                    (
	                                        startPos,
	                                        new Vector3(
	                                            objTrans.parent.position.x -
	                                            (count - thePos + 1)*BtnOffset, 0f, 0f),
	                                        accuFly/FlyTick
	                                    );
	                            accuFly++;
	                        }
	                        else
	                        {
	                            isShining = false;
	                        }
	                    }
	                }
	            }
	
	            if (isShining == false)
	            {
	                if (!isFlying)
	                {
	                    switch (objTrans.parent.name)
	                    {
	                        case "C1":
	                            mainScreenButton.BtnList1.Insert(thePos, gameObj);
	
	                            break;
	                        case "C2":
	                            mainScreenButton.BtnList2.Insert(thePos, gameObj);
	
	                            break;
	                        case "C3":
	                            mainScreenButton.BtnList3.Insert(thePos, gameObj);
	
	                            break;
	                        case "C4":
	                            mainScreenButton.BtnList4.Insert(thePos, gameObj);
	
	                            break;
	                        default:
	                            return;
	                    }
	                }
	
	                isFlying = false;
	                accuFly = 0;
	                accumulateHit = 0;
	                inAnim = false;
	                ischange = false;
	                if (null != theCallback)
	                {
	                    theCallback();
	                    theCallback = null;
	                }
	
	                var notice = objTrans.FindChild("NoticeStatus");
	                if (notice != null)
	                {
	                    notice.gameObject.SetActive(isActive);
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
	}
}