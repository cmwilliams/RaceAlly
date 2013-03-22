using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaceAlly.Contracts.DataModel;
using RaceAlly.Models;

namespace RaceAlly.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDataContext _context;

        public HomeController(IDataContext context)
        {
            _context = context;
        }


        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
