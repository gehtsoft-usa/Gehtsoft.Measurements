@echo off
rem The Scan target runs Asm2Xml over bin/Release/net8.0, so the library must be
rem built in Release first. Enforce it here so the extracted docs never come from
rem a stale or Debug assembly.
dotnet build "..\Gehtsoft.Measurements\Gehtsoft.Measurements.csproj" -c Release || exit /b 1
dotnet build project.proj /t:Scan,Raw
