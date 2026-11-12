using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using LitMotion.Adapters;
using LitMotion.Collections;

namespace LitMotion
{
    internal sealed class MotionSequenceBuilderSource : ILinkedPoolNode<MotionSequenceBuilderSource>
    {
        static LinkedPool<MotionSequenceBuilderSource> pool;

        public static MotionSequenceBuilderSource Rent()
        {
            if (!pool.TryPop(out var result)) result = new();
            return result;
        }

        public static void Return(MotionSequenceBuilderSource source)
        {
            if (source.buffer != null)
            {
                ArrayPool<MotionSequenceItem>.Shared.Return(source.buffer);
            }

            source.version++;
            source.buffer = null;
            source.tail = 0;
            source.lastTail = 0;
            source.count = 0;
            source.duration = 0;

            pool.TryPush(source);
        }

        public ref MotionSequenceBuilderSource NextNode => ref next;
        public ushort Version => version;

        MotionSequenceBuilderSource next;
        ushort version;
        MotionSequenceItem[] buffer;
        Ease ease;
        int count;
        double tail;
        double lastTail;
        double duration;

        public void Append(MotionHandle handle)
        {
            MotionManager.AddToSequence(handle, out var motionDuration);
            AddItem(new MotionSequenceItem(tail, handle));
            AppendInterval(motionDuration);
        }

        public void AppendInterval(double interval)
        {
            lastTail = tail;
            tail += interval;
            duration = Math.Max(duration, tail);
        }

        public void Insert(double position, MotionHandle handle)
        {
            MotionManager.AddToSequence(handle, out var motionDuration);
            AddItem(new MotionSequenceItem(position, handle));
            duration = Math.Max(duration, position + motionDuration);
        }

        public void Join(MotionHandle handle)
        {
            Insert(lastTail, handle);
        }

        public void WithEase(Ease ease)
        {
            this.ease = ease;
        }

        public MotionHandle Schedule(Action<MotionBuilder<double, NoOptions, DoubleMotionAdapter>> configuration)
        {
            var source = MotionSequenceSource.Rent();
            var builder = LMotion.Create(0.0, duration, (float)duration)
                .WithEase(ease)
                .WithOnComplete(source.OnCompleteDelegate)
                .WithOnCancel(source.OnCancelDelegate);

            configuration?.Invoke(builder);
            var handle = builder.Bind(source, (x, source) => source.Time = x);

            source.Initialize(handle, buffer, count, duration);
            buffer = null;
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddItem(in MotionSequenceItem item)
        {
            if (buffer == null)
            {
                buffer = ArrayPool<MotionSequenceItem>.Shared.Rent(32);
            }
            else if (buffer.Length == count)
            {
                var newBuffer = ArrayPool<MotionSequenceItem>.Shared.Rent(count * 2);
                buffer.CopyTo(newBuffer.AsSpan(0, count));
                ArrayPool<MotionSequenceItem>.Shared.Return(buffer);
                buffer = newBuffer;
            }

            buffer[count] = item;
            count++;
        }
    }

    public struct MotionSequenceBuilder : IDisposable
    {
        internal MotionSequenceBuilderSource source;
        internal ushort version;

        internal MotionSequenceBuilder(MotionSequenceBuilderSource source)
        {
            this.source = source;
            this.version = source.Version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionSequenceBuilder Append(MotionHandle handle)
        {
            CheckIsDisposed();
            source.Append(handle);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionSequenceBuilder AppendInterval(float interval)
        {
            CheckIsDisposed();
            source.AppendInterval(interval);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionSequenceBuilder Insert(float position, MotionHandle handle)
        {
            CheckIsDisposed();
            source.Insert(position, handle);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionSequenceBuilder Join(MotionHandle handle)
        {
            CheckIsDisposed();
            source.Join(handle);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly MotionSequenceBuilder WithEase(Ease ease)
        {
            CheckIsDisposed();
            source.WithEase(ease);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle Run()
        {
            return Run(null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MotionHandle Run(Action<MotionBuilder<double, NoOptions, DoubleMotionAdapter>> configuration)
        {
            CheckIsDisposed();
            var handle = source.Schedule(configuration);
            Dispose();
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            CheckIsDisposed();
            MotionSequenceBuilderSource.Return(source);
            source = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly void CheckIsDisposed()
        {
            if (source == null || source.Version != version)
            {
                throw new InvalidOperationException("MotionSequenuceBuilder is either not initialized or has already run.");
            }
        }
    }
}