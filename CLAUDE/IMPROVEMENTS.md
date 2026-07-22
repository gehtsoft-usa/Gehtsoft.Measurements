# Gehtsoft.Measurements — Review Findings (2026-07-22)

Package review covering performance improvements and suggested additions to the
conversion set. File/line references are against the current `main` branch.

> **Status (2026-07-22):** A1, A2, A3, A4, A6 implemented and benchmarked — see
> `BASELINE_PERF.md` for before/after (conversions/comparisons now 0-alloc and
> ~8–12× faster; `TryParse`-failure ~319× faster; `GetUnitNames` ~347× faster).
> A5 (multi-target) superseded by the net8.0-only retarget. A7 resolved (Option B:
> tolerance-based `Equals`/`==` with a quantized, Dictionary-safe `GetHashCode`).
> B1–B4 implemented and independently verified against the `pint` Python unit
> registry (45/47 new-or-changed conversions match to floating-point precision;
> BTU-family within ~1e-7 = pint's own rounding; furlong/fathom differ by 2e-6
> because we use the international foot, not the retired US survey foot).

## A. Performance improvements

### A1. Enum boxing in the hottest path — `Convert` ✅ DONE
`Measurement.cs:141-150` (same code in `DecimalMeasurement.cs:139-144`).

`from.CompareTo(to)` resolves to `Enum.CompareTo(object)`, which boxes **both**
operands. Every `In()`, `To()`, arithmetic operator, `Equals`, and
`GetHashCode` funnels through this, doing up to three boxed comparisons per
conversion — 2–6 heap allocations per conversion on netstandard2.0.

**Fix:** replace with `EqualityComparer<T>.Default.Equals(from, to)`, which the
JIT devirtualizes to a plain integer compare on .NET Core+.

**Bonus:** the two base-unit early-out checks are redundant — the generated
switch already returns the value unchanged for the `ConversionOperation.Base`
case — so a single `from == to` equality check suffices.

### A2. Transcendental math in every comparison — `eps()` ✅ DONE
`Measurement.cs:341`, used by `CompareTo` (line 328) which backs all six
comparison operators.

`Math.Pow(10, Math.Round(Math.Log10(value)) - 12)` runs twice per comparison.
A relative-tolerance check is ~50x cheaper:

```csharp
Math.Abs(v1 - v2) <= 1e-12 * Math.Max(Math.Abs(v1), Math.Abs(v2))
```

**Latent correctness bug fixed by this:** `Math.Log10` of a negative value
returns NaN, so comparisons between negative measurements currently get **no
tolerance at all**.

### A3. Exception-driven parsing ✅ DONE
`Measurement.cs:283-290` (`TryParseInternal` catches `ArgumentException` from
the generated `ParseUnitName`).

A thrown exception costs microseconds vs. nanoseconds. For a `TryParse` API
expected to fail routinely, build a non-throwing lookup instead: a static
`Dictionary<string, T>` (ordinal comparer) constructed once per closed generic
type. This is also O(1) versus the linear `string.Equals` chain that
`Expression.Switch` on strings compiles to.

### A4. `GetUnitNames()` runs reflection on every call ✅ DONE
`Measurement.cs:190` → `UnitUtils.GetUnits<T>()` re-enumerates enum fields and
re-reads attributes each call. Cache the result in a static like the other
generated delegates (return a defensive copy or `IReadOnlyList`).

### A5. Multi-target `netstandard2.0;net8.0` — SUPERSEDED (retargeted net8.0-only)
`Gehtsoft.Measurements.csproj` targets only `netstandard2.0`. Adding `net8.0`:
- gets devirtualized `EqualityComparer<T>.Default` guaranteed,
- allows `ReadOnlySpan<char>` slicing in `TryParseInternal`, eliminating the
  two `Substring` allocations per parse.

The test project already targets net8.0.

### A6. Culture bug in `ToString` (correctness, sits in serialization path) ✅ DONE
`Measurement.cs:114`: the `"NF"` branch calls `Value.ToString()` **without**
the format provider. The `Text` property — which is what `System.Text.Json`
serializes — therefore uses the *current* culture despite passing
`CultureInfo.InvariantCulture`. On a locale with `,` as the decimal separator
this produces `"3,5m"`, which then fails to round-trip through the
`[JsonConstructor]` (it parses with invariant culture).

**Fix:** `Value.ToString(formatProvider)`.

### A7. Equals / operator== / GetHashCode inconsistency ✅ DONE (Option B)
Original problem:
- `Equals` — exact base-value equality
- `operator ==` — tolerance-based via `CompareTo`
- `GetHashCode` — hashes the exact base value

Two measurements could be `==` yet have different hash codes and `Equals() == false`.

Resolution (double `Measurement<T>` only — `DecimalMeasurement<T>` was already
self-consistent because its `CompareTo` is exact):
- `Equals` now delegates to `CompareTo == 0`, so `==`, `Equals`, and the ordering
  operators all share the tolerance semantic. `1 m == 100 cm` is now true through all
  of them.
- `GetHashCode` quantizes the base value to `HashSignificantDigits` (10) significant
  figures before hashing — coarser than the `1e-12` relative comparison tolerance, so
  tolerance-equal values (which agree to ~12 sig figs) round to the same grid point and
  hash equally. Verified as a `Dictionary` key across units and round-trips
  (`CoreClassesTest.GetHashCode_UsableAsDictionaryKey`, 500 keys).
