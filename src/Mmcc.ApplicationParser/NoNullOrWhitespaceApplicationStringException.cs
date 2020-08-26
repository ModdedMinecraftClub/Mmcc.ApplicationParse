using System;

namespace Mmcc.ApplicationParser
{
    public class NoNullOrWhitespaceApplicationStringException : Exception
    {
        public NoNullOrWhitespaceApplicationStringException()
        {
        }

        public NoNullOrWhitespaceApplicationStringException(string message)
            : base(message)
        {
        }

        public NoNullOrWhitespaceApplicationStringException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}