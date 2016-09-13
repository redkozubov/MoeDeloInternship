using System;
using System.Runtime.Serialization;

namespace ExceptionPractice.MyExceptions
{
    public class NoDataException : MyException
    {
        public NoDataException()
        {
        }

        public NoDataException(string message)
            : base(message)
        {
        }

        public NoDataException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoDataException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}