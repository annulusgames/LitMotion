using System;
using LitMotion.Adapters;
using Unity.Collections;
using UnityEngine;

namespace LitMotion.Animation
{
    public abstract class PropertyAnimationComponent<TObject, TValue, TOptions, TAdapter> : LitMotionAnimationComponent
        where TObject : UnityEngine.Object
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        [SerializeField] TObject target;
        [SerializeField] SerializableMotionSettings<TValue, TOptions> settings;
        [SerializeField] bool relative;

        TValue startValue;

        public override void OnStop()
        {
            if (target == null) return;
            SetValue(target, startValue);
        }

        public override MotionHandle Play()
        {
            startValue = GetValue(target);

            MotionHandle handle;

            if (relative)
            {
                handle = LMotion.Create<TValue, TOptions, TAdapter>(settings)
                    .Bind(this, (x, state) =>
                    {
                        state.SetValue(target, state.GetRelativeValue(state.startValue, x));
                    });
            }
            else
            {
                handle = LMotion.Create<TValue, TOptions, TAdapter>(settings)
                    .Bind(this, (x, state) =>
                    {
                        state.SetValue(target, x);
                    });
            }

            return handle;
        }

        public override MotionHandle PlayBackward()
        {
            startValue = GetValue(target);

            MotionHandle handle;

            if (relative)
            {
                handle = LMotion.Create<TValue, TOptions, TAdapter>(settings, true)
                    .Bind(this, (x, state) =>
                    {
                        state.SetValue(target, state.GetRelativeValue(state.startValue, x));
                    });
            }
            else
            {
                handle = LMotion.Create<TValue, TOptions, TAdapter>(settings, true)
                    .Bind(this, (x, state) =>
                    {
                        state.SetValue(target, x);
                    });
            }

            return handle;
        }

        protected abstract TValue GetValue(TObject target);
        protected abstract void SetValue(TObject target, in TValue value);
        protected abstract TValue GetRelativeValue(in TValue startValue, in TValue relativeValue);
    }

    // I wish we could use Generic Math in Unity... :(

    public abstract class FloatPropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, float, NoOptions, FloatMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override float GetRelativeValue(in float startValue, in float relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class DoublePropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, double, NoOptions, DoubleMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override double GetRelativeValue(in double startValue, in double relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class IntPropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, int, IntegerOptions, IntMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override int GetRelativeValue(in int startValue, in int relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class LongPropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, long, IntegerOptions, LongMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override long GetRelativeValue(in long startValue, in long relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class Vector2PropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, Vector2, NoOptions, Vector2MotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override Vector2 GetRelativeValue(in Vector2 startValue, in Vector2 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class Vector3PropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, Vector3, NoOptions, Vector3MotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override Vector3 GetRelativeValue(in Vector3 startValue, in Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class Vector4PropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, Vector4, NoOptions, Vector4MotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override Vector4 GetRelativeValue(in Vector4 startValue, in Vector4 relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class ColorPropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, Color, NoOptions, ColorMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override Color GetRelativeValue(in Color startValue, in Color relativeValue)
        {
            return startValue + relativeValue;
        }
    }

    public abstract class RectPropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, Rect, NoOptions, RectMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override Rect GetRelativeValue(in Rect startValue, in Rect relativeValue)
        {
            return new Rect(startValue.position + relativeValue.position, startValue.size + relativeValue.size);
        }
    }

    public abstract class FixedString512BytesPropertyAnimationComponent<TObject> : PropertyAnimationComponent<TObject, FixedString512Bytes, StringOptions, FixedString512BytesMotionAdapter>
        where TObject : UnityEngine.Object
    {
        protected sealed override FixedString512Bytes GetRelativeValue(in FixedString512Bytes startValue, in FixedString512Bytes relativeValue)
        {
            var value = startValue;
            value.Append(relativeValue);
            return value;
        }
    }
}