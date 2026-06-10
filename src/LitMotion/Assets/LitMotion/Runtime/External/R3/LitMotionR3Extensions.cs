#if LITMOTION_SUPPORT_R3
using R3;

namespace LitMotion
{
    public static class LitMotionR3Extensions
    {
        /// <summary>
        /// Create the motion as Observable.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <returns>Observable of the created motion.</returns>
        public static Observable<TValue> ToObservable<TValue, TOptions, TAdapter>(this MotionBuilder<TValue, TOptions, TAdapter> builder)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            var subject = new Subject<TValue>();
            builder.SetCallbackData(subject, static (x, subject) => subject.OnNext(x));
            builder.buffer.OnCompleteAction += () => subject.OnCompleted();
            builder.buffer.OnCancelAction += () => subject.OnCompleted();
            builder.ScheduleMotion();
            return subject;
        }

        /// <summary>
        /// Create a motion data and bind it to ReactiveProperty.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="progress">Target object that implements IProgress</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToReactiveProperty<TValue, TOptions, TAdapter>(this MotionBuilder<TValue, TOptions, TAdapter> builder, ReactiveProperty<TValue> reactiveProperty)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            Error.IsNull(reactiveProperty);
            return builder.Bind(reactiveProperty, static (x, target) =>
            {
                target.Value = x;
            });
        }

        /// <summary>
        /// Add this motion handle to R3 CompositeDisposable.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="disposable">Target CompositeDisposable</param>
        /// <param name="disposeBehavior">Behavior when the disposable is disposed</param>
        public static MotionHandle AddTo(this MotionHandle handle, CompositeDisposable disposable, DisposeBehavior disposeBehavior = DisposeBehavior.Cancel)
        {
            disposable.Add(handle.ToDisposable(disposeBehavior));
            return handle;
        }

        /// <summary>
        /// Add this motion handle to R3 SerialDisposable.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="disposable">Target SerialDisposable</param>
        /// <param name="disposeBehavior">Behavior when the disposable is disposed</param>
        public static MotionHandle AddTo(this MotionHandle handle, SerialDisposable disposable, DisposeBehavior disposeBehavior = DisposeBehavior.Cancel)
        {
            disposable.Disposable = handle.ToDisposable(disposeBehavior);
            return handle;
        }

        /// <summary>
        /// Add this motion handle to R3 DisposableBag.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="disposable">Target DisposableBag</param>
        /// <param name="disposeBehavior">Behavior when the disposable is disposed</param>
        public static MotionHandle AddTo(this MotionHandle handle, ref DisposableBag disposable, DisposeBehavior disposeBehavior = DisposeBehavior.Cancel)
        {
            disposable.Add(handle.ToDisposable(disposeBehavior));
            return handle;
        }

        /// <summary>
        /// Add this motion handle to R3 DisposableBuilder.
        /// </summary>
        /// <param name="handle">This motion handle</param>
        /// <param name="disposable">Target DisposableBuilder</param>
        /// <param name="disposeBehavior">Behavior when the disposable is disposed</param>
        public static MotionHandle AddTo(this MotionHandle handle, ref DisposableBuilder disposable, DisposeBehavior disposeBehavior = DisposeBehavior.Cancel)
        {
            disposable.Add(handle.ToDisposable(disposeBehavior));
            return handle;
        }
    }
}
#endif
