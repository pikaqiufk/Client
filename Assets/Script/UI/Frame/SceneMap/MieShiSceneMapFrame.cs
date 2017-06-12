#region using
using System;
using EventSystem;
using UnityEngine;
using System.Collections.Generic;
using ClientDataModel;

#endregion

namespace GameUI
{
	public class MieShiSceneMapFrame : MonoBehaviour
	{
	    public BindDataRoot Binding;
	    public Transform CurrentMap;
	    public UITexture Texture;
        private readonly Dictionary<ulong, ListItemLogic> itemLogicDict = new Dictionary<ulong, ListItemLogic>();
        public Transform CharCursor;
	
	    public void OnClickClose()
	    {
	        var e = new Close_UI_Event(UIConfig.MieShiSceneMapUI);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickMapLoc()
	    {
	        var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	        var localPos = Texture.transform.InverseTransformPoint(worldPos);
	        Logger.Info("Touch Postion {0}", localPos);
            var e = new MieShiMapSceneClickLoction(localPos);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickPlayers()
	    {
	    }
	
	    public void OnClickSharePostion()
	    {
	        var arg = new ChatMainArguments {Type = 1};
	        var e = new Show_UI_Event(UIConfig.ChatMainFrame, arg);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    public void OnClickWorldMap()
	    {
	    }
	
	    private void OnDisable()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif

            EventDispatcher.Instance.RemoveEventListener(MieShiSceneMapRadar.EVENT_TYPE, OnShowRadar);
            EventDispatcher.Instance.RemoveEventListener(MieShiSceneMapRemoveRadar.EVENT_TYPE, OnRemoveShowRadar);
	
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
            if (!EventDispatcher.Instance.HasEventListener(MieShiSceneMapRadar.EVENT_TYPE, OnShowRadar))
            {
                EventDispatcher.Instance.AddEventListener(MieShiSceneMapRadar.EVENT_TYPE, OnShowRadar);
            }
            if (!EventDispatcher.Instance.HasEventListener(MieShiSceneMapRemoveRadar.EVENT_TYPE, OnRemoveShowRadar))
            {
                EventDispatcher.Instance.AddEventListener(MieShiSceneMapRemoveRadar.EVENT_TYPE, OnRemoveShowRadar);
            }


	        var controllerBase = UIManager.Instance.GetController(UIConfig.MieShiSceneMapUI);
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

        private void OnShowRadar(IEvent ievent)
        {
            var e = ievent as MieShiSceneMapRadar;
            if (e == null)
                return;

            var type = e.Type;
            var data = e.DataModel;
            if (type == 1)
            {
                if (e.Prefab != "")
                {
                    CreateCharRadar(data, e.Prefab);
                }
                else
                {
                    CreateCharRadar(data, "UI/MainUI/MieShiMapMonster.prefab");                    
                }
            }
            else
            {
                RemoveCharRadar(data.CharacterId);
            }
        }

	    private void OnRemoveShowRadar(IEvent ievent)
	    {
            var e = ievent as MieShiSceneMapRemoveRadar;
            if (e == null)
                return;

            RemoveCharRadar(e.id);
	    }

        private void CreateCharRadar(MapRadarDataModel data, string prefab)
        {
            var id = data.CharacterId;
            ComplexObjectPool.NewObject(prefab, o =>
            {
                if (data.CharType != 0)
                {
                    var charObj = ObjManager.Instance.FindCharacterById(id);
                    if (charObj == null || charObj.Dead)
                    {
                        ComplexObjectPool.Release(o);
                        return;
                    }
                }
                var oTransform = o.transform;
                oTransform.SetParentEX(CharCursor.transform);
                oTransform.localScale = Vector3.one;
                o.SetActive(true);
                var i = o.GetComponent<ListItemLogic>();
                i.Item = data;
                var r = o.GetComponent<BindDataRoot>();
                r.Source = data;

                itemLogicDict[data.CharacterId] = i;
            });
        }
        private void RemoveCharRadar(ulong id)
        {
            ListItemLogic obj;
            if (itemLogicDict.TryGetValue(id, out obj))
            {
                ComplexObjectPool.Release(obj.gameObject);
                itemLogicDict.Remove(id);
            }
        }
	
	    public void OnMapSceneMsgCancel()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MapSceneMsgOperation(1));
	    }
	
	    public void OnMapSceneMsgCheck()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MapSceneMsgOperation(2));
	    }
	
	    public void OnMapSceneMsgOK()
	    {
	        EventDispatcher.Instance.DispatchEvent(new MapSceneMsgOperation(0));
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
	}
}