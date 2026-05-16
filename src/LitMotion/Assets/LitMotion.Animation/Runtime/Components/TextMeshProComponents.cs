#if LITMOTION_ANIMATION_TMP

using System;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace LitMotion.Animation.Components
{
    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Text")]
    public sealed class TMPTextAnimation : FixedString512BytesPropertyAnimationComponent<TMP_Text>
    {
        protected override FixedString512Bytes GetValue(TMP_Text target)
        {
            return target.text;
        }

        protected override void SetValue(TMP_Text target, in FixedString512Bytes value)
        {
            target.text = value.ToString();
        }
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Character Spacing")]
    public sealed class TMPTextCharacterSpacingAnimation : FloatPropertyAnimationComponent<TMP_Text>
    {
        protected override float GetValue(TMP_Text target) => target.characterSpacing;
        protected override void SetValue(TMP_Text target, in float value) => target.characterSpacing = value;
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Word Spacing")]
    public sealed class TMPTextWordSpacingAnimation : FloatPropertyAnimationComponent<TMP_Text>
    {
        protected override float GetValue(TMP_Text target) => target.wordSpacing;
        protected override void SetValue(TMP_Text target, in float value) => target.wordSpacing = value;
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Line Spacing")]
    public sealed class TMPTextLineSpacingAnimation : FloatPropertyAnimationComponent<TMP_Text>
    {
        protected override float GetValue(TMP_Text target) => target.lineSpacing;
        protected override void SetValue(TMP_Text target, in float value) => target.lineSpacing = value;
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Paragraph Spacing")]
    public sealed class TMPTextParagraphSpacingAnimation : FloatPropertyAnimationComponent<TMP_Text>
    {
        protected override float GetValue(TMP_Text target) => target.paragraphSpacing;
        protected override void SetValue(TMP_Text target, in float value) => target.paragraphSpacing = value;
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Font Size")]
    public sealed class TMPTextFontSizeAnimation : FloatPropertyAnimationComponent<TMP_Text>
    {
        protected override float GetValue(TMP_Text target) => target.fontSize;
        protected override void SetValue(TMP_Text target, in float value) => target.fontSize = value;
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Color")]
    public sealed class TMPTextColorAnimation : ColorPropertyAnimationComponent<TMP_Text>
    {
        protected override Color GetValue(TMP_Text target) => target.color;
        protected override void SetValue(TMP_Text target, in Color value) => target.color = value;
    }

    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Color (Alpha)")]
    public sealed class TMPTextColorAlphaAnimation : FloatPropertyAnimationComponent<TMP_Text>
    {
        protected override float GetValue(TMP_Text target) => target.color.a;
        protected override void SetValue(TMP_Text target, in float value)
        {
            var c = target.color;
            c.a = value;
            target.color = c;
        }
    }
    [Serializable]
    [LitMotionAnimationComponentMenu("UI/TextMesh Pro/Color (For Sprite)")]
    public sealed class TMPTextColorCanvasRendererAnimation : ColorPropertyAnimationComponent<TextMeshProUGUI>
    {
        protected override Color GetValue(TextMeshProUGUI target) => target.canvasRenderer.GetColor();
        protected override void SetValue(TextMeshProUGUI target, in Color value)
        {
            target.havePropertiesChanged = true;
            target.canvasRenderer.SetColor(value);
        }
    }
}

#endif