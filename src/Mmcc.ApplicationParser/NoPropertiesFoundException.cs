using System;

namespace Mmcc.ApplicationParser
{
    public class NoPropertiesFoundException : Exception
    {
        public NoPropertiesFoundException()
        {
        }

        public NoPropertiesFoundException(string message)
            : base(message)
        {
        }
        
        public NoPropertiesFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}