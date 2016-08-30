using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IDADataAccess.Parameters;
using IDA.Models.CustomAUM;
using IDADataAccess.Parameters.Entities;

namespace IDA.Models.RiskReportData
{
    public class CustomAUMService : IDisposable
    {

        private RiskContext parameters;

        public CustomAUMService(RiskContext parameters)
        {
            this.parameters = parameters;
        }

        #region Create
        /*  
               public void CustomAUMTableColumnDef_Create(CustomAUMTableColumnDefViewModel valTableColumnDef)
               {
                   var entity = new ValidationTableColumnDef();

                   entity.TableName = valTableColumnDef.TableName;
                   entity.ColumnName = valTableColumnDef.ColumnName;

                   parameters.ValidationTableColumnDef.Add(entity);
                   parameters.SaveChanges();

                   valTableColumnDef.Id = entity.Id;
               }

                   public void CustomAUMMapping_Create(CustomAUMMappingViewModel CustomAUMMapping)
                      {
                          var entity = new CustomAUMMapping();

                          entity.CustomAUMTableColumnDefId = CustomAUMMapping.ValidationTableColumnDefId;
                          entity.ValidationFormTypeId = CustomAUMMapping.ValidationFormTypeId;
                          entity.RelatedQuestions = CustomAUMMapping.RelatedQuestions;
                          entity.Mappings = CustomAUMMapping.Mappings;
                          entity.ViewOrder = CustomAUMMapping.ViewOrder;

                          parameters.CustomAUMMapping.Add(entity);
                          parameters.SaveChanges();

                          CustomAUMMapping.Id = entity.Id;
                      }

                      public void ValidationEnumeration_Create(ValidationEnumerationViewModel ValidationEnumeration)
                      {
                          var entity = new ValidationEnumeration();

                          entity.Id = ValidationEnumeration.Id;
                          entity.ValidationEnumerationId = ValidationEnumeration.ValidationEnumerationId;
                          entity.Value = ValidationEnumeration.Value;

                          parameters.ValidationEnumeration.Add(entity);
                          parameters.SaveChanges();
                      }

                      public void ValidationRulesErrorMessage_Create(ValidationRulesErrorMessageViewModel validationRulesErrorMessage)
                      {
                          var entity = new ValidationRulesErrorMessages();

                          entity.ColumnID = validationRulesErrorMessage.ColumnID;
                          entity.Message = validationRulesErrorMessage.Message;
                          entity.ValidationFormTypeIds = validationRulesErrorMessage.ValidationFormTypeIds;

                          parameters.ValidationRulesErrorMessages.Add(entity);
                          parameters.SaveChanges();

                          validationRulesErrorMessage.Id = entity.Id;
                      }

                      public void ValidationRule_Create(ValidationRuleViewModel validationRule)
                      {
                          var entity = new ValidationRules();

                          entity.CustomAUMTableColumnDefId = validationRule.CustomAUMTableColumnDefId;
                          entity.OperandColumn = validationRule.OperandColumn;
                          entity.ValidationOperator = validationRule.ValidationOperator;
                          entity.OperandValue = validationRule.OperandValue;
                          entity.ValidationStep = validationRule.ValidationStep;
                          entity.ValidationFormTypeIds = validationRule.ValidationFormTypeIds;

                          parameters.ValidationRules.Add(entity);
                          parameters.SaveChanges();

                          validationRule.Id = entity.Id;
                      }*/

        #endregion

        #region Read

        public IEnumerable<RiskReportDataTableColumnDefViewModel> ValidationTableColumnDef_Read()
        {
            var riskCtx = new RiskContext();
            var riskReportDataColumnDefs = riskCtx.CustomAUMMapping.Select(riskReportDataColumnDef => new RiskReportDataTableColumnDefViewModel
            {
                Id = riskReportDataColumnDef.Id,
                Client = riskReportDataColumnDef.Client,
                Portfolio = riskReportDataColumnDef.Portfolio,
                AUM = riskReportDataColumnDef.AUM,
                EffectiveDate = riskReportDataColumnDef.EffectiveDate,
                IsStatic = riskReportDataColumnDef.IsStatic,
                sysDate = riskReportDataColumnDef.sysDate,
                ModifiedBy = riskReportDataColumnDef.ModifiedBy,
            });
            return riskReportDataColumnDefs;
        }
        
