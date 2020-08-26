using Xunit;
using Xunit.Abstractions;

namespace Mmcc.ApplicationParser.Tests
{
    public class SerializationTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly MmccApplicationSerializer _serializer;

        public SerializationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _serializer = new MmccApplicationSerializer();
        }

        [Fact]
        public void DeserializesBasicSupportedTypes()
        {
            var s = "Bool: true\n" +
                             "Byte: 255\n" +
                             "Char: !\n" +
                             $"Decimal: {decimal.One}\n" +
                             "Double: 0.5\n" +
                             "Int: 10\n";
            var actual = _serializer.Deserialize<OuterClasses.AllBasicSupportedTypes>(s);
            
            byte expectedByte = 255;
            char expectedChar = '!';
            decimal expectedDecimal = decimal.One;
            double expectedDouble = 0.5;
            int expectedInt = 10;
            
            Assert.True(actual.Bool);
            Assert.Equal(expectedByte, actual.Byte);
            Assert.Equal(expectedChar, actual.Char);
            Assert.Equal(expectedDecimal, actual.Decimal);
            Assert.Equal(expectedDouble, actual.Double);
            Assert.Equal(expectedInt, actual.Int);
        }

        [Fact]
        public void DeserializesWithoutWhitespaceAfterColon()
        {
            const string s = "Bool:true\n" +
                    "SecondBool:true";
            var actual = _serializer.Deserialize<OuterClasses.TwoBools>(s);
            
            Assert.True(actual.Bool);
            Assert.True(actual.SecondBool);
        }

        [Fact]
        public void DeserializesCaseInsensitive()
        {
            var caseInsensitiveSerializer = new MmccApplicationSerializer(true);
            const string s = "bOoL:true";
            var actual = caseInsensitiveSerializer.Deserialize<OuterClasses.OneBool>(s);
            
            Assert.True(actual.Bool);
        }

        [Fact]
        public void Serializes()
        {
            var obj = new OuterClasses.AllBasicSupportedTypes
            {
                Bool = true,
                Byte = 255,
                Char = '!',
                Decimal = decimal.One,
                Double = 0.55,
                Int = 10
            };
            const string expected = "Bool: True\r\nByte: 255\r\nChar: !\r\nDecimal: 1\r\nDouble: 0.55\r\nInt: 10\r\n";
            var actual = _serializer.Serialize(obj);
            
            Assert.Equal(expected, actual);
        }
    }
}
