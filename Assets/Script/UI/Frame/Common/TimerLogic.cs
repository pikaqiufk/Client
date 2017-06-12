#region using

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

public class TimerLogic : MonoBehaviour
{
    public bool IsAdd;
    public bool IsStart;
    private int mCountdown;
    private DateTime mTargetTime;
    public FormatTime OnFormatTime;
    public UILabel TextLabel;
    public Coroutine TimerCoroutine;
    public List<EventDelegate> UpdateDelegate = new List<EventDelegate>();

    public int Countdown
    {
        get { return mCountdown; }
        set
        {
            mCountdown = value;
            TargetTime = Game.Instance.ServerTime.AddSeconds(mCountdown);
        }
    }

    public DateTime TargetTime
    {
        get { return mTargetTime; }
        set
        {
            mTargetTime = value;
            StopTimer();

            IsStart = true;
            if (gameObject.activeInHierarchy)
            {
                SartTimer();
            }
            else
            {
                InitFormat();
            }
        }
    }

    private IEnumerator CountdownTimer()
    {
        InitFormat();
        if (IsAdd)
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                InitFormat();
            }
        }
        while (TargetTime >= Game.Instance.ServerTime)
        {
            yield return new WaitForSeconds(0.3f);
            InitFormat();
        }
        InitFormat();
        StopTimer();
    }

    private void InitFormat()
    {
        if (OnFormatTime != null)
        {
            var str = OnFormatTime(TargetTime);
            TextLabel.text = str;
        }
        else
        {
            EventDelegate.Execute(UpdateDelegate);
        }
    }

    private void OnDestroy()
    {
#if !UNITY_EDITOR
try
{
#endif
        UpdateDelegate.Clear();
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

        if (IsStart)
        {
            SartTimer();
        }

#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
    }

    private void SartTimer()
    {
        if (IsAdd)
        {
            TimerCoroutine = StartCoroutine(CountdownTimer());
        }
        else
        {
            if (mTargetTime > Game.Instance.ServerTime)
            {
                TimerCoroutine = StartCoroutine(CountdownTimer());
            }
            else
            {
                IsStart = false;
                InitFormat();
            }
        }
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

    public void StopTimer()
    {
        if (TimerCoroutine != null)
        {
            StopCoroutine(TimerCoroutine);
            TimerCoroutine = null;
        }
        IsStart = false;
    }

    public delegate string FormatTime(DateTime time);
}