using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the Event synchronization primitive.")]
    [Category("Threading")]
    [TestOf(typeof(Event<>))]
    public sealed class EventTTests
    {
        [Test]
        [Description("Dispose faults all remaining waiters.")]
        public void Dispose_FaultsWaiters()
        {
            var evt = new Event<int>();
            var task = evt.WaitAsync();
            evt.Dispose();

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
            var evt = new Event<int>();
            evt.Dispose();

            Assert.Throws<ObjectDisposedException>(() => evt.Fire(1));
        }

        [Test]
        [Description("WaitAsync on disposed returns a faulted task.")]
        public void WaitAsync_Disposed_ReturnsFaultedTask()
        {
            var evt = new Event<int>();
            evt.Dispose();

            var task = evt.WaitAsync();
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("WaitAsync waits until the next event fire.")]
        public void WaitAsync_WaitsForNextFire()
        {
            using (var evt = new Event<int>())
            {
                var task = evt.WaitAsync();
                Assert.That(task.Wait(10), Is.False);

                evt.Fire(1);
                var task2 = evt.WaitAsync();
                Assert.That(task.Wait(10), Is.True);
                Assert.That(task.Result, Is.EqualTo(1));
                Assert.That(task2.Wait(10), Is.False);
            }
        }
    }
}
