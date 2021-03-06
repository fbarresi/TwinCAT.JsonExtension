<img align="right" height="120" src="https://raw.githubusercontent.com/fbarresi/TwinCAT.JsonExtension/master/doc/images/logo.jpg">

# TwinCAT.JsonExtension
TwinCAT variables to and from json 

[![Build status](https://ci.appveyor.com/api/projects/status/4ggo35buwmno05u2/branch/master?svg=true)](https://ci.appveyor.com/project/fbarresi/twincat-jsonextension/branch/master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/6286aa6bb6f2402fa4f7553d749a5a8a)](https://www.codacy.com/manual/fbarresi/TwinCAT.JsonExtension?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=fbarresi/TwinCAT.JsonExtension&amp;utm_campaign=Badge_Grade)
[![codecov](https://codecov.io/gh/fbarresi/TwinCAT.JsonExtension/branch/master/graph/badge.svg)](https://codecov.io/gh/fbarresi/TwinCAT.JsonExtension)
![Licence](https://img.shields.io/github/license/fbarresi/twincat.jsonextension.svg)
[![Nuget Version](https://img.shields.io/nuget/v/TwinCAT.JsonExtension.svg)](https://www.nuget.org/packages/TwinCAT.JsonExtension/)

Bring the power of Json.Net to TwinCAT

Tranform DUTs decorated with the _custom_ **Json-Attribute** like this:

```reStructuredText
TYPE JsonDUT :
STRUCT
	{attribute 'json' := 'message'}
	sMessage : STRING := 'test';
	iResponse : INT;
	{attribute 'json' := 'status'}
	sStatus : STRING := 'success';
	{attribute 'json' := 'numbers'}
	daNumbers : ARRAY[1..3] OF DINT := [1,2,3];
END_STRUCT
END_TYPE
```

into this (and back) **recursively** and absolutely **type-independent**:

```javascript
{
  "message": "test",
  "status" : "success",
  "numbers" : [1,2,3]
}
```

only calling this two extension methods on your connected `TcAdsClient`:
```csharp
var json = await client.ReadJson("GVL.JsonDutVariable")
```

```csharp
await client.WriteJson("GVL.JsonDutVariable", json);
```

Have fun using this simple package and don't forget to **star this project**!

## Referenced projects

Whould you like to see the power of **TwinCAT.JsonExtension** in action?

Then checkout [BeckhoffHttpClient](https://github.com/fbarresi/BeckhoffHttpClient), an _unofficial_ TwinCAT function for HTTP requests

or

[TwincatAdsTool](https://github.com/fbarresi/TwincatAdsTool) your swiss knife for twincat development.

## Credits

Special thanks to [JetBrains](https://www.jetbrains.com/?from=TwinCAT.JsonExtension) for supporting this open source project.

<a href="https://www.jetbrains.com/?from=TwinCAT.JsonExtension"><img height="200" src="https://www.jetbrains.com/company/brand/img/jetbrains_logo.png"></a>
