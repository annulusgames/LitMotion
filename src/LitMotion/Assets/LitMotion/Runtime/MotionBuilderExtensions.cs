using System;
using Unity.Collections;

namespace LitMotion
{
    /// <summary>
    /// Provides additional Bind methods for MotionBuilder.
    /// </summary>
    public static class MotionBuilderExtensions
    {
        /// <summary>
        /// Create motion and bind it to a specific object. Unlike the regular Bind method, it avoids allocation by closure by passing an object.
        /// </summary>
        /// <typeparam name="TState">Type of state</typeparam>
        /// <param name="state">Motion state</param>
        /// <param name="action">Action that handles binding</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle Bind<TValue, TOptions, TAdapter, TState>(this MotionBuilder<TValue, TOptions, TAdapter> builder, TState state, Action<TValue, TState> action)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
            where TState : struct
        {
            return builder.Bind(Box.Create(state), action, (value, state, action) => action(value, state.Value));
        }

        /// <summary>
        /// Specifies the rounding format for decimal values when animating integer types.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="roundingMode">Rounding mode</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, IntegerOptions, TAdapter> WithRoundingMode<TValue, TAdapter>(this MotionBuilder<TValue, IntegerOptions, TAdapter> builder, RoundingMode roundingMode)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, IntegerOptions>
        {
            var options = builder.buffer.Options;
            options.RoundingMode = roundingMode;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the frequency of vibration.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, PunchOptions, TAdapter> WithFrequency<TValue, TAdapter>(this MotionBuilder<TValue, PunchOptions, TAdapter> builder, int frequency)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, PunchOptions>
        {
            var options = builder.buffer.Options;
            options.Frequency = frequency;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the vibration damping ratio.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="dampingRatio">Damping ratio</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, PunchOptions, TAdapter> WithDampingRatio<TValue, TAdapter>(this MotionBuilder<TValue, PunchOptions, TAdapter> builder, float dampingRatio)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, PunchOptions>
        {
            var options = builder.buffer.Options;
            options.DampingRatio = dampingRatio;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the noise used for shake motion.
        /// </summary>
        public static MotionBuilder<TValue, ShakeOptions, TAdapter> WithShakeMode<TValue, TAdapter>(this MotionBuilder<TValue, ShakeOptions, TAdapter> builder, ShakeMode shakeMode)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.ShakeMode = shakeMode;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the frequency of vibration.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="frequency">Frequency</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, ShakeOptions, TAdapter> WithFrequency<TValue, TAdapter>(this MotionBuilder<TValue, ShakeOptions, TAdapter> builder, int frequency)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.Frequency = frequency;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the vibration damping ratio.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="dampingRatio">Damping ratio</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, ShakeOptions, TAdapter> WithDampingRatio<TValue, TAdapter>(this MotionBuilder<TValue, ShakeOptions, TAdapter> builder, float dampingRatio)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.DampingRatio = dampingRatio;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the random number seed that determines the shake motion value.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="seed">Random number seed</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, ShakeOptions, TAdapter> WithRandomSeed<TValue, TAdapter>(this MotionBuilder<TValue, ShakeOptions, TAdapter> builder, uint seed)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, ShakeOptions>
        {
            var options = builder.buffer.Options;
            options.RandomSeed = seed;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Enable support for Rich Text tags.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="richTextEnabled">Whether to support Rich Text tags</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, StringOptions, TAdapter> WithRichText<TValue, TAdapter>(this MotionBuilder<TValue, StringOptions, TAdapter> builder, bool richTextEnabled = true)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, StringOptions>
        {
            var options = builder.buffer.Options;
            options.RichTextEnabled = richTextEnabled;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Specify the random number seed used to display scramble characters.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="seed">Rrandom number seed</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, StringOptions, TAdapter> WithRandomSeed<TValue, TAdapter>(this MotionBuilder<TValue, StringOptions, TAdapter> builder, uint seed)
           where TValue : unmanaged
           where TAdapter : unmanaged, IMotionAdapter<TValue, StringOptions>
        {
            var options = builder.buffer.Options;
            options.RandomSeed = seed;
            builder.buffer.Options = options;
            return builder;
        }

        /// <summary>
        /// Fill in the parts that are not yet displayed with random strings.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="scrambleMode">Type of characters used for blank padding</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, StringOptions, TAdapter> WithScrambleChars<TValue, TAdapter>(this MotionBuilder<TValue, StringOptions, TAdapter> builder, ScrambleMode scrambleMode)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, StringOptions>
        {
            if (scrambleMode == ScrambleMode.Custom) throw new ArgumentException("ScrambleMode.Custom cannot be specified explicitly. Use WithScrambleMode(FixedString64Bytes) instead.");

            var options = builder.buffer.Options;
            options.ScrambleMode = scrambleMode;
            builder.buffer.Options = options;

            return builder;
        }

        /// <summary>
        /// Fill in the parts that are not yet displayed with random strings.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="customScrambleChars">Characters used for blank padding</param>
        /// <returns>This builder to allow chaining multiple method calls.</returns>
        public static MotionBuilder<TValue, StringOptions, TAdapter> WithScrambleChars<TValue, TAdapter>(this MotionBuilder<TValue, StringOptions, TAdapter> builder, FixedString64Bytes customScrambleChars)
            where TValue : unmanaged
            where TAdapter : unmanaged, IMotionAdapter<TValue, StringOptions>
        {
            var options = builder.buffer.Options;
            options.ScrambleMode = ScrambleMode.Custom;
            options.CustomScrambleChars = customScrambleChars;
            builder.buffer.Options = options;
            return builder;
        }
    }
}