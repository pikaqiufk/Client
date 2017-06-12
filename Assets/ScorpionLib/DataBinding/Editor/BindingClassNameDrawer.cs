using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Component = UnityEngine.Component;

[CustomPropertyDrawer(typeof(BindingClassName))]
public class BindingClassNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty target = prop.FindPropertyRelative("ClassName");
        string curSel = target.stringValue;

        List<string> strList = new List<string>();
        //List<string> strListOrder = new List<string>();
        SerializedObject serObj = target.serializedObject;
        //strListOrder.Add(curSel == "" ? "none" : curSel);

        int index = 0;

        if (serObj.targetObject is BindDataRoot)
        {
            var path = Application.dataPath + "/Plugins/DataModel/";
			Assembly ass = Assembly.LoadFile(Path.Combine(path, "GameDataDefine.dll"));
            Type[] types = ass.GetTypes();
            int tempIndex = 0;
            {
                var __array1 = types;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var t = (Type)__array1[__i1];
                    {
                        if (CheckClassProperChanged(t, 0))
                        {
                            strList.Add(t.Name);
                        }
                    }
                }
            }
        }
        else
        {
            Transform tf = null;
            BindDataRoot root = null;
            if (serObj.targetObject is InverseBinding)
            {
                InverseBinding tarObj = serObj.targetObject as InverseBinding;
                //tf = tarObj.gameObject.transform.parent;
                tf = tarObj.gameObject.transform;
            }
            else if (serObj.targetObject is UIClassBinding)
            {
                UIClassBinding tarObj = serObj.targetObject as UIClassBinding;
                //tf = tarObj.gameObject.transform.parent;
                tf = tarObj.gameObject.transform;
            }


            while (tf != null)
            {
                root = tf.gameObject.GetComponent<BindDataRoot>();
                if (root != null)
                {
                    {
                        var __list2 = root.BindingNamelList;
                        var __listCount2 = __list2.Count;
                        for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                        {
                            var name = __list2[__i2];
                            {
                                strList.Add(name.ClassName);
                            }
                        }
                    }
                }
                tf = tf.parent;
            }
        }

        if (strList.Count == 0)
        {
            return;
        }
        strList.Sort();
        {
            var __list3 = strList;
            var __listCount3 = __list3.Count;
            for (int __i3 = 0; __i3 < __listCount3; ++__i3)
            {
                var s = __list3[__i3];
                {
                    if (curSel == s)
                    {
                        break;
                    }
                    index++;
                }
            }
        }
        GUI.changed = false;
        EditorGUI.BeginDisabledGroup(target.hasMultipleDifferentValues);
        int choice = EditorGUI.Popup(rect, "", index, strList.ToArray());

        if (GUI.changed && choice >= 0)
        {
            string str = strList[choice];
            target.stringValue = str;
        }
        EditorGUI.EndDisabledGroup();
    }
    public bool CheckClassName(string name)
    {
        if (name[0] == '_' ||
            name.Contains("Msg") ||
            name.Contains("OutMessage"))
        {
            return false;
        }
        return true;
    }
    public bool CheckClassProperChanged(Type type, int depth)
    {
        depth++;
        if (depth >= 3)
        {
            return false;
        }
        if (!CheckClassName(type.Name))
        {
            return false;
        }
        if (typeof(INotifyPropertyChanged).IsAssignableFrom(type))
        {
            return true;
        }


        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        PropertyInfo[] props = type.GetProperties(flags);
        return props.Any(prop => CheckClassProperChanged(prop.PropertyType, depth));
    }
}
