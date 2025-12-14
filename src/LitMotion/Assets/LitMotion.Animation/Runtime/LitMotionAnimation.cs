using System;
using System.Collections;
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
        public bool isPlayForward = true;
        public bool manualLoop;

        [SerializeReference]
        LitMotionAnimationComponent[] components;

        private List<LitMotionAnimationComponent> playingComponents = new();

        public IReadOnlyList<LitMotionAnimationComponent> Components => components;

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

            if(manualLoop)
                StartCoroutine(ManualLoopCr());
        }

        IEnumerator ManualLoopCr()
        {
            yield return null;

            while (manualLoop)
            {
                if (!IsPlaying)
                {
                    Play();
                }
                yield return null;
            }
        }

        private void OnNextSequence()
        {
#if UNITY_EDITOR
            Debug.Log($"OnCompleteAction called. playIndex: {playIndex} IsPlayForward {isPlayForward}");
#endif
            switch (animationMode)
            {
                case AnimationMode.Sequential:
                    try
                    {
                        playIndex += isPlayForward ? 1 : -1;

                        if (playIndex < playingComponents.Count && playIndex >= 0)
                        {
                            var component = playingComponents[playIndex];
                            var handle = isPlayForward ? component.Play() : component.PlayBackward();
                            component.TrackedHandle = handle;

                            if (handle.IsActive())
                            {
                                //handle.Preserve();
                                MotionManager.GetManagedDataRef(handle, false).OnCompleteAction += OnNextSequence;
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
            if (Resume())
                return;

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

                    playIndex = isPlayForward ? -1 : playingComponents.Count;
                    OnNextSequence();
                    break;

                case AnimationMode.Parallel:
                    try
                    {
                        foreach (var component in components)
                        {
                            if (component == null) continue;
                            if (!component.Enabled) continue;

                            var handle = isPlayForward ? component.Play() : component.PlayBackward();
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

#if UNITY_EDITOR
            Debug.Log($"Play called. Playing components count: {playingComponents.Count}");
#endif
        }

        public void PlayForward()
        {
            isPlayForward = true;
            Restart();
        }

        public void PlayBackward()
        {
            isPlayForward = false;
            Restart();
        }

        public bool Resume()
        {
            var isPlaying = false;

            foreach (var component in playingComponents)
            {
                var handle = component.TrackedHandle;
                if (handle.IsActive())
                {
                    handle.PlaybackSpeed = 1f;
                    isPlaying = true;

                    component.OnResume();
                }
            }

            return isPlaying;
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

        public async void Restart()
        {
            Stop();

            while (IsActive)
                await Awaitable.NextFrameAsync();

            Play();
        }

        public float GetDuration()
        {
            float totalDuration = 0f;
            switch(animationMode)
            {
                case AnimationMode.Sequential:
                    foreach (var component in playingComponents)
                    {
                        var handle = component.TrackedHandle;
                        totalDuration += (float)handle.TotalDuration;
                    }
                    break;

                case AnimationMode.Parallel:
                    foreach (var component in playingComponents)
                    {
                        var handle = component.TrackedHandle;
                        var duration = (float)handle.TotalDuration;
                        if (duration > totalDuration)
                            totalDuration = duration;
                    }
                    break;
            }
            return totalDuration;
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
                switch (animationMode)
                {
                    case AnimationMode.Sequential:
                        if (playIndex >= 0 && playIndex < playingComponents.Count)
                            return true;
                        break;

                    case AnimationMode.Parallel:
                        foreach (var component in playingComponents)
                        {
                            var handle = component.TrackedHandle;
                            if (handle.IsPlaying()) return true;
                        }
                        break;
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