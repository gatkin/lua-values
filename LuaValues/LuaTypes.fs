namespace LuaValues

module internal LuaTypes =

    // See https://www.lua.org/pil/2.html for a reference on all Lua types.
    // Although arrays in Lua are just tables, arrays are represented separately
    // from tables here because they are serialized differently.
    type LuaValue =
        | LuaBoolean of bool
        | LuaNumber of double
        | LuaString of string
        | LuaArray of list<LuaValue>
        | LuaTable of fields: list<TableField>

    and TableField =
        {
            name: string
            value: LuaValue
        }