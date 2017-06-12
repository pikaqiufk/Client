#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GameUI
{
	public class StarPlace : MonoBehaviour
	{
	    private int starLightCount;
	    private int starMaxCount;
	    public List<UISprite> StarList;
	
	    public int LightCount
	    {
	        get { return starLightCount; }
	        set
	        {
	            if (starMaxCount == 0)
	            {
	                starMaxCount = StarList.Count;
	            }
	            starLightCount = value;
	            for (var i = 0; i < MaxCount; i++)
	            {
	                var sprite = StarList[i];
	                if (i < starLightCount)
	                {
	                    GameUtils.SetSpriteGrey(sprite, false);
	                }
	                else
	                {
	                    GameUtils.SetSpriteGrey(sprite, true);
	                }
	            }
	        }
	    }
	
	    public int MaxCount
	    {
	        get { return starMaxCount; }
	        set
	        {
	            if (value > StarList.Count)
	            {
	                starMaxCount = StarList.Count;
	            }
	            else
	            {
	                starMaxCount = value;
	            }
	
	            var count = StarList.Count;
	            for (var i = 0; i < count; i++)
	            {
	                StarList[i].gameObject.SetActive(i < starMaxCount);
	            }
	        }
	    }
	}
}