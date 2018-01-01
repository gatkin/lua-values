using System;
using Xunit;

namespace LuaValues.Tests
{
    public class ParseTests
    {
        [Fact]
        public void TestBoolean()
        {
            var testCases = new []
            {
                ("true", (true, true)),
                ("42", (false, false)),
            };

            foreach(var (chunk, (expectedSuccess, expectedValue)) in testCases)
            {
                var (actualSuccess, actualValue) = LuaValues.BooleanFromLuaChunk(chunk);
                Assert.Equal(expectedSuccess, actualSuccess);
                Assert.Equal(expectedValue, actualValue);
            }

        }

        [Fact]
        public void TestNumber()
        {
             var testCases = new []
             {
                 ("1", (true, 1)),
                 ("3.14", (true, 3.14)),
                 ("'Hello'", (false, 0.0)),
             };

             foreach(var (chunk, (expectedSuccess, expectedValue)) in testCases )
             {
                 var (actualSuccess, actualValue) = LuaValues.NumberFromLuaChunk(chunk);
                 Assert.Equal(expectedSuccess, actualSuccess);
                 Assert.Equal(expectedValue, actualValue);
             }
        }

        [Fact]
        public void TestString()
        {
            var testCases = new []
            {
                ("'Hello'", (true, "Hello")),
                ("\"Hello\"", (true, "Hello")),
                ("3.14", (false, String.Empty)),
                ("No quotes", (false, String.Empty)),
            };

            foreach(var (chunk, (expectedSuccess, expectedValue)) in testCases)
            {
                var (actualSuccess, actualValue) = LuaValues.StringFromLuaChunk(chunk);
                Assert.Equal(expectedSuccess, actualSuccess);
                Assert.Equal(expectedValue, actualValue);
            }
        }
    }
}