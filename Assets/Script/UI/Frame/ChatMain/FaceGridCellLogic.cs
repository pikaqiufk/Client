using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

public class FaceGridCellLogic : MonoBehaviour
{
    public ListItemLogic ItemList;

    public void OnClickFaceIcon()
    {
        if (ItemList == null)
        {
            return;
        }
//        if (ItemList.Item == null)
//        {
//            return;
//        }
//        var item = ItemList.Item as ChatFaceDataModel;
//        if (item == null)
//        {
//            return;
//        }
////         ChatMainFaceAdd e = new ChatMainFaceAdd(0, item.FaceId);
////         EventDispatcher.Instance.DispatchEvent(e);
        var e = new FaceListClickIndex(ItemList.Index);
        EventDispatcher.Instance.DispatchEvent(e);
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