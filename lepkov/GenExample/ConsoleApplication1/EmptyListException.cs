using System;

namespace GenProj
{
    /// <summary>
    /// ������������� ��� ������� �������� ������� ������� ������
    /// </summary>
    class EmptyListException : Exception
    {
        public EmptyListException (string message): base(message) { }
    }
}