using System;
#region using

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GameUI
{
	public class SelectToggleControl : MonoBehaviour
	{
	    private int select;
	    public List<UIToggle> OperateMenus;
	
	    public int Select
	    {
	        get { return select; }
	        set
	        {
	            select = value;
	            if (select != -1)
	            {
	                SetSelect();
	            }
	            else
	            {
	                {
	                    var __list1 = OperateMenus;
	                    var __listCount1 = __list1.Count;
	                    for (var __i1 = 0; __i1 < __listCount1; ++__i1)
	                    {
	                        var menu = __list1[__i1];
	                        {
	                            menu.value = false;
	                            menu.mIsActive = false;
	                            menu.startsActive = false;
	                            if (menu.activeSprite != null)
	                            {
	                                menu.activeSprite.alpha = 0.0f;
	                            }
	                            var objs = menu.GetComponent<UIToggledObjects>();
	                            if (null != objs && null != objs.activate && objs.activate.Count > 0)
	                            {
	                                // foreach(var o in objs.activate)
	                                var __enumerator4 = (objs.activate).GetEnumerator();
	                                while (__enumerator4.MoveNext())
	                                {
	                                    var o = __enumerator4.Current;
	                                    {
	                                        o.SetActive(false);
	                                    }
	                                }
	                            }
	                        }
	                    }
	                }
	            }
	        }
	    }
	
	    public void SetSelect()
	    {
	        if (select < 0 || select >= OperateMenus.Count)
	        {
	            return;
	        }
	
	        var flag = 0;
	        {
	            var __list2 = OperateMenus;
	            var __listCount2 = __list2.Count;
	            for (var __i2 = 0; __i2 < __listCount2; ++__i2)
	            {
	                var menu = __list2[__i2];
	                {
	                    if (menu == null)
	                    {
	                        continue;
	                    }
	                    if (flag == select)
	                    {
	                        flag++;
	                        continue;
	                    }
	                    flag++;
	                    menu.value = false;
	                    menu.mIsActive = false;
	                    menu.startsActive = false;
	                    if (menu.activeSprite != null)
	                    {
	                        menu.activeSprite.alpha = 0.0f;
	                    }
	                    var objs = menu.GetComponent<UIToggledObjects>();
	                    if (null != objs && null != objs.activate && objs.activate.Count > 0)
	                    {
	                        // foreach(var o in objs.activate)
	                        var __enumerator5 = (objs.activate).GetEnumerator();
	                        while (__enumerator5.MoveNext())
	                        {
	                            var o = __enumerator5.Current;
	                            {
	                                if (o)
	                                {
	                                    o.SetActive(false);
	                                }
	                            }
	                        }
	                    }
	                }
	            }
	        }
	        {
	            var objs = OperateMenus[select].GetComponent<UIToggledObjects>();
	            if (null != objs && null != objs.activate && objs.activate.Count > 0)
	            {
	                var __list3 = objs.activate;
	                var __listCount3 = __list3.Count;
	                for (var __i3 = 0; __i3 < __listCount3; ++__i3)
	                {
	                    var o = __list3[__i3];
	                    {
	                        if (o)
	                        {
	                            o.SetActive(true);
	                        }
	                    }
	                }
	                {
	                    // foreach(var o in objs.deactivate)
	                    var __enumerator6 = (objs.deactivate).GetEnumerator();
	                    while (__enumerator6.MoveNext())
	                    {
	                        var o = __enumerator6.Current;
	                        {
	                            if (o)
	                            {
	                                o.SetActive(false);
	                            }
	                        }
	                    }
	                }
	            }
	        }
	        if (OperateMenus[select].value)
	        {
	            return;
	        }
	        if (OperateMenus[select].IsStart)
	        {
	            OperateMenus[select].value = true;
	            return;
	        }
	        {
	            OperateMenus[select].value = true;
	            OperateMenus[select].mIsActive = true;
	            OperateMenus[select].startsActive = true;
	            if (OperateMenus[select].activeSprite != null)
	            {
	                OperateMenus[select].activeSprite.alpha = 1.0f;
	            }
	        }
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
	}
}