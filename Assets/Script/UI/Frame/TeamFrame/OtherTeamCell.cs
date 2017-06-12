using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class OtherTeamCell : MonoBehaviour
	{
	    public UISprite FaceIcon;
	    public ListItemLogic ListItem;
	    // Use this for initialization
	    public BindDataRoot OtherPlayerDataRoot;
	    //邀请
	    public void OnClick_Apply()
	    {
	        var e = new TeamNearbyTeamClick(0, ListItem.Index);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }
	
	    //Tip
	    public void OnClick_Tip()
	    {
	        var parent = transform.root;
	        UIConfig.OperationList.Loction = parent.transform.worldToLocalMatrix*FaceIcon.worldCenter;
	        UIConfig.OperationList.Loction.x += 64;
	        UIConfig.OperationList.Loction.y += 100;
	        UIConfig.OperationList.Loction.z = 0;
	        var e = new TeamNearbyTeamClick(1, ListItem.Index);
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