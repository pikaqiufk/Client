#region using


using System;
using UnityEngine;


#endregion

namespace GameUI
{
	public class CutGridBG : MonoBehaviour
	{
	    public UISprite BackGround;
	    // Use this for initialization
	
	    public UIGridSimple Grid;
	    private int itemCount;
	    private int lastCount = -1;
	    public int Padding = 5;

        private void Start()
        {
#if !UNITY_EDITOR
            try
            {
#endif
                if (null == Grid)
                {
                    Grid = gameObject.GetComponent<UIGridSimple>();
                }

                if (null == Grid || null == BackGround)
                {
                    Logger.Error("CutGridBackGround cant find background or grid");
                    NGUITools.Destroy(this);
                    return;
                }
                Update();
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
	        itemCount = Grid.GetShowCount();
	        if (itemCount == lastCount)
	        {
	            return;
	        }
	
	        if (itemCount == 0)
	        {
	            BackGround.width = (int) Grid.cellWidth + Padding*2;
	            BackGround.height = (int) Grid.cellHeight + Padding*2;
	            return;
	        }
	
	        lastCount = itemCount;
	
	        if (Grid.arrangement == UIGridSimple.Arrangement.Horizontal)
	        {
	            var perLine = Grid.maxPerLine;
	
	            var widthCount = 0;
	
	            if (itemCount >= perLine)
	            {
	                widthCount = perLine;
	            }
	            else
	            {
	                widthCount = itemCount%perLine;
	            }
	
	            BackGround.width = widthCount*(int) Grid.cellWidth + Padding*2;
	
	
	            var maxPerHeight = (int) (Grid.maxlenth/Grid.cellHeight);
	
	            var heightCount = itemCount/Grid.maxPerLine;
	
	            if (itemCount%Grid.maxPerLine != 0)
	            {
	                heightCount += 1;
	            }
	
	            if (heightCount > maxPerHeight)
	            {
	                heightCount = maxPerHeight;
	            }
	
	            BackGround.height = heightCount*(int) Grid.cellHeight + Padding*2;
	        }
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