using System;
#region using

using ClientDataModel;
using DataTable;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ActivityListCell : MonoBehaviour
	{
	    public void OnGotoClick()
	    {
	        var dateModel = GetComponent<ListItemLogic>().Item as ActivityItemDataModel;
	        var table = Table.GetGift(dateModel.Id);
	        var uiId = table.Param[ActivityParamterIndx.UIId];
	        if (-1 == uiId)
	        {
	            return;
	        }
	        var ext = table.Param[5];
	        var hint = PlayerDataManager.Instance.CheckCondition(table.Param[6]);
	        if (hint == 0)
	        {
	            GameUtils.GotoUiTab(uiId, ext);
	        }
	        else
	        {
	            EventDispatcher.Instance.DispatchEvent(new ShowUIHintBoard(hint));
	        }
	    }
	
	    // Use this for initialization
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