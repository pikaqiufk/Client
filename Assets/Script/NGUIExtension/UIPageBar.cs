using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIPageBar : MonoBehaviour
{
    public List<UISprite> Pages;
    public int PageType;

    public UILabel Lable;
	void Start () 
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
    public void RefreshLight(float progress,int total)
    {
        if (PageType == 0)
        {
            int count = (int)(Pages.Count * progress);
            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].gameObject.SetActive(i == count);
            }    
        }
        else if (PageType == 1)
        {
            if (Lable != null)
            {
                if (total == 0)
                {
                    Lable.text = "0/0";
                }
                else
                {
                    int count = Mathf.CeilToInt(total * progress);
                    if (count == 0)
                    {
                        count = 1;
                    }
                    Lable.text = String.Format("{0}/{1}", count, total);
                }
            }
        }
    }
}
