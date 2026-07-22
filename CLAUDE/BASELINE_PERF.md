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
  The success path is unchanged in allocation (the 56 B are the two `Substring` calls —
  a future A5-style `ReadOnlySpan<char>` rewrite would remove those).
- **A4** cut `GetUnitNames` from ~9.9 µs / 4.7 KB to ~28 ns / 128 B (just the defensive clone).
- **A2** is folded into the comparison numbers above; `Compare_Negative` now takes the
  real tolerance path instead of the NaN-disabled one.

