using System;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The main organizational unit of a Cutscene, The TrackGroup contains tracks.
    /// </summary>
    [TrackGroupAttribute("Track Group", TimelineTrackGenre.GlobalTrack)]
    public abstract class TrackGroup : MonoBehaviour, IOptimizable
    {
        [SerializeField]
        private int ordinal = -1; // For ordering in UI

        [SerializeField]
        private bool canOptimize = true; // If true, this Track Group will load all tracks into cache on Optimize().

        // A cache of the tracks for optimization purposes.
        protected TimelineTrack[] trackCache;

        // A list of the types that this Track Group is allowed to contain.
        protected List<Type> allowedTrackTypes;

        private bool hasBeenOptimized = false;

        /// <summary>
        /// Prepares the TrackGroup by caching all TimelineTracks.
        /// </summary>
        public virtual void Optimize()
        {
            if (canOptimize)
            {
                trackCache = GetTracks();
                hasBeenOptimized = true;
            }
            {
                var __array1 = GetTracks();
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var track = (TimelineTrack)__array1[__i1];
                    {
                        track.Optimize();
                    }
                }
            }
        }

        /// <summary>
        /// Initialize all Tracks before beginning a fresh playback.
        /// </summary>
        public virtual void Initialize()
        {
            {
                var __array2 = GetTracks();
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var track = (TimelineTrack)__array2[__i2];
                    {
                        track.Initialize();
                    }
                }
            }
        }

        /// <summary>
        /// Update the track group to the current running time of the cutscene.
        /// </summary>
        /// <param name="time">The current running time</param>
        /// <param name="deltaTime">The deltaTime since the last update call</param>
        public virtual void UpdateTrackGroup(float time, float deltaTime)
        {
            {
                var __array3 = GetTracks();
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var track = (TimelineTrack)__array3[__i3];
                    {
                        track.UpdateTrack(time, deltaTime);
                    }
                }
            }
        }

        /// <summary>
        /// Pause all Track Items that this TrackGroup contains.
        /// </summary>
        public virtual void Pause()
        {
            {
                var __array4 = GetTracks();
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var track = (TimelineTrack)__array4[__i4];
                    {
                        track.Pause();
                    }
                }
            }
        }

        /// <summary>
        /// Stop all Track Items that this TrackGroup contains.
        /// </summary>
        public virtual void Stop()
        {
            {
                var __array5 = GetTracks();
                var __arrayLength5 = __array5.Length;
                for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var track = (TimelineTrack)__array5[__i5];
                    {
                        track.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Resume all Track Items that this TrackGroup contains.
        /// </summary>
        public virtual void Resume()
        {
            {
                var __array6 = GetTracks();
                var __arrayLength6 = __array6.Length;
                for (int __i6 = 0; __i6 < __arrayLength6; ++__i6)
                {
                    var track = (TimelineTrack)__array6[__i6];
                    {
                        track.Resume();
                    }
                }
            }
        }

        /// <summary>
        /// Set this TrackGroup to the state of a given new running time.
        /// </summary>
        /// <param name="time">The new running time</param>
        public virtual void SetRunningTime(float time)
        {
            {
                var __array7 = GetTracks();
                var __arrayLength7 = __array7.Length;
                for (int __i7 = 0; __i7 < __arrayLength7; ++__i7)
                {
                    var track = (TimelineTrack)__array7[__i7];
                    {
                        track.SetTime(time);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieve a list of important times for this track group within the given range.
        /// </summary>
        /// <param name="from">the starting time</param>
        /// <param name="to">the ending time</param>
        /// <returns>A list of ordered milestone times within the given range.</returns>
        public virtual List<float> GetMilestones(float from, float to)
        {
            List<float> times = new List<float>();
            {
                var __array8 = GetTracks();
                var __arrayLength8 = __array8.Length;
                for (int __i8 = 0; __i8 < __arrayLength8; ++__i8)
                {
                    var track = (TimelineTrack)__array8[__i8];
                    {
                        List<float> trackTimes = track.GetMilestones(from, to);
                        {
                            var __list10 = trackTimes;
                            var __listCount10 = __list10.Count;
                            for (int __i10 = 0; __i10 < __listCount10; ++__i10)
                            {
                                var f = (float)__list10[__i10];
                                {
                                    if (!times.Contains(f))
                                    {
                                        times.Add(f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            times.Sort();
            return times;
        }

        /// <summary>
        /// The Cutscene that this TrackGroup is associated with. Will return null if TrackGroup does not have a Cutscene as a parent.
        /// </summary>
        public Cutscene Cutscene
        {
            get
            {
                Cutscene cutscene = null;
                if (transform.parent != null)
                {
                    cutscene = transform.parent.GetComponentInParent<Cutscene>();
                    if (cutscene == null)
                    {
                        Debug.LogError("No Cutscene found on parent!", this);
                    }
                }
                else
                {
                    Debug.LogError("TrackGroup has no parent!", this);
                }
                return cutscene;
            }
        }

        /// <summary>
        /// The TimelineTracks that this TrackGroup contains.
        /// </summary>
        public virtual TimelineTrack[] GetTracks()
        {
            // Return the cache if possible
            if (hasBeenOptimized)
            {
                return trackCache;
            }

            List<TimelineTrack> tracks = new List<TimelineTrack>();
            {
                var __list9 = GetAllowedTrackTypes();
                var __listCount9 = __list9.Count;
                for (int __i9 = 0; __i9 < __listCount9; ++__i9)
                {
                    var t = (Type)__list9[__i9];
                    {
                        var components = GetComponentsInChildren(t);
                        {
                            var __array11 = components;
                            var __arrayLength11 = __array11.Length;
                            for (int __i11 = 0; __i11 < __arrayLength11; ++__i11)
                            {
                                var component = __array11[__i11];
                                {
                                    tracks.Add((TimelineTrack)component);
                                }
                            }
                        }
                    }
                }
            }
            tracks.Sort(
                delegate (TimelineTrack track1, TimelineTrack track2)
            {
                return track1.Ordinal - track2.Ordinal;
            });
            return tracks.ToArray();
        }

        /// <summary>
        /// Provides a list of Types this Track Group is allowed to contain. Loaded by looking at Attributes.
        /// </summary>
        /// <returns>The list of track types.</returns>
        public List<Type> GetAllowedTrackTypes()
        {
            if (allowedTrackTypes == null)
            {
                allowedTrackTypes = DirectorRuntimeHelper.GetAllowedTrackTypes(this);
            }
            return allowedTrackTypes;
        }

        /// <summary>
        /// Ordinal for UI ranking.
        /// </summary>
        public int Ordinal
        {
            get { return ordinal; }
            set { ordinal = value; }
        }

        /// <summary>
        /// Enable this if the TrackGroup does not have Tracks added/removed during running.
        /// </summary>
        public bool CanOptimize
        {
            get { return canOptimize; }
            set { canOptimize = value; }
        }
    }
}