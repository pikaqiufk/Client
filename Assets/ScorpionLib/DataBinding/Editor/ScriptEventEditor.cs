using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ScriptEventEditor
{
    public static ScriptEventEditor _Instance = null;

    public ScriptEventEditor()
    {
        Init();
    }
    public static ScriptEventEditor Instance
    {
        get { return _Instance ?? (_Instance = new ScriptEventEditor()); }
    }
    public List<string> EventList = new List<string>();
    private bool Init()
    {
        var ta = Resources.LoadAssetAtPath("Assets/Res/UI/ScriptEventDefine.txt", typeof(TextAsset)) as TextAsset;
        var ary = ta.text.Split('\n');
        EventList = new List<string>();
        EventList.Add("None");
        {
            var __array1 = ary;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var s = (string)__array1[__i1];
                {
                    int i = s.IndexOf('\r');
                    EventList.Add(i != -1 ? s.Remove(i) : s);
                }
            }
        }
        return true;
    }

    public int GetSelect(string select)
    {
        int ret = 0;
        {
            var __list2 = EventList;
            var __listCount2 = __list2.Count;
            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
            {
                var s = (string)__list2[__i2];
                {
                    if (s.Equals(select))
                    {
                        return ret;
                    }
                    ret++;
                }
            }
        }
        return 0;
    }
}
