@echo off
cd %~dp0
"PackageBuilder/PackageBuilder.exe" -b "Base/Base Package.package" -s "Sources/ExampleSTBLFile.English.stbl" -t "Output/Package.package"