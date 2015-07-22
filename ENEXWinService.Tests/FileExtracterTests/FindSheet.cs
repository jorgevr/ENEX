using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ENEXWinService.Model;
using ENEXWinService.FileExtracter;
using System;
using ENEXWinService.Services;
using ENEXWinService.Configuration;
using ENEXWinService.Quartz;
using Quartz;
using Moq;
using Quartz.Impl;
using System.Text.RegularExpressions;
using System.Data;
using System.Windows.Forms;
using System.Globalization;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;


namespace ENEXWinService.Tests.FileExtracterTests
{
    [TestClass]
    public class FindSheet
    {

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void FindMonth()
        {

            string filenamePath = @"TestData\Production NALBANT Jan-Jul16 2015.xls";
            FileStream _fileStream = new FileStream(filenamePath, FileMode.Open,
                                      FileAccess.Read);

            IWorkbook _workbook = WorkbookFactory.Create(_fileStream);
            _fileStream.Close();

            // End initialize

            DateTime now = DateTime.Now;
            string monthName = new DateTime(2010, 8, 1)
                .ToString("MMM");
            CultureInfo ru = new CultureInfo("ro-RO");
            string currentMonth = now.ToString("MMM", ru);
            currentMonth = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentMonth.Remove(currentMonth.Length - 1));

            string getMonth = "";

            foreach (ISheet sheet in _workbook)
            {
                    
                if (sheet.SheetName.Substring(0, 3) == currentMonth)
                { 
                    getMonth = sheet.SheetName;
                }
                
            }

            Assert.AreEqual(getMonth, "Iul 2015");
            
        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void FindYear()
        {

            string filenamePath = @"TestData\Production NALBANT Jan-Jul16 2015.xls";
            FileStream _fileStream = new FileStream(filenamePath, FileMode.Open,
                                      FileAccess.Read);

            IWorkbook _workbook = WorkbookFactory.Create(_fileStream);
            _fileStream.Close();

            // End initialize

            DateTime now = DateTime.Now;

            CultureInfo ru = new CultureInfo("ro-RO");
            string currentYear = now.Year.ToString();
            string getYear = " ";
            
            foreach (ISheet sheet in _workbook)
            {
                string sheetYear = sheet.SheetName.Substring(4, 4);
                if ( sheetYear == currentYear)
                {
                    getYear = sheetYear;
                }
                
            }
            
            Assert.AreEqual(getYear, "2015");
        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void FindDateFirstLine()
        {

            string filenamePath = @"TestData\Production NALBANT Jan-Jul16 2015.xls";
            FileStream _fileStream = new FileStream(filenamePath, FileMode.Open,
                                      FileAccess.Read);

            IWorkbook _workbook = WorkbookFactory.Create(_fileStream);
            _fileStream.Close();

            //formulas of the Workbook are evaluated and an instance of a data formatter is created
            IFormulaEvaluator formulaEvaluator = new HSSFFormulaEvaluator(_workbook);
            DataFormatter dataFormatter = new HSSFDataFormatter(new CultureInfo("en-US"));

            // End initialize
            string result = " ";
            ISheet _worksheet = _workbook.GetSheet("Iul 2015");
            IRow firstRow = _worksheet.GetRow(0);
            int numrows = firstRow.LastCellNum;
            DateTime now = DateTime.Now;
            foreach (ICell cell in firstRow)
            {

                if(cell.StringCellValue != "")
                {
                    var Title = cell.StringCellValue;
                    var Date = Title.Substring(Title.Length - 10, 10);
                    DateTime dt = Convert.ToDateTime(Date);
                    int currentYear = now.Year;
                    int currentMonth = now.Month;
                    if (dt.Year == currentYear)
                    {
                        if (dt.Month == currentMonth)
                        {
                            result = Date;
                            break;
                        }

                    }
                }
            }

            Assert.AreEqual(result, "31.07.2015");
            
        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void iterateData()
        {

            string filenamePath = @"TestData\Production NALBANT Jan-Jul16 2015.xls";
            FileStream _fileStream = new FileStream(filenamePath, FileMode.Open,
                                      FileAccess.Read);

            IWorkbook _workbook = WorkbookFactory.Create(_fileStream);
            _fileStream.Close();

            //formulas of the Workbook are evaluated and an instance of a data formatter is created
            IFormulaEvaluator formulaEvaluator = new HSSFFormulaEvaluator(_workbook);
            DataFormatter dataFormatter = new HSSFDataFormatter(new CultureInfo("en-US"));

            // End initialize
            ISheet _worksheet = _workbook.GetSheet("Iul 2015");
            int FirstRow = 0;
            int LastDay = 0;
            foreach (IRow row in _worksheet)
            {
                 //Index to find sheet header (first row)
                if (row.GetCell(0) != null && row.GetCell(1).NumericCellValue == 1) 
                {
                    FirstRow = row.RowNum;
                    int c = 1; 
                     
                    //Index to find last column (day) with value
                    while (row.GetCell(c).NumericCellValue <= 31)
                    {
                        LastDay = row.GetCell(c).ColumnIndex;
                        if(_worksheet.GetRow(FirstRow + 24).GetCell(c).NumericCellValue == 0)
                        {
                            break;
                        }
                        c++;
                    }
                    break;
                }
            }
            
            DataTable results = new DataTable();
            results.Columns.Add("Date", typeof(DateTime));
            results.Columns.Add("Value", typeof(double));
            DateTime now = DateTime.Now;
            DateTime ProdDate = new DateTime(now.Year, now.Month, 1);
            DateTime InitialDate = new DateTime(now.Year, now.Month, 1);   
            bool TheEnd = false;
            double Data = 0;
            //start iteration per day
            for (int day = 1; day <= LastDay; day++)
            {
                for (int hour = 1; hour <= 24; hour++)
                {
                    if (_worksheet.GetRow(FirstRow + hour).GetCell(day) == null)
                    {
                        TheEnd = true;
                        break;
                    }
                   // ProdDate = ProdDate(now.Year, now.Month, 1);
                    ProdDate = InitialDate.Date.AddDays(day - 1).AddHours(hour);
                    Data = _worksheet.GetRow(FirstRow + hour).GetCell(day).NumericCellValue;
                    results.Rows.Add(ProdDate, Data);
                }

                if (TheEnd == true)
                {
                    break;
                }
                
            }
       
                Assert.AreEqual(Data, 0.254);            
        }
    }

}
