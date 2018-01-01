namespace LuaValues


/// Contains public API functions intended to be called from C# code.
module LuaValues =

    open System
    open LuaValues.LuaTypes
    open LuaValues.Parse
    open LuaValues.Serialize


    /// Parses the given Lua chunk as a boolean value. Returns a pair where the first value
    /// is true if the chunk was parsed succesfully, false otherwise, and the second value
    /// is the parsed value.
    let BooleanFromLuaChunk (chunk: string) =
        match parseValue chunk with
        | Some(LuaBoolean(value)) -> (true, value)
        | _ -> (false, false)


    /// Parses the given Lua chunk as a numerical value. See also BooleanFromLuaChunk.
    let NumberFromLuaChunk (chunk: string) =
        match parseValue chunk with
        | Some(LuaNumber(value)) -> (true, value)
        | _ -> (false, 0.0)


    /// Parses the given Lua chunk as a string. See also BooleanFromLuaChunk.
    let StringFromLuaChunk (chunk: string) =
        match parseValue chunk with
        | Some(LuaString(value)) -> (true, value)
        | _ -> (false, String.Empty)    


    /// Converts a CLR value to a string that can be used as a Lua chunk. Primitive values such as
    /// numbers, strings, and booleans are serialized directly to the corresponding Lua primitives.
    /// Arrarys are serialized to Lua tables with no explicit field names. Objects are serialized
    /// to Lua tables where the public, readable properties of the object are used as the fields
    /// in the table with the name of the property used as the name of the table field.
    let ToLuaChunk (value: obj) =
        serializeValue value