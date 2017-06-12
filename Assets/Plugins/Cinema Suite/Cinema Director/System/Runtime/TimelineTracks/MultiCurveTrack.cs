// Cinema Suite 2014

using UnityEngine;
namespace CinemaDirector
{
    [TimelineTrackAttribute("Curve Track", TimelineTrackGenre.MultiActorTrack, CutsceneItemGenre.MultiActorCurveClipItem)]
    public class MultiCurveTrack : TimelineTrack, IActorTrack
    {

        public override void Initialize()
        {
            {
                var __array1 = this.TimelineItems;
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var clipCurve = (CinemaMultiActorCurveClip)__array1[__i1];
                    {
                        clipCurve.Initialize();
                    }
                }
            }
        }

        public override void UpdateTrack(float time, float deltaTime)
        {
            base.elapsedTime = time;
            {
                var __array2 = this.TimelineItems;
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var clipCurve = (CinemaMultiActorCurveClip)__array2[__i2];
                    {
                        clipCurve.SampleTime(time);
                    }
                }
            }
        }

        public override void Stop()
        {
            {
                var __array3 = this.TimelineItems;
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var clipCurve = (CinemaMultiActorCurveClip)__array3[__i3];
                    {
                        clipCurve.Revert();
                    }
                }
            }
        }

        public override TimelineItem[] TimelineItems
        {
            get
            {
                return GetComponentsInChildren<CinemaMultiActorCurveClip>();
            }
        }

        public Transform Actor
        {
            get
            {
                ActorTrackGroup component = base.transform.parent.GetComponent<ActorTrackGroup>();
                if (component == null)
                {
                    return null;
                }
                return component.Actor;
            }
        }
    }
}