using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using LitMotion;
using LitMotion.Adapters;

[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<float, SpringOptions, FloatSpringMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector2, SpringOptions, Vector2SpringMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector3, SpringOptions, Vector3SpringMotionAdapter>))]
[assembly: RegisterGenericJobType(typeof(MotionUpdateJob<Vector4, SpringOptions, Vector4SpringMotionAdapter>))]

namespace LitMotion.Adapters
{
    public readonly struct FloatSpringMotionAdapter : IMotionAdapter<float, SpringOptions>
    {
        public float Evaluate(ref float startValue, ref float endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            options.TargetValue.x = endValue;
            SpringUtility.SpringElastic(
                (float)context.DeltaTime,
                ref options.CurrentValue.x,
                ref options.CurrentVelocity.x,
                options.TargetValue.x,
                options.TargetVelocity.x,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.x;
        }

        public bool IsCompleted(ref float startValue, ref float endValue, ref SpringOptions options)
        {
            return SpringUtility.Approximately(options.CurrentValue.x, options.TargetValue.x);
        }

        public bool IsDurationBased => false;
    }

    /// <summary>
    /// Spring motion adapter for Vector2 using float4 SpringElastic method.
    /// </summary>
    public readonly struct Vector2SpringMotionAdapter : IMotionAdapter<Vector2, SpringOptions>
    {
        public Vector2 Evaluate(ref Vector2 startValue, ref Vector2 endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            float deltaTime = (float)context.DeltaTime;
            options.TargetValue.xy = endValue;
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                options.TargetValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.xy;
        }

        public bool IsCompleted(ref Vector2 startValue, ref Vector2 endValue, ref SpringOptions options)
        {
            return SpringUtility.Approximately(options.CurrentValue, options.TargetValue);
        }

        public bool IsDurationBased => false;
    }

    /// <summary>
    /// Spring motion adapter for Vector3 using float4 SpringElastic method.
    /// </summary>
    public readonly struct Vector3SpringMotionAdapter : IMotionAdapter<Vector3, SpringOptions>
    {
        public Vector3 Evaluate(ref Vector3 startValue, ref Vector3 endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            float deltaTime = (float)context.DeltaTime;
            options.TargetValue.xyz = endValue;
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                options.TargetValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            return options.CurrentValue.xyz;
        }

        public bool IsCompleted(ref Vector3 startValue, ref Vector3 endValue, ref SpringOptions options)
        {
            return SpringUtility.Approximately(options.CurrentValue, options.TargetValue);
        }

        public bool IsDurationBased => false;
    }

    /// <summary>
    /// Spring motion adapter for Vector4 using float4 SpringElastic method.
    /// </summary>
    public readonly struct Vector4SpringMotionAdapter : IMotionAdapter<Vector4, SpringOptions>
    {
        public Vector4 Evaluate(ref Vector4 startValue, ref Vector4 endValue, ref SpringOptions options, in MotionEvaluationContext context)
        {
            float deltaTime = (float)context.DeltaTime;
            options.TargetValue = endValue;
            SpringUtility.SpringElastic(
                deltaTime,
                ref options.CurrentValue,
                ref options.CurrentVelocity,
                options.TargetValue,
                options.TargetVelocity,
                options.DampingRatio,
                options.Stiffness
            );
            
            return options.CurrentValue;
        }

        public bool IsCompleted(ref Vector4 startValue, ref Vector4 endValue, ref SpringOptions options)
        {
            return SpringUtility.Approximately(options.CurrentValue, options.TargetValue);
        }

        public bool IsDurationBased => false;
    }
}
