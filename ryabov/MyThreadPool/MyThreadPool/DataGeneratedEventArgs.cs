using System.Collections.Generic;

namespace MyThreadPool
{
    public class DataGeneratedEventArgs
    {
        public DataGeneratedEventArgs(IEnumerable<GeneratedData> data)
        {
            Data = data;
        }

        public IEnumerable<GeneratedData> Data { get; set; }
    }
}
