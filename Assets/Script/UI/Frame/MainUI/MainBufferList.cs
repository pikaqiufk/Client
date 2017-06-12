using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MainBufferList : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public Animation BuffAnimation;
	    public BoxCollider CloseBuffListCollider;
	    private Transform objTrans;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        objTrans = gameObject.transform;
	
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
	
	        var con = UIManager.Instance.GetController(UIConfig.MainUI);
	        Binding.SetBindDataSource(con.GetDataModel("BuffList"));
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OpenBtnClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_BuffListBtn(0));
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (null != CloseBuffListCollider)
	        {
	            CloseBuffListCollider.center = Vector3.zero;
	
	            var root = objTrans.root.GetComponent<UIRoot>();
	            if (root != null)
	            {
	                var s = (float) root.activeHeight/Screen.height;
	                var size = new Vector3(Screen.width*s*1.5f, Screen.height*s*1.5f, 0f);
	                CloseBuffListCollider.size = size;
	                var pos = UICamera.mainCamera.WorldToScreenPoint(objTrans.position);
	                //偏移到屏幕中心
	                size = new Vector3(Screen.width, Screen.height, 0f);
	                CloseBuffListCollider.center = new Vector3(0, (size.y - pos.y) - size.y/2, 0f);
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
	}
}