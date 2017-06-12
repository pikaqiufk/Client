using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class CharacterInfoCellFrame : MonoBehaviour
	{
	    private ListItemLogic itemListLogic;
	
	    public void ItemClick()
	    {
	        if (itemListLogic != null)
	        {
	            var itemData = itemListLogic.Item as CharacterBaseInfoDataModel;
	            itemData.Selected = itemData.Selected == 0 ? 1 : 0;
	            //UIEvent_SailingPackItemUI e = new UIEvent_SailingPackItemUI();
	            //EventDispatcher.Instance.DispatchEvent(e);
	        }
	    }
	
	    public void NameClick()
	    {
	        if (itemListLogic != null)
	        {
	            var itemData = itemListLogic.Item as CharacterBaseInfoDataModel;
	            if (itemData.State == 1)
	            {
	                itemData.Selected = itemData.Selected == 0 ? 1 : 0;
	                return;
	            }
	            var worldPos = UICamera.currentCamera.ScreenToWorldPoint(UICamera.lastTouchPosition);
	            var localPos = transform.root.InverseTransformPoint(worldPos);
	            localPos.z = 0;
	            UIConfig.OperationList.Loction = localPos;
	
	            EventDispatcher.Instance.DispatchEvent(new UIEvent_UnionCharacterClick(itemData));
	        }
	    }
	
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
	}
}