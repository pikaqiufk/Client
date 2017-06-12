#region using

using UnityEngine;

#endregion

namespace GameUI
{
	public class BigNumLogic : MonoBehaviour
	{
	    public UILabel Label;
	
	    public int Count
	    {
	        set
	        {
	            var str = GameUtils.GetBigValueStr(value);
	            if (Label != null)
	            {
	                Label.text = str;
	            }
	        }
	    }
	}
}