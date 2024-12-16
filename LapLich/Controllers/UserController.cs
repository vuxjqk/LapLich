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
            ViewBag.LastWeek = GetWeeksInMonth(date);

            // Lọc lịch bác sĩ cho tuần tương ứng
            var startOfWeek = GetStartOfWeek(date, weekIndex);
            var endOfWeek = startOfWeek.AddDays(6);
            var doctorScheduleForWeek = schedule
                .Where(s => s.Doctor.DoctorID == id && s.Day.Date >= startOfWeek && s.Day.Date <= endOfWeek)
                .OrderBy(s => s.Day.Date)
                .ToList();

            return View(doctorScheduleForWeek);
        }

        private DateTime GetStartOfWeek(DateTime date, int weekIndex)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            var daysToFirstMonday = (int)DayOfWeek.Monday - (int)firstDayOfMonth.DayOfWeek;
            if (daysToFirstMonday > 0)
            {
                daysToFirstMonday -= 7;
            }

            var firstMonday = firstDayOfMonth.AddDays(daysToFirstMonday);

            var startOfWeek = firstMonday.AddDays((weekIndex - 1) * 7);

            return startOfWeek;
        }

        public int GetWeeksInMonth(DateTime date)
        {
            // Lấy ngày đầu tiên và cuối cùng của tháng
            int year = date.Year;
            int month = date.Month;

            // Lấy ngày đầu tiên của tháng
            DateTime firstDayOfMonth = new DateTime(year, month, 1);

            // Lấy ngày cuối cùng của tháng
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Lấy thứ trong tuần của ngày đầu tháng (để biết tuần bắt đầu từ ngày nào)
            int firstDayOfWeek = (int)firstDayOfMonth.DayOfWeek;

            // Nếu ngày đầu tháng là Chủ Nhật (0), thì tính là tuần 1
            firstDayOfWeek = (firstDayOfWeek == 0) ? 7 : firstDayOfWeek;

            // Tính tổng số tuần trong tháng
            int totalDaysInMonth = lastDayOfMonth.Day;
            int weeksInMonth = (totalDaysInMonth + firstDayOfWeek - 1) / 7 + 1;

            return weeksInMonth;
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