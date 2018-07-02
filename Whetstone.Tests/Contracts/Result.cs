using System;
using System.Collections;

using JetBrains.Annotations;

using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute

namespace Whetstone.Contracts
{
    [TestFixture]
    [Description("Testing the static factories of the Result type.")]
    [Category("Contracts")]
    [Category("Result")]
    [TestOf(typeof(Result))]
    public sealed class ResultTests
    {
        [Test]
        [Description("Ok factory returns a successful Result wrapping the value.")]
        public void Ok_IsSuccess()
        {
            var ok = Result.Ok(1);

            Assert.That(ok.IsSuccess, Is.True);
            Assert.That(ok.Value, Is.EqualTo(1));
        }

        [Test]
        [Description("Fail factory returns an erroneous Result wrapping the error.")]
        public void Fail_IsErroneous()
        {
            var error = new Exception();
            var fail = Result.Fail<int>(error);

            Assert.That(fail.IsSuccess, Is.False);
            Assert.That(fail.Error, Is.SameAs(error));
        }
    }

    [TestFixture]
    [Description("Testing the Result type itself.")]
    [Category("Contracts")]
    [Category("Result")]
    [TestOf(typeof(Result<>))]
    public sealed class ResultTTests
    {
        static readonly Exception _FCommonError = new NotSupportedException();

        [Test]
        [Description("Default constructor initializes an erroneous Result.")]
        public void Constructor_Default_IsErroneous()
        {
            var test = new Result<int>();

            Assert.That(test.IsSuccess, Is.False);
            Assert.That(test.Error, Is.Not.Null);
        }

        [Test]
        [Description("Value init constructor initializes a successful Result.")]
        public void Constructor_Value_IsSuccess()
        {
            var ok = new Result<int>(1);

            Assert.That(ok.IsSuccess, Is.True);
            Assert.That(ok.Value, Is.EqualTo(1));
        }

        [Test]
        [Description("Exception init constructor initializes an erroneous Result.")]
        public void Constructor_Exception_IsErroneous()
        {
            var error = new Exception();
            var fail = new Result<int>(error);

            Assert.That(fail.IsSuccess, Is.False);
            Assert.That(fail.Error, Is.SameAs(error));
        }

        [Test]
        [Description("OnSuccess on null action throws ArgumentNullException.")]
        public void OnSuccess_ActionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Result.Ok(0).OnSuccess(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("OnSuccess on erroneous Result does not call action.")]
        public void OnSuccess_Erroneous_ActionIsNotCalled()
        {
            Result<int>.Uninitialized.OnSuccess(X => { Assert.Fail("Action was called."); });
        }

        [Test]
        [Description("OnSuccess on successful Result calls action with the value.")]
        public void OnSuccess_Successful_CallsActionWithValue()
        {
            var called = false;
            Result.Ok(0).OnSuccess(X =>
            {
                called = true;
                Assert.That(X, Is.Zero);
            });

            Assert.That(called, Is.True);
        }

        [Test]
        [Description("OnSuccess on successful up-propagates an exception thrown in the action.")]
        public void OnSuccess_Successful_PropagatesActionException()
        {
            var error = new Exception();
            Assert.That(
                () => Result.Ok(0).OnSuccess(X => throw error),
                Throws.Exception.SameAs(error)
            );
        }

        [Test]
        [Description("OnError on null action throws ArgumentNullException.")]
        public void OnError_ActionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Result.Ok(0).OnError<Exception>(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("OnError on matching erroneous Result calls action with the error.")]
        public void OnError_MatchingErroneous_ActionIsCalledWithError()
        {
            var called = false;
            Result.Fail<int>(_FCommonError).OnError<NotSupportedException>(X =>
            {
                called = true;
                Assert.That(X, Is.SameAs(_FCommonError));
            });

            Assert.That(called, Is.True);
        }

        [Test]
        [Description("OnError on mismatched erroneous Result does not call action.")]
        public void OnError_MismatchedErroneous_ActionIsNotCalled()
        {
            Result.Ok(0).OnError<InvalidOperationException>(X => Assert.Fail("Action was called."));
        }

        [Test]
        [Description("OnError on erroneous up-propagates an exception thrown in the action.")]
        public void OnError_Erroneous_PropagatesActionException()
        {
            var error = new Exception();
            Assert.That(
                () => Result.Fail<int>(_FCommonError).OnError<Exception>(X => throw error),
                Throws.Exception.SameAs(error)
            );
        }

