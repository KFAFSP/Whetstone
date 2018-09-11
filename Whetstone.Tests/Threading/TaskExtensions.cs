using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Whetstone.Threading
{
    [TestFixture]
    [Description("Testing the static TaskExtensions extensions.")]
    [Category("Threading")]
    [TestOf(typeof(TaskExtensions))]
    public sealed class TaskExtensionsTests
    {
        static readonly CancellationTokenSource _FCanceled = new CancellationTokenSource();
        static readonly Exception _FFaulted = new Exception();

        static TaskExtensionsTests()
        {
            _FCanceled.Cancel();
        }

        static Task MakeCompletedTask(TaskStatus AStatus)
        {
            switch (AStatus)
            {
                case TaskStatus.RanToCompletion:
                    return Task.CompletedTask;

                case TaskStatus.Canceled:
                    return Task.FromCanceled(_FCanceled.Token);

                case TaskStatus.Faulted:
                    return Task.FromException(_FFaulted);

                default:
                    throw new NotImplementedException();
            }
        }
        static Task<TResult> MakeCompletedTask<TResult>(TaskStatus AStatus)
        {
            switch (AStatus)
            {
                case TaskStatus.RanToCompletion:
                    return Task.FromResult<TResult>(default);

                case TaskStatus.Canceled:
                    return Task.FromCanceled<TResult>(_FCanceled.Token);

                case TaskStatus.Faulted:
                    return Task.FromException<TResult>(_FFaulted);

                default:
                    throw new NotImplementedException();
            }
        }

        static async Task MakeUnendingTask()
        {
            while (true)
            {
                await Task.Yield();
            }
        }
        static async Task<TResult> MakeUnendingTask<TResult>()
        {
            while (true)
            {
                await Task.Yield();
            }

            // ReSharper disable once FunctionNeverReturns
        }

        [Test]
        [Description("OrCanceledBy can be canceled with the provided token.")]
        public void OrCanceledBy_Cancelable_CanBeCanceled()
        {
            using (var cts = new CancellationTokenSource())
            {
                var task = MakeUnendingTask();

                var cont = task.OrCanceledBy(cts.Token);
                cts.Cancel();

                Assert.Throws<AggregateException>(() => cont.Wait(10));
                Assert.That(cont.IsCanceled, Is.True);
            }
        }

        [Test]
        [Description("OrCanceledBy with a canceled token returns a canceled task.")]
        public void OrCanceledBy_CanceledToken_ReturnsCanceled()
        {
            var task = MakeUnendingTask();

            var cont = task.OrCanceledBy(_FCanceled.Token);
            Assert.That(cont.IsCanceled, Is.True);
        }

        [TestCase(TaskStatus.RanToCompletion, Description = "RanToCompletion is propagated.")]
        [TestCase(TaskStatus.Faulted, Description = "Faulted is propagated.")]
        [TestCase(TaskStatus.Canceled, Description = "Cancelation is propagated.")]
        [Description("OrCanceledBy that is not canceled propagates the input task result.")]
        public void OrCanceledBy_NotCanceled_Propagates(TaskStatus AStatus)
        {
            using (var cts = new CancellationTokenSource())
            {
                var tcs = new TaskCompletionSource<Void>();
                var cont = tcs.Task.OrCanceledBy(cts.Token);
                Assume.That(cont.Wait(10), Is.False);

                switch (AStatus)
                {
                    case TaskStatus.RanToCompletion:
                        tcs.TrySetResult(default);
                        Assert.That(cont.Wait(10), Is.True);
                        Assert.That(cont.Status == TaskStatus.RanToCompletion);
                        break;

                    case TaskStatus.Canceled:
                        tcs.TrySetCanceled(_FCanceled.Token);
                        Assert.Throws<AggregateException>(() => cont.Wait(10));
                        Assert.That(cont.IsCanceled, Is.True);
                        break;

                    case TaskStatus.Faulted:
                        tcs.TrySetException(_FFaulted);
                        Assert.Throws<AggregateException>(() => cont.Wait(10));
                        Assert.That(cont.IsFaulted, Is.True);
                        Assert.That(cont.Exception?.InnerExceptions[0], Is.SameAs(_FFaulted));
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        [Test]
        [Description("OrCanceledBy that is not canceled propagates the input task result value.")]
        public void OrCanceledBy_NotCanceled_PropagatesResult()
        {
            using (var cts = new CancellationTokenSource())
            {
                var tcs = new TaskCompletionSource<int>();
                var cont = tcs.Task.OrCanceledBy(cts.Token);
                Assume.That(cont.Wait(10), Is.False);

                tcs.TrySetResult(1);
                Assert.That(cont.Wait(10), Is.True);
                Assert.That(cont.Status == TaskStatus.RanToCompletion);
                Assert.That(cont.Result, Is.EqualTo(1));
            }
        }

        [TestCase(TaskStatus.RanToCompletion, Description = "RanToCompletion is propagated.")]
        [TestCase(TaskStatus.Faulted, Description = "Faulted is propagated.")]
        [TestCase(TaskStatus.Canceled, Description = "Cancelation is propagated.")]
        [Description("OrCanceledBy on a completed task propagates the input task.")]
        public void OrCanceledBy_Completed_ReturnsTask(TaskStatus AStatus)
        {
            using (var cts = new CancellationTokenSource())
            {
                var task = MakeCompletedTask(AStatus);
                Assume.That(task.Status == AStatus);

                var cont = task.OrCanceledBy(cts.Token);
                Assert.That(cont, Is.SameAs(task));
            }
        }

        [Test]
        [Description("OrCanceledBy with an uncancelable token propagates the input task.")]
        public void OrCanceledBy_Uncancelable_ReturnsTask()
        {
            var task = MakeUnendingTask();

            var cont = task.OrCanceledBy(CancellationToken.None);
            Assert.That(cont, Is.SameAs(task));
        }
    }
}
