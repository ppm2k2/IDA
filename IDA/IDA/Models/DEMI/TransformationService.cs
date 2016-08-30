using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using IDADataAccess.DEMI;
using IDADataAccess.DEMI.Entities;
using IDALibrary.DataLoaders;
using IDALibrary.DataLoaders.Interfaces;
using IDALibrary.Enums;

namespace IDA.Models.DEMI
{

    public class TransformationService : IDisposable
    {
        private DEMIContext _DemiContext;

        public TransformationService(DEMIContext context)
        {
            _DemiContext = context;
        }

        #region Create

        public void TransformationSet_Create(TransformationSet transformationSet)
        {
            transformationSet.CreateDateTime = DateTime.Now;
            _DemiContext.TransformationSets.Add(transformationSet);
            _DemiContext.SaveChanges();
        }

        public void Transformations_Create(IEnumerable<TransformationViewModel> transformations)
        {
            DateTime curDateTime = DateTime.Now;

            foreach (TransformationViewModel transformation in transformations)
            {
                Transformation entity = new Transformation();

                entity.TransformationSetId = transformation.TransformationSetId;
                entity.TargetColumn = transformation.TargetColumn;
                entity.TransformationRule = transformation.TransformationRule;
                entity.CreateDateTime = curDateTime;
                entity.UpdateDateTime = curDateTime;
                _DemiContext.Transformations.Add(entity);
            }

            _DemiContext.SaveChanges();
        }

        #endregion

        #region Read

        public IEnumerable<TransformationSetViewModel> TransformationSets_Read(string userName)
        {
            IEnumerable<TransformationSetViewModel> result = _DemiContext.TransformationSets.
                Where(t => t.UserName == userName).
                Select(transformationSet => new TransformationSetViewModel
                {
                    Id = transformationSet.Id,
                    Name = transformationSet.Name,
                    Create_Date_Time = transformationSet.CreateDateTime,
                    UserName = transformationSet.UserName
                });

            return result;
        }

        public TransformationSet TransformationSet_Read(string name, string userName)
        {
            TransformationSet result = _DemiContext.TransformationSets.
                Where(t => t.UserName == userName && t.Name == name).FirstOrDefault();

            return result;
        }

        public IEnumerable<TransformationViewModel> Transformations_Read(int setId)
        {
            IEnumerable<TransformationViewModel> result = _DemiContext.Transformations.
                Where(t => t.TransformationSetId == setId).
                Select(transformation => new TransformationViewModel
                {
                    Id = transformation.Id,
                    TransformationSetId = transformation.TransformationSetId,
                    TargetColumn = transformation.TargetColumn,
                    TransformationRule = transformation.TransformationRule,
                    Create_Date_Time = transformation.CreateDateTime,
                    Update_Date_Time = transformation.UpdateDateTime
                });

            return result;
        }

        #endregion

        #region Update

        public void Transformations_Update(IEnumerable<TransformationViewModel> transformations) //TransformationViewModel transformationViewModel)
        {
            foreach (TransformationViewModel transformationViewModel in transformations)
            {
                Transformation transformation = new Transformation();

                transformation.Id = transformationViewModel.Id;
                transformation.TransformationSetId = transformationViewModel.TransformationSetId;
                transformation.TargetColumn = transformationViewModel.TargetColumn;
                transformation.TransformationRule = transformationViewModel.TransformationRule;
                transformation.CreateDateTime = transformationViewModel.Create_Date_Time;
                transformation.UpdateDateTime = DateTime.Now;

                _DemiContext.Transformations.Attach(transformation);
                _DemiContext.Entry(transformation).State = EntityState.Modified;
                _DemiContext.SaveChanges();
            }
        }

        #endregion

        #region Destroy

        public void Transformations_Destroy(TransformationViewModel trans)
        {
            Transformation entity = new Transformation();

            entity.Id = trans.Id;

            _DemiContext.Transformations.Attach(entity);
            _DemiContext.Transformations.Remove(entity);
            _DemiContext.SaveChanges();
        }

        #endregion

        public int CreateTransformationSet(string userName, string setName, string fullPath)
        {
            TransformationSet set;
            List<TransformationViewModel> transformations = new List<TransformationViewModel>();

            IExcelService destFile = new ExcelService();
            destFile.InputFileName = fullPath;
            destFile.LoadData(userName, FileType.Destination);

            set = TransformationSet_Read(setName, userName);

            if (set == null)
                set = CreateNewSet(userName, setName, transformations, destFile);
            else
                UpdateExistingSet(set, transformations, destFile);

            int result = set.Id;

            return result;
        }

        public DataTable RunTransformation(string userName, int setId)
        {
            const string TRANSFORMED_DATA_FILE = "{0:yyyyMMdd_HHmm}_Transformed_Data_Set_{1:0000}.csv";

            DataTable result = _DemiContext.LoadTransformedDataTable(setId, userName);
            result.TableName = String.Format(TRANSFORMED_DATA_FILE, DateTime.Now, setId);

            return result;
        }

        public void Dispose()
        {
            _DemiContext.Dispose();
        }

        private TransformationSet CreateNewSet(string userName, string setName, List<TransformationViewModel> transformations, IExcelService destFile)
        {
            TransformationSet result = new TransformationSet();
            {
                result.Name = setName;
                result.UserName = userName;
            }

            TransformationSet_Create(result);

            foreach (string columnName in destFile.ColumnNames.Values)
            {
                TransformationViewModel model = new TransformationViewModel
                {
                    TransformationSetId = result.Id,
                    TransformationRule = "",
                    TargetColumn = columnName
                };
                transformations.Add(model);
            }

            Transformations_Create(transformations);

            return result;
        }

        private void UpdateExistingSet(TransformationSet set, List<TransformationViewModel> transformations, IExcelService destFile)
        {
            IEnumerable<TransformationViewModel> existingTranformations = Transformations_Read(set.Id).ToList();

            foreach (string columnName in destFile.ColumnNames.Values)
            {
                if (!existingTranformations.Where(t => t.TargetColumn.Contains(columnName)).Any())
                    transformations.Add(new TransformationViewModel { TransformationSetId = set.Id, TransformationRule = "", TargetColumn = columnName });
            }

            if (transformations.Count > 0)
                Transformations_Create(transformations);

            //TODO: I think this is wrong. We should not remove transformations, just 
            //because a column is not found in this table
            foreach (TransformationViewModel trans in existingTranformations)
            {
                if (!destFile.ColumnNames.Where(t => t.Value.Contains(trans.TargetColumn)).Any())
                    Transformations_Destroy(trans);
            }
        }

    }

}