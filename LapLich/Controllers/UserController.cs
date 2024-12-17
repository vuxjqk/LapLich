using LapLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapLich.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(int id, int weekIndex = 1)
        {
            var schedule = CSVReader.ReadSchedule();
            var doctor = schedule.Find(s => s.Doctor.DoctorID == id).Doctor;
            ViewBag.Name = doctor.DoctorName;

            var date = schedule[0].Day.Date;
            ViewBag.Year = date.Year;
            ViewBag.Month = date.Month;
            ViewBag.Week = weekIndex;
            ViewBag.LastWeek = Program.GetWeeksInMonth(date);

            // Lọc lịch bác sĩ cho tuần tương ứng
            var startOfWeek = Program.GetStartOfWeek(date, weekIndex);
            var endOfWeek = startOfWeek.AddDays(6);
            var doctorScheduleForWeek = schedule
                .Where(s => s.Doctor.DoctorID == id && s.Day.Date >= startOfWeek && s.Day.Date <= endOfWeek)
                .OrderBy(s => s.Day.Date)
                .ToList();

            return View(doctorScheduleForWeek);
        }

        public ActionResult userProfile(int id)
        {
            var doctor = CSVReader.ReadDoctors().SingleOrDefault(d => d.DoctorID == id);
            var users = CSVReader.ReadUsers();
            ViewBag.User = users.Find(u => u.UserID == doctor.DoctorID);
            return View(doctor);
        }
    }
}