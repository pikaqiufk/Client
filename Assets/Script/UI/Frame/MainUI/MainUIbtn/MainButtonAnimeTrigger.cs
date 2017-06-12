
using System;
#region using

using System.Collections.Generic;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	namespace Assets.Script.UI.MainUIbtn
	{
	    public class MainButtonAnimeTrigger : MonoBehaviour
	    {
	        public List<UIButton> BtnList;
	        private readonly List<GameObject> buildList = new List<GameObject>();
	        public int Level = 0;
	        [HideInInspector] public List<GameObject> MainBtn = new List<GameObject>();
	        private MainScreenButtonFrame mainScreenButton;
	        public int MissionId = -1;
	        public eMissionState MissionState;
	        private readonly List<GameObject> otherList = new List<GameObject>();
	        private readonly List<GameObject> contactList = new List<GameObject>();
	        private readonly List<GameObject> trainList = new List<GameObject>();
	
	        public int FindBtnByname(string Data)
	        {
	            for (var i = 0; i < trainList.Count; i++)
	            {
	                if (trainList[i] != null)
	                {
	                    if (trainList[i].name == Data)
	                    {
	                        if (i > mainScreenButton.BtnList1.Count)
	                        {
	                            return mainScreenButton.BtnList1.Count;
	                        }
	                        return i;
	                    }
	                }
	            }
	            for (var i = 0; i < buildList.Count; i++)
	            {
	                if (buildList[i] != null)
	                {
	                    if (buildList[i].name == Data)
	                    {
	                        if (i > mainScreenButton.BtnList2.Count)
	                        {
	                            return mainScreenButton.BtnList2.Count;
	                        }
	                        return i;
	                    }
	                }
	            }
	            for (var i = 0; i < contactList.Count; i++)
	            {
	                if (contactList[i] != null)
	                {
	                    if (contactList[i].name == Data)
	                    {
	                        if (i > mainScreenButton.BtnList3.Count)
	                        {
	                            return mainScreenButton.BtnList3.Count;
	                        }
	                        return i;
	                    }
	                }
	            }
	            for (var i = 0; i < otherList.Count; i++)
	            {
	                if (otherList[i] != null)
	                {
	                    if (otherList[i].name == Data)
	                    {
	                        if (i > mainScreenButton.BtnList4.Count)
	                        {
	                            return mainScreenButton.BtnList4.Count;
	                        }
	                        return i;
	                    }
	                }
	            }
	            return -1;
	        }
	
	        private void OnEvent_Levelup(IEvent ievent)
	        {
	            if (Level > 0)
	            {
	                if (PlayerDataManager.Instance.GetLevel() < Level)
	                {
	                }
	            }
	        }
	
	        private void OnEvent_UpdateTask(IEvent ievent)
	        {
	            var data = MissionManager.Instance.GetMissionById(MissionId);
	            if (null == data)
	            {
	                return;
	            }
	            if (MissionState != MissionManager.Instance.GetMissionState(MissionId))
	            {
	            }
	        }
	
	        private void OnDestroy()
	        {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	            EventDispatcher.Instance.RemoveEventListener(Event_LevelUp.EVENT_TYPE, OnEvent_Levelup);
	            EventDispatcher.Instance.RemoveEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent_UpdateTask);
	
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
		        return;

	            mainScreenButton = gameObject.GetComponent<MainScreenButtonFrame>();
	            {
	                // foreach(var VARIABLE in mainUiButtonLogic.BtnList1)
	                var __enumerator1 = (mainScreenButton.BtnList1).GetEnumerator();
	                while (__enumerator1.MoveNext())
	                {
	                    var VARIABLE = __enumerator1.Current;
	                    {
	                        trainList.Add(VARIABLE);
	                    }
	                }
	            }
	            {
	                // foreach(var VARIABLE in mainUiButtonLogic.BtnList2)
	                var __enumerator2 = (mainScreenButton.BtnList2).GetEnumerator();
	                while (__enumerator2.MoveNext())
	                {
	                    var VARIABLE = __enumerator2.Current;
	                    {
	                        buildList.Add(VARIABLE);
	                    }
	                }
	            }
	            {
	                // foreach(var VARIABLE in mainUiButtonLogic.BtnList3)
	                var __enumerator3 = (mainScreenButton.BtnList3).GetEnumerator();
	                while (__enumerator3.MoveNext())
	                {
	                    var VARIABLE = __enumerator3.Current;
	                    {
	                        contactList.Add(VARIABLE);
	                    }
	                }
	            }
	            {
	                // foreach(var VARIABLE in mainUiButtonLogic.BtnList4)
	                var __enumerator4 = (mainScreenButton.BtnList4).GetEnumerator();
	                while (__enumerator4.MoveNext())
	                {
	                    var VARIABLE = __enumerator4.Current;
	                    {
	                        otherList.Add(VARIABLE);
	                    }
	                }
	            }
	            {
	                // foreach(var VARIABLE in mainUiButtonLogic.BtnList)
	                var __enumerator5 = (mainScreenButton.BtnList).GetEnumerator();
	                while (__enumerator5.MoveNext())
	                {
	                    var VARIABLE = __enumerator5.Current;
	                    {
	                        MainBtn.Add(VARIABLE.gameObject);
	                    }
	                }
	            }
	            EventDispatcher.Instance.AddEventListener(Event_LevelUp.EVENT_TYPE, OnEvent_Levelup);
	            EventDispatcher.Instance.AddEventListener(Event_UpdateMissionData.EVENT_TYPE, OnEvent_UpdateTask);
	
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
}