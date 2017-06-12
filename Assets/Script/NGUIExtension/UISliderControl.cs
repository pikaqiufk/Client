using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EventSystem;

[RequireComponent(typeof(UISlider))]
public class UISliderControl : MonoBehaviour
{
	[System.Serializable]
	public class KeyPoint
	{
		[Range(0,1)]
		public float Key;
		[Range(0,1)]
		public float Value;
	}

	public List<KeyPoint> KeyPointList;

	private UISlider mUISlider;

    private float mValue;
    public float Value
    {
        get { return mValue; }
        set
        {
            mValue = value;
            SetValue(mValue);
        }
    }

    void Awake()
	{
#if !UNITY_EDITOR
try
{
#endif

		mUISlider = GetComponent<UISlider>();

		if(null!=KeyPointList)
		{
			KeyPointList.Sort(Compare);

			if (KeyPointList[KeyPointList.Count-1].Key<1)
			{
				var temp = new KeyPoint 
				{ 
					Key = 1,
					Value = 1 
				};
				KeyPointList.Add(temp);
			}
		}

        if (Value != 0f)
            SetValue(Value);
	
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

	public void SetValue(float value)
	{
		if (null == mUISlider)
		{
			return;
		}

		value = Mathf.Clamp(value,0,1);

		if (null == KeyPointList || KeyPointList.Count<=0)
		{
			mUISlider.value = value;
			return;
		}

		for(int i=0; i<KeyPointList.Count; i++)
		{
			var temp = KeyPointList[i];
			if (value <= temp.Key)
			{
				var preKey = 0f;
				var preValue = 0f;
				if (i>0)
				{
					preKey = KeyPointList[i - 1].Key;
					preValue = KeyPointList[i - 1].Value;
				}

				float t = 1;
				if (temp.Key - preKey>0)
				{
					t = (value - preKey) / (temp.Key - preKey);
				}

				mUISlider.value = Mathf.Lerp(preValue, temp.Value, t);
				return;
			}
		}
	}

	static int Compare(KeyPoint a,KeyPoint b)
	{
		if(a.Key<b.Key)
		{
			return -1;
		}
		else if(a.Key>b.Key)
		{
			return 1;
		}
		return 0;
	}
}
