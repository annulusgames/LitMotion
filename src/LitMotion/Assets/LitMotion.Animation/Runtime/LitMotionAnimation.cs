using System;
using System.Collections.Generic;
using UnityEngine;

namespace LitMotion.Animation
{
    [AddComponentMenu("LitMotion Animation")]
    public sealed class LitMotionAnimation : MonoBehaviour
    {
        enum AutoPlayMode
        {
            None,
            OnStart,
            OnEnable
        }

        enum AnimationMode
        {
            Parallel,
            Sequential
        }

        [SerializeField] AutoPlayMode autoPlayMode = AutoPlayMode.OnStart;
        [SerializeField] AnimationMode animationMode;

        [SerializeReference]
        LitMotionAnimationComponent[] components;

        private List<LitMotionAnimationComponent> playingComponents = new();

        public IReadOnlyList<LitMotionAnimationComponent> Components => components;

        private bool IsPlayForward = true;
        private int playIndex = 0;

        private void OnEnable()
        {
            if (autoPlayMode == AutoPlayMode.OnEnable)
                Play();
        }

        void Start()
        {
            if (autoPlayMode == AutoPlayMode.OnStart)
                Play();
        }

        private void OnCompleteAction()
        {
            playIndex += IsPlayForward ? 1 : -1;
            //Debug.Log($"OnCompleteAction called. playIndex: {playIndex}");
            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    try
                    {
                        if (playIndex < playingComponents.Count && playIndex >= 0)
                        {
                            var component = playingComponents[playIndex];
                            var handle = component.Play();
                            handle.PlaybackSpeed = IsPlayForward ? 1f : -1f;
                            handle.Time = IsPlayForward ? 0 : GetPlayBackwardTime(handle);
                            component.TrackedHandle = handle;

                            if (handle.IsActive())
                            {
                                int loop = handle.Loops;
                                if (loop <= 0)
                                    loop = 1;
                                var temp = LMotion.Create(0f, 1f, (float)handle.TotalDuration).WithOnComplete(OnCompleteAction).RunWithoutBinding();
                                MotionManager.GetManagedDataRef(handle, false).OnCancelAction += () => temp.TryCancel();
                                //MotionManager.GetManagedDataRef(handle, false).OnCompleteAction += OnCompleteAction;
                                //handle.Preserve();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                    break;
            }
        }

        public void Play()
        {
            playIndex = IsPlayForward ? -1 : components.Length;
            var isPlaying = false;

            foreach (var component in playingComponents)
            {
                var handle = component.TrackedHandle;
                if (handle.IsActive())
                {
                    handle.PlaybackSpeed = IsPlayForward ? 1f : -1f;
                    isPlaying = true;

                    component.OnResume();
                }
            }

            if (isPlaying) return;

            playingComponents.Clear();

            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    foreach (var component in components)
                    {
                        if (component == null) continue;
                        if (!component.Enabled) continue;
                        playingComponents.Add(component);
                    }

                    OnCompleteAction();
                    break;

                case AnimationMode.Parallel:
                    try
                    {
                        foreach (var component in components)
                        {
                            if (component == null) continue;
                            if (!component.Enabled) continue;

                            var handle = component.Play();
                            handle.PlaybackSpeed = IsPlayForward ? 1f : -1f;
                            handle.Time = IsPlayForward ? 0 : GetPlayBackwardTime(handle);
                            component.TrackedHandle = handle;

                            //if (handle.IsActive())
                            //{
                            //    handle.Preserve();
                            //}

                            playingComponents.Add(component);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                    break;
            }

            //Debug.Log($"Play called. Playing components count: {playingComponents.Count}");
        }

        private float GetPlayBackwardTime(MotionHandle handle)
        {
            float playbackwardTime = handle.Duration * handle.Loops;
            int loop = handle.Loops;
            if (loop < 0)
                loop = 1000000;
            playbackwardTime = handle.Duration * loop - 0.001f;
            //Debug.Log($"Setting backward time to {playbackwardTime}");
            return playbackwardTime;
        }

        public void PlayForward()
        {
            IsPlayForward = true;
            Restart();
        }

        public void PlayBackward()
        {
            IsPlayForward = false;
            Restart();
        }

        public void Pause()
        {
            foreach (var component in playingComponents)
            {
                var handle = component.TrackedHandle;
                if (handle.IsActive())
                {
                    handle.PlaybackSpeed = 0f;
                    component.OnPause();
                }
            }
        }

        public void Stop()
        {
            foreach (var component in playingComponents)
            {
                var handle = component.TrackedHandle;
                handle.TryCancel();
                component.OnStop();
                component.TrackedHandle = handle;
            }

            playingComponents.Clear();
        }

        public void Restart()
        {
            Stop();
            Play();
        }

        public bool IsActive
        {
            get
            {
                foreach (var component in playingComponents)
                {
                    var handle = component.TrackedHandle;
                    if (handle.IsActive()) return true;
                }

                return false;
            }
        }

        public bool IsPlaying
        {
            get
            {
                foreach (var component in playingComponents)
                {
                    var handle = component.TrackedHandle;
                    if (handle.IsPlaying()) return true;
                }

                return false;
            }
        }

        private void OnDisable()
        {
            if (autoPlayMode == AutoPlayMode.OnEnable)
                Stop();
        }

        void OnDestroy()
        {
            Stop();
        }
    }
}