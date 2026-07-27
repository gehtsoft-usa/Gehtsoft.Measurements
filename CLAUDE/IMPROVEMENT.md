# Gehtsoft.Measurements — Review Findings, Round 2 (2026-07-27)

Fresh review pass against `main` at 1.1.18. The Round 1 items (A1–A7 performance/
correctness, B1–B4 unit-set additions) are all implemented and verified — see git
history and `BASELINE_PERF.md`. File/line references below are against `main` as it
stood before this round.

> **Status (2026-07-27):** sections **C, D, E, F and G are implemented**; section H was
> explicitly left out of scope. The test suite went from 387 to 527 tests, all green.
> Version files were deliberately not touched. Deviations from the original findings are
> recorded under each item and summarized at the end.

## C. Modern .NET API surface — the internals are ready, the API isn't

### C1. Span parsing + `IParsable` / `ISpanParsable` ✅ DONE
`TryParseInternal` now takes a `ReadOnlySpan<char>` and a `NumberFormatInfo` (obtained through
`NumberFormatInfo.GetInstance`, which is allocation-free for a `CultureInfo` and accepts any
`IFormatProvider`). Added, with the existing overloads untouched:

- `TryParse(ReadOnlySpan<char>, out …)` and `TryParse(CultureInfo, ReadOnlySpan<char>, out …)`
- `TryParse(string, IFormatProvider, out …)` and `TryParse(ReadOnlySpan<char>, IFormatProvider, out …)`
- `Parse(string)`, `Parse(ReadOnlySpan<char>)`, `Parse(string, IFormatProvider)`,
  `Parse(ReadOnlySpan<char>, IFormatProvider)` — there was no `Parse` at all before
- `IParsable<…>` and `ISpanParsable<…>` on both measurement types

The culture asymmetry is unchanged and now documented on the constructor: the text constructor
parses in the invariant culture (so the value round-trips through `Text`) while
`TryParse(string)` uses the current culture. Changing either would break JSON round-trips.

Tests: `SpanParseTest.cs`, plus span rows in `ParseAllocationTest.cs`.

### C2. `ISpanFormattable` + cached format strings ✅ DONE
`UnitUtils.AccuracyFormat` serves `"N0"`…`"N15"` from a static table instead of building
`$"N{accuracy}"` on every `"ND"` format. `TryFormat` writes the value and then the unit name
straight into the caller's buffer; `ToString` runs on top of it through a 512-character stack
buffer and falls back to the old concatenation for formats which overflow it (`"N100"` of a
huge value). Both `ISpanFormattable` and `IUtf8SpanFormattable` are implemented — the UTF-8
variant transcodes the unit name with `Encoding.UTF8.TryGetBytes` directly into the
destination, which needs no name table and handles the non-ASCII names (`°`, `·`, `µ`, `³`).

Tests: `FormatTest.cs` — every unit of every enumeration against seven formats, in both
character and UTF-8 form, plus the too-small-destination and fallback cases.

### C3. Generic math operator interfaces ✅ DONE, with one omission
Implemented on both types: `IAdditionOperators`, `ISubtractionOperators`,
`IUnaryNegationOperators`, `IUnaryPlusOperators`, `IComparisonOperators` (which carries
`IEqualityOperators`), `IMultiplyOperators<…, double, …>`, `IDivisionOperators<…, double, …>`
and `IDivisionOperators<…, TSelf, double>` for the ratio operator.

**`IAdditiveIdentity` was deliberately left out.** It asserts that `x + Identity == x`, which is
false for the affine units in the library. `ZERO` is zero of the *base* unit, so for
`TemperatureUnit` (base Fahrenheit) `50°C + ZERO` converts 0°F into −17.78°C and yields
32.22°C. Declaring the interface would state a property the type does not have.

Tests: `GenericMathTest.cs` — helpers constrained to each interface, which compile only if the
constraints really are satisfied.

## D. Robustness for consumer-defined enums ✅ DONE

