using System.Web.Mvc;
using IDALibrary;
using IDALibrary.Constants;

namespace IDA.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Name = "Home";

            // string appType = ConfigurationManager.AppSettings["AppType"];

            if (Settings.AppType == AppTypes.DEMI)
                return RedirectToAction("Transformation", AppTypes.DEMI);
            
            return View();
        }
      
        public ActionResult Unauthorized()
        {
            ViewBag.Name = "Unauthorized";
                  
            return View();
        }
    }
}