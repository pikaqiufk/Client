#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class DailyActivityCell : MonoBehaviour
	{
	    //public ListItemLogic ItemLogic;
	
	    public void OnClickBtnGoto()
	    {
	        var itemLogic = GetComponent<ListItemLogic>();
	        if (itemLogic != null)
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_OnClickGotoActivity(itemLogic.Index));
	    }
	}
}