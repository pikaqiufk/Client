using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(MultiCurveTrack))]
public class MultiCurveTrackInspector : Editor
{
    // Properties
    private SerializedObject curveTrack;
    private GUIContent addClip = new GUIContent("Add Clip Curve", "Add a new clip curve to this track.");

    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        curveTrack = new SerializedObject(this.target);
    }


    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        curveTrack.Update();
        {
            var __array1 = (target as MultiCurveTrack).TimelineItems;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var clip = (CinemaMultiActorCurveClip)__array1[__i1];
                {
                    EditorGUILayout.ObjectField(clip.name, clip, typeof(CinemaMultiActorCurveClip), true);
                }
            }
        }
        if (GUILayout.Button(addClip))
        {
            Undo.RegisterCreatedObjectUndo(CutsceneItemFactory.CreateMultiActorClipCurve((target as MultiCurveTrack)).gameObject, "Create Curve Clip");
        }

        curveTrack.ApplyModifiedProperties();
    }
}
