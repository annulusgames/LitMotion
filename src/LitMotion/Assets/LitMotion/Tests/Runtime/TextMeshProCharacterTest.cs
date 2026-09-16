#if LITMOTION_TEST_TMP
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitMotion.Extensions;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Constraints;
using Is = UnityEngine.TestTools.Constraints.Is;

namespace LitMotion.Tests.Runtime
{
    public class TextMeshProCharacterTest
    {
        readonly List<MotionHandle> handles = new();
        Canvas canvas;
        TextMeshProUGUI text;
        TextMeshPro worldText;

        [SetUp]
        public void SetUp()
        {
            if (Resources.Load<TMP_Settings>("TMP Settings") == null)
            {
                Assert.Ignore("Import TMP Essential Resources to run the TextMesh Pro tests.");
            }

            canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            text = new GameObject("Text").AddComponent<TextMeshProUGUI>();
            text.rectTransform.SetParent(canvas.transform, false);
            text.rectTransform.sizeDelta = new Vector2(500f, 100f);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var handle in handles)
            {
                handle.TryCancel();
            }
            handles.Clear();

            if (canvas != null)
            {
                UnityEngine.Object.Destroy(canvas.gameObject);
            }

            if (worldText != null)
            {
                UnityEngine.Object.Destroy(worldText.gameObject);
            }
        }

        [UnityTest]
        public IEnumerator Test_UntouchedCharacter_KeepsRichTextColor()
        {
            SetText("<color=#FF0000>a</color>bc");
            Track(LMotion.Create(Vector3.one * 0.5f, Vector3.one * 0.5f, 10f)
                .WithImmediateBind()
                .BindToTMPCharScale(text, 2));
            yield return null;

            AssertQuadScale(2, 0.5f);
            Assert.That(GetMeshColor(0), Is.EqualTo(new Color32(255, 0, 0, 255)));
        }

        [UnityTest]
        public IEnumerator Test_UntouchedCharacter_KeepsGeneratedColor_WorldSpaceText()
        {
            // TextMeshPro (3D) converts vertex colors in linear color space, which untouched characters must match.
            worldText = new GameObject("WorldText").AddComponent<TextMeshPro>();
            worldText.text = "<color=#808080>a</color><color=#3366CC>b</color>c";
            worldText.ForceMeshUpdate();
            var generatedColors = worldText.mesh.colors32;

            Track(LMotion.Create(Vector3.one * 0.5f, Vector3.one * 0.5f, 10f)
                .WithImmediateBind()
                .BindToTMPCharScale(worldText, 2));
            yield return null;

            var colors = worldText.mesh.colors32;
            for (int i = 0; i < 2; i++)
            {
                var vertexIndex = worldText.textInfo.characterInfo[i].vertexIndex;
                Assert.That(colors[vertexIndex], Is.EqualTo(generatedColors[vertexIndex]));
            }
        }

        [UnityTest]
        public IEnumerator Test_ColorTint_MultipliesGeneratedColor()
        {
            SetText("abc");
            Track(LMotion.Create(0.5f, 0.5f, 10f)
                .WithImmediateBind()
                .BindToTMPCharColorA(text, 0));
            yield return null;

            text.color = new Color(0f, 1f, 0f, 1f);
            text.ForceMeshUpdate();

            Assert.That(GetMeshColor(0), Is.EqualTo(new Color32(0, 255, 0, 128)));
            Assert.That(GetMeshColor(1), Is.EqualTo(new Color32(0, 255, 0, 255)));
        }

        [UnityTest]
        public IEnumerator Test_BindOnLaterFrame_KeepsDelayedStartValue()
        {
            SetText("abc");
            Track(LMotion.Create(Vector3.zero, Vector3.one, 1f)
                .WithDelay(10f)
                .WithImmediateBind()
                .BindToTMPCharScale(text, 0));
            yield return null;
            yield return null;

            Track(LMotion.Create(Vector3.zero, Vector3.up * 10f, 10f)
                .WithImmediateBind()
                .BindToTMPCharPosition(text, 1));
            yield return null;
            yield return null;

            AssertQuadScale(0, 0f);
        }

