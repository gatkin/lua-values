namespace LuaValues

module internal Serialize =

    open LuaValues.LuaTypes

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


    /// Serliazes the list of values into a Lua chunk.
    let private aggregateToChunk toChunk values  =
        values |> Seq.map toChunk |> String.concat ", " |> sprintf "{%s}"  


    /// Serializes the value to a Lua chunk.
    let rec private toLuaChunk (value: LuaValue) =
        match value with
        | LuaBoolean(boolVal) -> sprintf "%b" boolVal
        | LuaNumber(numberVal) -> sprintf "%G" numberVal
        | LuaString(stringVal) -> sprintf "\"%s\"" stringVal
        | LuaArray(arrayValues) -> aggregateToChunk toLuaChunk arrayValues
        | LuaTable(fields) -> aggregateToChunk tableFieldToChunk fields


    /// Serializes the table field so it can be placed into a Lua chunk.
    and private tableFieldToChunk (field: TableField) =
        sprintf "%s=%s" field.name (toLuaChunk field.value)


    // Serializes a CLR object into a Lua chunk.
    let serializeValue (value: obj) =
        value |> toLuaValue |> toLuaChunk        