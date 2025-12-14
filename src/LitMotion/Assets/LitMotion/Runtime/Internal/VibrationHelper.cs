using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace LitMotion
{
    [BurstCompile]
    internal static class VibrationHelper
    {
        [BurstCompile]
        public static void EvaluateStrength(in float strength, in int frequency, in float dampingRatio, in float t, out float result)
        {
            if (t == 1f || t == 0f)
            {
                result = 0f;
                return;
            }
            float angularFrequency = (frequency * t - 0.5f) * math.PI;
            float dampingFactor = dampingRatio * frequency / (2f * math.PI);
            result = strength * math.pow(math.E, -dampingFactor * t) * math.cos(angularFrequency);
        }

        [BurstCompile]
        public static void EvaluateStrength(in Vector2 strength, in int frequency, in float dampingRatio, in float t, out Vector2 result)
        {
            if (t == 1f || t == 0f)
            {
                result = Vector2.zero;
                return;
            }
            float angularFrequency = (frequency * t - 0.5f) * math.PI;
            float dampingFactor = dampingRatio * frequency / (2f * math.PI);
            result = math.cos(angularFrequency) * math.pow(math.E, -dampingFactor * t) * strength;
        }

        [BurstCompile]
        public static void EvaluateStrength(in Vector3 strength, in int frequency, in float dampingRatio, in float t, out Vector3 result)
        {
            if (t == 1f || t == 0f)
            {
                result = Vector3.zero;
                return;
            }
            float angularFrequency = (frequency * t - 0.5f) * math.PI;
            float dampingFactor = dampingRatio * frequency / (2f * math.PI);
            result = math.cos(angularFrequency) * math.pow(math.E, -dampingFactor * t) * strength;
        }
    }
}