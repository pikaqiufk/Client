#region using
using System;
using ClientDataModel;
using UnityEngine;

#endregion

namespace GameUI
{
	public class IconBuyCell2 : MonoBehaviour
	{
        public BindDataRoot BindRoot;
        private ItemBuyDataModel itemDataModel = new ItemBuyDataModel();

        public ItemBuyDataModel ItemData
        {
            get { return itemDataModel; }
            set
            {
                itemDataModel = value;
                if (BindRoot != null)
                {
                    BindRoot.SetBindDataSource(itemDataModel);
                }
            }
        }

        [TableBinding("ItemBase")]
        public int ItemId
        {
            get { return itemDataModel.ItemId; }
            set
            {
                itemDataModel.ItemId = value;
                if (BindRoot != null)
                {
                    BindRoot.SetBindDataSource(itemDataModel);
                }
            }
        }
	
	    public void OnClick()
	    {
            if (ItemId != -1)
            {
                GameUtils.ShowItemIdTip(ItemId);
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