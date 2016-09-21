using System;

namespace GenericsPractice
{
    class Program
    {
        static int Main()
        {
            MyList<int> intList = new MyList<int>(new[] {1, 2, 3, 4, 5});
            intList.Add(17);
            intList.AddToHead(19);
            intList.GoToHead();
            Console.WriteLine(intList.CurrentElement);
            try
            {
                for (int i = 0; i < 10; ++i)
                {
                    Console.WriteLine(intList.GetNext());
                }
            }
            catch (NoMoreElementsException e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }
    }
}
