using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

public class ChatCityCellLogic : MonoBehaviour
{
    public ListItemLogic ItemList;

    public void OnClickCell()
    {
        var e = new ChatCityCellClick(ItemList.Index);
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