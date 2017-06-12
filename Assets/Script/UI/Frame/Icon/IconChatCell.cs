using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IconChatCell : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	    public Animation SelectAnimation;
	
	    public void OnClickIconShare()
	    {
	        var data = ItemLogic.Item as BagItemDataModel;
	        var itemId = data.ItemId;
	        if (itemId != -1)
	        {
	            var e = new ChatItemListClick(data);
	            EventDispatcher.Instance.DispatchEvent(e);
	
	            if (SelectAnimation && SelectAnimation.clip)
	            {
	                SelectAnimation.Stop();
	                SelectAnimation.Play(SelectAnimation.clip.name);
	            }
	        }
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