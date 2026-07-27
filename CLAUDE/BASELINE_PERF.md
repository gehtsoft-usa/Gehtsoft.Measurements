# Performance Baseline — before A1–A6 (2026-07-22)

Captured with BenchmarkDotNet 0.14.0 (`[MemoryDiagnoser]`) on the pre-optimization
`main` (assembly 1.1.17), before any IMPROVEMENTS.md performance work.

- Runtime: .NET 8.0.29, X64 RyuJIT AVX-512, Debug→**Release** build, WSL2
- Subject type: `Measurement<DistanceUnit>` (base unit = Inch)
- Reproduce: `dotnet run -c Release --project Gehtsoft.Measurements.Benchmark`

| Method                     | Mean         | Allocated | Targets |
|----------------------------|-------------:|----------:|---------|
| Convert_NonBase_To_NonBase |    22.49 ns  |     144 B | A1 |
| Convert_SameUnit           |     7.13 ns  |      48 B | A1 |
| Convert_FromBase           |    22.73 ns  |     144 B | A1 |
| To_NonBase                 |    22.44 ns  |     144 B | A1 |
| Compare_Equal (`==`)       |    65.88 ns  |     288 B | A2 |
| Compare_GreaterThan (`>`)  |    68.07 ns  |     288 B | A2 |
| CompareTo                  |    67.88 ns  |     288 B | A2 |
| Compare_Negative           |    56.62 ns  |     288 B | A2 (NaN-tolerance bug) |
| Add (`+`)                  |    22.92 ns  |     144 B | A1/A2 |
| TryParse_Success           |    45.85 ns  |      56 B | A3 |
| TryParse_UnknownUnit       | 4,894.03 ns  |     648 B | A3 (exception path) |
| GetUnitNames               | 9,881.29 ns  |   4,720 B | A4 |

## Observations to target
- **Every conversion allocates** (48–144 B) — the enum boxing in `Convert` (A1).
  Comparisons allocate 288 B because each does 2× `In()` (2 conversions) plus more
  boxed `CompareTo`.
- **`TryParse_UnknownUnit` is ~107× slower than success** (4894 ns vs 46 ns) — the
  thrown/caught `ArgumentException` (A3).
- **`GetUnitNames` is ~10 µs and 4.7 KB per call** — full reflection every call (A4).
- `Compare_Negative` runs the `eps()` path where `Math.Log10` of a negative → NaN,
  so tolerance is effectively disabled (A2 latent bug).

Re-run the same benchmark after each optimization and compare against this table.

## After A1–A4 + A6 (2026-07-22)

Same machine / runtime. All 284 tests still green.

| Method                     | Before        | After      | Time    | Alloc: before → after |
|----------------------------|--------------:|-----------:|--------:|-----------------------|
| Convert_NonBase_To_NonBase |    22.49 ns   |  2.05 ns   | ~11×    | 144 B → **0 B** |
| Convert_SameUnit           |     7.13 ns   |  0.03 ns   | ~230×   |  48 B → **0 B** |
| Convert_FromBase           |    22.73 ns   |  2.06 ns   | ~11×    | 144 B → **0 B** |
| To_NonBase                 |    22.44 ns   |  2.53 ns   | ~9×     | 144 B → **0 B** |
| Compare_Equal (`==`)       |    65.88 ns   |  5.80 ns   | ~11×    | 288 B → **0 B** |
| Compare_GreaterThan (`>`)  |    68.07 ns   |  5.83 ns   | ~12×    | 288 B → **0 B** |
| CompareTo                  |    67.88 ns   |  5.61 ns   | ~12×    | 288 B → **0 B** |
| Compare_Negative           |    56.62 ns   |  5.39 ns   | ~10.5×  | 288 B → **0 B** |
| Add (`+`)                  |    22.92 ns   |  2.91 ns   | ~8×     | 144 B → **0 B** |
| TryParse_Success           |    45.85 ns   | 43.24 ns   | ~1.06×  |  56 B → 56 B |
| TryParse_UnknownUnit       | 4,894.03 ns   | 15.32 ns   | **~319×** | 648 B → 32 B |
| GetUnitNames               | 9,881.29 ns   | 28.47 ns   | **~347×** | 4,720 B → 128 B |

### Takeaways
- **A1** eliminated all conversion/comparison allocations (the enum boxing) — every hot-path
  op is now 0 B, and conversions/comparisons are ~8–12× faster.
