using CinemaDirector;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The default track group control.
/// </summary>
[CutsceneTrackGroupAttribute(typeof(TrackGroup))]
public class GenericTrackGroupControl : TrackGroupControl
{
    /// <summary>
    /// Create and show a context menu for adding new Timeline Tracks.
    /// </summary>
    protected override void addTrackContext()
    {
        TrackGroup trackGroup = TrackGroup.Behaviour as TrackGroup;
        if (trackGroup != null)
        {
            // Get the possible tracks that this group can contain.
            List<Type> trackTypes = trackGroup.GetAllowedTrackTypes();

            GenericMenu createMenu = new GenericMenu();
            {
                var __list1 = trackTypes;
                var __listCount1 = __list1.Count;
                for (int __i1 = 0; __i1 < __listCount1; ++__i1)
                {
                    var t = (Type)__list1[__i1];
                    {
                        MemberInfo info = t;
                        string label = string.Empty;
                        {
                            var __array2 = info.GetCustomAttributes(typeof(TimelineTrackAttribute), true);
                            var __arrayLength2 = __array2.Length;
                            for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                            {
                                var attribute = (TimelineTrackAttribute)__array2[__i2];
                                {
                                    label = attribute.Label;
                                    break;
                                }
                            }
                        }
                        createMenu.AddItem(new GUIContent(string.Format("Add {0}", label)), false, addTrack, new TrackContextData(label, t, trackGroup));
                    }
                }
            }
            createMenu.ShowAsContext();
        }
    }

    /// <summary>
    /// Add a new track
    /// </summary>
    /// <param name="userData">TrackContextData for the track to be created.</param>
    private void addTrack(object userData)
    {
        TrackContextData trackData = userData as TrackContextData;
        if (trackData != null)
        {
            GameObject item = CutsceneItemFactory.CreateTimelineTrack(trackData.TrackGroup, trackData.Type, trackData.Label).gameObject;
            Undo.RegisterCreatedObjectUndo(item, string.Format("Create {0}", item.name));
        }
    }

    /// <summary>
    /// Context data for the track to be created.
    /// </summary>
    private class TrackContextData
    {
        public string Label;
        public Type Type;
        public TrackGroup TrackGroup;

        public TrackContextData(string label, Type type, TrackGroup trackGroup)
        {
            this.Label = label;
            this.Type = type;
            this.TrackGroup = trackGroup;
        }
    }
}
