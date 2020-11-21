using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using locker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace locker.Controllers
{
    public class BookingController : Controller
    {
        private readonly Contxtdb ctx = new Contxtdb();

        // GET: /<controller>/
        public IActionResult Index(int id)
        {
            var logon = HttpContext.Session.GetInt32("login")==1?true:false;
            if (logon)
            {
                ManageController m = new ManageController();
                var show = new List<showclass>();
                var Userid = HttpContext.Session.GetInt32("Userid");
                var box = ctx.Boxtimes.FromSqlInterpolated($"select * from Boxtimes where Boxid={id}");
                var boxOrder =box.OrderBy(aa => aa.Bookingstart).ToList();
                ViewData["id"] = id;
                TempData["username"] = HttpContext.Session.GetString("username");
                
                m.fit(box.ToList());
                m.checkDateExp();
                m.fit(boxOrder);
                show = m.getDetailBoxAt(id);

                return View(show);
            }
            else
            {
                return Redirect("/");
            }
            
        }

        public IActionResult Reserve(int id)
        {
            TempData["id"] = id;
            TempData["username"] = HttpContext.Session.GetString("username");
            var has = HttpContext.Session.GetInt32("has");
            var Userid = HttpContext.Session.GetInt32("Userid");
            var d = DateTime.Now.ToString("yyyy/MM/dd");
            Console.WriteLine("BookingController : Reserve");
            ViewBag.today = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.addDays = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            ViewBag.show = TempData["show"];
            ViewBag.msg = TempData["msg"];
            if(has==1){
                ViewBag.notReservation = "true";
                TempData["notMsg"]="you cannot reservation";
                ViewBag.notMsg = "you cannot reservation";
            }
            return View();
        }

        [HttpPost]
        public IActionResult authPost(Booking b)
        {
            DateTime reservation = b.startdate;
            DateTime s, e;
            var Userid = HttpContext.Session.GetInt32("Userid");
            int id = (int)TempData["id"];
            if (Regex.IsMatch(b.starttime, @"\b8"))
            {
                 s = new DateTime(reservation.Year,reservation.Month,reservation.Day,8, 0, 0);
                 e = new DateTime(reservation.Year, reservation.Month, reservation.Day, 12, 0, 0);
            }
            else
            {
                 s = new DateTime(reservation.Year, reservation.Month, reservation.Day, 13, 0, 0);
                 e = new DateTime(reservation.Year, reservation.Month, reservation.Day, 17, 0, 0);

            }
            var book = ctx.Boxtimes.Where(u=>u.Bookingstart == s).Where(b=>b.Boxid==id).Count();
            if(book>=1){
                TempData["show"] = "true";
                TempData["msg"]= "this time is busy";
                Console.WriteLine("you cannot booking");
                return RedirectToAction("Reserve",new {id=id});
            }else{
                // add box and change status use has box
                Boxtime boxtime = new Boxtime { Bookingstart = s, BookingEnd = e, Boxid = id,Userid=Userid};
                ctx.Boxtimes.Add(boxtime);
                ctx.Users.Where(t=>t.Userid == Userid).First().Has=1;
                ctx.SaveChanges();
            }
           
            return RedirectToAction("index",new { id= id });
        }
    }
}
