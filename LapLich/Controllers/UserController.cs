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
            var date = CSVReader.ReadDays()[0].Date;
            var startOfWeek = Program.GetStartOfWeek(date, weekIndex);
            var endOfWeek = startOfWeek.AddDays(6);

            var doctorScheduleForWeek = schedule
                .Where(s => s.Doctor.DoctorID == id && s.Day.Date >= startOfWeek && s.Day.Date <= endOfWeek)
                .OrderBy(s => s.Day.Date)
                .ToList();

            ViewBag.Year = date.Year;
            ViewBag.Month = date.Month;
            ViewBag.Week = weekIndex;
            ViewBag.LastWeek = Program.GetWeeksInMonth(date);
            ViewBag.Name = CSVReader.ReadDoctors().Find(d => d.DoctorID == id).DoctorName;

            return View(doctorScheduleForWeek);
        }

        public ActionResult userProfile(int id = -1)
        {
            var user = CSVReader.ReadUsers().SingleOrDefault(u => u.UserID == id);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }
    }
}