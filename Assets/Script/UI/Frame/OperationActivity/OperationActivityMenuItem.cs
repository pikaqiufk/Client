using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class OperationActivityMenuItem : MonoBehaviour
	{
		public int Idx;
		public void OnClick()
		{
			EventDispatcher.Instance.DispatchEvent(new OperationActivityPageClickEvent(Idx));
		}
	}
}