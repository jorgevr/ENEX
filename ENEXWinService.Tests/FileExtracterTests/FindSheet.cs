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
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.Globalization;


namespace ENEXWinService.Tests.FileExtracterTests
{
    [TestClass]
    public class FindSheet
    {

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void FindMonth()
        {

            // initialize
            OleDbConnection conn;
            //OleDbDataAdapter adapter;
            DataTable dt = null;

            var file = @"TestData\Production NALBANT Jan-Jul16 2015.xls";

            conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" +
                "Data Source=" + file + ";" +
                "Extended Properties=Excel 8.0");
            conn.Open();

            // Get the data table containg the schema guid.
            dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            // End initialize

            DateTime now = DateTime.Now;
            string monthName = new DateTime(2010, 8, 1)
                .ToString("MMM");
            CultureInfo ru = new CultureInfo("ro-RO");
            string currentMonth = now.ToString("MMM", ru);
            currentMonth = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentMonth.Remove(currentMonth.Length - 1));

            string getMonth = "";

            if (dt != null)
            {
                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    if (excelSheets[i].Length < 13)
                    {
                        if (excelSheets[i].Substring(1,3) == currentMonth)
                        {
                            getMonth = excelSheets[i];
                        }
                    }
                    i++;
                }
            }            

            Assert.AreEqual(getMonth, "'Iul 2015$'");
            
        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void FindYear()
        {

            // initialize
            OleDbConnection conn;
            //OleDbDataAdapter adapter;
            DataTable dt = null;

            var file = @"TestData\Production NALBANT Jan-Jul16 2015.xls";

            conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" +
                "Data Source=" + file + ";" +
                "Extended Properties=Excel 8.0");
            conn.Open();

            // Get the data table containg the schema guid.
            dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            // End initialize

            DateTime now = DateTime.Now;

            CultureInfo ru = new CultureInfo("ro-RO");
            int currentYear = now.Year;
            int getYear = 0;

            if (dt != null)
            {
                String[] excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    // if (excelSheets[i] == rumMonth) break;
                    if (excelSheets[i].Length < 13)
                    {
                        string getDate = excelSheets[i].Substring(5,4);
                        if (Convert.ToInt32(getDate) == currentYear)
                        {
                            getYear = Convert.ToInt32(excelSheets[i].Substring(5, 4));
                        }
                    }
                    i++;
                }
            }


            Assert.AreEqual(getYear, 2015);

        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void FindDateFirstLine()
        {

            // initialize
            OleDbConnection conn;
            //OleDbDataAdapter adapter;
            //DataTable dt = null;
            DataSet ds = new DataSet();
            
            var file = @"TestData\Production NALBANT Jan-Jul16 2015.xls";

            conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;" +
                "Data Source=" + file + ";" +
                "Extended Properties=Excel 8.0");
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;

            // Get the data table containg the schema guid.
            // dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            // End initialize

            string sheetName = "'Iul 2015$'";
            cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

            DataTable dt = new DataTable();
            dt.TableName = sheetName;

            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(dt);
            ds.Tables.Add(dt);

            List<List<string>> data = new List<List<string>>();
            
            foreach (DataRow row in dt.Rows)
            {
                //if (row == null) continue;
                List<string> gettedData = new List<string>();
                int i = 0;
                foreach (var columnitem in row.ItemArray) // Loop over the items.
                {
                    //Console.Write("Item: "); // Print label.
                    //Console.WriteLine(item); // Invokes ToString abstract method.
                    if (i == 0 && columnitem.ToString() == "")
                    {
                    }
                    else
                    {
                        gettedData.Add(columnitem.ToString()); //add data
                    }
                    i++;
                }
                data.Add(gettedData);
            }


            Console.Read();

         //   Assert.AreEqual(getYear, 2015);

        }

    
    }


}
