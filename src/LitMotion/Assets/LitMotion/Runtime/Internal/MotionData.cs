using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LitMotion.Collections;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;

namespace LitMotion
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MotionData
    {
        public struct MotionState
        {
            public MotionStatus Status;
            public MotionStatus PrevStatus;
            public bool IsPreserved;
            public bool IsInSequence;

            public ushort CompletedLoops;
            public ushort PrevCompletedLoops;

            public double Time;
            public float PlaybackSpeed;

            public readonly bool WasStatusChanged => Status != PrevStatus;
            public readonly bool WasLoopCompleted => CompletedLoops > PrevCompletedLoops;

        }

        public struct MotionParameters
        {
            public float Duration;
            public Ease Ease;
#if LITMOTION_COLLECTIONS_2_0_OR_NEWER
            public NativeAnimationCurve AnimationCurve;
#else
            public UnsafeAnimationCurve AnimationCurve;
#endif
            public MotionTimeKind TimeKind;
            public float Delay;
            public int Loops;
            public DelayType DelayType;
            public LoopType LoopType;

            public readonly double TotalDuration
            {
                get
                {
                    if (Loops < 0) return double.PositiveInfinity;
                    return Delay * (DelayType == DelayType.EveryLoop ? Loops : 1) + Duration * Loops;
                }
            }
        }

        public MotionState State;
        public MotionParameters Parameters;
        public double CurrentLoopStartTime;

        public readonly double TimeSinceStart => State.Time - Parameters.Delay;

        public void UpdateDurationBasedState(double time, out float progress)
        {
            State.PrevCompletedLoops = State.CompletedLoops;
            State.PrevStatus = State.Status;

            State.Time = time;
            time = math.max(time, 0.0);

            double t;
            bool isCompleted;
            bool isDelayed;
            int completedLoops;
            int clampedCompletedLoops;

            if (Hint.Unlikely(Parameters.Duration <= 0f))
            {
                if (Parameters.DelayType == DelayType.FirstLoop || Parameters.Delay == 0f)
                {
                    isCompleted = Parameters.Loops >= 0 && TimeSinceStart > 0f;
                    if (isCompleted)
                    {
                        t = 1f;
                        completedLoops = Parameters.Loops;
                    }
                    else
                    {
                        t = 0f;
                        completedLoops = TimeSinceStart < 0f ? -1 : 0;
                    }
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isDelayed = TimeSinceStart < 0;
                }
                else
                {
                    completedLoops = (int)math.floor(time / Parameters.Delay);
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = Parameters.Loops >= 0 && clampedCompletedLoops > Parameters.Loops - 1;
                    isDelayed = !isCompleted;
                    t = isCompleted ? 1f : 0f;
                }
            }
            else
            {
                if (Parameters.DelayType == DelayType.FirstLoop)
                {
                    completedLoops = (int)math.floor(TimeSinceStart / Parameters.Duration);
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = Parameters.Loops >= 0 && clampedCompletedLoops > Parameters.Loops - 1;
                    isDelayed = TimeSinceStart < 0f;

                    if (isCompleted)
                    {
                        t = 1f;
                    }
                    else
                    {
                        var currentLoopTime = TimeSinceStart - Parameters.Duration * clampedCompletedLoops;
                        t = math.clamp(currentLoopTime / Parameters.Duration, 0f, 1f);
                    }
                }
                else
                {
                    var currentLoopTime = math.fmod(time, Parameters.Duration + Parameters.Delay) - Parameters.Delay;
                    completedLoops = (int)math.floor(time / (Parameters.Duration + Parameters.Delay));
                    clampedCompletedLoops = GetClampedCompletedLoops(completedLoops);
                    isCompleted = Parameters.Loops >= 0 && clampedCompletedLoops > Parameters.Loops - 1;
                    isDelayed = currentLoopTime < 0;

                    if (isCompleted)
                    {
                        t = 1f;
                    }
                    else
                    {
                        t = math.clamp(currentLoopTime / Parameters.Duration, 0f, 1f);
                    }
                }
            }

            State.CompletedLoops = (ushort)clampedCompletedLoops;

            switch (Parameters.LoopType)
            {
                default:
                case LoopType.Restart:
                    progress = GetEasedValue((float)t);
                    break;
                case LoopType.Flip:
                    progress = GetEasedValue((float)t);
                    if ((clampedCompletedLoops + (int)t) % 2 == 1) progress = 1f - progress;
                    break;
                case LoopType.Incremental:
                    progress = GetEasedValue(1f) * clampedCompletedLoops + GetEasedValue((float)math.fmod(t, 1f));
                    break;
                case LoopType.Yoyo:
                    progress = (clampedCompletedLoops + (int)t) % 2 == 1
                        ? GetEasedValue((float)(1f - t))
                        : GetEasedValue((float)t);
                    break;
            }

            if (isCompleted)
            {
                State.Status = MotionStatus.Completed;
            }
            else if (isDelayed || State.Time < 0)
            {
                State.Status = MotionStatus.Delayed;
            }
            else
            {
                State.Status = MotionStatus.Playing;
            }
        }

        public void UpdateIterationState(double time, double deltaTime,bool isOnceCompleted)
        {
            State.PrevCompletedLoops = State.CompletedLoops;
            State.PrevStatus = State.Status;
            State.Time = time;
            bool isDelayed;
            if(time == 0 || (isOnceCompleted && State.CompletedLoops <= Parameters.Loops - 1))
            {
                CurrentLoopStartTime = time;
            }
            if (Parameters.DelayType == DelayType.FirstLoop)
                isDelayed = TimeSinceStart < 0f;
            else
            {
                isDelayed = State.Time - CurrentLoopStartTime - Parameters.Delay < 0f;
            }
                
            if (isOnceCompleted)
            {
                State.CompletedLoops++;
            }
            if (isOnceCompleted && Parameters.Loops >= 0 && State.CompletedLoops > Parameters.Loops - 1)
            {
                State.Status = MotionStatus.Completed;
            }
            else if (isDelayed || State.Time < 0)
            {
                State.Status = MotionStatus.Delayed;
            }
            else
            {
                State.Status = MotionStatus.Playing;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete(out float progress)
        {
            State.Status = MotionStatus.Completed;
            State.Time = Parameters.TotalDuration;
            State.CompletedLoops = (ushort)Parameters.Loops;

            progress = GetEasedValue(Parameters.LoopType switch
            {
                LoopType.Restart => 1f,
                LoopType.Flip or LoopType.Yoyo => Parameters.Loops % 2 == 0 ? 0f : 1f,
                LoopType.Incremental => Parameters.Loops,
                _ => 1f
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly int GetClampedCompletedLoops(int completedLoops)
        {
            return Parameters.Loops < 0
                ? math.max(0, completedLoops)
                : math.clamp(completedLoops, 0, Parameters.Loops);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly float GetEasedValue(float value)
        {
            return Parameters.Ease switch
            {
                Ease.CustomAnimationCurve => Parameters.AnimationCurve.Evaluate(value),
                _ => EaseUtility.Evaluate(value, Parameters.Ease)
            };
        }
    }

    /// <summary>
    /// A structure representing motion data.
    /// </summary>
    /// <typeparam name="TValue">The type of value to animate</typeparam>
    /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MotionData<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        // Because of pointer casting, this field must always be placed at the beginning.
        public MotionData Core;

        public TValue StartValue;
        public TValue EndValue;
        public TOptions Options;

        public void Update<TAdapter>(double time, double deltaTime, out TValue result)
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            bool isDurationBased = default(TAdapter).IsDurationBased;
            if (isDurationBased)
            {
                Core.UpdateDurationBasedState(time, out var progress);
                result = default(TAdapter).Evaluate(ref StartValue, ref EndValue, ref Options, new MotionEvaluationContext()
                {
                    Progress = progress,
                    Time = time,
                    DeltaTime = deltaTime,
                });
            }
            else
            {
                bool isOnceCompleted = false;
                if (time <= 0 || Core.State.Status == MotionStatus.Scheduled)
                    Core.UpdateIterationState(time, deltaTime, false);

                if (Core.State.Status == MotionStatus.Delayed)
                    result = StartValue;
                else if (Core.State.Status == MotionStatus.Completed)
                    result = EndValue;
                else
                {
                    if (Core.Parameters.LoopType == LoopType.Restart)
                    {
                        result = default(TAdapter).Evaluate(ref StartValue, ref EndValue, ref Options, new MotionEvaluationContext()
                        {
                            Time = time,
                            DeltaTime = deltaTime,
                        });
                        isOnceCompleted = default(TAdapter).IsCompleted(ref StartValue, ref EndValue, ref Options);
                        if (isOnceCompleted && Core.State.Status == MotionStatus.Playing)
                            Options.Restart();
                    }
                    else if (Core.Parameters.LoopType == LoopType.Flip || Core.Parameters.LoopType == LoopType.Yoyo)
                    {
                        var isOdd = Core.State.CompletedLoops % 2 == 1;
                        result = isOdd
                            ? default(TAdapter).Evaluate(ref EndValue, ref StartValue, ref Options, new MotionEvaluationContext()
                            {
                                Time = time,
                                DeltaTime = deltaTime,
                            })
                            : default(TAdapter).Evaluate(ref StartValue, ref EndValue, ref Options, new MotionEvaluationContext()
                            {
                                Time = time,
                                DeltaTime = deltaTime,
                            });
                        isOnceCompleted = isOdd
                        ? default(TAdapter).IsCompleted(ref EndValue, ref StartValue, ref Options) 
                        : default(TAdapter).IsCompleted(ref StartValue, ref EndValue, ref Options);
                    }
                    else
                    {

                        result = default(TAdapter).Evaluate(ref StartValue, ref EndValue, ref Options, new MotionEvaluationContext()
                        {
                            Time = time,
                            DeltaTime = deltaTime,
                        });
                        isOnceCompleted = default(TAdapter).IsCompleted(ref StartValue, ref EndValue, ref Options);
                    }
                }
                Core.UpdateIterationState(time, deltaTime, isOnceCompleted);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Complete<TAdapter>(out TValue result)
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            bool isDurationBased = default(TAdapter).IsDurationBased;
            Core.Complete(out var progress);
            if (isDurationBased)
            {
                result = default(TAdapter).Evaluate(
                    ref StartValue,
                    ref EndValue,
                    ref Options,
                    new()
                    {
                        Progress = progress,
                        Time = Core.State.Time,
                    }
                );
            }
            else
            {
                result = EndValue;
            }
        }
    }
}