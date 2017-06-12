using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ServerListFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private Transform bkTrans;
	    private PropertyChangedEventHandler propChangeHandler;
	    public UISprite NoticeBackground;
	    public ServerListDataModel DataModel { get; set; }
	
	    public void OnBtnAnnouncementClick()
	    {
	        var e = new Event_ServerListButton(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	        PlatformHelper.Event("Announcement");
	    }
	
	    public void OnBtnCancelLineUp()
	    {
	        Game.Instance.ExitToLogin();
	    }
	
	    //最近登录点击
	    public void OnBtnLastServerClick()
	    {
	        var e = new Event_ServerListButton(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnBtnQuit()
	    {
	        if (GameUtils.IsOurChannel())
	        {
	            Game.Instance.ExitToLogin();
	        }
	        else
	        {
	            PlatformHelper.ChangeAccount();
	        }
	    }
	
	    private void OnDestroy()
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
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        PlatformHelper.CLoseWebView();
	        DataModel.PropertyChanged -= propChangeHandler;
	        Binding.RemoveBinding();
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.ServerListUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        DataModel = controllerBase.GetDataModel("") as ServerListDataModel;
	        Binding.SetBindDataSource(DataModel);
	        Binding.SetBindDataSource(PlayerDataManager.Instance.AccountDataModel);
	        propChangeHandler = OnPropertyChangeAnnounce;
	        DataModel.PropertyChanged += propChangeHandler;
	        LoginWindow.InvisibleLoginFrame();


            

#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	    UIManager.Instance.ShowMessage(MessageBoxType.Ok, ex.ToString());
	}
	#endif
	    }
	
	    //关闭界面
	    public void OnEnterGame()
	    {
	        var e = new Event_ServerListButton(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnPropertyChangeAnnounce(object sender, PropertyChangedEventArgs e)
	    {
	        if (e.PropertyName == "AnnouncementShow")
	        {
	            if (DataModel.AnnouncementShow)
	            {
	                var uiroot = FindObjectOfType<UIRoot>();
	                var ResoultionRadio = GameUtils.GetResolutionRadio();
	                var scale = uiroot.activeHeight/(float) Screen.height;
	                var sprWidget = bkTrans.GetComponent<UIWidget>();
	                var size = sprWidget.localSize;
                    var uicamera = uiroot.GetComponentInChildren<UICamera>();
                    if (null == uicamera)
                    {
                        Logger.Error("can't find uicamera!!! on show announce!");
                        return;
                    }
                    var pos = uicamera.camera.WorldToScreenPoint(bkTrans.position);
	                float width = 0;
	                float height = 0;
	                float x = 0;
	                float y = 0;
	#if UNITY_ANDROID
	                width = Mathf.CeilToInt(size.x/scale);
	                height = Mathf.CeilToInt(size.y/scale);
	                x = pos.x;
	                y = Screen.height - pos.y;
	#elif UNITY_IOS
	                     width = Mathf.CeilToInt(size.x/scale);
	                     height = Mathf.CeilToInt(size.y/scale);
	                     x = pos.x;
	                     y = Screen.height - pos.y;
	#else
	
	#endif
                    PlatformHelper.ShowWebView(UpdateHelper.AnnoucementURL, x / ResoultionRadio, y / ResoultionRadio,
                        width / ResoultionRadio, height / ResoultionRadio);
	            }
	            else
	            {
	                PlatformHelper.CLoseWebView();
	            }
	        }
	    }
	
	    public void OnServerListClick()
	    {
	        var e = new Event_ServerListButton(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }


	    void Awake()
	    {
#if !UNITY_EDITOR
try
{
#endif

            bkTrans = NoticeBackground.transform;
	    
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