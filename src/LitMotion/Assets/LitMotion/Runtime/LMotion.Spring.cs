using UnityEngine;
using LitMotion.Adapters;
using Unity.Mathematics;
namespace LitMotion
{
    public static partial class LMotion
    {
        /// <summary>
        /// API for creating Spring motions.
        /// </summary>
        public static class Spring
        {
            /// <summary>
            /// Create a builder for building Spring motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="endValue">End value</param>
            /// <param name="options">Spring options</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<float, SpringOptions, FloatSpringMotionAdapter> Create(float startValue, float endValue, SpringOptions options = default)
            {
                options.Init(new float4(startValue, 0.0f, 0.0f, 0.0f), new float4(endValue, 0.0f, 0.0f, 0.0f));
                return Create<float, SpringOptions, FloatSpringMotionAdapter>(startValue, endValue, 0.0f)
                    .WithOptions(options);
            }

            /// <summary>
            /// Create a builder for building Vector2 Spring motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="endValue">End value</param>
            /// <param name="options">Spring options</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector2, SpringOptions, Vector2SpringMotionAdapter> Create(Vector2 startValue, Vector2 endValue, SpringOptions options = default)
            {
                options.Init(new float4(startValue, 0.0f, 0.0f), new float4(endValue, 0.0f, 0.0f));
                return Create<Vector2, SpringOptions, Vector2SpringMotionAdapter>(startValue, endValue, 0.0f)
                    .WithOptions(options);
            }

            /// <summary>
            /// Create a builder for building Vector3 Spring motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="endValue">End value</param>
            /// <param name="options">Spring options</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector3, SpringOptions, Vector3SpringMotionAdapter> Create(Vector3 startValue, Vector3 endValue, SpringOptions options = default)
            {
                options.Init(new float4(startValue, 0.0f), new float4(endValue, 0.0f));
                return Create<Vector3, SpringOptions, Vector3SpringMotionAdapter>(startValue, endValue, 0.0f)
                    .WithOptions(options);
            }

            /// <summary>
            /// Create a builder for building Vector4 Spring motion.
            /// </summary>
            /// <param name="startValue">Start value</param>
            /// <param name="endValue">End value</param>
            /// <param name="options">Spring options</param>
            /// <returns>Created motion builder</returns>
            public static MotionBuilder<Vector4, SpringOptions, Vector4SpringMotionAdapter> Create(Vector4 startValue, Vector4 endValue, SpringOptions options = default)
            {
                options.Init(new float4(startValue), new float4(endValue));
                return Create<Vector4, SpringOptions, Vector4SpringMotionAdapter>(startValue, endValue, 0.0f)
                    .WithOptions(options);
            }
        }
    }
}
