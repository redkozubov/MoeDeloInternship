using System;

namespace GenProj
{
    /// <summary>
    /// Выбрасывается при попытке получить элемент пустого списка
    /// </summary>
    class EmptyListException : Exception
    {
        public EmptyListException (string message): base(message) { }
    }
}