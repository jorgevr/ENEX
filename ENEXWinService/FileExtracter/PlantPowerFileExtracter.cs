using System;
using ENEXWinService.Model;
using ENEXWinService.WebApi;
using System.Globalization;
using ENEXWinService.Configuration;
using ENEXWinService.Services;


namespace ENEXWinService.FileExtracter
{
    public interface IPlantPowerFileExtracter
    {
        PlantPower ProcessLine(string line, string fileName, ApiPlant plant);
    }

    public class PlantPowerFileExtracter : IPlantPowerFileExtracter
    {
        private const char LINE_SEPARATOR = ';';
        private CultureInfo CULTURE = new CultureInfo("en-US");

        private const int LINE_LOCALDATE_POSITION = 0;
        private const int LINE_VALUE_POSITION = 2;
        private static IConfigurationProvider _confProvider;
        private static IPlantService _plantService;


        public PlantPowerFileExtracter(IConfigurationProvider configProvider, IPlantService plantService)
        {
            _confProvider = configProvider;
            _plantService = plantService;
        }

        public PlantPower ProcessLine(string line, string fileName, ApiPlant apiPlant)
        {

            var plant = apiPlant;
            var utcDateTime = extractUtcDateFromFileLine(line, plant.TimeZone);
            var utcUpdatedDateTime = string.Format("{0:yyyyMMddHHmmss}", utcDateTime);
            var utcInsertionDateTime = string.Format("{0:yyyyMMddHH0000}", DateTime.UtcNow);
            var power = extractValueFromFileLine(line);
            return new PlantPower(plant.Id, utcInsertionDateTime, utcUpdatedDateTime, power);
        }

        private DateTime extractUtcDateFromFileLine(string line, string plantTimeZone)
        {
            var data = line.Split(LINE_SEPARATOR);
            var localDate = data[LINE_LOCALDATE_POSITION].ToString();
            var localDateTime = localDateTimeFromString(localDate);
            var ENEXToGnarumOffset = _confProvider.GetENEXToGnarumOffsetHours();
            return TimeZoneInfo.ConvertTimeToUtc(localDateTime.AddHours(ENEXToGnarumOffset), TimeZoneInfo.FindSystemTimeZoneById(plantTimeZone));
        }

        private DateTime localDateTimeFromString(string localDate)
        {
            var localYear = int.Parse(localDate.Substring(0, 4));
            var localMonth = int.Parse(localDate.Substring(4, 2));
            var localDay = int.Parse(localDate.Substring(6, 2));
            var localHour = int.Parse(localDate.Substring(8, 2));
            var localMinute = int.Parse(localDate.Substring(10, 2));
            return new DateTime(localYear, localMonth, localDay, localHour, localMinute, 0);
        }


        private double extractValueFromFileLine(string line)
        {
            var data = line.Split(LINE_SEPARATOR);
            var inputValue = double.Parse(data[LINE_VALUE_POSITION], CULTURE);
            var multiplier = _confProvider.GetMeasureValueMultiplier();
            return inputValue * multiplier;
        }

    }
}
