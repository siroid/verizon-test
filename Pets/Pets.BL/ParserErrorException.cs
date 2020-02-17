using System;

namespace Pets.BL
{
    public class ParserErrorException : Exception
    {
        public ParserErrorException()
        {
        }

        public ParserErrorException(string message)
            : base(message)
        {
        }

        public ParserErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
