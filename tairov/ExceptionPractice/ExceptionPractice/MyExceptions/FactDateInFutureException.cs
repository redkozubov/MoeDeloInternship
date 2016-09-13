using System;
using System.Runtime.Serialization;

namespace ExceptionPractice.MyExceptions
{
    public class FactDateInFutureException : MyException
    {
        public FactDateInFutureException()
        {
        }

        public FactDateInFutureException(string message)
            : base(message)
        {
        }

        public FactDateInFutureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected FactDateInFutureException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}