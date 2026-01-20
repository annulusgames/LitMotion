using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace LitMotion
{
    internal interface IUpdateRunner
    {
        IMotionStorage Storage { get; }
        public void Update(double time, double unscaledTime, double realtime);
        public void Reset();
    }

    internal sealed class UpdateRunner<TValue, TOptions, TAdapter> : IUpdateRunner
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        public UpdateRunner(MotionStorage<TValue, TOptions, TAdapter> storage, double time, double unscaledTime, double realtime)
        {
            this.storage = storage;
            prevTime = time;
            prevUnscaledTime = unscaledTime;
            prevRealtime = realtime;
        }

        readonly MotionStorage<TValue, TOptions, TAdapter> storage;

        double prevTime;
        double prevUnscaledTime;
        double prevRealtime;

        public MotionStorage<TValue, TOptions, TAdapter> Storage => storage;
        IMotionStorage IUpdateRunner.Storage => storage;

        public unsafe void Update(double time, double unscaledTime, double realtime)
        {
            var count = storage.Count;
            using var output = new NativeArray<TValue>(count, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            using var completedIndexList = new NativeList<int>(count, Allocator.TempJob);

            var deltaTime = time - prevTime;
            var unscaledDeltaTime = unscaledTime - prevUnscaledTime;
            var realDeltaTime = realtime - prevRealtime;
            prevTime = time;
            prevUnscaledTime = unscaledTime;
            prevRealtime = realtime;

            fixed (MotionData<TValue, TOptions>* dataPtr = storage.GetDataSpan())
            {
                // update data
                var job = new MotionUpdateJob<TValue, TOptions, TAdapter>()
                {
                    DataPtr = dataPtr,
                    DeltaTime = deltaTime,
                    UnscaledDeltaTime = unscaledDeltaTime,
                    RealDeltaTime = realDeltaTime,
                    Output = output,
                    CompletedIndexList = completedIndexList.AsParallelWriter()
                };
                job.Schedule(count, 16).Complete();

                // invoke delegates
                var managedDataSpan = storage.GetManagedDataSpan();
                var outputPtr = (TValue*)output.GetUnsafePtr();
                for (int i = 0; i < managedDataSpan.Length; i++)
                {
                    var currentDataPtr = dataPtr + i;
                    ref var state = ref currentDataPtr->Core.State;

                    if (state.IsInSequence) continue;

                    var status = state.Status;
                    ref var managedData = ref managedDataSpan[i];
                    
                    bool isPlaying   = status == MotionStatus.Playing;
                    bool isCompleted = status == MotionStatus.Completed;
                    bool isDelayed   = status == MotionStatus.Delayed;
                    bool canUpdate = isPlaying || isCompleted || (isDelayed && !managedData.SkipValuesDuringDelay);
                    bool loopComplete = (isPlaying || isCompleted || isDelayed) && state.WasLoopCompleted;
                    if (canUpdate)
                    {
                        try
                        {
                            managedData.UpdateUnsafe(outputPtr[i]);
                        }
                        catch (Exception ex)
                        {
                            MotionDispatcher.GetUnhandledExceptionHandler()?.Invoke(ex);
                            if (managedData.CancelOnError)
                            {
                                state.Status = MotionStatus.Canceled;
                                managedData.OnCancelAction?.Invoke();
                            }
                        }
                    }
                    if (loopComplete) managedData.InvokeOnLoopComplete(state.CompletedLoops);
                    if (isCompleted && state.WasStatusChanged) managedData.InvokeOnComplete();
                }
            }

            storage.RemoveAll(completedIndexList);
        }

        public void Reset()
        {
            prevTime = 0;
            prevUnscaledTime = 0;
            prevRealtime = 0;
            storage.Reset();
        }
    }
}
