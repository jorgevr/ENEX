using System;
using ENEXWinService.Model;
using ENEXWinService.WebApi;
using System.Globalization;
using ENEXWinService.Configuration;
using ENEXWinService.Services;
using System.Text.RegularExpressions;
using log4net;
using System.Collections.Generic;


namespace ENEXWinService.FileExtracter
{
    public interface IMeasureFileExtracter
    {
        Measure ProcessLine(string line, string fileName, ApiPlant plant);
        String getPlantName(string filePath);
        bool FindSheet(string SheetName);
    }

    public class MeasureFileExtracter : IMeasureFileExtracter
    {
        private const char LINE_SEPARATOR = ' ';
        private CultureInfo CULTURE = new CultureInfo("en-US");

        private const int LINE_LOCALDATE_POSITION = 0;
        private const int LINE_VALUE_POSITION = 3;
        private static IConfigurationProvider _confProvider;
        private static IPlantService _plantService;


        public MeasureFileExtracter(IConfigurationProvider configProvider, IPlantService plantService)
        {
            _confProvider = configProvider;
        }

         
        public bool FindSheet(string SheetName)
        {
            //lets find Current month
            DateTime now = DateTime.Now;
            string monthName = new DateTime(2010, 8, 1)
                .ToString("MMM");
            CultureInfo ru = new CultureInfo("ro-RO");
            string currentMonth = now.ToString("MMM", ru);
            currentMonth = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(currentMonth.Remove(currentMonth.Length - 1));

            // also check that it is the current year
            if (SheetName.Substring(1, 3) == currentMonth && FindCurrentYear( SheetName ) )
            {
                return true;
            }
            else
            {
                return false;
            }            

        }

        private bool FindCurrentYear(string sheetName)
        {
            DateTime now = DateTime.Now;

            CultureInfo ru = new CultureInfo("ro-RO");
            int currentYear = now.Year;
            int sheetYear = Convert.ToInt32(sheetName.Substring(5,4));

            if (sheetYear == currentYear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
         

        public Measure ProcessLine(string line, string fileName, ApiPlant plant)
        {
            //var plant = _plantService.GetPlant(_confProvider.GetHourlyPlantsString());
            //var plant = _plantService.GetPlant(getPlantName(fileName));
            var source = _confProvider.GetMeasureSourceFor1HResolution();
            var datavariable = _confProvider.GetDataVariable();
            var resolution = _confProvider.GetResolution();
            var utcDateTime = extractUtcDateFromFileLine(line, plant.TimeZone, resolution);
            var utcDate = string.Format("{0:yyyyMMdd}", utcDateTime.Date);
            var utcHour = utcDateTime.Hour;
            var utcMinute = utcDateTime.Minute;
            var utcSecond = utcDateTime.Second;
            var value = extractValueFromFileLine(line);
            var percentage = 1;
            var reliability = 0;

            return new Measure(plant.Id, source, datavariable, utcDate, utcHour, utcMinute, utcSecond, value, percentage, reliability, resolution);
        }

        public string getPlantName(string filePath)
        {
            //string fName = filePath.Split('/')[1];
            string fName = filePath;

            string pat = "PotentialEA-";

            string sInput = Regex.Replace(fName, pat, "");
            int index2 = sInput.IndexOf("_");
            string Output = getPlantIdfromENEXPlantName(sInput.Substring(0, index2));

            return Output;
        }

        private string getPlantIdfromENEXPlantName(String input)
        {
            switch (input)
            {
                case "Pestera":
                    return "PEENEX01";
                case "Cernavoda1":
                    return "CEENEX01";
                case "Cernavoda2":
                    return "CEENEX02";
                case "Sarichioi":
                    return "SAENEX01";
                case "Vutcani":
                    return "VUENEX01";
                case "Cobadin":
                    return "COENEX01";
                case "Albesti":
                    return "ALENEX01";
                case "Facaeni":
                    return "FAENEX01";
                case "Vanjumare":
                    return "VAENEX01";
                default:
                    return "unknown";
            }
        }

        private DateTime extractUtcDateFromFileLine(string line, string plantTimeZone, string resolution)
        {

            var data = line.Split(' ');
            var localDate = data[LINE_LOCALDATE_POSITION].ToString();
            var ENEXToGnarumOffset = _confProvider.GetENEXToGnarumOffsetHours();
            var localDateTime = localDateTimeFromString(localDate).AddHours(ENEXToGnarumOffset);
            DateTime gnarumTime = localDateTime.AddHours(-(TimeZoneInfo.FindSystemTimeZoneById(plantTimeZone).BaseUtcOffset.Hours));
            //DateTime gnarumTime = TimeZoneInfo.ConvertTime(localDateTime, timeZone, TimeZoneInfo.Utc);

            //var localDateTimer = TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo.FindSystemTimeZoneById(plantTimeZone));
            //var localDateTimer = localDateTime.AddHours(-2);
            //var localDateTimer = TimeZoneInfo.ConvertTimeToUtc(localDateTime, TimeZoneInfo.FindSystemTimeZoneById(plantTimeZone));

            //return localDateTimer;
            return gnarumTime;

            // return TimeZoneInfo.ConvertTimeToUtc(localDateTime.AddHours(ENEXToGnarumOffset), TimeZoneInfo.FindSystemTimeZoneById(plantTimeZone));
        }

        private DateTime localDateTimeFromString(string localDate)
        {
            var localYear = int.Parse(localDate.Substring(0, 4));
            var localMonth = int.Parse(localDate.Substring(4, 2));
            var localDay = int.Parse(localDate.Substring(6, 2));
            var localHour = int.Parse(localDate.Substring(8, 2));
            //var localMinute = int.Parse(localDate.Substring(10, 2));
            return new DateTime(localYear, localMonth, localDay, localHour, 0, 0);
        }


        private double extractValueFromFileLine(string line)
        {
            var data = line.Split(LINE_SEPARATOR);
            var inputValue = double.Parse(data[LINE_VALUE_POSITION], CultureInfo.InvariantCulture);
            var multiplier = _confProvider.GetMeasureValueMultiplier();
            return inputValue * multiplier;
        }

    }
}