- **A3** turned the exception-driven `TryParse` failure from ~4.9 µs into ~15 ns (~319×).
  Follow-up (v1.1.18): `TryParseInternal` now slices with `ReadOnlySpan<char>` and looks
  the unit up via an ordinal span scan over the small unit set, so both the success and
  failure paths allocate **0 B** (was 56 B on success) — verified with
  `GC.GetAllocatedBytesForCurrentThread`.
- **A4** cut `GetUnitNames` from ~9.9 µs / 4.7 KB to ~28 ns / 128 B (just the defensive clone).
- **A2** is folded into the comparison numbers above; `Compare_Negative` now takes the
  real tolerance path instead of the NaN-disabled one.


---

## After Round 2 (2026-07-27)

Same machine, re-measured with the committed `Gehtsoft.Measurements.Benchmark` project
(BenchmarkDotNet 0.14.0, `[MemoryDiagnoser]`, .NET 8.0.29, Release, WSL2, Ryzen 9 9950X).
All 527 tests green.

| Method                     | Before (1.1.17) | After A1–A6 | After Round 2 | Allocated |
|----------------------------|----------------:|------------:|--------------:|----------:|
| Convert_NonBase_To_NonBase |      22.49 ns   |    2.05 ns  |     1.92 ns   | **0 B** |
| Convert_SameUnit           |       7.13 ns   |    0.03 ns  |     0.05 ns   | **0 B** |
| Convert_FromBase           |      22.73 ns   |    2.06 ns  |     1.90 ns   | **0 B** |
| To_NonBase                 |      22.44 ns   |    2.53 ns  |     2.46 ns   | **0 B** |
| Compare_Equal (`==`)       |      65.88 ns   |    5.80 ns  |     5.31 ns   | **0 B** |
| Compare_GreaterThan (`>`)  |      68.07 ns   |    5.83 ns  |     5.29 ns   | **0 B** |
| CompareTo                  |      67.88 ns   |    5.61 ns  |     5.14 ns   | **0 B** |
| Compare_Negative           |      56.62 ns   |    5.39 ns  |     5.08 ns   | **0 B** |
| Add (`+`)                  |      22.92 ns   |    2.91 ns  |     2.65 ns   | **0 B** |
| TryParse_Success           |      45.85 ns   |   43.24 ns  |    32.05 ns   | **0 B** |
| TryParse_UnknownUnit       |   4,894.03 ns   |   15.32 ns  |    14.28 ns   | **0 B** |
| GetUnitNames               |   9,881.29 ns   |   28.47 ns  |    27.57 ns   | 176 B |

New rows for the API added in this round:

| Method                    | Mean      | Allocated | Note |
|---------------------------|----------:|----------:|------|
| TryParse_Span             | 32.34 ns  | **0 B**   | parsing out of a caller buffer, no substring |
| TryParse_Exponent         | 35.71 ns  | **0 B**   | `"1.05e1ft"`, which did not parse at all before |
| TryFormat                 | 45.66 ns  | **0 B**   | formatting into a caller buffer |
| TryFormat_DefaultAccuracy | 52.60 ns  | **0 B**   | the `"ND"` format |
| ToString_NF               | 54.22 ns  | 40 B      | one allocation: the returned string |
| ToString_DefaultAccuracy  | 60.49 ns  | 40 B      | the `"ND"` format string is no longer built per call |
| Interpolation (`$"{m}"`)  | 66.68 ns  | 40 B      | goes through `TryFormat` now that `ISpanFormattable` is implemented |

### Takeaways
- **Formatting into a caller buffer is now allocation-free**, which was the last allocating hot
  path. `ToString` still allocates the string it returns, but only that one — it used to build
  an intermediate value string, and for `"ND"` a format string, as well.
- **Parsing got faster while gaining exponent support.** The suffix-first parser (`32.05 ns`
  against `43.24 ns`) beats the old left-to-right scanner despite doing strictly more.
- The failure path is `14.28 ns` — a first pass comparing only the last character of each unit
  name keeps a text ending in no known unit down to one character compare per unit. Without
  that guard the rewrite measured `25.97 ns`, slower than the `15.32 ns` it replaced.
- `GetUnitNames` allocates 176 B rather than the 128 B recorded above only because
  `DistanceUnit` gained units in 1.1.17; it is the same defensive clone.
- Conversion, comparison and arithmetic are unchanged within noise, as expected — this round
  did not touch them. They are now covered by `OperationAllocationTest` so the 0 B cannot
  silently regress.
