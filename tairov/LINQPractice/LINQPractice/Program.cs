using System;
using System.Linq;

namespace LINQPractice
{
    class Program
    {
        static int Main()
        {
            var stationsInfo = new[]
            {
                new {Company = "DwarfOil", Cost = 2098, Mark = 92, Street = "Pirogova"},
                new {Company = "DwarfOil", Cost = 2200, Mark = 95, Street = "Pirogova"},
                new {Company = "DwarfOil", Cost = 2550, Mark = 98, Street = "Pirogova"},
                new {Company = "RobbedYou", Cost = 2898, Mark = 92, Street = "Pirogova"},
                new {Company = "RobbedYou", Cost = 3200, Mark = 95, Street = "Pirogova"},
                new {Company = "RobbedYou", Cost = 5050, Mark = 98, Street = "Pirogova"},
                new {Company = "Santa", Cost = 1800, Mark = 92, Street = "Pirogova"},
                new {Company = "Santa", Cost = 2300, Mark = 98, Street = "Pirogova"},

                new {Company = "DwarfOil", Cost = 2098, Mark = 92, Street = "Bulkina"},
                new {Company = "DwarfOil", Cost = 2200, Mark = 95, Street = "Bulkina"},
                new {Company = "DwarfOil", Cost = 2550, Mark = 98, Street = "Bulkina"},
                new {Company = "RobbedYou", Cost = 2898, Mark = 92, Street = "Bulkina"},
                new {Company = "RobbedYou", Cost = 3200, Mark = 95, Street = "Bulkina"},
                new {Company = "RobbedYou", Cost = 5050, Mark = 98, Street = "Bulkina"},

                new {Company = "RobbedYou", Cost = 2898, Mark = 92, Street = "Myasnaya"},
                new {Company = "RobbedYou", Cost = 3200, Mark = 95, Street = "Myasnaya"},
                new {Company = "RobbedYou", Cost = 5050, Mark = 98, Street = "Myasnaya"},
                new {Company = "Santa", Cost = 1800, Mark = 92, Street = "Myasnaya"},
                new {Company = "Santa", Cost = 2300, Mark = 98, Street = "Myasnaya"},


                new {Company = "DwarfOil", Cost = 2098, Mark = 92, Street = "Kulebyaka"},
                new {Company = "DwarfOil", Cost = 2200, Mark = 95, Street = "Kulebyaka"},
                new {Company = "DwarfOil", Cost = 2550, Mark = 98, Street = "Kulebyaka"},
                new {Company = "RobbedYou", Cost = 3200, Mark = 95, Street = "Kulebyaka"},
                new {Company = "RobbedYou", Cost = 5050, Mark = 98, Street = "Kulebyaka"},
                new {Company = "Santa", Cost = 1800, Mark = 92, Street = "Kulebyaka"},
                new {Company = "Santa", Cost = 2300, Mark = 98, Street = "Kulebyaka"},
                new {Company = "GrayPay", Cost = 2098, Mark = 92, Street = "Kulebyaka"},
                new {Company = "GrayPay", Cost = 2200, Mark = 95, Street = "Kulebyaka"},
                new {Company = "GrayPay", Cost = 2550, Mark = 98, Street = "Kulebyaka"},
            };

            var streetsInfo = from stationEntity in stationsInfo 
                              group stationEntity by new { stationEntity.Company, stationEntity.Street }
                                  into streetEntityStations
                                  group streetEntityStations by streetEntityStations.Key.Street
                                      into groupedStations
                                      orderby groupedStations.Key
                                      select new
                                      {
                                          Street = groupedStations.Key,
                                          Count = groupedStations.Count()
                                      };
            // TODO методами расширения
            foreach (var street in streetsInfo)
            {
                Console.WriteLine(street.Street + " " + street.Count);
            }
            return 0;
        }
    }
}
