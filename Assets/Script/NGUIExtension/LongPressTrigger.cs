using System;
#region using

using UnityEngine;

#endregion

//长按触发器

[RequireComponent(typeof (UIWidget))]
[RequireComponent(typeof (BoxCollider))]
public class LongPressTrigger : MonoBehaviour
{
    //长按延迟时间
    public float DelaySeconds = 0.3f;
    //是否已经触发
    private bool mFiredLongPress;
    //按下的时刻
    private float mPressDownTime;
    //触发函数
    public EventDelegate OnLongPressCallback;
    public EventDelegate OnLongPressOverCallback;
    //触发GameObject
    public GameObject ToggleObject = null;

    private void OnDisable()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (mFiredLongPress)
        {
            OnLongPressOver();
        }
        mPressDownTime = 0;
        mFiredLongPress = false;

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    //当拖拽时
    private void OnDrag()
    {
        if (mFiredLongPress)
        {
            OnLongPressOver();
        }
        mPressDownTime = 0;
        mFiredLongPress = false;
    }

    private void OnEnable()
    {
#if !UNITY_EDITOR
try
{
#endif

        mPressDownTime = 0;
        mFiredLongPress = false;
        if (null != ToggleObject)
        {
            ToggleObject.SetActive(false);
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    //当长按时
    private void OnLongPress()
    {
        if (null != ToggleObject)
        {
            ToggleObject.SetActive(true);
        }

        if (null != OnLongPressCallback)
        {
            OnLongPressCallback.Execute();
        }

        //Debug.Log("OnLongPress");
    }

    private void OnLongPressOver()
    {
        if (null != ToggleObject)
        {
            ToggleObject.SetActive(false);
        }

        if (null != OnLongPressOverCallback)
        {
            OnLongPressOverCallback.Execute();
        }

        //Debug.Log("OnLongPressOver");
    }

    //响应NGUI的按下和抬起
    private void OnPress(bool state)
    {
        if (state)
        {
//按下
            mPressDownTime = Time.time;
        }
        else
        {
//抬起
            if (mFiredLongPress)
            {
                OnLongPressOver();
            }
            mPressDownTime = 0;
            mFiredLongPress = false;
        }
    }

    private void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (mPressDownTime > 0 && !mFiredLongPress)
        {
            if (Time.time > mPressDownTime + DelaySeconds)
            {
                mFiredLongPress = true;
                OnLongPress();
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