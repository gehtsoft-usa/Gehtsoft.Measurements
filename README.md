# Status

![unit tests](https://github.com/gehtsoft-usa/Gehtsoft.Measurements/actions/workflows/test.yml/badge.svg)
![documentation](https://github.com/gehtsoft-usa/Gehtsoft.Measurements/actions/workflows/doc.yml/badge.svg)

# Gehtsoft.Measurements

The C# library to manipulate and convert with various measurements/units (e.g. distances, weight, angles, temperatures, and so on)
expressed in various units (e.g. distances in inches, yards, meters).

The library may be useful for calculations, for example, in math, physic or GIS that does not depend from the system of units used (e.g. SI/Metric or Imperial) or to create a unit convertor application.

Currently, distance/length, velocity, weight, angular measurements, and energy units are supported.

The library is shared under LGPL license.

To use the last stable version of the library in your project please use the package on the nuget
https://www.nuget.org/packages/Gehtsoft.Measurements

## Using Library

The core class of the library is the generic structure `Measurement`.
The structure accepts an enumeration as a parameter and this enumeration
defines the measurement unit to be used:

```csharp
 var v = new Measurement<DistanceUnit>(10, DistanceUnit.Feet);
```

You can then manipulate this value using C# operator, format them or
convert it into another unit:

```csharp
 var v1 = v * 2;
 string v = v.ToString("N3");
 var v2 = v1.To(DistanceUnit.Meter);
```

or

```csharp
    var x = (10.As(DistanceUnit.Yard) + 36.As(DistanceUnit.Inch)).To(DistanceUnit.Meter);
```

The class fully supports serialization using `System.Text.Json`
and `Binaron.Serializer` (see https://github.com/zachsaw/Binaron.Serializer).
`XmlSerializer` cannot be implemented for a readonly structures without
introducing of non-safe code. Please refer to tests for an example
how to implement an XML serialization
(https://github.com/gehtsoft-usa/Gehtsoft.Measurements/blob/76fc639a657186dc91615839ca9ded4c14af7bc2/Gehtsoft.Measurements.Test/CoreClassesTest.cs#L181).

Read more on http://docs.gehtsoftusa.com/Gehtsoft.Measurements/web-content.html#index.html

## Defining your own units

You can define your own measurment units by
creating a enumeration and mark it using `Unit` and `Conversion`
attributes. The first attribute defines the unit name and the
default accuracy of the values. The second attribute defines the
rules of the conversion. One unit must always be a "base" unit, and
conversion rules for other units defines how to convert the
specified unit into a base unit.

```csharp
enum MyWeightUnit
{
    //1 gram
    [Unit("g", 3)]
    [Conversion(ConversionOperation.Base)]
    Gram,

   //1 kilogram (1 kilogram = 1000 gram)
    [Unit("kg", 3)]
    [Conversion(ConversionOperation.Multiply, 1000)]
    Kilogram,
}
```
