#region using
using System;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IconBuyCell : MonoBehaviour
	{
        public ListItemLogic ItemLogic;
	
	    public void OnClick()
	    {
	        if (ItemLogic != null)
	        {
                var data = ItemLogic.Item as ItemBuyDataModel;
                if (data != null)
                {
                    if (data.ItemId != -1)
                    {
                        GameUtils.ShowItemIdTip(data.ItemId);
                    }
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