#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class GroundMenuCell : MonoBehaviour
	{
	    public Transform BtnMenu;
	    public ListItemLogic ItemLogic;
	
	    public void OnClickMenu()
	    {
	        var e = new FarmMenuClickEvent(ItemLogic.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}