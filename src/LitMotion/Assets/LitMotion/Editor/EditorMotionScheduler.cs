#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion.Editor
{
    /// <summary>
    /// Schedulers available in Editor.
    /// </summary>
#if UNITY_7000_0_OR_NEWER
    [NoAutoStaticsCleanup] // Immutable scheduler instance.
#endif
    public static class EditorMotionScheduler
    {
        /// <summary>
        /// Scheduler that updates motion at EditorApplication.update.
        /// </summary>
        public static readonly IMotionScheduler Update = new EditorUpdateMotionScheduler();
    }
}