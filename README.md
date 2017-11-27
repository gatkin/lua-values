# LuaValues
Simple library for serializing CLR values into Lua values.

```c#
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
