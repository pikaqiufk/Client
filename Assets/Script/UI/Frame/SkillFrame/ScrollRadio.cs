using System;
#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class ScrollRadio : MonoBehaviour
	{
	    private Transform transform;
	    // Use this for initialization
	    private UIPanel uiPanel;
	    public float radio = 0.1f;
	    private Vector2 posVec2;
	    private float x;
	    private float y;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        transform = gameObject.transform;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    private void Start()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var scrollview = gameObject.GetComponentInParent<UIScrollView>();
	        uiPanel = scrollview.gameObject.GetComponent<UIPanel>();
	        posVec2.x = transform.localPosition.x;
	        posVec2.y = transform.localPosition.y;
	
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
	
	        x = posVec2.x + uiPanel.clipOffset.x*(1 - radio);
	        y = posVec2.y + uiPanel.clipOffset.y*(1 - radio);
	        transform.localPosition = new Vector3(x, y, 1.0f);
	
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