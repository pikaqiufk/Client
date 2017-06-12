using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class MonsterCellFrame : MonoBehaviour
	{
	    private ListItemLogic itemListLogic;
	
	    public void OnClickMenuCell()
	    {
	        if (itemListLogic != null)
	        {
	            EventDispatcher.Instance.DispatchEvent(new BossCellClickedEvent(itemListLogic.Item as BtnState));
	        }
	    }
	
	    // Use this for initialization
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        itemListLogic = gameObject.GetComponent<ListItemLogic>();
	
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