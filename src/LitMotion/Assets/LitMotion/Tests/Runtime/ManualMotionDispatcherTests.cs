using NUnit.Framework;
using UnityEngine.TestTools.Utils;

namespace LitMotion.Tests.Runtime
{
    public class ManualMotionDispatcherTests
    {
        [Test]
        public void Test_Update()
        {
            var dispatcher = new ManualMotionDispatcher();

            var x = 0f;
            var isCompleted = false;
            LMotion.Create(0f, 10f, 1f)
                .WithScheduler(dispatcher.Scheduler)
                .WithOnComplete(() => isCompleted = true)
                .Bind(v => x = v);

            dispatcher.Update(0.5);
            Assert.That(x, Is.EqualTo(5f).Using(FloatEqualityComparer.Instance));
            dispatcher.Update(0.5);
            Assert.That(x, Is.EqualTo(10f).Using(FloatEqualityComparer.Instance));
            Assert.That(isCompleted, Is.True);
        }

        [Test]
        public void Test_UpdateAfterStorageBecomesEmpty()
        {
            var dispatcher = new ManualMotionDispatcher();
            var handle = LMotion.Create(0f, 1f, 1f)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(_ => { });

            dispatcher.Update(1.0);
            dispatcher.Update(1.0);
            Assert.That(handle.IsActive(), Is.False);
            dispatcher.Update(10.0);

            var value = 0f;
            LMotion.Create(0f, 10f, 1f)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(x => value = x);

            dispatcher.Update(0.5);
            Assert.That(value, Is.EqualTo(5f).Using(FloatEqualityComparer.Instance));
        }

        [Test]
        public void Test_Time()
        {
            var dispatcher = new ManualMotionDispatcher();

            var x = 0f;
            var handle = LMotion.Create(0f, 10f, 1f)
                .WithScheduler(dispatcher.Scheduler)
                .Bind(v => x = v)
                .Preserve();

            dispatcher.Update(1.0);
            Assert.That(x, Is.EqualTo(10f).Using(FloatEqualityComparer.Instance));
            dispatcher.Time = 0;
            Assert.That(x, Is.EqualTo(0f).Using(FloatEqualityComparer.Instance));

            handle.Cancel();
        }
    }
}
