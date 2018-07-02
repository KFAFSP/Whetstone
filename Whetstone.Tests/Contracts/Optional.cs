using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MustUseReturnValue

namespace Whetstone.Contracts
{
    [TestFixture]
    [Description("Testing the static factories of the Optional type.")]
    [Category("Contracts")]
    [Category("Optional")]
    [TestOf(typeof(Optional))]
    public sealed class OptionalTests
    {
        [Test]
        [Description("Present factory returns a present Optional wrapping the value.")]
        public void Present_IsPresent()
        {
            var value = new object();
            var test = Optional.Present(value);

            Assert.That(test.IsPresent, Is.True);
            Assert.That(test.Value, Is.SameAs(value));
        }

        [Test]
        [Description("Absent factory returns an absent Optional.")]
        public void Absent_IsAbsent()
        {
            var test = Optional.Absent<int>();

            Assert.That(test.IsPresent, Is.False);
        }
    }

    [TestFixture]
    [Description("Testing the Optional type itself.")]
    [Category("Contracts")]
    [Category("Optional")]
    [TestOf(typeof(Optional<>))]
    public sealed class OptionalTTests
    {
        [Test]
        [Description("Default constructor initializes an absent Optional.")]
        public void Constructor_Default_IsAbsent()
        {
            var test = new Optional<int>();

            Assert.That(test.IsPresent, Is.False);
        }

        [Test]
        [Description("Init constructor initializes a present Optional wrapping the value.")]
        public void Constructor_Init_IsPresent()
        {
            var value = new object();
            var test = new Optional<object>(value);

            Assert.That(test.IsPresent, Is.True);
            Assert.That(test.Value, Is.SameAs(value));
        }

        [Test]
        [Description("If with null predicate throws ArgumentNullException.")]
        public void If_PredicateIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Optional.Present(0).If(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("If on absent Optional propagates absence without calling the predicate.")]
        public void If_IsAbsent_PropagatesAbsenceWithoutCallingPredicate()
        {
            Assert.That(Optional.Absent<int>().If(X =>
            {
                Assert.Fail("Predicate was called.");
                return true;
            }).IsPresent, Is.False);
        }

        [Test]
        [Description("If on matching present propagates the present Optional.")]
        public void If_PresentMatched_PropagatesPresent()
        {
            Assert.That(Optional.Present(0).If(X => X == 0).Value, Is.Zero);
        }

        [Test]
        [Description("If on mismatched present returns an absent Optional.")]
        public void If_PresentMismatched_ReturnsAbsent()
        {
            Assert.That(Optional.Present(0).If(X => X > 0).IsPresent, Is.False);
        }

        [Test]
        [Description("IfPresent on null action throws ArgumentNullException.")]
        public void IfPresent_ActionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Optional.Present(0).IfPresent(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("IfPresent on absent does not call the action.")]
        public void IfPresent_Absent_DoesNotCallAction()
        {
            Optional.Absent<int>().IfPresent(X => Assert.Fail("Action was called."));
        }

        [Test]
        [Description("IfPresent on present calls the action with the present value.")]
        public void IfPresent_Present_CallsActionWithValue()
        {
            var called = false;
            Optional.Present(0).IfPresent(X =>
            {
                called = true;
                Assert.That(X, Is.Zero);
            });

            Assert.That(called, Is.True);
        }

        [Test]
        [Description("IfPresent on present up-propagates an exception thrown in the action.")]
        public void IfPresent_Present_PropagatesActionException()
        {
            var error = new Exception();
            Assert.That(
                () => Optional.Present(0).IfPresent(X => throw error),
                Throws.Exception.SameAs(error)
            );
        }

        [Test]
        [Description("IfPresent on null function throws ArgumentNullException.")]
        public void IfPresent_FuncIsNull_ThrowsArgumentNullException()
        {
            Assert.That(
                () => Optional.Present(0).IfPresent((Func<int, int>)null),
                Throws.ArgumentNullException
            );
        }

        [Test]
        [Description("IfPresent on absent does not call the function.")]
        public void IfPresent_Absent_DoesNotCallFunc()
        {
            Optional.Absent<int>().IfPresent(X =>
            {
                Assert.Fail("Func was called.");
                return 0;
            });
        }

        [Test]
        [Description("IfPresent on present calls the function on the value and wraps the result.")]
        public void IfPresent_Present_CallsFuncWithValueAndWrapsResult()
        {
            Assert.That(Optional.Present(0).IfPresent(X => X == 0).Value, Is.True);
        }

