using LapLich.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LapLich.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Dichvu()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var users = CSVReader.ReadUsers();
            var user = users.Find(u => u.UserName == username && u.Password == password);
            if (user != null)
            {
                if (user.Role == "admin")
                    return RedirectToAction("Index", "Admin");
                else
                {
                    Session["UserID"] = user.UserID;
                    Session["User"] = user;
                    return RedirectToAction("Index", "User", new { id = user.UserID });
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            Session["User"] = null;
            return RedirectToAction("Index");
        }
    }
}