using System;
#region using

using EventSystem;
using UnityEngine;


#endregion

namespace GameUI
{
	public class BtnValue : MonoBehaviour
	{
	    public int Value;
	
	    public void NumberClick()
	    {
	        EventDispatcher.Instance.DispatchEvent(new UIEvent_NumberPad_Click(Value));
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