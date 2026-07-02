using System;
using System.Collections.Generic;

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
    public sealed class ManualMotionDispatcher
    {
        /// <summary>
        /// Default ManualMotionDispatcher
        /// </summary>
        public static readonly ManualMotionDispatcher Default = new();

        /// <summary>
        /// MotionScheduler for scheduling to the dispatcher.
        /// </summary>
        public IMotionScheduler Scheduler => scheduler;

        readonly ManualMotionDispatcherScheduler scheduler;
        readonly Dictionary<Type, IUpdateRunner> runners = new();

        // Reused buffer so Update/Reset iterate a snapshot of the runners instead of the live
        // dictionary. A motion callback fired during iteration can schedule the first motion of a
        // new type, which lazily adds a runner (see GetOrCreateRunner) and would otherwise throw
        // "InvalidOperationException: Collection was modified" mid-enumeration. Reused rather than
        // allocated per call; AddRange(runners.Values) copies via ICollection.CopyTo and the
        // Dictionary caches its ValueCollection, so this stays allocation-free after warmup.
        readonly List<IUpdateRunner> runnerBuffer = new();

        public ManualMotionDispatcher()
        {
            scheduler = new(this);
        }

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

            runnerBuffer.Clear();
            runnerBuffer.AddRange(runners.Values);
            for (int i = 0; i < runnerBuffer.Count; i++)
            {
                runnerBuffer[i].Update(time, time, time);
            }
            runnerBuffer.Clear();
        }

        /// <summary>
        /// Cancel all motions and reset data.
        /// </summary>
        public void Reset()
        {
            runnerBuffer.Clear();
            runnerBuffer.AddRange(runners.Values);
            for (int i = 0; i < runnerBuffer.Count; i++)
            {
                runnerBuffer[i].Reset();
            }
            runnerBuffer.Clear();

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