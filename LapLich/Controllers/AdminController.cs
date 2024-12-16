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
            return View();
        }

        public ActionResult DoctorSchedule()
        {
            return View(CSVReader.ReadSchedule());
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
                doctor.DaysOff = CSVReader.ReadDays().Where(d => selectedDays.Contains(d.DayID)).ToList();
                CSVReader.WriteDoctorsToCsv(doctors);
            }
            return RedirectToAction("DaysOff");
        }
    }
}