using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace LitMotion.Editor
{
    /// <summary>
    /// Editor window that displays a list of motions being tracked.
    /// </summary>
    public class MotionDebuggerWindow : EditorWindow
    {
        static MotionDebuggerWindow instance;

        [MenuItem("Window/LitMotion Debugger")]
        public static void OpenWindow()
        {
            if (instance != null) instance.Close();
            GetWindow<MotionDebuggerWindow>("LitMotion Debugger").Show();
        }

        MotionDebuggerTreeView treeView;
        SearchField searchField;
        object horizontalSplitterState;
        object verticalSplitterState;

        const string EnabledPrefsKey = "LitMotion-Debugger-Enabled";
        const string EnableStackTracePrefsKey = "LitMotion-Debugger-EnableStackTrace";

        void OnEnable()
        {
            instance = this;
            horizontalSplitterState = SplitterGUILayout.CreateSplitterState(new float[] { 50f, 50f }, new int[] { 32, 32 }, null);
            verticalSplitterState = SplitterGUILayout.CreateSplitterState(new float[] { 50f, 15f }, new int[] { 32, 32 }, null);
            treeView = new MotionDebuggerTreeView();
            searchField = new SearchField();
            MotionDebugger.Enabled = EditorPrefs.GetBool(EnabledPrefsKey, false);
            MotionDebugger.EnableStackTrace = EditorPrefs.GetBool(EnableStackTracePrefsKey, false);
        }

        void OnGUI()
        {
            RenderHeadPanel();
            SplitterGUILayout.BeginHorizontalSplit(horizontalSplitterState);
            RenderTable();
            RenderDetailsPanel();
            SplitterGUILayout.EndHorizontalSplit();
        }

        static readonly GUIContent ClearHeadContent = new("Clear");
        static readonly GUIContent EnabledHeadContent = new("Enable");
        static readonly GUIContent EnableStackTraceHeadContent = new("Stack Trace");

        void RenderHeadPanel()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Toggle(MotionDebugger.Enabled, EnabledHeadContent, EditorStyles.toolbarButton, GUILayout.Width(110f)) != MotionDebugger.Enabled)
            {
                MotionDebugger.Enabled = !MotionDebugger.Enabled;
                EditorPrefs.SetBool(EnabledPrefsKey, MotionDebugger.Enabled);
            }

            if (GUILayout.Toggle(MotionDebugger.EnableStackTrace, EnableStackTraceHeadContent, EditorStyles.toolbarButton, GUILayout.Width(90f)) != MotionDebugger.EnableStackTrace)
            {
                MotionDebugger.EnableStackTrace = !MotionDebugger.EnableStackTrace;
                EditorPrefs.SetBool(EnableStackTracePrefsKey, MotionDebugger.EnableStackTrace);
            }

            GUILayout.FlexibleSpace();

            treeView.searchString = searchField.OnToolbarGUI(treeView.searchString);

            if (GUILayout.Button(ClearHeadContent, EditorStyles.toolbarButton, GUILayout.Width(70f)))
            {
                MotionDebugger.Clear();
                treeView.ReloadAndSort();
                Repaint();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        static GUIStyle windowStyle;
        Vector2 tableScroll;

        void RenderTable()
        {
            if (windowStyle == null)
            {
                windowStyle = new GUIStyle("GroupBox")
                {
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = new RectOffset(0, 0, 0, 0)
                };
            }

            tableScroll = EditorGUILayout.BeginScrollView(tableScroll, windowStyle, new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.MaxWidth(2000f)
            });
            var controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true)
            });

            treeView?.OnGUI(controlRect);

            EditorGUILayout.EndScrollView();
        }

        static int interval;
        void Update()
        {
            if (interval++ % 120 == 0)
            {
                treeView.ReloadAndSort();
                Repaint();
            }
        }

        static GUIStyle detailsStyle;
        Vector2 detailsScroll;
        Vector2 stackTraceScroll;

        void RenderDetailsPanel()
        {
            if (detailsStyle == null)
            {
                detailsStyle = new GUIStyle("CN Message")
                {
                    wordWrap = false,
                    stretchHeight = true
                };
                detailsStyle.margin.right = 15;
            }

            SplitterGUILayout.BeginVerticalSplit(verticalSplitterState);

            detailsScroll = EditorGUILayout.BeginScrollView(detailsScroll, windowStyle);
            {
                var selected = treeView.state.selectedIDs;
                if (selected.Count > 0 && treeView.CurrentBindingItems.FirstOrDefault(x => x.id == selected[0]) is MotionDebuggerViewItem item)
                {
                    ref var dataRef = ref MotionManager.GetDataRef(item.Handle, false);
                    ref var managedDataRef = ref MotionManager.GetManagedDataRef(item.Handle, false);
                    var debugInfo = MotionManager.GetDebugInfo(item.Handle);

                    ref var state = ref dataRef.State;
                    ref var parameters = ref dataRef.Parameters;

                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        EditorGUILayout.LabelField("Motion Handle", EditorStyles.boldLabel);
                        EditorGUILayout.Space(1);

                        GenericField("Name", item.Handle.GetDebugName());
                        GenericField("Index", item.Handle.Index);
                        GenericField("Version", item.Handle.Version);

                        EditorGUILayout.Space(4);
                        GenericField("Type", item.MotionType);
                        GenericField("Scheduler", item.SchedulerType);
                    }

                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                        EditorGUILayout.Space(1);

                        GenericField("Start Value", debugInfo.StartValue);
                        GenericField("End Value", debugInfo.EndValue);

                        EditorGUILayout.Space(4);
                        GenericField("Duration", parameters.Duration);
                        GenericField("Delay", parameters.Delay);
                        GenericField("Delay Type", parameters.DelayType);
                        GenericField("Loops", parameters.Loops);
                        GenericField("Loop Type", parameters.LoopType);

                        EditorGUILayout.Space(4);
                        GenericField("Ease", parameters.Ease);
                        if (parameters.Ease is Ease.CustomAnimationCurve)
                        {
                            GenericField("Custom Ease Curve", parameters.AnimationCurve);
                        }
                        else if (parameters.Ease is Ease.CustomFunction)
                        {
                            GenericField("Custom Ease Function", parameters.CustomEaseFunction.IsCreated ? $"0x{parameters.CustomEaseFunction.Value.ToInt64():X}" : "None");
                        }

                        EditorGUILayout.Space(4);
                        GenericField("Cancel On Error", managedDataRef.CancelOnError);
                        GenericField("Skip Values During Delay", managedDataRef.SkipValuesDuringDelay);

                        EditorGUILayout.Space(4);
                        GenericField("State[0]", managedDataRef.State0);
                        GenericField("State[1]", managedDataRef.State1);
                        GenericField("State[2]", managedDataRef.State2);
                    }

                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
                        EditorGUILayout.Space(1);

                        EditorGUILayout.LabelField("Status", state.Status.ToString());
                        GenericField("Time", state.Time);
                        GenericField("Completed Loops", state.CompletedLoops);
                        EditorGUILayout.Space(4);
                        GenericField("Playback Speed", state.PlaybackSpeed);
                        GenericField("Is Preserved", state.IsPreserved);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            stackTraceScroll = EditorGUILayout.BeginScrollView(stackTraceScroll, windowStyle);
            {
                var selected = treeView.state.selectedIDs;
                if (selected.Count > 0 && treeView.CurrentBindingItems.FirstOrDefault(x => x.id == selected[0]) is MotionDebuggerViewItem item)
                {
                    EditorGUILayout.LabelField("Stack Trace");
                    var vector = detailsStyle.CalcSize(new GUIContent(item.StackTrace));
                    EditorGUILayout.SelectableLabel(item.StackTrace, detailsStyle, new GUILayoutOption[]
                    {
                        GUILayout.ExpandWidth(true),
                        GUILayout.MinWidth(vector.x),
                        GUILayout.MinHeight(vector.y)
                    });
                }
            }
            EditorGUILayout.EndScrollView();

            SplitterGUILayout.EndVerticalSplit();
        }

        static void GenericField<T>(string label, T value)
        {
            if (value is null) return;

            switch (value)
            {
                default:
                    EditorGUILayout.LabelField(label, value.ToString());
                    break;
                case UnityEngine.AnimationCurve v:
                    EditorGUILayout.CurveField(label, v);
                    break;
                case UnityEngine.Object v:
                    EditorGUILayout.ObjectField(label, v, v.GetType(), true);
                    break;
            }
        }
    }
}