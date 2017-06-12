#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class FuBenGroupCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickCell()
	    {
	        var e = new DungeonGroupCellClick(ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}