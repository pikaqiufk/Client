using EventSystem;
using System;
#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class ServiceMonkeyGuideTrigger : MonoBehaviour
	{
	    public int GuideId = -1;
	    public int Level = 0;
	    public int MissionId = -1;
	    public eMissionState MissionState;
	    public int ServiceId = -1;
	
	    private void OnEnable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        if (-1 == GuideId)
	        {
	            return;
	        }
	
	        var mf = gameObject.GetComponent<TaskFrame>();
	        if (null != mf)
	        {
	            var check = false;
	            foreach (var id in mf.DataModel.ServiceId)
	            {
	                if (id == ServiceId && -1 != id)
	                {
	                    check = true;
	                    break;
	                }
	            }
	
	            if (!check)
	            {
	                return;
	            }
	        }
	
	        if (Level > 0)
	        {
	            if (PlayerDataManager.Instance.GetLevel() < Level)
	            {
	                return;
	            }
	        }
	        if (-1 != MissionId)
	        {
	            var data = MissionManager.Instance.GetMissionById(MissionId);
	            if (null == data)
	            {
	                return;
	            }
	            if (MissionState != MissionManager.Instance.GetMissionState(MissionId))
	            {
	                return;
	            }
	        }
	
	        GuideManager.Instance.StartGuide(GuideId);
            EventDispatcher.Instance.DispatchEvent(new Event_RefreshFuctionOnState());

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