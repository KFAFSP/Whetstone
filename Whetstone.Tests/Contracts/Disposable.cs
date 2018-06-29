using System;

using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MustUseReturnValue

namespace Whetstone.Contracts
{
    [TestFixture]
    [TestOf(typeof(Disposable))]
    public class DisposableTests
    {
        public class Exemplar : Disposable
        {
            public static int Disposed;

            protected override void Dispose(bool ADisposing)
            {
                ++Disposed;
                base.Dispose(ADisposing);
            }
        }

        Exemplar FInstance;

        [SetUp]
        public void Setup()
        {
            FInstance = new Exemplar();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Exemplar.Disposed = 0;
        }

        [Test]
        public void Constructor_Default_IsNotDisposed()
        {
            Assert.That(!FInstance.IsDisposed);
        }

        [Test]
        public void Dispose_NotDisposed_Disposes()
        {
            Assume.That(!FInstance.IsDisposed);
            Assume.That(Exemplar.Disposed, Is.EqualTo(0));

            FInstance.Dispose();

            Assert.That(FInstance.IsDisposed);
            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
        }

        [Test]
        public void Dispose_Disposed_NoOperation()
        {
            FInstance.Dispose();

            Assume.That(FInstance.IsDisposed);
            Assume.That(Exemplar.Disposed, Is.EqualTo(1));

            FInstance.Dispose();

            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
        }

        [Test]
        public void Finalize_NotDisposed_Disposes()
        {
            Assume.That(Exemplar.Disposed, Is.EqualTo(0));

            var weakRef = new WeakReference<Exemplar>(FInstance);
            FInstance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assume.That(!weakRef.TryGetTarget(out _));

            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
        }

        [Test]
        public void Finalize_Disposed_NoOperation()
        {
            var weakRef = new WeakReference<Exemplar>(FInstance);

            FInstance.Dispose();

            Assume.That(FInstance.IsDisposed);
            Assume.That(Exemplar.Disposed, Is.EqualTo(1));

            FInstance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assume.That(!weakRef.TryGetTarget(out _));

            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
        }
    }
}
