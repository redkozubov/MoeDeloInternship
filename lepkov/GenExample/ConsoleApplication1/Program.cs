using System;
using GenProj;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new GenList<int>();

            for (int i = 1; i < 5; i++)
            {
                list.AddToHead(i * 2);
                Console.WriteLine("Добавлено значение :" + i * 2);
            }

            list.GoToHead();
            try
            {

                for (int i = 1; i < 100; i++)
                {
                    Console.WriteLine("Значение элемента " + i + ": " + list.GetNext());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine(); 
        }
    }
}
