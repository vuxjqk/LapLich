using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class Day
    {
        public int DayID { get; set; }
        public DateTime Date { get; set; }

        public Day(int dayID, DateTime date)
        {
            DayID = dayID;
            Date = date;
        }

        public override string ToString()
        {
            return $"DayID: {DayID}, Date: {Date.ToString("dd/MM/yyyy")}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Day))
                return false;

            Day otherDay = (Day)obj;
            return DayID == otherDay.DayID;
        }

        public override int GetHashCode()
        {
            return DayID.GetHashCode();
        }
    }
}