# Conversion operations in detail

You declare how **your unit reaches the base unit**. The library derives the opposite direction
by inverting your operations, which is why you never write a reverse conversion and why the two
directions cannot disagree.

## Contents

- [The operation set](#the-operation-set)
- [Chaining two operations](#chaining-two-operations)
- [Affine units, worked](#affine-units-worked)
- [Trigonometric units, worked](#trigonometric-units-worked)
- [Custom conversions](#custom-conversions)
- [Choosing factors](#choosing-factors)

## The operation set

`v` is the value in your unit, `b` the value in the base unit, `f` the factor you declare.

| Operation | To base | From base | Notes |
|---|---|---|---|
| `Base` | `b = v` | `v = b` | Exactly one member per enumeration. Takes no factor. |
| `Add` | `b = v + f` | `v = b - f` | An offset. |
| `Subtract` | `b = v - f` | `v = b + f` | An offset the other way. |
| `SubtractFromFactor` | `b = f - v` | `v = f - b` | Its own inverse. For a scale that runs backwards. |
| `Multiply` | `b = v * f` | `v = b / f` | The common case. `f` is how many base units one of yours is worth. |
| `Divide` | `b = v / f` | `v = b * f` | `f` is how many of yours make one base unit. |
| `DivideFactor` | `b = f / v` | `v = f / b` | Its own inverse. For reciprocal quantities such as fuel consumption. |
| `Negate` | `b = -v` | `v = -b` | Takes no factor. |
| `Tan` | `b = tan(v)` | `v = atan(b)` | Your unit is an angle, the base is a slope. |
| `Atan` | `b = atan(v)` | `v = tan(b)` | Your unit is a slope, the base is an angle. |
| `Custom` | `b = op.ToBase(v)` | `v = op.FromBase(b)` | You write both directions. |
| `None` | — | — | Only as a second operation, meaning "there isn't one". |

`Multiply` and `Divide` express the same relationship from opposite ends; pick whichever lets you
write the exact number. Prefer `Multiply 0.3048` over `Divide 3.28084` for feet-to-metres,
because 0.3048 is exact and 3.28084 is a rounded reciprocal.

## Chaining two operations

The four-argument attribute applies two operations **in the order written** on the way to the
base unit:

```csharp
[Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Add, 32)]
```

To base: multiply, then add. From base: the inverse of the second, then the inverse of the
first — subtract, then divide. You get the correct order for free.

Two operations are the maximum. If a conversion needs more, use `Custom`.

## Affine units, worked

Temperature is the reason two operations exist. `TemperatureUnit` is based on Fahrenheit:

```csharp
[Unit("°C", "C", 1)]
[Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Add, 32)]
Celsius,
```

To Fahrenheit: `f = c * 1.8 + 32`. Back: `c = (f - 32) / 1.8`. Check it: 100 °C gives
`100 * 1.8 + 32 = 212` °F.

Kelvin reaches the same base with a subtraction instead:

```csharp
[Conversion(ConversionOperation.Multiply, 1.8, ConversionOperation.Subtract, 459.67)]
```

Delisle runs backwards, which a negative factor handles:

```csharp
[Conversion(ConversionOperation.Multiply, -1.2, ConversionOperation.Add, 212)]
```

**What affine units cost you.** A unit whose conversion includes an offset is not linear, so
operations that assume linearity stop meaning anything even though they still compute:

- `Measurement<TemperatureUnit>.ZERO` is zero of the *base* unit, 0 °F. Adding it to 50 °C gives
  32.22 °C, so it is not an additive identity. This is why the library does not implement
  `IAdditiveIdentity`.
- Adding two temperatures, or summing and averaging them, is defined but physically meaningless.
  If you want a mean temperature, convert to one unit and average the bare numbers.
- Multiplying a temperature by a scalar scales the number in whichever unit it happens to be
  expressed in, which is almost certainly not what you want.

Comparison and conversion are fine — those are the operations affine units genuinely support.

## Trigonometric units, worked

`AngularUnit` is based on the radian, and several of its units are slopes rather than angles: a
rise over a run. The slope reaches an angle through an arc tangent:

```csharp
[Unit("in/100yd", 2)]
[Conversion(ConversionOperation.Divide, 3600, ConversionOperation.Atan, 0)]
InchesPer100Yards,
```

100 yards is 3600 inches, so `Divide 3600` turns inches-per-100-yards into a bare slope, and
`Atan` turns the slope into radians. Backwards, the library takes the tangent and multiplies.
`cm/100m` (divide by 10000) and `%` (divide by 100) work the same way.

Note the second factor is written as `0` because `Atan` ignores it. Passing a non-zero factor to
an operation that ignores it is legal but almost always a mistake, so the validator warns (GM010).

**The one thing to know**: the arc tangent only undoes the tangent between minus and plus a
quarter turn. A unit declared with a bare `Tan` will therefore not round-trip for values outside
that range — not a defect, just what the tangent is. Scale first, as the units above do, so the
tangent's argument stays small.

## Custom conversions

When one or two arithmetic operations cannot express the relationship, implement an interface and
name the type in the attribute:

```csharp
public class MyConversion : ICustomConversionOperation
{
    public double ToBase(double value) => (2 / value) - 1;
    public double FromBase(double value) => 2 / (value + 1);
}
```

```csharp
[Unit("mine", 3)]
[Conversion(ConversionOperation.Custom, "MyNamespace.MyConversion")]
Mine,
```

The name is the full name including the namespace. For a nested class use the `+` separator, as
reflection does: `"MyNamespace.Outer+MyConversion"`. The type is located by scanning the loaded
assemblies and instantiated with its parameterless constructor.

**Implement `ICustomConversionOperation2` instead if the unit will be used with
`DecimalMeasurement`.** It adds `ToBaseDecimal` and `FromBaseDecimal`, so a decimal measurement
converts without going through a `double` and losing accuracy. With only the `double` interface
the conversion still works, and the validator warns about the accuracy loss (GM013):

```csharp
public class MyConversion : ICustomConversionOperation2
{
    public double ToBase(double value) => (2 / value) - 1;
    public double FromBase(double value) => 2 / (value + 1);
    public decimal ToBaseDecimal(decimal value) => (2m / value) - 1m;
    public decimal FromBaseDecimal(decimal value) => 2m / (value + 1m);
}
```

**This is the one place a conversion can be wrong in a way the library cannot catch for you.**
Everywhere else the reverse is derived; here you write both directions and nothing checks that
they undo each other. Run `UnitEnumValidator` — its round-trip check (GM015) exists for exactly
this, and it will tell you if `FromBase` is not the inverse of `ToBase`.

Two more consequences of the type being found by name at run time:

- A typo in the name is not a compile error. It throws when the attribute is first read.
- Trimming can remove the type, because nothing references it statically. The attribute
  constructor is annotated `RequiresUnreferencedCode`, so you get a warning in a trimmed
  application; preserve the type explicitly if you need it there.

## Choosing factors

- **Write exact values where they exist.** An international foot is exactly 0.3048 m; a pound is
  exactly 453.59237 g. Rounded reciprocals accumulate error in both directions.
- **Let the compiler do the arithmetic** rather than pre-computing: `1.3558179483314 / 12` reads
  as "one twelfth of a foot-pound" and cannot be mistyped as easily as its decimal expansion.
- **A factor of zero for `Multiply`, `Divide` or `DivideFactor` is an error** — the value collapses
  and cannot be recovered. The validator reports it (GM009).
- **Watch the base unit.** Your factor must reach the base unit of *that* enumeration, which is
  frequently not SI. See `units.md` for which member is the base.
