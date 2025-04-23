using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace LitMotion.Animation.Editor
{
    [CustomEditor(typeof(LitMotionAnimation))]
    public sealed class LitMotionAnimationEditor : UnityEditor.Editor
    {
        SerializedProperty componentsProperty;
        int prevArraySize;

        AddAnimationComponentDropdown dropdown;
        VisualElement componentRoot;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            componentRoot = new VisualElement();

            componentsProperty = serializedObject.FindProperty("components");
            prevArraySize = componentsProperty.arraySize;

            dropdown = new AddAnimationComponentDropdown(new());
            dropdown.OnTypeSelected += type =>
            {
                var last = componentsProperty.arraySize;
                componentsProperty.InsertArrayElementAtIndex(componentsProperty.arraySize);
                var property = componentsProperty.GetArrayElementAtIndex(last);
                property.managedReferenceValue = ReflectionHelper.CreateDefaultInstance(type);
                serializedObject.ApplyModifiedProperties();
            };

            root.Add(CreateSettingsPanel());
            componentRoot.Add(CreateComponentsPanel());
            root.Add(componentRoot);
            root.Add(CreateDebugPanel());

            return root;
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnDisable()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && target != null)
            {
                ((LitMotionAnimation)target).Stop();
            }

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                ((LitMotionAnimation)target).Stop();
            }
        }

        VisualElement CreateBox(string label)
        {
            var box = new Box
            {
                style = {
                    marginTop = 6f,
                    marginBottom = 2f,
                    paddingLeft = 4f,
                    alignItems = Align.Stretch,
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1f,
                }
            };

            box.Add(new Label(label)
            {
                style = {
                    marginTop = 5f,
                    marginBottom = 3f,
                    unityFontStyleAndWeight = FontStyle.Bold
                }
            });

            return box;
        }

        VisualElement CreateSettingsPanel()
        {
            var box = CreateBox("Settings");
            box.Add(new PropertyField(serializedObject.FindProperty("playOnAwake")));
            box.Add(new PropertyField(serializedObject.FindProperty("animationMode")));
            return box;
        }

        VisualElement CreateComponentsPanel()
        {
            var box = CreateBox("Components");
            var views = new List<AnimationComponentView>();

            for (int i = 0; i < componentsProperty.arraySize; i++)
            {
                var property = componentsProperty.GetArrayElementAtIndex(i);
                var view = CreateComponentGUI(property.Copy());
                CreateContextMenuManipulator(componentsProperty, i, false).target = view.Foldout.Q<Toggle>();

                var enabledProperty = property.FindPropertyRelative("enabled");
                if (enabledProperty != null)
                {
                    view.EnabledToggle.BindProperty(enabledProperty);
                }

                box.Add(view);
                views.Add(view);
                CreateContextMenuManipulator(componentsProperty, i, true).target = view.ContextMenuButton;
            }

            var addButton = new Button()
            {
                text = "Add...",
                style = {
                    width = 200f,
                    alignSelf = Align.Center
                }
            };
            addButton.clicked += () => dropdown.Show(addButton.worldBound);
            box.Add(addButton);

            box.schedule.Execute(() =>
            {
                var enabled = IsActive();
                foreach (var view in views)
                {
                    view.SetEnabled(enabled);
                }
                addButton.SetEnabled(enabled);
            }).Every(10);

            box.schedule.Execute(() =>
            {
                if (componentsProperty.arraySize != prevArraySize)
                {
                    RefleshComponentsView(true);
                    prevArraySize = componentsProperty.arraySize;
                }

                var components = ((LitMotionAnimation)target).Components;
                for (int i = 0; i < views.Count; i++)
                {
                    if (components.Count <= i)
                    {
                        views[i].Progress = 0f;
                        continue;
                    }

                    var component = components[i];
                    if (component == null)
                    {
                        views[i].Progress = 0f;
                        continue;
                    }

                    var handle = component.TrackedHandle;

                    if (handle.IsActive() && !double.IsInfinity(handle.TotalDuration))
                    {
                        views[i].Progress = Mathf.InverseLerp(0f, (float)handle.TotalDuration, (float)handle.Time);
                    }
                    else
                    {
                        views[i].Progress = 0f;
                    }
                }
            })
            .Every(10);

            return box;
        }

        VisualElement CreateDebugPanel()
        {
            var box = CreateBox("Debug");
            var buttonGroup = new VisualElement
            {
                style = {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1f,
                }
            };
            var playButton = new Button(() => {
                ((LitMotionAnimation)target).Play();
                if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                {
                    PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
                    PrefabStage.prefabStageClosing += OnPrefabStageClosing;
                }
            })
            {
                text = "Play",
                style = {
                    flexGrow = 1f,
                }
            };
            var restartButton = new Button(() => ((LitMotionAnimation)target).Restart())
            {
                text = "Restart",
                style = {
                    flexGrow = 1f,
                }
            };
            var stopButton = new Button(() => ((LitMotionAnimation)target).Pause())
            {
                text = "Pause",
                style = {
                    flexGrow = 1f,
                }
            };
            var resetButton = new Button(() => ((LitMotionAnimation)target).Stop())
            {
                text = "Stop",
                style = {
                    flexGrow = 1f,
                }
            };

            buttonGroup.Add(playButton);
            buttonGroup.Add(restartButton);
            buttonGroup.Add(stopButton);
            buttonGroup.Add(resetButton);

            buttonGroup.schedule.Execute(() =>
            {
                var enabled = !IsActive();
                restartButton.SetEnabled(enabled);
                stopButton.SetEnabled(enabled);
                resetButton.SetEnabled(enabled);
            })
            .Every(10);

            box.Add(buttonGroup);

            return box;
        }

        void RefleshComponentsView(bool applyModifiedProperties)
        {
            if (applyModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }

            componentRoot.Clear();
            componentRoot.Add(CreateComponentsPanel());
        }

        AnimationComponentView CreateComponentGUI(SerializedProperty property)
        {
            var view = new AnimationComponentView();

            if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
            {
                view.Text = "(Missing)";
                view.Icon = (Texture2D)EditorGUIUtility.IconContent("Error").image;
                view.EnabledToggle.value = true;
                view.SetEnabled(true);
                view.EnabledToggle.Q("unity-checkmark").style.visibility = Visibility.Hidden;
                view.Add(new HelpBox("The type referenced in SerializeReference is missing. You may have renamed the type or moved it to a different namespace or assembly.", HelpBoxMessageType.Error));
            }
            else
            {
                view.Text = property.FindPropertyRelative("displayName").stringValue;

                var targetProperty = property.FindPropertyRelative("target");
                if (targetProperty != null)
                {
                    view.Icon = GUIHelper.GetComponentIcon(targetProperty.GetPropertyType());
                }

                view.TrackPropertyValue(property.FindPropertyRelative("displayName"), x =>
                {
                    view.Text = x.stringValue;
                });

                view.Foldout.BindProperty(property);

                var endProperty = property.GetEndProperty();
                var isFirst = true;
                while (property.NextVisible(isFirst))
                {
                    if (SerializedProperty.EqualContents(property, endProperty)) break;
                    if (property.name == "enabled") continue;
                    isFirst = false;

                    view.Add(new PropertyField(property));
                }
            }

            return view;
        }

        ContextualMenuManipulator CreateContextMenuManipulator(SerializedProperty property, int arrayIndex, bool activeLeftClick)
        {
            var manipulator = new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Reset", x =>
                {
                    Undo.RecordObject(serializedObject.targetObject, "Reset LitMotionAnimation component");
                    var elementProperty = property.GetArrayElementAtIndex(arrayIndex);
                    elementProperty.managedReferenceValue = ReflectionHelper.CreateDefaultInstance(elementProperty.managedReferenceValue.GetType());
                    RefleshComponentsView(true);
                }, string.IsNullOrEmpty(property.GetArrayElementAtIndex(arrayIndex).managedReferenceFullTypename) ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);

                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Remove Component", x =>
                {
                    property.DeleteArrayElementAtIndex(arrayIndex);
                    RefleshComponentsView(true);
                });

                evt.menu.AppendAction("Move Up", x =>
                {
                    property.MoveArrayElement(arrayIndex, arrayIndex - 1);
                    RefleshComponentsView(true);
                }, arrayIndex == 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);


                evt.menu.AppendAction("Move Down", x =>
                {
                    property.MoveArrayElement(arrayIndex, arrayIndex + 1);
                    RefleshComponentsView(true);
                }, arrayIndex == property.arraySize - 1 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
            });

            if (activeLeftClick)
            {
                manipulator.activators.Add(new ManipulatorActivationFilter
                {
                    button = MouseButton.LeftMouse,
                });
            }

            return manipulator;
        }

        bool IsActive()
        {
            return !((LitMotionAnimation)target).IsActive;
        }

        void OnPrefabStageClosing(PrefabStage stage)
        {
            PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
            foreach (var  i in stage.prefabContentsRoot.GetComponentsInChildren<LitMotionAnimation>(true))
            {
                i.Stop();
            }
        }
    }
}
