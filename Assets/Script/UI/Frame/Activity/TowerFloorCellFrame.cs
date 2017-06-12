using EventSystem;
using System;
#region using

using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
    public class TowerFloorCellFrame : MonoBehaviour
    {
        public ListItemLogic ItemLogic;
        public void OnClick()
        {
            //var data = ItemLogic.Item as ClimbingTowerCellDataModel;
            //if (data != null)
            //{
            //    EventDispatcher.Instance.DispatchEvent(new TowerFloorClickEvent(data.nIndex));
            //}
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