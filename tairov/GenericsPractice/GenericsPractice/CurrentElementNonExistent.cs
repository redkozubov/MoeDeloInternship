using System;
using System.Runtime.Serialization;

namespace GenericsPractice
{
    class CurrentElementNonExistent : Exception
    {
        public CurrentElementNonExistent()
        {
        }

        public CurrentElementNonExistent(string message)
            : base(message)
        {
        }

        public CurrentElementNonExistent(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected CurrentElementNonExistent(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }
    }
}