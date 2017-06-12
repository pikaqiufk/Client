using System;
using UnityEngine;
using System.Collections;

public class UISliderDouble : UISlider
{
    public UISprite MaskGround = null;

    private float FromValue = 0.0f;

    private float ToValue = 0.0f;
    
    private bool mRise = false;
    
    public float Speed;
    
    public UILabel ScheduleLabel;

    private int mMaxValue;
    public int MaxValue 
    {
        get { return mMaxValue; }
        set
        {
            if (mMaxValue == value)
            {
                return;
            }
            mMaxValue = value;
            mIsReset = 1;
            EventDelegate.Execute(onChange);
        }
    }

    private int mIsReset = 1;
    public int IsReset
    {
        get
        {
            return mIsReset;
        }
        set
        {
            mIsReset = value;
        }
    }

    public float TargetValue 
    {
        set
        {
            float val = Mathf.Clamp01(value);
            if (mIsReset == 0)
            {
                Change(FromValue, val);    
            }
            else
            {
                
                mIsReset = 0;
                FromValue = val;
                ToValue = val;
                base.value = val;
                RefreshMask(val);
                RefreshFore(val);
                EventDelegate.Execute(onChange);
            }
        }
    }


    void Awake()
    {
#if !UNITY_EDITOR
try
{
#endif

        FromValue = value;
        ToValue = value;
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

    void Start()
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

    public void Change(float frome, float to)
    {
        if (frome == to)
        {
            return;
        }

        FromValue = frome;
        ToValue = to;
         
        if (frome > to)
        {
            mRise = true;
            RefreshFore(ToValue);
        }
        else
        {
            mRise = false;
            RefreshMask(ToValue);
        }
    }

    public void RefreshMask(float fValue)
    {
        bool turnOff = false;
        if (MaskGround)
        {
            UIBasicSprite sprite = MaskGround as UIBasicSprite;
            if (isHorizontal)
            {
                if (sprite != null && sprite.type == UIBasicSprite.Type.Filled)
                {
                    if (sprite.fillDirection == UIBasicSprite.FillDirection.Horizontal ||
                        sprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
                    {
                        sprite.fillDirection = UIBasicSprite.FillDirection.Horizontal;
                        sprite.invert = isInverted;
                    }
                    sprite.fillAmount = fValue;
                }
                else
                {
                    MaskGround.drawRegion = isInverted ?
                        new Vector4(1f - fValue, 0f, 1f, 1f) :
                        new Vector4(0f, 0f, fValue, 1f);
                    MaskGround.enabled = true;
                    turnOff = fValue < 0.001f;
                }
            }
            else if (sprite != null && sprite.type == UIBasicSprite.Type.Filled)
            {
                if (sprite.fillDirection == UIBasicSprite.FillDirection.Horizontal ||
                    sprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
                {
                    sprite.fillDirection = UIBasicSprite.FillDirection.Vertical;
                    sprite.invert = isInverted;
                }
                sprite.fillAmount = fValue;
            }
            else
            {
                MaskGround.drawRegion = isInverted ?
                    new Vector4(0f, 1f - fValue, 1f, 1f) :
                    new Vector4(0f, 0f, 1f, fValue);
                MaskGround.enabled = true;
                turnOff = fValue < 0.001f;
            }
        }
        if (turnOff) MaskGround.enabled = false;
    }

    public void RefreshFore(float fValue)
    {
        base.value = fValue;
    }
    void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (FromValue == ToValue)
        {
            return;
        }
        float tikeDiff = Time.deltaTime * Speed;

        if (mRise)
        {
            FromValue -= tikeDiff;
            if (FromValue < ToValue)
            {
                FromValue = ToValue;
            }
            RefreshMask(FromValue);
        }
        else
        {
            FromValue += tikeDiff;
            if (FromValue > ToValue)
            {
                FromValue = ToValue;
            }
            RefreshFore(FromValue);
        }
    
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}
    public void OnChangeTargetValue()
    {
        if (ScheduleLabel != null)
        {
            ScheduleLabel.text = string.Format("{0}/{1}", Mathf.Floor(ToValue * MaxValue + 0.5f), MaxValue);
        }
    }

    public void OnChangeTargetFixValue()
    {
        if (ScheduleLabel != null)
        {
            int value = (int)Mathf.Floor(ToValue*MaxValue + 0.5f);
            ScheduleLabel.text = string.Format("{0}/{1}", GameUtils.ViewBigValueStr(value), GameUtils.ViewBigValueStr(MaxValue));
        }
    }
}
