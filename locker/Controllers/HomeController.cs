using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using locker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace locker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Contxtdb _ctx = new Contxtdb();
        ManageController manage = new ManageController();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public IActionResult Index()
        {
            var logon = HttpContext.Session.GetInt32("login");
            var Userid = HttpContext.Session.GetInt32("Userid");
            if (logon == 1)
            {

                var has = (int)_ctx.Users.Where(u => u.Userid == Userid).First().Has;
                var box = _ctx.Boxs.ToList();
                var boxToday = _ctx.Boxtimes.Where(w => w.Bookingstart.Value.Date == DateTime.Now.Date);
                var loopBox = boxToday.ToList();

                HttpContext.Session.SetInt32("has", has);
                TempData["username"] = HttpContext.Session.GetString("username");
                ViewBag.today = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.addDays = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

                manage.fit(loopBox);
                manage.checkDateExp();
                box = manage.getBoxDate(DateTime.Now);
                Console.WriteLine("Home controller : index");


                return View(box);
            }
            else
            {
                return Redirect("/login");
            }

        }

        public IActionResult searchdate(DateTime date)
        {
            var logon = HttpContext.Session.GetInt32("login");
            var Userid = HttpContext.Session.GetInt32("Userid");
            if (logon == 1)
            {
                ViewBag.today = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.addDays = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                TempData["username"] = HttpContext.Session.GetString("username");
                var box = _ctx.Boxs.ToList();
                Console.WriteLine("searchdate : " + date);
                manage.fit(_ctx.Boxtimes.ToList());
                manage.checkDateExp();
                box = manage.getBoxDate(date);
                return View(box);
            }
            else
            {
                return Redirect("/login");
            }

        }

        public IActionResult Mylockers()
        {
            try
            {
                TempData["username"] = HttpContext.Session.GetString("username");
                var Userid = (int)HttpContext.Session.GetInt32("Userid");
                var box = _ctx.Boxtimes.Where(b => b.Userid == Userid);
                var boxes = new List<showclass>();
                int Boxid = 0;
                string set = "";
                manage.fit(box.ToList());
                manage.checkDateExp();
                boxes = manage.getMyLocker(Userid);
                foreach (var i in boxes)
                {
                    ViewData["canUse"] = i.username;
                    ViewBag.Boxid = i.Boxid;
                    set = _ctx.Boxs.Where(a => a.Boxid == i.Boxid).First().BoxCheck == 1 ? "true" : "false";
                    ViewBag.pinBox = _ctx.Boxs.Where(a => a.Boxid == i.Boxid).First().Pin;
                }
                ViewBag.setCheckbox = set;
                return View(boxes);
            }
            catch
            {
                return Redirect("/");
            }
            
        }
        public IActionResult file()
        {
            return View();
        }

        [HttpGet]
        [Route("home/get")]
        public IActionResult file(string input)
        {
            string[] lines = System.IO.File.ReadAllLines(Path.Combine("wwwroot", "Reservation.txt"));
            var find = new List<string>();
            int count = 0;
            foreach (var i in lines)
            {
                var n = i.Replace(",", "");
                var start = n.Substring(8, 19);
                var end = n.Substring(27, 19);
                var locker = n.Substring(48, 15);
                var num = n.Substring(62, 1);
                var at = n.Substring(63, 22);
                var user = n.Substring(85);
                if (num == input)
                {
                    find.Add(locker);
                    find.Add($"{at} ==>start: {start} end: {end} // {user}");
                    count++;
                }
                if ("all" == input)
                {
                    find.Add(locker);
                    find.Add($"{at} ==>start: {start} end: {end}  // {user}");
                    count++;
                }
                Console.WriteLine($"{locker}\nstart: {start} and end: {end}  {num} {at} {user}");
            }

            if (input != null)
            {
                ViewBag.count = count;
                return View(find);
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public JsonResult time(int id)
        {
            var times = _ctx.Boxtimes.Where(i => i.Boxid == id).Where(i => i.Bookingstart.Value.Day == DateTime.Now.Day);
            //  var usersList = new List<TimeEvent> { new TimeEvent { start=0,end=30},new TimeEvent { start = 90, end = 180 } };
            Dictionary<string, string> d = new Dictionary<string, string>();
            var a = new List<Dictionary<string, string>>();
            foreach(var i in times)
            {
                if (Regex.IsMatch(i.Bookingstart.Value.Hour.ToString(), @"\b8"))
                {

                }
                    Console.WriteLine(i.Bookingstart);
            }
            var boss = new List<TimeEvent>();
            boss.Add(new TimeEvent { start = 0, end = 30, });
            boss.Add(new TimeEvent { start = 30, end = 90, });
            d.Add("start", "240");
            d.Add("end", "360");
            d.Add("name", "test");
            a.Add(d);

            //return boss;

            var jsonString = JsonConvert.SerializeObject(boss).Replace("\u0022", "");
            return Json(jsonString);
        }

    }
}
