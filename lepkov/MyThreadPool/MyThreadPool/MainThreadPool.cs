using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

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
        protected Queue<MyWorker> WorkersExcess; // сюда перемещаем воркеры, которым нужно доделатьсвою задачу. 
        private Timer chekWorkersTimer;
        private int maxExecutionTimeMs;
        private int maxWorkersCount;
        private int minWorkersCount;
        int chekPeriod;
        private object workersLocker = new object();

        public  MainThreadPool ()
        {
            Workers = new List<MyWorker>();
            for (int i = 0; i < 3; i++)
            {
                Workers.Add(new MyWorker());
            }
            WorkersExcess = new Queue<MyWorker>();
            maxExecutionTimeMs = 200;
            minWorkersCount = 2;
            maxWorkersCount = 15;
            chekWorkersTimer = new Timer(maxExecutionTimeMs/5);
            chekWorkersTimer.Elapsed += ChekWorkersTimerOnElapsed;
            chekWorkersTimer.Start();
        }


        public MainThreadPool(IDataGenerator generator) : this()
        {
            generator.DataGeneratedEvent += GeneratorOnDataGeneratedEvent;
        }

        /// <summary>
        /// обрабатываем появление новых заданий от генератора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataGeneratedEventArgs"></param>
        public void GeneratorOnDataGeneratedEvent(object sender, DataGeneratedEventArgs dataGeneratedEventArgs)
        {
            Console.WriteLine("{0} Получены задачи от генератора. {1} задач", DateTime.Now.ToString("hh: mm:ss.fff tt"),
                dataGeneratedEventArgs.Data.Count());
           
            var taskList =
                dataGeneratedEventArgs.Data.Select(n => new MyTask {Data = n, CreatedTime = DateTime.Now}).ToList();
            AddTasks(taskList, Workers);
        }
        private void ChekWorkersTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            ChekWorkers();
        }
        
        private void ChekWorkers()
        {
            int totalExecutedTaskCount = 0;
            int totalTaskTime = 0;
            int executedTaskCount = 0;
            int totalTasksInQueueCount = 0;
            DateTime startTime;

            lock (workersLocker)
            {
                Workers.ForEach(worker =>
                {
                    startTime = worker.ThreadStartTime;
                    executedTaskCount = worker.ExecutedTaskCount;

                    // время пошло, значит процесс уже запущен
                    if (startTime > DateTime.MinValue)
                    {
                        totalTaskTime += (int) (DateTime.Now - startTime).TotalMilliseconds;
                        totalExecutedTaskCount += executedTaskCount > 0 ? executedTaskCount : 1; // Если счётчик пока в нуле, значит обрабатываем первую задачу
                        totalTasksInQueueCount = worker.TasksInQueueCount;
                    }
                });

                // если сейчас что-то обрабатывается
                if (totalExecutedTaskCount > 0)
                {
                    int middleExecutionTime = totalTaskTime/totalExecutedTaskCount;

                    if (chekPeriod >= 10)
                    {
                        Console.WriteLine("Среднее время выоплнения задачи {0}", middleExecutionTime);
                        //WriteStatistic();
                        chekPeriod = 0;
                    }
                    chekPeriod ++;

                    // рассчётное количество необходимых воркеров
                    double needdedWorkersCount = (int) ((middleExecutionTime*totalTasksInQueueCount)/maxExecutionTimeMs);

                    // Нижняя и верхняя границы количества воркеров
                        // нижняя граница, на 10% больше рассчётного кол-ва
                    int minNeeddedWorkersCount = (int) Math.Ceiling(needdedWorkersCount*1.1);
                        // верхняя граница, на 60% больше рассчётного кол-ва
                    int maxNeeddedWorkersCount = (int) Math.Ceiling(needdedWorkersCount*1.6);

                    // проверить, чтобы количество воркеров было в пределах допустимых значений
                    minNeeddedWorkersCount = Math.Max(minWorkersCount, Math.Min(maxWorkersCount, minNeeddedWorkersCount));
                    maxNeeddedWorkersCount = Math.Max(minWorkersCount, Math.Min(maxWorkersCount, maxNeeddedWorkersCount));

                    // если воркеров меньше, то добавляем новых
                    if (Workers.Count < minNeeddedWorkersCount)
                    {
                        var needCount = (minNeeddedWorkersCount - Workers.Count);

                        // берём воркеры из очереди, ожидающей удаления
                        int getFromExcessCount = Math.Min(needCount, WorkersExcess.Count);
                        for (int i = 0; i < getFromExcessCount; i++)
                        {
                            Workers.Add(WorkersExcess.Dequeue());
                        }

                        Console.WriteLine("{0} ДОБАВЛЯЕМ воркеры {1}", DateTime.Now.ToString("hh: mm:ss.fff tt"), needCount);
                        // если не хватило, добавляем новые
                        for (int i = 0; i < needCount; i++)
                            Workers.Add(new MyWorker());
                        WriteStatistic();
                    }

                    List<MyTask> tasksForWorkers = new List<MyTask>();
                    // если больше , то перемещаем воркеры в очередь на удаление и забираем их задачи
                    if (Workers.Count > maxNeeddedWorkersCount)
                    {
                        //var tempWorkers = new List<MyWorker>();
                        for (int i = Workers.Count - 1; i >= maxNeeddedWorkersCount; i--)
                        {
                            var w = Workers[i];
                            tasksForWorkers.AddRange(w.TakeTasks());
                            WorkersExcess.Enqueue(w);
                            Workers.Remove(w);
                        }
                    }
                    AddTasks(tasksForWorkers, Workers);
                }

                
                ChekWorkersExcessQueue();
            }
        }

        private void WriteStatistic()
        {
            lock (workersLocker)
            {
                Console.WriteLine("{0} Всего запущего обработчиков {1}, Задач в ожидании {2}", DateTime.Now.ToString("hh: mm:ss.fff tt"),
                    Workers.Count + WorkersExcess.Count, Workers.Sum(w => w.TasksInQueueCount));
                //foreach (MyWorker worker in Workers)
                //    Console.Write("{0} \t", worker.TasksInQueueCount);
            }
        }

        /// <summary>
        /// Удаляем из очереди на удаление воркеры, которые завершили свои задачи
        /// </summary>
        private void ChekWorkersExcessQueue()
        {
            int deleted = 0;
            int count = WorkersExcess.Count;

            for (int i = 0; i < count; i++)
            {
                var worker = WorkersExcess.Dequeue();
                if (worker.IsWorking)
                {
                    WorkersExcess.Enqueue(worker);
                }
                else
                {
                    deleted ++;
                }
            }
            
            if (deleted > 0)
            {
                Console.WriteLine("{0} УДАЛЕНИЕ воркеров {1}", DateTime.Now.ToString("hh: mm:ss.fff tt"), deleted);
                WriteStatistic();
            }
        }

        private void AddTasks(List<MyTask> taskList, List<MyWorker> workers)
        {

            lock (workersLocker)
            {
                int totalTaskCount = workers.Sum(n => n.TasksInQueueCount) + taskList.Count;

                if (workers.Count == 0)
                {
                    for (int i = 0; i < minWorkersCount; i++)
                    {
                        workers.Add(new MyWorker());
                    }
                }

                //Console.WriteLine("Респределяем задачи по воркерам.");
                int taskRange = taskList.Count/workers.Count; // сколько задач отдадим каждому воркеру
                /*  int taskExcess = taskList.Count%workers.Count; // счётчик избытка задач

                for (int i = 0; i < workers.Count; i++)
                {
                    int index = i*taskRange;

                    // добавляем воркерам задач равными порциями и добавляем ещё по 1 задаче, если есть избыток задач.
                    workers[i].AddTasks(taskList.GetRange(index, taskRange + ((taskExcess--) > 0 ? 1 : 0)));
                }
                //Console.WriteLine("Задачи распределены.");
               */
               AddTasks(taskList, workers, taskRange);
            }
        }

        /// <summary>
        /// Распределяет получинные задачи и задачи , которые уже есть в воркерах, в соответствии с переданным значиением workerCapacity.
        /// Ёмкость воркеров workerCapacity должна быть рассчитана верно, иначе задачи могут не уместиться в воркеры. Использовать на свой страх и риск. 
        /// </summary>
        /// <param name="taskList">Задачи, которые будет добавлены на обработку воркерам</param>
        /// <param name="workers">Список воркеров. принимающих задачи.</param>
        /// <param name="workerCapacity">Максимальное количество задач, которое будет содержаться в воркере</param>
        private void AddTasks(List<MyTask> taskList, List<MyWorker> workers, int workerCapacity)
        {
            var ligthWorkers = new Queue<MyWorker>();   // воркеры которым не досталось задач при первом проходе
            var tasks = new List<MyTask>();             // сюда будем собирать излишки задач из перегруженных воркеров
            int taskToWorkerCount;

            workers.ForEach(worker =>
            {
                var taskInQueue = worker.TasksInQueueCount;
                // если воркер перегружен
                if (taskInQueue > workerCapacity)
                {
                    var taskExcess = worker.TakeTasks(taskInQueue - workerCapacity);
                    taskList.AddRange(taskExcess);
                }
                // если в воркер можно добавить задач
                else
                {
                    taskToWorkerCount = Math.Min(workerCapacity - taskInQueue, taskList.Count);
                    worker.AddTasks(taskList.GetRange(0, taskToWorkerCount));
                    taskList.RemoveRange(0, taskToWorkerCount);
                    if(worker.TasksInQueueCount < workerCapacity)
                    { ligthWorkers.Enqueue(worker);}
                }
            });
            while (taskList.Count > 0 && ligthWorkers.Count >0)
            {
                var worker = ligthWorkers.Dequeue();
                taskToWorkerCount = Math.Min(workerCapacity - worker.TasksInQueueCount, taskList.Count);
                worker.AddTasks(taskList.GetRange(0, taskToWorkerCount));
                taskList.RemoveRange(0, taskToWorkerCount);
            }
            WriteStatistic();
        }

    }

    public enum WorkerStates
    {
        TaskInProces,
        TaskQueueIsEmpty
    }
}