        [Test]
        [Description("IfPresent on present up-propagates an exception thrown in the function.")]
        public void IfPresent_Present_PropagatesFuncException()
        {
            var error = new Exception();
            Assert.That(() => Optional.Present(0).IfPresent(X =>
            {
                if (X == 0)
                    throw error;

                return false;
            }), Throws.Exception.SameAs(error));
        }

        [Test]
        [Description("OrDefault on absent returns the default.")]
        public void OrDefault_Absent_ReturnsDefault()
        {
            Assert.That(Optional.Absent<int>().OrDefault(1), Is.EqualTo(1));
        }

        [Test]
        [Description("OrDefault on present returns the present value.")]
        public void OrDefault_Present_ReturnsValue()
        {
            Assert.That(Optional.Present(2).OrDefault(1), Is.EqualTo(2));
        }

        [Test]
        [Description("As on absent propagates absent Optional.")]
        public void As_Absent_PropagatesAbsent()
        {
            Assert.That(Optional.Absent<object>().As<string>().IsPresent, Is.False);
        }

        [Test]
        [Description("As on present returns the cast result.")]
        public void As_Present_ReturnsCast()
        {
            Assert.That(Optional.Present(new object()).As<string>().Value, Is.Null);
        }

        [Test]
        [Description("Is on absent propagates absent Optional.")]
        public void Is_Absent_PropagatesAbsent()
        {
            Assert.That(Optional.Absent<object>().Is<string>().IsPresent, Is.False);
        }

        [Test]
        [Description("Is on present that failed the cast returns an absent Optional.")]
        public void Is_PresentButFail_ReturnsAbsent()
        {
            Assert.That(Optional.Present(new object()).Is<string>().IsPresent, Is.False);
        }

        [Test]
        [Description("Is on present that passes the cast returns a present Optional.")]
        public void Is_PresentAndValid_ReturnsCast()
        {
            Assert.That(Optional.Present<object>("asdf").Is<string>().Value, Is.EqualTo("asdf"));
        }

        [Test]
        [Description("GetEnumerator on absent yields empty enumeration.")]
        public void GetEnumerator_Absent_IsEmpty()
        {
            Assert.That(Optional.Absent<int>(), Is.Empty);
        }

        [Test]
        [Description("GetEnumerator on present yields only the value.")]
        public void GetEnumerator_Present_ContainsValue()
        {
            Assert.That(Optional.Present(1), Is.EquivalentTo(new []{1}));
        }

        [TestCaseSource(nameof(OptOptEqualsTestCases))]
        [TestCaseSource(nameof(OptIntEqualsTestCases))]
        [TestCaseSource(nameof(OptOptInequalTestCase))]
        [Description("Equals on optional and any is correct.")]
        public bool Equals_OptAny_Correct(object AOptional, object AOther)
        {
            return AOptional.Equals(AOther);
        }

        [Test]
        [Description("GetHashCode on absent returns zero.")]
        public void GetHashCode_Absent_ReturnsZero()
        {
            Assert.That(Optional.Absent<int>().GetHashCode(), Is.Zero);
        }

        [Test]
        [Description("GetHashCode on present returns the hash of the present value.")]
        public void GetHashCode_Present_ReturnsValueHash()
        {
            Assert.That(Optional.Present("abc").GetHashCode(), Is.EqualTo("abc".GetHashCode()));
        }

        [Test]
        [Description("ToString on absent returns the absence literal.")]
        public void ToString_Absent_ReturnsAbsentLiteral()
        {
            Assert.That(Optional.Absent<int>().ToString(), Is.EqualTo("Optional.Absent<Int32>"));
        }

        [Test]
        [Description("ToString on present returns the ToString result of the value.")]
        public void ToString_Present_ReturnsValueToString()
        {
            Assert.That(Optional.Present(1).ToString(), Is.EqualTo("1"));
        }

        [Test]
        [Description("ThrowIfAbsent on absent throws InvalidOperationException.")]
        public void ThrowIfAbsent_Absent_ThrowsInvalidOperationException()
        {
            Assert.That(
                () => Optional.Absent<int>().ThrowIfAbsent(),
                Throws.InvalidOperationException
            );
        }

        [Test]
        [Description("ThrowIfAbsent on present does nothing.")]
        public void ThrowIfAbsent_Present_DoesNothing()
        {
            Optional.Present(1).ThrowIfAbsent();
        }

        [Test]
        [Description("Getting Value on absent throws InvalidOperationException.")]
        public void GetValue_Absent_ThrowsInvalidOperationException()
        {
            Assert.That(() => Optional.Absent<int>().Value, Throws.InvalidOperationException);
        }

