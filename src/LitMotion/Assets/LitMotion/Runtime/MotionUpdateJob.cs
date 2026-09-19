using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace LitMotion
{
    /// <summary>
    /// A job that updates the status of the motion data and outputs the current value.
    /// </summary>
    /// <typeparam name="TValue">The type of value to animate</typeparam>
    /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
    /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
    [BurstCompile]
    public unsafe struct MotionUpdateJob<TValue, TOptions, TAdapter> : IJobParallelFor
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        [NativeDisableUnsafePtrRestriction] internal MotionData<TValue, TOptions>* DataPtr;
        [ReadOnly] public double DeltaTime;
        [ReadOnly] public double UnscaledDeltaTime;
        [ReadOnly] public double RealDeltaTime;

        [WriteOnly] public NativeList<int>.ParallelWriter CompletedIndexList;
        [WriteOnly] public NativeArray<TValue> Output;

        public void Execute([AssumeRange(0, int.MaxValue)] int index)
        {
            var ptr = DataPtr + index;
            ref var state = ref ptr->Core.State;
            ref var parameters = ref ptr->Core.Parameters;

            if (Hint.Likely(state.Status is MotionStatus.Scheduled or MotionStatus.Delayed or MotionStatus.Playing) ||
                Hint.Unlikely(state.IsPreserved && state.Status is MotionStatus.Completed))
            {
                if (Hint.Unlikely(state.IsInSequence)) return;

                var deltaTime = parameters.TimeKind switch
                {
                    MotionTimeKind.Time => DeltaTime,
                    MotionTimeKind.UnscaledTime => UnscaledDeltaTime,
                    MotionTimeKind.Realtime => RealDeltaTime,
                    _ => default
                };

                deltaTime *= state.PlaybackSpeed;
                var time = state.Time + deltaTime;
                ptr->Update<TAdapter>(time, deltaTime, out var result);
                Output[index] = result;
            }
            else if ((!state.IsPreserved && state.Status is MotionStatus.Completed) || state.Status is MotionStatus.Canceled)
            {
                CompletedIndexList.AddNoResize(index);
                state.Status = MotionStatus.Disposed;
            }
        }
    }
}