using System;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
namespace LitMotion
{
    [BurstCompile]
    public static class SpringUtility
    {
        /// <summary>
        /// Simple spring-damper system with critical damping only (no over-damping or under-damping).
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        public static void SpringSimple(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float stiffness = 10.0f)
        {
            // SpringSimple is a simplified version designed specifically for critical damping
            // Uses stiffness directly as natural frequency, where half of damping coefficient equals natural frequency
            float naturalFreq = stiffness;

            // Critical damping calculation logic
            float displacementFromTarget = currentValue - targetValue;  // Displacement from target position
            float velocityWithDamping = currentVelocity + displacementFromTarget * naturalFreq;  // Initial velocity with damping
            float exponentialDecay = FastNegExp(naturalFreq * deltaTime); // Exponential decay factor

            // Critical damping update formula
            currentValue = exponentialDecay * (displacementFromTarget + velocityWithDamping * deltaTime) + targetValue;  // New position
            currentVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * naturalFreq * deltaTime);    // New velocity
        }

        /// <summary>
        /// Elastic spring-damper system with over-damping, under-damping, and critical damping support.
        /// Uses high-performance approximation algorithms.
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="targetVelocity">Target velocity when reaching the target position</param>
        /// <param name="dampingRatio">Damping ratio: 0.6 = bouncy, 1.0 = critical, 1.2 = slow</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency). Examples: 5 = 1s, 10 = 0.5s, 16.5 = 0.2s</param>
        public static void SpringElastic(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float targetVelocity = 0.0f,
            in float dampingRatio = 0.5f,
            in float stiffness = 10.0f)
        {
            float eps = dampingRatio < 1.0f ? 1e-5f : 1e-2f;
            if (Hint.Unlikely(Approximately(currentValue, targetValue, eps)))
            {
                currentValue = targetValue;
                currentVelocity = 0.0f;
                return;
            }
            float targetPosition = targetValue;
            float targetVel = targetVelocity;
            // Rename stiffness parameter to naturalFreq for internal calculation
            float naturalFreq = stiffness;
            float stiffnessValue = naturalFreq * naturalFreq;  // Stiffness value = naturalFreq²
            float dampingHalf = dampingRatio * naturalFreq;
            float dampingCoeff = 2.0f * dampingHalf;  // Damping coefficient = 2 * damping ratio * naturalFreq
            float adjustedTargetPosition = targetPosition + (dampingCoeff * targetVel) / stiffnessValue;

            if (Math.Abs(dampingRatio - 1.0f) < 1e-5f) // Critically Damped
            {
                float initialDisplacement = currentValue - adjustedTargetPosition;
                float initialVelocityWithDamping = currentVelocity + initialDisplacement * dampingHalf;

                float exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                currentValue = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedTargetPosition;
                currentVelocity = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (dampingRatio < 1.0f) // Under Damped
            {
                float dampedFrequency = math.sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f);
                float displacementFromTarget = currentValue - adjustedTargetPosition;
                float amplitude = math.sqrt(FastSquare(currentVelocity + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency) + FastSquare(displacementFromTarget));
                float phase = FastAtan((currentVelocity + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency));

                amplitude = displacementFromTarget > 0.0f ? amplitude : -amplitude;

                float exponentialDecay = FastNegExp(dampingHalf * deltaTime);

                currentValue = amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) + adjustedTargetPosition;
                currentVelocity = -dampingHalf * amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) - dampedFrequency * amplitude * exponentialDecay * math.sin(dampedFrequency * deltaTime + phase);
            }
            else // Over Damped (dampingRatio > 1.0f)
            {
                float fastDecayRate = (dampingCoeff + math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                float slowDecayRate = (dampingCoeff - math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                // Calculate over-damped coefficients: fastDecayCoeff corresponds to fastDecayRate, slowDecayCoeff corresponds to slowDecayRate
                float fastDecayCoeff = (adjustedTargetPosition * fastDecayRate - currentValue * fastDecayRate - currentVelocity) / (slowDecayRate - fastDecayRate);
                float slowDecayCoeff = currentValue - fastDecayCoeff - adjustedTargetPosition;

                float fastExponentialDecay = FastNegExp(fastDecayRate * deltaTime);
                float slowExponentialDecay = FastNegExp(slowDecayRate * deltaTime);

                // Over-damped position update: slowDecayCoeff uses fastExponentialDecay, fastDecayCoeff uses slowExponentialDecay
                currentValue = slowDecayCoeff * fastExponentialDecay + fastDecayCoeff * slowExponentialDecay + adjustedTargetPosition;
                currentVelocity = -fastDecayRate * slowDecayCoeff * fastExponentialDecay - slowDecayRate * fastDecayCoeff * slowExponentialDecay;
            }
        }
        /// <summary>
        /// Precise spring-damper system with over-damping, under-damping, and critical damping support.
        /// Uses exact algorithms for higher accuracy.
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="targetVelocity">Target velocity when reaching the target position</param>
        /// <param name="dampingRatio">Damping ratio</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        public static void SpringPrecise(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float targetVelocity = 0.0f,
            in float dampingRatio = 0.5f,
            in float stiffness = 10.0f)
        {
            float naturalFreq = stiffness;

            float adjustedTargetPosition = targetValue;
            if (math.abs(targetVelocity) > 1e-5f)
            {
                // Adjust target position to achieve specified velocity at target: c = g + (d * q) / s
                // where d = 2 * dampingRatio * naturalFreq, s = naturalFreq^2
                float dampingCoeff = 2.0f * dampingRatio * naturalFreq;
                float stiffnessValue = naturalFreq * naturalFreq;
                adjustedTargetPosition = targetValue + (dampingCoeff * targetVelocity) / stiffnessValue;
            }

            float adjustedDisplacement = currentValue - adjustedTargetPosition;
            float dampingRatioSquared = dampingRatio * dampingRatio;
            float r = -dampingRatio * naturalFreq;

            float displacement;
            float calculatedVelocity;

            if (dampingRatio > 1)
            {
                // Over-damped
                float s = naturalFreq * math.sqrt(dampingRatioSquared - 1);
                float gammaPlus = r + s;
                float gammaMinus = r - s;

                float coeffB = (gammaMinus * adjustedDisplacement - currentVelocity) / (gammaMinus - gammaPlus);
                float coeffA = adjustedDisplacement - coeffB;
                displacement = coeffA * FastExp(gammaMinus * deltaTime) + coeffB * FastExp(gammaPlus * deltaTime);
                calculatedVelocity = coeffA * gammaMinus * FastExp(gammaMinus * deltaTime) +
                                    coeffB * gammaPlus * FastExp(gammaPlus * deltaTime);
            }
            else if (math.abs(dampingRatio - 1.0f) < 1e-5f)
            {
                // Critically damped
                float coeffA = adjustedDisplacement;
                float coeffB = currentVelocity + naturalFreq * adjustedDisplacement;
                float nFdT = -naturalFreq * deltaTime;
                displacement = (coeffA + coeffB * deltaTime) * FastExp(nFdT);
                calculatedVelocity = ((coeffA + coeffB * deltaTime) * FastExp(nFdT) * (-naturalFreq)) +
                                    coeffB * FastExp(nFdT);
            }
            else
            {
                // Under-damped
                float dampedFreq = naturalFreq * math.sqrt(1 - dampingRatioSquared);
                float cosCoeff = adjustedDisplacement;
                float sinCoeff = (1.0f / dampedFreq) * ((-r * adjustedDisplacement) + currentVelocity);
                float dFdT = dampedFreq * deltaTime;
                displacement = FastExp(r * deltaTime) * (cosCoeff * math.cos(dFdT) + sinCoeff * math.sin(dFdT));
                calculatedVelocity = displacement * r +
                                    (FastExp(r * deltaTime) *
                                     ((-dampedFreq * cosCoeff * math.sin(dFdT) +
                                       dampedFreq * sinCoeff * math.cos(dFdT))));
            }

            currentValue = displacement + adjustedTargetPosition;
            currentVelocity = calculatedVelocity;
        }
        /// <summary>
        /// Velocity smoothing spring-damper system that supports continuous motion with target velocity.
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="intermediatePosition">Intermediate position (maintains state)</param>
        /// <param name="smothingVelocity">Smoothing velocity (linear velocity to smooth)</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        public static void SpringSimpleVelocitySmoothing(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            ref float intermediatePosition,
            in float smothingVelocity = 2f,
            in float stiffness = 10.0f)
        {
            // According to the original design, use stiffness directly as natural frequency
            float naturalFreq = stiffness;

            // Calculate difference between target and intermediate position
            float targetIntermediateDiff = targetValue - intermediatePosition;
            float absTargetIntermediateDiff = math.abs(targetIntermediateDiff);

            // Calculate velocity direction
            float velocityDirection = (targetIntermediateDiff > 0.0f ? 1.0f : -1.0f) * smothingVelocity;

            float anticipatedTime = 1f / naturalFreq;

            // Calculate future target position
            float futureTargetPosition = absTargetIntermediateDiff > anticipatedTime * smothingVelocity ?
                intermediatePosition + velocityDirection * anticipatedTime : targetValue;

            // Directly call SpringSimple function
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, futureTargetPosition, stiffness);

            // Update intermediate position
            intermediatePosition = absTargetIntermediateDiff > deltaTime * smothingVelocity ?
                intermediatePosition + velocityDirection * deltaTime : targetValue;
        }

        /// <summary>
        /// Duration-limited spring-damper system that automatically calculates appropriate stiffness based on target arrival time.
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="durationSeconds">Target arrival time in seconds</param>
        public static void SpringSimpleDurationLimit(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            in float durationSeconds = 0.2f)
        {
            // For critically damped systems, convergence time is approximately 3-4 times the time constant.
            // Time constant = 1 / naturalFreq, so naturalFreq = 4.6 / durationSeconds
            // This achieves approximately 99% convergence within durationSeconds
            float naturalFreq = 4.6f / durationSeconds;

            // Use target value directly, no complex intermediate target logic needed
            // Let the spring converge directly to the final target
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, targetValue, naturalFreq);
        }

        /// <summary>
        /// Double smoothing spring-damper system using two cascaded spring systems for smoother motion.
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="intermediatePosition">Intermediate position (maintains state)</param>
        /// <param name="intermediateVelocity">Intermediate velocity (maintains state)</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        public static void SpringSimpleDoubleSmoothing(
            in float deltaTime,
            ref float currentValue,
            ref float currentVelocity,
            in float targetValue,
            ref float intermediatePosition,
            ref float intermediateVelocity,
            in float stiffness = 10.0f)
        {
            float floatStiffness = 2.0f * stiffness;

            // First spring: from intermediate position to target position
            SpringSimple(deltaTime, ref intermediatePosition, ref intermediateVelocity, targetValue, floatStiffness);

            // Second spring: from current position to intermediate position
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, intermediatePosition, floatStiffness);
        }

        #region float4 Spring Functions

        /// <summary>
        /// Simple spring-damper system with critical damping only (float4 version).
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        [BurstCompile]
        public static void SpringSimple(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            in float stiffness = 1.0f)
        {
            // Check if already converged to target value
            if (Hint.Unlikely(Approximately(currentValue, targetValue)))
            {
                currentValue = targetValue;
                currentVelocity = new float4(0.0f);
                return;
            }

            // Use stiffness directly as natural frequency, where half of damping coefficient equals natural frequency
            float naturalFreq = stiffness;

            // Critical damping calculation logic
            float4 displacementFromTarget = currentValue - targetValue;  // Displacement from target position
            float4 velocityWithDamping = currentVelocity + displacementFromTarget * naturalFreq;  // Initial velocity with damping
            float4 exponentialDecay = (float4)FastNegExp(naturalFreq * deltaTime); // Exponential decay factor

            // Critical damping update formula
            currentValue = exponentialDecay * (displacementFromTarget + velocityWithDamping * deltaTime) + targetValue;  // New position
            currentVelocity = exponentialDecay * (currentVelocity - velocityWithDamping * naturalFreq * deltaTime);    // New velocity
        }

        /// <summary>
        /// Elastic spring-damper system with over-damping, under-damping, and critical damping support (float4 version).
        /// Uses high-performance approximation algorithms.
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="targetVelocity">Target velocity when reaching the target position</param>
        /// <param name="dampingRatio">Damping ratio: 0.6 = bouncy, 1.0 = critical, 1.2 = slow</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency). Examples: 5 = 1s, 10 = 0.5s, 16.5 = 0.2s</param>
        [BurstCompile]
        public static void SpringElastic(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            in float4 targetVelocity,
            in float dampingRatio = 0.5f,
            in float stiffness = 10.0f)
        {
            // Check if already converged to target value
            if (Hint.Unlikely(Approximately(currentValue, targetValue)))
            {
                currentValue = targetValue;
                currentVelocity = new float4(0.0f);
                return;
            }

            // Rename stiffness parameter to naturalFreq for internal calculation
            float naturalFreq = stiffness;
            float stiffnessValue = naturalFreq * naturalFreq;  // Stiffness value = naturalFreq²
            float dampingHalf = dampingRatio * naturalFreq;
            float dampingCoeff = 2.0f * dampingHalf;  // Damping coefficient = 2 * damping ratio * naturalFreq
            float4 adjustedtargetValue = targetValue + (dampingCoeff * targetVelocity) / stiffnessValue;

            if (math.abs(dampingRatio - 1.0f) < 1e-5f) // Critically Damped
            {
                float4 initialDisplacement = currentValue - adjustedtargetValue;
                float4 initialVelocityWithDamping = currentVelocity + initialDisplacement * dampingHalf;

                float exponentialDecay = (float)FastNegExp((float)(dampingHalf * deltaTime));

                currentValue = initialDisplacement * exponentialDecay + deltaTime * initialVelocityWithDamping * exponentialDecay + adjustedtargetValue;
                currentVelocity = -dampingHalf * initialDisplacement * exponentialDecay - dampingHalf * deltaTime * initialVelocityWithDamping * exponentialDecay + initialVelocityWithDamping * exponentialDecay;
            }
            else if (dampingRatio < 1.0f) // Under Damped
            {
                float dampedFrequency = math.sqrt(stiffnessValue - (dampingCoeff * dampingCoeff) / 4.0f);
                float4 displacementFromTarget = currentValue - adjustedtargetValue;
                float4 amplitude = math.sqrt(Square(currentVelocity + dampingHalf * displacementFromTarget) / (dampedFrequency * dampedFrequency) + Square(displacementFromTarget));
                float4 phase = FastAtan((currentVelocity + displacementFromTarget * dampingHalf) / (-displacementFromTarget * dampedFrequency));

                amplitude = math.select(-amplitude, amplitude, displacementFromTarget > 0.0f);

                float exponentialDecay = (float)FastNegExp((float)(dampingHalf * deltaTime));

                currentValue = amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) + adjustedtargetValue;
                currentVelocity = -dampingHalf * amplitude * exponentialDecay * math.cos(dampedFrequency * deltaTime + phase) - dampedFrequency * amplitude * exponentialDecay * math.sin(dampedFrequency * deltaTime + phase);
            }
            else // Over Damped (dampingRatio > 1.0f)
            {
                float fastDecayRate = (dampingCoeff + math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                float slowDecayRate = (dampingCoeff - math.sqrt(dampingCoeff * dampingCoeff - 4f * stiffnessValue)) / 2.0f;
                // Calculate over-damped coefficients: fastDecayCoeff corresponds to fastDecayRate, slowDecayCoeff corresponds to slowDecayRate
                float4 fastDecayCoeff = (adjustedtargetValue * fastDecayRate - currentValue * fastDecayRate - currentVelocity) / (slowDecayRate - fastDecayRate);
                float4 slowDecayCoeff = currentValue - fastDecayCoeff - adjustedtargetValue;

                float fastExponentialDecay = (float)FastNegExp((float)(fastDecayRate * deltaTime));
                float slowExponentialDecay = (float)FastNegExp((float)(slowDecayRate * deltaTime));

                // Over-damped position update: slowDecayCoeff uses fastExponentialDecay, fastDecayCoeff uses slowExponentialDecay
                currentValue = slowDecayCoeff * fastExponentialDecay + fastDecayCoeff * slowExponentialDecay + adjustedtargetValue;
                currentVelocity = -fastDecayRate * slowDecayCoeff * fastExponentialDecay - slowDecayRate * fastDecayCoeff * slowExponentialDecay;
            }
        }

        /// <summary>
        /// Velocity smoothing spring-damper system that supports continuous motion with target velocity (float4 version).
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="intermediatePosition">Intermediate position (maintains state)</param>
        /// <param name="smothingVelocity">Smoothing velocity</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        [BurstCompile]
        public static void SpringSimpleVelocitySmoothing(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            ref float4 intermediatePosition,
            in float smothingVelocity = 2f,
            in float stiffness = 1.0f)
        {
            // According to the original design, use stiffness directly as natural frequency
            float naturalFreq = stiffness;

            // Calculate difference between target and intermediate position
            float4 targetIntermediateDiff = targetValue - intermediatePosition;
            float4 absTargetIntermediateDiff = math.abs(targetIntermediateDiff);

            // Calculate velocity direction
            float4 velocityDirection = math.sign(targetIntermediateDiff) * smothingVelocity;

            // Calculate anticipated time
            float anticipatedTime = 1f / naturalFreq;

            // Calculate future target position
            float4 futureTargetPosition = math.select(targetValue,
                intermediatePosition + velocityDirection * anticipatedTime,
                absTargetIntermediateDiff > anticipatedTime * smothingVelocity);

            // Directly call SpringSimple function
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, futureTargetPosition, stiffness);

            // Update intermediate position
            intermediatePosition = math.select(targetValue,
                intermediatePosition + velocityDirection * deltaTime,
                absTargetIntermediateDiff > deltaTime * smothingVelocity);
        }

        /// <summary>
        /// Duration-limited spring-damper system that automatically calculates appropriate stiffness based on target arrival time (float4 version).
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="durationSeconds">Target arrival time in seconds</param>
        [BurstCompile]
        public static void SpringSimpleDurationLimit(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            ref float4 targetValue,
            in float durationSeconds = 0.2f)
        {
            // For critically damped systems, convergence time is approximately 3-4 times the time constant.
            // Time constant = 1 / naturalFreq, so naturalFreq = 4.6 / durationSeconds
            // This achieves approximately 99% convergence within durationSeconds
            float naturalFreq = 4.6f / durationSeconds;

            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, targetValue, naturalFreq);
        }

        /// <summary>
        /// Double smoothing spring-damper system using two cascaded spring systems for smoother motion (float4 version).
        /// </summary>
        /// <param name="deltaTime">Time step in seconds</param>
        /// <param name="currentValue">Current position value</param>
        /// <param name="currentVelocity">Current velocity value</param>
        /// <param name="targetValue">Target position value</param>
        /// <param name="intermediatePosition">Intermediate position (maintains state)</param>
        /// <param name="intermediateVelocity">Intermediate velocity (maintains state)</param>
        /// <param name="stiffness">Natural frequency (stiffness is used directly as natural frequency)</param>
        [BurstCompile]
        public static void SpringSimpleDoubleSmoothing(
            in float deltaTime,
            ref float4 currentValue,
            ref float4 currentVelocity,
            in float4 targetValue,
            ref float4 intermediatePosition,
            ref float4 intermediateVelocity,
            in float stiffness = 1.0f)
        {
            float floatStiffness = 2.0f * stiffness;

            // First spring: from intermediate position to target position
            SpringSimple(deltaTime, ref intermediatePosition, ref intermediateVelocity, targetValue, floatStiffness);

            // Second spring: from current position to intermediate position
            SpringSimple(deltaTime, ref currentValue, ref currentVelocity, intermediatePosition, floatStiffness);
        }

        #endregion



        private static float HalfLifeToDamping(float halfLife, float eps = 1e-5f)
        {
            return (4.0f * 0.6931471805599453f) / (halfLife + eps);
        }

        private static float DampingToHalfLife(float damping, float eps = 1e-5f)
        {
            return (4.0f * 0.6931471805599453f) / (damping + eps);
        }

        private static float DampingRatioToStiffness(float ratio, float damping)
        {
            return Square(damping / (ratio * 2.0f));
        }

        private static float DampingRatioToDamping(float ratio, float stiffness)
        {
            return ratio * 2.0f * math.sqrt(stiffness);
        }

        private static float FrequencyToStiffness(float frequency)
        {
            return Square(2.0f * math.PI * frequency);
        }

        /// <summary>
        /// Converts half-life to natural frequency (for critical damping).
        /// </summary>
        /// <param name="halflife">Half-life in seconds</param>
        /// <returns>Natural frequency in rad/s</returns>
        private static float HalflifeToNaturalFreq(float halflife)
        {
            return 0.69314718056f / halflife;  // ω₀ = ln(2) / τ₁/₂
        }

        /// <summary>
        /// Converts natural frequency to half-life (for critical damping).
        /// </summary>
        /// <param name="naturalFreq">Natural frequency in rad/s</param>
        /// <returns>Half-life in seconds</returns>
        private static float NaturalFreqToHalflife(float naturalFreq)
        {
            return 0.69314718056f / naturalFreq;  // τ₁/₂ = ln(2) / ω₀
        }
        private static float FastNegExp(float x)
        {
            return 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);
        }

        private static float Square(float x)
        {
            return x * x;
        }

        private static float4 Square(float4 x)
        {
            return x * x;
        }

        /// <summary>
        /// Fast arctangent approximation, 2-5x faster than Math.Atan with minimal precision loss.
        /// </summary>
        private static float FastAtan(float x)
        {
            float z = math.abs(x);
            float w = z > 1.0f ? 1.0f / z : z;
            float y = (math.PI / 4.0f) * w - w * (w - 1) * (0.2447f + 0.0663f * w);
            return math.sign(x) * (z > 1.0f ? math.PI / 2.0f - y : y);
        }

        /// <summary>
        /// Fast arctangent approximation (float4 version).
        /// </summary>
        private static float4 FastAtan(float4 x)
        {
            float4 z = math.abs(x);
            float4 w = math.select(1.0f / z, z, z <= 1.0f);
            float4 y = (math.PI / 4.0f) * w - w * (w - 1.0f) * (0.2447f + 0.0663f * w);
            return math.sign(x) * math.select(math.PI / 2.0f - y, y, z <= 1.0f);
        }

        /// <summary>
        /// Fast square function with same performance as direct multiplication
        /// </summary>
        private static float FastSquare(float x)
        {
            return x * x;
        }

        private static float FastExp(float x)
        {
            // Fast exponential function approximation for Burst
            return 1.0f / (1.0f - x + 0.5f * x * x - 0.1667f * x * x * x);
        }
        /// <summary>
        /// Checks if two float values are approximately equal.
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="precision">Precision threshold</param>
        /// <returns>True if approximately equal</returns>
        [BurstCompile]
        public static bool Approximately(in float a, in float b, in float precision = 1e-2f)
        {
            return math.abs(b - a) < math.max(1E-06f * math.max(math.abs(a), math.abs(b)), precision);
        }

        /// <summary>
        /// Checks if two float4 values are approximately equal.
        /// </summary>
        /// <param name="a">First value</param>
        /// <param name="b">Second value</param>
        /// <param name="precision">Precision threshold</param>
        /// <returns>True if all components are approximately equal</returns>
        [BurstCompile]
        public static bool Approximately(in float4 a, in float4 b, in float precision = 1e-2f)
        {
            return math.all(math.abs(b - a) < math.max(1E-06f * math.max(math.abs(a), math.abs(b)), precision));
        }
    }
}
