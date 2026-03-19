
using System;
using LitMotion.Adapters;
using UnityEngine;

namespace LitMotion.Animation.Components
{
    [Serializable]
    public abstract class TransformPositionAnimationBase<TOptions, TAdapter> : PropertyAnimationComponent<Transform, Vector3, TOptions, TAdapter>
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
    {
        [SerializeField] bool useWorldSpace;

        protected override Vector3 GetValue(Transform target)
        {
            return useWorldSpace ? target.position : target.localPosition;
        }

        protected override void SetValue(Transform target, in Vector3 value)
        {
            if (useWorldSpace) target.position = value;
            else target.localPosition = value;
        }

        protected override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Position")]
    public sealed class TransformPositionAnimation : TransformPositionAnimationBase<NoOptions, Vector3MotionAdapter> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Position (Punch)")]
    public sealed class TransformPositionPunchAnimation : TransformPositionAnimationBase<PunchOptions, Vector3PunchMotionAdapter> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Position (Shake)")]
    public sealed class TransformPositionShakeAnimation : TransformPositionAnimationBase<ShakeOptions, Vector3ShakeMotionAdapter> { }

    [Serializable]
    public abstract class TransformRotationAnimationBase<TOptions, TAdapter> : PropertyAnimationComponent<Transform, Vector3, TOptions, TAdapter>
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
    {
        [SerializeField] bool useWorldSpace;

        protected override Vector3 GetValue(Transform target)
        {
            return useWorldSpace ? target.eulerAngles : target.localEulerAngles;
        }

        protected override void SetValue(Transform target, in Vector3 value)
        {
            if (useWorldSpace) target.eulerAngles = value;
            else target.localEulerAngles = value;
        }

        protected override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Rotation")]
    public sealed class TransformRotationAnimation : TransformRotationAnimationBase<NoOptions, Vector3MotionAdapter> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Rotation (Punch)")]
    public sealed class TransformRotationPunchAnimation : TransformRotationAnimationBase<PunchOptions, Vector3PunchMotionAdapter> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Rotation (Shake)")]
    public sealed class TransformRotationShakeAnimation : TransformRotationAnimationBase<ShakeOptions, Vector3ShakeMotionAdapter> { }

    [Serializable]
    public abstract class TransformScaleAnimationBase<TOptions, TAdapter> : PropertyAnimationComponent<Transform, Vector3, TOptions, TAdapter>
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
    {
        protected override Vector3 GetValue(Transform target)
        {
            return target.localScale;
        }

        protected override void SetValue(Transform target, in Vector3 value)
        {
            target.localScale = value;
        }

        protected override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Scale")]
    public sealed class TransformScaleAnimation : TransformScaleAnimationBase<NoOptions, Vector3MotionAdapter> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Scale (Punch)")]
    public sealed class TransformScalePunchAnimation : TransformScaleAnimationBase<PunchOptions, Vector3PunchMotionAdapter> { }

    [Serializable]
    [LitMotionAnimationComponentMenu("Transform/Scale (Shake)")]
    public sealed class TransformScaleShakeAnimation : TransformScaleAnimationBase<ShakeOptions, Vector3ShakeMotionAdapter> { }
}