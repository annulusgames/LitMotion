using System;
using AOT;
using LitMotion.Adapters;
using NUnit.Framework;
using Unity.Burst;

namespace LitMotion.Tests.Runtime
{
    public class CustomEaseTest
    {
        [BurstCompile]
        static class CustomEases
        {
            [BurstCompile]
            [MonoPInvokeCallback(typeof(CustomEaseDelegate))]
            public static float InQuad(float t) => t * t;

            [BurstCompile]
            [MonoPInvokeCallback(typeof(CustomEaseDelegate))]
            public static float OutCubic(float t)
            {
                var x = 1f - t;
                return 1f - x * x * x;
            }
        }

        static readonly FunctionPointer<CustomEaseDelegate> inQuad =
            BurstCompiler.CompileFunctionPointer<CustomEaseDelegate>(CustomEases.InQuad);
        static readonly FunctionPointer<CustomEaseDelegate> outCubic =
            BurstCompiler.CompileFunctionPointer<CustomEaseDelegate>(CustomEases.OutCubic);

        [Test]
        public void Test_CustomEase_ImmediateBind()
        {
            var value = -1f;

            var motion = LMotion.Create(0f, 1f, 1f)
                .WithEase(inQuad)
                .Bind(x => value = x);

            Assert.That(value, Is.EqualTo(0f).Within(1e-5f));
            motion.Cancel();
        }

        [Test]
        public void Test_CustomEase_ManualDispatcher()
        {
            var dispatcher = new ManualMotionDispatcher();
            var value = -1f;

            LMotion.Create(0f, 10f, 1f)
                .WithEase(outCubic)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(x => value = x);

            dispatcher.Update(0.5);
            Assert.That(value, Is.EqualTo(10f * (1f - 0.5f * 0.5f * 0.5f)).Within(1e-4f));

            dispatcher.Update(0.5);
            Assert.That(value, Is.EqualTo(10f).Within(1e-4f));

            dispatcher.Reset();
        }

        [Test]
        public void Test_CustomEase_Time()
        {
            var dispatcher = new ManualMotionDispatcher();
            var value = -1f;

            var handle = LMotion.Create(0f, 1f, 1f)
                .WithEase(inQuad)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(x => value = x);

            handle.Time = 0.25;
            Assert.That(value, Is.EqualTo(0.0625f).Within(1e-5f));

            handle.Time = 1.0;
            Assert.That(value, Is.EqualTo(1f).Within(1e-5f));

            dispatcher.Reset();
        }

        [Test]
        public void Test_CustomEase_LoopIncremental()
        {
            var dispatcher = new ManualMotionDispatcher();
            var value = -1f;

            LMotion.Create(0f, 1f, 1f)
                .WithEase(inQuad)
                .WithLoops(2, LoopType.Incremental)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(x => value = x);

            dispatcher.Update(1.5);
            Assert.That(value, Is.EqualTo(1.25f).Within(1e-4f));

            dispatcher.Reset();
        }

        [Test]
        public void Test_CustomEase_Complete()
        {
            var dispatcher = new ManualMotionDispatcher();
            var value = -1f;

            var handle = LMotion.Create(0f, 1f, 1f)
                .WithEase(inQuad)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(x => value = x);

            handle.Complete();
            Assert.That(value, Is.EqualTo(1f).Within(1e-5f));

            dispatcher.Reset();
        }

        [Test]
        public void Test_CustomEase_MotionSettings()
        {
            var dispatcher = new ManualMotionDispatcher();
            var value = -1f;

            var settings = LMotion.Create(0f, 1f, 1f)
                .WithEase(inQuad)
                .ToMotionSettings();

            LMotion.Create<float, NoOptions, FloatMotionAdapter>(settings)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(x => value = x);

            dispatcher.Update(0.5);
            Assert.That(value, Is.EqualTo(0.25f).Within(1e-5f));

            dispatcher.Reset();
        }

        [Test]
        public void Test_CustomEase_NotCreated()
        {
            Assert.Throws<ArgumentException>(() =>
                LMotion.Create(0f, 1f, 1f).WithEase(default(FunctionPointer<CustomEaseDelegate>)));

            Assert.Throws<ArgumentException>(() =>
                LMotion.Create(0f, 1f, 1f).WithEase(Ease.CustomFunction));
        }
    }
}
