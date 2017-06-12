
#region using

using System;
using ClientDataModel;
using EventSystem;
using UnityEngine;
using DataTable;
using System.Collections.Generic;
#endregion

namespace GameUI
{
	[Serializable]
	public class EquipPosition
	{
		public int EquipId;
		public Vector3 OriginPosition;
	}
	public class FirstChargeLogic : MonoBehaviour
	{
	    public GameObject BaoXiangPos;
	    public BindDataRoot Binding;
	    public GameObject FlyObjParent;
	    public GameObject ToObj;
        public GameObject UIModel;
        public int ModelRenderQueue = 3004;
        public CreateFakeCharacter ModelRoot;
	    public int ModelNpcId = 20;
	    private GameObject TModel;
		public UIDragRotate Drag;
        public int UniqueResourceId;
        private static int sUniqueResourceId = 12345;

        private void GenerateFakeModel(int dataId)
        {
            if (-1 == dataId)
            {
                ModelRoot.DestroyFakeCharacter();
                return;
            }

            if (null != ModelRoot.Character && dataId == ModelRoot.Character.GetDataId())
            {
                return;
            }


            var tableNpc = Table.GetCharacterBase(dataId);
            if (null == tableNpc)
            {
                return;
            }

            ModelRoot.Create(dataId, null, character =>
            {
                character.SetScale(tableNpc.CameraMult / 10000.0f);
                var pos = character.gameObject.transform.localPosition + new Vector3(0, tableNpc.CameraHeight / 10000.0f, 0);
                character.gameObject.transform.localPosition = pos;
                character.PlayAnimation(OBJ.CHARACTER_ANI.STAND);
            });
        }

	    public void OnCloseBtnClick()
	    {
	        var iEvent = new FirstChargeCloseBtnClick_Event();
	        EventDispatcher.Instance.DispatchEvent(iEvent);
	    }
	
