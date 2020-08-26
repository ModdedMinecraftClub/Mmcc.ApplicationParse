namespace Mmcc.ApplicationParser.Tests
{
    public class OuterClasses
    {
        public class AllBasicSupportedTypes
        {
            public bool Bool { get; set; }
            public byte Byte { get; set; }
            public char Char { get; set; }
            public decimal Decimal { get; set; }
            public double Double { get; set; }
            public int Int { get; set; }
        }
        
        public class OneBool
        {
            public bool Bool { get; set; }
        }
        
        public class TwoBools
        {
            public bool Bool { get; set; }
            public bool SecondBool { get; set; }
        }
    }
}