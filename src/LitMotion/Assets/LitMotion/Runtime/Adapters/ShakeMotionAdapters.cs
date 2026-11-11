using Unity.Jobs;
using UnityEngine;
using LitMotion;
using LitMotion.Adapters;
using Unity.Mathematics;

[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, ShakeOptions, FloatShakeMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector2, ShakeOptions, Vector2ShakeMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector3, ShakeOptions, Vector3ShakeMotionAdapter>))]

namespace LitMotion.Adapters
{
    // Note: Shake motion uses startValue as offset and endValue as vibration strength.

    /// <summary>
    /// Internal helper for generating shake noise.
    /// </summary>
    internal static class ShakeNoise
    {
        public static float PerlinNoise(uint seed, uint axis, float progress, int frequency)
        {
            var offset = (math.hash(new uint2(seed, axis)) & 65535u) / 256f;
            return noise.cnoise(new float2(progress * frequency, offset));
        }
    }

    public readonly struct FloatShakeMotionAdapter : IMotionAdapter<float, ShakeOptions>
    {
        public float Evaluate(ref float startValue, ref float endValue, ref ShakeOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var s);
            var multipliar = options.ShakeMode == ShakeMode.Random
                ? RandomHelper.NextFloat(options.RandomSeed, context.Time, -1f, 1f)
                : ShakeNoise.PerlinNoise(options.RandomSeed, 0, context.Progress, options.Frequency);
            return startValue + s * multipliar;
        }
    }

    public readonly struct Vector2ShakeMotionAdapter : IMotionAdapter<Vector2, ShakeOptions>
    {
        public Vector2 Evaluate(ref Vector2 startValue, ref Vector2 endValue, ref ShakeOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var s);
            var multipliar = options.ShakeMode == ShakeMode.Random
                ? RandomHelper.NextFloat2(options.RandomSeed, context.Time, new float2(-1f), new float2(1f))
                : new float2(
                ShakeNoise.PerlinNoise(options.RandomSeed, 0, context.Progress, options.Frequency),
                ShakeNoise.PerlinNoise(options.RandomSeed, 1, context.Progress, options.Frequency));
            return startValue + new Vector2(s.x * multipliar.x, s.y * multipliar.y);
        }
    }

    public readonly struct Vector3ShakeMotionAdapter : IMotionAdapter<Vector3, ShakeOptions>
    {
        public Vector3 Evaluate(ref Vector3 startValue, ref Vector3 endValue, ref ShakeOptions options, in MotionEvaluationContext context)
        {
            VibrationHelper.EvaluateStrength(endValue, options.Frequency, options.DampingRatio, context.Progress, out var s);
            var multipliar = options.ShakeMode == ShakeMode.Random
                ? RandomHelper.NextFloat3(options.RandomSeed, context.Time, new float3(-1f), new float3(1f))
                : new float3(
                ShakeNoise.PerlinNoise(options.RandomSeed, 0, context.Progress, options.Frequency),
                ShakeNoise.PerlinNoise(options.RandomSeed, 1, context.Progress, options.Frequency),
                ShakeNoise.PerlinNoise(options.RandomSeed, 2, context.Progress, options.Frequency));
            return startValue + new Vector3(s.x * multipliar.x, s.y * multipliar.y, s.z * multipliar.z);
        }
    }
}
