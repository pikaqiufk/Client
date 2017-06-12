using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IllustrationFrame : MonoBehaviour
	{
	    public GameObject AnimationBlocker;
	    public BindDataRoot Binding;
	    private Vector3 lastPos;
	    private float offset;
	    private bool isRemoveBind = true;
	    public UIScrollViewSimple ScrollView;
	
	    public void MoveToOffset(IEvent e)
	    {
	        ScrollView.MoveToOffset2(lastPos, offset);
	    }
	
	    public void OnBookInfoClose()
	    {
	        var e = new UIEvent_HandBookFrame_OnBookClick(null);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnButtonClose()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.HandBook, true));
	    }
	
	    private void OnUiCloseRemove(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.HandBook)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            isRemoveBind = false;
	        }
	        else
	        {
	            if (isRemoveBind == false)
	            {
	                DeleteBindListener();
	            }
	            isRemoveBind = true;
	        }
	    }
	
	    public void OnComposeButtonClick()
	    {
	        var e = new UIEvent_HandBookFrame_ComposeBookPieceFromBookInfo();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnComposeCardClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_HandBookFrame_ComposeBookCardFromBookInfo());
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (isRemoveBind == false)
	        {
	            DeleteBindListener();
	        }
	        isRemoveBind = true;
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
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_HandBookFrame_ShowAnimationBlocker.EVENT_TYPE, SetAnimBlocker);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_HandBookFrame_SetScrollViewLastPostion.EVENT_TYPE, SetOffset);
	        EventDispatcher.Instance.RemoveEventListener(UIEvent_HandBookFrame_RestScrollViewPostion.EVENT_TYPE, MoveToOffset);
	
	        if (isRemoveBind)
	        {
	            DeleteBindListener();
	        }
	
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
	        lastPos = ScrollView.transform.position;
	        offset = ScrollView.oldoffset;
	
	
	        AnimationBlocker.SetActive(false);
	        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_ShowAnimationBlocker.EVENT_TYPE, SetAnimBlocker);
	
	        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_SetScrollViewLastPostion.EVENT_TYPE, SetOffset);
	        EventDispatcher.Instance.AddEventListener(UIEvent_HandBookFrame_RestScrollViewPostion.EVENT_TYPE, MoveToOffset);
	        if (isRemoveBind)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnUiCloseRemove);
	
	            var controllerBase = UIManager.Instance.GetController(UIConfig.HandBook);
	            if (controllerBase == null)
	            {
	                return;
	            }
	            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	            Binding.SetBindDataSource(PlayerDataManager.Instance.NoticeData);
	        }
	        isRemoveBind = true;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnGetBookClick()
	    {
	        var e = new UIEvent_HandBookFrame_OnGetBookClick();
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void DeleteBindListener()
	    {
	        Binding.RemoveBinding();
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnUiCloseRemove);
	    }
	
	    public void SetOffset(IEvent e)
	    {
	        var ievent = e as UIEvent_HandBookFrame_SetScrollViewLastPostion;
	        if (ievent != null)
	        {
	            offset = ievent.offset;
	            lastPos = ievent.postion;
	        }
	    }
	
	    private void SetAnimBlocker(IEvent ievent)
	    {
	        var e = ievent as UIEvent_HandBookFrame_ShowAnimationBlocker;
	        AnimationBlocker.SetActive(e.bShow);
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
	}
}