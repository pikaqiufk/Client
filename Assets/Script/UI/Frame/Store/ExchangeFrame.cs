using System;
#region using

using System.ComponentModel;
using EventSystem;
using UnityEngine;
using ClientDataModel;
#endregion

namespace GameUI
{
    public class ExchangeFrame : MonoBehaviour
    {
        
        public BindDataRoot Binding;
        private IControllerBase controller;
        private ExchangeDataModel dataModel;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public void OnClickClose()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ExchangeUI));
        }

        private void OnEnable()
        {
            Binding.SetBindDataSource(PlayerDataManager.Instance.PlayerDataModel);
            controller = UIManager.Instance.GetController(UIConfig.ExchangeUI);
            if (controller == null)
            {
                return;
            }
            Binding.SetBindDataSource(controller.GetDataModel(""));

            EventDispatcher.Instance.DispatchEvent(new ExChangeInit_Event());
            
        }


        private void OnEventPropertyChanged(object o, PropertyChangedEventArgs args)
        {
         
        }
        public void OnClickExchange()
        {
            EventDispatcher.Instance.DispatchEvent(new ExChange_Event());
        }
        private void OnDisable()
        {
            Binding.RemoveBinding();
        }
    }
}