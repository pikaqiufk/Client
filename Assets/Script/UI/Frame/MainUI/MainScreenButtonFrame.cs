using System;
using UnityEngine;
using System.Collections;
using System.Security.Permissions;
using System.Collections.Generic;
using CinemaDirector;
using ClientService;
using EventSystem;


namespace GameUI
{
	public class MainScreenButtonFrame : MonoBehaviour
	{
	    public List<UIButton> BtnList;
	    public List<GameObject> BtnList1;
	    public List<GameObject> BtnList2;
	    public List<GameObject> BtnList3;
	    public List<GameObject> BtnList4;
	    // Use this for initialization
	
        //private float beginSecond = 0.3f;
	    private int currentState = 0;
	    public int mNowIndex = 0;   //当前点击的
	    private float currentSpeed = 315;
	    private float endSpeed = 315;
	    //private float mLenth = 90.0f;
	    private float radius = 53.74011537f; 
		[HideInInspector]
	    public float mLenth = 76.0f;
	    //private float mRadius = 49.49747468f;
	    public int mNowLook = -1; //正在看的
	    private int currentOpen = -1; //正在打开的
	    public int mOpenTick = -1; //开的心跳次数
	    public int mCloseTick = -1; //关的心跳次数
	    private int currentClose = -1; //正在关的
	    public DateTime CloseTime;
	
