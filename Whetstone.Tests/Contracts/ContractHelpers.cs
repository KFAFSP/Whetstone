using System;

using NUnit.Framework;

// ReSharper disable NotResolvedInText

namespace Whetstone.Contracts
{
    [TestFixture]
    [Description("Testing the static ContractHelpers extensions.")]
    [Category("Contracts")]
    [TestOf(typeof(ContractHelpers))]
    public class ContractHelpersTests
    {
        [Test]
        [Description("IsConstrainedBy of null to reference returns true.")]
        public void IsConstrainedBy_NullToRef_ReturnsTrue()
        {
            Assert.That(((object)null).IsConstrainedBy<IDisposable>(), Is.True);
        }

        [Test]
        [Description("IsConstrainedBy of null to value returns false.")]
        public void IsConstrainedBy_NullToValue_ReturnsFalse()
        {
            Assert.That(((object)null).IsConstrainedBy<int>(), Is.False);
        }

        [Test]
        [Description("IsConstrainedBy of any to any type is correct.")]
        public void IsConstrainedBy_AnyToType_Correct()
        {
            object anon = new object();
            object boxed = 1;

            Assert.That(anon.IsConstrainedBy<object>(), Is.True);
            Assert.That(anon.IsConstrainedBy<IDisposable>(), Is.False);
            Assert.That(anon.IsConstrainedBy<int>(), Is.False);

            Assert.That(boxed.IsConstrainedBy<object>(), Is.True);
            Assert.That(boxed.IsConstrainedBy<int>(), Is.True);
            Assert.That(boxed.IsConstrainedBy<int?>(), Is.True);
        }

        [Test]
        [Description("ThrowIfTypeMismatched on mismatched throws ArgumentException.")]
        public void ThrowIfTypeMismatched_Mismatch_ThrowsArgumentException()
        {
            var anon = new object();
            Assert.That(
                () => anon.ThrowIfTypeMismatched<int>("paramName"),
                Throws.ArgumentException.With.Property("ParamName").EqualTo("paramName")
            );
        }

        [Test]
        [Description("ThrowIfTypeMismatched on matched does nothing.")]
        public void ThrowIfTypeMismatched_Match_DoesNothing()
        {
            1.ThrowIfTypeMismatched<int>("paramName");
        }
    }
}
