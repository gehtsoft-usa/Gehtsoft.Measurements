# Gehtsoft.Measurements

The C# library to manipulate with various measurements (e.g. distances, weight, angles, temperatures, and so on) expressed in various units (e.g. distances in inches, yards, meters)

Currently, distance/length, velocity, weight, angular measurements, and energy units are supported.

The library is shared under LGPL license. 

## Using Library

The core class of the library is the generic structure `Measurement`. The structure accepts an enumeration as a parameter and this enumeration defines the measurement unit to be used:

```csharp
Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(10, DistanceUnit.Feet);
```

You can then manipulate this value using C# operator, format them or convert it into another unit:

```csharp
 var v1 = v * 2;
 string v = v.ToString("N3");
 var v2 = v1.To(DistanceUnit.Meter);
```

The class fully supports serialization using `System.Text.Json`, `System.Xml.Serialization.XmlSerializer` and `Binaron.Serializer` (see https://github.com/zachsaw/Binaron.Serializer).

Read more on http://docs.gehtsoftusa.com/Gehtsoft.Measurements/web-content.html#index.html

## Defining your own units

You can define your own measurment units by creating a enumeration and mark it using `Unit` and `Conversion`  attributes. The first attribute defines the unit name and the default accuracy of the values. The second attribute defines the rules of the conversion. One unit must always be a "base" unit, and conversion rules for other units defines how to convert the specified unit into a base unit. 

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
