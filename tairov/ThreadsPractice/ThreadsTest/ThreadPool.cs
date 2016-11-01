using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Timer = System.Threading.Timer;

namespace ThreadsPractice
{
    class ThreadPool
    {
        public class Worker
        {
            public Thread OperationalThread;
            private readonly ThreadPool pool;
            private volatile bool cancellationFlag;
            private const int SleepingTime = 10;
            public Worker(ThreadPool pool)
            {
                this.pool = pool;
                cancellationFlag = false;
                OperationalThread = new Thread(ExecuteThread)
                {
                    IsBackground = true
                };
                OperationalThread.Start();
            }

            public void ThreadStopRequest()
            {
                cancellationFlag = true;
            }

            public void ExecuteThread()
            {
                while (!cancellationFlag)
                {
                    while (pool.tasks.IsEmpty)
                    { Thread.Sleep(SleepingTime); }
                    if (cancellationFlag)
                        break;
                    ThreadStart currentTask;
                    bool isNew = !pool.taskCompleteFlag;
                    if (pool.tasks.TryDequeue(out currentTask))
                    {
                        currentTask.Invoke();
                        if (isNew)
                            pool.taskCompleteFlag = true;
                    }
                }
                Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff tt")} Удален поток, всего {pool.workers.Count} потоков");
            }
        }

        private readonly ConcurrentQueue<Worker> workers;
        private readonly ConcurrentQueue<ThreadStart> tasks;
        private readonly ConcurrentStack<DateTime> removingStack;
        private readonly int maxThreads;
        private readonly int minThreads;
        private readonly int holdingTime;
        private volatile bool taskCompleteFlag;
        private static readonly object LockObj = new object();
        private const int DefaultHoldingTime = 3000;

        public ThreadPool(int minThreads, int maxThreads, int holdingTime = DefaultHoldingTime)
        {
            this.minThreads = minThreads;
            this.maxThreads = maxThreads;
            this.holdingTime = holdingTime;
            taskCompleteFlag = false;
            workers = new ConcurrentQueue<Worker>();
            tasks = new ConcurrentQueue<ThreadStart>();
            removingStack = new ConcurrentStack<DateTime>();
            for (int i = 0; i < minThreads; ++i)
                workers.Enqueue(new Worker(this));
            RemoveThreads();
        }

        private void RemoveThreads()
        {
            new Timer(state =>
            {
                Queue<DateTime> tempQueue = new Queue<DateTime>();
                DateTime currentTime = DateTime.Now;
                while (!removingStack.IsEmpty)
                {
                    DateTime dt;
                    bool f = removingStack.TryPop(out dt);
                    if (!f)
                        continue;
                    TimeSpan dif = currentTime - dt;
                    if (dif.TotalMilliseconds >= holdingTime)
                    {
                        Worker stoppedWorker;
                        workers.TryDequeue(out stoppedWorker);
                        stoppedWorker?.ThreadStopRequest();
                    }
                    else
                        tempQueue.Enqueue(dt);
                }
                foreach (var dt in tempQueue)
                {
                    removingStack.Push(dt);
                }
            }, null, 0, holdingTime);
        }
        public void AddTasks(IEnumerable<ThreadStart> newTasks, int timeLimit)
        {
            Stopwatch localTaskTimer = Stopwatch.StartNew();
            lock (LockObj)
            {
                while (!tasks.IsEmpty)
                {
                    Thread.Sleep(10);
                }
                if (localTaskTimer.ElapsedMilliseconds > timeLimit)
                    return;
                taskCompleteFlag = false;
                foreach (ThreadStart t in newTasks) 
                {
                    tasks.Enqueue(t);
                }
                Stopwatch taskExecutionTime = Stopwatch.StartNew();
                bool timeIsFound = false;
                ManualResetEvent timerResetEvent = new ManualResetEvent(false);
                Timer manageTimer = null;
                manageTimer = new Timer(state =>
                {
                    int timeLeft = timeLimit - (int) localTaskTimer.ElapsedMilliseconds;
                    if (!tasks.IsEmpty)
                    {
                        if (taskCompleteFlag && !timeIsFound)
                        {
                            taskExecutionTime.Stop();
                            timeIsFound = true;
                        }
                        int approximateTime = (int) taskExecutionTime.ElapsedMilliseconds;

                        int requiredThreads;
                        if (timeLeft > 0)
                            requiredThreads = Math.Min(tasks.Count*approximateTime/timeLeft + 1, maxThreads);
                        else
                            requiredThreads = maxThreads;
                        while (workers.Count < requiredThreads)
                        {
                            workers.Enqueue(new Worker(this));
                            Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff tt")} Создан поток, всего {workers.Count} потоков");
                            DateTime t;
                            if (!removingStack.IsEmpty)
                                removingStack.TryPop(out t);
                        }
                        while (!removingStack.IsEmpty && workers.Count - removingStack.Count < requiredThreads)
                        {
                            DateTime t;
                            if (!removingStack.IsEmpty)
                                removingStack.TryPop(out t);
                        }
                        while (workers.Count - removingStack.Count > Math.Max(requiredThreads, minThreads))
                        {
                            removingStack.Push(DateTime.Now);
                        }
                    }
                    else
                    {
                        manageTimer?.Change(Timeout.Infinite, Timeout.Infinite);
                        ManualResetEvent stopEvent = (ManualResetEvent) state;
                        stopEvent.Set();
                    }
                },
                    timerResetEvent,
                    0,
                    50);

                timerResetEvent.WaitOne();
                localTaskTimer.Stop();
                if (taskExecutionTime.IsRunning)
                    taskExecutionTime.Stop();
                if (localTaskTimer.ElapsedMilliseconds < timeLimit)
                    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff tt")} Задачи выполнены в срок");
                else
                    Console.WriteLine($"{DateTime.Now.ToString("hh:mm:ss.fff tt")} Задачи выполнены с опозданием");
                timerResetEvent.Dispose();
                manageTimer.Dispose();
            }
        }
    }
}
