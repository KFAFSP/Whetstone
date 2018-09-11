using System;

using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MustUseReturnValue

namespace Whetstone.Contracts
{
    [TestFixture]
    [Description("Testing the Disposable base class.")]
    [Category("Contracts")]
    [TestOf(typeof(Disposable))]
    public sealed class DisposableTests
    {
        public sealed class Exemplar : Disposable
        {
            public static int Disposed;
            public static int Finalizer;
            public static int Disposing;

            ~Exemplar()
            {
                ++Finalizer;
            }

            protected override void Dispose(bool ADisposing)
            {
                if (ADisposing) ++Disposing;
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
            Exemplar.Finalizer = 0;
            Exemplar.Disposing = 0;
        }

        [Test]
        [Description("Default constructor initializes non disposed instance.")]
        public void Constructor_Default_IsNotDisposed()
        {
            Assert.That(!FInstance.IsDisposed);
        }

        [Test]
        [Description("Dispose on non disposed disposes the instance.")]
        public void Dispose_NotDisposed_Disposes()
        {
            Assume.That(!FInstance.IsDisposed);
            Assume.That(Exemplar.Disposed, Is.Zero);
            Assume.That(Exemplar.Disposing, Is.Zero);

            FInstance.Dispose();

            Assert.That(FInstance.IsDisposed);
            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
            Assert.That(Exemplar.Disposing, Is.EqualTo(1));
        }

        [Test]
        [Description("Dispose on disposed does nothing.")]
        public void Dispose_Disposed_DoesNothing()
        {
            FInstance.Dispose();
            Assume.That(FInstance.IsDisposed);
            Assume.That(Exemplar.Disposed, Is.EqualTo(1));

            FInstance.Dispose();

            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
        }

        [Test]
        [Description("Finalizer on non disposed disposes the instance.")]
        public void Finalize_NotDisposed_Disposes()
        {
            Assume.That(Exemplar.Disposed, Is.Zero);
            Assume.That(Exemplar.Finalizer, Is.Zero);
            Assume.That(Exemplar.Disposing, Is.Zero);

            var weakRef = new WeakReference<Exemplar>(FInstance);
            FInstance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assume.That(!weakRef.TryGetTarget(out _));

            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
            Assert.That(Exemplar.Finalizer, Is.EqualTo(1));
            Assert.That(Exemplar.Disposing, Is.Zero);
        }

        [Test]
        [Description("Finalizer on disposed does nothing.")]
        public void Finalize_Disposed_DoesNothing()
        {
            var weakRef = new WeakReference<Exemplar>(FInstance);

            FInstance.Dispose();

            Assume.That(FInstance.IsDisposed);
            Assume.That(Exemplar.Disposed, Is.EqualTo(1));
            Assume.That(Exemplar.Disposing, Is.EqualTo(1));
            Assume.That(Exemplar.Finalizer, Is.Zero);

            FInstance = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assume.That(!weakRef.TryGetTarget(out _));

            Assert.That(Exemplar.Disposed, Is.EqualTo(1));
            Assert.That(Exemplar.Finalizer, Is.Zero);
        }
    }
}