        /*
                public IEnumerable<CustomAUMMappingViewModel> CustomAUMMapping_Read()
                {

                    var parameters = new ParametersContext();

                    var CustomAUMMappings = parameters.CustomAUMMapping.Select(CustomAUMMapping => new CustomAUMMappingViewModel
                    {
                        Id = CustomAUMMapping.Id,
                        CustomAUMTableColumnDefId = CustomAUMMapping.Id,
                        ValidationFormTypeId = CustomAUMMapping.ValidationFormTypeId,
                        RelatedQuestions = CustomAUMMapping.RelatedQuestions,
                        Mappings = CustomAUMMapping.Mappings,
                        ViewOrder = CustomAUMMapping.ViewOrder
                    });

                    return CustomAUMMappings;

                }

                public IEnumerable<ValidationEnumerationViewModel> ValidationEnumeration_Read()
                {
                    var parameters = new ParametersContext();

                    var ValidationEnumerations = parameters.ValidationEnumeration.Select(ValidationEnumeration => new ValidationEnumerationViewModel
                    {
                        Id = ValidationEnumeration.Id,
                        ValidationEnumerationId = ValidationEnumeration.ValidationEnumerationId,
                        Value = ValidationEnumeration.Value
                    });

                    return ValidationEnumerations;
                }

                public IEnumerable<ValidationRulesErrorMessageViewModel> ValidationRulesErrorMessage_Read()
                {

                    var parameters = new ParametersContext();

                    var validationRulesErrorMessages = parameters.ValidationRulesErrorMessages.Select(validationRulesErrorMessage => new ValidationRulesErrorMessageViewModel
                    {
                        Id = validationRulesErrorMessage.Id,
                        ColumnID = validationRulesErrorMessage.ColumnID,
                        Message = validationRulesErrorMessage.Message,
                        ValidationFormTypeIds = validationRulesErrorMessage.ValidationFormTypeIds

                    });

                    return validationRulesErrorMessages;

                }

                public IEnumerable<ValidationRuleViewModel> ValidationRule_Read()
                {
                    var parameters = new ParametersContext();

                    var validationRules = parameters.ValidationRules.Select(validationRule => new ValidationRuleViewModel
                    {
                        Id = validationRule.Id,
                        CustomAUMTableColumnDefId = validationRule.CustomAUMTableColumnDefId,
                        OperandColumn = validationRule.OperandColumn,
                        ValidationOperator = validationRule.ValidationOperator,
                        OperandValue = validationRule.OperandValue,
                        ValidationStep = validationRule.ValidationStep,
                        ValidationFormTypeIds = validationRule.ValidationFormTypeIds
                    });

                    return validationRules;
                }

                #endregion

                #region update

                public void CustomAUMTableColumnDef_Update(CustomAUMTableColumnDefViewModel valTableColumnDef)
                {
                    var entity = new CustomAUMTableColumnDef();

                    entity.Id = valTableColumnDef.Id;
                    entity.TableName = valTableColumnDef.TableName;
                    entity.ColumnName = valTableColumnDef.ColumnName;

                    parameters.CustomAUMTableColumnDef.Attach(entity);
                    parameters.Entry(entity).State = EntityState.Modified;
                    parameters.SaveChanges();
                }

                public void CustomAUMMapping_Update(CustomAUMMappingViewModel CustomAUMMapping)
                {
                    var entity = new CustomAUMMapping();

                    entity.Id = CustomAUMMapping.Id;
                    entity.CustomAUMTableColumnDefId = CustomAUMMapping.CustomAUMTableColumnDefId;
                    entity.ValidationFormTypeId = CustomAUMMapping.ValidationFormTypeId;
                    entity.RelatedQuestions = CustomAUMMapping.RelatedQuestions;
                    entity.Mappings = CustomAUMMapping.Mappings;
                    entity.ViewOrder = CustomAUMMapping.ViewOrder;

                    parameters.CustomAUMMapping.Attach(entity);
                    parameters.Entry(entity).State = EntityState.Modified;
                    parameters.SaveChanges();
                }

                public void ValidationEnumeration_Update(ValidationEnumerationViewModel validationEnumeration)
                {
                    var entity = new ValidationEnumeration();

                    entity.Id = validationEnumeration.Id;
                    entity.ValidationEnumerationId = validationEnumeration.ValidationEnumerationId;
                    entity.Value = validationEnumeration.Value;

                    parameters.ValidationEnumeration.Attach(entity);
                    parameters.Entry(entity).State = EntityState.Modified;
                    parameters.SaveChanges();
                }

                public void ValidationRulesErrorMessage_Update(ValidationRulesErrorMessageViewModel validationRulesErrorMessage)
                {
                    var entity = new ValidationRulesErrorMessages();

                    entity.Id = validationRulesErrorMessage.Id;
                    entity.ColumnID = validationRulesErrorMessage.ColumnID;
                    entity.Message = validationRulesErrorMessage.Message;
                    entity.ValidationFormTypeIds = validationRulesErrorMessage.ValidationFormTypeIds;

                    parameters.ValidationRulesErrorMessages.Attach(entity);
                    parameters.Entry(entity).State = EntityState.Modified;
                    parameters.SaveChanges();
                }

                public void ValidationRule_Update(ValidationRuleViewModel validationRule)
                {
                    var entity = new ValidationRules();

                    entity.Id = validationRule.Id;
                    entity.CustomAUMTableColumnDefId = validationRule.CustomAUMTableColumnDefId;
                    entity.OperandColumn = validationRule.OperandColumn;
                    entity.ValidationOperator = validationRule.ValidationOperator;
                    entity.OperandValue = validationRule.OperandValue;
                    entity.ValidationStep = validationRule.ValidationStep;
                    entity.ValidationFormTypeIds = validationRule.ValidationFormTypeIds;

                    parameters.ValidationRules.Attach(entity);
                    parameters.Entry(entity).State = EntityState.Modified;
                    parameters.SaveChanges();
                }

                #endregion

                #region Destroy

                public void CustomAUMTableColumnDef_Destroy(CustomAUMTableColumnDefViewModel valTableColumnDef)
                {
                    var entity = new CustomAUMTableColumnDef();

                    entity.Id = valTableColumnDef.Id;

                    parameters.CustomAUMTableColumnDef.Attach(entity);

                    parameters.CustomAUMTableColumnDef.Remove(entity);

                    parameters.SaveChanges();
                }

                public void CustomAUMMapping_Destroy(CustomAUMMappingViewModel CustomAUMMapping)
                {
                    var entity = new CustomAUMMapping();

                    entity.Id = CustomAUMMapping.Id;

                    parameters.CustomAUMMapping.Attach(entity);

                    parameters.CustomAUMMapping.Remove(entity);

                    parameters.SaveChanges();
                }

                public void ValidationEnumeration_Destroy(ValidationEnumerationViewModel validationEnumeration)
                {
                    var entity = new ValidationEnumeration();

                    entity.Id = validationEnumeration.Id;

                    parameters.ValidationEnumeration.Attach(entity);

                    parameters.ValidationEnumeration.Remove(entity);

                    parameters.SaveChanges();
                }

                public void ValidationRulesErrorMessage_Destroy(ValidationRulesErrorMessageViewModel validationRulesErrorMessage)
                {
                    var entity = new ValidationRulesErrorMessages();

                    entity.Id = validationRulesErrorMessage.Id;

                    parameters.ValidationRulesErrorMessages.Attach(entity);

                    parameters.ValidationRulesErrorMessages.Remove(entity);

                    parameters.SaveChanges();
                }

                public void ValidationRule_Destroy(ValidationRuleViewModel validationRule)
                {
                    var entity = new ValidationRules();

                    entity.Id = validationRule.Id;

                    parameters.ValidationRules.Attach(entity);

                    parameters.ValidationRules.Remove(entity);

                    parameters.SaveChanges();
                }
                */
        #endregion

        public void Dispose()
        {
            parameters.Dispose();
        }
    }
}