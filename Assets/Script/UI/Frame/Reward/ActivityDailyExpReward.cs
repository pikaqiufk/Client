using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class ActivityDailyExpReward : MonoBehaviour
	{
		public ListItemLogic listItemLogic;

		public void OnDiamondClick()
		{
			if (listItemLogic != null)
			{
				EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityCompensateItem(3, listItemLogic.Index));
			}
		}

		public void OnGoodClick()
		{
			if (listItemLogic != null)
			{
				EventDispatcher.Instance.DispatchEvent(new UIEvent_ActivityCompensateItem(2, listItemLogic.Index));
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