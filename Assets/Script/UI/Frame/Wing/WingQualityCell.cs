#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class WingQualityCell : MonoBehaviour
	{
	    public ListItemLogic ListItem;
	
	    public void OnClickCell()
	    {
	        var e = new WingQuailtyCellClick(ListItem.Item as WingQualityData);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}