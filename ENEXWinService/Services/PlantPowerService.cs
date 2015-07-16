using System;
using System.Collections.Generic;

using ENEXWinService.WebApi;
using ENEXWinService.Model;

namespace ENEXWinService.Services
{
    public interface IPlantPowerService
    {
        IList<PlantPower> GetPlantPower(string plant);
        bool TrySendPlantPower(PlantPower plantPower);
        
    }

    public class PlantPowerService : IPlantPowerService
    {
        private static IWebApiClient _apiClient;

        public PlantPowerService(IWebApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public IList<PlantPower> GetPlantPower(string plant)
        {
            return _apiClient.GetPlantPower(plant);
        }

        public  bool TrySendPlantPower(PlantPower plantPower)
        {
            try
            {
                _apiClient.PutPlantPower(plantPower);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
