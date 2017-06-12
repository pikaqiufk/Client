using System;
using UnityEngine;
using System.Collections;

public class UISliderMove : UISlider
{
    public float mTargetValue = -1.0f;

    public float TargetValue 
    {
        get { return mTargetValue; }
        set
        {
            mTargetValue = value;
        }
    }

    public int mMaxValue;
    private bool mIsReset = true;
    public int MaxValue
    {
        get { return mMaxValue; }
        set
        {
            var lastValue = this.value;
            mMaxValue = value;
            if (mMaxValue == 0)
            {
                TargetValue = 0;
                this.value = 0.0f;
            }
            else
            {
                TargetValue = (float)CurrentValue / mMaxValue;
                this.value = TargetValue;    
            }
            mIsReset = true;
            if (Math.Abs(lastValue - this.value) < 0.01)
            {
                EventDelegate.Execute(onChange);
            }
        }
    }

    public int mCurrentValue = -1;
    public int CurrentValue
    {
        get { return mCurrentValue; }
        set
        {
            mCurrentValue = value;
            TargetValue = (float)mCurrentValue / MaxValue;   
            if (mIsReset)
            {
                this.value = TargetValue;
                mIsReset = false;
            }
        }
    }
    public int mCurrentValueEx = -1;
    public int CurrentValueEx
    {
        get { return mCurrentValueEx; }
        set
        {
            mCurrentValueEx = value;
            TargetValue = (float)mCurrentValueEx / MaxValue;
            this.value = TargetValue;
        }
    }
    public float Speed;
    public UILabel ScheduleLabel;
    
	void Start ()
	{
#if !UNITY_EDITOR
try
{
#endif

	    value = TargetValue;
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
	// Update is called once per frame
	void Update () 
    {
#if !UNITY_EDITOR
try
{
#endif

        float mDiff = TargetValue - value;
        if (mDiff == 0.0f)
        {
            return;
        }
        float tikeDiff = 0.0f;
        if (mDiff > 0.0f)
        {
            tikeDiff = Time.deltaTime * Speed;
        }
        else if (mDiff < 0.0f)
        {
            tikeDiff = -Time.deltaTime * Speed;
        }

        value += tikeDiff;
        if ((mDiff > 0 && value > TargetValue)
            || (mDiff < 0 && value < TargetValue))
        {
            value = TargetValue;
        }
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
    public void OnChangeValue()
    {
        if (ScheduleLabel != null)
        {
            ScheduleLabel.text = string.Format("{0}/{1}", Mathf.Floor(MaxValue * value + 0.5f), MaxValue);
        }
    }
}
