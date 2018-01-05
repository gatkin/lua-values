module Tests

open System
open Xunit
open LuaValues.LuaTypes
open LuaValues.Parse


let private testParse input expected =
    let actual = parseValue input
    Assert.Equal(expected, actual)

let private testSuccess input expected =
    testParse input (Some expected)


[<Fact>]
let ``Parse nil`` () =
    testParse "nil" (Some LuaNil)

[<Fact>]
let ``Parse nil with whitespace`` () =
    testSuccess "  nil " LuaNil

[<Fact>]
let ``Parse false`` () =
    testSuccess "false" (LuaBoolean false)

[<Fact>]
let ``Parse true`` () =
    testSuccess "true" (LuaBoolean true)

[<Fact>]
let ``Parse number`` () =
    testSuccess "3.14" (LuaNumber 3.14)

[<Fact>]
let ``Parse single quoted string`` () =
    testSuccess "'Hello, World!'" (LuaString "Hello, World!")

[<Fact>]
let ``Parse double quoted string`` () =
    testSuccess "\"Go Big Red!\"" (LuaString "Go Big Red!")

[<Fact>]
let ``Parse single quoted string with double quotes inside`` () =
    testSuccess "'He said \"Hello\" to her'" (LuaString "He said \"Hello\" to her" )

[<Fact>]
let ``Parse double quoted string with single quotes inside`` () =
    testSuccess "\"She said 'Goodbye' to him\"" (LuaString "She said 'Goodbye' to him")

[<Fact>]
let ``Parse array of numbers`` () =
    testSuccess "{1,2,3}" (LuaArray [LuaNumber 1.0; LuaNumber 2.0; LuaNumber 3.0])

[<Fact>]
let ``Parse array of numbers with whitespace`` () =
    testSuccess "{ 1 ,  2,3 ; 4  }" (LuaArray [LuaNumber 1.0; LuaNumber 2.0; LuaNumber 3.0; LuaNumber 4.0])

[<Fact>]
let ``Parse nested array`` () =
    let expected =
        LuaArray [LuaNumber 1.0
                  LuaArray [LuaString "hello"
                            LuaString "bye"
                            LuaArray [ LuaBoolean false ] 
                            LuaString "a"]
                  LuaNumber 2.0
                  LuaArray [LuaBoolean true; LuaNil]]

    testSuccess "{1, {'hello', 'bye', {false}, 'a'}, 2, {true, nil}}" expected

[<Fact>]
let ``Parse table`` () =
    testSuccess "{x = 1, y = 2}" (LuaTable [ { Name = "x"; Value = LuaNumber 1.0 }; { Name = "y"; Value = LuaNumber 2.0 } ])

[<Fact>]
let ``Parse empty table`` () =
    testSuccess "{}" (LuaTable [])

[<Fact>]
let ``Parse nested table`` () =
    let expected =
        LuaTable [{ Name = "x"; Value = LuaNumber 1.0 }
                  { Name = "y"; Value = LuaTable [{ Name = "a"; Value = LuaBoolean true }
                                                  { Name = "b"; Value = LuaArray [LuaNumber 1.0; LuaNumber 2.0] }]}
                  { Name = "x"; Value = LuaNil }]

    testSuccess "{x = 1, y = {a = true, b = {1, 2}}, x = nil}" expected