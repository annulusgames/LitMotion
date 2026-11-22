using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LitMotion
{
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Owns native allocators, so it cleans up manually in OnExitingPlayMode.
#endif
    internal static partial class RewindableAllocatorFactory
    {
        const int InitialSize = 128 * 1024;
        static bool isInitialized;
        static readonly Stack<AllocatorHelper<RewindableAllocator>> allocators = new();

        public static AllocatorHelper<RewindableAllocator> CreateAllocator()
        {
            Initialize();

            var allocatorHelper = new AllocatorHelper<RewindableAllocator>(Allocator.Persistent);
            allocatorHelper.Allocator.Initialize(InitialSize, true);
            allocators.Push(allocatorHelper);

            return allocatorHelper;
        }

        static void Initialize()
        {
            if (!isInitialized)
            {
#if UNITY_7000_0_OR_NEWER
                // The Lifecycle API preserves this static state across code reloads.
                // Native allocators are released when exiting Play mode instead.
#if !UNITY_EDITOR
                UnityEngine.Application.quitting += Dispose;
#endif
#elif UNITY_EDITOR
                AssemblyReloadEvents.beforeAssemblyReload += Dispose;
#else
                UnityEngine.Application.quitting += Dispose;
#endif
                isInitialized = true;
            }
        }

#if UNITY_7000_0_OR_NEWER
        [UnityEngine.OnExitingPlayMode]
#endif
        static void Dispose()
        {
            while (allocators.TryPop(out var allocatorHelper))
            {
                allocatorHelper.Allocator.Dispose();
                allocatorHelper.Dispose();
            }
        }
    }
}