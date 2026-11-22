using System;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion
{
    /// <summary>
    /// Schedulers available in Runtime.
    /// </summary>
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Scheduler instances are immutable and DefaultScheduler is user configuration.
#endif
    public static class MotionScheduler
    {
        static MotionScheduler()
        {
            DefaultScheduler = Update;
        }

        /// <summary>
        /// Default scheduler used if not specified
        /// </summary>
        public static IMotionScheduler DefaultScheduler { get; set; }

        /// <summary>
        /// Scheduler that updates motion at Initialization.
        /// </summary>
        public static readonly IMotionScheduler Initialization = new PlayerLoopMotionScheduler(PlayerLoopTiming.Initialization, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at Initialization. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler InitializationIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.Initialization, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at Initialization. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler InitializationRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.Initialization, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion at EarlyUpdate.
        /// </summary>
        public static readonly IMotionScheduler EarlyUpdate = new PlayerLoopMotionScheduler(PlayerLoopTiming.EarlyUpdate, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at EarlyUpdate. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler EarlyUpdateIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.EarlyUpdate, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at EarlyUpdate. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler EarlyUpdateRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.EarlyUpdate, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion at FixedUpdate.
        /// </summary>
        public static readonly IMotionScheduler FixedUpdate = new PlayerLoopMotionScheduler(PlayerLoopTiming.FixedUpdate, MotionTimeKind.Time);

        /// <summary>
        /// Scheduler that updates motion at PreUpdate.
        /// </summary>
        public static readonly IMotionScheduler PreUpdate = new PlayerLoopMotionScheduler(PlayerLoopTiming.PreUpdate, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at PreUpdate. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler PreUpdateIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.PreUpdate, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at PreUpdate. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler PreUpdateRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.PreUpdate, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion at Update.
        /// </summary>
        public static readonly IMotionScheduler Update = new PlayerLoopMotionScheduler(PlayerLoopTiming.Update, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at Update. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler UpdateIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.Update, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at Update. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler UpdateRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.Update, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion at PreLateUpdate.
        /// </summary>
        public static readonly IMotionScheduler PreLateUpdate = new PlayerLoopMotionScheduler(PlayerLoopTiming.PreLateUpdate, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at PreLateUpdate. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler PreLateUpdateIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.PreLateUpdate, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at PreLateUpdate. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler PreLateUpdateRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.PreLateUpdate, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion at PostLateUpdate.
        /// </summary>
        public static readonly IMotionScheduler PostLateUpdate = new PlayerLoopMotionScheduler(PlayerLoopTiming.PostLateUpdate, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at PostLateUpdate. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler PostLateUpdateIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.PostLateUpdate, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at PostLateUpdate. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler PostLateUpdateRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.PostLateUpdate, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion at TimeUpdate.
        /// </summary>
        public static readonly IMotionScheduler TimeUpdate = new PlayerLoopMotionScheduler(PlayerLoopTiming.TimeUpdate, MotionTimeKind.Time);
        /// <summary>
        /// Scheduler that updates motion at TimeUpdate. (Ignore timescale)
        /// </summary>
        public static readonly IMotionScheduler TimeUpdateIgnoreTimeScale = new PlayerLoopMotionScheduler(PlayerLoopTiming.TimeUpdate, MotionTimeKind.UnscaledTime);
        /// <summary>
        /// Scheduler that updates motion at TimeUpdate. (Realtime)
        /// </summary>
        public static readonly IMotionScheduler TimeUpdateRealtime = new PlayerLoopMotionScheduler(PlayerLoopTiming.TimeUpdate, MotionTimeKind.Realtime);

        /// <summary>
        /// Scheduler that updates motion with `ManualMotionDispatcher.Default.Update()`
        /// </summary>
        public static IMotionScheduler Manual => ManualMotionDispatcher.Default.Scheduler;
    }
}