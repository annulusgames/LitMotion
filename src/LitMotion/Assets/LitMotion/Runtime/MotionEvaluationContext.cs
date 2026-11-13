namespace LitMotion
{
    /// <summary>
    /// Provides basic information for evaluating motion.
    /// </summary>
    public struct MotionEvaluationContext
    {
        /// <summary>
        /// Progress (0-1)
        /// </summary>
        public float Progress;

        /// <summary>
        /// Current motion time
        /// </summary>
        public double Time;

        /// <summary>
        /// Delta time for this frame
        /// </summary>
        public double DeltaTime;
    }
}