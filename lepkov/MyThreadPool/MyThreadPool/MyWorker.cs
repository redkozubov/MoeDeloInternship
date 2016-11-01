using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MyThreadPool
{
    public class MyWorker
    {
        protected Thread WorkerThread;
        protected MyTask CurrentTask;
        protected List<MyTask> TaskQueue;
        private ConcurrentQueue<TimeSpan> taskTimeSpanStatistic;
        private int taskStatisticCount;

        private object taskQueueLocker;
        private object taskStartTimeLocker;
        private object threadStartTimeLocker;
        private object executedTaskCountLocker;

        public bool IsWorking { get; protected set; }

        public DateTime TaskStartTime
        {
            get
            {
                lock (taskStartTimeLocker)
                { return taskStartTime; }
            }
            private set
            {
                lock (taskStartTimeLocker)
                { taskStartTime = value; }
            }
        }
        private DateTime taskStartTime;

        /// <summary>
        /// Время запуска потока.
        /// </summary>
        public DateTime ThreadStartTime
        {
            get
            {
                lock (threadStartTimeLocker)
                { return threadStartTime; }
            }
            private set
            {
                lock (threadStartTimeLocker)
                { threadStartTime = value; }
            }
        }
        private DateTime threadStartTime;


        /// <summary>
        /// Кол-во выполненных задач с момента текущего запуска потока
        /// </summary>
        public int ExecutedTaskCount
        {
            get
            {
                lock (executedTaskCountLocker)
                { return executedTaskCount; }
            }
            private set
            {
                lock (executedTaskCountLocker)
                { executedTaskCount = value; }
            }
        }
        private int executedTaskCount;


        public WorkerStates State { get; private set; }


        /// <summary>
        /// Количество задач в очереди, ожидающих выполнения
        /// </summary>
        public int TasksInQueueCount
        {
            get
            {
                lock (taskQueueLocker)
                { return TaskQueue.Count; }
            }
        }

        public MyWorker()
        {
            WorkerThread = new Thread(RunTasksExecution);
            TaskQueue = new List<MyTask>();
            taskStatisticCount = 10;
            taskTimeSpanStatistic = new ConcurrentQueue<TimeSpan>();
            IsWorking = false;

            taskStartTimeLocker = new object();
            threadStartTimeLocker = new object();
            executedTaskCountLocker = new object();
            taskQueueLocker = new object();
        }

        /// <summary>
        /// Воркер начинает обрабатывать задачи в очереди
        /// </summary>
        public void Start()
        {
            //Console.WriteLine("Запускаем поток");

            ExecutedTaskCount = 0;
            WorkerThread.Start();
            ThreadStartTime = DateTime.Now;
        }

        public void AddTasks(List<MyTask> tasks)
        {
            lock (taskQueueLocker)
            {
                tasks.ForEach(n => TaskQueue.Add(n));
                TaskQueue.Sort();
            }
            if (!WorkerThread.IsAlive)
            {
                switch (WorkerThread.ThreadState)
                {
                    case ThreadState.StopRequested:
                    case ThreadState.Stopped:
                        WorkerThread = new Thread(RunTasksExecution);
                        break;
                }
                Start();
            }
        }


        /// <summary>
        /// Запускаем выполнение задач из очереди
        /// </summary>
        private void RunTasksExecution()
        {
            MyTask task;
            IsWorking = true;
            while (true)
            {
                lock (taskQueueLocker)
                {
                    if (TaskQueue.Count > 0)
                    {
                        task = TaskQueue[0];
                        TaskQueue.Remove(task);
                    }
                    else break;
                }
                ExecuteTask(task);
                ExecutedTaskCount++;
            }

            IsWorking = false;
            ExecutedTaskCount = 0;
            ThreadStartTime = DateTime.MinValue;
        }

        /// <summary>
        /// Рабочий метод. Выполнение задачи
        /// </summary>
        /// <param name="myTask"></param>
        private void ExecuteTask(MyTask myTask)
        {
            TaskStartTime = DateTime.Now;
            //            Console.WriteLine("Приступили к выполнению задачи");
            Thread.Sleep(myTask.Data.ExecutionTime);
            //StatisticReg(DateTime.Now - TaskStartTime);
            TaskStartTime = DateTime.MinValue;
        }

        /// <summary>
        /// Добавляем запись о длительости задачи в статистику
        /// </summary>
        /// <param name="timeSpan"></param>
        private void StatisticReg(TimeSpan timeSpan)
        {
            if (taskTimeSpanStatistic.Count >= taskStatisticCount)
            {
                TimeSpan tmp;
                taskTimeSpanStatistic.TryDequeue(out tmp);
            }
            taskTimeSpanStatistic.Enqueue(timeSpan);
        }

        public List<MyTask> TakeTasks(int count)
        {
            var res = new List<MyTask>();
            lock (taskQueueLocker)
            {
                int index = Math.Max(0, TaskQueue.Count - count);
                int c = Math.Min(TaskQueue.Count, count);
                res = TaskQueue.GetRange(index, c);
                TaskQueue.RemoveRange(index, c);
            }
            return res;
        }

        public List<MyTask> TakeTasks()
        {
            lock (taskQueueLocker)
            {
                return TakeTasks(TaskQueue.Count);
            }
        }

        /// <summary>
        /// Долго выполняемый метод
        /// </summary>
        /// <param name="time"></param>
        private static void Slow(int time)
        {
            var end = DateTime.Now + TimeSpan.FromMilliseconds(time);
            while (DateTime.Now < end);
        }

    }
}