cd TwinCAT.JsonExtension.Tests
dotnet build TwinCAT.JsonExtension.Tests.csproj -c Debug -o testDlls
cd testDlls
OpenCover.Console.exe -register:user -target:"xunit.console.x86.exe" -targetargs:"TwinCAT.JsonExtension.Tests.dll -noshadow" -filter:"+[TwinCAT.JsonExtension*]* -[TwinCAT.JsonExtension.Tests*]*" -output:"coverage.xml" -oldstyle
codecov -f "coverage.xml" -t $env:CODECOV_TOKEN
