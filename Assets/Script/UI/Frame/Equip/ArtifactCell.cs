#region using
using System;

using ClientDataModel;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using EventSystem;

#endregion

namespace GameUI
{
	public class ArtifactCell : MonoBehaviour
	{
        public WeaponModel EquipModel;
	    private ListItemLogic itemListLogic;
        public int ModelRenderQueue = 3004;
	    private RotateAround rotateAround;
        private Quaternion initRotate;
	    private GameObject model;
	
	    public void OnClickCell()
	    {
	        if (itemListLogic != null)
	        {
                //EventDispatcher.Instance.DispatchEvent(new BossCellClickedEvent(itemListLogic.Item as BtnState));
	        }
	    }

	    private void SetRotate()
	    {
            if (itemListLogic == null || itemListLogic.Item == null)
                return;

            var dm = itemListLogic.Item as EquipModelDataModel;
            if (dm != null && rotateAround != null)
            {
                if (dm.Select)
                {
                    rotateAround.enabled = true;
                }
                else
                {
                    rotateAround.enabled = false;
                    rotateAround.transform.localRotation = initRotate;
                }
            }	        
	    }

        private void OnPropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Select")
            {
                if (model == null)
                {
                    CreateModel();
                }

                SetRotate();
            }
        }

	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif

            itemListLogic = gameObject.GetComponent<ListItemLogic>();
	        if (itemListLogic != null)
	        {
                var t = itemListLogic.Item as EquipModelDataModel;
	            if (t != null)
	            {
	                t.PropertyChanged += OnPropertyChange;
	            }
	        };
            CreateModel();

#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }

	    private void CreateModel()
	    {
	        if (itemListLogic == null)
	            return;

	        if (model != null)
	            return;

            var t = itemListLogic.Item as EquipModelDataModel;
            if (t != null)
            {
                DestroyModel();
                EquipModel.CreateModel(t.EquipId, go =>
                {
                    if (null == go)
                    {
                        return;
                    }
                    model = go;
                    go.transform.parent = EquipModel.transform;
                    go.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.PerspectiveView));
                    go.gameObject.SetRenderQueue(ModelRenderQueue);

                    rotateAround = go.GetComponentInChildren<RotateAround>();
                    if (rotateAround != null)
                    {
                        initRotate = rotateAround.gameObject.transform.localRotation;
                    }
                    SetRotate();
                }, true);	 
            }       
	    }

	    private void DestroyModel()
	    {
            if (EquipModel != null)
            {
                EquipModel.DestroyModel();
            }
	        model = null;
	        if (rotateAround != null)
	        {
                rotateAround.enabled = true;
                rotateAround.transform.localRotation = initRotate;
            }
	    }

        private void OnEnable()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif
                CreateModel();

#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif
            DestroyModel();

#if !UNITY_EDITOR
	        }
	        catch (Exception ex)
	        {
	            Logger.Error(ex.ToString());
	        }
#endif
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
	try
	{
#endif
            if (itemListLogic != null)
            {
                var t = itemListLogic.Item as EquipModelDataModel;
                if (t != null)
                {
                    t.PropertyChanged -= OnPropertyChange;
                }
            };
            DestroyModel();


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