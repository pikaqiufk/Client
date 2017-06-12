// Cinema Suite
using System;
using UnityEngine;

namespace CinemaDirector
{
    public delegate void ShotBeginsEventHandler(object sender, ShotEventArgs e);
    public delegate void ShotEndsEventHandler(object sender, ShotEventArgs e);

    public class ShotEventArgs : EventArgs
    {
        public CinemaShot shot;

        public ShotEventArgs(CinemaShot shot)
        {
            this.shot = shot;
        }
    }

    /// <summary>
    /// A track that sorts shots and manages associated cameras.
    /// </summary>
    [TimelineTrackAttribute("Shot Track", TimelineTrackGenre.GlobalTrack, CutsceneItemGenre.CameraShot)]
    public class ShotTrack : TimelineTrack
    {
        public event ShotEndsEventHandler ShotEnds;
        public event ShotBeginsEventHandler ShotBegins;

        /// <summary>
        /// Initialize the shot track by enabling the first Camera and disabling all others in the track.
        /// </summary>
        public override void Initialize()
        {
            base.elapsedTime = 0f;

            CinemaShot firstCamera = null;
            {
                var __array1 = GetTimelineItems();
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var shot = (CinemaShot)__array1[__i1];
                    {
                        shot.Initialize();
                    }
                }
            }
            {
                var __array2 = GetTimelineItems();
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var shot = (CinemaShot)__array2[__i2];
                    {
                        if (shot.Firetime == 0)
                        {
                            firstCamera = shot;
                        }
                        else
                        {
                            shot.End();
                        }
                    }
                }
            }
            if (firstCamera != null)
            {
                firstCamera.Trigger();
                if (ShotBegins != null)
                {
                    ShotBegins(this, new ShotEventArgs(firstCamera));
                }
            }
        }

        /// <summary>
        /// Update the Shot Track by deltaTime. Will fire ShotBegins and ShotEnds events.
        /// </summary>
        /// <param name="time">The current running time.</param>
        /// <param name="deltaTime">The deltaTime since the last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float previousTime = base.elapsedTime;
            base.elapsedTime = time;
            {
                var __array3 = GetTimelineItems();
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var shot = (CinemaShot)__array3[__i3];
                    {
                        float endTime = shot.CutTime + shot.Duration;
                        if ((previousTime <= shot.CutTime) && (base.elapsedTime >= shot.CutTime) && (base.elapsedTime < endTime))
                        {
                            shot.Trigger();
                            if (ShotBegins != null)
                            {
                                ShotBegins(this, new ShotEventArgs(shot));
                            }
                        }
                        else if ((previousTime >= endTime) && (base.elapsedTime < endTime) && (base.elapsedTime >= shot.CutTime))
                        {
                            shot.Trigger();
                            if (ShotBegins != null)
                            {
                                ShotBegins(this, new ShotEventArgs(shot));
                            }
                        }
                        else if ((previousTime >= shot.CutTime) && (previousTime < endTime) && (base.elapsedTime >= endTime))
                        {
                            shot.End();
                            if (ShotEnds != null)
                            {
                                ShotEnds(this, new ShotEventArgs(shot));
                            }
                        }
                        else if ((previousTime > shot.CutTime) && (previousTime < endTime) && (base.elapsedTime < shot.CutTime))
                        {
                            shot.End();
                            if (ShotEnds != null)
                            {
                                ShotEnds(this, new ShotEventArgs(shot));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The shot track will jump to the given time. Disabling the current shot and enabling the new one.
        /// </summary>
        /// <param name="time">The new running time.</param>
        public override void SetTime(float time)
        {
            CinemaShot previousShot = null;
            CinemaShot newShot = null;
            {
                var __array4 = GetTimelineItems();
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var shot = (CinemaShot)__array4[__i4];
                    {
                        float endTime = shot.CutTime + shot.Duration;
                        if ((elapsedTime >= shot.CutTime) && (elapsedTime < endTime))
                        {
                            previousShot = shot;
                        }
                        if ((time >= shot.CutTime) && (time < endTime))
                        {
                            newShot = shot;
                        }
                    }
                }
            }
            // Trigger them as appropriate.
            if (newShot != previousShot)
            {
                if (previousShot != null)
                {
                    previousShot.End();
                    if (ShotEnds != null)
                    {
                        ShotEnds(this, new ShotEventArgs(previousShot));
                    }
                }
                if (newShot != null)
                {
                    newShot.Trigger();
                    if (ShotBegins != null)
                    {
                        ShotBegins(this, new ShotEventArgs(newShot));
                    }
                }
            }

            elapsedTime = time;
        }
    }
}