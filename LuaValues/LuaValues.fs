namespace LuaValues


/// Contains public API functions intended to be called from C# code.
module LuaValues =

    open LuaValues.Serialize


    /// Converts a CLR value to a string that can be used as a Lua chunk. Primitive values such as
    /// numbers, strings, and booleans are serialized directly to the corresponding Lua primitives.
    /// Arrarys are serialized to Lua tables with no explicit field names. Objects are serialized
    /// to Lua tables where the public, readable properties of the object are used as the fields
    /// in the table with the name of the property used as the name of the table field.
    let ToLuaChunk (value: obj) =
        serializeValue value