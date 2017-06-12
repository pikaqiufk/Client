using System;
#region using

using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TitleCell : MonoBehaviour
	{
	    public string AllianceName = "";
	    public ListItemLogic listItemLogic;
	    private bool mRefleshLayOut = true;
	    public StackLayout mStackLayout;
	    public OverheadTitleFrame titleLogic;
	
	    public int TitleId
	    {
	        set
	        {
	            //Logger.Error("TitleId ------ {0}", value);
	            var tbNameTitle = Table.GetNameTitle(value);
	            var active = true;
	
	            titleLogic.SetTitle(tbNameTitle, AllianceName, ref active);
	            mRefleshLayOut = true;
	        }
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (mRefleshLayOut)
	        {
	            if (mStackLayout != null)
	            {
	                mStackLayout.ResetLayout();
	            }
	            mRefleshLayOut = false;
	        }
	    
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	}
	
	    public void OnClickPutOn()
	    {
	        if (listItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_TitleItemOption(0, listItemLogic.Index));
	        }
	    }
	
	    public void OnClickSelect()
	    {
	        if (listItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_OnClickRankBtn(listItemLogic.Index));
	        }
	    }
	
	    public void OnClickShowGet()
	    {
	        if (listItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_TitleItemOption(1, listItemLogic.Index));
	        }
	    }
	
	    // Use this for initialization
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        mRefleshLayOut = true;
	    
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
	
	    // Update is called once per frame
	    private void Update()
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
	}
}