namespace LuaValues

module LuaValues =

    type private LuaValue =
        | LuaBoolean of bool
        | LuaNumber of double
        | LuaString of string
        | LuaArray of list<LuaValue>
        | LuaTable of fields: list<TableField>

    and private TableField =
        {
            name: string
            value: LuaValue
        }
