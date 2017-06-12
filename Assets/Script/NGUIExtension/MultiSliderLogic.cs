using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiSliderLogic : MonoBehaviour
{
    public UISliderNormal SliderMove;
    public List<UISprite> ForeSprites;

    public UILabel LayerLable;
    private UISprite mBottomSprite;
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
    private int mLayerCount;
    public int LayerCount
    {
        get { return mLayerCount; }
        set
        {
            mLayerCount = value;
            mRest = true;
        }
    }
    private int mMaxValue;
    public int MaxValue
    {
        get { return mMaxValue; }
        set
        {
            mMaxValue = value;
            mRest = true;

        }
    }
    private int mTargetValue;
    public int TargetValue
    {
        get { return mTargetValue; }
        set
        {
            mTargetValue = value;
            if (mMaxValue == 0)
            {
                return;
            }
            if (mRest == true)
            {
                mNowValue = mTargetValue;
                mRest = false;
                ResetSlider();
            }
            else
            {
                ChangeSlider();
            }
            UpdateChangeVlaueLable();
        }
    }
    private float PerLayerCount
    {
        get
        {
            return mMaxValue / (float)LayerCount;
        }
    }
    private bool mRest;
    private int mNowValue;
    private int mIsReset;
    public int IsReset
    {
        get
        {
            return mIsReset;
        }
        set
        {
            mIsReset = value;
            mRest = mIsReset == 1;
        }
    }
    private void ResetSlider()
    {
        mChangePairs.Clear();
        var nowCountFloat = mNowValue / PerLayerCount;
        var nowCount = (int)(nowCountFloat);
        var v = nowCountFloat - nowCount;
        if (Math.Abs(v) < 0.01f && nowCount > 0)
        {
            v = 1.0f;
            nowCount--;
        }
        UpdateLayerCount(nowCount);
        SliderMove.value = v;
    }
    private void UpdateLayerCount(int nowLayer, int nextLayer = -1)
    {
        {
            var __list1 = ForeSprites;
            var __listCount1 = __list1.Count;
            for (int __i1 = 0; __i1 < __listCount1; ++__i1)
            {
                var sprite = __list1[__i1];
                {
                    sprite.gameObject.SetActive(false);
                }
            }
        }
        if (nowLayer == 0)
        {
            SliderMove.foregroundWidget = ForeSprites[0];
        }
        else
        {
            nowLayer = nowLayer % ForeSprites.Count;
            SliderMove.foregroundWidget = ForeSprites[nowLayer];
            if (nextLayer == -1)
            {
                nextLayer = (nowLayer + ForeSprites.Count - 1) % ForeSprites.Count;
            }
            else
            {
                nextLayer = nextLayer % ForeSprites.Count;
            }
            if (nextLayer < 0 || nextLayer >= ForeSprites.Count)
            {
                return;
            }
            mBottomSprite = ForeSprites[nextLayer];
            if (mBottomSprite == null)
            {
                return;
            }
            mBottomSprite.gameObject.SetActive(true);
            if (mBottomSprite.type == UIBasicSprite.Type.Advanced)
            {
                if (mBottomSprite.rightType == UIBasicSprite.AdvancedType.Invisible)
                {
                    mBottomSprite.rightType = UIBasicSprite.AdvancedType.Sliced;        
                }
            }
            
            mBottomSprite.enabled = true;
            mBottomSprite.drawRegion = new Vector4(0, 0, 1, 1);
            mBottomSprite.depth = 9;
        }
        SliderMove.foregroundWidget.depth = 10;
        SliderMove.foregroundWidget.gameObject.SetActive(true);
        SliderMove.foregroundWidget.enabled = true;
    }
    private List<KeyValuePair<int, float>> mChangePairs = new List<KeyValuePair<int, float>>(3);
    private void ChangeSlider2(int begin, int end)
    {
        var nowLayer = 0;
        var nowValue = 0.0f;
        if (begin == mMaxValue)
        {
            nowLayer = LayerCount - 1;
            nowValue = 1.0f;
        }
        else
        {
            var nowCountFloat = begin / PerLayerCount;
            nowLayer = (int)(nowCountFloat);
            nowValue = nowCountFloat - nowLayer;
        }


        var targetCountFloat = end / PerLayerCount;
        var targetLayer = (int)(targetCountFloat);
        var targetValue = targetCountFloat - targetLayer;

        float tl = 0;
        if (nowLayer == targetLayer)
        {
            mChangePairs.Add(new KeyValuePair<int, float>(nowLayer, targetValue));
            tl = nowValue - targetValue;

        }
        else if ((nowLayer - 1 - targetLayer) % 5 == 0)
        {
            if (isDel)
            {
                mChangePairs.Add(new KeyValuePair<int, float>(nowLayer, 0.0f));
                mChangePairs.Add(new KeyValuePair<int, float>(targetLayer, targetValue));
            }
            else
            {
                mChangePairs.Add(new KeyValuePair<int, float>(nowLayer, nowValue));
                mChangePairs.Add(new KeyValuePair<int, float>(targetLayer, 1.0f));
            }
            tl = nowValue + 1 - targetValue;
        }
        else
        {
            if (isDel)
            {
                mChangePairs.Add(new KeyValuePair<int, float>(nowLayer, 0.0f)); //now ,   now-1
                mChangePairs.Add(new KeyValuePair<int, float>(nowLayer - 1, 0.0f)); //now -1 , target
                mChangePairs.Add(new KeyValuePair<int, float>(targetLayer, targetValue));//target , target -1
            }
            else
            {
                mChangePairs.Add(new KeyValuePair<int, float>(nowLayer, nowValue)); //now ,   now-1
                mChangePairs.Add(new KeyValuePair<int, float>(nowLayer - 1, 1.0f)); //now -1 , target
                mChangePairs.Add(new KeyValuePair<int, float>(targetLayer, 1.0f));//target , target -1
            }
            tl = nowValue + 2 - targetValue;
        }
        var tt = tl * 0.5f + 0.2f;
        speed = tl / tt;
    }
    private bool isDel = false;
    private void ChangeSlider()
    {
        mChangePairs.Clear();
        if (mTargetValue < mNowValue)
        {
            isDel = true;
            ChangeSlider2(mNowValue, mTargetValue);
        }
        else
        {
            isDel = false;
            ChangeSlider2(mTargetValue, mNowValue);
        }
    }
    private float speed = 1.0f;
    private KeyValuePair<int, float> GetHead()
    {
        if (isDel)
        {
            return mChangePairs[0];
        }
        return mChangePairs[mChangePairs.Count - 1];
    }
    void Update()
    {
#if !UNITY_EDITOR
try
{
#endif

        if (mChangePairs.Count == 0)
        {
            return;
        }

        var head = GetHead();
        var dif = Time.deltaTime * speed;
        var newValue = SliderMove.value + (isDel ? -dif : dif);

        if (mChangePairs.Count == 1)
        {
            if (isDel)
            {
                if (newValue < head.Value)
                {
                    newValue = head.Value;
                    mChangePairs.RemoveAt(0);
                    mNowValue = mTargetValue;
                }
                else
                {
                    mNowValue = (int)((head.Key + newValue) * PerLayerCount);
                }
            }
            else
            {
                if (newValue > head.Value)
                {
                    newValue = head.Value;
                    mChangePairs.RemoveAt(0);
                    mNowValue = mTargetValue;
                }
                else
                {
                    mNowValue = (int)((head.Key + newValue) * PerLayerCount);
                }
            }
            SliderMove.value = newValue;
        }
        else if (isDel)
        {
            if (newValue <= 0.0f)
            {//换层
                newValue = 1.0f;
                mChangePairs.RemoveAt(0);
                var nextPair = mChangePairs[0];
                if (mChangePairs.Count == 2)
                {
                    var latPair = mChangePairs[1];
                    UpdateLayerCount(nextPair.Key, latPair.Key);
                }
                else if (mChangePairs.Count == 1)
                {
                    UpdateLayerCount(nextPair.Key);
                }
                SliderMove.value = newValue;
                mNowValue = (int)((nextPair.Key + newValue) * PerLayerCount);
            }
            else
            {
                SliderMove.value = newValue;
                mNowValue = (int)((head.Key + newValue) * PerLayerCount);
            }
        }
        else
        {
            if (newValue >= 1.0f)
            {//换层
                newValue = 0.0f;
                int c = mChangePairs.Count;
                --c;
                mChangePairs.RemoveAt(c);
                var nextPair = mChangePairs[c - 1];
                if (c == 2)
                {
                    var latPair = mChangePairs[0];
                    UpdateLayerCount(nextPair.Key, latPair.Key);
                }
                else if (c == 1)
                {
                    UpdateLayerCount(nextPair.Key);
                }
                SliderMove.value = newValue;
                mNowValue = (int)((nextPair.Key + newValue) * PerLayerCount);
            }
            else
            {
                SliderMove.value = newValue;
                mNowValue = (int)((head.Key + newValue) * PerLayerCount);
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
    public void UpdateChangeVlaueLable()
    {
        if (SliderMove.ScheduleLabel != null)
        {
            SliderMove.ScheduleLabel.text = string.Format("{0}/{1}", mTargetValue, mMaxValue);
        }

        if (LayerLable != null)
        {

            var nowLayer = 0;
            if (mTargetValue == mMaxValue)
            {
                nowLayer = LayerCount;
            }
            else
            {
                nowLayer = (int)(mTargetValue / PerLayerCount) + 1;    
            }

            if (nowLayer <= 1)
            {
                if (LayerLable.gameObject.activeSelf)
                {
                    LayerLable.gameObject.SetActive(false);
                }
            }
            else
            {
                if (LayerLable.gameObject.activeSelf == false)
                {
                    LayerLable.gameObject.SetActive(true);
                }
                LayerLable.text = string.Format("×{0}", nowLayer);    
            }
            
        }
    }
}
