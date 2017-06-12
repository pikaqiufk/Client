using GameUI;
using System;
#region using

using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    internal class WorldMapFrame : MonoBehaviour
    {
        private void Start()
        {
#if !UNITY_EDITOR
            try
            {
#endif
            var sceneItems = GetComponentsInChildren<MapObjItem>();
            var playerLevel = PlayerDataManager.Instance.GetLevel();
            {
                var __array1 = sceneItems;
                var __arrayLength1 = __array1.Length;
                for (var __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var sceneItemLogic = __array1[__i1];
                    {
                        var sceneTable = Table.GetScene(sceneItemLogic.sceneId);
                        if (null == sceneTable)
                        {
                            Logger.Error("sceneId{0} do not find !!!!", sceneItemLogic.sceneId);
                            continue;
                        }
                        var dataModel = new SceneItemDataModel();
                        dataModel.SceneId = sceneItemLogic.sceneId;
                        dataModel.TransferCast = sceneTable.ConsumeMoney;

                        dataModel.Enable = (sceneTable.IsPublic == 1) && (sceneTable.LevelLimit <= playerLevel);
                        //if (sceneTable.IsPublic == 1)
                        //    dataModel.Text = string.Format(GameUtils.GetDictionaryText(533), sceneTable.LevelLimit);
                        //else
                        //{
                        //    dataModel.Text = string.Format(GameUtils.GetDictionaryText(532));
                        //}
                        sceneItemLogic.dataModel = dataModel;
                        EventDispatcher.Instance.DispatchEvent(new UIEvent_SceneMap_AddSceneItemDataModel(dataModel));
                    }
                }
            }


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