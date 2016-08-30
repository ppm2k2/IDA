using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ConceptONE.Infrastructure;
using ConceptONE.Infrastructure.Extensions;
using IDA.Models.DEMI;
using IDADataAccess.DEMI;
using IDADataAccess.DEMI.Entities;
using IDADataAccess.Parameters;
using IDALibrary;
using IDALibrary.Constants;
using IDALibrary.DataLoaders;
using IDALibrary.DataLoaders.Interfaces;
using IDALibrary.Enums;
using IDALibrary.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace IDA.Controllers
{
    public class DEMIController : BaseController
    {
        private List<SourceViewModel> _SourceData;
        private List<TransformationViewModel> _DestinationData;
        private string _InputFilesPath = Settings.InputFilesPath;

        private const string SUCCESS = "";

        public DEMIController()
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

        private string UserName
        {
            get
            {
                string result = User.Identity.Name.Substring(User.Identity.Name.LastIndexOf(@"\") + 1);
                return result;
            }
        }

        #region Step 1 Actions

        /// <summary>
        /// Step 1: load page
        /// </summary>
        public ActionResult Transformation()
        {
            ViewBag.Name = "Transformation";

            if (IsAuthorized(User.Identity.Name))
                return View();
            else
                return RedirectToAction("Unauthorized", "Home");
        }

        /// <summary>
        /// Step 1: load source File
        /// </summary>
        public ActionResult SaveSourceFile(IEnumerable<HttpPostedFileBase> sourceFiles)
        {
            string error = SaveFilesLocally(sourceFiles);
            ActionResult result = Content(error);

            return result;
        }

        /// <summary>
        /// Step 1: load destination file
        /// </summary>
        public ActionResult SaveDestinationFile(IEnumerable<HttpPostedFileBase> destinationFiles)
        {
            string error = SaveFilesLocally(destinationFiles);
            ActionResult result = Content(error);

            return result;
        }

        /// <summary>
        /// Step 1: auto-complete for drop down box
        /// </summary>
        public ActionResult GetTransformationSets(string text)
        {
            TransformationService demiService = new TransformationService(new DEMIContext());

            IEnumerable<TransformationSetViewModel> result = demiService.
                TransformationSets_Read(UserName).Where(t => t.Name.Contains(text)).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Step 1: undo file load
        /// </summary>
        public ActionResult RemoveFile(string[] fileNames)
        {
            if (fileNames != null)
            {
                foreach (string fullName in fileNames)
                {
                    string fileName = String.Format("{0}_{1}", UserName, Path.GetFileName(fullName));
                    string physicalPath = Path.Combine(Settings.InputFilesPath, fileName);

                    try
                    {
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        return Content(ex.Message);
                    }
                }
            }

            return Content(SUCCESS);
        }

        #endregion

        #region Step 2 Actions

        /// <summary>
        /// Step 2: load ("Next Step" in Step 1)
        /// </summary>
        public ActionResult CreateTransformationSet(string setName, string fileName)
        {
            fileName = String.Format("{0}_{1}", UserName, Path.GetFileName(fileName));
            string fullPath = Path.Combine(Settings.InputFilesPath, fileName);

            TransformationService demiService = new TransformationService(new DEMIContext());
            int result = demiService.CreateTransformationSet(UserName, setName, fullPath);

            //TODO: return as int
            return Content(result.ToString());
        }

        /// <summary>
        /// Step 2: load grid data
        /// </summary>
        public ActionResult SourceGrid_Read([DataSourceRequest]DataSourceRequest request, string fileName)
        {
            JsonResult result = null;
            object jsonData;

            fileName = String.Format("{0}_{1}", UserName, Path.GetFileName(fileName));

            string physicalPath = Path.Combine(Settings.InputFilesPath, fileName);

            try
            {
                LoadColumnNames(physicalPath, FileType.Source);
                jsonData = new { Data = _SourceData };
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                jsonData = new { status = "Failure", error = ex.ToString() };
            }

            result = Json(jsonData, JsonRequestBehavior.AllowGet);

            return result;
        }

        //TODO: string filename param not used
        public ActionResult TransformationGrid_Read([DataSourceRequest] DataSourceRequest request, string filename, int setId)
        {
            TransformationService demiService = new TransformationService(new DEMIContext());
            IEnumerable<TransformationViewModel> transformations = demiService.Transformations_Read(setId);
            DataSourceResult result = transformations.ToDataSourceResult(request);

            return Json(result);
        }

        /// <summary>
        /// Step 2: "Save changes" button
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TransformationGrid_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<TransformationViewModel> transformations, int setId)
        {
            if (transformations != null && ModelState.IsValid)
            {
                try
                {
                    TransformationService demiService = new TransformationService(new DEMIContext());
                    demiService.Transformations_Update(transformations);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return Json(transformations.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Step 3 Actions

        /// <summary>
        /// Step 3: file download
        /// </summary>
        public void DownloadTransformedData(int setId)
        {
            TransformationService service = new TransformationService(new DEMIContext());
            DataTable transformedData = service.RunTransformation(UserName, setId);

            string fileName = transformedData.TableName;
            string fileContents = transformedData.GetTableContent();

            DownloadTextFile(fileContents, fileName);
        }

        #endregion

        #region Private

        private void LoadColumnNames(string filename, FileType fileType)
        {
            _SourceData = new List<SourceViewModel>();
            _DestinationData = new List<TransformationViewModel>();

            IExcelService service = new ExcelService();
            service.InputFileName = filename;
            service.LoadData(UserName, fileType);

            if (fileType == FileType.Source)
            {
                foreach (string columnName in service.ColumnNames.Values)
                    _SourceData.Add(new SourceViewModel { ColumnName = columnName });
            }
            else
            {
                foreach (string columnName in service.ColumnNames.Values)
                    _DestinationData.Add(new TransformationViewModel { TransformationRule = "", TargetColumn = columnName });
            }
        }

        //TODO: change from collection to one file only
        private string SaveFilesLocally(IEnumerable<HttpPostedFileBase> files)
        {
            string result = SUCCESS;

            if (files != null && files.Count() > 0)
            {
                try
                {
                    HttpPostedFileBase file = files.ToList()[0];
                    string fileName = String.Format("{0}_{1}", UserName, Path.GetFileName(file.FileName));
                    string physicalPath = Path.Combine(_InputFilesPath, fileName);

                    if (!Directory.Exists(_InputFilesPath))
                        Directory.CreateDirectory(_InputFilesPath);

                    file.SaveAs(physicalPath);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                    result = ex.Message;
                }
            }

            return result;
        }

        #endregion

    }
}