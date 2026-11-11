using System;

namespace LitMotion
{
    /// <summary>
    /// Options for shake motion.
    /// </summary>
    [Serializable]
    public struct ShakeOptions : IEquatable<ShakeOptions>, IMotionOptions
    {
        public ShakeMode ShakeMode;
        public int Frequency;
        public float DampingRatio;
        public uint RandomSeed;

        public static ShakeOptions Default
        {
            get
            {
                return new ShakeOptions()
                {
                    ShakeMode = ShakeMode.PerlinNoise,
                    Frequency = 10,
                    DampingRatio = 1f
                };
            }
        }

        public readonly bool Equals(ShakeOptions other)
        {
            return other.ShakeMode == ShakeMode &&
                other.Frequency == Frequency &&
                other.DampingRatio == DampingRatio &&
                other.RandomSeed == RandomSeed;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is ShakeOptions options) return Equals(options);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(ShakeMode, Frequency, DampingRatio, RandomSeed);
        }
    }
}