#region using

using System;
using System.Collections.Generic;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	[Serializable]
	public class TaskRequirement
	{
	    public int MissionId = -1;
	    public eMissionState State = eMissionState.Acceptable;
	}
	
	public class TaskPoint : MonoBehaviour
	{
	    public List<TaskRequirement> Condition;
	    public GameObject EnableGameObject1;
        public GameObject EnableGameObject2;
        public GameObject EnableGameObject3;
        public GameObject NoticeObject;

	
	    private void CheckRequirement()
	    {
	        var conllor = UIManager.GetInstance().GetController(UIConfig.MissionTrackList);
	        if (conllor == null)
	        {
	            return;
	        }

	        var data = conllor.GetDataModel("") as MissionTrackListDataModel;
	        if (data == null)
	        {
	            return;
	        }

            if (EnableGameObject1 != null)
            {
                EnableGameObject1.SetActive(false);
                if (data.List[0].state == (int)eMissionState.Finished)
                {
                    EnableGameObject1.SetActive(true);
                }
            }
            if (EnableGameObject2 != null)
            {
                EnableGameObject2.SetActive(false);
                if (data.List[1].state == (int)eMissionState.Finished)
                {
                    EnableGameObject2.SetActive(true);
                }
            }
            if (EnableGameObject3 != null)
            {
                EnableGameObject3.SetActive(false);
                if (data.List[2].state == (int)eMissionState.Finished)
                {
                    EnableGameObject3.SetActive(data.List[2].state == (int)eMissionState.Finished);
                }
            }

            if (NoticeObject != null)
            {
                NoticeObject.SetActive(false);

                var mainMission = data.List[0];
                var enumerator = Condition.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var condition = enumerator.Current;
                    if (mainMission.MissionId == condition.MissionId
                        && condition.State == (eMissionState)mainMission.state)
                    {
                        NoticeObject.SetActive(true);
                        break;
                    }
                }
            }
	    }
	
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent);
	
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
	
	        CheckRequirement();
	
	#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
	#endif
	    }
	
	    private void OnEvent(IEvent e)
	    {
	        CheckRequirement();
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	        try
	        {
	#endif
	
	        EventDispatcher.Instance.AddEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent);
	        CheckRequirement();
	
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