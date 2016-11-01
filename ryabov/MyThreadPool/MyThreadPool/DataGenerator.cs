using System;
using System.Collections.Generic;
using System.Timers;

namespace MyThreadPool
{
    public class DataGenerator
    {
        public delegate void DataGeneratedEventHandler(object sender, DataGeneratedEventArgs e);

        /// <summary>
        /// Появление новых данных на обработку
        /// </summary>
        public event DataGeneratedEventHandler DataGeneratedEvent;

        public readonly Random Rand = new Random();

        private Timer dataGenerationTimer;

        private readonly int minTimerInterval = 20;

        private readonly int maxTimerInterval = 4000;

        private readonly int minTaskTimeMs = 30;

        private readonly int maxTimerTimeMs = 1000;


        public readonly int standartExecutionTime = 50;

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
}
