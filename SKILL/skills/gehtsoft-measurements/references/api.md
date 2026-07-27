# The complete public API

Every public type and member of the library, with signatures, generated from the assembly. Use
this so you never have to inspect the package, decompile the assembly, or guess at a signature.

Notes for reading it:

- Members marked `// OBSOLETE` still work but should not be used in new code; each has a
  correctly named replacement in the same type.
- `implicit operator` and `explicit operator` lines show the target type as the return type,
  which is how conversions are declared. `Measurement<T>` and `DecimalMeasurement<T>` convert to
  each other implicitly, and both convert to and from tuples.
- `T` is always the unit enumeration and is constrained to `Enum`. It also carries
  `[DynamicallyAccessedMembers(PublicFields)]`, which is what keeps the unit fields alive under
  trimming; if you write your own generic method that passes `T` on to `Measurement<T>`, copy that
  attribute or the trim analyzer will complain.
- The unit enumerations themselves are not listed here — see `units.md`, which has every unit with
  its names, accuracy and base unit.
- For prose, worked examples and the behaviours that are easy to get wrong, start from `SKILL.md`.
  This file is a lookup table, not a tutorial.

## ConversionAttribute  (class)

```csharp
ctor(ConversionOperation operation)
ctor(ConversionOperation operation, string name)
ctor(ConversionOperation operation, double factor)
ctor(ConversionOperation operation, double factor, ConversionOperation secondOperation, double secondFactor)
double Factor { get; set; }
ConversionOperation Operation { get; set; }
double SecondFactor { get; set; }
ConversionOperation SecondOperation { get; set; }
```

## DecimalMeasurement<T>  (struct)

```csharp
decimal Value
T Unit
ctor(decimal value, T unit)
ctor(Tuple<decimal, T> value)
ctor(ValueTuple<decimal, T> value)
ctor(string text)
static T BaseUnit { get; }
string Text { get; }
static DecimalMeasurement<T> ZERO { get; }
int CompareTo(DecimalMeasurement<T> other)
static decimal Convert(decimal value, T from, T to)
bool Equals(object obj)
bool Equals(DecimalMeasurement<T> other)
static decimal FromBase(decimal value, T unit)
int GetHashCode()
static int GetUnitDefaultAccuracy(T unit)
static string GetUnitName(T unit)
static Tuple<T, string>[] GetUnitNames()
decimal In(T unit)
static DecimalMeasurement<T> operator +(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static DecimalMeasurement<T> operator /(DecimalMeasurement<T> v1, decimal v2)
static decimal operator /(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static bool operator ==(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static DecimalMeasurement<T> explicit operator(Tuple<decimal, T> value)
static bool operator >(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static bool operator >=(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static Tuple<decimal, T> implicit operator(DecimalMeasurement<T> value)
static ValueTuple<decimal, T> implicit operator(DecimalMeasurement<T> value)
static Measurement<T> implicit operator(DecimalMeasurement<T> value)
static bool operator !=(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static bool operator <(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static bool operator <=(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static DecimalMeasurement<T> operator *(DecimalMeasurement<T> v1, decimal v2)
static DecimalMeasurement<T> operator *(decimal v1, DecimalMeasurement<T> v2)
static DecimalMeasurement<T> operator -(DecimalMeasurement<T> v1, DecimalMeasurement<T> v2)
static DecimalMeasurement<T> operator -(DecimalMeasurement<T> v1)
static DecimalMeasurement<T> operator +(DecimalMeasurement<T> v1)
static DecimalMeasurement<T> Parse(string text)
static DecimalMeasurement<T> Parse(ReadOnlySpan<char> text)
static DecimalMeasurement<T> Parse(string text, IFormatProvider provider)
static DecimalMeasurement<T> Parse(ReadOnlySpan<char> text, IFormatProvider provider)
static T ParseUnitName(string name)
DecimalMeasurement<T> To(T unit)
static decimal ToBase(decimal value, T unit)
string ToString()
string ToString(IFormatProvider cultureInfo)
string ToString(string format)
string ToString(string format, IFormatProvider formatProvider)
bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
static bool TryParse(string text, out DecimalMeasurement<T> value)
static bool TryParse(ReadOnlySpan<char> text, out DecimalMeasurement<T> value)
static bool TryParse(CultureInfo cultureInfo, string text, out DecimalMeasurement<T> value)
static bool TryParse(CultureInfo cultureInfo, ReadOnlySpan<char> text, out DecimalMeasurement<T> value)
static bool TryParse(string text, IFormatProvider provider, out DecimalMeasurement<T> value)
static bool TryParse(ReadOnlySpan<char> text, IFormatProvider provider, out DecimalMeasurement<T> value)
```

## ICustomConversionOperation  (interface)

```csharp
double FromBase(double value)
double ToBase(double value)
```

## ICustomConversionOperation2  (interface)

