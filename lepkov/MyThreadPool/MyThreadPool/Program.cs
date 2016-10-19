using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyThreadPool
{
    public class Program
    {
        static void Main(string[] args)
        {
            Init();
        }

        public static void Init()
        {
            var task = new Task(() =>
            {
                // Параллельно, иначе консоль закроется
                var dataGenerator = new DataGenerator();
                new MainThreadPool(dataGenerator);

                //dataGenerator.DataGeneratedEvent += threadPool.GeneratorOnDataGeneratedEvent;

                //(sender, eventArgs) =>
                //{
                //    ProcessData(eventArgs.Data);
                //};
                dataGenerator.Start();
            });
            task.Start();
            Console.ReadLine();
        }

        /// <summary>
        /// Обработка полученных от генератора данных
        /// </summary>
        /// <param name="data"></param>
        public static void ProcessData(IEnumerable<GeneratedData> data)
        {
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff tt")} Получено {data.Count()} элементов ");
            // ToDo вызывать Slow в параллельной среде.
        }

        /// <summary>
        /// Долго выполняемый метод
        /// </summary>
        /// <param name="time"></param>
        private static void Slow(int time)
        {
            var end = DateTime.Now + TimeSpan.FromMilliseconds(time);
            while (DateTime.Now < end)
                ;
        }
    }
}

