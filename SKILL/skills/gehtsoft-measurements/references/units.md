# The shipped unit enumerations

Every unit of all 17 enumerations the library ships, generated from the assembly so the member
names and the parse names are exactly right. Look a member up here rather than guessing it —
several are not what you would expect.

Reading the tables:

- **Member** is the C# enumeration member you write in code.
- **Name** is what formatting produces and what parsing accepts.
- **Alternative** also parses but is never produced by formatting.
- **Accuracy** is the number of decimal places the `"ND"` format uses.
- **base** marks the unit every conversion in that enumeration routes through. It is chosen per
  domain and is often not the SI unit — `DistanceUnit` is based on the inch, `WeightUnit` on the
  grain, `TemperatureUnit` on Fahrenheit, `AccelerationUnit` on the gal. This matters when you
  add a unit, because your `Conversion` attribute must reach *that* unit.
- **obsolete** members still convert and format, but are excluded from `GetUnitNames()` and
  cannot be parsed. They exist so that correcting a name did not break callers. Each has a
  correctly named replacement carrying the same unit name and factor; use the replacement.

One name worth a warning, kept as it is for compatibility rather than because it is right:
`WeightUnit.Newton` is a force expressed as a mass equivalent. A proper `ForceUnit` also exists,
and that is usually what you want.

## AccelerationUnit

Base unit: `AccelerationUnit.Gal`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Gal` | `gal` | `cm/s²` | 6 | **base** |
| `FeetPerSecondSquare` | `ft/s²` | `ft/s2` | 3 |  |
| `MeterPerSecondSquare` | `m/s²` | `m/s2` | 3 |  |
| `EarthGravity` | `g0` |  | 3 |  |
| `InchesPerSecondSquare` | `in/s²` | `in/s2` | 3 |  |

## AngularUnit

Base unit: `AngularUnit.Radian`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Radian` | `rad` |  | 6 | **base** |
| `Degree` | `°` | `deg` | 4 |  |
| `MOA` | `moa` |  | 2 |  |
| `Mil` | `mil` |  | 2 |  |
| `MRad` | `mrad` |  | 2 |  |
| `Thousand` | `ths` |  | 2 |  |
| `InchesPer100Yards` | `in/100yd` |  | 2 |  |
| `CmPer100Meters` | `cm/100m` |  | 2 |  |
| `Percent` | `%` | `percent` | 0 |  |
| `Turn` | `turn` |  | 0 |  |
| `Gradian` | `gon` | `ᵍ` | 0 |  |
| `ArcSecond` | `arcsec` |  | 1 |  |

## AreaUnit

Base unit: `AreaUnit.SquareMillimeter`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `SquareMillimeter` | `mm²` | `mm2` | 0 | **base** |
| `SquareCentimeter` | `cm²` | `cm2` | 0 |  |
| `SquareMeter` | `m²` | `m2` | 0 |  |
| `SquareKilometer` | `km²` | `km2` | 0 |  |
| `SquareInch` | `in²` | `in2` | 0 |  |
| `SquareFoot` | `ft²` | `ft2` | 0 |  |
| `SquareYard` | `yd²` | `yd2` | 0 |  |
| `SquareMile` | `mi²` | `mi2` | 0 |  |
| `Acre` | `ac` |  | 0 |  |
| `Hectare` | `ha` |  | 0 |  |
| `Ar` | `ar` |  | 0 |  |

## DensityUnit

Base unit: `DensityUnit.KilogramPerCubicMeter`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `GramPerCubicCentimeter` | `g/cm³` | `g/cm3` | 0 |  |
| `KilogramPerCubicMeter` | `kg/m³` | `kg/m3` | 3 | **base** |
| `PoundsPerCubicInch` | `lb/in³` | `lb/in3` | 0 |  |
| `OuncesPerCubicFeet` | `oz/in³` | `oz/in3` | 0 | obsolete |
| `PoundsPerCubicFoot` | `lb/ft³` | `lb/ft3` | 2 |  |
| `GrainsPerCubicInch` | `gr/in³` | `gr/in3` | 3 |  |
| `KilogramPerLiter` | `kg/l` |  | 3 |  |
| `PoundsPerGallon` | `lb/gal` |  | 2 |  |
| `OuncesPerCubicInch` | `oz/in³` | `oz/in3` | 0 |  |

## DistanceUnit

