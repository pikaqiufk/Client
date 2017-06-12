using System;
#region using

using ClientDataModel;
using EventSystem;
using UnityEngine;

#endregion

public class ActivityOnLineRewardCell : MonoBehaviour
{
	public void OnClaimClick()
	{
		var dataModel = GetComponent<ListItemLogic>().Item as OnLineRewardItemDataModel;
		if (null != dataModel)
		{
			EventDispatcher.Instance.DispatchEvent(new UIEvent_CliamReward(UIEvent_CliamReward.Type.OnLine, dataModel.Id));
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