	    public int RotateSpeed = 45;
	    public int AutoColseSpeed = 5;
	    //开启时
	    private void OnEnable()
	    {
#if !UNITY_EDITOR
try
{
#endif

	        CloseTime = Game.Instance.ServerTime.AddSeconds(10);
	        EventDispatcher.Instance.AddEventListener(UIEvent_MainuiCloseList.EVENT_TYPE, OnEvent_CloseList);
	    
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

	
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_MainuiCloseList.EVENT_TYPE, OnEvent_CloseList);
	    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
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
	
	    private void OnEvent_CloseList(IEvent ievent)
	    {
	        CloseTime = Game.Instance.ServerTime;
	        CloseTheList();
	        mNowLook = -1;
	        PlayerDataManager.Instance.SelectTargetData.IsCanShow = true;
	    }
	    // Update is called once per frame
	    private float diffCount = 1.0f;
	    private void Update()
	    {
#if !UNITY_EDITOR
try
{
#endif

	        float dt = Time.deltaTime;
	        //加设30帧在执行的速度的话，之前的速度是0.3333秒一帧，那么之前的改变量X 要变为 dt * X * 30
	        diffCount = dt * RotateSpeed;//dt*45;
	        if (diffCount < 0.5)
	        {
	            diffCount = 0.5f;
	        }
	        else if (diffCount>3)
	        {
	            diffCount = 3;
	        }
	        if (currentState == 1)
	        {
	            RefreshPosition();
	        }
	        else if (currentState == 2)
	        {
	            OnClick_Button();
	        }
	        if (mOpenTick != -1)
	        {
	            SetOpenList();
	        }
	
	        if (mCloseTick != -1)
	        {
	            SetClose();
	        }
	        if (mNowLook != -1)
	        {
	
	            if (Game.Instance.ServerTime > CloseTime)
	            {
	                if (null != GuideManager.Instance && GuideManager.Instance.IsGuiding())
	                {
	                    return;
	                }
	                CloseTime = Game.Instance.ServerTime;
	                CloseTheList();
	                mNowLook = -1;
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
	
	    private void RefreshPosition()
	    {
	        var diff = endSpeed - currentSpeed;
	        var f = Math.Abs(diff);
	        if (f < 0.01f)
	        {
	            currentState = 2;
	            return;
	        }
	        float oldRate = currentSpeed;
	        if (diff > 180)
	        {
	            //diff = diff - 360;
	            currentSpeed -= 9 * diffCount;
	        }
	        else if (diff < -180)
	        {
	            //diff = diff + 360;
	            currentSpeed += 9 * diffCount;
	        }
	        else
	        {
	            if (diff < 0)
	            {
	                currentSpeed -=  9 * diffCount;
	            }
	            else
	            {
	                currentSpeed +=  9 * diffCount;
	            }
	        }
	        if (currentSpeed < 0)
	        {
	            currentSpeed += 360;
	            diff *= -1;
	        }
	        else if (currentSpeed > 360)
	        {
	            currentSpeed -= 360;
	            diff *= -1;
	        }
	        //=IF(F6>0,IF(AND($C$6>=D6,$C$6<=H6),1,0),IF(AND($C$6<=D6,$C$6>=H6),1,0))
	        if (diff > 0)
	        {
	            if (endSpeed >= oldRate && endSpeed <= currentSpeed)
	            {
	                currentSpeed = endSpeed;
	                currentState = 2;
	            }
	        }
	        else
	        {
	            if (endSpeed <= oldRate && endSpeed >= currentSpeed)
	            {
	                currentSpeed = endSpeed;
	                currentState = 2;
	            }
	        }
	        //float or = oldRate;//(oldRate + 1080) % 720;
	        //float nr = mNowRate;//(mNowRate + 1080) % 720;
	        //float er = mEndRate;//(mEndRate + 1080) % 720;
	
	        //if (or >= er && nr <= er)
	        //{
	        //    mNowRate = mEndRate;
	        //    state = 2;
	        //}
	        //else if (or >= er && nr <= er)
	        //{
	        //    mNowRate = mEndRate;
	        //    state = 2;
	        //}
	
	        ResetPosition();
	    }
	
	    private void ResetPosition()
	    {
	        int i = 0;
	        {
	            var __list1 = BtnList;
	            var __listCount1 = __list1.Count;
	            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
	            {
	                var button = (UIButton)__list1[__i1];
	                {
	                    float f = (currentSpeed + i * 90) * Mathf.Deg2Rad;
	                    var fs = Mathf.Sin(f);
	                    var fc = Mathf.Cos(f);
	                    button.transform.localPosition = new Vector3(fs * radius - 37.5f, fc * radius - 37.5f, 0);
	                    i++;
	                }
	            }
	        }
	    }
	
	    private void OnClick_Button()
	    {
	        currentState = 3;
	        if (mNowIndex == currentOpen)
	        {
	            CloseTheList();
	            return;
	        }
	        currentOpen = mNowIndex;
	        SetCurrentOpen();
	    }
	
	    private void SetOpenList()
	    {
	        List<GameObject> temp = null;
	        switch (mNowIndex)
	        {
	            case 0:
	                temp = BtnList1;
	                break;
	            case 1:
	                temp = BtnList2;
	                break;
	            case 2:
	                temp = BtnList3;
	                break;
	            case 3:
	                temp = BtnList4;
	                break;
	            default:
	                return;
	        }
	        if (mOpenTick == 0)
	        {
	            var listCount2 = temp.Count;
	            for (int i2 = 0; i2 < listCount2; ++i2)
	            {
	                var button = temp[i2];
	                if (i2 == 0)
	                {
	                    button.gameObject.SetActive(true);
	                    //Logger.Error("look Index = {0}",i2);
	                }
	                else
	                {
	                    button.gameObject.SetActive(false);
	                }
	                button.transform.localPosition = new Vector3(0, 0, 0);
	            }
	        }
	        int openCount = 0;
	        var _listCount3 = temp.Count;
	        for (int _i3 = 0; _i3 < _listCount3; ++_i3)
	        {
	            var button = temp[_i3];
	            float maxPosX = (_listCount3 - _i3) * -mLenth;
	            var btnTransform = button.transform;
	            var pos = btnTransform.localPosition; 
	            if (Mathf.Abs(maxPosX - pos.x) < 0.01f)
	            {
	                openCount++;
	                continue;
	            }
	            float fNextPos = pos.x - 15*diffCount;
	            if (fNextPos <= maxPosX)
	            {
	                btnTransform.localPosition = new Vector3(maxPosX, pos.y, 0);
	                //Logger.Error("setPos Index = {0},pox={1}", _i3, maxPosX);
	
	                openCount++;
	                continue;
	            }
	            else
	            {
	                btnTransform.localPosition = new Vector3(fNextPos, pos.y, 0);
	                //Logger.Error("setPos Index = {0},pox={1}", _i3, fNextPos);
	                if (pos.x > -mLenth)
	                {
	                    if (fNextPos < -mLenth)
	                    {
	                        if (_i3 < _listCount3 - 1)
	                        {
	                            var nextbutton = temp[_i3 + 1];
	                            nextbutton.gameObject.SetActive(true);
	                            //Logger.Error("look Index = {0}", _i3 + 1);
	                            nextbutton.transform.localPosition = new Vector3(fNextPos + mLenth, pos.y, 0);
	                            //Logger.Error("setPos Index = {0},pox={1}", _i3 + 1, fNextPos + 90);
	                            break;
	                        }
	                    }
	                    break;
	                }
	            }
	        }
	
	        if (openCount == _listCount3)
	        {
	            //Logger.Error("open over mOpenTick = {0},mNowOpen={1},mNowLook={2}", mOpenTick, mNowOpen, mNowLook);
	            currentOpen = -1;
	            mOpenTick = -1;
	            mNowLook = mNowIndex;
	            //for (int i = 0; i != 4; ++i)
	            //{
	            //    if (mNowLook != i)
	            //    {
	            //        CloseType(i);
	            //    }
	            //}
	            CloseTime = Game.Instance.ServerTime.AddSeconds(AutoColseSpeed);
	            return;
	        }
	        //int index = 0;
	        //int openCount = 0;
	        //float lastX = -1;
	        //var __listCount3 = temp.Count;
	        //for (int __i3 = 0; __i3 < __listCount3; ++__i3)
	        //{
	        //    var button = temp[__i3];
	        //    float maxPosX = (__listCount3 - index) * -90;
	        //    if (Math.Abs(lastX + 1) < 0.01 || lastX < -90)
	        //    {
	        //        button.gameObject.SetActive(true);
	        //    }
	        //    else
	        //    {
	        //        continue;
	        //    }
	        //    var btnTransform = button.transform;
	        //    var pos = btnTransform.localPosition;
	        //    if (pos.x <= maxPosX + 15 * diffCount)
	        //    {
	        //        btnTransform.localPosition = new Vector3(maxPosX, pos.y, 0);
	        //        lastX = maxPosX;
	        //        openCount++;
	        //    }
	        //    else
	        //    {
	        //        lastX = pos.x - 15 * diffCount;
	        //        btnTransform.localPosition = new Vector3(lastX, pos.y, 0);
	        //    }
	        //    index++;
	        //}
	        //if (openCount == temp.Count)
	        //{
	        //    mNowOpen = -1;
	        //    mOpenTick = -1;
	        //    mNowLook = mNowIndex;
	        //    for (int i = 0; i != 4; ++i)
	        //    {
	        //        if (mNowLook != i)
	        //        {
	        //            CloseType(i);
	        //        }
	        //    }
	        //    CloseTime = Game.Instance.ServerTime.AddSeconds(5);
	        //    return;
	        //}
	        mOpenTick++;
	    }
	
	    private void SetCurrentOpen()
	    {
	        mOpenTick = 0;
	        if (currentOpen != -1)
	        {
	            SetClose(currentOpen);
	        }
	        currentOpen = mNowLook;
	        if (currentClose == currentOpen)
	        {
	            //mOpenTick = (20 - mCloseTick) * 2;
	            SetClose(currentClose);
	            currentClose = -1;
	            mCloseTick = -1;
	        }
	    }
	
	    private void SetClose()
	    {
	        List<GameObject> temp = null;
	        switch (currentClose)
	        {
	            case 0:
	                temp = BtnList1;
	                break;
	            case 1:
	                temp = BtnList2;
	                break;
	            case 2:
	                temp = BtnList3;
	                break;
	            case 3:
	                temp = BtnList4;
	                break;
	            default:
	                return;
	        }
	        int index = 0;
	        int colseCount = 0;
	        if (mCloseTick == 0)
	        {
	            for (int i = 0; i != 4; ++i)
	            {
	                if (currentClose != i)
	                {
	                    SetClose(i);
	                }
	            }
	        }
	        {
	            var __list4 = temp;
	            var __listCount4 = __list4.Count;
	            for (int __i4 = 0; __i4 < __listCount4; ++__i4)
	            {
	                var button = __list4[__i4];
	                {
	                    var btnTransform = button.transform;
	                    var pos = btnTransform.localPosition;
	                    if (pos.x >= -18)
	                    {
	                        btnTransform.localPosition = new Vector3(0, pos.y, 0);
	                        button.gameObject.SetActive(false);
	                        colseCount++;
	                    }
	                    else
	                    {
	                        btnTransform.localPosition = new Vector3(pos.x + 18 * diffCount, pos.y, 0);
	                    }
	                    index++;
	                }
	            }
	        }
	        if (colseCount == temp.Count)
	        {
	            mCloseTick = -1;
	            currentClose = -1;
	            if (mNowLook == -1)
	            {
	                UIEvent_MainUIButtonShowEvent e = new UIEvent_MainUIButtonShowEvent(0);
	                EventDispatcher.Instance.DispatchEvent(e);
	            }
	            return;
	        }
	        mCloseTick++;
	        //if (mCloseTick >= 20)
	        //{
	        //    CloseType(mNowClose);
	        //    mCloseTick = -1;
	        //    mNowClose = -1;
	        //}
	    }
	
	    private void SetClose(int type)
	    {
	        System.Collections.Generic.List<GameObject> temp = null;
	        switch (type)
	        {
	            case 0:
	                temp = BtnList1;
	                break;
	            case 1:
	                temp = BtnList2;
	                break;
	            case 2:
	                temp = BtnList3;
	                break;
	            case 3:
	                temp = BtnList4;
	                break;
	            default:
	                return;
	        }
	        var __listCount5 = temp.Count;
	        for (int __i5 = 0; __i5 < __listCount5; ++__i5)
	        {
	            var button = temp[__i5];
	            {
	                button.SetActive(false);
	            }
	        }
	    }
	
	    private void CloseTheList()
	    {
	        mCloseTick = 0;
	        if (currentClose != -1)
	        {
	            SetClose(currentClose);
	        }
	        currentClose = mNowLook;
	        if (mOpenTick != -1 && currentOpen == currentClose)
	        {
	            //mCloseTick = (40 - mOpenTick) / 2 - 1;
	            currentOpen = -1;
	            mOpenTick = -1;
	        }
	    }
	
	    public void OnClickType1()
	    {
	        endSpeed = 315;
	        mNowIndex = 0;
	
	        if (mNowLook == 0)
	        {
	            CloseTime = Game.Instance.ServerTime;
	            CloseTheList();
	            mNowLook = -1;
	        }
	        else
	        {
	            CloseTime = Game.Instance.ServerTime.AddSeconds(10);
	            if (mNowLook != -1)
	            {
	                SetClose(mNowLook);
	                //CloseList();
	            }
	            currentState = 1;
	            mNowLook = mNowIndex;
	            UIEvent_MainUIButtonShowEvent e = new UIEvent_MainUIButtonShowEvent(1);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void OnClickType2()
	    {
	        endSpeed = 225;
	        mNowIndex = 1;
	
	        if (mNowLook == 1)
	        {
	            CloseTime = Game.Instance.ServerTime;
	            CloseTheList();
	            mNowLook = -1;
	        }
	        else
	        {
	            CloseTime = Game.Instance.ServerTime.AddSeconds(10);
	            if (mNowLook != -1)
	            {
	                SetClose(mNowLook);
	                //CloseList();
	            }
	            currentState = 1;
	            mNowLook = mNowIndex;
	            UIEvent_MainUIButtonShowEvent e = new UIEvent_MainUIButtonShowEvent(1);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void OnClickType3()
	    {
	        endSpeed = 135;
	        mNowIndex = 2;
	
	        if (mNowLook == 2)
	        {
	            CloseTime = Game.Instance.ServerTime;
	            CloseTheList();
	            mNowLook = -1;
	        }
	        else
	        {
	            CloseTime = Game.Instance.ServerTime.AddSeconds(10);
	            if (mNowLook != -1)
	            {
	                SetClose(mNowLook);
	                //CloseList();
	            }
	            currentState = 1;
	            mNowLook = mNowIndex;
	            UIEvent_MainUIButtonShowEvent e = new UIEvent_MainUIButtonShowEvent(1);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void OnClickType4()
	    {
	        endSpeed = 45;
	        mNowIndex = 3;
	
	        if (mNowLook == 3)
	        {
	            CloseTime = Game.Instance.ServerTime;
	            CloseTheList();
	            mNowLook = -1;
	        }
	        else
	        {
	            CloseTime = Game.Instance.ServerTime.AddSeconds(10);
	            if (mNowLook != -1)
	            {
	                SetClose(mNowLook);
	                //CloseList();
	            }
	            currentState = 1;
	            mNowLook = mNowIndex;
	            UIEvent_MainUIButtonShowEvent e = new UIEvent_MainUIButtonShowEvent(1);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void OnClickType(int type)
	    {
	        endSpeed = 315 - type * 90;
	        mNowIndex = type;
	
	        if (mNowLook == type)
	        {
	            CloseTime = Game.Instance.ServerTime;
	            CloseTheList();
	            mNowLook = -1;
	        }
	        else
	        {
	            CloseTime = Game.Instance.ServerTime.AddSeconds(10);
	            if (mNowLook != -1)
	            {
	                CloseTheList();
	            }
	            currentState = 1;
	            mNowLook = mNowIndex;
	            UIEvent_MainUIButtonShowEvent e = new UIEvent_MainUIButtonShowEvent(1);
	            EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void MYtext()
	    {
	        BtnList1.RemoveAt(0);
	    }
	
	}
}
