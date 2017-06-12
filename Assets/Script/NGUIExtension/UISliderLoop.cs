using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventSystem;

public class UISliderLoop : UISlider
{
    private float mTargetValue;

    public float TargetValue
    {
        get { return mTargetValue; }
        set
        {
            if (mTargetValue != value)
            {
                mTargetValue = value;
            }
            OnChangeValue();
        }
    }

    private float mMoveValue;

    public float MoveValue
    {
        get { return mMoveValue; }
        set
        {
            mMoveValue = value;
            TargetValue = mMoveValue;
            this.value = TargetValue;
            //OnChangeValue();
        }
    }

    //public float MoveValue { get; set; }

    public float Speed;

    public UILabel ScheduleLabel;

    private List<int> mMaxValueList;
    private int CurIndex { get; set; }
    public List<int> MaxValueList
    {
        get { return mMaxValueList; }
        set
        {
            mMaxValueList = value;
            CurIndex = 0;
            mMoveValue = mTargetValue - (float)Math.Floor(mTargetValue);
            mTargetValue = mMoveValue;
            this.value = mTargetValue;
        }
    }
    
    void Start()
    {
#if !UNITY_EDITOR
try
{
#endif

        value = TargetValue - (int)TargetValue;
        MoveValue = TargetValue;
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        float diff = TargetValue - MoveValue;
        if (Math.Abs(diff) < 0.01)
        {
            return;
        }
        float tikeDiff = 0.0f;
        if (diff > 0.0f)
        {
            tikeDiff = Time.deltaTime * Speed;
        }
        else if (diff < 0.0f)
        {
            tikeDiff = -Time.deltaTime * Speed;
        }

        int nLast = (int) MoveValue;

        
        mMoveValue += tikeDiff;
        if ((diff > 0 && MoveValue > TargetValue)
            || (diff < 0 && MoveValue < TargetValue)
            ||Math.Abs(TargetValue - MoveValue) < 0.01)
        {
            mMoveValue = TargetValue;
            value = MoveValue - (float)Math.Floor(MoveValue);
            TargetValue = value;
            mMoveValue = value;

            int now = (int)MoveValue;
            if (nLast == now)
            {
                if (tikeDiff > 0)
                {
                    CurIndex++;
                }
                else
                {
                    CurIndex--;
                }
            }
            OnChangeValue();
            return;
        }

        int now1 = (int) MoveValue;
        if (nLast > now1)
        {
            CurIndex--;
        }
        else if (nLast < now1)
        {
            CurIndex++;
        }
        value = MoveValue - (float)Math.Floor(MoveValue);
    
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
        if (ScheduleLabel != null && mMaxValueList != null && mMaxValueList.Count > 0)
        {
            int max = 0;
            if (mMaxValueList.Count > CurIndex)
            {
                max = mMaxValueList[CurIndex];
            }
            else
            {
                max = mMaxValueList[0];
            }
            ScheduleLabel.text = string.Format("{0}/{1}", Mathf.Floor(max * value + 0.5f), max);
        }
    }
}
