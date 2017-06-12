using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
    public class TowerUIFrame : MonoBehaviour
    {
        public BindDataRoot Binding;
        private IControllerBase controller;

        private void OnEnable()
        {
#if !UNITY_EDITOR
            try
            {
#endif
            controller = UIManager.Instance.GetController(UIConfig.ClimbingTowerUI);
                if (controller == null)
                {
                    return;
                }
                Binding.SetBindDataSource(controller.GetDataModel(""));

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


#if !UNITY_EDITOR
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
#endif
        }

        public void OnClickClose()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ClimbingTowerUI));
        }

        public void OnClickStart()
        {
            EventDispatcher.Instance.DispatchEvent(new TowerBtnClickEvent(0)); 
        }

        public void OnClickSweep()
        {
            EventDispatcher.Instance.DispatchEvent(new TowerBtnClickEvent(1)); 
        }
    }
}