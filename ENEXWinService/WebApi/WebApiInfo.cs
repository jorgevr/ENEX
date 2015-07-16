using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENEXWinService.Configuration;

namespace ENEXWinService.WebApi
{
    public interface IWebApiInfo
    {
        string PlantBaseUri { get; }
        string MeasurePutUri { get; }
        string PlantPowerUri { get; }

    }

    public class WebApiInfo : IWebApiInfo
    {
        private static string _plantBaseUri;
        private static string _measurePutUri;
        private static string _plantPowerUri;


        public string PlantBaseUri { get { return _plantBaseUri; } }
        public string MeasurePutUri { get { return _measurePutUri; } }
        public string PlantPowerUri { get { return _plantPowerUri; } }


        public WebApiInfo(IConfigurationProvider configProvider)
        {
            _plantBaseUri = configProvider.GetWebApiPlantBaseUri();
            _measurePutUri = configProvider.GetWebApiMeasurePutUri();
            _plantPowerUri = configProvider.GetWebApiPlantPowerUri();
        }

        public WebApiInfo(string plantBaseUri, string measurePutUri, string plantPowerUri)
        {
            _plantBaseUri = plantBaseUri;
            _measurePutUri = measurePutUri;
            _plantPowerUri = plantPowerUri;
        }
    }
}
