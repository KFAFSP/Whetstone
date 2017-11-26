using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable MustUseReturnValue

namespace Whetstone.Contracts
{
    [TestOf(typeof(Optional))]
    public sealed class OptionalTests
    {
        [Test]
        public void Present_IsPresent()
        {
            var optTest = Optional.Present(0);

            Assert.That(optTest.IsPresent, Is.True);
        }

        [Test]
        public void Absent_IsAbsent()
        {
            var optTest = Optional.Absent<int>();

            Assert.That(optTest.IsPresent, Is.False);
        }
    }

    [TestOf(typeof(Optional<>))]
    public sealed class OptionalTTests
    {
        [Test]
        public void Constructor_Default_IsAbsent()
        {
            var optTest = new Optional<int>();

            Assert.That(optTest.IsPresent, Is.False);
        }

        [Test]
        public void Constructor_Init_IsPresent()
        {
            var optTest = new Optional<int>(0);

            Assert.That(optTest.IsPresent, Is.True);
        }

        [Test]
        public void If_PredicateIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Optional.Present(0).If(null), Throws.ArgumentNullException);
        }

        [Test]
        public void If_IsAbsent_PropagatesAbsenceWithoutCallingPredicate()
        {
            Assert.That(Optional.Absent<int>().If(X =>
            {
                Assert.Fail("Predicate was called.");
                return true;
            }).IsPresent, Is.False);
        }

        [Test]
        public void If_PresentMatched_PropagatesPresent()
        {
            Assert.That(Optional.Present(0).If(X => X == 0).Value, Is.Zero);
        }

        [Test]
        public void If_PresentMismatched_ReturnsAbsent()
        {
            Assert.That(Optional.Present(0).If(X => X > 0).IsPresent, Is.False);
        }

        [Test]
        public void IfPresent_ActionIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Optional.Present(0).IfPresent((Action<int>)null), Throws.ArgumentNullException);
        }

        [Test]
        public void IfPresent_Absent_DoesNotCallAction()
        {
            Optional.Absent<int>().IfPresent(X => Assert.Fail("Action was called."));
        }

        [Test]
        public void IfPresent_Present_CallsActionWithValue()
        {
            var bCalled = false;
            Optional.Present(0).IfPresent(X =>
            {
                bCalled = true;
                Assert.That(X, Is.Zero);
            });

            Assert.That(bCalled, Is.True);
        }

        [Test]
        public void IfPresent_Present_PropagatesActionException()
        {
            var eException = new Exception();
            Assert.That(() => Optional.Present(0).IfPresent(X => throw eException), Throws.Exception.SameAs(eException));
        }

        [Test]
        public void IfPresent_FuncIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => Optional.Present(0).IfPresent((Func<int, int>)null), Throws.ArgumentNullException);
        }

        [Test]
        public void IfPresent_Absent_DoesNotCallFunc()
        {
            Optional.Absent<int>().IfPresent(X =>
            {
                Assert.Fail("Func was called.");
                return 0;
            });
        }

        [Test]
        public void IfPresent_Present_CallsFuncWithValueAndWrapsResult()
        {
            Assert.That(Optional.Present(0).IfPresent(X => X == 0).Value, Is.True);
        }

        [Test]
        public void IfPresent_Present_PropagatesFuncException()
        {
            var eException = new Exception();
            Assert.That(() => Optional.Present(0).IfPresent(X =>
            {
                if (X == 0)
                    throw eException;

                return false;
            }), Throws.Exception.SameAs(eException));
        }

        [Test]
        public void OrDefault_Absent_ReturnsDefault()
        {
            Assert.That(Optional.Absent<int>().OrDefault(1), Is.EqualTo(1));
        }

        [Test]
        public void OrDefault_Present_ReturnsValue()
        {
            Assert.That(Optional.Present(2).OrDefault(1), Is.EqualTo(2));
        }

        [Test]
        public void As_Absent_PropagatesAbsent()
        {
            Assert.That(Optional.Absent<object>().As<string>().IsPresent, Is.False);
        }

        [Test]
        public void As_Present_ReturnsCast()
        {
            Assert.That(Optional.Present(new object()).As<string>().Value, Is.Null);
        }

        [Test]
        public void Is_Absent_PropagatesAbsent()
        {
            Assert.That(Optional.Absent<object>().Is<string>().IsPresent, Is.False);
        }

        [Test]
        public void Is_PresentButFail_ReturnsAbsent()
        {
            Assert.That(Optional.Present(new object()).Is<string>().IsPresent, Is.False);
        }

        [Test]
        public void Is_PresentAndValid_ReturnsCast()
        {
            Assert.That(Optional.Present<object>("asdf").Is<string>().Value, Is.EqualTo("asdf"));
        }

        [Test]
        public void GetEnumerator_Absent_IsEmpty()
        {
            Assert.That(Optional.Absent<int>(), Is.Empty);
        }

        [Test]
        public void GetEnumerator_Present_ContainsValue()
        {
            Assert.That(Optional.Present(1), Is.EquivalentTo(new []{1}));
        }

        [TestCaseSource(nameof(EqualsTestCases))]
        public bool Equals_Correct(object AOptional, object AOther)
        {
            return AOptional.Equals(AOther);
        }

        [Test]
        public void GetHashCode_Absent_ReturnsZero()
        {
            Assert.That(Optional.Absent<int>().GetHashCode(), Is.Zero);
        }

        [Test]
        public void GetHashCode_Present_ReturnsValueHash()
        {
            Assert.That(Optional.Present("abc").GetHashCode(), Is.EqualTo("abc".GetHashCode()));
        }

        [Test]
        public void ToString_Absent_ReturnsAbsentLiteral()
        {
            Assert.That(Optional.Absent<int>().ToString(), Is.EqualTo("Optional.Absent<Int32>"));
        }

        [Test]
        public void ToString_Present_ReturnsValueToString()
        {
            Assert.That(Optional.Present(1).ToString(), Is.EqualTo("1"));
        }

        [Test]
        public void Value_Absent_ThrowsInvalidOperationException()
        {
            Assert.That(() => Optional.Absent<int>().Value, Throws.InvalidOperationException);
        }

        [Test]
        public void Value_Present_ReturnsValue()
        {
            Assert.That(Optional.Present(1).Value, Is.EqualTo(1));
        }

        [TestCase(null)]
        [TestCase("abc")]
        public void ImplicitCast_FromValue_IsPresent(object AValue)
        {
            Optional<object> optTest = AValue;
            Assert.That(optTest.IsPresent, Is.True);
        }

        [Test]
        public void ImplicitCast_ToValueAbsent_ThrowsInvalidOperationException()
        {
            Assert.That(() =>
            {
                int I = Optional.Absent<int>();
            }, Throws.InvalidOperationException);
        }

        [Test]
        public void ImplicitCast_ToValuePresent_IsValue()
        {
            int I = Optional.Present(0);

            Assert.That(I, Is.Zero);
        }

        [Test]
        public void BinaryOr_PresentAny_ReturnsFirst()
        {
            Assert.That(Optional.Present(0) | Optional.Present(1), Is.EqualTo(0));
            Assert.That(Optional.Present(0) | Optional.Absent<int>(), Is.EqualTo(0));
        }

        [Test]
        public void BinaryOr_AbsentPresent_ReturnsSecond()
        {
            Assert.That(Optional.Absent<int>() | Optional.Present(1), Is.EqualTo(1));
        }

        [Test]
        public void BinaryOr_AbsentAbsent_ReturnsAbsent()
        {
            Assert.That((Optional.Absent<int>() | Optional.Absent<int>()).IsPresent, Is.False);
        }

        [Test]
        public void BinaryOr_AbsentValue_ReturnsValue()
        {
            Assert.That(Optional.Absent<int>() | 1, Is.EqualTo(1));
        }

        [Test]
        public void BinaryOr_PresentValue_ReturnsFirst()
        {
            Assert.That(Optional.Present(0) | 1, Is.EqualTo(0));
        }

        public static IEnumerable EqualsTestCases
        {
            get
            {
                yield return new TestCaseData(Optional.Absent<int>(), Optional.Absent<int>()).Returns(true);
                yield return new TestCaseData(Optional.Absent<int>(), Optional.Absent<string>()).Returns(false);
                yield return new TestCaseData(Optional.Present(0), Optional.Present(0)).Returns(true);
                yield return new TestCaseData(Optional.Present(0), Optional.Present(1)).Returns(false);

                yield return new TestCaseData(Optional.Absent<int>(), 0).Returns(false);
                yield return new TestCaseData(Optional.Present(0), 0).Returns(true);
                yield return new TestCaseData(Optional.Present(0), 1).Returns(false);
            }
        }
    }

    [TestOf(typeof(OptionalExtensions))]
    public sealed class OptionalExtensionsTests
    {
        [Test]
        public void IfNotNull_Null_IsAbsent()
        {
            Assert.That(((string)null).IfNotNull().IsPresent, Is.False);
        }

        [Test]
        public void IfNotNull_NotNull_WrapsValue()
        {
            Assert.That("abc".IfNotNull().Value, Is.EqualTo("abc"));
        }

        [Test]
        public void IfAny_EnumerableIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => ((IEnumerable<int>)null).IfAny(), Throws.ArgumentNullException);
        }

        [Test]
        public void IfAny_EnumerableIsEmpty_ReturnsAbsent()
        {
            Assert.That(new int[0].IfAny().IsPresent, Is.False);
        }

        [Test]
        public void IfAny_HasValues_ReturnsFirst()
        {
            Assert.That(new[] { 1, 2, 3 }.IfAny().Value, Is.EqualTo(1));
        }

        [Test]
        public void IfSucceeds_FuncIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => ((Func<int>)null).IfSucceeds(), Throws.ArgumentNullException);
        }

        [Test]
        public void IfSucceeds_Fails_ReturnsAbsent()
        {
            Assert.That(((Func<int>)(() => throw new Exception())).IfSucceeds().IsPresent, Is.False);
        }

        [Test]
        public void IfSucceeds_Succeeds_WrapsResult()
        {
            Assert.That(((Func<int>)(() => 5)).IfSucceeds().Value, Is.EqualTo(5));
        }

        [Test]
        public void IfIs_Not_ReturnsAbsent()
        {
            Assert.That("abc".IfIs<Optional<int>>().IsPresent, Is.False);
        }

        [Test]
        public void IfIs_Is_ReturnsCast()
        {
            Assert.That(((object)"abc").IfIs<string>().Value, Is.TypeOf<string>().And.EqualTo("abc"));
        }

        [Test]
        public void If_PredicateIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => 0.If(null), Throws.ArgumentNullException);
        }

        [Test]
        public void If_Matched_WrapsValue()
        {
            Assert.That(0.If(X => X == 0).Value, Is.Zero);
        }

        [Test]
        public void If_Mismatched_ReturnsAbsent()
        {
            Assert.That(0.If(X => X > 0).IsPresent, Is.False);
        }
    }
}
