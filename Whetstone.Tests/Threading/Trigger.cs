using System;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the Trigger synchronization primitive.")]
    [Category("Threading")]
    [TestOf(typeof(Trigger))]
    public sealed class TriggerTests
    {
        Trigger FInstance;

        [SetUp]
        public void Setup()
        {
            FInstance = new Trigger();
        }

        [Test]
        [Description("Dispose faults all remaining waiters.")]
        public void Dispose_FaultsWaiters()
        {
            var task = FInstance.WaitAsync();
            FInstance.Dispose();

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
            FInstance.Dispose();

            Assert.Throws<ObjectDisposedException>(() => FInstance.Fire());
        }

        [Test]
        [Description("WaitAsync on disposed returns a faulted task.")]
        public void WaitAsync_Disposed_ReturnsFaultedTask()
        {
            FInstance.Dispose();

            var task = FInstance.WaitAsync();
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
            var task = FInstance.WaitAsync();
            Assert.That(task.Wait(10), Is.False);

            FInstance.Fire();
            var task2 = FInstance.WaitAsync();
            Assert.That(task.Wait(10), Is.True);
            Assert.That(task2.Wait(10), Is.False);
        }
    }
}
