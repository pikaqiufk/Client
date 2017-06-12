#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ServerListCell : MonoBehaviour
	{
	    private void OnClick()
	    {
	        var listItem = GetComponent<ListItemLogic>();
	        var nIndex = listItem.Index;
	        var e = new Event_ServerListCellIndex(nIndex);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}