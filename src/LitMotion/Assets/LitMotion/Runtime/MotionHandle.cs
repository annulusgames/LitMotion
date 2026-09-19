using System;

namespace LitMotion
{
    /// <summary>
    /// An identifier that represents a specific motion entity.
    /// </summary>
    public struct MotionHandle : IEquatable<MotionHandle>
    {
        /// <summary>
        /// Represents an empty MotionHandle. This is the same as default(MotionHandle).
        /// </summary>
        public static readonly MotionHandle None = default;

        /// <summary>
        /// The ID of motion storage.
        /// </summary>
        public int StorageId;

        /// <summary>
        /// The ID of motion entity.
        /// </summary>
        public int Index;

        /// <summary>
        /// The generational version of motion entity.
        /// </summary>
        public int Version;

        /// <summary>
        /// Motion time
        /// </summary>
        public readonly double Time
        {
            get
            {
                return MotionManager.GetDataRef(this, false).State.Time;
            }
            set
            {
                MotionManager.SetTime(this, value);
            }
        }

        /// <summary>
        /// The delay of the motion
        /// </summary>
        public readonly float Delay
        {
            get
            {
                return MotionManager.GetDataRef(this, false).Parameters.Delay;
            }
        }

        /// <summary>
        /// The duration of the motion
        /// </summary>
        public readonly float Duration
        {
            get
            {
                return MotionManager.GetDataRef(this, false).Parameters.Duration;
            }
        }

        /// <summary>
        /// The total duration of the motion
        /// </summary>
        public readonly double TotalDuration
        {
            get
            {
                return MotionManager.GetDataRef(this, false).Parameters.TotalDuration;
            }
        }

        /// <summary>
        /// The number of loops
        /// </summary>
        public readonly int Loops
        {
            get
            {
                return MotionManager.GetDataRef(this, false).Parameters.Loops;
            }
        }

        /// <summary>
        /// The number of loops completed
        /// </summary>
        public readonly int CompletedLoops
        {
            get
            {
                return MotionManager.GetDataRef(this).State.CompletedLoops;
            }
        }

        /// <summary>
        /// Motion playback speed.
        /// </summary>
        public readonly float PlaybackSpeed
        {
            get
            {
                return MotionManager.GetDataRef(this).State.PlaybackSpeed;
            }
            set
            {
                MotionManager.GetDataRef(this).State.PlaybackSpeed = value;
            }
        }

        public TValue GetEndValue<TValue, TOptions>(bool checkIsInSequence = true)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
        {
            return MotionManager.GetTypeDataRef<TValue, TOptions>(this, checkIsInSequence).EndValue;
        }

        public void SetEndValue<TValue, TOptions>(TValue value, bool checkIsInSequence = true)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
        {
            MotionManager.GetTypeDataRef<TValue, TOptions>(this, checkIsInSequence).EndValue = value;
        }

        public ref TValue GetEndValueRef<TValue, TOptions>(bool checkIsInSequence = true)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
        {
            return ref MotionManager.GetTypeDataRef<TValue, TOptions>(this, checkIsInSequence).EndValue;
        }

        public ref TOptions GetOptions<TValue, TOptions>(bool checkIsInSequence = true)
            where TValue : unmanaged
            where TOptions : unmanaged, IMotionOptions
        {
            return ref MotionManager.GetTypeDataRef<TValue, TOptions>(this, checkIsInSequence).Options;
        }

        public override readonly string ToString()
        {
            return $"MotionHandle`{StorageId} ({Index}:{Version})";
        }

        public readonly bool Equals(MotionHandle other)
        {
            return Index == other.Index && Version == other.Version && StorageId == other.StorageId;
        }

        public override readonly bool Equals(object obj)
        {
            if (obj is MotionHandle handle) return Equals(handle);
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Index, Version, StorageId);
        }

        public static bool operator ==(MotionHandle a, MotionHandle b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(MotionHandle a, MotionHandle b)
        {
            return !(a == b);
        }
    }
}