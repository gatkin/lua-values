using System;
using Xunit;

namespace LuaValues.Tests
{
    public class LuaValueTests
    {
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
    }
}