        [Test]
        [Description("OnError on successful Result does not call action.")]
        public void OnError_Successful_ActionIsNotCalled()
        {
            Result.Ok(0).OnError<Exception>(X => Assert.Fail("Action was called."));
        }

        [Test]
        [Description("OnError on null action throws ArgumentNullException.")]
        public void OnError2_ActionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Result.Ok(0).OnError(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("OnError on erroneous Result calls action with the error.")]
        public void OnError2_Erroneous_ActionIsCalledWithError()
        {
            var called = false;
            Result.Fail<int>(_FCommonError).OnError(X =>
            {
                called = true;
                Assert.That(X, Is.SameAs(_FCommonError));
            });

            Assert.That(called, Is.True);
        }

        [Test]
        [Description("OnError on erroneous up-propagates an exception thrown in the action.")]
        public void OnError2_Erroneous_PropagatesActionException()
        {
            var error = new Exception();
            Assert.That(
                () => Result.Fail<int>(_FCommonError).OnError(X => throw error),
                Throws.Exception.SameAs(error)
            );
        }

        [Test]
        [Description("OnError on successful Result does not call action.")]
        public void OnError2_Successful_ActionIsNotCalled()
        {
            Result.Ok(0).OnError(X => Assert.Fail("Action was called."));
        }

        [Test]
        [Description("AndThen on null function throws ArgumentNullException.")]
        public void AndThen_FunctionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Result.Ok(0).AndThen<int>(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("AndThen on successful wraps the function result.")]
        public void AndThen_Successful_WrapsResult()
        {
            Assert.That(Result.Ok(2).AndThen(X => X * 2).Value, Is.EqualTo(4));
        }

        [Test]
        [Description("AndThen on erroneous propagates the error..")]
        public void AndThen_Erroneous_PropagatesError()
        {
            Assert.That(
                Result.Fail<int>(_FCommonError).AndThen(X => X * 2).Error,
                Is.SameAs(_FCommonError)
            );
        }

        [Test]
        [Description("AndThen on successful up-propagates an exception thrown in the function.")]
        public void AndThen_Successful_PropagatesFunctionException()
        {
            var error = new Exception();
            Assert.That(
                () => Result.Ok(1).AndThen<object>(X => throw error),
                Throws.Exception.SameAs(error)
            );
        }

        [Test]
        [Description("AndThenTry on null function throws ArgumentNullException.")]
        public void AndThenTry_FunctionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Result.Ok(0).AndThenTry<int>(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("AndThenTry on successful wraps the function result.")]
        public void AndThenTry_Successful_WrapsResult()
        {
            Assert.That(Result.Ok(2).AndThenTry(X => X * 2).Value, Is.EqualTo(4));
        }

        [Test]
        [Description("AndThenTry on erroneous propagates the error..")]
        public void AndThenTry_Erroneous_PropagatesError()
        {
            Assert.That(
                Result.Fail<int>(_FCommonError).AndThenTry(X => X * 2).Error,
                Is.SameAs(_FCommonError)
            );
        }

        [Test]
        [Description("AndThenTry on successfulwraps an exception thrown in the function.")]
        public void AndThenTry_Successful_WrapsFunctionException()
        {
            var error = new Exception();
            Assert.That(
                Result.Ok(1).AndThenTry<object>(X => throw error).Error,
                Is.SameAs(error)
            );
        }

        [Test]
        [Description("IgnoreError on successful returns present Optional.")]
        public void IgnoreError_Successful_WrapsValue()
        {
            Assert.That(Result.Ok(1).IgnoreError().Value, Is.EqualTo(1));
        }

        [Test]
        [Description("IgnoreError on successful returns present Optional.")]
        public void IgnoreError_Erroneous_ReturnsAbsent()
        {
            Assert.That(Result<int>.Uninitialized.IgnoreError().IsPresent, Is.False);
        }

        [TestCaseSource(nameof(ResResAlikeTestCases))]
        [TestCaseSource(nameof(ResIntAlikeTestCases))]
        [TestCaseSource(nameof(ResExcTestCases))]
        [TestCaseSource(nameof(ResAnyNotAlikeTestCases))]
        [Description("Equals on Result and any is correct.")]
        public bool Equals_OptAny_Correct(object AOptional, object AOther)
        {
            return AOptional.Equals(AOther);
        }

        [Test]
        [Description("GetHashCode on erroneous returns hash of the error.")]
        public void GetHashCode_Erroneous_ReturnsErrorHash()
        {
            Assert.That(
                Result.Fail<int>(_FCommonError).GetHashCode(),
                Is.EqualTo(_FCommonError.GetHashCode())
            );
        }

