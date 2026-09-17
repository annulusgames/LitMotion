using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace LitMotion.Animation.Editor
{
    public sealed class AddAnimationComponentDropdown : AdvancedDropdown
    {
        class Item : AdvancedDropdownItem
        {
            public Item(Type type, string menuName) : base(menuName)
            {
                Type = type;
            }

            public Type Type { get; }
        }

        static readonly (Type Type, string MenuName)[] cache = TypeCache.GetTypesDerivedFrom<LitMotionAnimationComponent>()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsSpecialName)
            .Where(x => !x.IsGenericType)
            .Select(x =>
            {
                var attribute = x.GetCustomAttribute<LitMotionAnimationComponentMenuAttribute>();
                var menuName = attribute == null ? x.Name : attribute.MenuName;
                return (x, menuName);
            })
            .OrderBy(x => x.menuName)
            .ToArray();

        public event Action<Type> OnTypeSelected;

        public AddAnimationComponentDropdown(AdvancedDropdownState state) : base(state)
        {
            var minimumSize = this.minimumSize;
            minimumSize.y = 200f;
            this.minimumSize = minimumSize;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Component");
            foreach ((var type, var menuName) in cache)
            {
                var splitStrings = menuName.Split('/');
                var parent = root;
                Item lastItem = null;

                for (int i = 0; i < splitStrings.Length; i++)
                {
                    var str = splitStrings[i];

#if UNITY_6000_5_OR_NEWER
                    var foundChildItem = parent.childList.FirstOrDefault(item => item.name == str);
#else
                    var foundChildItem = parent.children.FirstOrDefault(item => item.name == str);
#endif
                    if (foundChildItem != null)
                    {
                        parent = foundChildItem;
                        lastItem = (Item)foundChildItem;
                        continue;
                    }

                    var child = new Item(type, str);

                    if (i == splitStrings.Length - 1)
                    {
                        var targetField = ReflectionHelper.GetField(type, "target", includingBaseNonPublic: true);
                        if (targetField != null)
                        {
                            child.icon = GUIHelper.GetComponentIcon(targetField.FieldType);
                        }
                        else
                        {
                            child.icon = (Texture2D)EditorGUIUtility.IconContent("ScriptableObject Icon").image;
                        }
                    }

                    parent.AddChild(child);

                    parent = child;
                    lastItem = child;
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);

            if (item is Item componentItem)
            {
                OnTypeSelected?.Invoke(componentItem.Type);
            }
        }
    }
}
