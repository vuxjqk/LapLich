using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
        public List<Room> AllowedRooms { get; set; }
        public List<Day> DaysOff { get; set; }

        public Doctor(int doctorID, string doctorName, List<Room> allowedRooms, List<Day> daysOff)
        {
            DoctorID = doctorID;
            DoctorName = doctorName;
            AllowedRooms = allowedRooms;
            DaysOff = daysOff;
        }

        public override string ToString()
        {
            string allowedRoomsStr = string.Join(", ", AllowedRooms.Select(r => r.RoomName));
            string daysOffStr = string.Join(", ", DaysOff.Select(d => d.Date.ToString("dd/MM/yyyy")));

            return $"DoctorID: {DoctorID}, DoctorName: {DoctorName}, AllowedRooms: [{allowedRoomsStr}], DaysOff: [{daysOffStr}]";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Doctor))
                return false;

            Doctor otherDoctor = (Doctor)obj;
            return DoctorID == otherDoctor.DoctorID;
        }

        public override int GetHashCode()
        {
            return DoctorID.GetHashCode();
        }
    }
}