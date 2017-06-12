using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class ActivityItemFrame : MonoBehaviour
	{
	    private ListItemLogic cellListLogic;
	
	    public void OnClickMenuCell()
	    {
	        if (cellListLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new ActivityCellClickedEvent(cellListLogic.Index));
	        }
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        cellListLogic = gameObject.GetComponent<ListItemLogic>();
	
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