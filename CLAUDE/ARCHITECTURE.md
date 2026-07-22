# Gehtsoft.Measurements — Architecture Notes (2026-07-22)

## Purpose
A strongly-typed units-of-measurement library for .NET. A measurement is a
value + unit pair (`3.5 m`, `100 gr`), with unit conversion, arithmetic,
comparison, parsing/formatting, and JSON/XML serialization. The unit set skews
toward ballistics and automotive domains (grains, MOA, mil, twist-friendly
velocities, gas consumption).

## Solution layout

```
Gehtsoft.Measurements.sln
├── Gehtsoft.Measurements/          # the library (net8.0)
├── Gehtsoft.Measurements.Test/     # xUnit tests (net8.0, AwesomeAssertions 9.5.0)
├── doc/                            # documentation
├── nuget/                          # packaging assets
└── .github workflows → MyGet feed  # CI publishes to MyGet (see commit f7a30c7)
```

Single-assembly library with **no external package dependencies** — it uses
`System.Text.Json`, which ships in the net8.0 shared framework (the library
previously targeted netstandard2.0 and carried an explicit `System.Text.Json`
package reference for it).

## Core design

### 1. Units are plain enums decorated with attributes
Each measurement kind is an `enum` (e.g. `DistanceUnit`, `WeightUnit`). Every
enum member carries two attributes:

- **`[Unit("m", "alt", accuracy)]`** (`UnitAttribute.cs`) — display/parse name,
  optional alternative name, and default decimal places for formatting.
- **`[Conversion(op, factor, op2, factor2)]`** (`ConversionAttribute.cs`) —
  how to convert *this unit → the base unit* as one or two primitive
  operations. Exactly one member per enum is `ConversionOperation.Base`.

`ConversionOperation` (ConversionOperation.cs) is a small op-code set:
Add / Subtract / SubtractFromFactor / Multiply / Divide / DivideFactor /
Negate / Tan / Atan / Custom / Base / None. The reverse (base → unit)
conversion is derived automatically by inverting the ops in reverse order.

Non-linear or irregular conversions plug in via `ICustomConversionOperation`
(double) and `ICustomConversionOperation2` (adds decimal support); the
implementing type's full name is given to the `ConversionAttribute`, which
locates it by scanning loaded assemblies (cached in a static
`ConcurrentDictionary`) and instantiates it with `Activator`.

### 2. Runtime code generation instead of per-call reflection
`CodeGenerator.cs` is the heart of the library. For each closed generic
`Measurement<T>` it builds compiled delegates once (held in
`static readonly` fields, so one-time cost per unit type per process):

| Delegate | Built by | Purpose |
|----------|----------|---------|
| `Func<double,T,double>` ToBase / FromBase | `GenerateConversion<T>(bool)` | switch over enum → inlined arithmetic expression |
| `Func<decimal,T,decimal>` | `GenerateConversionDecimal<T>` | decimal twin (doubles through `Math.Tan/Atan` where decimal has no primitive) |
| `Func<T,string>` | `GenerateGetUnitName<T>` | unit → display name |
| `Func<string,T>` | `GenerateParseUnitName<T>` | name → unit (throws on unknown) |
| `Func<T,int>` | `GenerateGetDefaultUnitAccuracy<T>` | unit → default decimals |

All are `Expression.Switch`-based; attribute reflection happens only at
delegate-build time. `UnitUtils.cs` holds the remaining direct reflection
(`GetBase<T>`, `GetUnits<T>` — note `GetUnits` is *not* cached; see
IMPROVEMENTS.md A4).

