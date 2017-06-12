using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(CurveTrack))]
public class CurveTrackInspector : Editor
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
            var __array1 = (target as CurveTrack).TimelineItems;
            var __arrayLength1 = __array1.Length;
            for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
            {
                var clip = (CinemaActorClipCurve)__array1[__i1];
                {
                    EditorGUILayout.ObjectField(clip.name, clip, typeof(CinemaActorClipCurve), true);
                }
            }
        }
        if (GUILayout.Button(addClip))
        {
            Undo.RegisterCreatedObjectUndo(CutsceneItemFactory.CreateActorClipCurve((target as CurveTrack)).gameObject, "Create Curve Clip");
        }

        curveTrack.ApplyModifiedProperties();
    }
}
