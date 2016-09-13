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
        public event EventHandler<Counter> PingEvent;

        public void OnPingEvent(Counter arg)
        {
            if (PingEvent != null)
            {
                PingEvent(this, arg);
            }
        }
        public void PingNext(object s, Counter arg)
        {
            ++arg.Count;
            Console.WriteLine("Ping recieved Pong, value = " + arg.Count);
            Thread.Sleep(1000);
            OnPingEvent(arg);
        }
    }
    public class Pong
    {
        public event EventHandler<Counter> PongEvent;

        public void OnPongEvent(Counter arg)
        {
            if (PongEvent != null)
            {
                PongEvent(this, arg);
            }
        }

        public void PongNext(object s, Counter arg)
        {
            ++arg.Count;
            Console.WriteLine("Pong recieved Ping, value = " + arg.Count);
            Thread.Sleep(1000);
            OnPongEvent(arg);
        }
    }
    class Program
    {
        static int Main()
        {
            Ping pingObj = new Ping();
            Pong pongObj = new Pong();
            Counter pingPongCounter = new Counter();
            pongObj.PongEvent += pingObj.PingNext;
            pingObj.PingEvent += pongObj.PongNext;
            pongObj.OnPongEvent(pingPongCounter);
            return 0;
        }
    }
}
