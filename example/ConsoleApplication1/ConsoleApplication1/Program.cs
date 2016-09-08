using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Delate(1, 0);
            Console.WriteLine(result);
            Console.ReadLine();
        }

        private static double Delate(int x, int y)
        {
            return x / y;
        }
    }
}
