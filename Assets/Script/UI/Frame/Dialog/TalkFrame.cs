using System;
#region using

using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TalkFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    private DialogueDataModel DataModel;
	    private PropertyChangedEventHandler handler;
	    public CreateFakeCharacter ModelRoot;
	
	    private void CreateCopyObj(int dataId)
	    {
	        if (-2 == dataId)
	        {
	            ModelRoot.DestroyFakeCharacter();
	            return;
	        }
	
	        if (-1 == dataId)
	        {
	            var player = ObjManager.Instance.MyPlayer;
	            if (null != player)
	            {
	                if (null == GameLogic.Instance)
	                {
	                    ModelRoot.DestroyFakeCharacter();
	                    return;
	                }
					var tableId = ObjManager.Instance.MyPlayer.GetDataId();
					var tableChar = Table.GetCharacterBase(tableId);
					
	                ModelRoot.Create(player.GetDataId(), player.EquipList, character =>
	                {
						character.SetScale(tableChar.CameraMult / 10000.0f);
						var pos = character.gameObject.transform.localPosition + new Vector3(0, tableChar.CameraHeight / 10000.0f, 0);
						character.gameObject.transform.localPosition = pos;
		                character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
	                });
	            }
	
	            return;
	        }
	
	        if (null != ModelRoot.Character && dataId == ModelRoot.Character.GetDataId())
	        {
	            return;
	        }
	
	
	        var tableNpc = Table.GetCharacterBase(dataId);
	        if (null == tableNpc)
	        {
	            return;
	        }
	
	        ModelRoot.Create(dataId, null, character =>
	        {
	            character.SetScale(tableNpc.CameraMult / 10000.0f);
	            var pos = character.gameObject.transform.localPosition + new Vector3(0, tableNpc.CameraHeight / 10000.0f, 0);
	            character.gameObject.transform.localPosition = pos;
	            character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
	        });
	    }
	
	    public void OnClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Event_ShowNextDialogue());
	        ;
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        if (null != DataModel)
	        {
	            DataModel.PropertyChanged -= handler;
	        }
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
	
	        CreateCopyObj(-1);
	        DataModel.PropertyChanged -= OnEventPropertyChanged;
	
	        BindRoot.RemoveBinding();
	
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.DialogFrame);
	        DataModel = controllerBase.GetDataModel("") as DialogueDataModel;
	        BindRoot.SetBindDataSource(DataModel);
	
	
	        DataModel.PropertyChanged += OnEventPropertyChanged;
	        CreateCopyObj(DataModel.ModelId);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEventPropertyChanged(object o, PropertyChangedEventArgs args)
	    {
	        if (args.PropertyName == "ModelId")
	        {
	            CreateCopyObj(DataModel.ModelId);
	        }
	    }
	
	    // Use this for initialization
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