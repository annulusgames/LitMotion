using System;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion
{
    /// <summary>
    /// Provides functionality for tracking active motions.
    /// </summary>
#if UNITY_7000_0_OR_NEWER
    [AutoStaticsCleanup]
#endif
    public static partial class MotionDebugger
    {
#if UNITY_7000_0_OR_NEWER
        [NoAutoStaticsCleanup] // Editor preference; keep the user's debugger setting across Play mode.
#endif
        public static bool Enabled = false;
#if UNITY_7000_0_OR_NEWER
        [NoAutoStaticsCleanup] // Editor preference; keep the user's debugger setting across Play mode.
#endif
        public static bool EnableStackTrace = false;

        public static IReadOnlyList<TrackingState> Items => trackings;
        static readonly List<TrackingState> trackings = new(16);

        public static void AddTracking(MotionHandle motionHandle, IMotionScheduler scheduler, int skipFrames = 3)
        {
            var state = TrackingState.Create();
            (state.ValueType, state.OptionsType, state.AdapterType) = MotionManager.GetMotionType(motionHandle);
            state.Scheduler = scheduler;
            state.Handle = motionHandle;
#if UNITY_EDITOR
            state.CreatedOnEditor = UnityEditor.EditorApplication.isPlaying;
#endif

            if (EnableStackTrace) state.StackTrace = new StackTrace(skipFrames, true);

            ref var managedData = ref MotionManager.GetManagedDataRef(motionHandle, false);
            state.OriginalOnCompleteCallback = managedData.OnCompleteAction;
            managedData.OnCompleteAction = state.OnCompleteDelegate;
            state.OriginalOnCancelCallback = managedData.OnCancelAction;
            managedData.OnCancelAction = state.OnCancelDelegate;

            trackings.Add(state);
        }

        public static void Clear()
        {
            trackings.Clear();
        }

#if UNITY_7000_0_OR_NEWER
        [AutoStaticsCleanup]
#endif
        public sealed partial class TrackingState
        {
            static readonly Stack<TrackingState> pool = new(16);

            TrackingState()
            {
                OnCompleteDelegate = OnComplete;
                OnCancelDelegate = OnCancel;
            }

            public static TrackingState Create()
            {
                if (!pool.TryPop(out var state))
                {
                    state = new();
                }
                return state;
            }

            public Type ValueType;
            public Type OptionsType;
            public Type AdapterType;
            public IMotionScheduler Scheduler;
            public MotionHandle Handle;
            public StackTrace StackTrace;
            public bool CreatedOnEditor;
            public Action OriginalOnCompleteCallback;
            public Action OriginalOnCancelCallback;

            public readonly Action OnCompleteDelegate;
            public readonly Action OnCancelDelegate;

            void OnComplete()
            {
                try
                {
                    OriginalOnCompleteCallback?.Invoke();
                }
                catch (Exception ex)
                {
                    MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
                }

                if (Handle.IsActive() && !MotionManager.GetDataRef(Handle, false).State.IsPreserved)
                {
                    Release();
                }
            }

            void OnCancel()
            {
                try
                {
                    OriginalOnCancelCallback?.Invoke();
                }
                catch (Exception ex)
                {
                    MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
                }
                Release();
            }

            void Release()
            {
                trackings.Remove(this);
                ValueType = default;
                OptionsType = default;
                AdapterType = default;
                Scheduler = default;
                Handle = default;
                StackTrace = default;
                CreatedOnEditor = default;
                OriginalOnCompleteCallback = default;
                OriginalOnCancelCallback = default;
                pool.Push(this);
            }
        }
    }
}