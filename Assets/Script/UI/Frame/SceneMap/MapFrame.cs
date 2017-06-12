using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MapFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public Transform CurrentMap;
	    public UIToggle CurrentToggle;
	    public UITexture Texture;
	    public Transform WorldMap;
	    public UIToggle WorldToggle;
	
	    public void OnClickChangeLine()
	    {
	    }
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.SceneMapUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMapLoc()
	    {
	        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	        var localPos = Texture.transform.InverseTransformPoint(worldPos);
	        Logger.Info("Touch Postion {0}", localPos);
	        var e = new MapSceneClickLoction(localPos);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPlayers()
	    {
	    }
	
	    public void OnClickSharePostion()
	    {
	        var arg = new ChatMainArguments {Type = 1};
	        var e = new Show_UI_Event(UIConfig.ChatMainFrame, arg);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickWorldMap()
	    {
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        WorldToggle.value = false;
	        WorldToggle.mIsActive = false;
	        CurrentToggle.value = true;
	        CurrentToggle.mIsActive = true;
	        if (CurrentToggle.activeSprite)
	        {
	            CurrentToggle.activeSprite.alpha = 1.0f;
	        }
	        if (WorldToggle.activeSprite)
	        {
	            WorldToggle.activeSprite.alpha = 0.0f;
	        }
	        CurrentMap.gameObject.SetActive(true);
	        WorldMap.gameObject.SetActive(false);
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
	        var controllerBase = UIManager.Instance.GetController(UIConfig.SceneMapUI);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	        controllerBase = UIManager.Instance.GetController(UIConfig.TeamFrame);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnMapSceneMsgCancel()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MapSceneMsgOperation(1));
	    }
	
	    public void OnMapSceneMsgCheck()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MapSceneMsgOperation(2));
	    }
	
	    public void OnMapSceneMsgOK()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MapSceneMsgOperation(0));
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	// 	    var sceneItems = GetComponentsInChildren<SceneItemLogic>();
	// 	    int playerLevel = PlayerDataManager.Instance.PlayerDataModel.Attributes.Level;
	// 	    foreach (var sceneItemLogic in sceneItems)
	// 	    {
	// 	        var sceneTable = Table.GetScene(sceneItemLogic.sceneId);
	// 	        if (null == sceneTable)
	// 	        {
	// 	            Logger.Error("sceneId{0} do not find !!!!", sceneItemLogic.sceneId);
	// 	            continue;
	// 	        }
	// 	        var dataModel = new SceneItemDataModel();
	// 	        dataModel.SceneId = sceneItemLogic.sceneId;
	// 	        dataModel.TransferCast = sceneTable.ConsumeMoney;
	// 	        
	// 	        dataModel.Enable = (sceneTable.IsPublic == 1) && (sceneTable.LevelLimit <= playerLevel);
	//             if(sceneTable.IsPublic == 1)
	//                 dataModel.Text = string.Format(GameUtils.GetDictionaryText(533),sceneTable.LevelLimit);
	//             else
	//             {
	//                 dataModel.Text = string.Format(GameUtils.GetDictionaryText(532)); 
	//             }
	// 	        sceneItemLogic.dataModel = dataModel;
	// 	        EventDispatcher.Instance.DispatchEvent(new UIEvent_SceneMap_AddSceneItemDataModel(dataModel));
	// 	    }
	
	
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