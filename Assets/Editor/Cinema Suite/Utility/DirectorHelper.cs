using CinemaDirector;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A utility class for cutscene
/// </summary>
public class DirectorHelper
{
    public static CutsceneWrapper UpdateWrapper(Cutscene cutscene, CutsceneWrapper wrapper)
    {
        if (cutscene == null) return null;

        if (wrapper == null || !cutscene.Equals(wrapper.Behaviour))
        {
            return CreateWrapper(cutscene);
        }
        else
        {
            wrapper.Behaviour = cutscene;
            wrapper.Duration = cutscene.Duration;
            wrapper.IsPlaying = cutscene.State == Cutscene.CutsceneState.PreviewPlaying || cutscene.State == Cutscene.CutsceneState.Playing;
            wrapper.RunningTime = cutscene.RunningTime;

            List<Behaviour> itemsToRemove = new List<Behaviour>();
            {
                // foreach(var behaviour in wrapper.Behaviours)
                var __enumerator1 = (wrapper.Behaviours).GetEnumerator();
                while (__enumerator1.MoveNext())
                {
                    var behaviour = (Behaviour)__enumerator1.Current;
                    {
                        bool found = false;
                        {
                            // foreach(var group in cutscene.TrackGroups)
                            var __enumerator14 = (cutscene.TrackGroups).GetEnumerator();
                            while (__enumerator14.MoveNext())
                            {
                                var group = (TrackGroup)__enumerator14.Current;
                                {
                                    if (behaviour.Equals(group))
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!found || behaviour == null)
                        {
                            itemsToRemove.Add(behaviour);
                        }
                    }
                }
            }
            {
                var __list2 = itemsToRemove;
                var __listCount2 = __list2.Count;
                for (int __i2 = 0; __i2 < __listCount2; ++__i2)
                {
                    var trackGroup = (Behaviour)__list2[__i2];
                    {
                        wrapper.HasChanged = true;
                        wrapper.RemoveTrackGroup(trackGroup);
                    }
                }
            }
            {
                var __array3 = cutscene.TrackGroups;
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var tg = (TrackGroup)__array3[__i3];
                    {
                        TrackGroupWrapper tgWrapper = null;
                        if (!wrapper.ContainsTrackGroup(tg, out tgWrapper))
                        {

                            tgWrapper = new TrackGroupWrapper(tg);
                            tgWrapper.Ordinal = tg.Ordinal;
                            wrapper.AddTrackGroup(tg, tgWrapper);
                            wrapper.HasChanged = true;
                        }
                        {
                            // foreach(var track in tg.GetTracks())
                            var __enumerator19 = (tg.GetTracks()).GetEnumerator();
                            while (__enumerator19.MoveNext())
                            {
                                var track = (TimelineTrack)__enumerator19.Current;
                                {
                                    TimelineTrackWrapper trackWrapper = null;
                                    if (!tgWrapper.ContainsTrack(track, out trackWrapper))
                                    {
                                        trackWrapper = new TimelineTrackWrapper(track);
                                        trackWrapper.Ordinal = track.Ordinal;
                                        tgWrapper.AddTrack(track, trackWrapper);
                                        tgWrapper.HasChanged = true;
                                    }
                                    {
                                        // foreach(var item in track.GetTimelineItems())
                                        var __enumerator28 = (track.GetTimelineItems()).GetEnumerator();
                                        while (__enumerator28.MoveNext())
                                        {
                                            var item = (TimelineItem)__enumerator28.Current;
                                            {
                                                TimelineItemWrapper itemWrapper = null;
                                                if (!trackWrapper.ContainsItem(item, out itemWrapper))
                                                {
                                                    if (item.GetType().IsSubclassOf(typeof(CinemaClipCurve)))
                                                    {
                                                        CinemaClipCurve clip = item as CinemaClipCurve;
                                                        itemWrapper = new CinemaClipCurveWrapper(clip, clip.Firetime, clip.Duration);
                                                        trackWrapper.AddItem(clip, itemWrapper);
                                                    }
                                                    else if (item.GetType().IsSubclassOf(typeof(CinemaTween)))
                                                    {
                                                        CinemaTween tween = item as CinemaTween;
                                                        itemWrapper = new CinemaTweenWrapper(tween, tween.Firetime, tween.Duration);
                                                        trackWrapper.AddItem(tween, itemWrapper);
                                                    }
                                                    else if (item.GetType().IsSubclassOf(typeof(TimelineActionFixed)))
                                                    {
                                                        TimelineActionFixed fixedAction = item as TimelineActionFixed;
                                                        itemWrapper = new CinemaActionFixedWrapper(fixedAction, fixedAction.Firetime, fixedAction.Duration, fixedAction.InTime, fixedAction.OutTime, fixedAction.ItemLength);
                                                        trackWrapper.AddItem(fixedAction, itemWrapper);
                                                    }
                                                    else if (item.GetType().IsSubclassOf(typeof(TimelineAction)))
                                                    {
                                                        TimelineAction action = item as TimelineAction;
                                                        itemWrapper = new CinemaActionWrapper(action, action.Firetime, action.Duration);
                                                        trackWrapper.AddItem(action, itemWrapper);
                                                    }
                                                    else
                                                    {
                                                        itemWrapper = new TimelineItemWrapper(item, item.Firetime);
                                                        trackWrapper.AddItem(item, itemWrapper);
                                                    }
                                                    trackWrapper.HasChanged = true;
                                                }
                                                else
                                                {
                                                    if (GUIUtility.hotControl == 0)
                                                    {
                                                        if (itemWrapper.GetType() == (typeof(CinemaClipCurveWrapper)))
                                                        {
                                                            CinemaClipCurve clip = item as CinemaClipCurve;
                                                            CinemaClipCurveWrapper clipWrapper = itemWrapper as CinemaClipCurveWrapper;
                                                            clipWrapper.Firetime = clip.Firetime;
                                                            clipWrapper.Duration = clip.Duration;
                                                        }
                                                        else if (itemWrapper.GetType() == (typeof(CinemaTweenWrapper)))
                                                        {
                                                        }
                                                        else if (itemWrapper.GetType() == (typeof(CinemaActionFixedWrapper)))
                                                        {
                                                            TimelineActionFixed actionFixed = item as TimelineActionFixed;
                                                            CinemaActionFixedWrapper actionFixedWrapper = itemWrapper as CinemaActionFixedWrapper;
                                                            actionFixedWrapper.Firetime = actionFixed.Firetime;
                                                            actionFixedWrapper.Duration = actionFixed.Duration;
                                                            actionFixedWrapper.InTime = actionFixed.InTime;
                                                            actionFixedWrapper.OutTime = actionFixed.OutTime;
                                                            actionFixedWrapper.ItemLength = actionFixed.ItemLength;
                                                        }
                                                        else if (itemWrapper.GetType() == (typeof(CinemaActionWrapper)))
                                                        {
                                                            TimelineAction action = item as TimelineAction;
                                                            CinemaActionWrapper actionWrapper = itemWrapper as CinemaActionWrapper;
                                                            actionWrapper.Firetime = action.Firetime;
                                                            actionWrapper.Duration = action.Duration;
                                                        }
                                                        else
                                                        {
                                                            itemWrapper.Firetime = item.Firetime;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Remove missing track items
                                    List<Behaviour> itemRemovals = new List<Behaviour>();
                                    {
                                        // foreach(var behaviour in trackWrapper.Behaviours)
                                        var __enumerator30 = (trackWrapper.Behaviours).GetEnumerator();
                                        while (__enumerator30.MoveNext())
                                        {
                                            var behaviour = (Behaviour)__enumerator30.Current;
                                            {
                                                bool found = false;
                                                {
                                                    var __array34 = track.GetTimelineItems();
                                                    var __arrayLength34 = __array34.Length;
                                                    for (int __i34 = 0; __i34 < __arrayLength34; ++__i34)
                                                    {
                                                        var item = (TimelineItem)__array34[__i34];
                                                        {
                                                            if (behaviour.Equals(item))
                                                            {
                                                                found = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (!found || behaviour == null)
                                                {
                                                    itemRemovals.Add(behaviour);
                                                }
                                            }
                                        }
                                    }
                                    {
                                        var __list31 = itemRemovals;
                                        var __listCount31 = __list31.Count;
                                        for (int __i31 = 0; __i31 < __listCount31; ++__i31)
                                        {
                                            var item = (Behaviour)__list31[__i31];
                                            {
                                                trackWrapper.HasChanged = true;
                                                trackWrapper.RemoveItem(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Remove missing tracks
                        List<Behaviour> removals = new List<Behaviour>();
                        {
                            // foreach(var behaviour in tgWrapper.Behaviours)
                            var __enumerator21 = (tgWrapper.Behaviours).GetEnumerator();
                            while (__enumerator21.MoveNext())
                            {
                                var behaviour = (Behaviour)__enumerator21.Current;
                                {
                                    bool found = false;
                                    {
                                        // foreach(var track in tg.GetTracks())
                                        var __enumerator32 = (tg.GetTracks()).GetEnumerator();
                                        while (__enumerator32.MoveNext())
                                        {
                                            var track = (TimelineTrack)__enumerator32.Current;
                                            {
                                                if (behaviour.Equals(track))
                                                {
                                                    found = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    if (!found || behaviour == null)
                                    {
                                        removals.Add(behaviour);
                                    }
                                }
                            }
                        }
                        {
                            var __list22 = removals;
                            var __listCount22 = __list22.Count;
                            for (int __i22 = 0; __i22 < __listCount22; ++__i22)
                            {
                                var track = (Behaviour)__list22[__i22];
                                {
                                    tgWrapper.HasChanged = true;
                                    tgWrapper.RemoveTrack(track);
                                }
                            }
                        }
                    }
                }
            }
        }

        return wrapper;
    }

    public static void ReflectChanges(Cutscene cutscene, CutsceneWrapper wrapper)
    {
        if (cutscene == null || wrapper == null) return;

        cutscene.Duration = wrapper.Duration;
        {
            // foreach(var tgw in wrapper.TrackGroups)
            var __enumerator4 = (wrapper.TrackGroups).GetEnumerator();
            while (__enumerator4.MoveNext())
            {
                var tgw = (TrackGroupWrapper)__enumerator4.Current;
                {
                    TrackGroup tg = tgw.Behaviour as TrackGroup;
                    tg.Ordinal = tgw.Ordinal;
                    {
                        // foreach(var trackWrapper in tgw.Tracks)
                        var __enumerator23 = (tgw.Tracks).GetEnumerator();
                        while (__enumerator23.MoveNext())
                        {
                            var trackWrapper = (TimelineTrackWrapper)__enumerator23.Current;
                            {
                                TimelineTrack track = trackWrapper.Behaviour as TimelineTrack;
                                track.Ordinal = trackWrapper.Ordinal;
                            }
                        }
                    }
                }
            }
        }
    }

    public static CutsceneWrapper CreateWrapper(Cutscene cutscene)
    {
        CutsceneWrapper wrapper = new CutsceneWrapper(cutscene);
        if (cutscene != null)
        {
            wrapper.RunningTime = cutscene.RunningTime;
            wrapper.Duration = cutscene.Duration;
            wrapper.IsPlaying = cutscene.State == Cutscene.CutsceneState.PreviewPlaying || cutscene.State == Cutscene.CutsceneState.Playing;
            {
                var __array5 = cutscene.TrackGroups;
                var __arrayLength5 = __array5.Length;
                for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var tg = (TrackGroup)__array5[__i5];
                    {
                        TrackGroupWrapper tgWrapper = new TrackGroupWrapper(tg);
                        tgWrapper.Ordinal = tg.Ordinal;
                        wrapper.AddTrackGroup(tg, tgWrapper);
                        {
                            // foreach(var track in tg.GetTracks())
                            var __enumerator25 = (tg.GetTracks()).GetEnumerator();
                            while (__enumerator25.MoveNext())
                            {
                                var track = (TimelineTrack)__enumerator25.Current;
                                {
                                    TimelineTrackWrapper trackWrapper = new TimelineTrackWrapper(track);
                                    trackWrapper.Ordinal = track.Ordinal;
                                    tgWrapper.AddTrack(track, trackWrapper);
                                    {
                                        // foreach(var item in track.GetTimelineItems())
                                        var __enumerator33 = (track.GetTimelineItems()).GetEnumerator();
                                        while (__enumerator33.MoveNext())
                                        {
                                            var item = (TimelineItem)__enumerator33.Current;
                                            {
                                                if (item.GetType().IsSubclassOf(typeof(CinemaClipCurve)))
                                                {
                                                    CinemaClipCurve clip = item as CinemaClipCurve;
                                                    CinemaClipCurveWrapper clipWrapper = new CinemaClipCurveWrapper(clip, clip.Firetime, clip.Duration);
                                                    trackWrapper.AddItem(clip, clipWrapper);
                                                }
                                                else if (item.GetType().IsSubclassOf(typeof(CinemaTween)))
                                                {
                                                }
                                                else if (item.GetType().IsSubclassOf(typeof(TimelineActionFixed)))
                                                {
                                                    TimelineActionFixed actionFixed = item as TimelineActionFixed;
                                                    CinemaActionFixedWrapper actionFixedWrapper = new CinemaActionFixedWrapper(actionFixed, actionFixed.Firetime, actionFixed.Duration, actionFixed.InTime, actionFixed.OutTime, actionFixed.ItemLength);
                                                    trackWrapper.AddItem(actionFixed, actionFixedWrapper);
                                                }
                                                else if (item.GetType().IsSubclassOf(typeof(TimelineAction)))
                                                {
                                                    TimelineAction action = item as TimelineAction;
                                                    CinemaActionWrapper itemWrapper = new CinemaActionWrapper(action, action.Firetime, action.Duration);
                                                    trackWrapper.AddItem(action, itemWrapper);
                                                }
                                                else
                                                {
                                                    TimelineItemWrapper itemWrapper = new TimelineItemWrapper(item, item.Firetime);
                                                    trackWrapper.AddItem(item, itemWrapper);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return wrapper;
    }

    public static System.Type[] GetAllSubTypes(System.Type ParentType)
    {
        List<System.Type> list = new List<System.Type>();
        {
            var __array6 = System.AppDomain.CurrentDomain.GetAssemblies();
            var __arrayLength6 = __array6.Length;
            for (int __i6 = 0; __i6 < __arrayLength6; ++__i6)
            {
                var a = (Assembly)__array6[__i6];
                {
                    {
                        var __array26 = a.GetTypes();
                        var __arrayLength26 = __array26.Length;
                        for (int __i26 = 0; __i26 < __arrayLength26; ++__i26)
                        {
                            var type = (System.Type)__array26[__i26];
                            {
                                if (type.IsSubclassOf(ParentType))
                                {
                                    list.Add(type);
                                }
                            }
                        }
                    }
                }
            }
        }
        return list.ToArray();
    }

    public enum TimeEnum
    {
        Minutes = 60,
        Seconds = 1
    }

    /// <summary>
    /// Determines if a Cutscene exists with a given name. 
    /// This is to avoid cutscenes with the same name, but is not enforced by Unity.
    /// </summary>
    /// <param name="name">The name to check</param>
    /// <returns>true if the name exists</returns>
    public static bool isCutsceneNameDuplicate(string name)
    {
        bool isDuplicate = false;
        Object[] cutscenes = Object.FindObjectsOfType(typeof(Cutscene));
        {
            var __array7 = cutscenes;
            var __arrayLength7 = __array7.Length;
            for (int __i7 = 0; __i7 < __arrayLength7; ++__i7)
            {
                var obj = (Object)__array7[__i7];
                {
                    if (name == obj.name)
                    {
                        isDuplicate = true;
                    }
                }
            }
        }
        return isDuplicate;
    }

    public static string getCutsceneItemName(GameObject parent, string name, System.Type type)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        return getCutsceneItemName(children, name, type, 0);
    }

    public static string getCutsceneItemName(string name, System.Type type)
    {
        return getCutsceneItemName(name, type, 0);
    }

    private static string getCutsceneItemName(string name, System.Type type, int iteration)
    {
        string newName = name;
        if (iteration > 0)
        {
            newName = string.Format("{0} {1}", name, iteration);
        }
        bool isDuplicate = false;
        Object[] items = Object.FindObjectsOfType(type);
        {
            var __array8 = items;
            var __arrayLength8 = __array8.Length;
            for (int __i8 = 0; __i8 < __arrayLength8; ++__i8)
            {
                var obj = (Object)__array8[__i8];
                {
                    if (newName == obj.name)
                    {
                        isDuplicate = true;
                    }
                }
            }
        }
        if (isDuplicate)
        {
            return getCutsceneItemName(name, type, ++iteration);
        }
        return newName;
    }

    private static string getCutsceneItemName(Transform[] children, string name, System.Type type, int iteration)
    {
        string newName = name;
        if (iteration > 0)
        {
            newName = string.Format("{0} {1}", name, iteration);
        }
        bool isDuplicate = false;
        {
            var __array9 = children;
            var __arrayLength9 = __array9.Length;
            for (int __i9 = 0; __i9 < __arrayLength9; ++__i9)
            {
                var obj = (Object)__array9[__i9];
                {
                    if (newName == obj.name)
                    {
                        isDuplicate = true;
                    }
                }
            }
        }
        if (isDuplicate)
        {
            return getCutsceneItemName(children, name, type, ++iteration);
        }
        return newName;
    }

    public static Component[] getValidComponents(GameObject actor)
    {
        Component[] components = actor.GetComponents<Component>();
        return components;
    }

    public static Component[] getEnableableComponents(GameObject actor)
    {
        List<Component> behaviours = new List<Component>();
        {
            var __array10 = actor.GetComponents<Component>();
            var __arrayLength10 = __array10.Length;
            for (int __i10 = 0; __i10 < __arrayLength10; ++__i10)
            {
                var c = (Component)__array10[__i10];
                {
                    {
                        var __array27 = c.GetType().GetMember("enabled");
                        var __arrayLength27 = __array27.Length;
                        for (int __i27 = 0; __i27 < __arrayLength27; ++__i27)
                        {
                            var field = (MemberInfo)__array27[__i27];
                            {
                                if (field.Name == "enabled")
                                {
                                    behaviours.Add(c);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
        }
        return behaviours.ToArray();
    }

    public static PropertyInfo[] getValidProperties(Component component)
    {
        List<PropertyInfo> properties = new List<PropertyInfo>();
        {
            var __array11 = component.GetType().GetProperties();
            var __arrayLength11 = __array11.Length;
            for (int __i11 = 0; __i11 < __arrayLength11; ++__i11)
            {
                var propertyInfo = (PropertyInfo)__array11[__i11];
                {
                    if (UnityPropertyTypeInfo.GetMappedType(propertyInfo.PropertyType) != PropertyTypeInfo.None && propertyInfo.CanWrite)
                    {
                        properties.Add(propertyInfo);
                    }
                }
            }
        }
        return properties.ToArray();
    }

    public static FieldInfo[] getValidFields(Component component)
    {
        List<FieldInfo> fields = new List<FieldInfo>();
        {
            var __array12 = component.GetType().GetFields();
            var __arrayLength12 = __array12.Length;
            for (int __i12 = 0; __i12 < __arrayLength12; ++__i12)
            {
                var field = (FieldInfo)__array12[__i12];
                {
                    if (UnityPropertyTypeInfo.GetMappedType(field.FieldType) != PropertyTypeInfo.None)
                    {
                        fields.Add(field);
                    }
                }
            }
        }
        return fields.ToArray();
    }

    public static MemberInfo[] getValidMembers(Component component)
    {
        PropertyInfo[] properties = getValidProperties(component);
        FieldInfo[] fields = getValidFields(component);

        List<MemberInfo> members = new List<MemberInfo>();
        if (component.GetType() == typeof(Transform))
        {
            {
                var __array13 = properties;
                var __arrayLength13 = __array13.Length;
                for (int __i13 = 0; __i13 < __arrayLength13; ++__i13)
                {
                    var propertyInfo = (PropertyInfo)__array13[__i13];
                    {
                        if (propertyInfo.Name == "localPosition" || propertyInfo.Name == "localEulerAngles" || propertyInfo.Name == "localScale")
                        {
                            members.Add(propertyInfo);
                        }
                    }
                }
            }
        }
        else
        {
            members.AddRange(properties);
            members.AddRange(fields);
        }
        return members.ToArray();
    }

    public static string GetUserFriendlyName(Component component, MemberInfo memberInfo)
    {
        return GetUserFriendlyName(component.GetType().Name, memberInfo.Name);
    }

    public static string GetUserFriendlyName(string componentName, string memberName)
    {
        string name = memberName;
        if (componentName == "Transform")
        {
            if (memberName == "localPosition")
            {
                name = "Position";
            }
            else if (memberName == "localEulerAngles")
            {
                name = "Rotation";
            }
            else if (memberName == "localScale")
            {
                name = "Scale";
            }
        }
        return name;
    }

    public static bool IsTrackItemValidForTrack(Behaviour behaviour, TimelineTrack track)
    {
        bool retVal = false;
        if (track.GetType() == (typeof(ShotTrack)))
        {
            if (behaviour.GetType() == (typeof(CinemaShot)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(AudioTrack)))
        {
            if (behaviour.GetType() == (typeof(CinemaAudio)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(GlobalItemTrack)) || track.GetType().IsSubclassOf(typeof(GlobalItemTrack)))
        {
            if (behaviour.GetType() == typeof(CinemaShot) || (behaviour.GetType().IsSubclassOf(typeof(CinemaAudio))))
            {
                retVal = false;
            }
            else if (behaviour.GetType().IsSubclassOf(typeof(CinemaGlobalAction)) || behaviour.GetType().IsSubclassOf(typeof(CinemaGlobalEvent)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(ActorItemTrack)) || track.GetType().IsSubclassOf(typeof(ActorItemTrack)))
        {
            if (behaviour.GetType().IsSubclassOf(typeof(CinemaActorAction)) || behaviour.GetType().IsSubclassOf(typeof(CinemaActorEvent)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(CurveTrack)) || track.GetType().IsSubclassOf(typeof(CurveTrack)))
        {
            if (behaviour.GetType() == (typeof(CinemaActorClipCurve)))
            {
                retVal = true;
            }
        }
        else if (track.GetType() == (typeof(MultiCurveTrack)) || track.GetType().IsSubclassOf(typeof(MultiCurveTrack)))
        {
            if (behaviour.GetType() == (typeof(CinemaMultiActorCurveClip)))
            {
                retVal = true;
            }
        }
        return retVal;
    }
}
