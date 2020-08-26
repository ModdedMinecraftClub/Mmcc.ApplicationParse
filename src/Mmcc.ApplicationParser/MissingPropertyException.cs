using System;

namespace Mmcc.ApplicationParser
{
    public class MissingPropertyException : Exception
    {
        public MissingPropertyException()
        {
        }
        
        public MissingPropertyException(string message)
            : base(message)
        {
        }
        
        public MissingPropertyException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}