using System;

namespace GenProj
{
    /// <summary>
    /// Выбрасывается при попытке получить следующий элемент списка, когда этот элемент не задан
    /// </summary>
    class EmptyNextElementException : Exception
    {
        public EmptyNextElementException (string message): base(message) { }
    }
}