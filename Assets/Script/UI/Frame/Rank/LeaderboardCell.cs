#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class LeaderboardCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnClickCell()
	    {
	        var data = ItemLogic.Item as RankCellDataModel;
	        var e = new RankCellClick(data.Id - 1);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	}
}