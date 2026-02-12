#if LITMOTION_SUPPORT_TMP
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using LitMotion.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LitMotion.Extensions
{
    // TODO: optimization

    /// <summary>
    /// Wrapper class for animating individual characters in TextMeshPro.
    /// </summary>
    internal sealed class TextMeshProMotionAnimator
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
#if UNITY_EDITOR
            var domainReloadDisabled = EditorSettings.enterPlayModeOptionsEnabled && EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
            if (!domainReloadDisabled && initialized) return;
#else
            if (initialized) return;
#endif
            PlayerLoopHelper.OnUpdate += UpdateAnimators;
            initialized = true;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void InitEditor()
        {
            EditorApplication.update += UpdateAnimatorsEditor;
        }
#endif

        static bool initialized;
        static TextMeshProMotionAnimator rootNode;

        internal static TextMeshProMotionAnimator Get(TMP_Text text)
        {
            if (textToAnimator.TryGetValue(text, out var animator))
            {
                if (!animator.GetIsDirty())
                {
                    animator.Reset();
                }
                return animator;
            }

            // get or create animator
            animator = rootNode ?? new();
            rootNode = animator.nextNode;
            animator.nextNode = null;

            // set target
            animator.target = text;
            animator.Reset();

            // add to array
            if (tail == animators.Length)
            {
                Array.Resize(ref animators, tail * 2);
            }
            animators[tail] = animator;
            tail++;

            // add to dictionary
            textToAnimator.Add(text, animator);

            return animator;
        }

        internal static void Return(TextMeshProMotionAnimator animator)
        {
            animator.nextNode = rootNode;
            rootNode = animator;

            textToAnimator.Remove(animator.target);
            animator.target = null;
        }

        static readonly Dictionary<TMP_Text, TextMeshProMotionAnimator> textToAnimator = new();
        static TextMeshProMotionAnimator[] animators = new TextMeshProMotionAnimator[8];
        static int tail;

        static void UpdateAnimators()
        {
            var j = tail - 1;

            for (int i = 0; i < animators.Length; i++)
            {
                var animator = animators[i];
                if (animator != null)
                {
                    if (!animator.TryUpdate())
                    {
                        Return(animator);
                        animators[i] = null;
                    }
                    else
                    {
                        continue;
                    }
                }

                while (i < j)
                {
                    var fromTail = animators[j];
                    if (fromTail != null)
                    {
                        if (!fromTail.TryUpdate())
                        {
                            Return(fromTail);
                            animators[j] = null;
                            j--;
                            continue;
                        }
                        else
                        {
                            animators[i] = fromTail;
                            animators[j] = null;
                            j--;
                            goto NEXT_LOOP;
                        }
                    }
                    else
                    {
                        j--;
                    }
                }

                tail = i;
                break;

            NEXT_LOOP:
                continue;
            }
        }

#if UNITY_EDITOR
        static void UpdateAnimatorsEditor()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }
            UpdateAnimators();
        }
#endif

        internal struct CharInfo
        {
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
            public Color color;
            public bool isDirty;
        }

        public TextMeshProMotionAnimator()
        {
            charInfoArray = new CharInfo[32];
            for (int i = 0; i < charInfoArray.Length; i++)
            {
                charInfoArray[i].color = Color.white;
                charInfoArray[i].rotation = Quaternion.identity;
                charInfoArray[i].scale = Vector3.one;
                charInfoArray[i].position = Vector3.zero;
                charInfoArray[i].isDirty = false;
            }
        }

        TMP_Text target;
        internal CharInfo[] charInfoArray;
        bool dirtyFromComplete;
        int lastDirtyChar = -1;
        
        TextMeshProMotionAnimator nextNode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int length)
        {
            var prevLength = charInfoArray.Length;
            if (length > prevLength)
            {
                Array.Resize(ref charInfoArray, length);

                if (length > prevLength)
                {
                    for (int i = prevLength; i < length; i++)
                    {
                        charInfoArray[i].color = new(target.color.r, target.color.g, target.color.b, target.color.a);
                        charInfoArray[i].rotation = Quaternion.identity;
                        charInfoArray[i].scale = Vector3.one;
                        charInfoArray[i].position = Vector3.zero;
                        charInfoArray[i].isDirty = false;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCharDirty(int charIndex)
        {
            EnsureCapacity(charIndex + 1);
            if (charIndex > lastDirtyChar)
            {
                lastDirtyChar = charIndex;
            }
            charInfoArray[charIndex].isDirty = true;
        }

        bool GetIsDirty()
        {
            if (dirtyFromComplete)
            {
                return true;
            }
            return lastDirtyChar >= 0;
        }
        
        public void Reset()
        {
            for (int i = 0; i < charInfoArray.Length; i++)
            {
                charInfoArray[i].color = new(target.color.r, target.color.g, target.color.b, target.color.a);
                charInfoArray[i].rotation = Quaternion.identity;
                charInfoArray[i].scale = Vector3.one;
                charInfoArray[i].position = Vector3.zero;
                charInfoArray[i].isDirty = false;
            }
            dirtyFromComplete = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool TryUpdate()
        {
            if (target == null) return false;
            
            if (GetIsDirty())
            {
                UpdateCore();
                return true;
            }
            return false;
        }

        void UpdateCore()
        {
            target.ForceMeshUpdate();
            var textInfo = target.textInfo;
            EnsureCapacity(textInfo.characterCount);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                ref var charInfo = ref textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                var materialIndex = charInfo.materialReferenceIndex;
                var vertexIndex = charInfo.vertexIndex;

                ref var colors = ref textInfo.meshInfo[materialIndex].colors32;
                ref var motionCharInfo = ref charInfoArray[i];

                var charColor = motionCharInfo.color;
                for (int n = 0; n < 4; n++)
                {
                    colors[vertexIndex + n] = charColor;
                }

                var verts = textInfo.meshInfo[materialIndex].vertices;
                var center = (verts[vertexIndex] + verts[vertexIndex + 2]) * 0.5f;

                var charRotation = motionCharInfo.rotation;
                var charScale = motionCharInfo.scale;
                var charOffset = motionCharInfo.position;
                for (int n = 0; n < 4; n++)
                {
                    var vert = verts[vertexIndex + n];
                    var dir = vert - center;
                    verts[vertexIndex + n] = center +
                        charRotation * new Vector3(dir.x * charScale.x, dir.y * charScale.y, dir.z * charScale.z) +
                        charOffset;
                }
            }

            for (int i = 0; i < textInfo.materialCount; i++)
            {
                if (textInfo.meshInfo[i].mesh == null) continue;
                textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                target.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            if (dirtyFromComplete)
            {
                dirtyFromComplete = false;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CompleteForChar(int charIndex)
        {
            EnsureCapacity(charIndex + 1);
            charInfoArray[charIndex].isDirty = false;
            if (charIndex >= lastDirtyChar)
            {
                lastDirtyChar = -1;
                for (int i = charIndex; i >= 0; i--)
                {
                    if (charInfoArray[i].isDirty)
                    {
                        lastDirtyChar = i;
                    }
                }
            }
            dirtyFromComplete = true;
        }
    }
}
#endif