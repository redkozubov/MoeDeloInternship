using System;

namespace GenProj
{
    /// <summary>
    /// ������������� ��� ������� �������� ��������� ������� ������, ����� ���� ������� �� �����
    /// </summary>
    class EmptyNextElementException : Exception
    {
        public EmptyNextElementException (string message): base(message) { }
    }
}