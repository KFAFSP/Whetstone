using System;
using System.Collections;

using JetBrains.Annotations;

using NUnit.Framework;

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

    }
}
