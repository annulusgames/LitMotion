using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using LitMotion.Collections;
using LitMotion.LitMotion.Runtime.Internal;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LitMotion
{
    /// <summary>
    /// Motion dispatcher.
    /// </summary>
    public static class MotionDispatcher
    {
        static class StorageCache<TValue, TOptions, TAdapter>
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            public static MotionStorage<TValue, TOptions, TAdapter> initialization;
            public static MotionStorage<TValue, TOptions, TAdapter> earlyUpdate;
            public static MotionStorage<TValue, TOptions, TAdapter> fixedUpdate;
            public static MotionStorage<TValue, TOptions, TAdapter> preUpdate;
            public static MotionStorage<TValue, TOptions, TAdapter> update;
            public static MotionStorage<TValue, TOptions, TAdapter> preLateUpdate;
            public static MotionStorage<TValue, TOptions, TAdapter> postLateUpdate;
            public static MotionStorage<TValue, TOptions, TAdapter> timeUpdate;

            public static MotionStorage<TValue, TOptions, TAdapter> GetOrCreate(PlayerLoopTiming playerLoopTiming)
            {
                return playerLoopTiming switch
                {
                    PlayerLoopTiming.Initialization => CreateIfNull(ref initialization),
                    PlayerLoopTiming.EarlyUpdate => CreateIfNull(ref earlyUpdate),
                    PlayerLoopTiming.FixedUpdate => CreateIfNull(ref fixedUpdate),
                    PlayerLoopTiming.PreUpdate => CreateIfNull(ref preUpdate),
                    PlayerLoopTiming.Update => CreateIfNull(ref update),
                    PlayerLoopTiming.PreLateUpdate => CreateIfNull(ref preLateUpdate),
                    PlayerLoopTiming.PostLateUpdate => CreateIfNull(ref postLateUpdate),
                    PlayerLoopTiming.TimeUpdate => CreateIfNull(ref timeUpdate),
                    _ => null,
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static MotionStorage<TValue, TOptions, TAdapter> CreateIfNull(ref MotionStorage<TValue, TOptions, TAdapter> storage)
            {
                if (storage == null)
                {
                    storage = new MotionStorage<TValue, TOptions, TAdapter>(MotionManager.MotionTypeCount);
                    MotionManager.Register(storage);
                    EnterThePlayModeHelper.Register(storage);
                }
                return storage;
            }
        }

        static class RunnerCache<TValue, TOptions, TAdapter>
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            public static UpdateRunner<TValue, TOptions, TAdapter> initialization;
            public static UpdateRunner<TValue, TOptions, TAdapter> earlyUpdate;
            public static UpdateRunner<TValue, TOptions, TAdapter> fixedUpdate;
            public static UpdateRunner<TValue, TOptions, TAdapter> preUpdate;
            public static UpdateRunner<TValue, TOptions, TAdapter> update;
            public static UpdateRunner<TValue, TOptions, TAdapter> preLateUpdate;
            public static UpdateRunner<TValue, TOptions, TAdapter> postLateUpdate;
            public static UpdateRunner<TValue, TOptions, TAdapter> timeUpdate;

            public static (UpdateRunner<TValue, TOptions, TAdapter> runner, bool isCreated) GetOrCreate(PlayerLoopTiming playerLoopTiming, MotionStorage<TValue, TOptions, TAdapter> storage)
            {
                return playerLoopTiming switch
                {
                    PlayerLoopTiming.Initialization => CreateIfNull(playerLoopTiming, ref initialization, storage),
                    PlayerLoopTiming.EarlyUpdate => CreateIfNull(playerLoopTiming, ref earlyUpdate, storage),
                    PlayerLoopTiming.FixedUpdate => CreateIfNull(playerLoopTiming, ref fixedUpdate, storage),
                    PlayerLoopTiming.PreUpdate => CreateIfNull(playerLoopTiming, ref preUpdate, storage),
                    PlayerLoopTiming.Update => CreateIfNull(playerLoopTiming, ref update, storage),
                    PlayerLoopTiming.PreLateUpdate => CreateIfNull(playerLoopTiming, ref preLateUpdate, storage),
                    PlayerLoopTiming.PostLateUpdate => CreateIfNull(playerLoopTiming, ref postLateUpdate, storage),
                    PlayerLoopTiming.TimeUpdate => CreateIfNull(playerLoopTiming, ref timeUpdate, storage),
                    _ => default,
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static (UpdateRunner<TValue, TOptions, TAdapter>, bool) CreateIfNull(PlayerLoopTiming playerLoopTiming, ref UpdateRunner<TValue, TOptions, TAdapter> runner, MotionStorage<TValue, TOptions, TAdapter> storage)
            {
                if (runner == null)
                {
                    if (playerLoopTiming == PlayerLoopTiming.FixedUpdate)
                    {
                        runner = new UpdateRunner<TValue, TOptions, TAdapter>(storage, Time.fixedTimeAsDouble, Time.fixedUnscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
                    }
                    else
                    {
                        runner = new UpdateRunner<TValue, TOptions, TAdapter>(storage, Time.timeAsDouble, Time.unscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
                    }
                    GetRunnerList(playerLoopTiming).Add(runner);
                    EnterThePlayModeHelper.Register(runner);
                    return (runner, true);
                }
                return (runner, false);
            }
        }

        static FastListCore<IUpdateRunner> initializationRunners;
        static FastListCore<IUpdateRunner> earlyUpdateRunners;
        static FastListCore<IUpdateRunner> fixedUpdateRunners;
        static FastListCore<IUpdateRunner> preUpdateRunners;
        static FastListCore<IUpdateRunner> updateRunners;
        static FastListCore<IUpdateRunner> preLateUpdateRunners;
        static FastListCore<IUpdateRunner> postLateUpdateRunners;
        static FastListCore<IUpdateRunner> timeUpdateRunners;

        internal static FastListCore<IUpdateRunner> EmptyList = FastListCore<IUpdateRunner>.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static ref FastListCore<IUpdateRunner> GetRunnerList(PlayerLoopTiming playerLoopTiming)
        {
            // FastListCore<T> must be passed as ref
            switch (playerLoopTiming)
            {
                default:
                    return ref EmptyList;
                case PlayerLoopTiming.Initialization:
                    return ref initializationRunners;
                case PlayerLoopTiming.EarlyUpdate:
                    return ref earlyUpdateRunners;
                case PlayerLoopTiming.FixedUpdate:
                    return ref fixedUpdateRunners;
                case PlayerLoopTiming.PreUpdate:
                    return ref preUpdateRunners;
                case PlayerLoopTiming.Update:
                    return ref updateRunners;
                case PlayerLoopTiming.PreLateUpdate:
                    return ref preLateUpdateRunners;
                case PlayerLoopTiming.PostLateUpdate:
                    return ref postLateUpdateRunners;
                case PlayerLoopTiming.TimeUpdate:
                    return ref timeUpdateRunners;
            };
        }

        static Action<Exception> unhandledException = DefaultUnhandledExceptionHandler;
        static readonly PlayerLoopTiming[] playerLoopTimings = (PlayerLoopTiming[])Enum.GetValues(typeof(PlayerLoopTiming));

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Clear();
        }

        /// <summary>
        /// Set handling of unhandled exceptions.
        /// </summary>
        public static void RegisterUnhandledExceptionHandler(Action<Exception> unhandledExceptionHandler)
        {
            unhandledException = unhandledExceptionHandler;
        }

        /// <summary>
        /// Get handling of unhandled exceptions.
        /// </summary>
        public static Action<Exception> GetUnhandledExceptionHandler()
        {
            return unhandledException;
        }

        static void DefaultUnhandledExceptionHandler(Exception exception)
        {
            Debug.LogException(exception);
        }

        /// <summary>
        /// Cancel all motions.
        /// </summary>
        public static void Clear()
        {
            foreach (var playerLoopTiming in playerLoopTimings)
            {
                var span = GetRunnerList(playerLoopTiming).AsSpan();
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].Reset();
                }
            }
        }

        /// <summary>
        /// Ensures the storage capacity until it reaches at least `capacity`.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        public static void EnsureStorageCapacity<TValue, TOptions, TAdapter>(int capacity)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            foreach (var playerLoopTiming in playerLoopTimings)
            {
                StorageCache<TValue, TOptions, TAdapter>.GetOrCreate(playerLoopTiming).EnsureCapacity(capacity);
            }
#if UNITY_EDITOR
            EditorMotionDispatcher.EnsureStorageCapacity<TValue, TOptions, TAdapter>(capacity);
#endif
        }

        internal static MotionHandle Schedule<TValue, TOptions, TAdapter>(ref MotionBuilder<TValue, TOptions, TAdapter> builder, PlayerLoopTiming playerLoopTiming)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            var storage = StorageCache<TValue, TOptions, TAdapter>.GetOrCreate(playerLoopTiming);
            RunnerCache<TValue, TOptions, TAdapter>.GetOrCreate(playerLoopTiming, storage);
            return storage.Create(ref builder);
        }

        internal static void Update(PlayerLoopTiming playerLoopTiming)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
#endif
            var span = GetRunnerList(playerLoopTiming).AsSpan();
            if (playerLoopTiming == PlayerLoopTiming.FixedUpdate)
            {
                for (int i = 0; i < span.Length; i++) span[i].Update(Time.fixedTimeAsDouble, Time.fixedUnscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
            }
            else
            {
                for (int i = 0; i < span.Length; i++) span[i].Update(Time.timeAsDouble, Time.unscaledTimeAsDouble, Time.realtimeSinceStartupAsDouble);
            }
        }
    }

