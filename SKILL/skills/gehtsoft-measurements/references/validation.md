# Validator findings, GM001 to GM017

`UnitEnumValidator.Validate<T>()` returns a `UnitValidationReport`. Each finding carries a stable
code, a severity, the offending member, and a message. The codes never change; the wording may.

```csharp
var report = UnitEnumValidator.Validate<MyWeightUnit>();

report.HasErrors            // anything that makes the enumeration unusable
report.IsValid              // the opposite; warnings do not count
report.Findings             // everything, in declaration order
report.Errors()             // just the errors
report.Warnings()           // just the warnings
report.ThrowIfInvalid()     // throws InvalidOperationException listing everything
report.ToString()           // the whole report as text, one finding per line
```

**Errors** mean the enumeration cannot be used or produces wrong numbers. **Warnings** are legal
but suspicious — worth a look, not worth failing a build over on their own.

## How the validator works

It runs in two phases, and the order matters if you are ever surprised by a short report:

1. **Structural and declaration checks** (GM001–GM013) by reflection alone.
2. **Behavioural checks** (GM014–GM017), which actually perform conversions, **only if phase one
   found no errors.**

Phase two is skipped after a structural error because performing a conversion means touching
`Measurement<T>`, whose static initializer raises its own validation — that would replace your
report with an exception and leave the closed type unusable for the rest of the process. So a
report containing a single GM004 is complete, not truncated.

## Structural — the enumeration cannot be used as declared

| Code | Severity | Meaning and fix |
|---|---|---|
| GM001 | Error | The type is not an enumeration, or declares no unit at all. Add members with `[Unit]` and `[Conversion]`. |
| GM002 | Error | A member has no `[Unit]`. Every member needs one — a bare member is not an alias, it breaks the generated switch. Add the attribute, or remove the member. |
| GM003 | Error | A member has no `[Conversion]`. Same as above. |
| GM004 | Error | No member is marked `ConversionOperation.Base`. Nothing can be converted, because everything routes through the base. Mark exactly one. |
| GM005 | Error | More than one member is marked `Base`. Only one can be; the others need a real conversion. |
| GM006 | Error | A unit name is empty or whitespace. The name is what parsing accepts and formatting produces, so it has to exist. |
| GM007 | Error | Two units share a parse name, primary or alternative. Only the first would ever be parsed; the other could be formatted but never read back. Rename one, or if one supersedes the other, mark the old one `[Obsolete]` — obsolete members are excluded from parsing and so may share a name. |

## Declaration — legal, but probably not what you meant

| Code | Severity | Meaning and fix |
|---|---|---|
| GM008 | Error | A factor is `NaN` or infinite. Usually a mistyped constant or a division that went wrong at compile time. |
| GM009 | Error | `Multiply`, `Divide` or `DivideFactor` has a factor of zero. The value collapses and cannot be converted back. |
| GM010 | Warning | A non-zero factor on an operation that ignores it — `Base`, `Negate`, `Tan`, `Atan`. The factor has no effect, so either you meant a different operation or the factor is stale. Write `0` when a second operation needs a placeholder. |
| GM011 | Warning | The default accuracy is outside 0 to 15. Numeric formats support that range; outside it the `"ND"` format falls off the fast path. Accuracy is *decimal places for display*, not significant digits. |
| GM012 | Warning | A unit name starts with a character a number can end with — a digit, a decimal or group separator, or a space. Because a name is matched at the end of the text, such a name can swallow the last characters of the value. Check whether GM017 also fired: if it did, the collision is real, not hypothetical. |
| GM013 | Warning | A `Custom` conversion whose type implements only `ICustomConversionOperation`. `DecimalMeasurement` conversions will route through a `double` and lose accuracy. Implement `ICustomConversionOperation2` if the unit is used with decimals. |

## Behavioural — the conversions were run and misbehaved

These are the checks worth the most, because nothing about reading the declaration reveals them.

| Code | Severity | Meaning and fix |
|---|---|---|
| GM014 | Error | The base unit does not convert to itself unchanged. The base unit must be the identity; check that its `[Conversion]` really is `ConversionOperation.Base`. |
| GM015 | Error | Converting a probe value to the base unit and back does not return it. **In practice this always means a custom conversion whose `FromBase` is not the inverse of its `ToBase`**, since every other reverse direction is derived automatically. Fix the custom conversion. |
| GM016 | Error | A probe value converted to a non-finite result — infinity or `NaN`. Typically an enormous factor overflowing, or a reciprocal operation hitting zero. |
| GM017 | Error | A value formatted in a unit does not parse back to the same unit and value. This is the empirical proof of GM012: some other unit's name matched the text first. Rename the unit. |

**Probe values** are a small fixed set covering positive, negative, small and large. Units whose
conversion takes a tangent on the way to the base get probes inside a quarter turn instead, since
outside that range the arc tangent legitimately does not undo the tangent and the unit would be
reported as broken for merely being periodic.

## Where to call it

**A test over your own unit types** is the highest-value placement. It costs nothing and converts
a class of silent wrong-number bugs into a failing build:

```csharp
[Theory]
[InlineData(typeof(MyWeightUnit))]
[InlineData(typeof(MyPressureUnit))]
public void UnitEnumerationsAreValid(Type unitType)
    => UnitEnumValidator.Validate(unitType).Findings.Should().BeEmpty();
```

Assert on `Findings` being empty rather than on `IsValid`, so a warning has to be looked at
rather than silently accumulating.

**Application start-up** suits units assembled from configuration or discovered by plug-in, where
a test cannot see them:

```csharp
UnitEnumValidator.Validate<MyWeightUnit>().ThrowIfInvalid();
```

**Which overload.** `Validate<T>()` is the one to prefer: it is trim-safe and AOT-safe.
`Validate(Type)` exists for code that discovers unit types by reflection; it closes a generic
method at run time, so it is annotated `RequiresDynamicCode` and `RequiresUnreferencedCode` and
will warn in a trimmed or ahead-of-time-compiled application.

**Do not call it on a hot path.** It performs conversions and formats and parses every name of
every unit. It is meant to run once, at start-up or in a test.
