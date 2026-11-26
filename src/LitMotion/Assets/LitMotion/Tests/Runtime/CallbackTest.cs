using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LitMotion.Tests.Runtime
{
    public class CallbackTest
    {
        [UnityTest]
        public IEnumerator Test_OnCancel()
        {
            var canceled = false;
            var handle = LMotion.Create(0f, 10f, 2f)
                .WithOnCancel(() => canceled = true)
                .RunWithoutBinding();
            yield return new WaitForSeconds(0.5f);
            handle.Cancel();
            Assert.IsTrue(canceled);
        }

        [UnityTest]
        public IEnumerator Test_OnComplete()
        {
            var completed = false;
            LMotion.Create(0f, 10f, 2f)
                .WithOnComplete(() => completed = true)
                .RunWithoutBinding();
            yield return new WaitForSeconds(2.1f);
            Assert.IsTrue(completed);
        }

        [UnityTest]
        public IEnumerator Test_OnLoopComplete()
        {
            var loopCount = 0;
            LMotion.Create(0f, 10f, 1f)
                .WithLoops(3)
                .WithOnLoopComplete(x => loopCount = x)
                .RunWithoutBinding();
            yield return new WaitForSeconds(1f);
            Assert.That(loopCount, Is.EqualTo(1));
            yield return new WaitForSeconds(1f);
            Assert.That(loopCount, Is.EqualTo(2));
            yield return new WaitForSeconds(1f);
            Assert.That(loopCount, Is.EqualTo(3));
        }

        [UnityTest]
        public IEnumerator Test_CreateOnCallback()
        {
            var created = false;
            var completed = false;
            LMotion.Create(0f, 10f, 1f)
                .Bind(x =>
                {
                    if (x > 5f && !created)
                    {
                        LMotion.Create(0f, 10f, 1f)
                            .WithOnComplete(() => completed = true)
                            .RunWithoutBinding();
                        created = true;
                    }
                });
            yield return new WaitForSeconds(2.1f);
            Assert.IsTrue(created);
            Assert.IsTrue(completed);
        }

        [Test]
        public void Test_CompleteOnCallback_Self()
        {
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: Motion has already been canceled or completed.");

            MotionHandle handle = default;
            handle = LMotion.Create(0f, 10f, 1f)
                .WithOnComplete(() => handle.Complete())
                .RunWithoutBinding();

            handle.Complete();
        }

        [Test]
        public void Test_CompleteOnCallback_CircularReference()
        {
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: Motion has already been canceled or completed.");

            MotionHandle handle1 = default;
            MotionHandle handle2 = default;
            handle1 = LMotion.Create(0f, 10f, 1f)
                .WithOnComplete(() => handle2.Complete())
                .RunWithoutBinding();
            handle2 = LMotion.Create(0f, 10f, 1f)
                .WithOnComplete(() => handle1.Complete())
                .RunWithoutBinding();

            handle1.Complete();
        }

        [UnityTest]
        public IEnumerator Test_CompleteOnCallback_Other()
        {
            MotionHandle otherHandle = LMotion.Create(0f, 10f, 5f).RunWithoutBinding();
            LMotion.Create(0f, 10f, 0.5f)
                .WithOnComplete(() => otherHandle.Complete())
                .RunWithoutBinding();
            yield return otherHandle.ToYieldInstruction();
        }

        [UnityTest]
        public IEnumerator Test_ThrowExceptionInsideCallback()
        {
            LogAssert.Expect(LogType.Exception, "Exception: Test");
            yield return LMotion.Create(0f, 10f, 0.5f)
                .WithOnComplete(() => throw new Exception("Test"))
                .RunWithoutBinding()
                .ToYieldInstruction();
        }

        [Test]
        public void Test_ThrowExceptionInsideCallback_ThenCompleteManually()
        {
            LogAssert.Expect(LogType.Exception, "Exception: Test");
            var handle = LMotion.Create(0f, 10f, 0.5f)
                .WithOnComplete(() => throw new Exception("Test"))
                .RunWithoutBinding();
            handle.Complete();
        }

        [UnityTest]
        public IEnumerator Test_WithCancelOnError()
        {
            LogAssert.Expect(LogType.Exception, "Exception: Test");
            var completed = false;
            var canceled = false;
            LMotion.Create(0f, 10f, 0.5f)
                .WithImmediateBind(false)
                .WithCancelOnError()
                .WithOnCancel(() => canceled = true)
                .WithOnComplete(() => completed = true)
                .Bind(x => throw new Exception("Test"));
            yield return new WaitForSeconds(0.7f);
            Assert.IsTrue(canceled);
            Assert.IsFalse(completed);
        }

        [UnityTest]
        public IEnumerator Test_RegisterUnhandledExceptionHandler()
        {
            var defaultHandler = MotionDispatcher.GetUnhandledExceptionHandler();
            MotionDispatcher.RegisterUnhandledExceptionHandler(ex => Debug.LogWarning(ex));
            LogAssert.NoUnexpectedReceived();
            yield return LMotion.Create(0f, 10f, 0.5f)
                .WithOnComplete(() => throw new Exception("Test"))
                .RunWithoutBinding()
                .ToYieldInstruction();
            MotionDispatcher.RegisterUnhandledExceptionHandler(defaultHandler);
        }
    }
}
