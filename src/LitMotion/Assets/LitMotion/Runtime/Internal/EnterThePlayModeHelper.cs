using System;
using System.Collections.Generic;
using System.Diagnostics;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LitMotion.LitMotion.Runtime.Internal
{
    internal class EnterThePlayModeHelper
    {
#if UNITY_EDITOR
        private static readonly List<Action> _scheduledActions = new();

        [InitializeOnEnterPlayMode]
        private static void Reset()
        {
            foreach (var action in _scheduledActions)
                action();
            MotionDispatcher.Clear();
        }
#endif
        
        [Conditional("UNITY_EDITOR")]
        public static void Register<TValue, TOptions, TAdapter>(MotionStorage<TValue, TOptions, TAdapter> storage)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
#if UNITY_EDITOR
            if (EditorSettings.enterPlayModeOptionsEnabled)
                _scheduledActions.Add(storage.Reset);
#endif
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void Register<TValue, TOptions, TAdapter>(UpdateRunner<TValue, TOptions, TAdapter> runner)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
#if UNITY_EDITOR
            if (EditorSettings.enterPlayModeOptionsEnabled)
                _scheduledActions.Add(runner.Reset);
#endif
        }
    }
}