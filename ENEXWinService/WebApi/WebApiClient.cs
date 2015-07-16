using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENEXWinService.Model;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using log4net;

namespace ENEXWinService.WebApi
{
    public interface IWebApiClient
    {
        ApiPlant GetPlant(string plant);
        IList<PlantPower> GetPlantPower(string plant);
        IList<Measure> GetMeasures(string plant, string source, string datavariable, DateTime firstUtcDateTime, DateTime lastUtcDateTime);
        void PutMeasures(IList<Measure> measures);
        void PutPlantPower(PlantPower plantPower);

    }

    public class WebApiClient : IWebApiClient
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(WebApiClient));
        private static IWebApiInfo _info;

        public WebApiClient(IWebApiInfo info)
        {
            _info = info;
        }

        public ApiPlant GetPlant(string plant)
        {
            _logger.DebugFormat("Searching Plant: {0}", plant);

            Uri uri = new Uri(_info.PlantBaseUri + plant);

            WebRequest webRequest = WebRequest.Create(uri);
            WebResponse response = webRequest.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            String responseData = streamReader.ReadToEnd();

            var plantObject = JsonConvert.DeserializeObject<ApiPlant>(responseData);

            _logger.DebugFormat("Plant {0} found", plant);

            return plantObject;
        }

        public IList<PlantPower> GetPlantPower(string plant)
        {
            _logger.DebugFormat("Searching plantPower for plant {0}", plant);

            var stringUri = getPlantPowerUri(plant);
            Uri uri = new Uri(stringUri);
            WebRequest webRequest = WebRequest.Create(uri);
            WebResponse response = webRequest.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            String responseData = streamReader.ReadToEnd();

            var plantPower = JsonConvert.DeserializeObject<PlantPower[]>(responseData);
            _logger.DebugFormat("Found {0} plantPower", plantPower.Count());

            return plantPower.ToList();
        }

        public void PutPlantPower(PlantPower plantPower)
        {
            _logger.DebugFormat("Converting to JSON plantPower");
            var plantPowerObjects = JsonConvert.SerializeObject(plantPower);
            plantPowerObjects = string.Format("{0}", plantPowerObjects);
            _logger.DebugFormat("PlantPowerconverted. Result: {0}", plantPowerObjects);

            Uri uri = new Uri(_info.PlantPowerUri);
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Method = "PUT";
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = Encoding.GetEncoding("utf-8").GetBytes(plantPowerObjects).Length;

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(plantPowerObjects);
            }

            HttpStatusCode code;
            using (var response = (HttpWebResponse)webRequest.GetResponse())
            {
                code = response.StatusCode;
                response.GetResponseStream();
            }
            if (code != HttpStatusCode.NoContent)
                throw new InvalidOperationException(string.Format("Web Api returned code {0}.", code));
        }



        public void PutMeasures(IList<Measure> measures)
        {
            _logger.DebugFormat("Converting to JSON measures");
            var measureObjects = JsonConvert.SerializeObject(measures);
            measureObjects = string.Format("{{ \"MeasureList\" : {0} }}", measureObjects);
            _logger.DebugFormat("Measures converted. Result: {0}", measureObjects);

            Uri uri = new Uri(_info.MeasurePutUri);
            WebRequest webRequest = WebRequest.Create(uri);
            webRequest.Method = "PUT";
            webRequest.ContentType = "application/json";
            webRequest.ContentLength = Encoding.GetEncoding("utf-8").GetBytes(measureObjects).Length;

            using (var streamWriter = new StreamWriter(webRequest.GetRequestStream()))
            {
                streamWriter.Write(measureObjects);
            }

            HttpStatusCode code;
            using (var response = (HttpWebResponse)webRequest.GetResponse())
            {
                code = response.StatusCode;
                response.GetResponseStream();
            }
            if (code != HttpStatusCode.NoContent)
                throw new InvalidOperationException(string.Format("Web Api returned code {0}.", code));
        }

        public IList<Measure> GetMeasures(string plant, string source, string datavariable, DateTime firstUtcDateTime, DateTime lastUtcDateTime)
        {
            _logger.DebugFormat("Searching Measures for plant {0}, source {1}, datavariable {2}, firstUtcDateTime {3}, lastUtcDateTime {4}"
                , plant, source, datavariable, firstUtcDateTime, lastUtcDateTime);

            var stringUri = getMeasuresUri(plant, source, datavariable, firstUtcDateTime, lastUtcDateTime);
            Uri uri = new Uri(stringUri);
            WebRequest webRequest = WebRequest.Create(uri);
            WebResponse response = webRequest.GetResponse();
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            String responseData = streamReader.ReadToEnd();

            var measures = JsonConvert.DeserializeObject<Measure[]>(responseData);
            _logger.DebugFormat("Found {0} measures", measures.Count());

            return measures.ToList();
        }

        private string getMeasuresUri(string plant, string source, string datavariable, DateTime firstUtcDateTime, DateTime lastUtcDateTime)
        {
            var startUtcDateTimeString = string.Format("{0:yyyyMMddHHmmss}", firstUtcDateTime);
            var endUtcDateTimeString = string.Format("{0:yyyyMMddHHmmss}", lastUtcDateTime);

            var result = string.Format("{0}?plant={1}&source={2}&datavariable={3}&startutcdatetime={4}&endutcdatetime={5}"
                , _info.MeasurePutUri, plant, source, datavariable, startUtcDateTimeString, endUtcDateTimeString);

            _logger.InfoFormat("Using uri: {0}", result);

            return result;
        }
        private string getPlantPowerUri(string plant)
        {
            var result = string.Format("{0}?plant={1}"
                , _info.PlantPowerUri, plant);

            _logger.InfoFormat("Using uri: {0}", result);

            return result;
        }
    }
}
