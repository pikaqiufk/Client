#region using

using System;
using System.ComponentModel;
using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TaskFrame : MonoBehaviour
	{
	    public BindDataRoot BindRoot;
	    public GameObject BtnRoot;
	    public float CharactersPerSecond = 13;
	    public MissionContentDataModel DataModel;
	    public float MaxChracterTime = 3.0f;
	    public CreateFakeCharacter ModelRoot;
	    // Use this for initialization
	
	    public TweenLabelTextOneByOne TweenText;
	
		private bool isPressed = false;
	    private void GenerateFakeModel(int dataId)
	    {
	        if (-1 == dataId)
	        {
	            ModelRoot.DestroyFakeCharacter();
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
	            character.SetScale(tableNpc.CameraMult/10000.0f);
	            var pos = character.gameObject.transform.localPosition + new Vector3(0, tableNpc.CameraHeight/10000.0f, 0);
	            character.gameObject.transform.localPosition = pos;
	            character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
	        });
	    }
	
	    private void OpenService(int idx)
	    {
	        var npcId = MissionManager.Instance.NpcId;
	        var tableCharacter = Table.GetCharacterBase(npcId);
	        if (null == tableCharacter)
	        {
	            return;
	        }
	        var tableNpc = Table.GetNpcBase(tableCharacter.ExdataId);
	        if (null == tableNpc)
	        {
	            return;
	        }
	        var serverId = tableNpc.Service[idx];
	        if (-1 == serverId)
	        {
	            return;
	        }

			ServiceManager.DoServeice(npcId,MissionManager.Instance.NpcObjId, serverId);
	    }
	
		
		public void OnClosePressDown()
		{
			isPressed = true;
			
		}
		public void OnClosePressUp()
		{
			if (isPressed)
			{
				EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionFrame));
			}
			isPressed = false;
		}
	    public void OnCloseClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionFrame));
	// 		if (null != mCharacter)
	// 		{
	// 			GameObject.Destroy(mCharacter);
	// 			mCharacter = null;
	// 		}
	    }
	
	    // Update is called once per frame
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        GenerateFakeModel(-1);
	        DataModel.PropertyChanged -= OnEvent_PropertyChanged;
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
	        GenerateFakeModel(-1);
	        DataModel.PropertyChanged -= OnEvent_PropertyChanged;
	
	        if (null != ModelRoot.Character)
	        {
	            ModelRoot.DestroyFakeCharacter();
	        }
	        BindRoot.RemoveBinding();
		    isPressed = false;
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
	
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MissionFrame);
	        DataModel = controllerBase.GetDataModel("") as MissionContentDataModel;
	        BindRoot.SetBindDataSource(DataModel);
	
	        DataModel.PropertyChanged += OnEvent_PropertyChanged;
	        GenerateFakeModel(DataModel.NpcDataId);
	        SetTaskText(DataModel.MissionDialogContent);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void OnOKClick()
	    {
	        MissionManager.Instance.OperateCurrentMission();
	    }
	
	    private void OnEvent_PropertyChanged(object o, PropertyChangedEventArgs args)
	    {
	        if (args.PropertyName == "NpcDataId")
	        {
	            GenerateFakeModel(DataModel.NpcDataId);
	        }
	        else if (args.PropertyName == "MissionDialogContent")
	        {
	            SetTaskText(DataModel.MissionDialogContent);
	        }
	    }
	
	    public void OnService1Click()
	    {
	        OpenService(0);
	
	        //EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionFrame));
	    }
	
	    public void OnService2Click()
	    {
	        OpenService(1);
	
	        //EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionFrame));
	    }
	
	    public void OnService3Click()
	    {
	        OpenService(2);
	
	        //EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.MissionFrame));
	    }
	
	    public void OnTextOneByOneOver()
	    {
	        BtnRoot.SetActive(true);
	    }

        public void OnForceEnd()
        {
            TweenText.ForceEnd();
        }
	
	    private void SetTaskText(string str)
	    {
	        if (string.IsNullOrEmpty(str))
	        {
	            return;
	        }
	
	        BtnRoot.SetActive(false);
	        var duration = Math.Min(str.Length*1.0f/CharactersPerSecond, MaxChracterTime);
	#if UNITY_EDITOR
	        //duration = 0.1f;
	#endif
	        TweenText.duration = duration;
	        TweenText.to = str;
	        TweenText.ResetToBeginning();
	        TweenText.PlayForward();
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