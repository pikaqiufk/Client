﻿// Cinema Suite
using UnityEditor;
using UnityEngine;
using CinemaDirector;
using System.Reflection;

/// <summary>
/// Audio Track Control
/// </summary>
[CutsceneTrackAttribute(typeof(AudioTrack))]
public class AudioTrackControl : GenericTrackControl
{
    public override void UpdateTrackContents(DirectorControlState state, Rect position)
    {
        handleDragInteraction(position, TargetTrack.Behaviour as AudioTrack, state.Translation, state.Scale);
        base.UpdateTrackContents(state, position);
    }

    private void handleDragInteraction(Rect position, AudioTrack track, Vector2 translation, Vector2 scale)
    {
        Rect controlBackground = new Rect(0, 0, position.width, position.height);
        switch (Event.current.type)
        {
            case EventType.DragUpdated:
                if (controlBackground.Contains(Event.current.mousePosition))
                {
                    bool audioFound = false;
                    {
                        // foreach(var objectReference in DragAndDrop.objectReferences)
                        var __enumerator1 = (DragAndDrop.objectReferences).GetEnumerator();
                        while (__enumerator1.MoveNext())
                        {
                            var objectReference = (Object)__enumerator1.Current;
                            {
                                AudioClip clip = objectReference as AudioClip;
                                if (clip != null)
                                {
                                    audioFound = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (audioFound)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        Event.current.Use();
                    }
                }
                break;
            case EventType.DragPerform:
                if (controlBackground.Contains(Event.current.mousePosition))
                {
                    AudioClip clip = null;
                    {
                        // foreach(var objectReference in DragAndDrop.objectReferences)
                        var __enumerator2 = (DragAndDrop.objectReferences).GetEnumerator();
                        while (__enumerator2.MoveNext())
                        {
                            var objectReference = (Object)__enumerator2.Current;
                            {
                                AudioClip audioClip = objectReference as AudioClip;
                                if (audioClip != null)
                                {
                                    clip = audioClip;
                                    break;
                                }
                            }
                        }
                    }
                    if (clip != null)
                    {
                        float fireTime = (Event.current.mousePosition.x - translation.x) / scale.x;
                        CinemaAudio ca = CutsceneItemFactory.CreateCinemaAudio(track, clip, fireTime);
                        Undo.RegisterCreatedObjectUndo(ca, string.Format("Created {0}", ca.name));
                        Event.current.Use();
                    }
                }
                break;
        }
    }

}
