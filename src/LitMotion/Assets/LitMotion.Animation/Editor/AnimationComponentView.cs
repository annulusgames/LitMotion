using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LitMotion.Animation.Editor
{
    public class AnimationComponentView : VisualElement
    {
        VisualElement container;
        readonly VisualElement contextMenuButton;
        readonly Foldout foldout;
        readonly Toggle foldoutToggle;
        readonly VisualElement icon;
        readonly Toggle enabledToggle;
        readonly ProgressBar progressBar;

        public Foldout Foldout => foldout;
        public Toggle FoldoutToggle => foldoutToggle;
        public Toggle EnabledToggle => enabledToggle;
        public VisualElement ContextMenuButton => contextMenuButton;

        public string Text
        {
            get => enabledToggle.text;
            set => enabledToggle.text = value;
        }

        public float Progress
        {
            get => progressBar.value;
            set => progressBar.value = value;
        }

        public StyleBackground Icon
        {
            get => icon.style.backgroundImage;
            set => icon.style.backgroundImage = value;
        }

        public override VisualElement contentContainer => container;

        public AnimationComponentView()
        {
            container = this;

            var root = new HelpBox
            {
                style = {
                    flexDirection = FlexDirection.Column,
                    flexGrow = 1f,
                }
            };
            root.Clear();
            Add(root);

            foldout = new Foldout
            {
                style = {
                    marginLeft = 15f,
                    paddingRight = 3f,
                    alignSelf = Align.Stretch,
                }
            };
            root.Add(foldout);
            foldout.Add(new VisualElement() { style = { height = 5f } });
            foldoutToggle = foldout.Q<Toggle>();
            var foldoutCheck = foldout.Q(className: Foldout.checkmarkUssClassName);
            icon = new VisualElement
            {
                style = {
                    width = 16f,
                    height = 16f,
                    marginTop = 0.5f,
                    marginRight = 2f,
                    backgroundImage = (Texture2D)EditorGUIUtility.IconContent("ScriptableObject Icon").image,
                }
            };
            foldoutCheck.parent.Add(icon);
            enabledToggle = new Toggle
            {
                style = {
                    unityFontStyleAndWeight = FontStyle.Bold,
                }
            };
            enabledToggle.Q(className: Toggle.checkmarkUssClassName).style.marginRight = 6f;
            enabledToggle.schedule.Execute(() =>
            {
                enabledToggle.pickingMode = PickingMode.Ignore;
                enabledToggle.Q(className: Toggle.inputUssClassName).pickingMode = PickingMode.Ignore;
                // enabledToggle.Q(className: Toggle.textUssClassName).pickingMode = PickingMode.Ignore; # エラーが出るのでコメント
                enabledToggle.Q(className: Toggle.checkmarkUssClassName).pickingMode = PickingMode.Position;
            });
            foldoutCheck.parent.Add(enabledToggle);

            progressBar = new ProgressBar
            {
                lowValue = 0f,
                highValue = 1f,
                value = 0f,
                style = {
                    height = 2.5f,
                    position = Position.Absolute,
                    top = 22f,
                    left = 24f,
                    right = 2f,
                    alignSelf = Align.Stretch,
                }
            };
            var background = progressBar.Q(className: AbstractProgressBar.backgroundUssClassName);
            background.style.borderTopWidth = 0f;
            background.style.borderBottomWidth = 0f;
            background.style.borderLeftWidth = 0f;
            background.style.borderRightWidth = 0f;
            var progress = progressBar.Q(className: AbstractProgressBar.progressUssClassName);
            progress.style.backgroundColor = Color.white;
            progress.style.minWidth = 0f;
            progressBar.schedule.Execute(() => progress.style.display = progressBar.value > progressBar.lowValue ? DisplayStyle.Flex : DisplayStyle.None).Every(10);
            root.Add(progressBar);

            contextMenuButton = new VisualElement
            {
                style = {
                    height = 15f,
                    width = 15f,
                    top = 4f,
                    right = 4f,
                    position = Position.Absolute,
                    backgroundImage = (Texture2D)EditorGUIUtility.IconContent("_Menu").image,
                }
            };
            root.Add(contextMenuButton);

            var dynamicContainer = new VisualElement { name = "dynamic-container" };
            container = dynamicContainer;
            foldout.contentContainer.Add(container);
        }

        public new void SetEnabled(bool enabled)
        {
            Foldout.contentContainer.SetEnabled(enabled && EnabledToggle.value);
            EnabledToggle.SetEnabled(enabled);
            icon.SetEnabled(enabled);
            contextMenuButton.SetEnabled(enabled);
            progressBar.SetEnabled(enabled);
        }
    }
}