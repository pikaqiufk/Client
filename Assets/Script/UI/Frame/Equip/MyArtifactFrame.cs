using ClientDataModel;
using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class MyArtifactFrame : MonoBehaviour
    {
        public BindDataRoot Binding;
        public WeaponModel EquipModel;
        public SelectToggleControl SelectLogic;
        public int ModelRenderQueue = 3004;
        private Quaternion initRotate;
        private RotateAround rotateAround;


        private void OnDisable()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif
            DestroyModel();
            Binding.RemoveBinding();
            EventDispatcher.Instance.RemoveEventListener(MyArtifactShowEquipEvent.EVENT_TYPE, OnEvent_ShowModel);

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
            var controllerBase = UIManager.Instance.GetController(UIConfig.MyArtifactUI);
            if (controllerBase == null)
            {
                return;
            }
            Binding.SetBindDataSource(controllerBase.GetDataModel(""));
            EventDispatcher.Instance.AddEventListener(MyArtifactShowEquipEvent.EVENT_TYPE, OnEvent_ShowModel);

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

        private void ShowWeaponModel(int equipId)
        {
            DestroyModel();

            EquipModel.CreateModel(equipId, go =>
            {
                if (null == go)
                {
                    return;
                }
                go.transform.parent = EquipModel.transform;
                go.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.UI));
                go.gameObject.SetRenderQueue(ModelRenderQueue);
                if (rotateAround == null)
                {
                    rotateAround = go.GetComponentInChildren<RotateAround>();
                    if (rotateAround != null)
                        initRotate = rotateAround.gameObject.transform.localRotation;
                }
                else
                {
                    rotateAround.transform.localRotation = initRotate; 
                }

            }, true);
        }

        private void DestroyModel()
        {
            if (EquipModel != null)
            {
                EquipModel.DestroyModel();
            }
        }

        public void OnEvent_ShowModel(IEvent ievent)
        {
            var e = ievent as MyArtifactShowEquipEvent;
            if (e != null)
            {
                ShowWeaponModel(e.EquipId);
            }
        }

        public void OnClick_Pack()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(6));
        }

        public void OnClick_SelfPack()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(7));
        }


        public void OnClick_RandSkill()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(3));
        }

        public void OnClick_Save()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(4));
        }

        public void OnClick_Cancel()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(5));
        }

        public void OnClick_Close()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(2));
        }
        public void OnClick_ChangeSkill()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(1));
        }
        public void OnClick_Back()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(0));
        }

        public void OnClick_BuyItem()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(8));            
        }
        public void OnClick_Advance()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(9));
        }
        public void OnRemoveEvoEquip(BagItemDataModel bagItem)
        {
            EventDispatcher.Instance.DispatchEvent(new ArtifactRemoveEvoEquipEvent(bagItem));
        }
        public void OnButtonEvoEquip()
        {
            EventDispatcher.Instance.DispatchEvent(new MyArtifactOpEvent(10));
        }
    }
}