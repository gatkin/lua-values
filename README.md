# LuaValues

[![Build Status](https://travis-ci.org/gatkin/lua-values.svg?branch=master)](https://travis-ci.org/gatkin/lua-values)
[![Build status](https://ci.appveyor.com/api/projects/status/o05aikp5f4kxc8jd?svg=true)](https://ci.appveyor.com/project/gatkin/lua-values)
[![License](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/gatkin/lua-values/blob/master/LICENSE)

Simple library for serializing CLR values into Lua values.

```csharp
var value = new {
    message = "Hello",
    number = 125,
    flag = true,
    measurements = new[] { 1.23, 4.56, 7.89 },
    subValue = new {
        message = "Goodbye",
        flag = false
    }
};

LuaValues.ToLuaChunk(value);
// {message="Hello", number=125, flag=true, measurements={1.23, 4.56, 7.89}, subValue={message="Goodbye", flag=false}}
```
