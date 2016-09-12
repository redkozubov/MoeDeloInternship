using System;
using System.Threading;


namespace EventsPractice
{
    public class Counter : EventArgs
    {
        public int Count;
    }
    public class Ping
    {
        private int count;
        public event EventHandler<Counter> PingEvent;

        public void OnPingEvent()
        {
            Counter c = new Counter();
            if (PingEvent != null)
            {
                c.Count = ++count;
                PingEvent(this, c);
            }
        }

        public void Handler(object s, Counter arg)
        {
            count = arg.Count;
            Console.WriteLine("Ping recieved Pong, value = " + arg.Count);
            Thread.Sleep(1000);
            OnPingEvent();
        }
    }
    public class Pong
    {
        private int count;
        public event EventHandler<Counter> PongEvent;

        public void OnPongEvent()
        {
            Counter c = new Counter();
            if (PongEvent != null)
            {
                c.Count = ++count;
                PongEvent(this, c);
            }
        }

        public void Handler(object s, Counter arg)
        {
            count = arg.Count;
            Console.WriteLine("Pong recieved Ping, value = " + arg.Count);
            Thread.Sleep(1000);
            OnPongEvent();
        }
    }
    class Program
    {
        static int Main()
        {
            Ping pingObj = new Ping();
            Pong pongObj = new Pong();
            pongObj.PongEvent += pingObj.Handler;
            pingObj.PingEvent += pongObj.Handler;
            pongObj.OnPongEvent();
            return 0;
        }
    }
}
