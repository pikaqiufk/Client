//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIProgressBar))]
#else
[CustomEditor(typeof(UIProgressBar), true)]
#endif
public class UIProgressBarEditor : UIWidgetContainerEditor
{
    public override void OnInspectorGUI()
    {
        NGUIEditorTools.SetLabelWidth(80f);

        serializedObject.Update();

        GUILayout.Space(3f);

        DrawLegacyFields();

        if (serializedObject.targetObject as UISliderLoop)
        {
            NGUIEditorTools.DrawProperty("Target Value", serializedObject, "TargetValue", GUILayout.MinWidth(20f));
        }

        GUILayout.BeginHorizontal();

        SerializedProperty sp = NGUIEditorTools.DrawProperty("Steps", serializedObject, "numberOfSteps", GUILayout.Width(110f));
        if (sp.intValue == 0) GUILayout.Label("= unlimited");
        GUILayout.EndHorizontal();
        if (serializedObject.targetObject as UISliderLoop
            || serializedObject.targetObject as UISliderDouble
            || serializedObject.targetObject as UISliderMove
            || serializedObject.targetObject as UISliderNormal)
        {
            NGUIEditorTools.DrawProperty("ScheduleLabel", serializedObject, "ScheduleLabel", GUILayout.MinWidth(20f));
        }

        if (serializedObject.targetObject as UISliderLoop
            || serializedObject.targetObject as UISliderDouble
            || serializedObject.targetObject as UISliderMove)
        {
            NGUIEditorTools.DrawProperty("Speed", serializedObject, "Speed", GUILayout.MinWidth(20f));
        }

        if (serializedObject.targetObject as UISliderMove)
        {
            NGUIEditorTools.DrawProperty("TargetValue", serializedObject, "mTargetValue", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("MaxValue", serializedObject, "mMaxValue", GUILayout.MinWidth(20f));
            NGUIEditorTools.DrawProperty("CurrentValue", serializedObject, "mCurrentValue", GUILayout.MinWidth(20f));
        }

        OnDrawExtraFields();

        if (NGUIEditorTools.DrawHeader("Appearance", "Appearance", false, true))
        {
            NGUIEditorTools.BeginContents(true);
            NGUIEditorTools.DrawProperty("Foreground", serializedObject, "mFG");
            if (serializedObject.targetObject as UISliderDouble)
            {
                NGUIEditorTools.DrawProperty("MaskGround", serializedObject, "MaskGround");
            }
            NGUIEditorTools.DrawProperty("Background", serializedObject, "mBG");
            NGUIEditorTools.DrawProperty("Thumb", serializedObject, "thumb");

            GUILayout.BeginHorizontal();
            NGUIEditorTools.DrawProperty("Direction", serializedObject, "mFill");
            NGUIEditorTools.DrawPadding();
            GUILayout.EndHorizontal();

            OnDrawAppearance();
            NGUIEditorTools.EndContents();
        }

        UIProgressBar sb = target as UIProgressBar;
        NGUIEditorTools.DrawEvents("On Value Change", sb, sb.onChange);
        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void DrawLegacyFields()
    {
        UIProgressBar sb = target as UIProgressBar;
        float val = EditorGUILayout.Slider("Value", sb.value, 0f, 1f);
        float alpha = EditorGUILayout.Slider("Alpha", sb.alpha, 0f, 1f);

        if (sb.value != val ||
            sb.alpha != alpha)
        {
            NGUIEditorTools.RegisterUndo("Progress Bar Change", sb);
            sb.value = val;
            sb.alpha = alpha;
            NGUITools.SetDirty(sb);

            for (int i = 0; i < UIScrollView.list.size; ++i)
            {
                UIScrollView sv = UIScrollView.list[i];

                if (sv.horizontalScrollBar == sb || sv.verticalScrollBar == sb)
                {
                    NGUIEditorTools.RegisterUndo("Progress Bar Change", sv);
                    sv.UpdatePosition();
                }
            }
        }
    }

    protected virtual void OnDrawExtraFields() { }
    protected virtual void OnDrawAppearance() { }
}
