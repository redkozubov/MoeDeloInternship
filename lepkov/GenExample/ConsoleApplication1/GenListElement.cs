namespace GenProj
{
    class GenListElement <T> where T: struct
    {
        /// <summary>
        /// �������� �������� ��������
        /// </summary>
        public T Element;
        /// <summary>
        /// ������ �� ��������� �������
        /// </summary>
        public GenListElement<T> next;
    }
}