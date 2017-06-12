
#region using

using System;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
    public class FriendInfoCell : MonoBehaviour
    {
        public GameObject BtnMore;
        public ListItemLogic ListItem;

        public void OnClickCell()
        {
            var i = ListItem.Index;
            var e = new FriendContactCell(i);
            EventDispatcher.Instance.DispatchEvent(e);
        }

        public void OnClickInfo()
        {
            var info = ListItem.Item as ContactInfoDataModel;
            if (info == null)
            {
                return;
            }
            //         var parent = this.transform.parent;
            //         if (parent == null)
            //         {
            //             return;
            //         }
            //         parent = parent.parent;
            //         if (parent == null)
            //         {
            //             return;
            //         }
            var localPos = transform.root.InverseTransformPoint(transform.position);
            localPos.z = 0;
            UIConfig.OperationList.Loction = localPos;
            PlayerDataManager.Instance.ShowCharacterPopMenu(info.CharacterId, info.Name, 18, info.Level, info.Ladder,
                info.Type);
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
