
namespace CinemaDirector
{
    /// <summary>
    /// A track designed specifically to hold audio items.
    /// </summary>
    [TimelineTrackAttribute("Audio Track", TimelineTrackGenre.GlobalTrack, CutsceneItemGenre.AudioClipItem)]
    public class AudioTrack : TimelineTrack
    {
        /// <summary>
        /// Set the track to an arbitrary time.
        /// </summary>
        /// <param name="time">The new time.</param>
        public override void SetTime(float time)
        {
            {
                var __array1 = GetTimelineItems();
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var item = (TimelineItem)__array1[__i1];
                    {
                        CinemaAudio cinemaAudio = item as CinemaAudio;
                        if (cinemaAudio != null)
                        {
                            float audioTime = time - cinemaAudio.Firetime;
                            cinemaAudio.SetTime(audioTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pause all Audio Clips that are currently playing.
        /// </summary>
        public override void Pause()
        {
            {
                var __array2 = GetTimelineItems();
                var __arrayLength2 = __array2.Length;
                for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                {
                    var item = (TimelineItem)__array2[__i2];
                    {
                        CinemaAudio cinemaAudio = item as CinemaAudio;
                        if (cinemaAudio != null)
                        {
                            cinemaAudio.Pause();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update the track and play any newly triggered items.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since the last update call.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            float elapsedTime = base.elapsedTime;
            base.elapsedTime = time;
            {
                var __array3 = GetTimelineItems();
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var item = (TimelineItem)__array3[__i3];
                    {
                        CinemaAudio cinemaAudio = item as CinemaAudio;
                        if (cinemaAudio != null)
                        {
                            if (((elapsedTime < cinemaAudio.Firetime) || (elapsedTime <= 0f)) && (((base.elapsedTime >= cinemaAudio.Firetime))))
                            {
                                cinemaAudio.Trigger();
                            }
                            if ((base.elapsedTime > cinemaAudio.Firetime) && (base.elapsedTime <= (cinemaAudio.Firetime + cinemaAudio.Duration)))
                            {
                                float audioTime = time - cinemaAudio.Firetime;
                                cinemaAudio.UpdateTime(audioTime, deltaTime);
                            }
                            if (((elapsedTime <= (cinemaAudio.Firetime + cinemaAudio.Duration)) && (base.elapsedTime > (cinemaAudio.Firetime + cinemaAudio.Duration))))
                            {
                                cinemaAudio.End();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resume playing audio clips after calling a Pause.
        /// </summary>
        public override void Resume()
        {
            {
                var __array4 = GetTimelineItems();
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var item = (TimelineItem)__array4[__i4];
                    {
                        CinemaAudio cinemaAudio = item as CinemaAudio;
                        if (cinemaAudio != null)
                        {
                            if (((base.Cutscene.RunningTime > cinemaAudio.Firetime)) && (base.Cutscene.RunningTime < (cinemaAudio.Firetime + cinemaAudio.Duration)))
                            {
                                cinemaAudio.Resume();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stop playback of all playing audio items.
        /// </summary>
        public override void Stop()
        {
            base.elapsedTime = 0f;
            {
                var __array5 = GetTimelineItems();
                var __arrayLength5 = __array5.Length;
                for (int __i5 = 0; __i5 < __arrayLength5; ++__i5)
                {
                    var item = (TimelineItem)__array5[__i5];
                    {
                        CinemaAudio cinemaAudio = item as CinemaAudio;
                        if (cinemaAudio != null)
                        {
                            cinemaAudio.Stop();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get all cinema audio objects associated with this audio track
        /// </summary>
        public CinemaAudio[] AudioClips
        {
            get
            {
                return GetComponentsInChildren<CinemaAudio>();
            }
        }
    }
}