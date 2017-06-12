using System;
#region using

using EventSystem;
using SignalChain;
using UnityEngine;


#endregion

namespace GameUI
{
	public class JJCStatueCellFrame : MonoBehaviour, IChainRoot, IChainListener
	{
	    public UISprite Background;
	    public StackLayout Layout;
	    public ListItemLogic ListItem;
	
	    public void OnPetListClick()
	    {
	        var e = new ArenaSatueCellClick();
	        EventDispatcher.Instance.DispatchEvent(e);
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
	
	    public void Listen<T>(T message)
	    {
	        if (message is string && (message as string) == "ActiveChanged")
	        {
	            Layout.ResetLayout();
	            var transform = Layout.transform;
	            var pos = transform.localPosition;
	            var h = Layout.height - Background.height/2;
	            transform.localPosition = new Vector3(pos.x, h, pos.z);
	        }
	    }
	}
}