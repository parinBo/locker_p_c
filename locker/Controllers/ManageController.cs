using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using locker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace locker.Controllers
{
    public class ManageController : Controller
    {
        private readonly Contxtdb _ctx = new Contxtdb();
        private List<Boxtime> boxTime;
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        
        public void fit(List<Boxtime> box)
        {
            this.boxTime = box;
        }

        public List<Box> getBoxDate(DateTime dateTime)
        {
            var BOX = _ctx.Boxs.ToList();
            Console.WriteLine("getBoxDate  : "+dateTime.Date);
            var b = _ctx.Boxtimes.Where(a => a.Bookingstart.Value.Date == dateTime.Date);
            var bb = b.ToList();
            foreach (var i in bb)
            {
                var count = b.Where(qq => qq.Boxid == i.Boxid).Count();
                if (count == 1)
                {
                    _ctx.Boxs.Where(qq => qq.Boxid == i.Boxid).First().Boxstatus = 1;
                }
                else if (count == 2)
                {
                    _ctx.Boxs.Where(qq => qq.Boxid == i.Boxid).First().Boxstatus = 2;
                }
            }
            return BOX;
        }

        public List<showclass> getMyLocker(int Userid)
        {
            var show = new List<showclass>();
            var has = _ctx.Users.Where(i => i.Userid == Userid).First().Has;
            Console.WriteLine("My locker Manage: " + has);
            if (has == 1)
            {
                var i = this.boxTime.First();
                //check today you can use open/close the box : 0 = have not, : 1 = have
                var use = _ctx.Boxtimes.Where(i => i.Userid == Userid).Where(u => u.Bookingstart.Value.Date == DateTime.Now.Date);
                string username = "true";
                if (use.Count() != 0)
                {
                    username = (!(use.First().Bookingstart.Value.TimeOfDay <= DateTime.Now.TimeOfDay)).ToString().ToLower();
                    Console.WriteLine(use.First().Bookingstart);
                }
                Console.WriteLine(username+"  ");
                show.Add(new showclass {Bookingstart=(DateTime)i.Bookingstart,BookingEnd=(DateTime)i.BookingEnd,username=username,Boxid=(int)i.Boxid });
            }
            return show;
        }

        public List<showclass> getDetailBoxAt(int id)
        {
            var show = new List<showclass>();
            var boxAtId = this.boxTime.Where(a => a.Boxid == id).OrderBy(a => a.Bookingstart).ToList();
            foreach (var i in boxAtId)
            {

                var u = _ctx.Users.Where(qq => qq.Userid == i.Userid).First().Username;
                showclass s = new showclass { Bookingstart = (DateTime)i.Bookingstart, BookingEnd = (DateTime)i.BookingEnd, username = u };
                show.Add(s);
            }
            return show;
        }

        public void checkDateExp()
        {
            var tt = this.boxTime.Where(qq => qq.BookingEnd <= DateTime.Now).ToList();
            foreach(var i in tt)
            {
                _ctx.Remove(i);
                _ctx.Users.Where(u => u.Userid == i.Userid).First().Has = 0;
            }
            _ctx.SaveChanges();
            //return boxTime;
        }

        [HttpPost]
        [Route("booking/authPost")]
        public IActionResult authPost(Booking b)
        {
            DateTime reservation = b.startdate;
            DateTime s, e;
            var Userid = HttpContext.Session.GetInt32("Userid");
            int id = (int)TempData["id"];
            if (Regex.IsMatch(b.starttime, @"\b8"))
            {
                s = new DateTime(reservation.Year, reservation.Month, reservation.Day, 8, 0, 0);
                e = new DateTime(reservation.Year, reservation.Month, reservation.Day, 12, 0, 0);
            }
            else
            {
                s = new DateTime(reservation.Year, reservation.Month, reservation.Day, 13, 0, 0);
                e = new DateTime(reservation.Year, reservation.Month, reservation.Day, 17, 0, 0);

            }
            var book = _ctx.Boxtimes.Where(u => u.Bookingstart == s).Where(b => b.Boxid == id).Count();
            if (book >= 1)
            {
                TempData["show"] = "true";
                TempData["msg"] = "this time is busy";
                Console.WriteLine("you cannot booking");
                return RedirectToAction("Reserve", new { id = id });
            }
            else
            {
                var u = _ctx.Users.Where(i => i.Userid == Userid).First().Username;
                using (StreamWriter outputFile = new StreamWriter(Path.Combine("wwwroot", "Reservation.txt"), true))
                {
                    outputFile.WriteLine($"Booking:{s.ToString("dd/MM/yyyy HH:mm:ss")},{e.ToString("dd/MM/yyyy HH:mm:ss")},{Userid},Locker number {id},At {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")},By {u}");
                }
                // add box and change status use has box
                Boxtime boxtime = new Boxtime { Bookingstart = s, BookingEnd = e, Boxid = id, Userid = Userid };
                _ctx.Boxtimes.Add(boxtime);
                _ctx.Users.Where(t => t.Userid == Userid).First().Has = 1;
                _ctx.SaveChanges();
            }

            return Redirect("/home/Mylockers");
        }

        [HttpPut]
        [Route("home/authLocker/{id}")]
        public string Put(int id, int value)
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
        public string delete(int id)
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
