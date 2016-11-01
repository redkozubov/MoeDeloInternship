using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;


namespace ThreadsPractice
{
    public class Program
    {
        private static ThreadPool _pool;
        private static int timeLimit = 500; // TimeLimit for tasks, ms
        static void Main()
        {
            InitPool();
            Init();
        }

        public static void InitPool()
        {
            Console.WriteLine("[Опционально] Задание параметров пула: int minThreads, int maxThreads, int holdingTime");
            var readLine = Console.ReadLine();
            if (readLine != null)
            {
                string[] inp = readLine.Split();
                int minThreads = 2;
                int maxThreads = 6;
                int holdingTime = 3000;
                if (inp.Length == 3)
                {
                    var minThreadsExtracted = int.TryParse(inp[0], out minThreads);
                    var maxThreadsExtracted = int.TryParse(inp[1], out maxThreads);
                    var holdingTimeExtracted = int.TryParse(inp[2], out holdingTime);
                    if (!minThreadsExtracted)
                        minThreads = 2;
                    if (!maxThreadsExtracted)
                        maxThreads = 6;
                    if (!holdingTimeExtracted)
                        holdingTime = 3000;
                }
                _pool = new ThreadPool(minThreads, maxThreads, holdingTime);
            } 
            else
                _pool = new ThreadPool(2, 6);
        }
        public static void Init()
        {
            var task = new Task(() =>
            {
                // Параллельно, иначе консоль закроется
                var dataGenerator = new DataGenerator();
                dataGenerator.DataGeneratedEvent += (sender, eventArgs) =>
                {
                    ProcessData(eventArgs.Data);
                };
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
            //string n = DateTime.Now.ToString("hh:mm:ss.fff tt");
            var generatedDatas = data as GeneratedData[] ?? data.ToArray();
            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff tt")} Получено {generatedDatas.Count()} элементов ");
            _pool.AddTasks(generatedDatas.Select(genTask => (ThreadStart) (() => Slow(genTask.ExecutionTime))).ToList(), timeLimit);
        }
 
        /// <summary>
        /// Долго выполняемый метод
        /// </summary>
        /// <param name="time"></param>
        private static void Slow(int time)
        {
            var end = DateTime.Now + TimeSpan.FromMilliseconds(time);
            while (DateTime.Now < end)
            {
            }
        }
    }
 
    public class DataGenerator
    {
        public delegate void DataGeneratedEventHandler(object sender, DataGeneratedEventArgs e);
 
        /// <summary>
        /// Появление новых данных на обработку
        /// </summary>
        public event DataGeneratedEventHandler DataGeneratedEvent;
 
        public readonly Random Rand = new Random();
 
        private Timer dataGenerationTimer;
 
        private readonly int minTimerInterval = 500;
 
        private readonly int maxTimerInterval = 4000;
 
        private readonly int minTaskTimeMs = 30;
 
        private readonly int maxTimerTimeMs = 1000;
 
 
        private readonly int standartExecutionTime = 50;
 
        public void Start()
        {
            dataGenerationTimer = new Timer();
            dataGenerationTimer.Elapsed += TimerEventProcessor;
            dataGenerationTimer.Interval = Rand.Next(minTimerInterval, maxTimerInterval); 
            dataGenerationTimer.Start();
        }
 
 
 
        private void RaiseDataGeneratedEvent(IEnumerable<GeneratedData> data)
        {
            // Raise the event by using the () operator.
            DataGeneratedEvent?.Invoke(this, new DataGeneratedEventArgs(data));
        }
 
        private void TimerEventProcessor(object myObject, EventArgs myEventArgs)
        {
            int fortimerinterval = Rand.Next(minTimerInterval, maxTimerInterval);
            dataGenerationTimer.Interval = fortimerinterval;
 
            var data = GenerateData();
            RaiseDataGeneratedEvent(data);
        }
 
        private IEnumerable<GeneratedData> GenerateData()
        {
            var count = Rand.Next(0, 200);
            var result = new List<GeneratedData>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new GeneratedData() { ExecutionTime = standartExecutionTime });
            }
            return result;
        }
    }
 
    public class GeneratedData
    {
        public int ExecutionTime;
    }
 
    public class DataGeneratedEventArgs
    {
        public DataGeneratedEventArgs(IEnumerable<GeneratedData> data)
        {
            Data = data;
        }
 
        public IEnumerable<GeneratedData> Data { get; set; }
    }
}





