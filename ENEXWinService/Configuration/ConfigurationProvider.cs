using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ENEXWinService.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public void Refresh()
        {
            ConfigurationManager.RefreshSection("appSettings");
        }

        public string GetMeasuresLoaderCronExpression()
        {
            return ConfigurationManager.AppSettings["CronExpression"];
        }

        public string GetFtpAddress()
        {
            return ConfigurationManager.AppSettings["FtpAddress"];
        }

        public string GetFtpRemotePath()
        {
            return ConfigurationManager.AppSettings["FtpRemotePath"];
        }

        public string GetFtpRegExpression()
        {
            return ConfigurationManager.AppSettings["FtpFilenamePattern"];
        }

        public string GetFtpUsername()
        {
            return ConfigurationManager.AppSettings["FtpUsername"];
        }

        public string GetFtpPassword()
        {
            return ConfigurationManager.AppSettings["FtpPassword"];
        }

        public string GetFtpProcessedFilesPath()
        {
            return ConfigurationManager.AppSettings["FtpProcessedFilesPath"];
        }

        public string GetFtpProcessedFileTextToAppend()
        {
            return ConfigurationManager.AppSettings["FtpProcessedFileTextToAppend"];
        }

        public string GetWebApiPlantBaseUri()
        {
            return ConfigurationManager.AppSettings["WebApiPlantBaseUri"];
        }

        public string GetWebApiMeasurePutUri()
        {
            return ConfigurationManager.AppSettings["WebApiMeasurePutUri"];
        }
        public string GetWebApiPlantPowerUri()
        {
            return ConfigurationManager.AppSettings["WebApiPlantPowerUri"];
        }

        public double GetENEXToGnarumOffsetHours()
        {
            return double.Parse(ConfigurationManager.AppSettings["ENEXToGnarumOffsetHours"]);
        }

        public string GetMeasureSourceFor1HResolution()
        {
            return ConfigurationManager.AppSettings["MeasureSource"];
        }

        public double GetMeasureValueMultiplier()
        {
            return double.Parse(ConfigurationManager.AppSettings["MeasureValueMultiplier"]);
        }

        public string GetDataVariable()
        {
            return ConfigurationManager.AppSettings["DataVariable"];
        }

        public string GetHourlyCronExpression()
        {
            return ConfigurationManager.AppSettings["CronExpression"];
        }

        public char GetHourlyPlantsSeparator()
        {
            return ConfigurationManager.AppSettings["PlantsSeparator"].First();
        }

        public string GetHourlyPlantsString()
        {
            return ConfigurationManager.AppSettings["PlantsString"];
        }

        public string GetHourlyDataVariable()
        {
            return ConfigurationManager.AppSettings["DataVariable"];
        }
        public string GetResolution()
        {
            return ConfigurationManager.AppSettings["Resolution"];
        }

        public int GetHourlyNumberOfHoursToProcess()
        {
            return int.Parse(ConfigurationManager.AppSettings["NumberOfHoursToProcess"]);            
        }
        public string GetProcessMeasuresFilesListPath()
        {
            return ConfigurationManager.AppSettings["ProcessMeasuresFilesPath"];
        }
        
    }
}
