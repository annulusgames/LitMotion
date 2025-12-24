using UnityEngine;
using LitMotion.Adapters;

namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<float, NoOptions, FloatMotionAdapter> Create(MotionSettings<float, NoOptions> settings) => Create<float, NoOptions, FloatMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<double, NoOptions, DoubleMotionAdapter> Create(MotionSettings<double, NoOptions> settings) => Create<double, NoOptions, DoubleMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<int, IntegerOptions, IntMotionAdapter> Create(MotionSettings<int, IntegerOptions> settings) => Create<int, IntegerOptions, IntMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<long, IntegerOptions, LongMotionAdapter> Create(MotionSettings<long, IntegerOptions> settings) => Create<long, IntegerOptions, LongMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector2, NoOptions, Vector2MotionAdapter> Create(MotionSettings<Vector2, NoOptions> settings) => Create<Vector2, NoOptions, Vector2MotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector3, NoOptions, Vector3MotionAdapter> Create(MotionSettings<Vector3, NoOptions> settings) => Create<Vector3, NoOptions, Vector3MotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Vector4, NoOptions, Vector4MotionAdapter> Create(MotionSettings<Vector4, NoOptions> settings) => Create<Vector4, NoOptions, Vector4MotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Quaternion, NoOptions, QuaternionMotionAdapter> Create(MotionSettings<Quaternion, NoOptions> settings) => Create<Quaternion, NoOptions, QuaternionMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Color, NoOptions, ColorMotionAdapter> Create(MotionSettings<Color, NoOptions> settings) => Create<Color, NoOptions, ColorMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<Rect, NoOptions, RectMotionAdapter> Create(MotionSettings<Rect, NoOptions> settings) => Create<Rect, NoOptions, RectMotionAdapter>(settings);

        /// <summary>
        /// Create a builder for building motion.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion entity</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="settings">Motion settings</param>
        /// <returns>Created motion builder</returns>
        public static MotionBuilder<TValue, TOptions, TAdapter> Create<TValue, TOptions, TAdapter>(MotionSettings<TValue, TOptions> settings, bool swapStartEnd = false)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            var buffer = MotionBuilderBuffer<TValue, TOptions>.Rent();
            buffer.StartValue = swapStartEnd ? settings.EndValue : settings.StartValue;
            buffer.EndValue = swapStartEnd ? settings.StartValue : settings.EndValue;
            buffer.Duration = settings.Duration;
            buffer.Options = settings.Options;
            buffer.Ease = settings.Ease;
            buffer.AnimationCurve = settings.CustomEaseCurve;
            buffer.Delay = settings.Delay;
            buffer.DelayType = settings.DelayType;
            buffer.Loops = settings.Loops;
            buffer.LoopType = settings.LoopType;
            buffer.CancelOnError = settings.CancelOnError;
            buffer.SkipValuesDuringDelay = settings.SkipValuesDuringDelay;
            buffer.ImmediateBind = settings.ImmediateBind;
            buffer.Scheduler = settings.Scheduler;
            return new MotionBuilder<TValue, TOptions, TAdapter>(buffer);
        }
    }
}