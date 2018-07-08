module LuaValues.Test

open NUnit.Framework
open LuaValues.LuaTypes
open LuaValues.Parse

[<TestFixture>]
type TestParse () =

    let testParse input expected =
        let actual = parseValue input
        Assert.AreEqual(expected, actual)

    let testSuccess input expected =
        testParse input (Some expected)

    [<Test>]
    member this.``Parse nil`` () =
        testParse "nil" (Some LuaNil)

    [<Test>]
    member this.``Parse nil with whitespace`` () =
        testSuccess "  nil " LuaNil

    [<Test>]
    member this.``Parse false`` () =
        testSuccess "false" (LuaBoolean false)

    [<Test>]
    member this.``Parse true`` () =
        testSuccess "true" (LuaBoolean true)

    [<Test>]
    member this.``Parse number`` () =
        testSuccess "3.14" (LuaNumber 3.14)

    [<Test>]
    member this.``Parse single quoted string`` () =
        testSuccess "'Hello, World!'" (LuaString "Hello, World!")

    [<Test>]
    member this.``Parse double quoted string`` () =
        testSuccess "\"Go Big Red!\"" (LuaString "Go Big Red!")

    [<Test>]
    member this.``Parse single quoted string with double quotes inside`` () =
        testSuccess "'He said \"Hello\" to her'" (LuaString "He said \"Hello\" to her" )

    [<Test>]
    member this.``Parse double quoted string with single quotes inside`` () =
        testSuccess "\"She said 'Goodbye' to him\"" (LuaString "She said 'Goodbye' to him")

    [<Test>]
    member this.``Parse array of numbers`` () =
        testSuccess "{1,2,3}" (LuaArray [LuaNumber 1.0; LuaNumber 2.0; LuaNumber 3.0])

    [<Test>]
    member this.``Parse array of numbers with whitespace`` () =
        testSuccess "{ 1 ,  2,3 ; 4  }" (LuaArray [LuaNumber 1.0; LuaNumber 2.0; LuaNumber 3.0; LuaNumber 4.0])

    [<Test>]
    member this.``Parse nested array`` () =
        let expected =
            LuaArray [LuaNumber 1.0
                      LuaArray [LuaString "hello"
                                LuaString "bye"
                                LuaArray [ LuaBoolean false ] 
                                LuaString "a"]
                      LuaNumber 2.0
                      LuaArray [LuaBoolean true; LuaNil]]

        testSuccess "{1, {'hello', 'bye', {false}, 'a'}, 2, {true, nil}}" expected

    [<Test>]
    member this.``Parse table`` () =
        testSuccess "{x = 1, y = 2}" (LuaTable [ { Name = "x"; Value = LuaNumber 1.0 }; { Name = "y"; Value = LuaNumber 2.0 } ])

    [<Test>]
    member this.``Parse empty table`` () =
        testSuccess "{}" (LuaTable [])

    [<Test>]
    member this.``Parse nested table`` () =
        let expected =
            LuaTable [{ Name = "x"; Value = LuaNumber 1.0 }
                      { Name = "y"; Value = LuaTable [{ Name = "a"; Value = LuaBoolean true }
                                                      { Name = "b"; Value = LuaArray [LuaNumber 1.0; LuaNumber 2.0] }]}
                      { Name = "x"; Value = LuaNil }]

        testSuccess "{x = 1, y = {a = true, b = {1, 2}}, x = nil}" expected