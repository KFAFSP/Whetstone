using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the Trigger synchronization primitive.")]
    [Category("Threading")]
    [TestOf(typeof(Trigger))]
    public sealed class TriggerTests
    {
        [Test]
        [Description("Dispose faults all remaining waiters.")]
        public void Dispose_FaultsWaiters()
        {
            var trigger = new Trigger();
            var task = trigger.WaitAsync();
            trigger.Dispose();

            Assert.Throws<AggregateException>(() => task.Wait(10));
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("Fire on disposed throws ObjectDisposedException.")]
        public void Fire_Disposed_ThrowsObjectDisposedException()
        {
            var trigger = new Trigger();
            trigger.Dispose();

            Assert.Throws<ObjectDisposedException>(() => trigger.Fire());
        }

        [Test]
        [Description("WaitAsync on disposed returns a faulted task.")]
        public void WaitAsync_Disposed_ReturnsFaultedTask()
        {
            var trigger = new Trigger();
            trigger.Dispose();

            var task = trigger.WaitAsync();
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("WaitAsync waits until the next trigger fire.")]
        public void WaitAsync_WaitsForNextFire()
        {
            using (var trigger = new Trigger())
            {
                var task = trigger.WaitAsync();
                Assert.That(task.Wait(10), Is.False);

                trigger.Fire();
                var task2 = trigger.WaitAsync();
                Assert.That(task.Wait(10), Is.True);
                Assert.That(task2.Wait(10), Is.False);
            }
        }

        [Test]
        [Description("Is an awaitable type.")]
        public void Awaitable()
        {
            using (var trigger = new Trigger())
            {
                var task = Task.Run(async () =>
                {
                    // NOTE: We wait for the task to complete before exiting the using scope.
                    // ReSharper disable once AccessToDisposedClosure
                    await trigger;
                });
                Assert.That(task.Wait(10), Is.False);
                trigger.Fire();
                Assert.That(task.Wait(10), Is.True);
            }
        }
    }
}
