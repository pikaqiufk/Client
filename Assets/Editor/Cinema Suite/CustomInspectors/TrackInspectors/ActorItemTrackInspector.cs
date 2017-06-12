using System;
using UnityEditor;
using UnityEngine;
using CinemaDirector;

[CustomEditor(typeof(ActorItemTrack))]
public class ActorItemTrackInspector : Editor
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
        ActorItemTrack track = base.serializedObject.targetObject as ActorItemTrack;
        CinemaActorAction[] actions = track.ActorActions;
        CinemaActorEvent[] actorEvents = track.ActorEvents;

        if (actions.Length > 0 || actorEvents.Length > 0)
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
                        var action = (CinemaActorAction)__array1[__i1];
                        {
                            EditorGUILayout.ObjectField(action.name, action, typeof(CinemaActorAction), true);
                        }
                    }
                }
                {
                    var __array2 = actorEvents;
                    var __arrayLength2 = __array2.Length;
                    for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var actorEvent = (CinemaActorEvent)__array2[__i2];
                        {
                            EditorGUILayout.ObjectField(actorEvent.name, actorEvent, typeof(CinemaActorEvent), true);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
        }

        if (GUILayout.Button(addActionContent))
        {
            GenericMenu createMenu = new GenericMenu();

            Type actorActionType = typeof(CinemaActorAction);
            {
                // foreach(var type in DirectorHelper.GetAllSubTypes(actorActionType))
                var __enumerator3 = (DirectorHelper.GetAllSubTypes(actorActionType)).GetEnumerator();
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
                                        text = string.Format("{0}/{1}", category, label);
                                        break;
                                    }
                                }
                            }
                        }
                        ContextData userData = new ContextData { type = type, label = label, category = category };
                        createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddEvent), userData);
                    }
                }
            }

            Type actorEventType = typeof(CinemaActorEvent);
            {
                // foreach(var type in DirectorHelper.GetAllSubTypes(actorEventType))
                var __enumerator4 = (DirectorHelper.GetAllSubTypes(actorEventType)).GetEnumerator();
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
                        ContextData userData = new ContextData { type = type, label = label, category = category };
                        createMenu.AddItem(new GUIContent(text), false, new GenericMenu.MenuFunction2(AddEvent), userData);
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
            GameObject trackEvent = new GameObject(data.label);
            trackEvent.AddComponent(data.type);
            trackEvent.transform.parent = (this.target as ActorItemTrack).transform;
        }
    }

    private class ContextData
    {
        public Type type;
        public string category;
        public string label;
    }
}



