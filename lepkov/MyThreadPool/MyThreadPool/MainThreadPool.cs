using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyThreadPool
{
    /// <summary>
    /// Принимает список задач и организует их выполнение в многопоточном режиме
    /// </summary>
    public class MainThreadPool
    {
        /// <summary>
        /// очередь задач на выполнение
        /// </summary>
        private Queue<MyTask> taskQueue { get; set; }


        protected List<MyWorker> Workers;

        public void Init(DataGenerator generator)
        {
            generator.DataGeneratedEvent += GeneratorOnDataGeneratedEvent;
            Workers = new List<MyWorker>();
            for (int i = 0; i < 5; i++)
            {
                Workers.Add(new MyWorker());
            }
        }

        /// <summary>
        /// обрабатываем появление новых заданий от генератора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataGeneratedEventArgs"></param>
        public void GeneratorOnDataGeneratedEvent(object sender, DataGeneratedEventArgs dataGeneratedEventArgs)
        {
            Console.WriteLine("Получены задачи от генератора. {0} задач", dataGeneratedEventArgs.Data.Count());
            var taskList =
                dataGeneratedEventArgs.Data.Select(n => new MyTask {Data = n, CreatedTime = DateTime.Now}).ToList();
            AddTasks(taskList);
        }


        private void AddTasks(List<MyTask> taskList)
        {
            Console.WriteLine("Респределяем задачи по воркерам.");
//            taskList.ForEach(n=> taskQueue.Enqueue(n));
            int taskRange = taskList.Count/Workers.Count;
            int taskExcess = taskList.Count%Workers.Count;
            for (int i = 0; i < Workers.Count; i++)
            {
                Workers[i].AddTasks(taskList.GetRange(i*taskRange, taskRange + (taskExcess-->0?1:0)));
            }
            Console.WriteLine("Задачи распределены.");
            Console.WriteLine("Всего запущего обработчиков {0}", Workers.Count);
            foreach(MyWorker worker in Workers)
                Console.Write("{0} \t", worker.TasksInQueueCount);
            Console.WriteLine();
        }


    }

    /// <summary>
    /// Задание. Содержит данные для обработки и время поступления данных от генератора
    /// </summary>
    public class MyTask
    {
        public GeneratedData Data { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class MyWorker
    {
        protected Thread WorkerThread;
        protected MyTask CurrentTask;
        protected ConcurrentQueue<MyTask> TaskQueue;
        private Queue<TimeSpan> taskTimeSpanStatistic;
        private int taskStatisticCount;
             

        public int TasksInQueueCount => TaskQueue.Count;

        public MyWorker()
        {
            WorkerThread = new Thread(RunQueueWorking);
            TaskQueue = new ConcurrentQueue<MyTask>();
            taskStatisticCount = 10;
            taskTimeSpanStatistic = new Queue<TimeSpan>(taskStatisticCount);
        }

        /// <summary>
        /// Воркер начинает обрабатывать задачи в очереди
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Запускаем процесс");
            WorkerThread.Start();
        }

        public void AddTasks(List<MyTask> tasks)
        {
            tasks.ForEach(n => TaskQueue.Enqueue(n));
            if (!WorkerThread.IsAlive)
            {
                switch (WorkerThread.ThreadState)
                {
                    case ThreadState.StopRequested:
                    case ThreadState.Stopped: WorkerThread = new Thread(RunQueueWorking);
                        break;
                }
                Start();
            }
        }

        /// <summary>
        /// Рабочий метод. Выполнение задачи
        /// </summary>
        /// <param name="myTask"></param>
        private void ExecuteTask(MyTask myTask)
        {
            var beginTime = DateTime.Now;
//            Console.WriteLine("Приступили к выполнению задачи");
            Thread.Sleep(myTask.Data.ExecutionTime);
            StatisticReg(DateTime.Now - beginTime);
        }

        /// <summary>
        /// Добавляем запись о длительости задачи в статистику
        /// </summary>
        /// <param name="timeSpan"></param>
        private void StatisticReg(TimeSpan timeSpan)
        {
            if (taskTimeSpanStatistic.Count >= taskStatisticCount)
                taskTimeSpanStatistic.Dequeue();
            taskTimeSpanStatistic.Enqueue(timeSpan);
        }
        /// <summary>
        /// Запускаем выполнение задач из очереди
        /// </summary>
        private void RunQueueWorking()
        {
            MyTask task;
            while (TaskQueue.Count > 0)
            {
                if (TaskQueue.TryDequeue(out task))
                    ExecuteTask(task);
            }
        }
    }
}


