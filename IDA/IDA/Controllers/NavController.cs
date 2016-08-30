using System.Web.Mvc;
using IDA.Models;
using System.Configuration;
using IDALibrary.Constants;

namespace IDA.Controllers
{
    public class NavController : Controller
    {       
        public ActionResult Index()
        {
            var viewModel = new Navigation();

            string panelBarType = GetPanelBarType(ConfigurationManager.AppSettings["AppType"]);
            
            return View(panelBarType, viewModel);
        }

        public ActionResult Home()
        {
            var viewModel = new Navigation();

            string panelBarType = GetPanelBarType(ConfigurationManager.AppSettings["AppType"]);

            return View(panelBarType, viewModel);
        }

        public ActionResult DEMI()
        {
            var viewModel = new Navigation();

            string panelBarType = GetPanelBarType(ConfigurationManager.AppSettings["AppType"]);

            return View(panelBarType, viewModel);
        }

        public ActionResult ValidationMetaData()
        {
            var viewModel = new Navigation();

            string panelBarType = GetPanelBarType(ConfigurationManager.AppSettings["AppType"]);

            return View("PanelBar", viewModel);
        }

        public ActionResult Applications()
        {
            var viewModel = new Navigation();

            string panelBarType = GetPanelBarType(ConfigurationManager.AppSettings["AppType"]);

            return View("PanelBar", viewModel);
        }

        private string GetPanelBarType(string appType)
        {
            string result = "PanelBar";
            
            if (appType == AppTypes.DEMI)
                result = "PanelBar_DEMI";

            return result;
        }

    }
}