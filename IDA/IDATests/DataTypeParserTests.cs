using System;
using System.Collections.Generic;
using ConceptONE.Infrastructure;
using IDALibrary.DEMI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IDATests
{
    [TestClass]
    public class DataTypeParserTests
    {
        DataTypeParser _Parser;

        [TestInitialize]
        public void Initialize()
        {
            Logger.LogActivity("IDATests - TransformationServiceTests - Start");

            _Parser = new DataTypeParser();
        }

        [TestMethod]
        public void TestNumbers()
        {
            const decimal MAX_DECIMAL = Decimal.MaxValue;

            Type EXPECTED_TYPE = typeof(decimal);
            AssertValues(EXPECTED_TYPE, 
                "1", "2", "3", "4.0", "5.123456798", "-100", MAX_DECIMAL.ToString());
        }

        [TestMethod]
        public void TestStrings()
        {
            Type EXPECTED_TYPE = typeof(string);
            AssertValues(EXPECTED_TYPE, "XXXX", "XXXX", "XXXX");
        }

        [TestMethod]
        public void TestNumbersAndStrings()
        {
            Type EXPECTED_TYPE = typeof(string);
            AssertValues(EXPECTED_TYPE, "1", "XYZ", "300000000");
        }

        [TestMethod]
        public void TestDates()
        {
            Type EXPECTED_TYPE = typeof(DateTime);
            AssertValues(EXPECTED_TYPE,
                "1/1/15", "1/1/2015", "01/01/15", "01/01/2015", 
                "20150101", "2015-01-01", "2015-1-1",
                "2015.01.01", "2015.1.1",
                "2016-02-02 14:27:16.960");
        }

        private void AssertValues(Type expectedType, params string[] values)
        {
            List<string> list = new List<string>(values);
            Type type = _Parser.GetType(list);
            Type EXPECTED_TYPE = typeof(decimal);

            Assert.AreEqual(expectedType, type);
        }

    }
}
