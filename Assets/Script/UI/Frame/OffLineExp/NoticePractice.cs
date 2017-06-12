using GameUI;
using System;
#region using

using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class NoticePractice : MonoBehaviour
	{
	    public GameObject Btnlist;
	    public GameObject OfflineBtn;
	
	    private void ShowButton(IEvent ievent)
	    {
	        var OutLineOpenLevel = Table.GetClientConfig(104).ToInt();
	        if (GetLevel() > OutLineOpenLevel)
	        {
	            var btnlogic = Btnlist.GetComponent<MainScreenButtonFrame>();
	            if (!btnlogic.BtnList3.Contains(OfflineBtn))
	            {
	                btnlogic.BtnList3.Add(OfflineBtn);
	            }
	        }
	    }
	
	    // Use this for initialization
	    public int GetLevel()
	    {
	        return PlayerDataManager.Instance.GetLevel();
	    }
	
	    // Update is called once per frame
	    private void OnDestroy()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        EventDispatcher.Instance.RemoveEventListener(Event_LevelUp.EVENT_TYPE, ShowButton);
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var OutLineOpenLevel = Table.GetClientConfig(104).ToInt();
	        EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, ShowButton);
	        if (GetLevel() > OutLineOpenLevel)
	        {
	            var btnlogic = Btnlist.GetComponent<MainScreenButtonFrame>();
	            btnlogic.BtnList3.Add(OfflineBtn);
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