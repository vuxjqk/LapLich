using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class Program
    {
        public static List<Day> GetValidDays(int year, int month)
        {
            List<Day> days = new List<Day>();
            int numDays = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= numDays; day++)
            {
                Day d = new Day(day, new DateTime(year, month, day));
                days.Add(d);
            }

            return days;
        }

        public static void Main()
        {
            var days = CSVReader.ReadDays();
            var doctors = CSVReader.ReadDoctors();
            var rooms = CSVReader.ReadRooms();

            //foreach (var item in doctors)
            //    Debug.WriteLine(item);
            //foreach (var item in rooms)
            //    Debug.WriteLine(item);
            //foreach (var item in days)
            //    Debug.WriteLine(item);

            GeneticAlgorithm ga = new GeneticAlgorithm(doctors, rooms, days);
            var bestSchedule = ga.Run();
            CSVReader.WriteScheduleToCsv(bestSchedule);
            Debug.WriteLine("Lịch đã được ghi vào tệp schedule.csv.");
        }
    }
}