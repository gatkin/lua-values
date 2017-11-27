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
