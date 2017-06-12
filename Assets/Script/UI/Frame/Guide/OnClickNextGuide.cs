#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GameUI
{
	public class OnClickNextGuide : MonoBehaviour
	{
	    public List<int> GuideStepList = new List<int>();
	
	    private void OnClick()
	    {
	        if (!GuideManager.Instance.IsGuiding())
	        {
	            return;
	        }
	
	        var data = GuideManager.Instance.GetCurrentGuideData();
	        if (null == data)
	        {
	            return;
	        }
	
	        if (GuideStepList.Count > 0)
	        {
	            var ok = false;
	            {
	                var __list1 = GuideStepList;
	                var __listCount1 = __list1.Count;
	                for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	                {
	                    var id = __list1[__i1];
	                    {
	                        if (id == data.Id)
	                        {
	                            ok = true;
	                            break;
	                        }
	                    }
	                }
	            }
	            if (!ok)
	            {
	                return;
	            }
	        }
	        GuideManager.Instance.NextStep();
	    }
	}
}