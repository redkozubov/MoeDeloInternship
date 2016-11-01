using System;
using System.Runtime.Serialization;

namespace GenericsPractice
{
    class NoMoreElementsException : Exception
    {
        public NoMoreElementsException()
        {
        }

        public NoMoreElementsException(string message)
            : base(message)
        {
        }

        public NoMoreElementsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NoMoreElementsException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}