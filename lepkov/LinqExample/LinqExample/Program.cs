using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqExample
{
    class Program
    {
        static void Main(string[] args) //todo : rename
        {
            // Заполняем список АЗС
            var gasStationListList = new GasStation[]
            {
                new GasStation("Лукойл", "Рахова", 3450, 92 ),
                new GasStation("ТНК", "Вавилова", 3500, 92),
                new GasStation("Лукойл", "Чапаева", 3550, 95),
                new GasStation("Торэко", "Чапаева", 3650, 92),
                new GasStation("Лукойл", "Совесткая", 3545, 95),
                new GasStation("ТатНефть", "Тверская", 3520, 92),
                new GasStation("ТНК", "Рахова", 3550, 98),
                new GasStation("Премье", "Ямская", 3470, 98)
            };


            Console.WriteLine("\nФормируем LINQ запрос:");

            // группируем по названию улицы
            var streetList = from gas in gasStationListList
                group gas by gas.Street
                into street //todo : street
                         orderby street.Key                                         // соритруем по названию улицы 
                         select new {Name = street.Key, GasCount = street.Count()};     // создаём список пар { Улица, Кол-во_АЗС}



            foreach (var s in streetList)
            {
                Console.WriteLine("На улице {0} \tрасполагается {1} АЗС.", s.Name, s.GasCount);
            }

            // todo: методы расширения 

            streetList =
                gasStationListList.GroupBy(n => n.Street)
                    .OrderBy(n => n.Key)
                    .Select(n => new {Name = n.Key, GasCount = n.Count()});

            Console.WriteLine("\nЗапрос выполнен с помощью методов расширения:");

            foreach (var s in streetList)
            {
                Console.WriteLine("На улице {0} \tрасполагается {1} АЗС.", s.Name, s.GasCount);
            }



            Console.ReadLine();
        }
    }
}
