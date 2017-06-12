
using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(StackLayout), true)]
public class StackLayoutEditor : UIWidgetInspector
{
    protected override void DrawCustomProperties()
    {
        NGUIEditorTools.DrawProperty("Offset", serializedObject, "mOffset", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Padding", serializedObject, "mPadding", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Layout Type", serializedObject, "mLayoutType", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Max Count", serializedObject, "mMaxCount", GUILayout.MinWidth(20f));
        NGUIEditorTools.DrawProperty("Min Height", serializedObject, "mMinHeight", GUILayout.MinWidth(20f));
        base.DrawCustomProperties();

        StackLayout layout = target as StackLayout;
        layout.ResetLayout();
    }
}
