#if LITMOTION_SUPPORT_UNITASK
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion
{
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Managed object pool registered with UniTask's TaskPool.
#endif
    internal sealed class UniTaskMotionTaskSource : MotionTaskSourceBase, IUniTaskSource, ITaskPoolNode<UniTaskMotionTaskSource>
    {
        static UniTaskMotionTaskSource()
        {
            TaskPool.RegisterSizeGetter(typeof(UniTaskMotionTaskSource), () => pool.Size);
        }

        UniTaskMotionTaskSource() : base() { }

        static TaskPool<UniTaskMotionTaskSource> pool;
        UniTaskMotionTaskSource nextNode;
        public ref UniTaskMotionTaskSource NextNode => ref nextNode;

        UniTaskCompletionSourceCore<AsyncUnit> core;

        public static IUniTaskSource Create(MotionHandle motionHandle, CancelBehavior cancelBehavior, bool cancelAwaitOnMotionCanceled, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                OnCanceledTokenReceived(motionHandle, cancelBehavior);
                return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
            }

            if (!pool.TryPop(out var result))
            {
                result = new UniTaskMotionTaskSource();
            }

            result.Initialize(motionHandle, cancelBehavior, cancelAwaitOnMotionCanceled, cancellationToken);

            TaskTracker.TrackActiveTask(result, 3);

            token = result.core.Version;
            return result;
        }

        protected override void SetTaskCanceled(CancellationToken cancellationToken)
        {
            core.TrySetCanceled(cancellationToken);
        }

        protected override void SetTaskCompleted()
        {
            core.TrySetResult(AsyncUnit.Default);
        }

        public void GetResult(short token)
        {
            try
            {
                core.GetResult(token);
            }
            finally
            {
                TryReturn();
            }
        }

        public UniTaskStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public UniTaskStatus UnsafeGetStatus()
        {
            return core.UnsafeGetStatus();
        }

        public void OnCompleted(Action<object> continuation, object state, short token)
        {
            core.OnCompleted(continuation, state, token);
        }

        bool TryReturn()
        {
            TaskTracker.RemoveTracking(this);
            core.Reset();

            DisposeRegistration();
            RestoreOriginalCallback();
            ResetFields();

            return pool.TryPush(this);
        }
    }
}
#endif