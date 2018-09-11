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
        Era FInstance;

        [SetUp]
        public void Setup()
        {
            FInstance = new Era();
        }

        [Test]
        [Description("Default constructor initializes an unended era.")]
        public void Constructor_Default_HasNotEnded()
        {
            Assert.That(FInstance.HasEnded, Is.False);
        }

        [Test]
        [Description("Dispose on unended era ends the era.")]
        public void Dispose_NotEnded_EndsEra()
        {
            Assume.That(FInstance.HasEnded, Is.False);

            FInstance.Dispose();
            Assert.That(FInstance.HasEnded, Is.True);
        }

        [Test]
        [Description("Ended returns an ended era.")]
        public void Ended_HasEnded()
        {
            Assert.That(Era.Ended.HasEnded, Is.True);
        }

        [Test]
        [Description("TryEnd on unended ends and returns true.")]
        public void TryEnd_NotEnded_EndsAndReturnsTrue()
        {
            Assume.That(FInstance.HasEnded, Is.False);

            Assert.That(FInstance.TryEnd(), Is.True);
            Assert.That(FInstance.HasEnded, Is.True);
        }

        [Test]
        [Description("TryEnd on ended returns false.")]
        public void TryEnd_Ended_ReturnsFalse()
        {
            FInstance.TryEnd();
            Assume.That(FInstance.HasEnded, Is.True);

            Assert.That(FInstance.TryEnd(), Is.False);
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
        [Description("WaitAsync on ended returns a completed task.")]
        public void WaitAsync_Ended_ReturnsCompletedTask()
        {
            FInstance.TryEnd();
            Assume.That(FInstance.HasEnded, Is.True);

            var task = FInstance.WaitAsync();
            Assert.That(task.IsCompleted, Is.True);
        }

        [Test]
        [Description("WaitAsync on unended waits for the end.")]
        public void WaitAsync_NotEnded_WaitsForEnd()
        {
            Assume.That(FInstance.HasEnded, Is.False);

            var task = FInstance.WaitAsync();
            Assert.That(task.Wait(10), Is.False);

            FInstance.TryEnd();
            Assert.That(task.Wait(10), Is.True);
        }
    }
}
