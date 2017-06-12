// Cinema Suite
using CinemaDirector.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CinemaDirector
{
    /// <summary>
    /// The Cutscene is the main Behaviour of Cinema Director.
    /// </summary>
    [ExecuteInEditMode, Serializable]
    public class Cutscene : MonoBehaviour, IOptimizable
    {
        #region Fields
        [SerializeField]
        private float duration = 30f; // Duration of cutscene in seconds.

        [SerializeField]
        private float runningTime = 0f; // Running time of the cutscene in seconds.

        [SerializeField]
        private float playbackSpeed = 1f; // Multiplier for playback speed.

        [NonSerialized]
        private CutsceneState state = CutsceneState.Inactive;

        [SerializeField]
        private bool isSkippable = true;

        [SerializeField]
        private bool isLooping = false;

        [SerializeField]
        private bool canOptimize = true;

        // Keeps track of the previous time an update was made.
        private float previousTime;

        // Has the Cutscene been optimized yet.
        private bool hasBeenOptimized = false;

        // Has the Cutscene been initialized yet.
        private bool hasBeenInitialized = false;

        // The cache of Track Groups that this Cutscene contains.
        private TrackGroup[] trackGroupCache;

        // A list of all the Tracks and Items revert info, to revert states on Cutscene entering and exiting play mode.
        private List<RevertInfo> revertCache = new List<RevertInfo>();
        #endregion

        // Event fired when Cutscene's runtime reaches it's duration.
        public event CutsceneHandler CutsceneFinished;

        // Event fired when Cutscene has been paused.
        public event CutsceneHandler CutscenePaused;

        /// <summary>
        /// Optimizes the Cutscene by preparing all tracks and timeline items into a cache.
        /// Call this on scene load in most cases. (Avoid calling in edit mode).
        /// </summary>
        public void Optimize()
        {
            if (canOptimize)
            {
                trackGroupCache = GetTrackGroups();
                hasBeenOptimized = true;
            }
            {
                var __array1 = GetTrackGroups();
                var __arrayLength1 = __array1.Length;
                for (int __i1 = 0; __i1 < __arrayLength1; ++__i1)
                {
                    var tg = (TrackGroup)__array1[__i1];
                    {
                        tg.Optimize();
                    }
                }
            }
        }

        /// <summary>
        /// Plays/Resumes the cutscene from inactive/paused states using a Coroutine.
        /// </summary>
        public void Play()
        {
            if (state == CutsceneState.Inactive)
            {
                state = CutsceneState.Playing;
                if (!hasBeenOptimized)
                {
                    Optimize();
                }
                if (!hasBeenInitialized)
                {
                    initialize();
                }
                StartCoroutine("updateCoroutine");
            }
            else if (state == CutsceneState.Paused)
            {
                state = CutsceneState.Playing;
                StartCoroutine("updateCoroutine");
            }
        }

        /// <summary>
        /// Pause the playback of this cutscene.
        /// </summary>
        public void Pause()
        {
            if (state == CutsceneState.Playing)
            {
                StopCoroutine("updateCoroutine");
            }
            if (state == CutsceneState.PreviewPlaying || state == CutsceneState.Playing || state == CutsceneState.Scrubbing)
            {
                {
                    var __array2 = GetTrackGroups();
                    var __arrayLength2 = __array2.Length;
                    for (int __i2 = 0; __i2 < __arrayLength2; ++__i2)
                    {
                        var trackGroup = (TrackGroup)__array2[__i2];
                        {
                            trackGroup.Pause();
                        }
                    }
                }
            }
            state = CutsceneState.Paused;

            if (CutscenePaused != null)
            {
                CutscenePaused(this, new CutsceneEventArgs());
            }
        }

        /// <summary>
        /// Skip the cutscene to the end and stop it
        /// </summary>
        public void Skip()
        {
            if (isSkippable)
            {
                SetRunningTime(this.Duration);
                state = CutsceneState.Inactive;
                Stop();
            }
        }

        /// <summary>
        /// Stops the cutscene.
        /// </summary>
        public void Stop()
        {
            this.RunningTime = 0f;
            {
                var __array3 = GetTrackGroups();
                var __arrayLength3 = __array3.Length;
                for (int __i3 = 0; __i3 < __arrayLength3; ++__i3)
                {
                    var trackGroup = (TrackGroup)__array3[__i3];
                    {
                        trackGroup.Stop();
                    }
                }
            }
            revert();

            if (state == CutsceneState.Playing)
            {
                StopCoroutine("updateCoroutine");
                if (state == CutsceneState.Playing && isLooping)
                {
                    state = CutsceneState.Inactive;
                    Play();
                }
                else
                {
                    state = CutsceneState.Inactive;
                }
            }
            else
            {
                state = CutsceneState.Inactive;
            }

            if (state == CutsceneState.Inactive)
            {
                if (CutsceneFinished != null)
                {
                    CutsceneFinished(this, new CutsceneEventArgs());
                }
            }
        }

        /// <summary>
        /// Updates the cutscene by the amount of time passed.
        /// </summary>
        /// <param name="deltaTime">The delta in time between the last update call and this one.</param>
        public void UpdateCutscene(float deltaTime)
        {
            this.RunningTime += (deltaTime * playbackSpeed);
            {
                var __array4 = GetTrackGroups();
                var __arrayLength4 = __array4.Length;
                for (int __i4 = 0; __i4 < __arrayLength4; ++__i4)
                {
                    var group = (TrackGroup)__array4[__i4];
                    {
                        group.UpdateTrackGroup(RunningTime, deltaTime * playbackSpeed);
                    }
                }
            }
            if (state != CutsceneState.Scrubbing)
            {
                if (runningTime >= duration || runningTime < 0f)
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Preview play readies the cutscene to be played in edit mode. Never use for runtime.
        /// This is necessary for playing the cutscene in edit mode.
        /// </summary>
        public void PreviewPlay()
        {
            if (state == CutsceneState.Inactive)
            {
                EnterPreviewMode();
            }
            else if (state == CutsceneState.Paused)
            {
                resume();
            }

            if (Application.isPlaying)
            {
                state = CutsceneState.Playing;
            }
            else
            {
                state = CutsceneState.PreviewPlaying;
            }
        }

        /// <summary>
        /// Play the cutscene from it's given running time to a new time
        /// </summary>
        /// <param name="newTime">The new time to make up for</param>
        public void ScrubToTime(float newTime)
        {
            float deltaTime = Mathf.Clamp(newTime, 0, Duration) - this.RunningTime;

            state = CutsceneState.Scrubbing;
            if (deltaTime != 0)
            {
                if (deltaTime > (1 / 30f))
                {
                    float prevTime = RunningTime;
                    {
                        var __list5 = getMilestones(RunningTime + deltaTime);
                        var __listCount5 = __list5.Count;
                        for (int __i5 = 0; __i5 < __listCount5; ++__i5)
                        {
                            var milestone = (float)__list5[__i5];
                            {
                                float delta = milestone - prevTime;
                                UpdateCutscene(delta);
                                prevTime = milestone;
                            }
                        }
                    }
                }
                else
                {
                    UpdateCutscene(deltaTime);
                }
            }
            else
            {
                Pause();
            }
        }

        /// <summary>
        /// Set the cutscene to the state of a given new running time and do not proceed to play the cutscene
        /// </summary>
        /// <param name="time">The new running time to be set.</param>
        public void SetRunningTime(float time)
        {
            {
                var __list6 = getMilestones(time);
                var __listCount6 = __list6.Count;
                for (int __i6 = 0; __i6 < __listCount6; ++__i6)
                {
                    var milestone = (float)__list6[__i6];
                    {
                        {
                            var __array13 = this.TrackGroups;
                            var __arrayLength13 = __array13.Length;
                            for (int __i13 = 0; __i13 < __arrayLength13; ++__i13)
                            {
                                var group = (TrackGroup)__array13[__i13];
                                {
                                    group.SetRunningTime(milestone);
                                }
                            }
                        }
                    }
                }
            }
            this.RunningTime = time;
        }

        /// <summary>
        /// Set the cutscene into an active state.
        /// </summary>
        public void EnterPreviewMode()
        {
            if (state == CutsceneState.Inactive)
            {
                initialize();
                bake();
                SetRunningTime(RunningTime);
                state = CutsceneState.Paused;
            }
        }

        /// <summary>
        /// Set the cutscene into an inactive state.
        /// </summary>
        public void ExitPreviewMode()
        {
            Stop();
        }

        /// <summary>
        /// Cutscene has been destroyed. Revert everything if necessary.
        /// </summary>
        protected void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                Stop();
            }
        }

        /// <summary>
        /// Exit and Re-enter preview mode at the current running time.
        /// Important for Mecanim Previewing.
        /// </summary>
        public void Refresh()
        {
            if (state != CutsceneState.Inactive)
            {
                float tempTime = runningTime;
                Stop();
                EnterPreviewMode();
                SetRunningTime(tempTime);
            }
        }

        /// <summary>
        /// Bake all Track Groups who are Bakeable.
        /// This is necessary for things like mecanim previewing.
        /// </summary>
        private void bake()
        {
            if (Application.isEditor)
            {
                {
                    var __array7 = this.TrackGroups;
                    var __arrayLength7 = __array7.Length;
                    for (int __i7 = 0; __i7 < __arrayLength7; ++__i7)
                    {
                        var group = (TrackGroup)__array7[__i7];
                        {
                            if (group is IBakeable)
                            {
                                (group as IBakeable).Bake();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The duration of this cutscene in seconds.
        /// </summary>
        public float Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
                if (this.duration <= 0f)
                {
                    this.duration = 0.1f;
                }
            }
        }

        /// <summary>
        /// Returns true if this cutscene is currently playing.
        /// </summary>
        public CutsceneState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        /// The current running time of this cutscene in seconds. Values are restricted between 0 and duration.
        /// </summary>
        public float RunningTime
        {
            get
            {
                return this.runningTime;
            }
            set
            {
                runningTime = Mathf.Clamp(value, 0, duration);
            }
        }

        /// <summary>
        /// Get all Track Groups in this Cutscene. Will return cache if possible.
        /// </summary>
        /// <returns></returns>
        public TrackGroup[] GetTrackGroups()
        {
            // Return the cache if possible
            if (hasBeenOptimized)
            {
                return trackGroupCache;
            }

            return TrackGroups;
        }

        /// <summary>
        /// Get all track groups in this cutscene.
        /// </summary>
        public TrackGroup[] TrackGroups
        {
            get
            {
                return base.GetComponentsInChildren<TrackGroup>();
            }
        }

        /// <summary>
        /// Get the director group of this cutscene.
        /// </summary>
        public DirectorGroup DirectorGroup
        {
            get
            {
                return base.GetComponentInChildren<DirectorGroup>();
            }
        }

        /// <summary>
        /// Cutscene state is used to determine if the cutscene is currently Playing, Previewing (In edit mode), paused, scrubbing or inactive.
        /// </summary>
        public enum CutsceneState
        {
            Inactive,
            Playing,
            PreviewPlaying,
            Scrubbing,
            Paused
        }

        /// <summary>
        /// Enable this if the Cutscene does not have TrackGroups added/removed during running.
        /// </summary>
        public bool CanOptimize
        {
            get { return canOptimize; }
            set { canOptimize = value; }
        }

        /// <summary>
        /// True if Cutscene can be skipped.
        /// </summary>
        public bool IsSkippable
        {
            get { return isSkippable; }
            set { isSkippable = value; }
        }

        /// <summary>
        /// Will the Cutscene loop upon completion.
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
            set { isLooping = value; }
        }

        /// <summary>
        /// An important call to make before sampling the cutscene, to initialize all track groups 
        /// and save states of all actors/game objects.
        /// </summary>
        private void initialize()
        {
            saveRevertData();
            {
                var __array8 = this.TrackGroups;
                var __arrayLength8 = __array8.Length;
                for (int __i8 = 0; __i8 < __arrayLength8; ++__i8)
                {
                    var trackGroup = (TrackGroup)__array8[__i8];
                    {
                        trackGroup.Initialize();
                    }
                }
            }
        }

        /// <summary>
        /// Cache all the revert data.
        /// </summary>
        private void saveRevertData()
        {
            revertCache.Clear();
            {
                var __array9 = this.GetComponentsInChildren<MonoBehaviour>();
                var __arrayLength9 = __array9.Length;
                for (int __i9 = 0; __i9 < __arrayLength9; ++__i9)
                {
                    var mb = (MonoBehaviour)__array9[__i9];
                    {
                        IRevertable revertable = mb as IRevertable;
                        if (revertable != null)
                        {
                            revertCache.AddRange(revertable.CacheState());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Revert all data that has been manipulated by the Cutscene.
        /// </summary>
        private void revert()
        {
            {
                var __list10 = revertCache;
                var __listCount10 = __list10.Count;
                for (int __i10 = 0; __i10 < __listCount10; ++__i10)
                {
                    var revertable = (RevertInfo)__list10[__i10];
                    {
                        if (revertable != null)
                        {
                            if ((revertable.EditorRevert == RevertMode.Revert && !Application.isPlaying) ||
                                (revertable.RuntimeRevert == RevertMode.Revert && Application.isPlaying))
                            {
                                revertable.Revert();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the milestones between the current running time and the given time.
        /// </summary>
        /// <param name="time">The time to progress towards</param>
        /// <returns>A list of times that state changes are made in the Cutscene.</returns>
        private List<float> getMilestones(float time)
        {
            // Create a list of ordered milestone times.
            List<float> milestoneTimes = new List<float>();
            milestoneTimes.Add(time);
            {
                var __array11 = this.TrackGroups;
                var __arrayLength11 = __array11.Length;
                for (int __i11 = 0; __i11 < __arrayLength11; ++__i11)
                {
                    var group = (TrackGroup)__array11[__i11];
                    {
                        List<float> times = group.GetMilestones(RunningTime, time);
                        {
                            var __list14 = times;
                            var __listCount14 = __list14.Count;
                            for (int __i14 = 0; __i14 < __listCount14; ++__i14)
                            {
                                var f = (float)__list14[__i14];
                                {
                                    if (!milestoneTimes.Contains(f))
                                        milestoneTimes.Add(f);
                                }
                            }
                        }
                    }
                }
            }
            milestoneTimes.Sort();
            if (time < RunningTime)
            {
                milestoneTimes.Reverse();
            }

            return milestoneTimes;
        }

        /// <summary>
        /// Couroutine to call UpdateCutscene while the cutscene is in the playing state.
        /// </summary>
        /// <returns></returns>
        private IEnumerator updateCoroutine()
        {
            while (state == CutsceneState.Playing)
            {
                UpdateCutscene(Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// Prepare all track groups for resuming from pause.
        /// </summary>
        private void resume()
        {
            {
                var __array12 = this.TrackGroups;
                var __arrayLength12 = __array12.Length;
                for (int __i12 = 0; __i12 < __arrayLength12; ++__i12)
                {
                    var group = (TrackGroup)__array12[__i12];
                    {
                        group.Resume();
                    }
                }
            }
        }
    }

    // Delegate for handling Cutscene Events
    public delegate void CutsceneHandler(object sender, CutsceneEventArgs e);

    /// <summary>
    /// Cutscene event arguments. Blank for now.
    /// </summary>
    public class CutsceneEventArgs : EventArgs
    {
        public CutsceneEventArgs()
        {
        }
    }
}
