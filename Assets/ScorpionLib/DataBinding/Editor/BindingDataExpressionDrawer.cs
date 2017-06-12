using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using ClientDataModel;
using DataTable;
using Component = UnityEngine.Component;

[CustomPropertyDrawer(typeof(BindingDataExpression))]


public class BindingDataExpressionDrawer : PropertyDrawer
{
    public int MaxDepth = 10;
    public void FillProperties(Type type, ref List<List<string>> list, List<string> path, bool inList,int depth)
    {
        if (depth != -1)
        {
            if (depth > MaxDepth)
            {
                return;
            }
            depth++;
        }

        if (list.Count > 10000)
        {
            return;
        }

        if (null == type)
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
                    List<string> insertList = new List<string>();

                    if (path != null)
                    {
                        insertList.AddRange(path);
                    }

                    if (inList == false)
                    {
                        insertList.Add(prop.Name);

                        list.Add(insertList);
                    }

                    if (prop.IsDefined(typeof(ListSize), false))
                    {
                        FillPropertiesList(prop, ref list, insertList, depth);
                    }
                    else if (prop.IsDefined(typeof(TableBinding), false))
                    {
                        FillPropertiesTable(prop, ref list, insertList, depth);   
                    }
                    else
                    {
                        if (typeof(INotifyPropertyChanged).IsAssignableFrom(prop.PropertyType))
                        {
                            FillProperties(prop.PropertyType, ref list, insertList, false, depth);
                        }
                    }
                }
            }
        }
    }
    public void FillPropertiesList(PropertyInfo propInfo, ref List<List<string>> list, List<string> path, int depth)
    {
        if (depth != -1)
        {
            if (depth > MaxDepth)
            {
                return;
            }
            depth++;
        }
        object[] obj = propInfo.GetCustomAttributes(typeof(ListSize), false);
        ListSize[] attr = obj as ListSize[];
        int size = attr[0].Size;
        for (int i = 0; i < size; i++)
        {
            List<string> insertList = new List<string>();
            if (path != null)
            {
                insertList.AddRange(path);
            }
            insertList.Add(i.ToString());
            list.Add(insertList);

            if (propInfo.IsDefined(typeof(TableBinding), false))
            {
                FillPropertiesTable(propInfo, ref list, insertList, depth);
                continue;
            }

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(propInfo.PropertyType))
            {
                FillProperties(propInfo.PropertyType, ref list, insertList, true, depth);
                continue;
            }
        }
    }
    public void FillPropertiesTable(PropertyInfo propInfo, ref List<List<string>> list, List<string> path,int depth)
    {
        if (depth != -1)
        {
            if (depth > MaxDepth)
            {
                return;
            }
            depth++;
        }
        if (!propInfo.IsDefined(typeof(TableBinding), false))
        {
            return;
        }

        object[] obj = propInfo.GetCustomAttributes(typeof(TableBinding), false);
        TableBinding[] attr = obj as TableBinding[];

        string name = "DataTable." + attr[0].TableName + "Record";
        var tbType = Type.GetType(name + ",Assembly-CSharp-firstpass") ?? Type.GetType(name + ",Assembly-CSharp");
        if (tbType == null)
        {
            return;
        }
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        PropertyInfo[] props = tbType.GetProperties(flags);
        {
            var __array2 = props;
            var __arrayLength2 = __array2.Length;
            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
            {
                var prop = (PropertyInfo)__array2[__i2];
                {
                    if (!prop.CanRead) continue;

                    List<string> insertList = new List<string>();

                    if (path != null)
                    {
                        insertList.AddRange(path);
                    }
                    insertList.Add(prop.Name);

                    list.Add(insertList);
                    if (prop.IsDefined(typeof(ListSize), false))
                    {
                        FillPropertiesList(prop, ref list, insertList, depth);
                    }
                    else
                    {
                        FillPropertiesTable(prop, ref list, insertList, depth);
                    }
                }
            }
        }
    }

    public string[] GetNames(List<List<string>> list, string choice, out int index)
    {
        index = 0;
        string[] names = new string[list.Count + 1];
        names[0] = string.IsNullOrEmpty(choice) ? "<Choose>" : choice;
        for (var i = 0; i < list.Count;)
        {
            List<string> ent = list[i];
            string del = "";
            for (int j = 0; j < ent.Count; j++)
            {
                del += ent[j];
                if (j != ent.Count - 1)
                {
                    del += "/";
                }
            }
            names[++i] = del;
            if (index == 0 && string.Equals(del, choice))
                index = i;
        }
        return names;
    }

    private Type mType = null;
    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty target = prop.FindPropertyRelative("DataExpressionName");

        SerializedObject serObj = target.serializedObject;

        var path = Application.dataPath + "/Plugins/DataModel/";
		Assembly ass = Assembly.LoadFile(Path.Combine(path, "GameDataDefine.dll"));
        Type[] types = ass.GetTypes();
        mType = null;

        UIClassBinding tarObj = serObj.targetObject as UIClassBinding;

        InverseBinding tarObj1 = serObj.targetObject as InverseBinding;
        {
            var __array3 = types;
            var __arrayLength3 = __array3.Length;
            for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
            {
                var t = __array3[__i3];
                {
                    if (tarObj != null && t.Name == tarObj.BindingName.ClassName)
                    {
                        mType = t;
                        break;
                    }

                    if (tarObj1 != null && t.Name == tarObj1.BindingName.ClassName)
                    {
                        mType = t;
                        break;
                    }
                }
            }
        }
        List<List<string>> list = new List<List<string>>();

        List<string> thislist = new List<string>();
        thislist.Add("this");
        list.Add(thislist);
        var depth = 0;
        FillProperties(mType, ref list, null, false, depth);
        int index = 0;

        string strValue = target.stringValue;

        

        string current = strValue;
