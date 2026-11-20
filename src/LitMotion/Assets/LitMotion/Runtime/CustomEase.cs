using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Burst;

namespace LitMotion
{
    /// <summary>
    /// Delegate type for custom easing functions.
    /// The method must be a static method marked with <see cref="BurstCompileAttribute"/> and compiled with
    /// BurstCompiler.CompileFunctionPointer, then passed to WithEase(FunctionPointer&lt;CustomEaseDelegate&gt;).
    /// </summary>
    /// <param name="t">Progress (0-1)</param>
    /// <returns>Eased progress</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate float CustomEaseDelegate(float t);

    /// <summary>
    /// Helper for evaluating custom easing function pointers outside of Burst compiled code.
    /// </summary>
    internal static class CustomEaseHelper
    {
        // FunctionPointer<T>.Invoke allocates a managed delegate every time it is accessed,
        // so cache the delegate and reuse it when running without Burst (e.g. Burst disabled, edit mode, SetTime).
        static readonly ConcurrentDictionary<IntPtr, CustomEaseDelegate> managedDelegates = new();
        static readonly Func<IntPtr, CustomEaseDelegate> managedDelegateFactory = CreateManagedDelegate;

        public static void Register(FunctionPointer<CustomEaseDelegate> functionPointer)
        {
            if (!functionPointer.IsCreated) return;
            managedDelegates.GetOrAdd(functionPointer.Value, managedDelegateFactory);
        }

        /// <summary>
        /// Invoke the function pointer from regular C#. Zero allocation after the first call for each function pointer.
        /// </summary>
        public static float EvaluateManaged(IntPtr ptr, float t)
        {
            if (ptr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Custom ease function pointer is not created. Compile it with BurstCompiler.CompileFunctionPointer<T>() before use.");
            }
            return managedDelegates.GetOrAdd(ptr, managedDelegateFactory).Invoke(t);
        }

        static CustomEaseDelegate CreateManagedDelegate(IntPtr ptr)
        {
            return new FunctionPointer<CustomEaseDelegate>(ptr).Invoke;
        }

        /// <summary>
        /// Returns false when called from regular C# and true when called from Burst compiled code.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBurst()
        {
            var isBurst = true;
            SetIfManaged(ref isBurst);
            return isBurst;
        }

        [BurstDiscard]
        static void SetIfManaged(ref bool isBurst)
        {
            isBurst = false;
        }

        [BurstDiscard]
        public static void EvaluateManaged(IntPtr ptr, float t, ref float result)
        {
            result = EvaluateManaged(ptr, t);
        }
    }
}
