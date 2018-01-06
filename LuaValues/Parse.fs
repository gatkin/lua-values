namespace LuaValues

/// Parses Lua chunks into CLR values.
module Parse =

    open FParsec
    open LuaValues.LuaTypes

    (* Parsers for literals *)
    let ws = spaces

    let private pLuaNil = stringReturn "nil" LuaNil

    let private pLuaFalse = stringReturn "false" (LuaBoolean false)

    let private pLuaTrue = stringReturn "true" (LuaBoolean true)

    let private pLuaBoolean = pLuaFalse <|> pLuaTrue

    let private pLuaNumber = pfloat |>> LuaNumber

    let private pStringLiteral quoteChar =
        let normalChar = satisfy (fun c -> c <> '\\' && c <> quoteChar)

        let unEscape = function
            | 'n' -> '\n'
            | 'r' -> '\r'
            | 't' -> '\t'
            | c -> c

        let escapedSet = sprintf "\\nrt%c" quoteChar

        let escapedChar = pstring "\\" >>. (anyOf escapedSet |>> unEscape)

        let quote = pchar quoteChar

        between quote quote (manyChars (normalChar <|> escapedChar))

    let private pDoubleQuotedChar = pStringLiteral '"'

    let private pSingleQuotedChar = pStringLiteral '\''

    let private pLuaString = (pDoubleQuotedChar <|> pSingleQuotedChar) |>> LuaString


    (* Parsers for aggregates *)
    let private pLuaValue, pLuaValueRef = createParserForwardedToRef<LuaValue, unit>()

    let private pLuaValueWs = pLuaValue .>> ws

    let private pOpenBrace = pstring "{"

    let private pOpenBraceWs = pOpenBrace .>> ws

    let private pCloseBrace = pstring "}"

    let private pCloseBraceWs = ws >>. pCloseBrace

    let private pSeparator = (pstring ",") <|> (pstring ";")

    let private pSeparatorWs = pSeparator .>> ws
    
    let private pLuaArray =
        let pElements = sepEndBy pLuaValueWs pSeparatorWs
        between pOpenBraceWs pCloseBraceWs pElements |>> LuaArray

    let private pName = regex "[a-zA-Z_][a-zA-Z0-9_]*"

    let private pNameWs = pName .>> ws

    let private pEquals = pstring "="

    let private pEqualsWs = pEquals .>> ws

    let private pTableField =
        pipe3 pNameWs pEqualsWs pLuaValueWs (fun name _ value -> { Name = name; Value = value })

    let private pLuaAssociativeArray =
        let pElements = sepEndBy pTableField pSeparatorWs

        between pOpenBraceWs pCloseBraceWs pElements |>> LuaTable

    let private pLuaTable =
        (attempt pLuaAssociativeArray) <|> pLuaArray


    do pLuaValueRef := choice[pLuaNil
                              pLuaBoolean
                              pLuaNumber
                              pLuaTable
                              pLuaArray
                              pLuaString]


    /// A parser for Lua values that only succeeds when all input has been consumed.
    let private pLuaValueConsumeAll =
        let valueWs = ws >>. pLuaValue .>> ws
        valueWs .>> eof

    /// Parses the given string as a Lua value.
    let parseValue chunk =
        match run pLuaValueConsumeAll chunk with
        | Success(value, _, _) -> Some value
        | Failure(_) -> None



module LuaValueConverter =

    open System.Collections.Generic
    open System.Reflection
    open LuaValues.LuaTypes

    /// Encodes reflected type information of CLR types to support easier pattern matching than is
    /// possible when working with System.Type values directly. Only encodes the types that are relevant
    /// for parsing Lua values.
    type ClrType =
        | ClrBoolean
        | ClrNumber
        | ClrString
        | ClrList of elememtType: ClrType
        | ClrObject of classType: System.Type * fields: ClrObjectField list
    and
        ClrObjectField =
            { Name: string
              Type: ClrType
            }


    let rec private typeToClrType t =
        // Cannot pattern match on System.Type values
        if t = typeof<bool> then
            ClrBoolean
        else if t = typeof<double> || t = typeof<float> || t = typeof<int> then
            ClrNumber
        else if t = typeof<string> then
            ClrString
        else if t.IsArray then
            ClrList (typeToClrType (t.GetElementType ()))
        else if t = typeof<IEnumerable<_>> then
            let elementType = Array.head t.GenericTypeArguments
            ClrList (typeToClrType elementType)
        else
            objectTypeToClrType t
        
    and private objectTypeToClrType objectType =
        let fields =
            objectType.GetProperties()
            |> Seq.filter (fun prop -> prop.CanRead && prop.CanWrite)
            |> Seq.map (fun prop -> { Name = prop.Name; Type = typeToClrType prop.PropertyType })
            |> Seq.toList
    
        ClrObject (objectType, fields)


    let rec private convertLuaValue clrType luaValue =
        match (luaValue, clrType) with
        | (LuaBoolean value, ClrBoolean) ->
            value :> obj |> Some
        | (LuaNumber value, ClrNumber) ->
            value :> obj |> Some
        | (LuaString value, ClrString) ->
            value :> obj |> Some
        | (LuaArray elements, ClrList elementType) ->
            convertLuaArray elementType elements
        | (LuaTable tableFields, ClrObject(objectType,objectFields)) ->
            convertLuaTable tableFields objectFields objectType
        | _ ->
            None

    and private convertLuaArray elementType arrayElements =
        let rec convertArrayIter acc elements =
            match elements with
            | [] -> acc |> List.rev |> List.toArray :> obj |> Some
            | hd::tl ->
                match convertLuaValue elementType hd with
                | None -> None
                | Some value -> convertArrayIter (value::acc) tl

        convertArrayIter [] arrayElements            


    and private convertLuaTable tableFields objectFields objectType =
        
        let rec findFieldValueInTable (objectField: ClrObjectField) (tableFields: TableField list) =
            match tableFields with
            | [] ->
                None
            | field::_ when field.Name = objectField.Name ->
                convertLuaValue objectField.Type field.Value
            | _::tl ->
                findFieldValueInTable objectField tl

        let findFieldValuesInTable (tableFields: TableField list) (objectFields: ClrObjectField list) =
            let rec findFieldValuesIter acc tableFields objectFields =
                match objectFields with
                | [] -> Some acc
                | field::tl ->
                    match findFieldValueInTable field tableFields with
                    | None -> None
                    | Some value -> findFieldValuesIter ((field.Name, value)::acc) tableFields tl

            findFieldValuesIter [] tableFields objectFields    
        
       
        let setObjectFieldValue objectValue fieldName fieldValue =
            let bindingFlags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.SetProperty
            objectType.InvokeMember(fieldName, bindingFlags, System.Type.DefaultBinder, objectValue, [| fieldValue |])

        match findFieldValuesInTable tableFields objectFields with
        | None -> None
        | Some fieldValues ->
            // Instantiate and populate the object's fields.
            let objectValue = System.Activator.CreateInstance(objectType);
            fieldValues |> List.iter (fun (name, value) -> setObjectFieldValue objectValue name value |> ignore);
            Some objectValue


    /// Parses a Lua chunk into the specified CLR type.
    let fromLuaValue valueType chunk =
        let clrType = typeToClrType valueType
        Parse.parseValue chunk
        |> Option.bind (fun luaValue -> convertLuaValue clrType luaValue)