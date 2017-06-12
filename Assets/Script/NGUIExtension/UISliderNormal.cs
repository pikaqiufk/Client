using System;
using UnityEngine;
using System.Collections;

public class UISliderNormal : UISlider
{
    public UILabel ScheduleLabel;
    private int mMaxValue;
    public int MaxValue
    {
        get { return mMaxValue; }
        set
        {
            mMaxValue = value;
            var lastValue = this.value;
            if (mCurrentValue >= mMaxValue)
            {
                this.value = 1.0f;
            }
            else if (MaxValue != 0)
            {
                this.value = mCurrentValue / (float)value;
            }
            else
            {
                this.value = 0.0f;
            }
            if (Math.Abs(lastValue - this.value) < 0.01)
            {
                EventDelegate.Execute(onChange);
            }
        }
    }

    private int mCurrentValue;

    private float mTargetValue;

    public float TargetValue
    {
        get { return mTargetValue; }
        set
        {
            mTargetValue = value;
            this.value = mTargetValue;
            EventDelegate.Execute(onChange);
        }
    }

    public int CurrentValue
    {
        get { return mCurrentValue; }
        set
        {
            mCurrentValue = value;

            var lastValue = this.value;
            if (mCurrentValue >= mMaxValue)
            {
                this.value = 1.0f;
            }
            else if (MaxValue != 0)
            {
                this.value = value / (float)mMaxValue;    
            }
            else
            {
                this.value = 0.0f;
            }

            if (Math.Abs(lastValue - this.value) < 0.01)
            {
                EventDelegate.Execute(onChange);
            }
        }
    }

    public void OnChangeValue()
    {
        if (ScheduleLabel != null)
        {
            //ScheduleLabel.text = string.Format("{0}/{1}", mCurrentValue, mMaxValue);
            ScheduleLabel.text = string.Format("{0}/{1}", Mathf.Floor(base.value * mMaxValue + 0.5f), mMaxValue);
        }
    }

    public void OnChangeValueCountAndRate()
    {
        if (ScheduleLabel != null)
        {
            ScheduleLabel.text = string.Format("{0}/{1}({2:0.#}%)", (int)Mathf.Floor(base.value * mMaxValue + 0.5f), mMaxValue, base.value*100);
        }
    }
}
