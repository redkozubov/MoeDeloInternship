namespace GenProj
{
    class GenList <T> where T: struct
    {
        protected GenListElement<T> First { get; set; }
        protected GenListElement<T> Current { get; set; }

        public T GetNext()
        {
            if (First == null)
                throw new EmptyListException("� ��������� ������ ����������� ��������");
            if (Current == null)
                throw new EmptyNextElementException("���������� �������� ��������� ������� ������, �.�. ������� ����������");
            T res = Current.Element;
            var n = Current.next;
            Current = n;
            return res;
        }  

        public void AddToHead(T value)
        {
            // ���� � ������ ��� ��� ���������
            if (First == null && Current == null)
                First = Current = new GenListElement<T> { Element = value, next = null };
            // ���� ��� ���� ��������
            else
            {
                var tmp = First;
                First = new GenListElement<T> { Element = value, next = tmp };
            }
        }

        public void GoToHead()
        {
            if (First == null) throw new EmptyListException("� ��������� ������ ����������� ��������");
            Current = First;
        }
    }
}