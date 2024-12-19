using LapLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapLich.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rooms()
        {
            return View(CSVReader.ReadRooms());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(int year, int month)
        {
            var days = Program.GetValidDays(year, month);
            CSVReader.WriteDaysToCsv(days);

            var doctors = CSVReader.ReadDoctors();
            foreach (var dortor in doctors)
                dortor.DaysOff = new List<Day>();

            CSVReader.WriteDoctorsToCsv(doctors);
            CSVReader.WriteScheduleToCsv(new List<Schedule>());
            return View();
        }

        public ActionResult DoctorSchedule(int weekIndex = 1)
        {
            var days = CSVReader.ReadDays();
            ViewBag._Year = days[0].Date.Year;
            ViewBag._Month = days[0].Date.Month;

            var schedule = CSVReader.ReadSchedule();
            var date = days[0].Date;

            ViewBag.Year = date.Year;
            ViewBag.Month = date.Month;
            ViewBag.Week = weekIndex;
            ViewBag.LastWeek = Program.GetWeeksInMonth(date);

            // Lọc lịch bác sĩ cho tuần tương ứng
            var startOfWeek = Program.GetStartOfWeek(date, weekIndex);
            ViewBag.StartOfWeek = startOfWeek;

            var endOfWeek = startOfWeek.AddDays(6);
            var doctorScheduleForWeek = schedule
                .Where(s => s.Day.Date >= startOfWeek && s.Day.Date <= endOfWeek)
                .OrderBy(s => s.Day.Date)
                .ToList();

            return View(doctorScheduleForWeek);
        }

        public ActionResult _DoctorSchedule()
        {
            Program.Main();
            return RedirectToAction("DoctorSchedule");
        }

        public ActionResult DaysOff(int doctorId = 1)
        {
            ViewBag.Doctor = CSVReader.ReadDoctors().Find(d => d.DoctorID == doctorId);
            ViewBag.Days = CSVReader.ReadDays();
            return View();
        }

        [HttpPost]
        public ActionResult SaveDaysOff(int doctorId, List<int> selectedDays)
        {
            var doctors = CSVReader.ReadDoctors();
            var doctor = doctors.FirstOrDefault(d => d.DoctorID == doctorId);
            if (doctor != null)
            {
                if (selectedDays == null)
                    selectedDays = new List<int>();
                doctor.DaysOff = CSVReader.ReadDays().Where(d => selectedDays.Contains(d.DayID)).ToList();
                CSVReader.WriteDoctorsToCsv(doctors);
            }
            return RedirectToAction("DaysOff", "Admin", new { doctorId = doctorId });
        }
    }
}