        [Test]
        [Description("GetHashCode on successful returns hash of value.")]
        public void GetHashCode_Successful_ReturnsValueHash()
        {
            Assert.That(Result.Ok("abc").GetHashCode(), Is.EqualTo("abc".GetHashCode()));
        }

        [Test]
        [Description("ToString on erroneous returns error literal.")]
        public void ToString_Erroneous_ReturnsErrorLiteral()
        {
            Assert.That(
                Result.Fail<int>(_FCommonError).ToString(),
                Is.EqualTo($"Result.Fail<Int32>({_FCommonError.GetType().Name})")
            );
        }

        [Test]
        [Description("ToString on successful returns ToString of value.")]
        public void ToString_Successful_ReturnsValueToString()
        {
            Assert.That(Result.Ok(1).ToString(), Is.EqualTo(1.ToString()));
        }

        [Test]
        [Description("ThrowIfError on erroneous throws the contained error.")]
        public void ThrowIfError_Erroneous_ThrowsContainedError()
        {
            Assert.That(
                () => Result.Fail<int>(_FCommonError).ThrowIfError(),
                Throws.Exception.SameAs(_FCommonError)
            );
        }

        [Test]
        [Description("ThrowIfError on successful does nothing.")]
        public void ThrowIfError_Successful_DoesNothing()
        {
            Result.Ok(1).ThrowIfError();
        }

        [Test]
        [Description("Getting value on erroneous throws the contained error.")]
        public void GetValue_Erroneous_ThrowsContainedError()
        {
            Assert.That(
                () => Result.Fail<int>(_FCommonError).Value,
                Throws.Exception.SameAs(_FCommonError)
            );
        }

        [Test]
        [Description("Getting value on successful returns the contained value.")]
        public void GetValue_Successful_ReturnsValue()
        {
            Assert.That(Result.Ok(1).Value, Is.EqualTo(1));
        }

        [Test]
        [Description("Getting error on successful returns null.")]
        public void GetError_Successful_ReturnsNull()
        {
            Assert.That(Result.Ok(1).Error, Is.Null);
        }

        [TestCase(null, Description = "Null values are valid success values.")]
        [TestCase("abc", Description = "Non-null values are valid success values.")]
        [Description("Implicit cast from value type returns a successful Result.")]
        public void ImplicitCast_FromValue_IsSuccessful(object AValue)
        {
            Result<object> test = AValue;
            Assert.That(test.IsSuccess, Is.True);
            Assert.That(test.Value == AValue, Is.True);
        }

        [Test]
        [Description("Implicit cast to value type on erroneous throws the contained error.")]
        public void ImplicitCast_ToValueErroneous_ThrowsContainedError()
        {
            Assert.That(() =>
            {
                int _ = Result.Fail<int>(_FCommonError);
            }, Throws.Exception.SameAs(_FCommonError));
        }

        [Test]
        [Description("Implicit cast to value type on successful returns value.")]
        public void ImplicitCast_ToValueSuccessful_IsValue()
        {
            int I = Result.Ok(0);

            Assert.That(I, Is.Zero);
        }

        [Test]
        [Description("Implicit cast from null Exception throws ArgumentNullException.")]
        public void ImplicitCast_FromNullException_ThrowsArgumentNullException()
        {
            Assert.That(() =>
            {
                Result<int> _ = (Exception)null;
            }, Throws.ArgumentNullException);
        }

        [Test]
        [Description("Implicit cast from Exception is erroneous Result.")]
        public void ImplicitCast_FromException_IsErroneous()
        {
            Result<int> result = _FCommonError;

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.SameAs(_FCommonError));
        }

        [Test]
        [Description("Implicit cast to Exception of erroneous is contained error.")]
        public void ImplicitCast_ToExceptionErroneous_IsContainedError()
        {
            Exception error = Result.Fail<int>(_FCommonError);

            Assert.That(error, Is.SameAs(_FCommonError));
        }

        [Test]
        [Description("Implicit cast to Exception of successful is null.")]
        public void ImplicitCast_ToExceptionSuccessful_IsNull()
        {
            Exception error = Result.Ok(0);

            Assert.That(error, Is.Null);
        }

        [TestCaseSource(nameof(ResResAlikeTestCases))]
        [Description("Binary equals operator on Results is correct.")]
        public bool EqOp_OptOpt_Correct(Result<int> ALeft, Result<int> ARight)
        {
            return ALeft == ARight && !(ALeft != ARight);
        }

