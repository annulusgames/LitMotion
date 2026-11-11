namespace LitMotion
{
    /// <summary>
    /// Noise used to generate shake motion.
    /// </summary>
    public enum ShakeMode : byte
    {
        /// <summary>
        /// Independent random values at each evaluation time.
        /// </summary>
        Random = 0,

        /// <summary>
        /// Continuous Perlin noise sampled from motion progress.
        /// </summary>
        PerlinNoise = 1,
    }
}