#if UNITY_EDITOR
    internal static class EditorMotionDispatcher
    {
        static class Cache<TValue, TOptions, TAdapter>
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            static MotionStorage<TValue, TOptions, TAdapter> storage;
            static UpdateRunner<TValue, TOptions, TAdapter> updateRunner;

            public static MotionStorage<TValue, TOptions, TAdapter> GetOrCreateStorage()
            {
                if (storage == null)
                {
                    storage = new MotionStorage<TValue, TOptions, TAdapter>(MotionManager.MotionTypeCount);
                    MotionManager.Register(storage);
                    EnterThePlayModeHelper.Register(storage);
                }
                return storage;
            }

            public static void InitUpdateRunner()
            {
                if (updateRunner == null)
                {
                    var time = EditorApplication.timeSinceStartup;
                    updateRunner = new UpdateRunner<TValue, TOptions, TAdapter>(storage, time, time, time);
                    updateRunners.Add(updateRunner);
                }
            }
        }

        static FastListCore<IUpdateRunner> updateRunners;

        public static MotionHandle Schedule<TValue, TOptions, TAdapter>(ref MotionBuilder<TValue, TOptions, TAdapter> builder)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            var storage = Cache<TValue, TOptions, TAdapter>.GetOrCreateStorage();
            Cache<TValue, TOptions, TAdapter>.InitUpdateRunner();
            return storage.Create(ref builder);
        }

        public static void EnsureStorageCapacity<TValue, TOptions, TAdapter>(int capacity)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            Cache<TValue, TOptions, TAdapter>.GetOrCreateStorage().EnsureCapacity(capacity);
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            var span = updateRunners.AsSpan();
            for (int i = 0; i < span.Length; i++)
            {
                span[i].Update(EditorApplication.timeSinceStartup, EditorApplication.timeSinceStartup, Time.realtimeSinceStartupAsDouble);
            }
        }
    }
#endif
}