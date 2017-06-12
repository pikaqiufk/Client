using System;
#region using

using EventSystem;
using UnityEngine;

#endregion

namespace GameUI
{
	public class RewardCardCell : MonoBehaviour
	{
	    public void OnBountyGroupToggleClick()
	    {
	        var listItem = gameObject.GetComponent<ListItemLogic>();
	        if (listItem == null)
	        {
	            return;
	        }
	
	        var toggle = gameObject.GetComponent<UIToggle>();
	        if (toggle.value)
	        {
	            var ievent = new UIEvent_HandBookFrame_OnBountyGroupToggled(listItem.Index);
	            EventDispatcher.Instance.DispatchEvent(ievent);
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