namespace GenProj
{
    class GenList <T> where T: struct
    {
        protected GenListElement<T> First { get; set; }
        protected GenListElement<T> Current { get; set; }

        public T GetNext()
        {
            if (First == null)
                throw new EmptyListException("В указанном списке отсутствуют элементы");
            if (Current == null)
                throw new EmptyNextElementException("Невозможно получить следующий элемент списка, т.к. скписок закончился");
            T res = Current.Element;
            var n = Current.next;
            Current = n;
            return res;
        }  

        public void AddToHead(T value)
        {
            // если в списке ещё нет элементов
            if (First == null && Current == null)
                First = Current = new GenListElement<T> { Element = value, next = null };
            // если уже были элементы
            else
            {
                var tmp = First;
                First = new GenListElement<T> { Element = value, next = tmp };
            }
        }

        public void GoToHead()
        {
            if (First == null) throw new EmptyListException("В указанном списке отсутствуют элементы");
            Current = First;
        }
    }
}