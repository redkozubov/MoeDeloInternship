using System;
using System.Runtime.Serialization;

namespace GenericsPractice
{
    class CurrentElementNotExistExcpetion : Exception
    {
        public CurrentElementNotExistExcpetion()
        {
        }

        public CurrentElementNotExistExcpetion(string message)
            : base(message)
        {
        }

        public CurrentElementNotExistExcpetion(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected CurrentElementNotExistExcpetion(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}