- **Fundamental caveat:** relative-tolerance equality is not transitive, so no hash is
  perfect. Two tolerance-equal values astride a quantization boundary can still hash
  differently (~1-in-a-million even at the tolerance edge; ~1-in-a-billion for realistic
  ULP-level conversion differences). The only consequence is an occasional missed
  hashed-lookup (a logical duplicate), never a corrupted collection — and it is strictly
  better than an exact hash, which would mis-hash *every* cross-unit equal pair.

## B. Meaningful additions to the conversion set

The current set skews ballistics/automotive; suggestions stay in that spirit.

### B1. New units in existing enums ✅ DONE

| Enum | Additions |
|------|-----------|
| `AngularUnit` | `ArcSecond` (1/60 MOA) |
| `DistanceUnit` | `Micrometer`, `Decimeter`, `Thou` (0.001", machining/barrel specs), `Furlong`, `Fathom`, `Hand` |
| `WeightUnit` | `Milligram`, `Stone` (14 lb), `Carat` (0.2 g), `Slug` (14.59390 kg — pairs with force/acceleration enums) |
| `VelocityUnit` | `InchesPerSecond`, `CentimetersPerSecond` (twist-rate math), `FeetPerMinute` |
| `PressureUnit` | `Hectopascal` (modern meteorological label), `Megapascal` (chamber pressure), `Torr` (exactly 101325/760), `InchesOfWater`, `PoundsPerSquareFoot` |
| `EnergyUnit` | `Kilojoule`, `Calorie`, `Kilocalorie`, `Erg` |
| `PowerUnit` | `Kilowatt`, `Megawatt`, `BTUPerHour` (HVAC) |
| `VolumeUnit` | `CubicCentimeter` ("cc"), `ImperialFluidOunce` (28.4130625 ml), `OilBarrel` (158.987 l), `Teaspoon`, `Tablespoon`, `Cup` |
| `DensityUnit` | `GrainsPerCubicInch` (powder loading density), `KilogramPerLiter`, `PoundsPerGallon` |
| `GasConsumptionUnit` | `KilometersPerLiter`, `ImperialMilesPerGallon` (1 imp mpg = 2.82481 km/l; UK mpg ≠ US mpg) |
| `AccelerationUnit` | `InchesPerSecondSquare` |

### B2. New unit enums ✅ DONE
- **`TorqueUnit`** — N·m, kgf·m, ft·lbf, in·lbf. Dimensionally distinct from
  energy even though ft·lb exists in `EnergyUnit`; needed by automotive users.
- **`RotationalSpeedUnit`** — rpm, rad/s, Hz. Pairs naturally with torque
  (a `MeasurementMath.Power(torque, speed)` helper would be a good companion).

### B3. Accuracy fixes in existing factors ✅ DONE
Several factors are truncated where exact values exist:

| Unit | Current | Exact |
|------|---------|-------|
| `WeightUnit.USTonne` | `15432358.3529 * 0.907` | 14,000,000 grains (2000 lb × 7000 gr) — current is off ~0.02% |
| `WeightUnit.UKTonne` | `15432358.3529 * 1.016` | 15,680,000 grains (2240 lb × 7000 gr) |
| `VolumeUnit.ImperialPint` | 568 | 568.26125 |
| `VolumeUnit.ImperialQuart` | 1137 | 1136.5225 |
| `VolumeUnit.ImperialGallon` | 4546 | 4546.09 |
| `VolumeUnit.Ounce` (US fl oz) | 29.57 | 29.5735295625 |
| `VolumeUnit.Gallon` | 3785.412 | 3785.411784 |
| `VelocityUnit.FeetPerSecond` | Divide 3.2808399 | Multiply 0.3048 (exact) |
| `VelocityUnit.MilesPerHour` | Divide 2.23693629 | Multiply 0.44704 (exact) |
| `VelocityUnit.Knot` | Divide 1.94384449 | Multiply 1852.0/3600.0 (exact) |
| `EnergyUnit.BTU` | 1055 | 1055.05585262 |
| `EnergyUnit.HpH` | 2,684,500 | 2,684,519.54 (mechanical hp·h) |
| `PowerUnit.MetricHoursePower` | 735.5 | 735.49875 |
| `PowerUnit.MechanicalHoursePower` | 745.7 | 745.699872 |
| `PressureUnit.MillimetersOfWater` | 9.80638 | 9.80665 (conventional g₀) |

### B4. Public-API typos ✅ DONE (obsolete aliases; enum aliases excluded from listing/parsing via UnitUtils)
Fix with `[Obsolete]` aliases rather than breaking renames:
- `WeightUnit.Neuton` → `Newton`
- `PowerUnit.MetricHoursePower` / `MechanicalHoursePower` → `...HorsePower`
- `PressureUnit.TechincalAtmosphere` → `TechnicalAtmosphere`
- `MeasurementMath.RecangularPrismVolume` → `RectangularPrismVolume`
- `UnitAttribute.AlterantiveName` → `AlternativeName`

## Suggested priority
1. A1 (boxing), A2 (epsilon), A6 (culture bug) — small, safe, highest value per line.
2. A5 (multi-targeting) + A3 (parse path).
3. B3 accuracy fixes (may shift test expectations slightly).
4. B1/B2 new units.
5. A7, B4 — API-surface decisions, need versioning consideration.
