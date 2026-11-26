using System.Collections;
using LitMotion.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LitMotion.Tests.Runtime
{
    public class DelayTest
    {
        [UnityTest]
        public IEnumerator Test_Delay()
        {
            var t = Time.timeAsDouble;
            yield return LMotion.Create(0f, 1f, 0.5f)
                .WithDelay(0.5f)
                .BindToUnityLogger()
                .ToYieldInstruction();
            Assert.That(Time.timeAsDouble - t, Is.GreaterThan(0.95).And.LessThan(1.1));
        }

        [UnityTest]
        public IEnumerator Test_Delay_WithZeroDuration()
        {
            var t = Time.timeAsDouble;
            yield return LMotion.Create(0f, 1f, 0f)
                .WithDelay(1f)
                .BindToUnityLogger()
                .ToYieldInstruction();
            Assert.That(Time.timeAsDouble - t, Is.GreaterThan(0.95).And.LessThan(1.1));
        }

        [UnityTest]
        public IEnumerator Test_Delay_EveryLoop()
        {
            var t = Time.timeAsDouble;
            yield return LMotion.Create(0f, 1f, 0.5f)
                .WithLoops(2)
                .WithDelay(0.5f, DelayType.EveryLoop)
                .BindToUnityLogger()
                .ToYieldInstruction();
            Assert.That(Time.timeAsDouble - t, Is.GreaterThan(1.95).And.LessThan(2.1));
        }

        [UnityTest]
        public IEnumerator Test_Delay_EveryLoop_WithZeroDuration()
        {
            var t = Time.timeAsDouble;
            yield return LMotion.Create(0f, 1f, 0f)
                .WithLoops(3)
                .WithDelay(0.5f, DelayType.EveryLoop)
                .BindToUnityLogger()
                .ToYieldInstruction();
            Assert.That(Time.timeAsDouble - t, Is.GreaterThan(1.45).And.LessThan(1.6));
        }

        [UnityTest]
        public IEnumerator Test_Delay_SkipValuesDuringDelay()
        {
            var value = 0f;
            var handle = LMotion.Create(1f, 2f, 0.5f)
                .WithImmediateBind(false)
                .WithDelay(0.5f, skipValuesDuringDelay: false)
                .Bind(x => value = x);
            yield return new WaitForSeconds(0.1f);
            Assert.That(value, Is.GreaterThan(0.9f));

            handle.Cancel();
            value = 0f;
            handle = LMotion.Create(1f, 2f, 0.5f)
                .WithImmediateBind(false)
                .WithDelay(0.5f, skipValuesDuringDelay: true)
                .Bind(x => value = x);
            yield return new WaitForSeconds(0.1f);
            Assert.That(value, Is.LessThan(0.9f));
            handle.Cancel();
        }
    }
}
