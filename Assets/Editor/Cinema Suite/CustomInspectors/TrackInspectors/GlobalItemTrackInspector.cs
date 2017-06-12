using System;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(GlobalItemTrack))]
public class GlobalItemTrackInspector : Editor
{
    // Properties
    private SerializedObject eventTrack;
    private bool actionFoldout = true;

    GUIContent addActionContent = new GUIContent("Add New Action", "Add a new action to this track.");
    GUIContent actionContent = new GUIContent("Actions", "The actions associated with this track.");
    /// <summary>
    /// On inspector enable, load serialized objects
    /// </summary>
    public void OnEnable()
    {
        eventTrack = new SerializedObject(this.target);
    }

    /// <summary>
    /// Update and Draw the inspector
    /// </summary>
    public override void OnInspectorGUI()
    {
        eventTrack.Update();
        GlobalItemTrack track = base.serializedObject.targetObject as GlobalItemTrack;

        CinemaGlobalAction[] actions = track.Actions;
        CinemaGlobalEvent[] events = track.Events;

        if (actions.Length > 0 || events.Length > 0)
        {
            actionFoldout = EditorGUILayout.Foldout(actionFoldout, actionContent);
            if (actionFoldout)
            {
                EditorGUI.indentLevel++;
                {
                    var __array1 = actions;
                    var __arrayLength1 = __array1.Length;
                    for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                    {
                        var action = (CinemaGlobalAction)__array1[__i1];
                        {
                            EditorGUILayout.ObjectField(action.name, action, typeof(CinemaGlobalAction), true);
                        }
                    }
                }
                {
                    var __array2 = events;
                    var __arrayLength2 = __array2.Length;
                    for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var globalEvent = (CinemaGlobalEvent)__array2[__i2];
                        {
                            EditorGUILayout.ObjectField(globalEvent.name, globalEvent, typeof(CinemaGlobalEvent), true);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        if (GUILayout.Button(addActionContent))
        {
            GenericMenu createMenu = new GenericMenu();
            {
                // foreach(var type in DirectorHelper.GetAllSubTypes(typeof(CinemaGlobalAction)))
                var __enumerator3 = (DirectorHelper.GetAllSubTypes(typeof(CinemaGlobalAction))).GetEnumerator();
                while (__enumerator3.MoveNext())
                {
                    var type = (Type)__enumerator3.Current;
                    {
                        string text = string.Empty;
                        string category = string.Empty;
                        string label = string.Empty;
                        {
                            var __array5 = type.GetCustomAttributes(typeof(CutsceneItemAttribute), true);
                            var __arrayLength5 = __array5.Length;
                            for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                            {
                                var attribute = (CutsceneItemAttribute)__array5[__i5];
                                {
                                    if (attribute != null)
                                    {
                                        category = attribute.Category;
                                        label = attribute.Label;
                                        text = string.Format("{0}/{1}", attribute.Category, attribute.Label);
                                        break;
                                    }
                                }
                            }
                        }
                        if (label != string.Empty)
                        {
                            ContextData userData = new ContextData { Type = type, Label = label, Category = category };
                            createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddEvent), userData);
                        }
                    }
                }
            }
            {
                // foreach(var type in DirectorHelper.GetAllSubTypes(typeof(CinemaGlobalEvent)))
                var __enumerator4 = (DirectorHelper.GetAllSubTypes(typeof(CinemaGlobalEvent))).GetEnumerator();
                while (__enumerator4.MoveNext())
                {
                    var type = (Type)__enumerator4.Current;
                    {
                        string text = string.Empty;
                        string category = string.Empty;
                        string label = string.Empty;
                        {
                            var __array6 = type.GetCustomAttributes(typeof(CutsceneItemAttribute), true);
                            var __arrayLength6 = __array6.Length;
                            for (int __i6 = 0; __i6 < __arrayLength6; ++__i6)
                            {
                                var attribute = (CutsceneItemAttribute)__array6[__i6];
                                {
                                    if (attribute != null)
                                    {
                                        category = attribute.Category;
                                        label = attribute.Label;
                                        text = string.Format("{0}/{1}", attribute.Category, attribute.Label);
                                        break;
                                    }
                                }
                            }
                        }
                        if (label != string.Empty)
                        {
                            ContextData userData = new ContextData { Type = type, Label = label, Category = category };
                            createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddEvent), userData);
                        }
                    }
                }
            }
            createMenu.ShowAsContext();
        }

        eventTrack.ApplyModifiedProperties();
    }

    private void AddEvent(object userData)
    {
        ContextData data = userData as ContextData;
        if (data != null)
        {
            string name = DirectorHelper.getCutsceneItemName(data.Label, data.Type);
            GameObject trackEvent = new GameObject(name);
            trackEvent.AddComponent(data.Type);
            trackEvent.transform.parent = (this.target as GlobalItemTrack).transform;
        }
    }

    private class ContextData
    {
        public Type Type;
        public string Category;
        public string Label;
    }
}
