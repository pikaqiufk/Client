//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Reflection;
using ClientDataModel;
using Component = UnityEngine.Component;

/// <summary>
/// Generic property binding drawer.
/// </summary>

#if !UNITY_3_5
[CustomPropertyDrawer(typeof(SourceBindingProperty))]
public class SourceBindingPropertyDrawer : PropertyDrawer
#else
public class PropertyReferenceDrawer
#endif
{
    public class Entry
    {
        //public Component target;
        public object Target;
        public string Name;
        public List<string> Path = new List<string>();
    }
    static public void FillProperties(Type type, ref List<Entry> list, List<string> path)
    {
        if (path != null)
        {
            return;
        }

        if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
        {
            return;
        }

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        PropertyInfo[] props = type.GetProperties(flags);
        {
            var __array1 = props;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var prop = (PropertyInfo)__array1[__i1];
                {
                    if (!prop.CanRead) continue;

                    Entry ent = new Entry();
                    ent.Name = prop.Name;

                    if (path != null)
                    {
                        {
                            var __list2 = path;
                            var __listCount2 = __list2.Count;
                            for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                            {
                                var s = (string)__list2[__i2];
                                {
                                    ent.Path.Add(s);
                                }
                            }
                        }
                    }
                    ent.Path.Add(type.ToString());
                    list.Add(ent);

                    FillProperties(prop.PropertyType, ref list, ent.Path);
                }
            }
        }
    }

    static public List<Entry> GetProperties(GameObject target)
    {
        Component[] comps = target.GetComponents<Component>();
        List<Entry> list = new List<Entry>();
        for (int i = 0, imax = comps.Length; i < imax; ++i)
        {
            Component comp = comps[i];
            if (comp == null) continue;

            Type type = comp.GetType();

            FillProperties(type, ref list, null);
        }

        return list;
    }

    static public string[] GetNames(List<Entry> list, string choice, out int index)
    {
        index = 0;
        string[] names = new string[list.Count + 1];
        names[0] = string.IsNullOrEmpty(choice) ? "<Choose>" : choice;
        for (int i = 0; i < list.Count;)
        {
            Entry ent = list[i];
            string del = "";
            for (int j = 0; j < ent.Path.Count; j++)
            {
                del += ent.Path[j];
                del += "/";
            }
            del += ent.Name;
            names[++i] = del;
            if (index == 0 && string.Equals(del, choice))
                index = i;
        }
        return names;
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        SerializedProperty target = prop.FindPropertyRelative("mTarget");
        Component comp = target.objectReferenceValue as Component;
        return (comp != null) ? 36f : 16f;
    }

    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty target = prop.FindPropertyRelative("mTarget");
        SerializedProperty field = prop.FindPropertyRelative("mName");

        rect.height = 16f;
        EditorGUI.PropertyField(rect, target, label);

        Component comp = target.objectReferenceValue as Component;

        if (comp != null)
        {
            rect.y += 18f;
            GUI.changed = false;
            EditorGUI.BeginDisabledGroup(target.hasMultipleDifferentValues);
            int index = 0;

            List<Entry> list1 = GetProperties(comp.gameObject);
            string current = SourceBindingProperty.ToString(target.objectReferenceValue as Component, field.stringValue);
            string[] names = SourceBindingPropertyDrawer.GetNames(list1, current, out index);
            GUI.changed = false;
            rect.xMin += EditorGUIUtility.labelWidth;
            rect.width -= 18f;
            int choice = EditorGUI.Popup(rect, "", index, names);

            if (GUI.changed && choice > 0)
            {
                Entry ent = list1[choice - 1];
                string del = "";
                for (int j = 0; j < ent.Path.Count; j++)
                {
                    del += ent.Path[j];
                    del += ":";
                }
                del += ent.Name;
                field.stringValue = del;
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