`UnitUtils.GetBase` now rejects a member missing either attribute, an enumeration with no base
unit, and an enumeration with more than one, naming the offending member. `GetParseList`
rejects a parse name used by two units (primary or alternative). All 17 shipped enumerations
were checked against these rules by reflection before the checks were added — the `"ps"`,
`"hp"`, `"at"` and `"N"` name collisions are all `[Obsolete]` misspellings, which were already
skipped, so nothing in the library trips them.

D3 landed with C1: `((string)null).AsSpan()` is empty, so `TryParse(null)` is a routine `false`,
and the text constructor throws `ArgumentNullException` rather than failing with a null
reference.

Tests: `UnitValidationTest.cs` — six deliberately broken enumerations, each asserting the
message; null handling in `SpanParseTest.cs`.

## E. Suffix-first parsing ✅ DONE

The parser looks for the longest unit name which *ends* the text and parses what is in front of
it, retrying with the next shorter name when that is not a number. `"1e3m"`, `"1E3m"` and
`"1e-3m"` now parse. The retry is what keeps names holding digits and separators
(`in/100yd`, `cm/100m`, `l/100km`, `imp.mpg`) and names ending with another name (`mrad` over
`rad`, `mm` over `m`) unambiguous. Still allocation-free — `MemoryExtensions.EndsWith` over the
same static parse list.

Everything the old left-to-right scanner accepted still parses, and everything it rejected is
still rejected; `UnitNamesTest` re-validates all 17 enumerations, every name, unchanged.

Tests: `ParseSyntaxTest.cs`.

## F. Project hygiene ✅ DONE

### F1. README refresh
Rewritten. **The count in the original finding was wrong — there are 17 unit enumerations, not
19.** Two samples in the old README did not compile: `DistanceUnit.Feet` (the member is `Foot`)
and `ToString("N3")` (no single-argument format overload existed). The first is fixed, the
second is now supported — see the extra item below. The README now covers the net8.0
requirement, `DecimalMeasurement`, the span parse/format API, aggregation, the cross-dimension
helpers, both JSON shapes, and the allocation guarantees.

`ReadmeSampleTest.cs` pins every sample, because nothing checked them before, which is exactly
why two of them had rotted.

### F2. Benchmark project
`Gehtsoft.Measurements.Benchmark` created and added to the solution — BenchmarkDotNet 0.14.0,
`[MemoryDiagnoser]`, reproducing the `BASELINE_PERF.md` table and adding rows for this round
(span parse, exponent parse, `TryFormat`, interpolation). Note that CI builds the whole
solution and will now restore BenchmarkDotNet.

### F3. Allocation guards extended
The probe moved into `AllocationProbe.cs` and `OperationAllocationTest.cs` now measures
conversion, comparison, arithmetic and `TryFormat` at 0 B/op, next to the parse guards. The
`"ND"` format is checked by comparing its allocation against the equivalent explicit format
rather than against zero, since `ToString` must allocate the string it returns.

## G. Feature additions ✅ DONE

### G1. Cross-dimension math helpers
`MeasurementMath` gained `Power(torque, rotationalSpeed)` (the one promised in Round 1 B2),
`Force(weight, acceleration)`, `Density(weight, volume)`, `Weight(density, volume)` and
`Acceleration(velocity, time)`. Tests: `CrossDimensionMathTest.cs`, with values derived from
the definition of each quantity rather than from the implementation.

### G2. Aggregation and convenience helpers — scope reduced, for a reason
**`Min`/`Max` over a sequence were not added.** Both measurement types implement `IComparable`,
so `Enumerable.Min`, `Max`, `MinBy`, `MaxBy` and `OrderBy` already work and already compare
across units (verified: `Min` of `1ft` and `6in` is `6in`). Defining them here as well would
make `values.Min()` an *ambiguous call* for any caller with both `using System.Linq` and
`using Gehtsoft.Measurements` — a compile error in consumer code.