```csharp
decimal FromBaseDecimal(decimal value)
decimal ToBaseDecimal(decimal value)
```

## Measurement<T>  (struct)

```csharp
double Value
T Unit
ctor(double value, T unit)
ctor(Tuple<double, T> value)
ctor(ValueTuple<double, T> value)
ctor(string text)
static T BaseUnit { get; }
string Text { get; }
static Measurement<T> ZERO { get; }
int CompareTo(Measurement<T> other)
static double Convert(double value, T from, T to)
bool Equals(object obj)
bool Equals(Measurement<T> other)
static double FromBase(double value, T unit)
int GetHashCode()
static int GetUnitDefaultAccuracy(T unit)
static string GetUnitName(T unit)
static Tuple<T, string>[] GetUnitNames()
double In(T unit)
static Measurement<T> operator +(Measurement<T> v1, Measurement<T> v2)
static Measurement<T> operator /(Measurement<T> v1, double v2)
static double operator /(Measurement<T> v1, Measurement<T> v2)
static bool operator ==(Measurement<T> v1, Measurement<T> v2)
static Measurement<T> explicit operator(Tuple<double, T> value)
static bool operator >(Measurement<T> v1, Measurement<T> v2)
static bool operator >=(Measurement<T> v1, Measurement<T> v2)
static Tuple<double, T> implicit operator(Measurement<T> value)
static ValueTuple<double, T> implicit operator(Measurement<T> value)
static DecimalMeasurement<T> implicit operator(Measurement<T> value)
static bool operator !=(Measurement<T> v1, Measurement<T> v2)
static bool operator <(Measurement<T> v1, Measurement<T> v2)
static bool operator <=(Measurement<T> v1, Measurement<T> v2)
static Measurement<T> operator *(Measurement<T> v1, double v2)
static Measurement<T> operator *(double v1, Measurement<T> v2)
static Measurement<T> operator -(Measurement<T> v1, Measurement<T> v2)
static Measurement<T> operator -(Measurement<T> v1)
static Measurement<T> operator +(Measurement<T> v1)
static Measurement<T> Parse(string text)
static Measurement<T> Parse(ReadOnlySpan<char> text)
static Measurement<T> Parse(string text, IFormatProvider provider)
static Measurement<T> Parse(ReadOnlySpan<char> text, IFormatProvider provider)
static T ParseUnitName(string name)
Measurement<T> To(T unit)
static double ToBase(double value, T unit)
string ToString()
string ToString(IFormatProvider cultureInfo)
string ToString(string format)
string ToString(string format, IFormatProvider formatProvider)
bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
bool TryFormat(Span<byte> utf8Destination, out int bytesWritten, ReadOnlySpan<char> format, IFormatProvider formatProvider)
static bool TryParse(string text, out Measurement<T> value)
static bool TryParse(ReadOnlySpan<char> text, out Measurement<T> value)
static bool TryParse(CultureInfo cultureInfo, string text, out Measurement<T> value)
static bool TryParse(CultureInfo cultureInfo, ReadOnlySpan<char> text, out Measurement<T> value)
static bool TryParse(string text, IFormatProvider provider, out Measurement<T> value)
static bool TryParse(ReadOnlySpan<char> text, IFormatProvider provider, out Measurement<T> value)
```

## MeasurementEnumerableExtensions  (class)

```csharp
static Measurement<T> Average<T>(this IEnumerable<Measurement<T>> values)
static DecimalMeasurement<T> Average<T>(this IEnumerable<DecimalMeasurement<T>> values)
static Measurement<T> Sum<T>(this IEnumerable<Measurement<T>> values)
static DecimalMeasurement<T> Sum<T>(this IEnumerable<DecimalMeasurement<T>> values)
```

## MeasurementJsonConverter  (class)

```csharp
ctor()
bool CanConvert(Type typeToConvert)
JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
```

## MeasurementMath  (class)

