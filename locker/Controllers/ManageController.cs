using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using locker.Models;
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

        internal List<Boxtime> getMyLocker(int? userid)
        {
            throw new NotImplementedException();
        }
    }
}
