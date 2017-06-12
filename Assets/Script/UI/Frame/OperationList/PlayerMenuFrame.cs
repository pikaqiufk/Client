using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;

#endregion
namespace GameUI
{
	public class PlayerMenuFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public UISprite Background;
	    public StackLayout LayerOut;
	    private bool isEnable;
	    private Transform menuTransform;
	    public BindDataRoot OperationListBindData;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        menuTransform = transform;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void LateUpdate()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (isEnable)
	        {
	            LayerOut.ResetLayout();
	            ResetPostion();
	            isEnable = false;
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    //加为仇人	AddEnemy
	    public void OnClick_AddEnemy()
	    {
	        var e = new UIEvent_OperationList_Button(3);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //加为好友	AddFriend
	    public void OnClick_AddFriend()
	    {
	        var e = new UIEvent_OperationList_Button(2);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //屏蔽	AddShield
	    public void OnClick_AddShield()
	    {
	        var e = new UIEvent_OperationList_Button(4);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //申请进队	ApplyTeam
	    public void OnClick_ApplyTeam()
	    {
	        var e = new UIEvent_OperationList_Button(9);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //查看属性	Attribute
	    public void OnClick_Attribute()
	    {
	        var e = new UIEvent_OperationList_Button(1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //解除仇人	DelEnemy 
	    public void OnClick_DelEnemy()
	    {
	        var e = new UIEvent_OperationList_Button(6);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //删除好友	DelFriend
	    public void OnClick_DelFriend()
	    {
	        var e = new UIEvent_OperationList_Button(5);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //取消屏蔽	DelShield 
	    public void OnClick_DelShield()
	    {
	        var e = new UIEvent_OperationList_Button(7);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //降低权限	
	    public void OnClick_DownAccess()
	    {
	        var e = new UIEvent_OperationList_Button(16);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //邀请组队	InviteTeam
	    public void OnClick_InviteTeam()
	    {
	        var e = new UIEvent_OperationList_Button(8);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //邀请入盟	
	    public void OnClick_JoinUnion()
	    {
	        var e = new UIEvent_OperationList_Button(13);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //请出队伍	KickTeam
	    public void OnClick_KickTeam()
	    {
	        var e = new UIEvent_OperationList_Button(11);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //请出战盟	
	    public void OnClick_KickUnion()
	    {
	        var e = new UIEvent_OperationList_Button(17);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //离开队伍	LeaveTeam
	    public void OnClick_LeaveTeam()
	    {
	        var e = new UIEvent_OperationList_Button(12);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //发起聊天	Speek
	    public void OnClick_Speek()
	    {
	        var e = new UIEvent_OperationList_Button(0);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //提升权限	
	    public void OnClick_UpAccess()
	    {
	        var e = new UIEvent_OperationList_Button(15);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //提升队长	UpLeader
	    public void OnClick_UpLeader()
	    {
	        var e = new UIEvent_OperationList_Button(10);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //提升领袖	
	    public void OnClick_UpToChief()
	    {
	        var e = new UIEvent_OperationList_Button(14);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickCloseBtn()
	    {
	        var e2 = new Close_UI_Event(UIConfig.OperationList);
	        EventDispatcher.Instance.DispatchEvent(e2);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        OperationListBindData.RemoveBinding();
	        isEnable = true;
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
	        var controllerBase = UIManager.Instance.GetController(UIConfig.OperationList);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        OperationListBindData.SetBindDataSource(controllerBase.GetDataModel(""));
	        isEnable = true;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void ResetPostion()
	    {
	        var loc = menuTransform.localPosition;
	        var bottom = loc.y - Background.height;
	
	        var right = loc.x + Background.width;
	
	
	        var root = menuTransform.root.GetComponent<UIRoot>();
	        if (root != null)
	        {
	            var y = loc.y;
	            if (bottom < 0 && Mathf.Abs(bottom) > root.activeHeight/2.0f)
	            {
	                y = root.activeHeight/2.0f - Background.height;
	                y = -y + 10.0f;
	            }
	
	            var s = (float) root.activeHeight/Screen.height;
	            var w = Screen.width*s/2.0f;
	
	            var x = loc.x;
	            if (right > w)
	            {
	                x = w - Background.width - 20;
	            }
	
	            menuTransform.localPosition = new Vector3(x, y, loc.z);
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
	
	    public void Listen<T>(T message)
	    {
	        isEnable = true;
	    }
	}
}