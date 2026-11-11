using UnityEngine;
using LitMotion.Editor;
using System.Collections;
using UnityEngine.TestTools;
using LitMotion.Adapters;
using NUnit.Framework;

namespace LitMotion.Tests.Editor
{
    public class EditorMotionTest
    {
        [Test]
        public void ShakeNoiseIsContinuousAndSeeded()
        {
            var adapter = new Vector3ShakeMotionAdapter();
            var start = Vector3.zero;
            var strength = Vector3.one;
            var options = ShakeOptions.Default;
            var context = new MotionEvaluationContext { Progress = 0.37f, Time = 1.0 };
            var value = adapter.Evaluate(ref start, ref strength, ref options, in context);

            context.Time = 2.0;
            Assert.AreEqual(value, adapter.Evaluate(ref start, ref strength, ref options, in context));
            context.Progress += 0.00001f;
            Assert.Less((value - adapter.Evaluate(ref start, ref strength, ref options, in context)).magnitude, 0.01f);
            options.RandomSeed++;
            Assert.AreNotEqual(value, adapter.Evaluate(ref start, ref strength, ref options, in context));
            Assert.AreNotEqual(value.x, value.y);
            context.Progress = 1f;
            Assert.AreEqual(start, adapter.Evaluate(ref start, ref strength, ref options, in context));
        }

        [UnityTest]
        public IEnumerator Test_1()
        {
            bool completed = false;
            LMotion.Create(0f, 100f, 1f)
                .WithOnComplete(() => completed = true)
                .Bind(x => Debug.Log(x));

            while (!completed) yield return null;
        }
    }
}
