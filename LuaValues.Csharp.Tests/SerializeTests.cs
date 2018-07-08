using NUnit.Framework;

namespace LuaValues.CSharp.Tests
{
    [TestFixture]
    public class SerializeTests
    {
        [Test]
        public void Serialize_AnonymousClass()
        {
            var value = new {
                message = "Hello",
                intNumber = 1256,
                floatNumber = 3.14,
                flag = true,
            };

            var expected = @"{message=""Hello"", intNumber=1256, floatNumber=3.14, flag=true}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serialize_AnonymousClass_Nested()
        {
            var value = new {
                message = "Hello",
                data = new {
                    number = 37,
                    moreData = new {
                        flag = false,
                        values = new[] {37, 15, 72}
                    }
                }
            };

            var expected = @"{message=""Hello"", data={number=37, moreData={flag=false, values={37, 15, 72}}}}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serialize_Array_AnonymouClass()
        {
            var value = new[] {
                new { X = "Hello", Y = 37 },
                new { X = "Goodbye", Y = 42 },
            };

            var expected = @"{{X=""Hello"", Y=37}, {X=""Goodbye"", Y=42}}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serailize_Array_Booleans()
        {
            var value = new [] { true, true, false };
            
            var expected = "{true, true, false}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serialize_Array_Empty()
        {
            var value = new int[0];

            var expected = "{}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serialize_Array_Floats()
        {
            var value = new [] { 17.23, 42.1, 789.01 };
            
            var expected = "{17.23, 42.1, 789.01}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serialize_Array_Integers()
        {
            var value = new [] { 17, 42, 789, 23 };
            
            var expected = "{17, 42, 789, 23}";

            TestSerialize(value, expected);
        }

        [Test]
        public void Serialize_Array_Strings()
        {
            var value = new [] { "hello", "world", "goodbye", "mars" };
            
            var expected = @"{""hello"", ""world"", ""goodbye"", ""mars""}";

            TestSerialize(value, expected);
        }

        [Test]
        public void TestBoolean()
        {
            var testCases = new [] {
                (true, "true"),
                (false, "false"),
            };

            foreach (var (value, expected) in testCases)
            {
                TestSerialize(value, expected);
            }
        }

        [Test]
        public void TestNil()
        {
            TestSerialize(null, "nil");
        }

        [Test]
        public void TestNilInAggregate()
        {
            var value = new { X = 1, Y = (string)null };
            var expected = "{X=1, Y=nil}";
            TestSerialize(value, expected);
        }

        [Test]
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
                TestSerialize(value, expected);
            }
        }

        [Test]
        public void TestStrings()
        {
            var testCases = new[] {
                ("hello", @"""hello"""),
                ("", @""""""),
            };

            foreach (var (value, expected) in testCases)
            {
                TestSerialize(value, expected);
            }
        }

        [Test]
        public void TestReadmeExample()
        {
            var value = new {
                message = "Hello",
                number = 125,
                flag = true,
                measurements = new[] {1.23, 4.56, 7.89},
                subValue = new {
                    message = "Goodbye",
                    flag = false
                }
            };

            var expected = @"{message=""Hello"", number=125, flag=true, measurements={1.23, 4.56, 7.89}, subValue={message=""Goodbye"", flag=false}}";

            TestSerialize(value, expected);
        }

        private void TestSerialize(object value, string expected)
        {
            var actual = LuaValues.ToLuaChunk(value);
            Assert.AreEqual(expected, actual);
        }
    }
}
