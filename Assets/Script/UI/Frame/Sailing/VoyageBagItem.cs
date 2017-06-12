using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class VoyageBagItem : MonoBehaviour
	{
		// Use this for initialization
		private ListItemLogic listItemLogic;

		public void BagItemClick()
		{
			if (listItemLogic != null)
			{
				var itemData = listItemLogic.Item as MedalItemDataModel;

				if (itemData.BaseItemId != -1)
				{
					var e = new UIEvent_SailingPackItemUI();
					e.Index = listItemLogic.Index;
					e.BagId = (int)eBagType.MedalBag;
					e.PutOnOrOff = 1;
					EventDispatcher.Instance.DispatchEvent(e);
				}
			}
		}

		private void Start()
		{
#if !UNITY_EDITOR
	try
	{
#endif

			listItemLogic = gameObject.GetComponent<ListItemLogic>();


#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
#endif
		}

		public void TempBagItemClick()
		{
			if (listItemLogic != null)
			{
				var itemData = listItemLogic.Item as MedalItemDataModel;
				if (itemData.BaseItemId != -1)
				{
					var e = new UIEvent_SailingPickOne();
					e.Index = listItemLogic.Index;
					EventDispatcher.Instance.DispatchEvent(e);
				}
			}
		}
	}
}