using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MapObjItem : MonoBehaviour
	{
	    public BindDataRoot binding;
	    public SceneItemDataModel dataModel;
	    public int sceneId;
	
	    public void OnBtnTransferClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SceneMap_BtnTranfer(sceneId));
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        binding.RemoveBinding();
	
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
	        if (dataModel != null)
	        {
	            binding.SetBindDataSource(dataModel);
	        }
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (dataModel != null)
	        {
	            binding.SetBindDataSource(dataModel);
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