	    public void OnCtrBtnClick()
	    {
	        var iEvent = new FirstChargeBtnClick_Event();
	        EventDispatcher.Instance.DispatchEvent(iEvent);
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
            DestroyModel();
	        Binding.RemoveBinding();
            EventDispatcher.Instance.RemoveEventListener(FirstChargeGetItemSuccess_Event.EVENT_TYPE, PlayAnimation);
            EventDispatcher.Instance.RemoveEventListener(FirstChargeModelDisplay_Event.EVENT_TYPE, OnEvent_ShowModel);
            GenerateFakeModel(-1);
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
	        var controllerBase = UIManager.Instance.GetController(UIConfig.FirstChargeFrame);
	        if (controllerBase == null)
	        {
	            return;
	        }
	        Binding.SetBindDataSource(controllerBase.GetDataModel(""));
	
	        EventDispatcher.Instance.AddEventListener(FirstChargeGetItemSuccess_Event.EVENT_TYPE, PlayAnimation);
            EventDispatcher.Instance.AddEventListener(FirstChargeModelDisplay_Event.EVENT_TYPE, OnEvent_ShowModel);
            GenerateFakeModel(ModelNpcId);

#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
        public void OnEvent_ShowModel(IEvent ievent)
        {
            var e = ievent as FirstChargeModelDisplay_Event;
            if (e != null && !string.IsNullOrEmpty(e.PerfabPath))
                ShowModel(e.PerfabPath);
        }
	
	    public void PlayAnimation(IEvent ievent)
	    {
	        return;
	        var itemList = GetComponentsInChildren<ListItemLogic>();

	        for (var i = 0; i < itemList.Length; i++)
	        {
                var res = ResourceManager.PrepareResourceSync<GameObject>("UI/Icon/IconIdFrameCellFisrtCharge.prefab");
	            var prefab = Instantiate(res) as GameObject;
	            if (!prefab)
	            {
	                continue;
	            }
	            prefab.AddComponent<FirstChargeAni>();
	
	            prefab.transform.parent = FlyObjParent.transform;
	            prefab.transform.position = itemList[i].transform.position;
	            prefab.transform.localScale = Vector3.one;
	            if (itemList[i])
	            {
                    var bagItemDataModel = itemList[i].Item as ItemIdDataModel;
	                if (bagItemDataModel != null)
	                {
                        var itemData = new ItemIdDataModel
	                    {
	                        ItemId = bagItemDataModel.ItemId,
	                        Count = bagItemDataModel.Count
	                    };
	                    var bind = prefab.GetComponent<BindDataRoot>();
	                    bind.SetBindDataSource(itemData);
	                }
	            }
	            iTween.ScaleTo(prefab,
	                iTween.Hash("scale", new Vector3(1.5f, 1.5f, 1f), "time", 0.1f, "easetype", iTween.EaseType.linear));
	            iTween.MoveTo(prefab,
	                iTween.Hash("position", ToObj.transform.position, "time", 0.5, "islocal", true, "easetype",
	                    iTween.EaseType.linear, "oncomplete", "MoveToOver", "delay", 0.2f));
	            iTween.ScaleTo(prefab,
	                iTween.Hash("scale", ToObj.transform.position, "time", 0.5, "islocal", true, "easetype",
	                    iTween.EaseType.linear, "delay", 0.2f));
	        }
	
	        var BaoXiangAnimation =
	            Instantiate(ResourceManager.PrepareResourceSync<GameObject>("Model/UI/UI_BaoXiang.prefab")) as GameObject;
	        if (BaoXiangAnimation)
	        {
	            BaoXiangAnimation.transform.parent = BaoXiangPos.transform;
	            BaoXiangAnimation.transform.localPosition = Vector3.zero;
	            BaoXiangAnimation.transform.localScale = new Vector3(318f, 318f, 318f);
	            BaoXiangAnimation.AddComponent<WishingAnimation>();
	            BaoXiangAnimation.AddComponent<FirstChargeAni>();
	            var baoxiangAnim = BaoXiangAnimation.GetComponent<Animation>();
	            BaoXiangAnimation.SetActive(true);
	            baoxiangAnim.Play();
	            iTween.MoveTo(BaoXiangAnimation,
	                iTween.Hash("position", ToObj.transform.position, "time", 1.7, "islocal", true, "easetype",
	                    iTween.EaseType.linear, "oncomplete", "MoveToOver"));
	        }
	    }
        public static int GetNextUniqueResourceId()
        {
            return sUniqueResourceId++;
        }
        private void ShowModel(string perfabPath) //"res/firstcharget/a.prefab"
        {
            //var modelPath = Resource.GetModelPath(perfabPath);
            if (TModel != null)
            {   
                ComplexObjectPool.Release(TModel);
                TModel = null;
            }
            UniqueResourceId = GetNextUniqueResourceId();
            var resId = UniqueResourceId;
            ComplexObjectPool.NewObject(perfabPath, go =>
            {
                if (resId != UniqueResourceId)
                {
                    return;
                }
                TModel = go;
                go.transform.parent = UIModel.transform;
	            go.transform.localPosition = Vector3.zero;
                go.gameObject.SetLayerRecursive(LayerMask.NameToLayer(GAMELAYER.UI));
                go.gameObject.SetRenderQueue(ModelRenderQueue);

				Drag.Target = go.transform;
            });
        }

        public void Toggle1Click(IEvent ievent)
        {
            var iEvent = new FirstChargeToggleSuccess_Event(0);
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }

        public void Toggle2Click(IEvent ievent)
        {
            var iEvent = new FirstChargeToggleSuccess_Event(1);
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }

        public void Toggle3Click(IEvent ievent)
        {
            var iEvent = new FirstChargeToggleSuccess_Event(2);
            EventDispatcher.Instance.DispatchEvent(iEvent);
        }

        private void DestroyModel()
        {
            if (TModel != null)
            {
                ComplexObjectPool.Release(TModel);
                TModel = null;
            }
        }
        private void OnDestroy()
        {
#if !UNITY_EDITOR
	        try
	        {
#endif

            DestroyModel();
            UniqueResourceId = 0;
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
	
	        //todo
	    
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