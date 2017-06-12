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
	public class ActivityFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    private ActivityDataModel dataModel;
	    public UIDragRotate DragRorate;
	    public TimerLogic HomePageTimerLogic;
	    private IControllerBase controller;
	    public CreateFakeCharacter ModelRoot;
	    private Vector3 rootOriPosModel;
	    private Transform rootTransModel;
	    private bool removeBinding = true;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        rootTransModel = ModelRoot.transform;
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void FakeObjectCreate(int dataId)
	    {
	        ModelRoot.DestroyFakeCharacter();
	        if (-1 == dataId)
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
	            character.ObjTransform.localRotation = Quaternion.identity;
	            rootTransModel.localPosition = rootOriPosModel + new Vector3(0, tableNpc.CameraHeight/10000.0f, 0);
	            character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
	            DragRorate.Target = character.transform;
	        });
	    }
	
	    public void OnBtnDynamicActivityQueueClicked()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.DynamicActivity_Queue));
	    }
	
	    public void OnBtnGotoBoss()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_GotoMonster));
	    }

        public void OnBtnFlytoBoss()
        {
            EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_FlytoMonster));
        }
	
	    public void OnBtnQueueClicked()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_Queue));
	    }
	
	    public void OnBtnGetDoubleExp()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_GetDoubleExp));
	    }
	
	    public void OnButtonClose()
	    {
	        if (dataModel.PageId > 0 && dataModel.FirstPageId == 0)
	        {
	            dataModel.PageId = 0;
	        }
	        else
	        {
	            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ActivityUI));
	        }

            EventDispatcher.Instance.DispatchEvent(new ActivityClose_Event());
	    }
	
	    public void OnButtonEnterActivity()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.Activity_Enter));
	    }
	
	    public void OnButtonEnterDynamicActivity()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ButtonClicked(BtnType.DynamicActivity_Enter));
	    }
	
	    private void OnCloseUI(IEvent ievent)
	    {
	        var e = ievent as CloseUiBindRemove;
	        if (e.Config != UIConfig.ActivityUI)
	        {
	            return;
	        }
	        if (e.NeedRemove == 0)
	        {
	            removeBinding = false;
	        }
	        else
	        {
	            if (removeBinding == false)
	            {
	                DeleteBindEvent();
	            }
	            removeBinding = true;
	        }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        ModelRoot.DestroyFakeCharacter();
	        if (removeBinding == false)
	        {
	            DeleteBindEvent();
	        }
	        removeBinding = true;
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
	        ModelRoot.DestroyFakeCharacter();
	        FakeObjectCreate(-1);
	        if (removeBinding)
	        {
	            DeleteBindEvent();
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
	        if (removeBinding)
	        {
	            EventDispatcher.Instance.AddEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUI);
	
	            controller = UIManager.Instance.GetController(UIConfig.ActivityUI);
	            if (controller == null)
	            {
	                return;
	            }
	            dataModel = controller.GetDataModel("") as ActivityDataModel;
	
	            dataModel.PropertyChanged += OnEventPropertyChanged;
	
	            Binding.SetBindDataSource(controller.GetDataModel(""));
	        }
	        FakeObjectCreate(dataModel.BossDataId);
	        removeBinding = true;
	
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
	        if (args.PropertyName == "BossDataId")
	        {
	            FakeObjectCreate(dataModel.BossDataId);
	        }
	    }
	
	    public void OnTabBloodCastle()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(1));
	    }
	
	    public void OnTabDevilSquare()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(0));
	    }
	
	    public void OnTabGoldArmy()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(4));
	    }
	
	    public void OnTabMapCommander()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(3));
	    }
	
	    public void OnTabWorldBoss()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityTabSelectEvent(2));
	    }
	
	    private void DeleteBindEvent()
	    {
	        Binding.RemoveBinding();
	        dataModel.PropertyChanged -= OnEventPropertyChanged;
	        EventDispatcher.Instance.RemoveEventListener(CloseUiBindRemove.EVENT_TYPE, OnCloseUI);
	    }
        public void OnClickBuyTili()
        {
            EventDispatcher.Instance.DispatchEvent(new OnClickBuyTiliEvent(0));
        }

	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        rootOriPosModel = rootTransModel.localPosition;
	        HomePageTimerLogic.TargetTime = Game.Instance.ServerTime.AddYears(10);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void UpdateMainPageTimer()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UpdateActivityTimerEvent(UpdateActivityTimerType.MainPage));
	    }
	
	    public void UpdateTimer()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UpdateActivityTimerEvent(UpdateActivityTimerType.Single));
	    }
	}
}