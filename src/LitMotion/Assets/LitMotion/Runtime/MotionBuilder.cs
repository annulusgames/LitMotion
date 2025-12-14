#if DEVELOPMENT_BUILD || UNITY_EDITOR
#define LITMOTION_DEBUG
#endif

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LitMotion
{
    internal unsafe sealed class MotionBuilderBuffer<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        static MotionBuilderBuffer<TValue, TOptions> PoolRoot = new();

        public static MotionBuilderBuffer<TValue, TOptions> Rent()
        {
            MotionBuilderBuffer<TValue, TOptions> result;
            if (PoolRoot == null)
            {
                result = new();
            }
            else
            {
                result = PoolRoot;
                PoolRoot = PoolRoot.NextNode;
                result.NextNode = null;
            }
            return result;
        }

        public static void Return(MotionBuilderBuffer<TValue, TOptions> buffer)
        {
            buffer.Version++;
            buffer.ImmediateBind = true;

            buffer.StartValue = default;
            buffer.EndValue = default;
            buffer.Options = default;

            buffer.Duration = default;
            buffer.Ease = default;
            buffer.AnimationCurve = default;
            buffer.TimeKind = default;
            buffer.Delay = default;
            buffer.Loops = 1;
            buffer.LoopType = default;

            buffer.State0 = default;
            buffer.State1 = default;
            buffer.State2 = default;
            buffer.StateCount = default;

            buffer.UpdateAction = default;
            buffer.OnLoopCompleteAction = default;
            buffer.OnCompleteAction = default;
            buffer.OnCancelAction = default;
            buffer.OnStartAction = default;

            buffer.CancelOnError = default;
            buffer.SkipValuesDuringDelay = default;

            buffer.Scheduler = default;

#if LITMOTION_DEBUG
            buffer.DebugName = default;
#endif

            if (buffer.Version != ushort.MaxValue)
            {
                buffer.NextNode = PoolRoot;
                PoolRoot = buffer;
            }
        }

        public ushort Version;
        public MotionBuilderBuffer<TValue, TOptions> NextNode;
        public TValue StartValue;
        public TValue EndValue;
        public TOptions Options;
        public float Duration;
        public Ease Ease;
        public MotionTimeKind TimeKind;
        public float Delay;
        public int Loops = 1;
        public DelayType DelayType;
        public LoopType LoopType;
        public bool CancelOnError;
        public bool SkipValuesDuringDelay;
        public bool ImmediateBind = true;

        public object State0;
        public object State1;
        public object State2;
        public byte StateCount;
        public object UpdateAction;
        public Action<int> OnLoopCompleteAction;
        public Action OnCompleteAction;
        public Action OnCancelAction;
        public Action OnStartAction;
        public AnimationCurve AnimationCurve;
        public IMotionScheduler Scheduler;

#if LITMOTION_DEBUG
        public string DebugName;
#endif
    }

    /// <summary>
    /// Supports construction, scheduling, and binding of motion entities.
    /// </summary>
    /// <typeparam name="TValue">The type of value to animate</typeparam>
    /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
    /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
    public struct MotionBuilder<TValue, TOptions, TAdapter> : IDisposable
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        internal MotionBuilder(MotionBuilderBuffer<TValue, TOptions> buffer)
        {
            this.buffer = buffer;
            this.version = buffer.Version;
        }

        internal ushort version;
        internal MotionBuilderBuffer<TValue, TOptions> buffer;

        /// <summary>
        /// Specify easing for motion.
        /// </summary>
        /// <param name="ease">The type of easing</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithEase(Ease ease)
        {
            CheckEaseType(ease);
            CheckBuffer();
            buffer.Ease = ease;
            return this;
        }

        /// <summary>
        /// Specify easing for motion.
        /// </summary>
        /// <param name="animationCurve">Animation curve</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithEase(AnimationCurve animationCurve)
        {
            CheckBuffer();
            buffer.AnimationCurve = animationCurve;
            buffer.Ease = Ease.CustomAnimationCurve;
            return this;
        }

        /// <summary>
        /// Specify the delay time when the motion starts.
        /// </summary>
        /// <param name="delay">Delay time (seconds)</param>
        /// <param name="delayType">Delay type</param>
        /// <param name="skipValuesDuringDelay">Whether to skip updating values during the delay time</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithDelay(float delay, DelayType delayType = DelayType.FirstLoop, bool skipValuesDuringDelay = true)
        {
            CheckBuffer();
            buffer.Delay = delay;
            buffer.DelayType = delayType;
            buffer.SkipValuesDuringDelay = skipValuesDuringDelay;
            return this;
        }

        /// <summary>
        /// Specify the number of times the motion is repeated. If specified as less than 0, the motion will continue to play until manually completed or canceled.
        /// </summary>
        /// <param name="loops">Number of loops</param>
        /// <param name="loopType">Behavior at the end of each loop</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            CheckBuffer();
            buffer.Loops = loops;
            buffer.LoopType = loopType;
            return this;
        }

        /// <summary>
        /// Specify special parameters for each motion data.
        /// </summary>
        /// <param name="options">Option value to specify</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithOptions(TOptions options)
        {
            CheckBuffer();
            buffer.Options = options;
            return this;
        }

        /// <summary>
        /// Specify the callback when canceled.
        /// </summary>
        /// <param name="callback">Callback when canceled</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithOnCancel(Action callback)
        {
            CheckBuffer();
            buffer.OnCancelAction += callback;
            return this;
        }

        /// <summary>
        /// Specify the callback when playback ends.
        /// </summary>
        /// <param name="callback">Callback when playback ends</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithOnComplete(Action callback)
        {
            CheckBuffer();
            buffer.OnCompleteAction += callback;
            return this;
        }

        /// <summary>
        /// Specify a callback to be performed when each loop finishes.
        /// </summary>
        /// <param name="callback">Callback to be performed when each loop finishes.</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithOnLoopComplete(Action<int> callback)
        {
            CheckBuffer();
            buffer.OnLoopCompleteAction += callback;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithOnStart(Action callback)
        {
            CheckBuffer();
            buffer.OnStartAction += callback;
            return this;
        }

        /// <summary>
        /// Cancel Motion when an exception occurs during Bind processing.
        /// </summary>
        /// <param name="cancelOnError">Whether to cancel on error</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithCancelOnError(bool cancelOnError = true)
        {
            CheckBuffer();
            buffer.CancelOnError = cancelOnError;
            return this;
        }

        /// <summary>
        /// Bind values ​​immediately when scheduling the motion.
        /// </summary>
        /// <param name="immediateBind">Whether to bind on sheduling</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithImmediateBind(bool immediateBind = true)
        {
            CheckBuffer();
            buffer.ImmediateBind = immediateBind;
            return this;
        }

        /// <summary>
        /// Specifies the scheduler that schedule the motion.
        /// </summary>
        /// <param name="scheduler">Scheduler</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithScheduler(IMotionScheduler scheduler)
        {
            CheckBuffer();
            buffer.Scheduler = scheduler;
            return this;
        }

        /// <summary>
        /// Specifies the name that will be displayed in the debugger.
        /// </summary>
        /// <param name="debugName">Debug name</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionBuilder<TValue, TOptions, TAdapter> WithDebugName(string debugName)
        {
#if LITMOTION_DEBUG
            CheckBuffer();
            buffer.DebugName = debugName;
#endif
            return this;
        }

        /// <summary>
        /// Create motion and play it without binding it to a specific object.
        /// </summary>
        /// <returns>Handle of the created motion data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle RunWithoutBinding()
        {
            CheckBuffer();
            return ScheduleMotion();
        }

        /// <summary>
        /// Create motion and bind it to a specific object, property, etc.
        /// </summary>
        /// <param name="action">Action that handles binding</param>
        /// <returns>Handle of the created motion data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle Bind(Action<TValue> action)
        {
            CheckBuffer();
            SetCallbackData(action);
            return ScheduleMotion();
        }

        /// <summary>
        /// Create motion and bind it to a specific object. Unlike the regular Bind method, it avoids allocation by closure by passing an object.
        /// </summary>
        /// <typeparam name="TState">Type of state</typeparam>
        /// <param name="state">Motion state</param>
        /// <param name="action">Action that handles binding</param>
        /// <returns>Handle of the created motion data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle Bind<TState>(TState state, Action<TValue, TState> action)
            where TState : class
        {
            CheckBuffer();
            SetCallbackData(state, action);
            return ScheduleMotion();
        }

        /// <summary>
        /// Create motion and bind it to a specific object. Unlike the regular Bind method, it avoids allocation by closure by passing an object.
        /// </summary>
        /// <typeparam name="TState0">Type of state</typeparam>
        /// <typeparam name="TState1">Type of state</typeparam>
        /// <param name="state0">Motion state</param>
        /// <param name="state1">Motion state</param>
        /// <param name="action">Action that handles binding</param>
        /// <returns>Handle of the created motion data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle Bind<TState0, TState1>(TState0 state0, TState1 state1, Action<TValue, TState0, TState1> action)
            where TState0 : class
            where TState1 : class
        {
            CheckBuffer();
            SetCallbackData(state0, state1, action);
            return ScheduleMotion();
        }

        /// <summary>
        /// Create motion and bind it to a specific object. Unlike the regular Bind method, it avoids allocation by closure by passing an object.
        /// </summary>
        /// <typeparam name="TState0">Type of state</typeparam>
        /// <typeparam name="TState1">Type of state</typeparam>
        /// <typeparam name="TState2">Type of state</typeparam>
        /// <param name="state0">Motion state</param>
        /// <param name="state1">Motion state</param>
        /// <param name="state2">Motion state</param>
        /// <param name="action">Action that handles binding</param>
        /// <returns>Handle of the created motion data.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle Bind<TState0, TState1, TState2>(TState0 state0, TState1 state1, TState2 state2, Action<TValue, TState0, TState1, TState2> action)
            where TState0 : class
            where TState1 : class
            where TState2 : class
        {
            CheckBuffer();
            SetCallbackData(state0, state1, state2, action);
            return ScheduleMotion();
        }

        /// <summary>
        /// Creates a MotionSettings from the values ​​set in the builder.
        /// </summary>
        /// <returns>Configured MotionSettings</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionSettings<TValue, TOptions> ToMotionSettings()
        {
            CheckBuffer();
            return new MotionSettings<TValue, TOptions>()
            {
                StartValue = buffer.StartValue,
                EndValue = buffer.EndValue,
                Duration = buffer.Duration,
                Options = buffer.Options,
                Ease = buffer.Ease,
                CustomEaseCurve = buffer.AnimationCurve,
                Delay = buffer.Delay,
                DelayType = buffer.DelayType,
                Loops = buffer.Loops,
                LoopType = buffer.LoopType,
                CancelOnError = buffer.CancelOnError,
                SkipValuesDuringDelay = buffer.SkipValuesDuringDelay,
                ImmediateBind = buffer.ImmediateBind,
                Scheduler = buffer.Scheduler,
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal MotionHandle ScheduleMotion()
        {
            MotionHandle handle;

            if (buffer.Scheduler == null)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    handle = EditorMotionDispatcher.Schedule(ref this);
                }
                else if (MotionScheduler.DefaultScheduler == MotionScheduler.Update) // avoid virtual method call
                {
                    handle = MotionDispatcher.Schedule(ref this, PlayerLoopTiming.Update);
                }
                else
                {
                    handle = MotionScheduler.DefaultScheduler.Schedule(ref this);
                }
#else
                if (MotionScheduler.DefaultScheduler == MotionScheduler.Update) // avoid virtual method call
                {
                    handle = MotionDispatcher.Schedule(ref this, PlayerLoopTiming.Update);
                }
                else
                {
                    handle = MotionScheduler.DefaultScheduler.Schedule(ref this);
                }
#endif
            }
            else
            {
                handle = buffer.Scheduler.Schedule(ref this);
            }

            if (MotionDebugger.Enabled)
            {
                MotionDebugger.AddTracking(handle, buffer.Scheduler);
            }

            Dispose();

            return handle;
        }

        /// <summary>
        /// Dispose this builder. You need to call this manually after calling Preserve or if you have never created a motion data.
        /// </summary>
        public void Dispose()
        {
            if (buffer == null) return;
            MotionBuilderBuffer<TValue, TOptions>.Return(buffer);
            buffer = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly void SetCallbackData(Action<TValue> action)
        {
            buffer.StateCount = 0;
            buffer.UpdateAction = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly void SetCallbackData<TState>(TState state, Action<TValue, TState> action)
            where TState : class
        {
            buffer.StateCount = 1;
            buffer.State0 = state;
            buffer.UpdateAction = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly void SetCallbackData<TState0, TState1>(TState0 state0, TState1 state1, Action<TValue, TState0, TState1> action)
            where TState0 : class
            where TState1 : class
        {
            buffer.StateCount = 2;
            buffer.State0 = state0;
            buffer.State1 = state1;
            buffer.UpdateAction = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal readonly void SetCallbackData<TState0, TState1, TState2>(TState0 state0, TState1 state1, TState2 state2, Action<TValue, TState0, TState1, TState2> action)
            where TState0 : class
            where TState1 : class
            where TState2 : class
        {
            buffer.StateCount = 3;
            buffer.State0 = state0;
            buffer.State1 = state1;
            buffer.State2 = state2;
            buffer.UpdateAction = action;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckBuffer()
        {
            if (buffer == null || buffer.Version != version) throw new InvalidOperationException("MotionBuilder is either not initialized or has already run a Build (or Bind).");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckEaseType(Ease ease)
        {
            if (ease is Ease.CustomAnimationCurve) throw new ArgumentException($"Ease.{ease} cannot be specified directly.");
        }
    }
}