Added instead: `Sum` and `Average` over sequences of either type, which the standard operators
genuinely cannot express, plus `MeasurementMath.Min`/`Max`/`Clamp` over two or three values and
`Round(digits)`/`Round()`. Tests: `AggregationTest.cs`, which also pins the LINQ behaviour so
nobody adds the ambiguous overloads later.

### G3. Opt-in compact JSON converter
`MeasurementJsonConverter` is a `JsonConverterFactory` serving both measurement types and any
unit enumeration, reading and writing the bare string `"10.5in"` in the invariant culture. The
default `{"value":"10.5in"}` shape is untouched unless the converter is registered — a test
asserts that. Tests: `JsonConverterTest.cs`.

## H1. Trimming and NativeAOT ✅ DONE

The library is now **trim-clean and declared trimmable**, which turned out to be a much better
outcome than the "annotate and warn" the finding asked for: most of what the trimmer objected
to could be *fixed* rather than merely declared.

- `EnableTrimAnalyzer` and `EnableAotAnalyzer` are on in the project, so the build fails the
  moment a new reflection path appears unannotated. `IsTrimmable` is set.
- The enum reflection is preserved properly by annotating the generic parameter with
  `[DynamicallyAccessedMembers(PublicFields)]` — on `Measurement<T>`, `DecimalMeasurement<T>`
  and every generic method which flows `T` into them (`UnitUtils`, `CodeGenerator`,
  `MeasurementMath`, `UnitExtensions`, `MeasurementEnumerableExtensions`, the JSON converters).
  Trimming now keeps the unit fields instead of silently removing them.
- `Math.Tan`/`Math.Atan` are taken from a delegate (`((Func<double, double>)Math.Tan).Method`)
  instead of `Type.GetMethod(string)`, so the trimmer can see them being used.
- The custom conversion operations are invoked through `typeof(ICustomConversionOperation)`
  rather than `op.GetType()`, which makes the called method statically known.
- What genuinely cannot be preserved carries the annotation: the `ConversionAttribute`
  constructor which locates a custom conversion by scanning the loaded assemblies is
  `[RequiresUnreferencedCode]`, and `MeasurementJsonConverter` is `[RequiresDynamicCode]` plus
  `[RequiresUnreferencedCode]` because it closes a generic type over the unit enumeration at
  run time. (The annotation sits on the class, not on the `CreateConverter` override — IL3051
  forbids an override from carrying an annotation its base declaration does not have.)

**Verified end to end**, not only by the analyzer: a self-contained `PublishTrimmed` with
`TrimMode=full` publishes with zero IL warnings and the resulting binary converts, parses
`"1e3m"`, formats, enumerates all 19 `DistanceUnit` units, does the affine temperature
conversion and runs the decimal path correctly. Note the compiler-time analyzers were **not**
sufficient here: the whole-program ILLink pass found eight more warnings (reflection flowing
through a static field and through `GetType()`) that the in-build analyzers did not.

`IsAotCompatible` is deliberately **not** set. `LambdaExpression.Compile` carries no
`RequiresDynamicCode` in .NET 8 — checked — because NativeAOT falls back to the expression
interpreter, so the library *works*, but the interpreted delegates are far slower than the
compiled ones the performance numbers depend on. Claiming AOT compatibility would be
misleading until the code generation moves to a source generator.

## I. Unit enumeration validator ✅ DONE

`UnitEnumValidator.Validate<T>()` returns a `UnitValidationReport` listing everything wrong
with a unit enumeration, instead of throwing on the first problem the way the static
initializer does. Aimed at enumerations declared outside the library; the 17 shipped ones are
validated by it in the test suite.

Seventeen checks, each with a stable code: GM001–GM007 structural (missing attribute, no or
duplicated base unit, empty or duplicated name), GM008–GM013 declaration sanity (bad or zero
factor, a factor on an operation which ignores it, an accuracy outside 0..15, a name which can
swallow part of the value, a custom conversion without the decimal interface), GM014–GM017
behavioural (base unit identity, conversion round-trip, non-finite result, format-and-parse
round-trip).

