using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// An object that holds motion settings.
    /// </summary>
    /// <typeparam name="TValue">The type of value to animate</typeparam>
    /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
    [System.Serializable]
    public record MotionSettings<TValue, TOptions>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
    {
        public TValue StartValue
        {
            get => startValue;
            init => startValue = value;
        }

        public TValue EndValue
        {
            get => endValue;
            init => endValue = value;
        }

        public float Duration
        {
            get => duration;
            init => duration = value;
        }

        public TOptions Options
        {
            get => options;
            init => options = value;
        }

        public Ease Ease
        {
            get => ease;
            init => ease = value;
        }

        public AnimationCurve CustomEaseCurve
        {
            get => customEaseCurve;
            init => customEaseCurve = value;
        }

        public float Delay
        {
            get => delay;
            init => delay = value;
        }

        public DelayType DelayType
        {
            get => delayType;
            init => delayType = value;
        }

        public int Loops
        {
            get => loops;
            init => loops = value;
        }

        public LoopType LoopType
        {
            get => loopType;
            init => loopType = value;
        }

        public bool CancelOnError
        {
            get => cancelOnError;
            init => cancelOnError = value;
        }

        public bool SkipValuesDuringDelay
        {
            get => skipValuesDuringDelay;
            init => skipValuesDuringDelay = value;
        }

        public bool ImmediateBind
        {
            get => immediateBind;
            init => immediateBind = value;
        }

        public IMotionScheduler Scheduler
        {
            get => scheduler;
            init => scheduler = value;
        }

        [SerializeField] TValue startValue;
        [SerializeField] TValue endValue;
        [SerializeField] float duration;
        [SerializeField] TOptions options;
        [SerializeField] Ease ease;
        [SerializeField] AnimationCurve customEaseCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] float delay;
        [SerializeField] DelayType delayType;
        [SerializeField] int loops = 1;
        [SerializeField] LoopType loopType;
        [SerializeField] bool cancelOnError;
        [SerializeField] bool skipValuesDuringDelay;
        [SerializeField] bool immediateBind;

        internal IMotionScheduler scheduler;
    }
}