using System;
#region using

using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class TaskTrace : MonoBehaviour
	{
	    public UIButton HideBtn;
	    public UIWidget Hit;
	    public UIToggle InfotToggle;
	    private MissionTrackListDataModel taskListDM;
	    public UIButton ShowBtn;
	    public List<UIEventTrigger> TeamMembers;
	    public UIToggle TeamToggle;
	
	    public void OnClick_1()
	    {
	        if (-1 != taskListDM.List[0].MissionId)
	        {
	            MissionManager.Instance.GoToMissionPlace(taskListDM.List[0].MissionId);
	        }
	    }
	
	    public void OnClick_2()
	    {
	        if (-1 != taskListDM.List[1].MissionId)
	        {
	            MissionManager.Instance.GoToMissionPlace(taskListDM.List[1].MissionId);
	        }
	    }
	
	    public void OnClick_3()
	    {
	        if (taskListDM.List.Count < 3)
	        {
	            return;
	        }
	
	        /*
			if (-1 != mDataModel.List[2].MissionId)
			{
				MissionManager.Instance.GoToMissionPlace(mDataModel.List[2].MissionId);
			}
			*/
	
	        var missionId = taskListDM.List[2].MissionId;
	        if (missionId == MissionManager.Instance.CurrentDoingCircleMission)
	        {
	//如果当前任务是正在追踪的环任务那就GoTo
	            MissionManager.Instance.GoToMissionPlace(missionId);
	            return;
	        }
	
	        var arg = new MissionListArguments();
	        arg.Tab = 3;
	        arg.MissionId = missionId;
	        EventDispatcher.Instance.DispatchEvent(new Show_UI_Event(UIConfig.MissionList, arg));
	    }
	
	    public void OnClick_4()
	    {
	        if (-1 != taskListDM.List[3].MissionId)
	        {
	            MissionManager.Instance.GoToMissionPlace(taskListDM.List[3].MissionId);
	        }
	    }
	
	    public void OnClickMainTeamBtn()
	    {
            var e = new TeamOperateEvent(0, TeamToggle.value);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnEvent_ClickTeamMember(int index)
	    {
	        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	        var localPos = TeamMembers[index].transform.root.InverseTransformPoint(worldPos);
	        UIConfig.OperationList.Loction = localPos;
	
	        var e = new TeamMemberShowMenu(index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(TeamChangeEvent.EVENT_TYPE, OnEvent_TeamChange);
	
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
	
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void OnEvent_TeamChange(IEvent ievent)
	    {
	        var e = ievent as TeamChangeEvent;
	        if (e.Type == 10)
	        {
	            //InfotToggle.value = true;
	            //TeamToggle.value = false;
	
	            InfotToggle.Set(true);
	            TeamToggle.Set(false);
	        }
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	        var controllerBase = UIManager.Instance.GetController(UIConfig.MissionTrackList);
	        taskListDM = controllerBase.GetDataModel("") as MissionTrackListDataModel;
	
	        for (var i = 0; i < 5; i++)
	        {
	            var trigger = TeamMembers[i];
	            var j = i;
	            trigger.onClick.Add(new EventDelegate(() => { OnEvent_ClickTeamMember(j); }));
	        }
	
	        EventDispatcher.Instance.AddEventListener(TeamChangeEvent.EVENT_TYPE, OnEvent_TeamChange);
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void ToggleBtn()
	    {
	        if (ShowBtn.active)
	        {
	            ShowBtn.active = false;
	            HideBtn.active = true;
	            Hit.alpha = 1;
	        }
	        else
	        {
	            ShowBtn.active = true;
	            HideBtn.active = false;
	            Hit.alpha = 0;
	        }
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
	
	    public void UpdateInfoTimer()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MissionTrackUpdateTimerEvent());
	    }
	}
}