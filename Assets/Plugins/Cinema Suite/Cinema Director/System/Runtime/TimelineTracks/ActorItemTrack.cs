using System;
// Cinema Suite
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A track which maintains all timeline items marked for actor tracks and multi actor tracks.
    /// </summary>
    [TimelineTrackAttribute("Actor Track", new TimelineTrackGenre[] { TimelineTrackGenre.ActorTrack, TimelineTrackGenre.MultiActorTrack }, CutsceneItemGenre.ActorItem)]
    public class ActorItemTrack : TimelineTrack, IActorTrack, IMultiActorTrack
    {
        /// <summary>
        /// Initialize this Track and all the timeline items contained within.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            {
                var __array1 = this.ActorEvents;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var cinemaEvent = (CinemaActorEvent)__array1[__i1];
                    {
                        {
                            var __list6 = Actors;
                            var __listCount6 = __list6.Count;
                            for (int __i6 = 0; __i6 < __listCount6; ++__i6)
                            {
                                var actor = (Transform)__list6[__i6];
                                {
                                    if (actor != null)
                                    {
                                        cinemaEvent.Initialize(actor.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The cutscene has been set to an arbitrary time by the user.
        /// Processing must take place to catch up to the new time.
        /// </summary>
        /// <param name="time">The new cutscene running time</param>
        public override void SetTime(float time)
        {
            float previousTime = elapsedTime;
            base.SetTime(time);
            {
                var __array2 = GetTimelineItems();
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var item = (TimelineItem)__array2[__i2];
                    {
                        // Check if it is an actor event.
                        CinemaActorEvent cinemaEvent = item as CinemaActorEvent;
                        if (cinemaEvent != null)
                        {
                            if ((previousTime < cinemaEvent.Firetime) && (((time >= cinemaEvent.Firetime))))
                            {
                                {
                                    var __list7 = Actors;
                                    var __listCount7 = __list7.Count;
                                    for (int __i7 = 0; __i7 < __listCount7; ++__i7)
                                    {
                                        var actor = (Transform)__list7[__i7];
                                        {
                                            if (actor != null)
                                            {
                                                cinemaEvent.Trigger(actor.gameObject);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (((previousTime >= cinemaEvent.Firetime) && (time < cinemaEvent.Firetime)))
                            {
                                {
                                    var __list8 = Actors;
                                    var __listCount8 = __list8.Count;
                                    for (int __i8 = 0; __i8 < __listCount8; ++__i8)
                                    {
                                        var actor = (Transform)__list8[__i8];
                                        {
                                            if (actor != null)
                                                cinemaEvent.Reverse(actor.gameObject);
                                        }
                                    }
                                }
                            }
                        }

                        // Check if it is an actor action.
                        CinemaActorAction action = item as CinemaActorAction;
                        if (action != null)
                        {
                            {
                                var __list9 = Actors;
                                var __listCount9 = __list9.Count;
                                for (int __i9 = 0; __i9 < __listCount9; ++__i9)
                                {
                                    var actor = (Transform)__list9[__i9];
                                    {
                                        if (actor != null)
                                            action.SetTime(actor.gameObject, (time - action.Firetime), time - previousTime);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update this track since the last frame.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float previousTime = base.elapsedTime;
            base.UpdateTrack(time, deltaTime);
            {
                var __array3 = GetTimelineItems();
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var item = (TimelineItem)__array3[__i3];
                    {
                        // Check if it is an actor event.
                        CinemaActorEvent cinemaEvent = item as CinemaActorEvent;
                        if (cinemaEvent != null)
                        {
                            if ((previousTime < cinemaEvent.Firetime) && (((base.elapsedTime >= cinemaEvent.Firetime))))
                            {
                                {
                                    var __list10 = Actors;
                                    var __listCount10 = __list10.Count;
                                    for (int __i10 = 0; __i10 < __listCount10; ++__i10)
                                    {
                                        var actor = (Transform)__list10[__i10];
                                        {
                                            if (actor != null)
                                                cinemaEvent.Trigger(actor.gameObject);
                                        }
                                    }
                                }
                            }
                            if (((previousTime >= cinemaEvent.Firetime) && (base.elapsedTime < cinemaEvent.Firetime)))
                            {
                                {
                                    var __list11 = Actors;
                                    var __listCount11 = __list11.Count;
                                    for (int __i11 = 0; __i11 < __listCount11; ++__i11)
                                    {
                                        var actor = (Transform)__list11[__i11];
                                        {
                                            if (actor != null)
                                                cinemaEvent.Reverse(actor.gameObject);
                                        }
                                    }
                                }
                            }
                        }

                        CinemaActorAction action = item as CinemaActorAction;
                        if (action != null)
                        {
                            if ((previousTime < action.Firetime && base.elapsedTime >= action.Firetime) && base.elapsedTime < action.EndTime)
                            {
                                {
                                    var __list12 = Actors;
                                    var __listCount12 = __list12.Count;
                                    for (int __i12 = 0; __i12 < __listCount12; ++__i12)
                                    {
                                        var actor = (Transform)__list12[__i12];
                                        {
                                            if (actor != null)
                                            {
                                                action.Trigger(actor.gameObject);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (previousTime <= action.EndTime && base.elapsedTime > action.EndTime)
                            {
                                {
                                    var __list13 = Actors;
                                    var __listCount13 = __list13.Count;
                                    for (int __i13 = 0; __i13 < __listCount13; ++__i13)
                                    {
                                        var actor = (Transform)__list13[__i13];
                                        {
                                            if (actor != null)
                                            {
                                                action.End(actor.gameObject);
                                            }
                                        }
                                    }
                                }
                            }
                            else if (previousTime >= action.Firetime && previousTime < action.EndTime && base.elapsedTime < action.Firetime)
                            {
                                {
                                    var __list14 = Actors;
                                    var __listCount14 = __list14.Count;
                                    for (int __i14 = 0; __i14 < __listCount14; ++__i14)
                                    {
                                        var actor = (Transform)__list14[__i14];
                                        {
                                            if (actor != null)
                                            {
                                                action.ReverseTrigger(actor.gameObject);
                                            }
                                        }
                                    }
                                }
                            }
                            else if ((previousTime > action.EndTime && (base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime)))
                            {
                                {
                                    var __list15 = Actors;
                                    var __listCount15 = __list15.Count;
                                    for (int __i15 = 0; __i15 < __listCount15; ++__i15)
                                    {
                                        var actor = (Transform)__list15[__i15];
                                        {
                                            if (actor != null)
                                            {
                                                action.ReverseEnd(actor.gameObject);
                                            }
                                        }
                                    }
                                }
                            }
                            else if ((base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime))
                            {
                                {
                                    var __list16 = Actors;
                                    var __listCount16 = __list16.Count;
                                    for (int __i16 = 0; __i16 < __listCount16; ++__i16)
                                    {
                                        var actor = (Transform)__list16[__i16];
                                        {
                                            if (actor != null)
                                            {
                                                float runningTime = time - action.Firetime;
                                                action.UpdateTime(actor.gameObject, runningTime, deltaTime);
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

        /// <summary>
        /// Resume playback after being paused.
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            {
                var __array4 = GetTimelineItems();
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var item = (TimelineItem)__array4[__i4];
                    {
                        CinemaActorAction action = item as CinemaActorAction;
                        if (action != null)
                        {
                            if (((elapsedTime > action.Firetime)) && (elapsedTime < (action.Firetime + action.Duration)))
                            {
                                {
                                    var __list17 = Actors;
                                    var __listCount17 = __list17.Count;
                                    for (int __i17 = 0; __i17 < __listCount17; ++__i17)
                                    {
                                        var actor = (Transform)__list17[__i17];
                                        {
                                            if (actor != null)
                                            {
                                                action.Resume(actor.gameObject);
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

        /// <summary>
        /// Stop the playback of this track.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            base.elapsedTime = 0f;
            {
                var __array5 = GetTimelineItems();
                var __arrayLength5 = __array5.Length;
                for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var item = (TimelineItem)__array5[__i5];
                    {
                        CinemaActorEvent cinemaEvent = item as CinemaActorEvent;
                        if (cinemaEvent != null)
                        {
                            {
                                var __list18 = Actors;
                                var __listCount18 = __list18.Count;
                                for (int __i18 = 0; __i18 < __listCount18; ++__i18)
                                {
                                    var actor = (Transform)__list18[__i18];
                                    {
                                        if (actor != null)
                                            cinemaEvent.Stop(actor.gameObject);
                                    }
                                }
                            }
                        }

                        CinemaActorAction action = item as CinemaActorAction;
                        if (action != null)
                        {
                            {
                                var __list19 = Actors;
                                var __listCount19 = __list19.Count;
                                for (int __i19 = 0; __i19 < __listCount19; ++__i19)
                                {
                                    var actor = (Transform)__list19[__i19];
                                    {
                                        if (actor != null)
                                            action.Stop(actor.gameObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the Actor associated with this track. Can return null.
        /// </summary>
        public Transform Actor
        {
            get
            {
                ActorTrackGroup atg = this.TrackGroup as ActorTrackGroup;
                if (atg == null)
                {
                    Debug.LogError("No ActorTrackGroup found on parent.", this);
                    return null;
                }
                return atg.Actor;
            }
        }

        /// <summary>
        /// Get the Actors associated with this track. Can return null.
        /// In the case of MultiActors it will return the full list.
        /// </summary>
        public List<Transform> Actors
        {
            get
            {
                ActorTrackGroup trackGroup = TrackGroup as ActorTrackGroup;
                if (trackGroup != null)
                {
                    List<Transform> actors = new List<Transform>() { };
                    actors.Add(trackGroup.Actor);
                    return actors;
                }

                MultiActorTrackGroup multiActorTrackGroup = TrackGroup as MultiActorTrackGroup;
                if (multiActorTrackGroup != null)
                {
                    return multiActorTrackGroup.Actors;
                }
                return null;
            }
        }

        public CinemaActorEvent[] ActorEvents
        {
            get
            {
                return base.GetComponentsInChildren<CinemaActorEvent>();
            }
        }

        public CinemaActorAction[] ActorActions
        {
            get
            {
                return base.GetComponentsInChildren<CinemaActorAction>();
            }
        }
    }
}