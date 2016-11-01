using System;

namespace MyThreadPool
{
    class TaskReceivedEventArgs
    {
        public Action<int> Task { get; set; }
        public int param { get; set; }
        public TaskReceivedEventArgs(Action<int> task, int p)
        {
            Task = task;
            param = p;
        }
    }
}
