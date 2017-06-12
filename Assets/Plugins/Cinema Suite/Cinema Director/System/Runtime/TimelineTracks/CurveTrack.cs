using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// A track designed to hold Actor Curve Clip items.
    /// </summary>
    [TimelineTrackAttribute("Curve Track", TimelineTrackGenre.ActorTrack, CutsceneItemGenre.CurveClipItem)]
    public class CurveTrack : TimelineTrack, IActorTrack
    {
        /// <summary>
        /// Update all curve items.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            base.elapsedTime = time;
            {
                var __array1 = GetTimelineItems();
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var item = (TimelineItem)__array1[__i1];
                    {
                        CinemaActorClipCurve actorClipCurve = item as CinemaActorClipCurve;
                        if (actorClipCurve != null)
                        {
                            actorClipCurve.SampleTime(time);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set the track to an arbitrary time.
        /// </summary>
        /// <param name="time">The new running time.</param>
        public override void SetTime(float time)
        {
            base.elapsedTime = time;
            {
                var __array2 = GetTimelineItems();
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var item = (TimelineItem)__array2[__i2];
                    {
                        CinemaActorClipCurve actorClipCurve = item as CinemaActorClipCurve;
                        if (actorClipCurve != null)
                        {
                            actorClipCurve.SampleTime(time);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stop and reset all the curve data.
        /// </summary>
        public override void Stop()
        {
            {
                var __array3 = GetTimelineItems();
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var item = (TimelineItem)__array3[__i3];
                    {
                        CinemaActorClipCurve actorClipCurve = item as CinemaActorClipCurve;
                        if (actorClipCurve != null)
                        {
                            actorClipCurve.Reset();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the Actor associated with this Curve Track.
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
    }
}