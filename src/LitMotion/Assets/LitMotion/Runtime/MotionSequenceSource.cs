using System;
using System.Buffers;
using LitMotion.Collections;

namespace LitMotion
{
    internal struct MotionSequenceItem : IComparable<MotionSequenceItem>
    {
        public MotionSequenceItem(double position, MotionHandle handle)
        {
            Position = position;
            Handle = handle;
        }

        public double Position;
        public MotionHandle Handle;

        public int CompareTo(MotionSequenceItem other)
        {
            return Position.CompareTo(other.Position);
        }
    }

    internal sealed class MotionSequenceSource : ILinkedPoolNode<MotionSequenceSource>
    {
        static LinkedPool<MotionSequenceSource> pool;

        public static MotionSequenceSource Rent()
        {
            if (!pool.TryPop(out var result)) result = new();
            return result;
        }

        public static void Return(MotionSequenceSource source)
        {
            if (source.itemBuffer != null)
            {
                ArrayPool<MotionSequenceItem>.Shared.Return(source.itemBuffer);
                source.itemBuffer = null;
            }

            source.itemCount = default;
            source.duration = default;
            source.time = default;

            pool.TryPush(source);
        }

        public void Initialize(MotionHandle handle, MotionSequenceItem[] itemBuffer, int itemCount, double duration)
        {
            this.handle = handle;
            this.itemCount = itemCount;
            this.itemBuffer = itemBuffer;
            this.duration = duration;
            this.time = 0;

            if (itemBuffer != null) Array.Sort(itemBuffer, 0, itemCount);
        }

        MotionSequenceSource()
        {
            onCompleteDelegate = OnComplete;
            onCancelDelegate = OnCancel;
        }

        readonly Action onCompleteDelegate;
        readonly Action onCancelDelegate;
        MotionSequenceSource next;

        MotionHandle handle;
        MotionSequenceItem[] itemBuffer;
        int itemCount;
        double duration;
        double time;

        public ref MotionSequenceSource NextNode => ref next;

        public Action OnCompleteDelegate => onCompleteDelegate;
        public Action OnCancelDelegate => onCancelDelegate;

        public ReadOnlySpan<MotionSequenceItem> Items => itemBuffer.AsSpan(0, itemCount);

        public double Time
        {
            get => time;
            set
            {
                time = value;

                var span = Items;
                var index = span.Length - 1;
                while (index >= 0)
                {
                    var item = span[index];
                    if (item.Position < time) break;
                    MotionManager.SetTime(item.Handle, time - item.Position, false);
                    index--;
                }

                foreach (var item in span[..(index + 1)])
                {
                    MotionManager.SetTime(item.Handle, time - item.Position, false);
                }
            }
        }

        public double Duration => duration;

        void OnComplete()
        {
            if (handle.IsActive()) return;

            foreach (var item in Items)
            {
                MotionManager.Cancel(item.Handle, false);
            }

            Return(this);
        }

        void OnCancel()
        {
            foreach (var item in Items)
            {
                MotionManager.Cancel(item.Handle, false);
            }

            Return(this);
        }
    }
}