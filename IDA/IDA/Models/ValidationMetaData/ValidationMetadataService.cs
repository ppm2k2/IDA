using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using IDADataAccess.Parameters;
using IDADataAccess.Parameters.Entities;

namespace IDA.Models
{
    public class ValidationMetadataService : IDisposable
    {
        private ParametersContext parameters;

        public ValidationMetadataService(ParametersContext parameters)
        {
            this.parameters = parameters;
        }

        #region Create

        public void ValidationTableColumnDef_Create(ValidationTableColumnDefViewModel valTableColumnDef)
        {
            var entity = new ValidationTableColumnDef();

            entity.TableName = valTableColumnDef.TableName;
            entity.ColumnName = valTableColumnDef.ColumnName;

            parameters.ValidationTableColumnDef.Add(entity);
            parameters.SaveChanges();

            valTableColumnDef.Id = entity.Id;
        }

        public void ValidationFormTypeMapping_Create(ValidationFormTypeMappingViewModel validationFormTypeMapping)
        {
            var entity = new ValidationFormTypeMapping();

            entity.ValidationTableColumnDefId = validationFormTypeMapping.ValidationTableColumnDefId;
            entity.ValidationFormTypeId = validationFormTypeMapping.ValidationFormTypeId;
            entity.RelatedQuestions = validationFormTypeMapping.RelatedQuestions;
            entity.Mappings = validationFormTypeMapping.Mappings;
            entity.ViewOrder = validationFormTypeMapping.ViewOrder;

            parameters.ValidationFormTypeMapping.Add(entity);
            parameters.SaveChanges();

            validationFormTypeMapping.Id = entity.Id;
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

            entity.ValidationTableColumnDefId = validationRule.ValidationTableColumnDefId;
            entity.OperandColumn = validationRule.OperandColumn;
            entity.ValidationOperator = validationRule.ValidationOperator;
            entity.OperandValue = validationRule.OperandValue;
            entity.ValidationStep = validationRule.ValidationStep;
            entity.ValidationFormTypeIds = validationRule.ValidationFormTypeIds;
            
            parameters.ValidationRules.Add(entity);
            parameters.SaveChanges();

            validationRule.Id = entity.Id;
        }

        #endregion

        #region Read

        public IEnumerable<ValidationTableColumnDefViewModel> ValidationTableColumnDef_Read()
        {

            var parameters = new ParametersContext();

            var validationTableColumnDefs = parameters.ValidationTableColumnDef.Select(validationTableColumnDef => new ValidationTableColumnDefViewModel
            {
                Id = validationTableColumnDef.Id,
                TableName = validationTableColumnDef.TableName,
                ColumnName = validationTableColumnDef.ColumnName
            });

            return validationTableColumnDefs;

        }

        public IEnumerable<ValidationFormTypeMappingViewModel> ValidationFormTypeMapping_Read()
        {

            var parameters = new ParametersContext();

            var validationFormTypeMappings = parameters.ValidationFormTypeMapping.Select(validationFormTypeMapping => new ValidationFormTypeMappingViewModel
            {
                Id = validationFormTypeMapping.Id,
                ValidationTableColumnDefId = validationFormTypeMapping.Id,
                ValidationFormTypeId = validationFormTypeMapping.ValidationFormTypeId,
                RelatedQuestions = validationFormTypeMapping.RelatedQuestions,
                Mappings = validationFormTypeMapping.Mappings,
                ViewOrder = validationFormTypeMapping.ViewOrder
            });

            return validationFormTypeMappings;

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
                ValidationTableColumnDefId = validationRule.ValidationTableColumnDefId,
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

        public void ValidationTableColumnDef_Update(ValidationTableColumnDefViewModel valTableColumnDef)
        {
            var entity = new ValidationTableColumnDef();

            entity.Id = valTableColumnDef.Id;
            entity.TableName = valTableColumnDef.TableName;
            entity.ColumnName = valTableColumnDef.ColumnName;

            parameters.ValidationTableColumnDef.Attach(entity);
            parameters.Entry(entity).State = EntityState.Modified;
            parameters.SaveChanges();
        }

        public void ValidationFormTypeMapping_Update(ValidationFormTypeMappingViewModel validationFormTypeMapping)
        {
            var entity = new ValidationFormTypeMapping();

            entity.Id = validationFormTypeMapping.Id;
            entity.ValidationTableColumnDefId = validationFormTypeMapping.ValidationTableColumnDefId;
            entity.ValidationFormTypeId = validationFormTypeMapping.ValidationFormTypeId;
            entity.RelatedQuestions = validationFormTypeMapping.RelatedQuestions;
            entity.Mappings = validationFormTypeMapping.Mappings;
            entity.ViewOrder = validationFormTypeMapping.ViewOrder; 

            parameters.ValidationFormTypeMapping.Attach(entity);
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
            entity.ValidationTableColumnDefId = validationRule.ValidationTableColumnDefId;
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

        public void ValidationTableColumnDef_Destroy(ValidationTableColumnDefViewModel valTableColumnDef)
        {
            var entity = new ValidationTableColumnDef();

            entity.Id = valTableColumnDef.Id;

            parameters.ValidationTableColumnDef.Attach(entity);

            parameters.ValidationTableColumnDef.Remove(entity);

            parameters.SaveChanges();
        }

        public void ValidationFormTypeMapping_Destroy(ValidationFormTypeMappingViewModel validationFormTypeMapping)
        {
            var entity = new ValidationFormTypeMapping();

            entity.Id = validationFormTypeMapping.Id;

            parameters.ValidationFormTypeMapping.Attach(entity);

            parameters.ValidationFormTypeMapping.Remove(entity);

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

        #endregion

        public void Dispose()
        {
            parameters.Dispose();
        }
    }
}