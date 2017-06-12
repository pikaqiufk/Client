#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ServerGroupCell : MonoBehaviour
	{
	    public void OnClick_Select()
	    {
	        var otherPlayer = GetComponent<ListItemLogic>();
	        var nIndex = otherPlayer.Index;
	        var e = new Event_ServerGroupListCellIndex(nIndex);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}