        [Test]
        [Description("Getting Value on present returns the value.")]
        public void GetValue_Present_ReturnsValue()
        {
            Assert.That(Optional.Present(1).Value, Is.EqualTo(1));
        }

        [TestCase(null, Description = "Null values are valid present values.")]
        [TestCase("abc", Description = "Non-null values are valid present values.")]
        [Description("Implicit cast from value type returns a present Optional.")]
        public void ImplicitCast_FromValue_IsPresent(object AValue)
        {
            Optional<object> test = AValue;
            Assert.That(test.IsPresent, Is.True);
        }

        [Test]
        [Description("Implicit cast to value type on absent throws InvalidOperationException.")]
        public void ImplicitCast_ToValueAbsent_ThrowsInvalidOperationException()
        {
            Assert.That(() =>
            {
                int _ = Optional.Absent<int>();
            }, Throws.InvalidOperationException);
        }

        [Test]
        [Description("Implicit cast to value type on present returns present value.")]
        public void ImplicitCast_ToValuePresent_IsValue()
        {
            int I = Optional.Present(0);

            Assert.That(I, Is.Zero);
        }

        [TestCaseSource(nameof(OptOptOrTestCase))]
        [Description("")]
        public Optional<int> BinaryOr_OptOpt_ReturnsPresentOrFirst(
            Optional<int> ALeft,
            Optional<int> ARight
        )
        {
            return ALeft | ARight;
        }

        [Test]
        [Description("Binary or on absent and value returns the provided value.")]
        public void BinaryOr_AbsentValue_ReturnsValue()
        {
            Assert.That(Optional.Absent<int>() | 1, Is.EqualTo(1));
        }

        [Test]
        [Description("Binary or on present and value returns the present value.")]
        public void BinaryOr_PresentValue_ReturnsFirst()
        {
            Assert.That(Optional.Present(0) | 1, Is.EqualTo(0));
        }

        [TestCaseSource(nameof(OptOptEqualsTestCases))]
        [Description("Binary equals operator on optionals is correct.")]
        public bool EqOp_OptOpt_Correct(Optional<int> ALeft, Optional<int> ARight)
        {
            return ALeft == ARight && !(ALeft != ARight);
        }

        [TestCaseSource(nameof(OptIntEqualsTestCases))]
        [Description("Binary equals operator on optional and value is correct.")]
        public bool EqOp_OptVal_Correct(Optional<int> ALeft, int ARight)
        {
            return ALeft == ARight && !(ALeft != ARight);
        }

        [UsedImplicitly]
        public static IEnumerable OptOptEqualsTestCases
        {
            get
            {
                yield return new TestCaseData(Optional.Absent<int>(), Optional.Absent<int>())
                    .Returns(true)
                    .SetDescription("Absent with absent is true.");
                yield return new TestCaseData(Optional.Present(0), Optional.Present(0))
                    .Returns(true)
                    .SetDescription("Present with same-valued present is true.");
                yield return new TestCaseData(Optional.Present(0), Optional.Present(1))
                    .Returns(false)
                    .SetDescription("Present with different-valued present is false.");
            }
        }

        [UsedImplicitly]
        public static IEnumerable OptIntEqualsTestCases
        {
            get
            {
                yield return new TestCaseData(Optional.Absent<int>(), 0)
                    .Returns(false)
                    .SetDescription("Absent with value is false.");
                yield return new TestCaseData(Optional.Present(0), 0)
                    .Returns(true)
                    .SetDescription("Present with same value is true.");
                yield return new TestCaseData(Optional.Present(0), 1)
                    .Returns(false)
                    .SetDescription("Present with different value is false.");
            }
        }

        [UsedImplicitly]
        public static IEnumerable OptOptInequalTestCase
        {
            get
            {
                yield return new TestCaseData(Optional.Absent<int>(), Optional.Absent<string>())
                    .Returns(false)
                    .SetDescription("Absent with different-typed absent is false.");
                yield return new TestCaseData(Optional.Present(0), Optional.Present("asd"))
                    .Returns(false)
                    .SetDescription("Present with different-typed present is false.");
            }
        }

