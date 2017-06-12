// Cinema Suite 2014

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A helper class for getting useful data from Director Runtime objects.
    /// </summary>
    public static class DirectorRuntimeHelper
    {
        /// <summary>
        /// Returns a list of Track types that are associated with the given Track Group.
        /// </summary>
        /// <param name="trackGroup">The track group to be inspected</param>
        /// <returns>A list of track types that meet the genre criteria of the given track group.</returns>
        public static List<Type> GetAllowedTrackTypes(TrackGroup trackGroup)
        {
            // Get all the allowed Genres for this track group
            TimelineTrackGenre[] genres = new TimelineTrackGenre[0];
            MemberInfo info = trackGroup.GetType();
            {
                var __array1 = info.GetCustomAttributes(typeof(TrackGroupAttribute), true);
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var attribute = (TrackGroupAttribute)__array1[__i1];
                    {
                        if (attribute != null)
                        {
                            genres = attribute.AllowedTrackGenres;
                            break;
                        }
                    }
                }
            }
            List<Type> allowedTrackTypes = new List<Type>();
            {
                var __array2 = DirectorRuntimeHelper.GetAllSubTypes(typeof(TimelineTrack));
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var type = (Type)__array2[__i2];
                    {
                        {
                            var __array9 = type.GetCustomAttributes(typeof(TimelineTrackAttribute), true);
                            var __arrayLength9 = __array9.Length;
                            for (int __i9 = 0; __i9 < __arrayLength9; ++__i9)
                            {
                                var attribute = (TimelineTrackAttribute)__array9[__i9];
                                {
                                    if (attribute != null)
                                    {
                                        {
                                            // foreach(var genre in attribute.TrackGenres)
                                            var __enumerator15 = (attribute.TrackGenres).GetEnumerator();
                                            while (__enumerator15.MoveNext())
                                            {
                                                var genre = (TimelineTrackGenre)__enumerator15.Current;
                                                {
                                                    {
                                                        var __array18 = genres;
                                                        var __arrayLength18 = __array18.Length;
                                                        for (int __i18 = 0; __i18 < __arrayLength18; ++__i18)
                                                        {
                                                            var genre2 = (TimelineTrackGenre)__array18[__i18];
                                                            {
                                                                if (genre == genre2)
                                                                {
                                                                    allowedTrackTypes.Add(type);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return allowedTrackTypes;
        }

        /// <summary>
        /// Returns a list of Cutscene Item types that are associated with the given Track.
        /// </summary>
        /// <param name="timelineTrack">The track to look up.</param>
        /// <returns>A list of valid cutscene item types.</returns>
        public static List<Type> GetAllowedItemTypes(TimelineTrack timelineTrack)
        {
            // Get all the allowed Genres for this track
            CutsceneItemGenre[] genres = new CutsceneItemGenre[0];
            MemberInfo info = timelineTrack.GetType();
            {
                var __array3 = info.GetCustomAttributes(typeof(TimelineTrackAttribute), true);
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var attribute = (TimelineTrackAttribute)__array3[__i3];
                    {
                        if (attribute != null)
                        {
                            genres = attribute.AllowedItemGenres;
                            break;
                        }
                    }
                }
            }
            List<Type> allowedItemTypes = new List<Type>();
            {
                var __array4 = DirectorRuntimeHelper.GetAllSubTypes(typeof(TimelineItem));
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var type = (Type)__array4[__i4];
                    {
                        {
                            var __array12 = type.GetCustomAttributes(typeof(CutsceneItemAttribute), true);
                            var __arrayLength12 = __array12.Length;
                            for (int __i12 = 0; __i12 < __arrayLength12; ++__i12)
                            {
                                var attribute = (CutsceneItemAttribute)__array12[__i12];
                                {
                                    if (attribute != null)
                                    {
                                        {
                                            // foreach(var genre in attribute.Genres)
                                            var __enumerator17 = (attribute.Genres).GetEnumerator();
                                            while (__enumerator17.MoveNext())
                                            {
                                                var genre = (CutsceneItemGenre)__enumerator17.Current;
                                                {
                                                    {
                                                        var __array19 = genres;
                                                        var __arrayLength19 = __array19.Length;
                                                        for (int __i19 = 0; __i19 < __arrayLength19; ++__i19)
                                                        {
                                                            var genre2 = (CutsceneItemGenre)__array19[__i19];
                                                            {
                                                                if (genre == genre2)
                                                                {
                                                                    allowedItemTypes.Add(type);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return allowedItemTypes;
        }

        /// <summary>
        /// Get all Sub types from the given parent type.
        /// </summary>
        /// <param name="ParentType">The parent type</param>
        /// <returns>all children types of the parent.</returns>
        private static Type[] GetAllSubTypes(System.Type ParentType)
        {
            List<System.Type> list = new List<System.Type>();
            {
                var __array5 = System.AppDomain.CurrentDomain.GetAssemblies();
                var __arrayLength5 = __array5.Length;
                for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var a = (Assembly)__array5[__i5];
                    {
                        {
                            var __array13 = a.GetTypes();
                            var __arrayLength13 = __array13.Length;
                            for (int __i13 = 0; __i13 < __arrayLength13; ++__i13)
                            {
                                var type = (System.Type)__array13[__i13];
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

        /// <summary>
        /// Retrieve all children of a parent Transform recursively.
        /// </summary>
        /// <param name="parent">The parent transform</param>
        /// <returns>All children of that parent.</returns>
        public static List<Transform> GetAllTransformsInHierarchy(Transform parent)
        {
            List<Transform> children = new List<Transform>();
            {
                // foreach(var child in parent)
                var __enumerator6 = (parent).GetEnumerator();
                while (__enumerator6.MoveNext())
                {
                    var child = (Transform)__enumerator6.Current;
                    {
                        children.AddRange(GetAllTransformsInHierarchy(child));
                        children.Add(child);
                    }
                }
            }
            return children;
        }

    }
}
