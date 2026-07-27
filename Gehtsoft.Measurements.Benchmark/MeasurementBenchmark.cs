using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using Gehtsoft.Measurements;

namespace Gehtsoft.Measurements.Benchmark
{
    /// <summary>
    /// The benchmark behind CLAUDE/BASELINE_PERF.md.
    /// </summary>
    /// <remarks>
    /// The subject is Measurement&lt;DistanceUnit&gt;, whose base unit is the inch, so a
    /// conversion from a foot to a metre goes through the base and a conversion from an inch
    /// does not. Run it with: dotnet run -c Release --project Gehtsoft.Measurements.Benchmark
    /// </remarks>
    [MemoryDiagnoser]
    public class MeasurementBenchmark
    {
        private static readonly CultureInfo gInvariant = CultureInfo.InvariantCulture;

        private readonly Measurement<DistanceUnit> mFoot = new Measurement<DistanceUnit>(10.5, DistanceUnit.Foot);
        private readonly Measurement<DistanceUnit> mMeter = new Measurement<DistanceUnit>(3.2, DistanceUnit.Meter);
        private readonly Measurement<DistanceUnit> mInch = new Measurement<DistanceUnit>(126.0, DistanceUnit.Inch);
        private readonly Measurement<DistanceUnit> mNegative = new Measurement<DistanceUnit>(-10.5, DistanceUnit.Foot);

        private readonly char[] mBuffer = new char[128];

        // ---- conversion -------------------------------------------------------------------

        [Benchmark]
        public double Convert_NonBase_To_NonBase() => mFoot.In(DistanceUnit.Meter);

        [Benchmark]
        public double Convert_SameUnit() => mFoot.In(DistanceUnit.Foot);

        [Benchmark]
        public double Convert_FromBase() => mInch.In(DistanceUnit.Meter);

        [Benchmark]
        public Measurement<DistanceUnit> To_NonBase() => mFoot.To(DistanceUnit.Meter);

        // ---- comparison -------------------------------------------------------------------

        [Benchmark]
        public bool Compare_Equal() => mFoot == mMeter;

        [Benchmark]
        public bool Compare_GreaterThan() => mFoot > mMeter;

        [Benchmark]
        public int CompareTo() => mFoot.CompareTo(mMeter);

        [Benchmark]
        public int Compare_Negative() => mNegative.CompareTo(mFoot);

        // ---- arithmetic -------------------------------------------------------------------

        [Benchmark]
        public Measurement<DistanceUnit> Add() => mFoot + mMeter;

        // ---- parsing ----------------------------------------------------------------------

        [Benchmark]
        public bool TryParse_Success()
        {
            return Measurement<DistanceUnit>.TryParse(gInvariant, "10.5ft", out Measurement<DistanceUnit> _);
        }

        [Benchmark]
        public bool TryParse_Span()
        {
            return Measurement<DistanceUnit>.TryParse(gInvariant, "10.5ft".AsSpan(), out Measurement<DistanceUnit> _);
        }

        [Benchmark]
        public bool TryParse_Exponent()
        {
            return Measurement<DistanceUnit>.TryParse(gInvariant, "1.05e1ft", out Measurement<DistanceUnit> _);
        }

        [Benchmark]
        public bool TryParse_UnknownUnit()
        {
            return Measurement<DistanceUnit>.TryParse(gInvariant, "10.5zz", out Measurement<DistanceUnit> _);
        }

        // ---- formatting -------------------------------------------------------------------

        [Benchmark]
        public bool TryFormat() => mFoot.TryFormat(mBuffer, out int _, "NF".AsSpan(), gInvariant);

        [Benchmark]
        public bool TryFormat_DefaultAccuracy() => mFoot.TryFormat(mBuffer, out int _, "ND".AsSpan(), gInvariant);

        [Benchmark]
        public string ToString_NF() => mFoot.ToString("NF", gInvariant);

        [Benchmark]
        public string ToString_DefaultAccuracy() => mFoot.ToString("ND", gInvariant);

        [Benchmark]
        public string Interpolation() => $"{mFoot}";

        // ---- unit metadata ----------------------------------------------------------------

        [Benchmark]
        public Tuple<DistanceUnit, string>[] GetUnitNames() => Measurement<DistanceUnit>.GetUnitNames();
    }
}
