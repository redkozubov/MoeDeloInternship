using System;
using System.Runtime.Serialization;

namespace ExceptionPractice.MyExceptions
{
    public class MyException : Exception
    {
        public MyException()
        {
        }

        public MyException(string message)
            : base(message)
        {
        }

        public MyException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected MyException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }

        public override string ToString()
        {
            return (Message + ((InnerException != null) ? InnerException.ToString() : "")
                    + Source + StackTrace);
        }
    }
}