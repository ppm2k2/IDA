using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
using ConceptONE.Infrastructure;
using ConceptONE.Infrastructure.Extensions;
using IDADataAccess.Parameters;
using IDALibrary;
using IDALibrary.Constants;
using IDALibrary.Services;

namespace IDA.Controllers
{
    public abstract class BaseController : Controller
    {

        protected List<string> _RolesAllowed;

        #region Public Methods

        public virtual bool IsAuthorized(string userName, List<string> rolesAllowed = null)
        {
            bool result = false;

            if (rolesAllowed == null)
                rolesAllowed = _RolesAllowed;

            if (Settings.AppType == AppTypes.IDA)
            {
                AccountService accountService = new AccountService(new ParametersContext());
                result = accountService.IsAuthorized(userName, rolesAllowed);
            }

            return result;
        }

        /// <summary>
        /// Every controller inherits functionality to download the current OutputTable
        /// </summary>
        public void Download()
        {
            try
            {
                DataTable outputTable = (DataTable)Session["OutputTable"];
                Download(outputTable);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        #endregion

        #region Private/Protected Methods

        protected void Download(DataTable dataTable)
        {
            string fileName = dataTable.TableName.RemoveBrackets() + ".csv";
            string fileContents = dataTable.GetTableContent();

            DownloadTextFile(fileContents, fileName);
        }

        protected FileResult DownloadFile(string filePath)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = Path.GetFileName(filePath);
            FileResult result = File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            return result;
        }

        protected void DownloadTextFile(string fileContents, string fileName)
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            Response.AppendHeader("Content-Disposition", string.Format("attachment;filename={0}", fileName));
            Response.Write(fileContents);
            Response.End();
        }

        protected FileResult DownloadExcelFile(byte[] fileBytes, string fileName)
        {
            const string HTTP_APPTYPE_EXCEL = "application/vnd.ms-excel";
            FileResult result = File(fileBytes, HTTP_APPTYPE_EXCEL, fileName);

            return result;
        }

        #endregion

    }
}