Base unit: `DistanceUnit.Inch`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Line` | `ln` | `'''` | 1 |  |
| `RussianLine` | `rln` |  | 1 |  |
| `Inch` | `in` | `"` | 1 | **base** |
| `Foot` | `ft` | `'` | 2 |  |
| `Yard` | `yd` |  | 2 |  |
| `Mile` | `mi` |  | 3 |  |
| `NauticalMile` | `nm` |  | 3 |  |
| `Millimeter` | `mm` |  | 0 |  |
| `Centimeter` | `cm` |  | 1 |  |
| `Meter` | `m` |  | 1 |  |
| `Kilometer` | `km` |  | 3 |  |
| `Point` | `pt` |  | 1 |  |
| `Pica` | `p` |  | 1 |  |
| `Micrometer` | `µm` |  | 0 |  |
| `Decimeter` | `dm` |  | 2 |  |
| `Thou` | `thou` |  | 0 |  |
| `Furlong` | `fur` |  | 3 |  |
| `Fathom` | `ftm` |  | 2 |  |
| `Hand` | `hh` |  | 1 |  |

## EnergyUnit

Base unit: `EnergyUnit.Joule`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `FootPound` | `ft·lb` | `ft-lb` | 0 |  |
| `Joule` | `J` |  | 0 | **base** |
| `BTU` | `BTU` |  | 0 |  |
| `HpH` | `hp·h` | `hp-h` | 0 |  |
| `Wh` | `w·h` | `wh` | 0 |  |
| `kWh` | `kw·h` | `kwh` | 0 |  |
| `Kilojoule` | `kJ` |  | 0 |  |
| `Calorie` | `cal` |  | 0 |  |
| `Kilocalorie` | `kcal` |  | 0 |  |
| `Erg` | `erg` |  | 0 |  |

## ForceUnit

Base unit: `ForceUnit.Newton`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Newton` | `N` | `kg·m/s²` | 3 | **base** |
| `Dyne` | `dyn` |  | 3 |  |
| `KilogramForce` | `kp` |  | 3 |  |
| `PoundForce` | `lbf` |  | 3 |  |
| `Poundal` | `pdl` |  | 3 |  |

## GasConsumptionUnit

Base unit: `GasConsumptionUnit.LiterPerKm`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `LiterPerKm` | `l/km` |  | 5 | **base** |
| `LiterPer100Km` | `l/100km` |  | 1 |  |
| `MilesPerGallon` | `mpg` | `mi/gal` | 1 |  |
| `KilometersPerLiter` | `km/l` |  | 1 |  |
| `ImperialMilesPerGallon` | `imp.mpg` | `imp.mi/gal` | 1 |  |

## PowerUnit

Base unit: `PowerUnit.Watt`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Watt` | `w` |  | 1 | **base** |
| `MetricHoursePower` | `ps` |  | 1 | obsolete |
| `MechanicalHoursePower` | `hp` |  | 1 | obsolete |
| `FootPound` | `ft⋅lbf` | `ft-lbf` | 1 |  |
| `MetricHorsePower` | `ps` |  | 1 |  |
| `MechanicalHorsePower` | `hp` |  | 1 |  |
| `Kilowatt` | `kw` |  | 1 |  |
| `Megawatt` | `Mw` |  | 3 |  |
| `BTUPerHour` | `BTU/h` |  | 1 |  |

## PressureUnit

Base unit: `PressureUnit.Pascal`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Pascal` | `pa` |  | 0 | **base** |
| `KiloPascal` | `kPa` |  | 1 |  |
| `Bar` | `bar` |  | 3 |  |
| `Millibar` | `mbar` |  | 1 |  |
| `Atmosphere` | `atm` |  | 3 |  |
| `TechincalAtmosphere` | `at` |  | 3 | obsolete |
| `MillimetersOfMercury` | `mmHg` |  | 1 |  |
| `InchesOfMercury` | `inHg` |  | 2 |  |
| `PoundsPerSquareInch` | `psi` | `lbf/in2` | 1 |  |
| `MillimetersOfWater` | `mmH2O` |  | 2 |  |
| `TechnicalAtmosphere` | `at` |  | 3 |  |
| `Hectopascal` | `hPa` |  | 1 |  |
| `Megapascal` | `MPa` |  | 3 |  |
| `Torr` | `torr` |  | 2 |  |
| `InchesOfWater` | `inH2O` |  | 2 |  |
| `PoundsPerSquareFoot` | `psf` | `lbf/ft2` | 2 |  |

## RotationalSpeedUnit

Base unit: `RotationalSpeedUnit.RadianPerSecond`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `RadianPerSecond` | `rad/s` |  | 3 | **base** |
| `RevolutionsPerMinute` | `rpm` |  | 1 |  |
| `Hertz` | `Hz` |  | 3 |  |

## SolidAngularUnit

Base unit: `SolidAngularUnit.Steradian`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Steradian` | `sr` |  | 6 | **base** |
| `SquareDegree` | `deg2` | `sqdeg` | 6 |  |
| `SquareMinute` | `moa2` | `sqmoa` | 6 |  |

## TemperatureUnit

Base unit: `TemperatureUnit.Fahrenheit`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Fahrenheit` | `°F` | `F` | 1 | **base** |
| `Celsius` | `°C` | `C` | 1 |  |
| `Kelvin` | `°K` | `K` | 1 |  |
| `Rankin` | `°R` | `R` | 1 |  |
| `Reaumur` | `°Re` | `Re` | 1 |  |
| `Delisle` | `°De` | `De` | 1 |  |

## TorqueUnit

Base unit: `TorqueUnit.NewtonMeter`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `NewtonMeter` | `N·m` | `N-m` | 3 | **base** |
| `KilogramForceMeter` | `kgf·m` | `kgf-m` | 3 |  |
| `FootPoundForce` | `ft·lbf` | `ft-lbf` | 3 |  |
| `InchPoundForce` | `in·lbf` | `in-lbf` | 3 |  |

## VelocityUnit

Base unit: `VelocityUnit.MetersPerSecond`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `MetersPerSecond` | `m/s` | `mps` | 0 | **base** |
| `KilometersPerHour` | `km/h` | `kmph` | 1 |  |
| `FeetPerSecond` | `ft/s` | `fps` | 1 |  |
| `MilesPerHour` | `mi/h` | `mph` | 1 |  |
| `Knot` | `kt` |  | 1 |  |
| `InchesPerSecond` | `in/s` |  | 1 |  |
| `CentimetersPerSecond` | `cm/s` |  | 1 |  |
| `FeetPerMinute` | `ft/min` |  | 1 |  |

## VolumeUnit

Base unit: `VolumeUnit.Milliliter`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Milliliter` | `ml` |  | 1 | **base** |
| `Liter` | `l` |  | 3 |  |
| `CubicMeter` | `m³` | `m3` | 6 |  |
| `CubicInch` | `in³` | `in3` | 6 |  |
| `CubicFeet` | `ft³` | `ft3` | 6 |  |
| `CubicYard` | `yd³` | `yd3` | 6 |  |
| `ImperialPint` | `imp.pt` |  | 1 |  |
| `ImperialQuart` | `imp.qt` |  | 1 |  |
| `ImperialGallon` | `imp.gal` |  | 1 |  |
| `Ounce` | `oz` |  | 1 |  |
| `Pint` | `pt` |  | 1 |  |
| `Quart` | `qt` |  | 1 |  |
| `Gallon` | `gal` |  | 1 |  |
| `CubicCentimeter` | `cc` |  | 1 |  |
| `ImperialFluidOunce` | `imp.oz` |  | 1 |  |
| `OilBarrel` | `bbl` |  | 1 |  |
| `Teaspoon` | `tsp` |  | 1 |  |
| `Tablespoon` | `tbsp` |  | 1 |  |
| `Cup` | `cup` |  | 1 |  |

## WeightUnit

Base unit: `WeightUnit.Grain`

| Member | Name | Alternative | Accuracy | |
|---|---|---|---|---|
| `Grain` | `gr` |  | 0 | **base** |
| `Ounce` | `oz` |  | 1 |  |
| `Gram` | `g` |  | 1 |  |
| `Pound` | `lb` |  | 3 |  |
| `Kilogram` | `kg` |  | 3 |  |
| `Neuton` | `N` |  | 3 | obsolete |
| `Dram` | `dr` |  | 1 |  |
| `TroyOz` | `tr.oz` |  | 1 |  |
| `Tonne` | `t` |  | 3 |  |
| `USTonne` | `us.t` |  | 3 |  |
| `UKTonne` | `uk.t` |  | 3 |  |
| `Newton` | `N` |  | 3 |  |
| `Milligram` | `mg` |  | 0 |  |
| `Stone` | `st` |  | 2 |  |
| `Carat` | `ct` |  | 2 |  |
| `Slug` | `slug` |  | 3 |  |

