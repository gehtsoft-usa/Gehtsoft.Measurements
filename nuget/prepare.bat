cd ..
nuget restore Gehtsoft.Measurements.sln 
msbuild Gehtsoft.Measurements.sln -p:Configuration="Release"
cd nuget
msbuild nuget.proj -t:Prepare
msbuild nuget.proj -t:NuSpec,NuPack
