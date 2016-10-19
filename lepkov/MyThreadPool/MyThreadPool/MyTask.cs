using System;

namespace MyThreadPool
{
    /// <summary>
    /// «адание. —одержит данные дл€ обработки и врем€ поступлени€ данных от генератора
    /// </summary>
    public class MyTask : IComparable
    {
        public GeneratedData Data { get; set; }
        public DateTime CreatedTime { get; set; }

        public int CompareTo(object obj)
        {
            var task = obj as MyTask;
            if (CreatedTime > task.CreatedTime)
                return 1;
            if (CreatedTime == task.CreatedTime)
                return 0;
            return -1;
        }
    }
}