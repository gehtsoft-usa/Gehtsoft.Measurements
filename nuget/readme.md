# About

This package is a C# library to manipulate with various measurements (e.g. distances, weight, angles, temperatures, and so on) expressed in various units (e.g. distances in inches, yards, meters)
Currently, distance/length, velocity, weight, angular measurements, and energy units are supported.

The library is shared under LGPL license.

# Using Library

The core class of the library is the generic structure `Measurement`. The structure accepts an enumeration as a parameter and this enumeration defines the measurement unit to be used:

```cs
Measurement<DistanceUnit> v = new Measurement<DistanceUnit>(10, DistanceUnit.Feet);
```

You can then manipulate this value using C# operator, format them or convert it into another unit:

```cs
 var v1 = v * 2;
 string v = v.ToString("N3");
 var v2 = v1.To(DistanceUnit.Meter);
```

Read more on https://docs.gehtsoftusa.com/Gehtsoft.Measurements/index.html#main.html