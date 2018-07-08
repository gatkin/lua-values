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
