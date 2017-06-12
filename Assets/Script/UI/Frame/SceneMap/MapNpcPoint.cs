#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class MapNpcPoint : MonoBehaviour
	{
	    public ListItemLogic ItemLogic;
	
	    public void OnItemClick()
	    {
	        var itemData = ItemLogic.Item as SceneNpcDataModel;
	
	        var e = new MapSceneClickCell(itemData);
	        EventDispatcher.Instance.DispatchEvent(e);
	
	
	        var e1 = new Close_UI_Event(UIConfig.SceneMapUI);
	        EventDispatcher.Instance.DispatchEvent(e1);
	    }
	}
}