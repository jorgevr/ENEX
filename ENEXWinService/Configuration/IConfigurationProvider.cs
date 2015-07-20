using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ENEXWinService.Configuration
{
    public interface IConfigurationProvider
    {
        void Refresh();
        string GetMeasuresLoaderCronExpression();
        string GetDirectoryPathToWatch();
        string GetConnnectionString();
        string GetFilenamePattern();
        string GetRegExpression();
        //string GetFtpUsername();
        //string GetFtpPassword();
        string GetProcessedFilesPath();
        string GetPlantName();
        string GetProcessedFileTextToAppend();
        //string GetFtpProcessedFileTextToAppend();
        string GetWebApiPlantBaseUri();
        string GetWebApiMeasurePutUri();
        string GetWebApiPlantPowerUri();
        double GetENEXToGnarumOffsetHours();
        string GetMeasureSourceFor1HResolution();
        double GetMeasureValueMultiplier();
        string GetDataVariable();
        string GetHourlyCronExpression();
        char GetHourlyPlantsSeparator();
        string GetHourlyPlantsString();
        string GetHourlyDataVariable();
        string GetResolution();
        string GetProcessMeasuresFilesListPath();
        int GetHourlyNumberOfHoursToProcess();

    }
}
