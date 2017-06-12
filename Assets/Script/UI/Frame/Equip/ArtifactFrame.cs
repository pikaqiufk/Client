using ClientDataModel;
using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class ArtifactFrame : MonoBehaviour
    {
        public BindDataRoot Binding;
        public UICenterOnChild UICenter;
        public UIScrollView ModelScrollView;
        public UIScrollView ItemScrollView;
        public Camera PersCamera;
        private GameObject lastCenterObject = null;

        private void OnDisable()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif
            Binding.RemoveBinding();
            EventDispatcher.Instance.RemoveEventListener(EnableFrameEvent.EVENT_TYPE, OnEvent_EquipmentInfo);

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
            var controllerBase = UIManager.Instance.GetController(UIConfig.ArtifactUi);
            if (controllerBase == null)
            {
                return;
            }
            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
            EventDispatcher.Instance.AddEventListener(EnableFrameEvent.EVENT_TYPE, OnEvent_EquipmentInfo);


#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
        }

        private void OnEvent_EquipmentInfo(IEvent ievent)
        {
            var e = ievent as EnableFrameEvent;
            if (e != null && PersCamera != null)
            {
                if (e.Id < 0)
                    PersCamera.enabled = true;
                else
                    PersCamera.enabled = false;
            }
        }


        private void Start()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif

            UICenter.onCenter = OnSelectModel;

#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
        }

        private void LateUpdate()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif
            if (ModelScrollView.currentMomentum != Vector3.zero)
                ModelScrollView.MoveOtherScrollView(ItemScrollView);
            if (ItemScrollView.currentMomentum != Vector3.zero)
                ItemScrollView.MoveOtherScrollView(ModelScrollView);

#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
        }

        private void OnSelectModel(GameObject centerObject)
        {
            var obj = UICenter.centeredObject;
            if (null == obj || obj == lastCenterObject)
            {
                return;
            }

            lastCenterObject = obj;

            var item = obj.GetComponent<ListItemLogic>();
            EventDispatcher.Instance.DispatchEvent(new ArtifactSelectEvent(item));
        }

        public void OnClick_Buy()
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactOpEvent(0));
        }

        public void OnClick_ViewSkill()
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactOpEvent(1));
        }

        public void OnClick_GoAdvacne()
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactOpEvent(2));
        }

        public void OnClick_MyWeapon()
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactOpEvent(4));
        }

        public void OnClickBtnClose()
        {
            EventDispatcher.Instance.DispatchEvent(new Close_UI_Event(UIConfig.ArtifactUi));
        }

        public void OnClickHelp()
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactOpEvent(5));            
        }
        public void OnClickCloseHelp()
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactOpEvent(6));
        }
    }
}