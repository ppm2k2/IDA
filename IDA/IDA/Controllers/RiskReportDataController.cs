using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using IDA.Models;
using IDADataAccess.Parameters;
using IDALibrary.Constants;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace IDA.Controllers
{
    [Authorize]
    public class RiskReportDataController : BaseController
    {
        public RiskReportDataController()
        {
            _RolesAllowed = new List<string> { Roles.SUPER_USER, Roles.RISK_USER };
        }

        #region Views
        public ActionResult Index()
        {
            ViewBag.Name = "Validation Metadata";

            if (!IsAuthorized(User.Identity.Name))
                return RedirectToAction("Unauthorized", "Home");

            return View();
        }
        public ActionResult CustomAUM()
        {
            ViewBag.Name = "Custom AUM";

            if (!IsAuthorized(User.Identity.Name))
                return RedirectToAction("Unauthorized", "Home");

            return View();
        }


        #endregion

        #region Create

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationTableColumnDef_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationTableColumnDefViewModel> valTableColumnDefs)
        {
            var results = new List<ValidationTableColumnDefViewModel>();

            if (valTableColumnDefs != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var valTableColumnDef in valTableColumnDefs)
                    {
                        validationMetadataService.ValidationTableColumnDef_Create(valTableColumnDef);
                        results.Add(valTableColumnDef);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }

            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationFormTypeMapping_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationFormTypeMappingViewModel> validationFormTypeMappings)
        {
            var results = new List<ValidationFormTypeMappingViewModel>();

            if (validationFormTypeMappings != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationFormTypeMapping in validationFormTypeMappings)
                    {
                        validationMetadataService.ValidationFormTypeMapping_Create(validationFormTypeMapping);
                        results.Add(validationFormTypeMapping);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationEnumeration_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationEnumerationViewModel> validationEnumerations)
        {
            var results = new List<ValidationEnumerationViewModel>();

            if (validationEnumerations != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationEnumeration in validationEnumerations)
                    {
                        validationMetadataService.ValidationEnumeration_Create(validationEnumeration);
                        results.Add(validationEnumeration);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }

            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationRulesErrorMessage_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationRulesErrorMessageViewModel> validationRulesErrorMessages)
        {
            var results = new List<ValidationRulesErrorMessageViewModel>();

            if (validationRulesErrorMessages != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationRulesErrorMessage in validationRulesErrorMessages)
                    {
                        validationMetadataService.ValidationRulesErrorMessage_Create(validationRulesErrorMessage);
                        results.Add(validationRulesErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationRule_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationRuleViewModel> validationRules)
        {
            var results = new List<ValidationRuleViewModel>();

            if (validationRules != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationRule in validationRules)
                    {
                        validationMetadataService.ValidationRule_Create(validationRule);
                        results.Add(validationRule);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Read

        public ActionResult ValidationTableColumnDef_Read([DataSourceRequest] DataSourceRequest request)
        {
            ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());
            return Json(validationMetadataService.ValidationTableColumnDef_Read().ToDataSourceResult(request));
        }

        public ActionResult ValidationFormTypeMapping_Read([DataSourceRequest] DataSourceRequest request)
        {
            ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());
            return Json(validationMetadataService.ValidationFormTypeMapping_Read().ToDataSourceResult(request));
        }

        public ActionResult ValidationEnumeration_Read([DataSourceRequest] DataSourceRequest request)
        {
            ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());
            return Json(validationMetadataService.ValidationEnumeration_Read().ToDataSourceResult(request));
        }

        public ActionResult ValidationRulesErrorMessage_Read([DataSourceRequest] DataSourceRequest request)
        {
            ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());
            return Json(validationMetadataService.ValidationRulesErrorMessage_Read().ToDataSourceResult(request));
        }

        public ActionResult ValidationRule_Read([DataSourceRequest] DataSourceRequest request)
        {
            ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());
            return Json(validationMetadataService.ValidationRule_Read().ToDataSourceResult(request));
        }

        #endregion

        #region Update

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationTableColumnDef_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationTableColumnDefViewModel> valTableColumnDefs)
        {
            if (valTableColumnDefs != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var valTableColumnDef in valTableColumnDefs)
                    {
                        validationMetadataService.ValidationTableColumnDef_Update(valTableColumnDef);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(valTableColumnDefs.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationFormTypeMapping_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationFormTypeMappingViewModel> validationFormTypeMappings)
        {
            if (validationFormTypeMappings != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationFormTypeMapping in validationFormTypeMappings)
                    {
                        validationMetadataService.ValidationFormTypeMapping_Update(validationFormTypeMapping);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationFormTypeMappings.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationEnumeration_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationEnumerationViewModel> validationEnumerations)
        {
            if (validationEnumerations != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationEnumeration in validationEnumerations)
                    {
                        validationMetadataService.ValidationEnumeration_Update(validationEnumeration);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationEnumerations.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationRulesErrorMessage_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationRulesErrorMessageViewModel> validationRulesErrorMessages)
        {
            if (validationRulesErrorMessages != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationRulesErrorMessage in validationRulesErrorMessages)
                    {
                        validationMetadataService.ValidationRulesErrorMessage_Update(validationRulesErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationRulesErrorMessages.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationRule_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationRuleViewModel> validationRules)
        {
            if (validationRules != null && ModelState.IsValid)
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationRule in validationRules)
                    {
                        validationMetadataService.ValidationRule_Update(validationRule);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationRules.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Destroy

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationTableColumnDef_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationTableColumnDefViewModel> valTableColumnDefs)
        {
            if (valTableColumnDefs.Any())
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var valTableColumnDef in valTableColumnDefs)
                    {
                        validationMetadataService.ValidationTableColumnDef_Destroy(valTableColumnDef);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(valTableColumnDefs.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationFormTypeMapping_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationFormTypeMappingViewModel> validationFormTypeMappings)
        {
            if (validationFormTypeMappings.Any())
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationFormTypeMapping in validationFormTypeMappings)
                    {
                        validationMetadataService.ValidationFormTypeMapping_Destroy(validationFormTypeMapping);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationFormTypeMappings.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationEnumeration_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationEnumerationViewModel> validationEnumerations)
        {
            if (validationEnumerations.Any())
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationEnumeration in validationEnumerations)
                    {
                        validationMetadataService.ValidationEnumeration_Destroy(validationEnumeration);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationEnumerations.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationRulesErrorMessage_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationRulesErrorMessageViewModel> validationRulesErrorMessages)
        {
            if (validationRulesErrorMessages.Any())
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationRulesErrorMessage in validationRulesErrorMessages)
                    {
                        validationMetadataService.ValidationRulesErrorMessage_Destroy(validationRulesErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationRulesErrorMessages.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ValidationRule_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ValidationRuleViewModel> validationRules)
        {
            if (validationRules.Any())
            {
                try
                {
                    ValidationMetadataService validationMetadataService = new ValidationMetadataService(new ParametersContext());

                    foreach (var validationRule in validationRules)
                    {
                        validationMetadataService.ValidationRule_Destroy(validationRule);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.InnerException.ToString());
                }
            }

            return Json(validationRules.ToDataSourceResult(request, ModelState));
        }

        #endregion

        #region Private


        #endregion

    }
}