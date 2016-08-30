using System.Collections.Generic;
using System.Web.Mvc;
using IDADataAccess.Parameters;
using IDALibrary;
using IDALibrary.Constants;
using IDALibrary.Enums;
using IDALibrary.Services;

namespace IDA.Controllers
{
    public class ApplicationsController : BaseController
    {
        Dictionary<int, string> applications = new Dictionary<int, string>() {
            { (int)App.IDC, "IDC" },
            { (int)App.Workbench, "Workbench" },
            { (int)App.COSEngine, "COS Engine" },
            { (int)App.PAMEngine, "PAM Engine" },
            { (int)App.PAMAnalyzers, "PAM Analyzers" },
            { (int)App.IDQWinService, "IDQ WinService" },
            { (int)App.IDQWebService, "IDQ WebService" },
            { (int)App.PADIConsole, "PADI Console" },
            { (int)App.PADIWeb, "PADI Web" },
        };

        public ApplicationsController()
        {
            _RolesAllowed = new List<string> { Roles.SUPER_USER };
        }

        public override bool IsAuthorized(string userName, List<string> rolesAllowed = null)
        {
            bool result = false;

            if (rolesAllowed == null)
                rolesAllowed = _RolesAllowed;

            if (Settings.AppType == AppTypes.DEMI || Settings.AppType == AppTypes.IDA)
            {
                AccountService accountService = new AccountService(new ParametersContext());
                result = accountService.IsAuthorized(userName, rolesAllowed);
            }

            return result;
        }

        public ActionResult Index(int appId)
        {
            ViewBag.Name = applications[appId];

            return View();
        }

        public ActionResult InstalledVersions(int appId)
        {
            //TODO: initialize / load AppVersinsViewModel and bind to View.
                        
            return View();
        }
    }
}