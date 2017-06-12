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

[CustomPropertyDrawer(typeof(ScriptEventItem))]
public class ScriptEventItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        var editor = ScriptEventEditor.Instance;
        SerializedProperty targetName = prop.FindPropertyRelative("EventName");
        string curSel = targetName.stringValue;
        rect.height = 16.0f;
        SerializedProperty targetArg = prop.FindPropertyRelative("EventArg");
        
        rect.width = rect.width*0.7f;
        int index = ScriptEventEditor.Instance.GetSelect(curSel);
        GUI.changed = false;
        string[] names = ScriptEventEditor.Instance.EventList.ToArray();
        EditorGUI.BeginDisabledGroup(targetName.hasMultipleDifferentValues);
        int choice = EditorGUI.Popup(rect, "", index, names);
        if (GUI.changed && choice >= 0)
        {
            string del = names[choice];
            targetName.stringValue = del;
        }
        EditorGUI.EndDisabledGroup();
        rect.x += rect.width;
        EditorGUI.PropertyField(rect, targetArg, label);
    }
}
