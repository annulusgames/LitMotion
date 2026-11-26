using System;
using System.Collections.Generic;
using Unity.Collections;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion
{
    internal sealed class ManualMotionDispatcherScheduler : IMotionScheduler
    {
        readonly ManualMotionDispatcher dispatcher;

        public ManualMotionDispatcherScheduler(ManualMotionDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public MotionHandle Schedule<TValue, TOptions, TAdapter>(ref MotionBuilder<TValue, TOptions, TAdapter> builder)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            return dispatcher.GetOrCreateRunner<TValue, TOptions, TAdapter>().Storage.Create(ref builder);
        }
    }

    /// <summary>
    /// Manually updatable MotionDispatcher
    /// </summary>
    public sealed partial class ManualMotionDispatcher
    {
        /// <summary>
        /// Default ManualMotionDispatcher
        /// </summary>
#if UNITY_7000_0_OR_NEWER
        [NoAutoStaticsCleanup] // Singleton; its per-play state is reset by the lifecycle callbacks below.
#endif
        public static readonly ManualMotionDispatcher Default = new();

        /// <summary>
        /// MotionScheduler for scheduling to the dispatcher.
        /// </summary>
        public IMotionScheduler Scheduler => scheduler;

        readonly ManualMotionDispatcherScheduler scheduler;
        readonly Dictionary<Type, IUpdateRunner> runners = new();

        public ManualMotionDispatcher()
        {
            scheduler = new(this);
        }

#if UNITY_7000_0_OR_NEWER
        [UnityEngine.OnEnteringPlayMode]
        static void ResetOnEnteringPlayMode() => ResetDefault();

        [UnityEngine.OnExitingPlayMode]
        static void ResetOnExitingPlayMode() => ResetDefault();

        static void ResetDefault()
        {
            // MotionManager is auto-cleaned with the other static state, so the runners
            // held by the persistent singleton must be rebuilt for the new session.
            Default.runners.Clear();
            Default.time = 0;
        }
#endif

        /// <summary>
        /// ManualMotionDispatcher time. It increases every time Update is called.
        /// </summary>
        public double Time
        {
            get => time;
            set
            {
                Update(value - time);
            }
        }

        double time;

        /// <summary>
        /// Ensures the storage capacity until it reaches at least capacity.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure</param>
        public void EnsureStorageCapacity<TValue, TOptions, TAdapter>(int capacity)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {

            GetOrCreateRunner<TValue, TOptions, TAdapter>().Storage.EnsureCapacity(capacity);
        }

        /// <summary>
        /// Update all scheduled motions.
        /// </summary>
        /// <param name="deltaTime">Delta time</param>
        public void Update(double deltaTime)
        {
            time += deltaTime;

            NativeList<int> completedIndices = default;
            try
            {
                foreach (var kv in runners)
                {
                    kv.Value.Update(time, time, time, ref completedIndices);
                }
            }
            finally
            {
                if (completedIndices.IsCreated) completedIndices.Dispose();
            }
        }

        /// <summary>
        /// Cancel all motions and reset data.
        /// </summary>
        public void Reset()
        {
            foreach (var kv in runners)
            {
                kv.Value.Reset(0, 0, 0);
            }

            time = 0;
        }

        internal UpdateRunner<TValue, TOptions, TAdapter> GetOrCreateRunner<TValue, TOptions, TAdapter>()
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            var key = typeof((TValue, TOptions, TAdapter));
            if (!runners.TryGetValue(key, out var runner))
            {
                var storage = new MotionStorage<TValue, TOptions, TAdapter>(MotionManager.MotionTypeCount);
                MotionManager.Register(storage);

                runner = new UpdateRunner<TValue, TOptions, TAdapter>(storage, 0, 0, 0);
                runners.Add(key, runner);
            }

            return (UpdateRunner<TValue, TOptions, TAdapter>)runner;
        }
    }
}
