using LapLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class Schedule
    {
        public Doctor Doctor { get; set; }
        public Room Room { get; set; }
        public Day Day { get; set; }

        public Schedule(Doctor doctor, Room room, Day day)
        {            
            Doctor = doctor;
            Room = room;
            Day = day;
        }

        public override string ToString()
        {
            string doctorStr = Doctor != null ? Doctor.DoctorName : "No Doctor Assigned";
            string roomStr = Room != null ? Room.RoomName : "No Room Assigned";
            string dayStr = Day != null ? Day.Date.ToString("dd/MM/yyyy") : "No Day Assigned";

            return $"Doctor: {doctorStr}, Room: {roomStr}, Day: {dayStr}";
        }
    }
}