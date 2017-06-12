//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(TimerLogic))]
public class TimerLogicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NGUIEditorTools.BeginContents();
        NGUIEditorTools.DrawProperty("Text Label", serializedObject, "TextLabel");
        NGUIEditorTools.DrawProperty("Is Add", serializedObject, "IsAdd");
        NGUIEditorTools.DrawProperty("Is Start", serializedObject, "IsStart");
        NGUIEditorTools.EndContents();
        TimerLogic button = target as TimerLogic;
        NGUIEditorTools.DrawEvents("Update Delegate", button, button.UpdateDelegate, false);

        serializedObject.ApplyModifiedProperties();
    }
}
