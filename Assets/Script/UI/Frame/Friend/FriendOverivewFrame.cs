
#region using

using System;
using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class FriendOverivewFrame : MonoBehaviour
	{
	    public UISprite FaceIcon;
	    public ListItemLogic ItemLogic;
	
	    public void OnClickFriendInfo()
	    {
	        var data = ItemLogic.Item as FriendInfoDataModel;
	
	        if (data == null)
	        {
	            return;
	        }
	
	        var parent = UIManager.GetInstance().GetUIRoot(UIType.TYPE_TIP);
	        UIConfig.OperationList.Loction = parent.transform.worldToLocalMatrix*FaceIcon.worldCenter;
	        UIConfig.OperationList.Loction.x += 64;
	        UIConfig.OperationList.Loction.y += 100;
	        UIConfig.OperationList.Loction.z = 0;
	        var e = new FriendClickShowInfo(data);
	        EventDispatcher.Instance.DispatchEvent(e);
	    }

        public void OnClickAddFriend()
        {
            var data = ItemLogic.Item as FriendInfoDataModel;

            if (data == null)
            {
                return;
            }

            var parent = UIManager.GetInstance().GetUIRoot(UIType.TYPE_TIP);
            UIConfig.OperationList.Loction = parent.transform.worldToLocalMatrix * FaceIcon.worldCenter;
            UIConfig.OperationList.Loction.x += 64;
            UIConfig.OperationList.Loction.y += 100;
            UIConfig.OperationList.Loction.z = 0;

            var e = new FriendContactClickAddFriend(data);
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