#if DEVELOPMENT_BUILD || UNITY_EDITOR
#define LITMOTION_DEBUG
#endif

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// Provides methods for manipulating the motion entity pointed to by MotionHandle.
    /// </summary>
    public static class MotionHandleExtensions
    {
        /// <summary>
        /// Checks if a motion is active.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns>True if motion is active, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsActive(this MotionHandle handle)
        {
            return MotionManager.IsActive(handle);
        }

        /// <summary>
        /// Checks if a motion is currently playing.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns>True if motion is playing, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPlaying(this MotionHandle handle)
        {
            return MotionManager.IsPlaying(handle);
        }


        /// <summary>
        /// Gets the debug name set for the MotionHandle. If not specified, returns the result of ToString().
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns>Debug name</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDebugName(this MotionHandle handle)
        {
#if LITMOTION_DEBUG
            return MotionManager.GetManagedDataRef(handle, false).DebugName ?? handle.ToString();
#else
            return handle.ToString();
#endif
        }

        /// <summary>
        /// Preserves the MotionHandle so that it is not destroyed until Cancel() is explicitly called.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns>Returns itself for method chaining</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MotionHandle Preserve(this MotionHandle handle)
        {
            MotionManager.GetDataRef(handle).State.IsPreserved = true;
            return handle;
        }

        /// <summary>
        /// Complete motion.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Complete(this MotionHandle handle)
        {
            MotionManager.Complete(handle);
        }

        /// <summary>
        /// Attempt to complete the motion.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns>Returns true if the operation was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryComplete(this MotionHandle handle)
        {
            return MotionManager.TryComplete(handle);
        }

        /// <summary>
        /// Cancel motion.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cancel(this MotionHandle handle)
        {
            MotionManager.Cancel(handle);
        }

        /// <summary>
        /// Attempt to cancel the motion.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns>Returns true if the operation was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryCancel(this MotionHandle handle)
        {
            return MotionManager.TryCancel(handle);
        }

        /// <summary>
        /// Add this motion handle to CompositeMotionHandle.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="compositeMotionHandle">target CompositeMotionHandle</param>
        public static MotionHandle AddTo(this MotionHandle handle, CompositeMotionHandle compositeMotionHandle)
        {
            compositeMotionHandle.Add(handle);
            return handle;
        }

        /// <summary>
        /// Link the motion lifecycle to the target object.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="target">Target object</param>
        public static MotionHandle AddTo(this MotionHandle handle, GameObject target)
        {
            GetOrAddComponent<MotionHandleLinker>(target).Register(handle, LinkBehavior.CancelOnDestroy);
            return handle;
        }

        /// <summary>
        /// Link the motion lifecycle to the target object.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="target">Target object</param>
        /// <param name="linkBehaviour">Link behaviour</param>
        public static MotionHandle AddTo(this MotionHandle handle, GameObject target, LinkBehavior linkBehaviour)
        {
            GetOrAddComponent<MotionHandleLinker>(target).Register(handle, linkBehaviour);
            return handle;
        }

        /// <summary>
        /// Link the motion lifecycle to the target object.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="target">Target object</param>
        public static MotionHandle AddTo(this MotionHandle handle, Component target)
        {
            GetOrAddComponent<MotionHandleLinker>(target.gameObject).Register(handle, LinkBehavior.CancelOnDestroy);
            return handle;
        }

        /// <summary>
        /// Link the motion lifecycle to the target object.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="target">Target object</param>
        /// <param name="linkBehaviour">Link behaviour</param>
        public static MotionHandle AddTo(this MotionHandle handle, Component target, LinkBehavior linkBehaviour)
        {
            GetOrAddComponent<MotionHandleLinker>(target.gameObject).Register(handle, linkBehaviour);
            return handle;
        }

#if UNITY_2022_2_OR_NEWER
        /// <summary>
        /// Link the motion lifecycle to the target object.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="target">Target object</param>
        public static MotionHandle AddTo(this MotionHandle handle, MonoBehaviour target)
        {
            target.destroyCancellationToken.Register(() =>
            {
                if (handle.IsActive()) handle.Cancel();
            }, false);
            return handle;
        }
#endif

        static TComponent GetOrAddComponent<TComponent>(GameObject target) where TComponent : Component
        {
            if (!target.TryGetComponent<TComponent>(out var component))
            {
                component = target.AddComponent<TComponent>();
            }
            return component;
        }

        /// <summary>
        /// Convert MotionHandle to IDisposable.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="disposeBehavior">Behavior when disposing</param>
        /// <returns>Disposable motion</returns>
        public static IDisposable ToDisposable(this MotionHandle handle, DisposeBehavior disposeBehavior = DisposeBehavior.Cancel)
        {
            return new MotionHandleDisposable(handle, disposeBehavior);
        }

        /// <summary>
        /// Wait for the motion to finish in a coroutine.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <returns></returns>
        public static IEnumerator ToYieldInstruction(this MotionHandle handle)
        {
            while (handle.IsActive())
            {
                yield return null;
            }
        }

        public static MotionAwaiter GetAwaiter(this MotionHandle handle)
        {
            return new MotionAwaiter(handle);
        }

        /// <summary>
        /// Convert motion handle to ValueTask.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public static ValueTask ToValueTask(this MotionHandle handle, CancellationToken cancellationToken = default)
        {
            if (!handle.IsActive()) return default;
            var source = ValueTaskMotionTaskSource.Create(handle, CancelBehavior.Cancel, true, cancellationToken, out var token);
            return new ValueTask(source, token);
        }

        /// <summary>
        /// Convert motion handle to ValueTask.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="cancelBehavior">Behavior when canceling</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public static ValueTask ToValueTask(this MotionHandle handle, CancelBehavior cancelBehavior, CancellationToken cancellationToken = default)
        {
            if (!handle.IsActive()) return default;
            var source = ValueTaskMotionTaskSource.Create(handle, cancelBehavior, true, cancellationToken, out var token);
            return new ValueTask(source, token);
        }

        /// <summary>
        /// Convert motion handle to ValueTask.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="cancelBehavior">Behavior when canceling</param>
        /// <param name="cancelAwaitOnMotionCanceled">Whether to link MotionHandle.Cancel() to task cancellation</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public static ValueTask ToValueTask(this MotionHandle handle, CancelBehavior cancelBehavior, bool cancelAwaitOnMotionCanceled, CancellationToken cancellationToken = default)
        {
            if (!handle.IsActive()) return default;
            var source = ValueTaskMotionTaskSource.Create(handle, cancelBehavior, cancelAwaitOnMotionCanceled, cancellationToken, out var token);
            return new ValueTask(source, token);
        }

#if UNITY_2023_1_OR_NEWER
        /// <summary>
        /// Convert motion handle to Awaitable.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public static Awaitable ToAwaitable(this MotionHandle handle, CancellationToken cancellationToken = default)
        {
            if (!handle.IsActive()) return AwaitableMotionTaskSource.CompletedSource.Awaitable;
            return AwaitableMotionTaskSource.Create(handle, CancelBehavior.Cancel, true, cancellationToken).Awaitable;
        }

        /// <summary>
        /// Convert motion handle to Awaitable.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="cancelBehavior">Behavior when canceling</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public static Awaitable ToAwaitable(this MotionHandle handle, CancelBehavior cancelBehavior, CancellationToken cancellationToken = default)
        {
            if (!handle.IsActive()) return AwaitableMotionTaskSource.CompletedSource.Awaitable;
            return AwaitableMotionTaskSource.Create(handle, cancelBehavior, true, cancellationToken).Awaitable;
        }

        /// <summary>
        /// Convert motion handle to Awaitable.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="cancelBehavior">Behavior when canceling</param>
        /// <param name="cancelAwaitOnMotionCanceled">Whether to link MotionHandle.Cancel() to task cancellation</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns></returns>
        public static Awaitable ToAwaitable(this MotionHandle handle, CancelBehavior cancelBehavior, bool cancelAwaitOnMotionCanceled, CancellationToken cancellationToken = default)
        {
            if (!handle.IsActive()) return AwaitableMotionTaskSource.CompletedSource.Awaitable;
            return AwaitableMotionTaskSource.Create(handle, cancelBehavior, cancelAwaitOnMotionCanceled, cancellationToken).Awaitable;
        }
#endif

    }

    /// <summary>
    /// Specifies the behavior when the motion converted to IDisposable is disposed.
    /// </summary>
    public enum DisposeBehavior
    {
        Cancel,
        Complete,
    }

    internal sealed class MotionHandleDisposable : IDisposable
    {
        public MotionHandleDisposable(MotionHandle handle, DisposeBehavior disposeBehavior)
        {
            this.handle = handle;
            this.disposeBehavior = disposeBehavior;
        }

        public readonly MotionHandle handle;
        public readonly DisposeBehavior disposeBehavior;

        public void Dispose()
        {
            switch (disposeBehavior)
            {
                case DisposeBehavior.Cancel:
                    handle.TryCancel();
                    break;
                case DisposeBehavior.Complete:
                    handle.TryComplete();
                    break;
            }
        }
    }
}