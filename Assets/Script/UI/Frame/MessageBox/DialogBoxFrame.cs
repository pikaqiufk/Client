using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class DialogBoxFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    public UIButton CancelBtn;
	    private Transform transCancelButton;
	    private int dlgType;
	    public UIButton OkBtn;
	    private Transform transOKButton;
	
	    public int BoxType
	    {
	        get { return dlgType; }
	        set
	        {
	            //if (mBoxType != value)
	            {
	                dlgType = value;
	                RefreshBtn();
	            }
	        }
	    }
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (OkBtn)
	        {
	            transOKButton = OkBtn.transform;
	        }
	        if (CancelBtn)
	        {
	            transCancelButton = CancelBtn.transform;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnClickCancel()
	    {
	        if (BoxType == 2)
	        {
	            return;
	        }
	        //this.gameObject.SetActive(false);
	        var e = new MessageBoxClick(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        var e1 = new Close_UI_Event(UIConfig.MessageBox);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }

        public void OnClickCancelByX()
        {
            //this.gameObject.SetActive(false);
            //var e = new MessageBoxClick(0);
            //EventDispatcher.Instance.DispatchEvent(e);

            var e1 = new Close_UI_Event(UIConfig.MessageBox);
            EventDispatcher.Instance.DispatchEvent(e1);
        }
	
	    public void OnClickOK()
	    {
	        //this.gameObject.SetActive(false);
	        var e = new MessageBoxClick(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	        var e1 = new Close_UI_Event(UIConfig.MessageBox);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        BindRoot.RemoveBinding();
            EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(-1));

	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MessageBox);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        BindRoot.SetBindDataSource(controllerBase.GetDataModel(""));
            EventDispatcher.Instance.DispatchEvent(new EnableFrameEvent(UIConfig.MessageBox.UiRecord.Id));

	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void RefreshBtn()
	    {
	        switch ((MessageBoxType) BoxType)
	        {
	            case MessageBoxType.OkCancel:
	            {
	                OkBtn.gameObject.SetActive(true);
	                CancelBtn.gameObject.SetActive(true);
	                transOKButton.localPosition = new Vector3(-105, -70, 0);
	                transCancelButton.localPosition = new Vector3(74, -70, 0);
	            }
	                break;
	            case MessageBoxType.Ok:
	            {
	                OkBtn.gameObject.SetActive(true);
	                CancelBtn.gameObject.SetActive(false);
	                transOKButton.localPosition = new Vector3(0, -70, 0);
	                transCancelButton.localPosition = new Vector3(74, -70, 0);
	            }
	                break;
	            case MessageBoxType.No:
	            {
	                OkBtn.gameObject.SetActive(false);
	                CancelBtn.gameObject.SetActive(false);
	            }
	                break;
	            case MessageBoxType.Cancel:
	            {
	                OkBtn.gameObject.SetActive(false);
	                CancelBtn.gameObject.SetActive(true);
	                transOKButton.localPosition = new Vector3(74, -70, 0);
	                transCancelButton.localPosition = new Vector3(0, -70, 0);
	            }
	                break;
	            default:
	                break;
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
	}
}