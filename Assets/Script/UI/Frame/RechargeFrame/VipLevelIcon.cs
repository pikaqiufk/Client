using System;
#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class VipLevelIcon : MonoBehaviour
	{
	    private UISprite[] mIcons;
	    private int mLineCount;
	
	    public int LineCount
	    {
	        set
	        {
	            mLineCount = value;
	            SetIconVisible();
	        }
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        mIcons = GetComponentsInChildren<UISprite>(true);
	        SetIconVisible();
	    
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	}
	
	    private void SetIconVisible()
	    {
	        if (mIcons == null)
	        {
	            return;
	        }
	        var c = mIcons.Length;
	        for (var i = 0; i < c; i++)
	        {
	            mIcons[i].gameObject.SetActive(i < mLineCount);
	        }
	    }
	}
}