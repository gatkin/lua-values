using System;
using Xunit;

namespace LuaValues.Tests
{
    public class LuaValueTests
    {
        [Fact]
        public void TestAggregate()
        {
            var value = new {
                message = "Hello",
                intNumber = 1256,
                floatNumber = 3.14,
                flag = true,
            };

            var expected = @"{message=""Hello"", intNumber=1256, floatNumber=3.14, flag=true}";

            var actual = LuaValues.ToLuaChunk(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayOfBooleans()
        {
            var value = new [] { true, true, false };
            
            var expected = "{true, true, false}";

            var actual = LuaValues.ToLuaChunk(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayOfFloats()
        {
            var value = new [] { 17.23, 42.1, 789.01 };
            
            var expected = "{17.23, 42.1, 789.01}";

            var actual = LuaValues.ToLuaChunk(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayOfIntegers()
        {
            var value = new [] { 17, 42, 789, 23 };
            
            var expected = "{17, 42, 789, 23}";

            var actual = LuaValues.ToLuaChunk(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestArrayOfStrings()
        {
            var value = new [] { "hello", "world", "goodbye", "mars" };
            
            var expected = @"{""hello"", ""world"", ""goodbye"", ""mars""}";

            var actual = LuaValues.ToLuaChunk(value);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBoolean()
        {
            var testCases = new [] {
                (true, "true"),
                (false, "false"),
            };

            foreach (var (value, expected) in testCases)
            {
                var actual = LuaValues.ToLuaChunk(value);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestNumbers()
        {
            var testCases = new[] {
                    (1, "1"),
                    (100, "100"),
                    (3.14, "3.14"),
                    (5.29E-05, "5.29E-05"),
                    (1.7E07, "1.7E+07"),
                };

            foreach (var (value, expected) in testCases)
            {
                var actual = LuaValues.ToLuaChunk(value);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestStrings()
        {
            var testCases = new[] {
                ("hello", @"""hello"""),
                ("", @""""""),
            };

            foreach (var (value, expected) in testCases)
            {
                var actual = LuaValues.ToLuaChunk(value);
                Assert.Equal(expected, actual);
            }
        }
    }
}
