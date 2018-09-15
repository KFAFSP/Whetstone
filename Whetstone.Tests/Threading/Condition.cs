using System;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the Condition synchronization primitive.")]
    [Category("Threading")]
    [TestOf(typeof(Condition))]
    public sealed class ConditionTests
    {
        [Test]
        [Description("Default constructor initializes a condition with initial value of false.")]
        public void Constructor_Default_IsFalse()
        {
            Assert.That(new Condition().Value, Is.False);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("Initialization constructor initializes a condition with the desired value.")]
        public void Constructor_Init_IsValue(bool AValue)
        {
            Assert.That(new Condition(AValue).Value, Is.EqualTo(AValue));
        }

        [Test]
        [Description("Dispose faults all remaining waiters.")]
        public void Dispose_False_FaultsWaiters()
        {
            var cond = Condition.False();
            Assume.That(cond.Value, Is.False);

            var task = cond.WaitAsync();
            cond.Dispose();

            Assert.Throws<AggregateException>(() => task.Wait(10));
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("False creates a condition that is false.")]
        public void False_IsFalse()
        {
            Assert.That(Condition.False().Value, Is.False);
        }

        [Test]
        [Description("True creates a condition that is true.")]
        public void True_IsTrue()
        {
            using (var cond = Condition.True())
            {
                Assert.That(cond.Value, Is.True);
            }
        }

        [Test]
        [Description("TryReset on disposed throws ObjectDisposedException.")]
        public void TryReset_Disposed_ThrowsObjectDisposedException()
        {
            var cond = new Condition();
            cond.Dispose();

            Assert.Throws<ObjectDisposedException>(() => cond.TryReset());
        }

        [Test]
        [Description("TryReset on false returns false.")]
        public void TryReset_False_ReturnsFalse()
        {
            using (var cond = Condition.False())
            {
                Assume.That(cond.Value, Is.False);

                Assert.That(cond.TryReset(), Is.False);
            }
        }

        [Test]
        [Description("TryReset on true resets to false and returns true.")]
        public void TryReset_True_ResetsAndReturnsTrue()
        {
            using (var cond = Condition.True())
            {
                Assume.That(cond.Value, Is.True);

                Assert.That(cond.TryReset(), Is.True);
                Assert.That(cond.Value, Is.False);
            }
        }

        [Test]
        [Description("TrySet on disposed throws ObjectDisposedException.")]
        public void TrySet_Disposed_ThrowsObjectDisposedException()
        {
            var cond = new Condition();
            cond.Dispose();

            Assert.Throws<ObjectDisposedException>(() => cond.TrySet());
        }

        [Test]
        [Description("TrySet on false sets to true and returns true.")]
        public void TrySet_False_SetsAndReturnsTrue()
        {
            using (var cond = Condition.False())
            {
                Assume.That(cond.Value, Is.False);

                Assert.That(cond.TrySet(), Is.True);
                Assert.That(cond.Value, Is.True);
            }
        }

        [Test]
        [Description("TrySet on true returns false.")]
        public void TrySet_True_ReturnsFalse()
        {
            using (var cond = Condition.True())
            {
                Assume.That(cond.Value, Is.True);

                Assert.That(cond.TrySet(), Is.False);
            }
        }

        [Test]
        [Description("WaitAsync on disposed returns a faulted task.")]
        public void WaitAsync_Disposed_ReturnsFaultedTask()
        {
            var cond = new Condition();
            cond.Dispose();

            var task = cond.WaitAsync();
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("WaitAsync on true returns a completed task.")]
        public void WaitAsync_True_ReturnsCompletedTask()
        {
            using (var cond = Condition.True())
            {
                Assume.That(cond.Value, Is.True);

                var task = cond.WaitAsync();
                Assert.That(task.IsCompleted, Is.True);
            }
        }

        [Test]
        [Description("WaitAsync on false waits for change to true.")]
        public void WaitAsync_False_WaitsForTrue()
        {
            using (var cond = Condition.False())
            {
                Assume.That(cond.Value, Is.False);

                var task = cond.WaitAsync();
                Assert.That(task.Wait(10), Is.False);

                Assume.That(cond.TrySet(), Is.True);
                Assert.That(task.Wait(10), Is.True);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("Setting Value changes the value.")]
        public void SetValue_ChangesValue(bool ATo)
        {
            using (var cond = new Condition(!ATo))
            {
                Assume.That(cond.Value, Is.EqualTo(!ATo));

                cond.Value = ATo;
                Assert.That(cond.Value, Is.EqualTo(ATo));
            }
        }

        [Test]
        [Description("Setting Value on disposed throws ObjectDisposedException.")]
        public void SetValue_Disposed_ThrowsObjectDisposedException()
        {
            var cond = new Condition();
            cond.Dispose();

            Assert.Throws<ObjectDisposedException>(() => cond.Value = true);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Description("Implicit cast to bool returns Value.")]
        public void ImplicitCast_ToBool_ReturnsValue(bool AValue)
        {
            using (var cond = new Condition(AValue))
            {
                Assume.That(cond.Value, Is.EqualTo(AValue));

                bool value = cond;
                Assert.That(value, Is.EqualTo(AValue));
            }
        }
    }
}
