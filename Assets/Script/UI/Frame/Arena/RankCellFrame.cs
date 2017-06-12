#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class RankCellFrame : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickRankBtn()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_OnClickRankBtn(ItemLogic.Index));
	    }
	}
}