using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ENEXWinService.WebApi;
using ENEXWinService.Model;

namespace ENEXWinService.Services
{
    public interface IPlantService
    {
        double GetPlantPower(string plant);
        ApiPlant GetPlant(string plant);
    }

    public class PlantService : IPlantService
    {
        private static IWebApiClient _apiClient;

        public PlantService(IWebApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public double GetPlantPower(string idPlant)
        {
            var plant = _apiClient.GetPlant(idPlant);
            return plant.Power;
        }
        public ApiPlant GetPlant(string idPlant)
        {
            return _apiClient.GetPlant(idPlant);

        }
    }
}
