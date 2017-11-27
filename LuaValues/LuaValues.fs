namespace LuaValues

module LuaValues =

    // See https://www.lua.org/pil/2.html for a reference on all Lua types.
    // Although arrays in Lua are just tables, arrays are represented separately
    // from tables here because they are serialized differently.
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

    /// Converts a sequence of CLR values to a Lua array.
    let private toLuaArray (toValue : 'a -> LuaValue) arrayVal =
        LuaArray(arrayVal |> Seq.map toValue |> Seq.toList)
 
    /// Converts a CLR value to a Lua value.
    let rec private toLuaValue (value: obj) =
        match value with
        | :? bool as boolVal -> LuaBoolean(boolVal)
        | :? double as doubleVal -> LuaNumber(doubleVal)
        | :? int as intVal -> LuaNumber(double intVal)
        | :? string as stringVal -> LuaString(stringVal)
        | :? seq<bool> as arrayVal -> toLuaArray LuaBoolean arrayVal // Arrays of primitive values must be matched separately
        | :? seq<double> as arrayVal -> toLuaArray LuaNumber arrayVal
        | :? seq<int> as arrayVal -> toLuaArray (LuaNumber << double) arrayVal
        | :? seq<obj> as arrayVal -> toLuaArray toLuaValue arrayVal
        | _ -> toLuaTable value

    /// Converts a CLR object to a Lua table where the properties of the object are used as the fields
    /// of the table.
    and private toLuaTable value =
        value.GetType().GetProperties()
        |> Seq.filter (fun prop -> prop.CanRead)
        |> Seq.map (fun prop -> { name = prop.Name; value = toLuaValue (prop.GetValue value) })
        |> Seq.toList
        |> LuaTable
