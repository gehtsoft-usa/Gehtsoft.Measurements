# Gehtsoft.Measurements — Claude Code skill

A skill that teaches Claude how to use the
[Gehtsoft.Measurements](https://www.nuget.org/packages/Gehtsoft.Measurements) .NET library:
strongly typed measurements, unit conversion, measurement math, parsing and formatting,
serialization, and defining and validating your own units.

| | |
|---|---|
| API documentation | https://docs.gehtsoftusa.com/Gehtsoft.Measurements/ |
| NuGet package | https://www.nuget.org/packages/Gehtsoft.Measurements |
| Install this skill | [INSTALL.md](INSTALL.md) |

## What is in here

```
SKILL/
├── README.md                                  this file
├── INSTALL.md                                 how to install the skill
└── skills/
    └── gehtsoft-measurements/
        ├── SKILL.md                           the skill itself
        └── references/
            ├── units.md                       every shipped unit and its name
            ├── conversions.md                 the conversion operations in detail
            └── validation.md                  the validator codes GM001-GM017
```

`SKILL.md` is what Claude reads when the skill triggers. The files under `references/` are read
only when they are needed, so the everyday path stays small.

## What the skill covers

- **Units, conversion and storage** — `Measurement<TUnit>` and `DecimalMeasurement<TUnit>`,
  converting between units, comparing and storing values.
- **Math** — arithmetic and comparison operators, `MeasurementMath` (trigonometry, rounding,
  clamping) and the cross-dimension helpers such as power from torque and rotational speed.
- **Parsing and formatting** — the text form, cultures, the span API, and both JSON shapes plus
  the `XmlSerializer` pattern the library needs.
- **Defining your own conversions** — the `Unit` and `Conversion` attributes, the operation set,
  and custom conversion interfaces for anything arithmetic cannot express.
- **Validating your own conversions** — `UnitEnumValidator`, what each finding means, and where
  to call it.

## Why this skill exists

The library is small but has a handful of behaviours that are easy to get wrong and that no
amount of reading the method signatures reveals: the text constructor parses in the invariant
culture while `TryParse(string)` uses the current one, equality is tolerance-based rather than
exact, `XmlSerializer` cannot serialize the structure directly, and affine units such as
temperature make some otherwise reasonable operations meaningless. The skill front-loads those.

## Testing the skill

Test material lives outside version control on purpose — see `INSTALL.md`.

## License

Same as the library: LGPL.
