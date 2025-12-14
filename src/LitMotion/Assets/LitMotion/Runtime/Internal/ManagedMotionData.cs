#if DEVELOPMENT_BUILD || UNITY_EDITOR
#define LITMOTION_DEBUG
#endif

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace LitMotion
{
    /// <summary>
    /// A structure that holds motion managed data.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public unsafe struct ManagedMotionData
    {
        public bool CancelOnError;
        public bool SkipValuesDuringDelay;
        public byte StateCount;
        public object State0;
        public object State1;
        public object State2;
        public object UpdateAction;
        public Action<int> OnLoopCompleteAction;
        public Action OnCompleteAction;
        public Action OnCancelAction;
        public Action OnStartAction;

#if LITMOTION_DEBUG
        public string DebugName;
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateUnsafe<TValue>(in TValue value) where TValue : unmanaged
        {
            switch (StateCount)
            {
                case 0:
                    UnsafeUtility.As<object, Action<TValue>>(ref UpdateAction)?.Invoke(value);
                    break;
                case 1:
                    UnsafeUtility.As<object, Action<TValue, object>>(ref UpdateAction)?.Invoke(value, State0);
                    break;
                case 2:
                    UnsafeUtility.As<object, Action<TValue, object, object>>(ref UpdateAction)?.Invoke(value, State0, State1);
                    break;
                case 3:
                    UnsafeUtility.As<object, Action<TValue, object, object, object>>(ref UpdateAction)?.Invoke(value, State0, State1, State2);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void InvokeOnCancel()
        {
            try
            {
                OnCancelAction?.Invoke();
            }
            catch (Exception ex)
            {
                MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void InvokeOnComplete()
        {
            try
            {
                OnCompleteAction?.Invoke();
            }
            catch (Exception ex)
            {
                MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void InvokeOnLoopComplete(int completedLoops)
        {
            try
            {
                OnLoopCompleteAction?.Invoke(completedLoops);
            }
            catch (Exception ex)
            {
                MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void InvokeOnStart()
        {
            try
            {
                OnStartAction?.Invoke();
            }
            catch (Exception ex)
            {
                MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
            }
        }
    }
}