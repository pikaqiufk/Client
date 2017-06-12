using System;
using EventSystem;
using UnityEngine;

namespace GameUI
{
    public class WingChargeLogic : MonoBehaviour
    {
        public BindDataRoot Binding;

        private void Start()
        {
#if !UNITY_EDITOR
try
{
#endif

            var controllerBase = UIManager.Instance.GetController(UIConfig.WingChargeFrame);
            if (controllerBase == null) return;
            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
        
#if !UNITY_EDITOR
}
catch (Exception ex)
{
    Logger.Error(ex.ToString());
}
#endif
}

        // Update is called once per frame
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

        public void OnCloseBtnClick()
        {
            var iEvent = new WingChargeCloseBtnClick_Event();
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }
        public void OnItem1Click()
        {
            var iEvent = new WingChargeItemClick_Event(1);
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }

        public void OnItem2Click()
        {
            var iEvent = new WingChargeItemClick_Event(2);
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }

        public void OnBuyBtnClick()
        {
            var iEvent = new WingChargeBuyClick_Event();
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }
    }
}