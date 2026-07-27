using System;
using System.Globalization;
using AwesomeAssertions;
using Xunit;

namespace Gehtsoft.Measurements.Test
{
    /// <summary>
    /// Guards the allocation-free conversion, comparison, arithmetic and formatting.
    /// </summary>
    /// <remarks>
    /// These paths stopped allocating when the boxing enumeration comparison was replaced by
    /// the devirtualized equality comparer, and formatting stopped allocating when it was moved
    /// onto a caller-supplied buffer. Both are invisible to the functional tests.
    /// </remarks>
    public class OperationAllocationTest
    {
        private static readonly CultureInfo gInvariant = CultureInfo.InvariantCulture;

        private static double mDoubleSink;
        private static decimal mDecimalSink;
        private static int mIntSink;
        private static bool mBoolSink;

        private const string Because = "the operation must not allocate";

        private static long AllocatedBy(Action action) => AllocationProbe.AllocatedBy(action);

        [Fact]
        public void ConversionDoesNotAllocate()
        {
            var value = new Measurement<DistanceUnit>(10.5, DistanceUnit.Foot);

            AllocatedBy(() => mDoubleSink += value.In(DistanceUnit.Meter)).Should().Be(0, Because);
            AllocatedBy(() => mDoubleSink += value.In(DistanceUnit.Foot)).Should().Be(0, Because);       // same unit
            AllocatedBy(() => mDoubleSink += value.In(DistanceUnit.Inch)).Should().Be(0, Because);       // the base unit
            AllocatedBy(() => mDoubleSink += Measurement<DistanceUnit>.Convert(10.5, DistanceUnit.Foot, DistanceUnit.Meter)).Should().Be(0, Because);

            AllocatedBy(() =>
            {
                Measurement<DistanceUnit> converted = value.To(DistanceUnit.Meter);
                mDoubleSink += converted.Value;
                mIntSink += (int)converted.Unit;
            }).Should().Be(0, Because);
        }

        [Fact]
        public void ComparisonDoesNotAllocate()
        {
            var foot = new Measurement<DistanceUnit>(1, DistanceUnit.Foot);
            var inch = new Measurement<DistanceUnit>(6, DistanceUnit.Inch);
            var negative = new Measurement<DistanceUnit>(-1, DistanceUnit.Foot);

            AllocatedBy(() => mBoolSink = foot == inch).Should().Be(0, Because);
            AllocatedBy(() => mBoolSink = foot != inch).Should().Be(0, Because);
            AllocatedBy(() => mBoolSink = foot > inch).Should().Be(0, Because);
            AllocatedBy(() => mBoolSink = foot <= inch).Should().Be(0, Because);
            AllocatedBy(() => mIntSink += foot.CompareTo(inch)).Should().Be(0, Because);
            AllocatedBy(() => mIntSink += negative.CompareTo(foot)).Should().Be(0, Because);
            AllocatedBy(() => mBoolSink = foot.Equals(inch)).Should().Be(0, Because);
            AllocatedBy(() => mIntSink += foot.GetHashCode()).Should().Be(0, Because);
        }

        [Fact]
        public void ArithmeticDoesNotAllocate()
        {
            var foot = new Measurement<DistanceUnit>(1, DistanceUnit.Foot);
            var inch = new Measurement<DistanceUnit>(6, DistanceUnit.Inch);

            AllocatedBy(() => mDoubleSink += (foot + inch).Value).Should().Be(0, Because);
            AllocatedBy(() => mDoubleSink += (foot - inch).Value).Should().Be(0, Because);
            AllocatedBy(() => mDoubleSink += (foot * 2.0).Value).Should().Be(0, Because);
            AllocatedBy(() => mDoubleSink += (foot / 2.0).Value).Should().Be(0, Because);
            AllocatedBy(() => mDoubleSink += foot / inch).Should().Be(0, Because);
            AllocatedBy(() => mDoubleSink += (-foot).Value).Should().Be(0, Because);
        }

        [Fact]
        public void DecimalOperationsDoNotAllocate()
        {
            var foot = new DecimalMeasurement<DistanceUnit>(1, DistanceUnit.Foot);
            var inch = new DecimalMeasurement<DistanceUnit>(6, DistanceUnit.Inch);

            AllocatedBy(() => mDecimalSink += foot.In(DistanceUnit.Meter)).Should().Be(0, Because);
            AllocatedBy(() => mBoolSink = foot > inch).Should().Be(0, Because);
            AllocatedBy(() => mDecimalSink += (foot + inch).Value).Should().Be(0, Because);
            AllocatedBy(() => mDecimalSink += (foot * 2.0m).Value).Should().Be(0, Because);
        }

        /// <summary>
        /// Formatting into a caller buffer must not allocate at all.
        /// </summary>
        [Fact]
        public void TryFormatDoesNotAllocate()
        {
            var value = new Measurement<DistanceUnit>(-1234.5678, DistanceUnit.Foot);
            var decimalValue = new DecimalMeasurement<DistanceUnit>(-1234.5678m, DistanceUnit.Foot);

            // the buffers are allocated here, before the measurement starts
            char[] chars = new char[128];
            byte[] bytes = new byte[256];

            AllocatedBy(() =>
            {
                value.TryFormat(chars, out int written, "NF".AsSpan(), gInvariant);
                mIntSink += written;
            }).Should().Be(0, Because);

            AllocatedBy(() =>
            {
                value.TryFormat(chars, out int written, "ND".AsSpan(), gInvariant);
                mIntSink += written;
            }).Should().Be(0, Because);

            AllocatedBy(() =>
            {
                value.TryFormat(bytes, out int written, "N2".AsSpan(), gInvariant);
                mIntSink += written;
            }).Should().Be(0, Because);

            AllocatedBy(() =>
            {
                decimalValue.TryFormat(chars, out int written, "ND".AsSpan(), gInvariant);
                mIntSink += written;
            }).Should().Be(0, Because);
        }

        /// <summary>
        /// The default accuracy format is no longer built on every call.
        /// </summary>
        /// <remarks>
        /// `ToString` has to allocate the string it returns, so it cannot be measured against
        /// zero. It can be measured against an equivalent explicit format: the two produce the
        /// same string, so any difference is the format string which `"ND"` used to build.
        /// </remarks>
        [Fact]
        public void DefaultAccuracyFormatIsNotBuiltPerCall()
        {
            // TestUnit.Unit2 declares an accuracy of 2, so "ND" and "N2" produce the same text
            var value = new Measurement<TestUnit>(1.25, TestUnit.Unit2);
            value.ToString("ND", gInvariant).Should().Be(value.ToString("N2", gInvariant));

            long byAccuracy = AllocatedBy(() => mIntSink += value.ToString("ND", gInvariant).Length);
            long byExplicitFormat = AllocatedBy(() => mIntSink += value.ToString("N2", gInvariant).Length);

            byAccuracy.Should().Be(byExplicitFormat, "the accuracy format string must come from the cache, not from a per-call concatenation");
        }
    }
}
