using System.Web.Mvc;
using RaceAlly.Web.Filters;

namespace RaceAlly.Web.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/
         [RaceAllyAuthorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }

    }
}
