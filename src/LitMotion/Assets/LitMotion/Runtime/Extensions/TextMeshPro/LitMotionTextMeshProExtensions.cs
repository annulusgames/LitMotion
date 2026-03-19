#if LITMOTION_SUPPORT_TMP
using System.Buffers;
using UnityEngine;
using Unity.Collections;
using TMPro;

#if LITMOTION_SUPPORT_ZSTRING
using Cysharp.Text;
#endif

namespace LitMotion.Extensions
{
    /// <summary>
    /// Provides binding extension methods for TMP_Text
    /// </summary>
    public static class LitMotionTextMeshProExtensions
    {
        /// <summary>
        /// Create a motion data and bind it to TMP_Text.fontSize
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToFontSize<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.fontSize = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.maxVisibleCharacters
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaxVisibleCharacters<TOptions, TAdapter>(this MotionBuilder<int, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.maxVisibleCharacters = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.maxVisibleLines
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaxVisibleLines<TOptions, TAdapter>(this MotionBuilder<int, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.maxVisibleLines = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.characterSpacing
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToCharacterSpacing<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.characterSpacing = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.wordSpacing
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToWordSpacing<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.wordSpacing = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.paragraphSpacing
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToParagraphSpacing<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.paragraphSpacing = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.lineSpacing
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToLineSpacing<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.lineSpacing = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.maxVisibleWords
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToMaxVisibleWords<TOptions, TAdapter>(this MotionBuilder<int, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.maxVisibleWords = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.color
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColor<TOptions, TAdapter>(this MotionBuilder<Color, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Color, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                target.color = x;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.color.r
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorR<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var c = target.color;
                c.r = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.color.g
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorG<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var c = target.color;
                c.g = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.color.b
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorB<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var c = target.color;
                c.b = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.color.a
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToColorA<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var c = target.color;
                c.a = x;
                target.color = c;
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<FixedString32Bytes, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<FixedString32Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var enumerator = x.GetEnumerator();
                var length = 0;
                var buffer = ArrayPool<char>.Shared.Rent(64);
                fixed (char* c = buffer)
                {
                    Unicode.Utf8ToUtf16(x.GetUnsafePtr(), x.Length, c, out length, x.Length * 2);
                }
                target.SetText(buffer, 0, length);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<FixedString64Bytes, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<FixedString64Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var enumerator = x.GetEnumerator();
                var length = 0;
                var buffer = ArrayPool<char>.Shared.Rent(128);
                fixed (char* c = buffer)
                {
                    Unicode.Utf8ToUtf16(x.GetUnsafePtr(), x.Length, c, out length, x.Length * 2);
                }
                target.SetText(buffer, 0, length);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<FixedString128Bytes, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<FixedString128Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var enumerator = x.GetEnumerator();
                var length = 0;
                var buffer = ArrayPool<char>.Shared.Rent(256);
                fixed (char* c = buffer)
                {
                    Unicode.Utf8ToUtf16(x.GetUnsafePtr(), x.Length, c, out length, x.Length * 2);
                }
                target.SetText(buffer, 0, length);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<FixedString512Bytes, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<FixedString512Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var enumerator = x.GetEnumerator();
                var length = 0;
                var buffer = ArrayPool<char>.Shared.Rent(1024);
                fixed (char* c = buffer)
                {
                    Unicode.Utf8ToUtf16(x.GetUnsafePtr(), x.Length, c, out length, x.Length * 2);
                }
                target.SetText(buffer, 0, length);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<FixedString4096Bytes, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<FixedString4096Bytes, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var enumerator = x.GetEnumerator();
                var length = 0;
                var buffer = ArrayPool<char>.Shared.Rent(8192);
                fixed (char* c = buffer)
                {
                    Unicode.Utf8ToUtf16(x.GetUnsafePtr(), x.Length, c, out length, x.Length * 2);
                }
                target.SetText(buffer, 0, length);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<int, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var buffer = ArrayPool<char>.Shared.Rent(128);
                var bufferOffset = 0;
                Utf16StringHelper.WriteInt32(ref buffer, ref bufferOffset, x);
                target.SetText(buffer, 0, bufferOffset);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="format">Format string</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<int, TOptions, TAdapter> builder, TMP_Text text, string format)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<int, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, format, static (x, text, format) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                text.SetTextFormat(format, x);
#else
                text.text = string.Format(format, x);
#endif
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<long, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<long, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
                var buffer = ArrayPool<char>.Shared.Rent(128);
                var bufferOffset = 0;
                Utf16StringHelper.WriteInt64(ref buffer, ref bufferOffset, x);
                target.SetText(buffer, 0, bufferOffset);
                ArrayPool<char>.Shared.Return(buffer);
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="format">Format string</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<long, TOptions, TAdapter> builder, TMP_Text text, string format)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<long, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, format, static (x, text, format) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                text.SetTextFormat(format, x);
#else
                text.text = string.Format(format, x);
#endif
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <remarks>
        /// Note: This extension method uses TMP_Text.SetText() to achieve zero allocation, so it is recommended to use this method when binding to text.
        /// </remarks>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            const string format = "{0}";
            Error.IsNull(text);
            return builder.Bind(text, static (x, target) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                target.SetTextFormat(format, x);
#else
                target.SetText(format, x);
#endif
            });
        }

