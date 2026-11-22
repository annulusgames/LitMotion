using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if UNITY_7000_0_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace LitMotion.Collections
{
    /// <summary>
    /// A list of minimal features. Note that it is NOT thread-safe and must NOT be marked readonly as it is a mutable struct.
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    [StructLayout(LayoutKind.Auto)]
    public struct FastListCore<T>
    {
        const int InitialCapacity = 8;

#if UNITY_7000_0_OR_NEWER
        [NoAutoStaticsCleanup] // Immutable empty value.
#endif
        public static readonly FastListCore<T> Empty = default;

        T[] array;
        int tailIndex;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T element)
        {
            if (array == null)
            {
                array = new T[InitialCapacity];
            }
            else if (array.Length == tailIndex)
            {
                Array.Resize(ref array, tailIndex * 2);
            }

            array[tailIndex] = element;
            tailIndex++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAtSwapback(int index)
        {
            Error.IsNull(array);
            CheckIndex(index);

            array[index] = array[tailIndex - 1];
            array[tailIndex - 1] = default;
            tailIndex--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(bool removeArray = false)
        {
            if (array == null) return;

            array.AsSpan().Clear();
            tailIndex = 0;
            if (removeArray) array = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureCapacity(int capacity)
        {
            if (array == null)
            {
                array = new T[InitialCapacity];
            }

            while (array.Length < capacity)
            {
                Array.Resize(ref array, array.Length * 2);
            }
        }

        public readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => array[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => array[index] = value;
        }

        public readonly int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => tailIndex;
        }

        public readonly Span<T> AsSpan() => array == null ? Span<T>.Empty : array.AsSpan(0, tailIndex);
        public readonly T[] AsArray() => array;

        readonly void CheckIndex(int index)
        {
            if (index < 0 || index > tailIndex) throw new IndexOutOfRangeException();
        }
    }
}
