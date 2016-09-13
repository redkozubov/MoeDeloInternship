using System;
using System.Runtime.Serialization;

namespace ExceptionPractice.MyExceptions
{
    public class UserForgottenFileException : MyException
    {
        public UserForgottenFileException()
        {
        }

        public UserForgottenFileException(string message)
            : base(message)
        {
        }

        public UserForgottenFileException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected UserForgottenFileException(SerializationInfo si, StreamingContext sc) : base(si, sc)
        {
        }
    }
}