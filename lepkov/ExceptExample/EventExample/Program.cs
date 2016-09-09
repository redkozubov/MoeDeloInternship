using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventExample
{
    delegate void PingPongEventHandler( int count); 

    class PingPongEventArgs : EventArgs
    {
        //public Curr
    }


    class Ping
    {
        public event PingPongEventHandler PingEvent;

        public void OnPingEvent( int count)
        {
            PingEvent?.Invoke(count);
        }

        public void PongHandler( int count)
        {
            Console.WriteLine("Ping получил сообщение от Pong. Шаг: " + count);
            if (count < 5)
            {
                count++;
                OnPingEvent( count);
            }
        }
    }


    class Pong
    {
        public event PingPongEventHandler PongEvent;

        public void OnPongEvent( int count)
        {
            PongEvent?.Invoke(count);
        }

        public void PingHandler( int count)
        {
            Console.WriteLine("Pong получил сообщение от Ping. Шаг: " + count);
            if (count < 5)
            {
                count++;
                OnPongEvent( count);
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Ping pingOb = new Ping();
            Pong pongOb = new Pong();

            pingOb.PingEvent += pongOb.PingHandler;
            pongOb.PongEvent += pingOb.PongHandler;
            
            pingOb.OnPingEvent(1);
  /*          pongOb.OnPongEvent( c);
            pingOb.OnPingEvent( c);
            pongOb.OnPongEvent( c);
            pingOb.OnPingEvent( c);
*/
            Console.ReadLine();
        }
    }
}
