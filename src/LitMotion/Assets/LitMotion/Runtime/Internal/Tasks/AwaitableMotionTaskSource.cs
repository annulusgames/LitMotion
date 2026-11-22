#if UNITY_2023_1_OR_NEWER
using System.Threading;
using UnityEngine;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion
{
    // TODO: use object pool

#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Stateless singletons; keep them allocated between Play mode sessions.
#endif
    internal sealed class AwaitableMotionTaskSource : MotionTaskSourceBase
    {
        public static AwaitableMotionTaskSource CompletedSource
        {
            get
            {
                if (completedSource == null)
                {
                    completedSource = new();
                }
                completedSource.core.Reset();
                completedSource.core.SetResult();
                return completedSource;
            }
        }
        static AwaitableMotionTaskSource completedSource;

        public static AwaitableMotionTaskSource CanceledSource
        {
            get
            {
                if (canceledSource == null)
                {
                    canceledSource = new();
                }
                canceledSource.core.Reset();
                canceledSource.core.SetCanceled();
                return canceledSource;
            }
        }
        static AwaitableMotionTaskSource canceledSource;

        readonly AwaitableCompletionSource core = new();

        public Awaitable Awaitable => core.Awaitable;

        AwaitableMotionTaskSource() : base() { }

        public static AwaitableMotionTaskSource Create(MotionHandle motionHandle, CancelBehavior cancelBehavior, bool cancelAwaitOnMotionCanceled, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                OnCanceledTokenReceived(motionHandle, cancelBehavior);
                return CanceledSource;
            }

            var result = new AwaitableMotionTaskSource();
            result.Initialize(motionHandle, cancelBehavior, cancelAwaitOnMotionCanceled, cancellationToken);
            return result;
        }

        protected override void SetTaskCanceled(CancellationToken cancellationToken)
        {
            core.SetCanceled();
        }

        protected override void SetTaskCompleted()
        {
            core.SetResult();
        }
    }
}

#endif
