namespace GenProj
{
    class GenListElement <T> where T: struct
    {
        /// <summary>
        /// значение текущего элемента
        /// </summary>
        public T Element;
        /// <summary>
        /// сслыка на следующий элемент
        /// </summary>
        public GenListElement<T> next;
    }
}