using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class StrengthenCell : MonoBehaviour
	{
	    public ListItemLogic listItemLogic;
	
	    public void OnGotoClick()
	    {
	        if (listItemLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new StrongCellClickedEvent(listItemLogic.Index));
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
	
	    // Update is called once per frame
	    private void Update()
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