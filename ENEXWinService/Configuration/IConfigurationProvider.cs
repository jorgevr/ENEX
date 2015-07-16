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
        string GetFtpAddress();
        string GetFtpRemotePath();
        string GetFtpRegExpression();
        string GetFtpUsername();
        string GetFtpPassword();
        string GetFtpProcessedFilesPath();
        string GetFtpProcessedFileTextToAppend();
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
