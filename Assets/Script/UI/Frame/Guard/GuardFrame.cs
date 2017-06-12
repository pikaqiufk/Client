
#region using

using System;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class GuardFrame : MonoBehaviour
    {
        public BindDataRoot Binding;

        public void OnClickReBorn()
        {
            EventDispatcher.Instance.DispatchEvent(new GuardUIOperation(0, 0));
        }

        public void OnClose()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.GuardUI));
        }

        private void Start()
        {
#if !UNITY_EDITOR
            try
            {
#endif

                //todo
                var controllerBase = UIManager.Instance.GetController(UIConfig.GuardUI);
                if (controllerBase == null)
                {
                    return;
                }
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
    }
}
