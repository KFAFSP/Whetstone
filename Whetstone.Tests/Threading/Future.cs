using System;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the static Future factories.")]
    [Category("Threading")]
    [Category("Future")]
    [TestOf(typeof(Future))]
    public sealed class FutureTests
    {
        [Test]
        [Description("Of returns an existant future of that value.")]
        public void Of_ReturnsExistantFuture()
        {
            using (var future = Future.Of(1))
            {
                Assert.That(future.Exists, Is.True);
                Assert.That(future.Value, Is.EqualTo(1));
            }
        }
    }

    [TestFixture]
    [Description("Testing the Future synchronization primitive.")]
    [Category("Threading")]
    [Category("Future")]
    [TestOf(typeof(Future<>))]
    public sealed class FutureTTests
    {
        [Test]
        [Description("Default constructor initializes a non-existant future.")]
        public void Constructor_Default_HasNotEnded()
        {
            using (var future = new Future<int>())
            {
                Assert.That(future.Exists, Is.False);
            }
        }

        [Test]
        [Description("Dispose faults all remaining waiters.")]
        public void Dispose_FaultsWaiters()
        {
            var future = new Future<int>();
            var task = future.WaitAsync();
            future.Dispose();

            Assert.Throws<AggregateException>(() => task.Wait(10));
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("TryPost on non-existant provides and returns true.")]
        public void TryPost_NonExistant_ProvidesAndReturnsTrue()
        {
            using (var future = new Future<int>())
            {
                Assume.That(future.Exists, Is.False);

                Assert.That(future.TryPost(0), Is.True);
                Assert.That(future.Exists, Is.True);
            }
        }

        [Test]
        [Description("TryPost on existant returns false.")]
        public void TryPost_Existant_ReturnsFalse()
        {
            using (var future = Future.Of(0))
            {
                Assume.That(future.Exists, Is.True);

                Assert.That(future.TryPost(0), Is.False);
            }
        }

        [Test]
        [Description("WaitAsync on disposed returns a faulted task.")]
        public void WaitAsync_Disposed_ReturnsFaultedTask()
        {
            var future = new Future<int>();
            future.Dispose();

            var task = future.WaitAsync();
            Assert.That(task.IsFaulted, Is.True);
            Assert.That(
                task.Exception?.InnerExceptions[0],
                Is.InstanceOf<ObjectDisposedException>()
            );
        }

        [Test]
        [Description("WaitAsync on existant returns a completed task.")]
        public void WaitAsync_Existant_ReturnsCompletedTask()
        {
            using (var future = Future.Of(0))
            {
                Assume.That(future.Exists, Is.True);

                var task = future.WaitAsync();
                Assert.That(task.IsCompleted, Is.True);
            }
        }

        [Test]
        [Description("WaitAsync on non-existant waits for the value.")]
        public void WaitAsync_NonExistant_WaitsForEnd()
        {
            using (var future = new Future<int>())
            {
                Assume.That(future.Exists, Is.False);

                var task = future.WaitAsync();
                Assert.That(task.Wait(10), Is.False);

                future.TryPost(0);
                Assert.That(task.Wait(10), Is.True);
            }
        }

        [Test]
        [Description("Is an awaitable type.")]
        public void Awaitable()
        {
            var task = Task.Run(async () =>
            {
                using (var future = Future.Of(1))
                {
                    var value = await future;
                    Assert.That(value, Is.EqualTo(1));
                }
            });
            Assert.That(task.Wait(10), Is.True);
        }

        [Test]
        [Description("Getting Value on disposed throws ObjectDisposedException.")]
        public void GetValue_Disposed_ThrowsObjectDisposedException()
        {
            var future = new Future<int>();
            future.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { var _ = future.Value; });
        }

        [Test]
        [Description("Getting Value on existant returns the value.")]
        public void GetValue_Existant_ReturnsValue()
        {
            using (var future = Future.Of(1))
            {
                Assume.That(future.Exists, Is.True);

                Assert.That(future.Value, Is.EqualTo(1));
            }
        }

        [Test]
        [Description("Setting Value on disposed throws ObjectDisposedException.")]
        public void SetValue_Disposed_ThrowsObjectDisposedException()
        {
            var future = new Future<int>();
            future.Dispose();

            Assert.Throws<ObjectDisposedException>(() => { var _ = future.Value; });
        }

        [Test]
        [Description("Setting Value on existant throws InvalidOperationException.")]
        public void SetValue_Existant_ThrowsInvalidOperationException()
        {
            using (var future = Future.Of(1))
            {
                Assume.That(future.Exists, Is.True);

                Assert.Throws<InvalidOperationException>(() => future.Value = 1);
            }
        }

        [Test]
        [Description("Setting Value on non-existant posts the value.")]
        public void SetValue_NonExistant_PostsValue()
        {
            using (var future = new Future<int>())
            {
                Assume.That(future.Exists, Is.False);

                future.Value = 1;
                Assert.That(future.Exists, Is.True);
                Assert.That(future.Value, Is.EqualTo(1));
            }
        }
    }
}
