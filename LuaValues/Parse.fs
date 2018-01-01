namespace LuaValues

/// Parses Lua chunks into CLR values.
module Parse =

    open System
    open LuaValues.LuaTypes


    let private removeOuterChars (text: string) =
        if text.Length < 2 then text
        else text.Substring (1, text.Length - 2)


    let private tryParseBoolean = function
        | "true" -> Some(true)
        | "false" -> Some(false)
        | _ -> None


    let private tryParseNumber (text: string) =
        match Double.TryParse text with
        | (false, _) -> None
        | (true, value) -> Some(value)


    let private tryParseString (text: string) =
        if text.Length < 2 then
            // Must have at least two characters for the quotes
            None
        else
            let firstChar = text.Chars 0
            let finalChar = text.Chars (text.Length - 1) 
            match (firstChar, finalChar) with
            | ('\'', '\'') | ('"', '"') -> Some(removeOuterChars text)
            | _ -> None


    let private luaValueParser (parser: string -> 'a option) (valueType: 'a -> LuaValue) (chunk: string) =
        parser chunk |> Option.map valueType


    let private luaBooleanParser =
        luaValueParser tryParseBoolean LuaBoolean


    let private luaNumberParser =
        luaValueParser tryParseNumber LuaNumber


    let private luaStringParser =
        luaValueParser tryParseString LuaString


    /// Attempts to parse the given Lua chunk into a Lua value.
    let parseValue (chunk: string) =
        let rec parseChunk (parsers: (string -> 'a option) list) chunk =
            match parsers with
            | [] -> None
            | parser :: tl ->
                match parser chunk with
                | None -> parseChunk tl chunk
                | Some(value) -> Some(value)
        
        let parsers = [luaBooleanParser; luaNumberParser; luaStringParser]
        parseChunk parsers chunk
