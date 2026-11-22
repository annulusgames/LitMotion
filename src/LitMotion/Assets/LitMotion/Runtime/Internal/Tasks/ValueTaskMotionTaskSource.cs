using System;
using System.Threading;
using System.Threading.Tasks.Sources;
using LitMotion.Collections;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion
{
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Managed object pool; safe to reuse between Play mode sessions.
#endif
    internal sealed class ValueTaskMotionTaskSource : MotionTaskSourceBase, IValueTaskSource, ILinkedPoolNode<ValueTaskMotionTaskSource>
    {
        static LinkedPool<ValueTaskMotionTaskSource> pool;

        ValueTaskMotionTaskSource nextNode;
        public ref ValueTaskMotionTaskSource NextNode => ref nextNode;

        ManualResetValueTaskSourceCore<object> core;

        static ValueTaskMotionTaskSource FromCanceled(out short token)
        {
            if (canceledSource == null)
            {
                canceledSource = new();
            }

            canceledSource.core.Reset();
            canceledSource.core.SetException(new OperationCanceledException());

            token = canceledSource.Version;
            return canceledSource;
        }
        static ValueTaskMotionTaskSource canceledSource;

        ValueTaskMotionTaskSource() : base() { }

        public static IValueTaskSource Create(MotionHandle motionHandle, CancelBehavior cancelBehavior, bool cancelAwaitOnMotionCanceled, CancellationToken cancellationToken, out short token)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                OnCanceledTokenReceived(motionHandle, cancelBehavior);
                return FromCanceled(out token);
            }

            if (!pool.TryPop(out var result))
            {
                result = new ValueTaskMotionTaskSource();
            }

            result.Initialize(motionHandle, cancelBehavior, cancelAwaitOnMotionCanceled, cancellationToken);

            token = result.core.Version;
            return result;
        }

        public short Version => core.Version;

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

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return core.GetStatus(token);
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            core.OnCompleted(continuation, state, token, flags);
        }

        bool TryReturn()
        {
            core.Reset();

            DisposeRegistration();
            RestoreOriginalCallback();
            ResetFields();

            return pool.TryPush(this);
        }

        protected override void SetTaskCanceled(CancellationToken cancellationToken)
        {
            core.SetException(new OperationCanceledException());
        }

        protected override void SetTaskCompleted()
        {
            core.SetResult(null);
        }
    }
}