using System;

namespace Mmcc.ApplicationParser
{
    public class InvalidPairsException : Exception
    {
        public InvalidPairsException()
        {
        }
        
        public InvalidPairsException(string message)
            : base(message)
        {
        }
        
        public InvalidPairsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}