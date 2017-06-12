



using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class SkillTipFrame : MonoBehaviour
    {

        public BindDataRoot Binding;
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
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.SkillTipFrameUI));
        }

        // Use this for initialization
        private void Start()
        {
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

                var controllerBase = UIManager.Instance.GetController(UIConfig.SkillTipFrameUI);
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