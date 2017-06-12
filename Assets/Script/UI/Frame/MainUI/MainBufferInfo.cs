using System;
#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class MainBufferInfo : MonoBehaviour
	{
	    public UISprite BackGroundSprite;
	    public float BuffInfoOffSet = 100.0f;
	    public UILabel DescLabel;
	    private int beginHeight;
	
	    private void Awake()
	    {
	#if !UNITY_EDITOR
	try
	{
	#endif
	
	        var uiroot = gameObject.transform.root.GetComponent<UIRoot>();
	        if (null != uiroot)
	        {
	            var s = (float) uiroot.activeHeight/Screen.height;
	            BuffInfoOffSet = BuffInfoOffSet/s;
	        }
	
	        beginHeight = BackGroundSprite.height;
	
	#if !UNITY_EDITOR
	}
	catch (Exception ex)
	{
	    Logger.Error(ex.ToString());
	}
	#endif
	    }
	
	    public void CalculateBackGroundHeight()
	    {
	        BackGroundSprite.height = beginHeight + (int) DescLabel.printedSize.y;
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
	
	    public void UpdatePostion(Vector2 touchPos)
	    {
	        var pos = touchPos + new Vector2(BuffInfoOffSet, BackGroundSprite.height*0.5f);
	        transform.position = UICamera.currentCamera.ScreenToWorldPoint(pos);
	    }
	}
}