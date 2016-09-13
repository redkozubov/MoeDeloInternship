using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace generics_task
{
    class Program
    {
        static void Main(string[] args)
        {
            ValueLinkedList<int> intList = new ValueLinkedList<int>();
            try
            {
                intList.moveToHead();
                Console.WriteLine("Первый элемент: {0}", intList.peekCurrent());
            }
            catch (EmptyListException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                int temp = intList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
            }
            catch (EmptyListException e)
            {
                Console.WriteLine(e.Message);
            }
            intList.addToHead(1);
            intList.addToHead(2);
            try
            {
                intList.moveToPrevious();
            }
            catch (ListRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                int temp = intList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
                intList.moveToNext();
                temp = intList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
                intList.moveToNext();
            }
            catch (ListRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (EmptyListException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                int temp = intList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
                intList.moveToPrevious();
                temp = intList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
            }
            catch (ListRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (EmptyListException e)
            {
                Console.WriteLine(e.Message);
            }
            ValueLinkedList<char> charList = new ValueLinkedList<char>();
            charList.addToHead('a');
            charList.addToHead('b');
            try
            {
                char temp = charList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
                charList.moveToNext();
                temp = charList.peekCurrent();
                Console.WriteLine("Текущий элемент: {0}", temp);
            }
            catch (ListRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (EmptyListException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}