```csharp
static Measurement<T> Abs<T>(this Measurement<T> value)
static DecimalMeasurement<T> Abs<T>(this DecimalMeasurement<T> value)
static Measurement<AccelerationUnit> Acceleration(Measurement<VelocityUnit> velocity, TimeSpan time)
static Measurement<AngularUnit> Acos(double value)
static Measurement<AngularUnit> Asin(double value)
static Measurement<AngularUnit> Atan(double value)
static Measurement<T> Clamp<T>(Measurement<T> value, Measurement<T> min, Measurement<T> max)
static DecimalMeasurement<T> Clamp<T>(DecimalMeasurement<T> value, DecimalMeasurement<T> min, DecimalMeasurement<T> max)
static double Cos(this Measurement<AngularUnit> value)
static double Cos(this DecimalMeasurement<AngularUnit> value)
static Measurement<DensityUnit> Density(Measurement<WeightUnit> weight, Measurement<VolumeUnit> volume)
static Measurement<DistanceUnit> DistanceTraveled(Measurement<VelocityUnit> velocity, TimeSpan travelTime)
static Measurement<DistanceUnit> DistanceTraveled(Measurement<AccelerationUnit> acceleration, TimeSpan travelTime)
static Measurement<ForceUnit> Force(Measurement<WeightUnit> weight, Measurement<AccelerationUnit> acceleration)
static Measurement<EnergyUnit> KineticEnergy(Measurement<WeightUnit> weight, Measurement<VelocityUnit> velocity)
static Measurement<T> Max<T>(Measurement<T> first, Measurement<T> second)
static DecimalMeasurement<T> Max<T>(DecimalMeasurement<T> first, DecimalMeasurement<T> second)
static Measurement<T> Min<T>(Measurement<T> first, Measurement<T> second)
static DecimalMeasurement<T> Min<T>(DecimalMeasurement<T> first, DecimalMeasurement<T> second)
static Measurement<T> Pow<T>(this Measurement<T> value, double exp)
static DecimalMeasurement<T> Pow<T>(this DecimalMeasurement<T> value, decimal exp)
static Measurement<PowerUnit> Power(Measurement<TorqueUnit> torque, Measurement<RotationalSpeedUnit> rotationalSpeed)
static Measurement<PressureUnit> Pressure(Measurement<WeightUnit> weight, Measurement<AreaUnit> area)
static Measurement<VolumeUnit> RecangularPrismVolume(Measurement<AreaUnit> area, Measurement<DistanceUnit> depth)     // OBSOLETE
static Measurement<AreaUnit> RectangleArea(Measurement<DistanceUnit> width, Measurement<DistanceUnit> height)
static Measurement<VolumeUnit> RectangularPrismVolume(Measurement<AreaUnit> area, Measurement<DistanceUnit> depth)
static Measurement<T> Round<T>(this Measurement<T> value, int digits)
static Measurement<T> Round<T>(this Measurement<T> value)
static DecimalMeasurement<T> Round<T>(this DecimalMeasurement<T> value, int digits)
static DecimalMeasurement<T> Round<T>(this DecimalMeasurement<T> value)
static int Sign<T>(this Measurement<T> value)
static int Sign<T>(this DecimalMeasurement<T> value)
static double Sin(this Measurement<AngularUnit> value)
static double Sin(this DecimalMeasurement<AngularUnit> value)
static Measurement<T> Sqrt<T>(this Measurement<T> value)
static DecimalMeasurement<T> Sqrt<T>(this DecimalMeasurement<T> value)
static double Tan(this Measurement<AngularUnit> value)
static double Tan(this DecimalMeasurement<AngularUnit> value)
static TimeSpan TravelTime(Measurement<DistanceUnit> distance, Measurement<VelocityUnit> velocity)
static Measurement<VelocityUnit> Velocity(Measurement<DistanceUnit> distance, TimeSpan time)
static Measurement<VelocityUnit> Velocity(Measurement<AccelerationUnit> acceleration, TimeSpan time)
static Measurement<WeightUnit> Weight(Measurement<DensityUnit> density, Measurement<VolumeUnit> volume)
```

## UnitAttribute  (class)

```csharp
ctor(string name, int defaultAccuracy)
ctor(string name, string alternativeName, int defaultAccuracy)
string AlterantiveName { get; set; }     // OBSOLETE
string AlternativeName { get; set; }
int DefaultAccuracy { get; set; }
bool HasAlternativeName { get; }
string Name { get; set; }
```

## UnitEnumValidator  (class)

```csharp
static UnitValidationReport Validate<T>()
static UnitValidationReport Validate(Type unitType)
```

## UnitExtensions  (class)

```csharp
static Measurement<T> As<T>(this double value, T unit)
static Measurement<T> As<T>(this int value, T unit)
static IEnumerable<Measurement<T>> As<T>(this IEnumerable<double> values, T unit)
static DecimalMeasurement<T> AsDecimal<T>(this decimal value, T unit)
static Measurement<T> New<T>(this T unit, double value)
static DecimalMeasurement<T> NewDecimal<T>(this T unit, decimal value)
```

## UnitValidationFinding  (class)

```csharp
ctor(string code, UnitValidationSeverity severity, string unit, string message)
string Code { get; }
string Message { get; }
UnitValidationSeverity Severity { get; }
string Unit { get; }
string ToString()
```

## UnitValidationReport  (class)

```csharp
ctor(Type unitType, IReadOnlyList<UnitValidationFinding> findings)
IReadOnlyList<UnitValidationFinding> Findings { get; }
bool HasErrors { get; }
bool IsValid { get; }
Type UnitType { get; }
IEnumerable<UnitValidationFinding> Errors()
void ThrowIfInvalid()
string ToString()
IEnumerable<UnitValidationFinding> Warnings()
```

