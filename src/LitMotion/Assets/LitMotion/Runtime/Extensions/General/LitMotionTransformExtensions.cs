using UnityEngine;

namespace LitMotion.Extensions
{
    /// <summary>
    /// Provides binding extension methods for Transform.
    /// </summary>
    public static class LitMotionTransformExtensions
    {
        /// <summary>
        /// Create a motion data and bind it to Transform.position
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPosition<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.position = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.position.x
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.position;
                p.x = x;
                t.position = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.position.y
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.position;
                p.y = x;
                t.position = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.position.z
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.position;
                p.z = x;
                t.position = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.position.xy
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.position;
                p.x = x.x;
                p.y = x.y;
                t.position = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.position.xz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.position;
                p.x = x.x;
                p.z = x.y;
                t.position = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.position.yz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToPositionYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.position;
                p.y = x.x;
                p.z = x.y;
                t.position = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPosition<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.localPosition = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition.x
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPositionX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localPosition;
                p.x = x;
                t.localPosition = p;
            });
        }


        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition.y
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPositionY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localPosition;
                p.y = x;
                t.localPosition = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition.z
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPositionZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localPosition;
                p.z = x;
                t.localPosition = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition.xy
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPositionXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localPosition;
                p.x = x.x;
                p.y = x.y;
                t.localPosition = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition.xz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPositionXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localPosition;
                p.x = x.x;
                p.z = x.y;
                t.localPosition = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localPosition.yz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalPositionYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localPosition;
                p.y = x.x;
                p.z = x.y;
                t.localPosition = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.rotation
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToRotation<TOptions, TAdapter>(this MotionBuilder<Quaternion, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Quaternion, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.rotation = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localRotation
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalRotation<TOptions, TAdapter>(this MotionBuilder<Quaternion, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Quaternion, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.localRotation = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAngles<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.eulerAngles = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles.x
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAnglesX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.eulerAngles;
                p.x = x;
                t.eulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles.y
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAnglesY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.eulerAngles;
                p.y = x;
                t.eulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles.z
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAnglesZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.eulerAngles;
                p.z = x;
                t.eulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles.xy
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAnglesXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.eulerAngles;
                p.x = x.x;
                p.y = x.y;
                t.eulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles.xz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAnglesXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.eulerAngles;
                p.x = x.x;
                p.z = x.y;
                t.eulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.eulerAngles.yz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToEulerAnglesYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.eulerAngles;
                p.y = x.x;
                p.z = x.y;
                t.eulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAngles<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.localEulerAngles = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles.x
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAnglesX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localEulerAngles;
                p.x = x;
                t.localEulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles.y
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAnglesY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localEulerAngles;
                p.y = x;
                t.localEulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles.z
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAnglesZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localEulerAngles;
                p.z = x;
                t.localEulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles.xy
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAnglesXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localEulerAngles;
                p.x = x.x;
                p.y = x.y;
                t.localEulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles.xz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAnglesXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localEulerAngles;
                p.x = x.x;
                p.z = x.y;
                t.localEulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localEulerAngles.yz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalEulerAnglesYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localEulerAngles;
                p.y = x.x;
                p.z = x.y;
                t.localEulerAngles = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScale<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.localScale = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.x
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localScale;
                p.x = x;
                t.localScale = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.y
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localScale;
                p.y = x;
                t.localScale = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.z
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localScale;
                p.z = x;
                t.localScale = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.xy
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localScale;
                p.x = x.x;
                p.y = x.y;
                t.localScale = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.xz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localScale;
                p.x = x.x;
                p.z = x.y;
                t.localScale = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.yz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                var p = t.localScale;
                p.y = x.x;
                p.z = x.y;
                t.localScale = p;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to Transform.localScale.xyz
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="transform"></param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLocalScaleXYZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, Transform transform)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(transform);
            return builder.Bind(transform, static (x, t) =>
            {
                t.localScale = new Vector3(x, x, x);
            });
        }
    }
}