        [TestCaseSource(nameof(ResIntAlikeTestCases))]
        [Description("Binary equals operator on Result and value is correct.")]
        public bool EqOp_OptVal_Correct(Result<int> ALeft, int ARight)
        {
            return ALeft == ARight && !(ALeft != ARight);
        }

        [TestCaseSource(nameof(ResExcTestCases))]
        [Description("Binary equals operator on Result and Exception is correct.")]
        public bool EqOp_OptVal_Correct(Result<int> ALeft, Exception ARight)
        {
            return ALeft == ARight && !(ALeft != ARight);
        }

        [UsedImplicitly]
        public static IEnumerable ResResAlikeTestCases
        {
            get
            {
                yield return new TestCaseData(Result.Ok(0), Result.Ok(0))
                    .Returns(true)
                    .SetDescription("Success with same-valued success is true.");
                yield return new TestCaseData(Result.Ok(0), Result.Ok(1))
                    .Returns(false)
                    .SetDescription("Success with different-valued success is false.");
                yield return new TestCaseData(Result<int>.Uninitialized, Result<int>.Uninitialized)
                    .Returns(false)
                    .SetDescription("Uninitialized results are unique.");
                yield return new TestCaseData(
                    Result.Fail<int>(_FCommonError),
                    Result.Fail<int>(_FCommonError)
                )
                    .Returns(true)
                    .SetDescription("Erroroneous results with same error object is true.");
                yield return new TestCaseData(
                    Result.Fail<int>(_FCommonError),
                    Result.Fail<int>(new Exception())
                )
                    .Returns(false)
                    .SetDescription("Erroroneous results with different error object is false.");
            }
        }

        [UsedImplicitly]
        public static IEnumerable ResIntAlikeTestCases
        {
            get
            {
                yield return new TestCaseData(Result.Ok(1), 1)
                    .Returns(true)
                    .SetDescription("Success with contained value is true.");
                yield return new TestCaseData(Result.Ok(1), 2)
                    .Returns(false)
                    .SetDescription("Success with different value is false.");
                yield return new TestCaseData(Result<int>.Uninitialized, 0)
                    .Returns(false)
                    .SetDescription("Erroneous with value is false.");
            }
        }

        [UsedImplicitly]
        public static IEnumerable ResExcTestCases
        {
            get
            {
                yield return new TestCaseData(Result.Fail<int>(_FCommonError), _FCommonError)
                    .Returns(true)
                    .SetDescription("Erroneous with same error is true.");
                yield return new TestCaseData(Result.Fail<int>(_FCommonError), new Exception())
                    .Returns(false)
                    .SetDescription("Erroneous with different error is false.");
                yield return new TestCaseData(Result.Ok(0), _FCommonError)
                    .Returns(false)
                    .SetDescription("Success with error is false.");
            }
        }

        [UsedImplicitly]
        public static IEnumerable ResAnyNotAlikeTestCases
        {
            get
            {
                yield return new TestCaseData(Result.Ok(0), Result.Ok(""))
                    .Returns(false)
                    .SetDescription("Success with different-typed success is false.");
                yield return new TestCaseData(Result<int>.Uninitialized, Result<bool>.Uninitialized)
                    .Returns(false)
                    .SetDescription("Uninitialized results are unique.");
                yield return new TestCaseData(
                    Result.Fail<int>(_FCommonError),
                    Result.Fail<bool>(_FCommonError)
                )
                    .Returns(false)
                    .SetDescription("Different-typed erroroneous results is false.");
                yield return new TestCaseData(Result.Ok(1), "")
                    .Returns(false)
                    .SetDescription("Success with different-typed value is false.");
            }
        }
    }

    [TestFixture]
    [Description("Testing the static extension methods for the Result type.")]
    [Category("Contracts")]
    [Category("Result")]
    [TestOf(typeof(ResultExtensionsTests))]
    public sealed class ResultExtensionsTests
    {
        [Test]
        [Description("Try with null function throws ArgumentNullException.")]
        public void Try_FuncIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => ((Func<int>) null).Try(), Throws.ArgumentNullException);
        }

        [Test]
        [Description("Try with successful function call wraps it's result.")]
        public void Try_FuncSucceeds_WrapsResult()
        {
            Func<int> gen = () => 1;
            Assert.That(gen.Try().Value, Is.EqualTo(1));
        }

        [Test]
        [Description("Try with throwing function call wraps it's error.")]
        public void Try_FuncFails_WrapsError()
        {
            var error = new Exception();
            Func<int> gen = () => throw error;
            Assert.That(gen.Try().Error, Is.SameAs(error));
        }
    }
}
