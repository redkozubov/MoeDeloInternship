using System.Threading;
using System.Diagnostics;
using System;
using System.Collections.Concurrent;

namespace MyThreadPool
{
    class MyThreadPool
    {
        private int threadsNumber;
        private long threadWaitTime;
        private Stopwatch timer;
        private int maxExecutionTimeMs;

        private ConcurrentQueue<Action<int>> taskQueue;
        private ConcurrentQueue<long> timeQueue;
        private readonly ConcurrentQueue<int> paramQueue;

        private delegate void TaskReceivedEventHandler(object sender, TaskReceivedEventArgs e);

        private event TaskReceivedEventHandler TaskRecievedEvent;

        public MyThreadPool(int maxExTime, int threadNumber, long waitTime)
        {
            maxExecutionTimeMs = maxExTime;
            threadWaitTime = waitTime;
            taskQueue = new ConcurrentQueue<Action<int>>();
            timeQueue = new ConcurrentQueue<long>();
            paramQueue = new ConcurrentQueue<int>();
            TaskRecievedEvent += OnRecievingTask;
            timer = new Stopwatch();
            timer.Start();
            CreateThreads(threadNumber);
            Thread additionalThreadsCreator = new Thread(QueueObserver);
            additionalThreadsCreator.Start();
        }

        public void RaiseEvent(Action<int> a, int param)
        {
            TaskRecievedEvent?.Invoke(this, new TaskReceivedEventArgs(a, param));
        }

        private void OnRecievingTask(object sender, TaskReceivedEventArgs e)
        {
            taskQueue.Enqueue(e.Task);
            timeQueue.Enqueue(timer.ElapsedMilliseconds);
            paramQueue.Enqueue(e.param);
        }

        private void CreateThreads(int thrNum)
        {
            for (int i = 0; i < thrNum; ++i)
            {
                Thread currentThread = new Thread(ThreadEndlessCycle);
                Interlocked.Increment(ref threadsNumber);
                currentThread.Start();
            }
            Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff tt} Создано {thrNum} потоков");
        }

        private void ThreadEndlessCycle()
        {
            long waitStarted = timer.ElapsedMilliseconds;
            while (true)
            {
                Action<int> currentAction;
                int currentParam;
                long currentTaskTime;
                taskQueue.TryDequeue(out currentAction);
                paramQueue.TryDequeue(out currentParam);
                timeQueue.TryDequeue(out currentTaskTime);
                if (currentAction != null)
                {
                    Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff tt} Задача начата.");
                    currentAction(currentParam);
                    Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff tt} Задача завершена.");
                    waitStarted = timer.ElapsedMilliseconds;
                }
                else
                {
                    if (timer.ElapsedMilliseconds - waitStarted <= threadWaitTime)
                        continue;
                    Interlocked.Decrement(ref threadsNumber);
                    Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff tt} Поток удалён.");
                    return;
                }
            }
        }

        private void QueueObserver()
        {
            while (true)
            {
                long firstTaskTime;
                long currentTime = timer.ElapsedMilliseconds;
                timeQueue.TryPeek(out firstTaskTime);
                while (currentTime - firstTaskTime > maxExecutionTimeMs)
                {
                    timeQueue.TryDequeue(out firstTaskTime);
                    timeQueue.TryPeek(out firstTaskTime);
                }
                if (timeQueue.Count > threadsNumber)
                {
                    CreateThreads(timeQueue.Count - threadsNumber);
                }
            }
        }
    }
}