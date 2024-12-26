using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CsvHelper.Configuration;
using CsvHelper;

namespace LapLich.Models
{
    public class CSVReader
    {
        static string file = HttpContext.Current.Server.MapPath("~/Csv/");

        public static List<Room> ReadRooms()
        {
            string filePath = file + "rooms.csv";
            var rooms = new List<Room>();
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    int RoomID = Convert.ToInt32(record.RoomID);
                    string RoomName = record.RoomName;
                    rooms.Add(new Room(RoomID, RoomName));
                }
            }
            return rooms;
        }

        public static List<Day> ReadDays()
        {
            string filePath = file + "days.csv";
            var days = new List<Day>();
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    int DayID = Convert.ToInt32(record.DayID);
                    DateTime Date = Convert.ToDateTime(record.Date);
                    days.Add(new Day(DayID, Date));
                }
            }
            return days;
        }

        public static List<Doctor> ReadDoctors()
        {
            string filePath = file + "doctors.csv";
            var doctors = new List<Doctor>();
            var rooms = ReadRooms();
            var days = ReadDays();

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    int DoctorID = Convert.ToInt32(record.DoctorID);
                    string DoctorName = record.DoctorName;

                    var AllowedRooms = new List<Room>();
                    foreach (var room in record.AllowedRooms.ToString().Split(' '))
                    {
                        int RoomID = Convert.ToInt32(room);
                        Room Room = rooms.Find(r => r.RoomID == RoomID);
                        AllowedRooms.Add(Room);
                    }

                    var DaysOff = new List<Day>();
                    foreach (var day in record.DaysOff.ToString().Split(' '))
                    {
                        if (string.IsNullOrEmpty(day))
                            break;
                        int DayID = Convert.ToInt32(day);
                        Day Day = days.Find(d => d.DayID == DayID);
                        DaysOff.Add(Day);
                    }

                    doctors.Add(new Doctor(DoctorID, DoctorName, AllowedRooms, DaysOff));
                }
            }
            return doctors;
        }

        public static List<Schedule> ReadSchedule()
        {
            string filePath = file + "schedule.csv";
            var schedule = new List<Schedule>();
            var rooms = ReadRooms();
            var days = ReadDays();
            var doctors = ReadDoctors();

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    int DoctorID = Convert.ToInt32(record.DoctorID);
                    int RoomID = Convert.ToInt32(record.RoomID);
                    int DayID = Convert.ToInt32(record.DayID);

                    var Doctor = doctors.Find(d => d.DoctorID == DoctorID);
                    var Room = rooms.Find(r => r.RoomID == RoomID);
                    var Day = days.Find(d => d.DayID == DayID);

                    schedule.Add(new Schedule(Doctor, Room, Day));
                }
            }
            return schedule;
        }

        public static List<User> ReadUsers()
        {
            string filePath = file + "user.csv";
            var users = new List<User>();
            var doctors = ReadDoctors();

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
            {
                var records = csv.GetRecords<dynamic>();
                foreach (var record in records)
                {
                    int UserID = Convert.ToInt32(record.UserID);
                    string UserName = record.UserName;
                    string Password = record.Password;
                    string Role = record.Role;

                    var Doctor = doctors.Find(d => d.DoctorID == UserID);

                    users.Add(new User(UserID, UserName, Password, Role, Doctor));
                }
            }
            return users;
        }

        public static void WriteDaysToCsv(List<Day> days)
        {
            string filePath = file + "days.csv";
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("DayID,Date");

                foreach (var entry in days)
                {
                    writer.WriteLine($"{entry.DayID},{entry.Date}");
                }
            }
        }

        public static void WriteDoctorsToCsv(List<Doctor> doctors)
        {
            string filePath = file + "doctors.csv";
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("DoctorID,DoctorName,AllowedRooms,DaysOff");

                foreach (var entry in doctors)
                {
                    writer.WriteLine($"{entry.DoctorID},{entry.DoctorName},{string.Join(" ", entry.AllowedRooms.Select(r => r.RoomID))},{string.Join(" ", entry.DaysOff.Select(d => d.DayID))}");
                }
            }
        }

        public static void WriteScheduleToCsv(List<Schedule> schedule)
        {
            string filePath = file + "schedule.csv";
            using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                writer.WriteLine("DoctorID,RoomID,DayID");

                foreach (var entry in schedule)
                {
                    writer.WriteLine($"{entry.Doctor.DoctorID},{entry.Room.RoomID},{entry.Day.DayID}");
                }
            }
        }
    }
}