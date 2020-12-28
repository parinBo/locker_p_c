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
               
                
                return View(box);
            }
            else
            {
                return Redirect("/login");
            }

        }
        [HttpGet]
        public IActionResult Index(DateTime date)
        {
            if (date.ToString() == "01/01/0001 00:00:00")
            {
                date = DateTime.Now;
            }

            var logon = HttpContext.Session.GetInt32("login");
            var Userid = HttpContext.Session.GetInt32("Userid");
            if (logon == 1)
            {

                ViewBag.today = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.addDays = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                TempData["username"] = HttpContext.Session.GetString("username");
                var box = _ctx.Boxs.ToList();
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
                manage.fit(box.ToList());
                manage.checkDateExp();
                boxes = manage.getMyLocker(Userid);
                foreach(var i in boxes)
            {
                ViewData["canUse"] = "true";
                ViewBag.Boxid = i.Boxid;
                var locker = _ctx.Boxs.Where(a => a.Boxid == i.Boxid).First();
                ViewBag.setCheckbox = locker.BoxCheck == 1 ? "true" : "false";
                ViewBag.pinBox = locker.Pin;
                if(i.Bookingstart.TimeOfDay <= DateTime.Now.TimeOfDay && i.Bookingstart.Day == DateTime.Now.Day)
                {
                    ViewData["canUse"] = "false";
                    _ctx.Boxs.Where(the => the.Boxid == i.Boxid).First().Userid = Userid;
                    _ctx.SaveChanges();
                }
            }

                
         
               
                
               /* _ctx.Boxs.Where(i => i.Boxid == use.First().Boxid).First().Userid = Userid;*/

                return View(boxes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Redirect("/");
            }


        }
        public IActionResult file()
        {
            return View();
        }
        [HttpGet]
        [Route("home/get")]
        public IActionResult file(string input,string type)
        {
            Console.WriteLine("TYPE : "+type);
            string[] lines = System.IO.File.ReadAllLines(Path.Combine("wwwroot", "Reservation.txt"));
            var find = new List<filelist>();
            int count = 0;
            foreach (var i in lines)
            {
                var n = i.Replace(",", "");
                var start = n.Substring(8, 19);
                var end = n.Substring(27, 19);
                var locker = n.Substring(46, 15);
                var num = n.Substring(60, 1);
                var at = n.Substring(65, 19);
                var user = n.Substring(87);
                if (type == "lockers")
                {
                    if (num == input)
                    {
                        find.Add(new filelist { start = start, end = end, date = at, by = user, locker = num });
                        count++;
                    }
                }
                if (type == "users")
                {
                    if (user == input)
                    {
                        find.Add(new filelist { start = start, end = end, date = at, by = user, locker = num });
                        count++;
                    }
                }
                if ("all" == input)
                {
                    find.Add(new filelist { start = start, end = end, date = at, by = user, locker = num });
                    count++;
                }


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
        public string time(int id,DateTime date)
        {
            if(date.ToString()== "01/01/0001 00:00:00")
            {
                date = DateTime.Now;
            }
            var times = _ctx.Boxtimes.Where(i => i.Boxid == id).Where(i => i.Bookingstart.Value.Day == date.Day);
            var data = times.ToList();
            var boss = new List<TimeEvent>();
            foreach (var i in data)
            {
                var usr = _ctx.Users.Where(res => res.Userid == i.Userid).First().Username;
                var s = ((DateTime)i.Bookingstart).ToString("dd/MM/yyyy HH:mm");
                var e = ((DateTime)i.BookingEnd).ToString("dd/MM/yyyy HH:mm");
                boss.Add(new TimeEvent { start = s, end = e, name = usr });
            }
             var jsonString = JsonConvert.SerializeObject(boss);
            return jsonString;
        }

    }
}
