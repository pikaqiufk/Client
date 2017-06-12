



using System;
#region using

using EventSystem;
using UnityEngine;
using ClientDataModel;
#endregion

namespace GameUI
{
    public class FuctionTipFrame : MonoBehaviour
    {

        public BindDataRoot Binding;
        private MissionTrackListDataModel taskListDM;

        private bool deleteBind = true;
        private void OnDestroy()
        {
            if (deleteBind == false)
            {
                RemoveBindEvent();
            }
            deleteBind = true;
        }

        public void OnCloseTipClick()
        {
           
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.FuctionTipFrame));
            if (taskListDM.List[0].MissionId!=null)
            {
                if (-1 != taskListDM.List[0].MissionId)
                {
                    MissionManager.Instance.GoToMissionPlace(taskListDM.List[0].MissionId);
                } 
            }
        }

        // Use this for initialization
        private void Start()
        {
            var controllerBase = UIManager.Instance.GetController(UIConfig.MissionTrackList);
            taskListDM = controllerBase.GetDataModel("") as MissionTrackListDataModel;
        }

        // Update is called once per frame
        private void Update()
        {
        }
        private void RemoveBindEvent()
        {
            Binding.RemoveBinding();
        }
        private void OnEnable()
        {
            if (deleteBind)
            {

                var controllerBase = UIManager.Instance.GetController(UIConfig.FuctionTipFrame);
                if (controllerBase == null)
                {
                    return;
                }
                Binding.SetBindDataSource(controllerBase.GetDataModel(""));
            }
            deleteBind = true;

        }

        private void OnDisable()
        {
            var e = new UIEvent_SkillFrame_OnDisable();
            EventDispatcher.Instance.DispatchEvent(e);
            if (deleteBind)
            {
                RemoveBindEvent();
            }
        }
    }
}