        /// <summary>
        /// Create a motion data and bind it to TMP_Text.text.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="format">Format string</param>
        /// <returns>Handle of the created motion data.</returns>
        public unsafe static MotionHandle BindToText<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, string format)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            Error.IsNull(text);
            return builder.Bind(text, format, static (x, text, format) =>
            {
#if LITMOTION_SUPPORT_ZSTRING
                text.SetTextFormat(format, x);
#else
                text.text = string.Format(format, x);
#endif
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character color.
        /// </summary>
        /// <typeparam name="TValue">The type of value to animate</typeparam>
        /// <typeparam name="TOptions">The type of special parameters given to the motion entity</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <param name="action">Action to update the character</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPChar<TValue, TOptions, TAdapter>(this MotionBuilder<TValue, TOptions, TAdapter> builder, TMP_Text text, int charIndex, TMPCharacterMotionUpdateAction<TValue> action)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
        {
            Error.IsNull(text);

            var animator = TextMeshProMotionAnimator.Get(text);
            animator.EnsureCapacity(charIndex + 1);
            var handle = builder.WithOnComplete(animator.completeAction).Bind(animator, Box.Create(charIndex), action, static (x, animator, charIndex, action) =>
            {
                action(x, charIndex.Value, ref animator.charInfoArray[charIndex.Value]);
                animator.SetDirty();
            });

            return handle;
        }

        /// <summary>
        /// Create motion data and bind it to the character color.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharColor<TOptions, TAdapter>(this MotionBuilder<Color, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Color, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Color x, int _, ref TMPMotionCharacter c) =>
            {
                c.Color = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character color.r.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharColorR<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Color.r = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character color.g.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharColorG<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Color.g = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character color.b.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharColorB<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Color.b = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character color.a.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharColorA<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Color.a = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPosition<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector3 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.x.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPositionX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position.x = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.y.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPositionY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position.y = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.z.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPositionZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position.z = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.xy.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPositionXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position.x = x.x;
                c.Position.y = x.y;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.yz.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPositionYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position.y = x.x;
                c.Position.z = x.y;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character position.xz.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharPositionXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Position.x = x.x;
                c.Position.z = x.y;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharRotation<TOptions, TAdapter>(this MotionBuilder<Quaternion, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Quaternion, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Quaternion x, int _, ref TMPMotionCharacter c) =>
            {
                c.Rotation = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAngles<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector3 x, int _, ref TMPMotionCharacter c) =>
             {
                 c.Rotation = Quaternion.Euler(x);
             });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAnglesX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                var eulerAngles = c.Rotation.eulerAngles;
                eulerAngles.x = x;
                c.Rotation = Quaternion.Euler(eulerAngles);
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAnglesY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                var eulerAngles = c.Rotation.eulerAngles;
                eulerAngles.y = x;
                c.Rotation = Quaternion.Euler(eulerAngles);
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAnglesZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                var eulerAngles = c.Rotation.eulerAngles;
                eulerAngles.z = x;
                c.Rotation = Quaternion.Euler(eulerAngles);
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAnglesXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                var eulerAngles = c.Rotation.eulerAngles;
                eulerAngles.x = x.x;
                eulerAngles.y = x.y;
                c.Rotation = Quaternion.Euler(eulerAngles);
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAnglesYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                var eulerAngles = c.Rotation.eulerAngles;
                eulerAngles.y = x.x;
                eulerAngles.z = x.y;
                c.Rotation = Quaternion.Euler(eulerAngles);
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character rotation (using euler angles).
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharEulerAnglesXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                var eulerAngles = c.Rotation.eulerAngles;
                eulerAngles.x = x.x;
                eulerAngles.z = x.y;
                c.Rotation = Quaternion.Euler(eulerAngles);
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharScale<TOptions, TAdapter>(this MotionBuilder<Vector3, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector3, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector3 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.x.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharScaleX<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale.x = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.y.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharScaleY<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale.y = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.z.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharScaleZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale.z = x;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.xy.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        public static MotionHandle BindToTMPCharScaleXY<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale.x = x.x;
                c.Scale.y = x.y;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.yz.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        public static MotionHandle BindToTMPCharScaleYZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale.y = x.x;
                c.Scale.z = x.y;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.xz.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        public static MotionHandle BindToTMPCharScaleXZ<TOptions, TAdapter>(this MotionBuilder<Vector2, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Vector2, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (Vector2 x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale.x = x.x;
                c.Scale.z = x.y;
            });
        }

        /// <summary>
        /// Create motion data and bind it to the character scale.xyz.
        /// </summary>
        /// <typeparam name="TOptions">The type of special parameters given to the motion data</typeparam>
        /// <typeparam name="TAdapter">The type of adapter that support value animation</typeparam>
        /// <param name="builder">This builder</param>
        /// <param name="text">Target TMP_Text</param>
        /// <param name="charIndex">Target character index</param>
        /// <returns>Handle of the created motion data.</returns>
        public static MotionHandle BindToTMPCharScaleXYZ<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, TMP_Text text, int charIndex)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            return builder.BindToTMPChar(text, charIndex, static (float x, int _, ref TMPMotionCharacter c) =>
            {
                c.Scale = new Vector3(x, x, x);
            });
        }
    }
}
#endif
