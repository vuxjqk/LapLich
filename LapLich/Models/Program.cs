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

        static public DateTime GetStartOfWeek(DateTime date, int weekIndex)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var daysToFirstMonday = (int)DayOfWeek.Monday - (int)firstDayOfMonth.DayOfWeek;
            if (daysToFirstMonday > 0)
                daysToFirstMonday -= 7;

            var firstMonday = firstDayOfMonth.AddDays(daysToFirstMonday);
            var startOfWeek = firstMonday.AddDays((weekIndex - 1) * 7);

            return startOfWeek;
        }

        static public int GetWeeksInMonth(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            firstDayOfWeek = (firstDayOfWeek == 0) ? 7 : firstDayOfWeek;

            int totalDaysInMonth = lastDayOfMonth.Day;
            int weeksInMonth = (totalDaysInMonth + firstDayOfWeek - 1) / 7 + 1;

            return weeksInMonth;
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