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
            var b = _ctx.Boxtimes.Where(a => a.Bookingstart.Value.Date == dateTime.Date);
            var bb = b.ToList();
            var li = new List<int>();
            var libox = new List<int> { 1, 2, 3, 4, 5, 6 };
            foreach (var i in bb)
            {
                    var count = b.Where(qq => qq.Boxid == i.Boxid).Count();
                    if (count == 0)
                    {
                        _ctx.Boxs.Where(qq => qq.Boxid == i.Boxid).First().Boxstatus = 1;
                    }
                    else if (count >= 1)
                    {
                        _ctx.Boxs.Where(qq => qq.Boxid == i.Boxid).First().Boxstatus = 2;
                    }
                li.Add((int)i.Boxid);
            }
            foreach(var i in libox)
            {
                if (li.Contains(i))
                {
                }
                else
                {
                    _ctx.Boxs.Where(qq => qq.Boxid == i).First().Boxstatus = 0;
                }
            }
            _ctx.SaveChanges();
                return BOX;
        }
        public List<showclass> getMyLocker(int Userid)
        {
            var show = new List<showclass>();
            var has = _ctx.Users.Where(i => i.Userid == Userid).First().Has;
            if (has == 1)
            {
                var i = this.boxTime.First();
                //check today you can use open/close the box : 0 = have not, : 1 = have
                /*var use = _ctx.Boxtimes.Where(i => i.Userid == Userid).Where(u => u.Bookingstart.Value.Date == DateTime.Now.Date);
                string username = "true";
                if (use.Count() != 0)
                {
                    username = (!(use.First().Bookingstart.Value.TimeOfDay <= DateTime.Now.TimeOfDay)).ToString().ToLower();
                    _ctx.Boxs.Where(i => i.Boxid == use.First().Boxid).First().Userid = Userid;
                    _ctx.SaveChanges();
                }*/
                show.Add(new showclass {Bookingstart=(DateTime)i.Bookingstart,BookingEnd=(DateTime)i.BookingEnd,Boxid=(int)i.Boxid });
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



        [HttpGet]
        public string getauthPost()
        {
            var res = (string)TempData["getauthenPost"];
            return res;
        }
        [HttpPost]
        [Route("booking/authPost")]
        public IActionResult authPost(string start, string end, int locker)
        {
            var count = 0;
            var userid = HttpContext.Session.GetInt32("Userid");
            Console.WriteLine("auth post startTime: " + start);
            DateTime startTime = DateTime.ParseExact(start, "dd/MM/yyyy HH:mm", null);
            DateTime endTime = DateTime.ParseExact(end, "dd/MM/yyyy HH:mm", null);
            var timeList = _ctx.Boxtimes.Where(i => i.Boxid == locker).ToList();
            try
            {
                foreach (var i in timeList)
                {
                    if (startTime < i.BookingEnd && i.Bookingstart < endTime)
                    {
                        count += 1;
                    }
                }
                if (count == 0)
                {
                    TempData["getauthenPost"] = "0";
                    Boxtime boxtime = new Boxtime { Bookingstart = startTime, BookingEnd = endTime, Boxid = locker, Userid = userid };
                    _ctx.Boxtimes.Add(boxtime);
                    _ctx.Users.Where(t => t.Userid == userid).First().Has = 1;
                    _ctx.SaveChanges();
                }
                else
                {
                    TempData["getauthenPost"] = "1";

                }

            }
            catch
            {
            }
            return Redirect("/");

        }
        [HttpPut]
        [Route("home/authLocker/{id}")]
        public string Put(int id, int value)
        {
            try
            {
                var Userid = HttpContext.Session.GetInt32("Userid");
                var boxTime = _ctx.Boxtimes.Where(i => i.Bookingstart.Value.Date == DateTime.Now.Date).Where(i => i.Userid == Userid).First();
                var box = _ctx.Boxs.Where(i => i.Boxid == boxTime.Boxid).First().BoxCheck;
                Console.WriteLine("test1 "+box+"   "+id);
                _ctx.Boxs.Where(qq => qq.Boxid == id).First().BoxCheck = value;
                Console.WriteLine("change to:    "+value);
                _ctx.SaveChanges();
                return "Put Method is work ";
            }
            catch
            {
                return "error";
            }
        }
        [HttpGet]
        [Route("manage/get")]
        public IEnumerable<string> getValue()
        {
            try
            {
                var boxes = "";
                if ((int)HttpContext.Session.GetInt32("Userid") != null)
                {
                    var Userid = (int)HttpContext.Session.GetInt32("Userid");
                    //switch
                    var tt =_ctx.Boxtimes.Where(ii => ii.Userid == Userid).First().Boxid;
                    boxes = _ctx.Boxs.Where(i => i.Userid == Userid).Where(ii=>ii.Boxid==tt).First().BoxCheck == 0 ? "1" : "0";
                }
                var box = boxes == "0" ? "in" : "out";
                return new string[] { boxes, box };
            }
            catch
            {
                return new string[] { "", "" };
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
       
        [HttpGet]
        public bool getHas()
        {
            var Userid = (int)HttpContext.Session.GetInt32("Userid");
            var has = _ctx.Users.Where(i => i.Userid == Userid).First().Has;
            return has == 1 ? true : false;
        }
    }
}
