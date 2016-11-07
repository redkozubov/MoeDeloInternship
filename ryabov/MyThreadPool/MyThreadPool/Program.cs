using System;
using System.Threading.Tasks;

namespace MyThreadPool
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = new Task(() =>
            {
                DataGenerator dg = new DataGenerator();
                long threadWaitTime = 2000;
                double growthQuotient = 0.25;
                int threadNumber = 5;
                MyThreadPool pool = new MyThreadPool(dg.standartExecutionTime,threadNumber,threadWaitTime);
                dg.DataGeneratedEvent += (sender, dataGeneratedEventArgs) =>
                {
                    Console.WriteLine(($"{DateTime.Now:hh:mm:ss.fff tt} Сгенерировано задание"));
                    pool.RaiseEvent(Slow,dg.standartExecutionTime);
                };
                dg.Start();
            });
            task.Start();
            Console.ReadLine();
        }

        public static void Slow(int time)
        {
            var end = DateTime.Now + TimeSpan.FromMilliseconds(time);
            while (DateTime.Now < end)
                ;
        }
    }
}
