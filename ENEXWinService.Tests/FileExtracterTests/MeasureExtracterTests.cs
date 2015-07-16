using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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

namespace ENEXWinService.Tests.FileExtracterTests
{
    [TestClass]
    public class MeasureExtracterTests
    {

        [TestMethod]
        [DeploymentItem(@"../../TestData/Production NALBANT Jan-Jul16 2015.xls", "TestData")]
        public void ExtractMeasuresFromFile()
        {
            var expected = new List<Measure> {
                    new Measure("MAREE01","CLIENTE1H","E","20130826",1,0,0,6622030,1,0,"1H")
            };

            var file = @"TestData\Production NALBANT Jan-Jul16 2015.xls";
            IList<string> fileList = new List<String>();
            IList<string> lineList = new List<String>();
            fileList.Add(file);

            var mockConfigProvider = new Mock<IConfigurationProvider>();
            mockConfigProvider.Setup(x => x.GetHourlyPlantsString()).Returns("MAREE01");
            mockConfigProvider.Setup(x => x.GetMeasureSourceFor1HResolution()).Returns("CLIENTE1H");
            mockConfigProvider.Setup(x => x.GetDataVariable()).Returns("E");
            mockConfigProvider.Setup(x => x.GetResolution()).Returns("1H");
            mockConfigProvider.Setup(x => x.GetENEXToGnarumOffsetHours()).Returns(-1);
            mockConfigProvider.Setup(x => x.GetMeasureValueMultiplier()).Returns(1);


            var apiPlant = new ApiPlant
            {
                Id = "MAREE01",
                Technology = "EO",
                CountryCode = "ES",
                RegionCode = "28",
                TimeZone = "E. Europe Standard Time",
                Latitude = 40.4293,
                Longitude = -3.6574,
                Power = 22668
            };

            var mockPlantService = new Mock<IPlantService>();
            mockPlantService.Setup(x => x.GetPlant("MAREE01")).Returns(apiPlant);

            var _jobdetail = new JobDetailImpl("jobsettings", typeof(IJob));
            var mockExecutionContext = new Mock<IJobExecutionContext>();
            mockExecutionContext.SetupGet(p => p.JobDetail).Returns(_jobdetail);

            MeasuresLoaderJob measureJob = new MeasuresLoaderJob();
            MeasureFileExtracter measureFile = new MeasureFileExtracter(mockConfigProvider.Object, mockPlantService.Object);

            var result = new List<Measure>();
            foreach (var filePath in fileList)
            {
                var cont = 0;

                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (cont != 0)
                    {
                        var measure = measureFile.ProcessLine(line, file,apiPlant);
                        result.Add(measure);
                    }

                    cont++;
                }
            }


            Assert.AreEqual(expected.Count, result.Count);
            for (int i = 1; i < expected.Count; ++i)
            {
                Assert.AreEqual(expected[i], result[i]);
            }

        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/PotentialEA-Albesti_2015040100.txt", "TestData")]
        public void noOKFiles()
        {
            List<string> list = new List<string>();
            list.Add(@"TestData\PotentialEA-Albesti_2015040100.txt");
            list.Add(@"TestData\PotentialEA-Albesti_2015040100.txt.OK");

            foreach (String file in list)
            {

                string e = file.Substring(file.Length - 2);
            }

        }

        [TestMethod]
        [DeploymentItem(@"../../TestData/PotentialEA-Albesti_2015040100.txt", "TestData")]
        public void getPlantName()
        {
            string pat = "PotentialEA-";

            // Instantiate the regular expression object.
            //Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            string name = "PotentialEA-Albesti_2015040100.txt";
            string sOutput = Regex.Replace(name, pat, "");
            int index = sOutput.IndexOf("_");
            string input = getPlantIdfromENEXPlantName (sOutput.Substring(0, index));
            //System.Text.RegularExpressions.Match m = r.Match(name);

        }

        public string getPlantIdfromENEXPlantName (String input)
        {
            switch (input)
            {
                case "Pestera":
                    return "PEEDP01";
                
                default:
                    return "unknown";
            }


        }


    }


}
