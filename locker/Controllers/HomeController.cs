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

        [HttpPut]
        [Route("home/authLocker/{id}")]
        public string Put(int id,int value)
        {
            try
            {
                var Userid = HttpContext.Session.GetInt32("Userid");
                var boxTime = _ctx.Boxtimes.Where(i => i.Bookingstart.Value.Date == DateTime.Now.Date).Where(i => i.Userid == Userid).First();
                _ctx.Boxs.Where(i => i.Boxid == boxTime.Boxid).First().BoxCheck = value;
                _ctx.SaveChanges();
                return "Put Method is work " + value;
            }
            catch
            {
                return "error";
            }
            
        }

        [HttpDelete]
        [Route("home/delete/{id}")]
        public string delete(int id )
        {
            var Userid = (int)HttpContext.Session.GetInt32("Userid");
            var cancel = _ctx.Boxtimes.Where(u => u.Userid == Userid).Where(u => u.Boxid == id).First();
            _ctx.Users.Where(i => i.Userid == Userid).First().Has = 0;
            _ctx.Boxtimes.Remove(cancel);
            _ctx.SaveChanges();
            return id + "";
        }
    }
}
