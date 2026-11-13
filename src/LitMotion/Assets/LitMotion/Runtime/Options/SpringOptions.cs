using System;
using Unity.Mathematics;
using UnityEngine;

namespace LitMotion
{
    /// <summary>
    /// Options for spring motion.
    /// </summary>
    [Serializable]
    public struct SpringOptions : IEquatable<SpringOptions>, IMotionOptions
    {
        private float4 _startValue;
        private float4 _startVelocity;
        public float4 CurrentValue;
        public float4 CurrentVelocity;
        public float4 TargetValue;
        public float4 TargetVelocity;
        public float Stiffness;
        public float DampingRatio;

        /// <summary>
        /// Creates a new SpringOptions with specified parameters.
        /// </summary>
        /// <param name="stiffness">Spring stiffness (higher = faster convergence)</param>
        /// <param name="dampingRatio">Damping ratio (1.0 = critical damping)</param>
        /// <param name="startVelocity">Initial velocity</param>
        /// <param name="targetVelocity">Target velocity</param>
        public SpringOptions(float stiffness = 10.0f, float dampingRatio = 1.0f, float4 startVelocity = default, float4 targetVelocity = default)
        {
            Stiffness = stiffness;
            DampingRatio = dampingRatio;
            CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f);
            CurrentVelocity = startVelocity;
            TargetValue = new float4(0.0f, 0.0f, 0.0f, 0.0f);
            TargetVelocity = targetVelocity;
            _startValue = new float4(0.0f, 0.0f, 0.0f, 0.0f);
            _startVelocity = startVelocity;
        }

        /// <summary>
        /// Critical damping configuration (fastest convergence without oscillation).
        /// </summary>
        public static SpringOptions Critical
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 1.0f,
                    CurrentVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        /// <summary>
        /// Overdamped configuration (smooth, slow convergence without oscillation).
        /// </summary>
        public static SpringOptions Overdamped
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 1.2f,
                    CurrentVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        /// <summary>
        /// Underdamped configuration (bouncy, oscillating motion before settling).
        /// </summary>
        public static SpringOptions Underdamped
        {
            get
            {
                return new SpringOptions()
                {
                    Stiffness = 10.0f,
                    DampingRatio = 0.6f,
                    CurrentVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    TargetVelocity = new float4(0.0f, 0.0f, 0.0f, 0.0f),
                    CurrentValue = new float4(0.0f, 0.0f, 0.0f, 0.0f)
                };
            }
        }

        public void Init(float4 startValue, float4 targetValue, float4 startVelocity = default, float4 targetVelocity = default)
        {
            CurrentValue = startValue;
            TargetValue = targetValue;
            CurrentVelocity = startVelocity;
            TargetVelocity = targetVelocity;
            _startValue = startValue;
            _startVelocity = startVelocity;
        }

        public void Restart()
        {
            CurrentValue = _startValue;
            CurrentVelocity = _startVelocity;
        }

        public readonly bool Equals(SpringOptions other)
        {
            return Stiffness == other.Stiffness &&
                   DampingRatio == other.DampingRatio &&
                   math.all(TargetVelocity == other.TargetVelocity) &&
                   math.all(CurrentVelocity == other.CurrentVelocity) &&
                   math.all(CurrentValue == other.CurrentValue);
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is SpringOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Stiffness, DampingRatio, TargetVelocity, CurrentVelocity, CurrentValue);
        }
    }

}