        [UsedImplicitly]
        public static IEnumerable OptOptOrTestCase
        {
            get
            {
                yield return new TestCaseData(Optional.Present(0), Optional.Present(1))
                    .Returns(Optional.Present(0))
                    .SetDescription("Present with present returns first.");
                yield return new TestCaseData(Optional.Present(0), Optional.Absent<int>())
                    .Returns(Optional.Present(0))
                    .SetDescription("Present with absent returns first.");
                yield return new TestCaseData(Optional.Absent<int>(), Optional.Present(1))
                    .Returns(Optional.Present(1))
                    .SetDescription("Absent with present returns second.");
                yield return new TestCaseData(Optional.Absent<int>(), Optional.Absent<int>())
                    .Returns(Optional.Absent<int>())
                    .SetDescription("Absent with absent returns absent.");
            }
        }
    }

    [TestFixture]
    [Description("Testing the static extension methods for the Optional type.")]
    [Category("Contracts")]
    [Category("Optional")]
    [TestOf(typeof(OptionalExtensions))]
    public sealed class OptionalExtensionsTests
    {
        [Test]
        [Description("IfNotNull on null returns absent Optional.")]
        public void IfNotNull_Null_ReturnsAbsent()
        {
            Assert.That(((string)null).IfNotNull().IsPresent, Is.False);
        }

        [Test]
        [Description("IfNotNull on not null wraps value.")]
        public void IfNotNull_NotNull_WrapsValue()
        {
            Assert.That("abc".IfNotNull().Value, Is.EqualTo("abc"));
        }

        [Test]
        [Description("IfAny on null throws ArgumentNullException.")]
        public void IfAny_EnumerableIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => ((IEnumerable<int>)null).IfAny(), Throws.ArgumentNullException);
        }

        [Test]
        [Description("IfAny on empty Enumerable returns absent Optional.")]
        public void IfAny_EnumerableIsEmpty_ReturnsAbsent()
        {
            Assert.That(new int[0].IfAny().IsPresent, Is.False);
        }

        [Test]
        [Description("IfAny on non empty Enumerable wraps first value.")]
        public void IfAny_HasValues_ReturnsFirst()
        {
            Assert.That(new[] { 1, 2, 3 }.IfAny().Value, Is.EqualTo(1));
        }

        [Test]
        [Description("Predicated IfAny on null predicate throws ArgumentNullException.")]
        public void IfAny2_PredicateIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => new[] { 1, 2, 3 }.IfAny(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("Predicated IfAny on empty Enumerable returns absent Optional.")]
        public void IfAny2_HasNoMatchingValue_ReturnsAbsent()
        {
            Assert.That(() => new[] { 1, 2, 3 }.IfAny(X => X < 0).IsPresent, Is.False);
        }

        [Test]
        [Description("Predicated IfAny on Enumerable with match wraps first matching value.")]
        public void IfAny2_HasMatchingValue_ReturnsFirstMatching()
        {
            Assert.That(() => new[] { 1, 2, 3 }.IfAny(X => X > 2).Value, Is.EqualTo(3));
        }

        [Test]
        [Description("IfSucceeds on null function throws ArgumentNullException.")]
        public void IfSucceeds_FuncIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => ((Func<int>)null).IfSucceeds(), Throws.ArgumentNullException);
        }

        [Test]
        [Description("IfSucceeds on failing function returns absent Optional.")]
        public void IfSucceeds_Fails_ReturnsAbsent()
        {
            Func<int> test = () => throw new Exception();
            Assert.That(test.IfSucceeds().IsPresent, Is.False);
        }

        [Test]
        [Description("IfSucceeds on successful function wraps result value.")]
        public void IfSucceeds_Succeeds_WrapsResult()
        {
            Assert.That(((Func<int>)(() => 5)).IfSucceeds().Value, Is.EqualTo(5));
        }

        [Test]
        [Description("IfIs on mismatching returns absent Optional.")]
        public void IfIs_Not_ReturnsAbsent()
        {
            Assert.That("abc".IfIs<Optional<int>>().IsPresent, Is.False);
        }

        [Test]
        [Description("IfIs on matching wraps cast result.")]
        public void IfIs_Is_ReturnsCast()
        {
            Assert.That("abc".IfIs<string>().Value, Is.TypeOf<string>().And.EqualTo("abc"));
        }

        [Test]
        [Description("If on predicate is null throws ArgumentNullException.")]
        public void If_PredicateIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => 0.If(null), Throws.ArgumentNullException);
        }

        [Test]
        [Description("If on matched wraps the value.")]
        public void If_Matched_WrapsValue()
        {
            Assert.That(0.If(X => X == 0).Value, Is.Zero);
        }

        [Test]
        [Description("If on mismatched returns absent Optional.")]
        public void If_Mismatched_ReturnsAbsent()
        {
            Assert.That(0.If(X => X > 0).IsPresent, Is.False);
        }
    }
}