        [UnityTest]
        public IEnumerator Test_FinalValues_SurviveMeshRebuild()
        {
            SetText("abc");
            LMotion.Create(Vector3.one, Vector3.zero, 1f)
                .BindToTMPCharScale(text, 0)
                .Complete();
            yield return null;

            text.text = "abcd";
            text.ForceMeshUpdate();

            AssertQuadScale(0, 0f);
            AssertQuadScale(1, 1f);
        }

        [UnityTest]
        public IEnumerator Test_NeutralFinalValues_ReleaseAnimator()
        {
            SetText("abc");
            LMotion.Create(Vector3.zero, Vector3.one, 1f)
                .BindToTMPCharScale(text, 0)
                .Complete();
            Assert.That(GetAnimator(text), Is.Not.Null);
            yield return null;

            Assert.That(GetAnimator(text), Is.Null);
            AssertQuadScale(0, 1f);
        }

        [UnityTest]
        public IEnumerator Test_Cancel_ReleasesAnimator()
        {
            SetText("abc");
            LMotion.Create(Vector3.zero, Vector3.up * 10f, 10f)
                .BindToTMPCharPosition(text, 0)
                .Cancel();
            yield return null;

            Assert.That(GetAnimator(text), Is.Null);
        }

        [UnityTest]
        public IEnumerator Test_ResetTMPChars_RestoresGeneratedMesh()
        {
            SetText("abc");
            LMotion.Create(Vector3.one, Vector3.zero, 1f)
                .BindToTMPCharScale(text, 0)
                .Complete();
            yield return null;
            AssertQuadScale(0, 0f);

            text.ResetTMPChars();
            yield return null;

            Assert.That(GetAnimator(text), Is.Null);
            AssertQuadScale(0, 1f);
        }

        [UnityTest]
        public IEnumerator Test_ApplyingCharacters_DoesNotAllocate()
        {
            SetText("<color=#FF0000>ab</color>c");
            Track(LMotion.Create(0.5f, 0.5f, 10f)
                .WithImmediateBind()
                .BindToTMPCharScaleXYZ(text, 1));
            Track(LMotion.Create(0.5f, 0.5f, 10f)
                .WithImmediateBind()
                .BindToTMPCharColorA(text, 2));
            yield return null;

            var animator = GetAnimator(text);
            var flush = (Action)CreateDelegate(typeof(Action), animator, "Flush");
            var onPreRenderText = (Action<TMP_TextInfo>)CreateDelegate(typeof(Action<TMP_TextInfo>), animator, "OnPreRenderText");
            var textInfo = text.textInfo;

            flush();
            onPreRenderText(textInfo);

            Assert.That(() => flush(), Is.Not.AllocatingGCMemory());
            Assert.That(() => onPreRenderText(textInfo), Is.Not.AllocatingGCMemory());
        }

        void SetText(string value)
        {
            text.text = value;
            text.ForceMeshUpdate();
        }

        void Track(MotionHandle handle)
        {
            handles.Add(handle);
        }

        Color32 GetMeshColor(int charIndex)
        {
            return text.mesh.colors32[text.textInfo.characterInfo[charIndex].vertexIndex];
        }

        void AssertQuadScale(int charIndex, float scale)
        {
            var charInfo = text.textInfo.characterInfo[charIndex];
            var vertices = text.mesh.vertices;
            var generatedDiagonal = charInfo.vertex_TR.position - charInfo.vertex_BL.position;
            var meshDiagonal = vertices[charInfo.vertexIndex + 2] - vertices[charInfo.vertexIndex];
            Assert.That(meshDiagonal.magnitude, Is.EqualTo(generatedDiagonal.magnitude * scale).Within(0.01f));
        }

        static object GetAnimator(TMP_Text text)
        {
            var type = typeof(LitMotionTextMeshProExtensions).Assembly.GetType("LitMotion.Extensions.TextMeshProMotionAnimator");
            var animators = (IDictionary)type.GetField("textToAnimator", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            return animators.Contains(text) ? animators[text] : null;
        }

        static Delegate CreateDelegate(Type delegateType, object target, string methodName)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            return Delegate.CreateDelegate(delegateType, target, method);
        }
    }
}
#endif
