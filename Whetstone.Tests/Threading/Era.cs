using System;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the Era synchronization primitive.")]
    [Category("Threading")]
    [TestOf(typeof(Era))]
    public sealed class EraTests
    {
        [Test]
        [Description("Default constructor initializes an unended era.")]
        public void Constructor_Default_HasNotEnded()
        {
            using (var era = new Era())
            {
                Assert.That(era.HasEnded, Is.False);
            }
        }

        [Test]
        [Description("Dispose faults all remaining waiters.")]
        public void Dispose_FaultsWaiters()
        {
            var era = new Era();
            var task = era.WaitAsync();
            era.Dispose();

            Assert.Throws<AggregateException>(() => task.Wait(10));
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("Dispose on unended era ends the era.")]
        public void Dispose_NotEnded_EndsEra()
        {
            var era = new Era();
            Assume.That(era.HasEnded, Is.False);

            era.Dispose();
            Assert.That(era.HasEnded, Is.True);
        }

        [Test]
        [Description("Ended creates an ended era.")]
        public void Ended_HasEnded()
        {
            using (var era = Era.Ended())
            {
                Assert.That(era.HasEnded, Is.True);
            }
        }

        [Test]
        [Description("TryEnd on unended ends and returns true.")]
        public void TryEnd_NotEnded_EndsAndReturnsTrue()
        {
            using (var era = new Era())
            {
                Assume.That(era.HasEnded, Is.False);

                Assert.That(era.TryEnd(), Is.True);
                Assert.That(era.HasEnded, Is.True);
            }
        }

        [Test]
        [Description("TryEnd on ended returns false.")]
        public void TryEnd_Ended_ReturnsFalse()
        {
            using (var era = Era.Ended())
            {
                Assume.That(era.HasEnded, Is.True);

                Assert.That(era.TryEnd(), Is.False);
            }
        }

        [Test]
        [Description("WaitAsync on disposed returns a faulted task.")]
        public void WaitAsync_Disposed_ReturnsFaultedTask()
        {
            var era = new Era();
            era.Dispose();

            var task = era.WaitAsync();
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("WaitAsync on ended returns a completed task.")]
        public void WaitAsync_Ended_ReturnsCompletedTask()
        {
            using (var era = Era.Ended())
            {
                Assume.That(era.HasEnded, Is.True);

                var task = era.WaitAsync();
                Assert.That(task.IsCompleted, Is.True);
            }
        }

        [Test]
        [Description("WaitAsync on unended waits for the end.")]
        public void WaitAsync_NotEnded_WaitsForEnd()
        {
            using (var era = new Era())
            {
                Assume.That(era.HasEnded, Is.False);

                var task = era.WaitAsync();
                Assert.That(task.Wait(10), Is.False);

                era.TryEnd();
                Assert.That(task.Wait(10), Is.True);
            }
        }
    }
}
