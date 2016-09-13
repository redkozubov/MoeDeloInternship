using System;
using System.Runtime.Serialization;

namespace ExceptionPractice.MyExceptions
{
    public class IncorrectFileFormatException : MyException
    {
        public IncorrectFileFormatException()
        {
        }

        public IncorrectFileFormatException(string message)
            : base(message)
        {
        }

        public IncorrectFileFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected IncorrectFileFormatException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}