### 3. The value types
- **`Measurement<T>`** (Measurement.cs) — `readonly struct` with `double Value`
  + `T Unit` where `T : Enum`. Immutable; conversions return new instances.
  - All conversions route through the base unit: `from → base → to`.
  - Full operator set: `+ - * /` (with scalars and between measurements;
    measurement ÷ measurement yields a dimensionless `double` ratio),
    all six comparisons (epsilon-tolerant via base-unit comparison).
  - Parsing: `TryParse` scans digits from the left, splits number from unit
    suffix ("10.5m" → 10.5 + "m"), culture-aware.
  - Formatting: `ToString(format, provider)` with special formats `"ND"`
    (unit's default accuracy) and `"NF"` (full precision).
  - JSON: serializes as a single string property `value` (the `Text`
    property + `[JsonConstructor]` on the string ctor).
  - Implicit/explicit conversions to/from tuples and to
    `DecimalMeasurement<T>`.
- **`DecimalMeasurement<T>`** (DecimalMeasurement.cs) — near-verbatim twin
  backed by `decimal` for accuracy-critical use. Structure mirrors
  `Measurement<T>` closely (parallel maintenance burden: fixes must be
  applied to both).

### 4. Helper layers
- **`MeasurementMath`** (MeasurementMath.cs) — static/extension math:
  trig for `Measurement<AngularUnit>` (Sin/Cos/Tan/Asin/Acos/Atan), generic
  Sqrt/Pow/Abs/Sign, and cross-dimension physics helpers (Velocity,
  KineticEnergy, RectangleArea, prism volume, Pressure, TravelTime,
  DistanceTraveled). Cross-dimension results are computed in SI and returned
  in SI units. Time is represented by `System.TimeSpan`, not a unit enum.
- **`UnitExtensions`** (UnitExtensions.cs) — fluent constructors:
  `DistanceUnit.Meter.New(10)`, `10.0.As(DistanceUnit.Meter)`,
  `IEnumerable<double>.As(unit)`, decimal variants.

## Unit enums (17)
Acceleration, Angular, Area, Density, Distance, Energy, Force,
GasConsumption, Power, Pressure, RotationalSpeed, SolidAngular, Temperature,
Torque, Velocity, Volume, Weight.

Base units are chosen per-domain rather than uniformly SI — e.g. Distance's
base is the **inch**, Weight's is the **grain**, Temperature's is
**Fahrenheit**, Velocity's is m/s, Energy's is the Joule. This is invisible to
callers (everything routes through `In()`/`To()`) but matters when adding
units: a new unit's `Conversion` attribute must target that domain's base.

Angular is the most interesting enum: it includes slope-style units
(inches/100yd, cm/100m, percent) that need `Tan`/`Atan` ops, which is why
those exist in the op-code set.

## Extension points
1. **New unit in an existing enum** — add a member with `[Unit]` +
   `[Conversion]`; no other code changes (delegates are regenerated at
   runtime). Mind the enum's base unit.
2. **New measurement kind** — add a new enum with one `Base` member;
   `Measurement<NewUnit>` works immediately.
3. **Irregular conversions** — implement `ICustomConversionOperation`(2) and
   reference it by type name in the attribute.
4. **Consumer-defined enums** — the generic machinery works for any enum with
   the right attributes, including ones defined outside this assembly
   (tests exercise this via `TestConversion.cs` / `TestConversion2.cs`).

## Testing
xUnit + AwesomeAssertions 9.5.0 (the free Apache-2.0 fork of FluentAssertions;
migrated off FluentAssertions 7.2.0, which went to a paid license at v8).
One test file per unit enum checking conversion factors, plus core-class tests
(parsing, serialization, operators, both double and decimal variants) and
`MeasurementMath` tests.

## Known quirks / gotchas
- `Measurement<T>` and `DecimalMeasurement<T>` are ~85% duplicated code; any
  fix in one usually belongs in the other.
- Equality is tolerance-based and unified across `==`, `Equals`, and the ordering
  operators for `Measurement<T>` (A7 resolved). `GetHashCode` quantizes to 10
  significant figures to stay consistent with that tolerance for `Dictionary`/`HashSet`
  use; see IMPROVEMENTS.md A7 for the (benign) non-transitivity caveat.
  `DecimalMeasurement<T>` uses exact comparison throughout.
- The `"NF"` format path drops the format provider (culture bug affecting
  JSON round-trips on non-invariant locales) — see IMPROVEMENTS.md A6.
- `WeightUnit.Neuton` [sic] is a force expressed as mass-equivalent, kept in
  the weight enum for practical reasons; a proper `ForceUnit` also exists.
- Now targets net8.0 (was netstandard2.0). The A5 multi-targeting item in
  IMPROVEMENTS.md is therefore resolved, and the net8.0-only enablers it
  called out are now available: `EqualityComparer<T>.Default` devirtualizes
  (A1) and `ReadOnlySpan<char>` slicing is usable in parsing (A3). Those
  performance rewrites are still pending in the code — only the target moved.
