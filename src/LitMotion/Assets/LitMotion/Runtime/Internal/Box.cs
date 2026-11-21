using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace LitMotion
{
    internal static class Box
    {
        static readonly Box<int> BoxMinus1 = new(-1);
        static readonly Box<int> Box0 = new(0);
        static readonly Box<int> Box1 = new(1);
        static readonly Box<int> Box2 = new(2);
        static readonly Box<int> Box3 = new(3);
        static readonly Box<int> Box4 = new(4);
        static readonly Box<int> Box5 = new(5);
        static readonly Box<int> Box6 = new(6);
        static readonly Box<int> Box7 = new(7);
        static readonly Box<int> Box8 = new(8);
        static readonly Box<int> Box9 = new(9);

        static readonly Box<bool> BoxTrue = new(true);
        static readonly Box<bool> BoxFalse = new(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Box<T> Create<T>(T value)
            where T : struct
        {
            if (typeof(T) == typeof(int))
            {
                var box = Create(UnsafeUtility.As<T, int>(ref value));
                return UnsafeUtility.As<Box<int>, Box<T>>(ref box);
            }
            else if (typeof(T) == typeof(bool))
            {
                var box = Create(UnsafeUtility.As<T, bool>(ref value));
                return UnsafeUtility.As<Box<bool>, Box<T>>(ref box);
            }

            return new Box<T>(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Box<int> Create(int value)
        {
            return value switch
            {
                -1 => BoxMinus1,
                0 => Box0,
                1 => Box1,
                2 => Box2,
                3 => Box3,
                4 => Box4,
                5 => Box5,
                6 => Box6,
                7 => Box7,
                8 => Box8,
                9 => Box9,
                _ => new Box<int>(value),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Box<bool> Create(bool value)
        {
            return value ? BoxTrue : BoxFalse;
        }
    }

    internal sealed record Box<T>
        where T : struct
    {
        internal Box(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}