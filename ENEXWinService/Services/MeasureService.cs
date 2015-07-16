using System;
using System.Collections.Generic;
using ENEXWinService.Model;
using ENEXWinService.WebApi;

namespace ENEXWinService.Services
{
    public interface IMeasureService
    {
        IList<Measure> GetMeasures(string plant, string source, string datavariable, DateTime firstUtcDateTime, DateTime lastUtcDateTime);
        bool TrySendMeasures(IList<Measure> measures);
    }

    public class MeasureService : IMeasureService
    {
        private static IWebApiClient _apiClient;

        public MeasureService(IWebApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IList<Measure> GetMeasures(string plant, string source, string datavariable, DateTime firstUtcDateTime, DateTime lastUtcDateTime)
        {
            return _apiClient.GetMeasures(plant, source, datavariable, firstUtcDateTime, lastUtcDateTime);
        }

        public bool TrySendMeasures(IList<Measure> measures)
        {
            try
            {
                _apiClient.PutMeasures(measures);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
