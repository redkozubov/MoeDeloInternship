using System;

namespace generics_task
{
    class ListRangeException: Exception
    {
        public ListRangeException()
        {
        }
        public ListRangeException(string message):base(message)
        {
        }
        public ListRangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
