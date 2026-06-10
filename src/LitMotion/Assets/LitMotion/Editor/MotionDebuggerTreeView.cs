using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

#if UNITY_6000_2_OR_NEWER
using TreeView = UnityEditor.IMGUI.Controls.TreeView<int>;
using TreeViewItem = UnityEditor.IMGUI.Controls.TreeViewItem<int>;
using TreeViewState = UnityEditor.IMGUI.Controls.TreeViewState<int>;
#endif

namespace LitMotion.Editor
{
    internal sealed class MotionDebuggerViewItem : TreeViewItem
    {
        public MotionDebuggerViewItem(int id) : base(id) { }

        static readonly Regex removeHref = new("<a href.+>(.+)</a>", RegexOptions.Compiled);

        public MotionHandle Handle { get; set; }
        public string DebugName => debugName ??= Handle.GetDebugName();
        string debugName;

        public string MotionType { get; set; }
        public string SchedulerType { get; set; }

        string stackTrace;
        public string StackTrace
        {
            get { return stackTrace; }
            set
            {
                stackTrace = value;
                StackTraceFirstLine = value == null ? string.Empty : GetFirstLine(stackTrace);
            }
        }

        public string StackTraceFirstLine { get; private set; }

        static string GetFirstLine(string str)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\r' || str[i] == '\n')
                {
                    break;
                }
                sb.Append(str[i]);
            }

            return removeHref.Replace(sb.ToString(), "$1");
        }
    }

    internal sealed class MotionDebuggerTreeView : TreeView
    {
        const string sortedColumnIndexStateKey = "MotionTrackerTreeView_sortedColumnIndex";

        public IReadOnlyList<TreeViewItem> CurrentBindingItems;

        public MotionDebuggerTreeView()
            : this(new TreeViewState(), new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Name"), width = 50},
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Motion Type"), width = 80},
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Time"), width = 30},
            })))
        {

        }

        MotionDebuggerTreeView(TreeViewState state, MultiColumnHeader header)
            : base(state, header)
        {
            rowHeight = 20;
            showAlternatingRowBackgrounds = false;
            showBorder = false;
            header.sortingChanged += HeaderSortingChanged;

            header.ResizeToFit();
            Reload();

            header.sortedColumnIndex = SessionState.GetInt(sortedColumnIndexStateKey, 1);
        }

        public void ReloadAndSort()
        {
            var currentSelected = state.selectedIDs;
            Reload();
            HeaderSortingChanged(multiColumnHeader);
            state.selectedIDs = currentSelected;
        }

        void HeaderSortingChanged(MultiColumnHeader multiColumnHeader)
        {
            SessionState.SetInt(sortedColumnIndexStateKey, multiColumnHeader.sortedColumnIndex);
            var index = multiColumnHeader.sortedColumnIndex;
            var ascending = multiColumnHeader.IsSortedAscending(multiColumnHeader.sortedColumnIndex);

            var items = rootItem.children.Cast<MotionDebuggerViewItem>();
            var orderedEnumerable = index switch
            {
                0 => ascending ? items.OrderBy(item => item.DebugName) : items.OrderByDescending(item => item.DebugName),
                1 => ascending ? items.OrderBy(item => item.MotionType) : items.OrderByDescending(item => item.MotionType),
                2 => ascending ? items.OrderBy(item => item.Handle.Time) : items.OrderByDescending(item => item.Handle.Time),
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
            };
            CurrentBindingItems = rootItem.children = orderedEnumerable.Cast<TreeViewItem>().ToList();
            BuildRows(rootItem);
        }

        static string GetSchedulerName(IMotionScheduler scheduler, bool isCreatedOnEditor)
        {
            static string GetTimeKindName(MotionTimeKind motionTimeKind)
            {
                return motionTimeKind switch
                {
                    MotionTimeKind.Time => "",
                    MotionTimeKind.UnscaledTime => "IgnoreTimeScale",
                    MotionTimeKind.Realtime => "Realtime",
                    _ => null
                };
            }
            if (scheduler == null)
            {
                if (isCreatedOnEditor)
                {
                    scheduler = MotionScheduler.DefaultScheduler;
                }
                else
                {
                    scheduler = EditorMotionScheduler.Update;
                }
            }

            return scheduler switch
            {
                PlayerLoopMotionScheduler loopMotionScheduler => loopMotionScheduler.playerLoopTiming.ToString() + GetTimeKindName(loopMotionScheduler.timeKind),
                ManualMotionDispatcherScheduler => "Manual",
                EditorUpdateMotionScheduler => "EditorUpdate",
                _ => scheduler.GetType()?.Name,
            };
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem { depth = -1 };
            var children = new List<TreeViewItem>();

            var id = 0;
            foreach (var tracking in MotionDebugger.Items)
            {
                if (!tracking.Handle.IsActive()) continue;

                children.Add(new MotionDebuggerViewItem(id)
                {
                    Handle = tracking.Handle,
                    MotionType = $"[{tracking.ValueType.Name}, {tracking.OptionsType.Name}, {tracking.AdapterType.Name}]",
                    SchedulerType = GetSchedulerName(tracking.Scheduler, tracking.CreatedOnEditor),
                    StackTrace = tracking.StackTrace?.AddHyperLink()
                });
                id++;
            }

            CurrentBindingItems = children;
            root.children = CurrentBindingItems as List<TreeViewItem>;
            return root;
        }

        protected override bool CanMultiSelect(TreeViewItem item)
        {
            return false;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item as MotionDebuggerViewItem;

            for (var visibleColumnIndex = 0; visibleColumnIndex < args.GetNumVisibleColumns(); visibleColumnIndex++)
            {
                var rect = args.GetCellRect(visibleColumnIndex);
                var columnIndex = args.GetColumn(visibleColumnIndex);

                var labelStyle = args.selected ? EditorStyles.whiteLabel : EditorStyles.label;
                labelStyle.alignment = TextAnchor.MiddleLeft;
                switch (columnIndex)
                {
                    case 0:
                        EditorGUI.LabelField(rect, item.DebugName, labelStyle);
                        break;
                    case 1:
                        EditorGUI.LabelField(rect, item.MotionType, labelStyle);
                        break;
                    case 2:
                        EditorGUI.LabelField(rect, item.Handle.Time.ToString("00.00"), labelStyle);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(columnIndex), columnIndex, null);
                }
            }
        }

        protected override bool DoesItemMatchSearch(TreeViewItem item, string search)
        {
            var viewItem = item as MotionDebuggerViewItem;
            return viewItem.DebugName.Contains(search, StringComparison.InvariantCultureIgnoreCase);
        }
    }

}
