<img align="right" height="120" src="https://raw.githubusercontent.com/fbarresi/TwinCAT.JsonExtension/master/doc/images/logo.jpg">

# TwinCAT.JsonExtension
TwinCAT variables to and from json 

[![Build status](https://ci.appveyor.com/api/projects/status/4ggo35buwmno05u2?svg=true)](https://ci.appveyor.com/project/fbarresi/twincat-jsonextension)
![Licence](https://img.shields.io/github/license/fbarresi/twincat.jsonextension.svg)
[![Nuget Version](https://img.shields.io/nuget/v/TwinCAT.JsonExtension.svg)](https://www.nuget.org/packages/TwinCAT.JsonExtension/)

Bring the power of Json.Net to TwinCAT

Tranform DUTs decorated with the _custom_ **Json-Attribute** like this:

```
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


Have fun using this simple package and don't forget to star this project! 
