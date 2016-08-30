using System.Collections.Generic;
using System.Data;
using System.Linq;
using ConceptONE.Infrastructure;
using IDA.Models.DEMI;
using IDADataAccess.DEMI;
using IDADataAccess.DEMI.Entities;
using IDALibrary.DataLoaders;
using IDALibrary.DataLoaders.Interfaces;
using IDALibrary.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDATests
{
    [TestClass]
    public class TransformationServiceTests
    {

        //TODO: create "unittest" account
        private const string USER_NAME = "smazzucca";
        private const string TEST_FILE_FOLDER = @"..\..\TestFiles\";

        private DEMIContext _DemiContext;
        private TransformationService _TransformationService;
        private IExcelService _ExcelService;

        [TestInitialize]
        public void Initialize()
        {
            Logger.LogActivity("IDATests - TransformationServiceTests - Start");

            _DemiContext = new DEMIContext();
            _TransformationService = new TransformationService(_DemiContext);
            _ExcelService = new ExcelService();

            DeleteTransformationSet("UnitTest_CreateTransformationSet");
            DeleteTransformation("UnitTest_UpdateTransformationSetAddTwoFields", "Field5");
            DeleteTransformation("UnitTest_UpdateTransformationSetAddTwoFields", "Field6");
            DropTable(_DemiContext.GetTransformedDataTableName(USER_NAME));
        }

        private void DeleteTransformationSet(string setName)
        {
            TransformationSet set = _DemiContext.TransformationSets.
                Where(s => s.Name == setName).
                FirstOrDefault();

            if (set != null)
            {
                _DemiContext.TransformationSets.Remove(set);
                _DemiContext.SaveChanges();
            }
        }

        private void DropTable(string table)
        {
            _DemiContext.DropTable(table);
        }

        private void DeleteTransformation(string setName, string targetColumn)
        {
            TransformationSet set = _DemiContext.TransformationSets.
                Where(s => s.Name == setName).
                FirstOrDefault();

            if (set != null)
            {
                Transformation transformation = _DemiContext.Transformations.
                    Where(t => t.TargetColumn == targetColumn).
                    FirstOrDefault();

                if (transformation != null)
                {
                    _DemiContext.Transformations.Remove(transformation);
                    _DemiContext.SaveChanges();
                }
            }
        }

        [TestMethod]
        public void UpdateTransformationSetNoChanges()
        {
            const string DEST_FILE = TEST_FILE_FOLDER + @"01_UpdateTransformationSetNoChanges_Destination.xlsx";
            const string SET_NAME = "UnitTest_UpdateTransformationSetNoChanges";
            const int SET_ID = 26;

            int setId = _TransformationService.CreateTransformationSet(USER_NAME, SET_NAME, DEST_FILE);

            Assert.IsTrue(setId == SET_ID);
        }

        /// <summary>
        /// Starting TransformationSet:
        /// - Field1
        /// - Field2
        /// - Field3
        /// - Field4
        /// 
        /// Ending TransformationSet:
        /// - Field1
        /// - Field2
        /// - Field3
        /// - Field4
        /// - Field5
        /// - Field6
        /// </summary>
        [TestMethod]
        public void UpdateTransformationSetAddTwoFields()
        {
            const string DEST_FILE = TEST_FILE_FOLDER + @"02_UpdateTransformationSetAddTwoFields_Destination.xlsx";
            const string SET_NAME = "UnitTest_UpdateTransformationSetAddTwoFields";
            const int SET_ID = 29;
            const int EXPECTED_START_COUNT = 4;
            const int EXPECTED_END_COUNT = 6;

            TransformationSet set = _DemiContext.TransformationSets.Where(ts => ts.Name == SET_NAME).FirstOrDefault();
            Assert.AreEqual(EXPECTED_START_COUNT, set.Transformations.Count);

            _TransformationService.CreateTransformationSet(USER_NAME, SET_NAME, DEST_FILE);

            _DemiContext = new DEMIContext();
            set = _DemiContext.TransformationSets.Where(ts => ts.Name == SET_NAME).FirstOrDefault();

            Assert.AreEqual(EXPECTED_END_COUNT, set.Transformations.Count);
            Assert.IsTrue(set.Id == SET_ID);
        }

        [TestMethod]
        public void CreateTransformationSet()
        {
            const string DEST_FILE = TEST_FILE_FOLDER + @"03_CreateTransformationSet_Destination.xlsx";
            const string SET_NAME = "UnitTest_CreateTransformationSet";
            const int EXPECTED_COUNT = 10;

            if (_DemiContext.TransformationSets.Any(ts => ts.Name == SET_NAME))
            {
                Assert.Inconclusive("Delete set ({0}) before running this test", SET_NAME);
            }
            else
            {
                _TransformationService.CreateTransformationSet(USER_NAME, SET_NAME, DEST_FILE);

                TransformationSet newSet = _DemiContext.TransformationSets.Where(ts => ts.Name == SET_NAME).FirstOrDefault();
                Assert.IsNotNull(newSet, "New set ({0}) not created", SET_NAME);

                IList<Transformation> transformations = _DemiContext.Transformations.Where(t => t.TransformationSetId == newSet.Id).ToList();
                Assert.AreEqual(EXPECTED_COUNT, transformations.Count);
            }
        }

        [TestMethod]
        public void GetDataTableFromExcelTest()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"04_GetDataTableFromExcelTest_Source.xlsx";
            const int EXPECTED_ROW_COUNT = 5;

            DataTable table = _ExcelService.GetDataTableFromExcel(SOURCE_FILE, FileType.Source);

            Assert.IsNotNull(table);
            Assert.AreEqual(EXPECTED_ROW_COUNT, table.Rows.Count);
        }

        [TestMethod]
        public void RunTransformationsNoFormulas()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"05_RunTransformations_Source.xlsx";
            const string DESTINATION_FILE = TEST_FILE_FOLDER + @"00_Position_Level_Data_Target.xlsx";
            const int SET_ID = 38; //UnitTest_RunTransformationsNoFormulas

            ProcessSourceFile(SOURCE_FILE);
            ProcessDestinationFile(DESTINATION_FILE, SET_ID);

            //TODO: Assert result
        }

        [TestMethod]
        public void RunTransformationsWithFormulas()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"05_RunTransformations_Source.xlsx";
            const string DESTINATION_FILE = TEST_FILE_FOLDER + @"00_Position_Level_Data_Target.xlsx";
            const int SET_ID = 47; //UnitTest_RunTransformationsNoFormulas

            ProcessSourceFile(SOURCE_FILE);
            ProcessDestinationFile(DESTINATION_FILE, SET_ID);

            //TODO: Assert result
        }

        [TestMethod]
        public void CreateSourceTableWithVariousDataTypes()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"07_CreateSourceTableWithVariousDataTypes_Source.xlsx";

            DataTable table = ProcessSourceFile(SOURCE_FILE);
        }

        [TestMethod]
        public void CreateSourceTableWithGrussData()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"08_CreateSourceTableWithGrussData.xlsx";
            const int EXPECTED_ROW_COUNT = 1734;

            DataTable table = ProcessSourceFile(SOURCE_FILE);

            Assert.IsNotNull(table);
            Assert.AreEqual(EXPECTED_ROW_COUNT, table.Rows.Count);
        }

        [TestMethod]
        public void CreateSourceTableWithMarathonDataForAIFMD()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"09_CreateSourceTableWithMarathonDataForAIFMD.xlsx";
            const int EXPECTED_ROW_COUNT = 1487;

            DataTable table = ProcessSourceFile(SOURCE_FILE);

            Assert.IsNotNull(table);
            Assert.AreEqual(EXPECTED_ROW_COUNT, table.Rows.Count);
        }

        [TestMethod]
        public void CreateSourceTableWithMarathonDataForFormPF()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"10_CreateSourceTableWithMarathonDataForFormPF.xlsx";
            const int EXPECTED_ROW_COUNT = 3137;

            DataTable table = ProcessSourceFile(SOURCE_FILE);

            Assert.IsNotNull(table);
            Assert.AreEqual(EXPECTED_ROW_COUNT, table.Rows.Count);
        }

        [TestMethod]
        public void CreateOrUpdateTransformationsWithGrussData()
        {
            const string DEST_FILE = TEST_FILE_FOLDER + @"11_CreateOrUpdateTransformationsWithGrussData_Destination.xlsx";
            const string SET_NAME = "UnitTest_CreateOrUpdateTransformationsWithGrussData";
            const int EXPECTED_COUNT = 205;

            _TransformationService.CreateTransformationSet(USER_NAME, SET_NAME, DEST_FILE);

            TransformationSet newSet = _DemiContext.TransformationSets.Where(ts => ts.Name == SET_NAME).FirstOrDefault();
            Assert.IsNotNull(newSet, "New set ({0}) not created", SET_NAME);

            IList<Transformation> transformations = _DemiContext.Transformations.Where(t => t.TransformationSetId == newSet.Id).ToList();
            Assert.AreEqual(EXPECTED_COUNT, transformations.Count);
        }

        [TestMethod]
        public void CreateTargetTableWithVariousDataTypes()
        {
            const string TARGET_FILE = TEST_FILE_FOLDER + @"00_Position_Level_Data_Target.xlsx";

            DataTable table = ProcessDestinationFile(TARGET_FILE, 0);
        }

        [TestMethod]
        public void CreateTargetTableAndRunTransformation()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"08_CreateSourceTableWithGrussData.xlsx";
            const string TARGET_FILE = TEST_FILE_FOLDER + @"00_Position_Level_Data_Target.xlsx";
            const int SET_ID = 47; //UnitTest_RunTransformationsWithFormulas

            ProcessSourceFile(SOURCE_FILE);
            ProcessDestinationFile(TARGET_FILE, SET_ID);
        }

        [TestMethod]
        public void RunTransformationWithLookupReplace()
        {
            const string SOURCE_FILE = TEST_FILE_FOLDER + @"08_CreateSourceTableWithGrussData.xlsx";
            const string TARGET_FILE = TEST_FILE_FOLDER + @"00_Position_Level_Data_Target.xlsx";
            const int SET_ID = 71; //UnitTest_RunTransformationWithLookupReplace

            ProcessSourceFile(SOURCE_FILE);
            ProcessDestinationFile(TARGET_FILE, SET_ID);
        }

        #region Private Methods

        //[TestMethod]
        public void UseThisToCreateNewTransformationSets()
        {
            //const string DEST_FILE = TEST_FILE_FOLDER + @"Gruss - PLD in IDC.xlsx";
            //const string SET_NAME = "UnitTest_RunTransformationsWithFormulas";
            //_TransformationService.CreateTransformationSet(USER_NAME, SET_NAME, DEST_FILE;)
        }

        private DataTable ProcessSourceFile(string file)
        {
            DataTable result = _ExcelService.GetDataTableFromExcel(file, FileType.Source);

            _DemiContext.CreateSourceDataTable(result, USER_NAME);
            _DemiContext.LoadSourceDataTable(result, USER_NAME);

            return result;
        }

        private DataTable ProcessDestinationFile(string file, int setId)
        {
            DataTable result = _ExcelService.GetDataTableFromExcel(file, FileType.Destination);

            _DemiContext.CreateTransformedDataTable(result, USER_NAME);

            if (setId > 0)
                _DemiContext.LoadTransformedDataTable(setId, USER_NAME);

            return result;
        }

        #endregion

    }
}
