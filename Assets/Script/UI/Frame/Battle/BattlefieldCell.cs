using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
    public class BattlefieldCell : MonoBehaviour
	{
	    public ListItemLogic ListItem;
	
	    public void OnClickBattleCell()
	    {
	        var e = new BattleCellClick(ListItem.Index);
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
}