//        var strList = strValue.Split('.');
//         string subStr = "";
//         if (strList.Count() <= 3)
//         {
//             current = target.stringValue;    
//         }
//         else
//         {
//             for (int i = 0; i < 3; i++)
//             {
//                 current += strList[i];
//                 if (i != 3 -1)
//                 {
//                     current += '.';
//                 }
//             }
// 
// 
//             for (int i = 3; i < strList.Count(); i++)
//             {
//                 subStr += strList[i];
//                 if (i != strList.Count() - 1)
//                 {
//                     subStr += '.';
//                 }
//             }
//         }

        string[] names = GetNames(list, current, out index);

        GUI.changed = false;
        EditorGUI.BeginDisabledGroup(target.hasMultipleDifferentValues);
//         if (strList.Count() >= 3)
//         {
//             rect.width = rect.width / 2;
//         }
        
        int choice = EditorGUI.Popup(rect, "", index, names);

//         if (strList.Count() >= 3)
//         {
//             rect.x = rect.width ;
//         }

        if (GUI.changed && choice > 0)
        {
            List<string> ent = list[choice - 1];
            string del = "";
            for (var j = 0; j < ent.Count; j++)
            {
                del += ent[j];
                if (j != ent.Count - 1)
                {
                    del += ".";
                }
            }
            target.stringValue = del;
//            subStr = "";
            current = del;
            GUI.changed = false;
        }

        EditorGUI.EndDisabledGroup();
        //ShowSubsequent(rect,target,current,subStr);
    }

    public PropertyInfo GetListItemProp(Type type)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        PropertyInfo[] props = type.GetProperties(flags);

        if (props.Length != 2)
        {
            return null;
        }
        var p = props[1];
        if (typeof (INotifyPropertyChanged).IsAssignableFrom(p.PropertyType))
        {
            return p;
        }
        return null;
    }
    public void ShowSubsequent(Rect rect,SerializedProperty target,string current,string subStr)
    {
        var arg = current.Split('.');
        if (arg.Count() != 3)
        {
            return;
        }
        Type type = mType;
        var prop = type.GetProperty(arg[0]);
       
        var flag = 1;
        while (flag < 3)
        {
            if (type == null || prop == null)
            {
                break;
            }
            if (prop.IsDefined(typeof(ListSize), false))
            {
                prop = GetListItemProp(prop.PropertyType);
                flag++;
                if (flag < 3 && prop != null)
                {
                    prop = prop.PropertyType.GetProperty(arg[flag]);
                }
                type = prop != null ? prop.PropertyType : null;
            }
            else if (prop.IsDefined(typeof(TableBinding), false))
            {

                object[] obj = prop.GetCustomAttributes(typeof(TableBinding), false);
                TableBinding[] attr = obj as TableBinding[];

                string name = "DataTable." + attr[0].TableName + "Record";
                var tbType = Type.GetType(name + ",Assembly-CSharp-firstpass") ?? Type.GetType(name + ",Assembly-CSharp");
                if (tbType == null)
                {
                    return;
                }
                type = tbType;
                flag++;
                
                if (flag < 3)
                {
                    prop = tbType.GetProperty(arg[flag]);

                    type = prop != null ? prop.PropertyType : null;
                }
            }
            else
            {
                if (typeof(INotifyPropertyChanged).IsAssignableFrom(prop.PropertyType))
                {
                    prop = prop.PropertyType.GetProperty(arg[flag]);
                    type = prop != null ? prop.PropertyType : null;
                }
                flag++;
            }

            
        }


        List<List<string>> list = new List<List<string>>();

        if (type == null || prop == null)
        {
            return;
        }
        var depth = -1;

        if (prop.IsDefined(typeof(ListSize), false))
        {
            FillPropertiesList(prop, ref list, null, depth);
        }
        else if (prop.IsDefined(typeof(TableBinding), false))
        {
            FillPropertiesTable(prop, ref list, null, depth);
        }

        FillProperties(type, ref list, null, false, depth);
        int index = 0;


        string[] names = GetNames(list, subStr, out index);

        GUI.changed = false;
        EditorGUI.BeginDisabledGroup(false);

        int choice = EditorGUI.Popup(rect, "", index, names);

        if (GUI.changed && choice > 0)
        {
            List<string> ent = list[choice - 1];
            string del = "";
            for (var j = 0; j < ent.Count; j++)
            {
                del += ent[j];
                if (j != ent.Count - 1)
                {
                    del += ".";
                }
            }
            subStr = del;
            target.stringValue = current + "." + subStr;
            GUI.changed = false;
        }

    }
}