Two design points worth keeping in mind:

- **The structural checks run first and the behavioural ones only if they pass.** Touching
  `Measurement<T>` raises the validation in the static initializer, which would both hide the
  collected findings behind an exception and leave the closed type faulted for the process.
  `ValidatingABrokenEnumerationReturnsInsteadOfThrowing` guards this.
- **The round-trip probes are chosen per unit.** The first version reported every `Tan`
  conversion as broken, because the arc tangent only undoes the tangent between minus and plus
  a quarter turn and the probe set went well past it. A tangent conversion now gets probes
  inside that range; `APeriodicConversionIsNotReportedAsBroken` guards it.

`Validate<T>()` is trim-clean and verified working in a fully trimmed binary. Only the
`Validate(Type)` overload, which closes the generic method at run time for a caller which
discovers unit types rather than naming them, carries `RequiresDynamicCode` and
`RequiresUnreferencedCode`. That overload is the hook for the CLI or build-time tool which was
deliberately left out of this round.

## H2. Longer-term / structural — still open

- **The `Measurement`/`DecimalMeasurement` duplication.** Still ~85% copy-paste; every item
  in this round was written twice. A source generator or a shared template would end it, and
  would also remove the `Expression.Compile` dependency, which is what stands between the
  library and a genuine `IsAotCompatible`. The cheap stopgap remains a reflection test
  asserting public-API parity between the twins.

## J. `DensityUnit.OuncesPerCubicFeet` was misnamed ✅ DONE

Found while generating the unit catalogue for the Claude Code skill under `SKILL/`. The member
was named after the cubic foot, but both its unit name (`oz/in³`) and its factor (1729.994) are
ounces per cubic **inch** — check the arithmetic: 28.349523125 g over 16.387064 cm³ is
1729.994 kg/m³. Only the C# identifier was wrong, so every caller who used it by its name got
the right answer and every caller who trusted the identifier got a number 1728 times off.

Fixed the way the Round 1 B4 misspellings were: `OuncesPerCubicInch` added at the **end** of the
enumeration so no other member's numeric value moves, and `OuncesPerCubicFeet` kept in place and
marked `[Obsolete]`, which excludes it from `GetUnitNames()` and from parsing while it still
converts for previously persisted values. Covered by
`BSeriesUnitsTest.ObsoleteEnumMembers_ExcludedFromListingAndParsing_ButStillConvert` and by a new
`Density_OuncesPerCubicInch`, which pins the factor to its definition so nobody later "fixes" the
rename in the wrong direction.

`DensityUnit.cs` had no `using System;`, which is why adding `[Obsolete]` broke the build at
first — worth knowing if another unit file needs the same treatment.

## Extra items found while implementing

### Added: `ToString(string format)`
There was no single-argument format overload, so `measurement.ToString("N3")` — the most
obvious way to format, and what the README already claimed — did not compile. Added to both
types, formatting in the invariant culture to match the parameterless `ToString()`.
Caveat: `ToString(null)` now becomes an ambiguous call between this and
`ToString(IFormatProvider)`. That is a source-level break only, for a pathological call.

### Fixed: `DecimalMeasurement<T>.ZERO` had the wrong type
It was declared as `Measurement<T>`, the double-based type, and compiled at call sites only
because of the implicit conversion between the two — which quietly dragged the value through a
`double`. Now typed `DecimalMeasurement<T>`. This is a binary and source breaking change for
anyone who assigned it to an explicitly typed `Measurement<T>` variable, taken deliberately.
`DecimalCoreClassesTest.ZeroIsADecimalMeasurement` asserts the property type by reflection so
it cannot regress.

## What is left

| Item | Why it is still open |
|------|----------------------|
| H2 De-duplicate the two measurement types | Large; the parity test is the cheap first step. Also the route to a real `IsAotCompatible` |
| `IAdditiveIdentity` | Only meaningful if affine units are excluded somehow |
| A trimming check in CI | The trimmed publish was run by hand. Automating it needs the runtime packs in the workflow |
