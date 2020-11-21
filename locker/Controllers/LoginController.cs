using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using locker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace locker.Controllers
{

    public class LoginController : Controller
    {
        private readonly Contxtdb ctx = new Contxtdb();
        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewBag.isLogin = TempData["login"];
            ViewBag.msg = TempData["Message"];
            ViewBag.msgg = TempData["Messagee"];
            return View();
        }

        public IActionResult regis()
        {
            ViewBag.isLogin = TempData["login"];
            ViewBag.msg = TempData["Message"];
            return View();
        }


        [HttpPost]
        public IActionResult authRegis(User user)
        {
            var u = ctx.Users.Where(z => z.Username == user.Username);
            if (u.Count() == 0)
            {
               
                User newUser = new User { Username = user.Username, Password = user.Password, Status = 0, Has = 0 };
                Console.WriteLine(newUser.Username + "  " + newUser.Password);
                ctx.Users.Add(newUser);
                ctx.SaveChanges();
                TempData["Messagee"] = "Register success";
                TempData["login"] = "true";

                return RedirectToAction("index");


            }
            else
            {
                TempData["Message"] = "username is existed";
                TempData["login"] = "true";
                return RedirectToAction("regis");

            }
        }

        [HttpPost]
        public IActionResult authLogin(User u)
        {
            var login = ctx.Users.Where(i => i.Username == u.Username).Where(i => i.Password == u.Password);
            var logon = login.Count() == 1 ? true : false;
            if (logon)
            {
                HttpContext.Session.SetInt32("login", 1);
                HttpContext.Session.SetString("username", u.Username);
                HttpContext.Session.SetInt32("Userid", login.First().Userid);
                
                return Redirect("/");

            }
            else
            {
                TempData["Message"] = "username or password wrong!!";
               
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public IActionResult authlogout()
        {

            HttpContext.Session.SetInt32("login", 0);
            return Redirect("/");
        }

    }
}
